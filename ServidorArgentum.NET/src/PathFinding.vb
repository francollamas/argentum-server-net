Option Strict Off
Option Explicit On
Module PathFinding
    Private Const ROWS As Short = 100
    Private Const COLUMS As Short = 100
    Private Const MAXINT As Short = 1000

    Private Structure tIntermidiateWork
        Dim Known As Boolean
        Dim DistV As Short
        Dim PrevV As tVertice
    End Structure

    'UPGRADE_WARNING: El límite inferior de la matriz TmpArray ha cambiado de 1,1 a 0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    Dim TmpArray(ROWS, COLUMS) As tIntermidiateWork

    Dim TilePosY As Short

    Private Function Limites(ByVal vfila As Short, ByVal vcolu As Short) As Object
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Limites. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        Limites = vcolu >= 1 And vcolu <= COLUMS And vfila >= 1 And vfila <= ROWS
    End Function

    Private Function IsWalkable(ByVal map As Short, ByVal row As Short, ByVal Col As Short, ByVal NpcIndex As Short) _
        As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        IsWalkable = MapData(map, row, Col).Blocked = 0 And MapData(map, row, Col).NpcIndex = 0

        If MapData(map, row, Col).UserIndex <> 0 Then
            If MapData(map, row, Col).UserIndex <> Npclist(NpcIndex).PFINFO.TargetUser Then IsWalkable = False
        End If
    End Function

    Private Sub ProcessAdjacents(ByVal MapIndex As Short, ByRef T(,) As tIntermidiateWork, ByRef vfila As Short,
                                 ByRef vcolu As Short, ByVal NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim V As tVertice
        Dim j As Short
        'Look to North
        j = vfila - 1
        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Limites(j, vcolu). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        If Limites(j, vcolu) Then
            If IsWalkable(MapIndex, j, vcolu, NpcIndex) Then
                'Nos aseguramos que no hay un camino más corto
                If T(j, vcolu).DistV = MAXINT Then
                    'Actualizamos la tabla de calculos intermedios
                    T(j, vcolu).DistV = T(vfila, vcolu).DistV + 1
                    T(j, vcolu).PrevV.X = vcolu
                    T(j, vcolu).PrevV.Y = vfila
                    'Mete el vertice en la cola
                    V.X = vcolu
                    V.Y = j
                    Call Push(V)
                End If
            End If
        End If
        j = vfila + 1
        'look to south
        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Limites(j, vcolu). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        If Limites(j, vcolu) Then
            If IsWalkable(MapIndex, j, vcolu, NpcIndex) Then
                'Nos aseguramos que no hay un camino más corto
                If T(j, vcolu).DistV = MAXINT Then
                    'Actualizamos la tabla de calculos intermedios
                    T(j, vcolu).DistV = T(vfila, vcolu).DistV + 1
                    T(j, vcolu).PrevV.X = vcolu
                    T(j, vcolu).PrevV.Y = vfila
                    'Mete el vertice en la cola
                    V.X = vcolu
                    V.Y = j
                    Call Push(V)
                End If
            End If
        End If
        'look to west
        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Limites(vfila, vcolu - 1). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        If Limites(vfila, vcolu - 1) Then
            If IsWalkable(MapIndex, vfila, vcolu - 1, NpcIndex) Then
                'Nos aseguramos que no hay un camino más corto
                If T(vfila, vcolu - 1).DistV = MAXINT Then
                    'Actualizamos la tabla de calculos intermedios
                    T(vfila, vcolu - 1).DistV = T(vfila, vcolu).DistV + 1
                    T(vfila, vcolu - 1).PrevV.X = vcolu
                    T(vfila, vcolu - 1).PrevV.Y = vfila
                    'Mete el vertice en la cola
                    V.X = vcolu - 1
                    V.Y = vfila
                    Call Push(V)
                End If
            End If
        End If
        'look to east
        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Limites(vfila, vcolu + 1). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        If Limites(vfila, vcolu + 1) Then
            If IsWalkable(MapIndex, vfila, vcolu + 1, NpcIndex) Then
                'Nos aseguramos que no hay un camino más corto
                If T(vfila, vcolu + 1).DistV = MAXINT Then
                    'Actualizamos la tabla de calculos intermedios
                    T(vfila, vcolu + 1).DistV = T(vfila, vcolu).DistV + 1
                    T(vfila, vcolu + 1).PrevV.X = vcolu
                    T(vfila, vcolu + 1).PrevV.Y = vfila
                    'Mete el vertice en la cola
                    V.X = vcolu + 1
                    V.Y = vfila
                    Call Push(V)
                End If
            End If
        End If
    End Sub


    Public Sub SeekPath(ByVal NpcIndex As Short, Optional ByVal MaxSteps As Short = 30)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        'This Sub seeks a path from the npclist(npcindex).pos
        'to the location NPCList(NpcIndex).PFINFO.Target.
        'The optional parameter MaxSteps is the maximum of steps
        'allowed for the path.
        '***************************************************

        Dim cur_npc_pos As tVertice
        Dim tar_npc_pos As tVertice
        Dim V As tVertice
        Dim NpcMap As Short
        Dim steps As Short

        NpcMap = Npclist(NpcIndex).Pos.map

        steps = 0

        cur_npc_pos.X = Npclist(NpcIndex).Pos.Y
        cur_npc_pos.Y = Npclist(NpcIndex).Pos.X

        tar_npc_pos.X = Npclist(NpcIndex).PFINFO.Target.X '  UserList(NPCList(NpcIndex).PFINFO.TargetUser).Pos.X
        tar_npc_pos.Y = Npclist(NpcIndex).PFINFO.Target.Y '  UserList(NPCList(NpcIndex).PFINFO.TargetUser).Pos.Y

        Call InitializeTable(TmpArray, cur_npc_pos)
        Call InitQueue()

        'We add the first vertex to the Queue
        Call Push(cur_npc_pos)

        Do While (Not IsEmpty)
            If steps > MaxSteps Then Exit Do
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto V. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            V = Pop
            If V.X = tar_npc_pos.X And V.Y = tar_npc_pos.Y Then Exit Do
            Call ProcessAdjacents(NpcMap, TmpArray, V.Y, V.X, NpcIndex)
        Loop

        Call MakePath(NpcIndex)
    End Sub

    Private Sub MakePath(ByVal NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        'Builds the path previously calculated
        '***************************************************

        Dim Pasos As Short
        Dim miV As tVertice
        Dim i As Short

        Pasos = TmpArray(Npclist(NpcIndex).PFINFO.Target.Y, Npclist(NpcIndex).PFINFO.Target.X).DistV
        Npclist(NpcIndex).PFINFO.PathLenght = Pasos


        If Pasos = MAXINT Then
            'MsgBox "There is no path."
            Npclist(NpcIndex).PFINFO.NoPath = True
            Npclist(NpcIndex).PFINFO.PathLenght = 0
            Exit Sub
        End If

        ReDim Npclist(NpcIndex).PFINFO.Path(Pasos)

        miV.X = Npclist(NpcIndex).PFINFO.Target.X
        miV.Y = Npclist(NpcIndex).PFINFO.Target.Y

        For i = Pasos To 1 Step - 1
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Npclist().PFINFO.Path(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Npclist(NpcIndex).PFINFO.Path(i) = miV
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto miV. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            miV = TmpArray(miV.Y, miV.X).PrevV
        Next i

        Npclist(NpcIndex).PFINFO.CurPos = 1
        Npclist(NpcIndex).PFINFO.NoPath = False
    End Sub

    Private Sub InitializeTable(ByRef T(,) As tIntermidiateWork, ByRef S As tVertice,
                                Optional ByVal MaxSteps As Short = 30)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        'Initialize the array where we calculate the path
        '***************************************************


        Dim j, k As Short
        Const anymap As Short = 1
        For j = S.Y - MaxSteps To S.Y + MaxSteps
            For k = S.X - MaxSteps To S.X + MaxSteps
                If InMapBounds(anymap, j, k) Then
                    T(j, k).Known = False
                    T(j, k).DistV = MAXINT
                    T(j, k).PrevV.X = 0
                    T(j, k).PrevV.Y = 0
                End If
            Next
        Next

        T(S.Y, S.X).Known = False
        T(S.Y, S.X).DistV = 0
    End Sub
End Module
