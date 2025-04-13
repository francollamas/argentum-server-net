Option Strict Off
Option Explicit On
Friend Class clsClan
    ''
    ' clase clan
    '
    ' Es el "ADO" de los clanes. La interfaz entre el disco y
    ' el juego. Los datos no se guardan en memoria
    ' para evitar problemas de sincronizacion, y considerando
    ' que la performance de estas rutinas NO es critica.
    ' by el oso :p

    Private p_GuildName As String
    Private p_Alineacion As modGuilds.ALINEACION_GUILD
    Private p_OnlineMembers As List(Of String) 'Array de UserIndexes!
    Private p_GMsOnline As List(Of String)
    Private p_PropuestasDePaz As List(Of String)
    Private p_PropuestasDeAlianza As List(Of String)
    Private p_IteradorRelaciones As Short
    Private p_IteradorOnlineMembers As Short
    Private p_IteradorPropuesta As Short
    Private p_IteradorOnlineGMs As Short
    Private p_GuildNumber As Short 'Numero de guild en el mundo
    Private p_Relaciones() As modGuilds.RELACIONES_GUILD 'array de relaciones con los otros clanes
    Private GUILDINFOFILE As String
    Private GUILDPATH As String 'aca pq me es mas comodo setearlo y pq en ningun disenio
    Private MEMBERSFILE As String 'decente la capa de arriba se entera donde estan
    Private SOLICITUDESFILE As String 'los datos fisicamente
    Private PROPUESTASFILE As String
    Private RELACIONESFILE As String
    Private VOTACIONESFILE As String

    Private Const NEWSLENGTH As Short = 1024
    Private Const DESCLENGTH As Short = 256
    Private Const CODEXLENGTH As Short = 256

    Public ReadOnly Property GuildName() As String
        Get
            GuildName = p_GuildName
        End Get
    End Property


    '
    'ALINEACION Y ANTIFACCION
    '

    Public ReadOnly Property Alineacion() As modGuilds.ALINEACION_GUILD
        Get
            Alineacion = p_Alineacion
        End Get
    End Property


    Public Property PuntosAntifaccion() As Short
        Get
            PuntosAntifaccion = Val(GetVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "Antifaccion"))
        End Get
        Set(ByVal Value As Short)
            Call WriteVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "Antifaccion", CStr(Value))
        End Set
    End Property


    '
    'MEMBRESIAS
    '

    Public ReadOnly Property Fundador() As String
        Get
            Fundador = GetVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "Founder")
        End Get
    End Property

    'Public Property Get JugadoresOnline() As String
    'Dim i As Integer
    '    'leve violacion de capas x aqui, je
    '    For i = 1 To p_OnlineMembers.Count
    '        JugadoresOnline = UserList(p_OnlineMembers.Item(i)).Name & "," & JugadoresOnline
    '    Next i
    'End Property

    Public ReadOnly Property CantidadDeMiembros() As Short
        Get
            Dim OldQ As String
            OldQ = GetVar(MEMBERSFILE, "INIT", "NroMembers")
            CantidadDeMiembros = IIf(IsNumeric(OldQ), CShort(OldQ), 0)
        End Get
    End Property

    '/VOTACIONES


    '
    'RELACIONES
    '

    Public ReadOnly Property CantidadPropuestas(ByVal Tipo As modGuilds.RELACIONES_GUILD) As Short
        Get
            Select Case Tipo
                Case modGuilds.RELACIONES_GUILD.ALIADOS
                    CantidadPropuestas = p_PropuestasDeAlianza.Count()
                Case modGuilds.RELACIONES_GUILD.GUERRA

                Case modGuilds.RELACIONES_GUILD.PAZ
                    CantidadPropuestas = p_PropuestasDePaz.Count()
            End Select
        End Get
    End Property

    Public ReadOnly Property CantidadEnemys() As Short
        Get
            Dim i As Short
            For i = 1 To CANTIDADDECLANES
                CantidadEnemys = CantidadEnemys + IIf(p_Relaciones(i) = modGuilds.RELACIONES_GUILD.GUERRA, 1, 0)
            Next i
        End Get
    End Property

    Public ReadOnly Property CantidadAllies() As Short
        Get
            Dim i As Short
            For i = 1 To CANTIDADDECLANES
                CantidadAllies = CantidadAllies + IIf(p_Relaciones(i) = modGuilds.RELACIONES_GUILD.ALIADOS, 1, 0)
            Next i
        End Get
    End Property

    Public Function CambiarAlineacion(ByVal NuevaAlineacion As modGuilds.ALINEACION_GUILD) As Boolean
        p_Alineacion = NuevaAlineacion
        Call WriteVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "Alineacion", Alineacion2String(p_Alineacion))

        If p_Alineacion = modGuilds.ALINEACION_GUILD.ALINEACION_NEUTRO Then CambiarAlineacion = True
    End Function

    '
    'INICIALIZADORES
    '

    'UPGRADE_NOTE: Class_Initialize se actualizó a Class_Initialize_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Private Sub Class_Initialize_Renamed()
        GUILDPATH = AppDomain.CurrentDomain.BaseDirectory & "GUILDS/"
        GUILDINFOFILE = GUILDPATH & "guildsinfo.inf"
    End Sub

    Public Sub New()
        MyBase.New()
        Class_Initialize_Renamed()
    End Sub

    'UPGRADE_NOTE: Class_Terminate se actualizó a Class_Terminate_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Private Sub Class_Terminate_Renamed()
        'UPGRADE_NOTE: El objeto p_OnlineMembers no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        p_OnlineMembers = Nothing
        'UPGRADE_NOTE: El objeto p_GMsOnline no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        p_GMsOnline = Nothing
        'UPGRADE_NOTE: El objeto p_PropuestasDePaz no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        p_PropuestasDePaz = Nothing
        'UPGRADE_NOTE: El objeto p_PropuestasDeAlianza no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        p_PropuestasDeAlianza = Nothing
    End Sub

    Protected Overrides Sub Finalize()
        Class_Terminate_Renamed()
        MyBase.Finalize()
    End Sub


    Public Sub Inicializar(ByVal GuildName As String, ByVal GuildNumber As Short,
                           ByVal Alineacion As modGuilds.ALINEACION_GUILD)
        Dim i As Short

        p_GuildName = GuildName
        p_GuildNumber = GuildNumber
        p_Alineacion = Alineacion
        p_OnlineMembers = New List(Of String)
        p_GMsOnline = New List(Of String)
        p_PropuestasDePaz = New List(Of String)
        p_PropuestasDeAlianza = New List(Of String)
        'ALLIESFILE = GUILDPATH & p_GuildName & "-Allied.all"
        'ENEMIESFILE = GUILDPATH & p_GuildName & "-enemys.ene"
        RELACIONESFILE = GUILDPATH & p_GuildName & "-relaciones.rel"
        MEMBERSFILE = GUILDPATH & p_GuildName & "-members.mem"
        PROPUESTASFILE = GUILDPATH & p_GuildName & "-propositions.pro"
        SOLICITUDESFILE = GUILDPATH & p_GuildName & "-solicitudes.sol"
        VOTACIONESFILE = GUILDPATH & p_GuildName & "-votaciones.vot"
        p_IteradorOnlineMembers = 0
        p_IteradorPropuesta = 0
        p_IteradorOnlineGMs = 0
        p_IteradorRelaciones = 0
        'UPGRADE_WARNING: El límite inferior de la matriz p_Relaciones ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim Preserve p_Relaciones(CANTIDADDECLANES)
        For i = 1 To CANTIDADDECLANES
            p_Relaciones(i) = String2Relacion(GetVar(RELACIONESFILE, "RELACIONES", CStr(i)))
        Next i
        For i = 1 To CANTIDADDECLANES
            If Trim(GetVar(PROPUESTASFILE, CStr(i), "Pendiente")) = "1" Then
                Select Case String2Relacion(Trim(GetVar(PROPUESTASFILE, CStr(i), "Tipo")))
                    Case modGuilds.RELACIONES_GUILD.ALIADOS
                        p_PropuestasDeAlianza.Add(i)
                    Case modGuilds.RELACIONES_GUILD.PAZ
                        p_PropuestasDePaz.Add(i)
                End Select
            End If
        Next i
    End Sub

    ''
    ' esta TIENE QUE LLAMARSE LUEGO DE INICIALIZAR()
    '
    ' @param Fundador Nombre del fundador del clan
    '
    Public Sub InicializarNuevoClan(ByRef Fundador As String)
        Dim OldQ As String 'string pq al comienzo quizas no hay archivo guildinfo.ini y oldq es ""
        Dim NewQ As Short
        'para que genere los archivos
        Call WriteVar(MEMBERSFILE, "INIT", "NroMembers", "0")
        Call WriteVar(SOLICITUDESFILE, "INIT", "CantSolicitudes", "0")


        OldQ = GetVar(GUILDINFOFILE, "INIT", "nroguilds")
        If IsNumeric(OldQ) Then
            NewQ = CShort(Trim(OldQ)) + 1
        Else
            NewQ = 1
        End If

        Call WriteVar(GUILDINFOFILE, "INIT", "NroGuilds", CStr(NewQ))

        Call WriteVar(GUILDINFOFILE, "GUILD" & NewQ, "Founder", Fundador)
        Call WriteVar(GUILDINFOFILE, "GUILD" & NewQ, "GuildName", p_GuildName)
        Call WriteVar(GUILDINFOFILE, "GUILD" & NewQ, "Date", CStr(Today))
        Call WriteVar(GUILDINFOFILE, "GUILD" & NewQ, "Antifaccion", "0")
        Call WriteVar(GUILDINFOFILE, "GUILD" & NewQ, "Alineacion", Alineacion2String(p_Alineacion))
    End Sub

    Public Sub ProcesarFundacionDeOtroClan()
        'UPGRADE_WARNING: El límite inferior de la matriz p_Relaciones ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim Preserve p_Relaciones(CANTIDADDECLANES)
        p_Relaciones(CANTIDADDECLANES) = modGuilds.RELACIONES_GUILD.PAZ
    End Sub

    Public Sub SetLeader(ByRef leader As String)
        Call WriteVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "Leader", leader)
    End Sub

    Public Function GetLeader() As String
        GetLeader = GetVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "Leader")
    End Function

    Public Function GetMemberList() As String()
        Dim OldQ As Short
        Dim list() As String
        Dim i As Integer

        OldQ = Me.CantidadDeMiembros

        ReDim list(OldQ - 1)

        For i = 1 To OldQ
            list(i - 1) = UCase(GetVar(MEMBERSFILE, "Members", "Member" & i))
        Next i

        Return list
    End Function

    Public Sub ConectarMiembro(ByVal UserIndex As Short)
        p_OnlineMembers.Add(UserIndex)

        With UserList(UserIndex)
            Call _
                SendData(modSendData.SendTarget.ToDiosesYclan, .GuildIndex,
                         PrepareMessageGuildChat(.name & " se ha conectado."))
        End With
    End Sub

    Public Sub DesConectarMiembro(ByVal UserIndex As Short)
        Dim i As Short
        For i = 1 To p_OnlineMembers.Count()
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_OnlineMembers.Item(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            If p_OnlineMembers.Item(i) = UserIndex Then
                p_OnlineMembers.Remove(i)

                With UserList(UserIndex)
                    Call _
                        SendData(modSendData.SendTarget.ToDiosesYclan, .GuildIndex,
                                 PrepareMessageGuildChat(.name & " se ha desconectado."))
                End With

                Exit Sub
            End If
        Next i
    End Sub

    Public Sub AceptarNuevoMiembro(ByRef Nombre As String)
        Dim OldQ As Short
        Dim OldQs As String
        Dim ruta As String

        ruta = CharPath & Nombre & ".chr"
        If FileExist(ruta) Then
            Call WriteVar(ruta, "GUILD", "GUILDINDEX", CStr(p_GuildNumber))
            Call WriteVar(ruta, "GUILD", "AspiranteA", "0")
            'CantPs = GetVar(CharPath & Nombre & ".chr", "GUILD", "ClanesParticipo")
            'If IsNumeric(CantPs) Then
            '    CantP = CInt(CantPs)
            'Else
            '    CantP = 0
            'End If
            'Call WriteVar(CharPath & Nombre & ".chr", "GUILD", "ClanesParticipo", CantP + 1)
            OldQs = GetVar(MEMBERSFILE, "INIT", "NroMembers")
            If IsNumeric(OldQs) Then
                OldQ = CShort(OldQs)
            Else
                OldQ = 0
            End If
            Call WriteVar(MEMBERSFILE, "INIT", "NroMembers", CStr(OldQ + 1))
            Call WriteVar(MEMBERSFILE, "Members", "Member" & OldQ + 1, Nombre)
        End If
    End Sub

    Public Sub ExpulsarMiembro(ByRef Nombre As String)
        Dim OldQ As Short
        Dim Temps As String
        Dim i As Short
        Dim EsMiembro As Boolean
        Dim MiembroDe As String

        'UPGRADE_WARNING: Dir tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If migr_LenB(Dir(CharPath & Nombre & ".chr")) <> 0 Then
            OldQ = CShort(GetVar(MEMBERSFILE, "INIT", "NroMembers"))
            i = 1
            Nombre = UCase(Nombre)
            While i <= OldQ And UCase(Trim(GetVar(MEMBERSFILE, "Members", "Member" & i))) <> Nombre
                i = i + 1
            End While
            EsMiembro = i <= OldQ

            If EsMiembro Then
                Call WriteVar(CharPath & Nombre & ".chr", "GUILD", "GuildIndex", vbNullString)
                While i < OldQ
                    Temps = GetVar(MEMBERSFILE, "Members", "Member" & i + 1)
                    Call WriteVar(MEMBERSFILE, "Members", "Member" & i, Temps)
                    i = i + 1
                End While
                Call WriteVar(MEMBERSFILE, "Members", "Member" & OldQ, vbNullString)
                'seteo la cantidad de miembros nueva
                Call WriteVar(MEMBERSFILE, "INIT", "NroMembers", CStr(OldQ - 1))
                'lo echo a el
                MiembroDe = GetVar(CharPath & Nombre & ".chr", "GUILD", "Miembro")
                If Not InStr(1, MiembroDe, p_GuildName, CompareMethod.Text) > 0 Then
                    If migr_LenB(MiembroDe) <> 0 Then
                        MiembroDe = MiembroDe & ","
                    End If
                    MiembroDe = MiembroDe & p_GuildName
                    Call WriteVar(CharPath & Nombre & ".chr", "GUILD", "Miembro", MiembroDe)
                End If
            End If

        End If
    End Sub

    '
    'ASPIRANTES
    '

    Public Function GetAspirantes() As String()
        Dim OldQ As Short
        Dim list() As String
        Dim i As Integer

        OldQ = Me.CantidadAspirantes()

        If OldQ > 1 Then
            ReDim list(OldQ - 1)
        Else
            ReDim list(0)
        End If

        For i = 1 To OldQ
            list(i - 1) = GetVar(SOLICITUDESFILE, "SOLICITUD" & i, "Nombre")
        Next i

        Return list
    End Function

    Public Function CantidadAspirantes() As Short
        Dim Temps As String

        CantidadAspirantes = 0
        Temps = GetVar(SOLICITUDESFILE, "INIT", "CantSolicitudes")
        If Not IsNumeric(Temps) Then
            Exit Function
        End If
        CantidadAspirantes = CShort(Temps)
    End Function

    Public Function DetallesSolicitudAspirante(ByVal NroAspirante As Short) As String
        DetallesSolicitudAspirante = GetVar(SOLICITUDESFILE, "SOLICITUD" & NroAspirante, "Detalle")
    End Function

    Public Function NumeroDeAspirante(ByRef Nombre As String) As Short
        Dim i As Short

        NumeroDeAspirante = 0

        For i = 1 To MAXASPIRANTES
            If UCase(Trim(GetVar(SOLICITUDESFILE, "SOLICITUD" & i, "Nombre"))) = UCase(Nombre) Then
                NumeroDeAspirante = i
                Exit Function
            End If
        Next i
    End Function

    Public Sub NuevoAspirante(ByRef Nombre As String, ByRef Peticion As String)
        Dim i As Short
        Dim OldQ As String
        Dim OldQI As Short

        OldQ = GetVar(SOLICITUDESFILE, "INIT", "CantSolicitudes")
        If IsNumeric(OldQ) Then
            OldQI = CShort(OldQ)
        Else
            OldQI = 0
        End If
        For i = 1 To MAXASPIRANTES
            If GetVar(SOLICITUDESFILE, "SOLICITUD" & i, "Nombre") = vbNullString Then
                Call WriteVar(SOLICITUDESFILE, "SOLICITUD" & i, "Nombre", Nombre)
                Call _
                    WriteVar(SOLICITUDESFILE, "SOLICITUD" & i, "Detalle",
                             IIf(Trim(Peticion) = vbNullString, "Peticion vacia", Peticion))
                Call WriteVar(SOLICITUDESFILE, "INIT", "CantSolicitudes", CStr(OldQI + 1))
                Call WriteVar(CharPath & Nombre & ".chr", "GUILD", "ASPIRANTEA", CStr(p_GuildNumber))
                Exit Sub
            End If
        Next i
    End Sub

    Public Sub RetirarAspirante(ByRef Nombre As String, ByRef NroAspirante As Short)
        Dim OldQ As String
        Dim OldQI As String
        Dim Pedidos As String
        Dim i As Short

        OldQ = GetVar(SOLICITUDESFILE, "INIT", "CantSolicitudes")
        If IsNumeric(OldQ) Then
            OldQI = CStr(CShort(OldQ))
        Else
            OldQI = CStr(1)
        End If
        'Call WriteVar(SOLICITUDESFILE, "SOLICITUD" & NroAspirante, "Nombre", vbNullString)
        'Call WriteVar(SOLICITUDESFILE, "SOLICITUD" & NroAspirante, "Detalle", vbNullString)
        Call WriteVar(CharPath & Nombre & ".chr", "GUILD", "ASPIRANTEA", "0")
        Pedidos = GetVar(CharPath & Nombre & ".chr", "GUILD", "Pedidos")
        If Not InStr(1, Pedidos, p_GuildName, CompareMethod.Text) > 0 Then
            If migr_LenB(Pedidos) <> 0 Then
                Pedidos = Pedidos & ","
            End If
            Pedidos = Pedidos & p_GuildName
            Call WriteVar(CharPath & Nombre & ".chr", "GUILD", "Pedidos", Pedidos)
        End If

        Call WriteVar(SOLICITUDESFILE, "INIT", "CantSolicitudes", CStr(CDbl(OldQI) - 1))
        For i = NroAspirante To modGuilds.MAXASPIRANTES - 1
            Call _
                WriteVar(SOLICITUDESFILE, "SOLICITUD" & i, "Nombre",
                         GetVar(SOLICITUDESFILE, "SOLICITUD" & (i + 1), "Nombre"))
            Call _
                WriteVar(SOLICITUDESFILE, "SOLICITUD" & i, "Detalle",
                         GetVar(SOLICITUDESFILE, "SOLICITUD" & (i + 1), "Detalle"))
        Next i

        Call WriteVar(SOLICITUDESFILE, "SOLICITUD" & modGuilds.MAXASPIRANTES, "Nombre", vbNullString)
        Call WriteVar(SOLICITUDESFILE, "SOLICITUD" & modGuilds.MAXASPIRANTES, "Detalle", vbNullString)
    End Sub

    Public Sub InformarRechazoEnChar(ByRef Nombre As String, ByRef Detalles As String)
        Call WriteVar(CharPath & Nombre & ".chr", "GUILD", "MotivoRechazo", Detalles)
    End Sub

    '
    'DEFINICION DEL CLAN (CODEX Y NOTICIAS)
    '

    Public Function GetFechaFundacion() As String
        GetFechaFundacion = GetVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "Date")
    End Function

    Public Sub SetCodex(ByVal CodexNumber As Short, ByRef codex As String)
        Call ReplaceInvalidChars(codex)
        codex = Left(codex, CODEXLENGTH)
        Call WriteVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "Codex" & CodexNumber, codex)
    End Sub

    Public Function GetCodex(ByVal CodexNumber As Short) As String
        GetCodex = GetVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "Codex" & CodexNumber)
    End Function


    Public Sub SetURL(ByRef URL As String)
        Call WriteVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "URL", Left(URL, 40))
    End Sub

    Public Function GetURL() As String
        GetURL = GetVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "URL")
    End Function

    Public Sub SetGuildNews(ByRef News As String)
        Call ReplaceInvalidChars(News)

        News = Left(News, NEWSLENGTH)

        Call WriteVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "GuildNews", News)
    End Sub

    Public Function GetGuildNews() As String
        GetGuildNews = GetVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "GuildNews")
    End Function

    Public Sub SetDesc(ByRef desc As String)
        Call ReplaceInvalidChars(desc)
        desc = Left(desc, DESCLENGTH)

        Call WriteVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "Desc", desc)
    End Sub

    Public Function GetDesc() As String
        GetDesc = GetVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "Desc")
    End Function

    '
    '
    'ELECCIONES
    '
    '

    Public Function EleccionesAbiertas() As Boolean
        Dim ee As String
        ee = GetVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "EleccionesAbiertas")
        EleccionesAbiertas = (ee = "1") 'cualquier otra cosa da falso
    End Function

    Public Sub AbrirElecciones()
        Call WriteVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "EleccionesAbiertas", "1")
        Call _
            WriteVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "EleccionesFinalizan", (DateTime.Now.AddDays(1)).ToString())
        Call WriteVar(VOTACIONESFILE, "INIT", "NumVotos", "0")
    End Sub

    Private Sub CerrarElecciones() 'solo pueden cerrarse mediante recuento de votos
        Call WriteVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "EleccionesAbiertas", "0")
        Call WriteVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "EleccionesFinalizan", vbNullString)
        Call Kill(VOTACIONESFILE) 'borramos toda la evidencia ;-)
    End Sub

    Public Sub ContabilizarVoto(ByRef Votante As String, ByRef Votado As String)
        Dim q As Short
        Dim Temps As String

        Temps = GetVar(VOTACIONESFILE, "INIT", "NumVotos")
        q = IIf(IsNumeric(Temps), CShort(Temps), 0)
        Call WriteVar(VOTACIONESFILE, "VOTOS", Votante, Votado)
        Call WriteVar(VOTACIONESFILE, "INIT", "NumVotos", CStr(q + 1))
    End Sub

    Public Function YaVoto(ByRef Votante As Object) As Boolean
        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Votante. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        YaVoto = ((migr_LenB(Trim(GetVar(VOTACIONESFILE, "VOTOS", Votante)))) <> 0)
    End Function

    Private Function ContarVotos(ByRef CantGanadores As Short) As String
        Dim q As Short
        Dim i As Short
        Dim Temps As String
        Dim tempV As String
        Dim d As diccionario
        Dim voteCount As Object

        On Error GoTo errh
        ContarVotos = vbNullString
        CantGanadores = 0
        Temps = GetVar(MEMBERSFILE, "INIT", "NroMembers")
        q = IIf(IsNumeric(Temps), CShort(Temps), 0)
        If q > 0 Then
            'el diccionario tiene clave el elegido y valor la #votos
            d = New diccionario

            For i = 1 To q
                'miembro del clan
                Temps = GetVar(MEMBERSFILE, "MEMBERS", "Member" & i)

                'a quienvoto
                tempV = GetVar(VOTACIONESFILE, "VOTOS", Temps)

                'si voto a alguien contabilizamos el voto
                If migr_LenB(tempV) <> 0 Then
                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto d.At(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto voteCount. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    voteCount = d.At(tempV)
                    If Not voteCount Is Nothing Then 'cuantos votos tiene?
                        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto voteCount. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Call d.AtPut(tempV, CShort(voteCount) + 1)
                    Else
                        Call d.AtPut(tempV, 1)
                    End If
                End If
            Next i

            'quien quedo con mas votos, y cuantos tuvieron esos votos?
            ContarVotos = d.MayorValor(CantGanadores)

            'UPGRADE_NOTE: El objeto d no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            d = Nothing
        End If

        Exit Function
        errh:
        LogError(("clsClan.Contarvotos: " & Err.Description))
        'UPGRADE_NOTE: El objeto d no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
        If Not d Is Nothing Then d = Nothing
        ContarVotos = vbNullString
    End Function

    Public Function RevisarElecciones() As Boolean
        Dim FechaSufragio As Date
        Dim Temps As String
        Dim Ganador As String
        Dim CantGanadores As Short
        Dim list() As String
        Dim i As Integer

        RevisarElecciones = False
        Temps = Trim(GetVar(GUILDINFOFILE, "GUILD" & p_GuildNumber, "EleccionesFinalizan"))

        If Temps = vbNullString Then Exit Function

        If IsDate(Temps) Then
            FechaSufragio = CDate(Temps)
            If FechaSufragio < Now Then 'toca!
                Ganador = ContarVotos(CantGanadores)

                If CantGanadores > 1 Then
                    'empate en la votacion
                    Call _
                        SetGuildNews(
                            "*Empate en la votación. " & Ganador & " con " & CantGanadores &
                            " votos ganaron las elecciones del clan.")
                ElseIf CantGanadores = 1 Then
                    list = GetMemberList()

                    For i = 0 To UBound(list)
                        If Ganador = list(i) Then Exit For
                    Next i

                    If i <= UBound(list) Then
                        Call SetGuildNews("*" & Ganador & " ganó la elección del clan*")
                        Call Me.SetLeader(Ganador)
                        RevisarElecciones = True
                    Else
                        Call _
                            SetGuildNews(
                                "*" & Ganador &
                                " ganó la elección del clan pero abandonó las filas por lo que la votación queda desierta*")
                    End If
                Else
                    Call SetGuildNews("*El período de votación se cerró sin votos*")
                End If

                Call CerrarElecciones()

            End If
        Else
            Call LogError("clsClan.RevisarElecciones: tempS is not Date")
        End If
    End Function

    Public Function GetRelacion(ByVal OtroGuild As Short) As modGuilds.RELACIONES_GUILD
        GetRelacion = p_Relaciones(OtroGuild)
    End Function

    Public Sub SetRelacion(ByVal GuildIndex As Short, ByVal Relacion As modGuilds.RELACIONES_GUILD)
        p_Relaciones(GuildIndex) = Relacion
        Call WriteVar(RELACIONESFILE, "RELACIONES", CStr(GuildIndex), Relacion2String(Relacion))
    End Sub

    Public Sub SetPropuesta(ByVal Tipo As modGuilds.RELACIONES_GUILD, ByVal OtroGuild As Short, ByRef Detalle As String)
        Call WriteVar(PROPUESTASFILE, CStr(OtroGuild), "Detalle", Detalle)
        Call WriteVar(PROPUESTASFILE, CStr(OtroGuild), "Tipo", Relacion2String(Tipo))
        Call WriteVar(PROPUESTASFILE, CStr(OtroGuild), "Pendiente", "1")
        Select Case Tipo
            Case modGuilds.RELACIONES_GUILD.ALIADOS
                p_PropuestasDeAlianza.Add(OtroGuild)
            Case modGuilds.RELACIONES_GUILD.PAZ
                p_PropuestasDePaz.Add(OtroGuild)
        End Select
    End Sub

    Public Sub AnularPropuestas(ByVal OtroGuild As Short)
        Dim i As Short

        Call WriteVar(PROPUESTASFILE, CStr(OtroGuild), "Detalle", vbNullString)
        Call WriteVar(PROPUESTASFILE, CStr(OtroGuild), "Pendiente", "0")
        For i = 1 To p_PropuestasDePaz.Count()
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_PropuestasDePaz.Item(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            If p_PropuestasDePaz.Item(i) = OtroGuild Then p_PropuestasDePaz.Remove((i))
            Exit Sub
        Next i
        For i = 1 To p_PropuestasDeAlianza.Count()
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_PropuestasDeAlianza.Item(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            If p_PropuestasDeAlianza.Item(i) = OtroGuild Then p_PropuestasDeAlianza.Remove((i))
            Exit Sub
        Next i
    End Sub

    Public Function GetPropuesta(ByVal OtroGuild As Short, ByRef Tipo As modGuilds.RELACIONES_GUILD) As String
        'trae la solicitd que haya, no valida si es actual o de que tipo es
        GetPropuesta = GetVar(PROPUESTASFILE, CStr(OtroGuild), "Detalle")
        Tipo = String2Relacion(GetVar(PROPUESTASFILE, CStr(OtroGuild), "Tipo"))
    End Function

    Public Function HayPropuesta(ByVal OtroGuild As Short, ByRef Tipo As modGuilds.RELACIONES_GUILD) As Boolean
        Dim i As Short

        HayPropuesta = False
        Select Case Tipo
            Case modGuilds.RELACIONES_GUILD.ALIADOS
                For i = 1 To p_PropuestasDeAlianza.Count()
                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_PropuestasDeAlianza.Item(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If p_PropuestasDeAlianza.Item(i) = OtroGuild Then
                        HayPropuesta = True
                    End If
                Next i
            Case modGuilds.RELACIONES_GUILD.PAZ
                For i = 1 To p_PropuestasDePaz.Count()
                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_PropuestasDePaz.Item(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If p_PropuestasDePaz.Item(i) = OtroGuild Then
                        HayPropuesta = True
                    End If
                Next i
            Case modGuilds.RELACIONES_GUILD.GUERRA

        End Select
    End Function

    'Public Function GetEnemy(ByVal EnemyIndex As Integer) As String
    '    GetEnemy = GetVar(ENEMIESFILE, "ENEMYS", "ENEMY" & EnemyIndex)
    'End Function

    'Public Function GetAllie(ByVal AllieIndex As Integer) As String
    '    GetAllie = GetVar(ALLIESFILE, "ALLIES", "ALLIE" & AllieIndex)
    'End Function


    '
    'ITERADORES
    '

    Public Function Iterador_ProximaPropuesta(ByVal Tipo As modGuilds.RELACIONES_GUILD) As Short

        Iterador_ProximaPropuesta = 0
        Select Case Tipo
            Case modGuilds.RELACIONES_GUILD.ALIADOS
                If p_IteradorPropuesta < p_PropuestasDeAlianza.Count() Then
                    p_IteradorPropuesta = p_IteradorPropuesta + 1
                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_PropuestasDeAlianza.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Iterador_ProximaPropuesta = p_PropuestasDeAlianza.Item(p_IteradorPropuesta)
                End If

                If p_IteradorPropuesta >= p_PropuestasDeAlianza.Count() Then
                    p_IteradorPropuesta = 0
                End If
            Case modGuilds.RELACIONES_GUILD.PAZ
                If p_IteradorPropuesta < p_PropuestasDePaz.Count() Then
                    p_IteradorPropuesta = p_IteradorPropuesta + 1
                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_PropuestasDePaz.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Iterador_ProximaPropuesta = p_PropuestasDePaz.Item(p_IteradorPropuesta)
                End If

                If p_IteradorPropuesta >= p_PropuestasDePaz.Count() Then
                    p_IteradorPropuesta = 0
                End If
        End Select
    End Function

    Public Function m_Iterador_ProximoUserIndex() As Short

        If p_IteradorOnlineMembers < p_OnlineMembers.Count() Then
            p_IteradorOnlineMembers = p_IteradorOnlineMembers + 1
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_OnlineMembers.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            m_Iterador_ProximoUserIndex = p_OnlineMembers.Item(p_IteradorOnlineMembers)
        Else
            p_IteradorOnlineMembers = 0
            m_Iterador_ProximoUserIndex = 0
        End If
    End Function

    Public Function Iterador_ProximoGM() As Short

        If p_IteradorOnlineGMs < p_GMsOnline.Count() Then
            p_IteradorOnlineGMs = p_IteradorOnlineGMs + 1
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_GMsOnline.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Iterador_ProximoGM = p_GMsOnline.Item(p_IteradorOnlineGMs)
        Else
            p_IteradorOnlineGMs = 0
            Iterador_ProximoGM = 0
        End If
    End Function

    Public Function Iterador_ProximaRelacion(ByVal r As modGuilds.RELACIONES_GUILD) As Short

        While p_IteradorRelaciones < UBound(p_Relaciones)

            p_IteradorRelaciones = p_IteradorRelaciones + 1
            If p_Relaciones(p_IteradorRelaciones) = r Then
                Iterador_ProximaRelacion = p_IteradorRelaciones
                Exit Function
            End If
        End While

        If p_IteradorRelaciones >= UBound(p_Relaciones) Then
            p_IteradorRelaciones = 0
        End If
    End Function
    '
    '
    '


    '
    'ADMINISTRATIVAS
    '

    Public Sub ConectarGM(ByVal UserIndex As Short)
        p_GMsOnline.Add(UserIndex)
    End Sub

    Public Sub DesconectarGM(ByVal UserIndex As Short)
        Dim i As Short
        For i = 1 To p_GMsOnline.Count()
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto p_GMsOnline.Item(i). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            If p_GMsOnline.Item(i) = UserIndex Then
                p_GMsOnline.Remove((i))
            End If
        Next i
    End Sub


    '
    'VARIAS, EXTRAS Y DEMASES
    '

    Private Sub ReplaceInvalidChars(ByRef S As String)
        If migr_InStrB(S, Chr(13)) <> 0 Then
            S = Replace(S, Chr(13), vbNullString)
        End If
        If migr_InStrB(S, Chr(10)) <> 0 Then
            S = Replace(S, Chr(10), vbNullString)
        End If
        If migr_InStrB(S, "¬") <> 0 Then
            S = Replace(S, "¬", vbNullString) 'morgo usaba esto como "separador"
        End If
    End Sub
End Class
