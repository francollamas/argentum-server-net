Option Strict Off
Option Explicit On
Module Migration

	Private StopWatch As Stopwatch = Stopwatch.StartNew()

	' TODO MIGRA: a medianoche se reinicia. Asi que esto NO escala, pero sirve para probar q anda bien el server...
	Public Function GetTickCount() As Long
		'Convertir Timer (segundos desde medianoche) a milisegundos
		GetTickCount = StopWatch.ElapsedMilliseconds
	End Function

	'UPGRADE_NOTE: str se actualizó a str_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Function migr_LenB(ByVal str_Renamed As String) As Integer
		If str_Renamed = "" Then
			migr_LenB = 0
		Else
			migr_LenB = Len(str_Renamed) * 2
		End If
	End Function

	Function migr_InStrB(ByVal s1 As String, ByVal s2 As String) As Integer
		Dim i As Integer
		Dim maxPos As Integer

		' Verificar que la subcadena no sea vacía
		If Len(s2) = 0 Then
			migr_InStrB = 1 ' Si la subcadena está vacía, consideramos que se encuentra al principio
			Exit Function
		End If

		' La búsqueda solo puede ir hasta la longitud de la cadena principal menos la longitud de la subcadena
		maxPos = Len(s1) - Len(s2) + 1

		' Buscar la subcadena dentro de la cadena principal
		For i = 1 To maxPos
			If Mid(s1, i, Len(s2)) = s2 Then
				migr_InStrB = i
				Exit Function
			End If
		Next i

		' Si no se encuentra la subcadena, devolver 0
		migr_InStrB = 0
	End Function
End Module
