using System;
using System.IO;
using System.Threading;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Legacy;

public static class General
{
    internal static clsIniReader LeerNPCs = new();

    /// <summary>
    ///     Appends a line to a log file. Creates the file if it doesn't exist.
    /// </summary>
    public static void AppendLog(string relativePath, string message)
    {
        try
        {
            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            File.AppendAllText(fullPath, message + Environment.NewLine);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in AppendLog (" + relativePath + "): " + ex.Message);
        }
    }

    public static void DarCuerpoDesnudo(short UserIndex, bool Mimetizado = false)
    {
        // ***************************************************
        // Autor: Nacho (Integer)
        // Last Modification: 03/14/07
        // Da cuerpo desnudo a un usuario
        // 23/11/2009: ZaMa - Optimizacion de codigo.
        // ***************************************************

        var CuerpoDesnudo = default(short);

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            switch (withBlock.Genero)
            {
                case Declaraciones.eGenero.Hombre:
                {
                    switch (withBlock.raza)
                    {
                        case Declaraciones.eRaza.Humano:
                        {
                            CuerpoDesnudo = 21;
                            break;
                        }
                        case Declaraciones.eRaza.Drow:
                        {
                            CuerpoDesnudo = 32;
                            break;
                        }
                        case Declaraciones.eRaza.Elfo:
                        {
                            CuerpoDesnudo = 210;
                            break;
                        }
                        case Declaraciones.eRaza.Gnomo:
                        {
                            CuerpoDesnudo = 222;
                            break;
                        }
                        case Declaraciones.eRaza.Enano:
                        {
                            CuerpoDesnudo = 53;
                            break;
                        }
                    }

                    break;
                }
                case Declaraciones.eGenero.Mujer:
                {
                    switch (withBlock.raza)
                    {
                        case Declaraciones.eRaza.Humano:
                        {
                            CuerpoDesnudo = 39;
                            break;
                        }
                        case Declaraciones.eRaza.Drow:
                        {
                            CuerpoDesnudo = 40;
                            break;
                        }
                        case Declaraciones.eRaza.Elfo:
                        {
                            CuerpoDesnudo = 259;
                            break;
                        }
                        case Declaraciones.eRaza.Gnomo:
                        {
                            CuerpoDesnudo = 260;
                            break;
                        }
                        case Declaraciones.eRaza.Enano:
                        {
                            CuerpoDesnudo = 60;
                            break;
                        }
                    }

                    break;
                }
            }

            if (Mimetizado)
                withBlock.CharMimetizado.body = CuerpoDesnudo;
            else
                withBlock.character.body = CuerpoDesnudo;

            withBlock.flags.Desnudo = 1;
        }
    }


    public static void Bloquear(bool toMap, short sndIndex, short X, short Y, bool b)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // b ahora es boolean,
        // b=true bloquea el tile en (x,y)
        // b=false desbloquea el tile en (x,y)
        // toMap = true -> Envia los datos a todo el mapa
        // toMap = false -> Envia los datos al user
        // Unifique los tres parametros (sndIndex,sndMap y map) en sndIndex... pero de todas formas, el mapa jamas se indica.. eso esta bien asi?
        // Puede llegar a ser, que se quiera mandar el mapa, habria que agregar un nuevo parametro y modificar.. lo quite porque no se usaba ni aca ni en el cliente :s
        // ***************************************************

        if (toMap)
            modSendData.SendData(modSendData.SendTarget.toMap, sndIndex,
                Protocol.PrepareMessageBlockPosition(Convert.ToByte(X), Convert.ToByte(Y), b));
        else
            Protocol.WriteBlockPosition(sndIndex, Convert.ToByte(X), Convert.ToByte(Y), b);
    }


    public static bool HayAgua(short Map, short X, short Y)
    {
        bool HayAguaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if ((Map > 0) & (Map < Declaraciones.NumMaps + 1) & (X > 0) & (X < 101) & (Y > 0) & (Y < 101))
        {
            ref var withBlock = ref Declaraciones.MapData[Map, X, Y];
            if ((((withBlock.Graphic[1] >= 1505) & (withBlock.Graphic[1] <= 1520)) |
                 ((withBlock.Graphic[1] >= 5665) & (withBlock.Graphic[1] <= 5680)) |
                 ((withBlock.Graphic[1] >= 13547) & (withBlock.Graphic[1] <= 13562))) & (withBlock.Graphic[2] == 0))
                HayAguaRet = true;
            else
                HayAguaRet = false;
        }
        else
        {
            HayAguaRet = false;
        }

        return HayAguaRet;
    }

    private static bool HayLava(short Map, short X, short Y)
    {
        bool HayLavaRet = default;
        // ***************************************************
        // Autor: Nacho (Integer)
        // Last Modification: 03/12/07
        // ***************************************************
        if ((Map > 0) & (Map < Declaraciones.NumMaps + 1) & (X > 0) & (X < 101) & (Y > 0) & (Y < 101))
        {
            if ((Declaraciones.MapData[Map, X, Y].Graphic[1] >= 5837) &
                (Declaraciones.MapData[Map, X, Y].Graphic[1] <= 5852))
                HayLavaRet = true;
            else
                HayLavaRet = false;
        }
        else
        {
            HayLavaRet = false;
        }

        return HayLavaRet;
    }


    public static void LimpiarMundo()
    {
        // ***************************************************
        // Author: Unknow
        // Last Modification: 04/15/2008
        // 01/14/2008: Marcos Martinez (ByVal) - La funcion FOR estaba mal. En ves de i habia un 1.
        // 04/15/2008: (NicoNZ) - La funcion FOR estaba mal, de la forma que se hacia tiraba error.
        // ***************************************************
        try
        {
            short i;
            var d = new Declaraciones.WorldPos();

            for (i = Convert.ToInt16(Declaraciones.TrashCollector.Count - 1); i >= 0; i += -1)
            {
                d = Declaraciones.TrashCollector[i];
                InvUsuario.EraseObj(1, d.Map, d.X, d.Y);
                Declaraciones.TrashCollector.RemoveAt(i);
                // UPGRADE_NOTE: El objeto d no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
                d = default;
            }

            SecurityIp.IpSecurityMantenimientoLista();
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in LimpiarMundo: " + ex.Message);
            var argdesc = "Error producido en el sub LimpiarMundo: " + ex.Message;
            LogError(ref argdesc);
        }
    }

    public static void EnviarSpawnList(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int k;
        string[] npcNames;

        // UPGRADE_WARNING: El límite inferior de la matriz npcNames ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        npcNames = new string[Declaraciones.SpawnList.Length];

        var loopTo = Declaraciones.SpawnList.Length - 1;
        for (k = 1; k <= loopTo; k++)
            npcNames[k] = Declaraciones.SpawnList[k].NpcName;

        Protocol.WriteSpawnList(UserIndex, npcNames);
    }

    public static void Main()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            ES.LoadMotd();
            Admin.BanIpCargar();

            // TODO: reubicar estas inicializaciones!
            ArrayInitializers.InitializeStruct(Declaraciones.Npclist);
            ArrayInitializers.InitializeStruct(ModFacciones.ArmadurasFaccion);
            ArrayInitializers.InitializeStruct(Statistics.fragLvlRaceData);
            ArrayInitializers.InitializeStruct(Statistics.fragLvlLvlData);


            Declaraciones.Prision.Map = 66;
            Declaraciones.Libertad.Map = 66;

            Declaraciones.Prision.X = 75;
            Declaraciones.Prision.Y = 47;
            Declaraciones.Libertad.X = 75;
            Declaraciones.Libertad.Y = 65;


            Declaraciones.LastBackup = DateTime.Now.ToString("HH:mm");
            Declaraciones.Minutos = DateTime.Now.ToString("HH:mm");

            Declaraciones.IniPath = AppDomain.CurrentDomain.BaseDirectory;
            Declaraciones.DatPath = AppDomain.CurrentDomain.BaseDirectory + "Dat/";


            Declaraciones.levelSkill[1].LevelValue = 3;
            Declaraciones.levelSkill[2].LevelValue = 5;
            Declaraciones.levelSkill[3].LevelValue = 7;
            Declaraciones.levelSkill[4].LevelValue = 10;
            Declaraciones.levelSkill[5].LevelValue = 13;
            Declaraciones.levelSkill[6].LevelValue = 15;
            Declaraciones.levelSkill[7].LevelValue = 17;
            Declaraciones.levelSkill[8].LevelValue = 20;
            Declaraciones.levelSkill[9].LevelValue = 23;
            Declaraciones.levelSkill[10].LevelValue = 25;
            Declaraciones.levelSkill[11].LevelValue = 27;
            Declaraciones.levelSkill[12].LevelValue = 30;
            Declaraciones.levelSkill[13].LevelValue = 33;
            Declaraciones.levelSkill[14].LevelValue = 35;
            Declaraciones.levelSkill[15].LevelValue = 37;
            Declaraciones.levelSkill[16].LevelValue = 40;
            Declaraciones.levelSkill[17].LevelValue = 43;
            Declaraciones.levelSkill[18].LevelValue = 45;
            Declaraciones.levelSkill[19].LevelValue = 47;
            Declaraciones.levelSkill[20].LevelValue = 50;
            Declaraciones.levelSkill[21].LevelValue = 53;
            Declaraciones.levelSkill[22].LevelValue = 55;
            Declaraciones.levelSkill[23].LevelValue = 57;
            Declaraciones.levelSkill[24].LevelValue = 60;
            Declaraciones.levelSkill[25].LevelValue = 63;
            Declaraciones.levelSkill[26].LevelValue = 65;
            Declaraciones.levelSkill[27].LevelValue = 67;
            Declaraciones.levelSkill[28].LevelValue = 70;
            Declaraciones.levelSkill[29].LevelValue = 73;
            Declaraciones.levelSkill[30].LevelValue = 75;
            Declaraciones.levelSkill[31].LevelValue = 77;
            Declaraciones.levelSkill[32].LevelValue = 80;
            Declaraciones.levelSkill[33].LevelValue = 83;
            Declaraciones.levelSkill[34].LevelValue = 85;
            Declaraciones.levelSkill[35].LevelValue = 87;
            Declaraciones.levelSkill[36].LevelValue = 90;
            Declaraciones.levelSkill[37].LevelValue = 93;
            Declaraciones.levelSkill[38].LevelValue = 95;
            Declaraciones.levelSkill[39].LevelValue = 97;
            Declaraciones.levelSkill[40].LevelValue = 100;
            Declaraciones.levelSkill[41].LevelValue = 100;
            Declaraciones.levelSkill[42].LevelValue = 100;
            Declaraciones.levelSkill[43].LevelValue = 100;
            Declaraciones.levelSkill[44].LevelValue = 100;
            Declaraciones.levelSkill[45].LevelValue = 100;
            Declaraciones.levelSkill[46].LevelValue = 100;
            Declaraciones.levelSkill[47].LevelValue = 100;
            Declaraciones.levelSkill[48].LevelValue = 100;
            Declaraciones.levelSkill[49].LevelValue = 100;
            Declaraciones.levelSkill[50].LevelValue = 100;


            Declaraciones.ListaRazas[(int)Declaraciones.eRaza.Humano] = "Humano";
            Declaraciones.ListaRazas[(int)Declaraciones.eRaza.Elfo] = "Elfo";
            Declaraciones.ListaRazas[(int)Declaraciones.eRaza.Drow] = "Drow";
            Declaraciones.ListaRazas[(int)Declaraciones.eRaza.Gnomo] = "Gnomo";
            Declaraciones.ListaRazas[(int)Declaraciones.eRaza.Enano] = "Enano";

            Declaraciones.ListaClases[(int)Declaraciones.eClass.Mage] = "Mago";
            Declaraciones.ListaClases[(int)Declaraciones.eClass.Cleric] = "Clerigo";
            Declaraciones.ListaClases[(int)Declaraciones.eClass.Warrior] = "Guerrero";
            Declaraciones.ListaClases[(int)Declaraciones.eClass.Assasin] = "Asesino";
            Declaraciones.ListaClases[(int)Declaraciones.eClass.Thief] = "Ladron";
            Declaraciones.ListaClases[(int)Declaraciones.eClass.Bard] = "Bardo";
            Declaraciones.ListaClases[(int)Declaraciones.eClass.Druid] = "Druida";
            Declaraciones.ListaClases[(int)Declaraciones.eClass.Bandit] = "Bandido";
            Declaraciones.ListaClases[(int)Declaraciones.eClass.Paladin] = "Paladin";
            Declaraciones.ListaClases[(int)Declaraciones.eClass.Hunter] = "Cazador";
            Declaraciones.ListaClases[(int)Declaraciones.eClass.Worker] = "Trabajador";
            Declaraciones.ListaClases[(int)Declaraciones.eClass.Pirat] = "Pirata";

            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Magia] = "Magia";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Robar] = "Robar";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Tacticas] = "Evasión en combate";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Armas] = "Combate con armas";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Meditar] = "Meditar";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Apuñalar] = "Apuñalar";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Ocultarse] = "Ocultarse";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Supervivencia] = "Supervivencia";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Talar] = "Talar";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Comerciar] = "Comercio";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Defensa] = "Defensa con escudos";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Pesca] = "Pesca";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Mineria] = "Mineria";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Carpinteria] = "Carpinteria";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Herreria] = "Herreria";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Liderazgo] = "Liderazgo";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Domar] = "Domar animales";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Proyectiles] = "Combate a distancia";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Wrestling] = "Combate sin armas";
            Declaraciones.SkillsNames[(int)Declaraciones.eSkill.Navegacion] = "Navegacion";

            Declaraciones.ListaAtributos[(int)Declaraciones.eAtributos.Fuerza] = "Fuerza";
            Declaraciones.ListaAtributos[(int)Declaraciones.eAtributos.Agilidad] = "Agilidad";
            Declaraciones.ListaAtributos[(int)Declaraciones.eAtributos.Inteligencia] = "Inteligencia";
            Declaraciones.ListaAtributos[(int)Declaraciones.eAtributos.Carisma] = "Carisma";
            Declaraciones.ListaAtributos[(int)Declaraciones.eAtributos.Constitucion] = "Constitucion";

            Declaraciones.IniPath = AppDomain.CurrentDomain.BaseDirectory;
            Declaraciones.CharPath = AppDomain.CurrentDomain.BaseDirectory + "Charfile/";

            // Bordes del mapa
            Declaraciones.MinXBorder = Declaraciones.XMinMapSize + Declaraciones.XWindow / 2;
            Declaraciones.MaxXBorder = Declaraciones.XMaxMapSize - Declaraciones.XWindow / 2;
            Declaraciones.MinYBorder = Declaraciones.YMinMapSize + Declaraciones.YWindow / 2;
            Declaraciones.MaxYBorder = Declaraciones.YMaxMapSize - Declaraciones.YWindow / 2;

            modGuilds.LoadGuildsDB();


            ES.CargarSpawnList();
            ES.CargarForbidenWords();
            // ¿?¿?¿?¿?¿?¿?¿?¿ CARGAMOS DATOS DESDE ARCHIVOS ¿??¿?¿?¿?¿?¿?¿?¿

            Declaraciones.MaxUsers = 0;
            ES.LoadSini();
            ES.CargaApuestas();
            CargaNpcsDat();
            ES.LoadOBJData();
            ES.CargarHechizos();
            ES.LoadArmasHerreria();
            ES.LoadArmadurasHerreria();
            ES.LoadObjCarpintero();
            ES.LoadBalance(); // 4/01/08 Pablo ToxicWaste
            ES.LoadArmadurasFaccion();

            if (Admin.BootDelBackUp != 0)
                ES.CargarBackUp();
            else
                ES.LoadMapData();


            Declaraciones.SonidosMapas.LoadSoundMapInfo();

            ES.generateMatrix(Declaraciones.MATRIX_INITIAL_MAP);

            // Comentado porque hay worldsave en ese mapa!
            // Call CrearClanPretoriano(MAPA_PRETORIANO, ALCOBA2_X, ALCOBA2_Y)
            // ¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿

            short LoopC;

            // Resetea las conexiones de los usuarios
            var loopTo = Declaraciones.MaxUsers;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                Declaraciones.UserList[LoopC].ConnID = -1;
                Declaraciones.UserList[LoopC].ConnIDValida = false;
                Declaraciones.UserList[LoopC].incomingData = new clsByteQueue();
                Declaraciones.UserList[LoopC].outgoingData = new clsByteQueue();
            }

            // Configuracion de los sockets

            SecurityIp.InitIpTables(1000);
            wskapiAO.IniciaWsApi(Admin.Puerto);

            // Log - ensure log file exists
            AppendLog("logs/Main.log", "");

            Admin.tInicioServer = Convert.ToInt32(Migration.GetTickCount());
            InicializaEstadisticas();

            Console.WriteLine("Server started!");

            GameLoop.DoGameLoop();

            GameLoop.CloseServer();
            Console.WriteLine("Server finalizado!");
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in Main: " + ex.StackTrace);
        }
    }

    public static string ReadField(short Pos, ref string Text, byte SepASCII)
    {
        string ReadFieldRet = default;
        // *****************************************************************
        // Gets a field from a string
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modify Date: 11/15/2004
        // Gets a field from a delimited string
        // *****************************************************************

        int i;
        var LastPos = default(int);
        var CurrentPos = default(int);
        string delimiter;

        delimiter = ((char)SepASCII).ToString();

        if (string.IsNullOrEmpty(Text))
        {
            ReadFieldRet = "";
            return ReadFieldRet;
        }

        var loopTo = (int)Pos;
        for (i = 1; i <= loopTo; i++)
        {
            LastPos = CurrentPos;
            var idx = Text.IndexOf(delimiter, LastPos, StringComparison.Ordinal);
            CurrentPos = idx >= 0 ? idx + 1 : 0;
        }

        if (CurrentPos == 0)
            ReadFieldRet = Text.Substring(LastPos);
        else
            ReadFieldRet = Text.Substring(LastPos, CurrentPos - LastPos - 1);

        return ReadFieldRet;
    }

    public static bool MapaValido(short Map)
    {
        bool MapaValidoRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        MapaValidoRet = (Map >= 1) & (Map <= Declaraciones.NumMaps);
        return MapaValidoRet;
    }

    public static void MostrarNumUsers()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************
        // TODO MIGRA: completar si veo que es necesario
    }


    public static void LogCriticEvent(ref string desc)
    {
        AppendLog("logs/Eventos.log",
            DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " + desc);
    }

    public static void LogEjercitoReal(ref string desc)
    {
        AppendLog("logs/EjercitoReal.log", desc);
    }

    public static void LogEjercitoCaos(ref string desc)
    {
        AppendLog("logs/EjercitoCaos.log", desc);
    }


    public static void LogIndex(short Index, string desc)
    {
        AppendLog("logs/" + Index + ".log",
            DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " + desc);
    }


    public static void LogError(ref string desc)
    {
        AppendLog("logs/errores.log",
            DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " + desc);
    }

    public static void LogStatic(ref string desc)
    {
        AppendLog("logs/Stats.log",
            DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " + desc);
    }

    public static void LogTarea(ref string desc)
    {
        AppendLog("logs/haciendo.log",
            DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " + desc);
    }


    public static void LogClanes(string str)
    {
        AppendLog("logs/clanes.log",
            DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " +
            str);
    }

    public static void LogIP(string str)
    {
        AppendLog("logs/IP.log",
            DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " +
            str);
    }


    public static void LogDesarrollo(string str)
    {
        AppendLog(
            "logs/desarrollo" + Thread.CurrentThread.CurrentCulture.Calendar.GetMonth(DateTime.Today) +
            Thread.CurrentThread.CurrentCulture.Calendar.GetYear(DateTime.Today) + ".log",
            DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " +
            str);
    }

    public static void LogGM(ref string Nombre, ref string texto)
    {
        AppendLog("logs/" + Nombre + ".log",
            DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " + texto);
    }

    public static void LogAsesinato(ref string texto)
    {
        AppendLog("logs/asesinatos.log",
            DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " + texto);
    }

    public static void logVentaCasa(string texto)
    {
        AppendLog("logs/propiedades.log", "----------------------------------------------------------");
        AppendLog("logs/propiedades.log",
            DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " + texto);
        AppendLog("logs/propiedades.log", "----------------------------------------------------------");
    }

    public static void LogHackAttemp(ref string texto)
    {
        AppendLog("logs/HackAttemps.log", "----------------------------------------------------------");
        AppendLog("logs/HackAttemps.log",
            DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " + texto);
        AppendLog("logs/HackAttemps.log", "----------------------------------------------------------");
    }

    public static void LogCheating(ref string texto)
    {
        AppendLog("logs/CH.log",
            DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " + texto);
    }


    public static void LogCriticalHackAttemp(ref string texto)
    {
        AppendLog("logs/CriticalHackAttemps.log", "----------------------------------------------------------");
        AppendLog("logs/CriticalHackAttemps.log",
            DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " + texto);
        AppendLog("logs/CriticalHackAttemps.log", "----------------------------------------------------------");
    }

    public static void LogAntiCheat(ref string texto)
    {
        AppendLog("logs/AntiCheat.log",
            DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() + " " + texto);
        AppendLog("logs/AntiCheat.log", "");
    }

    public static bool ValidInputNP(string cad)
    {
        bool ValidInputNPRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        string Arg;
        short i;


        for (i = 1; i <= 33; i++)
        {
            Arg = ReadField(i, ref cad, 44);

            if (Migration.migr_LenB(Arg) == 0)
                return ValidInputNPRet;
        }

        ValidInputNPRet = true;
        return ValidInputNPRet;
    }


    public static void Restart()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // Se asegura de que los sockets estan cerrados e ignora cualquier err
        try
        {
            int LoopC;

            // Inicia el socket de escucha
            wskapiAO.IniciaWsApi(Admin.Puerto);

            // Initialize statistics!!
            Statistics.Initialize();

            var loopTo = Declaraciones.UserList.Length - 1;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                // UPGRADE_NOTE: El objeto UserList().incomingData no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
                Declaraciones.UserList[LoopC].incomingData = null;
                // UPGRADE_NOTE: El objeto UserList().outgoingData no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
                Declaraciones.UserList[LoopC].outgoingData = null;
            }

            // UPGRADE_WARNING: Es posible que la matriz UserList necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
            // UPGRADE_WARNING: El límite inferior de la matriz UserList ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            Declaraciones.UserList = new Declaraciones.User[Declaraciones.MaxUsers + 1];
            ArrayInitializers.InitializeStruct(Declaraciones.UserList);

            var loopTo1 = (int)Declaraciones.MaxUsers;
            for (LoopC = 1; LoopC <= loopTo1; LoopC++)
            {
                Declaraciones.UserList[LoopC].ConnID = -1;
                Declaraciones.UserList[LoopC].ConnIDValida = false;
                Declaraciones.UserList[LoopC].incomingData = new clsByteQueue();
                Declaraciones.UserList[LoopC].outgoingData = new clsByteQueue();
            }

            Declaraciones.LastUser = 0;
            Declaraciones.NumUsers = 0;

            FreeNPCs();
            FreeCharIndexes();

            ES.LoadSini();

            modForum.ResetForums();
            ES.LoadOBJData();

            ES.LoadMapData();

            ES.CargarHechizos();

            // Log it
            AppendLog("logs/Main.log",
                DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() +
                " servidor reiniciado.");
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in ReadField: " + ex.Message);
        }
    }


    public static bool Intemperie(short UserIndex)
    {
        bool IntemperieRet = default;
        // **************************************************************
        // Author: Unknown
        // Last Modify Date: 15/11/2009
        // 15/11/2009: ZaMa - La lluvia no quita stamina en las arenas.
        // 23/11/2009: ZaMa - Optimizacion de codigo.
        // **************************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (Declaraciones.mapInfo[withBlock.Pos.Map].Zona != "DUNGEON")
            {
                if (((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger != 1) &
                    ((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger != 2) &
                    ((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger != 4))
                    IntemperieRet = true;
            }
            else
            {
                IntemperieRet = false;
            }
        }

        // En las arenas no te afecta la lluvia
        if (UsUaRiOs.IsArena(UserIndex))
            IntemperieRet = false;
        return IntemperieRet;
    }

    public static void EfectoLluvia(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            int modifi;
            if (Declaraciones.UserList[UserIndex].flags.UserLogged)
                if (Intemperie(UserIndex))
                {
                    modifi = Matematicas.Porcentaje(Declaraciones.UserList[UserIndex].Stats.MaxSta, 3);
                    Trabajo.QuitarSta(UserIndex, Convert.ToInt16(modifi));
                    Protocol.FlushBuffer(UserIndex);
                }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in ValidInputNP: " + ex.Message);
        }
    }

    public static void TiempoInvocacion(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short i;
        for (i = 1; i <= Declaraciones.MAXMASCOTAS; i++)
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.MascotasIndex[i] > 0)
                if (Declaraciones.Npclist[withBlock.MascotasIndex[i]].Contadores.TiempoExistencia > 0)
                {
                    Declaraciones.Npclist[withBlock.MascotasIndex[i]].Contadores.TiempoExistencia =
                        Declaraciones.Npclist[withBlock.MascotasIndex[i]].Contadores.TiempoExistencia - 1;
                    if (Declaraciones.Npclist[withBlock.MascotasIndex[i]].Contadores.TiempoExistencia == 0)
                        NPCs.MuereNpc(withBlock.MascotasIndex[i], 0);
                }
        }
    }

    public static void EfectoFrio(short UserIndex)
    {
        // ***************************************************
        // Autor: Unkonwn
        // Last Modification: 23/11/2009
        // If user is naked and it's in a cold map, take health points from him
        // 23/11/2009: ZaMa - Optimizacion de codigo.
        // ***************************************************
        short modifi;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.Counters.Frio < Admin.IntervaloFrio)
            {
                withBlock.Counters.Frio = Convert.ToInt16(withBlock.Counters.Frio + 1);
            }
            else
            {
                if ((Declaraciones.mapInfo[withBlock.Pos.Map].Terreno ?? "") == Declaraciones.Nieve)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "¡¡Estás muriendo de frío, abrigate o morirás!!",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    modifi = Convert.ToInt16(Matematicas.Porcentaje(withBlock.Stats.MaxHp, 5));
                    withBlock.Stats.MinHp = (short)(withBlock.Stats.MinHp - modifi);

                    if (withBlock.Stats.MinHp < 1)
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "¡¡Has muerto de frío!!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        withBlock.Stats.MinHp = 0;
                        UsUaRiOs.UserDie(UserIndex);
                    }

                    Protocol.WriteUpdateHP(UserIndex);
                }
                else
                {
                    modifi = Convert.ToInt16(Matematicas.Porcentaje(withBlock.Stats.MaxSta, 5));
                    Trabajo.QuitarSta(UserIndex, modifi);
                    Protocol.WriteUpdateSta(UserIndex);
                }

                withBlock.Counters.Frio = 0;
            }
        }
    }

    public static void EfectoLava(short UserIndex)
    {
        // ***************************************************
        // Autor: Nacho (Integer)
        // Last Modification: 23/11/2009
        // If user is standing on lava, take health points from him
        // 23/11/2009: ZaMa - Optimizacion de codigo.
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.Counters.Lava < Admin.IntervaloFrio) // Usamos el mismo intervalo que el del frio
            {
                withBlock.Counters.Lava = Convert.ToInt16(withBlock.Counters.Lava + 1);
            }
            else
            {
                if (HayLava(withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y))
                {
                    Protocol.WriteConsoleMsg(UserIndex, "¡¡Quitate de la lava, te estás quemando!!",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    withBlock.Stats.MinHp = (short)(withBlock.Stats.MinHp -
                                                    Convert.ToInt16(Matematicas.Porcentaje(withBlock.Stats.MaxHp, 5)));

                    if (withBlock.Stats.MinHp < 1)
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "¡¡Has muerto quemado!!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        withBlock.Stats.MinHp = 0;
                        UsUaRiOs.UserDie(UserIndex);
                    }

                    Protocol.WriteUpdateHP(UserIndex);
                }

                withBlock.Counters.Lava = 0;
            }
        }
    }

    // '
    // Maneja  el efecto del estado atacable
    // 
    // @param UserIndex  El index del usuario a ser afectado por el estado atacable
    // 

    public static void EfectoEstadoAtacable(short UserIndex)
    {
        // ******************************************************
        // Author: ZaMa
        // Last Update: 13/01/2010 (ZaMa)
        // ******************************************************

        // Si ya paso el tiempo de penalizacion
        if (!modNuevoTimer.IntervaloEstadoAtacable(UserIndex))
        {
            // Deja de poder ser atacado
            Declaraciones.UserList[UserIndex].flags.AtacablePor = 0;
            // Send nick normal
            UsUaRiOs.RefreshCharStatus(UserIndex);
        }
    }

    // '
    // Maneja el tiempo y el efecto del mimetismo
    // 
    // @param UserIndex  El index del usuario a ser afectado por el mimetismo
    // 

    public static void EfectoMimetismo(short UserIndex)
    {
        // ******************************************************
        // Author: Unknown
        // Last Update: 12/01/2010 (ZaMa)
        // 12/01/2010: ZaMa - Los druidas pierden la inmunidad de ser atacados cuando pierden el efecto del mimetismo.
        // ******************************************************
        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Barco, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Declaraciones.ObjData Barco;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.Counters.Mimetismo < Admin.IntervaloInvisible)
            {
                withBlock.Counters.Mimetismo = Convert.ToInt16(withBlock.Counters.Mimetismo + 1);
            }
            else
            {
                // restore old char
                Protocol.WriteConsoleMsg(UserIndex, "Recuperas tu apariencia normal.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);

                if (withBlock.flags.Navegando != 0)
                {
                    if (withBlock.flags.Muerto == 0)
                    {
                        if (withBlock.Faccion.ArmadaReal == 1)
                        {
                            withBlock.character.body = Declaraciones.iFragataReal;
                        }
                        else if (withBlock.Faccion.FuerzasCaos == 1)
                        {
                            withBlock.character.body = Declaraciones.iFragataCaos;
                        }
                        else
                        {
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Barco. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            Barco = Declaraciones.objData[
                                Declaraciones.UserList[UserIndex].Invent.BarcoObjIndex];
                            if (ES.criminal(UserIndex))
                            {
                                if (Barco.Ropaje == Declaraciones.iBarca)
                                    withBlock.character.body = Declaraciones.iBarcaPk;
                                if (Barco.Ropaje == Declaraciones.iGalera)
                                    withBlock.character.body = Declaraciones.iGaleraPk;
                                if (Barco.Ropaje == Declaraciones.iGaleon)
                                    withBlock.character.body = Declaraciones.iGaleonPk;
                            }
                            else
                            {
                                if (Barco.Ropaje == Declaraciones.iBarca)
                                    withBlock.character.body = Declaraciones.iBarcaCiuda;
                                if (Barco.Ropaje == Declaraciones.iGalera)
                                    withBlock.character.body = Declaraciones.iGaleraCiuda;
                                if (Barco.Ropaje == Declaraciones.iGaleon)
                                    withBlock.character.body = Declaraciones.iGaleonCiuda;
                            }
                        }
                    }
                    else
                    {
                        withBlock.character.body = Declaraciones.iFragataFantasmal;
                    }

                    withBlock.character.ShieldAnim = Declaraciones.NingunEscudo;
                    withBlock.character.WeaponAnim = Declaraciones.NingunArma;
                    withBlock.character.CascoAnim = Declaraciones.NingunCasco;
                }
                else
                {
                    withBlock.character.body = withBlock.CharMimetizado.body;
                    withBlock.character.Head = withBlock.CharMimetizado.Head;
                    withBlock.character.CascoAnim = withBlock.CharMimetizado.CascoAnim;
                    withBlock.character.ShieldAnim = withBlock.CharMimetizado.ShieldAnim;
                    withBlock.character.WeaponAnim = withBlock.CharMimetizado.WeaponAnim;
                }

                {
                    ref var withBlock1 = ref withBlock.character;
                    UsUaRiOs.ChangeUserChar(UserIndex, withBlock1.body, withBlock1.Head, (byte)withBlock1.heading,
                        withBlock1.WeaponAnim, withBlock1.ShieldAnim, withBlock1.CascoAnim);
                }

                withBlock.Counters.Mimetismo = 0;
                withBlock.flags.Mimetizado = 0;
                // Se fue el efecto del mimetismo, puede ser atacado por npcs
                withBlock.flags.Ignorado = false;
            }
        }
    }

    public static void EfectoInvisibilidad(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.Counters.Invisibilidad < Admin.IntervaloInvisible)
            {
                withBlock.Counters.Invisibilidad = Convert.ToInt16(withBlock.Counters.Invisibilidad + 1);
            }
            else
            {
                withBlock.Counters.Invisibilidad =
                    Convert.ToInt16(Matematicas.RandomNumber(-100, 100)); // Invi variable :D
                withBlock.flags.invisible = 0;
                if (withBlock.flags.Oculto == 0)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Has vuelto a ser visible.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    UsUaRiOs.SetInvisible(UserIndex, withBlock.character.CharIndex, false);
                    // Call SendData(SendTarget.ToPCArea, UserIndex, PrepareMessageSetInvisible(.Char.CharIndex, False))
                }
            }
        }
    }


    public static void EfectoParalisisNpc(short NpcIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
            if (withBlock.Contadores.Paralisis > 0)
            {
                withBlock.Contadores.Paralisis = Convert.ToInt16(withBlock.Contadores.Paralisis - 1);
            }
            else
            {
                withBlock.flags.Paralizado = 0;
                withBlock.flags.Inmovilizado = 0;
            }
        }
    }

    public static void EfectoCegueEstu(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.Counters.Ceguera > 0)
            {
                withBlock.Counters.Ceguera = Convert.ToInt16(withBlock.Counters.Ceguera - 1);
            }
            else
            {
                if (withBlock.flags.Ceguera == 1)
                {
                    withBlock.flags.Ceguera = 0;
                    Protocol.WriteBlindNoMore(UserIndex);
                }

                if (withBlock.flags.Estupidez == 1)
                {
                    withBlock.flags.Estupidez = 0;
                    Protocol.WriteDumbNoMore(UserIndex);
                }
            }
        }
    }


    public static void EfectoParalisisUser(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.Counters.Paralisis > 0)
            {
                withBlock.Counters.Paralisis = Convert.ToInt16(withBlock.Counters.Paralisis - 1);
            }
            else
            {
                withBlock.flags.Paralizado = 0;
                withBlock.flags.Inmovilizado = 0;
                // .Flags.AdministrativeParalisis = 0
                Protocol.WriteParalizeOK(UserIndex);
            }
        }
    }

    public static void RecStamina(short UserIndex, ref bool EnviarStats, short Intervalo)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short massta;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger == 1) &
                ((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger == 2) &
                ((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger == 4))
                return;


            if (withBlock.Stats.MinSta < withBlock.Stats.MaxSta)
            {
                if (withBlock.Counters.STACounter < Intervalo)
                {
                    withBlock.Counters.STACounter = Convert.ToInt16(withBlock.Counters.STACounter + 1);
                }
                else
                {
                    EnviarStats = true;
                    withBlock.Counters.STACounter = 0;
                    if (withBlock.flags.Desnudo != 0)
                        return; // Desnudo no sube energía. (ToxicWaste)

                    massta = Convert.ToInt16(Matematicas.RandomNumber(1,
                        Matematicas.Porcentaje(withBlock.Stats.MaxSta, 5)));
                    withBlock.Stats.MinSta = (short)(withBlock.Stats.MinSta + massta);
                    if (withBlock.Stats.MinSta > withBlock.Stats.MaxSta)
                        withBlock.Stats.MinSta = withBlock.Stats.MaxSta;
                }
            }
        }
    }

    public static void EfectoVeneno(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short N;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.Counters.Veneno < Admin.IntervaloVeneno)
            {
                withBlock.Counters.Veneno = Convert.ToInt16(withBlock.Counters.Veneno + 1);
            }
            else
            {
                Protocol.WriteConsoleMsg(UserIndex, "Estás envenenado, si no te curas morirás.",
                    Protocol.FontTypeNames.FONTTYPE_VENENO);
                withBlock.Counters.Veneno = 0;
                N = Convert.ToInt16(Matematicas.RandomNumber(1, 5));
                withBlock.Stats.MinHp = (short)(withBlock.Stats.MinHp - N);
                if (withBlock.Stats.MinHp < 1)
                    UsUaRiOs.UserDie(UserIndex);
                Protocol.WriteUpdateHP(UserIndex);
            }
        }
    }

    public static void DuracionPociones(short UserIndex)
    {
        // ***************************************************
        // Author: ??????
        // Last Modification: 11/27/09 (Budi)
        // Cuando se pierde el efecto de la poción updatea fz y agi (No me gusta que ambos atributos aunque se haya modificado solo uno, pero bueno :p)
        // ***************************************************
        short loopX;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Controla la duracion de las pociones
            if (withBlock.flags.DuracionEfecto > 0)
            {
                withBlock.flags.DuracionEfecto = withBlock.flags.DuracionEfecto - 1;
                if (withBlock.flags.DuracionEfecto == 0)
                {
                    withBlock.flags.TomoPocion = false;
                    withBlock.flags.TipoPocion = 0;
                    // volvemos los atributos al estado normal

                    for (loopX = 1; loopX <= Declaraciones.NUMATRIBUTOS; loopX++)
                        withBlock.Stats.UserAtributos[loopX] = withBlock.Stats.UserAtributosBackUP[loopX];

                    Protocol.WriteUpdateStrenghtAndDexterity(UserIndex);
                }
            }
        }
    }

    public static void HambreYSed(short UserIndex, ref bool fenviarAyS)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (!(withBlock.flags.Privilegios != Declaraciones.PlayerType.User))
                return;

            // Sed
            if (withBlock.Stats.MinAGU > 0)
            {
                if (withBlock.Counters.AGUACounter < Admin.IntervaloSed)
                {
                    withBlock.Counters.AGUACounter = Convert.ToInt16(withBlock.Counters.AGUACounter + 1);
                }
                else
                {
                    withBlock.Counters.AGUACounter = 0;
                    withBlock.Stats.MinAGU = Convert.ToInt16(withBlock.Stats.MinAGU - 10);

                    if (withBlock.Stats.MinAGU <= 0)
                    {
                        withBlock.Stats.MinAGU = 0;
                        withBlock.flags.Sed = 1;
                    }

                    fenviarAyS = true;
                }
            }

            // hambre
            if (withBlock.Stats.MinHam > 0)
            {
                if (withBlock.Counters.COMCounter < Admin.IntervaloHambre)
                {
                    withBlock.Counters.COMCounter = Convert.ToInt16(withBlock.Counters.COMCounter + 1);
                }
                else
                {
                    withBlock.Counters.COMCounter = 0;
                    withBlock.Stats.MinHam = Convert.ToInt16(withBlock.Stats.MinHam - 10);
                    if (withBlock.Stats.MinHam <= 0)
                    {
                        withBlock.Stats.MinHam = 0;
                        withBlock.flags.Hambre = 1;
                    }

                    fenviarAyS = true;
                }
            }
        }
    }

    public static void Sanar(short UserIndex, ref bool EnviarStats, short Intervalo)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short mashit;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger == 1) &
                ((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger == 2) &
                ((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger == 4))
                return;

            // con el paso del tiempo va sanando....pero muy lentamente ;-)
            if (withBlock.Stats.MinHp < withBlock.Stats.MaxHp)
            {
                if (withBlock.Counters.HPCounter < Intervalo)
                {
                    withBlock.Counters.HPCounter = Convert.ToInt16(withBlock.Counters.HPCounter + 1);
                }
                else
                {
                    mashit = Convert.ToInt16(Matematicas.RandomNumber(2,
                        Matematicas.Porcentaje(withBlock.Stats.MaxSta, 5)));

                    withBlock.Counters.HPCounter = 0;
                    withBlock.Stats.MinHp = (short)(withBlock.Stats.MinHp + mashit);
                    if (withBlock.Stats.MinHp > withBlock.Stats.MaxHp)
                        withBlock.Stats.MinHp = withBlock.Stats.MaxHp;
                    Protocol.WriteConsoleMsg(UserIndex, "Has sanado.", Protocol.FontTypeNames.FONTTYPE_INFO);
                    EnviarStats = true;
                }
            }
        }
    }

    public static void CargaNpcsDat()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        string npcfile;

        npcfile = Declaraciones.DatPath + "NPCs.dat";
        LeerNPCs.Initialize(npcfile);
    }

    public static void PasarSegundo()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        var i = default(int);
        try
        {
            var loopTo = (int)Declaraciones.LastUser;
            for (i = 1; i <= loopTo; i++)
                if (Declaraciones.UserList[i].flags.UserLogged)
                    // Cerrar usuario
                    if (Declaraciones.UserList[i].Counters.Saliendo)
                    {
                        Declaraciones.UserList[i].Counters.salir =
                            Convert.ToInt16(Declaraciones.UserList[i].Counters.salir - 1);
                        if (Declaraciones.UserList[i].Counters.salir <= 0)
                        {
                            Protocol.WriteConsoleMsg(Convert.ToInt16(i), "Gracias por jugar Argentum Online",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            Protocol.WriteDisconnect(Convert.ToInt16(i));
                            Protocol.FlushBuffer(Convert.ToInt16(i));

                            TCP.CloseSocket(Convert.ToInt16(i));
                        }
                    }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in EfectoInvisibilidad: " + ex.Message);
            var argdesc = "Error en PasarSegundo. Err: " + ex.Message + " - " + ex.GetType().Name + " - UserIndex: " +
                          i;
            LogError(ref argdesc);
        }
    }

    public static double ReiniciarAutoUpdate()
    {
        double ReiniciarAutoUpdateRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = AppDomain.CurrentDomain.BaseDirectory + "autoupdater/aoau.exe",
            WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized,
            UseShellExecute = true
        });
        ReiniciarAutoUpdateRet = process?.Id ?? 0;
        return ReiniciarAutoUpdateRet;
    }

    public static void ReiniciarServidor(bool EjecutarLauncher = true)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // WorldSave
        ES.DoBackUp();

        // commit experiencias
        mdParty.ActualizaExperiencias();

        // Guardar Pjs
        GuardarUsuarios();

        if (EjecutarLauncher)
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = AppDomain.CurrentDomain.BaseDirectory + "/launcher.exe",
                UseShellExecute = true
            });
    }


    public static void GuardarUsuarios()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        Declaraciones.haciendoBK = true;

        modSendData.SendData(modSendData.SendTarget.ToAll, 0, Protocol.PrepareMessagePauseToggle());
        modSendData.SendData(modSendData.SendTarget.ToAll, 0,
            Protocol.PrepareMessageConsoleMsg("Servidor> Grabando Personajes", Protocol.FontTypeNames.FONTTYPE_SERVER));

        short i;
        var loopTo = Declaraciones.LastUser;
        for (i = 1; i <= loopTo; i++)
            if (Declaraciones.UserList[i].flags.UserLogged)
                ES.SaveUser(i, Declaraciones.CharPath + Declaraciones.UserList[i].name.ToUpper() + ".chr");

        modSendData.SendData(modSendData.SendTarget.ToAll, 0,
            Protocol.PrepareMessageConsoleMsg("Servidor> Personajes Grabados", Protocol.FontTypeNames.FONTTYPE_SERVER));
        modSendData.SendData(modSendData.SendTarget.ToAll, 0, Protocol.PrepareMessagePauseToggle());

        Declaraciones.haciendoBK = false;
    }


    public static void InicializaEstadisticas()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int Ta;
        Ta = Convert.ToInt32(Migration.GetTickCount());

        Admin.EstadisticasWeb.Inicializa();
        Admin.EstadisticasWeb.Informar(clsEstadisticasIPC.EstaNotificaciones.CANTIDAD_MAPAS, Declaraciones.NumMaps);
        Admin.EstadisticasWeb.Informar(clsEstadisticasIPC.EstaNotificaciones.CANTIDAD_ONLINE, Declaraciones.NumUsers);
        Admin.EstadisticasWeb.Informar(clsEstadisticasIPC.EstaNotificaciones.UPTIME_SERVER,
            Convert.ToInt32((Ta - Admin.tInicioServer) / 1000d));
        Admin.EstadisticasWeb.Informar(clsEstadisticasIPC.EstaNotificaciones.RECORD_USUARIOS,
            Declaraciones.recordusuarios);
    }

    public static void FreeNPCs()
    {
        // ***************************************************
        // Autor: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Releases all NPC Indexes
        // ***************************************************
        int LoopC;

        // Free all NPC indexes
        for (LoopC = 1; LoopC <= Declaraciones.MAXNPCS; LoopC++)
            Declaraciones.Npclist[LoopC].flags.NPCActive = false;
    }

    public static void FreeCharIndexes()
    {
        // ***************************************************
        // Autor: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Releases all char indexes
        // ***************************************************
        // Free all char indexes (set them all to 0)
        // TODO MIGRA: antes se usaba ZeroMemory. Quizas como esta ahora no sea del todo performante, pero no estoy seguro
        short i;
        for (i = 1; i <= Declaraciones.MAXCHARS; i++)
            Declaraciones.CharList[i] = 0;
    }
}