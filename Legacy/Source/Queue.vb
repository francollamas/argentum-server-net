Option Strict Off
Option Explicit On
Module Queue
    Public Structure tVertice
        Dim X As Short
        Dim Y As Short
    End Structure

    Private Const MAXELEM As Short = 1000

    Private m_array() As tVertice
    Private m_lastelem As Short
    Private m_firstelem As Short
    Private m_size As Short

    Public Function IsEmpty() As Boolean
        IsEmpty = m_size = 0
    End Function

    Public Function IsFull() As Boolean
        IsFull = m_lastelem = MAXELEM
    End Function

    Public Function Push(ByRef Vertice As tVertice) As Boolean

        If Not IsFull Then

            If IsEmpty Then m_firstelem = 1

            m_lastelem = m_lastelem + 1
            m_size = m_size + 1
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto m_array(m_lastelem). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            m_array(m_lastelem) = Vertice

            Push = True
        Else
            Push = False
        End If
    End Function

    Public Function Pop() As tVertice

        If Not IsEmpty Then

            Pop = m_array(m_firstelem)
            m_firstelem = m_firstelem + 1
            m_size = m_size - 1

            If m_firstelem > m_lastelem And m_size = 0 Then
                m_lastelem = 0
                m_firstelem = 0
                m_size = 0
            End If

        End If
    End Function

    Public Sub InitQueue()
        ReDim m_array(MAXELEM)
        m_lastelem = 0
        m_firstelem = 0
        m_size = 0
    End Sub
End Module
