Option Strict Off
Option Explicit On
Friend Class clsAntiMassClon
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
	
	Private Const MaximoPersonajesPorIP As Short = 15
	Private m_coleccion As New Collection
	
	Public Function MaxPersonajes(ByRef sIp As String) As Boolean
		Dim i As Integer
		
		For i = 1 To m_coleccion.Count()
			'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto m_coleccion.Item(i).ip. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			If m_coleccion.Item(i).ip = sIp Then
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto m_coleccion.Item(i).PersonajesCreados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto m_coleccion.Item().PersonajesCreados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				m_coleccion.Item(i).PersonajesCreados = m_coleccion.Item(i).PersonajesCreados + 1
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto m_coleccion.Item().PersonajesCreados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				MaxPersonajes = (m_coleccion.Item(i).PersonajesCreados > MaximoPersonajesPorIP)
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto m_coleccion.Item().PersonajesCreados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				If MaxPersonajes Then m_coleccion.Item(i).PersonajesCreados = 16
				Exit Function
			End If
		Next i
		
		MaxPersonajes = False
		Exit Function
	End Function
	
	Public Function VaciarColeccion() As Object
		
		On Error GoTo Errhandler
		
		Dim i As Short
		
		For i = 1 To m_coleccion.Count()
			Call m_coleccion.Remove(1)
		Next 
		
		
		Exit Function
Errhandler: 
		Call LogError("Error en RestarConexion " & Err.Description)
	End Function
End Class