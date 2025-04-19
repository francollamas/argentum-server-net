Option Strict Off
Option Explicit On
Module Acciones

    ''
    ' Modulo para manejar las acciones (doble click) de los carteles, foro, puerta, ramitas
    '

    ''
    ' Ejecuta la accion del doble click
    '
    ' @param UserIndex UserIndex
    ' @param Map Numero de mapa
    ' @param X X
    ' @param Y Y

    Sub Accion(ByVal UserIndex As Short, ByVal Map As Short, ByVal X As Short, ByVal Y As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim tempIndex As Short

        Try
        '¿Rango Visión? (ToxicWaste)
        If _
            (System.Math.Abs(UserList(UserIndex).Pos.Y - Y) > RANGO_VISION_Y) Or
            (System.Math.Abs(UserList(UserIndex).Pos.X - X) > RANGO_VISION_X) Then
            Exit Sub
        End If

        '¿Posicion valida?
        If InMapBounds(Map, X, Y) Then
            With UserList(UserIndex)
                If MapData(Map, X, Y).NpcIndex > 0 Then 'Acciones NPCs
                    tempIndex = MapData(Map, X, Y).NpcIndex

                    'Set the target NPC
                    .flags.TargetNPC = tempIndex

                    If Npclist(tempIndex).Comercia = 1 Then
                        '¿Esta el user muerto? Si es asi no puede comerciar
                        If .flags.Muerto = 1 Then
                            Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", Protocol.FontTypeNames.FONTTYPE_INFO)
                            Exit Sub
                        End If

                        'Is it already in commerce mode??
                        If .flags.Comerciando Then
                            Exit Sub
                        End If

                        If Distancia(Npclist(tempIndex).Pos, .Pos) > 3 Then
                            Call _
                                WriteConsoleMsg(UserIndex, "Estás demasiado lejos del vendedor.",
                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                            Exit Sub
                        End If

                        'Iniciamos la rutina pa' comerciar.
                        Call IniciarComercioNPC(UserIndex)

                    ElseIf Npclist(tempIndex).NPCtype = Declaraciones.eNPCType.Banquero Then
                        '¿Esta el user muerto? Si es asi no puede comerciar
                        If .flags.Muerto = 1 Then
                            Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", Protocol.FontTypeNames.FONTTYPE_INFO)
                            Exit Sub
                        End If

                        'Is it already in commerce mode??
                        If .flags.Comerciando Then
                            Exit Sub
                        End If

                        If Distancia(Npclist(tempIndex).Pos, .Pos) > 3 Then
                            Call _
                                WriteConsoleMsg(UserIndex, "Estás demasiado lejos del vendedor.",
                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                            Exit Sub
                        End If

                        'A depositar de una
                        Call IniciarDeposito(UserIndex)

                    ElseIf _
                        Npclist(tempIndex).NPCtype = Declaraciones.eNPCType.Revividor Or
                        Npclist(tempIndex).NPCtype = Declaraciones.eNPCType.ResucitadorNewbie Then
                        If Distancia(.Pos, Npclist(tempIndex).Pos) > 10 Then
                            Call _
                                WriteConsoleMsg(UserIndex,
                                                "El sacerdote no puede curarte debido a que estás demasiado lejos.",
                                                Protocol.FontTypeNames.FONTTYPE_INFO)
                            Exit Sub
                        End If

                        'Revivimos si es necesario
                        If _
                            .flags.Muerto = 1 And
                            (Npclist(tempIndex).NPCtype = Declaraciones.eNPCType.Revividor Or EsNewbie(UserIndex)) Then
                            Call RevivirUsuario(UserIndex)
                        End If

                        If Npclist(tempIndex).NPCtype = Declaraciones.eNPCType.Revividor Or EsNewbie(UserIndex) Then
                            'curamos totalmente
                            .Stats.MinHp = .Stats.MaxHp
                            Call WriteUpdateUserStats(UserIndex)
                        End If
                    End If

                    '¿Es un obj?
                ElseIf MapData(Map, X, Y).ObjInfo.ObjIndex > 0 Then
                    tempIndex = MapData(Map, X, Y).ObjInfo.ObjIndex

                    .flags.TargetObj = tempIndex

                    Select Case ObjData_Renamed(tempIndex).OBJType
                        Case Declaraciones.eOBJType.otPuertas 'Es una puerta
                            Call AccionParaPuerta(Map, X, Y, UserIndex)
                        Case Declaraciones.eOBJType.otCarteles 'Es un cartel
                            Call AccionParaCartel(Map, X, Y, UserIndex)
                        Case Declaraciones.eOBJType.otForos 'Foro
                            Call AccionParaForo(Map, X, Y, UserIndex)
                        Case Declaraciones.eOBJType.otLeña 'Leña
                            If tempIndex = FOGATA_APAG And .flags.Muerto = 0 Then
                                Call AccionParaRamita(Map, X, Y, UserIndex)
                            End If
                    End Select
                    '>>>>>>>>>>>OBJETOS QUE OCUPAM MAS DE UN TILE<<<<<<<<<<<<<
                ElseIf MapData(Map, X + 1, Y).ObjInfo.ObjIndex > 0 Then
                    tempIndex = MapData(Map, X + 1, Y).ObjInfo.ObjIndex
                    .flags.TargetObj = tempIndex

                    Select Case ObjData_Renamed(tempIndex).OBJType

                        Case Declaraciones.eOBJType.otPuertas 'Es una puerta
                            Call AccionParaPuerta(Map, X + 1, Y, UserIndex)

                    End Select

                ElseIf MapData(Map, X + 1, Y + 1).ObjInfo.ObjIndex > 0 Then
                    tempIndex = MapData(Map, X + 1, Y + 1).ObjInfo.ObjIndex
                    .flags.TargetObj = tempIndex

                    Select Case ObjData_Renamed(tempIndex).OBJType
                        Case Declaraciones.eOBJType.otPuertas 'Es una puerta
                            Call AccionParaPuerta(Map, X + 1, Y + 1, UserIndex)
                    End Select

                ElseIf MapData(Map, X, Y + 1).ObjInfo.ObjIndex > 0 Then
                    tempIndex = MapData(Map, X, Y + 1).ObjInfo.ObjIndex
                    .flags.TargetObj = tempIndex

                    Select Case ObjData_Renamed(tempIndex).OBJType
                        Case Declaraciones.eOBJType.otPuertas 'Es una puerta
                            Call AccionParaPuerta(Map, X, Y + 1, UserIndex)
                    End Select
                End If
            End With
        End If
    
Catch ex As Exception
    Console.WriteLine("Error in Accion: " & ex.Message)
End Try
End Sub

    Public Sub AccionParaForo(ByVal Map As Short, ByVal X As Short, ByVal Y As Short, ByVal UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 02/01/2010
        '02/01/2010: ZaMa - Agrego foros faccionarios
        '***************************************************

        Try

        Dim Pos As WorldPos

        Pos.Map = Map
        Pos.X = X
        Pos.Y = Y

        If Distancia(Pos, UserList(UserIndex).Pos) > 2 Then
            Call WriteConsoleMsg(UserIndex, "Estas demasiado lejos.", Protocol.FontTypeNames.FONTTYPE_INFO)
            Exit Sub
        End If

        If SendPosts(UserIndex, ObjData_Renamed(MapData(Map, X, Y).ObjInfo.ObjIndex).ForoID) Then
            Call WriteShowForumForm(UserIndex)
        End If
    
Catch ex As Exception
    Console.WriteLine("Error in AccionParaForo: " & ex.Message)
End Try
End Sub

    Sub AccionParaPuerta(ByVal Map As Short, ByVal X As Short, ByVal Y As Short, ByVal UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

        If Not (Distance(UserList(UserIndex).Pos.X, UserList(UserIndex).Pos.Y, X, Y) > 2) Then
            If ObjData_Renamed(MapData(Map, X, Y).ObjInfo.ObjIndex).Llave = 0 Then
                If ObjData_Renamed(MapData(Map, X, Y).ObjInfo.ObjIndex).Cerrada = 1 Then
                    'Abre la puerta
                    If ObjData_Renamed(MapData(Map, X, Y).ObjInfo.ObjIndex).Llave = 0 Then

                        MapData(Map, X, Y).ObjInfo.ObjIndex =
                            ObjData_Renamed(MapData(Map, X, Y).ObjInfo.ObjIndex).IndexAbierta

                        Call _
                            modSendData.SendToAreaByPos(Map, X, Y,
                                                        PrepareMessageObjectCreate(
                                                            ObjData_Renamed(MapData(Map, X, Y).ObjInfo.ObjIndex).
                                                                                      GrhIndex, X, Y))

                        'Desbloquea
                        MapData(Map, X, Y).Blocked = 0
                        MapData(Map, X - 1, Y).Blocked = 0

                        'Bloquea todos los mapas
                        Call Bloquear(True, Map, X, Y, 0)
                        Call Bloquear(True, Map, X - 1, Y, 0)


                        'Sonido
                        Call _
                            SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                     PrepareMessagePlayWave(SND_PUERTA, X, Y))

                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, "La puerta esta cerrada con llave.",
                                            Protocol.FontTypeNames.FONTTYPE_INFO)
                    End If
                Else
                    'Cierra puerta
                    MapData(Map, X, Y).ObjInfo.ObjIndex =
                        ObjData_Renamed(MapData(Map, X, Y).ObjInfo.ObjIndex).IndexCerrada

                    Call _
                        modSendData.SendToAreaByPos(Map, X, Y,
                                                    PrepareMessageObjectCreate(
                                                        ObjData_Renamed(MapData(Map, X, Y).ObjInfo.ObjIndex).GrhIndex, X,
                                                        Y))

                    MapData(Map, X, Y).Blocked = 1
                    MapData(Map, X - 1, Y).Blocked = 1


                    Call Bloquear(True, Map, X - 1, Y, 1)
                    Call Bloquear(True, Map, X, Y, 1)

                    Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessagePlayWave(SND_PUERTA, X, Y))
                End If

                UserList(UserIndex).flags.TargetObj = MapData(Map, X, Y).ObjInfo.ObjIndex
            Else
                Call _
                    WriteConsoleMsg(UserIndex, "La puerta está cerrada con llave.", Protocol.FontTypeNames.FONTTYPE_INFO)
            End If
        Else
            Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", Protocol.FontTypeNames.FONTTYPE_INFO)
        End If
    
Catch ex As Exception
    Console.WriteLine("Error in AccionParaPuerta: " & ex.Message)
End Try
End Sub

    Sub AccionParaCartel(ByVal Map As Short, ByVal X As Short, ByVal Y As Short, ByVal UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

        If ObjData_Renamed(MapData(Map, X, Y).ObjInfo.ObjIndex).OBJType = 8 Then

            If Len(ObjData_Renamed(MapData(Map, X, Y).ObjInfo.ObjIndex).texto) > 0 Then
                Call WriteShowSignal(UserIndex, MapData(Map, X, Y).ObjInfo.ObjIndex)
            End If

        End If
    
Catch ex As Exception
    Console.WriteLine("Error in AccionParaCartel: " & ex.Message)
End Try
End Sub

    Sub AccionParaRamita(ByVal Map As Short, ByVal X As Short, ByVal Y As Short, ByVal UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

        Dim Suerte As Byte
        Dim exito As Byte
        'UPGRADE_NOTE: Obj se actualizó a Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim Obj_Renamed As Obj

        Dim Pos As WorldPos
        Pos.Map = Map
        Pos.X = X
        Pos.Y = Y

        Dim Fogatita As New WorldPos
        With UserList(UserIndex)
            If Distancia(Pos, .Pos) > 2 Then
                Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", Protocol.FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If MapData(Map, X, Y).trigger = Declaraciones.eTrigger.ZONASEGURA Or MapInfo_Renamed(Map).Pk = False Then
                Call _
                    WriteConsoleMsg(UserIndex, "No puedes hacer fogatas en zona segura.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If _
                .Stats.UserSkills(Declaraciones.eSkill.Supervivencia) > 1 And
                .Stats.UserSkills(Declaraciones.eSkill.Supervivencia) < 6 Then
                Suerte = 3
            ElseIf _
                .Stats.UserSkills(Declaraciones.eSkill.Supervivencia) >= 6 And
                .Stats.UserSkills(Declaraciones.eSkill.Supervivencia) <= 10 Then
                Suerte = 2
            ElseIf _
                .Stats.UserSkills(Declaraciones.eSkill.Supervivencia) >= 10 And
                .Stats.UserSkills(Declaraciones.eSkill.Supervivencia) Then
                Suerte = 1
            End If

            exito = RandomNumber(1, Suerte)

            If exito = 1 Then
                If MapInfo_Renamed(.Pos.Map).Zona <> Ciudad Then
                    Obj_Renamed.ObjIndex = FOGATA
                    Obj_Renamed.Amount = 1

                    Call WriteConsoleMsg(UserIndex, "Has prendido la fogata.", Protocol.FontTypeNames.FONTTYPE_INFO)

                    Call MakeObj(Obj_Renamed, Map, X, Y)

                    'Las fogatas prendidas se deben eliminar
                    Fogatita.Map = Map
                    Fogatita.X = X
                    Fogatita.Y = Y
                    Call TrashCollector.Add(Fogatita)

                    Call SubirSkill(UserIndex, Declaraciones.eSkill.Supervivencia, True)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "La ley impide realizar fogatas en las ciudades.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If
            Else
                Call WriteConsoleMsg(UserIndex, "No has podido hacer fuego.", Protocol.FontTypeNames.FONTTYPE_INFO)
                Call SubirSkill(UserIndex, Declaraciones.eSkill.Supervivencia, False)
            End If

        End With
    
Catch ex As Exception
    Console.WriteLine("Error in AccionParaRamita: " & ex.Message)
End Try
End Sub
End Module
