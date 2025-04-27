Option Strict Off
Option Explicit On

Imports System.Drawing

Module Extra
    Public Function EsNewbie(UserIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        EsNewbie = UserList(UserIndex).Stats.ELV <= LimiteNewbie
    End Function

    Public Function esArmada(UserIndex As Short) As Boolean
        '***************************************************
        'Autor: Pablo (ToxicWaste)
        'Last Modification: 23/01/2007
        '***************************************************

        esArmada = (UserList(UserIndex).Faccion.ArmadaReal = 1)
    End Function

    Public Function esCaos(UserIndex As Short) As Boolean
        '***************************************************
        'Autor: Pablo (ToxicWaste)
        'Last Modification: 23/01/2007
        '***************************************************

        esCaos = (UserList(UserIndex).Faccion.FuerzasCaos = 1)
    End Function

    Public Function EsGM(UserIndex As Short) As Boolean
        '***************************************************
        'Autor: Pablo (ToxicWaste)
        'Last Modification: 23/01/2007
        '***************************************************

        EsGM =
            (UserList(UserIndex).flags.Privilegios And
             (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios Or
              PlayerType.Consejero))
    End Function

    Public Sub DoTileEvents(UserIndex As Short, Map As Short, X As Short, Y As Short)
        '***************************************************
        'Autor: Pablo (ToxicWaste) & Unknown (orginal version)
        'Last Modification: 06/03/2010
        'Handles the Map passage of Users. Allows the existance
        'of exclusive maps for Newbies, Royal Army and Caos Legion members
        'and enables GMs to enter every map without restriction.
        'Uses: Mapinfo(map).Restringir = "NEWBIE" (newbies), "ARMADA", "CAOS", "FACCION" or "NO".
        ' 06/03/2010 : Now we have 5 attemps to not fall into a map change or another teleport while going into a teleport. (Marco)
        '***************************************************

        Dim nPos As WorldPos
        Dim FxFlag As Boolean
        Dim TelepRadio As Short
        Dim DestPos As WorldPos

        Try
            'Controla las salidas
            Dim attemps As Integer
            Dim exitMap As Boolean
            Dim aN As Short
            If InMapBounds(Map, X, Y) Then
                With MapData(Map, X, Y)
                    If .ObjInfo.ObjIndex > 0 Then
                        FxFlag = ObjData_Renamed(.ObjInfo.ObjIndex).OBJType = eOBJType.otTeleport
                        TelepRadio = ObjData_Renamed(.ObjInfo.ObjIndex).Radio
                    End If

                    If .TileExit.Map > 0 And .TileExit.Map <= NumMaps Then

                        ' Es un teleport, entra en una posicion random, acorde al radio (si es 0, es pos fija)
                        ' We have 5 attempts to not falling into another teleport or a map exit.. If we get to the fifth attemp,
                        ' the teleport will act as if its radius = 0.
                        If FxFlag And TelepRadio > 0 Then
                            Do
                                DestPos.X = .TileExit.X + RandomNumber(TelepRadio*(- 1), TelepRadio)
                                DestPos.Y = .TileExit.Y + RandomNumber(TelepRadio*(- 1), TelepRadio)

                                attemps = attemps + 1

                                exitMap = MapData(.TileExit.Map, DestPos.X, DestPos.Y).TileExit.Map > 0 And
                                          MapData(.TileExit.Map, DestPos.X, DestPos.Y).TileExit.Map <= NumMaps
                            Loop Until (attemps >= 5 Or exitMap = False)

                            If attemps >= 5 Then
                                DestPos.X = .TileExit.X
                                DestPos.Y = .TileExit.Y
                            End If
                            ' Posicion fija
                        Else
                            DestPos.X = .TileExit.X
                            DestPos.Y = .TileExit.Y
                        End If

                        DestPos.Map = .TileExit.Map

                        '¿Es mapa de newbies?
                        If UCase(MapInfo_Renamed(DestPos.Map).Restringir) = "NEWBIE" Then
                            '¿El usuario es un newbie?
                            If EsNewbie(UserIndex) Or EsGM(UserIndex) Then
                                If LegalPos(DestPos.Map, DestPos.X, DestPos.Y, PuedeAtravesarAgua(UserIndex)) Then
                                    Call WarpUserChar(UserIndex, DestPos.Map, DestPos.X, DestPos.Y, FxFlag)
                                Else
                                    Call ClosestLegalPos(DestPos, nPos)
                                    If nPos.X <> 0 And nPos.Y <> 0 Then
                                        Call WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag)
                                    End If
                                End If
                            Else 'No es newbie
                                Call _
                                    WriteConsoleMsg(UserIndex, "Mapa exclusivo para newbies.",
                                                    FontTypeNames.FONTTYPE_INFO)
                                Call ClosestStablePos(UserList(UserIndex).Pos, nPos)

                                If nPos.X <> 0 And nPos.Y <> 0 Then
                                    Call WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, False)
                                End If
                            End If
                        ElseIf UCase(MapInfo_Renamed(DestPos.Map).Restringir) = "ARMADA" Then '¿Es mapa de Armadas?
                            '¿El usuario es Armada?
                            If esArmada(UserIndex) Or EsGM(UserIndex) Then
                                If LegalPos(DestPos.Map, DestPos.X, DestPos.Y, PuedeAtravesarAgua(UserIndex)) Then
                                    Call WarpUserChar(UserIndex, DestPos.Map, DestPos.X, DestPos.Y, FxFlag)
                                Else
                                    Call ClosestLegalPos(DestPos, nPos)
                                    If nPos.X <> 0 And nPos.Y <> 0 Then
                                        Call WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag)
                                    End If
                                End If
                            Else 'No es armada
                                Call _
                                    WriteConsoleMsg(UserIndex, "Mapa exclusivo para miembros del ejército real.",
                                                    FontTypeNames.FONTTYPE_INFO)
                                Call ClosestStablePos(UserList(UserIndex).Pos, nPos)

                                If nPos.X <> 0 And nPos.Y <> 0 Then
                                    Call WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag)
                                End If
                            End If
                        ElseIf UCase(MapInfo_Renamed(DestPos.Map).Restringir) = "CAOS" Then '¿Es mapa de Caos?
                            '¿El usuario es Caos?
                            If esCaos(UserIndex) Or EsGM(UserIndex) Then
                                If LegalPos(DestPos.Map, DestPos.X, DestPos.Y, PuedeAtravesarAgua(UserIndex)) Then
                                    Call WarpUserChar(UserIndex, DestPos.Map, DestPos.X, DestPos.Y, FxFlag)
                                Else
                                    Call ClosestLegalPos(DestPos, nPos)
                                    If nPos.X <> 0 And nPos.Y <> 0 Then
                                        Call WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag)
                                    End If
                                End If
                            Else 'No es caos
                                Call _
                                    WriteConsoleMsg(UserIndex, "Mapa exclusivo para miembros de la legión oscura.",
                                                    FontTypeNames.FONTTYPE_INFO)
                                Call ClosestStablePos(UserList(UserIndex).Pos, nPos)

                                If nPos.X <> 0 And nPos.Y <> 0 Then
                                    Call WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag)
                                End If
                            End If
                        ElseIf UCase(MapInfo_Renamed(DestPos.Map).Restringir) = "FACCION" Then _
                            '¿Es mapa de faccionarios?
                            '¿El usuario es Armada o Caos?
                            If esArmada(UserIndex) Or esCaos(UserIndex) Or EsGM(UserIndex) Then
                                If LegalPos(DestPos.Map, DestPos.X, DestPos.Y, PuedeAtravesarAgua(UserIndex)) Then
                                    Call WarpUserChar(UserIndex, DestPos.Map, DestPos.X, DestPos.Y, FxFlag)
                                Else
                                    Call ClosestLegalPos(DestPos, nPos)
                                    If nPos.X <> 0 And nPos.Y <> 0 Then
                                        Call WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag)
                                    End If
                                End If
                            Else 'No es Faccionario
                                Call _
                                    WriteConsoleMsg(UserIndex,
                                                    "Solo se permite entrar al mapa si eres miembro de alguna facción.",
                                                    FontTypeNames.FONTTYPE_INFO)
                                Call ClosestStablePos(UserList(UserIndex).Pos, nPos)

                                If nPos.X <> 0 And nPos.Y <> 0 Then
                                    Call WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag)
                                End If
                            End If
                        Else 'No es un mapa de newbies, ni Armadas, ni Caos, ni faccionario.
                            If LegalPos(DestPos.Map, DestPos.X, DestPos.Y, PuedeAtravesarAgua(UserIndex)) Then
                                Call WarpUserChar(UserIndex, DestPos.Map, DestPos.X, DestPos.Y, FxFlag)
                            Else
                                Call ClosestLegalPos(DestPos, nPos)
                                If nPos.X <> 0 And nPos.Y <> 0 Then
                                    Call WarpUserChar(UserIndex, nPos.Map, nPos.X, nPos.Y, FxFlag)
                                End If
                            End If
                        End If

                        'Te fusite del mapa. La criatura ya no es más tuya ni te reconoce como que vos la atacaste.

                        aN = UserList(UserIndex).flags.AtacadoPorNpc
                        If aN > 0 Then
                            Npclist(aN).Movement = Npclist(aN).flags.OldMovement
                            Npclist(aN).Hostile = Npclist(aN).flags.OldHostil
                            Npclist(aN).flags.AttackedBy = vbNullString
                        End If

                        aN = UserList(UserIndex).flags.NPCAtacado
                        If aN > 0 Then
                            If Npclist(aN).flags.AttackedFirstBy = UserList(UserIndex).name Then
                                Npclist(aN).flags.AttackedFirstBy = vbNullString
                            End If
                        End If
                        UserList(UserIndex).flags.AtacadoPorNpc = 0
                        UserList(UserIndex).flags.NPCAtacado = 0
                    End If
                End With
            End If


        Catch ex As Exception
            Console.WriteLine("Error in EsNewbie: " & ex.Message)
            Call LogError("Error en DotileEvents. Error: " & Err.Number & " - Desc: " & Err.Description)
        End Try
    End Sub

    Function InRangoVision(UserIndex As Short, X As Short, Y As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        If X > UserList(UserIndex).Pos.X - MinXBorder And X < UserList(UserIndex).Pos.X + MinXBorder Then
            If Y > UserList(UserIndex).Pos.Y - MinYBorder And Y < UserList(UserIndex).Pos.Y + MinYBorder Then
                InRangoVision = True
                Exit Function
            End If
        End If
        InRangoVision = False
    End Function

    Function InRangoVisionNPC(NpcIndex As Short, ByRef X As Short, ByRef Y As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        If X > Npclist(NpcIndex).Pos.X - MinXBorder And X < Npclist(NpcIndex).Pos.X + MinXBorder Then
            If Y > Npclist(NpcIndex).Pos.Y - MinYBorder And Y < Npclist(NpcIndex).Pos.Y + MinYBorder Then
                InRangoVisionNPC = True
                Exit Function
            End If
        End If
        InRangoVisionNPC = False
    End Function


    Function InMapBounds(Map As Short, X As Short, Y As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        If (Map <= 0 Or Map > NumMaps) Or X < MinXBorder Or X > MaxXBorder Or Y < MinYBorder Or Y > MaxYBorder Then
            InMapBounds = False
        Else
            InMapBounds = True
        End If
    End Function

    Sub ClosestLegalPos(ByRef Pos As WorldPos, ByRef nPos As WorldPos, Optional ByRef PuedeAgua As Boolean = False,
                        Optional ByRef PuedeTierra As Boolean = True)
        '*****************************************************************
        'Author: Unknown (original version)
        'Last Modification: 24/01/2007 (ToxicWaste)
        'Encuentra la posicion legal mas cercana y la guarda en nPos
        '*****************************************************************

        Dim Notfound As Boolean
        Dim LoopC As Short
        Dim tX As Integer
        Dim tY As Integer

        nPos.Map = Pos.Map

        Do While Not LegalPos(Pos.Map, nPos.X, nPos.Y, PuedeAgua, PuedeTierra)
            If LoopC > 12 Then
                Notfound = True
                Exit Do
            End If

            For tY = Pos.Y - LoopC To Pos.Y + LoopC
                For tX = Pos.X - LoopC To Pos.X + LoopC

                    If LegalPos(nPos.Map, tX, tY, PuedeAgua, PuedeTierra) Then
                        nPos.X = tX
                        nPos.Y = tY
                        '¿Hay objeto?

                        tX = Pos.X + LoopC
                        tY = Pos.Y + LoopC
                    End If
                Next tX
            Next tY

            LoopC = LoopC + 1
        Loop

        If Notfound = True Then
            nPos.X = 0
            nPos.Y = 0
        End If
    End Sub

    Private Sub ClosestStablePos(ByRef Pos As WorldPos, ByRef nPos As WorldPos)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        'Encuentra la posicion legal mas cercana que no sea un portal y la guarda en nPos
        '*****************************************************************

        Dim Notfound As Boolean
        Dim LoopC As Short
        Dim tX As Integer
        Dim tY As Integer

        nPos.Map = Pos.Map

        Do While Not LegalPos(Pos.Map, nPos.X, nPos.Y)
            If LoopC > 12 Then
                Notfound = True
                Exit Do
            End If

            For tY = Pos.Y - LoopC To Pos.Y + LoopC
                For tX = Pos.X - LoopC To Pos.X + LoopC

                    If LegalPos(nPos.Map, tX, tY) And MapData(nPos.Map, tX, tY).TileExit.Map = 0 Then
                        nPos.X = tX
                        nPos.Y = tY
                        '¿Hay objeto?

                        tX = Pos.X + LoopC
                        tY = Pos.Y + LoopC
                    End If
                Next tX
            Next tY

            LoopC = LoopC + 1
        Loop

        If Notfound = True Then
            nPos.X = 0
            nPos.Y = 0
        End If
    End Sub

    Function NameIndex(name As String) As Short
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim UserIndex As Integer

        '¿Nombre valido?
        If migr_LenB(name) = 0 Then
            NameIndex = 0
            Exit Function
        End If

        If migr_InStrB(name, "+") <> 0 Then
            name = UCase(Replace(name, "+", " "))
        End If

        UserIndex = 1
        Do Until UCase(UserList(UserIndex).name) = UCase(name)

            UserIndex = UserIndex + 1

            If UserIndex > MaxUsers Then
                NameIndex = 0
                Exit Function
            End If
        Loop

        NameIndex = UserIndex
    End Function

    Function CheckForSameIP(UserIndex As Short, UserIP As String) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim LoopC As Integer

        For LoopC = 1 To MaxUsers
            If UserList(LoopC).flags.UserLogged = True Then
                If UserList(LoopC).ip = UserIP And UserIndex <> LoopC Then
                    CheckForSameIP = True
                    Exit Function
                End If
            End If
        Next LoopC

        CheckForSameIP = False
    End Function

    Function CheckForSameName(name As String) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'Controlo que no existan usuarios con el mismo nombre
        Dim LoopC As Integer

        For LoopC = 1 To LastUser
            If UserList(LoopC).flags.UserLogged Then

                'If UCase$(UserList(LoopC).Name) = UCase$(Name) And UserList(LoopC).ConnID <> -1 Then
                'OJO PREGUNTAR POR EL CONNID <> -1 PRODUCE QUE UN PJ EN DETERMINADO
                'MOMENTO PUEDA ESTAR LOGUEADO 2 VECES (IE: CIERRA EL SOCKET DESDE ALLA)
                'ESE EVENTO NO DISPARA UN SAVE USER, LO QUE PUEDE SER UTILIZADO PARA DUPLICAR ITEMS
                'ESTE BUG EN ALKON PRODUJO QUE EL SERVIDOR ESTE CAIDO DURANTE 3 DIAS. ATENTOS.

                If UCase(UserList(LoopC).name) = UCase(name) Then
                    CheckForSameName = True
                    Exit Function
                End If
            End If
        Next LoopC

        CheckForSameName = False
    End Function

    Sub HeadtoPos(Head As eHeading, ByRef Pos As WorldPos)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        'Toma una posicion y se mueve hacia donde esta perfilado
        '*****************************************************************

        Select Case Head
            Case eHeading.NORTH
                Pos.Y = Pos.Y - 1

            Case eHeading.SOUTH
                Pos.Y = Pos.Y + 1

            Case eHeading.EAST
                Pos.X = Pos.X + 1

            Case eHeading.WEST
                Pos.X = Pos.X - 1
        End Select
    End Sub

    Function LegalPos(Map As Short, X As Short, Y As Short,
                      Optional ByVal PuedeAgua As Boolean = False, Optional ByVal PuedeTierra As Boolean = True) _
        As Boolean
        '***************************************************
        'Autor: Pablo (ToxicWaste) & Unknown (orginal version)
        'Last Modification: 23/01/2007
        'Checks if the position is Legal.
        '***************************************************

        '¿Es un mapa valido?
        If (Map <= 0 Or Map > NumMaps) Or (X < MinXBorder Or X > MaxXBorder Or Y < MinYBorder Or Y > MaxYBorder) Then
            LegalPos = False
        Else
            With MapData(Map, X, Y)
                If PuedeAgua And PuedeTierra Then
                    LegalPos = (.Blocked <> 1) And (.UserIndex = 0) And (.NpcIndex = 0)
                ElseIf PuedeTierra And Not PuedeAgua Then
                    LegalPos = (.Blocked <> 1) And (.UserIndex = 0) And (.NpcIndex = 0) And (Not HayAgua(Map, X, Y))
                ElseIf PuedeAgua And Not PuedeTierra Then
                    LegalPos = (.Blocked <> 1) And (.UserIndex = 0) And (.NpcIndex = 0) And (HayAgua(Map, X, Y))
                Else
                    LegalPos = False
                End If
            End With
        End If
    End Function

    Function MoveToLegalPos(Map As Short, X As Short, Y As Short,
                            Optional ByVal PuedeAgua As Boolean = False, Optional ByVal PuedeTierra As Boolean = True) _
        As Boolean
        '***************************************************
        'Autor: ZaMa
        'Last Modification: 13/07/2009
        'Checks if the position is Legal, but considers that if there's a casper, it's a legal movement.
        '13/07/2009: ZaMa - Now it's also legal move where an invisible admin is.
        '***************************************************

        Dim UserIndex As Short
        Dim IsDeadChar As Boolean
        Dim IsAdminInvisible As Boolean


        '¿Es un mapa valido?
        If (Map <= 0 Or Map > NumMaps) Or (X < MinXBorder Or X > MaxXBorder Or Y < MinYBorder Or Y > MaxYBorder) Then
            MoveToLegalPos = False
        Else
            With MapData(Map, X, Y)
                UserIndex = .UserIndex

                If UserIndex > 0 Then
                    IsDeadChar = (UserList(UserIndex).flags.Muerto = 1)
                    IsAdminInvisible = (UserList(UserIndex).flags.AdminInvisible = 1)
                Else
                    IsDeadChar = False
                    IsAdminInvisible = False
                End If

                If PuedeAgua And PuedeTierra Then
                    MoveToLegalPos = (.Blocked <> 1) And (UserIndex = 0 Or IsDeadChar Or IsAdminInvisible) And
                                     (.NpcIndex = 0)
                ElseIf PuedeTierra And Not PuedeAgua Then
                    MoveToLegalPos = (.Blocked <> 1) And (UserIndex = 0 Or IsDeadChar Or IsAdminInvisible) And
                                     (.NpcIndex = 0) And (Not HayAgua(Map, X, Y))
                ElseIf PuedeAgua And Not PuedeTierra Then
                    MoveToLegalPos = (.Blocked <> 1) And (UserIndex = 0 Or IsDeadChar Or IsAdminInvisible) And
                                     (.NpcIndex = 0) And (HayAgua(Map, X, Y))
                Else
                    MoveToLegalPos = False
                End If
            End With
        End If
    End Function

    Public Sub FindLegalPos(UserIndex As Short, Map As Short, ByRef X As Short, ByRef Y As Short)
        '***************************************************
        'Autor: ZaMa
        'Last Modification: 26/03/2009
        'Search for a Legal pos for the user who is being teleported.
        '***************************************************

        Dim FoundPlace As Boolean
        Dim tX As Integer
        Dim tY As Integer
        Dim Rango As Integer
        Dim OtherUserIndex As Short
        If MapData(Map, X, Y).UserIndex <> 0 Or MapData(Map, X, Y).NpcIndex <> 0 Then

            ' Se teletransporta a la misma pos a la que estaba
            If MapData(Map, X, Y).UserIndex = UserIndex Then Exit Sub


            For Rango = 1 To 5
                For tY = Y - Rango To Y + Rango
                    For tX = X - Rango To X + Rango
                        'Reviso que no haya User ni NPC
                        If MapData(Map, tX, tY).UserIndex = 0 And MapData(Map, tX, tY).NpcIndex = 0 Then

                            If InMapBounds(Map, tX, tY) Then FoundPlace = True

                            Exit For
                        End If

                    Next tX

                    If FoundPlace Then Exit For
                Next tY

                If FoundPlace Then Exit For
            Next Rango


            If FoundPlace Then 'Si encontramos un lugar, listo, nos quedamos ahi
                X = tX
                Y = tY
            Else
                'Muy poco probable, pero..
                'Si no encontramos un lugar, sacamos al usuario que tenemos abajo, y si es un NPC, lo pisamos.
                OtherUserIndex = MapData(Map, X, Y).UserIndex
                If OtherUserIndex <> 0 Then
                    'Si no encontramos lugar, y abajo teniamos a un usuario, lo pisamos y cerramos su comercio seguro
                    If UserList(OtherUserIndex).ComUsu.DestUsu > 0 Then
                        'Le avisamos al que estaba comerciando que se tuvo que ir.
                        If UserList(UserList(OtherUserIndex).ComUsu.DestUsu).flags.UserLogged Then
                            Call FinComerciarUsu(UserList(OtherUserIndex).ComUsu.DestUsu)
                            Call _
                                WriteConsoleMsg(UserList(OtherUserIndex).ComUsu.DestUsu,
                                                "Comercio cancelado. El otro usuario se ha desconectado.",
                                                FontTypeNames.FONTTYPE_TALK)
                            Call FlushBuffer(UserList(OtherUserIndex).ComUsu.DestUsu)
                        End If
                        'Lo sacamos.
                        If UserList(OtherUserIndex).flags.UserLogged Then
                            Call FinComerciarUsu(OtherUserIndex)
                            Call _
                                WriteErrorMsg(OtherUserIndex,
                                              "Alguien se ha conectado donde te encontrabas, por favor reconéctate...")
                            Call FlushBuffer(OtherUserIndex)
                        End If
                    End If

                    Call CloseSocket(OtherUserIndex)
                End If
            End If
        End If
    End Sub

    Function LegalPosNPC(Map As Short, X As Short, Y As Short, AguaValida As Byte,
                         Optional ByVal IsPet As Boolean = False) As Boolean
        '***************************************************
        'Autor: Unkwnown
        'Last Modification: 09/23/2009
        'Checks if it's a Legal pos for the npc to move to.
        '09/23/2009: Pato - If UserIndex is a AdminInvisible, then is a legal pos.
        '***************************************************
        Dim IsDeadChar As Boolean
        Dim UserIndex As Short
        Dim IsAdminInvisible As Boolean


        If (Map <= 0 Or Map > NumMaps) Or (X < MinXBorder Or X > MaxXBorder Or Y < MinYBorder Or Y > MaxYBorder) Then
            LegalPosNPC = False
            Exit Function
        End If

        With MapData(Map, X, Y)
            UserIndex = .UserIndex
            If UserIndex > 0 Then
                IsDeadChar = UserList(UserIndex).flags.Muerto = 1
                IsAdminInvisible = (UserList(UserIndex).flags.AdminInvisible = 1)
            Else
                IsDeadChar = False
                IsAdminInvisible = False
            End If

            If AguaValida = 0 Then
                LegalPosNPC = (.Blocked <> 1) And (.UserIndex = 0 Or IsDeadChar Or IsAdminInvisible) And (.NpcIndex = 0) And
                              (.trigger <> eTrigger.POSINVALIDA Or IsPet) And Not HayAgua(Map, X, Y)
            Else
                LegalPosNPC = (.Blocked <> 1) And (.UserIndex = 0 Or IsDeadChar Or IsAdminInvisible) And (.NpcIndex = 0) And
                              (.trigger <> eTrigger.POSINVALIDA Or IsPet)
            End If
        End With
    End Function

    Sub SendHelp(Index As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim NumHelpLines As Short
        Dim LoopC As Short

        NumHelpLines = Val(GetVar(DatPath & "Help.dat", "INIT", "NumLines"))

        For LoopC = 1 To NumHelpLines
            Call _
                WriteConsoleMsg(Index, GetVar(DatPath & "Help.dat", "Help", "Line" & LoopC),
                                FontTypeNames.FONTTYPE_INFO)
        Next LoopC
    End Sub

    Public Sub Expresar(NpcIndex As Short, UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim randomi As Object
        If Npclist(NpcIndex).NroExpresiones > 0 Then
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto randomi. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            randomi = RandomNumber(1, Npclist(NpcIndex).NroExpresiones)
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto randomi. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Call _
                SendData(SendTarget.ToPCArea, UserIndex,
                         PrepareMessageChatOverHead(Npclist(NpcIndex).Expresiones(randomi),
                                                    Npclist(NpcIndex).Char_Renamed.CharIndex,
                                                    ColorTranslator.ToOle(Color.White)))
        End If
    End Sub

    Sub LookatTile(UserIndex As Short, Map As Short, X As Short, Y As Short)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 26/03/2009
        '13/02/2009: ZaMa - EL nombre del gm que aparece por consola al clickearlo, tiene el color correspondiente a su rango
        '***************************************************

        Try

            'Responde al click del usuario sobre el mapa
            Dim FoundChar As Byte
            Dim FoundSomething As Byte
            Dim TempCharIndex As Short
            Dim Stat As String
            Dim ft As FontTypeNames

            Dim estatus As String
            Dim MinHp As Integer
            Dim MaxHp As Integer
            Dim SupervivenciaSkill As Byte
            Dim sDesc As String
            With UserList(UserIndex)
                '¿Rango Visión? (ToxicWaste)
                If (Math.Abs(.Pos.Y - Y) > RANGO_VISION_Y) Or (Math.Abs(.Pos.X - X) > RANGO_VISION_X) Then
                    Exit Sub
                End If

                '¿Posicion valida?
                If InMapBounds(Map, X, Y) Then
                    With .flags
                        .TargetMap = Map
                        .TargetX = X
                        .TargetY = Y
                        '¿Es un obj?
                        If MapData(Map, X, Y).ObjInfo.ObjIndex > 0 Then
                            'Informa el nombre
                            .TargetObjMap = Map
                            .TargetObjX = X
                            .TargetObjY = Y
                            FoundSomething = 1
                        ElseIf MapData(Map, X + 1, Y).ObjInfo.ObjIndex > 0 Then
                            'Informa el nombre
                            If _
                                ObjData_Renamed(MapData(Map, X + 1, Y).ObjInfo.ObjIndex).OBJType =
                                eOBJType.otPuertas Then
                                .TargetObjMap = Map
                                .TargetObjX = X + 1
                                .TargetObjY = Y
                                FoundSomething = 1
                            End If
                        ElseIf MapData(Map, X + 1, Y + 1).ObjInfo.ObjIndex > 0 Then
                            If _
                                ObjData_Renamed(MapData(Map, X + 1, Y + 1).ObjInfo.ObjIndex).OBJType =
                                eOBJType.otPuertas Then
                                'Informa el nombre
                                .TargetObjMap = Map
                                .TargetObjX = X + 1
                                .TargetObjY = Y + 1
                                FoundSomething = 1
                            End If
                        ElseIf MapData(Map, X, Y + 1).ObjInfo.ObjIndex > 0 Then
                            If _
                                ObjData_Renamed(MapData(Map, X, Y + 1).ObjInfo.ObjIndex).OBJType =
                                eOBJType.otPuertas Then
                                'Informa el nombre
                                .TargetObjMap = Map
                                .TargetObjX = X
                                .TargetObjY = Y + 1
                                FoundSomething = 1
                            End If
                        End If

                        If FoundSomething = 1 Then
                            .TargetObj = MapData(Map, .TargetObjX, .TargetObjY).ObjInfo.ObjIndex
                            If MostrarCantidad(.TargetObj) Then
                                Call _
                                    WriteConsoleMsg(UserIndex,
                                                    ObjData_Renamed(.TargetObj).name & " - " &
                                                    MapData(.TargetObjMap, .TargetObjX, .TargetObjY).ObjInfo.Amount & "",
                                                    FontTypeNames.FONTTYPE_INFO)
                            Else
                                Call _
                                    WriteConsoleMsg(UserIndex, ObjData_Renamed(.TargetObj).name,
                                                    FontTypeNames.FONTTYPE_INFO)
                            End If

                        End If
                        '¿Es un personaje?
                        If Y + 1 <= YMaxMapSize Then
                            If MapData(Map, X, Y + 1).UserIndex > 0 Then
                                TempCharIndex = MapData(Map, X, Y + 1).UserIndex
                                FoundChar = 1
                            End If
                            If MapData(Map, X, Y + 1).NpcIndex > 0 Then
                                TempCharIndex = MapData(Map, X, Y + 1).NpcIndex
                                FoundChar = 2
                            End If
                        End If
                        '¿Es un personaje?
                        If FoundChar = 0 Then
                            If MapData(Map, X, Y).UserIndex > 0 Then
                                TempCharIndex = MapData(Map, X, Y).UserIndex
                                FoundChar = 1
                            End If
                            If MapData(Map, X, Y).NpcIndex > 0 Then
                                TempCharIndex = MapData(Map, X, Y).NpcIndex
                                FoundChar = 2
                            End If
                        End If
                    End With


                    'Reaccion al personaje
                    If FoundChar = 1 Then '  ¿Encontro un Usuario?
                        If _
                            UserList(TempCharIndex).flags.AdminInvisible = 0 Or
                            .flags.Privilegios And PlayerType.Dios Then
                            With UserList(TempCharIndex)
                                If migr_LenB(.DescRM) = 0 And .showName Then _
                                    'No tiene descRM y quiere que se vea su nombre.
                                    If EsNewbie(TempCharIndex) Then
                                        Stat = " <NEWBIE>"
                                    End If

                                    If .Faccion.ArmadaReal = 1 Then
                                        Stat = Stat & " <Ejército Real> " & "<" & TituloReal(TempCharIndex) & ">"
                                    ElseIf .Faccion.FuerzasCaos = 1 Then
                                        Stat = Stat & " <Legión Oscura> " & "<" & TituloCaos(TempCharIndex) & ">"
                                    End If

                                    If .GuildIndex > 0 Then
                                        Stat = Stat & " <" & GuildName(.GuildIndex) & ">"
                                    End If

                                    If Len(.desc) > 0 Then
                                        Stat = "Ves a " & .name & Stat & " - " & .desc
                                    Else
                                        Stat = "Ves a " & .name & Stat
                                    End If


                                    If .flags.Privilegios And PlayerType.RoyalCouncil Then
                                        Stat = Stat & " [CONSEJO DE BANDERBILL]"
                                        ft = FontTypeNames.FONTTYPE_CONSEJOVesA
                                    ElseIf .flags.Privilegios And PlayerType.ChaosCouncil Then
                                        Stat = Stat & " [CONCILIO DE LAS SOMBRAS]"
                                        ft = FontTypeNames.FONTTYPE_CONSEJOCAOSVesA
                                    Else
                                        If Not .flags.Privilegios And PlayerType.User Then
                                            Stat = Stat & " <GAME MASTER>"

                                            ' Elijo el color segun el rango del GM:
                                            ' Dios
                                            If .flags.Privilegios = PlayerType.Dios Then
                                                ft = FontTypeNames.FONTTYPE_DIOS
                                                ' Gm
                                            ElseIf .flags.Privilegios = PlayerType.SemiDios Then
                                                ft = FontTypeNames.FONTTYPE_GM
                                                ' Conse
                                            ElseIf .flags.Privilegios = PlayerType.Consejero Then
                                                ft = FontTypeNames.FONTTYPE_CONSE
                                                ' Rm o Dsrm
                                            ElseIf _
                                                .flags.Privilegios =
                                                (PlayerType.RoleMaster Or PlayerType.Consejero) Or
                                                .flags.Privilegios =
                                                (PlayerType.RoleMaster Or PlayerType.Dios) Then
                                                ft = FontTypeNames.FONTTYPE_EJECUCION
                                            End If

                                        ElseIf criminal(TempCharIndex) Then
                                            Stat = Stat & " <CRIMINAL>"
                                            ft = FontTypeNames.FONTTYPE_FIGHT
                                        Else
                                            Stat = Stat & " <CIUDADANO>"
                                            ft = FontTypeNames.FONTTYPE_CITIZEN
                                        End If
                                    End If
                                Else 'Si tiene descRM la muestro siempre.
                                    Stat = .DescRM
                                    ft = FontTypeNames.FONTTYPE_INFOBOLD
                                End If
                            End With

                            If migr_LenB(Stat) > 0 Then
                                Call WriteConsoleMsg(UserIndex, Stat, ft)
                            End If

                            FoundSomething = 1
                            .flags.TargetUser = TempCharIndex
                            .flags.TargetNPC = 0
                            .flags.TargetNpcTipo = eNPCType.Comun
                        End If
                    End If

                    With .flags
                        If FoundChar = 2 Then '¿Encontro un NPC?

                            MinHp = Npclist(TempCharIndex).Stats.MinHp
                            MaxHp = Npclist(TempCharIndex).Stats.MaxHp
                            SupervivenciaSkill = UserList(UserIndex).Stats.UserSkills(eSkill.Supervivencia)

                            If _
                                .Privilegios And
                                (PlayerType.SemiDios Or PlayerType.Dios Or
                                 PlayerType.Admin) Then
                                estatus = "(" & MinHp & "/" & MaxHp & ") "
                            Else
                                If .Muerto = 0 Then
                                    If SupervivenciaSkill >= 0 And SupervivenciaSkill <= 10 Then
                                        estatus = "(Dudoso) "
                                    ElseIf SupervivenciaSkill > 10 And SupervivenciaSkill <= 20 Then
                                        If MinHp < (MaxHp/2) Then
                                            estatus = "(Herido) "
                                        Else
                                            estatus = "(Sano) "
                                        End If
                                    ElseIf SupervivenciaSkill > 20 And SupervivenciaSkill <= 30 Then
                                        If MinHp < (MaxHp*0.5) Then
                                            estatus = "(Malherido) "
                                        ElseIf MinHp < (MaxHp*0.75) Then
                                            estatus = "(Herido) "
                                        Else
                                            estatus = "(Sano) "
                                        End If
                                    ElseIf SupervivenciaSkill > 30 And SupervivenciaSkill <= 40 Then
                                        If MinHp < (MaxHp*0.25) Then
                                            estatus = "(Muy malherido) "
                                        ElseIf MinHp < (MaxHp*0.5) Then
                                            estatus = "(Herido) "
                                        ElseIf MinHp < (MaxHp*0.75) Then
                                            estatus = "(Levemente herido) "
                                        Else
                                            estatus = "(Sano) "
                                        End If
                                    ElseIf SupervivenciaSkill > 40 And SupervivenciaSkill < 60 Then
                                        If MinHp < (MaxHp*0.05) Then
                                            estatus = "(Agonizando) "
                                        ElseIf MinHp < (MaxHp*0.1) Then
                                            estatus = "(Casi muerto) "
                                        ElseIf MinHp < (MaxHp*0.25) Then
                                            estatus = "(Muy Malherido) "
                                        ElseIf MinHp < (MaxHp*0.5) Then
                                            estatus = "(Herido) "
                                        ElseIf MinHp < (MaxHp*0.75) Then
                                            estatus = "(Levemente herido) "
                                        ElseIf MinHp < (MaxHp) Then
                                            estatus = "(Sano) "
                                        Else
                                            estatus = "(Intacto) "
                                        End If
                                    ElseIf SupervivenciaSkill >= 60 Then
                                        estatus = "(" & MinHp & "/" & MaxHp & ") "
                                    Else
                                        estatus = "¡Error!"
                                    End If
                                End If
                            End If

                            If Len(Npclist(TempCharIndex).desc) > 1 Then
                                Call _
                                    WriteChatOverHead(UserIndex, Npclist(TempCharIndex).desc,
                                                      Npclist(TempCharIndex).Char_Renamed.CharIndex,
                                                      ColorTranslator.ToOle(Color.White))
                            ElseIf TempCharIndex = CentinelaNPCIndex Then
                                'Enviamos nuevamente el texto del centinela según quien pregunta
                                Call CentinelaSendClave(UserIndex)
                            Else
                                If Npclist(TempCharIndex).MaestroUser > 0 Then
                                    Call _
                                        WriteConsoleMsg(UserIndex,
                                                        estatus & Npclist(TempCharIndex).name & " es mascota de " &
                                                        UserList(Npclist(TempCharIndex).MaestroUser).name & ".",
                                                        FontTypeNames.FONTTYPE_INFO)
                                Else
                                    sDesc = estatus & Npclist(TempCharIndex).name
                                    If Npclist(TempCharIndex).Owner > 0 Then _
                                        sDesc = sDesc & " le pertenece a " & UserList(Npclist(TempCharIndex).Owner).name
                                    sDesc = sDesc & "."

                                    Call WriteConsoleMsg(UserIndex, sDesc, FontTypeNames.FONTTYPE_INFO)

                                    If .Privilegios And (PlayerType.Dios Or PlayerType.Admin) _
                                        Then
                                        Call _
                                            WriteConsoleMsg(UserIndex,
                                                            "Le pegó primero: " &
                                                            Npclist(TempCharIndex).flags.AttackedFirstBy & ".",
                                                            FontTypeNames.FONTTYPE_INFO)
                                    End If
                                End If
                            End If

                            FoundSomething = 1
                            .TargetNpcTipo = Npclist(TempCharIndex).NPCtype
                            .TargetNPC = TempCharIndex
                            .TargetUser = 0
                            .TargetObj = 0
                        End If

                        If FoundChar = 0 Then
                            .TargetNPC = 0
                            .TargetNpcTipo = eNPCType.Comun
                            .TargetUser = 0
                        End If

                        '*** NO ENCOTRO NADA ***
                        If FoundSomething = 0 Then
                            .TargetNPC = 0
                            .TargetNpcTipo = eNPCType.Comun
                            .TargetUser = 0
                            .TargetObj = 0
                            .TargetObjMap = 0
                            .TargetObjX = 0
                            .TargetObjY = 0
                            Call WriteMultiMessage(UserIndex, eMessages.DontSeeAnything)
                        End If
                    End With
                Else
                    If FoundSomething = 0 Then
                        With .flags
                            .TargetNPC = 0
                            .TargetNpcTipo = eNPCType.Comun
                            .TargetUser = 0
                            .TargetObj = 0
                            .TargetObjMap = 0
                            .TargetObjX = 0
                            .TargetObjY = 0
                        End With

                        Call WriteMultiMessage(UserIndex, eMessages.DontSeeAnything)
                    End If
                End If
            End With


        Catch ex As Exception
            Console.WriteLine("Error in InRangoVision: " & ex.Message)
            Call LogError("Error en LookAtTile. Error " & Err.Number & " : " & Err.Description)
        End Try
    End Sub

    Function FindDirection(ByRef Pos As WorldPos, ByRef Target As WorldPos) As eHeading
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        'Devuelve la direccion en la cual el target se encuentra
        'desde pos, 0 si la direc es igual
        '*****************************************************************

        Dim X As Short
        Dim Y As Short

        X = Pos.X - Target.X
        Y = Pos.Y - Target.Y

        'NE
        If Math.Sign(X) = - 1 And Math.Sign(Y) = 1 Then
            FindDirection = IIf(RandomNumber(0, 1), eHeading.NORTH, eHeading.EAST)
            Exit Function
        End If

        'NW
        If Math.Sign(X) = 1 And Math.Sign(Y) = 1 Then
            FindDirection = IIf(RandomNumber(0, 1), eHeading.WEST, eHeading.NORTH)
            Exit Function
        End If

        'SW
        If Math.Sign(X) = 1 And Math.Sign(Y) = - 1 Then
            FindDirection = IIf(RandomNumber(0, 1), eHeading.WEST, eHeading.SOUTH)
            Exit Function
        End If

        'SE
        If Math.Sign(X) = - 1 And Math.Sign(Y) = - 1 Then
            FindDirection = IIf(RandomNumber(0, 1), eHeading.SOUTH, eHeading.EAST)
            Exit Function
        End If

        'Sur
        If Math.Sign(X) = 0 And Math.Sign(Y) = - 1 Then
            FindDirection = eHeading.SOUTH
            Exit Function
        End If

        'norte
        If Math.Sign(X) = 0 And Math.Sign(Y) = 1 Then
            FindDirection = eHeading.NORTH
            Exit Function
        End If

        'oeste
        If Math.Sign(X) = 1 And Math.Sign(Y) = 0 Then
            FindDirection = eHeading.WEST
            Exit Function
        End If

        'este
        If Math.Sign(X) = - 1 And Math.Sign(Y) = 0 Then
            FindDirection = eHeading.EAST
            Exit Function
        End If

        'misma
        If Math.Sign(X) = 0 And Math.Sign(Y) = 0 Then
            FindDirection = 0
            Exit Function
        End If
    End Function

    Public Function ItemNoEsDeMapa(Index As Short, bIsExit As Boolean) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        With ObjData_Renamed(Index)
            ItemNoEsDeMapa = .OBJType <> eOBJType.otPuertas And .OBJType <> eOBJType.otForos And
                             .OBJType <> eOBJType.otCarteles And
                             .OBJType <> eOBJType.otArboles And
                             .OBJType <> eOBJType.otYacimiento And
                             Not (.OBJType = eOBJType.otTeleport And bIsExit)

        End With
    End Function

    Public Function MostrarCantidad(Index As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        With ObjData_Renamed(Index)
            MostrarCantidad = .OBJType <> eOBJType.otPuertas And
                              .OBJType <> eOBJType.otForos And
                              .OBJType <> eOBJType.otCarteles And
                              .OBJType <> eOBJType.otArboles And
                              .OBJType <> eOBJType.otYacimiento And
                              .OBJType <> eOBJType.otTeleport
        End With
    End Function

    Public Function EsObjetoFijo(OBJType As eOBJType) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        EsObjetoFijo = OBJType = eOBJType.otForos Or OBJType = eOBJType.otCarteles Or
                       OBJType = eOBJType.otArboles Or OBJType = eOBJType.otYacimiento
    End Function
End Module
