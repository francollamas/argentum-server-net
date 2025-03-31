Option Strict Off
Option Explicit On
Friend Class frmAdmin
	Inherits System.Windows.Forms.Form
	'**************************************************************
	' frmAdmin.frm
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
	
	
	'UPGRADE_WARNING: El evento cboPjs.TextChanged se puede desencadenar cuando se inicializa el formulario. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
	'UPGRADE_WARNING: ComboBox evento cboPjs.Change se actualizó a cboPjs.TextChanged, que tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="DFCDE711-9694-47D7-9C50-45A99CD8E91E"'
	Private Sub cboPjs_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cboPjs.TextChanged
		Call ActualizaPjInfo()
	End Sub
	
	'UPGRADE_WARNING: El evento cboPjs.SelectedIndexChanged se puede desencadenar cuando se inicializa el formulario. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
	Private Sub cboPjs_SelectedIndexChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cboPjs.SelectedIndexChanged
		Call ActualizaPjInfo()
	End Sub
	
	Private Sub Command1_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command1.Click
		Dim tIndex As Integer
		
		tIndex = NameIndex(cboPjs.Text)
		If tIndex > 0 Then
			Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg("Servidor> " & UserList(tIndex).name & " ha sido echado.", Protocol.FontTypeNames.FONTTYPE_SERVER))
			Call CloseSocket(tIndex)
		End If
		
	End Sub
	
	Public Sub ActualizaListaPjs()
		Dim LoopC As Integer
		
		With cboPjs
			.Items.Clear()
			
			For LoopC = 1 To LastUser
				If UserList(LoopC).flags.UserLogged And UserList(LoopC).ConnID >= 0 And UserList(LoopC).ConnIDValida Then
					If UserList(LoopC).flags.Privilegios And Declaraciones.PlayerType.User Then
						.Items.Add(New VB6.ListBoxItem(UserList(LoopC).name, LoopC))
					End If
				End If
			Next LoopC
		End With
		
	End Sub
	
	Private Sub Command3_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command3.Click
		Call EcharPjsNoPrivilegiados()
		
	End Sub
	
	Private Sub ActualizaPjInfo()
		Dim tIndex As Integer
		
		tIndex = NameIndex(cboPjs.Text)
		If tIndex > 0 Then
			With UserList(tIndex)
				Text1.Text = .outgoingData.length & " elementos en cola." & vbCrLf
			End With
		End If
		
	End Sub
End Class