Option Strict Off
Option Explicit On
Friend Class frmMain
	Inherits System.Windows.Forms.Form
	'Argentum Online 0.12.2
	'Copyright (C) 2002 Márquez Pablo Ignacio
	'
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
	'
	'Argentum Online is based on Baronsoft's VB6 Online RPG
	'You can contact the original creator of ORE at aaron@baronsoft.com
	'for more information about ORE please visit http://www.baronsoft.com/
	'
	'
	'You can contact me at:
	'morgolock@speedy.com.ar
	'www.geocities.com/gmorgolock
	'Calle 3 número 983 piso 7 dto A
	'La Plata - Pcia, Buenos Aires - Republica Argentina
	'Código Postal 1900
	'Pablo Ignacio Márquez
	
	
	Public ESCUCHADAS As Integer
	
	Private Structure NOTIFYICONDATA
		Dim cbSize As Integer
		Dim hWnd As Integer
		Dim uID As Integer
		Dim uFlags As Integer
		Dim uCallbackMessage As Integer
		Dim hIcon As Integer
		'UPGRADE_WARNING: El tamaño de la cadena de longitud fija debe caber en el búfer. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"'
		<VBFixedString(64),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=64)> Public szTip() As Char
	End Structure
	
	Const NIM_ADD As Short = 0
	Const NIM_DELETE As Short = 2
	Const NIF_MESSAGE As Short = 1
	Const NIF_ICON As Short = 2
	Const NIF_TIP As Short = 4
	
	Const WM_MOUSEMOVE As Integer = &H200
	Const WM_LBUTTONDBLCLK As Integer = &H203
	Const WM_RBUTTONUP As Integer = &H205
	
	Private Declare Function GetWindowThreadProcessId Lib "user32" (ByVal hWnd As Integer, ByRef lpdwProcessId As Integer) As Integer
	'UPGRADE_WARNING: La estructura NOTIFYICONDATA puede requerir que se pasen atributos de cálculo de referencia como argumento en esta instrucción Declare. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	Private Declare Function Shell_NotifyIconA Lib "SHELL32" (ByVal dwMessage As Integer, ByRef lpData As NOTIFYICONDATA) As Short
	
	'UPGRADE_NOTE: Icon se actualizó a Icon_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Private Function setNOTIFYICONDATA(ByRef hWnd As Integer, ByRef ID As Integer, ByRef flags As Integer, ByRef CallbackMessage As Integer, ByRef Icon_Renamed As Integer, ByRef Tip As String) As NOTIFYICONDATA
		Dim nidTemp As NOTIFYICONDATA
		
		nidTemp.cbSize = Len(nidTemp)
		nidTemp.hWnd = hWnd
		nidTemp.uID = ID
		nidTemp.uFlags = flags
		nidTemp.uCallbackMessage = CallbackMessage
		nidTemp.hIcon = Icon_Renamed
		nidTemp.szTip = Tip & Chr(0)
		
		'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto setNOTIFYICONDATA. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		setNOTIFYICONDATA = nidTemp
	End Function
	
	Sub CheckIdleUser()
		Dim iUserIndex As Integer
		
		For iUserIndex = 1 To MaxUsers
			With UserList(iUserIndex)
				'Conexion activa? y es un usuario loggeado?
				If .ConnID <> -1 And .flags.UserLogged Then
					'Actualiza el contador de inactividad
					If .flags.Traveling = 0 Then
						.Counters.IdleCount = .Counters.IdleCount + 1
					End If
					
					If .Counters.IdleCount >= IdleLimit Then
						Call WriteShowMessageBox(iUserIndex, "Demasiado tiempo inactivo. Has sido desconectado.")
						'mato los comercios seguros
						If .ComUsu.DestUsu > 0 Then
							If UserList(.ComUsu.DestUsu).flags.UserLogged Then
								If UserList(.ComUsu.DestUsu).ComUsu.DestUsu = iUserIndex Then
									Call WriteConsoleMsg(.ComUsu.DestUsu, "Comercio cancelado por el otro usuario.", Protocol.FontTypeNames.FONTTYPE_TALK)
									Call FinComerciarUsu(.ComUsu.DestUsu)
									Call FlushBuffer(.ComUsu.DestUsu) 'flush the buffer to send the message right away
								End If
							End If
							Call FinComerciarUsu(iUserIndex)
						End If
						Call Cerrar_Usuario(iUserIndex)
					End If
				End If
			End With
		Next iUserIndex
	End Sub
	
	Private Sub Auditoria_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Auditoria.Tick
		On Error GoTo errhand
		Static centinelSecs As Byte
		
		centinelSecs = centinelSecs + 1
		
		If centinelSecs = 5 Then
			'Every 5 seconds, we try to call the player's attention so it will report the code.
			Call modCentinela.CallUserAttention()
			
			centinelSecs = 0
		End If
		
		Call PasarSegundo() 'sistema de desconexion de 10 segs
		
		Call ActualizaEstadisticasWeb()
		
		Exit Sub
		
errhand: 
		
		Call LogError("Error en Timer Auditoria. Err: " & Err.Description & " - " & Err.Number)
		Resume Next
		
	End Sub
	
	Private Sub AutoSave_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles AutoSave.Tick
		
		On Error GoTo Errhandler
		'fired every minute
		Static Minutos As Integer
		Static MinutosLatsClean As Integer
		Static MinsPjesSave As Integer
		
		Dim i As Short
		Dim num As Integer
		
		Minutos = Minutos + 1
		
		'¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
		Call ModAreas.AreasOptimizacion()
		'¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
		
		'Actualizamos el centinela
		Call modCentinela.PasarMinutoCentinela()
		
		If Minutos = MinutosWs - 1 Then
			Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg("Worldsave en 1 minuto ...", Protocol.FontTypeNames.FONTTYPE_VENENO))
		End If
		
		If Minutos >= MinutosWs Then
			Call ES.DoBackUp()
			Call aClon.VaciarColeccion()
			Minutos = 0
		End If
		
		If MinutosLatsClean >= 15 Then
			MinutosLatsClean = 0
			Call ReSpawnOrigPosNpcs() 'respawn de los guardias en las pos originales
			Call LimpiarMundo()
		Else
			MinutosLatsClean = MinutosLatsClean + 1
		End If
		
		Call PurgarPenas()
		Call CheckIdleUser()
		
		'<<<<<-------- Log the number of users online ------>>>
		Dim N As Short
		N = FreeFile
		FileOpen(N, My.Application.Info.DirectoryPath & "\logs\numusers.log", OpenMode.Output, , OpenShare.Shared)
		PrintLine(N, NumUsers)
		FileClose(N)
		'<<<<<-------- Log the number of users online ------>>>
		
		Exit Sub
Errhandler: 
		Call LogError("Error en TimerAutoSave " & Err.Number & ": " & Err.Description)
		Resume Next
	End Sub
	
	Private Sub CMDDUMP_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles CMDDUMP.Click
		On Error Resume Next
		
		Dim i As Short
		For i = 1 To MaxUsers
			Call LogCriticEvent(i & ") ConnID: " & UserList(i).ConnID & ". ConnidValida: " & UserList(i).ConnIDValida & " Name: " & UserList(i).name & " UserLogged: " & UserList(i).flags.UserLogged)
		Next i
		
		Call LogCriticEvent("Lastuser: " & LastUser & " NextOpenUser: " & NextOpenUser)
		
	End Sub
	
	Private Sub Command1_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command1.Click
		Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageShowMessageBox(BroadMsg.Text))
		''''''''''''''''SOLO PARA EL TESTEO'''''''
		''''''''''SE USA PARA COMUNICARSE CON EL SERVER'''''''''''
		txtChat.Text = txtChat.Text & vbNewLine & "Servidor> " & BroadMsg.Text
	End Sub
	
	Public Sub InitMain(ByVal f As Byte)
		
		If f = 1 Then
			Call mnuSystray_Click(mnuSystray, New System.EventArgs())
		Else
			Me.Show()
		End If
		
	End Sub
	
	Private Sub Command2_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command2.Click
		Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg("Servidor> " & BroadMsg.Text, Protocol.FontTypeNames.FONTTYPE_SERVER))
		''''''''''''''''SOLO PARA EL TESTEO'''''''
		''''''''''SE USA PARA COMUNICARSE CON EL SERVER'''''''''''
		txtChat.Text = txtChat.Text & vbNewLine & "Servidor> " & BroadMsg.Text
	End Sub
	
	Private Sub frmMain_MouseMove(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		On Error Resume Next
		
		Dim hProcess As Integer
		If Not Visible Then
			Select Case X \ VB6.TwipsPerPixelX
				
				Case WM_LBUTTONDBLCLK
					WindowState = System.Windows.Forms.FormWindowState.Normal
					Visible = True
					GetWindowThreadProcessId(Handle.ToInt32, hProcess)
					AppActivate(hProcess)
				Case WM_RBUTTONUP
					'UPGRADE_WARNING: Agregue un delegado para el operador AddressOf AppHook Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="E9E157F7-EF0C-4016-87B7-7D7FBBC6EE08"'
					hHook = SetWindowsHookEx(WH_CALLWNDPROC, AddressOf AppHook, VB6.GetHInstance.ToInt32, System.Threading.Thread.CurrentThread.ManagedThreadID)
					'UPGRADE_ISSUE: Form método frmMain.PopupMenu no se actualizó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"'
					PopupMenu(mnuPopUp)
					If hHook Then UnhookWindowsHookEx(hHook) : hHook = 0
			End Select
		End If
		
	End Sub
	
	Private Sub QuitarIconoSystray()
		On Error Resume Next
		
		'Borramos el icono del systray
		Dim i As Short
		Dim nid As NOTIFYICONDATA
		
		'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nid. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		nid = setNOTIFYICONDATA(Me.Handle.ToInt32, VariantType.Null, NIF_MESSAGE Or NIF_ICON Or NIF_TIP, VariantType.Null, CInt(CObj(Me.Icon)), "")
		
		i = Shell_NotifyIconA(NIM_DELETE, nid)
		
		
	End Sub
	
	Private Sub frmMain_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
		On Error Resume Next
		
		'Save stats!!!
		Call Statistics.DumpStatistics()
		
		Call QuitarIconoSystray()
		
#If UsarQueSocket = 1 Then
		Call LimpiaWsApi()
#ElseIf UsarQueSocket = 0 Then
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión UsarQueSocket = 0 no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		Socket1.Cleanup
#ElseIf UsarQueSocket = 2 Then
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión UsarQueSocket = 2 no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		Serv.Detener
#End If
		
		Dim LoopC As Short
		
		For LoopC = 1 To MaxUsers
			If UserList(LoopC).ConnID <> -1 Then Call CloseSocket(LoopC)
		Next 
		
		'Log
		Dim N As Short
		N = FreeFile
		FileOpen(N, My.Application.Info.DirectoryPath & "\logs\Main.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(N, Today & " " & TimeOfDay & " server cerrado.")
		FileClose(N)
		
		End
		
		'UPGRADE_NOTE: El objeto SonidosMapas no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		SonidosMapas = Nothing
		
	End Sub
	
	Private Sub FX_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles FX.Tick
		On Error GoTo hayerror
		
		Call SonidosMapas.ReproducirSonidosDeMapas()
		
		Exit Sub
hayerror: 
		
	End Sub
	
	Private Sub GameTimer_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles GameTimer.Tick
		'********************************************************
		'Author: Unknown
		'Last Modify Date: -
		'********************************************************
		Dim iUserIndex As Integer
		Dim bEnviarStats As Boolean
		Dim bEnviarAyS As Boolean
		
		On Error GoTo hayerror
		
		'<<<<<< Procesa eventos de los usuarios >>>>>>
		For iUserIndex = 1 To MaxUsers 'LastUser
			With UserList(iUserIndex)
				'Conexion activa?
				If .ConnID <> -1 Then
					'¿User valido?
					
					If .ConnIDValida And .flags.UserLogged Then
						
						'[Alejo-18-5]
						bEnviarStats = False
						bEnviarAyS = False
						
						Call DoTileEvents(iUserIndex, .Pos.Map, .Pos.X, .Pos.Y)
						
						
						If .flags.Paralizado = 1 Then Call EfectoParalisisUser(iUserIndex)
						If .flags.Ceguera = 1 Or .flags.Estupidez Then Call EfectoCegueEstu(iUserIndex)
						
						
						If .flags.Muerto = 0 Then
							
							'[Consejeros]
							If (.flags.Privilegios And Declaraciones.PlayerType.User) Then Call EfectoLava(iUserIndex)
							
							If .flags.Desnudo <> 0 And (.flags.Privilegios And Declaraciones.PlayerType.User) <> 0 Then Call EfectoFrio(iUserIndex)
							
							If .flags.Meditando Then Call DoMeditar(iUserIndex)
							
							If .flags.Envenenado <> 0 And (.flags.Privilegios And Declaraciones.PlayerType.User) <> 0 Then Call EfectoVeneno(iUserIndex)
							
							If .flags.AdminInvisible <> 1 Then
								If .flags.invisible = 1 Then Call EfectoInvisibilidad(iUserIndex)
								If .flags.Oculto = 1 Then Call DoPermanecerOculto(iUserIndex)
							End If
							
							If .flags.Mimetizado = 1 Then Call EfectoMimetismo(iUserIndex)
							
							If .flags.AtacablePor > 0 Then Call EfectoEstadoAtacable(iUserIndex)
							
							Call DuracionPociones(iUserIndex)
							
							Call HambreYSed(iUserIndex, bEnviarAyS)
							
							If .flags.Hambre = 0 And .flags.Sed = 0 Then
								If Lloviendo Then
									If Not Intemperie(iUserIndex) Then
										If Not .flags.Descansar Then
											'No esta descansando
											Call Sanar(iUserIndex, bEnviarStats, SanaIntervaloSinDescansar)
											If bEnviarStats Then
												Call WriteUpdateHP(iUserIndex)
												bEnviarStats = False
											End If
											Call RecStamina(iUserIndex, bEnviarStats, StaminaIntervaloSinDescansar)
											If bEnviarStats Then
												Call WriteUpdateSta(iUserIndex)
												bEnviarStats = False
											End If
										Else
											'esta descansando
											Call Sanar(iUserIndex, bEnviarStats, SanaIntervaloDescansar)
											If bEnviarStats Then
												Call WriteUpdateHP(iUserIndex)
												bEnviarStats = False
											End If
											Call RecStamina(iUserIndex, bEnviarStats, StaminaIntervaloDescansar)
											If bEnviarStats Then
												Call WriteUpdateSta(iUserIndex)
												bEnviarStats = False
											End If
											'termina de descansar automaticamente
											If .Stats.MaxHp = .Stats.MinHp And .Stats.MaxSta = .Stats.MinSta Then
												Call WriteRestOK(iUserIndex)
												Call WriteConsoleMsg(iUserIndex, "Has terminado de descansar.", Protocol.FontTypeNames.FONTTYPE_INFO)
												.flags.Descansar = False
											End If
											
										End If
									End If
								Else
									If Not .flags.Descansar Then
										'No esta descansando
										
										Call Sanar(iUserIndex, bEnviarStats, SanaIntervaloSinDescansar)
										If bEnviarStats Then
											Call WriteUpdateHP(iUserIndex)
											bEnviarStats = False
										End If
										Call RecStamina(iUserIndex, bEnviarStats, StaminaIntervaloSinDescansar)
										If bEnviarStats Then
											Call WriteUpdateSta(iUserIndex)
											bEnviarStats = False
										End If
										
									Else
										'esta descansando
										
										Call Sanar(iUserIndex, bEnviarStats, SanaIntervaloDescansar)
										If bEnviarStats Then
											Call WriteUpdateHP(iUserIndex)
											bEnviarStats = False
										End If
										Call RecStamina(iUserIndex, bEnviarStats, StaminaIntervaloDescansar)
										If bEnviarStats Then
											Call WriteUpdateSta(iUserIndex)
											bEnviarStats = False
										End If
										'termina de descansar automaticamente
										If .Stats.MaxHp = .Stats.MinHp And .Stats.MaxSta = .Stats.MinSta Then
											Call WriteRestOK(iUserIndex)
											Call WriteConsoleMsg(iUserIndex, "Has terminado de descansar.", Protocol.FontTypeNames.FONTTYPE_INFO)
											.flags.Descansar = False
										End If
										
									End If
								End If
							End If
							
							If bEnviarAyS Then Call WriteUpdateHungerAndThirst(iUserIndex)
							
							If .NroMascotas > 0 Then Call TiempoInvocacion(iUserIndex)
						End If 'Muerto
					Else 'no esta logeado?
						'Inactive players will be removed!
						.Counters.IdleCount = .Counters.IdleCount + 1
						If .Counters.IdleCount > IntervaloParaConexion Then
							.Counters.IdleCount = 0
							Call CloseSocket(iUserIndex)
						End If
					End If 'UserLogged
					
					'If there is anything to be sent, we send it
					Call FlushBuffer(iUserIndex)
				End If
			End With
		Next iUserIndex
		Exit Sub
		
hayerror: 
		LogError(("Error en GameTimer: " & Err.Description & " UserIndex = " & iUserIndex))
	End Sub
	
	Public Sub mnuCerrar_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles mnuCerrar.Click
		
		
		Dim f As Object
		If MsgBox("¡¡Atencion!! Si cierra el servidor puede provocar la perdida de datos. ¿Desea hacerlo de todas maneras?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
			For	Each f In My.Application.OpenForms
				'UPGRADE_ISSUE: f de descarga no se actualizó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="875EBAD7-D704-4539-9969-BC7DBDAA62A2"'
				Unload(f)
			Next f
		End If
		
	End Sub
	
	Public Sub mnusalir_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles mnusalir.Click
		Call mnuCerrar_Click(mnuCerrar, New System.EventArgs())
	End Sub
	
	Public Sub mnuMostrar_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles mnuMostrar.Click
		On Error Resume Next
		WindowState = System.Windows.Forms.FormWindowState.Normal
		frmMain_MouseMove(Me, New System.Windows.Forms.MouseEventArgs(0 * &H100000, 0, VB6.TwipsToPixelsX(7725), 0, 0))
	End Sub
	
	Private Sub KillLog_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles KillLog.Tick
		On Error Resume Next
		'UPGRADE_ISSUE: No se puede determinar a qué constante se va a actualizar vbNormal. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"'
		If FileExist(My.Application.Info.DirectoryPath & "\logs\connect.log", vbNormal) Then Kill(My.Application.Info.DirectoryPath & "\logs\connect.log")
		'UPGRADE_ISSUE: No se puede determinar a qué constante se va a actualizar vbNormal. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"'
		If FileExist(My.Application.Info.DirectoryPath & "\logs\haciendo.log", vbNormal) Then Kill(My.Application.Info.DirectoryPath & "\logs\haciendo.log")
		'UPGRADE_ISSUE: No se puede determinar a qué constante se va a actualizar vbNormal. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"'
		If FileExist(My.Application.Info.DirectoryPath & "\logs\stats.log", vbNormal) Then Kill(My.Application.Info.DirectoryPath & "\logs\stats.log")
		'UPGRADE_ISSUE: No se puede determinar a qué constante se va a actualizar vbNormal. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"'
		If FileExist(My.Application.Info.DirectoryPath & "\logs\Asesinatos.log", vbNormal) Then Kill(My.Application.Info.DirectoryPath & "\logs\Asesinatos.log")
		'UPGRADE_ISSUE: No se puede determinar a qué constante se va a actualizar vbNormal. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"'
		If FileExist(My.Application.Info.DirectoryPath & "\logs\HackAttemps.log", vbNormal) Then Kill(My.Application.Info.DirectoryPath & "\logs\HackAttemps.log")
		If Not FileExist(My.Application.Info.DirectoryPath & "\logs\nokillwsapi.txt") Then
			'UPGRADE_ISSUE: No se puede determinar a qué constante se va a actualizar vbNormal. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"'
			If FileExist(My.Application.Info.DirectoryPath & "\logs\wsapi.log", vbNormal) Then Kill(My.Application.Info.DirectoryPath & "\logs\wsapi.log")
		End If
		
	End Sub
	
	Public Sub mnuServidor_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles mnuServidor.Click
		frmServidor.Visible = True
	End Sub
	
	Public Sub mnuSystray_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles mnuSystray.Click
		
		Dim i As Short
		Dim S As String
		Dim nid As NOTIFYICONDATA
		
		S = "ARGENTUM-ONLINE"
		'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nid. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		nid = setNOTIFYICONDATA(Me.Handle.ToInt32, VariantType.Null, NIF_MESSAGE Or NIF_ICON Or NIF_TIP, WM_MOUSEMOVE, CInt(CObj(Me.Icon)), S)
		i = Shell_NotifyIconA(NIM_ADD, nid)
		
		If WindowState <> System.Windows.Forms.FormWindowState.Minimized Then WindowState = System.Windows.Forms.FormWindowState.Minimized
		Visible = False
		
	End Sub
	
	Private Sub npcataca_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles npcataca.Tick
		
		On Error Resume Next
		'UPGRADE_NOTE: npc se actualizó a npc_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim npc_Renamed As Integer
		
		For npc_Renamed = 1 To LastNPC
			Npclist(npc_Renamed).CanAttack = 1
		Next npc_Renamed
		
	End Sub
	
	Private Sub packetResend_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles packetResend.Tick
		'***************************************************
		'Autor: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 04/01/07
		'Attempts to resend to the user all data that may be enqueued.
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		
		For i = 1 To MaxUsers
			If UserList(i).ConnIDValida Then
				If UserList(i).outgoingData.length > 0 Then
					Call EnviarDatosASlot(i, UserList(i).outgoingData.ReadASCIIStringFixed(UserList(i).outgoingData.length))
				End If
			End If
		Next i
		
		Exit Sub
		
Errhandler: 
		LogError(("Error en packetResend - Error: " & Err.Number & " - Desc: " & Err.Description))
		Resume Next
	End Sub
	
	Private Sub securityTimer_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles securityTimer.Tick
		
#If SeguridadAlkon Then
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión SeguridadAlkon no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		Call Security.SecurityCheck
#End If
		
	End Sub
	
	Private Sub TIMER_AI_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles TIMER_AI.Tick
		
		On Error GoTo ErrorHandler
		Dim NpcIndex As Integer
		Dim X As Short
		Dim Y As Short
		Dim UseAI As Short
		Dim mapa As Short
		Dim e_p As Short
		
		'Barrin 29/9/03
		If Not haciendoBK And Not EnPausa Then
			'Update NPCs
			For NpcIndex = 1 To LastNPC
				
				With Npclist(NpcIndex)
					If .flags.NPCActive Then 'Nos aseguramos que sea INTELIGENTE!
						
						' Chequea si contiua teniendo dueño
						If .Owner > 0 Then Call ValidarPermanenciaNpc(NpcIndex)
						
						If .flags.Paralizado = 1 Then
							Call EfectoParalisisNpc(NpcIndex)
						Else
							e_p = esPretoriano(NpcIndex)
							If e_p > 0 Then
								Select Case e_p
									Case 1 ''clerigo
										Call PRCLER_AI(NpcIndex)
									Case 2 ''mago
										Call PRMAGO_AI(NpcIndex)
									Case 3 ''cazador
										Call PRCAZA_AI(NpcIndex)
									Case 4 ''rey
										Call PRREY_AI(NpcIndex)
									Case 5 ''guerre
										Call PRGUER_AI(NpcIndex)
								End Select
							Else
								'Usamos AI si hay algun user en el mapa
								If .flags.Inmovilizado = 1 Then
									Call EfectoParalisisNpc(NpcIndex)
								End If
								
								mapa = .Pos.Map
								
								If mapa > 0 Then
									If MapInfo_Renamed(mapa).NumUsers > 0 Then
										If .Movement <> AI.TipoAI.ESTATICO Then
											Call NPCAI(NpcIndex)
										End If
									End If
								End If
							End If
						End If
					End If
				End With
			Next NpcIndex
		End If
		
		Exit Sub
		
ErrorHandler: 
		Call LogError("Error en TIMER_AI_Timer " & Npclist(NpcIndex).name & " mapa:" & Npclist(NpcIndex).Pos.Map)
		Call MuereNpc(NpcIndex, 0)
	End Sub
	
	Private Sub tLluvia_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles tLluvia.Tick
		On Error GoTo Errhandler
		
		Dim iCount As Integer
		If Lloviendo Then
			For iCount = 1 To LastUser
				Call EfectoLluvia(iCount)
			Next iCount
		End If
		
		Exit Sub
Errhandler: 
		Call LogError("tLluvia " & Err.Number & ": " & Err.Description)
	End Sub
	
	Private Sub tLluviaEvent_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles tLluviaEvent.Tick
		
		On Error GoTo ErrorHandler
		Static MinutosLloviendo As Integer
		Static MinutosSinLluvia As Integer
		
		If Not Lloviendo Then
			MinutosSinLluvia = MinutosSinLluvia + 1
			If MinutosSinLluvia >= 15 And MinutosSinLluvia < 1440 Then
				If RandomNumber(1, 100) <= 2 Then
					Lloviendo = True
					MinutosSinLluvia = 0
					Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageRainToggle())
				End If
			ElseIf MinutosSinLluvia >= 1440 Then 
				Lloviendo = True
				MinutosSinLluvia = 0
				Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageRainToggle())
			End If
		Else
			MinutosLloviendo = MinutosLloviendo + 1
			If MinutosLloviendo >= 5 Then
				Lloviendo = False
				Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageRainToggle())
				MinutosLloviendo = 0
			Else
				If RandomNumber(1, 100) <= 2 Then
					Lloviendo = False
					MinutosLloviendo = 0
					Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageRainToggle())
				End If
			End If
		End If
		
		Exit Sub
ErrorHandler: 
		Call LogError("Error tLluviaTimer")
		
	End Sub
	
	Private Sub tPiqueteC_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles tPiqueteC.Tick
		Dim NuevaA As Boolean
		' Dim NuevoL As Boolean
		Dim GI As Short
		
		Dim i As Integer
		
		On Error GoTo Errhandler
		For i = 1 To LastUser
			With UserList(i)
				If .flags.UserLogged Then
					If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = Declaraciones.eTrigger.ANTIPIQUETE Then
						.Counters.PiqueteC = .Counters.PiqueteC + 1
						Call WriteConsoleMsg(i, "¡¡¡Estás obstruyendo la vía pública, muévete o serás encarcelado!!!", Protocol.FontTypeNames.FONTTYPE_INFO)
						
						If .Counters.PiqueteC > 23 Then
							.Counters.PiqueteC = 0
							Call Encarcelar(i, TIEMPO_CARCEL_PIQUETE)
						End If
					Else
						.Counters.PiqueteC = 0
					End If
					
					If .flags.Muerto = 1 Then
						If .flags.Traveling = 1 Then
							If .Counters.goHome <= 0 Then
								Call FindLegalPos(i, Ciudades(.Hogar).Map, Ciudades(.Hogar).X, Ciudades(.Hogar).Y)
								Call WarpUserChar(i, Ciudades(.Hogar).Map, Ciudades(.Hogar).X, Ciudades(.Hogar).Y, True)
								Call WriteMultiMessage(i, Declaraciones.eMessages.FinishHome)
								.flags.Traveling = 0
							Else
								.Counters.goHome = .Counters.goHome - 1
							End If
						End If
					End If
					
					'ustedes se preguntaran que hace esto aca?
					'bueno la respuesta es simple: el codigo de AO es una mierda y encontrar
					'todos los puntos en los cuales la alineacion puede cambiar es un dolor de
					'huevos, asi que lo controlo aca, cada 6 segundos, lo cual es razonable
					
					GI = .GuildIndex
					If GI > 0 Then
						NuevaA = False
						' NuevoL = False
						If Not modGuilds.m_ValidarPermanencia(i, True, NuevaA) Then
							Call WriteConsoleMsg(i, "Has sido expulsado del clan. ¡El clan ha sumado un punto de antifacción!", Protocol.FontTypeNames.FONTTYPE_GUILD)
						End If
						If NuevaA Then
							Call SendData(modSendData.SendTarget.ToGuildMembers, GI, PrepareMessageConsoleMsg("¡El clan ha pasado a tener alineación " & GuildAlignment(GI) & "!", Protocol.FontTypeNames.FONTTYPE_GUILD))
							Call LogClanes("¡El clan cambio de alineación!")
						End If
						'                    If NuevoL Then
						'                        Call SendData(SendTarget.ToGuildMembers, GI, PrepareMessageConsoleMsg("¡El clan tiene un nuevo líder!", FontTypeNames.FONTTYPE_GUILD))
						'                        Call LogClanes("¡El clan tiene nuevo lider!")
						'                    End If
					End If
					
					Call FlushBuffer(i)
				End If
			End With
		Next i
		Exit Sub
		
Errhandler: 
		Call LogError("Error en tPiqueteC_Timer " & Err.Number & ": " & Err.Description)
	End Sub
	
	
	
	
	
	'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
	'''''''''''''''''USO DEL CONTROL TCPSERV'''''''''''''''''''''''''''
	'''''''''''''Compilar con UsarQueSocket = 3''''''''''''''''''''''''
	'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
	
	
#If UsarQueSocket = 3 Then
	'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión UsarQueSocket = 3 no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
	
	Private Sub TCPServ_Eror(ByVal Numero As Long, ByVal Descripcion As String)
	Call LogError("TCPSERVER SOCKET ERROR: " & Numero & "/" & Descripcion)
	End Sub
	
	Private Sub TCPServ_NuevaConn(ByVal ID As Long)
	On Error GoTo errorHandlerNC
	
	ESCUCHADAS = ESCUCHADAS + 1
	Escuch.Caption = ESCUCHADAS
	
	Dim i As Integer
	
	Dim NewIndex As Integer
	NewIndex = NextOpenUser
	
	If NewIndex <= MaxUsers Then
	'call logindex(NewIndex, "******> Accept. ConnId: " & ID)
	
	TCPServ.SetDato ID, NewIndex
	
	If aDos.MaxConexiones(TCPServ.GetIP(ID)) Then
	Call aDos.RestarConexion(TCPServ.GetIP(ID))
	Call ResetUserSlot(NewIndex)
	Exit Sub
	End If
	
	If NewIndex > LastUser Then LastUser = NewIndex
	
	UserList(NewIndex).ConnID = ID
	UserList(NewIndex).ip = TCPServ.GetIP(ID)
	UserList(NewIndex).ConnIDValida = True
	Set UserList(NewIndex).CommandsBuffer = New CColaArray
	
	For i = 1 To BanIps.Count
	If BanIps.Item(i) = TCPServ.GetIP(ID) Then
	Call ResetUserSlot(NewIndex)
	Exit Sub
	End If
	Next i
	
	Else
	Call CloseSocket(NewIndex, True)
	LogCriticEvent ("NEWINDEX > MAXUSERS. IMPOSIBLE ALOCATEAR SOCKETS")
	End If
	
	Exit Sub
	
	errorHandlerNC:
	Call LogError("TCPServer::NuevaConexion " & Err.description)
	End Sub
	
	Private Sub TCPServ_Close(ByVal ID As Long, ByVal MiDato As Long)
	On Error GoTo eh
	'' No cierro yo el socket. El on_close lo cierra por mi.
	'call logindex(MiDato, "******> Remote Close. ConnId: " & ID & " Midato: " & MiDato)
	Call CloseSocket(MiDato, False)
	Exit Sub
	eh:
	Call LogError("Ocurrio un error en el evento TCPServ_Close. ID/miDato:" & ID & "/" & MiDato)
	End Sub
	
	Private Sub TCPServ_Read(ByVal ID As Long, Datos As Variant, ByVal Cantidad As Long, ByVal MiDato As Long)
	On Error GoTo errorh
	
	With UserList(MiDato)
	Datos = StrConv(StrConv(Datos, vbUnicode), vbFromUnicode)
	
	Call .incomingData.WriteASCIIStringFixed(Datos)
	
	If .ConnID <> -1 Then
	Call HandleIncomingData(MiDato)
	Else
	Exit Sub
	End If
	End With
	
	Exit Sub
	
	errorh:
	Call LogError("Error socket read: " & MiDato & " dato:" & RD & " userlogged: " & UserList(MiDato).flags.UserLogged & " connid:" & UserList(MiDato).ConnID & " ID Parametro" & ID & " error:" & Err.description)
	
	End Sub
	
#End If
	'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
	''''''''''''''FIN  USO DEL CONTROL TCPSERV'''''''''''''''''''''''''
	'''''''''''''Compilar con UsarQueSocket = 3''''''''''''''''''''''''
	'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
End Class