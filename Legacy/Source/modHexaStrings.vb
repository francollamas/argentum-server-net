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

        If MD5.Length And &H1 Then MD5 = "0" & MD5

        For i = 1 To MD5.Length\2
            L = MD5.Substring((2*i) - 2, 2)
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

        hexHex2Dec = Convert.ToInt64(hex_Renamed, 16)
    End Function

    Public Function txtOffset(Text As String, off As Short) As String
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim i As Integer
        Dim L As String

        For i = 1 To Text.Length
            L = Text.Substring(i - 1, 1)
            txtOffset = txtOffset & Chr((Asc(L) + off) And &HFF)
        Next i
    End Function
End Module
