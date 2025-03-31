Option Strict Off
Option Explicit On
Module wskapiAO
	
	Private Const MAX_INT As Short = 32767
	Public WSAPISock2Usr As New Collection
	
	Public Function GetAscIP(ByVal inn As Integer) As String
		GetAscIP = frmMain.Winsock1(inn).RemoteHostIP
	End Function
	
	Public Function Winsock_ConnectionRequest(ByRef Index As Short, ByVal requestID As Integer) As Object
		Dim intNext As Short
		
		intNext = Winsock_NextOpenSocket
		
		If intNext <= 0 Then
			Exit Function
		End If
		
		'Found a socket to use; accept connection.
		frmMain.Winsock1(intNext).Accept(requestID)
		
		Dim NewIndex As Short
		NewIndex = NextOpenUser ' Nuevo indice
		
		Dim i As Short
		Dim data() As Byte
		'UPGRADE_NOTE: str se actualizó a str_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim str_Renamed As String
		If NewIndex <= MaxUsers Then
			' Leer para limpiar pendientes
			Call UserList(NewIndex).incomingData.ReadASCIIStringFixed(UserList(NewIndex).incomingData.length)
			Call UserList(NewIndex).outgoingData.ReadASCIIStringFixed(UserList(NewIndex).outgoingData.length)
			
			UserList(NewIndex).ip = GetAscIP(intNext)
			
			For i = 1 To BanIps.Count()
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto BanIps.Item(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
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
			
			str_Renamed = Protocol.PrepareMessageErrorMsg("El servidor se encuentra lleno en este momento. Disculpe las molestias ocasionadas.")
			
			ReDim Preserve data(Len(str_Renamed) - 1)
			
			frmMain.Winsock1(intNext).SendData(data)
			
			Call Winsock_Close(intNext)
		End If
		
	End Function
	
	Public Function Winsock_Close(ByRef Index As Short) As Object
		Dim N As Short
		N = BuscaSlotSock(Index)
		If Index > 0 Then frmMain.Winsock1(Index).Close()
		
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
		
		Dim intLoop As Short
		
		With frmMain
			.Winsock1(0).Close() 'Close first control.
			
			If .Winsock1.UBound > 0 Then
				'More than one Winsock control in the array.
				'Loop through and close/unload all of them.
				For intLoop = 1 To .Winsock1.UBound
					.Winsock1(intLoop).Close()
					.Winsock1.Unload(intLoop)
				Next intLoop
			End If
			
		End With
	End Sub
	
	'Finds an available Winsock control to use for an incoming connection.
	'You can just copy/paste this code into your chat program if you want.
	'Just change "sckServer" to the name of your Winsock control (array).
	'And change MAX_INT to max simultaneous connections that you want (it is at top of this module).
	Private Function Winsock_NextOpenSocket() As Short
		Dim intLoop, intFound As Short
		
		With frmMain
			'First, see if there is only one Winsock control.
			If .Winsock1.UBound = 0 Then
				'Just load #1.
				.Winsock1.Load(1)
				.Winsock1(1).Close()
				Winsock_NextOpenSocket = 1
			Else
				'There is more than 1.
				'Loop through all of them to find one not being used.
				'If it is not being used, it's state will = sckClosed (no connections).
				For intLoop = 1 To .Winsock1.UBound
					'UPGRADE_NOTE: State se actualizó a CtlState. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
					If .Winsock1(intLoop).CtlState = MSWinsockLib.StateConstants.sckClosed Then
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
						.Winsock1.Load(intFound)
						.Winsock1(intFound).Close()
						Winsock_NextOpenSocket = intFound
					Else
						'Server is full!
						Debug.Print("CONNECTION REJECTED! MAX CLIENTS (" & MAX_INT & ") REACHED!")
					End If
					
				End If
			End If
		End With
		
	End Function
	
	
	' IniciaWsApi: Inicializa la API de sockets, creando una ventana oculta para manejar mensajes de red.
	' @param hwndParent: Handle de la ventana principal del programa.
	Public Sub IniciaWsApi(ByVal port As Short)
		Winsock_Erase()
		
		frmMain.Winsock1(0).Close()
		frmMain.Winsock1(0).LocalPort = port
		frmMain.Winsock1(0).Listen()
	End Sub
	
	' LimpiaWsApi: Limpia los recursos utilizados por la API de sockets, destruyendo la ventana oculta y restaurando el procedimiento de ventana original.
	Public Sub LimpiaWsApi()
		Winsock_Erase()
	End Sub
	
	' WsApiEnviar: EnvÃ­a datos a travÃ©s de un socket asociado a un slot.
	' @param Slot: NÃºmero del slot.
	' @param str: Cadena de datos a enviar.
	' @return: Resultado del envÃ­o (0 si es exitoso, -1 si falla).
	'UPGRADE_NOTE: str se actualizó a str_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public Function WsApiEnviar(ByVal Slot As Short, ByRef str_Renamed As String) As Integer
		On Error GoTo ErrorHandler
		
		Dim Retorno As Integer
		Retorno = 0
		
		Dim slotIndex As Short
		If UserList(Slot).ConnID <> -1 And UserList(Slot).ConnIDValida Then
			slotIndex = UserList(Slot).ConnID
			frmMain.Winsock1(slotIndex).SendData(str_Renamed)
			System.Windows.Forms.Application.DoEvents()
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
	
	Public Function Winsock_DataArrival(ByRef Index As Short, ByVal bytesTotal As Integer) As Object
		Dim N As Short
		N = BuscaSlotSock(Index)
		
		If N < 0 And Index = 0 Then
			Call Winsock_Close(Index)
			Exit Function
		End If
		
		
		Dim datos() As Byte
		frmMain.Winsock1(Index).GetData(datos, VariantType.Array + VariantType.Byte, bytesTotal)
		
		Call EventoSockRead(N, datos)
	End Function
	
	' LogApiSock: Registra mensajes en un archivo de log para depuraciÃ³n.
	' @param str: Mensaje a registrar.
	'UPGRADE_NOTE: str se actualizó a str_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public Sub LogApiSock(ByVal str_Renamed As String)
		
	End Sub
	
	' EventoSockRead: Maneja el evento de lectura de datos desde un socket.
	' @param Slot: NÃºmero del slot asociado al socket.
	' @param Datos: Datos recibidos en formato de arreglo de bytes.
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
	
	' EventoSockClose: Maneja el evento de cierre de un socket.
	' @param Slot: NÃºmero del slot asociado al socket.
	Public Sub EventoSockClose(ByVal Slot As Short)
		If Centinela.RevisandoUserIndex = Slot Then Call modCentinela.CentinelaUserLogout()
		
		If UserList(Slot).flags.UserLogged Then
			Call CloseSocketSL(Slot)
			Call Cerrar_Usuario(Slot)
		Else
			Call CloseSocket(Slot)
		End If
	End Sub
	
	' WSApiReiniciarSockets: Reinicia todos los sockets, limpiando y reconfigurando los recursos.
	Public Sub WSApiReiniciarSockets()
		Dim i As Integer
		
		For i = 1 To MaxUsers
			If UserList(i).ConnID <> -1 And UserList(i).ConnIDValida Then
				Call CloseSocket(i)
			End If
		Next i
		
		For i = 1 To MaxUsers
			'UPGRADE_NOTE: El objeto UserList().incomingData no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
			UserList(i).incomingData = Nothing
			'UPGRADE_NOTE: El objeto UserList().outgoingData no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
			UserList(i).outgoingData = Nothing
		Next i

		'UPGRADE_WARNING: El límite inferior de la matriz UserList ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
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
	
	' BuscaSlotSock: Busca el slot asociado a un socket específico.
	' @param S: Identificador del socket.
	' @return: El número de slot asociado al socket o -1 si no se encuentra.
	Public Function BuscaSlotSock(ByVal S As Integer) As Integer
		On Error GoTo hayerror
		'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto WSAPISock2Usr.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		BuscaSlotSock = WSAPISock2Usr.Item(CStr(S))
		Exit Function
		
hayerror: 
		BuscaSlotSock = -1
	End Function
	
	' AgregaSlotSock: Asocia un socket a un slot en la colección WSAPISock2Usr.
	' @param Sock: Identificador del socket.
	' @param Slot: Número del slot a asociar.
	Public Sub AgregaSlotSock(ByVal Sock As Integer, ByVal Slot As Integer)
		Debug.Print("AgregaSockSlot")
		
		If WSAPISock2Usr.Count() > MaxUsers Then
			Call CloseSocket(Slot)
			Exit Sub
		End If
		
		WSAPISock2Usr.Add(CStr(Slot), CStr(Sock))
	End Sub
	
	' BorraSlotSock: Elimina la asociación de un socket con un slot en la colección WSAPISock2Usr.
	' @param Sock: Identificador del socket a eliminar.
	Public Sub BorraSlotSock(ByVal Sock As Integer)
		Dim cant As Integer
		cant = WSAPISock2Usr.Count()
		
		On Error Resume Next
		WSAPISock2Usr.Remove(CStr(Sock))
		
		Debug.Print("BorraSockSlot " & cant & " -> " & WSAPISock2Usr.Count())
	End Sub
End Module