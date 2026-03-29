using System;
using Microsoft.VisualBasic;

namespace Legacy
{
    static class AI
    {
        public enum TipoAI
        {
            ESTATICO = 1,
            MueveAlAzar = 2,
            NpcMaloAtacaUsersBuenos = 3,
            NPCDEFENSA = 4,
            GuardiasAtacanCriminales = 5,
            NpcObjeto = 6,
            SigueAmo = 8,
            NpcAtacaNpc = 9,
            NpcPathfinding = 10,
            // Pretorianos
            SacerdotePretorianoAi = 20,
            GuerreroPretorianoAi = 21,
            MagoPretorianoAi = 22,
            CazadorPretorianoAi = 23,
            ReyPretoriano = 24
        }

        public const short ELEMENTALFUEGO = 93;
        public const short ELEMENTALTIERRA = 94;
        public const short ELEMENTALAGUA = 92;

        // Damos a los NPCs el mismo rango de visiï¿½n que un PJ
        public const byte RANGO_VISION_X = 8;
        public const byte RANGO_VISION_Y = 6;

        // ?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
        // ?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
        // ?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
        // Modulo AI_NPC
        // ?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
        // ?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
        // ?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
        // AI de los NPC
        // ?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
        // ?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
        // ?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½

        private static void GuardiasAI(short NpcIndex, bool DelCaos)
        {
            // ***************************************************
            // Autor: Unknown (orginal version)
            // Last Modification: 12/01/2010 (ZaMa)
            // 14/09/2009: ZaMa - Now npcs don't atack protected users.
            // 12/01/2010: ZaMa - Los npcs no atacan druidas mimetizados con npcs
            // ***************************************************
            Declaraciones.WorldPos nPos;
            Declaraciones.eHeading headingloop;
            short UI;
            bool UserProtected;

            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                for (headingloop = Declaraciones.eHeading.NORTH; headingloop <= Declaraciones.eHeading.WEST; headingloop++)
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    nPos = withBlock.Pos;
                    if (withBlock.flags.Inmovilizado == 0 | headingloop == withBlock.Char_Renamed.heading)
                    {
                        Extra.HeadtoPos(headingloop, ref nPos);
                        if (Extra.InMapBounds(nPos.Map, nPos.X, nPos.Y))
                        {
                            UI = Declaraciones.MapData[nPos.Map, nPos.X, nPos.Y].UserIndex;
                            if (UI > 0)
                            {
                                UserProtected = !modNuevoTimer.IntervaloPermiteSerAtacado(UI) & Declaraciones.UserList[UI].flags.NoPuedeSerAtacado;
                                UserProtected = UserProtected | Declaraciones.UserList[UI].flags.Ignorado | Declaraciones.UserList[UI].flags.EnConsulta;

                                if (Declaraciones.UserList[UI].flags.Muerto == 0 & Declaraciones.UserList[UI].flags.AdminPerseguible & !UserProtected)
                                {
                                    // ï¿½ES CRIMINAL?
                                    if (!DelCaos)
                                    {
                                        if (ES.criminal(UI))
                                        {
                                            if (SistemaCombate.NpcAtacaUser(NpcIndex, UI))
                                            {
                                                NPCs.ChangeNPCChar(NpcIndex, withBlock.Char_Renamed.body, withBlock.Char_Renamed.Head, headingloop);
                                            }
                                            return;
                                        }
                                        else if ((withBlock.flags.AttackedBy ?? "") == (Declaraciones.UserList[UI].name ?? "") & !withBlock.flags.Follow)
                                        {

                                            if (SistemaCombate.NpcAtacaUser(NpcIndex, UI))
                                            {
                                                NPCs.ChangeNPCChar(NpcIndex, withBlock.Char_Renamed.body, withBlock.Char_Renamed.Head, headingloop);
                                            }
                                            return;
                                        }
                                    }
                                    else if (!ES.criminal(UI))
                                    {
                                        if (SistemaCombate.NpcAtacaUser(NpcIndex, UI))
                                        {
                                            NPCs.ChangeNPCChar(NpcIndex, withBlock.Char_Renamed.body, withBlock.Char_Renamed.Head, headingloop);
                                        }
                                        return;
                                    }
                                    else if ((withBlock.flags.AttackedBy ?? "") == (Declaraciones.UserList[UI].name ?? "") & !withBlock.flags.Follow)
                                    {

                                        if (SistemaCombate.NpcAtacaUser(NpcIndex, UI))
                                        {
                                            NPCs.ChangeNPCChar(NpcIndex, withBlock.Char_Renamed.body, withBlock.Char_Renamed.Head, headingloop);
                                        }
                                        return;
                                    }
                                }
                            }
                        }
                    } // not inmovil
                }
            }

            RestoreOldMovement(NpcIndex);
        }

        // '
        // Handles the evil npcs' artificial intelligency.
        // 
        // @param NpcIndex Specifies reference to the npc
        private static void HostilMalvadoAI(short NpcIndex)
        {
            // **************************************************************
            // Author: Unknown
            // Last Modify Date: 12/01/2010 (ZaMa)
            // 28/04/2009: ZaMa - Now those NPCs who doble attack, have 50% of posibility of casting a spell on user.
            // 14/09/200*: ZaMa - Now npcs don't atack protected users.
            // 12/01/2010: ZaMa - Los npcs no atacan druidas mimetizados con npcs
            // **************************************************************
            Declaraciones.WorldPos nPos;
            Declaraciones.eHeading headingloop;
            short UI;
            short NPCI;
            bool atacoPJ;
            bool UserProtected;

            atacoPJ = false;

            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                for (headingloop = Declaraciones.eHeading.NORTH; headingloop <= Declaraciones.eHeading.WEST; headingloop++)
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    nPos = withBlock.Pos;
                    if (withBlock.flags.Inmovilizado == 0 | withBlock.Char_Renamed.heading == headingloop)
                    {
                        Extra.HeadtoPos(headingloop, ref nPos);
                        if (Extra.InMapBounds(nPos.Map, nPos.X, nPos.Y))
                        {
                            UI = Declaraciones.MapData[nPos.Map, nPos.X, nPos.Y].UserIndex;
                            NPCI = Declaraciones.MapData[nPos.Map, nPos.X, nPos.Y].NpcIndex;
                            if (UI > 0 & !atacoPJ)
                            {
                                UserProtected = !modNuevoTimer.IntervaloPermiteSerAtacado(UI) & Declaraciones.UserList[UI].flags.NoPuedeSerAtacado;
                                UserProtected = UserProtected | Declaraciones.UserList[UI].flags.Ignorado | Declaraciones.UserList[UI].flags.EnConsulta;

                                if (Declaraciones.UserList[UI].flags.Muerto == 0 & Declaraciones.UserList[UI].flags.AdminPerseguible & !UserProtected)
                                {

                                    atacoPJ = true;
                                    if (withBlock.Movement == TipoAI.NpcObjeto)
                                    {
                                        // Los npc objeto no atacan siempre al mismo usuario
                                        if (Matematicas.RandomNumber(1, 3) == 3)
                                            atacoPJ = false;
                                    }

                                    if (atacoPJ)
                                    {
                                        if (withBlock.flags.LanzaSpells != 0)
                                        {
                                            if (withBlock.flags.AtacaDoble != 0)
                                            {
                                                if (Matematicas.RandomNumber(0, 1) != 0)
                                                {
                                                    if (SistemaCombate.NpcAtacaUser(NpcIndex, UI))
                                                    {
                                                        NPCs.ChangeNPCChar(NpcIndex, withBlock.Char_Renamed.body, withBlock.Char_Renamed.Head, headingloop);
                                                    }
                                                    return;
                                                }
                                            }

                                            NPCs.ChangeNPCChar(NpcIndex, withBlock.Char_Renamed.body, withBlock.Char_Renamed.Head, headingloop);
                                            NpcLanzaUnSpell(NpcIndex, UI);
                                        }
                                    }
                                    if (SistemaCombate.NpcAtacaUser(NpcIndex, UI))
                                    {
                                        NPCs.ChangeNPCChar(NpcIndex, withBlock.Char_Renamed.body, withBlock.Char_Renamed.Head, headingloop);
                                    }
                                    return;

                                }
                            }
                            else if (NPCI > 0)
                            {
                                if (Declaraciones.Npclist[NPCI].MaestroUser > 0 & Declaraciones.Npclist[NPCI].flags.Paralizado == 0)
                                {
                                    NPCs.ChangeNPCChar(NpcIndex, withBlock.Char_Renamed.body, withBlock.Char_Renamed.Head, headingloop);
                                    SistemaCombate.NpcAtacaNpc(NpcIndex, NPCI, false);
                                    return;
                                }
                            }
                        }
                    } // inmo
                }
            }

            RestoreOldMovement(NpcIndex);
        }

        private static void HostilBuenoAI(short NpcIndex)
        {
            // ***************************************************
            // Autor: Unknown (orginal version)
            // Last Modification: 12/01/2010 (ZaMa)
            // 14/09/2009: ZaMa - Now npcs don't atack protected users.
            // 12/01/2010: ZaMa - Los npcs no atacan druidas mimetizados con npcs
            // ***************************************************
            Declaraciones.WorldPos nPos;
            Declaraciones.eHeading headingloop;
            short UI;
            bool UserProtected;

            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                for (headingloop = Declaraciones.eHeading.NORTH; headingloop <= Declaraciones.eHeading.WEST; headingloop++)
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    nPos = withBlock.Pos;
                    if (withBlock.flags.Inmovilizado == 0 | withBlock.Char_Renamed.heading == headingloop)
                    {
                        Extra.HeadtoPos(headingloop, ref nPos);
                        if (Extra.InMapBounds(nPos.Map, nPos.X, nPos.Y))
                        {
                            UI = Declaraciones.MapData[nPos.Map, nPos.X, nPos.Y].UserIndex;
                            if (UI > 0)
                            {
                                if ((Declaraciones.UserList[UI].name ?? "") == (withBlock.flags.AttackedBy ?? ""))
                                {

                                    UserProtected = !modNuevoTimer.IntervaloPermiteSerAtacado(UI) & Declaraciones.UserList[UI].flags.NoPuedeSerAtacado;
                                    UserProtected = UserProtected | Declaraciones.UserList[UI].flags.Ignorado | Declaraciones.UserList[UI].flags.EnConsulta;

                                    if (Declaraciones.UserList[UI].flags.Muerto == 0 & Declaraciones.UserList[UI].flags.AdminPerseguible & !UserProtected)
                                    {
                                        if (withBlock.flags.LanzaSpells > 0)
                                        {
                                            NpcLanzaUnSpell(NpcIndex, UI);
                                        }

                                        if (SistemaCombate.NpcAtacaUser(NpcIndex, UI))
                                        {
                                            NPCs.ChangeNPCChar(NpcIndex, withBlock.Char_Renamed.body, withBlock.Char_Renamed.Head, headingloop);
                                        }
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            RestoreOldMovement(NpcIndex);
        }

        private static void IrUsuarioCercano(short NpcIndex)
        {
            // ***************************************************
            // Autor: Unknown (orginal version)
            // Last Modification: 12/01/2010 (ZaMa)
            // 14/09/2009: ZaMa - Now npcs don't follow protected users.
            // 12/01/2010: ZaMa - Los npcs no atacan druidas mimetizados con npcs
            // ***************************************************
            byte tHeading;
            short UserIndex;
            var SignoNS = default(short);
            var SignoEO = default(short);
            int i;
            bool UserProtected;

            short OwnerIndex;
            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                if (withBlock.flags.Inmovilizado == 1)
                {
                    switch (withBlock.Char_Renamed.heading)
                    {
                        case Declaraciones.eHeading.NORTH:
                            {
                                SignoNS = -1;
                                SignoEO = 0;
                                break;
                            }

                        case Declaraciones.eHeading.EAST:
                            {
                                SignoNS = 0;
                                SignoEO = 1;
                                break;
                            }

                        case Declaraciones.eHeading.SOUTH:
                            {
                                SignoNS = 1;
                                SignoEO = 0;
                                break;
                            }

                        case Declaraciones.eHeading.WEST:
                            {
                                SignoEO = -1;
                                SignoNS = 0;
                                break;
                            }
                    }

                    var loopTo = ModAreas.ConnGroups[withBlock.Pos.Map].CountEntrys;
                    for (i = 1; i <= loopTo; i++)
                    {
                        UserIndex = Convert.ToInt16(ModAreas.ConnGroups[withBlock.Pos.Map].UserEntrys[i]);

                        // Is it in it's range of vision??
                        if (Math.Abs((short)(Declaraciones.UserList[UserIndex].Pos.X - withBlock.Pos.X)) <= RANGO_VISION_X & Math.Sign((short)(Declaraciones.UserList[UserIndex].Pos.X - withBlock.Pos.X)) == SignoEO)
                        {
                            if (Math.Abs((short)(Declaraciones.UserList[UserIndex].Pos.Y - withBlock.Pos.Y)) <= RANGO_VISION_Y & Math.Sign((short)(Declaraciones.UserList[UserIndex].Pos.Y - withBlock.Pos.Y)) == SignoNS)
                            {

                                UserProtected = !modNuevoTimer.IntervaloPermiteSerAtacado(UserIndex) & Declaraciones.UserList[UserIndex].flags.NoPuedeSerAtacado;
                                UserProtected = UserProtected | Declaraciones.UserList[UserIndex].flags.Ignorado | Declaraciones.UserList[UserIndex].flags.EnConsulta;

                                if (Declaraciones.UserList[UserIndex].flags.Muerto == 0)
                                {
                                    if (!UserProtected)
                                    {
                                        if (withBlock.flags.LanzaSpells != 0)
                                            NpcLanzaUnSpell(NpcIndex, UserIndex);
                                        return;
                                    }
                                }

                            }
                        }
                    }
                }

                // No esta inmobilizado
                else
                {

                    // Tiene prioridad de seguir al usuario al que le pertenece si esta en el rango de vision

                    OwnerIndex = withBlock.Owner;
                    if (OwnerIndex > 0)
                    {

                        // Is it in it's range of vision??
                        if (Math.Abs((short)(Declaraciones.UserList[OwnerIndex].Pos.X - withBlock.Pos.X)) <= RANGO_VISION_X)
                        {
                            if (Math.Abs((short)(Declaraciones.UserList[OwnerIndex].Pos.Y - withBlock.Pos.Y)) <= RANGO_VISION_Y)
                            {

                                // va hacia el si o esta invi ni oculto
                                if (Declaraciones.UserList[OwnerIndex].flags.invisible == 0 & Declaraciones.UserList[OwnerIndex].flags.Oculto == 0 & !Declaraciones.UserList[OwnerIndex].flags.EnConsulta & !Declaraciones.UserList[OwnerIndex].flags.Ignorado)

                                {
                                    if (withBlock.flags.LanzaSpells != 0)
                                        NpcLanzaUnSpell(NpcIndex, OwnerIndex);

                                    tHeading = (byte)Extra.FindDirection(ref withBlock.Pos, ref Declaraciones.UserList[OwnerIndex].Pos);
                                    NPCs.MoveNPCChar(NpcIndex, tHeading);
                                    return;
                                }
                            }
                        }

                    }

                    // No le pertenece a nadie o el dueño no esta en el rango de vision, sigue a cualquiera
                    var loopTo1 = ModAreas.ConnGroups[withBlock.Pos.Map].CountEntrys;
                    for (i = 1; i <= loopTo1; i++)
                    {
                        UserIndex = Convert.ToInt16(ModAreas.ConnGroups[withBlock.Pos.Map].UserEntrys[i]);

                        // Is it in it's range of vision??
                        if (Math.Abs((short)(Declaraciones.UserList[UserIndex].Pos.X - withBlock.Pos.X)) <= RANGO_VISION_X)
                        {
                            if (Math.Abs((short)(Declaraciones.UserList[UserIndex].Pos.Y - withBlock.Pos.Y)) <= RANGO_VISION_Y)
                            {

                                {
                                    ref var withBlock1 = ref Declaraciones.UserList[UserIndex];

                                    UserProtected = !modNuevoTimer.IntervaloPermiteSerAtacado(UserIndex) & withBlock1.flags.NoPuedeSerAtacado;
                                    UserProtected = UserProtected | withBlock1.flags.Ignorado | withBlock1.flags.EnConsulta;

                                    if (withBlock1.flags.Muerto == 0 & withBlock1.flags.invisible == 0 & withBlock1.flags.Oculto == 0 & withBlock1.flags.AdminPerseguible & !UserProtected)
                                    {

                                        if (Declaraciones.Npclist[NpcIndex].flags.LanzaSpells != 0)
                                            NpcLanzaUnSpell(NpcIndex, UserIndex);

                                        tHeading = (byte)Extra.FindDirection(ref Declaraciones.Npclist[NpcIndex].Pos, ref withBlock1.Pos);
                                        NPCs.MoveNPCChar(NpcIndex, tHeading);
                                        return;
                                    }

                                }

                            }
                        }
                    }

                    // Si llega aca es que no habï¿½a ningï¿½n usuario cercano vivo.
                    // A bailar. Pablo (ToxicWaste)
                    if (Matematicas.RandomNumber(0, 10) == 0)
                    {
                        NPCs.MoveNPCChar(NpcIndex, Convert.ToByte(Matematicas.RandomNumber((int)Declaraciones.eHeading.NORTH, (int)Declaraciones.eHeading.WEST)));
                    }

                }
            }

            RestoreOldMovement(NpcIndex);
        }

        // '
        // Makes a Pet / Summoned Npc to Follow an enemy
        // 
        // @param NpcIndex Specifies reference to the npc
        private static void SeguirAgresor(short NpcIndex)
        {
            // **************************************************************
            // Author: Unknown
            // Last Modify by: Marco Vanotti (MarKoxX)
            // Last Modify Date: 08/16/2008
            // 08/16/2008: MarKoxX - Now pets that do melï¿½ attacks have to be near the enemy to attack.
            // **************************************************************
            byte tHeading;
            short UI;

            int i;

            var SignoNS = default(short);
            var SignoEO = default(short);

            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                if (withBlock.flags.Paralizado == 1 | withBlock.flags.Inmovilizado == 1)
                {
                    switch (withBlock.Char_Renamed.heading)
                    {
                        case Declaraciones.eHeading.NORTH:
                            {
                                SignoNS = -1;
                                SignoEO = 0;
                                break;
                            }

                        case Declaraciones.eHeading.EAST:
                            {
                                SignoNS = 0;
                                SignoEO = 1;
                                break;
                            }

                        case Declaraciones.eHeading.SOUTH:
                            {
                                SignoNS = 1;
                                SignoEO = 0;
                                break;
                            }

                        case Declaraciones.eHeading.WEST:
                            {
                                SignoEO = -1;
                                SignoNS = 0;
                                break;
                            }
                    }

                    var loopTo = ModAreas.ConnGroups[withBlock.Pos.Map].CountEntrys;
                    for (i = 1; i <= loopTo; i++)
                    {
                        UI = Convert.ToInt16(ModAreas.ConnGroups[withBlock.Pos.Map].UserEntrys[i]);

                        // Is it in it's range of vision??
                        if (Math.Abs((short)(Declaraciones.UserList[UI].Pos.X - withBlock.Pos.X)) <= RANGO_VISION_X & Math.Sign((short)(Declaraciones.UserList[UI].Pos.X - withBlock.Pos.X)) == SignoEO)
                        {
                            if (Math.Abs((short)(Declaraciones.UserList[UI].Pos.Y - withBlock.Pos.Y)) <= RANGO_VISION_Y & Math.Sign((short)(Declaraciones.UserList[UI].Pos.Y - withBlock.Pos.Y)) == SignoNS)
                            {

                                if ((Declaraciones.UserList[UI].name ?? "") == (withBlock.flags.AttackedBy ?? ""))
                                {
                                    if (withBlock.MaestroUser > 0)
                                    {
                                        if (!ES.criminal(withBlock.MaestroUser) & !ES.criminal(UI) & (Declaraciones.UserList[withBlock.MaestroUser].flags.Seguro | Declaraciones.UserList[withBlock.MaestroUser].Faccion.ArmadaReal == 1))
                                        {
                                            Protocol.WriteConsoleMsg(withBlock.MaestroUser, "La mascota no atacará a ciudadanos si eres miembro del ejército real o tienes el seguro activado.", Protocol.FontTypeNames.FONTTYPE_INFO);
                                            Protocol.FlushBuffer(withBlock.MaestroUser);
                                            withBlock.flags.AttackedBy = Constants.vbNullString;
                                            return;
                                        }
                                    }

                                    if (Declaraciones.UserList[UI].flags.Muerto == 0 & Declaraciones.UserList[UI].flags.invisible == 0 & Declaraciones.UserList[UI].flags.Oculto == 0)
                                    {
                                        if (withBlock.flags.LanzaSpells > 0)
                                        {
                                            NpcLanzaUnSpell(NpcIndex, UI);
                                        }
                                        else if (Matematicas.Distancia(ref Declaraciones.UserList[UI].Pos, ref Declaraciones.Npclist[NpcIndex].Pos) <= 1)
                                        {
                                            // TODO : Set this a separate AI for Elementals and Druid's pets
                                            if (Declaraciones.Npclist[NpcIndex].Numero != 92)
                                            {
                                                SistemaCombate.NpcAtacaUser(NpcIndex, UI);
                                            }
                                        }
                                        return;
                                    }
                                }

                            }
                        }

                    }
                }
                else
                {
                    var loopTo1 = ModAreas.ConnGroups[withBlock.Pos.Map].CountEntrys;
                    for (i = 1; i <= loopTo1; i++)
                    {
                        UI = Convert.ToInt16(ModAreas.ConnGroups[withBlock.Pos.Map].UserEntrys[i]);

                        // Is it in it's range of vision??
                        if (Math.Abs((short)(Declaraciones.UserList[UI].Pos.X - withBlock.Pos.X)) <= RANGO_VISION_X)
                        {
                            if (Math.Abs((short)(Declaraciones.UserList[UI].Pos.Y - withBlock.Pos.Y)) <= RANGO_VISION_Y)
                            {

                                if ((Declaraciones.UserList[UI].name ?? "") == (withBlock.flags.AttackedBy ?? ""))
                                {
                                    if (withBlock.MaestroUser > 0)
                                    {
                                        if (!ES.criminal(withBlock.MaestroUser) & !ES.criminal(UI) & (Declaraciones.UserList[withBlock.MaestroUser].flags.Seguro | Declaraciones.UserList[withBlock.MaestroUser].Faccion.ArmadaReal == 1))
                                        {
                                            Protocol.WriteConsoleMsg(withBlock.MaestroUser, "La mascota no atacará a ciudadanos si eres miembro del ejército real o tienes el seguro activado.", Protocol.FontTypeNames.FONTTYPE_INFO);
                                            Protocol.FlushBuffer(withBlock.MaestroUser);
                                            withBlock.flags.AttackedBy = Constants.vbNullString;
                                            NPCs.FollowAmo(NpcIndex);
                                            return;
                                        }
                                    }

                                    if (Declaraciones.UserList[UI].flags.Muerto == 0 & Declaraciones.UserList[UI].flags.invisible == 0 & Declaraciones.UserList[UI].flags.Oculto == 0)
                                    {
                                        if (withBlock.flags.LanzaSpells > 0)
                                        {
                                            NpcLanzaUnSpell(NpcIndex, UI);
                                        }
                                        else if (Matematicas.Distancia(ref Declaraciones.UserList[UI].Pos, ref Declaraciones.Npclist[NpcIndex].Pos) <= 1)
                                        {
                                            // TODO : Set this a separate AI for Elementals and Druid's pets
                                            if (Declaraciones.Npclist[NpcIndex].Numero != 92)
                                            {
                                                SistemaCombate.NpcAtacaUser(NpcIndex, UI);
                                            }
                                        }

                                        tHeading = (byte)Extra.FindDirection(ref withBlock.Pos, ref Declaraciones.UserList[UI].Pos);
                                        NPCs.MoveNPCChar(NpcIndex, tHeading);

                                        return;
                                    }
                                }

                            }
                        }

                    }
                }
            }

            RestoreOldMovement(NpcIndex);
        }

        private static void RestoreOldMovement(short NpcIndex)
        {
            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                if (withBlock.MaestroUser == 0)
                {
                    withBlock.Movement = withBlock.flags.OldMovement;
                    withBlock.Hostile = withBlock.flags.OldHostil;
                    withBlock.flags.AttackedBy = Constants.vbNullString;
                }
            }
        }

        private static void PersigueCiudadano(short NpcIndex)
        {
            // ***************************************************
            // Autor: Unknown (orginal version)
            // Last Modification: 12/01/2010 (ZaMa)
            // 14/09/2009: ZaMa - Now npcs don't follow protected users.
            // 12/01/2010: ZaMa - Los npcs no atacan druidas mimetizados con npcs.
            // ***************************************************
            short UserIndex;
            byte tHeading;
            int i;
            bool UserProtected;

            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                var loopTo = ModAreas.ConnGroups[withBlock.Pos.Map].CountEntrys;
                for (i = 1; i <= loopTo; i++)
                {
                    UserIndex = Convert.ToInt16(ModAreas.ConnGroups[withBlock.Pos.Map].UserEntrys[i]);

                    // Is it in it's range of vision??
                    if (Math.Abs((short)(Declaraciones.UserList[UserIndex].Pos.X - withBlock.Pos.X)) <= RANGO_VISION_X)
                    {
                        if (Math.Abs((short)(Declaraciones.UserList[UserIndex].Pos.Y - withBlock.Pos.Y)) <= RANGO_VISION_Y)
                        {

                            if (!ES.criminal(UserIndex))
                            {

                                UserProtected = !modNuevoTimer.IntervaloPermiteSerAtacado(UserIndex) & Declaraciones.UserList[UserIndex].flags.NoPuedeSerAtacado;
                                UserProtected = UserProtected | Declaraciones.UserList[UserIndex].flags.Ignorado | Declaraciones.UserList[UserIndex].flags.EnConsulta;

                                if (Declaraciones.UserList[UserIndex].flags.Muerto == 0 & Declaraciones.UserList[UserIndex].flags.invisible == 0 & Declaraciones.UserList[UserIndex].flags.Oculto == 0 & Declaraciones.UserList[UserIndex].flags.AdminPerseguible & !UserProtected)
                                {

                                    if (withBlock.flags.LanzaSpells > 0)
                                    {
                                        NpcLanzaUnSpell(NpcIndex, UserIndex);
                                    }
                                    tHeading = (byte)Extra.FindDirection(ref withBlock.Pos, ref Declaraciones.UserList[UserIndex].Pos);
                                    NPCs.MoveNPCChar(NpcIndex, tHeading);
                                    return;
                                }
                            }

                        }
                    }

                }
            }

            RestoreOldMovement(NpcIndex);
        }

        private static void PersigueCriminal(short NpcIndex)
        {
            // ***************************************************
            // Autor: Unknown (orginal version)
            // Last Modification: 12/01/2010 (ZaMa)
            // 14/09/2009: ZaMa - Now npcs don't follow protected users.
            // 12/01/2010: ZaMa - Los npcs no atacan druidas mimetizados con npcs.
            // ***************************************************
            short UserIndex;
            byte tHeading;
            int i;
            var SignoNS = default(short);
            var SignoEO = default(short);
            bool UserProtected;

            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                if (withBlock.flags.Inmovilizado == 1)
                {
                    switch (withBlock.Char_Renamed.heading)
                    {
                        case Declaraciones.eHeading.NORTH:
                            {
                                SignoNS = -1;
                                SignoEO = 0;
                                break;
                            }

                        case Declaraciones.eHeading.EAST:
                            {
                                SignoNS = 0;
                                SignoEO = 1;
                                break;
                            }

                        case Declaraciones.eHeading.SOUTH:
                            {
                                SignoNS = 1;
                                SignoEO = 0;
                                break;
                            }

                        case Declaraciones.eHeading.WEST:
                            {
                                SignoEO = -1;
                                SignoNS = 0;
                                break;
                            }
                    }

                    var loopTo = ModAreas.ConnGroups[withBlock.Pos.Map].CountEntrys;
                    for (i = 1; i <= loopTo; i++)
                    {
                        UserIndex = Convert.ToInt16(ModAreas.ConnGroups[withBlock.Pos.Map].UserEntrys[i]);

                        // Is it in it's range of vision??
                        if (Math.Abs((short)(Declaraciones.UserList[UserIndex].Pos.X - withBlock.Pos.X)) <= RANGO_VISION_X & Math.Sign((short)(Declaraciones.UserList[UserIndex].Pos.X - withBlock.Pos.X)) == SignoEO)
                        {
                            if (Math.Abs((short)(Declaraciones.UserList[UserIndex].Pos.Y - withBlock.Pos.Y)) <= RANGO_VISION_Y & Math.Sign((short)(Declaraciones.UserList[UserIndex].Pos.Y - withBlock.Pos.Y)) == SignoNS)
                            {

                                if (ES.criminal(UserIndex))
                                {
                                    {
                                        ref var withBlock1 = ref Declaraciones.UserList[UserIndex];

                                        UserProtected = !modNuevoTimer.IntervaloPermiteSerAtacado(UserIndex) & withBlock1.flags.NoPuedeSerAtacado;
                                        UserProtected = UserProtected | Declaraciones.UserList[UserIndex].flags.Ignorado | Declaraciones.UserList[UserIndex].flags.EnConsulta;

                                        if (withBlock1.flags.Muerto == 0 & withBlock1.flags.invisible == 0 & withBlock1.flags.Oculto == 0 & withBlock1.flags.AdminPerseguible & !UserProtected)
                                        {

                                            if (Declaraciones.Npclist[NpcIndex].flags.LanzaSpells > 0)
                                            {
                                                NpcLanzaUnSpell(NpcIndex, UserIndex);
                                            }
                                            return;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {
                    var loopTo1 = ModAreas.ConnGroups[withBlock.Pos.Map].CountEntrys;
                    for (i = 1; i <= loopTo1; i++)
                    {
                        UserIndex = Convert.ToInt16(ModAreas.ConnGroups[withBlock.Pos.Map].UserEntrys[i]);

                        // Is it in it's range of vision??
                        if (Math.Abs((short)(Declaraciones.UserList[UserIndex].Pos.X - withBlock.Pos.X)) <= RANGO_VISION_X)
                        {
                            if (Math.Abs((short)(Declaraciones.UserList[UserIndex].Pos.Y - withBlock.Pos.Y)) <= RANGO_VISION_Y)
                            {

                                if (ES.criminal(UserIndex))
                                {

                                    UserProtected = !modNuevoTimer.IntervaloPermiteSerAtacado(UserIndex) & Declaraciones.UserList[UserIndex].flags.NoPuedeSerAtacado;
                                    UserProtected = UserProtected | Declaraciones.UserList[UserIndex].flags.Ignorado;

                                    if (Declaraciones.UserList[UserIndex].flags.Muerto == 0 & Declaraciones.UserList[UserIndex].flags.invisible == 0 & Declaraciones.UserList[UserIndex].flags.Oculto == 0 & Declaraciones.UserList[UserIndex].flags.AdminPerseguible & !UserProtected)
                                    {
                                        if (withBlock.flags.LanzaSpells > 0)
                                        {
                                            NpcLanzaUnSpell(NpcIndex, UserIndex);
                                        }
                                        if (withBlock.flags.Inmovilizado == 1)
                                            return;
                                        tHeading = (byte)Extra.FindDirection(ref withBlock.Pos, ref Declaraciones.UserList[UserIndex].Pos);
                                        NPCs.MoveNPCChar(NpcIndex, tHeading);
                                        return;
                                    }
                                }

                            }
                        }

                    }
                }
            }

            RestoreOldMovement(NpcIndex);
        }

        private static void SeguirAmo(short NpcIndex)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            byte tHeading;
            short UI;

            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                if (withBlock.Target == 0 & withBlock.TargetNPC == 0)
                {
                    UI = withBlock.MaestroUser;

                    if (UI > 0)
                    {
                        // Is it in it's range of vision??
                        if (Math.Abs((short)(Declaraciones.UserList[UI].Pos.X - withBlock.Pos.X)) <= RANGO_VISION_X)
                        {
                            if (Math.Abs((short)(Declaraciones.UserList[UI].Pos.Y - withBlock.Pos.Y)) <= RANGO_VISION_Y)
                            {
                                if (Declaraciones.UserList[UI].flags.Muerto == 0 & Declaraciones.UserList[UI].flags.invisible == 0 & Declaraciones.UserList[UI].flags.Oculto == 0 & Matematicas.Distancia(ref withBlock.Pos, ref Declaraciones.UserList[UI].Pos) > 3)
                                {
                                    tHeading = (byte)Extra.FindDirection(ref withBlock.Pos, ref Declaraciones.UserList[UI].Pos);
                                    NPCs.MoveNPCChar(NpcIndex, tHeading);
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            RestoreOldMovement(NpcIndex);
        }

        private static void AiNpcAtacaNpc(short NpcIndex)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            byte tHeading;
            int X;
            int Y;
            short NI;
            var bNoEsta = default(bool);

            var SignoNS = default(short);
            var SignoEO = default(short);

            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                if (withBlock.flags.Inmovilizado == 1)
                {
                    switch (withBlock.Char_Renamed.heading)
                    {
                        case Declaraciones.eHeading.NORTH:
                            {
                                SignoNS = -1;
                                SignoEO = 0;
                                break;
                            }

                        case Declaraciones.eHeading.EAST:
                            {
                                SignoNS = 0;
                                SignoEO = 1;
                                break;
                            }

                        case Declaraciones.eHeading.SOUTH:
                            {
                                SignoNS = 1;
                                SignoEO = 0;
                                break;
                            }

                        case Declaraciones.eHeading.WEST:
                            {
                                SignoEO = -1;
                                SignoNS = 0;
                                break;
                            }
                    }

                    var loopTo = withBlock.Pos.Y + (short)(SignoNS * RANGO_VISION_Y);
                    for (Y = withBlock.Pos.Y; SignoNS == 0 ? 1 : SignoNS >= 0 ? Y <= loopTo : Y >= loopTo; Y += SignoNS == 0 ? 1 : SignoNS)
                    {
                        var loopTo1 = withBlock.Pos.X + (short)(SignoEO * RANGO_VISION_X);
                        for (X = withBlock.Pos.X; SignoEO == 0 ? 1 : SignoEO >= 0 ? X <= loopTo1 : X >= loopTo1; X += SignoEO == 0 ? 1 : SignoEO)
                        {
                            if (X >= Declaraciones.MinXBorder & X <= Declaraciones.MaxXBorder & Y >= Declaraciones.MinYBorder & Y <= Declaraciones.MaxYBorder)
                            {
                                NI = Declaraciones.MapData[withBlock.Pos.Map, X, Y].NpcIndex;
                                if (NI > 0)
                                {
                                    if (withBlock.TargetNPC == NI)
                                    {
                                        bNoEsta = true;
                                        if (withBlock.Numero == ELEMENTALFUEGO)
                                        {
                                            NpcLanzaUnSpellSobreNpc(NpcIndex, NI);
                                            if (Declaraciones.Npclist[NI].NPCtype == Declaraciones.eNPCType.DRAGON)
                                            {
                                                Declaraciones.Npclist[NI].CanAttack = 1;
                                                NpcLanzaUnSpellSobreNpc(NI, NpcIndex);
                                            }
                                        }
                                        // aca verificamosss la distancia de ataque
                                        else if (Matematicas.Distancia(ref withBlock.Pos, ref Declaraciones.Npclist[NI].Pos) <= 1)
                                        {
                                            SistemaCombate.NpcAtacaNpc(NpcIndex, NI);
                                        }
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    var loopTo2 = withBlock.Pos.Y + RANGO_VISION_Y;
                    for (Y = withBlock.Pos.Y - RANGO_VISION_Y; Y <= loopTo2; Y++)
                    {
                        var loopTo3 = withBlock.Pos.X + RANGO_VISION_Y;
                        for (X = withBlock.Pos.X - RANGO_VISION_Y; X <= loopTo3; X++)
                        {
                            if (X >= Declaraciones.MinXBorder & X <= Declaraciones.MaxXBorder & Y >= Declaraciones.MinYBorder & Y <= Declaraciones.MaxYBorder)
                            {
                                NI = Declaraciones.MapData[withBlock.Pos.Map, X, Y].NpcIndex;
                                if (NI > 0)
                                {
                                    if (withBlock.TargetNPC == NI)
                                    {
                                        bNoEsta = true;
                                        if (withBlock.Numero == ELEMENTALFUEGO)
                                        {
                                            NpcLanzaUnSpellSobreNpc(NpcIndex, NI);
                                            if (Declaraciones.Npclist[NI].NPCtype == Declaraciones.eNPCType.DRAGON)
                                            {
                                                Declaraciones.Npclist[NI].CanAttack = 1;
                                                NpcLanzaUnSpellSobreNpc(NI, NpcIndex);
                                            }
                                        }
                                        // aca verificamosss la distancia de ataque
                                        else if (Matematicas.Distancia(ref withBlock.Pos, ref Declaraciones.Npclist[NI].Pos) <= 1)
                                        {
                                            SistemaCombate.NpcAtacaNpc(NpcIndex, NI);
                                        }
                                        if (withBlock.flags.Inmovilizado == 1)
                                            return;
                                        if (withBlock.TargetNPC == 0)
                                            return;
                                        tHeading = (byte)Extra.FindDirection(ref withBlock.Pos, ref Declaraciones.Npclist[Declaraciones.MapData[withBlock.Pos.Map, X, Y].NpcIndex].Pos);
                                        NPCs.MoveNPCChar(NpcIndex, tHeading);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }

                if (!bNoEsta)
                {
                    if (withBlock.MaestroUser > 0)
                    {
                        NPCs.FollowAmo(NpcIndex);
                    }
                    else
                    {
                        withBlock.Movement = withBlock.flags.OldMovement;
                        withBlock.Hostile = withBlock.flags.OldHostil;
                    }
                }
            }
        }

        public static void AiNpcObjeto(short NpcIndex)
        {
            // ***************************************************
            // Autor: ZaMa
            // Last Modification: 14/09/2009 (ZaMa)
            // 14/09/2009: ZaMa - Now npcs don't follow protected users.
            // ***************************************************
            short UserIndex;
            byte tHeading;
            int i;
            short SignoNS;
            short SignoEO;
            bool UserProtected;

            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                var loopTo = ModAreas.ConnGroups[withBlock.Pos.Map].CountEntrys;
                for (i = 1; i <= loopTo; i++)
                {
                    UserIndex = Convert.ToInt16(ModAreas.ConnGroups[withBlock.Pos.Map].UserEntrys[i]);

                    // Is it in it's range of vision??
                    if (Math.Abs((short)(Declaraciones.UserList[UserIndex].Pos.X - withBlock.Pos.X)) <= RANGO_VISION_X)
                    {
                        if (Math.Abs((short)(Declaraciones.UserList[UserIndex].Pos.Y - withBlock.Pos.Y)) <= RANGO_VISION_Y)
                        {

                            {
                                ref var withBlock1 = ref Declaraciones.UserList[UserIndex];
                                UserProtected = !modNuevoTimer.IntervaloPermiteSerAtacado(UserIndex) & withBlock1.flags.NoPuedeSerAtacado;

                                if (withBlock1.flags.Muerto == 0 & withBlock1.flags.invisible == 0 & withBlock1.flags.Oculto == 0 & withBlock1.flags.AdminPerseguible & !UserProtected)
                                {

                                    // No quiero que ataque siempre al primero
                                    if (Matematicas.RandomNumber(1, 3) < 3)
                                    {
                                        if (Declaraciones.Npclist[NpcIndex].flags.LanzaSpells > 0)
                                        {
                                            NpcLanzaUnSpell(NpcIndex, UserIndex);
                                        }

                                        return;
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }

        public static void NPCAI(short NpcIndex)
        {
            // **************************************************************
            // Author: Unknown
            // Last Modify by: ZaMa
            // Last Modify Date: 15/11/2009
            // 08/16/2008: MarKoxX - Now pets that do melï¿½ attacks have to be near the enemy to attack.
            // 15/11/2009: ZaMa - Implementacion de npc objetos ai.
            // **************************************************************
            try
            {
                {
                    ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                    // <<<<<<<<<<< Ataques >>>>>>>>>>>>>>>>
                    if (withBlock.MaestroUser == 0)
                    {
                        // Busca a alguien para atacar
                        // ï¿½Es un guardia?
                        if (withBlock.NPCtype == Declaraciones.eNPCType.GuardiaReal)
                        {
                            GuardiasAI(NpcIndex, false);
                        }
                        else if (withBlock.NPCtype == Declaraciones.eNPCType.Guardiascaos)
                        {
                            GuardiasAI(NpcIndex, true);
                        }
                        else if (withBlock.Hostile != 0 && withBlock.Stats.Alineacion != 0)
                        {
                            HostilMalvadoAI(NpcIndex);
                        }
                        else if (withBlock.Hostile != 0 && withBlock.Stats.Alineacion == 0)
                        {
                            HostilBuenoAI(NpcIndex);
                        }
                    }
                    else
                    {
                        // Evitamos que ataque a su amo, a menos
                        // que el amo lo ataque.
                        // Call HostilBuenoAI(NpcIndex)
                    }


                    // <<<<<<<<<<<Movimiento>>>>>>>>>>>>>>>>
                    switch (withBlock.Movement)
                    {
                        case TipoAI.MueveAlAzar:
                            {
                                if (withBlock.flags.Inmovilizado == 1)
                                    return;
                                if (withBlock.NPCtype == Declaraciones.eNPCType.GuardiaReal)
                                {
                                    if (Matematicas.RandomNumber(1, 12) == 3)
                                    {
                                        NPCs.MoveNPCChar(NpcIndex, Convert.ToByte(Matematicas.RandomNumber((int)Declaraciones.eHeading.NORTH, (int)Declaraciones.eHeading.WEST)));
                                    }

                                    PersigueCriminal(NpcIndex);
                                }

                                else if (withBlock.NPCtype == Declaraciones.eNPCType.Guardiascaos)
                                {
                                    if (Matematicas.RandomNumber(1, 12) == 3)
                                    {
                                        NPCs.MoveNPCChar(NpcIndex, Convert.ToByte(Matematicas.RandomNumber((int)Declaraciones.eHeading.NORTH, (int)Declaraciones.eHeading.WEST)));
                                    }

                                    PersigueCiudadano(NpcIndex);
                                }

                                else if (Matematicas.RandomNumber(1, 12) == 3)
                                {
                                    NPCs.MoveNPCChar(NpcIndex, Convert.ToByte(Matematicas.RandomNumber((int)Declaraciones.eHeading.NORTH, (int)Declaraciones.eHeading.WEST)));
                                }

                                break;
                            }

                        // Va hacia el usuario cercano
                        case TipoAI.NpcMaloAtacaUsersBuenos:
                            {
                                IrUsuarioCercano(NpcIndex);
                                break;
                            }

                        // Va hacia el usuario que lo ataco(FOLLOW)
                        case TipoAI.NPCDEFENSA:
                            {
                                SeguirAgresor(NpcIndex);
                                break;
                            }

                        // Persigue criminales
                        case TipoAI.GuardiasAtacanCriminales:
                            {
                                PersigueCriminal(NpcIndex);
                                break;
                            }

                        case TipoAI.SigueAmo:
                            {
                                if (withBlock.flags.Inmovilizado == 1)
                                    return;
                                SeguirAmo(NpcIndex);
                                if (Matematicas.RandomNumber(1, 12) == 3)
                                {
                                    NPCs.MoveNPCChar(NpcIndex, Convert.ToByte(Matematicas.RandomNumber((int)Declaraciones.eHeading.NORTH, (int)Declaraciones.eHeading.WEST)));
                                }

                                break;
                            }

                        case TipoAI.NpcAtacaNpc:
                            {
                                AiNpcAtacaNpc(NpcIndex);
                                break;
                            }

                        case TipoAI.NpcObjeto:
                            {
                                AiNpcObjeto(NpcIndex);
                                break;
                            }

                        case TipoAI.NpcPathfinding:
                            {
                                if (withBlock.flags.Inmovilizado == 1)
                                    return;
                                if (ReCalculatePath(NpcIndex))
                                {
                                    PathFindingAI(NpcIndex);
                                    // Existe el camino?
                                    if (withBlock.PFINFO.NoPath) // Si no existe nos movemos al azar
                                    {
                                        // Move randomly
                                        NPCs.MoveNPCChar(NpcIndex, Convert.ToByte(Matematicas.RandomNumber((int)Declaraciones.eHeading.NORTH, (int)Declaraciones.eHeading.WEST)));
                                    }
                                }
                                else if (!PathEnd(NpcIndex))
                                {
                                    FollowPath(NpcIndex);
                                }
                                else
                                {
                                    withBlock.PFINFO.PathLenght = 0;
                                }

                                break;
                            }
                    }
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in GuardiasAI: " + ex.Message);
                string argdesc = "NPCAI " + Declaraciones.Npclist[NpcIndex].name + " " + Declaraciones.Npclist[NpcIndex].MaestroUser + " " + Declaraciones.Npclist[NpcIndex].MaestroNpc + " mapa:" + Declaraciones.Npclist[NpcIndex].Pos.Map + " x:" + Declaraciones.Npclist[NpcIndex].Pos.X + " y:" + Declaraciones.Npclist[NpcIndex].Pos.Y + " Mov:" + ((int)Declaraciones.Npclist[NpcIndex].Movement).ToString() + " TargU:" + Declaraciones.Npclist[NpcIndex].Target + " TargN:" + Declaraciones.Npclist[NpcIndex].TargetNPC;
                General.LogError(ref argdesc);
                // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura MiNPC, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
                Declaraciones.npc MiNPC;
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto MiNPC. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                MiNPC = Declaraciones.Npclist[NpcIndex];
                NPCs.QuitarNPC(NpcIndex);
                NPCs.ReSpawnNpc(ref MiNPC);
            }
        }

        public static bool UserNear(short NpcIndex)
        {
            bool UserNearRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // Returns True if there is an user adjacent to the npc position.
            // ***************************************************

            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                UserNearRet = !(Conversion.Int(Matematicas.Distance(withBlock.Pos.X, withBlock.Pos.Y, Declaraciones.UserList[withBlock.PFINFO.TargetUser].Pos.X, Declaraciones.UserList[withBlock.PFINFO.TargetUser].Pos.Y)) > 1d);
            }

            return UserNearRet;
        }

        public static bool ReCalculatePath(short NpcIndex)
        {
            bool ReCalculatePathRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // Returns true if we have to seek a new path
            // ***************************************************

            if (Declaraciones.Npclist[NpcIndex].PFINFO.PathLenght == 0)
            {
                ReCalculatePathRet = true;
            }
            else if (!UserNear(NpcIndex) & Declaraciones.Npclist[NpcIndex].PFINFO.PathLenght == Declaraciones.Npclist[NpcIndex].PFINFO.CurPos - 1)
            {
                ReCalculatePathRet = true;
            }

            return ReCalculatePathRet;
        }

        public static bool PathEnd(short NpcIndex)
        {
            bool PathEndRet = default;
            // ***************************************************
            // Author: Gulfas Morgolock
            // Last Modification: -
            // Returns if the npc has arrived to the end of its path
            // ***************************************************
            PathEndRet = Declaraciones.Npclist[NpcIndex].PFINFO.CurPos == Declaraciones.Npclist[NpcIndex].PFINFO.PathLenght;
            return PathEndRet;
        }

        public static bool FollowPath(short NpcIndex)
        {
            // ***************************************************
            // Author: Gulfas Morgolock
            // Last Modification: -
            // Moves the npc.
            // ***************************************************
            Declaraciones.WorldPos tmpPos;
            byte tHeading;

            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                tmpPos.Map = withBlock.Pos.Map;
                tmpPos.X = withBlock.PFINFO.Path[withBlock.PFINFO.CurPos].Y; // invertï¿½ las coordenadas
                tmpPos.Y = withBlock.PFINFO.Path[withBlock.PFINFO.CurPos].X;

                tHeading = (byte)Extra.FindDirection(ref withBlock.Pos, ref tmpPos);

                NPCs.MoveNPCChar(NpcIndex, tHeading);

                withBlock.PFINFO.CurPos = Convert.ToInt16(withBlock.PFINFO.CurPos + 1);
            }

            return default;
        }

        public static bool PathFindingAI(short NpcIndex)
        {
            // ***************************************************
            // Author: Gulfas Morgolock
            // Last Modification: -
            // This function seeks the shortest path from the Npc
            // to the user's location.
            // ***************************************************
            int Y;
            int X;

            short tmpUserIndex;
            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                var loopTo = withBlock.Pos.Y + 10;
                for (Y = withBlock.Pos.Y - 10; Y <= loopTo; Y++) // Makes a loop that looks at
                {
                    var loopTo1 = withBlock.Pos.X + 10;
                    for (X = withBlock.Pos.X - 10; X <= loopTo1; X++) // 5 tiles in every direction
                    {

                        // Make sure tile is legal
                        if (X > Declaraciones.MinXBorder & X < Declaraciones.MaxXBorder & Y > Declaraciones.MinYBorder & Y < Declaraciones.MaxYBorder)
                        {

                            // look for a user
                            if (Declaraciones.MapData[withBlock.Pos.Map, X, Y].UserIndex > 0)
                            {
                                // Move towards user
                                tmpUserIndex = Declaraciones.MapData[withBlock.Pos.Map, X, Y].UserIndex;
                                {
                                    ref var withBlock1 = ref Declaraciones.UserList[tmpUserIndex];
                                    if (withBlock1.flags.Muerto == 0 & withBlock1.flags.invisible == 0 & withBlock1.flags.Oculto == 0 & withBlock1.flags.AdminPerseguible)
                                    {
                                        // We have to invert the coordinates, this is because
                                        // ORE refers to maps in converse way of my pathfinding
                                        // routines.
                                        Declaraciones.Npclist[NpcIndex].PFINFO.Target.X = withBlock1.Pos.Y;
                                        Declaraciones.Npclist[NpcIndex].PFINFO.Target.Y = withBlock1.Pos.X; // ops!
                                        Declaraciones.Npclist[NpcIndex].PFINFO.TargetUser = tmpUserIndex;
                                        PathFinding.SeekPath(NpcIndex);
                                        return default;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return default;
        }

        public static void NpcLanzaUnSpell(short NpcIndex, short UserIndex)
        {
            // **************************************************************
            // Author: Unknown
            // Last Modify by: -
            // Last Modify Date: -
            // **************************************************************
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                if (withBlock.flags.invisible == 1 | withBlock.flags.Oculto == 1)
                    return;
            }

            short k;
            k = Convert.ToInt16(Matematicas.RandomNumber(1, Declaraciones.Npclist[NpcIndex].flags.LanzaSpells));
            modHechizos.NpcLanzaSpellSobreUser(NpcIndex, UserIndex, Declaraciones.Npclist[NpcIndex].Spells[k]);
        }

        public static void NpcLanzaUnSpellSobreNpc(short NpcIndex, short TargetNPC)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short k;
            k = Convert.ToInt16(Matematicas.RandomNumber(1, Declaraciones.Npclist[NpcIndex].flags.LanzaSpells));
            modHechizos.NpcLanzaSpellSobreNpc(NpcIndex, TargetNPC, Declaraciones.Npclist[NpcIndex].Spells[k]);
        }
    }
}