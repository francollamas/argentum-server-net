using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Legacy;

internal static class Admin
{
    public static short MaxLines;
    public static tMotd[] MOTD;

    public static tAPuestas Apuestas;

    public static int tInicioServer;
    public static clsEstadisticasIPC EstadisticasWeb = new();

    // INTERVALOS
    public static short SanaIntervaloSinDescansar;
    public static short StaminaIntervaloSinDescansar;
    public static short SanaIntervaloDescansar;
    public static short StaminaIntervaloDescansar;
    public static short IntervaloSed;
    public static short IntervaloHambre;
    public static short IntervaloVeneno;
    public static short IntervaloParalizado;
    public static short IntervaloInvisible;
    public static short IntervaloFrio;
    public static short IntervaloWavFx;
    public static short IntervaloLanzaHechizo;
    public static short IntervaloNPCPuedeAtacar;
    public static short IntervaloNPCAI;
    public static short IntervaloInvocacion;
    public static short IntervaloOculto; // [Nacho]
    public static int IntervaloUserPuedeAtacar;
    public static int IntervaloGolpeUsar;
    public static int IntervaloMagiaGolpe;
    public static int IntervaloGolpeMagia;
    public static int IntervaloUserPuedeCastear;
    public static int IntervaloUserPuedeTrabajar;
    public static int IntervaloParaConexion;
    public static int IntervaloCerrarConexion; // [Gonzalo]
    public static int IntervaloUserPuedeUsar;
    public static int IntervaloFlechasCazadores;
    public static int IntervaloPuedeSerAtacado;
    public static int IntervaloAtacable;
    public static int IntervaloOwnedNpc;

    // BALANCE

    public static short PorcentajeRecuperoMana;

    public static int MinutosWs;
    public static short Puerto;

    public static byte BootDelBackUp;
    public static bool Lloviendo;
    public static bool DeNoche;

    private static bool _ActualizaEstadisticasWeb_Andando;
    private static int _ActualizaEstadisticasWeb_Contador;

    public static bool VersionOK(string Ver)
    {
        bool VersionOKRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        VersionOKRet = (Ver ?? "") == (Declaraciones.ULTIMAVERSION ?? "");
        return VersionOKRet;
    }

    public static void ReSpawnOrigPosNpcs()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            short i;
            // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura MiNPC, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
            Declaraciones.npc MiNPC;

            var loopTo = Declaraciones.LastNPC;
            for (i = 1; i <= loopTo; i++)
                // OJO
                if (Declaraciones.Npclist[i].flags.NPCActive)
                    if (Extra.InMapBounds(Declaraciones.Npclist[i].Orig.Map, Declaraciones.Npclist[i].Orig.X,
                            Declaraciones.Npclist[i].Orig.Y) &
                        (Declaraciones.Npclist[i].Numero == Declaraciones.Guardias))
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto MiNPC. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        MiNPC = Declaraciones.Npclist[i];
                        NPCs.QuitarNPC(i);
                        NPCs.ReSpawnNpc(ref MiNPC);
                    }
            // tildada por sugerencia de yind
            // If Npclist(i).Contadores.TiempoExistencia > 0 Then
            // Call MuereNpc(i, 0)
            // End If
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in VersionOK: " + ex.Message);
        }
    }

    public static void WorldSave()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            short loopX;
            int Porc;

            modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                Protocol.PrepareMessageConsoleMsg("Servidor> Iniciando WorldSave",
                    Protocol.FontTypeNames.FONTTYPE_SERVER));

            ReSpawnOrigPosNpcs(); // respawn de los guardias en las pos originales

            short j, k = default;

            var loopTo = Declaraciones.NumMaps;
            for (j = 1; j <= loopTo; j++)
                if (Declaraciones.mapInfo[j].BackUp == 1)
                    k = Convert.ToInt16(k + 1);

            var loopTo1 = Declaraciones.NumMaps;
            for (loopX = 1; loopX <= loopTo1; loopX++)
                // DoEvents
                if (Declaraciones.mapInfo[loopX].BackUp == 1)
                    ES.GrabarMapa(loopX, AppDomain.CurrentDomain.BaseDirectory + "WorldBackUp/Mapa" + loopX);

            if (File.Exists(Declaraciones.DatPath + "/bkNpc.dat"))
                FileSystem.Kill(Declaraciones.DatPath + "bkNpc.dat");
            // If System.IO.File.Exists(DatPath & "/bkNPCs-HOSTILES.dat") Then Kill (DatPath & "bkNPCs-HOSTILES.dat")

            var loopTo2 = Declaraciones.LastNPC;
            for (loopX = 1; loopX <= loopTo2; loopX++)
                if (Declaraciones.Npclist[loopX].flags.BackUp == 1)
                    ES.BackUPnPc(ref loopX);

            modForum.SaveForums();

            modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                Protocol.PrepareMessageConsoleMsg("Servidor> WorldSave ha concluído.",
                    Protocol.FontTypeNames.FONTTYPE_SERVER));
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in WorldSave: " + ex.Message);
        }
    }

    public static void PurgarPenas()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int i;

        var loopTo = (int)Declaraciones.LastUser;
        for (i = 1; i <= loopTo; i++)
            if (Declaraciones.UserList[i].flags.UserLogged)
                if (Declaraciones.UserList[i].Counters.Pena > 0)
                {
                    Declaraciones.UserList[i].Counters.Pena = Declaraciones.UserList[i].Counters.Pena - 1;

                    if (Declaraciones.UserList[i].Counters.Pena < 1)
                    {
                        Declaraciones.UserList[i].Counters.Pena = 0;
                        UsUaRiOs.WarpUserChar(Convert.ToInt16(i), Declaraciones.Libertad.Map, Declaraciones.Libertad.X,
                            Declaraciones.Libertad.Y, true);
                        Protocol.WriteConsoleMsg(Convert.ToInt16(i), "¡Has sido liberado!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);

                        Protocol.FlushBuffer(Convert.ToInt16(i));
                    }
                }
    }


    public static void Encarcelar(short UserIndex, int Minutos, string GmName = Constants.vbNullString)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        Declaraciones.UserList[UserIndex].Counters.Pena = Minutos;


        UsUaRiOs.WarpUserChar(UserIndex, Declaraciones.Prision.Map, Declaraciones.Prision.X, Declaraciones.Prision.Y,
            true);

        if (Migration.migr_LenB(GmName) == 0)
            Protocol.WriteConsoleMsg(UserIndex,
                "Has sido encarcelado, deberás permanecer en la cárcel " + Minutos + " minutos.",
                Protocol.FontTypeNames.FONTTYPE_INFO);
        else
            Protocol.WriteConsoleMsg(UserIndex,
                GmName + " te ha encarcelado, deberás permanecer en la cárcel " + Minutos + " minutos.",
                Protocol.FontTypeNames.FONTTYPE_INFO);
        if (Declaraciones.UserList[UserIndex].flags.Traveling == 1)
        {
            Declaraciones.UserList[UserIndex].flags.Traveling = 0;
            Declaraciones.UserList[UserIndex].Counters.goHome = 0;
            Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.CancelHome);
        }
    }


    public static void BorrarUsuario(string UserName)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            if (File.Exists(Declaraciones.CharPath + UserName.ToUpper() + ".chr"))
                FileSystem.Kill(Declaraciones.CharPath + UserName.ToUpper() + ".chr");
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in PurgarPenas: " + ex.Message);
        }
    }

    public static bool BANCheck(string name)
    {
        bool BANCheckRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        var argEmptySpaces = 1024;
        BANCheckRet = Migration.ParseVal(ES.GetVar(AppDomain.CurrentDomain.BaseDirectory + "charfile/" + name + ".chr",
            "FLAGS", "Ban", ref argEmptySpaces)) == 1d;
        return BANCheckRet;
    }

    public static bool PersonajeExiste(string name)
    {
        bool PersonajeExisteRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        PersonajeExisteRet = File.Exists(Declaraciones.CharPath + name.ToUpper() + ".chr");
        return PersonajeExisteRet;
    }

    public static bool UnBan(string name)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // Unban the character
        ES.WriteVar(AppDomain.CurrentDomain.BaseDirectory + "charfile/" + name + ".chr", "FLAGS", "Ban", "0");

        // Remove it from the banned people database
        ES.WriteVar(AppDomain.CurrentDomain.BaseDirectory + "logs/" + "BanDetail.dat", name, "BannedBy", "NOBODY");
        ES.WriteVar(AppDomain.CurrentDomain.BaseDirectory + "logs/" + "BanDetail.dat", name, "Reason", "NO REASON");
        return default;
    }

    public static bool MD5ok(string md5formateado)
    {
        bool MD5okRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short i;

        if (Declaraciones.MD5ClientesActivado == 1)
        {
            var loopTo = Convert.ToInt16(Declaraciones.MD5s.Length - 1);
            for (i = 0; i <= loopTo; i++)
                if ((md5formateado ?? "") == (Declaraciones.MD5s[i] ?? ""))
                {
                    MD5okRet = true;
                    return MD5okRet;
                }

            MD5okRet = false;
        }
        else
        {
            MD5okRet = true;
        }

        return MD5okRet;
    }

    public static void MD5sCarga()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short LoopC;

        var argEmptySpaces = 1024;
        Declaraciones.MD5ClientesActivado =
            Convert.ToByte(Migration.ParseVal(ES.GetVar(Declaraciones.IniPath + "Server.ini", "MD5Hush", "Activado",
                ref argEmptySpaces)));

        if (Declaraciones.MD5ClientesActivado == 1)
        {
            var argEmptySpaces1 = 1024;
            Declaraciones.MD5s = new string[Convert.ToInt32(Migration.ParseVal(
                ES.GetVar(Declaraciones.IniPath + "Server.ini", "MD5Hush", "MD5Aceptados", ref argEmptySpaces1))) + 1];
            var loopTo = Convert.ToInt16(Declaraciones.MD5s.Length - 1);
            for (LoopC = 0; LoopC <= loopTo; LoopC++)
            {
                var argEmptySpaces2 = 1024;
                Declaraciones.MD5s[LoopC] = ES.GetVar(Declaraciones.IniPath + "Server.ini", "MD5Hush",
                    "MD5Aceptado" + (LoopC + 1), ref argEmptySpaces2);
                Declaraciones.MD5s[LoopC] =
                    modHexaStrings.txtOffset(modHexaStrings.hexMd52Asc(Declaraciones.MD5s[LoopC]), 55);
            }
        }
    }

    public static void BanIpAgrega(string ip)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        Declaraciones.BanIps.Add(ip);

        BanIpGuardar();
    }

    public static int BanIpBuscar(string ip)
    {
        int BanIpBuscarRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        bool Dale;
        int LoopC;

        Dale = true;
        LoopC = 1;
        while ((LoopC <= Declaraciones.BanIps.Count) & Dale)
        {
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto BanIps.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Dale = (Declaraciones.BanIps[LoopC] ?? "") != (ip ?? "");
            LoopC = LoopC + 1;
        }

        if (Dale)
            BanIpBuscarRet = 0;
        else
            BanIpBuscarRet = LoopC - 1;

        return BanIpBuscarRet;
    }

    public static bool BanIpQuita(string ip)
    {
        bool BanIpQuitaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            int N;

            N = BanIpBuscar(ip);
            if (N > 0)
            {
                Declaraciones.BanIps.RemoveAt(N - 1);
                BanIpGuardar();
                BanIpQuitaRet = true;
            }
            else
            {
                BanIpQuitaRet = false;
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in BANCheck: " + ex.Message);
        }

        return BanIpQuitaRet;
    }

    public static void BanIpGuardar()
    {
        var ArchivoBanIp = AppDomain.CurrentDomain.BaseDirectory + "Dat/BanIps.dat";
        var lines = new List<string>();

        for (int LoopC = 1, loopTo = Declaraciones.BanIps.Count; LoopC <= loopTo; LoopC++)
            lines.Add(Declaraciones.BanIps[LoopC]);

        File.WriteAllLines(ArchivoBanIp, lines);
    }

    public static void BanIpCargar()
    {
        var ArchivoBanIp = AppDomain.CurrentDomain.BaseDirectory + "Dat/BanIps.dat";

        while (Declaraciones.BanIps.Count > 0)
            Declaraciones.BanIps.RemoveAt(0);

        foreach (var line in File.ReadAllLines(ArchivoBanIp))
            Declaraciones.BanIps.Add(line);
    }

    // ***************************************************
    // Author: Unknown
    // Last Modification: -
    // 
    // ***************************************************

    public static void ActualizaEstadisticasWeb()
    {
        bool Tmp;

        _ActualizaEstadisticasWeb_Contador = _ActualizaEstadisticasWeb_Contador + 1;

        if (_ActualizaEstadisticasWeb_Contador >= 10)
        {
            _ActualizaEstadisticasWeb_Contador = 0;
            Tmp = EstadisticasWeb.EstadisticasAndando();

            if (!_ActualizaEstadisticasWeb_Andando & Tmp) General.InicializaEstadisticas();

            _ActualizaEstadisticasWeb_Andando = Tmp;
        }
    }

    public static Declaraciones.PlayerType UserDarPrivilegioLevel(string name)
    {
        Declaraciones.PlayerType UserDarPrivilegioLevelRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: 03/02/07
        // Last Modified By: Juan Martín Sotuyo Dodero (Maraxus)
        // ***************************************************

        if (ES.EsAdmin(name))
            UserDarPrivilegioLevelRet = Declaraciones.PlayerType.Admin;
        else if (ES.EsDios(name))
            UserDarPrivilegioLevelRet = Declaraciones.PlayerType.Dios;
        else if (ES.EsSemiDios(name))
            UserDarPrivilegioLevelRet = Declaraciones.PlayerType.SemiDios;
        else if (ES.EsConsejero(name))
            UserDarPrivilegioLevelRet = Declaraciones.PlayerType.Consejero;
        else
            UserDarPrivilegioLevelRet = Declaraciones.PlayerType.User;

        return UserDarPrivilegioLevelRet;
    }

    public static void BanCharacter(short bannerUserIndex, string UserName, string reason)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 03/02/07
        // 
        // ***************************************************

        short tUser;
        byte userPriv;
        byte cantPenas;
        short rank;

        if (Migration.migr_InStrB(UserName, "+") != 0) UserName = UserName.Replace("+", " ");

        tUser = Extra.NameIndex(UserName);

        rank = Convert.ToInt16((int)(Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                     Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.Consejero));

        {
            ref var withBlock = ref Declaraciones.UserList[bannerUserIndex];
            if (tUser <= 0)
            {
                Protocol.WriteConsoleMsg(bannerUserIndex, "El usuario no está online.",
                    Protocol.FontTypeNames.FONTTYPE_TALK);

                if (File.Exists(Declaraciones.CharPath + UserName + ".chr"))
                {
                    userPriv = Convert.ToByte((int)UserDarPrivilegioLevel(UserName));

                    if ((userPriv & rank) > ((int)withBlock.flags.Privilegios & rank))
                    {
                        Protocol.WriteConsoleMsg(bannerUserIndex, "No puedes banear a al alguien de mayor jerarquía.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        var argEmptySpaces1 = 1024;
                        if (ES.GetVar(Declaraciones.CharPath + UserName + ".chr", "FLAGS", "Ban",
                                ref argEmptySpaces1) != "0")
                        {
                            Protocol.WriteConsoleMsg(bannerUserIndex, "El personaje ya se encuentra baneado.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                        }
                        else
                        {
                            ES.LogBanFromName(UserName, bannerUserIndex, reason);
                            modSendData.SendData(modSendData.SendTarget.ToAdmins, 0,
                                Protocol.PrepareMessageConsoleMsg(
                                    "Servidor> " + withBlock.name + " ha baneado a " + UserName + ".",
                                    Protocol.FontTypeNames.FONTTYPE_SERVER));

                            // ponemos el flag de ban a 1
                            ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "FLAGS", "Ban", "1");
                            // ponemos la pena
                            var argEmptySpaces = 1024;
                            cantPenas = Convert.ToByte(Migration.ParseVal(ES.GetVar(
                                Declaraciones.CharPath + UserName + ".chr", "PENAS", "Cant", ref argEmptySpaces)));
                            ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "PENAS", "Cant",
                                (cantPenas + 1).ToString());
                            ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "PENAS", "P" + (cantPenas + 1),
                                withBlock.name.ToLower() + ": BAN POR " + reason.ToLower() + " " +
                                Conversions.ToString(DateTime.Today) + " " +
                                Conversions.ToString(DateAndTime.TimeOfDay));

                            if ((userPriv & rank) == ((int)withBlock.flags.Privilegios & rank))
                            {
                                withBlock.flags.Ban = 1;
                                modSendData.SendData(modSendData.SendTarget.ToAdmins, 0,
                                    Protocol.PrepareMessageConsoleMsg(
                                        withBlock.name + " banned by the server por bannear un Administrador.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT));
                                TCP.CloseSocket(bannerUserIndex);
                            }

                            var argtexto = "BAN a " + UserName;
                            General.LogGM(ref withBlock.name, ref argtexto);
                        }
                    }
                }
                else
                {
                    Protocol.WriteConsoleMsg(bannerUserIndex, "El pj " + UserName + " no existe.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                }
            }
            else
            {
                if (((int)Declaraciones.UserList[tUser].flags.Privilegios & rank) >
                    ((int)withBlock.flags.Privilegios & rank))
                    Protocol.WriteConsoleMsg(bannerUserIndex, "No puedes banear a al alguien de mayor jerarquía.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);

                ES.LogBan(tUser, bannerUserIndex, reason);
                modSendData.SendData(modSendData.SendTarget.ToAdmins, 0,
                    Protocol.PrepareMessageConsoleMsg(
                        "Servidor> " + withBlock.name + " ha baneado a " + Declaraciones.UserList[tUser].name + ".",
                        Protocol.FontTypeNames.FONTTYPE_SERVER));

                // Ponemos el flag de ban a 1
                Declaraciones.UserList[tUser].flags.Ban = 1;

                if (((int)Declaraciones.UserList[tUser].flags.Privilegios & rank) ==
                    ((int)withBlock.flags.Privilegios & rank))
                {
                    withBlock.flags.Ban = 1;
                    modSendData.SendData(modSendData.SendTarget.ToAdmins, 0,
                        Protocol.PrepareMessageConsoleMsg(
                            withBlock.name + " banned by the server por bannear un Administrador.",
                            Protocol.FontTypeNames.FONTTYPE_FIGHT));
                    TCP.CloseSocket(bannerUserIndex);
                }

                var argtexto1 = "BAN a " + UserName;
                General.LogGM(ref withBlock.name, ref argtexto1);

                // ponemos el flag de ban a 1
                ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "FLAGS", "Ban", "1");
                // ponemos la pena
                var argEmptySpaces2 = 1024;
                cantPenas = Convert.ToByte(Migration.ParseVal(ES.GetVar(Declaraciones.CharPath + UserName + ".chr",
                    "PENAS", "Cant", ref argEmptySpaces2)));
                ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "PENAS", "Cant", (cantPenas + 1).ToString());
                ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "PENAS", "P" + (cantPenas + 1),
                    withBlock.name.ToLower() + ": BAN POR " + reason.ToLower() + " " +
                    Conversions.ToString(DateTime.Today) + " " + Conversions.ToString(DateAndTime.TimeOfDay));

                TCP.CloseSocket(tUser);
            }
        }
    }

    public struct tMotd
    {
        public string texto;
        public string Formato;
    }

    public struct tAPuestas
    {
        public int Ganancias;
        public int Perdidas;
        public int Jugadas;
    }
}