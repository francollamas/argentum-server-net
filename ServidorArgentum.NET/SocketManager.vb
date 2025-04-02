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

        _listener = New TcpListener(IPAddress.Any, _port)
        _listener.Start()
        _isListening = True

        ' Reset client tracking
        SyncLock _socketLock
            _clients.Clear()
            _clientStreams.Clear()
            _clientCounter = 1
        End SyncLock

        ' Start accepting connections asynchronously
        Dim t As New Thread(AddressOf AcceptConnections)
        t.IsBackground = True
        t.Start()
    End Sub

    Public Sub StopListening()
        _isListening = False
        If _listener IsNot Nothing Then
            _listener.Stop()
        End If

        ' Close all client connections
        SyncLock _socketLock
            Dim socketIds As New List(Of Integer)(_clients.Keys)
            Dim socketId As Integer
            For Each socketId In socketIds
                CloseSocket(socketId)
            Next
        End SyncLock
    End Sub

    Private Sub AcceptConnections()
        While _isListening
            Try
                Dim client As TcpClient = _listener.AcceptTcpClient()
                Dim socketId As Integer = GetNextSocketId()

                If socketId > 0 Then
                    ' Configure client for proper operation
                    client.ReceiveBufferSize = 8192
                    client.SendBufferSize = 8192
                    client.NoDelay = True

                    Dim stream As NetworkStream = client.GetStream()
                    SyncLock _socketLock
                        ' Double check we're not reusing an ID
                        If _clients.ContainsKey(socketId) Then
                            Debug.Print("ERROR: Socket ID collision detected for ID " & socketId)
                            Try
                                client.Close()
                            Catch ex As Exception
                                ' Ignore errors when closing
                            End Try
                            Continue While
                        End If

                        _clients.Add(socketId, client)
                        _clientStreams.Add(socketId, stream)
                    End SyncLock

                    Debug.Print("New client connected with socket ID: " & socketId)

                    ' Start listening for data from this client
                    Dim t As New Thread(Sub() ListenForData(socketId, client))
                    t.IsBackground = True
                    t.Start()

                    ' Raise connection accepted event
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
    End Sub

    Private Function GetNextSocketId() As Integer
        SyncLock _socketLock
            If _clients.Count >= _maxClients Then
                Return -1
            End If

            ' Find an unused ID with protection against wrapping
            Dim startCounter As Integer = _clientCounter
            Dim found As Boolean = False

            Do
                If Not _clients.ContainsKey(_clientCounter) Then
                    found = True
                    Exit Do
                End If

                _clientCounter += 1
                If _clientCounter > _maxClients Then _clientCounter = 1
                
                ' Prevent infinite loop if all IDs are used
                If _clientCounter = startCounter Then
                    Return -1
                End If
            Loop

            If found Then
                Dim result As Integer = _clientCounter
                ' Increment for next time
                _clientCounter += 1
                If _clientCounter > _maxClients Then _clientCounter = 1
                Return result
            Else
                Return -1
            End If
        End SyncLock
    End Function

    Private Sub ListenForData(ByVal socketId As Integer, ByVal client As TcpClient)
        Dim buffer(8192) As Byte

        Try
            Dim stream As NetworkStream = Nothing
            
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

            While _isListening
                Dim bytesRead As Integer = 0
                Dim connected As Boolean = False

                Try
                    ' Check if client is still connected
                    SyncLock _socketLock
                        connected = _clients.ContainsKey(socketId) AndAlso _clients(socketId).Connected
                    End SyncLock
                    
                    If Not connected Then
                        Debug.Print("Client " & socketId & " disconnected")
                        Exit While
                    End If
                    
                    If stream.DataAvailable Then
                        bytesRead = stream.Read(buffer, 0, buffer.Length)
                    Else
                        Thread.Sleep(1) ' Reduce CPU usage
                        Continue While
                    End If
                Catch ex As Exception
                    ' Connection error
                    Debug.Print("Error reading from socket " & socketId & ": " & ex.Message)
                    Exit While
                End Try

                If bytesRead > 0 Then
                    Dim data(bytesRead - 1) As Byte
                    Array.Copy(buffer, data, bytesRead)

                    ' Raise data received event
                    RaiseEvent DataReceived(socketId, data, bytesRead)
                Else
                    ' Client disconnected
                    Debug.Print("Client " & socketId & " sent 0 bytes, disconnecting")
                    Exit While
                End If
            End While
        Catch ex As Exception
            Debug.Print("Error in client thread for socket " & socketId & ": " & ex.Message)
        Finally
            ' Client disconnected or error occurred
            CloseSocket(socketId)
        End Try
    End Sub

    Public Function SendData(ByVal socketId As Integer, ByVal data() As Byte) As Boolean
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
        Dim data() As Byte = System.Text.Encoding.ASCII.GetBytes(message)
        Return SendData(socketId, data)
    End Function

    Public Sub CloseSocket(ByVal socketId As Integer)
        Dim needToRaiseEvent As Boolean = False
        
        SyncLock _socketLock
            If _clients.ContainsKey(socketId) Then
                Debug.Print("Closing socket " & socketId)
                needToRaiseEvent = True
                
                Try
                    _clientStreams(socketId).Close()
                    _clients(socketId).Close()
                Catch ex As Exception
                    Debug.Print("Error closing socket " & socketId & ": " & ex.Message)
                End Try
                
                _clientStreams.Remove(socketId)
                _clients.Remove(socketId)
            End If
        End SyncLock

        ' Raise event outside the lock to prevent deadlocks
        If needToRaiseEvent Then
            RaiseEvent ClientDisconnected(socketId)
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
                    Dim endpoint As System.Net.IPEndPoint = DirectCast(client.Client.RemoteEndPoint, System.Net.IPEndPoint)
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
