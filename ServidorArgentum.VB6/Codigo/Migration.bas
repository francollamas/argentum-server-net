Attribute VB_Name = "Migration"
Option Explicit

' VB6 >>>>>
Private Declare Sub win_sleep Lib "kernel32.dll" Alias "Sleep" (ByVal dwMilliseconds As Long)
' <<<<<< VB6


' TODO MIGRA: a medianoche se reinicia. Asi que esto NO escala, pero sirve para probar q anda bien el server...
Public Function GetTickCount() As Long
    'Convertir Timer (segundos desde medianoche) a milisegundos
    GetTickCount = CLng(Timer * 1000)
End Function


Public Sub sleep(duration As Long)
    ' TODO MIGRA: Adaptar implementacion
    win_sleep duration
End Sub
