using System;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;

namespace Legacy;

internal static class NPCs
{
    // Contiene todas las rutinas necesarias para cotrolar los
    // NPCs meno la rutina de AI que se encuentra en el modulo
    // AI_NPCs para su mejor comprension.

    public static void QuitarMascota(short UserIndex, short NpcIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short i;

        for (i = 1; i <= Declaraciones.MAXMASCOTAS; i++)
            if (Declaraciones.UserList[UserIndex].MascotasIndex[i] == NpcIndex)
            {
                Declaraciones.UserList[UserIndex].MascotasIndex[i] = 0;
                Declaraciones.UserList[UserIndex].MascotasType[i] = 0;

                Declaraciones.UserList[UserIndex].NroMascotas =
                    Convert.ToInt16(Declaraciones.UserList[UserIndex].NroMascotas - 1);
                break;
            }
    }

    public static void QuitarMascotaNpc(short Maestro)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        Declaraciones.Npclist[Maestro].Mascotas = Convert.ToInt16(Declaraciones.Npclist[Maestro].Mascotas - 1);
    }

    public static void MuereNpc(short NpcIndex, short UserIndex)
    {
        // ********************************************************
        // Author: Unknown
        // Llamado cuando la vida de un NPC llega a cero.
        // Last Modify Date: 24/01/2007
        // 22/06/06: (Nacho) Chequeamos si es pretoriano
        // 24/01/2007: Pablo (ToxicWaste): Agrego para actualización de tag si cambia de status.
        // ********************************************************
        try
        {
            // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura MiNPC, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
            Declaraciones.npc MiNPC;
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto MiNPC. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            MiNPC = Declaraciones.Npclist[NpcIndex];
            bool EraCriminal;
            var IsPretoriano = default(bool);

            short i;
            short j;
            short NPCI;
            if (PraetoriansCoopNPC.esPretoriano(NpcIndex) == 4)
            {
                // Solo nos importa si fue matado en el mapa pretoriano.
                IsPretoriano = true;
                if (Declaraciones.Npclist[NpcIndex].Pos.Map == PraetoriansCoopNPC.MAPA_PRETORIANO)
                {
                    // seteamos todos estos 'flags' acorde para que cambien solos de alcoba

                    for (i = 8; i <= 90; i++)
                    for (j = 8; j <= 90; j++)
                    {
                        NPCI = Declaraciones.MapData[Declaraciones.Npclist[NpcIndex].Pos.Map, i, j].NpcIndex;
                        if (NPCI > 0)
                            if ((PraetoriansCoopNPC.esPretoriano(NPCI) > 0) & (NPCI != NpcIndex))
                            {
                                if (Declaraciones.Npclist[NpcIndex].Pos.X > 50)
                                {
                                    if (Declaraciones.Npclist[NPCI].Pos.X > 50)
                                        Declaraciones.Npclist[NPCI].Invent.ArmourEqpSlot = 1;
                                }
                                else if (Declaraciones.Npclist[NPCI].Pos.X <= 50)
                                {
                                    Declaraciones.Npclist[NPCI].Invent.ArmourEqpSlot = 5;
                                }
                            }
                    }

                    PraetoriansCoopNPC.CrearClanPretoriano(Declaraciones.Npclist[NpcIndex].Pos.X);
                }
            }
            else if (PraetoriansCoopNPC.esPretoriano(NpcIndex) > 0)
            {
                IsPretoriano = true;
                if (Declaraciones.Npclist[NpcIndex].Pos.Map == PraetoriansCoopNPC.MAPA_PRETORIANO)
                {
                    Declaraciones.Npclist[NpcIndex].Invent.ArmourEqpSlot = 0;
                    PraetoriansCoopNPC.pretorianosVivos = Convert.ToInt16(PraetoriansCoopNPC.pretorianosVivos - 1);
                }
            }

            // Quitamos el npc
            QuitarNPC(NpcIndex);


            short T;
            if (UserIndex > 0) // Lo mato un usuario?
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];

                if (MiNPC.flags.Snd3 > 0)
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                        Protocol.PrepareMessagePlayWave(Convert.ToByte(MiNPC.flags.Snd3), Convert.ToByte(MiNPC.Pos.X),
                            Convert.ToByte(MiNPC.Pos.Y)));
                withBlock.flags.TargetNPC = 0;
                withBlock.flags.TargetNpcTipo = Declaraciones.eNPCType.Comun;

                // El user que lo mato tiene mascotas?
                if (withBlock.NroMascotas > 0)
                    for (T = 1; T <= Declaraciones.MAXMASCOTAS; T++)
                        if (withBlock.MascotasIndex[T] > 0)
                            if (Declaraciones.Npclist[withBlock.MascotasIndex[T]].TargetNPC == NpcIndex)
                                FollowAmo(withBlock.MascotasIndex[T]);

                // [KEVIN]
                if (MiNPC.flags.ExpCount > 0)
                {
                    if (withBlock.PartyIndex > 0)
                    {
                        mdParty.ObtenerExito(UserIndex, MiNPC.flags.ExpCount, ref MiNPC.Pos.Map, ref MiNPC.Pos.X,
                            ref MiNPC.Pos.Y);
                    }
                    else
                    {
                        withBlock.Stats.Exp = withBlock.Stats.Exp + MiNPC.flags.ExpCount;
                        if (withBlock.Stats.Exp > Declaraciones.MAXEXP)
                            withBlock.Stats.Exp = Declaraciones.MAXEXP;
                        Protocol.WriteConsoleMsg(UserIndex,
                            "Has ganado " + MiNPC.flags.ExpCount + " puntos de experiencia.",
                            Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    }

                    MiNPC.flags.ExpCount = 0;
                }

                // [/KEVIN]
                Protocol.WriteConsoleMsg(UserIndex, "¡Has matado a la criatura!",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
                if (withBlock.Stats.NPCsMuertos < 32000)
                    withBlock.Stats.NPCsMuertos = Convert.ToInt16(withBlock.Stats.NPCsMuertos + 1);

                EraCriminal = ES.criminal(UserIndex);

                if (MiNPC.Stats.Alineacion == 0)
                {
                    if (MiNPC.Numero == Declaraciones.Guardias)
                    {
                        withBlock.Reputacion.NobleRep = 0;
                        withBlock.Reputacion.PlebeRep = 0;
                        withBlock.Reputacion.AsesinoRep = withBlock.Reputacion.AsesinoRep + 500;
                        if (withBlock.Reputacion.AsesinoRep > Declaraciones.MAXREP)
                            withBlock.Reputacion.AsesinoRep = Declaraciones.MAXREP;
                    }

                    if (MiNPC.MaestroUser == 0)
                    {
                        withBlock.Reputacion.AsesinoRep = withBlock.Reputacion.AsesinoRep + Declaraciones.vlASESINO;
                        if (withBlock.Reputacion.AsesinoRep > Declaraciones.MAXREP)
                            withBlock.Reputacion.AsesinoRep = Declaraciones.MAXREP;
                    }
                }
                else if (MiNPC.Stats.Alineacion == 1)
                {
                    withBlock.Reputacion.PlebeRep = withBlock.Reputacion.PlebeRep + Declaraciones.vlCAZADOR;
                    if (withBlock.Reputacion.PlebeRep > Declaraciones.MAXREP)
                        withBlock.Reputacion.PlebeRep = Declaraciones.MAXREP;
                }

                else if (MiNPC.Stats.Alineacion == 2)
                {
                    withBlock.Reputacion.NobleRep =
                        Convert.ToInt32(withBlock.Reputacion.NobleRep + Declaraciones.vlASESINO / 2d);
                    if (withBlock.Reputacion.NobleRep > Declaraciones.MAXREP)
                        withBlock.Reputacion.NobleRep = Declaraciones.MAXREP;
                }

                else if (MiNPC.Stats.Alineacion == 4)
                {
                    withBlock.Reputacion.PlebeRep = withBlock.Reputacion.PlebeRep + Declaraciones.vlCAZADOR;
                    if (withBlock.Reputacion.PlebeRep > Declaraciones.MAXREP)
                        withBlock.Reputacion.PlebeRep = Declaraciones.MAXREP;
                }

                if (ES.criminal(UserIndex) & Extra.esArmada(UserIndex))
                {
                    var argExpulsado = true;
                    ModFacciones.ExpulsarFaccionReal(UserIndex, ref argExpulsado);
                }

                if (!ES.criminal(UserIndex) & Extra.esCaos(UserIndex))
                {
                    var argExpulsado1 = true;
                    ModFacciones.ExpulsarFaccionCaos(UserIndex, ref argExpulsado1);
                }

                if (EraCriminal & !ES.criminal(UserIndex))
                    UsUaRiOs.RefreshCharStatus(UserIndex);
                else if (!EraCriminal & ES.criminal(UserIndex)) UsUaRiOs.RefreshCharStatus(UserIndex);

                UsUaRiOs.CheckUserLevel(UserIndex);
            } // Userindex > 0

            if (MiNPC.MaestroUser == 0)
            {
                // Tiramos el oro
                // Call NPCTirarOro(MiNPC)
                // Tiramos el inventario
                InvNpc.NPC_TIRAR_ITEMS(ref MiNPC, IsPretoriano);
                // ReSpawn o no
                ReSpawnNpc(ref MiNPC);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in QuitarMascota: " + ex.Message);
            var argdesc = "Error en MuereNpc - Error: " + ex.GetType().Name + " - Desc: " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    private static void ResetNpcFlags(short NpcIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // Clear the npc's flags

        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex].flags;
            withBlock.AfectaParalisis = 0;
            withBlock.AguaValida = 0;
            withBlock.AttackedBy = Constants.vbNullString;
            withBlock.AttackedFirstBy = Constants.vbNullString;
            withBlock.BackUp = 0;
            withBlock.Bendicion = 0;
            withBlock.Domable = 0;
            withBlock.Envenenado = 0;
            withBlock.Faccion = 0;
            withBlock.Follow = false;
            withBlock.AtacaDoble = 0;
            withBlock.LanzaSpells = 0;
            withBlock.invisible = 0;
            withBlock.Maldicion = 0;
            withBlock.OldHostil = 0;
            withBlock.OldMovement = 0;
            withBlock.Paralizado = 0;
            withBlock.Inmovilizado = 0;
            withBlock.Respawn = 0;
            withBlock.RespawnOrigPos = 0;
            withBlock.Snd1 = 0;
            withBlock.Snd2 = 0;
            withBlock.Snd3 = 0;
            withBlock.TierraInvalida = 0;
        }
    }

    private static void ResetNpcCounters(short NpcIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex].Contadores;
            withBlock.Paralisis = 0;
            withBlock.TiempoExistencia = 0;
        }
    }

    private static void ResetNpcCharInfo(short NpcIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex].character;
            withBlock.body = 0;
            withBlock.CascoAnim = 0;
            withBlock.CharIndex = 0;
            withBlock.FX = 0;
            withBlock.Head = 0;
            withBlock.heading = 0;
            withBlock.loops = 0;
            withBlock.ShieldAnim = 0;
            withBlock.WeaponAnim = 0;
        }
    }

    private static void ResetNpcCriatures(short NpcIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int j;

        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
            var loopTo = (int)withBlock.NroCriaturas;
            for (j = 1; j <= loopTo; j++)
            {
                withBlock.Criaturas[j].NpcIndex = 0;
                withBlock.Criaturas[j].NpcName = Constants.vbNullString;
            }

            withBlock.NroCriaturas = 0;
        }
    }

    public static void ResetExpresiones(short NpcIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int j;

        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
            var loopTo = (int)withBlock.NroExpresiones;
            for (j = 1; j <= loopTo; j++)
                withBlock.Expresiones[j] = Constants.vbNullString;

            withBlock.NroExpresiones = 0;
        }
    }

    private static void ResetNpcMainInfo(short NpcIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int j;
        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
            withBlock.Attackable = 0;
            withBlock.CanAttack = 0;
            withBlock.Comercia = 0;
            withBlock.GiveEXP = 0;
            withBlock.GiveGLD = 0;
            withBlock.Hostile = 0;
            withBlock.InvReSpawn = 0;

            if (withBlock.MaestroUser > 0)
                QuitarMascota(withBlock.MaestroUser, NpcIndex);
            if (withBlock.MaestroNpc > 0)
                QuitarMascotaNpc(withBlock.MaestroNpc);

            withBlock.MaestroUser = 0;
            withBlock.MaestroNpc = 0;

            withBlock.Mascotas = 0;
            withBlock.Movement = 0;
            withBlock.name = Constants.vbNullString;
            withBlock.NPCtype = 0;
            withBlock.Numero = 0;
            withBlock.Orig.Map = 0;
            withBlock.Orig.X = 0;
            withBlock.Orig.Y = 0;
            withBlock.PoderAtaque = 0;
            withBlock.PoderEvasion = 0;
            withBlock.Pos.Map = 0;
            withBlock.Pos.X = 0;
            withBlock.Pos.Y = 0;
            withBlock.SkillDomar = 0;
            withBlock.Target = 0;
            withBlock.TargetNPC = 0;
            withBlock.TipoItems = 0;
            withBlock.Veneno = 0;
            withBlock.desc = Constants.vbNullString;


            var loopTo = (int)withBlock.NroSpells;
            for (j = 1; j <= loopTo; j++)
                withBlock.Spells[j] = 0;
        }

        ResetNpcCharInfo(NpcIndex);
        ResetNpcCriatures(NpcIndex);
        ResetExpresiones(NpcIndex);
    }

    public static void QuitarNPC(short NpcIndex)
    {
        // ***************************************************
        // Autor: Unknown (orginal version)
        // Last Modification: 16/11/2009
        // 16/11/2009: ZaMa - Now npcs lose their owner
        // ***************************************************
        try
        {
            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                withBlock.flags.NPCActive = false;

                withBlock.Owner = 0; // Murio, no necesita mas dueños :P.

                if (Extra.InMapBounds(withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y)) EraseNPCChar(NpcIndex);
            }

            // Nos aseguramos de que el inventario sea removido...
            // asi los lobos no volveran a tirar armaduras ;))
            InvNpc.ResetNpcInv(NpcIndex);
            ResetNpcFlags(NpcIndex);
            ResetNpcCounters(NpcIndex);

            ResetNpcMainInfo(NpcIndex);

            if (NpcIndex == Declaraciones.LastNPC)
                while (!Declaraciones.Npclist[Declaraciones.LastNPC].flags.NPCActive)
                {
                    Declaraciones.LastNPC = Convert.ToInt16(Declaraciones.LastNPC - 1);
                    if (Declaraciones.LastNPC < 1)
                        break;
                }


            if (Declaraciones.NumNPCs != 0) Declaraciones.NumNPCs = Convert.ToInt16(Declaraciones.NumNPCs - 1);
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in ResetNpcFlags: " + ex.Message);
            var argdesc = "Error en QuitarNPC";
            General.LogError(ref argdesc);
        }
    }

    public static void QuitarPet(short UserIndex, short NpcIndex)
    {
        // ***************************************************
        // Autor: ZaMa
        // Last Modification: 18/11/2009
        // Kills a pet
        // ***************************************************
        short i;
        var PetIndex = default(short);

        try
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];

                // Busco el indice de la mascota
                for (i = 1; i <= Declaraciones.MAXMASCOTAS; i++)
                    if (withBlock.MascotasIndex[i] == NpcIndex)
                        PetIndex = i;

                // Poco probable que pase, pero por las dudas..
                if (PetIndex == 0)
                    return;

                // Limpio el slot de la mascota
                withBlock.NroMascotas = Convert.ToInt16(withBlock.NroMascotas - 1);
                withBlock.MascotasIndex[PetIndex] = 0;
                withBlock.MascotasType[PetIndex] = 0;

                // Elimino la mascota
                QuitarNPC(NpcIndex);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in QuitarPet: " + ex.Message);
            var argdesc = "Error en QuitarPet. Error: " + ex.GetType().Name + " Desc: " + ex.Message + " NpcIndex: " +
                          NpcIndex + " UserIndex: " + UserIndex + " PetIndex: " + PetIndex;
            General.LogError(ref argdesc);
        }
    }

    private static bool TestSpawnTrigger(ref Declaraciones.WorldPos Pos,
        [Optional] [DefaultParameterValue(false)] ref bool PuedeAgua)
    {
        bool TestSpawnTriggerRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if (Extra.LegalPos(Pos.Map, Pos.X, Pos.Y, PuedeAgua))
            TestSpawnTriggerRet = ((int)Declaraciones.MapData[Pos.Map, Pos.X, Pos.Y].trigger != 3) &
                                  ((int)Declaraciones.MapData[Pos.Map, Pos.X, Pos.Y].trigger != 2) &
                                  ((int)Declaraciones.MapData[Pos.Map, Pos.X, Pos.Y].trigger != 1);

        return TestSpawnTriggerRet;
    }

    public static void CrearNPC(ref short NroNPC, ref short mapa, ref Declaraciones.WorldPos OrigPos)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // Crea un NPC del tipo NRONPC

        Declaraciones.WorldPos Pos;
        var newpos = default(Declaraciones.WorldPos);
        var altpos = default(Declaraciones.WorldPos);
        short nIndex;
        var PosicionValida = default(bool);
        var Iteraciones = default(int);
        bool PuedeAgua;
        bool PuedeTierra;


        short Map;
        short X;
        short Y;

        nIndex = OpenNPC(NroNPC); // Conseguimos un indice

        if (nIndex > Declaraciones.MAXNPCS)
            return;
        PuedeAgua = Declaraciones.Npclist[nIndex].flags.AguaValida != 0;
        PuedeTierra = Declaraciones.Npclist[nIndex].flags.TierraInvalida == 1 ? false : true;

        // Necesita ser respawned en un lugar especifico
        if (Extra.InMapBounds(OrigPos.Map, OrigPos.X, OrigPos.Y))
        {
            Map = OrigPos.Map;
            X = OrigPos.X;
            Y = OrigPos.Y;
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Npclist().Orig. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Declaraciones.Npclist[nIndex].Orig = OrigPos;
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Npclist().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Declaraciones.Npclist[nIndex].Pos = OrigPos;
        }

        else
        {
            Pos.Map = mapa; // mapa
            altpos.Map = mapa;

            while (!PosicionValida)
            {
                Pos.X = Convert.ToInt16(Matematicas.RandomNumber(Declaraciones.MinXBorder,
                    Declaraciones.MaxXBorder)); // Obtenemos posicion al azar en x
                Pos.Y = Convert.ToInt16(Matematicas.RandomNumber(Declaraciones.MinYBorder,
                    Declaraciones.MaxYBorder)); // Obtenemos posicion al azar en y

                Extra.ClosestLegalPos(ref Pos, ref newpos, ref PuedeAgua,
                    ref PuedeTierra); // Nos devuelve la posicion valida mas cercana
                if ((newpos.X != 0) & (newpos.Y != 0))
                {
                    altpos.X = newpos.X;
                    altpos.Y = newpos.Y;
                }
                // posicion alternativa (para evitar el anti respawn, pero intentando qeu si tenía que ser en el agua, sea en el agua.)
                else
                {
                    var argPuedeTierra = true;
                    Extra.ClosestLegalPos(ref Pos, ref newpos, ref PuedeAgua, ref argPuedeTierra);
                    if ((newpos.X != 0) & (newpos.Y != 0))
                    {
                        altpos.X = newpos.X;
                        altpos.Y = newpos.Y; // posicion alternativa (para evitar el anti respawn)
                    }
                }

                // Si X e Y son iguales a 0 significa que no se encontro posicion valida
                if (Extra.LegalPosNPC(newpos.Map, newpos.X, newpos.Y, Convert.ToByte(PuedeAgua)) &
                    !TCP.HayPCarea(ref newpos) & TestSpawnTrigger(ref newpos, ref PuedeAgua))
                {
                    // Asignamos las nuevas coordenas solo si son validas
                    Declaraciones.Npclist[nIndex].Pos.Map = newpos.Map;
                    Declaraciones.Npclist[nIndex].Pos.X = newpos.X;
                    Declaraciones.Npclist[nIndex].Pos.Y = newpos.Y;
                    PosicionValida = true;
                }
                else
                {
                    newpos.X = 0;
                    newpos.Y = 0;
                }


                // for debug
                Iteraciones = Iteraciones + 1;
                if (Iteraciones > Declaraciones.MAXSPAWNATTEMPS)
                {
                    if ((altpos.X != 0) & (altpos.Y != 0))
                    {
                        Map = altpos.Map;
                        X = altpos.X;
                        Y = altpos.Y;
                        Declaraciones.Npclist[nIndex].Pos.Map = Map;
                        Declaraciones.Npclist[nIndex].Pos.X = X;
                        Declaraciones.Npclist[nIndex].Pos.Y = Y;
                        MakeNPCChar(true, ref Map, ref nIndex, Map, X, Y);
                        return;
                    }

                    altpos.X = 50;
                    altpos.Y = 50;
                    var argPuedeAgua = false;
                    var argPuedeTierra1 = true;
                    Extra.ClosestLegalPos(ref altpos, ref newpos, ref argPuedeAgua, ref argPuedeTierra1);
                    if ((newpos.X != 0) & (newpos.Y != 0))
                    {
                        Declaraciones.Npclist[nIndex].Pos.Map = newpos.Map;
                        Declaraciones.Npclist[nIndex].Pos.X = newpos.X;
                        Declaraciones.Npclist[nIndex].Pos.Y = newpos.Y;
                        MakeNPCChar(true, ref newpos.Map, ref nIndex, newpos.Map, newpos.X, newpos.Y);
                        return;
                    }

                    QuitarNPC(nIndex);
                    var argdesc = Declaraciones.MAXSPAWNATTEMPS + " iteraciones en CrearNpc Mapa:" + mapa + " NroNpc:" +
                                  NroNPC;
                    General.LogError(ref argdesc);
                    return;
                }
            }

            // asignamos las nuevas coordenas
            Map = newpos.Map;
            X = Declaraciones.Npclist[nIndex].Pos.X;
            Y = Declaraciones.Npclist[nIndex].Pos.Y;
        }

        // Crea el NPC
        MakeNPCChar(true, ref Map, ref nIndex, Map, X, Y);
    }

    public static void MakeNPCChar(bool toMap, ref short sndIndex, ref short NpcIndex, short Map, short X, short Y)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short CharIndex;

        if (Declaraciones.Npclist[NpcIndex].character.CharIndex == 0)
        {
            CharIndex = UsUaRiOs.NextOpenCharIndex();
            Declaraciones.Npclist[NpcIndex].character.CharIndex = CharIndex;
            Declaraciones.CharList[CharIndex] = NpcIndex;
        }

        Declaraciones.MapData[Map, X, Y].NpcIndex = NpcIndex;

        if (!toMap)
        {
            Protocol.WriteCharacterCreate(sndIndex, Declaraciones.Npclist[NpcIndex].character.body,
                Declaraciones.Npclist[NpcIndex].character.Head, Declaraciones.Npclist[NpcIndex].character.heading,
                Declaraciones.Npclist[NpcIndex].character.CharIndex, Convert.ToByte(X), Convert.ToByte(Y), 0, 0, 0,
                0, 0, Constants.vbNullString, 0, 0);
            Protocol.FlushBuffer(sndIndex);
        }
        else
        {
            ModAreas.AgregarNpc(NpcIndex);
        }
    }

    public static void ChangeNPCChar(short NpcIndex, short body, short Head, Declaraciones.eHeading heading)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if (NpcIndex > 0)
        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex].character;
            withBlock.body = body;
            withBlock.Head = Head;
            withBlock.heading = heading;

            modSendData.SendData(modSendData.SendTarget.ToNPCArea, NpcIndex,
                Protocol.PrepareMessageCharacterChange(body, Head, heading, withBlock.CharIndex, 0, 0, 0, 0, 0));
        }
    }

    private static void EraseNPCChar(short NpcIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if (Declaraciones.Npclist[NpcIndex].character.CharIndex != 0)
            Declaraciones.CharList[Declaraciones.Npclist[NpcIndex].character.CharIndex] = 0;

        if (Declaraciones.Npclist[NpcIndex].character.CharIndex == Declaraciones.LastChar)
            while (Declaraciones.CharList[Declaraciones.LastChar] <= 0)
            {
                Declaraciones.LastChar = Convert.ToInt16(Declaraciones.LastChar - 1);
                if (Declaraciones.LastChar <= 1)
                    break;
            }

        // Quitamos del mapa
        Declaraciones.MapData[Declaraciones.Npclist[NpcIndex].Pos.Map, Declaraciones.Npclist[NpcIndex].Pos.X,
            Declaraciones.Npclist[NpcIndex].Pos.Y].NpcIndex = 0;

        // Actualizamos los clientes
        modSendData.SendData(modSendData.SendTarget.ToNPCArea, NpcIndex,
            Protocol.PrepareMessageCharacterRemove(Declaraciones.Npclist[NpcIndex].character.CharIndex));

        // Update la lista npc
        Declaraciones.Npclist[NpcIndex].character.CharIndex = 0;


        // update NumChars
        Declaraciones.NumChars = Convert.ToInt16(Declaraciones.NumChars - 1);
    }

    public static void MoveNPCChar(short NpcIndex, byte nHeading)
    {
        // ***************************************************
        // Autor: Unknown (orginal version)
        // Last Modification: 06/04/2009
        // 06/04/2009: ZaMa - Now npcs can force to change position with dead character
        // 01/08/2009: ZaMa - Now npcs can't force to chance position with a dead character if that means to change the terrain the character is in
        // ***************************************************

        try
        {
            Declaraciones.WorldPos nPos;
            short UserIndex;

            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                nPos = withBlock.Pos;
                Extra.HeadtoPos((Declaraciones.eHeading)nHeading, ref nPos);

                // es una posicion legal
                if (Extra.LegalPosNPC(withBlock.Pos.Map, nPos.X, nPos.Y, withBlock.flags.AguaValida,
                        withBlock.MaestroUser != 0))
                {
                    if ((withBlock.flags.AguaValida == 0) & General.HayAgua(withBlock.Pos.Map, nPos.X, nPos.Y))
                        return;
                    if ((withBlock.flags.TierraInvalida == 1) & !General.HayAgua(withBlock.Pos.Map, nPos.X, nPos.Y))
                        return;

                    UserIndex = Declaraciones.MapData[withBlock.Pos.Map, nPos.X, nPos.Y].UserIndex;
                    // Si hay un usuario a donde se mueve el npc, entonces esta muerto
                    if (UserIndex > 0)
                    {
                        // No se traslada caspers de agua a tierra
                        if (General.HayAgua(withBlock.Pos.Map, nPos.X, nPos.Y) &
                            !General.HayAgua(withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y))
                            return;
                        // No se traslada caspers de tierra a agua
                        if (!General.HayAgua(withBlock.Pos.Map, nPos.X, nPos.Y) &
                            General.HayAgua(withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y))
                            return;

                        {
                            ref var withBlock1 = ref Declaraciones.UserList[UserIndex];
                            // Actualizamos posicion y mapa
                            Declaraciones.MapData[withBlock1.Pos.Map, withBlock1.Pos.X, withBlock1.Pos.Y].UserIndex = 0;
                            withBlock1.Pos.X = Declaraciones.Npclist[NpcIndex].Pos.X;
                            withBlock1.Pos.Y = Declaraciones.Npclist[NpcIndex].Pos.Y;
                            Declaraciones.MapData[withBlock1.Pos.Map, withBlock1.Pos.X, withBlock1.Pos.Y].UserIndex =
                                UserIndex;

                            // Avisamos a los usuarios del area, y al propio usuario lo forzamos a moverse
                            modSendData.SendData(modSendData.SendTarget.ToPCAreaButIndex, UserIndex,
                                Protocol.PrepareMessageCharacterMove(
                                    Declaraciones.UserList[UserIndex].character.CharIndex,
                                    Convert.ToByte(withBlock1.Pos.X), Convert.ToByte(withBlock1.Pos.Y)));
                            Protocol.WriteForceCharMove(UserIndex,
                                UsUaRiOs.InvertHeading((Declaraciones.eHeading)nHeading));
                        }
                    }

                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, NpcIndex,
                        Protocol.PrepareMessageCharacterMove(withBlock.character.CharIndex, Convert.ToByte(nPos.X),
                            Convert.ToByte(nPos.Y)));

                    // Update map and user pos
                    Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].NpcIndex = 0;
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Npclist().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    withBlock.Pos = nPos;
                    withBlock.character.heading = (Declaraciones.eHeading)nHeading;
                    Declaraciones.MapData[withBlock.Pos.Map, nPos.X, nPos.Y].NpcIndex = NpcIndex;
                    ModAreas.CheckUpdateNeededNpc(NpcIndex, nHeading);
                }

                else if (withBlock.MaestroUser == 0)
                {
                    if (withBlock.Movement == AI.TipoAI.NpcPathfinding)
                        // Someone has blocked the npc's way, we must to seek a new path!
                        withBlock.PFINFO.PathLenght = 0;
                }
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in TestSpawnTrigger: " + ex.Message);
            var argdesc = "Error en move npc " + NpcIndex;
            General.LogError(ref argdesc);
        }
    }

    public static short NextOpenNPC()
    {
        short NextOpenNPCRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            int LoopC;

            for (LoopC = 1; LoopC <= Declaraciones.MAXNPCS + 1; LoopC++)
            {
                if (LoopC > Declaraciones.MAXNPCS)
                    break;
                if (!Declaraciones.Npclist[LoopC].flags.NPCActive)
                    break;
            }

            NextOpenNPCRet = Convert.ToInt16(LoopC);
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in NextOpenNPC: " + ex.Message);
            var argdesc = "Error en NextOpenNPC";
            General.LogError(ref argdesc);
        }

        return NextOpenNPCRet;
    }

    public static void NpcEnvenenarUser(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short N;
        N = Convert.ToInt16(Matematicas.RandomNumber(1, 100));
        if (N < 30)
        {
            Declaraciones.UserList[UserIndex].flags.Envenenado = 1;
            Protocol.WriteConsoleMsg(UserIndex, "¡¡La criatura te ha envenenado!!",
                Protocol.FontTypeNames.FONTTYPE_FIGHT);
        }
    }

    public static short SpawnNpc(short NpcIndex, ref Declaraciones.WorldPos Pos, bool FX, bool Respawn)
    {
        short SpawnNpcRet = default;
        // ***************************************************
        // Autor: Unknown (orginal version)
        // Last Modification: 06/15/2008
        // 23/01/2007 -> Pablo (ToxicWaste): Creates an NPC of the type Npcindex
        // 06/15/2008 -> Optimizé el codigo. (NicoNZ)
        // ***************************************************
        var newpos = default(Declaraciones.WorldPos);
        var altpos = default(Declaraciones.WorldPos);
        short nIndex;
        bool PosicionValida;
        bool PuedeAgua;
        bool PuedeTierra;


        short Map;
        short X;
        short Y;

        nIndex = OpenNPC(NpcIndex, Respawn); // Conseguimos un indice

        if (nIndex > Declaraciones.MAXNPCS)
        {
            SpawnNpcRet = 0;
            return SpawnNpcRet;
        }

        PuedeAgua = Declaraciones.Npclist[nIndex].flags.AguaValida != 0;
        PuedeTierra = !(Declaraciones.Npclist[nIndex].flags.TierraInvalida == 1);

        Extra.ClosestLegalPos(ref Pos, ref newpos, ref PuedeAgua,
            ref PuedeTierra); // Nos devuelve la posicion valida mas cercana
        var argPuedeTierra = true;
        Extra.ClosestLegalPos(ref Pos, ref altpos, ref PuedeAgua, ref argPuedeTierra);
        // Si X e Y son iguales a 0 significa que no se encontro posicion valida

        if ((newpos.X != 0) & (newpos.Y != 0))
        {
            // Asignamos las nuevas coordenas solo si son validas
            Declaraciones.Npclist[nIndex].Pos.Map = newpos.Map;
            Declaraciones.Npclist[nIndex].Pos.X = newpos.X;
            Declaraciones.Npclist[nIndex].Pos.Y = newpos.Y;
            PosicionValida = true;
        }
        else if ((altpos.X != 0) & (altpos.Y != 0))
        {
            Declaraciones.Npclist[nIndex].Pos.Map = altpos.Map;
            Declaraciones.Npclist[nIndex].Pos.X = altpos.X;
            Declaraciones.Npclist[nIndex].Pos.Y = altpos.Y;
            PosicionValida = true;
        }
        else
        {
            PosicionValida = false;
        }

        if (!PosicionValida)
        {
            QuitarNPC(nIndex);
            SpawnNpcRet = 0;
            return SpawnNpcRet;
        }

        // asignamos las nuevas coordenas
        Map = newpos.Map;
        X = Declaraciones.Npclist[nIndex].Pos.X;
        Y = Declaraciones.Npclist[nIndex].Pos.Y;

        // Crea el NPC
        MakeNPCChar(true, ref Map, ref nIndex, Map, X, Y);

        if (FX)
        {
            modSendData.SendData(modSendData.SendTarget.ToNPCArea, nIndex,
                Protocol.PrepareMessagePlayWave(Declaraciones.SND_WARP, Convert.ToByte(X), Convert.ToByte(Y)));
            modSendData.SendData(modSendData.SendTarget.ToNPCArea, nIndex,
                Protocol.PrepareMessageCreateFX(Declaraciones.Npclist[nIndex].character.CharIndex,
                    (short)Declaraciones.FXIDs.FXWARP, 0));
        }

        SpawnNpcRet = nIndex;
        return SpawnNpcRet;
    }

    public static void ReSpawnNpc(ref Declaraciones.npc MiNPC)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if (MiNPC.flags.Respawn == 0)
            CrearNPC(ref MiNPC.Numero, ref MiNPC.Pos.Map, ref MiNPC.Orig);
    }

    private static void NPCTirarOro(ref Declaraciones.npc MiNPC)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // SI EL NPC TIENE ORO LO TIRAMOS
        Declaraciones.Obj MiObj;
        int MiAux;
        if (MiNPC.GiveGLD > 0)
        {
            MiAux = MiNPC.GiveGLD;
            while (MiAux > Declaraciones.MAX_INVENTORY_OBJS)
            {
                MiObj.Amount = Declaraciones.MAX_INVENTORY_OBJS;
                MiObj.ObjIndex = Declaraciones.iORO;
                var argNotPirata = true;
                InvNpc.TirarItemAlPiso(ref MiNPC.Pos, ref MiObj, ref argNotPirata);
                MiAux = MiAux - Declaraciones.MAX_INVENTORY_OBJS;
            }

            if (MiAux > 0)
            {
                MiObj.Amount = Convert.ToInt16(MiAux);
                MiObj.ObjIndex = Declaraciones.iORO;
                var argNotPirata1 = true;
                InvNpc.TirarItemAlPiso(ref MiNPC.Pos, ref MiObj, ref argNotPirata1);
            }
        }
    }

    public static short OpenNPC(short NpcNumber, bool Respawn = true)
    {
        short OpenNPCRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // ###################################################
        // #               ATENCION PELIGRO                  #
        // ###################################################
        // 
        // ¡¡¡¡ NO USAR GetVar PARA LEER LOS NPCS !!!!
        // 
        // El que ose desafiar esta LEY, se las tendrá que ver
        // conmigo. Para leer los NPCS se deberá usar la
        // nueva clase clsIniReader.
        // 
        // Alejo
        // 
        // ###################################################
        short NpcIndex;
        clsIniReader Leer;
        int LoopC;
        string ln;
        string aux;

        Leer = General.LeerNPCs;

        // If requested index is invalid, abort
        if (!Leer.KeyExists("NPC" + NpcNumber))
        {
            OpenNPCRet = Declaraciones.MAXNPCS + 1;
            return OpenNPCRet;
        }

        NpcIndex = NextOpenNPC();

        if (NpcIndex > Declaraciones.MAXNPCS) // Limite de npcs
        {
            OpenNPCRet = NpcIndex;
            return OpenNPCRet;
        }

        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
            withBlock.Numero = NpcNumber;
            withBlock.name = Leer.GetValue("NPC" + NpcNumber, "Name");
            withBlock.desc = Leer.GetValue("NPC" + NpcNumber, "Desc");

            withBlock.Movement =
                (AI.TipoAI)Math.Round(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Movement")));
            withBlock.flags.OldMovement = withBlock.Movement;

            withBlock.flags.AguaValida =
                Convert.ToByte(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "AguaValida")));
            withBlock.flags.TierraInvalida =
                Convert.ToByte(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "TierraInValida")));
            withBlock.flags.Faccion = Convert.ToByte(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Faccion")));
            withBlock.flags.AtacaDoble =
                Convert.ToByte(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "AtacaDoble")));

            withBlock.NPCtype =
                (Declaraciones.eNPCType)Math.Round(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "NpcType")));

            withBlock.character.body = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Body")));
            withBlock.character.Head = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Head")));
            withBlock.character.heading =
                (Declaraciones.eHeading)Math.Round(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Heading")));

            withBlock.Attackable = Convert.ToByte(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Attackable")));
            withBlock.Comercia = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Comercia")));
            withBlock.Hostile = Convert.ToByte(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Hostile")));
            withBlock.flags.OldHostil = withBlock.Hostile;

            withBlock.GiveEXP = (int)Math.Round(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "GiveEXP")));

            withBlock.flags.ExpCount = withBlock.GiveEXP;

            withBlock.Veneno = Convert.ToByte(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Veneno")));

            withBlock.flags.Domable = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Domable")));

            withBlock.GiveGLD = (int)Math.Round(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "GiveGLD")));

            withBlock.PoderAtaque =
                (int)Math.Round(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "PoderAtaque")));
            withBlock.PoderEvasion =
                (int)Math.Round(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "PoderEvasion")));

            withBlock.InvReSpawn = Convert.ToByte(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "InvReSpawn")));

            {
                ref var withBlock1 = ref withBlock.Stats;
                withBlock1.MaxHp = (int)Math.Round(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "MaxHP")));
                withBlock1.MinHp = (int)Math.Round(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "MinHP")));
                withBlock1.MaxHIT = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "MaxHIT")));
                withBlock1.MinHIT = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "MinHIT")));
                withBlock1.def = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "DEF")));
                withBlock1.defM = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "DEFm")));
                withBlock1.Alineacion =
                    Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Alineacion")));
            }

            withBlock.Invent.NroItems =
                Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "NROITEMS")));
            var loopTo = (int)withBlock.Invent.NroItems;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                ln = Leer.GetValue("NPC" + NpcNumber, "Obj" + LoopC);
                withBlock.Invent.userObj[LoopC].ObjIndex =
                    Convert.ToInt16(Migration.ParseVal(General.ReadField(1, ref ln, 45)));
                withBlock.Invent.userObj[LoopC].Amount =
                    Convert.ToInt16(Migration.ParseVal(General.ReadField(2, ref ln, 45)));
            }

            for (LoopC = 1; LoopC <= Declaraciones.MAX_NPC_DROPS; LoopC++)
            {
                ln = Leer.GetValue("NPC" + NpcNumber, "Drop" + LoopC);
                withBlock.Drop[LoopC].ObjIndex = Convert.ToInt16(Migration.ParseVal(General.ReadField(1, ref ln, 45)));
                withBlock.Drop[LoopC].Amount = Convert.ToInt32(Migration.ParseVal(General.ReadField(2, ref ln, 45)));
            }


            withBlock.flags.LanzaSpells =
                Convert.ToByte(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "LanzaSpells")));
            // UPGRADE_WARNING: El límite inferior de la matriz .Spells ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            if (withBlock.flags.LanzaSpells > 0)
                withBlock.Spells = new short[withBlock.flags.LanzaSpells + 1];
            var loopTo1 = (int)withBlock.flags.LanzaSpells;
            for (LoopC = 1; LoopC <= loopTo1; LoopC++)
                withBlock.Spells[LoopC] =
                    Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Sp" + LoopC)));

            if (withBlock.NPCtype == Declaraciones.eNPCType.Entrenador)
            {
                withBlock.NroCriaturas =
                    Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "NroCriaturas")));
                // UPGRADE_WARNING: El límite inferior de la matriz .Criaturas ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                withBlock.Criaturas = new Declaraciones.tCriaturasEntrenador[withBlock.NroCriaturas + 1];
                var loopTo2 = (int)withBlock.NroCriaturas;
                for (LoopC = 1; LoopC <= loopTo2; LoopC++)
                {
                    withBlock.Criaturas[LoopC].NpcIndex =
                        Convert.ToInt16(Leer.GetValue("NPC" + NpcNumber, "CI" + LoopC));
                    withBlock.Criaturas[LoopC].NpcName = Leer.GetValue("NPC" + NpcNumber, "CN" + LoopC);
                }
            }

            {
                ref var withBlock2 = ref withBlock.flags;
                withBlock2.NPCActive = true;

                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Respawn. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                if (Respawn)
                    withBlock2.Respawn =
                        Convert.ToByte(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "ReSpawn")));
                else
                    withBlock2.Respawn = 1;

                withBlock2.BackUp = Convert.ToByte(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "BackUp")));
                withBlock2.RespawnOrigPos =
                    Convert.ToByte(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "OrigPos")));
                withBlock2.AfectaParalisis =
                    Convert.ToByte(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "AfectaParalisis")));

                withBlock2.Snd1 = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Snd1")));
                withBlock2.Snd2 = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Snd2")));
                withBlock2.Snd3 = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Snd3")));
            }

            // <<<<<<<<<<<<<< Expresiones >>>>>>>>>>>>>>>>
            withBlock.NroExpresiones = Convert.ToByte(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "NROEXP")));
            // UPGRADE_WARNING: El límite inferior de la matriz .Expresiones ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            if (withBlock.NroExpresiones > 0)
                withBlock.Expresiones = new string[withBlock.NroExpresiones + 1];
            var loopTo3 = (int)withBlock.NroExpresiones;
            for (LoopC = 1; LoopC <= loopTo3; LoopC++)
                withBlock.Expresiones[LoopC] = Leer.GetValue("NPC" + NpcNumber, "Exp" + LoopC);
            // <<<<<<<<<<<<<< Expresiones >>>>>>>>>>>>>>>>

            // Tipo de items con los que comercia
            withBlock.TipoItems = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "TipoItems")));

            withBlock.Ciudad = Convert.ToByte(Migration.ParseVal(Leer.GetValue("NPC" + NpcNumber, "Ciudad")));
        }

        // Update contadores de NPCs
        if (NpcIndex > Declaraciones.LastNPC)
            Declaraciones.LastNPC = NpcIndex;
        Declaraciones.NumNPCs = Convert.ToInt16(Declaraciones.NumNPCs + 1);

        // Devuelve el nuevo Indice
        OpenNPCRet = NpcIndex;
        return OpenNPCRet;
    }

    public static void DoFollow(short NpcIndex, string UserName)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
            if (withBlock.flags.Follow)
            {
                withBlock.flags.AttackedBy = Constants.vbNullString;
                withBlock.flags.Follow = false;
                withBlock.Movement = withBlock.flags.OldMovement;
                withBlock.Hostile = withBlock.flags.OldHostil;
            }
            else
            {
                withBlock.flags.AttackedBy = UserName;
                withBlock.flags.Follow = true;
                withBlock.Movement = AI.TipoAI.NPCDEFENSA;
                withBlock.Hostile = 0;
            }
        }
    }

    public static void FollowAmo(short NpcIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
            withBlock.flags.Follow = true;
            withBlock.Movement = AI.TipoAI.SigueAmo;
            withBlock.Hostile = 0;
            withBlock.Target = 0;
            withBlock.TargetNPC = 0;
        }
    }

    public static void ValidarPermanenciaNpc(short NpcIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // Chequea si el npc continua perteneciendo a algún usuario
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
            if (modNuevoTimer.IntervaloPerdioNpc(withBlock.Owner))
                UsUaRiOs.PerdioNpc(withBlock.Owner);
        }
    }
}