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
        Debug.Print("New connection accepted: socket=" & socketId)
        Dim NewIndex As Short
        NewIndex = NextOpenUser() ' Nuevo indice

        If NewIndex <= MaxUsers Then
            ' Leer para limpiar pendientes
            Call UserList(NewIndex).incomingData.ReadASCIIStringFixed(UserList(NewIndex).incomingData.length)
            Call UserList(NewIndex).outgoingData.ReadASCIIStringFixed(UserList(NewIndex).outgoingData.length)

            UserList(NewIndex).ip = GetAscIP(socketId)
            Debug.Print("Connection from IP: " & UserList(NewIndex).ip)

            Dim i As Short
            For i = 1 To BanIps.Count()
                If BanIps.Item(i) = UserList(NewIndex).ip Then
                    Call WriteErrorMsg(NewIndex, "Su IP se encuentra bloqueada en este servidor.")
                    Call FlushBuffer(NewIndex)
                    Call CloseSocket(NewIndex)
                    Debug.Print("Banned IP rejected: " & UserList(NewIndex).ip)
                    Exit Sub
                End If
            Next i

            If NewIndex > LastUser Then LastUser = NewIndex

            UserList(NewIndex).ConnID = socketId
            UserList(NewIndex).ConnIDValida = True

            Call AgregaSlotSock(socketId, NewIndex)
        Else
            ' Server is full
            Dim str_Renamed As String = Protocol.PrepareMessageErrorMsg("El servidor se encuentra lleno en este momento. Disculpe las molestias ocasionadas.")

            socketManager.SendString(socketId, str_Renamed)
            socketManager.CloseSocket(socketId)
            Debug.Print("Server full - connection rejected")
        End If
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
            Dim data() As Byte = System.Text.Encoding.Default.GetBytes(str_Renamed)

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
        Dim N As Short = BuscaSlotSock(socketId)

        If N > 0 Then
            Call BorraSlotSock(socketId)
            UserList(N).ConnID = -1
            UserList(N).ConnIDValida = False
            Call EventoSockClose(N)
        End If
    End Sub

    Public Sub LogApiSock(ByVal str_Renamed As String)
        ' This can be implemented if logging is needed
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
        If Centinela.RevisandoUserIndex = Slot Then Call modCentinela.CentinelaUserLogout()

        If UserList(Slot).flags.UserLogged Then
            Call CloseSocketSL(Slot)
            Call Cerrar_Usuario(Slot)
        Else
            Call CloseSocket(Slot)
        End If
    End Sub

    Public Sub WSApiReiniciarSockets()
        Dim i As Integer

        For i = 1 To MaxUsers
            If UserList(i).ConnID <> -1 And UserList(i).ConnIDValida Then
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

        Call IniciaWsApi(Puerto)
    End Sub

    Public Function BuscaSlotSock(ByVal S As Integer) As Integer
        If S <= 0 Then
            Debug.Print("BuscaSlotSock: Invalid socket ID: " & S)
            Return -1
        End If

        Try
            If WSAPISock2Usr.ContainsKey(S) Then
                Return WSAPISock2Usr(S)
            Else
                Debug.Print("BuscaSlotSock: Socket " & S & " not found in dictionary")
                Return -1
            End If
        Catch ex As Exception
            Debug.Print("BuscaSlotSock error: " & ex.Message)
            Return -1
        End Try
    End Function

    Public Sub AgregaSlotSock(ByVal Sock As Integer, ByVal Slot As Integer)
        Debug.Print("AgregaSockSlot: Socket=" & Sock & ", Slot=" & Slot)

        If Sock <= 0 Then
            Debug.Print("ERROR: Attempted to add invalid socket ID: " & Sock)
            Exit Sub
        End If

        If WSAPISock2Usr.Count >= MaxUsers Then
            Debug.Print("WARNING: Socket dictionary full, closing connection")
            Call CloseSocket(Slot)
            Exit Sub
        End If

        Try
            ' Check if any other socket is already assigned to this slot
            Dim socketsToRemove As New List(Of Integer)

            For Each kvp As KeyValuePair(Of Integer, Integer) In WSAPISock2Usr
                If kvp.Value = Slot AndAlso kvp.Key <> Sock Then
                    Debug.Print("WARNING: Slot " & Slot & " already mapped to socket " & kvp.Key)
                    socketsToRemove.Add(kvp.Key)
                End If
            Next

            ' Remove any sockets that were assigned to this slot
            For Each sockToRemove As Integer In socketsToRemove
                WSAPISock2Usr.Remove(sockToRemove)
                Debug.Print("Removed existing mapping for socket " & sockToRemove)
            Next

            ' Check if this socket already exists
            If WSAPISock2Usr.ContainsKey(Sock) Then
                Dim existingSlot As Integer = WSAPISock2Usr(Sock)

                ' Socket already exists, remove it first
                Debug.Print("Socket already in use, removing old entry")
                WSAPISock2Usr.Remove(Sock)

                ' If there was a user with this socket, clean up that user
                If existingSlot > 0 AndAlso existingSlot <> Slot Then
                    Debug.Print("Cleaning up old user in slot " & existingSlot)
                    UserList(existingSlot).ConnID = -1
                    UserList(existingSlot).ConnIDValida = False
                End If
            End If

            ' Make sure this user slot is valid
            If Slot <= 0 OrElse Slot > MaxUsers Then
                Debug.Print("ERROR: Invalid slot number: " & Slot)
                If socketManager IsNot Nothing Then socketManager.CloseSocket(Sock)
                Exit Sub
            End If

            ' Finally add the mapping
            WSAPISock2Usr.Add(Sock, Slot)
            Debug.Print("Added socket " & Sock & " with slot " & Slot & " to dictionary, count=" & WSAPISock2Usr.Count)

        Catch ex As Exception
            Debug.Print("Error in AgregaSlotSock: " & ex.Message)
            If socketManager IsNot Nothing Then socketManager.CloseSocket(Sock)
        End Try
    End Sub

    Public Sub BorraSlotSock(ByVal Sock As Integer)
        Debug.Print("BorraSlotSock: " & Sock)
        Dim cant As Integer = WSAPISock2Usr.Count

        Try
            If WSAPISock2Usr.ContainsKey(Sock) Then
                WSAPISock2Usr.Remove(Sock)
                Debug.Print("BorraSockSlot " & cant & " -> " & WSAPISock2Usr.Count)
            Else
                Debug.Print("Socket " & Sock & " not found in dictionary")
            End If
        Catch ex As Exception
            Debug.Print("Error removing socket " & Sock & " from dictionary: " & ex.Message)
        End Try
    End Sub
End Module
