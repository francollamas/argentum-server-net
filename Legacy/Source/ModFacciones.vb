Option Strict Off
Option Explicit On

Imports System.Drawing

Module ModFacciones
    Public ArmaduraImperial1 As Short
    Public ArmaduraImperial2 As Short
    Public ArmaduraImperial3 As Short
    Public TunicaMagoImperial As Short
    Public TunicaMagoImperialEnanos As Short
    Public ArmaduraCaos1 As Short
    Public ArmaduraCaos2 As Short
    Public ArmaduraCaos3 As Short
    Public TunicaMagoCaos As Short
    Public TunicaMagoCaosEnanos As Short

    Public VestimentaImperialHumano As Short
    Public VestimentaImperialEnano As Short
    Public TunicaConspicuaHumano As Short
    Public TunicaConspicuaEnano As Short
    Public ArmaduraNobilisimaHumano As Short
    Public ArmaduraNobilisimaEnano As Short
    Public ArmaduraGranSacerdote As Short

    Public VestimentaLegionHumano As Short
    Public VestimentaLegionEnano As Short
    Public TunicaLobregaHumano As Short
    Public TunicaLobregaEnano As Short
    Public TunicaEgregiaHumano As Short
    Public TunicaEgregiaEnano As Short
    Public SacerdoteDemoniaco As Short


    Public Const NUM_RANGOS_FACCION As Short = 15
    Private Const NUM_DEF_FACCION_ARMOURS As Byte = 3

    Public Enum eTipoDefArmors
        ieBaja
        ieMedia
        ieAlta
    End Enum

    Public Structure tFaccionArmaduras
        <VBFixedArray(NUM_DEF_FACCION_ARMOURS - 1)> Dim Armada() As Short
        <VBFixedArray(NUM_DEF_FACCION_ARMOURS - 1)> Dim Caos() As Short

        'UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        Public Sub Initialize()
            ReDim Armada(NUM_DEF_FACCION_ARMOURS - 1)
            ReDim Caos(NUM_DEF_FACCION_ARMOURS - 1)
        End Sub
    End Structure

    ' Matriz que contiene las armaduras faccionarias segun raza, clase, faccion y defensa de armadura
    'UPGRADE_WARNING: El límite inferior de la matriz ArmadurasFaccion ha cambiado de 1,1 a 0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    'UPGRADE_WARNING: Es posible que la matriz ArmadurasFaccion necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
    Public ArmadurasFaccion(NUMCLASES, NUMRAZAS) As tFaccionArmaduras

    ' Contiene la cantidad de exp otorgada cada vez que aumenta el rango
    Public RecompensaFacciones(NUM_RANGOS_FACCION) As Integer

    Private Function GetArmourAmount(Rango As Short, TipoDef As eTipoDefArmors) As Short
        '***************************************************
        'Autor: ZaMa
        'Last Modification: 15/04/2010
        'Returns the amount of armours to give, depending on the specified rank
        '***************************************************

        Select Case TipoDef

            Case eTipoDefArmors.ieBaja
                GetArmourAmount = 20/(Rango + 1)

            Case eTipoDefArmors.ieMedia
                GetArmourAmount = Rango*2/MaximoInt(Rango - 4, 1)

            Case eTipoDefArmors.ieAlta
                GetArmourAmount = Rango*1.35

        End Select
    End Function

    Private Sub GiveFactionArmours(UserIndex As Short, IsCaos As Boolean)
        '***************************************************
        'Autor: ZaMa
        'Last Modification: 15/04/2010
        'Gives faction armours to user
        '***************************************************

        Dim ObjArmour As Obj
        Dim Rango As Short

        With UserList(UserIndex)

            Rango = Val(IIf(IsCaos, .Faccion.RecompensasCaos, .Faccion.RecompensasReal)) + 1


            ' Entrego armaduras de defensa baja
            ObjArmour.Amount = GetArmourAmount(Rango, eTipoDefArmors.ieBaja)

            If IsCaos Then
                ObjArmour.ObjIndex = ArmadurasFaccion(.clase, .raza).Caos(eTipoDefArmors.ieBaja)
            Else
                ObjArmour.ObjIndex = ArmadurasFaccion(.clase, .raza).Armada(eTipoDefArmors.ieBaja)
            End If

            If Not MeterItemEnInventario(UserIndex, ObjArmour) Then
                Call TirarItemAlPiso(.Pos, ObjArmour)
            End If


            ' Entrego armaduras de defensa media
            ObjArmour.Amount = GetArmourAmount(Rango, eTipoDefArmors.ieMedia)

            If IsCaos Then
                ObjArmour.ObjIndex = ArmadurasFaccion(.clase, .raza).Caos(eTipoDefArmors.ieMedia)
            Else
                ObjArmour.ObjIndex = ArmadurasFaccion(.clase, .raza).Armada(eTipoDefArmors.ieMedia)
            End If

            If Not MeterItemEnInventario(UserIndex, ObjArmour) Then
                Call TirarItemAlPiso(.Pos, ObjArmour)
            End If


            ' Entrego armaduras de defensa alta
            ObjArmour.Amount = GetArmourAmount(Rango, eTipoDefArmors.ieAlta)

            If IsCaos Then
                ObjArmour.ObjIndex = ArmadurasFaccion(.clase, .raza).Caos(eTipoDefArmors.ieAlta)
            Else
                ObjArmour.ObjIndex = ArmadurasFaccion(.clase, .raza).Armada(eTipoDefArmors.ieAlta)
            End If

            If Not MeterItemEnInventario(UserIndex, ObjArmour) Then
                Call TirarItemAlPiso(.Pos, ObjArmour)
            End If

        End With
    End Sub

    Public Sub GiveExpReward(UserIndex As Short, Rango As Integer)
        '***************************************************
        'Autor: ZaMa
        'Last Modification: 15/04/2010
        'Gives reward exp to user
        '***************************************************

        Dim GivenExp As Integer

        With UserList(UserIndex)

            GivenExp = RecompensaFacciones(Rango)

            .Stats.Exp = .Stats.Exp + GivenExp

            If .Stats.Exp > MAXEXP Then .Stats.Exp = MAXEXP

            Call _
                WriteConsoleMsg(UserIndex, "Has sido recompensado con " & GivenExp & " puntos de experiencia.",
                                FontTypeNames.FONTTYPE_FIGHT)

            Call CheckUserLevel(UserIndex)

        End With
    End Sub

    Public Sub EnlistarArmadaReal(UserIndex As Short)
        '***************************************************
        'Autor: Pablo (ToxicWaste) & Unknown (orginal version)
        'Last Modification: 15/04/2010
        'Handles the entrance of users to the "Armada Real"
        '15/03/2009: ZaMa - No se puede enlistar el fundador de un clan con alineación neutral.
        '27/11/2009: ZaMa - Ahora no se puede enlistar un miembro de un clan neutro, por ende saque la antifaccion.
        '15/04/2010: ZaMa - Cambio en recompensas iniciales.
        '***************************************************

        With UserList(UserIndex)
            If .Faccion.ArmadaReal = 1 Then
                Call _
                    WriteChatOverHead(UserIndex, "¡¡¡Ya perteneces a las tropas reales!!! Ve a combatir criminales.",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            If .Faccion.FuerzasCaos = 1 Then
                Call _
                    WriteChatOverHead(UserIndex, "¡¡¡Maldito insolente!!! Vete de aquí seguidor de las sombras.",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            If criminal(UserIndex) Then
                Call _
                    WriteChatOverHead(UserIndex, "¡¡¡No se permiten criminales en el ejército real!!!",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            If .Faccion.CriminalesMatados < 30 Then
                Call _
                    WriteChatOverHead(UserIndex,
                                      "Para unirte a nuestras fuerzas debes matar al menos 30 criminales, sólo has matado " &
                                      .Faccion.CriminalesMatados & ".",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            If .Stats.ELV < 25 Then
                Call _
                    WriteChatOverHead(UserIndex, "¡¡¡Para unirte a nuestras fuerzas debes ser al menos de nivel 25!!!",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            If .Faccion.CiudadanosMatados > 0 Then
                Call _
                    WriteChatOverHead(UserIndex,
                                      "¡Has asesinado gente inocente, no aceptamos asesinos en las tropas reales!",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            If .Faccion.Reenlistadas > 4 Then
                Call _
                    WriteChatOverHead(UserIndex, "¡Has sido expulsado de las fuerzas reales demasiadas veces!",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            If .Reputacion.NobleRep < 1000000 Then
                Call _
                    WriteChatOverHead(UserIndex,
                                      "Necesitas ser aún más noble para integrar el ejército real, sólo tienes " &
                                      .Reputacion.NobleRep & "/1.000.000 puntos de nobleza",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            If .GuildIndex > 0 Then
                If GuildAlignment(.GuildIndex) = "Neutral" Then
                    Call _
                        WriteChatOverHead(UserIndex,
                                          "¡¡¡Perteneces a un clan neutro, sal de él si quieres unirte a nuestras fuerzas!!!",
                                          CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                          ColorTranslator.ToOle(Color.White))
                    Exit Sub
                End If
            End If

            .Faccion.ArmadaReal = 1
            .Faccion.Reenlistadas = .Faccion.Reenlistadas + 1

            Call _
                WriteChatOverHead(UserIndex,
                                  "¡¡¡Bienvenido al ejército real!!! Aquí tienes tus vestimentas. Cumple bien tu labor exterminando criminales y me encargaré de recompensarte.",
                                  CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                  ColorTranslator.ToOle(Color.White))

            ' TODO: Dejo esta variable por ahora, pero con chequear las reenlistadas deberia ser suficiente :S
            If .Faccion.RecibioArmaduraReal = 0 Then

                Call GiveFactionArmours(UserIndex, False)
                Call GiveExpReward(UserIndex, 0)

                .Faccion.RecibioArmaduraReal = 1
                .Faccion.NivelIngreso = .Stats.ELV
                .Faccion.FechaIngreso = CStr(Today)
                'Esto por ahora es inútil, siempre va a ser cero, pero bueno, despues va a servir.
                .Faccion.MatadosIngreso = .Faccion.CiudadanosMatados

                .Faccion.RecibioExpInicialReal = 1
                .Faccion.RecompensasReal = 0
                .Faccion.NextRecompensa = 70

            End If

            If .flags.Navegando Then Call RefreshCharStatus(UserIndex) 'Actualizamos la barca si esta navegando (NicoNZ)

            Call LogEjercitoReal(.name & " ingresó el " & Today & " cuando era nivel " & .Stats.ELV)
        End With
    End Sub

    Public Sub RecompensaArmadaReal(UserIndex As Short)
        '***************************************************
        'Autor: Pablo (ToxicWaste) & Unknown (orginal version)
        'Last Modification: 15/04/2010
        'Handles the way of gaining new ranks in the "Armada Real"
        '15/04/2010: ZaMa - Agrego recompensas de oro y armaduras
        '***************************************************
        Dim Crimis As Integer
        Dim Lvl As Byte
        Dim NextRecom As Integer
        Dim Nobleza As Integer

        With UserList(UserIndex)
            Lvl = .Stats.ELV
            Crimis = .Faccion.CriminalesMatados
            NextRecom = .Faccion.NextRecompensa
            Nobleza = .Reputacion.NobleRep

            If Crimis < NextRecom Then
                Call _
                    WriteChatOverHead(UserIndex,
                                      "Mata " & NextRecom - Crimis &
                                      " criminales más para recibir la próxima recompensa.",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            Select Case NextRecom
                Case 70
                    .Faccion.RecompensasReal = 1
                    .Faccion.NextRecompensa = 130

                Case 130
                    .Faccion.RecompensasReal = 2
                    .Faccion.NextRecompensa = 210

                Case 210
                    .Faccion.RecompensasReal = 3
                    .Faccion.NextRecompensa = 320

                Case 320
                    .Faccion.RecompensasReal = 4
                    .Faccion.NextRecompensa = 460

                Case 460
                    .Faccion.RecompensasReal = 5
                    .Faccion.NextRecompensa = 640

                Case 640
                    If Lvl < 27 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes criminales, pero te faltan " & 27 - Lvl &
                                              " niveles para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    .Faccion.RecompensasReal = 6
                    .Faccion.NextRecompensa = 870

                Case 870
                    .Faccion.RecompensasReal = 7
                    .Faccion.NextRecompensa = 1160

                Case 1160
                    .Faccion.RecompensasReal = 8
                    .Faccion.NextRecompensa = 2000

                Case 2000
                    If Lvl < 30 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes criminales, pero te faltan " & 30 - Lvl &
                                              " niveles para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    .Faccion.RecompensasReal = 9
                    .Faccion.NextRecompensa = 2500

                Case 2500
                    If Nobleza < 2000000 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes criminales, pero te faltan " & 2000000 - Nobleza &
                                              " puntos de nobleza para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    .Faccion.RecompensasReal = 10
                    .Faccion.NextRecompensa = 3000

                Case 3000
                    If Nobleza < 3000000 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes criminales, pero te faltan " & 3000000 - Nobleza &
                                              " puntos de nobleza para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    .Faccion.RecompensasReal = 11
                    .Faccion.NextRecompensa = 3500

                Case 3500
                    If Lvl < 35 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes criminales, pero te faltan " & 35 - Lvl &
                                              " niveles para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    If Nobleza < 4000000 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes criminales, pero te faltan " & 4000000 - Nobleza &
                                              " puntos de nobleza para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    .Faccion.RecompensasReal = 12
                    .Faccion.NextRecompensa = 4000

                Case 4000
                    If Lvl < 36 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes criminales, pero te faltan " & 36 - Lvl &
                                              " niveles para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    If Nobleza < 5000000 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes criminales, pero te faltan " & 5000000 - Nobleza &
                                              " puntos de nobleza para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    .Faccion.RecompensasReal = 13
                    .Faccion.NextRecompensa = 5000

                Case 5000
                    If Lvl < 37 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes criminales, pero te faltan " & 37 - Lvl &
                                              " niveles para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    If Nobleza < 6000000 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes criminales, pero te faltan " & 6000000 - Nobleza &
                                              " puntos de nobleza para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    .Faccion.RecompensasReal = 14
                    .Faccion.NextRecompensa = 10000

                Case 10000
                    Call _
                        WriteChatOverHead(UserIndex,
                                          "Eres uno de mis mejores soldados. Mataste " & Crimis &
                                          " criminales, sigue así. Ya no tengo más recompensa para darte que mi agradecimiento. ¡Felicidades!",
                                          CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                          ColorTranslator.ToOle(Color.White))
                    Exit Sub

                Case Else
                    Exit Sub
            End Select

            Call _
                WriteChatOverHead(UserIndex, "¡¡¡Aquí tienes tu recompensa " & TituloReal(UserIndex) & "!!!",
                                  CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                  ColorTranslator.ToOle(Color.White))

            ' Recompensas de armaduras y exp
            Call GiveFactionArmours(UserIndex, False)
            Call GiveExpReward(UserIndex, .Faccion.RecompensasReal)

        End With
    End Sub

    Public Sub ExpulsarFaccionReal(UserIndex As Short, Optional ByRef Expulsado As Boolean = True)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        With UserList(UserIndex)
            .Faccion.ArmadaReal = 0
            'Call PerderItemsFaccionarios(UserIndex)
            If Expulsado Then
                Call _
                    WriteConsoleMsg(UserIndex, "¡¡¡Has sido expulsado del ejército real!!!",
                                    FontTypeNames.FONTTYPE_FIGHT)
            Else
                Call _
                    WriteConsoleMsg(UserIndex, "¡¡¡Te has retirado del ejército real!!!",
                                    FontTypeNames.FONTTYPE_FIGHT)
            End If

            If .Invent.ArmourEqpObjIndex <> 0 Then
                'Desequipamos la armadura real si está equipada
                If ObjData_Renamed(.Invent.ArmourEqpObjIndex).Real = 1 Then _
                    Call Desequipar(UserIndex, .Invent.ArmourEqpSlot)
            End If

            If .Invent.EscudoEqpObjIndex <> 0 Then
                'Desequipamos el escudo de caos si está equipado
                If ObjData_Renamed(.Invent.EscudoEqpObjIndex).Real = 1 Then _
                    Call Desequipar(UserIndex, .Invent.EscudoEqpObjIndex)
            End If

            If .flags.Navegando Then Call RefreshCharStatus(UserIndex) 'Actualizamos la barca si esta navegando (NicoNZ)
        End With
    End Sub

    Public Sub ExpulsarFaccionCaos(UserIndex As Short, Optional ByRef Expulsado As Boolean = True)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        With UserList(UserIndex)
            .Faccion.FuerzasCaos = 0
            'Call PerderItemsFaccionarios(UserIndex)
            If Expulsado Then
                Call _
                    WriteConsoleMsg(UserIndex, "¡¡¡Has sido expulsado de la Legión Oscura!!!",
                                    FontTypeNames.FONTTYPE_FIGHT)
            Else
                Call _
                    WriteConsoleMsg(UserIndex, "¡¡¡Te has retirado de la Legión Oscura!!!",
                                    FontTypeNames.FONTTYPE_FIGHT)
            End If

            If .Invent.ArmourEqpObjIndex <> 0 Then
                'Desequipamos la armadura de caos si está equipada
                If ObjData_Renamed(.Invent.ArmourEqpObjIndex).Caos = 1 Then _
                    Call Desequipar(UserIndex, .Invent.ArmourEqpSlot)
            End If

            If .Invent.EscudoEqpObjIndex <> 0 Then
                'Desequipamos el escudo de caos si está equipado
                If ObjData_Renamed(.Invent.EscudoEqpObjIndex).Caos = 1 Then _
                    Call Desequipar(UserIndex, .Invent.EscudoEqpObjIndex)
            End If

            If .flags.Navegando Then Call RefreshCharStatus(UserIndex) 'Actualizamos la barca si esta navegando (NicoNZ)
        End With
    End Sub

    Public Function TituloReal(UserIndex As Short) As String
        '***************************************************
        'Autor: Unknown
        'Last Modification: 23/01/2007 Pablo (ToxicWaste)
        'Handles the titles of the members of the "Armada Real"
        '***************************************************

        Select Case UserList(UserIndex).Faccion.RecompensasReal
            'Rango 1: Aprendiz (30 Criminales)
            'Rango 2: Escudero (70 Criminales)
            'Rango 3: Soldado (130 Criminales)
            'Rango 4: Sargento (210 Criminales)
            'Rango 5: Caballero (320 Criminales)
            'Rango 6: Comandante (460 Criminales)
            'Rango 7: Capitán (640 Criminales + > lvl 27)
            'Rango 8: Senescal (870 Criminales)
            'Rango 9: Mariscal (1160 Criminales)
            'Rango 10: Condestable (2000 Criminales + > lvl 30)
            'Rangos de Honor de la Armada Real: (Consejo de Bander)
            'Rango 11: Ejecutor Imperial (2500 Criminales + 2.000.000 Nobleza)
            'Rango 12: Protector del Reino (3000 Criminales + 3.000.000 Nobleza)
            'Rango 13: Avatar de la Justicia (3500 Criminales + 4.000.000 Nobleza + > lvl 35)
            'Rango 14: Guardián del Bien (4000 Criminales + 5.000.000 Nobleza + > lvl 36)
            'Rango 15: Campeón de la Luz (5000 Criminales + 6.000.000 Nobleza + > lvl 37)

            Case 0
                TituloReal = "Aprendiz"
            Case 1
                TituloReal = "Escudero"
            Case 2
                TituloReal = "Soldado"
            Case 3
                TituloReal = "Sargento"
            Case 4
                TituloReal = "Teniente"
            Case 5
                TituloReal = "Comandante"
            Case 6
                TituloReal = "Capitán"
            Case 7
                TituloReal = "Senescal"
            Case 8
                TituloReal = "Mariscal"
            Case 9
                TituloReal = "Condestable"
            Case 10
                TituloReal = "Ejecutor Imperial"
            Case 11
                TituloReal = "Protector del Reino"
            Case 12
                TituloReal = "Avatar de la Justicia"
            Case 13
                TituloReal = "Guardián del Bien"
            Case Else
                TituloReal = "Campeón de la Luz"
        End Select
    End Function

    Public Sub EnlistarCaos(UserIndex As Short)
        '***************************************************
        'Autor: Pablo (ToxicWaste) & Unknown (orginal version)
        'Last Modification: 27/11/2009
        '15/03/2009: ZaMa - No se puede enlistar el fundador de un clan con alineación neutral.
        '27/11/2009: ZaMa - Ahora no se puede enlistar un miembro de un clan neutro, por ende saque la antifaccion.
        'Handles the entrance of users to the "Legión Oscura"
        '***************************************************

        With UserList(UserIndex)
            If Not criminal(UserIndex) Then
                Call _
                    WriteChatOverHead(UserIndex, "¡¡¡Lárgate de aquí, bufón!!!",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            If .Faccion.FuerzasCaos = 1 Then
                Call _
                    WriteChatOverHead(UserIndex, "¡¡¡Ya perteneces a la legión oscura!!!",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            If .Faccion.ArmadaReal = 1 Then
                Call _
                    WriteChatOverHead(UserIndex, "Las sombras reinarán en Argentum. ¡¡¡Fuera de aquí insecto real!!!",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            '[Barrin 17-12-03] Si era miembro de la Armada Real no se puede enlistar
            If .Faccion.RecibioExpInicialReal = 1 Then 'Tomamos el valor de ahí: ¿Recibio la experiencia para entrar?
                Call _
                    WriteChatOverHead(UserIndex, "No permitiré que ningún insecto real ingrese a mis tropas.",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If
            '[/Barrin]

            If Not criminal(UserIndex) Then
                Call _
                    WriteChatOverHead(UserIndex, "¡¡Ja ja ja!! Tú no eres bienvenido aquí asqueroso ciudadano.",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            If .Faccion.CiudadanosMatados < 70 Then
                Call _
                    WriteChatOverHead(UserIndex,
                                      "Para unirte a nuestras fuerzas debes matar al menos 70 ciudadanos, sólo has matado " &
                                      .Faccion.CiudadanosMatados & ".",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            If .Stats.ELV < 25 Then
                Call _
                    WriteChatOverHead(UserIndex, "¡¡¡Para unirte a nuestras fuerzas debes ser al menos nivel 25!!!",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            If .GuildIndex > 0 Then
                If GuildAlignment(.GuildIndex) = "Neutral" Then
                    Call _
                        WriteChatOverHead(UserIndex,
                                          "¡¡¡Perteneces a un clan neutro, sal de él si quieres unirte a nuestras fuerzas!!!",
                                          CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                          ColorTranslator.ToOle(Color.White))
                    Exit Sub
                End If
            End If


            If .Faccion.Reenlistadas > 4 Then
                If .Faccion.Reenlistadas = 200 Then
                    Call _
                        WriteChatOverHead(UserIndex,
                                          "Has sido expulsado de las fuerzas oscuras y durante tu rebeldía has atacado a mi ejército. ¡Vete de aquí!",
                                          CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                          ColorTranslator.ToOle(Color.White))
                Else
                    Call _
                        WriteChatOverHead(UserIndex, "¡Has sido expulsado de las fuerzas oscuras demasiadas veces!",
                                          CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                          ColorTranslator.ToOle(Color.White))
                End If
                Exit Sub
            End If

            .Faccion.Reenlistadas = .Faccion.Reenlistadas + 1
            .Faccion.FuerzasCaos = 1

            Call _
                WriteChatOverHead(UserIndex,
                                  "¡¡¡Bienvenido al lado oscuro!!! Aquí tienes tus armaduras. Derrama sangre ciudadana y real, y serás recompensado, lo prometo.",
                                  CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                  ColorTranslator.ToOle(Color.White))

            If .Faccion.RecibioArmaduraCaos = 0 Then

                Call GiveFactionArmours(UserIndex, True)
                Call GiveExpReward(UserIndex, 0)

                .Faccion.RecibioArmaduraCaos = 1
                .Faccion.NivelIngreso = .Stats.ELV
                .Faccion.FechaIngreso = CStr(Today)

                .Faccion.RecibioExpInicialCaos = 1
                .Faccion.RecompensasCaos = 0
                .Faccion.NextRecompensa = 160
            End If

            If .flags.Navegando Then Call RefreshCharStatus(UserIndex) 'Actualizamos la barca si esta navegando (NicoNZ)

            Call LogEjercitoCaos(.name & " ingresó el " & Today & " cuando era nivel " & .Stats.ELV)
        End With
    End Sub

    Public Sub RecompensaCaos(UserIndex As Short)
        '***************************************************
        'Author: Pablo (ToxicWaste) & Unknown (orginal version)
        'Last Modification: 15/04/2010
        'Handles the way of gaining new ranks in the "Legión Oscura"
        '15/04/2010: ZaMa - Agrego recompensas de oro y armaduras
        '***************************************************
        Dim Ciudas As Integer
        Dim Lvl As Byte
        Dim NextRecom As Integer

        With UserList(UserIndex)
            Lvl = .Stats.ELV
            Ciudas = .Faccion.CiudadanosMatados
            NextRecom = .Faccion.NextRecompensa

            If Ciudas < NextRecom Then
                Call _
                    WriteChatOverHead(UserIndex,
                                      "Mata " & NextRecom - Ciudas &
                                      " cuidadanos más para recibir la próxima recompensa.",
                                      CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                      ColorTranslator.ToOle(Color.White))
                Exit Sub
            End If

            Select Case NextRecom
                Case 160
                    .Faccion.RecompensasCaos = 1
                    .Faccion.NextRecompensa = 300

                Case 300
                    .Faccion.RecompensasCaos = 2
                    .Faccion.NextRecompensa = 490

                Case 490
                    .Faccion.RecompensasCaos = 3
                    .Faccion.NextRecompensa = 740

                Case 740
                    .Faccion.RecompensasCaos = 4
                    .Faccion.NextRecompensa = 1100

                Case 1100
                    .Faccion.RecompensasCaos = 5
                    .Faccion.NextRecompensa = 1500

                Case 1500
                    If Lvl < 27 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes ciudadanos, pero te faltan " & 27 - Lvl &
                                              " niveles para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    .Faccion.RecompensasCaos = 6
                    .Faccion.NextRecompensa = 2010

                Case 2010
                    .Faccion.RecompensasCaos = 7
                    .Faccion.NextRecompensa = 2700

                Case 2700
                    .Faccion.RecompensasCaos = 8
                    .Faccion.NextRecompensa = 4600

                Case 4600
                    If Lvl < 30 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes ciudadanos, pero te faltan " & 30 - Lvl &
                                              " niveles para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    .Faccion.RecompensasCaos = 9
                    .Faccion.NextRecompensa = 5800

                Case 5800
                    If Lvl < 31 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes ciudadanos, pero te faltan " & 31 - Lvl &
                                              " niveles para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    .Faccion.RecompensasCaos = 10
                    .Faccion.NextRecompensa = 6990

                Case 6990
                    If Lvl < 33 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes ciudadanos, pero te faltan " & 33 - Lvl &
                                              " niveles para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    .Faccion.RecompensasCaos = 11
                    .Faccion.NextRecompensa = 8100

                Case 8100
                    If Lvl < 35 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes ciudadanos, pero te faltan " & 35 - Lvl &
                                              " niveles para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    .Faccion.RecompensasCaos = 12
                    .Faccion.NextRecompensa = 9300

                Case 9300
                    If Lvl < 36 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes ciudadanos, pero te faltan " & 36 - Lvl &
                                              " niveles para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    .Faccion.RecompensasCaos = 13
                    .Faccion.NextRecompensa = 11500

                Case 11500
                    If Lvl < 37 Then
                        Call _
                            WriteChatOverHead(UserIndex,
                                              "Mataste suficientes ciudadanos, pero te faltan " & 37 - Lvl &
                                              " niveles para poder recibir la próxima recompensa.",
                                              CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                              ColorTranslator.ToOle(Color.White))
                        Exit Sub
                    End If
                    .Faccion.RecompensasCaos = 14
                    .Faccion.NextRecompensa = 23000

                Case 23000
                    Call _
                        WriteChatOverHead(UserIndex,
                                          "Eres uno de mis mejores soldados. Mataste " & Ciudas &
                                          " ciudadanos . Tu única recompensa será la sangre derramada. ¡¡Continúa así!!",
                                          CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                          ColorTranslator.ToOle(Color.White))
                    Exit Sub

                Case Else
                    Exit Sub

            End Select

            Call _
                WriteChatOverHead(UserIndex, "¡¡¡Bien hecho " & TituloCaos(UserIndex) & ", aquí tienes tu recompensa!!!",
                                  CShort(Str(Npclist(.flags.TargetNPC).Char_Renamed.CharIndex)),
                                  ColorTranslator.ToOle(Color.White))

            ' Recompensas de armaduras y exp
            Call GiveFactionArmours(UserIndex, True)
            Call GiveExpReward(UserIndex, .Faccion.RecompensasCaos)

        End With
    End Sub

    Public Function TituloCaos(UserIndex As Short) As String
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 23/01/2007 Pablo (ToxicWaste)
        'Handles the titles of the members of the "Legión Oscura"
        '***************************************************
        'Rango 1: Acólito (70)
        'Rango 2: Alma Corrupta (160)
        'Rango 3: Paria (300)
        'Rango 4: Condenado (490)
        'Rango 5: Esbirro (740)
        'Rango 6: Sanguinario (1100)
        'Rango 7: Corruptor (1500 + lvl 27)
        'Rango 8: Heraldo Impio (2010)
        'Rango 9: Caballero de la Oscuridad (2700)
        'Rango 10: Señor del Miedo (4600 + lvl 30)
        'Rango 11: Ejecutor Infernal (5800 + lvl 31)
        'Rango 12: Protector del Averno (6990 + lvl 33)
        'Rango 13: Avatar de la Destrucción (8100 + lvl 35)
        'Rango 14: Guardián del Mal (9300 + lvl 36)
        'Rango 15: Campeón de la Oscuridad (11500 + lvl 37)

        Select Case UserList(UserIndex).Faccion.RecompensasCaos
            Case 0
                TituloCaos = "Acólito"
            Case 1
                TituloCaos = "Alma Corrupta"
            Case 2
                TituloCaos = "Paria"
            Case 3
                TituloCaos = "Condenado"
            Case 4
                TituloCaos = "Esbirro"
            Case 5
                TituloCaos = "Sanguinario"
            Case 6
                TituloCaos = "Corruptor"
            Case 7
                TituloCaos = "Heraldo Impío"
            Case 8
                TituloCaos = "Caballero de la Oscuridad"
            Case 9
                TituloCaos = "Señor del Miedo"
            Case 10
                TituloCaos = "Ejecutor Infernal"
            Case 11
                TituloCaos = "Protector del Averno"
            Case 12
                TituloCaos = "Avatar de la Destrucción"
            Case 13
                TituloCaos = "Guardián del Mal"
            Case Else
                TituloCaos = "Campeón de la Oscuridad"
        End Select
    End Function
End Module
