Option Strict Off
Option Explicit On
Imports VB = Microsoft.VisualBasic
Module Migration
	' TODO MIGRA: a medianoche se reinicia. Asi que esto NO escala, pero sirve para probar q anda bien el server...
	Public Function GetTickCount() As Integer
		'Convertir Timer (segundos desde medianoche) a milisegundos
		GetTickCount = CInt(VB.Timer() * 1000)
	End Function
End Module