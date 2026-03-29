using System;
using System.IO;
using Microsoft.VisualBasic;

namespace Legacy;

internal static class modGuilds
{
    // puntos maximos de antifaccion que un clan tolera antes de ser cambiada su alineacion

    public enum ALINEACION_GUILD
    {
        ALINEACION_LEGION = 1,
        ALINEACION_CRIMINAL = 2,
        ALINEACION_NEUTRO = 3,
        ALINEACION_CIUDA = 4,
        ALINEACION_ARMADA = 5,
        ALINEACION_MASTER = 6
    }
    // numero de .wav del cliente

    public enum RELACIONES_GUILD
    {
        GUERRA = -1,
        PAZ = 0,
        ALIADOS = 1
    }
    // alineaciones permitidas

    public enum SONIDOS_GUILD
    {
        SND_CREACIONCLAN = 44,
        SND_ACEPTADOCLAN = 43,
        SND_DECLAREWAR = 45
    }
    // archivo ./guilds/guildinfo.ini o similar

    private const short MAX_GUILDS = 1000;
    // array global de guilds, se indexa por userlist().guildindex

    private const byte CANTIDADMAXIMACODEX = 8;
    // cantidad maxima de codecs que se pueden definir

    public const byte MAXASPIRANTES = 10;
    // cantidad maxima de aspirantes que puede tener un clan acumulados a la vez

    private const byte MAXANTIFACCION = 5;
    // **************************************************************
    // modGuilds.bas - Module to allow the usage of areas instead of maps.
    // Saves a lot of bandwidth.
    // 
    // Implemented by Mariano Barrou (El Oso)
    // **************************************************************

    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // DECLARACIOENS PUBLICAS CONCERNIENTES AL JUEGO
    // Y CONFIGURACION DEL SISTEMA DE CLANES
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    private static string GUILDINFOFILE;
    // cantidad maxima de guilds en el servidor

    public static short CANTIDADDECLANES;
    // cantidad actual de clanes en el servidor

    // UPGRADE_WARNING: El límite inferior de la matriz guilds ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    private static readonly clsClan[] guilds = new clsClan[MAX_GUILDS + 1];
    // estado entre clanes
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    public static void LoadGuildsDB()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        string CantClanes;
        short i;
        string tempStr;
        ALINEACION_GUILD Alin;

        GUILDINFOFILE = AppDomain.CurrentDomain.BaseDirectory + "guilds/guildsinfo.inf";

        var argEmptySpaces = 1024;
        CantClanes = ES.GetVar(GUILDINFOFILE, "INIT", "nroGuilds", ref argEmptySpaces);

        if (Information.IsNumeric(CantClanes))
            CANTIDADDECLANES = Convert.ToInt16(CantClanes);
        else
            CANTIDADDECLANES = 0;

        var loopTo = CANTIDADDECLANES;
        for (i = 1; i <= loopTo; i++)
        {
            guilds[i] = new clsClan();
            var argEmptySpaces1 = 1024;
            tempStr = ES.GetVar(GUILDINFOFILE, "GUILD" + i, "GUILDNAME", ref argEmptySpaces1);
            var argEmptySpaces2 = 1024;
            var argS = ES.GetVar(GUILDINFOFILE, "GUILD" + i, "Alineacion", ref argEmptySpaces2);
            Alin = String2Alineacion(ref argS);
            guilds[i].Inicializar(tempStr, i, Alin);
        }
    }

    public static bool m_ConectarMiembroAClan(short UserIndex, short GuildIndex)
    {
        bool m_ConectarMiembroAClanRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************


        var NuevaA = default(bool);
        var News = default(string);

        if ((GuildIndex > CANTIDADDECLANES) | (GuildIndex <= 0))
            return m_ConectarMiembroAClanRet; // x las dudas...
        if (m_EstadoPermiteEntrar(UserIndex, GuildIndex))
        {
            guilds[GuildIndex].ConectarMiembro(UserIndex);
            Declaraciones.UserList[UserIndex].GuildIndex = GuildIndex;
            m_ConectarMiembroAClanRet = true;
        }
        else
        {
            m_ConectarMiembroAClanRet = m_ValidarPermanencia(UserIndex, true, ref NuevaA);
            if (NuevaA)
                News = News + "El clan tiene nueva alineación.";
            // If NuevoL Or NuevaA Then Call guilds(GuildIndex).SetGuildNews(News)
        }

        return m_ConectarMiembroAClanRet;
    }


    public static bool m_ValidarPermanencia(short UserIndex, bool SumaAntifaccion, ref bool CambioAlineacion)
    {
        bool m_ValidarPermanenciaRet = default;
        // ***************************************************
        // Autor: Unknown (orginal version)
        // Last Modification: 14/12/2009
        // 25/03/2009: ZaMa - Desequipo los items faccionarios que tenga el funda al abandonar la faccion
        // 14/12/2009: ZaMa - La alineacion del clan depende del lider
        // 14/02/2010: ZaMa - Ya no es necesario saber si el lider cambia, ya que no puede cambiar.
        // ***************************************************

        short GuildIndex;

        m_ValidarPermanenciaRet = true;

        GuildIndex = Declaraciones.UserList[UserIndex].GuildIndex;
        if ((GuildIndex > CANTIDADDECLANES) & (GuildIndex <= 0))
            return m_ValidarPermanenciaRet;

        if (!m_EstadoPermiteEntrar(UserIndex, GuildIndex))
        {
            // Es el lider, bajamos 1 rango de alineacion
            if ((GuildLeader(GuildIndex) ?? "") == (Declaraciones.UserList[UserIndex].name ?? ""))
            {
                General.LogClanes(Declaraciones.UserList[UserIndex].name + ", líder de " +
                                  guilds[GuildIndex].GuildName + " hizo bajar la alienación de su clan.");

                CambioAlineacion = true;

                // Por si paso de ser armada/legion a pk/ciuda, chequeo de nuevo
                do
                {
                    UpdateGuildMembers(GuildIndex);
                } while (!m_EstadoPermiteEntrar(UserIndex, GuildIndex));
            }
            else
            {
                General.LogClanes(Declaraciones.UserList[UserIndex].name + " de " + guilds[GuildIndex].GuildName +
                                  " es expulsado en validar permanencia.");

                m_ValidarPermanenciaRet = false;
                if (SumaAntifaccion)
                    guilds[GuildIndex].PuntosAntifaccion = Convert.ToInt16(guilds[GuildIndex].PuntosAntifaccion + 1);

                CambioAlineacion = guilds[GuildIndex].PuntosAntifaccion == MAXANTIFACCION;

                General.LogClanes(Declaraciones.UserList[UserIndex].name + " de " + guilds[GuildIndex].GuildName +
                                  (CambioAlineacion ? " SI " : " NO ") + "provoca cambio de alineación. MAXANT:" +
                                  CambioAlineacion);

                m_EcharMiembroDeClan(-1, Declaraciones.UserList[UserIndex].name);

                // Llegamos a la maxima cantidad de antifacciones permitidas, bajamos un grado de alineación
                if (CambioAlineacion) UpdateGuildMembers(GuildIndex);
            }
        }

        return m_ValidarPermanenciaRet;
    }

    private static void UpdateGuildMembers(short GuildIndex)
    {
        // ***************************************************
        // Autor: ZaMa
        // Last Modification: 14/01/2010 (ZaMa)
        // 14/01/2010: ZaMa - Pulo detalles en el funcionamiento general.
        // ***************************************************
        string[] GuildMembers;
        short TotalMembers;
        int MemberIndex;
        bool Sale;
        string MemberName;
        short UserIndex;
        short Reenlistadas;

        // Si devuelve true, cambio a neutro y echamos a todos los que estén de mas, sino no echamos a nadie
        if (guilds[GuildIndex].CambiarAlineacion(BajarGrado(GuildIndex))) // ALINEACION_NEUTRO)
        {
            // uso GetMemberList y no los iteradores pq voy a rajar gente y puedo alterar
            // internamente al iterador en el proceso
            GuildMembers = guilds[GuildIndex].GetMemberList();
            TotalMembers = Convert.ToInt16(GuildMembers.Length - 1);

            var loopTo = (int)TotalMembers;
            for (MemberIndex = 0; MemberIndex <= loopTo; MemberIndex++)
            {
                MemberName = GuildMembers[MemberIndex];

                // vamos a violar un poco de capas..
                UserIndex = Extra.NameIndex(MemberName);
                if (UserIndex > 0)
                    Sale = !m_EstadoPermiteEntrar(UserIndex, GuildIndex);
                else
                    Sale = !m_EstadoPermiteEntrarChar(ref MemberName, GuildIndex);

                if (Sale)
                {
                    if (m_EsGuildLeader(ref MemberName, GuildIndex)) // hay que sacarlo de las facciones
                    {
                        if (UserIndex > 0)
                        {
                            if (Declaraciones.UserList[UserIndex].Faccion.ArmadaReal != 0)
                            {
                                var argExpulsado = true;
                                ModFacciones.ExpulsarFaccionReal(UserIndex, ref argExpulsado);
                                // No cuenta como reenlistada :p.
                                Declaraciones.UserList[UserIndex].Faccion.Reenlistadas =
                                    Convert.ToByte(Declaraciones.UserList[UserIndex].Faccion.Reenlistadas - 1);
                            }
                            else if (Declaraciones.UserList[UserIndex].Faccion.FuerzasCaos != 0)
                            {
                                var argExpulsado1 = true;
                                ModFacciones.ExpulsarFaccionCaos(UserIndex, ref argExpulsado1);
                                // No cuenta como reenlistada :p.
                                Declaraciones.UserList[UserIndex].Faccion.Reenlistadas =
                                    Convert.ToByte(Declaraciones.UserList[UserIndex].Faccion.Reenlistadas - 1);
                            }
                        }
                        else if (File.Exists(Declaraciones.CharPath + MemberName + ".chr"))
                        {
                            ES.WriteVar(Declaraciones.CharPath + MemberName + ".chr", "FACCIONES", "EjercitoCaos",
                                0.ToString());
                            ES.WriteVar(Declaraciones.CharPath + MemberName + ".chr", "FACCIONES", "EjercitoReal",
                                0.ToString());
                            var argEmptySpaces = 1024;
                            Reenlistadas = Convert.ToInt16(ES.GetVar(Declaraciones.CharPath + MemberName + ".chr",
                                "FACCIONES", "Reenlistadas", ref argEmptySpaces));
                            ES.WriteVar(Declaraciones.CharPath + MemberName + ".chr", "FACCIONES", "Reenlistadas",
                                (Reenlistadas > 1 ? Reenlistadas - 1 : Reenlistadas).ToString());
                        }
                    }
                    else // sale si no es guildLeader
                    {
                        m_EcharMiembroDeClan(-1, MemberName);
                    }
                }
            }
        }
        else
        {
            // Resetea los puntos de antifacción
            guilds[GuildIndex].PuntosAntifaccion = 0;
        }
    }

    private static ALINEACION_GUILD BajarGrado(short GuildIndex)
    {
        ALINEACION_GUILD BajarGradoRet = default;
        // ***************************************************
        // Autor: ZaMa
        // Last Modification: 27/11/2009
        // Reduce el grado de la alineacion a partir de la alineacion dada
        // ***************************************************

        switch (guilds[GuildIndex].Alineacion)
        {
            case ALINEACION_GUILD.ALINEACION_ARMADA:
            {
                BajarGradoRet = ALINEACION_GUILD.ALINEACION_CIUDA;
                break;
            }
            case ALINEACION_GUILD.ALINEACION_LEGION:
            {
                BajarGradoRet = ALINEACION_GUILD.ALINEACION_CRIMINAL;
                break;
            }

            default:
            {
                BajarGradoRet = ALINEACION_GUILD.ALINEACION_NEUTRO;
                break;
            }
        }

        return BajarGradoRet;
    }

    public static void m_DesconectarMiembroDelClan(short UserIndex, short GuildIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if (Declaraciones.UserList[UserIndex].GuildIndex > CANTIDADDECLANES)
            return;
        guilds[GuildIndex].DesConectarMiembro(UserIndex);
    }

    private static bool m_EsGuildLeader(ref string PJ, short GuildIndex)
    {
        bool m_EsGuildLeaderRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        m_EsGuildLeaderRet = (PJ.ToUpper() ?? "") == (guilds[GuildIndex].GetLeader().Trim().ToUpper() ?? "");
        return m_EsGuildLeaderRet;
    }

    private static bool m_EsGuildFounder(ref string PJ, short GuildIndex)
    {
        bool m_EsGuildFounderRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        m_EsGuildFounderRet = (PJ.ToUpper() ?? "") == (guilds[GuildIndex].Fundador.Trim().ToUpper() ?? "");
        return m_EsGuildFounderRet;
    }

    public static short m_EcharMiembroDeClan(short Expulsador, string Expulsado)
    {
        short m_EcharMiembroDeClanRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // UI echa a Expulsado del clan de Expulsado
        short UserIndex;
        short GI;

        m_EcharMiembroDeClanRet = 0;

        UserIndex = Extra.NameIndex(Expulsado);
        if (UserIndex > 0)
        {
            // pj online
            GI = Declaraciones.UserList[UserIndex].GuildIndex;
            if (GI > 0)
            {
                if (m_PuedeSalirDeClan(ref Expulsado, GI, Expulsador))
                {
                    guilds[GI].DesConectarMiembro(UserIndex);
                    guilds[GI].ExpulsarMiembro(ref Expulsado);
                    General.LogClanes(Expulsado + " ha sido expulsado de " + guilds[GI].GuildName + " Expulsador = " +
                                      Expulsador);
                    Declaraciones.UserList[UserIndex].GuildIndex = 0;
                    UsUaRiOs.RefreshCharStatus(UserIndex);
                    m_EcharMiembroDeClanRet = GI;
                }
                else
                {
                    m_EcharMiembroDeClanRet = 0;
                }
            }
            else
            {
                m_EcharMiembroDeClanRet = 0;
            }
        }
        else
        {
            // pj offline
            GI = GetGuildIndexFromChar(ref Expulsado);
            if (GI > 0)
            {
                if (m_PuedeSalirDeClan(ref Expulsado, GI, Expulsador))
                {
                    guilds[GI].ExpulsarMiembro(ref Expulsado);
                    General.LogClanes(Expulsado + " ha sido expulsado de " + guilds[GI].GuildName + " Expulsador = " +
                                      Expulsador);
                    m_EcharMiembroDeClanRet = GI;
                }
                else
                {
                    m_EcharMiembroDeClanRet = 0;
                }
            }
            else
            {
                m_EcharMiembroDeClanRet = 0;
            }
        }

        return m_EcharMiembroDeClanRet;
    }

    public static void ActualizarWebSite(short UserIndex, ref string Web)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short GI;

        GI = Declaraciones.UserList[UserIndex].GuildIndex;
        if ((GI <= 0) | (GI > CANTIDADDECLANES))
            return;

        if (!m_EsGuildLeader(ref Declaraciones.UserList[UserIndex].name, GI))
            return;

        guilds[GI].SetURL(ref Web);
    }


    public static void ChangeCodexAndDesc(ref string desc, ref string[] codex, short GuildIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int i;

        if ((GuildIndex < 1) | (GuildIndex > CANTIDADDECLANES))
            return;

        {
            ref var withBlock = ref guilds[GuildIndex];
            withBlock.SetDesc(ref desc);

            var loopTo = codex.Length - 1;
            for (i = 0; i <= loopTo; i++)
                withBlock.SetCodex(Convert.ToByte(i), ref codex[i]);

            var loopTo1 = Convert.ToInt32(CANTIDADMAXIMACODEX);
            for (i = i; i <= loopTo1; i++)
            {
                var argcodex = Constants.vbNullString;
                withBlock.SetCodex(Convert.ToByte(i), ref argcodex);
            }
        }
    }

    public static void ActualizarNoticias(short UserIndex, ref string datos)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 21/02/2010
        // 21/02/2010: ZaMa - Ahora le avisa a los miembros que cambio el guildnews.
        // ***************************************************

        short GI;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            GI = withBlock.GuildIndex;

            if ((GI <= 0) | (GI > CANTIDADDECLANES))
                return;

            if (!m_EsGuildLeader(ref withBlock.name, GI))
                return;

            guilds[GI].SetGuildNews(ref datos);

            modSendData.SendData(modSendData.SendTarget.ToDiosesYclan, withBlock.GuildIndex,
                Protocol.PrepareMessageGuildChat(withBlock.name + " ha actualizado las noticias del clan!"));
        }
    }

    public static bool CrearNuevoClan(short FundadorIndex, ref string desc, ref string GuildName, ref string URL,
        ref string[] codex, ALINEACION_GUILD Alineacion, ref string refError)
    {
        bool CrearNuevoClanRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short CantCodex;
        short i;
        var DummyString = default(string);

        CrearNuevoClanRet = false;
        if (!PuedeFundarUnClan(FundadorIndex, Alineacion, ref DummyString))
        {
            refError = DummyString;
            return CrearNuevoClanRet;
        }

        if (string.IsNullOrEmpty(GuildName) | !GuildNameValido(GuildName))
        {
            refError = "Nombre de clan inválido.";
            return CrearNuevoClanRet;
        }

        if (YaExiste(GuildName))
        {
            refError = "Ya existe un clan con ese nombre.";
            return CrearNuevoClanRet;
        }

        CantCodex = Convert.ToInt16(codex.Length);

        // tenemos todo para fundar ya
        if (CANTIDADDECLANES < guilds.Length - 1)
        {
            CANTIDADDECLANES = Convert.ToInt16(CANTIDADDECLANES + 1);
            // ReDim Preserve Guilds(1 To CANTIDADDECLANES) As clsClan

            // constructor custom de la clase clan
            guilds[CANTIDADDECLANES] = new clsClan();

            {
                ref var withBlock = ref guilds[CANTIDADDECLANES];
                withBlock.Inicializar(GuildName, CANTIDADDECLANES, Alineacion);

                // Damos de alta al clan como nuevo inicializando sus archivos
                withBlock.InicializarNuevoClan(ref Declaraciones.UserList[FundadorIndex].name);

                // seteamos codex y descripcion
                var loopTo = CantCodex;
                for (i = 1; i <= loopTo; i++)
                    withBlock.SetCodex(Convert.ToByte(i), ref codex[i - 1]);
                withBlock.SetDesc(ref desc);
                var argNews = "Clan creado con alineación: " + Alineacion2String(Alineacion);
                withBlock.SetGuildNews(ref argNews);
                withBlock.SetLeader(ref Declaraciones.UserList[FundadorIndex].name);
                withBlock.SetURL(ref URL);

                // "conectamos" al nuevo miembro a la lista de la clase
                withBlock.AceptarNuevoMiembro(ref Declaraciones.UserList[FundadorIndex].name);
                withBlock.ConectarMiembro(FundadorIndex);
            }

            Declaraciones.UserList[FundadorIndex].GuildIndex = CANTIDADDECLANES;
            UsUaRiOs.RefreshCharStatus(FundadorIndex);

            var loopTo1 = Convert.ToInt16(CANTIDADDECLANES - 1);
            for (i = 1; i <= loopTo1; i++)
                guilds[i].ProcesarFundacionDeOtroClan();
        }
        else
        {
            refError = "No hay más slots para fundar clanes. Consulte a un administrador.";
            return CrearNuevoClanRet;
        }

        CrearNuevoClanRet = true;
        return CrearNuevoClanRet;
    }

    public static void SendGuildNews(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short GuildIndex;
        short i;
        short go;

        GuildIndex = Declaraciones.UserList[UserIndex].GuildIndex;
        if (GuildIndex == 0)
            return;

        string[] enemies;

        string[] allies;
        {
            ref var withBlock = ref guilds[GuildIndex];
            if (withBlock.CantidadEnemys != 0)
                enemies = new string[withBlock.CantidadEnemys];
            else
                enemies = new string[1];


            if (withBlock.CantidadAllies != 0)
                allies = new string[withBlock.CantidadAllies];
            else
                allies = new string[1];

            i = withBlock.Iterador_ProximaRelacion(RELACIONES_GUILD.GUERRA);
            go = 0;

            while (i > 0)
            {
                enemies[go] = guilds[i].GuildName;
                i = withBlock.Iterador_ProximaRelacion(RELACIONES_GUILD.GUERRA);
                go = Convert.ToInt16(go + 1);
            }

            i = withBlock.Iterador_ProximaRelacion(RELACIONES_GUILD.ALIADOS);
            go = 0;

            while (i > 0)
            {
                allies[go] = guilds[i].GuildName;
                i = withBlock.Iterador_ProximaRelacion(RELACIONES_GUILD.ALIADOS);
            }

            Protocol.WriteGuildNews(UserIndex, withBlock.GetGuildNews(), enemies, allies);

            if (withBlock.EleccionesAbiertas())
            {
                Protocol.WriteConsoleMsg(UserIndex, "Hoy es la votación para elegir un nuevo líder para el clan.",
                    Protocol.FontTypeNames.FONTTYPE_GUILD);
                Protocol.WriteConsoleMsg(UserIndex,
                    "La elección durará 24 horas, se puede votar a cualquier miembro del clan.",
                    Protocol.FontTypeNames.FONTTYPE_GUILD);
                Protocol.WriteConsoleMsg(UserIndex, "Para votar escribe /VOTO NICKNAME.",
                    Protocol.FontTypeNames.FONTTYPE_GUILD);
                Protocol.WriteConsoleMsg(UserIndex,
                    "Sólo se computará un voto por miembro. Tu voto no puede ser cambiado.",
                    Protocol.FontTypeNames.FONTTYPE_GUILD);
            }
        }
    }

    public static bool m_PuedeSalirDeClan(ref string Nombre, short GuildIndex, short QuienLoEchaUI)
    {
        bool m_PuedeSalirDeClanRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // sale solo si no es fundador del clan.

        m_PuedeSalirDeClanRet = false;
        if (GuildIndex == 0)
            return m_PuedeSalirDeClanRet;

        // esto es un parche, si viene en -1 es porque la invoca la rutina de expulsion automatica de clanes x antifacciones
        if (QuienLoEchaUI == -1)
        {
            m_PuedeSalirDeClanRet = true;
            return m_PuedeSalirDeClanRet;
        }

        // cuando UI no puede echar a nombre?
        // si no es gm Y no es lider del clan del pj Y no es el mismo que se va voluntariamente
        if ((Declaraciones.UserList[QuienLoEchaUI].flags.Privilegios & Declaraciones.PlayerType.User) != 0)
        {
            bool localm_EsGuildLeader()
            {
                var argPJ = Declaraciones.UserList[QuienLoEchaUI].name.ToUpper();
                var ret = m_EsGuildLeader(ref argPJ, GuildIndex);
                return ret;
            }

            if (!localm_EsGuildLeader())
                if ((Declaraciones.UserList[QuienLoEchaUI].name.ToUpper() ?? "") !=
                    (Nombre.ToUpper() ?? "")) // si no sale voluntariamente...
                    return m_PuedeSalirDeClanRet;
        }

        // Ahora el lider es el unico que no puede salir del clan
        m_PuedeSalirDeClanRet = (guilds[GuildIndex].GetLeader().ToUpper() ?? "") != (Nombre.ToUpper() ?? "");
        return m_PuedeSalirDeClanRet;
    }

    public static bool PuedeFundarUnClan(short UserIndex, ALINEACION_GUILD Alineacion, ref string refError)
    {
        bool PuedeFundarUnClanRet = default;
        // ***************************************************
        // Autor: Unknown
        // Last Modification: 27/11/2009
        // Returns true if can Found a guild
        // 27/11/2009: ZaMa - Ahora valida si ya fundo clan o no.
        // ***************************************************

        if (Declaraciones.UserList[UserIndex].GuildIndex > 0)
        {
            refError = "Ya perteneces a un clan, no puedes fundar otro";
            return PuedeFundarUnClanRet;
        }

        if ((Declaraciones.UserList[UserIndex].Stats.ELV < 25) |
            (Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Liderazgo] < 90))
        {
            refError = "Para fundar un clan debes ser nivel 25 y tener 90 skills en liderazgo.";
            return PuedeFundarUnClanRet;
        }

        switch (Alineacion)
        {
            case ALINEACION_GUILD.ALINEACION_ARMADA:
            {
                if (Declaraciones.UserList[UserIndex].Faccion.ArmadaReal != 1)
                {
                    refError = "Para fundar un clan real debes ser miembro del ejército real.";
                    return PuedeFundarUnClanRet;
                }

                break;
            }
            case ALINEACION_GUILD.ALINEACION_CIUDA:
            {
                if (ES.criminal(UserIndex))
                {
                    refError = "Para fundar un clan de ciudadanos no debes ser criminal.";
                    return PuedeFundarUnClanRet;
                }

                break;
            }
            case ALINEACION_GUILD.ALINEACION_CRIMINAL:
            {
                if (!ES.criminal(UserIndex))
                {
                    refError = "Para fundar un clan de criminales no debes ser ciudadano.";
                    return PuedeFundarUnClanRet;
                }

                break;
            }
            case ALINEACION_GUILD.ALINEACION_LEGION:
            {
                if (Declaraciones.UserList[UserIndex].Faccion.FuerzasCaos != 1)
                {
                    refError = "Para fundar un clan del mal debes pertenecer a la legión oscura.";
                    return PuedeFundarUnClanRet;
                }

                break;
            }
            case ALINEACION_GUILD.ALINEACION_MASTER:
            {
                if ((Declaraciones.UserList[UserIndex].flags.Privilegios & (Declaraciones.PlayerType.User |
                                                                            Declaraciones.PlayerType.Consejero |
                                                                            Declaraciones.PlayerType.SemiDios)) != 0)
                {
                    refError = "Para fundar un clan sin alineación debes ser un dios.";
                    return PuedeFundarUnClanRet;
                }

                break;
            }
            case ALINEACION_GUILD.ALINEACION_NEUTRO:
            {
                if ((Declaraciones.UserList[UserIndex].Faccion.ArmadaReal != 0) |
                    (Declaraciones.UserList[UserIndex].Faccion.FuerzasCaos != 0))
                {
                    refError = "Para fundar un clan neutro no debes pertenecer a ninguna facción.";
                    return PuedeFundarUnClanRet;
                }

                break;
            }
        }

        PuedeFundarUnClanRet = true;
        return PuedeFundarUnClanRet;
    }

    private static bool m_EstadoPermiteEntrarChar(ref string Personaje, short GuildIndex)
    {
        bool m_EstadoPermiteEntrarCharRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int Promedio;
        short ELV;
        var f = default(byte);

        m_EstadoPermiteEntrarCharRet = false;

        if (Migration.migr_InStrB(Personaje, @"\") != 0) Personaje = Personaje.Replace(@"\", Constants.vbNullString);
        if (Migration.migr_InStrB(Personaje, "/") != 0) Personaje = Personaje.Replace("/", Constants.vbNullString);
        if (Migration.migr_InStrB(Personaje, ".") != 0) Personaje = Personaje.Replace(".", Constants.vbNullString);

        if (File.Exists(Declaraciones.CharPath + Personaje + ".chr"))
        {
            var argEmptySpaces = 1024;
            Promedio = Convert.ToInt32(ES.GetVar(Declaraciones.CharPath + Personaje + ".chr", "REP", "Promedio",
                ref argEmptySpaces));
            switch (guilds[GuildIndex].Alineacion)
            {
                case ALINEACION_GUILD.ALINEACION_ARMADA:
                {
                    if (Promedio >= 0)
                    {
                        var argEmptySpaces1 = 1024;
                        ELV = Convert.ToInt16(ES.GetVar(Declaraciones.CharPath + Personaje + ".chr", "Stats", "ELV",
                            ref argEmptySpaces1));
                        if (ELV >= 25)
                        {
                            var argEmptySpaces2 = 1024;
                            f = Convert.ToByte(ES.GetVar(Declaraciones.CharPath + Personaje + ".chr", "Facciones",
                                "EjercitoReal", ref argEmptySpaces2));
                        }

                        m_EstadoPermiteEntrarCharRet = ELV >= 25 ? f != 0 : true;
                    }

                    break;
                }
                case ALINEACION_GUILD.ALINEACION_CIUDA:
                {
                    m_EstadoPermiteEntrarCharRet = Promedio >= 0;
                    break;
                }
                case ALINEACION_GUILD.ALINEACION_CRIMINAL:
                {
                    m_EstadoPermiteEntrarCharRet = Promedio < 0;
                    break;
                }
                case ALINEACION_GUILD.ALINEACION_NEUTRO:
                {
                    var argEmptySpaces3 = 1024;
                    m_EstadoPermiteEntrarCharRet = Convert.ToByte(ES.GetVar(Declaraciones.CharPath + Personaje + ".chr",
                        "Facciones", "EjercitoReal", ref argEmptySpaces3)) == 0;
                    var argEmptySpaces4 = 1024;
                    m_EstadoPermiteEntrarCharRet = m_EstadoPermiteEntrarCharRet &
                                                   (Convert.ToByte(ES.GetVar(
                                                       Declaraciones.CharPath + Personaje + ".chr", "Facciones",
                                                       "EjercitoCaos", ref argEmptySpaces4)) == 0);
                    break;
                }
                case ALINEACION_GUILD.ALINEACION_LEGION:
                {
                    if (Promedio < 0)
                    {
                        var argEmptySpaces5 = 1024;
                        ELV = Convert.ToInt16(ES.GetVar(Declaraciones.CharPath + Personaje + ".chr", "Stats", "ELV",
                            ref argEmptySpaces5));
                        if (ELV >= 25)
                        {
                            var argEmptySpaces6 = 1024;
                            f = Convert.ToByte(ES.GetVar(Declaraciones.CharPath + Personaje + ".chr", "Facciones",
                                "EjercitoCaos", ref argEmptySpaces6));
                        }

                        m_EstadoPermiteEntrarCharRet = ELV >= 25 ? f != 0 : true;
                    }

                    break;
                }

                default:
                {
                    m_EstadoPermiteEntrarCharRet = true;
                    break;
                }
            }
        }

        return m_EstadoPermiteEntrarCharRet;
    }

    private static bool m_EstadoPermiteEntrar(short UserIndex, short GuildIndex)
    {
        bool m_EstadoPermiteEntrarRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        switch (guilds[GuildIndex].Alineacion)
        {
            case ALINEACION_GUILD.ALINEACION_ARMADA:
            {
                m_EstadoPermiteEntrarRet = !ES.criminal(UserIndex) & (Declaraciones.UserList[UserIndex].Stats.ELV >= 25
                    ? Declaraciones.UserList[UserIndex].Faccion.ArmadaReal != 0
                    : true);
                break;
            }

            case ALINEACION_GUILD.ALINEACION_LEGION:
            {
                m_EstadoPermiteEntrarRet = ES.criminal(UserIndex) & (Declaraciones.UserList[UserIndex].Stats.ELV >= 25
                    ? Declaraciones.UserList[UserIndex].Faccion.FuerzasCaos != 0
                    : true);
                break;
            }

            case ALINEACION_GUILD.ALINEACION_NEUTRO:
            {
                m_EstadoPermiteEntrarRet = (Declaraciones.UserList[UserIndex].Faccion.ArmadaReal == 0) &
                                           (Declaraciones.UserList[UserIndex].Faccion.FuerzasCaos == 0);
                break;
            }

            case ALINEACION_GUILD.ALINEACION_CIUDA:
            {
                m_EstadoPermiteEntrarRet = !ES.criminal(UserIndex);
                break;
            }

            case ALINEACION_GUILD.ALINEACION_CRIMINAL:
            {
                m_EstadoPermiteEntrarRet = ES.criminal(UserIndex); // game masters
                break;
            }

            default:
            {
                m_EstadoPermiteEntrarRet = true;
                break;
            }
        }

        return m_EstadoPermiteEntrarRet;
    }

    public static ALINEACION_GUILD String2Alineacion(ref string S)
    {
        ALINEACION_GUILD String2AlineacionRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        switch (S ?? "")
        {
            case "Neutral":
            {
                String2AlineacionRet = ALINEACION_GUILD.ALINEACION_NEUTRO;
                break;
            }
            case "Del Mal":
            {
                String2AlineacionRet = ALINEACION_GUILD.ALINEACION_LEGION;
                break;
            }
            case "Real":
            {
                String2AlineacionRet = ALINEACION_GUILD.ALINEACION_ARMADA;
                break;
            }
            case "Game Masters":
            {
                String2AlineacionRet = ALINEACION_GUILD.ALINEACION_MASTER;
                break;
            }
            case "Legal":
            {
                String2AlineacionRet = ALINEACION_GUILD.ALINEACION_CIUDA;
                break;
            }
            case "Criminal":
            {
                String2AlineacionRet = ALINEACION_GUILD.ALINEACION_CRIMINAL;
                break;
            }

            default:
            {
                String2AlineacionRet = ALINEACION_GUILD.ALINEACION_NEUTRO;
                break;
            }
        }

        return String2AlineacionRet;
    }

    public static string Alineacion2String(ALINEACION_GUILD Alineacion)
    {
        string Alineacion2StringRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        switch (Alineacion)
        {
            case ALINEACION_GUILD.ALINEACION_NEUTRO:
            {
                Alineacion2StringRet = "Neutral";
                break;
            }
            case ALINEACION_GUILD.ALINEACION_LEGION:
            {
                Alineacion2StringRet = "Del Mal";
                break;
            }
            case ALINEACION_GUILD.ALINEACION_ARMADA:
            {
                Alineacion2StringRet = "Real";
                break;
            }
            case ALINEACION_GUILD.ALINEACION_MASTER:
            {
                Alineacion2StringRet = "Game Masters";
                break;
            }
            case ALINEACION_GUILD.ALINEACION_CIUDA:
            {
                Alineacion2StringRet = "Legal";
                break;
            }
            case ALINEACION_GUILD.ALINEACION_CRIMINAL:
            {
                Alineacion2StringRet = "Criminal";
                break;
            }
        }

        return Alineacion2StringRet;
    }

    public static string Relacion2String(RELACIONES_GUILD Relacion)
    {
        string Relacion2StringRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        switch (Relacion)
        {
            case RELACIONES_GUILD.ALIADOS:
            {
                Relacion2StringRet = "A";
                break;
            }
            case RELACIONES_GUILD.GUERRA:
            {
                Relacion2StringRet = "G";
                break;
            }
            case RELACIONES_GUILD.PAZ:
            {
                Relacion2StringRet = "P";
                break;
            }
            case var @case when @case == RELACIONES_GUILD.ALIADOS:
            {
                Relacion2StringRet = "?";
                break;
            }
        }

        return Relacion2StringRet;
    }

    public static RELACIONES_GUILD String2Relacion(string S)
    {
        RELACIONES_GUILD String2RelacionRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        switch (S.Trim().ToUpper() ?? "")
        {
            case var @case when @case == "":
            case "P":
            {
                String2RelacionRet = RELACIONES_GUILD.PAZ;
                break;
            }
            case "G":
            {
                String2RelacionRet = RELACIONES_GUILD.GUERRA;
                break;
            }
            case "A":
            {
                String2RelacionRet = RELACIONES_GUILD.ALIADOS;
                break;
            }

            default:
            {
                String2RelacionRet = RELACIONES_GUILD.PAZ;
                break;
            }
        }

        return String2RelacionRet;
    }

    private static bool GuildNameValido(string cad)
    {
        bool GuildNameValidoRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        byte car;
        short i;

        // old function by morgo

        cad = cad.ToLower();

        var loopTo = Convert.ToInt16(cad.Length);
        for (i = 1; i <= loopTo; i++)
        {
            car = Convert.ToByte(Strings.AscW(cad[i - 1]));

            if (((car < 97) | (car > 122)) & (car != 255) & (car != 32))
            {
                GuildNameValidoRet = false;
                return GuildNameValidoRet;
            }
        }

        GuildNameValidoRet = true;
        return GuildNameValidoRet;
    }

    private static bool YaExiste(string GuildName)
    {
        bool YaExisteRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short i;

        YaExisteRet = false;
        GuildName = GuildName.ToUpper();

        var loopTo = CANTIDADDECLANES;
        for (i = 1; i <= loopTo; i++)
        {
            YaExisteRet = (guilds[i].GuildName.ToUpper() ?? "") == (GuildName ?? "");
            if (YaExisteRet)
                return YaExisteRet;
        }

        return YaExisteRet;
    }

    public static bool HasFound(ref string UserName)
    {
        bool HasFoundRet = default;
        // ***************************************************
        // Autor: ZaMa
        // Last Modification: 27/11/2009
        // Returns true if it's already the founder of other guild
        // ***************************************************
        int i;
        string name;

        HasFoundRet = false;
        name = UserName.ToUpper();

        var loopTo = (int)CANTIDADDECLANES;
        for (i = 1; i <= loopTo; i++)
        {
            HasFoundRet = (guilds[i].Fundador.ToUpper() ?? "") == (name ?? "");
            if (HasFoundRet)
                return HasFoundRet;
        }

        return HasFoundRet;
    }

    public static bool v_AbrirElecciones(short UserIndex, ref string refError)
    {
        bool v_AbrirEleccionesRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short GuildIndex;

        v_AbrirEleccionesRet = false;
        GuildIndex = Declaraciones.UserList[UserIndex].GuildIndex;

        if ((GuildIndex == 0) | (GuildIndex > CANTIDADDECLANES))
        {
            refError = "Tú no perteneces a ningún clan.";
            return v_AbrirEleccionesRet;
        }

        if (!m_EsGuildLeader(ref Declaraciones.UserList[UserIndex].name, GuildIndex))
        {
            refError = "No eres el líder de tu clan";
            return v_AbrirEleccionesRet;
        }

        if (guilds[GuildIndex].EleccionesAbiertas())
        {
            refError = "Las elecciones ya están abiertas.";
            return v_AbrirEleccionesRet;
        }

        v_AbrirEleccionesRet = true;
        guilds[GuildIndex].AbrirElecciones();
        return v_AbrirEleccionesRet;
    }

    public static bool v_UsuarioVota(short UserIndex, ref string Votado, ref string refError)
    {
        bool v_UsuarioVotaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short GuildIndex;
        string[] list;
        int i;

        v_UsuarioVotaRet = false;
        GuildIndex = Declaraciones.UserList[UserIndex].GuildIndex;

        if ((GuildIndex == 0) | (GuildIndex > CANTIDADDECLANES))
        {
            refError = "Tú no perteneces a ningún clan.";
            return v_UsuarioVotaRet;
        }

        {
            ref var withBlock = ref guilds[GuildIndex];
            if (!withBlock.EleccionesAbiertas())
            {
                refError = "No hay elecciones abiertas en tu clan.";
                return v_UsuarioVotaRet;
            }


            list = withBlock.GetMemberList();
            var loopTo = list.Length - 1;
            for (i = 0; i <= loopTo; i++)
                if ((Votado.ToUpper() ?? "") == (list[i] ?? ""))
                    break;

            if (i > list.Length - 1)
            {
                refError = Votado + " no pertenece al clan.";
                return v_UsuarioVotaRet;
            }


            if (withBlock.YaVoto(ref Declaraciones.UserList[UserIndex].name))
            {
                refError = "Ya has votado, no puedes cambiar tu voto.";
                return v_UsuarioVotaRet;
            }

            withBlock.ContabilizarVoto(ref Declaraciones.UserList[UserIndex].name, ref Votado);
            v_UsuarioVotaRet = true;
        }

        return v_UsuarioVotaRet;
    }

    public static void v_RutinaElecciones()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short i;

        try
        {
            modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                Protocol.PrepareMessageConsoleMsg("Servidor> Revisando elecciones",
                    Protocol.FontTypeNames.FONTTYPE_SERVER));
            var loopTo = CANTIDADDECLANES;
            for (i = 1; i <= loopTo; i++)
                if (guilds[i] is not null)
                    if (guilds[i].RevisarElecciones())
                        modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                            Protocol.PrepareMessageConsoleMsg(
                                "Servidor> " + guilds[i].GetLeader() + " es el nuevo líder de " + guilds[i].GuildName +
                                ".", Protocol.FontTypeNames.FONTTYPE_SERVER));

            modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                Protocol.PrepareMessageConsoleMsg("Servidor> Elecciones revisadas.",
                    Protocol.FontTypeNames.FONTTYPE_SERVER));
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in LoadGuildsDB: " + ex.Message);
            var argdesc = "modGuilds.v_RutinaElecciones():" + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    private static short GetGuildIndexFromChar(ref string PlayerName)
    {
        short GetGuildIndexFromCharRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // aca si que vamos a violar las capas deliveradamente ya que
        // visual basic no permite declarar metodos de clase
        string Temps;
        if (Migration.migr_InStrB(PlayerName, @"\") != 0) PlayerName = PlayerName.Replace(@"\", Constants.vbNullString);
        if (Migration.migr_InStrB(PlayerName, "/") != 0) PlayerName = PlayerName.Replace("/", Constants.vbNullString);
        if (Migration.migr_InStrB(PlayerName, ".") != 0) PlayerName = PlayerName.Replace(".", Constants.vbNullString);
        var argEmptySpaces = 1024;
        Temps = ES.GetVar(Declaraciones.CharPath + PlayerName + ".chr", "GUILD", "GUILDINDEX", ref argEmptySpaces);
        if (Information.IsNumeric(Temps))
            GetGuildIndexFromCharRet = Convert.ToInt16(Temps);
        else
            GetGuildIndexFromCharRet = 0;

        return GetGuildIndexFromCharRet;
    }

    public static short GuildIndex(ref string GuildName)
    {
        short GuildIndexRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // me da el indice del guildname
        short i;

        GuildIndexRet = 0;
        GuildName = GuildName.ToUpper();
        var loopTo = CANTIDADDECLANES;
        for (i = 1; i <= loopTo; i++)
            if ((guilds[i].GuildName.ToUpper() ?? "") == (GuildName ?? ""))
            {
                GuildIndexRet = i;
                return GuildIndexRet;
            }

        return GuildIndexRet;
    }

    public static string m_ListaDeMiembrosOnline(short UserIndex, short GuildIndex)
    {
        string m_ListaDeMiembrosOnlineRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short i;

        m_ListaDeMiembrosOnlineRet = Constants.vbNullString;
        if ((GuildIndex > 0) & (GuildIndex <= CANTIDADDECLANES))
        {
            i = guilds[GuildIndex].m_Iterador_ProximoUserIndex();
            while (i > 0)
            {
                // No mostramos dioses y admins
                if ((i != UserIndex) &
                    (((Declaraciones.UserList[i].flags.Privilegios & (Declaraciones.PlayerType.User |
                                                                      Declaraciones.PlayerType.Consejero |
                                                                      Declaraciones.PlayerType.SemiDios)) != 0) |
                     ((Declaraciones.UserList[UserIndex].flags.Privilegios &
                       (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin)) != 0)))

                    m_ListaDeMiembrosOnlineRet = m_ListaDeMiembrosOnlineRet + Declaraciones.UserList[i].name + ",";
                i = guilds[GuildIndex].m_Iterador_ProximoUserIndex();
            }
        }

        if (m_ListaDeMiembrosOnlineRet.Length > 0)
            m_ListaDeMiembrosOnlineRet = m_ListaDeMiembrosOnlineRet.Substring(0, m_ListaDeMiembrosOnlineRet.Length - 1);

        return m_ListaDeMiembrosOnlineRet;
    }

    public static string[] PrepareGuildsList()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        string[] tStr;
        int i;

        if (CANTIDADDECLANES == 0)
        {
            tStr = new string[1];
        }
        else
        {
            tStr = new string[CANTIDADDECLANES];

            var loopTo = (int)CANTIDADDECLANES;
            for (i = 1; i <= loopTo; i++)
                tStr[i - 1] = guilds[i].GuildName;
        }

        return tStr;
    }

    public static void SendGuildDetails(short UserIndex, ref string GuildName)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        var codex = new string[8];
        short GI;
        int i;

        GI = GuildIndex(ref GuildName);
        if (GI == 0)
            return;

        {
            ref var withBlock = ref guilds[GI];
            for (i = 1; i <= CANTIDADMAXIMACODEX; i++)
                codex[i - 1] = withBlock.GetCodex(Convert.ToByte(i));

            Protocol.WriteGuildDetails(UserIndex, GuildName, withBlock.Fundador, withBlock.GetFechaFundacion(),
                withBlock.GetLeader(), withBlock.GetURL(), withBlock.CantidadDeMiembros, withBlock.EleccionesAbiertas(),
                Alineacion2String(withBlock.Alineacion), withBlock.CantidadEnemys, withBlock.CantidadAllies,
                withBlock.PuntosAntifaccion + "/" + MAXANTIFACCION, codex, withBlock.GetDesc());
        }
    }

    public static void SendGuildLeaderInfo(short UserIndex)
    {
        // ***************************************************
        // Autor: Mariano Barrou (El Oso)
        // Last Modification: 12/10/06
        // Las Modified By: Juan Martín Sotuyo Dodero (Maraxus)
        // ***************************************************
        short GI;
        string[] guildList;
        string[] MemberList;
        string[] aspirantsList;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            GI = withBlock.GuildIndex;

            guildList = PrepareGuildsList();

            if ((GI <= 0) | (GI > CANTIDADDECLANES))
            {
                // Send the guild list instead
                Protocol.WriteGuildList(UserIndex, guildList);
                return;
            }

            MemberList = guilds[GI].GetMemberList();

            if (!m_EsGuildLeader(ref withBlock.name, GI))
            {
                // Send the guild list instead
                Protocol.WriteGuildMemberInfo(UserIndex, guildList, MemberList);
                return;
            }

            aspirantsList = guilds[GI].GetAspirantes();

            Protocol.WriteGuildLeaderInfo(UserIndex, guildList, MemberList, guilds[GI].GetGuildNews(), aspirantsList);
        }
    }


    public static short m_Iterador_ProximoUserIndex(short GuildIndex)
    {
        short m_Iterador_ProximoUserIndexRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // itera sobre los onlinemembers
        m_Iterador_ProximoUserIndexRet = 0;
        if ((GuildIndex > 0) & (GuildIndex <= CANTIDADDECLANES))
            m_Iterador_ProximoUserIndexRet = guilds[GuildIndex].m_Iterador_ProximoUserIndex();

        return m_Iterador_ProximoUserIndexRet;
    }

    public static short Iterador_ProximoGM(short GuildIndex)
    {
        short Iterador_ProximoGMRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // itera sobre los gms escuchando este clan
        Iterador_ProximoGMRet = 0;
        if ((GuildIndex > 0) & (GuildIndex <= CANTIDADDECLANES))
            Iterador_ProximoGMRet = guilds[GuildIndex].Iterador_ProximoGM();

        return Iterador_ProximoGMRet;
    }

    public static short r_Iterador_ProximaPropuesta(short GuildIndex, RELACIONES_GUILD Tipo)
    {
        short r_Iterador_ProximaPropuestaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // itera sobre las propuestas
        r_Iterador_ProximaPropuestaRet = 0;
        if ((GuildIndex > 0) & (GuildIndex <= CANTIDADDECLANES))
            r_Iterador_ProximaPropuestaRet = guilds[GuildIndex].Iterador_ProximaPropuesta(Tipo);

        return r_Iterador_ProximaPropuestaRet;
    }

    public static short GMEscuchaClan(short UserIndex, string GuildName)
    {
        short GMEscuchaClanRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short GI;

        // listen to no guild at all
        if ((Migration.migr_LenB(GuildName) == 0) & (Declaraciones.UserList[UserIndex].EscucheClan != 0))
        {
            // Quit listening to previous guild!!
            Protocol.WriteConsoleMsg(UserIndex,
                "Dejas de escuchar a : " + guilds[Declaraciones.UserList[UserIndex].EscucheClan].GuildName,
                Protocol.FontTypeNames.FONTTYPE_GUILD);
            guilds[Declaraciones.UserList[UserIndex].EscucheClan].DesconectarGM(UserIndex);
            return GMEscuchaClanRet;
        }

        // devuelve el guildindex
        GI = GuildIndex(ref GuildName);
        if (GI > 0)
        {
            if (Declaraciones.UserList[UserIndex].EscucheClan != 0)
            {
                if (Declaraciones.UserList[UserIndex].EscucheClan == GI)
                {
                    // Already listening to them...
                    Protocol.WriteConsoleMsg(UserIndex, "Conectado a : " + GuildName,
                        Protocol.FontTypeNames.FONTTYPE_GUILD);
                    GMEscuchaClanRet = GI;
                    return GMEscuchaClanRet;
                }

                // Quit listening to previous guild!!
                Protocol.WriteConsoleMsg(UserIndex,
                    "Dejas de escuchar a : " + guilds[Declaraciones.UserList[UserIndex].EscucheClan].GuildName,
                    Protocol.FontTypeNames.FONTTYPE_GUILD);
                guilds[Declaraciones.UserList[UserIndex].EscucheClan].DesconectarGM(UserIndex);
            }

            guilds[GI].ConectarGM(UserIndex);
            Protocol.WriteConsoleMsg(UserIndex, "Conectado a : " + GuildName, Protocol.FontTypeNames.FONTTYPE_GUILD);
            GMEscuchaClanRet = GI;
            Declaraciones.UserList[UserIndex].EscucheClan = GI;
        }
        else
        {
            Protocol.WriteConsoleMsg(UserIndex, "Error, el clan no existe.", Protocol.FontTypeNames.FONTTYPE_GUILD);
            GMEscuchaClanRet = 0;
        }

        return GMEscuchaClanRet;
    }

    public static void GMDejaDeEscucharClan(short UserIndex, short GuildIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // el index lo tengo que tener de cuando me puse a escuchar
        Declaraciones.UserList[UserIndex].EscucheClan = 0;
        guilds[GuildIndex].DesconectarGM(UserIndex);
    }

    public static short r_DeclararGuerra(short UserIndex, ref string GuildGuerra, ref string refError)
    {
        short r_DeclararGuerraRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short GI;
        short GIG;

        r_DeclararGuerraRet = 0;
        GI = Declaraciones.UserList[UserIndex].GuildIndex;
        if ((GI <= 0) | (GI > CANTIDADDECLANES))
        {
            refError = "No eres miembro de ningún clan.";
            return r_DeclararGuerraRet;
        }

        if (!m_EsGuildLeader(ref Declaraciones.UserList[UserIndex].name, GI))
        {
            refError = "No eres el líder de tu clan.";
            return r_DeclararGuerraRet;
        }

        if (string.IsNullOrEmpty(Strings.Trim(GuildGuerra)))
        {
            refError = "No has seleccionado ningún clan.";
            return r_DeclararGuerraRet;
        }

        GIG = GuildIndex(ref GuildGuerra);
        if (guilds[GI].GetRelacion(GIG) == RELACIONES_GUILD.GUERRA)
        {
            refError = "Tu clan ya está en guerra con " + GuildGuerra + ".";
            return r_DeclararGuerraRet;
        }

        if (GI == GIG)
        {
            refError = "No puedes declarar la guerra a tu mismo clan.";
            return r_DeclararGuerraRet;
        }

        if ((GIG < 1) | (GIG > CANTIDADDECLANES))
        {
            var argdesc = "ModGuilds.r_DeclararGuerra: " + GI + " declara a " + GuildGuerra;
            General.LogError(ref argdesc);
            refError = "Inconsistencia en el sistema de clanes. Avise a un administrador (GIG fuera de rango)";
            return r_DeclararGuerraRet;
        }

        guilds[GI].AnularPropuestas(GIG);
        guilds[GIG].AnularPropuestas(GI);
        guilds[GI].SetRelacion(GIG, RELACIONES_GUILD.GUERRA);
        guilds[GIG].SetRelacion(GI, RELACIONES_GUILD.GUERRA);

        r_DeclararGuerraRet = GIG;
        return r_DeclararGuerraRet;
    }


    public static short r_AceptarPropuestaDePaz(short UserIndex, ref string GuildPaz, ref string refError)
    {
        short r_AceptarPropuestaDePazRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // el clan de userindex acepta la propuesta de paz de guildpaz, con quien esta en guerra
        short GI;
        short GIG;

        r_AceptarPropuestaDePazRet = 0;
        GI = Declaraciones.UserList[UserIndex].GuildIndex;
        if ((GI <= 0) | (GI > CANTIDADDECLANES))
        {
            refError = "No eres miembro de ningún clan.";
            return r_AceptarPropuestaDePazRet;
        }

        if (!m_EsGuildLeader(ref Declaraciones.UserList[UserIndex].name, GI))
        {
            refError = "No eres el líder de tu clan.";
            return r_AceptarPropuestaDePazRet;
        }

        if (string.IsNullOrEmpty(Strings.Trim(GuildPaz)))
        {
            refError = "No has seleccionado ningún clan.";
            return r_AceptarPropuestaDePazRet;
        }

        GIG = GuildIndex(ref GuildPaz);

        if ((GIG < 1) | (GIG > CANTIDADDECLANES))
        {
            var argdesc = "ModGuilds.r_AceptarPropuestaDePaz: " + GI + " acepta de " + GuildPaz;
            General.LogError(ref argdesc);
            refError = "Inconsistencia en el sistema de clanes. Avise a un administrador (GIG fuera de rango).";
            return r_AceptarPropuestaDePazRet;
        }

        if (guilds[GI].GetRelacion(GIG) != RELACIONES_GUILD.GUERRA)
        {
            refError = "No estás en guerra con ese clan.";
            return r_AceptarPropuestaDePazRet;
        }

        var argTipo = RELACIONES_GUILD.PAZ;
        if (!guilds[GI].HayPropuesta(GIG, ref argTipo))
        {
            refError = "No hay ninguna propuesta de paz para aceptar.";
            return r_AceptarPropuestaDePazRet;
        }

        guilds[GI].AnularPropuestas(GIG);
        guilds[GIG].AnularPropuestas(GI);
        guilds[GI].SetRelacion(GIG, RELACIONES_GUILD.PAZ);
        guilds[GIG].SetRelacion(GI, RELACIONES_GUILD.PAZ);

        r_AceptarPropuestaDePazRet = GIG;
        return r_AceptarPropuestaDePazRet;
    }

    public static short r_RechazarPropuestaDeAlianza(short UserIndex, ref string GuildPro, ref string refError)
    {
        short r_RechazarPropuestaDeAlianzaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // devuelve el index al clan guildPro
        short GI;
        short GIG;

        r_RechazarPropuestaDeAlianzaRet = 0;
        GI = Declaraciones.UserList[UserIndex].GuildIndex;

        if ((GI <= 0) | (GI > CANTIDADDECLANES))
        {
            refError = "No eres miembro de ningún clan.";
            return r_RechazarPropuestaDeAlianzaRet;
        }

        if (!m_EsGuildLeader(ref Declaraciones.UserList[UserIndex].name, GI))
        {
            refError = "No eres el líder de tu clan.";
            return r_RechazarPropuestaDeAlianzaRet;
        }

        if (string.IsNullOrEmpty(Strings.Trim(GuildPro)))
        {
            refError = "No has seleccionado ningún clan.";
            return r_RechazarPropuestaDeAlianzaRet;
        }

        GIG = GuildIndex(ref GuildPro);

        if ((GIG < 1) | (GIG > CANTIDADDECLANES))
        {
            var argdesc = "ModGuilds.r_RechazarPropuestaDeAlianza: " + GI + " acepta de " + GuildPro;
            General.LogError(ref argdesc);
            refError = "Inconsistencia en el sistema de clanes. Avise a un administrador (GIG fuera de rango).";
            return r_RechazarPropuestaDeAlianzaRet;
        }

        var argTipo = RELACIONES_GUILD.ALIADOS;
        if (!guilds[GI].HayPropuesta(GIG, ref argTipo))
        {
            refError = "No hay propuesta de alianza del clan " + GuildPro;
            return r_RechazarPropuestaDeAlianzaRet;
        }

        guilds[GI].AnularPropuestas(GIG);
        // avisamos al otro clan
        var argNews = guilds[GI].GuildName + " ha rechazado nuestra propuesta de alianza. " +
                      guilds[GIG].GetGuildNews();
        guilds[GIG].SetGuildNews(ref argNews);
        r_RechazarPropuestaDeAlianzaRet = GIG;
        return r_RechazarPropuestaDeAlianzaRet;
    }


    public static short r_RechazarPropuestaDePaz(short UserIndex, ref string GuildPro, ref string refError)
    {
        short r_RechazarPropuestaDePazRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // devuelve el index al clan guildPro
        short GI;
        short GIG;

        r_RechazarPropuestaDePazRet = 0;
        GI = Declaraciones.UserList[UserIndex].GuildIndex;

        if ((GI <= 0) | (GI > CANTIDADDECLANES))
        {
            refError = "No eres miembro de ningún clan.";
            return r_RechazarPropuestaDePazRet;
        }

        if (!m_EsGuildLeader(ref Declaraciones.UserList[UserIndex].name, GI))
        {
            refError = "No eres el líder de tu clan.";
            return r_RechazarPropuestaDePazRet;
        }

        if (string.IsNullOrEmpty(Strings.Trim(GuildPro)))
        {
            refError = "No has seleccionado ningún clan.";
            return r_RechazarPropuestaDePazRet;
        }

        GIG = GuildIndex(ref GuildPro);

        if ((GIG < 1) | (GIG > CANTIDADDECLANES))
        {
            var argdesc = "ModGuilds.r_RechazarPropuestaDePaz: " + GI + " acepta de " + GuildPro;
            General.LogError(ref argdesc);
            refError = "Inconsistencia en el sistema de clanes. Avise a un administrador (GIG fuera de rango).";
            return r_RechazarPropuestaDePazRet;
        }

        var argTipo = RELACIONES_GUILD.PAZ;
        if (!guilds[GI].HayPropuesta(GIG, ref argTipo))
        {
            refError = "No hay propuesta de paz del clan " + GuildPro;
            return r_RechazarPropuestaDePazRet;
        }

        guilds[GI].AnularPropuestas(GIG);
        // avisamos al otro clan
        var argNews = guilds[GI].GuildName + " ha rechazado nuestra propuesta de paz. " + guilds[GIG].GetGuildNews();
        guilds[GIG].SetGuildNews(ref argNews);
        r_RechazarPropuestaDePazRet = GIG;
        return r_RechazarPropuestaDePazRet;
    }

    public static short r_AceptarPropuestaDeAlianza(short UserIndex, ref string GuildAllie, ref string refError)
    {
        short r_AceptarPropuestaDeAlianzaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // el clan de userindex acepta la propuesta de paz de guildpaz, con quien esta en guerra
        short GI;
        short GIG;

        r_AceptarPropuestaDeAlianzaRet = 0;
        GI = Declaraciones.UserList[UserIndex].GuildIndex;
        if ((GI <= 0) | (GI > CANTIDADDECLANES))
        {
            refError = "No eres miembro de ningún clan.";
            return r_AceptarPropuestaDeAlianzaRet;
        }

        if (!m_EsGuildLeader(ref Declaraciones.UserList[UserIndex].name, GI))
        {
            refError = "No eres el líder de tu clan.";
            return r_AceptarPropuestaDeAlianzaRet;
        }

        if (string.IsNullOrEmpty(Strings.Trim(GuildAllie)))
        {
            refError = "No has seleccionado ningún clan.";
            return r_AceptarPropuestaDeAlianzaRet;
        }

        GIG = GuildIndex(ref GuildAllie);

        if ((GIG < 1) | (GIG > CANTIDADDECLANES))
        {
            var argdesc = "ModGuilds.r_AceptarPropuestaDeAlianza: " + GI + " acepta de " + GuildAllie;
            General.LogError(ref argdesc);
            refError = "Inconsistencia en el sistema de clanes. Avise a un administrador (GIG fuera de rango).";
            return r_AceptarPropuestaDeAlianzaRet;
        }

        if (guilds[GI].GetRelacion(GIG) != RELACIONES_GUILD.PAZ)
        {
            refError =
                "No estás en paz con el clan, solo puedes aceptar propuesas de alianzas con alguien que estes en paz.";
            return r_AceptarPropuestaDeAlianzaRet;
        }

        var argTipo = RELACIONES_GUILD.ALIADOS;
        if (!guilds[GI].HayPropuesta(GIG, ref argTipo))
        {
            refError = "No hay ninguna propuesta de alianza para aceptar.";
            return r_AceptarPropuestaDeAlianzaRet;
        }

        guilds[GI].AnularPropuestas(GIG);
        guilds[GIG].AnularPropuestas(GI);
        guilds[GI].SetRelacion(GIG, RELACIONES_GUILD.ALIADOS);
        guilds[GIG].SetRelacion(GI, RELACIONES_GUILD.ALIADOS);

        r_AceptarPropuestaDeAlianzaRet = GIG;
        return r_AceptarPropuestaDeAlianzaRet;
    }


    public static bool r_ClanGeneraPropuesta(short UserIndex, ref string OtroClan, RELACIONES_GUILD Tipo,
        ref string Detalle, ref string refError)
    {
        bool r_ClanGeneraPropuestaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short OtroClanGI;
        short GI;

        r_ClanGeneraPropuestaRet = false;

        GI = Declaraciones.UserList[UserIndex].GuildIndex;
        if ((GI <= 0) | (GI > CANTIDADDECLANES))
        {
            refError = "No eres miembro de ningún clan.";
            return r_ClanGeneraPropuestaRet;
        }

        OtroClanGI = GuildIndex(ref OtroClan);

        if (OtroClanGI == GI)
        {
            refError = "No puedes declarar relaciones con tu propio clan.";
            return r_ClanGeneraPropuestaRet;
        }

        if ((OtroClanGI <= 0) | (OtroClanGI > CANTIDADDECLANES))
        {
            refError = "El sistema de clanes esta inconsistente, el otro clan no existe.";
            return r_ClanGeneraPropuestaRet;
        }

        if (guilds[OtroClanGI].HayPropuesta(GI, ref Tipo))
        {
            refError = "Ya hay propuesta de " + Relacion2String(Tipo) + " con " + OtroClan;
            return r_ClanGeneraPropuestaRet;
        }

        if (!m_EsGuildLeader(ref Declaraciones.UserList[UserIndex].name, GI))
        {
            refError = "No eres el líder de tu clan.";
            return r_ClanGeneraPropuestaRet;
        }

        // de acuerdo al tipo procedemos validando las transiciones
        if (Tipo == RELACIONES_GUILD.PAZ)
        {
            if (guilds[GI].GetRelacion(OtroClanGI) != RELACIONES_GUILD.GUERRA)
            {
                refError = "No estás en guerra con " + OtroClan;
                return r_ClanGeneraPropuestaRet;
            }
        }
        else if (Tipo == RELACIONES_GUILD.GUERRA)
        {
        }
        // por ahora no hay propuestas de guerra
        else if (Tipo == RELACIONES_GUILD.ALIADOS)
        {
            if (guilds[GI].GetRelacion(OtroClanGI) != RELACIONES_GUILD.PAZ)
            {
                refError = "Para solicitar alianza no debes estar ni aliado ni en guerra con " + OtroClan;
                return r_ClanGeneraPropuestaRet;
            }
        }

        guilds[OtroClanGI].SetPropuesta(Tipo, GI, ref Detalle);
        r_ClanGeneraPropuestaRet = true;
        return r_ClanGeneraPropuestaRet;
    }

    public static string r_VerPropuesta(short UserIndex, ref string OtroGuild, RELACIONES_GUILD Tipo,
        ref string refError)
    {
        string r_VerPropuestaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short OtroClanGI;
        short GI;

        r_VerPropuestaRet = Constants.vbNullString;
        refError = Constants.vbNullString;

        GI = Declaraciones.UserList[UserIndex].GuildIndex;
        if ((GI <= 0) | (GI > CANTIDADDECLANES))
        {
            refError = "No eres miembro de ningún clan.";
            return r_VerPropuestaRet;
        }

        if (!m_EsGuildLeader(ref Declaraciones.UserList[UserIndex].name, GI))
        {
            refError = "No eres el líder de tu clan.";
            return r_VerPropuestaRet;
        }

        OtroClanGI = GuildIndex(ref OtroGuild);

        if (!guilds[GI].HayPropuesta(OtroClanGI, ref Tipo))
        {
            refError = "No existe la propuesta solicitada.";
            return r_VerPropuestaRet;
        }

        r_VerPropuestaRet = guilds[GI].GetPropuesta(OtroClanGI, ref Tipo);
        return r_VerPropuestaRet;
    }

    public static string[] r_ListaDePropuestas(short UserIndex, RELACIONES_GUILD Tipo)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short GI;
        short i;
        short proposalCount;
        string[] proposals;

        GI = Declaraciones.UserList[UserIndex].GuildIndex;

        if ((GI > 0) & (GI <= CANTIDADDECLANES))
        {
            ref var withBlock = ref guilds[GI];
            proposalCount = withBlock.get_CantidadPropuestas(Tipo);

            // Resize array to contain all proposals
            if (proposalCount > 0)
                proposals = new string[proposalCount];
            else
                proposals = new string[1];

            // Store each guild name
            var loopTo = Convert.ToInt16(proposalCount - 1);
            for (i = 0; i <= loopTo; i++)
                proposals[i] = guilds[withBlock.Iterador_ProximaPropuesta(Tipo)].GuildName;
        }
        else
        {
            proposals = new string[1];
        }

        return proposals;
    }

    public static void a_RechazarAspiranteChar(ref string Aspirante, short guild, ref string Detalles)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if (Migration.migr_InStrB(Aspirante, @"\") != 0) Aspirante = Aspirante.Replace(@"\", "");
        if (Migration.migr_InStrB(Aspirante, "/") != 0) Aspirante = Aspirante.Replace("/", "");
        if (Migration.migr_InStrB(Aspirante, ".") != 0) Aspirante = Aspirante.Replace(".", "");
        guilds[guild].InformarRechazoEnChar(ref Aspirante, ref Detalles);
    }

    public static string a_ObtenerRechazoDeChar(ref string Aspirante)
    {
        string a_ObtenerRechazoDeCharRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if (Migration.migr_InStrB(Aspirante, @"\") != 0) Aspirante = Aspirante.Replace(@"\", "");
        if (Migration.migr_InStrB(Aspirante, "/") != 0) Aspirante = Aspirante.Replace("/", "");
        if (Migration.migr_InStrB(Aspirante, ".") != 0) Aspirante = Aspirante.Replace(".", "");
        var argEmptySpaces = 1024;
        a_ObtenerRechazoDeCharRet = ES.GetVar(Declaraciones.CharPath + Aspirante + ".chr", "GUILD", "MotivoRechazo",
            ref argEmptySpaces);
        ES.WriteVar(Declaraciones.CharPath + Aspirante + ".chr", "GUILD", "MotivoRechazo", Constants.vbNullString);
        return a_ObtenerRechazoDeCharRet;
    }

    public static bool a_RechazarAspirante(short UserIndex, ref string Nombre, ref string refError)
    {
        bool a_RechazarAspiranteRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short GI;
        short NroAspirante;

        a_RechazarAspiranteRet = false;
        GI = Declaraciones.UserList[UserIndex].GuildIndex;
        if ((GI <= 0) | (GI > CANTIDADDECLANES))
        {
            refError = "No perteneces a ningún clan";
            return a_RechazarAspiranteRet;
        }

        NroAspirante = guilds[GI].NumeroDeAspirante(ref Nombre);

        if (NroAspirante == 0)
        {
            refError = Nombre + " no es aspirante a tu clan.";
            return a_RechazarAspiranteRet;
        }

        guilds[GI].RetirarAspirante(ref Nombre, ref NroAspirante);
        refError = "Fue rechazada tu solicitud de ingreso a " + guilds[GI].GuildName;
        a_RechazarAspiranteRet = true;
        return a_RechazarAspiranteRet;
    }

    public static string a_DetallesAspirante(short UserIndex, ref string Nombre)
    {
        string a_DetallesAspiranteRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short GI;
        short NroAspirante;

        a_DetallesAspiranteRet = Constants.vbNullString;
        GI = Declaraciones.UserList[UserIndex].GuildIndex;
        if ((GI <= 0) | (GI > CANTIDADDECLANES)) return a_DetallesAspiranteRet;

        if (!m_EsGuildLeader(ref Declaraciones.UserList[UserIndex].name, GI)) return a_DetallesAspiranteRet;

        NroAspirante = guilds[GI].NumeroDeAspirante(ref Nombre);
        if (NroAspirante > 0) a_DetallesAspiranteRet = guilds[GI].DetallesSolicitudAspirante(NroAspirante);

        return a_DetallesAspiranteRet;
    }

    public static void SendDetallesPersonaje(short UserIndex, string Personaje)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short GI;
        short NroAsp;
        string GuildName;
        clsIniReader UserFile;
        string Miembro;
        short GuildActual;
        string[] list;
        int i;

        try
        {
            GI = Declaraciones.UserList[UserIndex].GuildIndex;

            Personaje = Personaje.ToUpper();

            if ((GI <= 0) | (GI > CANTIDADDECLANES))
            {
                Protocol.WriteConsoleMsg(UserIndex, "No perteneces a ningún clan.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if (!m_EsGuildLeader(ref Declaraciones.UserList[UserIndex].name, GI))
            {
                Protocol.WriteConsoleMsg(UserIndex, "No eres el líder de tu clan.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if (Migration.migr_InStrB(Personaje, @"\") != 0)
                Personaje = Personaje.Replace(@"\", Constants.vbNullString);
            if (Migration.migr_InStrB(Personaje, "/") != 0) Personaje = Personaje.Replace("/", Constants.vbNullString);
            if (Migration.migr_InStrB(Personaje, ".") != 0) Personaje = Personaje.Replace(".", Constants.vbNullString);

            NroAsp = guilds[GI].NumeroDeAspirante(ref Personaje);

            if (NroAsp == 0)
            {
                list = guilds[GI].GetMemberList();

                var loopTo = list.Length - 1;
                for (i = 0; i <= loopTo; i++)
                    if ((Personaje ?? "") == (list[i] ?? ""))
                        break;

                if (i > list.Length - 1)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "El personaje no es ni aspirante ni miembro del clan.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return;
                }
            }

            // ahora traemos la info

            UserFile = new clsIniReader();

            {
                ref var withBlock = ref UserFile;
                withBlock.Initialize(Declaraciones.CharPath + Personaje + ".chr");

                // Get the character's current guild
                GuildActual = Convert.ToInt16(Migration.ParseVal(withBlock.GetValue("GUILD", "GuildIndex")));
                if ((GuildActual > 0) & (GuildActual <= CANTIDADDECLANES))
                    GuildName = "<" + guilds[GuildActual].GuildName + ">";
                else
                    GuildName = "Ninguno";

                // Get previous guilds
                Miembro = withBlock.GetValue("GUILD", "Miembro");
                if (Miembro.Length > 400) Miembro = ".." + Miembro.Substring(Math.Max(0, Miembro.Length - 400));

                Protocol.WriteCharacterInfo(UserIndex, Personaje,
                    (Declaraciones.eRaza)Convert.ToInt16(withBlock.GetValue("INIT", "Raza")),
                    (Declaraciones.eClass)Convert.ToInt16(withBlock.GetValue("INIT", "Clase")),
                    (Declaraciones.eGenero)Convert.ToInt16(withBlock.GetValue("INIT", "Genero")),
                    Convert.ToByte(withBlock.GetValue("STATS", "ELV")),
                    Convert.ToInt32(withBlock.GetValue("STATS", "GLD")),
                    Convert.ToInt32(withBlock.GetValue("STATS", "Banco")),
                    Convert.ToInt32(withBlock.GetValue("REP", "Promedio")), withBlock.GetValue("GUILD", "Pedidos"),
                    GuildName, Miembro, Convert.ToBoolean(withBlock.GetValue("FACCIONES", "EjercitoReal")),
                    Convert.ToBoolean(withBlock.GetValue("FACCIONES", "EjercitoCaos")),
                    Convert.ToInt32(withBlock.GetValue("FACCIONES", "CiudMatados")),
                    Convert.ToInt32(withBlock.GetValue("FACCIONES", "CrimMatados")));
            }

            // UPGRADE_NOTE: El objeto UserFile no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            UserFile = null;
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in GetGuildIndexFromChar: " + ex.Message);
            // UPGRADE_NOTE: El objeto UserFile no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            UserFile = null;
            if (!File.Exists(Declaraciones.CharPath + Personaje + ".chr"))
            {
                var argdesc = "El usuario " + Declaraciones.UserList[UserIndex].name + " (" + UserIndex +
                              " ) ha pedido los detalles del personaje " + Personaje + " que no se encuentra.";
                General.LogError(ref argdesc);
            }
            else
            {
                var argdesc1 = "[" + ex.GetType().Name + "] " + ex.Message +
                               " En la rutina SendDetallesPersonaje, por el usuario " +
                               Declaraciones.UserList[UserIndex].name + " (" + UserIndex +
                               " ), pidiendo información sobre el personaje " + Personaje;
                General.LogError(ref argdesc1);
            }
        }
    }

    public static bool a_NuevoAspirante(short UserIndex, ref string clan, ref string Solicitud, ref string refError)
    {
        bool a_NuevoAspiranteRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        string ViejoSolicitado;
        short ViejoGuildINdex;
        short ViejoNroAspirante;
        short NuevoGuildIndex;

        a_NuevoAspiranteRet = false;

        if (Declaraciones.UserList[UserIndex].GuildIndex > 0)
        {
            refError = "Ya perteneces a un clan, debes salir del mismo antes de solicitar ingresar a otro.";
            return a_NuevoAspiranteRet;
        }

        if (Extra.EsNewbie(UserIndex))
        {
            refError = "Los newbies no tienen derecho a entrar a un clan.";
            return a_NuevoAspiranteRet;
        }

        NuevoGuildIndex = GuildIndex(ref clan);
        if (NuevoGuildIndex == 0)
        {
            refError = "Ese clan no existe, avise a un administrador.";
            return a_NuevoAspiranteRet;
        }

        if (!m_EstadoPermiteEntrar(UserIndex, NuevoGuildIndex))
        {
            refError = "Tú no puedes entrar a un clan de alineación " +
                       Alineacion2String(guilds[NuevoGuildIndex].Alineacion);
            return a_NuevoAspiranteRet;
        }

        if (guilds[NuevoGuildIndex].CantidadAspirantes() >= MAXASPIRANTES)
        {
            refError =
                "El clan tiene demasiados aspirantes. Contáctate con un miembro para que procese las solicitudes.";
            return a_NuevoAspiranteRet;
        }

        var argEmptySpaces = 1024;
        ViejoSolicitado = ES.GetVar(Declaraciones.CharPath + Declaraciones.UserList[UserIndex].name + ".chr", "GUILD",
            "ASPIRANTEA", ref argEmptySpaces);

        if (Migration.migr_LenB(ViejoSolicitado) != 0)
        {
            // borramos la vieja solicitud
            ViejoGuildINdex = Convert.ToInt16(ViejoSolicitado);
            if (ViejoGuildINdex != 0)
            {
                ViejoNroAspirante =
                    guilds[ViejoGuildINdex].NumeroDeAspirante(ref Declaraciones.UserList[UserIndex].name);
                if (ViejoNroAspirante > 0)
                    guilds[ViejoGuildINdex]
                        .RetirarAspirante(ref Declaraciones.UserList[UserIndex].name, ref ViejoNroAspirante);
            }
            // RefError = "Inconsistencia en los clanes, avise a un administrador"
            // Exit Function
        }

        guilds[NuevoGuildIndex].NuevoAspirante(ref Declaraciones.UserList[UserIndex].name, ref Solicitud);
        a_NuevoAspiranteRet = true;
        return a_NuevoAspiranteRet;
    }

    public static bool a_AceptarAspirante(short UserIndex, ref string Aspirante, ref string refError)
    {
        bool a_AceptarAspiranteRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short GI;
        short NroAspirante;
        short AspiranteUI;

        // un pj ingresa al clan :D

        a_AceptarAspiranteRet = false;

        GI = Declaraciones.UserList[UserIndex].GuildIndex;
        if ((GI <= 0) | (GI > CANTIDADDECLANES))
        {
            refError = "No perteneces a ningún clan";
            return a_AceptarAspiranteRet;
        }

        if (!m_EsGuildLeader(ref Declaraciones.UserList[UserIndex].name, GI))
        {
            refError = "No eres el líder de tu clan";
            return a_AceptarAspiranteRet;
        }

        NroAspirante = guilds[GI].NumeroDeAspirante(ref Aspirante);

        if (NroAspirante == 0)
        {
            refError = "El Pj no es aspirante al clan.";
            return a_AceptarAspiranteRet;
        }

        AspiranteUI = Extra.NameIndex(Aspirante);
        if (AspiranteUI > 0)
        {
            // pj Online
            if (!m_EstadoPermiteEntrar(AspiranteUI, GI))
            {
                refError = Aspirante + " no puede entrar a un clan de alineación " +
                           Alineacion2String(guilds[GI].Alineacion);
                guilds[GI].RetirarAspirante(ref Aspirante, ref NroAspirante);
                return a_AceptarAspiranteRet;
            }

            if (!(Declaraciones.UserList[AspiranteUI].GuildIndex == 0))
            {
                refError = Aspirante + " ya es parte de otro clan.";
                guilds[GI].RetirarAspirante(ref Aspirante, ref NroAspirante);
                return a_AceptarAspiranteRet;
            }
        }
        else if (!m_EstadoPermiteEntrarChar(ref Aspirante, GI))
        {
            refError = Aspirante + " no puede entrar a un clan de alineación " +
                       Alineacion2String(guilds[GI].Alineacion);
            guilds[GI].RetirarAspirante(ref Aspirante, ref NroAspirante);
            return a_AceptarAspiranteRet;
        }
        else if (GetGuildIndexFromChar(ref Aspirante) != 0)
        {
            refError = Aspirante + " ya es parte de otro clan.";
            guilds[GI].RetirarAspirante(ref Aspirante, ref NroAspirante);
            return a_AceptarAspiranteRet;
        }
        // el pj es aspirante al clan y puede entrar

        guilds[GI].RetirarAspirante(ref Aspirante, ref NroAspirante);
        guilds[GI].AceptarNuevoMiembro(ref Aspirante);

        // If player is online, update tag
        if (AspiranteUI > 0) UsUaRiOs.RefreshCharStatus(AspiranteUI);

        a_AceptarAspiranteRet = true;
        return a_AceptarAspiranteRet;
    }

    public static string GuildName(short GuildIndex)
    {
        string GuildNameRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        GuildNameRet = Constants.vbNullString;
        if ((GuildIndex <= 0) | (GuildIndex > CANTIDADDECLANES))
            return GuildNameRet;

        GuildNameRet = guilds[GuildIndex].GuildName;
        return GuildNameRet;
    }

    public static string GuildLeader(short GuildIndex)
    {
        string GuildLeaderRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        GuildLeaderRet = Constants.vbNullString;
        if ((GuildIndex <= 0) | (GuildIndex > CANTIDADDECLANES))
            return GuildLeaderRet;

        GuildLeaderRet = guilds[GuildIndex].GetLeader();
        return GuildLeaderRet;
    }

    public static string GuildAlignment(short GuildIndex)
    {
        string GuildAlignmentRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        GuildAlignmentRet = Constants.vbNullString;
        if ((GuildIndex <= 0) | (GuildIndex > CANTIDADDECLANES))
            return GuildAlignmentRet;

        GuildAlignmentRet = Alineacion2String(guilds[GuildIndex].Alineacion);
        return GuildAlignmentRet;
    }

    public static string GuildFounder(short GuildIndex)
    {
        string GuildFounderRet = default;
        // ***************************************************
        // Autor: ZaMa
        // Returns the guild founder's name
        // Last Modification: 25/03/2009
        // ***************************************************
        GuildFounderRet = Constants.vbNullString;
        if ((GuildIndex <= 0) | (GuildIndex > CANTIDADDECLANES))
            return GuildFounderRet;

        GuildFounderRet = guilds[GuildIndex].Fundador;
        return GuildFounderRet;
    }
}