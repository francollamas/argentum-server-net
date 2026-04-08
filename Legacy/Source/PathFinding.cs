using System;

namespace Legacy;

internal static class PathFinding
{
    private const short ROWS = 100;
    private const short COLUMS = 100;
    private const short MAXINT = 1000;

    // UPGRADE_WARNING: El límite inferior de la matriz TmpArray ha cambiado de 1,1 a 0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    private static tIntermidiateWork[,] TmpArray = new tIntermidiateWork[ROWS + 1, COLUMS + 1];

    private static bool Limites(short vfila, short vcolu)
    {
        bool LimitesRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Limites. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        LimitesRet = (vcolu >= 1) & (vcolu <= COLUMS) & (vfila >= 1) & (vfila <= ROWS);
        return LimitesRet;
    }

    private static bool IsWalkable(short map, short row, short Col, short NpcIndex)
    {
        bool IsWalkableRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        IsWalkableRet = (Declaraciones.MapData[map, row, Col].Blocked == 0) &
                        (Declaraciones.MapData[map, row, Col].NpcIndex == 0);

        if (Declaraciones.MapData[map, row, Col].UserIndex != 0)
            if (Declaraciones.MapData[map, row, Col].UserIndex != Declaraciones.Npclist[NpcIndex].PFINFO.TargetUser)
                IsWalkableRet = false;

        return IsWalkableRet;
    }

    private static void ProcessAdjacents(short MapIndex, ref tIntermidiateWork[,] T, ref short vfila, ref short vcolu,
        short NpcIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        Queue.tVertice V;
        short j;
        // Look to North
        j = Convert.ToInt16(vfila - 1);
        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Limites(j, vcolu). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        if (Limites(j, vcolu))
            if (IsWalkable(MapIndex, j, vcolu, NpcIndex))
                // Nos aseguramos que no hay un camino más corto
                if (T[j, vcolu].DistV == MAXINT)
                {
                    // Actualizamos la tabla de calculos intermedios
                    T[j, vcolu].DistV = Convert.ToInt16(T[vfila, vcolu].DistV + 1);
                    T[j, vcolu].PrevV.X = vcolu;
                    T[j, vcolu].PrevV.Y = vfila;
                    // Mete el vertice en la cola
                    V.X = vcolu;
                    V.Y = j;
                    Queue.Push(ref V);
                }

        j = Convert.ToInt16(vfila + 1);
        // look to south
        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Limites(j, vcolu). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        if (Limites(j, vcolu))
            if (IsWalkable(MapIndex, j, vcolu, NpcIndex))
                // Nos aseguramos que no hay un camino más corto
                if (T[j, vcolu].DistV == MAXINT)
                {
                    // Actualizamos la tabla de calculos intermedios
                    T[j, vcolu].DistV = Convert.ToInt16(T[vfila, vcolu].DistV + 1);
                    T[j, vcolu].PrevV.X = vcolu;
                    T[j, vcolu].PrevV.Y = vfila;
                    // Mete el vertice en la cola
                    V.X = vcolu;
                    V.Y = j;
                    Queue.Push(ref V);
                }

        // look to west
        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Limites(vfila, vcolu - 1). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        if (Limites(vfila, Convert.ToInt16(vcolu - 1)))
            if (IsWalkable(MapIndex, vfila, Convert.ToInt16(vcolu - 1), NpcIndex))
                // Nos aseguramos que no hay un camino más corto
                if (T[vfila, Convert.ToInt16(vcolu - 1)].DistV == MAXINT)
                {
                    // Actualizamos la tabla de calculos intermedios
                    T[vfila, Convert.ToInt16(vcolu - 1)].DistV = Convert.ToInt16(T[vfila, vcolu].DistV + 1);
                    T[vfila, Convert.ToInt16(vcolu - 1)].PrevV.X = vcolu;
                    T[vfila, Convert.ToInt16(vcolu - 1)].PrevV.Y = vfila;
                    // Mete el vertice en la cola
                    V.X = Convert.ToInt16(vcolu - 1);
                    V.Y = vfila;
                    Queue.Push(ref V);
                }

        // look to east
        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Limites(vfila, vcolu + 1). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        if (Limites(vfila, Convert.ToInt16(vcolu + 1)))
            if (IsWalkable(MapIndex, vfila, Convert.ToInt16(vcolu + 1), NpcIndex))
                // Nos aseguramos que no hay un camino más corto
                if (T[vfila, Convert.ToInt16(vcolu + 1)].DistV == MAXINT)
                {
                    // Actualizamos la tabla de calculos intermedios
                    T[vfila, Convert.ToInt16(vcolu + 1)].DistV = Convert.ToInt16(T[vfila, vcolu].DistV + 1);
                    T[vfila, Convert.ToInt16(vcolu + 1)].PrevV.X = vcolu;
                    T[vfila, Convert.ToInt16(vcolu + 1)].PrevV.Y = vfila;
                    // Mete el vertice en la cola
                    V.X = Convert.ToInt16(vcolu + 1);
                    V.Y = vfila;
                    Queue.Push(ref V);
                }
    }


    public static void SeekPath(short NpcIndex, short MaxSteps = 30)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // This Sub seeks a path from the npclist(npcindex).pos
        // to the location NPCList(NpcIndex).PFINFO.Target.
        // The optional parameter MaxSteps is the maximum of steps
        // allowed for the path.
        // ***************************************************

        Queue.tVertice cur_npc_pos;
        Queue.tVertice tar_npc_pos;
        Queue.tVertice V;
        short NpcMap;
        short steps;

        NpcMap = Declaraciones.Npclist[NpcIndex].Pos.Map;

        steps = 0;

        cur_npc_pos.X = Declaraciones.Npclist[NpcIndex].Pos.Y;
        cur_npc_pos.Y = Declaraciones.Npclist[NpcIndex].Pos.X;

        tar_npc_pos.X =
            Declaraciones.Npclist[NpcIndex].PFINFO.Target.X; // UserList(NPCList(NpcIndex).PFINFO.TargetUser).Pos.X
        tar_npc_pos.Y =
            Declaraciones.Npclist[NpcIndex].PFINFO.Target.Y; // UserList(NPCList(NpcIndex).PFINFO.TargetUser).Pos.Y

        InitializeTable(ref TmpArray, ref cur_npc_pos);
        Queue.InitQueue();

        // We add the first vertex to the Queue
        Queue.Push(ref cur_npc_pos);

        while (!Queue.IsEmpty())
        {
            if (steps > MaxSteps)
                break;
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto V. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            V = Queue.Pop();
            if ((V.X == tar_npc_pos.X) & (V.Y == tar_npc_pos.Y))
                break;
            ProcessAdjacents(NpcMap, ref TmpArray, ref V.Y, ref V.X, NpcIndex);
        }

        MakePath(NpcIndex);
    }

    private static void MakePath(short NpcIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // Builds the path previously calculated
        // ***************************************************

        short Pasos;
        Queue.tVertice miV;
        short i;

        Pasos = TmpArray[Declaraciones.Npclist[NpcIndex].PFINFO.Target.Y,
            Declaraciones.Npclist[NpcIndex].PFINFO.Target.X].DistV;
        Declaraciones.Npclist[NpcIndex].PFINFO.PathLenght = Pasos;


        if (Pasos == MAXINT)
        {
            // MsgBox "There is no path."
            Declaraciones.Npclist[NpcIndex].PFINFO.NoPath = true;
            Declaraciones.Npclist[NpcIndex].PFINFO.PathLenght = 0;
            return;
        }

        Declaraciones.Npclist[NpcIndex].PFINFO.Path = new Queue.tVertice[Pasos + 1];

        miV.X = Declaraciones.Npclist[NpcIndex].PFINFO.Target.X;
        miV.Y = Declaraciones.Npclist[NpcIndex].PFINFO.Target.Y;

        for (i = Pasos; i >= 1; i += -1)
        {
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Npclist().PFINFO.Path(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Declaraciones.Npclist[NpcIndex].PFINFO.Path[i] = miV;
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto miV. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            miV = TmpArray[miV.Y, miV.X].PrevV;
        }

        Declaraciones.Npclist[NpcIndex].PFINFO.CurPos = 1;
        Declaraciones.Npclist[NpcIndex].PFINFO.NoPath = false;
    }

    private static void InitializeTable(ref tIntermidiateWork[,] T, ref Queue.tVertice S, short MaxSteps = 30)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // Initialize the array where we calculate the path
        // ***************************************************


        short j, k;
        const short anymap = 1;
        var loopTo = (short)(S.Y + MaxSteps);
        for (j = (short)(S.Y - MaxSteps); j <= loopTo; j++)
        {
            var loopTo1 = (short)(S.X + MaxSteps);
            for (k = (short)(S.X - MaxSteps); k <= loopTo1; k++)
                if (Extra.InMapBounds(anymap, j, k))
                {
                    T[j, k].Known = false;
                    T[j, k].DistV = MAXINT;
                    T[j, k].PrevV.X = 0;
                    T[j, k].PrevV.Y = 0;
                }
        }

        T[S.Y, S.X].Known = false;
        T[S.Y, S.X].DistV = 0;
    }

    private struct tIntermidiateWork
    {
        public bool Known;
        public short DistV;
        public Queue.tVertice PrevV;
    }
}