Option Strict Off
Option Explicit On
Module mdlCOmercioConUsuario
	'**************************************************************
	' mdlComercioConUsuarios.bas - Allows players to commerce between themselves.
	'
	' Designed and implemented by Alejandro Santos (AlejoLP)
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
	
	'[Alejo]
	
	Private Const MAX_ORO_LOGUEABLE As Integer = 50000
	Private Const MAX_OBJ_LOGUEABLE As Integer = 1000
	
	Public Const MAX_OFFER_SLOTS As Short = 30 '20
	Public Const GOLD_OFFER_SLOT As Short = MAX_OFFER_SLOTS + 1
	
	Public Structure tCOmercioUsuario
		Dim DestUsu As Short 'El otro Usuario
		Dim DestNick As String
		<VBFixedArray(MAX_OFFER_SLOTS)> Dim Objeto() As Short 'Indice de los objetos que se desea dar
		Dim GoldAmount As Integer
		<VBFixedArray(MAX_OFFER_SLOTS)> Dim cant() As Integer 'Cuantos objetos desea dar
		Dim Acepto As Boolean
		Dim Confirmo As Boolean
		
		'UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
		Public Sub Initialize()
			'UPGRADE_WARNING: El límite inferior de la matriz Objeto ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim Objeto(MAX_OFFER_SLOTS)
			'UPGRADE_WARNING: El límite inferior de la matriz cant ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim cant(MAX_OFFER_SLOTS)
		End Sub
	End Structure
	
	'origen: origen de la transaccion, originador del comando
	'destino: receptor de la transaccion
	Public Sub IniciarComercioConUsuario(ByVal Origen As Short, ByVal Destino As Short)
		'***************************************************
		'Autor: Unkown
		'Last Modification: 25/11/2009
		'
		'***************************************************
		On Error GoTo Errhandler
		
		'Si ambos pusieron /comerciar entonces
		If UserList(Origen).ComUsu.DestUsu = Destino And UserList(Destino).ComUsu.DestUsu = Origen Then
			'Actualiza el inventario del usuario
			Call UpdateUserInv(True, Origen, 0)
			'Decirle al origen que abra la ventanita.
			Call WriteUserCommerceInit(Origen)
			UserList(Origen).flags.Comerciando = True
			
			'Actualiza el inventario del usuario
			Call UpdateUserInv(True, Destino, 0)
			'Decirle al origen que abra la ventanita.
			Call WriteUserCommerceInit(Destino)
			UserList(Destino).flags.Comerciando = True
			
			'Call EnviarObjetoTransaccion(Origen)
		Else
			'Es el primero que comercia ?
			Call WriteConsoleMsg(Destino, UserList(Origen).name & " desea comerciar. Si deseas aceptar, escribe /COMERCIAR.", Protocol.FontTypeNames.FONTTYPE_TALK)
			UserList(Destino).flags.TargetUser = Origen
			
		End If
		
		Call FlushBuffer(Destino)
		
		Exit Sub
Errhandler: 
		Call LogError("Error en IniciarComercioConUsuario: " & Err.Description)
	End Sub
	
	Public Sub EnviarOferta(ByVal UserIndex As Short, ByVal OfferSlot As Byte)
		'***************************************************
		'Autor: Unkown
		'Last Modification: 25/11/2009
		'Sends the offer change to the other trading user
		'25/11/2009: ZaMa - Implementado nuevo sistema de comercio con ofertas variables.
		'***************************************************
		Dim ObjIndex As Short
		Dim ObjAmount As Integer
		
		With UserList(UserIndex)
			If OfferSlot = GOLD_OFFER_SLOT Then
				ObjIndex = iORO
				ObjAmount = UserList(.ComUsu.DestUsu).ComUsu.GoldAmount
			Else
				ObjIndex = UserList(.ComUsu.DestUsu).ComUsu.Objeto(OfferSlot)
				ObjAmount = UserList(.ComUsu.DestUsu).ComUsu.cant(OfferSlot)
			End If
		End With
		
		Call WriteChangeUserTradeSlot(UserIndex, OfferSlot, ObjIndex, ObjAmount)
		Call FlushBuffer(UserIndex)
		
	End Sub
	
	Public Sub FinComerciarUsu(ByVal UserIndex As Short)
		'***************************************************
		'Autor: Unkown
		'Last Modification: 25/11/2009
		'25/11/2009: ZaMa - Limpio los arrays (por el nuevo sistema)
		'***************************************************
		Dim i As Integer
		
		With UserList(UserIndex)
			If .ComUsu.DestUsu > 0 Then
				Call WriteUserCommerceEnd(UserIndex)
			End If
			
			.ComUsu.Acepto = False
			.ComUsu.Confirmo = False
			.ComUsu.DestUsu = 0
			
			For i = 1 To MAX_OFFER_SLOTS
				.ComUsu.cant(i) = 0
				.ComUsu.Objeto(i) = 0
			Next i
			
			.ComUsu.GoldAmount = 0
			.ComUsu.DestNick = vbNullString
			.flags.Comerciando = False
		End With
	End Sub
	
	Public Sub AceptarComercioUsu(ByVal UserIndex As Short)
		'***************************************************
		'Autor: Unkown
		'Last Modification: 25/11/2009
		'25/11/2009: ZaMa - Ahora se traspasan hasta 5 items + oro al comerciar
		'***************************************************
		Dim TradingObj As Obj
		Dim OtroUserIndex As Short
		Dim TerminarAhora As Boolean
		Dim OfferSlot As Short
		
		UserList(UserIndex).ComUsu.Acepto = True
		
		OtroUserIndex = UserList(UserIndex).ComUsu.DestUsu
		
		If UserList(OtroUserIndex).ComUsu.Acepto = False Then
			Exit Sub
		End If
		
		' Envio los items a quien corresponde
		For OfferSlot = 1 To MAX_OFFER_SLOTS + 1
			
			' Items del 1er usuario
			With UserList(UserIndex)
				' Le pasa el oro
				If OfferSlot = GOLD_OFFER_SLOT Then
					' Quito la cantidad de oro ofrecida
					.Stats.GLD = .Stats.GLD - .ComUsu.GoldAmount
					' Log
					If .ComUsu.GoldAmount > MAX_ORO_LOGUEABLE Then Call LogDesarrollo(.name & " soltó oro en comercio seguro con " & UserList(OtroUserIndex).name & ". Cantidad: " & .ComUsu.GoldAmount)
					' Update Usuario
					Call WriteUpdateUserStats(UserIndex)
					' Se la doy al otro
					UserList(OtroUserIndex).Stats.GLD = UserList(OtroUserIndex).Stats.GLD + .ComUsu.GoldAmount
					' Update Otro Usuario
					Call WriteUpdateUserStats(OtroUserIndex)
					
					' Le pasa lo ofertado de los slots con items
				ElseIf .ComUsu.Objeto(OfferSlot) > 0 Then 
					TradingObj.ObjIndex = .ComUsu.Objeto(OfferSlot)
					TradingObj.Amount = .ComUsu.cant(OfferSlot)
					
					'Quita el objeto y se lo da al otro
					If Not MeterItemEnInventario(OtroUserIndex, TradingObj) Then
						Call TirarItemAlPiso(UserList(OtroUserIndex).Pos, TradingObj)
					End If
					
					Call QuitarObjetos(TradingObj.ObjIndex, TradingObj.Amount, UserIndex)
					
					'Es un Objeto que tenemos que loguear? Pablo (ToxicWaste) 07/09/07
					If ObjData_Renamed(TradingObj.ObjIndex).Log = 1 Then
						Call LogDesarrollo(.name & " le pasó en comercio seguro a " & UserList(OtroUserIndex).name & " " & TradingObj.Amount & " " & ObjData_Renamed(TradingObj.ObjIndex).name)
					End If
					
					'Es mucha cantidad?
					If TradingObj.Amount > MAX_OBJ_LOGUEABLE Then
						'Si no es de los prohibidos de loguear, lo logueamos.
						If ObjData_Renamed(TradingObj.ObjIndex).NoLog <> 1 Then
							Call LogDesarrollo(UserList(OtroUserIndex).name & " le pasó en comercio seguro a " & .name & " " & TradingObj.Amount & " " & ObjData_Renamed(TradingObj.ObjIndex).name)
						End If
					End If
				End If
			End With
			
			' Items del 2do usuario
			With UserList(OtroUserIndex)
				' Le pasa el oro
				If OfferSlot = GOLD_OFFER_SLOT Then
					' Quito la cantidad de oro ofrecida
					.Stats.GLD = .Stats.GLD - .ComUsu.GoldAmount
					' Log
					If .ComUsu.GoldAmount > MAX_ORO_LOGUEABLE Then Call LogDesarrollo(.name & " soltó oro en comercio seguro con " & UserList(UserIndex).name & ". Cantidad: " & .ComUsu.GoldAmount)
					' Update Usuario
					Call WriteUpdateUserStats(OtroUserIndex)
					'y se la doy al otro
					UserList(UserIndex).Stats.GLD = UserList(UserIndex).Stats.GLD + .ComUsu.GoldAmount
					If .ComUsu.GoldAmount > MAX_ORO_LOGUEABLE Then Call LogDesarrollo(UserList(UserIndex).name & " recibió oro en comercio seguro con " & .name & ". Cantidad: " & .ComUsu.GoldAmount)
					' Update Otro Usuario
					Call WriteUpdateUserStats(UserIndex)
					
					' Le pasa la oferta de los slots con items
				ElseIf .ComUsu.Objeto(OfferSlot) > 0 Then 
					TradingObj.ObjIndex = .ComUsu.Objeto(OfferSlot)
					TradingObj.Amount = .ComUsu.cant(OfferSlot)
					
					'Quita el objeto y se lo da al otro
					If Not MeterItemEnInventario(UserIndex, TradingObj) Then
						Call TirarItemAlPiso(UserList(UserIndex).Pos, TradingObj)
					End If
					
					Call QuitarObjetos(TradingObj.ObjIndex, TradingObj.Amount, OtroUserIndex)
					
					'Es un Objeto que tenemos que loguear? Pablo (ToxicWaste) 07/09/07
					If ObjData_Renamed(TradingObj.ObjIndex).Log = 1 Then
						Call LogDesarrollo(.name & " le pasó en comercio seguro a " & UserList(UserIndex).name & " " & TradingObj.Amount & " " & ObjData_Renamed(TradingObj.ObjIndex).name)
					End If
					
					'Es mucha cantidad?
					If TradingObj.Amount > MAX_OBJ_LOGUEABLE Then
						'Si no es de los prohibidos de loguear, lo logueamos.
						If ObjData_Renamed(TradingObj.ObjIndex).NoLog <> 1 Then
							Call LogDesarrollo(.name & " le pasó en comercio seguro a " & UserList(UserIndex).name & " " & TradingObj.Amount & " " & ObjData_Renamed(TradingObj.ObjIndex).name)
						End If
					End If
				End If
			End With
			
		Next OfferSlot
		
		' End Trade
		Call FinComerciarUsu(UserIndex)
		Call FinComerciarUsu(OtroUserIndex)
		
	End Sub
	
	Public Sub AgregarOferta(ByVal UserIndex As Short, ByVal OfferSlot As Byte, ByVal ObjIndex As Short, ByVal Amount As Integer, ByVal IsGold As Boolean)
		'***************************************************
		'Autor: ZaMa
		'Last Modification: 24/11/2009
		'Adds gold or items to the user's offer
		'***************************************************
		
		If PuedeSeguirComerciando(UserIndex) Then
			With UserList(UserIndex).ComUsu
				' Si ya confirmo su oferta, no puede cambiarla!
				If Not .Confirmo Then
					If IsGold Then
						' Agregamos (o quitamos) mas oro a la oferta
						.GoldAmount = .GoldAmount + Amount
						
						' Imposible que pase, pero por las dudas..
						If .GoldAmount < 0 Then .GoldAmount = 0
					Else
						' Agreamos (o quitamos) el item y su cantidad en el slot correspondiente
						' Si es 0 estoy modificando la cantidad, no agregando
						If ObjIndex > 0 Then .Objeto(OfferSlot) = ObjIndex
						.cant(OfferSlot) = .cant(OfferSlot) + Amount
						
						'Quitó todos los items de ese tipo
						If .cant(OfferSlot) <= 0 Then
							' Removemos el objeto para evitar conflictos
							.Objeto(OfferSlot) = 0
							.cant(OfferSlot) = 0
						End If
					End If
				End If
			End With
		End If
		
	End Sub
	
	Public Function PuedeSeguirComerciando(ByVal UserIndex As Short) As Boolean
		'***************************************************
		'Autor: ZaMa
		'Last Modification: 24/11/2009
		'Validates wether the conditions for the commerce to keep going are satisfied
		'***************************************************
		Dim OtroUserIndex As Short
		Dim ComercioInvalido As Boolean
		
		With UserList(UserIndex)
			' Usuario valido?
			If .ComUsu.DestUsu <= 0 Or .ComUsu.DestUsu > MaxUsers Then
				ComercioInvalido = True
			End If
			
			OtroUserIndex = .ComUsu.DestUsu
			
			If Not ComercioInvalido Then
				' Estan logueados?
				If UserList(OtroUserIndex).flags.UserLogged = False Or .flags.UserLogged = False Then
					ComercioInvalido = True
				End If
			End If
			
			If Not ComercioInvalido Then
				' Se estan comerciando el uno al otro?
				If UserList(OtroUserIndex).ComUsu.DestUsu <> UserIndex Then
					ComercioInvalido = True
				End If
			End If
			
			If Not ComercioInvalido Then
				' El nombre del otro es el mismo que al que le comercio?
				If UserList(OtroUserIndex).name <> .ComUsu.DestNick Then
					ComercioInvalido = True
				End If
			End If
			
			If Not ComercioInvalido Then
				' Mi nombre  es el mismo que al que el le comercia?
				If .name <> UserList(OtroUserIndex).ComUsu.DestNick Then
					ComercioInvalido = True
				End If
			End If
			
			If Not ComercioInvalido Then
				' Esta vivo?
				If UserList(OtroUserIndex).flags.Muerto = 1 Then
					ComercioInvalido = True
				End If
			End If
			
			' Fin del comercio
			If ComercioInvalido = True Then
				Call FinComerciarUsu(UserIndex)
				
				If OtroUserIndex <= 0 Or OtroUserIndex > MaxUsers Then
					Call FinComerciarUsu(OtroUserIndex)
					Call Protocol.FlushBuffer(OtroUserIndex)
				End If
				
				Exit Function
			End If
		End With
		
		PuedeSeguirComerciando = True
		
	End Function
End Module