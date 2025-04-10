Option Strict Off
Option Explicit On
Module modBanco
	'**************************************************************
	' modBanco.bas - Handles the character's bank accounts.
	'
	' Implemented by Kevin Birmingham (NEB)
	' kbneb@hotmail.com
	'**************************************************************
	
	'**************************************************************************
	'This program is free software; you can redistribute it and/or modify
	'it under the terms of the Affero General Public License;
	'either version 1 of the License, or any later version.
	'
	'This program is distributed in the hope that it will be useful,
	'but WITHOUT ANY WARRANTY; without even the implied warranty of
	'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	'Affero General Public License for more details.
	'
	'You should have received a copy of the Affero General Public License
	'along with this program; if not, you can find it at http://www.affero.org/oagpl.html
	'**************************************************************************
	
	
	Sub IniciarDeposito(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		'Hacemos un Update del inventario del usuario
		Call UpdateBanUserInv(True, UserIndex, 0)
		'Actualizamos el dinero
		Call WriteUpdateUserStats(UserIndex)
		'Mostramos la ventana pa' comerciar y ver ladear la osamenta. jajaja
		Call WriteBankInit(UserIndex)
		UserList(UserIndex).flags.Comerciando = True
		
Errhandler: 
		
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
	
	Sub UpdateBanUserInv(ByVal UpdateAll As Boolean, ByVal UserIndex As Short, ByVal Slot As Byte)
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
	
	Sub UserRetiraItem(ByVal UserIndex As Short, ByVal i As Short, ByVal Cantidad As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		
		If Cantidad < 1 Then Exit Sub
		
		Call WriteUpdateUserStats(UserIndex)
		
		If UserList(UserIndex).BancoInvent.Object_Renamed(i).Amount > 0 Then
			If Cantidad > UserList(UserIndex).BancoInvent.Object_Renamed(i).Amount Then Cantidad = UserList(UserIndex).BancoInvent.Object_Renamed(i).Amount
			'Agregamos el obj que compro al inventario
			Call UserReciveObj(UserIndex, CShort(i), Cantidad)
			'Actualizamos el inventario del usuario
			Call UpdateUserInv(True, UserIndex, 0)
			'Actualizamos el banco
			Call UpdateBanUserInv(True, UserIndex, 0)
		End If
		
		'Actualizamos la ventana de comercio
		Call UpdateVentanaBanco(UserIndex)
		
Errhandler: 
		
	End Sub
	
	Sub UserReciveObj(ByVal UserIndex As Short, ByVal ObjIndex As Short, ByVal Cantidad As Short)
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
			Do Until .Invent.Object_Renamed(Slot).ObjIndex = obji And .Invent.Object_Renamed(Slot).Amount + Cantidad <= MAX_INVENTORY_OBJS
				
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
						Call WriteConsoleMsg(UserIndex, "No podés tener mas objetos.", Protocol.FontTypeNames.FONTTYPE_INFO)
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
				Call WriteConsoleMsg(UserIndex, "No podés tener mas objetos.", Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
		End With
		
	End Sub
	
	Sub QuitarBancoInvItem(ByVal UserIndex As Short, ByVal Slot As Byte, ByVal Cantidad As Short)
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
	
	Sub UpdateVentanaBanco(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Call WriteBankOK(UserIndex)
	End Sub
	
	Sub UserDepositaItem(ByVal UserIndex As Short, ByVal Item As Short, ByVal Cantidad As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		If UserList(UserIndex).Invent.Object_Renamed(Item).Amount > 0 And Cantidad > 0 Then
			If Cantidad > UserList(UserIndex).Invent.Object_Renamed(Item).Amount Then Cantidad = UserList(UserIndex).Invent.Object_Renamed(Item).Amount
			
			'Agregamos el obj que deposita al banco
			Call UserDejaObj(UserIndex, CShort(Item), Cantidad)
			
			'Actualizamos el inventario del usuario
			Call UpdateUserInv(True, UserIndex, 0)
			
			'Actualizamos el inventario del banco
			Call UpdateBanUserInv(True, UserIndex, 0)
		End If
		
		'Actualizamos la ventana del banco
		Call UpdateVentanaBanco(UserIndex)
Errhandler: 
	End Sub
	
	Sub UserDejaObj(ByVal UserIndex As Short, ByVal ObjIndex As Short, ByVal Cantidad As Short)
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
			Do Until .BancoInvent.Object_Renamed(Slot).ObjIndex = obji And .BancoInvent.Object_Renamed(Slot).Amount + Cantidad <= MAX_INVENTORY_OBJS
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
						Call WriteConsoleMsg(UserIndex, "No tienes mas espacio en el banco!!", Protocol.FontTypeNames.FONTTYPE_INFO)
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
					Call WriteConsoleMsg(UserIndex, "El banco no puede cargar tantos objetos.", Protocol.FontTypeNames.FONTTYPE_INFO)
				End If
			End If
		End With
	End Sub
	
	Sub SendUserBovedaTxt(ByVal sendIndex As Short, ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error Resume Next
		Dim j As Short
		
		Call WriteConsoleMsg(sendIndex, UserList(UserIndex).name, Protocol.FontTypeNames.FONTTYPE_INFO)
		Call WriteConsoleMsg(sendIndex, "Tiene " & UserList(UserIndex).BancoInvent.NroItems & " objetos.", Protocol.FontTypeNames.FONTTYPE_INFO)
		
		For j = 1 To MAX_BANCOINVENTORY_SLOTS
			If UserList(UserIndex).BancoInvent.Object_Renamed(j).ObjIndex > 0 Then
				Call WriteConsoleMsg(sendIndex, "Objeto " & j & " " & ObjData_Renamed(UserList(UserIndex).BancoInvent.Object_Renamed(j).ObjIndex).name & " Cantidad:" & UserList(UserIndex).BancoInvent.Object_Renamed(j).Amount, Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
		Next 
		
	End Sub
	
	Sub SendUserBovedaTxtFromChar(ByVal sendIndex As Short, ByVal charName As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error Resume Next
		Dim j As Short
		Dim CharFile, Tmp As String
		Dim ObjInd, ObjCant As Integer
		
		CharFile = CharPath & charName & ".chr"
		
		If FileExist(CharFile) Then
			Call WriteConsoleMsg(sendIndex, charName, Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Tiene " & GetVar(CharFile, "BancoInventory", "CantidadItems") & " objetos.", Protocol.FontTypeNames.FONTTYPE_INFO)
			For j = 1 To MAX_BANCOINVENTORY_SLOTS
				Tmp = GetVar(CharFile, "BancoInventory", "Obj" & j)
				ObjInd = CInt(ReadField(1, Tmp, Asc("-")))
				ObjCant = CInt(ReadField(2, Tmp, Asc("-")))
				If ObjInd > 0 Then
					Call WriteConsoleMsg(sendIndex, "Objeto " & j & " " & ObjData_Renamed(ObjInd).name & " Cantidad:" & ObjCant, Protocol.FontTypeNames.FONTTYPE_INFO)
				End If
			Next 
		Else
			Call WriteConsoleMsg(sendIndex, "Usuario inexistente: " & charName, Protocol.FontTypeNames.FONTTYPE_INFO)
		End If
		
	End Sub
End Module
