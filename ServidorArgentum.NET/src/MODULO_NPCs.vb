Option Strict Off
Option Explicit On
Module NPCs
    'Argentum Online 0.12.2
    'Copyright (C) 2002 Márquez Pablo Ignacio
    '
    'This program is free software; you can redistribute it and/or modify
    'it under the terms of the Affero General Public License;
    'either version 1 of the License, or any later version.
    '
    'This program is distributed in the hope that it will be useful,
    'but WITHOUT ANY WARRANTY; without even the implied warranty of
    'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    'Affero General Public License for more details.
    '
    'You should have received a copy of the Affero General Public License
    'along with this program; if not, you can find it at http://www.affero.org/oagpl.html
    '
    'Argentum Online is based on Baronsoft's VB6 Online RPG
    'You can contact the original creator of ORE at aaron@baronsoft.com
    'for more information about ORE please visit http://www.baronsoft.com/
    '
    '
    'You can contact me at:
    'morgolock@speedy.com.ar
    'www.geocities.com/gmorgolock
    'Calle 3 número 983 piso 7 dto A
    'La Plata - Pcia, Buenos Aires - Republica Argentina
    'Código Postal 1900
    'Pablo Ignacio Márquez

    '?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
    '?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
    '?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
    '                        Modulo NPC
    '?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
    '?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
    '?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
    'Contiene todas las rutinas necesarias para cotrolar los
    'NPCs meno la rutina de AI que se encuentra en el modulo
    'AI_NPCs para su mejor comprension.
    '?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
    '?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
    '?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿

    Sub QuitarMascota(ByVal UserIndex As Short, ByVal NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim i As Short

        For i = 1 To MAXMASCOTAS
            If UserList(UserIndex).MascotasIndex(i) = NpcIndex Then
                UserList(UserIndex).MascotasIndex(i) = 0
                UserList(UserIndex).MascotasType(i) = 0

                UserList(UserIndex).NroMascotas = UserList(UserIndex).NroMascotas - 1
                Exit For
            End If
        Next i
    End Sub

    Sub QuitarMascotaNpc(ByVal Maestro As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Npclist(Maestro).Mascotas = Npclist(Maestro).Mascotas - 1
    End Sub

    Sub MuereNpc(ByVal NpcIndex As Short, ByVal UserIndex As Short)
        '********************************************************
        'Author: Unknown
        'Llamado cuando la vida de un NPC llega a cero.
        'Last Modify Date: 24/01/2007
        '22/06/06: (Nacho) Chequeamos si es pretoriano
        '24/01/2007: Pablo (ToxicWaste): Agrego para actualización de tag si cambia de status.
        '********************************************************
        On Error GoTo Errhandler
        'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura MiNPC, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Dim MiNPC As npc
        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto MiNPC. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        MiNPC = Npclist(NpcIndex)
        Dim EraCriminal As Boolean
        Dim IsPretoriano As Boolean

        Dim i As Short
        Dim j As Short
        Dim NPCI As Short
        If (esPretoriano(NpcIndex) = 4) Then
            'Solo nos importa si fue matado en el mapa pretoriano.
            IsPretoriano = True
            If Npclist(NpcIndex).Pos.Map = MAPA_PRETORIANO Then
                'seteamos todos estos 'flags' acorde para que cambien solos de alcoba

                For i = 8 To 90
                    For j = 8 To 90

                        NPCI = MapData(Npclist(NpcIndex).Pos.Map, i, j).NpcIndex
                        If NPCI > 0 Then
                            If esPretoriano(NPCI) > 0 And NPCI <> NpcIndex Then
                                If Npclist(NpcIndex).Pos.X > 50 Then
                                    If Npclist(NPCI).Pos.X > 50 Then Npclist(NPCI).Invent.ArmourEqpSlot = 1
                                Else
                                    If Npclist(NPCI).Pos.X <= 50 Then Npclist(NPCI).Invent.ArmourEqpSlot = 5
                                End If
                            End If
                        End If
                    Next j
                Next i
                Call CrearClanPretoriano(Npclist(NpcIndex).Pos.X)
            End If
        ElseIf esPretoriano(NpcIndex) > 0 Then
            IsPretoriano = True
            If Npclist(NpcIndex).Pos.Map = MAPA_PRETORIANO Then
                Npclist(NpcIndex).Invent.ArmourEqpSlot = 0
                pretorianosVivos = pretorianosVivos - 1
            End If
        End If

        'Quitamos el npc
        Call QuitarNPC(NpcIndex)


        Dim T As Short
        If UserIndex > 0 Then ' Lo mato un usuario?
            With UserList(UserIndex)

                If MiNPC.flags.Snd3 > 0 Then
                    Call _
                        SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                 PrepareMessagePlayWave(MiNPC.flags.Snd3, MiNPC.Pos.X, MiNPC.Pos.Y))
                End If
                .flags.TargetNPC = 0
                .flags.TargetNpcTipo = Declaraciones.eNPCType.Comun

                'El user que lo mato tiene mascotas?
                If .NroMascotas > 0 Then
                    For T = 1 To MAXMASCOTAS
                        If .MascotasIndex(T) > 0 Then
                            If Npclist(.MascotasIndex(T)).TargetNPC = NpcIndex Then
                                Call FollowAmo(.MascotasIndex(T))
                            End If
                        End If
                    Next T
                End If

                '[KEVIN]
                If MiNPC.flags.ExpCount > 0 Then
                    If .PartyIndex > 0 Then
                        Call _
                            mdParty.ObtenerExito(UserIndex, MiNPC.flags.ExpCount, MiNPC.Pos.Map, MiNPC.Pos.X,
                                                 MiNPC.Pos.Y)
                    Else
                        .Stats.Exp = .Stats.Exp + MiNPC.flags.ExpCount
                        If .Stats.Exp > MAXEXP Then .Stats.Exp = MAXEXP
                        Call _
                            WriteConsoleMsg(UserIndex, "Has ganado " & MiNPC.flags.ExpCount & " puntos de experiencia.",
                                            Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    End If
                    MiNPC.flags.ExpCount = 0
                End If

                '[/KEVIN]
                Call WriteConsoleMsg(UserIndex, "¡Has matado a la criatura!", Protocol.FontTypeNames.FONTTYPE_FIGHT)
                If .Stats.NPCsMuertos < 32000 Then .Stats.NPCsMuertos = .Stats.NPCsMuertos + 1

                EraCriminal = criminal(UserIndex)

                If MiNPC.Stats.Alineacion = 0 Then

                    If MiNPC.Numero = Guardias Then
                        .Reputacion.NobleRep = 0
                        .Reputacion.PlebeRep = 0
                        .Reputacion.AsesinoRep = .Reputacion.AsesinoRep + 500
                        If .Reputacion.AsesinoRep > MAXREP Then .Reputacion.AsesinoRep = MAXREP
                    End If

                    If MiNPC.MaestroUser = 0 Then
                        .Reputacion.AsesinoRep = .Reputacion.AsesinoRep + vlASESINO
                        If .Reputacion.AsesinoRep > MAXREP Then .Reputacion.AsesinoRep = MAXREP
                    End If
                ElseIf MiNPC.Stats.Alineacion = 1 Then
                    .Reputacion.PlebeRep = .Reputacion.PlebeRep + vlCAZADOR
                    If .Reputacion.PlebeRep > MAXREP Then .Reputacion.PlebeRep = MAXREP

                ElseIf MiNPC.Stats.Alineacion = 2 Then
                    .Reputacion.NobleRep = .Reputacion.NobleRep + vlASESINO/2
                    If .Reputacion.NobleRep > MAXREP Then .Reputacion.NobleRep = MAXREP

                ElseIf MiNPC.Stats.Alineacion = 4 Then
                    .Reputacion.PlebeRep = .Reputacion.PlebeRep + vlCAZADOR
                    If .Reputacion.PlebeRep > MAXREP Then .Reputacion.PlebeRep = MAXREP

                End If

                If criminal(UserIndex) And esArmada(UserIndex) Then Call ExpulsarFaccionReal(UserIndex)
                If Not criminal(UserIndex) And esCaos(UserIndex) Then Call ExpulsarFaccionCaos(UserIndex)

                If EraCriminal And Not criminal(UserIndex) Then
                    Call RefreshCharStatus(UserIndex)
                ElseIf Not EraCriminal And criminal(UserIndex) Then
                    Call RefreshCharStatus(UserIndex)
                End If

                Call CheckUserLevel(UserIndex)

            End With
        End If ' Userindex > 0

        If MiNPC.MaestroUser = 0 Then
            'Tiramos el oro
            ' Call NPCTirarOro(MiNPC)
            'Tiramos el inventario
            Call NPC_TIRAR_ITEMS(MiNPC, IsPretoriano)
            'ReSpawn o no
            Call ReSpawnNpc(MiNPC)
        End If


        Exit Sub

        Errhandler:
        Call LogError("Error en MuereNpc - Error: " & Err.Number & " - Desc: " & Err.Description)
    End Sub

    Private Sub ResetNpcFlags(ByVal NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'Clear the npc's flags

        With Npclist(NpcIndex).flags
            .AfectaParalisis = 0
            .AguaValida = 0
            .AttackedBy = vbNullString
            .AttackedFirstBy = vbNullString
            .BackUp = 0
            .Bendicion = 0
            .Domable = 0
            .Envenenado = 0
            .Faccion = 0
            .Follow = False
            .AtacaDoble = 0
            .LanzaSpells = 0
            .invisible = 0
            .Maldicion = 0
            .OldHostil = 0
            .OldMovement = 0
            .Paralizado = 0
            .Inmovilizado = 0
            .Respawn = 0
            .RespawnOrigPos = 0
            .Snd1 = 0
            .Snd2 = 0
            .Snd3 = 0
            .TierraInvalida = 0
        End With
    End Sub

    Private Sub ResetNpcCounters(ByVal NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        With Npclist(NpcIndex).Contadores
            .Paralisis = 0
            .TiempoExistencia = 0
        End With
    End Sub

    Private Sub ResetNpcCharInfo(ByVal NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        With Npclist(NpcIndex).Char_Renamed
            .body = 0
            .CascoAnim = 0
            .CharIndex = 0
            .FX = 0
            .Head = 0
            .heading = 0
            .loops = 0
            .ShieldAnim = 0
            .WeaponAnim = 0
        End With
    End Sub

    Private Sub ResetNpcCriatures(ByVal NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim j As Integer

        With Npclist(NpcIndex)
            For j = 1 To .NroCriaturas
                .Criaturas(j).NpcIndex = 0
                .Criaturas(j).NpcName = vbNullString
            Next j

            .NroCriaturas = 0
        End With
    End Sub

    Sub ResetExpresiones(ByVal NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim j As Integer

        With Npclist(NpcIndex)
            For j = 1 To .NroExpresiones
                .Expresiones(j) = vbNullString
            Next j

            .NroExpresiones = 0
        End With
    End Sub

    Private Sub ResetNpcMainInfo(ByVal NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim j As Integer
        With Npclist(NpcIndex)
            .Attackable = 0
            .CanAttack = 0
            .Comercia = 0
            .GiveEXP = 0
            .GiveGLD = 0
            .Hostile = 0
            .InvReSpawn = 0

            If .MaestroUser > 0 Then Call QuitarMascota(.MaestroUser, NpcIndex)
            If .MaestroNpc > 0 Then Call QuitarMascotaNpc(.MaestroNpc)

            .MaestroUser = 0
            .MaestroNpc = 0

            .Mascotas = 0
            .Movement = 0
            .name = vbNullString
            .NPCtype = 0
            .Numero = 0
            .Orig.Map = 0
            .Orig.X = 0
            .Orig.Y = 0
            .PoderAtaque = 0
            .PoderEvasion = 0
            .Pos.Map = 0
            .Pos.X = 0
            .Pos.Y = 0
            .SkillDomar = 0
            .Target = 0
            .TargetNPC = 0
            .TipoItems = 0
            .Veneno = 0
            .desc = vbNullString


            For j = 1 To .NroSpells
                .Spells(j) = 0
            Next j
        End With

        Call ResetNpcCharInfo(NpcIndex)
        Call ResetNpcCriatures(NpcIndex)
        Call ResetExpresiones(NpcIndex)
    End Sub

    Public Sub QuitarNPC(ByVal NpcIndex As Short)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 16/11/2009
        '16/11/2009: ZaMa - Now npcs lose their owner
        '***************************************************
        On Error GoTo Errhandler

        With Npclist(NpcIndex)
            .flags.NPCActive = False

            .Owner = 0 ' Murio, no necesita mas dueños :P.

            If InMapBounds(.Pos.Map, .Pos.X, .Pos.Y) Then
                Call EraseNPCChar(NpcIndex)
            End If
        End With

        'Nos aseguramos de que el inventario sea removido...
        'asi los lobos no volveran a tirar armaduras ;))
        Call ResetNpcInv(NpcIndex)
        Call ResetNpcFlags(NpcIndex)
        Call ResetNpcCounters(NpcIndex)

        Call ResetNpcMainInfo(NpcIndex)

        If NpcIndex = LastNPC Then
            Do Until Npclist(LastNPC).flags.NPCActive
                LastNPC = LastNPC - 1
                If LastNPC < 1 Then Exit Do
            Loop
        End If


        If NumNPCs <> 0 Then
            NumNPCs = NumNPCs - 1
        End If
        Exit Sub

        Errhandler:
        Call LogError("Error en QuitarNPC")
    End Sub

    Public Sub QuitarPet(ByVal UserIndex As Short, ByVal NpcIndex As Short)
        '***************************************************
        'Autor: ZaMa
        'Last Modification: 18/11/2009
        'Kills a pet
        '***************************************************
        On Error GoTo Errhandler

        Dim i As Short
        Dim PetIndex As Short

        With UserList(UserIndex)

            ' Busco el indice de la mascota
            For i = 1 To MAXMASCOTAS
                If .MascotasIndex(i) = NpcIndex Then PetIndex = i
            Next i

            ' Poco probable que pase, pero por las dudas..
            If PetIndex = 0 Then Exit Sub

            ' Limpio el slot de la mascota
            .NroMascotas = .NroMascotas - 1
            .MascotasIndex(PetIndex) = 0
            .MascotasType(PetIndex) = 0

            ' Elimino la mascota
            Call QuitarNPC(NpcIndex)
        End With

        Exit Sub

        Errhandler:
        Call _
            LogError(
                "Error en QuitarPet. Error: " & Err.Number & " Desc: " & Err.Description & " NpcIndex: " & NpcIndex &
                " UserIndex: " & UserIndex & " PetIndex: " & PetIndex)
    End Sub

    Private Function TestSpawnTrigger(ByRef Pos As WorldPos, Optional ByRef PuedeAgua As Boolean = False) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        If LegalPos(Pos.Map, Pos.X, Pos.Y, PuedeAgua) Then
            TestSpawnTrigger = MapData(Pos.Map, Pos.X, Pos.Y).trigger <> 3 And
                               MapData(Pos.Map, Pos.X, Pos.Y).trigger <> 2 And
                               MapData(Pos.Map, Pos.X, Pos.Y).trigger <> 1
        End If
    End Function

    Sub CrearNPC(ByRef NroNPC As Short, ByRef mapa As Short, ByRef OrigPos As WorldPos)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'Crea un NPC del tipo NRONPC

        Dim Pos As WorldPos
        Dim newpos As WorldPos
        Dim altpos As WorldPos
        Dim nIndex As Short
        Dim PosicionValida As Boolean
        Dim Iteraciones As Integer
        Dim PuedeAgua As Boolean
        Dim PuedeTierra As Boolean


        Dim Map As Short
        Dim X As Short
        Dim Y As Short

        nIndex = OpenNPC(NroNPC) 'Conseguimos un indice

        If nIndex > MAXNPCS Then Exit Sub
        PuedeAgua = Npclist(nIndex).flags.AguaValida
        PuedeTierra = IIf(Npclist(nIndex).flags.TierraInvalida = 1, False, True)

        'Necesita ser respawned en un lugar especifico
        If InMapBounds(OrigPos.Map, OrigPos.X, OrigPos.Y) Then

            Map = OrigPos.Map
            X = OrigPos.X
            Y = OrigPos.Y
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Npclist().Orig. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Npclist(nIndex).Orig = OrigPos
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Npclist().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Npclist(nIndex).Pos = OrigPos

        Else

            Pos.Map = mapa 'mapa
            altpos.Map = mapa

            Do While Not PosicionValida
                Pos.X = RandomNumber(MinXBorder, MaxXBorder) 'Obtenemos posicion al azar en x
                Pos.Y = RandomNumber(MinYBorder, MaxYBorder) 'Obtenemos posicion al azar en y

                Call ClosestLegalPos(Pos, newpos, PuedeAgua, PuedeTierra) 'Nos devuelve la posicion valida mas cercana
                If newpos.X <> 0 And newpos.Y <> 0 Then
                    altpos.X = newpos.X
                    altpos.Y = newpos.Y _
                    'posicion alternativa (para evitar el anti respawn, pero intentando qeu si tenía que ser en el agua, sea en el agua.)
                Else
                    Call ClosestLegalPos(Pos, newpos, PuedeAgua)
                    If newpos.X <> 0 And newpos.Y <> 0 Then
                        altpos.X = newpos.X
                        altpos.Y = newpos.Y 'posicion alternativa (para evitar el anti respawn)
                    End If
                End If
                'Si X e Y son iguales a 0 significa que no se encontro posicion valida
                If _
                    LegalPosNPC(newpos.Map, newpos.X, newpos.Y, PuedeAgua) And Not HayPCarea(newpos) And
                    TestSpawnTrigger(newpos, PuedeAgua) Then
                    'Asignamos las nuevas coordenas solo si son validas
                    Npclist(nIndex).Pos.Map = newpos.Map
                    Npclist(nIndex).Pos.X = newpos.X
                    Npclist(nIndex).Pos.Y = newpos.Y
                    PosicionValida = True
                Else
                    newpos.X = 0
                    newpos.Y = 0

                End If


                'for debug
                Iteraciones = Iteraciones + 1
                If Iteraciones > MAXSPAWNATTEMPS Then
                    If altpos.X <> 0 And altpos.Y <> 0 Then
                        Map = altpos.Map
                        X = altpos.X
                        Y = altpos.Y
                        Npclist(nIndex).Pos.Map = Map
                        Npclist(nIndex).Pos.X = X
                        Npclist(nIndex).Pos.Y = Y
                        Call MakeNPCChar(True, Map, nIndex, Map, X, Y)
                        Exit Sub
                    Else
                        altpos.X = 50
                        altpos.Y = 50
                        Call ClosestLegalPos(altpos, newpos)
                        If newpos.X <> 0 And newpos.Y <> 0 Then
                            Npclist(nIndex).Pos.Map = newpos.Map
                            Npclist(nIndex).Pos.X = newpos.X
                            Npclist(nIndex).Pos.Y = newpos.Y
                            Call MakeNPCChar(True, newpos.Map, nIndex, newpos.Map, newpos.X, newpos.Y)
                            Exit Sub
                        Else
                            Call QuitarNPC(nIndex)
                            Call _
                                LogError(MAXSPAWNATTEMPS & " iteraciones en CrearNpc Mapa:" & mapa & " NroNpc:" & NroNPC)
                            Exit Sub
                        End If
                    End If
                End If
            Loop

            'asignamos las nuevas coordenas
            Map = newpos.Map
            X = Npclist(nIndex).Pos.X
            Y = Npclist(nIndex).Pos.Y
        End If

        'Crea el NPC
        Call MakeNPCChar(True, Map, nIndex, Map, X, Y)
    End Sub

    Public Sub MakeNPCChar(ByVal toMap As Boolean, ByRef sndIndex As Short, ByRef NpcIndex As Short, ByVal Map As Short,
                           ByVal X As Short, ByVal Y As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim CharIndex As Short

        If Npclist(NpcIndex).Char_Renamed.CharIndex = 0 Then
            CharIndex = NextOpenCharIndex
            Npclist(NpcIndex).Char_Renamed.CharIndex = CharIndex
            CharList(CharIndex) = NpcIndex
        End If

        MapData(Map, X, Y).NpcIndex = NpcIndex

        If Not toMap Then
            Call _
                WriteCharacterCreate(sndIndex, Npclist(NpcIndex).Char_Renamed.body, Npclist(NpcIndex).Char_Renamed.Head,
                                     Npclist(NpcIndex).Char_Renamed.heading, Npclist(NpcIndex).Char_Renamed.CharIndex, X,
                                     Y, 0, 0, 0, 0, 0, vbNullString, 0, 0)
            Call FlushBuffer(sndIndex)
        Else
            Call AgregarNpc(NpcIndex)
        End If
    End Sub

    Public Sub ChangeNPCChar(ByVal NpcIndex As Short, ByVal body As Short, ByVal Head As Short,
                             ByVal heading As Declaraciones.eHeading)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        If NpcIndex > 0 Then
            With Npclist(NpcIndex).Char_Renamed
                .body = body
                .Head = Head
                .heading = heading

                Call _
                    SendData(modSendData.SendTarget.ToNPCArea, NpcIndex,
                             PrepareMessageCharacterChange(body, Head, heading, .CharIndex, 0, 0, 0, 0, 0))
            End With
        End If
    End Sub

    Private Sub EraseNPCChar(ByVal NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        If Npclist(NpcIndex).Char_Renamed.CharIndex <> 0 Then CharList(Npclist(NpcIndex).Char_Renamed.CharIndex) = 0

        If Npclist(NpcIndex).Char_Renamed.CharIndex = LastChar Then
            Do Until CharList(LastChar) > 0
                LastChar = LastChar - 1
                If LastChar <= 1 Then Exit Do
            Loop
        End If

        'Quitamos del mapa
        MapData(Npclist(NpcIndex).Pos.Map, Npclist(NpcIndex).Pos.X, Npclist(NpcIndex).Pos.Y).NpcIndex = 0

        'Actualizamos los clientes
        Call _
            SendData(modSendData.SendTarget.ToNPCArea, NpcIndex,
                     PrepareMessageCharacterRemove(Npclist(NpcIndex).Char_Renamed.CharIndex))

        'Update la lista npc
        Npclist(NpcIndex).Char_Renamed.CharIndex = 0


        'update NumChars
        NumChars = NumChars - 1
    End Sub

    Public Sub MoveNPCChar(ByVal NpcIndex As Short, ByVal nHeading As Byte)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 06/04/2009
        '06/04/2009: ZaMa - Now npcs can force to change position with dead character
        '01/08/2009: ZaMa - Now npcs can't force to chance position with a dead character if that means to change the terrain the character is in
        '***************************************************

        On Error GoTo errh
        Dim nPos As WorldPos
        Dim UserIndex As Short

        With Npclist(NpcIndex)
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            nPos = .Pos
            Call HeadtoPos(nHeading, nPos)

            ' es una posicion legal
            If LegalPosNPC(.Pos.Map, nPos.X, nPos.Y, .flags.AguaValida = 1, .MaestroUser <> 0) Then

                If .flags.AguaValida = 0 And HayAgua(.Pos.Map, nPos.X, nPos.Y) Then Exit Sub
                If .flags.TierraInvalida = 1 And Not HayAgua(.Pos.Map, nPos.X, nPos.Y) Then Exit Sub

                UserIndex = MapData(.Pos.Map, nPos.X, nPos.Y).UserIndex
                ' Si hay un usuario a donde se mueve el npc, entonces esta muerto
                If UserIndex > 0 Then

                    ' No se traslada caspers de agua a tierra
                    If HayAgua(.Pos.Map, nPos.X, nPos.Y) And Not HayAgua(.Pos.Map, .Pos.X, .Pos.Y) Then Exit Sub
                    ' No se traslada caspers de tierra a agua
                    If Not HayAgua(.Pos.Map, nPos.X, nPos.Y) And HayAgua(.Pos.Map, .Pos.X, .Pos.Y) Then Exit Sub

                    With UserList(UserIndex)
                        ' Actualizamos posicion y mapa
                        MapData(.Pos.Map, .Pos.X, .Pos.Y).UserIndex = 0
                        .Pos.X = Npclist(NpcIndex).Pos.X
                        .Pos.Y = Npclist(NpcIndex).Pos.Y
                        MapData(.Pos.Map, .Pos.X, .Pos.Y).UserIndex = UserIndex

                        ' Avisamos a los usuarios del area, y al propio usuario lo forzamos a moverse
                        Call _
                            SendData(modSendData.SendTarget.ToPCAreaButIndex, UserIndex,
                                     PrepareMessageCharacterMove(UserList(UserIndex).Char_Renamed.CharIndex, .Pos.X,
                                                                 .Pos.Y))
                        Call WriteForceCharMove(UserIndex, InvertHeading(nHeading))
                    End With
                End If

                Call _
                    SendData(modSendData.SendTarget.ToNPCArea, NpcIndex,
                             PrepareMessageCharacterMove(.Char_Renamed.CharIndex, nPos.X, nPos.Y))

                'Update map and user pos
                MapData(.Pos.Map, .Pos.X, .Pos.Y).NpcIndex = 0
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Npclist().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                .Pos = nPos
                .Char_Renamed.heading = nHeading
                MapData(.Pos.Map, nPos.X, nPos.Y).NpcIndex = NpcIndex
                Call CheckUpdateNeededNpc(NpcIndex, nHeading)

            ElseIf .MaestroUser = 0 Then
                If .Movement = AI.TipoAI.NpcPathfinding Then
                    'Someone has blocked the npc's way, we must to seek a new path!
                    .PFINFO.PathLenght = 0
                End If
            End If
        End With
        Exit Sub

        errh:
        LogError(("Error en move npc " & NpcIndex))
    End Sub

    Function NextOpenNPC() As Short
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        On Error GoTo Errhandler
        Dim LoopC As Integer

        For LoopC = 1 To MAXNPCS + 1
            If LoopC > MAXNPCS Then Exit For
            If Not Npclist(LoopC).flags.NPCActive Then Exit For
        Next LoopC

        NextOpenNPC = LoopC
        Exit Function

        Errhandler:
        Call LogError("Error en NextOpenNPC")
    End Function

    Sub NpcEnvenenarUser(ByVal UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim N As Short
        N = RandomNumber(1, 100)
        If N < 30 Then
            UserList(UserIndex).flags.Envenenado = 1
            Call WriteConsoleMsg(UserIndex, "¡¡La criatura te ha envenenado!!", Protocol.FontTypeNames.FONTTYPE_FIGHT)
        End If
    End Sub

    Function SpawnNpc(ByVal NpcIndex As Short, ByRef Pos As WorldPos, ByVal FX As Boolean, ByVal Respawn As Boolean) _
        As Short
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 06/15/2008
        '23/01/2007 -> Pablo (ToxicWaste): Creates an NPC of the type Npcindex
        '06/15/2008 -> Optimizé el codigo. (NicoNZ)
        '***************************************************
        Dim newpos As WorldPos
        Dim altpos As WorldPos
        Dim nIndex As Short
        Dim PosicionValida As Boolean
        Dim PuedeAgua As Boolean
        Dim PuedeTierra As Boolean


        Dim Map As Short
        Dim X As Short
        Dim Y As Short

        nIndex = OpenNPC(NpcIndex, Respawn) 'Conseguimos un indice

        If nIndex > MAXNPCS Then
            SpawnNpc = 0
            Exit Function
        End If

        PuedeAgua = Npclist(nIndex).flags.AguaValida
        PuedeTierra = Not Npclist(nIndex).flags.TierraInvalida = 1

        Call ClosestLegalPos(Pos, newpos, PuedeAgua, PuedeTierra) 'Nos devuelve la posicion valida mas cercana
        Call ClosestLegalPos(Pos, altpos, PuedeAgua)
        'Si X e Y son iguales a 0 significa que no se encontro posicion valida

        If newpos.X <> 0 And newpos.Y <> 0 Then
            'Asignamos las nuevas coordenas solo si son validas
            Npclist(nIndex).Pos.Map = newpos.Map
            Npclist(nIndex).Pos.X = newpos.X
            Npclist(nIndex).Pos.Y = newpos.Y
            PosicionValida = True
        Else
            If altpos.X <> 0 And altpos.Y <> 0 Then
                Npclist(nIndex).Pos.Map = altpos.Map
                Npclist(nIndex).Pos.X = altpos.X
                Npclist(nIndex).Pos.Y = altpos.Y
                PosicionValida = True
            Else
                PosicionValida = False
            End If
        End If

        If Not PosicionValida Then
            Call QuitarNPC(nIndex)
            SpawnNpc = 0
            Exit Function
        End If

        'asignamos las nuevas coordenas
        Map = newpos.Map
        X = Npclist(nIndex).Pos.X
        Y = Npclist(nIndex).Pos.Y

        'Crea el NPC
        Call MakeNPCChar(True, Map, nIndex, Map, X, Y)

        If FX Then
            Call SendData(modSendData.SendTarget.ToNPCArea, nIndex, PrepareMessagePlayWave(SND_WARP, X, Y))
            Call _
                SendData(modSendData.SendTarget.ToNPCArea, nIndex,
                         PrepareMessageCreateFX(Npclist(nIndex).Char_Renamed.CharIndex, Declaraciones.FXIDs.FXWARP, 0))
        End If

        SpawnNpc = nIndex
    End Function

    Sub ReSpawnNpc(ByRef MiNPC As npc)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        If (MiNPC.flags.Respawn = 0) Then Call CrearNPC(MiNPC.Numero, MiNPC.Pos.Map, MiNPC.Orig)
    End Sub

    Private Sub NPCTirarOro(ByRef MiNPC As npc)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'SI EL NPC TIENE ORO LO TIRAMOS
        Dim MiObj As Obj
        Dim MiAux As Integer
        If MiNPC.GiveGLD > 0 Then
            MiAux = MiNPC.GiveGLD
            Do While MiAux > MAX_INVENTORY_OBJS
                MiObj.Amount = MAX_INVENTORY_OBJS
                MiObj.ObjIndex = iORO
                Call TirarItemAlPiso(MiNPC.Pos, MiObj)
                MiAux = MiAux - MAX_INVENTORY_OBJS
            Loop
            If MiAux > 0 Then
                MiObj.Amount = MiAux
                MiObj.ObjIndex = iORO
                Call TirarItemAlPiso(MiNPC.Pos, MiObj)
            End If
        End If
    End Sub

    Public Function OpenNPC(ByVal NpcNumber As Short, Optional ByVal Respawn As Object = True) As Short
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        '###################################################
        '#               ATENCION PELIGRO                  #
        '###################################################
        '
        '    ¡¡¡¡ NO USAR GetVar PARA LEER LOS NPCS !!!!
        '
        'El que ose desafiar esta LEY, se las tendrá que ver
        'conmigo. Para leer los NPCS se deberá usar la
        'nueva clase clsIniReader.
        '
        'Alejo
        '
        '###################################################
        Dim NpcIndex As Short
        Dim Leer As clsIniReader
        Dim LoopC As Integer
        Dim ln As String
        Dim aux As String

        Leer = LeerNPCs

        'If requested index is invalid, abort
        If Not Leer.KeyExists("NPC" & NpcNumber) Then
            OpenNPC = MAXNPCS + 1
            Exit Function
        End If

        NpcIndex = NextOpenNPC

        If NpcIndex > MAXNPCS Then 'Limite de npcs
            OpenNPC = NpcIndex
            Exit Function
        End If

        With Npclist(NpcIndex)
            .Numero = NpcNumber
            .name = Leer.GetValue("NPC" & NpcNumber, "Name")
            .desc = Leer.GetValue("NPC" & NpcNumber, "Desc")

            .Movement = Val(Leer.GetValue("NPC" & NpcNumber, "Movement"))
            .flags.OldMovement = .Movement

            .flags.AguaValida = Val(Leer.GetValue("NPC" & NpcNumber, "AguaValida"))
            .flags.TierraInvalida = Val(Leer.GetValue("NPC" & NpcNumber, "TierraInValida"))
            .flags.Faccion = Val(Leer.GetValue("NPC" & NpcNumber, "Faccion"))
            .flags.AtacaDoble = Val(Leer.GetValue("NPC" & NpcNumber, "AtacaDoble"))

            .NPCtype = Val(Leer.GetValue("NPC" & NpcNumber, "NpcType"))

            .Char_Renamed.body = Val(Leer.GetValue("NPC" & NpcNumber, "Body"))
            .Char_Renamed.Head = Val(Leer.GetValue("NPC" & NpcNumber, "Head"))
            .Char_Renamed.heading = Val(Leer.GetValue("NPC" & NpcNumber, "Heading"))

            .Attackable = Val(Leer.GetValue("NPC" & NpcNumber, "Attackable"))
            .Comercia = Val(Leer.GetValue("NPC" & NpcNumber, "Comercia"))
            .Hostile = Val(Leer.GetValue("NPC" & NpcNumber, "Hostile"))
            .flags.OldHostil = .Hostile

            .GiveEXP = Val(Leer.GetValue("NPC" & NpcNumber, "GiveEXP"))

            .flags.ExpCount = .GiveEXP

            .Veneno = Val(Leer.GetValue("NPC" & NpcNumber, "Veneno"))

            .flags.Domable = Val(Leer.GetValue("NPC" & NpcNumber, "Domable"))

            .GiveGLD = Val(Leer.GetValue("NPC" & NpcNumber, "GiveGLD"))

            .PoderAtaque = Val(Leer.GetValue("NPC" & NpcNumber, "PoderAtaque"))
            .PoderEvasion = Val(Leer.GetValue("NPC" & NpcNumber, "PoderEvasion"))

            .InvReSpawn = Val(Leer.GetValue("NPC" & NpcNumber, "InvReSpawn"))

            With .Stats
                .MaxHp = Val(Leer.GetValue("NPC" & NpcNumber, "MaxHP"))
                .MinHp = Val(Leer.GetValue("NPC" & NpcNumber, "MinHP"))
                .MaxHIT = Val(Leer.GetValue("NPC" & NpcNumber, "MaxHIT"))
                .MinHIT = Val(Leer.GetValue("NPC" & NpcNumber, "MinHIT"))
                .def = Val(Leer.GetValue("NPC" & NpcNumber, "DEF"))
                .defM = Val(Leer.GetValue("NPC" & NpcNumber, "DEFm"))
                .Alineacion = Val(Leer.GetValue("NPC" & NpcNumber, "Alineacion"))
            End With

            .Invent.NroItems = Val(Leer.GetValue("NPC" & NpcNumber, "NROITEMS"))
            For LoopC = 1 To .Invent.NroItems
                ln = Leer.GetValue("NPC" & NpcNumber, "Obj" & LoopC)
                .Invent.Object_Renamed(LoopC).ObjIndex = Val(ReadField(1, ln, 45))
                .Invent.Object_Renamed(LoopC).Amount = Val(ReadField(2, ln, 45))
            Next LoopC

            For LoopC = 1 To MAX_NPC_DROPS
                ln = Leer.GetValue("NPC" & NpcNumber, "Drop" & LoopC)
                .Drop(LoopC).ObjIndex = Val(ReadField(1, ln, 45))
                .Drop(LoopC).Amount = Val(ReadField(2, ln, 45))
            Next LoopC


            .flags.LanzaSpells = Val(Leer.GetValue("NPC" & NpcNumber, "LanzaSpells"))
            'UPGRADE_WARNING: El límite inferior de la matriz .Spells ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            If .flags.LanzaSpells > 0 Then ReDim .Spells(.flags.LanzaSpells)
            For LoopC = 1 To .flags.LanzaSpells
                .Spells(LoopC) = Val(Leer.GetValue("NPC" & NpcNumber, "Sp" & LoopC))
            Next LoopC

            If .NPCtype = Declaraciones.eNPCType.Entrenador Then
                .NroCriaturas = Val(Leer.GetValue("NPC" & NpcNumber, "NroCriaturas"))
                'UPGRADE_WARNING: El límite inferior de la matriz .Criaturas ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                ReDim .Criaturas(.NroCriaturas)
                For LoopC = 1 To .NroCriaturas
                    .Criaturas(LoopC).NpcIndex = CShort(Leer.GetValue("NPC" & NpcNumber, "CI" & LoopC))
                    .Criaturas(LoopC).NpcName = Leer.GetValue("NPC" & NpcNumber, "CN" & LoopC)
                Next LoopC
            End If

            With .flags
                .NPCActive = True

                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Respawn. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                If Respawn Then
                    .Respawn = Val(Leer.GetValue("NPC" & NpcNumber, "ReSpawn"))
                Else
                    .Respawn = 1
                End If

                .BackUp = Val(Leer.GetValue("NPC" & NpcNumber, "BackUp"))
                .RespawnOrigPos = Val(Leer.GetValue("NPC" & NpcNumber, "OrigPos"))
                .AfectaParalisis = Val(Leer.GetValue("NPC" & NpcNumber, "AfectaParalisis"))

                .Snd1 = Val(Leer.GetValue("NPC" & NpcNumber, "Snd1"))
                .Snd2 = Val(Leer.GetValue("NPC" & NpcNumber, "Snd2"))
                .Snd3 = Val(Leer.GetValue("NPC" & NpcNumber, "Snd3"))
            End With

            '<<<<<<<<<<<<<< Expresiones >>>>>>>>>>>>>>>>
            .NroExpresiones = Val(Leer.GetValue("NPC" & NpcNumber, "NROEXP"))
            'UPGRADE_WARNING: El límite inferior de la matriz .Expresiones ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            If .NroExpresiones > 0 Then ReDim .Expresiones(.NroExpresiones)
            For LoopC = 1 To .NroExpresiones
                .Expresiones(LoopC) = Leer.GetValue("NPC" & NpcNumber, "Exp" & LoopC)
            Next LoopC
            '<<<<<<<<<<<<<< Expresiones >>>>>>>>>>>>>>>>

            'Tipo de items con los que comercia
            .TipoItems = Val(Leer.GetValue("NPC" & NpcNumber, "TipoItems"))

            .Ciudad = Val(Leer.GetValue("NPC" & NpcNumber, "Ciudad"))
        End With

        'Update contadores de NPCs
        If NpcIndex > LastNPC Then LastNPC = NpcIndex
        NumNPCs = NumNPCs + 1

        'Devuelve el nuevo Indice
        OpenNPC = NpcIndex
    End Function

    Public Sub DoFollow(ByVal NpcIndex As Short, ByVal UserName As String)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        With Npclist(NpcIndex)
            If .flags.Follow Then
                .flags.AttackedBy = vbNullString
                .flags.Follow = False
                .Movement = .flags.OldMovement
                .Hostile = .flags.OldHostil
            Else
                .flags.AttackedBy = UserName
                .flags.Follow = True
                .Movement = AI.TipoAI.NPCDEFENSA
                .Hostile = 0
            End If
        End With
    End Sub

    Public Sub FollowAmo(ByVal NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        With Npclist(NpcIndex)
            .flags.Follow = True
            .Movement = AI.TipoAI.SigueAmo
            .Hostile = 0
            .Target = 0
            .TargetNPC = 0
        End With
    End Sub

    Public Sub ValidarPermanenciaNpc(ByVal NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        'Chequea si el npc continua perteneciendo a algún usuario
        '***************************************************

        With Npclist(NpcIndex)
            If IntervaloPerdioNpc(.Owner) Then Call PerdioNpc(.Owner)
        End With
    End Sub
End Module
