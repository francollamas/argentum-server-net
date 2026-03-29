using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Legacy;

/// <summary>
///     Manages socket connections for the server.
/// </summary>
public class SocketManager
{
    public delegate void ConnectionClosedEventHandler(int socketID);

    public delegate void ConnectionReceivedEventHandler(int socketID, string clientIP);

    public delegate void DataReceivedEventHandler(int socketID, byte[] data);

    public delegate void ServerErrorEventHandler(string errorMessage);

    private static int _nextID = 1;
    private static Socket _listener;
    private static int _listeningPort;

    // Maps socket IDs to socket state objects
    public static Dictionary<int, SocketState> SocketStates = new();

    // Pending operations tracking
    private static readonly HashSet<int> _pendingReceives = new();
    private static readonly HashSet<int> _pendingSends = new();

    // Simple synchronized message queue
    private static readonly Queue<QueuedMessage> _messageQueue = new();
    private static readonly object _messageQueueLock = new();
    private static Timer _messageTimer;

    // Lock objects
    private static readonly object _socketStatesLock = new();
    private static readonly object _pendingReceivesLock = new();
    private static readonly object _pendingSendsLock = new();

    // Flag to control if message processing is enabled
    public static bool ProcessMessagesEnabled = true;

    // Events
    public static event ConnectionReceivedEventHandler ConnectionReceived;
    public static event DataReceivedEventHandler DataReceived;
    public static event ConnectionClosedEventHandler ConnectionClosed;
    public static event ServerErrorEventHandler ServerError;

    /// <summary>
    ///     Initializes the socket server to listen on the specified port.
    /// </summary>
    public static void Initialize(int port)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        _listeningPort = port;

        // Initialize the message queue timer using System.Timers.Timer
        _messageTimer = new Timer(10d); // Check queue every 10ms
        _messageTimer.Elapsed += ProcessMessageQueue;
        _messageTimer.AutoReset = true;
        _messageTimer.Start();

        StartListening();
    }

    /// <summary>
    ///     Process queued messages on the main thread
    /// </summary>
    private static void ProcessMessageQueue(object sender, ElapsedEventArgs e)
    {
        // Skip if processing is disabled
        if (!ProcessMessagesEnabled) return;

        // Process all available messages in one go
        while (true)
        {
            QueuedMessage message = null;

            lock (_messageQueueLock)
            {
                if (_messageQueue.Count > 0)
                    message = _messageQueue.Dequeue();
                else
                    break;
            }

            // Process message on the main thread
            if (message is not null) DataReceived?.Invoke(message.SocketID, message.Data);
        }
    }

    /// <summary>
    ///     Starts listening for incoming connections.
    /// </summary>
    public static void StartListening()
    {
        try
        {
            // Create an endpoint for the server
            var localEndPoint = new IPEndPoint(IPAddress.Any, _listeningPort);

            // Create a TCP/IP socket
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections
            _listener.Bind(localEndPoint);
            _listener.Listen(100);

            // Start an asynchronous socket to listen for connections
            _listener.BeginAccept(AcceptCallback, _listener);
        }
        catch (Exception ex)
        {
            ServerError?.Invoke("Error starting server: " + ex.Message);
        }
    }

    /// <summary>
    ///     Callback method executed when a connection request is received.
    /// </summary>
    private static void AcceptCallback(IAsyncResult ar)
    {
        try
        {
            // Get the socket that handles the client request
            var listener = (Socket)ar.AsyncState;

            // Check if the listener is still valid
            if (listener is null || !listener.IsBound) return;

            var socket = listener.EndAccept(ar);

            // Generate a unique ID for this socket
            var socketID = _nextID;
            Interlocked.Increment(ref _nextID);

            // Create the state object for this client
            var state = new SocketState(socket, socketID);

            // Store the socket state in the dictionary
            lock (_socketStatesLock)
            {
                SocketStates[socketID] = state;
            }

            // Get client IP
            var clientIP = ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();

            // Notify about the new connection
            ConnectionReceived?.Invoke(socketID, clientIP);

            // Begin receiving data from the client
            StartReceive(state);

            // Continue listening for connections
            if (_listener is not null && _listener.IsBound) _listener.BeginAccept(AcceptCallback, _listener);
        }
        catch (ObjectDisposedException ex)
        {
        }
        // Socket was closed
        catch (Exception ex)
        {
            ServerError?.Invoke("Error accepting connection: " + ex.Message);

            // Try to continue listening
            try
            {
                if (_listener is not null && _listener.IsBound) _listener.BeginAccept(AcceptCallback, _listener);
            }
            catch
            {
                // Ignore if we can't continue
            }
        }
    }

    /// <summary>
    ///     Starts an asynchronous receive operation on a socket.
    /// </summary>
    private static void StartReceive(SocketState state)
    {
        // Mark this socket as having a pending receive
        var socketID = state.ID;

        lock (_pendingReceivesLock)
        {
            if (_pendingReceives.Contains(socketID))
                // Already receiving on this socket, don't start another receive
                return;

            _pendingReceives.Add(socketID);
        }

        try
        {
            if (state.Socket is null || !state.Socket.Connected || state.IsClosing)
            {
                // Socket is no longer valid or is being closed, don't start a receive
                lock (_pendingReceivesLock)
                {
                    _pendingReceives.Remove(socketID);
                }

                return;
            }

            // Begin receiving data from the client
            state.Socket.BeginReceive(state.Buffer, 0, SocketState.BufferSize, 0, ReadCallback, state);
        }
        catch (ObjectDisposedException ex)
        {
            // Socket was closed
            lock (_pendingReceivesLock)
            {
                _pendingReceives.Remove(socketID);
            }

            CloseSocketInternal(socketID);
        }
        catch (Exception ex)
        {
            // Error during BeginReceive
            lock (_pendingReceivesLock)
            {
                _pendingReceives.Remove(socketID);
            }

            ServerError?.Invoke("Error starting receive: " + ex.Message);
            CloseSocketInternal(socketID);
        }
    }

    /// <summary>
    ///     Callback method executed when data is read from the client.
    /// </summary>
    private static void ReadCallback(IAsyncResult ar)
    {
        if (ar is null)
            return;

        SocketState state = null;
        var socketID = -1;

        try
        {
            state = (SocketState)ar.AsyncState;
            if (state is null)
                return;

            socketID = state.ID;

            // Remove from pending receives
            lock (_pendingReceivesLock)
            {
                _pendingReceives.Remove(socketID);
            }

            // Check if the socket is valid
            if (state.Socket is null || !state.Socket.Connected || state.IsClosing)
            {
                CloseSocketInternal(socketID);
                return;
            }

            // Read data from the client socket
            var bytesRead = state.Socket.EndReceive(ar);

            if (bytesRead > 0)
            {
                // Create a copy of the data received
                var data = new byte[bytesRead];
                Array.Copy(state.Buffer, data, bytesRead);

                // Add to message queue instead of raising event directly
                EnqueueMessage(socketID, data);

                // Start receiving more data
                StartReceive(state);
            }
            else
            {
                // Connection closed by the client
                CloseSocketInternal(socketID);
            }
        }
        catch (ObjectDisposedException ex)
        {
            // Socket was closed
            if (socketID >= 0)
                CloseSocketInternal(socketID);
        }
        catch (SocketException ex)
        {
            // Socket error
            if (socketID >= 0)
                CloseSocketInternal(socketID);
        }
        catch (Exception ex)
        {
            // Other error
            ServerError?.Invoke("Error reading from socket: " + ex.Message);
            if (socketID >= 0)
                CloseSocketInternal(socketID);
        }
    }

    /// <summary>
    ///     Adds a message to the queue for processing on the main thread
    /// </summary>
    private static void EnqueueMessage(int socketID, byte[] data)
    {
        lock (_messageQueueLock)
        {
            _messageQueue.Enqueue(new QueuedMessage(socketID, data));
        }
    }

    /// <summary>
    ///     Manually process any pending messages
    /// </summary>
    public static void ProcessPendingMessages()
    {
        // Only call this from the main thread
        ProcessMessageQueue(null, null);
    }

    /// <summary>
    ///     Sends data to a client.
    /// </summary>
    public static bool SendData(int socketID, byte[] data)
    {
        if (data is null || data.Length == 0) return true; // Nothing to send, consider it a success

        SocketState state = null;

        lock (_socketStatesLock)
        {
            if (!SocketStates.TryGetValue(socketID, out state)) return false;

            // Don't send data to a socket that's being closed
            if (state.IsClosing) return false;
        }

        // Mark this socket as having a pending send
        lock (_pendingSendsLock)
        {
            _pendingSends.Add(socketID);
        }

        try
        {
            if (state.Socket is null || !state.Socket.Connected)
            {
                lock (_pendingSendsLock)
                {
                    _pendingSends.Remove(socketID);
                }

                CloseSocketInternal(socketID);
                return false;
            }

            state.Socket.BeginSend(data, 0, data.Length, 0, SendCallback, state);
            return true;
        }
        catch (ObjectDisposedException ex)
        {
            // Socket was closed
            lock (_pendingSendsLock)
            {
                _pendingSends.Remove(socketID);
            }

            CloseSocketInternal(socketID);
            return false;
        }
        catch (Exception ex)
        {
            ServerError?.Invoke("Error sending data: " + ex.Message);
            lock (_pendingSendsLock)
            {
                _pendingSends.Remove(socketID);
            }

            CloseSocketInternal(socketID);
            return false;
        }
    }

    /// <summary>
    ///     Callback method executed when data send operation completes.
    /// </summary>
    private static void SendCallback(IAsyncResult ar)
    {
        if (ar is null)
            return;

        SocketState state = null;
        var socketID = -1;

        try
        {
            state = (SocketState)ar.AsyncState;
            if (state is null)
                return;

            socketID = state.ID;

            // Remove from pending sends
            lock (_pendingSendsLock)
            {
                _pendingSends.Remove(socketID);
            }

            // Check if the socket is still valid
            if (state.Socket is null || state.IsClosing) return;

            // Complete sending the data to the remote device
            state.Socket.EndSend(ar);
        }
        catch (ObjectDisposedException ex)
        {
        }
        // Socket was closed
        catch (SocketException ex)
        {
            // Socket error
            if (socketID >= 0)
                CloseSocketInternal(socketID);
        }
        catch (Exception ex)
        {
            ServerError?.Invoke("Error completing send: " + ex.Message);
            if (socketID >= 0)
                CloseSocketInternal(socketID);
        }
    }

    /// <summary>
    ///     Closes a client socket.
    /// </summary>
    public static void CloseSocket(int socketID)
    {
        CloseSocketInternal(socketID);
    }

    /// <summary>
    ///     Internal implementation of closing a socket.
    /// </summary>
    private static void CloseSocketInternal(int socketID)
    {
        SocketState state = null;

        lock (_socketStatesLock)
        {
            if (!SocketStates.TryGetValue(socketID, out state)) return;

            // Mark the socket as closing to prevent further operations
            state.IsClosing = true;
        }

        // Check if there are pending operations on this socket
        var hasPendingOperations = false;

        lock (_pendingReceivesLock)
        {
            hasPendingOperations = hasPendingOperations || _pendingReceives.Contains(socketID);
        }

        lock (_pendingSendsLock)
        {
            hasPendingOperations = hasPendingOperations || _pendingSends.Contains(socketID);
        }

        // If we have pending operations, we'll wait for them to complete
        if (hasPendingOperations)
        {
            // Schedule a check to see if pending operations have finished
            var timer = new Timer(100d); // Check again after 100ms
            timer.Elapsed += (sender, e) =>
            {
                timer.Stop();
                timer.Dispose();
                FinishCloseSocket(socketID);
            };
            timer.AutoReset = false;
            timer.Start();
        }
        else
        {
            // No pending operations, proceed with closing
            FinishCloseSocket(socketID);
        }
    }

    /// <summary>
    ///     Completes the socket closing process after any pending operations are done.
    /// </summary>
    private static void FinishCloseSocket(int socketID)
    {
        SocketState state = null;

        lock (_socketStatesLock)
        {
            if (!SocketStates.TryGetValue(socketID, out state)) return;

            SocketStates.Remove(socketID);
        }

        // Check if there are still pending operations
        var hasPendingOperations = false;

        lock (_pendingReceivesLock)
        {
            hasPendingOperations = hasPendingOperations || _pendingReceives.Contains(socketID);
        }

        lock (_pendingSendsLock)
        {
            hasPendingOperations = hasPendingOperations || _pendingSends.Contains(socketID);
        }

        // If we still have pending operations, wait a bit longer
        if (hasPendingOperations)
        {
            var timer = new Timer(100d); // Check again after 100ms
            timer.Elapsed += (sender, e) =>
            {
                timer.Stop();
                timer.Dispose();
                FinishCloseSocket(socketID);
            };
            timer.AutoReset = false;
            timer.Start();
            return;
        }

        // Actually close the socket
        try
        {
            if (state is not null && state.Socket is not null)
            {
                state.Socket.Shutdown(SocketShutdown.Both);
                state.Socket.Close();
                state.Socket = null;
            }
        }
        catch (ObjectDisposedException ex)
        {
        }
        // Socket was already disposed
        catch (SocketException ex)
        {
        }
        // Socket error during shutdown/close
        catch (Exception ex)
        {
            // Other errors during shutdown/close
        }

        // Notify about the closed connection
        ConnectionClosed?.Invoke(socketID);
    }

    /// <summary>
    ///     Converts a string to a byte array using Windows-1252 encoding.
    /// </summary>
    public static byte[] StringToBytes(string text)
    {
        return Encoding.GetEncoding("Windows-1252").GetBytes(text);
    }

    /// <summary>
    ///     Converts a byte array to a string using Windows-1252 encoding.
    /// </summary>
    public static string BytesToString(byte[] bytes)
    {
        return Encoding.GetEncoding("Windows-1252").GetString(bytes);
    }

    /// <summary>
    ///     Shutdown the socket server.
    /// </summary>
    public static void Shutdown()
    {
        // Stop the message processing timer
        if (_messageTimer is not null)
        {
            _messageTimer.Stop();
            _messageTimer.Elapsed -= ProcessMessageQueue;
            _messageTimer.Dispose();
            _messageTimer = null;
        }

        // Clear the message queue
        lock (_messageQueueLock)
        {
            _messageQueue.Clear();
        }

        // Close all client sockets
        var allSocketIDs = new List<int>();

        lock (_socketStatesLock)
        {
            foreach (var kvp in SocketStates)
                allSocketIDs.Add(kvp.Key);
        }

        foreach (var socketID in allSocketIDs)
            CloseSocketInternal(socketID);

        // Close the listening socket
        try
        {
            if (_listener is not null)
            {
                _listener.Close();
                _listener = null;
            }
        }
        catch
        {
            // Ignore errors during shutdown
        }
    }

    // Simple message structure for the queue
    public class QueuedMessage
    {
        public byte[] Data;
        public int SocketID;

        public QueuedMessage(int socketId, byte[] data)
        {
            SocketID = socketId;
            Data = data;
        }
    }

    // State object for reading client data asynchronously
    public class SocketState
    {
        // Size of receive buffer.
        public const int BufferSize = 8192;

        // Receive buffer.
        public byte[] Buffer = new byte[8193];

        // ID associated with this socket
        public int ID = -1;

        // Flag to track if socket is being closed
        public bool IsClosing;

        // Client socket.
        public Socket Socket;

        public SocketState(Socket socket, int id)
        {
            Socket = socket;
            ID = id;
        }
    }
}