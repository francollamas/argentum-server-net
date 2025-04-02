Option Strict Off
Option Explicit On
Module modSendData
	'**************************************************************
	' SendData.bas - Has all methods to send data to different user groups.
	' Makes use of the modAreas module.
	'
	' Implemented by Juan Martín Sotuyo Dodero (Maraxus) (juansotuyo@gmail.com)
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
	
	''
	' Contains all methods to send data to different user groups.
	' Makes use of the modAreas module.
	'
	' @author Juan Martín Sotuyo Dodero (Maraxus) juansotuyo@gmail.com
	' @version 1.0.0
	' @date 20070107
	
	
	Public Enum SendTarget
		ToAll = 1
		toMap
		ToPCArea
		ToAllButIndex
		ToMapButIndex
		ToGM
		ToNPCArea
		ToGuildMembers
		ToAdmins
		ToPCAreaButIndex
		ToAdminsAreaButConsejeros
		ToDiosesYclan
		ToConsejo
		ToClanArea
		ToConsejoCaos
		ToRolesMasters
		ToDeadArea
		ToCiudadanos
		ToCriminales
		ToPartyArea
		ToReal
		ToCaos
		ToCiudadanosYRMs
		ToCriminalesYRMs
		ToRealYRMs
		ToCaosYRMs
		ToHigherAdmins
		ToGMsAreaButRmsOrCounselors
		ToUsersAreaButGMs
		ToUsersAndRmsAndCounselorsAreaButGMs
	End Enum
	
	Public Sub SendData(ByVal sndRoute As SendTarget, ByVal sndIndex As Short, ByVal sndData As String)
		'**************************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus) - Rewrite of original
		'Last Modify Date: 01/08/2007
		'Last modified by: (liquid)
		'**************************************************************
		On Error Resume Next
		Dim LoopC As Integer
		Dim Map As Short
		
		Select Case sndRoute
			Case SendTarget.ToPCArea
				Call SendToUserArea(sndIndex, sndData)
				Exit Sub
				
			Case SendTarget.ToAdmins
				For LoopC = 1 To LastUser
					If UserList(LoopC).ConnID <> -1 Then
						If UserList(LoopC).flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.Consejero) Then
							Call EnviarDatosASlot(LoopC, sndData)
						End If
					End If
				Next LoopC
				Exit Sub
				
			Case SendTarget.ToAll
				For LoopC = 1 To LastUser
					If UserList(LoopC).ConnID <> -1 Then
						If UserList(LoopC).flags.UserLogged Then 'Esta logeado como usuario?
							Call EnviarDatosASlot(LoopC, sndData)
						End If
					End If
				Next LoopC
				Exit Sub
				
			Case SendTarget.ToAllButIndex
				For LoopC = 1 To LastUser
					If (UserList(LoopC).ConnID <> -1) And (LoopC <> sndIndex) Then
						If UserList(LoopC).flags.UserLogged Then 'Esta logeado como usuario?
							Call EnviarDatosASlot(LoopC, sndData)
						End If
					End If
				Next LoopC
				Exit Sub
				
			Case SendTarget.toMap
				Call SendToMap(sndIndex, sndData)
				Exit Sub
				
			Case SendTarget.ToMapButIndex
				Call SendToMapButIndex(sndIndex, sndData)
				Exit Sub
				
			Case SendTarget.ToGuildMembers
				LoopC = modGuilds.m_Iterador_ProximoUserIndex(sndIndex)
				While LoopC > 0
					If (UserList(LoopC).ConnID <> -1) Then
						Call EnviarDatosASlot(LoopC, sndData)
					End If
					LoopC = modGuilds.m_Iterador_ProximoUserIndex(sndIndex)
				End While
				Exit Sub
				
			Case SendTarget.ToDeadArea
				Call SendToDeadUserArea(sndIndex, sndData)
				Exit Sub
				
			Case SendTarget.ToPCAreaButIndex
				Call SendToUserAreaButindex(sndIndex, sndData)
				Exit Sub
				
			Case SendTarget.ToClanArea
				Call SendToUserGuildArea(sndIndex, sndData)
				Exit Sub
				
			Case SendTarget.ToPartyArea
				Call SendToUserPartyArea(sndIndex, sndData)
				Exit Sub
				
			Case SendTarget.ToAdminsAreaButConsejeros
				Call SendToAdminsButConsejerosArea(sndIndex, sndData)
				Exit Sub
				
			Case SendTarget.ToNPCArea
				Call SendToNpcArea(sndIndex, sndData)
				Exit Sub
				
			Case SendTarget.ToDiosesYclan
				LoopC = modGuilds.m_Iterador_ProximoUserIndex(sndIndex)
				While LoopC > 0
					If (UserList(LoopC).ConnID <> -1) Then
						Call EnviarDatosASlot(LoopC, sndData)
					End If
					LoopC = modGuilds.m_Iterador_ProximoUserIndex(sndIndex)
				End While
				
				LoopC = modGuilds.Iterador_ProximoGM(sndIndex)
				While LoopC > 0
					If (UserList(LoopC).ConnID <> -1) Then
						Call EnviarDatosASlot(LoopC, sndData)
					End If
					LoopC = modGuilds.Iterador_ProximoGM(sndIndex)
				End While
				
				Exit Sub
				
			Case SendTarget.ToConsejo
				For LoopC = 1 To LastUser
					If (UserList(LoopC).ConnID <> -1) Then
						If UserList(LoopC).flags.Privilegios And Declaraciones.PlayerType.RoyalCouncil Then
							Call EnviarDatosASlot(LoopC, sndData)
						End If
					End If
				Next LoopC
				Exit Sub
				
			Case SendTarget.ToConsejoCaos
				For LoopC = 1 To LastUser
					If (UserList(LoopC).ConnID <> -1) Then
						If UserList(LoopC).flags.Privilegios And Declaraciones.PlayerType.ChaosCouncil Then
							Call EnviarDatosASlot(LoopC, sndData)
						End If
					End If
				Next LoopC
				Exit Sub
				
			Case SendTarget.ToRolesMasters
				For LoopC = 1 To LastUser
					If (UserList(LoopC).ConnID <> -1) Then
						If UserList(LoopC).flags.Privilegios And Declaraciones.PlayerType.RoleMaster Then
							Call EnviarDatosASlot(LoopC, sndData)
						End If
					End If
				Next LoopC
				Exit Sub
				
			Case SendTarget.ToCiudadanos
				For LoopC = 1 To LastUser
					If (UserList(LoopC).ConnID <> -1) Then
						If Not criminal(LoopC) Then
							Call EnviarDatosASlot(LoopC, sndData)
						End If
					End If
				Next LoopC
				Exit Sub
				
			Case SendTarget.ToCriminales
				For LoopC = 1 To LastUser
					If (UserList(LoopC).ConnID <> -1) Then
						If criminal(LoopC) Then
							Call EnviarDatosASlot(LoopC, sndData)
						End If
					End If
				Next LoopC
				Exit Sub
				
			Case SendTarget.ToReal
				For LoopC = 1 To LastUser
					If (UserList(LoopC).ConnID <> -1) Then
						If UserList(LoopC).Faccion.ArmadaReal = 1 Then
							Call EnviarDatosASlot(LoopC, sndData)
						End If
					End If
				Next LoopC
				Exit Sub
				
			Case SendTarget.ToCaos
				For LoopC = 1 To LastUser
					If (UserList(LoopC).ConnID <> -1) Then
						If UserList(LoopC).Faccion.FuerzasCaos = 1 Then
							Call EnviarDatosASlot(LoopC, sndData)
						End If
					End If
				Next LoopC
				Exit Sub
				
			Case SendTarget.ToCiudadanosYRMs
				For LoopC = 1 To LastUser
					If (UserList(LoopC).ConnID <> -1) Then
						If Not criminal(LoopC) Or (UserList(LoopC).flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 Then
							Call EnviarDatosASlot(LoopC, sndData)
						End If
					End If
				Next LoopC
				Exit Sub
				
			Case SendTarget.ToCriminalesYRMs
				For LoopC = 1 To LastUser
					If (UserList(LoopC).ConnID <> -1) Then
						If criminal(LoopC) Or (UserList(LoopC).flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 Then
							Call EnviarDatosASlot(LoopC, sndData)
						End If
					End If
				Next LoopC
				Exit Sub
				
			Case SendTarget.ToRealYRMs
				For LoopC = 1 To LastUser
					If (UserList(LoopC).ConnID <> -1) Then
						If UserList(LoopC).Faccion.ArmadaReal = 1 Or (UserList(LoopC).flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 Then
							Call EnviarDatosASlot(LoopC, sndData)
						End If
					End If
				Next LoopC
				Exit Sub
				
			Case SendTarget.ToCaosYRMs
				For LoopC = 1 To LastUser
					If (UserList(LoopC).ConnID <> -1) Then
						If UserList(LoopC).Faccion.FuerzasCaos = 1 Or (UserList(LoopC).flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 Then
							Call EnviarDatosASlot(LoopC, sndData)
						End If
					End If
				Next LoopC
				Exit Sub
				
			Case SendTarget.ToHigherAdmins
				For LoopC = 1 To LastUser
					If UserList(LoopC).ConnID <> -1 Then
						If UserList(LoopC).flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios) Then
							Call EnviarDatosASlot(LoopC, sndData)
						End If
					End If
				Next LoopC
				Exit Sub
				
			Case SendTarget.ToGMsAreaButRmsOrCounselors
				Call SendToGMsAreaButRmsOrCounselors(sndIndex, sndData)
				Exit Sub
				
			Case SendTarget.ToUsersAreaButGMs
				Call SendToUsersAreaButGMs(sndIndex, sndData)
				Exit Sub
			Case SendTarget.ToUsersAndRmsAndCounselorsAreaButGMs
				Call SendToUsersAndRmsAndCounselorsAreaButGMs(sndIndex, sndData)
				Exit Sub
		End Select
	End Sub
	
	Private Sub SendToUserArea(ByVal UserIndex As Short, ByVal sdData As String)
		'**************************************************************
		'Author: Lucio N. Tourrilhes (DuNga)
		'Last Modify Date: Unknow
		'
		'**************************************************************
		Dim LoopC As Integer
		Dim tempIndex As Short
		
		Dim Map As Short
		Dim AreaX As Short
		Dim AreaY As Short
		
		Map = UserList(UserIndex).Pos.Map
		AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
		AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY
		
		If Not MapaValido(Map) Then Exit Sub
		
		For LoopC = 1 To ConnGroups(Map).CountEntrys
			tempIndex = ConnGroups(Map).UserEntrys(LoopC)
			
			If UserList(tempIndex).AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
				If UserList(tempIndex).AreasInfo.AreaReciveY And AreaY Then
					If UserList(tempIndex).ConnIDValida Then
						Call EnviarDatosASlot(tempIndex, sdData)
					End If
				End If
			End If
		Next LoopC
	End Sub
	
	Private Sub SendToUserAreaButindex(ByVal UserIndex As Short, ByVal sdData As String)
		'**************************************************************
		'Author: Lucio N. Tourrilhes (DuNga)
		'Last Modify Date: Unknow
		'
		'**************************************************************
		Dim LoopC As Integer
		Dim TempInt As Short
		Dim tempIndex As Short
		
		Dim Map As Short
		Dim AreaX As Short
		Dim AreaY As Short
		
		Map = UserList(UserIndex).Pos.Map
		AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
		AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY
		
		If Not MapaValido(Map) Then Exit Sub
		
		For LoopC = 1 To ConnGroups(Map).CountEntrys
			tempIndex = ConnGroups(Map).UserEntrys(LoopC)
			
			TempInt = UserList(tempIndex).AreasInfo.AreaReciveX And AreaX
			If TempInt Then 'Esta en el area?
				TempInt = UserList(tempIndex).AreasInfo.AreaReciveY And AreaY
				If TempInt Then
					If tempIndex <> UserIndex Then
						If UserList(tempIndex).ConnIDValida Then
							Call EnviarDatosASlot(tempIndex, sdData)
						End If
					End If
				End If
			End If
		Next LoopC
	End Sub
	
	Private Sub SendToDeadUserArea(ByVal UserIndex As Short, ByVal sdData As String)
		'**************************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modify Date: Unknow
		'
		'**************************************************************
		Dim LoopC As Integer
		Dim tempIndex As Short
		
		Dim Map As Short
		Dim AreaX As Short
		Dim AreaY As Short
		
		Map = UserList(UserIndex).Pos.Map
		AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
		AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY
		
		If Not MapaValido(Map) Then Exit Sub
		
		For LoopC = 1 To ConnGroups(Map).CountEntrys
			tempIndex = ConnGroups(Map).UserEntrys(LoopC)
			
			If UserList(tempIndex).AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
				If UserList(tempIndex).AreasInfo.AreaReciveY And AreaY Then
					'Dead and admins read
					If UserList(tempIndex).ConnIDValida = True And (UserList(tempIndex).flags.Muerto = 1 Or (UserList(tempIndex).flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.Consejero)) <> 0) Then
						Call EnviarDatosASlot(tempIndex, sdData)
					End If
				End If
			End If
		Next LoopC
	End Sub
	
	Private Sub SendToUserGuildArea(ByVal UserIndex As Short, ByVal sdData As String)
		'**************************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modify Date: Unknow
		'
		'**************************************************************
		Dim LoopC As Integer
		Dim tempIndex As Short
		
		Dim Map As Short
		Dim AreaX As Short
		Dim AreaY As Short
		
		Map = UserList(UserIndex).Pos.Map
		AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
		AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY
		
		If Not MapaValido(Map) Then Exit Sub
		
		If UserList(UserIndex).GuildIndex = 0 Then Exit Sub
		
		For LoopC = 1 To ConnGroups(Map).CountEntrys
			tempIndex = ConnGroups(Map).UserEntrys(LoopC)
			
			If UserList(tempIndex).AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
				If UserList(tempIndex).AreasInfo.AreaReciveY And AreaY Then
					If UserList(tempIndex).ConnIDValida And (UserList(tempIndex).GuildIndex = UserList(UserIndex).GuildIndex Or ((UserList(tempIndex).flags.Privilegios And Declaraciones.PlayerType.Dios) And (UserList(tempIndex).flags.Privilegios And Declaraciones.PlayerType.RoleMaster) = 0)) Then
						Call EnviarDatosASlot(tempIndex, sdData)
					End If
				End If
			End If
		Next LoopC
	End Sub
	
	Private Sub SendToUserPartyArea(ByVal UserIndex As Short, ByVal sdData As String)
		'**************************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modify Date: Unknow
		'
		'**************************************************************
		Dim LoopC As Integer
		Dim tempIndex As Short
		
		Dim Map As Short
		Dim AreaX As Short
		Dim AreaY As Short
		
		Map = UserList(UserIndex).Pos.Map
		AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
		AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY
		
		If Not MapaValido(Map) Then Exit Sub
		
		If UserList(UserIndex).PartyIndex = 0 Then Exit Sub
		
		For LoopC = 1 To ConnGroups(Map).CountEntrys
			tempIndex = ConnGroups(Map).UserEntrys(LoopC)
			
			If UserList(tempIndex).AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
				If UserList(tempIndex).AreasInfo.AreaReciveY And AreaY Then
					If UserList(tempIndex).ConnIDValida And UserList(tempIndex).PartyIndex = UserList(UserIndex).PartyIndex Then
						Call EnviarDatosASlot(tempIndex, sdData)
					End If
				End If
			End If
		Next LoopC
	End Sub
	
	Private Sub SendToAdminsButConsejerosArea(ByVal UserIndex As Short, ByVal sdData As String)
		'**************************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modify Date: Unknow
		'
		'**************************************************************
		Dim LoopC As Integer
		Dim tempIndex As Short
		
		Dim Map As Short
		Dim AreaX As Short
		Dim AreaY As Short
		
		Map = UserList(UserIndex).Pos.Map
		AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
		AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY
		
		If Not MapaValido(Map) Then Exit Sub
		
		For LoopC = 1 To ConnGroups(Map).CountEntrys
			tempIndex = ConnGroups(Map).UserEntrys(LoopC)
			
			If UserList(tempIndex).AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
				If UserList(tempIndex).AreasInfo.AreaReciveY And AreaY Then
					If UserList(tempIndex).ConnIDValida Then
						If UserList(tempIndex).flags.Privilegios And (Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin) Then Call EnviarDatosASlot(tempIndex, sdData)
					End If
				End If
			End If
		Next LoopC
	End Sub
	
	Private Sub SendToNpcArea(ByVal NpcIndex As Integer, ByVal sdData As String)
		'**************************************************************
		'Author: Lucio N. Tourrilhes (DuNga)
		'Last Modify Date: Unknow
		'
		'**************************************************************
		Dim LoopC As Integer
		Dim TempInt As Short
		Dim tempIndex As Short
		
		Dim Map As Short
		Dim AreaX As Short
		Dim AreaY As Short
		
		Map = Npclist(NpcIndex).Pos.Map
		AreaX = Npclist(NpcIndex).AreasInfo.AreaPerteneceX
		AreaY = Npclist(NpcIndex).AreasInfo.AreaPerteneceY
		
		If Not MapaValido(Map) Then Exit Sub
		
		For LoopC = 1 To ConnGroups(Map).CountEntrys
			tempIndex = ConnGroups(Map).UserEntrys(LoopC)
			
			TempInt = UserList(tempIndex).AreasInfo.AreaReciveX And AreaX
			If TempInt Then 'Esta en el area?
				TempInt = UserList(tempIndex).AreasInfo.AreaReciveY And AreaY
				If TempInt Then
					If UserList(tempIndex).ConnIDValida Then
						Call EnviarDatosASlot(tempIndex, sdData)
					End If
				End If
			End If
		Next LoopC
	End Sub
	
	Public Sub SendToAreaByPos(ByVal Map As Short, ByVal AreaX As Short, ByVal AreaY As Short, ByVal sdData As String)
		'**************************************************************
		'Author: Lucio N. Tourrilhes (DuNga)
		'Last Modify Date: Unknow
		'
		'**************************************************************
		Dim LoopC As Integer
		Dim TempInt As Short
		Dim tempIndex As Short
		
		AreaX = 2 ^ (AreaX \ 9)
		AreaY = 2 ^ (AreaY \ 9)
		
		If Not MapaValido(Map) Then Exit Sub
		
		For LoopC = 1 To ConnGroups(Map).CountEntrys
			tempIndex = ConnGroups(Map).UserEntrys(LoopC)
			
			TempInt = UserList(tempIndex).AreasInfo.AreaReciveX And AreaX
			If TempInt Then 'Esta en el area?
				TempInt = UserList(tempIndex).AreasInfo.AreaReciveY And AreaY
				If TempInt Then
					If UserList(tempIndex).ConnIDValida Then
						Call EnviarDatosASlot(tempIndex, sdData)
					End If
				End If
			End If
		Next LoopC
	End Sub
	
	Public Sub SendToMap(ByVal Map As Short, ByVal sdData As String)
		'**************************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modify Date: 5/24/2007
		'
		'**************************************************************
		Dim LoopC As Integer
		Dim tempIndex As Short
		
		If Not MapaValido(Map) Then Exit Sub
		
		For LoopC = 1 To ConnGroups(Map).CountEntrys
			tempIndex = ConnGroups(Map).UserEntrys(LoopC)
			
			If UserList(tempIndex).ConnIDValida Then
				Call EnviarDatosASlot(tempIndex, sdData)
			End If
		Next LoopC
	End Sub
	
	Public Sub SendToMapButIndex(ByVal UserIndex As Short, ByVal sdData As String)
		'**************************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modify Date: 5/24/2007
		'
		'**************************************************************
		Dim LoopC As Integer
		Dim Map As Short
		Dim tempIndex As Short
		
		Map = UserList(UserIndex).Pos.Map
		
		If Not MapaValido(Map) Then Exit Sub
		
		For LoopC = 1 To ConnGroups(Map).CountEntrys
			tempIndex = ConnGroups(Map).UserEntrys(LoopC)
			
			If tempIndex <> UserIndex And UserList(tempIndex).ConnIDValida Then
				Call EnviarDatosASlot(tempIndex, sdData)
			End If
		Next LoopC
	End Sub
	
	Private Sub SendToGMsAreaButRmsOrCounselors(ByVal UserIndex As Short, ByVal sdData As String)
		'**************************************************************
		'Author: Torres Patricio(Pato)
		'Last Modify Date: 12/02/2010
		'12/02/2010: ZaMa - Restrinjo solo a dioses, admins y gms.
		'15/02/2010: ZaMa - Cambio el nombre de la funcion (viejo: ToGmsArea, nuevo: ToGmsAreaButRMsOrCounselors)
		'**************************************************************
		Dim LoopC As Integer
		Dim tempIndex As Short
		
		Dim Map As Short
		Dim AreaX As Short
		Dim AreaY As Short
		
		Map = UserList(UserIndex).Pos.Map
		AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
		AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY
		
		If Not MapaValido(Map) Then Exit Sub
		
		For LoopC = 1 To ConnGroups(Map).CountEntrys
			tempIndex = ConnGroups(Map).UserEntrys(LoopC)
			
			With UserList(tempIndex)
				If .AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
					If .AreasInfo.AreaReciveY And AreaY Then
						If .ConnIDValida Then
							' Exclusivo para dioses, admins y gms
							If (.flags.Privilegios And Not Declaraciones.PlayerType.User And Not Declaraciones.PlayerType.Consejero And Not Declaraciones.PlayerType.RoleMaster) = .flags.Privilegios Then
								Call EnviarDatosASlot(tempIndex, sdData)
							End If
						End If
					End If
				End If
			End With
		Next LoopC
	End Sub
	
	Private Sub SendToUsersAreaButGMs(ByVal UserIndex As Short, ByVal sdData As String)
		'**************************************************************
		'Author: Torres Patricio(Pato)
		'Last Modify Date: 10/17/2009
		'
		'**************************************************************
		Dim LoopC As Integer
		Dim tempIndex As Short
		
		Dim Map As Short
		Dim AreaX As Short
		Dim AreaY As Short
		
		Map = UserList(UserIndex).Pos.Map
		AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
		AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY
		
		If Not MapaValido(Map) Then Exit Sub
		
		For LoopC = 1 To ConnGroups(Map).CountEntrys
			tempIndex = ConnGroups(Map).UserEntrys(LoopC)
			
			If UserList(tempIndex).AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
				If UserList(tempIndex).AreasInfo.AreaReciveY And AreaY Then
					If UserList(tempIndex).ConnIDValida Then
						If UserList(tempIndex).flags.Privilegios And Declaraciones.PlayerType.User Then
							Call EnviarDatosASlot(tempIndex, sdData)
						End If
					End If
				End If
			End If
		Next LoopC
	End Sub
	
	Private Sub SendToUsersAndRmsAndCounselorsAreaButGMs(ByVal UserIndex As Short, ByVal sdData As String)
		'**************************************************************
		'Author: Torres Patricio(Pato)
		'Last Modify Date: 10/17/2009
		'
		'**************************************************************
		Dim LoopC As Integer
		Dim tempIndex As Short
		
		Dim Map As Short
		Dim AreaX As Short
		Dim AreaY As Short
		
		Map = UserList(UserIndex).Pos.Map
		AreaX = UserList(UserIndex).AreasInfo.AreaPerteneceX
		AreaY = UserList(UserIndex).AreasInfo.AreaPerteneceY
		
		If Not MapaValido(Map) Then Exit Sub
		
		For LoopC = 1 To ConnGroups(Map).CountEntrys
			tempIndex = ConnGroups(Map).UserEntrys(LoopC)
			
			If UserList(tempIndex).AreasInfo.AreaReciveX And AreaX Then 'Esta en el area?
				If UserList(tempIndex).AreasInfo.AreaReciveY And AreaY Then
					If UserList(tempIndex).ConnIDValida Then
						If UserList(tempIndex).flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.RoleMaster) Then
							Call EnviarDatosASlot(tempIndex, sdData)
						End If
					End If
				End If
			End If
		Next LoopC
	End Sub
	
	Public Sub AlertarFaccionarios(ByVal UserIndex As Short)
		'**************************************************************
		'Author: ZaMa
		'Last Modify Date: 17/11/2009
		'Alerta a los faccionarios, dandoles una orientacion
		'**************************************************************
		Dim LoopC As Integer
		Dim tempIndex As Short
		Dim Map As Short
		Dim Font As Protocol.FontTypeNames
		
		If esCaos(UserIndex) Then
			Font = Protocol.FontTypeNames.FONTTYPE_CONSEJOCAOS
		Else
			Font = Protocol.FontTypeNames.FONTTYPE_CONSEJO
		End If
		
		Map = UserList(UserIndex).Pos.Map
		
		If Not MapaValido(Map) Then Exit Sub
		
		For LoopC = 1 To ConnGroups(Map).CountEntrys
			tempIndex = ConnGroups(Map).UserEntrys(LoopC)
			
			If UserList(tempIndex).ConnIDValida Then
				If tempIndex <> UserIndex Then
					' Solo se envia a los de la misma faccion
					If SameFaccion(UserIndex, tempIndex) Then
						Call EnviarDatosASlot(tempIndex, PrepareMessageConsoleMsg("Escuchas el llamado de un compañero que proviene del " & GetDireccion(UserIndex, tempIndex), Font))
					End If
				End If
			End If
		Next LoopC
		
	End Sub
End Module
