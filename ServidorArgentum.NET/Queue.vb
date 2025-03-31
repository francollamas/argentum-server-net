Option Strict Off
Option Explicit On
Module Queue
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
	
	
	Public Structure tVertice
		Dim X As Short
		Dim Y As Short
	End Structure
	
	Private Const MAXELEM As Short = 1000
	
	Private m_array() As tVertice
	Private m_lastelem As Short
	Private m_firstelem As Short
	Private m_size As Short
	
	Public Function IsEmpty() As Boolean
		IsEmpty = m_size = 0
	End Function
	
	Public Function IsFull() As Boolean
		IsFull = m_lastelem = MAXELEM
	End Function
	
	Public Function Push(ByRef Vertice As tVertice) As Boolean
		
		If Not IsFull Then
			
			If IsEmpty Then m_firstelem = 1
			
			m_lastelem = m_lastelem + 1
			m_size = m_size + 1
			'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto m_array(m_lastelem). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			m_array(m_lastelem) = Vertice
			
			Push = True
		Else
			Push = False
		End If
		
	End Function
	
	Public Function Pop() As tVertice
		
		If Not IsEmpty Then
			
			Pop = m_array(m_firstelem)
			m_firstelem = m_firstelem + 1
			m_size = m_size - 1
			
			If m_firstelem > m_lastelem And m_size = 0 Then
				m_lastelem = 0
				m_firstelem = 0
				m_size = 0
			End If
			
		End If
		
	End Function
	
	Public Sub InitQueue()
		ReDim m_array(MAXELEM)
		m_lastelem = 0
		m_firstelem = 0
		m_size = 0
	End Sub
End Module