Option Strict Off
Option Explicit On
Imports System.Collections.Generic

Module wskapiAO

    Private Const MAX_INT As Short = 32767
    ' Change from Collection to Dictionary (socket ID → user slot)
    Public WSAPISock2Usr As New Dictionary(Of Integer, Integer)

    ' Our new socket manager
    Private WithEvents socketManager As SocketManager = Nothing

    Public Function GetAscIP(ByVal inn As Integer) As String
        ' Manteniendo la firma original del método para compatibilidad
        If socketManager IsNot Nothing Then
            Return socketManager.GetClientIP(inn)
        End If
        Return String.Empty
    End Function

    Public Function Winsock_ConnectionRequest(ByRef Index As Short, ByVal requestID As Integer) As Object
        ' This function is no longer needed with the new implementation
        ' It's kept for backward compatibility
        Debug.Print("Winsock_ConnectionRequest should not be called directly anymore")
        Return Nothing
    End Function

    ' This handler is called when a new connection is accepted by the SocketManager
    Private Sub SocketManager_ConnectionAccepted(ByVal socketId As Integer, ByVal client As System.Net.Sockets.TcpClient) Handles socketManager.ConnectionAccepted
        Try
            Debug.Print("Processing new connection: socket=" & socketId)
            Dim NewIndex As Short = NextOpenUser() ' Nuevo indice

            If NewIndex <= MaxUsers Then
                ' Clean incoming and outgoing data
                UserList(NewIndex).incomingData.ReadASCIIStringFixed(UserList(NewIndex).incomingData.length)
                UserList(NewIndex).outgoingData.ReadASCIIStringFixed(UserList(NewIndex).outgoingData.length)

                ' Store IP and check bans
                UserList(NewIndex).ip = GetAscIP(socketId)
                Debug.Print("Connection from IP: " & UserList(NewIndex).ip)

                ' Only ban check if necessary (commented out for simplicity)
                'Dim i As Short
                'For i = 1 To BanIps.Count()
                '    If BanIps.Item(i) = UserList(NewIndex).ip Then
                '        Debug.Print("Banned IP rejected: " & UserList(NewIndex).ip)
                '        WriteErrorMsg(NewIndex, "Su IP se encuentra bloqueada en este servidor.")
                '        FlushBuffer(NewIndex)
                '        Call CloseSocketBySlot(NewIndex)  ' Changed from CloseSocket
                '        Exit Sub
                '    End If
                'Next i

                ' Assign slot and accept connection
                If NewIndex > LastUser Then LastUser = NewIndex

                ' Add socket mapping first to ensure it's in place before other operations
                AgregaSlotSock(socketId, NewIndex)

                ' Then update user data
                UserList(NewIndex).ConnID = socketId
                UserList(NewIndex).ConnIDValida = True

                Debug.Print("User successfully connected in slot: " & NewIndex)
            Else
                ' Server is full
                Debug.Print("Server full - connection rejected")
                Dim str_Renamed As String = Protocol.PrepareMessageErrorMsg("El servidor se encuentra lleno en este momento. Disculpe las molestias ocasionadas.")

                socketManager.SendString(socketId, str_Renamed)
                socketManager.CloseSocket(socketId)
            End If
        Catch ex As Exception
            Debug.Print("Error handling new connection: " & ex.Message & vbCrLf & ex.StackTrace)
            Try
                socketManager.CloseSocket(socketId)
            Catch ex2 As Exception
                ' Just log
                Debug.Print("Error closing socket: " & ex2.Message)
            End Try
        End Try
    End Sub

    Public Function Winsock_Close(ByRef Index As Short) As Object

        If socketManager IsNot Nothing Then
            socketManager.CloseSocket(Index)
        End If
        Return Nothing
    End Function

    Public Sub Winsock_Erase()
        If socketManager IsNot Nothing Then
            socketManager.StopListening()
            socketManager = Nothing
        End If
    End Sub

    ' No longer needed - SocketManager handles this internally
    Private Function Winsock_NextOpenSocket() As Short
        Debug.Print("Winsock_NextOpenSocket is no longer used")
        Return 0
    End Function

    Public Sub IniciaWsApi(ByVal port As Short)
        ' Clean up existing resources first
        Winsock_Erase()

        ' Clear the socket dictionary to prevent stale mappings
        WSAPISock2Usr.Clear()

        socketManager = New SocketManager(port, MAX_INT)
        socketManager.StartListening()

        Debug.Print("Socket server initialized on port " & port)
    End Sub

    Public Sub LimpiaWsApi()
        Winsock_Erase()
    End Sub

    Public Function WsApiEnviar(ByVal Slot As Short, ByRef str_Renamed As String) As Integer
        On Error GoTo ErrorHandler

        Dim Retorno As Integer
        Retorno = 0

        If UserList(Slot).ConnID <> -1 And UserList(Slot).ConnIDValida Then
            Dim slotIndex As Integer = UserList(Slot).ConnID
            Dim data() As Byte = System.Text.Encoding.GetEncoding(1252).GetBytes(str_Renamed)

            Debug.Print("> Enviando data: [len]: " + str_Renamed.Length.ToString() + " [data]: " + String.Join(" ", Array.ConvertAll(data, Function(b) b.ToString())))

            If socketManager Is Nothing OrElse socketManager.SendData(slotIndex, data) = False Then
                Retorno = -1
            End If
        ElseIf UserList(Slot).ConnID <> -1 And Not UserList(Slot).ConnIDValida Then
            If Not UserList(Slot).Counters.Saliendo Then
                Retorno = -1
            End If
        End If

        WsApiEnviar = Retorno

        Exit Function

ErrorHandler:
        Call UserList(Slot).outgoingData.WriteASCIIStringFixed(str_Renamed)
        Resume Next
    End Function

    ' This function is called by the SocketManager when data is received
    Private Sub SocketManager_DataReceived(ByVal socketId As Integer, ByVal data() As Byte, ByVal bytesTotal As Integer) Handles socketManager.DataReceived
        Dim N As Short
        N = BuscaSlotSock(socketId)

        If N <= 0 Then
            socketManager.CloseSocket(socketId)
            Exit Sub
        End If

        Dim datosInString As String = String.Join(" ", Array.ConvertAll(data, Function(b) b.ToString()))
        Debug.Print("< Recibiendo data: [len]: " + data.Length.ToString() + " [data]: " + datosInString)

        Call EventoSockRead(N, data)
    End Sub

    ' This function is called by the SocketManager when a client disconnects
    Private Sub SocketManager_ClientDisconnected(ByVal socketId As Integer) Handles socketManager.ClientDisconnected
        Debug.Print("SocketManager_ClientDisconnected called for socket " & socketId)
        
        ' Find the user slot for this socket
        Dim N As Short = BuscaSlotSock(socketId)
        Debug.Print("Socket " & socketId & " was mapped to user slot " & N)
        
        If N > 0 Then
            ' CRITICAL: Remove from socket dictionary FIRST to prevent recursion and race conditions
            Call BorraSlotSock(socketId)
            
            ' Call the actual user cleanup function from TCP.bas
            Call CloseUserInstance(N, True)
        Else
            Debug.Print("No user found for socket " & socketId)
        End If
    End Sub

    ' New helper function that calls the right sequence of cleanup functions
    Private Sub CloseUserInstance(ByVal UserIndex As Integer, ByVal forceClose As Boolean)
        Debug.Print("CloseUserInstance called for user " & UserIndex & ", forced: " & forceClose)
        
        ' Only proceed if this is a valid user index
        If UserIndex <= 0 OrElse UserIndex > MaxUsers Then
            Debug.Print("Invalid user index in CloseUserInstance: " & UserIndex)
            Exit Sub
        End If
        
        ' Check for valid connection - if not and not forcing, exit
        If Not forceClose AndAlso UserList(UserIndex).ConnIDValida = False Then
            Debug.Print("User " & UserIndex & " already has invalid connection")
            Exit Sub
        End If
        
        ' Debug info
        Debug.Print("Cleaning up user " & UserIndex & ": " & UserList(UserIndex).name & _
                    ", Logged: " & UserList(UserIndex).flags.UserLogged)
        
        ' Call cleanup based on login state
        If UserList(UserIndex).flags.UserLogged Then
            Debug.Print("User is logged in, calling TCP.CloseSocketSL")
            Call CloseSocketSL(UserIndex)
            Call Cerrar_Usuario(UserIndex)
        Else
            ' Not logged in, just clean up the connection
            Debug.Print("User not logged in, cleaning connection only")
            
            ' Directly clean up without going through external functions
            With UserList(UserIndex)
                ' Store ConnID before resetting
                Dim oldConnID As Integer = .ConnID
                
                ' Reset connection state
                .ConnIDValida = False
                .ConnID = -1
                
                ' Ensure complete reset of key fields
                .flags.UserLogged = False
                .name = ""
                .ip = ""
                
                ' Close the actual socket if needed
                If oldConnID > 0 AndAlso socketManager IsNot Nothing Then
                    Try
                        ' Don't trigger events again
                        RemoveMappingOnly(oldConnID)
                        socketManager.CloseSocket(oldConnID)
                    Catch ex As Exception
                        Debug.Print("Error closing socket in CloseUserInstance: " & ex.Message)
                    End Try
                End If
            End With
            
            ' Ensure other cleanup occurs
            If Centinela.RevisandoUserIndex = UserIndex Then
                Call modCentinela.CentinelaUserLogout()
            End If
        End If
        
        ' Final verification of complete cleanup
        Call VerifyUserCleanup(UserIndex)
        
        Debug.Print("CloseUserInstance completed for user " & UserIndex)
    End Sub

    ' Helper to just remove mapping without triggering events
    Private Sub RemoveMappingOnly(ByVal socketId As Integer)
        Try
            If WSAPISock2Usr.ContainsKey(socketId) Then
                WSAPISock2Usr.Remove(socketId)
                Debug.Print("Removed socket " & socketId & " mapping without events")
            End If
        Catch ex As Exception
            Debug.Print("Error in RemoveMappingOnly: " & ex.Message)
        End Try
    End Sub

    ' Make sure user is completely clean
    Private Sub VerifyUserCleanup(ByVal UserIndex As Integer)
        With UserList(UserIndex)
            ' Verify connection is reset
            If .ConnID <> -1 Then
                Debug.Print("WARNING: User " & UserIndex & " still has ConnID " & .ConnID & " after cleanup")
                .ConnID = -1
            End If
            
            If .ConnIDValida Then
                Debug.Print("WARNING: User " & UserIndex & " still has valid connection flag after cleanup")
                .ConnIDValida = False
            End If
            
            ' Verify logged state is reset
            If .flags.UserLogged Then
                Debug.Print("WARNING: User " & UserIndex & " still marked as logged in after cleanup")
                .flags.UserLogged = False
            End If
            
            ' Verify name is cleared
            If .name <> "" Then
                Debug.Print("WARNING: User " & UserIndex & " still has name '" & .name & "' after cleanup")
                .name = ""
            End If
        End With
    End Sub

    Public Sub EventoSockRead(ByVal Slot As Short, ByRef datos() As Byte)
        With UserList(Slot)
            Call .incomingData.WriteBlock(datos)

            If .ConnID <> -1 Then
                Call HandleIncomingData(Slot)
            Else
                Exit Sub
            End If
        End With
    End Sub

    Public Sub EventoSockClose(ByVal Slot As Short)
        Debug.Print("EventoSockClose for slot " & Slot)

        ' Handle Centinela if needed
        If Centinela.RevisandoUserIndex = Slot Then
            Debug.Print("Calling CentinelaUserLogout for slot " & Slot)
            Call modCentinela.CentinelaUserLogout()
        End If

        ' Check if user is logged in
        If UserList(Slot).flags.UserLogged Then
            Debug.Print("User " & UserList(Slot).name & " was logged in, calling CloseSocketSL and Cerrar_Usuario")
            Call CloseSocketSL(Slot)
            Call Cerrar_Usuario(Slot)
        Else
            Debug.Print("User was not logged in, calling CloseSocketBySlot")
            Call CloseSocketBySlot(Slot)  ' Changed from CloseSocket
        End If

        ' Call our thorough reset function to ensure clean state
        Call ResetUserSlot(Slot)

        Debug.Print("EventoSockClose completed for slot " & Slot)
    End Sub

    ' Renamed from CloseSocket to CloseSocketBySlot for clarity
    Public Sub CloseSocketBySlot(ByVal Slot As Short)
        ' Check if this is a valid user slot
        If Slot <= 0 OrElse Slot > MaxUsers Then
            Debug.Print("CloseSocketBySlot called with invalid slot: " & Slot)
            Exit Sub
        End If

        Debug.Print("CloseSocketBySlot called for slot " & Slot)

        ' Use our centralized cleanup function
        Call CloseUserInstance(Slot, True)
    End Sub

    Public Sub WSApiReiniciarSockets()
        Dim i As Integer

        For i = 1 To MaxUsers
            If UserList(i).ConnID <> -1 And UserList(i).ConnIDValida Then
                Call CloseSocketBySlot(i)  ' Changed from CloseSocket
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

        Call IniciaWsApi(Puerto)
    End Sub

    ' Simplified slot mapping functions

    Public Function BuscaSlotSock(ByVal S As Integer) As Integer
        ' Simple validation and lookup
        If S <= 0 Then Return -1

        Try
            If WSAPISock2Usr.ContainsKey(S) Then Return WSAPISock2Usr(S)
        Catch ex As Exception
            Debug.Print("BuscaSlotSock error: " & ex.Message)
        End Try

        Return -1
    End Function

    Public Sub AgregaSlotSock(ByVal Sock As Integer, ByVal Slot As Integer)
        Debug.Print("AgregaSlotSock: Socket=" & Sock & ", Slot=" & Slot)
        
        If Sock <= 0 OrElse Slot <= 0 OrElse Slot > MaxUsers Then
            Debug.Print("AgregaSlotSock: Invalid parameters")
            Return
        End If
        
        Try
            ' CRITICAL: Check if this slot is already in use by ANY socket and force cleanup
            If UserList(Slot).ConnID > 0 Then
                Debug.Print("WARNING: Slot " & Slot & " already has active connection with socket " & UserList(Slot).ConnID)
                ' Force close this slot entirely to clean any state
                Call CloseUserInstance(Slot, True)
                ' Wait a moment to ensure cleanup completes
                System.Threading.Thread.Sleep(50)
            End If
            
            ' Also check if this socket is already mapped to ANY slot and clean that too
            Dim existingSlot As Integer = BuscaSlotSock(Sock)
            If existingSlot > 0 Then
                Debug.Print("WARNING: Socket " & Sock & " already mapped to slot " & existingSlot)
                If existingSlot <> Slot Then
                    ' Clean up the previous slot
                    Call CloseUserInstance(existingSlot, True)
                    System.Threading.Thread.Sleep(50)
                End If
            End If
            
            ' Remove all mappings for this slot and this socket
            Dim slotCurrentSocket As Integer = -1
            Dim socketsToRemove As New List(Of Integer)

            ' Find all sockets using this slot
            Dim kvp As KeyValuePair(Of Integer, Integer)
            For Each kvp In WSAPISock2Usr
                If kvp.Value = Slot Then
                    socketsToRemove.Add(kvp.Key)
                    Debug.Print("Found existing socket " & kvp.Key & " for slot " & Slot)
                End If
            Next

            ' Remove them all
            Dim oldSock As Integer
            For Each oldSock In socketsToRemove
                WSAPISock2Usr.Remove(oldSock)
                Debug.Print("Removed old socket " & oldSock & " from slot " & Slot)
                ' Also close the socket if it's different from our current one
                If oldSock <> Sock AndAlso socketManager IsNot Nothing Then
                    Try
                        socketManager.CloseSocket(oldSock)
                    Catch ex As Exception
                        Debug.Print("Error closing old socket: " & ex.Message)
                    End Try
                End If
            Next
            
            ' Add the new mapping - but only after we've cleared everything
            WSAPISock2Usr.Add(Sock, Slot)
            Debug.Print("Added socket " & Sock & " with slot " & Slot)
            
            ' Also set the user's ConnID
            UserList(Slot).ConnID = Sock
            UserList(Slot).ConnIDValida = True
            
            ' Verify no duplicates exist
            VerifyNoUserDuplicates(Slot, Sock)
        Catch ex As Exception
            Debug.Print("Error in AgregaSlotSock: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    ' New verification to catch any lingering duplicates
    Private Sub VerifyNoUserDuplicates(ByVal slot As Integer, ByVal socket As Integer)
        Try
            ' Check no other socket is mapped to this slot
            Dim kvp As KeyValuePair(Of Integer, Integer)
            For Each kvp In WSAPISock2Usr
                If kvp.Value = slot AndAlso kvp.Key <> socket Then
                    Debug.Print("CRITICAL ERROR: Duplicate socket " & kvp.Key & " found for slot " & slot)
                    ' Emergency cleanup
                    WSAPISock2Usr.Remove(kvp.Key)
                    If socketManager IsNot Nothing Then
                        Try
                            socketManager.CloseSocket(kvp.Key)
                        Catch ex As Exception
                            ' Just log
                        End Try
                    End If
                End If
            Next

            ' Verify every user slot has at most one socket
            Dim slotCounts As New Dictionary(Of Integer, Integer)
            For Each kvp In WSAPISock2Usr
                If slotCounts.ContainsKey(kvp.Value) Then
                    slotCounts(kvp.Value) += 1
                Else
                    slotCounts.Add(kvp.Value, 1)
                End If
            Next

            Dim slotCount As KeyValuePair(Of Integer, Integer)
            For Each slotCount In slotCounts
                If slotCount.Value > 1 Then
                    Debug.Print("CRITICAL ERROR: Slot " & slotCount.Key & " has " & slotCount.Value & " sockets!")
                End If
            Next
        Catch ex As Exception
            Debug.Print("Error in VerifyNoUserDuplicates: " & ex.Message)
        End Try
    End Sub

    Public Sub BorraSlotSock(ByVal Sock As Integer)
        Debug.Print("BorraSlotSock: " & Sock)
        Try
            If WSAPISock2Usr.ContainsKey(Sock) Then
                WSAPISock2Usr.Remove(Sock)
                Debug.Print("Socket " & "Sock removed from dictionary")
            End If
        Catch ex As Exception
            Debug.Print("Error in BorraSlotSock: " & ex.Message)
        End Try
    End Sub

    Public Sub LogApiSock(ByRef str_Renamed As String)
        Debug.Print("LogApiSock: " & str_Renamed)
    End Sub

End Module
