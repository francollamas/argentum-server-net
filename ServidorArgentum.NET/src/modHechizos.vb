Option Strict Off
Option Explicit On
Module modHechizos
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

    Public Const HELEMENTAL_FUEGO As Short = 26
    Public Const HELEMENTAL_TIERRA As Short = 28
    Public Const SUPERANILLO As Short = 700

    Sub NpcLanzaSpellSobreUser(ByVal NpcIndex As Short, ByVal UserIndex As Short, ByVal Spell As Short)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 13/02/2009
        '13/02/2009: ZaMa - Los npcs que tiren magias, no podran hacerlo en mapas donde no se permita usarla.
        '***************************************************
        If Npclist(NpcIndex).CanAttack = 0 Then Exit Sub
        If UserList(UserIndex).flags.invisible = 1 Or UserList(UserIndex).flags.Oculto = 1 Then Exit Sub

        ' Si no se peude usar magia en el mapa, no le deja hacerlo.
        If MapInfo_Renamed(UserList(UserIndex).Pos.Map).MagiaSinEfecto > 0 Then Exit Sub

        Npclist(NpcIndex).CanAttack = 0
        Dim daño As Short

        With UserList(UserIndex)
            If Hechizos(Spell).SubeHP = 1 Then

                daño = RandomNumber(Hechizos(Spell).MinHp, Hechizos(Spell).MaxHp)
                Call _
                    SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                             PrepareMessagePlayWave(Hechizos(Spell).WAV, .Pos.X, .Pos.Y))
                Call _
                    SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                             PrepareMessageCreateFX(.Char_Renamed.CharIndex, Hechizos(Spell).FXgrh,
                                                    Hechizos(Spell).loops))

                .Stats.MinHp = .Stats.MinHp + daño
                If .Stats.MinHp > .Stats.MaxHp Then .Stats.MinHp = .Stats.MaxHp

                Call _
                    WriteConsoleMsg(UserIndex, Npclist(NpcIndex).name & " te ha quitado " & daño & " puntos de vida.",
                                    Protocol.FontTypeNames.FONTTYPE_FIGHT)
                Call WriteUpdateUserStats(UserIndex)

            ElseIf Hechizos(Spell).SubeHP = 2 Then

                If .flags.Privilegios And Declaraciones.PlayerType.User Then

                    daño = RandomNumber(Hechizos(Spell).MinHp, Hechizos(Spell).MaxHp)

                    If .Invent.CascoEqpObjIndex > 0 Then
                        daño = daño -
                               RandomNumber(ObjData_Renamed(.Invent.CascoEqpObjIndex).DefensaMagicaMin,
                                            ObjData_Renamed(.Invent.CascoEqpObjIndex).DefensaMagicaMax)
                    End If

                    If .Invent.AnilloEqpObjIndex > 0 Then
                        daño = daño -
                               RandomNumber(ObjData_Renamed(.Invent.AnilloEqpObjIndex).DefensaMagicaMin,
                                            ObjData_Renamed(.Invent.AnilloEqpObjIndex).DefensaMagicaMax)
                    End If

                    If daño < 0 Then daño = 0

                    Call _
                        SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                 PrepareMessagePlayWave(Hechizos(Spell).WAV, .Pos.X, .Pos.Y))
                    Call _
                        SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                 PrepareMessageCreateFX(.Char_Renamed.CharIndex, Hechizos(Spell).FXgrh,
                                                        Hechizos(Spell).loops))

                    .Stats.MinHp = .Stats.MinHp - daño

                    Call _
                        WriteConsoleMsg(UserIndex,
                                        Npclist(NpcIndex).name & " te ha quitado " & daño & " puntos de vida.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Call WriteUpdateUserStats(UserIndex)

                    'Muere
                    If .Stats.MinHp < 1 Then
                        .Stats.MinHp = 0
                        If Npclist(NpcIndex).NPCtype = Declaraciones.eNPCType.GuardiaReal Then
                            RestarCriminalidad((UserIndex))
                        End If
                        Call UserDie(UserIndex)
                        '[Barrin 1-12-03]
                        If Npclist(NpcIndex).MaestroUser > 0 Then
                            'Store it!
                            Call Statistics.StoreFrag(Npclist(NpcIndex).MaestroUser, UserIndex)

                            Call ContarMuerte(UserIndex, Npclist(NpcIndex).MaestroUser)
                            Call ActStats(UserIndex, Npclist(NpcIndex).MaestroUser)
                        End If
                        '[/Barrin]
                    End If

                End If

            End If

            If Hechizos(Spell).Paraliza = 1 Or Hechizos(Spell).Inmoviliza = 1 Then
                If .flags.Paralizado = 0 Then
                    Call _
                        SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                 PrepareMessagePlayWave(Hechizos(Spell).WAV, .Pos.X, .Pos.Y))
                    Call _
                        SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                 PrepareMessageCreateFX(.Char_Renamed.CharIndex, Hechizos(Spell).FXgrh,
                                                        Hechizos(Spell).loops))

                    If .Invent.AnilloEqpObjIndex = SUPERANILLO Then
                        Call _
                            WriteConsoleMsg(UserIndex, " Tu anillo rechaza los efectos del hechizo.",
                                            Protocol.FontTypeNames.FONTTYPE_FIGHT)
                        Exit Sub
                    End If

                    If Hechizos(Spell).Inmoviliza = 1 Then
                        .flags.Inmovilizado = 1
                    End If

                    .flags.Paralizado = 1
                    .Counters.Paralisis = IntervaloParalizado

                    Call WriteParalizeOK(UserIndex)
                End If
            End If

            If Hechizos(Spell).Estupidez = 1 Then ' turbacion
                If .flags.Estupidez = 0 Then
                    Call _
                        SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                 PrepareMessagePlayWave(Hechizos(Spell).WAV, .Pos.X, .Pos.Y))
                    Call _
                        SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                 PrepareMessageCreateFX(.Char_Renamed.CharIndex, Hechizos(Spell).FXgrh,
                                                        Hechizos(Spell).loops))

                    If .Invent.AnilloEqpObjIndex = SUPERANILLO Then
                        Call _
                            WriteConsoleMsg(UserIndex, " Tu anillo rechaza los efectos del hechizo.",
                                            Protocol.FontTypeNames.FONTTYPE_FIGHT)
                        Exit Sub
                    End If

                    .flags.Estupidez = 1
                    .Counters.Ceguera = IntervaloInvisible

                    Call WriteDumb(UserIndex)
                End If
            End If
        End With
    End Sub

    Sub NpcLanzaSpellSobreNpc(ByVal NpcIndex As Short, ByVal TargetNPC As Short, ByVal Spell As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'solo hechizos ofensivos!

        If Npclist(NpcIndex).CanAttack = 0 Then Exit Sub
        Npclist(NpcIndex).CanAttack = 0

        Dim daño As Short

        If Hechizos(Spell).SubeHP = 2 Then

            daño = RandomNumber(Hechizos(Spell).MinHp, Hechizos(Spell).MaxHp)
            Call _
                SendData(modSendData.SendTarget.ToNPCArea, TargetNPC,
                         PrepareMessagePlayWave(Hechizos(Spell).WAV, Npclist(TargetNPC).Pos.X, Npclist(TargetNPC).Pos.Y))
            Call _
                SendData(modSendData.SendTarget.ToNPCArea, TargetNPC,
                         PrepareMessageCreateFX(Npclist(TargetNPC).Char_Renamed.CharIndex, Hechizos(Spell).FXgrh,
                                                Hechizos(Spell).loops))

            Npclist(TargetNPC).Stats.MinHp = Npclist(TargetNPC).Stats.MinHp - daño

            'Muere
            If Npclist(TargetNPC).Stats.MinHp < 1 Then
                Npclist(TargetNPC).Stats.MinHp = 0
                If Npclist(NpcIndex).MaestroUser > 0 Then
                    Call MuereNpc(TargetNPC, Npclist(NpcIndex).MaestroUser)
                Else
                    Call MuereNpc(TargetNPC, 0)
                End If
            End If

        End If
    End Sub

    Function TieneHechizo(ByVal i As Short, ByVal UserIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        On Error GoTo Errhandler

        Dim j As Short
        For j = 1 To MAXUSERHECHIZOS
            If UserList(UserIndex).Stats.UserHechizos(j) = i Then
                TieneHechizo = True
                Exit Function
            End If
        Next

        Exit Function
        Errhandler:
    End Function

    Sub AgregarHechizo(ByVal UserIndex As Short, ByVal Slot As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim hIndex As Short
        Dim j As Short

        With UserList(UserIndex)
            hIndex = ObjData_Renamed(.Invent.Object_Renamed(Slot).ObjIndex).HechizoIndex

            If Not TieneHechizo(hIndex, UserIndex) Then
                'Buscamos un slot vacio
                For j = 1 To MAXUSERHECHIZOS
                    If .Stats.UserHechizos(j) = 0 Then Exit For
                Next j

                If .Stats.UserHechizos(j) <> 0 Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No tienes espacio para más hechizos.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                Else
                    .Stats.UserHechizos(j) = hIndex
                    Call UpdateUserHechizos(False, UserIndex, CByte(j))
                    'Quitamos del inv el item
                    Call QuitarUserInvItem(UserIndex, CByte(Slot), 1)
                End If
            Else
                Call WriteConsoleMsg(UserIndex, "Ya tienes ese hechizo.", Protocol.FontTypeNames.FONTTYPE_INFO)
            End If
        End With
    End Sub

    Sub DecirPalabrasMagicas(ByVal SpellWords As String, ByVal UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 17/11/2009
        '25/07/2009: ZaMa - Invisible admins don't say any word when casting a spell
        '17/11/2009: ZaMa - Now the user become visible when casting a spell, if it is hidden
        '***************************************************
        On Error Resume Next
        With UserList(UserIndex)
            If .flags.AdminInvisible <> 1 Then
                Call _
                    SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                             PrepareMessageChatOverHead(SpellWords, .Char_Renamed.CharIndex,
                                                        System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Cyan)))

                ' Si estaba oculto, se vuelve visible
                If .flags.Oculto = 1 Then
                    .flags.Oculto = 0
                    .Counters.TiempoOculto = 0

                    If .flags.invisible = 0 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "Has vuelto a ser visible.", Protocol.FontTypeNames.FONTTYPE_INFO)
                        Call SetInvisible(UserIndex, .Char_Renamed.CharIndex, False)
                    End If
                End If
            End If
        End With
        Exit Sub
    End Sub

    ''
    ' Check if an user can cast a certain spell
    '
    ' @param UserIndex Specifies reference to user
    ' @param HechizoIndex Specifies reference to spell
    ' @return   True if the user can cast the spell, otherwise returns false
    Function PuedeLanzar(ByVal UserIndex As Short, ByVal HechizoIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: 12/01/2010
        'Last Modification By: ZaMa
        '06/11/09 - Corregida la bonificación de maná del mimetismo en el druida con flauta mágica equipada.
        '19/11/2009: ZaMa - Validacion de mana para el Invocar Mascotas
        '12/01/2010: ZaMa - Validacion de mana para hechizos lanzados por druida.
        '***************************************************
        Dim DruidManaBonus As Single

        With UserList(UserIndex)
            If .flags.Muerto Then
                Call _
                    WriteConsoleMsg(UserIndex, "No puedes lanzar hechizos estando muerto.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO)
                Exit Function
            End If

            If Hechizos(HechizoIndex).NeedStaff > 0 Then
                If .clase = Declaraciones.eClass.Mage Then
                    If .Invent.WeaponEqpObjIndex > 0 Then
                        If ObjData_Renamed(.Invent.WeaponEqpObjIndex).StaffPower < Hechizos(HechizoIndex).NeedStaff Then
                            Call _
                                WriteConsoleMsg(UserIndex,
                                                "No posees un báculo lo suficientemente poderoso para poder lanzar el conjuro.",
                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                            Exit Function
                        End If
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, "No puedes lanzar este conjuro sin la ayuda de un báculo.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        Exit Function
                    End If
                End If
            End If

            If .Stats.UserSkills(Declaraciones.eSkill.Magia) < Hechizos(HechizoIndex).MinSkill Then
                Call _
                    WriteConsoleMsg(UserIndex, "No tienes suficientes puntos de magia para lanzar este hechizo.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO)
                Exit Function
            End If

            If .Stats.MinSta < Hechizos(HechizoIndex).StaRequerido Then
                If .Genero = Declaraciones.eGenero.Hombre Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Estás muy cansado para lanzar este hechizo.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "Estás muy cansada para lanzar este hechizo.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                End If
                Exit Function
            End If

            DruidManaBonus = 1
            If .clase = Declaraciones.eClass.Druid Then
                If .Invent.AnilloEqpObjIndex = FLAUTAELFICA Then
                    ' 50% menos de mana requerido para mimetismo
                    If Hechizos(HechizoIndex).Mimetiza = 1 Then
                        DruidManaBonus = 0.5

                        ' 30% menos de mana requerido para invocaciones
                    ElseIf Hechizos(HechizoIndex).Tipo = Declaraciones.TipoHechizo.uInvocacion Then
                        DruidManaBonus = 0.7

                        ' 10% menos de mana requerido para las demas magias, excepto apoca
                    ElseIf HechizoIndex <> APOCALIPSIS_SPELL_INDEX Then
                        DruidManaBonus = 0.9
                    End If
                End If

                ' Necesita tener la barra de mana completa para invocar una mascota
                If Hechizos(HechizoIndex).Warp = 1 Then
                    If .Stats.MinMAN <> .Stats.MaxMAN Then
                        Call _
                            WriteConsoleMsg(UserIndex, "Debes poseer toda tu maná para poder lanzar este hechizo.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        Exit Function
                        ' Si no tiene mascotas, no tiene sentido que lo use
                    ElseIf .NroMascotas = 0 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "Debes poseer alguna mascota para poder lanzar este hechizo.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        Exit Function
                    End If
                End If
            End If

            If .Stats.MinMAN < Hechizos(HechizoIndex).ManaRequerido*DruidManaBonus Then
                Call WriteConsoleMsg(UserIndex, "No tienes suficiente maná.", Protocol.FontTypeNames.FONTTYPE_INFO)
                Exit Function
            End If

        End With

        PuedeLanzar = True
    End Function

    Sub HechizoTerrenoEstado(ByVal UserIndex As Short, ByRef b As Boolean)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim PosCasteadaX As Short
        Dim PosCasteadaY As Short
        Dim PosCasteadaM As Short
        Dim H As Short
        Dim TempX As Short
        Dim TempY As Short

        With UserList(UserIndex)
            PosCasteadaX = .flags.TargetX
            PosCasteadaY = .flags.TargetY
            PosCasteadaM = .flags.TargetMap

            H = .flags.Hechizo

            If Hechizos(H).RemueveInvisibilidadParcial = 1 Then
                b = True
                For TempX = PosCasteadaX - 8 To PosCasteadaX + 8
                    For TempY = PosCasteadaY - 8 To PosCasteadaY + 8
                        If InMapBounds(PosCasteadaM, TempX, TempY) Then
                            If MapData(PosCasteadaM, TempX, TempY).UserIndex > 0 Then
                                'hay un user
                                If _
                                    UserList(MapData(PosCasteadaM, TempX, TempY).UserIndex).flags.invisible = 1 And
                                    UserList(MapData(PosCasteadaM, TempX, TempY).UserIndex).flags.AdminInvisible = 0 _
                                    Then
                                    Call _
                                        SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                                 PrepareMessageCreateFX(
                                                     UserList(MapData(PosCasteadaM, TempX, TempY).UserIndex).
                                                                           Char_Renamed.CharIndex, Hechizos(H).FXgrh,
                                                     Hechizos(H).loops))
                                End If
                            End If
                        End If
                    Next TempY
                Next TempX

                Call InfoHechizo(UserIndex)
            End If
        End With
    End Sub

    ''
    ' Le da propiedades al nuevo npc
    '
    ' @param UserIndex  Indice del usuario que invoca.
    ' @param b  Indica si se termino la operación.

    Sub HechizoInvocacion(ByVal UserIndex As Short, ByRef HechizoCasteado As Boolean)
        '***************************************************
        'Author: Uknown
        'Last modification: 18/11/2009
        'Sale del sub si no hay una posición valida.
        '18/11/2009: Optimizacion de codigo.
        '***************************************************

        On Error GoTo error_Renamed

        Dim NpcIndex, SpellIndex, NroNpcs, PetIndex As Short
        Dim TargetPos As WorldPos
        With UserList(UserIndex)
            'No permitimos se invoquen criaturas en zonas seguras
            If _
                MapInfo_Renamed(.Pos.Map).Pk = False Or
                MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = Declaraciones.eTrigger.ZONASEGURA Then
                Call _
                    WriteConsoleMsg(UserIndex, "No puedes invocar criaturas en zona segura.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If


            TargetPos.Map = .flags.TargetMap
            TargetPos.X = .flags.TargetX
            TargetPos.Y = .flags.TargetY

            SpellIndex = .flags.Hechizo

            ' Warp de mascotas
            If Hechizos(SpellIndex).Warp = 1 Then
                PetIndex = FarthestPet(UserIndex)

                ' La invoco cerca mio
                If PetIndex > 0 Then
                    Call WarpMascota(UserIndex, PetIndex)
                End If

                ' Invocacion normal
            Else
                If .NroMascotas >= MAXMASCOTAS Then Exit Sub

                For NroNpcs = 1 To Hechizos(SpellIndex).cant

                    If .NroMascotas < MAXMASCOTAS Then
                        NpcIndex = SpawnNpc(Hechizos(SpellIndex).NumNpc, TargetPos, True, False)
                        If NpcIndex > 0 Then
                            .NroMascotas = .NroMascotas + 1

                            PetIndex = FreeMascotaIndex(UserIndex)

                            .MascotasIndex(PetIndex) = NpcIndex
                            .MascotasType(PetIndex) = Npclist(NpcIndex).Numero

                            With Npclist(NpcIndex)
                                .MaestroUser = UserIndex
                                .Contadores.TiempoExistencia = IntervaloInvocacion
                                .GiveGLD = 0
                            End With

                            Call FollowAmo(NpcIndex)
                        Else
                            Exit Sub
                        End If
                    Else
                        Exit For
                    End If

                Next NroNpcs
            End If
        End With

        Call InfoHechizo(UserIndex)
        HechizoCasteado = True

        Exit Sub

        error_Renamed:
        With UserList(UserIndex)
            LogError(
                ("[" & Err.Number & "] " & Err.Description & " por el usuario " & .name & "(" & UserIndex & ") en (" &
                 .Pos.Map & ", " & .Pos.X & ", " & .Pos.Y & "). Tratando de tirar el hechizo " &
                 Hechizos(SpellIndex).Nombre & "(" & SpellIndex & ") en la posicion ( " & .flags.TargetX & ", " &
                 .flags.TargetY & ")"))
        End With
    End Sub

    Sub HandleHechizoTerreno(ByVal UserIndex As Short, ByVal SpellIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 18/11/2009
        '18/11/2009: ZaMa - Optimizacion de codigo.
        '***************************************************

        Dim HechizoCasteado As Boolean
        Dim ManaRequerida As Short

        Select Case Hechizos(SpellIndex).Tipo
            Case Declaraciones.TipoHechizo.uInvocacion
                Call HechizoInvocacion(UserIndex, HechizoCasteado)

            Case Declaraciones.TipoHechizo.uEstado
                Call HechizoTerrenoEstado(UserIndex, HechizoCasteado)
        End Select

        If HechizoCasteado Then
            With UserList(UserIndex)
                Call SubirSkill(UserIndex, Declaraciones.eSkill.Magia, True)

                ManaRequerida = Hechizos(SpellIndex).ManaRequerido

                If Hechizos(SpellIndex).Warp = 1 Then ' Invocó una mascota
                    ' Consume toda la mana
                    ManaRequerida = .Stats.MinMAN
                Else
                    ' Bonificaciones en hechizos
                    If .clase = Declaraciones.eClass.Druid Then
                        ' Solo con flauta equipada
                        If .Invent.AnilloEqpObjIndex = FLAUTAELFICA Then
                            ' 30% menos de mana para invocaciones
                            ManaRequerida = ManaRequerida*0.7
                        End If
                    End If
                End If

                ' Quito la mana requerida
                .Stats.MinMAN = .Stats.MinMAN - ManaRequerida
                If .Stats.MinMAN < 0 Then .Stats.MinMAN = 0

                ' Quito la estamina requerida
                .Stats.MinSta = .Stats.MinSta - Hechizos(SpellIndex).StaRequerido
                If .Stats.MinSta < 0 Then .Stats.MinSta = 0

                ' Update user stats
                Call WriteUpdateUserStats(UserIndex)
            End With
        End If
    End Sub

    Sub HandleHechizoUsuario(ByVal UserIndex As Short, ByVal SpellIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 12/01/2010
        '18/11/2009: ZaMa - Optimizacion de codigo.
        '12/01/2010: ZaMa - Optimizacion y agrego bonificaciones al druida.
        '***************************************************

        Dim HechizoCasteado As Boolean
        Dim ManaRequerida As Short

        Select Case Hechizos(SpellIndex).Tipo
            Case Declaraciones.TipoHechizo.uEstado
                ' Afectan estados (por ejem : Envenenamiento)
                Call HechizoEstadoUsuario(UserIndex, HechizoCasteado)

            Case Declaraciones.TipoHechizo.uPropiedades
                ' Afectan HP,MANA,STAMINA,ETC
                HechizoCasteado = HechizoPropUsuario(UserIndex)
        End Select

        If HechizoCasteado Then
            With UserList(UserIndex)
                Call SubirSkill(UserIndex, Declaraciones.eSkill.Magia, True)

                ManaRequerida = Hechizos(SpellIndex).ManaRequerido

                ' Bonificaciones para druida
                If .clase = Declaraciones.eClass.Druid Then
                    ' Solo con flauta magica
                    If .Invent.AnilloEqpObjIndex = FLAUTAELFICA Then
                        If Hechizos(SpellIndex).Mimetiza = 1 Then
                            ' 50% menos de mana para mimetismo
                            ManaRequerida = ManaRequerida*0.5

                        ElseIf SpellIndex <> APOCALIPSIS_SPELL_INDEX Then
                            ' 10% menos de mana para todo menos apoca y descarga
                            ManaRequerida = ManaRequerida*0.9
                        End If
                    End If
                End If

                ' Quito la mana requerida
                .Stats.MinMAN = .Stats.MinMAN - ManaRequerida
                If .Stats.MinMAN < 0 Then .Stats.MinMAN = 0

                ' Quito la estamina requerida
                .Stats.MinSta = .Stats.MinSta - Hechizos(SpellIndex).StaRequerido
                If .Stats.MinSta < 0 Then .Stats.MinSta = 0

                ' Update user stats
                Call WriteUpdateUserStats(UserIndex)
                Call WriteUpdateUserStats(.flags.TargetUser)
                .flags.TargetUser = 0
            End With
        End If
    End Sub

    Sub HandleHechizoNPC(ByVal UserIndex As Short, ByVal HechizoIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 12/01/2010
        '13/02/2009: ZaMa - Agregada 50% bonificacion en coste de mana a mimetismo para druidas
        '17/11/2009: ZaMa - Optimizacion de codigo.
        '12/01/2010: ZaMa - Bonificacion para druidas de 10% para todos hechizos excepto apoca y descarga.
        '12/01/2010: ZaMa - Los druidas mimetizados con npcs ahora son ignorados.
        '***************************************************
        Dim HechizoCasteado As Boolean
        Dim ManaRequerida As Integer

        With UserList(UserIndex)
            Select Case Hechizos(HechizoIndex).Tipo
                Case Declaraciones.TipoHechizo.uEstado
                    ' Afectan estados (por ejem : Envenenamiento)
                    Call HechizoEstadoNPC(.flags.TargetNPC, HechizoIndex, HechizoCasteado, UserIndex)

                Case Declaraciones.TipoHechizo.uPropiedades
                    ' Afectan HP,MANA,STAMINA,ETC
                    Call HechizoPropNPC(HechizoIndex, .flags.TargetNPC, UserIndex, HechizoCasteado)
            End Select


            If HechizoCasteado Then
                Call SubirSkill(UserIndex, Declaraciones.eSkill.Magia, True)

                ManaRequerida = Hechizos(HechizoIndex).ManaRequerido

                ' Bonificación para druidas.
                If .clase = Declaraciones.eClass.Druid Then
                    ' Se mostró como usuario, puede ser atacado por npcs
                    .flags.Ignorado = False

                    ' Solo con flauta equipada
                    If .Invent.AnilloEqpObjIndex = FLAUTAELFICA Then
                        If Hechizos(HechizoIndex).Mimetiza = 1 Then
                            ' 50% menos de mana para mimetismo
                            ManaRequerida = ManaRequerida*0.5
                            ' Será ignorado hasta que pierda el efecto del mimetismo o ataque un npc
                            .flags.Ignorado = True
                        Else
                            ' 10% menos de mana para hechizos
                            If HechizoIndex <> APOCALIPSIS_SPELL_INDEX Then
                                ManaRequerida = ManaRequerida*0.9
                            End If
                        End If
                    End If
                End If

                ' Quito la mana requerida
                .Stats.MinMAN = .Stats.MinMAN - ManaRequerida
                If .Stats.MinMAN < 0 Then .Stats.MinMAN = 0

                ' Quito la estamina requerida
                .Stats.MinSta = .Stats.MinSta - Hechizos(HechizoIndex).StaRequerido
                If .Stats.MinSta < 0 Then .Stats.MinSta = 0

                ' Update user stats
                Call WriteUpdateUserStats(UserIndex)
                .flags.TargetNPC = 0
            End If
        End With
    End Sub


    Sub LanzarHechizo(ByVal SpellIndex As Short, ByVal UserIndex As Short)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 02/16/2010
        '24/01/2007 ZaMa - Optimizacion de codigo.
        '02/16/2010: Marco - Now .flags.hechizo makes reference to global spell index instead of user's spell index
        '***************************************************
        On Error GoTo Errhandler

        With UserList(UserIndex)

            If .flags.EnConsulta Then
                Call _
                    WriteConsoleMsg(UserIndex, "No puedes lanzar hechizos si estás en consulta.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If PuedeLanzar(UserIndex, SpellIndex) Then
                Select Case Hechizos(SpellIndex).Target
                    Case Declaraciones.TargetType.uUsuarios
                        If .flags.TargetUser > 0 Then
                            If System.Math.Abs(UserList(.flags.TargetUser).Pos.Y - .Pos.Y) <= RANGO_VISION_Y Then
                                Call HandleHechizoUsuario(UserIndex, SpellIndex)
                            Else
                                Call _
                                    WriteConsoleMsg(UserIndex, "Estás demasiado lejos para lanzar este hechizo.",
                                                    Protocol.FontTypeNames.FONTTYPE_WARNING)
                            End If
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex, "Este hechizo actúa sólo sobre usuarios.",
                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                        End If

                    Case Declaraciones.TargetType.uNPC
                        If .flags.TargetNPC > 0 Then
                            If System.Math.Abs(Npclist(.flags.TargetNPC).Pos.Y - .Pos.Y) <= RANGO_VISION_Y Then
                                Call HandleHechizoNPC(UserIndex, SpellIndex)
                            Else
                                Call _
                                    WriteConsoleMsg(UserIndex, "Estás demasiado lejos para lanzar este hechizo.",
                                                    Protocol.FontTypeNames.FONTTYPE_WARNING)
                            End If
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex, "Este hechizo sólo afecta a los npcs.",
                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                        End If

                    Case Declaraciones.TargetType.uUsuariosYnpc
                        If .flags.TargetUser > 0 Then
                            If System.Math.Abs(UserList(.flags.TargetUser).Pos.Y - .Pos.Y) <= RANGO_VISION_Y Then
                                Call HandleHechizoUsuario(UserIndex, SpellIndex)
                            Else
                                Call _
                                    WriteConsoleMsg(UserIndex, "Estás demasiado lejos para lanzar este hechizo.",
                                                    Protocol.FontTypeNames.FONTTYPE_WARNING)
                            End If
                        ElseIf .flags.TargetNPC > 0 Then
                            If System.Math.Abs(Npclist(.flags.TargetNPC).Pos.Y - .Pos.Y) <= RANGO_VISION_Y Then
                                Call HandleHechizoNPC(UserIndex, SpellIndex)
                            Else
                                Call _
                                    WriteConsoleMsg(UserIndex, "Estás demasiado lejos para lanzar este hechizo.",
                                                    Protocol.FontTypeNames.FONTTYPE_WARNING)
                            End If
                        Else
                            Call WriteConsoleMsg(UserIndex, "Target inválido.", Protocol.FontTypeNames.FONTTYPE_INFO)
                        End If

                    Case Declaraciones.TargetType.uTerreno
                        Call HandleHechizoTerreno(UserIndex, SpellIndex)
                End Select

            End If

            If .Counters.Trabajando Then .Counters.Trabajando = .Counters.Trabajando - 1

            If .Counters.Ocultando Then .Counters.Ocultando = .Counters.Ocultando - 1

        End With

        Exit Sub

        Errhandler:
        Call _
            LogError(
                "Error en LanzarHechizo. Error " & Err.Number & " : " & Err.Description & " Hechizo: " &
                Hechizos(SpellIndex).Nombre & "(" & SpellIndex & "). Casteado por: " & UserList(UserIndex).name & "(" &
                UserIndex & ").")
    End Sub

    Sub HechizoEstadoUsuario(ByVal UserIndex As Short, ByRef HechizoCasteado As Boolean)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 28/04/2010
        'Handles the Spells that afect the Stats of an User
        '24/01/2007 Pablo (ToxicWaste) - Invisibilidad no permitida en Mapas con InviSinEfecto
        '26/01/2007 Pablo (ToxicWaste) - Cambios que permiten mejor manejo de ataques en los rings.
        '26/01/2007 Pablo (ToxicWaste) - Revivir no permitido en Mapas con ResuSinEfecto
        '02/01/2008 Marcos (ByVal) - Curar Veneno no permitido en usuarios muertos.
        '06/28/2008 NicoNZ - Agregué que se le de valor al flag Inmovilizado.
        '17/11/2008: NicoNZ - Agregado para quitar la penalización de vida en el ring y cambio de ecuacion.
        '13/02/2009: ZaMa - Arreglada ecuacion para quitar vida tras resucitar en rings.
        '23/11/2009: ZaMa - Optimizacion de codigo.
        '28/04/2010: ZaMa - Agrego Restricciones para ciudas respecto al estado atacable.
        '***************************************************


        Dim HechizoIndex As Short
        Dim TargetIndex As Short

        Dim EraCriminal As Boolean
        With UserList(UserIndex)
            HechizoIndex = .flags.Hechizo
            TargetIndex = .flags.TargetUser

            ' <-------- Agrega Invisibilidad ---------->
            If Hechizos(HechizoIndex).Invisibilidad = 1 Then
                If UserList(TargetIndex).flags.Muerto = 1 Then
                    Call WriteConsoleMsg(UserIndex, "¡El usuario está muerto!", Protocol.FontTypeNames.FONTTYPE_INFO)
                    HechizoCasteado = False
                    Exit Sub
                End If

                If UserList(TargetIndex).Counters.Saliendo Then
                    If UserIndex <> TargetIndex Then
                        Call _
                            WriteConsoleMsg(UserIndex, "¡El hechizo no tiene efecto!",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        HechizoCasteado = False
                        Exit Sub
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, "¡No puedes hacerte invisible mientras te encuentras saliendo!",
                                            Protocol.FontTypeNames.FONTTYPE_WARNING)
                        HechizoCasteado = False
                        Exit Sub
                    End If
                End If

                'No usar invi mapas InviSinEfecto
                If MapInfo_Renamed(UserList(TargetIndex).Pos.Map).InviSinEfecto > 0 Then
                    Call _
                        WriteConsoleMsg(UserIndex, "¡La invisibilidad no funciona aquí!",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                    HechizoCasteado = False
                    Exit Sub
                End If

                ' Chequea si el status permite ayudar al otro usuario
                HechizoCasteado = CanSupportUser(UserIndex, TargetIndex, True)
                If Not HechizoCasteado Then Exit Sub

                'Si sos user, no uses este hechizo con GMS.
                If .flags.Privilegios And Declaraciones.PlayerType.User Then
                    If Not UserList(TargetIndex).flags.Privilegios And Declaraciones.PlayerType.User Then
                        HechizoCasteado = False
                        Exit Sub
                    End If
                End If

                UserList(TargetIndex).flags.invisible = 1
                Call SetInvisible(TargetIndex, UserList(TargetIndex).Char_Renamed.CharIndex, True)

                Call InfoHechizo(UserIndex)
                HechizoCasteado = True
            End If

            ' <-------- Agrega Mimetismo ---------->
            If Hechizos(HechizoIndex).Mimetiza = 1 Then
                If UserList(TargetIndex).flags.Muerto = 1 Then
                    Exit Sub
                End If

                If UserList(TargetIndex).flags.Navegando = 1 Then
                    Exit Sub
                End If
                If .flags.Navegando = 1 Then
                    Exit Sub
                End If

                'Si sos user, no uses este hechizo con GMS.
                If .flags.Privilegios And Declaraciones.PlayerType.User Then
                    If Not UserList(TargetIndex).flags.Privilegios And Declaraciones.PlayerType.User Then
                        Exit Sub
                    End If
                End If

                If .flags.Mimetizado = 1 Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Ya te encuentras mimetizado. El hechizo no ha tenido efecto.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If

                If .flags.AdminInvisible = 1 Then Exit Sub

                'copio el char original al mimetizado

                .CharMimetizado.body = .Char_Renamed.body
                .CharMimetizado.Head = .Char_Renamed.Head
                .CharMimetizado.CascoAnim = .Char_Renamed.CascoAnim
                .CharMimetizado.ShieldAnim = .Char_Renamed.ShieldAnim
                .CharMimetizado.WeaponAnim = .Char_Renamed.WeaponAnim

                .flags.Mimetizado = 1

                'ahora pongo local el del enemigo
                .Char_Renamed.body = UserList(TargetIndex).Char_Renamed.body
                .Char_Renamed.Head = UserList(TargetIndex).Char_Renamed.Head
                .Char_Renamed.CascoAnim = UserList(TargetIndex).Char_Renamed.CascoAnim
                .Char_Renamed.ShieldAnim = UserList(TargetIndex).Char_Renamed.ShieldAnim
                .Char_Renamed.WeaponAnim = GetWeaponAnim(UserIndex, UserList(TargetIndex).Invent.WeaponEqpObjIndex)

                Call _
                    ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading,
                                   .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim, .Char_Renamed.CascoAnim)

                Call InfoHechizo(UserIndex)
                HechizoCasteado = True
            End If

            ' <-------- Agrega Envenenamiento ---------->
            If Hechizos(HechizoIndex).Envenena = 1 Then
                If UserIndex = TargetIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes atacarte a vos mismo.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Exit Sub
                End If

                If Not PuedeAtacar(UserIndex, TargetIndex) Then Exit Sub
                If UserIndex <> TargetIndex Then
                    Call UsuarioAtacadoPorUsuario(UserIndex, TargetIndex)
                End If
                UserList(TargetIndex).flags.Envenenado = 1
                Call InfoHechizo(UserIndex)
                HechizoCasteado = True
            End If

            ' <-------- Cura Envenenamiento ---------->
            If Hechizos(HechizoIndex).CuraVeneno = 1 Then

                'Verificamos que el usuario no este muerto
                If UserList(TargetIndex).flags.Muerto = 1 Then
                    Call WriteConsoleMsg(UserIndex, "¡El usuario está muerto!", Protocol.FontTypeNames.FONTTYPE_INFO)
                    HechizoCasteado = False
                    Exit Sub
                End If

                ' Chequea si el status permite ayudar al otro usuario
                HechizoCasteado = CanSupportUser(UserIndex, TargetIndex)
                If Not HechizoCasteado Then Exit Sub

                'Si sos user, no uses este hechizo con GMS.
                If .flags.Privilegios And Declaraciones.PlayerType.User Then
                    If Not UserList(TargetIndex).flags.Privilegios And Declaraciones.PlayerType.User Then
                        Exit Sub
                    End If
                End If

                UserList(TargetIndex).flags.Envenenado = 0
                Call InfoHechizo(UserIndex)
                HechizoCasteado = True
            End If

            ' <-------- Agrega Maldicion ---------->
            If Hechizos(HechizoIndex).Maldicion = 1 Then
                If UserIndex = TargetIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes atacarte a vos mismo.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Exit Sub
                End If

                If Not PuedeAtacar(UserIndex, TargetIndex) Then Exit Sub
                If UserIndex <> TargetIndex Then
                    Call UsuarioAtacadoPorUsuario(UserIndex, TargetIndex)
                End If
                UserList(TargetIndex).flags.Maldicion = 1
                Call InfoHechizo(UserIndex)
                HechizoCasteado = True
            End If

            ' <-------- Remueve Maldicion ---------->
            If Hechizos(HechizoIndex).RemoverMaldicion = 1 Then
                UserList(TargetIndex).flags.Maldicion = 0
                Call InfoHechizo(UserIndex)
                HechizoCasteado = True
            End If

            ' <-------- Agrega Bendicion ---------->
            If Hechizos(HechizoIndex).Bendicion = 1 Then
                UserList(TargetIndex).flags.Bendicion = 1
                Call InfoHechizo(UserIndex)
                HechizoCasteado = True
            End If

            ' <-------- Agrega Paralisis/Inmobilidad ---------->
            If Hechizos(HechizoIndex).Paraliza = 1 Or Hechizos(HechizoIndex).Inmoviliza = 1 Then
                If UserIndex = TargetIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes atacarte a vos mismo.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Exit Sub
                End If

                If UserList(TargetIndex).flags.Paralizado = 0 Then
                    If Not PuedeAtacar(UserIndex, TargetIndex) Then Exit Sub

                    If UserIndex <> TargetIndex Then
                        Call UsuarioAtacadoPorUsuario(UserIndex, TargetIndex)
                    End If

                    Call InfoHechizo(UserIndex)
                    HechizoCasteado = True
                    If UserList(TargetIndex).Invent.AnilloEqpObjIndex = SUPERANILLO Then
                        Call _
                            WriteConsoleMsg(TargetIndex, " Tu anillo rechaza los efectos del hechizo.",
                                            Protocol.FontTypeNames.FONTTYPE_FIGHT)
                        Call _
                            WriteConsoleMsg(UserIndex, " ¡El hechizo no tiene efecto!",
                                            Protocol.FontTypeNames.FONTTYPE_FIGHT)
                        Call FlushBuffer(TargetIndex)
                        Exit Sub
                    End If

                    If Hechizos(HechizoIndex).Inmoviliza = 1 Then UserList(TargetIndex).flags.Inmovilizado = 1
                    UserList(TargetIndex).flags.Paralizado = 1
                    UserList(TargetIndex).Counters.Paralisis = IntervaloParalizado

                    Call WriteParalizeOK(TargetIndex)
                    Call FlushBuffer(TargetIndex)
                End If
            End If

            ' <-------- Remueve Paralisis/Inmobilidad ---------->
            If Hechizos(HechizoIndex).RemoverParalisis = 1 Then

                ' Remueve si esta en ese estado
                If UserList(TargetIndex).flags.Paralizado = 1 Then

                    ' Chequea si el status permite ayudar al otro usuario
                    HechizoCasteado = CanSupportUser(UserIndex, TargetIndex, True)
                    If Not HechizoCasteado Then Exit Sub

                    UserList(TargetIndex).flags.Inmovilizado = 0
                    UserList(TargetIndex).flags.Paralizado = 0

                    'no need to crypt this
                    Call WriteParalizeOK(TargetIndex)
                    Call InfoHechizo(UserIndex)

                End If
            End If

            ' <-------- Remueve Estupidez (Aturdimiento) ---------->
            If Hechizos(HechizoIndex).RemoverEstupidez = 1 Then

                ' Remueve si esta en ese estado
                If UserList(TargetIndex).flags.Estupidez = 1 Then

                    ' Chequea si el status permite ayudar al otro usuario
                    HechizoCasteado = CanSupportUser(UserIndex, TargetIndex)
                    If Not HechizoCasteado Then Exit Sub

                    UserList(TargetIndex).flags.Estupidez = 0

                    'no need to crypt this
                    Call WriteDumbNoMore(TargetIndex)
                    Call FlushBuffer(TargetIndex)
                    Call InfoHechizo(UserIndex)

                End If
            End If

            ' <-------- Revive ---------->
            If Hechizos(HechizoIndex).Revivir = 1 Then
                If UserList(TargetIndex).flags.Muerto = 1 Then

                    'Seguro de resurreccion (solo afecta a los hechizos, no al sacerdote ni al comando de GM)
                    If UserList(TargetIndex).flags.SeguroResu Then
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "¡El espíritu no tiene intenciones de regresar al mundo de los vivos!",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        HechizoCasteado = False
                        Exit Sub
                    End If

                    'No usar resu en mapas con ResuSinEfecto
                    If MapInfo_Renamed(UserList(TargetIndex).Pos.Map).ResuSinEfecto > 0 Then
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "¡Revivir no está permitido aquí! Retirate de la Zona si deseas utilizar el Hechizo.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        HechizoCasteado = False
                        Exit Sub
                    End If

                    'No podemos resucitar si nuestra barra de energía no está llena. (GD: 29/04/07)
                    If .Stats.MaxSta <> .Stats.MinSta Then
                        Call _
                            WriteConsoleMsg(UserIndex, "No puedes resucitar si no tienes tu barra de energía llena.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        HechizoCasteado = False
                        Exit Sub
                    End If


                    'revisamos si necesita vara
                    If .clase = Declaraciones.eClass.Mage Then
                        If .Invent.WeaponEqpObjIndex > 0 Then
                            If ObjData_Renamed(.Invent.WeaponEqpObjIndex).StaffPower < Hechizos(HechizoIndex).NeedStaff _
                                Then
                                Call _
                                    WriteConsoleMsg(UserIndex, "Necesitas un báculo mejor para lanzar este hechizo.",
                                                    Protocol.FontTypeNames.FONTTYPE_INFO)
                                HechizoCasteado = False
                                Exit Sub
                            End If
                        End If
                    ElseIf .clase = Declaraciones.eClass.Bard Then
                        If .Invent.AnilloEqpObjIndex <> LAUDELFICO And .Invent.AnilloEqpObjIndex <> LAUDMAGICO Then
                            Call _
                                WriteConsoleMsg(UserIndex, "Necesitas un instrumento mágico para devolver la vida.",
                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                            HechizoCasteado = False
                            Exit Sub
                        End If
                    ElseIf .clase = Declaraciones.eClass.Druid Then
                        If .Invent.AnilloEqpObjIndex <> FLAUTAELFICA And .Invent.AnilloEqpObjIndex <> FLAUTAMAGICA Then
                            Call _
                                WriteConsoleMsg(UserIndex, "Necesitas un instrumento mágico para devolver la vida.",
                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                            HechizoCasteado = False
                            Exit Sub
                        End If
                    End If

                    ' Chequea si el status permite ayudar al otro usuario
                    HechizoCasteado = CanSupportUser(UserIndex, TargetIndex, True)
                    If Not HechizoCasteado Then Exit Sub

                    EraCriminal = criminal(UserIndex)

                    If Not criminal(TargetIndex) Then
                        If TargetIndex <> UserIndex Then
                            .Reputacion.NobleRep = .Reputacion.NobleRep + 500
                            If .Reputacion.NobleRep > MAXREP Then .Reputacion.NobleRep = MAXREP
                            Call _
                                WriteConsoleMsg(UserIndex, "¡Los Dioses te sonríen, has ganado 500 puntos de nobleza!",
                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If

                    If EraCriminal And Not criminal(UserIndex) Then
                        Call RefreshCharStatus(UserIndex)
                    End If

                    With UserList(TargetIndex)
                        'Pablo Toxic Waste (GD: 29/04/07)
                        .Stats.MinAGU = 0
                        .flags.Sed = 1
                        .Stats.MinHam = 0
                        .flags.Hambre = 1
                        Call WriteUpdateHungerAndThirst(TargetIndex)
                        Call InfoHechizo(UserIndex)
                        .Stats.MinMAN = 0
                        .Stats.MinSta = 0
                    End With

                    'Agregado para quitar la penalización de vida en el ring y cambio de ecuacion. (NicoNZ)
                    If (TriggerZonaPelea(UserIndex, TargetIndex) <> Declaraciones.eTrigger6.TRIGGER6_PERMITE) Then
                        'Solo saco vida si es User. no quiero que exploten GMs por ahi.
                        If .flags.Privilegios And Declaraciones.PlayerType.User Then
                            .Stats.MinHp = .Stats.MinHp*(1 - UserList(TargetIndex).Stats.ELV*0.015)
                        End If
                    End If

                    If (.Stats.MinHp <= 0) Then
                        Call UserDie(UserIndex)
                        Call _
                            WriteConsoleMsg(UserIndex, "El esfuerzo de resucitar fue demasiado grande.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        HechizoCasteado = False
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, "El esfuerzo de resucitar te ha debilitado.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        HechizoCasteado = True
                    End If

                    If UserList(TargetIndex).flags.Traveling = 1 Then
                        UserList(TargetIndex).Counters.goHome = 0
                        UserList(TargetIndex).flags.Traveling = 0
                        'Call WriteConsoleMsg(TargetIndex, "Tu viaje ha sido cancelado.", FontTypeNames.FONTTYPE_FIGHT)
                        Call WriteMultiMessage(TargetIndex, Declaraciones.eMessages.CancelHome)
                    End If

                    Call RevivirUsuario(TargetIndex)
                Else
                    HechizoCasteado = False
                End If

            End If

            ' <-------- Agrega Ceguera ---------->
            If Hechizos(HechizoIndex).Ceguera = 1 Then
                If UserIndex = TargetIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes atacarte a vos mismo.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Exit Sub
                End If

                If Not PuedeAtacar(UserIndex, TargetIndex) Then Exit Sub
                If UserIndex <> TargetIndex Then
                    Call UsuarioAtacadoPorUsuario(UserIndex, TargetIndex)
                End If
                UserList(TargetIndex).flags.Ceguera = 1
                UserList(TargetIndex).Counters.Ceguera = IntervaloParalizado/3

                Call WriteBlind(TargetIndex)
                Call FlushBuffer(TargetIndex)
                Call InfoHechizo(UserIndex)
                HechizoCasteado = True
            End If

            ' <-------- Agrega Estupidez (Aturdimiento) ---------->
            If Hechizos(HechizoIndex).Estupidez = 1 Then
                If UserIndex = TargetIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes atacarte a vos mismo.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Exit Sub
                End If
                If Not PuedeAtacar(UserIndex, TargetIndex) Then Exit Sub
                If UserIndex <> TargetIndex Then
                    Call UsuarioAtacadoPorUsuario(UserIndex, TargetIndex)
                End If
                If UserList(TargetIndex).flags.Estupidez = 0 Then
                    UserList(TargetIndex).flags.Estupidez = 1
                    UserList(TargetIndex).Counters.Ceguera = IntervaloParalizado
                End If
                Call WriteDumb(TargetIndex)
                Call FlushBuffer(TargetIndex)

                Call InfoHechizo(UserIndex)
                HechizoCasteado = True
            End If
        End With
    End Sub

    Sub HechizoEstadoNPC(ByVal NpcIndex As Short, ByVal SpellIndex As Short, ByRef HechizoCasteado As Boolean,
                         ByVal UserIndex As Short)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 07/07/2008
        'Handles the Spells that afect the Stats of an NPC
        '04/13/2008 NicoNZ - Guardias Faccionarios pueden ser
        'removidos por users de su misma faccion.
        '07/07/2008: NicoNZ - Solo se puede mimetizar con npcs si es druida
        '***************************************************

        With Npclist(NpcIndex)
            If Hechizos(SpellIndex).Invisibilidad = 1 Then
                Call InfoHechizo(UserIndex)
                .flags.invisible = 1
                HechizoCasteado = True
            End If

            If Hechizos(SpellIndex).Envenena = 1 Then
                If Not PuedeAtacarNPC(UserIndex, NpcIndex) Then
                    HechizoCasteado = False
                    Exit Sub
                End If
                Call NPCAtacado(NpcIndex, UserIndex)
                Call InfoHechizo(UserIndex)
                .flags.Envenenado = 1
                HechizoCasteado = True
            End If

            If Hechizos(SpellIndex).CuraVeneno = 1 Then
                Call InfoHechizo(UserIndex)
                .flags.Envenenado = 0
                HechizoCasteado = True
            End If

            If Hechizos(SpellIndex).Maldicion = 1 Then
                If Not PuedeAtacarNPC(UserIndex, NpcIndex) Then
                    HechizoCasteado = False
                    Exit Sub
                End If
                Call NPCAtacado(NpcIndex, UserIndex)
                Call InfoHechizo(UserIndex)
                .flags.Maldicion = 1
                HechizoCasteado = True
            End If

            If Hechizos(SpellIndex).RemoverMaldicion = 1 Then
                Call InfoHechizo(UserIndex)
                .flags.Maldicion = 0
                HechizoCasteado = True
            End If

            If Hechizos(SpellIndex).Bendicion = 1 Then
                Call InfoHechizo(UserIndex)
                .flags.Bendicion = 1
                HechizoCasteado = True
            End If

            If Hechizos(SpellIndex).Paraliza = 1 Then
                If .flags.AfectaParalisis = 0 Then
                    If Not PuedeAtacarNPC(UserIndex, NpcIndex, True) Then
                        HechizoCasteado = False
                        Exit Sub
                    End If
                    Call NPCAtacado(NpcIndex, UserIndex)
                    Call InfoHechizo(UserIndex)
                    .flags.Paralizado = 1
                    .flags.Inmovilizado = 0
                    .Contadores.Paralisis = IntervaloParalizado
                    HechizoCasteado = True
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "El NPC es inmune a este hechizo.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                    HechizoCasteado = False
                    Exit Sub
                End If
            End If

            If Hechizos(SpellIndex).RemoverParalisis = 1 Then
                If .flags.Paralizado = 1 Or .flags.Inmovilizado = 1 Then
                    If .MaestroUser = UserIndex Then
                        Call InfoHechizo(UserIndex)
                        .flags.Paralizado = 0
                        .Contadores.Paralisis = 0
                        HechizoCasteado = True
                    Else
                        If .NPCtype = Declaraciones.eNPCType.GuardiaReal Then
                            If esArmada(UserIndex) Then
                                Call InfoHechizo(UserIndex)
                                .flags.Paralizado = 0
                                .Contadores.Paralisis = 0
                                HechizoCasteado = True
                                Exit Sub
                            Else
                                Call _
                                    WriteConsoleMsg(UserIndex,
                                                    "Sólo puedes remover la parálisis de los Guardias si perteneces a su facción.",
                                                    Protocol.FontTypeNames.FONTTYPE_INFO)
                                HechizoCasteado = False
                                Exit Sub
                            End If

                            Call _
                                WriteConsoleMsg(UserIndex,
                                                "Solo puedes remover la parálisis de los NPCs que te consideren su amo.",
                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                            HechizoCasteado = False
                            Exit Sub
                        Else
                            If .NPCtype = Declaraciones.eNPCType.Guardiascaos Then
                                If esCaos(UserIndex) Then
                                    Call InfoHechizo(UserIndex)
                                    .flags.Paralizado = 0
                                    .Contadores.Paralisis = 0
                                    HechizoCasteado = True
                                    Exit Sub
                                Else
                                    Call _
                                        WriteConsoleMsg(UserIndex,
                                                        "Solo puedes remover la parálisis de los Guardias si perteneces a su facción.",
                                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                                    HechizoCasteado = False
                                    Exit Sub
                                End If
                            End If
                        End If
                    End If
                Else
                    Call WriteConsoleMsg(UserIndex, "Este NPC no está paralizado", Protocol.FontTypeNames.FONTTYPE_INFO)
                    HechizoCasteado = False
                    Exit Sub
                End If
            End If

            If Hechizos(SpellIndex).Inmoviliza = 1 Then
                If .flags.AfectaParalisis = 0 Then
                    If Not PuedeAtacarNPC(UserIndex, NpcIndex, True) Then
                        HechizoCasteado = False
                        Exit Sub
                    End If
                    Call NPCAtacado(NpcIndex, UserIndex)
                    .flags.Inmovilizado = 1
                    .flags.Paralizado = 0
                    .Contadores.Paralisis = IntervaloParalizado
                    Call InfoHechizo(UserIndex)
                    HechizoCasteado = True
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "El NPC es inmune al hechizo.", Protocol.FontTypeNames.FONTTYPE_INFO)
                End If
            End If
        End With

        If Hechizos(SpellIndex).Mimetiza = 1 Then
            With UserList(UserIndex)
                If .flags.Mimetizado = 1 Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Ya te encuentras mimetizado. El hechizo no ha tenido efecto.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If

                If .flags.AdminInvisible = 1 Then Exit Sub


                If .clase = Declaraciones.eClass.Druid Then
                    'copio el char original al mimetizado

                    .CharMimetizado.body = .Char_Renamed.body
                    .CharMimetizado.Head = .Char_Renamed.Head
                    .CharMimetizado.CascoAnim = .Char_Renamed.CascoAnim
                    .CharMimetizado.ShieldAnim = .Char_Renamed.ShieldAnim
                    .CharMimetizado.WeaponAnim = .Char_Renamed.WeaponAnim

                    .flags.Mimetizado = 1

                    'ahora pongo lo del NPC.
                    .Char_Renamed.body = Npclist(NpcIndex).Char_Renamed.body
                    .Char_Renamed.Head = Npclist(NpcIndex).Char_Renamed.Head
                    .Char_Renamed.CascoAnim = NingunCasco
                    .Char_Renamed.ShieldAnim = NingunEscudo
                    .Char_Renamed.WeaponAnim = NingunArma

                    Call _
                        ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading,
                                       .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim, .Char_Renamed.CascoAnim)

                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "Sólo los druidas pueden mimetizarse con criaturas.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If

                Call InfoHechizo(UserIndex)
                HechizoCasteado = True
            End With
        End If
    End Sub

    Sub HechizoPropNPC(ByVal SpellIndex As Short, ByVal NpcIndex As Short, ByVal UserIndex As Short,
                       ByRef HechizoCasteado As Boolean)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 14/08/2007
        'Handles the Spells that afect the Life NPC
        '14/08/2007 Pablo (ToxicWaste) - Orden general.
        '***************************************************

        Dim daño As Integer

        With Npclist(NpcIndex)
            'Salud
            If Hechizos(SpellIndex).SubeHP = 1 Then
                daño = RandomNumber(Hechizos(SpellIndex).MinHp, Hechizos(SpellIndex).MaxHp)
                daño = daño + Porcentaje(daño, 3*UserList(UserIndex).Stats.ELV)

                Call InfoHechizo(UserIndex)
                .Stats.MinHp = .Stats.MinHp + daño
                If .Stats.MinHp > .Stats.MaxHp Then .Stats.MinHp = .Stats.MaxHp
                Call _
                    WriteConsoleMsg(UserIndex, "Has curado " & daño & " puntos de vida a la criatura.",
                                    Protocol.FontTypeNames.FONTTYPE_FIGHT)
                HechizoCasteado = True

            ElseIf Hechizos(SpellIndex).SubeHP = 2 Then
                If Not PuedeAtacarNPC(UserIndex, NpcIndex) Then
                    HechizoCasteado = False
                    Exit Sub
                End If
                Call NPCAtacado(NpcIndex, UserIndex)
                daño = RandomNumber(Hechizos(SpellIndex).MinHp, Hechizos(SpellIndex).MaxHp)
                daño = daño + Porcentaje(daño, 3*UserList(UserIndex).Stats.ELV)

                If Hechizos(SpellIndex).StaffAffected Then
                    If UserList(UserIndex).clase = Declaraciones.eClass.Mage Then
                        If UserList(UserIndex).Invent.WeaponEqpObjIndex > 0 Then
                            daño =
                                (daño*
                                 (ObjData_Renamed(UserList(UserIndex).Invent.WeaponEqpObjIndex).StaffDamageBonus + 70))/
                                100
                            'Aumenta daño segun el staff-
                            'Daño = (Daño* (70 + BonifBáculo)) / 100
                        Else
                            daño = daño*0.7 'Baja daño a 70% del original
                        End If
                    End If
                End If
                If _
                    UserList(UserIndex).Invent.AnilloEqpObjIndex = LAUDELFICO Or
                    UserList(UserIndex).Invent.AnilloEqpObjIndex = FLAUTAELFICA Then
                    daño = daño*1.04 'laud magico de los bardos
                End If

                Call InfoHechizo(UserIndex)
                HechizoCasteado = True

                If .flags.Snd2 > 0 Then
                    Call _
                        SendData(modSendData.SendTarget.ToNPCArea, NpcIndex,
                                 PrepareMessagePlayWave(.flags.Snd2, .Pos.X, .Pos.Y))
                End If

                'Quizas tenga defenza magica el NPC. Pablo (ToxicWaste)
                daño = daño - .Stats.defM
                If daño < 0 Then daño = 0

                .Stats.MinHp = .Stats.MinHp - daño
                Call _
                    WriteConsoleMsg(UserIndex, "¡Le has quitado " & daño & " puntos de vida a la criatura!",
                                    Protocol.FontTypeNames.FONTTYPE_FIGHT)
                Call CalcularDarExp(UserIndex, NpcIndex, daño)

                If .Stats.MinHp < 1 Then
                    .Stats.MinHp = 0
                    Call MuereNpc(NpcIndex, UserIndex)
                End If
            End If
        End With
    End Sub

    Sub InfoHechizo(ByVal UserIndex As Short)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 25/07/2009
        '25/07/2009: ZaMa - Code improvements.
        '25/07/2009: ZaMa - Now invisible admins magic sounds are not sent to anyone but themselves
        '***************************************************
        Dim SpellIndex As Short
        Dim tUser As Short
        Dim tNPC As Short

        With UserList(UserIndex)
            SpellIndex = .flags.Hechizo
            tUser = .flags.TargetUser
            tNPC = .flags.TargetNPC

            Call DecirPalabrasMagicas(Hechizos(SpellIndex).PalabrasMagicas, UserIndex)

            If tUser > 0 Then
                ' Los admins invisibles no producen sonidos ni fx's
                If .flags.AdminInvisible = 1 And UserIndex = tUser Then
                    Call _
                        EnviarDatosASlot(UserIndex,
                                         PrepareMessageCreateFX(UserList(tUser).Char_Renamed.CharIndex,
                                                                Hechizos(SpellIndex).FXgrh, Hechizos(SpellIndex).loops))
                    Call _
                        EnviarDatosASlot(UserIndex,
                                         PrepareMessagePlayWave(Hechizos(SpellIndex).WAV, UserList(tUser).Pos.X,
                                                                UserList(tUser).Pos.Y))
                Else
                    Call _
                        SendData(modSendData.SendTarget.ToPCArea, tUser,
                                 PrepareMessageCreateFX(UserList(tUser).Char_Renamed.CharIndex,
                                                        Hechizos(SpellIndex).FXgrh, Hechizos(SpellIndex).loops))
                    Call _
                        SendData(modSendData.SendTarget.ToPCArea, tUser,
                                 PrepareMessagePlayWave(Hechizos(SpellIndex).WAV, UserList(tUser).Pos.X,
                                                        UserList(tUser).Pos.Y)) 'Esta linea faltaba. Pablo (ToxicWaste)
                End If
            ElseIf tNPC > 0 Then
                Call _
                    SendData(modSendData.SendTarget.ToNPCArea, tNPC,
                             PrepareMessageCreateFX(Npclist(tNPC).Char_Renamed.CharIndex, Hechizos(SpellIndex).FXgrh,
                                                    Hechizos(SpellIndex).loops))
                Call _
                    SendData(modSendData.SendTarget.ToNPCArea, tNPC,
                             PrepareMessagePlayWave(Hechizos(SpellIndex).WAV, Npclist(tNPC).Pos.X, Npclist(tNPC).Pos.Y))
            End If

            If tUser > 0 Then
                If UserIndex <> tUser Then
                    If .showName Then
                        Call _
                            WriteConsoleMsg(UserIndex, Hechizos(SpellIndex).HechizeroMsg & " " & UserList(tUser).name,
                                            Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, Hechizos(SpellIndex).HechizeroMsg & " alguien.",
                                            Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    End If
                    Call _
                        WriteConsoleMsg(tUser, .name & " " & Hechizos(SpellIndex).TargetMsg,
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, Hechizos(SpellIndex).PropioMsg, Protocol.FontTypeNames.FONTTYPE_FIGHT)
                End If
            ElseIf tNPC > 0 Then
                Call _
                    WriteConsoleMsg(UserIndex, Hechizos(SpellIndex).HechizeroMsg & " " & "la criatura.",
                                    Protocol.FontTypeNames.FONTTYPE_FIGHT)
            End If
        End With
    End Sub

    Public Function HechizoPropUsuario(ByVal UserIndex As Short) As Boolean
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 28/04/2010
        '02/01/2008 Marcos (ByVal) - No permite tirar curar heridas a usuarios muertos.
        '28/04/2010: ZaMa - Agrego Restricciones para ciudas respecto al estado atacable.
        '***************************************************

        Dim SpellIndex As Short
        Dim daño As Integer
        Dim TargetIndex As Short

        SpellIndex = UserList(UserIndex).flags.Hechizo
        TargetIndex = UserList(UserIndex).flags.TargetUser

        With UserList(TargetIndex)
            If .flags.Muerto Then
                Call _
                    WriteConsoleMsg(UserIndex, "No puedes lanzar este hechizo a un muerto.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO)
                Exit Function
            End If

            ' <-------- Aumenta Hambre ---------->
            If Hechizos(SpellIndex).SubeHam = 1 Then

                Call InfoHechizo(UserIndex)

                daño = RandomNumber(Hechizos(SpellIndex).MinHam, Hechizos(SpellIndex).MaxHam)

                .Stats.MinHam = .Stats.MinHam + daño
                If .Stats.MinHam > .Stats.MaxHam Then .Stats.MinHam = .Stats.MaxHam

                If UserIndex <> TargetIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Le has restaurado " & daño & " puntos de hambre a " & .name & ".",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Call _
                        WriteConsoleMsg(TargetIndex,
                                        UserList(UserIndex).name & " te ha restaurado " & daño & " puntos de hambre.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "Te has restaurado " & daño & " puntos de hambre.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                End If

                Call WriteUpdateHungerAndThirst(TargetIndex)

                ' <-------- Quita Hambre ---------->
            ElseIf Hechizos(SpellIndex).SubeHam = 2 Then
                If Not PuedeAtacar(UserIndex, TargetIndex) Then Exit Function

                If UserIndex <> TargetIndex Then
                    Call UsuarioAtacadoPorUsuario(UserIndex, TargetIndex)
                Else
                    Exit Function
                End If

                Call InfoHechizo(UserIndex)

                daño = RandomNumber(Hechizos(SpellIndex).MinHam, Hechizos(SpellIndex).MaxHam)

                .Stats.MinHam = .Stats.MinHam - daño

                If UserIndex <> TargetIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Le has quitado " & daño & " puntos de hambre a " & .name & ".",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Call _
                        WriteConsoleMsg(TargetIndex,
                                        UserList(UserIndex).name & " te ha quitado " & daño & " puntos de hambre.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "Te has quitado " & daño & " puntos de hambre.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                End If

                If .Stats.MinHam < 1 Then
                    .Stats.MinHam = 0
                    .flags.Hambre = 1
                End If

                Call WriteUpdateHungerAndThirst(TargetIndex)
            End If

            ' <-------- Aumenta Sed ---------->
            If Hechizos(SpellIndex).SubeSed = 1 Then

                Call InfoHechizo(UserIndex)

                daño = RandomNumber(Hechizos(SpellIndex).MinSed, Hechizos(SpellIndex).MaxSed)

                .Stats.MinAGU = .Stats.MinAGU + daño
                If .Stats.MinAGU > .Stats.MaxAGU Then .Stats.MinAGU = .Stats.MaxAGU

                Call WriteUpdateHungerAndThirst(TargetIndex)

                If UserIndex <> TargetIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Le has restaurado " & daño & " puntos de sed a " & .name & ".",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Call _
                        WriteConsoleMsg(TargetIndex,
                                        UserList(UserIndex).name & " te ha restaurado " & daño & " puntos de sed.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "Te has restaurado " & daño & " puntos de sed.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                End If


                ' <-------- Quita Sed ---------->
            ElseIf Hechizos(SpellIndex).SubeSed = 2 Then

                If Not PuedeAtacar(UserIndex, TargetIndex) Then Exit Function

                If UserIndex <> TargetIndex Then
                    Call UsuarioAtacadoPorUsuario(UserIndex, TargetIndex)
                End If

                Call InfoHechizo(UserIndex)

                daño = RandomNumber(Hechizos(SpellIndex).MinSed, Hechizos(SpellIndex).MaxSed)

                .Stats.MinAGU = .Stats.MinAGU - daño

                If UserIndex <> TargetIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Le has quitado " & daño & " puntos de sed a " & .name & ".",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Call _
                        WriteConsoleMsg(TargetIndex,
                                        UserList(UserIndex).name & " te ha quitado " & daño & " puntos de sed.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "Te has quitado " & daño & " puntos de sed.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                End If

                If .Stats.MinAGU < 1 Then
                    .Stats.MinAGU = 0
                    .flags.Sed = 1
                End If

                Call WriteUpdateHungerAndThirst(TargetIndex)

            End If

            ' <-------- Aumenta Agilidad ---------->
            If Hechizos(SpellIndex).SubeAgilidad = 1 Then

                ' Chequea si el status permite ayudar al otro usuario
                If Not CanSupportUser(UserIndex, TargetIndex) Then Exit Function

                Call InfoHechizo(UserIndex)
                daño = RandomNumber(Hechizos(SpellIndex).MinAgilidad, Hechizos(SpellIndex).MaxAgilidad)

                .flags.DuracionEfecto = 1200
                .Stats.UserAtributos(Declaraciones.eAtributos.Agilidad) =
                    .Stats.UserAtributos(Declaraciones.eAtributos.Agilidad) + daño
                If _
                    .Stats.UserAtributos(Declaraciones.eAtributos.Agilidad) >
                    MinimoInt(MAXATRIBUTOS, .Stats.UserAtributosBackUP(Declaraciones.eAtributos.Agilidad)*2) Then _
                    .Stats.UserAtributos(Declaraciones.eAtributos.Agilidad) = MinimoInt(MAXATRIBUTOS,
                                                                                        .Stats.UserAtributosBackUP(
                                                                                            Declaraciones.eAtributos.
                                                                                                                      Agilidad)*
                                                                                        2)

                .flags.TomoPocion = True
                Call WriteUpdateDexterity(TargetIndex)

                ' <-------- Quita Agilidad ---------->
            ElseIf Hechizos(SpellIndex).SubeAgilidad = 2 Then

                If Not PuedeAtacar(UserIndex, TargetIndex) Then Exit Function

                If UserIndex <> TargetIndex Then
                    Call UsuarioAtacadoPorUsuario(UserIndex, TargetIndex)
                End If

                Call InfoHechizo(UserIndex)

                .flags.TomoPocion = True
                daño = RandomNumber(Hechizos(SpellIndex).MinAgilidad, Hechizos(SpellIndex).MaxAgilidad)
                .flags.DuracionEfecto = 700
                .Stats.UserAtributos(Declaraciones.eAtributos.Agilidad) =
                    .Stats.UserAtributos(Declaraciones.eAtributos.Agilidad) - daño
                If .Stats.UserAtributos(Declaraciones.eAtributos.Agilidad) < MINATRIBUTOS Then _
                    .Stats.UserAtributos(Declaraciones.eAtributos.Agilidad) = MINATRIBUTOS

                Call WriteUpdateDexterity(TargetIndex)
            End If

            ' <-------- Aumenta Fuerza ---------->
            If Hechizos(SpellIndex).SubeFuerza = 1 Then

                ' Chequea si el status permite ayudar al otro usuario
                If Not CanSupportUser(UserIndex, TargetIndex) Then Exit Function

                Call InfoHechizo(UserIndex)
                daño = RandomNumber(Hechizos(SpellIndex).MinFuerza, Hechizos(SpellIndex).MaxFuerza)

                .flags.DuracionEfecto = 1200

                .Stats.UserAtributos(Declaraciones.eAtributos.Fuerza) =
                    .Stats.UserAtributos(Declaraciones.eAtributos.Fuerza) + daño
                If _
                    .Stats.UserAtributos(Declaraciones.eAtributos.Fuerza) >
                    MinimoInt(MAXATRIBUTOS, .Stats.UserAtributosBackUP(Declaraciones.eAtributos.Fuerza)*2) Then _
                    .Stats.UserAtributos(Declaraciones.eAtributos.Fuerza) = MinimoInt(MAXATRIBUTOS,
                                                                                      .Stats.UserAtributosBackUP(
                                                                                          Declaraciones.eAtributos.
                                                                                                                    Fuerza)*
                                                                                      2)

                .flags.TomoPocion = True
                Call WriteUpdateStrenght(TargetIndex)

                ' <-------- Quita Fuerza ---------->
            ElseIf Hechizos(SpellIndex).SubeFuerza = 2 Then

                If Not PuedeAtacar(UserIndex, TargetIndex) Then Exit Function

                If UserIndex <> TargetIndex Then
                    Call UsuarioAtacadoPorUsuario(UserIndex, TargetIndex)
                End If

                Call InfoHechizo(UserIndex)

                .flags.TomoPocion = True

                daño = RandomNumber(Hechizos(SpellIndex).MinFuerza, Hechizos(SpellIndex).MaxFuerza)
                .flags.DuracionEfecto = 700
                .Stats.UserAtributos(Declaraciones.eAtributos.Fuerza) =
                    .Stats.UserAtributos(Declaraciones.eAtributos.Fuerza) - daño
                If .Stats.UserAtributos(Declaraciones.eAtributos.Fuerza) < MINATRIBUTOS Then _
                    .Stats.UserAtributos(Declaraciones.eAtributos.Fuerza) = MINATRIBUTOS

                Call WriteUpdateStrenght(TargetIndex)
            End If

            ' <-------- Cura salud ---------->
            If Hechizos(SpellIndex).SubeHP = 1 Then

                'Verifica que el usuario no este muerto
                If .flags.Muerto = 1 Then
                    Call WriteConsoleMsg(UserIndex, "¡El usuario está muerto!", Protocol.FontTypeNames.FONTTYPE_INFO)
                    Exit Function
                End If

                ' Chequea si el status permite ayudar al otro usuario
                If Not CanSupportUser(UserIndex, TargetIndex) Then Exit Function

                daño = RandomNumber(Hechizos(SpellIndex).MinHp, Hechizos(SpellIndex).MaxHp)
                daño = daño + Porcentaje(daño, 3*UserList(UserIndex).Stats.ELV)

                Call InfoHechizo(UserIndex)

                .Stats.MinHp = .Stats.MinHp + daño
                If .Stats.MinHp > .Stats.MaxHp Then .Stats.MinHp = .Stats.MaxHp

                Call WriteUpdateHP(TargetIndex)

                If UserIndex <> TargetIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Le has restaurado " & daño & " puntos de vida a " & .name & ".",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Call _
                        WriteConsoleMsg(TargetIndex,
                                        UserList(UserIndex).name & " te ha restaurado " & daño & " puntos de vida.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "Te has restaurado " & daño & " puntos de vida.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                End If

                ' <-------- Quita salud (Daña) ---------->
            ElseIf Hechizos(SpellIndex).SubeHP = 2 Then

                If UserIndex = TargetIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes atacarte a vos mismo.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Exit Function
                End If

                daño = RandomNumber(Hechizos(SpellIndex).MinHp, Hechizos(SpellIndex).MaxHp)

                daño = daño + Porcentaje(daño, 3*UserList(UserIndex).Stats.ELV)

                If Hechizos(SpellIndex).StaffAffected Then
                    If UserList(UserIndex).clase = Declaraciones.eClass.Mage Then
                        If UserList(UserIndex).Invent.WeaponEqpObjIndex > 0 Then
                            daño =
                                (daño*
                                 (ObjData_Renamed(UserList(UserIndex).Invent.WeaponEqpObjIndex).StaffDamageBonus + 70))/
                                100
                        Else
                            daño = daño*0.7 'Baja daño a 70% del original
                        End If
                    End If
                End If

                If _
                    UserList(UserIndex).Invent.AnilloEqpObjIndex = LAUDELFICO Or
                    UserList(UserIndex).Invent.AnilloEqpObjIndex = FLAUTAELFICA Then
                    daño = daño*1.04 'laud magico de los bardos
                End If

                'cascos antimagia
                If (.Invent.CascoEqpObjIndex > 0) Then
                    daño = daño -
                           RandomNumber(ObjData_Renamed(.Invent.CascoEqpObjIndex).DefensaMagicaMin,
                                        ObjData_Renamed(.Invent.CascoEqpObjIndex).DefensaMagicaMax)
                End If

                'anillos
                If (.Invent.AnilloEqpObjIndex > 0) Then
                    daño = daño -
                           RandomNumber(ObjData_Renamed(.Invent.AnilloEqpObjIndex).DefensaMagicaMin,
                                        ObjData_Renamed(.Invent.AnilloEqpObjIndex).DefensaMagicaMax)
                End If

                If daño < 0 Then daño = 0

                If Not PuedeAtacar(UserIndex, TargetIndex) Then Exit Function

                If UserIndex <> TargetIndex Then
                    Call UsuarioAtacadoPorUsuario(UserIndex, TargetIndex)
                End If

                Call InfoHechizo(UserIndex)

                .Stats.MinHp = .Stats.MinHp - daño

                Call WriteUpdateHP(TargetIndex)

                Call _
                    WriteConsoleMsg(UserIndex, "Le has quitado " & daño & " puntos de vida a " & .name & ".",
                                    Protocol.FontTypeNames.FONTTYPE_FIGHT)
                Call _
                    WriteConsoleMsg(TargetIndex,
                                    UserList(UserIndex).name & " te ha quitado " & daño & " puntos de vida.",
                                    Protocol.FontTypeNames.FONTTYPE_FIGHT)

                'Muere
                If .Stats.MinHp < 1 Then

                    If .flags.AtacablePor <> UserIndex Then
                        'Store it!
                        Call Statistics.StoreFrag(UserIndex, TargetIndex)
                        Call ContarMuerte(TargetIndex, UserIndex)
                    End If

                    .Stats.MinHp = 0
                    Call ActStats(TargetIndex, UserIndex)
                    Call UserDie(TargetIndex)
                End If

            End If

            ' <-------- Aumenta Mana ---------->
            If Hechizos(SpellIndex).SubeMana = 1 Then

                Call InfoHechizo(UserIndex)
                .Stats.MinMAN = .Stats.MinMAN + daño
                If .Stats.MinMAN > .Stats.MaxMAN Then .Stats.MinMAN = .Stats.MaxMAN

                Call WriteUpdateMana(TargetIndex)

                If UserIndex <> TargetIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Le has restaurado " & daño & " puntos de maná a " & .name & ".",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Call _
                        WriteConsoleMsg(TargetIndex,
                                        UserList(UserIndex).name & " te ha restaurado " & daño & " puntos de maná.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "Te has restaurado " & daño & " puntos de maná.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                End If


                ' <-------- Quita Mana ---------->
            ElseIf Hechizos(SpellIndex).SubeMana = 2 Then
                If Not PuedeAtacar(UserIndex, TargetIndex) Then Exit Function

                If UserIndex <> TargetIndex Then
                    Call UsuarioAtacadoPorUsuario(UserIndex, TargetIndex)
                End If

                Call InfoHechizo(UserIndex)

                If UserIndex <> TargetIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Le has quitado " & daño & " puntos de maná a " & .name & ".",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Call _
                        WriteConsoleMsg(TargetIndex,
                                        UserList(UserIndex).name & " te ha quitado " & daño & " puntos de maná.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "Te has quitado " & daño & " puntos de maná.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                End If

                .Stats.MinMAN = .Stats.MinMAN - daño
                If .Stats.MinMAN < 1 Then .Stats.MinMAN = 0

                Call WriteUpdateMana(TargetIndex)

            End If

            ' <-------- Aumenta Stamina ---------->
            If Hechizos(SpellIndex).SubeSta = 1 Then
                Call InfoHechizo(UserIndex)
                .Stats.MinSta = .Stats.MinSta + daño
                If .Stats.MinSta > .Stats.MaxSta Then .Stats.MinSta = .Stats.MaxSta

                Call WriteUpdateSta(TargetIndex)

                If UserIndex <> TargetIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Le has restaurado " & daño & " puntos de energía a " & .name & ".",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Call _
                        WriteConsoleMsg(TargetIndex,
                                        UserList(UserIndex).name & " te ha restaurado " & daño & " puntos de energía.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "Te has restaurado " & daño & " puntos de energía.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                End If

                ' <-------- Quita Stamina ---------->
            ElseIf Hechizos(SpellIndex).SubeSta = 2 Then
                If Not PuedeAtacar(UserIndex, TargetIndex) Then Exit Function

                If UserIndex <> TargetIndex Then
                    Call UsuarioAtacadoPorUsuario(UserIndex, TargetIndex)
                End If

                Call InfoHechizo(UserIndex)

                If UserIndex <> TargetIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Le has quitado " & daño & " puntos de energía a " & .name & ".",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    Call _
                        WriteConsoleMsg(TargetIndex,
                                        UserList(UserIndex).name & " te ha quitado " & daño & " puntos de energía.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "Te has quitado " & daño & " puntos de energía.",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                End If

                .Stats.MinSta = .Stats.MinSta - daño

                If .Stats.MinSta < 1 Then .Stats.MinSta = 0

                Call WriteUpdateSta(TargetIndex)

            End If
        End With

        HechizoPropUsuario = True

        Call FlushBuffer(TargetIndex)
    End Function

    Public Function CanSupportUser(ByVal CasterIndex As Short, ByVal TargetIndex As Short,
                                   Optional ByVal DoCriminal As Boolean = False) As Boolean
        '***************************************************
        'Author: ZaMa
        'Last Modification: 28/04/2010
        'Checks if caster can cast support magic on target user.
        '***************************************************

        On Error GoTo Errhandler

        With UserList(CasterIndex)

            ' Te podes curar a vos mismo
            If CasterIndex = TargetIndex Then
                CanSupportUser = True
                Exit Function
            End If

            ' No podes ayudar si estas en consulta
            If .flags.EnConsulta Then
                Call _
                    WriteConsoleMsg(CasterIndex, "No puedes ayudar usuarios mientras estas en consulta.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO)
                Exit Function
            End If

            ' Si estas en la arena, esta todo permitido
            If TriggerZonaPelea(CasterIndex, TargetIndex) = Declaraciones.eTrigger6.TRIGGER6_PERMITE Then
                CanSupportUser = True
                Exit Function
            End If

            ' Victima criminal?
            If criminal(TargetIndex) Then

                ' Casteador Ciuda?
                If Not criminal(CasterIndex) Then

                    ' Armadas no pueden ayudar
                    If esArmada(CasterIndex) Then
                        Call _
                            WriteConsoleMsg(CasterIndex,
                                            "Los miembros del ejército real no pueden ayudar a los criminales.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        Exit Function
                    End If

                    ' Si el ciuda tiene el seguro puesto no puede ayudar
                    If .flags.Seguro Then
                        Call _
                            WriteConsoleMsg(CasterIndex,
                                            "Para ayudar criminales debes sacarte el seguro ya que te volverás criminal como ellos.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        Exit Function
                    Else
                        ' Penalizacion
                        If DoCriminal Then
                            Call VolverCriminal(CasterIndex)
                        Else
                            Call DisNobAuBan(CasterIndex, .Reputacion.NobleRep*0.5, 10000)
                        End If
                    End If
                End If

                ' Victima ciuda o army
            Else
                ' Casteador es caos? => No Pueden ayudar ciudas
                If esCaos(CasterIndex) Then
                    Call _
                        WriteConsoleMsg(CasterIndex,
                                        "Los miembros de la legión oscura no pueden ayudar a los ciudadanos.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                    Exit Function

                    ' Casteador ciuda/army?
                ElseIf Not criminal(CasterIndex) Then

                    ' Esta en estado atacable?
                    If UserList(TargetIndex).flags.AtacablePor > 0 Then

                        ' No esta atacable por el casteador?
                        If UserList(TargetIndex).flags.AtacablePor <> CasterIndex Then

                            ' Si es armada no puede ayudar
                            If esArmada(CasterIndex) Then
                                Call _
                                    WriteConsoleMsg(CasterIndex,
                                                    "Los miembros del ejército real no pueden ayudar a ciudadanos en estado atacable.",
                                                    Protocol.FontTypeNames.FONTTYPE_INFO)
                                Exit Function
                            End If

                            ' Seguro puesto?
                            If .flags.Seguro Then
                                Call _
                                    WriteConsoleMsg(CasterIndex,
                                                    "Para ayudar ciudadanos en estado atacable debes sacarte el seguro, pero te puedes volver criminal.",
                                                    Protocol.FontTypeNames.FONTTYPE_INFO)
                                Exit Function
                            Else
                                Call DisNobAuBan(CasterIndex, .Reputacion.NobleRep*0.5, 10000)
                            End If
                        End If
                    End If

                End If
            End If
        End With

        CanSupportUser = True

        Exit Function

        Errhandler:
        Call _
            LogError(
                "Error en CanSupportUser, Error: " & Err.Number & " - " & Err.Description & " CasterIndex: " &
                CasterIndex & ", TargetIndex: " & TargetIndex)
    End Function

    Sub UpdateUserHechizos(ByVal UpdateAll As Boolean, ByVal UserIndex As Short, ByVal Slot As Byte)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim LoopC As Byte

        With UserList(UserIndex)
            'Actualiza un solo slot
            If Not UpdateAll Then
                'Actualiza el inventario
                If .Stats.UserHechizos(Slot) > 0 Then
                    Call ChangeUserHechizo(UserIndex, Slot, .Stats.UserHechizos(Slot))
                Else
                    Call ChangeUserHechizo(UserIndex, Slot, 0)
                End If
            Else
                'Actualiza todos los slots
                For LoopC = 1 To MAXUSERHECHIZOS
                    'Actualiza el inventario
                    If .Stats.UserHechizos(LoopC) > 0 Then
                        Call ChangeUserHechizo(UserIndex, LoopC, .Stats.UserHechizos(LoopC))
                    Else
                        Call ChangeUserHechizo(UserIndex, LoopC, 0)
                    End If
                Next LoopC
            End If
        End With
    End Sub

    Sub ChangeUserHechizo(ByVal UserIndex As Short, ByVal Slot As Byte, ByVal Hechizo As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        UserList(UserIndex).Stats.UserHechizos(Slot) = Hechizo

        If Hechizo > 0 And Hechizo < NumeroHechizos + 1 Then
            Call WriteChangeSpellSlot(UserIndex, Slot)
        Else
            Call WriteChangeSpellSlot(UserIndex, Slot)
        End If
    End Sub


    Public Sub DesplazarHechizo(ByVal UserIndex As Short, ByVal Dire As Short, ByVal HechizoDesplazado As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        If (Dire <> 1 And Dire <> - 1) Then Exit Sub
        If Not (HechizoDesplazado >= 1 And HechizoDesplazado <= MAXUSERHECHIZOS) Then Exit Sub

        Dim TempHechizo As Short

        With UserList(UserIndex)
            If Dire = 1 Then 'Mover arriba
                If HechizoDesplazado = 1 Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes mover el hechizo en esa dirección.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                Else
                    TempHechizo = .Stats.UserHechizos(HechizoDesplazado)
                    .Stats.UserHechizos(HechizoDesplazado) = .Stats.UserHechizos(HechizoDesplazado - 1)
                    .Stats.UserHechizos(HechizoDesplazado - 1) = TempHechizo
                End If
            Else 'mover abajo
                If HechizoDesplazado = MAXUSERHECHIZOS Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes mover el hechizo en esa dirección.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                Else
                    TempHechizo = .Stats.UserHechizos(HechizoDesplazado)
                    .Stats.UserHechizos(HechizoDesplazado) = .Stats.UserHechizos(HechizoDesplazado + 1)
                    .Stats.UserHechizos(HechizoDesplazado + 1) = TempHechizo
                End If
            End If
        End With
    End Sub

    Public Sub DisNobAuBan(ByVal UserIndex As Short, ByRef NoblePts As Integer, ByRef BandidoPts As Integer)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'disminuye la nobleza NoblePts puntos y aumenta el bandido BandidoPts puntos
        Dim EraCriminal As Boolean
        EraCriminal = criminal(UserIndex)

        With UserList(UserIndex)
            'Si estamos en la arena no hacemos nada
            If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = 6 Then Exit Sub

            If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero) Then
                'pierdo nobleza...
                .Reputacion.NobleRep = .Reputacion.NobleRep - NoblePts
                If .Reputacion.NobleRep < 0 Then
                    .Reputacion.NobleRep = 0
                End If

                'gano bandido...
                .Reputacion.BandidoRep = .Reputacion.BandidoRep + BandidoPts
                If .Reputacion.BandidoRep > MAXREP Then .Reputacion.BandidoRep = MAXREP
                Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.NobilityLost) _
                'Call WriteNobilityLost(UserIndex)
                If criminal(UserIndex) Then If .Faccion.ArmadaReal = 1 Then Call ExpulsarFaccionReal(UserIndex)
            End If

            If Not EraCriminal And criminal(UserIndex) Then
                Call RefreshCharStatus(UserIndex)
            End If
        End With
    End Sub
End Module
