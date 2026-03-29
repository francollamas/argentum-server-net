using System;
using Microsoft.VisualBasic;

namespace Legacy;

internal static class SistemaCombate
{
    public const byte MAXDISTANCIAARCO = 18;
    public const byte MAXDISTANCIAMAGIA = 18;

    public static int MinimoInt(int a, int b)
    {
        int MinimoIntRet = default;
        if (a > b)
            MinimoIntRet = b;
        else
            MinimoIntRet = a;

        return MinimoIntRet;
    }

    public static int MaximoInt(int a, int b)
    {
        int MaximoIntRet = default;
        if (a > b)
            MaximoIntRet = a;
        else
            MaximoIntRet = b;

        return MaximoIntRet;
    }

    private static int PoderEvasionEscudo(short UserIndex)
    {
        int PoderEvasionEscudoRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        PoderEvasionEscudoRet =
            Convert.ToInt32(Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Defensa] *
                Declaraciones.modClase[(int)Declaraciones.UserList[UserIndex].clase].Escudo / 2d);
        return PoderEvasionEscudoRet;
    }

    private static int PoderEvasion(short UserIndex)
    {
        int PoderEvasionRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************
        int lTemp;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            lTemp = Convert.ToInt32(
                (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Tacticas] +
                 withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Tacticas] / 33d *
                 withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad]) *
                Declaraciones.modClase[(int)withBlock.clase].Evasion);

            PoderEvasionRet = Convert.ToInt32(lTemp + 2.5d * MaximoInt(withBlock.Stats.ELV - 12, 0));
        }

        return PoderEvasionRet;
    }

    private static int PoderAtaqueArma(short UserIndex)
    {
        int PoderAtaqueArmaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int PoderAtaqueTemp;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Armas] < 31)
                PoderAtaqueTemp = Convert.ToInt32(withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Armas] *
                                                  Declaraciones.modClase[(int)withBlock.clase].AtaqueArmas);
            else if (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Armas] < 61)
                PoderAtaqueTemp = Convert.ToInt32(
                    (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Armas] +
                     withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad]) *
                    Declaraciones.modClase[(int)withBlock.clase].AtaqueArmas);
            else if (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Armas] < 91)
                PoderAtaqueTemp = Convert.ToInt32(
                    (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Armas] +
                     2 * withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad]) *
                    Declaraciones.modClase[(int)withBlock.clase].AtaqueArmas);
            else
                PoderAtaqueTemp = Convert.ToInt32(
                    (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Armas] +
                     3 * withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad]) *
                    Declaraciones.modClase[(int)withBlock.clase].AtaqueArmas);

            PoderAtaqueArmaRet = Convert.ToInt32(PoderAtaqueTemp + 2.5d * MaximoInt(withBlock.Stats.ELV - 12, 0));
        }

        return PoderAtaqueArmaRet;
    }

    private static int PoderAtaqueProyectil(short UserIndex)
    {
        int PoderAtaqueProyectilRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int PoderAtaqueTemp;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Proyectiles] < 31)
                PoderAtaqueTemp = Convert.ToInt32(withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Proyectiles] *
                                                  Declaraciones.modClase[(int)withBlock.clase]
                                                      .AtaqueProyectiles);
            else if (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Proyectiles] < 61)
                PoderAtaqueTemp = Convert.ToInt32(
                    (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Proyectiles] +
                     withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad]) *
                    Declaraciones.modClase[(int)withBlock.clase].AtaqueProyectiles);
            else if (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Proyectiles] < 91)
                PoderAtaqueTemp = Convert.ToInt32(
                    (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Proyectiles] +
                     2 * withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad]) *
                    Declaraciones.modClase[(int)withBlock.clase].AtaqueProyectiles);
            else
                PoderAtaqueTemp = Convert.ToInt32(
                    (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Proyectiles] +
                     3 * withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad]) *
                    Declaraciones.modClase[(int)withBlock.clase].AtaqueProyectiles);

            PoderAtaqueProyectilRet = Convert.ToInt32(PoderAtaqueTemp + 2.5d * MaximoInt(withBlock.Stats.ELV - 12, 0));
        }

        return PoderAtaqueProyectilRet;
    }

    private static int PoderAtaqueWrestling(short UserIndex)
    {
        int PoderAtaqueWrestlingRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int PoderAtaqueTemp;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Wrestling] < 31)
                PoderAtaqueTemp = Convert.ToInt32(withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Wrestling] *
                                                  Declaraciones.modClase[(int)withBlock.clase].AtaqueWrestling);
            else if (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Wrestling] < 61)
                PoderAtaqueTemp = Convert.ToInt32(
                    (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Wrestling] +
                     withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad]) *
                    Declaraciones.modClase[(int)withBlock.clase].AtaqueWrestling);
            else if (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Wrestling] < 91)
                PoderAtaqueTemp = Convert.ToInt32(
                    (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Wrestling] +
                     2 * withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad]) *
                    Declaraciones.modClase[(int)withBlock.clase].AtaqueWrestling);
            else
                PoderAtaqueTemp = Convert.ToInt32(
                    (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Wrestling] +
                     3 * withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad]) *
                    Declaraciones.modClase[(int)withBlock.clase].AtaqueWrestling);

            PoderAtaqueWrestlingRet = Convert.ToInt32(PoderAtaqueTemp + 2.5d * MaximoInt(withBlock.Stats.ELV - 12, 0));
        }

        return PoderAtaqueWrestlingRet;
    }

    public static bool UserImpactoNpc(short UserIndex, short NpcIndex)
    {
        bool UserImpactoNpcRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int PoderAtaque;
        short Arma;
        Declaraciones.eSkill Skill;
        int ProbExito;

        Arma = Declaraciones.UserList[UserIndex].Invent.WeaponEqpObjIndex;

        if (Arma > 0) // Usando un arma
        {
            if (Declaraciones.objData[Arma].proyectil == 1)
            {
                PoderAtaque = PoderAtaqueProyectil(UserIndex);
                Skill = Declaraciones.eSkill.Proyectiles;
            }
            else
            {
                PoderAtaque = PoderAtaqueArma(UserIndex);
                Skill = Declaraciones.eSkill.Armas;
            }
        }
        else // Peleando con puños
        {
            PoderAtaque = PoderAtaqueWrestling(UserIndex);
            Skill = Declaraciones.eSkill.Wrestling;
        }

        // Chances are rounded
        ProbExito = MaximoInt(10,
            MinimoInt(90, Convert.ToInt32(50d + (PoderAtaque - Declaraciones.Npclist[NpcIndex].PoderEvasion) * 0.4d)));

        UserImpactoNpcRet = Matematicas.RandomNumber(1, 100) <= ProbExito;

        if (UserImpactoNpcRet)
            UsUaRiOs.SubirSkill(UserIndex, (short)Skill, true);
        else
            UsUaRiOs.SubirSkill(UserIndex, (short)Skill, false);

        return UserImpactoNpcRet;
    }

    public static bool NpcImpacto(short NpcIndex, short UserIndex)
    {
        bool NpcImpactoRet = default;
        // *************************************************
        // Author: Unknown
        // Last modified: 03/15/2006
        // Revisa si un NPC logra impactar a un user o no
        // 03/15/2006 Maraxus - Evité una división por cero que eliminaba NPCs
        // *************************************************
        bool Rechazo;
        int ProbRechazo;
        int ProbExito;
        int UserEvasion;
        int NpcPoderAtaque;
        int PoderEvasioEscudo;
        int SkillTacticas;
        int SkillDefensa;

        UserEvasion = PoderEvasion(UserIndex);
        NpcPoderAtaque = Declaraciones.Npclist[NpcIndex].PoderAtaque;
        PoderEvasioEscudo = PoderEvasionEscudo(UserIndex);

        SkillTacticas = Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Tacticas];
        SkillDefensa = Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Defensa];

        // Esta usando un escudo ???
        if (Declaraciones.UserList[UserIndex].Invent.EscudoEqpObjIndex > 0)
            UserEvasion = UserEvasion + PoderEvasioEscudo;

        // Chances are rounded
        ProbExito = MaximoInt(10, MinimoInt(90, Convert.ToInt32(50d + (NpcPoderAtaque - UserEvasion) * 0.4d)));

        NpcImpactoRet = Matematicas.RandomNumber(1, 100) <= ProbExito;

        // el usuario esta usando un escudo ???
        if (Declaraciones.UserList[UserIndex].Invent.EscudoEqpObjIndex > 0)
            if (!NpcImpactoRet)
                if (SkillDefensa + SkillTacticas > 0) // Evitamos división por cero
                {
                    // Chances are rounded
                    ProbRechazo = MaximoInt(10,
                        MinimoInt(90, Convert.ToInt32(100 * SkillDefensa / (double)(SkillDefensa + SkillTacticas))));
                    Rechazo = Matematicas.RandomNumber(1, 100) <= ProbRechazo;

                    if (Rechazo)
                    {
                        // Se rechazo el ataque con el escudo
                        modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                            Protocol.PrepareMessagePlayWave(Declaraciones.SND_ESCUDO,
                                Convert.ToByte(Declaraciones.UserList[UserIndex].Pos.X),
                                Convert.ToByte(Declaraciones.UserList[UserIndex].Pos.Y)));
                        Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.BlockedWithShieldUser);
                        // Call WriteBlockedWithShieldUser(UserIndex)
                        UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Defensa, true);
                    }
                    else
                    {
                        UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Defensa, false);
                    }
                }

        return NpcImpactoRet;
    }

    public static int CalcularDaño(short UserIndex, short NpcIndex = 0)
    {
        int CalcularDañoRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: 01/04/2010 (ZaMa)
        // 01/04/2010: ZaMa - Modifico el daño de wrestling.
        // 01/04/2010: ZaMa - Agrego bonificadores de wrestling para los guantes.
        // ***************************************************
        int DañoArma;
        int DañoUsuario;
        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Arma, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Declaraciones.ObjData Arma;
        float ModifClase;
        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura proyectil, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Declaraciones.ObjData proyectil;
        int DañoMaxArma;
        int DañoMinArma;
        short ObjIndex;

        // 'sacar esto si no queremos q la matadracos mate el Dragon si o si
        bool matoDragon;
        matoDragon = false;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.Invent.WeaponEqpObjIndex > 0)
            {
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Arma. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                Arma = Declaraciones.objData[withBlock.Invent.WeaponEqpObjIndex];

                // Ataca a un npc?
                if (NpcIndex > 0)
                {
                    if (Arma.proyectil == 1)
                    {
                        ModifClase =
                            Convert.ToSingle(Declaraciones.modClase[(int)withBlock.clase].DañoProyectiles);
                        DañoArma = Matematicas.RandomNumber(Arma.MinHIT, Arma.MaxHIT);
                        DañoMaxArma = Arma.MaxHIT;

                        if (Arma.Municion == 1)
                        {
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto proyectil. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            proyectil = Declaraciones.objData[withBlock.Invent.MunicionEqpObjIndex];
                            DañoArma = DañoArma + Matematicas.RandomNumber(proyectil.MinHIT, proyectil.MaxHIT);
                            // For some reason this isn't done...
                            // DañoMaxArma = DañoMaxArma + proyectil.MaxHIT
                        }
                    }
                    else
                    {
                        ModifClase = Convert.ToSingle(Declaraciones.modClase[(int)withBlock.clase].DañoArmas);

                        if (withBlock.Invent.WeaponEqpObjIndex ==
                            Declaraciones.EspadaMataDragonesIndex) // Usa la mata Dragones?
                        {
                            if (Declaraciones.Npclist[NpcIndex].NPCtype ==
                                Declaraciones.eNPCType.DRAGON) // Ataca Dragon?
                            {
                                DañoArma = Matematicas.RandomNumber(Arma.MinHIT, Arma.MaxHIT);
                                DañoMaxArma = Arma.MaxHIT;
                                matoDragon = true; // 'sacar esto si no queremos q la matadracos mate el Dragon si o si
                            }
                            else // Sino es Dragon daño es 1
                            {
                                DañoArma = 1;
                                DañoMaxArma = 1;
                            }
                        }
                        else
                        {
                            DañoArma = Matematicas.RandomNumber(Arma.MinHIT, Arma.MaxHIT);
                            DañoMaxArma = Arma.MaxHIT;
                        }
                    }
                }
                else if (Arma.proyectil == 1) // Ataca usuario
                {
                    ModifClase = Convert.ToSingle(Declaraciones.modClase[(int)withBlock.clase].DañoProyectiles);
                    DañoArma = Matematicas.RandomNumber(Arma.MinHIT, Arma.MaxHIT);
                    DañoMaxArma = Arma.MaxHIT;

                    if (Arma.Municion == 1)
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto proyectil. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        proyectil = Declaraciones.objData[withBlock.Invent.MunicionEqpObjIndex];
                        DañoArma = DañoArma + Matematicas.RandomNumber(proyectil.MinHIT, proyectil.MaxHIT);
                        // For some reason this isn't done...
                        // DañoMaxArma = DañoMaxArma + proyectil.MaxHIT
                    }
                }
                else
                {
                    ModifClase = Convert.ToSingle(Declaraciones.modClase[(int)withBlock.clase].DañoArmas);

                    if (withBlock.Invent.WeaponEqpObjIndex == Declaraciones.EspadaMataDragonesIndex)
                    {
                        ModifClase = Convert.ToSingle(Declaraciones.modClase[(int)withBlock.clase].DañoArmas);
                        DañoArma = 1; // Si usa la espada mataDragones daño es 1
                        DañoMaxArma = 1;
                    }
                    else
                    {
                        DañoArma = Matematicas.RandomNumber(Arma.MinHIT, Arma.MaxHIT);
                        DañoMaxArma = Arma.MaxHIT;
                    }
                }
            }
            else
            {
                ModifClase = Convert.ToSingle(Declaraciones.modClase[(int)withBlock.clase].DañoWrestling);

                // Daño sin guantes
                DañoMinArma = 4;
                DañoMaxArma = 9;

                // Plus de guantes (en slot de anillo)
                ObjIndex = withBlock.Invent.AnilloEqpObjIndex;
                if (ObjIndex > 0)
                    if (Declaraciones.objData[ObjIndex].Guante == 1)
                    {
                        DañoMinArma = DañoMinArma + Declaraciones.objData[ObjIndex].MinHIT;
                        DañoMaxArma = DañoMaxArma + Declaraciones.objData[ObjIndex].MaxHIT;
                    }

                DañoArma = Matematicas.RandomNumber(DañoMinArma, DañoMaxArma);
            }

            DañoUsuario = Matematicas.RandomNumber(withBlock.Stats.MinHIT, withBlock.Stats.MaxHIT);

            // 'sacar esto si no queremos q la matadracos mate el Dragon si o si
            if (matoDragon)
                CalcularDañoRet = Declaraciones.Npclist[NpcIndex].Stats.MinHp +
                                  Declaraciones.Npclist[NpcIndex].Stats.def;
            else
                CalcularDañoRet = Convert.ToInt32((3 * DañoArma + DañoMaxArma / 5d * MaximoInt(0,
                                                       Convert.ToInt32(
                                                           withBlock.Stats.UserAtributos[
                                                               (int)Declaraciones.eAtributos.Fuerza] - 15)) +
                                                   DañoUsuario) * ModifClase);
        }

        return CalcularDañoRet;
    }

    public static void UserDañoNpc(short UserIndex, short NpcIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 07/04/2010 (ZaMa)
        // 25/01/2010: ZaMa - Agrego poder acuchillar npcs.
        // 07/04/2010: ZaMa - Los asesinos apuñalan acorde al daño base sin descontar la defensa del npc.
        // ***************************************************

        int daño;
        int DañoBase;

        DañoBase = CalcularDaño(UserIndex, NpcIndex);

        // esta navegando? si es asi le sumamos el daño del barco
        if (Declaraciones.UserList[UserIndex].flags.Navegando == 1)
            if (Declaraciones.UserList[UserIndex].Invent.BarcoObjIndex > 0)
                DañoBase = DañoBase + Matematicas.RandomNumber(
                    Declaraciones.objData[Declaraciones.UserList[UserIndex].Invent.BarcoObjIndex].MinHIT,
                    Declaraciones.objData[Declaraciones.UserList[UserIndex].Invent.BarcoObjIndex].MaxHIT);

        short j;
        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
            daño = DañoBase - withBlock.Stats.def;

            if (daño < 0)
                daño = 0;

            // Call WriteUserHitNPC(UserIndex, daño)
            Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.UserHitNPC, daño);
            CalcularDarExp(UserIndex, NpcIndex, daño);
            withBlock.Stats.MinHp = withBlock.Stats.MinHp - daño;

            if (withBlock.Stats.MinHp > 0)
            {
                // Trata de apuñalar por la espalda al enemigo
                if (UsUaRiOs.PuedeApuñalar(UserIndex))
                    Trabajo.DoApuñalar(UserIndex, NpcIndex, 0, Convert.ToInt16(DañoBase));

                // trata de dar golpe crítico
                Trabajo.DoGolpeCritico(UserIndex, NpcIndex, 0, Convert.ToInt16(daño));

                if (UsUaRiOs.PuedeAcuchillar(UserIndex))
                    Trabajo.DoAcuchillar(UserIndex, NpcIndex, 0, Convert.ToInt16(daño));
            }


            if (withBlock.Stats.MinHp <= 0)
            {
                // Si era un Dragon perdemos la espada mataDragones
                if (withBlock.NPCtype == Declaraciones.eNPCType.DRAGON)
                {
                    // Si tiene equipada la matadracos se la sacamos
                    if (Declaraciones.UserList[UserIndex].Invent.WeaponEqpObjIndex ==
                        Declaraciones.EspadaMataDragonesIndex)
                        Trabajo.QuitarObjetos(Declaraciones.EspadaMataDragonesIndex, 1, UserIndex);
                    if (withBlock.Stats.MaxHp > 100000)
                        General.LogDesarrollo(Declaraciones.UserList[UserIndex].name + " mató un dragón");
                }

                // Para que las mascotas no sigan intentando luchar y
                // comiencen a seguir al amo
                for (j = 1; j <= Declaraciones.MAXMASCOTAS; j++)
                    if (Declaraciones.UserList[UserIndex].MascotasIndex[j] > 0)
                        if (Declaraciones.Npclist[Declaraciones.UserList[UserIndex].MascotasIndex[j]].TargetNPC ==
                            NpcIndex)
                        {
                            Declaraciones.Npclist[Declaraciones.UserList[UserIndex].MascotasIndex[j]].TargetNPC = 0;
                            Declaraciones.Npclist[Declaraciones.UserList[UserIndex].MascotasIndex[j]].Movement =
                                AI.TipoAI.SigueAmo;
                        }

                NPCs.MuereNpc(NpcIndex, UserIndex);
            }
        }
    }

    public static void NpcDaño(short NpcIndex, short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short daño;
        short Lugar;
        var absorbido = default(short);
        var defbarco = default(short);
        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Declaraciones.ObjData obj;

        daño = Convert.ToInt16(Matematicas.RandomNumber(Declaraciones.Npclist[NpcIndex].Stats.MinHIT,
            Declaraciones.Npclist[NpcIndex].Stats.MaxHIT));

        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj2, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Declaraciones.ObjData Obj2;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if ((withBlock.flags.Navegando == 1) & (withBlock.Invent.BarcoObjIndex > 0))
            {
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto obj. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                obj = Declaraciones.objData[withBlock.Invent.BarcoObjIndex];
                defbarco = Convert.ToInt16(Matematicas.RandomNumber(obj.MinDef, obj.MaxDef));
            }

            Lugar = Convert.ToInt16(Matematicas.RandomNumber(Convert.ToInt32((int)Declaraciones.PartesCuerpo.bCabeza),
                Convert.ToInt32((int)Declaraciones.PartesCuerpo.bTorso)));

            switch (Lugar)
            {
                case var @case when @case == Convert.ToInt16((int)Declaraciones.PartesCuerpo.bCabeza):
                {
                    // Si tiene casco absorbe el golpe
                    if (withBlock.Invent.CascoEqpObjIndex > 0)
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto obj. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        obj = Declaraciones.objData[withBlock.Invent.CascoEqpObjIndex];
                        absorbido = Convert.ToInt16(Matematicas.RandomNumber(obj.MinDef, obj.MaxDef));
                    }

                    break;
                }

                default:
                {
                    // Si tiene armadura absorbe el golpe
                    if (withBlock.Invent.ArmourEqpObjIndex > 0)
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto obj. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        obj = Declaraciones.objData[withBlock.Invent.ArmourEqpObjIndex];
                        if (withBlock.Invent.EscudoEqpObjIndex != 0)
                        {
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj2. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            Obj2 = Declaraciones.objData[withBlock.Invent.EscudoEqpObjIndex];
                            absorbido = Convert.ToInt16(Matematicas.RandomNumber(obj.MinDef + Obj2.MinDef,
                                obj.MaxDef + Obj2.MaxDef));
                        }
                        else
                        {
                            absorbido = Convert.ToInt16(
                                Matematicas.RandomNumber(obj.MinDef, obj.MaxDef));
                        }
                    }

                    break;
                }
            }

            absorbido = (short)(absorbido + defbarco);
            daño = (short)(daño - absorbido);
            if (daño < 1)
                daño = 1;

            Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.NPCHitUser, Lugar, daño);
            // Call WriteNPCHitUser(UserIndex, Lugar, daño)

            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                withBlock.Stats.MinHp = (short)(withBlock.Stats.MinHp - daño);

            if (withBlock.flags.Meditando)
                if (daño > Conversion.Fix(withBlock.Stats.MinHp / 100d *
                                          withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Inteligencia] *
                                          withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Meditar] / 100d * 12d /
                                          (Matematicas.RandomNumber(0, 5) + 7)))
                {
                    withBlock.flags.Meditando = false;
                    Protocol.WriteMeditateToggle(UserIndex);
                    Protocol.WriteConsoleMsg(UserIndex, "Dejas de meditar.", Protocol.FontTypeNames.FONTTYPE_INFO);
                    withBlock.character.FX = 0;
                    withBlock.character.loops = 0;
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                        Protocol.PrepareMessageCreateFX(withBlock.character.CharIndex, 0, 0));
                }

            // Muere el usuario
            if (withBlock.Stats.MinHp <= 0)
            {
                Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.NPCKillUser);
                // Call WriteNPCKillUser(UserIndex) ' Le informamos que ha muerto ;)

                // Si lo mato un guardia
                if (ES.criminal(UserIndex) &
                    (Declaraciones.Npclist[NpcIndex].NPCtype == Declaraciones.eNPCType.GuardiaReal))
                {
                    RestarCriminalidad(UserIndex);
                    if (!ES.criminal(UserIndex) & (withBlock.Faccion.FuerzasCaos == 1))
                    {
                        var argExpulsado = true;
                        ModFacciones.ExpulsarFaccionCaos(UserIndex, ref argExpulsado);
                    }
                }

                if (Declaraciones.Npclist[NpcIndex].MaestroUser > 0)
                {
                    AllFollowAmo(Declaraciones.Npclist[NpcIndex].MaestroUser);
                }
                // Al matarlo no lo sigue mas
                else if (Declaraciones.Npclist[NpcIndex].Stats.Alineacion == 0)
                {
                    Declaraciones.Npclist[NpcIndex].Movement = Declaraciones.Npclist[NpcIndex].flags.OldMovement;
                    Declaraciones.Npclist[NpcIndex].Hostile = Declaraciones.Npclist[NpcIndex].flags.OldHostil;
                    Declaraciones.Npclist[NpcIndex].flags.AttackedBy = Constants.vbNullString;
                }

                UsUaRiOs.UserDie(UserIndex);
            }
        }
    }

    public static void RestarCriminalidad(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        bool EraCriminal;
        EraCriminal = ES.criminal(UserIndex);

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].Reputacion;
            if (withBlock.BandidoRep > 0)
            {
                withBlock.BandidoRep = withBlock.BandidoRep - Declaraciones.vlASALTO;
                if (withBlock.BandidoRep < 0)
                    withBlock.BandidoRep = 0;
            }
            else if (withBlock.LadronesRep > 0)
            {
                withBlock.LadronesRep = withBlock.LadronesRep - Declaraciones.vlCAZADOR * 10;
                if (withBlock.LadronesRep < 0)
                    withBlock.LadronesRep = 0;
            }
        }

        if (EraCriminal & !ES.criminal(UserIndex)) UsUaRiOs.RefreshCharStatus(UserIndex);
    }

    public static void CheckPets(short NpcIndex, short UserIndex, bool CheckElementales = true)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 15/04/2010
        // 15/04/2010: ZaMa - Las mascotas no se apropian de npcs.
        // ***************************************************

        short j;

        // Si no tengo mascotas, para que cheaquear lo demas?
        if (Declaraciones.UserList[UserIndex].NroMascotas == 0)
            return;

        if (!PuedeAtacarNPC(UserIndex, NpcIndex, IsPet: true))
            return;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            for (j = 1; j <= Declaraciones.MAXMASCOTAS; j++)
                if (withBlock.MascotasIndex[j] > 0)
                    if (withBlock.MascotasIndex[j] != NpcIndex)
                        if (CheckElementales |
                            ((Declaraciones.Npclist[withBlock.MascotasIndex[j]].Numero != AI.ELEMENTALFUEGO) &
                             (Declaraciones.Npclist[withBlock.MascotasIndex[j]].Numero != AI.ELEMENTALTIERRA)))
                        {
                            if (Declaraciones.Npclist[withBlock.MascotasIndex[j]].TargetNPC == 0)
                                Declaraciones.Npclist[withBlock.MascotasIndex[j]].TargetNPC = NpcIndex;
                            Declaraciones.Npclist[withBlock.MascotasIndex[j]].Movement = AI.TipoAI.NpcAtacaNpc;
                        }
        }
    }

    public static void AllFollowAmo(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short j;

        for (j = 1; j <= Declaraciones.MAXMASCOTAS; j++)
            if (Declaraciones.UserList[UserIndex].MascotasIndex[j] > 0)
                NPCs.FollowAmo(Declaraciones.UserList[UserIndex].MascotasIndex[j]);
    }

    public static bool NpcAtacaUser(short NpcIndex, short UserIndex)
    {
        bool NpcAtacaUserRet = default;
        // *************************************************
        // Author: Unknown
        // Last modified: -
        // 
        // *************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.flags.AdminInvisible == 1)
                return NpcAtacaUserRet;
            if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0) &
                !withBlock.flags.AdminPerseguible)
                return NpcAtacaUserRet;
        }

        {
            ref var withBlock1 = ref Declaraciones.Npclist[NpcIndex];
            // El npc puede atacar ???
            if (withBlock1.CanAttack == 1)
            {
                NpcAtacaUserRet = true;
                CheckPets(NpcIndex, UserIndex, false);

                if (withBlock1.Target == 0)
                    withBlock1.Target = UserIndex;

                if ((Declaraciones.UserList[UserIndex].flags.AtacadoPorNpc == 0) &
                    (Declaraciones.UserList[UserIndex].flags.AtacadoPorUser == 0))
                    Declaraciones.UserList[UserIndex].flags.AtacadoPorNpc = NpcIndex;
            }
            else
            {
                NpcAtacaUserRet = false;
                return NpcAtacaUserRet;
            }

            withBlock1.CanAttack = 0;

            if (withBlock1.flags.Snd1 > 0)
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, NpcIndex,
                    Protocol.PrepareMessagePlayWave(Convert.ToByte(withBlock1.flags.Snd1),
                        Convert.ToByte(withBlock1.Pos.X), Convert.ToByte(withBlock1.Pos.Y)));
        }

        if (NpcImpacto(NpcIndex, UserIndex))
        {
            {
                ref var withBlock2 = ref Declaraciones.UserList[UserIndex];
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    Protocol.PrepareMessagePlayWave(Declaraciones.SND_IMPACTO, Convert.ToByte(withBlock2.Pos.X),
                        Convert.ToByte(withBlock2.Pos.Y)));

                if (!withBlock2.flags.Meditando)
                    if (withBlock2.flags.Navegando == 0)
                        modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                            Protocol.PrepareMessageCreateFX(withBlock2.character.CharIndex, Declaraciones.FXSANGRE,
                                0));

                NpcDaño(NpcIndex, UserIndex);
                Protocol.WriteUpdateHP(UserIndex);

                // ¿Puede envenenar?
                if (Declaraciones.Npclist[NpcIndex].Veneno == 1)
                    NPCs.NpcEnvenenarUser(UserIndex);
            }

            UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Tacticas, false);
        }
        else
        {
            Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.NPCSwing);
            UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Tacticas, true);
        }

        // Controla el nivel del usuario
        UsUaRiOs.CheckUserLevel(UserIndex);
        return NpcAtacaUserRet;
    }

    private static bool NpcImpactoNpc(short Atacante, short Victima)
    {
        bool NpcImpactoNpcRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int PoderAtt;
        int PoderEva;
        int ProbExito;

        PoderAtt = Declaraciones.Npclist[Atacante].PoderAtaque;
        PoderEva = Declaraciones.Npclist[Victima].PoderEvasion;

        // Chances are rounded
        ProbExito = MaximoInt(10, MinimoInt(90, Convert.ToInt32(50d + (PoderAtt - PoderEva) * 0.4d)));
        NpcImpactoNpcRet = Matematicas.RandomNumber(1, 100) <= ProbExito;
        return NpcImpactoNpcRet;
    }

    public static void NpcDañoNpc(short Atacante, short Victima)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short daño;

        {
            ref var withBlock = ref Declaraciones.Npclist[Atacante];
            daño = Convert.ToInt16(Matematicas.RandomNumber(withBlock.Stats.MinHIT, withBlock.Stats.MaxHIT));
            Declaraciones.Npclist[Victima].Stats.MinHp = Declaraciones.Npclist[Victima].Stats.MinHp - daño;

            if (Declaraciones.Npclist[Victima].Stats.MinHp < 1)
            {
                withBlock.Movement = withBlock.flags.OldMovement;

                if (Migration.migr_LenB(withBlock.flags.AttackedBy) != 0) withBlock.Hostile = withBlock.flags.OldHostil;

                if (withBlock.MaestroUser > 0) NPCs.FollowAmo(Atacante);

                NPCs.MuereNpc(Victima, withBlock.MaestroUser);
            }
        }
    }

    public static void NpcAtacaNpc(short Atacante, short Victima, bool cambiarMOvimiento = true)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 01/03/2009
        // 01/03/2009: ZaMa - Las mascotas no pueden atacar al rey si quedan pretorianos vivos.
        // *************************************************

        {
            ref var withBlock = ref Declaraciones.Npclist[Atacante];

            // Es el Rey Preatoriano?
            if (Declaraciones.Npclist[Victima].Numero == PraetoriansCoopNPC.PRKING_NPC)
                if (PraetoriansCoopNPC.pretorianosVivos > 0)
                {
                    Protocol.WriteConsoleMsg(withBlock.MaestroUser,
                        "Debes matar al resto del ejército antes de atacar al rey!",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    withBlock.TargetNPC = 0;
                    return;
                }

            // El npc puede atacar ???
            if (withBlock.CanAttack == 1)
            {
                withBlock.CanAttack = 0;
                if (cambiarMOvimiento)
                {
                    Declaraciones.Npclist[Victima].TargetNPC = Atacante;
                    Declaraciones.Npclist[Victima].Movement = AI.TipoAI.NpcAtacaNpc;
                }
            }
            else
            {
                return;
            }

            if (withBlock.flags.Snd1 > 0)
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, Atacante,
                    Protocol.PrepareMessagePlayWave(Convert.ToByte(withBlock.flags.Snd1),
                        Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));

            if (NpcImpactoNpc(Atacante, Victima))
            {
                if (Declaraciones.Npclist[Victima].flags.Snd2 > 0)
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, Victima,
                        Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Npclist[Victima].flags.Snd2),
                            Convert.ToByte(Declaraciones.Npclist[Victima].Pos.X),
                            Convert.ToByte(Declaraciones.Npclist[Victima].Pos.Y)));
                else
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, Victima,
                        Protocol.PrepareMessagePlayWave(Declaraciones.SND_IMPACTO2,
                            Convert.ToByte(Declaraciones.Npclist[Victima].Pos.X),
                            Convert.ToByte(Declaraciones.Npclist[Victima].Pos.Y)));

                if (withBlock.MaestroUser > 0)
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, Atacante,
                        Protocol.PrepareMessagePlayWave(Declaraciones.SND_IMPACTO, Convert.ToByte(withBlock.Pos.X),
                            Convert.ToByte(withBlock.Pos.Y)));
                else
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, Victima,
                        Protocol.PrepareMessagePlayWave(Declaraciones.SND_IMPACTO,
                            Convert.ToByte(Declaraciones.Npclist[Victima].Pos.X),
                            Convert.ToByte(Declaraciones.Npclist[Victima].Pos.Y)));

                NpcDañoNpc(Atacante, Victima);
            }
            else if (withBlock.MaestroUser > 0)
            {
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, Atacante,
                    Protocol.PrepareMessagePlayWave(Declaraciones.SND_SWING, Convert.ToByte(withBlock.Pos.X),
                        Convert.ToByte(withBlock.Pos.Y)));
            }
            else
            {
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, Victima,
                    Protocol.PrepareMessagePlayWave(Declaraciones.SND_SWING,
                        Convert.ToByte(Declaraciones.Npclist[Victima].Pos.X),
                        Convert.ToByte(Declaraciones.Npclist[Victima].Pos.Y)));
            }
        }
    }

    public static bool UsuarioAtacaNpc(short UserIndex, short NpcIndex)
    {
        bool UsuarioAtacaNpcRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: 14/01/2010 (ZaMa)
        // 12/01/2010: ZaMa - Los druidas pierden la inmunidad de ser atacados por npcs cuando los atacan.
        // 14/01/2010: ZaMa - Lo transformo en función, para que no se pierdan municiones al atacar targets inválidos.
        // ***************************************************

        try
        {
            if (!PuedeAtacarNPC(UserIndex, NpcIndex))
                return UsuarioAtacaNpcRet;

            UsUaRiOs.NPCAtacado(NpcIndex, UserIndex);

            if (UserImpactoNpc(UserIndex, NpcIndex))
            {
                if (Declaraciones.Npclist[NpcIndex].flags.Snd2 > 0)
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, NpcIndex,
                        Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Npclist[NpcIndex].flags.Snd2),
                            Convert.ToByte(Declaraciones.Npclist[NpcIndex].Pos.X),
                            Convert.ToByte(Declaraciones.Npclist[NpcIndex].Pos.Y)));
                else
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                        Protocol.PrepareMessagePlayWave(Declaraciones.SND_IMPACTO2,
                            Convert.ToByte(Declaraciones.Npclist[NpcIndex].Pos.X),
                            Convert.ToByte(Declaraciones.Npclist[NpcIndex].Pos.Y)));

                UserDañoNpc(UserIndex, NpcIndex);
            }
            else
            {
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    Protocol.PrepareMessagePlayWave(Declaraciones.SND_SWING,
                        Convert.ToByte(Declaraciones.UserList[UserIndex].Pos.X),
                        Convert.ToByte(Declaraciones.UserList[UserIndex].Pos.Y)));
                Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.UserSwing);
            }

            // Reveló su condición de usuario al atacar, los npcs lo van a atacar
            Declaraciones.UserList[UserIndex].flags.Ignorado = false;

            UsuarioAtacaNpcRet = true;
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in MinimoInt: " + ex.Message);
            var argdesc = "Error en UsuarioAtacaNpc. Error " + ex.GetType().Name + " : " + ex.Message;
            General.LogError(ref argdesc);
        }

        return UsuarioAtacaNpcRet;
    }

    public static void UsuarioAtaca(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short Index;
        Declaraciones.WorldPos AttackPos;

        // Check bow's interval
        if (!modNuevoTimer.IntervaloPermiteUsarArcos(UserIndex, false))
            return;

        // Check Spell-Magic interval
        if (!modNuevoTimer.IntervaloPermiteMagiaGolpe(UserIndex))
            // Check Attack interval
            if (!modNuevoTimer.IntervaloPermiteAtacar(UserIndex))
                return;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Quitamos stamina
            if (withBlock.Stats.MinSta >= 10)
            {
                Trabajo.QuitarSta(UserIndex, Convert.ToInt16(Matematicas.RandomNumber(1, 10)));
            }
            else
            {
                if (withBlock.Genero == Declaraciones.eGenero.Hombre)
                    Protocol.WriteConsoleMsg(UserIndex, "Estás muy cansado para luchar.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                else
                    Protocol.WriteConsoleMsg(UserIndex, "Estás muy cansada para luchar.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto AttackPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            AttackPos = withBlock.Pos;
            Extra.HeadtoPos(withBlock.character.heading, ref AttackPos);

            // Exit if not legal
            if ((AttackPos.X < Declaraciones.XMinMapSize) | (AttackPos.X > Declaraciones.XMaxMapSize) |
                (AttackPos.Y <= Declaraciones.YMinMapSize) | (AttackPos.Y > Declaraciones.YMaxMapSize))
            {
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    Protocol.PrepareMessagePlayWave(Declaraciones.SND_SWING, Convert.ToByte(withBlock.Pos.X),
                        Convert.ToByte(withBlock.Pos.Y)));
                return;
            }

            Index = Declaraciones.MapData[AttackPos.Map, AttackPos.X, AttackPos.Y].UserIndex;

            // Look for user
            if (Index > 0)
            {
                UsuarioAtacaUsuario(UserIndex, Index);
                Protocol.WriteUpdateUserStats(UserIndex);
                Protocol.WriteUpdateUserStats(Index);
                return;
            }

            Index = Declaraciones.MapData[AttackPos.Map, AttackPos.X, AttackPos.Y].NpcIndex;

            // Look for NPC
            if (Index > 0)
            {
                if (Declaraciones.Npclist[Index].Attackable != 0)
                {
                    if ((Declaraciones.Npclist[Index].MaestroUser > 0) &
                        !Declaraciones.mapInfo[Declaraciones.Npclist[Index].Pos.Map].Pk)
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "No puedes atacar mascotas en zona segura.",
                            Protocol.FontTypeNames.FONTTYPE_FIGHT);
                        return;
                    }

                    UsuarioAtacaNpc(UserIndex, Index);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No puedes atacar a este NPC.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }

                Protocol.WriteUpdateUserStats(UserIndex);

                return;
            }

            modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                Protocol.PrepareMessagePlayWave(Declaraciones.SND_SWING, Convert.ToByte(withBlock.Pos.X),
                    Convert.ToByte(withBlock.Pos.Y)));
            Protocol.WriteUpdateUserStats(UserIndex);

            if (withBlock.Counters.Trabajando != 0)
                withBlock.Counters.Trabajando = withBlock.Counters.Trabajando - 1;

            if (withBlock.Counters.Ocultando != 0)
                withBlock.Counters.Ocultando = withBlock.Counters.Ocultando - 1;
        }
    }

    public static bool UsuarioImpacto(short AtacanteIndex, short VictimaIndex)
    {
        bool UsuarioImpactoRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            int ProbRechazo;
            bool Rechazo;
            int ProbExito;
            int PoderAtaque;
            int UserPoderEvasion;
            int UserPoderEvasionEscudo;
            short Arma;
            int SkillTacticas;
            int SkillDefensa;
            int ProbEvadir;
            Declaraciones.eSkill Skill;

            SkillTacticas = Declaraciones.UserList[VictimaIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Tacticas];
            SkillDefensa = Declaraciones.UserList[VictimaIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Defensa];

            Arma = Declaraciones.UserList[AtacanteIndex].Invent.WeaponEqpObjIndex;

            // Calculamos el poder de evasion...
            UserPoderEvasion = PoderEvasion(VictimaIndex);

            if (Declaraciones.UserList[VictimaIndex].Invent.EscudoEqpObjIndex > 0)
            {
                UserPoderEvasionEscudo = PoderEvasionEscudo(VictimaIndex);
                UserPoderEvasion = UserPoderEvasion + UserPoderEvasionEscudo;
            }
            else
            {
                UserPoderEvasionEscudo = 0;
            }

            // Esta usando un arma ???
            if (Declaraciones.UserList[AtacanteIndex].Invent.WeaponEqpObjIndex > 0)
            {
                if (Declaraciones.objData[Arma].proyectil == 1)
                {
                    PoderAtaque = PoderAtaqueProyectil(AtacanteIndex);
                    Skill = Declaraciones.eSkill.Proyectiles;
                }
                else
                {
                    PoderAtaque = PoderAtaqueArma(AtacanteIndex);
                    Skill = Declaraciones.eSkill.Armas;
                }
            }
            else
            {
                PoderAtaque = PoderAtaqueWrestling(AtacanteIndex);
                Skill = Declaraciones.eSkill.Wrestling;
            }

            // Chances are rounded
            ProbExito = MaximoInt(10, MinimoInt(90, Convert.ToInt32(50d + (PoderAtaque - UserPoderEvasion) * 0.4d)));

            // Se reduce la evasion un 25%
            if (Declaraciones.UserList[VictimaIndex].flags.Meditando)
            {
                ProbEvadir = Convert.ToInt32((100 - ProbExito) * 0.75d);
                ProbExito = MinimoInt(90, 100 - ProbEvadir);
            }

            UsuarioImpactoRet = Matematicas.RandomNumber(1, 100) <= ProbExito;

            // el usuario esta usando un escudo ???
            if (Declaraciones.UserList[VictimaIndex].Invent.EscudoEqpObjIndex > 0)
                // Fallo ???
                if (!UsuarioImpactoRet)
                {
                    // Chances are rounded
                    ProbRechazo = MaximoInt(10,
                        MinimoInt(90, Convert.ToInt32(100 * SkillDefensa / (double)(SkillDefensa + SkillTacticas))));
                    Rechazo = Matematicas.RandomNumber(1, 100) <= ProbRechazo;
                    if (Rechazo)
                    {
                        // Se rechazo el ataque con el escudo
                        modSendData.SendData(modSendData.SendTarget.ToPCArea, VictimaIndex,
                            Protocol.PrepareMessagePlayWave(Declaraciones.SND_ESCUDO,
                                Convert.ToByte(Declaraciones.UserList[VictimaIndex].Pos.X),
                                Convert.ToByte(Declaraciones.UserList[VictimaIndex].Pos.Y)));

                        Protocol.WriteMultiMessage(AtacanteIndex,
                            (short)Declaraciones.eMessages.BlockedWithShieldother);
                        Protocol.WriteMultiMessage(VictimaIndex, (short)Declaraciones.eMessages.BlockedWithShieldUser);

                        UsUaRiOs.SubirSkill(VictimaIndex, (short)Declaraciones.eSkill.Defensa, true);
                    }
                    else
                    {
                        UsUaRiOs.SubirSkill(VictimaIndex, (short)Declaraciones.eSkill.Defensa, false);
                    }
                }

            if (!UsuarioImpactoRet) UsUaRiOs.SubirSkill(AtacanteIndex, (short)Skill, false);

            Protocol.FlushBuffer(VictimaIndex);
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in UsuarioAtaca: " + ex.Message);
            var AtacanteNick = default(string);
            var VictimaNick = default(string);

            if (AtacanteIndex > 0)
                AtacanteNick = Declaraciones.UserList[AtacanteIndex].name;
            if (VictimaIndex > 0)
                VictimaNick = Declaraciones.UserList[VictimaIndex].name;

            var argdesc = "Error en UsuarioImpacto. Error " + ex.GetType().Name + " : " + ex.Message +
                          " AtacanteIndex: " + AtacanteIndex + " Nick: " + AtacanteNick + " VictimaIndex: " +
                          VictimaIndex + " Nick: " + VictimaNick;
            General.LogError(ref argdesc);
        }

        return UsuarioImpactoRet;
    }

    public static bool UsuarioAtacaUsuario(short AtacanteIndex, short VictimaIndex)
    {
        bool UsuarioAtacaUsuarioRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: 14/01/2010 (ZaMa)
        // 14/01/2010: ZaMa - Lo transformo en función, para que no se pierdan municiones al atacar targets
        // inválidos, y evitar un doble chequeo innecesario
        // ***************************************************

        try
        {
            if (!PuedeAtacar(AtacanteIndex, VictimaIndex))
                return UsuarioAtacaUsuarioRet;

            {
                ref var withBlock = ref Declaraciones.UserList[AtacanteIndex];
                if (Matematicas.Distancia(ref withBlock.Pos, ref Declaraciones.UserList[VictimaIndex].Pos) >
                    MAXDISTANCIAARCO)
                {
                    Protocol.WriteConsoleMsg(AtacanteIndex, "Estás muy lejos para disparar.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    return UsuarioAtacaUsuarioRet;
                }

                UsuarioAtacadoPorUsuario(AtacanteIndex, VictimaIndex);

                if (UsuarioImpacto(AtacanteIndex, VictimaIndex))
                {
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, AtacanteIndex,
                        Protocol.PrepareMessagePlayWave(Declaraciones.SND_IMPACTO, Convert.ToByte(withBlock.Pos.X),
                            Convert.ToByte(withBlock.Pos.Y)));

                    if (Declaraciones.UserList[VictimaIndex].flags.Navegando == 0)
                        modSendData.SendData(modSendData.SendTarget.ToPCArea, VictimaIndex,
                            Protocol.PrepareMessageCreateFX(Declaraciones.UserList[VictimaIndex].character.CharIndex,
                                Declaraciones.FXSANGRE, 0));

                    // Pablo (ToxicWaste): Guantes de Hurto del Bandido en acción
                    if (withBlock.clase == Declaraciones.eClass.Bandit)
                        Trabajo.DoDesequipar(AtacanteIndex, VictimaIndex);

                    // y ahora, el ladrón puede llegar a paralizar con el golpe.
                    else if (withBlock.clase == Declaraciones.eClass.Thief)
                        Trabajo.DoHandInmo(AtacanteIndex, VictimaIndex);

                    UsUaRiOs.SubirSkill(VictimaIndex, (short)Declaraciones.eSkill.Tacticas, false);
                    UserDañoUser(AtacanteIndex, VictimaIndex);
                }
                else
                {
                    // Invisible admins doesn't make sound to other clients except itself
                    if (withBlock.flags.AdminInvisible == 1)
                    {
                        var argdatos = Protocol.PrepareMessagePlayWave(Declaraciones.SND_SWING,
                            Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y));
                        TCP.EnviarDatosASlot(AtacanteIndex, ref argdatos);
                    }
                    else
                    {
                        modSendData.SendData(modSendData.SendTarget.ToPCArea, AtacanteIndex,
                            Protocol.PrepareMessagePlayWave(Declaraciones.SND_SWING, Convert.ToByte(withBlock.Pos.X),
                                Convert.ToByte(withBlock.Pos.Y)));
                    }

                    Protocol.WriteMultiMessage(AtacanteIndex, (short)Declaraciones.eMessages.UserSwing);
                    Protocol.WriteMultiMessage(VictimaIndex, (short)Declaraciones.eMessages.UserAttackedSwing,
                        AtacanteIndex);
                    UsUaRiOs.SubirSkill(VictimaIndex, (short)Declaraciones.eSkill.Tacticas, true);
                }

                if (withBlock.clase == Declaraciones.eClass.Thief)
                    Trabajo.Desarmar(AtacanteIndex, VictimaIndex);
            }

            UsuarioAtacaUsuarioRet = true;
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in UsuarioAtacaUsuario: " + ex.Message);
            var argdesc = "Error en UsuarioAtacaUsuario. Error " + ex.GetType().Name + " : " + ex.Message;
            General.LogError(ref argdesc);
        }

        return UsuarioAtacaUsuarioRet;
    }

    public static void UserDañoUser(short AtacanteIndex, short VictimaIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 12/01/2010 (ZaMa)
        // 12/01/2010: ZaMa - Implemento armas arrojadizas y probabilidad de acuchillar
        // 11/03/2010: ZaMa - Ahora no cuenta la muerte si estaba en estado atacable, y no se vuelve criminal
        // ***************************************************

        try
        {
            int daño;
            byte Lugar;
            int absorbido;
            var defbarco = default(short);
            // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
            Declaraciones.ObjData obj;
            var Resist = default(byte);

            daño = CalcularDaño(AtacanteIndex);

            UserEnvenena(AtacanteIndex, VictimaIndex);

            short j;
            // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj2, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
            Declaraciones.ObjData Obj2;
            {
                ref var withBlock = ref Declaraciones.UserList[AtacanteIndex];
                if ((withBlock.flags.Navegando == 1) & (withBlock.Invent.BarcoObjIndex > 0))
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto obj. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    obj = Declaraciones.objData[withBlock.Invent.BarcoObjIndex];
                    daño = daño + Matematicas.RandomNumber(obj.MinHIT, obj.MaxHIT);
                }

                if ((Declaraciones.UserList[VictimaIndex].flags.Navegando == 1) &
                    (Declaraciones.UserList[VictimaIndex].Invent.BarcoObjIndex > 0))
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto obj. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    obj =
                        Declaraciones.objData[Declaraciones.UserList[VictimaIndex].Invent.BarcoObjIndex];
                    defbarco = Convert.ToInt16(Matematicas.RandomNumber(obj.MinDef, obj.MaxDef));
                }

                if (withBlock.Invent.WeaponEqpObjIndex > 0)
                    Resist = Declaraciones.objData[withBlock.Invent.WeaponEqpObjIndex].Refuerzo;

                Lugar = Convert.ToByte(Matematicas.RandomNumber(
                    Convert.ToInt32((int)Declaraciones.PartesCuerpo.bCabeza),
                    Convert.ToInt32((int)Declaraciones.PartesCuerpo.bTorso)));

                switch (Lugar)
                {
                    case 1:
                    {
                        // Si tiene casco absorbe el golpe
                        if (Declaraciones.UserList[VictimaIndex].Invent.CascoEqpObjIndex > 0)
                        {
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto obj. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            obj =
                                Declaraciones.objData[
                                    Declaraciones.UserList[VictimaIndex].Invent.CascoEqpObjIndex];
                            absorbido = Matematicas.RandomNumber(obj.MinDef, obj.MaxDef);
                            absorbido = absorbido + defbarco - Resist;
                            daño = daño - absorbido;
                            if (daño < 0)
                                daño = 1;
                        }

                        break;
                    }

                    default:
                    {
                        // Si tiene armadura absorbe el golpe
                        if (Declaraciones.UserList[VictimaIndex].Invent.ArmourEqpObjIndex > 0)
                        {
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto obj. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            obj =
                                Declaraciones.objData[
                                    Declaraciones.UserList[VictimaIndex].Invent.ArmourEqpObjIndex];
                            if (Declaraciones.UserList[VictimaIndex].Invent.EscudoEqpObjIndex != 0)
                            {
                                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj2. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                Obj2 = Declaraciones.objData[
                                    Declaraciones.UserList[VictimaIndex].Invent.EscudoEqpObjIndex];
                                absorbido = Matematicas.RandomNumber(obj.MinDef + Obj2.MinDef,
                                    obj.MaxDef + Obj2.MaxDef);
                            }
                            else
                            {
                                absorbido = Matematicas.RandomNumber(obj.MinDef, obj.MaxDef);
                            }

                            absorbido = absorbido + defbarco - Resist;
                            daño = daño - absorbido;
                            if (daño < 0)
                                daño = 1;
                        }

                        break;
                    }
                }

                Protocol.WriteMultiMessage(AtacanteIndex, (short)Declaraciones.eMessages.UserHittedUser,
                    Declaraciones.UserList[VictimaIndex].character.CharIndex, Lugar, daño);
                Protocol.WriteMultiMessage(VictimaIndex, (short)Declaraciones.eMessages.UserHittedByUser,
                    withBlock.character.CharIndex, Lugar, daño);

                Declaraciones.UserList[VictimaIndex].Stats.MinHp =
                    Convert.ToInt16(Declaraciones.UserList[VictimaIndex].Stats.MinHp - daño);

                if ((withBlock.flags.Hambre == 0) & (withBlock.flags.Sed == 0))
                {
                    // Si usa un arma quizas suba "Combate con armas"
                    if (withBlock.Invent.WeaponEqpObjIndex > 0)
                    {
                        if (Declaraciones.objData[withBlock.Invent.WeaponEqpObjIndex].proyectil != 0)
                        {
                            // es un Arco. Sube Armas a Distancia
                            UsUaRiOs.SubirSkill(AtacanteIndex, (short)Declaraciones.eSkill.Proyectiles, true);

                            // Si es arma arrojadiza..
                            if (Declaraciones.objData[withBlock.Invent.WeaponEqpObjIndex].Municion == 0)
                                // Si acuchilla
                                if (Declaraciones.objData[withBlock.Invent.WeaponEqpObjIndex].Acuchilla == 1)
                                    Trabajo.DoAcuchillar(AtacanteIndex, 0, VictimaIndex, Convert.ToInt16(daño));
                        }
                        else
                        {
                            // Sube combate con armas.
                            UsUaRiOs.SubirSkill(AtacanteIndex, (short)Declaraciones.eSkill.Armas, true);
                        }
                    }
                    else
                    {
                        // sino tal vez lucha libre
                        UsUaRiOs.SubirSkill(AtacanteIndex, (short)Declaraciones.eSkill.Wrestling, true);
                    }

                    // Trata de apuñalar por la espalda al enemigo
                    if (UsUaRiOs.PuedeApuñalar(AtacanteIndex))
                        Trabajo.DoApuñalar(AtacanteIndex, 0, VictimaIndex, Convert.ToInt16(daño));
                    // e intenta dar un golpe crítico [Pablo (ToxicWaste)]
                    Trabajo.DoGolpeCritico(AtacanteIndex, 0, VictimaIndex, Convert.ToInt16(daño));
                }

                if (Declaraciones.UserList[VictimaIndex].Stats.MinHp <= 0)
                {
                    // No cuenta la muerte si estaba en estado atacable
                    if (Declaraciones.UserList[VictimaIndex].flags.AtacablePor != AtacanteIndex)
                    {
                        // Store it!
                        Statistics.StoreFrag(AtacanteIndex, VictimaIndex);

                        UsUaRiOs.ContarMuerte(VictimaIndex, AtacanteIndex);
                    }

                    // Para que las mascotas no sigan intentando luchar y
                    // comiencen a seguir al amo
                    for (j = 1; j <= Declaraciones.MAXMASCOTAS; j++)
                        if (withBlock.MascotasIndex[j] > 0)
                            if (Declaraciones.Npclist[withBlock.MascotasIndex[j]].Target == VictimaIndex)
                            {
                                Declaraciones.Npclist[withBlock.MascotasIndex[j]].Target = 0;
                                NPCs.FollowAmo(withBlock.MascotasIndex[j]);
                            }

                    UsUaRiOs.ActStats(VictimaIndex, AtacanteIndex);
                    UsUaRiOs.UserDie(VictimaIndex);
                }
                else
                {
                    // Está vivo - Actualizamos el HP
                    Protocol.WriteUpdateHP(VictimaIndex);
                }
            }

            // Controla el nivel del usuario
            UsUaRiOs.CheckUserLevel(AtacanteIndex);

            Protocol.FlushBuffer(VictimaIndex);
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in UserDañoUser: " + ex.Message);
            var AtacanteNick = default(string);
            var VictimaNick = default(string);

            if (AtacanteIndex > 0)
                AtacanteNick = Declaraciones.UserList[AtacanteIndex].name;
            if (VictimaIndex > 0)
                VictimaNick = Declaraciones.UserList[VictimaIndex].name;

            var argdesc = "Error en UserDañoUser. Error " + ex.GetType().Name + " : " + ex.Message +
                          " AtacanteIndex: " + AtacanteIndex + " Nick: " + AtacanteNick + " VictimaIndex: " +
                          VictimaIndex + " Nick: " + VictimaNick;
            General.LogError(ref argdesc);
        }
    }

    public static void UsuarioAtacadoPorUsuario(short AttackerIndex, short VictimIndex)
    {
        // ***************************************************
        // Autor: Unknown
        // Last Modification: 05/05/2010
        // Last Modified By: Lucas Tavolaro Ortiz (Tavo)
        // 10/01/2008: Tavo - Se cancela la salida del juego si el user esta saliendo
        // 05/05/2010: ZaMa - Ahora no suma puntos de bandido al atacar a alguien en estado atacable.
        // ***************************************************

        if (TriggerZonaPelea(AttackerIndex, VictimIndex) == Declaraciones.eTrigger6.TRIGGER6_PERMITE)
            return;

        bool EraCriminal;
        var VictimaEsAtacable = default(bool);

        if (!ES.criminal(AttackerIndex))
            if (!ES.criminal(VictimIndex))
            {
                // Si la victima no es atacable por el agresor, entonces se hace pk
                VictimaEsAtacable = Declaraciones.UserList[VictimIndex].flags.AtacablePor == AttackerIndex;
                if (!VictimaEsAtacable)
                    UsUaRiOs.VolverCriminal(AttackerIndex);
            }

        {
            ref var withBlock = ref Declaraciones.UserList[VictimIndex];
            if (withBlock.flags.Meditando)
            {
                withBlock.flags.Meditando = false;
                Protocol.WriteMeditateToggle(VictimIndex);
                Protocol.WriteConsoleMsg(VictimIndex, "Dejas de meditar.", Protocol.FontTypeNames.FONTTYPE_INFO);
                withBlock.character.FX = 0;
                withBlock.character.loops = 0;
                modSendData.SendData(modSendData.SendTarget.ToPCArea, VictimIndex,
                    Protocol.PrepareMessageCreateFX(withBlock.character.CharIndex, 0, 0));
            }
        }

        EraCriminal = ES.criminal(AttackerIndex);

        // Si ataco a un atacable, no suma puntos de bandido
        if (!VictimaEsAtacable)
        {
            ref var withBlock1 = ref Declaraciones.UserList[AttackerIndex].Reputacion;
            if (!ES.criminal(VictimIndex))
            {
                withBlock1.BandidoRep = withBlock1.BandidoRep + Declaraciones.vlASALTO;
                if (withBlock1.BandidoRep > Declaraciones.MAXREP)
                    withBlock1.BandidoRep = Declaraciones.MAXREP;

                withBlock1.NobleRep = Convert.ToInt32(withBlock1.NobleRep * 0.5d);
                if (withBlock1.NobleRep < 0)
                    withBlock1.NobleRep = 0;
            }
            else
            {
                withBlock1.NobleRep = withBlock1.NobleRep + Declaraciones.vlNoble;
                if (withBlock1.NobleRep > Declaraciones.MAXREP)
                    withBlock1.NobleRep = Declaraciones.MAXREP;
            }
        }

        if (ES.criminal(AttackerIndex))
        {
            if (Declaraciones.UserList[AttackerIndex].Faccion.ArmadaReal == 1)
            {
                var argExpulsado = true;
                ModFacciones.ExpulsarFaccionReal(AttackerIndex, ref argExpulsado);
            }

            if (!EraCriminal)
                UsUaRiOs.RefreshCharStatus(AttackerIndex);
        }
        else if (EraCriminal)
        {
            UsUaRiOs.RefreshCharStatus(AttackerIndex);
        }

        AllMascotasAtacanUser(AttackerIndex, VictimIndex);
        AllMascotasAtacanUser(VictimIndex, AttackerIndex);

        // Si la victima esta saliendo se cancela la salida
        UsUaRiOs.CancelExit(VictimIndex);
        Protocol.FlushBuffer(VictimIndex);
    }

    public static void AllMascotasAtacanUser(short victim, short Maestro)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************
        // Reaccion de las mascotas
        short iCount;

        for (iCount = 1; iCount <= Declaraciones.MAXMASCOTAS; iCount++)
            if (Declaraciones.UserList[Maestro].MascotasIndex[iCount] > 0)
            {
                Declaraciones.Npclist[Declaraciones.UserList[Maestro].MascotasIndex[iCount]].flags.AttackedBy =
                    Declaraciones.UserList[victim].name;
                Declaraciones.Npclist[Declaraciones.UserList[Maestro].MascotasIndex[iCount]].Movement =
                    AI.TipoAI.NPCDEFENSA;
                Declaraciones.Npclist[Declaraciones.UserList[Maestro].MascotasIndex[iCount]].Hostile = 1;
            }
    }

    public static bool PuedeAtacar(short AttackerIndex, short VictimIndex)
    {
        bool PuedeAtacarRet = default;
        // ***************************************************
        // Autor: Unknown
        // Last Modification: 02/04/2010
        // Returns true if the AttackerIndex is allowed to attack the VictimIndex.
        // 24/01/2007 Pablo (ToxicWaste) - Ordeno todo y agrego situacion de Defensa en ciudad Armada y Caos.
        // 24/02/2009: ZaMa - Los usuarios pueden atacarse entre si.
        // 02/04/2010: ZaMa - Los armadas no pueden atacar nunca a los ciudas, salvo que esten atacables.
        // ***************************************************
        try
        {
            // MUY importante el orden de estos "IF"...

            // Estas muerto no podes atacar
            if (Declaraciones.UserList[AttackerIndex].flags.Muerto == 1)
            {
                Protocol.WriteConsoleMsg(AttackerIndex, "¡¡Estás muerto!!", Protocol.FontTypeNames.FONTTYPE_INFO);
                PuedeAtacarRet = false;
                return PuedeAtacarRet;
            }

            // No podes atacar a alguien muerto
            if (Declaraciones.UserList[VictimIndex].flags.Muerto == 1)
            {
                Protocol.WriteConsoleMsg(AttackerIndex, "No puedes atacar a un espíritu.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                PuedeAtacarRet = false;
                return PuedeAtacarRet;
            }

            // No podes atacar si estas en consulta
            if (Declaraciones.UserList[AttackerIndex].flags.EnConsulta)
            {
                Protocol.WriteConsoleMsg(AttackerIndex, "No puedes atacar usuarios mientras estas en consulta.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                return PuedeAtacarRet;
            }

            // No podes atacar si esta en consulta
            if (Declaraciones.UserList[VictimIndex].flags.EnConsulta)
            {
                Protocol.WriteConsoleMsg(AttackerIndex, "No puedes atacar usuarios mientras estan en consulta.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                return PuedeAtacarRet;
            }

            // Estamos en una Arena? o un trigger zona segura?
            switch (TriggerZonaPelea(AttackerIndex, VictimIndex))
            {
                case Declaraciones.eTrigger6.TRIGGER6_PERMITE:
                {
                    PuedeAtacarRet = Declaraciones.UserList[VictimIndex].flags.AdminInvisible == 0;
                    return PuedeAtacarRet;
                }

                case Declaraciones.eTrigger6.TRIGGER6_PROHIBE:
                {
                    PuedeAtacarRet = false;
                    return PuedeAtacarRet;
                }

                case Declaraciones.eTrigger6.TRIGGER6_AUSENTE:
                {
                    // Si no estamos en el Trigger 6 entonces es imposible atacar un gm
                    if ((Declaraciones.UserList[VictimIndex].flags.Privilegios & Declaraciones.PlayerType.User) == 0)
                    {
                        if (Declaraciones.UserList[VictimIndex].flags.AdminInvisible == 0)
                            Protocol.WriteConsoleMsg(AttackerIndex, "El ser es demasiado poderoso.",
                                Protocol.FontTypeNames.FONTTYPE_WARNING);
                        PuedeAtacarRet = false;
                        return PuedeAtacarRet;
                    }

                    break;
                }
            }

            // Ataca un ciudadano?
            if (!ES.criminal(VictimIndex))
            {
                // El atacante es ciuda?
                if (!ES.criminal(AttackerIndex))
                {
                    // El atacante es armada?
                    if (Extra.esArmada(AttackerIndex))
                        // La victima es armada?
                        if (Extra.esArmada(VictimIndex))
                        {
                            // No puede
                            Protocol.WriteConsoleMsg(AttackerIndex,
                                "Los soldados del ejército real tienen prohibido atacar ciudadanos.",
                                Protocol.FontTypeNames.FONTTYPE_WARNING);
                            return PuedeAtacarRet;
                        }

                    // Ciuda (o army) atacando a otro ciuda (o army)
                    if (Declaraciones.UserList[VictimIndex].flags.AtacablePor == AttackerIndex)
                        // Se vuelve atacable.
                        if (UsUaRiOs.ToogleToAtackable(AttackerIndex, VictimIndex, false))
                        {
                            PuedeAtacarRet = true;
                            return PuedeAtacarRet;
                        }
                }
            }
            // Ataca a un criminal
            // Sos un Caos atacando otro caos?
            else if (Extra.esCaos(VictimIndex))
            {
                if (Extra.esCaos(AttackerIndex))
                {
                    Protocol.WriteConsoleMsg(AttackerIndex,
                        "Los miembros de la legión oscura tienen prohibido atacarse entre sí.",
                        Protocol.FontTypeNames.FONTTYPE_WARNING);
                    return PuedeAtacarRet;
                }
            }

            // Tenes puesto el seguro?
            if (Declaraciones.UserList[AttackerIndex].flags.Seguro)
            {
                if (!ES.criminal(VictimIndex))
                {
                    Protocol.WriteConsoleMsg(AttackerIndex,
                        "No puedes atacar ciudadanos, para hacerlo debes desactivar el seguro.",
                        Protocol.FontTypeNames.FONTTYPE_WARNING);
                    PuedeAtacarRet = false;
                    return PuedeAtacarRet;
                }
            }
            // Un ciuda es atacado
            else if (!ES.criminal(VictimIndex))
            {
                // Por un armada sin seguro
                if (Extra.esArmada(AttackerIndex))
                {
                    // No puede
                    Protocol.WriteConsoleMsg(AttackerIndex,
                        "Los soldados del ejército real tienen prohibido atacar ciudadanos.",
                        Protocol.FontTypeNames.FONTTYPE_WARNING);
                    PuedeAtacarRet = false;
                    return PuedeAtacarRet;
                }
            }

            // Estas en un Mapa Seguro?
            if (!Declaraciones.mapInfo[Declaraciones.UserList[VictimIndex].Pos.Map].Pk)
            {
                if (Extra.esArmada(AttackerIndex))
                    if (Declaraciones.UserList[AttackerIndex].Faccion.RecompensasReal > 11)
                        if ((Declaraciones.UserList[VictimIndex].Pos.Map == 58) |
                            (Declaraciones.UserList[VictimIndex].Pos.Map == 59) |
                            (Declaraciones.UserList[VictimIndex].Pos.Map == 60))
                        {
                            Protocol.WriteConsoleMsg(VictimIndex,
                                "¡Huye de la ciudad! Estás siendo atacado y no podrás defenderte.",
                                Protocol.FontTypeNames.FONTTYPE_WARNING);
                            PuedeAtacarRet = true; // Beneficio de Armadas que atacan en su ciudad.
                            return PuedeAtacarRet;
                        }

                if (Extra.esCaos(AttackerIndex))
                    if (Declaraciones.UserList[AttackerIndex].Faccion.RecompensasCaos > 11)
                        if ((Declaraciones.UserList[VictimIndex].Pos.Map == 151) |
                            (Declaraciones.UserList[VictimIndex].Pos.Map == 156))
                        {
                            Protocol.WriteConsoleMsg(VictimIndex,
                                "¡Huye de la ciudad! Estás siendo atacado y no podrás defenderte.",
                                Protocol.FontTypeNames.FONTTYPE_WARNING);
                            PuedeAtacarRet = true; // Beneficio de Caos que atacan en su ciudad.
                            return PuedeAtacarRet;
                        }

                Protocol.WriteConsoleMsg(AttackerIndex,
                    "Esta es una zona segura, aquí no puedes atacar a otros usuarios.",
                    Protocol.FontTypeNames.FONTTYPE_WARNING);
                PuedeAtacarRet = false;
                return PuedeAtacarRet;
            }

            // Estas atacando desde un trigger seguro? o tu victima esta en uno asi?
            if ((Declaraciones.MapData[Declaraciones.UserList[VictimIndex].Pos.Map,
                     Declaraciones.UserList[VictimIndex].Pos.X, Declaraciones.UserList[VictimIndex].Pos.Y].trigger ==
                 Declaraciones.eTrigger.ZONASEGURA) |
                (Declaraciones.MapData[Declaraciones.UserList[AttackerIndex].Pos.Map,
                     Declaraciones.UserList[AttackerIndex].Pos.X, Declaraciones.UserList[AttackerIndex].Pos.Y]
                 .trigger ==
                 Declaraciones.eTrigger.ZONASEGURA))
            {
                Protocol.WriteConsoleMsg(AttackerIndex, "No puedes pelear aquí.",
                    Protocol.FontTypeNames.FONTTYPE_WARNING);
                PuedeAtacarRet = false;
                return PuedeAtacarRet;
            }

            PuedeAtacarRet = true;
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in UsuarioAtacadoPorUsuario: " + ex.Message);
            var argdesc = "Error en PuedeAtacar. Error " + ex.GetType().Name + " : " + ex.Message;
            General.LogError(ref argdesc);
        }

        return PuedeAtacarRet;
    }

    public static bool PuedeAtacarNPC(short AttackerIndex, short NpcIndex, bool Paraliza = false, bool IsPet = false)
    {
        bool PuedeAtacarNPCRet = default;
        // ***************************************************
        // Autor: Unknown Author (Original version)
        // Returns True if AttackerIndex can attack the NpcIndex
        // Last Modification: 16/11/2009
        // 24/01/2007 Pablo (ToxicWaste) - Orden y corrección de ataque sobre una mascota y guardias
        // 14/08/2007 Pablo (ToxicWaste) - Reescribo y agrego TODOS los casos posibles cosa de usar
        // esta función para todo lo referente a ataque a un NPC. Ya sea Magia, Físico o a Distancia.
        // 16/11/2009: ZaMa - Agrego validacion de pertenencia de npc.
        // 02/04/2010: ZaMa - Los armadas ya no peuden atacar npcs no hotiles.
        // ***************************************************

        short OwnerUserIndex;

        // Estas muerto?
        if (Declaraciones.UserList[AttackerIndex].flags.Muerto == 1)
        {
            Protocol.WriteConsoleMsg(AttackerIndex, "¡¡Estás muerto!!", Protocol.FontTypeNames.FONTTYPE_INFO);
            return PuedeAtacarNPCRet;
        }

        // Sos consejero?
        if ((Declaraciones.UserList[AttackerIndex].flags.Privilegios & Declaraciones.PlayerType.Consejero) != 0)
            // No pueden atacar NPC los Consejeros.
            return PuedeAtacarNPCRet;

        // No podes atacar si estas en consulta
        if (Declaraciones.UserList[AttackerIndex].flags.EnConsulta)
        {
            Protocol.WriteConsoleMsg(AttackerIndex, "No puedes atacar npcs mientras estas en consulta.",
                Protocol.FontTypeNames.FONTTYPE_INFO);
            return PuedeAtacarNPCRet;
        }

        // Es una criatura atacable?
        if (Declaraciones.Npclist[NpcIndex].Attackable == 0)
        {
            Protocol.WriteConsoleMsg(AttackerIndex, "No puedes atacar esta criatura.",
                Protocol.FontTypeNames.FONTTYPE_INFO);
            return PuedeAtacarNPCRet;
        }

        // Es valida la distancia a la cual estamos atacando?
        if (Matematicas.Distancia(ref Declaraciones.UserList[AttackerIndex].Pos,
                ref Declaraciones.Npclist[NpcIndex].Pos) >= MAXDISTANCIAARCO)
        {
            Protocol.WriteConsoleMsg(AttackerIndex, "Estás muy lejos para disparar.",
                Protocol.FontTypeNames.FONTTYPE_FIGHT);
            return PuedeAtacarNPCRet;
        }

        // Es una criatura No-Hostil?
        if (Declaraciones.Npclist[NpcIndex].Hostile == 0)
        {
            // Es Guardia del Caos?
            if (Declaraciones.Npclist[NpcIndex].NPCtype == Declaraciones.eNPCType.Guardiascaos)
            {
                // Lo quiere atacar un caos?
                if (Extra.esCaos(AttackerIndex))
                {
                    Protocol.WriteConsoleMsg(AttackerIndex,
                        "No puedes atacar Guardias del Caos siendo de la legión oscura.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return PuedeAtacarNPCRet;
                }
            }
            // Es guardia Real?
            else if (Declaraciones.Npclist[NpcIndex].NPCtype == Declaraciones.eNPCType.GuardiaReal)
            {
                // Lo quiere atacar un Armada?
                if (Extra.esArmada(AttackerIndex))
                {
                    Protocol.WriteConsoleMsg(AttackerIndex,
                        "No puedes atacar Guardias Reales siendo del ejército real.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return PuedeAtacarNPCRet;
                }

                // Tienes el seguro puesto?
                if (Declaraciones.UserList[AttackerIndex].flags.Seguro)
                {
                    Protocol.WriteConsoleMsg(AttackerIndex,
                        "Para poder atacar Guardias Reales debes quitarte el seguro.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return PuedeAtacarNPCRet;
                }

                Protocol.WriteConsoleMsg(AttackerIndex, "¡Atacaste un Guardia Real! Eres un criminal.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                UsUaRiOs.VolverCriminal(AttackerIndex);
                PuedeAtacarNPCRet = true;
                return PuedeAtacarNPCRet;
            }

            // No era un Guardia, asi que es una criatura No-Hostil común.
            // Para asegurarnos que no sea una Mascota:
            else if (Declaraciones.Npclist[NpcIndex].MaestroUser == 0)
            {
                // Si sos ciudadano tenes que quitar el seguro para atacarla.
                if (!ES.criminal(AttackerIndex))
                {
                    // Si sos armada no podes atacarlo directamente
                    if (Extra.esArmada(AttackerIndex))
                    {
                        Protocol.WriteConsoleMsg(AttackerIndex,
                            "Los miembros del ejército real no pueden atacar npcs no hostiles.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return PuedeAtacarNPCRet;
                    }

                    // Sos ciudadano, tenes el seguro puesto?
                    if (Declaraciones.UserList[AttackerIndex].flags.Seguro)
                    {
                        Protocol.WriteConsoleMsg(AttackerIndex, "Para atacar a este NPC debes quitarte el seguro.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return PuedeAtacarNPCRet;
                    }

                    // No tiene seguro puesto. Puede atacar pero es penalizado.
                    Protocol.WriteConsoleMsg(AttackerIndex,
                        "Atacaste un NPC no-hostil. Continúa haciéndolo y te podrás convertir en criminal.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    // NicoNZ: Cambio para que al atacar npcs no hostiles no bajen puntos de nobleza
                    var argNoblePts = 0;
                    var argBandidoPts = 1000;
                    modHechizos.DisNobAuBan(AttackerIndex, ref argNoblePts, ref argBandidoPts);
                    PuedeAtacarNPCRet = true;
                    return PuedeAtacarNPCRet;
                }
            }
        }

        // Es el NPC mascota de alguien?
        if (Declaraciones.Npclist[NpcIndex].MaestroUser > 0)
        {
            if (!ES.criminal(Declaraciones.Npclist[NpcIndex].MaestroUser))
            {
                // Es mascota de un Ciudadano.
                if (Extra.esArmada(AttackerIndex))
                {
                    // El atacante es Armada y esta intentando atacar mascota de un Ciudadano
                    Protocol.WriteConsoleMsg(AttackerIndex,
                        "Los miembros del ejército real no pueden atacar mascotas de ciudadanos.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return PuedeAtacarNPCRet;
                }

                if (!ES.criminal(AttackerIndex))
                {
                    // El atacante es Ciudadano y esta intentando atacar mascota de un Ciudadano.
                    if (Declaraciones.UserList[AttackerIndex].flags.Seguro)
                    {
                        // El atacante tiene el seguro puesto. No puede atacar.
                        Protocol.WriteConsoleMsg(AttackerIndex,
                            "Para atacar mascotas de ciudadanos debes quitarte el seguro.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return PuedeAtacarNPCRet;
                    }

                    // El atacante no tiene el seguro puesto. Recibe penalización.
                    Protocol.WriteConsoleMsg(AttackerIndex, "Has atacado la Mascota de un ciudadano. Eres un criminal.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    UsUaRiOs.VolverCriminal(AttackerIndex);
                    PuedeAtacarNPCRet = true;
                    return PuedeAtacarNPCRet;
                }
                // El atacante es criminal y quiere atacar un elemental ciuda, pero tiene el seguro puesto (NicoNZ)

                if (Declaraciones.UserList[AttackerIndex].flags.Seguro)
                {
                    Protocol.WriteConsoleMsg(AttackerIndex,
                        "Para atacar mascotas de ciudadanos debes quitarte el seguro.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return PuedeAtacarNPCRet;
                }
            }
            // Es mascota de un Criminal.
            else if (Extra.esCaos(Declaraciones.Npclist[NpcIndex].MaestroUser))
            {
                // Es Caos el Dueño.
                if (Extra.esCaos(AttackerIndex))
                {
                    // Un Caos intenta atacar una criatura de un Caos. No puede atacar.
                    Protocol.WriteConsoleMsg(AttackerIndex,
                        "Los miembros de la legión oscura no pueden atacar mascotas de otros legionarios. ",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return PuedeAtacarNPCRet;
                }
            }
        }

        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
            // El npc le pertenece a alguien?
            OwnerUserIndex = withBlock.Owner;

            if (OwnerUserIndex > 0)
            {
                // Puede atacar a su propia criatura!
                if (OwnerUserIndex == AttackerIndex)
                {
                    PuedeAtacarNPCRet = true;
                    modNuevoTimer.IntervaloPerdioNpc(OwnerUserIndex, true); // Renuevo el timer
                    return PuedeAtacarNPCRet;
                }

                // Esta compartiendo el npc con el atacante? => Puede atacar!
                if (Declaraciones.UserList[OwnerUserIndex].flags.ShareNpcWith == AttackerIndex)
                {
                    PuedeAtacarNPCRet = true;
                    return PuedeAtacarNPCRet;
                }

                // Si son del mismo clan o party, pueden atacar (No renueva el timer)
                if (!SameClan(OwnerUserIndex, AttackerIndex) & !SameParty(OwnerUserIndex, AttackerIndex))
                {
                    // Si se le agoto el tiempo
                    if (modNuevoTimer.IntervaloPerdioNpc(OwnerUserIndex)) // Se lo roba :P
                    {
                        UsUaRiOs.PerdioNpc(OwnerUserIndex);
                        UsUaRiOs.ApropioNpc(AttackerIndex, NpcIndex);
                        PuedeAtacarNPCRet = true;
                        return PuedeAtacarNPCRet;
                    }

                    // Si lanzo un hechizo de para o inmo

                    if (Paraliza)
                    {
                        // Si ya esta paralizado o inmobilizado, no puedo inmobilizarlo de nuevo
                        if ((withBlock.flags.Inmovilizado == 1) | (withBlock.flags.Paralizado == 1))
                        {
                            // TODO_ZAMA: Si dejo esto asi, los pks con seguro peusto van a poder inmobilizar criaturas con dueño
                            // Si es pk neutral, puede hacer lo que quiera :P.
                            if (!ES.criminal(AttackerIndex) & !ES.criminal(OwnerUserIndex))
                            {
                                // El atacante es Armada
                                if (Extra.esArmada(AttackerIndex))
                                {
                                    // Intententa paralizar un npc de un armada?
                                    if (Extra.esArmada(OwnerUserIndex))
                                    {
                                        // El atacante es Armada y esta intentando paralizar un npc de un armada: No puede
                                        Protocol.WriteConsoleMsg(AttackerIndex,
                                            "Los miembros del Ejército Real no pueden paralizar criaturas ya paralizadas pertenecientes a otros miembros del Ejército Real",
                                            Protocol.FontTypeNames.FONTTYPE_INFO);
                                        return PuedeAtacarNPCRet;
                                    }

                                    // El atacante es Armada y esta intentando paralizar un npc de un ciuda
                                    // Si tiene seguro no puede

                                    if (Declaraciones.UserList[AttackerIndex].flags.Seguro)
                                    {
                                        Protocol.WriteConsoleMsg(AttackerIndex,
                                            "Para paralizar criaturas ya paralizadas pertenecientes a ciudadanos debes quitarte el seguro.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO);
                                        return PuedeAtacarNPCRet;
                                    }

                                    // Si ya estaba atacable, no podrá atacar a un npc perteneciente a otro ciuda
                                    if (UsUaRiOs.ToogleToAtackable(AttackerIndex, OwnerUserIndex))
                                    {
                                        Protocol.WriteConsoleMsg(AttackerIndex,
                                            "Has paralizado la criatura de un ciudadano, ahora eres atacable por él.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO);
                                        PuedeAtacarNPCRet = true;
                                    }

                                    return PuedeAtacarNPCRet;
                                }

                                // El atacante es ciuda
                                // El atacante tiene el seguro puesto, no puede paralizar

                                if (Declaraciones.UserList[AttackerIndex].flags.Seguro)
                                {
                                    Protocol.WriteConsoleMsg(AttackerIndex,
                                        "Para paralizar criaturas ya paralizadas pertenecientes a ciudadanos debes quitarte el seguro.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO);
                                    return PuedeAtacarNPCRet;
                                }

                                // El atacante no tiene el seguro puesto, ataca.

                                // Si ya estaba atacable, no podrá atacar a un npc perteneciente a otro ciuda
                                if (UsUaRiOs.ToogleToAtackable(AttackerIndex, OwnerUserIndex))
                                {
                                    Protocol.WriteConsoleMsg(AttackerIndex,
                                        "Has paralizado la criatura de un ciudadano, ahora eres atacable por él.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO);
                                    PuedeAtacarNPCRet = true;
                                }

                                return PuedeAtacarNPCRet;
                            }

                            // Al menos uno de los dos es criminal
                            // Si ambos son caos

                            if (Extra.esCaos(AttackerIndex) & Extra.esCaos(OwnerUserIndex))
                            {
                                // El atacante es Caos y esta intentando paralizar un npc de un Caos
                                Protocol.WriteConsoleMsg(AttackerIndex,
                                    "Los miembros de la legión oscura no pueden paralizar criaturas ya paralizadas por otros legionarios.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                                return PuedeAtacarNPCRet;
                            }
                        }

                        // El npc no esta inmobilizado ni paralizado
                        else
                        {
                            // Si no tiene dueño, puede apropiarselo
                            if (OwnerUserIndex == 0)
                                // Siempre que no posea uno ya (el inmo/para no cambia pertenencia de npcs).
                                if (Declaraciones.UserList[AttackerIndex].flags.OwnedNpc == 0)
                                    UsUaRiOs.ApropioNpc(AttackerIndex, NpcIndex);

                            // Siempre se pueden paralizar/inmobilizar npcs con o sin dueño
                            // que no tengan ese estado
                            PuedeAtacarNPCRet = true;
                            return PuedeAtacarNPCRet;
                        }
                    }

                    // No lanzó hechizos inmobilizantes

                    // El npc le pertenece a un ciudadano
                    else if (!ES.criminal(OwnerUserIndex))
                    {
                        // El atacante es Armada y esta intentando atacar un npc de un Ciudadano
                        if (Extra.esArmada(AttackerIndex))
                        {
                            // Intententa atacar un npc de un armada?
                            if (Extra.esArmada(OwnerUserIndex))
                            {
                                // El atacante es Armada y esta intentando atacar el npc de un armada: No puede
                                Protocol.WriteConsoleMsg(AttackerIndex,
                                    "Los miembros del Ejército Real no pueden atacar criaturas pertenecientes a otros miembros del Ejército Real",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                                return PuedeAtacarNPCRet;
                            }

                            // El atacante es Armada y esta intentando atacar un npc de un ciuda

                            // Si tiene seguro no puede

                            if (Declaraciones.UserList[AttackerIndex].flags.Seguro)
                            {
                                Protocol.WriteConsoleMsg(AttackerIndex,
                                    "Para atacar criaturas ya pertenecientes a ciudadanos debes quitarte el seguro.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                                return PuedeAtacarNPCRet;
                            }

                            // Si ya estaba atacable, no podrá atacar a un npc perteneciente a otro ciuda
                            if (UsUaRiOs.ToogleToAtackable(AttackerIndex, OwnerUserIndex))
                            {
                                Protocol.WriteConsoleMsg(AttackerIndex,
                                    "Has atacado a la criatura de un ciudadano, ahora eres atacable por él.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                                PuedeAtacarNPCRet = true;
                            }

                            return PuedeAtacarNPCRet;
                        }

                        // No es aramda, puede ser criminal o ciuda

                        // El atacante es Ciudadano y esta intentando atacar un npc de un Ciudadano.

                        if (!ES.criminal(AttackerIndex))
                        {
                            if (Declaraciones.UserList[AttackerIndex].flags.Seguro)
                            {
                                // El atacante tiene el seguro puesto. No puede atacar.
                                Protocol.WriteConsoleMsg(AttackerIndex,
                                    "Para atacar criaturas pertenecientes a ciudadanos debes quitarte el seguro.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                                return PuedeAtacarNPCRet;
                            }

                            // El atacante no tiene el seguro puesto, ataca.

                            if (UsUaRiOs.ToogleToAtackable(AttackerIndex, OwnerUserIndex))
                            {
                                Protocol.WriteConsoleMsg(AttackerIndex,
                                    "Has atacado a la criatura de un ciudadano, ahora eres atacable por él.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                                PuedeAtacarNPCRet = true;
                            }

                            return PuedeAtacarNPCRet;
                        }

                        // El atacante es criminal y esta intentando atacar un npc de un Ciudadano.

                        // Es criminal atacando un npc de un ciuda, con seguro puesto.
                        if (Declaraciones.UserList[AttackerIndex].flags.Seguro)
                        {
                            Protocol.WriteConsoleMsg(AttackerIndex,
                                "Para atacar criaturas pertenecientes a ciudadanos debes quitarte el seguro.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            return PuedeAtacarNPCRet;
                        }

                        PuedeAtacarNPCRet = true;
                    }

                    // Es npc de un criminal
                    else if (Extra.esCaos(OwnerUserIndex))
                    {
                        // Es Caos el Dueño.
                        if (Extra.esCaos(AttackerIndex))
                        {
                            // Un Caos intenta atacar una npc de un Caos. No puede atacar.
                            Protocol.WriteConsoleMsg(AttackerIndex,
                                "Los miembros de la Legión Oscura no pueden atacar criaturas de otros legionarios. ",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            return PuedeAtacarNPCRet;
                        }
                    }
                }
            }

            // Si no tiene dueño el npc, se lo apropia
            // Solo pueden apropiarse de npcs los caos, armadas o ciudas.
            else if (!ES.criminal(AttackerIndex) | Extra.esCaos(AttackerIndex))
            {
                // No puede apropiarse de los pretos!
                if (!(PraetoriansCoopNPC.esPretoriano(NpcIndex) != 0))
                    // Si es una mascota atacando, no se apropia del npc
                    if (!IsPet)
                    {
                        // No es dueño de ningun npc => Se lo apropia.
                        if (Declaraciones.UserList[AttackerIndex].flags.OwnedNpc == 0)
                            UsUaRiOs.ApropioNpc(AttackerIndex, NpcIndex);
                        // Es dueño de un npc, pero no puede ser de este porque no tiene propietario.
                        // Se va a adueñar del npc (y perder el otro) solo si no inmobiliza/paraliza
                        else if (!Paraliza)
                            UsUaRiOs.ApropioNpc(AttackerIndex, NpcIndex);
                    }
            }
        }

        // Es el Rey Preatoriano?
        if (PraetoriansCoopNPC.esPretoriano(NpcIndex) == 4)
            if (PraetoriansCoopNPC.pretorianosVivos > 0)
            {
                Protocol.WriteConsoleMsg(AttackerIndex, "Debes matar al resto del ejército antes de atacar al rey.",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
                return PuedeAtacarNPCRet;
            }

        PuedeAtacarNPCRet = true;
        return PuedeAtacarNPCRet;
    }

    private static bool SameClan(short UserIndex, short OtherUserIndex)
    {
        bool SameClanRet = default;
        // ***************************************************
        // Autor: ZaMa
        // Returns True if both players belong to the same clan.
        // Last Modification: 16/11/2009
        // ***************************************************
        SameClanRet =
            (Declaraciones.UserList[UserIndex].GuildIndex == Declaraciones.UserList[OtherUserIndex].GuildIndex) &
            (Declaraciones.UserList[UserIndex].GuildIndex != 0);
        return SameClanRet;
    }

    private static bool SameParty(short UserIndex, short OtherUserIndex)
    {
        bool SamePartyRet = default;
        // ***************************************************
        // Autor: ZaMa
        // Returns True if both players belong to the same party.
        // Last Modification: 16/11/2009
        // ***************************************************
        SamePartyRet =
            (Declaraciones.UserList[UserIndex].PartyIndex == Declaraciones.UserList[OtherUserIndex].PartyIndex) &
            (Declaraciones.UserList[UserIndex].PartyIndex != 0);
        return SamePartyRet;
    }

    public static void CalcularDarExp(short UserIndex, short NpcIndex, int ElDaño)
    {
        // ***************************************************
        // Autor: Nacho (Integer)
        // Last Modification: 03/09/06 Nacho
        // Reescribi gran parte del Sub
        // Ahora, da toda la experiencia del npc mientras este vivo.
        // ***************************************************
        int ExpaDar;

        // [Nacho] Chekeamos que las variables sean validas para las operaciones
        if (ElDaño <= 0)
            ElDaño = 0;
        if (Declaraciones.Npclist[NpcIndex].Stats.MaxHp <= 0)
            return;
        if (ElDaño > Declaraciones.Npclist[NpcIndex].Stats.MinHp)
            ElDaño = Declaraciones.Npclist[NpcIndex].Stats.MinHp;

        // [Nacho] La experiencia a dar es la porcion de vida quitada * toda la experiencia
        ExpaDar = Convert.ToInt32(ElDaño * (Declaraciones.Npclist[NpcIndex].GiveEXP /
                                            (double)Declaraciones.Npclist[NpcIndex].Stats.MaxHp));
        if (ExpaDar <= 0)
            return;

        // [Nacho] Vamos contando cuanta experiencia sacamos, porque se da toda la que no se dio al user que mata al NPC
        // Esto es porque cuando un elemental ataca, no se da exp, y tambien porque la cuenta que hicimos antes
        // Podria dar un numero fraccionario, esas fracciones se acumulan hasta formar enteros ;P
        if (ExpaDar > Declaraciones.Npclist[NpcIndex].flags.ExpCount)
        {
            ExpaDar = Declaraciones.Npclist[NpcIndex].flags.ExpCount;
            Declaraciones.Npclist[NpcIndex].flags.ExpCount = 0;
        }
        else
        {
            Declaraciones.Npclist[NpcIndex].flags.ExpCount = Declaraciones.Npclist[NpcIndex].flags.ExpCount - ExpaDar;
        }

        // [Nacho] Le damos la exp al user
        if (ExpaDar > 0)
        {
            if (Declaraciones.UserList[UserIndex].PartyIndex > 0)
            {
                mdParty.ObtenerExito(UserIndex, ExpaDar, ref Declaraciones.Npclist[NpcIndex].Pos.Map,
                    ref Declaraciones.Npclist[NpcIndex].Pos.X, ref Declaraciones.Npclist[NpcIndex].Pos.Y);
            }
            else
            {
                Declaraciones.UserList[UserIndex].Stats.Exp = Declaraciones.UserList[UserIndex].Stats.Exp + ExpaDar;
                if (Declaraciones.UserList[UserIndex].Stats.Exp > Declaraciones.MAXEXP)
                    Declaraciones.UserList[UserIndex].Stats.Exp = Declaraciones.MAXEXP;
                Protocol.WriteConsoleMsg(UserIndex, "Has ganado " + ExpaDar + " puntos de experiencia.",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
            }

            UsUaRiOs.CheckUserLevel(UserIndex);
        }
    }

    public static Declaraciones.eTrigger6 TriggerZonaPelea(short Origen, short Destino)
    {
        Declaraciones.eTrigger6 TriggerZonaPeleaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // TODO: Pero que rebuscado!!
        // Nigo:  Te lo rediseñe, pero no te borro el TODO para que lo revises.
        try
        {
            Declaraciones.eTrigger tOrg;
            Declaraciones.eTrigger tDst;

            tOrg = Declaraciones.MapData[Declaraciones.UserList[Origen].Pos.Map, Declaraciones.UserList[Origen].Pos.X,
                Declaraciones.UserList[Origen].Pos.Y].trigger;
            tDst = Declaraciones.MapData[Declaraciones.UserList[Destino].Pos.Map, Declaraciones.UserList[Destino].Pos.X,
                Declaraciones.UserList[Destino].Pos.Y].trigger;

            if ((tOrg == Declaraciones.eTrigger.ZONAPELEA) | (tDst == Declaraciones.eTrigger.ZONAPELEA))
            {
                if (tOrg == tDst)
                    TriggerZonaPeleaRet = Declaraciones.eTrigger6.TRIGGER6_PERMITE;
                else
                    TriggerZonaPeleaRet = Declaraciones.eTrigger6.TRIGGER6_PROHIBE;
            }
            else
            {
                TriggerZonaPeleaRet = Declaraciones.eTrigger6.TRIGGER6_AUSENTE;
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in PuedeAtacarNPC: " + ex.Message);
            TriggerZonaPeleaRet = Declaraciones.eTrigger6.TRIGGER6_AUSENTE;
            var argdesc = "Error en TriggerZonaPelea - " + ex.Message;
            General.LogError(ref argdesc);
        }

        return TriggerZonaPeleaRet;
    }

    public static void UserEnvenena(short AtacanteIndex, short VictimaIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short ObjInd;

        ObjInd = Declaraciones.UserList[AtacanteIndex].Invent.WeaponEqpObjIndex;

        if (ObjInd > 0)
        {
            if (Declaraciones.objData[ObjInd].proyectil == 1)
                ObjInd = Declaraciones.UserList[AtacanteIndex].Invent.MunicionEqpObjIndex;

            if (ObjInd > 0)
                if (Declaraciones.objData[ObjInd].Envenena == 1)
                    if (Matematicas.RandomNumber(1, 100) < 60)
                    {
                        Declaraciones.UserList[VictimaIndex].flags.Envenenado = 1;
                        Protocol.WriteConsoleMsg(VictimaIndex,
                            "¡¡" + Declaraciones.UserList[AtacanteIndex].name + " te ha envenenado!!",
                            Protocol.FontTypeNames.FONTTYPE_FIGHT);
                        Protocol.WriteConsoleMsg(AtacanteIndex,
                            "¡¡Has envenenado a " + Declaraciones.UserList[VictimaIndex].name + "!!",
                            Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    }
        }

        Protocol.FlushBuffer(VictimaIndex);
    }
}