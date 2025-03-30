Attribute VB_Name = "wskapiAO"
Option Explicit

Private Const MAX_INT As Integer = 32767
Public WSAPISock2Usr As New Collection

Public Function GetAscIP(ByVal inn As Long) As String
    GetAscIP = frmMain.Winsock1(inn).RemoteHostIP
End Function

Public Function Winsock_ConnectionRequest(Index As Integer, ByVal requestID As Long)
    Dim intNext As Integer
    
    intNext = Winsock_NextOpenSocket
    
    If intNext <= 0 Then
        Exit Function
    End If
    
    'Found a socket to use; accept connection.
    frmMain.Winsock1(intNext).Accept requestID

    Dim NewIndex As Integer
    NewIndex = NextOpenUser ' Nuevo indice
    
    If NewIndex <= MaxUsers Then
        ' Leer para limpiar pendientes
        Call UserList(NewIndex).incomingData.ReadASCIIStringFixed(UserList(NewIndex).incomingData.length)
        Call UserList(NewIndex).outgoingData.ReadASCIIStringFixed(UserList(NewIndex).outgoingData.length)

        UserList(NewIndex).ip = GetAscIP(intNext)

        Dim i As Integer
        For i = 1 To BanIps.Count
            If BanIps.Item(i) = UserList(NewIndex).ip Then
                Call WriteErrorMsg(NewIndex, "Su IP se encuentra bloqueada en este servidor.")
                Call FlushBuffer(NewIndex)
                Call Winsock_Close(intNext)
                Exit Function
            End If
        Next i

        If NewIndex > LastUser Then LastUser = NewIndex

        UserList(NewIndex).ConnID = intNext
        UserList(NewIndex).ConnIDValida = True
        
        Call AgregaSlotSock(intNext, NewIndex)
    Else
        Dim str As String
        Dim data() As Byte

        str = Protocol.PrepareMessageErrorMsg("El servidor se encuentra lleno en este momento. Disculpe las molestias ocasionadas.")

        ReDim Preserve data(Len(str) - 1) As Byte

        frmMain.Winsock1(intNext).SendData data
        
        Call Winsock_Close(intNext)
    End If
    
End Function

Public Function Winsock_Close(Index As Integer)
    Dim N As Integer
    N = BuscaSlotSock(Index)
    If Index > 0 Then frmMain.Winsock1(Index).Close
    
    If N > 0 Then
        Call BorraSlotSock(Index)
        UserList(N).ConnID = -1
        UserList(N).ConnIDValida = False
        Call EventoSockClose(N)
    End If
End Function

Public Sub Winsock_Erase()
    'Steps:
    '------
    '1. Unload & close all Winsock controls.
    '2. Erase udtUsers() array to clear up memory.
    
    Dim intLoop As Integer
    
    With frmMain
        .Winsock1(0).Close 'Close first control.
        
        If .Winsock1.UBound > 0 Then
            'More than one Winsock control in the array.
            'Loop through and close/unload all of them.
            For intLoop = 1 To .Winsock1.UBound
                .Winsock1(intLoop).Close
                Unload .Winsock1(intLoop)
            Next intLoop
        End If
    
    End With
    
    ' TODO: hacer erase en la data de los userss!!
End Sub

'Finds an available Winsock control to use for an incoming connection.
'You can just copy/paste this code into your chat program if you want.
'Just change "sckServer" to the name of your Winsock control (array).
'And change MAX_INT to max simultaneous connections that you want (it is at top of this module).
Private Function Winsock_NextOpenSocket() As Integer
    Dim intLoop As Integer, intFound As Integer
    
    With frmMain
        'First, see if there is only one Winsock control.
        If .Winsock1.UBound = 0 Then
            'Just load #1.
            Load .Winsock1(1)
            .Winsock1(1).Close
            Winsock_NextOpenSocket = 1
        Else
            'There is more than 1.
            'Loop through all of them to find one not being used.
            'If it is not being used, it's state will = sckClosed (no connections).
            For intLoop = 1 To .Winsock1.UBound
                If .Winsock1(intLoop).State = sckClosed Then
                    'Found one not being used.
                    intFound = intLoop
                    Exit For
                End If
            Next intLoop
            
            'Check if we found one.
            If intFound > 0 Then
                Winsock_NextOpenSocket = intFound
            Else
                'Didn't find one.
                'Load a new one.
                'Unless we reached MAX_INT
                'which is max number of clients.
                If .Winsock1.UBound + 1 < MAX_INT Then
                    'There is room for another one.
                    intFound = .Winsock1.UBound + 1
                    Load .Winsock1(intFound)
                    .Winsock1(intFound).Close
                    Winsock_NextOpenSocket = intFound
                Else
                    'Server is full!
                    Debug.Print "CONNECTION REJECTED! MAX CLIENTS (" & MAX_INT & ") REACHED!"
                End If
            
            End If
        End If
    End With
    
End Function


' IniciaWsApi: Inicializa la API de sockets, creando una ventana oculta para manejar mensajes de red.
' @param hwndParent: Handle de la ventana principal del programa.
Public Sub IniciaWsApi(ByVal port As Integer)
    Winsock_Erase
    
    frmMain.Winsock1(0).Close
    frmMain.Winsock1(0).LocalPort = port
    frmMain.Winsock1(0).Listen
End Sub

' LimpiaWsApi: Limpia los recursos utilizados por la API de sockets, destruyendo la ventana oculta y restaurando el procedimiento de ventana original.
Public Sub LimpiaWsApi()
    Winsock_Erase
End Sub

' WsApiEnviar: EnvÃ­a datos a travÃ©s de un socket asociado a un slot.
' @param Slot: NÃºmero del slot.
' @param str: Cadena de datos a enviar.
' @return: Resultado del envÃ­o (0 si es exitoso, -1 si falla).
Public Function WsApiEnviar(ByVal Slot As Integer, ByRef str As String) As Long
    On Error GoTo ErrorHandler
    
    Dim Retorno As Long
    Retorno = 0

    If UserList(Slot).ConnID <> -1 And UserList(Slot).ConnIDValida Then
        Dim slotIndex As Integer
        slotIndex = UserList(Slot).ConnID
        frmMain.Winsock1(slotIndex).SendData str
        DoEvents
    ElseIf UserList(Slot).ConnID <> -1 And Not UserList(Slot).ConnIDValida Then
        If Not UserList(Slot).Counters.Saliendo Then
            Retorno = -1
        End If
    End If

    WsApiEnviar = Retorno
    
    Exit Function
    
ErrorHandler:
    Call UserList(Slot).outgoingData.WriteASCIIStringFixed(str)
    Resume Next

End Function

Public Function Winsock_DataArrival(Index As Integer, ByVal bytesTotal As Long)
    Dim N As Integer
    N = BuscaSlotSock(Index)
    
    If N < 0 And Index = 0 Then
        Call Winsock_Close(Index)
        Exit Function
    End If
    
    
    Dim datos() As Byte
    frmMain.Winsock1(Index).GetData datos, vbArray + vbByte, bytesTotal
    
    Call EventoSockRead(N, datos)
End Function

' LogApiSock: Registra mensajes en un archivo de log para depuraciÃ³n.
' @param str: Mensaje a registrar.
Public Sub LogApiSock(ByVal str As String)

End Sub

' EventoSockRead: Maneja el evento de lectura de datos desde un socket.
' @param Slot: NÃºmero del slot asociado al socket.
' @param Datos: Datos recibidos en formato de arreglo de bytes.
Public Sub EventoSockRead(ByVal Slot As Integer, ByRef datos() As Byte)
    With UserList(Slot)
        Call .incomingData.WriteBlock(datos)

        If .ConnID <> -1 Then
            Call HandleIncomingData(Slot)
        Else
            Exit Sub
        End If
    End With
End Sub

' EventoSockClose: Maneja el evento de cierre de un socket.
' @param Slot: NÃºmero del slot asociado al socket.
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

    Call IniciaWsApi(Puerto)
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
