Option Strict Off
Option Explicit On
Friend Class frmDebugNpc
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
	
	
	Private Sub Command1_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command1.Click
		Dim i, k As Short
		
		For i = 1 To LastNPC
			If Npclist(i).flags.NPCActive Then k = k + 1
		Next i
		
		Label1.Text = "Npcs Activos:" & k
		Label2.Text = "Npcs Libres:" & MAXNPCS - k
		Label3.Text = "LastNpcIndex:" & LastNPC
		Label4.Text = "MAXNPCS:" & MAXNPCS
		
	End Sub
	
	Private Sub Command2_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command2.Click
		Me.Close()
	End Sub
End Class
