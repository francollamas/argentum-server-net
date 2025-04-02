Option Strict Off
Option Explicit On
Friend Class diccionario
	'**************************************************************
	' diccionario.cls
	'
	' Designed and implemented by Mariono Barrou (El Oso)
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
	
	'clase diccionario
	'basico, plain sin queso ni papa fritas
	
	
	'mi idea cuando hice esto, lo encontre en el rigido :p. Hecha por el oso
	
	
	Private Const MAX_ELEM As Short = 100
	
	Private Structure diccElem
		Dim clave As String
		Dim def As Object
	End Structure
	
	'UPGRADE_WARNING: El límite inferior de la matriz p_elementos ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
	Private p_elementos(MAX_ELEM) As diccElem 'visual basic es una mierda para usar memoria dinamica, asi que uso esto
	Private p_cant As Short
	
	'UPGRADE_NOTE: Class_Initialize se actualizó a Class_Initialize_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public Sub Class_Initialize_Renamed()
		'constructor
		p_cant = 0
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'UPGRADE_NOTE: Class_Terminate se actualizó a Class_Terminate_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public Sub Class_Terminate_Renamed()
		'destructor
		'destruir los variants?????
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	Public ReadOnly Property CantElem() As Short
		Get
			CantElem = p_cant
		End Get
	End Property
	
	Public Function AtPut(ByVal clave As String, ByRef elem As Object) As Boolean
		Dim i As Short
		
		AtPut = False
		
		If migr_LenB(clave) = 0 Then Exit Function
		
		clave = UCase(clave)
		
		If p_cant = MAX_ELEM Then
			AtPut = False
		Else
			For i = 1 To p_cant
				If clave = p_elementos(i).clave Then
					'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto elem. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_elementos().def. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					p_elementos(i).def = elem
					AtPut = True
					Exit For ' epa ;)
				End If
			Next i
			If Not AtPut Then
				p_cant = p_cant + 1
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto elem. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_elementos().def. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				p_elementos(p_cant).def = elem
				p_elementos(p_cant).clave = clave
				AtPut = True
			End If
			
		End If
	End Function
	
	Public Function At(ByVal clave As String) As Object
		Dim i As Short
		
		clave = UCase(clave)
		
		'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto At. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		At = Nothing ' Using Empty instead of Null
		For i = 1 To p_cant
			If clave = p_elementos(i).clave Then
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_elementos().def. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto At. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				At = p_elementos(i).def
				Exit Function
			End If
		Next i
		
	End Function
	
	Public Function AtIndex(ByVal i As Short) As String
		AtIndex = p_elementos(i).clave
	End Function
	
	
	Public Function MayorValor(ByRef cant As Short) As String
		'parchecito para el AO, me da la clave con mayor valor en valor
		'y la cantidad de claves con ese valor (por si hay empate)
		Dim i As Short
		Dim max As Short
		Dim clave As String
		max = -1
		cant = 0
		clave = vbNullString
		For i = 1 To p_cant
			'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_elementos().def. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			If max <= CShort(p_elementos(i).def) Then
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_elementos().def. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				cant = IIf(max = CShort(p_elementos(i).def), cant + 1, 1)
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_elementos().def. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				clave = IIf(max = CShort(p_elementos(i).def), clave & "," & p_elementos(i).clave, p_elementos(i).clave)
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_elementos().def. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				max = CShort(p_elementos(i).def)
			End If
		Next i
		
		MayorValor = clave
		
	End Function
	
	Public Sub DumpAll()
		Dim i As Short
		
		For i = 1 To MAX_ELEM
			p_elementos(i).clave = vbNullString
			'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_elementos().def. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			p_elementos(i).def = Nothing ' Using Empty instead of Null
		Next i
		p_cant = 0
		
	End Sub
End Class
