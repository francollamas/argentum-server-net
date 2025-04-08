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
	
	Const NIM_ADD As Short = 0
	Const NIM_DELETE As Short = 2
	Const NIF_MESSAGE As Short = 1
	Const NIF_ICON As Short = 2
	Const NIF_TIP As Short = 4
	
	Const WM_MOUSEMOVE As Integer = &H200
	Const WM_LBUTTONDBLCLK As Integer = &H203
	Const WM_RBUTTONUP As Integer = &H205

	Private Sub Auditoria_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Auditoria.Tick


	End Sub
	
	Private Sub AutoSave_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles AutoSave.Tick


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
	
	Private Sub Command2_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command2.Click
		Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg("Servidor> " & BroadMsg.Text, Protocol.FontTypeNames.FONTTYPE_SERVER))
		''''''''''''''''SOLO PARA EL TESTEO'''''''
		''''''''''SE USA PARA COMUNICARSE CON EL SERVER'''''''''''
		txtChat.Text = txtChat.Text & vbNewLine & "Servidor> " & BroadMsg.Text
	End Sub
	
	Private Sub frmMain_FormClosing(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		Dim Cancel As Boolean = eventArgs.Cancel
		Dim UnloadMode As System.Windows.Forms.CloseReason = eventArgs.CloseReason
		If Not salir Then
			Cancel = True
		End If
		eventArgs.Cancel = Cancel
	End Sub
	
	Private Sub frmMain_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
		On Error Resume Next
		
		'Save stats!!!
		Call Statistics.DumpStatistics()
		
		Call LimpiaWsApi()
		
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


	Private Function salir() As Boolean
		Dim f As Object
		If MsgBox("¡¡Atencion!! Si cierra el servidor puede provocar la perdida de datos. ¿Desea hacerlo de todas maneras?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
			For Each f In Application.OpenForms
				'UPGRADE_ISSUE: f de descarga no se actualizó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="875EBAD7-D704-4539-9969-BC7DBDAA62A2"'
				f.Close()
			Next f
			salir = True
			Exit Function
		End If
		
		salir = False
	End Function
	
	Private Sub mnusalir_Click()
		Me.Close()
	End Sub

End Class
