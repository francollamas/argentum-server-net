Option Strict Off
Option Explicit On
Friend Class CColaArray
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

    Private m_maxelem As Integer

    Private m_array() As String
    Private m_lastelem As Short
    Private m_firstelem As Short
    Private m_size As Short

    Public Function IsEmpty() As Boolean
        IsEmpty = m_size = 0
    End Function

    Public Function IsFull() As Boolean
        'IsFull = m_lastelem = m_maxelem
        IsFull = m_size = m_maxelem
    End Function

    Public Function Push(ByVal aString As String) As Boolean

        If Not Me.IsFull Then

            If Me.IsEmpty Then m_firstelem = 1

            m_lastelem = m_lastelem + 1
            If (m_lastelem > m_maxelem) Then m_lastelem = m_lastelem - m_maxelem
            m_size = m_size + 1
            m_array(m_lastelem) = aString

            Push = True
        Else
            Push = False
        End If
    End Function

    Public Function Pop() As String

        If Not Me.IsEmpty Then

            Pop = m_array(m_firstelem)
            m_firstelem = m_firstelem + 1
            If (m_firstelem > m_maxelem) Then m_firstelem = m_firstelem - m_maxelem
            m_size = m_size - 1

            'If m_firstelem > m_lastelem And m_size = 0 Then
            If m_size = 0 Then
                m_lastelem = 0
                m_firstelem = 0
                m_size = 0
            End If
        Else
            Pop = vbNullString

        End If
    End Function

    'UPGRADE_NOTE: Class_Initialize se actualizó a Class_Initialize_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Private Sub Class_Initialize_Renamed()
        m_lastelem = 0
        m_firstelem = 0
        m_size = 0
        m_maxelem = 300

        'UPGRADE_WARNING: El límite inferior de la matriz m_array ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim m_array(m_maxelem)
    End Sub

    Public Sub New()
        MyBase.New()
        Class_Initialize_Renamed()
    End Sub


    Public Property MaxElems() As Integer
        Get
            MaxElems = m_maxelem
        End Get
        Set(ByVal Value As Integer)
            m_maxelem = Value
            'UPGRADE_WARNING: El límite inferior de la matriz m_array ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim Preserve m_array(m_maxelem)
        End Set
    End Property
End Class
