Option Strict Off
Option Explicit On
Module modHexaStrings
	'Argentum Online 0.12.2
	'
	'Copyright (C) 2002 Márquez Pablo Ignacio
	'Copyright (C) 2002 Otto Perez
	'Copyright (C) 2002 Aaron Perkins
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
	
	'Modulo realizado por Gonzalo Larralde(CDT) <gonzalolarralde@yahoo.com.ar>
	'Para la conversion a caracteres de cadenas MD5 y de
	'semi encriptación de cadenas por ascii table offset
	
	
	Public Function hexMd52Asc(ByVal MD5 As String) As String
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim i As Integer
		Dim L As String
		
		If Len(MD5) And &H1 Then MD5 = "0" & MD5
		
		For i = 1 To Len(MD5) \ 2
			L = Mid(MD5, (2 * i) - 1, 2)
			hexMd52Asc = hexMd52Asc & Chr(hexHex2Dec(L))
		Next i
	End Function
	
	'UPGRADE_NOTE: hex se actualizó a hex_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public Function hexHex2Dec(ByVal hex_Renamed As String) As Integer
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		hexHex2Dec = Val("&H" & hex_Renamed)
	End Function
	
	Public Function txtOffset(ByVal Text As String, ByVal off As Short) As String
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim i As Integer
		Dim L As String
		
		For i = 1 To Len(Text)
			L = Mid(Text, i, 1)
			txtOffset = txtOffset & Chr((Asc(L) + off) And &HFF)
		Next i
	End Function
End Module
