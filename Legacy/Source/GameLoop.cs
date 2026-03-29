using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
namespace Legacy;

internal static class GameLoop
{
    // TODO: verificar si el servidor se esta cerrando correctamente!!
    private static bool running = true;

    private static readonly int piqueteCInterval = 6000;
    private static readonly int packetResendInterval = 10;
    private static readonly int fxInterval = 4000;
    private static readonly int auditoriaInterval = 1000;
    private static readonly int gameTimerInterval = 40;
    private static readonly int lluviaEventInterval = 60000;
    public static int lluviaInterval = 500;
    private static readonly int autoSaveInterval = 60000;
    public static int npcAtacaInterval = 4000;
    private static readonly int killLogInterval = 60000;
    public static int timerAIInterval = 100;
    private static readonly int connectionTimerInterval = 5;

    // Últimos tiempos de ejecución
    private static double lastPiqueteC;
    private static double lastPacketResend;
    private static double lastFx;
    private static double lastAuditoria;
    private static double lastGameTimer;
    private static double lastLluviaEvent;
    private static double lastLluvia;
    private static double lastAutoSave;
    private static double lastNpcAtaca;
    private static double lastKillLog;
    private static double lastTimerAI;
    private static double lastConnection;

    private static byte _TickAuditoria_centinelSecs;

    private static int _TickLluviaEvent_MinutosLloviendo;
    private static int _TickLluviaEvent_MinutosSinLluvia;

    private static int _TickAutoSave_Minutos;
    private static int _TickAutoSave_MinutosLatsClean;

    private static void OnExit(object sender, EventArgs e)
    {
        Console.WriteLine("Evento: ProcessExit");
        running = false;
    }

    private static void OnCancel(object sender, ConsoleCancelEventArgs e)
    {
        Console.WriteLine("Evento: CancelKeyPress");
        e.Cancel = true; // Previene salida inmediata
        running = false;
    }

    public static void DoGameLoop()
    {
        AppDomain.CurrentDomain.ProcessExit += OnExit;
        Console.CancelKeyPress += OnCancel;

        var sw = Stopwatch.StartNew();

        while (running)
        {
            var now = sw.Elapsed.TotalMilliseconds;

            if (now - lastPiqueteC >= piqueteCInterval)
            {
                TickPiqueteC();
                lastPiqueteC = now;
            }

            if (now - lastPacketResend >= packetResendInterval)
            {
                TickPacketResend();
                lastPacketResend = now;
            }

            if (now - lastFx >= fxInterval)
            {
                TickFx();
                lastFx = now;
            }

            if (now - lastAuditoria >= auditoriaInterval)
            {
                TickAuditoria();
                lastAuditoria = now;
            }

            if (now - lastGameTimer >= gameTimerInterval)
            {
                TickGameTimer();
                lastGameTimer = now;
            }

            if (now - lastLluviaEvent >= lluviaEventInterval)
            {
                TickLluviaEvent();
                lastLluviaEvent = now;
            }

            if (now - lastLluvia >= lluviaInterval)
            {
                TickLluvia();
                lastLluvia = now;
            }

            if (now - lastAutoSave >= autoSaveInterval)
            {
                TickAutoSave();
                lastAutoSave = now;
            }

            if (now - lastNpcAtaca >= npcAtacaInterval)
            {
                TickNpcAtaca();
                lastNpcAtaca = now;
            }

            if (now - lastKillLog >= killLogInterval)
            {
                TickKillLog();
                lastKillLog = now;
            }

            if (now - lastTimerAI >= timerAIInterval)
            {
                TickAI();
                lastTimerAI = now;
            }

            if (now - lastConnection >= connectionTimerInterval)
            {
                TickConnection();
                lastConnection = now;
            }

            // Pausa mínima para evitar uso excesivo de CPU
            Thread.Sleep(1);
        }
    }

    // =====================
    // Subrutinas de ticks
    // =====================

    private static void TickPiqueteC()
    {
        bool NuevaA;
        // Dim NuevoL As Boolean
        short GI;

        short i;

        try
        {
            var loopTo = Declaraciones.LastUser;
            for (i = 1; i <= loopTo; i++)
            {
                ref var withBlock = ref Declaraciones.UserList[i];
                if (withBlock.flags.UserLogged)
                {
                    if (Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger ==
                        Declaraciones.eTrigger.ANTIPIQUETE)
                    {
                        withBlock.Counters.PiqueteC = withBlock.Counters.PiqueteC + 1;
                        Protocol.WriteConsoleMsg(i,
                            "¡¡¡Estás obstruyendo la vía pública, muévete o serás encarcelado!!!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);

                        if (withBlock.Counters.PiqueteC > 23)
                        {
                            withBlock.Counters.PiqueteC = 0;
                            Admin.Encarcelar(i, Declaraciones.TIEMPO_CARCEL_PIQUETE);
                        }
                    }
                    else
                    {
                        withBlock.Counters.PiqueteC = 0;
                    }

                    if (withBlock.flags.Muerto == 1)
                        if (withBlock.flags.Traveling == 1)
                        {
                            if (withBlock.Counters.goHome <= 0)
                            {
                                Extra.FindLegalPos(i, Declaraciones.Ciudades[(int)withBlock.Hogar].Map,
                                    ref Declaraciones.Ciudades[(int)withBlock.Hogar].X,
                                    ref Declaraciones.Ciudades[(int)withBlock.Hogar].Y);
                                UsUaRiOs.WarpUserChar(i, Declaraciones.Ciudades[(int)withBlock.Hogar].Map,
                                    Declaraciones.Ciudades[(int)withBlock.Hogar].X,
                                    Declaraciones.Ciudades[(int)withBlock.Hogar].Y, true);
                                Protocol.WriteMultiMessage(i, (short)Declaraciones.eMessages.FinishHome);
                                withBlock.flags.Traveling = 0;
                            }
                            else
                            {
                                withBlock.Counters.goHome = withBlock.Counters.goHome - 1;
                            }
                        }

                    // ustedes se preguntaran que hace esto aca?
                    // bueno la respuesta es simple: el codigo de AO es una mierda y encontrar
                    // todos los puntos en los cuales la alineacion puede cambiar es un dolor de
                    // huevos, asi que lo controlo aca, cada 6 segundos, lo cual es razonable
                    GI = withBlock.GuildIndex;
                    if (GI > 0)
                    {
                        NuevaA = false;
                        // NuevoL = False
                        if (!modGuilds.m_ValidarPermanencia(i, true, ref NuevaA))
                            Protocol.WriteConsoleMsg(i,
                                "Has sido expulsado del clan. ¡El clan ha sumado un punto de antifacción!",
                                Protocol.FontTypeNames.FONTTYPE_GUILD);
                        if (NuevaA)
                        {
                            modSendData.SendData(modSendData.SendTarget.ToGuildMembers, GI,
                                Protocol.PrepareMessageConsoleMsg(
                                    "¡El clan ha pasado a tener alineación " + modGuilds.GuildAlignment(GI) + "!",
                                    Protocol.FontTypeNames.FONTTYPE_GUILD));
                            General.LogClanes("¡El clan cambio de alineación!");
                        }
                        // If NuevoL Then
                        // Call SendData(SendTarget.ToGuildMembers, GI, PrepareMessageConsoleMsg("¡El clan tiene un nuevo líder!", FontTypeNames.FONTTYPE_GUILD))
                        // Call LogClanes("¡El clan tiene nuevo lider!")
                        // End If
                    }

                    Protocol.FlushBuffer(i);
                }
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in OnExit: " + ex.Message);
            var argdesc = "Error en tPiqueteC_Timer " + ex.GetType().Name + ": " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    private static void TickPacketResend()
    {
        // ***************************************************
        // Autor: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 04/01/07
        // Attempts to resend to the user all data that may be enqueued.
        // ***************************************************
        try
        {
            short i;

            var loopTo = Declaraciones.MaxUsers;
            for (i = 1; i <= loopTo; i++)
                if (Declaraciones.UserList[i].ConnIDValida)
                    if (Declaraciones.UserList[i].outgoingData.length > 0)
                    {
                        var argdatos = Declaraciones.UserList[i].outgoingData
                            .ReadASCIIStringFixed(Declaraciones.UserList[i].outgoingData.length);
                        TCP.EnviarDatosASlot(i, ref argdatos);
                    }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in TickPacketResend: " + ex.Message);
            var argdesc = "Error en packetResend - Error: " + ex.GetType().Name + " - Desc: " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    private static void TickFx()
    {
        try
        {
            Declaraciones.SonidosMapas.ReproducirSonidosDeMapas();
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in TickFx: " + ex.Message);
        }
    }

    private static void TickAuditoria()
    {
        try
        {
            _TickAuditoria_centinelSecs = Convert.ToByte(_TickAuditoria_centinelSecs + 1);

            if (_TickAuditoria_centinelSecs == 5)
            {
                // Every 5 seconds, we try to call the player's attention so it will report the code.
                modCentinela.CallUserAttention();

                _TickAuditoria_centinelSecs = 0;
            }

            General.PasarSegundo(); // sistema de desconexion de 10 segs

            Admin.ActualizaEstadisticasWeb();
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in TickAuditoria: " + ex.Message);
            var argdesc = "Error en Timer Auditoria. Err: " + ex.Message + " - " + ex.GetType().Name;
            General.LogError(ref argdesc);
        }
    }

    private static void TickGameTimer()
    {
        // ********************************************************
        // Author: Unknown
        // Last Modify Date: -
        // ********************************************************
        var iUserIndex = default(short);
        bool bEnviarStats;
        bool bEnviarAyS;

        try
        {
            // <<<<<< Procesa eventos de los usuarios >>>>>>
            var loopTo = Declaraciones.MaxUsers;
            for (iUserIndex = 1; iUserIndex <= loopTo; iUserIndex++) // LastUser
            {
                ref var withBlock = ref Declaraciones.UserList[iUserIndex];
                // Conexion activa?
                if (withBlock.ConnID != -1)
                {
                    // ¿User valido?
                    if (withBlock.ConnIDValida & withBlock.flags.UserLogged)
                    {
                        // [Alejo-18-5]
                        bEnviarStats = false;
                        bEnviarAyS = false;

                        Extra.DoTileEvents(iUserIndex, withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y);


                        if (withBlock.flags.Paralizado == 1)
                            General.EfectoParalisisUser(iUserIndex);
                        if ((withBlock.flags.Ceguera == 1) | (withBlock.flags.Estupidez != 0))
                            General.EfectoCegueEstu(iUserIndex);


                        if (withBlock.flags.Muerto == 0)
                        {
                            // [Consejeros]
                            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                                General.EfectoLava(iUserIndex);

                            if ((withBlock.flags.Desnudo != 0) &
                                ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0))
                                General.EfectoFrio(iUserIndex);

                            if (withBlock.flags.Meditando)
                                Trabajo.DoMeditar(iUserIndex);

                            if ((withBlock.flags.Envenenado != 0) &
                                ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0))
                                General.EfectoVeneno(iUserIndex);

                            if (withBlock.flags.AdminInvisible != 1)
                            {
                                if (withBlock.flags.invisible == 1)
                                    General.EfectoInvisibilidad(iUserIndex);
                                if (withBlock.flags.Oculto == 1)
                                    Trabajo.DoPermanecerOculto(iUserIndex);
                            }

                            if (withBlock.flags.Mimetizado == 1)
                                General.EfectoMimetismo(iUserIndex);

                            if (withBlock.flags.AtacablePor > 0)
                                General.EfectoEstadoAtacable(iUserIndex);

                            General.DuracionPociones(iUserIndex);

                            General.HambreYSed(iUserIndex, ref bEnviarAyS);

                            if ((withBlock.flags.Hambre == 0) & (withBlock.flags.Sed == 0))
                            {
                                if (Admin.Lloviendo)
                                {
                                    if (!General.Intemperie(iUserIndex))
                                    {
                                        if (!withBlock.flags.Descansar)
                                        {
                                            // No esta descansando
                                            General.Sanar(iUserIndex, ref bEnviarStats,
                                                Admin.SanaIntervaloSinDescansar);
                                            if (bEnviarStats)
                                            {
                                                Protocol.WriteUpdateHP(iUserIndex);
                                                bEnviarStats = false;
                                            }

                                            General.RecStamina(iUserIndex, ref bEnviarStats,
                                                Admin.StaminaIntervaloSinDescansar);
                                            if (bEnviarStats)
                                            {
                                                Protocol.WriteUpdateSta(iUserIndex);
                                                bEnviarStats = false;
                                            }
                                        }
                                        else
                                        {
                                            // esta descansando
                                            General.Sanar(iUserIndex, ref bEnviarStats, Admin.SanaIntervaloDescansar);
                                            if (bEnviarStats)
                                            {
                                                Protocol.WriteUpdateHP(iUserIndex);
                                                bEnviarStats = false;
                                            }

                                            General.RecStamina(iUserIndex, ref bEnviarStats,
                                                Admin.StaminaIntervaloDescansar);
                                            if (bEnviarStats)
                                            {
                                                Protocol.WriteUpdateSta(iUserIndex);
                                                bEnviarStats = false;
                                            }

                                            // termina de descansar automaticamente
                                            if ((withBlock.Stats.MaxHp == withBlock.Stats.MinHp) &
                                                (withBlock.Stats.MaxSta == withBlock.Stats.MinSta))
                                            {
                                                Protocol.WriteRestOK(iUserIndex);
                                                Protocol.WriteConsoleMsg(iUserIndex, "Has terminado de descansar.",
                                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                                                withBlock.flags.Descansar = false;
                                            }
                                        }
                                    }
                                }
                                else if (!withBlock.flags.Descansar)
                                {
                                    // No esta descansando
                                    General.Sanar(iUserIndex, ref bEnviarStats, Admin.SanaIntervaloSinDescansar);
                                    if (bEnviarStats)
                                    {
                                        Protocol.WriteUpdateHP(iUserIndex);
                                        bEnviarStats = false;
                                    }

                                    General.RecStamina(iUserIndex, ref bEnviarStats,
                                        Admin.StaminaIntervaloSinDescansar);
                                    if (bEnviarStats)
                                    {
                                        Protocol.WriteUpdateSta(iUserIndex);
                                        bEnviarStats = false;
                                    }
                                }

                                else
                                {
                                    // esta descansando
                                    General.Sanar(iUserIndex, ref bEnviarStats, Admin.SanaIntervaloDescansar);
                                    if (bEnviarStats)
                                    {
                                        Protocol.WriteUpdateHP(iUserIndex);
                                        bEnviarStats = false;
                                    }

                                    General.RecStamina(iUserIndex, ref bEnviarStats, Admin.StaminaIntervaloDescansar);
                                    if (bEnviarStats)
                                    {
                                        Protocol.WriteUpdateSta(iUserIndex);
                                        bEnviarStats = false;
                                    }

                                    // termina de descansar automaticamente
                                    if ((withBlock.Stats.MaxHp == withBlock.Stats.MinHp) &
                                        (withBlock.Stats.MaxSta == withBlock.Stats.MinSta))
                                    {
                                        Protocol.WriteRestOK(iUserIndex);
                                        Protocol.WriteConsoleMsg(iUserIndex, "Has terminado de descansar.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO);
                                        withBlock.flags.Descansar = false;
                                    }
                                }
                            }

                            if (bEnviarAyS)
                                Protocol.WriteUpdateHungerAndThirst(iUserIndex);

                            if (withBlock.NroMascotas > 0)
                                General.TiempoInvocacion(iUserIndex);
                        } // Muerto
                    }
                    else // no esta logeado?
                    {
                        // Inactive players will be removed!
                        withBlock.Counters.IdleCount = withBlock.Counters.IdleCount + 1;
                        if (withBlock.Counters.IdleCount > Admin.IntervaloParaConexion)
                        {
                            withBlock.Counters.IdleCount = 0;
                            TCP.CloseSocket(iUserIndex);
                        }
                    } // UserLogged

                    // If there is anything to be sent, we send it
                    Protocol.FlushBuffer(iUserIndex);
                }
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in TickGameTimer: " + ex.Message);
            var argdesc = "Error en GameTimer: " + ex.Message + " UserIndex = " + iUserIndex;
            General.LogError(ref argdesc);
        }
    }

    private static void TickLluviaEvent()
    {
        try
        {
            if (!Admin.Lloviendo)
            {
                _TickLluviaEvent_MinutosSinLluvia = Convert.ToInt32(_TickLluviaEvent_MinutosSinLluvia + 1);
                if ((_TickLluviaEvent_MinutosSinLluvia >= 15) & (_TickLluviaEvent_MinutosSinLluvia < 1440))
                {
                    if (Matematicas.RandomNumber(1, 100) <= 2)
                    {
                        Admin.Lloviendo = true;
                        _TickLluviaEvent_MinutosSinLluvia = 0;
                        modSendData.SendData(modSendData.SendTarget.ToAll, 0, Protocol.PrepareMessageRainToggle());
                    }
                }
                else if (_TickLluviaEvent_MinutosSinLluvia >= 1440)
                {
                    Admin.Lloviendo = true;
                    _TickLluviaEvent_MinutosSinLluvia = 0;
                    modSendData.SendData(modSendData.SendTarget.ToAll, 0, Protocol.PrepareMessageRainToggle());
                }
            }
            else
            {
                _TickLluviaEvent_MinutosLloviendo = Convert.ToInt32(_TickLluviaEvent_MinutosLloviendo + 1);
                if (_TickLluviaEvent_MinutosLloviendo >= 5)
                {
                    Admin.Lloviendo = false;
                    modSendData.SendData(modSendData.SendTarget.ToAll, 0, Protocol.PrepareMessageRainToggle());
                    _TickLluviaEvent_MinutosLloviendo = 0;
                }
                else if (Matematicas.RandomNumber(1, 100) <= 2)
                {
                    Admin.Lloviendo = false;
                    _TickLluviaEvent_MinutosLloviendo = 0;
                    modSendData.SendData(modSendData.SendTarget.ToAll, 0, Protocol.PrepareMessageRainToggle());
                }
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in TickLluviaEvent: " + ex.Message);
            var argdesc = "Error tLluviaTimer";
            General.LogError(ref argdesc);
        }
    }

    private static void TickLluvia()
    {
        try
        {
            short iCount;
            if (Admin.Lloviendo)
            {
                var loopTo = Declaraciones.LastUser;
                for (iCount = 1; iCount <= loopTo; iCount++)
                    General.EfectoLluvia(iCount);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in TickLluvia: " + ex.Message);
            var argdesc = "tLluvia " + ex.GetType().Name + ": " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    private static void TickAutoSave()
    {
        // fired every minute
        try
        {
            _TickAutoSave_Minutos = Convert.ToInt32(_TickAutoSave_Minutos + 1);

            // ¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
            ModAreas.AreasOptimizacion();
            // ¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿

            // Actualizamos el centinela
            modCentinela.PasarMinutoCentinela();

            if (_TickAutoSave_Minutos == Admin.MinutosWs - 1)
                modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                    Protocol.PrepareMessageConsoleMsg("Worldsave en 1 minuto ...",
                        Protocol.FontTypeNames.FONTTYPE_VENENO));

            if (_TickAutoSave_Minutos >= Admin.MinutosWs)
            {
                ES.DoBackUp();
                _TickAutoSave_Minutos = 0;
            }

            if (_TickAutoSave_MinutosLatsClean >= 15)
            {
                _TickAutoSave_MinutosLatsClean = 0;
                Admin.ReSpawnOrigPosNpcs(); // respawn de los guardias en las pos originales
                General.LimpiarMundo();
            }
            else
            {
                _TickAutoSave_MinutosLatsClean = Convert.ToInt32(_TickAutoSave_MinutosLatsClean + 1);
            }

            Admin.PurgarPenas();
            CheckIdleUser();

            // <<<<<-------- Log the number of users online ------>>>
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "logs/numusers.log",
                Declaraciones.NumUsers + Environment.NewLine);
        }
        // <<<<<-------- Log the number of users online ------>>>


        catch (Exception ex)
        {
            Console.WriteLine("Error in TickAutoSave: " + ex.Message);
            var argdesc = "Error en TimerAutoSave " + ex.GetType().Name + ": " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    private static void TickNpcAtaca()
    {
        try
        {
            short Npc;

            var loopTo = Declaraciones.LastNPC;
            for (Npc = 1; Npc <= loopTo; Npc++)
                Declaraciones.Npclist[Npc].CanAttack = 1;
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in OnExit: " + ex.Message);
        }
    }

    private static void TickKillLog()
    {
        try
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "logs/connect.log"))
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "logs/connect.log");
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "logs/haciendo.log"))
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "logs/haciendo.log");
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "logs/stats.log"))
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "logs/stats.log");
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "logs/Asesinatos.log"))
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "logs/Asesinatos.log");
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "logs/HackAttemps.log"))
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + "logs/HackAttemps.log");
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "logs/nokillwsapi.txt"))
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "logs/wsapi.log"))
                    File.Delete(AppDomain.CurrentDomain.BaseDirectory + "logs/wsapi.log");
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in TickKillLog: " + ex.Message);
        }
    }

    private static void TickAI()
    {
        var NpcIndex = default(short);
        short mapa;
        short e_p;

        try
        {
            // Barrin 29/9/03
            if (!Declaraciones.haciendoBK)
            {
                // Update NPCs
                var loopTo = Declaraciones.LastNPC;
                for (NpcIndex = 1; NpcIndex <= loopTo; NpcIndex++)
                {
                    ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                    if (withBlock.flags.NPCActive) // Nos aseguramos que sea INTELIGENTE!
                    {
                        // Chequea si contiua teniendo dueño
                        if (withBlock.Owner > 0)
                            NPCs.ValidarPermanenciaNpc(NpcIndex);

                        if (withBlock.flags.Paralizado == 1)
                        {
                            General.EfectoParalisisNpc(NpcIndex);
                        }
                        else
                        {
                            e_p = PraetoriansCoopNPC.esPretoriano(NpcIndex);
                            if (e_p > 0)
                            {
                                switch (e_p)
                                {
                                    case 1: // 'clerigo
                                    {
                                        PraetoriansCoopNPC.PRCLER_AI(NpcIndex);
                                        break;
                                    }
                                    case 2: // 'mago
                                    {
                                        PraetoriansCoopNPC.PRMAGO_AI(NpcIndex);
                                        break;
                                    }
                                    case 3: // 'cazador
                                    {
                                        PraetoriansCoopNPC.PRCAZA_AI(NpcIndex);
                                        break;
                                    }
                                    case 4: // 'rey
                                    {
                                        PraetoriansCoopNPC.PRREY_AI(NpcIndex);
                                        break;
                                    }
                                    case 5: // 'guerre
                                    {
                                        PraetoriansCoopNPC.PRGUER_AI(NpcIndex);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                // Usamos AI si hay algun user en el mapa
                                if (withBlock.flags.Inmovilizado == 1) General.EfectoParalisisNpc(NpcIndex);

                                mapa = withBlock.Pos.Map;

                                if (mapa > 0)
                                    if (Declaraciones.mapInfo[mapa].NumUsers > 0)
                                        if (withBlock.Movement != AI.TipoAI.ESTATICO)
                                            AI.NPCAI(NpcIndex);
                            }
                        }
                    }
                }
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in TickNpcAtaca: " + ex.Message);
            var argdesc = "Error en TIMER_AI_Timer " + Declaraciones.Npclist[NpcIndex].name + " mapa:" +
                          Declaraciones.Npclist[NpcIndex].Pos.Map;
            General.LogError(ref argdesc);
            NPCs.MuereNpc(NpcIndex, 0);
        }
    }

    private static void TickConnection()
    {
        wskapiAO.ProcessNetworkMessages();
    }

    // TODO: esta funcion estaba suelta en el frmMain, ver donde deberia reubicarla
    public static void CheckIdleUser()
    {
        short iUserIndex;

        var loopTo = Declaraciones.MaxUsers;
        for (iUserIndex = 1; iUserIndex <= loopTo; iUserIndex++)
        {
            ref var withBlock = ref Declaraciones.UserList[iUserIndex];
            // Conexion activa? y es un usuario loggeado?
            if ((withBlock.ConnID != -1) & withBlock.flags.UserLogged)
            {
                // Actualiza el contador de inactividad
                if (withBlock.flags.Traveling == 0) withBlock.Counters.IdleCount = withBlock.Counters.IdleCount + 1;

                if (withBlock.Counters.IdleCount >= Declaraciones.IdleLimit)
                {
                    Protocol.WriteShowMessageBox(iUserIndex, "Demasiado tiempo inactivo. Has sido desconectado.");
                    // mato los comercios seguros
                    if (withBlock.ComUsu.DestUsu > 0)
                    {
                        if (Declaraciones.UserList[withBlock.ComUsu.DestUsu].flags.UserLogged)
                            if (Declaraciones.UserList[withBlock.ComUsu.DestUsu].ComUsu.DestUsu == iUserIndex)
                            {
                                Protocol.WriteConsoleMsg(withBlock.ComUsu.DestUsu,
                                    "Comercio cancelado por el otro usuario.", Protocol.FontTypeNames.FONTTYPE_TALK);
                                mdlCOmercioConUsuario.FinComerciarUsu(withBlock.ComUsu.DestUsu);
                                Protocol.FlushBuffer(withBlock.ComUsu
                                    .DestUsu); // flush the buffer to send the message right away
                            }

                        mdlCOmercioConUsuario.FinComerciarUsu(iUserIndex);
                    }

                    UsUaRiOs.Cerrar_Usuario(iUserIndex);
                }
            }
        }
    }

    public static void CloseServer()
    {
        try
        {
            // Save stats!!!
            Statistics.DumpStatistics();

            wskapiAO.LimpiaWsApi();

            short LoopC;

            var loopTo = Declaraciones.MaxUsers;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
                if (Declaraciones.UserList[LoopC].ConnID != -1)
                    TCP.CloseSocket(LoopC);

            // Log
            General.AppendLog("logs/Main.log",
                DateTime.Today.ToString() + " " + DateTime.Now.TimeOfDay.ToString() +
                " server cerrado.");

            running = false;

            // UPGRADE_NOTE: El objeto SonidosMapas no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            Declaraciones.SonidosMapas = null;
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in TickAI: " + ex.Message);
        }
    }
}