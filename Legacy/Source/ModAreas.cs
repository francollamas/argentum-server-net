using System;
using System.Threading;
namespace Legacy;

internal static class ModAreas
{
    public const byte USER_NUEVO = 255;

    // Cuidado:
    // ¡¡¡LAS AREAS ESTÁN HARDCODEADAS!!!
    private static byte CurDay;
    private static byte CurHour;

    // UPGRADE_WARNING: El límite inferior de la matriz AreasInfo ha cambiado de 1,1 a 0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    private static readonly byte[,] AreasInfo = new byte[101, 101];

    // UPGRADE_WARNING: El límite inferior de la matriz PosToArea ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    private static readonly byte[] PosToArea = new byte[101];

    private static readonly short[] AreasRecive = new short[13];

    public static ConnGroup[] ConnGroups;

    public static void InitAreas()
    {
        // **************************************************************
        // Author: Lucio N. Tourrilhes (DuNga)
        // Last Modify Date: Unknow
        // 
        // **************************************************************
        int LoopC;
        int loopX;

        // Setup areas...
        for (LoopC = 0; LoopC <= 11; LoopC++)
            AreasRecive[LoopC] = Convert.ToInt16((long)Math.Round(Math.Pow(Convert.ToInt64(2), LoopC)) |
                                                 Convert.ToInt64(LoopC != 0
                                                     ? (long)Math.Round(Math.Pow(Convert.ToInt64(2),
                                                         Convert.ToInt64(LoopC - 1)))
                                                     : 0L) | Convert.ToInt64(LoopC != 11
                                                     ? (long)Math.Round(Math.Pow(Convert.ToInt64(2),
                                                         Convert.ToInt64(LoopC + 1)))
                                                     : 0L));

        for (LoopC = 1; LoopC <= 100; LoopC++)
            PosToArea[LoopC] = Convert.ToByte(LoopC / 9);

        for (LoopC = 1; LoopC <= 100; LoopC++)
        for (loopX = 1; loopX <= 100; loopX++)
            // Usamos 121 IDs de area para saber si pasasamos de area "más rápido"
            AreasInfo[LoopC, loopX] = Convert.ToByte((LoopC / 9 + 1) * (loopX / 9 + 1));

        // Setup AutoOptimizacion de areas
        CurDay = Convert.ToByte(((int)DateTime.Today.DayOfWeek + 1) > 6 ? 1 : 2); // A ke tipo de dia pertenece?
        CurHour = Convert.ToByte(
            (int)Math.Truncate(Thread.CurrentThread.CurrentCulture.Calendar.GetHour(DateTime.Now) /
                           3.0)); // A ke parte de la hora pertenece

        // UPGRADE_WARNING: El límite inferior de la matriz ConnGroups ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ConnGroups = new ConnGroup[Declaraciones.NumMaps + 1];

        var loopTo = (int)Declaraciones.NumMaps;
        for (LoopC = 1; LoopC <= loopTo; LoopC++)
        {
            var argEmptySpaces = 1024;
            ConnGroups[LoopC].OptValue = Convert.ToInt32(Migration.ParseVal(ES.GetVar(
                Declaraciones.DatPath + "AreasStats.dat", "Mapa" + LoopC, CurDay + "-" + CurHour, ref argEmptySpaces)));

            if (ConnGroups[LoopC].OptValue == 0)
                ConnGroups[LoopC].OptValue = 1;
            // UPGRADE_WARNING: El límite inferior de la matriz ConnGroups(LoopC).UserEntrys ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ConnGroups[LoopC].UserEntrys = new int[ConnGroups[LoopC].OptValue + 1];
        }
    }

    public static void AreasOptimizacion()
    {
        // **************************************************************
        // Author: Lucio N. Tourrilhes (DuNga)
        // Last Modify Date: Unknow
        // Es la función de autooptimizacion.... la idea es no mandar redimensionando arrays grandes todo el tiempo
        // **************************************************************
        int LoopC;
        byte tCurDay;
        byte tCurHour;
        int EntryValue;

        if ((CurDay != Convert.ToByte(((int)DateTime.Today.DayOfWeek + 1) > 6 ? 1 : 2)) | (CurHour !=
                Convert.ToByte(
                    (int)Math.Truncate(Thread.CurrentThread.CurrentCulture.Calendar.GetHour(DateTime.Now) / 3.0))))
        {
            tCurDay = Convert.ToByte(((int)DateTime.Today.DayOfWeek + 1) > 6 ? 1 : 2); // A ke tipo de dia pertenece?
            tCurHour = Convert.ToByte(
                (int)Math.Truncate(Thread.CurrentThread.CurrentCulture.Calendar.GetHour(DateTime.Now) /
                               3.0)); // A ke parte de la hora pertenece

            var loopTo = (int)Declaraciones.NumMaps;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                var argEmptySpaces = 1024;
                EntryValue = Convert.ToInt32(Migration.ParseVal(ES.GetVar(Declaraciones.DatPath + "AreasStats.dat",
                    "Mapa" + LoopC, CurDay + "-" + CurHour, ref argEmptySpaces)));
                ES.WriteVar(Declaraciones.DatPath + "AreasStats.dat", "Mapa" + LoopC, CurDay + "-" + CurHour,
                    Convert.ToInt16((EntryValue + ConnGroups[LoopC].OptValue) / 2).ToString());

                var argEmptySpaces1 = 1024;
                ConnGroups[LoopC].OptValue = Convert.ToInt32(Migration.ParseVal(ES.GetVar(
                    Declaraciones.DatPath + "AreasStats.dat", "Mapa" + LoopC, tCurDay + "-" + tCurHour,
                    ref argEmptySpaces1)));
                if (ConnGroups[LoopC].OptValue == 0)
                    ConnGroups[LoopC].OptValue = 1;
                // UPGRADE_WARNING: El límite inferior de la matriz ConnGroups(LoopC).UserEntrys ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                if (ConnGroups[LoopC].OptValue >= Declaraciones.mapInfo[LoopC].NumUsers)
                    Array.Resize(ref ConnGroups[LoopC].UserEntrys, ConnGroups[LoopC].OptValue + 1);
            }

            CurDay = tCurDay;
            CurHour = tCurHour;
        }
    }

    public static void CheckUpdateNeededUser(short UserIndex, byte Head, bool ButIndex = false)
    {
        // **************************************************************
        // Author: Lucio N. Tourrilhes (DuNga)
        // Last Modify Date: 15/07/2009
        // Es la función clave del sistema de areas... Es llamada al mover un user
        // 15/07/2009: ZaMa - Now it doesn't send an invisible admin char info
        // **************************************************************
        if (Declaraciones.UserList[UserIndex].AreasInfo.AreaID == AreasInfo[Declaraciones.UserList[UserIndex].Pos.X,
                Declaraciones.UserList[UserIndex].Pos.Y])
            return;

        int X, MinY, MinX, MaxX = default, MaxY = default, Y;
        int TempInt, Map;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            MinX = withBlock.AreasInfo.MinX;
            MinY = withBlock.AreasInfo.MinY;

            if (Head == (byte)Declaraciones.eHeading.NORTH)
            {
                MaxY = MinY - 1;
                MinY = MinY - 9;
                MaxX = MinX + 26;
                withBlock.AreasInfo.MinX = Convert.ToInt16(MinX);
                withBlock.AreasInfo.MinY = Convert.ToInt16(MinY);
            }

            else if (Head == (byte)Declaraciones.eHeading.SOUTH)
            {
                MaxY = MinY + 35;
                MinY = MinY + 27;
                MaxX = MinX + 26;
                withBlock.AreasInfo.MinX = Convert.ToInt16(MinX);
                withBlock.AreasInfo.MinY = Convert.ToInt16(MinY - 18);
            }

            else if (Head == (byte)Declaraciones.eHeading.WEST)
            {
                MaxX = MinX - 1;
                MinX = MinX - 9;
                MaxY = MinY + 26;
                withBlock.AreasInfo.MinX = Convert.ToInt16(MinX);
                withBlock.AreasInfo.MinY = Convert.ToInt16(MinY);
            }


            else if (Head == (byte)Declaraciones.eHeading.EAST)
            {
                MaxX = MinX + 35;
                MinX = MinX + 27;
                MaxY = MinY + 26;
                withBlock.AreasInfo.MinX = Convert.ToInt16(MinX - 18);
                withBlock.AreasInfo.MinY = Convert.ToInt16(MinY);
            }


            else if (Head == USER_NUEVO)
            {
                // Esto pasa por cuando cambiamos de mapa o logeamos...
                MinY = (withBlock.Pos.Y / 9 - 1) * 9;
                MaxY = MinY + 26;

                MinX = (withBlock.Pos.X / 9 - 1) * 9;
                MaxX = MinX + 26;

                withBlock.AreasInfo.MinX = Convert.ToInt16(MinX);
                withBlock.AreasInfo.MinY = Convert.ToInt16(MinY);
            }

            if (MinY < 1)
                MinY = 1;
            if (MinX < 1)
                MinX = 1;
            if (MaxY > 100)
                MaxY = 100;
            if (MaxX > 100)
                MaxX = 100;

            Map = withBlock.Pos.Map;

            // Esto es para ke el cliente elimine lo "fuera de area..."
            Protocol.WriteAreaChanged(UserIndex);

            // Actualizamos!!!
            var loopTo = MaxX;
            for (X = MinX; X <= loopTo; X++)
            {
                var loopTo1 = MaxY;
                for (Y = MinY; Y <= loopTo1; Y++)
                {
                    // <<< User >>>
                    if (Declaraciones.MapData[Map, X, Y].UserIndex != 0)
                    {
                        TempInt = Declaraciones.MapData[Map, X, Y].UserIndex;

                        if (UserIndex != TempInt)
                        {
                            // Solo avisa al otro cliente si no es un admin invisible
                            if (!(Declaraciones.UserList[TempInt].flags.AdminInvisible == 1))
                            {
                                var argButIndex = false;
                                UsUaRiOs.MakeUserChar(false, Convert.ToInt16(UserIndex), Convert.ToInt16(TempInt),
                                    Convert.ToInt16(Map), Convert.ToInt16(X), Convert.ToInt16(Y), ref argButIndex);

                                // Si el user estaba invisible le avisamos al nuevo cliente de eso
                                if ((Declaraciones.UserList[TempInt].flags.invisible != 0) |
                                    (Declaraciones.UserList[TempInt].flags.Oculto != 0))
                                    if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User |
                                                                        Declaraciones.PlayerType.Consejero |
                                                                        Declaraciones.PlayerType.RoleMaster)) != 0)
                                        Protocol.WriteSetInvisible(UserIndex,
                                            Declaraciones.UserList[TempInt].character.CharIndex, true);
                            }

                            // Solo avisa al otro cliente si no es un admin invisible
                            if (!(withBlock.flags.AdminInvisible == 1))
                            {
                                var argButIndex1 = false;
                                UsUaRiOs.MakeUserChar(false, Convert.ToInt16(TempInt), UserIndex, withBlock.Pos.Map,
                                    withBlock.Pos.X, withBlock.Pos.Y, ref argButIndex1);

                                if ((withBlock.flags.invisible != 0) | (withBlock.flags.Oculto != 0))
                                    if ((Declaraciones.UserList[TempInt].flags.Privilegios &
                                         Declaraciones.PlayerType.User) != 0)
                                        Protocol.WriteSetInvisible(Convert.ToInt16(TempInt),
                                            withBlock.character.CharIndex, true);
                            }

                            Protocol.FlushBuffer(Convert.ToInt16(TempInt));
                        }

                        else if (Head == USER_NUEVO)
                        {
                            if (!ButIndex)
                            {
                                var argButIndex2 = false;
                                UsUaRiOs.MakeUserChar(false, Convert.ToInt16(UserIndex), Convert.ToInt16(UserIndex),
                                    Convert.ToInt16(Map), Convert.ToInt16(X), Convert.ToInt16(Y), ref argButIndex2);
                            }
                        }
                    }

                    // <<< Npc >>>
                    if (Declaraciones.MapData[Map, X, Y].NpcIndex != 0)
                    {
                        var argsndIndex = Convert.ToInt16(UserIndex);
                        NPCs.MakeNPCChar(false, ref argsndIndex, ref Declaraciones.MapData[Map, X, Y].NpcIndex,
                            Convert.ToInt16(Map), Convert.ToInt16(X), Convert.ToInt16(Y));
                    }

                    // <<< Item >>>
                    if (Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex != 0)
                    {
                        TempInt = Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex;
                        if (!Extra.EsObjetoFijo(Declaraciones.objData[TempInt].OBJType))
                        {
                            Protocol.WriteObjectCreate(UserIndex, Declaraciones.objData[TempInt].GrhIndex,
                                Convert.ToByte(X), Convert.ToByte(Y));

                            if (Declaraciones.objData[TempInt].OBJType == Declaraciones.eOBJType.otPuertas)
                            {
                                General.Bloquear(false, UserIndex, Convert.ToInt16(X), Convert.ToInt16(Y),
                                    Declaraciones.MapData[Map, X, Y].Blocked != 0);
                                General.Bloquear(false, UserIndex, Convert.ToInt16(X - 1), Convert.ToInt16(Y),
                                    Declaraciones.MapData[Map, X - 1, Y].Blocked != 0);
                            }
                        }
                    }
                }
            }

            // Precalculados :P
            TempInt = withBlock.Pos.X / 9;
            withBlock.AreasInfo.AreaReciveX = AreasRecive[TempInt];
            withBlock.AreasInfo.AreaPerteneceX = Convert.ToInt16(Math.Pow(2d, TempInt));

            TempInt = withBlock.Pos.Y / 9;
            withBlock.AreasInfo.AreaReciveY = AreasRecive[TempInt];
            withBlock.AreasInfo.AreaPerteneceY = Convert.ToInt16(Math.Pow(2d, TempInt));

            withBlock.AreasInfo.AreaID = AreasInfo[withBlock.Pos.X, withBlock.Pos.Y];
        }
    }

    public static void CheckUpdateNeededNpc(short NpcIndex, byte Head)
    {
        // **************************************************************
        // Author: Lucio N. Tourrilhes (DuNga)
        // Last Modify Date: Unknow
        // Se llama cuando se mueve un Npc
        // **************************************************************
        if (Declaraciones.Npclist[NpcIndex].AreasInfo.AreaID == AreasInfo[Declaraciones.Npclist[NpcIndex].Pos.X,
                Declaraciones.Npclist[NpcIndex].Pos.Y])
            return;

        int X, MinY, MinX, MaxX = default, MaxY = default, Y;
        int TempInt;

        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
            MinX = withBlock.AreasInfo.MinX;
            MinY = withBlock.AreasInfo.MinY;

            if (Head == (byte)Declaraciones.eHeading.NORTH)
            {
                MaxY = MinY - 1;
                MinY = MinY - 9;
                MaxX = MinX + 26;
                withBlock.AreasInfo.MinX = Convert.ToInt16(MinX);
                withBlock.AreasInfo.MinY = Convert.ToInt16(MinY);
            }

            else if (Head == (byte)Declaraciones.eHeading.SOUTH)
            {
                MaxY = MinY + 35;
                MinY = MinY + 27;
                MaxX = MinX + 26;
                withBlock.AreasInfo.MinX = Convert.ToInt16(MinX);
                withBlock.AreasInfo.MinY = Convert.ToInt16(MinY - 18);
            }

            else if (Head == (byte)Declaraciones.eHeading.WEST)
            {
                MaxX = MinX - 1;
                MinX = MinX - 9;
                MaxY = MinY + 26;
                withBlock.AreasInfo.MinX = Convert.ToInt16(MinX);
                withBlock.AreasInfo.MinY = Convert.ToInt16(MinY);
            }


            else if (Head == (byte)Declaraciones.eHeading.EAST)
            {
                MaxX = MinX + 35;
                MinX = MinX + 27;
                MaxY = MinY + 26;
                withBlock.AreasInfo.MinX = Convert.ToInt16(MinX - 18);
                withBlock.AreasInfo.MinY = Convert.ToInt16(MinY);
            }


            else if (Head == USER_NUEVO)
            {
                // Esto pasa por cuando cambiamos de mapa o logeamos...
                MinY = (withBlock.Pos.Y / 9 - 1) * 9;
                MaxY = MinY + 26;

                MinX = (withBlock.Pos.X / 9 - 1) * 9;
                MaxX = MinX + 26;

                withBlock.AreasInfo.MinX = Convert.ToInt16(MinX);
                withBlock.AreasInfo.MinY = Convert.ToInt16(MinY);
            }

            if (MinY < 1)
                MinY = 1;
            if (MinX < 1)
                MinX = 1;
            if (MaxY > 100)
                MaxY = 100;
            if (MaxX > 100)
                MaxX = 100;


            // Actualizamos!!!
            if (Declaraciones.mapInfo[withBlock.Pos.Map].NumUsers != 0)
            {
                var loopTo = MaxX;
                for (X = MinX; X <= loopTo; X++)
                {
                    var loopTo1 = MaxY;
                    for (Y = MinY; Y <= loopTo1; Y++)
                        if (Declaraciones.MapData[withBlock.Pos.Map, X, Y].UserIndex != 0)
                            NPCs.MakeNPCChar(false, ref Declaraciones.MapData[withBlock.Pos.Map, X, Y].UserIndex,
                                ref NpcIndex, withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y);
                }
            }

            // Precalculados :P
            TempInt = withBlock.Pos.X / 9;
            withBlock.AreasInfo.AreaReciveX = AreasRecive[TempInt];
            withBlock.AreasInfo.AreaPerteneceX = Convert.ToInt16(Math.Pow(2d, TempInt));

            TempInt = withBlock.Pos.Y / 9;
            withBlock.AreasInfo.AreaReciveY = AreasRecive[TempInt];
            withBlock.AreasInfo.AreaPerteneceY = Convert.ToInt16(Math.Pow(2d, TempInt));

            withBlock.AreasInfo.AreaID = AreasInfo[withBlock.Pos.X, withBlock.Pos.Y];
        }
    }

    public static void QuitarUser(short UserIndex, short Map)
    {
        // **************************************************************
        // Author: Lucio N. Tourrilhes (DuNga)
        // Last Modify Date: Unknow
        // 
        // **************************************************************
        int TempVal;
        int LoopC;

        // Search for the user
        var loopTo = ConnGroups[Map].CountEntrys;
        for (LoopC = 1; LoopC <= loopTo; LoopC++)
            if (ConnGroups[Map].UserEntrys[LoopC] == UserIndex)
                break;

        // Char not found
        if (LoopC > ConnGroups[Map].CountEntrys)
            return;

        // Remove from old map
        ConnGroups[Map].CountEntrys = ConnGroups[Map].CountEntrys - 1;
        TempVal = ConnGroups[Map].CountEntrys;

        // Move list back (shift elements from found index)
        int foundIndex = LoopC;
        var loopTo1 = TempVal;
        for (LoopC = foundIndex; LoopC <= loopTo1; LoopC++)
            ConnGroups[Map].UserEntrys[LoopC] = ConnGroups[Map].UserEntrys[LoopC + 1];

        if (TempVal > ConnGroups[Map].OptValue) // Nescesito Redim?
            // UPGRADE_WARNING: El límite inferior de la matriz ConnGroups(Map).UserEntrys ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            Array.Resize(ref ConnGroups[Map].UserEntrys, TempVal + 1);
    }

    public static void AgregarUser(short UserIndex, short Map, bool ButIndex = false)
    {
        // **************************************************************
        // Author: Lucio N. Tourrilhes (DuNga)
        // Last Modify Date: 04/01/2007
        // Modified by Juan Martín Sotuyo Dodero (Maraxus)
        // - Now the method checks for repetead users instead of trusting parameters.
        // - If the character is new to the map, update it
        // **************************************************************
        int TempVal;
        bool EsNuevo;
        int i;

        if (!General.MapaValido(Map))
            return;

        EsNuevo = true;

        // Prevent adding repeated users
        var loopTo = ConnGroups[Map].CountEntrys;
        for (i = 1; i <= loopTo; i++)
            if (ConnGroups[Map].UserEntrys[i] == UserIndex)
            {
                EsNuevo = false;
                break;
            }

        if (EsNuevo)
        {
            // Update map and connection groups data
            ConnGroups[Map].CountEntrys = ConnGroups[Map].CountEntrys + 1;
            TempVal = ConnGroups[Map].CountEntrys;

            if (TempVal > ConnGroups[Map].OptValue) // Nescesito Redim
                // UPGRADE_WARNING: El límite inferior de la matriz ConnGroups(Map).UserEntrys ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                Array.Resize(ref ConnGroups[Map].UserEntrys, TempVal + 1);

            ConnGroups[Map].UserEntrys[TempVal] = UserIndex;
        }

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Update user
            withBlock.AreasInfo.AreaID = 0;

            withBlock.AreasInfo.AreaPerteneceX = 0;
            withBlock.AreasInfo.AreaPerteneceY = 0;
            withBlock.AreasInfo.AreaReciveX = 0;
            withBlock.AreasInfo.AreaReciveY = 0;
        }

        CheckUpdateNeededUser(UserIndex, USER_NUEVO, ButIndex);
    }

    public static void AgregarNpc(short NpcIndex)
    {
        // **************************************************************
        // Author: Lucio N. Tourrilhes (DuNga)
        // Last Modify Date: Unknow
        // 
        // **************************************************************
        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
            withBlock.AreasInfo.AreaID = 0;

            withBlock.AreasInfo.AreaPerteneceX = 0;
            withBlock.AreasInfo.AreaPerteneceY = 0;
            withBlock.AreasInfo.AreaReciveX = 0;
            withBlock.AreasInfo.AreaReciveY = 0;
        }

        CheckUpdateNeededNpc(NpcIndex, USER_NUEVO);
    }
    // **************************************************************
    // ModAreas.bas - Module to allow the usage of areas instead of maps.
    // Saves a lot of bandwidth.
    // 
    // Original Idea by Juan Martín Sotuyo Dodero (Maraxus)
    // (juansotuyo@gmail.com)
    // Implemented by Lucio N. Tourrilhes (DuNga)
    // **************************************************************

    // Modulo de envio por areas compatible con la versión 9.10.x ... By DuNga

    // >>>>>>AREAS>>>>>AREAS>>>>>>>>AREAS>>>>>>>AREAS>>>>>>>>>>
    public struct AreaInfo
    {
        public short AreaPerteneceX;
        public short AreaPerteneceY;
        public short AreaReciveX;
        public short AreaReciveY;
        public short MinX; // -!!!
        public short MinY; // -!!!
        public int AreaID;
    }

    public struct ConnGroup
    {
        public int CountEntrys;
        public int OptValue;
        public int[] UserEntrys;
    }
}