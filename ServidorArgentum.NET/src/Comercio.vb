Option Strict Off
Option Explicit On
Module modSistemaComercio
	'*****************************************************
	'Sistema de Comercio para Argentum Online
	'Programado por Nacho (Integer)
	'integer-x@hotmail.com
	'*****************************************************
	
	'**************************************************************************
	'This program is free software; you can redistribute it and/or modify
	'it under the terms of the GNU General Public License as published by
	'the Free Software Foundation; either version 2 of the License, or
	'(at your option) any later version.
	'
	'This program is distributed in the hope that it will be useful,
	'but WITHOUT ANY WARRANTY; without even the implied warranty of
	'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	'GNU General Public License for more details.
	'
	'You should have received a copy of the GNU General Public License
	'along with this program; if not, write to the Free Software
	'Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
	'**************************************************************************
	
	
	Enum eModoComercio
		Compra = 1
		Venta = 2
	End Enum
	
	Public Const REDUCTOR_PRECIOVENTA As Byte = 3
	
	''
	' Makes a trade. (Buy or Sell)
	'
	' @param Modo The trade type (sell or buy)
	' @param UserIndex Specifies the index of the user
	' @param NpcIndex specifies the index of the npc
	' @param Slot Specifies which slot are you trying to sell / buy
	' @param Cantidad Specifies how many items in that slot are you trying to sell / buy
	Public Sub Comercio(ByVal Modo As eModoComercio, ByVal UserIndex As Short, ByVal NpcIndex As Short, ByVal Slot As Short, ByVal Cantidad As Short)
		'*************************************************
		'Author: Nacho (Integer)
		'Last modified: 27/07/08 (MarKoxX) | New changes in the way of trading (now when you buy it rounds to ceil and when you sell it rounds to floor)
		'  - 06/13/08 (NicoNZ)
		'*************************************************
		Dim Precio As Integer
		Dim Objeto As Obj
		
		If Cantidad < 1 Or Slot < 1 Then Exit Sub
		
		Dim NpcSlot As Short
		If Modo = eModoComercio.Compra Then
			If Slot > MAX_INVENTORY_SLOTS Then
				Exit Sub
			ElseIf Cantidad > MAX_INVENTORY_OBJS Then 
				Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg(UserList(UserIndex).name & " ha sido baneado por el sistema anti-cheats.", Protocol.FontTypeNames.FONTTYPE_FIGHT))
				Call Ban(UserList(UserIndex).name, "Sistema Anti Cheats", "Intentar hackear el sistema de comercio. Quiso comprar demasiados ítems:" & Cantidad)
				UserList(UserIndex).flags.Ban = 1
				Call WriteErrorMsg(UserIndex, "Has sido baneado por el Sistema AntiCheat.")
				Call FlushBuffer(UserIndex)
				Call CloseSocket(UserIndex)
				Exit Sub
			ElseIf Not Npclist(NpcIndex).Invent.Object_Renamed(Slot).Amount > 0 Then 
				Exit Sub
			End If
			
			If Cantidad > Npclist(NpcIndex).Invent.Object_Renamed(Slot).Amount Then Cantidad = Npclist(UserList(UserIndex).flags.TargetNPC).Invent.Object_Renamed(Slot).Amount
			
			Objeto.Amount = Cantidad
			Objeto.ObjIndex = Npclist(NpcIndex).Invent.Object_Renamed(Slot).ObjIndex
			
			'El precio, cuando nos venden algo, lo tenemos que redondear para arriba.
			'Es decir, 1.1 = 2, por lo cual se hace de la siguiente forma Precio = Clng(PrecioFinal + 0.5) Siempre va a darte el proximo numero. O el "Techo" (MarKoxX)
			
			Precio = CInt((ObjData_Renamed(Npclist(NpcIndex).Invent.Object_Renamed(Slot).ObjIndex).Valor / Descuento(UserIndex) * Cantidad) + 0.5)
			
			If UserList(UserIndex).Stats.GLD < Precio Then
				Call WriteConsoleMsg(UserIndex, "No tienes suficiente dinero.", Protocol.FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If
			
			
			If MeterItemEnInventario(UserIndex, Objeto) = False Then
				'Call WriteConsoleMsg(UserIndex, "No puedes cargar mas objetos.", FontTypeNames.FONTTYPE_INFO)
				Call EnviarNpcInv(UserIndex, UserList(UserIndex).flags.TargetNPC)
				Call WriteTradeOK(UserIndex)
				Exit Sub
			End If
			
			UserList(UserIndex).Stats.GLD = UserList(UserIndex).Stats.GLD - Precio
			
			Call QuitarNpcInvItem(UserList(UserIndex).flags.TargetNPC, CByte(Slot), Cantidad)
			
			'Bien, ahora logueo de ser necesario. Pablo (ToxicWaste) 07/09/07
			'Es un Objeto que tenemos que loguear?
			If ObjData_Renamed(Objeto.ObjIndex).Log = 1 Then
				Call LogDesarrollo(UserList(UserIndex).name & " compró del NPC " & Objeto.Amount & " " & ObjData_Renamed(Objeto.ObjIndex).name)
			ElseIf Objeto.Amount = 1000 Then  'Es mucha cantidad?
				'Si no es de los prohibidos de loguear, lo logueamos.
				If ObjData_Renamed(Objeto.ObjIndex).NoLog <> 1 Then
					Call LogDesarrollo(UserList(UserIndex).name & " compró del NPC " & Objeto.Amount & " " & ObjData_Renamed(Objeto.ObjIndex).name)
				End If
			End If
			
			'Agregado para que no se vuelvan a vender las llaves si se recargan los .dat.
			If ObjData_Renamed(Objeto.ObjIndex).OBJType = Declaraciones.eOBJType.otLlaves Then
				Call WriteVar(DatPath & "NPCs.dat", "NPC" & Npclist(NpcIndex).Numero, "obj" & Slot, Objeto.ObjIndex & "-0")
				Call logVentaCasa(UserList(UserIndex).name & " compró " & ObjData_Renamed(Objeto.ObjIndex).name)
			End If
			
		ElseIf Modo = eModoComercio.Venta Then 
			
			If Cantidad > UserList(UserIndex).Invent.Object_Renamed(Slot).Amount Then Cantidad = UserList(UserIndex).Invent.Object_Renamed(Slot).Amount
			
			Objeto.Amount = Cantidad
			Objeto.ObjIndex = UserList(UserIndex).Invent.Object_Renamed(Slot).ObjIndex
			
			If Objeto.ObjIndex = 0 Then
				Exit Sub
			ElseIf (Npclist(NpcIndex).TipoItems <> ObjData_Renamed(Objeto.ObjIndex).OBJType And Npclist(NpcIndex).TipoItems <> Declaraciones.eOBJType.otCualquiera) Or Objeto.ObjIndex = iORO Then 
				Call WriteConsoleMsg(UserIndex, "Lo siento, no estoy interesado en este tipo de objetos.", Protocol.FontTypeNames.FONTTYPE_INFO)
				Call EnviarNpcInv(UserIndex, UserList(UserIndex).flags.TargetNPC)
				Call WriteTradeOK(UserIndex)
				Exit Sub
			ElseIf ObjData_Renamed(Objeto.ObjIndex).Real = 1 Then 
				If Npclist(NpcIndex).name <> "SR" Then
					Call WriteConsoleMsg(UserIndex, "Las armaduras del ejército real sólo pueden ser vendidas a los sastres reales.", Protocol.FontTypeNames.FONTTYPE_INFO)
					Call EnviarNpcInv(UserIndex, UserList(UserIndex).flags.TargetNPC)
					Call WriteTradeOK(UserIndex)
					Exit Sub
				End If
			ElseIf ObjData_Renamed(Objeto.ObjIndex).Caos = 1 Then 
				If Npclist(NpcIndex).name <> "SC" Then
					Call WriteConsoleMsg(UserIndex, "Las armaduras de la legión oscura sólo pueden ser vendidas a los sastres del demonio.", Protocol.FontTypeNames.FONTTYPE_INFO)
					Call EnviarNpcInv(UserIndex, UserList(UserIndex).flags.TargetNPC)
					Call WriteTradeOK(UserIndex)
					Exit Sub
				End If
			ElseIf UserList(UserIndex).Invent.Object_Renamed(Slot).Amount < 0 Or Cantidad = 0 Then 
				Exit Sub
			ElseIf Slot < LBound(UserList(UserIndex).Invent.Object_Renamed) Or Slot > UBound(UserList(UserIndex).Invent.Object_Renamed) Then 
				Call EnviarNpcInv(UserIndex, UserList(UserIndex).flags.TargetNPC)
				Exit Sub
			ElseIf UserList(UserIndex).flags.Privilegios And Declaraciones.PlayerType.Consejero Then 
				Call WriteConsoleMsg(UserIndex, "No puedes vender ítems.", Protocol.FontTypeNames.FONTTYPE_WARNING)
				Call EnviarNpcInv(UserIndex, UserList(UserIndex).flags.TargetNPC)
				Call WriteTradeOK(UserIndex)
				Exit Sub
			End If
			
			Call QuitarUserInvItem(UserIndex, Slot, Cantidad)
			
			'Precio = Round(ObjData(Objeto.ObjIndex).valor / REDUCTOR_PRECIOVENTA * Cantidad, 0)
			Precio = Fix(SalePrice(Objeto.ObjIndex) * Cantidad)
			UserList(UserIndex).Stats.GLD = UserList(UserIndex).Stats.GLD + Precio
			
			If UserList(UserIndex).Stats.GLD > MAXORO Then UserList(UserIndex).Stats.GLD = MAXORO
			
			NpcSlot = SlotEnNPCInv(NpcIndex, Objeto.ObjIndex, Objeto.Amount)
			
			If NpcSlot <= MAX_INVENTORY_SLOTS Then 'Slot valido
				'Mete el obj en el slot
				Npclist(NpcIndex).Invent.Object_Renamed(NpcSlot).ObjIndex = Objeto.ObjIndex
				Npclist(NpcIndex).Invent.Object_Renamed(NpcSlot).Amount = Npclist(NpcIndex).Invent.Object_Renamed(NpcSlot).Amount + Objeto.Amount
				If Npclist(NpcIndex).Invent.Object_Renamed(NpcSlot).Amount > MAX_INVENTORY_OBJS Then
					Npclist(NpcIndex).Invent.Object_Renamed(NpcSlot).Amount = MAX_INVENTORY_OBJS
				End If
			End If
			
			'Bien, ahora logueo de ser necesario. Pablo (ToxicWaste) 07/09/07
			'Es un Objeto que tenemos que loguear?
			If ObjData_Renamed(Objeto.ObjIndex).Log = 1 Then
				Call LogDesarrollo(UserList(UserIndex).name & " vendió al NPC " & Objeto.Amount & " " & ObjData_Renamed(Objeto.ObjIndex).name)
			ElseIf Objeto.Amount = 1000 Then  'Es mucha cantidad?
				'Si no es de los prohibidos de loguear, lo logueamos.
				If ObjData_Renamed(Objeto.ObjIndex).NoLog <> 1 Then
					Call LogDesarrollo(UserList(UserIndex).name & " vendió al NPC " & Objeto.Amount & " " & ObjData_Renamed(Objeto.ObjIndex).name)
				End If
			End If
			
		End If
		
		Call UpdateUserInv(True, UserIndex, 0)
		Call WriteUpdateUserStats(UserIndex)
		Call EnviarNpcInv(UserIndex, UserList(UserIndex).flags.TargetNPC)
		Call WriteTradeOK(UserIndex)
		
		Call SubirSkill(UserIndex, Declaraciones.eSkill.Comerciar, True)
	End Sub
	
	Public Sub IniciarComercioNPC(ByVal UserIndex As Short)
		'*************************************************
		'Author: Nacho (Integer)
		'Last modified: 2/8/06
		'*************************************************
		Call EnviarNpcInv(UserIndex, UserList(UserIndex).flags.TargetNPC)
		UserList(UserIndex).flags.Comerciando = True
		Call WriteCommerceInit(UserIndex)
	End Sub
	
	Private Function SlotEnNPCInv(ByVal NpcIndex As Short, ByVal Objeto As Short, ByVal Cantidad As Short) As Short
		'*************************************************
		'Author: Nacho (Integer)
		'Last modified: 2/8/06
		'*************************************************
		SlotEnNPCInv = 1
		Do Until Npclist(NpcIndex).Invent.Object_Renamed(SlotEnNPCInv).ObjIndex = Objeto And Npclist(NpcIndex).Invent.Object_Renamed(SlotEnNPCInv).Amount + Cantidad <= MAX_INVENTORY_OBJS
			
			SlotEnNPCInv = SlotEnNPCInv + 1
			If SlotEnNPCInv > MAX_INVENTORY_SLOTS Then Exit Do
			
		Loop 
		
		If SlotEnNPCInv > MAX_INVENTORY_SLOTS Then
			
			SlotEnNPCInv = 1
			
			Do Until Npclist(NpcIndex).Invent.Object_Renamed(SlotEnNPCInv).ObjIndex = 0
				
				SlotEnNPCInv = SlotEnNPCInv + 1
				If SlotEnNPCInv > MAX_INVENTORY_SLOTS Then Exit Do
				
			Loop 
			
			If SlotEnNPCInv <= MAX_INVENTORY_SLOTS Then Npclist(NpcIndex).Invent.NroItems = Npclist(NpcIndex).Invent.NroItems + 1
			
		End If
		
	End Function
	
	Private Function Descuento(ByVal UserIndex As Short) As Single
		'*************************************************
		'Author: Nacho (Integer)
		'Last modified: 2/8/06
		'*************************************************
		Descuento = 1 + UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Comerciar) / 100
	End Function
	
	''
	' Send the inventory of the Npc to the user
	'
	' @param userIndex The index of the User
	' @param npcIndex The index of the NPC
	
	Private Sub EnviarNpcInv(ByVal UserIndex As Short, ByVal NpcIndex As Short)
		'*************************************************
		'Author: Nacho (Integer)
		'Last Modified: 06/14/08
		'Last Modified By: Nicolás Ezequiel Bouhid (NicoNZ)
		'*************************************************
		Dim Slot As Byte
		'UPGRADE_NOTE: val se actualizó a val_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim val_Renamed As Single
		
		Dim thisObj As Obj
		Dim DummyObj As Obj
		For Slot = 1 To MAX_NORMAL_INVENTORY_SLOTS
			If Npclist(NpcIndex).Invent.Object_Renamed(Slot).ObjIndex > 0 Then
				
				thisObj.ObjIndex = Npclist(NpcIndex).Invent.Object_Renamed(Slot).ObjIndex
				thisObj.Amount = Npclist(NpcIndex).Invent.Object_Renamed(Slot).Amount
				
				val_Renamed = (ObjData_Renamed(thisObj.ObjIndex).Valor) / Descuento(UserIndex)
				
				Call WriteChangeNPCInventorySlot(UserIndex, Slot, thisObj, val_Renamed)
			Else
				Call WriteChangeNPCInventorySlot(UserIndex, Slot, DummyObj, 0)
			End If
		Next Slot
	End Sub
	
	''
	' Devuelve el valor de venta del objeto
	'
	' @param ObjIndex  El número de objeto al cual le calculamos el precio de venta
	
	Public Function SalePrice(ByVal ObjIndex As Short) As Single
		'*************************************************
		'Author: Nicolás (NicoNZ)
		'
		'*************************************************
		If ObjIndex < 1 Or ObjIndex > UBound(ObjData_Renamed) Then Exit Function
		If ItemNewbie(ObjIndex) Then Exit Function
		
		SalePrice = ObjData_Renamed(ObjIndex).Valor / REDUCTOR_PRECIOVENTA
	End Function
End Module
