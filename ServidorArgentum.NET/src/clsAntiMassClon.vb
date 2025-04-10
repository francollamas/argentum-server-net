Option Strict Off
Option Explicit On
Friend Class clsAntiMassClon
    Private Const MaximoPersonajesPorIP As Short = 15
    Private m_coleccion As New Collection

    Public Function MaxPersonajes(ByRef sIp As String) As Boolean
        Dim i As Integer

        For i = 1 To m_coleccion.Count()
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto m_coleccion.Item(i).ip. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            If m_coleccion.Item(i).ip = sIp Then
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto m_coleccion.Item(i).PersonajesCreados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto m_coleccion.Item().PersonajesCreados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                m_coleccion.Item(i).PersonajesCreados = m_coleccion.Item(i).PersonajesCreados + 1
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto m_coleccion.Item().PersonajesCreados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                MaxPersonajes = (m_coleccion.Item(i).PersonajesCreados > MaximoPersonajesPorIP)
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto m_coleccion.Item().PersonajesCreados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                If MaxPersonajes Then m_coleccion.Item(i).PersonajesCreados = 16
                Exit Function
            End If
        Next i

        MaxPersonajes = False
        Exit Function
    End Function

    Public Function VaciarColeccion() As Object

        On Error GoTo Errhandler

        Dim i As Short

        For i = 1 To m_coleccion.Count()
            Call m_coleccion.Remove(1)
        Next


        Exit Function
        Errhandler:
        Call LogError("Error en RestarConexion " & Err.Description)
    End Function
End Class
