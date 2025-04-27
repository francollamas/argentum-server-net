Option Strict Off
Option Explicit On
Module modBanco
    Sub IniciarDeposito(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

            'Hacemos un Update del inventario del usuario
            Call UpdateBanUserInv(True, UserIndex, 0)
            'Actualizamos el dinero
            Call WriteUpdateUserStats(UserIndex)
            'Mostramos la ventana pa' comerciar y ver ladear la osamenta. jajaja
            Call WriteBankInit(UserIndex)
            UserList(UserIndex).flags.Comerciando = True


        Catch ex As Exception
            Console.WriteLine("Error in IniciarDeposito: " & ex.Message)

        End Try
    End Sub

    'UPGRADE_NOTE: Object se actualizó a Object_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Sub SendBanObj(ByRef UserIndex As Short, ByRef Slot As Byte, ByRef Object_Renamed As UserOBJ)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().BancoInvent.Object(Slot). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        UserList(UserIndex).BancoInvent.Object_Renamed(Slot) = Object_Renamed

        Call WriteChangeBankSlot(UserIndex, Slot)
    End Sub

    Sub UpdateBanUserInv(UpdateAll As Boolean, UserIndex As Short, Slot As Byte)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim NullObj As UserOBJ
        Dim LoopC As Byte

        With UserList(UserIndex)
            'Actualiza un solo slot
            If Not UpdateAll Then
                'Actualiza el inventario
                If .BancoInvent.Object_Renamed(Slot).ObjIndex > 0 Then
                    Call SendBanObj(UserIndex, Slot, .BancoInvent.Object_Renamed(Slot))
                Else
                    Call SendBanObj(UserIndex, Slot, NullObj)
                End If
            Else
                'Actualiza todos los slots
                For LoopC = 1 To MAX_BANCOINVENTORY_SLOTS
                    'Actualiza el inventario
                    If .BancoInvent.Object_Renamed(LoopC).ObjIndex > 0 Then
                        Call SendBanObj(UserIndex, LoopC, .BancoInvent.Object_Renamed(LoopC))
                    Else
                        Call SendBanObj(UserIndex, LoopC, NullObj)
                    End If
                Next LoopC
            End If
        End With
    End Sub

    Sub UserRetiraItem(UserIndex As Short, i As Short, Cantidad As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try


            If Cantidad < 1 Then Exit Sub

            Call WriteUpdateUserStats(UserIndex)

            If UserList(UserIndex).BancoInvent.Object_Renamed(i).Amount > 0 Then
                If Cantidad > UserList(UserIndex).BancoInvent.Object_Renamed(i).Amount Then _
                    Cantidad = UserList(UserIndex).BancoInvent.Object_Renamed(i).Amount
                'Agregamos el obj que compro al inventario
                Call UserReciveObj(UserIndex, CShort(i), Cantidad)
                'Actualizamos el inventario del usuario
                Call UpdateUserInv(True, UserIndex, 0)
                'Actualizamos el banco
                Call UpdateBanUserInv(True, UserIndex, 0)
            End If

            'Actualizamos la ventana de comercio
            Call UpdateVentanaBanco(UserIndex)


        Catch ex As Exception
            Console.WriteLine("Error in SendBanObj: " & ex.Message)

        End Try
    End Sub

    Sub UserReciveObj(UserIndex As Short, ObjIndex As Short, Cantidad As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim Slot As Short
        Dim obji As Short

        With UserList(UserIndex)
            If .BancoInvent.Object_Renamed(ObjIndex).Amount <= 0 Then Exit Sub

            obji = .BancoInvent.Object_Renamed(ObjIndex).ObjIndex


            '¿Ya tiene un objeto de este tipo?
            Slot = 1
            Do _
                Until _
                    .Invent.Object_Renamed(Slot).ObjIndex = obji And
                    .Invent.Object_Renamed(Slot).Amount + Cantidad <= MAX_INVENTORY_OBJS

                Slot = Slot + 1
                If Slot > .CurrentInventorySlots Then
                    Exit Do
                End If
            Loop

            'Sino se fija por un slot vacio
            If Slot > .CurrentInventorySlots Then
                Slot = 1
                Do Until .Invent.Object_Renamed(Slot).ObjIndex = 0
                    Slot = Slot + 1

                    If Slot > .CurrentInventorySlots Then
                        Call _
                            WriteConsoleMsg(UserIndex, "No podés tener mas objetos.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If
                Loop
                .Invent.NroItems = .Invent.NroItems + 1
            End If


            'Mete el obj en el slot
            If .Invent.Object_Renamed(Slot).Amount + Cantidad <= MAX_INVENTORY_OBJS Then
                'Menor que MAX_INV_OBJS
                .Invent.Object_Renamed(Slot).ObjIndex = obji
                .Invent.Object_Renamed(Slot).Amount = .Invent.Object_Renamed(Slot).Amount + Cantidad

                Call QuitarBancoInvItem(UserIndex, CByte(ObjIndex), Cantidad)
            Else
                Call WriteConsoleMsg(UserIndex, "No podés tener mas objetos.", FontTypeNames.FONTTYPE_INFO)
            End If
        End With
    End Sub

    Sub QuitarBancoInvItem(UserIndex As Short, Slot As Byte, Cantidad As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim ObjIndex As Short

        With UserList(UserIndex)
            ObjIndex = .BancoInvent.Object_Renamed(Slot).ObjIndex

            'Quita un Obj

            .BancoInvent.Object_Renamed(Slot).Amount = .BancoInvent.Object_Renamed(Slot).Amount - Cantidad

            If .BancoInvent.Object_Renamed(Slot).Amount <= 0 Then
                .BancoInvent.NroItems = .BancoInvent.NroItems - 1
                .BancoInvent.Object_Renamed(Slot).ObjIndex = 0
                .BancoInvent.Object_Renamed(Slot).Amount = 0
            End If
        End With
    End Sub

    Sub UpdateVentanaBanco(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Call WriteBankOK(UserIndex)
    End Sub

    Sub UserDepositaItem(UserIndex As Short, Item As Short, Cantidad As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try
            If UserList(UserIndex).Invent.Object_Renamed(Item).Amount > 0 And Cantidad > 0 Then
                If Cantidad > UserList(UserIndex).Invent.Object_Renamed(Item).Amount Then _
                    Cantidad = UserList(UserIndex).Invent.Object_Renamed(Item).Amount

                'Agregamos el obj que deposita al banco
                Call UserDejaObj(UserIndex, CShort(Item), Cantidad)

                'Actualizamos el inventario del usuario
                Call UpdateUserInv(True, UserIndex, 0)

                'Actualizamos el inventario del banco
                Call UpdateBanUserInv(True, UserIndex, 0)
            End If

            'Actualizamos la ventana del banco
            Call UpdateVentanaBanco(UserIndex)

        Catch ex As Exception
            Console.WriteLine("Error in UserReciveObj: " & ex.Message)

        End Try
    End Sub

    Sub UserDejaObj(UserIndex As Short, ObjIndex As Short, Cantidad As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim Slot As Short
        Dim obji As Short

        If Cantidad < 1 Then Exit Sub

        With UserList(UserIndex)
            obji = .Invent.Object_Renamed(ObjIndex).ObjIndex

            '¿Ya tiene un objeto de este tipo?
            Slot = 1
            Do _
                Until _
                    .BancoInvent.Object_Renamed(Slot).ObjIndex = obji And
                    .BancoInvent.Object_Renamed(Slot).Amount + Cantidad <= MAX_INVENTORY_OBJS
                Slot = Slot + 1

                If Slot > MAX_BANCOINVENTORY_SLOTS Then
                    Exit Do
                End If
            Loop

            'Sino se fija por un slot vacio antes del slot devuelto
            If Slot > MAX_BANCOINVENTORY_SLOTS Then
                Slot = 1
                Do Until .BancoInvent.Object_Renamed(Slot).ObjIndex = 0
                    Slot = Slot + 1

                    If Slot > MAX_BANCOINVENTORY_SLOTS Then
                        Call _
                            WriteConsoleMsg(UserIndex, "No tienes mas espacio en el banco!!",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If
                Loop

                .BancoInvent.NroItems = .BancoInvent.NroItems + 1
            End If

            If Slot <= MAX_BANCOINVENTORY_SLOTS Then 'Slot valido
                'Mete el obj en el slot
                If .BancoInvent.Object_Renamed(Slot).Amount + Cantidad <= MAX_INVENTORY_OBJS Then

                    'Menor que MAX_INV_OBJS
                    .BancoInvent.Object_Renamed(Slot).ObjIndex = obji
                    .BancoInvent.Object_Renamed(Slot).Amount = .BancoInvent.Object_Renamed(Slot).Amount + Cantidad

                    Call QuitarUserInvItem(UserIndex, CByte(ObjIndex), Cantidad)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "El banco no puede cargar tantos objetos.",
                                        FontTypeNames.FONTTYPE_INFO)
                End If
            End If
        End With
    End Sub

    Sub SendUserBovedaTxt(sendIndex As Short, UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try
            Dim j As Short

            Call WriteConsoleMsg(sendIndex, UserList(UserIndex).name, FontTypeNames.FONTTYPE_INFO)
            Call _
                WriteConsoleMsg(sendIndex, "Tiene " & UserList(UserIndex).BancoInvent.NroItems & " objetos.",
                                FontTypeNames.FONTTYPE_INFO)

            For j = 1 To MAX_BANCOINVENTORY_SLOTS
                If UserList(UserIndex).BancoInvent.Object_Renamed(j).ObjIndex > 0 Then
                    Call _
                        WriteConsoleMsg(sendIndex,
                                        "Objeto " & j & " " &
                                        ObjData_Renamed(UserList(UserIndex).BancoInvent.Object_Renamed(j).ObjIndex).name &
                                        " Cantidad:" & UserList(UserIndex).BancoInvent.Object_Renamed(j).Amount,
                                        FontTypeNames.FONTTYPE_INFO)
                End If
            Next

        Catch ex As Exception
            Console.WriteLine("Error in IniciarDeposito: " & ex.Message)
        End Try
    End Sub

    Sub SendUserBovedaTxtFromChar(sendIndex As Short, charName As String)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try
            Dim j As Short
            Dim CharFile, Tmp As String
            Dim ObjInd, ObjCant As Integer

            CharFile = CharPath & charName & ".chr"

            If FileExist(CharFile) Then
                Call WriteConsoleMsg(sendIndex, charName, FontTypeNames.FONTTYPE_INFO)
                Call _
                    WriteConsoleMsg(sendIndex,
                                    "Tiene " & GetVar(CharFile, "BancoInventory", "CantidadItems") & " objetos.",
                                    FontTypeNames.FONTTYPE_INFO)
                For j = 1 To MAX_BANCOINVENTORY_SLOTS
                    Tmp = GetVar(CharFile, "BancoInventory", "Obj" & j)
                    ObjInd = CInt(ReadField(1, Tmp, Asc("-")))
                    ObjCant = CInt(ReadField(2, Tmp, Asc("-")))
                    If ObjInd > 0 Then
                        Call _
                            WriteConsoleMsg(sendIndex,
                                            "Objeto " & j & " " & ObjData_Renamed(ObjInd).name & " Cantidad:" & ObjCant,
                                            FontTypeNames.FONTTYPE_INFO)
                    End If
                Next
            Else
                Call WriteConsoleMsg(sendIndex, "Usuario inexistente: " & charName, FontTypeNames.FONTTYPE_INFO)
            End If

        Catch ex As Exception
            Console.WriteLine("Error in SendUserBovedaTxtFromChar: " & ex.Message)
        End Try
    End Sub
End Module
