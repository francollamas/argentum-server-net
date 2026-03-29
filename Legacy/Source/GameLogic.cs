using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;

namespace Legacy
{

    static class Extra
    {
        public static bool EsNewbie(short UserIndex)
        {
            bool EsNewbieRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            EsNewbieRet = Declaraciones.UserList[UserIndex].Stats.ELV <= Declaraciones.LimiteNewbie;
            return EsNewbieRet;
        }

        public static bool esArmada(short UserIndex)
        {
            bool esArmadaRet = default;
            // ***************************************************
            // Autor: Pablo (ToxicWaste)
            // Last Modification: 23/01/2007
            // ***************************************************

            esArmadaRet = Declaraciones.UserList[UserIndex].Faccion.ArmadaReal == 1;
            return esArmadaRet;
        }

        public static bool esCaos(short UserIndex)
        {
            bool esCaosRet = default;
            // ***************************************************
            // Autor: Pablo (ToxicWaste)
            // Last Modification: 23/01/2007
            // ***************************************************

            esCaosRet = Declaraciones.UserList[UserIndex].Faccion.FuerzasCaos == 1;
            return esCaosRet;
        }

        public static bool EsGM(short UserIndex)
        {
            bool EsGMRet = default;
            // ***************************************************
            // Autor: Pablo (ToxicWaste)
            // Last Modification: 23/01/2007
            // ***************************************************

            EsGMRet = (Declaraciones.UserList[UserIndex].flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.Consejero)) != 0;
            return EsGMRet;
        }

        public static void DoTileEvents(short UserIndex, short Map, short X, short Y)
        {
            // ***************************************************
            // Autor: Pablo (ToxicWaste) & Unknown (orginal version)
            // Last Modification: 06/03/2010
            // Handles the Map passage of Users. Allows the existance
            // of exclusive maps for Newbies, Royal Army and Caos Legion members
            // and enables GMs to enter every map without restriction.
            // Uses: Mapinfo(map).Restringir = "NEWBIE" (newbies), "ARMADA", "CAOS", "FACCION" or "NO".
            // 06/03/2010 : Now we have 5 attemps to not fall into a map change or another teleport while going into a teleport. (Marco)
            // ***************************************************

            var nPos = default(Declaraciones.WorldPos);
            var FxFlag = default(bool);
            var TelepRadio = default(short);
            Declaraciones.WorldPos DestPos;

            try
            {
                // Controla las salidas
                var attemps = default(int);
                bool exitMap;
                short aN;
                if (InMapBounds(Map, X, Y))
                {
                    {
                        ref var withBlock = ref Declaraciones.MapData[Map, X, Y];
                        if (withBlock.ObjInfo.ObjIndex > 0)
                        {
                            FxFlag = Declaraciones.ObjData_Renamed[withBlock.ObjInfo.ObjIndex].OBJType == Declaraciones.eOBJType.otTeleport;
                            TelepRadio = Declaraciones.ObjData_Renamed[withBlock.ObjInfo.ObjIndex].Radio;
                        }

                        if (withBlock.TileExit.Map > 0 & withBlock.TileExit.Map <= Declaraciones.NumMaps)
                        {

                            // Es un teleport, entra en una posicion random, acorde al radio (si es 0, es pos fija)
                            // We have 5 attempts to not falling into another teleport or a map exit.. If we get to the fifth attemp,
                            // the teleport will act as if its radius = 0.
                            if (FxFlag & TelepRadio > 0)
                            {
                                do
                                {
                                    DestPos.X = (short)(withBlock.TileExit.X + Convert.ToInt16(Matematicas.RandomNumber(TelepRadio * -1, TelepRadio)));
                                    DestPos.Y = (short)(withBlock.TileExit.Y + Convert.ToInt16(Matematicas.RandomNumber(TelepRadio * -1, TelepRadio)));

                                    attemps = attemps + 1;

                                    exitMap = Declaraciones.MapData[withBlock.TileExit.Map, DestPos.X, DestPos.Y].TileExit.Map > 0 & Declaraciones.MapData[withBlock.TileExit.Map, DestPos.X, DestPos.Y].TileExit.Map <= Declaraciones.NumMaps;
                                }
                                while (!(attemps >= 5 | exitMap == false));

                                if (attemps >= 5)
                                {
                                    DestPos.X = withBlock.TileExit.X;
                                    DestPos.Y = withBlock.TileExit.Y;
                                }
                            }
                            // Posicion fija
                            else
                            {
                                DestPos.X = withBlock.TileExit.X;
                                DestPos.Y = withBlock.TileExit.Y;
                            }

                            DestPos.Map = withBlock.TileExit.Map;

                            // ¿Es mapa de newbies?
                            if (Declaraciones.MapInfo_Renamed[DestPos.Map].Restringir.ToUpper() == "NEWBIE")
                            {
                                // ¿El usuario es un newbie?
                                if (EsNewbie(UserIndex) | EsGM(UserIndex))
                                {
                                    if (LegalPos(DestPos.Map, DestPos.X, DestPos.Y, UsUaRiOs.PuedeAtravesarAgua(UserIndex)))
                                    {
                                        UsUaRiOs.WarpUserChar(UserIndex, DestPos.Map, DestPos.X, DestPos.Y, FxFlag);
                                    }
                                    else
                                    {
                                        bool argPuedeAgua = false;
                                        bool argPuedeTierra = true;
                                        ClosestLegalPos(ref DestPos, ref nPos, PuedeAgua: ref argPuedeAgua, PuedeTierra: ref argPuedeTierra);
                                        if (nPos.X != 0 & nPos.Y != 0)
                                        {
                                            UsUaRiOs.WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag);
                                        }
                                    }
                                }
                                else // No es newbie
                                {
                                    Protocol.WriteConsoleMsg(UserIndex, "Mapa exclusivo para newbies.", Protocol.FontTypeNames.FONTTYPE_INFO);
                                    ClosestStablePos(ref Declaraciones.UserList[UserIndex].Pos, ref nPos);

                                    if (nPos.X != 0 & nPos.Y != 0)
                                    {
                                        UsUaRiOs.WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, false);
                                    }
                                }
                            }
                            else if (Declaraciones.MapInfo_Renamed[DestPos.Map].Restringir.ToUpper() == "ARMADA") // ¿Es mapa de Armadas?
                            {
                                // ¿El usuario es Armada?
                                if (esArmada(UserIndex) | EsGM(UserIndex))
                                {
                                    if (LegalPos(DestPos.Map, DestPos.X, DestPos.Y, UsUaRiOs.PuedeAtravesarAgua(UserIndex)))
                                    {
                                        UsUaRiOs.WarpUserChar(UserIndex, DestPos.Map, DestPos.X, DestPos.Y, FxFlag);
                                    }
                                    else
                                    {
                                        bool argPuedeAgua2 = false;
                                        bool argPuedeTierra2 = true;
                                        ClosestLegalPos(ref DestPos, ref nPos, PuedeAgua: ref argPuedeAgua2, PuedeTierra: ref argPuedeTierra2);
                                        if (nPos.X != 0 & nPos.Y != 0)
                                        {
                                            UsUaRiOs.WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag);
                                        }
                                    }
                                }
                                else // No es armada
                                {
                                    Protocol.WriteConsoleMsg(UserIndex, "Mapa exclusivo para miembros del ejército real.", Protocol.FontTypeNames.FONTTYPE_INFO);
                                    ClosestStablePos(ref Declaraciones.UserList[UserIndex].Pos, ref nPos);

                                    if (nPos.X != 0 & nPos.Y != 0)
                                    {
                                        UsUaRiOs.WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag);
                                    }
                                }
                            }
                            else if (Declaraciones.MapInfo_Renamed[DestPos.Map].Restringir.ToUpper() == "CAOS") // ¿Es mapa de Caos?
                            {
                                // ¿El usuario es Caos?
                                if (esCaos(UserIndex) | EsGM(UserIndex))
                                {
                                    if (LegalPos(DestPos.Map, DestPos.X, DestPos.Y, UsUaRiOs.PuedeAtravesarAgua(UserIndex)))
                                    {
                                        UsUaRiOs.WarpUserChar(UserIndex, DestPos.Map, DestPos.X, DestPos.Y, FxFlag);
                                    }
                                    else
                                    {
                                        bool argPuedeAgua3 = false;
                                        bool argPuedeTierra3 = true;
                                        ClosestLegalPos(ref DestPos, ref nPos, PuedeAgua: ref argPuedeAgua3, PuedeTierra: ref argPuedeTierra3);
                                        if (nPos.X != 0 & nPos.Y != 0)
                                        {
                                            UsUaRiOs.WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag);
                                        }
                                    }
                                }
                                else // No es caos
                                {
                                    Protocol.WriteConsoleMsg(UserIndex, "Mapa exclusivo para miembros de la legión oscura.", Protocol.FontTypeNames.FONTTYPE_INFO);
                                    ClosestStablePos(ref Declaraciones.UserList[UserIndex].Pos, ref nPos);

                                    if (nPos.X != 0 & nPos.Y != 0)
                                    {
                                        UsUaRiOs.WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag);
                                    }
                                }
                            }
                            else if (Declaraciones.MapInfo_Renamed[DestPos.Map].Restringir.ToUpper() == "FACCION")
                            {
                                // ¿Es mapa de faccionarios?
                                // ¿El usuario es Armada o Caos?
                                if (esArmada(UserIndex) | esCaos(UserIndex) | EsGM(UserIndex))
                                {
                                    if (LegalPos(DestPos.Map, DestPos.X, DestPos.Y, UsUaRiOs.PuedeAtravesarAgua(UserIndex)))
                                    {
                                        UsUaRiOs.WarpUserChar(UserIndex, DestPos.Map, DestPos.X, DestPos.Y, FxFlag);
                                    }
                                    else
                                    {
                                        bool argPuedeAgua4 = false;
                                        bool argPuedeTierra4 = true;
                                        ClosestLegalPos(ref DestPos, ref nPos, PuedeAgua: ref argPuedeAgua4, PuedeTierra: ref argPuedeTierra4);
                                        if (nPos.X != 0 & nPos.Y != 0)
                                        {
                                            UsUaRiOs.WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag);
                                        }
                                    }
                                }
                                else // No es Faccionario
                                {
                                    Protocol.WriteConsoleMsg(UserIndex, "Solo se permite entrar al mapa si eres miembro de alguna facción.", Protocol.FontTypeNames.FONTTYPE_INFO);
                                    ClosestStablePos(ref Declaraciones.UserList[UserIndex].Pos, ref nPos);

                                    if (nPos.X != 0 & nPos.Y != 0)
                                    {
                                        UsUaRiOs.WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag);
                                    }
                                }
                            }
                            else if (LegalPos(DestPos.Map, DestPos.X, DestPos.Y, UsUaRiOs.PuedeAtravesarAgua(UserIndex))) // No es un mapa de newbies, ni Armadas, ni Caos, ni faccionario.
                            {
                                UsUaRiOs.WarpUserChar(UserIndex, DestPos.Map, DestPos.X, DestPos.Y, FxFlag);
                            }
                            else
                            {
                                bool argPuedeAgua1 = false;
                                bool argPuedeTierra1 = true;
                                ClosestLegalPos(ref DestPos, ref nPos, PuedeAgua: ref argPuedeAgua1, PuedeTierra: ref argPuedeTierra1);
                                if (nPos.X != 0 & nPos.Y != 0)
                                {
                                    UsUaRiOs.WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag);
                                }
                            }

                            // Te fusite del mapa. La criatura ya no es más tuya ni te reconoce como que vos la atacaste.

                            aN = Declaraciones.UserList[UserIndex].flags.AtacadoPorNpc;
                            if (aN > 0)
                            {
                                Declaraciones.Npclist[aN].Movement = Declaraciones.Npclist[aN].flags.OldMovement;
                                Declaraciones.Npclist[aN].Hostile = Declaraciones.Npclist[aN].flags.OldHostil;
                                Declaraciones.Npclist[aN].flags.AttackedBy = Constants.vbNullString;
                            }

                            aN = Declaraciones.UserList[UserIndex].flags.NPCAtacado;
                            if (aN > 0)
                            {
                                if ((Declaraciones.Npclist[aN].flags.AttackedFirstBy ?? "") == (Declaraciones.UserList[UserIndex].name ?? ""))
                                {
                                    Declaraciones.Npclist[aN].flags.AttackedFirstBy = Constants.vbNullString;
                                }
                            }
                            Declaraciones.UserList[UserIndex].flags.AtacadoPorNpc = 0;
                            Declaraciones.UserList[UserIndex].flags.NPCAtacado = 0;
                        }
                    }
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in EsNewbie: " + ex.Message);
                string argdesc = "Error en DotileEvents. Error: " + ex.GetType().Name + " - Desc: " + ex.Message;
                General.LogError(ref argdesc);
            }
        }

        public static bool InRangoVision(short UserIndex, short X, short Y)
        {
            bool InRangoVisionRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            if (X > (short)(Declaraciones.UserList[UserIndex].Pos.X - Declaraciones.MinXBorder) & X < (short)(Declaraciones.UserList[UserIndex].Pos.X + Declaraciones.MinXBorder))
            {
                if (Y > (short)(Declaraciones.UserList[UserIndex].Pos.Y - Declaraciones.MinYBorder) & Y < (short)(Declaraciones.UserList[UserIndex].Pos.Y + Declaraciones.MinYBorder))
                {
                    InRangoVisionRet = true;
                    return InRangoVisionRet;
                }
            }
            InRangoVisionRet = false;
            return InRangoVisionRet;
        }

        public static bool InRangoVisionNPC(short NpcIndex, ref short X, ref short Y)
        {
            bool InRangoVisionNPCRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            if (X > (short)(Declaraciones.Npclist[NpcIndex].Pos.X - Declaraciones.MinXBorder) & X < (short)(Declaraciones.Npclist[NpcIndex].Pos.X + Declaraciones.MinXBorder))
            {
                if (Y > (short)(Declaraciones.Npclist[NpcIndex].Pos.Y - Declaraciones.MinYBorder) & Y < (short)(Declaraciones.Npclist[NpcIndex].Pos.Y + Declaraciones.MinYBorder))
                {
                    InRangoVisionNPCRet = true;
                    return InRangoVisionNPCRet;
                }
            }
            InRangoVisionNPCRet = false;
            return InRangoVisionNPCRet;
        }


        public static bool InMapBounds(short Map, short X, short Y)
        {
            bool InMapBoundsRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            if (Map <= 0 | Map > Declaraciones.NumMaps | X < Declaraciones.MinXBorder | X > Declaraciones.MaxXBorder | Y < Declaraciones.MinYBorder | Y > Declaraciones.MaxYBorder)
            {
                InMapBoundsRet = false;
            }
            else
            {
                InMapBoundsRet = true;
            }

            return InMapBoundsRet;
        }

        public static void ClosestLegalPos(ref Declaraciones.WorldPos Pos, ref Declaraciones.WorldPos nPos, [Optional, DefaultParameterValue(false)] ref bool PuedeAgua, [Optional, DefaultParameterValue(true)] ref bool PuedeTierra)
        {
            // *****************************************************************
            // Author: Unknown (original version)
            // Last Modification: 24/01/2007 (ToxicWaste)
            // Encuentra la posicion legal mas cercana y la guarda en nPos
            // *****************************************************************

            var Notfound = default(bool);
            var LoopC = default(short);
            int tX;
            int tY;

            nPos.Map = Pos.Map;

            while (!LegalPos(Pos.Map, nPos.X, nPos.Y, PuedeAgua, PuedeTierra))
            {
                if (LoopC > 12)
                {
                    Notfound = true;
                    break;
                }

                var loopTo = Pos.Y + LoopC;
                for (tY = Pos.Y - LoopC; tY <= loopTo; tY++)
                {
                    var loopTo1 = Pos.X + LoopC;
                    for (tX = Pos.X - LoopC; tX <= loopTo1; tX++)
                    {

                        if (LegalPos(nPos.Map, Convert.ToInt16(tX), Convert.ToInt16(tY), PuedeAgua, PuedeTierra))
                        {
                            nPos.X = Convert.ToInt16(tX);
                            nPos.Y = Convert.ToInt16(tY);
                            // ¿Hay objeto?

                            tX = Convert.ToInt16((short)(Pos.X + LoopC));
                            tY = Convert.ToInt16((short)(Pos.Y + LoopC));
                        }
                    }
                }

                LoopC = Convert.ToInt16(LoopC + 1);
            }

            if (Notfound == true)
            {
                nPos.X = 0;
                nPos.Y = 0;
            }
        }

        private static void ClosestStablePos(ref Declaraciones.WorldPos Pos, ref Declaraciones.WorldPos nPos)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // Encuentra la posicion legal mas cercana que no sea un portal y la guarda en nPos
            // *****************************************************************

            var Notfound = default(bool);
            var LoopC = default(short);
            int tX;
            int tY;

            nPos.Map = Pos.Map;

            while (!LegalPos(Pos.Map, nPos.X, nPos.Y))
            {
                if (LoopC > 12)
                {
                    Notfound = true;
                    break;
                }

                var loopTo = Pos.Y + LoopC;
                for (tY = Pos.Y - LoopC; tY <= loopTo; tY++)
                {
                    var loopTo1 = Pos.X + LoopC;
                    for (tX = Pos.X - LoopC; tX <= loopTo1; tX++)
                    {

                        if (LegalPos(nPos.Map, Convert.ToInt16(tX), Convert.ToInt16(tY)) & Declaraciones.MapData[nPos.Map, Convert.ToInt16(tX), Convert.ToInt16(tY)].TileExit.Map == 0)
                        {
                            nPos.X = Convert.ToInt16(tX);
                            nPos.Y = Convert.ToInt16(tY);
                            // ¿Hay objeto?

                            tX = Convert.ToInt16((short)(Pos.X + LoopC));
                            tY = Convert.ToInt16((short)(Pos.Y + LoopC));
                        }
                    }
                }

                LoopC = Convert.ToInt16(LoopC + 1);
            }

            if (Notfound == true)
            {
                nPos.X = 0;
                nPos.Y = 0;
            }
        }

        public static short NameIndex(string name)
        {
            short NameIndexRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            int UserIndex;

            // ¿Nombre valido?
            if (Migration.migr_LenB(name) == 0)
            {
                NameIndexRet = 0;
                return NameIndexRet;
            }

            if (Migration.migr_InStrB(name, "+") != 0)
            {
                name = name.Replace("+", " ").ToUpper();
            }

            UserIndex = 1;
            while ((Declaraciones.UserList[UserIndex].name.ToUpper() ?? "") != (name.ToUpper() ?? ""))
            {

                UserIndex = UserIndex + 1;

                if (UserIndex > Declaraciones.MaxUsers)
                {
                    NameIndexRet = 0;
                    return NameIndexRet;
                }
            }

            NameIndexRet = Convert.ToInt16(UserIndex);
            return NameIndexRet;
        }

        public static bool CheckForSameIP(short UserIndex, string UserIP)
        {
            bool CheckForSameIPRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            int LoopC;

            var loopTo = (int)Declaraciones.MaxUsers;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                if (Declaraciones.UserList[LoopC].flags.UserLogged == true)
                {
                    if ((Declaraciones.UserList[LoopC].ip ?? "") == (UserIP ?? "") & UserIndex != LoopC)
                    {
                        CheckForSameIPRet = true;
                        return CheckForSameIPRet;
                    }
                }
            }

            CheckForSameIPRet = false;
            return CheckForSameIPRet;
        }

        public static bool CheckForSameName(string name)
        {
            bool CheckForSameNameRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            // Controlo que no existan usuarios con el mismo nombre
            int LoopC;

            var loopTo = (int)Declaraciones.LastUser;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                if (Declaraciones.UserList[LoopC].flags.UserLogged)
                {

                    // If UCase$(UserList(LoopC).Name) = UCase$(Name) And UserList(LoopC).ConnID <> -1 Then
                    // OJO PREGUNTAR POR EL CONNID <> -1 PRODUCE QUE UN PJ EN DETERMINADO
                    // MOMENTO PUEDA ESTAR LOGUEADO 2 VECES (IE: CIERRA EL SOCKET DESDE ALLA)
                    // ESE EVENTO NO DISPARA UN SAVE USER, LO QUE PUEDE SER UTILIZADO PARA DUPLICAR ITEMS
                    // ESTE BUG EN ALKON PRODUJO QUE EL SERVIDOR ESTE CAIDO DURANTE 3 DIAS. ATENTOS.

                    if ((Declaraciones.UserList[LoopC].name.ToUpper() ?? "") == (name.ToUpper() ?? ""))
                    {
                        CheckForSameNameRet = true;
                        return CheckForSameNameRet;
                    }
                }
            }

            CheckForSameNameRet = false;
            return CheckForSameNameRet;
        }

        public static void HeadtoPos(Declaraciones.eHeading Head, ref Declaraciones.WorldPos Pos)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // Toma una posicion y se mueve hacia donde esta perfilado
            // *****************************************************************

            switch (Head)
            {
                case Declaraciones.eHeading.NORTH:
                    {
                        Pos.Y = Convert.ToInt16(Pos.Y - 1);
                        break;
                    }

                case Declaraciones.eHeading.SOUTH:
                    {
                        Pos.Y = Convert.ToInt16(Pos.Y + 1);
                        break;
                    }

                case Declaraciones.eHeading.EAST:
                    {
                        Pos.X = Convert.ToInt16(Pos.X + 1);
                        break;
                    }

                case Declaraciones.eHeading.WEST:
                    {
                        Pos.X = Convert.ToInt16(Pos.X - 1);
                        break;
                    }
            }
        }

        public static bool LegalPos(short Map, short X, short Y, bool PuedeAgua = false, bool PuedeTierra = true)
        {
            bool LegalPosRet = default;
            // ***************************************************
            // Autor: Pablo (ToxicWaste) & Unknown (orginal version)
            // Last Modification: 23/01/2007
            // Checks if the position is Legal.
            // ***************************************************

            // ¿Es un mapa valido?
            if (Map <= 0 | Map > Declaraciones.NumMaps | X < Declaraciones.MinXBorder | X > Declaraciones.MaxXBorder | Y < Declaraciones.MinYBorder | Y > Declaraciones.MaxYBorder)
            {
                LegalPosRet = false;
            }
            else
            {
                {
                    ref var withBlock = ref Declaraciones.MapData[Map, X, Y];
                    if (PuedeAgua & PuedeTierra)
                    {
                        LegalPosRet = withBlock.Blocked != 1 & withBlock.UserIndex == 0 & withBlock.NpcIndex == 0;
                    }
                    else if (PuedeTierra & !PuedeAgua)
                    {
                        LegalPosRet = withBlock.Blocked != 1 & withBlock.UserIndex == 0 & withBlock.NpcIndex == 0 & !General.HayAgua(Map, X, Y);
                    }
                    else if (PuedeAgua & !PuedeTierra)
                    {
                        LegalPosRet = withBlock.Blocked != 1 & withBlock.UserIndex == 0 & withBlock.NpcIndex == 0 & General.HayAgua(Map, X, Y);
                    }
                    else
                    {
                        LegalPosRet = false;
                    }
                }
            }

            return LegalPosRet;
        }

        public static bool MoveToLegalPos(short Map, short X, short Y, bool PuedeAgua = false, bool PuedeTierra = true)
        {
            bool MoveToLegalPosRet = default;
            // ***************************************************
            // Autor: ZaMa
            // Last Modification: 13/07/2009
            // Checks if the position is Legal, but considers that if there's a casper, it's a legal movement.
            // 13/07/2009: ZaMa - Now it's also legal move where an invisible admin is.
            // ***************************************************

            short UserIndex;
            bool IsDeadChar;
            bool IsAdminInvisible;


            // ¿Es un mapa valido?
            if (Map <= 0 | Map > Declaraciones.NumMaps | X < Declaraciones.MinXBorder | X > Declaraciones.MaxXBorder | Y < Declaraciones.MinYBorder | Y > Declaraciones.MaxYBorder)
            {
                MoveToLegalPosRet = false;
            }
            else
            {
                {
                    ref var withBlock = ref Declaraciones.MapData[Map, X, Y];
                    UserIndex = withBlock.UserIndex;

                    if (UserIndex > 0)
                    {
                        IsDeadChar = Declaraciones.UserList[UserIndex].flags.Muerto == 1;
                        IsAdminInvisible = Declaraciones.UserList[UserIndex].flags.AdminInvisible == 1;
                    }
                    else
                    {
                        IsDeadChar = false;
                        IsAdminInvisible = false;
                    }

                    if (PuedeAgua & PuedeTierra)
                    {
                        MoveToLegalPosRet = withBlock.Blocked != 1 & (UserIndex == 0 | IsDeadChar | IsAdminInvisible) & withBlock.NpcIndex == 0;
                    }
                    else if (PuedeTierra & !PuedeAgua)
                    {
                        MoveToLegalPosRet = withBlock.Blocked != 1 & (UserIndex == 0 | IsDeadChar | IsAdminInvisible) & withBlock.NpcIndex == 0 & !General.HayAgua(Map, X, Y);
                    }
                    else if (PuedeAgua & !PuedeTierra)
                    {
                        MoveToLegalPosRet = withBlock.Blocked != 1 & (UserIndex == 0 | IsDeadChar | IsAdminInvisible) & withBlock.NpcIndex == 0 & General.HayAgua(Map, X, Y);
                    }
                    else
                    {
                        MoveToLegalPosRet = false;
                    }
                }
            }

            return MoveToLegalPosRet;
        }

        public static void FindLegalPos(short UserIndex, short Map, ref short X, ref short Y)
        {
            // ***************************************************
            // Autor: ZaMa
            // Last Modification: 26/03/2009
            // Search for a Legal pos for the user who is being teleported.
            // ***************************************************

            var FoundPlace = default(bool);
            var tX = default(int);
            var tY = default(int);
            int Rango;
            short OtherUserIndex;
            if (Declaraciones.MapData[Map, X, Y].UserIndex != 0 | Declaraciones.MapData[Map, X, Y].NpcIndex != 0)
            {

                // Se teletransporta a la misma pos a la que estaba
                if (Declaraciones.MapData[Map, X, Y].UserIndex == UserIndex)
                    return;


                for (Rango = 1; Rango <= 5; Rango++)
                {
                    var loopTo = Y + Rango;
                    for (tY = Y - Rango; tY <= loopTo; tY++)
                    {
                        var loopTo1 = X + Rango;
                        for (tX = X - Rango; tX <= loopTo1; tX++)
                        {
                            // Reviso que no haya User ni NPC
                            if (Declaraciones.MapData[Map, Convert.ToInt16(tX), Convert.ToInt16(tY)].UserIndex == 0 & Declaraciones.MapData[Map, Convert.ToInt16(tX), Convert.ToInt16(tY)].NpcIndex == 0)
                            {

                                if (InMapBounds(Map, Convert.ToInt16(tX), Convert.ToInt16(tY)))
                                    FoundPlace = true;

                                break;
                            }

                        }

                        if (FoundPlace)
                            break;
                    }

                    if (FoundPlace)
                        break;
                }


                if (FoundPlace) // Si encontramos un lugar, listo, nos quedamos ahi
                {
                    X = Convert.ToInt16(tX);
                    Y = Convert.ToInt16(tY);
                }
                else
                {
                    // Muy poco probable, pero..
                    // Si no encontramos un lugar, sacamos al usuario que tenemos abajo, y si es un NPC, lo pisamos.
                    OtherUserIndex = Declaraciones.MapData[Map, X, Y].UserIndex;
                    if (OtherUserIndex != 0)
                    {
                        // Si no encontramos lugar, y abajo teniamos a un usuario, lo pisamos y cerramos su comercio seguro
                        if (Declaraciones.UserList[OtherUserIndex].ComUsu.DestUsu > 0)
                        {
                            // Le avisamos al que estaba comerciando que se tuvo que ir.
                            if (Declaraciones.UserList[Declaraciones.UserList[OtherUserIndex].ComUsu.DestUsu].flags.UserLogged)
                            {
                                mdlCOmercioConUsuario.FinComerciarUsu(Declaraciones.UserList[OtherUserIndex].ComUsu.DestUsu);
                                Protocol.WriteConsoleMsg(Declaraciones.UserList[OtherUserIndex].ComUsu.DestUsu, "Comercio cancelado. El otro usuario se ha desconectado.", Protocol.FontTypeNames.FONTTYPE_TALK);
                                Protocol.FlushBuffer(Declaraciones.UserList[OtherUserIndex].ComUsu.DestUsu);
                            }
                            // Lo sacamos.
                            if (Declaraciones.UserList[OtherUserIndex].flags.UserLogged)
                            {
                                mdlCOmercioConUsuario.FinComerciarUsu(OtherUserIndex);
                                Protocol.WriteErrorMsg(OtherUserIndex, "Alguien se ha conectado donde te encontrabas, por favor reconéctate...");
                                Protocol.FlushBuffer(OtherUserIndex);
                            }
                        }

                        TCP.CloseSocket(OtherUserIndex);
                    }
                }
            }
        }

        public static bool LegalPosNPC(short Map, short X, short Y, byte AguaValida, bool IsPet = false)
        {
            bool LegalPosNPCRet = default;
            // ***************************************************
            // Autor: Unkwnown
            // Last Modification: 09/23/2009
            // Checks if it's a Legal pos for the npc to move to.
            // 09/23/2009: Pato - If UserIndex is a AdminInvisible, then is a legal pos.
            // ***************************************************
            bool IsDeadChar;
            short UserIndex;
            bool IsAdminInvisible;


            if (Map <= 0 | Map > Declaraciones.NumMaps | X < Declaraciones.MinXBorder | X > Declaraciones.MaxXBorder | Y < Declaraciones.MinYBorder | Y > Declaraciones.MaxYBorder)
            {
                LegalPosNPCRet = false;
                return LegalPosNPCRet;
            }

            {
                ref var withBlock = ref Declaraciones.MapData[Map, X, Y];
                UserIndex = withBlock.UserIndex;
                if (UserIndex > 0)
                {
                    IsDeadChar = Declaraciones.UserList[UserIndex].flags.Muerto == 1;
                    IsAdminInvisible = Declaraciones.UserList[UserIndex].flags.AdminInvisible == 1;
                }
                else
                {
                    IsDeadChar = false;
                    IsAdminInvisible = false;
                }

                if (AguaValida == 0)
                {
                    LegalPosNPCRet = withBlock.Blocked != 1 & (withBlock.UserIndex == 0 | IsDeadChar | IsAdminInvisible) & withBlock.NpcIndex == 0 & (withBlock.trigger != Declaraciones.eTrigger.POSINVALIDA | IsPet) & !General.HayAgua(Map, X, Y);
                }
                else
                {
                    LegalPosNPCRet = withBlock.Blocked != 1 & (withBlock.UserIndex == 0 | IsDeadChar | IsAdminInvisible) & withBlock.NpcIndex == 0 & (withBlock.trigger != Declaraciones.eTrigger.POSINVALIDA | IsPet);
                }
            }

            return LegalPosNPCRet;
        }

        public static void SendHelp(short Index)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short NumHelpLines;
            short LoopC;

            int argEmptySpaces = 1024;
            NumHelpLines = Convert.ToInt16(Migration.ParseVal(ES.GetVar(Declaraciones.DatPath + "Help.dat", "INIT", "NumLines", EmptySpaces: ref argEmptySpaces)));

            var loopTo = NumHelpLines;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                int argEmptySpaces1 = 1024;
                Protocol.WriteConsoleMsg(Index, ES.GetVar(Declaraciones.DatPath + "Help.dat", "Help", "Line" + LoopC, EmptySpaces: ref argEmptySpaces1), Protocol.FontTypeNames.FONTTYPE_INFO);
            }
        }

        public static void Expresar(short NpcIndex, short UserIndex)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short randomi;
            if (Declaraciones.Npclist[NpcIndex].NroExpresiones > 0)
            {
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto randomi. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                randomi = Convert.ToInt16(Matematicas.RandomNumber(1, Declaraciones.Npclist[NpcIndex].NroExpresiones));
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto randomi. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex, Protocol.PrepareMessageChatOverHead(Declaraciones.Npclist[NpcIndex].Expresiones[randomi], Declaraciones.Npclist[NpcIndex].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.White)));
            }
        }

        public static void LookatTile(short UserIndex, short Map, short X, short Y)
        {
            // ***************************************************
            // Autor: Unknown (orginal version)
            // Last Modification: 26/03/2009
            // 13/02/2009: ZaMa - EL nombre del gm que aparece por consola al clickearlo, tiene el color correspondiente a su rango
            // ***************************************************

            try
            {

                // Responde al click del usuario sobre el mapa
                var FoundChar = default(byte);
                var FoundSomething = default(byte);
                var TempCharIndex = default(short);
                var Stat = default(string);
                var ft = default(Protocol.FontTypeNames);

                var estatus = default(string);
                int MinHp;
                int MaxHp;
                byte SupervivenciaSkill;
                string sDesc;
                {
                    ref var withBlock = ref Declaraciones.UserList[UserIndex];
                    // ¿Rango Visión? (ToxicWaste)
                    if (Math.Abs((short)(withBlock.Pos.Y - Y)) > AI.RANGO_VISION_Y | Math.Abs((short)(withBlock.Pos.X - X)) > AI.RANGO_VISION_X)
                    {
                        return;
                    }

                    // ¿Posicion valida?
                    if (InMapBounds(Map, X, Y))
                    {
                        {
                            ref var withBlock1 = ref withBlock.flags;
                            withBlock1.TargetMap = Map;
                            withBlock1.TargetX = X;
                            withBlock1.TargetY = Y;
                            // ¿Es un obj?
                            if (Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex > 0)
                            {
                                // Informa el nombre
                                withBlock1.TargetObjMap = Map;
                                withBlock1.TargetObjX = X;
                                withBlock1.TargetObjY = Y;
                                FoundSomething = 1;
                            }
                            else if (Declaraciones.MapData[Map, X + 1, Y].ObjInfo.ObjIndex > 0)
                            {
                                // Informa el nombre
                                if (Declaraciones.ObjData_Renamed[Declaraciones.MapData[Map, X + 1, Y].ObjInfo.ObjIndex].OBJType == Declaraciones.eOBJType.otPuertas)
                                {
                                    withBlock1.TargetObjMap = Map;
                                    withBlock1.TargetObjX = Convert.ToInt16(X + 1);
                                    withBlock1.TargetObjY = Y;
                                    FoundSomething = 1;
                                }
                            }
                            else if (Declaraciones.MapData[Map, X + 1, Y + 1].ObjInfo.ObjIndex > 0)
                            {
                                if (Declaraciones.ObjData_Renamed[Declaraciones.MapData[Map, X + 1, Y + 1].ObjInfo.ObjIndex].OBJType == Declaraciones.eOBJType.otPuertas)
                                {
                                    // Informa el nombre
                                    withBlock1.TargetObjMap = Map;
                                    withBlock1.TargetObjX = Convert.ToInt16(X + 1);
                                    withBlock1.TargetObjY = Convert.ToInt16(Y + 1);
                                    FoundSomething = 1;
                                }
                            }
                            else if (Declaraciones.MapData[Map, X, Y + 1].ObjInfo.ObjIndex > 0)
                            {
                                if (Declaraciones.ObjData_Renamed[Declaraciones.MapData[Map, X, Y + 1].ObjInfo.ObjIndex].OBJType == Declaraciones.eOBJType.otPuertas)
                                {
                                    // Informa el nombre
                                    withBlock1.TargetObjMap = Map;
                                    withBlock1.TargetObjX = X;
                                    withBlock1.TargetObjY = Convert.ToInt16(Y + 1);
                                    FoundSomething = 1;
                                }
                            }

                            if (FoundSomething == 1)
                            {
                                withBlock1.TargetObj = Declaraciones.MapData[Map, withBlock1.TargetObjX, withBlock1.TargetObjY].ObjInfo.ObjIndex;
                                if (MostrarCantidad(withBlock1.TargetObj))
                                {
                                    Protocol.WriteConsoleMsg(UserIndex, Declaraciones.ObjData_Renamed[withBlock1.TargetObj].name + " - " + Declaraciones.MapData[withBlock1.TargetObjMap, withBlock1.TargetObjX, withBlock1.TargetObjY].ObjInfo.Amount + "", Protocol.FontTypeNames.FONTTYPE_INFO);
                                }
                                else
                                {
                                    Protocol.WriteConsoleMsg(UserIndex, Declaraciones.ObjData_Renamed[withBlock1.TargetObj].name, Protocol.FontTypeNames.FONTTYPE_INFO);
                                }

                            }
                            // ¿Es un personaje?
                            if (Y + 1 <= Declaraciones.YMaxMapSize)
                            {
                                if (Declaraciones.MapData[Map, X, Y + 1].UserIndex > 0)
                                {
                                    TempCharIndex = Declaraciones.MapData[Map, X, Y + 1].UserIndex;
                                    FoundChar = 1;
                                }
                                if (Declaraciones.MapData[Map, X, Y + 1].NpcIndex > 0)
                                {
                                    TempCharIndex = Declaraciones.MapData[Map, X, Y + 1].NpcIndex;
                                    FoundChar = 2;
                                }
                            }
                            // ¿Es un personaje?
                            if (FoundChar == 0)
                            {
                                if (Declaraciones.MapData[Map, X, Y].UserIndex > 0)
                                {
                                    TempCharIndex = Declaraciones.MapData[Map, X, Y].UserIndex;
                                    FoundChar = 1;
                                }
                                if (Declaraciones.MapData[Map, X, Y].NpcIndex > 0)
                                {
                                    TempCharIndex = Declaraciones.MapData[Map, X, Y].NpcIndex;
                                    FoundChar = 2;
                                }
                            }
                        }


                        // Reaccion al personaje
                        if (FoundChar == 1) // ¿Encontro un Usuario?
                        {
                            if (Declaraciones.UserList[TempCharIndex].flags.AdminInvisible == 0 | (withBlock.flags.Privilegios & Declaraciones.PlayerType.Dios) != 0)
                            {
                                {
                                    ref var withBlock2 = ref Declaraciones.UserList[TempCharIndex];
                                    if (Migration.migr_LenB(withBlock2.DescRM) == 0 & withBlock2.showName)
                                    {
                                        // No tiene descRM y quiere que se vea su nombre.
                                        if (EsNewbie(TempCharIndex))
                                        {
                                            Stat = " <NEWBIE>";
                                        }

                                        if (withBlock2.Faccion.ArmadaReal == 1)
                                        {
                                            Stat = Stat + " <Ejército Real> " + "<" + ModFacciones.TituloReal(TempCharIndex) + ">";
                                        }
                                        else if (withBlock2.Faccion.FuerzasCaos == 1)
                                        {
                                            Stat = Stat + " <Legión Oscura> " + "<" + ModFacciones.TituloCaos(TempCharIndex) + ">";
                                        }

                                        if (withBlock2.GuildIndex > 0)
                                        {
                                            Stat = Stat + " <" + modGuilds.GuildName(withBlock2.GuildIndex) + ">";
                                        }

                                        if (withBlock2.desc.Length > 0)
                                        {
                                            Stat = "Ves a " + withBlock2.name + Stat + " - " + withBlock2.desc;
                                        }
                                        else
                                        {
                                            Stat = "Ves a " + withBlock2.name + Stat;
                                        }


                                        if ((withBlock2.flags.Privilegios & Declaraciones.PlayerType.RoyalCouncil) != 0)
                                        {
                                            Stat = Stat + " [CONSEJO DE BANDERBILL]";
                                            ft = Protocol.FontTypeNames.FONTTYPE_CONSEJOVesA;
                                        }
                                        else if ((withBlock2.flags.Privilegios & Declaraciones.PlayerType.ChaosCouncil) != 0)
                                        {
                                            Stat = Stat + " [CONCILIO DE LAS SOMBRAS]";
                                            ft = Protocol.FontTypeNames.FONTTYPE_CONSEJOCAOSVesA;
                                        }
                                        else if ((withBlock2.flags.Privilegios & Declaraciones.PlayerType.User) == 0)
                                        {
                                            Stat = Stat + " <GAME MASTER>";

                                            // Elijo el color segun el rango del GM:
                                            // Dios
                                            if (withBlock2.flags.Privilegios == Declaraciones.PlayerType.Dios)
                                            {
                                                ft = Protocol.FontTypeNames.FONTTYPE_DIOS;
                                            }
                                            // Gm
                                            else if (withBlock2.flags.Privilegios == Declaraciones.PlayerType.SemiDios)
                                            {
                                                ft = Protocol.FontTypeNames.FONTTYPE_GM;
                                            }
                                            // Conse
                                            else if (withBlock2.flags.Privilegios == Declaraciones.PlayerType.Consejero)
                                            {
                                                // Rm o Dsrm
                                                ft = Protocol.FontTypeNames.FONTTYPE_CONSE;
                                            }
                                            else if (withBlock2.flags.Privilegios == (Declaraciones.PlayerType.RoleMaster | Declaraciones.PlayerType.Consejero) | withBlock2.flags.Privilegios == (Declaraciones.PlayerType.RoleMaster | Declaraciones.PlayerType.Dios))
                                            {
                                                ft = Protocol.FontTypeNames.FONTTYPE_EJECUCION;
                                            }
                                        }

                                        else if (ES.criminal(TempCharIndex))
                                        {
                                            Stat = Stat + " <CRIMINAL>";
                                            ft = Protocol.FontTypeNames.FONTTYPE_FIGHT;
                                        }
                                        else
                                        {
                                            Stat = Stat + " <CIUDADANO>";
                                            ft = Protocol.FontTypeNames.FONTTYPE_CITIZEN;
                                        }
                                    }
                                    else // Si tiene descRM la muestro siempre.
                                    {
                                        Stat = withBlock2.DescRM;
                                        ft = Protocol.FontTypeNames.FONTTYPE_INFOBOLD;
                                    }
                                }

                                if (Migration.migr_LenB(Stat) > 0)
                                {
                                    Protocol.WriteConsoleMsg(UserIndex, Stat, ft);
                                }

                                FoundSomething = 1;
                                withBlock.flags.TargetUser = TempCharIndex;
                                withBlock.flags.TargetNPC = 0;
                                withBlock.flags.TargetNpcTipo = Declaraciones.eNPCType.Comun;
                            }
                        }

                        {
                            ref var withBlock3 = ref withBlock.flags;
                            if (FoundChar == 2) // ¿Encontro un NPC?
                            {

                                MinHp = Declaraciones.Npclist[TempCharIndex].Stats.MinHp;
                                MaxHp = Declaraciones.Npclist[TempCharIndex].Stats.MaxHp;
                                SupervivenciaSkill = Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Supervivencia];

                                if ((withBlock3.Privilegios & (Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin)) != 0)
                                {
                                    estatus = "(" + MinHp + "/" + MaxHp + ") ";
                                }
                                else if (withBlock3.Muerto == 0)
                                {
                                    if (SupervivenciaSkill >= 0 & SupervivenciaSkill <= 10)
                                    {
                                        estatus = "(Dudoso) ";
                                    }
                                    else if (SupervivenciaSkill > 10 & SupervivenciaSkill <= 20)
                                    {
                                        if (MinHp < MaxHp / 2d)
                                        {
                                            estatus = "(Herido) ";
                                        }
                                        else
                                        {
                                            estatus = "(Sano) ";
                                        }
                                    }
                                    else if (SupervivenciaSkill > 20 & SupervivenciaSkill <= 30)
                                    {
                                        if (MinHp < MaxHp * 0.5d)
                                        {
                                            estatus = "(Malherido) ";
                                        }
                                        else if (MinHp < MaxHp * 0.75d)
                                        {
                                            estatus = "(Herido) ";
                                        }
                                        else
                                        {
                                            estatus = "(Sano) ";
                                        }
                                    }
                                    else if (SupervivenciaSkill > 30 & SupervivenciaSkill <= 40)
                                    {
                                        if (MinHp < MaxHp * 0.25d)
                                        {
                                            estatus = "(Muy malherido) ";
                                        }
                                        else if (MinHp < MaxHp * 0.5d)
                                        {
                                            estatus = "(Herido) ";
                                        }
                                        else if (MinHp < MaxHp * 0.75d)
                                        {
                                            estatus = "(Levemente herido) ";
                                        }
                                        else
                                        {
                                            estatus = "(Sano) ";
                                        }
                                    }
                                    else if (SupervivenciaSkill > 40 & SupervivenciaSkill < 60)
                                    {
                                        if (MinHp < MaxHp * 0.05d)
                                        {
                                            estatus = "(Agonizando) ";
                                        }
                                        else if (MinHp < MaxHp * 0.1d)
                                        {
                                            estatus = "(Casi muerto) ";
                                        }
                                        else if (MinHp < MaxHp * 0.25d)
                                        {
                                            estatus = "(Muy Malherido) ";
                                        }
                                        else if (MinHp < MaxHp * 0.5d)
                                        {
                                            estatus = "(Herido) ";
                                        }
                                        else if (MinHp < MaxHp * 0.75d)
                                        {
                                            estatus = "(Levemente herido) ";
                                        }
                                        else if (MinHp < MaxHp)
                                        {
                                            estatus = "(Sano) ";
                                        }
                                        else
                                        {
                                            estatus = "(Intacto) ";
                                        }
                                    }
                                    else if (SupervivenciaSkill >= 60)
                                    {
                                        estatus = "(" + MinHp + "/" + MaxHp + ") ";
                                    }
                                    else
                                    {
                                        estatus = "¡Error!";
                                    }
                                }

                                if (Declaraciones.Npclist[TempCharIndex].desc.Length > 1)
                                {
                                    Protocol.WriteChatOverHead(UserIndex, Declaraciones.Npclist[TempCharIndex].desc, Declaraciones.Npclist[TempCharIndex].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.White));
                                }
                                else if (TempCharIndex == modCentinela.CentinelaNPCIndex)
                                {
                                    // Enviamos nuevamente el texto del centinela según quien pregunta
                                    modCentinela.CentinelaSendClave(UserIndex);
                                }
                                else if (Declaraciones.Npclist[TempCharIndex].MaestroUser > 0)
                                {
                                    Protocol.WriteConsoleMsg(UserIndex, estatus + Declaraciones.Npclist[TempCharIndex].name + " es mascota de " + Declaraciones.UserList[Declaraciones.Npclist[TempCharIndex].MaestroUser].name + ".", Protocol.FontTypeNames.FONTTYPE_INFO);
                                }
                                else
                                {
                                    sDesc = estatus + Declaraciones.Npclist[TempCharIndex].name;
                                    if (Declaraciones.Npclist[TempCharIndex].Owner > 0)
                                        sDesc = sDesc + " le pertenece a " + Declaraciones.UserList[Declaraciones.Npclist[TempCharIndex].Owner].name;
                                    sDesc = sDesc + ".";

                                    Protocol.WriteConsoleMsg(UserIndex, sDesc, Protocol.FontTypeNames.FONTTYPE_INFO);

                                    if ((withBlock3.Privilegios & (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin)) != 0)
                                    {
                                        Protocol.WriteConsoleMsg(UserIndex, "Le pegó primero: " + Declaraciones.Npclist[TempCharIndex].flags.AttackedFirstBy + ".", Protocol.FontTypeNames.FONTTYPE_INFO);
                                    }
                                }

                                FoundSomething = 1;
                                withBlock3.TargetNpcTipo = Declaraciones.Npclist[TempCharIndex].NPCtype;
                                withBlock3.TargetNPC = TempCharIndex;
                                withBlock3.TargetUser = 0;
                                withBlock3.TargetObj = 0;
                            }

                            if (FoundChar == 0)
                            {
                                withBlock3.TargetNPC = 0;
                                withBlock3.TargetNpcTipo = Declaraciones.eNPCType.Comun;
                                withBlock3.TargetUser = 0;
                            }

                            // *** NO ENCOTRO NADA ***
                            if (FoundSomething == 0)
                            {
                                withBlock3.TargetNPC = 0;
                                withBlock3.TargetNpcTipo = Declaraciones.eNPCType.Comun;
                                withBlock3.TargetUser = 0;
                                withBlock3.TargetObj = 0;
                                withBlock3.TargetObjMap = 0;
                                withBlock3.TargetObjX = 0;
                                withBlock3.TargetObjY = 0;
                                Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.DontSeeAnything);
                            }
                        }
                    }
                    else if (FoundSomething == 0)
                    {
                        {
                            ref var withBlock4 = ref withBlock.flags;
                            withBlock4.TargetNPC = 0;
                            withBlock4.TargetNpcTipo = Declaraciones.eNPCType.Comun;
                            withBlock4.TargetUser = 0;
                            withBlock4.TargetObj = 0;
                            withBlock4.TargetObjMap = 0;
                            withBlock4.TargetObjX = 0;
                            withBlock4.TargetObjY = 0;
                        }

                        Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.DontSeeAnything);
                    }
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in InRangoVision: " + ex.Message);
                string argdesc = "Error en LookAtTile. Error " + ex.GetType().Name + " : " + ex.Message;
                General.LogError(ref argdesc);
            }
        }

        public static Declaraciones.eHeading FindDirection(ref Declaraciones.WorldPos Pos, ref Declaraciones.WorldPos Target)
        {
            Declaraciones.eHeading FindDirectionRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // Devuelve la direccion en la cual el target se encuentra
            // desde pos, 0 si la direc es igual
            // *****************************************************************

            short X;
            short Y;

            X = (short)(Pos.X - Target.X);
            Y = (short)(Pos.Y - Target.Y);

            // NE
            if (Math.Sign(X) == -1 & Math.Sign(Y) == 1)
            {
                FindDirectionRet = Matematicas.RandomNumber(0, 1) != 0 ? Declaraciones.eHeading.NORTH : Declaraciones.eHeading.EAST;
                return FindDirectionRet;
            }

            // NW
            if (Math.Sign(X) == 1 & Math.Sign(Y) == 1)
            {
                FindDirectionRet = Matematicas.RandomNumber(0, 1) != 0 ? Declaraciones.eHeading.WEST : Declaraciones.eHeading.NORTH;
                return FindDirectionRet;
            }

            // SW
            if (Math.Sign(X) == 1 & Math.Sign(Y) == -1)
            {
                FindDirectionRet = Matematicas.RandomNumber(0, 1) != 0 ? Declaraciones.eHeading.WEST : Declaraciones.eHeading.SOUTH;
                return FindDirectionRet;
            }

            // SE
            if (Math.Sign(X) == -1 & Math.Sign(Y) == -1)
            {
                FindDirectionRet = Matematicas.RandomNumber(0, 1) != 0 ? Declaraciones.eHeading.SOUTH : Declaraciones.eHeading.EAST;
                return FindDirectionRet;
            }

            // Sur
            if (Math.Sign(X) == 0 & Math.Sign(Y) == -1)
            {
                FindDirectionRet = Declaraciones.eHeading.SOUTH;
                return FindDirectionRet;
            }

            // norte
            if (Math.Sign(X) == 0 & Math.Sign(Y) == 1)
            {
                FindDirectionRet = Declaraciones.eHeading.NORTH;
                return FindDirectionRet;
            }

            // oeste
            if (Math.Sign(X) == 1 & Math.Sign(Y) == 0)
            {
                FindDirectionRet = Declaraciones.eHeading.WEST;
                return FindDirectionRet;
            }

            // este
            if (Math.Sign(X) == -1 & Math.Sign(Y) == 0)
            {
                FindDirectionRet = Declaraciones.eHeading.EAST;
                return FindDirectionRet;
            }

            // misma
            if (Math.Sign(X) == 0 & Math.Sign(Y) == 0)
            {
                FindDirectionRet = 0;
                return FindDirectionRet;
            }

            return FindDirectionRet;
        }

        public static bool ItemNoEsDeMapa(short Index, bool bIsExit)
        {
            bool ItemNoEsDeMapaRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            {
                ref var withBlock = ref Declaraciones.ObjData_Renamed[Index];
                ItemNoEsDeMapaRet = withBlock.OBJType != Declaraciones.eOBJType.otPuertas & withBlock.OBJType != Declaraciones.eOBJType.otForos & withBlock.OBJType != Declaraciones.eOBJType.otCarteles & withBlock.OBJType != Declaraciones.eOBJType.otArboles & withBlock.OBJType != Declaraciones.eOBJType.otYacimiento & !(withBlock.OBJType == Declaraciones.eOBJType.otTeleport & bIsExit);

            }

            return ItemNoEsDeMapaRet;
        }

        public static bool MostrarCantidad(short Index)
        {
            bool MostrarCantidadRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            {
                ref var withBlock = ref Declaraciones.ObjData_Renamed[Index];
                MostrarCantidadRet = withBlock.OBJType != Declaraciones.eOBJType.otPuertas & withBlock.OBJType != Declaraciones.eOBJType.otForos & withBlock.OBJType != Declaraciones.eOBJType.otCarteles & withBlock.OBJType != Declaraciones.eOBJType.otArboles & withBlock.OBJType != Declaraciones.eOBJType.otYacimiento & withBlock.OBJType != Declaraciones.eOBJType.otTeleport;
            }

            return MostrarCantidadRet;
        }

        public static bool EsObjetoFijo(Declaraciones.eOBJType OBJType)
        {
            bool EsObjetoFijoRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            EsObjetoFijoRet = OBJType == Declaraciones.eOBJType.otForos | OBJType == Declaraciones.eOBJType.otCarteles | OBJType == Declaraciones.eOBJType.otArboles | OBJType == Declaraciones.eOBJType.otYacimiento;
            return EsObjetoFijoRet;
        }
    }
}