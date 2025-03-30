Attribute VB_Name = "Migration"
Option Explicit

' TODO MIGRA: a medianoche se reinicia. Asi que esto NO escala, pero sirve para probar q anda bien el server...
Public Function GetTickCount() As Long
    'Convertir Timer (segundos desde medianoche) a milisegundos
    GetTickCount = CLng(Timer * 1000)
End Function

Function migr_LenB(ByVal str As String) As Long
    If str = "" Then
        migr_LenB = 0
    Else
        migr_LenB = Len(str) * 2
    End If
End Function

Function migr_InStrB(ByVal s1 As String, ByVal s2 As String) As Long
    Dim i As Long
    Dim maxPos As Long

    ' Verificar que la subcadena no sea vacía
    If Len(s2) = 0 Then
        migr_InStrB = 1 ' Si la subcadena está vacía, consideramos que se encuentra al principio
        Exit Function
    End If
    
    ' La búsqueda solo puede ir hasta la longitud de la cadena principal menos la longitud de la subcadena
    maxPos = Len(s1) - Len(s2) + 1
    
    ' Buscar la subcadena dentro de la cadena principal
    For i = 1 To maxPos
        If mid(s1, i, Len(s2)) = s2 Then
            migr_InStrB = i
            Exit Function
        End If
    Next i
    
    ' Si no se encuentra la subcadena, devolver 0
    migr_InStrB = 0
End Function

