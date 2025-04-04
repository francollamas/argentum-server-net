Option Strict Off
Option Explicit On
Module Matematicas
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
	
	
	Public Function Porcentaje(ByVal Total As Integer, ByVal Porc As Integer) As Integer
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Porcentaje = (Total * Porc) / 100
	End Function
	
	Function Distancia(ByRef wp1 As WorldPos, ByRef wp2 As WorldPos) As Integer
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		'Encuentra la distancia entre dos WorldPos
		Distancia = System.Math.Abs(wp1.X - wp2.X) + System.Math.Abs(wp1.Y - wp2.Y) + (System.Math.Abs(wp1.map - wp2.map) * 100)
	End Function
	
	Function Distance(ByVal X1 As Short, ByVal Y1 As Short, ByVal X2 As Short, ByVal Y2 As Short) As Double
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		'Encuentra la distancia entre dos puntos
		
		Distance = System.Math.Sqrt((Y1 - Y2) ^ 2 + (X1 - X2) ^ 2)
		
	End Function
	
	Public Function RandomNumber(ByVal LowerBound As Integer, ByVal UpperBound As Integer) As Integer
		'**************************************************************
		'Author: Juan Martín Sotuyo Dodero
		'Last Modify Date: 3/06/2006
		'Generates a random number in the range given - recoded to use longs and work properly with ranges
		'**************************************************************
		RandomNumber = Fix(Rnd() * (UpperBound - LowerBound + 1)) + LowerBound
	End Function
End Module
