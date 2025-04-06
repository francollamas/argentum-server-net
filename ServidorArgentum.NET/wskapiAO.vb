Option Strict Off
Option Explicit On

Imports System.Collections.Generic

Module wskapiAO
    ''' <summary>
    ''' Maps socket IDs to user indices
    ''' </summary>
    Private SocketToUserMap As New Dictionary(Of Integer, Integer)
    
    ''' <summary>
    ''' Gets the IP address of a connected client
    ''' </summary>
    Public Function GetAscIP(ByVal socketID As Integer) As String
        Try
            Dim userIdx As Integer = BuscaSlotSock(socketID)
            If userIdx > 0 Then
                Return UserList(userIdx).ip
            End If
        Catch
        End Try
        
        Return "0.0.0.0"
    End Function
    
    ''' <summary>
    ''' Handles a connection request from a client
    ''' </summary>
    Private Sub HandleConnectionReceived(socketID As Integer, clientIP As String)
        ' This runs on the main thread via the Windows.Forms event system
        Dim NewIndex As Integer = NextOpenUser()
        
        If NewIndex <= MaxUsers Then
            Call UserList(NewIndex).incomingData.ReadASCIIStringFixed(UserList(NewIndex).incomingData.length)
            Call UserList(NewIndex).outgoingData.ReadASCIIStringFixed(UserList(NewIndex).outgoingData.length)
            
            UserList(NewIndex).ip = clientIP
            
            For i As Integer = 1 To BanIps.Count()
                Dim bannedIP As String = CStr(BanIps.Item(i))
                If bannedIP = clientIP Then
                    Call WriteErrorMsg(NewIndex, "Su IP se encuentra bloqueada en este servidor.")
                    Call FlushBuffer(NewIndex)
                    Call SocketManager.CloseSocket(socketID)
                    Return
                End If
            Next
            
            If NewIndex > LastUser Then
                LastUser = NewIndex
            End If
            
            UserList(NewIndex).ConnID = socketID
            UserList(NewIndex).ConnIDValida = True
            
            AgregaSlotSock(socketID, NewIndex)
        Else
            Dim errorMsg As String = Protocol.PrepareMessageErrorMsg("El servidor se encuentra lleno en este momento. Disculpe las molestias ocasionadas.")
            Dim data() As Byte = SocketManager.StringToBytes(errorMsg)
            SocketManager.SendData(socketID, data)
            SocketManager.CloseSocket(socketID)
        End If
    End Sub
    
    ''' <summary>
    ''' Handles data received from a client
    ''' </summary>
    Private Sub HandleDataReceived(socketID As Integer, data() As Byte)
        ' This now runs on the main thread via the synchronized queue system
        Dim userIndex As Integer = BuscaSlotSock(socketID)
        
        If userIndex > 0 Then
            With UserList(userIndex)
                Call .incomingData.WriteBlock(data)
                
                If .ConnID <> -1 Then
                    Call HandleIncomingData(userIndex)
                End If
            End With
        End If
    End Sub

    ''' <summary>
    ''' Handles closure of a socket connection
    ''' </summary>
    Private Sub HandleConnectionClosed(socketID As Integer)
        ' This runs on the main thread via the Windows.Forms event system
        Dim userIndex As Integer = BuscaSlotSock(socketID)

        If userIndex > 0 Then
            Call BorraSlotSock(socketID)

            UserList(userIndex).ConnID = -1
            UserList(userIndex).ConnIDValida = False

            Call EventoSockClose(userIndex)
        End If
    End Sub

    ''' <summary>
    ''' Handles connection error
    ''' </summary>
    Private Sub HandleConnectionError(errorMessage As String)
        ' This runs on the main thread via the Windows.Forms event system
        Debug.Print("Connection Error: " & errorMessage)
    End Sub
    
    ''' <summary>
    ''' Closes a socket connection
    ''' </summary>
    Public Function Winsock_Close(ByRef socketID As Integer) As Object
        ' This is called from the main thread
        Dim userIndex As Integer = BuscaSlotSock(socketID)

        If socketID > 0 Then
            SocketManager.CloseSocket(socketID)
        End If

        If userIndex > 0 Then
            Call BorraSlotSock(socketID)
            UserList(userIndex).ConnID = -1
            UserList(userIndex).ConnIDValida = False
            Call EventoSockClose(userIndex)
        End If

        Return Nothing
    End Function

    ''' <summary>
    ''' Initializes the socket API
    ''' </summary>
    Public Sub IniciaWsApi(ByVal port As Integer)
        ' Set up event handlers to process messages on the main thread
        AddHandler SocketManager.ConnectionReceived, AddressOf HandleConnectionReceived
        AddHandler SocketManager.DataReceived, AddressOf HandleDataReceived
        AddHandler SocketManager.ConnectionClosed, AddressOf HandleConnectionClosed
        AddHandler SocketManager.ServerError, AddressOf HandleConnectionError

        SocketManager.Initialize(port)
    End Sub
    
    ''' <summary>
    ''' Cleans up the socket API
    ''' </summary>
    Public Sub LimpiaWsApi()
        RemoveHandler SocketManager.ConnectionReceived, AddressOf HandleConnectionReceived
        RemoveHandler SocketManager.DataReceived, AddressOf HandleDataReceived
        RemoveHandler SocketManager.ConnectionClosed, AddressOf HandleConnectionClosed
        RemoveHandler SocketManager.ServerError, AddressOf HandleConnectionError

        SocketManager.Shutdown()
        
        SocketToUserMap.Clear()
    End Sub
    
    ''' <summary>
    ''' Sends data through a user's socket connection
    ''' </summary>
    Public Function WsApiEnviar(ByVal userIndex As Integer, ByRef message As String) As Integer
        On Error GoTo ErrorHandler
        
        Dim returnCode As Integer = 0
        
        Dim socketID As Integer = UserList(userIndex).ConnID
        
        If socketID <> -1 AndAlso UserList(userIndex).ConnIDValida Then
            Dim data() As Byte = SocketManager.StringToBytes(message)
            
            If Not SocketManager.SendData(socketID, data) Then
                returnCode = -1
            End If
            
            ' Process any pending messages that may have arrived
            SocketManager.ProcessPendingMessages()
        ElseIf socketID <> -1 AndAlso Not UserList(userIndex).ConnIDValida Then
            If Not UserList(userIndex).Counters.Saliendo Then
                returnCode = -1
            End If
        End If
        
        WsApiEnviar = returnCode
        
        Exit Function
        
ErrorHandler:
        Call UserList(userIndex).outgoingData.WriteASCIIStringFixed(message)
        Resume Next
    End Function
    
    ''' <summary>
    ''' Process any pending messages in the queue (can be called from main game loop)
    ''' </summary>
    Public Sub ProcessNetworkMessages()
        ' Call this from your main game loop or timer to ensure messages get processed
        SocketManager.ProcessPendingMessages()
    End Sub
    
    ''' <summary>
    ''' Reinitializes all sockets
    ''' </summary>
    Public Sub WSApiReiniciarSockets()
        Dim i As Integer
        
        For i = 1 To MaxUsers
            If UserList(i).ConnID <> -1 AndAlso UserList(i).ConnIDValida Then
                Call CloseSocket(i)
            End If
        Next i
        
        For i = 1 To MaxUsers
            UserList(i).incomingData = Nothing
            UserList(i).outgoingData = Nothing
        Next i
        
        ReDim UserList(MaxUsers)
        ArrayInitializers.InitializeStruct(UserList)
        For i = 1 To MaxUsers
            UserList(i).ConnID = -1
            UserList(i).ConnIDValida = False
            
            UserList(i).incomingData = New clsByteQueue
            UserList(i).outgoingData = New clsByteQueue
        Next i
        
        LastUser = 1
        NumUsers = 0
        
        SocketToUserMap.Clear()
        
        Call IniciaWsApi(Puerto)
    End Sub
    
    ''' <summary>
    ''' Finds the user index associated with a socket ID
    ''' </summary>
    Public Function BuscaSlotSock(ByVal socketID As Integer) As Integer
        Dim userIndex As Integer = -1
        
        If SocketToUserMap.TryGetValue(socketID, userIndex) Then
            Return userIndex
        End If
        
        Return -1
    End Function
    
    ''' <summary>
    ''' Associates a socket ID with a user index
    ''' </summary>
    Public Sub AgregaSlotSock(ByVal socketID As Integer, ByVal userIndex As Integer)
        Debug.Print("AgregaSlotSock: Socket " & socketID & " -> User " & userIndex)
        
        If SocketToUserMap.Count > MaxUsers Then
            Call CloseSocket(userIndex)
            Exit Sub
        End If
        
        SocketToUserMap(socketID) = userIndex
    End Sub
    
    ''' <summary>
    ''' Removes the association between a socket ID and a user index
    ''' </summary>
    Public Sub BorraSlotSock(ByVal socketID As Integer)
        Dim count As Integer = SocketToUserMap.Count
        
        SocketToUserMap.Remove(socketID)
        
        Debug.Print("BorraSlotSock: " & count & " -> " & SocketToUserMap.Count)
    End Sub
    
    ''' <summary>
    ''' Handles the event of a socket being closed
    ''' </summary>
    Public Sub EventoSockClose(ByVal userIndex As Integer)
        If Centinela.RevisandoUserIndex = userIndex Then 
            Call modCentinela.CentinelaUserLogout()
        End If
        
        If UserList(userIndex).flags.UserLogged Then
            Call CloseSocketSL(userIndex)
            Call Cerrar_Usuario(userIndex)
        Else
            Call CloseSocket(userIndex)
        End If
    End Sub
End Module
