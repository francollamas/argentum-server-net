using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Legacy;

internal class clsClan
{
    private const short NEWSLENGTH = 1024;
    private const short DESCLENGTH = 256;
    private const short CODEXLENGTH = 256;
    private string GUILDINFOFILE;
    private string GUILDPATH; // aca pq me es mas comodo setearlo y pq en ningun disenio
    private string MEMBERSFILE; // decente la capa de arriba se entera donde estan
    private modGuilds.ALINEACION_GUILD p_Alineacion;

    private List<string> p_GMsOnline;
    // '
    // clase clan
    // 
    // Es el "ADO" de los clanes. La interfaz entre el disco y
    // el juego. Los datos no se guardan en memoria
    // para evitar problemas de sincronizacion, y considerando
    // que la performance de estas rutinas NO es critica.
    // by el oso :p

    private string p_GuildName;
    private short p_GuildNumber; // Numero de guild en el mundo
    private short p_IteradorOnlineGMs;
    private short p_IteradorOnlineMembers;
    private short p_IteradorPropuesta;
    private short p_IteradorRelaciones;
    private List<string> p_OnlineMembers; // Array de UserIndexes!
    private List<string> p_PropuestasDeAlianza;
    private List<string> p_PropuestasDePaz;
    private modGuilds.RELACIONES_GUILD[] p_Relaciones; // array de relaciones con los otros clanes
    private string PROPUESTASFILE;
    private string RELACIONESFILE;
    private string SOLICITUDESFILE; // los datos fisicamente
    private string VOTACIONESFILE;

    public clsClan()
    {
        Class_Initialize();
    }

    public string GuildName
    {
        get
        {
            string GuildNameRet = default;
            GuildNameRet = p_GuildName;
            return GuildNameRet;
        }
    }


    // 
    // ALINEACION Y ANTIFACCION
    // 

    public modGuilds.ALINEACION_GUILD Alineacion
    {
        get
        {
            modGuilds.ALINEACION_GUILD AlineacionRet = default;
            AlineacionRet = p_Alineacion;
            return AlineacionRet;
        }
    }


    public short PuntosAntifaccion
    {
        get
        {
            short PuntosAntifaccionRet = default;
            var argEmptySpaces = 1024;
            PuntosAntifaccionRet = Convert.ToInt16(Migration.ParseVal(ES.GetVar(GUILDINFOFILE, "GUILD" + p_GuildNumber,
                "Antifaccion", ref argEmptySpaces)));
            return PuntosAntifaccionRet;
        }
        set => ES.WriteVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "Antifaccion", value.ToString());
    }


    // 
    // MEMBRESIAS
    // 

    public string Fundador
    {
        get
        {
            string FundadorRet = default;
            var argEmptySpaces = 1024;
            FundadorRet = ES.GetVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "Founder", ref argEmptySpaces);
            return FundadorRet;
        }
    }

    // Public Property Get JugadoresOnline() As String
    // Dim i As Integer
    // 'leve violacion de capas x aqui, je
    // For i = 1 To p_OnlineMembers.Count
    // JugadoresOnline = UserList(p_OnlineMembers.Item(i)).Name & "," & JugadoresOnline
    // Next i
    // End Property

    public short CantidadDeMiembros
    {
        get
        {
            short CantidadDeMiembrosRet = default;
            string OldQ;
            var argEmptySpaces = 1024;
            OldQ = ES.GetVar(MEMBERSFILE, "INIT", "NroMembers", ref argEmptySpaces);
            CantidadDeMiembrosRet = Convert.ToInt16(Information.IsNumeric(OldQ) ? Convert.ToInt16(OldQ) : 0);
            return CantidadDeMiembrosRet;
        }
    }

    public short CantidadEnemys
    {
        get
        {
            short CantidadEnemysRet = default;
            short i;
            CantidadEnemysRet = 0;
            var loopTo = modGuilds.CANTIDADDECLANES;
            for (i = 1; i <= loopTo; i++)
                CantidadEnemysRet = (short)(CantidadEnemysRet +
                                            Convert.ToInt16(
                                                p_Relaciones[i] == modGuilds.RELACIONES_GUILD.GUERRA ? 1 : 0));
            return CantidadEnemysRet;
        }
    }

    public short CantidadAllies
    {
        get
        {
            short CantidadAlliesRet = default;
            short i;
            CantidadAlliesRet = 0;
            var loopTo = modGuilds.CANTIDADDECLANES;
            for (i = 1; i <= loopTo; i++)
                CantidadAlliesRet = (short)(CantidadAlliesRet +
                                            Convert.ToInt16(p_Relaciones[i] == modGuilds.RELACIONES_GUILD.ALIADOS
                                                ? 1
                                                : 0));
            return CantidadAlliesRet;
        }
    }

    // /VOTACIONES


    // 
    // RELACIONES
    // 

    public short get_CantidadPropuestas(modGuilds.RELACIONES_GUILD Tipo)
    {
        short CantidadPropuestasRet = default;
        switch (Tipo)
        {
            case modGuilds.RELACIONES_GUILD.ALIADOS:
            {
                CantidadPropuestasRet = Convert.ToInt16(p_PropuestasDeAlianza.Count);
                break;
            }
            case modGuilds.RELACIONES_GUILD.GUERRA:
            {
                CantidadPropuestasRet = 0;
                break;
            }
            case modGuilds.RELACIONES_GUILD.PAZ:
            {
                CantidadPropuestasRet = Convert.ToInt16(p_PropuestasDePaz.Count);
                break;
            }
        }

        return CantidadPropuestasRet;
    }

    public bool CambiarAlineacion(modGuilds.ALINEACION_GUILD NuevaAlineacion)
    {
        bool CambiarAlineacionRet = default;
        p_Alineacion = NuevaAlineacion;
        ES.WriteVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "Alineacion", modGuilds.Alineacion2String(p_Alineacion));

        if (p_Alineacion == modGuilds.ALINEACION_GUILD.ALINEACION_NEUTRO)
            CambiarAlineacionRet = true;
        return CambiarAlineacionRet;
    }

    // 
    // INICIALIZADORES
    // 

    private void Class_Initialize()
    {
        GUILDPATH = AppDomain.CurrentDomain.BaseDirectory + "GUILDS/";
        GUILDINFOFILE = GUILDPATH + "guildsinfo.inf";
    }

    private void Class_Terminate()
    {
        // UPGRADE_NOTE: El objeto p_OnlineMembers no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        p_OnlineMembers = null;
        // UPGRADE_NOTE: El objeto p_GMsOnline no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        p_GMsOnline = null;
        // UPGRADE_NOTE: El objeto p_PropuestasDePaz no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        p_PropuestasDePaz = null;
        // UPGRADE_NOTE: El objeto p_PropuestasDeAlianza no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        p_PropuestasDeAlianza = null;
    }

    ~clsClan()
    {
        Class_Terminate();
    }


    public void Inicializar(string GuildName, short GuildNumber, modGuilds.ALINEACION_GUILD Alineacion)
    {
        short i;

        p_GuildName = GuildName;
        p_GuildNumber = GuildNumber;
        p_Alineacion = Alineacion;
        p_OnlineMembers = new List<string>();
        p_GMsOnline = new List<string>();
        p_PropuestasDePaz = new List<string>();
        p_PropuestasDeAlianza = new List<string>();
        // ALLIESFILE = GUILDPATH & p_GuildName & "-Allied.all"
        // ENEMIESFILE = GUILDPATH & p_GuildName & "-enemys.ene"
        RELACIONESFILE = GUILDPATH + p_GuildName + "-relaciones.rel";
        MEMBERSFILE = GUILDPATH + p_GuildName + "-members.mem";
        PROPUESTASFILE = GUILDPATH + p_GuildName + "-propositions.pro";
        SOLICITUDESFILE = GUILDPATH + p_GuildName + "-solicitudes.sol";
        VOTACIONESFILE = GUILDPATH + p_GuildName + "-votaciones.vot";
        p_IteradorOnlineMembers = 0;
        p_IteradorPropuesta = 0;
        p_IteradorOnlineGMs = 0;
        p_IteradorRelaciones = 0;
        // UPGRADE_WARNING: El límite inferior de la matriz p_Relaciones ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        Array.Resize(ref p_Relaciones, modGuilds.CANTIDADDECLANES + 1);
        var loopTo = modGuilds.CANTIDADDECLANES;
        for (i = 1; i <= loopTo; i++)
        {
            var argEmptySpaces = 1024;
            p_Relaciones[i] =
                modGuilds.String2Relacion(ES.GetVar(RELACIONESFILE, "RELACIONES", i.ToString(), ref argEmptySpaces));
        }

        var loopTo1 = modGuilds.CANTIDADDECLANES;
        for (i = 1; i <= loopTo1; i++)
        {
            var argEmptySpaces2 = 1024;
            if (Strings.Trim(ES.GetVar(PROPUESTASFILE, i.ToString(), "Pendiente", ref argEmptySpaces2)) == "1")
            {
                var argEmptySpaces1 = 1024;
                switch (modGuilds.String2Relacion(Strings.Trim(ES.GetVar(PROPUESTASFILE, i.ToString(), "Tipo",
                            ref argEmptySpaces1))))
                {
                    case modGuilds.RELACIONES_GUILD.ALIADOS:
                    {
                        p_PropuestasDeAlianza.Add(i.ToString());
                        break;
                    }
                    case modGuilds.RELACIONES_GUILD.PAZ:
                    {
                        p_PropuestasDePaz.Add(i.ToString());
                        break;
                    }
                }
            }
        }
    }

    // '
    // esta TIENE QUE LLAMARSE LUEGO DE INICIALIZAR()
    // 
    // @param Fundador Nombre del fundador del clan
    // 
    public void InicializarNuevoClan(ref string Fundador)
    {
        string OldQ; // string pq al comienzo quizas no hay archivo guildinfo.ini y oldq es ""
        short NewQ;
        // para que genere los archivos
        ES.WriteVar(MEMBERSFILE, "INIT", "NroMembers", "0");
        ES.WriteVar(SOLICITUDESFILE, "INIT", "CantSolicitudes", "0");


        var argEmptySpaces = 1024;
        OldQ = ES.GetVar(GUILDINFOFILE, "INIT", "nroguilds", ref argEmptySpaces);
        if (Information.IsNumeric(OldQ))
            NewQ = Convert.ToInt16(Convert.ToInt16(Strings.Trim(OldQ)) + 1);
        else
            NewQ = 1;

        ES.WriteVar(GUILDINFOFILE, "INIT", "NroGuilds", NewQ.ToString());

        ES.WriteVar(GUILDINFOFILE, "GUILD" + NewQ, "Founder", Fundador);
        ES.WriteVar(GUILDINFOFILE, "GUILD" + NewQ, "GuildName", p_GuildName);
        ES.WriteVar(GUILDINFOFILE, "GUILD" + NewQ, "Date", DateTime.Today.ToString());
        ES.WriteVar(GUILDINFOFILE, "GUILD" + NewQ, "Antifaccion", "0");
        ES.WriteVar(GUILDINFOFILE, "GUILD" + NewQ, "Alineacion", modGuilds.Alineacion2String(p_Alineacion));
    }

    public void ProcesarFundacionDeOtroClan()
    {
        // UPGRADE_WARNING: El límite inferior de la matriz p_Relaciones ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        Array.Resize(ref p_Relaciones, modGuilds.CANTIDADDECLANES + 1);
        p_Relaciones[modGuilds.CANTIDADDECLANES] = modGuilds.RELACIONES_GUILD.PAZ;
    }

    public void SetLeader(ref string leader)
    {
        ES.WriteVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "Leader", leader);
    }

    public string GetLeader()
    {
        string GetLeaderRet = default;
        var argEmptySpaces = 1024;
        GetLeaderRet = ES.GetVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "Leader", ref argEmptySpaces);
        return GetLeaderRet;
    }

    public string[] GetMemberList()
    {
        short OldQ;
        string[] list;
        int i;

        OldQ = CantidadDeMiembros;

        list = new string[OldQ];

        var loopTo = (int)OldQ;
        for (i = 1; i <= loopTo; i++)
        {
            var argEmptySpaces = 1024;
            list[i - 1] = ES.GetVar(MEMBERSFILE, "Members", "Member" + i, ref argEmptySpaces).ToUpper();
        }

        return list;
    }

    public void ConectarMiembro(short UserIndex)
    {
        p_OnlineMembers.Add(UserIndex.ToString());

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            modSendData.SendData(modSendData.SendTarget.ToDiosesYclan, withBlock.GuildIndex,
                Protocol.PrepareMessageGuildChat(withBlock.name + " se ha conectado."));
        }
    }

    public void DesConectarMiembro(short UserIndex)
    {
        short i;
        var loopTo = Convert.ToInt16(p_OnlineMembers.Count - 1);
        for (i = 0; i <= loopTo; i++)
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_OnlineMembers.Item(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            if ((p_OnlineMembers[i] ?? "") == (UserIndex.ToString() ?? ""))
            {
                p_OnlineMembers.Remove(p_OnlineMembers[i]);

                {
                    ref var withBlock = ref Declaraciones.UserList[UserIndex];
                    modSendData.SendData(modSendData.SendTarget.ToDiosesYclan, withBlock.GuildIndex,
                        Protocol.PrepareMessageGuildChat(withBlock.name + " se ha desconectado."));
                }

                return;
            }
    }

    public void AceptarNuevoMiembro(ref string Nombre)
    {
        short OldQ;
        string OldQs;
        string ruta;

        ruta = Declaraciones.CharPath + Nombre + ".chr";
        if (File.Exists(ruta))
        {
            ES.WriteVar(ruta, "GUILD", "GUILDINDEX", p_GuildNumber.ToString());
            ES.WriteVar(ruta, "GUILD", "AspiranteA", "0");
            // CantPs = GetVar(CharPath & Nombre & ".chr", "GUILD", "ClanesParticipo")
            // If IsNumeric(CantPs) Then
            // CantP = CInt(CantPs)
            // Else
            // CantP = 0
            // End If
            // Call WriteVar(CharPath & Nombre & ".chr", "GUILD", "ClanesParticipo", CantP + 1)
            var argEmptySpaces = 1024;
            OldQs = ES.GetVar(MEMBERSFILE, "INIT", "NroMembers", ref argEmptySpaces);
            if (Information.IsNumeric(OldQs))
                OldQ = Convert.ToInt16(OldQs);
            else
                OldQ = 0;
            ES.WriteVar(MEMBERSFILE, "INIT", "NroMembers", (OldQ + 1).ToString());
            ES.WriteVar(MEMBERSFILE, "Members", "Member" + (OldQ + 1), Nombre);
        }
    }

    public void ExpulsarMiembro(ref string Nombre)
    {
        short OldQ;
        string Temps;
        short i;
        bool EsMiembro;
        string MiembroDe;

        if (File.Exists(Declaraciones.CharPath + Nombre + ".chr"))
        {
            var argEmptySpaces = 1024;
            OldQ = Convert.ToInt16(ES.GetVar(MEMBERSFILE, "INIT", "NroMembers", ref argEmptySpaces));
            i = 1;
            Nombre = Nombre.ToUpper();
            var argEmptySpaces1 = 1024;
            while ((i <= OldQ) &
                   ((ES.GetVar(MEMBERSFILE, "Members", "Member" + i, ref argEmptySpaces1).Trim().ToUpper() ?? "") !=
                    (Nombre ?? "")))
                i = Convert.ToInt16(i + 1);
            EsMiembro = i <= OldQ;

            if (EsMiembro)
            {
                ES.WriteVar(Declaraciones.CharPath + Nombre + ".chr", "GUILD", "GuildIndex", Constants.vbNullString);
                while (i < OldQ)
                {
                    var argEmptySpaces2 = 1024;
                    Temps = ES.GetVar(MEMBERSFILE, "Members", "Member" + Convert.ToInt16(i + 1), ref argEmptySpaces2);
                    ES.WriteVar(MEMBERSFILE, "Members", "Member" + i, Temps);
                    i = Convert.ToInt16(i + 1);
                }

                ES.WriteVar(MEMBERSFILE, "Members", "Member" + OldQ, Constants.vbNullString);
                // seteo la cantidad de miembros nueva
                ES.WriteVar(MEMBERSFILE, "INIT", "NroMembers", (OldQ - 1).ToString());
                // lo echo a el
                var argEmptySpaces3 = 1024;
                MiembroDe = ES.GetVar(Declaraciones.CharPath + Nombre + ".chr", "GUILD", "Miembro",
                    ref argEmptySpaces3);
                if (!(MiembroDe.IndexOf(p_GuildName, StringComparison.OrdinalIgnoreCase) + 1 > 0))
                {
                    if (Migration.migr_LenB(MiembroDe) != 0) MiembroDe = MiembroDe + ",";
                    MiembroDe = MiembroDe + p_GuildName;
                    ES.WriteVar(Declaraciones.CharPath + Nombre + ".chr", "GUILD", "Miembro", MiembroDe);
                }
            }
        }
    }

    // 
    // ASPIRANTES
    // 

    public string[] GetAspirantes()
    {
        short OldQ;
        string[] list;
        int i;

        OldQ = CantidadAspirantes();

        if (OldQ > 1)
            list = new string[OldQ];
        else
            list = new string[1];

        var loopTo = (int)OldQ;
        for (i = 1; i <= loopTo; i++)
        {
            var argEmptySpaces = 1024;
            list[i - 1] = ES.GetVar(SOLICITUDESFILE, "SOLICITUD" + i, "Nombre", ref argEmptySpaces);
        }

        return list;
    }

    public short CantidadAspirantes()
    {
        short CantidadAspirantesRet = default;
        string Temps;

        CantidadAspirantesRet = 0;
        var argEmptySpaces = 1024;
        Temps = ES.GetVar(SOLICITUDESFILE, "INIT", "CantSolicitudes", ref argEmptySpaces);
        if (!Information.IsNumeric(Temps)) return CantidadAspirantesRet;
        CantidadAspirantesRet = Convert.ToInt16(Temps);
        return CantidadAspirantesRet;
    }

    public string DetallesSolicitudAspirante(short NroAspirante)
    {
        string DetallesSolicitudAspiranteRet = default;
        var argEmptySpaces = 1024;
        DetallesSolicitudAspiranteRet =
            ES.GetVar(SOLICITUDESFILE, "SOLICITUD" + NroAspirante, "Detalle", ref argEmptySpaces);
        return DetallesSolicitudAspiranteRet;
    }

    public short NumeroDeAspirante(ref string Nombre)
    {
        short NumeroDeAspiranteRet = default;
        short i;

        NumeroDeAspiranteRet = 0;

        for (i = 1; i <= modGuilds.MAXASPIRANTES; i++)
        {
            var argEmptySpaces = 1024;
            if ((ES.GetVar(SOLICITUDESFILE, "SOLICITUD" + i, "Nombre", ref argEmptySpaces).Trim().ToUpper() ?? "") ==
                (Nombre.ToUpper() ?? ""))
            {
                NumeroDeAspiranteRet = i;
                return NumeroDeAspiranteRet;
            }
        }

        return NumeroDeAspiranteRet;
    }

    public void NuevoAspirante(ref string Nombre, ref string Peticion)
    {
        short i;
        string OldQ;
        short OldQI;

        var argEmptySpaces = 1024;
        OldQ = ES.GetVar(SOLICITUDESFILE, "INIT", "CantSolicitudes", ref argEmptySpaces);
        if (Information.IsNumeric(OldQ))
            OldQI = Convert.ToInt16(OldQ);
        else
            OldQI = 0;
        for (i = 1; i <= modGuilds.MAXASPIRANTES; i++)
        {
            var argEmptySpaces1 = 1024;
            if (string.IsNullOrEmpty(ES.GetVar(SOLICITUDESFILE, "SOLICITUD" + i, "Nombre", ref argEmptySpaces1)))
            {
                ES.WriteVar(SOLICITUDESFILE, "SOLICITUD" + i, "Nombre", Nombre);
                ES.WriteVar(SOLICITUDESFILE, "SOLICITUD" + i, "Detalle",
                    string.IsNullOrEmpty(Strings.Trim(Peticion)) ? "Peticion vacia" : Peticion);
                ES.WriteVar(SOLICITUDESFILE, "INIT", "CantSolicitudes", (OldQI + 1).ToString());
                ES.WriteVar(Declaraciones.CharPath + Nombre + ".chr", "GUILD", "ASPIRANTEA", p_GuildNumber.ToString());
                return;
            }
        }
    }

    public void RetirarAspirante(ref string Nombre, ref short NroAspirante)
    {
        string OldQ;
        string OldQI;
        string Pedidos;
        short i;

        var argEmptySpaces = 1024;
        OldQ = ES.GetVar(SOLICITUDESFILE, "INIT", "CantSolicitudes", ref argEmptySpaces);
        if (Information.IsNumeric(OldQ))
            OldQI = Convert.ToInt16(OldQ).ToString();
        else
            OldQI = 1.ToString();
        // Call WriteVar(SOLICITUDESFILE, "SOLICITUD" & NroAspirante, "Nombre", vbNullString)
        // Call WriteVar(SOLICITUDESFILE, "SOLICITUD" & NroAspirante, "Detalle", vbNullString)
        ES.WriteVar(Declaraciones.CharPath + Nombre + ".chr", "GUILD", "ASPIRANTEA", "0");
        var argEmptySpaces1 = 1024;
        Pedidos = ES.GetVar(Declaraciones.CharPath + Nombre + ".chr", "GUILD", "Pedidos", ref argEmptySpaces1);
        if (!(Pedidos.IndexOf(p_GuildName, StringComparison.OrdinalIgnoreCase) + 1 > 0))
        {
            if (Migration.migr_LenB(Pedidos) != 0) Pedidos = Pedidos + ",";
            Pedidos = Pedidos + p_GuildName;
            ES.WriteVar(Declaraciones.CharPath + Nombre + ".chr", "GUILD", "Pedidos", Pedidos);
        }

        ES.WriteVar(SOLICITUDESFILE, "INIT", "CantSolicitudes", (Convert.ToInt16(OldQI) - 1).ToString());
        var loopTo = Convert.ToInt16(modGuilds.MAXASPIRANTES - 1);
        for (i = NroAspirante; i <= loopTo; i++)
        {
            var argEmptySpaces2 = 1024;
            ES.WriteVar(SOLICITUDESFILE, "SOLICITUD" + i, "Nombre",
                ES.GetVar(SOLICITUDESFILE, "SOLICITUD" + (i + 1), "Nombre", ref argEmptySpaces2));
            var argEmptySpaces3 = 1024;
            ES.WriteVar(SOLICITUDESFILE, "SOLICITUD" + i, "Detalle",
                ES.GetVar(SOLICITUDESFILE, "SOLICITUD" + (i + 1), "Detalle", ref argEmptySpaces3));
        }

        ES.WriteVar(SOLICITUDESFILE, "SOLICITUD" + modGuilds.MAXASPIRANTES, "Nombre", Constants.vbNullString);
        ES.WriteVar(SOLICITUDESFILE, "SOLICITUD" + modGuilds.MAXASPIRANTES, "Detalle", Constants.vbNullString);
    }

    public void InformarRechazoEnChar(ref string Nombre, ref string Detalles)
    {
        ES.WriteVar(Declaraciones.CharPath + Nombre + ".chr", "GUILD", "MotivoRechazo", Detalles);
    }

    // 
    // DEFINICION DEL CLAN (CODEX Y NOTICIAS)
    // 

    public string GetFechaFundacion()
    {
        string GetFechaFundacionRet = default;
        var argEmptySpaces = 1024;
        GetFechaFundacionRet = ES.GetVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "Date", ref argEmptySpaces);
        return GetFechaFundacionRet;
    }

    public void SetCodex(short CodexNumber, ref string codex)
    {
        ReplaceInvalidChars(ref codex);
        codex = codex.Substring(0, Math.Min(CODEXLENGTH, codex.Length));
        ES.WriteVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "Codex" + CodexNumber, codex);
    }

    public string GetCodex(short CodexNumber)
    {
        string GetCodexRet = default;
        var argEmptySpaces = 1024;
        GetCodexRet = ES.GetVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "Codex" + CodexNumber, ref argEmptySpaces);
        return GetCodexRet;
    }


    public void SetURL(ref string URL)
    {
        ES.WriteVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "URL", URL.Substring(0, Math.Min(40, URL.Length)));
    }

    public string GetURL()
    {
        string GetURLRet = default;
        var argEmptySpaces = 1024;
        GetURLRet = ES.GetVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "URL", ref argEmptySpaces);
        return GetURLRet;
    }

    public void SetGuildNews(ref string News)
    {
        ReplaceInvalidChars(ref News);

        News = News.Substring(0, Math.Min(NEWSLENGTH, News.Length));

        ES.WriteVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "GuildNews", News);
    }

    public string GetGuildNews()
    {
        string GetGuildNewsRet = default;
        var argEmptySpaces = 1024;
        GetGuildNewsRet = ES.GetVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "GuildNews", ref argEmptySpaces);
        return GetGuildNewsRet;
    }

    public void SetDesc(ref string desc)
    {
        ReplaceInvalidChars(ref desc);
        desc = desc.Substring(0, Math.Min(DESCLENGTH, desc.Length));

        ES.WriteVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "Desc", desc);
    }

    public string GetDesc()
    {
        string GetDescRet = default;
        var argEmptySpaces = 1024;
        GetDescRet = ES.GetVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "Desc", ref argEmptySpaces);
        return GetDescRet;
    }

    // 
    // 
    // ELECCIONES
    // 
    // 

    public bool EleccionesAbiertas()
    {
        bool EleccionesAbiertasRet = default;
        string ee;
        var argEmptySpaces = 1024;
        ee = ES.GetVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "EleccionesAbiertas", ref argEmptySpaces);
        EleccionesAbiertasRet = ee == "1"; // cualquier otra cosa da falso
        return EleccionesAbiertasRet;
    }

    public void AbrirElecciones()
    {
        ES.WriteVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "EleccionesAbiertas", "1");
        ES.WriteVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "EleccionesFinalizan", DateTime.Now.AddDays(1d).ToString());
        ES.WriteVar(VOTACIONESFILE, "INIT", "NumVotos", "0");
    }

    private void CerrarElecciones() // solo pueden cerrarse mediante recuento de votos
    {
        ES.WriteVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "EleccionesAbiertas", "0");
        ES.WriteVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "EleccionesFinalizan", Constants.vbNullString);
        FileSystem.Kill(VOTACIONESFILE); // borramos toda la evidencia ;-)
    }

    public void ContabilizarVoto(ref string Votante, ref string Votado)
    {
        short q;
        string Temps;

        var argEmptySpaces = 1024;
        Temps = ES.GetVar(VOTACIONESFILE, "INIT", "NumVotos", ref argEmptySpaces);
        q = Convert.ToInt16(Information.IsNumeric(Temps) ? Convert.ToInt16(Temps) : 0);
        ES.WriteVar(VOTACIONESFILE, "VOTOS", Votante, Votado);
        ES.WriteVar(VOTACIONESFILE, "INIT", "NumVotos", (q + 1).ToString());
    }

    public bool YaVoto(ref string Votante)
    {
        bool YaVotoRet = default;
        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Votante. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        var argEmptySpaces = 1024;
        YaVotoRet =
            Migration.migr_LenB(Strings.Trim(ES.GetVar(VOTACIONESFILE, "VOTOS", Votante, ref argEmptySpaces))) != 0;
        return YaVotoRet;
    }

    private string ContarVotos(ref short CantGanadores)
    {
        string ContarVotosRet = default;
        short q;
        short i;
        string Temps;
        string tempV;
        var d = new Dictionary<string, short>(StringComparer.OrdinalIgnoreCase);
        short currentCount;

        try
        {
            ContarVotosRet = Constants.vbNullString;
            CantGanadores = 0;
            var argEmptySpaces = 1024;
            Temps = ES.GetVar(MEMBERSFILE, "INIT", "NroMembers", ref argEmptySpaces);
            q = Convert.ToInt16(Information.IsNumeric(Temps) ? Convert.ToInt16(Temps) : 0);
            if (q > 0)
            {
                var loopTo = q;
                for (i = 1; i <= loopTo; i++)
                {
                    // miembro del clan
                    var argEmptySpaces1 = 1024;
                    Temps = ES.GetVar(MEMBERSFILE, "MEMBERS", "Member" + i, ref argEmptySpaces1);

                    // a quienvoto
                    var argEmptySpaces2 = 1024;
                    tempV = ES.GetVar(VOTACIONESFILE, "VOTOS", Temps, ref argEmptySpaces2);

                    // si voto a alguien contabilizamos el voto
                    if (Migration.migr_LenB(tempV) != 0)
                    {
                        if (d.TryGetValue(tempV, out currentCount))
                            d[tempV] = Convert.ToInt16(currentCount + 1);
                        else
                            d[tempV] = Convert.ToInt16(1);
                    }
                }

                // quien quedo con mas votos, y cuantos tuvieron esos votos?
                short maxVotes = -1;
                short cantGan = 0;
                var claveGanadora = Constants.vbNullString;

                foreach (var kvp in d)
                    if (kvp.Value > maxVotes)
                    {
                        maxVotes = kvp.Value;
                        cantGan = Convert.ToInt16(1);
                        claveGanadora = kvp.Key;
                    }
                    else if (kvp.Value == maxVotes)
                    {
                        cantGan = Convert.ToInt16(cantGan + 1);
                        claveGanadora = claveGanadora + "," + kvp.Key;
                    }

                CantGanadores = Convert.ToInt16(cantGan);
                ContarVotosRet = claveGanadora;
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in CambiarAlineacion: " + ex.Message);
            var argdesc = "clsClan.Contarvotos: " + ex.Message;
            General.LogError(ref argdesc);
            ContarVotosRet = Constants.vbNullString;
        }

        return ContarVotosRet;
    }

    public bool RevisarElecciones()
    {
        bool RevisarEleccionesRet = default;
        DateTime FechaSufragio;
        string Temps;
        string Ganador;
        var CantGanadores = default(short);
        string[] list;
        int i;

        RevisarEleccionesRet = false;
        var argEmptySpaces = 1024;
        Temps = Strings.Trim(ES.GetVar(GUILDINFOFILE, "GUILD" + p_GuildNumber, "EleccionesFinalizan",
            ref argEmptySpaces));

        if (string.IsNullOrEmpty(Temps))
            return RevisarEleccionesRet;

        if (Information.IsDate(Temps))
        {
            FechaSufragio = Conversions.ToDate(Temps);
            if (FechaSufragio < DateTime.Now) // toca!
            {
                Ganador = ContarVotos(ref CantGanadores);

                if (CantGanadores > 1)
                {
                    // empate en la votacion
                    var argNews = "*Empate en la votación. " + Ganador + " con " + CantGanadores +
                                  " votos ganaron las elecciones del clan.";
                    SetGuildNews(ref argNews);
                }
                else if (CantGanadores == 1)
                {
                    list = GetMemberList();

                    var loopTo = list.Length - 1;
                    for (i = 0; i <= loopTo; i++)
                        if ((Ganador ?? "") == (list[i] ?? ""))
                            break;

                    if (i <= list.Length - 1)
                    {
                        var argNews2 = "*" + Ganador + " ganó la elección del clan*";
                        SetGuildNews(ref argNews2);
                        SetLeader(ref Ganador);
                        RevisarEleccionesRet = true;
                    }
                    else
                    {
                        var argNews3 = "*" + Ganador +
                                       " ganó la elección del clan pero abandonó las filas por lo que la votación queda desierta*";
                        SetGuildNews(ref argNews3);
                    }
                }
                else
                {
                    var argNews1 = "*El período de votación se cerró sin votos*";
                    SetGuildNews(ref argNews1);
                }

                CerrarElecciones();
            }
        }
        else
        {
            var argdesc = "clsClan.RevisarElecciones: tempS is not Date";
            General.LogError(ref argdesc);
        }

        return RevisarEleccionesRet;
    }

    public modGuilds.RELACIONES_GUILD GetRelacion(short OtroGuild)
    {
        modGuilds.RELACIONES_GUILD GetRelacionRet = default;
        GetRelacionRet = p_Relaciones[OtroGuild];
        return GetRelacionRet;
    }

    public void SetRelacion(short GuildIndex, modGuilds.RELACIONES_GUILD Relacion)
    {
        p_Relaciones[GuildIndex] = Relacion;
        ES.WriteVar(RELACIONESFILE, "RELACIONES", GuildIndex.ToString(), modGuilds.Relacion2String(Relacion));
    }

    public void SetPropuesta(modGuilds.RELACIONES_GUILD Tipo, short OtroGuild, ref string Detalle)
    {
        ES.WriteVar(PROPUESTASFILE, OtroGuild.ToString(), "Detalle", Detalle);
        ES.WriteVar(PROPUESTASFILE, OtroGuild.ToString(), "Tipo", modGuilds.Relacion2String(Tipo));
        ES.WriteVar(PROPUESTASFILE, OtroGuild.ToString(), "Pendiente", "1");
        switch (Tipo)
        {
            case modGuilds.RELACIONES_GUILD.ALIADOS:
            {
                p_PropuestasDeAlianza.Add(OtroGuild.ToString());
                break;
            }
            case modGuilds.RELACIONES_GUILD.PAZ:
            {
                p_PropuestasDePaz.Add(OtroGuild.ToString());
                break;
            }
        }
    }

    public void AnularPropuestas(short OtroGuild)
    {
        short i;

        ES.WriteVar(PROPUESTASFILE, OtroGuild.ToString(), "Detalle", Constants.vbNullString);
        ES.WriteVar(PROPUESTASFILE, OtroGuild.ToString(), "Pendiente", "0");
        var loopTo = Convert.ToInt16(p_PropuestasDePaz.Count - 1);
        for (i = 0; i <= loopTo; i++)
        {
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_PropuestasDePaz.Item(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            if ((p_PropuestasDePaz[i] ?? "") == (OtroGuild.ToString() ?? ""))
                p_PropuestasDePaz.Remove(p_PropuestasDePaz[i]);
            return;
        }

        var loopTo1 = Convert.ToInt16(p_PropuestasDeAlianza.Count - 1);
        for (i = 0; i <= loopTo1; i++)
        {
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_PropuestasDeAlianza.Item(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            if ((p_PropuestasDeAlianza[i] ?? "") == (OtroGuild.ToString() ?? ""))
                p_PropuestasDeAlianza.Remove(p_PropuestasDeAlianza[i]);
            return;
        }
    }

    public string GetPropuesta(short OtroGuild, ref modGuilds.RELACIONES_GUILD Tipo)
    {
        string GetPropuestaRet = default;
        // trae la solicitd que haya, no valida si es actual o de que tipo es
        var argEmptySpaces = 1024;
        GetPropuestaRet = ES.GetVar(PROPUESTASFILE, OtroGuild.ToString(), "Detalle", ref argEmptySpaces);
        var argEmptySpaces1 = 1024;
        Tipo = modGuilds.String2Relacion(ES.GetVar(PROPUESTASFILE, OtroGuild.ToString(), "Tipo", ref argEmptySpaces1));
        return GetPropuestaRet;
    }

    public bool HayPropuesta(short OtroGuild, ref modGuilds.RELACIONES_GUILD Tipo)
    {
        bool HayPropuestaRet = default;
        short i;

        HayPropuestaRet = false;
        switch (Tipo)
        {
            case modGuilds.RELACIONES_GUILD.ALIADOS:
            {
                var loopTo = Convert.ToInt16(p_PropuestasDeAlianza.Count - 1);
                for (i = 0; i <= loopTo; i++)
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_PropuestasDeAlianza.Item(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    if ((p_PropuestasDeAlianza[i] ?? "") == (OtroGuild.ToString() ?? ""))
                        HayPropuestaRet = true;

                break;
            }
            case modGuilds.RELACIONES_GUILD.PAZ:
            {
                var loopTo1 = Convert.ToInt16(p_PropuestasDePaz.Count - 1);
                for (i = 0; i <= loopTo1; i++)
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_PropuestasDePaz.Item(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    if ((p_PropuestasDePaz[i] ?? "") == (OtroGuild.ToString() ?? ""))
                        HayPropuestaRet = true;

                break;
            }
            case modGuilds.RELACIONES_GUILD.GUERRA:
            {
                break;
            }
        }

        return HayPropuestaRet;
    }

    // Public Function GetEnemy(ByVal EnemyIndex As Integer) As String
    // GetEnemy = GetVar(ENEMIESFILE, "ENEMYS", "ENEMY" & EnemyIndex)
    // End Function

    // Public Function GetAllie(ByVal AllieIndex As Integer) As String
    // GetAllie = GetVar(ALLIESFILE, "ALLIES", "ALLIE" & AllieIndex)
    // End Function


    // 
    // ITERADORES
    // 

    public short Iterador_ProximaPropuesta(modGuilds.RELACIONES_GUILD Tipo)
    {
        short Iterador_ProximaPropuestaRet = default;

        Iterador_ProximaPropuestaRet = 0;
        switch (Tipo)
        {
            case modGuilds.RELACIONES_GUILD.ALIADOS:
            {
                if (p_IteradorPropuesta < Convert.ToInt16(p_PropuestasDeAlianza.Count))
                {
                    p_IteradorPropuesta = Convert.ToInt16(p_IteradorPropuesta + 1);
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_PropuestasDeAlianza.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Iterador_ProximaPropuestaRet = Convert.ToInt16(p_PropuestasDeAlianza[p_IteradorPropuesta]);
                }

                if (p_IteradorPropuesta >= Convert.ToInt16(p_PropuestasDeAlianza.Count)) p_IteradorPropuesta = 0;

                break;
            }
            case modGuilds.RELACIONES_GUILD.PAZ:
            {
                if (p_IteradorPropuesta < Convert.ToInt16(p_PropuestasDePaz.Count))
                {
                    p_IteradorPropuesta = Convert.ToInt16(p_IteradorPropuesta + 1);
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_PropuestasDePaz.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Iterador_ProximaPropuestaRet = Convert.ToInt16(p_PropuestasDePaz[p_IteradorPropuesta]);
                }

                if (p_IteradorPropuesta >= Convert.ToInt16(p_PropuestasDePaz.Count)) p_IteradorPropuesta = 0;

                break;
            }
        }

        return Iterador_ProximaPropuestaRet;
    }

    public short m_Iterador_ProximoUserIndex()
    {
        short m_Iterador_ProximoUserIndexRet = default;

        if (p_IteradorOnlineMembers < Convert.ToInt16(p_OnlineMembers.Count))
        {
            p_IteradorOnlineMembers = Convert.ToInt16(p_IteradorOnlineMembers + 1);
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_OnlineMembers.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            m_Iterador_ProximoUserIndexRet = Convert.ToInt16(p_OnlineMembers[p_IteradorOnlineMembers]);
        }
        else
        {
            p_IteradorOnlineMembers = 0;
            m_Iterador_ProximoUserIndexRet = 0;
        }

        return m_Iterador_ProximoUserIndexRet;
    }

    public short Iterador_ProximoGM()
    {
        short Iterador_ProximoGMRet = default;

        if (p_IteradorOnlineGMs < Convert.ToInt16(p_GMsOnline.Count))
        {
            p_IteradorOnlineGMs = Convert.ToInt16(p_IteradorOnlineGMs + 1);
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_GMsOnline.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Iterador_ProximoGMRet = Convert.ToInt16(p_GMsOnline[p_IteradorOnlineGMs]);
        }
        else
        {
            p_IteradorOnlineGMs = 0;
            Iterador_ProximoGMRet = 0;
        }

        return Iterador_ProximoGMRet;
    }

    public short Iterador_ProximaRelacion(modGuilds.RELACIONES_GUILD r)
    {
        short Iterador_ProximaRelacionRet = default;

        while (p_IteradorRelaciones < Convert.ToInt16(p_Relaciones.Length - 1))
        {
            p_IteradorRelaciones = Convert.ToInt16(p_IteradorRelaciones + 1);
            if (p_Relaciones[p_IteradorRelaciones] == r)
            {
                Iterador_ProximaRelacionRet = p_IteradorRelaciones;
                return Iterador_ProximaRelacionRet;
            }
        }

        if (p_IteradorRelaciones >= Convert.ToInt16(p_Relaciones.Length - 1)) p_IteradorRelaciones = 0;

        return Iterador_ProximaRelacionRet;
    }
    // 
    // 
    // 


    // 
    // ADMINISTRATIVAS
    // 

    public void ConectarGM(short UserIndex)
    {
        p_GMsOnline.Add(UserIndex.ToString());
    }

    public void DesconectarGM(short UserIndex)
    {
        short i;
        var loopTo = Convert.ToInt16(p_GMsOnline.Count - 1);
        for (i = 0; i <= loopTo; i++)
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_GMsOnline.Item(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            if ((p_GMsOnline[i] ?? "") == (UserIndex.ToString() ?? ""))
                p_GMsOnline.Remove(p_GMsOnline[i]);
    }


    // 
    // VARIAS, EXTRAS Y DEMASES
    // 

    private void ReplaceInvalidChars(ref string S)
    {
        if (Migration.migr_InStrB(S, "\r") != 0) S = S.Replace("\r", Constants.vbNullString);
        if (Migration.migr_InStrB(S, "\n") != 0) S = S.Replace("\n", Constants.vbNullString);
        if (Migration.migr_InStrB(S, "¬") != 0)
            S = S.Replace("¬", Constants.vbNullString); // morgo usaba esto como "separador"
    }
}