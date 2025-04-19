Option Strict Off
Option Explicit On

Imports System.Threading

Module GameLoop
    ' TODO: verificar si el servidor se esta cerrando correctamente!!
    Private running As Boolean = True

    Private piqueteCInterval As Integer = 6000
    Private packetResendInterval As Integer = 10
    Private fxInterval As Integer = 4000
    Private auditoriaInterval As Integer = 1000
    Private gameTimerInterval As Integer = 40
    Private lluviaEventInterval As Integer = 60000
    Public lluviaInterval As Integer = 500
    Private autoSaveInterval As Integer = 60000
    Public npcAtacaInterval As Integer = 4000
    Private killLogInterval As Integer = 60000
    Public timerAIInterval As Integer = 100
    Private connectionTimerInterval As Integer = 5

    ' Últimos tiempos de ejecución
    Dim lastPiqueteC As Double = 0
    Dim lastPacketResend As Double = 0
    Dim lastFx As Double = 0
    Dim lastAuditoria As Double = 0
    Dim lastGameTimer As Double = 0
    Dim lastLluviaEvent As Double = 0
    Dim lastLluvia As Double = 0
    Dim lastAutoSave As Double = 0
    Dim lastNpcAtaca As Double = 0
    Dim lastKillLog As Double = 0
    Dim lastTimerAI As Double = 0
    Dim lastConnection As Double = 0

    Private Sub OnExit(sender As Object, e As EventArgs)
        Console.WriteLine("Evento: ProcessExit")
        running = False
    End Sub

    Private Sub OnCancel(sender As Object, e As ConsoleCancelEventArgs)
        Console.WriteLine("Evento: CancelKeyPress")
        e.Cancel = True ' Previene salida inmediata
        running = False
    End Sub

    Public Sub DoGameLoop()

        AddHandler AppDomain.CurrentDomain.ProcessExit, AddressOf OnExit
        AddHandler Console.CancelKeyPress, AddressOf OnCancel

        Dim sw As Stopwatch = Stopwatch.StartNew()

        While running
            Dim now As Double = sw.Elapsed.TotalMilliseconds

            If now - lastPiqueteC >= piqueteCInterval Then
                TickPiqueteC()
                lastPiqueteC = now
            End If

            If now - lastPacketResend >= packetResendInterval Then
                TickPacketResend()
                lastPacketResend = now
            End If

            If now - lastFx >= fxInterval Then
                TickFx()
                lastFx = now
            End If

            If now - lastAuditoria >= auditoriaInterval Then
                TickAuditoria()
                lastAuditoria = now
            End If

            If now - lastGameTimer >= gameTimerInterval Then
                TickGameTimer()
                lastGameTimer = now
            End If

            If now - lastLluviaEvent >= lluviaEventInterval Then
                TickLluviaEvent()
                lastLluviaEvent = now
            End If

            If now - lastLluvia >= lluviaInterval Then
                TickLluvia()
                lastLluvia = now
            End If

            If now - lastAutoSave >= autoSaveInterval Then
                TickAutoSave()
                lastAutoSave = now
            End If

            If now - lastNpcAtaca >= npcAtacaInterval Then
                TickNpcAtaca()
                lastNpcAtaca = now
            End If

            If now - lastKillLog >= killLogInterval Then
                TickKillLog()
                lastKillLog = now
            End If

            If now - lastTimerAI >= timerAIInterval Then
                TickAI()
                lastTimerAI = now
            End If

            If now - lastConnection >= connectionTimerInterval Then
                TickConnection()
                lastConnection = now
            End If

            ' Pausa mínima para evitar uso excesivo de CPU
            Thread.Sleep(1)
        End While
    End Sub

    ' =====================
    ' Subrutinas de ticks
    ' =====================

    Private Sub TickPiqueteC()
        Dim NuevaA As Boolean
        ' Dim NuevoL As Boolean
        Dim GI As Short

        Dim i As Integer

        Try
        For i = 1 To LastUser
            With UserList(i)
                If .flags.UserLogged Then
                    If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = Declaraciones.eTrigger.ANTIPIQUETE Then
                        .Counters.PiqueteC = .Counters.PiqueteC + 1
                        Call _
                            WriteConsoleMsg(i, "¡¡¡Estás obstruyendo la vía pública, muévete o serás encarcelado!!!",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)

                        If .Counters.PiqueteC > 23 Then
                            .Counters.PiqueteC = 0
                            Call Encarcelar(i, TIEMPO_CARCEL_PIQUETE)
                        End If
                    Else
                        .Counters.PiqueteC = 0
                    End If

                    If .flags.Muerto = 1 Then
                        If .flags.Traveling = 1 Then
                            If .Counters.goHome <= 0 Then
                                Call FindLegalPos(i, Ciudades(.Hogar).Map, Ciudades(.Hogar).X, Ciudades(.Hogar).Y)
                                Call WarpUserChar(i, Ciudades(.Hogar).Map, Ciudades(.Hogar).X, Ciudades(.Hogar).Y, True)
                                Call WriteMultiMessage(i, Declaraciones.eMessages.FinishHome)
                                .flags.Traveling = 0
                            Else
                                .Counters.goHome = .Counters.goHome - 1
                            End If
                        End If
                    End If

                    'ustedes se preguntaran que hace esto aca?
                    'bueno la respuesta es simple: el codigo de AO es una mierda y encontrar
                    'todos los puntos en los cuales la alineacion puede cambiar es un dolor de
                    'huevos, asi que lo controlo aca, cada 6 segundos, lo cual es razonable

                    GI = .GuildIndex
                    If GI > 0 Then
                        NuevaA = False
                        ' NuevoL = False
                        If Not modGuilds.m_ValidarPermanencia(i, True, NuevaA) Then
                            Call _
                                WriteConsoleMsg(i,
                                                "Has sido expulsado del clan. ¡El clan ha sumado un punto de antifacción!",
                                                Protocol.FontTypeNames.FONTTYPE_GUILD)
                        End If
                        If NuevaA Then
                            Call _
                                SendData(modSendData.SendTarget.ToGuildMembers, GI,
                                         PrepareMessageConsoleMsg(
                                             "¡El clan ha pasado a tener alineación " & GuildAlignment(GI) & "!",
                                             Protocol.FontTypeNames.FONTTYPE_GUILD))
                            Call LogClanes("¡El clan cambio de alineación!")
                        End If
                        '                    If NuevoL Then
                        '                        Call SendData(SendTarget.ToGuildMembers, GI, PrepareMessageConsoleMsg("¡El clan tiene un nuevo líder!", FontTypeNames.FONTTYPE_GUILD))
                        '                        Call LogClanes("¡El clan tiene nuevo lider!")
                        '                    End If
                    End If

                    Call FlushBuffer(i)
                End If
            End With
        Next i
        

        
Catch ex As Exception
    Console.WriteLine("Error in OnExit: " & ex.Message)
    Call LogError("Error en tPiqueteC_Timer " & Err.Number & ": " & Err.Description)
End Try
End Sub

    Private Sub TickPacketResend()
        '***************************************************
        'Autor: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 04/01/07
        'Attempts to resend to the user all data that may be enqueued.
        '***************************************************
        Try
        Dim i As Integer

        For i = 1 To MaxUsers
            If UserList(i).ConnIDValida Then
                If UserList(i).outgoingData.length > 0 Then
                    Call _
                        EnviarDatosASlot(i,
                                         UserList(i).outgoingData.ReadASCIIStringFixed(UserList(i).outgoingData.length))
                End If
            End If
        Next i

        

        
Catch ex As Exception
    Console.WriteLine("Error in TickPacketResend: " & ex.Message)
    LogError(("Error en packetResend - Error: " & Err.Number & " - Desc: " & Err.Description))
        Resume Next
End Try
End Sub

    Private Sub TickFx()
        Try

        Call SonidosMapas.ReproducirSonidosDeMapas()

        
        
Catch ex As Exception
    Console.WriteLine("Error in TickFx: " & ex.Message)
    
End Try
End Sub

    Private Sub TickAuditoria()
        Try
        Static centinelSecs As Byte

        centinelSecs = centinelSecs + 1

        If centinelSecs = 5 Then
            'Every 5 seconds, we try to call the player's attention so it will report the code.
            Call modCentinela.CallUserAttention()

            centinelSecs = 0
        End If

        Call PasarSegundo() 'sistema de desconexion de 10 segs

        Call ActualizaEstadisticasWeb()

        

        
Catch ex As Exception
    Console.WriteLine("Error in TickAuditoria: " & ex.Message)
    Call LogError("Error en Timer Auditoria. Err: " & Err.Description & " - " & Err.Number)
        Resume Next
End Try
End Sub

    Private Sub TickGameTimer()
        '********************************************************
        'Author: Unknown
        'Last Modify Date: -
        '********************************************************
        Dim iUserIndex As Integer
        Dim bEnviarStats As Boolean
        Dim bEnviarAyS As Boolean

        Try

        '<<<<<< Procesa eventos de los usuarios >>>>>>
        For iUserIndex = 1 To MaxUsers 'LastUser
            With UserList(iUserIndex)
                'Conexion activa?
                If .ConnID <> - 1 Then
                    '¿User valido?

                    If .ConnIDValida And .flags.UserLogged Then

                        '[Alejo-18-5]
                        bEnviarStats = False
                        bEnviarAyS = False

                        Call DoTileEvents(iUserIndex, .Pos.Map, .Pos.X, .Pos.Y)


                        If .flags.Paralizado = 1 Then Call EfectoParalisisUser(iUserIndex)
                        If .flags.Ceguera = 1 Or .flags.Estupidez Then Call EfectoCegueEstu(iUserIndex)


                        If .flags.Muerto = 0 Then

                            '[Consejeros]
                            If (.flags.Privilegios And Declaraciones.PlayerType.User) Then Call EfectoLava(iUserIndex)

                            If .flags.Desnudo <> 0 And (.flags.Privilegios And Declaraciones.PlayerType.User) <> 0 Then _
                                Call EfectoFrio(iUserIndex)

                            If .flags.Meditando Then Call DoMeditar(iUserIndex)

                            If .flags.Envenenado <> 0 And (.flags.Privilegios And Declaraciones.PlayerType.User) <> 0 _
                                Then Call EfectoVeneno(iUserIndex)

                            If .flags.AdminInvisible <> 1 Then
                                If .flags.invisible = 1 Then Call EfectoInvisibilidad(iUserIndex)
                                If .flags.Oculto = 1 Then Call DoPermanecerOculto(iUserIndex)
                            End If

                            If .flags.Mimetizado = 1 Then Call EfectoMimetismo(iUserIndex)

                            If .flags.AtacablePor > 0 Then Call EfectoEstadoAtacable(iUserIndex)

                            Call DuracionPociones(iUserIndex)

                            Call HambreYSed(iUserIndex, bEnviarAyS)

                            If .flags.Hambre = 0 And .flags.Sed = 0 Then
                                If Lloviendo Then
                                    If Not Intemperie(iUserIndex) Then
                                        If Not .flags.Descansar Then
                                            'No esta descansando
                                            Call Sanar(iUserIndex, bEnviarStats, SanaIntervaloSinDescansar)
                                            If bEnviarStats Then
                                                Call WriteUpdateHP(iUserIndex)
                                                bEnviarStats = False
                                            End If
                                            Call RecStamina(iUserIndex, bEnviarStats, StaminaIntervaloSinDescansar)
                                            If bEnviarStats Then
                                                Call WriteUpdateSta(iUserIndex)
                                                bEnviarStats = False
                                            End If
                                        Else
                                            'esta descansando
                                            Call Sanar(iUserIndex, bEnviarStats, SanaIntervaloDescansar)
                                            If bEnviarStats Then
                                                Call WriteUpdateHP(iUserIndex)
                                                bEnviarStats = False
                                            End If
                                            Call RecStamina(iUserIndex, bEnviarStats, StaminaIntervaloDescansar)
                                            If bEnviarStats Then
                                                Call WriteUpdateSta(iUserIndex)
                                                bEnviarStats = False
                                            End If
                                            'termina de descansar automaticamente
                                            If .Stats.MaxHp = .Stats.MinHp And .Stats.MaxSta = .Stats.MinSta Then
                                                Call WriteRestOK(iUserIndex)
                                                Call _
                                                    WriteConsoleMsg(iUserIndex, "Has terminado de descansar.",
                                                                    Protocol.FontTypeNames.FONTTYPE_INFO)
                                                .flags.Descansar = False
                                            End If

                                        End If
                                    End If
                                Else
                                    If Not .flags.Descansar Then
                                        'No esta descansando

                                        Call Sanar(iUserIndex, bEnviarStats, SanaIntervaloSinDescansar)
                                        If bEnviarStats Then
                                            Call WriteUpdateHP(iUserIndex)
                                            bEnviarStats = False
                                        End If
                                        Call RecStamina(iUserIndex, bEnviarStats, StaminaIntervaloSinDescansar)
                                        If bEnviarStats Then
                                            Call WriteUpdateSta(iUserIndex)
                                            bEnviarStats = False
                                        End If

                                    Else
                                        'esta descansando

                                        Call Sanar(iUserIndex, bEnviarStats, SanaIntervaloDescansar)
                                        If bEnviarStats Then
                                            Call WriteUpdateHP(iUserIndex)
                                            bEnviarStats = False
                                        End If
                                        Call RecStamina(iUserIndex, bEnviarStats, StaminaIntervaloDescansar)
                                        If bEnviarStats Then
                                            Call WriteUpdateSta(iUserIndex)
                                            bEnviarStats = False
                                        End If
                                        'termina de descansar automaticamente
                                        If .Stats.MaxHp = .Stats.MinHp And .Stats.MaxSta = .Stats.MinSta Then
                                            Call WriteRestOK(iUserIndex)
                                            Call _
                                                WriteConsoleMsg(iUserIndex, "Has terminado de descansar.",
                                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                                            .flags.Descansar = False
                                        End If

                                    End If
                                End If
                            End If

                            If bEnviarAyS Then Call WriteUpdateHungerAndThirst(iUserIndex)

                            If .NroMascotas > 0 Then Call TiempoInvocacion(iUserIndex)
                        End If 'Muerto
                    Else 'no esta logeado?
                        'Inactive players will be removed!
                        .Counters.IdleCount = .Counters.IdleCount + 1
                        If .Counters.IdleCount > IntervaloParaConexion Then
                            .Counters.IdleCount = 0
                            Call CloseSocket(iUserIndex)
                        End If
                    End If 'UserLogged

                    'If there is anything to be sent, we send it
                    Call FlushBuffer(iUserIndex)
                End If
            End With
        Next iUserIndex
        

        
Catch ex As Exception
    Console.WriteLine("Error in TickGameTimer: " & ex.Message)
    LogError(("Error en GameTimer: " & Err.Description & " UserIndex = " & iUserIndex))
End Try
End Sub

    Private Sub TickLluviaEvent()
        Try
        Static MinutosLloviendo As Integer
        Static MinutosSinLluvia As Integer

        If Not Lloviendo Then
            MinutosSinLluvia = MinutosSinLluvia + 1
            If MinutosSinLluvia >= 15 And MinutosSinLluvia < 1440 Then
                If RandomNumber(1, 100) <= 2 Then
                    Lloviendo = True
                    MinutosSinLluvia = 0
                    Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageRainToggle())
                End If
            ElseIf MinutosSinLluvia >= 1440 Then
                Lloviendo = True
                MinutosSinLluvia = 0
                Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageRainToggle())
            End If
        Else
            MinutosLloviendo = MinutosLloviendo + 1
            If MinutosLloviendo >= 5 Then
                Lloviendo = False
                Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageRainToggle())
                MinutosLloviendo = 0
            Else
                If RandomNumber(1, 100) <= 2 Then
                    Lloviendo = False
                    MinutosLloviendo = 0
                    Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageRainToggle())
                End If
            End If
        End If

        
        
Catch ex As Exception
    Console.WriteLine("Error in TickLluviaEvent: " & ex.Message)
    Call LogError("Error tLluviaTimer")
End Try
End Sub

    Private Sub TickLluvia()
        Try

        Dim iCount As Integer
        If Lloviendo Then
            For iCount = 1 To LastUser
                Call EfectoLluvia(iCount)
            Next iCount
        End If

        
        
Catch ex As Exception
    Console.WriteLine("Error in TickLluvia: " & ex.Message)
    Call LogError("tLluvia " & Err.Number & ": " & Err.Description)
End Try
End Sub

    Private Sub TickAutoSave()
        Try
        'fired every minute
        Static Minutos As Integer
        Static MinutosLatsClean As Integer
        Static MinsPjesSave As Integer

        Dim i As Short
        Dim num As Integer

        Minutos = Minutos + 1

        '¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
        Call ModAreas.AreasOptimizacion()
        '¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿

        'Actualizamos el centinela
        Call modCentinela.PasarMinutoCentinela()

        If Minutos = MinutosWs - 1 Then
            Call _
                SendData(modSendData.SendTarget.ToAll, 0,
                         PrepareMessageConsoleMsg("Worldsave en 1 minuto ...", Protocol.FontTypeNames.FONTTYPE_VENENO))
        End If

        If Minutos >= MinutosWs Then
            Call ES.DoBackUp()
            Minutos = 0
        End If

        If MinutosLatsClean >= 15 Then
            MinutosLatsClean = 0
            Call ReSpawnOrigPosNpcs() 'respawn de los guardias en las pos originales
            Call LimpiarMundo()
        Else
            MinutosLatsClean = MinutosLatsClean + 1
        End If

        Call PurgarPenas()
        Call CheckIdleUser()

        '<<<<<-------- Log the number of users online ------>>>
        Dim N As Short
        N = FreeFile()
        FileOpen(N, AppDomain.CurrentDomain.BaseDirectory & "logs/numusers.log", OpenMode.Output, , OpenShare.Shared)
        PrintLine(N, NumUsers)
        FileClose(N)
        '<<<<<-------- Log the number of users online ------>>>

        
        
Catch ex As Exception
    Console.WriteLine("Error in TickAutoSave: " & ex.Message)
    Call LogError("Error en TimerAutoSave " & Err.Number & ": " & Err.Description)
        Resume Next
End Try
End Sub

    Private Sub TickNpcAtaca()
        Try
        'UPGRADE_NOTE: npc se actualizó a npc_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim npc_Renamed As Integer

        For npc_Renamed = 1 To LastNPC
            Npclist(npc_Renamed).CanAttack = 1
        Next npc_Renamed
    
Catch ex As Exception
    Console.WriteLine("Error in OnExit: " & ex.Message)
End Try
End Sub

    Private Sub TickKillLog()
        Try
        If FileExist(AppDomain.CurrentDomain.BaseDirectory & "logs/connect.log") Then _
            Kill(AppDomain.CurrentDomain.BaseDirectory & "logs/connect.log")
        If FileExist(AppDomain.CurrentDomain.BaseDirectory & "logs/haciendo.log") Then _
            Kill(AppDomain.CurrentDomain.BaseDirectory & "logs/haciendo.log")
        If FileExist(AppDomain.CurrentDomain.BaseDirectory & "logs/stats.log") Then _
            Kill(AppDomain.CurrentDomain.BaseDirectory & "logs/stats.log")
        If FileExist(AppDomain.CurrentDomain.BaseDirectory & "logs/Asesinatos.log") Then _
            Kill(AppDomain.CurrentDomain.BaseDirectory & "logs/Asesinatos.log")
        If FileExist(AppDomain.CurrentDomain.BaseDirectory & "logs/HackAttemps.log") Then _
            Kill(AppDomain.CurrentDomain.BaseDirectory & "logs/HackAttemps.log")
        If Not FileExist(AppDomain.CurrentDomain.BaseDirectory & "logs/nokillwsapi.txt") Then
            If FileExist(AppDomain.CurrentDomain.BaseDirectory & "logs/wsapi.log") Then _
                Kill(AppDomain.CurrentDomain.BaseDirectory & "logs/wsapi.log")
        End If
    
Catch ex As Exception
    Console.WriteLine("Error in TickKillLog: " & ex.Message)
End Try
End Sub

    Private Sub TickAI()
        Dim NpcIndex As Integer
        Dim X As Short
        Dim Y As Short
        Dim UseAI As Short
        Dim mapa As Short
        Dim e_p As Short

        Try

            'Barrin 29/9/03
            If Not haciendoBK And Not EnPausa Then
                'Update NPCs
                For NpcIndex = 1 To LastNPC

                    With Npclist(NpcIndex)
                        If .flags.NPCActive Then 'Nos aseguramos que sea INTELIGENTE!

                            ' Chequea si contiua teniendo dueño
                            If .Owner > 0 Then Call ValidarPermanenciaNpc(NpcIndex)

                            If .flags.Paralizado = 1 Then
                                Call EfectoParalisisNpc(NpcIndex)
                            Else
                                e_p = esPretoriano(NpcIndex)
                                If e_p > 0 Then
                                    Select Case e_p
                                        Case 1 ''clerigo
                                            Call PRCLER_AI(NpcIndex)
                                        Case 2 ''mago
                                            Call PRMAGO_AI(NpcIndex)
                                        Case 3 ''cazador
                                            Call PRCAZA_AI(NpcIndex)
                                        Case 4 ''rey
                                            Call PRREY_AI(NpcIndex)
                                        Case 5 ''guerre
                                            Call PRGUER_AI(NpcIndex)
                                    End Select
                                Else
                                    'Usamos AI si hay algun user en el mapa
                                    If .flags.Inmovilizado = 1 Then
                                        Call EfectoParalisisNpc(NpcIndex)
                                    End If

                                    mapa = .Pos.Map

                                    If mapa > 0 Then
                                        If MapInfo_Renamed(mapa).NumUsers > 0 Then
                                            If .Movement <> AI.TipoAI.ESTATICO Then
                                                Call NPCAI(NpcIndex)
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End With
                Next NpcIndex
            End If




        Catch ex As Exception
            Console.WriteLine("Error in TickNpcAtaca: " & ex.Message)
        Call LogError("Error en TIMER_AI_Timer " & Npclist(NpcIndex).name & " mapa:" & Npclist(NpcIndex).Pos.Map)
        Call MuereNpc(NpcIndex, 0)
        End Try
    End Sub

    Private Sub TickConnection()
        ProcessNetworkMessages()
    End Sub

    ' TODO: esta funcion estaba suelta en el frmMain, ver donde deberia reubicarla
    Sub CheckIdleUser()
        Dim iUserIndex As Integer

        For iUserIndex = 1 To MaxUsers
            With UserList(iUserIndex)
                'Conexion activa? y es un usuario loggeado?
                If .ConnID <> - 1 And .flags.UserLogged Then
                    'Actualiza el contador de inactividad
                    If .flags.Traveling = 0 Then
                        .Counters.IdleCount = .Counters.IdleCount + 1
                    End If

                    If .Counters.IdleCount >= IdleLimit Then
                        Call WriteShowMessageBox(iUserIndex, "Demasiado tiempo inactivo. Has sido desconectado.")
                        'mato los comercios seguros
                        If .ComUsu.DestUsu > 0 Then
                            If UserList(.ComUsu.DestUsu).flags.UserLogged Then
                                If UserList(.ComUsu.DestUsu).ComUsu.DestUsu = iUserIndex Then
                                    Call _
                                        WriteConsoleMsg(.ComUsu.DestUsu, "Comercio cancelado por el otro usuario.",
                                                        Protocol.FontTypeNames.FONTTYPE_TALK)
                                    Call FinComerciarUsu(.ComUsu.DestUsu)
                                    Call FlushBuffer(.ComUsu.DestUsu) 'flush the buffer to send the message right away
                                End If
                            End If
                            Call FinComerciarUsu(iUserIndex)
                        End If
                        Call Cerrar_Usuario(iUserIndex)
                    End If
                End If
            End With
        Next iUserIndex
    End Sub

    Public Sub CloseServer()
        Try

        'Save stats!!!
        Call Statistics.DumpStatistics()

        Call LimpiaWsApi()

        Dim LoopC As Short

        For LoopC = 1 To MaxUsers
            If UserList(LoopC).ConnID <> - 1 Then Call CloseSocket(LoopC)
        Next

        'Log
        Dim N As Short
        N = FreeFile()
        FileOpen(N, AppDomain.CurrentDomain.BaseDirectory & "logs/Main.log", OpenMode.Append, , OpenShare.Shared)
        PrintLine(N, Today & " " & TimeOfDay & " server cerrado.")
        FileClose(N)

        running = False

        'UPGRADE_NOTE: El objeto SonidosMapas no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        SonidosMapas = Nothing
    
Catch ex As Exception
    Console.WriteLine("Error in TickAI: " & ex.Message)
End Try
End Sub
End Module
