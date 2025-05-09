Option Strict Off
Option Explicit On

Imports System.Drawing

Module PraetoriansCoopNPC
    '''''''''''''''''''''''''''''''''''''''''
    '' DECLARACIONES DEL MODULO PRETORIANO ''
    '''''''''''''''''''''''''''''''''''''''''
    '' Estas constantes definen que valores tienen
    '' los NPCs pretorianos en el NPC-HOSTILES.DAT
    '' Son FIJAS, pero se podria hacer una rutina que
    '' las lea desde el npcshostiles.dat

        Public Const PRCLER_NPC As Short = 900 ''"Sacerdote Pretoriano"
    Public Const PRGUER_NPC As Short = 901 ''"Guerrero  Pretoriano"
    Public Const PRMAGO_NPC As Short = 902 ''"Mago Pretoriano"
    Public Const PRCAZA_NPC As Short = 903 ''"Cazador Pretoriano"
    Public Const PRKING_NPC As Short = 904 ''"Rey Pretoriano"
    ''''''''''''''''''''''''''''''''''''''''''''''
    ''Esta constante identifica en que mapa esta
    ''la fortaleza pretoriana (no es lo mismo de
    ''donde estan los NPCs!).
    ''Se extrae el dato del server.ini en sub LoadSIni
        Public MAPA_PRETORIANO As Short

    ''''''''''''''''''''''''''''''''''''''''''''''
    ''Estos numeros son necesarios por cuestiones de
    ''sonido. Son los numeros de los wavs del cliente.
        Private Const SONIDO_Dragon_VIVO As Short = 30
    ''ALCOBAS REALES
    ''OJO LOS BICHOS TAN HARDCODEADOS, NO CAMBIAR EL MAPA DONDE
    ''ESTÁN UBICADOS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    ''MUCHO MENOS LA COORDENADA Y DE LAS ALCOBAS YA QUE DEBE SER LA MISMA!!!
    ''(HAY FUNCIONES Q CUENTAN CON QUE ES LA MISMA!)
    Public Const ALCOBA1_X As Short = 35
    Public Const ALCOBA1_Y As Short = 25
    Public Const ALCOBA2_X As Short = 67
    Public Const ALCOBA2_Y As Short = 25

    'Added by Nacho
    'Cuantos pretorianos vivos quedan. Uno por cada alcoba
    Public pretorianosVivos As Short

    '/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
    '/\/\/\/\/\/\/\/\ MODULO DE COMBATE PRETORIANO /\/\/\/\/\/\/\/\/\
    '/\/\/\/\/\/\/\/\ (NPCS COOPERATIVOS TIPO CLAN)/\/\/\/\/\/\/\/\/\
    '/\/\/\/\/\/\/\/\         por EL OSO           /\/\/\/\/\/\/\/\/\
    '/\/\/\/\/\/\/\/\       mbarrou@dc.uba.ar      /\/\/\/\/\/\/\/\/\
    '/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\

    Public Function esPretoriano(NpcIndex As Short) As Short
        Try

            Dim N As Short
            Dim i As Short
            N = Npclist(NpcIndex).Numero
            i = Npclist(NpcIndex).Char_Renamed.CharIndex
            '    Call SendData(SendTarget.ToNPCArea, NpcIndex, Npclist(NpcIndex).Pos.Map, "||" & vbGreen & "° Soy Pretoriano °" & Str(ind))
            Select Case Npclist(NpcIndex).Numero
                Case PRCLER_NPC
                    esPretoriano = 1
                Case PRMAGO_NPC
                    esPretoriano = 2
                Case PRCAZA_NPC
                    esPretoriano = 3
                Case PRKING_NPC
                    esPretoriano = 4
                Case PRGUER_NPC
                    esPretoriano = 5
            End Select


        Catch ex As Exception
            Console.WriteLine("Error in LoadSIni: " & ex.Message)
            LogError(("Error en NPCAI.EsPretoriano? " & Npclist(NpcIndex).name))
            'do nothing
        End Try
    End Function


    Sub CrearClanPretoriano(X As Short)
        '********************************************************
        'Author: EL OSO
        'Inicializa el clan Pretoriano.
        'Last Modify Date: 22/6/06: (Nacho) Seteamos cuantos NPCs creamos
        '********************************************************
        Try

            ''------------------------------------------------------
            ''recibe el X,Y donde EL REY ANTERIOR ESTABA POSICIONADO.
            ''------------------------------------------------------
            ''35,25 y 67,25 son las posiciones del rey

            ''Sub CrearNPC(NroNPC As Integer, mapa As Integer, OrigPos As WorldPos)
            ''Public Const PRCLER_NPC = 900
            ''Public Const PRGUER_NPC = 901
            ''Public Const PRMAGO_NPC = 902
            ''Public Const PRCAZA_NPC = 903
            ''Public Const PRKING_NPC = 904
            Dim wp As WorldPos
            Dim wp2 As WorldPos
            Dim TeleFrag As Short

            wp.Map = MAPA_PRETORIANO
            If X < 50 Then ''forma burda de ver que alcoba es
                wp.X = ALCOBA2_X
                wp.Y = ALCOBA2_Y
            Else
                wp.X = ALCOBA1_X
                wp.Y = ALCOBA1_Y
            End If
            pretorianosVivos = 7 'Hay 7 + el Rey.
            TeleFrag = MapData(wp.Map, wp.X, wp.Y).NpcIndex

            If TeleFrag > 0 Then
                ''El rey va a pisar a un npc de antiguo rey
                ''Obtengo en WP2 la mejor posicion cercana
                Call ClosestLegalPos(wp, wp2)
                If (LegalPos(wp2.Map, wp2.X, wp2.Y)) Then
                    ''mover al actual

                    Call _
                        SendData(SendTarget.ToNPCArea, TeleFrag,
                                 PrepareMessageCharacterMove(Npclist(TeleFrag).Char_Renamed.CharIndex, wp2.X, wp2.Y))
                    'Update map and user pos
                    MapData(wp.Map, wp.X, wp.Y).NpcIndex = 0
                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Npclist().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Npclist(TeleFrag).Pos = wp2
                    MapData(wp2.Map, wp2.X, wp2.Y).NpcIndex = TeleFrag
                Else
                    ''TELEFRAG!!!
                    Call QuitarNPC(TeleFrag)
                End If
            End If
            ''ya limpié el lugar para el rey (wp)
            ''Los otros no necesitan este caso ya que respawnan lejos
            Dim nPos As WorldPos
            'Busco la posicion legal mas cercana aca, aun que creo que tendría que ir en el crearnpc. (NicoNZ)
            Call ClosestLegalPos(wp, nPos, False, True)
            Call CrearNPC(PRKING_NPC, MAPA_PRETORIANO, nPos)

            wp.X = wp.X + 3
            Call ClosestLegalPos(wp, nPos, False, True)
            Call CrearNPC(PRCLER_NPC, MAPA_PRETORIANO, nPos)

            wp.X = wp.X - 6
            Call ClosestLegalPos(wp, nPos, False, True)
            Call CrearNPC(PRCLER_NPC, MAPA_PRETORIANO, nPos)

            wp.Y = wp.Y + 3
            Call ClosestLegalPos(wp, nPos, False, True)
            Call CrearNPC(PRGUER_NPC, MAPA_PRETORIANO, nPos)

            wp.X = wp.X + 3
            Call ClosestLegalPos(wp, nPos, False, True)
            Call CrearNPC(PRGUER_NPC, MAPA_PRETORIANO, nPos)

            wp.X = wp.X + 3
            Call ClosestLegalPos(wp, nPos, False, True)
            Call CrearNPC(PRGUER_NPC, MAPA_PRETORIANO, nPos)

            wp.Y = wp.Y - 6
            wp.X = wp.X - 1
            Call ClosestLegalPos(wp, nPos, False, True)
            Call CrearNPC(PRCAZA_NPC, MAPA_PRETORIANO, nPos)

            wp.X = wp.X - 4
            Call ClosestLegalPos(wp, nPos, False, True)
            Call CrearNPC(PRMAGO_NPC, MAPA_PRETORIANO, nPos)


        Catch ex As Exception
            Console.WriteLine("Error in CrearClanPretoriano: " & ex.Message)
            LogError(("Error en NPCAI.CrearClanPretoriano "))
            'do nothing
        End Try
    End Sub

    Sub PRCAZA_AI(npcind As Short)
        Try
            '' NO CAMBIAR:
            '' HECHIZOS: 1- FLECHA


            Dim X As Short
            Dim Y As Short
            Dim NPCPosX As Short
            Dim NPCPosY As Short
            Dim NPCPosM As Short
            Dim BestTarget As Short
            Dim NPCAlInd As Short
            Dim PJEnInd As Short

            Dim PJBestTarget As Boolean
            Dim BTx As Short
            Dim BTy As Short
            Dim Xc As Short
            Dim Yc As Short
            Dim azar As Short
            Dim azar2 As Short

            Dim quehacer As Byte
            ''1- Ataca usuarios

            NPCPosX = Npclist(npcind).Pos.X
            NPCPosY = Npclist(npcind).Pos.Y
            NPCPosM = Npclist(npcind).Pos.Map

            PJBestTarget = False
            X = 0
            Y = 0
            quehacer = 0


            azar = Math.Sign(RandomNumber(- 1, 1))
            'azar = Sgn(azar)
            If azar = 0 Then azar = 1
            azar2 = Math.Sign(RandomNumber(- 1, 1))
            'azar2 = Sgn(azar2)
            If azar2 = 0 Then azar2 = 1


            'pick the best target according to the following criteria:
            '1) magues ARE dangerous, but they are weak too, they're
            '   our primary target
            '2) in any other case, our nearest enemy will be attacked

            For X = NPCPosX + (azar*8) To NPCPosX + (azar*- 8) Step - azar
                For Y = NPCPosY + (azar2*7) To NPCPosY + (azar2*- 7) Step - azar2
                    NPCAlInd = MapData(NPCPosM, X, Y).NpcIndex ''por si implementamos algo contra NPCs
                    PJEnInd = MapData(NPCPosM, X, Y).UserIndex
                    If (PJEnInd > 0) And (Npclist(npcind).CanAttack = 1) Then
                        If _
                            (UserList(PJEnInd).flags.invisible = 0 Or UserList(PJEnInd).flags.Oculto = 0) And
                            Not (UserList(PJEnInd).flags.Muerto = 1) And Not UserList(PJEnInd).flags.AdminInvisible = 1 And
                            UserList(PJEnInd).flags.AdminPerseguible Then
                            'ToDo: Borrar los GMs
                            If (EsMagoOClerigo(PJEnInd)) Then
                                ''say no more, atacar a este
                                PJBestTarget = True
                                BestTarget = PJEnInd
                                quehacer = 1
                                'Call NpcLanzaSpellSobreUser(npcind, PJEnInd, Npclist(npcind).Spells(1)) ''flecha pasa como spell
                                X = NPCPosX + (azar*- 8)
                                Y = NPCPosY + (azar2*- 7)
                                ''forma espantosa de zafar del for
                            Else
                                If (BestTarget > 0) Then
                                    ''ver el mas cercano a mi
                                    If _
                                        Math.Sqrt((X - NPCPosX)^2 + (Y - NPCPosY)^2) <
                                        Math.Sqrt(
                                            (NPCPosX - UserList(BestTarget).Pos.X)^2 +
                                            (NPCPosY - UserList(BestTarget).Pos.Y)^2) Then
                                        ''el nuevo esta mas cerca
                                        PJBestTarget = True
                                        BestTarget = PJEnInd
                                        quehacer = 1
                                    End If
                                Else
                                    PJBestTarget = True
                                    BestTarget = PJEnInd
                                    quehacer = 1
                                End If
                            End If
                        End If
                    End If ''Fin analisis del tile
                Next Y
            Next X

            Select Case quehacer
                Case 1 ''nearest target
                    If (Npclist(npcind).CanAttack = 1) Then
                        Call NpcLanzaSpellSobreUser(npcind, BestTarget, Npclist(npcind).Spells(1))
                    End If
                    ''case 2: not yet implemented
            End Select

            ''  Vamos a setear el hold on del cazador en el medio entre el rey
            ''  y el atacante. De esta manera se lo podra atacar aun asi está lejos
            ''  pero sin alejarse del rango de los an hoax vorps de los
            ''  clerigos o rey. A menos q este paralizado, claro

            If Npclist(npcind).flags.Paralizado = 1 Then Exit Sub

            If Not NPCPosM = MAPA_PRETORIANO Then Exit Sub


            'MEJORA: Si quedan solos, se van con el resto del ejercito
            If Npclist(npcind).Invent.ArmourEqpSlot <> 0 Then
                'si me estoy yendo a alguna alcoba
                Call CambiarAlcoba(npcind)
                Exit Sub
            End If


            If EstoyMuyLejos(npcind) Then
                VolverAlCentro((npcind))
                Exit Sub
            End If

            If (BestTarget > 0) Then

                BTx = UserList(BestTarget).Pos.X
                BTy = UserList(BestTarget).Pos.Y

                If NPCPosX < 50 Then

                    Call _
                        GreedyWalkTo(npcind, MAPA_PRETORIANO, ALCOBA1_X + ((BTx - ALCOBA1_X)\2),
                                     ALCOBA1_Y + ((BTy - ALCOBA1_Y)\2))
                    'GreedyWalkTo npcind, MAPA_PRETORIANO, ALCOBA1_X + ((BTx - ALCOBA1_X) \ 2), ALCOBA1_Y + ((BTy - ALCOBA1_Y) \ 2)
                Else
                    Call _
                        GreedyWalkTo(npcind, MAPA_PRETORIANO, ALCOBA2_X + ((BTx - ALCOBA2_X)\2),
                                     ALCOBA2_Y + ((BTy - ALCOBA2_Y)\2))
                    'GreedyWalkTo npcind, MAPA_PRETORIANO, ALCOBA2_X + ((BTx - ALCOBA2_X) \ 2), ALCOBA2_Y + ((BTy - ALCOBA2_Y) \ 2)
                End If
            Else
                ''2do Loop. Busca gente acercandose por otros frentes para frenarla
                If NPCPosX < 50 Then Xc = ALCOBA1_X Else Xc = ALCOBA2_X
                Yc = ALCOBA1_Y

                For X = Xc - 16 To Xc + 16
                    For Y = Yc - 14 To Yc + 14
                        If Not (X <= NPCPosX + 8 And X >= NPCPosX - 8 And Y >= NPCPosY - 7 And Y <= NPCPosY + 7) Then
                            ''si es un tile no analizado
                            PJEnInd = MapData(NPCPosM, X, Y).UserIndex ''por si implementamos algo contra NPCs
                            If (PJEnInd > 0) Then
                                If _
                                    Not _
                                    (UserList(PJEnInd).flags.invisible = 1 Or UserList(PJEnInd).flags.Oculto = 1 Or
                                     UserList(PJEnInd).flags.Muerto = 1) Then
                                    ''si no esta muerto.., ya encontro algo para ir a buscar
                                    Call _
                                        GreedyWalkTo(npcind, MAPA_PRETORIANO, UserList(PJEnInd).Pos.X,
                                                     UserList(PJEnInd).Pos.Y)
                                    Exit Sub
                                End If
                            End If
                        End If
                    Next Y
                Next X

                ''vuelve si no esta en proceso de ataque a usuarios
                If (Npclist(npcind).CanAttack = 1) Then Call VolverAlCentro(npcind)

            End If


        Catch ex As Exception
            Console.WriteLine("Error in PRCAZA_AI: " & ex.Message)
            LogError(("Error en NPCAI.PRCAZA_AI "))
            'do nothing
        End Try
    End Sub

    Sub PRMAGO_AI(npcind As Short)
        Try

            'HECHIZOS: NO CAMBIAR ACA
            'REPRESENTAN LA UBICACION DE LOS SPELLS EN NPC_HOSTILES.DAT y si se los puede cambiar en ese archivo
            '1- APOCALIPSIS 'modificable
            '2- REMOVER INVISIBILIDAD 'NO MODIFICABLE
            Dim DAT_APOCALIPSIS As Short
            Dim DAT_REMUEVE_INVI As Short
            DAT_APOCALIPSIS = 1
            DAT_REMUEVE_INVI = 2

            ''EL mago pretoriano guarda  el index al NPC Rey en el
            ''inventario.barcoobjind parameter. Ese no es usado nunca.
            ''EL objetivo es no modificar al TAD NPC utilizando una propiedad
            ''que nunca va a ser utilizada por un NPC (espero)
            Dim X As Short
            Dim Y As Short
            Dim NPCPosX As Short
            Dim NPCPosY As Short
            Dim NPCPosM As Short
            Dim BestTarget As Short
            Dim NPCAlInd As Short
            Dim PJEnInd As Short
            Dim PJBestTarget As Boolean
            Dim bs As Byte
            Dim azar As Short
            Dim azar2 As Short

            Dim quehacer As Byte
            ''1- atacar a enemigos
            ''2- remover invisibilidades
            ''3- rotura de vara

            NPCPosX = Npclist(npcind).Pos.X ''store current position
            NPCPosY = Npclist(npcind).Pos.Y ''for direct access
            NPCPosM = Npclist(npcind).Pos.Map

            PJBestTarget = False
            BestTarget = 0
            quehacer = 0
            X = 0
            Y = 0


            If (Npclist(npcind).Stats.MinHp < 750) Then ''Dying
                quehacer = 3 ''va a romper su vara en 5 segundos
            Else
                If Not (Npclist(npcind).Invent.BarcoSlot = 6) Then
                    Npclist(npcind).Invent.BarcoSlot = 6 ''restore wand break counter
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessageCreateFX(Npclist(npcind).Char_Renamed.CharIndex, 0, 0))
                End If

                'pick the best target according to the following criteria:
                '1) invisible enemies can be detected sometimes
                '2) a wizard's mission is background spellcasting attack

                azar = Math.Sign(RandomNumber(- 1, 1))
                'azar = Sgn(azar)
                If azar = 0 Then azar = 1
                azar2 = Math.Sign(RandomNumber(- 1, 1))
                'azar2 = Sgn(azar2)
                If azar2 = 0 Then azar2 = 1

                ''esto fue para rastrear el combat field al azar
                ''Si no se hace asi, los NPCs Pretorianos "combinan" ataques, y cada
                ''ataque puede sumar hasta 700 Hit Points, lo cual los vuelve
                ''invulnerables

                '        azar = 1

                For X = NPCPosX + (azar*8) To NPCPosX + (azar*- 8) Step - azar
                    For Y = NPCPosY + (azar2*7) To NPCPosY + (azar2*- 7) Step - azar2
                        NPCAlInd = MapData(NPCPosM, X, Y).NpcIndex ''por si implementamos algo contra NPCs
                        PJEnInd = MapData(NPCPosM, X, Y).UserIndex

                        If (PJEnInd > 0) And (Npclist(npcind).CanAttack = 1) Then
                            If _
                                Not (UserList(PJEnInd).flags.Muerto = 1) And
                                Not (UserList(PJEnInd).flags.AdminInvisible = 1) And
                                UserList(PJEnInd).flags.AdminPerseguible Then
                                If (UserList(PJEnInd).flags.invisible = 1) Or (UserList(PJEnInd).flags.Oculto = 1) Then
                                    ''usuario invisible, vamos a ver si se la podemos sacar

                                    If (RandomNumber(1, 100) <= 35) Then
                                        ''mago detecta invisiblidad
                                        Npclist(npcind).CanAttack = 0
                                        Call NPCRemueveInvisibilidad(npcind, PJEnInd, DAT_REMUEVE_INVI)
                                        Exit Sub ''basta, SUFICIENTE!, jeje
                                    End If
                                    If UserList(PJEnInd).flags.Paralizado = 1 Then
                                        ''los usuarios invisibles y paralizados son un buen target!
                                        BestTarget = PJEnInd
                                        PJBestTarget = True
                                        quehacer = 2
                                    End If
                                ElseIf (UserList(PJEnInd).flags.Paralizado = 1) Then
                                    If (BestTarget > 0) Then
                                        If _
                                            Not _
                                            (UserList(BestTarget).flags.invisible = 1 Or
                                             UserList(PJEnInd).flags.Oculto = 1) _
                                            Then
                                            ''encontre un paralizado visible, y no hay un besttarget invisible (paralizado invisible)
                                            BestTarget = PJEnInd
                                            PJBestTarget = True
                                            quehacer = 2
                                        End If
                                    Else
                                        BestTarget = PJEnInd
                                        PJBestTarget = True
                                        quehacer = 2
                                    End If
                                ElseIf BestTarget = 0 Then
                                    ''movil visible
                                    BestTarget = PJEnInd
                                    PJBestTarget = True
                                    quehacer = 2
                                End If ''
                            End If ''endif:    not muerto
                        End If ''endif: es un tile con PJ y puede atacar
                    Next Y
                Next X
            End If ''endif esta muriendo


            Select Case quehacer
                ''case 1 esta "harcodeado" en el doble for
                ''es remover invisibilidades
                Case 2 ''apocalipsis Rahma Nañarak O'al
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessageChatOverHead(
                                     Hechizos(Npclist(npcind).Spells(DAT_APOCALIPSIS)).PalabrasMagicas,
                                     Npclist(npcind).Char_Renamed.CharIndex,
                                     ColorTranslator.ToOle(Color.Cyan)))
                    Call NpcLanzaSpellSobreUser2(npcind, BestTarget, Npclist(npcind).Spells(DAT_APOCALIPSIS)) _
                    ''SPELL 1 de Mago: Apocalipsis
                Case 3

                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessageCreateFX(Npclist(npcind).Char_Renamed.CharIndex,
                                                        FXIDs.FXMEDITARGRANDE, INFINITE_LOOPS))
                    ''UserList(UserIndex).Char.FX = FXIDs.FXMEDITARGRANDE

                    If Npclist(npcind).CanAttack = 1 Then
                        Npclist(npcind).CanAttack = 0
                        bs = Npclist(npcind).Invent.BarcoSlot
                        bs = bs - 1
                        Call MagoDestruyeWand(npcind, bs, DAT_APOCALIPSIS)
                        If bs = 0 Then
                            Call MuereNpc(npcind, 0)
                        Else
                            Npclist(npcind).Invent.BarcoSlot = bs
                        End If
                    End If
            End Select


            ''movimiento (si puede)
            ''El mago no se mueve a menos q tenga alguien al lado

            If Npclist(npcind).flags.Paralizado = 1 Then Exit Sub

            If Not (quehacer = 3) Then ''si no ta matandose
                ''alejarse si tiene un PJ cerca
                ''pero alejarse sin alejarse del rey
                If Not (NPCPosM = MAPA_PRETORIANO) Then Exit Sub

                ''Si no hay nadie cerca, o no tengo nada que hacer...
                If (BestTarget = 0) And (Npclist(npcind).CanAttack = 1) Then Call VolverAlCentro(npcind)

                PJEnInd = MapData(NPCPosM, NPCPosX - 1, NPCPosY).UserIndex
                If (PJEnInd > 0) Then
                    If _
                        Not (UserList(PJEnInd).flags.Muerto = 1) And
                        Not (UserList(PJEnInd).flags.invisible = 1 Or UserList(PJEnInd).flags.Oculto = 1) Then
                        ''esta es una forma muy facil de matar 2 pajaros
                        ''de un tiro. Se aleja del usuario pq el centro va a
                        ''estar ocupado, y a la vez se aproxima al rey, manteniendo
                        ''una linea de defensa compacta
                        Call VolverAlCentro(npcind)
                        Exit Sub
                    End If
                End If

                PJEnInd = MapData(NPCPosM, NPCPosX + 1, NPCPosY).UserIndex
                If PJEnInd > 0 Then
                    If _
                        Not (UserList(PJEnInd).flags.Muerto = 1) And
                        Not (UserList(PJEnInd).flags.invisible = 1 Or UserList(PJEnInd).flags.Oculto = 1) Then
                        Call VolverAlCentro(npcind)
                        Exit Sub
                    End If
                End If

                PJEnInd = MapData(NPCPosM, NPCPosX, NPCPosY - 1).UserIndex
                If PJEnInd > 0 Then
                    If _
                        Not (UserList(PJEnInd).flags.Muerto = 1) And
                        Not (UserList(PJEnInd).flags.invisible = 1 Or UserList(PJEnInd).flags.Oculto = 1) Then
                        Call VolverAlCentro(npcind)
                        Exit Sub
                    End If
                End If

                PJEnInd = MapData(NPCPosM, NPCPosX, NPCPosY + 1).UserIndex
                If PJEnInd > 0 Then
                    If _
                        Not (UserList(PJEnInd).flags.Muerto = 1) And
                        Not (UserList(PJEnInd).flags.invisible = 1 Or UserList(PJEnInd).flags.Oculto = 1) Then
                        Call VolverAlCentro(npcind)
                        Exit Sub
                    End If
                End If


            End If ''end if not matandose


        Catch ex As Exception
            Console.WriteLine("Error in PRMAGO_AI: " & ex.Message)
            LogError(("Error en NPCAI.PRMAGO_AI? "))
        End Try
    End Sub

    Sub PRREY_AI(npcind As Short)
        Try
            'HECHIZOS: NO CAMBIAR ACA
            'REPRESENTAN LA UBICACION DE LOS SPELLS EN NPC_HOSTILES.DAT y si se los puede cambiar en ese archivo
            '1- CURAR_LEVES 'NO MODIFICABLE
            '2- REMOVER PARALISIS 'NO MODIFICABLE
            '3- CEUGERA - 'NO MODIFICABLE
            '4- ESTUPIDEZ - 'NO MODIFICABLE
            '5- CURARVENENO - 'NO MODIFICABLE
            Dim DAT_CURARLEVES As Short
            Dim DAT_REMUEVEPARALISIS As Short
            Dim DAT_CEGUERA As Short
            Dim DAT_ESTUPIDEZ As Short
            Dim DAT_CURARVENENO As Short
            DAT_CURARLEVES = 1
            DAT_REMUEVEPARALISIS = 2
            DAT_CEGUERA = 3
            DAT_ESTUPIDEZ = 4
            DAT_CURARVENENO = 5


            Dim UI As Short
            Dim X As Short
            Dim Y As Short
            Dim NPCPosX As Short
            Dim NPCPosY As Short
            Dim NPCPosM As Short
            Dim NPCAlInd As Short
            Dim PJEnInd As Short
            Dim BestTarget As Short
            Dim distBestTarget As Short
            Dim dist As Short
            Dim e_p As Short
            Dim hayPretorianos As Boolean
            Dim headingloop As Byte
            Dim nPos As WorldPos
            ''Dim quehacer As Integer
            ''1- remueve paralisis con un minimo % de efecto
            ''2- remueve veneno
            ''3- cura

            NPCPosM = Npclist(npcind).Pos.Map
            NPCPosX = Npclist(npcind).Pos.X
            NPCPosY = Npclist(npcind).Pos.Y
            BestTarget = 0
            distBestTarget = 0
            hayPretorianos = False

            'pick the best target according to the following criteria:
            'King won't fight. Since praetorians' mission is to keep him alive
            'he will stay as far as possible from combat environment, but close enought
            'as to aid his loyal army.
            'If his army has been annihilated, the king will pick the
            'closest enemy an chase it using his special 'weapon speedhack' ability
            For X = NPCPosX - 8 To NPCPosX + 8
                For Y = NPCPosY - 7 To NPCPosY + 7
                    'scan combat field
                    NPCAlInd = MapData(NPCPosM, X, Y).NpcIndex
                    PJEnInd = MapData(NPCPosM, X, Y).UserIndex
                    If (Npclist(npcind).CanAttack = 1) Then ''saltea el analisis si no puede atacar para evitar cuentas
                        If (NPCAlInd > 0) Then
                            e_p = esPretoriano(NPCAlInd)
                            If e_p > 0 And e_p < 6 And (Not (NPCAlInd = npcind)) Then
                                hayPretorianos = True

                                'Me curo mientras haya pretorianos (no es lo ideal, debería no dar experiencia tampoco, pero por ahora es lo que hay)
                                Npclist(npcind).Stats.MinHp = Npclist(npcind).Stats.MaxHp
                            End If

                            If (Npclist(NPCAlInd).flags.Paralizado = 1 And e_p > 0 And e_p < 6) Then
                                ''el rey puede desparalizar con una efectividad del 20%
                                If (RandomNumber(1, 100) < 21) Then
                                    Call NPCRemueveParalisisNPC(npcind, NPCAlInd, DAT_REMUEVEPARALISIS)
                                    Npclist(npcind).CanAttack = 0
                                    Exit Sub
                                End If

                                ''failed to remove
                            ElseIf (Npclist(NPCAlInd).flags.Envenenado = 1) Then ''un chiche :D
                                If esPretoriano(NPCAlInd) Then
                                    Call NPCRemueveVenenoNPC(npcind, NPCAlInd, DAT_CURARVENENO)
                                    Npclist(npcind).CanAttack = 0
                                    Exit Sub
                                End If
                            ElseIf (Npclist(NPCAlInd).Stats.MaxHp > Npclist(NPCAlInd).Stats.MinHp) Then
                                If esPretoriano(NPCAlInd) And Not (NPCAlInd = npcind) Then
                                    ''cura, salvo q sea yo mismo. Eso lo hace 'despues'
                                    Call NPCCuraLevesNPC(npcind, NPCAlInd, DAT_CURARLEVES)
                                    Npclist(npcind).CanAttack = 0
                                    ''Exit Sub
                                End If
                            End If
                        End If

                        If PJEnInd > 0 And Not hayPretorianos Then
                            If _
                                Not _
                                (UserList(PJEnInd).flags.Muerto = 1 Or UserList(PJEnInd).flags.invisible = 1 Or
                                 UserList(PJEnInd).flags.Oculto = 1 Or UserList(PJEnInd).flags.Ceguera = 1) And
                                UserList(PJEnInd).flags.AdminPerseguible Then
                                ''si no esta muerto o invisible o ciego... o tiene el /ignorando
                                dist =
                                    Math.Sqrt(
                                        (UserList(PJEnInd).Pos.X - NPCPosX)^2 + (UserList(PJEnInd).Pos.Y - NPCPosY)^2)
                                If (dist < distBestTarget Or BestTarget = 0) Then
                                    BestTarget = PJEnInd
                                    distBestTarget = dist
                                End If
                            End If
                        End If
                    End If ''canattack = 1
                Next Y
            Next X

            If Not hayPretorianos Then
                ''si estoy aca es porque no hay pretorianos cerca!!!
                ''Todo mi ejercito fue asesinado
                ''Salgo a atacar a todos a lo loco a espadazos
                If BestTarget > 0 Then
                    If EsAlcanzable(npcind, BestTarget) Then
                        Call _
                            GreedyWalkTo(npcind, UserList(BestTarget).Pos.Map, UserList(BestTarget).Pos.X,
                                         UserList(BestTarget).Pos.Y)
                        'GreedyWalkTo npcind, UserList(BestTarget).Pos.Map, UserList(BestTarget).Pos.X, UserList(BestTarget).Pos.Y
                    Else
                        ''el chabon es piola y ataca desde lejos entonces lo castigamos!
                        Call NPCLanzaEstupidezPJ(npcind, BestTarget, DAT_ESTUPIDEZ)
                        Call NPCLanzaCegueraPJ(npcind, BestTarget, DAT_CEGUERA)
                    End If

                    ''heading loop de ataque
                    ''teclavolaespada
                    For headingloop = eHeading.NORTH To eHeading.WEST
                        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        nPos = Npclist(npcind).Pos
                        Call HeadtoPos(headingloop, nPos)
                        If InMapBounds(nPos.Map, nPos.X, nPos.Y) Then
                            UI = MapData(nPos.Map, nPos.X, nPos.Y).UserIndex
                            If UI > 0 Then
                                If NpcAtacaUser(npcind, UI) Then
                                    Call _
                                        ChangeNPCChar(npcind, Npclist(npcind).Char_Renamed.body,
                                                      Npclist(npcind).Char_Renamed.Head, headingloop)
                                End If

                                ''special speed ability for praetorian king ---------
                                Npclist(npcind).CanAttack = 1 ''this is NOT a bug!!
                                '----------------------------------------------------

                            End If
                        End If
                    Next headingloop

                Else ''no hay targets cerca
                    Call VolverAlCentro(npcind)
                    If (Npclist(npcind).Stats.MinHp < Npclist(npcind).Stats.MaxHp) And (Npclist(npcind).CanAttack = 1) _
                        Then
                        ''si no hay ndie y estoy daniado me curo
                        Call NPCCuraLevesNPC(npcind, npcind, DAT_CURARLEVES)
                        Npclist(npcind).CanAttack = 0
                    End If

                End If
            End If


        Catch ex As Exception
            Console.WriteLine("Error in PRREY_AI: " & ex.Message)
            LogError(("Error en NPCAI.PRREY_AI? "))
        End Try
    End Sub

    Sub PRGUER_AI(npcind As Short)
        Try

            Dim headingloop As Byte
            Dim nPos As WorldPos
            Dim X As Short
            Dim Y As Short
            Dim dist As Short
            Dim distBestTarget As Short
            Dim NPCPosX As Short
            Dim NPCPosY As Short
            Dim NPCPosM As Short
            Dim NPCAlInd As Short
            Dim UI As Short
            Dim PJEnInd As Short
            Dim BestTarget As Short
            NPCPosM = Npclist(npcind).Pos.Map
            NPCPosX = Npclist(npcind).Pos.X
            NPCPosY = Npclist(npcind).Pos.Y
            BestTarget = 0
            dist = 0
            distBestTarget = 0

            For X = NPCPosX - 8 To NPCPosX + 8
                For Y = NPCPosY - 7 To NPCPosY + 7
                    PJEnInd = MapData(NPCPosM, X, Y).UserIndex
                    If (PJEnInd > 0) Then
                        If _
                            (Not _
                             (UserList(PJEnInd).flags.invisible = 1 Or UserList(PJEnInd).flags.Oculto = 1 Or
                              UserList(PJEnInd).flags.Muerto = 1)) And EsAlcanzable(npcind, PJEnInd) And
                            UserList(PJEnInd).flags.AdminPerseguible Then
                            ''caluclo la distancia al PJ, si esta mas cerca q el actual
                            ''mejor besttarget entonces ataco a ese.
                            If (BestTarget > 0) Then
                                dist =
                                    Math.Sqrt(
                                        (UserList(PJEnInd).Pos.X - NPCPosX)^2 + (UserList(PJEnInd).Pos.Y - NPCPosY)^2)
                                If (dist < distBestTarget) Then
                                    BestTarget = PJEnInd
                                    distBestTarget = dist
                                End If
                            Else
                                distBestTarget =
                                    Math.Sqrt(
                                        (UserList(PJEnInd).Pos.X - NPCPosX)^2 + (UserList(PJEnInd).Pos.Y - NPCPosY)^2)
                                BestTarget = PJEnInd
                            End If
                        End If
                    End If
                Next Y
            Next X

            ''LLamo a esta funcion si lo llevaron muy lejos.
            ''La idea es que no lo "alejen" del rey y despues queden
            ''lejos de la batalla cuando matan a un enemigo o este
            ''sale del area de combate (tipica forma de separar un clan)
            If Npclist(npcind).flags.Paralizado = 0 Then

                'MEJORA: Si quedan solos, se van con el resto del ejercito
                If Npclist(npcind).Invent.ArmourEqpSlot <> 0 Then
                    Call CambiarAlcoba(npcind)
                    'si me estoy yendo a alguna alcoba
                ElseIf BestTarget = 0 Or EstoyMuyLejos(npcind) Then
                    Call VolverAlCentro(npcind)
                ElseIf BestTarget > 0 Then
                    Call _
                        GreedyWalkTo(npcind, UserList(BestTarget).Pos.Map, UserList(BestTarget).Pos.X,
                                     UserList(BestTarget).Pos.Y)
                End If
            End If

            ''teclavolaespada
            For headingloop = eHeading.NORTH To eHeading.WEST
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                nPos = Npclist(npcind).Pos
                Call HeadtoPos(headingloop, nPos)
                If InMapBounds(nPos.Map, nPos.X, nPos.Y) Then
                    UI = MapData(nPos.Map, nPos.X, nPos.Y).UserIndex
                    If UI > 0 Then
                        If Not (UserList(UI).flags.Muerto = 1) Then
                            If NpcAtacaUser(npcind, UI) Then
                                Call _
                                    ChangeNPCChar(npcind, Npclist(npcind).Char_Renamed.body,
                                                  Npclist(npcind).Char_Renamed.Head, headingloop)
                            End If
                            Npclist(npcind).CanAttack = 0
                        End If
                    End If
                End If
            Next headingloop


        Catch ex As Exception
            Console.WriteLine("Error in PRGUER_AI: " & ex.Message)
            LogError(("Error en NPCAI.PRGUER_AI? "))
        End Try
    End Sub

    Sub PRCLER_AI(npcind As Short)
        Try

            'HECHIZOS: NO CAMBIAR ACA
            'REPRESENTAN LA UBICACION DE LOS SPELLS EN NPC_HOSTILES.DAT y si se los puede cambiar en ese archivo
            '1- PARALIZAR PJS 'MODIFICABLE
            '2- REMOVER PARALISIS 'NO MODIFICABLE
            '3- CURARGRAVES - 'NO MODIFICABLE
            '4- PARALIZAR MASCOTAS - 'NO MODIFICABLE
            '5- CURARVENENO - 'NO MODIFICABLE
            Dim DAT_PARALIZARPJ As Short
            Dim DAT_REMUEVEPARALISIS As Short
            Dim DAT_CURARGRAVES As Short
            Dim DAT_PARALIZAR_NPC As Short
            Dim DAT_TORMENTAAVANZADA As Short
            DAT_PARALIZARPJ = 1
            DAT_REMUEVEPARALISIS = 2
            DAT_PARALIZAR_NPC = 3
            DAT_CURARGRAVES = 4
            DAT_TORMENTAAVANZADA = 5

            Dim X As Short
            Dim Y As Short
            Dim NPCPosX As Short
            Dim NPCPosY As Short
            Dim NPCPosM As Short
            Dim NPCAlInd As Short
            Dim PJEnInd As Short
            Dim centroX As Short
            Dim centroY As Short
            Dim BestTarget As Short
            Dim PJBestTarget As Boolean
            Dim azar As Object
            Dim azar2 As Short
            Dim quehacer As Byte
            ''1- paralizar enemigo,
            ''2- bombardear enemigo
            ''3- ataque a mascotas
            ''4- curar aliado
            quehacer = 0
            NPCPosM = Npclist(npcind).Pos.Map
            NPCPosX = Npclist(npcind).Pos.X
            NPCPosY = Npclist(npcind).Pos.Y
            PJBestTarget = False
            BestTarget = 0

            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto azar. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            azar = Math.Sign(RandomNumber(- 1, 1))
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto azar. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            If azar = 0 Then azar = 1
            azar2 = Math.Sign(RandomNumber(- 1, 1))
            If azar2 = 0 Then azar2 = 1

            'pick the best target according to the following criteria:
            '1) "hoaxed" friends MUST be released
            '2) enemy shall be annihilated no matter what
            '3) party healing if no threats
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto azar. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            For X = NPCPosX + (azar*8) To NPCPosX + (azar*- 8) Step - azar
                For Y = NPCPosY + (azar2*7) To NPCPosY + (azar2*- 7) Step - azar2
                    'scan combat field
                    NPCAlInd = MapData(NPCPosM, X, Y).NpcIndex
                    PJEnInd = MapData(NPCPosM, X, Y).UserIndex
                    If (Npclist(npcind).CanAttack = 1) Then ''saltea el analisis si no puede atacar para evitar cuentas
                        If (NPCAlInd > 0) Then ''allie?
                            If (esPretoriano(NPCAlInd) = 0) Then
                                If _
                                    (Npclist(NPCAlInd).MaestroUser > 0) And
                                    (Not (Npclist(NPCAlInd).flags.Paralizado > 0)) _
                                    Then
                                    Call NPCparalizaNPC(npcind, NPCAlInd, DAT_PARALIZAR_NPC)
                                    Npclist(npcind).CanAttack = 0
                                    Exit Sub
                                End If
                            Else 'es un PJ aliado en combate
                                If (Npclist(NPCAlInd).flags.Paralizado = 1) Then
                                    ' amigo paralizado, an hoax vorp YA
                                    Call NPCRemueveParalisisNPC(npcind, NPCAlInd, DAT_REMUEVEPARALISIS)
                                    Npclist(npcind).CanAttack = 0
                                    Exit Sub
                                ElseIf (BestTarget = 0) Then ''si no tiene nada q hacer..
                                    If (Npclist(NPCAlInd).Stats.MaxHp > Npclist(NPCAlInd).Stats.MinHp) Then
                                        BestTarget = NPCAlInd ''cura heridas
                                        PJBestTarget = False
                                        quehacer = 4
                                    End If
                                End If
                            End If
                        ElseIf (PJEnInd > 0) Then ''aggressor
                            If Not (UserList(PJEnInd).flags.Muerto = 1) And UserList(PJEnInd).flags.AdminPerseguible _
                                Then
                                If (UserList(PJEnInd).flags.Paralizado = 0) Then
                                    If _
                                        (Not _
                                         (UserList(PJEnInd).flags.invisible = 1 Or UserList(PJEnInd).flags.Oculto = 1)) _
                                        Then
                                        ''PJ movil y visible, jeje, si o si es target
                                        BestTarget = PJEnInd
                                        PJBestTarget = True
                                        quehacer = 1
                                    End If
                                Else ''PJ paralizado, ataca este invisible o no
                                    If Not (BestTarget > 0) Or Not (PJBestTarget) Then ''a menos q tenga algo mejor
                                        BestTarget = PJEnInd
                                        PJBestTarget = True
                                        quehacer = 2
                                    End If
                                End If ''endif paralizado
                            End If ''end if not muerto
                        End If ''listo el analisis del tile
                    End If _
                    ''saltea el analisis si no puede atacar, en realidad no es lo "mejor" pero evita cuentas inútiles
                Next Y
            Next X

            ''aqui (si llego) tiene el mejor target
            Select Case quehacer
                Case 0
                    ''nada que hacer. Buscar mas alla del campo de visión algun aliado, a menos
                    ''que este paralizado pq no puedo ir
                    If Npclist(npcind).flags.Paralizado = 1 Then Exit Sub

                    If Not NPCPosM = MAPA_PRETORIANO Then Exit Sub

                    If NPCPosX < 50 Then centroX = ALCOBA1_X Else centroX = ALCOBA2_X
                    centroY = ALCOBA1_Y
                    ''aca establecí el lugar de las alcobas

                    ''Este doble for busca amigos paralizados lejos para ir a rescatarlos
                    ''Entra aca solo si en el area cercana al rey no hay algo mejor que
                    ''hacer.
                    For X = centroX - 16 To centroX + 16
                        For Y = centroY - 15 To centroY + 15
                            If Not (X < NPCPosX + 8 And X > NPCPosX + 8 And Y < NPCPosY + 7 And Y > NPCPosY - 7) Then
                                ''si no es un tile ya analizado... (evito cuentas)
                                NPCAlInd = MapData(NPCPosM, X, Y).NpcIndex
                                If NPCAlInd > 0 Then
                                    If (esPretoriano(NPCAlInd) > 0 And Npclist(NPCAlInd).flags.Paralizado = 1) Then
                                        ''si esta paralizado lo va a rescatar, sino
                                        ''ya va a volver por su cuenta
                                        Call _
                                            GreedyWalkTo(npcind, NPCPosM, Npclist(NPCAlInd).Pos.X,
                                                         Npclist(NPCAlInd).Pos.Y)
                                        '                            GreedyWalkTo npcind, NPCPosM, Npclist(NPCAlInd).Pos.X, Npclist(NPCAlInd).Pos.Y
                                        Exit Sub
                                    End If
                                End If ''endif npc
                            End If ''endif tile analizado
                        Next Y
                    Next X

                    ''si estoy aca esta totalmente al cuete el clerigo o mal posicionado por rescate anterior
                    If Npclist(npcind).Invent.ArmourEqpSlot = 0 Then
                        Call VolverAlCentro(npcind)
                        Exit Sub
                    End If
                    ''fin quehacer = 0 (npc al cuete)

                Case 1 '' paralizar enemigo PJ
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessageChatOverHead(
                                     Hechizos(Npclist(npcind).Spells(DAT_PARALIZARPJ)).PalabrasMagicas,
                                     Npclist(npcind).Char_Renamed.CharIndex,
                                     ColorTranslator.ToOle(Color.Cyan)))
                    Call NpcLanzaSpellSobreUser(npcind, BestTarget, Npclist(npcind).Spells(DAT_PARALIZARPJ)) _
                    ''SPELL 1 de Clerigo es PARALIZAR
                    Exit Sub
                Case 2 '' ataque a usuarios (invisibles tambien)
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessageChatOverHead(
                                     Hechizos(Npclist(npcind).Spells(DAT_TORMENTAAVANZADA)).PalabrasMagicas,
                                     Npclist(npcind).Char_Renamed.CharIndex,
                                     ColorTranslator.ToOle(Color.Cyan)))
                    Call NpcLanzaSpellSobreUser2(npcind, BestTarget, Npclist(npcind).Spells(DAT_TORMENTAAVANZADA)) _
                    ''SPELL 2 de Clerigo es Vax On Tar avanzado
                    Exit Sub
                Case 3 '' ataque a mascotas
                    If Not (Npclist(BestTarget).flags.Paralizado = 1) Then
                        Call NPCparalizaNPC(npcind, BestTarget, DAT_PARALIZAR_NPC)
                        Npclist(npcind).CanAttack = 0
                    End If ''TODO: vax on tar sobre mascotas
                Case 4 '' party healing
                    Call NPCcuraNPC(npcind, BestTarget, DAT_CURARGRAVES)
                    Npclist(npcind).CanAttack = 0
            End Select


            ''movimientos
            ''EL clerigo no tiene un movimiento fijo, pero es esperable
            ''que no se aleje mucho del rey... y si se aleje de espaderos

            If Npclist(npcind).flags.Paralizado = 1 Then Exit Sub

            If Not (NPCPosM = MAPA_PRETORIANO) Then Exit Sub

            'MEJORA: Si quedan solos, se van con el resto del ejercito
            If Npclist(npcind).Invent.ArmourEqpSlot <> 0 Then
                Call CambiarAlcoba(npcind)
                Exit Sub
            End If


            PJEnInd = MapData(NPCPosM, NPCPosX - 1, NPCPosY).UserIndex
            If PJEnInd > 0 Then
                If _
                    Not (UserList(PJEnInd).flags.Muerto = 1) And
                    Not (UserList(PJEnInd).flags.invisible = 1 Or UserList(PJEnInd).flags.Oculto = 1) Then
                    ''esta es una forma muy facil de matar 2 pajaros
                    ''de un tiro. Se aleja del usuario pq el centro va a
                    ''estar ocupado, y a la vez se aproxima al rey, manteniendo
                    ''una linea de defensa compacta
                    Call VolverAlCentro(npcind)
                    Exit Sub
                End If
            End If

            PJEnInd = MapData(NPCPosM, NPCPosX + 1, NPCPosY).UserIndex
            If PJEnInd > 0 Then
                If _
                    Not (UserList(PJEnInd).flags.Muerto = 1) And
                    Not (UserList(PJEnInd).flags.invisible = 1 Or UserList(PJEnInd).flags.Oculto = 1) Then
                    Call VolverAlCentro(npcind)
                    Exit Sub
                End If
            End If

            PJEnInd = MapData(NPCPosM, NPCPosX, NPCPosY - 1).UserIndex
            If PJEnInd > 0 Then
                If _
                    Not (UserList(PJEnInd).flags.Muerto = 1) And
                    Not (UserList(PJEnInd).flags.invisible = 1 Or UserList(PJEnInd).flags.Oculto = 1) Then
                    Call VolverAlCentro(npcind)
                    Exit Sub
                End If
            End If

            PJEnInd = MapData(NPCPosM, NPCPosX, NPCPosY + 1).UserIndex
            If PJEnInd > 0 Then
                If _
                    Not (UserList(PJEnInd).flags.Muerto = 1) And
                    Not (UserList(PJEnInd).flags.invisible = 1 Or UserList(PJEnInd).flags.Oculto = 1) Then
                    Call VolverAlCentro(npcind)
                    Exit Sub
                End If
            End If


        Catch ex As Exception
            Console.WriteLine("Error in PRCLER_AI: " & ex.Message)
            LogError(("Error en NPCAI.PRCLER_AI? "))
        End Try
    End Sub

    Function EsMagoOClerigo(PJEnInd As Short) As Boolean
        Try

            EsMagoOClerigo = UserList(PJEnInd).clase = eClass.Mage Or
                             UserList(PJEnInd).clase = eClass.Cleric Or
                             UserList(PJEnInd).clase = eClass.Druid Or
                             UserList(PJEnInd).clase = eClass.Bard


        Catch ex As Exception
            Console.WriteLine("Error in EsMagoOClerigo: " & ex.Message)
            LogError(("Error en NPCAI.EsMagoOClerigo? "))
        End Try
    End Function

    Sub NPCRemueveVenenoNPC(npcind As Short, NPCAlInd As Short, indice As Short)
        Try
            Dim indireccion As Short

            indireccion = Npclist(npcind).Spells(indice)
            '' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
            Call _
                SendData(SendTarget.ToNPCArea, npcind,
                         PrepareMessageChatOverHead(Hechizos(indireccion).PalabrasMagicas,
                                                    Npclist(npcind).Char_Renamed.CharIndex,
                                                    ColorTranslator.ToOle(Color.Cyan)))
            Call _
                SendData(SendTarget.ToNPCArea, NPCAlInd,
                         PrepareMessageCreateFX(Npclist(NPCAlInd).Char_Renamed.CharIndex, Hechizos(indireccion).FXgrh,
                                                Hechizos(indireccion).loops))
            Call _
                SendData(SendTarget.ToNPCArea, NPCAlInd,
                         PrepareMessagePlayWave(Hechizos(indireccion).WAV, Npclist(NPCAlInd).Pos.X,
                                                Npclist(NPCAlInd).Pos.Y))
            Npclist(NPCAlInd).Veneno = 0
            Npclist(NPCAlInd).flags.Envenenado = 0


        Catch ex As Exception
            Console.WriteLine("Error in NPCRemueveVenenoNPC: " & ex.Message)
            LogError(("Error en NPCAI.NPCRemueveVenenoNPC? "))
        End Try
    End Sub

    Sub NPCCuraLevesNPC(npcind As Short, NPCAlInd As Short, indice As Short)
        Try
            Dim indireccion As Short

            indireccion = Npclist(npcind).Spells(indice)
            '' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
            Call _
                SendData(SendTarget.ToNPCArea, npcind,
                         PrepareMessageChatOverHead(Hechizos(indireccion).PalabrasMagicas,
                                                    Npclist(npcind).Char_Renamed.CharIndex,
                                                    ColorTranslator.ToOle(Color.Cyan)))
            Call _
                SendData(SendTarget.ToNPCArea, NPCAlInd,
                         PrepareMessagePlayWave(Hechizos(indireccion).WAV, Npclist(NPCAlInd).Pos.X,
                                                Npclist(NPCAlInd).Pos.Y))
            Call _
                SendData(SendTarget.ToNPCArea, NPCAlInd,
                         PrepareMessageCreateFX(Npclist(NPCAlInd).Char_Renamed.CharIndex, Hechizos(indireccion).FXgrh,
                                                Hechizos(indireccion).loops))

            If (Npclist(NPCAlInd).Stats.MinHp + 5 < Npclist(NPCAlInd).Stats.MaxHp) Then
                Npclist(NPCAlInd).Stats.MinHp = Npclist(NPCAlInd).Stats.MinHp + 5
            Else
                Npclist(NPCAlInd).Stats.MinHp = Npclist(NPCAlInd).Stats.MaxHp
            End If


        Catch ex As Exception
            Console.WriteLine("Error in NPCCuraLevesNPC: " & ex.Message)
            LogError(("Error en NPCAI.NPCCuraLevesNPC? "))
        End Try
    End Sub


    Sub NPCRemueveParalisisNPC(npcind As Short, NPCAlInd As Short, indice As Short)
        Try
            Dim indireccion As Short

            indireccion = Npclist(npcind).Spells(indice)
            '' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
            Call _
                SendData(SendTarget.ToNPCArea, npcind,
                         PrepareMessageChatOverHead(Hechizos(indireccion).PalabrasMagicas,
                                                    Npclist(npcind).Char_Renamed.CharIndex,
                                                    ColorTranslator.ToOle(Color.Cyan)))
            Call _
                SendData(SendTarget.ToNPCArea, NPCAlInd,
                         PrepareMessagePlayWave(Hechizos(indireccion).WAV, Npclist(NPCAlInd).Pos.X,
                                                Npclist(NPCAlInd).Pos.Y))
            Call _
                SendData(SendTarget.ToNPCArea, NPCAlInd,
                         PrepareMessageCreateFX(Npclist(NPCAlInd).Char_Renamed.CharIndex, Hechizos(indireccion).FXgrh,
                                                Hechizos(indireccion).loops))
            Npclist(NPCAlInd).Contadores.Paralisis = 0
            Npclist(NPCAlInd).flags.Paralizado = 0


        Catch ex As Exception
            Console.WriteLine("Error in NPCRemueveParalisisNPC: " & ex.Message)
            LogError(("Error en NPCAI.NPCRemueveParalisisNPC? "))
        End Try
    End Sub

    Sub NPCparalizaNPC(paralizador As Short, Paralizado As Short, indice As Object)
        Try
            Dim indireccion As Short

            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto indice. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            indireccion = Npclist(paralizador).Spells(indice)
            '' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
            Call _
                SendData(SendTarget.ToNPCArea, paralizador,
                         PrepareMessageChatOverHead(Hechizos(indireccion).PalabrasMagicas,
                                                    Npclist(paralizador).Char_Renamed.CharIndex,
                                                    ColorTranslator.ToOle(Color.Cyan)))
            Call _
                SendData(SendTarget.ToNPCArea, Paralizado,
                         PrepareMessagePlayWave(Hechizos(indireccion).WAV, Npclist(Paralizado).Pos.X,
                                                Npclist(Paralizado).Pos.Y))
            Call _
                SendData(SendTarget.ToNPCArea, Paralizado,
                         PrepareMessageCreateFX(Npclist(Paralizado).Char_Renamed.CharIndex, Hechizos(indireccion).FXgrh,
                                                Hechizos(indireccion).loops))

            Npclist(Paralizado).flags.Paralizado = 1
            Npclist(Paralizado).Contadores.Paralisis = IntervaloParalizado*2


        Catch ex As Exception
            Console.WriteLine("Error in NPCparalizaNPC: " & ex.Message)
            LogError(("Error en NPCAI.NPCParalizaNPC? "))
        End Try
    End Sub

    Sub NPCcuraNPC(curador As Short, curado As Short, indice As Short)
        Try
            Dim indireccion As Short


            indireccion = Npclist(curador).Spells(indice)
            '' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
            Call _
                SendData(SendTarget.ToNPCArea, curador,
                         PrepareMessageChatOverHead(Hechizos(indireccion).PalabrasMagicas,
                                                    Npclist(curador).Char_Renamed.CharIndex,
                                                    ColorTranslator.ToOle(Color.Cyan)))
            Call _
                SendData(SendTarget.ToNPCArea, curado,
                         PrepareMessagePlayWave(Hechizos(indireccion).WAV, Npclist(curado).Pos.X, Npclist(curado).Pos.Y))
            Call _
                SendData(SendTarget.ToNPCArea, curado,
                         PrepareMessageCreateFX(Npclist(curado).Char_Renamed.CharIndex, Hechizos(indireccion).FXgrh,
                                                Hechizos(indireccion).loops))
            If Npclist(curado).Stats.MinHp + 30 > Npclist(curado).Stats.MaxHp Then
                Npclist(curado).Stats.MinHp = Npclist(curado).Stats.MaxHp
            Else
                Npclist(curado).Stats.MinHp = Npclist(curado).Stats.MinHp + 30
            End If


        Catch ex As Exception
            Console.WriteLine("Error in NPCcuraNPC: " & ex.Message)
            LogError(("Error en NPCAI.NPCcuraNPC? "))
        End Try
    End Sub

    Sub NPCLanzaCegueraPJ(npcind As Short, PJEnInd As Short, indice As Short)
        Try
            Dim indireccion As Short

            indireccion = Npclist(npcind).Spells(indice)
            '' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
            Call _
                SendData(SendTarget.ToNPCArea, npcind,
                         PrepareMessageChatOverHead(Hechizos(indireccion).PalabrasMagicas,
                                                    Npclist(npcind).Char_Renamed.CharIndex,
                                                    ColorTranslator.ToOle(Color.Cyan)))
            Call _
                SendData(SendTarget.ToNPCArea, PJEnInd,
                         PrepareMessagePlayWave(Hechizos(indireccion).WAV, UserList(PJEnInd).Pos.X,
                                                UserList(PJEnInd).Pos.Y))
            Call _
                SendData(SendTarget.ToPCArea, PJEnInd,
                         PrepareMessageCreateFX(UserList(PJEnInd).Char_Renamed.CharIndex, Hechizos(indireccion).FXgrh,
                                                Hechizos(indireccion).loops))

            UserList(PJEnInd).flags.Ceguera = 1
            UserList(PJEnInd).Counters.Ceguera = IntervaloInvisible
            ''Envia ceguera
            Call WriteBlind(PJEnInd)
            ''bardea si es el rey
            If Npclist(npcind).name = "Rey Pretoriano" Then
                Call _
                    WriteConsoleMsg(PJEnInd, "El rey pretoriano te ha vuelto ciego ", FontTypeNames.FONTTYPE_FIGHT)
                Call _
                    WriteConsoleMsg(PJEnInd,
                                    "A la distancia escuchas las siguientes palabras: ¡Cobarde, no eres digno de luchar conmigo si escapas! ",
                                    FontTypeNames.FONTTYPE_VENENO)
            End If


        Catch ex As Exception
            Console.WriteLine("Error in NPCLanzaCegueraPJ: " & ex.Message)
            LogError(("Error en NPCAI.NPCLanzaCegueraPJ? "))
        End Try
    End Sub

    Sub NPCLanzaEstupidezPJ(npcind As Short, PJEnInd As Short, indice As Short)
        Try
            Dim indireccion As Short


            indireccion = Npclist(npcind).Spells(indice)
            '' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
            Call _
                SendData(SendTarget.ToNPCArea, npcind,
                         PrepareMessageChatOverHead(Hechizos(indireccion).PalabrasMagicas,
                                                    Npclist(npcind).Char_Renamed.CharIndex,
                                                    ColorTranslator.ToOle(Color.Cyan)))
            Call _
                SendData(SendTarget.ToNPCArea, PJEnInd,
                         PrepareMessagePlayWave(Hechizos(indireccion).WAV, UserList(PJEnInd).Pos.X,
                                                UserList(PJEnInd).Pos.Y))
            Call _
                SendData(SendTarget.ToPCArea, PJEnInd,
                         PrepareMessageCreateFX(UserList(PJEnInd).Char_Renamed.CharIndex, Hechizos(indireccion).FXgrh,
                                                Hechizos(indireccion).loops))
            UserList(PJEnInd).flags.Estupidez = 1
            UserList(PJEnInd).Counters.Estupidez = IntervaloInvisible
            'manda estupidez
            Call WriteDumb(PJEnInd)

            'bardea si es el rey
            If Npclist(npcind).name = "Rey Pretoriano" Then
                Call _
                    WriteConsoleMsg(PJEnInd, "El rey pretoriano te ha vuelto estúpido.",
                                    FontTypeNames.FONTTYPE_FIGHT)
            End If


        Catch ex As Exception
            Console.WriteLine("Error in NPCLanzaEstupidezPJ: " & ex.Message)
            LogError(("Error en NPCAI.NPCLanzaEstupidezPJ? "))
        End Try
    End Sub

    Sub NPCRemueveInvisibilidad(npcind As Short, PJEnInd As Short, indice As Short)
        Try
            Dim indireccion As Short

            indireccion = Npclist(npcind).Spells(indice)
            '' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
            Call _
                SendData(SendTarget.ToNPCArea, npcind,
                         PrepareMessageChatOverHead(Hechizos(indireccion).PalabrasMagicas,
                                                    Npclist(npcind).Char_Renamed.CharIndex,
                                                    ColorTranslator.ToOle(Color.Cyan)))
            Call _
                SendData(SendTarget.ToNPCArea, PJEnInd,
                         PrepareMessagePlayWave(Hechizos(indireccion).WAV, UserList(PJEnInd).Pos.X,
                                                UserList(PJEnInd).Pos.Y))
            Call _
                SendData(SendTarget.ToPCArea, PJEnInd,
                         PrepareMessageCreateFX(UserList(PJEnInd).Char_Renamed.CharIndex, Hechizos(indireccion).FXgrh,
                                                Hechizos(indireccion).loops))

            'Sacamos el efecto de ocultarse
            If UserList(PJEnInd).flags.Oculto = 1 Then
                UserList(PJEnInd).Counters.TiempoOculto = 0
                UserList(PJEnInd).flags.Oculto = 0
                Call SetInvisible(PJEnInd, UserList(PJEnInd).Char_Renamed.CharIndex, False)
                'Call SendData(SendTarget.ToPCArea, PJEnInd, PrepareMessageSetInvisible(UserList(PJEnInd).Char.CharIndex, False))
                Call WriteConsoleMsg(PJEnInd, "¡Has sido detectado!", FontTypeNames.FONTTYPE_VENENO)
            Else
                'sino, solo lo "iniciamos" en la sacada de invisibilidad.
                Call WriteConsoleMsg(PJEnInd, "Comienzas a hacerte visible.", FontTypeNames.FONTTYPE_VENENO)
                UserList(PJEnInd).Counters.Invisibilidad = IntervaloInvisible - 1
            End If


        Catch ex As Exception
            Console.WriteLine("Error in NPCRemueveInvisibilidad: " & ex.Message)
            LogError(("Error en NPCAI.NPCRemueveInvisibilidad "))
        End Try
    End Sub

    Sub NpcLanzaSpellSobreUser2(NpcIndex As Short, UserIndex As Short, Spell As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 05/09/09
        '05/09/09: Pato - Ahora actualiza la vida del usuario atacado
        '***************************************************
        Try
            ''  Igual a la otra pero ataca invisibles!!!
            '' (malditos controles de casos imposibles...)

            If Npclist(NpcIndex).CanAttack = 0 Then Exit Sub
            'If UserList(UserIndex).Flags.Invisible = 1 Then Exit Sub

            Npclist(NpcIndex).CanAttack = 0
            Dim daño As Short

            If Hechizos(Spell).SubeHP = 1 Then

                daño = RandomNumber(Hechizos(Spell).MinHp, Hechizos(Spell).MaxHp)
                Call _
                    SendData(SendTarget.ToPCArea, UserIndex,
                             PrepareMessagePlayWave(Hechizos(Spell).WAV, UserList(UserIndex).Pos.X,
                                                    UserList(UserIndex).Pos.Y))
                Call _
                    SendData(SendTarget.ToPCArea, UserIndex,
                             PrepareMessageCreateFX(UserList(UserIndex).Char_Renamed.CharIndex, Hechizos(Spell).FXgrh,
                                                    Hechizos(Spell).loops))

                UserList(UserIndex).Stats.MinHp = UserList(UserIndex).Stats.MinHp + daño
                If UserList(UserIndex).Stats.MinHp > UserList(UserIndex).Stats.MaxHp Then _
                    UserList(UserIndex).Stats.MinHp = UserList(UserIndex).Stats.MaxHp

                Call _
                    WriteConsoleMsg(UserIndex, Npclist(NpcIndex).name & " te ha quitado " & daño & " puntos de vida.",
                                    FontTypeNames.FONTTYPE_FIGHT)

                Call WriteUpdateHP(UserIndex)
            ElseIf Hechizos(Spell).SubeHP = 2 Then

                daño = RandomNumber(Hechizos(Spell).MinHp, Hechizos(Spell).MaxHp)
                Call _
                    SendData(SendTarget.ToPCArea, UserIndex,
                             PrepareMessagePlayWave(Hechizos(Spell).WAV, UserList(UserIndex).Pos.X,
                                                    UserList(UserIndex).Pos.Y))
                Call _
                    SendData(SendTarget.ToPCArea, UserIndex,
                             PrepareMessageCreateFX(UserList(UserIndex).Char_Renamed.CharIndex, Hechizos(Spell).FXgrh,
                                                    Hechizos(Spell).loops))

                If UserList(UserIndex).flags.Privilegios And PlayerType.User Then _
                    UserList(UserIndex).Stats.MinHp = UserList(UserIndex).Stats.MinHp - daño

                Call _
                    WriteConsoleMsg(UserIndex, Npclist(NpcIndex).name & " te ha quitado " & daño & " puntos de vida.",
                                    FontTypeNames.FONTTYPE_FIGHT)

                'Muere
                If UserList(UserIndex).Stats.MinHp < 1 Then
                    UserList(UserIndex).Stats.MinHp = 0
                    Call UserDie(UserIndex)
                End If

                Call WriteUpdateHP(UserIndex)
            End If

            If Hechizos(Spell).Paraliza = 1 Then
                If UserList(UserIndex).flags.Paralizado = 0 Then
                    UserList(UserIndex).flags.Paralizado = 1
                    UserList(UserIndex).Counters.Paralisis = IntervaloParalizado
                    Call _
                        SendData(SendTarget.ToPCArea, UserIndex,
                                 PrepareMessagePlayWave(Hechizos(Spell).WAV, UserList(UserIndex).Pos.X,
                                                        UserList(UserIndex).Pos.Y))
                    Call _
                        SendData(SendTarget.ToPCArea, UserIndex,
                                 PrepareMessageCreateFX(UserList(UserIndex).Char_Renamed.CharIndex,
                                                        Hechizos(Spell).FXgrh,
                                                        Hechizos(Spell).loops))

                    Call WriteParalizeOK(UserIndex)

                End If
            End If


        Catch ex As Exception
            Console.WriteLine("Error in NpcLanzaSpellSobreUser2: " & ex.Message)
            LogError(("Error en NPCAI.NPCLanzaSpellSobreUser2 "))
        End Try
    End Sub


    Sub MagoDestruyeWand(npcind As Short, bs As Byte, indice As Short)
        Try
            ''sonidos: 30 y 32, y no los cambien sino termina siendo muy chistoso...
            ''Para el FX utiliza el del hechizos(indice)
            Dim X As Short
            Dim Y As Short
            Dim PJInd As Short
            Dim NPCPosX As Short
            Dim NPCPosY As Short
            Dim NPCPosM As Short
            Dim danio As Double
            Dim dist As Double
            Dim danioI As Short
            Dim MascotaInd As Short
            Dim indireccion As Short

            Select Case bs
                Case 5
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessageChatOverHead("Rahma", Npclist(npcind).Char_Renamed.CharIndex,
                                                            ColorTranslator.ToOle(Color.Lime)))
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessagePlayWave(SONIDO_Dragon_VIVO, Npclist(npcind).Pos.X, Npclist(npcind).Pos.Y))
                Case 4
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessageChatOverHead("vôrtax", Npclist(npcind).Char_Renamed.CharIndex,
                                                            ColorTranslator.ToOle(Color.Lime)))
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessagePlayWave(SONIDO_Dragon_VIVO, Npclist(npcind).Pos.X, Npclist(npcind).Pos.Y))
                Case 3
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessageChatOverHead("Zill", Npclist(npcind).Char_Renamed.CharIndex,
                                                            ColorTranslator.ToOle(Color.Lime)))
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessagePlayWave(SONIDO_Dragon_VIVO, Npclist(npcind).Pos.X, Npclist(npcind).Pos.Y))
                Case 2
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessageChatOverHead("yäkà E'nta", Npclist(npcind).Char_Renamed.CharIndex,
                                                            ColorTranslator.ToOle(Color.Lime)))
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessagePlayWave(SONIDO_Dragon_VIVO, Npclist(npcind).Pos.X, Npclist(npcind).Pos.Y))
                Case 1
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessageChatOverHead("¡¡Koràtá!!", Npclist(npcind).Char_Renamed.CharIndex,
                                                            ColorTranslator.ToOle(Color.Lime)))
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessagePlayWave(SONIDO_Dragon_VIVO, Npclist(npcind).Pos.X, Npclist(npcind).Pos.Y))
                Case 0
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessageChatOverHead(vbNullString, Npclist(npcind).Char_Renamed.CharIndex,
                                                            ColorTranslator.ToOle(Color.Lime)))
                    Call _
                        SendData(SendTarget.ToNPCArea, npcind,
                                 PrepareMessagePlayWave(SONIDO_Dragon_VIVO, Npclist(npcind).Pos.X, Npclist(npcind).Pos.Y))
                    NPCPosX = Npclist(npcind).Pos.X
                    NPCPosY = Npclist(npcind).Pos.Y
                    NPCPosM = Npclist(npcind).Pos.Map
                    PJInd = 0
                    indireccion = Npclist(npcind).Spells(indice)
                    ''Daño masivo por destruccion de wand
                    For X = 8 To 95
                        For Y = 8 To 95
                            PJInd = MapData(NPCPosM, X, Y).UserIndex
                            MascotaInd = MapData(NPCPosM, X, Y).NpcIndex
                            If PJInd > 0 Then
                                dist =
                                    Math.Sqrt(
                                        (UserList(PJInd).Pos.X - NPCPosX)^2 + (UserList(PJInd).Pos.Y - NPCPosY)^2)
                                danio = 880/(dist^(3/7))
                                danioI = Math.Abs(Int(danio))
                                ''efectiviza el danio
                                If UserList(PJInd).flags.Privilegios And PlayerType.User Then _
                                    UserList(PJInd).Stats.MinHp = UserList(PJInd).Stats.MinHp - danioI

                                Call _
                                    WriteConsoleMsg(PJInd,
                                                    Npclist(npcind).name & " te ha quitado " & danioI &
                                                    " puntos de vida al romper su vara.",
                                                    FontTypeNames.FONTTYPE_FIGHT)
                                Call _
                                    SendData(SendTarget.ToPCArea, PJInd,
                                             PrepareMessagePlayWave(Hechizos(indireccion).WAV, UserList(PJInd).Pos.X,
                                                                    UserList(PJInd).Pos.Y))
                                Call _
                                    SendData(SendTarget.ToPCArea, PJInd,
                                             PrepareMessageCreateFX(UserList(PJInd).Char_Renamed.CharIndex,
                                                                    Hechizos(indireccion).FXgrh,
                                                                    Hechizos(indireccion).loops))

                                If UserList(PJInd).Stats.MinHp < 1 Then
                                    UserList(PJInd).Stats.MinHp = 0
                                    Call UserDie(PJInd)
                                End If

                            ElseIf (MascotaInd > 0) Then
                                If (Npclist(MascotaInd).MaestroUser > 0) Then

                                    dist =
                                        Math.Sqrt(
                                            (Npclist(MascotaInd).Pos.X - NPCPosX)^2 +
                                            (Npclist(MascotaInd).Pos.Y - NPCPosY)^2)
                                    danio = 880/(dist^(3/7))
                                    danioI = Math.Abs(Int(danio))
                                    ''efectiviza el danio
                                    Npclist(MascotaInd).Stats.MinHp = Npclist(MascotaInd).Stats.MinHp - danioI

                                    Call _
                                        SendData(SendTarget.ToNPCArea, MascotaInd,
                                                 PrepareMessagePlayWave(Hechizos(indireccion).WAV,
                                                                        Npclist(MascotaInd).Pos.X,
                                                                        Npclist(MascotaInd).Pos.Y))
                                    Call _
                                        SendData(SendTarget.ToNPCArea, MascotaInd,
                                                 PrepareMessageCreateFX(Npclist(MascotaInd).Char_Renamed.CharIndex,
                                                                        Hechizos(indireccion).FXgrh,
                                                                        Hechizos(indireccion).loops))

                                    If Npclist(MascotaInd).Stats.MinHp < 1 Then
                                        Npclist(MascotaInd).Stats.MinHp = 0
                                        Call MuereNpc(MascotaInd, 0)
                                    End If
                                End If ''es mascota
                            End If ''hay npc

                        Next Y
                    Next X
            End Select


        Catch ex As Exception
            Console.WriteLine("Error in MagoDestruyeWand: " & ex.Message)
            LogError(("Error en NPCAI.MagoDestruyeWand "))
        End Try
    End Sub


    Sub GreedyWalkTo(npcorig As Short, Map As Short, X As Short, Y As Short)
        Try
            ''  Este procedimiento es llamado cada vez que un NPC deba ir
            ''  a otro lugar en el mismo mapa. Utiliza una técnica
            ''  de programación greedy no determinística.
            ''  Cada paso azaroso que me acerque al destino, es un buen paso.
            ''  Si no hay mejor paso válido, entonces hay que volver atrás y reintentar.
            ''  Si no puedo moverme, me considero piketeado
            ''  La funcion es larga, pero es O(1) - orden algorítmico temporal constante

            'Rapsodius - Changed Mod by And for speed

            Dim NPCx As Short
            Dim NPCy As Short
            Dim USRx As Short
            Dim USRy As Short
            Dim dual As Short
            Dim mapa As Short

            If Not (Npclist(npcorig).Pos.Map = Map) Then Exit Sub ''si son distintos mapas abort

            NPCx = Npclist(npcorig).Pos.X
            NPCy = Npclist(npcorig).Pos.Y

            If (NPCx = X And NPCy = Y) Then Exit Sub ''ya llegué!!


            ''  Levanto las coordenadas del destino
            USRx = X
            USRy = Y
            mapa = Map

            ''  moverse
            If (NPCx > USRx) Then
                If (NPCy < USRy) Then
                    ''NPC esta arriba a la derecha
                    dual = RandomNumber(0, 10)
                    If ((dual And 1) = 0) Then ''move down
                        If LegalPos(mapa, NPCx, NPCy + 1) Then
                            Call MoverAba(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx - 1, NPCy) Then
                            Call MoverIzq(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx + 1, NPCy) Then
                            Call MoverDer(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx, NPCy - 1) Then
                            Call MoverArr(npcorig)
                            Exit Sub
                        Else
                            ''aqui no puedo ir a ningun lado. Hay q ver si me bloquean caspers
                            If CasperBlock(npcorig) Then Call LiberarCasperBlock(npcorig)
                        End If

                    Else ''random first move
                        If LegalPos(mapa, NPCx - 1, NPCy) Then
                            Call MoverIzq(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx, NPCy + 1) Then
                            Call MoverAba(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx + 1, NPCy) Then
                            Call MoverDer(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx, NPCy - 1) Then
                            Call MoverArr(npcorig)
                            Exit Sub
                        Else
                            If CasperBlock(npcorig) Then Call LiberarCasperBlock(npcorig)
                        End If
                    End If ''checked random first move
                ElseIf (NPCy > USRy) Then ''NPC esta abajo a la derecha
                    dual = RandomNumber(0, 10)
                    If ((dual And 1) = 0) Then ''move up
                        If LegalPos(mapa, NPCx, NPCy - 1) Then ''U
                            Call MoverArr(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx - 1, NPCy) Then ''L
                            Call MoverIzq(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx, NPCy + 1) Then ''D
                            Call MoverAba(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx + 1, NPCy) Then ''R
                            Call MoverDer(npcorig)
                            Exit Sub
                        Else
                            If CasperBlock(npcorig) Then Call LiberarCasperBlock(npcorig)
                        End If
                    Else ''random first move
                        If LegalPos(mapa, NPCx - 1, NPCy) Then ''L
                            Call MoverIzq(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx, NPCy - 1) Then ''U
                            Call MoverArr(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx, NPCy + 1) Then ''D
                            Call MoverAba(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx + 1, NPCy) Then ''R
                            Call MoverDer(npcorig)
                            Exit Sub
                        Else
                            If CasperBlock(npcorig) Then Call LiberarCasperBlock(npcorig)
                        End If
                    End If ''endif random first move
                Else ''x completitud, esta en la misma Y
                    If LegalPos(mapa, NPCx - 1, NPCy) Then ''L
                        Call MoverIzq(npcorig)
                        Exit Sub
                    ElseIf LegalPos(mapa, NPCx, NPCy + 1) Then ''D
                        Call MoverAba(npcorig)
                        Exit Sub
                    ElseIf LegalPos(mapa, NPCx, NPCy - 1) Then ''U
                        Call MoverArr(npcorig)
                        Exit Sub
                    Else
                        ''si me muevo abajo entro en loop. Aca el algoritmo falla
                        If Npclist(npcorig).CanAttack = 1 And (RandomNumber(1, 100) > 95) Then
                            Call _
                                SendData(SendTarget.ToNPCArea, npcorig,
                                         PrepareMessageChatOverHead("Maldito bastardo, ¡Ven aquí!",
                                                                    CShort(Str(Npclist(npcorig).Char_Renamed.CharIndex)),
                                                                    ColorTranslator.ToOle(
                                                                        Color.Yellow)))
                            Npclist(npcorig).CanAttack = 0
                        End If
                    End If
                End If

            ElseIf (NPCx < USRx) Then

                If (NPCy < USRy) Then
                    ''NPC esta arriba a la izquierda
                    dual = RandomNumber(0, 10)
                    If ((dual And 1) = 0) Then ''move down
                        If LegalPos(mapa, NPCx, NPCy + 1) Then ''ABA
                            Call MoverAba(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx + 1, NPCy) Then ''R
                            Call MoverDer(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx - 1, NPCy) Then
                            Call MoverIzq(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx, NPCy - 1) Then
                            Call MoverArr(npcorig)
                            Exit Sub
                        Else
                            If CasperBlock(npcorig) Then Call LiberarCasperBlock(npcorig)
                        End If
                    Else ''random first move
                        If LegalPos(mapa, NPCx + 1, NPCy) Then ''DER
                            Call MoverDer(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx, NPCy + 1) Then ''ABA
                            Call MoverAba(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx - 1, NPCy) Then
                            Call MoverIzq(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx, NPCy - 1) Then
                            Call MoverArr(npcorig)
                            Exit Sub
                        Else
                            If CasperBlock(npcorig) Then Call LiberarCasperBlock(npcorig)
                        End If
                    End If

                ElseIf (NPCy > USRy) Then ''NPC esta abajo a la izquierda
                    dual = RandomNumber(0, 10)
                    If ((dual And 1) = 0) Then ''move up
                        If LegalPos(mapa, NPCx, NPCy - 1) Then ''U
                            Call MoverArr(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx + 1, NPCy) Then ''R
                            Call MoverDer(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx - 1, NPCy) Then ''L
                            Call MoverIzq(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx, NPCy + 1) Then ''D
                            Call MoverAba(npcorig)
                            Exit Sub
                        Else
                            If CasperBlock(npcorig) Then Call LiberarCasperBlock(npcorig)
                        End If
                    Else
                        If LegalPos(mapa, NPCx + 1, NPCy) Then ''R
                            Call MoverDer(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx, NPCy - 1) Then ''U
                            Call MoverArr(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx, NPCy + 1) Then ''D
                            Call MoverAba(npcorig)
                            Exit Sub
                        ElseIf LegalPos(mapa, NPCx - 1, NPCy) Then ''L
                            Call MoverIzq(npcorig)
                            Exit Sub
                        Else
                            If CasperBlock(npcorig) Then Call LiberarCasperBlock(npcorig)
                        End If
                    End If
                Else ''x completitud, esta en la misma Y
                    If LegalPos(mapa, NPCx + 1, NPCy) Then ''R
                        Call MoverDer(npcorig)
                        Exit Sub
                    ElseIf LegalPos(mapa, NPCx, NPCy + 1) Then ''D
                        Call MoverAba(npcorig)
                        Exit Sub
                    ElseIf LegalPos(mapa, NPCx, NPCy - 1) Then ''U
                        Call MoverArr(npcorig)
                        Exit Sub
                    Else
                        ''si me muevo loopeo. aca falla el algoritmo
                        If Npclist(npcorig).CanAttack = 1 And (RandomNumber(1, 100) > 95) Then
                            Call _
                                SendData(SendTarget.ToNPCArea, npcorig,
                                         PrepareMessageChatOverHead("Maldito bastardo, ¡Ven aquí!",
                                                                    Npclist(npcorig).Char_Renamed.CharIndex,
                                                                    ColorTranslator.ToOle(
                                                                        Color.Yellow)))
                            Npclist(npcorig).CanAttack = 0
                        End If
                    End If
                End If


            Else ''igual X
                If (NPCy > USRy) Then ''NPC ESTA ABAJO
                    If LegalPos(mapa, NPCx, NPCy - 1) Then ''U
                        Call MoverArr(npcorig)
                        Exit Sub
                    ElseIf LegalPos(mapa, NPCx + 1, NPCy) Then ''R
                        Call MoverDer(npcorig)
                        Exit Sub
                    ElseIf LegalPos(mapa, NPCx - 1, NPCy) Then ''L
                        Call MoverIzq(npcorig)
                        Exit Sub
                    Else
                        ''aca tambien falla el algoritmo
                        If Npclist(npcorig).CanAttack = 1 And (RandomNumber(1, 100) > 95) Then
                            Call _
                                SendData(SendTarget.ToNPCArea, npcorig,
                                         PrepareMessageChatOverHead("Maldito bastardo, ¡Ven aquí!",
                                                                    Npclist(npcorig).Char_Renamed.CharIndex,
                                                                    ColorTranslator.ToOle(
                                                                        Color.Yellow)))
                            Npclist(npcorig).CanAttack = 0
                        End If
                    End If
                Else ''NPC ESTA ARRIBA
                    If LegalPos(mapa, NPCx, NPCy + 1) Then ''ABA
                        Call MoverAba(npcorig)
                        Exit Sub
                    ElseIf LegalPos(mapa, NPCx + 1, NPCy) Then ''R
                        Call MoverDer(npcorig)
                        Exit Sub
                    ElseIf LegalPos(mapa, NPCx - 1, NPCy) Then ''L
                        Call MoverIzq(npcorig)
                        Exit Sub
                    Else
                        ''posible loop
                        If Npclist(npcorig).CanAttack = 1 And (RandomNumber(1, 100) > 95) Then
                            Call _
                                SendData(SendTarget.ToNPCArea, npcorig,
                                         PrepareMessageChatOverHead("Maldito bastardo, ¡Ven aquí!",
                                                                    Npclist(npcorig).Char_Renamed.CharIndex,
                                                                    ColorTranslator.ToOle(
                                                                        Color.Yellow)))
                            Npclist(npcorig).CanAttack = 0
                        End If
                    End If
                End If
            End If


        Catch ex As Exception
            Console.WriteLine("Error in GreedyWalkTo: " & ex.Message)
            LogError(("Error en NPCAI.GreedyWalkTo"))
        End Try
    End Sub

    Sub MoverAba(npcorig As Short)
        Try

            Dim mapa As Short
            Dim NPCx As Short
            Dim NPCy As Short
            mapa = Npclist(npcorig).Pos.Map
            NPCx = Npclist(npcorig).Pos.X
            NPCy = Npclist(npcorig).Pos.Y

            Call _
                SendData(SendTarget.ToNPCArea, npcorig,
                         PrepareMessageCharacterMove(Npclist(npcorig).Char_Renamed.CharIndex, NPCx, NPCy + 1))
            'Update map and npc pos
            MapData(mapa, NPCx, NPCy).NpcIndex = 0
            Npclist(npcorig).Pos.Y = NPCy + 1
            Npclist(npcorig).Char_Renamed.heading = eHeading.SOUTH
            MapData(mapa, NPCx, NPCy + 1).NpcIndex = npcorig

            'Revisamos sidebemos cambair el área
            Call CheckUpdateNeededNpc(npcorig, eHeading.SOUTH)


        Catch ex As Exception
            Console.WriteLine("Error in MoverAba: " & ex.Message)
            LogError(("Error en NPCAI.MoverAba "))
        End Try
    End Sub

    Sub MoverArr(npcorig As Short)
        Try

            Dim mapa As Short
            Dim NPCx As Short
            Dim NPCy As Short
            mapa = Npclist(npcorig).Pos.Map
            NPCx = Npclist(npcorig).Pos.X
            NPCy = Npclist(npcorig).Pos.Y

            Call _
                SendData(SendTarget.ToNPCArea, npcorig,
                         PrepareMessageCharacterMove(Npclist(npcorig).Char_Renamed.CharIndex, NPCx, NPCy - 1))
            'Update map and npc pos
            MapData(mapa, NPCx, NPCy).NpcIndex = 0
            Npclist(npcorig).Pos.Y = NPCy - 1
            Npclist(npcorig).Char_Renamed.heading = eHeading.NORTH
            MapData(mapa, NPCx, NPCy - 1).NpcIndex = npcorig

            'Revisamos sidebemos cambair el área
            Call CheckUpdateNeededNpc(npcorig, eHeading.NORTH)


        Catch ex As Exception
            Console.WriteLine("Error in MoverArr: " & ex.Message)
            LogError(("Error en NPCAI.MoverArr"))
        End Try
    End Sub

    Sub MoverIzq(npcorig As Short)
        Try

            Dim mapa As Short
            Dim NPCx As Short
            Dim NPCy As Short
            mapa = Npclist(npcorig).Pos.Map
            NPCx = Npclist(npcorig).Pos.X
            NPCy = Npclist(npcorig).Pos.Y

            Call _
                SendData(SendTarget.ToNPCArea, npcorig,
                         PrepareMessageCharacterMove(Npclist(npcorig).Char_Renamed.CharIndex, NPCx - 1, NPCy))
            'Update map and npc pos
            MapData(mapa, NPCx, NPCy).NpcIndex = 0
            Npclist(npcorig).Pos.X = NPCx - 1
            Npclist(npcorig).Char_Renamed.heading = eHeading.WEST
            MapData(mapa, NPCx - 1, NPCy).NpcIndex = npcorig

            'Revisamos sidebemos cambair el área
            Call CheckUpdateNeededNpc(npcorig, eHeading.WEST)


        Catch ex As Exception
            Console.WriteLine("Error in MoverIzq: " & ex.Message)
            LogError(("Error en NPCAI.MoverIzq"))
        End Try
    End Sub

    Sub MoverDer(npcorig As Short)
        Try

            Dim mapa As Short
            Dim NPCx As Short
            Dim NPCy As Short
            mapa = Npclist(npcorig).Pos.Map
            NPCx = Npclist(npcorig).Pos.X
            NPCy = Npclist(npcorig).Pos.Y

            Call _
                SendData(SendTarget.ToNPCArea, npcorig,
                         PrepareMessageCharacterMove(Npclist(npcorig).Char_Renamed.CharIndex, NPCx + 1, NPCy))
            'Update map and npc pos
            MapData(mapa, NPCx, NPCy).NpcIndex = 0
            Npclist(npcorig).Pos.X = NPCx + 1
            Npclist(npcorig).Char_Renamed.heading = eHeading.EAST
            MapData(mapa, NPCx + 1, NPCy).NpcIndex = npcorig

            'Revisamos sidebemos cambair el área
            Call CheckUpdateNeededNpc(npcorig, eHeading.EAST)


        Catch ex As Exception
            Console.WriteLine("Error in MoverDer: " & ex.Message)
            LogError(("Error en NPCAI.MoverDer"))
        End Try
    End Sub


    Sub VolverAlCentro(npcind As Short)
        Try

            Dim NPCPosX As Short
            Dim NPCPosY As Short
            Dim NpcMap As Short
            NPCPosX = Npclist(npcind).Pos.X
            NPCPosY = Npclist(npcind).Pos.Y
            NpcMap = Npclist(npcind).Pos.Map

            If NpcMap = MAPA_PRETORIANO Then
                ''35,25 y 67,25 son las posiciones del rey
                If NPCPosX < 50 Then ''esta a la izquierda
                    Call GreedyWalkTo(npcind, NpcMap, ALCOBA1_X, ALCOBA1_Y)
                    'GreedyWalkTo npcind, NpcMap, 35, 25
                Else
                    Call GreedyWalkTo(npcind, NpcMap, ALCOBA2_X, ALCOBA2_Y)
                    'GreedyWalkTo npcind, NpcMap, 67, 25
                End If
            End If


        Catch ex As Exception
            Console.WriteLine("Error in VolverAlCentro: " & ex.Message)
            LogError(("Error en NPCAI.VolverAlCentro"))
        End Try
    End Sub

    Function EstoyMuyLejos(npcind As Object) As Boolean
        ''me dice si estoy fuera del anillo exterior de proteccion
        ''de los clerigos

        Dim retvalue As Boolean

        'If Npclist(npcind).Pos.X < 50 Then
        '    retvalue = Npclist(npcind).Pos.X < 43 And Npclist(npcind).Pos.X > 27
        'Else
        '    retvalue = Npclist(npcind).Pos.X < 80 And Npclist(npcind).Pos.X > 59
        'End If

        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto npcind. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        retvalue = Npclist(npcind).Pos.Y > 39

        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto npcind. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        If Not Npclist(npcind).Pos.Map = MAPA_PRETORIANO Then
            EstoyMuyLejos = False
        Else
            EstoyMuyLejos = retvalue
        End If

        Exit Function

        errorh:
        LogError(("Error en NPCAI.EstoymUYLejos"))
    End Function

    Function EstoyLejos(npcind As Object) As Boolean
        Try

            ''35,25 y 67,25 son las posiciones del rey
            ''esta fction me indica si estoy lejos del rango de vision


            Dim retvalue As Boolean

            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto npcind. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            If Npclist(npcind).Pos.X < 50 Then
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto npcind. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                retvalue = Npclist(npcind).Pos.X < 43 And Npclist(npcind).Pos.X > 27
            Else
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto npcind. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                retvalue = Npclist(npcind).Pos.X < 75 And Npclist(npcind).Pos.X > 59
            End If

            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto npcind. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            retvalue = retvalue And Npclist(npcind).Pos.Y > 19 And Npclist(npcind).Pos.Y < 31

            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto npcind. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            If Not Npclist(npcind).Pos.Map = MAPA_PRETORIANO Then
                EstoyLejos = False
            Else
                EstoyLejos = Not retvalue
            End If


        Catch ex As Exception
            Console.WriteLine("Error in EstoyMuyLejos: " & ex.Message)
            LogError(("Error en NPCAI.EstoyLejos"))
        End Try
    End Function

    Function EsAlcanzable(npcind As Short, PJEnInd As Short) As Boolean
        Try

            ''esta funcion es especialmente hecha para el mapa pretoriano
            ''Está diseñada para que se ignore a los PJs que estan demasiado lejos
            ''evitando asi que los "lockeen" en la pelea sacandolos de combate
            ''sin matarlos. La fcion es totalmente inutil si los NPCs estan en otro mapa.
            ''Chequea la posibilidad que les hagan /racc desde otro mapa para evitar
            ''malos comportamientos
            ''35,25 y 67,25 son las posiciones del rey
            ''Try


            Dim retvalue As Boolean
            Dim retValue2 As Boolean

            Dim PJPosX As Short
            Dim PJPosY As Short
            Dim NPCPosX As Short
            Dim NPCPosY As Short

            PJPosX = UserList(PJEnInd).Pos.X
            PJPosY = UserList(PJEnInd).Pos.Y
            NPCPosX = Npclist(npcind).Pos.X
            NPCPosY = Npclist(npcind).Pos.Y

            If (Npclist(npcind).Pos.Map = MAPA_PRETORIANO) And (UserList(PJEnInd).Pos.Map = MAPA_PRETORIANO) Then
                ''los bounds del mapa pretoriano son fijos.
                ''Esta en una posicion alcanzable si esta dentro del
                ''espacio de las alcobas reales del mapa diseñado por mi.
                ''Y dentro de la alcoba en el rango del perimetro de defensa
                '' 8+8+8+8 x 7+7+7+7
                retvalue = PJPosX > 18 And PJPosX < 49 And NPCPosX <= 51 'And NPCPosX < 49
                retvalue = retvalue And (PJPosY > 14 And PJPosY < 40) 'And NPCPosY > 14 And NPCPosY < 50)
                retValue2 = PJPosX > 52 And PJPosX < 81 And NPCPosX > 51 'And NPCPosX < 81
                retValue2 = retValue2 And (PJPosY > 14 And PJPosY < 40) 'And NPCPosY > 14 And NPCPosY < 50)
                ''rv dice si estan en la alcoba izquierda los 2 y en zona valida de combate
                ''rv2 dice si estan en la derecha
                retvalue = retvalue Or retValue2
                'If retvalue = False Then
                '    If Npclist(npcind).CanAttack = 1 Then
                '        Call SendData(SendTarget.ToNPCArea, npcind, Npclist(npcind).Pos.Map, "||" & vbYellow & "°¡ Cobarde !°" & str(Npclist(npcind).Char.CharIndex))
                '        Npclist(npcind).CanAttack = 0
                '    End If
                'End If
            Else
                retvalue = False
            End If

            EsAlcanzable = retvalue


        Catch ex As Exception
            Console.WriteLine("Error in EsAlcanzable: " & ex.Message)
            LogError(("Error en NPCAI.EsAlcanzable"))
        End Try
    End Function


    Function CasperBlock(npc As Short) As Boolean
        Try

            Dim NPCPosM As Short
            Dim NPCPosX As Short
            Dim NPCPosY As Short
            Dim PJ As Short

            Dim retvalue As Boolean

            NPCPosX = Npclist(npc).Pos.X
            NPCPosY = Npclist(npc).Pos.Y
            NPCPosM = Npclist(npc).Pos.Map

            retvalue =
                Not _
                (LegalPos(NPCPosM, NPCPosX + 1, NPCPosY) Or LegalPos(NPCPosM, NPCPosX - 1, NPCPosY) Or
                 LegalPos(NPCPosM, NPCPosX, NPCPosY + 1) Or LegalPos(NPCPosM, NPCPosX, NPCPosY - 1))

            If retvalue Then
                ''si son todas invalidas
                ''busco que algun casper sea causante de piketeo
                retvalue = False

                PJ = MapData(NPCPosM, NPCPosX + 1, NPCPosY).UserIndex
                If PJ > 0 Then
                    retvalue = UserList(PJ).flags.Muerto = 1
                End If

                PJ = MapData(NPCPosM, NPCPosX - 1, NPCPosY).UserIndex
                If PJ > 0 Then
                    retvalue = retvalue Or UserList(PJ).flags.Muerto = 1
                End If

                PJ = MapData(NPCPosM, NPCPosX, NPCPosY + 1).UserIndex
                If PJ > 0 Then
                    retvalue = retvalue Or UserList(PJ).flags.Muerto = 1
                End If

                PJ = MapData(NPCPosM, NPCPosX, NPCPosY - 1).UserIndex
                If PJ > 0 Then
                    retvalue = retvalue Or UserList(PJ).flags.Muerto = 1
                End If

            Else
                retvalue = False

            End If

            CasperBlock = retvalue


        Catch ex As Exception
            Console.WriteLine("Error in CasperBlock: " & ex.Message)
            '    MsgBox ("ERROR!!")
            CasperBlock = False
            LogError(("Error en NPCAI.CasperBlock"))
        End Try
    End Function


    Sub LiberarCasperBlock(npcind As Short)
        Try

            Dim NPCPosX As Short
            Dim NPCPosY As Short
            Dim NPCPosM As Short

            NPCPosX = Npclist(npcind).Pos.X
            NPCPosY = Npclist(npcind).Pos.Y
            NPCPosM = Npclist(npcind).Pos.Map

            If LegalPos(NPCPosM, NPCPosX + 1, NPCPosY + 1) Then
                Call _
                    SendData(SendTarget.ToNPCArea, npcind,
                             PrepareMessageCharacterMove(Npclist(npcind).Char_Renamed.CharIndex, NPCPosX + 1,
                                                         NPCPosY + 1))
                'Update map and npc pos
                MapData(NPCPosM, NPCPosX, NPCPosY).NpcIndex = 0
                Npclist(npcind).Pos.Y = NPCPosY + 1
                Npclist(npcind).Pos.X = NPCPosX + 1
                Npclist(npcind).Char_Renamed.heading = eHeading.SOUTH
                MapData(NPCPosM, NPCPosX + 1, NPCPosY + 1).NpcIndex = npcind
                Call _
                    SendData(SendTarget.ToNPCArea, npcind,
                             PrepareMessageChatOverHead("¡¡JA JA JA JA!!", Npclist(npcind).Char_Renamed.CharIndex,
                                                        ColorTranslator.ToOle(Color.Yellow)))
                Exit Sub
            End If

            If LegalPos(NPCPosM, NPCPosX - 1, NPCPosY - 1) Then
                Call _
                    SendData(SendTarget.ToNPCArea, npcind,
                             PrepareMessageCharacterMove(Npclist(npcind).Char_Renamed.CharIndex, NPCPosX - 1,
                                                         NPCPosY - 1))
                'Update map and npc pos
                MapData(NPCPosM, NPCPosX, NPCPosY).NpcIndex = 0
                Npclist(npcind).Pos.Y = NPCPosY - 1
                Npclist(npcind).Pos.X = NPCPosX - 1
                Npclist(npcind).Char_Renamed.heading = eHeading.NORTH
                MapData(NPCPosM, NPCPosX - 1, NPCPosY - 1).NpcIndex = npcind
                Call _
                    SendData(SendTarget.ToNPCArea, npcind,
                             PrepareMessageChatOverHead("¡¡JA JA JA JA!!", Npclist(npcind).Char_Renamed.CharIndex,
                                                        ColorTranslator.ToOle(Color.Yellow)))
                Exit Sub
            End If

            If LegalPos(NPCPosM, NPCPosX + 1, NPCPosY - 1) Then
                Call _
                    SendData(SendTarget.ToNPCArea, npcind,
                             PrepareMessageCharacterMove(Npclist(npcind).Char_Renamed.CharIndex, NPCPosX + 1,
                                                         NPCPosY - 1))
                'Update map and npc pos
                MapData(NPCPosM, NPCPosX, NPCPosY).NpcIndex = 0
                Npclist(npcind).Pos.Y = NPCPosY - 1
                Npclist(npcind).Pos.X = NPCPosX + 1
                Npclist(npcind).Char_Renamed.heading = eHeading.EAST
                MapData(NPCPosM, NPCPosX + 1, NPCPosY - 1).NpcIndex = npcind
                Call _
                    SendData(SendTarget.ToNPCArea, npcind,
                             PrepareMessageChatOverHead("¡¡JA JA JA JA!!", Npclist(npcind).Char_Renamed.CharIndex,
                                                        ColorTranslator.ToOle(Color.Yellow)))
                Exit Sub
            End If

            If LegalPos(NPCPosM, NPCPosX - 1, NPCPosY + 1) Then
                Call _
                    SendData(SendTarget.ToNPCArea, npcind,
                             PrepareMessageCharacterMove(Npclist(npcind).Char_Renamed.CharIndex, NPCPosX - 1,
                                                         NPCPosY + 1))
                'Update map and npc pos
                MapData(NPCPosM, NPCPosX, NPCPosY).NpcIndex = 0
                Npclist(npcind).Pos.Y = NPCPosY + 1
                Npclist(npcind).Pos.X = NPCPosX - 1
                Npclist(npcind).Char_Renamed.heading = eHeading.WEST
                MapData(NPCPosM, NPCPosX - 1, NPCPosY + 1).NpcIndex = npcind
                Call _
                    SendData(SendTarget.ToNPCArea, npcind,
                             PrepareMessageChatOverHead("¡¡JA JA JA JA!!", Npclist(npcind).Char_Renamed.CharIndex,
                                                        ColorTranslator.ToOle(Color.Yellow)))
                Exit Sub
            End If

            ''si esta aca, estamos fritos!
            If Npclist(npcind).CanAttack = 1 Then
                Call _
                    SendData(SendTarget.ToNPCArea, npcind,
                             PrepareMessageChatOverHead(
                                 "¡Por las barbas de los antiguos reyes! ¡Alejáos endemoniados espectros o sufriréis la furia de los dioses!",
                                 Npclist(npcind).Char_Renamed.CharIndex,
                                 ColorTranslator.ToOle(Color.Yellow)))
                Npclist(npcind).CanAttack = 0
            End If


        Catch ex As Exception
            Console.WriteLine("Error in LiberarCasperBlock: " & ex.Message)
            LogError(("Error en NPCAI.LiberarCasperBlock"))
        End Try
    End Sub

    Public Sub CambiarAlcoba(npcind As Short)
        Try

            Select Case Npclist(npcind).Invent.ArmourEqpSlot
                Case 2
                    Call GreedyWalkTo(npcind, MAPA_PRETORIANO, 48, 70)
                    If Npclist(npcind).Pos.X = 48 And Npclist(npcind).Pos.Y = 70 Then _
                        Npclist(npcind).Invent.ArmourEqpSlot = Npclist(npcind).Invent.ArmourEqpSlot + 1
                Case 6
                    Call GreedyWalkTo(npcind, MAPA_PRETORIANO, 52, 71)
                    If Npclist(npcind).Pos.X = 52 And Npclist(npcind).Pos.Y = 71 Then _
                        Npclist(npcind).Invent.ArmourEqpSlot = Npclist(npcind).Invent.ArmourEqpSlot + 1
                Case 1
                    Call GreedyWalkTo(npcind, MAPA_PRETORIANO, 73, 56)
                    If Npclist(npcind).Pos.X = 73 And Npclist(npcind).Pos.Y = 56 Then _
                        Npclist(npcind).Invent.ArmourEqpSlot = Npclist(npcind).Invent.ArmourEqpSlot + 1
                Case 7
                    Call GreedyWalkTo(npcind, MAPA_PRETORIANO, 73, 48)
                    If Npclist(npcind).Pos.X = 73 And Npclist(npcind).Pos.Y = 48 Then _
                        Npclist(npcind).Invent.ArmourEqpSlot = Npclist(npcind).Invent.ArmourEqpSlot + 1
                Case 5
                    Call GreedyWalkTo(npcind, MAPA_PRETORIANO, 31, 56)
                    If Npclist(npcind).Pos.X = 31 And Npclist(npcind).Pos.Y = 56 Then _
                        Npclist(npcind).Invent.ArmourEqpSlot = Npclist(npcind).Invent.ArmourEqpSlot + 1
                Case 3
                    Call GreedyWalkTo(npcind, MAPA_PRETORIANO, 31, 48)
                    If Npclist(npcind).Pos.X = 31 And Npclist(npcind).Pos.Y = 48 Then _
                        Npclist(npcind).Invent.ArmourEqpSlot = Npclist(npcind).Invent.ArmourEqpSlot + 1
                Case 4, 8
                    Npclist(npcind).Invent.ArmourEqpSlot = 0
                    Exit Sub
            End Select


        Catch ex As Exception
            Console.WriteLine("Error in CambiarAlcoba: " & ex.Message)
            Call LogError("Error en CambiarAlcoba " & Err.Description)
        End Try
    End Sub
End Module
