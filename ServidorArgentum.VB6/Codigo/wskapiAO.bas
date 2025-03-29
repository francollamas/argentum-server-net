Attribute VB_Name = "wskapiAO"
Option Explicit

Private Const SD_BOTH As Long = &H2

Public Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Long)

Public Declare Function GetWindowLong Lib "user32" Alias "GetWindowLongA" (ByVal hWnd As Long, ByVal nIndex As Long) As Long
Public Declare Function SetWindowLong Lib "user32" Alias "SetWindowLongA" (ByVal hWnd As Long, ByVal nIndex As Long, ByVal dwNewLong As Long) As Long
Public Declare Function CallWindowProc Lib "user32" Alias "CallWindowProcA" (ByVal lpPrevWndFunc As Long, ByVal hWnd As Long, ByVal msg As Long, ByVal wParam As Long, ByVal lParam As Long) As Long

Private Declare Function CreateWindowEx Lib "user32" Alias "CreateWindowExA" (ByVal dwExStyle As Long, ByVal lpClassName As String, ByVal lpWindowName As String, ByVal dwStyle As Long, ByVal X As Long, ByVal Y As Long, ByVal nWidth As Long, ByVal nHeight As Long, ByVal hwndParent As Long, ByVal hMenu As Long, ByVal hInstance As Long, lpParam As Any) As Long
Private Declare Function DestroyWindow Lib "user32" (ByVal hWnd As Long) As Long
Private Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (ByRef destination As Any, ByRef source As Any, ByVal length As Long)

Private Const WS_CHILD = &H40000000
Public Const GWL_WNDPROC = (-4)

Private Const SIZE_RCVBUF As Long = 8192
Private Const SIZE_SNDBUF As Long = 8192

Public Type tSockCache
    Sock As Long
    Slot As Long
End Type

Public WSAPISock2Usr As New Collection

Public OldWProc As Long
Public ActualWProc As Long
Public hWndMsg As Long

Public SockListen As Long

' IniciaWsApi: Inicializa la API de sockets, creando una ventana oculta para manejar mensajes de red.
' @param hwndParent: Handle de la ventana principal del programa.
Public Sub IniciaWsApi(ByVal hwndParent As Long)
    Call LogApiSock("IniciaWsApi")
    Debug.Print "IniciaWsApi"

    hWndMsg = CreateWindowEx(0, "STATIC", "AOMSG", WS_CHILD, 0, 0, 0, 0, hwndParent, 0, App.hInstance, ByVal 0&)

    OldWProc = SetWindowLong(hWndMsg, GWL_WNDPROC, AddressOf WndProc)
    ActualWProc = GetWindowLong(hWndMsg, GWL_WNDPROC)

    Dim desc As String
    Call StartWinsock(desc)
End Sub

' LimpiaWsApi: Limpia los recursos utilizados por la API de sockets, destruyendo la ventana oculta y restaurando el procedimiento de ventana original.
Public Sub LimpiaWsApi()
    Call LogApiSock("LimpiaWsApi")

    If WSAStartedUp Then
        Call EndWinsock
    End If

    If OldWProc <> 0 Then
        SetWindowLong hWndMsg, GWL_WNDPROC, OldWProc
        OldWProc = 0
    End If

    If hWndMsg <> 0 Then
        DestroyWindow hWndMsg
    End If
End Sub

' BuscaSlotSock: Busca el slot asociado a un socket específico.
' @param S: Identificador del socket.
' @return: El número de slot asociado al socket o -1 si no se encuentra.
Public Function BuscaSlotSock(ByVal S As Long) As Long
    On Error GoTo hayerror
    BuscaSlotSock = WSAPISock2Usr.Item(CStr(S))
    Exit Function

hayerror:
    BuscaSlotSock = -1
End Function

' AgregaSlotSock: Asocia un socket a un slot en la colección WSAPISock2Usr.
' @param Sock: Identificador del socket.
' @param Slot: Número del slot a asociar.
Public Sub AgregaSlotSock(ByVal Sock As Long, ByVal Slot As Long)
    Debug.Print "AgregaSockSlot"

    If WSAPISock2Usr.Count > MaxUsers Then
        Call CloseSocket(Slot)
        Exit Sub
    End If

    WSAPISock2Usr.Add CStr(Slot), CStr(Sock)
End Sub

' BorraSlotSock: Elimina la asociación de un socket con un slot en la colección WSAPISock2Usr.
' @param Sock: Identificador del socket a eliminar.
Public Sub BorraSlotSock(ByVal Sock As Long)
    Dim cant As Long
    cant = WSAPISock2Usr.Count

    On Error Resume Next
    WSAPISock2Usr.Remove CStr(Sock)

    Debug.Print "BorraSockSlot " & cant & " -> " & WSAPISock2Usr.Count
End Sub

' WndProc: Procedimiento de ventana para manejar mensajes de red.
' @param hWnd: Handle de la ventana.
' @param msg: Mensaje recibido.
' @param wParam: Parámetro adicional del mensaje.
' @param lParam: Parámetro adicional del mensaje.
' @return: Resultado del procesamiento del mensaje.
Public Function WndProc(ByVal hWnd As Long, ByVal msg As Long, ByVal wParam As Long, ByVal lParam As Long) As Long
    On Error Resume Next

    Dim Ret As Long
    Dim Tmp() As Byte
    Dim S As Long
    Dim E As Long
    Dim N As Integer
    Dim UltError As Long

    Select Case msg
        Case 1025
            S = wParam
            E = WSAGetSelectEvent(lParam)

            Select Case E
                Case FD_ACCEPT
                    If S = SockListen Then
                        Call EventoSockAccept(S)
                    End If

                Case FD_READ
                    N = BuscaSlotSock(S)
                    If N < 0 And S <> SockListen Then
                        Call WSApiCloseSocket(S)
                        Exit Function
                    End If

                    ReDim Preserve Tmp(SIZE_RCVBUF - 1) As Byte

                    Ret = recv(S, Tmp(0), SIZE_RCVBUF, 0)
                    If Ret < 0 Then
                        UltError = Err.LastDllError
                        If UltError = WSAEMSGSIZE Then
                            Debug.Print "WSAEMSGSIZE"
                            Ret = SIZE_RCVBUF
                        Else
                            Debug.Print "Error en Recv: " & GetWSAErrorString(UltError)
                            Call LogApiSock("Error en Recv: N=" & N & " S=" & S & " Str=" & GetWSAErrorString(UltError))

                            Call CloseSocketSL(N)
                            Call Cerrar_Usuario(N)
                            Exit Function
                        End If
                    ElseIf Ret = 0 Then
                        Call CloseSocketSL(N)
                        Call Cerrar_Usuario(N)
                    End If

                    ReDim Preserve Tmp(Ret - 1) As Byte

                    Call EventoSockRead(N, Tmp)

                Case FD_CLOSE
                    N = BuscaSlotSock(S)
                    If S <> SockListen Then Call apiclosesocket(S)

                    If N > 0 Then
                        Call BorraSlotSock(S)
                        UserList(N).ConnID = -1
                        UserList(N).ConnIDValida = False
                        Call EventoSockClose(N)
                    End If
            End Select

        Case Else
            WndProc = CallWindowProc(OldWProc, hWnd, msg, wParam, lParam)
    End Select
End Function

' WsApiEnviar: Envía datos a través de un socket asociado a un slot.
' @param Slot: Número del slot.
' @param str: Cadena de datos a enviar.
' @return: Resultado del envío (0 si es exitoso, -1 si falla).
Public Function WsApiEnviar(ByVal Slot As Integer, ByRef str As String) As Long
    Dim Ret As String
    Dim Retorno As Long
    Dim data() As Byte

    ReDim Preserve data(Len(str) - 1) As Byte
    data = StrConv(str, vbFromUnicode)

    Retorno = 0

    If UserList(Slot).ConnID <> -1 And UserList(Slot).ConnIDValida Then
        Ret = send(ByVal UserList(Slot).ConnID, data(0), ByVal UBound(data()) + 1, ByVal 0)
        If Ret < 0 Then
            Ret = Err.LastDllError
            If Ret = WSAEWOULDBLOCK Then
                Call UserList(Slot).outgoingData.WriteASCIIStringFixed(str)
            End If
        End If
    ElseIf UserList(Slot).ConnID <> -1 And Not UserList(Slot).ConnIDValida Then
        If Not UserList(Slot).Counters.Saliendo Then
            Retorno = -1
        End If
    End If

    WsApiEnviar = Retorno
End Function

' LogApiSock: Registra mensajes en un archivo de log para depuración.
' @param str: Mensaje a registrar.
Public Sub LogApiSock(ByVal str As String)
    On Error GoTo Errhandler

    Dim nfile As Integer
    nfile = FreeFile
    Open App.Path & "\logs\wsapi.log" For Append Shared As #nfile
    Print #nfile, Date & " " & time & " " & str
    Close #nfile

    Exit Sub

Errhandler:
End Sub

' EventoSockAccept: Maneja el evento de aceptación de una nueva conexión.
' @param SockID: Identificador del socket que recibió la conexión.
Public Sub EventoSockAccept(ByVal SockID As Long)
    Dim NewIndex As Integer
    Dim Ret As Long
    Dim Tam As Long, sa As sockaddr
    Dim NuevoSock As Long
    Dim i As Long
    Dim tStr As String

    Tam = sockaddr_size

    Ret = accept(SockID, sa, Tam)

    If Ret = INVALID_SOCKET Then
        i = Err.LastDllError
        Call LogCriticEvent("Error en Accept() API " & i & ": " & GetWSAErrorString(i))
        Exit Sub
    End If

    If Not SecurityIp.IpSecurityAceptarNuevaConexion(sa.sin_addr) Then
        Call WSApiCloseSocket(NuevoSock)
        Exit Sub
    End If

    NuevoSock = Ret

    If setsockopt(NuevoSock, SOL_SOCKET, SO_RCVBUFFER, SIZE_RCVBUF, 4) <> 0 Then
        i = Err.LastDllError
        Call LogCriticEvent("Error al setear el tamaño del buffer de entrada " & i & ": " & GetWSAErrorString(i))
    End If

    If setsockopt(NuevoSock, SOL_SOCKET, SO_SNDBUFFER, SIZE_SNDBUF, 4) <> 0 Then
        i = Err.LastDllError
        Call LogCriticEvent("Error al setear el tamaño del buffer de salida " & i & ": " & GetWSAErrorString(i))
    End If

    NewIndex = NextOpenUser

    If NewIndex <= MaxUsers Then
        Call UserList(NewIndex).incomingData.ReadASCIIStringFixed(UserList(NewIndex).incomingData.length)
        Call UserList(NewIndex).outgoingData.ReadASCIIStringFixed(UserList(NewIndex).outgoingData.length)

        UserList(NewIndex).ip = GetAscIP(sa.sin_addr)

        For i = 1 To BanIps.Count
            If BanIps.Item(i) = UserList(NewIndex).ip Then
                Call WriteErrorMsg(NewIndex, "Su IP se encuentra bloqueada en este servidor.")
                Call FlushBuffer(NewIndex)
                Call WSApiCloseSocket(NuevoSock)
                Exit Sub
            End If
        Next i

        If NewIndex > LastUser Then LastUser = NewIndex

        UserList(NewIndex).ConnID = NuevoSock
        UserList(NewIndex).ConnIDValida = True

        Call AgregaSlotSock(NuevoSock, NewIndex)
    Else
        Dim str As String
        Dim data() As Byte

        str = Protocol.PrepareMessageErrorMsg("El servidor se encuentra lleno en este momento. Disculpe las molestias ocasionadas.")

        ReDim Preserve data(Len(str) - 1) As Byte

        data = StrConv(str, vbFromUnicode)

        Call send(ByVal NuevoSock, data(0), ByVal UBound(data()) + 1, ByVal 0)
        Call WSApiCloseSocket(NuevoSock)
    End If
End Sub

' EventoSockRead: Maneja el evento de lectura de datos desde un socket.
' @param Slot: Número del slot asociado al socket.
' @param Datos: Datos recibidos en formato de arreglo de bytes.
Public Sub EventoSockRead(ByVal Slot As Integer, ByRef Datos() As Byte)
    With UserList(Slot)
        Call .incomingData.WriteBlock(Datos)

        If .ConnID <> -1 Then
            Call HandleIncomingData(Slot)
        Else
            Exit Sub
        End If
    End With
End Sub

' EventoSockClose: Maneja el evento de cierre de un socket.
' @param Slot: Número del slot asociado al socket.
Public Sub EventoSockClose(ByVal Slot As Integer)
    If Centinela.RevisandoUserIndex = Slot Then _
        Call modCentinela.CentinelaUserLogout

    If UserList(Slot).flags.UserLogged Then
        Call CloseSocketSL(Slot)
        Call Cerrar_Usuario(Slot)
    Else
        Call CloseSocket(Slot)
    End If
End Sub

' WSApiReiniciarSockets: Reinicia todos los sockets, limpiando y reconfigurando los recursos.
Public Sub WSApiReiniciarSockets()
    Dim i As Long

    If SockListen >= 0 Then Call apiclosesocket(SockListen)

    For i = 1 To MaxUsers
        If UserList(i).ConnID <> -1 And UserList(i).ConnIDValida Then
            Call CloseSocket(i)
        End If
    Next i

    For i = 1 To MaxUsers
        Set UserList(i).incomingData = Nothing
        Set UserList(i).outgoingData = Nothing
    Next i

    ReDim UserList(1 To MaxUsers)
    For i = 1 To MaxUsers
        UserList(i).ConnID = -1
        UserList(i).ConnIDValida = False

        Set UserList(i).incomingData = New clsByteQueue
        Set UserList(i).outgoingData = New clsByteQueue
    Next i

    LastUser = 1
    NumUsers = 0

    Call LimpiaWsApi
    Call Sleep(100)
    Call IniciaWsApi(frmMain.hWnd)
    SockListen = ListenForConnect(Puerto, hWndMsg, "")
End Sub

' WSApiCloseSocket: Cierra un socket de manera controlada.
' @param Socket: Identificador del socket a cerrar.
Public Sub WSApiCloseSocket(ByVal Socket As Long)
    Call WSAAsyncSelect(Socket, hWndMsg, ByVal 1025, ByVal (FD_CLOSE))
    Call ShutDown(Socket, SD_BOTH)
End Sub

' CondicionSocket: Determina si se acepta o rechaza una nueva conexión basada en la dirección IP.
' @param lpCallerId: Información del socket que solicita la conexión.
' @param lpCallerData: Datos adicionales del socket que solicita la conexión.
' @param lpSQOS: Especificaciones de calidad de servicio.
' @param Reserved: Reservado para uso futuro.
' @param lpCalleeId: Información del socket receptor.
' @param lpCalleeData: Datos adicionales del socket receptor.
' @param Group: Grupo de sockets.
' @param dwCallbackData: Datos adicionales para el callback.
' @return: CF_ACCEPT si se acepta la conexión, CF_REJECT si se rechaza.
Public Function CondicionSocket(ByRef lpCallerId As WSABUF, ByRef lpCallerData As WSABUF, ByRef lpSQOS As FLOWSPEC, ByVal Reserved As Long, ByRef lpCalleeId As WSABUF, ByRef lpCalleeData As WSABUF, ByRef Group As Long, ByVal dwCallbackData As Long) As Long
    Dim sa As sockaddr

    If dwCallbackData = 1 Then
        CondicionSocket = CF_REJECT
        Exit Function
    End If

    CopyMemory sa, ByVal lpCallerId.lpBuffer, lpCallerId.dwBufferLen

    If Not SecurityIp.IpSecurityAceptarNuevaConexion(sa.sin_addr) Then
        CondicionSocket = CF_REJECT
        Exit Function
    End If

    CondicionSocket = CF_ACCEPT
End Function
