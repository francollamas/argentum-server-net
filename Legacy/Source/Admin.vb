Option Strict Off
Option Explicit On
Module Admin
    Public Structure tMotd
        Dim texto As String
        Dim Formato As String
    End Structure

    Public MaxLines As Short
    Public MOTD() As tMotd

    Public Structure tAPuestas
        Dim Ganancias As Integer
        Dim Perdidas As Integer
        Dim Jugadas As Integer
    End Structure

    Public Apuestas As tAPuestas

    Public tInicioServer As Integer
    Public EstadisticasWeb As New clsEstadisticasIPC

    'INTERVALOS
    Public SanaIntervaloSinDescansar As Short
    Public StaminaIntervaloSinDescansar As Short
    Public SanaIntervaloDescansar As Short
    Public StaminaIntervaloDescansar As Short
    Public IntervaloSed As Short
    Public IntervaloHambre As Short
    Public IntervaloVeneno As Short
    Public IntervaloParalizado As Short
    Public IntervaloInvisible As Short
    Public IntervaloFrio As Short
    Public IntervaloWavFx As Short
    Public IntervaloLanzaHechizo As Short
    Public IntervaloNPCPuedeAtacar As Short
    Public IntervaloNPCAI As Short
    Public IntervaloInvocacion As Short
    Public IntervaloOculto As Short '[Nacho]
    Public IntervaloUserPuedeAtacar As Integer
    Public IntervaloGolpeUsar As Integer
    Public IntervaloMagiaGolpe As Integer
    Public IntervaloGolpeMagia As Integer
    Public IntervaloUserPuedeCastear As Integer
    Public IntervaloUserPuedeTrabajar As Integer
    Public IntervaloParaConexion As Integer
    Public IntervaloCerrarConexion As Integer '[Gonzalo]
    Public IntervaloUserPuedeUsar As Integer
    Public IntervaloFlechasCazadores As Integer
    Public IntervaloPuedeSerAtacado As Integer
    Public IntervaloAtacable As Integer
    Public IntervaloOwnedNpc As Integer

    'BALANCE

    Public PorcentajeRecuperoMana As Short

    Public MinutosWs As Integer
    Public Puerto As Short

    Public BootDelBackUp As Byte
    Public Lloviendo As Boolean
    Public DeNoche As Boolean

    Function VersionOK(ByVal Ver As String) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        VersionOK = (Ver = ULTIMAVERSION)
    End Function

    Sub ReSpawnOrigPosNpcs()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

        Dim i As Short
        'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura MiNPC, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Dim MiNPC As npc

        For i = 1 To LastNPC
            'OJO
            If Npclist(i).flags.NPCActive Then

                If _
                    InMapBounds(Npclist(i).Orig.Map, Npclist(i).Orig.X, Npclist(i).Orig.Y) And
                    Npclist(i).Numero = Guardias Then
                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto MiNPC. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    MiNPC = Npclist(i)
                    Call QuitarNPC(i)
                    Call ReSpawnNpc(MiNPC)
                End If

                'tildada por sugerencia de yind
                'If Npclist(i).Contadores.TiempoExistencia > 0 Then
                '        Call MuereNpc(i, 0)
                'End If
            End If

        Next i
    
Catch ex As Exception
    Console.WriteLine("Error in VersionOK: " & ex.Message)
End Try
End Sub

    Sub WorldSave()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

        Dim loopX As Short
        Dim Porc As Integer

        Call _
            SendData(modSendData.SendTarget.ToAll, 0,
                     PrepareMessageConsoleMsg("Servidor> Iniciando WorldSave", Protocol.FontTypeNames.FONTTYPE_SERVER))

        Call ReSpawnOrigPosNpcs() 'respawn de los guardias en las pos originales

        Dim j, k As Short

        For j = 1 To NumMaps
            If MapInfo_Renamed(j).BackUp = 1 Then k = k + 1
        Next j

        For loopX = 1 To NumMaps
            'DoEvents

            If MapInfo_Renamed(loopX).BackUp = 1 Then

                Call GrabarMapa(loopX, AppDomain.CurrentDomain.BaseDirectory & "WorldBackUp/Mapa" & loopX)
            End If

        Next loopX

        If FileExist(DatPath & "/bkNpc.dat") Then Kill((DatPath & "bkNpc.dat"))
        'If FileExist(DatPath & "/bkNPCs-HOSTILES.dat") Then Kill (DatPath & "bkNPCs-HOSTILES.dat")

        For loopX = 1 To LastNPC
            If Npclist(loopX).flags.BackUp = 1 Then
                Call BackUPnPc(loopX)
            End If
        Next

        Call SaveForums()

        Call _
            SendData(modSendData.SendTarget.ToAll, 0,
                     PrepareMessageConsoleMsg("Servidor> WorldSave ha concluído.",
                                              Protocol.FontTypeNames.FONTTYPE_SERVER))
    
Catch ex As Exception
    Console.WriteLine("Error in WorldSave: " & ex.Message)
End Try
End Sub

    Public Sub PurgarPenas()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim i As Integer

        For i = 1 To LastUser
            If UserList(i).flags.UserLogged Then
                If UserList(i).Counters.Pena > 0 Then
                    UserList(i).Counters.Pena = UserList(i).Counters.Pena - 1

                    If UserList(i).Counters.Pena < 1 Then
                        UserList(i).Counters.Pena = 0
                        Call WarpUserChar(i, Libertad.Map, Libertad.X, Libertad.Y, True)
                        Call WriteConsoleMsg(i, "¡Has sido liberado!", Protocol.FontTypeNames.FONTTYPE_INFO)

                        Call FlushBuffer(i)
                    End If
                End If
            End If
        Next i
    End Sub


    Public Sub Encarcelar(ByVal UserIndex As Short, ByVal Minutos As Integer,
                          Optional ByVal GmName As String = vbNullString)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        UserList(UserIndex).Counters.Pena = Minutos


        Call WarpUserChar(UserIndex, Prision.Map, Prision.X, Prision.Y, True)

        If migr_LenB(GmName) = 0 Then
            Call _
                WriteConsoleMsg(UserIndex,
                                "Has sido encarcelado, deberás permanecer en la cárcel " & Minutos & " minutos.",
                                Protocol.FontTypeNames.FONTTYPE_INFO)
        Else
            Call _
                WriteConsoleMsg(UserIndex,
                                GmName & " te ha encarcelado, deberás permanecer en la cárcel " & Minutos & " minutos.",
                                Protocol.FontTypeNames.FONTTYPE_INFO)
        End If
        If UserList(UserIndex).flags.Traveling = 1 Then
            UserList(UserIndex).flags.Traveling = 0
            UserList(UserIndex).Counters.goHome = 0
            Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.CancelHome)
        End If
    End Sub


    Public Sub BorrarUsuario(ByVal UserName As String)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try
        If FileExist(CharPath & UCase(UserName) & ".chr") Then
            Kill(CharPath & UCase(UserName) & ".chr")
        End If
    
Catch ex As Exception
    Console.WriteLine("Error in PurgarPenas: " & ex.Message)
End Try
End Sub

    Public Function BANCheck(ByVal name As String) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        BANCheck =
            (Val(GetVar(AppDomain.CurrentDomain.BaseDirectory & "charfile/" & name & ".chr", "FLAGS", "Ban")) = 1)
    End Function

    Public Function PersonajeExiste(ByVal name As String) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        PersonajeExiste = FileExist(CharPath & UCase(name) & ".chr")
    End Function

    Public Function UnBan(ByVal name As String) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'Unban the character
        Call WriteVar(AppDomain.CurrentDomain.BaseDirectory & "charfile/" & name & ".chr", "FLAGS", "Ban", "0")

        'Remove it from the banned people database
        Call WriteVar(AppDomain.CurrentDomain.BaseDirectory & "logs/" & "BanDetail.dat", name, "BannedBy", "NOBODY")
        Call WriteVar(AppDomain.CurrentDomain.BaseDirectory & "logs/" & "BanDetail.dat", name, "Reason", "NO REASON")
    End Function

    Public Function MD5ok(ByVal md5formateado As String) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim i As Short

        If MD5ClientesActivado = 1 Then
            For i = 0 To UBound(MD5s)
                If (md5formateado = MD5s(i)) Then
                    MD5ok = True
                    Exit Function
                End If
            Next i
            MD5ok = False
        Else
            MD5ok = True
        End If
    End Function

    Public Sub MD5sCarga()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim LoopC As Short

        MD5ClientesActivado = Val(GetVar(IniPath & "Server.ini", "MD5Hush", "Activado"))

        If MD5ClientesActivado = 1 Then
            ReDim MD5s(Val(GetVar(IniPath & "Server.ini", "MD5Hush", "MD5Aceptados")))
            For LoopC = 0 To UBound(MD5s)
                MD5s(LoopC) = GetVar(IniPath & "Server.ini", "MD5Hush", "MD5Aceptado" & (LoopC + 1))
                MD5s(LoopC) = txtOffset(hexMd52Asc(MD5s(LoopC)), 55)
            Next LoopC
        End If
    End Sub

    Public Sub BanIpAgrega(ByVal ip As String)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        BanIps.Add(ip)

        Call BanIpGuardar()
    End Sub

    Public Function BanIpBuscar(ByVal ip As String) As Integer
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim Dale As Boolean
        Dim LoopC As Integer

        Dale = True
        LoopC = 1
        Do While LoopC <= BanIps.Count() And Dale
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto BanIps.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Dale = (BanIps.Item(LoopC) <> ip)
            LoopC = LoopC + 1
        Loop

        If Dale Then
            BanIpBuscar = 0
        Else
            BanIpBuscar = LoopC - 1
        End If
    End Function

    Public Function BanIpQuita(ByVal ip As String) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

        Dim N As Integer

        N = BanIpBuscar(ip)
        If N > 0 Then
            BanIps.Remove(N)
            BanIpGuardar()
            BanIpQuita = True
        Else
            BanIpQuita = False
        End If
    
Catch ex As Exception
    Console.WriteLine("Error in BANCheck: " & ex.Message)
End Try
End Function

    Public Sub BanIpGuardar()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim ArchivoBanIp As String
        Dim ArchN As Integer
        Dim LoopC As Integer

        ArchivoBanIp = AppDomain.CurrentDomain.BaseDirectory & "Dat/BanIps.dat"

        ArchN = FreeFile
        FileOpen(ArchN, ArchivoBanIp, OpenMode.Output)

        For LoopC = 1 To BanIps.Count()
            PrintLine(ArchN, BanIps.Item(LoopC))
        Next LoopC

        FileClose(ArchN)
    End Sub

    Public Sub BanIpCargar()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim ArchN As Integer
        Dim Tmp As String
        Dim ArchivoBanIp As String

        ArchivoBanIp = AppDomain.CurrentDomain.BaseDirectory & "Dat/BanIps.dat"

        Do While BanIps.Count() > 0
            BanIps.Remove(1)
        Loop

        ArchN = FreeFile
        FileOpen(ArchN, ArchivoBanIp, OpenMode.Input)

        Do While Not EOF(ArchN)
            Tmp = LineInput(ArchN)
            BanIps.Add(Tmp)
        Loop

        FileClose(ArchN)
    End Sub

    Public Sub ActualizaEstadisticasWeb()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Static Andando As Boolean
        Static Contador As Integer
        Dim Tmp As Boolean

        Contador = Contador + 1

        If Contador >= 10 Then
            Contador = 0
            Tmp = EstadisticasWeb.EstadisticasAndando()

            If Andando = False And Tmp = True Then
                Call InicializaEstadisticas()
            End If

            Andando = Tmp
        End If
    End Sub

    Public Function UserDarPrivilegioLevel(ByVal name As String) As Declaraciones.PlayerType
        '***************************************************
        'Author: Unknown
        'Last Modification: 03/02/07
        'Last Modified By: Juan Martín Sotuyo Dodero (Maraxus)
        '***************************************************

        If EsAdmin(name) Then
            UserDarPrivilegioLevel = Declaraciones.PlayerType.Admin
        ElseIf EsDios(name) Then
            UserDarPrivilegioLevel = Declaraciones.PlayerType.Dios
        ElseIf EsSemiDios(name) Then
            UserDarPrivilegioLevel = Declaraciones.PlayerType.SemiDios
        ElseIf EsConsejero(name) Then
            UserDarPrivilegioLevel = Declaraciones.PlayerType.Consejero
        Else
            UserDarPrivilegioLevel = Declaraciones.PlayerType.User
        End If
    End Function

    Public Sub BanCharacter(ByVal bannerUserIndex As Short, ByVal UserName As String, ByVal reason As String)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 03/02/07
        '
        '***************************************************

        Dim tUser As Short
        Dim userPriv As Byte
        Dim cantPenas As Byte
        Dim rank As Short

        If migr_InStrB(UserName, "+") Then
            UserName = Replace(UserName, "+", " ")
        End If

        tUser = NameIndex(UserName)

        rank = Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios Or
               Declaraciones.PlayerType.Consejero

        With UserList(bannerUserIndex)
            If tUser <= 0 Then
                Call _
                    WriteConsoleMsg(bannerUserIndex, "El usuario no está online.", Protocol.FontTypeNames.FONTTYPE_TALK)

                If FileExist(CharPath & UserName & ".chr") Then
                    userPriv = UserDarPrivilegioLevel(UserName)

                    If (userPriv And rank) > (.flags.Privilegios And rank) Then
                        Call _
                            WriteConsoleMsg(bannerUserIndex, "No puedes banear a al alguien de mayor jerarquía.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                    Else
                        If GetVar(CharPath & UserName & ".chr", "FLAGS", "Ban") <> "0" Then
                            Call _
                                WriteConsoleMsg(bannerUserIndex, "El personaje ya se encuentra baneado.",
                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                        Else
                            Call LogBanFromName(UserName, bannerUserIndex, reason)
                            Call _
                                SendData(modSendData.SendTarget.ToAdmins, 0,
                                         PrepareMessageConsoleMsg(
                                             "Servidor> " & .name & " ha baneado a " & UserName & ".",
                                             Protocol.FontTypeNames.FONTTYPE_SERVER))

                            'ponemos el flag de ban a 1
                            Call WriteVar(CharPath & UserName & ".chr", "FLAGS", "Ban", "1")
                            'ponemos la pena
                            cantPenas = Val(GetVar(CharPath & UserName & ".chr", "PENAS", "Cant"))
                            Call WriteVar(CharPath & UserName & ".chr", "PENAS", "Cant", CStr(cantPenas + 1))
                            Call _
                                WriteVar(CharPath & UserName & ".chr", "PENAS", "P" & cantPenas + 1,
                                         LCase(.name) & ": BAN POR " & LCase(reason) & " " & Today & " " & TimeOfDay)

                            If (userPriv And rank) = (.flags.Privilegios And rank) Then
                                .flags.Ban = 1
                                Call _
                                    SendData(modSendData.SendTarget.ToAdmins, 0,
                                             PrepareMessageConsoleMsg(
                                                 .name & " banned by the server por bannear un Administrador.",
                                                 Protocol.FontTypeNames.FONTTYPE_FIGHT))
                                Call CloseSocket(bannerUserIndex)
                            End If

                            Call LogGM(.name, "BAN a " & UserName)
                        End If
                    End If
                Else
                    Call _
                        WriteConsoleMsg(bannerUserIndex, "El pj " & UserName & " no existe.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                End If
            Else
                If (UserList(tUser).flags.Privilegios And rank) > (.flags.Privilegios And rank) Then
                    Call _
                        WriteConsoleMsg(bannerUserIndex, "No puedes banear a al alguien de mayor jerarquía.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                End If

                Call LogBan(tUser, bannerUserIndex, reason)
                Call _
                    SendData(modSendData.SendTarget.ToAdmins, 0,
                             PrepareMessageConsoleMsg(
                                 "Servidor> " & .name & " ha baneado a " & UserList(tUser).name & ".",
                                 Protocol.FontTypeNames.FONTTYPE_SERVER))

                'Ponemos el flag de ban a 1
                UserList(tUser).flags.Ban = 1

                If (UserList(tUser).flags.Privilegios And rank) = (.flags.Privilegios And rank) Then
                    .flags.Ban = 1
                    Call _
                        SendData(modSendData.SendTarget.ToAdmins, 0,
                                 PrepareMessageConsoleMsg(.name & " banned by the server por bannear un Administrador.",
                                                          Protocol.FontTypeNames.FONTTYPE_FIGHT))
                    Call CloseSocket(bannerUserIndex)
                End If

                Call LogGM(.name, "BAN a " & UserName)

                'ponemos el flag de ban a 1
                Call WriteVar(CharPath & UserName & ".chr", "FLAGS", "Ban", "1")
                'ponemos la pena
                cantPenas = Val(GetVar(CharPath & UserName & ".chr", "PENAS", "Cant"))
                Call WriteVar(CharPath & UserName & ".chr", "PENAS", "Cant", CStr(cantPenas + 1))
                Call _
                    WriteVar(CharPath & UserName & ".chr", "PENAS", "P" & cantPenas + 1,
                             LCase(.name) & ": BAN POR " & LCase(reason) & " " & Today & " " & TimeOfDay)

                Call CloseSocket(tUser)
            End If
        End With
    End Sub
End Module
