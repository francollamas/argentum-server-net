Option Strict Off
Option Explicit On
Friend Class frmUserList
	Inherits System.Windows.Forms.Form
	'**************************************************************
	' frmUserList.frm
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
	
	
	Private Sub Command1_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command1.Click
		Dim LoopC As Short
		
		Text2.Text = "MaxUsers: " & MaxUsers & vbCrLf
		Text2.Text = Text2.Text & "LastUser: " & LastUser & vbCrLf
		Text2.Text = Text2.Text & "NumUsers: " & NumUsers & vbCrLf
		'Text2.Text = Text2.Text & "" & vbCrLf
		
		List1.Items.Clear()
		
		For LoopC = 1 To MaxUsers
			List1.Items.Add(New VB6.ListBoxItem(VB6.Format(LoopC, "000") & " " & IIf(UserList(LoopC).flags.UserLogged, UserList(LoopC).name, ""), LoopC))
		Next LoopC
		
		
	End Sub
	
	Private Sub Command2_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command2.Click
		Dim LoopC As Short
		
		For LoopC = 1 To MaxUsers
			If UserList(LoopC).ConnID <> -1 And Not UserList(LoopC).flags.UserLogged Then
				Call CloseSocket(LoopC)
			End If
		Next LoopC
		
	End Sub
	
	'UPGRADE_WARNING: El evento List1.SelectedIndexChanged se puede desencadenar cuando se inicializa el formulario. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
	Private Sub List1_SelectedIndexChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles List1.SelectedIndexChanged
		Dim UserIndex As Short
		If List1.SelectedIndex <> -1 Then
			UserIndex = VB6.GetItemData(List1, List1.SelectedIndex)
			If UserIndex > 0 And UserIndex <= MaxUsers Then
				With UserList(UserIndex)
					Text1.Text = "UserLogged: " & .flags.UserLogged & vbCrLf
					Text1.Text = Text1.Text & "IdleCount: " & .Counters.IdleCount & vbCrLf
					Text1.Text = Text1.Text & "ConnId: " & .ConnID & vbCrLf
					Text1.Text = Text1.Text & "ConnIDValida: " & .ConnIDValida & vbCrLf
				End With
			End If
		End If
		
	End Sub
End Class
