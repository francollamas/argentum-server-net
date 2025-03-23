Option Strict Off
Option Explicit On
Module wskapiAO
	'**************************************************************
	' wskapiAO.bas
	'
	'**************************************************************
	
	'**************************************************************************
	'This program is free software; you can redistribute it and/or modify
	'it under the terms of the Affero General Public License;
	'either version 1 of the License, or any later version.
	'
	'This program is distributed in the hope that it will be useful,
	'but WITHOUT ANY WARRANTY; without even the implied warranty of
	'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	'Affero General Public License for more details.
	'
	'You should have received a copy of the Affero General Public License
	'along with this program; if not, you can find it at http://www.affero.org/oagpl.html
	'**************************************************************************
	
	
	''
	' Modulo para manejar Winsock
	'
	
#If UsarQueSocket = 1 Then
	
	
	'Si la variable esta en TRUE , al iniciar el WsApi se crea
	'una ventana LABEL para recibir los mensajes. Al detenerlo,
	'se destruye.
	'Si es FALSE, los mensajes se envian al form frmMain (o el
	'que sea).
#Const WSAPI_CREAR_LABEL = True
	
	Private Const SD_BOTH As Integer = &H2
	
	Public Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Integer)
	
	Public Declare Function GetWindowLong Lib "user32"  Alias "GetWindowLongA"(ByVal hWnd As Integer, ByVal nIndex As Integer) As Integer
	Public Declare Function SetWindowLong Lib "user32"  Alias "SetWindowLongA"(ByVal hWnd As Integer, ByVal nIndex As Integer, ByVal dwNewLong As Integer) As Integer
	Public Declare Function CallWindowProc Lib "user32"  Alias "CallWindowProcA"(ByVal lpPrevWndFunc As Integer, ByVal hWnd As Integer, ByVal msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
	
	'UPGRADE_ISSUE: No se admite la declaración de un parámetro 'As Any'. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	Private Declare Function CreateWindowEx Lib "user32"  Alias "CreateWindowExA"(ByVal dwExStyle As Integer, ByVal lpClassName As String, ByVal lpWindowName As String, ByVal dwStyle As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hwndParent As Integer, ByVal hMenu As Integer, ByVal hInstance As Integer, ByRef lpParam As Any) As Integer
	Private Declare Function DestroyWindow Lib "user32" (ByVal hWnd As Integer) As Integer
	
	Private Const WS_CHILD As Integer = &H40000000
	Public Const GWL_WNDPROC As Short = (-4)
	
	Private Const SIZE_RCVBUF As Integer = 8192
	Private Const SIZE_SNDBUF As Integer = 8192
	
	''
	'Esto es para agilizar la busqueda del slot a partir de un socket dado,
	'sino, la funcion BuscaSlotSock se nos come todo el uso del CPU.
	'
	' @param Sock sock
	' @param slot slot
	'
	Public Structure tSockCache
		Dim Sock As Integer
		Dim Slot As Integer
	End Structure
	
	Public WSAPISock2Usr As New Collection
	
	' ====================================================================================
	' ====================================================================================
	
	Public OldWProc As Integer
	Public ActualWProc As Integer
	Public hWndMsg As Integer
	
	' ====================================================================================
	' ====================================================================================
	
	Public SockListen As Integer
	
#End If
	
	' ====================================================================================
	' ====================================================================================
	
	
	Public Sub IniciaWsApi(ByVal hwndParent As Integer)
#If UsarQueSocket = 1 Then
		
		Call LogApiSock("IniciaWsApi")
		Debug.Print("IniciaWsApi")
		
#If WSAPI_CREAR_LABEL Then
		hWndMsg = CreateWindowEx(0, "STATIC", "AOMSG", WS_CHILD, 0, 0, 0, 0, hwndParent, 0, VB6.GetHInstance.ToInt32, 0)
#Else
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Else no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		hWndMsg = hwndParent
#End If 'WSAPI_CREAR_LABEL
		
		'UPGRADE_WARNING: Agregue un delegado para el operador AddressOf WndProc Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="E9E157F7-EF0C-4016-87B7-7D7FBBC6EE08"'
		OldWProc = SetWindowLong(hWndMsg, GWL_WNDPROC, AddressOf WndProc)
		ActualWProc = GetWindowLong(hWndMsg, GWL_WNDPROC)
		
		Dim desc As String
		Call StartWinsock(desc)
		
#End If
	End Sub
	
	Public Sub LimpiaWsApi()
#If UsarQueSocket = 1 Then
		
		Call LogApiSock("LimpiaWsApi")
		
		If WSAStartedUp Then
			Call EndWinsock()
		End If
		
		If OldWProc <> 0 Then
			SetWindowLong(hWndMsg, GWL_WNDPROC, OldWProc)
			OldWProc = 0
		End If
		
#If WSAPI_CREAR_LABEL Then
		If hWndMsg <> 0 Then
			DestroyWindow(hWndMsg)
		End If
#End If
		
#End If
	End Sub
	
	Public Function BuscaSlotSock(ByVal S As Integer) As Integer
#If UsarQueSocket = 1 Then
		
		On Error GoTo hayerror
		
		'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto WSAPISock2Usr.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		BuscaSlotSock = WSAPISock2Usr.Item(CStr(S))
		Exit Function
		
hayerror: 
		BuscaSlotSock = -1
#End If
		
	End Function
	
	Public Sub AgregaSlotSock(ByVal Sock As Integer, ByVal Slot As Integer)
		Debug.Print("AgregaSockSlot")
#If (UsarQueSocket = 1) Then
		
		If WSAPISock2Usr.Count() > MaxUsers Then
			Call CloseSocket(Slot)
			Exit Sub
		End If
		
		WSAPISock2Usr.Add(CStr(Slot), CStr(Sock))
		
		'Dim Pri As Long, Ult As Long, Med As Long
		'Dim LoopC As Long
		'
		'If WSAPISockChacheCant > 0 Then
		'    Pri = 1
		'    Ult = WSAPISockChacheCant
		'    Med = Int((Pri + Ult) / 2)
		'
		'    Do While (Pri <= Ult) And (Ult > 1)
		'        If Sock < WSAPISockChache(Med).Sock Then
		'            Ult = Med - 1
		'        Else
		'            Pri = Med + 1
		'        End If
		'        Med = Int((Pri + Ult) / 2)
		'    Loop
		'
		'    Pri = IIf(Sock < WSAPISockChache(Med).Sock, Med, Med + 1)
		'    Ult = WSAPISockChacheCant
		'    For LoopC = Ult To Pri Step -1
		'        WSAPISockChache(LoopC + 1) = WSAPISockChache(LoopC)
		'    Next LoopC
		'    Med = Pri
		'Else
		'    Med = 1
		'End If
		'WSAPISockChache(Med).Slot = Slot
		'WSAPISockChache(Med).Sock = Sock
		'WSAPISockChacheCant = WSAPISockChacheCant + 1
		
#End If
	End Sub
	
	Public Sub BorraSlotSock(ByVal Sock As Integer)
#If (UsarQueSocket = 1) Then
		Dim cant As Integer
		
		cant = WSAPISock2Usr.Count()
		On Error Resume Next
		WSAPISock2Usr.Remove(CStr(Sock))
		
		Debug.Print("BorraSockSlot " & cant & " -> " & WSAPISock2Usr.Count())
		
#End If
	End Sub
	
	
	
	Public Function WndProc(ByVal hWnd As Integer, ByVal msg As Integer, ByVal wParam As Integer, ByVal lParam As Integer) As Integer
#If UsarQueSocket = 1 Then
		
		On Error Resume Next
		
		Dim Ret As Integer
		Dim Tmp() As Byte
		Dim S As Integer
		Dim E As Integer
		Dim N As Short
		Dim UltError As Integer
		
		Select Case msg
			Case 1025
				S = wParam
				E = WSAGetSelectEvent(lParam)
				
				Select Case E
					Case FD_ACCEPT
						If S = SockListen Then
							Call EventoSockAccept(S)
						End If
						
						'    Case FD_WRITE
						'        N = BuscaSlotSock(s)
						'        If N < 0 And s <> SockListen Then
						'            'Call apiclosesocket(s)
						'            call WSApiCloseSocket(s)
						'            Exit Function
						'        End If
						'
						
						'        Call IntentarEnviarDatosEncolados(N)
						'
						'        Dale = UserList(N).ColaSalida.Count > 0
						'        Do While Dale
						'            Ret = WsApiEnviar(N, UserList(N).ColaSalida.Item(1), False)
						'            If Ret <> 0 Then
						'                If Ret = WSAEWOULDBLOCK Then
						'                    Dale = False
						'                Else
						'                    'y aca que hacemo' ?? help! i need somebody, help!
						'                    Dale = False
						'                    Debug.Print "ERROR AL ENVIAR EL DATO DESDE LA COLA " & Ret & ": " & GetWSAErrorString(Ret)
						'                End If
						'            Else
						'            '    Debug.Print "Dato de la cola enviado"
						'                UserList(N).ColaSalida.Remove 1
						'                Dale = (UserList(N).ColaSalida.Count > 0)
						'            End If
						'        Loop
						
					Case FD_READ
						N = BuscaSlotSock(S)
						If N < 0 And S <> SockListen Then
							'Call apiclosesocket(s)
							Call WSApiCloseSocket(S)
							Exit Function
						End If
						
						'create appropiate sized buffer
						ReDim Preserve Tmp(SIZE_RCVBUF - 1)
						
						Ret = recv(S, Tmp(0), SIZE_RCVBUF, 0)
						' Comparo por = 0 ya que esto es cuando se cierra
						' "gracefully". (mas abajo)
						If Ret < 0 Then
							UltError = Err.LastDllError
							If UltError = WSAEMSGSIZE Then
								Debug.Print("WSAEMSGSIZE")
								Ret = SIZE_RCVBUF
							Else
								Debug.Print("Error en Recv: " & GetWSAErrorString(UltError))
								Call LogApiSock("Error en Recv: N=" & N & " S=" & S & " Str=" & GetWSAErrorString(UltError))
								
								'no hay q llamar a CloseSocket() directamente,
								'ya q pueden abusar de algun error para
								'desconectarse sin los 10segs. CREEME.
								Call CloseSocketSL(N)
								Call Cerrar_Usuario(N)
								Exit Function
							End If
						ElseIf Ret = 0 Then 
							Call CloseSocketSL(N)
							Call Cerrar_Usuario(N)
						End If
						
						ReDim Preserve Tmp(Ret - 1)
						
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
#End If
	End Function
	
	'Retorna 0 cuando se envió o se metio en la cola,
	'retorna <> 0 cuando no se pudo enviar o no se pudo meter en la cola
	'UPGRADE_NOTE: str se actualizó a str_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public Function WsApiEnviar(ByVal Slot As Short, ByRef str_Renamed As String) As Integer
#If UsarQueSocket = 1 Then
		Dim Ret As String
		Dim Retorno As Integer
		Dim data() As Byte
		
		ReDim Preserve data(Len(str_Renamed) - 1)
		
		'UPGRADE_ISSUE: No se actualizó la constante vbFromUnicode. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"'
		'UPGRADE_TODO: El código se actualizó para usar System.Text.UnicodeEncoding.Unicode.GetBytes(), que podría no tener el mismo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="93DD716C-10E3-41BE-A4A8-3BA40157905B"'
		data = System.Text.UnicodeEncoding.Unicode.GetBytes(StrConv(str_Renamed, vbFromUnicode))
		
#If SeguridadAlkon Then
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión SeguridadAlkon no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		Call Security.DataSent(Slot, data)
#End If
		
		Retorno = 0
		
		If UserList(Slot).ConnID <> -1 And UserList(Slot).ConnIDValida Then
			Ret = CStr(send(UserList(Slot).ConnID, data(0), UBound(data) + 1, 0))
			If CDbl(Ret) < 0 Then
				Ret = CStr(Err.LastDllError)
				If CDbl(Ret) = WSAEWOULDBLOCK Then
					
#If SeguridadAlkon Then
					'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión SeguridadAlkon no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
					Call Security.DataStored(Slot)
#End If
					
					' WSAEWOULDBLOCK, put the data again in the outgoingData Buffer
					Call UserList(Slot).outgoingData.WriteASCIIStringFixed(str_Renamed)
				End If
			End If
		ElseIf UserList(Slot).ConnID <> -1 And Not UserList(Slot).ConnIDValida Then 
			If Not UserList(Slot).Counters.Saliendo Then
				Retorno = -1
			End If
		End If
		
		WsApiEnviar = Retorno
#End If
	End Function
	
	'UPGRADE_NOTE: str se actualizó a str_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public Sub LogApiSock(ByVal str_Renamed As String)
#If (UsarQueSocket = 1) Then
		
		On Error GoTo Errhandler
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\wsapi.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, Today & " " & TimeOfDay & " " & str_Renamed)
		FileClose(nfile)
		
		Exit Sub
		
Errhandler: 
		
#End If
	End Sub
	
	Public Sub EventoSockAccept(ByVal SockID As Integer)
#If UsarQueSocket = 1 Then
		'==========================================================
		'USO DE LA API DE WINSOCK
		'========================
		
		Dim NewIndex As Short
		Dim Ret As Integer
		Dim Tam As Integer
		Dim sa As sockaddr
		Dim NuevoSock As Integer
		Dim i As Integer
		Dim tStr As String
		
		Tam = sockaddr_size
		
		'=============================================
		'SockID es en este caso es el socket de escucha,
		'a diferencia de socketwrench que es el nuevo
		'socket de la nueva conn
		
		'Modificado por Maraxus
		'Ret = WSAAccept(SockID, sa, Tam, AddressOf CondicionSocket, 0)
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
		
		'If Ret = INVALID_SOCKET Then
		'    If Err.LastDllError = 11002 Then
		'        ' We couldn't decide if to accept or reject the connection
		'        'Force reject so we can get it out of the queue
		'        Ret = WSAAccept(SockID, sa, Tam, AddressOf CondicionSocket, 1)
		'        Call LogCriticEvent("Error en WSAAccept() API 11002: No se pudo decidir si aceptar o rechazar la conexión.")
		'    Else
		'        i = Err.LastDllError
		'        Call LogCriticEvent("Error en WSAAccept() API " & i & ": " & GetWSAErrorString(i))
		'        Exit Sub
		'    End If
		'End If
		
		NuevoSock = Ret
		
		'Seteamos el tamaño del buffer de entrada
		If setsockopt(NuevoSock, SOL_SOCKET, SO_RCVBUFFER, SIZE_RCVBUF, 4) <> 0 Then
			i = Err.LastDllError
			Call LogCriticEvent("Error al setear el tamaño del buffer de entrada " & i & ": " & GetWSAErrorString(i))
		End If
		'Seteamos el tamaño del buffer de salida
		If setsockopt(NuevoSock, SOL_SOCKET, SO_SNDBUFFER, SIZE_SNDBUF, 4) <> 0 Then
			i = Err.LastDllError
			Call LogCriticEvent("Error al setear el tamaño del buffer de salida " & i & ": " & GetWSAErrorString(i))
		End If
		
		'If SecurityIp.IPSecuritySuperaLimiteConexiones(sa.sin_addr) Then
		'tStr = "Limite de conexiones para su IP alcanzado."
		'Call send(ByVal NuevoSock, ByVal tStr, ByVal Len(tStr), ByVal 0)
		'Call WSApiCloseSocket(NuevoSock)
		'Exit Sub
		'End If
		
		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		'   BIENVENIDO AL SERVIDOR!!!!!!!!
		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
		
		'Mariano: Baje la busqueda de slot abajo de CondicionSocket y limite x ip
		NewIndex = NextOpenUser ' Nuevo indice
		
		Dim data() As Byte
		'UPGRADE_NOTE: str se actualizó a str_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim str_Renamed As String
		If NewIndex <= MaxUsers Then
			
			'Make sure both outgoing and incoming data buffers are clean
			Call UserList(NewIndex).incomingData.ReadASCIIStringFixed(UserList(NewIndex).incomingData.length)
			Call UserList(NewIndex).outgoingData.ReadASCIIStringFixed(UserList(NewIndex).outgoingData.length)
			
#If SeguridadAlkon Then
			'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión SeguridadAlkon no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
			Call Security.NewConnection(NewIndex)
#End If
			
			UserList(NewIndex).ip = GetAscIP(sa.sin_addr)
			'Busca si esta banneada la ip
			For i = 1 To BanIps.Count()
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto BanIps.Item(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				If BanIps.Item(i) = UserList(NewIndex).ip Then
					'Call apiclosesocket(NuevoSock)
					Call WriteErrorMsg(NewIndex, "Su IP se encuentra bloqueada en este servidor.")
					Call FlushBuffer(NewIndex)
					'Call SecurityIp.IpRestarConexion(sa.sin_addr)
					Call WSApiCloseSocket(NuevoSock)
					Exit Sub
				End If
			Next i
			
			If NewIndex > LastUser Then LastUser = NewIndex
			
			UserList(NewIndex).ConnID = NuevoSock
			UserList(NewIndex).ConnIDValida = True
			
			Call AgregaSlotSock(NuevoSock, NewIndex)
		Else
			
			str_Renamed = Protocol.PrepareMessageErrorMsg("El servidor se encuentra lleno en este momento. Disculpe las molestias ocasionadas.")
			
			ReDim Preserve data(Len(str_Renamed) - 1)
			
			'UPGRADE_ISSUE: No se actualizó la constante vbFromUnicode. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"'
			'UPGRADE_TODO: El código se actualizó para usar System.Text.UnicodeEncoding.Unicode.GetBytes(), que podría no tener el mismo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="93DD716C-10E3-41BE-A4A8-3BA40157905B"'
			data = System.Text.UnicodeEncoding.Unicode.GetBytes(StrConv(str_Renamed, vbFromUnicode))
			
#If SeguridadAlkon Then
			'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión SeguridadAlkon no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
			Call Security.DataSent(Security.NO_SLOT, data)
#End If
			
			Call send(NuevoSock, data(0), UBound(data) + 1, 0)
			Call WSApiCloseSocket(NuevoSock)
		End If
		
#End If
	End Sub
	
	Public Sub EventoSockRead(ByVal Slot As Short, ByRef Datos() As Byte)
#If UsarQueSocket = 1 Then
		
		With UserList(Slot)
			
#If SeguridadAlkon Then
			'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión SeguridadAlkon no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
			Call Security.DataReceived(Slot, Datos)
#End If
			
			Call .incomingData.WriteBlock(Datos)
			
			If .ConnID <> -1 Then
				Call HandleIncomingData(Slot)
			Else
				Exit Sub
			End If
		End With
		
#End If
	End Sub
	
	Public Sub EventoSockClose(ByVal Slot As Short)
#If UsarQueSocket = 1 Then
		
		'Es el mismo user al que está revisando el centinela??
		'Si estamos acá es porque se cerró la conexión, no es un /salir, y no queremos banearlo....
		If Centinela.RevisandoUserIndex = Slot Then Call modCentinela.CentinelaUserLogout()
		
#If SeguridadAlkon Then
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión SeguridadAlkon no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		Call Security.UserDisconnected(Slot)
#End If
		
		If UserList(Slot).flags.UserLogged Then
			Call CloseSocketSL(Slot)
			Call Cerrar_Usuario(Slot)
		Else
			Call CloseSocket(Slot)
		End If
#End If
	End Sub
	
	
	Public Sub WSApiReiniciarSockets()
#If UsarQueSocket = 1 Then
		Dim i As Integer
		'Cierra el socket de escucha
		If SockListen >= 0 Then Call apiclosesocket(SockListen)
		
		'Cierra todas las conexiones
		For i = 1 To MaxUsers
			If UserList(i).ConnID <> -1 And UserList(i).ConnIDValida Then
				Call CloseSocket(i)
			End If
			
			'Call ResetUserSlot(i)
		Next i
		
		For i = 1 To MaxUsers
			'UPGRADE_NOTE: El objeto UserList().incomingData no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
			UserList(i).incomingData = Nothing
			'UPGRADE_NOTE: El objeto UserList().outgoingData no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
			UserList(i).outgoingData = Nothing
		Next i
		
		' No 'ta el PRESERVE :p
		'UPGRADE_WARNING: El límite inferior de la matriz UserList ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		ReDim UserList(MaxUsers)
		For i = 1 To MaxUsers
			UserList(i).ConnID = -1
			UserList(i).ConnIDValida = False
			
			UserList(i).incomingData = New clsByteQueue
			UserList(i).outgoingData = New clsByteQueue
		Next i
		
		LastUser = 1
		NumUsers = 0
		
		Call LimpiaWsApi()
		Call Sleep(100)
		Call IniciaWsApi(frmMain.Handle.ToInt32)
		SockListen = ListenForConnect(Puerto, hWndMsg, "")
		
		
#End If
	End Sub
	
	Public Sub WSApiCloseSocket(ByVal Socket As Integer)
#If UsarQueSocket = 1 Then
		Call WSAAsyncSelect(Socket, hWndMsg, 1025, (FD_CLOSE))
		Call ShutDown(Socket, SD_BOTH)
#End If
	End Sub
	
	Public Function CondicionSocket(ByRef lpCallerId As WSABUF, ByRef lpCallerData As WSABUF, ByRef lpSQOS As FLOWSPEC, ByVal Reserved As Integer, ByRef lpCalleeId As WSABUF, ByRef lpCalleeData As WSABUF, ByRef Group As Integer, ByVal dwCallbackData As Integer) As Integer
#If UsarQueSocket = 1 Then
		Dim sa As sockaddr
		
		'Check if we were requested to force reject
		
		If dwCallbackData = 1 Then
			CondicionSocket = CF_REJECT
			Exit Function
		End If
		
		'Get the address
		
		'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto sa. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		CopyMemory(sa, lpCallerId.lpBuffer, lpCallerId.dwBufferLen)
		
		
		If Not SecurityIp.IpSecurityAceptarNuevaConexion(sa.sin_addr) Then
			CondicionSocket = CF_REJECT
			Exit Function
		End If
		
		CondicionSocket = CF_ACCEPT 'En realdiad es al pedo, porque CondicionSocket se inicializa a 0, pero así es más claro....
#End If
	End Function
End Module