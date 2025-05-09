Option Strict Off
Option Explicit On

Imports System.Drawing

Module modCentinela
    Private Const NPC_CENTINELA_TIERRA As Short = 16 'Índice del NPC en el .dat
    Private Const NPC_CENTINELA_AGUA As Short = 16 'Ídem anterior, pero en mapas de agua

    Public CentinelaNPCIndex As Short 'Índice del NPC en el servidor

    Private Const TIEMPO_INICIAL As Byte = 2 _
    'Tiempo inicial en minutos. No reducir sin antes revisar el timer que maneja estos datos.

    Public Structure tCentinela
        Dim RevisandoUserIndex As Short '¿Qué índice revisamos?
        Dim TiempoRestante As Short '¿Cuántos minutos le quedan al usuario?
        Dim clave As Short 'Clave que debe escribir
        Dim spawnTime As Integer
    End Structure

    Public centinelaActivado As Boolean

    Public Centinela As tCentinela

    Public Sub CallUserAttention()
        '############################################################
        'Makes noise and FX to call the user's attention.
        '############################################################
        If GetTickCount() - Centinela.spawnTime >= 5000 Then
            If Centinela.RevisandoUserIndex <> 0 And centinelaActivado Then
                If Not UserList(Centinela.RevisandoUserIndex).flags.CentinelaOK Then
                    Call _
                        WritePlayWave(Centinela.RevisandoUserIndex, SND_WARP, Npclist(CentinelaNPCIndex).Pos.X,
                                      Npclist(CentinelaNPCIndex).Pos.Y)
                    Call _
                        WriteCreateFX(Centinela.RevisandoUserIndex, Npclist(CentinelaNPCIndex).Char_Renamed.CharIndex,
                                      FXIDs.FXWARP, 0)

                    'Resend the key
                    Call CentinelaSendClave(Centinela.RevisandoUserIndex)

                    Call FlushBuffer(Centinela.RevisandoUserIndex)
                End If
            End If
        End If
    End Sub

    Private Sub GoToNextWorkingChar()
        '############################################################
        'Va al siguiente usuario que se encuentre trabajando
        '############################################################
        Dim LoopC As Integer

        For LoopC = 1 To LastUser
            If _
                UserList(LoopC).flags.UserLogged And UserList(LoopC).Counters.Trabajando > 0 And
                (UserList(LoopC).flags.Privilegios And PlayerType.User) Then
                If Not UserList(LoopC).flags.CentinelaOK Then
                    'Inicializamos
                    Centinela.RevisandoUserIndex = LoopC
                    Centinela.TiempoRestante = TIEMPO_INICIAL
                    Centinela.clave = RandomNumber(1, 32000)
                    Centinela.spawnTime = GetTickCount()

                    'Ponemos al centinela en posición
                    Call WarpCentinela(LoopC)

                    If CentinelaNPCIndex Then
                        'Mandamos el mensaje (el centinela habla y aparece en consola para que no haya dudas)
                        Call _
                            WriteChatOverHead(LoopC,
                                              "Saludos " & UserList(LoopC).name &
                                              ", soy el Centinela de estas tierras. Me gustaría que escribas /CENTINELA " &
                                              Centinela.clave & " en no más de dos minutos.",
                                              CShort(CStr(Npclist(CentinelaNPCIndex).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.Lime))
                        Call _
                            WriteConsoleMsg(LoopC, "El centinela intenta llamar tu atención. ¡Respóndele rápido!",
                                            FontTypeNames.FONTTYPE_CENTINELA)
                        Call FlushBuffer(LoopC)
                    End If
                    Exit Sub
                End If
            End If
        Next LoopC

        'No hay chars trabajando, eliminamos el NPC si todavía estaba en algún lado y esperamos otro minuto
        If CentinelaNPCIndex Then
            Call QuitarNPC(CentinelaNPCIndex)
            CentinelaNPCIndex = 0
        End If

        'No estamos revisando a nadie
        Centinela.RevisandoUserIndex = 0
    End Sub

    Private Sub CentinelaFinalCheck()
        '############################################################
        'Al finalizar el tiempo, se retira y realiza la acción
        'pertinente dependiendo del caso
        '############################################################
        Try
            Dim name As String
            Dim numPenas As Short

            Dim Index As Short
            If Not UserList(Centinela.RevisandoUserIndex).flags.CentinelaOK Then
                'Logueamos el evento
                Call _
                    LogCentinela(
                        "Centinela baneo a " & UserList(Centinela.RevisandoUserIndex).name &
                        " por uso de macro inasistido.")

                'Ponemos el ban
                UserList(Centinela.RevisandoUserIndex).flags.Ban = 1

                name = UserList(Centinela.RevisandoUserIndex).name

                'Avisamos a los admins
                Call _
                    SendData(SendTarget.ToAdmins, 0,
                             PrepareMessageConsoleMsg("Servidor> El centinela ha baneado a " & name,
                                                      FontTypeNames.FONTTYPE_SERVER))

                'ponemos el flag de ban a 1
                Call WriteVar(CharPath & name & ".chr", "FLAGS", "Ban", "1")
                'ponemos la pena
                numPenas = Val(GetVar(CharPath & name & ".chr", "PENAS", "Cant"))
                Call WriteVar(CharPath & name & ".chr", "PENAS", "Cant", CStr(numPenas + 1))
                Call _
                    WriteVar(CharPath & name & ".chr", "PENAS", "P" & numPenas + 1,
                             "CENTINELA : BAN POR MACRO INASISTIDO " & Today & " " & TimeOfDay)

                'Evitamos loguear el logout
                Index = Centinela.RevisandoUserIndex
                Centinela.RevisandoUserIndex = 0

                Call CloseSocket(Index)
            End If

            Centinela.clave = 0
            Centinela.TiempoRestante = 0
            Centinela.RevisandoUserIndex = 0

            If CentinelaNPCIndex Then
                Call QuitarNPC(CentinelaNPCIndex)
                CentinelaNPCIndex = 0
            End If


        Catch ex As Exception
            Console.WriteLine("Error in CallUserAttention: " & ex.Message)
            Centinela.clave = 0
            Centinela.TiempoRestante = 0
            Centinela.RevisandoUserIndex = 0

            If CentinelaNPCIndex Then
                Call QuitarNPC(CentinelaNPCIndex)
                CentinelaNPCIndex = 0
            End If

            Call LogError("Error en el checkeo del centinela: " & Err.Description)
        End Try
    End Sub

    Public Sub CentinelaCheckClave(UserIndex As Short, clave As Short)
        '############################################################
        'Corrobora la clave que le envia el usuario
        '############################################################
        If clave = Centinela.clave And UserIndex = Centinela.RevisandoUserIndex Then
            UserList(Centinela.RevisandoUserIndex).flags.CentinelaOK = True
            Call _
                WriteChatOverHead(UserIndex,
                                  "¡Muchas gracias " & UserList(Centinela.RevisandoUserIndex).name &
                                  "! Espero no haber sido una molestia.",
                                  CShort(CStr(Npclist(CentinelaNPCIndex).Char_Renamed.CharIndex)),
                                  ColorTranslator.ToOle(Color.White))
            Centinela.RevisandoUserIndex = 0
            Call FlushBuffer(UserIndex)
        Else
            Call CentinelaSendClave(UserIndex)

            'Logueamos el evento
            If UserIndex <> Centinela.RevisandoUserIndex Then
                Call LogCentinela("El usuario " & UserList(UserIndex).name & " respondió aunque no se le hablaba a él.")
            Else
                Call _
                    LogCentinela(
                        "El usuario " & UserList(UserIndex).name & " respondió una clave incorrecta: " & clave &
                        " - Se esperaba : " & Centinela.clave)
            End If
        End If
    End Sub

    Public Sub ResetCentinelaInfo()
        '############################################################
        'Cada determinada cantidad de tiempo, volvemos a revisar
        '############################################################
        Dim LoopC As Integer

        For LoopC = 1 To LastUser
            If (migr_LenB(UserList(LoopC).name) <> 0 And LoopC <> Centinela.RevisandoUserIndex) Then
                UserList(LoopC).flags.CentinelaOK = False
            End If
        Next LoopC
    End Sub

    Public Sub CentinelaSendClave(UserIndex As Short)
        '############################################################
        'Enviamos al usuario la clave vía el personaje centinela
        '############################################################
        If CentinelaNPCIndex = 0 Then Exit Sub

        If UserIndex = Centinela.RevisandoUserIndex Then
            If Not UserList(UserIndex).flags.CentinelaOK Then
                Call _
                    WriteChatOverHead(UserIndex,
                                      "¡La clave que te he dicho es /CENTINELA " & Centinela.clave &
                                      ", escríbelo rápido!",
                                      CShort(CStr(Npclist(CentinelaNPCIndex).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.Lime))
                Call _
                    WriteConsoleMsg(UserIndex, "El centinela intenta llamar tu atención. ¡Respondele rápido!",
                                    FontTypeNames.FONTTYPE_CENTINELA)
            Else
                'Logueamos el evento
                Call _
                    LogCentinela(
                        "El usuario " & UserList(Centinela.RevisandoUserIndex).name &
                        " respondió más de una vez la contraseña correcta.")
                Call _
                    WriteChatOverHead(UserIndex, "Te agradezco, pero ya me has respondido. Me retiraré pronto.",
                                      CShort(CStr(Npclist(CentinelaNPCIndex).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.Lime))
            End If
        Else
            Call _
                WriteChatOverHead(UserIndex, "No es a ti a quien estoy hablando, ¿No ves?",
                                  CShort(CStr(Npclist(CentinelaNPCIndex).Char_Renamed.CharIndex)),
                                  ColorTranslator.ToOle(Color.White))
        End If
    End Sub

    Public Sub PasarMinutoCentinela()
        '############################################################
        'Control del timer. Llamado cada un minuto.
        '############################################################
        If Not centinelaActivado Then Exit Sub

        If Centinela.RevisandoUserIndex = 0 Then
            Call GoToNextWorkingChar()
        Else
            Centinela.TiempoRestante = Centinela.TiempoRestante - 1

            If Centinela.TiempoRestante = 0 Then
                Call CentinelaFinalCheck()
                Call GoToNextWorkingChar()
            Else
                'Recordamos al user que debe escribir
                If Distancia(Npclist(CentinelaNPCIndex).Pos, UserList(Centinela.RevisandoUserIndex).Pos) > 5 _
                    Then
                    Call WarpCentinela(Centinela.RevisandoUserIndex)
                End If

                'El centinela habla y se manda a consola para que no quepan dudas
                Call _
                    WriteChatOverHead(Centinela.RevisandoUserIndex,
                                      "¡" & UserList(Centinela.RevisandoUserIndex).name &
                                      ", tienes un minuto más para responder! Debes escribir /CENTINELA " &
                                      Centinela.clave & ".",
                                      CShort(CStr(Npclist(CentinelaNPCIndex).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.Red))
                Call _
                    WriteConsoleMsg(Centinela.RevisandoUserIndex,
                                    "¡" & UserList(Centinela.RevisandoUserIndex).name &
                                    ", tienes un minuto más para responder!", FontTypeNames.FONTTYPE_CENTINELA)
                Call FlushBuffer(Centinela.RevisandoUserIndex)
            End If
        End If
    End Sub

    Private Sub WarpCentinela(UserIndex As Short)
        '############################################################
        'Inciamos la revisión del usuario UserIndex
        '############################################################
        'Evitamos conflictos de índices
        If CentinelaNPCIndex Then
            Call QuitarNPC(CentinelaNPCIndex)
            CentinelaNPCIndex = 0
        End If

        If HayAgua(UserList(UserIndex).Pos.Map, UserList(UserIndex).Pos.X, UserList(UserIndex).Pos.Y) Then
            CentinelaNPCIndex = SpawnNpc(NPC_CENTINELA_AGUA, UserList(UserIndex).Pos, True, False)
        Else
            CentinelaNPCIndex = SpawnNpc(NPC_CENTINELA_TIERRA, UserList(UserIndex).Pos, True, False)
        End If

        'Si no pudimos crear el NPC, seguimos esperando a poder hacerlo
        If CentinelaNPCIndex = 0 Then Centinela.RevisandoUserIndex = 0
    End Sub

    Public Sub CentinelaUserLogout()
        '############################################################
        'El usuario al que revisabamos se desconectó
        '############################################################
        If Centinela.RevisandoUserIndex Then
            'Logueamos el evento
            Call _
                LogCentinela(
                    "El usuario " & UserList(Centinela.RevisandoUserIndex).name &
                    " se desolgueó al pedirsele la contraseña.")

            'Reseteamos y esperamos a otro PasarMinuto para ir al siguiente user
            Centinela.clave = 0
            Centinela.TiempoRestante = 0
            Centinela.RevisandoUserIndex = 0

            If CentinelaNPCIndex Then
                Call QuitarNPC(CentinelaNPCIndex)
                CentinelaNPCIndex = 0
            End If
        End If
    End Sub

    Private Sub LogCentinela(texto As String)
        '*************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last modified: 03/15/2006
        'Loguea un evento del centinela
        '*************************************************
        Try

            Dim nfile As Short
            nfile = FreeFile ' obtenemos un canal

            FileOpen(nfile, AppDomain.CurrentDomain.BaseDirectory & "logs/Centinela.log", OpenMode.Append, ,
                     OpenShare.Shared)
            PrintLine(nfile, Today & " " & TimeOfDay & " " & texto)
            FileClose(nfile)


        Catch ex As Exception
            Console.WriteLine("Error in CentinelaCheckClave: " & ex.Message)

        End Try
    End Sub
End Module
