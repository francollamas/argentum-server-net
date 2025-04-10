Option Strict Off
Option Explicit On
Module ModAreas
    '**************************************************************
    ' ModAreas.bas - Module to allow the usage of areas instead of maps.
    ' Saves a lot of bandwidth.
    '
    ' Original Idea by Juan Martín Sotuyo Dodero (Maraxus)
    ' (juansotuyo@gmail.com)
    ' Implemented by Lucio N. Tourrilhes (DuNga)
    '**************************************************************

    '**************************************************************************
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
    '**************************************************************************

    ' Modulo de envio por areas compatible con la versión 9.10.x ... By DuNga

    '>>>>>>AREAS>>>>>AREAS>>>>>>>>AREAS>>>>>>>AREAS>>>>>>>>>>
    Public Structure AreaInfo
        Dim AreaPerteneceX As Short
        Dim AreaPerteneceY As Short
        Dim AreaReciveX As Short
        Dim AreaReciveY As Short
        Dim MinX As Short '-!!!
        Dim MinY As Short '-!!!
        Dim AreaID As Integer
    End Structure

    Public Structure ConnGroup
        Dim CountEntrys As Integer
        Dim OptValue As Integer
        Dim UserEntrys() As Integer
    End Structure

    Public Const USER_NUEVO As Byte = 255

    'Cuidado:
    ' ¡¡¡LAS AREAS ESTÁN HARDCODEADAS!!!
    Private CurDay As Byte
    Private CurHour As Byte

    'UPGRADE_WARNING: El límite inferior de la matriz AreasInfo ha cambiado de 1,1 a 0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    Private AreasInfo(100, 100) As Byte
    'UPGRADE_WARNING: El límite inferior de la matriz PosToArea ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    Private PosToArea(100) As Byte

    Private AreasRecive(12) As Short

    Public ConnGroups() As ConnGroup

    Public Sub InitAreas()
        '**************************************************************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: Unknow
        '
        '**************************************************************
        Dim LoopC As Integer
        Dim loopX As Integer

        ' Setup areas...
        For LoopC = 0 To 11
            AreasRecive(LoopC) = (2^LoopC) Or IIf(LoopC <> 0, 2^(LoopC - 1), 0) Or IIf(LoopC <> 11, 2^(LoopC + 1), 0)
        Next LoopC

        For LoopC = 1 To 100
            PosToArea(LoopC) = LoopC\9
        Next LoopC

        For LoopC = 1 To 100
            For loopX = 1 To 100
                'Usamos 121 IDs de area para saber si pasasamos de area "más rápido"
                AreasInfo(LoopC, loopX) = (LoopC\9 + 1)*(loopX\9 + 1)
            Next loopX
        Next LoopC

        'Setup AutoOptimizacion de areas
        CurDay = IIf(WeekDay(Today) > 6, 1, 2) 'A ke tipo de dia pertenece?
        CurHour = Fix(Hour(TimeOfDay)\3) 'A ke parte de la hora pertenece

        'UPGRADE_WARNING: El límite inferior de la matriz ConnGroups ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim ConnGroups(NumMaps)

        For LoopC = 1 To NumMaps
            ConnGroups(LoopC).OptValue = Val(GetVar(DatPath & "AreasStats.dat", "Mapa" & LoopC, CurDay & "-" & CurHour))

            If ConnGroups(LoopC).OptValue = 0 Then ConnGroups(LoopC).OptValue = 1
            'UPGRADE_WARNING: El límite inferior de la matriz ConnGroups(LoopC).UserEntrys ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim ConnGroups(LoopC).UserEntrys(ConnGroups(LoopC).OptValue)
        Next LoopC
    End Sub

    Public Sub AreasOptimizacion()
        '**************************************************************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: Unknow
        'Es la función de autooptimizacion.... la idea es no mandar redimensionando arrays grandes todo el tiempo
        '**************************************************************
        Dim LoopC As Integer
        Dim tCurDay As Byte
        Dim tCurHour As Byte
        Dim EntryValue As Integer

        If (CurDay <> IIf(WeekDay(Today) > 6, 1, 2)) Or (CurHour <> Fix(Hour(TimeOfDay)\3)) Then

            tCurDay = IIf(WeekDay(Today) > 6, 1, 2) 'A ke tipo de dia pertenece?
            tCurHour = Fix(Hour(TimeOfDay)\3) 'A ke parte de la hora pertenece

            For LoopC = 1 To NumMaps
                EntryValue = Val(GetVar(DatPath & "AreasStats.dat", "Mapa" & LoopC, CurDay & "-" & CurHour))
                Call _
                    WriteVar(DatPath & "AreasStats.dat", "Mapa" & LoopC, CurDay & "-" & CurHour,
                             CStr(CShort((EntryValue + ConnGroups(LoopC).OptValue)\2)))

                ConnGroups(LoopC).OptValue = Val(GetVar(DatPath & "AreasStats.dat", "Mapa" & LoopC,
                                                        tCurDay & "-" & tCurHour))
                If ConnGroups(LoopC).OptValue = 0 Then ConnGroups(LoopC).OptValue = 1
                'UPGRADE_WARNING: El límite inferior de la matriz ConnGroups(LoopC).UserEntrys ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                If ConnGroups(LoopC).OptValue >= MapInfo_Renamed(LoopC).NumUsers Then _
                    ReDim Preserve ConnGroups(LoopC).UserEntrys(ConnGroups(LoopC).OptValue)
            Next LoopC

            CurDay = tCurDay
            CurHour = tCurHour
        End If
    End Sub

    Public Sub CheckUpdateNeededUser(ByVal UserIndex As Short, ByVal Head As Byte,
                                     Optional ByVal ButIndex As Boolean = False)
        '**************************************************************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: 15/07/2009
        'Es la función clave del sistema de areas... Es llamada al mover un user
        '15/07/2009: ZaMa - Now it doesn't send an invisible admin char info
        '**************************************************************
        If UserList(UserIndex).AreasInfo.AreaID = AreasInfo(UserList(UserIndex).Pos.X, UserList(UserIndex).Pos.Y) Then _
            Exit Sub

        Dim X, MinY, MinX, MaxX, MaxY, Y As Integer
        Dim TempInt, Map As Integer

        With UserList(UserIndex)
            MinX = .AreasInfo.MinX
            MinY = .AreasInfo.MinY

            If Head = Declaraciones.eHeading.NORTH Then
                MaxY = MinY - 1
                MinY = MinY - 9
                MaxX = MinX + 26
                .AreasInfo.MinX = CShort(MinX)
                .AreasInfo.MinY = CShort(MinY)

            ElseIf Head = Declaraciones.eHeading.SOUTH Then
                MaxY = MinY + 35
                MinY = MinY + 27
                MaxX = MinX + 26
                .AreasInfo.MinX = CShort(MinX)
                .AreasInfo.MinY = CShort(MinY - 18)

            ElseIf Head = Declaraciones.eHeading.WEST Then
                MaxX = MinX - 1
                MinX = MinX - 9
                MaxY = MinY + 26
                .AreasInfo.MinX = CShort(MinX)
                .AreasInfo.MinY = CShort(MinY)


            ElseIf Head = Declaraciones.eHeading.EAST Then
                MaxX = MinX + 35
                MinX = MinX + 27
                MaxY = MinY + 26
                .AreasInfo.MinX = CShort(MinX - 18)
                .AreasInfo.MinY = CShort(MinY)


            ElseIf Head = USER_NUEVO Then
                'Esto pasa por cuando cambiamos de mapa o logeamos...
                MinY = ((.Pos.Y\9) - 1)*9
                MaxY = MinY + 26

                MinX = ((.Pos.X\9) - 1)*9
                MaxX = MinX + 26

                .AreasInfo.MinX = CShort(MinX)
                .AreasInfo.MinY = CShort(MinY)
            End If

            If MinY < 1 Then MinY = 1
            If MinX < 1 Then MinX = 1
            If MaxY > 100 Then MaxY = 100
            If MaxX > 100 Then MaxX = 100

            Map = .Pos.Map

            'Esto es para ke el cliente elimine lo "fuera de area..."
            Call WriteAreaChanged(UserIndex)

            'Actualizamos!!!
            For X = MinX To MaxX
                For Y = MinY To MaxY

                    '<<< User >>>
                    If MapData(Map, X, Y).UserIndex Then

                        TempInt = MapData(Map, X, Y).UserIndex

                        If UserIndex <> TempInt Then

                            ' Solo avisa al otro cliente si no es un admin invisible
                            If Not (UserList(TempInt).flags.AdminInvisible = 1) Then
                                Call MakeUserChar(False, UserIndex, TempInt, Map, X, Y)

                                'Si el user estaba invisible le avisamos al nuevo cliente de eso
                                If UserList(TempInt).flags.invisible Or UserList(TempInt).flags.Oculto Then
                                    If _
                                        .flags.Privilegios And
                                        (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or
                                         Declaraciones.PlayerType.RoleMaster) Then
                                        Call _
                                            WriteSetInvisible(UserIndex, UserList(TempInt).Char_Renamed.CharIndex, True)
                                    End If
                                End If
                            End If

                            ' Solo avisa al otro cliente si no es un admin invisible
                            If Not (.flags.AdminInvisible = 1) Then
                                Call MakeUserChar(False, TempInt, UserIndex, .Pos.Map, .Pos.X, .Pos.Y)

                                If .flags.invisible Or .flags.Oculto Then
                                    If UserList(TempInt).flags.Privilegios And Declaraciones.PlayerType.User Then
                                        Call WriteSetInvisible(TempInt, .Char_Renamed.CharIndex, True)
                                    End If
                                End If
                            End If

                            Call FlushBuffer(TempInt)

                        ElseIf Head = USER_NUEVO Then
                            If Not ButIndex Then
                                Call MakeUserChar(False, UserIndex, UserIndex, Map, X, Y)
                            End If
                        End If
                    End If

                    '<<< Npc >>>
                    If MapData(Map, X, Y).NpcIndex Then
                        Call MakeNPCChar(False, UserIndex, MapData(Map, X, Y).NpcIndex, Map, X, Y)
                    End If

                    '<<< Item >>>
                    If MapData(Map, X, Y).ObjInfo.ObjIndex Then
                        TempInt = MapData(Map, X, Y).ObjInfo.ObjIndex
                        If Not EsObjetoFijo(ObjData_Renamed(TempInt).OBJType) Then
                            Call WriteObjectCreate(UserIndex, ObjData_Renamed(TempInt).GrhIndex, X, Y)

                            If ObjData_Renamed(TempInt).OBJType = Declaraciones.eOBJType.otPuertas Then
                                Call Bloquear(False, UserIndex, X, Y, MapData(Map, X, Y).Blocked)
                                Call Bloquear(False, UserIndex, X - 1, Y, MapData(Map, X - 1, Y).Blocked)
                            End If
                        End If
                    End If

                Next Y
            Next X

            'Precalculados :P
            TempInt = .Pos.X\9
            .AreasInfo.AreaReciveX = AreasRecive(TempInt)
            .AreasInfo.AreaPerteneceX = 2^TempInt

            TempInt = .Pos.Y\9
            .AreasInfo.AreaReciveY = AreasRecive(TempInt)
            .AreasInfo.AreaPerteneceY = 2^TempInt

            .AreasInfo.AreaID = AreasInfo(.Pos.X, .Pos.Y)
        End With
    End Sub

    Public Sub CheckUpdateNeededNpc(ByVal NpcIndex As Short, ByVal Head As Byte)
        '**************************************************************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: Unknow
        ' Se llama cuando se mueve un Npc
        '**************************************************************
        If Npclist(NpcIndex).AreasInfo.AreaID = AreasInfo(Npclist(NpcIndex).Pos.X, Npclist(NpcIndex).Pos.Y) Then _
            Exit Sub

        Dim X, MinY, MinX, MaxX, MaxY, Y As Integer
        Dim TempInt As Integer

        With Npclist(NpcIndex)
            MinX = .AreasInfo.MinX
            MinY = .AreasInfo.MinY

            If Head = Declaraciones.eHeading.NORTH Then
                MaxY = MinY - 1
                MinY = MinY - 9
                MaxX = MinX + 26
                .AreasInfo.MinX = CShort(MinX)
                .AreasInfo.MinY = CShort(MinY)

            ElseIf Head = Declaraciones.eHeading.SOUTH Then
                MaxY = MinY + 35
                MinY = MinY + 27
                MaxX = MinX + 26
                .AreasInfo.MinX = CShort(MinX)
                .AreasInfo.MinY = CShort(MinY - 18)

            ElseIf Head = Declaraciones.eHeading.WEST Then
                MaxX = MinX - 1
                MinX = MinX - 9
                MaxY = MinY + 26
                .AreasInfo.MinX = CShort(MinX)
                .AreasInfo.MinY = CShort(MinY)


            ElseIf Head = Declaraciones.eHeading.EAST Then
                MaxX = MinX + 35
                MinX = MinX + 27
                MaxY = MinY + 26
                .AreasInfo.MinX = CShort(MinX - 18)
                .AreasInfo.MinY = CShort(MinY)


            ElseIf Head = USER_NUEVO Then
                'Esto pasa por cuando cambiamos de mapa o logeamos...
                MinY = ((.Pos.Y\9) - 1)*9
                MaxY = MinY + 26

                MinX = ((.Pos.X\9) - 1)*9
                MaxX = MinX + 26

                .AreasInfo.MinX = CShort(MinX)
                .AreasInfo.MinY = CShort(MinY)
            End If

            If MinY < 1 Then MinY = 1
            If MinX < 1 Then MinX = 1
            If MaxY > 100 Then MaxY = 100
            If MaxX > 100 Then MaxX = 100


            'Actualizamos!!!
            If MapInfo_Renamed(.Pos.Map).NumUsers <> 0 Then
                For X = MinX To MaxX
                    For Y = MinY To MaxY
                        If MapData(.Pos.Map, X, Y).UserIndex Then _
                            Call _
                                MakeNPCChar(False, MapData(.Pos.Map, X, Y).UserIndex, NpcIndex, .Pos.Map, .Pos.X, .Pos.Y)
                    Next Y
                Next X
            End If

            'Precalculados :P
            TempInt = .Pos.X\9
            .AreasInfo.AreaReciveX = AreasRecive(TempInt)
            .AreasInfo.AreaPerteneceX = 2^TempInt

            TempInt = .Pos.Y\9
            .AreasInfo.AreaReciveY = AreasRecive(TempInt)
            .AreasInfo.AreaPerteneceY = 2^TempInt

            .AreasInfo.AreaID = AreasInfo(.Pos.X, .Pos.Y)
        End With
    End Sub

    Public Sub QuitarUser(ByVal UserIndex As Short, ByVal Map As Short)
        '**************************************************************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: Unknow
        '
        '**************************************************************
        Dim TempVal As Integer
        Dim LoopC As Integer

        'Search for the user
        For LoopC = 1 To ConnGroups(Map).CountEntrys
            If ConnGroups(Map).UserEntrys(LoopC) = UserIndex Then Exit For
        Next LoopC

        'Char not found
        If LoopC > ConnGroups(Map).CountEntrys Then Exit Sub

        'Remove from old map
        ConnGroups(Map).CountEntrys = ConnGroups(Map).CountEntrys - 1
        TempVal = ConnGroups(Map).CountEntrys

        'Move list back
        For LoopC = LoopC To TempVal
            ConnGroups(Map).UserEntrys(LoopC) = ConnGroups(Map).UserEntrys(LoopC + 1)
        Next LoopC

        If TempVal > ConnGroups(Map).OptValue Then 'Nescesito Redim?
            'UPGRADE_WARNING: El límite inferior de la matriz ConnGroups(Map).UserEntrys ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim Preserve ConnGroups(Map).UserEntrys(TempVal)
        End If
    End Sub

    Public Sub AgregarUser(ByVal UserIndex As Short, ByVal Map As Short, Optional ByVal ButIndex As Boolean = False)
        '**************************************************************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: 04/01/2007
        'Modified by Juan Martín Sotuyo Dodero (Maraxus)
        '   - Now the method checks for repetead users instead of trusting parameters.
        '   - If the character is new to the map, update it
        '**************************************************************
        Dim TempVal As Integer
        Dim EsNuevo As Boolean
        Dim i As Integer

        If Not MapaValido(Map) Then Exit Sub

        EsNuevo = True

        'Prevent adding repeated users
        For i = 1 To ConnGroups(Map).CountEntrys
            If ConnGroups(Map).UserEntrys(i) = UserIndex Then
                EsNuevo = False
                Exit For
            End If
        Next i

        If EsNuevo Then
            'Update map and connection groups data
            ConnGroups(Map).CountEntrys = ConnGroups(Map).CountEntrys + 1
            TempVal = ConnGroups(Map).CountEntrys

            If TempVal > ConnGroups(Map).OptValue Then 'Nescesito Redim
                'UPGRADE_WARNING: El límite inferior de la matriz ConnGroups(Map).UserEntrys ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                ReDim Preserve ConnGroups(Map).UserEntrys(TempVal)
            End If

            ConnGroups(Map).UserEntrys(TempVal) = UserIndex
        End If

        With UserList(UserIndex)
            'Update user
            .AreasInfo.AreaID = 0

            .AreasInfo.AreaPerteneceX = 0
            .AreasInfo.AreaPerteneceY = 0
            .AreasInfo.AreaReciveX = 0
            .AreasInfo.AreaReciveY = 0
        End With

        Call CheckUpdateNeededUser(UserIndex, USER_NUEVO, ButIndex)
    End Sub

    Public Sub AgregarNpc(ByVal NpcIndex As Short)
        '**************************************************************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: Unknow
        '
        '**************************************************************
        With Npclist(NpcIndex)
            .AreasInfo.AreaID = 0

            .AreasInfo.AreaPerteneceX = 0
            .AreasInfo.AreaPerteneceY = 0
            .AreasInfo.AreaReciveX = 0
            .AreasInfo.AreaReciveY = 0
        End With

        Call CheckUpdateNeededNpc(NpcIndex, USER_NUEVO)
    End Sub
End Module
