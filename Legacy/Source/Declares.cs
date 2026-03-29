using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;

namespace Legacy;

internal static class Declaraciones
{
    public enum e_ObjetosCriticos
    {
        Manzana = 1,
        Manzana2 = 2,
        ManzanaNewbie = 467
    }

    public enum eAtributos
    {
        Fuerza = 1,
        Agilidad = 2,
        Inteligencia = 3,
        Carisma = 4,
        Constitucion = 5
    }

    public enum eCiudad
    {
        cUllathorpe = 1,
        cNix,
        cBanderbill,
        cLindos,
        cArghal
    }

    public enum eClanType
    {
        ct_RoyalArmy,
        ct_Evil,
        ct_Neutral,
        ct_GM,
        ct_Legal,
        ct_Criminal
    }

    public enum eClass
    {
        Mage = 1, // Mago
        Cleric, // Clérigo
        Warrior, // Guerrero
        Assasin, // Asesino
        Thief, // Ladrón
        Bard, // Bardo
        Druid, // Druida
        Bandit, // Bandido
        Paladin, // Paladín
        Hunter, // Cazador
        Worker, // Trabajador
        Pirat // Pirata
    }

    // *******
    // FOROS *
    // *******

    // Tipos de mensajes
    public enum eForumMsgType : byte
    {
        ieGeneral,
        ieGENERAL_STICKY,
        ieREAL,
        ieREAL_STICKY,
        ieCAOS,
        ieCAOS_STICKY
    }

    // Indica el tipo de foro
    public enum eForumType : byte
    {
        ieGeneral,
        ieREAL,
        ieCAOS
    }

    // Indica los privilegios para visualizar los diferentes foros
    public enum eForumVisibility
    {
        ieGENERAL_MEMBER = 0x1,
        ieREAL_MEMBER = 0x2,
        ieCAOS_MEMBER = 0x4
    }

    public enum eGenero : byte
    {
        Hombre = 1,
        Mujer
    }

    public enum eGMCommands : byte
    {
        GMMessage = 1, // /GMSG
        showName, // /SHOWNAME
        OnlineRoyalArmy, // /ONLINEREAL
        OnlineChaosLegion, // /ONLINECAOS
        GoNearby, // /IRCERCA
        comment, // /REM
        serverTime, // /HORA
        Where, // /DONDE
        CreaturesInMap, // /NENE
        WarpMeToTarget, // /TELEPLOC
        WarpChar, // /TELEP
        Silence, // /SILENCIAR
        SOSShowList, // /SHOW SOS
        SOSRemove, // SOSDONE
        GoToChar, // /IRA
        invisible, // /INVISIBLE
        GMPanel, // /PANELGM
        RequestUserList, // LISTUSU
        Working, // /TRABAJANDO
        Hiding, // /OCULTANDO
        Jail, // /CARCEL
        KillNPC, // /RMATA
        WarnUser, // /ADVERTENCIA
        EditChar, // /MOD
        RequestCharInfo, // /INFO
        RequestCharStats, // /STAT
        RequestCharGold, // /BAL
        RequestCharInventory, // /INV
        RequestCharBank, // /BOV
        RequestCharSkills, // /SKILLS
        ReviveChar, // /REVIVIR
        OnlineGM, // /ONLINEGM
        OnlineMap, // /ONLINEMAP
        Forgive, // /PERDON
        Kick, // /ECHAR
        Execute, // /EJECUTAR
        BanChar, // /BAN
        UnbanChar, // /UNBAN
        NPCFollow, // /SEGUIR
        SummonChar, // /SUM
        SpawnListRequest, // /CC
        SpawnCreature, // SPA
        ResetNPCInventory, // /RESETINV
        CleanWorld, // /LIMPIAR
        ServerMessage, // /RMSG
        NickToIP, // /NICK2IP
        IPToNick, // /IP2NICK
        GuildOnlineMembers, // /ONCLAN
        TeleportCreate, // /CT
        TeleportDestroy, // /DT
        RainToggle, // /LLUVIA
        SetCharDescription, // /SETDESC
        ForceMIDIToMap, // /FORCEMIDIMAP
        ForceWAVEToMap, // /FORCEWAVMAP
        RoyalArmyMessage, // /REALMSG
        ChaosLegionMessage, // /CAOSMSG
        CitizenMessage, // /CIUMSG
        CriminalMessage, // /CRIMSG
        TalkAsNPC, // /TALKAS
        DestroyAllItemsInArea, // /MASSDEST
        AcceptRoyalCouncilMember, // /ACEPTCONSE
        AcceptChaosCouncilMember, // /ACEPTCONSECAOS
        ItemsInTheFloor, // /PISO
        MakeDumb, // /ESTUPIDO
        MakeDumbNoMore, // /NOESTUPIDO
        DumpIPTables, // /DUMPSECURITY
        CouncilKick, // /KICKCONSE
        SetTrigger, // /TRIGGER
        AskTrigger, // /TRIGGER with no args
        BannedIPList, // /BANIPLIST
        BannedIPReload, // /BANIPRELOAD
        GuildMemberList, // /MIEMBROSCLAN
        GuildBan, // /BANCLAN
        BanIP, // /BANIP
        UnbanIP, // /UNBANIP
        CreateItem, // /CI
        DestroyItems, // /DEST
        ChaosLegionKick, // /NOCAOS
        RoyalArmyKick, // /NOREAL
        ForceMIDIAll, // /FORCEMIDI
        ForceWAVEAll, // /FORCEWAV
        RemovePunishment, // /BORRARPENA
        TileBlockedToggle, // /BLOQ
        KillNPCNoRespawn, // /MATA
        KillAllNearbyNPCs, // /MASSKILL
        LastIP, // /LASTIP
        ChangeMOTD, // /MOTDCAMBIA
        SetMOTD, // ZMOTD
        SystemMessage, // /SMSG
        CreateNPC, // /ACC
        CreateNPCWithRespawn, // /RACC
        ImperialArmour, // /AI1 - 4
        ChaosArmour, // /AC1 - 4
        NavigateToggle, // /NAVE
        ServerOpenToUsersToggle, // /HABILITAR
        TurnOffServer, // /APAGAR
        TurnCriminal, // /CONDEN
        ResetFactions, // /RAJAR
        RemoveCharFromGuild, // /RAJARCLAN
        RequestCharMail, // /LASTEMAIL
        AlterPassword, // /APASS
        AlterMail, // /AEMAIL
        AlterName, // /ANAME
        ToggleCentinelActivated, // /CENTINELAACTIVADO
        DoBackUp, // /DOBACKUP
        ShowGuildMessages, // /SHOWCMSG
        SaveMap, // /GUARDAMAPA
        ChangeMapInfoPK, // /MODMAPINFO PK
        ChangeMapInfoBackup, // /MODMAPINFO BACKUP
        ChangeMapInfoRestricted, // /MODMAPINFO RESTRINGIR
        ChangeMapInfoNoMagic, // /MODMAPINFO MAGIASINEFECTO
        ChangeMapInfoNoInvi, // /MODMAPINFO INVISINEFECTO
        ChangeMapInfoNoResu, // /MODMAPINFO RESUSINEFECTO
        ChangeMapInfoLand, // /MODMAPINFO TERRENO
        ChangeMapInfoZone, // /MODMAPINFO ZONA
        SaveChars, // /GRABAR
        CleanSOS, // /BORRAR SOS
        ShowServerForm, // /SHOW INT
        night, // /NOCHE
        KickAllChars, // /ECHARTODOSPJS
        ReloadNPCs, // /RELOADNPCS
        ReloadServerIni, // /RELOADSINI
        ReloadSpells, // /RELOADHECHIZOS
        ReloadObjects, // /RELOADOBJ
        Restart, // /REINICIAR
        ResetAutoUpdate, // /AUTOUPDATE
        ChatColor, // /CHATCOLOR
        Ignored, // /IGNORADO
        CheckSlot, // /SLOT
        SetIniVar // /SETINIVAR LLAVE CLAVE VALOR
    }


    // '
    // Direccion
    // 
    // @param NORTH Norte
    // @param EAST Este
    // @param SOUTH Sur
    // @param WEST Oeste
    // 
    public enum eHeading : byte
    {
        NORTH = 1,
        EAST = 2,
        SOUTH = 3,
        WEST = 4
    }

    public enum eMessages : short
    {
        DontSeeAnything,
        NPCSwing,
        NPCKillUser,
        BlockedWithShieldUser,
        BlockedWithShieldother,
        UserSwing,
        SafeModeOn,
        SafeModeOff,
        ResuscitationSafeOff,
        ResuscitationSafeOn,
        NobilityLost,
        CantUseWhileMeditating,
        NPCHitUser,
        UserHitNPC,
        UserAttackedSwing,
        UserHittedByUser,
        UserHittedUser,
        WorkRequestTarget,
        HaveKilledUser,
        UserKill,
        EarnExp,
        Home,
        CancelHome,
        FinishHome
    }

    public enum eMochilas
    {
        Mediana = 1,
        Grande = 2
    }
    // [/KEVIN]

    // Determina el color del nick
    public enum eNickColor
    {
        ieCriminal = 0x1,
        ieCiudadano = 0x2,
        ieAtacable = 0x4
    }

    public enum eNPCType
    {
        Comun = 0,
        Revividor = 1,
        GuardiaReal = 2,
        Entrenador = 3,
        Banquero = 4,
        Noble = 5,
        DRAGON = 6,
        Timbero = 7,
        Guardiascaos = 8,
        ResucitadorNewbie = 9,
        Pretoriano = 10,
        Gobernador = 11
    }


    // CATEGORIAS PRINCIPALES
    public enum eOBJType
    {
        otUseOnce = 1,
        otWeapon = 2,
        otArmadura = 3,
        otArboles = 4,
        otGuita = 5,
        otPuertas = 6,
        otContenedores = 7,
        otCarteles = 8,
        otLlaves = 9,
        otForos = 10,
        otPociones = 11,
        otBebidas = 13,
        otLeña = 14,
        otFogata = 15,
        otESCUDO = 16,
        otCASCO = 17,
        otAnillo = 18,
        otTeleport = 19,
        otYacimiento = 22,
        otMinerales = 23,
        otPergaminos = 24,
        otInstrumentos = 26,
        otYunque = 27,
        otFragua = 28,
        otBarcos = 31,
        otFlechas = 32,
        otBotellaVacia = 33,
        otBotellaLlena = 34,
        otManchas = 35, // No se usa
        otArbolElfico = 36,
        otMochilas = 37,
        otCualquiera = 1000
    }

    public enum eRaza : byte
    {
        Humano = 1,
        Elfo,
        Drow,
        Gnomo,
        Enano
    }

    // %%%%%%%%%% CONSTANTES DE INDICES %%%%%%%%%%%%%%%
    public enum eSkill : short
    {
        Magia = 1,
        Robar = 2,
        Tacticas = 3,
        Armas = 4,
        Meditar = 5,
        Apuñalar = 6,
        Ocultarse = 7,
        Supervivencia = 8,
        Talar = 9,
        Comerciar = 10,
        Defensa = 11,
        Pesca = 12,
        Mineria = 13,
        Carpinteria = 14,
        Herreria = 15,
        Liderazgo = 16,
        Domar = 17,
        Proyectiles = 18,
        Wrestling = 19,
        Navegacion = 20
    }

    // '
    // TRIGGERS
    // 
    // @param NADA nada
    // @param BAJOTECHO bajo techo
    // @param trigger_2 ???
    // @param POSINVALIDA los npcs no pueden pisar tiles con este trigger
    // @param ZONASEGURA no se puede robar o pelear desde este trigger
    // @param ANTIPIQUETE
    // @param ZONAPELEA al pelear en este trigger no se caen las cosas y no cambia el estado de ciuda o crimi
    // 
    public enum eTrigger
    {
        NADA = 0,
        BAJOTECHO = 1,
        trigger_2 = 2,
        POSINVALIDA = 3,
        ZONASEGURA = 4,
        ANTIPIQUETE = 5,
        ZONAPELEA = 6
    }

    // '
    // constantes para el trigger 6
    // 
    // @see eTrigger
    // @param TRIGGER6_PERMITE TRIGGER6_PERMITE
    // @param TRIGGER6_PROHIBE TRIGGER6_PROHIBE
    // @param TRIGGER6_AUSENTE El trigger no aparece
    // 
    public enum eTrigger6
    {
        TRIGGER6_PERMITE = 1,
        TRIGGER6_PROHIBE = 2,
        TRIGGER6_AUSENTE = 3
    }

    public enum FXIDs : short
    {
        FXWARP = 1,
        FXMEDITARCHICO = 4,
        FXMEDITARMEDIANO = 5,
        FXMEDITARGRANDE = 6,
        FXMEDITARXGRANDE = 16,
        FXMEDITARXXGRANDE = 34
    }

    public enum iMinerales
    {
        HierroCrudo = 192,
        PlataCruda = 193,
        OroCrudo = 194,
        LingoteDeHierro = 386,
        LingoteDePlata = 387,
        LingoteDeOro = 388
    }

    // La utilidad de esto es casi nula, sólo se revisa si fue a la cabeza...
    public enum PartesCuerpo
    {
        bCabeza = 1,
        bPiernaIzquierda = 2,
        bPiernaDerecha = 3,
        bBrazoDerecho = 4,
        bBrazoIzquierdo = 5,
        bTorso = 6
    }

    public enum PECES_POSIBLES
    {
        PESCADO1 = 139,
        PESCADO2 = 544,
        PESCADO3 = 545,
        PESCADO4 = 546
    }

    public enum PlayerType
    {
        User = 0x1,
        Consejero = 0x2,
        SemiDios = 0x4,
        Dios = 0x8,
        Admin = 0x10,
        RoleMaster = 0x20,
        ChaosCouncil = 0x40,
        RoyalCouncil = 0x80
    }

    // <<<<<< Targets >>>>>>
    public enum TargetType
    {
        uUsuarios = 1,
        uNPC = 2,
        uUsuariosYnpc = 3,
        uTerreno = 4
    }

    // <<<<<< Acciona sobre >>>>>>
    public enum TipoHechizo
    {
        uPropiedades = 1,
        uEstado = 2,
        uMaterializa = 3, // Nose usa
        uInvocacion = 4
    }


    public const short MAXSPAWNATTEMPS = 60;
    public const short INFINITE_LOOPS = -1;
    public const short FXSANGRE = 14;

    // '
    // The color of chats over head of dead characters.
    public const int CHAT_COLOR_DEAD_CHAR = 0xC0C0C0;

    // '
    // The color of yells made by any kind of game administrator.
    public const int CHAT_COLOR_GM_YELL = 0xF82FF;

    // '
    // Coordinates for normal sounds (not 3D, like rain)
    public const byte NO_3D_SOUND = 0;

    public const short iFragataFantasmal = 87;
    public const short iFragataReal = 190;
    public const short iFragataCaos = 189;
    public const short iBarca = 84;
    public const short iGalera = 85;
    public const short iGaleon = 86;
    public const short iBarcaCiuda = 395;
    public const short iBarcaPk = 396;
    public const short iGaleraCiuda = 397;
    public const short iGaleraPk = 398;
    public const short iGaleonCiuda = 399;
    public const short iGaleonPk = 400;

    public const byte LimiteNewbie = 12;

    // Barrin 3/10/03
    // Cambiado a 2 segundos el 30/11/07
    public const short TIEMPO_INICIOMEDITAR = 2000;

    public const short NingunEscudo = 2;
    public const short NingunCasco = 2;
    public const short NingunArma = 2;

    public const short EspadaMataDragonesIndex = 402;
    public const short LAUDMAGICO = 696;
    public const short FLAUTAMAGICA = 208;

    public const short LAUDELFICO = 1049;
    public const short FLAUTAELFICA = 1050;

    public const short APOCALIPSIS_SPELL_INDEX = 25;
    public const short DESCARGA_SPELL_INDEX = 23;

    public const byte SLOTS_POR_FILA = 5;

    public const byte PROB_ACUCHILLAR = 20;
    public const float DAÑO_ACUCHILLAR = 0.2f;

    public const byte MAXMASCOTASENTRENADOR = 7;

    public const int TIEMPO_CARCEL_PIQUETE = 10;

    // TODO : Reemplazar por un enum
    public const string Bosque = "BOSQUE";
    public const string Nieve = "NIEVE";
    public const string Desierto = "DESIERTO";
    public const string Ciudad = "CIUDAD";
    public const string Campo = "CAMPO";
    public const string Dungeon = "DUNGEON";

    public const byte MAXUSERHECHIZOS = 35;


    // TODO: Y ESTO ? LO CONOCE GD ?
    public const byte EsfuerzoTalarGeneral = 4;
    public const byte EsfuerzoTalarLeñador = 2;

    public const byte EsfuerzoPescarPescador = 1;
    public const byte EsfuerzoPescarGeneral = 3;

    public const byte EsfuerzoExcavarMinero = 2;
    public const byte EsfuerzoExcavarGeneral = 5;

    public const short FX_TELEPORT_INDEX = 1;

    public const float PORCENTAJE_MATERIALES_UPGRADE = 0.85f;

    public const short Guardias = 6;

    public const int MAX_ORO_EDIT = 5000000;


    public const string STANDARD_BOUNTY_HUNTER_MESSAGE =
        "Se te ha otorgado un premio por ayudar al proyecto reportando bugs, el mismo está disponible en tu bóveda.";

    public const string TAG_USER_INVISIBLE = "[INVISIBLE]";
    public const string TAG_CONSULT_MODE = "[CONSULTA]";

    public const int MAXREP = 6000000;
    public const int MAXORO = 90000000;
    public const int MAXEXP = 99999999;

    public const int MAXUSERMATADOS = 65000;

    public const byte MAXATRIBUTOS = 40;
    public const byte MINATRIBUTOS = 6;

    public const short LingoteHierro = 386;
    public const short LingotePlata = 387;
    public const short LingoteOro = 388;
    public const short Leña = 58;
    public const short LeñaElfica = 1006;

    public const short MAXNPCS = 10000;
    public const short MAXCHARS = 10000;

    public const short HACHA_LEÑADOR = 127;
    public const short HACHA_LEÑA_ELFICA = 1005;
    public const short PIQUETE_MINERO = 187;

    public const short DAGA = 15;
    public const short FOGATA_APAG = 136;
    public const short FOGATA = 63;
    public const short ORO_MINA = 194;
    public const short PLATA_MINA = 193;
    public const short HIERRO_MINA = 192;
    public const short MARTILLO_HERRERO = 389;
    public const short SERRUCHO_CARPINTERO = 198;
    public const short ObjArboles = 4;
    public const short RED_PESCA = 543;
    public const short CAÑA_PESCA = 138;

    public const byte MIN_APUÑALAR = 10;

    // ********** CONSTANTANTES ***********

    // '
    // Cantidad de skills
    public const byte NUMSKILLS = 20;

    // '
    // Cantidad de Atributos
    public const byte NUMATRIBUTOS = 5;

    // '
    // Cantidad de Clases
    public const byte NUMCLASES = 12;

    // '
    // Cantidad de Razas
    public const byte NUMRAZAS = 5;


    // '
    // Valor maximo de cada skill
    public const byte MAXSKILLPOINTS = 100;

    // '
    // Cantidad de Ciudades
    public const byte NUMCIUDADES = 5;

    // '
    // Cantidad maxima de mascotas
    public const byte MAXMASCOTAS = 3;

    // %%%%%%%%%% CONSTANTES DE INDICES %%%%%%%%%%%%%%%
    public const short vlASALTO = 100;
    public const short vlASESINO = 1000;
    public const short vlCAZADOR = 5;
    public const short vlNoble = 5;
    public const short vlLadron = 25;
    public const short vlProleta = 2;

    // %%%%%%%%%% CONSTANTES DE INDICES %%%%%%%%%%%%%%%
    public const short iCuerpoMuerto = 8;
    public const short iCabezaMuerto = 500;


    public const byte iORO = 12;
    public const byte Pescado = 139;

    public const short FundirMetal = 88;

    public const byte AdicionalHPGuerrero = 2; // HP adicionales cuando sube de nivel
    public const byte AdicionalHPCazador = 1; // HP adicionales cuando sube de nivel

    public const byte AumentoSTDef = 15;
    public const byte AumentoStBandido = 38;
    public const byte AumentoSTLadron = 18;
    public const byte AumentoSTMago = 14;
    public const byte AumentoSTTrabajador = 40;

    // Tamaño del mapa
    public const byte XMaxMapSize = 100;
    public const byte XMinMapSize = 1;
    public const byte YMaxMapSize = 100;
    public const byte YMinMapSize = 1;

    // Tamaño del tileset
    public const byte TileSizeX = 32;
    public const byte TileSizeY = 32;

    // Tamaño en Tiles de la pantalla de visualizacion
    public const byte XWindow = 17;
    public const byte YWindow = 13;

    // Sonidos
    public const byte SND_SWING = 2;
    public const byte SND_TALAR = 13;
    public const byte SND_PESCAR = 14;
    public const byte SND_MINERO = 15;
    public const byte SND_WARP = 3;
    public const byte SND_PUERTA = 5;
    public const byte SND_NIVEL = 6;

    public const byte SND_USERMUERTE = 11;
    public const byte SND_IMPACTO = 10;
    public const byte SND_IMPACTO2 = 12;
    public const byte SND_LEÑADOR = 13;
    public const byte SND_FOGATA = 14;
    public const byte SND_AVE = 21;
    public const byte SND_AVE2 = 22;
    public const byte SND_AVE3 = 34;
    public const byte SND_GRILLO = 28;
    public const byte SND_GRILLO2 = 29;
    public const byte SND_SACARARMA = 25;
    public const byte SND_ESCUDO = 37;
    public const byte MARTILLOHERRERO = 41;
    public const byte LABUROCARPINTERO = 42;
    public const byte SND_BEBER = 46;

    // '
    // Cantidad maxima de objetos por slot de inventario
    public const short MAX_INVENTORY_OBJS = 10000;

    // '
    // Cantidad de "slots" en el inventario con mochila
    public const byte MAX_INVENTORY_SLOTS = 30;

    // '
    // Cantidad de "slots" en el inventario sin mochila
    public const byte MAX_NORMAL_INVENTORY_SLOTS = 20;

    // '
    // Constante para indicar que se esta usando ORO
    public const short FLAGORO = 31;

    // Texto
    public const string FONTTYPE_TALK = "~255~255~255~0~0";
    public const string FONTTYPE_FIGHT = "~255~0~0~1~0";
    public const string FONTTYPE_WARNING = "~32~51~223~1~1";
    public const string FONTTYPE_INFO = "~65~190~156~0~0";
    public const string FONTTYPE_INFOBOLD = "~65~190~156~1~0";
    public const string FONTTYPE_EJECUCION = "~130~130~130~1~0";
    public const string FONTTYPE_PARTY = "~255~180~255~0~0";
    public const string FONTTYPE_VENENO = "~0~255~0~0~0";
    public const string FONTTYPE_GUILD = "~255~255~255~1~0";
    public const string FONTTYPE_SERVER = "~0~185~0~0~0";
    public const string FONTTYPE_GUILDMSG = "~228~199~27~0~0";
    public const string FONTTYPE_CONSEJO = "~130~130~255~1~0";
    public const string FONTTYPE_CONSEJOCAOS = "~255~60~00~1~0";
    public const string FONTTYPE_CONSEJOVesA = "~0~200~255~1~0";
    public const string FONTTYPE_CONSEJOCAOSVesA = "~255~50~0~1~0";
    public const string FONTTYPE_CENTINELA = "~0~255~0~1~0";

    // Estadisticas
    public const byte STAT_MAXELV = 255;
    public const short STAT_MAXHP = 999;
    public const short STAT_MAXSTA = 999;
    public const short STAT_MAXMAN = 9999;
    public const byte STAT_MAXHIT_UNDER36 = 99;
    public const short STAT_MAXHIT_OVER36 = 999;
    public const byte STAT_MAXDEF = 99;

    public const byte ELU_SKILL_INICIAL = 200;
    public const byte EXP_ACIERTO_SKILL = 50;

    public const byte EXP_FALLO_SKILL = 20;
    // [/Pablo ToxicWaste]

    // [KEVIN]
    // Banco Objs
    public const byte MAX_BANCOINVENTORY_SLOTS = 40;

    // Limite de posts
    public const byte MAX_STICKY_POST = 10;
    public const byte MAX_GENERAL_POST = 35;

    public const byte MAX_NPC_DROPS = 5;

    public const short MATRIX_INITIAL_MAP = 1;

    public const short GOHOME_PENALTY = 5;
    public const short GM_MAP = 49;

    public const short TELEP_OBJ_INDEX = 1012;

    public const short HUMANO_H_PRIMER_CABEZA = 1;

    public const short HUMANO_H_ULTIMA_CABEZA = 40;
    // En verdad es hasta la 51, pero como son muchas estas las dejamos no seleccionables

    public const short ELFO_H_PRIMER_CABEZA = 101;
    public const short ELFO_H_ULTIMA_CABEZA = 122;

    public const short DROW_H_PRIMER_CABEZA = 201;
    public const short DROW_H_ULTIMA_CABEZA = 221;

    public const short ENANO_H_PRIMER_CABEZA = 301;
    public const short ENANO_H_ULTIMA_CABEZA = 319;

    public const short GNOMO_H_PRIMER_CABEZA = 401;

    public const short GNOMO_H_ULTIMA_CABEZA = 416;

    // **************************************************
    public const short HUMANO_M_PRIMER_CABEZA = 70;
    public const short HUMANO_M_ULTIMA_CABEZA = 89;

    public const short ELFO_M_PRIMER_CABEZA = 170;
    public const short ELFO_M_ULTIMA_CABEZA = 188;

    public const short DROW_M_PRIMER_CABEZA = 270;
    public const short DROW_M_ULTIMA_CABEZA = 288;

    public const short ENANO_M_PRIMER_CABEZA = 370;
    public const short ENANO_M_ULTIMA_CABEZA = 384;

    public const short GNOMO_M_PRIMER_CABEZA = 470;
    public const short GNOMO_M_ULTIMA_CABEZA = 484;

    // Por ahora la dejo constante.. SI se quisiera extender la propiedad de paralziar, se podria hacer
    // una nueva variable en el dat.
    public const short GUANTE_HURTO = 873;

    // '
    // Modulo de declaraciones. Aca hay de todo.
    // 
    public static List<WorldPos> TrashCollector = new();


    // ********** V A R I A B L E S     P U B L I C A S ***********

    public static bool SERVERONLINE;
    public static string ULTIMAVERSION;
    public static bool BackUp; // TODO: Se usa esta variable ?

    // UPGRADE_WARNING: El límite inferior de la matriz ListaRazas ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    public static string[] ListaRazas = new string[NUMRAZAS + 1];

    // UPGRADE_WARNING: El límite inferior de la matriz SkillsNames ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    public static string[] SkillsNames = new string[NUMSKILLS + 1];

    // UPGRADE_WARNING: El límite inferior de la matriz ListaClases ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    public static string[] ListaClases = new string[NUMCLASES + 1];

    // UPGRADE_WARNING: El límite inferior de la matriz ListaAtributos ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    public static string[] ListaAtributos = new string[NUMATRIBUTOS + 1];


    public static int recordusuarios;

    // 
    // Directorios
    // 

    // '
    // Ruta base del server, en donde esta el "server.ini"
    public static string IniPath;

    // '
    // Ruta base para guardar los chars
    public static string CharPath;

    // '
    // Ruta base para los archivos de mapas
    public static string MapPath;

    // '
    // Ruta base para los DATs
    public static string DatPath;

    // '
    // Bordes del mapa
    public static byte MinXBorder;
    public static byte MaxXBorder;
    public static byte MinYBorder;
    public static byte MaxYBorder;

    // '
    // Numero de usuarios actual
    public static short NumUsers;
    public static short LastUser;
    public static short LastChar;
    public static short NumChars;
    public static short LastNPC;
    public static short NumNPCs;
    public static short NumFX;
    public static short NumMaps;
    public static short NumObjDatas;
    public static short NumeroHechizos;
    public static byte AllowMultiLogins;
    public static short IdleLimit;
    public static short MaxUsers;
    public static byte HideMe;
    public static string LastBackup;
    public static string Minutos;
    public static bool haciendoBK;
    public static short PuedeCrearPersonajes;
    public static short ServerSoloGMs;

    // '
    // Esta activada la verificacion MD5 ?
    public static byte MD5ClientesActivado;


    public static bool EnPausa;
    public static bool EnTesting;


    // *****************ARRAYS PUBLICOS*************************
    public static User[] UserList; // USUARIOS

    // UPGRADE_WARNING: El límite inferior de la matriz Npclist ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    // UPGRADE_WARNING: Es posible que la matriz Npclist necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
    public static npc[] Npclist = new npc[MAXNPCS + 1]; // NPCS

    public static MapBlock[,,] MapData;

    public static MapInfo[] mapInfo;

    public static tHechizo[] Hechizos;

    // UPGRADE_WARNING: El límite inferior de la matriz CharList ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    public static short[] CharList = new short[MAXCHARS + 1];

    public static ObjData[] objData;
    public static FXdata[] FX;

    public static tCriaturasEntrenador[] SpawnList;

    // UPGRADE_WARNING: El límite inferior de la matriz levelSkill ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    public static LevelSkill[] levelSkill = new LevelSkill[51];
    public static string[] ForbidenNames;
    public static short[] ArmasHerrero;
    public static short[] ArmadurasHerrero;
    public static short[] ObjCarpintero;
    public static string[] MD5s;

    public static List<string> BanIps = new();

    // UPGRADE_WARNING: El límite inferior de la matriz Parties ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    public static clsParty[] Parties = new clsParty[mdParty.MAX_PARTIES + 1];

    // UPGRADE_WARNING: El límite inferior de la matriz modClase ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    public static ModClase[] modClase = new ModClase[NUMCLASES + 1];

    // UPGRADE_WARNING: El límite inferior de la matriz modRaza ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    public static ModRaza[] modRaza = new ModRaza[NUMRAZAS + 1];

    // UPGRADE_WARNING: El límite inferior de la matriz ModVida ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    public static double[] ModVida = new double[NUMCLASES + 1];

    // UPGRADE_WARNING: El límite inferior de la matriz DistribucionEnteraVida ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    public static short[] DistribucionEnteraVida = new short[6];

    // UPGRADE_WARNING: El límite inferior de la matriz DistribucionSemienteraVida ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    public static short[] DistribucionSemienteraVida = new short[5];

    // UPGRADE_WARNING: El límite inferior de la matriz Ciudades ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    public static WorldPos[] Ciudades = new WorldPos[NUMCIUDADES + 1];
    public static HomeDistance[] distanceToCities;

    public static WorldPos Nix;
    public static WorldPos Ullathorpe;
    public static WorldPos Banderbill;
    public static WorldPos Lindos;
    public static WorldPos Arghal;

    public static WorldPos Prision;
    public static WorldPos Libertad;

    public static cCola Ayuda = new();
    public static ConsultasPopulares ConsultaPopular = new();
    public static SoundMapInfo SonidosMapas = new();

    // **************************************************************
    // **************************************************************
    // ************************ TIPOS *******************************
    // **************************************************************
    // **************************************************************

    public struct tHechizo
    {
        public string Nombre;
        public string desc;
        public string PalabrasMagicas;
        public string HechizeroMsg;
        public string TargetMsg;

        public string PropioMsg;

        // Resis As Byte
        public TipoHechizo Tipo;
        public short WAV;
        public short FXgrh;
        public byte loops;
        public byte SubeHP;
        public short MinHp;
        public short MaxHp;
        public byte SubeMana;
        public short MiMana;
        public short MaMana;
        public byte SubeSta;
        public short MinSta;
        public short MaxSta;
        public byte SubeHam;
        public short MinHam;
        public short MaxHam;
        public byte SubeSed;
        public short MinSed;
        public short MaxSed;
        public byte SubeAgilidad;
        public short MinAgilidad;
        public short MaxAgilidad;
        public byte SubeFuerza;
        public short MinFuerza;
        public short MaxFuerza;
        public byte SubeCarisma;
        public short MinCarisma;
        public short MaxCarisma;
        public byte Invisibilidad;
        public byte Paraliza;
        public byte Inmoviliza;
        public byte RemoverParalisis;
        public byte RemoverEstupidez;
        public byte CuraVeneno;
        public byte Envenena;
        public byte Maldicion;
        public byte RemoverMaldicion;
        public byte Bendicion;
        public byte Estupidez;
        public byte Ceguera;
        public byte Revivir;
        public byte Morph;
        public byte Mimetiza;
        public byte RemueveInvisibilidadParcial;
        public byte Warp;
        public byte Invoca;
        public short NumNpc;

        public short cant;

        // Materializa As Byte
        // ItemIndex As Byte
        public short MinSkill;

        public short ManaRequerido;

        // Barrin 29/9/03
        public short StaRequerido;
        public TargetType Target;
        public short NeedStaff;
        public bool StaffAffected;
    }

    public struct LevelSkill
    {
        public short LevelValue;
    }

    public struct UserOBJ
    {
        public short ObjIndex;
        public short Amount;
        public byte Equipped;
    }

    public struct Inventario
    {
        [VBFixedArray(MAX_INVENTORY_SLOTS)] public UserOBJ[] userObj;
        public short WeaponEqpObjIndex;
        public byte WeaponEqpSlot;
        public short ArmourEqpObjIndex;
        public byte ArmourEqpSlot;
        public short EscudoEqpObjIndex;
        public byte EscudoEqpSlot;
        public short CascoEqpObjIndex;
        public byte CascoEqpSlot;
        public short MunicionEqpObjIndex;
        public byte MunicionEqpSlot;
        public short AnilloEqpObjIndex;
        public byte AnilloEqpSlot;
        public short BarcoObjIndex;
        public byte BarcoSlot;
        public short MochilaEqpObjIndex;
        public byte MochilaEqpSlot;
        public short NroItems;

        public void Initialize()
        {
            userObj = new UserOBJ[MAX_INVENTORY_SLOTS + 1];
        }
    }

    public struct tPartyData
    {
        public short PIndex;
        public double RemXP; // La exp. en el server se cuenta con Doubles
        public short TargetUser; // Para las invitaciones
    }

    public struct Position
    {
        public short X;
        public short Y;
    }

    public struct WorldPos
    {
        public short Map;
        public short X;
        public short Y;
    }

    public struct FXdata
    {
        public string Nombre;
        public short GrhIndex;
        public short Delay;
    }

    // Datos de user o npc
    public struct Character
    {
        public short CharIndex;
        public short Head;
        public short body;
        public short WeaponAnim;
        public short ShieldAnim;
        public short CascoAnim;
        public short FX;
        public short loops;
        public eHeading heading;
    }

    // Tipos de objetos
    public struct ObjData
    {
        public string name; // Nombre del obj
        public eOBJType OBJType; // Tipo enum que determina cuales son las caract del obj
        public short GrhIndex; // Indice del grafico que representa el obj

        public short GrhSecundario;

        // Solo contenedores
        public short MaxItems;

        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Conte, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        public Inventario Conte;
        public byte Apuñala;
        public byte Acuchilla;
        public short HechizoIndex;
        public string ForoID;
        public short MinHp; // Minimo puntos de vida
        public short MaxHp; // Maximo puntos de vida
        public short MineralIndex;
        public short LingoteInex;
        public short proyectil;
        public short Municion;
        public byte Crucial;

        public short Newbie;

        // Puntos de Stamina que da
        public short MinSta; // Minimo puntos de stamina

        // Pociones
        public byte TipoPocion;
        public short MaxModificador;
        public short MinModificador;
        public int DuracionEfecto;
        public short MinSkill;
        public short LingoteIndex;
        public short MinHIT; // Minimo golpe
        public short MaxHIT; // Maximo golpe
        public short MinHam;
        public short MinSed;
        public short def;
        public short MinDef; // Armaduras
        public short MaxDef; // Armaduras
        public short Ropaje; // Indice del grafico del ropaje
        public short WeaponAnim; // Apunta a una anim de armas
        public short WeaponRazaEnanaAnim;
        public short ShieldAnim; // Apunta a una anim de escudo
        public short CascoAnim;
        public int Valor; // Precio
        public short Cerrada;
        public byte Llave;
        public int clave; // si clave=llave la puerta se abre o cierra
        public short Radio; // Para teleps: El radio para calcular el random de la pos destino
        public byte MochilaType; // Tipo de Mochila (1 la chica, 2 la grande)
        public byte Guante; // Indica si es un guante o no.
        public short IndexAbierta;
        public short IndexCerrada;
        public short IndexCerradaLlave;
        public byte RazaEnana;
        public byte RazaDrow;
        public byte RazaElfa;
        public byte RazaGnoma;
        public byte RazaHumana;
        public byte Mujer;
        public byte Hombre;
        public byte Envenena;
        public byte Paraliza;
        public byte Agarrable;
        public short LingH;
        public short LingO;
        public short LingP;
        public short Madera;
        public short MaderaElfica;
        public short SkHerreria;
        public short SkCarpinteria;

        public string texto;

        // Clases que no tienen permitido usar este obj
        [VBFixedArray(NUMCLASES)] public eClass[] ClaseProhibida;
        public short Snd1;
        public short Snd2;
        public short Snd3;
        public short Real;
        public short Caos;
        public short NoSeCae;
        public short StaffPower;
        public short StaffDamageBonus;
        public short DefensaMagicaMax;
        public short DefensaMagicaMin;
        public byte Refuerzo;
        public byte Log; // es un objeto que queremos loguear? Pablo (ToxicWaste) 07/09/07
        public byte NoLog; // es un objeto que esta prohibido loguear?
        public short Upgrade;

        // UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        public void Initialize()
        {
            // UPGRADE_WARNING: El límite inferior de la matriz ClaseProhibida ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ClaseProhibida = new eClass[NUMCLASES + 1];
        }
    }

    public struct Obj
    {
        public short ObjIndex;
        public short Amount;
    }

    // [Pablo ToxicWaste]
    public struct ModClase
    {
        public double Evasion;
        public double AtaqueArmas;
        public double AtaqueProyectiles;
        public double AtaqueWrestling;
        public double DañoArmas;
        public double DañoProyectiles;
        public double DañoWrestling;
        public double Escudo;
    }

    public struct ModRaza
    {
        public float Fuerza;
        public float Agilidad;
        public float Inteligencia;
        public float Carisma;
        public float Constitucion;
    }
    // [/KEVIN]

    // [KEVIN]
    public struct BancoInventario
    {
        [VBFixedArray(MAX_BANCOINVENTORY_SLOTS)]
        public UserOBJ[] userObj;

        public short NroItems;

        public void Initialize()
        {
            userObj = new UserOBJ[MAX_BANCOINVENTORY_SLOTS + 1];
        }
    }

    // Estructura contenedora de mensajes
    public struct tForo
    {
        [VBFixedArray(MAX_STICKY_POST)] public string[] StickyTitle;
        [VBFixedArray(MAX_STICKY_POST)] public string[] StickyPost;
        [VBFixedArray(MAX_GENERAL_POST)] public string[] GeneralTitle;
        [VBFixedArray(MAX_GENERAL_POST)] public string[] GeneralPost;

        // UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        public void Initialize()
        {
            // UPGRADE_WARNING: El límite inferior de la matriz StickyTitle ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            StickyTitle = new string[MAX_STICKY_POST + 1];
            // UPGRADE_WARNING: El límite inferior de la matriz StickyPost ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            StickyPost = new string[MAX_STICKY_POST + 1];
            // UPGRADE_WARNING: El límite inferior de la matriz GeneralTitle ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            GeneralTitle = new string[MAX_GENERAL_POST + 1];
            // UPGRADE_WARNING: El límite inferior de la matriz GeneralPost ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            GeneralPost = new string[MAX_GENERAL_POST + 1];
        }
    }

    // *********************************************************
    // *********************************************************
    // *********************************************************
    // *********************************************************
    // ******* T I P O S   D E    U S U A R I O S **************
    // *********************************************************
    // *********************************************************
    // *********************************************************
    // *********************************************************

    public struct tReputacion // Fama del usuario
    {
        public int NobleRep;
        public int BurguesRep;
        public int PlebeRep;
        public int LadronesRep;
        public int BandidoRep;
        public int AsesinoRep;
        public int Promedio;
    }

    // Estadisticas de los usuarios
    public struct UserStats
    {
        public int GLD; // Dinero
        public int Banco;
        public short MaxHp;
        public short MinHp;
        public short MaxSta;
        public short MinSta;
        public short MaxMAN;
        public short MinMAN;
        public short MaxHIT;
        public short MinHIT;
        public short MaxHam;
        public short MinHam;
        public short MaxAGU;
        public short MinAGU;
        public short def;
        public double Exp;
        public byte ELV;
        public int ELU;
        [VBFixedArray(NUMSKILLS)] public byte[] UserSkills;
        [VBFixedArray(NUMATRIBUTOS)] public byte[] UserAtributos;
        [VBFixedArray(NUMATRIBUTOS)] public byte[] UserAtributosBackUP;
        [VBFixedArray(MAXUSERHECHIZOS)] public short[] UserHechizos;
        public int UsuariosMatados;
        public int CriminalesMatados;
        public short NPCsMuertos;
        public short SkillPts;
        [VBFixedArray(NUMSKILLS)] public int[] ExpSkills;
        [VBFixedArray(NUMSKILLS)] public int[] EluSkills;

        // UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        public void Initialize()
        {
            // UPGRADE_WARNING: El límite inferior de la matriz UserSkills ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            UserSkills = new byte[NUMSKILLS + 1];
            // UPGRADE_WARNING: El límite inferior de la matriz UserAtributos ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            UserAtributos = new byte[NUMATRIBUTOS + 1];
            // UPGRADE_WARNING: El límite inferior de la matriz UserAtributosBackUP ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            UserAtributosBackUP = new byte[NUMATRIBUTOS + 1];
            // UPGRADE_WARNING: El límite inferior de la matriz UserHechizos ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            UserHechizos = new short[MAXUSERHECHIZOS + 1];
            // UPGRADE_WARNING: El límite inferior de la matriz ExpSkills ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ExpSkills = new int[NUMSKILLS + 1];
            // UPGRADE_WARNING: El límite inferior de la matriz EluSkills ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            EluSkills = new int[NUMSKILLS + 1];
        }
    }

    // Flags
    public struct UserFlags
    {
        public byte Muerto; // ¿Esta muerto?
        public byte Escondido; // ¿Esta escondido?
        public bool Comerciando; // ¿Esta comerciando?
        public bool UserLogged; // ¿Esta online?
        public bool Meditando;
        public string Descuento;
        public byte Hambre;
        public byte Sed;
        public byte PuedeMoverse;
        public int TimerLanzarSpell;
        public byte PuedeTrabajar;
        public byte Envenenado;
        public byte Paralizado;
        public byte Inmovilizado;
        public byte Estupidez;
        public byte Ceguera;
        public byte invisible;
        public byte Maldicion;
        public byte Bendicion;
        public byte Oculto;
        public byte Desnudo;
        public bool Descansar;
        public short Hechizo;
        public bool TomoPocion;
        public byte TipoPocion;
        public bool NoPuedeSerAtacado;
        public short AtacablePor;
        public short ShareNpcWith;
        public byte Vuela;
        public byte Navegando;
        public bool Seguro;
        public bool SeguroResu;
        public int DuracionEfecto;
        public short TargetNPC; // Npc señalado por el usuario
        public eNPCType TargetNpcTipo; // Tipo del npc señalado
        public short OwnedNpc; // Npc que le pertenece (no puede ser atacado)
        public short NpcInv;
        public byte Ban;
        public byte AdministrativeBan;
        public short TargetUser; // Usuario señalado
        public short TargetObj; // Obj señalado
        public short TargetObjMap;
        public short TargetObjX;
        public short TargetObjY;
        public short TargetMap;
        public short TargetX;
        public short TargetY;
        public short TargetObjInvIndex;
        public short TargetObjInvSlot;
        public short AtacadoPorNpc;
        public short AtacadoPorUser;
        public short NPCAtacado;
        public bool Ignorado;
        public bool EnConsulta;
        public byte StatsChanged;
        public PlayerType Privilegios;
        public short ValCoDe;
        public string LastCrimMatado;
        public string LastCiudMatado;
        public short OldBody;
        public short OldHead;
        public byte AdminInvisible;
        public bool AdminPerseguible;

        public int ChatColor;

        // [el oso]
        public string MD5Reportado;

        // [/el oso]
        // [Barrin 30-11-03]
        public int TimesWalk;
        public int StartWalk;

        public int CountSH;

        // [/Barrin 30-11-03]
        // [CDT 17-02-04]
        public byte UltimoMensaje;

        // [/CDT]
        public byte Silenciado;
        public byte Mimetizado;
        public bool CentinelaOK; // Centinela
        public short lastMap;
        public byte Traveling; // Travelin Band ¿?
    }

    public struct UserCounters
    {
        public int IdleCount;
        public short AttackCounter;
        public short HPCounter;
        public short STACounter;
        public short Frio;
        public short Lava;
        public short COMCounter;
        public short AGUACounter;
        public short Veneno;
        public short Paralisis;
        public short Ceguera;
        public short Estupidez;
        public short Invisibilidad;
        public short TiempoOculto;
        public short Mimetismo;
        public int PiqueteC;
        public int Pena;

        public WorldPos SendMapCounter;

        // [Gonzalo]
        public bool Saliendo;

        public short salir;

        // [/Gonzalo]
        // Barrin 3/10/03
        public int tInicioMeditar;

        public bool bPuedeMeditar;

        // Barrin
        public int TimerLanzarSpell;
        public int TimerPuedeAtacar;
        public int TimerPuedeUsarArco;
        public int TimerPuedeTrabajar;
        public int TimerUsar;
        public int TimerMagiaGolpe;
        public int TimerGolpeMagia;
        public int TimerGolpeUsar;
        public int TimerPuedeSerAtacado;
        public int TimerPerteneceNpc;
        public int TimerEstadoAtacable;
        public int Trabajando; // Para el centinela
        public int Ocultando; // Unico trabajo no revisado por el centinela
        public int failedUsageAttempts;
        public int goHome;
        public byte AsignedSkills;
    }

    // Cosas faccionarias.
    public struct tFacciones
    {
        public byte ArmadaReal;
        public byte FuerzasCaos;
        public int CriminalesMatados;
        public int CiudadanosMatados;
        public int RecompensasReal;
        public int RecompensasCaos;
        public byte RecibioExpInicialReal;
        public byte RecibioExpInicialCaos;
        public byte RecibioArmaduraReal;
        public byte RecibioArmaduraCaos;
        public byte Reenlistadas;
        public short NivelIngreso;
        public string FechaIngreso;
        public short MatadosIngreso; // Para Armadas nada mas
        public short NextRecompensa;
    }

    public struct tCrafting
    {
        public int Cantidad;
        public short PorCiclo;
    }

    // Tipo de los Usuarios
    public struct User
    {
        public string name;
        public int ID;

        public bool showName; // Permite que los GMs oculten su nick con el comando /SHOWNAME

        public Character character; // Define la apariencia
        public Character CharMimetizado;
        public Character OrigChar;
        public string desc; // Descripcion
        public string DescRM;
        public eClass clase;
        public eRaza raza;
        public eGenero Genero;
        public string email;

        public eCiudad Hogar;

        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Invent, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        public Inventario Invent;
        public WorldPos Pos;
        public bool ConnIDValida;

        public short ConnID; // ID

        // [KEVIN]
        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura BancoInvent, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        public BancoInventario BancoInvent;

        // [/KEVIN]
        public UserCounters Counters;
        public tCrafting Construir;
        [VBFixedArray(MAXMASCOTAS)] public short[] MascotasIndex;
        [VBFixedArray(MAXMASCOTAS)] public short[] MascotasType;

        public short NroMascotas;

        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Stats, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        public UserStats Stats;
        public UserFlags flags;
        public tReputacion Reputacion;
        public tFacciones Faccion;
        public DateTime LogOnTime;
        public int UpTime;

        public string ip;

        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura ComUsu, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        public mdlCOmercioConUsuario.tCOmercioUsuario ComUsu;
        public short GuildIndex; // puntero al array global de guilds

        public modGuilds.ALINEACION_GUILD FundandoGuildAlineacion;

        // esto esta aca hasta que se parchee el cliente y se pongan cadenas de datos distintas para cada alineacion
        public short EscucheClan;
        public short PartyIndex; // index a la party q es miembro
        public short PartySolicitud; // index a la party q solicito
        public short KeyCrypt;

        public ModAreas.AreaInfo AreasInfo;

        // Outgoing and incoming messages
        public clsByteQueue outgoingData;
        public clsByteQueue incomingData;
        public byte CurrentInventorySlots;

        // UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        public void Initialize()
        {
            // UPGRADE_WARNING: El límite inferior de la matriz MascotasIndex ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            MascotasIndex = new short[MAXMASCOTAS + 1];
            // UPGRADE_WARNING: El límite inferior de la matriz MascotasType ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            MascotasType = new short[MAXMASCOTAS + 1];
            Stats.Initialize();
            ComUsu.Initialize();
            Invent.Initialize();
            BancoInvent.Initialize();
        }
    }


    // *********************************************************
    // *********************************************************
    // *********************************************************
    // *********************************************************
    // **  T I P O S   D E    N P C S **************************
    // *********************************************************
    // *********************************************************
    // *********************************************************
    // *********************************************************

    public struct NPCStats
    {
        public short Alineacion;
        public int MaxHp;
        public int MinHp;
        public short MaxHIT;
        public short MinHIT;
        public short def;
        public short defM;
    }

    public struct NpcCounters
    {
        public short Paralisis;
        public int TiempoExistencia;
    }

    public struct NPCFlags
    {
        public byte AfectaParalisis;
        public short Domable;
        public byte Respawn;
        public bool NPCActive; // ¿Esta vivo?
        public bool Follow;
        public byte Faccion;
        public byte AtacaDoble;
        public byte LanzaSpells;
        public int ExpCount;
        public AI.TipoAI OldMovement;
        public byte OldHostil;
        public byte AguaValida;
        public byte TierraInvalida;
        public short Sound;
        public string AttackedBy;
        public string AttackedFirstBy;
        public byte BackUp;
        public byte RespawnOrigPos;
        public byte Envenenado;
        public byte Paralizado;
        public byte Inmovilizado;
        public byte invisible;
        public byte Maldicion;
        public byte Bendicion;
        public short Snd1;
        public short Snd2;
        public short Snd3;
    }

    public struct tCriaturasEntrenador
    {
        public short NpcIndex;
        public string NpcName;
        public short tmpIndex;
    }

    // New type for holding the pathfinding info
    public struct NpcPathFindingInfo
    {
        public Queue.tVertice[] Path; // This array holds the path
        public Position Target; // The location where the NPC has to go
        public short PathLenght; // Number of steps *
        public short CurPos; // Current location of the npc
        public short TargetUser; // UserIndex chased

        public bool NoPath; // If it is true there is no path to the target location
        // * By setting PathLenght to 0 we force the recalculation
        // of the path, this is very useful. For example,
        // if a NPC or a User moves over the npc's path, blocking
        // its way, the function NpcLegalPos set PathLenght to 0
        // forcing the seek of a new path.
    }
    // New type for holding the pathfinding info

    public struct tDrops
    {
        public short ObjIndex;
        public int Amount;
    }

    public struct npc
    {
        public string name;

        public Character character; // Define como se vera
        public string desc;
        public eNPCType NPCtype;
        public short Numero;
        public byte InvReSpawn;
        public short Comercia;
        public int Target;
        public int TargetNPC;
        public short TipoItems;
        public byte Veneno;
        public WorldPos Pos; // Posicion
        public WorldPos Orig;
        public short SkillDomar;
        public AI.TipoAI Movement;
        public byte Attackable;
        public byte Hostile;
        public int PoderAtaque;
        public int PoderEvasion;
        public short Owner;
        public int GiveEXP;
        public int GiveGLD;
        [VBFixedArray(MAX_NPC_DROPS)] public tDrops[] Drop;
        public NPCStats Stats;
        public NPCFlags flags;

        public NpcCounters Contadores;

        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Invent, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        public Inventario Invent;
        public byte CanAttack;
        public byte NroExpresiones;
        public string[] Expresiones; // le da vida ;)
        public byte NroSpells;

        public short[] Spells; // le da vida ;)

        // <<<<Entrenadores>>>>>
        public short NroCriaturas;
        public tCriaturasEntrenador[] Criaturas;
        public short MaestroUser;
        public short MaestroNpc;

        public short Mascotas;

        // New!! Needed for pathfindig
        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura PFINFO, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        public NpcPathFindingInfo PFINFO;

        public ModAreas.AreaInfo AreasInfo;

        // Hogar
        public byte Ciudad;

        // UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        public void Initialize()
        {
            // UPGRADE_WARNING: El límite inferior de la matriz Drop ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            Drop = new tDrops[MAX_NPC_DROPS + 1];
            Invent.Initialize();
        }
    }

    // **********************************************************
    // **********************************************************
    // ******************** Tipos del mapa **********************
    // **********************************************************
    // **********************************************************
    // Tile
    public struct MapBlock
    {
        public byte Blocked;
        [VBFixedArray(4)] public short[] Graphic;
        public short UserIndex;
        public short NpcIndex;
        public Obj ObjInfo;
        public WorldPos TileExit;
        public eTrigger trigger;

        // UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        public void Initialize()
        {
            // UPGRADE_WARNING: El límite inferior de la matriz Graphic ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            Graphic = new short[5];
        }
    }

    // Info del mapa
    public struct MapInfo
    {
        public short NumUsers;
        public string Music;
        public string name;
        public short MapVersion;
        public bool Pk;
        public byte MagiaSinEfecto;
        public byte NoEncriptarMP;
        public byte InviSinEfecto;
        public byte ResuSinEfecto;
        public byte RoboNpcsPermitido;
        public string Terreno;
        public string Zona;
        public string Restringir;
        public byte BackUp;
    }
    // *********************************************************

    public struct HomeDistance
    {
        [VBFixedArray(5)] public short[] distanceToCity;

        // UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        public void Initialize()
        {
            // UPGRADE_WARNING: El límite inferior de la matriz distanceToCity ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            distanceToCity = new short[6];
        }
    }
}