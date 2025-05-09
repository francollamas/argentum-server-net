Option Strict Off
Option Explicit On
Friend Class clsParty
    'UPGRADE_WARNING: El límite inferior de la matriz p_members ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    Private ReadOnly p_members(PARTY_MAXMEMBERS) As tPartyMember
    'miembros

    Private p_expTotal As Integer
    'Estadistica :D

    Private p_Fundador As Short
    'el creador

    Private p_CantMiembros As Short
    'cantidad de miembros

    Private p_SumaNivelesElevados As Single
    'suma de todos los niveles elevados a la ExponenteNivelParty > Esta variable se usa para calcular la experiencia repartida en la Party.

    'datos en los pjs: | indexParty(indice en p_members), partyLeader(userindex del lider) |

    'Constructor de clase
    'UPGRADE_NOTE: Class_Initialize se actualizó a Class_Initialize_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Public Sub Class_Initialize_Renamed()
        '***************************************************
        'Author: Unknown
        'Last Modification: 07/04/08
        'Last Modification By: Marco Vanotti (MarKoxX)
        ' - 09/29/07 p_SumaNiveles added (Tavo)
        ' - 07/04/08 p_SumaNiveles changed to p_SumaNivelesElevados (MarKoxX)
        '***************************************************
        p_expTotal = 0
        p_CantMiembros = 0
        p_SumaNivelesElevados = 0
    End Sub

    Public Sub New()
        MyBase.New()
        Class_Initialize_Renamed()
    End Sub

    'Destructor de clase
    'UPGRADE_NOTE: Class_Terminate se actualizó a Class_Terminate_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Public Sub Class_Terminate_Renamed()
    End Sub

    Protected Overrides Sub Finalize()
        Class_Terminate_Renamed()
        MyBase.Finalize()
    End Sub

    ''
    ' Sets the new p_sumaniveleselevados to the party.
    '
    ' @param lvl Specifies reference to user level
    ' @remarks When a user level up and he is in a party, we update p_sumaNivelesElavados so the formula still works.
    Public Sub UpdateSumaNivelesElevados(Lvl As Short)
        '*************************************************
        'Author: Marco Vanotti (MarKoxX)
        'Last modified: 11/24/09
        '11/24/09: Pato - Change the exponent to a variable with the exponent
        '*************************************************
        p_SumaNivelesElevados = p_SumaNivelesElevados - ((Lvl - 1)^ExponenteNivelParty) + Lvl^ExponenteNivelParty
    End Sub

    Public Function MiExperiencia(UserIndex As Short) As Integer
        '***************************************************
        'Author: Unknown
        'Last Modification: 11/27/09
        'Last Modification By: Budi
        ' - 09/29/07 Experience is round to the biggest number less than that number
        ' - 09/29/07 Now experience is a real-number
        ' - 11/27/09 Arreglé el Out of Range.
        '***************************************************
        'Me dice cuanta experiencia tengo colectada ya en la party
        Dim i As Short
        i = 1

        While i <= PARTY_MAXMEMBERS And p_members(i).UserIndex <> UserIndex
            i = i + 1
        End While

        If i <= PARTY_MAXMEMBERS Then
            MiExperiencia = Fix(p_members(i).Experiencia)
        Else 'esto no deberia pasar :p
            MiExperiencia = - 1
        End If
    End Function

    Public Sub ObtenerExito(ExpGanada As Integer, mapa As Short, ByRef X As Short, ByRef Y As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 07/04/08
        'Last Modification By: Marco Vanotti (MarKoxX)
        ' - 09/29/07 New formula for calculating the experience point of each user
        ' - 09/29/07 Experience is round to the biggest number less than that number
        ' - 09/29/07 Now experience is a real-number
        ' - 04/04/08 Ahora antes de calcular la experiencia a X usuario se fija si ese usuario existe (MarKoxX)
        ' - 07/04/08 New formula to calculate Experience for each user. (MarKoxX)
        '***************************************************
        'Se produjo un evento que da experiencia en la wp referenciada
        Dim i As Short
        Dim UI As Short
        Dim expThisUser As Double

        p_expTotal = p_expTotal + ExpGanada

        For i = 1 To PARTY_MAXMEMBERS
            UI = p_members(i).UserIndex
            If UI > 0 Then
                ' Formula: Exp* (Nivel ^ ExponenteNivelParty) / sumadeNivelesElevados
                expThisUser =
                    CDbl(
                        ExpGanada*(UserList(p_members(i).UserIndex).Stats.ELV^ExponenteNivelParty)/p_SumaNivelesElevados)

                If mapa = UserList(UI).Pos.map And UserList(UI).flags.Muerto = 0 Then
                    If Distance(UserList(UI).Pos.X, UserList(UI).Pos.Y, X, Y) <= PARTY_MAXDISTANCIA Then
                        p_members(i).Experiencia = p_members(i).Experiencia + expThisUser
                        If p_members(i).Experiencia < 0 Then
                            p_members(i).Experiencia = 0
                        End If
                        If PARTY_EXPERIENCIAPORGOLPE Then
                            UserList(UI).Stats.Exp = UserList(UI).Stats.Exp + Fix(expThisUser)
                            If UserList(UI).Stats.Exp > MAXEXP Then UserList(UI).Stats.Exp = MAXEXP
                            Call CheckUserLevel(UI)
                            Call WriteUpdateUserStats(UI)
                        End If
                    End If
                End If
            End If
        Next i
    End Sub

    Public Sub MandarMensajeAConsola(texto As String, Sender As String)
        'feo feo, muy feo acceder a senddata desde aca, pero BUEEEEEEEEEEE...
        Dim i As Short

        For i = 1 To PARTY_MAXMEMBERS
            If p_members(i).UserIndex > 0 Then
                Call _
                    WriteConsoleMsg(p_members(i).UserIndex, " [" & Sender & "] " & texto,
                                    FontTypeNames.FONTTYPE_PARTY)
            End If
        Next i
    End Sub

    Public Function EsPartyLeader(UserIndex As Short) As Boolean
        EsPartyLeader = (UserIndex = p_Fundador)
    End Function

    Public Function NuevoMiembro(UserIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: 07/04/08
        'Last Modification By: Marco Vanotti (MarKoxX)
        ' - 09/29/07 There is no level prohibition (Tavo)
        ' - 07/04/08 Added const ExponenteNivelParty. (MarKoxX)
        '***************************************************

        Dim i As Short
        i = 1
        While i <= PARTY_MAXMEMBERS And p_members(i).UserIndex > 0
            i = i + 1
        End While

        If i <= PARTY_MAXMEMBERS Then
            p_members(i).Experiencia = 0
            p_members(i).UserIndex = UserIndex
            NuevoMiembro = True
            p_CantMiembros = p_CantMiembros + 1
            p_SumaNivelesElevados = p_SumaNivelesElevados + (UserList(UserIndex).Stats.ELV^ExponenteNivelParty)
        Else
            NuevoMiembro = False
        End If
    End Function

    Public Function SaleMiembro(UserIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: 07/04/08
        'Last Modification By: Marco Vanotti (MarKoxX)
        ' - 09/29/07 Experience is round to the biggest number less than that number
        ' - 09/29/07 Now experience is a real-number (Tavo)
        ' - 07/04/08 Added const ExponenteNivelParty. (MarKoxX)
        '11/03/2010: ZaMa - Ahora no le dice al lider que salio de su propia party, y optimice con with.
        '***************************************************
        'el valor de retorno representa si se disuelve la party
        Dim i As Short
        Dim j As Short
        Dim MemberIndex As Short

        i = 1
        SaleMiembro = False
        While i <= PARTY_MAXMEMBERS And p_members(i).UserIndex <> UserIndex
            i = i + 1
        End While

        If i = 1 Then
            'sale el founder, la party se disuelve
            SaleMiembro = True
            Call MandarMensajeAConsola("El líder disuelve la party.", "Servidor")

            For j = PARTY_MAXMEMBERS To 1 Step - 1

                With p_members(j)

                    If .UserIndex > 0 Then

                        ' No envia el mensaje al lider
                        If j <> 1 Then
                            Call _
                                WriteConsoleMsg(.UserIndex,
                                                "Abandonas la party liderada por " &
                                                UserList(p_members(1).UserIndex).name & ".",
                                                FontTypeNames.FONTTYPE_PARTY)
                        End If

                        Call _
                            WriteConsoleMsg(.UserIndex,
                                            "Durante la misma has conseguido " & CStr(Fix(.Experiencia)) &
                                            " puntos de experiencia.", FontTypeNames.FONTTYPE_PARTY)

                        If Not PARTY_EXPERIENCIAPORGOLPE Then
                            UserList(.UserIndex).Stats.Exp = UserList(.UserIndex).Stats.Exp + Fix(.Experiencia)
                            If UserList(.UserIndex).Stats.Exp > MAXEXP Then UserList(.UserIndex).Stats.Exp = MAXEXP
                            Call CheckUserLevel(.UserIndex)
                            Call WriteUpdateUserStats(.UserIndex)
                        End If

                        Call MandarMensajeAConsola(UserList(.UserIndex).name & " abandona la party.", "Servidor")

                        UserList(.UserIndex).PartyIndex = 0
                        p_CantMiembros = p_CantMiembros - 1
                        p_SumaNivelesElevados = p_SumaNivelesElevados -
                                                (UserList(UserIndex).Stats.ELV^ExponenteNivelParty)
                        .UserIndex = 0
                        .Experiencia = 0

                    End If

                End With

            Next j
        Else
            If i <= PARTY_MAXMEMBERS Then

                MemberIndex = p_members(i).UserIndex

                With UserList(MemberIndex)
                    If Not PARTY_EXPERIENCIAPORGOLPE Then
                        .Stats.Exp = .Stats.Exp + Fix(p_members(i).Experiencia)
                        If .Stats.Exp > MAXEXP Then .Stats.Exp = MAXEXP

                        Call CheckUserLevel(MemberIndex)
                        Call WriteUpdateUserStats(MemberIndex)
                    End If

                    Call MandarMensajeAConsola(.name & " abandona la party.", "Servidor")
                    'TODO: Revisar que esto este bien, y no este faltando/sobrando un mensaje, ahora solo los estoy corrigiendo
                    Call _
                        WriteConsoleMsg(MemberIndex,
                                        "Durante la misma has conseguido " & CStr(Fix(p_members(i).Experiencia)) &
                                        " puntos de experiencia.", FontTypeNames.FONTTYPE_PARTY)

                    p_CantMiembros = p_CantMiembros - 1
                    p_SumaNivelesElevados = p_SumaNivelesElevados - (UserList(UserIndex).Stats.ELV^ExponenteNivelParty)
                    MemberIndex = 0
                    p_members(i).Experiencia = 0
                    p_members(i).UserIndex = 0
                    CompactMemberList()
                End With
            End If
        End If
    End Function

    Public Function HacerLeader(UserIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: 09/29/07
        'Last Modification By: Lucas Tavolaro Ortiz (Tavo)
        ' - 09/29/07 There is no level prohibition
        '***************************************************
        Dim i As Short
        Dim OldLeader As Short
        Dim oldExp As Double
        Dim UserIndexIndex As Short

        UserIndexIndex = 0
        HacerLeader = True

        For i = 1 To PARTY_MAXMEMBERS
            If p_members(i).UserIndex > 0 Then
                If p_members(i).UserIndex = UserIndex Then
                    UserIndexIndex = i
                End If
            End If
        Next i

        If Not HacerLeader Then Exit Function

        If UserIndexIndex = 0 Then
            'catastrofe! esto no deberia pasar nunca! pero como es AO.... :p
            Call LogError("INCONSISTENCIA DE PARTIES")
            Call _
                SendData(SendTarget.ToAdmins, 0,
                         PrepareMessageConsoleMsg(
                             "¡¡¡Inconsistencia de parties en HACERLEADER (UII = 0), AVISE A UN PROGRAMADOR ESTO ES UNA CATASTROFE!!!!",
                             FontTypeNames.FONTTYPE_GUILD))
            HacerLeader = False
            Exit Function
        End If


        'aca esta todo bien y doy vuelta las collections
        OldLeader = p_members(1).UserIndex
        oldExp = p_members(1).Experiencia

        p_members(1).UserIndex = p_members(UserIndexIndex).UserIndex _
        'que en realdiad es el userindex, pero no quiero inconsistencias moviendo experiencias
        p_members(1).Experiencia = p_members(UserIndexIndex).Experiencia

        p_members(UserIndexIndex).UserIndex = OldLeader
        p_members(UserIndexIndex).Experiencia = oldExp

        p_Fundador = p_members(1).UserIndex

        'no need to compact
    End Function


    Public Sub ObtenerMiembrosOnline(ByRef MemberList() As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 09/29/07
        'Last Modification By: Marco Vanotti (MarKoxX)
        ' - 09/29/07 Experience is round to the biggest number less than that number
        ' - 09/29/07 Now experience is a real-number (Tavo)
        ' - 08/18/08 Now TotalExperience is fixed (MarKoxX)
        ' - 11/27/09 Rehice la función, ahora devuelve el array con los UI online (Budi)
        '***************************************************

        Dim i As Short

        For i = 1 To PARTY_MAXMEMBERS
            If p_members(i).UserIndex > 0 Then
                MemberList(i) = p_members(i).UserIndex
            End If
        Next i
    End Sub

    Public Function ObtenerExperienciaTotal() As Integer
        '***************************************************
        'Author: Budi
        'Last Modification: 11/27/09
        'Retrieves the total experience acumulated in the party
        '***************************************************
        ObtenerExperienciaTotal = p_expTotal
    End Function

    Public Function PuedeEntrar(UserIndex As Short, ByRef razon As String) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: 09/29/07
        'Last Modification By: Lucas Tavolaro Ortiz (Tavo)
        ' - 09/29/07 There is no level prohibition
        '***************************************************
        'DEFINE LAS REGLAS DEL JUEGO PARA DEJAR ENTRAR A MIEMBROS
        Dim esArmada As Boolean
        Dim esCaos As Boolean
        Dim MyLevel As Short
        Dim i As Short
        Dim rv As Boolean
        Dim UI As Short

        rv = True
        esArmada = (UserList(UserIndex).Faccion.ArmadaReal = 1)
        esCaos = (UserList(UserIndex).Faccion.FuerzasCaos = 1)
        MyLevel = UserList(UserIndex).Stats.ELV

        rv = Distancia(UserList(p_members(1).UserIndex).Pos, UserList(UserIndex).Pos) <= MAXDISTANCIAINGRESOPARTY
        If rv Then
            rv = (p_members(PARTY_MAXMEMBERS).UserIndex = 0)
            If rv Then
                For i = 1 To PARTY_MAXMEMBERS
                    UI = p_members(i).UserIndex
                    'pongo los casos que evitarian que pueda entrar
                    'aspirante armada en party crimi
                    If UI > 0 Then
                        If esArmada And criminal(UI) Then
                            razon = "Los miembros del ejército real no entran a una party con criminales."
                            rv = False
                        End If
                        'aspirante caos en party ciuda
                        If esCaos And Not criminal(UI) Then
                            razon = "Los miembros de la legión oscura no entran a una party con ciudadanos."
                            rv = False
                        End If
                        'aspirante crimi en party armada
                        If UserList(UI).Faccion.ArmadaReal = 1 And criminal(UserIndex) Then
                            razon = "Los criminales no entran a parties con miembros del ejército real."
                            rv = False
                        End If
                        'aspirante ciuda en party caos
                        If UserList(UI).Faccion.FuerzasCaos = 1 And Not criminal(UserIndex) Then
                            razon = "Los ciudadanos no entran a parties con miembros de la legión oscura."
                            rv = False
                        End If

                        If Not rv Then Exit For 'violate una programacion estructurada
                    End If
                Next i
            Else
                razon = "La mayor cantidad de miembros es " & PARTY_MAXMEMBERS
            End If
        Else
            '¿Con o sin nombre?
            razon = "El usuario " & UserList(UserIndex).name & " se encuentra muy lejos."
        End If

        PuedeEntrar = rv
    End Function


    Public Sub FlushExperiencia()
        '***************************************************
        'Author: Unknown
        'Last Modification: 09/29/07
        'Last Modification By: Lucas Tavolaro Ortiz (Tavo)
        ' - 09/29/07 Experience is round to the biggest number less than that number
        ' - 09/29/07 Now experience is a real-number
        '***************************************************
        'esta funcion se invoca frente a cerradas del servidor. Flushea la experiencia
        'acumulada a los usuarios.

        Dim i As Short
        If Not PARTY_EXPERIENCIAPORGOLPE Then 'esto sirve SOLO cuando acumulamos la experiencia!
            For i = 1 To PARTY_MAXMEMBERS
                If p_members(i).UserIndex > 0 Then
                    If p_members(i).Experiencia > 0 Then
                        UserList(p_members(i).UserIndex).Stats.Exp = UserList(p_members(i).UserIndex).Stats.Exp +
                                                                     Fix(p_members(i).Experiencia)
                        If UserList(p_members(i).UserIndex).Stats.Exp > MAXEXP Then _
                            UserList(p_members(i).UserIndex).Stats.Exp = MAXEXP
                        Call CheckUserLevel(p_members(i).UserIndex)
                    Else
                        If _
                            Math.Abs(UserList(p_members(i).UserIndex).Stats.Exp) >
                            Math.Abs(Fix(p_members(i).Experiencia)) Then
                            UserList(p_members(i).UserIndex).Stats.Exp = UserList(p_members(i).UserIndex).Stats.Exp +
                                                                         Fix(p_members(i).Experiencia)
                        Else
                            UserList(p_members(i).UserIndex).Stats.Exp = 0
                        End If
                    End If
                    p_members(i).Experiencia = 0
                    Call WriteUpdateUserStats(p_members(i).UserIndex)
                End If
            Next i
        End If
    End Sub

    Private Sub CompactMemberList()
        Dim i As Short
        Dim freeIndex As Short
        i = 1
        While i <= PARTY_MAXMEMBERS
            If p_members(i).UserIndex = 0 And freeIndex = 0 Then
                freeIndex = i
            ElseIf p_members(i).UserIndex > 0 And freeIndex > 0 Then
                p_members(freeIndex).Experiencia = p_members(i).Experiencia
                p_members(freeIndex).UserIndex = p_members(i).UserIndex
                p_members(i).UserIndex = 0
                p_members(i).Experiencia = 0
                'muevo el de la pos i a freeindex
                i = freeIndex
                freeIndex = 0
            End If
            i = i + 1
        End While
    End Sub

    Public Function CantMiembros() As Short
        CantMiembros = p_CantMiembros
    End Function
End Class
