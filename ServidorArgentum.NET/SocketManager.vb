Option Strict On
Option Explicit On

Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports System.Collections.Generic
Imports System.Windows.Forms

''' <summary>
''' Manages socket connections for the server.
''' </summary>
Public Class SocketManager
    ' State object for reading client data asynchronously
    Public Class SocketState
        ' Client socket.
        Public Socket As Socket = Nothing
        ' Size of receive buffer.
        Public Const BufferSize As Integer = 8192
        ' Receive buffer.
        Public Buffer(BufferSize) As Byte
        ' ID associated with this socket
        Public ID As Integer = -1
        
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
    
    ' Synchronization context for switching back to the UI thread
    Private Shared _syncContext As SynchronizationContext = Nothing
    
    ' Events
    Public Shared Event ConnectionReceived(socketID As Integer, clientIP As String)
    Public Shared Event DataReceived(socketID As Integer, data() As Byte)
    Public Shared Event ConnectionClosed(socketID As Integer)
    Public Shared Event ServerError(errorMessage As String)

    ''' <summary>
    ''' Initializes the socket server to listen on the specified port.
    ''' </summary>
    Public Shared Sub Initialize(port As Integer)
        ' Store the synchronization context for later use
        _syncContext = SynchronizationContext.Current
        If _syncContext Is Nothing Then
            ' If we don't have a synchronization context, create one for Windows Forms
            _syncContext = New WindowsFormsSynchronizationContext()
        End If

        _listeningPort = port
        StartListening()
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
            OnServerError("Error starting server: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Callback method executed when a connection request is received.
    ''' </summary>
    Private Shared Sub AcceptCallback(ar As IAsyncResult)
        Try
            ' Get the socket that handles the client request
            Dim listener As Socket = CType(ar.AsyncState, Socket)
            Dim socket As Socket = listener.EndAccept(ar)

            ' Generate a unique ID for this socket
            Dim socketID As Integer = _nextID
            Interlocked.Increment(_nextID)

            ' Create the state object for this client
            Dim state As New SocketState(socket, socketID)
            
            ' Store the socket state in the dictionary
            SyncLock SocketStates
                SocketStates(socketID) = state
            End SyncLock

            ' Get client IP
            Dim clientIP As String = CType(socket.RemoteEndPoint, IPEndPoint).Address.ToString()
            
            ' Notify about the new connection on the main thread
            _syncContext.Post(Sub(s) RaiseEvent ConnectionReceived(socketID, clientIP), Nothing)

            ' Begin receiving data from the client
            socket.BeginReceive(state.Buffer, 0, SocketState.BufferSize, 0, New AsyncCallback(AddressOf ReadCallback), state)

            ' Continue listening for connections
            _listener.BeginAccept(New AsyncCallback(AddressOf AcceptCallback), _listener)
        Catch ex As ObjectDisposedException
            ' Socket was closed
        Catch ex As Exception
            OnServerError("Error accepting connection: " & ex.Message)
            
            ' Try to continue listening
            Try
                _listener.BeginAccept(New AsyncCallback(AddressOf AcceptCallback), _listener)
            Catch
                ' Ignore if we can't continue
            End Try
        End Try
    End Sub

    ''' <summary>
    ''' Callback method executed when data is read from the client.
    ''' </summary>
    Private Shared Sub ReadCallback(ar As IAsyncResult)
        Dim state As SocketState = CType(ar.AsyncState, SocketState)
        Dim socket As Socket = state.Socket
        Dim socketID As Integer = state.ID

        Try
            ' Read data from the client socket
            Dim bytesRead As Integer = socket.EndReceive(ar)

            If bytesRead > 0 Then
                ' Create a copy of the data received
                Dim data(bytesRead - 1) As Byte
                Array.Copy(state.Buffer, data, bytesRead)
                
                ' Process the data on the main thread
                _syncContext.Post(Sub(s) RaiseEvent DataReceived(socketID, data), Nothing)
                
                ' Get ready to receive more data
                socket.BeginReceive(state.Buffer, 0, SocketState.BufferSize, 0, New AsyncCallback(AddressOf ReadCallback), state)
            Else
                ' Connection closed by the client
                CloseSocketInternal(socketID)
            End If
        Catch ex As ObjectDisposedException
            ' Socket was closed
            CloseSocketInternal(socketID)
        Catch ex As SocketException
            ' Socket error
            CloseSocketInternal(socketID)
        Catch ex As Exception
            OnServerError("Error reading from socket: " & ex.Message)
            CloseSocketInternal(socketID)
        End Try
    End Sub

    ''' <summary>
    ''' Sends data to a client.
    ''' </summary>
    Public Shared Function SendData(socketID As Integer, data() As Byte) As Boolean
        Try
            Dim state As SocketState = Nothing
            
            SyncLock SocketStates
                If Not SocketStates.TryGetValue(socketID, state) Then
                    Return False
                End If
            End SyncLock
            
            state.Socket.BeginSend(data, 0, data.Length, 0, New AsyncCallback(AddressOf SendCallback), state)
            Return True
        Catch ex As Exception
            OnServerError("Error sending data: " & ex.Message)
            CloseSocket(socketID)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Callback method executed when data send operation completes.
    ''' </summary>
    Private Shared Sub SendCallback(ar As IAsyncResult)
        Try
            Dim state As SocketState = CType(ar.AsyncState, SocketState)
            
            ' Complete sending the data to the remote device
            state.Socket.EndSend(ar)
        Catch ex As Exception
            ' Error occurred during the send operation
            Dim state As SocketState = CType(ar.AsyncState, SocketState)
            CloseSocketInternal(state.ID)
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
        
        SyncLock SocketStates
            If Not SocketStates.TryGetValue(socketID, state) Then
                Return
            End If
            
            SocketStates.Remove(socketID)
        End SyncLock
        
        Try
            If state IsNot Nothing AndAlso state.Socket IsNot Nothing Then
                state.Socket.Close()
            End If
        Catch
            ' Ignore errors during socket closing
        End Try
        
        ' Notify about the closed connection on the main thread
        _syncContext.Post(Sub(s) RaiseEvent ConnectionClosed(socketID), Nothing)
    End Sub

    ''' <summary>
    ''' Converts a string to a byte array using Windows-1252 encoding.
    ''' </summary>
    Public Shared Function StringToBytes(text As String) As Byte()
        Return Encoding.GetEncoding("Windows-1252").GetBytes(text)
    End Function

    ''' <summary>
    ''' Shutdown the socket server.
    ''' </summary>
    Public Shared Sub Shutdown()
        ' Close all client sockets
        Dim allSocketIDs As New List(Of Integer)

        SyncLock SocketStates
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
    
    ''' <summary>
    ''' Raises the ServerError event.
    ''' </summary>
    Private Shared Sub OnServerError(errorMessage As String)
        _syncContext.Post(Sub(s) RaiseEvent ServerError(errorMessage), Nothing)
    End Sub
    
    ''' <summary>
    ''' Converts a byte array to a string using Windows-1252 encoding.
    ''' </summary>
    Public Shared Function BytesToString(bytes() As Byte) As String
        Return Encoding.GetEncoding("Windows-1252").GetString(bytes)
    End Function
End Class