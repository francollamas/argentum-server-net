Option Strict Off
Option Explicit On
Module mdParty
    ''
    ' SOPORTES PARA LAS PARTIES
    ' (Ver este modulo como una clase abstracta "PartyManager")
    '

    ''
    'cantidad maxima de parties en el servidor
    Public Const MAX_PARTIES As Short = 300

    ''
    'nivel minimo para crear party
    Public Const MINPARTYLEVEL As Byte = 15

    ''
    'Cantidad maxima de gente en la party
    Public Const PARTY_MAXMEMBERS As Byte = 5

    ''
    'Si esto esta en True, la exp sale por cada golpe que le da
    'Si no, la exp la recibe al salirse de la party (pq las partys, floodean)
    Public Const PARTY_EXPERIENCIAPORGOLPE As Boolean = False

    ''
    'maxima diferencia de niveles permitida en una party
    Public Const MAXPARTYDELTALEVEL As Byte = 7

    ''
    'distancia al leader para que este acepte el ingreso
    Public Const MAXDISTANCIAINGRESOPARTY As Byte = 2

    ''
    'maxima distancia a un exito para obtener su experiencia
    Public Const PARTY_MAXDISTANCIA As Byte = 18

    ''
    'restan las muertes de los miembros?
    Public Const CASTIGOS As Boolean = False

    ''
    'Numero al que elevamos el nivel de cada miembro de la party
    'Esto es usado para calcular la distribución de la experiencia entre los miembros
    'Se lee del archivo de balance
    Public ExponenteNivelParty As Single

    ''
    'tPartyMember
    '
    ' @param UserIndex UserIndex
    ' @param Experiencia Experiencia
    '
    Public Structure tPartyMember
        Dim UserIndex As Short
        Dim Experiencia As Double
    End Structure


    Public Function NextParty() As Short
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim i As Short
        NextParty = - 1
        For i = 1 To MAX_PARTIES
            If Parties(i) Is Nothing Then
                NextParty = i
                Exit Function
            End If
        Next i
    End Function

    Public Function PuedeCrearParty(ByVal UserIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        PuedeCrearParty = True
        '    If UserList(UserIndex).Stats.ELV < MINPARTYLEVEL Then

        If _
            CShort(UserList(UserIndex).Stats.UserAtributos(Declaraciones.eAtributos.Carisma))*
            UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Liderazgo) < 100 Then
            Call _
                WriteConsoleMsg(UserIndex, "Tu carisma y liderazgo no son suficientes para liderar una party.",
                                Protocol.FontTypeNames.FONTTYPE_PARTY)
            PuedeCrearParty = False
        ElseIf UserList(UserIndex).flags.Muerto = 1 Then
            Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", Protocol.FontTypeNames.FONTTYPE_PARTY)
            PuedeCrearParty = False
        End If
    End Function

    Public Sub CrearParty(ByVal UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim tInt As Short

        With UserList(UserIndex)
            If .PartyIndex = 0 Then
                If .flags.Muerto = 0 Then
                    If .Stats.UserSkills(Declaraciones.eSkill.Liderazgo) >= 5 Then
                        tInt = mdParty.NextParty
                        If tInt = - 1 Then
                            Call _
                                WriteConsoleMsg(UserIndex, "Por el momento no se pueden crear más parties.",
                                                Protocol.FontTypeNames.FONTTYPE_PARTY)
                            Exit Sub
                        Else
                            Parties(tInt) = New clsParty
                            If Not Parties(tInt).NuevoMiembro(UserIndex) Then
                                Call _
                                    WriteConsoleMsg(UserIndex, "La party está llena, no puedes entrar.",
                                                    Protocol.FontTypeNames.FONTTYPE_PARTY)
                                'UPGRADE_NOTE: El objeto Parties() no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
                                Parties(tInt) = Nothing
                                Exit Sub
                            Else
                                Call _
                                    WriteConsoleMsg(UserIndex, "¡Has formado una party!",
                                                    Protocol.FontTypeNames.FONTTYPE_PARTY)
                                .PartyIndex = tInt
                                .PartySolicitud = 0
                                If Not Parties(tInt).HacerLeader(UserIndex) Then
                                    Call _
                                        WriteConsoleMsg(UserIndex, "No puedes hacerte líder.",
                                                        Protocol.FontTypeNames.FONTTYPE_PARTY)
                                Else
                                    Call _
                                        WriteConsoleMsg(UserIndex, "¡Te has convertido en líder de la party!",
                                                        Protocol.FontTypeNames.FONTTYPE_PARTY)
                                End If
                            End If
                        End If
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "No tienes suficientes puntos de liderazgo para liderar una party.",
                                            Protocol.FontTypeNames.FONTTYPE_PARTY)
                    End If
                Else
                    Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", Protocol.FontTypeNames.FONTTYPE_PARTY)
                End If
            Else
                Call WriteConsoleMsg(UserIndex, "Ya perteneces a una party.", Protocol.FontTypeNames.FONTTYPE_PARTY)
            End If
        End With
    End Sub

    Public Sub SolicitarIngresoAParty(ByVal UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'ESTO ES enviado por el PJ para solicitar el ingreso a la party
        Dim tInt As Short

        With UserList(UserIndex)
            If .PartyIndex > 0 Then
                'si ya esta en una party
                Call _
                    WriteConsoleMsg(UserIndex, "Ya perteneces a una party, escribe /SALIRPARTY para abandonarla",
                                    Protocol.FontTypeNames.FONTTYPE_PARTY)
                .PartySolicitud = 0
                Exit Sub
            End If
            If .flags.Muerto = 1 Then
                Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", Protocol.FontTypeNames.FONTTYPE_INFO)
                .PartySolicitud = 0
                Exit Sub
            End If
            tInt = .flags.TargetUser
            If tInt > 0 Then
                If UserList(tInt).PartyIndex > 0 Then
                    .PartySolicitud = UserList(tInt).PartyIndex
                    Call _
                        WriteConsoleMsg(UserIndex, "El fundador decidirá si te acepta en la party.",
                                        Protocol.FontTypeNames.FONTTYPE_PARTY)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, UserList(tInt).name & " no es fundador de ninguna party.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                    .PartySolicitud = 0
                    Exit Sub
                End If
            Else
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Para ingresar a una party debes hacer click sobre el fundador y luego escribir /PARTY",
                                    Protocol.FontTypeNames.FONTTYPE_PARTY)
                .PartySolicitud = 0
            End If
        End With
    End Sub

    Public Sub SalirDeParty(ByVal UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim PI As Short
        PI = UserList(UserIndex).PartyIndex
        If PI > 0 Then
            If Parties(PI).SaleMiembro(UserIndex) Then
                'sale el leader
                'UPGRADE_NOTE: El objeto Parties() no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
                Parties(PI) = Nothing
            Else
                UserList(UserIndex).PartyIndex = 0
            End If
        Else
            Call WriteConsoleMsg(UserIndex, "No eres miembro de ninguna party.", Protocol.FontTypeNames.FONTTYPE_INFO)
        End If
    End Sub

    Public Sub ExpulsarDeParty(ByVal leader As Short, ByVal OldMember As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim PI As Short
        PI = UserList(leader).PartyIndex

        If PI = UserList(OldMember).PartyIndex Then
            If Parties(PI).SaleMiembro(OldMember) Then
                'si la funcion me da true, entonces la party se disolvio
                'y los partyindex fueron reseteados a 0
                'UPGRADE_NOTE: El objeto Parties() no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
                Parties(PI) = Nothing
            Else
                UserList(OldMember).PartyIndex = 0
            End If
        Else
            Call _
                WriteConsoleMsg(leader, LCase(UserList(OldMember).name) & " no pertenece a tu party.",
                                Protocol.FontTypeNames.FONTTYPE_INFO)
        End If
    End Sub

    ''
    ' Determines if a user can use party commands like /acceptparty or not.
    '
    ' @param User Specifies reference to user
    ' @return  True if the user can use party commands, false if not.
    Public Function UserPuedeEjecutarComandos(ByVal User As Short) As Boolean
        '*************************************************
        'Author: Marco Vanotti(Marco)
        'Last modified: 05/05/09
        '
        '*************************************************
        Dim PI As Short

        PI = UserList(User).PartyIndex

        If PI > 0 Then
            If Parties(PI).EsPartyLeader(User) Then
                UserPuedeEjecutarComandos = True
            Else
                Call WriteConsoleMsg(User, "¡No eres el líder de tu party!", Protocol.FontTypeNames.FONTTYPE_PARTY)
                Exit Function
            End If
        Else
            Call WriteConsoleMsg(User, "No eres miembro de ninguna party.", Protocol.FontTypeNames.FONTTYPE_INFO)
            Exit Function
        End If
    End Function

    Public Sub AprobarIngresoAParty(ByVal leader As Short, ByVal NewMember As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 11/03/2010
        '11/03/2010: ZaMa - Le avisa al lider si intenta aceptar a alguien que sea mimebro de su propia party.
        '***************************************************

        'el UI es el leader
        Dim PI As Short
        Dim razon As String

        PI = UserList(leader).PartyIndex

        With UserList(NewMember)
            If .PartySolicitud = PI Then
                If Not .flags.Muerto = 1 Then
                    If .PartyIndex = 0 Then
                        If Parties(PI).PuedeEntrar(NewMember, razon) Then
                            If Parties(PI).NuevoMiembro(NewMember) Then
                                Call _
                                    Parties(PI).MandarMensajeAConsola(
                                        UserList(leader).name & " ha aceptado a " & .name & " en la party.", "Servidor")
                                .PartyIndex = PI
                                .PartySolicitud = 0
                            Else
                                'no pudo entrar
                                'ACA UNO PUEDE CODIFICAR OTRO TIPO DE ERRORES...
                                Call _
                                    SendData(modSendData.SendTarget.ToAdmins, leader,
                                             PrepareMessageConsoleMsg(
                                                 " Servidor> CATÁSTROFE EN PARTIES, NUEVOMIEMBRO DIO FALSE! :S ",
                                                 Protocol.FontTypeNames.FONTTYPE_PARTY))
                            End If
                        Else
                            'no debe entrar
                            Call WriteConsoleMsg(leader, razon, Protocol.FontTypeNames.FONTTYPE_PARTY)
                        End If
                    Else
                        If .PartyIndex = PI Then
                            Call _
                                WriteConsoleMsg(leader, LCase(.name) & " ya es miembro de la party.",
                                                Protocol.FontTypeNames.FONTTYPE_PARTY)
                        Else
                            Call _
                                WriteConsoleMsg(leader, .name & " ya es miembro de otra party.",
                                                Protocol.FontTypeNames.FONTTYPE_PARTY)
                        End If

                        Exit Sub
                    End If
                Else
                    Call _
                        WriteConsoleMsg(leader, "¡Está muerto, no puedes aceptar miembros en ese estado!",
                                        Protocol.FontTypeNames.FONTTYPE_PARTY)
                    Exit Sub
                End If
            Else
                If .PartyIndex = PI Then
                    Call _
                        WriteConsoleMsg(leader, LCase(.name) & " ya es miembro de la party.",
                                        Protocol.FontTypeNames.FONTTYPE_PARTY)
                Else
                    Call _
                        WriteConsoleMsg(leader, LCase(.name) & " no ha solicitado ingresar a tu party.",
                                        Protocol.FontTypeNames.FONTTYPE_PARTY)
                End If

                Exit Sub
            End If
        End With
    End Sub

    Private Function IsPartyMember(ByVal UserIndex As Short, ByVal PartyIndex As Short) As Object
        Dim MemberIndex As Short

        For MemberIndex = 1 To PARTY_MAXMEMBERS

        Next MemberIndex
    End Function

    Public Sub BroadCastParty(ByVal UserIndex As Short, ByRef texto As String)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim PI As Short

        PI = UserList(UserIndex).PartyIndex

        If PI > 0 Then
            Call Parties(PI).MandarMensajeAConsola(texto, UserList(UserIndex).name)
        End If
    End Sub

    Public Sub OnlineParty(ByVal UserIndex As Short)
        '*************************************************
        'Author: Unknown
        'Last modified: 11/27/09 (Budi)
        'Adapte la función a los nuevos métodos de clsParty
        '*************************************************
        Dim i As Short
        Dim PI As Short
        Dim Text As String
        'UPGRADE_WARNING: El límite inferior de la matriz MembersOnline ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        Dim MembersOnline(PARTY_MAXMEMBERS) As Short

        PI = UserList(UserIndex).PartyIndex

        If PI > 0 Then
            Call Parties(PI).ObtenerMiembrosOnline(MembersOnline)
            Text = "Nombre(Exp): "
            For i = 1 To PARTY_MAXMEMBERS
                If MembersOnline(i) > 0 Then
                    Text = Text & " - " & UserList(MembersOnline(i)).name & " (" &
                           Fix(Parties(PI).MiExperiencia(MembersOnline(i))) & ")"
                End If
            Next i
            Text = Text & ". Experiencia total: " & Parties(PI).ObtenerExperienciaTotal
            Call WriteConsoleMsg(UserIndex, Text, Protocol.FontTypeNames.FONTTYPE_PARTY)
        End If
    End Sub


    Public Sub TransformarEnLider(ByVal OldLeader As Short, ByVal NewLeader As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim PI As Short

        If OldLeader = NewLeader Then Exit Sub

        PI = UserList(OldLeader).PartyIndex

        If PI = UserList(NewLeader).PartyIndex Then
            If UserList(NewLeader).flags.Muerto = 0 Then
                If Parties(PI).HacerLeader(NewLeader) Then
                    Call _
                        Parties(PI).MandarMensajeAConsola("El nuevo líder de la party es " & UserList(NewLeader).name,
                                                          UserList(OldLeader).name)
                Else
                    Call _
                        WriteConsoleMsg(OldLeader, "¡No se ha hecho el cambio de mando!",
                                        Protocol.FontTypeNames.FONTTYPE_PARTY)
                End If
            Else
                Call WriteConsoleMsg(OldLeader, "¡Está muerto!", Protocol.FontTypeNames.FONTTYPE_INFO)
            End If
        Else
            Call _
                WriteConsoleMsg(OldLeader, LCase(UserList(NewLeader).name) & " no pertenece a tu party.",
                                Protocol.FontTypeNames.FONTTYPE_INFO)
        End If
    End Sub


    Public Sub ActualizaExperiencias()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'esta funcion se invoca antes de worlsaves, y apagar servidores
        'en caso que la experiencia sea acumulada y no por golpe
        'para que grabe los datos en los charfiles
        Dim i As Short

        If Not PARTY_EXPERIENCIAPORGOLPE Then

            haciendoBK = True
            Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessagePauseToggle())

            Call _
                SendData(modSendData.SendTarget.ToAll, 0,
                         PrepareMessageConsoleMsg("Servidor> Distribuyendo experiencia en parties.",
                                                  Protocol.FontTypeNames.FONTTYPE_SERVER))
            For i = 1 To MAX_PARTIES
                If Not Parties(i) Is Nothing Then
                    Call Parties(i).FlushExperiencia()
                End If
            Next i
            Call _
                SendData(modSendData.SendTarget.ToAll, 0,
                         PrepareMessageConsoleMsg("Servidor> Experiencia distribuida.",
                                                  Protocol.FontTypeNames.FONTTYPE_SERVER))
            Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessagePauseToggle())
            haciendoBK = False

        End If
    End Sub

    Public Sub ObtenerExito(ByVal UserIndex As Short, ByVal Exp As Integer, ByRef mapa As Short, ByRef X As Short,
                            ByRef Y As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        If Exp <= 0 Then
            If Not CASTIGOS Then Exit Sub
        End If

        Call Parties(UserList(UserIndex).PartyIndex).ObtenerExito(Exp, mapa, X, Y)
    End Sub

    Public Function CantMiembros(ByVal UserIndex As Short) As Short
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        CantMiembros = 0
        If UserList(UserIndex).PartyIndex > 0 Then
            CantMiembros = Parties(UserList(UserIndex).PartyIndex).CantMiembros
        End If
    End Function

    ''
    ' Sets the new p_sumaniveleselevados to the party.
    '
    ' @param UserInidex Specifies reference to user
    ' @remarks When a user level up and he is in a party, we call this sub to don't desestabilice the party exp formula
    Public Sub ActualizarSumaNivelesElevados(ByVal UserIndex As Short)
        '*************************************************
        'Author: Marco Vanotti (MarKoxX)
        'Last modified: 28/10/08
        '
        '*************************************************
        If UserList(UserIndex).PartyIndex > 0 Then
            Call Parties(UserList(UserIndex).PartyIndex).UpdateSumaNivelesElevados(UserList(UserIndex).Stats.ELV)
        End If
    End Sub
End Module
