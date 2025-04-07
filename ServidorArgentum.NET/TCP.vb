Option Strict Off
Option Explicit On
Module TCP
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
	
	
	Sub DarCuerpo(ByVal UserIndex As Short)
		'*************************************************
		'Author: Nacho (Integer)
		'Last modified: 14/03/2007
		'Elije una cabeza para el usuario y le da un body
		'*************************************************
		Dim NewBody As Short
		Dim UserRaza As Byte
		Dim UserGenero As Byte
		
		UserGenero = UserList(UserIndex).Genero
		UserRaza = UserList(UserIndex).raza
		
		Select Case UserGenero
			Case Declaraciones.eGenero.Hombre
				Select Case UserRaza
					Case Declaraciones.eRaza.Humano
						NewBody = 1
					Case Declaraciones.eRaza.Elfo
						NewBody = 2
					Case Declaraciones.eRaza.Drow
						NewBody = 3
					Case Declaraciones.eRaza.Enano
						NewBody = 300
					Case Declaraciones.eRaza.Gnomo
						NewBody = 300
				End Select
			Case Declaraciones.eGenero.Mujer
				Select Case UserRaza
					Case Declaraciones.eRaza.Humano
						NewBody = 1
					Case Declaraciones.eRaza.Elfo
						NewBody = 2
					Case Declaraciones.eRaza.Drow
						NewBody = 3
					Case Declaraciones.eRaza.Gnomo
						NewBody = 300
					Case Declaraciones.eRaza.Enano
						NewBody = 300
				End Select
		End Select
		
		UserList(UserIndex).Char_Renamed.body = NewBody
	End Sub
	
	Private Function ValidarCabeza(ByVal UserRaza As Byte, ByVal UserGenero As Byte, ByVal Head As Short) As Boolean
		
		Select Case UserGenero
			Case Declaraciones.eGenero.Hombre
				Select Case UserRaza
					Case Declaraciones.eRaza.Humano
						ValidarCabeza = (Head >= HUMANO_H_PRIMER_CABEZA And Head <= HUMANO_H_ULTIMA_CABEZA)
					Case Declaraciones.eRaza.Elfo
						ValidarCabeza = (Head >= ELFO_H_PRIMER_CABEZA And Head <= ELFO_H_ULTIMA_CABEZA)
					Case Declaraciones.eRaza.Drow
						ValidarCabeza = (Head >= DROW_H_PRIMER_CABEZA And Head <= DROW_H_ULTIMA_CABEZA)
					Case Declaraciones.eRaza.Enano
						ValidarCabeza = (Head >= ENANO_H_PRIMER_CABEZA And Head <= ENANO_H_ULTIMA_CABEZA)
					Case Declaraciones.eRaza.Gnomo
						ValidarCabeza = (Head >= GNOMO_H_PRIMER_CABEZA And Head <= GNOMO_H_ULTIMA_CABEZA)
				End Select
				
			Case Declaraciones.eGenero.Mujer
				Select Case UserRaza
					Case Declaraciones.eRaza.Humano
						ValidarCabeza = (Head >= HUMANO_M_PRIMER_CABEZA And Head <= HUMANO_M_ULTIMA_CABEZA)
					Case Declaraciones.eRaza.Elfo
						ValidarCabeza = (Head >= ELFO_M_PRIMER_CABEZA And Head <= ELFO_M_ULTIMA_CABEZA)
					Case Declaraciones.eRaza.Drow
						ValidarCabeza = (Head >= DROW_M_PRIMER_CABEZA And Head <= DROW_M_ULTIMA_CABEZA)
					Case Declaraciones.eRaza.Enano
						ValidarCabeza = (Head >= ENANO_M_PRIMER_CABEZA And Head <= ENANO_M_ULTIMA_CABEZA)
					Case Declaraciones.eRaza.Gnomo
						ValidarCabeza = (Head >= GNOMO_M_PRIMER_CABEZA And Head <= GNOMO_M_ULTIMA_CABEZA)
				End Select
		End Select
		
	End Function
	
	Function AsciiValidos(ByVal cad As String) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim car As Byte
		Dim i As Short
		
		cad = LCase(cad)
		
		For i = 1 To Len(cad)
			car = Asc(Mid(cad, i, 1))
			
			If (car < 97 Or car > 122) And (car <> 255) And (car <> 32) Then
				AsciiValidos = False
				Exit Function
			End If
			
		Next i
		
		AsciiValidos = True
		
	End Function
	
	Function Numeric(ByVal cad As String) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim car As Byte
		Dim i As Short
		
		cad = LCase(cad)
		
		For i = 1 To Len(cad)
			car = Asc(Mid(cad, i, 1))
			
			If (car < 48 Or car > 57) Then
				Numeric = False
				Exit Function
			End If
			
		Next i
		
		Numeric = True
		
	End Function
	
	
	Function NombrePermitido(ByVal Nombre As String) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim i As Short
		
		For i = 1 To UBound(ForbidenNames)
			If InStr(Nombre, ForbidenNames(i)) Then
				NombrePermitido = False
				Exit Function
			End If
		Next i
		
		NombrePermitido = True
		
	End Function
	
	Function ValidateSkills(ByVal UserIndex As Short) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim LoopC As Short
		
		For LoopC = 1 To NUMSKILLS
			If UserList(UserIndex).Stats.UserSkills(LoopC) < 0 Then
				Exit Function
				If UserList(UserIndex).Stats.UserSkills(LoopC) > 100 Then UserList(UserIndex).Stats.UserSkills(LoopC) = 100
			End If
		Next LoopC
		
		ValidateSkills = True
		
	End Function
	
	Sub ConnectNewUser(ByVal UserIndex As Short, ByRef name As String, ByRef Password As String, ByVal UserRaza As Declaraciones.eRaza, ByVal UserSexo As Declaraciones.eGenero, ByVal UserClase As Declaraciones.eClass, ByRef UserEmail As String, ByVal Hogar As Declaraciones.eCiudad, ByVal Head As Short)
		'*************************************************
		'Author: Unknown
		'Last modified: 3/12/2009
		'Conecta un nuevo Usuario
		'23/01/2007 Pablo (ToxicWaste) - Agregué ResetFaccion al crear usuario
		'24/01/2007 Pablo (ToxicWaste) - Agregué el nuevo mana inicial de los magos.
		'12/02/2007 Pablo (ToxicWaste) - Puse + 1 de const al Elfo normal.
		'20/04/2007 Pablo (ToxicWaste) - Puse -1 de fuerza al Elfo.
		'09/01/2008 Pablo (ToxicWaste) - Ahora los modificadores de Raza se controlan desde Balance.dat
		'11/19/2009: Pato - Modifico la maná inicial del bandido.
		'11/19/2009: Pato - Asigno los valores iniciales de ExpSkills y EluSkills.
		'03/12/2009: Budi - Optimización del código.
		'*************************************************
		Dim i As Integer
		
		Dim MiInt As Integer
		Dim Slot As Byte
		Dim IsPaladin As Boolean
		With UserList(UserIndex)
			
			If Not AsciiValidos(name) Or migr_LenB(name) = 0 Then
				Call WriteErrorMsg(UserIndex, "Nombre inválido.")
				Exit Sub
			End If
			
			If UserList(UserIndex).flags.UserLogged Then
				Call LogCheating("El usuario " & UserList(UserIndex).name & " ha intentado crear a " & name & " desde la IP " & UserList(UserIndex).ip)
				
				'Kick player ( and leave character inside :D )!
				Call CloseSocketSL(UserIndex)
				Call Cerrar_Usuario(UserIndex)
				
				Exit Sub
			End If
			
			'¿Existe el personaje?
			If FileExist(CharPath & UCase(name) & ".chr") = True Then
				Call WriteErrorMsg(UserIndex, "Ya existe el personaje.")
				Exit Sub
			End If
			
			'Tiró los dados antes de llegar acá??
			If .Stats.UserAtributos(Declaraciones.eAtributos.Fuerza) = 0 Then
				Call WriteErrorMsg(UserIndex, "Debe tirar los dados antes de poder crear un personaje.")
				Exit Sub
			End If
			
			If Not ValidarCabeza(UserRaza, UserSexo, Head) Then
				Call LogCheating("El usuario " & name & " ha seleccionado la cabeza " & Head & " desde la IP " & .ip)
				
				Call WriteErrorMsg(UserIndex, "Cabeza inválida, elija una cabeza seleccionable.")
				Exit Sub
			End If
			
			.flags.Muerto = 0
			.flags.Escondido = 0
			
			.Reputacion.AsesinoRep = 0
			.Reputacion.BandidoRep = 0
			.Reputacion.BurguesRep = 0
			.Reputacion.LadronesRep = 0
			.Reputacion.NobleRep = 1000
			.Reputacion.PlebeRep = 30
			
			.Reputacion.Promedio = 30 / 6
			
			
			.name = name
			.clase = UserClase
			.raza = UserRaza
			.Genero = UserSexo
			.email = UserEmail
			.Hogar = Hogar
			
			'[Pablo (Toxic Waste) 9/01/08]
			.Stats.UserAtributos(Declaraciones.eAtributos.Fuerza) = .Stats.UserAtributos(Declaraciones.eAtributos.Fuerza) + ModRaza_Renamed(UserRaza).Fuerza
			.Stats.UserAtributos(Declaraciones.eAtributos.Agilidad) = .Stats.UserAtributos(Declaraciones.eAtributos.Agilidad) + ModRaza_Renamed(UserRaza).Agilidad
			.Stats.UserAtributos(Declaraciones.eAtributos.Inteligencia) = .Stats.UserAtributos(Declaraciones.eAtributos.Inteligencia) + ModRaza_Renamed(UserRaza).Inteligencia
			.Stats.UserAtributos(Declaraciones.eAtributos.Carisma) = .Stats.UserAtributos(Declaraciones.eAtributos.Carisma) + ModRaza_Renamed(UserRaza).Carisma
			.Stats.UserAtributos(Declaraciones.eAtributos.Constitucion) = .Stats.UserAtributos(Declaraciones.eAtributos.Constitucion) + ModRaza_Renamed(UserRaza).Constitucion
			'[/Pablo (Toxic Waste)]
			
			For i = 1 To NUMSKILLS
				.Stats.UserSkills(i) = 0
				Call CheckEluSkill(UserIndex, i, True)
			Next i
			
			.Stats.SkillPts = 10
			
			.Char_Renamed.heading = Declaraciones.eHeading.SOUTH
			
			Call DarCuerpo(UserIndex)
			.Char_Renamed.Head = Head
			
			'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().OrigChar. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			.OrigChar = .Char_Renamed
			
			MiInt = RandomNumber(1, .Stats.UserAtributos(Declaraciones.eAtributos.Constitucion) \ 3)
			
			.Stats.MaxHp = 15 + MiInt
			.Stats.MinHp = 15 + MiInt
			
			MiInt = RandomNumber(1, .Stats.UserAtributos(Declaraciones.eAtributos.Agilidad) \ 6)
			If MiInt = 1 Then MiInt = 2
			
			.Stats.MaxSta = 20 * MiInt
			.Stats.MinSta = 20 * MiInt
			
			
			.Stats.MaxAGU = 100
			.Stats.MinAGU = 100
			
			.Stats.MaxHam = 100
			.Stats.MinHam = 100
			
			
			'<-----------------MANA----------------------->
			If UserClase = Declaraciones.eClass.Mage Then 'Cambio en mana inicial (ToxicWaste)
				MiInt = .Stats.UserAtributos(Declaraciones.eAtributos.Inteligencia) * 3
				.Stats.MaxMAN = MiInt
				.Stats.MinMAN = MiInt
			ElseIf UserClase = Declaraciones.eClass.Cleric Or UserClase = Declaraciones.eClass.Druid Or UserClase = Declaraciones.eClass.Bard Or UserClase = Declaraciones.eClass.Assasin Then 
				.Stats.MaxMAN = 50
				.Stats.MinMAN = 50
			ElseIf UserClase = Declaraciones.eClass.Bandit Then  'Mana Inicial del Bandido (ToxicWaste)
				.Stats.MaxMAN = 50
				.Stats.MinMAN = 50
			Else
				.Stats.MaxMAN = 0
				.Stats.MinMAN = 0
			End If
			
			If UserClase = Declaraciones.eClass.Mage Or UserClase = Declaraciones.eClass.Cleric Or UserClase = Declaraciones.eClass.Druid Or UserClase = Declaraciones.eClass.Bard Or UserClase = Declaraciones.eClass.Assasin Then
				.Stats.UserHechizos(1) = 2
				
				If UserClase = Declaraciones.eClass.Druid Then .Stats.UserHechizos(2) = 46
			End If
			
			.Stats.MaxHIT = 2
			.Stats.MinHIT = 1
			
			.Stats.GLD = 0
			
			.Stats.Exp = 0
			.Stats.ELU = 300
			.Stats.ELV = 1
			
			'???????????????? INVENTARIO ¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿
			
			IsPaladin = UserClase = Declaraciones.eClass.Paladin
			
			'Pociones Rojas (Newbie)
			Slot = 1
			.Invent.Object_Renamed(Slot).ObjIndex = 857
			.Invent.Object_Renamed(Slot).Amount = 200
			
			'Pociones azules (Newbie)
			If .Stats.MaxMAN > 0 Or IsPaladin Then
				Slot = Slot + 1
				.Invent.Object_Renamed(Slot).ObjIndex = 856
				.Invent.Object_Renamed(Slot).Amount = 200
				
			Else
				'Pociones amarillas (Newbie)
				Slot = Slot + 1
				.Invent.Object_Renamed(Slot).ObjIndex = 855
				.Invent.Object_Renamed(Slot).Amount = 100
				
				'Pociones verdes (Newbie)
				Slot = Slot + 1
				.Invent.Object_Renamed(Slot).ObjIndex = 858
				.Invent.Object_Renamed(Slot).Amount = 50
				
			End If
			
			' Ropa (Newbie)
			Slot = Slot + 1
			Select Case UserRaza
				Case Declaraciones.eRaza.Humano
					.Invent.Object_Renamed(Slot).ObjIndex = 463
				Case Declaraciones.eRaza.Elfo
					.Invent.Object_Renamed(Slot).ObjIndex = 464
				Case Declaraciones.eRaza.Drow
					.Invent.Object_Renamed(Slot).ObjIndex = 465
				Case Declaraciones.eRaza.Enano
					.Invent.Object_Renamed(Slot).ObjIndex = 466
				Case Declaraciones.eRaza.Gnomo
					.Invent.Object_Renamed(Slot).ObjIndex = 466
			End Select
			
			' Equipo ropa
			.Invent.Object_Renamed(Slot).Amount = 1
			.Invent.Object_Renamed(Slot).Equipped = 1
			
			.Invent.ArmourEqpSlot = Slot
			.Invent.ArmourEqpObjIndex = .Invent.Object_Renamed(Slot).ObjIndex
			
			'Arma (Newbie)
			Slot = Slot + 1
			Select Case UserClase
				Case Declaraciones.eClass.Hunter
					' Arco (Newbie)
					.Invent.Object_Renamed(Slot).ObjIndex = 859
				Case Declaraciones.eClass.Worker
					' Herramienta (Newbie)
					.Invent.Object_Renamed(Slot).ObjIndex = RandomNumber(561, 565)
				Case Else
					' Daga (Newbie)
					.Invent.Object_Renamed(Slot).ObjIndex = 460
			End Select
			
			' Equipo arma
			.Invent.Object_Renamed(Slot).Amount = 1
			.Invent.Object_Renamed(Slot).Equipped = 1
			
			.Invent.WeaponEqpObjIndex = .Invent.Object_Renamed(Slot).ObjIndex
			.Invent.WeaponEqpSlot = Slot
			
			.Char_Renamed.WeaponAnim = GetWeaponAnim(UserIndex, .Invent.WeaponEqpObjIndex)
			
			' Municiones (Newbie)
			If UserClase = Declaraciones.eClass.Hunter Then
				Slot = Slot + 1
				.Invent.Object_Renamed(Slot).ObjIndex = 860
				.Invent.Object_Renamed(Slot).Amount = 150
				
				' Equipo flechas
				.Invent.Object_Renamed(Slot).Equipped = 1
				.Invent.MunicionEqpSlot = Slot
				.Invent.MunicionEqpObjIndex = 860
			End If
			
			' Manzanas (Newbie)
			Slot = Slot + 1
			.Invent.Object_Renamed(Slot).ObjIndex = 467
			.Invent.Object_Renamed(Slot).Amount = 100
			
			' Jugos (Nwbie)
			Slot = Slot + 1
			.Invent.Object_Renamed(Slot).ObjIndex = 468
			.Invent.Object_Renamed(Slot).Amount = 100
			
			' Sin casco y escudo
			.Char_Renamed.ShieldAnim = NingunEscudo
			.Char_Renamed.CascoAnim = NingunCasco
			
			' Total Items
			.Invent.NroItems = Slot
			
			.LogOnTime = Now
			.UpTime = 0
			
		End With
		
		'Valores Default de facciones al Activar nuevo usuario
		Call ResetFacciones(UserIndex)
		
		Call WriteVar(CharPath & UCase(name) & ".chr", "INIT", "Password", Password) 'grabamos el password aqui afuera, para no mantenerlo cargado en memoria
		
		Call SaveUser(UserIndex, CharPath & UCase(name) & ".chr")
		
		'Open User
		Call ConnectUser(UserIndex, name, Password)
		
	End Sub
	
	Sub CloseSocket(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		If UserIndex = LastUser Then
			Do Until UserList(LastUser).flags.UserLogged
				LastUser = LastUser - 1
				If LastUser < 1 Then Exit Do
			Loop 
		End If
		
		'Call SecurityIp.IpRestarConexion(GetLongIp(UserList(UserIndex).ip))
		
		If UserList(UserIndex).ConnID <> -1 Then
			Call CloseSocketSL(UserIndex)
		End If
		
		'Es el mismo user al que está revisando el centinela??
		'IMPORTANTE!!! hacerlo antes de resetear así todavía sabemos el nombre del user
		' y lo podemos loguear
		If Centinela.RevisandoUserIndex = UserIndex Then Call modCentinela.CentinelaUserLogout()
		
		'mato los comercios seguros
		If UserList(UserIndex).ComUsu.DestUsu > 0 Then
			If UserList(UserList(UserIndex).ComUsu.DestUsu).flags.UserLogged Then
				If UserList(UserList(UserIndex).ComUsu.DestUsu).ComUsu.DestUsu = UserIndex Then
					Call WriteConsoleMsg(UserList(UserIndex).ComUsu.DestUsu, "Comercio cancelado por el otro usuario", Protocol.FontTypeNames.FONTTYPE_TALK)
					Call FinComerciarUsu(UserList(UserIndex).ComUsu.DestUsu)
					Call FlushBuffer(UserList(UserIndex).ComUsu.DestUsu)
				End If
			End If
		End If
		
		'Empty buffer for reuse
		Call UserList(UserIndex).incomingData.ReadASCIIStringFixed(UserList(UserIndex).incomingData.length)
		
		If UserList(UserIndex).flags.UserLogged Then
			If NumUsers > 0 Then NumUsers = NumUsers - 1
			Call CloseUser(UserIndex)
			
			Call EstadisticasWeb.Informar(clsEstadisticasIPC.EstaNotificaciones.CANTIDAD_ONLINE, NumUsers)
		Else
			Call ResetUserSlot(UserIndex)
		End If
		
		UserList(UserIndex).ConnID = -1
		UserList(UserIndex).ConnIDValida = False
		
		Exit Sub
		
Errhandler: 
		UserList(UserIndex).ConnID = -1
		UserList(UserIndex).ConnIDValida = False
		Call ResetUserSlot(UserIndex)
		
		Call LogError("CloseSocket - Error = " & Err.Number & " - Descripción = " & Err.Description & " - UserIndex = " & UserIndex)
	End Sub
	
	'[Alejo-21-5]: Cierra un socket sin limpiar el slot
	Sub CloseSocketSL(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		If UserList(UserIndex).ConnID <> -1 And UserList(UserIndex).ConnIDValida Then
			Call Winsock_Close(UserList(UserIndex).ConnID)
			UserList(UserIndex).ConnIDValida = False
		End If
		
	End Sub
	
	''
	' Send an string to a Slot
	'
	' @param userIndex The index of the User
	' @param Datos The string that will be send
	
	Public Function EnviarDatosASlot(ByVal UserIndex As Short, ByRef datos As String) As Integer
		'***************************************************
		'Author: Unknown
		'Last Modification: 01/10/07
		'Last Modified By: Lucas Tavolaro Ortiz (Tavo)
		'Now it uses the clsByteQueue class and don`t make a FIFO Queue of String
		'***************************************************
		On Error GoTo Err_Renamed
		
		Dim Ret As Integer
		
		Ret = WsApiEnviar(UserIndex, datos)
		
		If Ret <> 0 Then
			' Close the socket avoiding any critical error
			Call CloseSocketSL(UserIndex)
			Call Cerrar_Usuario(UserIndex)
		End If
		Exit Function
		
Err_Renamed: 
		
	End Function
	Function EstaPCarea(ByRef Index As Short, ByRef Index2 As Short) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim X, Y As Short
		For Y = UserList(Index).Pos.Y - MinYBorder + 1 To UserList(Index).Pos.Y + MinYBorder - 1
			For X = UserList(Index).Pos.X - MinXBorder + 1 To UserList(Index).Pos.X + MinXBorder - 1
				
				If MapData(UserList(Index).Pos.Map, X, Y).UserIndex = Index2 Then
					EstaPCarea = True
					Exit Function
				End If
				
			Next X
		Next Y
		EstaPCarea = False
	End Function
	
	Function HayPCarea(ByRef Pos As WorldPos) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim X, Y As Short
		For Y = Pos.Y - MinYBorder + 1 To Pos.Y + MinYBorder - 1
			For X = Pos.X - MinXBorder + 1 To Pos.X + MinXBorder - 1
				If X > 0 And Y > 0 And X < 101 And Y < 101 Then
					If MapData(Pos.Map, X, Y).UserIndex > 0 Then
						HayPCarea = True
						Exit Function
					End If
				End If
			Next X
		Next Y
		HayPCarea = False
	End Function
	
	Function HayOBJarea(ByRef Pos As WorldPos, ByRef ObjIndex As Short) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim X, Y As Short
		For Y = Pos.Y - MinYBorder + 1 To Pos.Y + MinYBorder - 1
			For X = Pos.X - MinXBorder + 1 To Pos.X + MinXBorder - 1
				If MapData(Pos.Map, X, Y).ObjInfo.ObjIndex = ObjIndex Then
					HayOBJarea = True
					Exit Function
				End If
				
			Next X
		Next Y
		HayOBJarea = False
	End Function
	Function ValidateChr(ByVal UserIndex As Short) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		ValidateChr = UserList(UserIndex).Char_Renamed.Head <> 0 And UserList(UserIndex).Char_Renamed.body <> 0 And ValidateSkills(UserIndex)
		
	End Function
	
	Sub ConnectUser(ByVal UserIndex As Short, ByRef name As String, ByRef Password As String)
		'***************************************************
		'Autor: Unknown (orginal version)
		'Last Modification: 3/12/2009 (Budi)
		'26/03/2009: ZaMa - Agrego por default que el color de dialogo de los dioses, sea como el de su nick.
		'12/06/2009: ZaMa - Agrego chequeo de nivel al loguear
		'14/09/2009: ZaMa - Ahora el usuario esta protegido del ataque de npcs al loguear
		'11/27/2009: Budi - Se envian los InvStats del personaje y su Fuerza y Agilidad
		'03/12/2009: Budi - Optimización del código
		'***************************************************
		Dim N As Short
		Dim tStr As String
		
		Dim Leer As New clsIniReader
		Dim FoundPlace As Boolean
		Dim esAgua As Boolean
		Dim tX As Integer
		Dim tY As Integer
		Dim i As Short
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Barco, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim Barco As ObjData
		With UserList(UserIndex)
			
			If .flags.UserLogged Then
				Call LogCheating("El usuario " & .name & " ha intentado loguear a " & name & " desde la IP " & .ip)
				'Kick player ( and leave character inside :D )!
				Call CloseSocketSL(UserIndex)
				Call Cerrar_Usuario(UserIndex)
				Exit Sub
			End If
			
			'Reseteamos los FLAGS
			.flags.Escondido = 0
			.flags.TargetNPC = 0
			.flags.TargetNpcTipo = Declaraciones.eNPCType.Comun
			.flags.TargetObj = 0
			.flags.TargetUser = 0
			.Char_Renamed.FX = 0
			
			'Controlamos no pasar el maximo de usuarios
			If NumUsers >= MaxUsers Then
				Call WriteErrorMsg(UserIndex, "El servidor ha alcanzado el máximo de usuarios soportado, por favor vuelva a intertarlo más tarde.")
				Call FlushBuffer(UserIndex)
				Call CloseSocket(UserIndex)
				Exit Sub
			End If
			
			'¿Este IP ya esta conectado?
			If AllowMultiLogins = 0 Then
				If CheckForSameIP(UserIndex, .ip) = True Then
					Call WriteErrorMsg(UserIndex, "No es posible usar más de un personaje al mismo tiempo.")
					Call FlushBuffer(UserIndex)
					Call CloseSocket(UserIndex)
					Exit Sub
				End If
			End If
			
			'¿Existe el personaje?
			If Not FileExist(CharPath & UCase(name) & ".chr") Then
				Call WriteErrorMsg(UserIndex, "El personaje no existe.")
				Call FlushBuffer(UserIndex)
				Call CloseSocket(UserIndex)
				Exit Sub
			End If
			
			'¿Es el passwd valido?
			If UCase(Password) <> UCase(GetVar(CharPath & UCase(name) & ".chr", "INIT", "Password")) Then
				Call WriteErrorMsg(UserIndex, "Password incorrecto.")
				Call FlushBuffer(UserIndex)
				Call CloseSocket(UserIndex)
				Exit Sub
			End If
			
			'¿Ya esta conectado el personaje?
			If CheckForSameName(name) Then
				If UserList(NameIndex(name)).Counters.Saliendo Then
					Call WriteErrorMsg(UserIndex, "El usuario está saliendo.")
				Else
					Call WriteErrorMsg(UserIndex, "Perdón, un usuario con el mismo nombre se ha logueado.")
				End If
				Call FlushBuffer(UserIndex)
				Call CloseSocket(UserIndex)
				Exit Sub
			End If
			
			'Reseteamos los privilegios
			.flags.Privilegios = 0
			
			'Vemos que clase de user es (se lo usa para setear los privilegios al loguear el PJ)
			If EsAdmin(name) Then
				.flags.Privilegios = .flags.Privilegios Or Declaraciones.PlayerType.Admin
				Call LogGM(name, "Se conecto con ip:" & .ip)
			ElseIf EsDios(name) Then 
				.flags.Privilegios = .flags.Privilegios Or Declaraciones.PlayerType.Dios
				Call LogGM(name, "Se conecto con ip:" & .ip)
			ElseIf EsSemiDios(name) Then 
				.flags.Privilegios = .flags.Privilegios Or Declaraciones.PlayerType.SemiDios
				Call LogGM(name, "Se conecto con ip:" & .ip)
			ElseIf EsConsejero(name) Then 
				.flags.Privilegios = .flags.Privilegios Or Declaraciones.PlayerType.Consejero
				Call LogGM(name, "Se conecto con ip:" & .ip)
			Else
				.flags.Privilegios = .flags.Privilegios Or Declaraciones.PlayerType.User
				.flags.AdminPerseguible = True
			End If
			
			'Add RM flag if needed
			If EsRolesMaster(name) Then
				.flags.Privilegios = .flags.Privilegios Or Declaraciones.PlayerType.RoleMaster
			End If
			
			If ServerSoloGMs > 0 Then
				If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.Consejero)) = 0 Then
					Call WriteErrorMsg(UserIndex, "Servidor restringido a administradores. Por favor reintente en unos momentos.")
					Call FlushBuffer(UserIndex)
					Call CloseSocket(UserIndex)
					Exit Sub
				End If
			End If
			
			'Cargamos el personaje
			
			Call Leer.Initialize(CharPath & UCase(name) & ".chr")
			
			'Cargamos los datos del personaje
			Call LoadUserInit(UserIndex, Leer)
			
			Call LoadUserStats(UserIndex, Leer)
			
			If Not ValidateChr(UserIndex) Then
				Call WriteErrorMsg(UserIndex, "Error en el personaje.")
				Call CloseSocket(UserIndex)
				Exit Sub
			End If
			
			Call LoadUserReputacion(UserIndex, Leer)
			
			'UPGRADE_NOTE: El objeto Leer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
			Leer = Nothing
			
			If .Invent.EscudoEqpSlot = 0 Then .Char_Renamed.ShieldAnim = NingunEscudo
			If .Invent.CascoEqpSlot = 0 Then .Char_Renamed.CascoAnim = NingunCasco
			If .Invent.WeaponEqpSlot = 0 Then .Char_Renamed.WeaponAnim = NingunArma
			
			If .Invent.MochilaEqpSlot > 0 Then
				.CurrentInventorySlots = MAX_NORMAL_INVENTORY_SLOTS + ObjData_Renamed(.Invent.Object_Renamed(.Invent.MochilaEqpSlot).ObjIndex).MochilaType * 5
			Else
				.CurrentInventorySlots = MAX_NORMAL_INVENTORY_SLOTS
			End If
			If (.flags.Muerto = 0) Then
				.flags.SeguroResu = False
				Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.ResuscitationSafeOff) 'Call WriteResuscitationSafeOff(UserIndex)
			Else
				.flags.SeguroResu = True
				Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.ResuscitationSafeOn) 'Call WriteResuscitationSafeOn(UserIndex)
			End If
			
			Call UpdateUserInv(True, UserIndex, 0)
			Call UpdateUserHechizos(True, UserIndex, 0)
			
			If .flags.Paralizado Then
				Call WriteParalizeOK(UserIndex)
			End If
			
			''
			'TODO : Feo, esto tiene que ser parche cliente
			If .flags.Estupidez = 0 Then
				Call WriteDumbNoMore(UserIndex)
			End If
			
			'Posicion de comienzo
			If .Pos.Map = 0 Then
				Select Case .Hogar
					Case Declaraciones.eCiudad.cNix
						'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						.Pos = Nix
					Case Declaraciones.eCiudad.cUllathorpe
						'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						.Pos = Ullathorpe
					Case Declaraciones.eCiudad.cBanderbill
						'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						.Pos = Banderbill
					Case Declaraciones.eCiudad.cLindos
						'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						.Pos = Lindos
					Case Declaraciones.eCiudad.cArghal
						'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						.Pos = Arghal
					Case Else
						.Hogar = Declaraciones.eCiudad.cUllathorpe
						'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						.Pos = Ullathorpe
				End Select
			Else
				If Not MapaValido(.Pos.Map) Then
					Call WriteErrorMsg(UserIndex, "El PJ se encuenta en un mapa inválido.")
					Call FlushBuffer(UserIndex)
					Call CloseSocket(UserIndex)
					Exit Sub
				End If
			End If
			
			'Tratamos de evitar en lo posible el "Telefrag". Solo 1 intento de loguear en pos adjacentes.
			'Codigo por Pablo (ToxicWaste) y revisado por Nacho (Integer), corregido para que realmetne ande y no tire el server por Juan Martín Sotuyo Dodero (Maraxus)
			If MapData(.Pos.Map, .Pos.X, .Pos.Y).UserIndex <> 0 Or MapData(.Pos.Map, .Pos.X, .Pos.Y).NpcIndex <> 0 Then
				
				FoundPlace = False
				esAgua = HayAgua(.Pos.Map, .Pos.X, .Pos.Y)
				
				For tY = .Pos.Y - 1 To .Pos.Y + 1
					For tX = .Pos.X - 1 To .Pos.X + 1
						If esAgua Then
							'reviso que sea pos legal en agua, que no haya User ni NPC para poder loguear.
							If LegalPos(.Pos.Map, tX, tY, True, False) Then
								FoundPlace = True
								Exit For
							End If
						Else
							'reviso que sea pos legal en tierra, que no haya User ni NPC para poder loguear.
							If LegalPos(.Pos.Map, tX, tY, False, True) Then
								FoundPlace = True
								Exit For
							End If
						End If
					Next tX
					
					If FoundPlace Then Exit For
				Next tY
				
				If FoundPlace Then 'Si encontramos un lugar, listo, nos quedamos ahi
					.Pos.X = tX
					.Pos.Y = tY
				Else
					'Si no encontramos un lugar, sacamos al usuario que tenemos abajo, y si es un NPC, lo pisamos.
					If MapData(.Pos.Map, .Pos.X, .Pos.Y).UserIndex <> 0 Then
						'Si no encontramos lugar, y abajo teniamos a un usuario, lo pisamos y cerramos su comercio seguro
						If UserList(MapData(.Pos.Map, .Pos.X, .Pos.Y).UserIndex).ComUsu.DestUsu > 0 Then
							'Le avisamos al que estaba comerciando que se tuvo que ir.
							If UserList(UserList(MapData(.Pos.Map, .Pos.X, .Pos.Y).UserIndex).ComUsu.DestUsu).flags.UserLogged Then
								Call FinComerciarUsu(UserList(MapData(.Pos.Map, .Pos.X, .Pos.Y).UserIndex).ComUsu.DestUsu)
								Call WriteConsoleMsg(UserList(MapData(.Pos.Map, .Pos.X, .Pos.Y).UserIndex).ComUsu.DestUsu, "Comercio cancelado. El otro usuario se ha desconectado.", Protocol.FontTypeNames.FONTTYPE_TALK)
								Call FlushBuffer(UserList(MapData(.Pos.Map, .Pos.X, .Pos.Y).UserIndex).ComUsu.DestUsu)
							End If
							'Lo sacamos.
							If UserList(MapData(.Pos.Map, .Pos.X, .Pos.Y).UserIndex).flags.UserLogged Then
								Call FinComerciarUsu(MapData(.Pos.Map, .Pos.X, .Pos.Y).UserIndex)
								Call WriteErrorMsg(MapData(.Pos.Map, .Pos.X, .Pos.Y).UserIndex, "Alguien se ha conectado donde te encontrabas, por favor reconéctate...")
								Call FlushBuffer(MapData(.Pos.Map, .Pos.X, .Pos.Y).UserIndex)
							End If
						End If
						
						Call CloseSocket(MapData(.Pos.Map, .Pos.X, .Pos.Y).UserIndex)
					End If
				End If
			End If
			
			'Nombre de sistema
			.name = name
			
			.showName = True 'Por default los nombres son visibles
			
			'If in the water, and has a boat, equip it!
			If .Invent.BarcoObjIndex > 0 And (HayAgua(.Pos.Map, .Pos.X, .Pos.Y) Or BodyIsBoat(.Char_Renamed.body)) Then
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Barco. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				Barco = ObjData_Renamed(.Invent.BarcoObjIndex)
				.Char_Renamed.Head = 0
				If .flags.Muerto = 0 Then
					Call ToogleBoatBody(UserIndex)
				Else
					.Char_Renamed.body = iFragataFantasmal
					.Char_Renamed.ShieldAnim = NingunEscudo
					.Char_Renamed.WeaponAnim = NingunArma
					.Char_Renamed.CascoAnim = NingunCasco
				End If
				
				.flags.Navegando = 1
			End If
			
			
			'Info
			Call WriteUserIndexInServer(UserIndex) 'Enviamos el User index
			Call WriteChangeMap(UserIndex, .Pos.Map, MapInfo_Renamed(.Pos.Map).MapVersion) 'Carga el mapa
			Call WritePlayMidi(UserIndex, Val(ReadField(1, MapInfo_Renamed(.Pos.Map).Music, 45)))
			
			If .flags.Privilegios = Declaraciones.PlayerType.Dios Then
				.flags.ChatColor = RGB(250, 250, 150)
			ElseIf .flags.Privilegios <> Declaraciones.PlayerType.User And .flags.Privilegios <> (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.ChaosCouncil) And .flags.Privilegios <> (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.RoyalCouncil) Then 
				.flags.ChatColor = RGB(0, 255, 0)
			ElseIf .flags.Privilegios = (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.RoyalCouncil) Then 
				.flags.ChatColor = RGB(0, 255, 255)
			ElseIf .flags.Privilegios = (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.ChaosCouncil) Then 
				.flags.ChatColor = RGB(255, 128, 64)
			Else
				.flags.ChatColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
			End If
			
			.LogOnTime = Now
			
			'Crea  el personaje del usuario
			Call MakeUserChar(True, .Pos.Map, UserIndex, .Pos.Map, .Pos.X, .Pos.Y)
			
			Call WriteUserCharIndexInServer(UserIndex)
			''[/el oso]
			
			Call CheckUserLevel(UserIndex)
			Call WriteUpdateUserStats(UserIndex)
			
			Call WriteUpdateHungerAndThirst(UserIndex)
			Call WriteUpdateStrenghtAndDexterity(UserIndex)
			
			Call SendMOTD(UserIndex)
			
			If haciendoBK Then
				Call WritePauseToggle(UserIndex)
				Call WriteConsoleMsg(UserIndex, "Servidor> Por favor espera algunos segundos, el WorldSave está ejecutándose.", Protocol.FontTypeNames.FONTTYPE_SERVER)
			End If
			
			If EnPausa Then
				Call WritePauseToggle(UserIndex)
				Call WriteConsoleMsg(UserIndex, "Servidor> Lo sentimos mucho pero el servidor se encuentra actualmente detenido. Intenta ingresar más tarde.", Protocol.FontTypeNames.FONTTYPE_SERVER)
			End If
			
			If EnTesting And .Stats.ELV >= 18 Then
				Call WriteErrorMsg(UserIndex, "Servidor en Testing por unos minutos, conectese con PJs de nivel menor a 18. No se conecte con Pjs que puedan resultar importantes por ahora pues pueden arruinarse.")
				Call FlushBuffer(UserIndex)
				Call CloseSocket(UserIndex)
				Exit Sub
			End If
			
			'Actualiza el Num de usuarios
			'DE ACA EN ADELANTE GRABA EL CHARFILE, OJO!
			NumUsers = NumUsers + 1
			.flags.UserLogged = True
			
			'usado para borrar Pjs
			Call WriteVar(CharPath & .name & ".chr", "INIT", "Logged", "1")
			
			Call EstadisticasWeb.Informar(clsEstadisticasIPC.EstaNotificaciones.CANTIDAD_ONLINE, NumUsers)
			
			MapInfo_Renamed(.Pos.Map).NumUsers = MapInfo_Renamed(.Pos.Map).NumUsers + 1
			
			If .Stats.SkillPts > 0 Then
				Call WriteSendSkills(UserIndex)
				Call WriteLevelUp(UserIndex, .Stats.SkillPts)
			End If
			
			If NumUsers > recordusuarios Then
				Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg("Record de usuarios conectados simultaneamente." & "Hay " & NumUsers & " usuarios.", Protocol.FontTypeNames.FONTTYPE_INFO))
				recordusuarios = NumUsers
				Call WriteVar(IniPath & "Server.ini", "INIT", "Record", Str(recordusuarios))
				
				Call EstadisticasWeb.Informar(clsEstadisticasIPC.EstaNotificaciones.RECORD_USUARIOS, recordusuarios)
			End If
			
			If .NroMascotas > 0 And MapInfo_Renamed(.Pos.Map).Pk Then
				For i = 1 To MAXMASCOTAS
					If .MascotasType(i) > 0 Then
						.MascotasIndex(i) = SpawnNpc(.MascotasType(i), .Pos, True, True)
						
						If .MascotasIndex(i) > 0 Then
							Npclist(.MascotasIndex(i)).MaestroUser = UserIndex
							Call FollowAmo(.MascotasIndex(i))
						Else
							.MascotasIndex(i) = 0
						End If
					End If
				Next i
			End If
			
			If .flags.Navegando = 1 Then
				Call WriteNavigateToggle(UserIndex)
			End If
			
			If criminal(UserIndex) Then
				Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.SafeModeOff) 'Call WriteSafeModeOff(UserIndex)
				.flags.Seguro = False
			Else
				.flags.Seguro = True
				Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.SafeModeOn) 'Call WriteSafeModeOn(UserIndex)
			End If
			
			If .GuildIndex > 0 Then
				'welcome to the show baby...
				If Not modGuilds.m_ConectarMiembroAClan(UserIndex, .GuildIndex) Then
					Call WriteConsoleMsg(UserIndex, "Tu estado no te permite entrar al clan.", Protocol.FontTypeNames.FONTTYPE_GUILD)
				End If
			End If
			
			Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageCreateFX(.Char_Renamed.CharIndex, Declaraciones.FXIDs.FXWARP, 0))
			
			Call WriteLoggedMessage(UserIndex)
			
			Call modGuilds.SendGuildNews(UserIndex)
			
			' Esta protegido del ataque de npcs por 5 segundos, si no realiza ninguna accion
			Call IntervaloPermiteSerAtacado(UserIndex, True)
			
			If Lloviendo Then
				Call WriteRainToggle(UserIndex)
			End If
			
			tStr = modGuilds.a_ObtenerRechazoDeChar(.name)
			
			If migr_LenB(tStr) <> 0 Then
				Call WriteShowMessageBox(UserIndex, "Tu solicitud de ingreso al clan ha sido rechazada. El clan te explica que: " & tStr)
			End If
			
			'Load the user statistics
			Call Statistics.UserConnected(UserIndex)
			
			Call MostrarNumUsers()
			
			N = FreeFile
			FileOpen(N, My.Application.Info.DirectoryPath & "\logs\numusers.log", OpenMode.Output)
			PrintLine(N, NumUsers)
			FileClose(N)
			
			N = FreeFile
			'Log
			FileOpen(N, My.Application.Info.DirectoryPath & "\logs\Connect.log", OpenMode.Append, , OpenShare.Shared)
			PrintLine(N, .name & " ha entrado al juego. UserIndex:" & UserIndex & " " & TimeOfDay & " " & Today)
			FileClose(N)
			
		End With
	End Sub
	
	Sub SendMOTD(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim j As Integer
		
		Call WriteGuildChat(UserIndex, "Mensajes de entrada:")
		For j = 1 To MaxLines
			Call WriteGuildChat(UserIndex, MOTD(j).texto)
		Next j
	End Sub
	
	Sub ResetFacciones(ByVal UserIndex As Short)
		'*************************************************
		'Author: Unknown
		'Last modified: 23/01/2007
		'Resetea todos los valores generales y las stats
		'03/15/2006 Maraxus - Uso de With para mayor performance y claridad.
		'23/01/2007 Pablo (ToxicWaste) - Agrego NivelIngreso, FechaIngreso, MatadosIngreso y NextRecompensa.
		'*************************************************
		With UserList(UserIndex).Faccion
			.ArmadaReal = 0
			.CiudadanosMatados = 0
			.CriminalesMatados = 0
			.FuerzasCaos = 0
			.FechaIngreso = "No ingresó a ninguna Facción"
			.RecibioArmaduraCaos = 0
			.RecibioArmaduraReal = 0
			.RecibioExpInicialCaos = 0
			.RecibioExpInicialReal = 0
			.RecompensasCaos = 0
			.RecompensasReal = 0
			.Reenlistadas = 0
			.NivelIngreso = 0
			.MatadosIngreso = 0
			.NextRecompensa = 0
		End With
	End Sub
	
	Sub ResetContadores(ByVal UserIndex As Short)
		'*************************************************
		'Author: Unknown
		'Last modified: 03/15/2006
		'Resetea todos los valores generales y las stats
		'03/15/2006 Maraxus - Uso de With para mayor performance y claridad.
		'05/20/2007 Integer - Agregue todas las variables que faltaban.
		'*************************************************
		With UserList(UserIndex).Counters
			.AGUACounter = 0
			.AttackCounter = 0
			.Ceguera = 0
			.COMCounter = 0
			.Estupidez = 0
			.Frio = 0
			.HPCounter = 0
			.IdleCount = 0
			.Invisibilidad = 0
			.Paralisis = 0
			.Pena = 0
			.PiqueteC = 0
			.STACounter = 0
			.Veneno = 0
			.Trabajando = 0
			.Ocultando = 0
			.bPuedeMeditar = False
			.Lava = 0
			.Mimetismo = 0
			.Saliendo = False
			.salir = 0
			.TiempoOculto = 0
			.TimerMagiaGolpe = 0
			.TimerGolpeMagia = 0
			.TimerLanzarSpell = 0
			.TimerPuedeAtacar = 0
			.TimerPuedeUsarArco = 0
			.TimerPuedeTrabajar = 0
			.TimerUsar = 0
			.goHome = 0
			.AsignedSkills = 0
		End With
	End Sub
	
	Sub ResetCharInfo(ByVal UserIndex As Short)
		'*************************************************
		'Author: Unknown
		'Last modified: 03/15/2006
		'Resetea todos los valores generales y las stats
		'03/15/2006 Maraxus - Uso de With para mayor performance y claridad.
		'*************************************************
		With UserList(UserIndex).Char_Renamed
			.body = 0
			.CascoAnim = 0
			.CharIndex = 0
			.FX = 0
			.Head = 0
			.loops = 0
			.heading = 0
			.loops = 0
			.ShieldAnim = 0
			.WeaponAnim = 0
		End With
	End Sub
	
	Sub ResetBasicUserInfo(ByVal UserIndex As Short)
		'*************************************************
		'Author: Unknown
		'Last modified: 03/15/2006
		'Resetea todos los valores generales y las stats
		'03/15/2006 Maraxus - Uso de With para mayor performance y claridad.
		'*************************************************
		With UserList(UserIndex)
			.name = vbNullString
			.desc = vbNullString
			.DescRM = vbNullString
			.Pos.Map = 0
			.Pos.X = 0
			.Pos.Y = 0
			.ip = vbNullString
			.clase = 0
			.email = vbNullString
			.Genero = 0
			.Hogar = 0
			.raza = 0
			
			.PartyIndex = 0
			.PartySolicitud = 0
			
			With .Stats
				.Banco = 0
				.ELV = 0
				.ELU = 0
				.Exp = 0
				.def = 0
				'.CriminalesMatados = 0
				.NPCsMuertos = 0
				.UsuariosMatados = 0
				.SkillPts = 0
				.GLD = 0
				.UserAtributos(1) = 0
				.UserAtributos(2) = 0
				.UserAtributos(3) = 0
				.UserAtributos(4) = 0
				.UserAtributos(5) = 0
				.UserAtributosBackUP(1) = 0
				.UserAtributosBackUP(2) = 0
				.UserAtributosBackUP(3) = 0
				.UserAtributosBackUP(4) = 0
				.UserAtributosBackUP(5) = 0
			End With
			
		End With
	End Sub
	
	Sub ResetReputacion(ByVal UserIndex As Short)
		'*************************************************
		'Author: Unknown
		'Last modified: 03/15/2006
		'Resetea todos los valores generales y las stats
		'03/15/2006 Maraxus - Uso de With para mayor performance y claridad.
		'*************************************************
		With UserList(UserIndex).Reputacion
			.AsesinoRep = 0
			.BandidoRep = 0
			.BurguesRep = 0
			.LadronesRep = 0
			.NobleRep = 0
			.PlebeRep = 0
			.NobleRep = 0
			.Promedio = 0
		End With
	End Sub
	
	Sub ResetGuildInfo(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		If UserList(UserIndex).EscucheClan > 0 Then
			Call modGuilds.GMDejaDeEscucharClan(UserIndex, UserList(UserIndex).EscucheClan)
			UserList(UserIndex).EscucheClan = 0
		End If
		If UserList(UserIndex).GuildIndex > 0 Then
			Call modGuilds.m_DesconectarMiembroDelClan(UserIndex, UserList(UserIndex).GuildIndex)
		End If
		UserList(UserIndex).GuildIndex = 0
	End Sub
	
	Sub ResetUserFlags(ByVal UserIndex As Short)
		'*************************************************
		'Author: Unknown
		'Last modified: 06/28/2008
		'Resetea todos los valores generales y las stats
		'03/15/2006 Maraxus - Uso de With para mayor performance y claridad.
		'03/29/2006 Maraxus - Reseteo el CentinelaOK también.
		'06/28/2008 NicoNZ - Agrego el flag Inmovilizado
		'*************************************************
		With UserList(UserIndex).flags
			.Comerciando = False
			.Ban = 0
			.Escondido = 0
			.DuracionEfecto = 0
			.NpcInv = 0
			.StatsChanged = 0
			.TargetNPC = 0
			.TargetNpcTipo = Declaraciones.eNPCType.Comun
			.TargetObj = 0
			.TargetObjMap = 0
			.TargetObjX = 0
			.TargetObjY = 0
			.TargetUser = 0
			.TipoPocion = 0
			.TomoPocion = False
			.Descuento = vbNullString
			.Hambre = 0
			.Sed = 0
			.Descansar = False
			.Vuela = 0
			.Navegando = 0
			.Oculto = 0
			.Envenenado = 0
			.invisible = 0
			.Paralizado = 0
			.Inmovilizado = 0
			.Maldicion = 0
			.Bendicion = 0
			.Meditando = 0
			.Privilegios = 0
			.PuedeMoverse = 0
			.OldBody = 0
			.OldHead = 0
			.AdminInvisible = 0
			.ValCoDe = 0
			.Hechizo = 0
			.TimesWalk = 0
			.StartWalk = 0
			.CountSH = 0
			.Silenciado = 0
			.CentinelaOK = False
			.AdminPerseguible = False
			.lastMap = 0
			.Traveling = 0
			.AtacablePor = 0
			.AtacadoPorNpc = 0
			.AtacadoPorUser = 0
			.NoPuedeSerAtacado = False
			.OwnedNpc = 0
			.ShareNpcWith = 0
			.EnConsulta = False
			.Ignorado = False
		End With
	End Sub
	
	Sub ResetUserSpells(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim LoopC As Integer
		For LoopC = 1 To MAXUSERHECHIZOS
			UserList(UserIndex).Stats.UserHechizos(LoopC) = 0
		Next LoopC
	End Sub
	
	Sub ResetUserPets(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim LoopC As Integer
		
		UserList(UserIndex).NroMascotas = 0
		
		For LoopC = 1 To MAXMASCOTAS
			UserList(UserIndex).MascotasIndex(LoopC) = 0
			UserList(UserIndex).MascotasType(LoopC) = 0
		Next LoopC
	End Sub
	
	Sub ResetUserBanco(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim LoopC As Integer
		
		For LoopC = 1 To MAX_BANCOINVENTORY_SLOTS
			UserList(UserIndex).BancoInvent.Object_Renamed(LoopC).Amount = 0
			UserList(UserIndex).BancoInvent.Object_Renamed(LoopC).Equipped = 0
			UserList(UserIndex).BancoInvent.Object_Renamed(LoopC).ObjIndex = 0
		Next LoopC
		
		UserList(UserIndex).BancoInvent.NroItems = 0
	End Sub
	
	Public Sub LimpiarComercioSeguro(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		With UserList(UserIndex).ComUsu
			If .DestUsu > 0 Then
				Call FinComerciarUsu(.DestUsu)
				Call FinComerciarUsu(UserIndex)
			End If
		End With
	End Sub
	
	Sub ResetUserSlot(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim i As Integer
		
		UserList(UserIndex).ConnIDValida = False
		UserList(UserIndex).ConnID = -1
		
		Call LimpiarComercioSeguro(UserIndex)
		Call ResetFacciones(UserIndex)
		Call ResetContadores(UserIndex)
		Call ResetGuildInfo(UserIndex)
		Call ResetCharInfo(UserIndex)
		Call ResetBasicUserInfo(UserIndex)
		Call ResetReputacion(UserIndex)
		Call ResetUserFlags(UserIndex)
		Call LimpiarInventario(UserIndex)
		Call ResetUserSpells(UserIndex)
		Call ResetUserPets(UserIndex)
		Call ResetUserBanco(UserIndex)
		With UserList(UserIndex).ComUsu
			.Acepto = False
			
			For i = 1 To MAX_OFFER_SLOTS
				.cant(i) = 0
				.Objeto(i) = 0
			Next i
			
			.GoldAmount = 0
			.DestNick = vbNullString
			.DestUsu = 0
		End With
		
	End Sub
	
	Sub CloseUser(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim N As Short
		Dim LoopC As Short
		Dim Map As Short
		Dim name As String
		Dim i As Short
		
		Dim aN As Short
		
		aN = UserList(UserIndex).flags.AtacadoPorNpc
		If aN > 0 Then
			Npclist(aN).Movement = Npclist(aN).flags.OldMovement
			Npclist(aN).Hostile = Npclist(aN).flags.OldHostil
			Npclist(aN).flags.AttackedBy = vbNullString
		End If
		aN = UserList(UserIndex).flags.NPCAtacado
		If aN > 0 Then
			If Npclist(aN).flags.AttackedFirstBy = UserList(UserIndex).name Then
				Npclist(aN).flags.AttackedFirstBy = vbNullString
			End If
		End If
		UserList(UserIndex).flags.AtacadoPorNpc = 0
		UserList(UserIndex).flags.NPCAtacado = 0
		
		Map = UserList(UserIndex).Pos.Map
		name = UCase(UserList(UserIndex).name)
		
		UserList(UserIndex).Char_Renamed.FX = 0
		UserList(UserIndex).Char_Renamed.loops = 0
		Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageCreateFX(UserList(UserIndex).Char_Renamed.CharIndex, 0, 0))
		
		
		UserList(UserIndex).flags.UserLogged = False
		UserList(UserIndex).Counters.Saliendo = False
		
		'Le devolvemos el body y head originales
		If UserList(UserIndex).flags.AdminInvisible = 1 Then Call DoAdminInvisible(UserIndex)
		
		'si esta en party le devolvemos la experiencia
		If UserList(UserIndex).PartyIndex > 0 Then Call mdParty.SalirDeParty(UserIndex)
		
		'Save statistics
		Call Statistics.UserDisconnected(UserIndex)
		
		' Grabamos el personaje del usuario
		Call SaveUser(UserIndex, CharPath & name & ".chr")
		
		'usado para borrar Pjs
		Call WriteVar(CharPath & UserList(UserIndex).name & ".chr", "INIT", "Logged", "0")
		
		
		'Quitar el dialogo
		'If MapInfo(Map).NumUsers > 0 Then
		'    Call SendToUserArea(UserIndex, "QDL" & UserList(UserIndex).Char.charindex)
		'End If
		
		If MapInfo_Renamed(Map).NumUsers > 0 Then
			Call SendData(modSendData.SendTarget.ToPCAreaButIndex, UserIndex, PrepareMessageRemoveCharDialog(UserList(UserIndex).Char_Renamed.CharIndex))
		End If
		
		
		
		'Borrar el personaje
		If UserList(UserIndex).Char_Renamed.CharIndex > 0 Then
			Call EraseUserChar(UserIndex, UserList(UserIndex).flags.AdminInvisible = 1)
		End If
		
		'Borrar mascotas
		For i = 1 To MAXMASCOTAS
			If UserList(UserIndex).MascotasIndex(i) > 0 Then
				If Npclist(UserList(UserIndex).MascotasIndex(i)).flags.NPCActive Then Call QuitarNPC(UserList(UserIndex).MascotasIndex(i))
			End If
		Next i
		
		'Update Map Users
		MapInfo_Renamed(Map).NumUsers = MapInfo_Renamed(Map).NumUsers - 1
		
		If MapInfo_Renamed(Map).NumUsers < 0 Then
			MapInfo_Renamed(Map).NumUsers = 0
		End If
		
		' Si el usuario habia dejado un msg en la gm's queue lo borramos
		If Ayuda.Existe(UserList(UserIndex).name) Then Call Ayuda.Quitar(UserList(UserIndex).name)
		
		Call ResetUserSlot(UserIndex)
		
		Call MostrarNumUsers()
		
		N = FreeFile()
		FileOpen(N, My.Application.Info.DirectoryPath & "\logs\Connect.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(N, name & " ha dejado el juego. " & "User Index:" & UserIndex & " " & TimeOfDay & " " & Today)
		FileClose(N)
		
		Exit Sub
		
Errhandler: 
		Call LogError("Error en CloseUser. Número " & Err.Number & " Descripción: " & Err.Description)
		
	End Sub
	
	Sub ReloadSokcet()
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************

		On Error GoTo Errhandler

		If NumUsers <= 0 Then
			Call WSApiReiniciarSockets()
		End If
		
		Exit Sub
Errhandler: 
		Call LogError("Error en CheckSocketState " & Err.Number & ": " & Err.Description)
		
	End Sub
	
	Public Sub EnviarNoche(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Call WriteSendNight(UserIndex, IIf(DeNoche And (MapInfo_Renamed(UserList(UserIndex).Pos.Map).Zona = Campo Or MapInfo_Renamed(UserList(UserIndex).Pos.Map).Zona = Ciudad), True, False))
		Call WriteSendNight(UserIndex, IIf(DeNoche, True, False))
	End Sub
	
	Public Sub EcharPjsNoPrivilegiados()
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim LoopC As Integer
		
		For LoopC = 1 To LastUser
			If UserList(LoopC).flags.UserLogged And UserList(LoopC).ConnID >= 0 And UserList(LoopC).ConnIDValida Then
				If UserList(LoopC).flags.Privilegios And Declaraciones.PlayerType.User Then
					Call CloseSocket(LoopC)
				End If
			End If
		Next LoopC
		
	End Sub
End Module
