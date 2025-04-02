Imports System.Collections.Generic
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Public Class SocketManager
    Private _listener As TcpListener
    Private _clients As New Dictionary(Of Integer, TcpClient)
    Private _clientStreams As New Dictionary(Of Integer, NetworkStream)
    Private _clientCounter As Integer = 1
    Private _maxClients As Integer
    Private _port As Integer
    Private _isListening As Boolean = False
    Private _socketLock As New Object() ' Lock object for thread safety

    ' Events for connecting module hooks
    Public Event ConnectionAccepted(ByVal socketId As Integer, ByVal client As TcpClient)
    Public Event DataReceived(ByVal socketId As Integer, ByVal data() As Byte, ByVal bytesTotal As Integer)
    Public Event ClientDisconnected(ByVal socketId As Integer)

    Public Sub New(ByVal port As Integer, ByVal maxClients As Integer)
        _port = port
        _maxClients = maxClients
    End Sub

    Public Sub StartListening()
        If _isListening Then Return

        ' Reset client tracking
        SyncLock _socketLock
            _clients.Clear()
            _clientStreams.Clear()
            _clientCounter = 1
        End SyncLock

        Try
            _listener = New TcpListener(IPAddress.Any, _port)
            _listener.Start()
            _isListening = True

            ' Start accepting connections asynchronously
            Dim t As New Thread(AddressOf AcceptConnections)
            t.IsBackground = True
            t.Start()

            Debug.Print("Socket server started listening on port " & _port)
        Catch ex As Exception
            Debug.Print("Failed to start listening: " & ex.Message)
        End Try
    End Sub

    Public Sub StopListening()
        _isListening = False
        Debug.Print("Stopping socket server...")

        Try
            If _listener IsNot Nothing Then
                _listener.Stop()
            End If
        Catch ex As Exception
            Debug.Print("Error stopping listener: " & ex.Message)
        End Try

        ' Close all client connections
        SyncLock _socketLock
            Dim socketIds As New List(Of Integer)(_clients.Keys)
            For Each socketId As Integer In socketIds
                Try
                    CloseSocket(socketId)
                Catch ex As Exception
                    Debug.Print("Error closing socket " & socketId & ": " & ex.Message)
                End Try
            Next
        End SyncLock
    End Sub

    Private Sub AcceptConnections()
        Debug.Print("AcceptConnections thread started")

        While _isListening
            Try
                Dim client As TcpClient = _listener.AcceptTcpClient()
                Debug.Print("New connection accepted from " & DirectCast(client.Client.RemoteEndPoint, IPEndPoint).Address.ToString())

                ' Configure client with more generous timeout
                client.ReceiveBufferSize = 8192
                client.SendBufferSize = 8192
                client.NoDelay = True
                client.ReceiveTimeout = 30000  ' 30 seconds timeout
                client.SendTimeout = 30000     ' 30 seconds timeout

                Dim socketId As Integer = 0

                SyncLock _socketLock
                    ' Simple ID assignment - just use the next available number
                    For i As Integer = 1 To _maxClients
                        If Not _clients.ContainsKey(i) Then
                            socketId = i
                            Exit For
                        End If
                    Next

                    If socketId > 0 AndAlso socketId <= _maxClients Then
                        Dim stream As NetworkStream = client.GetStream()
                        _clients.Add(socketId, client)
                        _clientStreams.Add(socketId, stream)
                        Debug.Print("Assigned socket ID: " & socketId)
                    Else
                        ' No IDs available
                        socketId = -1
                    End If
                End SyncLock

                If socketId > 0 Then
                    ' Give a short delay to ensure everything is properly set up
                    Thread.Sleep(50)

                    ' Start listening for data from this client - using a separate thread
                    Dim t As New Thread(Sub() ListenForData(socketId))
                    t.IsBackground = True
                    t.Start()

                    ' Short delay before raising event to ensure listener is ready
                    Thread.Sleep(50)

                    ' Raise event after everything is set up
                    RaiseEvent ConnectionAccepted(socketId, client)
                Else
                    ' Server is full, close connection
                    Debug.Print("Server full, rejecting connection")
                    client.Close()
                End If
            Catch ex As Exception
                If _isListening Then
                    Debug.Print("Error accepting connection: " & ex.Message)
                    ' Short pause to prevent CPU thrashing if there's a persistent error
                    Thread.Sleep(100)
                End If
            End Try
        End While

        Debug.Print("AcceptConnections thread ending")
    End Sub

    Private Sub ListenForData(ByVal socketId As Integer)
        Debug.Print("ListenForData thread started for socket " & socketId)

        Dim buffer(8192) As Byte
        Dim stream As NetworkStream = Nothing

        ' Get the stream outside the main loop
        SyncLock _socketLock
            If _clientStreams.ContainsKey(socketId) Then
                stream = _clientStreams(socketId)
            End If
        End SyncLock

        If stream Is Nothing Then
            Debug.Print("Stream for socket " & socketId & " not found, closing connection")
            CloseSocket(socketId)
            Exit Sub
        End If

        ' Increased idle counter threshold to prevent premature disconnection
        Dim idleCounter As Integer = 0
        Dim maxIdleCount As Integer = 10000 ' Much higher tolerance

        ' Initial wait to make sure connection is properly established
        Thread.Sleep(100)

        While _isListening
            Try
                ' Only check connection every 10 seconds (at 1ms sleep interval)
                If idleCounter >= maxIdleCount Then
                    Dim connected As Boolean = False
                    SyncLock _socketLock
                        connected = _clients.ContainsKey(socketId)
                    End SyncLock

                    If Not connected Then
                        Debug.Print("Client " & socketId & " no longer in client list")
                        Exit While
                    End If

                    idleCounter = 0
                End If

                ' Check for data
                If stream.DataAvailable Then
                    ' Reset idle counter when we get data
                    idleCounter = 0

                    Try
                        Dim bytesRead As Integer = 0
                        bytesRead = stream.Read(buffer, 0, buffer.Length)

                        If bytesRead > 0 Then
                            ' Copy data to correctly sized array
                            Dim data(bytesRead - 1) As Byte
                            Array.Copy(buffer, data, bytesRead)

                            ' Raise event
                            RaiseEvent DataReceived(socketId, data, bytesRead)
                        Else
                            ' Don't disconnect on zero bytes if we're still connected
                            If Not IsSocketConnected(socketId) Then
                                Debug.Print("Socket " & socketId & " no longer connected")
                                Exit While
                            End If
                        End If
                    Catch ex As Exception
                        Debug.Print("Error reading from socket " & socketId & ": " & ex.Message)
                        If Not IsSocketConnected(socketId) Then
                            Exit While
                        End If
                    End Try
                Else
                    ' No data available, yield CPU and increment idle counter
                    idleCounter += 1
                    Thread.Sleep(1)
                End If

                ' Add additional socket closure detection
                If Not IsSocketConnected(socketId) Then
                    Debug.Print("Socket " & socketId & " detected as disconnected in data listener")
                    Exit While
                End If
            Catch ex As Exception
                Debug.Print("Error in ListenForData: " & ex.Message)
                ' Only exit if we're really disconnected
                If Not IsSocketConnected(socketId) Then
                    Debug.Print("Socket " & socketId & " confirmed disconnected after error")
                    Exit While
                End If
                ' Otherwise, let's be more tolerant and continue
                Thread.Sleep(100)
            End Try
        End While

        Debug.Print("ListenForData thread ending for socket " & socketId)
        CloseSocket(socketId)
    End Sub

    ' Helper method to check if a socket is still connected
    Private Function IsSocketConnected(ByVal socketId As Integer) As Boolean
        Try
            SyncLock _socketLock
                If Not _clients.ContainsKey(socketId) Then
                    Return False
                End If

                Dim client As TcpClient = _clients(socketId)

                ' First quick check
                If client Is Nothing OrElse client.Client Is Nothing Then
                    Return False
                End If

                ' Socket.Poll with SelectMode.SelectRead and zero timeout will return:
                ' - True if socket is closed, reset, terminated, or if there's pending data
                ' - False if connection is open and there's no pending data
                If client.Client.Poll(0, SelectMode.SelectRead) Then
                    ' If there's data available, the socket is still connected
                    If client.Available > 0 Then
                        Return True
                    End If

                    ' Otherwise, the socket might be closed
                    ' Additional check to be sure
                    Dim tmp(0) As Byte
                    Return client.Client.Receive(tmp, SocketFlags.Peek) <> 0
                End If

                ' If Poll returned false, the socket is definitely connected
                Return True
            End SyncLock
        Catch ex As Exception
            Debug.Print("Error checking socket connected state: " & ex.Message)
            Return False
        End Try
    End Function

    Public Function SendData(ByVal socketId As Integer, ByVal data() As Byte) As Boolean
        If data Is Nothing OrElse data.Length = 0 Then
            Return True  ' Nothing to send
        End If

        SyncLock _socketLock
            If Not _clients.ContainsKey(socketId) Then
                Debug.Print("Cannot send data: socket " & socketId & " not found")
                Return False
            End If

            Try
                Dim stream As NetworkStream = _clientStreams(socketId)
                stream.Write(data, 0, data.Length)
                Return True
            Catch ex As Exception
                Debug.Print("Error sending data to socket " & socketId & ": " & ex.Message)
                CloseSocket(socketId)
                Return False
            End Try
        End SyncLock
    End Function

    Public Function SendString(ByVal socketId As Integer, ByVal message As String) As Boolean
        If String.IsNullOrEmpty(message) Then
            Return True  ' Nothing to send
        End If

        Dim data() As Byte = System.Text.Encoding.Default.GetBytes(message)
        Return SendData(socketId, data)
    End Function

    Public Sub CloseSocket(ByVal socketId As Integer)
        Dim needToRaiseEvent As Boolean = False
        Dim clientExists As Boolean = False

        SyncLock _socketLock
            clientExists = _clients.ContainsKey(socketId)
            If clientExists Then
                Debug.Print("Closing socket " & socketId)
                needToRaiseEvent = True

                Try
                    If _clientStreams.ContainsKey(socketId) Then
                        _clientStreams(socketId).Close()
                        _clientStreams.Remove(socketId)
                    End If

                    _clients(socketId).Close()
                    _clients.Remove(socketId)
                Catch ex As Exception
                    Debug.Print("Error during socket " & socketId & " cleanup: " & ex.Message)
                End Try
            Else
                Debug.Print("CloseSocket called for non-existent socket " & socketId)
                ' Socket already gone, but we'll still raise the event to ensure cleanup
                needToRaiseEvent = True
            End If
        End SyncLock

        ' Raise event outside the lock to prevent deadlocks
        ' Always raise the event even if the socket was already gone
        ' This ensures user data is properly cleaned up in all cases
        If needToRaiseEvent Then
            Try
                Debug.Print("Raising ClientDisconnected event for socket " & socketId)
                RaiseEvent ClientDisconnected(socketId)
            Catch ex As Exception
                Debug.Print("Error in ClientDisconnected event handler: " & ex.Message & vbCrLf & ex.StackTrace)
            End Try
        End If
    End Sub

    Public Function GetClientIP(ByVal socketId As Integer) As String
        SyncLock _socketLock
            If Not _clients.ContainsKey(socketId) Then
                Return String.Empty
            End If

            Try
                Dim client As TcpClient = _clients(socketId)
                If client.Client IsNot Nothing AndAlso client.Client.RemoteEndPoint IsNot Nothing Then
                    Dim endpoint As IPEndPoint = DirectCast(client.Client.RemoteEndPoint, IPEndPoint)
                    Return endpoint.Address.ToString()
                End If
                Return String.Empty
            Catch ex As Exception
                Debug.Print("Error getting client IP for socket " & socketId & ": " & ex.Message)
                Return String.Empty
            End Try
        End SyncLock
    End Function
End Class
