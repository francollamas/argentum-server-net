Attribute VB_Name = "wskapiAO"
Option Explicit

Public SockListen As Long

' //////////////// FUNCIONES PROPIAS DE LA IMPLEMENTACION DE SOCKETS /////////////////////
Public Function ListenForConnect(ByVal Port&, ByVal HWndToMsg&, ByVal Enlazar As String) As Long
    ' TODO SOCKETS: completar metodo. se usa mucho por fuera
    ListenForConnect = 0
End Function

Public Function apiclosesocket(ByVal S As Integer) As Integer
    ' TODO SOCKETS: completar metodo. se usa mucho por fuera
End Function

Public Function GetAscIP(ByVal inn As Long) As String
    ' TODO SOCKETS: revisar logica! es probable que
    GetAscIP = "255.255.255.255"
End Function
' //////////////// FUNCIONES PROPIAS DE LA IMPLEMENTACION DE SOCKETS /////////////////////


' IniciaWsApi: Inicializa la API de sockets, creando una ventana oculta para manejar mensajes de red.
' @param hwndParent: Handle de la ventana principal del programa.
Public Sub IniciaWsApi(ByVal hwndParent As Long)

End Sub

' LimpiaWsApi: Limpia los recursos utilizados por la API de sockets, destruyendo la ventana oculta y restaurando el procedimiento de ventana original.
Public Sub LimpiaWsApi()
End Sub

' BuscaSlotSock: Busca el slot asociado a un socket específico.
' @param S: Identificador del socket.
' @return: El número de slot asociado al socket o -1 si no se encuentra.
Public Function BuscaSlotSock(ByVal S As Long) As Long
End Function

' AgregaSlotSock: Asocia un socket a un slot en la colección WSAPISock2Usr.
' @param Sock: Identificador del socket.
' @param Slot: Número del slot a asociar.
Public Sub AgregaSlotSock(ByVal Sock As Long, ByVal Slot As Long)
End Sub

' BorraSlotSock: Elimina la asociación de un socket con un slot en la colección WSAPISock2Usr.
' @param Sock: Identificador del socket a eliminar.
Public Sub BorraSlotSock(ByVal Sock As Long)
End Sub

' WndProc: Procedimiento de ventana para manejar mensajes de red.
' @param hWnd: Handle de la ventana.
' @param msg: Mensaje recibido.
' @param wParam: Parámetro adicional del mensaje.
' @param lParam: Parámetro adicional del mensaje.
' @return: Resultado del procesamiento del mensaje.
Public Function WndProc(ByVal hWnd As Long, ByVal msg As Long, ByVal wParam As Long, ByVal lParam As Long) As Long
' TODO SOCKETS: aca se manejaban los eventos de accept, read y close. Revisar en codigo viejo que podemos sacar...
' aqui hay logica necesaria para extraer
    WndProc = 0
End Function

' WsApiEnviar: Envía datos a través de un socket asociado a un slot.
' @param Slot: Número del slot.
' @param str: Cadena de datos a enviar.
' @return: Resultado del envío (0 si es exitoso, -1 si falla).
Public Function WsApiEnviar(ByVal Slot As Integer, ByRef str As String) As Long
' TODO SOCKETS: logica para envio de datos.
' aqui hay logica necesaria para extraer
End Function

' LogApiSock: Registra mensajes en un archivo de log para depuración.
' @param str: Mensaje a registrar.
Public Sub LogApiSock(ByVal str As String)

End Sub

' EventoSockAccept: Maneja el evento de aceptación de una nueva conexión.
' @param SockID: Identificador del socket que recibió la conexión.
Public Sub EventoSockAccept(ByVal SockID As Long)
' TODO SOCKETS: logica para aceptar un socket.
' aqui hay logica necesaria para extraer (logica de aplicacion: BAN, etc)
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
    Call sleep(100)
    Call IniciaWsApi(frmMain.hWnd)
    SockListen = ListenForConnect(Puerto, frmMain.hWnd, "")
End Sub

' WSApiCloseSocket: Cierra un socket de manera controlada.
' @param Socket: Identificador del socket a cerrar.
Public Sub WSApiCloseSocket(ByVal Socket As Long)

End Sub
