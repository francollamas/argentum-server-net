Option Strict Off
Option Explicit On
Module modHexaStrings
    Public Function hexMd52Asc(MD5 As String) As String
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim i As Integer
        Dim L As String

        If Len(MD5) And &H1 Then MD5 = "0" & MD5

        For i = 1 To Len(MD5)\2
            L = Mid(MD5, (2*i) - 1, 2)
            hexMd52Asc = hexMd52Asc & Chr(hexHex2Dec(L))
        Next i
    End Function

    'UPGRADE_NOTE: hex se actualizó a hex_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Public Function hexHex2Dec(hex_Renamed As String) As Integer
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        hexHex2Dec = Val("&H" & hex_Renamed)
    End Function

    Public Function txtOffset(Text As String, off As Short) As String
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim i As Integer
        Dim L As String

        For i = 1 To Len(Text)
            L = Mid(Text, i, 1)
            txtOffset = txtOffset & Chr((Asc(L) + off) And &HFF)
        Next i
    End Function
End Module
