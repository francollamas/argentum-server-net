Option Strict Off
Option Explicit On
Module SistemaCombate
    Public Const MAXDISTANCIAARCO As Byte = 18
    Public Const MAXDISTANCIAMAGIA As Byte = 18

    Public Function MinimoInt(ByVal a As Short, ByVal b As Short) As Short
        If a > b Then
            MinimoInt = b
        Else
            MinimoInt = a
        End If
    End Function

    Public Function MaximoInt(ByVal a As Short, ByVal b As Short) As Short
        If a > b Then
            MaximoInt = a
        Else
            MaximoInt = b
        End If
    End Function

    Private Function PoderEvasionEscudo(ByVal UserIndex As Short) As Integer
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        PoderEvasionEscudo =
            (UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Defensa)*
             ModClase_Renamed(UserList(UserIndex).clase).Escudo)/2
    End Function

    Private Function PoderEvasion(ByVal UserIndex As Short) As Integer
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************
        Dim lTemp As Integer
        With UserList(UserIndex)
            lTemp =
                (.Stats.UserSkills(Declaraciones.eSkill.Tacticas) +
                 .Stats.UserSkills(Declaraciones.eSkill.Tacticas)/33*
                 .Stats.UserAtributos(Declaraciones.eAtributos.Agilidad))*ModClase_Renamed(.clase).Evasion

            PoderEvasion = (lTemp + (2.5*MaximoInt(.Stats.ELV - 12, 0)))
        End With
    End Function

    Private Function PoderAtaqueArma(ByVal UserIndex As Short) As Integer
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim PoderAtaqueTemp As Integer

        With UserList(UserIndex)
            If .Stats.UserSkills(Declaraciones.eSkill.Armas) < 31 Then
                PoderAtaqueTemp = .Stats.UserSkills(Declaraciones.eSkill.Armas)*ModClase_Renamed(.clase).AtaqueArmas
            ElseIf .Stats.UserSkills(Declaraciones.eSkill.Armas) < 61 Then
                PoderAtaqueTemp =
                    (.Stats.UserSkills(Declaraciones.eSkill.Armas) +
                     .Stats.UserAtributos(Declaraciones.eAtributos.Agilidad))*ModClase_Renamed(.clase).AtaqueArmas
            ElseIf .Stats.UserSkills(Declaraciones.eSkill.Armas) < 91 Then
                PoderAtaqueTemp =
                    (.Stats.UserSkills(Declaraciones.eSkill.Armas) +
                     2*.Stats.UserAtributos(Declaraciones.eAtributos.Agilidad))*ModClase_Renamed(.clase).AtaqueArmas
            Else
                PoderAtaqueTemp =
                    (.Stats.UserSkills(Declaraciones.eSkill.Armas) +
                     3*.Stats.UserAtributos(Declaraciones.eAtributos.Agilidad))*ModClase_Renamed(.clase).AtaqueArmas
            End If

            PoderAtaqueArma = (PoderAtaqueTemp + (2.5*MaximoInt(.Stats.ELV - 12, 0)))
        End With
    End Function

    Private Function PoderAtaqueProyectil(ByVal UserIndex As Short) As Integer
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim PoderAtaqueTemp As Integer

        With UserList(UserIndex)
            If .Stats.UserSkills(Declaraciones.eSkill.Proyectiles) < 31 Then
                PoderAtaqueTemp = .Stats.UserSkills(Declaraciones.eSkill.Proyectiles)*
                                  ModClase_Renamed(.clase).AtaqueProyectiles
            ElseIf .Stats.UserSkills(Declaraciones.eSkill.Proyectiles) < 61 Then
                PoderAtaqueTemp =
                    (.Stats.UserSkills(Declaraciones.eSkill.Proyectiles) +
                     .Stats.UserAtributos(Declaraciones.eAtributos.Agilidad))*ModClase_Renamed(.clase).AtaqueProyectiles
            ElseIf .Stats.UserSkills(Declaraciones.eSkill.Proyectiles) < 91 Then
                PoderAtaqueTemp =
                    (.Stats.UserSkills(Declaraciones.eSkill.Proyectiles) +
                     2*.Stats.UserAtributos(Declaraciones.eAtributos.Agilidad))*
                    ModClase_Renamed(.clase).AtaqueProyectiles
            Else
                PoderAtaqueTemp =
                    (.Stats.UserSkills(Declaraciones.eSkill.Proyectiles) +
                     3*.Stats.UserAtributos(Declaraciones.eAtributos.Agilidad))*
                    ModClase_Renamed(.clase).AtaqueProyectiles
            End If

            PoderAtaqueProyectil = (PoderAtaqueTemp + (2.5*MaximoInt(.Stats.ELV - 12, 0)))
        End With
    End Function

    Private Function PoderAtaqueWrestling(ByVal UserIndex As Short) As Integer
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim PoderAtaqueTemp As Integer

        With UserList(UserIndex)
            If .Stats.UserSkills(Declaraciones.eSkill.Wrestling) < 31 Then
                PoderAtaqueTemp = .Stats.UserSkills(Declaraciones.eSkill.Wrestling)*
                                  ModClase_Renamed(.clase).AtaqueWrestling
            ElseIf .Stats.UserSkills(Declaraciones.eSkill.Wrestling) < 61 Then
                PoderAtaqueTemp =
                    (.Stats.UserSkills(Declaraciones.eSkill.Wrestling) +
                     .Stats.UserAtributos(Declaraciones.eAtributos.Agilidad))*ModClase_Renamed(.clase).AtaqueWrestling
            ElseIf .Stats.UserSkills(Declaraciones.eSkill.Wrestling) < 91 Then
                PoderAtaqueTemp =
                    (.Stats.UserSkills(Declaraciones.eSkill.Wrestling) +
                     2*.Stats.UserAtributos(Declaraciones.eAtributos.Agilidad))*ModClase_Renamed(.clase).AtaqueWrestling
            Else
                PoderAtaqueTemp =
                    (.Stats.UserSkills(Declaraciones.eSkill.Wrestling) +
                     3*.Stats.UserAtributos(Declaraciones.eAtributos.Agilidad))*ModClase_Renamed(.clase).AtaqueWrestling
            End If

            PoderAtaqueWrestling = (PoderAtaqueTemp + (2.5*MaximoInt(.Stats.ELV - 12, 0)))
        End With
    End Function

    Public Function UserImpactoNpc(ByVal UserIndex As Short, ByVal NpcIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim PoderAtaque As Integer
        Dim Arma As Short
        Dim Skill As Declaraciones.eSkill
        Dim ProbExito As Integer

        Arma = UserList(UserIndex).Invent.WeaponEqpObjIndex

        If Arma > 0 Then 'Usando un arma
            If ObjData_Renamed(Arma).proyectil = 1 Then
                PoderAtaque = PoderAtaqueProyectil(UserIndex)
                Skill = Declaraciones.eSkill.Proyectiles
            Else
                PoderAtaque = PoderAtaqueArma(UserIndex)
                Skill = Declaraciones.eSkill.Armas
            End If
        Else 'Peleando con puños
            PoderAtaque = PoderAtaqueWrestling(UserIndex)
            Skill = Declaraciones.eSkill.Wrestling
        End If

        ' Chances are rounded
        ProbExito = MaximoInt(10, MinimoInt(90, 50 + ((PoderAtaque - Npclist(NpcIndex).PoderEvasion)*0.4)))

        UserImpactoNpc = (RandomNumber(1, 100) <= ProbExito)

        If UserImpactoNpc Then
            Call SubirSkill(UserIndex, Skill, True)
        Else
            Call SubirSkill(UserIndex, Skill, False)
        End If
    End Function

    Public Function NpcImpacto(ByVal NpcIndex As Short, ByVal UserIndex As Short) As Boolean
        '*************************************************
        'Author: Unknown
        'Last modified: 03/15/2006
        'Revisa si un NPC logra impactar a un user o no
        '03/15/2006 Maraxus - Evité una división por cero que eliminaba NPCs
        '*************************************************
        Dim Rechazo As Boolean
        Dim ProbRechazo As Integer
        Dim ProbExito As Integer
        Dim UserEvasion As Integer
        Dim NpcPoderAtaque As Integer
        Dim PoderEvasioEscudo As Integer
        Dim SkillTacticas As Integer
        Dim SkillDefensa As Integer

        UserEvasion = PoderEvasion(UserIndex)
        NpcPoderAtaque = Npclist(NpcIndex).PoderAtaque
        PoderEvasioEscudo = PoderEvasionEscudo(UserIndex)

        SkillTacticas = UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Tacticas)
        SkillDefensa = UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Defensa)

        'Esta usando un escudo ???
        If UserList(UserIndex).Invent.EscudoEqpObjIndex > 0 Then UserEvasion = UserEvasion + PoderEvasioEscudo

        ' Chances are rounded
        ProbExito = MaximoInt(10, MinimoInt(90, 50 + ((NpcPoderAtaque - UserEvasion)*0.4)))

        NpcImpacto = (RandomNumber(1, 100) <= ProbExito)

        ' el usuario esta usando un escudo ???
        If UserList(UserIndex).Invent.EscudoEqpObjIndex > 0 Then
            If Not NpcImpacto Then
                If SkillDefensa + SkillTacticas > 0 Then 'Evitamos división por cero
                    ' Chances are rounded
                    ProbRechazo = MaximoInt(10, MinimoInt(90, 100*SkillDefensa/(SkillDefensa + SkillTacticas)))
                    Rechazo = (RandomNumber(1, 100) <= ProbRechazo)

                    If Rechazo Then
                        'Se rechazo el ataque con el escudo
                        Call _
                            SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                     PrepareMessagePlayWave(SND_ESCUDO, UserList(UserIndex).Pos.X,
                                                            UserList(UserIndex).Pos.Y))
                        Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.BlockedWithShieldUser) _
                        'Call WriteBlockedWithShieldUser(UserIndex)
                        Call SubirSkill(UserIndex, Declaraciones.eSkill.Defensa, True)
                    Else
                        Call SubirSkill(UserIndex, Declaraciones.eSkill.Defensa, False)
                    End If
                End If
            End If
        End If
    End Function

    Public Function CalcularDaño(ByVal UserIndex As Short, Optional ByVal NpcIndex As Short = 0) As Integer
        '***************************************************
        'Author: Unknown
        'Last Modification: 01/04/2010 (ZaMa)
        '01/04/2010: ZaMa - Modifico el daño de wrestling.
        '01/04/2010: ZaMa - Agrego bonificadores de wrestling para los guantes.
        '***************************************************
        Dim DañoArma As Integer
        Dim DañoUsuario As Integer
        'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Arma, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Dim Arma As ObjData
        Dim ModifClase As Single
        'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura proyectil, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Dim proyectil As ObjData
        Dim DañoMaxArma As Integer
        Dim DañoMinArma As Integer
        Dim ObjIndex As Short

        ''sacar esto si no queremos q la matadracos mate el Dragon si o si
        Dim matoDragon As Boolean
        matoDragon = False

        With UserList(UserIndex)
            If .Invent.WeaponEqpObjIndex > 0 Then
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Arma. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                Arma = ObjData_Renamed(.Invent.WeaponEqpObjIndex)

                ' Ataca a un npc?
                If NpcIndex > 0 Then
                    If Arma.proyectil = 1 Then
                        ModifClase = ModClase_Renamed(.clase).DañoProyectiles
                        DañoArma = RandomNumber(Arma.MinHIT, Arma.MaxHIT)
                        DañoMaxArma = Arma.MaxHIT

                        If Arma.Municion = 1 Then
                            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto proyectil. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            proyectil = ObjData_Renamed(.Invent.MunicionEqpObjIndex)
                            DañoArma = DañoArma + RandomNumber(proyectil.MinHIT, proyectil.MaxHIT)
                            ' For some reason this isn't done...
                            'DañoMaxArma = DañoMaxArma + proyectil.MaxHIT
                        End If
                    Else
                        ModifClase = ModClase_Renamed(.clase).DañoArmas

                        If .Invent.WeaponEqpObjIndex = EspadaMataDragonesIndex Then ' Usa la mata Dragones?
                            If Npclist(NpcIndex).NPCtype = Declaraciones.eNPCType.DRAGON Then 'Ataca Dragon?
                                DañoArma = RandomNumber(Arma.MinHIT, Arma.MaxHIT)
                                DañoMaxArma = Arma.MaxHIT
                                matoDragon = True ''sacar esto si no queremos q la matadracos mate el Dragon si o si
                            Else ' Sino es Dragon daño es 1
                                DañoArma = 1
                                DañoMaxArma = 1
                            End If
                        Else
                            DañoArma = RandomNumber(Arma.MinHIT, Arma.MaxHIT)
                            DañoMaxArma = Arma.MaxHIT
                        End If
                    End If
                Else ' Ataca usuario
                    If Arma.proyectil = 1 Then
                        ModifClase = ModClase_Renamed(.clase).DañoProyectiles
                        DañoArma = RandomNumber(Arma.MinHIT, Arma.MaxHIT)
                        DañoMaxArma = Arma.MaxHIT

                        If Arma.Municion = 1 Then
                            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto proyectil. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            proyectil = ObjData_Renamed(.Invent.MunicionEqpObjIndex)
                            DañoArma = DañoArma + RandomNumber(proyectil.MinHIT, proyectil.MaxHIT)
                            ' For some reason this isn't done...
                            'DañoMaxArma = DañoMaxArma + proyectil.MaxHIT
                        End If
                    Else
                        ModifClase = ModClase_Renamed(.clase).DañoArmas

                        If .Invent.WeaponEqpObjIndex = EspadaMataDragonesIndex Then
                            ModifClase = ModClase_Renamed(.clase).DañoArmas
                            DañoArma = 1 ' Si usa la espada mataDragones daño es 1
                            DañoMaxArma = 1
                        Else
                            DañoArma = RandomNumber(Arma.MinHIT, Arma.MaxHIT)
                            DañoMaxArma = Arma.MaxHIT
                        End If
                    End If
                End If
            Else
                ModifClase = ModClase_Renamed(.clase).DañoWrestling

                ' Daño sin guantes
                DañoMinArma = 4
                DañoMaxArma = 9

                ' Plus de guantes (en slot de anillo)
                ObjIndex = .Invent.AnilloEqpObjIndex
                If ObjIndex > 0 Then
                    If ObjData_Renamed(ObjIndex).Guante = 1 Then
                        DañoMinArma = DañoMinArma + ObjData_Renamed(ObjIndex).MinHIT
                        DañoMaxArma = DañoMaxArma + ObjData_Renamed(ObjIndex).MaxHIT
                    End If
                End If

                DañoArma = RandomNumber(DañoMinArma, DañoMaxArma)

            End If

            DañoUsuario = RandomNumber(.Stats.MinHIT, .Stats.MaxHIT)

            ''sacar esto si no queremos q la matadracos mate el Dragon si o si
            If matoDragon Then
                CalcularDaño = Npclist(NpcIndex).Stats.MinHp + Npclist(NpcIndex).Stats.def
            Else
                CalcularDaño =
                    (3*DañoArma +
                     ((DañoMaxArma/5)*MaximoInt(0, .Stats.UserAtributos(Declaraciones.eAtributos.Fuerza) - 15)) +
                     DañoUsuario)*ModifClase
            End If
        End With
    End Function

    Public Sub UserDañoNpc(ByVal UserIndex As Short, ByVal NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 07/04/2010 (ZaMa)
        '25/01/2010: ZaMa - Agrego poder acuchillar npcs.
        '07/04/2010: ZaMa - Los asesinos apuñalan acorde al daño base sin descontar la defensa del npc.
        '***************************************************

        Dim daño As Integer
        Dim DañoBase As Integer

        DañoBase = CalcularDaño(UserIndex, NpcIndex)

        'esta navegando? si es asi le sumamos el daño del barco
        If UserList(UserIndex).flags.Navegando = 1 Then
            If UserList(UserIndex).Invent.BarcoObjIndex > 0 Then
                DañoBase = DañoBase +
                           RandomNumber(ObjData_Renamed(UserList(UserIndex).Invent.BarcoObjIndex).MinHIT,
                                        ObjData_Renamed(UserList(UserIndex).Invent.BarcoObjIndex).MaxHIT)
            End If
        End If

        Dim j As Short
        With Npclist(NpcIndex)
            daño = DañoBase - .Stats.def

            If daño < 0 Then daño = 0

            'Call WriteUserHitNPC(UserIndex, daño)
            Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.UserHitNPC, daño)
            Call CalcularDarExp(UserIndex, NpcIndex, daño)
            .Stats.MinHp = .Stats.MinHp - daño

            If .Stats.MinHp > 0 Then
                'Trata de apuñalar por la espalda al enemigo
                If PuedeApuñalar(UserIndex) Then
                    Call DoApuñalar(UserIndex, NpcIndex, 0, DañoBase)
                End If

                'trata de dar golpe crítico
                Call DoGolpeCritico(UserIndex, NpcIndex, 0, daño)

                If PuedeAcuchillar(UserIndex) Then
                    Call DoAcuchillar(UserIndex, NpcIndex, 0, daño)
                End If
            End If


            If .Stats.MinHp <= 0 Then
                ' Si era un Dragon perdemos la espada mataDragones
                If .NPCtype = Declaraciones.eNPCType.DRAGON Then
                    'Si tiene equipada la matadracos se la sacamos
                    If UserList(UserIndex).Invent.WeaponEqpObjIndex = EspadaMataDragonesIndex Then
                        Call QuitarObjetos(EspadaMataDragonesIndex, 1, UserIndex)
                    End If
                    If .Stats.MaxHp > 100000 Then Call LogDesarrollo(UserList(UserIndex).name & " mató un dragón")
                End If

                ' Para que las mascotas no sigan intentando luchar y
                ' comiencen a seguir al amo
                For j = 1 To MAXMASCOTAS
                    If UserList(UserIndex).MascotasIndex(j) > 0 Then
                        If Npclist(UserList(UserIndex).MascotasIndex(j)).TargetNPC = NpcIndex Then
                            Npclist(UserList(UserIndex).MascotasIndex(j)).TargetNPC = 0
                            Npclist(UserList(UserIndex).MascotasIndex(j)).Movement = AI.TipoAI.SigueAmo
                        End If
                    End If
                Next j

                Call MuereNpc(NpcIndex, UserIndex)
            End If
        End With
    End Sub

    Public Sub NpcDaño(ByVal NpcIndex As Short, ByVal UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim daño As Short
        Dim Lugar As Short
        Dim absorbido As Short
        Dim defbarco As Short
        'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        'UPGRADE_NOTE: Obj se actualizó a Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim Obj_Renamed As ObjData

        daño = RandomNumber(Npclist(NpcIndex).Stats.MinHIT, Npclist(NpcIndex).Stats.MaxHIT)

        'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj2, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Dim Obj2 As ObjData
        With UserList(UserIndex)
            If .flags.Navegando = 1 And .Invent.BarcoObjIndex > 0 Then
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                Obj_Renamed = ObjData_Renamed(.Invent.BarcoObjIndex)
                defbarco = RandomNumber(Obj_Renamed.MinDef, Obj_Renamed.MaxDef)
            End If

            Lugar = RandomNumber(Declaraciones.PartesCuerpo.bCabeza, Declaraciones.PartesCuerpo.bTorso)

            Select Case Lugar
                Case Declaraciones.PartesCuerpo.bCabeza
                    'Si tiene casco absorbe el golpe
                    If .Invent.CascoEqpObjIndex > 0 Then
                        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Obj_Renamed = ObjData_Renamed(.Invent.CascoEqpObjIndex)
                        absorbido = RandomNumber(Obj_Renamed.MinDef, Obj_Renamed.MaxDef)
                    End If
                Case Else
                    'Si tiene armadura absorbe el golpe
                    If .Invent.ArmourEqpObjIndex > 0 Then
                        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Obj_Renamed = ObjData_Renamed(.Invent.ArmourEqpObjIndex)
                        If .Invent.EscudoEqpObjIndex Then
                            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj2. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            Obj2 = ObjData_Renamed(.Invent.EscudoEqpObjIndex)
                            absorbido = RandomNumber(Obj_Renamed.MinDef + Obj2.MinDef, Obj_Renamed.MaxDef + Obj2.MaxDef)
                        Else
                            absorbido = RandomNumber(Obj_Renamed.MinDef, Obj_Renamed.MaxDef)
                        End If
                    End If
            End Select

            absorbido = absorbido + defbarco
            daño = daño - absorbido
            If daño < 1 Then daño = 1

            Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.NPCHitUser, Lugar, daño)
            'Call WriteNPCHitUser(UserIndex, Lugar, daño)

            If .flags.Privilegios And Declaraciones.PlayerType.User Then .Stats.MinHp = .Stats.MinHp - daño

            If .flags.Meditando Then
                If _
                    daño >
                    Fix(
                        .Stats.MinHp/100*.Stats.UserAtributos(Declaraciones.eAtributos.Inteligencia)*
                        .Stats.UserSkills(Declaraciones.eSkill.Meditar)/100*12/(RandomNumber(0, 5) + 7)) Then
                    .flags.Meditando = False
                    Call WriteMeditateToggle(UserIndex)
                    Call WriteConsoleMsg(UserIndex, "Dejas de meditar.", Protocol.FontTypeNames.FONTTYPE_INFO)
                    .Char_Renamed.FX = 0
                    .Char_Renamed.loops = 0
                    Call _
                        SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                 PrepareMessageCreateFX(.Char_Renamed.CharIndex, 0, 0))
                End If
            End If

            'Muere el usuario
            If .Stats.MinHp <= 0 Then
                Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.NPCKillUser) _
                'Call WriteNPCKillUser(UserIndex) ' Le informamos que ha muerto ;)

                'Si lo mato un guardia
                If criminal(UserIndex) And Npclist(NpcIndex).NPCtype = Declaraciones.eNPCType.GuardiaReal Then
                    Call RestarCriminalidad(UserIndex)
                    If Not criminal(UserIndex) And .Faccion.FuerzasCaos = 1 Then Call ExpulsarFaccionCaos(UserIndex)
                End If

                If Npclist(NpcIndex).MaestroUser > 0 Then
                    Call AllFollowAmo(Npclist(NpcIndex).MaestroUser)
                Else
                    'Al matarlo no lo sigue mas
                    If Npclist(NpcIndex).Stats.Alineacion = 0 Then
                        Npclist(NpcIndex).Movement = Npclist(NpcIndex).flags.OldMovement
                        Npclist(NpcIndex).Hostile = Npclist(NpcIndex).flags.OldHostil
                        Npclist(NpcIndex).flags.AttackedBy = vbNullString
                    End If
                End If

                Call UserDie(UserIndex)
            End If
        End With
    End Sub

    Public Sub RestarCriminalidad(ByVal UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim EraCriminal As Boolean
        EraCriminal = criminal(UserIndex)

        With UserList(UserIndex).Reputacion
            If .BandidoRep > 0 Then
                .BandidoRep = .BandidoRep - vlASALTO
                If .BandidoRep < 0 Then .BandidoRep = 0
            ElseIf .LadronesRep > 0 Then
                .LadronesRep = .LadronesRep - (vlCAZADOR*10)
                If .LadronesRep < 0 Then .LadronesRep = 0
            End If
        End With

        If EraCriminal And Not criminal(UserIndex) Then
            Call RefreshCharStatus(UserIndex)
        End If
    End Sub

    Public Sub CheckPets(ByVal NpcIndex As Short, ByVal UserIndex As Short,
                         Optional ByVal CheckElementales As Boolean = True)
        '***************************************************
        'Author: Unknown
        'Last Modification: 15/04/2010
        '15/04/2010: ZaMa - Las mascotas no se apropian de npcs.
        '***************************************************

        Dim j As Short

        ' Si no tengo mascotas, para que cheaquear lo demas?
        If UserList(UserIndex).NroMascotas = 0 Then Exit Sub

        If Not PuedeAtacarNPC(UserIndex, NpcIndex, , True) Then Exit Sub

        With UserList(UserIndex)
            For j = 1 To MAXMASCOTAS
                If .MascotasIndex(j) > 0 Then
                    If .MascotasIndex(j) <> NpcIndex Then
                        If _
                            CheckElementales Or
                            (Npclist(.MascotasIndex(j)).Numero <> ELEMENTALFUEGO And
                             Npclist(.MascotasIndex(j)).Numero <> ELEMENTALTIERRA) Then

                            If Npclist(.MascotasIndex(j)).TargetNPC = 0 Then _
                                Npclist(.MascotasIndex(j)).TargetNPC = NpcIndex
                            Npclist(.MascotasIndex(j)).Movement = AI.TipoAI.NpcAtacaNpc
                        End If
                    End If
                End If
            Next j
        End With
    End Sub

    Public Sub AllFollowAmo(ByVal UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim j As Short

        For j = 1 To MAXMASCOTAS
            If UserList(UserIndex).MascotasIndex(j) > 0 Then
                Call FollowAmo(UserList(UserIndex).MascotasIndex(j))
            End If
        Next j
    End Sub

    Public Function NpcAtacaUser(ByVal NpcIndex As Short, ByVal UserIndex As Short) As Boolean
        '*************************************************
        'Author: Unknown
        'Last modified: -
        '
        '*************************************************

        With UserList(UserIndex)
            If .flags.AdminInvisible = 1 Then Exit Function
            If (Not .flags.Privilegios And Declaraciones.PlayerType.User) <> 0 And Not .flags.AdminPerseguible Then _
                Exit Function
        End With

        With Npclist(NpcIndex)
            ' El npc puede atacar ???
            If .CanAttack = 1 Then
                NpcAtacaUser = True
                Call CheckPets(NpcIndex, UserIndex, False)

                If .Target = 0 Then .Target = UserIndex

                If UserList(UserIndex).flags.AtacadoPorNpc = 0 And UserList(UserIndex).flags.AtacadoPorUser = 0 Then
                    UserList(UserIndex).flags.AtacadoPorNpc = NpcIndex
                End If
            Else
                NpcAtacaUser = False
                Exit Function
            End If

            .CanAttack = 0

            If .flags.Snd1 > 0 Then
                Call _
                    SendData(modSendData.SendTarget.ToNPCArea, NpcIndex,
                             PrepareMessagePlayWave(.flags.Snd1, .Pos.X, .Pos.Y))
            End If
        End With

        If NpcImpacto(NpcIndex, UserIndex) Then
            With UserList(UserIndex)
                Call _
                    SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                             PrepareMessagePlayWave(SND_IMPACTO, .Pos.X, .Pos.Y))

                If .flags.Meditando = False Then
                    If .flags.Navegando = 0 Then
                        Call _
                            SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                     PrepareMessageCreateFX(.Char_Renamed.CharIndex, FXSANGRE, 0))
                    End If
                End If

                Call NpcDaño(NpcIndex, UserIndex)
                Call WriteUpdateHP(UserIndex)

                '¿Puede envenenar?
                If Npclist(NpcIndex).Veneno = 1 Then Call NpcEnvenenarUser(UserIndex)
            End With

            Call SubirSkill(UserIndex, Declaraciones.eSkill.Tacticas, False)
        Else
            Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.NPCSwing)
            Call SubirSkill(UserIndex, Declaraciones.eSkill.Tacticas, True)
        End If

        'Controla el nivel del usuario
        Call CheckUserLevel(UserIndex)
    End Function

    Private Function NpcImpactoNpc(ByVal Atacante As Short, ByVal Victima As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim PoderAtt As Integer
        Dim PoderEva As Integer
        Dim ProbExito As Integer

        PoderAtt = Npclist(Atacante).PoderAtaque
        PoderEva = Npclist(Victima).PoderEvasion

        ' Chances are rounded
        ProbExito = MaximoInt(10, MinimoInt(90, 50 + (PoderAtt - PoderEva)*0.4))
        NpcImpactoNpc = (RandomNumber(1, 100) <= ProbExito)
    End Function

    Public Sub NpcDañoNpc(ByVal Atacante As Short, ByVal Victima As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim daño As Short

        With Npclist(Atacante)
            daño = RandomNumber(.Stats.MinHIT, .Stats.MaxHIT)
            Npclist(Victima).Stats.MinHp = Npclist(Victima).Stats.MinHp - daño

            If Npclist(Victima).Stats.MinHp < 1 Then
                .Movement = .flags.OldMovement

                If migr_LenB(.flags.AttackedBy) <> 0 Then
                    .Hostile = .flags.OldHostil
                End If

                If .MaestroUser > 0 Then
                    Call FollowAmo(Atacante)
                End If

                Call MuereNpc(Victima, .MaestroUser)
            End If
        End With
    End Sub

    Public Sub NpcAtacaNpc(ByVal Atacante As Short, ByVal Victima As Short,
                           Optional ByVal cambiarMOvimiento As Boolean = True)
        '*************************************************
        'Author: Unknown
        'Last modified: 01/03/2009
        '01/03/2009: ZaMa - Las mascotas no pueden atacar al rey si quedan pretorianos vivos.
        '*************************************************

        With Npclist(Atacante)

            'Es el Rey Preatoriano?
            If Npclist(Victima).Numero = PRKING_NPC Then
                If pretorianosVivos > 0 Then
                    Call _
                        WriteConsoleMsg(.MaestroUser, "Debes matar al resto del ejército antes de atacar al rey!",
                                        Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    .TargetNPC = 0
                    Exit Sub
                End If
            End If

            ' El npc puede atacar ???
            If .CanAttack = 1 Then
                .CanAttack = 0
                If cambiarMOvimiento Then
                    Npclist(Victima).TargetNPC = Atacante
                    Npclist(Victima).Movement = AI.TipoAI.NpcAtacaNpc
                End If
            Else
                Exit Sub
            End If

            If .flags.Snd1 > 0 Then
                Call _
                    SendData(modSendData.SendTarget.ToNPCArea, Atacante,
                             PrepareMessagePlayWave(.flags.Snd1, .Pos.X, .Pos.Y))
            End If

            If NpcImpactoNpc(Atacante, Victima) Then
                If Npclist(Victima).flags.Snd2 > 0 Then
                    Call _
                        SendData(modSendData.SendTarget.ToNPCArea, Victima,
                                 PrepareMessagePlayWave(Npclist(Victima).flags.Snd2, Npclist(Victima).Pos.X,
                                                        Npclist(Victima).Pos.Y))
                Else
                    Call _
                        SendData(modSendData.SendTarget.ToNPCArea, Victima,
                                 PrepareMessagePlayWave(SND_IMPACTO2, Npclist(Victima).Pos.X, Npclist(Victima).Pos.Y))
                End If

                If .MaestroUser > 0 Then
                    Call _
                        SendData(modSendData.SendTarget.ToNPCArea, Atacante,
                                 PrepareMessagePlayWave(SND_IMPACTO, .Pos.X, .Pos.Y))
                Else
                    Call _
                        SendData(modSendData.SendTarget.ToNPCArea, Victima,
                                 PrepareMessagePlayWave(SND_IMPACTO, Npclist(Victima).Pos.X, Npclist(Victima).Pos.Y))
                End If

                Call NpcDañoNpc(Atacante, Victima)
            Else
                If .MaestroUser > 0 Then
                    Call _
                        SendData(modSendData.SendTarget.ToNPCArea, Atacante,
                                 PrepareMessagePlayWave(SND_SWING, .Pos.X, .Pos.Y))
                Else
                    Call _
                        SendData(modSendData.SendTarget.ToNPCArea, Victima,
                                 PrepareMessagePlayWave(SND_SWING, Npclist(Victima).Pos.X, Npclist(Victima).Pos.Y))
                End If
            End If
        End With
    End Sub

    Public Function UsuarioAtacaNpc(ByVal UserIndex As Short, ByVal NpcIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: 14/01/2010 (ZaMa)
        '12/01/2010: ZaMa - Los druidas pierden la inmunidad de ser atacados por npcs cuando los atacan.
        '14/01/2010: ZaMa - Lo transformo en función, para que no se pierdan municiones al atacar targets inválidos.
        '***************************************************

        Try

        If Not PuedeAtacarNPC(UserIndex, NpcIndex) Then Exit Function

        Call NPCAtacado(NpcIndex, UserIndex)

        If UserImpactoNpc(UserIndex, NpcIndex) Then
            If Npclist(NpcIndex).flags.Snd2 > 0 Then
                Call _
                    SendData(modSendData.SendTarget.ToNPCArea, NpcIndex,
                             PrepareMessagePlayWave(Npclist(NpcIndex).flags.Snd2, Npclist(NpcIndex).Pos.X,
                                                    Npclist(NpcIndex).Pos.Y))
            Else
                Call _
                    SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                             PrepareMessagePlayWave(SND_IMPACTO2, Npclist(NpcIndex).Pos.X, Npclist(NpcIndex).Pos.Y))
            End If

            Call UserDañoNpc(UserIndex, NpcIndex)
        Else
            Call _
                SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                         PrepareMessagePlayWave(SND_SWING, UserList(UserIndex).Pos.X, UserList(UserIndex).Pos.Y))
            Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.UserSwing)
        End If

        ' Reveló su condición de usuario al atacar, los npcs lo van a atacar
        UserList(UserIndex).flags.Ignorado = False

        UsuarioAtacaNpc = True

        

        
Catch ex As Exception
    Console.WriteLine("Error in MinimoInt: " & ex.Message)
    Call LogError("Error en UsuarioAtacaNpc. Error " & Err.Number & " : " & Err.Description)
End Try
End Function

    Public Sub UsuarioAtaca(ByVal UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim Index As Short
        Dim AttackPos As WorldPos

        'Check bow's interval
        If Not IntervaloPermiteUsarArcos(UserIndex, False) Then Exit Sub

        'Check Spell-Magic interval
        If Not IntervaloPermiteMagiaGolpe(UserIndex) Then
            'Check Attack interval
            If Not IntervaloPermiteAtacar(UserIndex) Then
                Exit Sub
            End If
        End If

        With UserList(UserIndex)
            'Quitamos stamina
            If .Stats.MinSta >= 10 Then
                Call QuitarSta(UserIndex, RandomNumber(1, 10))
            Else
                If .Genero = Declaraciones.eGenero.Hombre Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Estás muy cansado para luchar.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "Estás muy cansada para luchar.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                End If
                Exit Sub
            End If

            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto AttackPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            AttackPos = .Pos
            Call HeadtoPos(.Char_Renamed.heading, AttackPos)

            'Exit if not legal
            If _
                AttackPos.X < XMinMapSize Or AttackPos.X > XMaxMapSize Or AttackPos.Y <= YMinMapSize Or
                AttackPos.Y > YMaxMapSize Then
                Call _
                    SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                             PrepareMessagePlayWave(SND_SWING, .Pos.X, .Pos.Y))
                Exit Sub
            End If

            Index = MapData(AttackPos.Map, AttackPos.X, AttackPos.Y).UserIndex

            'Look for user
            If Index > 0 Then
                Call UsuarioAtacaUsuario(UserIndex, Index)
                Call WriteUpdateUserStats(UserIndex)
                Call WriteUpdateUserStats(Index)
                Exit Sub
            End If

            Index = MapData(AttackPos.Map, AttackPos.X, AttackPos.Y).NpcIndex

            'Look for NPC
            If Index > 0 Then
                If Npclist(Index).Attackable Then
                    If Npclist(Index).MaestroUser > 0 And MapInfo_Renamed(Npclist(Index).Pos.Map).Pk = False Then
                        Call _
                            WriteConsoleMsg(UserIndex, "No puedes atacar mascotas en zona segura.",
                                            Protocol.FontTypeNames.FONTTYPE_FIGHT)
                        Exit Sub
                    End If

                    Call UsuarioAtacaNpc(UserIndex, Index)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes atacar a este NPC.", Protocol.FontTypeNames.FONTTYPE_FIGHT)
                End If

                Call WriteUpdateUserStats(UserIndex)

                Exit Sub
            End If

            Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessagePlayWave(SND_SWING, .Pos.X, .Pos.Y))
            Call WriteUpdateUserStats(UserIndex)

            If .Counters.Trabajando Then .Counters.Trabajando = .Counters.Trabajando - 1

            If .Counters.Ocultando Then .Counters.Ocultando = .Counters.Ocultando - 1
        End With
    End Sub

    Public Function UsuarioImpacto(ByVal AtacanteIndex As Short, ByVal VictimaIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

        Dim ProbRechazo As Integer
        Dim Rechazo As Boolean
        Dim ProbExito As Integer
        Dim PoderAtaque As Integer
        Dim UserPoderEvasion As Integer
        Dim UserPoderEvasionEscudo As Integer
        Dim Arma As Short
        Dim SkillTacticas As Integer
        Dim SkillDefensa As Integer
        Dim ProbEvadir As Integer
        Dim Skill As Declaraciones.eSkill

        SkillTacticas = UserList(VictimaIndex).Stats.UserSkills(Declaraciones.eSkill.Tacticas)
        SkillDefensa = UserList(VictimaIndex).Stats.UserSkills(Declaraciones.eSkill.Defensa)

        Arma = UserList(AtacanteIndex).Invent.WeaponEqpObjIndex

        'Calculamos el poder de evasion...
        UserPoderEvasion = PoderEvasion(VictimaIndex)

        If UserList(VictimaIndex).Invent.EscudoEqpObjIndex > 0 Then
            UserPoderEvasionEscudo = PoderEvasionEscudo(VictimaIndex)
            UserPoderEvasion = UserPoderEvasion + UserPoderEvasionEscudo
        Else
            UserPoderEvasionEscudo = 0
        End If

        'Esta usando un arma ???
        If UserList(AtacanteIndex).Invent.WeaponEqpObjIndex > 0 Then
            If ObjData_Renamed(Arma).proyectil = 1 Then
                PoderAtaque = PoderAtaqueProyectil(AtacanteIndex)
                Skill = Declaraciones.eSkill.Proyectiles
            Else
                PoderAtaque = PoderAtaqueArma(AtacanteIndex)
                Skill = Declaraciones.eSkill.Armas
            End If
        Else
            PoderAtaque = PoderAtaqueWrestling(AtacanteIndex)
            Skill = Declaraciones.eSkill.Wrestling
        End If

        ' Chances are rounded
        ProbExito = MaximoInt(10, MinimoInt(90, 50 + (PoderAtaque - UserPoderEvasion)*0.4))

        ' Se reduce la evasion un 25%
        If UserList(VictimaIndex).flags.Meditando = True Then
            ProbEvadir = (100 - ProbExito)*0.75
            ProbExito = MinimoInt(90, 100 - ProbEvadir)
        End If

        UsuarioImpacto = (RandomNumber(1, 100) <= ProbExito)

        ' el usuario esta usando un escudo ???
        If UserList(VictimaIndex).Invent.EscudoEqpObjIndex > 0 Then
            'Fallo ???
            If Not UsuarioImpacto Then
                ' Chances are rounded
                ProbRechazo = MaximoInt(10, MinimoInt(90, 100*SkillDefensa/(SkillDefensa + SkillTacticas)))
                Rechazo = (RandomNumber(1, 100) <= ProbRechazo)
                If Rechazo Then
                    'Se rechazo el ataque con el escudo
                    Call _
                        SendData(modSendData.SendTarget.ToPCArea, VictimaIndex,
                                 PrepareMessagePlayWave(SND_ESCUDO, UserList(VictimaIndex).Pos.X,
                                                        UserList(VictimaIndex).Pos.Y))

                    Call WriteMultiMessage(AtacanteIndex, Declaraciones.eMessages.BlockedWithShieldother)
                    Call WriteMultiMessage(VictimaIndex, Declaraciones.eMessages.BlockedWithShieldUser)

                    Call SubirSkill(VictimaIndex, Declaraciones.eSkill.Defensa, True)
                Else
                    Call SubirSkill(VictimaIndex, Declaraciones.eSkill.Defensa, False)
                End If
            End If
        End If

        If Not UsuarioImpacto Then
            Call SubirSkill(AtacanteIndex, Skill, False)
        End If

        Call FlushBuffer(VictimaIndex)

        

        
Catch ex As Exception
    Console.WriteLine("Error in UsuarioAtaca: " & ex.Message)
    Dim AtacanteNick As String
        Dim VictimaNick As String

        If AtacanteIndex > 0 Then AtacanteNick = UserList(AtacanteIndex).name
        If VictimaIndex > 0 Then VictimaNick = UserList(VictimaIndex).name

        Call _
            LogError(
                "Error en UsuarioImpacto. Error " & Err.Number & " : " & Err.Description & " AtacanteIndex: " &
                AtacanteIndex & " Nick: " & AtacanteNick & " VictimaIndex: " & VictimaIndex & " Nick: " & VictimaNick)
End Try
End Function

    Public Function UsuarioAtacaUsuario(ByVal AtacanteIndex As Short, ByVal VictimaIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: 14/01/2010 (ZaMa)
        '14/01/2010: ZaMa - Lo transformo en función, para que no se pierdan municiones al atacar targets
        '                    inválidos, y evitar un doble chequeo innecesario
        '***************************************************

        Try

        If Not PuedeAtacar(AtacanteIndex, VictimaIndex) Then Exit Function

        With UserList(AtacanteIndex)
            If Distancia(.Pos, UserList(VictimaIndex).Pos) > MAXDISTANCIAARCO Then
                Call _
                    WriteConsoleMsg(AtacanteIndex, "Estás muy lejos para disparar.",
                                    Protocol.FontTypeNames.FONTTYPE_FIGHT)
                Exit Function
            End If

            Call UsuarioAtacadoPorUsuario(AtacanteIndex, VictimaIndex)

            If UsuarioImpacto(AtacanteIndex, VictimaIndex) Then
                Call _
                    SendData(modSendData.SendTarget.ToPCArea, AtacanteIndex,
                             PrepareMessagePlayWave(SND_IMPACTO, .Pos.X, .Pos.Y))

                If UserList(VictimaIndex).flags.Navegando = 0 Then
                    Call _
                        SendData(modSendData.SendTarget.ToPCArea, VictimaIndex,
                                 PrepareMessageCreateFX(UserList(VictimaIndex).Char_Renamed.CharIndex, FXSANGRE, 0))
                End If

                'Pablo (ToxicWaste): Guantes de Hurto del Bandido en acción
                If .clase = Declaraciones.eClass.Bandit Then
                    Call DoDesequipar(AtacanteIndex, VictimaIndex)

                    'y ahora, el ladrón puede llegar a paralizar con el golpe.
                ElseIf .clase = Declaraciones.eClass.Thief Then
                    Call DoHandInmo(AtacanteIndex, VictimaIndex)
                End If

                Call SubirSkill(VictimaIndex, Declaraciones.eSkill.Tacticas, False)
                Call UserDañoUser(AtacanteIndex, VictimaIndex)
            Else
                ' Invisible admins doesn't make sound to other clients except itself
                If .flags.AdminInvisible = 1 Then
                    Call EnviarDatosASlot(AtacanteIndex, PrepareMessagePlayWave(SND_SWING, .Pos.X, .Pos.Y))
                Else
                    Call _
                        SendData(modSendData.SendTarget.ToPCArea, AtacanteIndex,
                                 PrepareMessagePlayWave(SND_SWING, .Pos.X, .Pos.Y))
                End If

                Call WriteMultiMessage(AtacanteIndex, Declaraciones.eMessages.UserSwing)
                Call WriteMultiMessage(VictimaIndex, Declaraciones.eMessages.UserAttackedSwing, AtacanteIndex)
                Call SubirSkill(VictimaIndex, Declaraciones.eSkill.Tacticas, True)
            End If

            If .clase = Declaraciones.eClass.Thief Then Call Desarmar(AtacanteIndex, VictimaIndex)
        End With

        UsuarioAtacaUsuario = True

        

        
Catch ex As Exception
    Console.WriteLine("Error in UsuarioAtacaUsuario: " & ex.Message)
    Call LogError("Error en UsuarioAtacaUsuario. Error " & Err.Number & " : " & Err.Description)
End Try
End Function

    Public Sub UserDañoUser(ByVal AtacanteIndex As Short, ByVal VictimaIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 12/01/2010 (ZaMa)
        '12/01/2010: ZaMa - Implemento armas arrojadizas y probabilidad de acuchillar
        '11/03/2010: ZaMa - Ahora no cuenta la muerte si estaba en estado atacable, y no se vuelve criminal
        '***************************************************

        Try

        Dim daño As Integer
        Dim Lugar As Byte
        Dim absorbido As Integer
        Dim defbarco As Short
        'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        'UPGRADE_NOTE: Obj se actualizó a Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim Obj_Renamed As ObjData
        Dim Resist As Byte

        daño = CalcularDaño(AtacanteIndex)

        Call UserEnvenena(AtacanteIndex, VictimaIndex)

        Dim j As Short
        'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj2, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Dim Obj2 As ObjData
        With UserList(AtacanteIndex)
            If .flags.Navegando = 1 And .Invent.BarcoObjIndex > 0 Then
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                Obj_Renamed = ObjData_Renamed(.Invent.BarcoObjIndex)
                daño = daño + RandomNumber(Obj_Renamed.MinHIT, Obj_Renamed.MaxHIT)
            End If

            If UserList(VictimaIndex).flags.Navegando = 1 And UserList(VictimaIndex).Invent.BarcoObjIndex > 0 Then
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                Obj_Renamed = ObjData_Renamed(UserList(VictimaIndex).Invent.BarcoObjIndex)
                defbarco = RandomNumber(Obj_Renamed.MinDef, Obj_Renamed.MaxDef)
            End If

            If .Invent.WeaponEqpObjIndex > 0 Then
                Resist = ObjData_Renamed(.Invent.WeaponEqpObjIndex).Refuerzo
            End If

            Lugar = RandomNumber(Declaraciones.PartesCuerpo.bCabeza, Declaraciones.PartesCuerpo.bTorso)

            Select Case Lugar
                Case Declaraciones.PartesCuerpo.bCabeza
                    'Si tiene casco absorbe el golpe
                    If UserList(VictimaIndex).Invent.CascoEqpObjIndex > 0 Then
                        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Obj_Renamed = ObjData_Renamed(UserList(VictimaIndex).Invent.CascoEqpObjIndex)
                        absorbido = RandomNumber(Obj_Renamed.MinDef, Obj_Renamed.MaxDef)
                        absorbido = absorbido + defbarco - Resist
                        daño = daño - absorbido
                        If daño < 0 Then daño = 1
                    End If

                Case Else
                    'Si tiene armadura absorbe el golpe
                    If UserList(VictimaIndex).Invent.ArmourEqpObjIndex > 0 Then
                        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Obj_Renamed = ObjData_Renamed(UserList(VictimaIndex).Invent.ArmourEqpObjIndex)
                        If UserList(VictimaIndex).Invent.EscudoEqpObjIndex Then
                            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj2. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            Obj2 = ObjData_Renamed(UserList(VictimaIndex).Invent.EscudoEqpObjIndex)
                            absorbido = RandomNumber(Obj_Renamed.MinDef + Obj2.MinDef, Obj_Renamed.MaxDef + Obj2.MaxDef)
                        Else
                            absorbido = RandomNumber(Obj_Renamed.MinDef, Obj_Renamed.MaxDef)
                        End If
                        absorbido = absorbido + defbarco - Resist
                        daño = daño - absorbido
                        If daño < 0 Then daño = 1
                    End If
            End Select

            Call _
                WriteMultiMessage(AtacanteIndex, Declaraciones.eMessages.UserHittedUser,
                                  UserList(VictimaIndex).Char_Renamed.CharIndex, Lugar, daño)
            Call _
                WriteMultiMessage(VictimaIndex, Declaraciones.eMessages.UserHittedByUser, .Char_Renamed.CharIndex, Lugar,
                                  daño)

            UserList(VictimaIndex).Stats.MinHp = UserList(VictimaIndex).Stats.MinHp - daño

            If .flags.Hambre = 0 And .flags.Sed = 0 Then
                'Si usa un arma quizas suba "Combate con armas"
                If .Invent.WeaponEqpObjIndex > 0 Then
                    If ObjData_Renamed(.Invent.WeaponEqpObjIndex).proyectil Then
                        'es un Arco. Sube Armas a Distancia
                        Call SubirSkill(AtacanteIndex, Declaraciones.eSkill.Proyectiles, True)

                        ' Si es arma arrojadiza..
                        If ObjData_Renamed(.Invent.WeaponEqpObjIndex).Municion = 0 Then
                            ' Si acuchilla
                            If ObjData_Renamed(.Invent.WeaponEqpObjIndex).Acuchilla = 1 Then
                                Call DoAcuchillar(AtacanteIndex, 0, VictimaIndex, daño)
                            End If
                        End If
                    Else
                        'Sube combate con armas.
                        Call SubirSkill(AtacanteIndex, Declaraciones.eSkill.Armas, True)
                    End If
                Else
                    'sino tal vez lucha libre
                    Call SubirSkill(AtacanteIndex, Declaraciones.eSkill.Wrestling, True)
                End If

                'Trata de apuñalar por la espalda al enemigo
                If PuedeApuñalar(AtacanteIndex) Then
                    Call DoApuñalar(AtacanteIndex, 0, VictimaIndex, daño)
                End If
                'e intenta dar un golpe crítico [Pablo (ToxicWaste)]
                Call DoGolpeCritico(AtacanteIndex, 0, VictimaIndex, daño)
            End If

            If UserList(VictimaIndex).Stats.MinHp <= 0 Then

                ' No cuenta la muerte si estaba en estado atacable
                If UserList(VictimaIndex).flags.AtacablePor <> AtacanteIndex Then
                    'Store it!
                    Call Statistics.StoreFrag(AtacanteIndex, VictimaIndex)

                    Call ContarMuerte(VictimaIndex, AtacanteIndex)
                End If

                ' Para que las mascotas no sigan intentando luchar y
                ' comiencen a seguir al amo
                For j = 1 To MAXMASCOTAS
                    If .MascotasIndex(j) > 0 Then
                        If Npclist(.MascotasIndex(j)).Target = VictimaIndex Then
                            Npclist(.MascotasIndex(j)).Target = 0
                            Call FollowAmo(.MascotasIndex(j))
                        End If
                    End If
                Next j

                Call ActStats(VictimaIndex, AtacanteIndex)
                Call UserDie(VictimaIndex)
            Else
                'Está vivo - Actualizamos el HP
                Call WriteUpdateHP(VictimaIndex)
            End If
        End With

        'Controla el nivel del usuario
        Call CheckUserLevel(AtacanteIndex)

        Call FlushBuffer(VictimaIndex)

        

        
Catch ex As Exception
    Console.WriteLine("Error in UserDañoUser: " & ex.Message)
    Dim AtacanteNick As String
        Dim VictimaNick As String

        If AtacanteIndex > 0 Then AtacanteNick = UserList(AtacanteIndex).name
        If VictimaIndex > 0 Then VictimaNick = UserList(VictimaIndex).name

        Call _
            LogError(
                "Error en UserDañoUser. Error " & Err.Number & " : " & Err.Description & " AtacanteIndex: " &
                AtacanteIndex & " Nick: " & AtacanteNick & " VictimaIndex: " & VictimaIndex & " Nick: " & VictimaNick)
End Try
End Sub

    Sub UsuarioAtacadoPorUsuario(ByVal AttackerIndex As Short, ByVal VictimIndex As Short)
        '***************************************************
        'Autor: Unknown
        'Last Modification: 05/05/2010
        'Last Modified By: Lucas Tavolaro Ortiz (Tavo)
        '10/01/2008: Tavo - Se cancela la salida del juego si el user esta saliendo
        '05/05/2010: ZaMa - Ahora no suma puntos de bandido al atacar a alguien en estado atacable.
        '***************************************************

        If TriggerZonaPelea(AttackerIndex, VictimIndex) = Declaraciones.eTrigger6.TRIGGER6_PERMITE Then Exit Sub

        Dim EraCriminal As Boolean
        Dim VictimaEsAtacable As Boolean

        If Not criminal(AttackerIndex) Then
            If Not criminal(VictimIndex) Then
                ' Si la victima no es atacable por el agresor, entonces se hace pk
                VictimaEsAtacable = UserList(VictimIndex).flags.AtacablePor = AttackerIndex
                If Not VictimaEsAtacable Then Call VolverCriminal(AttackerIndex)
            End If
        End If

        With UserList(VictimIndex)
            If .flags.Meditando Then
                .flags.Meditando = False
                Call WriteMeditateToggle(VictimIndex)
                Call WriteConsoleMsg(VictimIndex, "Dejas de meditar.", Protocol.FontTypeNames.FONTTYPE_INFO)
                .Char_Renamed.FX = 0
                .Char_Renamed.loops = 0
                Call _
                    SendData(modSendData.SendTarget.ToPCArea, VictimIndex,
                             PrepareMessageCreateFX(.Char_Renamed.CharIndex, 0, 0))
            End If
        End With

        EraCriminal = criminal(AttackerIndex)

        ' Si ataco a un atacable, no suma puntos de bandido
        If Not VictimaEsAtacable Then
            With UserList(AttackerIndex).Reputacion
                If Not criminal(VictimIndex) Then
                    .BandidoRep = .BandidoRep + vlASALTO
                    If .BandidoRep > MAXREP Then .BandidoRep = MAXREP

                    .NobleRep = .NobleRep*0.5
                    If .NobleRep < 0 Then .NobleRep = 0
                Else
                    .NobleRep = .NobleRep + vlNoble
                    If .NobleRep > MAXREP Then .NobleRep = MAXREP
                End If
            End With
        End If

        If criminal(AttackerIndex) Then
            If UserList(AttackerIndex).Faccion.ArmadaReal = 1 Then Call ExpulsarFaccionReal(AttackerIndex)

            If Not EraCriminal Then Call RefreshCharStatus(AttackerIndex)
        ElseIf EraCriminal Then
            Call RefreshCharStatus(AttackerIndex)
        End If

        Call AllMascotasAtacanUser(AttackerIndex, VictimIndex)
        Call AllMascotasAtacanUser(VictimIndex, AttackerIndex)

        'Si la victima esta saliendo se cancela la salida
        Call CancelExit(VictimIndex)
        Call FlushBuffer(VictimIndex)
    End Sub

    Sub AllMascotasAtacanUser(ByVal victim As Short, ByVal Maestro As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************
        'Reaccion de las mascotas
        Dim iCount As Short

        For iCount = 1 To MAXMASCOTAS
            If UserList(Maestro).MascotasIndex(iCount) > 0 Then
                Npclist(UserList(Maestro).MascotasIndex(iCount)).flags.AttackedBy = UserList(victim).name
                Npclist(UserList(Maestro).MascotasIndex(iCount)).Movement = AI.TipoAI.NPCDEFENSA
                Npclist(UserList(Maestro).MascotasIndex(iCount)).Hostile = 1
            End If
        Next iCount
    End Sub

    Public Function PuedeAtacar(ByVal AttackerIndex As Short, ByVal VictimIndex As Short) As Boolean
        '***************************************************
        'Autor: Unknown
        'Last Modification: 02/04/2010
        'Returns true if the AttackerIndex is allowed to attack the VictimIndex.
        '24/01/2007 Pablo (ToxicWaste) - Ordeno todo y agrego situacion de Defensa en ciudad Armada y Caos.
        '24/02/2009: ZaMa - Los usuarios pueden atacarse entre si.
        '02/04/2010: ZaMa - Los armadas no pueden atacar nunca a los ciudas, salvo que esten atacables.
        '***************************************************
        Try

        'MUY importante el orden de estos "IF"...

        'Estas muerto no podes atacar
        If UserList(AttackerIndex).flags.Muerto = 1 Then
            Call WriteConsoleMsg(AttackerIndex, "¡¡Estás muerto!!", Protocol.FontTypeNames.FONTTYPE_INFO)
            PuedeAtacar = False
            Exit Function
        End If

        'No podes atacar a alguien muerto
        If UserList(VictimIndex).flags.Muerto = 1 Then
            Call WriteConsoleMsg(AttackerIndex, "No puedes atacar a un espíritu.", Protocol.FontTypeNames.FONTTYPE_INFO)
            PuedeAtacar = False
            Exit Function
        End If

        ' No podes atacar si estas en consulta
        If UserList(AttackerIndex).flags.EnConsulta Then
            Call _
                WriteConsoleMsg(AttackerIndex, "No puedes atacar usuarios mientras estas en consulta.",
                                Protocol.FontTypeNames.FONTTYPE_INFO)
            Exit Function
        End If

        ' No podes atacar si esta en consulta
        If UserList(VictimIndex).flags.EnConsulta Then
            Call _
                WriteConsoleMsg(AttackerIndex, "No puedes atacar usuarios mientras estan en consulta.",
                                Protocol.FontTypeNames.FONTTYPE_INFO)
            Exit Function
        End If

        'Estamos en una Arena? o un trigger zona segura?
        Select Case TriggerZonaPelea(AttackerIndex, VictimIndex)
            Case Declaraciones.eTrigger6.TRIGGER6_PERMITE
                PuedeAtacar = (UserList(VictimIndex).flags.AdminInvisible = 0)
                Exit Function

            Case Declaraciones.eTrigger6.TRIGGER6_PROHIBE
                PuedeAtacar = False
                Exit Function

            Case Declaraciones.eTrigger6.TRIGGER6_AUSENTE
                'Si no estamos en el Trigger 6 entonces es imposible atacar un gm
                If (UserList(VictimIndex).flags.Privilegios And Declaraciones.PlayerType.User) = 0 Then
                    If UserList(VictimIndex).flags.AdminInvisible = 0 Then _
                        Call _
                            WriteConsoleMsg(AttackerIndex, "El ser es demasiado poderoso.",
                                            Protocol.FontTypeNames.FONTTYPE_WARNING)
                    PuedeAtacar = False
                    Exit Function
                End If
        End Select

        'Ataca un ciudadano?
        If Not criminal(VictimIndex) Then
            ' El atacante es ciuda?
            If Not criminal(AttackerIndex) Then
                ' El atacante es armada?
                If esArmada(AttackerIndex) Then
                    ' La victima es armada?
                    If esArmada(VictimIndex) Then
                        ' No puede
                        Call _
                            WriteConsoleMsg(AttackerIndex,
                                            "Los soldados del ejército real tienen prohibido atacar ciudadanos.",
                                            Protocol.FontTypeNames.FONTTYPE_WARNING)
                        Exit Function
                    End If
                End If

                ' Ciuda (o army) atacando a otro ciuda (o army)
                If UserList(VictimIndex).flags.AtacablePor = AttackerIndex Then
                    ' Se vuelve atacable.
                    If ToogleToAtackable(AttackerIndex, VictimIndex, False) Then
                        PuedeAtacar = True
                        Exit Function
                    End If
                End If
            End If
            ' Ataca a un criminal
        Else
            'Sos un Caos atacando otro caos?
            If esCaos(VictimIndex) Then
                If esCaos(AttackerIndex) Then
                    Call _
                        WriteConsoleMsg(AttackerIndex,
                                        "Los miembros de la legión oscura tienen prohibido atacarse entre sí.",
                                        Protocol.FontTypeNames.FONTTYPE_WARNING)
                    Exit Function
                End If
            End If
        End If

        'Tenes puesto el seguro?
        If UserList(AttackerIndex).flags.Seguro Then
            If Not criminal(VictimIndex) Then
                Call _
                    WriteConsoleMsg(AttackerIndex,
                                    "No puedes atacar ciudadanos, para hacerlo debes desactivar el seguro.",
                                    Protocol.FontTypeNames.FONTTYPE_WARNING)
                PuedeAtacar = False
                Exit Function
            End If
        Else
            ' Un ciuda es atacado
            If Not criminal(VictimIndex) Then
                ' Por un armada sin seguro
                If esArmada(AttackerIndex) Then
                    ' No puede
                    Call _
                        WriteConsoleMsg(AttackerIndex,
                                        "Los soldados del ejército real tienen prohibido atacar ciudadanos.",
                                        Protocol.FontTypeNames.FONTTYPE_WARNING)
                    PuedeAtacar = False
                    Exit Function
                End If
            End If
        End If

        'Estas en un Mapa Seguro?
        If MapInfo_Renamed(UserList(VictimIndex).Pos.Map).Pk = False Then
            If esArmada(AttackerIndex) Then
                If UserList(AttackerIndex).Faccion.RecompensasReal > 11 Then
                    If _
                        UserList(VictimIndex).Pos.Map = 58 Or UserList(VictimIndex).Pos.Map = 59 Or
                        UserList(VictimIndex).Pos.Map = 60 Then
                        Call _
                            WriteConsoleMsg(VictimIndex,
                                            "¡Huye de la ciudad! Estás siendo atacado y no podrás defenderte.",
                                            Protocol.FontTypeNames.FONTTYPE_WARNING)
                        PuedeAtacar = True 'Beneficio de Armadas que atacan en su ciudad.
                        Exit Function
                    End If
                End If
            End If
            If esCaos(AttackerIndex) Then
                If UserList(AttackerIndex).Faccion.RecompensasCaos > 11 Then
                    If UserList(VictimIndex).Pos.Map = 151 Or UserList(VictimIndex).Pos.Map = 156 Then
                        Call _
                            WriteConsoleMsg(VictimIndex,
                                            "¡Huye de la ciudad! Estás siendo atacado y no podrás defenderte.",
                                            Protocol.FontTypeNames.FONTTYPE_WARNING)
                        PuedeAtacar = True 'Beneficio de Caos que atacan en su ciudad.
                        Exit Function
                    End If
                End If
            End If
            Call _
                WriteConsoleMsg(AttackerIndex, "Esta es una zona segura, aquí no puedes atacar a otros usuarios.",
                                Protocol.FontTypeNames.FONTTYPE_WARNING)
            PuedeAtacar = False
            Exit Function
        End If

        'Estas atacando desde un trigger seguro? o tu victima esta en uno asi?
        If _
            MapData(UserList(VictimIndex).Pos.Map, UserList(VictimIndex).Pos.X, UserList(VictimIndex).Pos.Y).trigger =
            Declaraciones.eTrigger.ZONASEGURA Or
            MapData(UserList(AttackerIndex).Pos.Map, UserList(AttackerIndex).Pos.X, UserList(AttackerIndex).Pos.Y).
                trigger = Declaraciones.eTrigger.ZONASEGURA Then
            Call WriteConsoleMsg(AttackerIndex, "No puedes pelear aquí.", Protocol.FontTypeNames.FONTTYPE_WARNING)
            PuedeAtacar = False
            Exit Function
        End If

        PuedeAtacar = True
        

        
Catch ex As Exception
    Console.WriteLine("Error in UsuarioAtacadoPorUsuario: " & ex.Message)
    Call LogError("Error en PuedeAtacar. Error " & Err.Number & " : " & Err.Description)
End Try
End Function

    Public Function PuedeAtacarNPC(ByVal AttackerIndex As Short, ByVal NpcIndex As Short,
                                   Optional ByVal Paraliza As Boolean = False, Optional ByVal IsPet As Boolean = False) _
        As Boolean
        '***************************************************
        'Autor: Unknown Author (Original version)
        'Returns True if AttackerIndex can attack the NpcIndex
        'Last Modification: 16/11/2009
        '24/01/2007 Pablo (ToxicWaste) - Orden y corrección de ataque sobre una mascota y guardias
        '14/08/2007 Pablo (ToxicWaste) - Reescribo y agrego TODOS los casos posibles cosa de usar
        'esta función para todo lo referente a ataque a un NPC. Ya sea Magia, Físico o a Distancia.
        '16/11/2009: ZaMa - Agrego validacion de pertenencia de npc.
        '02/04/2010: ZaMa - Los armadas ya no peuden atacar npcs no hotiles.
        '***************************************************

        Dim OwnerUserIndex As Short

        'Estas muerto?
        If UserList(AttackerIndex).flags.Muerto = 1 Then
            Call WriteConsoleMsg(AttackerIndex, "¡¡Estás muerto!!", Protocol.FontTypeNames.FONTTYPE_INFO)
            Exit Function
        End If

        'Sos consejero?
        If UserList(AttackerIndex).flags.Privilegios And Declaraciones.PlayerType.Consejero Then
            'No pueden atacar NPC los Consejeros.
            Exit Function
        End If

        ' No podes atacar si estas en consulta
        If UserList(AttackerIndex).flags.EnConsulta Then
            Call _
                WriteConsoleMsg(AttackerIndex, "No puedes atacar npcs mientras estas en consulta.",
                                Protocol.FontTypeNames.FONTTYPE_INFO)
            Exit Function
        End If

        'Es una criatura atacable?
        If Npclist(NpcIndex).Attackable = 0 Then
            Call WriteConsoleMsg(AttackerIndex, "No puedes atacar esta criatura.", Protocol.FontTypeNames.FONTTYPE_INFO)
            Exit Function
        End If

        'Es valida la distancia a la cual estamos atacando?
        If Distancia(UserList(AttackerIndex).Pos, Npclist(NpcIndex).Pos) >= MAXDISTANCIAARCO Then
            Call WriteConsoleMsg(AttackerIndex, "Estás muy lejos para disparar.", Protocol.FontTypeNames.FONTTYPE_FIGHT)
            Exit Function
        End If

        'Es una criatura No-Hostil?
        If Npclist(NpcIndex).Hostile = 0 Then
            'Es Guardia del Caos?
            If Npclist(NpcIndex).NPCtype = Declaraciones.eNPCType.Guardiascaos Then
                'Lo quiere atacar un caos?
                If esCaos(AttackerIndex) Then
                    Call _
                        WriteConsoleMsg(AttackerIndex, "No puedes atacar Guardias del Caos siendo de la legión oscura.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                    Exit Function
                End If
                'Es guardia Real?
            ElseIf Npclist(NpcIndex).NPCtype = Declaraciones.eNPCType.GuardiaReal Then
                'Lo quiere atacar un Armada?
                If esArmada(AttackerIndex) Then
                    Call _
                        WriteConsoleMsg(AttackerIndex, "No puedes atacar Guardias Reales siendo del ejército real.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                    Exit Function
                End If
                'Tienes el seguro puesto?
                If UserList(AttackerIndex).flags.Seguro Then
                    Call _
                        WriteConsoleMsg(AttackerIndex, "Para poder atacar Guardias Reales debes quitarte el seguro.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                    Exit Function
                Else
                    Call _
                        WriteConsoleMsg(AttackerIndex, "¡Atacaste un Guardia Real! Eres un criminal.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                    Call VolverCriminal(AttackerIndex)
                    PuedeAtacarNPC = True
                    Exit Function
                End If

                'No era un Guardia, asi que es una criatura No-Hostil común.
                'Para asegurarnos que no sea una Mascota:
            ElseIf Npclist(NpcIndex).MaestroUser = 0 Then
                'Si sos ciudadano tenes que quitar el seguro para atacarla.
                If Not criminal(AttackerIndex) Then

                    ' Si sos armada no podes atacarlo directamente
                    If esArmada(AttackerIndex) Then
                        Call _
                            WriteConsoleMsg(AttackerIndex,
                                            "Los miembros del ejército real no pueden atacar npcs no hostiles.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        Exit Function
                    End If

                    'Sos ciudadano, tenes el seguro puesto?
                    If UserList(AttackerIndex).flags.Seguro Then
                        Call _
                            WriteConsoleMsg(AttackerIndex, "Para atacar a este NPC debes quitarte el seguro.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        Exit Function
                    Else
                        'No tiene seguro puesto. Puede atacar pero es penalizado.
                        Call _
                            WriteConsoleMsg(AttackerIndex,
                                            "Atacaste un NPC no-hostil. Continúa haciéndolo y te podrás convertir en criminal.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        'NicoNZ: Cambio para que al atacar npcs no hostiles no bajen puntos de nobleza
                        Call DisNobAuBan(AttackerIndex, 0, 1000)
                        PuedeAtacarNPC = True
                        Exit Function
                    End If
                End If
            End If
        End If

        'Es el NPC mascota de alguien?
        If Npclist(NpcIndex).MaestroUser > 0 Then
            If Not criminal(Npclist(NpcIndex).MaestroUser) Then

                'Es mascota de un Ciudadano.
                If esArmada(AttackerIndex) Then
                    'El atacante es Armada y esta intentando atacar mascota de un Ciudadano
                    Call _
                        WriteConsoleMsg(AttackerIndex,
                                        "Los miembros del ejército real no pueden atacar mascotas de ciudadanos.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                    Exit Function
                End If

                If Not criminal(AttackerIndex) Then

                    'El atacante es Ciudadano y esta intentando atacar mascota de un Ciudadano.
                    If UserList(AttackerIndex).flags.Seguro Then
                        'El atacante tiene el seguro puesto. No puede atacar.
                        Call _
                            WriteConsoleMsg(AttackerIndex,
                                            "Para atacar mascotas de ciudadanos debes quitarte el seguro.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        Exit Function
                    Else
                        'El atacante no tiene el seguro puesto. Recibe penalización.
                        Call _
                            WriteConsoleMsg(AttackerIndex, "Has atacado la Mascota de un ciudadano. Eres un criminal.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        Call VolverCriminal(AttackerIndex)
                        PuedeAtacarNPC = True
                        Exit Function
                    End If
                Else
                    'El atacante es criminal y quiere atacar un elemental ciuda, pero tiene el seguro puesto (NicoNZ)
                    If UserList(AttackerIndex).flags.Seguro Then
                        Call _
                            WriteConsoleMsg(AttackerIndex,
                                            "Para atacar mascotas de ciudadanos debes quitarte el seguro.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        Exit Function
                    End If
                End If
            Else
                'Es mascota de un Criminal.
                If esCaos(Npclist(NpcIndex).MaestroUser) Then
                    'Es Caos el Dueño.
                    If esCaos(AttackerIndex) Then
                        'Un Caos intenta atacar una criatura de un Caos. No puede atacar.
                        Call _
                            WriteConsoleMsg(AttackerIndex,
                                            "Los miembros de la legión oscura no pueden atacar mascotas de otros legionarios. ",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                        Exit Function
                    End If
                End If
            End If
        End If

        With Npclist(NpcIndex)
            ' El npc le pertenece a alguien?
            OwnerUserIndex = .Owner

            If OwnerUserIndex > 0 Then

                ' Puede atacar a su propia criatura!
                If OwnerUserIndex = AttackerIndex Then
                    PuedeAtacarNPC = True
                    Call IntervaloPerdioNpc(OwnerUserIndex, True) ' Renuevo el timer
                    Exit Function
                End If

                ' Esta compartiendo el npc con el atacante? => Puede atacar!
                If UserList(OwnerUserIndex).flags.ShareNpcWith = AttackerIndex Then
                    PuedeAtacarNPC = True
                    Exit Function
                End If

                ' Si son del mismo clan o party, pueden atacar (No renueva el timer)
                If Not SameClan(OwnerUserIndex, AttackerIndex) And Not SameParty(OwnerUserIndex, AttackerIndex) Then

                    ' Si se le agoto el tiempo
                    If IntervaloPerdioNpc(OwnerUserIndex) Then ' Se lo roba :P
                        Call PerdioNpc(OwnerUserIndex)
                        Call ApropioNpc(AttackerIndex, NpcIndex)
                        PuedeAtacarNPC = True
                        Exit Function

                        ' Si lanzo un hechizo de para o inmo
                    ElseIf Paraliza Then

                        ' Si ya esta paralizado o inmobilizado, no puedo inmobilizarlo de nuevo
                        If .flags.Inmovilizado = 1 Or .flags.Paralizado = 1 Then

                            'TODO_ZAMA: Si dejo esto asi, los pks con seguro peusto van a poder inmobilizar criaturas con dueño
                            ' Si es pk neutral, puede hacer lo que quiera :P.
                            If Not criminal(AttackerIndex) And Not criminal(OwnerUserIndex) Then

                                'El atacante es Armada
                                If esArmada(AttackerIndex) Then

                                    'Intententa paralizar un npc de un armada?
                                    If esArmada(OwnerUserIndex) Then
                                        'El atacante es Armada y esta intentando paralizar un npc de un armada: No puede
                                        Call _
                                            WriteConsoleMsg(AttackerIndex,
                                                            "Los miembros del Ejército Real no pueden paralizar criaturas ya paralizadas pertenecientes a otros miembros del Ejército Real",
                                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                                        Exit Function

                                        'El atacante es Armada y esta intentando paralizar un npc de un ciuda
                                    Else
                                        ' Si tiene seguro no puede
                                        If UserList(AttackerIndex).flags.Seguro Then
                                            Call _
                                                WriteConsoleMsg(AttackerIndex,
                                                                "Para paralizar criaturas ya paralizadas pertenecientes a ciudadanos debes quitarte el seguro.",
                                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                                            Exit Function
                                        Else
                                            ' Si ya estaba atacable, no podrá atacar a un npc perteneciente a otro ciuda
                                            If ToogleToAtackable(AttackerIndex, OwnerUserIndex) Then
                                                Call _
                                                    WriteConsoleMsg(AttackerIndex,
                                                                    "Has paralizado la criatura de un ciudadano, ahora eres atacable por él.",
                                                                    Protocol.FontTypeNames.FONTTYPE_INFO)
                                                PuedeAtacarNPC = True
                                            End If

                                            Exit Function

                                        End If
                                    End If

                                    ' El atacante es ciuda
                                Else
                                    'El atacante tiene el seguro puesto, no puede paralizar
                                    If UserList(AttackerIndex).flags.Seguro Then
                                        Call _
                                            WriteConsoleMsg(AttackerIndex,
                                                            "Para paralizar criaturas ya paralizadas pertenecientes a ciudadanos debes quitarte el seguro.",
                                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                                        Exit Function

                                        'El atacante no tiene el seguro puesto, ataca.
                                    Else
                                        ' Si ya estaba atacable, no podrá atacar a un npc perteneciente a otro ciuda
                                        If ToogleToAtackable(AttackerIndex, OwnerUserIndex) Then
                                            Call _
                                                WriteConsoleMsg(AttackerIndex,
                                                                "Has paralizado la criatura de un ciudadano, ahora eres atacable por él.",
                                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                                            PuedeAtacarNPC = True
                                        End If

                                        Exit Function
                                    End If
                                End If

                                ' Al menos uno de los dos es criminal
                            Else
                                ' Si ambos son caos
                                If esCaos(AttackerIndex) And esCaos(OwnerUserIndex) Then
                                    'El atacante es Caos y esta intentando paralizar un npc de un Caos
                                    Call _
                                        WriteConsoleMsg(AttackerIndex,
                                                        "Los miembros de la legión oscura no pueden paralizar criaturas ya paralizadas por otros legionarios.",
                                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                                    Exit Function
                                End If
                            End If

                            ' El npc no esta inmobilizado ni paralizado
                        Else
                            ' Si no tiene dueño, puede apropiarselo
                            If OwnerUserIndex = 0 Then
                                ' Siempre que no posea uno ya (el inmo/para no cambia pertenencia de npcs).
                                If UserList(AttackerIndex).flags.OwnedNpc = 0 Then
                                    Call ApropioNpc(AttackerIndex, NpcIndex)
                                End If
                            End If

                            ' Siempre se pueden paralizar/inmobilizar npcs con o sin dueño
                            ' que no tengan ese estado
                            PuedeAtacarNPC = True
                            Exit Function

                        End If

                        ' No lanzó hechizos inmobilizantes
                    Else

                        ' El npc le pertenece a un ciudadano
                        If Not criminal(OwnerUserIndex) Then

                            'El atacante es Armada y esta intentando atacar un npc de un Ciudadano
                            If esArmada(AttackerIndex) Then

                                'Intententa atacar un npc de un armada?
                                If esArmada(OwnerUserIndex) Then
                                    'El atacante es Armada y esta intentando atacar el npc de un armada: No puede
                                    Call _
                                        WriteConsoleMsg(AttackerIndex,
                                                        "Los miembros del Ejército Real no pueden atacar criaturas pertenecientes a otros miembros del Ejército Real",
                                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                                    Exit Function

                                    'El atacante es Armada y esta intentando atacar un npc de un ciuda
                                Else

                                    ' Si tiene seguro no puede
                                    If UserList(AttackerIndex).flags.Seguro Then
                                        Call _
                                            WriteConsoleMsg(AttackerIndex,
                                                            "Para atacar criaturas ya pertenecientes a ciudadanos debes quitarte el seguro.",
                                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                                        Exit Function
                                    Else
                                        ' Si ya estaba atacable, no podrá atacar a un npc perteneciente a otro ciuda
                                        If ToogleToAtackable(AttackerIndex, OwnerUserIndex) Then
                                            Call _
                                                WriteConsoleMsg(AttackerIndex,
                                                                "Has atacado a la criatura de un ciudadano, ahora eres atacable por él.",
                                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                                            PuedeAtacarNPC = True
                                        End If

                                        Exit Function
                                    End If
                                End If

                                ' No es aramda, puede ser criminal o ciuda
                            Else

                                'El atacante es Ciudadano y esta intentando atacar un npc de un Ciudadano.
                                If Not criminal(AttackerIndex) Then

                                    If UserList(AttackerIndex).flags.Seguro Then
                                        'El atacante tiene el seguro puesto. No puede atacar.
                                        Call _
                                            WriteConsoleMsg(AttackerIndex,
                                                            "Para atacar criaturas pertenecientes a ciudadanos debes quitarte el seguro.",
                                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                                        Exit Function

                                        'El atacante no tiene el seguro puesto, ataca.
                                    Else
                                        If ToogleToAtackable(AttackerIndex, OwnerUserIndex) Then
                                            Call _
                                                WriteConsoleMsg(AttackerIndex,
                                                                "Has atacado a la criatura de un ciudadano, ahora eres atacable por él.",
                                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                                            PuedeAtacarNPC = True
                                        End If

                                        Exit Function
                                    End If

                                    'El atacante es criminal y esta intentando atacar un npc de un Ciudadano.
                                Else
                                    ' Es criminal atacando un npc de un ciuda, con seguro puesto.
                                    If UserList(AttackerIndex).flags.Seguro Then
                                        Call _
                                            WriteConsoleMsg(AttackerIndex,
                                                            "Para atacar criaturas pertenecientes a ciudadanos debes quitarte el seguro.",
                                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                                        Exit Function
                                    End If

                                    PuedeAtacarNPC = True
                                End If
                            End If

                            ' Es npc de un criminal
                        Else
                            If esCaos(OwnerUserIndex) Then
                                'Es Caos el Dueño.
                                If esCaos(AttackerIndex) Then
                                    'Un Caos intenta atacar una npc de un Caos. No puede atacar.
                                    Call _
                                        WriteConsoleMsg(AttackerIndex,
                                                        "Los miembros de la Legión Oscura no pueden atacar criaturas de otros legionarios. ",
                                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                                    Exit Function
                                End If
                            End If
                        End If
                    End If
                End If

                ' Si no tiene dueño el npc, se lo apropia
            Else
                ' Solo pueden apropiarse de npcs los caos, armadas o ciudas.
                If Not criminal(AttackerIndex) Or esCaos(AttackerIndex) Then
                    ' No puede apropiarse de los pretos!
                    If Not (esPretoriano(NpcIndex) <> 0) Then
                        ' Si es una mascota atacando, no se apropia del npc
                        If Not IsPet Then
                            ' No es dueño de ningun npc => Se lo apropia.
                            If UserList(AttackerIndex).flags.OwnedNpc = 0 Then
                                Call ApropioNpc(AttackerIndex, NpcIndex)
                                ' Es dueño de un npc, pero no puede ser de este porque no tiene propietario.
                            Else
                                ' Se va a adueñar del npc (y perder el otro) solo si no inmobiliza/paraliza
                                If Not Paraliza Then Call ApropioNpc(AttackerIndex, NpcIndex)
                            End If
                        End If
                    End If
                End If
            End If
        End With

        'Es el Rey Preatoriano?
        If esPretoriano(NpcIndex) = 4 Then
            If pretorianosVivos > 0 Then
                Call _
                    WriteConsoleMsg(AttackerIndex, "Debes matar al resto del ejército antes de atacar al rey.",
                                    Protocol.FontTypeNames.FONTTYPE_FIGHT)
                Exit Function
            End If
        End If

        PuedeAtacarNPC = True
    End Function

    Private Function SameClan(ByVal UserIndex As Short, ByVal OtherUserIndex As Short) As Boolean
        '***************************************************
        'Autor: ZaMa
        'Returns True if both players belong to the same clan.
        'Last Modification: 16/11/2009
        '***************************************************
        SameClan = (UserList(UserIndex).GuildIndex = UserList(OtherUserIndex).GuildIndex) And
                   UserList(UserIndex).GuildIndex <> 0
    End Function

    Private Function SameParty(ByVal UserIndex As Short, ByVal OtherUserIndex As Short) As Boolean
        '***************************************************
        'Autor: ZaMa
        'Returns True if both players belong to the same party.
        'Last Modification: 16/11/2009
        '***************************************************
        SameParty = UserList(UserIndex).PartyIndex = UserList(OtherUserIndex).PartyIndex And
                    UserList(UserIndex).PartyIndex <> 0
    End Function

    Sub CalcularDarExp(ByVal UserIndex As Short, ByVal NpcIndex As Short, ByVal ElDaño As Integer)
        '***************************************************
        'Autor: Nacho (Integer)
        'Last Modification: 03/09/06 Nacho
        'Reescribi gran parte del Sub
        'Ahora, da toda la experiencia del npc mientras este vivo.
        '***************************************************
        Dim ExpaDar As Integer

        '[Nacho] Chekeamos que las variables sean validas para las operaciones
        If ElDaño <= 0 Then ElDaño = 0
        If Npclist(NpcIndex).Stats.MaxHp <= 0 Then Exit Sub
        If ElDaño > Npclist(NpcIndex).Stats.MinHp Then ElDaño = Npclist(NpcIndex).Stats.MinHp

        '[Nacho] La experiencia a dar es la porcion de vida quitada * toda la experiencia
        ExpaDar = CInt(ElDaño*(Npclist(NpcIndex).GiveEXP/Npclist(NpcIndex).Stats.MaxHp))
        If ExpaDar <= 0 Then Exit Sub

        '[Nacho] Vamos contando cuanta experiencia sacamos, porque se da toda la que no se dio al user que mata al NPC
        'Esto es porque cuando un elemental ataca, no se da exp, y tambien porque la cuenta que hicimos antes
        'Podria dar un numero fraccionario, esas fracciones se acumulan hasta formar enteros ;P
        If ExpaDar > Npclist(NpcIndex).flags.ExpCount Then
            ExpaDar = Npclist(NpcIndex).flags.ExpCount
            Npclist(NpcIndex).flags.ExpCount = 0
        Else
            Npclist(NpcIndex).flags.ExpCount = Npclist(NpcIndex).flags.ExpCount - ExpaDar
        End If

        '[Nacho] Le damos la exp al user
        If ExpaDar > 0 Then
            If UserList(UserIndex).PartyIndex > 0 Then
                Call _
                    mdParty.ObtenerExito(UserIndex, ExpaDar, Npclist(NpcIndex).Pos.Map, Npclist(NpcIndex).Pos.X,
                                         Npclist(NpcIndex).Pos.Y)
            Else
                UserList(UserIndex).Stats.Exp = UserList(UserIndex).Stats.Exp + ExpaDar
                If UserList(UserIndex).Stats.Exp > MAXEXP Then UserList(UserIndex).Stats.Exp = MAXEXP
                Call _
                    WriteConsoleMsg(UserIndex, "Has ganado " & ExpaDar & " puntos de experiencia.",
                                    Protocol.FontTypeNames.FONTTYPE_FIGHT)
            End If

            Call CheckUserLevel(UserIndex)
        End If
    End Sub

    Public Function TriggerZonaPelea(ByVal Origen As Short, ByVal Destino As Short) As Declaraciones.eTrigger6
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'TODO: Pero que rebuscado!!
        'Nigo:  Te lo rediseñe, pero no te borro el TODO para que lo revises.
        Try
        Dim tOrg As Declaraciones.eTrigger
        Dim tDst As Declaraciones.eTrigger

        tOrg = MapData(UserList(Origen).Pos.Map, UserList(Origen).Pos.X, UserList(Origen).Pos.Y).trigger
        tDst = MapData(UserList(Destino).Pos.Map, UserList(Destino).Pos.X, UserList(Destino).Pos.Y).trigger

        If tOrg = Declaraciones.eTrigger.ZONAPELEA Or tDst = Declaraciones.eTrigger.ZONAPELEA Then
            If tOrg = tDst Then
                TriggerZonaPelea = Declaraciones.eTrigger6.TRIGGER6_PERMITE
            Else
                TriggerZonaPelea = Declaraciones.eTrigger6.TRIGGER6_PROHIBE
            End If
        Else
            TriggerZonaPelea = Declaraciones.eTrigger6.TRIGGER6_AUSENTE
        End If

        
        
Catch ex As Exception
    Console.WriteLine("Error in PuedeAtacarNPC: " & ex.Message)
    TriggerZonaPelea = Declaraciones.eTrigger6.TRIGGER6_AUSENTE
        LogError(("Error en TriggerZonaPelea - " & Err.Description))
End Try
End Function

    Sub UserEnvenena(ByVal AtacanteIndex As Short, ByVal VictimaIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim ObjInd As Short

        ObjInd = UserList(AtacanteIndex).Invent.WeaponEqpObjIndex

        If ObjInd > 0 Then
            If ObjData_Renamed(ObjInd).proyectil = 1 Then
                ObjInd = UserList(AtacanteIndex).Invent.MunicionEqpObjIndex
            End If

            If ObjInd > 0 Then
                If ObjData_Renamed(ObjInd).Envenena = 1 Then

                    If RandomNumber(1, 100) < 60 Then
                        UserList(VictimaIndex).flags.Envenenado = 1
                        Call _
                            WriteConsoleMsg(VictimaIndex, "¡¡" & UserList(AtacanteIndex).name & " te ha envenenado!!",
                                            Protocol.FontTypeNames.FONTTYPE_FIGHT)
                        Call _
                            WriteConsoleMsg(AtacanteIndex, "¡¡Has envenenado a " & UserList(VictimaIndex).name & "!!",
                                            Protocol.FontTypeNames.FONTTYPE_FIGHT)
                    End If
                End If
            End If
        End If

        Call FlushBuffer(VictimaIndex)
    End Sub
End Module
