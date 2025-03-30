Attribute VB_Name = "Migration"
Option Explicit

' TODO MIGRA: a medianoche se reinicia. Asi que esto NO escala, pero sirve para probar q anda bien el server...
Public Function GetTickCount() As Long
    'Convertir Timer (segundos desde medianoche) a milisegundos
    GetTickCount = CLng(Timer * 1000)
End Function

