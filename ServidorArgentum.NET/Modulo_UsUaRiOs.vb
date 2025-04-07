Option Strict Off
Option Explicit On
Module UsUaRiOs
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
	
	
	'?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
	'                        Modulo Usuarios
	'?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
	'Rutinas de los usuarios
	'?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
	
	Public Sub ActStats(ByVal VictimIndex As Short, ByVal AttackerIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: 11/03/2010
		'11/03/2010: ZaMa - Ahora no te vuelve cirminal por matar un atacable
		'***************************************************
		
		Dim DaExp As Short
		Dim EraCriminal As Boolean
		
		DaExp = CShort(UserList(VictimIndex).Stats.ELV) * 2
		
		With UserList(AttackerIndex)
			.Stats.Exp = .Stats.Exp + DaExp
			If .Stats.Exp > MAXEXP Then .Stats.Exp = MAXEXP
			
			If TriggerZonaPelea(VictimIndex, AttackerIndex) <> Declaraciones.eTrigger6.TRIGGER6_PERMITE Then
				
				' Es legal matarlo si estaba en atacable
				If UserList(VictimIndex).flags.AtacablePor <> AttackerIndex Then
					EraCriminal = criminal(AttackerIndex)
					
					With .Reputacion
						If Not criminal(VictimIndex) Then
							.AsesinoRep = .AsesinoRep + vlASESINO * 2
							If .AsesinoRep > MAXREP Then .AsesinoRep = MAXREP
							.BurguesRep = 0
							.NobleRep = 0
							.PlebeRep = 0
						Else
							.NobleRep = .NobleRep + vlNoble
							If .NobleRep > MAXREP Then .NobleRep = MAXREP
						End If
					End With
					
					If criminal(AttackerIndex) Then
						If Not EraCriminal Then Call RefreshCharStatus(AttackerIndex)
					Else
						If EraCriminal Then Call RefreshCharStatus(AttackerIndex)
					End If
				End If
			End If
			
			'Lo mata
			'Call WriteConsoleMsg(attackerIndex, "Has matado a " & UserList(VictimIndex).name & "!", FontTypeNames.FONTTYPE_FIGHT)
			'Call WriteConsoleMsg(attackerIndex, "Has ganado " & DaExp & " puntos de experiencia.", FontTypeNames.FONTTYPE_FIGHT)
			'Call WriteConsoleMsg(VictimIndex, "¡" & .name & " te ha matado!", FontTypeNames.FONTTYPE_FIGHT)
			Call WriteMultiMessage(AttackerIndex, Declaraciones.eMessages.HaveKilledUser, VictimIndex, DaExp)
			Call WriteMultiMessage(VictimIndex, Declaraciones.eMessages.UserKill, AttackerIndex)
			
			'Call UserDie(VictimIndex)
			Call FlushBuffer(VictimIndex)
			
			'Log
			Call LogAsesinato(.name & " asesino a " & UserList(VictimIndex).name)
		End With
	End Sub
	
	Public Sub RevivirUsuario(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		With UserList(UserIndex)
			.flags.Muerto = 0
			.Stats.MinHp = .Stats.UserAtributos(Declaraciones.eAtributos.Constitucion)
			
			If .Stats.MinHp > .Stats.MaxHp Then
				.Stats.MinHp = .Stats.MaxHp
			End If
			
			If .flags.Navegando = 1 Then
				Call ToogleBoatBody(UserIndex)
			Else
				Call DarCuerpoDesnudo(UserIndex)
				
				.Char_Renamed.Head = .OrigChar.Head
			End If
			
			If .flags.Traveling Then
				.flags.Traveling = 0
				.Counters.goHome = 0
				Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.CancelHome)
			End If
			
			Call ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading, .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim, .Char_Renamed.CascoAnim)
			Call WriteUpdateUserStats(UserIndex)
		End With
	End Sub
	
	Public Sub ToogleBoatBody(ByVal UserIndex As Short)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 13/01/2010
		'Gives boat body depending on user alignment.
		'***************************************************
		
		Dim Ropaje As Short
		
		With UserList(UserIndex)
			
			
			.Char_Renamed.Head = 0
			
			' Barco de armada
			If .Faccion.ArmadaReal = 1 Then
				.Char_Renamed.body = iFragataReal
				
				' Barco de caos
			ElseIf .Faccion.FuerzasCaos = 1 Then 
				.Char_Renamed.body = iFragataCaos
				
				'Barcos neutrales
			Else
				Ropaje = ObjData_Renamed(.Invent.BarcoObjIndex).Ropaje
				
				If criminal(UserIndex) Then
					Select Case Ropaje
						Case iBarca
							.Char_Renamed.body = iBarcaPk
							
						Case iGalera
							.Char_Renamed.body = iGaleraPk
							
						Case iGaleon
							.Char_Renamed.body = iGaleonPk
					End Select
				Else
					Select Case Ropaje
						Case iBarca
							.Char_Renamed.body = iBarcaCiuda
							
						Case iGalera
							.Char_Renamed.body = iGaleraCiuda
							
						Case iGaleon
							.Char_Renamed.body = iGaleonCiuda
					End Select
				End If
			End If
			
			.Char_Renamed.ShieldAnim = NingunEscudo
			.Char_Renamed.WeaponAnim = NingunArma
			.Char_Renamed.CascoAnim = NingunCasco
		End With
		
	End Sub
	
	Public Sub ChangeUserChar(ByVal UserIndex As Short, ByVal body As Short, ByVal Head As Short, ByVal heading As Byte, ByVal Arma As Short, ByVal Escudo As Short, ByVal casco As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		With UserList(UserIndex).Char_Renamed
			.body = body
			.Head = Head
			.heading = heading
			.WeaponAnim = Arma
			.ShieldAnim = Escudo
			.CascoAnim = casco
			
			Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageCharacterChange(body, Head, heading, .CharIndex, Arma, Escudo, .FX, .loops, casco))
		End With
	End Sub
	
	Public Function GetWeaponAnim(ByVal UserIndex As Short, ByVal ObjIndex As Short) As Short
		'***************************************************
		'Author: Torres Patricio (Pato)
		'Last Modification: 03/29/10
		'
		'***************************************************
		Dim Tmp As Short
		
		With UserList(UserIndex)
			Tmp = ObjData_Renamed(ObjIndex).WeaponRazaEnanaAnim
			
			If Tmp > 0 Then
				If .raza = Declaraciones.eRaza.Enano Or .raza = Declaraciones.eRaza.Gnomo Then
					GetWeaponAnim = Tmp
					Exit Function
				End If
			End If
			
			GetWeaponAnim = ObjData_Renamed(ObjIndex).WeaponAnim
		End With
	End Function
	
	Public Sub EnviarFama(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim L As Integer
		
		With UserList(UserIndex).Reputacion
			L = (-.AsesinoRep) + (-.BandidoRep) + .BurguesRep + (-.LadronesRep) + .NobleRep + .PlebeRep
			L = System.Math.Round(L / 6)
			
			.Promedio = L
		End With
		
		Call WriteFame(UserIndex)
	End Sub
	
	Public Sub EraseUserChar(ByVal UserIndex As Short, ByVal IsAdminInvisible As Boolean)
		'*************************************************
		'Author: Unknown
		'Last modified: 08/01/2009
		'08/01/2009: ZaMa - No se borra el char de un admin invisible en todos los clientes excepto en su mismo cliente.
		'*************************************************
		
		On Error GoTo ErrorHandler
		
		With UserList(UserIndex)
			CharList(.Char_Renamed.CharIndex) = 0
			
			If .Char_Renamed.CharIndex = LastChar Then
				Do Until CharList(LastChar) > 0
					LastChar = LastChar - 1
					If LastChar <= 1 Then Exit Do
				Loop 
			End If
			
			' Si esta invisible, solo el sabe de su propia existencia, es innecesario borrarlo en los demas clientes
			If IsAdminInvisible Then
				Call EnviarDatosASlot(UserIndex, PrepareMessageCharacterRemove(.Char_Renamed.CharIndex))
			Else
				'Le mandamos el mensaje para que borre el personaje a los clientes que estén cerca
				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageCharacterRemove(.Char_Renamed.CharIndex))
			End If
			
			Call QuitarUser(UserIndex, .Pos.Map)
			
			MapData(.Pos.Map, .Pos.X, .Pos.Y).UserIndex = 0
			.Char_Renamed.CharIndex = 0
		End With
		
		NumChars = NumChars - 1
		Exit Sub
		
ErrorHandler: 
		Call LogError("Error en EraseUserchar " & Err.Number & ": " & Err.Description)
	End Sub
	
	Public Sub RefreshCharStatus(ByVal UserIndex As Short)
		'*************************************************
		'Author: Tararira
		'Last modified: 04/07/2009
		'Refreshes the status and tag of UserIndex.
		'04/07/2009: ZaMa - Ahora mantenes la fragata fantasmal si estas muerto.
		'*************************************************
		Dim ClanTag As String
		Dim NickColor As Byte
		
		With UserList(UserIndex)
			If .GuildIndex > 0 Then
				ClanTag = modGuilds.GuildName(.GuildIndex)
				ClanTag = " <" & ClanTag & ">"
			End If
			
			NickColor = GetNickColor(UserIndex)
			
			If .showName Then
				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageUpdateTagAndStatus(UserIndex, NickColor, .name & ClanTag))
			Else
				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageUpdateTagAndStatus(UserIndex, NickColor, vbNullString))
			End If
			
			'Si esta navengando, se cambia la barca.
			If .flags.Navegando Then
				If .flags.Muerto = 1 Then
					.Char_Renamed.body = iFragataFantasmal
				Else
					Call ToogleBoatBody(UserIndex)
				End If
				
				Call ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading, .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim, .Char_Renamed.CascoAnim)
			End If
		End With
	End Sub
	
	Public Function GetNickColor(ByVal UserIndex As Short) As Byte
		'*************************************************
		'Author: ZaMa
		'Last modified: 15/01/2010
		'
		'*************************************************
		
		With UserList(UserIndex)
			
			If criminal(UserIndex) Then
				GetNickColor = Declaraciones.eNickColor.ieCriminal
			Else
				GetNickColor = Declaraciones.eNickColor.ieCiudadano
			End If
			
			If .flags.AtacablePor > 0 Then GetNickColor = GetNickColor Or Declaraciones.eNickColor.ieAtacable
		End With
		
	End Function
	
	Public Sub MakeUserChar(ByVal toMap As Boolean, ByVal sndIndex As Short, ByVal UserIndex As Short, ByVal Map As Short, ByVal X As Short, ByVal Y As Short, Optional ByRef ButIndex As Boolean = False)
		'*************************************************
		'Author: Unknown
		'Last modified: 15/01/2010
		'23/07/2009: Budi - Ahora se envía el nick
		'15/01/2010: ZaMa - Ahora se envia el color del nick.
		'*************************************************
		
		On Error GoTo Errhandler
		
		Dim CharIndex As Short
		Dim ClanTag As String
		Dim NickColor As Byte
		Dim UserName As String
		Dim Privileges As Byte
		
		With UserList(UserIndex)
			
			If InMapBounds(Map, X, Y) Then
				'If needed make a new character in list
				If .Char_Renamed.CharIndex = 0 Then
					CharIndex = NextOpenCharIndex
					.Char_Renamed.CharIndex = CharIndex
					CharList(CharIndex) = UserIndex
				End If
				
				'Place character on map if needed
				If toMap Then MapData(Map, X, Y).UserIndex = UserIndex
				
				'Send make character command to clients
				If Not toMap Then
					If .GuildIndex > 0 Then
						ClanTag = modGuilds.GuildName(.GuildIndex)
					End If
					
					NickColor = GetNickColor(UserIndex)
					Privileges = .flags.Privilegios
					
					'Preparo el nick
					If .showName Then
						UserName = .name
						
						If .flags.EnConsulta Then
							UserName = UserName & " " & TAG_CONSULT_MODE
						Else
							If UserList(sndIndex).flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.RoleMaster) Then
								If migr_LenB(ClanTag) <> 0 Then UserName = UserName & " <" & ClanTag & ">"
							Else
								If (.flags.invisible Or .flags.Oculto) And (Not .flags.AdminInvisible = 1) Then
									UserName = UserName & " " & TAG_USER_INVISIBLE
								Else
									If migr_LenB(ClanTag) <> 0 Then UserName = UserName & " <" & ClanTag & ">"
								End If
							End If
						End If
					End If
					
					Call WriteCharacterCreate(sndIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading, .Char_Renamed.CharIndex, X, Y, .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim, .Char_Renamed.FX, 999, .Char_Renamed.CascoAnim, UserName, NickColor, Privileges)
				Else
					'Hide the name and clan - set privs as normal user
					Call AgregarUser(UserIndex, .Pos.Map, ButIndex)
				End If
			End If
		End With
		Exit Sub
		
Errhandler: 
		LogError(("MakeUserChar: num: " & Err.Number & " desc: " & Err.Description))
		'Resume Next
		Call CloseSocket(UserIndex)
	End Sub
	
	''
	' Checks if the user gets the next level.
	'
	' @param UserIndex Specifies reference to user
	
	Public Sub CheckUserLevel(ByVal UserIndex As Short)
		'*************************************************
		'Author: Unknown
		'Last modified: 11/19/2009
		'Chequea que el usuario no halla alcanzado el siguiente nivel,
		'de lo contrario le da la vida, mana, etc, correspodiente.
		'07/08/2006 Integer - Modificacion de los valores
		'01/10/2007 Tavo - Corregido el BUG de STAT_MAXELV
		'24/01/2007 Pablo (ToxicWaste) - Agrego modificaciones en ELU al subir de nivel.
		'24/01/2007 Pablo (ToxicWaste) - Agrego modificaciones de la subida de mana de los magos por lvl.
		'13/03/2007 Pablo (ToxicWaste) - Agrego diferencias entre el 18 y el 19 en Constitución.
		'09/01/2008 Pablo (ToxicWaste) - Ahora el incremento de vida por Consitución se controla desde Balance.dat
		'12/09/2008 Marco Vanotti (Marco) - Ahora si se llega a nivel 25 y está en un clan, se lo expulsa para no sumar antifacción
		'02/03/2009 ZaMa - Arreglada la validacion de expulsion para miembros de clanes faccionarios que llegan a 25.
		'11/19/2009 Pato - Modifico la nueva fórmula de maná ganada para el bandido y se la limito a 499
		'02/04/2010: ZaMa - Modifico la ganancia de hit por nivel del ladron.
		'*************************************************
		Dim Pts As Short
		Dim AumentoHIT As Short
		Dim AumentoMANA As Short
		Dim AumentoSTA As Short
		Dim AumentoHP As Short
		Dim WasNewbie As Boolean
		Dim Promedio As Double
		Dim aux As Short
		'UPGRADE_WARNING: El límite inferior de la matriz DistVida ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		Dim DistVida(5) As Short
		Dim GI As Short 'Guild Index
		
		On Error GoTo Errhandler
		
		WasNewbie = EsNewbie(UserIndex)
		
		With UserList(UserIndex)
			Do While .Stats.Exp >= .Stats.ELU
				
				'Checkea si alcanzó el máximo nivel
				If .Stats.ELV >= STAT_MAXELV Then
					.Stats.Exp = 0
					.Stats.ELU = 0
					Exit Sub
				End If
				
				'Store it!
				Call Statistics.UserLevelUp(UserIndex)
				
				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessagePlayWave(SND_NIVEL, .Pos.X, .Pos.Y))
				Call WriteConsoleMsg(UserIndex, "¡Has subido de nivel!", Protocol.FontTypeNames.FONTTYPE_INFO)
				
				If .Stats.ELV = 1 Then
					Pts = 10
				Else
					'For multiple levels being rised at once
					Pts = Pts + 5
				End If
				
				.Stats.ELV = .Stats.ELV + 1
				
				.Stats.Exp = .Stats.Exp - .Stats.ELU
				
				'Nueva subida de exp x lvl. Pablo (ToxicWaste)
				If .Stats.ELV < 15 Then
					.Stats.ELU = .Stats.ELU * 1.4
				ElseIf .Stats.ELV < 21 Then 
					.Stats.ELU = .Stats.ELU * 1.35
				ElseIf .Stats.ELV < 33 Then 
					.Stats.ELU = .Stats.ELU * 1.3
				ElseIf .Stats.ELV < 41 Then 
					.Stats.ELU = .Stats.ELU * 1.225
				Else
					.Stats.ELU = .Stats.ELU * 1.25
				End If
				
				'Calculo subida de vida
				Promedio = ModVida(.clase) - (21 - .Stats.UserAtributos(Declaraciones.eAtributos.Constitucion)) * 0.5
				aux = RandomNumber(0, 100)
				
				
				If Promedio - Int(Promedio) = 0.5 Then
					'Es promedio semientero
					DistVida(1) = DistribucionSemienteraVida(1)
					DistVida(2) = DistVida(1) + DistribucionSemienteraVida(2)
					DistVida(3) = DistVida(2) + DistribucionSemienteraVida(3)
					DistVida(4) = DistVida(3) + DistribucionSemienteraVida(4)
					
					If aux <= DistVida(1) Then
						AumentoHP = Promedio + 1.5
					ElseIf aux <= DistVida(2) Then 
						AumentoHP = Promedio + 0.5
					ElseIf aux <= DistVida(3) Then 
						AumentoHP = Promedio - 0.5
					Else
						AumentoHP = Promedio - 1.5
					End If
				Else
					'Es promedio entero
					
					DistVida(1) = DistribucionSemienteraVida(1)
					DistVida(2) = DistVida(1) + DistribucionEnteraVida(2)
					DistVida(3) = DistVida(2) + DistribucionEnteraVida(3)
					DistVida(4) = DistVida(3) + DistribucionEnteraVida(4)
					DistVida(5) = DistVida(4) + DistribucionEnteraVida(5)
					
					If aux <= DistVida(1) Then
						AumentoHP = Promedio + 2
					ElseIf aux <= DistVida(2) Then 
						AumentoHP = Promedio + 1
					ElseIf aux <= DistVida(3) Then 
						AumentoHP = Promedio
					ElseIf aux <= DistVida(4) Then 
						AumentoHP = Promedio - 1
					Else
						AumentoHP = Promedio - 2
					End If
					
				End If
				
				Select Case .clase
					Case Declaraciones.eClass.Warrior
						AumentoHIT = IIf(.Stats.ELV > 35, 2, 3)
						AumentoSTA = AumentoSTDef
						
					Case Declaraciones.eClass.Hunter
						AumentoHIT = IIf(.Stats.ELV > 35, 2, 3)
						AumentoSTA = AumentoSTDef
						
					Case Declaraciones.eClass.Pirat
						AumentoHIT = 3
						AumentoSTA = AumentoSTDef
						
					Case Declaraciones.eClass.Paladin
						AumentoHIT = IIf(.Stats.ELV > 35, 1, 3)
						AumentoMANA = .Stats.UserAtributos(Declaraciones.eAtributos.Inteligencia)
						AumentoSTA = AumentoSTDef
						
					Case Declaraciones.eClass.Thief
						AumentoHIT = 2
						AumentoSTA = AumentoSTLadron
						
					Case Declaraciones.eClass.Mage
						AumentoHIT = 1
						AumentoMANA = 2.8 * .Stats.UserAtributos(Declaraciones.eAtributos.Inteligencia)
						AumentoSTA = AumentoSTMago
						
					Case Declaraciones.eClass.Worker
						AumentoHIT = 2
						AumentoSTA = AumentoSTTrabajador
						
					Case Declaraciones.eClass.Cleric
						AumentoHIT = 2
						AumentoMANA = 2 * .Stats.UserAtributos(Declaraciones.eAtributos.Inteligencia)
						AumentoSTA = AumentoSTDef
						
					Case Declaraciones.eClass.Druid
						AumentoHIT = 2
						AumentoMANA = 2 * .Stats.UserAtributos(Declaraciones.eAtributos.Inteligencia)
						AumentoSTA = AumentoSTDef
						
					Case Declaraciones.eClass.Assasin
						AumentoHIT = IIf(.Stats.ELV > 35, 1, 3)
						AumentoMANA = .Stats.UserAtributos(Declaraciones.eAtributos.Inteligencia)
						AumentoSTA = AumentoSTDef
						
					Case Declaraciones.eClass.Bard
						AumentoHIT = 2
						AumentoMANA = 2 * .Stats.UserAtributos(Declaraciones.eAtributos.Inteligencia)
						AumentoSTA = AumentoSTDef
						
					Case Declaraciones.eClass.Bandit
						AumentoHIT = IIf(.Stats.ELV > 35, 1, 3)
						AumentoMANA = .Stats.UserAtributos(Declaraciones.eAtributos.Inteligencia) / 3 * 2
						AumentoSTA = AumentoStBandido
						
					Case Else
						AumentoHIT = 2
						AumentoSTA = AumentoSTDef
				End Select
				
				'Actualizamos HitPoints
				.Stats.MaxHp = .Stats.MaxHp + AumentoHP
				If .Stats.MaxHp > STAT_MAXHP Then .Stats.MaxHp = STAT_MAXHP
				
				'Actualizamos Stamina
				.Stats.MaxSta = .Stats.MaxSta + AumentoSTA
				If .Stats.MaxSta > STAT_MAXSTA Then .Stats.MaxSta = STAT_MAXSTA
				
				'Actualizamos Mana
				.Stats.MaxMAN = .Stats.MaxMAN + AumentoMANA
				If .Stats.MaxMAN > STAT_MAXMAN Then .Stats.MaxMAN = STAT_MAXMAN
				
				'Actualizamos Golpe Máximo
				.Stats.MaxHIT = .Stats.MaxHIT + AumentoHIT
				If .Stats.ELV < 36 Then
					If .Stats.MaxHIT > STAT_MAXHIT_UNDER36 Then .Stats.MaxHIT = STAT_MAXHIT_UNDER36
				Else
					If .Stats.MaxHIT > STAT_MAXHIT_OVER36 Then .Stats.MaxHIT = STAT_MAXHIT_OVER36
				End If
				
				'Actualizamos Golpe Mínimo
				.Stats.MinHIT = .Stats.MinHIT + AumentoHIT
				If .Stats.ELV < 36 Then
					If .Stats.MinHIT > STAT_MAXHIT_UNDER36 Then .Stats.MinHIT = STAT_MAXHIT_UNDER36
				Else
					If .Stats.MinHIT > STAT_MAXHIT_OVER36 Then .Stats.MinHIT = STAT_MAXHIT_OVER36
				End If
				
				'Notificamos al user
				If AumentoHP > 0 Then
					Call WriteConsoleMsg(UserIndex, "Has ganado " & AumentoHP & " puntos de vida.", Protocol.FontTypeNames.FONTTYPE_INFO)
				End If
				If AumentoSTA > 0 Then
					Call WriteConsoleMsg(UserIndex, "Has ganado " & AumentoSTA & " puntos de energía.", Protocol.FontTypeNames.FONTTYPE_INFO)
				End If
				If AumentoMANA > 0 Then
					Call WriteConsoleMsg(UserIndex, "Has ganado " & AumentoMANA & " puntos de maná.", Protocol.FontTypeNames.FONTTYPE_INFO)
				End If
				If AumentoHIT > 0 Then
					Call WriteConsoleMsg(UserIndex, "Tu golpe máximo aumentó en " & AumentoHIT & " puntos.", Protocol.FontTypeNames.FONTTYPE_INFO)
					Call WriteConsoleMsg(UserIndex, "Tu golpe mínimo aumentó en " & AumentoHIT & " puntos.", Protocol.FontTypeNames.FONTTYPE_INFO)
				End If
				
				Call LogDesarrollo(.name & " paso a nivel " & .Stats.ELV & " gano HP: " & AumentoHP)
				
				.Stats.MinHp = .Stats.MaxHp
				
				'If user is in a party, we modify the variable p_sumaniveleselevados
				Call mdParty.ActualizarSumaNivelesElevados(UserIndex)
				'If user reaches lvl 25 and he is in a guild, we check the guild's alignment and expulses the user if guild has factionary alignment
				
				If .Stats.ELV = 25 Then
					GI = .GuildIndex
					If GI > 0 Then
						If modGuilds.GuildAlignment(GI) = "Del Mal" Or modGuilds.GuildAlignment(GI) = "Real" Then
							'We get here, so guild has factionary alignment, we have to expulse the user
							Call modGuilds.m_EcharMiembroDeClan(-1, .name)
							Call SendData(modSendData.SendTarget.ToGuildMembers, GI, PrepareMessageConsoleMsg(.name & " deja el clan.", Protocol.FontTypeNames.FONTTYPE_GUILD))
							Call WriteConsoleMsg(UserIndex, "¡Ya tienes la madurez suficiente como para decidir bajo que estandarte pelearás! Por esta razón, hasta tanto no te enlistes en la facción bajo la cual tu clan está alineado, estarás excluído del mismo.", Protocol.FontTypeNames.FONTTYPE_GUILD)
						End If
					End If
				End If
				
			Loop 
			
			'If it ceased to be a newbie, remove newbie items and get char away from newbie dungeon
			If Not EsNewbie(UserIndex) And WasNewbie Then
				Call QuitarNewbieObj(UserIndex)
				If UCase(MapInfo_Renamed(.Pos.Map).Restringir) = "NEWBIE" Then
					Call WarpUserChar(UserIndex, 1, 50, 50, True)
					Call WriteConsoleMsg(UserIndex, "Debes abandonar el Dungeon Newbie.", Protocol.FontTypeNames.FONTTYPE_INFO)
				End If
			End If
			
			'Send all gained skill points at once (if any)
			If Pts > 0 Then
				Call WriteLevelUp(UserIndex, Pts)
				
				.Stats.SkillPts = .Stats.SkillPts + Pts
				
				Call WriteConsoleMsg(UserIndex, "Has ganado un total de " & Pts & " skillpoints.", Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
			
		End With
		
		Call WriteUpdateUserStats(UserIndex)
		Exit Sub
		
Errhandler: 
		Call LogError("Error en la subrutina CheckUserLevel - Error : " & Err.Number & " - Description : " & Err.Description)
	End Sub
	
	Public Function PuedeAtravesarAgua(ByVal UserIndex As Short) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		PuedeAtravesarAgua = UserList(UserIndex).flags.Navegando = 1 Or UserList(UserIndex).flags.Vuela = 1
	End Function
	
	Sub MoveUserChar(ByVal UserIndex As Short, ByVal nHeading As Declaraciones.eHeading)
		'*************************************************
		'Author: Unknown
		'Last modified: 13/07/2009
		'Moves the char, sending the message to everyone in range.
		'30/03/2009: ZaMa - Now it's legal to move where a casper is, changing its pos to where the moving char was.
		'28/05/2009: ZaMa - When you are moved out of an Arena, the resurrection safe is activated.
		'13/07/2009: ZaMa - Now all the clients don't know when an invisible admin moves, they force the admin to move.
		'13/07/2009: ZaMa - Invisible admins aren't allowed to force dead characater to move
		'*************************************************
		Dim nPos As WorldPos
		Dim sailing As Boolean
		Dim CasperIndex As Short
		Dim CasperHeading As Declaraciones.eHeading
		Dim CasPerPos As WorldPos
		
		sailing = PuedeAtravesarAgua(UserIndex)
		'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		nPos = UserList(UserIndex).Pos
		Call HeadtoPos(nHeading, nPos)
		
		Dim oldUserIndex As Short
		If MoveToLegalPos(UserList(UserIndex).Pos.Map, nPos.X, nPos.Y, sailing, Not sailing) Then
			'si no estoy solo en el mapa...
			If MapInfo_Renamed(UserList(UserIndex).Pos.Map).NumUsers > 1 Then
				
				CasperIndex = MapData(UserList(UserIndex).Pos.Map, nPos.X, nPos.Y).UserIndex
				'Si hay un usuario, y paso la validacion, entonces es un casper
				If CasperIndex > 0 Then
					' Los admins invisibles no pueden patear caspers
					If Not (UserList(UserIndex).flags.AdminInvisible = 1) Then
						
						If TriggerZonaPelea(UserIndex, CasperIndex) = Declaraciones.eTrigger6.TRIGGER6_PROHIBE Then
							If UserList(CasperIndex).flags.SeguroResu = False Then
								UserList(CasperIndex).flags.SeguroResu = True
								Call WriteMultiMessage(CasperIndex, Declaraciones.eMessages.ResuscitationSafeOn)
							End If
						End If
						
						CasperHeading = InvertHeading(nHeading)
						'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto CasPerPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						CasPerPos = UserList(CasperIndex).Pos
						Call HeadtoPos(CasperHeading, CasPerPos)
						
						With UserList(CasperIndex)
							
							' Si es un admin invisible, no se avisa a los demas clientes
							If Not .flags.AdminInvisible = 1 Then Call SendData(modSendData.SendTarget.ToPCAreaButIndex, CasperIndex, PrepareMessageCharacterMove(.Char_Renamed.CharIndex, CasPerPos.X, CasPerPos.Y))
							
							Call WriteForceCharMove(CasperIndex, CasperHeading)
							
							'Update map and user pos
							'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
							.Pos = CasPerPos
							.Char_Renamed.heading = CasperHeading
							MapData(.Pos.Map, CasPerPos.X, CasPerPos.Y).UserIndex = CasperIndex
							
						End With
						
						'Actualizamos las áreas de ser necesario
						Call ModAreas.CheckUpdateNeededUser(CasperIndex, CasperHeading)
					End If
				End If
				
				
				' Si es un admin invisible, no se avisa a los demas clientes
				If Not UserList(UserIndex).flags.AdminInvisible = 1 Then Call SendData(modSendData.SendTarget.ToPCAreaButIndex, UserIndex, PrepareMessageCharacterMove(UserList(UserIndex).Char_Renamed.CharIndex, nPos.X, nPos.Y))
				
			End If
			
			' Los admins invisibles no pueden patear caspers
			If Not ((UserList(UserIndex).flags.AdminInvisible = 1) And CasperIndex <> 0) Then
				
				oldUserIndex = MapData(UserList(UserIndex).Pos.Map, UserList(UserIndex).Pos.X, UserList(UserIndex).Pos.Y).UserIndex
				
				' Si no hay intercambio de pos con nadie
				If oldUserIndex = UserIndex Then
					MapData(UserList(UserIndex).Pos.Map, UserList(UserIndex).Pos.X, UserList(UserIndex).Pos.Y).UserIndex = 0
				End If
				
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				UserList(UserIndex).Pos = nPos
				UserList(UserIndex).Char_Renamed.heading = nHeading
				MapData(UserList(UserIndex).Pos.Map, UserList(UserIndex).Pos.X, UserList(UserIndex).Pos.Y).UserIndex = UserIndex
				
				'Actualizamos las áreas de ser necesario
				Call ModAreas.CheckUpdateNeededUser(UserIndex, nHeading)
			Else
				Call WritePosUpdate(UserIndex)
			End If
			
		Else
			Call WritePosUpdate(UserIndex)
		End If
		
		If UserList(UserIndex).Counters.Trabajando Then UserList(UserIndex).Counters.Trabajando = UserList(UserIndex).Counters.Trabajando - 1
		
		If UserList(UserIndex).Counters.Ocultando Then UserList(UserIndex).Counters.Ocultando = UserList(UserIndex).Counters.Ocultando - 1
	End Sub
	
	Public Function InvertHeading(ByVal nHeading As Declaraciones.eHeading) As Declaraciones.eHeading
		'*************************************************
		'Author: ZaMa
		'Last modified: 30/03/2009
		'Returns the heading opposite to the one passed by val.
		'*************************************************
		Select Case nHeading
			Case Declaraciones.eHeading.EAST
				InvertHeading = Declaraciones.eHeading.WEST
			Case Declaraciones.eHeading.WEST
				InvertHeading = Declaraciones.eHeading.EAST
			Case Declaraciones.eHeading.SOUTH
				InvertHeading = Declaraciones.eHeading.NORTH
			Case Declaraciones.eHeading.NORTH
				InvertHeading = Declaraciones.eHeading.SOUTH
		End Select
	End Function
	
	'UPGRADE_NOTE: Object se actualizó a Object_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Sub ChangeUserInv(ByVal UserIndex As Short, ByVal Slot As Byte, ByRef Object_Renamed As UserOBJ)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Invent.Object(Slot). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		UserList(UserIndex).Invent.Object_Renamed(Slot) = Object_Renamed
		Call WriteChangeInventorySlot(UserIndex, Slot)
	End Sub
	
	Function NextOpenCharIndex() As Short
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim LoopC As Integer
		
		For LoopC = 1 To MAXCHARS
			If CharList(LoopC) = 0 Then
				NextOpenCharIndex = LoopC
				NumChars = NumChars + 1
				
				If LoopC > LastChar Then LastChar = LoopC
				
				Exit Function
			End If
		Next LoopC
	End Function
	
	Function NextOpenUser() As Short
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim LoopC As Integer
		
		For LoopC = 1 To MaxUsers + 1
			If LoopC > MaxUsers Then Exit For
			If (UserList(LoopC).ConnID = -1 And UserList(LoopC).flags.UserLogged = False) Then Exit For
		Next LoopC
		
		NextOpenUser = LoopC
	End Function
	
	Public Sub SendUserStatsTxt(ByVal sendIndex As Short, ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim GuildI As Short
		
		Dim TempDate As Date
		Dim TempSecs As Integer
		Dim tempStr As String
		With UserList(UserIndex)
			Call WriteConsoleMsg(sendIndex, "Estadísticas de: " & .name, Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Nivel: " & .Stats.ELV & "  EXP: " & .Stats.Exp & "/" & .Stats.ELU, Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Salud: " & .Stats.MinHp & "/" & .Stats.MaxHp & "  Maná: " & .Stats.MinMAN & "/" & .Stats.MaxMAN & "  Energía: " & .Stats.MinSta & "/" & .Stats.MaxSta, Protocol.FontTypeNames.FONTTYPE_INFO)
			
			If .Invent.WeaponEqpObjIndex > 0 Then
				Call WriteConsoleMsg(sendIndex, "Menor Golpe/Mayor Golpe: " & .Stats.MinHIT & "/" & .Stats.MaxHIT & " (" & ObjData_Renamed(.Invent.WeaponEqpObjIndex).MinHIT & "/" & ObjData_Renamed(.Invent.WeaponEqpObjIndex).MaxHIT & ")", Protocol.FontTypeNames.FONTTYPE_INFO)
			Else
				Call WriteConsoleMsg(sendIndex, "Menor Golpe/Mayor Golpe: " & .Stats.MinHIT & "/" & .Stats.MaxHIT, Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
			
			If .Invent.ArmourEqpObjIndex > 0 Then
				If .Invent.EscudoEqpObjIndex > 0 Then
					Call WriteConsoleMsg(sendIndex, "(CUERPO) Mín Def/Máx Def: " & ObjData_Renamed(.Invent.ArmourEqpObjIndex).MinDef + ObjData_Renamed(.Invent.EscudoEqpObjIndex).MinDef & "/" & ObjData_Renamed(.Invent.ArmourEqpObjIndex).MaxDef + ObjData_Renamed(.Invent.EscudoEqpObjIndex).MaxDef, Protocol.FontTypeNames.FONTTYPE_INFO)
				Else
					Call WriteConsoleMsg(sendIndex, "(CUERPO) Mín Def/Máx Def: " & ObjData_Renamed(.Invent.ArmourEqpObjIndex).MinDef & "/" & ObjData_Renamed(.Invent.ArmourEqpObjIndex).MaxDef, Protocol.FontTypeNames.FONTTYPE_INFO)
				End If
			Else
				Call WriteConsoleMsg(sendIndex, "(CUERPO) Mín Def/Máx Def: 0", Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
			
			If .Invent.CascoEqpObjIndex > 0 Then
				Call WriteConsoleMsg(sendIndex, "(CABEZA) Mín Def/Máx Def: " & ObjData_Renamed(.Invent.CascoEqpObjIndex).MinDef & "/" & ObjData_Renamed(.Invent.CascoEqpObjIndex).MaxDef, Protocol.FontTypeNames.FONTTYPE_INFO)
			Else
				Call WriteConsoleMsg(sendIndex, "(CABEZA) Mín Def/Máx Def: 0", Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
			
			GuildI = .GuildIndex
			If GuildI > 0 Then
				Call WriteConsoleMsg(sendIndex, "Clan: " & modGuilds.GuildName(GuildI), Protocol.FontTypeNames.FONTTYPE_INFO)
				If UCase(modGuilds.GuildLeader(GuildI)) = UCase(.name) Then
					Call WriteConsoleMsg(sendIndex, "Status: Líder", Protocol.FontTypeNames.FONTTYPE_INFO)
				End If
				'guildpts no tienen objeto
			End If
			
			TempDate = System.Date.FromOADate(Now.ToOADate - .LogOnTime.ToOADate)
			TempSecs = (.UpTime + (System.Math.Abs(TempDate.Day - 30) * 24 * 3600) + (Hour(TempDate) * 3600) + (Minute(TempDate) * 60) + Second(TempDate))
			tempStr = (TempSecs \ 86400) & " Dias, " & ((TempSecs Mod 86400) \ 3600) & " Horas, " & ((TempSecs Mod 86400) Mod 3600) \ 60 & " Minutos, " & (((TempSecs Mod 86400) Mod 3600) Mod 60) & " Segundos."
			Call WriteConsoleMsg(sendIndex, "Logeado hace: " & Hour(TempDate) & ":" & Minute(TempDate) & ":" & Second(TempDate), Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Total: " & tempStr, Protocol.FontTypeNames.FONTTYPE_INFO)
			
			Call WriteConsoleMsg(sendIndex, "Oro: " & .Stats.GLD & "  Posición: " & .Pos.X & "," & .Pos.Y & " en mapa " & .Pos.Map, Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Dados: " & .Stats.UserAtributos(Declaraciones.eAtributos.Fuerza) & ", " & .Stats.UserAtributos(Declaraciones.eAtributos.Agilidad) & ", " & .Stats.UserAtributos(Declaraciones.eAtributos.Inteligencia) & ", " & .Stats.UserAtributos(Declaraciones.eAtributos.Carisma) & ", " & .Stats.UserAtributos(Declaraciones.eAtributos.Constitucion), Protocol.FontTypeNames.FONTTYPE_INFO)
		End With
	End Sub
	
	Sub SendUserMiniStatsTxt(ByVal sendIndex As Short, ByVal UserIndex As Short)
		'*************************************************
		'Author: Unknown
		'Last modified: 23/01/2007
		'Shows the users Stats when the user is online.
		'23/01/2007 Pablo (ToxicWaste) - Agrego de funciones y mejora de distribución de parámetros.
		'*************************************************
		With UserList(UserIndex)
			Call WriteConsoleMsg(sendIndex, "Pj: " & .name, Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Ciudadanos matados: " & .Faccion.CiudadanosMatados & " Criminales matados: " & .Faccion.CriminalesMatados & " usuarios matados: " & .Stats.UsuariosMatados, Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "NPCs muertos: " & .Stats.NPCsMuertos, Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Clase: " & ListaClases(.clase), Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Pena: " & .Counters.Pena, Protocol.FontTypeNames.FONTTYPE_INFO)
			
			If .Faccion.ArmadaReal = 1 Then
				Call WriteConsoleMsg(sendIndex, "Ejército real desde: " & .Faccion.FechaIngreso, Protocol.FontTypeNames.FONTTYPE_INFO)
				Call WriteConsoleMsg(sendIndex, "Ingresó en nivel: " & .Faccion.NivelIngreso & " con " & .Faccion.MatadosIngreso & " ciudadanos matados.", Protocol.FontTypeNames.FONTTYPE_INFO)
				Call WriteConsoleMsg(sendIndex, "Veces que ingresó: " & .Faccion.Reenlistadas, Protocol.FontTypeNames.FONTTYPE_INFO)
				
			ElseIf .Faccion.FuerzasCaos = 1 Then 
				Call WriteConsoleMsg(sendIndex, "Legión oscura desde: " & .Faccion.FechaIngreso, Protocol.FontTypeNames.FONTTYPE_INFO)
				Call WriteConsoleMsg(sendIndex, "Ingresó en nivel: " & .Faccion.NivelIngreso, Protocol.FontTypeNames.FONTTYPE_INFO)
				Call WriteConsoleMsg(sendIndex, "Veces que ingresó: " & .Faccion.Reenlistadas, Protocol.FontTypeNames.FONTTYPE_INFO)
				
			ElseIf .Faccion.RecibioExpInicialReal = 1 Then 
				Call WriteConsoleMsg(sendIndex, "Fue ejército real", Protocol.FontTypeNames.FONTTYPE_INFO)
				Call WriteConsoleMsg(sendIndex, "Veces que ingresó: " & .Faccion.Reenlistadas, Protocol.FontTypeNames.FONTTYPE_INFO)
				
			ElseIf .Faccion.RecibioExpInicialCaos = 1 Then 
				Call WriteConsoleMsg(sendIndex, "Fue legión oscura", Protocol.FontTypeNames.FONTTYPE_INFO)
				Call WriteConsoleMsg(sendIndex, "Veces que ingresó: " & .Faccion.Reenlistadas, Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
			
			Call WriteConsoleMsg(sendIndex, "Asesino: " & .Reputacion.AsesinoRep, Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Noble: " & .Reputacion.NobleRep, Protocol.FontTypeNames.FONTTYPE_INFO)
			
			If .GuildIndex > 0 Then
				Call WriteConsoleMsg(sendIndex, "Clan: " & GuildName(.GuildIndex), Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub
	
	Sub SendUserMiniStatsTxtFromChar(ByVal sendIndex As Short, ByVal charName As String)
		'*************************************************
		'Author: Unknown
		'Last modified: 23/01/2007
		'Shows the users Stats when the user is offline.
		'23/01/2007 Pablo (ToxicWaste) - Agrego de funciones y mejora de distribución de parámetros.
		'*************************************************
		Dim CharFile As String
		Dim Ban As String
		Dim BanDetailPath As String
		
		BanDetailPath = My.Application.Info.DirectoryPath & "\logs\" & "BanDetail.dat"
		CharFile = CharPath & charName & ".chr"
		
		If FileExist(CharFile) Then
			Call WriteConsoleMsg(sendIndex, "Pj: " & charName, Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Ciudadanos matados: " & GetVar(CharFile, "FACCIONES", "CiudMatados") & " CriminalesMatados: " & GetVar(CharFile, "FACCIONES", "CrimMatados") & " usuarios matados: " & GetVar(CharFile, "MUERTES", "UserMuertes"), Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "NPCs muertos: " & GetVar(CharFile, "MUERTES", "NpcsMuertes"), Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Clase: " & ListaClases(CInt(GetVar(CharFile, "INIT", "Clase"))), Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Pena: " & GetVar(CharFile, "COUNTERS", "PENA"), Protocol.FontTypeNames.FONTTYPE_INFO)
			
			If CByte(GetVar(CharFile, "FACCIONES", "EjercitoReal")) = 1 Then
				Call WriteConsoleMsg(sendIndex, "Ejército real desde: " & GetVar(CharFile, "FACCIONES", "FechaIngreso"), Protocol.FontTypeNames.FONTTYPE_INFO)
				Call WriteConsoleMsg(sendIndex, "Ingresó en nivel: " & CShort(GetVar(CharFile, "FACCIONES", "NivelIngreso")) & " con " & CShort(GetVar(CharFile, "FACCIONES", "MatadosIngreso")) & " ciudadanos matados.", Protocol.FontTypeNames.FONTTYPE_INFO)
				Call WriteConsoleMsg(sendIndex, "Veces que ingresó: " & CByte(GetVar(CharFile, "FACCIONES", "Reenlistadas")), Protocol.FontTypeNames.FONTTYPE_INFO)
				
			ElseIf CByte(GetVar(CharFile, "FACCIONES", "EjercitoCaos")) = 1 Then 
				Call WriteConsoleMsg(sendIndex, "Legión oscura desde: " & GetVar(CharFile, "FACCIONES", "FechaIngreso"), Protocol.FontTypeNames.FONTTYPE_INFO)
				Call WriteConsoleMsg(sendIndex, "Ingresó en nivel: " & CShort(GetVar(CharFile, "FACCIONES", "NivelIngreso")), Protocol.FontTypeNames.FONTTYPE_INFO)
				Call WriteConsoleMsg(sendIndex, "Veces que ingresó: " & CByte(GetVar(CharFile, "FACCIONES", "Reenlistadas")), Protocol.FontTypeNames.FONTTYPE_INFO)
				
			ElseIf CByte(GetVar(CharFile, "FACCIONES", "rExReal")) = 1 Then 
				Call WriteConsoleMsg(sendIndex, "Fue ejército real", Protocol.FontTypeNames.FONTTYPE_INFO)
				Call WriteConsoleMsg(sendIndex, "Veces que ingresó: " & CByte(GetVar(CharFile, "FACCIONES", "Reenlistadas")), Protocol.FontTypeNames.FONTTYPE_INFO)
				
			ElseIf CByte(GetVar(CharFile, "FACCIONES", "rExCaos")) = 1 Then 
				Call WriteConsoleMsg(sendIndex, "Fue legión oscura", Protocol.FontTypeNames.FONTTYPE_INFO)
				Call WriteConsoleMsg(sendIndex, "Veces que ingresó: " & CByte(GetVar(CharFile, "FACCIONES", "Reenlistadas")), Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
			
			
			Call WriteConsoleMsg(sendIndex, "Asesino: " & CInt(GetVar(CharFile, "REP", "Asesino")), Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Noble: " & CInt(GetVar(CharFile, "REP", "Nobles")), Protocol.FontTypeNames.FONTTYPE_INFO)
			
			If IsNumeric(GetVar(CharFile, "Guild", "GUILDINDEX")) Then
				Call WriteConsoleMsg(sendIndex, "Clan: " & modGuilds.GuildName(CShort(GetVar(CharFile, "Guild", "GUILDINDEX"))), Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
			
			Ban = GetVar(CharFile, "FLAGS", "Ban")
			Call WriteConsoleMsg(sendIndex, "Ban: " & Ban, Protocol.FontTypeNames.FONTTYPE_INFO)
			
			If Ban = "1" Then
				Call WriteConsoleMsg(sendIndex, "Ban por: " & GetVar(CharFile, charName, "BannedBy") & " Motivo: " & GetVar(BanDetailPath, charName, "Reason"), Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
		Else
			Call WriteConsoleMsg(sendIndex, "El pj no existe: " & charName, Protocol.FontTypeNames.FONTTYPE_INFO)
		End If
	End Sub
	
	Sub SendUserInvTxt(ByVal sendIndex As Short, ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error Resume Next
		
		Dim j As Integer
		
		With UserList(UserIndex)
			Call WriteConsoleMsg(sendIndex, .name, Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Tiene " & .Invent.NroItems & " objetos.", Protocol.FontTypeNames.FONTTYPE_INFO)
			
			For j = 1 To .CurrentInventorySlots
				If .Invent.Object_Renamed(j).ObjIndex > 0 Then
					Call WriteConsoleMsg(sendIndex, "Objeto " & j & " " & ObjData_Renamed(.Invent.Object_Renamed(j).ObjIndex).name & " Cantidad:" & .Invent.Object_Renamed(j).Amount, Protocol.FontTypeNames.FONTTYPE_INFO)
				End If
			Next j
		End With
	End Sub
	
	Sub SendUserInvTxtFromChar(ByVal sendIndex As Short, ByVal charName As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error Resume Next
		
		Dim j As Integer
		Dim CharFile, Tmp As String
		Dim ObjInd, ObjCant As Integer
		
		CharFile = CharPath & charName & ".chr"
		
		If FileExist(CharFile) Then
			Call WriteConsoleMsg(sendIndex, charName, Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Tiene " & GetVar(CharFile, "Inventory", "CantidadItems") & " objetos.", Protocol.FontTypeNames.FONTTYPE_INFO)
			
			For j = 1 To MAX_INVENTORY_SLOTS
				Tmp = GetVar(CharFile, "Inventory", "Obj" & j)
				ObjInd = CInt(ReadField(1, Tmp, Asc("-")))
				ObjCant = CInt(ReadField(2, Tmp, Asc("-")))
				If ObjInd > 0 Then
					Call WriteConsoleMsg(sendIndex, "Objeto " & j & " " & ObjData_Renamed(ObjInd).name & " Cantidad:" & ObjCant, Protocol.FontTypeNames.FONTTYPE_INFO)
				End If
			Next j
		Else
			Call WriteConsoleMsg(sendIndex, "Usuario inexistente: " & charName, Protocol.FontTypeNames.FONTTYPE_INFO)
		End If
	End Sub
	
	Sub SendUserSkillsTxt(ByVal sendIndex As Short, ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error Resume Next
		Dim j As Short
		
		Call WriteConsoleMsg(sendIndex, UserList(UserIndex).name, Protocol.FontTypeNames.FONTTYPE_INFO)
		
		For j = 1 To NUMSKILLS
			Call WriteConsoleMsg(sendIndex, SkillsNames(j) & " = " & UserList(UserIndex).Stats.UserSkills(j), Protocol.FontTypeNames.FONTTYPE_INFO)
		Next j
		
		Call WriteConsoleMsg(sendIndex, "SkillLibres:" & UserList(UserIndex).Stats.SkillPts, Protocol.FontTypeNames.FONTTYPE_INFO)
	End Sub
	
	Private Function EsMascotaCiudadano(ByVal NpcIndex As Short, ByVal UserIndex As Short) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		If Npclist(NpcIndex).MaestroUser > 0 Then
			EsMascotaCiudadano = Not criminal(Npclist(NpcIndex).MaestroUser)
			If EsMascotaCiudadano Then
				Call WriteConsoleMsg(Npclist(NpcIndex).MaestroUser, "¡¡" & UserList(UserIndex).name & " esta atacando tu mascota!!", Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
		End If
	End Function
	
	Sub NPCAtacado(ByVal NpcIndex As Short, ByVal UserIndex As Short)
		'**********************************************
		'Author: Unknown
		'Last Modification: 02/04/2010
		'24/01/2007 -> Pablo (ToxicWaste): Agrego para que se actualize el tag si corresponde.
		'24/07/2007 -> Pablo (ToxicWaste): Guardar primero que ataca NPC y el que atacas ahora.
		'06/28/2008 -> NicoNZ: Los elementales al atacarlos por su amo no se paran más al lado de él sin hacer nada.
		'02/04/2010: ZaMa: Un ciuda no se vuelve mas criminal al atacar un npc no hostil.
		'**********************************************
		Dim EraCriminal As Boolean
		
		'Guardamos el usuario que ataco el npc.
		Npclist(NpcIndex).flags.AttackedBy = UserList(UserIndex).name
		
		'Npc que estabas atacando.
		Dim LastNpcHit As Short
		LastNpcHit = UserList(UserIndex).flags.NPCAtacado
		'Guarda el NPC que estas atacando ahora.
		UserList(UserIndex).flags.NPCAtacado = NpcIndex
		
		'Revisamos robo de npc.
		'Guarda el primer nick que lo ataca.
		If Npclist(NpcIndex).flags.AttackedFirstBy = vbNullString Then
			'El que le pegabas antes ya no es tuyo
			If LastNpcHit <> 0 Then
				If Npclist(LastNpcHit).flags.AttackedFirstBy = UserList(UserIndex).name Then
					Npclist(LastNpcHit).flags.AttackedFirstBy = vbNullString
				End If
			End If
			Npclist(NpcIndex).flags.AttackedFirstBy = UserList(UserIndex).name
		ElseIf Npclist(NpcIndex).flags.AttackedFirstBy <> UserList(UserIndex).name Then 
			'Estas robando NPC
			'El que le pegabas antes ya no es tuyo
			If LastNpcHit <> 0 Then
				If Npclist(LastNpcHit).flags.AttackedFirstBy = UserList(UserIndex).name Then
					Npclist(LastNpcHit).flags.AttackedFirstBy = vbNullString
				End If
			End If
		End If
		
		If Npclist(NpcIndex).MaestroUser > 0 Then
			If Npclist(NpcIndex).MaestroUser <> UserIndex Then
				Call AllMascotasAtacanUser(UserIndex, Npclist(NpcIndex).MaestroUser)
			End If
		End If
		
		If EsMascotaCiudadano(NpcIndex, UserIndex) Then
			Call VolverCriminal(UserIndex)
			Npclist(NpcIndex).Movement = AI.TipoAI.NPCDEFENSA
			Npclist(NpcIndex).Hostile = 1
		Else
			EraCriminal = criminal(UserIndex)
			
			'Reputacion
			If Npclist(NpcIndex).Stats.Alineacion = 0 Then
				If Npclist(NpcIndex).NPCtype = Declaraciones.eNPCType.GuardiaReal Then
					Call VolverCriminal(UserIndex)
				End If
				
			ElseIf Npclist(NpcIndex).Stats.Alineacion = 1 Then 
				UserList(UserIndex).Reputacion.PlebeRep = UserList(UserIndex).Reputacion.PlebeRep + vlCAZADOR / 2
				If UserList(UserIndex).Reputacion.PlebeRep > MAXREP Then UserList(UserIndex).Reputacion.PlebeRep = MAXREP
			End If
			
			If Npclist(NpcIndex).MaestroUser <> UserIndex Then
				'hacemos que el npc se defienda
				Npclist(NpcIndex).Movement = AI.TipoAI.NPCDEFENSA
				Npclist(NpcIndex).Hostile = 1
			End If
			
			If EraCriminal And Not criminal(UserIndex) Then
				Call VolverCiudadano(UserIndex)
			End If
		End If
	End Sub
	Public Function PuedeApuñalar(ByVal UserIndex As Short) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		If UserList(UserIndex).Invent.WeaponEqpObjIndex > 0 Then
			If ObjData_Renamed(UserList(UserIndex).Invent.WeaponEqpObjIndex).Apuñala = 1 Then
				PuedeApuñalar = UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Apuñalar) >= MIN_APUÑALAR Or UserList(UserIndex).clase = Declaraciones.eClass.Assasin
			End If
		End If
	End Function
	
	Public Function PuedeAcuchillar(ByVal UserIndex As Short) As Boolean
		'***************************************************
		'Author: ZaMa
		'Last Modification: 25/01/2010 (ZaMa)
		'
		'***************************************************
		
		With UserList(UserIndex)
			If .clase = Declaraciones.eClass.Pirat Then
				If .Invent.WeaponEqpObjIndex > 0 Then
					PuedeAcuchillar = (ObjData_Renamed(.Invent.WeaponEqpObjIndex).Acuchilla = 1)
				End If
			End If
		End With
		
	End Function
	
	Sub SubirSkill(ByVal UserIndex As Short, ByVal Skill As Short, ByVal Acerto As Boolean)
		'*************************************************
		'Author: Unknown
		'Last modified: 11/19/2009
		'11/19/2009 Pato - Implement the new system to train the skills.
		'*************************************************
		Dim Lvl As Short
		With UserList(UserIndex)
			If .flags.Hambre = 0 And .flags.Sed = 0 Then
				If .Counters.AsignedSkills < 10 Then
					If Not .flags.UltimoMensaje = 7 Then
						Call WriteConsoleMsg(UserIndex, "Para poder entrenar un skill debes asignar los 10 skills iniciales.", Protocol.FontTypeNames.FONTTYPE_INFO)
						.flags.UltimoMensaje = 7
					End If
					
					Exit Sub
				End If
				
				With .Stats
					If .UserSkills(Skill) = MAXSKILLPOINTS Then Exit Sub
					
					Lvl = .ELV
					
					If Lvl > UBound(LevelSkill_Renamed) Then Lvl = UBound(LevelSkill_Renamed)
					
					If .UserSkills(Skill) >= LevelSkill_Renamed(Lvl).LevelValue Then Exit Sub
					
					If Acerto Then
						.ExpSkills(Skill) = .ExpSkills(Skill) + EXP_ACIERTO_SKILL
					Else
						.ExpSkills(Skill) = .ExpSkills(Skill) + EXP_FALLO_SKILL
					End If
					
					If .ExpSkills(Skill) >= .EluSkills(Skill) Then
						.UserSkills(Skill) = .UserSkills(Skill) + 1
						Call WriteConsoleMsg(UserIndex, "¡Has mejorado tu skill " & SkillsNames(Skill) & " en un punto! Ahora tienes " & .UserSkills(Skill) & " pts.", Protocol.FontTypeNames.FONTTYPE_INFO)
						
						.Exp = .Exp + 50
						If .Exp > MAXEXP Then .Exp = MAXEXP
						
						Call WriteConsoleMsg(UserIndex, "¡Has ganado 50 puntos de experiencia!", Protocol.FontTypeNames.FONTTYPE_FIGHT)
						
						Call WriteUpdateExp(UserIndex)
						Call CheckUserLevel(UserIndex)
						Call CheckEluSkill(UserIndex, Skill, False)
					End If
				End With
			End If
		End With
	End Sub
	
	''
	' Muere un usuario
	'
	' @param UserIndex  Indice del usuario que muere
	'
	
	Sub UserDie(ByVal UserIndex As Short)
		'************************************************
		'Author: Uknown
		'Last Modified: 12/01/2010 (ZaMa)
		'04/15/2008: NicoNZ - Ahora se resetea el counter del invi
		'13/02/2009: ZaMa - Ahora se borran las mascotas cuando moris en agua.
		'27/05/2009: ZaMa - El seguro de resu no se activa si estas en una arena.
		'21/07/2009: Marco - Al morir se desactiva el comercio seguro.
		'16/11/2009: ZaMa - Al morir perdes la criatura que te pertenecia.
		'27/11/2009: Budi - Al morir envia los atributos originales.
		'12/01/2010: ZaMa - Los druidas pierden la inmunidad de ser atacados cuando mueren.
		'************************************************
		On Error GoTo ErrorHandler
		Dim i As Integer
		Dim aN As Short
		
		With UserList(UserIndex)
			'Sonido
			If .Genero = Declaraciones.eGenero.Mujer Then
				Call SonidosMapas.ReproducirSonido(modSendData.SendTarget.ToPCArea, UserIndex, SoundMapInfo.e_SoundIndex.MUERTE_MUJER)
			Else
				Call SonidosMapas.ReproducirSonido(modSendData.SendTarget.ToPCArea, UserIndex, SoundMapInfo.e_SoundIndex.MUERTE_HOMBRE)
			End If
			
			'Quitar el dialogo del user muerto
			Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageRemoveCharDialog(.Char_Renamed.CharIndex))
			
			.Stats.MinHp = 0
			.Stats.MinSta = 0
			.flags.AtacadoPorUser = 0
			.flags.Envenenado = 0
			.flags.Muerto = 1
			' No se activa en arenas
			If TriggerZonaPelea(UserIndex, UserIndex) <> Declaraciones.eTrigger6.TRIGGER6_PERMITE Then
				.flags.SeguroResu = True
				Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.ResuscitationSafeOn) 'Call WriteResuscitationSafeOn(UserIndex)
			Else
				.flags.SeguroResu = False
				Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.ResuscitationSafeOff) 'Call WriteResuscitationSafeOff(UserIndex)
			End If
			
			aN = .flags.AtacadoPorNpc
			If aN > 0 Then
				Npclist(aN).Movement = Npclist(aN).flags.OldMovement
				Npclist(aN).Hostile = Npclist(aN).flags.OldHostil
				Npclist(aN).flags.AttackedBy = vbNullString
			End If
			
			aN = .flags.NPCAtacado
			If aN > 0 Then
				If Npclist(aN).flags.AttackedFirstBy = .name Then
					Npclist(aN).flags.AttackedFirstBy = vbNullString
				End If
			End If
			.flags.AtacadoPorNpc = 0
			.flags.NPCAtacado = 0
			Call PerdioNpc(UserIndex)
			
			'<<<< Atacable >>>>
			If .flags.AtacablePor > 0 Then
				.flags.AtacablePor = 0
				Call RefreshCharStatus(UserIndex)
			End If
			
			'<<<< Paralisis >>>>
			If .flags.Paralizado = 1 Then
				.flags.Paralizado = 0
				Call WriteParalizeOK(UserIndex)
			End If
			
			'<<< Estupidez >>>
			If .flags.Estupidez = 1 Then
				.flags.Estupidez = 0
				Call WriteDumbNoMore(UserIndex)
			End If
			
			'<<<< Descansando >>>>
			If .flags.Descansar Then
				.flags.Descansar = False
				Call WriteRestOK(UserIndex)
			End If
			
			'<<<< Meditando >>>>
			If .flags.Meditando Then
				.flags.Meditando = False
				Call WriteMeditateToggle(UserIndex)
			End If
			
			'<<<< Invisible >>>>
			If .flags.invisible = 1 Or .flags.Oculto = 1 Then
				.flags.Oculto = 0
				.flags.invisible = 0
				.Counters.TiempoOculto = 0
				.Counters.Invisibilidad = 0
				
				'Call SendData(SendTarget.ToPCArea, UserIndex, PrepareMessageSetInvisible(.Char.CharIndex, False))
				Call SetInvisible(UserIndex, UserList(UserIndex).Char_Renamed.CharIndex, False)
			End If
			
			If TriggerZonaPelea(UserIndex, UserIndex) <> Declaraciones.eTrigger6.TRIGGER6_PERMITE Then
				' << Si es newbie no pierde el inventario >>
				If Not EsNewbie(UserIndex) Then
					Call TirarTodo(UserIndex)
				Else
					Call TirarTodosLosItemsNoNewbies(UserIndex)
				End If
			End If
			
			' DESEQUIPA TODOS LOS OBJETOS
			'desequipar armadura
			If .Invent.ArmourEqpObjIndex > 0 Then
				Call Desequipar(UserIndex, .Invent.ArmourEqpSlot)
			End If
			
			'desequipar arma
			If .Invent.WeaponEqpObjIndex > 0 Then
				Call Desequipar(UserIndex, .Invent.WeaponEqpSlot)
			End If
			
			'desequipar casco
			If .Invent.CascoEqpObjIndex > 0 Then
				Call Desequipar(UserIndex, .Invent.CascoEqpSlot)
			End If
			
			'desequipar herramienta
			If .Invent.AnilloEqpSlot > 0 Then
				Call Desequipar(UserIndex, .Invent.AnilloEqpSlot)
			End If
			
			'desequipar municiones
			If .Invent.MunicionEqpObjIndex > 0 Then
				Call Desequipar(UserIndex, .Invent.MunicionEqpSlot)
			End If
			
			'desequipar escudo
			If .Invent.EscudoEqpObjIndex > 0 Then
				Call Desequipar(UserIndex, .Invent.EscudoEqpSlot)
			End If
			
			' << Reseteamos los posibles FX sobre el personaje >>
			If .Char_Renamed.loops = INFINITE_LOOPS Then
				.Char_Renamed.FX = 0
				.Char_Renamed.loops = 0
			End If
			
			' << Restauramos el mimetismo
			If .flags.Mimetizado = 1 Then
				.Char_Renamed.body = .CharMimetizado.body
				.Char_Renamed.Head = .CharMimetizado.Head
				.Char_Renamed.CascoAnim = .CharMimetizado.CascoAnim
				.Char_Renamed.ShieldAnim = .CharMimetizado.ShieldAnim
				.Char_Renamed.WeaponAnim = .CharMimetizado.WeaponAnim
				.Counters.Mimetismo = 0
				.flags.Mimetizado = 0
				' Puede ser atacado por npcs (cuando resucite)
				.flags.Ignorado = False
			End If
			
			' << Restauramos los atributos >>
			If .flags.TomoPocion = True Then
				For i = 1 To 5
					.Stats.UserAtributos(i) = .Stats.UserAtributosBackUP(i)
				Next i
			End If
			
			'<< Cambiamos la apariencia del char >>
			If .flags.Navegando = 0 Then
				.Char_Renamed.body = iCuerpoMuerto
				.Char_Renamed.Head = iCabezaMuerto
				.Char_Renamed.ShieldAnim = NingunEscudo
				.Char_Renamed.WeaponAnim = NingunArma
				.Char_Renamed.CascoAnim = NingunCasco
			Else
				.Char_Renamed.body = iFragataFantasmal
			End If
			
			For i = 1 To MAXMASCOTAS
				If .MascotasIndex(i) > 0 Then
					Call MuereNpc(.MascotasIndex(i), 0)
					' Si estan en agua o zona segura
				Else
					.MascotasType(i) = 0
				End If
			Next i
			
			.NroMascotas = 0
			
			'<< Actualizamos clientes >>
			Call ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading, NingunArma, NingunEscudo, NingunCasco)
			Call WriteUpdateUserStats(UserIndex)
			Call WriteUpdateStrenghtAndDexterity(UserIndex)
			'<<Castigos por party>>
			If .PartyIndex > 0 Then
				Call mdParty.ObtenerExito(UserIndex, .Stats.ELV * -10 * mdParty.CantMiembros(UserIndex), .Pos.Map, .Pos.X, .Pos.Y)
			End If
			
			'<<Cerramos comercio seguro>>
			Call LimpiarComercioSeguro(UserIndex)
		End With
		Exit Sub
		
ErrorHandler: 
		Call LogError("Error en SUB USERDIE. Error: " & Err.Number & " Descripción: " & Err.Description)
	End Sub
	
	Sub ContarMuerte(ByVal Muerto As Short, ByVal Atacante As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		If EsNewbie(Muerto) Then Exit Sub
		
		With UserList(Atacante)
			If TriggerZonaPelea(Muerto, Atacante) = Declaraciones.eTrigger6.TRIGGER6_PERMITE Then Exit Sub
			
			If criminal(Muerto) Then
				If .flags.LastCrimMatado <> UserList(Muerto).name Then
					.flags.LastCrimMatado = UserList(Muerto).name
					If .Faccion.CriminalesMatados < MAXUSERMATADOS Then .Faccion.CriminalesMatados = .Faccion.CriminalesMatados + 1
				End If
				
				If .Faccion.RecibioExpInicialCaos = 1 And UserList(Muerto).Faccion.FuerzasCaos = 1 Then
					.Faccion.Reenlistadas = 200 'jaja que trucho
					
					'con esto evitamos que se vuelva a reenlistar
				End If
			Else
				If .flags.LastCiudMatado <> UserList(Muerto).name Then
					.flags.LastCiudMatado = UserList(Muerto).name
					If .Faccion.CiudadanosMatados < MAXUSERMATADOS Then .Faccion.CiudadanosMatados = .Faccion.CiudadanosMatados + 1
				End If
			End If
			
			If .Stats.UsuariosMatados < MAXUSERMATADOS Then .Stats.UsuariosMatados = .Stats.UsuariosMatados + 1
		End With
	End Sub
	
	Sub Tilelibre(ByRef Pos As WorldPos, ByRef nPos As WorldPos, ByRef Obj As Obj, ByRef Agua As Boolean, ByRef Tierra As Boolean)
		'**************************************************************
		'Author: Unknown
		'Last Modify Date: 23/01/2007
		'23/01/2007 -> Pablo (ToxicWaste): El agua es ahora un TileLibre agregando las condiciones necesarias.
		'**************************************************************
		Dim LoopC As Short
		Dim tX As Integer
		Dim tY As Integer
		Dim hayobj As Boolean
		
		hayobj = False
		nPos.Map = Pos.Map
		nPos.X = 0
		nPos.Y = 0
		
		Do While Not LegalPos(Pos.Map, nPos.X, nPos.Y, Agua, Tierra) Or hayobj
			
			If LoopC > 15 Then
				Exit Do
			End If
			
			For tY = Pos.Y - LoopC To Pos.Y + LoopC
				For tX = Pos.X - LoopC To Pos.X + LoopC
					
					If LegalPos(nPos.Map, tX, tY, Agua, Tierra) Then
						'We continue if: a - the item is different from 0 and the dropped item or b - the amount dropped + amount in map exceeds MAX_INVENTORY_OBJS
						hayobj = (MapData(nPos.Map, tX, tY).ObjInfo.ObjIndex > 0 And MapData(nPos.Map, tX, tY).ObjInfo.ObjIndex <> Obj.ObjIndex)
						If Not hayobj Then hayobj = (MapData(nPos.Map, tX, tY).ObjInfo.Amount + Obj.Amount > MAX_INVENTORY_OBJS)
						If Not hayobj And MapData(nPos.Map, tX, tY).TileExit.Map = 0 Then
							nPos.X = tX
							nPos.Y = tY
							
							'break both fors
							tX = Pos.X + LoopC
							tY = Pos.Y + LoopC
						End If
					End If
					
				Next tX
			Next tY
			
			LoopC = LoopC + 1
		Loop 
	End Sub
	
	Sub WarpUserChar(ByVal UserIndex As Short, ByVal Map As Short, ByVal X As Short, ByVal Y As Short, ByVal FX As Boolean, Optional ByVal Teletransported As Boolean = False)
		'**************************************************************
		'Author: Unknown
		'Last Modify Date: 13/11/2009
		'15/07/2009 - ZaMa: Automatic toogle navigate after warping to water.
		'13/11/2009 - ZaMa: Now it's activated the timer which determines if the npc can atacak the user.
		'**************************************************************
		Dim OldMap As Short
		Dim OldX As Short
		Dim OldY As Short
		
		Dim nextMap As Object
		Dim previousMap As Boolean
		With UserList(UserIndex)
			'Quitar el dialogo
			Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageRemoveCharDialog(.Char_Renamed.CharIndex))
			
			Call WriteRemoveAllDialogs(UserIndex)
			
			OldMap = .Pos.Map
			OldX = .Pos.X
			OldY = .Pos.Y
			
			Call EraseUserChar(UserIndex, .flags.AdminInvisible = 1)
			
			If OldMap <> Map Then
				Call WriteChangeMap(UserIndex, Map, MapInfo_Renamed(.Pos.Map).MapVersion)
				Call WritePlayMidi(UserIndex, Val(ReadField(1, MapInfo_Renamed(Map).Music, 45)))
				
				'Update new Map Users
				MapInfo_Renamed(Map).NumUsers = MapInfo_Renamed(Map).NumUsers + 1
				
				'Update old Map Users
				MapInfo_Renamed(OldMap).NumUsers = MapInfo_Renamed(OldMap).NumUsers - 1
				If MapInfo_Renamed(OldMap).NumUsers < 0 Then
					MapInfo_Renamed(OldMap).NumUsers = 0
				End If
				
				'Si el mapa al que entro NO ES superficial AND en el que estaba TAMPOCO ES superficial, ENTONCES
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nextMap. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				nextMap = IIf(distanceToCities(Map).distanceToCity(.Hogar) >= 0, True, False)
				previousMap = IIf(distanceToCities(.Pos.Map).distanceToCity(.Hogar) >= 0, True, False)
				
				If previousMap And nextMap Then '138 => 139 (Ambos superficiales, no tiene que pasar nada)
					'NO PASA NADA PORQUE NO ENTRO A UN DUNGEON.
				ElseIf previousMap And Not nextMap Then  '139 => 140 (139 es superficial, 140 no. Por lo tanto 139 es el ultimo mapa superficial)
					.flags.lastMap = .Pos.Map
				ElseIf Not previousMap And nextMap Then  '140 => 139 (140 es no es superficial, 139 si. Por lo tanto, el último mapa es 0 ya que no esta en un dungeon)
					.flags.lastMap = 0
				ElseIf Not previousMap And Not nextMap Then  '140 => 141 (Ninguno es superficial, el ultimo mapa es el mismo de antes)
					.flags.lastMap = .flags.lastMap
				End If
				
			End If
			
			.Pos.X = X
			.Pos.Y = Y
			.Pos.Map = Map
			
			Call MakeUserChar(True, Map, UserIndex, Map, X, Y)
			Call WriteUserCharIndexInServer(UserIndex)
			
			'Force a flush, so user index is in there before it's destroyed for teleporting
			Call FlushBuffer(UserIndex)
			
			'Seguis invisible al pasar de mapa
			If (.flags.invisible = 1 Or .flags.Oculto = 1) And (Not .flags.AdminInvisible = 1) Then
				Call SetInvisible(UserIndex, .Char_Renamed.CharIndex, True)
				'Call SendData(SendTarget.ToPCArea, UserIndex, PrepareMessageSetInvisible(.Char.CharIndex, True))
			End If
			
			If Teletransported Then
				If .flags.Traveling = 1 Then
					.flags.Traveling = 0
					.Counters.goHome = 0
					Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.CancelHome)
				End If
			End If
			
			If FX And .flags.AdminInvisible = 0 Then 'FX
				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessagePlayWave(SND_WARP, X, Y))
				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageCreateFX(.Char_Renamed.CharIndex, Declaraciones.FXIDs.FXWARP, 0))
			End If
			
			If .NroMascotas Then Call WarpMascotas(UserIndex)
			
			' No puede ser atacado cuando cambia de mapa, por cierto tiempo
			Call IntervaloPermiteSerAtacado(UserIndex, True)
			
			' Perdes el npc al cambiar de mapa
			Call PerdioNpc(UserIndex)
			
			' Automatic toogle navigate
			If (.flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero)) = 0 Then
				If HayAgua(.Pos.Map, .Pos.X, .Pos.Y) Then
					If .flags.Navegando = 0 Then
						.flags.Navegando = 1
						
						'Tell the client that we are navigating.
						Call WriteNavigateToggle(UserIndex)
					End If
				Else
					If .flags.Navegando = 1 Then
						.flags.Navegando = 0
						
						'Tell the client that we are navigating.
						Call WriteNavigateToggle(UserIndex)
					End If
				End If
			End If
			
		End With
	End Sub
	
	Private Sub WarpMascotas(ByVal UserIndex As Short)
		'************************************************
		'Author: Uknown
		'Last Modified: 11/05/2009
		'13/02/2009: ZaMa - Arreglado respawn de mascotas al cambiar de mapa.
		'13/02/2009: ZaMa - Las mascotas no regeneran su vida al cambiar de mapa (Solo entre mapas inseguros).
		'11/05/2009: ZaMa - Chequeo si la mascota pueden spwnear para asiganrle los stats.
		'************************************************
		Dim i As Short
		Dim petType As Short
		Dim PetRespawn As Boolean
		Dim PetTiempoDeVida As Short
		Dim NroPets As Short
		Dim InvocadosMatados As Short
		Dim canWarp As Boolean
		Dim Index As Short
		Dim iMinHP As Short
		
		NroPets = UserList(UserIndex).NroMascotas
		canWarp = (MapInfo_Renamed(UserList(UserIndex).Pos.Map).Pk = True)
		
		For i = 1 To MAXMASCOTAS
			Index = UserList(UserIndex).MascotasIndex(i)
			
			If Index > 0 Then
				' si la mascota tiene tiempo de vida > 0 significa q fue invocada => we kill it
				If Npclist(Index).Contadores.TiempoExistencia > 0 Then
					Call QuitarNPC(Index)
					UserList(UserIndex).MascotasIndex(i) = 0
					InvocadosMatados = InvocadosMatados + 1
					NroPets = NroPets - 1
					
					petType = 0
				Else
					'Store data and remove NPC to recreate it after warp
					'PetRespawn = Npclist(index).flags.Respawn = 0
					petType = UserList(UserIndex).MascotasType(i)
					'PetTiempoDeVida = Npclist(index).Contadores.TiempoExistencia
					
					' Guardamos el hp, para restaurarlo uando se cree el npc
					iMinHP = Npclist(Index).Stats.MinHp
					
					Call QuitarNPC(Index)
					
					' Restauramos el valor de la variable
					UserList(UserIndex).MascotasType(i) = petType
					
				End If
			ElseIf UserList(UserIndex).MascotasType(i) > 0 Then 
				'Store data and remove NPC to recreate it after warp
				PetRespawn = True
				petType = UserList(UserIndex).MascotasType(i)
				PetTiempoDeVida = 0
			Else
				petType = 0
			End If
			
			If petType > 0 And canWarp Then
				Index = SpawnNpc(petType, UserList(UserIndex).Pos, False, PetRespawn)
				
				'Controlamos que se sumoneo OK - should never happen. Continue to allow removal of other pets if not alone
				' Exception: Pets don't spawn in water if they can't swim
				If Index = 0 Then
					Call WriteConsoleMsg(UserIndex, "Tus mascotas no pueden transitar este mapa.", Protocol.FontTypeNames.FONTTYPE_INFO)
				Else
					UserList(UserIndex).MascotasIndex(i) = Index
					
					' Nos aseguramos de que conserve el hp, si estaba dañado
					Npclist(Index).Stats.MinHp = IIf(iMinHP = 0, Npclist(Index).Stats.MinHp, iMinHP)
					
					Npclist(Index).MaestroUser = UserIndex
					Npclist(Index).Contadores.TiempoExistencia = PetTiempoDeVida
					Call FollowAmo(Index)
				End If
			End If
		Next i
		
		If InvocadosMatados > 0 Then
			Call WriteConsoleMsg(UserIndex, "Pierdes el control de tus mascotas invocadas.", Protocol.FontTypeNames.FONTTYPE_INFO)
		End If
		
		If Not canWarp Then
			Call WriteConsoleMsg(UserIndex, "No se permiten mascotas en zona segura. Éstas te esperarán afuera.", Protocol.FontTypeNames.FONTTYPE_INFO)
		End If
		
		UserList(UserIndex).NroMascotas = NroPets
	End Sub
	
	Public Sub WarpMascota(ByVal UserIndex As Short, ByVal PetIndex As Short)
		'************************************************
		'Author: ZaMa
		'Last Modified: 18/11/2009
		'Warps a pet without changing its stats
		'************************************************
		Dim petType As Short
		Dim NpcIndex As Short
		Dim iMinHP As Short
		Dim TargetPos As WorldPos
		
		With UserList(UserIndex)
			
			TargetPos.Map = .flags.TargetMap
			TargetPos.X = .flags.TargetX
			TargetPos.Y = .flags.TargetY
			
			NpcIndex = .MascotasIndex(PetIndex)
			
			'Store data and remove NPC to recreate it after warp
			petType = .MascotasType(PetIndex)
			
			' Guardamos el hp, para restaurarlo cuando se cree el npc
			iMinHP = Npclist(NpcIndex).Stats.MinHp
			
			Call QuitarNPC(NpcIndex)
			
			' Restauramos el valor de la variable
			.MascotasType(PetIndex) = petType
			.NroMascotas = .NroMascotas + 1
			NpcIndex = SpawnNpc(petType, TargetPos, False, False)
			
			'Controlamos que se sumoneo OK - should never happen. Continue to allow removal of other pets if not alone
			' Exception: Pets don't spawn in water if they can't swim
			If NpcIndex = 0 Then
				Call WriteConsoleMsg(UserIndex, "Tu mascota no pueden transitar este sector del mapa, intenta invocarla en otra parte.", Protocol.FontTypeNames.FONTTYPE_INFO)
			Else
				.MascotasIndex(PetIndex) = NpcIndex
				
				With Npclist(NpcIndex)
					' Nos aseguramos de que conserve el hp, si estaba dañado
					.Stats.MinHp = IIf(iMinHP = 0, .Stats.MinHp, iMinHP)
					
					.MaestroUser = UserIndex
					.Movement = AI.TipoAI.SigueAmo
					.Target = 0
					.TargetNPC = 0
				End With
				
				Call FollowAmo(NpcIndex)
			End If
		End With
	End Sub
	
	
	''
	' Se inicia la salida de un usuario.
	'
	' @param    UserIndex   El index del usuario que va a salir
	
	Sub Cerrar_Usuario(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: 09/04/08 (NicoNZ)
		'
		'***************************************************
		Dim isNotVisible As Boolean
		Dim HiddenPirat As Boolean
		
		With UserList(UserIndex)
			If .flags.UserLogged And Not .Counters.Saliendo Then
				.Counters.Saliendo = True
				.Counters.salir = IIf((.flags.Privilegios And Declaraciones.PlayerType.User) And MapInfo_Renamed(.Pos.Map).Pk, IntervaloCerrarConexion, 0)
				
				isNotVisible = (.flags.Oculto Or .flags.invisible)
				If isNotVisible Then
					.flags.invisible = 0
					
					If .flags.Oculto Then
						If .flags.Navegando = 1 Then
							If .clase = Declaraciones.eClass.Pirat Then
								' Pierde la apariencia de fragata fantasmal
								Call ToogleBoatBody(UserIndex)
								Call WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!", Protocol.FontTypeNames.FONTTYPE_INFO)
								Call ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading, NingunArma, NingunEscudo, NingunCasco)
								HiddenPirat = True
							End If
						End If
					End If
					
					.flags.Oculto = 0
					
					' Para no repetir mensajes
					If Not HiddenPirat Then Call WriteConsoleMsg(UserIndex, "Has vuelto a ser visible.", Protocol.FontTypeNames.FONTTYPE_INFO)
					
					Call SetInvisible(UserIndex, .Char_Renamed.CharIndex, False)
					
				End If
				
				If .flags.Traveling = 1 Then
					Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.CancelHome)
				End If
				
				Call WriteConsoleMsg(UserIndex, "Cerrando...Se cerrará el juego en " & .Counters.salir & " segundos...", Protocol.FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub
	
	''
	' Cancels the exit of a user. If it's disconnected it's reset.
	'
	' @param    UserIndex   The index of the user whose exit is being reset.
	
	Public Sub CancelExit(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 04/02/08
		'
		'***************************************************
		If UserList(UserIndex).Counters.Saliendo Then
			' Is the user still connected?
			If UserList(UserIndex).ConnIDValida Then
				UserList(UserIndex).Counters.Saliendo = False
				UserList(UserIndex).Counters.salir = 0
				Call WriteConsoleMsg(UserIndex, "/salir cancelado.", Protocol.FontTypeNames.FONTTYPE_WARNING)
			Else
				'Simply reset
				UserList(UserIndex).Counters.salir = IIf((UserList(UserIndex).flags.Privilegios And Declaraciones.PlayerType.User) And MapInfo_Renamed(UserList(UserIndex).Pos.Map).Pk, IntervaloCerrarConexion, 0)
			End If
		End If
	End Sub
	
	'CambiarNick: Cambia el Nick de un slot.
	'
	'UserIndex: Quien ejecutó la orden
	'UserIndexDestino: SLot del usuario destino, a quien cambiarle el nick
	'NuevoNick: Nuevo nick de UserIndexDestino
	Public Sub CambiarNick(ByVal UserIndex As Short, ByVal UserIndexDestino As Short, ByVal NuevoNick As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim ViejoNick As String
		Dim ViejoCharBackup As String
		
		If UserList(UserIndexDestino).flags.UserLogged = False Then Exit Sub
		ViejoNick = UserList(UserIndexDestino).name
		
		If FileExist(CharPath & ViejoNick & ".chr") Then
			'hace un backup del char
			ViejoCharBackup = CharPath & ViejoNick & ".chr.old-"
			Rename(CharPath & ViejoNick & ".chr", ViejoCharBackup)
		End If
	End Sub
	
	Sub SendUserStatsTxtOFF(ByVal sendIndex As Short, ByVal Nombre As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim TempSecs As Integer
		Dim tempStr As String
		If FileExist(CharPath & Nombre & ".chr") = False Then
			Call WriteConsoleMsg(sendIndex, "Pj Inexistente", Protocol.FontTypeNames.FONTTYPE_INFO)
		Else
			Call WriteConsoleMsg(sendIndex, "Estadísticas de: " & Nombre, Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Nivel: " & GetVar(CharPath & Nombre & ".chr", "stats", "elv") & "  EXP: " & GetVar(CharPath & Nombre & ".chr", "stats", "Exp") & "/" & GetVar(CharPath & Nombre & ".chr", "stats", "elu"), Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Energía: " & GetVar(CharPath & Nombre & ".chr", "stats", "minsta") & "/" & GetVar(CharPath & Nombre & ".chr", "stats", "maxSta"), Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Salud: " & GetVar(CharPath & Nombre & ".chr", "stats", "MinHP") & "/" & GetVar(CharPath & Nombre & ".chr", "Stats", "MaxHP") & "  Maná: " & GetVar(CharPath & Nombre & ".chr", "Stats", "MinMAN") & "/" & GetVar(CharPath & Nombre & ".chr", "Stats", "MaxMAN"), Protocol.FontTypeNames.FONTTYPE_INFO)
			
			Call WriteConsoleMsg(sendIndex, "Menor Golpe/Mayor Golpe: " & GetVar(CharPath & Nombre & ".chr", "stats", "MaxHIT"), Protocol.FontTypeNames.FONTTYPE_INFO)
			
			Call WriteConsoleMsg(sendIndex, "Oro: " & GetVar(CharPath & Nombre & ".chr", "stats", "GLD"), Protocol.FontTypeNames.FONTTYPE_INFO)
			
			TempSecs = CInt(GetVar(CharPath & Nombre & ".chr", "INIT", "UpTime"))
			tempStr = (TempSecs \ 86400) & " Días, " & ((TempSecs Mod 86400) \ 3600) & " Horas, " & ((TempSecs Mod 86400) Mod 3600) \ 60 & " Minutos, " & (((TempSecs Mod 86400) Mod 3600) Mod 60) & " Segundos."
			Call WriteConsoleMsg(sendIndex, "Tiempo Logeado: " & tempStr, Protocol.FontTypeNames.FONTTYPE_INFO)
			
		End If
	End Sub
	
	Sub SendUserOROTxtFromChar(ByVal sendIndex As Short, ByVal charName As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim CharFile As String
		
		On Error Resume Next
		CharFile = CharPath & charName & ".chr"
		
		If FileExist(CharFile) Then
			Call WriteConsoleMsg(sendIndex, charName, Protocol.FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(sendIndex, "Tiene " & GetVar(CharFile, "STATS", "BANCO") & " en el banco.", Protocol.FontTypeNames.FONTTYPE_INFO)
		Else
			Call WriteConsoleMsg(sendIndex, "Usuario inexistente: " & charName, Protocol.FontTypeNames.FONTTYPE_INFO)
		End If
	End Sub
	
	Sub VolverCriminal(ByVal UserIndex As Short)
		'**************************************************************
		'Author: Unknown
		'Last Modify Date: 21/02/2010
		'Nacho: Actualiza el tag al cliente
		'21/02/2010: ZaMa - Ahora deja de ser atacable si se hace criminal.
		'**************************************************************
		With UserList(UserIndex)
			If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = Declaraciones.eTrigger.ZONAPELEA Then Exit Sub
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero) Then
				.Reputacion.BurguesRep = 0
				.Reputacion.NobleRep = 0
				.Reputacion.PlebeRep = 0
				.Reputacion.BandidoRep = .Reputacion.BandidoRep + vlASALTO
				If .Reputacion.BandidoRep > MAXREP Then .Reputacion.BandidoRep = MAXREP
				If .Faccion.ArmadaReal = 1 Then Call ExpulsarFaccionReal(UserIndex)
				
				If .flags.AtacablePor > 0 Then .flags.AtacablePor = 0
				
			End If
		End With
		
		Call RefreshCharStatus(UserIndex)
	End Sub
	
	Sub VolverCiudadano(ByVal UserIndex As Short)
		'**************************************************************
		'Author: Unknown
		'Last Modify Date: 21/06/2006
		'Nacho: Actualiza el tag al cliente.
		'**************************************************************
		With UserList(UserIndex)
			If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = 6 Then Exit Sub
			
			.Reputacion.LadronesRep = 0
			.Reputacion.BandidoRep = 0
			.Reputacion.AsesinoRep = 0
			.Reputacion.PlebeRep = .Reputacion.PlebeRep + vlASALTO
			If .Reputacion.PlebeRep > MAXREP Then .Reputacion.PlebeRep = MAXREP
		End With
		
		Call RefreshCharStatus(UserIndex)
	End Sub
	
	''
	'Checks if a given body index is a boat or not.
	'
	'@param body    The body index to bechecked.
	'@return    True if the body is a boat, false otherwise.
	
	Public Function BodyIsBoat(ByVal body As Short) As Boolean
		'**************************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modify Date: 10/07/2008
		'Checks if a given body index is a boat
		'**************************************************************
		'TODO : This should be checked somehow else. This is nasty....
		If body = iFragataReal Or body = iFragataCaos Or body = iBarcaPk Or body = iGaleraPk Or body = iGaleonPk Or body = iBarcaCiuda Or body = iGaleraCiuda Or body = iGaleonCiuda Or body = iFragataFantasmal Then
			BodyIsBoat = True
		End If
	End Function
	
	Public Sub SetInvisible(ByVal UserIndex As Short, ByVal userCharIndex As Short, ByVal invisible As Boolean)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim sndNick As String
		
		With UserList(UserIndex)
			Call SendData(modSendData.SendTarget.ToUsersAndRmsAndCounselorsAreaButGMs, UserIndex, PrepareMessageSetInvisible(userCharIndex, invisible))
			
			sndNick = .name
			
			If invisible Then
				sndNick = sndNick & " " & TAG_USER_INVISIBLE
			Else
				If .GuildIndex > 0 Then
					sndNick = sndNick & " <" & modGuilds.GuildName(.GuildIndex) & ">"
				End If
			End If
			
			Call SendData(modSendData.SendTarget.ToGMsAreaButRmsOrCounselors, UserIndex, PrepareMessageCharacterChangeNick(userCharIndex, sndNick))
		End With
	End Sub
	
	Public Sub SetConsulatMode(ByVal UserIndex As Short)
		'***************************************************
		'Author: Torres Patricio (Pato)
		'Last Modification: 05/06/10
		'
		'***************************************************
		
		Dim sndNick As String
		
		With UserList(UserIndex)
			sndNick = .name
			
			If .flags.EnConsulta Then
				sndNick = sndNick & " " & TAG_CONSULT_MODE
			Else
				If .GuildIndex > 0 Then
					sndNick = sndNick & " <" & modGuilds.GuildName(.GuildIndex) & ">"
				End If
			End If
			
			Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageCharacterChangeNick(.Char_Renamed.CharIndex, sndNick))
		End With
	End Sub
	
	Public Function IsArena(ByVal UserIndex As Short) As Boolean
		'**************************************************************
		'Author: ZaMa
		'Last Modify Date: 10/11/2009
		'Returns true if the user is in an Arena
		'**************************************************************
		IsArena = (TriggerZonaPelea(UserIndex, UserIndex) = Declaraciones.eTrigger6.TRIGGER6_PERMITE)
	End Function
	
	Public Sub PerdioNpc(ByVal UserIndex As Short)
		'**************************************************************
		'Author: ZaMa
		'Last Modify Date: 18/01/2010 (ZaMa)
		'The user loses his owned npc
		'18/01/2010: ZaMa - Las mascotas dejan de atacar al npc que se perdió.
		'**************************************************************
		
		Dim PetIndex As Integer
		
		With UserList(UserIndex)
			If .flags.OwnedNpc > 0 Then
				Npclist(.flags.OwnedNpc).Owner = 0
				.flags.OwnedNpc = 0
				
				' Dejan de atacar las mascotas
				If .NroMascotas > 0 Then
					For PetIndex = 1 To MAXMASCOTAS
						If .MascotasType(PetIndex) > 0 Then Call FollowAmo(PetIndex)
					Next PetIndex
				End If
			End If
		End With
	End Sub
	
	Public Sub ApropioNpc(ByVal UserIndex As Short, ByVal NpcIndex As Short)
		'**************************************************************
		'Author: ZaMa
		'Last Modify Date: 18/01/2010 (zaMa)
		'The user owns a new npc
		'18/01/2010: ZaMa - El sistema no aplica a zonas seguras.
		'19/04/2010: ZaMa - Ahora los admins no se pueden apropiar de npcs.
		'**************************************************************
		
		With UserList(UserIndex)
			' Los admins no se pueden apropiar de npcs
			If EsGM(UserIndex) Then Exit Sub
			
			'No aplica a zonas seguras
			If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = Declaraciones.eTrigger.ZONASEGURA Then Exit Sub
			
			' No aplica a algunos mapas que permiten el robo de npcs
			If MapInfo_Renamed(.Pos.Map).RoboNpcsPermitido = 1 Then Exit Sub
			
			' Pierde el npc anterior
			If .flags.OwnedNpc > 0 Then Npclist(.flags.OwnedNpc).Owner = 0
			
			' Si tenia otro dueño, lo perdio aca
			Npclist(NpcIndex).Owner = UserIndex
			.flags.OwnedNpc = NpcIndex
		End With
		
		' Inicializo o actualizo el timer de pertenencia
		Call IntervaloPerdioNpc(UserIndex, True)
	End Sub
	
	Public Function GetDireccion(ByVal UserIndex As Short, ByVal OtherUserIndex As Short) As String
		'**************************************************************
		'Author: ZaMa
		'Last Modify Date: 17/11/2009
		'Devuelve la direccion hacia donde esta el usuario
		'**************************************************************
		Dim X As Short
		Dim Y As Short
		
		X = UserList(UserIndex).Pos.X - UserList(OtherUserIndex).Pos.X
		Y = UserList(UserIndex).Pos.Y - UserList(OtherUserIndex).Pos.Y
		
		If X = 0 And Y > 0 Then
			GetDireccion = "Sur"
		ElseIf X = 0 And Y < 0 Then 
			GetDireccion = "Norte"
		ElseIf X > 0 And Y = 0 Then 
			GetDireccion = "Este"
		ElseIf X < 0 And Y = 0 Then 
			GetDireccion = "Oeste"
		ElseIf X > 0 And Y < 0 Then 
			GetDireccion = "NorEste"
		ElseIf X < 0 And Y < 0 Then 
			GetDireccion = "NorOeste"
		ElseIf X > 0 And Y > 0 Then 
			GetDireccion = "SurEste"
		ElseIf X < 0 And Y > 0 Then 
			GetDireccion = "SurOeste"
		End If
		
	End Function
	
	Public Function SameFaccion(ByVal UserIndex As Short, ByVal OtherUserIndex As Short) As Boolean
		'**************************************************************
		'Author: ZaMa
		'Last Modify Date: 17/11/2009
		'Devuelve True si son de la misma faccion
		'**************************************************************
		SameFaccion = (esCaos(UserIndex) And esCaos(OtherUserIndex)) Or (esArmada(UserIndex) And esArmada(OtherUserIndex))
	End Function
	
	Public Function FarthestPet(ByVal UserIndex As Short) As Short
		'**************************************************************
		'Author: ZaMa
		'Last Modify Date: 18/11/2009
		'Devuelve el indice de la mascota mas lejana.
		'**************************************************************
		On Error GoTo Errhandler
		
		Dim PetIndex As Short
		Dim Distancia As Short
		Dim OtraDistancia As Short
		
		With UserList(UserIndex)
			If .NroMascotas = 0 Then Exit Function
			
			For PetIndex = 1 To MAXMASCOTAS
				' Solo pos invocar criaturas que exitan!
				If .MascotasIndex(PetIndex) > 0 Then
					' Solo aplica a mascota, nada de elementales..
					If Npclist(.MascotasIndex(PetIndex)).Contadores.TiempoExistencia = 0 Then
						If FarthestPet = 0 Then
							' Por si tiene 1 sola mascota
							FarthestPet = PetIndex
							Distancia = System.Math.Abs(.Pos.X - Npclist(.MascotasIndex(PetIndex)).Pos.X) + System.Math.Abs(.Pos.Y - Npclist(.MascotasIndex(PetIndex)).Pos.Y)
						Else
							' La distancia de la proxima mascota
							OtraDistancia = System.Math.Abs(.Pos.X - Npclist(.MascotasIndex(PetIndex)).Pos.X) + System.Math.Abs(.Pos.Y - Npclist(.MascotasIndex(PetIndex)).Pos.Y)
							' Esta mas lejos?
							If OtraDistancia > Distancia Then
								Distancia = OtraDistancia
								FarthestPet = PetIndex
							End If
						End If
					End If
				End If
			Next PetIndex
		End With
		
		Exit Function
		
Errhandler: 
		Call LogError("Error en FarthestPet")
	End Function
	
	''
	' Set the EluSkill value at the skill.
	'
	' @param UserIndex  Specifies reference to user
	' @param Skill      Number of the skill to check
	' @param Allocation True If the motive of the modification is the allocation, False if the skill increase by training
	
	Public Sub CheckEluSkill(ByVal UserIndex As Short, ByVal Skill As Byte, ByVal Allocation As Boolean)
		'*************************************************
		'Author: Torres Patricio (Pato)
		'Last modified: 11/20/2009
		'
		'*************************************************
		
		With UserList(UserIndex).Stats
			If .UserSkills(Skill) < MAXSKILLPOINTS Then
				If Allocation Then
					.ExpSkills(Skill) = 0
				Else
					.ExpSkills(Skill) = .ExpSkills(Skill) - .EluSkills(Skill)
				End If
				
				.EluSkills(Skill) = ELU_SKILL_INICIAL * 1.05 ^ .UserSkills(Skill)
			Else
				.ExpSkills(Skill) = 0
				.EluSkills(Skill) = 0
			End If
		End With
		
	End Sub
	
	Public Function HasEnoughItems(ByVal UserIndex As Short, ByVal ObjIndex As Short, ByVal Amount As Integer) As Boolean
		'**************************************************************
		'Author: ZaMa
		'Last Modify Date: 25/11/2009
		'Cheks Wether the user has the required amount of items in the inventory or not
		'**************************************************************
		
		Dim Slot As Integer
		Dim ItemInvAmount As Integer
		
		For Slot = 1 To UserList(UserIndex).CurrentInventorySlots
			' Si es el item que busco
			If UserList(UserIndex).Invent.Object_Renamed(Slot).ObjIndex = ObjIndex Then
				' Lo sumo a la cantidad total
				ItemInvAmount = ItemInvAmount + UserList(UserIndex).Invent.Object_Renamed(Slot).Amount
			End If
		Next Slot
		
		HasEnoughItems = Amount <= ItemInvAmount
	End Function
	
	Public Function TotalOfferItems(ByVal ObjIndex As Short, ByVal UserIndex As Short) As Integer
		'**************************************************************
		'Author: ZaMa
		'Last Modify Date: 25/11/2009
		'Cheks the amount of items the user has in offerSlots.
		'**************************************************************
		Dim Slot As Byte
		
		For Slot = 1 To MAX_OFFER_SLOTS
			' Si es el item que busco
			If UserList(UserIndex).ComUsu.Objeto(Slot) = ObjIndex Then
				' Lo sumo a la cantidad total
				TotalOfferItems = TotalOfferItems + UserList(UserIndex).ComUsu.cant(Slot)
			End If
		Next Slot
		
	End Function
	
	Public Function getMaxInventorySlots(ByVal UserIndex As Short) As Byte
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		If UserList(UserIndex).Invent.MochilaEqpObjIndex > 0 Then
			getMaxInventorySlots = MAX_NORMAL_INVENTORY_SLOTS + ObjData_Renamed(UserList(UserIndex).Invent.MochilaEqpObjIndex).MochilaType * 5 '5=slots por fila, hacer constante
		Else
			getMaxInventorySlots = MAX_NORMAL_INVENTORY_SLOTS
		End If
	End Function
	
	Public Sub goHome(ByVal UserIndex As Short)
		Dim Distance As Short
		Dim tiempo As Integer
		
		With UserList(UserIndex)
			If .flags.Muerto = 1 Then
				If .flags.lastMap = 0 Then
					Distance = distanceToCities(.Pos.Map).distanceToCity(.Hogar)
				Else
					Distance = distanceToCities(.flags.lastMap).distanceToCity(.Hogar) + GOHOME_PENALTY
				End If
				
				tiempo = (Distance + 1) * 30 'segundos
				
				.Counters.goHome = tiempo / 6 'Se va a chequear cada 6 segundos.
				
				.flags.Traveling = 1
				
				Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.Home, Distance, tiempo,  , MapInfo_Renamed(Ciudades(.Hogar).Map).name)
			Else
				Call WriteConsoleMsg(UserIndex, "Debes estar muerto para poder utilizar este comando.", Protocol.FontTypeNames.FONTTYPE_FIGHT)
			End If
		End With
	End Sub
	
	Public Function ToogleToAtackable(ByVal UserIndex As Short, ByVal OwnerIndex As Short, Optional ByVal StealingNpc As Boolean = True) As Boolean
		'***************************************************
		'Author: ZaMa
		'Last Modification: 15/01/2010
		'Change to Atackable mode.
		'***************************************************
		
		Dim AtacablePor As Short
		
		With UserList(UserIndex)
			
			AtacablePor = .flags.AtacablePor
			
			If AtacablePor > 0 Then
				' Intenta robar un npc
				If StealingNpc Then
					' Puede atacar el mismo npc que ya estaba robando, pero no una nuevo.
					If AtacablePor <> OwnerIndex Then
						Call WriteConsoleMsg(UserIndex, "No puedes atacar otra criatura con dueño hasta que haya terminado tu castigo.", Protocol.FontTypeNames.FONTTYPE_INFO)
						Exit Function
					End If
					' Esta atacando a alguien en estado atacable => Se renueva el timer de atacable
				Else
					' Renovar el timer
					Call IntervaloEstadoAtacable(UserIndex, True)
					ToogleToAtackable = True
					Exit Function
				End If
			End If
			
			.flags.AtacablePor = OwnerIndex
			
			' Actualizar clientes
			Call RefreshCharStatus(UserIndex)
			
			' Inicializar el timer
			Call IntervaloEstadoAtacable(UserIndex, True)
			
			ToogleToAtackable = True
			
		End With
		
	End Function
	
	Public Sub setHome(ByVal UserIndex As Short, ByVal newHome As Declaraciones.eCiudad, ByVal NpcIndex As Short)
		'***************************************************
		'Author: Budi
		'Last Modification: 30/04/2010
		'30/04/2010: ZaMa - Ahora el npc avisa que se cambio de hogar.
		'***************************************************
		If newHome < Declaraciones.eCiudad.cUllathorpe Or newHome > Declaraciones.eCiudad.cArghal Then Exit Sub
		UserList(UserIndex).Hogar = newHome
		
		Call WriteChatOverHead(UserIndex, "¡¡¡Bienvenido a nuestra humilde comunidad, este es ahora tu nuevo hogar!!!", Npclist(NpcIndex).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
	End Sub
End Module
