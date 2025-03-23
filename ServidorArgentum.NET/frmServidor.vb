Option Strict Off
Option Explicit On
Imports Microsoft.VisualBasic.PowerPacks
Friend Class frmServidor
	Inherits System.Windows.Forms.Form
	'Argentum Online 0.12.2
	'Copyright (C) 2002 M�rquez Pablo Ignacio
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
	'Calle 3 n�mero 983 piso 7 dto A
	'La Plata - Pcia, Buenos Aires - Republica Argentina
	'C�digo Postal 1900
	'Pablo Ignacio M�rquez
	
	
	Private Sub Command1_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command1.Click
		Call ResetForums()
		Call LoadOBJData()
		
	End Sub
	
	Private Sub Command10_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command10.Click
		frmTrafic.Show()
	End Sub
	
	Private Sub Command11_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command11.Click
		frmConID.Show()
	End Sub
	
	Private Sub Command12_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command12.Click
		frmDebugNpc.Show()
	End Sub
	
	Private Sub Command14_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command14.Click
		Call LoadMotd()
	End Sub
	
	Private Sub Command15_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command15.Click
		On Error Resume Next
		
		Dim Fn As String
		Dim cad As String
		Dim N, k As Short
		
		Dim sENtrada As String
		
		sENtrada = InputBox("Escribe ""estoy DE acuerdo"" entre comillas y con distinci�n de may�sculas min�sculas para desbanear a todos los personajes.", "UnBan", "hola")
		If sENtrada = "estoy DE acuerdo" Then
			
			Fn = My.Application.Info.DirectoryPath & "\logs\GenteBanned.log"
			
			'UPGRADE_ISSUE: No se puede determinar a qu� constante se va a actualizar vbNormal. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"'
			If FileExist(Fn, vbNormal) Then
				N = FreeFile
				FileOpen(N, Fn, OpenMode.Input, , OpenShare.Shared)
				Do While Not EOF(N)
					k = k + 1
					Input(N, cad)
					Call UnBan(cad)
					
				Loop 
				FileClose(N)
				MsgBox("Se han habilitado " & k & " personajes.")
				Kill(Fn)
			End If
		End If
		
	End Sub
	
	Private Sub Command16_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command16.Click
		Call LoadSini()
	End Sub
	
	Private Sub Command17_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command17.Click
		Call CargaNpcsDat()
	End Sub
	
	Private Sub Command18_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command18.Click
		Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
		Call mdParty.ActualizaExperiencias()
		Call GuardarUsuarios()
		Me.Cursor = System.Windows.Forms.Cursors.Default
		MsgBox("Grabado de personajes OK!")
	End Sub
	
	Private Sub Command19_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command19.Click
		Dim i, N As Integer
		
		Dim sENtrada As String
		
		sENtrada = InputBox("Escribe ""estoy DE acuerdo"" sin comillas y con distinci�n de may�sculas min�sculas para desbanear a todos los personajes", "UnBan", "hola")
		If sENtrada = "estoy DE acuerdo" Then
			
			N = BanIps.Count()
			For i = 1 To BanIps.Count()
				BanIps.Remove(1)
			Next i
			
			MsgBox("Se han habilitado " & N & " ipes")
		End If
		
	End Sub
	
	Private Sub Command2_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command2.Click
		Me.Visible = False
	End Sub
	
	Private Sub Command20_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command20.Click
#If UsarQueSocket = 1 Then
		
		If MsgBox("�Est� seguro que desea reiniciar los sockets? Se cerrar�n todas las conexiones activas.", MsgBoxStyle.YesNo, "Reiniciar Sockets") = MsgBoxResult.Yes Then
			Call WSApiReiniciarSockets()
		End If
		
#ElseIf UsarQueSocket = 2 Then
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualiz� porque la expresi�n UsarQueSocket = 2 no dio como resultado True o ni siquiera se evalu�. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		
		Dim LoopC As Integer
		
		If MsgBox("�Est� seguro que desea reiniciar los sockets? Se cerrar�n todas las conexiones activas.", vbYesNo, "Reiniciar Sockets") = vbYes Then
		For LoopC = 1 To MaxUsers
		If UserList(LoopC).ConnID <> -1 And UserList(LoopC).ConnIDValida Then
		Call CloseSocket(LoopC)
		End If
		Next LoopC
		
		Call frmMain.Serv.Detener
		Call frmMain.Serv.Iniciar(Puerto)
		End If
		
#End If
	End Sub
	
	'Barrin 29/9/03
	Private Sub Command21_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command21.Click
		
		If EnPausa = False Then
			EnPausa = True
			Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessagePauseToggle())
			Command21.Text = "Reanudar el servidor"
		Else
			EnPausa = False
			Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessagePauseToggle())
			Command21.Text = "Pausar el servidor"
		End If
		
	End Sub
	
	Private Sub Command22_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command22.Click
		Me.Visible = False
		frmAdmin.Show()
	End Sub
	
	Private Sub Command23_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command23.Click
		If MsgBox("�Est� seguro que desea hacer WorldSave, guardar pjs y cerrar?", MsgBoxStyle.YesNo, "Apagar Magicamente") = MsgBoxResult.Yes Then
			Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
			
			FrmStat.Show()
			
			'WorldSave
			Call ES.DoBackUp()
			
			'commit experiencia
			Call mdParty.ActualizaExperiencias()
			
			'Guardar Pjs
			Call GuardarUsuarios()
			
			'Chauuu
			frmMain.Close()
		End If
	End Sub
	
	Private Sub Command25_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command25.Click
		Call MD5sCarga()
		
	End Sub
	
	Private Sub Command26_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command26.Click
#If UsarQueSocket = 1 Then
		'Cierra el socket de escucha
		If SockListen >= 0 Then Call apiclosesocket(SockListen)
		
		'Inicia el socket de escucha
		SockListen = ListenForConnect(Puerto, hWndMsg, "")
#End If
	End Sub
	
	Private Sub Command27_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command27.Click
		frmUserList.Show()
		
	End Sub
	
	Private Sub Command28_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command28.Click
		Call LoadBalance()
	End Sub
	
	Private Sub Command3_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command3.Click
		If MsgBox("��Atencion!! Si reinicia el servidor puede provocar la p�rdida de datos de los usarios. �Desea reiniciar el servidor de todas maneras?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
			Me.Visible = False
			Call General.Restart()
		End If
	End Sub
	
	Private Sub Command4_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command4.Click
		On Error GoTo eh
		Me.Cursor = System.Windows.Forms.Cursors.WaitCursor
		FrmStat.Show()
		Call ES.DoBackUp()
		Me.Cursor = System.Windows.Forms.Cursors.Default
		MsgBox("WORLDSAVE OK!!")
		Exit Sub
eh: 
		Call LogError("Error en WORLDSAVE")
	End Sub
	
	Private Sub Command5_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command5.Click
		
		'Se asegura de que los sockets estan cerrados e ignora cualquier err
		On Error Resume Next
		
		If frmMain.Visible Then frmMain.txStatus.Text = "Reiniciando."
		
		FrmStat.Show()
		
		'UPGRADE_ISSUE: No se puede determinar a qu� constante se va a actualizar vbNormal. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"'
		If FileExist(My.Application.Info.DirectoryPath & "\logs\errores.log", vbNormal) Then Kill(My.Application.Info.DirectoryPath & "\logs\errores.log")
		'UPGRADE_ISSUE: No se puede determinar a qu� constante se va a actualizar vbNormal. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"'
		If FileExist(My.Application.Info.DirectoryPath & "\logs\connect.log", vbNormal) Then Kill(My.Application.Info.DirectoryPath & "\logs\Connect.log")
		'UPGRADE_ISSUE: No se puede determinar a qu� constante se va a actualizar vbNormal. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"'
		If FileExist(My.Application.Info.DirectoryPath & "\logs\HackAttemps.log", vbNormal) Then Kill(My.Application.Info.DirectoryPath & "\logs\HackAttemps.log")
		'UPGRADE_ISSUE: No se puede determinar a qu� constante se va a actualizar vbNormal. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"'
		If FileExist(My.Application.Info.DirectoryPath & "\logs\Asesinatos.log", vbNormal) Then Kill(My.Application.Info.DirectoryPath & "\logs\Asesinatos.log")
		'UPGRADE_ISSUE: No se puede determinar a qu� constante se va a actualizar vbNormal. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"'
		If FileExist(My.Application.Info.DirectoryPath & "\logs\Resurrecciones.log", vbNormal) Then Kill(My.Application.Info.DirectoryPath & "\logs\Resurrecciones.log")
		'UPGRADE_ISSUE: No se puede determinar a qu� constante se va a actualizar vbNormal. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"'
		If FileExist(My.Application.Info.DirectoryPath & "\logs\Teleports.Log", vbNormal) Then Kill(My.Application.Info.DirectoryPath & "\logs\Teleports.Log")
		
		
#If UsarQueSocket = 1 Then
		Call apiclosesocket(SockListen)
#ElseIf UsarQueSocket = 0 Then
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualiz� porque la expresi�n UsarQueSocket = 0 no dio como resultado True o ni siquiera se evalu�. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		frmMain.Socket1.Cleanup
		frmMain.Socket2(0).Cleanup
#ElseIf UsarQueSocket = 2 Then
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualiz� porque la expresi�n UsarQueSocket = 2 no dio como resultado True o ni siquiera se evalu�. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		frmMain.Serv.Detener
#End If
		
		Dim LoopC As Short
		
		For LoopC = 1 To MaxUsers
			Call CloseSocket(LoopC)
		Next 
		
		
		LastUser = 0
		NumUsers = 0
		
		Call FreeNPCs()
		Call FreeCharIndexes()
		
		Call LoadSini()
		Call CargarBackUp()
		Call LoadOBJData()
		
#If UsarQueSocket = 1 Then
		SockListen = ListenForConnect(Puerto, hWndMsg, "")
		
#ElseIf UsarQueSocket = 0 Then
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualiz� porque la expresi�n UsarQueSocket = 0 no dio como resultado True o ni siquiera se evalu�. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		frmMain.Socket1.AddressFamily = AF_INET
		frmMain.Socket1.Protocol = IPPROTO_IP
		frmMain.Socket1.SocketType = SOCK_STREAM
		frmMain.Socket1.Binary = False
		frmMain.Socket1.Blocking = False
		frmMain.Socket1.BufferSize = 1024
		
		frmMain.Socket2(0).AddressFamily = AF_INET
		frmMain.Socket2(0).Protocol = IPPROTO_IP
		frmMain.Socket2(0).SocketType = SOCK_STREAM
		frmMain.Socket2(0).Blocking = False
		frmMain.Socket2(0).BufferSize = 2048
		
		'Escucha
		frmMain.Socket1.LocalPort = Puerto
		frmMain.Socket1.listen
#End If
		
		If frmMain.Visible Then frmMain.txStatus.Text = "Escuchando conexiones entrantes ..."
		
	End Sub
	
	Private Sub Command6_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command6.Click
		Call ReSpawnOrigPosNpcs()
	End Sub
	
	Private Sub Command7_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command7.Click
		FrmInterv.Show()
	End Sub
	
	Private Sub Command8_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command8.Click
		Call CargarHechizos()
	End Sub
	
	Private Sub Command9_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command9.Click
		Call CargarForbidenWords()
	End Sub
	
	'UPGRADE_WARNING: Form evento frmServidor.Deactivate tiene un nuevo comportamiento. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"'
	Private Sub frmServidor_Deactivate(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Deactivate
		Me.Visible = False
	End Sub
	
	Private Sub frmServidor_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
#If UsarQueSocket = 1 Then
		Command20.Visible = True
		Command26.Visible = True
#ElseIf UsarQueSocket = 0 Then
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualiz� porque la expresi�n UsarQueSocket = 0 no dio como resultado True o ni siquiera se evalu�. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		Command20.Visible = False
		Command26.Visible = False
#ElseIf UsarQueSocket = 2 Then
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualiz� porque la expresi�n UsarQueSocket = 2 no dio como resultado True o ni siquiera se evalu�. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		Command20.Visible = True
		Command26.Visible = False
#End If
		
		VS1.Minimum = 0
		If picCont.Height > picFuera.ClientRectangle.Height Then
			VS1.Maximum = (picCont.Height - picFuera.ClientRectangle.Height + VS1.LargeChange - 1)
		Else
			VS1.Maximum = (0 + VS1.LargeChange - 1)
		End If
		picCont.Top = -VS1.Value
		
	End Sub
	
	'UPGRADE_NOTE: VS1.Change pas� de ser un evento a un procedimiento. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"'
	'UPGRADE_WARNING: VScrollBar evento VS1.Change tiene un nuevo comportamiento. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"'
	Private Sub VS1_Change(ByVal newScrollValue As Integer)
		picCont.Top = -newScrollValue
	End Sub
	
	'UPGRADE_NOTE: VS1.Scroll pas� de ser un evento a un procedimiento. Haga clic aqu� para obtener m�s informaci�n: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"'
	Private Sub VS1_Scroll_Renamed(ByVal newScrollValue As Integer)
		picCont.Top = -newScrollValue
	End Sub
	Private Sub VS1_Scroll(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.ScrollEventArgs) Handles VS1.Scroll
		Select Case eventArgs.type
			Case System.Windows.Forms.ScrollEventType.ThumbTrack
				VS1_Scroll_Renamed(eventArgs.newValue)
			Case System.Windows.Forms.ScrollEventType.EndScroll
				VS1_Change(eventArgs.newValue)
		End Select
	End Sub
End Class