Option Strict On
Option Explicit On

Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Collections.Generic
Imports System.Timers

''' <summary>
''' Manages socket connections for the server.
''' </summary>
Public Class SocketManager
    ' Simple message structure for the queue
    Public Class QueuedMessage
        Public SocketID As Integer
        Public Data() As Byte

        Public Sub New(socketId As Integer, data() As Byte)
            Me.SocketID = socketId
            Me.Data = data
        End Sub
    End Class

    ' State object for reading client data asynchronously
    Public Class SocketState
        ' Client socket.
        Public Socket As Socket = Nothing
        ' Size of receive buffer.
        Public Const BufferSize As Integer = 8192
        ' Receive buffer.
        Public Buffer(BufferSize) As Byte
        ' ID associated with this socket
        Public ID As Integer = - 1
        ' Flag to track if socket is being closed
        Public IsClosing As Boolean = False

        Public Sub New(socket As Socket, id As Integer)
            Me.Socket = socket
            Me.ID = id
        End Sub
    End Class

    Private Shared _nextID As Integer = 1
    Private Shared _listener As Socket = Nothing
    Private Shared _listeningPort As Integer = 0

    ' Maps socket IDs to socket state objects
    Public Shared SocketStates As New Dictionary(Of Integer, SocketState)

    ' Pending operations tracking
    Private Shared _pendingReceives As New HashSet(Of Integer)
    Private Shared _pendingSends As New HashSet(Of Integer)

    ' Simple synchronized message queue
    Private Shared _messageQueue As New Queue(Of QueuedMessage)
    Private Shared _messageQueueLock As New Object()
    Private Shared _messageTimer As System.Timers.Timer

    ' Lock objects
    Private Shared _socketStatesLock As New Object()
    Private Shared _pendingReceivesLock As New Object()
    Private Shared _pendingSendsLock As New Object()

    ' Events
    Public Shared Event ConnectionReceived(socketID As Integer, clientIP As String)
    Public Shared Event DataReceived(socketID As Integer, data() As Byte)
    Public Shared Event ConnectionClosed(socketID As Integer)
    Public Shared Event ServerError(errorMessage As String)

    ' Flag to control if message processing is enabled
    Public Shared ProcessMessagesEnabled As Boolean = True

    ''' <summary>
    ''' Initializes the socket server to listen on the specified port.
    ''' </summary>
    Public Shared Sub Initialize(port As Integer)
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)
        _listeningPort = port

        ' Initialize the message queue timer using System.Timers.Timer
        _messageTimer = New System.Timers.Timer(10) ' Check queue every 10ms
        AddHandler _messageTimer.Elapsed, AddressOf ProcessMessageQueue
        _messageTimer.AutoReset = True
        _messageTimer.Start()

        StartListening()
    End Sub

    ''' <summary>
    ''' Process queued messages on the main thread
    ''' </summary>
    Private Shared Sub ProcessMessageQueue(sender As Object, e As ElapsedEventArgs)
        ' Skip if processing is disabled
        If Not ProcessMessagesEnabled Then
            Return
        End If

        ' Process all available messages in one go
        While True
            Dim message As QueuedMessage = Nothing

            SyncLock _messageQueueLock
                If _messageQueue.Count > 0 Then
                    message = _messageQueue.Dequeue()
                Else
                    Exit While
                End If
            End SyncLock

            ' Process message on the main thread
            If message IsNot Nothing Then
                RaiseEvent DataReceived(message.SocketID, message.Data)
            End If
        End While
    End Sub

    ''' <summary>
    ''' Starts listening for incoming connections.
    ''' </summary>
    Public Shared Sub StartListening()
        Try
            ' Create an endpoint for the server
            Dim localEndPoint As New IPEndPoint(IPAddress.Any, _listeningPort)

            ' Create a TCP/IP socket
            _listener = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)

            ' Bind the socket to the local endpoint and listen for incoming connections
            _listener.Bind(localEndPoint)
            _listener.Listen(100)

            ' Start an asynchronous socket to listen for connections
            _listener.BeginAccept(New AsyncCallback(AddressOf AcceptCallback), _listener)
        Catch ex As Exception
            RaiseEvent ServerError("Error starting server: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Callback method executed when a connection request is received.
    ''' </summary>
    Private Shared Sub AcceptCallback(ar As IAsyncResult)
        Try
            ' Get the socket that handles the client request
            Dim listener As Socket = CType(ar.AsyncState, Socket)

            ' Check if the listener is still valid
            If listener Is Nothing OrElse Not listener.IsBound Then
                Return
            End If

            Dim socket As Socket = listener.EndAccept(ar)

            ' Generate a unique ID for this socket
            Dim socketID As Integer = _nextID
            Interlocked.Increment(_nextID)

            ' Create the state object for this client
            Dim state As New SocketState(socket, socketID)

            ' Store the socket state in the dictionary
            SyncLock _socketStatesLock
                SocketStates(socketID) = state
            End SyncLock

            ' Get client IP
            Dim clientIP As String = CType(socket.RemoteEndPoint, IPEndPoint).Address.ToString()

            ' Notify about the new connection
            RaiseEvent ConnectionReceived(socketID, clientIP)

            ' Begin receiving data from the client
            StartReceive(state)

            ' Continue listening for connections
            If _listener IsNot Nothing AndAlso _listener.IsBound Then
                _listener.BeginAccept(New AsyncCallback(AddressOf AcceptCallback), _listener)
            End If
        Catch ex As ObjectDisposedException
            ' Socket was closed
        Catch ex As Exception
            RaiseEvent ServerError("Error accepting connection: " & ex.Message)

            ' Try to continue listening
            Try
                If _listener IsNot Nothing AndAlso _listener.IsBound Then
                    _listener.BeginAccept(New AsyncCallback(AddressOf AcceptCallback), _listener)
                End If
            Catch
                ' Ignore if we can't continue
            End Try
        End Try
    End Sub

    ''' <summary>
    ''' Starts an asynchronous receive operation on a socket.
    ''' </summary>
    Private Shared Sub StartReceive(state As SocketState)
        ' Mark this socket as having a pending receive
        Dim socketID As Integer = state.ID

        SyncLock _pendingReceivesLock
            If _pendingReceives.Contains(socketID) Then
                ' Already receiving on this socket, don't start another receive
                Return
            End If

            _pendingReceives.Add(socketID)
        End SyncLock

        Try
            If state.Socket Is Nothing OrElse Not state.Socket.Connected OrElse state.IsClosing Then
                ' Socket is no longer valid or is being closed, don't start a receive
                SyncLock _pendingReceivesLock
                    _pendingReceives.Remove(socketID)
                End SyncLock
                Return
            End If

            ' Begin receiving data from the client
            state.Socket.BeginReceive(state.Buffer, 0, SocketState.BufferSize, 0,
                                      New AsyncCallback(AddressOf ReadCallback), state)
        Catch ex As ObjectDisposedException
            ' Socket was closed
            SyncLock _pendingReceivesLock
                _pendingReceives.Remove(socketID)
            End SyncLock
            CloseSocketInternal(socketID)
        Catch ex As Exception
            ' Error during BeginReceive
            SyncLock _pendingReceivesLock
                _pendingReceives.Remove(socketID)
            End SyncLock
            RaiseEvent ServerError("Error starting receive: " & ex.Message)
            CloseSocketInternal(socketID)
        End Try
    End Sub

    ''' <summary>
    ''' Callback method executed when data is read from the client.
    ''' </summary>
    Private Shared Sub ReadCallback(ar As IAsyncResult)
        If ar Is Nothing Then Return

        Dim state As SocketState = Nothing
        Dim socketID As Integer = - 1

        Try
            state = CType(ar.AsyncState, SocketState)
            If state Is Nothing Then Return

            socketID = state.ID

            ' Remove from pending receives
            SyncLock _pendingReceivesLock
                _pendingReceives.Remove(socketID)
            End SyncLock

            ' Check if the socket is valid
            If state.Socket Is Nothing OrElse Not state.Socket.Connected OrElse state.IsClosing Then
                CloseSocketInternal(socketID)
                Return
            End If

            ' Read data from the client socket
            Dim bytesRead As Integer = state.Socket.EndReceive(ar)

            If bytesRead > 0 Then
                ' Create a copy of the data received
                Dim data(bytesRead - 1) As Byte
                Array.Copy(state.Buffer, data, bytesRead)

                ' Add to message queue instead of raising event directly
                EnqueueMessage(socketID, data)

                ' Start receiving more data
                StartReceive(state)
            Else
                ' Connection closed by the client
                CloseSocketInternal(socketID)
            End If
        Catch ex As ObjectDisposedException
            ' Socket was closed
            If socketID >= 0 Then CloseSocketInternal(socketID)
        Catch ex As SocketException
            ' Socket error
            If socketID >= 0 Then CloseSocketInternal(socketID)
        Catch ex As Exception
            ' Other error
            RaiseEvent ServerError("Error reading from socket: " & ex.Message)
            If socketID >= 0 Then CloseSocketInternal(socketID)
        End Try
    End Sub

    ''' <summary>
    ''' Adds a message to the queue for processing on the main thread
    ''' </summary>
    Private Shared Sub EnqueueMessage(socketID As Integer, data() As Byte)
        SyncLock _messageQueueLock
            _messageQueue.Enqueue(New QueuedMessage(socketID, data))
        End SyncLock
    End Sub

    ''' <summary>
    ''' Manually process any pending messages
    ''' </summary>
    Public Shared Sub ProcessPendingMessages()
        ' Only call this from the main thread
        ProcessMessageQueue(Nothing, Nothing)
    End Sub

    ''' <summary>
    ''' Sends data to a client.
    ''' </summary>
    Public Shared Function SendData(socketID As Integer, data() As Byte) As Boolean
        If data Is Nothing OrElse data.Length = 0 Then
            Return True ' Nothing to send, consider it a success
        End If

        Dim state As SocketState = Nothing

        SyncLock _socketStatesLock
            If Not SocketStates.TryGetValue(socketID, state) Then
                Return False
            End If

            ' Don't send data to a socket that's being closed
            If state.IsClosing Then
                Return False
            End If
        End SyncLock

        ' Mark this socket as having a pending send
        SyncLock _pendingSendsLock
            _pendingSends.Add(socketID)
        End SyncLock

        Try
            If state.Socket Is Nothing OrElse Not state.Socket.Connected Then
                SyncLock _pendingSendsLock
                    _pendingSends.Remove(socketID)
                End SyncLock
                CloseSocketInternal(socketID)
                Return False
            End If

            state.Socket.BeginSend(data, 0, data.Length, 0, New AsyncCallback(AddressOf SendCallback), state)
            Return True
        Catch ex As ObjectDisposedException
            ' Socket was closed
            SyncLock _pendingSendsLock
                _pendingSends.Remove(socketID)
            End SyncLock
            CloseSocketInternal(socketID)
            Return False
        Catch ex As Exception
            RaiseEvent ServerError("Error sending data: " & ex.Message)
            SyncLock _pendingSendsLock
                _pendingSends.Remove(socketID)
            End SyncLock
            CloseSocketInternal(socketID)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Callback method executed when data send operation completes.
    ''' </summary>
    Private Shared Sub SendCallback(ar As IAsyncResult)
        If ar Is Nothing Then Return

        Dim state As SocketState = Nothing
        Dim socketID As Integer = - 1

        Try
            state = CType(ar.AsyncState, SocketState)
            If state Is Nothing Then Return

            socketID = state.ID

            ' Remove from pending sends
            SyncLock _pendingSendsLock
                _pendingSends.Remove(socketID)
            End SyncLock

            ' Check if the socket is still valid
            If state.Socket Is Nothing OrElse state.IsClosing Then
                Return
            End If

            ' Complete sending the data to the remote device
            state.Socket.EndSend(ar)
        Catch ex As ObjectDisposedException
            ' Socket was closed
        Catch ex As SocketException
            ' Socket error
            If socketID >= 0 Then CloseSocketInternal(socketID)
        Catch ex As Exception
            RaiseEvent ServerError("Error completing send: " & ex.Message)
            If socketID >= 0 Then CloseSocketInternal(socketID)
        End Try
    End Sub

    ''' <summary>
    ''' Closes a client socket.
    ''' </summary>
    Public Shared Sub CloseSocket(socketID As Integer)
        CloseSocketInternal(socketID)
    End Sub

    ''' <summary>
    ''' Internal implementation of closing a socket.
    ''' </summary>
    Private Shared Sub CloseSocketInternal(socketID As Integer)
        Dim state As SocketState = Nothing

        SyncLock _socketStatesLock
            If Not SocketStates.TryGetValue(socketID, state) Then
                Return
            End If

            ' Mark the socket as closing to prevent further operations
            state.IsClosing = True
        End SyncLock

        ' Check if there are pending operations on this socket
        Dim hasPendingOperations As Boolean = False

        SyncLock _pendingReceivesLock
            hasPendingOperations = hasPendingOperations OrElse _pendingReceives.Contains(socketID)
        End SyncLock

        SyncLock _pendingSendsLock
            hasPendingOperations = hasPendingOperations OrElse _pendingSends.Contains(socketID)
        End SyncLock

        ' If we have pending operations, we'll wait for them to complete
        If hasPendingOperations Then
            ' Schedule a check to see if pending operations have finished
            Dim timer As New System.Timers.Timer(100) ' Check again after 100ms
            AddHandler timer.Elapsed, Sub(sender, e)
                timer.Stop()
                timer.Dispose()
                FinishCloseSocket(socketID)
            End Sub
            timer.AutoReset = False
            timer.Start()
        Else
            ' No pending operations, proceed with closing
            FinishCloseSocket(socketID)
        End If
    End Sub

    ''' <summary>
    ''' Completes the socket closing process after any pending operations are done.
    ''' </summary>
    Private Shared Sub FinishCloseSocket(socketID As Integer)
        Dim state As SocketState = Nothing

        SyncLock _socketStatesLock
            If Not SocketStates.TryGetValue(socketID, state) Then
                Return
            End If

            SocketStates.Remove(socketID)
        End SyncLock

        ' Check if there are still pending operations
        Dim hasPendingOperations As Boolean = False

        SyncLock _pendingReceivesLock
            hasPendingOperations = hasPendingOperations OrElse _pendingReceives.Contains(socketID)
        End SyncLock

        SyncLock _pendingSendsLock
            hasPendingOperations = hasPendingOperations OrElse _pendingSends.Contains(socketID)
        End SyncLock

        ' If we still have pending operations, wait a bit longer
        If hasPendingOperations Then
            Dim timer As New System.Timers.Timer(100) ' Check again after 100ms
            AddHandler timer.Elapsed, Sub(sender, e)
                timer.Stop()
                timer.Dispose()
                FinishCloseSocket(socketID)
            End Sub
            timer.AutoReset = False
            timer.Start()
            Return
        End If

        ' Actually close the socket
        Try
            If state IsNot Nothing AndAlso state.Socket IsNot Nothing Then
                state.Socket.Shutdown(SocketShutdown.Both)
                state.Socket.Close()
                state.Socket = Nothing
            End If
        Catch ex As ObjectDisposedException
            ' Socket was already disposed
        Catch ex As SocketException
            ' Socket error during shutdown/close
        Catch ex As Exception
            ' Other errors during shutdown/close
        End Try

        ' Notify about the closed connection
        RaiseEvent ConnectionClosed(socketID)
    End Sub

    ''' <summary>
    ''' Converts a string to a byte array using Windows-1252 encoding.
    ''' </summary>
    Public Shared Function StringToBytes(text As String) As Byte()
        Return Encoding.GetEncoding("Windows-1252").GetBytes(text)
    End Function

    ''' <summary>
    ''' Converts a byte array to a string using Windows-1252 encoding.
    ''' </summary>
    Public Shared Function BytesToString(bytes() As Byte) As String
        Return Encoding.GetEncoding("Windows-1252").GetString(bytes)
    End Function

    ''' <summary>
    ''' Shutdown the socket server.
    ''' </summary>
    Public Shared Sub Shutdown()
        ' Stop the message processing timer
        If _messageTimer IsNot Nothing Then
            _messageTimer.Stop()
            RemoveHandler _messageTimer.Elapsed, AddressOf ProcessMessageQueue
            _messageTimer.Dispose()
            _messageTimer = Nothing
        End If

        ' Clear the message queue
        SyncLock _messageQueueLock
            _messageQueue.Clear()
        End SyncLock

        ' Close all client sockets
        Dim allSocketIDs As New List(Of Integer)

        SyncLock _socketStatesLock
            Dim kvp As KeyValuePair(Of Integer, SocketState)
            For Each kvp In SocketStates
                allSocketIDs.Add(kvp.Key)
            Next
        End SyncLock

        Dim socketID As Integer
        For Each socketID In allSocketIDs
            CloseSocketInternal(socketID)
        Next

        ' Close the listening socket
        Try
            If _listener IsNot Nothing Then
                _listener.Close()
                _listener = Nothing
            End If
        Catch
            ' Ignore errors during shutdown
        End Try
    End Sub
End Class