Option Strict Off
Option Explicit On
Friend Class cCola
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
	
	'                    Metodos publicos
	'
	' Public sub Push(byval i as variant) mete el elemento i
	' al final de la cola.
	'
	' Public Function Pop As Variant: quita de la cola el primer elem
	' y lo devuelve
	'
	' Public Function VerElemento(ByVal Index As Integer) As Variant
	' muestra el elemento numero Index de la cola sin quitarlo
	'
	' Public Function PopByVal() As Variant: muestra el primer
	' elemento de la cola sin quitarlo
	'
	' Public Property Get Longitud() As Integer: devuelve la
	' cantidad de elementos que tiene la cola.
	
	Private Const FRENTE As Short = 1
	
	Private Cola As Collection
	
	'UPGRADE_NOTE: Reset se actualizó a Reset_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public Sub Reset_Renamed()
		On Error Resume Next
		
		Dim i As Short
		For i = 1 To Me.Longitud
			Cola.Remove(FRENTE)
		Next i
		
	End Sub
	
	Public ReadOnly Property Longitud() As Short
		Get
			Longitud = Cola.Count()
		End Get
	End Property
	
	Private Function IndexValido(ByVal i As Short) As Boolean
		IndexValido = i >= 1 And i <= Me.Longitud
	End Function
	
	'UPGRADE_NOTE: Class_Initialize se actualizó a Class_Initialize_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Private Sub Class_Initialize_Renamed()
		Cola = New Collection
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	Public Function VerElemento(ByVal index As Short) As String
		On Error Resume Next
		If IndexValido(index) Then
			'Pablo
			'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Cola.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			VerElemento = UCase(Cola.Item(index))
			'/Pablo
			'VerElemento = Cola(Index)
		Else
			VerElemento = CStr(0)
		End If
	End Function
	
	
	Public Sub Push(ByVal Nombre As String)
		On Error Resume Next
		'Mete elemento en la cola
		'Pablo
		Dim aux As String
		aux = TimeString & " " & UCase(Nombre)
		Call Cola.Add(aux)
		'/Pablo
		
		'Call Cola.Add(UCase$(Nombre))
	End Sub
	
	Public Function Pop() As String
		On Error Resume Next
		'Quita elemento de la cola
		If Cola.Count() > 0 Then
			'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Cola(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			Pop = Cola.Item(FRENTE)
			Call Cola.Remove(FRENTE)
		Else
			Pop = CStr(0)
		End If
	End Function
	
	Public Function PopByVal() As String
		On Error Resume Next
		'Call LogTarea("PopByVal SOS")
		
		'Quita elemento de la cola
		If Cola.Count() > 0 Then
			'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Cola.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			PopByVal = Cola.Item(1)
		Else
			PopByVal = CStr(0)
		End If
		
	End Function
	
	Public Function Existe(ByVal Nombre As String) As Boolean
		On Error Resume Next
		
		Dim V As String
		Dim i As Short
		Dim NombreEnMayusculas As String
		NombreEnMayusculas = UCase(Nombre)
		
		For i = 1 To Me.Longitud
			'Pablo
			V = Mid(Me.VerElemento(i), 10, Len(Me.VerElemento(i)))
			'/Pablo
			'V = Me.VerElemento(i)
			If V = NombreEnMayusculas Then
				Existe = True
				Exit Function
			End If
		Next 
		Existe = False
		
	End Function
	
	Public Sub Quitar(ByVal Nombre As String)
		On Error Resume Next
		Dim V As String
		Dim i As Short
		Dim NombreEnMayusculas As String
		
		NombreEnMayusculas = UCase(Nombre)
		
		For i = 1 To Me.Longitud
			'Pablo
			V = Mid(Me.VerElemento(i), 10, Len(Me.VerElemento(i)))
			'/Pablo
			'V = Me.VerElemento(i)
			If V = NombreEnMayusculas Then
				Call Cola.Remove(i)
				Exit Sub
			End If
		Next i
		
	End Sub
	
	Public Sub QuitarIndex(ByVal index As Short)
		On Error Resume Next
		If IndexValido(index) Then Call Cola.Remove(index)
	End Sub
	
	
	'UPGRADE_NOTE: Class_Terminate se actualizó a Class_Terminate_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Private Sub Class_Terminate_Renamed()
		'Destruimos el objeto Cola
		'UPGRADE_NOTE: El objeto Cola no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		Cola = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
End Class