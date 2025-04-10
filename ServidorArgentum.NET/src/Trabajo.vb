Option Strict Off
Option Explicit On
Module Trabajo
	'Argentum Online 0.12.2
	'Copyright (C) 2002 Márquez Pablo Ignacio
	'
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
	'
	'Argentum Online is based on Baronsoft's VB6 Online RPG
	'You can contact the original creator of ORE at aaron@baronsoft.com
	'for more information about ORE please visit http://www.baronsoft.com/
	'
	'
	'You can contact me at:
	'morgolock@speedy.com.ar
	'www.geocities.com/gmorgolock
	'Calle 3 número 983 piso 7 dto A
	'La Plata - Pcia, Buenos Aires - Republica Argentina
	'Código Postal 1900
	'Pablo Ignacio Márquez
	
	
	Private Const GASTO_ENERGIA_TRABAJADOR As Byte = 2
	Private Const GASTO_ENERGIA_NO_TRABAJADOR As Byte = 6
	
	
	Public Sub DoPermanecerOculto(ByVal UserIndex As Short)
		'********************************************************
		'Autor: Nacho (Integer)
		'Last Modif: 11/19/2009
		'Chequea si ya debe mostrarse
		'Pablo (ToxicWaste): Cambie los ordenes de prioridades porque sino no andaba.
		'11/19/2009: Pato - Ahora el bandido se oculta la mitad del tiempo de las demás clases.
		'13/01/2010: ZaMa - Now hidden on boat pirats recover the proper boat body.
		'13/01/2010: ZaMa - Arreglo condicional para que el bandido camine oculto.
		'********************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex)
			.Counters.TiempoOculto = .Counters.TiempoOculto - 1
			If .Counters.TiempoOculto <= 0 Then
				
				If .clase = Declaraciones.eClass.Bandit Then
					.Counters.TiempoOculto = Int(IntervaloOculto / 2)
				Else
					.Counters.TiempoOculto = IntervaloOculto
				End If
				
				If .clase = Declaraciones.eClass.Hunter And .Stats.UserSkills(Declaraciones.eSkill.Ocultarse) > 90 Then
					If .Invent.ArmourEqpObjIndex = 648 Or .Invent.ArmourEqpObjIndex = 360 Then
						Exit Sub
					End If
				End If
				.Counters.TiempoOculto = 0
				.flags.Oculto = 0
				
				If .flags.Navegando = 1 Then
					If .clase = Declaraciones.eClass.Pirat Then
						' Pierde la apariencia de fragata fantasmal
						Call ToogleBoatBody(UserIndex)
						Call WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!", Protocol.FontTypeNames.FONTTYPE_INFO)
						Call ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading, NingunArma, NingunEscudo, NingunCasco)
					End If
				Else
					If .flags.invisible = 0 Then
						Call WriteConsoleMsg(UserIndex, "Has vuelto a ser visible.", Protocol.FontTypeNames.FONTTYPE_INFO)
						Call SetInvisible(UserIndex, .Char_Renamed.CharIndex, False)
					End If
				End If
			End If
		End With
		
		Exit Sub
		
Errhandler: 
		Call LogError("Error en Sub DoPermanecerOculto")
		
		
	End Sub
	
	Public Sub DoOcultarse(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: 13/01/2010 (ZaMa)
		'Pablo (ToxicWaste): No olvidar agregar IntervaloOculto=500 al Server.ini.
		'Modifique la fórmula y ahora anda bien.
		'13/01/2010: ZaMa - El pirata se transforma en galeon fantasmal cuando se oculta en agua.
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim Suerte As Double
		Dim res As Short
		Dim Skill As Short
		
		With UserList(UserIndex)
			Skill = .Stats.UserSkills(Declaraciones.eSkill.Ocultarse)
			
			Suerte = (((0.000002 * Skill - 0.0002) * Skill + 0.0064) * Skill + 0.1124) * 100
			
			res = RandomNumber(1, 100)
			
			If res <= Suerte Then
				
				.flags.Oculto = 1
				Suerte = (-0.000001 * (100 - Skill) ^ 3)
				Suerte = Suerte + (0.00009229 * (100 - Skill) ^ 2)
				Suerte = Suerte + (-0.0088 * (100 - Skill))
				Suerte = Suerte + (0.9571)
				Suerte = Suerte * IntervaloOculto
				.Counters.TiempoOculto = Suerte
				
				' No es pirata o es uno sin barca
				If .flags.Navegando = 0 Then
					Call SetInvisible(UserIndex, .Char_Renamed.CharIndex, True)
					
					Call WriteConsoleMsg(UserIndex, "¡Te has escondido entre las sombras!", Protocol.FontTypeNames.FONTTYPE_INFO)
					' Es un pirata navegando
				Else
					' Le cambiamos el body a galeon fantasmal
					.Char_Renamed.body = iFragataFantasmal
					' Actualizamos clientes
					Call ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading, NingunArma, NingunEscudo, NingunCasco)
				End If
				
				Call SubirSkill(UserIndex, Declaraciones.eSkill.Ocultarse, True)
			Else
				'[CDT 17-02-2004]
				If Not .flags.UltimoMensaje = 4 Then
					Call WriteConsoleMsg(UserIndex, "¡No has logrado esconderte!", Protocol.FontTypeNames.FONTTYPE_INFO)
					.flags.UltimoMensaje = 4
				End If
				'[/CDT]
				
				Call SubirSkill(UserIndex, Declaraciones.eSkill.Ocultarse, False)
			End If
			
			.Counters.Ocultando = .Counters.Ocultando + 1
		End With
		
		Exit Sub
		
Errhandler: 
		Call LogError("Error en Sub DoOcultarse")
		
	End Sub
	
	Public Sub DoNavega(ByVal UserIndex As Short, ByRef Barco As ObjData, ByVal Slot As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: 13/01/2010 (ZaMa)
		'13/01/2010: ZaMa - El pirata pierde el ocultar si desequipa barca.
		'***************************************************
		
		Dim ModNave As Single
		
		With UserList(UserIndex)
			ModNave = ModNavegacion(.clase, UserIndex)
			
			If .Stats.UserSkills(Declaraciones.eSkill.Navegacion) / ModNave < Barco.MinSkill Then
				Call WriteConsoleMsg(UserIndex, "No tienes suficientes conocimientos para usar este barco.", Protocol.FontTypeNames.FONTTYPE_INFO)
				Call WriteConsoleMsg(UserIndex, "Para usar este barco necesitas " & Barco.MinSkill * ModNave & " puntos en navegacion.", Protocol.FontTypeNames.FONTTYPE_INFO)
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
					
					If .clase = Declaraciones.eClass.Pirat Then
						If .flags.Oculto = 1 Then
							.flags.Oculto = 0
							Call SetInvisible(UserIndex, .Char_Renamed.CharIndex, False)
							Call WriteConsoleMsg(UserIndex, "¡Has vuelto a ser visible!", Protocol.FontTypeNames.FONTTYPE_INFO)
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
					
					If .clase = Declaraciones.eClass.Pirat Then
						If .flags.Oculto = 1 Then
							' Al desequipar barca, perdió el ocultar
							.flags.Oculto = 0
							.Counters.Ocultando = 0
							Call WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!", Protocol.FontTypeNames.FONTTYPE_INFO)
						End If
					End If
					
					If .Invent.ArmourEqpObjIndex > 0 Then
						.Char_Renamed.body = ObjData_Renamed(.Invent.ArmourEqpObjIndex).Ropaje
					Else
						Call DarCuerpoDesnudo(UserIndex)
					End If
					
					If .Invent.EscudoEqpObjIndex > 0 Then .Char_Renamed.ShieldAnim = ObjData_Renamed(.Invent.EscudoEqpObjIndex).ShieldAnim
					If .Invent.WeaponEqpObjIndex > 0 Then .Char_Renamed.WeaponAnim = GetWeaponAnim(UserIndex, .Invent.WeaponEqpObjIndex)
					If .Invent.CascoEqpObjIndex > 0 Then .Char_Renamed.CascoAnim = ObjData_Renamed(.Invent.CascoEqpObjIndex).CascoAnim
					
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
			Call ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading, .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim, .Char_Renamed.CascoAnim)
		End With
		
		Call WriteNavigateToggle(UserIndex)
		
	End Sub
	
	Public Sub FundirMineral(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		With UserList(UserIndex)
			If .flags.TargetObjInvIndex > 0 Then
				
				If ObjData_Renamed(.flags.TargetObjInvIndex).OBJType = Declaraciones.eOBJType.otMinerales And ObjData_Renamed(.flags.TargetObjInvIndex).MinSkill <= .Stats.UserSkills(Declaraciones.eSkill.Mineria) / ModFundicion(.clase) Then
					Call DoLingotes(UserIndex)
				Else
					Call WriteConsoleMsg(UserIndex, "No tienes conocimientos de minería suficientes para trabajar este mineral.", Protocol.FontTypeNames.FONTTYPE_INFO)
				End If
				
			End If
		End With
		
		Exit Sub
		
Errhandler: 
		Call LogError("Error en FundirMineral. Error " & Err.Number & " : " & Err.Description)
		
	End Sub
	
	Public Sub FundirArmas(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		With UserList(UserIndex)
			If .flags.TargetObjInvIndex > 0 Then
				If ObjData_Renamed(.flags.TargetObjInvIndex).OBJType = Declaraciones.eOBJType.otWeapon Then
					If ObjData_Renamed(.flags.TargetObjInvIndex).SkHerreria <= .Stats.UserSkills(Declaraciones.eSkill.Herreria) / ModHerreriA(.clase) Then
						Call DoFundir(UserIndex)
					Else
						Call WriteConsoleMsg(UserIndex, "No tienes los conocimientos suficientes en herrería para fundir este objeto.", Protocol.FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			End If
		End With
		
		Exit Sub
Errhandler: 
		Call LogError("Error en FundirArmas. Error " & Err.Number & " : " & Err.Description)
	End Sub
	
	Function TieneObjetos(ByVal ItemIndex As Short, ByVal cant As Short, ByVal UserIndex As Short) As Boolean
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
	
	Public Sub QuitarObjetos(ByVal ItemIndex As Short, ByVal cant As Short, ByVal UserIndex As Short)
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
						cant = System.Math.Abs(.Amount)
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
	
	Sub HerreroQuitarMateriales(ByVal UserIndex As Short, ByVal ItemIndex As Short, ByVal CantidadItems As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: 16/11/2009
		'16/11/2009: ZaMa - Ahora considera la cantidad de items a construir
		'***************************************************
		With ObjData_Renamed(ItemIndex)
			If .LingH > 0 Then Call QuitarObjetos(LingoteHierro, .LingH * CantidadItems, UserIndex)
			If .LingP > 0 Then Call QuitarObjetos(LingotePlata, .LingP * CantidadItems, UserIndex)
			If .LingO > 0 Then Call QuitarObjetos(LingoteOro, .LingO * CantidadItems, UserIndex)
		End With
	End Sub
	
	Sub CarpinteroQuitarMateriales(ByVal UserIndex As Short, ByVal ItemIndex As Short, ByVal CantidadItems As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: 16/11/2009
		'16/11/2009: ZaMa - Ahora quita tambien madera elfica
		'***************************************************
		With ObjData_Renamed(ItemIndex)
			If .Madera > 0 Then Call QuitarObjetos(Leña, .Madera * CantidadItems, UserIndex)
			If .MaderaElfica > 0 Then Call QuitarObjetos(LeñaElfica, .MaderaElfica * CantidadItems, UserIndex)
		End With
	End Sub
	
	Function CarpinteroTieneMateriales(ByVal UserIndex As Short, ByVal ItemIndex As Short, ByVal Cantidad As Short, Optional ByVal ShowMsg As Boolean = False) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: 16/11/2009
		'16/11/2009: ZaMa - Agregada validacion a madera elfica.
		'16/11/2009: ZaMa - Ahora considera la cantidad de items a construir
		'***************************************************
		
		With ObjData_Renamed(ItemIndex)
			If .Madera > 0 Then
				If Not TieneObjetos(Leña, .Madera * Cantidad, UserIndex) Then
					If ShowMsg Then Call WriteConsoleMsg(UserIndex, "No tienes suficiente madera.", Protocol.FontTypeNames.FONTTYPE_INFO)
					CarpinteroTieneMateriales = False
					Exit Function
				End If
			End If
			
			If .MaderaElfica > 0 Then
				If Not TieneObjetos(LeñaElfica, .MaderaElfica * Cantidad, UserIndex) Then
					If ShowMsg Then Call WriteConsoleMsg(UserIndex, "No tienes suficiente madera élfica.", Protocol.FontTypeNames.FONTTYPE_INFO)
					CarpinteroTieneMateriales = False
					Exit Function
				End If
			End If
			
		End With
		CarpinteroTieneMateriales = True
		
	End Function
	
	Function HerreroTieneMateriales(ByVal UserIndex As Short, ByVal ItemIndex As Short, ByVal CantidadItems As Short) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: 16/11/2009
		'16/11/2009: ZaMa - Agregada validacion a madera elfica.
		'***************************************************
		With ObjData_Renamed(ItemIndex)
			If .LingH > 0 Then
				If Not TieneObjetos(LingoteHierro, .LingH * CantidadItems, UserIndex) Then
					Call WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de hierro.", Protocol.FontTypeNames.FONTTYPE_INFO)
					HerreroTieneMateriales = False
					Exit Function
				End If
			End If
			If .LingP > 0 Then
				If Not TieneObjetos(LingotePlata, .LingP * CantidadItems, UserIndex) Then
					Call WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de plata.", Protocol.FontTypeNames.FONTTYPE_INFO)
					HerreroTieneMateriales = False
					Exit Function
				End If
			End If
			If .LingO > 0 Then
				If Not TieneObjetos(LingoteOro, .LingO * CantidadItems, UserIndex) Then
					Call WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de oro.", Protocol.FontTypeNames.FONTTYPE_INFO)
					HerreroTieneMateriales = False
					Exit Function
				End If
			End If
		End With
		HerreroTieneMateriales = True
	End Function
	
	Function TieneMaterialesUpgrade(ByVal UserIndex As Short, ByVal ItemIndex As Short) As Boolean
		'***************************************************
		'Author: Torres Patricio (Pato)
		'Last Modification: 12/08/2009
		'
		'***************************************************
		Dim ItemUpgrade As Short
		
		ItemUpgrade = ObjData_Renamed(ItemIndex).Upgrade
		
		With ObjData_Renamed(ItemUpgrade)
			If .LingH > 0 Then
				If Not TieneObjetos(LingoteHierro, CShort(.LingH - ObjData_Renamed(ItemIndex).LingH * PORCENTAJE_MATERIALES_UPGRADE), UserIndex) Then
					Call WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de hierro.", Protocol.FontTypeNames.FONTTYPE_INFO)
					TieneMaterialesUpgrade = False
					Exit Function
				End If
			End If
			
			If .LingP > 0 Then
				If Not TieneObjetos(LingotePlata, CShort(.LingP - ObjData_Renamed(ItemIndex).LingP * PORCENTAJE_MATERIALES_UPGRADE), UserIndex) Then
					Call WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de plata.", Protocol.FontTypeNames.FONTTYPE_INFO)
					TieneMaterialesUpgrade = False
					Exit Function
				End If
			End If
			
			If .LingO > 0 Then
				If Not TieneObjetos(LingoteOro, CShort(.LingO - ObjData_Renamed(ItemIndex).LingO * PORCENTAJE_MATERIALES_UPGRADE), UserIndex) Then
					Call WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de oro.", Protocol.FontTypeNames.FONTTYPE_INFO)
					TieneMaterialesUpgrade = False
					Exit Function
				End If
			End If
			
			If .Madera > 0 Then
				If Not TieneObjetos(Leña, CShort(.Madera - ObjData_Renamed(ItemIndex).Madera * PORCENTAJE_MATERIALES_UPGRADE), UserIndex) Then
					Call WriteConsoleMsg(UserIndex, "No tienes suficiente madera.", Protocol.FontTypeNames.FONTTYPE_INFO)
					TieneMaterialesUpgrade = False
					Exit Function
				End If
			End If
			
			If .MaderaElfica > 0 Then
				If Not TieneObjetos(LeñaElfica, CShort(.MaderaElfica - ObjData_Renamed(ItemIndex).MaderaElfica * PORCENTAJE_MATERIALES_UPGRADE), UserIndex) Then
					Call WriteConsoleMsg(UserIndex, "No tienes suficiente madera élfica.", Protocol.FontTypeNames.FONTTYPE_INFO)
					TieneMaterialesUpgrade = False
					Exit Function
				End If
			End If
		End With
		
		TieneMaterialesUpgrade = True
	End Function
	
	Sub QuitarMaterialesUpgrade(ByVal UserIndex As Short, ByVal ItemIndex As Short)
		'***************************************************
		'Author: Torres Patricio (Pato)
		'Last Modification: 12/08/2009
		'
		'***************************************************
		Dim ItemUpgrade As Short
		
		ItemUpgrade = ObjData_Renamed(ItemIndex).Upgrade
		
		With ObjData_Renamed(ItemUpgrade)
			If .LingH > 0 Then Call QuitarObjetos(LingoteHierro, CShort(.LingH - ObjData_Renamed(ItemIndex).LingH * PORCENTAJE_MATERIALES_UPGRADE), UserIndex)
			If .LingP > 0 Then Call QuitarObjetos(LingotePlata, CShort(.LingP - ObjData_Renamed(ItemIndex).LingP * PORCENTAJE_MATERIALES_UPGRADE), UserIndex)
			If .LingO > 0 Then Call QuitarObjetos(LingoteOro, CShort(.LingO - ObjData_Renamed(ItemIndex).LingO * PORCENTAJE_MATERIALES_UPGRADE), UserIndex)
			If .Madera > 0 Then Call QuitarObjetos(Leña, CShort(.Madera - ObjData_Renamed(ItemIndex).Madera * PORCENTAJE_MATERIALES_UPGRADE), UserIndex)
			If .MaderaElfica > 0 Then Call QuitarObjetos(LeñaElfica, CShort(.MaderaElfica - ObjData_Renamed(ItemIndex).MaderaElfica * PORCENTAJE_MATERIALES_UPGRADE), UserIndex)
		End With
		
		Call QuitarObjetos(ItemIndex, 1, UserIndex)
	End Sub
	
	Public Function PuedeConstruir(ByVal UserIndex As Short, ByVal ItemIndex As Short, ByVal CantidadItems As Short) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: 24/08/2009
		'24/08/2008: ZaMa - Validates if the player has the required skill
		'16/11/2009: ZaMa - Validates if the player has the required amount of materials, depending on the number of items to make
		'***************************************************
		PuedeConstruir = HerreroTieneMateriales(UserIndex, ItemIndex, CantidadItems) And System.Math.Round(UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Herreria) / ModHerreriA(UserList(UserIndex).clase), 0) >= ObjData_Renamed(ItemIndex).SkHerreria
	End Function
	
	Public Function PuedeConstruirHerreria(ByVal ItemIndex As Short) As Boolean
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
	
	Public Sub HerreroConstruirItem(ByVal UserIndex As Short, ByVal ItemIndex As Short)
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
					Call WriteConsoleMsg(UserIndex, "No tienes suficientes materiales.", Protocol.FontTypeNames.FONTTYPE_INFO)
					Call WriteStopWorking(UserIndex)
					Exit Sub
				End If
				
				'Sacamos energía
				If .clase = Declaraciones.eClass.Worker Then
					'Chequeamos que tenga los puntos antes de sacarselos
					If .Stats.MinSta >= GASTO_ENERGIA_TRABAJADOR Then
						.Stats.MinSta = .Stats.MinSta - GASTO_ENERGIA_TRABAJADOR
						Call WriteUpdateSta(UserIndex)
					Else
						Call WriteConsoleMsg(UserIndex, "No tienes suficiente energía.", Protocol.FontTypeNames.FONTTYPE_INFO)
						Exit Sub
					End If
				Else
					'Chequeamos que tenga los puntos antes de sacarselos
					If .Stats.MinSta >= GASTO_ENERGIA_NO_TRABAJADOR Then
						.Stats.MinSta = .Stats.MinSta - GASTO_ENERGIA_NO_TRABAJADOR
						Call WriteUpdateSta(UserIndex)
					Else
						Call WriteConsoleMsg(UserIndex, "No tienes suficiente energía.", Protocol.FontTypeNames.FONTTYPE_INFO)
						Exit Sub
					End If
				End If
				
				Call HerreroQuitarMateriales(UserIndex, ItemIndex, CantidadItems)
				' AGREGAR FX
				If ObjData_Renamed(ItemIndex).OBJType = Declaraciones.eOBJType.otWeapon Then
					Call WriteConsoleMsg(UserIndex, "Has construido " & IIf(CantidadItems > 1, CantidadItems & " armas!", "el arma!"), Protocol.FontTypeNames.FONTTYPE_INFO)
				ElseIf ObjData_Renamed(ItemIndex).OBJType = Declaraciones.eOBJType.otESCUDO Then 
					Call WriteConsoleMsg(UserIndex, "Has construido " & IIf(CantidadItems > 1, CantidadItems & " escudos!", "el escudo!"), Protocol.FontTypeNames.FONTTYPE_INFO)
				ElseIf ObjData_Renamed(ItemIndex).OBJType = Declaraciones.eOBJType.otCASCO Then 
					Call WriteConsoleMsg(UserIndex, "Has construido " & IIf(CantidadItems > 1, CantidadItems & " cascos!", "el casco!"), Protocol.FontTypeNames.FONTTYPE_INFO)
				ElseIf ObjData_Renamed(ItemIndex).OBJType = Declaraciones.eOBJType.otArmadura Then 
					Call WriteConsoleMsg(UserIndex, "Has construido " & IIf(CantidadItems > 1, CantidadItems & " armaduras", "la armadura!"), Protocol.FontTypeNames.FONTTYPE_INFO)
				End If
				
				
				MiObj.Amount = CantidadItems
				MiObj.ObjIndex = ItemIndex
				If Not MeterItemEnInventario(UserIndex, MiObj) Then
					Call TirarItemAlPiso(.Pos, MiObj)
				End If
				
				'Log de construcción de Items. Pablo (ToxicWaste) 10/09/07
				If ObjData_Renamed(MiObj.ObjIndex).Log = 1 Then
					Call LogDesarrollo(.name & " ha construído " & MiObj.Amount & " " & ObjData_Renamed(MiObj.ObjIndex).name)
				End If
				
				Call SubirSkill(UserIndex, Declaraciones.eSkill.Herreria, True)
				Call UpdateUserInv(True, UserIndex, 0)
				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessagePlayWave(MARTILLOHERRERO, .Pos.X, .Pos.Y))
				
				.Reputacion.PlebeRep = .Reputacion.PlebeRep + vlProleta
				If .Reputacion.PlebeRep > MAXREP Then .Reputacion.PlebeRep = MAXREP
				
				.Counters.Trabajando = .Counters.Trabajando + 1
			End If
		End With
	End Sub
	
	Public Function PuedeConstruirCarpintero(ByVal ItemIndex As Short) As Boolean
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
	
	Public Sub CarpinteroConstruirItem(ByVal UserIndex As Short, ByVal ItemIndex As Short)
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
			
			If System.Math.Round(.Stats.UserSkills(Declaraciones.eSkill.Carpinteria) \ ModCarpinteria(.clase), 0) >= ObjData_Renamed(ItemIndex).SkCarpinteria And PuedeConstruirCarpintero(ItemIndex) And .Invent.WeaponEqpObjIndex = SERRUCHO_CARPINTERO Then
				
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
				If .clase = Declaraciones.eClass.Worker Then
					'Chequeamos que tenga los puntos antes de sacarselos
					If .Stats.MinSta >= GASTO_ENERGIA_TRABAJADOR Then
						.Stats.MinSta = .Stats.MinSta - GASTO_ENERGIA_TRABAJADOR
						Call WriteUpdateSta(UserIndex)
					Else
						Call WriteConsoleMsg(UserIndex, "No tienes suficiente energía.", Protocol.FontTypeNames.FONTTYPE_INFO)
						Exit Sub
					End If
				Else
					'Chequeamos que tenga los puntos antes de sacarselos
					If .Stats.MinSta >= GASTO_ENERGIA_NO_TRABAJADOR Then
						.Stats.MinSta = .Stats.MinSta - GASTO_ENERGIA_NO_TRABAJADOR
						Call WriteUpdateSta(UserIndex)
					Else
						Call WriteConsoleMsg(UserIndex, "No tienes suficiente energía.", Protocol.FontTypeNames.FONTTYPE_INFO)
						Exit Sub
					End If
				End If
				
				Call CarpinteroQuitarMateriales(UserIndex, ItemIndex, CantidadItems)
				Call WriteConsoleMsg(UserIndex, "Has construido " & CantidadItems & IIf(CantidadItems = 1, " objeto!", " objetos!"), Protocol.FontTypeNames.FONTTYPE_INFO)
				
				MiObj.Amount = CantidadItems
				MiObj.ObjIndex = ItemIndex
				If Not MeterItemEnInventario(UserIndex, MiObj) Then
					Call TirarItemAlPiso(.Pos, MiObj)
				End If
				
				'Log de construcción de Items. Pablo (ToxicWaste) 10/09/07
				If ObjData_Renamed(MiObj.ObjIndex).Log = 1 Then
					Call LogDesarrollo(.name & " ha construído " & MiObj.Amount & " " & ObjData_Renamed(MiObj.ObjIndex).name)
				End If
				
				Call SubirSkill(UserIndex, Declaraciones.eSkill.Carpinteria, True)
				Call UpdateUserInv(True, UserIndex, 0)
				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessagePlayWave(LABUROCARPINTERO, .Pos.X, .Pos.Y))
				
				
				.Reputacion.PlebeRep = .Reputacion.PlebeRep + vlProleta
				If .Reputacion.PlebeRep > MAXREP Then .Reputacion.PlebeRep = MAXREP
				
				.Counters.Trabajando = .Counters.Trabajando + 1
				
			ElseIf .Invent.WeaponEqpObjIndex <> SERRUCHO_CARPINTERO Then 
				Call WriteConsoleMsg(UserIndex, "Debes tener equipado el serrucho para trabajar.", Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub
	
	Private Function MineralesParaLingote(ByVal Lingote As Declaraciones.iMinerales) As Short
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		Select Case Lingote
			Case Declaraciones.iMinerales.HierroCrudo
				MineralesParaLingote = 14
			Case Declaraciones.iMinerales.PlataCruda
				MineralesParaLingote = 20
			Case Declaraciones.iMinerales.OroCrudo
				MineralesParaLingote = 35
			Case Else
				MineralesParaLingote = 10000
		End Select
	End Function
	
	
	Public Sub DoLingotes(ByVal UserIndex As Short)
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
			CantidadItems = MaximoInt(1, CShort((.Stats.ELV - 4) / 5))
			
			Slot = .flags.TargetObjInvSlot
			obji = .Invent.Object_Renamed(Slot).ObjIndex
			
			While CantidadItems > 0 And Not TieneMinerales
				If .Invent.Object_Renamed(Slot).Amount >= MineralesParaLingote(obji) * CantidadItems Then
					TieneMinerales = True
				Else
					CantidadItems = CantidadItems - 1
				End If
			End While
			
			If Not TieneMinerales Or ObjData_Renamed(obji).OBJType <> Declaraciones.eOBJType.otMinerales Then
				Call WriteConsoleMsg(UserIndex, "No tienes suficientes minerales para hacer un lingote.", Protocol.FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If
			
			.Invent.Object_Renamed(Slot).Amount = .Invent.Object_Renamed(Slot).Amount - MineralesParaLingote(obji) * CantidadItems
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
			Call WriteConsoleMsg(UserIndex, "¡Has obtenido " & CantidadItems & " lingote" & IIf(CantidadItems = 1, "", "s") & "!", Protocol.FontTypeNames.FONTTYPE_INFO)
			
			.Counters.Trabajando = .Counters.Trabajando + 1
		End With
	End Sub
	
	Public Sub DoFundir(ByVal UserIndex As Short)
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
			
			Lingotes(0) = (ObjData_Renamed(.flags.TargetObjInvIndex).LingH * num) * 0.01
			Lingotes(1) = (ObjData_Renamed(.flags.TargetObjInvIndex).LingP * num) * 0.01
			Lingotes(2) = (ObjData_Renamed(.flags.TargetObjInvIndex).LingO * num) * 0.01
			
			
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
			
			Call WriteConsoleMsg(UserIndex, "¡Has obtenido el " & num & "% de los lingotes utilizados para la construcción del objeto!", Protocol.FontTypeNames.FONTTYPE_INFO)
			
			.Counters.Trabajando = .Counters.Trabajando + 1
			
		End With
		
	End Sub
	
	Public Sub DoUpgrade(ByVal UserIndex As Short, ByVal ItemIndex As Short)
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
			If .clase = Declaraciones.eClass.Worker Then
				'Chequeamos que tenga los puntos antes de sacarselos
				If .Stats.MinSta >= GASTO_ENERGIA_TRABAJADOR Then
					.Stats.MinSta = .Stats.MinSta - GASTO_ENERGIA_TRABAJADOR
					Call WriteUpdateSta(UserIndex)
				Else
					Call WriteConsoleMsg(UserIndex, "No tienes suficiente energía.", Protocol.FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If
			Else
				'Chequeamos que tenga los puntos antes de sacarselos
				If .Stats.MinSta >= GASTO_ENERGIA_NO_TRABAJADOR Then
					.Stats.MinSta = .Stats.MinSta - GASTO_ENERGIA_NO_TRABAJADOR
					Call WriteUpdateSta(UserIndex)
				Else
					Call WriteConsoleMsg(UserIndex, "No tienes suficiente energía.", Protocol.FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If
			End If
			
			If ItemUpgrade <= 0 Then Exit Sub
			If Not TieneMaterialesUpgrade(UserIndex, ItemIndex) Then Exit Sub
			
			If PuedeConstruirHerreria(ItemUpgrade) Then
				If .Invent.WeaponEqpObjIndex <> MARTILLO_HERRERO Then
					Call WriteConsoleMsg(UserIndex, "Debes equiparte el martillo de herrero.", Protocol.FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If
				If System.Math.Round(.Stats.UserSkills(Declaraciones.eSkill.Herreria) / ModHerreriA(.clase), 0) < ObjData_Renamed(ItemUpgrade).SkHerreria Then
					Call WriteConsoleMsg(UserIndex, "No tienes suficientes skills.", Protocol.FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If
				
				Select Case ObjData_Renamed(ItemIndex).OBJType
					Case Declaraciones.eOBJType.otWeapon
						Call WriteConsoleMsg(UserIndex, "Has mejorado el arma!", Protocol.FontTypeNames.FONTTYPE_INFO)
						
					Case Declaraciones.eOBJType.otESCUDO 'Todavía no hay, pero just in case
						Call WriteConsoleMsg(UserIndex, "Has mejorado el escudo!", Protocol.FontTypeNames.FONTTYPE_INFO)
						
					Case Declaraciones.eOBJType.otCASCO
						Call WriteConsoleMsg(UserIndex, "Has mejorado el casco!", Protocol.FontTypeNames.FONTTYPE_INFO)
						
					Case Declaraciones.eOBJType.otArmadura
						Call WriteConsoleMsg(UserIndex, "Has mejorado la armadura!", Protocol.FontTypeNames.FONTTYPE_INFO)
				End Select
				
				Call SubirSkill(UserIndex, Declaraciones.eSkill.Herreria, True)
				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessagePlayWave(MARTILLOHERRERO, .Pos.X, .Pos.Y))
				
			ElseIf PuedeConstruirCarpintero(ItemUpgrade) Then 
				If .Invent.WeaponEqpObjIndex <> SERRUCHO_CARPINTERO Then
					Call WriteConsoleMsg(UserIndex, "Debes equiparte el serrucho.", Protocol.FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If
				If System.Math.Round(.Stats.UserSkills(Declaraciones.eSkill.Carpinteria) \ ModCarpinteria(.clase), 0) < ObjData_Renamed(ItemUpgrade).SkCarpinteria Then
					Call WriteConsoleMsg(UserIndex, "No tienes suficientes skills.", Protocol.FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If
				
				Select Case ObjData_Renamed(ItemIndex).OBJType
					Case Declaraciones.eOBJType.otFlechas
						Call WriteConsoleMsg(UserIndex, "Has mejorado la flecha!", Protocol.FontTypeNames.FONTTYPE_INFO)
						
					Case Declaraciones.eOBJType.otWeapon
						Call WriteConsoleMsg(UserIndex, "Has mejorado el arma!", Protocol.FontTypeNames.FONTTYPE_INFO)
						
					Case Declaraciones.eOBJType.otBarcos
						Call WriteConsoleMsg(UserIndex, "Has mejorado el barco!", Protocol.FontTypeNames.FONTTYPE_INFO)
				End Select
				
				Call SubirSkill(UserIndex, Declaraciones.eSkill.Carpinteria, True)
				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessagePlayWave(LABUROCARPINTERO, .Pos.X, .Pos.Y))
			Else
				Exit Sub
			End If
			
			Call QuitarMaterialesUpgrade(UserIndex, ItemIndex)
			
			MiObj.Amount = 1
			MiObj.ObjIndex = ItemUpgrade
			
			If Not MeterItemEnInventario(UserIndex, MiObj) Then
				Call TirarItemAlPiso(.Pos, MiObj)
			End If
			
			If ObjData_Renamed(ItemIndex).Log = 1 Then Call LogDesarrollo(.name & " ha mejorado el ítem " & ObjData_Renamed(ItemIndex).name & " a " & ObjData_Renamed(ItemUpgrade).name)
			
			Call UpdateUserInv(True, UserIndex, 0)
			
			.Reputacion.PlebeRep = .Reputacion.PlebeRep + vlProleta
			If .Reputacion.PlebeRep > MAXREP Then .Reputacion.PlebeRep = MAXREP
			
			.Counters.Trabajando = .Counters.Trabajando + 1
		End With
	End Sub
	
	Function ModNavegacion(ByVal clase As Declaraciones.eClass, ByVal UserIndex As Short) As Single
		'***************************************************
		'Autor: Unknown (orginal version)
		'Last Modification: 27/11/2009
		'27/11/2009: ZaMa - A worker can navigate before only if it's an expert fisher
		'12/04/2010: ZaMa - Arreglo modificador de pescador, para que navegue con 60 skills.
		'***************************************************
		Select Case clase
			Case Declaraciones.eClass.Pirat
				ModNavegacion = 1
			Case Declaraciones.eClass.Worker
				If UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Pesca) = 100 Then
					ModNavegacion = 1.71
				Else
					ModNavegacion = 2
				End If
			Case Else
				ModNavegacion = 2
		End Select
		
	End Function
	
	
	Function ModFundicion(ByVal clase As Declaraciones.eClass) As Single
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Select Case clase
			Case Declaraciones.eClass.Worker
				ModFundicion = 1
			Case Else
				ModFundicion = 3
		End Select
		
	End Function
	
	Function ModCarpinteria(ByVal clase As Declaraciones.eClass) As Short
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Select Case clase
			Case Declaraciones.eClass.Worker
				ModCarpinteria = 1
			Case Else
				ModCarpinteria = 3
		End Select
		
	End Function
	
	Function ModHerreriA(ByVal clase As Declaraciones.eClass) As Single
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		Select Case clase
			Case Declaraciones.eClass.Worker
				ModHerreriA = 1
			Case Else
				ModHerreriA = 4
		End Select
		
	End Function
	
	Function ModDomar(ByVal clase As Declaraciones.eClass) As Short
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		Select Case clase
			Case Declaraciones.eClass.Druid
				ModDomar = 6
			Case Declaraciones.eClass.Hunter
				ModDomar = 6
			Case Declaraciones.eClass.Cleric
				ModDomar = 7
			Case Else
				ModDomar = 10
		End Select
	End Function
	
	Function FreeMascotaIndex(ByVal UserIndex As Short) As Short
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
	
	Sub DoDomar(ByVal UserIndex As Short, ByVal NpcIndex As Short)
		'***************************************************
		'Author: Nacho (Integer)
		'Last Modification: 01/05/2010
		'12/15/2008: ZaMa - Limits the number of the same type of pet to 2.
		'02/03/2009: ZaMa - Las criaturas domadas en zona segura, esperan afuera (desaparecen).
		'01/05/2010: ZaMa - Agrego bonificacion 11% para domar con flauta magica.
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim puntosDomar As Short
		Dim puntosRequeridos As Short
		Dim CanStay As Boolean
		Dim petType As Short
		Dim NroPets As Short
		
		
		If Npclist(NpcIndex).MaestroUser = UserIndex Then
			Call WriteConsoleMsg(UserIndex, "Ya domaste a esa criatura.", Protocol.FontTypeNames.FONTTYPE_INFO)
			Exit Sub
		End If
		
		Dim index As Short
		With UserList(UserIndex)
			If .NroMascotas < MAXMASCOTAS Then
				
				If Npclist(NpcIndex).MaestroNpc > 0 Or Npclist(NpcIndex).MaestroUser > 0 Then
					Call WriteConsoleMsg(UserIndex, "La criatura ya tiene amo.", Protocol.FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If
				
				If Not PuedeDomarMascota(UserIndex, NpcIndex) Then
					Call WriteConsoleMsg(UserIndex, "No puedes domar más de dos criaturas del mismo tipo.", Protocol.FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If
				
				puntosDomar = CShort(.Stats.UserAtributos(Declaraciones.eAtributos.Carisma)) * CShort(.Stats.UserSkills(Declaraciones.eSkill.Domar))
				
				' 20% de bonificacion
				If .Invent.AnilloEqpObjIndex = FLAUTAELFICA Then
					puntosRequeridos = Npclist(NpcIndex).flags.Domable * 0.8
					
					' 11% de bonificacion
				ElseIf .Invent.AnilloEqpObjIndex = FLAUTAMAGICA Then 
					puntosRequeridos = Npclist(NpcIndex).flags.Domable * 0.89
					
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
					
					Call WriteConsoleMsg(UserIndex, "La criatura te ha aceptado como su amo.", Protocol.FontTypeNames.FONTTYPE_INFO)
					
					' Es zona segura?
					CanStay = (MapInfo_Renamed(.Pos.Map).Pk = True)
					
					If Not CanStay Then
						petType = Npclist(NpcIndex).Numero
						NroPets = .NroMascotas
						
						Call QuitarNPC(NpcIndex)
						
						.MascotasType(index) = petType
						.NroMascotas = NroPets
						
						Call WriteConsoleMsg(UserIndex, "No se permiten mascotas en zona segura. Éstas te esperarán afuera.", Protocol.FontTypeNames.FONTTYPE_INFO)
					End If
					
					Call SubirSkill(UserIndex, Declaraciones.eSkill.Domar, True)
					
				Else
					If Not .flags.UltimoMensaje = 5 Then
						Call WriteConsoleMsg(UserIndex, "No has logrado domar la criatura.", Protocol.FontTypeNames.FONTTYPE_INFO)
						.flags.UltimoMensaje = 5
					End If
					
					Call SubirSkill(UserIndex, Declaraciones.eSkill.Domar, False)
				End If
			Else
				Call WriteConsoleMsg(UserIndex, "No puedes controlar más criaturas.", Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
		End With
		
		Exit Sub
		
Errhandler: 
		Call LogError("Error en DoDomar. Error " & Err.Number & " : " & Err.Description)
		
	End Sub
	
	''
	' Checks if the user can tames a pet.
	'
	' @param integer userIndex The user id from who wants tame the pet.
	' @param integer NPCindex The index of the npc to tome.
	' @return boolean True if can, false if not.
	Private Function PuedeDomarMascota(ByVal UserIndex As Short, ByVal NpcIndex As Short) As Boolean
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
	
	Sub DoAdminInvisible(ByVal UserIndex As Short)
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
				Call SendData(modSendData.SendTarget.ToPCAreaButIndex, UserIndex, PrepareMessageCharacterRemove(.Char_Renamed.CharIndex))
			Else
				.flags.AdminInvisible = 0
				.flags.invisible = 0
				.flags.Oculto = 0
				.Counters.TiempoOculto = 0
				.Char_Renamed.body = .flags.OldBody
				.Char_Renamed.Head = .flags.OldHead
				
				' Solo el admin sabe que se hace visible
				Call EnviarDatosASlot(UserIndex, PrepareMessageCharacterChange(.Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading, .Char_Renamed.CharIndex, .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim, .Char_Renamed.FX, .Char_Renamed.loops, .Char_Renamed.CascoAnim))
				Call EnviarDatosASlot(UserIndex, PrepareMessageSetInvisible(.Char_Renamed.CharIndex, False))
				
				'Le mandamos el mensaje para crear el personaje a los clientes que estén cerca
				Call MakeUserChar(True, .Pos.Map, UserIndex, .Pos.Map, .Pos.X, .Pos.Y, True)
			End If
		End With
		
	End Sub
	
	Sub TratarDeHacerFogata(ByVal Map As Short, ByVal X As Short, ByVal Y As Short, ByVal UserIndex As Short)
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
			Call WriteConsoleMsg(UserIndex, "Necesitas clickear sobre leña para hacer ramitas.", Protocol.FontTypeNames.FONTTYPE_INFO)
			Exit Sub
		End If
		
		If Distancia(posMadera, UserList(UserIndex).Pos) > 2 Then
			Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos para prender la fogata.", Protocol.FontTypeNames.FONTTYPE_INFO)
			Exit Sub
		End If
		
		If UserList(UserIndex).flags.Muerto = 1 Then
			Call WriteConsoleMsg(UserIndex, "No puedes hacer fogatas estando muerto.", Protocol.FontTypeNames.FONTTYPE_INFO)
			Exit Sub
		End If
		
		If MapData(Map, X, Y).ObjInfo.Amount < 3 Then
			Call WriteConsoleMsg(UserIndex, "Necesitas por lo menos tres troncos para hacer una fogata.", Protocol.FontTypeNames.FONTTYPE_INFO)
			Exit Sub
		End If
		
		Dim SupervivenciaSkill As Byte
		
		SupervivenciaSkill = UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Supervivencia)
		
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
			Obj_Renamed.Amount = MapData(Map, X, Y).ObjInfo.Amount \ 3
			
			Call WriteConsoleMsg(UserIndex, "Has hecho " & Obj_Renamed.Amount & " fogatas.", Protocol.FontTypeNames.FONTTYPE_INFO)
			
			Call MakeObj(Obj_Renamed, Map, X, Y)
			
			'Seteamos la fogata como el nuevo TargetObj del user
			UserList(UserIndex).flags.TargetObj = FOGATA_APAG
			
			Call SubirSkill(UserIndex, Declaraciones.eSkill.Supervivencia, True)
		Else
			'[CDT 17-02-2004]
			If Not UserList(UserIndex).flags.UltimoMensaje = 10 Then
				Call WriteConsoleMsg(UserIndex, "No has podido hacer la fogata.", Protocol.FontTypeNames.FONTTYPE_INFO)
				UserList(UserIndex).flags.UltimoMensaje = 10
			End If
			'[/CDT]
			
			Call SubirSkill(UserIndex, Declaraciones.eSkill.Supervivencia, False)
		End If
		
	End Sub
	
	Public Sub DoPescar(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: 16/11/2009
		'16/11/2009: ZaMa - Implementado nuevo sistema de extraccion.
		'***************************************************
		On Error GoTo Errhandler
		
		Dim Suerte As Short
		Dim res As Short
		Dim CantidadItems As Short
		
		If UserList(UserIndex).clase = Declaraciones.eClass.Worker Then
			Call QuitarSta(UserIndex, EsfuerzoPescarPescador)
		Else
			Call QuitarSta(UserIndex, EsfuerzoPescarGeneral)
		End If
		
		Dim Skill As Short
		Skill = UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Pesca)
		Suerte = Int(-0.00125 * Skill * Skill - 0.3 * Skill + 49)
		
		res = RandomNumber(1, Suerte)
		
		Dim MiObj As Obj
		If res <= 6 Then
			
			If UserList(UserIndex).clase = Declaraciones.eClass.Worker Then
				With UserList(UserIndex)
					CantidadItems = 1 + MaximoInt(1, CShort((.Stats.ELV - 4) / 5))
				End With
				
				MiObj.Amount = RandomNumber(1, CantidadItems)
			Else
				MiObj.Amount = 1
			End If
			MiObj.ObjIndex = Pescado
			
			If Not MeterItemEnInventario(UserIndex, MiObj) Then
				Call TirarItemAlPiso(UserList(UserIndex).Pos, MiObj)
			End If
			
			Call WriteConsoleMsg(UserIndex, "¡Has pescado un lindo pez!", Protocol.FontTypeNames.FONTTYPE_INFO)
			
			Call SubirSkill(UserIndex, Declaraciones.eSkill.Pesca, True)
		Else
			'[CDT 17-02-2004]
			If Not UserList(UserIndex).flags.UltimoMensaje = 6 Then
				Call WriteConsoleMsg(UserIndex, "¡No has pescado nada!", Protocol.FontTypeNames.FONTTYPE_INFO)
				UserList(UserIndex).flags.UltimoMensaje = 6
			End If
			'[/CDT]
			
			Call SubirSkill(UserIndex, Declaraciones.eSkill.Pesca, False)
		End If
		
		UserList(UserIndex).Reputacion.PlebeRep = UserList(UserIndex).Reputacion.PlebeRep + vlProleta
		If UserList(UserIndex).Reputacion.PlebeRep > MAXREP Then UserList(UserIndex).Reputacion.PlebeRep = MAXREP
		
		UserList(UserIndex).Counters.Trabajando = UserList(UserIndex).Counters.Trabajando + 1
		
		Exit Sub
		
Errhandler: 
		Call LogError("Error en DoPescar. Error " & Err.Number & " : " & Err.Description)
	End Sub
	
	Public Sub DoPescarRed(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		On Error GoTo Errhandler
		
		Dim iSkill As Short
		Dim Suerte As Short
		Dim res As Short
		Dim EsPescador As Boolean
		
		If UserList(UserIndex).clase = Declaraciones.eClass.Worker Then
			Call QuitarSta(UserIndex, EsfuerzoPescarPescador)
			EsPescador = True
		Else
			Call QuitarSta(UserIndex, EsfuerzoPescarGeneral)
			EsPescador = False
		End If
		
		iSkill = UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Pesca)
		
		' m = (60-11)/(1-10)
		' y = mx - m*10 + 11
		
		Suerte = Int(-0.00125 * iSkill * iSkill - 0.3 * iSkill + 49)
		
		Dim MiObj As Obj
		'UPGRADE_WARNING: El límite inferior de la matriz PecesPosibles ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		Dim PecesPosibles(4) As Short
		If Suerte > 0 Then
			res = RandomNumber(1, Suerte)
			
			If res < 6 Then
				
				PecesPosibles(1) = Declaraciones.PECES_POSIBLES.PESCADO1
				PecesPosibles(2) = Declaraciones.PECES_POSIBLES.PESCADO2
				PecesPosibles(3) = Declaraciones.PECES_POSIBLES.PESCADO3
				PecesPosibles(4) = Declaraciones.PECES_POSIBLES.PESCADO4
				
				If EsPescador = True Then
					MiObj.Amount = RandomNumber(1, 5)
				Else
					MiObj.Amount = 1
				End If
				MiObj.ObjIndex = PecesPosibles(RandomNumber(LBound(PecesPosibles), UBound(PecesPosibles)))
				
				If Not MeterItemEnInventario(UserIndex, MiObj) Then
					Call TirarItemAlPiso(UserList(UserIndex).Pos, MiObj)
				End If
				
				Call WriteConsoleMsg(UserIndex, "¡Has pescado algunos peces!", Protocol.FontTypeNames.FONTTYPE_INFO)
				
				Call SubirSkill(UserIndex, Declaraciones.eSkill.Pesca, True)
			Else
				Call WriteConsoleMsg(UserIndex, "¡No has pescado nada!", Protocol.FontTypeNames.FONTTYPE_INFO)
				Call SubirSkill(UserIndex, Declaraciones.eSkill.Pesca, False)
			End If
		End If
		
		UserList(UserIndex).Reputacion.PlebeRep = UserList(UserIndex).Reputacion.PlebeRep + vlProleta
		If UserList(UserIndex).Reputacion.PlebeRep > MAXREP Then UserList(UserIndex).Reputacion.PlebeRep = MAXREP
		
		Exit Sub
		
Errhandler: 
		Call LogError("Error en DoPescarRed")
	End Sub
	
	''
	' Try to steal an item / gold to another character
	'
	' @param LadrOnIndex Specifies reference to user that stoles
	' @param VictimaIndex Specifies reference to user that is being stolen
	
	Public Sub DoRobar(ByVal LadrOnIndex As Short, ByVal VictimaIndex As Short)
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
		
		On Error GoTo Errhandler
		
		If Not MapInfo_Renamed(UserList(VictimaIndex).Pos.Map).Pk Then Exit Sub
		
		If UserList(LadrOnIndex).flags.Seguro Then
			If Not criminal(VictimaIndex) Then
				Call WriteConsoleMsg(LadrOnIndex, "Debes quitarte el seguro para robarle a un ciudadano.", Protocol.FontTypeNames.FONTTYPE_FIGHT)
				Exit Sub
			End If
		Else
			If UserList(LadrOnIndex).Faccion.ArmadaReal = 1 Then
				Call WriteConsoleMsg(LadrOnIndex, "Los miembros del ejército real no tienen permitido robarle a ciudadanos.", Protocol.FontTypeNames.FONTTYPE_FIGHT)
				Exit Sub
			End If
		End If
		
		If TriggerZonaPelea(LadrOnIndex, VictimaIndex) <> Declaraciones.eTrigger6.TRIGGER6_AUSENTE Then Exit Sub
		
		
		Dim GuantesHurto As Boolean
		Dim Suerte As Short
		Dim res As Short
		Dim RobarSkill As Byte
		Dim N As Short
		With UserList(LadrOnIndex)
			
			' Caos robando a caos?
			If UserList(VictimaIndex).Faccion.FuerzasCaos = 1 And .Faccion.FuerzasCaos = 1 Then
				Call WriteConsoleMsg(LadrOnIndex, "No puedes robar a otros miembros de la legión oscura.", Protocol.FontTypeNames.FONTTYPE_FIGHT)
				Exit Sub
			End If
			
			' Tiene energia?
			If .Stats.MinSta < 15 Then
				If .Genero = Declaraciones.eGenero.Hombre Then
					Call WriteConsoleMsg(LadrOnIndex, "Estás muy cansado para robar.", Protocol.FontTypeNames.FONTTYPE_INFO)
				Else
					Call WriteConsoleMsg(LadrOnIndex, "Estás muy cansada para robar.", Protocol.FontTypeNames.FONTTYPE_INFO)
				End If
				
				Exit Sub
			End If
			
			' Quito energia
			Call QuitarSta(LadrOnIndex, 15)
			
			
			If .Invent.AnilloEqpObjIndex = GUANTE_HURTO Then GuantesHurto = True
			
			If UserList(VictimaIndex).flags.Privilegios And Declaraciones.PlayerType.User Then
				
				
				RobarSkill = .Stats.UserSkills(Declaraciones.eSkill.Robar)
				
				If RobarSkill <= 10 And RobarSkill >= -1 Then
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
					
					If (RandomNumber(1, 50) < 25) And (.clase = Declaraciones.eClass.Thief) Then
						If TieneObjetosRobables(VictimaIndex) Then
							Call RobarObjeto(LadrOnIndex, VictimaIndex)
						Else
							Call WriteConsoleMsg(LadrOnIndex, UserList(VictimaIndex).name & " no tiene objetos.", Protocol.FontTypeNames.FONTTYPE_INFO)
						End If
					Else 'Roba oro
						If UserList(VictimaIndex).Stats.GLD > 0 Then
							
							If .clase = Declaraciones.eClass.Thief Then
								' Si no tine puestos los guantes de hurto roba un 50% menos. Pablo (ToxicWaste)
								If GuantesHurto Then
									N = RandomNumber(.Stats.ELV * 50, .Stats.ELV * 100)
								Else
									N = RandomNumber(.Stats.ELV * 25, .Stats.ELV * 50)
								End If
							Else
								N = RandomNumber(1, 100)
							End If
							If N > UserList(VictimaIndex).Stats.GLD Then N = UserList(VictimaIndex).Stats.GLD
							UserList(VictimaIndex).Stats.GLD = UserList(VictimaIndex).Stats.GLD - N
							
							.Stats.GLD = .Stats.GLD + N
							If .Stats.GLD > MAXORO Then .Stats.GLD = MAXORO
							
							Call WriteConsoleMsg(LadrOnIndex, "Le has robado " & N & " monedas de oro a " & UserList(VictimaIndex).name, Protocol.FontTypeNames.FONTTYPE_INFO)
							Call WriteUpdateGold(LadrOnIndex) 'Le actualizamos la billetera al ladron
							
							Call WriteUpdateGold(VictimaIndex) 'Le actualizamos la billetera a la victima
							Call FlushBuffer(VictimaIndex)
						Else
							Call WriteConsoleMsg(LadrOnIndex, UserList(VictimaIndex).name & " no tiene oro.", Protocol.FontTypeNames.FONTTYPE_INFO)
						End If
					End If
					
					Call SubirSkill(LadrOnIndex, Declaraciones.eSkill.Robar, True)
				Else
					Call WriteConsoleMsg(LadrOnIndex, "¡No has logrado robar nada!", Protocol.FontTypeNames.FONTTYPE_INFO)
					Call WriteConsoleMsg(VictimaIndex, "¡" & .name & " ha intentado robarte!", Protocol.FontTypeNames.FONTTYPE_INFO)
					Call WriteConsoleMsg(VictimaIndex, "¡" & .name & " es un criminal!", Protocol.FontTypeNames.FONTTYPE_INFO)
					Call FlushBuffer(VictimaIndex)
					
					Call SubirSkill(LadrOnIndex, Declaraciones.eSkill.Robar, False)
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
		
		Exit Sub
		
Errhandler: 
		Call LogError("Error en DoRobar. Error " & Err.Number & " : " & Err.Description)
		
	End Sub
	
	''
	' Check if one item is stealable
	'
	' @param VictimaIndex Specifies reference to victim
	' @param Slot Specifies reference to victim's inventory slot
	' @return If the item is stealable
	Public Function ObjEsRobable(ByVal VictimaIndex As Short, ByVal Slot As Short) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		' Agregué los barcos
		' Esta funcion determina qué objetos son robables.
		'***************************************************
		
		Dim OI As Short
		
		OI = UserList(VictimaIndex).Invent.Object_Renamed(Slot).ObjIndex
		
		ObjEsRobable = ObjData_Renamed(OI).OBJType <> Declaraciones.eOBJType.otLlaves And UserList(VictimaIndex).Invent.Object_Renamed(Slot).Equipped = 0 And ObjData_Renamed(OI).Real = 0 And ObjData_Renamed(OI).Caos = 0 And ObjData_Renamed(OI).OBJType <> Declaraciones.eOBJType.otBarcos
		
	End Function
	
	''
	' Try to steal an item to another character
	'
	' @param LadrOnIndex Specifies reference to user that stoles
	' @param VictimaIndex Specifies reference to user that is being stolen
	Public Sub RobarObjeto(ByVal LadrOnIndex As Short, ByVal VictimaIndex As Short)
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
			num = MaximoInt(1, RandomNumber(ObjAmount * 0.05, ObjAmount * 0.1))
			
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
			
			If UserList(LadrOnIndex).clase = Declaraciones.eClass.Thief Then
				Call WriteConsoleMsg(LadrOnIndex, "Has robado " & MiObj.Amount & " " & ObjData_Renamed(MiObj.ObjIndex).name, Protocol.FontTypeNames.FONTTYPE_INFO)
			Else
				Call WriteConsoleMsg(LadrOnIndex, "Has hurtado " & MiObj.Amount & " " & ObjData_Renamed(MiObj.ObjIndex).name, Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
		Else
			Call WriteConsoleMsg(LadrOnIndex, "No has logrado robar ningún objeto.", Protocol.FontTypeNames.FONTTYPE_INFO)
		End If
		
		'If exiting, cancel de quien es robado
		Call CancelExit(VictimaIndex)
		
	End Sub
	
	Public Sub DoApuñalar(ByVal UserIndex As Short, ByVal VictimNpcIndex As Short, ByVal VictimUserIndex As Short, ByVal daño As Short)
		'***************************************************
		'Autor: Nacho (Integer) & Unknown (orginal version)
		'Last Modification: 04/17/08 - (NicoNZ)
		'Simplifique la cuenta que hacia para sacar la suerte
		'y arregle la cuenta que hacia para sacar el daño
		'***************************************************
		Dim Suerte As Short
		Dim Skill As Short
		
		Skill = UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Apuñalar)
		
		Select Case UserList(UserIndex).clase
			Case Declaraciones.eClass.Assasin
				Suerte = Int(((0.00003 * Skill - 0.002) * Skill + 0.098) * Skill + 4.25)
				
			Case Declaraciones.eClass.Cleric, Declaraciones.eClass.Paladin, Declaraciones.eClass.Pirat
				Suerte = Int(((0.000003 * Skill + 0.0006) * Skill + 0.0107) * Skill + 4.93)
				
			Case Declaraciones.eClass.Bard
				Suerte = Int(((0.000002 * Skill + 0.0002) * Skill + 0.032) * Skill + 4.81)
				
			Case Else
				Suerte = Int(0.0361 * Skill + 4.39)
		End Select
		
		
		If RandomNumber(0, 100) < Suerte Then
			If VictimUserIndex <> 0 Then
				If UserList(UserIndex).clase = Declaraciones.eClass.Assasin Then
					daño = System.Math.Round(daño * 1.4, 0)
				Else
					daño = System.Math.Round(daño * 1.5, 0)
				End If
				
				UserList(VictimUserIndex).Stats.MinHp = UserList(VictimUserIndex).Stats.MinHp - daño
				Call WriteConsoleMsg(UserIndex, "Has apuñalado a " & UserList(VictimUserIndex).name & " por " & daño, Protocol.FontTypeNames.FONTTYPE_FIGHT)
				Call WriteConsoleMsg(VictimUserIndex, "Te ha apuñalado " & UserList(UserIndex).name & " por " & daño, Protocol.FontTypeNames.FONTTYPE_FIGHT)
				
				Call FlushBuffer(VictimUserIndex)
			Else
				Npclist(VictimNpcIndex).Stats.MinHp = Npclist(VictimNpcIndex).Stats.MinHp - Int(daño * 2)
				Call WriteConsoleMsg(UserIndex, "Has apuñalado la criatura por " & Int(daño * 2), Protocol.FontTypeNames.FONTTYPE_FIGHT)
				'[Alejo]
				Call CalcularDarExp(UserIndex, VictimNpcIndex, daño * 2)
			End If
			
			Call SubirSkill(UserIndex, Declaraciones.eSkill.Apuñalar, True)
		Else
			Call WriteConsoleMsg(UserIndex, "¡No has logrado apuñalar a tu enemigo!", Protocol.FontTypeNames.FONTTYPE_FIGHT)
			Call SubirSkill(UserIndex, Declaraciones.eSkill.Apuñalar, False)
		End If
		
	End Sub
	
	Public Sub DoAcuchillar(ByVal UserIndex As Short, ByVal VictimNpcIndex As Short, ByVal VictimUserIndex As Short, ByVal daño As Short)
		'***************************************************
		'Autor: ZaMa
		'Last Modification: 12/01/2010
		'***************************************************
		
		If UserList(UserIndex).clase <> Declaraciones.eClass.Pirat Then Exit Sub
		If UserList(UserIndex).Invent.WeaponEqpSlot = 0 Then Exit Sub
		
		If RandomNumber(0, 100) < PROB_ACUCHILLAR Then
			daño = Int(daño * DAÑO_ACUCHILLAR)
			
			If VictimUserIndex <> 0 Then
				UserList(VictimUserIndex).Stats.MinHp = UserList(VictimUserIndex).Stats.MinHp - daño
				Call WriteConsoleMsg(UserIndex, "Has acuchillado a " & UserList(VictimUserIndex).name & " por " & daño, Protocol.FontTypeNames.FONTTYPE_FIGHT)
				Call WriteConsoleMsg(VictimUserIndex, UserList(UserIndex).name & " te ha acuchillado por " & daño, Protocol.FontTypeNames.FONTTYPE_FIGHT)
			Else
				Npclist(VictimNpcIndex).Stats.MinHp = Npclist(VictimNpcIndex).Stats.MinHp - daño
				Call WriteConsoleMsg(UserIndex, "Has acuchillado a la criatura por " & daño, Protocol.FontTypeNames.FONTTYPE_FIGHT)
				Call CalcularDarExp(UserIndex, VictimNpcIndex, daño)
			End If
		End If
		
	End Sub
	
	Public Sub DoGolpeCritico(ByVal UserIndex As Short, ByVal VictimNpcIndex As Short, ByVal VictimUserIndex As Short, ByVal daño As Short)
		'***************************************************
		'Autor: Pablo (ToxicWaste)
		'Last Modification: 28/01/2007
		'***************************************************
		Dim Suerte As Short
		Dim Skill As Short
		
		If UserList(UserIndex).clase <> Declaraciones.eClass.Bandit Then Exit Sub
		If UserList(UserIndex).Invent.WeaponEqpSlot = 0 Then Exit Sub
		If ObjData_Renamed(UserList(UserIndex).Invent.WeaponEqpObjIndex).name <> "Espada Vikinga" Then Exit Sub
		
		
		Skill = UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Wrestling)
		
		Suerte = Int((((0.00000003 * Skill + 0.000006) * Skill + 0.000107) * Skill + 0.0893) * 100)
		
		If RandomNumber(0, 100) < Suerte Then
			daño = Int(daño * 0.75)
			If VictimUserIndex <> 0 Then
				UserList(VictimUserIndex).Stats.MinHp = UserList(VictimUserIndex).Stats.MinHp - daño
				Call WriteConsoleMsg(UserIndex, "Has golpeado críticamente a " & UserList(VictimUserIndex).name & " por " & daño & ".", Protocol.FontTypeNames.FONTTYPE_FIGHT)
				Call WriteConsoleMsg(VictimUserIndex, UserList(UserIndex).name & " te ha golpeado críticamente por " & daño & ".", Protocol.FontTypeNames.FONTTYPE_FIGHT)
			Else
				Npclist(VictimNpcIndex).Stats.MinHp = Npclist(VictimNpcIndex).Stats.MinHp - daño
				Call WriteConsoleMsg(UserIndex, "Has golpeado críticamente a la criatura por " & daño & ".", Protocol.FontTypeNames.FONTTYPE_FIGHT)
				'[Alejo]
				Call CalcularDarExp(UserIndex, VictimNpcIndex, daño)
			End If
		End If
		
	End Sub
	
	Public Sub QuitarSta(ByVal UserIndex As Short, ByVal Cantidad As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		UserList(UserIndex).Stats.MinSta = UserList(UserIndex).Stats.MinSta - Cantidad
		If UserList(UserIndex).Stats.MinSta < 0 Then UserList(UserIndex).Stats.MinSta = 0
		Call WriteUpdateSta(UserIndex)
		
		Exit Sub
		
Errhandler: 
		Call LogError("Error en QuitarSta. Error " & Err.Number & " : " & Err.Description)
		
	End Sub
	
	Public Sub DoTalar(ByVal UserIndex As Short, Optional ByVal DarMaderaElfica As Boolean = False)
		'***************************************************
		'Autor: Unknown
		'Last Modification: 16/11/2009
		'16/11/2009: ZaMa - Ahora Se puede dar madera elfica.
		'16/11/2009: ZaMa - Implementado nuevo sistema de extraccion.
		'***************************************************
		On Error GoTo Errhandler
		
		Dim Suerte As Short
		Dim res As Short
		Dim CantidadItems As Short
		
		If UserList(UserIndex).clase = Declaraciones.eClass.Worker Then
			Call QuitarSta(UserIndex, EsfuerzoTalarLeñador)
		Else
			Call QuitarSta(UserIndex, EsfuerzoTalarGeneral)
		End If
		
		Dim Skill As Short
		Skill = UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Talar)
		Suerte = Int(-0.00125 * Skill * Skill - 0.3 * Skill + 49)
		
		res = RandomNumber(1, Suerte)
		
		Dim MiObj As Obj
		If res <= 6 Then
			
			If UserList(UserIndex).clase = Declaraciones.eClass.Worker Then
				With UserList(UserIndex)
					CantidadItems = 1 + MaximoInt(1, CShort((.Stats.ELV - 4) / 5))
				End With
				
				MiObj.Amount = RandomNumber(1, CantidadItems)
			Else
				MiObj.Amount = 1
			End If
			
			MiObj.ObjIndex = IIf(DarMaderaElfica, LeñaElfica, Leña)
			
			
			If Not MeterItemEnInventario(UserIndex, MiObj) Then
				
				Call TirarItemAlPiso(UserList(UserIndex).Pos, MiObj)
				
			End If
			
			Call WriteConsoleMsg(UserIndex, "¡Has conseguido algo de leña!", Protocol.FontTypeNames.FONTTYPE_INFO)
			
			Call SubirSkill(UserIndex, Declaraciones.eSkill.Talar, True)
		Else
			'[CDT 17-02-2004]
			If Not UserList(UserIndex).flags.UltimoMensaje = 8 Then
				Call WriteConsoleMsg(UserIndex, "¡No has obtenido leña!", Protocol.FontTypeNames.FONTTYPE_INFO)
				UserList(UserIndex).flags.UltimoMensaje = 8
			End If
			'[/CDT]
			Call SubirSkill(UserIndex, Declaraciones.eSkill.Talar, False)
		End If
		
		UserList(UserIndex).Reputacion.PlebeRep = UserList(UserIndex).Reputacion.PlebeRep + vlProleta
		If UserList(UserIndex).Reputacion.PlebeRep > MAXREP Then UserList(UserIndex).Reputacion.PlebeRep = MAXREP
		
		UserList(UserIndex).Counters.Trabajando = UserList(UserIndex).Counters.Trabajando + 1
		
		Exit Sub
		
Errhandler: 
		Call LogError("Error en DoTalar")
		
	End Sub
	
	Public Sub DoMineria(ByVal UserIndex As Short)
		'***************************************************
		'Autor: Unknown
		'Last Modification: 16/11/2009
		'16/11/2009: ZaMa - Implementado nuevo sistema de extraccion.
		'***************************************************
		On Error GoTo Errhandler
		
		Dim Suerte As Short
		Dim res As Short
		Dim CantidadItems As Short
		
		Dim Skill As Short
		Dim MiObj As Obj
		With UserList(UserIndex)
			If .clase = Declaraciones.eClass.Worker Then
				Call QuitarSta(UserIndex, EsfuerzoExcavarMinero)
			Else
				Call QuitarSta(UserIndex, EsfuerzoExcavarGeneral)
			End If
			
			Skill = .Stats.UserSkills(Declaraciones.eSkill.Mineria)
			Suerte = Int(-0.00125 * Skill * Skill - 0.3 * Skill + 49)
			
			res = RandomNumber(1, Suerte)
			
			If res <= 5 Then
				
				If .flags.TargetObj = 0 Then Exit Sub
				
				MiObj.ObjIndex = ObjData_Renamed(.flags.TargetObj).MineralIndex
				
				If UserList(UserIndex).clase = Declaraciones.eClass.Worker Then
					CantidadItems = 1 + MaximoInt(1, CShort((.Stats.ELV - 4) / 5))
					
					MiObj.Amount = RandomNumber(1, CantidadItems)
				Else
					MiObj.Amount = 1
				End If
				
				If Not MeterItemEnInventario(UserIndex, MiObj) Then Call TirarItemAlPiso(.Pos, MiObj)
				
				Call WriteConsoleMsg(UserIndex, "¡Has extraido algunos minerales!", Protocol.FontTypeNames.FONTTYPE_INFO)
				
				Call SubirSkill(UserIndex, Declaraciones.eSkill.Mineria, True)
			Else
				'[CDT 17-02-2004]
				If Not .flags.UltimoMensaje = 9 Then
					Call WriteConsoleMsg(UserIndex, "¡No has conseguido nada!", Protocol.FontTypeNames.FONTTYPE_INFO)
					.flags.UltimoMensaje = 9
				End If
				'[/CDT]
				Call SubirSkill(UserIndex, Declaraciones.eSkill.Mineria, False)
			End If
			
			.Reputacion.PlebeRep = .Reputacion.PlebeRep + vlProleta
			If .Reputacion.PlebeRep > MAXREP Then .Reputacion.PlebeRep = MAXREP
			
			.Counters.Trabajando = UserList(UserIndex).Counters.Trabajando + 1
		End With
		
		Exit Sub
		
Errhandler: 
		Call LogError("Error en Sub DoMineria")
		
	End Sub
	
	Public Sub DoMeditar(ByVal UserIndex As Short)
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
				Call WriteConsoleMsg(UserIndex, "Has terminado de meditar.", Protocol.FontTypeNames.FONTTYPE_INFO)
				Call WriteMeditateToggle(UserIndex)
				.flags.Meditando = False
				.Char_Renamed.FX = 0
				.Char_Renamed.loops = 0
				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageCreateFX(.Char_Renamed.CharIndex, 0, 0))
				Exit Sub
			End If
			
			MeditarSkill = .Stats.UserSkills(Declaraciones.eSkill.Meditar)
			
			If MeditarSkill <= 10 And MeditarSkill >= -1 Then
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
					Call WriteConsoleMsg(UserIndex, "¡Has recuperado " & cant & " puntos de maná!", Protocol.FontTypeNames.FONTTYPE_INFO)
					.flags.UltimoMensaje = 22
				End If
				
				Call WriteUpdateMana(UserIndex)
				Call SubirSkill(UserIndex, Declaraciones.eSkill.Meditar, True)
			Else
				Call SubirSkill(UserIndex, Declaraciones.eSkill.Meditar, False)
			End If
		End With
	End Sub
	
	Public Sub DoDesequipar(ByVal UserIndex As Short, ByVal VictimIndex As Short)
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
			
			WrestlingSkill = .Stats.UserSkills(Declaraciones.eSkill.Wrestling)
			
			Probabilidad = WrestlingSkill * 0.2 + .Stats.ELV * 0.66
		End With
		
		With UserList(VictimIndex)
			' Si tiene escudo, intenta desequiparlo
			If .Invent.EscudoEqpObjIndex > 0 Then
				
				Resultado = RandomNumber(1, 100)
				
				If Resultado <= Probabilidad Then
					' Se lo desequipo
					Call Desequipar(VictimIndex, .Invent.EscudoEqpSlot)
					
					Call WriteConsoleMsg(UserIndex, "Has logrado desequipar el escudo de tu oponente!", Protocol.FontTypeNames.FONTTYPE_FIGHT)
					
					If .Stats.ELV < 20 Then
						Call WriteConsoleMsg(VictimIndex, "¡Tu oponente te ha desequipado el escudo!", Protocol.FontTypeNames.FONTTYPE_FIGHT)
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
					
					Call WriteConsoleMsg(UserIndex, "Has logrado desarmar a tu oponente!", Protocol.FontTypeNames.FONTTYPE_FIGHT)
					
					If .Stats.ELV < 20 Then
						Call WriteConsoleMsg(VictimIndex, "¡Tu oponente te ha desarmado!", Protocol.FontTypeNames.FONTTYPE_FIGHT)
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
					
					Call WriteConsoleMsg(UserIndex, "Has logrado desequipar el casco de tu oponente!", Protocol.FontTypeNames.FONTTYPE_FIGHT)
					
					If .Stats.ELV < 20 Then
						Call WriteConsoleMsg(VictimIndex, "¡Tu oponente te ha desequipado el casco!", Protocol.FontTypeNames.FONTTYPE_FIGHT)
					End If
					
					Call FlushBuffer(VictimIndex)
					
					Exit Sub
				End If
				
				AlgoEquipado = True
			End If
			
			If AlgoEquipado Then
				Call WriteConsoleMsg(UserIndex, "Tu oponente no tiene equipado items!", Protocol.FontTypeNames.FONTTYPE_FIGHT)
			Else
				Call WriteConsoleMsg(UserIndex, "No has logrado desequipar ningún item a tu oponente!", Protocol.FontTypeNames.FONTTYPE_FIGHT)
			End If
			
		End With
		
		
	End Sub
	
	Public Sub DoHurtar(ByVal UserIndex As Short, ByVal VictimaIndex As Short)
		'***************************************************
		'Author: Pablo (ToxicWaste)
		'Last Modif: 03/03/2010
		'Implements the pick pocket skill of the Bandit :)
		'03/03/2010 - Pato: Sólo se puede hurtar si no está en trigger 6 :)
		'***************************************************
		If TriggerZonaPelea(UserIndex, VictimaIndex) <> Declaraciones.eTrigger6.TRIGGER6_AUSENTE Then Exit Sub
		
		If UserList(UserIndex).clase <> Declaraciones.eClass.Bandit Then Exit Sub
		'Esto es precario y feo, pero por ahora no se me ocurrió nada mejor.
		'Uso el slot de los anillos para "equipar" los guantes.
		'Y los reconozco porque les puse DefensaMagicaMin y Max = 0
		If UserList(UserIndex).Invent.AnilloEqpObjIndex <> GUANTE_HURTO Then Exit Sub
		
		Dim res As Short
		res = RandomNumber(1, 100)
		If (res < 20) Then
			If TieneObjetosRobables(VictimaIndex) Then
				Call RobarObjeto(UserIndex, VictimaIndex)
				Call WriteConsoleMsg(VictimaIndex, "¡" & UserList(UserIndex).name & " es un Bandido!", Protocol.FontTypeNames.FONTTYPE_INFO)
			Else
				Call WriteConsoleMsg(UserIndex, UserList(VictimaIndex).name & " no tiene objetos.", Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
		End If
		
	End Sub
	
	Public Sub DoHandInmo(ByVal UserIndex As Short, ByVal VictimaIndex As Short)
		'***************************************************
		'Author: Pablo (ToxicWaste)
		'Last Modif: 17/02/2007
		'Implements the special Skill of the Thief
		'***************************************************
		If UserList(VictimaIndex).flags.Paralizado = 1 Then Exit Sub
		If UserList(UserIndex).clase <> Declaraciones.eClass.Thief Then Exit Sub
		
		
		If UserList(UserIndex).Invent.AnilloEqpObjIndex <> GUANTE_HURTO Then Exit Sub
		
		Dim res As Short
		res = RandomNumber(0, 100)
		If res < (UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Wrestling) / 4) Then
			UserList(VictimaIndex).flags.Paralizado = 1
			UserList(VictimaIndex).Counters.Paralisis = IntervaloParalizado / 2
			Call WriteParalizeOK(VictimaIndex)
			Call WriteConsoleMsg(UserIndex, "Tu golpe ha dejado inmóvil a tu oponente", Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(VictimaIndex, "¡El golpe te ha dejado inmóvil!", Protocol.FontTypeNames.FONTTYPE_INFO)
		End If
		
	End Sub
	
	Public Sub Desarmar(ByVal UserIndex As Short, ByVal VictimIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: 02/04/2010 (ZaMa)
		'02/04/2010: ZaMa - Nueva formula para desarmar.
		'***************************************************
		
		Dim Probabilidad As Short
		Dim Resultado As Short
		Dim WrestlingSkill As Byte
		
		With UserList(UserIndex)
			WrestlingSkill = .Stats.UserSkills(Declaraciones.eSkill.Wrestling)
			
			Probabilidad = WrestlingSkill * 0.2 + .Stats.ELV * 0.66
			
			Resultado = RandomNumber(1, 100)
			
			If Resultado <= Probabilidad Then
				Call Desequipar(VictimIndex, UserList(VictimIndex).Invent.WeaponEqpSlot)
				Call WriteConsoleMsg(UserIndex, "Has logrado desarmar a tu oponente!", Protocol.FontTypeNames.FONTTYPE_FIGHT)
				If UserList(VictimIndex).Stats.ELV < 20 Then
					Call WriteConsoleMsg(VictimIndex, "¡Tu oponente te ha desarmado!", Protocol.FontTypeNames.FONTTYPE_FIGHT)
				End If
				Call FlushBuffer(VictimIndex)
			End If
		End With
		
	End Sub
	
	
	Public Function MaxItemsConstruibles(ByVal UserIndex As Short) As Short
		'***************************************************
		'Author: ZaMa
		'Last Modification: 29/01/2010
		'
		'***************************************************
		MaxItemsConstruibles = MaximoInt(1, CShort((UserList(UserIndex).Stats.ELV - 4) / 5))
	End Function
End Module
