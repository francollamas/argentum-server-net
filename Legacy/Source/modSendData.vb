Option Strict Off
Option Explicit On
Module modSendData
    '**************************************************************
    ' SendData.bas - Has all methods to send data to different user groups.
    ' Makes use of the modAreas module.
    '
    ' Implemented by Juan Martín Sotuyo Dodero (Maraxus) (juansotuyo@gmail.com)
    '**************************************************************

    ''
    ' Contains all methods to send data to different user groups.
    ' Makes use of the modAreas module.
    '
    ' @author Juan Martín Sotuyo Dodero (Maraxus) juansotuyo@gmail.com
    ' @version 1.0.0
    ' @date 20070107

    Public Enum SendTarget
        ToAll = 1
        toMap
        ToPCArea
        ToAllButIndex
        ToMapButIndex
        ToGM
        ToNPCArea
        ToGuildMembers
        ToAdmins
        ToPCAreaButIndex
        ToAdminsAreaButConsejeros
        ToDiosesYclan
        ToConsejo
        ToClanArea
        ToConsejoCaos
        ToRolesMasters
        ToDeadArea
        ToCiudadanos
        ToCriminales
        ToPartyArea
        ToReal
        ToCaos
        ToCiudadanosYRMs
        ToCriminalesYRMs
        ToRealYRMs
        ToCaosYRMs
        ToHigherAdmins
        ToGMsAreaButRmsOrCounselors
        ToUsersAreaButGMs
        ToUsersAndRmsAndCounselorsAreaButGMs
    End Enum

    Public Sub SendData(sndRoute As SendTarget, sndIndex As Short, sndData As String)
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus) - Rewrite of original
        'Last Modify Date: 01/08/2007
        'Last modified by: (liquid)
        '**************************************************************
        Try
            Dim LoopC As Integer
            Dim Map As Short

            Select Case sndRoute
                Case SendTarget.ToPCArea
                    Call SendToUserArea(sndIndex, sndData)
                    Exit Sub

                Case SendTarget.ToAdmins
                    For LoopC = 1 To LastUser
                        If UserList(LoopC).ConnID <> - 1 Then
                            If _
                                UserList(LoopC).flags.Privilegios And
                                (PlayerType.Admin Or PlayerType.Dios Or
                                 PlayerType.SemiDios Or PlayerType.Consejero) Then
                                Call EnviarDatosASlot(LoopC, sndData)
                            End If
                        End If
                    Next LoopC
                    Exit Sub

                Case SendTarget.ToAll
                    For LoopC = 1 To LastUser
                        If UserList(LoopC).ConnID <> - 1 Then
                            If UserList(LoopC).flags.UserLogged Then 'Esta logeado como usuario?
                                Call EnviarDatosASlot(LoopC, sndData)
                            End If
                        End If
                    Next LoopC
                    Exit Sub

                Case SendTarget.ToAllButIndex
                    For LoopC = 1 To LastUser
                        If (UserList(LoopC).ConnID <> - 1) And (LoopC <> sndIndex) Then
                            If UserList(LoopC).flags.UserLogged Then 'Esta logeado como usuario?
                                Call EnviarDatosASlot(LoopC, sndData)
                            End If
                        End If
                    Next LoopC
                    Exit Sub

                Case SendTarget.toMap
                    Call SendToMap(sndIndex, sndData)
                    Exit Sub

                Case SendTarget.ToMapButIndex
                    Call SendToMapButIndex(sndIndex, sndData)
                    Exit Sub

                Case SendTarget.ToGuildMembers
                    LoopC = m_Iterador_ProximoUserIndex(sndIndex)
                    While LoopC > 0
                        If (UserList(LoopC).ConnID <> - 1) Then
                            Call EnviarDatosASlot(LoopC, sndData)
                        End If
                        LoopC = m_Iterador_ProximoUserIndex(sndIndex)
                    End While
                    Exit Sub

                Case SendTarget.ToDeadArea
                    Call SendToDeadUserArea(sndIndex, sndData)
                    Exit Sub

                Case SendTarget.ToPCAreaButIndex
                    Call SendToUserAreaButindex(sndIndex, sndData)
                    Exit Sub

                Case SendTarget.ToClanArea
                    Call SendToUserGuildArea(sndIndex, sndData)
                    Exit Sub

                Case SendTarget.ToPartyArea
                    Call SendToUserPartyArea(sndIndex, sndData)
                    Exit Sub

                Case SendTarget.ToAdminsAreaButConsejeros
                    Call SendToAdminsButConsejerosArea(sndIndex, sndData)
                    Exit Sub

                Case SendTarget.ToNPCArea
                    Call SendToNpcArea(sndIndex, sndData)
                    Exit Sub

                Case SendTarget.ToDiosesYclan
                    LoopC = m_Iterador_ProximoUserIndex(sndIndex)
                    While LoopC > 0
                        If (UserList(LoopC).ConnID <> - 1) Then
                            Call EnviarDatosASlot(LoopC, sndData)
                        End If
                        LoopC = m_Iterador_ProximoUserIndex(sndIndex)
                    End While

                    LoopC = Iterador_ProximoGM(sndIndex)
                    While LoopC > 0
                        If (UserList(LoopC).ConnID <> - 1) Then
                            Call EnviarDatosASlot(LoopC, sndData)
                        End If
                        LoopC = Iterador_ProximoGM(sndIndex)
                    End While

                    Exit Sub

                Case SendTarget.ToConsejo
                    For LoopC = 1 To LastUser
                        If (UserList(LoopC).ConnID <> - 1) Then
                            If UserList(LoopC).flags.Privilegios And PlayerType.RoyalCouncil Then
                                Call EnviarDatosASlot(LoopC, sndData)
                            End If
                        End If
                    Next LoopC
                    Exit Sub

                Case SendTarget.ToConsejoCaos
                    For LoopC = 1 To LastUser
                        If (UserList(LoopC).ConnID <> - 1) Then
                            If UserList(LoopC).flags.Privilegios And PlayerType.ChaosCouncil Then
                                Call EnviarDatosASlot(LoopC, sndData)
                            End If
                        End If
                    Next LoopC
                    Exit Sub

                Case SendTarget.ToRolesMasters
                    For LoopC = 1 To LastUser
                        If (UserList(LoopC).ConnID <> - 1) Then
                            If UserList(LoopC).flags.Privilegios And PlayerType.RoleMaster Then
                                Call EnviarDatosASlot(LoopC, sndData)
                            End If
                        End If
                    Next LoopC
                    Exit Sub

                Case SendTarget.ToCiudadanos
                    For LoopC = 1 To LastUser
                        If (UserList(LoopC).ConnID <> - 1) Then
                            If Not criminal(LoopC) Then
                                Call EnviarDatosASlot(LoopC, sndData)
                            End If
                        End If
                    Next LoopC
                    Exit Sub

                Case SendTarget.ToCriminales
                    For LoopC = 1 To LastUser
                        If (UserList(LoopC).ConnID <> - 1) Then
                            If criminal(LoopC) Then
                                Call EnviarDatosASlot(LoopC, sndData)
                            End If
                        End If
                    Next LoopC
                    Exit Sub

                Case SendTarget.ToReal
                    For LoopC = 1 To LastUser
                        If (UserList(LoopC).ConnID <> - 1) Then
                            If UserList(LoopC).Faccion.ArmadaReal = 1 Then
                                Call EnviarDatosASlot(LoopC, sndData)
                            End If
                        End If
                    Next LoopC
                    Exit Sub

                Case SendTarget.ToCaos
                    For LoopC = 1 To LastUser
                        If (UserList(LoopC).ConnID <> - 1) Then
                            If UserList(LoopC).Faccion.FuerzasCaos = 1 Then
                                Call EnviarDatosASlot(LoopC, sndData)
                            End If
                        End If
                    Next LoopC
                    Exit Sub

                Case SendTarget.ToCiudadanosYRMs
                    For LoopC = 1 To LastUser
                        If (UserList(LoopC).ConnID <> - 1) Then
                            If _
                                Not criminal(LoopC) Or
                                (UserList(LoopC).flags.Privilegios And PlayerType.RoleMaster) <> 0 Then
                                Call EnviarDatosASlot(LoopC, sndData)
                            End If
                        End If
                    Next LoopC
                    Exit Sub

                Case SendTarget.ToCriminalesYRMs
                    For LoopC = 1 To LastUser
                        If (UserList(LoopC).ConnID <> - 1) Then
                            If _
                                criminal(LoopC) Or
                                (UserList(LoopC).flags.Privilegios And PlayerType.RoleMaster) <> 0 Then
                                Call EnviarDatosASlot(LoopC, sndData)
                            End If
                        End If
                    Next LoopC
                    Exit Sub

                Case SendTarget.ToRealYRMs
                    For LoopC = 1 To LastUser
                        If (UserList(LoopC).ConnID <> - 1) Then
                            If _
                                UserList(LoopC).Faccion.ArmadaReal = 1 Or
                                (UserList(LoopC).flags.Privilegios And PlayerType.RoleMaster) <> 0 Then
                                Call EnviarDatosASlot(LoopC, sndData)
                            End If
                        End If
                    Next LoopC
                    Exit Sub

                Case SendTarget.ToCaosYRMs
                    For LoopC = 1 To LastUser
                        If (UserList(LoopC).ConnID <> - 1) Then
                            If _
                                UserList(LoopC).Faccion.FuerzasCaos = 1 Or
                                (UserList(LoopC).flags.Privilegios And PlayerType.RoleMaster) <> 0 Then
                                Call EnviarDatosASlot(LoopC, sndData)
                            End If
                        End If
                    Next LoopC
                    Exit Sub

                Case SendTarget.ToHigherAdmins
                    For LoopC = 1 To LastUser
                        If UserList(LoopC).ConnID <> - 1 Then
                            If _
                                UserList(LoopC).flags.Privilegios And
                                (PlayerType.Admin Or PlayerType.Dios) Then
                                Call EnviarDatosASlot(LoopC, sndData)
                            End If
                        End If
                    Next LoopC
                    Exit Sub

                Case SendTarget.ToGMsAreaButRmsOrCounselors
                    Call SendToGMsAreaButRmsOrCounselors(sndIndex, sndData)
                    Exit Sub

                Case SendTarget.ToUsersAreaButGMs
                    Call SendToUsersAreaButGMs(sndIndex, sndData)
                    Exit Sub
                Case SendTarget.ToUsersAndRmsAndCounselorsAreaButGMs
                    Call SendToUsersAndRmsAndCounselorsAreaButGMs(sndIndex, sndData)
                    Exit Sub
            End Select

        Catch ex As Exception
            Console.WriteLine("Error in SendData: " & ex.Message)
        End Try
    End Sub

    Private Sub SendToUserArea(UserIndex As Short, sdData As String)
        '**************************************************************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: Unknow
        '
        '**************************************************************
        Dim LoopC As Integer
        Dim tempIndex As Short

        Dim Map As Short
        Dim AreaX As Short
        Dim AreaY As Short

        Map = UserList(UserIndex).Pos.Map
        AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
        AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY

        If Not MapaValido(Map) Then Exit Sub

        For LoopC = 1 To ConnGroups(Map).CountEntrys
            tempIndex = ConnGroups(Map).UserEntrys(LoopC)

            If UserList(tempIndex).AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
                If UserList(tempIndex).AreasInfo.AreaReciveY And AreaY Then
                    If UserList(tempIndex).ConnIDValida Then
                        Call EnviarDatosASlot(tempIndex, sdData)
                    End If
                End If
            End If
        Next LoopC
    End Sub

    Private Sub SendToUserAreaButindex(UserIndex As Short, sdData As String)
        '**************************************************************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: Unknow
        '
        '**************************************************************
        Dim LoopC As Integer
        Dim TempInt As Short
        Dim tempIndex As Short

        Dim Map As Short
        Dim AreaX As Short
        Dim AreaY As Short

        Map = UserList(UserIndex).Pos.Map
        AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
        AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY

        If Not MapaValido(Map) Then Exit Sub

        For LoopC = 1 To ConnGroups(Map).CountEntrys
            tempIndex = ConnGroups(Map).UserEntrys(LoopC)

            TempInt = UserList(tempIndex).AreasInfo.AreaReciveX And AreaX
            If TempInt Then 'Esta en el area?
                TempInt = UserList(tempIndex).AreasInfo.AreaReciveY And AreaY
                If TempInt Then
                    If tempIndex <> UserIndex Then
                        If UserList(tempIndex).ConnIDValida Then
                            Call EnviarDatosASlot(tempIndex, sdData)
                        End If
                    End If
                End If
            End If
        Next LoopC
    End Sub

    Private Sub SendToDeadUserArea(UserIndex As Short, sdData As String)
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modify Date: Unknow
        '
        '**************************************************************
        Dim LoopC As Integer
        Dim tempIndex As Short

        Dim Map As Short
        Dim AreaX As Short
        Dim AreaY As Short

        Map = UserList(UserIndex).Pos.Map
        AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
        AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY

        If Not MapaValido(Map) Then Exit Sub

        For LoopC = 1 To ConnGroups(Map).CountEntrys
            tempIndex = ConnGroups(Map).UserEntrys(LoopC)

            If UserList(tempIndex).AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
                If UserList(tempIndex).AreasInfo.AreaReciveY And AreaY Then
                    'Dead and admins read
                    If _
                        UserList(tempIndex).ConnIDValida = True And
                        (UserList(tempIndex).flags.Muerto = 1 Or
                         (UserList(tempIndex).flags.Privilegios And
                          (PlayerType.Admin Or PlayerType.Dios Or
                           PlayerType.SemiDios Or PlayerType.Consejero)) <> 0) Then
                        Call EnviarDatosASlot(tempIndex, sdData)
                    End If
                End If
            End If
        Next LoopC
    End Sub

    Private Sub SendToUserGuildArea(UserIndex As Short, sdData As String)
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modify Date: Unknow
        '
        '**************************************************************
        Dim LoopC As Integer
        Dim tempIndex As Short

        Dim Map As Short
        Dim AreaX As Short
        Dim AreaY As Short

        Map = UserList(UserIndex).Pos.Map
        AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
        AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY

        If Not MapaValido(Map) Then Exit Sub

        If UserList(UserIndex).GuildIndex = 0 Then Exit Sub

        For LoopC = 1 To ConnGroups(Map).CountEntrys
            tempIndex = ConnGroups(Map).UserEntrys(LoopC)

            If UserList(tempIndex).AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
                If UserList(tempIndex).AreasInfo.AreaReciveY And AreaY Then
                    If _
                        UserList(tempIndex).ConnIDValida And
                        (UserList(tempIndex).GuildIndex = UserList(UserIndex).GuildIndex Or
                         ((UserList(tempIndex).flags.Privilegios And PlayerType.Dios) And
                          (UserList(tempIndex).flags.Privilegios And PlayerType.RoleMaster) = 0)) Then
                        Call EnviarDatosASlot(tempIndex, sdData)
                    End If
                End If
            End If
        Next LoopC
    End Sub

    Private Sub SendToUserPartyArea(UserIndex As Short, sdData As String)
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modify Date: Unknow
        '
        '**************************************************************
        Dim LoopC As Integer
        Dim tempIndex As Short

        Dim Map As Short
        Dim AreaX As Short
        Dim AreaY As Short

        Map = UserList(UserIndex).Pos.Map
        AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
        AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY

        If Not MapaValido(Map) Then Exit Sub

        If UserList(UserIndex).PartyIndex = 0 Then Exit Sub

        For LoopC = 1 To ConnGroups(Map).CountEntrys
            tempIndex = ConnGroups(Map).UserEntrys(LoopC)

            If UserList(tempIndex).AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
                If UserList(tempIndex).AreasInfo.AreaReciveY And AreaY Then
                    If _
                        UserList(tempIndex).ConnIDValida And
                        UserList(tempIndex).PartyIndex = UserList(UserIndex).PartyIndex Then
                        Call EnviarDatosASlot(tempIndex, sdData)
                    End If
                End If
            End If
        Next LoopC
    End Sub

    Private Sub SendToAdminsButConsejerosArea(UserIndex As Short, sdData As String)
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modify Date: Unknow
        '
        '**************************************************************
        Dim LoopC As Integer
        Dim tempIndex As Short

        Dim Map As Short
        Dim AreaX As Short
        Dim AreaY As Short

        Map = UserList(UserIndex).Pos.Map
        AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
        AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY

        If Not MapaValido(Map) Then Exit Sub

        For LoopC = 1 To ConnGroups(Map).CountEntrys
            tempIndex = ConnGroups(Map).UserEntrys(LoopC)

            If UserList(tempIndex).AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
                If UserList(tempIndex).AreasInfo.AreaReciveY And AreaY Then
                    If UserList(tempIndex).ConnIDValida Then
                        If _
                            UserList(tempIndex).flags.Privilegios And
                            (PlayerType.SemiDios Or PlayerType.Dios Or
                             PlayerType.Admin) Then Call EnviarDatosASlot(tempIndex, sdData)
                    End If
                End If
            End If
        Next LoopC
    End Sub

    Private Sub SendToNpcArea(NpcIndex As Integer, sdData As String)
        '**************************************************************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: Unknow
        '
        '**************************************************************
        Dim LoopC As Integer
        Dim TempInt As Short
        Dim tempIndex As Short

        Dim Map As Short
        Dim AreaX As Short
        Dim AreaY As Short

        Map = Npclist(NpcIndex).Pos.Map
        AreaX = Npclist(NpcIndex).AreasInfo.AreaPerteneceX
        AreaY = Npclist(NpcIndex).AreasInfo.AreaPerteneceY

        If Not MapaValido(Map) Then Exit Sub

        For LoopC = 1 To ConnGroups(Map).CountEntrys
            tempIndex = ConnGroups(Map).UserEntrys(LoopC)

            TempInt = UserList(tempIndex).AreasInfo.AreaReciveX And AreaX
            If TempInt Then 'Esta en el area?
                TempInt = UserList(tempIndex).AreasInfo.AreaReciveY And AreaY
                If TempInt Then
                    If UserList(tempIndex).ConnIDValida Then
                        Call EnviarDatosASlot(tempIndex, sdData)
                    End If
                End If
            End If
        Next LoopC
    End Sub

    Public Sub SendToAreaByPos(Map As Short, AreaX As Short, AreaY As Short, sdData As String)
        '**************************************************************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: Unknow
        '
        '**************************************************************
        Dim LoopC As Integer
        Dim TempInt As Short
        Dim tempIndex As Short

        AreaX = 2^(AreaX\9)
        AreaY = 2^(AreaY\9)

        If Not MapaValido(Map) Then Exit Sub

        For LoopC = 1 To ConnGroups(Map).CountEntrys
            tempIndex = ConnGroups(Map).UserEntrys(LoopC)

            TempInt = UserList(tempIndex).AreasInfo.AreaReciveX And AreaX
            If TempInt Then 'Esta en el area?
                TempInt = UserList(tempIndex).AreasInfo.AreaReciveY And AreaY
                If TempInt Then
                    If UserList(tempIndex).ConnIDValida Then
                        Call EnviarDatosASlot(tempIndex, sdData)
                    End If
                End If
            End If
        Next LoopC
    End Sub

    Public Sub SendToMap(Map As Short, sdData As String)
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modify Date: 5/24/2007
        '
        '**************************************************************
        Dim LoopC As Integer
        Dim tempIndex As Short

        If Not MapaValido(Map) Then Exit Sub

        For LoopC = 1 To ConnGroups(Map).CountEntrys
            tempIndex = ConnGroups(Map).UserEntrys(LoopC)

            If UserList(tempIndex).ConnIDValida Then
                Call EnviarDatosASlot(tempIndex, sdData)
            End If
        Next LoopC
    End Sub

    Public Sub SendToMapButIndex(UserIndex As Short, sdData As String)
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modify Date: 5/24/2007
        '
        '**************************************************************
        Dim LoopC As Integer
        Dim Map As Short
        Dim tempIndex As Short

        Map = UserList(UserIndex).Pos.Map

        If Not MapaValido(Map) Then Exit Sub

        For LoopC = 1 To ConnGroups(Map).CountEntrys
            tempIndex = ConnGroups(Map).UserEntrys(LoopC)

            If tempIndex <> UserIndex And UserList(tempIndex).ConnIDValida Then
                Call EnviarDatosASlot(tempIndex, sdData)
            End If
        Next LoopC
    End Sub

    Private Sub SendToGMsAreaButRmsOrCounselors(UserIndex As Short, sdData As String)
        '**************************************************************
        'Author: Torres Patricio(Pato)
        'Last Modify Date: 12/02/2010
        '12/02/2010: ZaMa - Restrinjo solo a dioses, admins y gms.
        '15/02/2010: ZaMa - Cambio el nombre de la funcion (viejo: ToGmsArea, nuevo: ToGmsAreaButRMsOrCounselors)
        '**************************************************************
        Dim LoopC As Integer
        Dim tempIndex As Short

        Dim Map As Short
        Dim AreaX As Short
        Dim AreaY As Short

        Map = UserList(UserIndex).Pos.Map
        AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
        AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY

        If Not MapaValido(Map) Then Exit Sub

        For LoopC = 1 To ConnGroups(Map).CountEntrys
            tempIndex = ConnGroups(Map).UserEntrys(LoopC)

            With UserList(tempIndex)
                If .AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
                    If .AreasInfo.AreaReciveY And AreaY Then
                        If .ConnIDValida Then
                            ' Exclusivo para dioses, admins y gms
                            If _
                                (.flags.Privilegios And Not PlayerType.User And
                                 Not PlayerType.Consejero And Not PlayerType.RoleMaster) =
                                .flags.Privilegios Then
                                Call EnviarDatosASlot(tempIndex, sdData)
                            End If
                        End If
                    End If
                End If
            End With
        Next LoopC
    End Sub

    Private Sub SendToUsersAreaButGMs(UserIndex As Short, sdData As String)
        '**************************************************************
        'Author: Torres Patricio(Pato)
        'Last Modify Date: 10/17/2009
        '
        '**************************************************************
        Dim LoopC As Integer
        Dim tempIndex As Short

        Dim Map As Short
        Dim AreaX As Short
        Dim AreaY As Short

        Map = UserList(UserIndex).Pos.Map
        AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
        AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY

        If Not MapaValido(Map) Then Exit Sub

        For LoopC = 1 To ConnGroups(Map).CountEntrys
            tempIndex = ConnGroups(Map).UserEntrys(LoopC)

            If UserList(tempIndex).AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
                If UserList(tempIndex).AreasInfo.AreaReciveY And AreaY Then
                    If UserList(tempIndex).ConnIDValida Then
                        If UserList(tempIndex).flags.Privilegios And PlayerType.User Then
                            Call EnviarDatosASlot(tempIndex, sdData)
                        End If
                    End If
                End If
            End If
        Next LoopC
    End Sub

    Private Sub SendToUsersAndRmsAndCounselorsAreaButGMs(UserIndex As Short, sdData As String)
        '**************************************************************
        'Author: Torres Patricio(Pato)
        'Last Modify Date: 10/17/2009
        '
        '**************************************************************
        Dim LoopC As Integer
        Dim tempIndex As Short

        Dim Map As Short
        Dim AreaX As Short
        Dim AreaY As Short

        Map = UserList(UserIndex).Pos.Map
        AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
        AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY

        If Not MapaValido(Map) Then Exit Sub

        For LoopC = 1 To ConnGroups(Map).CountEntrys
            tempIndex = ConnGroups(Map).UserEntrys(LoopC)

            If UserList(tempIndex).AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
                If UserList(tempIndex).AreasInfo.AreaReciveY And AreaY Then
                    If UserList(tempIndex).ConnIDValida Then
                        If _
                            UserList(tempIndex).flags.Privilegios And
                            (PlayerType.User Or PlayerType.Consejero Or
                             PlayerType.RoleMaster) Then
                            Call EnviarDatosASlot(tempIndex, sdData)
                        End If
                    End If
                End If
            End If
        Next LoopC
    End Sub

    Public Sub AlertarFaccionarios(UserIndex As Short)
        '**************************************************************
        'Author: ZaMa
        'Last Modify Date: 17/11/2009
        'Alerta a los faccionarios, dandoles una orientacion
        '**************************************************************
        Dim LoopC As Integer
        Dim tempIndex As Short
        Dim Map As Short
        Dim Font As FontTypeNames

        If esCaos(UserIndex) Then
            Font = FontTypeNames.FONTTYPE_CONSEJOCAOS
        Else
            Font = FontTypeNames.FONTTYPE_CONSEJO
        End If

        Map = UserList(UserIndex).Pos.Map

        If Not MapaValido(Map) Then Exit Sub

        For LoopC = 1 To ConnGroups(Map).CountEntrys
            tempIndex = ConnGroups(Map).UserEntrys(LoopC)

            If UserList(tempIndex).ConnIDValida Then
                If tempIndex <> UserIndex Then
                    ' Solo se envia a los de la misma faccion
                    If SameFaccion(UserIndex, tempIndex) Then
                        Call _
                            EnviarDatosASlot(tempIndex,
                                             PrepareMessageConsoleMsg(
                                                 "Escuchas el llamado de un compañero que proviene del " &
                                                 GetDireccion(UserIndex, tempIndex), Font))
                    End If
                End If
            End If
        Next LoopC
    End Sub
End Module
