using System;

namespace Legacy
{
    static class modSendData
    {
        // **************************************************************
        // SendData.bas - Has all methods to send data to different user groups.
        // Makes use of the modAreas module.
        // 
        // Implemented by Juan Martín Sotuyo Dodero (Maraxus) (juansotuyo@gmail.com)
        // **************************************************************

        // '
        // Contains all methods to send data to different user groups.
        // Makes use of the modAreas module.
        // 
        // @author Juan Martín Sotuyo Dodero (Maraxus) juansotuyo@gmail.com
        // @version 1.0.0
        // @date 20070107

        public enum SendTarget
        {
            ToAll = 1,
            toMap,
            ToPCArea,
            ToAllButIndex,
            ToMapButIndex,
            ToGM,
            ToNPCArea,
            ToGuildMembers,
            ToAdmins,
            ToPCAreaButIndex,
            ToAdminsAreaButConsejeros,
            ToDiosesYclan,
            ToConsejo,
            ToClanArea,
            ToConsejoCaos,
            ToRolesMasters,
            ToDeadArea,
            ToCiudadanos,
            ToCriminales,
            ToPartyArea,
            ToReal,
            ToCaos,
            ToCiudadanosYRMs,
            ToCriminalesYRMs,
            ToRealYRMs,
            ToCaosYRMs,
            ToHigherAdmins,
            ToGMsAreaButRmsOrCounselors,
            ToUsersAreaButGMs,
            ToUsersAndRmsAndCounselorsAreaButGMs
        }

        public static void SendData(SendTarget sndRoute, short sndIndex, string sndData)
        {
            // **************************************************************
            // Author: Juan Martín Sotuyo Dodero (Maraxus) - Rewrite of original
            // Last Modify Date: 01/08/2007
            // Last modified by: (liquid)
            // **************************************************************
            try
            {
                int LoopC;
                short Map;

                switch (sndRoute)
                {
                    case SendTarget.ToPCArea:
                        {
                            SendToUserArea(sndIndex, sndData);
                            return;
                        }

                    case SendTarget.ToAdmins:
                        {
                            var loopTo = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo; LoopC++)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    if ((Declaraciones.UserList[LoopC].flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.Consejero)) != 0)
                                    {
                                        TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                    }
                                }
                            }
                            return;
                        }

                    case SendTarget.ToAll:
                        {
                            var loopTo1 = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo1; LoopC++)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    if (Declaraciones.UserList[LoopC].flags.UserLogged) // Esta logeado como usuario?
                                    {
                                        TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                    }
                                }
                            }
                            return;
                        }

                    case SendTarget.ToAllButIndex:
                        {
                            var loopTo2 = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo2; LoopC++)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1 & LoopC != sndIndex)
                                {
                                    if (Declaraciones.UserList[LoopC].flags.UserLogged) // Esta logeado como usuario?
                                    {
                                        TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                    }
                                }
                            }
                            return;
                        }

                    case SendTarget.toMap:
                        {
                            SendToMap(sndIndex, sndData);
                            return;
                        }

                    case SendTarget.ToMapButIndex:
                        {
                            SendToMapButIndex(sndIndex, sndData);
                            return;
                        }

                    case SendTarget.ToGuildMembers:
                        {
                            LoopC = modGuilds.m_Iterador_ProximoUserIndex(sndIndex);
                            while (LoopC > 0)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                }
                                LoopC = modGuilds.m_Iterador_ProximoUserIndex(sndIndex);
                            }
                            return;
                        }

                    case SendTarget.ToDeadArea:
                        {
                            SendToDeadUserArea(sndIndex, sndData);
                            return;
                        }

                    case SendTarget.ToPCAreaButIndex:
                        {
                            SendToUserAreaButindex(sndIndex, sndData);
                            return;
                        }

                    case SendTarget.ToClanArea:
                        {
                            SendToUserGuildArea(sndIndex, sndData);
                            return;
                        }

                    case SendTarget.ToPartyArea:
                        {
                            SendToUserPartyArea(sndIndex, sndData);
                            return;
                        }

                    case SendTarget.ToAdminsAreaButConsejeros:
                        {
                            SendToAdminsButConsejerosArea(sndIndex, sndData);
                            return;
                        }

                    case SendTarget.ToNPCArea:
                        {
                            SendToNpcArea(sndIndex, sndData);
                            return;
                        }

                    case SendTarget.ToDiosesYclan:
                        {
                            LoopC = modGuilds.m_Iterador_ProximoUserIndex(sndIndex);
                            while (LoopC > 0)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                }
                                LoopC = modGuilds.m_Iterador_ProximoUserIndex(sndIndex);
                            }

                            LoopC = modGuilds.Iterador_ProximoGM(sndIndex);
                            while (LoopC > 0)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                }
                                LoopC = modGuilds.Iterador_ProximoGM(sndIndex);
                            }

                            return;
                        }

                    case SendTarget.ToConsejo:
                        {
                            var loopTo3 = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo3; LoopC++)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    if ((Declaraciones.UserList[LoopC].flags.Privilegios & Declaraciones.PlayerType.RoyalCouncil) != 0)
                                    {
                                        TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                    }
                                }
                            }
                            return;
                        }

                    case SendTarget.ToConsejoCaos:
                        {
                            var loopTo4 = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo4; LoopC++)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    if ((Declaraciones.UserList[LoopC].flags.Privilegios & Declaraciones.PlayerType.ChaosCouncil) != 0)
                                    {
                                        TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                    }
                                }
                            }
                            return;
                        }

                    case SendTarget.ToRolesMasters:
                        {
                            var loopTo5 = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo5; LoopC++)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    if ((Declaraciones.UserList[LoopC].flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0)
                                    {
                                        TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                    }
                                }
                            }
                            return;
                        }

                    case SendTarget.ToCiudadanos:
                        {
                            var loopTo6 = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo6; LoopC++)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    if (!ES.criminal(Convert.ToInt16(LoopC)))
                                    {
                                        TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                    }
                                }
                            }
                            return;
                        }

                    case SendTarget.ToCriminales:
                        {
                            var loopTo7 = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo7; LoopC++)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    if (ES.criminal(Convert.ToInt16(LoopC)))
                                    {
                                        TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                    }
                                }
                            }
                            return;
                        }

                    case SendTarget.ToReal:
                        {
                            var loopTo8 = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo8; LoopC++)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    if (Declaraciones.UserList[LoopC].Faccion.ArmadaReal == 1)
                                    {
                                        TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                    }
                                }
                            }
                            return;
                        }

                    case SendTarget.ToCaos:
                        {
                            var loopTo9 = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo9; LoopC++)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    if (Declaraciones.UserList[LoopC].Faccion.FuerzasCaos == 1)
                                    {
                                        TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                    }
                                }
                            }
                            return;
                        }

                    case SendTarget.ToCiudadanosYRMs:
                        {
                            var loopTo10 = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo10; LoopC++)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    if (!ES.criminal(Convert.ToInt16(LoopC)) | (Declaraciones.UserList[LoopC].flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0)
                                    {
                                        TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                    }
                                }
                            }
                            return;
                        }

                    case SendTarget.ToCriminalesYRMs:
                        {
                            var loopTo11 = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo11; LoopC++)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    if (ES.criminal(Convert.ToInt16(LoopC)) | (Declaraciones.UserList[LoopC].flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0)
                                    {
                                        TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                    }
                                }
                            }
                            return;
                        }

                    case SendTarget.ToRealYRMs:
                        {
                            var loopTo12 = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo12; LoopC++)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    if (Declaraciones.UserList[LoopC].Faccion.ArmadaReal == 1 | (Declaraciones.UserList[LoopC].flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0)
                                    {
                                        TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                    }
                                }
                            }
                            return;
                        }

                    case SendTarget.ToCaosYRMs:
                        {
                            var loopTo13 = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo13; LoopC++)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    if (Declaraciones.UserList[LoopC].Faccion.FuerzasCaos == 1 | (Declaraciones.UserList[LoopC].flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0)
                                    {
                                        TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                    }
                                }
                            }
                            return;
                        }

                    case SendTarget.ToHigherAdmins:
                        {
                            var loopTo14 = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo14; LoopC++)
                            {
                                if (Declaraciones.UserList[LoopC].ConnID != -1)
                                {
                                    if ((Declaraciones.UserList[LoopC].flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) != 0)
                                    {
                                        TCP.EnviarDatosASlot(Convert.ToInt16(LoopC), ref sndData);
                                    }
                                }
                            }
                            return;
                        }

                    case SendTarget.ToGMsAreaButRmsOrCounselors:
                        {
                            SendToGMsAreaButRmsOrCounselors(sndIndex, sndData);
                            return;
                        }

                    case SendTarget.ToUsersAreaButGMs:
                        {
                            SendToUsersAreaButGMs(sndIndex, sndData);
                            return;
                        }
                    case SendTarget.ToUsersAndRmsAndCounselorsAreaButGMs:
                        {
                            SendToUsersAndRmsAndCounselorsAreaButGMs(sndIndex, sndData);
                            return;
                        }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in SendData: " + ex.Message);
            }
        }

        private static void SendToUserArea(short UserIndex, string sdData)
        {
            // **************************************************************
            // Author: Lucio N. Tourrilhes (DuNga)
            // Last Modify Date: Unknow
            // 
            // **************************************************************
            int LoopC;
            short tempIndex;

            short Map;
            short AreaX;
            short AreaY;

            Map = Declaraciones.UserList[UserIndex].Pos.Map;
            AreaX = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceX;
            AreaY = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceY;

            if (!General.MapaValido(Map))
                return;

            var loopTo = ModAreas.ConnGroups[Map].CountEntrys;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                tempIndex = Convert.ToInt16(ModAreas.ConnGroups[Map].UserEntrys[LoopC]);

                if ((Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveX & AreaX) != 0) // Esta en el area?
                {
                    if ((Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveY & AreaY) != 0)
                    {
                        if (Declaraciones.UserList[tempIndex].ConnIDValida)
                        {
                            TCP.EnviarDatosASlot(tempIndex, ref sdData);
                        }
                    }
                }
            }
        }

        private static void SendToUserAreaButindex(short UserIndex, string sdData)
        {
            // **************************************************************
            // Author: Lucio N. Tourrilhes (DuNga)
            // Last Modify Date: Unknow
            // 
            // **************************************************************
            int LoopC;
            short TempInt;
            short tempIndex;

            short Map;
            short AreaX;
            short AreaY;

            Map = Declaraciones.UserList[UserIndex].Pos.Map;
            AreaX = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceX;
            AreaY = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceY;

            if (!General.MapaValido(Map))
                return;

            var loopTo = ModAreas.ConnGroups[Map].CountEntrys;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                tempIndex = Convert.ToInt16(ModAreas.ConnGroups[Map].UserEntrys[LoopC]);

                TempInt = (short)(Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveX & AreaX);
                if (TempInt != 0) // Esta en el area?
                {
                    TempInt = (short)(Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveY & AreaY);
                    if (TempInt != 0)
                    {
                        if (tempIndex != UserIndex)
                        {
                            if (Declaraciones.UserList[tempIndex].ConnIDValida)
                            {
                                TCP.EnviarDatosASlot(tempIndex, ref sdData);
                            }
                        }
                    }
                }
            }
        }

        private static void SendToDeadUserArea(short UserIndex, string sdData)
        {
            // **************************************************************
            // Author: Juan Martín Sotuyo Dodero (Maraxus)
            // Last Modify Date: Unknow
            // 
            // **************************************************************
            int LoopC;
            short tempIndex;

            short Map;
            short AreaX;
            short AreaY;

            Map = Declaraciones.UserList[UserIndex].Pos.Map;
            AreaX = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceX;
            AreaY = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceY;

            if (!General.MapaValido(Map))
                return;

            var loopTo = ModAreas.ConnGroups[Map].CountEntrys;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                tempIndex = Convert.ToInt16(ModAreas.ConnGroups[Map].UserEntrys[LoopC]);

                if ((Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveX & AreaX) != 0) // Esta en el area?
                {
                    if ((Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveY & AreaY) != 0)
                    {
                        // Dead and admins read
                        if (Declaraciones.UserList[tempIndex].ConnIDValida == true & (Declaraciones.UserList[tempIndex].flags.Muerto == 1 | (Declaraciones.UserList[tempIndex].flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.Consejero)) != 0))
                        {
                            TCP.EnviarDatosASlot(tempIndex, ref sdData);
                        }
                    }
                }
            }
        }

        private static void SendToUserGuildArea(short UserIndex, string sdData)
        {
            // **************************************************************
            // Author: Juan Martín Sotuyo Dodero (Maraxus)
            // Last Modify Date: Unknow
            // 
            // **************************************************************
            int LoopC;
            short tempIndex;

            short Map;
            short AreaX;
            short AreaY;

            Map = Declaraciones.UserList[UserIndex].Pos.Map;
            AreaX = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceX;
            AreaY = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceY;

            if (!General.MapaValido(Map))
                return;

            if (Declaraciones.UserList[UserIndex].GuildIndex == 0)
                return;

            var loopTo = ModAreas.ConnGroups[Map].CountEntrys;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                tempIndex = Convert.ToInt16(ModAreas.ConnGroups[Map].UserEntrys[LoopC]);

                if ((Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveX & AreaX) != 0) // Esta en el area?
                {
                    if ((Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveY & AreaY) != 0)
                    {
                        if (Declaraciones.UserList[tempIndex].ConnIDValida & (Declaraciones.UserList[tempIndex].GuildIndex == Declaraciones.UserList[UserIndex].GuildIndex | (Declaraciones.UserList[tempIndex].flags.Privilegios & Declaraciones.PlayerType.Dios) != 0 & (Declaraciones.UserList[tempIndex].flags.Privilegios & Declaraciones.PlayerType.RoleMaster) == 0))
                        {
                            TCP.EnviarDatosASlot(tempIndex, ref sdData);
                        }
                    }
                }
            }
        }

        private static void SendToUserPartyArea(short UserIndex, string sdData)
        {
            // **************************************************************
            // Author: Juan Martín Sotuyo Dodero (Maraxus)
            // Last Modify Date: Unknow
            // 
            // **************************************************************
            int LoopC;
            short tempIndex;

            short Map;
            short AreaX;
            short AreaY;

            Map = Declaraciones.UserList[UserIndex].Pos.Map;
            AreaX = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceX;
            AreaY = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceY;

            if (!General.MapaValido(Map))
                return;

            if (Declaraciones.UserList[UserIndex].PartyIndex == 0)
                return;

            var loopTo = ModAreas.ConnGroups[Map].CountEntrys;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                tempIndex = Convert.ToInt16(ModAreas.ConnGroups[Map].UserEntrys[LoopC]);

                if ((Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveX & AreaX) != 0) // Esta en el area?
                {
                    if ((Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveY & AreaY) != 0)
                    {
                        if (Declaraciones.UserList[tempIndex].ConnIDValida & Declaraciones.UserList[tempIndex].PartyIndex == Declaraciones.UserList[UserIndex].PartyIndex)
                        {
                            TCP.EnviarDatosASlot(tempIndex, ref sdData);
                        }
                    }
                }
            }
        }

        private static void SendToAdminsButConsejerosArea(short UserIndex, string sdData)
        {
            // **************************************************************
            // Author: Juan Martín Sotuyo Dodero (Maraxus)
            // Last Modify Date: Unknow
            // 
            // **************************************************************
            int LoopC;
            short tempIndex;

            short Map;
            short AreaX;
            short AreaY;

            Map = Declaraciones.UserList[UserIndex].Pos.Map;
            AreaX = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceX;
            AreaY = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceY;

            if (!General.MapaValido(Map))
                return;

            var loopTo = ModAreas.ConnGroups[Map].CountEntrys;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                tempIndex = Convert.ToInt16(ModAreas.ConnGroups[Map].UserEntrys[LoopC]);

                if ((Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveX & AreaX) != 0) // Esta en el area?
                {
                    if ((Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveY & AreaY) != 0)
                    {
                        if (Declaraciones.UserList[tempIndex].ConnIDValida)
                        {
                            if ((Declaraciones.UserList[tempIndex].flags.Privilegios & (Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin)) != 0)
                                TCP.EnviarDatosASlot(tempIndex, ref sdData);
                        }
                    }
                }
            }
        }

        private static void SendToNpcArea(int NpcIndex, string sdData)
        {
            // **************************************************************
            // Author: Lucio N. Tourrilhes (DuNga)
            // Last Modify Date: Unknow
            // 
            // **************************************************************
            int LoopC;
            short TempInt;
            short tempIndex;

            short Map;
            short AreaX;
            short AreaY;

            Map = Declaraciones.Npclist[NpcIndex].Pos.Map;
            AreaX = Declaraciones.Npclist[NpcIndex].AreasInfo.AreaPerteneceX;
            AreaY = Declaraciones.Npclist[NpcIndex].AreasInfo.AreaPerteneceY;

            if (!General.MapaValido(Map))
                return;

            var loopTo = ModAreas.ConnGroups[Map].CountEntrys;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                tempIndex = Convert.ToInt16(ModAreas.ConnGroups[Map].UserEntrys[LoopC]);

                TempInt = (short)(Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveX & AreaX);
                if (TempInt != 0) // Esta en el area?
                {
                    TempInt = (short)(Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveY & AreaY);
                    if (TempInt != 0)
                    {
                        if (Declaraciones.UserList[tempIndex].ConnIDValida)
                        {
                            TCP.EnviarDatosASlot(tempIndex, ref sdData);
                        }
                    }
                }
            }
        }

        public static void SendToAreaByPos(short Map, short AreaX, short AreaY, string sdData)
        {
            // **************************************************************
            // Author: Lucio N. Tourrilhes (DuNga)
            // Last Modify Date: Unknow
            // 
            // **************************************************************
            int LoopC;
            short TempInt;
            short tempIndex;

            AreaX = Convert.ToInt16(Math.Pow(2d, AreaX / 9));
            AreaY = Convert.ToInt16(Math.Pow(2d, AreaY / 9));

            if (!General.MapaValido(Map))
                return;

            var loopTo = ModAreas.ConnGroups[Map].CountEntrys;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                tempIndex = Convert.ToInt16(ModAreas.ConnGroups[Map].UserEntrys[LoopC]);

                TempInt = (short)(Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveX & AreaX);
                if (TempInt != 0) // Esta en el area?
                {
                    TempInt = (short)(Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveY & AreaY);
                    if (TempInt != 0)
                    {
                        if (Declaraciones.UserList[tempIndex].ConnIDValida)
                        {
                            TCP.EnviarDatosASlot(tempIndex, ref sdData);
                        }
                    }
                }
            }
        }

        public static void SendToMap(short Map, string sdData)
        {
            // **************************************************************
            // Author: Juan Martín Sotuyo Dodero (Maraxus)
            // Last Modify Date: 5/24/2007
            // 
            // **************************************************************
            int LoopC;
            short tempIndex;

            if (!General.MapaValido(Map))
                return;

            var loopTo = ModAreas.ConnGroups[Map].CountEntrys;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                tempIndex = Convert.ToInt16(ModAreas.ConnGroups[Map].UserEntrys[LoopC]);

                if (Declaraciones.UserList[tempIndex].ConnIDValida)
                {
                    TCP.EnviarDatosASlot(tempIndex, ref sdData);
                }
            }
        }

        public static void SendToMapButIndex(short UserIndex, string sdData)
        {
            // **************************************************************
            // Author: Juan Martín Sotuyo Dodero (Maraxus)
            // Last Modify Date: 5/24/2007
            // 
            // **************************************************************
            int LoopC;
            short Map;
            short tempIndex;

            Map = Declaraciones.UserList[UserIndex].Pos.Map;

            if (!General.MapaValido(Map))
                return;

            var loopTo = ModAreas.ConnGroups[Map].CountEntrys;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                tempIndex = Convert.ToInt16(ModAreas.ConnGroups[Map].UserEntrys[LoopC]);

                if (tempIndex != UserIndex & Declaraciones.UserList[tempIndex].ConnIDValida)
                {
                    TCP.EnviarDatosASlot(tempIndex, ref sdData);
                }
            }
        }

        private static void SendToGMsAreaButRmsOrCounselors(short UserIndex, string sdData)
        {
            // **************************************************************
            // Author: Torres Patricio(Pato)
            // Last Modify Date: 12/02/2010
            // 12/02/2010: ZaMa - Restrinjo solo a dioses, admins y gms.
            // 15/02/2010: ZaMa - Cambio el nombre de la funcion (viejo: ToGmsArea, nuevo: ToGmsAreaButRMsOrCounselors)
            // **************************************************************
            int LoopC;
            short tempIndex;

            short Map;
            short AreaX;
            short AreaY;

            Map = Declaraciones.UserList[UserIndex].Pos.Map;
            AreaX = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceX;
            AreaY = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceY;

            if (!General.MapaValido(Map))
                return;

            var loopTo = ModAreas.ConnGroups[Map].CountEntrys;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                tempIndex = Convert.ToInt16(ModAreas.ConnGroups[Map].UserEntrys[LoopC]);

                {
                    ref var withBlock = ref Declaraciones.UserList[tempIndex];
                    if ((withBlock.AreasInfo.AreaReciveX & AreaX) != 0) // Esta en el area?
                    {
                        if ((withBlock.AreasInfo.AreaReciveY & AreaY) != 0)
                        {
                            if (withBlock.ConnIDValida)
                            {
                                // Exclusivo para dioses, admins y gms
                                if ((int)(withBlock.flags.Privilegios & ~Declaraciones.PlayerType.User & ~Declaraciones.PlayerType.Consejero & ~Declaraciones.PlayerType.RoleMaster) != 0 & withBlock.flags.Privilegios != 0)
                                {
                                    TCP.EnviarDatosASlot(tempIndex, ref sdData);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void SendToUsersAreaButGMs(short UserIndex, string sdData)
        {
            // **************************************************************
            // Author: Torres Patricio(Pato)
            // Last Modify Date: 10/17/2009
            // 
            // **************************************************************
            int LoopC;
            short tempIndex;

            short Map;
            short AreaX;
            short AreaY;

            Map = Declaraciones.UserList[UserIndex].Pos.Map;
            AreaX = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceX;
            AreaY = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceY;

            if (!General.MapaValido(Map))
                return;

            var loopTo = ModAreas.ConnGroups[Map].CountEntrys;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                tempIndex = Convert.ToInt16(ModAreas.ConnGroups[Map].UserEntrys[LoopC]);

                if ((Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveX & AreaX) != 0) // Esta en el area?
                {
                    if ((Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveY & AreaY) != 0)
                    {
                        if (Declaraciones.UserList[tempIndex].ConnIDValida)
                        {
                            if ((Declaraciones.UserList[tempIndex].flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                            {
                                TCP.EnviarDatosASlot(tempIndex, ref sdData);
                            }
                        }
                    }
                }
            }
        }

        private static void SendToUsersAndRmsAndCounselorsAreaButGMs(short UserIndex, string sdData)
        {
            // **************************************************************
            // Author: Torres Patricio(Pato)
            // Last Modify Date: 10/17/2009
            // 
            // **************************************************************
            int LoopC;
            short tempIndex;

            short Map;
            short AreaX;
            short AreaY;

            Map = Declaraciones.UserList[UserIndex].Pos.Map;
            AreaX = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceX;
            AreaY = Declaraciones.UserList[UserIndex].AreasInfo.AreaPerteneceY;

            if (!General.MapaValido(Map))
                return;

            var loopTo = ModAreas.ConnGroups[Map].CountEntrys;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                tempIndex = Convert.ToInt16(ModAreas.ConnGroups[Map].UserEntrys[LoopC]);

                if ((Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveX & AreaX) != 0) // Esta en el area?
                {
                    if ((Declaraciones.UserList[tempIndex].AreasInfo.AreaReciveY & AreaY) != 0)
                    {
                        if (Declaraciones.UserList[tempIndex].ConnIDValida)
                        {
                            if ((Declaraciones.UserList[tempIndex].flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero | Declaraciones.PlayerType.RoleMaster)) != 0)
                            {
                                TCP.EnviarDatosASlot(tempIndex, ref sdData);
                            }
                        }
                    }
                }
            }
        }

        public static void AlertarFaccionarios(short UserIndex)
        {
            // **************************************************************
            // Author: ZaMa
            // Last Modify Date: 17/11/2009
            // Alerta a los faccionarios, dandoles una orientacion
            // **************************************************************
            int LoopC;
            short tempIndex;
            short Map;
            Protocol.FontTypeNames Font;

            if (Extra.esCaos(UserIndex))
            {
                Font = Protocol.FontTypeNames.FONTTYPE_CONSEJOCAOS;
            }
            else
            {
                Font = Protocol.FontTypeNames.FONTTYPE_CONSEJO;
            }

            Map = Declaraciones.UserList[UserIndex].Pos.Map;

            if (!General.MapaValido(Map))
                return;

            var loopTo = ModAreas.ConnGroups[Map].CountEntrys;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                tempIndex = Convert.ToInt16(ModAreas.ConnGroups[Map].UserEntrys[LoopC]);

                if (Declaraciones.UserList[tempIndex].ConnIDValida)
                {
                    if (tempIndex != UserIndex)
                    {
                        // Solo se envia a los de la misma faccion
                        if (UsUaRiOs.SameFaccion(UserIndex, tempIndex))
                        {
                            string argdatos = Protocol.PrepareMessageConsoleMsg("Escuchas el llamado de un compañero que proviene del " + UsUaRiOs.GetDireccion(UserIndex, tempIndex), Font);
                            TCP.EnviarDatosASlot(tempIndex, ref argdatos);
                        }
                    }
                }
            }
        }
    }
}