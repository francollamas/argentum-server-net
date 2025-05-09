Option Strict Off
Option Explicit On
Module InvNpc
    'Modulo Inv & Obj
    'Modulo para controlar los objetos y los inventarios.
    Public Function TirarItemAlPiso(ByRef Pos As WorldPos, ByRef Obj As Obj, Optional ByRef NotPirata As Boolean = True) _
        As WorldPos
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

            Dim NuevaPos As WorldPos
            NuevaPos.X = 0
            NuevaPos.Y = 0

            Tilelibre(Pos, NuevaPos, Obj, NotPirata, True)
            If NuevaPos.X <> 0 And NuevaPos.Y <> 0 Then
                Call MakeObj(Obj, Pos.Map, NuevaPos.X, NuevaPos.Y)
            End If
            'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto TirarItemAlPiso. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            TirarItemAlPiso = NuevaPos


        Catch ex As Exception
            Console.WriteLine("Error in TirarItemAlPiso: " & ex.Message)

        End Try
    End Function

    Public Sub NPC_TIRAR_ITEMS(ByRef npc As npc, IsPretoriano As Boolean)
        '***************************************************
        'Autor: Unknown (orginal version)
        'Last Modification: 28/11/2009
        'Give away npc's items.
        '28/11/2009: ZaMa - Implementado drops complejos
        '02/04/2010: ZaMa - Los pretos vuelven a tirar oro.
        '***************************************************
        Try

            Dim i As Byte
            Dim MiObj As Obj
            Dim NroDrop As Short
            Dim Random As Short
            Dim ObjIndex As Short
            With npc


                ' Tira todo el inventario
                If IsPretoriano Then
                    For i = 1 To MAX_INVENTORY_SLOTS
                        If .Invent.Object_Renamed(i).ObjIndex > 0 Then
                            MiObj.Amount = .Invent.Object_Renamed(i).Amount
                            MiObj.ObjIndex = .Invent.Object_Renamed(i).ObjIndex
                            Call TirarItemAlPiso(.Pos, MiObj)
                        End If
                    Next i

                    ' Dropea oro?
                    If .GiveGLD > 0 Then Call TirarOroNpc(.GiveGLD, .Pos)

                    Exit Sub
                End If

                Random = RandomNumber(1, 100)

                ' Tiene 10% de prob de no tirar nada
                If Random <= 90 Then
                    NroDrop = 1

                    If Random <= 10 Then
                        NroDrop = NroDrop + 1

                        For i = 1 To 3
                            ' 10% de ir pasando de etapas
                            If RandomNumber(1, 100) <= 10 Then
                                NroDrop = NroDrop + 1
                            Else
                                Exit For
                            End If
                        Next i

                    End If


                    ObjIndex = .Drop(NroDrop).ObjIndex
                    If ObjIndex > 0 Then

                        If ObjIndex = iORO Then
                            Call TirarOroNpc(.Drop(NroDrop).Amount, npc.Pos)
                        Else
                            MiObj.Amount = .Drop(NroDrop).Amount
                            MiObj.ObjIndex = .Drop(NroDrop).ObjIndex

                            Call TirarItemAlPiso(.Pos, MiObj)
                        End If
                    End If

                End If

            End With

        Catch ex As Exception
            Console.WriteLine("Error in TirarItemAlPiso: " & ex.Message)
        End Try
    End Sub

    Function QuedanItems(NpcIndex As Short, ObjIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

            Dim i As Short
            If Npclist(NpcIndex).Invent.NroItems > 0 Then
                For i = 1 To MAX_INVENTORY_SLOTS
                    If Npclist(NpcIndex).Invent.Object_Renamed(i).ObjIndex = ObjIndex Then
                        QuedanItems = True
                        Exit Function
                    End If
                Next
            End If
            QuedanItems = False

        Catch ex As Exception
            Console.WriteLine("Error in QuedanItems: " & ex.Message)
        End Try
    End Function

    ''
    ' Gets the amount of a certain item that an npc has.
    '
    ' @param npcIndex Specifies reference to npcmerchant
    ' @param ObjIndex Specifies reference to object
    ' @return   The amount of the item that the npc has
    ' @remarks This function reads the Npc.dat file
    Function EncontrarCant(NpcIndex As Short, ObjIndex As Short) As Short
        '***************************************************
        'Author: Unknown
        'Last Modification: 03/09/08
        'Last Modification By: Marco Vanotti (Marco)
        ' - 03/09/08 EncontrarCant now returns 0 if the npc doesn't have it (Marco)
        '***************************************************
        Try
            'Devuelve la cantidad original del obj de un npc

            Dim ln, npcfile As String
            Dim i As Short

            npcfile = DatPath & "NPCs.dat"

            For i = 1 To MAX_INVENTORY_SLOTS
                ln = GetVar(npcfile, "NPC" & Npclist(NpcIndex).Numero, "Obj" & i)
                If ObjIndex = Val(ReadField(1, ln, 45)) Then
                    EncontrarCant = Val(ReadField(2, ln, 45))
                    Exit Function
                End If
            Next

            EncontrarCant = 0

        Catch ex As Exception
            Console.WriteLine("Error in reads: " & ex.Message)
        End Try
    End Function

    Sub ResetNpcInv(NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try

            Dim i As Short

            With Npclist(NpcIndex)
                .Invent.NroItems = 0

                For i = 1 To MAX_INVENTORY_SLOTS
                    .Invent.Object_Renamed(i).ObjIndex = 0
                    .Invent.Object_Renamed(i).Amount = 0
                Next i

                .InvReSpawn = 0
            End With

        Catch ex As Exception
            Console.WriteLine("Error in ResetNpcInv: " & ex.Message)
        End Try
    End Sub

    ''
    ' Removes a certain amount of items from a slot of an npc's inventory
    '
    ' @param npcIndex Specifies reference to npcmerchant
    ' @param Slot Specifies reference to npc's inventory's slot
    ' @param antidad Specifies amount of items that will be removed
    Sub QuitarNpcInvItem(NpcIndex As Short, Slot As Byte, Cantidad As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: 23/11/2009
        'Last Modification By: Marco Vanotti (Marco)
        ' - 03/09/08 Now this sub checks that te npc has an item before respawning it (Marco)
        '23/11/2009: ZaMa - Optimizacion de codigo.
        '***************************************************
        Dim ObjIndex As Short
        Dim iCant As Short

        With Npclist(NpcIndex)
            ObjIndex = .Invent.Object_Renamed(Slot).ObjIndex

            'Quita un Obj
            If ObjData_Renamed(.Invent.Object_Renamed(Slot).ObjIndex).Crucial = 0 Then
                .Invent.Object_Renamed(Slot).Amount = .Invent.Object_Renamed(Slot).Amount - Cantidad

                If .Invent.Object_Renamed(Slot).Amount <= 0 Then
                    .Invent.NroItems = .Invent.NroItems - 1
                    .Invent.Object_Renamed(Slot).ObjIndex = 0
                    .Invent.Object_Renamed(Slot).Amount = 0
                    If .Invent.NroItems = 0 And .InvReSpawn <> 1 Then
                        Call CargarInvent(NpcIndex) 'Reponemos el inventario
                    End If
                End If
            Else
                .Invent.Object_Renamed(Slot).Amount = .Invent.Object_Renamed(Slot).Amount - Cantidad

                If .Invent.Object_Renamed(Slot).Amount <= 0 Then
                    .Invent.NroItems = .Invent.NroItems - 1
                    .Invent.Object_Renamed(Slot).ObjIndex = 0
                    .Invent.Object_Renamed(Slot).Amount = 0

                    If Not QuedanItems(NpcIndex, ObjIndex) Then
                        'Check if the item is in the npc's dat.
                        iCant = EncontrarCant(NpcIndex, ObjIndex)
                        If iCant Then
                            .Invent.Object_Renamed(Slot).ObjIndex = ObjIndex
                            .Invent.Object_Renamed(Slot).Amount = iCant
                            .Invent.NroItems = .Invent.NroItems + 1
                        End If
                    End If

                    If .Invent.NroItems = 0 And .InvReSpawn <> 1 Then
                        Call CargarInvent(NpcIndex) 'Reponemos el inventario
                    End If
                End If
            End If
        End With
    End Sub

    Sub CargarInvent(NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'Vuelve a cargar el inventario del npc NpcIndex
        Dim LoopC As Short
        Dim ln As String
        Dim npcfile As String

        npcfile = DatPath & "NPCs.dat"

        With Npclist(NpcIndex)
            .Invent.NroItems = Val(GetVar(npcfile, "NPC" & .Numero, "NROITEMS"))

            For LoopC = 1 To .Invent.NroItems
                ln = GetVar(npcfile, "NPC" & .Numero, "Obj" & LoopC)
                .Invent.Object_Renamed(LoopC).ObjIndex = Val(ReadField(1, ln, 45))
                .Invent.Object_Renamed(LoopC).Amount = Val(ReadField(2, ln, 45))

            Next LoopC
        End With
    End Sub


    Public Sub TirarOroNpc(Cantidad As Integer, ByRef Pos As WorldPos)
        '***************************************************
        'Autor: ZaMa
        'Last Modification: 13/02/2010
        '***************************************************
        Try

            Dim i As Byte
            Dim MiObj As Obj
            Dim RemainingGold As Integer
            If Cantidad > 0 Then

                RemainingGold = Cantidad

                While (RemainingGold > 0)

                    ' Tira pilon de 10k
                    If RemainingGold > MAX_INVENTORY_OBJS Then
                        MiObj.Amount = MAX_INVENTORY_OBJS
                        RemainingGold = RemainingGold - MAX_INVENTORY_OBJS

                        ' Tira lo que quede
                    Else
                        MiObj.Amount = RemainingGold
                        RemainingGold = 0
                    End If

                    MiObj.ObjIndex = iORO

                    Call TirarItemAlPiso(Pos, MiObj)
                End While
            End If


        Catch ex As Exception
            Console.WriteLine("Error in NPC_TIRAR_ITEMS: " & ex.Message)
            Call LogError("Error en TirarOro. Error " & Err.Number & " : " & Err.Description)
        End Try
    End Sub
End Module
