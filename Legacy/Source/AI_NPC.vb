Option Strict Off
Option Explicit On
Module AI
    Public Enum TipoAI
        ESTATICO = 1
        MueveAlAzar = 2
        NpcMaloAtacaUsersBuenos = 3
        NPCDEFENSA = 4
        GuardiasAtacanCriminales = 5
        NpcObjeto = 6
        SigueAmo = 8
        NpcAtacaNpc = 9
        NpcPathfinding = 10
        'Pretorianos
        SacerdotePretorianoAi = 20
        GuerreroPretorianoAi = 21
        MagoPretorianoAi = 22
        CazadorPretorianoAi = 23
        ReyPretoriano = 24
    End Enum

    Public Const ELEMENTALFUEGO As Short = 93
    Public Const ELEMENTALTIERRA As Short = 94
    Public Const ELEMENTALAGUA As Short = 92

    'Damos a los NPCs el mismo rango de visiï¿½n que un PJ
    Public Const RANGO_VISION_X As Byte = 8
    Public Const RANGO_VISION_Y As Byte = 6

    '?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
    '?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
    '?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
    '                        Modulo AI_NPC
    '?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
    '?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
    '?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
    'AI de los NPC
    '?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
    '?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½
    '?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½?ï¿½

    Private Sub GuardiasAI(NpcIndex As Short, DelCaos As Boolean)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 12/01/2010 (ZaMa)
        '14/09/2009: ZaMa - Now npcs don't atack protected users.
        '12/01/2010: ZaMa - Los npcs no atacan druidas mimetizados con npcs
        '***************************************************
        Dim nPos As WorldPos
        Dim headingloop As Byte
        Dim UI As Short
        Dim UserProtected As Boolean

        With Npclist(NpcIndex)
            For headingloop = eHeading.NORTH To eHeading.WEST
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                nPos = .Pos
                If .flags.Inmovilizado = 0 Or headingloop = .Char_Renamed.heading Then
                    Call HeadtoPos(headingloop, nPos)
                    If InMapBounds(nPos.Map, nPos.X, nPos.Y) Then
                        UI = MapData(nPos.Map, nPos.X, nPos.Y).UserIndex
                        If UI > 0 Then
                            UserProtected = Not IntervaloPermiteSerAtacado(UI) And UserList(UI).flags.NoPuedeSerAtacado
                            UserProtected = UserProtected Or UserList(UI).flags.Ignorado Or
                                            UserList(UI).flags.EnConsulta

                            If _
                                UserList(UI).flags.Muerto = 0 And UserList(UI).flags.AdminPerseguible And
                                Not UserProtected Then
                                'ï¿½ES CRIMINAL?
                                If Not DelCaos Then
                                    If criminal(UI) Then
                                        If NpcAtacaUser(NpcIndex, UI) Then
                                            Call _
                                                ChangeNPCChar(NpcIndex, .Char_Renamed.body, .Char_Renamed.Head,
                                                              headingloop)
                                        End If
                                        Exit Sub
                                    ElseIf .flags.AttackedBy = UserList(UI).name And Not .flags.Follow Then

                                        If NpcAtacaUser(NpcIndex, UI) Then
                                            Call _
                                                ChangeNPCChar(NpcIndex, .Char_Renamed.body, .Char_Renamed.Head,
                                                              headingloop)
                                        End If
                                        Exit Sub
                                    End If
                                Else
                                    If Not criminal(UI) Then
                                        If NpcAtacaUser(NpcIndex, UI) Then
                                            Call _
                                                ChangeNPCChar(NpcIndex, .Char_Renamed.body, .Char_Renamed.Head,
                                                              headingloop)
                                        End If
                                        Exit Sub
                                    ElseIf .flags.AttackedBy = UserList(UI).name And Not .flags.Follow Then

                                        If NpcAtacaUser(NpcIndex, UI) Then
                                            Call _
                                                ChangeNPCChar(NpcIndex, .Char_Renamed.body, .Char_Renamed.Head,
                                                              headingloop)
                                        End If
                                        Exit Sub
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If 'not inmovil
            Next headingloop
        End With

        Call RestoreOldMovement(NpcIndex)
    End Sub

    ''
    ' Handles the evil npcs' artificial intelligency.
    '
    ' @param NpcIndex Specifies reference to the npc
    Private Sub HostilMalvadoAI(NpcIndex As Short)
        '**************************************************************
        'Author: Unknown
        'Last Modify Date: 12/01/2010 (ZaMa)
        '28/04/2009: ZaMa - Now those NPCs who doble attack, have 50% of posibility of casting a spell on user.
        '14/09/200*: ZaMa - Now npcs don't atack protected users.
        '12/01/2010: ZaMa - Los npcs no atacan druidas mimetizados con npcs
        '**************************************************************
        Dim nPos As WorldPos
        Dim headingloop As Byte
        Dim UI As Short
        Dim NPCI As Short
        Dim atacoPJ As Boolean
        Dim UserProtected As Boolean

        atacoPJ = False

        With Npclist(NpcIndex)
            For headingloop = eHeading.NORTH To eHeading.WEST
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                nPos = .Pos
                If .flags.Inmovilizado = 0 Or .Char_Renamed.heading = headingloop Then
                    Call HeadtoPos(headingloop, nPos)
                    If InMapBounds(nPos.Map, nPos.X, nPos.Y) Then
                        UI = MapData(nPos.Map, nPos.X, nPos.Y).UserIndex
                        NPCI = MapData(nPos.Map, nPos.X, nPos.Y).NpcIndex
                        If UI > 0 And Not atacoPJ Then
                            UserProtected = Not IntervaloPermiteSerAtacado(UI) And UserList(UI).flags.NoPuedeSerAtacado
                            UserProtected = UserProtected Or UserList(UI).flags.Ignorado Or
                                            UserList(UI).flags.EnConsulta

                            If _
                                UserList(UI).flags.Muerto = 0 And UserList(UI).flags.AdminPerseguible And
                                (Not UserProtected) Then

                                atacoPJ = True
                                If .Movement = TipoAI.NpcObjeto Then
                                    ' Los npc objeto no atacan siempre al mismo usuario
                                    If RandomNumber(1, 3) = 3 Then atacoPJ = False
                                End If

                                If atacoPJ Then
                                    If .flags.LanzaSpells Then
                                        If .flags.AtacaDoble Then
                                            If (RandomNumber(0, 1)) Then
                                                If NpcAtacaUser(NpcIndex, UI) Then
                                                    Call _
                                                        ChangeNPCChar(NpcIndex, .Char_Renamed.body, .Char_Renamed.Head,
                                                                      headingloop)
                                                End If
                                                Exit Sub
                                            End If
                                        End If

                                        Call _
                                            ChangeNPCChar(NpcIndex, .Char_Renamed.body, .Char_Renamed.Head, headingloop)
                                        Call NpcLanzaUnSpell(NpcIndex, UI)
                                    End If
                                End If
                                If NpcAtacaUser(NpcIndex, UI) Then
                                    Call ChangeNPCChar(NpcIndex, .Char_Renamed.body, .Char_Renamed.Head, headingloop)
                                End If
                                Exit Sub

                            End If
                        ElseIf NPCI > 0 Then
                            If Npclist(NPCI).MaestroUser > 0 And Npclist(NPCI).flags.Paralizado = 0 Then
                                Call ChangeNPCChar(NpcIndex, .Char_Renamed.body, .Char_Renamed.Head, headingloop)
                                Call NpcAtacaNpc(NpcIndex, NPCI, False)
                                Exit Sub
                            End If
                        End If
                    End If
                End If 'inmo
            Next headingloop
        End With

        Call RestoreOldMovement(NpcIndex)
    End Sub

    Private Sub HostilBuenoAI(NpcIndex As Short)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 12/01/2010 (ZaMa)
        '14/09/2009: ZaMa - Now npcs don't atack protected users.
        '12/01/2010: ZaMa - Los npcs no atacan druidas mimetizados con npcs
        '***************************************************
        Dim nPos As WorldPos
        Dim headingloop As eHeading
        Dim UI As Short
        Dim UserProtected As Boolean

        With Npclist(NpcIndex)
            For headingloop = eHeading.NORTH To eHeading.WEST
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                nPos = .Pos
                If .flags.Inmovilizado = 0 Or .Char_Renamed.heading = headingloop Then
                    Call HeadtoPos(headingloop, nPos)
                    If InMapBounds(nPos.Map, nPos.X, nPos.Y) Then
                        UI = MapData(nPos.Map, nPos.X, nPos.Y).UserIndex
                        If UI > 0 Then
                            If UserList(UI).name = .flags.AttackedBy Then

                                UserProtected = Not IntervaloPermiteSerAtacado(UI) And
                                                UserList(UI).flags.NoPuedeSerAtacado
                                UserProtected = UserProtected Or UserList(UI).flags.Ignorado Or
                                                UserList(UI).flags.EnConsulta

                                If _
                                    UserList(UI).flags.Muerto = 0 And UserList(UI).flags.AdminPerseguible And
                                    Not UserProtected Then
                                    If .flags.LanzaSpells > 0 Then
                                        Call NpcLanzaUnSpell(NpcIndex, UI)
                                    End If

                                    If NpcAtacaUser(NpcIndex, UI) Then
                                        Call _
                                            ChangeNPCChar(NpcIndex, .Char_Renamed.body, .Char_Renamed.Head, headingloop)
                                    End If
                                    Exit Sub
                                End If
                            End If
                        End If
                    End If
                End If
            Next headingloop
        End With

        Call RestoreOldMovement(NpcIndex)
    End Sub

    Private Sub IrUsuarioCercano(NpcIndex As Short)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 12/01/2010 (ZaMa)
        '14/09/2009: ZaMa - Now npcs don't follow protected users.
        '12/01/2010: ZaMa - Los npcs no atacan druidas mimetizados con npcs
        '***************************************************
        Dim tHeading As Byte
        Dim UserIndex As Short
        Dim SignoNS As Short
        Dim SignoEO As Short
        Dim i As Integer
        Dim UserProtected As Boolean

        Dim OwnerIndex As Short
        With Npclist(NpcIndex)
            If .flags.Inmovilizado = 1 Then
                Select Case .Char_Renamed.heading
                    Case eHeading.NORTH
                        SignoNS = - 1
                        SignoEO = 0

                    Case eHeading.EAST
                        SignoNS = 0
                        SignoEO = 1

                    Case eHeading.SOUTH
                        SignoNS = 1
                        SignoEO = 0

                    Case eHeading.WEST
                        SignoEO = - 1
                        SignoNS = 0
                End Select

                For i = 1 To ConnGroups(.Pos.Map).CountEntrys
                    UserIndex = ConnGroups(.Pos.Map).UserEntrys(i)

                    'Is it in it's range of vision??
                    If _
                        Math.Abs(UserList(UserIndex).Pos.X - .Pos.X) <= RANGO_VISION_X And
                        Math.Sign(UserList(UserIndex).Pos.X - .Pos.X) = SignoEO Then
                        If _
                            Math.Abs(UserList(UserIndex).Pos.Y - .Pos.Y) <= RANGO_VISION_Y And
                            Math.Sign(UserList(UserIndex).Pos.Y - .Pos.Y) = SignoNS Then

                            UserProtected = Not IntervaloPermiteSerAtacado(UserIndex) And
                                            UserList(UserIndex).flags.NoPuedeSerAtacado
                            UserProtected = UserProtected Or UserList(UserIndex).flags.Ignorado Or
                                            UserList(UserIndex).flags.EnConsulta

                            If UserList(UserIndex).flags.Muerto = 0 Then
                                If Not UserProtected Then
                                    If .flags.LanzaSpells <> 0 Then Call NpcLanzaUnSpell(NpcIndex, UserIndex)
                                    Exit Sub
                                End If
                            End If

                        End If
                    End If
                Next i

                ' No esta inmobilizado
            Else

                ' Tiene prioridad de seguir al usuario al que le pertenece si esta en el rango de vision

                OwnerIndex = .Owner
                If OwnerIndex > 0 Then

                    'Is it in it's range of vision??
                    If Math.Abs(UserList(OwnerIndex).Pos.X - .Pos.X) <= RANGO_VISION_X Then
                        If Math.Abs(UserList(OwnerIndex).Pos.Y - .Pos.Y) <= RANGO_VISION_Y Then

                            ' va hacia el si o esta invi ni oculto
                            If _
                                UserList(OwnerIndex).flags.invisible = 0 And UserList(OwnerIndex).flags.Oculto = 0 And
                                Not UserList(OwnerIndex).flags.EnConsulta And Not UserList(OwnerIndex).flags.Ignorado _
                                Then
                                If .flags.LanzaSpells <> 0 Then Call NpcLanzaUnSpell(NpcIndex, OwnerIndex)

                                tHeading = FindDirection(.Pos, UserList(OwnerIndex).Pos)
                                Call MoveNPCChar(NpcIndex, tHeading)
                                Exit Sub
                            End If
                        End If
                    End If

                End If

                ' No le pertenece a nadie o el dueño no esta en el rango de vision, sigue a cualquiera
                For i = 1 To ConnGroups(.Pos.Map).CountEntrys
                    UserIndex = ConnGroups(.Pos.Map).UserEntrys(i)

                    'Is it in it's range of vision??
                    If Math.Abs(UserList(UserIndex).Pos.X - .Pos.X) <= RANGO_VISION_X Then
                        If Math.Abs(UserList(UserIndex).Pos.Y - .Pos.Y) <= RANGO_VISION_Y Then

                            With UserList(UserIndex)

                                UserProtected = Not IntervaloPermiteSerAtacado(UserIndex) And .flags.NoPuedeSerAtacado
                                UserProtected = UserProtected Or .flags.Ignorado Or .flags.EnConsulta

                                If _
                                    .flags.Muerto = 0 And .flags.invisible = 0 And .flags.Oculto = 0 And
                                    .flags.AdminPerseguible And Not UserProtected Then

                                    If Npclist(NpcIndex).flags.LanzaSpells <> 0 Then _
                                        Call NpcLanzaUnSpell(NpcIndex, UserIndex)

                                    tHeading = FindDirection(Npclist(NpcIndex).Pos, .Pos)
                                    Call MoveNPCChar(NpcIndex, tHeading)
                                    Exit Sub
                                End If

                            End With

                        End If
                    End If
                Next i

                'Si llega aca es que no habï¿½a ningï¿½n usuario cercano vivo.
                'A bailar. Pablo (ToxicWaste)
                If RandomNumber(0, 10) = 0 Then
                    Call _
                        MoveNPCChar(NpcIndex,
                                    CByte(RandomNumber(eHeading.NORTH, eHeading.WEST)))
                End If

            End If
        End With

        Call RestoreOldMovement(NpcIndex)
    End Sub

    ''
    ' Makes a Pet / Summoned Npc to Follow an enemy
    '
    ' @param NpcIndex Specifies reference to the npc
    Private Sub SeguirAgresor(NpcIndex As Short)
        '**************************************************************
        'Author: Unknown
        'Last Modify by: Marco Vanotti (MarKoxX)
        'Last Modify Date: 08/16/2008
        '08/16/2008: MarKoxX - Now pets that do melï¿½ attacks have to be near the enemy to attack.
        '**************************************************************
        Dim tHeading As Byte
        Dim UI As Short

        Dim i As Integer

        Dim SignoNS As Short
        Dim SignoEO As Short

        With Npclist(NpcIndex)
            If .flags.Paralizado = 1 Or .flags.Inmovilizado = 1 Then
                Select Case .Char_Renamed.heading
                    Case eHeading.NORTH
                        SignoNS = - 1
                        SignoEO = 0

                    Case eHeading.EAST
                        SignoNS = 0
                        SignoEO = 1

                    Case eHeading.SOUTH
                        SignoNS = 1
                        SignoEO = 0

                    Case eHeading.WEST
                        SignoEO = - 1
                        SignoNS = 0
                End Select

                For i = 1 To ConnGroups(.Pos.Map).CountEntrys
                    UI = ConnGroups(.Pos.Map).UserEntrys(i)

                    'Is it in it's range of vision??
                    If _
                        Math.Abs(UserList(UI).Pos.X - .Pos.X) <= RANGO_VISION_X And
                        Math.Sign(UserList(UI).Pos.X - .Pos.X) = SignoEO Then
                        If _
                            Math.Abs(UserList(UI).Pos.Y - .Pos.Y) <= RANGO_VISION_Y And
                            Math.Sign(UserList(UI).Pos.Y - .Pos.Y) = SignoNS Then

                            If UserList(UI).name = .flags.AttackedBy Then
                                If .MaestroUser > 0 Then
                                    If _
                                        Not criminal(.MaestroUser) And Not criminal(UI) And
                                        (UserList(.MaestroUser).flags.Seguro Or
                                         UserList(.MaestroUser).Faccion.ArmadaReal = 1) Then
                                        Call _
                                            WriteConsoleMsg(.MaestroUser,
                                                            "La mascota no atacará a ciudadanos si eres miembro del ejército real o tienes el seguro activado.",
                                                            FontTypeNames.FONTTYPE_INFO)
                                        Call FlushBuffer(.MaestroUser)
                                        .flags.AttackedBy = vbNullString
                                        Exit Sub
                                    End If
                                End If

                                If _
                                    UserList(UI).flags.Muerto = 0 And UserList(UI).flags.invisible = 0 And
                                    UserList(UI).flags.Oculto = 0 Then
                                    If .flags.LanzaSpells > 0 Then
                                        Call NpcLanzaUnSpell(NpcIndex, UI)
                                    Else
                                        If Distancia(UserList(UI).Pos, Npclist(NpcIndex).Pos) <= 1 Then
                                            ' TODO : Set this a separate AI for Elementals and Druid's pets
                                            If Npclist(NpcIndex).Numero <> 92 Then
                                                Call NpcAtacaUser(NpcIndex, UI)
                                            End If
                                        End If
                                    End If
                                    Exit Sub
                                End If
                            End If

                        End If
                    End If

                Next i
            Else
                For i = 1 To ConnGroups(.Pos.Map).CountEntrys
                    UI = ConnGroups(.Pos.Map).UserEntrys(i)

                    'Is it in it's range of vision??
                    If Math.Abs(UserList(UI).Pos.X - .Pos.X) <= RANGO_VISION_X Then
                        If Math.Abs(UserList(UI).Pos.Y - .Pos.Y) <= RANGO_VISION_Y Then

                            If UserList(UI).name = .flags.AttackedBy Then
                                If .MaestroUser > 0 Then
                                    If _
                                        Not criminal(.MaestroUser) And Not criminal(UI) And
                                        (UserList(.MaestroUser).flags.Seguro Or
                                         UserList(.MaestroUser).Faccion.ArmadaReal = 1) Then
                                        Call _
                                            WriteConsoleMsg(.MaestroUser,
                                                            "La mascota no atacará a ciudadanos si eres miembro del ejército real o tienes el seguro activado.",
                                                            FontTypeNames.FONTTYPE_INFO)
                                        Call FlushBuffer(.MaestroUser)
                                        .flags.AttackedBy = vbNullString
                                        Call FollowAmo(NpcIndex)
                                        Exit Sub
                                    End If
                                End If

                                If _
                                    UserList(UI).flags.Muerto = 0 And UserList(UI).flags.invisible = 0 And
                                    UserList(UI).flags.Oculto = 0 Then
                                    If .flags.LanzaSpells > 0 Then
                                        Call NpcLanzaUnSpell(NpcIndex, UI)
                                    Else
                                        If Distancia(UserList(UI).Pos, Npclist(NpcIndex).Pos) <= 1 Then
                                            ' TODO : Set this a separate AI for Elementals and Druid's pets
                                            If Npclist(NpcIndex).Numero <> 92 Then
                                                Call NpcAtacaUser(NpcIndex, UI)
                                            End If
                                        End If
                                    End If

                                    tHeading = FindDirection(.Pos, UserList(UI).Pos)
                                    Call MoveNPCChar(NpcIndex, tHeading)

                                    Exit Sub
                                End If
                            End If

                        End If
                    End If

                Next i
            End If
        End With

        Call RestoreOldMovement(NpcIndex)
    End Sub

    Private Sub RestoreOldMovement(NpcIndex As Short)
        With Npclist(NpcIndex)
            If .MaestroUser = 0 Then
                .Movement = .flags.OldMovement
                .Hostile = .flags.OldHostil
                .flags.AttackedBy = vbNullString
            End If
        End With
    End Sub

    Private Sub PersigueCiudadano(NpcIndex As Short)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 12/01/2010 (ZaMa)
        '14/09/2009: ZaMa - Now npcs don't follow protected users.
        '12/01/2010: ZaMa - Los npcs no atacan druidas mimetizados con npcs.
        '***************************************************
        Dim UserIndex As Short
        Dim tHeading As Byte
        Dim i As Integer
        Dim UserProtected As Boolean

        With Npclist(NpcIndex)
            For i = 1 To ConnGroups(.Pos.Map).CountEntrys
                UserIndex = ConnGroups(.Pos.Map).UserEntrys(i)

                'Is it in it's range of vision??
                If Math.Abs(UserList(UserIndex).Pos.X - .Pos.X) <= RANGO_VISION_X Then
                    If Math.Abs(UserList(UserIndex).Pos.Y - .Pos.Y) <= RANGO_VISION_Y Then

                        If Not criminal(UserIndex) Then

                            UserProtected = Not IntervaloPermiteSerAtacado(UserIndex) And
                                            UserList(UserIndex).flags.NoPuedeSerAtacado
                            UserProtected = UserProtected Or UserList(UserIndex).flags.Ignorado Or
                                            UserList(UserIndex).flags.EnConsulta

                            If _
                                UserList(UserIndex).flags.Muerto = 0 And UserList(UserIndex).flags.invisible = 0 And
                                UserList(UserIndex).flags.Oculto = 0 And UserList(UserIndex).flags.AdminPerseguible And
                                Not UserProtected Then

                                If .flags.LanzaSpells > 0 Then
                                    Call NpcLanzaUnSpell(NpcIndex, UserIndex)
                                End If
                                tHeading = FindDirection(.Pos, UserList(UserIndex).Pos)
                                Call MoveNPCChar(NpcIndex, tHeading)
                                Exit Sub
                            End If
                        End If

                    End If
                End If

            Next i
        End With

        Call RestoreOldMovement(NpcIndex)
    End Sub

    Private Sub PersigueCriminal(NpcIndex As Short)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 12/01/2010 (ZaMa)
        '14/09/2009: ZaMa - Now npcs don't follow protected users.
        '12/01/2010: ZaMa - Los npcs no atacan druidas mimetizados con npcs.
        '***************************************************
        Dim UserIndex As Short
        Dim tHeading As Byte
        Dim i As Integer
        Dim SignoNS As Short
        Dim SignoEO As Short
        Dim UserProtected As Boolean

        With Npclist(NpcIndex)
            If .flags.Inmovilizado = 1 Then
                Select Case .Char_Renamed.heading
                    Case eHeading.NORTH
                        SignoNS = - 1
                        SignoEO = 0

                    Case eHeading.EAST
                        SignoNS = 0
                        SignoEO = 1

                    Case eHeading.SOUTH
                        SignoNS = 1
                        SignoEO = 0

                    Case eHeading.WEST
                        SignoEO = - 1
                        SignoNS = 0
                End Select

                For i = 1 To ConnGroups(.Pos.Map).CountEntrys
                    UserIndex = ConnGroups(.Pos.Map).UserEntrys(i)

                    'Is it in it's range of vision??
                    If _
                        Math.Abs(UserList(UserIndex).Pos.X - .Pos.X) <= RANGO_VISION_X And
                        Math.Sign(UserList(UserIndex).Pos.X - .Pos.X) = SignoEO Then
                        If _
                            Math.Abs(UserList(UserIndex).Pos.Y - .Pos.Y) <= RANGO_VISION_Y And
                            Math.Sign(UserList(UserIndex).Pos.Y - .Pos.Y) = SignoNS Then

                            If criminal(UserIndex) Then
                                With UserList(UserIndex)

                                    UserProtected = Not IntervaloPermiteSerAtacado(UserIndex) And
                                                    .flags.NoPuedeSerAtacado
                                    UserProtected = UserProtected Or UserList(UserIndex).flags.Ignorado Or
                                                    UserList(UserIndex).flags.EnConsulta

                                    If _
                                        .flags.Muerto = 0 And .flags.invisible = 0 And .flags.Oculto = 0 And
                                        .flags.AdminPerseguible And Not UserProtected Then

                                        If Npclist(NpcIndex).flags.LanzaSpells > 0 Then
                                            Call NpcLanzaUnSpell(NpcIndex, UserIndex)
                                        End If
                                        Exit Sub
                                    End If
                                End With
                            End If

                        End If
                    End If
                Next i
            Else
                For i = 1 To ConnGroups(.Pos.Map).CountEntrys
                    UserIndex = ConnGroups(.Pos.Map).UserEntrys(i)

                    'Is it in it's range of vision??
                    If Math.Abs(UserList(UserIndex).Pos.X - .Pos.X) <= RANGO_VISION_X Then
                        If Math.Abs(UserList(UserIndex).Pos.Y - .Pos.Y) <= RANGO_VISION_Y Then

                            If criminal(UserIndex) Then

                                UserProtected = Not IntervaloPermiteSerAtacado(UserIndex) And
                                                UserList(UserIndex).flags.NoPuedeSerAtacado
                                UserProtected = UserProtected Or UserList(UserIndex).flags.Ignorado

                                If _
                                    UserList(UserIndex).flags.Muerto = 0 And UserList(UserIndex).flags.invisible = 0 And
                                    UserList(UserIndex).flags.Oculto = 0 And UserList(UserIndex).flags.AdminPerseguible And
                                    Not UserProtected Then
                                    If .flags.LanzaSpells > 0 Then
                                        Call NpcLanzaUnSpell(NpcIndex, UserIndex)
                                    End If
                                    If .flags.Inmovilizado = 1 Then Exit Sub
                                    tHeading = FindDirection(.Pos, UserList(UserIndex).Pos)
                                    Call MoveNPCChar(NpcIndex, tHeading)
                                    Exit Sub
                                End If
                            End If

                        End If
                    End If

                Next i
            End If
        End With

        Call RestoreOldMovement(NpcIndex)
    End Sub

    Private Sub SeguirAmo(NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim tHeading As Byte
        Dim UI As Short

        With Npclist(NpcIndex)
            If .Target = 0 And .TargetNPC = 0 Then
                UI = .MaestroUser

                If UI > 0 Then
                    'Is it in it's range of vision??
                    If Math.Abs(UserList(UI).Pos.X - .Pos.X) <= RANGO_VISION_X Then
                        If Math.Abs(UserList(UI).Pos.Y - .Pos.Y) <= RANGO_VISION_Y Then
                            If _
                                UserList(UI).flags.Muerto = 0 And UserList(UI).flags.invisible = 0 And
                                UserList(UI).flags.Oculto = 0 And Distancia(.Pos, UserList(UI).Pos) > 3 Then
                                tHeading = FindDirection(.Pos, UserList(UI).Pos)
                                Call MoveNPCChar(NpcIndex, tHeading)
                                Exit Sub
                            End If
                        End If
                    End If
                End If
            End If
        End With

        Call RestoreOldMovement(NpcIndex)
    End Sub

    Private Sub AiNpcAtacaNpc(NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim tHeading As Byte
        Dim X As Integer
        Dim Y As Integer
        Dim NI As Short
        Dim bNoEsta As Boolean

        Dim SignoNS As Short
        Dim SignoEO As Short

        With Npclist(NpcIndex)
            If .flags.Inmovilizado = 1 Then
                Select Case .Char_Renamed.heading
                    Case eHeading.NORTH
                        SignoNS = - 1
                        SignoEO = 0

                    Case eHeading.EAST
                        SignoNS = 0
                        SignoEO = 1

                    Case eHeading.SOUTH
                        SignoNS = 1
                        SignoEO = 0

                    Case eHeading.WEST
                        SignoEO = - 1
                        SignoNS = 0
                End Select

                For Y = .Pos.Y To .Pos.Y + SignoNS*RANGO_VISION_Y Step IIf(SignoNS = 0, 1, SignoNS)
                    For X = .Pos.X To .Pos.X + SignoEO*RANGO_VISION_X Step IIf(SignoEO = 0, 1, SignoEO)
                        If X >= MinXBorder And X <= MaxXBorder And Y >= MinYBorder And Y <= MaxYBorder Then
                            NI = MapData(.Pos.Map, X, Y).NpcIndex
                            If NI > 0 Then
                                If .TargetNPC = NI Then
                                    bNoEsta = True
                                    If .Numero = ELEMENTALFUEGO Then
                                        Call NpcLanzaUnSpellSobreNpc(NpcIndex, NI)
                                        If Npclist(NI).NPCtype = eNPCType.DRAGON Then
                                            Npclist(NI).CanAttack = 1
                                            Call NpcLanzaUnSpellSobreNpc(NI, NpcIndex)
                                        End If
                                    Else
                                        'aca verificamosss la distancia de ataque
                                        If Distancia(.Pos, Npclist(NI).Pos) <= 1 Then
                                            Call NpcAtacaNpc(NpcIndex, NI)
                                        End If
                                    End If
                                    Exit Sub
                                End If
                            End If
                        End If
                    Next X
                Next Y
            Else
                For Y = .Pos.Y - RANGO_VISION_Y To .Pos.Y + RANGO_VISION_Y
                    For X = .Pos.X - RANGO_VISION_Y To .Pos.X + RANGO_VISION_Y
                        If X >= MinXBorder And X <= MaxXBorder And Y >= MinYBorder And Y <= MaxYBorder Then
                            NI = MapData(.Pos.Map, X, Y).NpcIndex
                            If NI > 0 Then
                                If .TargetNPC = NI Then
                                    bNoEsta = True
                                    If .Numero = ELEMENTALFUEGO Then
                                        Call NpcLanzaUnSpellSobreNpc(NpcIndex, NI)
                                        If Npclist(NI).NPCtype = eNPCType.DRAGON Then
                                            Npclist(NI).CanAttack = 1
                                            Call NpcLanzaUnSpellSobreNpc(NI, NpcIndex)
                                        End If
                                    Else
                                        'aca verificamosss la distancia de ataque
                                        If Distancia(.Pos, Npclist(NI).Pos) <= 1 Then
                                            Call NpcAtacaNpc(NpcIndex, NI)
                                        End If
                                    End If
                                    If .flags.Inmovilizado = 1 Then Exit Sub
                                    If .TargetNPC = 0 Then Exit Sub
                                    tHeading = FindDirection(.Pos, Npclist(MapData(.Pos.Map, X, Y).NpcIndex).Pos)
                                    Call MoveNPCChar(NpcIndex, tHeading)
                                    Exit Sub
                                End If
                            End If
                        End If
                    Next X
                Next Y
            End If

            If Not bNoEsta Then
                If .MaestroUser > 0 Then
                    Call FollowAmo(NpcIndex)
                Else
                    .Movement = .flags.OldMovement
                    .Hostile = .flags.OldHostil
                End If
            End If
        End With
    End Sub

    Public Sub AiNpcObjeto(NpcIndex As Short)
        '***************************************************
        'Autor: ZaMa
        'Last Modification: 14/09/2009 (ZaMa)
        '14/09/2009: ZaMa - Now npcs don't follow protected users.
        '***************************************************
        Dim UserIndex As Short
        Dim tHeading As Byte
        Dim i As Integer
        Dim SignoNS As Short
        Dim SignoEO As Short
        Dim UserProtected As Boolean

        With Npclist(NpcIndex)
            For i = 1 To ConnGroups(.Pos.Map).CountEntrys
                UserIndex = ConnGroups(.Pos.Map).UserEntrys(i)

                'Is it in it's range of vision??
                If Math.Abs(UserList(UserIndex).Pos.X - .Pos.X) <= RANGO_VISION_X Then
                    If Math.Abs(UserList(UserIndex).Pos.Y - .Pos.Y) <= RANGO_VISION_Y Then

                        With UserList(UserIndex)
                            UserProtected = Not IntervaloPermiteSerAtacado(UserIndex) And .flags.NoPuedeSerAtacado

                            If _
                                .flags.Muerto = 0 And .flags.invisible = 0 And .flags.Oculto = 0 And
                                .flags.AdminPerseguible And Not UserProtected Then

                                ' No quiero que ataque siempre al primero
                                If RandomNumber(1, 3) < 3 Then
                                    If Npclist(NpcIndex).flags.LanzaSpells > 0 Then
                                        Call NpcLanzaUnSpell(NpcIndex, UserIndex)
                                    End If

                                    Exit Sub
                                End If
                            End If
                        End With
                    End If
                End If

            Next i
        End With
    End Sub

    Sub NPCAI(NpcIndex As Short)
        '**************************************************************
        'Author: Unknown
        'Last Modify by: ZaMa
        'Last Modify Date: 15/11/2009
        '08/16/2008: MarKoxX - Now pets that do melï¿½ attacks have to be near the enemy to attack.
        '15/11/2009: ZaMa - Implementacion de npc objetos ai.
        '**************************************************************
        Try
            With Npclist(NpcIndex)
                '<<<<<<<<<<< Ataques >>>>>>>>>>>>>>>>
                If .MaestroUser = 0 Then
                    'Busca a alguien para atacar
                    'ï¿½Es un guardia?
                    If .NPCtype = eNPCType.GuardiaReal Then
                        Call GuardiasAI(NpcIndex, False)
                    ElseIf .NPCtype = eNPCType.Guardiascaos Then
                        Call GuardiasAI(NpcIndex, True)
                    ElseIf .Hostile And .Stats.Alineacion <> 0 Then
                        Call HostilMalvadoAI(NpcIndex)
                    ElseIf .Hostile And .Stats.Alineacion = 0 Then
                        Call HostilBuenoAI(NpcIndex)
                    End If
                Else
                    'Evitamos que ataque a su amo, a menos
                    'que el amo lo ataque.
                    'Call HostilBuenoAI(NpcIndex)
                End If


                '<<<<<<<<<<<Movimiento>>>>>>>>>>>>>>>>
                Select Case .Movement
                    Case TipoAI.MueveAlAzar
                        If .flags.Inmovilizado = 1 Then Exit Sub
                        If .NPCtype = eNPCType.GuardiaReal Then
                            If RandomNumber(1, 12) = 3 Then
                                Call _
                                    MoveNPCChar(NpcIndex,
                                                CByte(RandomNumber(eHeading.NORTH, eHeading.WEST)))
                            End If

                            Call PersigueCriminal(NpcIndex)

                        ElseIf .NPCtype = eNPCType.Guardiascaos Then
                            If RandomNumber(1, 12) = 3 Then
                                Call _
                                    MoveNPCChar(NpcIndex,
                                                CByte(RandomNumber(eHeading.NORTH, eHeading.WEST)))
                            End If

                            Call PersigueCiudadano(NpcIndex)

                        Else
                            If RandomNumber(1, 12) = 3 Then
                                Call _
                                    MoveNPCChar(NpcIndex,
                                                CByte(RandomNumber(eHeading.NORTH, eHeading.WEST)))
                            End If
                        End If

                        'Va hacia el usuario cercano
                    Case TipoAI.NpcMaloAtacaUsersBuenos
                        Call IrUsuarioCercano(NpcIndex)

                        'Va hacia el usuario que lo ataco(FOLLOW)
                    Case TipoAI.NPCDEFENSA
                        Call SeguirAgresor(NpcIndex)

                        'Persigue criminales
                    Case TipoAI.GuardiasAtacanCriminales
                        Call PersigueCriminal(NpcIndex)

                    Case TipoAI.SigueAmo
                        If .flags.Inmovilizado = 1 Then Exit Sub
                        Call SeguirAmo(NpcIndex)
                        If RandomNumber(1, 12) = 3 Then
                            Call _
                                MoveNPCChar(NpcIndex,
                                            CByte(RandomNumber(eHeading.NORTH, eHeading.WEST)))
                        End If

                    Case TipoAI.NpcAtacaNpc
                        Call AiNpcAtacaNpc(NpcIndex)

                    Case TipoAI.NpcObjeto
                        Call AiNpcObjeto(NpcIndex)

                    Case TipoAI.NpcPathfinding
                        If .flags.Inmovilizado = 1 Then Exit Sub
                        If ReCalculatePath(NpcIndex) Then
                            Call PathFindingAI(NpcIndex)
                            'Existe el camino?
                            If .PFINFO.NoPath Then 'Si no existe nos movemos al azar
                                'Move randomly
                                Call _
                                    MoveNPCChar(NpcIndex,
                                                RandomNumber(eHeading.NORTH, eHeading.WEST))
                            End If
                        Else
                            If Not PathEnd(NpcIndex) Then
                                Call FollowPath(NpcIndex)
                            Else
                                .PFINFO.PathLenght = 0
                            End If
                        End If
                End Select
            End With


        Catch ex As Exception
            Console.WriteLine("Error in GuardiasAI: " & ex.Message)
            Call _
                LogError(
                    "NPCAI " & Npclist(NpcIndex).name & " " & Npclist(NpcIndex).MaestroUser & " " &
                    Npclist(NpcIndex).MaestroNpc & " mapa:" & Npclist(NpcIndex).Pos.Map & " x:" &
                    Npclist(NpcIndex).Pos.X &
                    " y:" & Npclist(NpcIndex).Pos.Y & " Mov:" & Npclist(NpcIndex).Movement & " TargU:" &
                    Npclist(NpcIndex).Target & " TargN:" & Npclist(NpcIndex).TargetNPC)
            'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura MiNPC, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
            Dim MiNPC As npc
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto MiNPC. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            MiNPC = Npclist(NpcIndex)
            Call QuitarNPC(NpcIndex)
            Call ReSpawnNpc(MiNPC)
        End Try
    End Sub

    Function UserNear(NpcIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        'Returns True if there is an user adjacent to the npc position.
        '***************************************************

        With Npclist(NpcIndex)
            UserNear =
                Not _
                Int(Distance(.Pos.X, .Pos.Y, UserList(.PFINFO.TargetUser).Pos.X, UserList(.PFINFO.TargetUser).Pos.Y)) >
                1
        End With
    End Function

    Function ReCalculatePath(NpcIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        'Returns true if we have to seek a new path
        '***************************************************

        If Npclist(NpcIndex).PFINFO.PathLenght = 0 Then
            ReCalculatePath = True
        ElseIf Not UserNear(NpcIndex) And Npclist(NpcIndex).PFINFO.PathLenght = Npclist(NpcIndex).PFINFO.CurPos - 1 Then
            ReCalculatePath = True
        End If
    End Function

    Function PathEnd(NpcIndex As Short) As Boolean
        '***************************************************
        'Author: Gulfas Morgolock
        'Last Modification: -
        'Returns if the npc has arrived to the end of its path
        '***************************************************
        PathEnd = Npclist(NpcIndex).PFINFO.CurPos = Npclist(NpcIndex).PFINFO.PathLenght
    End Function

    Function FollowPath(NpcIndex As Short) As Boolean
        '***************************************************
        'Author: Gulfas Morgolock
        'Last Modification: -
        'Moves the npc.
        '***************************************************
        Dim tmpPos As WorldPos
        Dim tHeading As Byte

        With Npclist(NpcIndex)
            tmpPos.Map = .Pos.Map
            tmpPos.X = .PFINFO.Path(.PFINFO.CurPos).Y ' invertï¿½ las coordenadas
            tmpPos.Y = .PFINFO.Path(.PFINFO.CurPos).X

            tHeading = FindDirection(.Pos, tmpPos)

            MoveNPCChar(NpcIndex, tHeading)

            .PFINFO.CurPos = .PFINFO.CurPos + 1
        End With
    End Function

    Function PathFindingAI(NpcIndex As Short) As Boolean
        '***************************************************
        'Author: Gulfas Morgolock
        'Last Modification: -
        'This function seeks the shortest path from the Npc
        'to the user's location.
        '***************************************************
        Dim Y As Integer
        Dim X As Integer

        Dim tmpUserIndex As Short
        With Npclist(NpcIndex)
            For Y = .Pos.Y - 10 To .Pos.Y + 10 'Makes a loop that looks at
                For X = .Pos.X - 10 To .Pos.X + 10 '5 tiles in every direction

                    'Make sure tile is legal
                    If X > MinXBorder And X < MaxXBorder And Y > MinYBorder And Y < MaxYBorder Then

                        'look for a user
                        If MapData(.Pos.Map, X, Y).UserIndex > 0 Then
                            'Move towards user
                            tmpUserIndex = MapData(.Pos.Map, X, Y).UserIndex
                            With UserList(tmpUserIndex)
                                If _
                                    .flags.Muerto = 0 And .flags.invisible = 0 And .flags.Oculto = 0 And
                                    .flags.AdminPerseguible Then
                                    'We have to invert the coordinates, this is because
                                    'ORE refers to maps in converse way of my pathfinding
                                    'routines.
                                    Npclist(NpcIndex).PFINFO.Target.X = .Pos.Y
                                    Npclist(NpcIndex).PFINFO.Target.Y = .Pos.X 'ops!
                                    Npclist(NpcIndex).PFINFO.TargetUser = tmpUserIndex
                                    Call SeekPath(NpcIndex)
                                    Exit Function
                                End If
                            End With
                        End If
                    End If
                Next X
            Next Y
        End With
    End Function

    Sub NpcLanzaUnSpell(NpcIndex As Short, UserIndex As Short)
        '**************************************************************
        'Author: Unknown
        'Last Modify by: -
        'Last Modify Date: -
        '**************************************************************
        With UserList(UserIndex)
            If .flags.invisible = 1 Or .flags.Oculto = 1 Then Exit Sub
        End With

        Dim k As Short
        k = RandomNumber(1, Npclist(NpcIndex).flags.LanzaSpells)
        Call NpcLanzaSpellSobreUser(NpcIndex, UserIndex, Npclist(NpcIndex).Spells(k))
    End Sub

    Sub NpcLanzaUnSpellSobreNpc(NpcIndex As Short, TargetNPC As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim k As Short
        k = RandomNumber(1, Npclist(NpcIndex).flags.LanzaSpells)
        Call NpcLanzaSpellSobreNpc(NpcIndex, TargetNPC, Npclist(NpcIndex).Spells(k))
    End Sub
End Module
