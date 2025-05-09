Option Strict Off
Option Explicit On
Module InvUsuario
    Public Function TieneObjetosRobables(UserIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        '17/09/02
        'Agregue que la función se asegure que el objeto no es un barco

        Try

            Dim i As Short
            Dim ObjIndex As Short

            For i = 1 To UserList(UserIndex).CurrentInventorySlots
                ObjIndex = UserList(UserIndex).Invent.Object_Renamed(i).ObjIndex
                If ObjIndex > 0 Then
                    If _
                        (ObjData_Renamed(ObjIndex).OBJType <> eOBJType.otLlaves And
                         ObjData_Renamed(ObjIndex).OBJType <> eOBJType.otBarcos) Then
                        TieneObjetosRobables = True
                        Exit Function
                    End If

                End If
            Next i

        Catch ex As Exception
            Console.WriteLine("Error in TieneObjetosRobables: " & ex.Message)
        End Try
    End Function

    Function ClasePuedeUsarItem(UserIndex As Short, ObjIndex As Short, Optional ByRef sMotivo As String = "") _
        As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: 14/01/2010 (ZaMa)
        '14/01/2010: ZaMa - Agrego el motivo por el que no puede equipar/usar el item.
        '***************************************************

        Try

            Dim flag As Boolean

            'Admins can use ANYTHING!
            Dim i As Short
            If UserList(UserIndex).flags.Privilegios And PlayerType.User Then
                If ObjData_Renamed(ObjIndex).ClaseProhibida(1) <> 0 Then
                    For i = 1 To NUMCLASES
                        If ObjData_Renamed(ObjIndex).ClaseProhibida(i) = UserList(UserIndex).clase Then
                            ClasePuedeUsarItem = False
                            sMotivo = "Tu clase no puede usar este objeto."
                            Exit Function
                        End If
                    Next i
                End If
            End If

            ClasePuedeUsarItem = True


        Catch ex As Exception
            Console.WriteLine("Error in TieneObjetosRobables: " & ex.Message)
            LogError(("Error en ClasePuedeUsarItem"))
        End Try
    End Function

    Sub QuitarNewbieObj(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim j As Short

        Dim DeDonde As WorldPos
        With UserList(UserIndex)
            For j = 1 To UserList(UserIndex).CurrentInventorySlots
                If .Invent.Object_Renamed(j).ObjIndex > 0 Then

                    If ObjData_Renamed(.Invent.Object_Renamed(j).ObjIndex).Newbie = 1 Then _
                        Call QuitarUserInvItem(UserIndex, j, MAX_INVENTORY_OBJS)
                    Call UpdateUserInv(False, UserIndex, j)

                End If
            Next j

            '[Barrin 17-12-03] Si el usuario dejó de ser Newbie, y estaba en el Newbie Dungeon
            'es transportado a su hogar de origen ;)
            If UCase(MapInfo_Renamed(.Pos.Map).Restringir) = "NEWBIE" Then


                Select Case .Hogar
                    Case eCiudad.cLindos 'Vamos a tener que ir por todo el desierto... uff!
                        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto DeDonde. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        DeDonde = Lindos
                    Case eCiudad.cUllathorpe
                        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto DeDonde. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        DeDonde = Ullathorpe
                    Case eCiudad.cBanderbill
                        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto DeDonde. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        DeDonde = Banderbill
                    Case Else
                        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto DeDonde. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        DeDonde = Nix
                End Select

                Call WarpUserChar(UserIndex, DeDonde.Map, DeDonde.X, DeDonde.Y, True)

            End If
            '[/Barrin]
        End With
    End Sub

    Sub LimpiarInventario(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim j As Short

        With UserList(UserIndex)
            For j = 1 To .CurrentInventorySlots
                .Invent.Object_Renamed(j).ObjIndex = 0
                .Invent.Object_Renamed(j).Amount = 0
                .Invent.Object_Renamed(j).Equipped = 0
            Next j

            .Invent.NroItems = 0

            .Invent.ArmourEqpObjIndex = 0
            .Invent.ArmourEqpSlot = 0

            .Invent.WeaponEqpObjIndex = 0
            .Invent.WeaponEqpSlot = 0

            .Invent.CascoEqpObjIndex = 0
            .Invent.CascoEqpSlot = 0

            .Invent.EscudoEqpObjIndex = 0
            .Invent.EscudoEqpSlot = 0

            .Invent.AnilloEqpObjIndex = 0
            .Invent.AnilloEqpSlot = 0

            .Invent.MunicionEqpObjIndex = 0
            .Invent.MunicionEqpSlot = 0

            .Invent.BarcoObjIndex = 0
            .Invent.BarcoSlot = 0

            .Invent.MochilaEqpObjIndex = 0
            .Invent.MochilaEqpSlot = 0
        End With
    End Sub

    Sub TirarOro(Cantidad As Integer, UserIndex As Short)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 23/01/2007
        '23/01/2007 -> Pablo (ToxicWaste): Billetera invertida y explotar oro en el agua.
        '***************************************************
        Try

            'If Cantidad > 100000 Then Exit Sub

            Dim i As Byte
            Dim MiObj As Obj
            Dim loops As Short
            Dim j As Short
            Dim k As Short
            Dim M As Short
            Dim Cercanos As String
            Dim TeniaOro As Integer
            Dim AuxPos As WorldPos
            'UPGRADE_NOTE: Extra se actualizó a Extra_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            Dim Extra_Renamed As Integer
            With UserList(UserIndex)
                'SI EL Pjta TIENE ORO LO TIRAMOS
                If (Cantidad > 0) And (Cantidad <= .Stats.GLD) Then
                    'info debug

                    'Seguridad Alkon (guardo el oro tirado si supera los 50k)
                    If Cantidad > 50000 Then
                        M = .Pos.Map
                        For j = .Pos.X - 10 To .Pos.X + 10
                            For k = .Pos.Y - 10 To .Pos.Y + 10
                                If InMapBounds(M, j, k) Then
                                    If MapData(M, j, k).UserIndex > 0 Then
                                        Cercanos = Cercanos & UserList(MapData(M, j, k).UserIndex).name & ","
                                    End If
                                End If
                            Next k
                        Next j
                        Call LogDesarrollo(.name & " tira oro. Cercanos: " & Cercanos)
                    End If
                    '/Seguridad
                    TeniaOro = .Stats.GLD
                    If Cantidad > 500000 Then 'Para evitar explotar demasiado
                        Extra_Renamed = Cantidad - 500000
                        Cantidad = 500000
                    End If

                    Do While (Cantidad > 0)

                        If Cantidad > MAX_INVENTORY_OBJS And .Stats.GLD > MAX_INVENTORY_OBJS Then
                            MiObj.Amount = MAX_INVENTORY_OBJS
                            Cantidad = Cantidad - MiObj.Amount
                        Else
                            MiObj.Amount = Cantidad
                            Cantidad = Cantidad - MiObj.Amount
                        End If

                        MiObj.ObjIndex = iORO

                        If EsGM(UserIndex) Then _
                            Call _
                                LogGM(.name,
                                      "Tiró cantidad:" & MiObj.Amount & " Objeto:" &
                                      ObjData_Renamed(MiObj.ObjIndex).name)

                        If .clase = eClass.Pirat And .Invent.BarcoObjIndex = 476 Then
                            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto AuxPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            AuxPos = TirarItemAlPiso(.Pos, MiObj, False)
                            If AuxPos.X <> 0 And AuxPos.Y <> 0 Then
                                .Stats.GLD = .Stats.GLD - MiObj.Amount
                            End If
                        Else
                            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto AuxPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            AuxPos = TirarItemAlPiso(.Pos, MiObj, True)
                            If AuxPos.X <> 0 And AuxPos.Y <> 0 Then
                                .Stats.GLD = .Stats.GLD - MiObj.Amount
                            End If
                        End If

                        'info debug
                        loops = loops + 1
                        If loops > 100 Then
                            LogError(("Error en tiraroro"))
                            Exit Sub
                        End If

                    Loop
                    If TeniaOro = .Stats.GLD Then Extra_Renamed = 0
                    If Extra_Renamed > 0 Then
                        .Stats.GLD = .Stats.GLD - Extra_Renamed
                    End If

                End If
            End With


        Catch ex As Exception
            Console.WriteLine("Error in QuitarNewbieObj: " & ex.Message)
            Call LogError("Error en TirarOro. Error " & Err.Number & " : " & Err.Description)
        End Try
    End Sub

    Sub QuitarUserInvItem(UserIndex As Short, Slot As Byte, Cantidad As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

            If Slot < 1 Or Slot > UserList(UserIndex).CurrentInventorySlots Then Exit Sub

            With UserList(UserIndex).Invent.Object_Renamed(Slot)
                If .Amount <= Cantidad And .Equipped = 1 Then
                    Call Desequipar(UserIndex, Slot)
                End If

                'Quita un objeto
                .Amount = .Amount - Cantidad
                '¿Quedan mas?
                If .Amount <= 0 Then
                    UserList(UserIndex).Invent.NroItems = UserList(UserIndex).Invent.NroItems - 1
                    .ObjIndex = 0
                    .Amount = 0
                End If
            End With


        Catch ex As Exception
            Console.WriteLine("Error in QuitarUserInvItem: " & ex.Message)
            Call LogError("Error en QuitarUserInvItem. Error " & Err.Number & " : " & Err.Description)
        End Try
    End Sub

    Sub UpdateUserInv(UpdateAll As Boolean, UserIndex As Short, Slot As Byte)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

            Dim NullObj As UserOBJ
            Dim LoopC As Integer

            With UserList(UserIndex)
                'Actualiza un solo slot
                If Not UpdateAll Then

                    'Actualiza el inventario
                    If .Invent.Object_Renamed(Slot).ObjIndex > 0 Then
                        Call ChangeUserInv(UserIndex, Slot, .Invent.Object_Renamed(Slot))
                    Else
                        Call ChangeUserInv(UserIndex, Slot, NullObj)
                    End If

                Else

                    'Actualiza todos los slots
                    For LoopC = 1 To .CurrentInventorySlots
                        'Actualiza el inventario
                        If .Invent.Object_Renamed(LoopC).ObjIndex > 0 Then
                            Call ChangeUserInv(UserIndex, LoopC, .Invent.Object_Renamed(LoopC))
                        Else
                            Call ChangeUserInv(UserIndex, LoopC, NullObj)
                        End If
                    Next LoopC
                End If

                Exit Sub
            End With


        Catch ex As Exception
            Console.WriteLine("Error in UpdateUserInv: " & ex.Message)
            Call LogError("Error en UpdateUserInv. Error " & Err.Number & " : " & Err.Description)
        End Try
    End Sub

    Sub DropObj(UserIndex As Short, Slot As Byte, num As Short, Map As Short, X As Short,
                Y As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'UPGRADE_NOTE: Obj se actualizó a Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim Obj_Renamed As Obj

        With UserList(UserIndex)
            If num > 0 Then

                If num > .Invent.Object_Renamed(Slot).Amount Then num = .Invent.Object_Renamed(Slot).Amount

                Obj_Renamed.ObjIndex = .Invent.Object_Renamed(Slot).ObjIndex
                Obj_Renamed.Amount = num

                If (ItemNewbie(Obj_Renamed.ObjIndex) And (.flags.Privilegios And PlayerType.User)) Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes tirar objetos newbie.",
                                        FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If

                'Check objeto en el suelo
                If _
                    MapData(.Pos.Map, X, Y).ObjInfo.ObjIndex = 0 Or
                    MapData(.Pos.Map, X, Y).ObjInfo.ObjIndex = Obj_Renamed.ObjIndex Then
                    If num + MapData(.Pos.Map, X, Y).ObjInfo.Amount > MAX_INVENTORY_OBJS Then
                        num = MAX_INVENTORY_OBJS - MapData(.Pos.Map, X, Y).ObjInfo.Amount
                    End If

                    Call MakeObj(Obj_Renamed, Map, X, Y)
                    Call QuitarUserInvItem(UserIndex, Slot, num)
                    Call UpdateUserInv(False, UserIndex, Slot)

                    If ObjData_Renamed(Obj_Renamed.ObjIndex).OBJType = eOBJType.otBarcos Then
                        Call _
                            WriteConsoleMsg(UserIndex, "¡¡ATENCIÓN!! ¡ACABAS DE TIRAR TU BARCA!",
                                            FontTypeNames.FONTTYPE_TALK)
                    End If

                    If Not .flags.Privilegios And PlayerType.User Then _
                        Call _
                            LogGM(.name,
                                  "Tiró cantidad:" & num & " Objeto:" & ObjData_Renamed(Obj_Renamed.ObjIndex).name)

                    'Log de Objetos que se tiran al piso. Pablo (ToxicWaste) 07/09/07
                    'Es un Objeto que tenemos que loguear?
                    If ObjData_Renamed(Obj_Renamed.ObjIndex).Log = 1 Then
                        Call _
                            LogDesarrollo(
                                .name & " tiró al piso " & Obj_Renamed.Amount & " " &
                                ObjData_Renamed(Obj_Renamed.ObjIndex).name & " Mapa: " & Map & " X: " & X & " Y: " & Y)
                    ElseIf Obj_Renamed.Amount > 5000 Then _
'Es mucha cantidad? > Subí a 5000 el minimo porque si no se llenaba el log de cosas al pedo. (NicoNZ)
                        'Si no es de los prohibidos de loguear, lo logueamos.
                        If ObjData_Renamed(Obj_Renamed.ObjIndex).NoLog <> 1 Then
                            Call _
                                LogDesarrollo(
                                    .name & " tiró al piso " & Obj_Renamed.Amount & " " &
                                    ObjData_Renamed(Obj_Renamed.ObjIndex).name & " Mapa: " & Map & " X: " & X & " Y: " &
                                    Y)
                        End If
                    End If
                Else
                    Call WriteConsoleMsg(UserIndex, "No hay espacio en el piso.", FontTypeNames.FONTTYPE_INFO)
                End If
            End If
        End With
    End Sub

    Sub EraseObj(num As Short, Map As Short, X As Short, Y As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        With MapData(Map, X, Y)
            .ObjInfo.Amount = .ObjInfo.Amount - num

            If .ObjInfo.Amount <= 0 Then
                .ObjInfo.ObjIndex = 0
                .ObjInfo.Amount = 0

                Call SendToAreaByPos(Map, X, Y, PrepareMessageObjectDelete(X, Y))
            End If
        End With
    End Sub

    Sub MakeObj(ByRef Obj As Obj, Map As Short, X As Short, Y As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        If Obj.ObjIndex > 0 And Obj.ObjIndex <= UBound(ObjData_Renamed) Then

            With MapData(Map, X, Y)
                If .ObjInfo.ObjIndex = Obj.ObjIndex Then
                    .ObjInfo.Amount = .ObjInfo.Amount + Obj.Amount
                Else
                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto MapData().ObjInfo. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    .ObjInfo = Obj

                    Call _
                        SendToAreaByPos(Map, X, Y,
                                        PrepareMessageObjectCreate(ObjData_Renamed(Obj.ObjIndex).GrhIndex, X,
                                                                   Y))
                End If
            End With
        End If
    End Sub

    Function MeterItemEnInventario(UserIndex As Short, ByRef MiObj As Obj) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

            Dim X As Short
            Dim Y As Short
            Dim Slot As Byte

            With UserList(UserIndex)
                '¿el user ya tiene un objeto del mismo tipo?
                Slot = 1
                Do _
                    Until _
                        .Invent.Object_Renamed(Slot).ObjIndex = MiObj.ObjIndex And
                        .Invent.Object_Renamed(Slot).Amount + MiObj.Amount <= MAX_INVENTORY_OBJS
                    Slot = Slot + 1
                    If Slot > .CurrentInventorySlots Then
                        Exit Do
                    End If
                Loop

                'Sino busca un slot vacio
                If Slot > .CurrentInventorySlots Then
                    Slot = 1
                    Do Until .Invent.Object_Renamed(Slot).ObjIndex = 0
                        Slot = Slot + 1
                        If Slot > .CurrentInventorySlots Then
                            Call _
                                WriteConsoleMsg(UserIndex, "No puedes cargar más objetos.",
                                                FontTypeNames.FONTTYPE_FIGHT)
                            MeterItemEnInventario = False
                            Exit Function
                        End If
                    Loop
                    .Invent.NroItems = .Invent.NroItems + 1
                End If

                If Slot > MAX_NORMAL_INVENTORY_SLOTS And Slot < MAX_INVENTORY_SLOTS Then
                    If Not ItemSeCae(MiObj.ObjIndex) Then
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "No puedes contener objetos especiales en tu " &
                                            ObjData_Renamed(.Invent.MochilaEqpObjIndex).name & ".",
                                            FontTypeNames.FONTTYPE_FIGHT)
                        MeterItemEnInventario = False
                        Exit Function
                    End If
                End If
                'Mete el objeto
                If .Invent.Object_Renamed(Slot).Amount + MiObj.Amount <= MAX_INVENTORY_OBJS Then
                    'Menor que MAX_INV_OBJS
                    .Invent.Object_Renamed(Slot).ObjIndex = MiObj.ObjIndex
                    .Invent.Object_Renamed(Slot).Amount = .Invent.Object_Renamed(Slot).Amount + MiObj.Amount
                Else
                    .Invent.Object_Renamed(Slot).Amount = MAX_INVENTORY_OBJS
                End If
            End With

            MeterItemEnInventario = True

            Call UpdateUserInv(False, UserIndex, Slot)


        Catch ex As Exception
            Console.WriteLine("Error in DropObj: " & ex.Message)
            Call LogError("Error en MeterItemEnInventario. Error " & Err.Number & " : " & Err.Description)
        End Try
    End Function

    Sub GetObj(UserIndex As Short)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 18/12/2009
        '18/12/2009: ZaMa - Oro directo a la billetera.
        '***************************************************

        'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        'UPGRADE_NOTE: Obj se actualizó a Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim Obj_Renamed As ObjData
        Dim MiObj As Obj
        Dim ObjPos As String

        Dim X As Short
        Dim Y As Short
        Dim Slot As Byte
        With UserList(UserIndex)
            '¿Hay algun obj?
            If MapData(.Pos.Map, .Pos.X, .Pos.Y).ObjInfo.ObjIndex > 0 Then
                '¿Esta permitido agarrar este obj?
                If ObjData_Renamed(MapData(.Pos.Map, .Pos.X, .Pos.Y).ObjInfo.ObjIndex).Agarrable <> 1 Then

                    X = .Pos.X
                    Y = .Pos.Y

                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Obj_Renamed = ObjData_Renamed(MapData(.Pos.Map, .Pos.X, .Pos.Y).ObjInfo.ObjIndex)
                    MiObj.Amount = MapData(.Pos.Map, X, Y).ObjInfo.Amount
                    MiObj.ObjIndex = MapData(.Pos.Map, X, Y).ObjInfo.ObjIndex

                    ' Oro directo a la billetera!
                    If Obj_Renamed.OBJType = eOBJType.otGuita Then
                        .Stats.GLD = .Stats.GLD + MiObj.Amount
                        'Quitamos el objeto
                        Call EraseObj(MapData(.Pos.Map, X, Y).ObjInfo.Amount, .Pos.Map, .Pos.X, .Pos.Y)

                        Call WriteUpdateGold(UserIndex)
                    Else
                        If MeterItemEnInventario(UserIndex, MiObj) Then

                            'Quitamos el objeto
                            Call EraseObj(MapData(.Pos.Map, X, Y).ObjInfo.Amount, .Pos.Map, .Pos.X, .Pos.Y)
                            If Not .flags.Privilegios And PlayerType.User Then _
                                Call _
                                    LogGM(.name,
                                          "Agarro:" & MiObj.Amount & " Objeto:" & ObjData_Renamed(MiObj.ObjIndex).name)

                            'Log de Objetos que se agarran del piso. Pablo (ToxicWaste) 07/09/07
                            'Es un Objeto que tenemos que loguear?
                            If ObjData_Renamed(MiObj.ObjIndex).Log = 1 Then
                                ObjPos = " Mapa: " & .Pos.Map & " X: " & .Pos.X & " Y: " & .Pos.Y
                                Call _
                                    LogDesarrollo(
                                        .name & " juntó del piso " & MiObj.Amount & " " &
                                        ObjData_Renamed(MiObj.ObjIndex).name & ObjPos)
                            ElseIf MiObj.Amount > MAX_INVENTORY_OBJS - 1000 Then 'Es mucha cantidad?
                                'Si no es de los prohibidos de loguear, lo logueamos.
                                If ObjData_Renamed(MiObj.ObjIndex).NoLog <> 1 Then
                                    ObjPos = " Mapa: " & .Pos.Map & " X: " & .Pos.X & " Y: " & .Pos.Y
                                    Call _
                                        LogDesarrollo(
                                            .name & " juntó del piso " & MiObj.Amount & " " &
                                            ObjData_Renamed(MiObj.ObjIndex).name & ObjPos)
                                End If
                            End If
                        End If
                    End If
                End If
            Else
                Call WriteConsoleMsg(UserIndex, "No hay nada aquí.", FontTypeNames.FONTTYPE_INFO)
            End If
        End With
    End Sub

    Sub Desequipar(UserIndex As Short, Slot As Byte)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

            'Desequipa el item slot del inventario
            'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
            'UPGRADE_NOTE: Obj se actualizó a Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            Dim Obj_Renamed As ObjData

            With UserList(UserIndex)
                With .Invent
                    If (Slot < LBound(.Object_Renamed)) Or (Slot > UBound(.Object_Renamed)) Then
                        Exit Sub
                    ElseIf .Object_Renamed(Slot).ObjIndex = 0 Then
                        Exit Sub
                    End If

                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    Obj_Renamed = ObjData_Renamed(.Object_Renamed(Slot).ObjIndex)
                End With

                Select Case Obj_Renamed.OBJType
                    Case eOBJType.otWeapon
                        With .Invent
                            .Object_Renamed(Slot).Equipped = 0
                            .WeaponEqpObjIndex = 0
                            .WeaponEqpSlot = 0
                        End With

                        If Not .flags.Mimetizado = 1 Then
                            With .Char_Renamed
                                .WeaponAnim = NingunArma
                                Call _
                                    ChangeUserChar(UserIndex, .body, .Head, .heading, .WeaponAnim, .ShieldAnim,
                                                   .CascoAnim)
                            End With
                        End If

                    Case eOBJType.otFlechas
                        With .Invent
                            .Object_Renamed(Slot).Equipped = 0
                            .MunicionEqpObjIndex = 0
                            .MunicionEqpSlot = 0
                        End With

                    Case eOBJType.otAnillo
                        With .Invent
                            .Object_Renamed(Slot).Equipped = 0
                            .AnilloEqpObjIndex = 0
                            .AnilloEqpSlot = 0
                        End With

                    Case eOBJType.otArmadura
                        With .Invent
                            .Object_Renamed(Slot).Equipped = 0
                            .ArmourEqpObjIndex = 0
                            .ArmourEqpSlot = 0
                        End With

                        Call DarCuerpoDesnudo(UserIndex, .flags.Mimetizado = 1)
                        With .Char_Renamed
                            Call ChangeUserChar(UserIndex, .body, .Head, .heading, .WeaponAnim, .ShieldAnim, .CascoAnim)
                        End With

                    Case eOBJType.otCASCO
                        With .Invent
                            .Object_Renamed(Slot).Equipped = 0
                            .CascoEqpObjIndex = 0
                            .CascoEqpSlot = 0
                        End With

                        If Not .flags.Mimetizado = 1 Then
                            With .Char_Renamed
                                .CascoAnim = NingunCasco
                                Call _
                                    ChangeUserChar(UserIndex, .body, .Head, .heading, .WeaponAnim, .ShieldAnim,
                                                   .CascoAnim)
                            End With
                        End If

                    Case eOBJType.otESCUDO
                        With .Invent
                            .Object_Renamed(Slot).Equipped = 0
                            .EscudoEqpObjIndex = 0
                            .EscudoEqpSlot = 0
                        End With

                        If Not .flags.Mimetizado = 1 Then
                            With .Char_Renamed
                                .ShieldAnim = NingunEscudo
                                Call _
                                    ChangeUserChar(UserIndex, .body, .Head, .heading, .WeaponAnim, .ShieldAnim,
                                                   .CascoAnim)
                            End With
                        End If

                    Case eOBJType.otMochilas
                        With .Invent
                            .Object_Renamed(Slot).Equipped = 0
                            .MochilaEqpObjIndex = 0
                            .MochilaEqpSlot = 0
                        End With

                        Call TirarTodosLosItemsEnMochila(UserIndex)
                        .CurrentInventorySlots = MAX_NORMAL_INVENTORY_SLOTS
                End Select
            End With

            Call WriteUpdateUserStats(UserIndex)
            Call UpdateUserInv(False, UserIndex, Slot)


        Catch ex As Exception
            Console.WriteLine("Error in GetObj: " & ex.Message)
            Call LogError("Error en Desquipar. Error " & Err.Number & " : " & Err.Description)
        End Try
    End Sub

    Function SexoPuedeUsarItem(UserIndex As Short, ObjIndex As Short, Optional ByRef sMotivo As String = "") _
        As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: 14/01/2010 (ZaMa)
        '14/01/2010: ZaMa - Agrego el motivo por el que no puede equipar/usar el item.
        '***************************************************

        Try

            If ObjData_Renamed(ObjIndex).Mujer = 1 Then
                SexoPuedeUsarItem = UserList(UserIndex).Genero <> eGenero.Hombre
            ElseIf ObjData_Renamed(ObjIndex).Hombre = 1 Then
                SexoPuedeUsarItem = UserList(UserIndex).Genero <> eGenero.Mujer
            Else
                SexoPuedeUsarItem = True
            End If

            If Not SexoPuedeUsarItem Then sMotivo = "Tu género no puede usar este objeto."


        Catch ex As Exception
            Console.WriteLine("Error in SexoPuedeUsarItem: " & ex.Message)
            Call LogError("SexoPuedeUsarItem")
        End Try
    End Function


    Function FaccionPuedeUsarItem(UserIndex As Short, ObjIndex As Short,
                                  Optional ByRef sMotivo As String = "") As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: 14/01/2010 (ZaMa)
        '14/01/2010: ZaMa - Agrego el motivo por el que no puede equipar/usar el item.
        '***************************************************

        If ObjData_Renamed(ObjIndex).Real = 1 Then
            If Not criminal(UserIndex) Then
                FaccionPuedeUsarItem = esArmada(UserIndex)
            Else
                FaccionPuedeUsarItem = False
            End If
        ElseIf ObjData_Renamed(ObjIndex).Caos = 1 Then
            If criminal(UserIndex) Then
                FaccionPuedeUsarItem = esCaos(UserIndex)
            Else
                FaccionPuedeUsarItem = False
            End If
        Else
            FaccionPuedeUsarItem = True
        End If

        If Not FaccionPuedeUsarItem Then sMotivo = "Tu alineación no puede usar este objeto."
    End Function

    Sub EquiparInvItem(UserIndex As Short, Slot As Byte)
        '*************************************************
        'Author: Unknown
        'Last modified: 14/01/2010 (ZaMa)
        '01/08/2009: ZaMa - Now it's not sent any sound made by an invisible admin
        '14/01/2010: ZaMa - Agrego el motivo especifico por el que no puede equipar/usar el item.
        '*************************************************

        Try

            'Equipa un item del inventario
            'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
            'UPGRADE_NOTE: Obj se actualizó a Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            Dim Obj_Renamed As ObjData
            Dim ObjIndex As Short
            Dim sMotivo As String

            With UserList(UserIndex)
                ObjIndex = .Invent.Object_Renamed(Slot).ObjIndex
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                Obj_Renamed = ObjData_Renamed(ObjIndex)

                If Obj_Renamed.Newbie = 1 And Not EsNewbie(UserIndex) Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Sólo los newbies pueden usar este objeto.",
                                        FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If

                Select Case Obj_Renamed.OBJType
                    Case eOBJType.otWeapon
                        If _
                            ClasePuedeUsarItem(UserIndex, ObjIndex, sMotivo) And
                            FaccionPuedeUsarItem(UserIndex, ObjIndex, sMotivo) Then
                            'Si esta equipado lo quita
                            If .Invent.Object_Renamed(Slot).Equipped Then
                                'Quitamos del inv el item
                                Call Desequipar(UserIndex, Slot)
                                'Animacion por defecto
                                If .flags.Mimetizado = 1 Then
                                    .CharMimetizado.WeaponAnim = NingunArma
                                Else
                                    .Char_Renamed.WeaponAnim = NingunArma
                                    Call _
                                        ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head,
                                                       .Char_Renamed.heading, .Char_Renamed.WeaponAnim,
                                                       .Char_Renamed.ShieldAnim, .Char_Renamed.CascoAnim)
                                End If
                                Exit Sub
                            End If

                            'Quitamos el elemento anterior
                            If .Invent.WeaponEqpObjIndex > 0 Then
                                Call Desequipar(UserIndex, .Invent.WeaponEqpSlot)
                            End If

                            .Invent.Object_Renamed(Slot).Equipped = 1
                            .Invent.WeaponEqpObjIndex = ObjIndex
                            .Invent.WeaponEqpSlot = Slot

                            'El sonido solo se envia si no lo produce un admin invisible
                            If Not (.flags.AdminInvisible = 1) Then _
                                Call _
                                    SendData(SendTarget.ToPCArea, UserIndex,
                                             PrepareMessagePlayWave(SND_SACARARMA, .Pos.X, .Pos.Y))

                            If .flags.Mimetizado = 1 Then
                                .CharMimetizado.WeaponAnim = GetWeaponAnim(UserIndex, ObjIndex)
                            Else
                                .Char_Renamed.WeaponAnim = GetWeaponAnim(UserIndex, ObjIndex)
                                Call _
                                    ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head,
                                                   .Char_Renamed.heading,
                                                   .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim,
                                                   .Char_Renamed.CascoAnim)
                            End If
                        Else
                            Call WriteConsoleMsg(UserIndex, sMotivo, FontTypeNames.FONTTYPE_INFO)
                        End If

                    Case eOBJType.otAnillo
                        If _
                            ClasePuedeUsarItem(UserIndex, ObjIndex, sMotivo) And
                            FaccionPuedeUsarItem(UserIndex, ObjIndex, sMotivo) Then
                            'Si esta equipado lo quita
                            If .Invent.Object_Renamed(Slot).Equipped Then
                                'Quitamos del inv el item
                                Call Desequipar(UserIndex, Slot)
                                Exit Sub
                            End If

                            'Quitamos el elemento anterior
                            If .Invent.AnilloEqpObjIndex > 0 Then
                                Call Desequipar(UserIndex, .Invent.AnilloEqpSlot)
                            End If

                            .Invent.Object_Renamed(Slot).Equipped = 1
                            .Invent.AnilloEqpObjIndex = ObjIndex
                            .Invent.AnilloEqpSlot = Slot

                        Else
                            Call WriteConsoleMsg(UserIndex, sMotivo, FontTypeNames.FONTTYPE_INFO)
                        End If

                    Case eOBJType.otFlechas
                        If _
                            ClasePuedeUsarItem(UserIndex, ObjIndex, sMotivo) And
                            FaccionPuedeUsarItem(UserIndex, ObjIndex, sMotivo) Then

                            'Si esta equipado lo quita
                            If .Invent.Object_Renamed(Slot).Equipped Then
                                'Quitamos del inv el item
                                Call Desequipar(UserIndex, Slot)
                                Exit Sub
                            End If

                            'Quitamos el elemento anterior
                            If .Invent.MunicionEqpObjIndex > 0 Then
                                Call Desequipar(UserIndex, .Invent.MunicionEqpSlot)
                            End If

                            .Invent.Object_Renamed(Slot).Equipped = 1
                            .Invent.MunicionEqpObjIndex = ObjIndex
                            .Invent.MunicionEqpSlot = Slot

                        Else
                            Call WriteConsoleMsg(UserIndex, sMotivo, FontTypeNames.FONTTYPE_INFO)
                        End If

                    Case eOBJType.otArmadura
                        If .flags.Navegando = 1 Then Exit Sub

                        'Nos aseguramos que puede usarla
                        If _
                            ClasePuedeUsarItem(UserIndex, ObjIndex, sMotivo) And
                            SexoPuedeUsarItem(UserIndex, ObjIndex, sMotivo) And
                            CheckRazaUsaRopa(UserIndex, ObjIndex, sMotivo) And
                            FaccionPuedeUsarItem(UserIndex, ObjIndex, sMotivo) Then

                            'Si esta equipado lo quita
                            If .Invent.Object_Renamed(Slot).Equipped Then
                                Call Desequipar(UserIndex, Slot)
                                Call DarCuerpoDesnudo(UserIndex, .flags.Mimetizado = 1)
                                If Not .flags.Mimetizado = 1 Then
                                    Call _
                                        ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head,
                                                       .Char_Renamed.heading, .Char_Renamed.WeaponAnim,
                                                       .Char_Renamed.ShieldAnim, .Char_Renamed.CascoAnim)
                                End If
                                Exit Sub
                            End If

                            'Quita el anterior
                            If .Invent.ArmourEqpObjIndex > 0 Then
                                Call Desequipar(UserIndex, .Invent.ArmourEqpSlot)
                            End If

                            'Lo equipa
                            .Invent.Object_Renamed(Slot).Equipped = 1
                            .Invent.ArmourEqpObjIndex = ObjIndex
                            .Invent.ArmourEqpSlot = Slot

                            If .flags.Mimetizado = 1 Then
                                .CharMimetizado.body = Obj_Renamed.Ropaje
                            Else
                                .Char_Renamed.body = Obj_Renamed.Ropaje
                                Call _
                                    ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head,
                                                   .Char_Renamed.heading,
                                                   .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim,
                                                   .Char_Renamed.CascoAnim)
                            End If
                            .flags.Desnudo = 0
                        Else
                            Call WriteConsoleMsg(UserIndex, sMotivo, FontTypeNames.FONTTYPE_INFO)
                        End If

                    Case eOBJType.otCASCO
                        If .flags.Navegando = 1 Then Exit Sub
                        If ClasePuedeUsarItem(UserIndex, ObjIndex, sMotivo) Then
                            'Si esta equipado lo quita
                            If .Invent.Object_Renamed(Slot).Equipped Then
                                Call Desequipar(UserIndex, Slot)
                                If .flags.Mimetizado = 1 Then
                                    .CharMimetizado.CascoAnim = NingunCasco
                                Else
                                    .Char_Renamed.CascoAnim = NingunCasco
                                    Call _
                                        ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head,
                                                       .Char_Renamed.heading, .Char_Renamed.WeaponAnim,
                                                       .Char_Renamed.ShieldAnim, .Char_Renamed.CascoAnim)
                                End If
                                Exit Sub
                            End If

                            'Quita el anterior
                            If .Invent.CascoEqpObjIndex > 0 Then
                                Call Desequipar(UserIndex, .Invent.CascoEqpSlot)
                            End If

                            'Lo equipa

                            .Invent.Object_Renamed(Slot).Equipped = 1
                            .Invent.CascoEqpObjIndex = ObjIndex
                            .Invent.CascoEqpSlot = Slot
                            If .flags.Mimetizado = 1 Then
                                .CharMimetizado.CascoAnim = Obj_Renamed.CascoAnim
                            Else
                                .Char_Renamed.CascoAnim = Obj_Renamed.CascoAnim
                                Call _
                                    ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head,
                                                   .Char_Renamed.heading,
                                                   .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim,
                                                   .Char_Renamed.CascoAnim)
                            End If
                        Else
                            Call WriteConsoleMsg(UserIndex, sMotivo, FontTypeNames.FONTTYPE_INFO)
                        End If

                    Case eOBJType.otESCUDO
                        If .flags.Navegando = 1 Then Exit Sub

                        If _
                            ClasePuedeUsarItem(UserIndex, ObjIndex, sMotivo) And
                            FaccionPuedeUsarItem(UserIndex, ObjIndex, sMotivo) Then

                            'Si esta equipado lo quita
                            If .Invent.Object_Renamed(Slot).Equipped Then
                                Call Desequipar(UserIndex, Slot)
                                If .flags.Mimetizado = 1 Then
                                    .CharMimetizado.ShieldAnim = NingunEscudo
                                Else
                                    .Char_Renamed.ShieldAnim = NingunEscudo
                                    Call _
                                        ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head,
                                                       .Char_Renamed.heading, .Char_Renamed.WeaponAnim,
                                                       .Char_Renamed.ShieldAnim, .Char_Renamed.CascoAnim)
                                End If
                                Exit Sub
                            End If

                            'Quita el anterior
                            If .Invent.EscudoEqpObjIndex > 0 Then
                                Call Desequipar(UserIndex, .Invent.EscudoEqpSlot)
                            End If

                            'Lo equipa

                            .Invent.Object_Renamed(Slot).Equipped = 1
                            .Invent.EscudoEqpObjIndex = ObjIndex
                            .Invent.EscudoEqpSlot = Slot

                            If .flags.Mimetizado = 1 Then
                                .CharMimetizado.ShieldAnim = Obj_Renamed.ShieldAnim
                            Else
                                .Char_Renamed.ShieldAnim = Obj_Renamed.ShieldAnim

                                Call _
                                    ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head,
                                                   .Char_Renamed.heading,
                                                   .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim,
                                                   .Char_Renamed.CascoAnim)
                            End If
                        Else
                            Call WriteConsoleMsg(UserIndex, sMotivo, FontTypeNames.FONTTYPE_INFO)
                        End If

                    Case eOBJType.otMochilas
                        If .flags.Muerto = 1 Then
                            Call _
                                WriteConsoleMsg(UserIndex, "¡¡Estas muerto!! Solo podes usar items cuando estas vivo. ",
                                                FontTypeNames.FONTTYPE_INFO)
                            Exit Sub
                        End If
                        If .Invent.Object_Renamed(Slot).Equipped Then
                            Call Desequipar(UserIndex, Slot)
                            Exit Sub
                        End If
                        If .Invent.MochilaEqpObjIndex > 0 Then
                            Call Desequipar(UserIndex, .Invent.MochilaEqpSlot)
                        End If
                        .Invent.Object_Renamed(Slot).Equipped = 1
                        .Invent.MochilaEqpObjIndex = ObjIndex
                        .Invent.MochilaEqpSlot = Slot
                        .CurrentInventorySlots = MAX_NORMAL_INVENTORY_SLOTS + Obj_Renamed.MochilaType*5
                        Call WriteAddSlots(UserIndex, Obj_Renamed.MochilaType)
                End Select
            End With

            'Actualiza
            Call UpdateUserInv(False, UserIndex, Slot)


        Catch ex As Exception
            Console.WriteLine("Error in FaccionPuedeUsarItem: " & ex.Message)
            Call _
                LogError(
                    "EquiparInvItem Slot:" & Slot & " - Error: " & Err.Number & " - Error Description : " &
                    Err.Description)
        End Try
    End Sub

    Private Function CheckRazaUsaRopa(UserIndex As Short, ByRef ItemIndex As Short,
                                      Optional ByRef sMotivo As String = "") As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: 14/01/2010 (ZaMa)
        '14/01/2010: ZaMa - Agrego el motivo por el que no puede equipar/usar el item.
        '***************************************************

        Try

            With UserList(UserIndex)
                'Verifica si la raza puede usar la ropa
                If _
                    .raza = eRaza.Humano Or .raza = eRaza.Elfo Or
                    .raza = eRaza.Drow Then
                    CheckRazaUsaRopa = (ObjData_Renamed(ItemIndex).RazaEnana = 0)
                Else
                    CheckRazaUsaRopa = (ObjData_Renamed(ItemIndex).RazaEnana = 1)
                End If

                'Solo se habilita la ropa exclusiva para Drows por ahora. Pablo (ToxicWaste)
                If (.raza <> eRaza.Drow) And ObjData_Renamed(ItemIndex).RazaDrow Then
                    CheckRazaUsaRopa = False
                End If
            End With

            If Not CheckRazaUsaRopa Then sMotivo = "Tu raza no puede usar este objeto."


        Catch ex As Exception
            Console.WriteLine("Error in CheckRazaUsaRopa: " & ex.Message)
            Call LogError("Error CheckRazaUsaRopa ItemIndex:" & ItemIndex)
        End Try
    End Function

    Sub UseInvItem(UserIndex As Short, Slot As Byte)
        '*************************************************
        'Author: Unknown
        'Last modified: 10/12/2009
        'Handels the usage of items from inventory box.
        '24/01/2007 Pablo (ToxicWaste) - Agrego el Cuerno de la Armada y la Legión.
        '24/01/2007 Pablo (ToxicWaste) - Utilización nueva de Barco en lvl 20 por clase Pirata y Pescador.
        '01/08/2009: ZaMa - Now it's not sent any sound made by an invisible admin, except to its own client
        '17/11/2009: ZaMa - Ahora se envia una orientacion de la posicion hacia donde esta el que uso el cuerno.
        '27/11/2009: Budi - Se envia indivualmente cuando se modifica a la Agilidad o la Fuerza del personaje.
        '08/12/2009: ZaMa - Agrego el uso de hacha de madera elfica.
        '10/12/2009: ZaMa - Arreglos y validaciones en todos las herramientas de trabajo.
        '*************************************************

        'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        'UPGRADE_NOTE: Obj se actualizó a Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim Obj_Renamed As ObjData
        Dim ObjIndex As Short
        'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura TargObj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Dim TargObj As ObjData
        Dim MiObj As Obj

        With UserList(UserIndex)

            If .Invent.Object_Renamed(Slot).Amount = 0 Then Exit Sub

            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Obj_Renamed = ObjData_Renamed(.Invent.Object_Renamed(Slot).ObjIndex)

            If Obj_Renamed.Newbie = 1 And Not EsNewbie(UserIndex) Then
                Call _
                    WriteConsoleMsg(UserIndex, "Sólo los newbies pueden usar estos objetos.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If Obj_Renamed.OBJType = eOBJType.otWeapon Then
                If Obj_Renamed.proyectil = 1 Then

                    'valido para evitar el flood pero no bloqueo. El bloqueo se hace en WLC con proyectiles.
                    If Not IntervaloPermiteUsar(UserIndex, False) Then Exit Sub
                Else
                    'dagas
                    If Not IntervaloPermiteUsar(UserIndex) Then Exit Sub
                End If
            Else
                If Not IntervaloPermiteUsar(UserIndex) Then Exit Sub
            End If

            ObjIndex = .Invent.Object_Renamed(Slot).ObjIndex
            .flags.TargetObjInvIndex = ObjIndex
            .flags.TargetObjInvSlot = Slot

            Select Case Obj_Renamed.OBJType
                Case eOBJType.otUseOnce
                    If .flags.Muerto = 1 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If

                    'Usa el item
                    .Stats.MinHam = .Stats.MinHam + Obj_Renamed.MinHam
                    If .Stats.MinHam > .Stats.MaxHam Then .Stats.MinHam = .Stats.MaxHam
                    .flags.Hambre = 0
                    Call WriteUpdateHungerAndThirst(UserIndex)
                    'Sonido

                    If _
                        ObjIndex = e_ObjetosCriticos.Manzana Or
                        ObjIndex = e_ObjetosCriticos.Manzana2 Or
                        ObjIndex = e_ObjetosCriticos.ManzanaNewbie Then
                        Call _
                            SonidosMapas.ReproducirSonido(SendTarget.ToPCArea, UserIndex,
                                                          SoundMapInfo.e_SoundIndex.MORFAR_MANZANA)
                    Else
                        Call _
                            SonidosMapas.ReproducirSonido(SendTarget.ToPCArea, UserIndex,
                                                          SoundMapInfo.e_SoundIndex.SOUND_COMIDA)
                    End If

                    'Quitamos del inv el item
                    Call QuitarUserInvItem(UserIndex, Slot, 1)

                    Call UpdateUserInv(False, UserIndex, Slot)

                Case eOBJType.otGuita
                    If .flags.Muerto = 1 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If

                    .Stats.GLD = .Stats.GLD + .Invent.Object_Renamed(Slot).Amount
                    .Invent.Object_Renamed(Slot).Amount = 0
                    .Invent.Object_Renamed(Slot).ObjIndex = 0
                    .Invent.NroItems = .Invent.NroItems - 1

                    Call UpdateUserInv(False, UserIndex, Slot)
                    Call WriteUpdateGold(UserIndex)

                Case eOBJType.otWeapon
                    If .flags.Muerto = 1 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If

                    If Not .Stats.MinSta > 0 Then
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "Estás muy cansad" & IIf(.Genero = eGenero.Hombre, "o", "a") &
                                            ".", FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If

                    If ObjData_Renamed(ObjIndex).proyectil = 1 Then
                        If .Invent.Object_Renamed(Slot).Equipped = 0 Then
                            Call _
                                WriteConsoleMsg(UserIndex, "Antes de usar la herramienta deberías equipartela.",
                                                FontTypeNames.FONTTYPE_INFO)
                            Exit Sub
                        End If
                        Call _
                            WriteMultiMessage(UserIndex, eMessages.WorkRequestTarget,
                                              eSkill.Proyectiles) _
                        'Call WriteWorkRequestTarget(UserIndex, Proyectiles)
                    ElseIf .flags.TargetObj = Leña Then
                        If .Invent.Object_Renamed(Slot).ObjIndex = DAGA Then
                            If .Invent.Object_Renamed(Slot).Equipped = 0 Then
                                Call _
                                    WriteConsoleMsg(UserIndex, "Antes de usar la herramienta deberías equipartela.",
                                                    FontTypeNames.FONTTYPE_INFO)
                                Exit Sub
                            End If

                            Call _
                                TratarDeHacerFogata(.flags.TargetObjMap, .flags.TargetObjX, .flags.TargetObjY, UserIndex)
                        End If
                    Else

                        Select Case ObjIndex
                            Case CAÑA_PESCA, RED_PESCA
                                If .Invent.WeaponEqpObjIndex = CAÑA_PESCA Or .Invent.WeaponEqpObjIndex = RED_PESCA Then
                                    Call _
                                        WriteMultiMessage(UserIndex, eMessages.WorkRequestTarget,
                                                          eSkill.Pesca) _
                                    'Call WriteWorkRequestTarget(UserIndex, eSkill.Pesca)
                                Else
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Debes tener equipada la herramienta para trabajar.",
                                                        FontTypeNames.FONTTYPE_INFO)
                                End If

                            Case HACHA_LEÑADOR, HACHA_LEÑA_ELFICA
                                If _
                                    .Invent.WeaponEqpObjIndex = HACHA_LEÑADOR Or
                                    .Invent.WeaponEqpObjIndex = HACHA_LEÑA_ELFICA Then
                                    Call _
                                        WriteMultiMessage(UserIndex, eMessages.WorkRequestTarget,
                                                          eSkill.Talar)
                                Else
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Debes tener equipada la herramienta para trabajar.",
                                                        FontTypeNames.FONTTYPE_INFO)
                                End If

                            Case PIQUETE_MINERO
                                If .Invent.WeaponEqpObjIndex = PIQUETE_MINERO Then
                                    Call _
                                        WriteMultiMessage(UserIndex, eMessages.WorkRequestTarget,
                                                          eSkill.Mineria)
                                Else
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Debes tener equipada la herramienta para trabajar.",
                                                        FontTypeNames.FONTTYPE_INFO)
                                End If

                            Case MARTILLO_HERRERO
                                If .Invent.WeaponEqpObjIndex = MARTILLO_HERRERO Then
                                    Call _
                                        WriteMultiMessage(UserIndex, eMessages.WorkRequestTarget,
                                                          eSkill.Herreria)
                                Else
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Debes tener equipada la herramienta para trabajar.",
                                                        FontTypeNames.FONTTYPE_INFO)
                                End If

                            Case SERRUCHO_CARPINTERO
                                If .Invent.WeaponEqpObjIndex = SERRUCHO_CARPINTERO Then
                                    Call EnivarObjConstruibles(UserIndex)
                                    Call WriteShowCarpenterForm(UserIndex)
                                Else
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Debes tener equipada la herramienta para trabajar.",
                                                        FontTypeNames.FONTTYPE_INFO)
                                End If

                            Case Else ' Las herramientas no se pueden fundir
                                If ObjData_Renamed(ObjIndex).SkHerreria > 0 Then
                                    ' Solo objetos que pueda hacer el herrero
                                    Call _
                                        WriteMultiMessage(UserIndex, eMessages.WorkRequestTarget,
                                                          FundirMetal) _
                                    'Call WriteWorkRequestTarget(UserIndex, FundirMetal)
                                End If
                        End Select
                    End If

                Case eOBJType.otPociones
                    If .flags.Muerto = 1 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo. ",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If

                    If Not IntervaloPermiteGolpeUsar(UserIndex, False) Then
                        Call _
                            WriteConsoleMsg(UserIndex, "¡¡Debes esperar unos momentos para tomar otra poción!!",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If

                    .flags.TomoPocion = True
                    .flags.TipoPocion = Obj_Renamed.TipoPocion

                    Select Case .flags.TipoPocion

                        Case 1 'Modif la agilidad
                            .flags.DuracionEfecto = Obj_Renamed.DuracionEfecto

                            'Usa el item
                            .Stats.UserAtributos(eAtributos.Agilidad) =
                                .Stats.UserAtributos(eAtributos.Agilidad) +
                                RandomNumber(Obj_Renamed.MinModificador, Obj_Renamed.MaxModificador)
                            If .Stats.UserAtributos(eAtributos.Agilidad) > MAXATRIBUTOS Then _
                                .Stats.UserAtributos(eAtributos.Agilidad) = MAXATRIBUTOS
                            If _
                                .Stats.UserAtributos(eAtributos.Agilidad) >
                                2*.Stats.UserAtributosBackUP(eAtributos.Agilidad) Then _
                                .Stats.UserAtributos(eAtributos.Agilidad) = 2*
                                                                            .Stats.UserAtributosBackUP(
                                                                                eAtributos.
                                                                                                          Agilidad)

                            'Quitamos del inv el item
                            Call QuitarUserInvItem(UserIndex, Slot, 1)

                            ' Los admin invisibles solo producen sonidos a si mismos
                            If .flags.AdminInvisible = 1 Then
                                Call EnviarDatosASlot(UserIndex, PrepareMessagePlayWave(SND_BEBER, .Pos.X, .Pos.Y))
                            Else
                                Call _
                                    SendData(SendTarget.ToPCArea, UserIndex,
                                             PrepareMessagePlayWave(SND_BEBER, .Pos.X, .Pos.Y))
                            End If
                            Call WriteUpdateDexterity(UserIndex)

                        Case 2 'Modif la fuerza
                            .flags.DuracionEfecto = Obj_Renamed.DuracionEfecto

                            'Usa el item
                            .Stats.UserAtributos(eAtributos.Fuerza) =
                                .Stats.UserAtributos(eAtributos.Fuerza) +
                                RandomNumber(Obj_Renamed.MinModificador, Obj_Renamed.MaxModificador)
                            If .Stats.UserAtributos(eAtributos.Fuerza) > MAXATRIBUTOS Then _
                                .Stats.UserAtributos(eAtributos.Fuerza) = MAXATRIBUTOS
                            If _
                                .Stats.UserAtributos(eAtributos.Fuerza) >
                                2*.Stats.UserAtributosBackUP(eAtributos.Fuerza) Then _
                                .Stats.UserAtributos(eAtributos.Fuerza) = 2*
                                                                          .Stats.UserAtributosBackUP(
                                                                              eAtributos.
                                                                                                        Fuerza)


                            'Quitamos del inv el item
                            Call QuitarUserInvItem(UserIndex, Slot, 1)

                            ' Los admin invisibles solo producen sonidos a si mismos
                            If .flags.AdminInvisible = 1 Then
                                Call EnviarDatosASlot(UserIndex, PrepareMessagePlayWave(SND_BEBER, .Pos.X, .Pos.Y))
                            Else
                                Call _
                                    SendData(SendTarget.ToPCArea, UserIndex,
                                             PrepareMessagePlayWave(SND_BEBER, .Pos.X, .Pos.Y))
                            End If
                            Call WriteUpdateStrenght(UserIndex)

                        Case 3 'Pocion roja, restaura HP
                            'Usa el item
                            .Stats.MinHp = .Stats.MinHp +
                                           RandomNumber(Obj_Renamed.MinModificador, Obj_Renamed.MaxModificador)
                            If .Stats.MinHp > .Stats.MaxHp Then .Stats.MinHp = .Stats.MaxHp

                            'Quitamos del inv el item
                            Call QuitarUserInvItem(UserIndex, Slot, 1)

                            ' Los admin invisibles solo producen sonidos a si mismos
                            If .flags.AdminInvisible = 1 Then
                                Call EnviarDatosASlot(UserIndex, PrepareMessagePlayWave(SND_BEBER, .Pos.X, .Pos.Y))
                            Else
                                Call _
                                    SendData(SendTarget.ToPCArea, UserIndex,
                                             PrepareMessagePlayWave(SND_BEBER, .Pos.X, .Pos.Y))
                            End If

                        Case 4 'Pocion azul, restaura MANA
                            'Usa el item
                            'nuevo calculo para recargar mana
                            .Stats.MinMAN = .Stats.MinMAN + Porcentaje(.Stats.MaxMAN, 4) + .Stats.ELV\2 + 40/.Stats.ELV
                            If .Stats.MinMAN > .Stats.MaxMAN Then .Stats.MinMAN = .Stats.MaxMAN

                            'Quitamos del inv el item
                            Call QuitarUserInvItem(UserIndex, Slot, 1)

                            ' Los admin invisibles solo producen sonidos a si mismos
                            If .flags.AdminInvisible = 1 Then
                                Call EnviarDatosASlot(UserIndex, PrepareMessagePlayWave(SND_BEBER, .Pos.X, .Pos.Y))
                            Else
                                Call _
                                    SendData(SendTarget.ToPCArea, UserIndex,
                                             PrepareMessagePlayWave(SND_BEBER, .Pos.X, .Pos.Y))
                            End If

                        Case 5 ' Pocion violeta
                            If .flags.Envenenado = 1 Then
                                .flags.Envenenado = 0
                                Call _
                                    WriteConsoleMsg(UserIndex, "Te has curado del envenenamiento.",
                                                    FontTypeNames.FONTTYPE_INFO)
                            End If
                            'Quitamos del inv el item
                            Call QuitarUserInvItem(UserIndex, Slot, 1)

                            ' Los admin invisibles solo producen sonidos a si mismos
                            If .flags.AdminInvisible = 1 Then
                                Call EnviarDatosASlot(UserIndex, PrepareMessagePlayWave(SND_BEBER, .Pos.X, .Pos.Y))
                            Else
                                Call _
                                    SendData(SendTarget.ToPCArea, UserIndex,
                                             PrepareMessagePlayWave(SND_BEBER, .Pos.X, .Pos.Y))
                            End If

                        Case 6 ' Pocion Negra
                            If .flags.Privilegios And PlayerType.User Then
                                Call QuitarUserInvItem(UserIndex, Slot, 1)
                                Call UserDie(UserIndex)
                                Call _
                                    WriteConsoleMsg(UserIndex, "Sientes un gran mareo y pierdes el conocimiento.",
                                                    FontTypeNames.FONTTYPE_FIGHT)
                            End If
                    End Select
                    Call WriteUpdateUserStats(UserIndex)
                    Call UpdateUserInv(False, UserIndex, Slot)

                Case eOBJType.otBebidas
                    If .flags.Muerto = 1 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If
                    .Stats.MinAGU = .Stats.MinAGU + Obj_Renamed.MinSed
                    If .Stats.MinAGU > .Stats.MaxAGU Then .Stats.MinAGU = .Stats.MaxAGU
                    .flags.Sed = 0
                    Call WriteUpdateHungerAndThirst(UserIndex)

                    'Quitamos del inv el item
                    Call QuitarUserInvItem(UserIndex, Slot, 1)

                    ' Los admin invisibles solo producen sonidos a si mismos
                    If .flags.AdminInvisible = 1 Then
                        Call EnviarDatosASlot(UserIndex, PrepareMessagePlayWave(SND_BEBER, .Pos.X, .Pos.Y))
                    Else
                        Call _
                            SendData(SendTarget.ToPCArea, UserIndex,
                                     PrepareMessagePlayWave(SND_BEBER, .Pos.X, .Pos.Y))
                    End If

                    Call UpdateUserInv(False, UserIndex, Slot)

                Case eOBJType.otLlaves
                    If .flags.Muerto = 1 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If

                    If .flags.TargetObj = 0 Then Exit Sub
                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto TargObj. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    TargObj = ObjData_Renamed(.flags.TargetObj)
                    '¿El objeto clickeado es una puerta?
                    If TargObj.OBJType = eOBJType.otPuertas Then
                        '¿Esta cerrada?
                        If TargObj.Cerrada = 1 Then
                            '¿Cerrada con llave?
                            If TargObj.Llave > 0 Then
                                If TargObj.clave = Obj_Renamed.clave Then

                                    MapData(.flags.TargetObjMap, .flags.TargetObjX, .flags.TargetObjY).ObjInfo.ObjIndex _
                                        =
                                        ObjData_Renamed(
                                            MapData(.flags.TargetObjMap, .flags.TargetObjX, .flags.TargetObjY).ObjInfo.
                                                           ObjIndex).IndexCerrada
                                    .flags.TargetObj =
                                        MapData(.flags.TargetObjMap, .flags.TargetObjX, .flags.TargetObjY).ObjInfo.
                                            ObjIndex
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Has abierto la puerta.",
                                                        FontTypeNames.FONTTYPE_INFO)
                                    Exit Sub
                                Else
                                    Call _
                                        WriteConsoleMsg(UserIndex, "La llave no sirve.",
                                                        FontTypeNames.FONTTYPE_INFO)
                                    Exit Sub
                                End If
                            Else
                                If TargObj.clave = Obj_Renamed.clave Then
                                    MapData(.flags.TargetObjMap, .flags.TargetObjX, .flags.TargetObjY).ObjInfo.ObjIndex _
                                        =
                                        ObjData_Renamed(
                                            MapData(.flags.TargetObjMap, .flags.TargetObjX, .flags.TargetObjY).ObjInfo.
                                                           ObjIndex).IndexCerradaLlave
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Has cerrado con llave la puerta.",
                                                        FontTypeNames.FONTTYPE_INFO)
                                    .flags.TargetObj =
                                        MapData(.flags.TargetObjMap, .flags.TargetObjX, .flags.TargetObjY).ObjInfo.
                                            ObjIndex
                                    Exit Sub
                                Else
                                    Call _
                                        WriteConsoleMsg(UserIndex, "La llave no sirve.",
                                                        FontTypeNames.FONTTYPE_INFO)
                                    Exit Sub
                                End If
                            End If
                        Else
                            Call WriteConsoleMsg(UserIndex, "No está cerrada.", FontTypeNames.FONTTYPE_INFO)
                            Exit Sub
                        End If
                    End If

                Case eOBJType.otBotellaVacia
                    If .flags.Muerto = 1 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If
                    If Not HayAgua(.Pos.Map, .flags.TargetX, .flags.TargetY) Then
                        Call WriteConsoleMsg(UserIndex, "No hay agua allí.", FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If
                    MiObj.Amount = 1
                    MiObj.ObjIndex = ObjData_Renamed(.Invent.Object_Renamed(Slot).ObjIndex).IndexAbierta
                    Call QuitarUserInvItem(UserIndex, Slot, 1)
                    If Not MeterItemEnInventario(UserIndex, MiObj) Then
                        Call TirarItemAlPiso(.Pos, MiObj)
                    End If

                    Call UpdateUserInv(False, UserIndex, Slot)

                Case eOBJType.otBotellaLlena
                    If .flags.Muerto = 1 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If
                    .Stats.MinAGU = .Stats.MinAGU + Obj_Renamed.MinSed
                    If .Stats.MinAGU > .Stats.MaxAGU Then .Stats.MinAGU = .Stats.MaxAGU
                    .flags.Sed = 0
                    Call WriteUpdateHungerAndThirst(UserIndex)
                    MiObj.Amount = 1
                    MiObj.ObjIndex = ObjData_Renamed(.Invent.Object_Renamed(Slot).ObjIndex).IndexCerrada
                    Call QuitarUserInvItem(UserIndex, Slot, 1)
                    If Not MeterItemEnInventario(UserIndex, MiObj) Then
                        Call TirarItemAlPiso(.Pos, MiObj)
                    End If

                    Call UpdateUserInv(False, UserIndex, Slot)

                Case eOBJType.otPergaminos
                    If .flags.Muerto = 1 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If

                    If .Stats.MaxMAN > 0 Then
                        If .flags.Hambre = 0 And .flags.Sed = 0 Then
                            Call AgregarHechizo(UserIndex, Slot)
                            Call UpdateUserInv(False, UserIndex, Slot)
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex, "Estás demasiado hambriento y sediento.",
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, "No tienes conocimientos de las Artes Arcanas.",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If
                Case eOBJType.otMinerales
                    If .flags.Muerto = 1 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If
                    Call WriteMultiMessage(UserIndex, eMessages.WorkRequestTarget, FundirMetal) _
                    'Call WriteWorkRequestTarget(UserIndex, FundirMetal)

                Case eOBJType.otInstrumentos
                    If .flags.Muerto = 1 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If

                    If Obj_Renamed.Real Then '¿Es el Cuerno Real?
                        If FaccionPuedeUsarItem(UserIndex, ObjIndex) Then
                            If MapInfo_Renamed(.Pos.Map).Pk = False Then
                                Call _
                                    WriteConsoleMsg(UserIndex, "No hay peligro aquí. Es zona segura.",
                                                    FontTypeNames.FONTTYPE_INFO)
                                Exit Sub
                            End If

                            ' Los admin invisibles solo producen sonidos a si mismos
                            If .flags.AdminInvisible = 1 Then
                                Call _
                                    EnviarDatosASlot(UserIndex, PrepareMessagePlayWave(Obj_Renamed.Snd1, .Pos.X, .Pos.Y))
                            Else
                                Call AlertarFaccionarios(UserIndex)
                                Call _
                                    SendData(SendTarget.toMap, .Pos.Map,
                                             PrepareMessagePlayWave(Obj_Renamed.Snd1, .Pos.X, .Pos.Y))
                            End If

                            Exit Sub
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex, "Sólo miembros del ejército real pueden usar este cuerno.",
                                                FontTypeNames.FONTTYPE_INFO)
                            Exit Sub
                        End If
                    ElseIf Obj_Renamed.Caos Then '¿Es el Cuerno Legión?
                        If FaccionPuedeUsarItem(UserIndex, ObjIndex) Then
                            If MapInfo_Renamed(.Pos.Map).Pk = False Then
                                Call _
                                    WriteConsoleMsg(UserIndex, "No hay peligro aquí. Es zona segura.",
                                                    FontTypeNames.FONTTYPE_INFO)
                                Exit Sub
                            End If

                            ' Los admin invisibles solo producen sonidos a si mismos
                            If .flags.AdminInvisible = 1 Then
                                Call _
                                    EnviarDatosASlot(UserIndex, PrepareMessagePlayWave(Obj_Renamed.Snd1, .Pos.X, .Pos.Y))
                            Else
                                Call AlertarFaccionarios(UserIndex)
                                Call _
                                    SendData(SendTarget.toMap, .Pos.Map,
                                             PrepareMessagePlayWave(Obj_Renamed.Snd1, .Pos.X, .Pos.Y))
                            End If

                            Exit Sub
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex, "Sólo miembros de la legión oscura pueden usar este cuerno.",
                                                FontTypeNames.FONTTYPE_INFO)
                            Exit Sub
                        End If
                    End If
                    'Si llega aca es porque es o Laud o Tambor o Flauta
                    ' Los admin invisibles solo producen sonidos a si mismos
                    If .flags.AdminInvisible = 1 Then
                        Call EnviarDatosASlot(UserIndex, PrepareMessagePlayWave(Obj_Renamed.Snd1, .Pos.X, .Pos.Y))
                    Else
                        Call _
                            SendData(SendTarget.ToPCArea, UserIndex,
                                     PrepareMessagePlayWave(Obj_Renamed.Snd1, .Pos.X, .Pos.Y))
                    End If

                Case eOBJType.otBarcos
                    'Verifica si esta aproximado al agua antes de permitirle navegar
                    If .Stats.ELV < 25 Then
                        If .clase <> eClass.Worker And .clase <> eClass.Pirat Then
                            Call _
                                WriteConsoleMsg(UserIndex, "Para recorrer los mares debes ser nivel 25 o superior.",
                                                FontTypeNames.FONTTYPE_INFO)
                            Exit Sub
                        Else
                            If .Stats.ELV < 20 Then
                                Call _
                                    WriteConsoleMsg(UserIndex, "Para recorrer los mares debes ser nivel 20 o superior.",
                                                    FontTypeNames.FONTTYPE_INFO)
                                Exit Sub
                            End If
                        End If
                    End If

                    If _
                        ((LegalPos(.Pos.Map, .Pos.X - 1, .Pos.Y, True, False) Or
                          LegalPos(.Pos.Map, .Pos.X, .Pos.Y - 1, True, False) Or
                          LegalPos(.Pos.Map, .Pos.X + 1, .Pos.Y, True, False) Or
                          LegalPos(.Pos.Map, .Pos.X, .Pos.Y + 1, True, False)) And .flags.Navegando = 0) Or
                        .flags.Navegando = 1 Then
                        Call DoNavega(UserIndex, Obj_Renamed, Slot)
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, "¡Debes aproximarte al agua para usar el barco!",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If

            End Select

        End With
    End Sub

    Sub EnivarArmasConstruibles(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Call WriteBlacksmithWeapons(UserIndex)
    End Sub

    Sub EnivarObjConstruibles(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Call WriteCarpenterObjects(UserIndex)
    End Sub

    Sub EnivarArmadurasConstruibles(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Call WriteBlacksmithArmors(UserIndex)
    End Sub

    Sub TirarTodo(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

            Dim Cantidad As Integer
            With UserList(UserIndex)
                If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = 6 Then Exit Sub

                Call TirarTodosLosItems(UserIndex)

                Cantidad = .Stats.GLD - CInt(.Stats.ELV)*10000

                If Cantidad > 0 Then Call TirarOro(Cantidad, UserIndex)
            End With

        Catch ex As Exception
            Console.WriteLine("Error in ClasePuedeUsarItem: " & ex.Message)
        End Try
    End Sub

    Public Function ItemSeCae(index As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        With ObjData_Renamed(index)
            ItemSeCae = (.Real <> 1 Or .NoSeCae = 0) And (.Caos <> 1 Or .NoSeCae = 0) And
                        .OBJType <> eOBJType.otLlaves And .OBJType <> eOBJType.otBarcos And
                        .NoSeCae = 0
        End With
    End Function

    Sub TirarTodosLosItems(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 12/01/2010 (ZaMa)
        '12/01/2010: ZaMa - Ahora los piratas no explotan items solo si estan entre 20 y 25
        '***************************************************

        Dim i As Byte
        Dim NuevaPos As WorldPos
        Dim MiObj As Obj
        Dim ItemIndex As Short
        Dim DropAgua As Boolean

        With UserList(UserIndex)
            For i = 1 To .CurrentInventorySlots
                ItemIndex = .Invent.Object_Renamed(i).ObjIndex
                If ItemIndex > 0 Then
                    If ItemSeCae(ItemIndex) Then
                        NuevaPos.X = 0
                        NuevaPos.Y = 0

                        'Creo el Obj
                        MiObj.Amount = .Invent.Object_Renamed(i).Amount
                        MiObj.ObjIndex = ItemIndex

                        DropAgua = True
                        ' Es pirata?
                        If .clase = eClass.Pirat Then
                            ' Si tiene galeon equipado
                            If .Invent.BarcoObjIndex = 476 Then
                                ' Limitación por nivel, después dropea normalmente
                                If .Stats.ELV >= 20 And .Stats.ELV <= 25 Then
                                    ' No dropea en agua
                                    DropAgua = False
                                End If
                            End If
                        End If

                        Call Tilelibre(.Pos, NuevaPos, MiObj, DropAgua, True)

                        If NuevaPos.X <> 0 And NuevaPos.Y <> 0 Then
                            Call DropObj(UserIndex, i, MAX_INVENTORY_OBJS, NuevaPos.Map, NuevaPos.X, NuevaPos.Y)
                        End If
                    End If
                End If
            Next i
        End With
    End Sub

    Function ItemNewbie(ItemIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        If ItemIndex < 1 Or ItemIndex > UBound(ObjData_Renamed) Then Exit Function

        ItemNewbie = ObjData_Renamed(ItemIndex).Newbie = 1
    End Function

    Sub TirarTodosLosItemsNoNewbies(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 23/11/2009
        '07/11/09: Pato - Fix bug #2819911
        '23/11/2009: ZaMa - Optimizacion de codigo.
        '***************************************************
        Dim i As Byte
        Dim NuevaPos As WorldPos
        Dim MiObj As Obj
        Dim ItemIndex As Short

        With UserList(UserIndex)
            If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = 6 Then Exit Sub

            For i = 1 To UserList(UserIndex).CurrentInventorySlots
                ItemIndex = .Invent.Object_Renamed(i).ObjIndex
                If ItemIndex > 0 Then
                    If ItemSeCae(ItemIndex) And Not ItemNewbie(ItemIndex) Then
                        NuevaPos.X = 0
                        NuevaPos.Y = 0

                        'Creo MiObj
                        MiObj.Amount = .Invent.Object_Renamed(i).Amount
                        MiObj.ObjIndex = ItemIndex
                        'Pablo (ToxicWaste) 24/01/2007
                        'Tira los Items no newbies en todos lados.
                        Tilelibre(.Pos, NuevaPos, MiObj, True, True)
                        If NuevaPos.X <> 0 And NuevaPos.Y <> 0 Then
                            Call DropObj(UserIndex, i, MAX_INVENTORY_OBJS, NuevaPos.Map, NuevaPos.X, NuevaPos.Y)
                        End If
                    End If
                End If
            Next i
        End With
    End Sub

    Sub TirarTodosLosItemsEnMochila(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 12/01/09 (Budi)
        '***************************************************
        Dim i As Byte
        Dim NuevaPos As WorldPos
        Dim MiObj As Obj
        Dim ItemIndex As Short

        With UserList(UserIndex)
            If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = 6 Then Exit Sub

            For i = MAX_NORMAL_INVENTORY_SLOTS + 1 To .CurrentInventorySlots
                ItemIndex = .Invent.Object_Renamed(i).ObjIndex
                If ItemIndex > 0 Then
                    If ItemSeCae(ItemIndex) Then
                        NuevaPos.X = 0
                        NuevaPos.Y = 0

                        'Creo MiObj
                        MiObj.Amount = .Invent.Object_Renamed(i).Amount
                        MiObj.ObjIndex = ItemIndex
                        Tilelibre(.Pos, NuevaPos, MiObj, True, True)
                        If NuevaPos.X <> 0 And NuevaPos.Y <> 0 Then
                            Call DropObj(UserIndex, i, MAX_INVENTORY_OBJS, NuevaPos.Map, NuevaPos.X, NuevaPos.Y)
                        End If
                    End If
                End If
            Next i
        End With
    End Sub

    Public Function getObjType(ObjIndex As Short) As eOBJType
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        If ObjIndex > 0 Then
            getObjType = ObjData_Renamed(ObjIndex).OBJType
        End If
    End Function
End Module
