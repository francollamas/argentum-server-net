using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
namespace Legacy;

internal static class UsUaRiOs
{
    public static void ActStats(short VictimIndex, short AttackerIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 11/03/2010
        // 11/03/2010: ZaMa - Ahora no te vuelve cirminal por matar un atacable
        // ***************************************************

        short DaExp;
        bool EraCriminal;

        DaExp = Convert.ToInt16(Convert.ToInt16(Declaraciones.UserList[VictimIndex].Stats.ELV) * 2);

        {
            ref var withBlock = ref Declaraciones.UserList[AttackerIndex];
            withBlock.Stats.Exp = withBlock.Stats.Exp + DaExp;
            if (withBlock.Stats.Exp > Declaraciones.MAXEXP)
                withBlock.Stats.Exp = Declaraciones.MAXEXP;

            if (SistemaCombate.TriggerZonaPelea(VictimIndex, AttackerIndex) != Declaraciones.eTrigger6.TRIGGER6_PERMITE)
                // Es legal matarlo si estaba en atacable
                if (Declaraciones.UserList[VictimIndex].flags.AtacablePor != AttackerIndex)
                {
                    EraCriminal = ES.criminal(AttackerIndex);

                    {
                        ref var withBlock1 = ref withBlock.Reputacion;
                        if (!ES.criminal(VictimIndex))
                        {
                            withBlock1.AsesinoRep = withBlock1.AsesinoRep + Declaraciones.vlASESINO * 2;
                            if (withBlock1.AsesinoRep > Declaraciones.MAXREP)
                                withBlock1.AsesinoRep = Declaraciones.MAXREP;
                            withBlock1.BurguesRep = 0;
                            withBlock1.NobleRep = 0;
                            withBlock1.PlebeRep = 0;
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
                        if (!EraCriminal)
                            RefreshCharStatus(AttackerIndex);
                    }
                    else if (EraCriminal)
                    {
                        RefreshCharStatus(AttackerIndex);
                    }
                }

            // Lo mata
            // Call WriteConsoleMsg(attackerIndex, "Has matado a " & UserList(VictimIndex).name & "!", FontTypeNames.FONTTYPE_FIGHT)
            // Call WriteConsoleMsg(attackerIndex, "Has ganado " & DaExp & " puntos de experiencia.", FontTypeNames.FONTTYPE_FIGHT)
            // Call WriteConsoleMsg(VictimIndex, "¡" & .name & " te ha matado!", FontTypeNames.FONTTYPE_FIGHT)
            Protocol.WriteMultiMessage(AttackerIndex, (short)Declaraciones.eMessages.HaveKilledUser, VictimIndex,
                DaExp);
            Protocol.WriteMultiMessage(VictimIndex, (short)Declaraciones.eMessages.UserKill, AttackerIndex);

            // Call UserDie(VictimIndex)
            Protocol.FlushBuffer(VictimIndex);

            // Log
            var argtexto = withBlock.name + " asesino a " + Declaraciones.UserList[VictimIndex].name;
            General.LogAsesinato(ref argtexto);
        }
    }

    public static void RevivirUsuario(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            withBlock.flags.Muerto = 0;
            withBlock.Stats.MinHp = withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Constitucion];

            if (withBlock.Stats.MinHp > withBlock.Stats.MaxHp) withBlock.Stats.MinHp = withBlock.Stats.MaxHp;

            if (withBlock.flags.Navegando == 1)
            {
                ToogleBoatBody(UserIndex);
            }
            else
            {
                General.DarCuerpoDesnudo(UserIndex);

                withBlock.character.Head = withBlock.OrigChar.Head;
            }

            if (withBlock.flags.Traveling != 0)
            {
                withBlock.flags.Traveling = 0;
                withBlock.Counters.goHome = 0;
                Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.CancelHome);
            }

            ChangeUserChar(UserIndex, withBlock.character.body, withBlock.character.Head,
                (byte)withBlock.character.heading, withBlock.character.WeaponAnim,
                withBlock.character.ShieldAnim, withBlock.character.CascoAnim);
            Protocol.WriteUpdateUserStats(UserIndex);
        }
    }

    public static void ToogleBoatBody(short UserIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 13/01/2010
        // Gives boat body depending on user alignment.
        // ***************************************************

        short Ropaje;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];


            withBlock.character.Head = 0;

            // Barco de armada
            if (withBlock.Faccion.ArmadaReal == 1)
            {
                withBlock.character.body = Declaraciones.iFragataReal;
            }

            // Barco de caos
            else if (withBlock.Faccion.FuerzasCaos == 1)
            {
                withBlock.character.body = Declaraciones.iFragataCaos;
            }

            // Barcos neutrales
            else
            {
                Ropaje = Declaraciones.objData[withBlock.Invent.BarcoObjIndex].Ropaje;

                if (ES.criminal(UserIndex))
                    switch (Ropaje)
                    {
                        case Declaraciones.iBarca:
                        {
                            withBlock.character.body = Declaraciones.iBarcaPk;
                            break;
                        }

                        case Declaraciones.iGalera:
                        {
                            withBlock.character.body = Declaraciones.iGaleraPk;
                            break;
                        }

                        case Declaraciones.iGaleon:
                        {
                            withBlock.character.body = Declaraciones.iGaleonPk;
                            break;
                        }
                    }
                else
                    switch (Ropaje)
                    {
                        case Declaraciones.iBarca:
                        {
                            withBlock.character.body = Declaraciones.iBarcaCiuda;
                            break;
                        }

                        case Declaraciones.iGalera:
                        {
                            withBlock.character.body = Declaraciones.iGaleraCiuda;
                            break;
                        }

                        case Declaraciones.iGaleon:
                        {
                            withBlock.character.body = Declaraciones.iGaleonCiuda;
                            break;
                        }
                    }
            }

            withBlock.character.ShieldAnim = Declaraciones.NingunEscudo;
            withBlock.character.WeaponAnim = Declaraciones.NingunArma;
            withBlock.character.CascoAnim = Declaraciones.NingunCasco;
        }
    }

    public static void ChangeUserChar(short UserIndex, short body, short Head, byte heading, short Arma, short Escudo,
        short casco)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].character;
            withBlock.body = body;
            withBlock.Head = Head;
            withBlock.heading = (Declaraciones.eHeading)heading;
            withBlock.WeaponAnim = Arma;
            withBlock.ShieldAnim = Escudo;
            withBlock.CascoAnim = casco;

            modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                Protocol.PrepareMessageCharacterChange(body, Head, (Declaraciones.eHeading)heading, withBlock.CharIndex,
                    Arma, Escudo, withBlock.FX, withBlock.loops, casco));
        }
    }

    public static short GetWeaponAnim(short UserIndex, short ObjIndex)
    {
        short GetWeaponAnimRet = default;
        // ***************************************************
        // Author: Torres Patricio (Pato)
        // Last Modification: 03/29/10
        // 
        // ***************************************************
        short Tmp;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            Tmp = Declaraciones.objData[ObjIndex].WeaponRazaEnanaAnim;

            if (Tmp > 0)
                if ((withBlock.raza == Declaraciones.eRaza.Enano) | (withBlock.raza == Declaraciones.eRaza.Gnomo))
                {
                    GetWeaponAnimRet = Tmp;
                    return GetWeaponAnimRet;
                }

            GetWeaponAnimRet = Declaraciones.objData[ObjIndex].WeaponAnim;
        }

        return GetWeaponAnimRet;
    }

    public static void EnviarFama(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int L;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].Reputacion;
            L = -withBlock.AsesinoRep + -withBlock.BandidoRep + withBlock.BurguesRep + -withBlock.LadronesRep +
                withBlock.NobleRep + withBlock.PlebeRep;
            L = Convert.ToInt32(Math.Round(L / 6d));

            withBlock.Promedio = L;
        }

        Protocol.WriteFame(UserIndex);
    }

    public static void EraseUserChar(short UserIndex, bool IsAdminInvisible)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 08/01/2009
        // 08/01/2009: ZaMa - No se borra el char de un admin invisible en todos los clientes excepto en su mismo cliente.
        // *************************************************

        try
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                Declaraciones.CharList[withBlock.character.CharIndex] = 0;

                if (withBlock.character.CharIndex == Declaraciones.LastChar)
                    while (Declaraciones.CharList[Declaraciones.LastChar] <= 0)
                    {
                        Declaraciones.LastChar = Convert.ToInt16(Declaraciones.LastChar - 1);
                        if (Declaraciones.LastChar <= 1)
                            break;
                    }

                // Si esta invisible, solo el sabe de su propia existencia, es innecesario borrarlo en los demas clientes
                if (IsAdminInvisible)
                {
                    var argdatos = Protocol.PrepareMessageCharacterRemove(withBlock.character.CharIndex);
                    TCP.EnviarDatosASlot(UserIndex, ref argdatos);
                }
                else
                {
                    // Le mandamos el mensaje para que borre el personaje a los clientes que estén cerca
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                        Protocol.PrepareMessageCharacterRemove(withBlock.character.CharIndex));
                }

                ModAreas.QuitarUser(UserIndex, withBlock.Pos.Map);

                Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].UserIndex = 0;
                withBlock.character.CharIndex = 0;
            }

            Declaraciones.NumChars = Convert.ToInt16(Declaraciones.NumChars - 1);
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in ActStats: " + ex.Message);
            var argdesc = "Error en EraseUserchar " + ex.GetType().Name + ": " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    public static void RefreshCharStatus(short UserIndex)
    {
        // *************************************************
        // Author: Tararira
        // Last modified: 04/07/2009
        // Refreshes the status and tag of UserIndex.
        // 04/07/2009: ZaMa - Ahora mantenes la fragata fantasmal si estas muerto.
        // *************************************************
        var ClanTag = default(string);
        byte NickColor;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.GuildIndex > 0)
            {
                ClanTag = modGuilds.GuildName(withBlock.GuildIndex);
                ClanTag = " <" + ClanTag + ">";
            }

            NickColor = GetNickColor(UserIndex);

            if (withBlock.showName)
            {
                var nameValue = withBlock.name;

                string localPrepareMessageUpdateTagAndStatus()
                {
                    var argTag = nameValue + ClanTag;
                    var ret = Protocol.PrepareMessageUpdateTagAndStatus(UserIndex, NickColor, ref argTag);
                    return ret;
                }

                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    localPrepareMessageUpdateTagAndStatus());
            }
            else
            {
                var argTag = string.Empty;
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    Protocol.PrepareMessageUpdateTagAndStatus(UserIndex, NickColor, ref argTag));
            }

            // Si esta navengando, se cambia la barca.
            if (withBlock.flags.Navegando != 0)
            {
                if (withBlock.flags.Muerto == 1)
                    withBlock.character.body = Declaraciones.iFragataFantasmal;
                else
                    ToogleBoatBody(UserIndex);

                ChangeUserChar(UserIndex, withBlock.character.body, withBlock.character.Head,
                    (byte)withBlock.character.heading, withBlock.character.WeaponAnim,
                    withBlock.character.ShieldAnim, withBlock.character.CascoAnim);
            }
        }
    }

    public static byte GetNickColor(short UserIndex)
    {
        byte GetNickColorRet = default;
        // *************************************************
        // Author: ZaMa
        // Last modified: 15/01/2010
        // 
        // *************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];

            if (ES.criminal(UserIndex))
                GetNickColorRet = Convert.ToByte((int)Declaraciones.eNickColor.ieCriminal);
            else
                GetNickColorRet = Convert.ToByte((int)Declaraciones.eNickColor.ieCiudadano);

            if (withBlock.flags.AtacablePor > 0)
                GetNickColorRet = Convert.ToByte(GetNickColorRet | (int)Declaraciones.eNickColor.ieAtacable);
        }

        return GetNickColorRet;
    }

    public static void MakeUserChar(bool toMap, short sndIndex, short UserIndex, short Map, short X, short Y,
        [Optional] [DefaultParameterValue(false)] ref bool ButIndex)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 15/01/2010
        // 23/07/2009: Budi - Ahora se envía el nick
        // 15/01/2010: ZaMa - Ahora se envia el color del nick.
        // *************************************************

        try
        {
            short CharIndex;
            var ClanTag = default(string);
            byte NickColor;
            var UserName = default(string);
            byte Privileges;

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];

                if (Extra.InMapBounds(Map, X, Y))
                {
                    // If needed make a new character in list
                    if (withBlock.character.CharIndex == 0)
                    {
                        CharIndex = NextOpenCharIndex();
                        withBlock.character.CharIndex = CharIndex;
                        Declaraciones.CharList[CharIndex] = UserIndex;
                    }

                    // Place character on map if needed
                    if (toMap)
                        Declaraciones.MapData[Map, X, Y].UserIndex = UserIndex;

                    // Send make character command to clients
                    if (!toMap)
                    {
                        if (withBlock.GuildIndex > 0) ClanTag = modGuilds.GuildName(withBlock.GuildIndex);

                        NickColor = GetNickColor(UserIndex);
                        Privileges = Convert.ToByte((int)withBlock.flags.Privilegios);

                        // Preparo el nick
                        if (withBlock.showName)
                        {
                            UserName = withBlock.name;

                            if (withBlock.flags.EnConsulta)
                            {
                                UserName = UserName + " " + Declaraciones.TAG_CONSULT_MODE;
                            }
                            else if ((Declaraciones.UserList[sndIndex].flags.Privilegios &
                                      (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                       Declaraciones.PlayerType.RoleMaster)) != 0)
                            {
                                if (Migration.migr_LenB(ClanTag) != 0)
                                    UserName = UserName + " <" + ClanTag + ">";
                            }
                            else if ((withBlock.flags.invisible != 0 || withBlock.flags.Oculto != 0) &&
                                     withBlock.flags.AdminInvisible != 1)
                            {
                                UserName = UserName + " " + Declaraciones.TAG_USER_INVISIBLE;
                            }
                            else if (Migration.migr_LenB(ClanTag) != 0)
                            {
                                UserName = UserName + " <" + ClanTag + ">";
                            }
                        }

                        Protocol.WriteCharacterCreate(sndIndex, withBlock.character.body,
                            withBlock.character.Head, withBlock.character.heading,
                            withBlock.character.CharIndex, Convert.ToByte(X), Convert.ToByte(Y),
                            withBlock.character.WeaponAnim, withBlock.character.ShieldAnim,
                            withBlock.character.FX, 999, withBlock.character.CascoAnim, UserName, NickColor,
                            Privileges);
                    }
                    else
                    {
                        // Hide the name and clan - set privs as normal user
                        ModAreas.AgregarUser(UserIndex, withBlock.Pos.Map, ButIndex);
                    }
                }
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in RefreshCharStatus: " + ex.Message);
            var argdesc = "MakeUserChar: num: " + ex.GetType().Name + " desc: " + ex.Message;
            General.LogError(ref argdesc);
            TCP.CloseSocket(UserIndex);
        }
    }

    // '
    // Checks if the user gets the next level.
    // 
    // @param UserIndex Specifies reference to user

    public static void CheckUserLevel(short UserIndex)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 11/19/2009
        // Chequea que el usuario no halla alcanzado el siguiente nivel,
        // de lo contrario le da la vida, mana, etc, correspodiente.
        // 07/08/2006 Integer - Modificacion de los valores
        // 01/10/2007 Tavo - Corregido el BUG de STAT_MAXELV
        // 24/01/2007 Pablo (ToxicWaste) - Agrego modificaciones en ELU al subir de nivel.
        // 24/01/2007 Pablo (ToxicWaste) - Agrego modificaciones de la subida de mana de los magos por lvl.
        // 13/03/2007 Pablo (ToxicWaste) - Agrego diferencias entre el 18 y el 19 en Constitución.
        // 09/01/2008 Pablo (ToxicWaste) - Ahora el incremento de vida por Consitución se controla desde Balance.dat
        // 12/09/2008 Marco Vanotti (Marco) - Ahora si se llega a nivel 25 y está en un clan, se lo expulsa para no sumar antifacción
        // 02/03/2009 ZaMa - Arreglada la validacion de expulsion para miembros de clanes faccionarios que llegan a 25.
        // 11/19/2009 Pato - Modifico la nueva fórmula de maná ganada para el bandido y se la limito a 499
        // 02/04/2010: ZaMa - Modifico la ganancia de hit por nivel del ladron.
        // *************************************************
        var Pts = default(short);
        short AumentoHIT;
        var AumentoMANA = default(short);
        short AumentoSTA;
        short AumentoHP;
        bool WasNewbie;
        double Promedio;
        short aux;
        // UPGRADE_WARNING: El límite inferior de la matriz DistVida ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        var DistVida = new short[6];
        short GI; // Guild Index

        try
        {
            WasNewbie = Extra.EsNewbie(UserIndex);

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                while (withBlock.Stats.Exp >= withBlock.Stats.ELU)
                {
                    // Checkea si alcanzó el máximo nivel
                    if (withBlock.Stats.ELV >= Declaraciones.STAT_MAXELV)
                    {
                        withBlock.Stats.Exp = 0d;
                        withBlock.Stats.ELU = 0;
                        return;
                    }

                    // Store it!
                    Statistics.UserLevelUp(UserIndex);

                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                        Protocol.PrepareMessagePlayWave(Declaraciones.SND_NIVEL, Convert.ToByte(withBlock.Pos.X),
                            Convert.ToByte(withBlock.Pos.Y)));
                    Protocol.WriteConsoleMsg(UserIndex, "¡Has subido de nivel!", Protocol.FontTypeNames.FONTTYPE_INFO);

                    if (withBlock.Stats.ELV == 1)
                        Pts = 10;
                    else
                        // For multiple levels being rised at once
                        Pts = Convert.ToInt16(Pts + 5);

                    withBlock.Stats.ELV = Convert.ToByte(withBlock.Stats.ELV + 1);

                    withBlock.Stats.Exp = withBlock.Stats.Exp - withBlock.Stats.ELU;

                    // Nueva subida de exp x lvl. Pablo (ToxicWaste)
                    if (withBlock.Stats.ELV < 15)
                        withBlock.Stats.ELU = Convert.ToInt32(withBlock.Stats.ELU * 1.4d);
                    else if (withBlock.Stats.ELV < 21)
                        withBlock.Stats.ELU = Convert.ToInt32(withBlock.Stats.ELU * 1.35d);
                    else if (withBlock.Stats.ELV < 33)
                        withBlock.Stats.ELU = Convert.ToInt32(withBlock.Stats.ELU * 1.3d);
                    else if (withBlock.Stats.ELV < 41)
                        withBlock.Stats.ELU = Convert.ToInt32(withBlock.Stats.ELU * 1.225d);
                    else
                        withBlock.Stats.ELU = Convert.ToInt32(withBlock.Stats.ELU * 1.25d);

                    // Calculo subida de vida
                    Promedio = Declaraciones.ModVida[(int)withBlock.clase] -
                               (21 - withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Constitucion]) * 0.5d;
                    aux = Convert.ToInt16(Matematicas.RandomNumber(0, 100));


                    if (Promedio - (int)Math.Floor(Promedio) == 0.5d)
                    {
                        // Es promedio semientero
                        DistVida[1] = Declaraciones.DistribucionSemienteraVida[1];
                        DistVida[2] = (short)(DistVida[1] + Declaraciones.DistribucionSemienteraVida[2]);
                        DistVida[3] = (short)(DistVida[2] + Declaraciones.DistribucionSemienteraVida[3]);
                        DistVida[4] = (short)(DistVida[3] + Declaraciones.DistribucionSemienteraVida[4]);

                        if (aux <= DistVida[1])
                            AumentoHP = Convert.ToInt16(Promedio + 1.5d);
                        else if (aux <= DistVida[2])
                            AumentoHP = Convert.ToInt16(Promedio + 0.5d);
                        else if (aux <= DistVida[3])
                            AumentoHP = Convert.ToInt16(Promedio - 0.5d);
                        else
                            AumentoHP = Convert.ToInt16(Promedio - 1.5d);
                    }
                    else
                    {
                        // Es promedio entero

                        DistVida[1] = Declaraciones.DistribucionSemienteraVida[1];
                        DistVida[2] = (short)(DistVida[1] + Declaraciones.DistribucionEnteraVida[2]);
                        DistVida[3] = (short)(DistVida[2] + Declaraciones.DistribucionEnteraVida[3]);
                        DistVida[4] = (short)(DistVida[3] + Declaraciones.DistribucionEnteraVida[4]);
                        DistVida[5] = (short)(DistVida[4] + Declaraciones.DistribucionEnteraVida[5]);

                        if (aux <= DistVida[1])
                            AumentoHP = Convert.ToInt16(Promedio + 2d);
                        else if (aux <= DistVida[2])
                            AumentoHP = Convert.ToInt16(Promedio + 1d);
                        else if (aux <= DistVida[3])
                            AumentoHP = Convert.ToInt16(Promedio);
                        else if (aux <= DistVida[4])
                            AumentoHP = Convert.ToInt16(Promedio - 1d);
                        else
                            AumentoHP = Convert.ToInt16(Promedio - 2d);
                    }

                    switch (withBlock.clase)
                    {
                        case Declaraciones.eClass.Warrior:
                        {
                            AumentoHIT = Convert.ToInt16(withBlock.Stats.ELV > 35 ? 2 : 3);
                            AumentoSTA = Declaraciones.AumentoSTDef;
                            break;
                        }

                        case Declaraciones.eClass.Hunter:
                        {
                            AumentoHIT = Convert.ToInt16(withBlock.Stats.ELV > 35 ? 2 : 3);
                            AumentoSTA = Declaraciones.AumentoSTDef;
                            break;
                        }

                        case Declaraciones.eClass.Pirat:
                        {
                            AumentoHIT = 3;
                            AumentoSTA = Declaraciones.AumentoSTDef;
                            break;
                        }

                        case Declaraciones.eClass.Paladin:
                        {
                            AumentoHIT = Convert.ToInt16(withBlock.Stats.ELV > 35 ? 1 : 3);
                            AumentoMANA = withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Inteligencia];
                            AumentoSTA = Declaraciones.AumentoSTDef;
                            break;
                        }

                        case Declaraciones.eClass.Thief:
                        {
                            AumentoHIT = 2;
                            AumentoSTA = Declaraciones.AumentoSTLadron;
                            break;
                        }

                        case Declaraciones.eClass.Mage:
                        {
                            AumentoHIT = 1;
                            AumentoMANA = Convert.ToInt16(2.8d *
                                                          withBlock.Stats.UserAtributos[
                                                              (int)Declaraciones.eAtributos.Inteligencia]);
                            AumentoSTA = Declaraciones.AumentoSTMago;
                            break;
                        }

                        case Declaraciones.eClass.Worker:
                        {
                            AumentoHIT = 2;
                            AumentoSTA = Declaraciones.AumentoSTTrabajador;
                            break;
                        }

                        case Declaraciones.eClass.Cleric:
                        {
                            AumentoHIT = 2;
                            AumentoMANA = Convert.ToInt16(2 *
                                                          withBlock.Stats.UserAtributos[
                                                              (int)Declaraciones.eAtributos.Inteligencia]);
                            AumentoSTA = Declaraciones.AumentoSTDef;
                            break;
                        }

                        case Declaraciones.eClass.Druid:
                        {
                            AumentoHIT = 2;
                            AumentoMANA = Convert.ToInt16(2 *
                                                          withBlock.Stats.UserAtributos[
                                                              (int)Declaraciones.eAtributos.Inteligencia]);
                            AumentoSTA = Declaraciones.AumentoSTDef;
                            break;
                        }

                        case Declaraciones.eClass.Assasin:
                        {
                            AumentoHIT = Convert.ToInt16(withBlock.Stats.ELV > 35 ? 1 : 3);
                            AumentoMANA = withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Inteligencia];
                            AumentoSTA = Declaraciones.AumentoSTDef;
                            break;
                        }

                        case Declaraciones.eClass.Bard:
                        {
                            AumentoHIT = 2;
                            AumentoMANA = Convert.ToInt16(2 *
                                                          withBlock.Stats.UserAtributos[
                                                              (int)Declaraciones.eAtributos.Inteligencia]);
                            AumentoSTA = Declaraciones.AumentoSTDef;
                            break;
                        }

                        case Declaraciones.eClass.Bandit:
                        {
                            AumentoHIT = Convert.ToInt16(withBlock.Stats.ELV > 35 ? 1 : 3);
                            AumentoMANA =
                                Convert.ToInt16(
                                    withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Inteligencia] / 3d *
                                    2d);
                            AumentoSTA = Declaraciones.AumentoStBandido;
                            break;
                        }

                        default:
                        {
                            AumentoHIT = 2;
                            AumentoSTA = Declaraciones.AumentoSTDef;
                            break;
                        }
                    }

                    // Actualizamos HitPoints
                    withBlock.Stats.MaxHp = (short)(withBlock.Stats.MaxHp + AumentoHP);
                    if (withBlock.Stats.MaxHp > Declaraciones.STAT_MAXHP)
                        withBlock.Stats.MaxHp = Declaraciones.STAT_MAXHP;

                    // Actualizamos Stamina
                    withBlock.Stats.MaxSta = (short)(withBlock.Stats.MaxSta + AumentoSTA);
                    if (withBlock.Stats.MaxSta > Declaraciones.STAT_MAXSTA)
                        withBlock.Stats.MaxSta = Declaraciones.STAT_MAXSTA;

                    // Actualizamos Mana
                    withBlock.Stats.MaxMAN = (short)(withBlock.Stats.MaxMAN + AumentoMANA);
                    if (withBlock.Stats.MaxMAN > Declaraciones.STAT_MAXMAN)
                        withBlock.Stats.MaxMAN = Declaraciones.STAT_MAXMAN;

                    // Actualizamos Golpe Máximo
                    withBlock.Stats.MaxHIT = (short)(withBlock.Stats.MaxHIT + AumentoHIT);
                    if (withBlock.Stats.ELV < 36)
                    {
                        if (withBlock.Stats.MaxHIT > Declaraciones.STAT_MAXHIT_UNDER36)
                            withBlock.Stats.MaxHIT = Declaraciones.STAT_MAXHIT_UNDER36;
                    }
                    else if (withBlock.Stats.MaxHIT > Declaraciones.STAT_MAXHIT_OVER36)
                    {
                        withBlock.Stats.MaxHIT = Declaraciones.STAT_MAXHIT_OVER36;
                    }

                    // Actualizamos Golpe Mínimo
                    withBlock.Stats.MinHIT = (short)(withBlock.Stats.MinHIT + AumentoHIT);
                    if (withBlock.Stats.ELV < 36)
                    {
                        if (withBlock.Stats.MinHIT > Declaraciones.STAT_MAXHIT_UNDER36)
                            withBlock.Stats.MinHIT = Declaraciones.STAT_MAXHIT_UNDER36;
                    }
                    else if (withBlock.Stats.MinHIT > Declaraciones.STAT_MAXHIT_OVER36)
                    {
                        withBlock.Stats.MinHIT = Declaraciones.STAT_MAXHIT_OVER36;
                    }

                    // Notificamos al user
                    if (AumentoHP > 0)
                        Protocol.WriteConsoleMsg(UserIndex, "Has ganado " + AumentoHP + " puntos de vida.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                    if (AumentoSTA > 0)
                        Protocol.WriteConsoleMsg(UserIndex, "Has ganado " + AumentoSTA + " puntos de energía.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                    if (AumentoMANA > 0)
                        Protocol.WriteConsoleMsg(UserIndex, "Has ganado " + AumentoMANA + " puntos de maná.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                    if (AumentoHIT > 0)
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "Tu golpe máximo aumentó en " + AumentoHIT + " puntos.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        Protocol.WriteConsoleMsg(UserIndex, "Tu golpe mínimo aumentó en " + AumentoHIT + " puntos.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                    }

                    General.LogDesarrollo(withBlock.name + " paso a nivel " + withBlock.Stats.ELV + " gano HP: " +
                                          AumentoHP);

                    withBlock.Stats.MinHp = withBlock.Stats.MaxHp;

                    // If user is in a party, we modify the variable p_sumaniveleselevados
                    mdParty.ActualizarSumaNivelesElevados(UserIndex);
                    // If user reaches lvl 25 and he is in a guild, we check the guild's alignment and expulses the user if guild has factionary alignment

                    if (withBlock.Stats.ELV == 25)
                    {
                        GI = withBlock.GuildIndex;
                        if (GI > 0)
                            if ((modGuilds.GuildAlignment(GI) == "Del Mal") | (modGuilds.GuildAlignment(GI) == "Real"))
                            {
                                // We get here, so guild has factionary alignment, we have to expulse the user
                                modGuilds.m_EcharMiembroDeClan(-1, withBlock.name);
                                modSendData.SendData(modSendData.SendTarget.ToGuildMembers, GI,
                                    Protocol.PrepareMessageConsoleMsg(withBlock.name + " deja el clan.",
                                        Protocol.FontTypeNames.FONTTYPE_GUILD));
                                Protocol.WriteConsoleMsg(UserIndex,
                                    "¡Ya tienes la madurez suficiente como para decidir bajo que estandarte pelearás! Por esta razón, hasta tanto no te enlistes en la facción bajo la cual tu clan está alineado, estarás excluído del mismo.",
                                    Protocol.FontTypeNames.FONTTYPE_GUILD);
                            }
                    }
                }

                // If it ceased to be a newbie, remove newbie items and get char away from newbie dungeon
                if (!Extra.EsNewbie(UserIndex) & WasNewbie)
                {
                    InvUsuario.QuitarNewbieObj(UserIndex);
                    if (Declaraciones.mapInfo[withBlock.Pos.Map].Restringir.ToUpper() == "NEWBIE")
                    {
                        WarpUserChar(UserIndex, 1, 50, 50, true);
                        Protocol.WriteConsoleMsg(UserIndex, "Debes abandonar el Dungeon Newbie.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // Send all gained skill points at once (if any)
                if (Pts > 0)
                {
                    Protocol.WriteLevelUp(UserIndex, Pts);

                    withBlock.Stats.SkillPts = (short)(withBlock.Stats.SkillPts + Pts);

                    Protocol.WriteConsoleMsg(UserIndex, "Has ganado un total de " + Pts + " skillpoints.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                }
            }

            Protocol.WriteUpdateUserStats(UserIndex);
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in CheckUserLevel: " + ex.Message);
            var argdesc = "Error en la subrutina CheckUserLevel - Error : " + ex.GetType().Name + " - Description : " +
                          ex.Message;
            General.LogError(ref argdesc);
        }
    }

    public static bool PuedeAtravesarAgua(short UserIndex)
    {
        bool PuedeAtravesarAguaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        PuedeAtravesarAguaRet = (Declaraciones.UserList[UserIndex].flags.Navegando == 1) |
                                (Declaraciones.UserList[UserIndex].flags.Vuela == 1);
        return PuedeAtravesarAguaRet;
    }

    public static void MoveUserChar(short UserIndex, Declaraciones.eHeading nHeading)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 13/07/2009
        // Moves the char, sending the message to everyone in range.
        // 30/03/2009: ZaMa - Now it's legal to move where a casper is, changing its pos to where the moving char was.
        // 28/05/2009: ZaMa - When you are moved out of an Arena, the resurrection safe is activated.
        // 13/07/2009: ZaMa - Now all the clients don't know when an invisible admin moves, they force the admin to move.
        // 13/07/2009: ZaMa - Invisible admins aren't allowed to force dead characater to move
        // *************************************************
        Declaraciones.WorldPos nPos;
        bool sailing;
        var CasperIndex = default(short);
        Declaraciones.eHeading CasperHeading;
        Declaraciones.WorldPos CasPerPos;

        sailing = PuedeAtravesarAgua(UserIndex);
        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        nPos = Declaraciones.UserList[UserIndex].Pos;
        Extra.HeadtoPos(nHeading, ref nPos);

        short oldUserIndex;
        if (Extra.MoveToLegalPos(Declaraciones.UserList[UserIndex].Pos.Map, nPos.X, nPos.Y, sailing, !sailing))
        {
            // si no estoy solo en el mapa...
            if (Declaraciones.mapInfo[Declaraciones.UserList[UserIndex].Pos.Map].NumUsers > 1)
            {
                CasperIndex = Declaraciones.MapData[Declaraciones.UserList[UserIndex].Pos.Map, nPos.X, nPos.Y]
                    .UserIndex;
                // Si hay un usuario, y paso la validacion, entonces es un casper
                if (CasperIndex > 0)
                    // Los admins invisibles no pueden patear caspers
                    if (!(Declaraciones.UserList[UserIndex].flags.AdminInvisible == 1))
                    {
                        if (SistemaCombate.TriggerZonaPelea(UserIndex, CasperIndex) ==
                            Declaraciones.eTrigger6.TRIGGER6_PROHIBE)
                            if (!Declaraciones.UserList[CasperIndex].flags.SeguroResu)
                            {
                                Declaraciones.UserList[CasperIndex].flags.SeguroResu = true;
                                Protocol.WriteMultiMessage(CasperIndex,
                                    (short)Declaraciones.eMessages.ResuscitationSafeOn);
                            }

                        CasperHeading = InvertHeading(nHeading);
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto CasPerPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        CasPerPos = Declaraciones.UserList[CasperIndex].Pos;
                        Extra.HeadtoPos(CasperHeading, ref CasPerPos);

                        {
                            ref var withBlock = ref Declaraciones.UserList[CasperIndex];

                            // Si es un admin invisible, no se avisa a los demas clientes
                            if (!(withBlock.flags.AdminInvisible == 1))
                                modSendData.SendData(modSendData.SendTarget.ToPCAreaButIndex, CasperIndex,
                                    Protocol.PrepareMessageCharacterMove(withBlock.character.CharIndex,
                                        Convert.ToByte(CasPerPos.X), Convert.ToByte(CasPerPos.Y)));

                            Protocol.WriteForceCharMove(CasperIndex, CasperHeading);

                            // Update map and user pos
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            withBlock.Pos = CasPerPos;
                            withBlock.character.heading = CasperHeading;
                            Declaraciones.MapData[withBlock.Pos.Map, CasPerPos.X, CasPerPos.Y].UserIndex = CasperIndex;
                        }

                        // Actualizamos las áreas de ser necesario
                        ModAreas.CheckUpdateNeededUser(CasperIndex, (byte)CasperHeading);
                    }


                // Si es un admin invisible, no se avisa a los demas clientes
                if (!(Declaraciones.UserList[UserIndex].flags.AdminInvisible == 1))
                    modSendData.SendData(modSendData.SendTarget.ToPCAreaButIndex, UserIndex,
                        Protocol.PrepareMessageCharacterMove(Declaraciones.UserList[UserIndex].character.CharIndex,
                            Convert.ToByte(nPos.X), Convert.ToByte(nPos.Y)));
            }

            // Los admins invisibles no pueden patear caspers
            if (!((Declaraciones.UserList[UserIndex].flags.AdminInvisible == 1) & (CasperIndex != 0)))
            {
                oldUserIndex = Declaraciones.MapData[Declaraciones.UserList[UserIndex].Pos.Map,
                    Declaraciones.UserList[UserIndex].Pos.X, Declaraciones.UserList[UserIndex].Pos.Y].UserIndex;

                // Si no hay intercambio de pos con nadie
                if (oldUserIndex == UserIndex)
                    Declaraciones.MapData[Declaraciones.UserList[UserIndex].Pos.Map,
                        Declaraciones.UserList[UserIndex].Pos.X, Declaraciones.UserList[UserIndex].Pos.Y].UserIndex = 0;

                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                Declaraciones.UserList[UserIndex].Pos = nPos;
                Declaraciones.UserList[UserIndex].character.heading = nHeading;
                Declaraciones.MapData[Declaraciones.UserList[UserIndex].Pos.Map,
                        Declaraciones.UserList[UserIndex].Pos.X, Declaraciones.UserList[UserIndex].Pos.Y].UserIndex =
                    UserIndex;

                // Actualizamos las áreas de ser necesario
                ModAreas.CheckUpdateNeededUser(UserIndex, (byte)nHeading);
            }
            else
            {
                Protocol.WritePosUpdate(UserIndex);
            }
        }

        else
        {
            Protocol.WritePosUpdate(UserIndex);
        }

        if (Declaraciones.UserList[UserIndex].Counters.Trabajando != 0)
            Declaraciones.UserList[UserIndex].Counters.Trabajando =
                Declaraciones.UserList[UserIndex].Counters.Trabajando - 1;

        if (Declaraciones.UserList[UserIndex].Counters.Ocultando != 0)
            Declaraciones.UserList[UserIndex].Counters.Ocultando =
                Declaraciones.UserList[UserIndex].Counters.Ocultando - 1;
    }

    public static Declaraciones.eHeading InvertHeading(Declaraciones.eHeading nHeading)
    {
        Declaraciones.eHeading InvertHeadingRet = default;
        // *************************************************
        // Author: ZaMa
        // Last modified: 30/03/2009
        // Returns the heading opposite to the one passed by val.
        // *************************************************
        switch (nHeading)
        {
            case Declaraciones.eHeading.EAST:
            {
                InvertHeadingRet = Declaraciones.eHeading.WEST;
                break;
            }
            case Declaraciones.eHeading.WEST:
            {
                InvertHeadingRet = Declaraciones.eHeading.EAST;
                break;
            }
            case Declaraciones.eHeading.SOUTH:
            {
                InvertHeadingRet = Declaraciones.eHeading.NORTH;
                break;
            }
            case Declaraciones.eHeading.NORTH:
            {
                InvertHeadingRet = Declaraciones.eHeading.SOUTH;
                break;
            }
        }

        return InvertHeadingRet;
    }

    public static void ChangeUserInv(short UserIndex, byte Slot, ref Declaraciones.UserOBJ obj)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Invent.Object(Slot). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        Declaraciones.UserList[UserIndex].Invent.userObj[Slot] = obj;
        Protocol.WriteChangeInventorySlot(UserIndex, Slot);
    }

    public static short NextOpenCharIndex()
    {
        short NextOpenCharIndexRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int LoopC;

        for (LoopC = 1; LoopC <= Declaraciones.MAXCHARS; LoopC++)
            if (Declaraciones.CharList[LoopC] == 0)
            {
                NextOpenCharIndexRet = Convert.ToInt16(LoopC);
                Declaraciones.NumChars = Convert.ToInt16(Declaraciones.NumChars + 1);

                if (LoopC > Declaraciones.LastChar)
                    Declaraciones.LastChar = Convert.ToInt16(LoopC);

                return NextOpenCharIndexRet;
            }

        return NextOpenCharIndexRet;
    }

    public static short NextOpenUser()
    {
        short NextOpenUserRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int LoopC;

        var loopTo = Declaraciones.MaxUsers + 1;
        for (LoopC = 1; LoopC <= loopTo; LoopC++)
        {
            if (LoopC > Declaraciones.MaxUsers)
                break;
            if ((Declaraciones.UserList[LoopC].ConnID == -1) & !Declaraciones.UserList[LoopC].flags.UserLogged)
                break;
        }

        NextOpenUserRet = Convert.ToInt16(LoopC);
        return NextOpenUserRet;
    }

    public static void SendUserStatsTxt(short sendIndex, short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short GuildI;

        DateTime TempDate;
        int TempSecs;
        string tempStr;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            Protocol.WriteConsoleMsg(sendIndex, "Estadísticas de: " + withBlock.name,
                Protocol.FontTypeNames.FONTTYPE_INFO);
            Protocol.WriteConsoleMsg(sendIndex,
                "Nivel: " + withBlock.Stats.ELV + "  EXP: " + withBlock.Stats.Exp + "/" + withBlock.Stats.ELU,
                Protocol.FontTypeNames.FONTTYPE_INFO);
            Protocol.WriteConsoleMsg(sendIndex,
                "Salud: " + withBlock.Stats.MinHp + "/" + withBlock.Stats.MaxHp + "  Maná: " + withBlock.Stats.MinMAN +
                "/" + withBlock.Stats.MaxMAN + "  Energía: " + withBlock.Stats.MinSta + "/" + withBlock.Stats.MaxSta,
                Protocol.FontTypeNames.FONTTYPE_INFO);

            if (withBlock.Invent.WeaponEqpObjIndex > 0)
                Protocol.WriteConsoleMsg(sendIndex,
                    "Menor Golpe/Mayor Golpe: " + withBlock.Stats.MinHIT + "/" + withBlock.Stats.MaxHIT + " (" +
                    Declaraciones.objData[withBlock.Invent.WeaponEqpObjIndex].MinHIT + "/" +
                    Declaraciones.objData[withBlock.Invent.WeaponEqpObjIndex].MaxHIT + ")",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            else
                Protocol.WriteConsoleMsg(sendIndex,
                    "Menor Golpe/Mayor Golpe: " + withBlock.Stats.MinHIT + "/" + withBlock.Stats.MaxHIT,
                    Protocol.FontTypeNames.FONTTYPE_INFO);

            if (withBlock.Invent.ArmourEqpObjIndex > 0)
            {
                if (withBlock.Invent.EscudoEqpObjIndex > 0)
                    Protocol.WriteConsoleMsg(sendIndex,
                        "(CUERPO) Mín Def/Máx Def: " +
                        (Declaraciones.objData[withBlock.Invent.ArmourEqpObjIndex].MinDef +
                         Declaraciones.objData[withBlock.Invent.EscudoEqpObjIndex].MinDef) + "/" +
                        (Declaraciones.objData[withBlock.Invent.ArmourEqpObjIndex].MaxDef +
                         Declaraciones.objData[withBlock.Invent.EscudoEqpObjIndex].MaxDef),
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                else
                    Protocol.WriteConsoleMsg(sendIndex,
                        "(CUERPO) Mín Def/Máx Def: " +
                        Declaraciones.objData[withBlock.Invent.ArmourEqpObjIndex].MinDef + "/" +
                        Declaraciones.objData[withBlock.Invent.ArmourEqpObjIndex].MaxDef,
                        Protocol.FontTypeNames.FONTTYPE_INFO);
            }
            else
            {
                Protocol.WriteConsoleMsg(sendIndex, "(CUERPO) Mín Def/Máx Def: 0",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }

            if (withBlock.Invent.CascoEqpObjIndex > 0)
                Protocol.WriteConsoleMsg(sendIndex,
                    "(CABEZA) Mín Def/Máx Def: " +
                    Declaraciones.objData[withBlock.Invent.CascoEqpObjIndex].MinDef + "/" +
                    Declaraciones.objData[withBlock.Invent.CascoEqpObjIndex].MaxDef,
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            else
                Protocol.WriteConsoleMsg(sendIndex, "(CABEZA) Mín Def/Máx Def: 0",
                    Protocol.FontTypeNames.FONTTYPE_INFO);

            GuildI = withBlock.GuildIndex;
            if (GuildI > 0)
            {
                Protocol.WriteConsoleMsg(sendIndex, "Clan: " + modGuilds.GuildName(GuildI),
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                if ((modGuilds.GuildLeader(GuildI).ToUpper() ?? "") == (withBlock.name.ToUpper() ?? ""))
                    Protocol.WriteConsoleMsg(sendIndex, "Status: Líder", Protocol.FontTypeNames.FONTTYPE_INFO);
                // guildpts no tienen objeto
            }

            TempDate = DateTime.FromOADate(DateTime.Now.ToOADate() - withBlock.LogOnTime.ToOADate());
            TempSecs = withBlock.UpTime + Math.Abs(TempDate.Day - 30) * 24 * 3600 +
                       Thread.CurrentThread.CurrentCulture.Calendar.GetHour(TempDate) * 3600 +
                       Thread.CurrentThread.CurrentCulture.Calendar.GetMinute(TempDate) * 60 +
                       Thread.CurrentThread.CurrentCulture.Calendar.GetSecond(TempDate);
            tempStr = TempSecs / 86400 + " Dias, " + TempSecs % 86400 / 3600 + " Horas, " +
                      TempSecs % 86400 % 3600 / 60 + " Minutos, " + TempSecs % 86400 % 3600 % 60 + " Segundos.";
            Protocol.WriteConsoleMsg(sendIndex,
                "Logeado hace: " + Thread.CurrentThread.CurrentCulture.Calendar.GetHour(TempDate) + ":" +
                Thread.CurrentThread.CurrentCulture.Calendar.GetMinute(TempDate) + ":" +
                Thread.CurrentThread.CurrentCulture.Calendar.GetSecond(TempDate), Protocol.FontTypeNames.FONTTYPE_INFO);
            Protocol.WriteConsoleMsg(sendIndex, "Total: " + tempStr, Protocol.FontTypeNames.FONTTYPE_INFO);

            Protocol.WriteConsoleMsg(sendIndex,
                "Oro: " + withBlock.Stats.GLD + "  Posición: " + withBlock.Pos.X + "," + withBlock.Pos.Y + " en mapa " +
                withBlock.Pos.Map, Protocol.FontTypeNames.FONTTYPE_INFO);
            Protocol.WriteConsoleMsg(sendIndex,
                "Dados: " + withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] + ", " +
                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] + ", " +
                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Inteligencia] + ", " +
                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Carisma] + ", " +
                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Constitucion],
                Protocol.FontTypeNames.FONTTYPE_INFO);
        }
    }

    public static void SendUserMiniStatsTxt(short sendIndex, short UserIndex)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 23/01/2007
        // Shows the users Stats when the user is online.
        // 23/01/2007 Pablo (ToxicWaste) - Agrego de funciones y mejora de distribución de parámetros.
        // *************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            Protocol.WriteConsoleMsg(sendIndex, "Pj: " + withBlock.name, Protocol.FontTypeNames.FONTTYPE_INFO);
            Protocol.WriteConsoleMsg(sendIndex,
                "Ciudadanos matados: " + withBlock.Faccion.CiudadanosMatados + " Criminales matados: " +
                withBlock.Faccion.CriminalesMatados + " usuarios matados: " + withBlock.Stats.UsuariosMatados,
                Protocol.FontTypeNames.FONTTYPE_INFO);
            Protocol.WriteConsoleMsg(sendIndex, "NPCs muertos: " + withBlock.Stats.NPCsMuertos,
                Protocol.FontTypeNames.FONTTYPE_INFO);
            Protocol.WriteConsoleMsg(sendIndex, "Clase: " + Declaraciones.ListaClases[(int)withBlock.clase],
                Protocol.FontTypeNames.FONTTYPE_INFO);
            Protocol.WriteConsoleMsg(sendIndex, "Pena: " + withBlock.Counters.Pena,
                Protocol.FontTypeNames.FONTTYPE_INFO);

            if (withBlock.Faccion.ArmadaReal == 1)
            {
                Protocol.WriteConsoleMsg(sendIndex, "Ejército real desde: " + withBlock.Faccion.FechaIngreso,
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                Protocol.WriteConsoleMsg(sendIndex,
                    "Ingresó en nivel: " + withBlock.Faccion.NivelIngreso + " con " + withBlock.Faccion.MatadosIngreso +
                    " ciudadanos matados.", Protocol.FontTypeNames.FONTTYPE_INFO);
                Protocol.WriteConsoleMsg(sendIndex, "Veces que ingresó: " + withBlock.Faccion.Reenlistadas,
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }

            else if (withBlock.Faccion.FuerzasCaos == 1)
            {
                Protocol.WriteConsoleMsg(sendIndex, "Legión oscura desde: " + withBlock.Faccion.FechaIngreso,
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                Protocol.WriteConsoleMsg(sendIndex, "Ingresó en nivel: " + withBlock.Faccion.NivelIngreso,
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                Protocol.WriteConsoleMsg(sendIndex, "Veces que ingresó: " + withBlock.Faccion.Reenlistadas,
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }

            else if (withBlock.Faccion.RecibioExpInicialReal == 1)
            {
                Protocol.WriteConsoleMsg(sendIndex, "Fue ejército real", Protocol.FontTypeNames.FONTTYPE_INFO);
                Protocol.WriteConsoleMsg(sendIndex, "Veces que ingresó: " + withBlock.Faccion.Reenlistadas,
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }

            else if (withBlock.Faccion.RecibioExpInicialCaos == 1)
            {
                Protocol.WriteConsoleMsg(sendIndex, "Fue legión oscura", Protocol.FontTypeNames.FONTTYPE_INFO);
                Protocol.WriteConsoleMsg(sendIndex, "Veces que ingresó: " + withBlock.Faccion.Reenlistadas,
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }

            Protocol.WriteConsoleMsg(sendIndex, "Asesino: " + withBlock.Reputacion.AsesinoRep,
                Protocol.FontTypeNames.FONTTYPE_INFO);
            Protocol.WriteConsoleMsg(sendIndex, "Noble: " + withBlock.Reputacion.NobleRep,
                Protocol.FontTypeNames.FONTTYPE_INFO);

            if (withBlock.GuildIndex > 0)
                Protocol.WriteConsoleMsg(sendIndex, "Clan: " + modGuilds.GuildName(withBlock.GuildIndex),
                    Protocol.FontTypeNames.FONTTYPE_INFO);
        }
    }

    public static void SendUserMiniStatsTxtFromChar(short sendIndex, string charName)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 23/01/2007
        // Shows the users Stats when the user is offline.
        // 23/01/2007 Pablo (ToxicWaste) - Agrego de funciones y mejora de distribución de parámetros.
        // *************************************************
        string CharFile;
        string Ban;
        string BanDetailPath;

        BanDetailPath = AppDomain.CurrentDomain.BaseDirectory + "logs/" + "BanDetail.dat";
        CharFile = Declaraciones.CharPath + charName + ".chr";

        if (File.Exists(CharFile))
        {
            Protocol.WriteConsoleMsg(sendIndex, "Pj: " + charName, Protocol.FontTypeNames.FONTTYPE_INFO);
            var argEmptySpaces = 1024;
            var argEmptySpaces1 = 1024;
            var argEmptySpaces2 = 1024;
            Protocol.WriteConsoleMsg(sendIndex,
                "Ciudadanos matados: " + ES.GetVar(CharFile, "FACCIONES", "CiudMatados", ref argEmptySpaces) +
                " CriminalesMatados: " + ES.GetVar(CharFile, "FACCIONES", "CrimMatados", ref argEmptySpaces1) +
                " usuarios matados: " + ES.GetVar(CharFile, "MUERTES", "UserMuertes", ref argEmptySpaces2),
                Protocol.FontTypeNames.FONTTYPE_INFO);
            var argEmptySpaces3 = 1024;
            Protocol.WriteConsoleMsg(sendIndex,
                "NPCs muertos: " + ES.GetVar(CharFile, "MUERTES", "NpcsMuertes", ref argEmptySpaces3),
                Protocol.FontTypeNames.FONTTYPE_INFO);
            var argEmptySpaces4 = 1024;
            Protocol.WriteConsoleMsg(sendIndex,
                "Clase: " + Declaraciones.ListaClases[
                    Convert.ToInt32(ES.GetVar(CharFile, "INIT", "Clase", ref argEmptySpaces4))],
                Protocol.FontTypeNames.FONTTYPE_INFO);
            var argEmptySpaces5 = 1024;
            Protocol.WriteConsoleMsg(sendIndex, "Pena: " + ES.GetVar(CharFile, "COUNTERS", "PENA", ref argEmptySpaces5),
                Protocol.FontTypeNames.FONTTYPE_INFO);

            var argEmptySpaces15 = 1024;
            var argEmptySpaces16 = 1024;
            var argEmptySpaces17 = 1024;
            var argEmptySpaces18 = 1024;
            if (Convert.ToByte(ES.GetVar(CharFile, "FACCIONES", "EjercitoReal", ref argEmptySpaces15)) == 1)
            {
                var argEmptySpaces6 = 1024;
                Protocol.WriteConsoleMsg(sendIndex,
                    "Ejército real desde: " + ES.GetVar(CharFile, "FACCIONES", "FechaIngreso", ref argEmptySpaces6),
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                var argEmptySpaces7 = 1024;
                var argEmptySpaces8 = 1024;
                Protocol.WriteConsoleMsg(sendIndex,
                    "Ingresó en nivel: " +
                    Convert.ToInt16(ES.GetVar(CharFile, "FACCIONES", "NivelIngreso", ref argEmptySpaces7)) + " con " +
                    Convert.ToInt16(ES.GetVar(CharFile, "FACCIONES", "MatadosIngreso", ref argEmptySpaces8)) +
                    " ciudadanos matados.", Protocol.FontTypeNames.FONTTYPE_INFO);
                var argEmptySpaces9 = 1024;
                Protocol.WriteConsoleMsg(sendIndex,
                    "Veces que ingresó: " +
                    Convert.ToByte(ES.GetVar(CharFile, "FACCIONES", "Reenlistadas", ref argEmptySpaces9)),
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }

            else if (Convert.ToByte(ES.GetVar(CharFile, "FACCIONES", "EjercitoCaos", ref argEmptySpaces16)) == 1)
            {
                var argEmptySpaces10 = 1024;
                Protocol.WriteConsoleMsg(sendIndex,
                    "Legión oscura desde: " + ES.GetVar(CharFile, "FACCIONES", "FechaIngreso", ref argEmptySpaces10),
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                var argEmptySpaces11 = 1024;
                Protocol.WriteConsoleMsg(sendIndex,
                    "Ingresó en nivel: " +
                    Convert.ToInt16(ES.GetVar(CharFile, "FACCIONES", "NivelIngreso", ref argEmptySpaces11)),
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                var argEmptySpaces12 = 1024;
                Protocol.WriteConsoleMsg(sendIndex,
                    "Veces que ingresó: " +
                    Convert.ToByte(ES.GetVar(CharFile, "FACCIONES", "Reenlistadas", ref argEmptySpaces12)),
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }

            else if (Convert.ToByte(ES.GetVar(CharFile, "FACCIONES", "rExReal", ref argEmptySpaces17)) == 1)
            {
                Protocol.WriteConsoleMsg(sendIndex, "Fue ejército real", Protocol.FontTypeNames.FONTTYPE_INFO);
                var argEmptySpaces13 = 1024;
                Protocol.WriteConsoleMsg(sendIndex,
                    "Veces que ingresó: " +
                    Convert.ToByte(ES.GetVar(CharFile, "FACCIONES", "Reenlistadas", ref argEmptySpaces13)),
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }

            else if (Convert.ToByte(ES.GetVar(CharFile, "FACCIONES", "rExCaos", ref argEmptySpaces18)) == 1)
            {
                Protocol.WriteConsoleMsg(sendIndex, "Fue legión oscura", Protocol.FontTypeNames.FONTTYPE_INFO);
                var argEmptySpaces14 = 1024;
                Protocol.WriteConsoleMsg(sendIndex,
                    "Veces que ingresó: " +
                    Convert.ToByte(ES.GetVar(CharFile, "FACCIONES", "Reenlistadas", ref argEmptySpaces14)),
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }


            var argEmptySpaces19 = 1024;
            Protocol.WriteConsoleMsg(sendIndex,
                "Asesino: " + Convert.ToInt32(ES.GetVar(CharFile, "REP", "Asesino", ref argEmptySpaces19)),
                Protocol.FontTypeNames.FONTTYPE_INFO);
            var argEmptySpaces20 = 1024;
            Protocol.WriteConsoleMsg(sendIndex,
                "Noble: " + Convert.ToInt32(ES.GetVar(CharFile, "REP", "Nobles", ref argEmptySpaces20)),
                Protocol.FontTypeNames.FONTTYPE_INFO);

            var argEmptySpaces22 = 1024;
            var guildIndexStr = ES.GetVar(CharFile, "Guild", "GUILDINDEX", ref argEmptySpaces22);
            if (short.TryParse(guildIndexStr, out var guildIndex))
            {
                Protocol.WriteConsoleMsg(sendIndex,
                    "Clan: " + modGuilds.GuildName(guildIndex), Protocol.FontTypeNames.FONTTYPE_INFO);
            }

            var argEmptySpaces23 = 1024;
            Ban = ES.GetVar(CharFile, "FLAGS", "Ban", ref argEmptySpaces23);
            Protocol.WriteConsoleMsg(sendIndex, "Ban: " + Ban, Protocol.FontTypeNames.FONTTYPE_INFO);

            if (Ban == "1")
            {
                var argEmptySpaces24 = 1024;
                var argEmptySpaces25 = 1024;
                Protocol.WriteConsoleMsg(sendIndex,
                    "Ban por: " + ES.GetVar(CharFile, charName, "BannedBy", ref argEmptySpaces24) + " Motivo: " +
                    ES.GetVar(BanDetailPath, charName, "Reason", ref argEmptySpaces25),
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }
        }
        else
        {
            Protocol.WriteConsoleMsg(sendIndex, "El pj no existe: " + charName, Protocol.FontTypeNames.FONTTYPE_INFO);
        }
    }

    public static void SendUserInvTxt(short sendIndex, short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            int j;

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                Protocol.WriteConsoleMsg(sendIndex, withBlock.name, Protocol.FontTypeNames.FONTTYPE_INFO);
                Protocol.WriteConsoleMsg(sendIndex, "Tiene " + withBlock.Invent.NroItems + " objetos.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);

                var loopTo = (int)withBlock.CurrentInventorySlots;
                for (j = 1; j <= loopTo; j++)
                    if (withBlock.Invent.userObj[j].ObjIndex > 0)
                        Protocol.WriteConsoleMsg(sendIndex,
                            "Objeto " + j + " " +
                            Declaraciones.objData[withBlock.Invent.userObj[j].ObjIndex].name +
                            " Cantidad:" + withBlock.Invent.userObj[j].Amount,
                            Protocol.FontTypeNames.FONTTYPE_INFO);
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in ActStats: " + ex.Message);
        }
    }

    public static void SendUserInvTxtFromChar(short sendIndex, string charName)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            int j;
            string CharFile, Tmp;
            int ObjInd, ObjCant;

            CharFile = Declaraciones.CharPath + charName + ".chr";

            if (File.Exists(CharFile))
            {
                Protocol.WriteConsoleMsg(sendIndex, charName, Protocol.FontTypeNames.FONTTYPE_INFO);
                var argEmptySpaces = 1024;
                Protocol.WriteConsoleMsg(sendIndex,
                    "Tiene " + ES.GetVar(CharFile, "Inventory", "CantidadItems", ref argEmptySpaces) + " objetos.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);

                for (j = 1; j <= Declaraciones.MAX_INVENTORY_SLOTS; j++)
                {
                    var argEmptySpaces1 = 1024;
                    Tmp = ES.GetVar(CharFile, "Inventory", "Obj" + j, ref argEmptySpaces1);
                    ObjInd = Convert.ToInt32(General.ReadField(1, ref Tmp, (byte)'-'));
                    ObjCant = Convert.ToInt32(General.ReadField(2, ref Tmp, (byte)'-'));
                    if (ObjInd > 0)
                        Protocol.WriteConsoleMsg(sendIndex,
                            "Objeto " + j + " " + Declaraciones.objData[ObjInd].name + " Cantidad:" + ObjCant,
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                }
            }
            else
            {
                Protocol.WriteConsoleMsg(sendIndex, "Usuario inexistente: " + charName,
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in SendUserInvTxtFromChar: " + ex.Message);
        }
    }

    public static void SendUserSkillsTxt(short sendIndex, short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            short j;

            Protocol.WriteConsoleMsg(sendIndex, Declaraciones.UserList[UserIndex].name,
                Protocol.FontTypeNames.FONTTYPE_INFO);

            for (j = 1; j <= Declaraciones.NUMSKILLS; j++)
                Protocol.WriteConsoleMsg(sendIndex,
                    Declaraciones.SkillsNames[j] + " = " + Declaraciones.UserList[UserIndex].Stats.UserSkills[j],
                    Protocol.FontTypeNames.FONTTYPE_INFO);

            Protocol.WriteConsoleMsg(sendIndex, "SkillLibres:" + Declaraciones.UserList[UserIndex].Stats.SkillPts,
                Protocol.FontTypeNames.FONTTYPE_INFO);
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in SendUserSkillsTxt: " + ex.Message);
        }
    }

    private static bool EsMascotaCiudadano(short NpcIndex, short UserIndex)
    {
        bool EsMascotaCiudadanoRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if (Declaraciones.Npclist[NpcIndex].MaestroUser > 0)
        {
            EsMascotaCiudadanoRet = !ES.criminal(Declaraciones.Npclist[NpcIndex].MaestroUser);
            if (EsMascotaCiudadanoRet)
                Protocol.WriteConsoleMsg(Declaraciones.Npclist[NpcIndex].MaestroUser,
                    "¡¡" + Declaraciones.UserList[UserIndex].name + " esta atacando tu mascota!!",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
        }

        return EsMascotaCiudadanoRet;
    }

    public static void NPCAtacado(short NpcIndex, short UserIndex)
    {
        // **********************************************
        // Author: Unknown
        // Last Modification: 02/04/2010
        // 24/01/2007 -> Pablo (ToxicWaste): Agrego para que se actualize el tag si corresponde.
        // 24/07/2007 -> Pablo (ToxicWaste): Guardar primero que ataca NPC y el que atacas ahora.
        // 06/28/2008 -> NicoNZ: Los elementales al atacarlos por su amo no se paran más al lado de él sin hacer nada.
        // 02/04/2010: ZaMa: Un ciuda no se vuelve mas criminal al atacar un Npc no hostil.
        // **********************************************
        bool EraCriminal;

        // Guardamos el usuario que ataco el Npc.
        Declaraciones.Npclist[NpcIndex].flags.AttackedBy = Declaraciones.UserList[UserIndex].name;

        // Npc que estabas atacando.
        short LastNpcHit;
        LastNpcHit = Declaraciones.UserList[UserIndex].flags.NPCAtacado;
        // Guarda el NPC que estas atacando ahora.
        Declaraciones.UserList[UserIndex].flags.NPCAtacado = NpcIndex;

        // Revisamos robo de Npc.
        // Guarda el primer nick que lo ataca.
        if (string.IsNullOrEmpty(Declaraciones.Npclist[NpcIndex].flags.AttackedFirstBy))
        {
            // El que le pegabas antes ya no es tuyo
            if (LastNpcHit != 0)
                if ((Declaraciones.Npclist[LastNpcHit].flags.AttackedFirstBy ?? "") ==
                    (Declaraciones.UserList[UserIndex].name ?? ""))
                    Declaraciones.Npclist[LastNpcHit].flags.AttackedFirstBy = string.Empty;

            Declaraciones.Npclist[NpcIndex].flags.AttackedFirstBy = Declaraciones.UserList[UserIndex].name;
        }
        else if ((Declaraciones.Npclist[NpcIndex].flags.AttackedFirstBy ?? "") !=
                 (Declaraciones.UserList[UserIndex].name ?? ""))
        {
            // Estas robando NPC
            // El que le pegabas antes ya no es tuyo
            if (LastNpcHit != 0)
                if ((Declaraciones.Npclist[LastNpcHit].flags.AttackedFirstBy ?? "") ==
                    (Declaraciones.UserList[UserIndex].name ?? ""))
                    Declaraciones.Npclist[LastNpcHit].flags.AttackedFirstBy = string.Empty;
        }

        if (Declaraciones.Npclist[NpcIndex].MaestroUser > 0)
            if (Declaraciones.Npclist[NpcIndex].MaestroUser != UserIndex)
                SistemaCombate.AllMascotasAtacanUser(UserIndex, Declaraciones.Npclist[NpcIndex].MaestroUser);

        if (EsMascotaCiudadano(NpcIndex, UserIndex))
        {
            VolverCriminal(UserIndex);
            Declaraciones.Npclist[NpcIndex].Movement = AI.TipoAI.NPCDEFENSA;
            Declaraciones.Npclist[NpcIndex].Hostile = 1;
        }
        else
        {
            EraCriminal = ES.criminal(UserIndex);

            // Reputacion
            if (Declaraciones.Npclist[NpcIndex].Stats.Alineacion == 0)
            {
                if (Declaraciones.Npclist[NpcIndex].NPCtype == Declaraciones.eNPCType.GuardiaReal)
                    VolverCriminal(UserIndex);
            }

            else if (Declaraciones.Npclist[NpcIndex].Stats.Alineacion == 1)
            {
                Declaraciones.UserList[UserIndex].Reputacion.PlebeRep =
                    Declaraciones.UserList[UserIndex].Reputacion.PlebeRep +
                    Convert.ToInt32(Declaraciones.vlCAZADOR / 2d);
                if (Declaraciones.UserList[UserIndex].Reputacion.PlebeRep > Declaraciones.MAXREP)
                    Declaraciones.UserList[UserIndex].Reputacion.PlebeRep = Declaraciones.MAXREP;
            }

            if (Declaraciones.Npclist[NpcIndex].MaestroUser != UserIndex)
            {
                // hacemos que el Npc se defienda
                Declaraciones.Npclist[NpcIndex].Movement = AI.TipoAI.NPCDEFENSA;
                Declaraciones.Npclist[NpcIndex].Hostile = 1;
            }

            if (EraCriminal & !ES.criminal(UserIndex)) VolverCiudadano(UserIndex);
        }
    }

    public static bool PuedeApuñalar(short UserIndex)
    {
        bool PuedeApuñalarRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if (Declaraciones.UserList[UserIndex].Invent.WeaponEqpObjIndex > 0)
            if (Declaraciones.objData[Declaraciones.UserList[UserIndex].Invent.WeaponEqpObjIndex].Apuñala == 1)
                PuedeApuñalarRet =
                    (Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Apuñalar] >=
                     Declaraciones.MIN_APUÑALAR) |
                    (Declaraciones.UserList[UserIndex].clase == Declaraciones.eClass.Assasin);

        return PuedeApuñalarRet;
    }

    public static bool PuedeAcuchillar(short UserIndex)
    {
        bool PuedeAcuchillarRet = default;
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 25/01/2010 (ZaMa)
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.clase == Declaraciones.eClass.Pirat)
                if (withBlock.Invent.WeaponEqpObjIndex > 0)
                    PuedeAcuchillarRet = Declaraciones.objData[withBlock.Invent.WeaponEqpObjIndex].Acuchilla ==
                                         1;
        }

        return PuedeAcuchillarRet;
    }

    public static void SubirSkill(short UserIndex, short Skill, bool Acerto)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 11/19/2009
        // 11/19/2009 Pato - Implement the new system to train the skills.
        // *************************************************
        short Lvl;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if ((withBlock.flags.Hambre == 0) & (withBlock.flags.Sed == 0))
            {
                if (withBlock.Counters.AsignedSkills < 10)
                {
                    if (!(withBlock.flags.UltimoMensaje == 7))
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "Para poder entrenar un skill debes asignar los 10 skills iniciales.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        withBlock.flags.UltimoMensaje = 7;
                    }

                    return;
                }

                {
                    ref var withBlock1 = ref withBlock.Stats;
                    if (withBlock1.UserSkills[Skill] == Declaraciones.MAXSKILLPOINTS)
                        return;

                    Lvl = withBlock1.ELV;

                    if (Lvl > Declaraciones.levelSkill.Length - 1)
                        Lvl = Convert.ToInt16(Declaraciones.levelSkill.Length - 1);

                    if (withBlock1.UserSkills[Skill] >= Declaraciones.levelSkill[Lvl].LevelValue)
                        return;

                    if (Acerto)
                        withBlock1.ExpSkills[Skill] = withBlock1.ExpSkills[Skill] + Declaraciones.EXP_ACIERTO_SKILL;
                    else
                        withBlock1.ExpSkills[Skill] = withBlock1.ExpSkills[Skill] + Declaraciones.EXP_FALLO_SKILL;

                    if (withBlock1.ExpSkills[Skill] >= withBlock1.EluSkills[Skill])
                    {
                        withBlock1.UserSkills[Skill] = Convert.ToByte(withBlock1.UserSkills[Skill] + 1);
                        Protocol.WriteConsoleMsg(UserIndex,
                            "¡Has mejorado tu skill " + Declaraciones.SkillsNames[Skill] +
                            " en un punto! Ahora tienes " + withBlock1.UserSkills[Skill] + " pts.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);

                        withBlock1.Exp = withBlock1.Exp + 50d;
                        if (withBlock1.Exp > Declaraciones.MAXEXP)
                            withBlock1.Exp = Declaraciones.MAXEXP;

                        Protocol.WriteConsoleMsg(UserIndex, "¡Has ganado 50 puntos de experiencia!",
                            Protocol.FontTypeNames.FONTTYPE_FIGHT);

                        Protocol.WriteUpdateExp(UserIndex);
                        CheckUserLevel(UserIndex);
                        CheckEluSkill(UserIndex, Convert.ToByte(Skill), false);
                    }
                }
            }
        }
    }

    // '
    // Muere un usuario
    // 
    // @param UserIndex  Indice del usuario que muere
    // 

    public static void UserDie(short UserIndex)
    {
        // ************************************************
        // Author: Uknown
        // Last Modified: 12/01/2010 (ZaMa)
        // 04/15/2008: NicoNZ - Ahora se resetea el counter del invi
        // 13/02/2009: ZaMa - Ahora se borran las mascotas cuando moris en agua.
        // 27/05/2009: ZaMa - El seguro de resu no se activa si estas en una arena.
        // 21/07/2009: Marco - Al morir se desactiva el comercio seguro.
        // 16/11/2009: ZaMa - Al morir perdes la criatura que te pertenecia.
        // 27/11/2009: Budi - Al morir envia los atributos originales.
        // 12/01/2010: ZaMa - Los druidas pierden la inmunidad de ser atacados cuando mueren.
        // ************************************************
        try
        {
            int i;
            short aN;

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // Sonido
                if (withBlock.Genero == Declaraciones.eGenero.Mujer)
                    Declaraciones.SonidosMapas.ReproducirSonido(modSendData.SendTarget.ToPCArea, UserIndex,
                        Convert.ToInt16((int)SoundMapInfo.e_SoundIndex.MUERTE_MUJER));
                else
                    Declaraciones.SonidosMapas.ReproducirSonido(modSendData.SendTarget.ToPCArea, UserIndex,
                        Convert.ToInt16((int)SoundMapInfo.e_SoundIndex.MUERTE_HOMBRE));

                // Quitar el dialogo del user muerto
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    Protocol.PrepareMessageRemoveCharDialog(withBlock.character.CharIndex));

                withBlock.Stats.MinHp = 0;
                withBlock.Stats.MinSta = 0;
                withBlock.flags.AtacadoPorUser = 0;
                withBlock.flags.Envenenado = 0;
                withBlock.flags.Muerto = 1;
                // No se activa en arenas
                if (SistemaCombate.TriggerZonaPelea(UserIndex, UserIndex) != Declaraciones.eTrigger6.TRIGGER6_PERMITE)
                {
                    withBlock.flags.SeguroResu = true;
                    Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.ResuscitationSafeOn);
                }
                // Call WriteResuscitationSafeOn(UserIndex)
                else
                {
                    withBlock.flags.SeguroResu = false;
                    Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.ResuscitationSafeOff);
                    // Call WriteResuscitationSafeOff(UserIndex)
                }

                aN = withBlock.flags.AtacadoPorNpc;
                if (aN > 0)
                {
                    Declaraciones.Npclist[aN].Movement = Declaraciones.Npclist[aN].flags.OldMovement;
                    Declaraciones.Npclist[aN].Hostile = Declaraciones.Npclist[aN].flags.OldHostil;
                    Declaraciones.Npclist[aN].flags.AttackedBy = string.Empty;
                }

                aN = withBlock.flags.NPCAtacado;
                if (aN > 0)
                    if ((Declaraciones.Npclist[aN].flags.AttackedFirstBy ?? "") == (withBlock.name ?? ""))
                        Declaraciones.Npclist[aN].flags.AttackedFirstBy = string.Empty;

                withBlock.flags.AtacadoPorNpc = 0;
                withBlock.flags.NPCAtacado = 0;
                PerdioNpc(UserIndex);

                // <<<< Atacable >>>>
                if (withBlock.flags.AtacablePor > 0)
                {
                    withBlock.flags.AtacablePor = 0;
                    RefreshCharStatus(UserIndex);
                }

                // <<<< Paralisis >>>>
                if (withBlock.flags.Paralizado == 1)
                {
                    withBlock.flags.Paralizado = 0;
                    Protocol.WriteParalizeOK(UserIndex);
                }

                // <<< Estupidez >>>
                if (withBlock.flags.Estupidez == 1)
                {
                    withBlock.flags.Estupidez = 0;
                    Protocol.WriteDumbNoMore(UserIndex);
                }

                // <<<< Descansando >>>>
                if (withBlock.flags.Descansar)
                {
                    withBlock.flags.Descansar = false;
                    Protocol.WriteRestOK(UserIndex);
                }

                // <<<< Meditando >>>>
                if (withBlock.flags.Meditando)
                {
                    withBlock.flags.Meditando = false;
                    Protocol.WriteMeditateToggle(UserIndex);
                }

                // <<<< Invisible >>>>
                if ((withBlock.flags.invisible == 1) | (withBlock.flags.Oculto == 1))
                {
                    withBlock.flags.Oculto = 0;
                    withBlock.flags.invisible = 0;
                    withBlock.Counters.TiempoOculto = 0;
                    withBlock.Counters.Invisibilidad = 0;

                    // Call SendData(SendTarget.ToPCArea, UserIndex, PrepareMessageSetInvisible(.Char.CharIndex, False))
                    SetInvisible(UserIndex, Declaraciones.UserList[UserIndex].character.CharIndex, false);
                }

                if (SistemaCombate.TriggerZonaPelea(UserIndex, UserIndex) != Declaraciones.eTrigger6.TRIGGER6_PERMITE)
                {
                    // << Si es newbie no pierde el inventario >>
                    if (!Extra.EsNewbie(UserIndex))
                        InvUsuario.TirarTodo(UserIndex);
                    else
                        InvUsuario.TirarTodosLosItemsNoNewbies(UserIndex);
                }

                // DESEQUIPA TODOS LOS OBJETOS
                // desequipar armadura
                if (withBlock.Invent.ArmourEqpObjIndex > 0)
                    InvUsuario.Desequipar(UserIndex, withBlock.Invent.ArmourEqpSlot);

                // desequipar arma
                if (withBlock.Invent.WeaponEqpObjIndex > 0)
                    InvUsuario.Desequipar(UserIndex, withBlock.Invent.WeaponEqpSlot);

                // desequipar casco
                if (withBlock.Invent.CascoEqpObjIndex > 0)
                    InvUsuario.Desequipar(UserIndex, withBlock.Invent.CascoEqpSlot);

                // desequipar herramienta
                if (withBlock.Invent.AnilloEqpSlot > 0)
                    InvUsuario.Desequipar(UserIndex, withBlock.Invent.AnilloEqpSlot);

                // desequipar municiones
                if (withBlock.Invent.MunicionEqpObjIndex > 0)
                    InvUsuario.Desequipar(UserIndex, withBlock.Invent.MunicionEqpSlot);

                // desequipar escudo
                if (withBlock.Invent.EscudoEqpObjIndex > 0)
                    InvUsuario.Desequipar(UserIndex, withBlock.Invent.EscudoEqpSlot);

                // << Reseteamos los posibles FX sobre el personaje >>
                if (withBlock.character.loops == Declaraciones.INFINITE_LOOPS)
                {
                    withBlock.character.FX = 0;
                    withBlock.character.loops = 0;
                }

                // << Restauramos el mimetismo
                if (withBlock.flags.Mimetizado == 1)
                {
                    withBlock.character.body = withBlock.CharMimetizado.body;
                    withBlock.character.Head = withBlock.CharMimetizado.Head;
                    withBlock.character.CascoAnim = withBlock.CharMimetizado.CascoAnim;
                    withBlock.character.ShieldAnim = withBlock.CharMimetizado.ShieldAnim;
                    withBlock.character.WeaponAnim = withBlock.CharMimetizado.WeaponAnim;
                    withBlock.Counters.Mimetismo = 0;
                    withBlock.flags.Mimetizado = 0;
                    // Puede ser atacado por npcs (cuando resucite)
                    withBlock.flags.Ignorado = false;
                }

                // << Restauramos los atributos >>
                if (withBlock.flags.TomoPocion)
                    for (i = 1; i <= 5; i++)
                        withBlock.Stats.UserAtributos[i] = withBlock.Stats.UserAtributosBackUP[i];

                // << Cambiamos la apariencia del char >>
                if (withBlock.flags.Navegando == 0)
                {
                    withBlock.character.body = Declaraciones.iCuerpoMuerto;
                    withBlock.character.Head = Declaraciones.iCabezaMuerto;
                    withBlock.character.ShieldAnim = Declaraciones.NingunEscudo;
                    withBlock.character.WeaponAnim = Declaraciones.NingunArma;
                    withBlock.character.CascoAnim = Declaraciones.NingunCasco;
                }
                else
                {
                    withBlock.character.body = Declaraciones.iFragataFantasmal;
                }

                for (i = 1; i <= Declaraciones.MAXMASCOTAS; i++)
                    if (withBlock.MascotasIndex[i] > 0)
                        NPCs.MuereNpc(withBlock.MascotasIndex[i], 0);
                    // Si estan en agua o zona segura
                    else
                        withBlock.MascotasType[i] = 0;

                withBlock.NroMascotas = 0;

                // << Actualizamos clientes >>
                ChangeUserChar(UserIndex, withBlock.character.body, withBlock.character.Head,
                    (byte)withBlock.character.heading, Declaraciones.NingunArma, Declaraciones.NingunEscudo,
                    Declaraciones.NingunCasco);
                Protocol.WriteUpdateUserStats(UserIndex);
                Protocol.WriteUpdateStrenghtAndDexterity(UserIndex);
                // <<Castigos por party>>
                if (withBlock.PartyIndex > 0)
                    mdParty.ObtenerExito(UserIndex, withBlock.Stats.ELV * -10 * mdParty.CantMiembros(UserIndex),
                        ref withBlock.Pos.Map, ref withBlock.Pos.X, ref withBlock.Pos.Y);

                // <<Cerramos comercio seguro>>
                TCP.LimpiarComercioSeguro(UserIndex);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in PuedeAtravesarAgua: " + ex.Message);
            var argdesc = "Error en SUB USERDIE. Error: " + ex.GetType().Name + " Descripción: " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    public static void ContarMuerte(short Muerto, short Atacante)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if (Extra.EsNewbie(Muerto))
            return;

        {
            ref var withBlock = ref Declaraciones.UserList[Atacante];
            if (SistemaCombate.TriggerZonaPelea(Muerto, Atacante) == Declaraciones.eTrigger6.TRIGGER6_PERMITE)
                return;

            if (ES.criminal(Muerto))
            {
                if ((withBlock.flags.LastCrimMatado ?? "") != (Declaraciones.UserList[Muerto].name ?? ""))
                {
                    withBlock.flags.LastCrimMatado = Declaraciones.UserList[Muerto].name;
                    if (withBlock.Faccion.CriminalesMatados < Declaraciones.MAXUSERMATADOS)
                        withBlock.Faccion.CriminalesMatados = withBlock.Faccion.CriminalesMatados + 1;
                }

                if ((withBlock.Faccion.RecibioExpInicialCaos == 1) &
                    (Declaraciones.UserList[Muerto].Faccion.FuerzasCaos == 1))
                    withBlock.Faccion.Reenlistadas = 200; // jaja que trucho
                // con esto evitamos que se vuelva a reenlistar
            }
            else if ((withBlock.flags.LastCiudMatado ?? "") != (Declaraciones.UserList[Muerto].name ?? ""))
            {
                withBlock.flags.LastCiudMatado = Declaraciones.UserList[Muerto].name;
                if (withBlock.Faccion.CiudadanosMatados < Declaraciones.MAXUSERMATADOS)
                    withBlock.Faccion.CiudadanosMatados = withBlock.Faccion.CiudadanosMatados + 1;
            }

            if (withBlock.Stats.UsuariosMatados < Declaraciones.MAXUSERMATADOS)
                withBlock.Stats.UsuariosMatados = withBlock.Stats.UsuariosMatados + 1;
        }
    }

    public static void Tilelibre(ref Declaraciones.WorldPos Pos, ref Declaraciones.WorldPos nPos,
        ref Declaraciones.Obj Obj, ref bool Agua, ref bool Tierra)
    {
        // **************************************************************
        // Author: Unknown
        // Last Modify Date: 23/01/2007
        // 23/01/2007 -> Pablo (ToxicWaste): El agua es ahora un TileLibre agregando las condiciones necesarias.
        // **************************************************************
        var LoopC = default(short);
        int tX;
        int tY;
        bool hayobj;

        hayobj = false;
        nPos.Map = Pos.Map;
        nPos.X = 0;
        nPos.Y = 0;

        while (!Extra.LegalPos(Pos.Map, nPos.X, nPos.Y, Agua, Tierra) | hayobj)
        {
            if (LoopC > 15) break;

            var loopTo = Pos.Y + LoopC;
            for (tY = Pos.Y - LoopC; tY <= loopTo; tY++)
            {
                var loopTo1 = Pos.X + LoopC;
                for (tX = Pos.X - LoopC; tX <= loopTo1; tX++)
                    if (Extra.LegalPos(nPos.Map, Convert.ToInt16(tX), Convert.ToInt16(tY), Agua, Tierra))
                    {
                        // We continue if: a - the item is different from 0 and the dropped item or b - the amount dropped + amount in map exceeds MAX_INVENTORY_OBJS
                        hayobj = (Declaraciones.MapData[nPos.Map, tX, tY].ObjInfo.ObjIndex > 0) &
                                 (Declaraciones.MapData[nPos.Map, tX, tY].ObjInfo.ObjIndex != Obj.ObjIndex);
                        if (!hayobj)
                            hayobj = (short)(Declaraciones.MapData[nPos.Map, tX, tY].ObjInfo.Amount + Obj.Amount) >
                                     Declaraciones.MAX_INVENTORY_OBJS;
                        if (!hayobj & (Declaraciones.MapData[nPos.Map, tX, tY].TileExit.Map == 0))
                        {
                            nPos.X = Convert.ToInt16(tX);
                            nPos.Y = Convert.ToInt16(tY);

                            // break both fors
                            tX = Pos.X + LoopC;
                            tY = Pos.Y + LoopC;
                        }
                    }
            }

            LoopC = Convert.ToInt16(LoopC + 1);
        }
    }

    public static void WarpUserChar(short UserIndex, short Map, short X, short Y, bool FX, bool Teletransported = false)
    {
        // **************************************************************
        // Author: Unknown
        // Last Modify Date: 13/11/2009
        // 15/07/2009 - ZaMa: Automatic toogle navigate after warping to water.
        // 13/11/2009 - ZaMa: Now it's activated the timer which determines if the Npc can atacak the user.
        // **************************************************************
        short OldMap;
        short OldX;
        short OldY;

        bool nextMap;
        bool previousMap;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Quitar el dialogo
            modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                Protocol.PrepareMessageRemoveCharDialog(withBlock.character.CharIndex));

            Protocol.WriteRemoveAllDialogs(UserIndex);

            OldMap = withBlock.Pos.Map;
            OldX = withBlock.Pos.X;
            OldY = withBlock.Pos.Y;

            EraseUserChar(UserIndex, withBlock.flags.AdminInvisible == 1);

            if (OldMap != Map)
            {
                Protocol.WriteChangeMap(UserIndex, Map, Declaraciones.mapInfo[withBlock.Pos.Map].MapVersion);
                Protocol.WritePlayMidi(UserIndex,
                    Convert.ToByte(Migration.ParseVal(General.ReadField(1, ref Declaraciones.mapInfo[Map].Music,
                        45))));

                // Update new Map Users
                Declaraciones.mapInfo[Map].NumUsers =
                    Convert.ToInt16(Declaraciones.mapInfo[Map].NumUsers + 1);

                // Update old Map Users
                Declaraciones.mapInfo[OldMap].NumUsers =
                    Convert.ToInt16(Declaraciones.mapInfo[OldMap].NumUsers - 1);
                if (Declaraciones.mapInfo[OldMap].NumUsers < 0)
                    Declaraciones.mapInfo[OldMap].NumUsers = 0;

                // Si el mapa al que entro NO ES superficial AND en el que estaba TAMPOCO ES superficial, ENTONCES
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nextMap. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                nextMap = Declaraciones.distanceToCities[Map].distanceToCity[(int)withBlock.Hogar] >= 0 ? true : false;
                previousMap =
                    Declaraciones.distanceToCities[withBlock.Pos.Map].distanceToCity[(int)withBlock.Hogar] >= 0
                        ? true
                        : false;

                if (previousMap & nextMap) // 138 => 139 (Ambos superficiales, no tiene que pasar nada)
                {
                }
                // NO PASA NADA PORQUE NO ENTRO A UN DUNGEON.
                else if (previousMap & !nextMap)
                {
                    // 139 => 140 (139 es superficial, 140 no. Por lo tanto 139 es el ultimo mapa superficial)
                    withBlock.flags.lastMap = withBlock.Pos.Map;
                }
                else if (!previousMap & nextMap)
                {
                    // 140 => 139 (140 es no es superficial, 139 si. Por lo tanto, el último mapa es 0 ya que no esta en un dungeon)
                    withBlock.flags.lastMap = 0;
                }
                else if (!previousMap & !nextMap)
                {
                    // 140 => 141 (Ninguno es superficial, el ultimo mapa es el mismo de antes)
                    // withBlock.flags.lastMap se mantiene sin cambios
                }
            }

            withBlock.Pos.X = X;
            withBlock.Pos.Y = Y;
            withBlock.Pos.Map = Map;

            var argButIndex = false;
            MakeUserChar(true, Map, UserIndex, Map, X, Y, ref argButIndex);
            Protocol.WriteUserCharIndexInServer(UserIndex);

            // Force a flush, so user index is in there before it's destroyed for teleporting
            Protocol.FlushBuffer(UserIndex);

            // Seguis invisible al pasar de mapa
            if (((withBlock.flags.invisible == 1) | (withBlock.flags.Oculto == 1)) &
                !(withBlock.flags.AdminInvisible == 1)) SetInvisible(UserIndex, withBlock.character.CharIndex, true);
            // Call SendData(SendTarget.ToPCArea, UserIndex, PrepareMessageSetInvisible(.Char.CharIndex, True))
            if (Teletransported)
                if (withBlock.flags.Traveling == 1)
                {
                    withBlock.flags.Traveling = 0;
                    withBlock.Counters.goHome = 0;
                    Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.CancelHome);
                }

            if (FX & (withBlock.flags.AdminInvisible == 0)) // FX
            {
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    Protocol.PrepareMessagePlayWave(Declaraciones.SND_WARP, Convert.ToByte(X), Convert.ToByte(Y)));
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    Protocol.PrepareMessageCreateFX(withBlock.character.CharIndex, (short)Declaraciones.FXIDs.FXWARP,
                        0));
            }

            if (withBlock.NroMascotas != 0)
                WarpMascotas(UserIndex);

            // No puede ser atacado cuando cambia de mapa, por cierto tiempo
            modNuevoTimer.IntervaloPermiteSerAtacado(UserIndex, true);

            // Perdes el Npc al cambiar de mapa
            PerdioNpc(UserIndex);

            // Automatic toogle navigate
            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero)) ==
                0)
            {
                if (General.HayAgua(withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y))
                {
                    if (withBlock.flags.Navegando == 0)
                    {
                        withBlock.flags.Navegando = 1;

                        // Tell the client that we are navigating.
                        Protocol.WriteNavigateToggle(UserIndex);
                    }
                }
                else if (withBlock.flags.Navegando == 1)
                {
                    withBlock.flags.Navegando = 0;

                    // Tell the client that we are navigating.
                    Protocol.WriteNavigateToggle(UserIndex);
                }
            }
        }
    }

    private static void WarpMascotas(short UserIndex)
    {
        // ************************************************
        // Author: Uknown
        // Last Modified: 11/05/2009
        // 13/02/2009: ZaMa - Arreglado respawn de mascotas al cambiar de mapa.
        // 13/02/2009: ZaMa - Las mascotas no regeneran su vida al cambiar de mapa (Solo entre mapas inseguros).
        // 11/05/2009: ZaMa - Chequeo si la mascota pueden spwnear para asiganrle los stats.
        // ************************************************
        short i;
        short petType;
        var PetRespawn = default(bool);
        var PetTiempoDeVida = default(short);
        short NroPets;
        var InvocadosMatados = default(short);
        bool canWarp;
        short Index;
        var iMinHP = default(short);

        NroPets = Declaraciones.UserList[UserIndex].NroMascotas;
        canWarp = Declaraciones.mapInfo[Declaraciones.UserList[UserIndex].Pos.Map].Pk;

        for (i = 1; i <= Declaraciones.MAXMASCOTAS; i++)
        {
            Index = Declaraciones.UserList[UserIndex].MascotasIndex[i];

            if (Index > 0)
            {
                // si la mascota tiene tiempo de vida > 0 significa q fue invocada => we kill it
                if (Declaraciones.Npclist[Index].Contadores.TiempoExistencia > 0)
                {
                    NPCs.QuitarNPC(Index);
                    Declaraciones.UserList[UserIndex].MascotasIndex[i] = 0;
                    InvocadosMatados = Convert.ToInt16(InvocadosMatados + 1);
                    NroPets = Convert.ToInt16(NroPets - 1);

                    petType = 0;
                }
                else
                {
                    // Store data and remove NPC to recreate it after warp
                    // PetRespawn = Npclist(index).flags.Respawn = 0
                    petType = Declaraciones.UserList[UserIndex].MascotasType[i];
                    // PetTiempoDeVida = Npclist(index).Contadores.TiempoExistencia

                    // Guardamos el hp, para restaurarlo uando se cree el Npc
                    iMinHP = Convert.ToInt16(Declaraciones.Npclist[Index].Stats.MinHp);

                    NPCs.QuitarNPC(Index);

                    // Restauramos el valor de la variable
                    Declaraciones.UserList[UserIndex].MascotasType[i] = petType;
                }
            }
            else if (Declaraciones.UserList[UserIndex].MascotasType[i] > 0)
            {
                // Store data and remove NPC to recreate it after warp
                PetRespawn = true;
                petType = Declaraciones.UserList[UserIndex].MascotasType[i];
                PetTiempoDeVida = 0;
            }
            else
            {
                petType = 0;
            }

            if ((petType > 0) & canWarp)
            {
                Index = NPCs.SpawnNpc(petType, ref Declaraciones.UserList[UserIndex].Pos, false, PetRespawn);

                // Controlamos que se sumoneo OK - should never happen. Continue to allow removal of other pets if not alone
                // Exception: Pets don't spawn in water if they can't swim
                if (Index == 0)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Tus mascotas no pueden transitar este mapa.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                }
                else
                {
                    Declaraciones.UserList[UserIndex].MascotasIndex[i] = Index;

                    // Nos aseguramos de que conserve el hp, si estaba dañado
                    Declaraciones.Npclist[Index].Stats.MinHp =
                        iMinHP == 0 ? Declaraciones.Npclist[Index].Stats.MinHp : Convert.ToInt32(iMinHP);

                    Declaraciones.Npclist[Index].MaestroUser = UserIndex;
                    Declaraciones.Npclist[Index].Contadores.TiempoExistencia = PetTiempoDeVida;
                    NPCs.FollowAmo(Index);
                }
            }
        }

        if (InvocadosMatados > 0)
            Protocol.WriteConsoleMsg(UserIndex, "Pierdes el control de tus mascotas invocadas.",
                Protocol.FontTypeNames.FONTTYPE_INFO);

        if (!canWarp)
            Protocol.WriteConsoleMsg(UserIndex, "No se permiten mascotas en zona segura. Éstas te esperarán afuera.",
                Protocol.FontTypeNames.FONTTYPE_INFO);

        Declaraciones.UserList[UserIndex].NroMascotas = NroPets;
    }

    public static void WarpMascota(short UserIndex, short PetIndex)
    {
        // ************************************************
        // Author: ZaMa
        // Last Modified: 18/11/2009
        // Warps a pet without changing its stats
        // ************************************************
        short petType;
        short NpcIndex;
        short iMinHP;
        Declaraciones.WorldPos TargetPos;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];

            TargetPos.Map = withBlock.flags.TargetMap;
            TargetPos.X = withBlock.flags.TargetX;
            TargetPos.Y = withBlock.flags.TargetY;

            NpcIndex = withBlock.MascotasIndex[PetIndex];

            // Store data and remove NPC to recreate it after warp
            petType = withBlock.MascotasType[PetIndex];

            // Guardamos el hp, para restaurarlo cuando se cree el Npc
            iMinHP = Convert.ToInt16(Declaraciones.Npclist[NpcIndex].Stats.MinHp);

            NPCs.QuitarNPC(NpcIndex);

            // Restauramos el valor de la variable
            withBlock.MascotasType[PetIndex] = petType;
            withBlock.NroMascotas = Convert.ToInt16(withBlock.NroMascotas + 1);
            NpcIndex = NPCs.SpawnNpc(petType, ref TargetPos, false, false);

            // Controlamos que se sumoneo OK - should never happen. Continue to allow removal of other pets if not alone
            // Exception: Pets don't spawn in water if they can't swim
            if (NpcIndex == 0)
            {
                Protocol.WriteConsoleMsg(UserIndex,
                    "Tu mascota no pueden transitar este sector del mapa, intenta invocarla en otra parte.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }
            else
            {
                withBlock.MascotasIndex[PetIndex] = NpcIndex;

                {
                    ref var withBlock1 = ref Declaraciones.Npclist[NpcIndex];
                    // Nos aseguramos de que conserve el hp, si estaba dañado
                    withBlock1.Stats.MinHp = iMinHP == 0 ? withBlock1.Stats.MinHp : Convert.ToInt32(iMinHP);

                    withBlock1.MaestroUser = UserIndex;
                    withBlock1.Movement = AI.TipoAI.SigueAmo;
                    withBlock1.Target = 0;
                    withBlock1.TargetNPC = 0;
                }

                NPCs.FollowAmo(NpcIndex);
            }
        }
    }


    // '
    // Se inicia la salida de un usuario.
    // 
    // @param    UserIndex   El index del usuario que va a salir

    public static void Cerrar_Usuario(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 09/04/08 (NicoNZ)
        // 
        // ***************************************************
        bool isNotVisible;
        var HiddenPirat = default(bool);

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.flags.UserLogged & !withBlock.Counters.Saliendo)
            {
                withBlock.Counters.Saliendo = true;
                withBlock.Counters.salir =
                    Convert.ToInt16(
                        (withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0 &&
                        Declaraciones.mapInfo[withBlock.Pos.Map].Pk
                            ? Admin.IntervaloCerrarConexion
                            : 0);

                isNotVisible = withBlock.flags.Oculto != 0 || withBlock.flags.invisible != 0;
                if (isNotVisible)
                {
                    withBlock.flags.invisible = 0;

                    if (withBlock.flags.Oculto != 0)
                        if (withBlock.flags.Navegando == 1)
                            if (withBlock.clase == Declaraciones.eClass.Pirat)
                            {
                                // Pierde la apariencia de fragata fantasmal
                                ToogleBoatBody(UserIndex);
                                Protocol.WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                                ChangeUserChar(UserIndex, withBlock.character.body, withBlock.character.Head,
                                    (byte)withBlock.character.heading, Declaraciones.NingunArma,
                                    Declaraciones.NingunEscudo, Declaraciones.NingunCasco);
                                HiddenPirat = true;
                            }

                    withBlock.flags.Oculto = 0;

                    // Para no repetir mensajes
                    if (!HiddenPirat)
                        Protocol.WriteConsoleMsg(UserIndex, "Has vuelto a ser visible.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);

                    SetInvisible(UserIndex, withBlock.character.CharIndex, false);
                }

                if (withBlock.flags.Traveling == 1)
                    Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.CancelHome);

                Protocol.WriteConsoleMsg(UserIndex,
                    "Cerrando...Se cerrará el juego en " + withBlock.Counters.salir + " segundos...",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Cancels the exit of a user. If it's disconnected it's reset.
    // 
    // @param    UserIndex   The index of the user whose exit is being reset.

    public static void CancelExit(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 04/02/08
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].Counters.Saliendo)
        {
            // Is the user still connected?
            if (Declaraciones.UserList[UserIndex].ConnIDValida)
            {
                Declaraciones.UserList[UserIndex].Counters.Saliendo = false;
                Declaraciones.UserList[UserIndex].Counters.salir = 0;
                Protocol.WriteConsoleMsg(UserIndex, "/salir cancelado.", Protocol.FontTypeNames.FONTTYPE_WARNING);
            }
            else
            {
                // Simply reset
                Declaraciones.UserList[UserIndex].Counters.salir = Convert.ToInt16(
                    (Declaraciones.UserList[UserIndex].flags.Privilegios & Declaraciones.PlayerType.User) != 0 &&
                    Declaraciones.mapInfo[Declaraciones.UserList[UserIndex].Pos.Map].Pk
                        ? Admin.IntervaloCerrarConexion
                        : 0);
            }
        }
    }

    // CambiarNick: Cambia el Nick de un slot.
    // 
    // UserIndex: Quien ejecutó la orden
    // UserIndexDestino: SLot del usuario destino, a quien cambiarle el nick
    // NuevoNick: Nuevo nick de UserIndexDestino
    public static void CambiarNick(short UserIndex, short UserIndexDestino, string NuevoNick)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        string ViejoNick;
        string ViejoCharBackup;

        if (!Declaraciones.UserList[UserIndexDestino].flags.UserLogged)
            return;
        ViejoNick = Declaraciones.UserList[UserIndexDestino].name;

        if (File.Exists(Declaraciones.CharPath + ViejoNick + ".chr"))
        {
            // hace un backup del char
            ViejoCharBackup = Declaraciones.CharPath + ViejoNick + ".chr.old-";
            File.Move(Declaraciones.CharPath + ViejoNick + ".chr", ViejoCharBackup, overwrite: false);
        }
    }

    public static void SendUserStatsTxtOFF(short sendIndex, string Nombre)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int TempSecs;
        string tempStr;
        if (!File.Exists(Declaraciones.CharPath + Nombre + ".chr"))
        {
            Protocol.WriteConsoleMsg(sendIndex, "Pj Inexistente", Protocol.FontTypeNames.FONTTYPE_INFO);
        }
        else
        {
            Protocol.WriteConsoleMsg(sendIndex, "Estadísticas de: " + Nombre, Protocol.FontTypeNames.FONTTYPE_INFO);
            var argEmptySpaces = 1024;
            var argEmptySpaces1 = 1024;
            var argEmptySpaces2 = 1024;
            Protocol.WriteConsoleMsg(sendIndex,
                "Nivel: " + ES.GetVar(Declaraciones.CharPath + Nombre + ".chr", "stats", "elv", ref argEmptySpaces) +
                "  EXP: " + ES.GetVar(Declaraciones.CharPath + Nombre + ".chr", "stats", "Exp", ref argEmptySpaces1) +
                "/" + ES.GetVar(Declaraciones.CharPath + Nombre + ".chr", "stats", "elu", ref argEmptySpaces2),
                Protocol.FontTypeNames.FONTTYPE_INFO);
            var argEmptySpaces3 = 1024;
            var argEmptySpaces4 = 1024;
            Protocol.WriteConsoleMsg(sendIndex,
                "Energía: " +
                ES.GetVar(Declaraciones.CharPath + Nombre + ".chr", "stats", "minsta", ref argEmptySpaces3) + "/" +
                ES.GetVar(Declaraciones.CharPath + Nombre + ".chr", "stats", "maxSta", ref argEmptySpaces4),
                Protocol.FontTypeNames.FONTTYPE_INFO);
            var argEmptySpaces5 = 1024;
            var argEmptySpaces6 = 1024;
            var argEmptySpaces7 = 1024;
            var argEmptySpaces8 = 1024;
            Protocol.WriteConsoleMsg(sendIndex,
                "Salud: " + ES.GetVar(Declaraciones.CharPath + Nombre + ".chr", "stats", "MinHP", ref argEmptySpaces5) +
                "/" + ES.GetVar(Declaraciones.CharPath + Nombre + ".chr", "Stats", "MaxHP", ref argEmptySpaces6) +
                "  Maná: " +
                ES.GetVar(Declaraciones.CharPath + Nombre + ".chr", "Stats", "MinMAN", ref argEmptySpaces7) + "/" +
                ES.GetVar(Declaraciones.CharPath + Nombre + ".chr", "Stats", "MaxMAN", ref argEmptySpaces8),
                Protocol.FontTypeNames.FONTTYPE_INFO);

            var argEmptySpaces9 = 1024;
            Protocol.WriteConsoleMsg(sendIndex,
                "Menor Golpe/Mayor Golpe: " + ES.GetVar(Declaraciones.CharPath + Nombre + ".chr", "stats", "MaxHIT",
                    ref argEmptySpaces9), Protocol.FontTypeNames.FONTTYPE_INFO);

            var argEmptySpaces10 = 1024;
            Protocol.WriteConsoleMsg(sendIndex,
                "Oro: " + ES.GetVar(Declaraciones.CharPath + Nombre + ".chr", "stats", "GLD", ref argEmptySpaces10),
                Protocol.FontTypeNames.FONTTYPE_INFO);

            var argEmptySpaces11 = 1024;
            TempSecs = Convert.ToInt32(ES.GetVar(Declaraciones.CharPath + Nombre + ".chr", "INIT", "UpTime",
                ref argEmptySpaces11));
            tempStr = TempSecs / 86400 + " Días, " + TempSecs % 86400 / 3600 + " Horas, " +
                      TempSecs % 86400 % 3600 / 60 + " Minutos, " + TempSecs % 86400 % 3600 % 60 + " Segundos.";
            Protocol.WriteConsoleMsg(sendIndex, "Tiempo Logeado: " + tempStr, Protocol.FontTypeNames.FONTTYPE_INFO);
        }
    }

    public static void SendUserOROTxtFromChar(short sendIndex, string charName)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        string CharFile;

        try
        {
            CharFile = Declaraciones.CharPath + charName + ".chr";

            if (File.Exists(CharFile))
            {
                Protocol.WriteConsoleMsg(sendIndex, charName, Protocol.FontTypeNames.FONTTYPE_INFO);
                var argEmptySpaces = 1024;
                Protocol.WriteConsoleMsg(sendIndex,
                    "Tiene " + ES.GetVar(CharFile, "STATS", "BANCO", ref argEmptySpaces) + " en el banco.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }
            else
            {
                Protocol.WriteConsoleMsg(sendIndex, "Usuario inexistente: " + charName,
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in EsMascotaCiudadano: " + ex.Message);
        }
    }

    public static void VolverCriminal(short UserIndex)
    {
        // **************************************************************
        // Author: Unknown
        // Last Modify Date: 21/02/2010
        // Nacho: Actualiza el tag al cliente
        // 21/02/2010: ZaMa - Ahora deja de ser atacable si se hace criminal.
        // **************************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger ==
                Declaraciones.eTrigger.ZONAPELEA)
                return;

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero)) !=
                0)
            {
                withBlock.Reputacion.BurguesRep = 0;
                withBlock.Reputacion.NobleRep = 0;
                withBlock.Reputacion.PlebeRep = 0;
                withBlock.Reputacion.BandidoRep = withBlock.Reputacion.BandidoRep + Declaraciones.vlASALTO;
                if (withBlock.Reputacion.BandidoRep > Declaraciones.MAXREP)
                    withBlock.Reputacion.BandidoRep = Declaraciones.MAXREP;
                if (withBlock.Faccion.ArmadaReal == 1)
                {
                    var argExpulsado = true;
                    ModFacciones.ExpulsarFaccionReal(UserIndex, ref argExpulsado);
                }

                if (withBlock.flags.AtacablePor > 0)
                    withBlock.flags.AtacablePor = 0;
            }
        }

        RefreshCharStatus(UserIndex);
    }

    public static void VolverCiudadano(short UserIndex)
    {
        // **************************************************************
        // Author: Unknown
        // Last Modify Date: 21/06/2006
        // Nacho: Actualiza el tag al cliente.
        // **************************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if ((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger == 6)
                return;

            withBlock.Reputacion.LadronesRep = 0;
            withBlock.Reputacion.BandidoRep = 0;
            withBlock.Reputacion.AsesinoRep = 0;
            withBlock.Reputacion.PlebeRep = withBlock.Reputacion.PlebeRep + Declaraciones.vlASALTO;
            if (withBlock.Reputacion.PlebeRep > Declaraciones.MAXREP)
                withBlock.Reputacion.PlebeRep = Declaraciones.MAXREP;
        }

        RefreshCharStatus(UserIndex);
    }

    // '
    // Checks if a given body index is a boat or not.
    // 
    // @param body    The body index to bechecked.
    // @return    True if the body is a boat, false otherwise.

    public static bool BodyIsBoat(short body)
    {
        bool BodyIsBoatRet = default;
        // **************************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modify Date: 10/07/2008
        // Checks if a given body index is a boat
        // **************************************************************
        // TODO : This should be checked somehow else. This is nasty....
        if ((body == Declaraciones.iFragataReal) | (body == Declaraciones.iFragataCaos) |
            (body == Declaraciones.iBarcaPk) | (body == Declaraciones.iGaleraPk) | (body == Declaraciones.iGaleonPk) |
            (body == Declaraciones.iBarcaCiuda) | (body == Declaraciones.iGaleraCiuda) |
            (body == Declaraciones.iGaleonCiuda) | (body == Declaraciones.iFragataFantasmal)) BodyIsBoatRet = true;

        return BodyIsBoatRet;
    }

    public static void SetInvisible(short UserIndex, short userCharIndex, bool invisible)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        string sndNick;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            modSendData.SendData(modSendData.SendTarget.ToUsersAndRmsAndCounselorsAreaButGMs, UserIndex,
                Protocol.PrepareMessageSetInvisible(userCharIndex, invisible));

            sndNick = withBlock.name;

            if (invisible)
                sndNick = sndNick + " " + Declaraciones.TAG_USER_INVISIBLE;
            else if (withBlock.GuildIndex > 0)
                sndNick = sndNick + " <" + modGuilds.GuildName(withBlock.GuildIndex) + ">";

            modSendData.SendData(modSendData.SendTarget.ToGMsAreaButRmsOrCounselors, UserIndex,
                Protocol.PrepareMessageCharacterChangeNick(userCharIndex, sndNick));
        }
    }

    public static void SetConsulatMode(short UserIndex)
    {
        // ***************************************************
        // Author: Torres Patricio (Pato)
        // Last Modification: 05/06/10
        // 
        // ***************************************************

        string sndNick;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            sndNick = withBlock.name;

            if (withBlock.flags.EnConsulta)
                sndNick = sndNick + " " + Declaraciones.TAG_CONSULT_MODE;
            else if (withBlock.GuildIndex > 0)
                sndNick = sndNick + " <" + modGuilds.GuildName(withBlock.GuildIndex) + ">";

            modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                Protocol.PrepareMessageCharacterChangeNick(withBlock.character.CharIndex, sndNick));
        }
    }

    public static bool IsArena(short UserIndex)
    {
        bool IsArenaRet = default;
        // **************************************************************
        // Author: ZaMa
        // Last Modify Date: 10/11/2009
        // Returns true if the user is in an Arena
        // **************************************************************
        IsArenaRet = SistemaCombate.TriggerZonaPelea(UserIndex, UserIndex) == Declaraciones.eTrigger6.TRIGGER6_PERMITE;
        return IsArenaRet;
    }

    public static void PerdioNpc(short UserIndex)
    {
        // **************************************************************
        // Author: ZaMa
        // Last Modify Date: 18/01/2010 (ZaMa)
        // The user loses his owned Npc
        // 18/01/2010: ZaMa - Las mascotas dejan de atacar al Npc que se perdió.
        // **************************************************************

        int PetIndex;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.flags.OwnedNpc > 0)
            {
                Declaraciones.Npclist[withBlock.flags.OwnedNpc].Owner = 0;
                withBlock.flags.OwnedNpc = 0;

                // Dejan de atacar las mascotas
                if (withBlock.NroMascotas > 0)
                    for (PetIndex = 1; PetIndex <= Declaraciones.MAXMASCOTAS; PetIndex++)
                        if (withBlock.MascotasType[PetIndex] > 0)
                            NPCs.FollowAmo(Convert.ToInt16(PetIndex));
            }
        }
    }

    public static void ApropioNpc(short UserIndex, short NpcIndex)
    {
        // **************************************************************
        // Author: ZaMa
        // Last Modify Date: 18/01/2010 (zaMa)
        // The user owns a new Npc
        // 18/01/2010: ZaMa - El sistema no aplica a zonas seguras.
        // 19/04/2010: ZaMa - Ahora los admins no se pueden apropiar de npcs.
        // **************************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Los admins no se pueden apropiar de npcs
            if (Extra.EsGM(UserIndex))
                return;

            // No aplica a zonas seguras
            if (Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger ==
                Declaraciones.eTrigger.ZONASEGURA)
                return;

            // No aplica a algunos mapas que permiten el robo de npcs
            if (Declaraciones.mapInfo[withBlock.Pos.Map].RoboNpcsPermitido == 1)
                return;

            // Pierde el Npc anterior
            if (withBlock.flags.OwnedNpc > 0)
                Declaraciones.Npclist[withBlock.flags.OwnedNpc].Owner = 0;

            // Si tenia otro dueño, lo perdio aca
            Declaraciones.Npclist[NpcIndex].Owner = UserIndex;
            withBlock.flags.OwnedNpc = NpcIndex;
        }

        // Inicializo o actualizo el timer de pertenencia
        modNuevoTimer.IntervaloPerdioNpc(UserIndex, true);
    }

    public static string GetDireccion(short UserIndex, short OtherUserIndex)
    {
        string GetDireccionRet = default;
        // **************************************************************
        // Author: ZaMa
        // Last Modify Date: 17/11/2009
        // Devuelve la direccion hacia donde esta el usuario
        // **************************************************************
        short X;
        short Y;

        X = (short)(Declaraciones.UserList[UserIndex].Pos.X - Declaraciones.UserList[OtherUserIndex].Pos.X);
        Y = (short)(Declaraciones.UserList[UserIndex].Pos.Y - Declaraciones.UserList[OtherUserIndex].Pos.Y);

        if ((X == 0) & (Y > 0))
            GetDireccionRet = "Sur";
        else if ((X == 0) & (Y < 0))
            GetDireccionRet = "Norte";
        else if ((X > 0) & (Y == 0))
            GetDireccionRet = "Este";
        else if ((X < 0) & (Y == 0))
            GetDireccionRet = "Oeste";
        else if ((X > 0) & (Y < 0))
            GetDireccionRet = "NorEste";
        else if ((X < 0) & (Y < 0))
            GetDireccionRet = "NorOeste";
        else if ((X > 0) & (Y > 0))
            GetDireccionRet = "SurEste";
        else if ((X < 0) & (Y > 0)) GetDireccionRet = "SurOeste";

        return GetDireccionRet;
    }

    public static bool SameFaccion(short UserIndex, short OtherUserIndex)
    {
        bool SameFaccionRet = default;
        // **************************************************************
        // Author: ZaMa
        // Last Modify Date: 17/11/2009
        // Devuelve True si son de la misma faccion
        // **************************************************************
        SameFaccionRet = (Extra.esCaos(UserIndex) & Extra.esCaos(OtherUserIndex)) |
                         (Extra.esArmada(UserIndex) & Extra.esArmada(OtherUserIndex));
        return SameFaccionRet;
    }

    public static short FarthestPet(short UserIndex)
    {
        short FarthestPetRet = default;
        // **************************************************************
        // Author: ZaMa
        // Last Modify Date: 18/11/2009
        // Devuelve el indice de la mascota mas lejana.
        // **************************************************************
        try
        {
            short PetIndex;
            var Distancia = default(short);
            short OtraDistancia;

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                if (withBlock.NroMascotas == 0)
                    return FarthestPetRet;

                for (PetIndex = 1; PetIndex <= Declaraciones.MAXMASCOTAS; PetIndex++)
                    // Solo pos invocar criaturas que exitan!
                    if (withBlock.MascotasIndex[PetIndex] > 0)
                        // Solo aplica a mascota, nada de elementales..
                        if (Declaraciones.Npclist[withBlock.MascotasIndex[PetIndex]].Contadores.TiempoExistencia == 0)
                        {
                            if (FarthestPetRet == 0)
                            {
                                // Por si tiene 1 sola mascota
                                FarthestPetRet = PetIndex;
                                Distancia =
                                    (short)(Math.Abs((short)(withBlock.Pos.X -
                                                             Declaraciones.Npclist[withBlock.MascotasIndex[PetIndex]]
                                                                 .Pos.X)) + Math.Abs((short)(withBlock.Pos.Y -
                                        Declaraciones.Npclist[withBlock.MascotasIndex[PetIndex]].Pos.Y)));
                            }
                            else
                            {
                                // La distancia de la proxima mascota
                                OtraDistancia =
                                    (short)(Math.Abs((short)(withBlock.Pos.X -
                                                             Declaraciones.Npclist[withBlock.MascotasIndex[PetIndex]]
                                                                 .Pos.X)) + Math.Abs((short)(withBlock.Pos.Y -
                                        Declaraciones.Npclist[withBlock.MascotasIndex[PetIndex]].Pos.Y)));
                                // Esta mas lejos?
                                if (OtraDistancia > Distancia)
                                {
                                    Distancia = OtraDistancia;
                                    FarthestPetRet = PetIndex;
                                }
                            }
                        }
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in ContarMuerte: " + ex.Message);
            var argdesc = "Error en FarthestPet";
            General.LogError(ref argdesc);
        }

        return FarthestPetRet;
    }

    // '
    // Set the EluSkill value at the skill.
    // 
    // @param UserIndex  Specifies reference to user
    // @param Skill      Number of the skill to check
    // @param Allocation True If the motive of the modification is the allocation, False if the skill increase by training

    public static void CheckEluSkill(short UserIndex, byte Skill, bool Allocation)
    {
        // *************************************************
        // Author: Torres Patricio (Pato)
        // Last modified: 11/20/2009
        // 
        // *************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].Stats;
            if (withBlock.UserSkills[Skill] < Declaraciones.MAXSKILLPOINTS)
            {
                if (Allocation)
                    withBlock.ExpSkills[Skill] = 0;
                else
                    withBlock.ExpSkills[Skill] = withBlock.ExpSkills[Skill] - withBlock.EluSkills[Skill];

                withBlock.EluSkills[Skill] =
                    Convert.ToInt32(Declaraciones.ELU_SKILL_INICIAL * Math.Pow(1.05d, withBlock.UserSkills[Skill]));
            }
            else
            {
                withBlock.ExpSkills[Skill] = 0;
                withBlock.EluSkills[Skill] = 0;
            }
        }
    }

    public static bool HasEnoughItems(short UserIndex, short ObjIndex, int Amount)
    {
        bool HasEnoughItemsRet = default;
        // **************************************************************
        // Author: ZaMa
        // Last Modify Date: 25/11/2009
        // Cheks Wether the user has the required amount of items in the inventory or not
        // **************************************************************

        int Slot;
        var ItemInvAmount = default(int);

        var loopTo = (int)Declaraciones.UserList[UserIndex].CurrentInventorySlots;
        for (Slot = 1; Slot <= loopTo; Slot++)
            // Si es el item que busco
            if (Declaraciones.UserList[UserIndex].Invent.userObj[Slot].ObjIndex == ObjIndex)
                // Lo sumo a la cantidad total
                ItemInvAmount = ItemInvAmount + Declaraciones.UserList[UserIndex].Invent.userObj[Slot].Amount;

        HasEnoughItemsRet = Amount <= ItemInvAmount;
        return HasEnoughItemsRet;
    }

    public static int TotalOfferItems(short ObjIndex, short UserIndex)
    {
        int TotalOfferItemsRet = default;
        // **************************************************************
        // Author: ZaMa
        // Last Modify Date: 25/11/2009
        // Cheks the amount of items the user has in offerSlots.
        // **************************************************************
        byte Slot;

        for (Slot = 1; Slot <= (byte)mdlCOmercioConUsuario.MAX_OFFER_SLOTS; Slot++)
            // Si es el item que busco
            if (Declaraciones.UserList[UserIndex].ComUsu.Objeto[Slot] == ObjIndex)
                // Lo sumo a la cantidad total
                TotalOfferItemsRet = TotalOfferItemsRet + Declaraciones.UserList[UserIndex].ComUsu.cant[Slot];

        return TotalOfferItemsRet;
    }

    public static byte getMaxInventorySlots(short UserIndex)
    {
        byte getMaxInventorySlotsRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if (Declaraciones.UserList[UserIndex].Invent.MochilaEqpObjIndex > 0)
            getMaxInventorySlotsRet = Convert.ToByte(Declaraciones.MAX_NORMAL_INVENTORY_SLOTS +
                                                     Declaraciones
                                                         .objData[
                                                             Declaraciones.UserList[UserIndex].Invent
                                                                 .MochilaEqpObjIndex].MochilaType * 5);
        // 5=slots por fila, hacer constante
        else
            getMaxInventorySlotsRet = Declaraciones.MAX_NORMAL_INVENTORY_SLOTS;

        return getMaxInventorySlotsRet;
    }

    public static void goHome(short UserIndex)
    {
        short Distance;
        int tiempo;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.flags.Muerto == 1)
            {
                if (withBlock.flags.lastMap == 0)
                    Distance = Declaraciones.distanceToCities[withBlock.Pos.Map].distanceToCity[(int)withBlock.Hogar];
                else
                    Distance = (short)(Declaraciones.distanceToCities[withBlock.flags.lastMap]
                        .distanceToCity[(int)withBlock.Hogar] + Declaraciones.GOHOME_PENALTY);

                tiempo = (Distance + 1) * 30; // segundos

                withBlock.Counters.goHome = tiempo / 6; // Se va a chequear cada 6 segundos.

                withBlock.flags.Traveling = 1;

                Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.Home, Distance, tiempo,
                    StringArg1: Declaraciones.mapInfo[Declaraciones.Ciudades[(int)withBlock.Hogar].Map].name);
            }
            else
            {
                Protocol.WriteConsoleMsg(UserIndex, "Debes estar muerto para poder utilizar este comando.",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
            }
        }
    }

    public static bool ToogleToAtackable(short UserIndex, short OwnerIndex, bool StealingNpc = true)
    {
        bool ToogleToAtackableRet = default;
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 15/01/2010
        // Change to Atackable mode.
        // ***************************************************

        short AtacablePor;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];

            AtacablePor = withBlock.flags.AtacablePor;

            if (AtacablePor > 0)
            {
                // Intenta robar un Npc
                if (StealingNpc)
                {
                    // Puede atacar el mismo Npc que ya estaba robando, pero no una nuevo.
                    if (AtacablePor != OwnerIndex)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "No puedes atacar otra criatura con dueño hasta que haya terminado tu castigo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return ToogleToAtackableRet;
                    }
                }
                // Esta atacando a alguien en estado atacable => Se renueva el timer de atacable
                else
                {
                    // Renovar el timer
                    modNuevoTimer.IntervaloEstadoAtacable(UserIndex, true);
                    ToogleToAtackableRet = true;
                    return ToogleToAtackableRet;
                }
            }

            withBlock.flags.AtacablePor = OwnerIndex;

            // Actualizar clientes
            RefreshCharStatus(UserIndex);

            // Inicializar el timer
            modNuevoTimer.IntervaloEstadoAtacable(UserIndex, true);

            ToogleToAtackableRet = true;
        }

        return ToogleToAtackableRet;
    }

    public static void setHome(short UserIndex, Declaraciones.eCiudad newHome, short NpcIndex)
    {
        // ***************************************************
        // Author: Budi
        // Last Modification: 30/04/2010
        // 30/04/2010: ZaMa - Ahora el Npc avisa que se cambio de hogar.
        // ***************************************************
        if ((newHome < Declaraciones.eCiudad.cUllathorpe) | (newHome > Declaraciones.eCiudad.cArghal))
            return;
        Declaraciones.UserList[UserIndex].Hogar = newHome;

        Protocol.WriteChatOverHead(UserIndex,
            "¡¡¡Bienvenido a nuestra humilde comunidad, este es ahora tu nuevo hogar!!!",
            Declaraciones.Npclist[NpcIndex].character.CharIndex, ColorTranslator.ToOle(Color.White));
    }
}