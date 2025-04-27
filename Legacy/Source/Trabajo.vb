Option Strict Off
Option Explicit On
Module Trabajo
    Private Const GASTO_ENERGIA_TRABAJADOR As Byte = 2
    Private Const GASTO_ENERGIA_NO_TRABAJADOR As Byte = 6


    Public Sub DoPermanecerOculto(UserIndex As Short)
        '********************************************************
        'Autor: Nacho (Integer)
        'Last Modif: 11/19/2009
        'Chequea si ya debe mostrarse
        'Pablo (ToxicWaste): Cambie los ordenes de prioridades porque sino no andaba.
        '11/19/2009: Pato - Ahora el bandido se oculta la mitad del tiempo de las demás clases.
        '13/01/2010: ZaMa - Now hidden on boat pirats recover the proper boat body.
        '13/01/2010: ZaMa - Arreglo condicional para que el bandido camine oculto.
        '********************************************************
        Try
            With UserList(UserIndex)
                .Counters.TiempoOculto = .Counters.TiempoOculto - 1
                If .Counters.TiempoOculto <= 0 Then

                    If .clase = eClass.Bandit Then
                        .Counters.TiempoOculto = Int(IntervaloOculto/2)
                    Else
                        .Counters.TiempoOculto = IntervaloOculto
                    End If

                    If .clase = eClass.Hunter And .Stats.UserSkills(eSkill.Ocultarse) > 90 Then
                        If .Invent.ArmourEqpObjIndex = 648 Or .Invent.ArmourEqpObjIndex = 360 Then
                            Exit Sub
                        End If
                    End If
                    .Counters.TiempoOculto = 0
                    .flags.Oculto = 0

                    If .flags.Navegando = 1 Then
                        If .clase = eClass.Pirat Then
                            ' Pierde la apariencia de fragata fantasmal
                            Call ToogleBoatBody(UserIndex)
                            Call _
                                WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!",
                                                FontTypeNames.FONTTYPE_INFO)
                            Call _
                                ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading,
                                               NingunArma, NingunEscudo, NingunCasco)
                        End If
                    Else
                        If .flags.invisible = 0 Then
                            Call _
                                WriteConsoleMsg(UserIndex, "Has vuelto a ser visible.", FontTypeNames.FONTTYPE_INFO)
                            Call SetInvisible(UserIndex, .Char_Renamed.CharIndex, False)
                        End If
                    End If
                End If
            End With


        Catch ex As Exception
            Console.WriteLine("Error in DoPermanecerOculto: " & ex.Message)
            Call LogError("Error en Sub DoPermanecerOculto")
        End Try
    End Sub

    Public Sub DoOcultarse(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 13/01/2010 (ZaMa)
        'Pablo (ToxicWaste): No olvidar agregar IntervaloOculto=500 al Server.ini.
        'Modifique la fórmula y ahora anda bien.
        '13/01/2010: ZaMa - El pirata se transforma en galeon fantasmal cuando se oculta en agua.
        '***************************************************

        Try

            Dim Suerte As Double
            Dim res As Short
            Dim Skill As Short

            With UserList(UserIndex)
                Skill = .Stats.UserSkills(eSkill.Ocultarse)

                Suerte = (((0.000002*Skill - 0.0002)*Skill + 0.0064)*Skill + 0.1124)*100

                res = RandomNumber(1, 100)

                If res <= Suerte Then

                    .flags.Oculto = 1
                    Suerte = (- 0.000001*(100 - Skill)^3)
                    Suerte = Suerte + (0.00009229*(100 - Skill)^2)
                    Suerte = Suerte + (- 0.0088*(100 - Skill))
                    Suerte = Suerte + (0.9571)
                    Suerte = Suerte*IntervaloOculto
                    .Counters.TiempoOculto = Suerte

                    ' No es pirata o es uno sin barca
                    If .flags.Navegando = 0 Then
                        Call SetInvisible(UserIndex, .Char_Renamed.CharIndex, True)

                        Call _
                            WriteConsoleMsg(UserIndex, "¡Te has escondido entre las sombras!",
                                            FontTypeNames.FONTTYPE_INFO)
                        ' Es un pirata navegando
                    Else
                        ' Le cambiamos el body a galeon fantasmal
                        .Char_Renamed.body = iFragataFantasmal
                        ' Actualizamos clientes
                        Call _
                            ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading,
                                           NingunArma, NingunEscudo, NingunCasco)
                    End If

                    Call SubirSkill(UserIndex, eSkill.Ocultarse, True)
                Else
                    '[CDT 17-02-2004]
                    If Not .flags.UltimoMensaje = 4 Then
                        Call WriteConsoleMsg(UserIndex, "¡No has logrado esconderte!", FontTypeNames.FONTTYPE_INFO)
                        .flags.UltimoMensaje = 4
                    End If
                    '[/CDT]

                    Call SubirSkill(UserIndex, eSkill.Ocultarse, False)
                End If

                .Counters.Ocultando = .Counters.Ocultando + 1
            End With


        Catch ex As Exception
            Console.WriteLine("Error in DoOcultarse: " & ex.Message)
            Call LogError("Error en Sub DoOcultarse")
        End Try
    End Sub

    Public Sub DoNavega(UserIndex As Short, ByRef Barco As ObjData, Slot As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 13/01/2010 (ZaMa)
        '13/01/2010: ZaMa - El pirata pierde el ocultar si desequipa barca.
        '***************************************************

        Dim ModNave As Single

        With UserList(UserIndex)
            ModNave = ModNavegacion(.clase, UserIndex)

            If .Stats.UserSkills(eSkill.Navegacion)/ModNave < Barco.MinSkill Then
                Call _
                    WriteConsoleMsg(UserIndex, "No tienes suficientes conocimientos para usar este barco.",
                                    FontTypeNames.FONTTYPE_INFO)
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Para usar este barco necesitas " & Barco.MinSkill*ModNave &
                                    " puntos en navegacion.", FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            .Invent.BarcoObjIndex = .Invent.Object_Renamed(Slot).ObjIndex
            .Invent.BarcoSlot = Slot

            ' No estaba navegando
            If .flags.Navegando = 0 Then

                .Char_Renamed.Head = 0

                ' No esta muerto
                If .flags.Muerto = 0 Then

                    Call ToogleBoatBody(UserIndex)

                    If .clase = eClass.Pirat Then
                        If .flags.Oculto = 1 Then
                            .flags.Oculto = 0
                            Call SetInvisible(UserIndex, .Char_Renamed.CharIndex, False)
                            Call _
                                WriteConsoleMsg(UserIndex, "¡Has vuelto a ser visible!",
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If

                    ' Esta muerto
                Else
                    .Char_Renamed.body = iFragataFantasmal
                    .Char_Renamed.ShieldAnim = NingunEscudo
                    .Char_Renamed.WeaponAnim = NingunArma
                    .Char_Renamed.CascoAnim = NingunCasco
                End If

                ' Comienza a navegar
                .flags.Navegando = 1

                ' Estaba navegando
            Else
                ' No esta muerto
                If .flags.Muerto = 0 Then
                    .Char_Renamed.Head = .OrigChar.Head

                    If .clase = eClass.Pirat Then
                        If .flags.Oculto = 1 Then
                            ' Al desequipar barca, perdió el ocultar
                            .flags.Oculto = 0
                            .Counters.Ocultando = 0
                            Call _
                                WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!",
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If

                    If .Invent.ArmourEqpObjIndex > 0 Then
                        .Char_Renamed.body = ObjData_Renamed(.Invent.ArmourEqpObjIndex).Ropaje
                    Else
                        Call DarCuerpoDesnudo(UserIndex)
                    End If

                    If .Invent.EscudoEqpObjIndex > 0 Then _
                        .Char_Renamed.ShieldAnim = ObjData_Renamed(.Invent.EscudoEqpObjIndex).ShieldAnim
                    If .Invent.WeaponEqpObjIndex > 0 Then _
                        .Char_Renamed.WeaponAnim = GetWeaponAnim(UserIndex, .Invent.WeaponEqpObjIndex)
                    If .Invent.CascoEqpObjIndex > 0 Then _
                        .Char_Renamed.CascoAnim = ObjData_Renamed(.Invent.CascoEqpObjIndex).CascoAnim

                    ' Esta muerto
                Else
                    .Char_Renamed.body = iCuerpoMuerto
                    .Char_Renamed.Head = iCabezaMuerto
                    .Char_Renamed.ShieldAnim = NingunEscudo
                    .Char_Renamed.WeaponAnim = NingunArma
                    .Char_Renamed.CascoAnim = NingunCasco
                End If

                ' Termina de navegar
                .flags.Navegando = 0
            End If

            ' Actualizo clientes
            Call _
                ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading,
                               .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim, .Char_Renamed.CascoAnim)
        End With

        Call WriteNavigateToggle(UserIndex)
    End Sub

    Public Sub FundirMineral(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

            With UserList(UserIndex)
                If .flags.TargetObjInvIndex > 0 Then

                    If _
                        ObjData_Renamed(.flags.TargetObjInvIndex).OBJType = eOBJType.otMinerales And
                        ObjData_Renamed(.flags.TargetObjInvIndex).MinSkill <=
                        .Stats.UserSkills(eSkill.Mineria)/ModFundicion(.clase) Then
                        Call DoLingotes(UserIndex)
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "No tienes conocimientos de minería suficientes para trabajar este mineral.",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If

                End If
            End With


        Catch ex As Exception
            Console.WriteLine("Error in DoNavega: " & ex.Message)
            Call LogError("Error en FundirMineral. Error " & Err.Number & " : " & Err.Description)
        End Try
    End Sub

    Public Sub FundirArmas(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try
            With UserList(UserIndex)
                If .flags.TargetObjInvIndex > 0 Then
                    If ObjData_Renamed(.flags.TargetObjInvIndex).OBJType = eOBJType.otWeapon Then
                        If _
                            ObjData_Renamed(.flags.TargetObjInvIndex).SkHerreria <=
                            .Stats.UserSkills(eSkill.Herreria)/ModHerreriA(.clase) Then
                            Call DoFundir(UserIndex)
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex,
                                                "No tienes los conocimientos suficientes en herrería para fundir este objeto.",
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If
                End If
            End With


        Catch ex As Exception
            Console.WriteLine("Error in FundirArmas: " & ex.Message)
            Call LogError("Error en FundirArmas. Error " & Err.Number & " : " & Err.Description)
        End Try
    End Sub

    Function TieneObjetos(ItemIndex As Short, cant As Short, UserIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim i As Short
        Dim Total As Integer
        For i = 1 To UserList(UserIndex).CurrentInventorySlots
            If UserList(UserIndex).Invent.Object_Renamed(i).ObjIndex = ItemIndex Then
                Total = Total + UserList(UserIndex).Invent.Object_Renamed(i).Amount
            End If
        Next i

        If cant <= Total Then
            TieneObjetos = True
            Exit Function
        End If
    End Function

    Public Sub QuitarObjetos(ItemIndex As Short, cant As Short, UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 05/08/09
        '05/08/09: Pato - Cambie la funcion a procedimiento ya que se usa como procedimiento siempre, y fixie el bug 2788199
        '***************************************************

        Dim i As Short
        For i = 1 To UserList(UserIndex).CurrentInventorySlots
            With UserList(UserIndex).Invent.Object_Renamed(i)
                If .ObjIndex = ItemIndex Then
                    If .Amount <= cant And .Equipped = 1 Then Call Desequipar(UserIndex, i)

                    .Amount = .Amount - cant
                    If .Amount <= 0 Then
                        cant = Math.Abs(.Amount)
                        UserList(UserIndex).Invent.NroItems = UserList(UserIndex).Invent.NroItems - 1
                        .Amount = 0
                        .ObjIndex = 0
                    Else
                        cant = 0
                    End If

                    Call UpdateUserInv(False, UserIndex, i)

                    If cant = 0 Then Exit Sub
                End If
            End With
        Next i
    End Sub

    Sub HerreroQuitarMateriales(UserIndex As Short, ItemIndex As Short, CantidadItems As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 16/11/2009
        '16/11/2009: ZaMa - Ahora considera la cantidad de items a construir
        '***************************************************
        With ObjData_Renamed(ItemIndex)
            If .LingH > 0 Then Call QuitarObjetos(LingoteHierro, .LingH*CantidadItems, UserIndex)
            If .LingP > 0 Then Call QuitarObjetos(LingotePlata, .LingP*CantidadItems, UserIndex)
            If .LingO > 0 Then Call QuitarObjetos(LingoteOro, .LingO*CantidadItems, UserIndex)
        End With
    End Sub

    Sub CarpinteroQuitarMateriales(UserIndex As Short, ItemIndex As Short, CantidadItems As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 16/11/2009
        '16/11/2009: ZaMa - Ahora quita tambien madera elfica
        '***************************************************
        With ObjData_Renamed(ItemIndex)
            If .Madera > 0 Then Call QuitarObjetos(Leña, .Madera*CantidadItems, UserIndex)
            If .MaderaElfica > 0 Then Call QuitarObjetos(LeñaElfica, .MaderaElfica*CantidadItems, UserIndex)
        End With
    End Sub

    Function CarpinteroTieneMateriales(UserIndex As Short, ItemIndex As Short, Cantidad As Short,
                                       Optional ByVal ShowMsg As Boolean = False) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: 16/11/2009
        '16/11/2009: ZaMa - Agregada validacion a madera elfica.
        '16/11/2009: ZaMa - Ahora considera la cantidad de items a construir
        '***************************************************

        With ObjData_Renamed(ItemIndex)
            If .Madera > 0 Then
                If Not TieneObjetos(Leña, .Madera*Cantidad, UserIndex) Then
                    If ShowMsg Then _
                        Call _
                            WriteConsoleMsg(UserIndex, "No tienes suficiente madera.",
                                            FontTypeNames.FONTTYPE_INFO)
                    CarpinteroTieneMateriales = False
                    Exit Function
                End If
            End If

            If .MaderaElfica > 0 Then
                If Not TieneObjetos(LeñaElfica, .MaderaElfica*Cantidad, UserIndex) Then
                    If ShowMsg Then _
                        Call _
                            WriteConsoleMsg(UserIndex, "No tienes suficiente madera élfica.",
                                            FontTypeNames.FONTTYPE_INFO)
                    CarpinteroTieneMateriales = False
                    Exit Function
                End If
            End If

        End With
        CarpinteroTieneMateriales = True
    End Function

    Function HerreroTieneMateriales(UserIndex As Short, ItemIndex As Short, CantidadItems As Short) _
        As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: 16/11/2009
        '16/11/2009: ZaMa - Agregada validacion a madera elfica.
        '***************************************************
        With ObjData_Renamed(ItemIndex)
            If .LingH > 0 Then
                If Not TieneObjetos(LingoteHierro, .LingH*CantidadItems, UserIndex) Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de hierro.",
                                        FontTypeNames.FONTTYPE_INFO)
                    HerreroTieneMateriales = False
                    Exit Function
                End If
            End If
            If .LingP > 0 Then
                If Not TieneObjetos(LingotePlata, .LingP*CantidadItems, UserIndex) Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de plata.",
                                        FontTypeNames.FONTTYPE_INFO)
                    HerreroTieneMateriales = False
                    Exit Function
                End If
            End If
            If .LingO > 0 Then
                If Not TieneObjetos(LingoteOro, .LingO*CantidadItems, UserIndex) Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de oro.",
                                        FontTypeNames.FONTTYPE_INFO)
                    HerreroTieneMateriales = False
                    Exit Function
                End If
            End If
        End With
        HerreroTieneMateriales = True
    End Function

    Function TieneMaterialesUpgrade(UserIndex As Short, ItemIndex As Short) As Boolean
        '***************************************************
        'Author: Torres Patricio (Pato)
        'Last Modification: 12/08/2009
        '
        '***************************************************
        Dim ItemUpgrade As Short

        ItemUpgrade = ObjData_Renamed(ItemIndex).Upgrade

        With ObjData_Renamed(ItemUpgrade)
            If .LingH > 0 Then
                If _
                    Not _
                    TieneObjetos(LingoteHierro,
                                 CShort(.LingH - ObjData_Renamed(ItemIndex).LingH*PORCENTAJE_MATERIALES_UPGRADE),
                                 UserIndex) Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de hierro.",
                                        FontTypeNames.FONTTYPE_INFO)
                    TieneMaterialesUpgrade = False
                    Exit Function
                End If
            End If

            If .LingP > 0 Then
                If _
                    Not _
                    TieneObjetos(LingotePlata,
                                 CShort(.LingP - ObjData_Renamed(ItemIndex).LingP*PORCENTAJE_MATERIALES_UPGRADE),
                                 UserIndex) Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de plata.",
                                        FontTypeNames.FONTTYPE_INFO)
                    TieneMaterialesUpgrade = False
                    Exit Function
                End If
            End If

            If .LingO > 0 Then
                If _
                    Not _
                    TieneObjetos(LingoteOro,
                                 CShort(.LingO - ObjData_Renamed(ItemIndex).LingO*PORCENTAJE_MATERIALES_UPGRADE),
                                 UserIndex) Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de oro.",
                                        FontTypeNames.FONTTYPE_INFO)
                    TieneMaterialesUpgrade = False
                    Exit Function
                End If
            End If

            If .Madera > 0 Then
                If _
                    Not _
                    TieneObjetos(Leña, CShort(.Madera - ObjData_Renamed(ItemIndex).Madera*PORCENTAJE_MATERIALES_UPGRADE),
                                 UserIndex) Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No tienes suficiente madera.", FontTypeNames.FONTTYPE_INFO)
                    TieneMaterialesUpgrade = False
                    Exit Function
                End If
            End If

            If .MaderaElfica > 0 Then
                If _
                    Not _
                    TieneObjetos(LeñaElfica,
                                 CShort(
                                     .MaderaElfica -
                                     ObjData_Renamed(ItemIndex).MaderaElfica*PORCENTAJE_MATERIALES_UPGRADE), UserIndex) _
                    Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No tienes suficiente madera élfica.",
                                        FontTypeNames.FONTTYPE_INFO)
                    TieneMaterialesUpgrade = False
                    Exit Function
                End If
            End If
        End With

        TieneMaterialesUpgrade = True
    End Function

    Sub QuitarMaterialesUpgrade(UserIndex As Short, ItemIndex As Short)
        '***************************************************
        'Author: Torres Patricio (Pato)
        'Last Modification: 12/08/2009
        '
        '***************************************************
        Dim ItemUpgrade As Short

        ItemUpgrade = ObjData_Renamed(ItemIndex).Upgrade

        With ObjData_Renamed(ItemUpgrade)
            If .LingH > 0 Then _
                Call _
                    QuitarObjetos(LingoteHierro,
                                  CShort(.LingH - ObjData_Renamed(ItemIndex).LingH*PORCENTAJE_MATERIALES_UPGRADE),
                                  UserIndex)
            If .LingP > 0 Then _
                Call _
                    QuitarObjetos(LingotePlata,
                                  CShort(.LingP - ObjData_Renamed(ItemIndex).LingP*PORCENTAJE_MATERIALES_UPGRADE),
                                  UserIndex)
            If .LingO > 0 Then _
                Call _
                    QuitarObjetos(LingoteOro,
                                  CShort(.LingO - ObjData_Renamed(ItemIndex).LingO*PORCENTAJE_MATERIALES_UPGRADE),
                                  UserIndex)
            If .Madera > 0 Then _
                Call _
                    QuitarObjetos(Leña,
                                  CShort(.Madera - ObjData_Renamed(ItemIndex).Madera*PORCENTAJE_MATERIALES_UPGRADE),
                                  UserIndex)
            If .MaderaElfica > 0 Then _
                Call _
                    QuitarObjetos(LeñaElfica,
                                  CShort(
                                      .MaderaElfica -
                                      ObjData_Renamed(ItemIndex).MaderaElfica*PORCENTAJE_MATERIALES_UPGRADE), UserIndex)
        End With

        Call QuitarObjetos(ItemIndex, 1, UserIndex)
    End Sub

    Public Function PuedeConstruir(UserIndex As Short, ItemIndex As Short, CantidadItems As Short) _
        As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: 24/08/2009
        '24/08/2008: ZaMa - Validates if the player has the required skill
        '16/11/2009: ZaMa - Validates if the player has the required amount of materials, depending on the number of items to make
        '***************************************************
        PuedeConstruir = HerreroTieneMateriales(UserIndex, ItemIndex, CantidadItems) And
                         Math.Round(
                             UserList(UserIndex).Stats.UserSkills(eSkill.Herreria)/
                             ModHerreriA(UserList(UserIndex).clase), 0) >= ObjData_Renamed(ItemIndex).SkHerreria
    End Function

    Public Function PuedeConstruirHerreria(ItemIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************
        Dim i As Integer

        For i = 1 To UBound(ArmasHerrero)
            If ArmasHerrero(i) = ItemIndex Then
                PuedeConstruirHerreria = True
                Exit Function
            End If
        Next i
        For i = 1 To UBound(ArmadurasHerrero)
            If ArmadurasHerrero(i) = ItemIndex Then
                PuedeConstruirHerreria = True
                Exit Function
            End If
        Next i
        PuedeConstruirHerreria = False
    End Function

    Public Sub HerreroConstruirItem(UserIndex As Short, ItemIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 16/11/2009
        '16/11/2009: ZaMa - Implementado nuevo sistema de construccion de items
        '***************************************************
        Dim CantidadItems As Short
        Dim TieneMateriales As Boolean

        Dim MiObj As Obj
        With UserList(UserIndex)
            CantidadItems = .Construir.PorCiclo

            If .Construir.Cantidad < CantidadItems Then CantidadItems = .Construir.Cantidad

            If .Construir.Cantidad > 0 Then .Construir.Cantidad = .Construir.Cantidad - CantidadItems

            If CantidadItems = 0 Then
                Call WriteStopWorking(UserIndex)
                Exit Sub
            End If

            If PuedeConstruirHerreria(ItemIndex) Then

                While CantidadItems > 0 And Not TieneMateriales
                    If PuedeConstruir(UserIndex, ItemIndex, CantidadItems) Then
                        TieneMateriales = True
                    Else
                        CantidadItems = CantidadItems - 1
                    End If
                End While

                ' Chequeo si puede hacer al menos 1 item
                If Not TieneMateriales Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No tienes suficientes materiales.",
                                        FontTypeNames.FONTTYPE_INFO)
                    Call WriteStopWorking(UserIndex)
                    Exit Sub
                End If

                'Sacamos energía
                If .clase = eClass.Worker Then
                    'Chequeamos que tenga los puntos antes de sacarselos
                    If .Stats.MinSta >= GASTO_ENERGIA_TRABAJADOR Then
                        .Stats.MinSta = .Stats.MinSta - GASTO_ENERGIA_TRABAJADOR
                        Call WriteUpdateSta(UserIndex)
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, "No tienes suficiente energía.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If
                Else
                    'Chequeamos que tenga los puntos antes de sacarselos
                    If .Stats.MinSta >= GASTO_ENERGIA_NO_TRABAJADOR Then
                        .Stats.MinSta = .Stats.MinSta - GASTO_ENERGIA_NO_TRABAJADOR
                        Call WriteUpdateSta(UserIndex)
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, "No tienes suficiente energía.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If
                End If

                Call HerreroQuitarMateriales(UserIndex, ItemIndex, CantidadItems)
                ' AGREGAR FX
                If ObjData_Renamed(ItemIndex).OBJType = eOBJType.otWeapon Then
                    Call _
                        WriteConsoleMsg(UserIndex,
                                        "Has construido " &
                                        IIf(CantidadItems > 1, CantidadItems & " armas!", "el arma!"),
                                        FontTypeNames.FONTTYPE_INFO)
                ElseIf ObjData_Renamed(ItemIndex).OBJType = eOBJType.otESCUDO Then
                    Call _
                        WriteConsoleMsg(UserIndex,
                                        "Has construido " &
                                        IIf(CantidadItems > 1, CantidadItems & " escudos!", "el escudo!"),
                                        FontTypeNames.FONTTYPE_INFO)
                ElseIf ObjData_Renamed(ItemIndex).OBJType = eOBJType.otCASCO Then
                    Call _
                        WriteConsoleMsg(UserIndex,
                                        "Has construido " &
                                        IIf(CantidadItems > 1, CantidadItems & " cascos!", "el casco!"),
                                        FontTypeNames.FONTTYPE_INFO)
                ElseIf ObjData_Renamed(ItemIndex).OBJType = eOBJType.otArmadura Then
                    Call _
                        WriteConsoleMsg(UserIndex,
                                        "Has construido " &
                                        IIf(CantidadItems > 1, CantidadItems & " armaduras", "la armadura!"),
                                        FontTypeNames.FONTTYPE_INFO)
                End If


                MiObj.Amount = CantidadItems
                MiObj.ObjIndex = ItemIndex
                If Not MeterItemEnInventario(UserIndex, MiObj) Then
                    Call TirarItemAlPiso(.Pos, MiObj)
                End If

                'Log de construcción de Items. Pablo (ToxicWaste) 10/09/07
                If ObjData_Renamed(MiObj.ObjIndex).Log = 1 Then
                    Call _
                        LogDesarrollo(
                            .name & " ha construído " & MiObj.Amount & " " & ObjData_Renamed(MiObj.ObjIndex).name)
                End If

                Call SubirSkill(UserIndex, eSkill.Herreria, True)
                Call UpdateUserInv(True, UserIndex, 0)
                Call _
                    SendData(SendTarget.ToPCArea, UserIndex,
                             PrepareMessagePlayWave(MARTILLOHERRERO, .Pos.X, .Pos.Y))

                .Reputacion.PlebeRep = .Reputacion.PlebeRep + vlProleta
                If .Reputacion.PlebeRep > MAXREP Then .Reputacion.PlebeRep = MAXREP

                .Counters.Trabajando = .Counters.Trabajando + 1
            End If
        End With
    End Sub

    Public Function PuedeConstruirCarpintero(ItemIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************
        Dim i As Integer

        For i = 1 To UBound(ObjCarpintero)
            If ObjCarpintero(i) = ItemIndex Then
                PuedeConstruirCarpintero = True
                Exit Function
            End If
        Next i
        PuedeConstruirCarpintero = False
    End Function

    Public Sub CarpinteroConstruirItem(UserIndex As Short, ItemIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 16/11/2009
        '24/08/2008: ZaMa - Validates if the player has the required skill
        '16/11/2009: ZaMa - Implementado nuevo sistema de construccion de items
        '***************************************************
        Dim CantidadItems As Short
        Dim TieneMateriales As Boolean

        Dim MiObj As Obj
        With UserList(UserIndex)
            CantidadItems = .Construir.PorCiclo

            If .Construir.Cantidad < CantidadItems Then CantidadItems = .Construir.Cantidad

            If .Construir.Cantidad > 0 Then .Construir.Cantidad = .Construir.Cantidad - CantidadItems

            If CantidadItems = 0 Then
                Call WriteStopWorking(UserIndex)
                Exit Sub
            End If

            If _
                Math.Round(.Stats.UserSkills(eSkill.Carpinteria)\ModCarpinteria(.clase), 0) >=
                ObjData_Renamed(ItemIndex).SkCarpinteria And PuedeConstruirCarpintero(ItemIndex) And
                .Invent.WeaponEqpObjIndex = SERRUCHO_CARPINTERO Then

                ' Calculo cuantos item puede construir
                While CantidadItems > 0 And Not TieneMateriales
                    If CarpinteroTieneMateriales(UserIndex, ItemIndex, CantidadItems) Then
                        TieneMateriales = True
                    Else
                        CantidadItems = CantidadItems - 1
                    End If
                End While

                ' No tiene los materiales ni para construir 1 item?
                If Not TieneMateriales Then
                    ' Para que muestre el mensaje
                    Call CarpinteroTieneMateriales(UserIndex, ItemIndex, 1, True)
                    Call WriteStopWorking(UserIndex)
                    Exit Sub
                End If

                'Sacamos energía
                If .clase = eClass.Worker Then
                    'Chequeamos que tenga los puntos antes de sacarselos
                    If .Stats.MinSta >= GASTO_ENERGIA_TRABAJADOR Then
                        .Stats.MinSta = .Stats.MinSta - GASTO_ENERGIA_TRABAJADOR
                        Call WriteUpdateSta(UserIndex)
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, "No tienes suficiente energía.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If
                Else
                    'Chequeamos que tenga los puntos antes de sacarselos
                    If .Stats.MinSta >= GASTO_ENERGIA_NO_TRABAJADOR Then
                        .Stats.MinSta = .Stats.MinSta - GASTO_ENERGIA_NO_TRABAJADOR
                        Call WriteUpdateSta(UserIndex)
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, "No tienes suficiente energía.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If
                End If

                Call CarpinteroQuitarMateriales(UserIndex, ItemIndex, CantidadItems)
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Has construido " & CantidadItems & IIf(CantidadItems = 1, " objeto!", " objetos!"),
                                    FontTypeNames.FONTTYPE_INFO)

                MiObj.Amount = CantidadItems
                MiObj.ObjIndex = ItemIndex
                If Not MeterItemEnInventario(UserIndex, MiObj) Then
                    Call TirarItemAlPiso(.Pos, MiObj)
                End If

                'Log de construcción de Items. Pablo (ToxicWaste) 10/09/07
                If ObjData_Renamed(MiObj.ObjIndex).Log = 1 Then
                    Call _
                        LogDesarrollo(
                            .name & " ha construído " & MiObj.Amount & " " & ObjData_Renamed(MiObj.ObjIndex).name)
                End If

                Call SubirSkill(UserIndex, eSkill.Carpinteria, True)
                Call UpdateUserInv(True, UserIndex, 0)
                Call _
                    SendData(SendTarget.ToPCArea, UserIndex,
                             PrepareMessagePlayWave(LABUROCARPINTERO, .Pos.X, .Pos.Y))


                .Reputacion.PlebeRep = .Reputacion.PlebeRep + vlProleta
                If .Reputacion.PlebeRep > MAXREP Then .Reputacion.PlebeRep = MAXREP

                .Counters.Trabajando = .Counters.Trabajando + 1

            ElseIf .Invent.WeaponEqpObjIndex <> SERRUCHO_CARPINTERO Then
                Call _
                    WriteConsoleMsg(UserIndex, "Debes tener equipado el serrucho para trabajar.",
                                    FontTypeNames.FONTTYPE_INFO)
            End If
        End With
    End Sub

    Private Function MineralesParaLingote(Lingote As iMinerales) As Short
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************
        Select Case Lingote
            Case iMinerales.HierroCrudo
                MineralesParaLingote = 14
            Case iMinerales.PlataCruda
                MineralesParaLingote = 20
            Case iMinerales.OroCrudo
                MineralesParaLingote = 35
            Case Else
                MineralesParaLingote = 10000
        End Select
    End Function


    Public Sub DoLingotes(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 16/11/2009
        '16/11/2009: ZaMa - Implementado nuevo sistema de construccion de items
        '***************************************************
        '    Call LogTarea("Sub DoLingotes")
        Dim Slot As Short
        Dim obji As Short
        Dim CantidadItems As Short
        Dim TieneMinerales As Boolean

        Dim MiObj As Obj
        With UserList(UserIndex)
            CantidadItems = MaximoInt(1, CShort((.Stats.ELV - 4)/5))

            Slot = .flags.TargetObjInvSlot
            obji = .Invent.Object_Renamed(Slot).ObjIndex

            While CantidadItems > 0 And Not TieneMinerales
                If .Invent.Object_Renamed(Slot).Amount >= MineralesParaLingote(obji)*CantidadItems Then
                    TieneMinerales = True
                Else
                    CantidadItems = CantidadItems - 1
                End If
            End While

            If Not TieneMinerales Or ObjData_Renamed(obji).OBJType <> eOBJType.otMinerales Then
                Call _
                    WriteConsoleMsg(UserIndex, "No tienes suficientes minerales para hacer un lingote.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            .Invent.Object_Renamed(Slot).Amount = .Invent.Object_Renamed(Slot).Amount -
                                                  MineralesParaLingote(obji)*CantidadItems
            If .Invent.Object_Renamed(Slot).Amount < 1 Then
                .Invent.Object_Renamed(Slot).Amount = 0
                .Invent.Object_Renamed(Slot).ObjIndex = 0
            End If

            MiObj.Amount = CantidadItems
            MiObj.ObjIndex = ObjData_Renamed(.flags.TargetObjInvIndex).LingoteIndex
            If Not MeterItemEnInventario(UserIndex, MiObj) Then
                Call TirarItemAlPiso(.Pos, MiObj)
            End If
            Call UpdateUserInv(False, UserIndex, Slot)
            Call _
                WriteConsoleMsg(UserIndex,
                                "¡Has obtenido " & CantidadItems & " lingote" & IIf(CantidadItems = 1, "", "s") & "!",
                                FontTypeNames.FONTTYPE_INFO)

            .Counters.Trabajando = .Counters.Trabajando + 1
        End With
    End Sub

    Public Sub DoFundir(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 03/06/2010
        '03/06/2010 - Pato: Si es el último ítem a fundir y está equipado lo desequipamos.
        '11/03/2010 - ZaMa: Reemplazo división por producto para uan mejor performanse.
        '***************************************************
        Dim i As Short
        Dim num As Short
        Dim Slot As Byte
        Dim Lingotes(2) As Short

        Dim MiObj(2) As Obj
        With UserList(UserIndex)
            Slot = .flags.TargetObjInvSlot

            With .Invent.Object_Renamed(Slot)
                .Amount = .Amount - 1

                If .Amount < 1 Then
                    If .Equipped = 1 Then Call Desequipar(UserIndex, Slot)

                    .Amount = 0
                    .ObjIndex = 0
                End If
            End With

            num = RandomNumber(10, 25)

            Lingotes(0) = (ObjData_Renamed(.flags.TargetObjInvIndex).LingH*num)*0.01
            Lingotes(1) = (ObjData_Renamed(.flags.TargetObjInvIndex).LingP*num)*0.01
            Lingotes(2) = (ObjData_Renamed(.flags.TargetObjInvIndex).LingO*num)*0.01


            For i = 0 To 2
                MiObj(i).Amount = Lingotes(i)
                MiObj(i).ObjIndex = LingoteHierro + i 'Una gran negrada pero práctica
                If MiObj(i).Amount > 0 Then
                    If Not MeterItemEnInventario(UserIndex, MiObj(i)) Then
                        Call TirarItemAlPiso(.Pos, MiObj(i))
                    End If
                    Call UpdateUserInv(True, UserIndex, Slot)
                End If
            Next i

            Call _
                WriteConsoleMsg(UserIndex,
                                "¡Has obtenido el " & num &
                                "% de los lingotes utilizados para la construcción del objeto!",
                                FontTypeNames.FONTTYPE_INFO)

            .Counters.Trabajando = .Counters.Trabajando + 1

        End With
    End Sub

    Public Sub DoUpgrade(UserIndex As Short, ItemIndex As Short)
        '***************************************************
        'Author: Torres Patricio (Pato)
        'Last Modification: 12/08/2009
        '12/08/2009: Pato - Implementado nuevo sistema de mejora de items
        '***************************************************
        Dim ItemUpgrade As Short

        ItemUpgrade = ObjData_Renamed(ItemIndex).Upgrade

        Dim MiObj As Obj
        With UserList(UserIndex)
            'Sacamos energía
            If .clase = eClass.Worker Then
                'Chequeamos que tenga los puntos antes de sacarselos
                If .Stats.MinSta >= GASTO_ENERGIA_TRABAJADOR Then
                    .Stats.MinSta = .Stats.MinSta - GASTO_ENERGIA_TRABAJADOR
                    Call WriteUpdateSta(UserIndex)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "No tienes suficiente energía.", FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If
            Else
                'Chequeamos que tenga los puntos antes de sacarselos
                If .Stats.MinSta >= GASTO_ENERGIA_NO_TRABAJADOR Then
                    .Stats.MinSta = .Stats.MinSta - GASTO_ENERGIA_NO_TRABAJADOR
                    Call WriteUpdateSta(UserIndex)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "No tienes suficiente energía.", FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If
            End If

            If ItemUpgrade <= 0 Then Exit Sub
            If Not TieneMaterialesUpgrade(UserIndex, ItemIndex) Then Exit Sub

            If PuedeConstruirHerreria(ItemUpgrade) Then
                If .Invent.WeaponEqpObjIndex <> MARTILLO_HERRERO Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Debes equiparte el martillo de herrero.",
                                        FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If
                If _
                    Math.Round(.Stats.UserSkills(eSkill.Herreria)/ModHerreriA(.clase), 0) <
                    ObjData_Renamed(ItemUpgrade).SkHerreria Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No tienes suficientes skills.", FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If

                Select Case ObjData_Renamed(ItemIndex).OBJType
                    Case eOBJType.otWeapon
                        Call WriteConsoleMsg(UserIndex, "Has mejorado el arma!", FontTypeNames.FONTTYPE_INFO)

                    Case eOBJType.otESCUDO 'Todavía no hay, pero just in case
                        Call WriteConsoleMsg(UserIndex, "Has mejorado el escudo!", FontTypeNames.FONTTYPE_INFO)

                    Case eOBJType.otCASCO
                        Call WriteConsoleMsg(UserIndex, "Has mejorado el casco!", FontTypeNames.FONTTYPE_INFO)

                    Case eOBJType.otArmadura
                        Call _
                            WriteConsoleMsg(UserIndex, "Has mejorado la armadura!", FontTypeNames.FONTTYPE_INFO)
                End Select

                Call SubirSkill(UserIndex, eSkill.Herreria, True)
                Call _
                    SendData(SendTarget.ToPCArea, UserIndex,
                             PrepareMessagePlayWave(MARTILLOHERRERO, .Pos.X, .Pos.Y))

            ElseIf PuedeConstruirCarpintero(ItemUpgrade) Then
                If .Invent.WeaponEqpObjIndex <> SERRUCHO_CARPINTERO Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Debes equiparte el serrucho.", FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If
                If _
                    Math.Round(.Stats.UserSkills(eSkill.Carpinteria)\ModCarpinteria(.clase), 0) <
                    ObjData_Renamed(ItemUpgrade).SkCarpinteria Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No tienes suficientes skills.", FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If

                Select Case ObjData_Renamed(ItemIndex).OBJType
                    Case eOBJType.otFlechas
                        Call WriteConsoleMsg(UserIndex, "Has mejorado la flecha!", FontTypeNames.FONTTYPE_INFO)

                    Case eOBJType.otWeapon
                        Call WriteConsoleMsg(UserIndex, "Has mejorado el arma!", FontTypeNames.FONTTYPE_INFO)

                    Case eOBJType.otBarcos
                        Call WriteConsoleMsg(UserIndex, "Has mejorado el barco!", FontTypeNames.FONTTYPE_INFO)
                End Select

                Call SubirSkill(UserIndex, eSkill.Carpinteria, True)
                Call _
                    SendData(SendTarget.ToPCArea, UserIndex,
                             PrepareMessagePlayWave(LABUROCARPINTERO, .Pos.X, .Pos.Y))
            Else
                Exit Sub
            End If

            Call QuitarMaterialesUpgrade(UserIndex, ItemIndex)

            MiObj.Amount = 1
            MiObj.ObjIndex = ItemUpgrade

            If Not MeterItemEnInventario(UserIndex, MiObj) Then
                Call TirarItemAlPiso(.Pos, MiObj)
            End If

            If ObjData_Renamed(ItemIndex).Log = 1 Then _
                Call _
                    LogDesarrollo(
                        .name & " ha mejorado el ítem " & ObjData_Renamed(ItemIndex).name & " a " &
                        ObjData_Renamed(ItemUpgrade).name)

            Call UpdateUserInv(True, UserIndex, 0)

            .Reputacion.PlebeRep = .Reputacion.PlebeRep + vlProleta
            If .Reputacion.PlebeRep > MAXREP Then .Reputacion.PlebeRep = MAXREP

            .Counters.Trabajando = .Counters.Trabajando + 1
        End With
    End Sub

    Function ModNavegacion(clase As eClass, UserIndex As Short) As Single
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 27/11/2009
        '27/11/2009: ZaMa - A worker can navigate before only if it's an expert fisher
        '12/04/2010: ZaMa - Arreglo modificador de pescador, para que navegue con 60 skills.
        '***************************************************
        Select Case clase
            Case eClass.Pirat
                ModNavegacion = 1
            Case eClass.Worker
                If UserList(UserIndex).Stats.UserSkills(eSkill.Pesca) = 100 Then
                    ModNavegacion = 1.71
                Else
                    ModNavegacion = 2
                End If
            Case Else
                ModNavegacion = 2
        End Select
    End Function


    Function ModFundicion(clase As eClass) As Single
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Select Case clase
            Case eClass.Worker
                ModFundicion = 1
            Case Else
                ModFundicion = 3
        End Select
    End Function

    Function ModCarpinteria(clase As eClass) As Short
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Select Case clase
            Case eClass.Worker
                ModCarpinteria = 1
            Case Else
                ModCarpinteria = 3
        End Select
    End Function

    Function ModHerreriA(clase As eClass) As Single
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************
        Select Case clase
            Case eClass.Worker
                ModHerreriA = 1
            Case Else
                ModHerreriA = 4
        End Select
    End Function

    Function ModDomar(clase As eClass) As Short
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************
        Select Case clase
            Case eClass.Druid
                ModDomar = 6
            Case eClass.Hunter
                ModDomar = 6
            Case eClass.Cleric
                ModDomar = 7
            Case Else
                ModDomar = 10
        End Select
    End Function

    Function FreeMascotaIndex(UserIndex As Short) As Short
        '***************************************************
        'Author: Unknown
        'Last Modification: 02/03/09
        '02/03/09: ZaMa - Busca un indice libre de mascotas, revisando los types y no los indices de los npcs
        '***************************************************
        Dim j As Short
        For j = 1 To MAXMASCOTAS
            If UserList(UserIndex).MascotasType(j) = 0 Then
                FreeMascotaIndex = j
                Exit Function
            End If
        Next j
    End Function

    Sub DoDomar(UserIndex As Short, NpcIndex As Short)
        '***************************************************
        'Author: Nacho (Integer)
        'Last Modification: 01/05/2010
        '12/15/2008: ZaMa - Limits the number of the same type of pet to 2.
        '02/03/2009: ZaMa - Las criaturas domadas en zona segura, esperan afuera (desaparecen).
        '01/05/2010: ZaMa - Agrego bonificacion 11% para domar con flauta magica.
        '***************************************************

        Try

            Dim puntosDomar As Short
            Dim puntosRequeridos As Short
            Dim CanStay As Boolean
            Dim petType As Short
            Dim NroPets As Short


            If Npclist(NpcIndex).MaestroUser = UserIndex Then
                Call WriteConsoleMsg(UserIndex, "Ya domaste a esa criatura.", FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            Dim index As Short
            With UserList(UserIndex)
                If .NroMascotas < MAXMASCOTAS Then

                    If Npclist(NpcIndex).MaestroNpc > 0 Or Npclist(NpcIndex).MaestroUser > 0 Then
                        Call WriteConsoleMsg(UserIndex, "La criatura ya tiene amo.", FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If

                    If Not PuedeDomarMascota(UserIndex, NpcIndex) Then
                        Call _
                            WriteConsoleMsg(UserIndex, "No puedes domar más de dos criaturas del mismo tipo.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If

                    puntosDomar = CShort(.Stats.UserAtributos(eAtributos.Carisma))*
                                  CShort(.Stats.UserSkills(eSkill.Domar))

                    ' 20% de bonificacion
                    If .Invent.AnilloEqpObjIndex = FLAUTAELFICA Then
                        puntosRequeridos = Npclist(NpcIndex).flags.Domable*0.8

                        ' 11% de bonificacion
                    ElseIf .Invent.AnilloEqpObjIndex = FLAUTAMAGICA Then
                        puntosRequeridos = Npclist(NpcIndex).flags.Domable*0.89

                    Else
                        puntosRequeridos = Npclist(NpcIndex).flags.Domable
                    End If

                    If puntosRequeridos <= puntosDomar And RandomNumber(1, 5) = 1 Then
                        .NroMascotas = .NroMascotas + 1
                        index = FreeMascotaIndex(UserIndex)
                        .MascotasIndex(index) = NpcIndex
                        .MascotasType(index) = Npclist(NpcIndex).Numero

                        Npclist(NpcIndex).MaestroUser = UserIndex

                        Call FollowAmo(NpcIndex)
                        Call ReSpawnNpc(Npclist(NpcIndex))

                        Call _
                            WriteConsoleMsg(UserIndex, "La criatura te ha aceptado como su amo.",
                                            FontTypeNames.FONTTYPE_INFO)

                        ' Es zona segura?
                        CanStay = (MapInfo_Renamed(.Pos.Map).Pk = True)

                        If Not CanStay Then
                            petType = Npclist(NpcIndex).Numero
                            NroPets = .NroMascotas

                            Call QuitarNPC(NpcIndex)

                            .MascotasType(index) = petType
                            .NroMascotas = NroPets

                            Call _
                                WriteConsoleMsg(UserIndex,
                                                "No se permiten mascotas en zona segura. Éstas te esperarán afuera.",
                                                FontTypeNames.FONTTYPE_INFO)
                        End If

                        Call SubirSkill(UserIndex, eSkill.Domar, True)

                    Else
                        If Not .flags.UltimoMensaje = 5 Then
                            Call _
                                WriteConsoleMsg(UserIndex, "No has logrado domar la criatura.",
                                                FontTypeNames.FONTTYPE_INFO)
                            .flags.UltimoMensaje = 5
                        End If

                        Call SubirSkill(UserIndex, eSkill.Domar, False)
                    End If
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes controlar más criaturas.",
                                        FontTypeNames.FONTTYPE_INFO)
                End If
            End With


        Catch ex As Exception
            Console.WriteLine("Error in TieneObjetos: " & ex.Message)
            Call LogError("Error en DoDomar. Error " & Err.Number & " : " & Err.Description)
        End Try
    End Sub

    ''
    ' Checks if the user can tames a pet.
    '
    ' @param integer userIndex The user id from who wants tame the pet.
    ' @param integer NPCindex The index of the npc to tome.
    ' @return boolean True if can, false if not.
    Private Function PuedeDomarMascota(UserIndex As Short, NpcIndex As Short) As Boolean
        '***************************************************
        'Author: ZaMa
        'This function checks how many NPCs of the same type have
        'been tamed by the user.
        'Returns True if that amount is less than two.
        '***************************************************
        Dim i As Integer
        Dim numMascotas As Integer

        For i = 1 To MAXMASCOTAS
            If UserList(UserIndex).MascotasType(i) = Npclist(NpcIndex).Numero Then
                numMascotas = numMascotas + 1
            End If
        Next i

        If numMascotas <= 1 Then PuedeDomarMascota = True
    End Function

    Sub DoAdminInvisible(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 12/01/2010 (ZaMa)
        'Makes an admin invisible o visible.
        '13/07/2009: ZaMa - Now invisible admins' chars are erased from all clients, except from themselves.
        '12/01/2010: ZaMa - Los druidas pierden la inmunidad de ser atacados cuando pierden el efecto del mimetismo.
        '***************************************************

        With UserList(UserIndex)
            If .flags.AdminInvisible = 0 Then
                ' Sacamos el mimetizmo
                If .flags.Mimetizado = 1 Then
                    .Char_Renamed.body = .CharMimetizado.body
                    .Char_Renamed.Head = .CharMimetizado.Head
                    .Char_Renamed.CascoAnim = .CharMimetizado.CascoAnim
                    .Char_Renamed.ShieldAnim = .CharMimetizado.ShieldAnim
                    .Char_Renamed.WeaponAnim = .CharMimetizado.WeaponAnim
                    .Counters.Mimetismo = 0
                    .flags.Mimetizado = 0
                    ' Se fue el efecto del mimetismo, puede ser atacado por npcs
                    .flags.Ignorado = False
                End If

                .flags.AdminInvisible = 1
                .flags.invisible = 1
                .flags.Oculto = 1
                .flags.OldBody = .Char_Renamed.body
                .flags.OldHead = .Char_Renamed.Head
                .Char_Renamed.body = 0
                .Char_Renamed.Head = 0

                ' Solo el admin sabe que se hace invi
                Call EnviarDatosASlot(UserIndex, PrepareMessageSetInvisible(.Char_Renamed.CharIndex, True))
                'Le mandamos el mensaje para que borre el personaje a los clientes que estén cerca
                Call _
                    SendData(SendTarget.ToPCAreaButIndex, UserIndex,
                             PrepareMessageCharacterRemove(.Char_Renamed.CharIndex))
            Else
                .flags.AdminInvisible = 0
                .flags.invisible = 0
                .flags.Oculto = 0
                .Counters.TiempoOculto = 0
                .Char_Renamed.body = .flags.OldBody
                .Char_Renamed.Head = .flags.OldHead

                ' Solo el admin sabe que se hace visible
                Call _
                    EnviarDatosASlot(UserIndex,
                                     PrepareMessageCharacterChange(.Char_Renamed.body, .Char_Renamed.Head,
                                                                   .Char_Renamed.heading, .Char_Renamed.CharIndex,
                                                                   .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim,
                                                                   .Char_Renamed.FX, .Char_Renamed.loops,
                                                                   .Char_Renamed.CascoAnim))
                Call EnviarDatosASlot(UserIndex, PrepareMessageSetInvisible(.Char_Renamed.CharIndex, False))

                'Le mandamos el mensaje para crear el personaje a los clientes que estén cerca
                Call MakeUserChar(True, .Pos.Map, UserIndex, .Pos.Map, .Pos.X, .Pos.Y, True)
            End If
        End With
    End Sub

    Sub TratarDeHacerFogata(Map As Short, X As Short, Y As Short, UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim Suerte As Byte
        Dim exito As Byte
        'UPGRADE_NOTE: Obj se actualizó a Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim Obj_Renamed As Obj
        Dim posMadera As WorldPos

        If Not LegalPos(Map, X, Y) Then Exit Sub

        With posMadera
            .Map = Map
            .X = X
            .Y = Y
        End With

        If MapData(Map, X, Y).ObjInfo.ObjIndex <> 58 Then
            Call _
                WriteConsoleMsg(UserIndex, "Necesitas clickear sobre leña para hacer ramitas.",
                                FontTypeNames.FONTTYPE_INFO)
            Exit Sub
        End If

        If Distancia(posMadera, UserList(UserIndex).Pos) > 2 Then
            Call _
                WriteConsoleMsg(UserIndex, "Estás demasiado lejos para prender la fogata.",
                                FontTypeNames.FONTTYPE_INFO)
            Exit Sub
        End If

        If UserList(UserIndex).flags.Muerto = 1 Then
            Call _
                WriteConsoleMsg(UserIndex, "No puedes hacer fogatas estando muerto.",
                                FontTypeNames.FONTTYPE_INFO)
            Exit Sub
        End If

        If MapData(Map, X, Y).ObjInfo.Amount < 3 Then
            Call _
                WriteConsoleMsg(UserIndex, "Necesitas por lo menos tres troncos para hacer una fogata.",
                                FontTypeNames.FONTTYPE_INFO)
            Exit Sub
        End If

        Dim SupervivenciaSkill As Byte

        SupervivenciaSkill = UserList(UserIndex).Stats.UserSkills(eSkill.Supervivencia)

        If SupervivenciaSkill >= 0 And SupervivenciaSkill < 6 Then
            Suerte = 3
        ElseIf SupervivenciaSkill >= 6 And SupervivenciaSkill <= 34 Then
            Suerte = 2
        ElseIf SupervivenciaSkill >= 35 Then
            Suerte = 1
        End If

        exito = RandomNumber(1, Suerte)

        If exito = 1 Then
            Obj_Renamed.ObjIndex = FOGATA_APAG
            Obj_Renamed.Amount = MapData(Map, X, Y).ObjInfo.Amount\3

            Call _
                WriteConsoleMsg(UserIndex, "Has hecho " & Obj_Renamed.Amount & " fogatas.",
                                FontTypeNames.FONTTYPE_INFO)

            Call MakeObj(Obj_Renamed, Map, X, Y)

            'Seteamos la fogata como el nuevo TargetObj del user
            UserList(UserIndex).flags.TargetObj = FOGATA_APAG

            Call SubirSkill(UserIndex, eSkill.Supervivencia, True)
        Else
            '[CDT 17-02-2004]
            If Not UserList(UserIndex).flags.UltimoMensaje = 10 Then
                Call WriteConsoleMsg(UserIndex, "No has podido hacer la fogata.", FontTypeNames.FONTTYPE_INFO)
                UserList(UserIndex).flags.UltimoMensaje = 10
            End If
            '[/CDT]

            Call SubirSkill(UserIndex, eSkill.Supervivencia, False)
        End If
    End Sub

    Public Sub DoPescar(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 16/11/2009
        '16/11/2009: ZaMa - Implementado nuevo sistema de extraccion.
        '***************************************************
        Try

            Dim Suerte As Short
            Dim res As Short
            Dim CantidadItems As Short

            If UserList(UserIndex).clase = eClass.Worker Then
                Call QuitarSta(UserIndex, EsfuerzoPescarPescador)
            Else
                Call QuitarSta(UserIndex, EsfuerzoPescarGeneral)
            End If

            Dim Skill As Short
            Skill = UserList(UserIndex).Stats.UserSkills(eSkill.Pesca)
            Suerte = Int(- 0.00125*Skill*Skill - 0.3*Skill + 49)

            res = RandomNumber(1, Suerte)

            Dim MiObj As Obj
            If res <= 6 Then

                If UserList(UserIndex).clase = eClass.Worker Then
                    With UserList(UserIndex)
                        CantidadItems = 1 + MaximoInt(1, CShort((.Stats.ELV - 4)/5))
                    End With

                    MiObj.Amount = RandomNumber(1, CantidadItems)
                Else
                    MiObj.Amount = 1
                End If
                MiObj.ObjIndex = Pescado

                If Not MeterItemEnInventario(UserIndex, MiObj) Then
                    Call TirarItemAlPiso(UserList(UserIndex).Pos, MiObj)
                End If

                Call WriteConsoleMsg(UserIndex, "¡Has pescado un lindo pez!", FontTypeNames.FONTTYPE_INFO)

                Call SubirSkill(UserIndex, eSkill.Pesca, True)
            Else
                '[CDT 17-02-2004]
                If Not UserList(UserIndex).flags.UltimoMensaje = 6 Then
                    Call WriteConsoleMsg(UserIndex, "¡No has pescado nada!", FontTypeNames.FONTTYPE_INFO)
                    UserList(UserIndex).flags.UltimoMensaje = 6
                End If
                '[/CDT]

                Call SubirSkill(UserIndex, eSkill.Pesca, False)
            End If

            UserList(UserIndex).Reputacion.PlebeRep = UserList(UserIndex).Reputacion.PlebeRep + vlProleta
            If UserList(UserIndex).Reputacion.PlebeRep > MAXREP Then UserList(UserIndex).Reputacion.PlebeRep = MAXREP

            UserList(UserIndex).Counters.Trabajando = UserList(UserIndex).Counters.Trabajando + 1


        Catch ex As Exception
            Console.WriteLine("Error in PuedeDomarMascota: " & ex.Message)
            Call LogError("Error en DoPescar. Error " & Err.Number & " : " & Err.Description)
        End Try
    End Sub

    Public Sub DoPescarRed(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************
        Try

            Dim iSkill As Short
            Dim Suerte As Short
            Dim res As Short
            Dim EsPescador As Boolean

            If UserList(UserIndex).clase = eClass.Worker Then
                Call QuitarSta(UserIndex, EsfuerzoPescarPescador)
                EsPescador = True
            Else
                Call QuitarSta(UserIndex, EsfuerzoPescarGeneral)
                EsPescador = False
            End If

            iSkill = UserList(UserIndex).Stats.UserSkills(eSkill.Pesca)

            ' m = (60-11)/(1-10)
            ' y = mx - m*10 + 11

            Suerte = Int(- 0.00125*iSkill*iSkill - 0.3*iSkill + 49)

            Dim MiObj As Obj
            'UPGRADE_WARNING: El límite inferior de la matriz PecesPosibles ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            Dim PecesPosibles(4) As Short
            If Suerte > 0 Then
                res = RandomNumber(1, Suerte)

                If res < 6 Then

                    PecesPosibles(1) = PECES_POSIBLES.PESCADO1
                    PecesPosibles(2) = PECES_POSIBLES.PESCADO2
                    PecesPosibles(3) = PECES_POSIBLES.PESCADO3
                    PecesPosibles(4) = PECES_POSIBLES.PESCADO4

                    If EsPescador = True Then
                        MiObj.Amount = RandomNumber(1, 5)
                    Else
                        MiObj.Amount = 1
                    End If
                    MiObj.ObjIndex = PecesPosibles(RandomNumber(LBound(PecesPosibles), UBound(PecesPosibles)))

                    If Not MeterItemEnInventario(UserIndex, MiObj) Then
                        Call TirarItemAlPiso(UserList(UserIndex).Pos, MiObj)
                    End If

                    Call WriteConsoleMsg(UserIndex, "¡Has pescado algunos peces!", FontTypeNames.FONTTYPE_INFO)

                    Call SubirSkill(UserIndex, eSkill.Pesca, True)
                Else
                    Call WriteConsoleMsg(UserIndex, "¡No has pescado nada!", FontTypeNames.FONTTYPE_INFO)
                    Call SubirSkill(UserIndex, eSkill.Pesca, False)
                End If
            End If

            UserList(UserIndex).Reputacion.PlebeRep = UserList(UserIndex).Reputacion.PlebeRep + vlProleta
            If UserList(UserIndex).Reputacion.PlebeRep > MAXREP Then UserList(UserIndex).Reputacion.PlebeRep = MAXREP


        Catch ex As Exception
            Console.WriteLine("Error in DoPescarRed: " & ex.Message)
            Call LogError("Error en DoPescarRed")
        End Try
    End Sub

    ''
    ' Try to steal an item / gold to another character
    '
    ' @param LadrOnIndex Specifies reference to user that stoles
    ' @param VictimaIndex Specifies reference to user that is being stolen

    Public Sub DoRobar(LadrOnIndex As Short, VictimaIndex As Short)
        '*************************************************
        'Author: Unknown
        'Last modified: 05/04/2010
        'Last Modification By: ZaMa
        '24/07/08: Marco - Now it calls to WriteUpdateGold(VictimaIndex and LadrOnIndex) when the thief stoles gold. (MarKoxX)
        '27/11/2009: ZaMa - Optimizacion de codigo.
        '18/12/2009: ZaMa - Los ladrones ciudas pueden robar a pks.
        '01/04/2010: ZaMa - Los ladrones pasan a robar oro acorde a su nivel.
        '05/04/2010: ZaMa - Los armadas no pueden robarle a ciudadanos jamas.
        '23/04/2010: ZaMa - No se puede robar mas sin energia.
        '23/04/2010: ZaMa - El alcance de robo pasa a ser de 1 tile.
        '*************************************************

        Try

            If Not MapInfo_Renamed(UserList(VictimaIndex).Pos.Map).Pk Then Exit Sub

            If UserList(LadrOnIndex).flags.Seguro Then
                If Not criminal(VictimaIndex) Then
                    Call _
                        WriteConsoleMsg(LadrOnIndex, "Debes quitarte el seguro para robarle a un ciudadano.",
                                        FontTypeNames.FONTTYPE_FIGHT)
                    Exit Sub
                End If
            Else
                If UserList(LadrOnIndex).Faccion.ArmadaReal = 1 Then
                    Call _
                        WriteConsoleMsg(LadrOnIndex,
                                        "Los miembros del ejército real no tienen permitido robarle a ciudadanos.",
                                        FontTypeNames.FONTTYPE_FIGHT)
                    Exit Sub
                End If
            End If

            If TriggerZonaPelea(LadrOnIndex, VictimaIndex) <> eTrigger6.TRIGGER6_AUSENTE Then Exit Sub


            Dim GuantesHurto As Boolean
            Dim Suerte As Short
            Dim res As Short
            Dim RobarSkill As Byte
            Dim N As Short
            With UserList(LadrOnIndex)

                ' Caos robando a caos?
                If UserList(VictimaIndex).Faccion.FuerzasCaos = 1 And .Faccion.FuerzasCaos = 1 Then
                    Call _
                        WriteConsoleMsg(LadrOnIndex, "No puedes robar a otros miembros de la legión oscura.",
                                        FontTypeNames.FONTTYPE_FIGHT)
                    Exit Sub
                End If

                ' Tiene energia?
                If .Stats.MinSta < 15 Then
                    If .Genero = eGenero.Hombre Then
                        Call _
                            WriteConsoleMsg(LadrOnIndex, "Estás muy cansado para robar.",
                                            FontTypeNames.FONTTYPE_INFO)
                    Else
                        Call _
                            WriteConsoleMsg(LadrOnIndex, "Estás muy cansada para robar.",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If

                    Exit Sub
                End If

                ' Quito energia
                Call QuitarSta(LadrOnIndex, 15)


                If .Invent.AnilloEqpObjIndex = GUANTE_HURTO Then GuantesHurto = True

                If UserList(VictimaIndex).flags.Privilegios And PlayerType.User Then


                    RobarSkill = .Stats.UserSkills(eSkill.Robar)

                    If RobarSkill <= 10 And RobarSkill >= - 1 Then
                        Suerte = 35
                    ElseIf RobarSkill <= 20 And RobarSkill >= 11 Then
                        Suerte = 30
                    ElseIf RobarSkill <= 30 And RobarSkill >= 21 Then
                        Suerte = 28
                    ElseIf RobarSkill <= 40 And RobarSkill >= 31 Then
                        Suerte = 24
                    ElseIf RobarSkill <= 50 And RobarSkill >= 41 Then
                        Suerte = 22
                    ElseIf RobarSkill <= 60 And RobarSkill >= 51 Then
                        Suerte = 20
                    ElseIf RobarSkill <= 70 And RobarSkill >= 61 Then
                        Suerte = 18
                    ElseIf RobarSkill <= 80 And RobarSkill >= 71 Then
                        Suerte = 15
                    ElseIf RobarSkill <= 90 And RobarSkill >= 81 Then
                        Suerte = 10
                    ElseIf RobarSkill < 100 And RobarSkill >= 91 Then
                        Suerte = 7
                    ElseIf RobarSkill = 100 Then
                        Suerte = 5
                    End If

                    res = RandomNumber(1, Suerte)

                    If res < 3 Then 'Exito robo

                        If (RandomNumber(1, 50) < 25) And (.clase = eClass.Thief) Then
                            If TieneObjetosRobables(VictimaIndex) Then
                                Call RobarObjeto(LadrOnIndex, VictimaIndex)
                            Else
                                Call _
                                    WriteConsoleMsg(LadrOnIndex, UserList(VictimaIndex).name & " no tiene objetos.",
                                                    FontTypeNames.FONTTYPE_INFO)
                            End If
                        Else 'Roba oro
                            If UserList(VictimaIndex).Stats.GLD > 0 Then

                                If .clase = eClass.Thief Then
                                    ' Si no tine puestos los guantes de hurto roba un 50% menos. Pablo (ToxicWaste)
                                    If GuantesHurto Then
                                        N = RandomNumber(.Stats.ELV*50, .Stats.ELV*100)
                                    Else
                                        N = RandomNumber(.Stats.ELV*25, .Stats.ELV*50)
                                    End If
                                Else
                                    N = RandomNumber(1, 100)
                                End If
                                If N > UserList(VictimaIndex).Stats.GLD Then N = UserList(VictimaIndex).Stats.GLD
                                UserList(VictimaIndex).Stats.GLD = UserList(VictimaIndex).Stats.GLD - N

                                .Stats.GLD = .Stats.GLD + N
                                If .Stats.GLD > MAXORO Then .Stats.GLD = MAXORO

                                Call _
                                    WriteConsoleMsg(LadrOnIndex,
                                                    "Le has robado " & N & " monedas de oro a " &
                                                    UserList(VictimaIndex).name, FontTypeNames.FONTTYPE_INFO)
                                Call WriteUpdateGold(LadrOnIndex) 'Le actualizamos la billetera al ladron

                                Call WriteUpdateGold(VictimaIndex) 'Le actualizamos la billetera a la victima
                                Call FlushBuffer(VictimaIndex)
                            Else
                                Call _
                                    WriteConsoleMsg(LadrOnIndex, UserList(VictimaIndex).name & " no tiene oro.",
                                                    FontTypeNames.FONTTYPE_INFO)
                            End If
                        End If

                        Call SubirSkill(LadrOnIndex, eSkill.Robar, True)
                    Else
                        Call _
                            WriteConsoleMsg(LadrOnIndex, "¡No has logrado robar nada!", FontTypeNames.FONTTYPE_INFO)
                        Call _
                            WriteConsoleMsg(VictimaIndex, "¡" & .name & " ha intentado robarte!",
                                            FontTypeNames.FONTTYPE_INFO)
                        Call _
                            WriteConsoleMsg(VictimaIndex, "¡" & .name & " es un criminal!",
                                            FontTypeNames.FONTTYPE_INFO)
                        Call FlushBuffer(VictimaIndex)

                        Call SubirSkill(LadrOnIndex, eSkill.Robar, False)
                    End If

                    If Not criminal(LadrOnIndex) Then
                        If Not criminal(VictimaIndex) Then
                            Call VolverCriminal(LadrOnIndex)
                        End If
                    End If

                    ' Se pudo haber convertido si robo a un ciuda
                    If criminal(LadrOnIndex) Then
                        .Reputacion.LadronesRep = .Reputacion.LadronesRep + vlLadron
                        If .Reputacion.LadronesRep > MAXREP Then .Reputacion.LadronesRep = MAXREP
                    End If
                End If
            End With


        Catch ex As Exception
            Console.WriteLine("Error in DoRobar: " & ex.Message)
            Call LogError("Error en DoRobar. Error " & Err.Number & " : " & Err.Description)
        End Try
    End Sub

    ''
    ' Check if one item is stealable
    '
    ' @param VictimaIndex Specifies reference to victim
    ' @param Slot Specifies reference to victim's inventory slot
    ' @return If the item is stealable
    Public Function ObjEsRobable(VictimaIndex As Short, Slot As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        ' Agregué los barcos
        ' Esta funcion determina qué objetos son robables.
        '***************************************************

        Dim OI As Short

        OI = UserList(VictimaIndex).Invent.Object_Renamed(Slot).ObjIndex

        ObjEsRobable = ObjData_Renamed(OI).OBJType <> eOBJType.otLlaves And
                       UserList(VictimaIndex).Invent.Object_Renamed(Slot).Equipped = 0 And ObjData_Renamed(OI).Real = 0 And
                       ObjData_Renamed(OI).Caos = 0 And ObjData_Renamed(OI).OBJType <> eOBJType.otBarcos
    End Function

    ''
    ' Try to steal an item to another character
    '
    ' @param LadrOnIndex Specifies reference to user that stoles
    ' @param VictimaIndex Specifies reference to user that is being stolen
    Public Sub RobarObjeto(LadrOnIndex As Short, VictimaIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 02/04/2010
        '02/04/2010: ZaMa - Modifico la cantidad de items robables por el ladron.
        '***************************************************

        Dim flag As Boolean
        Dim i As Short
        flag = False

        If RandomNumber(1, 12) < 6 Then 'Comenzamos por el principio o el final?
            i = 1
            Do While Not flag And i <= UserList(VictimaIndex).CurrentInventorySlots
                'Hay objeto en este slot?
                If UserList(VictimaIndex).Invent.Object_Renamed(i).ObjIndex > 0 Then
                    If ObjEsRobable(VictimaIndex, i) Then
                        If RandomNumber(1, 10) < 4 Then flag = True
                    End If
                End If
                If Not flag Then i = i + 1
            Loop
        Else
            i = 20
            Do While Not flag And i > 0
                'Hay objeto en este slot?
                If UserList(VictimaIndex).Invent.Object_Renamed(i).ObjIndex > 0 Then
                    If ObjEsRobable(VictimaIndex, i) Then
                        If RandomNumber(1, 10) < 4 Then flag = True
                    End If
                End If
                If Not flag Then i = i - 1
            Loop
        End If

        Dim MiObj As Obj
        Dim num As Byte
        Dim ObjAmount As Short
        If flag Then

            ObjAmount = UserList(VictimaIndex).Invent.Object_Renamed(i).Amount

            'Cantidad al azar entre el 5% y el 10% del total, con minimo 1.
            num = MaximoInt(1, RandomNumber(ObjAmount*0.05, ObjAmount*0.1))

            MiObj.Amount = num
            MiObj.ObjIndex = UserList(VictimaIndex).Invent.Object_Renamed(i).ObjIndex

            UserList(VictimaIndex).Invent.Object_Renamed(i).Amount = ObjAmount - num

            If UserList(VictimaIndex).Invent.Object_Renamed(i).Amount <= 0 Then
                Call QuitarUserInvItem(VictimaIndex, CByte(i), 1)
            End If

            Call UpdateUserInv(False, VictimaIndex, CByte(i))

            If Not MeterItemEnInventario(LadrOnIndex, MiObj) Then
                Call TirarItemAlPiso(UserList(LadrOnIndex).Pos, MiObj)
            End If

            If UserList(LadrOnIndex).clase = eClass.Thief Then
                Call _
                    WriteConsoleMsg(LadrOnIndex,
                                    "Has robado " & MiObj.Amount & " " & ObjData_Renamed(MiObj.ObjIndex).name,
                                    FontTypeNames.FONTTYPE_INFO)
            Else
                Call _
                    WriteConsoleMsg(LadrOnIndex,
                                    "Has hurtado " & MiObj.Amount & " " & ObjData_Renamed(MiObj.ObjIndex).name,
                                    FontTypeNames.FONTTYPE_INFO)
            End If
        Else
            Call _
                WriteConsoleMsg(LadrOnIndex, "No has logrado robar ningún objeto.", FontTypeNames.FONTTYPE_INFO)
        End If

        'If exiting, cancel de quien es robado
        Call CancelExit(VictimaIndex)
    End Sub

    Public Sub DoApuñalar(UserIndex As Short, VictimNpcIndex As Short, VictimUserIndex As Short,
                          daño As Short)
        '***************************************************
        'Autor: Nacho (Integer) & Unknown (orginal version)
        'Last Modification: 04/17/08 - (NicoNZ)
        'Simplifique la cuenta que hacia para sacar la suerte
        'y arregle la cuenta que hacia para sacar el daño
        '***************************************************
        Dim Suerte As Short
        Dim Skill As Short

        Skill = UserList(UserIndex).Stats.UserSkills(eSkill.Apuñalar)

        Select Case UserList(UserIndex).clase
            Case eClass.Assasin
                Suerte = Int(((0.00003*Skill - 0.002)*Skill + 0.098)*Skill + 4.25)

            Case eClass.Cleric, eClass.Paladin, eClass.Pirat
                Suerte = Int(((0.000003*Skill + 0.0006)*Skill + 0.0107)*Skill + 4.93)

            Case eClass.Bard
                Suerte = Int(((0.000002*Skill + 0.0002)*Skill + 0.032)*Skill + 4.81)

            Case Else
                Suerte = Int(0.0361*Skill + 4.39)
        End Select


        If RandomNumber(0, 100) < Suerte Then
            If VictimUserIndex <> 0 Then
                If UserList(UserIndex).clase = eClass.Assasin Then
                    daño = Math.Round(daño*1.4, 0)
                Else
                    daño = Math.Round(daño*1.5, 0)
                End If

                UserList(VictimUserIndex).Stats.MinHp = UserList(VictimUserIndex).Stats.MinHp - daño
                Call _
                    WriteConsoleMsg(UserIndex, "Has apuñalado a " & UserList(VictimUserIndex).name & " por " & daño,
                                    FontTypeNames.FONTTYPE_FIGHT)
                Call _
                    WriteConsoleMsg(VictimUserIndex, "Te ha apuñalado " & UserList(UserIndex).name & " por " & daño,
                                    FontTypeNames.FONTTYPE_FIGHT)

                Call FlushBuffer(VictimUserIndex)
            Else
                Npclist(VictimNpcIndex).Stats.MinHp = Npclist(VictimNpcIndex).Stats.MinHp - Int(daño*2)
                Call _
                    WriteConsoleMsg(UserIndex, "Has apuñalado la criatura por " & Int(daño*2),
                                    FontTypeNames.FONTTYPE_FIGHT)
                '[Alejo]
                Call CalcularDarExp(UserIndex, VictimNpcIndex, daño*2)
            End If

            Call SubirSkill(UserIndex, eSkill.Apuñalar, True)
        Else
            Call _
                WriteConsoleMsg(UserIndex, "¡No has logrado apuñalar a tu enemigo!",
                                FontTypeNames.FONTTYPE_FIGHT)
            Call SubirSkill(UserIndex, eSkill.Apuñalar, False)
        End If
    End Sub

    Public Sub DoAcuchillar(UserIndex As Short, VictimNpcIndex As Short, VictimUserIndex As Short,
                            daño As Short)
        '***************************************************
        'Autor: ZaMa
        'Last Modification: 12/01/2010
        '***************************************************

        If UserList(UserIndex).clase <> eClass.Pirat Then Exit Sub
        If UserList(UserIndex).Invent.WeaponEqpSlot = 0 Then Exit Sub

        If RandomNumber(0, 100) < PROB_ACUCHILLAR Then
            daño = Int(daño*DAÑO_ACUCHILLAR)

            If VictimUserIndex <> 0 Then
                UserList(VictimUserIndex).Stats.MinHp = UserList(VictimUserIndex).Stats.MinHp - daño
                Call _
                    WriteConsoleMsg(UserIndex, "Has acuchillado a " & UserList(VictimUserIndex).name & " por " & daño,
                                    FontTypeNames.FONTTYPE_FIGHT)
                Call _
                    WriteConsoleMsg(VictimUserIndex, UserList(UserIndex).name & " te ha acuchillado por " & daño,
                                    FontTypeNames.FONTTYPE_FIGHT)
            Else
                Npclist(VictimNpcIndex).Stats.MinHp = Npclist(VictimNpcIndex).Stats.MinHp - daño
                Call _
                    WriteConsoleMsg(UserIndex, "Has acuchillado a la criatura por " & daño,
                                    FontTypeNames.FONTTYPE_FIGHT)
                Call CalcularDarExp(UserIndex, VictimNpcIndex, daño)
            End If
        End If
    End Sub

    Public Sub DoGolpeCritico(UserIndex As Short, VictimNpcIndex As Short, VictimUserIndex As Short,
                              daño As Short)
        '***************************************************
        'Autor: Pablo (ToxicWaste)
        'Last Modification: 28/01/2007
        '***************************************************
        Dim Suerte As Short
        Dim Skill As Short

        If UserList(UserIndex).clase <> eClass.Bandit Then Exit Sub
        If UserList(UserIndex).Invent.WeaponEqpSlot = 0 Then Exit Sub
        If ObjData_Renamed(UserList(UserIndex).Invent.WeaponEqpObjIndex).name <> "Espada Vikinga" Then Exit Sub


        Skill = UserList(UserIndex).Stats.UserSkills(eSkill.Wrestling)

        Suerte = Int((((0.00000003*Skill + 0.000006)*Skill + 0.000107)*Skill + 0.0893)*100)

        If RandomNumber(0, 100) < Suerte Then
            daño = Int(daño*0.75)
            If VictimUserIndex <> 0 Then
                UserList(VictimUserIndex).Stats.MinHp = UserList(VictimUserIndex).Stats.MinHp - daño
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Has golpeado críticamente a " & UserList(VictimUserIndex).name & " por " & daño &
                                    ".", FontTypeNames.FONTTYPE_FIGHT)
                Call _
                    WriteConsoleMsg(VictimUserIndex,
                                    UserList(UserIndex).name & " te ha golpeado críticamente por " & daño & ".",
                                    FontTypeNames.FONTTYPE_FIGHT)
            Else
                Npclist(VictimNpcIndex).Stats.MinHp = Npclist(VictimNpcIndex).Stats.MinHp - daño
                Call _
                    WriteConsoleMsg(UserIndex, "Has golpeado críticamente a la criatura por " & daño & ".",
                                    FontTypeNames.FONTTYPE_FIGHT)
                '[Alejo]
                Call CalcularDarExp(UserIndex, VictimNpcIndex, daño)
            End If
        End If
    End Sub

    Public Sub QuitarSta(UserIndex As Short, Cantidad As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

            UserList(UserIndex).Stats.MinSta = UserList(UserIndex).Stats.MinSta - Cantidad
            If UserList(UserIndex).Stats.MinSta < 0 Then UserList(UserIndex).Stats.MinSta = 0
            Call WriteUpdateSta(UserIndex)


        Catch ex As Exception
            Console.WriteLine("Error in ObjEsRobable: " & ex.Message)
            Call LogError("Error en QuitarSta. Error " & Err.Number & " : " & Err.Description)
        End Try
    End Sub

    Public Sub DoTalar(UserIndex As Short, Optional ByVal DarMaderaElfica As Boolean = False)
        '***************************************************
        'Autor: Unknown
        'Last Modification: 16/11/2009
        '16/11/2009: ZaMa - Ahora Se puede dar madera elfica.
        '16/11/2009: ZaMa - Implementado nuevo sistema de extraccion.
        '***************************************************
        Try

            Dim Suerte As Short
            Dim res As Short
            Dim CantidadItems As Short

            If UserList(UserIndex).clase = eClass.Worker Then
                Call QuitarSta(UserIndex, EsfuerzoTalarLeñador)
            Else
                Call QuitarSta(UserIndex, EsfuerzoTalarGeneral)
            End If

            Dim Skill As Short
            Skill = UserList(UserIndex).Stats.UserSkills(eSkill.Talar)
            Suerte = Int(- 0.00125*Skill*Skill - 0.3*Skill + 49)

            res = RandomNumber(1, Suerte)

            Dim MiObj As Obj
            If res <= 6 Then

                If UserList(UserIndex).clase = eClass.Worker Then
                    With UserList(UserIndex)
                        CantidadItems = 1 + MaximoInt(1, CShort((.Stats.ELV - 4)/5))
                    End With

                    MiObj.Amount = RandomNumber(1, CantidadItems)
                Else
                    MiObj.Amount = 1
                End If

                MiObj.ObjIndex = IIf(DarMaderaElfica, LeñaElfica, Leña)


                If Not MeterItemEnInventario(UserIndex, MiObj) Then

                    Call TirarItemAlPiso(UserList(UserIndex).Pos, MiObj)

                End If

                Call WriteConsoleMsg(UserIndex, "¡Has conseguido algo de leña!", FontTypeNames.FONTTYPE_INFO)

                Call SubirSkill(UserIndex, eSkill.Talar, True)
            Else
                '[CDT 17-02-2004]
                If Not UserList(UserIndex).flags.UltimoMensaje = 8 Then
                    Call WriteConsoleMsg(UserIndex, "¡No has obtenido leña!", FontTypeNames.FONTTYPE_INFO)
                    UserList(UserIndex).flags.UltimoMensaje = 8
                End If
                '[/CDT]
                Call SubirSkill(UserIndex, eSkill.Talar, False)
            End If

            UserList(UserIndex).Reputacion.PlebeRep = UserList(UserIndex).Reputacion.PlebeRep + vlProleta
            If UserList(UserIndex).Reputacion.PlebeRep > MAXREP Then UserList(UserIndex).Reputacion.PlebeRep = MAXREP

            UserList(UserIndex).Counters.Trabajando = UserList(UserIndex).Counters.Trabajando + 1


        Catch ex As Exception
            Console.WriteLine("Error in DoTalar: " & ex.Message)
            Call LogError("Error en DoTalar")
        End Try
    End Sub

    Public Sub DoMineria(UserIndex As Short)
        '***************************************************
        'Autor: Unknown
        'Last Modification: 16/11/2009
        '16/11/2009: ZaMa - Implementado nuevo sistema de extraccion.
        '***************************************************
        Try

            Dim Suerte As Short
            Dim res As Short
            Dim CantidadItems As Short

            Dim Skill As Short
            Dim MiObj As Obj
            With UserList(UserIndex)
                If .clase = eClass.Worker Then
                    Call QuitarSta(UserIndex, EsfuerzoExcavarMinero)
                Else
                    Call QuitarSta(UserIndex, EsfuerzoExcavarGeneral)
                End If

                Skill = .Stats.UserSkills(eSkill.Mineria)
                Suerte = Int(- 0.00125*Skill*Skill - 0.3*Skill + 49)

                res = RandomNumber(1, Suerte)

                If res <= 5 Then

                    If .flags.TargetObj = 0 Then Exit Sub

                    MiObj.ObjIndex = ObjData_Renamed(.flags.TargetObj).MineralIndex

                    If UserList(UserIndex).clase = eClass.Worker Then
                        CantidadItems = 1 + MaximoInt(1, CShort((.Stats.ELV - 4)/5))

                        MiObj.Amount = RandomNumber(1, CantidadItems)
                    Else
                        MiObj.Amount = 1
                    End If

                    If Not MeterItemEnInventario(UserIndex, MiObj) Then Call TirarItemAlPiso(.Pos, MiObj)

                    Call _
                        WriteConsoleMsg(UserIndex, "¡Has extraido algunos minerales!", FontTypeNames.FONTTYPE_INFO)

                    Call SubirSkill(UserIndex, eSkill.Mineria, True)
                Else
                    '[CDT 17-02-2004]
                    If Not .flags.UltimoMensaje = 9 Then
                        Call WriteConsoleMsg(UserIndex, "¡No has conseguido nada!", FontTypeNames.FONTTYPE_INFO)
                        .flags.UltimoMensaje = 9
                    End If
                    '[/CDT]
                    Call SubirSkill(UserIndex, eSkill.Mineria, False)
                End If

                .Reputacion.PlebeRep = .Reputacion.PlebeRep + vlProleta
                If .Reputacion.PlebeRep > MAXREP Then .Reputacion.PlebeRep = MAXREP

                .Counters.Trabajando = UserList(UserIndex).Counters.Trabajando + 1
            End With


        Catch ex As Exception
            Console.WriteLine("Error in DoMineria: " & ex.Message)
            Call LogError("Error en Sub DoMineria")
        End Try
    End Sub

    Public Sub DoMeditar(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim Suerte As Short
        Dim res As Short
        Dim cant As Short
        Dim MeditarSkill As Byte
        Dim TActual As Integer
        With UserList(UserIndex)
            .Counters.IdleCount = 0


            'Barrin 3/10/03
            'Esperamos a que se termine de concentrar
            TActual = GetTickCount()
            If TActual - .Counters.tInicioMeditar < TIEMPO_INICIOMEDITAR Then
                Exit Sub
            End If

            If .Counters.bPuedeMeditar = False Then
                .Counters.bPuedeMeditar = True
            End If

            If .Stats.MinMAN >= .Stats.MaxMAN Then
                Call WriteConsoleMsg(UserIndex, "Has terminado de meditar.", FontTypeNames.FONTTYPE_INFO)
                Call WriteMeditateToggle(UserIndex)
                .flags.Meditando = False
                .Char_Renamed.FX = 0
                .Char_Renamed.loops = 0
                Call _
                    SendData(SendTarget.ToPCArea, UserIndex,
                             PrepareMessageCreateFX(.Char_Renamed.CharIndex, 0, 0))
                Exit Sub
            End If

            MeditarSkill = .Stats.UserSkills(eSkill.Meditar)

            If MeditarSkill <= 10 And MeditarSkill >= - 1 Then
                Suerte = 35
            ElseIf MeditarSkill <= 20 And MeditarSkill >= 11 Then
                Suerte = 30
            ElseIf MeditarSkill <= 30 And MeditarSkill >= 21 Then
                Suerte = 28
            ElseIf MeditarSkill <= 40 And MeditarSkill >= 31 Then
                Suerte = 24
            ElseIf MeditarSkill <= 50 And MeditarSkill >= 41 Then
                Suerte = 22
            ElseIf MeditarSkill <= 60 And MeditarSkill >= 51 Then
                Suerte = 20
            ElseIf MeditarSkill <= 70 And MeditarSkill >= 61 Then
                Suerte = 18
            ElseIf MeditarSkill <= 80 And MeditarSkill >= 71 Then
                Suerte = 15
            ElseIf MeditarSkill <= 90 And MeditarSkill >= 81 Then
                Suerte = 10
            ElseIf MeditarSkill < 100 And MeditarSkill >= 91 Then
                Suerte = 7
            ElseIf MeditarSkill = 100 Then
                Suerte = 5
            End If
            res = RandomNumber(1, Suerte)

            If res = 1 Then

                cant = Porcentaje(.Stats.MaxMAN, PorcentajeRecuperoMana)
                If cant <= 0 Then cant = 1
                .Stats.MinMAN = .Stats.MinMAN + cant
                If .Stats.MinMAN > .Stats.MaxMAN Then .Stats.MinMAN = .Stats.MaxMAN

                If Not .flags.UltimoMensaje = 22 Then
                    Call _
                        WriteConsoleMsg(UserIndex, "¡Has recuperado " & cant & " puntos de maná!",
                                        FontTypeNames.FONTTYPE_INFO)
                    .flags.UltimoMensaje = 22
                End If

                Call WriteUpdateMana(UserIndex)
                Call SubirSkill(UserIndex, eSkill.Meditar, True)
            Else
                Call SubirSkill(UserIndex, eSkill.Meditar, False)
            End If
        End With
    End Sub

    Public Sub DoDesequipar(UserIndex As Short, VictimIndex As Short)
        '***************************************************
        'Author: ZaMa
        'Last Modif: 15/04/2010
        'Unequips either shield, weapon or helmet from target user.
        '***************************************************

        Dim Probabilidad As Short
        Dim Resultado As Short
        Dim WrestlingSkill As Byte
        Dim AlgoEquipado As Boolean

        With UserList(UserIndex)
            ' Si no tiene guantes de hurto no desequipa.
            If .Invent.AnilloEqpObjIndex <> GUANTE_HURTO Then Exit Sub

            ' Si no esta solo con manos, no desequipa tampoco.
            If .Invent.WeaponEqpObjIndex > 0 Then Exit Sub

            WrestlingSkill = .Stats.UserSkills(eSkill.Wrestling)

            Probabilidad = WrestlingSkill*0.2 + .Stats.ELV*0.66
        End With

        With UserList(VictimIndex)
            ' Si tiene escudo, intenta desequiparlo
            If .Invent.EscudoEqpObjIndex > 0 Then

                Resultado = RandomNumber(1, 100)

                If Resultado <= Probabilidad Then
                    ' Se lo desequipo
                    Call Desequipar(VictimIndex, .Invent.EscudoEqpSlot)

                    Call _
                        WriteConsoleMsg(UserIndex, "Has logrado desequipar el escudo de tu oponente!",
                                        FontTypeNames.FONTTYPE_FIGHT)

                    If .Stats.ELV < 20 Then
                        Call _
                            WriteConsoleMsg(VictimIndex, "¡Tu oponente te ha desequipado el escudo!",
                                            FontTypeNames.FONTTYPE_FIGHT)
                    End If

                    Call FlushBuffer(VictimIndex)

                    Exit Sub
                End If

                AlgoEquipado = True
            End If

            ' No tiene escudo, o fallo desequiparlo, entonces trata de desequipar arma
            If .Invent.WeaponEqpObjIndex > 0 Then

                Resultado = RandomNumber(1, 100)

                If Resultado <= Probabilidad Then
                    ' Se lo desequipo
                    Call Desequipar(VictimIndex, .Invent.WeaponEqpSlot)

                    Call _
                        WriteConsoleMsg(UserIndex, "Has logrado desarmar a tu oponente!",
                                        FontTypeNames.FONTTYPE_FIGHT)

                    If .Stats.ELV < 20 Then
                        Call _
                            WriteConsoleMsg(VictimIndex, "¡Tu oponente te ha desarmado!",
                                            FontTypeNames.FONTTYPE_FIGHT)
                    End If

                    Call FlushBuffer(VictimIndex)

                    Exit Sub
                End If

                AlgoEquipado = True
            End If

            ' No tiene arma, o fallo desequiparla, entonces trata de desequipar casco
            If .Invent.CascoEqpObjIndex > 0 Then

                Resultado = RandomNumber(1, 100)

                If Resultado <= Probabilidad Then
                    ' Se lo desequipo
                    Call Desequipar(VictimIndex, .Invent.CascoEqpSlot)

                    Call _
                        WriteConsoleMsg(UserIndex, "Has logrado desequipar el casco de tu oponente!",
                                        FontTypeNames.FONTTYPE_FIGHT)

                    If .Stats.ELV < 20 Then
                        Call _
                            WriteConsoleMsg(VictimIndex, "¡Tu oponente te ha desequipado el casco!",
                                            FontTypeNames.FONTTYPE_FIGHT)
                    End If

                    Call FlushBuffer(VictimIndex)

                    Exit Sub
                End If

                AlgoEquipado = True
            End If

            If AlgoEquipado Then
                Call _
                    WriteConsoleMsg(UserIndex, "Tu oponente no tiene equipado items!",
                                    FontTypeNames.FONTTYPE_FIGHT)
            Else
                Call _
                    WriteConsoleMsg(UserIndex, "No has logrado desequipar ningún item a tu oponente!",
                                    FontTypeNames.FONTTYPE_FIGHT)
            End If

        End With
    End Sub

    Public Sub DoHurtar(UserIndex As Short, VictimaIndex As Short)
        '***************************************************
        'Author: Pablo (ToxicWaste)
        'Last Modif: 03/03/2010
        'Implements the pick pocket skill of the Bandit :)
        '03/03/2010 - Pato: Sólo se puede hurtar si no está en trigger 6 :)
        '***************************************************
        If TriggerZonaPelea(UserIndex, VictimaIndex) <> eTrigger6.TRIGGER6_AUSENTE Then Exit Sub

        If UserList(UserIndex).clase <> eClass.Bandit Then Exit Sub
        'Esto es precario y feo, pero por ahora no se me ocurrió nada mejor.
        'Uso el slot de los anillos para "equipar" los guantes.
        'Y los reconozco porque les puse DefensaMagicaMin y Max = 0
        If UserList(UserIndex).Invent.AnilloEqpObjIndex <> GUANTE_HURTO Then Exit Sub

        Dim res As Short
        res = RandomNumber(1, 100)
        If (res < 20) Then
            If TieneObjetosRobables(VictimaIndex) Then
                Call RobarObjeto(UserIndex, VictimaIndex)
                Call _
                    WriteConsoleMsg(VictimaIndex, "¡" & UserList(UserIndex).name & " es un Bandido!",
                                    FontTypeNames.FONTTYPE_INFO)
            Else
                Call _
                    WriteConsoleMsg(UserIndex, UserList(VictimaIndex).name & " no tiene objetos.",
                                    FontTypeNames.FONTTYPE_INFO)
            End If
        End If
    End Sub

    Public Sub DoHandInmo(UserIndex As Short, VictimaIndex As Short)
        '***************************************************
        'Author: Pablo (ToxicWaste)
        'Last Modif: 17/02/2007
        'Implements the special Skill of the Thief
        '***************************************************
        If UserList(VictimaIndex).flags.Paralizado = 1 Then Exit Sub
        If UserList(UserIndex).clase <> eClass.Thief Then Exit Sub


        If UserList(UserIndex).Invent.AnilloEqpObjIndex <> GUANTE_HURTO Then Exit Sub

        Dim res As Short
        res = RandomNumber(0, 100)
        If res < (UserList(UserIndex).Stats.UserSkills(eSkill.Wrestling)/4) Then
            UserList(VictimaIndex).flags.Paralizado = 1
            UserList(VictimaIndex).Counters.Paralisis = IntervaloParalizado/2
            Call WriteParalizeOK(VictimaIndex)
            Call _
                WriteConsoleMsg(UserIndex, "Tu golpe ha dejado inmóvil a tu oponente",
                                FontTypeNames.FONTTYPE_INFO)
            Call WriteConsoleMsg(VictimaIndex, "¡El golpe te ha dejado inmóvil!", FontTypeNames.FONTTYPE_INFO)
        End If
    End Sub

    Public Sub Desarmar(UserIndex As Short, VictimIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 02/04/2010 (ZaMa)
        '02/04/2010: ZaMa - Nueva formula para desarmar.
        '***************************************************

        Dim Probabilidad As Short
        Dim Resultado As Short
        Dim WrestlingSkill As Byte

        With UserList(UserIndex)
            WrestlingSkill = .Stats.UserSkills(eSkill.Wrestling)

            Probabilidad = WrestlingSkill*0.2 + .Stats.ELV*0.66

            Resultado = RandomNumber(1, 100)

            If Resultado <= Probabilidad Then
                Call Desequipar(VictimIndex, UserList(VictimIndex).Invent.WeaponEqpSlot)
                Call _
                    WriteConsoleMsg(UserIndex, "Has logrado desarmar a tu oponente!",
                                    FontTypeNames.FONTTYPE_FIGHT)
                If UserList(VictimIndex).Stats.ELV < 20 Then
                    Call _
                        WriteConsoleMsg(VictimIndex, "¡Tu oponente te ha desarmado!",
                                        FontTypeNames.FONTTYPE_FIGHT)
                End If
                Call FlushBuffer(VictimIndex)
            End If
        End With
    End Sub


    Public Function MaxItemsConstruibles(UserIndex As Short) As Short
        '***************************************************
        'Author: ZaMa
        'Last Modification: 29/01/2010
        '
        '***************************************************
        MaxItemsConstruibles = MaximoInt(1, CShort((UserList(UserIndex).Stats.ELV - 4)/5))
    End Function
End Module
