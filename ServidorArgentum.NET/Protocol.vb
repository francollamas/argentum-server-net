Option Strict Off
Option Explicit On
Module Protocol
	'**************************************************************
	' Protocol.bas - Handles all incoming / outgoing messages for client-server communications.
	' Uses a binary protocol designed by myself.
	'
	' Designed and implemented by Juan Martín Sotuyo Dodero (Maraxus)
	' (juansotuyo@gmail.com)
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
	'Handles all incoming / outgoing packets for client - server communications
	'The binary prtocol here used was designed by Juan Martín Sotuyo Dodero.
	'This is the first time it's used in Alkon, though the second time it's coded.
	'This implementation has several enhacements from the first design.
	'
	' @author Juan Martín Sotuyo Dodero (Maraxus) juansotuyo@gmail.com
	' @version 1.0.0
	' @date 20060517


	''
	'When we have a list of strings, we use this to separate them and prevent
	'having too many string lengths in the queue. Yes, each string is NULL-terminated :P
	'UPGRADE_NOTE: SEPARATOR ha cambiado de Constant a Variable. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C54B49D7-5804-4D48-834B-B3D81E4C2F13"'
	Private SEPARATOR As Char = Chr(0)

	''
	'Auxiliar ByteQueue used as buffer to generate messages not intended to be sent right away.
	'Specially usefull to create a message once and send it over to several clients.
	Private auxiliarBuffer As New clsByteQueue
	
	
	Private Enum ServerPacketID
		Logged ' LOGGED
		RemoveDialogs ' QTDL
		RemoveCharDialog ' QDL
		NavigateToggle ' NAVEG
		Disconnect ' FINOK
		CommerceEnd ' FINCOMOK
		BankEnd ' FINBANOK
		CommerceInit ' INITCOM
		BankInit ' INITBANCO
		UserCommerceInit ' INITCOMUSU
		UserCommerceEnd ' FINCOMUSUOK
		UserOfferConfirm
		CommerceChat
		ShowBlacksmithForm ' SFH
		ShowCarpenterForm ' SFC
		UpdateSta ' ASS
		UpdateMana ' ASM
		UpdateHP ' ASH
		UpdateGold ' ASG
		UpdateBankGold
		UpdateExp ' ASE
		ChangeMap ' CM
		PosUpdate ' PU
		ChatOverHead ' ||
		ConsoleMsg ' || - Beware!! its the same as above, but it was properly splitted
		GuildChat ' |+
		ShowMessageBox ' !!
		UserIndexInServer ' IU
		UserCharIndexInServer ' IP
		CharacterCreate ' CC
		CharacterRemove ' BP
		CharacterChangeNick
		CharacterMove ' MP, +, * and _ '
		ForceCharMove
		CharacterChange ' CP
		ObjectCreate ' HO
		ObjectDelete ' BO
		BlockPosition ' BQ
		PlayMidi ' TM
		PlayWave ' TW
		guildList ' GL
		AreaChanged ' CA
		PauseToggle ' BKW
		RainToggle ' LLU
		CreateFX ' CFX
		UpdateUserStats ' EST
		WorkRequestTarget ' T01
		ChangeInventorySlot ' CSI
		ChangeBankSlot ' SBO
		ChangeSpellSlot ' SHS
		Atributes ' ATR
		BlacksmithWeapons ' LAH
		BlacksmithArmors ' LAR
		CarpenterObjects ' OBR
		RestOK ' DOK
		ErrorMsg ' ERR
		Blind ' CEGU
		Dumb ' DUMB
		ShowSignal ' MCAR
		ChangeNPCInventorySlot ' NPCI
		UpdateHungerAndThirst ' EHYS
		Fame ' FAMA
		MiniStats ' MEST
		LevelUp ' SUNI
		AddForumMsg ' FMSG
		ShowForumForm ' MFOR
		SetInvisible ' NOVER
		DiceRoll ' DADOS
		MeditateToggle ' MEDOK
		BlindNoMore ' NSEGUE
		DumbNoMore ' NESTUP
		SendSkills ' SKILLS
		TrainerCreatureList ' LSTCRI
		guildNews ' GUILDNE
		OfferDetails ' PEACEDE & ALLIEDE
		AlianceProposalsList ' ALLIEPR
		PeaceProposalsList ' PEACEPR
		CharacterInfo ' CHRINFO
		GuildLeaderInfo ' LEADERI
		GuildMemberInfo
		GuildDetails ' CLANDET
		ShowGuildFundationForm ' SHOWFUN
		ParalizeOK ' PARADOK
		ShowUserRequest ' PETICIO
		TradeOK ' TRANSOK
		BankOK ' BANCOOK
		ChangeUserTradeSlot ' COMUSUINV
		SendNight ' NOC
		Pong
		UpdateTagAndStatus
		'GM messages
		SpawnList ' SPL
		ShowSOSForm ' MSOS
		ShowMOTDEditionForm ' ZMOTD
		ShowGMPanelForm ' ABPANEL
		UserNameList ' LISTUSU
		ShowGuildAlign
		ShowPartyForm
		UpdateStrenghtAndDexterity
		UpdateStrenght
		UpdateDexterity
		AddSlots
		MultiMessage
		StopWorking
		CancelOfferItem
	End Enum
	
	Private Enum ClientPacketID
		LoginExistingChar 'OLOGIN
		ThrowDices 'TIRDAD
		LoginNewChar 'NLOGIN
		Talk ';
		Yell '-
		Whisper '\
		Walk 'M
		RequestPositionUpdate 'RPU
		Attack 'AT
		PickUp 'AG
		SafeToggle '/SEG & SEG  (SEG's behaviour has to be coded in the client)
		ResuscitationSafeToggle
		RequestGuildLeaderInfo 'GLINFO
		RequestAtributes 'ATR
		RequestFame 'FAMA
		RequestSkills 'ESKI
		RequestMiniStats 'FEST
		CommerceEnd 'FINCOM
		UserCommerceEnd 'FINCOMUSU
		UserCommerceConfirm
		CommerceChat
		BankEnd 'FINBAN
		UserCommerceOk 'COMUSUOK
		UserCommerceReject 'COMUSUNO
		Drop 'TI
		CastSpell 'LH
		LeftClick 'LC
		DoubleClick 'RC
		Work 'UK
		UseSpellMacro 'UMH
		UseItem 'USA
		CraftBlacksmith 'CNS
		CraftCarpenter 'CNC
		WorkLeftClick 'WLC
		CreateNewGuild 'CIG
		SpellInfo 'INFS
		EquipItem 'EQUI
		ChangeHeading 'CHEA
		ModifySkills 'SKSE
		Train 'ENTR
		CommerceBuy 'COMP
		BankExtractItem 'RETI
		CommerceSell 'VEND
		BankDeposit 'DEPO
		ForumPost 'DEMSG
		MoveSpell 'DESPHE
		MoveBank
		ClanCodexUpdate 'DESCOD
		UserCommerceOffer 'OFRECER
		GuildAcceptPeace 'ACEPPEAT
		GuildRejectAlliance 'RECPALIA
		GuildRejectPeace 'RECPPEAT
		GuildAcceptAlliance 'ACEPALIA
		GuildOfferPeace 'PEACEOFF
		GuildOfferAlliance 'ALLIEOFF
		GuildAllianceDetails 'ALLIEDET
		GuildPeaceDetails 'PEACEDET
		GuildRequestJoinerInfo 'ENVCOMEN
		GuildAlliancePropList 'ENVALPRO
		GuildPeacePropList 'ENVPROPP
		GuildDeclareWar 'DECGUERR
		GuildNewWebsite 'NEWWEBSI
		GuildAcceptNewMember 'ACEPTARI
		GuildRejectNewMember 'RECHAZAR
		GuildKickMember 'ECHARCLA
		GuildUpdateNews 'ACTGNEWS
		GuildMemberInfo '1HRINFO<
		GuildOpenElections 'ABREELEC
		GuildRequestMembership 'SOLICITUD
		GuildRequestDetails 'CLANDETAILS
		Online '/ONLINE
		Quit '/SALIR
		GuildLeave '/SALIRCLAN
		RequestAccountState '/BALANCE
		PetStand '/QUIETO
		PetFollow '/ACOMPAÑAR
		ReleasePet '/LIBERAR
		TrainList '/ENTRENAR
		Rest '/DESCANSAR
		Meditate '/MEDITAR
		Resucitate '/RESUCITAR
		Heal '/CURAR
		Help '/AYUDA
		RequestStats '/EST
		CommerceStart '/COMERCIAR
		BankStart '/BOVEDA
		Enlist '/ENLISTAR
		Information '/INFORMACION
		Reward '/RECOMPENSA
		RequestMOTD '/MOTD
		UpTime '/UPTIME
		PartyLeave '/SALIRPARTY
		PartyCreate '/CREARPARTY
		PartyJoin '/PARTY
		Inquiry '/ENCUESTA ( with no params )
		GuildMessage '/CMSG
		PartyMessage '/PMSG
		CentinelReport '/CENTINELA
		GuildOnline '/ONLINECLAN
		PartyOnline '/ONLINEPARTY
		CouncilMessage '/BMSG
		RoleMasterRequest '/ROL
		GMRequest '/GM
		bugReport '/_BUG
		ChangeDescription '/DESC
		GuildVote '/VOTO
		Punishments '/PENAS
		ChangePassword '/CONTRASEÑA
		Gamble '/APOSTAR
		InquiryVote '/ENCUESTA ( with parameters )
		LeaveFaction '/RETIRAR ( with no arguments )
		BankExtractGold '/RETIRAR ( with arguments )
		BankDepositGold '/DEPOSITAR
		Denounce '/DENUNCIAR
		GuildFundate '/FUNDARCLAN
		GuildFundation
		PartyKick '/ECHARPARTY
		PartySetLeader '/PARTYLIDER
		PartyAcceptMember '/ACCEPTPARTY
		Ping '/PING
		RequestPartyForm
		ItemUpgrade
		GMCommands
		InitCrafting
		Home
		ShowGuildNews
		ShareNpc '/COMPARTIR
		StopSharingNpc
		Consultation
	End Enum
	
	''
	'The last existing client packet id.
	Private Const LAST_CLIENT_PACKET_ID As Byte = 128
	
	Public Enum FontTypeNames
		FONTTYPE_TALK
		FONTTYPE_FIGHT
		FONTTYPE_WARNING
		FONTTYPE_INFO
		FONTTYPE_INFOBOLD
		FONTTYPE_EJECUCION
		FONTTYPE_PARTY
		FONTTYPE_VENENO
		FONTTYPE_GUILD
		FONTTYPE_SERVER
		FONTTYPE_GUILDMSG
		FONTTYPE_CONSEJO
		FONTTYPE_CONSEJOCAOS
		FONTTYPE_CONSEJOVesA
		FONTTYPE_CONSEJOCAOSVesA
		FONTTYPE_CENTINELA
		FONTTYPE_GMMSG
		FONTTYPE_GM
		FONTTYPE_CITIZEN
		FONTTYPE_CONSE
		FONTTYPE_DIOS
	End Enum
	
	Public Enum eEditOptions
		eo_Gold = 1
		eo_Experience
		eo_Body
		eo_Head
		eo_CiticensKilled
		eo_CriminalsKilled
		eo_Level
		eo_Class
		eo_Skills
		eo_SkillPointsLeft
		eo_Nobleza
		eo_Asesino
		eo_Sex
		eo_Raza
		eo_addGold
	End Enum
	
	
	
	''
	' Handles incoming data.
	'
	' @param    userIndex The index of the user sending the message.
	
	Public Sub HandleIncomingData(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 01/09/07
		'
		'***************************************************
		On Error Resume Next
		Dim packetID As Byte
		
		packetID = UserList(UserIndex).incomingData.PeekByte()
		
		'Does the packet requires a logged user??
		If Not (packetID = ClientPacketID.ThrowDices Or packetID = ClientPacketID.LoginExistingChar Or packetID = ClientPacketID.LoginNewChar) Then
			
			'Is the user actually logged?
			If Not UserList(UserIndex).flags.UserLogged Then
				Call CloseSocket(UserIndex)
				Exit Sub
				
				'He is logged. Reset idle counter if id is valid.
			ElseIf packetID <= LAST_CLIENT_PACKET_ID Then 
				UserList(UserIndex).Counters.IdleCount = 0
			End If
		ElseIf packetID <= LAST_CLIENT_PACKET_ID Then 
			UserList(UserIndex).Counters.IdleCount = 0
			
			'Is the user logged?
			If UserList(UserIndex).flags.UserLogged Then
				Call CloseSocket(UserIndex)
				Exit Sub
			End If
		End If
		
		' Ante cualquier paquete, pierde la proteccion de ser atacado.
		UserList(UserIndex).flags.NoPuedeSerAtacado = False
		
		Select Case packetID
			Case ClientPacketID.LoginExistingChar 'OLOGIN
				Call HandleLoginExistingChar(UserIndex)
				
			Case ClientPacketID.ThrowDices 'TIRDAD
				Call HandleThrowDices(UserIndex)
				
			Case ClientPacketID.LoginNewChar 'NLOGIN
				Call HandleLoginNewChar(UserIndex)
				
			Case ClientPacketID.Talk ';
				Call HandleTalk(UserIndex)
				
			Case ClientPacketID.Yell '-
				Call HandleYell(UserIndex)
				
			Case ClientPacketID.Whisper '\
				Call HandleWhisper(UserIndex)
				
			Case ClientPacketID.Walk 'M
				Call HandleWalk(UserIndex)
				
			Case ClientPacketID.RequestPositionUpdate 'RPU
				Call HandleRequestPositionUpdate(UserIndex)
				
			Case ClientPacketID.Attack 'AT
				Call HandleAttack(UserIndex)
				
			Case ClientPacketID.PickUp 'AG
				Call HandlePickUp(UserIndex)
				
			Case ClientPacketID.SafeToggle '/SEG & SEG  (SEG's behaviour has to be coded in the client)
				Call HandleSafeToggle(UserIndex)
				
			Case ClientPacketID.ResuscitationSafeToggle
				Call HandleResuscitationToggle(UserIndex)
				
			Case ClientPacketID.RequestGuildLeaderInfo 'GLINFO
				Call HandleRequestGuildLeaderInfo(UserIndex)
				
			Case ClientPacketID.RequestAtributes 'ATR
				Call HandleRequestAtributes(UserIndex)
				
			Case ClientPacketID.RequestFame 'FAMA
				Call HandleRequestFame(UserIndex)
				
			Case ClientPacketID.RequestSkills 'ESKI
				Call HandleRequestSkills(UserIndex)
				
			Case ClientPacketID.RequestMiniStats 'FEST
				Call HandleRequestMiniStats(UserIndex)
				
			Case ClientPacketID.CommerceEnd 'FINCOM
				Call HandleCommerceEnd(UserIndex)
				
			Case ClientPacketID.CommerceChat
				Call HandleCommerceChat(UserIndex)
				
			Case ClientPacketID.UserCommerceEnd 'FINCOMUSU
				Call HandleUserCommerceEnd(UserIndex)
				
			Case ClientPacketID.UserCommerceConfirm
				Call HandleUserCommerceConfirm(UserIndex)
				
			Case ClientPacketID.BankEnd 'FINBAN
				Call HandleBankEnd(UserIndex)
				
			Case ClientPacketID.UserCommerceOk 'COMUSUOK
				Call HandleUserCommerceOk(UserIndex)
				
			Case ClientPacketID.UserCommerceReject 'COMUSUNO
				Call HandleUserCommerceReject(UserIndex)
				
			Case ClientPacketID.Drop 'TI
				Call HandleDrop(UserIndex)
				
			Case ClientPacketID.CastSpell 'LH
				Call HandleCastSpell(UserIndex)
				
			Case ClientPacketID.LeftClick 'LC
				Call HandleLeftClick(UserIndex)
				
			Case ClientPacketID.DoubleClick 'RC
				Call HandleDoubleClick(UserIndex)
				
			Case ClientPacketID.Work 'UK
				Call HandleWork(UserIndex)
				
			Case ClientPacketID.UseSpellMacro 'UMH
				Call HandleUseSpellMacro(UserIndex)
				
			Case ClientPacketID.UseItem 'USA
				Call HandleUseItem(UserIndex)
				
			Case ClientPacketID.CraftBlacksmith 'CNS
				Call HandleCraftBlacksmith(UserIndex)
				
			Case ClientPacketID.CraftCarpenter 'CNC
				Call HandleCraftCarpenter(UserIndex)
				
			Case ClientPacketID.WorkLeftClick 'WLC
				Call HandleWorkLeftClick(UserIndex)
				
			Case ClientPacketID.CreateNewGuild 'CIG
				Call HandleCreateNewGuild(UserIndex)
				
			Case ClientPacketID.SpellInfo 'INFS
				Call HandleSpellInfo(UserIndex)
				
			Case ClientPacketID.EquipItem 'EQUI
				Call HandleEquipItem(UserIndex)
				
			Case ClientPacketID.ChangeHeading 'CHEA
				Call HandleChangeHeading(UserIndex)
				
			Case ClientPacketID.ModifySkills 'SKSE
				Call HandleModifySkills(UserIndex)
				
			Case ClientPacketID.Train 'ENTR
				Call HandleTrain(UserIndex)
				
			Case ClientPacketID.CommerceBuy 'COMP
				Call HandleCommerceBuy(UserIndex)
				
			Case ClientPacketID.BankExtractItem 'RETI
				Call HandleBankExtractItem(UserIndex)
				
			Case ClientPacketID.CommerceSell 'VEND
				Call HandleCommerceSell(UserIndex)
				
			Case ClientPacketID.BankDeposit 'DEPO
				Call HandleBankDeposit(UserIndex)
				
			Case ClientPacketID.ForumPost 'DEMSG
				Call HandleForumPost(UserIndex)
				
			Case ClientPacketID.MoveSpell 'DESPHE
				Call HandleMoveSpell(UserIndex)
				
			Case ClientPacketID.MoveBank
				Call HandleMoveBank(UserIndex)
				
			Case ClientPacketID.ClanCodexUpdate 'DESCOD
				Call HandleClanCodexUpdate(UserIndex)
				
			Case ClientPacketID.UserCommerceOffer 'OFRECER
				Call HandleUserCommerceOffer(UserIndex)
				
			Case ClientPacketID.GuildAcceptPeace 'ACEPPEAT
				Call HandleGuildAcceptPeace(UserIndex)
				
			Case ClientPacketID.GuildRejectAlliance 'RECPALIA
				Call HandleGuildRejectAlliance(UserIndex)
				
			Case ClientPacketID.GuildRejectPeace 'RECPPEAT
				Call HandleGuildRejectPeace(UserIndex)
				
			Case ClientPacketID.GuildAcceptAlliance 'ACEPALIA
				Call HandleGuildAcceptAlliance(UserIndex)
				
			Case ClientPacketID.GuildOfferPeace 'PEACEOFF
				Call HandleGuildOfferPeace(UserIndex)
				
			Case ClientPacketID.GuildOfferAlliance 'ALLIEOFF
				Call HandleGuildOfferAlliance(UserIndex)
				
			Case ClientPacketID.GuildAllianceDetails 'ALLIEDET
				Call HandleGuildAllianceDetails(UserIndex)
				
			Case ClientPacketID.GuildPeaceDetails 'PEACEDET
				Call HandleGuildPeaceDetails(UserIndex)
				
			Case ClientPacketID.GuildRequestJoinerInfo 'ENVCOMEN
				Call HandleGuildRequestJoinerInfo(UserIndex)
				
			Case ClientPacketID.GuildAlliancePropList 'ENVALPRO
				Call HandleGuildAlliancePropList(UserIndex)
				
			Case ClientPacketID.GuildPeacePropList 'ENVPROPP
				Call HandleGuildPeacePropList(UserIndex)
				
			Case ClientPacketID.GuildDeclareWar 'DECGUERR
				Call HandleGuildDeclareWar(UserIndex)
				
			Case ClientPacketID.GuildNewWebsite 'NEWWEBSI
				Call HandleGuildNewWebsite(UserIndex)
				
			Case ClientPacketID.GuildAcceptNewMember 'ACEPTARI
				Call HandleGuildAcceptNewMember(UserIndex)
				
			Case ClientPacketID.GuildRejectNewMember 'RECHAZAR
				Call HandleGuildRejectNewMember(UserIndex)
				
			Case ClientPacketID.GuildKickMember 'ECHARCLA
				Call HandleGuildKickMember(UserIndex)
				
			Case ClientPacketID.GuildUpdateNews 'ACTGNEWS
				Call HandleGuildUpdateNews(UserIndex)
				
			Case ClientPacketID.GuildMemberInfo '1HRINFO<
				Call HandleGuildMemberInfo(UserIndex)
				
			Case ClientPacketID.GuildOpenElections 'ABREELEC
				Call HandleGuildOpenElections(UserIndex)
				
			Case ClientPacketID.GuildRequestMembership 'SOLICITUD
				Call HandleGuildRequestMembership(UserIndex)
				
			Case ClientPacketID.GuildRequestDetails 'CLANDETAILS
				Call HandleGuildRequestDetails(UserIndex)
				
			Case ClientPacketID.Online '/ONLINE
				Call HandleOnline(UserIndex)
				
			Case ClientPacketID.Quit '/SALIR
				Call HandleQuit(UserIndex)
				
			Case ClientPacketID.GuildLeave '/SALIRCLAN
				Call HandleGuildLeave(UserIndex)
				
			Case ClientPacketID.RequestAccountState '/BALANCE
				Call HandleRequestAccountState(UserIndex)
				
			Case ClientPacketID.PetStand '/QUIETO
				Call HandlePetStand(UserIndex)
				
			Case ClientPacketID.PetFollow '/ACOMPAÑAR
				Call HandlePetFollow(UserIndex)
				
			Case ClientPacketID.ReleasePet '/LIBERAR
				Call HandleReleasePet(UserIndex)
				
			Case ClientPacketID.TrainList '/ENTRENAR
				Call HandleTrainList(UserIndex)
				
			Case ClientPacketID.Rest '/DESCANSAR
				Call HandleRest(UserIndex)
				
			Case ClientPacketID.Meditate '/MEDITAR
				Call HandleMeditate(UserIndex)
				
			Case ClientPacketID.Resucitate '/RESUCITAR
				Call HandleResucitate(UserIndex)
				
			Case ClientPacketID.Heal '/CURAR
				Call HandleHeal(UserIndex)
				
			Case ClientPacketID.Help '/AYUDA
				Call HandleHelp(UserIndex)
				
			Case ClientPacketID.RequestStats '/EST
				Call HandleRequestStats(UserIndex)
				
			Case ClientPacketID.CommerceStart '/COMERCIAR
				Call HandleCommerceStart(UserIndex)
				
			Case ClientPacketID.BankStart '/BOVEDA
				Call HandleBankStart(UserIndex)
				
			Case ClientPacketID.Enlist '/ENLISTAR
				Call HandleEnlist(UserIndex)
				
			Case ClientPacketID.Information '/INFORMACION
				Call HandleInformation(UserIndex)
				
			Case ClientPacketID.Reward '/RECOMPENSA
				Call HandleReward(UserIndex)
				
			Case ClientPacketID.RequestMOTD '/MOTD
				Call HandleRequestMOTD(UserIndex)
				
			Case ClientPacketID.UpTime '/UPTIME
				Call HandleUpTime(UserIndex)
				
			Case ClientPacketID.PartyLeave '/SALIRPARTY
				Call HandlePartyLeave(UserIndex)
				
			Case ClientPacketID.PartyCreate '/CREARPARTY
				Call HandlePartyCreate(UserIndex)
				
			Case ClientPacketID.PartyJoin '/PARTY
				Call HandlePartyJoin(UserIndex)
				
			Case ClientPacketID.Inquiry '/ENCUESTA ( with no params )
				Call HandleInquiry(UserIndex)
				
			Case ClientPacketID.GuildMessage '/CMSG
				Call HandleGuildMessage(UserIndex)
				
			Case ClientPacketID.PartyMessage '/PMSG
				Call HandlePartyMessage(UserIndex)
				
			Case ClientPacketID.CentinelReport '/CENTINELA
				Call HandleCentinelReport(UserIndex)
				
			Case ClientPacketID.GuildOnline '/ONLINECLAN
				Call HandleGuildOnline(UserIndex)
				
			Case ClientPacketID.PartyOnline '/ONLINEPARTY
				Call HandlePartyOnline(UserIndex)
				
			Case ClientPacketID.CouncilMessage '/BMSG
				Call HandleCouncilMessage(UserIndex)
				
			Case ClientPacketID.RoleMasterRequest '/ROL
				Call HandleRoleMasterRequest(UserIndex)
				
			Case ClientPacketID.GMRequest '/GM
				Call HandleGMRequest(UserIndex)
				
			Case ClientPacketID.bugReport '/_BUG
				Call HandleBugReport(UserIndex)
				
			Case ClientPacketID.ChangeDescription '/DESC
				Call HandleChangeDescription(UserIndex)
				
			Case ClientPacketID.GuildVote '/VOTO
				Call HandleGuildVote(UserIndex)
				
			Case ClientPacketID.Punishments '/PENAS
				Call HandlePunishments(UserIndex)
				
			Case ClientPacketID.ChangePassword '/CONTRASEÑA
				Call HandleChangePassword(UserIndex)
				
			Case ClientPacketID.Gamble '/APOSTAR
				Call HandleGamble(UserIndex)
				
			Case ClientPacketID.InquiryVote '/ENCUESTA ( with parameters )
				Call HandleInquiryVote(UserIndex)
				
			Case ClientPacketID.LeaveFaction '/RETIRAR ( with no arguments )
				Call HandleLeaveFaction(UserIndex)
				
			Case ClientPacketID.BankExtractGold '/RETIRAR ( with arguments )
				Call HandleBankExtractGold(UserIndex)
				
			Case ClientPacketID.BankDepositGold '/DEPOSITAR
				Call HandleBankDepositGold(UserIndex)
				
			Case ClientPacketID.Denounce '/DENUNCIAR
				Call HandleDenounce(UserIndex)
				
			Case ClientPacketID.GuildFundate '/FUNDARCLAN
				Call HandleGuildFundate(UserIndex)
				
			Case ClientPacketID.GuildFundation
				Call HandleGuildFundation(UserIndex)
				
			Case ClientPacketID.PartyKick '/ECHARPARTY
				Call HandlePartyKick(UserIndex)
				
			Case ClientPacketID.PartySetLeader '/PARTYLIDER
				Call HandlePartySetLeader(UserIndex)
				
			Case ClientPacketID.PartyAcceptMember '/ACCEPTPARTY
				Call HandlePartyAcceptMember(UserIndex)
				
			Case ClientPacketID.Ping '/PING
				Call HandlePing(UserIndex)
				
			Case ClientPacketID.RequestPartyForm
				Call HandlePartyForm(UserIndex)
				
			Case ClientPacketID.ItemUpgrade
				Call HandleItemUpgrade(UserIndex)
				
			Case ClientPacketID.GMCommands 'GM Messages
				Call HandleGMCommands(UserIndex)
				
			Case ClientPacketID.InitCrafting
				Call HandleInitCrafting(UserIndex)
				
			Case ClientPacketID.Home
				Call HandleHome(UserIndex)
				
			Case ClientPacketID.ShowGuildNews
				Call HandleShowGuildNews(UserIndex)
				
			Case ClientPacketID.ShareNpc
				Call HandleShareNpc(UserIndex)
				
			Case ClientPacketID.StopSharingNpc
				Call HandleStopSharingNpc(UserIndex)
				
			Case ClientPacketID.Consultation
				Call HandleConsultation(CStr(UserIndex))
				
			Case Else
				'ERROR : Abort!
				Call CloseSocket(UserIndex)
		End Select
		
		'Done with this packet, move on to next one or send everything if no more packets found
		If UserList(UserIndex).incomingData.length > 0 And Err.Number = 0 Then
			Err.Clear()
			Call HandleIncomingData(UserIndex)
			
		ElseIf Err.Number <> 0 And Not Err.Number = UserList(UserIndex).incomingData.NotEnoughDataErrCode Then 
			'An error ocurred, log it and kick player.
			Call LogError("Error: " & Err.Number & " [" & Err.Description & "] " & " Source: " & Err.Source & vbTab & " HelpFile: " & Err.HelpFile & vbTab & " HelpContext: " & Err.HelpContext & vbTab & " LastDllError: " & Err.LastDllError & vbTab & " - UserIndex: " & UserIndex & " - producido al manejar el paquete: " & CStr(packetID))
			Call CloseSocket(UserIndex)
			
		Else
			'Flush buffer - send everything that has been written
			Call FlushBuffer(UserIndex)
		End If
	End Sub
	
	Public Sub WriteMultiMessage(ByVal UserIndex As Short, ByVal MessageIndex As Short, Optional ByVal Arg1 As Integer = 0, Optional ByVal Arg2 As Integer = 0, Optional ByVal Arg3 As Integer = 0, Optional ByVal StringArg1 As String = "")
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.MultiMessage)
			Call .WriteByte(MessageIndex)
			
			Select Case MessageIndex
				Case Declaraciones.eMessages.DontSeeAnything, Declaraciones.eMessages.NPCSwing, Declaraciones.eMessages.NPCKillUser, Declaraciones.eMessages.BlockedWithShieldUser, Declaraciones.eMessages.BlockedWithShieldother, Declaraciones.eMessages.UserSwing, Declaraciones.eMessages.SafeModeOn, Declaraciones.eMessages.SafeModeOff, Declaraciones.eMessages.ResuscitationSafeOff, Declaraciones.eMessages.ResuscitationSafeOn, Declaraciones.eMessages.NobilityLost, Declaraciones.eMessages.CantUseWhileMeditating, Declaraciones.eMessages.CancelHome, Declaraciones.eMessages.FinishHome
					
				Case Declaraciones.eMessages.NPCHitUser
					Call .WriteByte(Arg1) 'Target
					Call .WriteInteger(Arg2) 'damage
					
				Case Declaraciones.eMessages.UserHitNPC
					Call .WriteLong(Arg1) 'damage
					
				Case Declaraciones.eMessages.UserAttackedSwing
					Call .WriteInteger(UserList(Arg1).Char_Renamed.CharIndex)
					
				Case Declaraciones.eMessages.UserHittedByUser
					Call .WriteInteger(Arg1) 'AttackerIndex
					Call .WriteByte(Arg2) 'Target
					Call .WriteInteger(Arg3) 'damage
					
				Case Declaraciones.eMessages.UserHittedUser
					Call .WriteInteger(Arg1) 'AttackerIndex
					Call .WriteByte(Arg2) 'Target
					Call .WriteInteger(Arg3) 'damage
					
				Case Declaraciones.eMessages.WorkRequestTarget
					Call .WriteByte(Arg1) 'skill
					
				Case Declaraciones.eMessages.HaveKilledUser '"Has matado a " & UserList(VictimIndex).name & "!" "Has ganado " & DaExp & " puntos de experiencia."
					Call .WriteInteger(UserList(Arg1).Char_Renamed.CharIndex) 'VictimIndex
					Call .WriteLong(Arg2) 'Expe
					
				Case Declaraciones.eMessages.UserKill '"¡" & .name & " te ha matado!"
					Call .WriteInteger(UserList(Arg1).Char_Renamed.CharIndex) 'AttackerIndex
					
				Case Declaraciones.eMessages.EarnExp
					
				Case Declaraciones.eMessages.Home
					Call .WriteByte(CByte(Arg1))
					Call .WriteInteger(CShort(Arg2))
					'El cliente no conoce nada sobre nombre de mapas y hogares, por lo tanto _
					'hasta que no se pasen los dats e .INFs al cliente, esto queda así.
					Call .WriteASCIIString(StringArg1) 'Call .WriteByte(CByte(Arg2))
					
			End Select
		End With
		Exit Sub ''
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	Private Sub HandleGMCommands(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		'UPGRADE_NOTE: Command se actualizó a Command_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim Command_Renamed As Byte
		
		With UserList(UserIndex)
			Call .incomingData.ReadByte()
			
			Command_Renamed = .incomingData.PeekByte
			
			Select Case Command_Renamed
				Case Declaraciones.eGMCommands.GMMessage '/GMSG
					Call HandleGMMessage(UserIndex)
					
				Case Declaraciones.eGMCommands.showName '/SHOWNAME
					Call HandleShowName(UserIndex)
					
				Case Declaraciones.eGMCommands.OnlineRoyalArmy
					Call HandleOnlineRoyalArmy(UserIndex)
					
				Case Declaraciones.eGMCommands.OnlineChaosLegion '/ONLINECAOS
					Call HandleOnlineChaosLegion(UserIndex)
					
				Case Declaraciones.eGMCommands.GoNearby '/IRCERCA
					Call HandleGoNearby(UserIndex)
					
				Case Declaraciones.eGMCommands.comment '/REM
					Call HandleComment(UserIndex)
					
				Case Declaraciones.eGMCommands.serverTime '/HORA
					Call HandleServerTime(UserIndex)
					
				Case Declaraciones.eGMCommands.Where '/DONDE
					Call HandleWhere(UserIndex)
					
				Case Declaraciones.eGMCommands.CreaturesInMap '/NENE
					Call HandleCreaturesInMap(UserIndex)
					
				Case Declaraciones.eGMCommands.WarpMeToTarget '/TELEPLOC
					Call HandleWarpMeToTarget(UserIndex)
					
				Case Declaraciones.eGMCommands.WarpChar '/TELEP
					Call HandleWarpChar(UserIndex)
					
				Case Declaraciones.eGMCommands.Silence '/SILENCIAR
					Call HandleSilence(UserIndex)
					
				Case Declaraciones.eGMCommands.SOSShowList '/SHOW SOS
					Call HandleSOSShowList(UserIndex)
					
				Case Declaraciones.eGMCommands.SOSRemove 'SOSDONE
					Call HandleSOSRemove(UserIndex)
					
				Case Declaraciones.eGMCommands.GoToChar '/IRA
					Call HandleGoToChar(UserIndex)
					
				Case Declaraciones.eGMCommands.invisible '/INVISIBLE
					Call HandleInvisible(UserIndex)
					
				Case Declaraciones.eGMCommands.GMPanel '/PANELGM
					Call HandleGMPanel(UserIndex)
					
				Case Declaraciones.eGMCommands.RequestUserList 'LISTUSU
					Call HandleRequestUserList(UserIndex)
					
				Case Declaraciones.eGMCommands.Working '/TRABAJANDO
					Call HandleWorking(UserIndex)
					
				Case Declaraciones.eGMCommands.Hiding '/OCULTANDO
					Call HandleHiding(UserIndex)
					
				Case Declaraciones.eGMCommands.Jail '/CARCEL
					Call HandleJail(UserIndex)
					
				Case Declaraciones.eGMCommands.KillNPC '/RMATA
					Call HandleKillNPC(UserIndex)
					
				Case Declaraciones.eGMCommands.WarnUser '/ADVERTENCIA
					Call HandleWarnUser(UserIndex)
					
				Case Declaraciones.eGMCommands.EditChar '/MOD
					Call HandleEditChar(UserIndex)
					
				Case Declaraciones.eGMCommands.RequestCharInfo '/INFO
					Call HandleRequestCharInfo(UserIndex)
					
				Case Declaraciones.eGMCommands.RequestCharStats '/STAT
					Call HandleRequestCharStats(UserIndex)
					
				Case Declaraciones.eGMCommands.RequestCharGold '/BAL
					Call HandleRequestCharGold(UserIndex)
					
				Case Declaraciones.eGMCommands.RequestCharInventory '/INV
					Call HandleRequestCharInventory(UserIndex)
					
				Case Declaraciones.eGMCommands.RequestCharBank '/BOV
					Call HandleRequestCharBank(UserIndex)
					
				Case Declaraciones.eGMCommands.RequestCharSkills '/SKILLS
					Call HandleRequestCharSkills(UserIndex)
					
				Case Declaraciones.eGMCommands.ReviveChar '/REVIVIR
					Call HandleReviveChar(UserIndex)
					
				Case Declaraciones.eGMCommands.OnlineGM '/ONLINEGM
					Call HandleOnlineGM(UserIndex)
					
				Case Declaraciones.eGMCommands.OnlineMap '/ONLINEMAP
					Call HandleOnlineMap(UserIndex)
					
				Case Declaraciones.eGMCommands.Forgive '/PERDON
					Call HandleForgive(UserIndex)
					
				Case Declaraciones.eGMCommands.Kick '/ECHAR
					Call HandleKick(UserIndex)
					
				Case Declaraciones.eGMCommands.Execute '/EJECUTAR
					Call HandleExecute(UserIndex)
					
				Case Declaraciones.eGMCommands.BanChar '/BAN
					Call HandleBanChar(UserIndex)
					
				Case Declaraciones.eGMCommands.UnbanChar '/UNBAN
					Call HandleUnbanChar(UserIndex)
					
				Case Declaraciones.eGMCommands.NPCFollow '/SEGUIR
					Call HandleNPCFollow(UserIndex)
					
				Case Declaraciones.eGMCommands.SummonChar '/SUM
					Call HandleSummonChar(UserIndex)
					
				Case Declaraciones.eGMCommands.SpawnListRequest '/CC
					Call HandleSpawnListRequest(UserIndex)
					
				Case Declaraciones.eGMCommands.SpawnCreature 'SPA
					Call HandleSpawnCreature(UserIndex)
					
				Case Declaraciones.eGMCommands.ResetNPCInventory '/RESETINV
					Call HandleResetNPCInventory(UserIndex)
					
				Case Declaraciones.eGMCommands.CleanWorld '/LIMPIAR
					Call HandleCleanWorld(UserIndex)
					
				Case Declaraciones.eGMCommands.ServerMessage '/RMSG
					Call HandleServerMessage(UserIndex)
					
				Case Declaraciones.eGMCommands.NickToIP '/NICK2IP
					Call HandleNickToIP(UserIndex)
					
				Case Declaraciones.eGMCommands.IPToNick '/IP2NICK
					Call HandleIPToNick(UserIndex)
					
				Case Declaraciones.eGMCommands.GuildOnlineMembers '/ONCLAN
					Call HandleGuildOnlineMembers(UserIndex)
					
				Case Declaraciones.eGMCommands.TeleportCreate '/CT
					Call HandleTeleportCreate(UserIndex)
					
				Case Declaraciones.eGMCommands.TeleportDestroy '/DT
					Call HandleTeleportDestroy(UserIndex)
					
				Case Declaraciones.eGMCommands.RainToggle '/LLUVIA
					Call HandleRainToggle(UserIndex)
					
				Case Declaraciones.eGMCommands.SetCharDescription '/SETDESC
					Call HandleSetCharDescription(UserIndex)
					
				Case Declaraciones.eGMCommands.ForceMIDIToMap '/FORCEMIDIMAP
					Call HanldeForceMIDIToMap(UserIndex)
					
				Case Declaraciones.eGMCommands.ForceWAVEToMap '/FORCEWAVMAP
					Call HandleForceWAVEToMap(UserIndex)
					
				Case Declaraciones.eGMCommands.RoyalArmyMessage '/REALMSG
					Call HandleRoyalArmyMessage(UserIndex)
					
				Case Declaraciones.eGMCommands.ChaosLegionMessage '/CAOSMSG
					Call HandleChaosLegionMessage(UserIndex)
					
				Case Declaraciones.eGMCommands.CitizenMessage '/CIUMSG
					Call HandleCitizenMessage(UserIndex)
					
				Case Declaraciones.eGMCommands.CriminalMessage '/CRIMSG
					Call HandleCriminalMessage(UserIndex)
					
				Case Declaraciones.eGMCommands.TalkAsNPC '/TALKAS
					Call HandleTalkAsNPC(UserIndex)
					
				Case Declaraciones.eGMCommands.DestroyAllItemsInArea '/MASSDEST
					Call HandleDestroyAllItemsInArea(UserIndex)
					
				Case Declaraciones.eGMCommands.AcceptRoyalCouncilMember '/ACEPTCONSE
					Call HandleAcceptRoyalCouncilMember(UserIndex)
					
				Case Declaraciones.eGMCommands.AcceptChaosCouncilMember '/ACEPTCONSECAOS
					Call HandleAcceptChaosCouncilMember(UserIndex)
					
				Case Declaraciones.eGMCommands.ItemsInTheFloor '/PISO
					Call HandleItemsInTheFloor(UserIndex)
					
				Case Declaraciones.eGMCommands.MakeDumb '/ESTUPIDO
					Call HandleMakeDumb(UserIndex)
					
				Case Declaraciones.eGMCommands.MakeDumbNoMore '/NOESTUPIDO
					Call HandleMakeDumbNoMore(UserIndex)
					
				Case Declaraciones.eGMCommands.DumpIPTables '/DUMPSECURITY
					Call HandleDumpIPTables(UserIndex)
					
				Case Declaraciones.eGMCommands.CouncilKick '/KICKCONSE
					Call HandleCouncilKick(UserIndex)
					
				Case Declaraciones.eGMCommands.SetTrigger '/TRIGGER
					Call HandleSetTrigger(UserIndex)
					
				Case Declaraciones.eGMCommands.AskTrigger '/TRIGGER with no args
					Call HandleAskTrigger(UserIndex)
					
				Case Declaraciones.eGMCommands.BannedIPList '/BANIPLIST
					Call HandleBannedIPList(UserIndex)
					
				Case Declaraciones.eGMCommands.BannedIPReload '/BANIPRELOAD
					Call HandleBannedIPReload(UserIndex)
					
				Case Declaraciones.eGMCommands.GuildMemberList '/MIEMBROSCLAN
					Call HandleGuildMemberList(UserIndex)
					
				Case Declaraciones.eGMCommands.GuildBan '/BANCLAN
					Call HandleGuildBan(UserIndex)
					
				Case Declaraciones.eGMCommands.BanIP '/BANIP
					Call HandleBanIP(UserIndex)
					
				Case Declaraciones.eGMCommands.UnbanIP '/UNBANIP
					Call HandleUnbanIP(UserIndex)
					
				Case Declaraciones.eGMCommands.CreateItem '/CI
					Call HandleCreateItem(UserIndex)
					
				Case Declaraciones.eGMCommands.DestroyItems '/DEST
					Call HandleDestroyItems(UserIndex)
					
				Case Declaraciones.eGMCommands.ChaosLegionKick '/NOCAOS
					Call HandleChaosLegionKick(UserIndex)
					
				Case Declaraciones.eGMCommands.RoyalArmyKick '/NOREAL
					Call HandleRoyalArmyKick(UserIndex)
					
				Case Declaraciones.eGMCommands.ForceMIDIAll '/FORCEMIDI
					Call HandleForceMIDIAll(UserIndex)
					
				Case Declaraciones.eGMCommands.ForceWAVEAll '/FORCEWAV
					Call HandleForceWAVEAll(UserIndex)
					
				Case Declaraciones.eGMCommands.RemovePunishment '/BORRARPENA
					Call HandleRemovePunishment(UserIndex)
					
				Case Declaraciones.eGMCommands.TileBlockedToggle '/BLOQ
					Call HandleTileBlockedToggle(UserIndex)
					
				Case Declaraciones.eGMCommands.KillNPCNoRespawn '/MATA
					Call HandleKillNPCNoRespawn(UserIndex)
					
				Case Declaraciones.eGMCommands.KillAllNearbyNPCs '/MASSKILL
					Call HandleKillAllNearbyNPCs(UserIndex)
					
				Case Declaraciones.eGMCommands.LastIP '/LASTIP
					Call HandleLastIP(UserIndex)
					
				Case Declaraciones.eGMCommands.ChangeMOTD '/MOTDCAMBIA
					Call HandleChangeMOTD(UserIndex)
					
				Case Declaraciones.eGMCommands.SetMOTD 'ZMOTD
					Call HandleSetMOTD(UserIndex)
					
				Case Declaraciones.eGMCommands.SystemMessage '/SMSG
					Call HandleSystemMessage(UserIndex)
					
				Case Declaraciones.eGMCommands.CreateNPC '/ACC
					Call HandleCreateNPC(UserIndex)
					
				Case Declaraciones.eGMCommands.CreateNPCWithRespawn '/RACC
					Call HandleCreateNPCWithRespawn(UserIndex)
					
				Case Declaraciones.eGMCommands.ImperialArmour '/AI1 - 4
					Call HandleImperialArmour(UserIndex)
					
				Case Declaraciones.eGMCommands.ChaosArmour '/AC1 - 4
					Call HandleChaosArmour(UserIndex)
					
				Case Declaraciones.eGMCommands.NavigateToggle '/NAVE
					Call HandleNavigateToggle(UserIndex)
					
				Case Declaraciones.eGMCommands.ServerOpenToUsersToggle '/HABILITAR
					Call HandleServerOpenToUsersToggle(UserIndex)
					
				Case Declaraciones.eGMCommands.TurnOffServer '/APAGAR
					Call HandleTurnOffServer(UserIndex)
					
				Case Declaraciones.eGMCommands.TurnCriminal '/CONDEN
					Call HandleTurnCriminal(UserIndex)
					
				Case Declaraciones.eGMCommands.ResetFactions '/RAJAR
					Call HandleResetFactions(UserIndex)
					
				Case Declaraciones.eGMCommands.RemoveCharFromGuild '/RAJARCLAN
					Call HandleRemoveCharFromGuild(UserIndex)
					
				Case Declaraciones.eGMCommands.RequestCharMail '/LASTEMAIL
					Call HandleRequestCharMail(UserIndex)
					
				Case Declaraciones.eGMCommands.AlterPassword '/APASS
					Call HandleAlterPassword(UserIndex)
					
				Case Declaraciones.eGMCommands.AlterMail '/AEMAIL
					Call HandleAlterMail(UserIndex)
					
				Case Declaraciones.eGMCommands.AlterName '/ANAME
					Call HandleAlterName(UserIndex)
					
				Case Declaraciones.eGMCommands.ToggleCentinelActivated '/CENTINELAACTIVADO
					Call HandleToggleCentinelActivated(UserIndex)
					
				Case Declaraciones.eGMCommands.DoBackUp '/DOBACKUP
					Call HandleDoBackUp(UserIndex)
					
				Case Declaraciones.eGMCommands.ShowGuildMessages '/SHOWCMSG
					Call HandleShowGuildMessages(UserIndex)
					
				Case Declaraciones.eGMCommands.SaveMap '/GUARDAMAPA
					Call HandleSaveMap(UserIndex)
					
				Case Declaraciones.eGMCommands.ChangeMapInfoPK '/MODMAPINFO PK
					Call HandleChangeMapInfoPK(UserIndex)
					
				Case Declaraciones.eGMCommands.ChangeMapInfoBackup '/MODMAPINFO BACKUP
					Call HandleChangeMapInfoBackup(UserIndex)
					
				Case Declaraciones.eGMCommands.ChangeMapInfoRestricted '/MODMAPINFO RESTRINGIR
					Call HandleChangeMapInfoRestricted(UserIndex)
					
				Case Declaraciones.eGMCommands.ChangeMapInfoNoMagic '/MODMAPINFO MAGIASINEFECTO
					Call HandleChangeMapInfoNoMagic(UserIndex)
					
				Case Declaraciones.eGMCommands.ChangeMapInfoNoInvi '/MODMAPINFO INVISINEFECTO
					Call HandleChangeMapInfoNoInvi(UserIndex)
					
				Case Declaraciones.eGMCommands.ChangeMapInfoNoResu '/MODMAPINFO RESUSINEFECTO
					Call HandleChangeMapInfoNoResu(UserIndex)
					
				Case Declaraciones.eGMCommands.ChangeMapInfoLand '/MODMAPINFO TERRENO
					Call HandleChangeMapInfoLand(UserIndex)
					
				Case Declaraciones.eGMCommands.ChangeMapInfoZone '/MODMAPINFO ZONA
					Call HandleChangeMapInfoZone(UserIndex)
					
				Case Declaraciones.eGMCommands.SaveChars '/GRABAR
					Call HandleSaveChars(UserIndex)
					
				Case Declaraciones.eGMCommands.CleanSOS '/BORRAR SOS
					Call HandleCleanSOS(UserIndex)
					
				Case Declaraciones.eGMCommands.ShowServerForm '/SHOW INT
					Call HandleShowServerForm(UserIndex)
					
				Case Declaraciones.eGMCommands.night '/NOCHE
					Call HandleNight(UserIndex)
					
				Case Declaraciones.eGMCommands.KickAllChars '/ECHARTODOSPJS
					Call HandleKickAllChars(UserIndex)
					
				Case Declaraciones.eGMCommands.ReloadNPCs '/RELOADNPCS
					Call HandleReloadNPCs(UserIndex)
					
				Case Declaraciones.eGMCommands.ReloadServerIni '/RELOADSINI
					Call HandleReloadServerIni(UserIndex)
					
				Case Declaraciones.eGMCommands.ReloadSpells '/RELOADHECHIZOS
					Call HandleReloadSpells(UserIndex)
					
				Case Declaraciones.eGMCommands.ReloadObjects '/RELOADOBJ
					Call HandleReloadObjects(UserIndex)
					
				Case Declaraciones.eGMCommands.Restart '/REINICIAR
					Call HandleRestart(UserIndex)
					
				Case Declaraciones.eGMCommands.ResetAutoUpdate '/AUTOUPDATE
					Call HandleResetAutoUpdate(UserIndex)
					
				Case Declaraciones.eGMCommands.ChatColor '/CHATCOLOR
					Call HandleChatColor(UserIndex)
					
				Case Declaraciones.eGMCommands.Ignored '/IGNORADO
					Call HandleIgnored(UserIndex)
					
				Case Declaraciones.eGMCommands.CheckSlot '/SLOT
					Call HandleCheckSlot(UserIndex)
					
				Case Declaraciones.eGMCommands.SetIniVar '/SETINIVAR LLAVE CLAVE VALOR
					Call HandleSetIniVar(UserIndex)
			End Select
		End With
		
		Exit Sub
		
Errhandler: 
		Call LogError("Error en GmCommands. Error: " & Err.Number & " - " & Err.Description & ". Paquete: " & Command_Renamed)
		
	End Sub
	
	''
	' Handles the "Home" message.
	'
	' @param    userIndex The index of the user sending the message.
	Private Sub HandleHome(ByVal UserIndex As Short)
		'***************************************************
		'Author: Budi
		'Creation Date: 06/01/2010
		'Last Modification: 05/06/10
		'Pato - 05/06/10: Add the Ucase$ to prevent problems.
		'***************************************************
		With UserList(UserIndex)
			Call .incomingData.ReadByte()
			If .flags.TargetNpcTipo = Declaraciones.eNPCType.Gobernador Then
				Call setHome(UserIndex, Npclist(.flags.TargetNPC).Ciudad, .flags.TargetNPC)
			Else
				If .flags.Muerto = 1 Then
					'Si es un mapa común y no está en cana
					If UCase(MapInfo_Renamed(.Pos.Map).Restringir) = "NO" And .Counters.Pena = 0 Then
						If .flags.Traveling = 0 Then
							If Ciudades(.Hogar).Map <> .Pos.Map Then
								Call goHome(UserIndex)
							Else
								Call WriteConsoleMsg(UserIndex, "Ya te encuentras en tu hogar.", FontTypeNames.FONTTYPE_INFO)
							End If
						Else
							Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.CancelHome)
							.flags.Traveling = 0
							.Counters.goHome = 0
						End If
					Else
						Call WriteConsoleMsg(UserIndex, "No puedes usar este comando aquí.", FontTypeNames.FONTTYPE_FIGHT)
					End If
				Else
					Call WriteConsoleMsg(UserIndex, "Debes estar muerto para utilizar este comando.", FontTypeNames.FONTTYPE_INFO)
				End If
			End If
		End With
	End Sub
	
	''
	' Handles the "LoginExistingChar" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleLoginExistingChar(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 6 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
		Dim buffer As New clsByteQueue
		Call buffer.CopyBuffer(UserList(UserIndex).incomingData)
		
		'Remove packet ID
		Call buffer.ReadByte()
		
		Dim UserName As String
		Dim Password As String
		Dim version As String
		
		UserName = buffer.ReadASCIIString()
		Password = buffer.ReadASCIIString()
		
		'Convert version number to string
		version = CStr(buffer.ReadByte()) & "." & CStr(buffer.ReadByte()) & "." & CStr(buffer.ReadByte())
		
		If Not AsciiValidos(UserName) Then
			Call WriteErrorMsg(UserIndex, "Nombre inválido.")
			Call FlushBuffer(UserIndex)
			Call CloseSocket(UserIndex)
			
			Exit Sub
		End If
		
		If Not PersonajeExiste(UserName) Then
			Call WriteErrorMsg(UserIndex, "El personaje no existe.")
			Call FlushBuffer(UserIndex)
			Call CloseSocket(UserIndex)
			
			Exit Sub
		End If
		
		If BANCheck(UserName) Then
			Call WriteErrorMsg(UserIndex, "Se te ha prohibido la entrada a Argentum Online debido a tu mal comportamiento. Puedes consultar el reglamento y el sistema de soporte desde www.argentumonline.com.ar")
		ElseIf Not VersionOK(version) Then 
			Call WriteErrorMsg(UserIndex, "Esta versión del juego es obsoleta, la versión correcta es la " & ULTIMAVERSION & ". La misma se encuentra disponible en www.argentumonline.com.ar")
		Else
			Call ConnectUser(UserIndex, UserName, Password)
		End If
		
		'If we got here then packet is complete, copy data back to original queue
		Call UserList(UserIndex).incomingData.CopyBuffer(buffer)
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "ThrowDices" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleThrowDices(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()
		
		With UserList(UserIndex).Stats
			.UserAtributos(Declaraciones.eAtributos.Fuerza) = MaximoInt(15, 13 + RandomNumber(0, 3) + RandomNumber(0, 2))
			.UserAtributos(Declaraciones.eAtributos.Agilidad) = MaximoInt(15, 12 + RandomNumber(0, 3) + RandomNumber(0, 3))
			.UserAtributos(Declaraciones.eAtributos.Inteligencia) = MaximoInt(16, 13 + RandomNumber(0, 3) + RandomNumber(0, 2))
			.UserAtributos(Declaraciones.eAtributos.Carisma) = MaximoInt(15, 12 + RandomNumber(0, 3) + RandomNumber(0, 3))
			.UserAtributos(Declaraciones.eAtributos.Constitucion) = 16 + RandomNumber(0, 1) + RandomNumber(0, 1)
		End With
		
		Call WriteDiceRoll(UserIndex)
	End Sub
	
	''
	' Handles the "LoginNewChar" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleLoginNewChar(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 15 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
		Dim buffer As New clsByteQueue
		Call buffer.CopyBuffer(UserList(UserIndex).incomingData)
		
		'Remove packet ID
		Call buffer.ReadByte()
		
		Dim UserName As String
		Dim Password As String
		Dim version As String
		Dim race As Declaraciones.eRaza
		Dim gender As Declaraciones.eGenero
		Dim homeland As Declaraciones.eCiudad
		'UPGRADE_NOTE: Class se actualizó a Class_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim Class_Renamed As Declaraciones.eClass
		Dim Head As Short
		Dim mail As String
		
		If PuedeCrearPersonajes = 0 Then
			Call WriteErrorMsg(UserIndex, "La creación de personajes en este servidor se ha deshabilitado.")
			Call FlushBuffer(UserIndex)
			Call CloseSocket(UserIndex)
			
			Exit Sub
		End If
		
		If ServerSoloGMs <> 0 Then
			Call WriteErrorMsg(UserIndex, "Servidor restringido a administradores. Consulte la página oficial o el foro oficial para más información.")
			Call FlushBuffer(UserIndex)
			Call CloseSocket(UserIndex)
			
			Exit Sub
		End If
		
		If aClon.MaxPersonajes(UserList(UserIndex).ip) Then
			Call WriteErrorMsg(UserIndex, "Has creado demasiados personajes.")
			Call FlushBuffer(UserIndex)
			Call CloseSocket(UserIndex)
			
			Exit Sub
		End If
		
		UserName = buffer.ReadASCIIString()
		Password = buffer.ReadASCIIString()
		
		'Convert version number to string
		version = CStr(buffer.ReadByte()) & "." & CStr(buffer.ReadByte()) & "." & CStr(buffer.ReadByte())
		race = buffer.ReadByte()
		gender = buffer.ReadByte()
		Class_Renamed = buffer.ReadByte()
		Head = buffer.ReadInteger
		mail = buffer.ReadASCIIString()
		homeland = buffer.ReadByte()
		
		If Not VersionOK(version) Then
			Call WriteErrorMsg(UserIndex, "Esta versión del juego es obsoleta, la versión correcta es la " & ULTIMAVERSION & ". La misma se encuentra disponible en www.argentumonline.com.ar")
		Else
			Call ConnectNewUser(UserIndex, UserName, Password, race, gender, Class_Renamed, mail, homeland, Head)
		End If
		
		'If we got here then packet is complete, copy data back to original queue
		Call UserList(UserIndex).incomingData.CopyBuffer(buffer)
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "Talk" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleTalk(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 13/01/2010
		'15/07/2009: ZaMa - Now invisible admins talk by console.
		'23/09/2009: ZaMa - Now invisible admins can't send empty chat.
		'13/01/2010: ZaMa - Now hidden on boat pirats recover the proper boat body.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim Chat As String
		With UserList(UserIndex)
			
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			Chat = buffer.ReadASCIIString()
			
			'[Consejeros & GMs]
			If .flags.Privilegios And (Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then
				Call LogGM(.name, "Dijo: " & Chat)
			End If
			
			'I see you....
			If .flags.Oculto > 0 Then
				.flags.Oculto = 0
				.Counters.TiempoOculto = 0
				
				If .flags.Navegando = 1 Then
					If .clase = Declaraciones.eClass.Pirat Then
						' Pierde la apariencia de fragata fantasmal
						Call ToogleBoatBody(UserIndex)
						Call WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!", FontTypeNames.FONTTYPE_INFO)
						Call ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading, NingunArma, NingunEscudo, NingunCasco)
					End If
				Else
					If .flags.invisible = 0 Then
						Call UsUaRiOs.SetInvisible(UserIndex, UserList(UserIndex).Char_Renamed.CharIndex, False)
						Call WriteConsoleMsg(UserIndex, "¡Has vuelto a ser visible!", FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			End If
			
			If migr_LenB(Chat) <> 0 Then
				'Analize chat...
				Call Statistics.ParseChat(Chat)
				
				If Not (.flags.AdminInvisible = 1) Then
					If .flags.Muerto = 1 Then
						Call SendData(modSendData.SendTarget.ToDeadArea, UserIndex, PrepareMessageChatOverHead(Chat, .Char_Renamed.CharIndex, CHAT_COLOR_DEAD_CHAR))
					Else
						Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageChatOverHead(Chat, .Char_Renamed.CharIndex, .flags.ChatColor))
					End If
				Else
					If RTrim(Chat) <> "" Then
						Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageConsoleMsg("Gm> " & Chat, FontTypeNames.FONTTYPE_GM))
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "Yell" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleYell(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 13/01/2010 (ZaMa)
		'15/07/2009: ZaMa - Now invisible admins yell by console.
		'13/01/2010: ZaMa - Now hidden on boat pirats recover the proper boat body.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim Chat As String
		With UserList(UserIndex)
			
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			Chat = buffer.ReadASCIIString()
			
			
			'[Consejeros & GMs]
			If .flags.Privilegios And (Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then
				Call LogGM(.name, "Grito: " & Chat)
			End If
			
			'I see you....
			If .flags.Oculto > 0 Then
				.flags.Oculto = 0
				.Counters.TiempoOculto = 0
				
				If .flags.Navegando = 1 Then
					If .clase = Declaraciones.eClass.Pirat Then
						' Pierde la apariencia de fragata fantasmal
						Call ToogleBoatBody(UserIndex)
						Call WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!", FontTypeNames.FONTTYPE_INFO)
						Call ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading, NingunArma, NingunEscudo, NingunCasco)
					End If
				Else
					If .flags.invisible = 0 Then
						Call UsUaRiOs.SetInvisible(UserIndex, .Char_Renamed.CharIndex, False)
						Call WriteConsoleMsg(UserIndex, "¡Has vuelto a ser visible!", FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			End If
			
			If migr_LenB(Chat) <> 0 Then
				'Analize chat...
				Call Statistics.ParseChat(Chat)
				
				If .flags.Privilegios And Declaraciones.PlayerType.User Then
					If UserList(UserIndex).flags.Muerto = 1 Then
						Call SendData(modSendData.SendTarget.ToDeadArea, UserIndex, PrepareMessageChatOverHead(Chat, .Char_Renamed.CharIndex, CHAT_COLOR_DEAD_CHAR))
					Else
						Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageChatOverHead(Chat, .Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red)))
					End If
				Else
					If Not (.flags.AdminInvisible = 1) Then
						Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageChatOverHead(Chat, .Char_Renamed.CharIndex, CHAT_COLOR_GM_YELL))
					Else
						Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageConsoleMsg("Gm> " & Chat, FontTypeNames.FONTTYPE_GM))
					End If
				End If
			End If
			
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "Whisper" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleWhisper(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 15/07/2009
		'28/05/2009: ZaMa - Now it doesn't appear any message when private talking to an invisible admin
		'15/07/2009: ZaMa - Now invisible admins wisper by console.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim Chat As String
		Dim targetCharIndex As Short
		Dim TargetUserIndex As Short
		Dim targetPriv As Declaraciones.PlayerType
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			targetCharIndex = buffer.ReadInteger()
			Chat = buffer.ReadASCIIString()
			
			TargetUserIndex = CharIndexToUserIndex(targetCharIndex)
			
			If .flags.Muerto Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Los muertos no pueden comunicarse con el mundo de los vivos. ", FontTypeNames.FONTTYPE_INFO)
			Else
				If TargetUserIndex = INVALID_INDEX Then
					Call WriteConsoleMsg(UserIndex, "Usuario inexistente.", FontTypeNames.FONTTYPE_INFO)
				Else
					targetPriv = UserList(TargetUserIndex).flags.Privilegios
					'A los dioses y admins no vale susurrarles si no sos uno vos mismo (así no pueden ver si están conectados o no)
					If (targetPriv And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin)) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios)) <> 0 Then
						' Controlamos que no este invisible
						If UserList(TargetUserIndex).flags.AdminInvisible <> 1 Then
							Call WriteConsoleMsg(UserIndex, "No puedes susurrarle a los Dioses y Admins.", FontTypeNames.FONTTYPE_INFO)
						End If
						'A los Consejeros y SemiDioses no vale susurrarles si sos un PJ común.
					ElseIf (.flags.Privilegios And Declaraciones.PlayerType.User) <> 0 And (Not targetPriv And Declaraciones.PlayerType.User) <> 0 Then 
						' Controlamos que no este invisible
						If UserList(TargetUserIndex).flags.AdminInvisible <> 1 Then
							Call WriteConsoleMsg(UserIndex, "No puedes susurrarle a los GMs.", FontTypeNames.FONTTYPE_INFO)
						End If
					ElseIf Not EstaPCarea(UserIndex, TargetUserIndex) Then 
						Call WriteConsoleMsg(UserIndex, "Estás muy lejos del usuario.", FontTypeNames.FONTTYPE_INFO)
						
					Else
						'[Consejeros & GMs]
						If .flags.Privilegios And (Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then
							Call LogGM(.name, "Le dijo a '" & UserList(TargetUserIndex).name & "' " & Chat)
						End If
						
						If migr_LenB(Chat) <> 0 Then
							'Analize chat...
							Call Statistics.ParseChat(Chat)
							
							If Not (.flags.AdminInvisible = 1) Then
								Call WriteChatOverHead(UserIndex, Chat, .Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue))
								Call WriteChatOverHead(TargetUserIndex, Chat, .Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue))
								Call FlushBuffer(TargetUserIndex)
								
								'[CDT 17-02-2004]
								If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero) Then
									Call SendData(modSendData.SendTarget.ToAdminsAreaButConsejeros, UserIndex, PrepareMessageChatOverHead("A " & UserList(TargetUserIndex).name & "> " & Chat, .Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow)))
								End If
							Else
								Call WriteConsoleMsg(UserIndex, "Susurraste> " & Chat, FontTypeNames.FONTTYPE_GM)
								If UserIndex <> TargetUserIndex Then Call WriteConsoleMsg(TargetUserIndex, "Gm susurra> " & Chat, FontTypeNames.FONTTYPE_GM)
								
								If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero) Then
									Call SendData(modSendData.SendTarget.ToAdminsAreaButConsejeros, UserIndex, PrepareMessageConsoleMsg("Gm dijo a " & UserList(TargetUserIndex).name & "> " & Chat, FontTypeNames.FONTTYPE_GM))
								End If
							End If
						End If
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "Walk" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleWalk(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 13/01/2010 (ZaMa)
		'11/19/09 Pato - Now the class bandit can walk hidden.
		'13/01/2010: ZaMa - Now hidden on boat pirats recover the proper boat body.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim dummy As Integer
		Dim TempTick As Integer
		Dim heading As Declaraciones.eHeading
		
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			heading = .incomingData.ReadByte()
			
			'Prevent SpeedHack
			If .flags.TimesWalk >= 30 Then
				TempTick = GetTickCount()
				dummy = (TempTick - .flags.StartWalk)

				' 5800 is actually less than what would be needed in perfect conditions to take 30 steps
				'(it's about 193 ms per step against the over 200 needed in perfect conditions)
				If dummy < 5800 Then
					If TempTick - .flags.CountSH > 30000 Then
						.flags.CountSH = 0
					End If

					If Not .flags.CountSH = 0 Then
						If dummy <> 0 Then dummy = 126000 \ dummy

						Call LogHackAttemp("Tramposo SH: " & .name & " , " & dummy)
						Call SendData(modSendData.SendTarget.ToAdmins, 0, PrepareMessageConsoleMsg("Servidor> " & .name & " ha sido echado por el servidor por posible uso de SH.", FontTypeNames.FONTTYPE_SERVER))
						Call CloseSocket(UserIndex)

						Exit Sub
					Else
						.flags.CountSH = TempTick
					End If
				End If
				.flags.StartWalk = TempTick
				.flags.TimesWalk = 0
			End If

			.flags.TimesWalk = .flags.TimesWalk + 1

			'If exiting, cancel
			Call CancelExit(UserIndex)

			'TODO: Debería decirle por consola que no puede?
			'Esta usando el /HOGAR, no se puede mover
			If .flags.Traveling = 1 Then Exit Sub

			If .flags.Paralizado = 0 Then
				If .flags.Meditando Then
					'Stop meditating, next action will start movement.
					.flags.Meditando = False
					.Char_Renamed.FX = 0
					.Char_Renamed.loops = 0

					Call WriteMeditateToggle(UserIndex)
					Call WriteConsoleMsg(UserIndex, "Dejas de meditar.", FontTypeNames.FONTTYPE_INFO)

					Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageCreateFX(.Char_Renamed.CharIndex, 0, 0))
				Else
					'Move user
					Call MoveUserChar(UserIndex, heading)

					'Stop resting if needed
					If .flags.Descansar Then
						.flags.Descansar = False

						Call WriteRestOK(UserIndex)
						Call WriteConsoleMsg(UserIndex, "Has dejado de descansar.", FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			Else 'paralized
				If Not .flags.UltimoMensaje = 1 Then
					.flags.UltimoMensaje = 1

					Call WriteConsoleMsg(UserIndex, "No puedes moverte porque estás paralizado.", FontTypeNames.FONTTYPE_INFO)
				End If

				.flags.CountSH = 0
			End If

			'Can't move while hidden except he is a thief
			If .flags.Oculto = 1 And .flags.AdminInvisible = 0 Then
				If .clase <> Declaraciones.eClass.Thief And .clase <> Declaraciones.eClass.Bandit Then
					.flags.Oculto = 0
					.Counters.TiempoOculto = 0

					If .flags.Navegando = 1 Then
						If .clase = Declaraciones.eClass.Pirat Then
							' Pierde la apariencia de fragata fantasmal
							Call ToogleBoatBody(UserIndex)
							Call WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!", FontTypeNames.FONTTYPE_INFO)
							Call ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading, NingunArma, NingunEscudo, NingunCasco)
						End If
					Else
						'If not under a spell effect, show char
						If .flags.invisible = 0 Then
							Call WriteConsoleMsg(UserIndex, "Has vuelto a ser visible.", FontTypeNames.FONTTYPE_INFO)
							Call UsUaRiOs.SetInvisible(UserIndex, .Char_Renamed.CharIndex, False)
						End If
					End If
				End If
			End If
		End With
	End Sub

	''
	' Handles the "RequestPositionUpdate" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleRequestPositionUpdate(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		UserList(UserIndex).incomingData.ReadByte()

		Call WritePosUpdate(UserIndex)
	End Sub

	''
	' Handles the "Attack" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleAttack(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 13/01/2010
		'Last Modified By: ZaMa
		'10/01/2008: Tavo - Se cancela la salida del juego si el user esta saliendo.
		'13/11/2009: ZaMa - Se cancela el estado no atacable al atcar.
		'13/01/2010: ZaMa - Now hidden on boat pirats recover the proper boat body.
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'If dead, can't attack
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'If user meditates, can't attack
			If .flags.Meditando Then
				Exit Sub
			End If

			'If equiped weapon is ranged, can't attack this way
			If .Invent.WeaponEqpObjIndex > 0 Then
				If ObjData_Renamed(.Invent.WeaponEqpObjIndex).proyectil = 1 Then
					Call WriteConsoleMsg(UserIndex, "No puedes usar así este arma.", FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If
			End If

			'If exiting, cancel
			Call CancelExit(UserIndex)

			'Attack!
			Call UsuarioAtaca(UserIndex)

			'Now you can be atacked
			.flags.NoPuedeSerAtacado = False

			'I see you...
			If .flags.Oculto > 0 And .flags.AdminInvisible = 0 Then
				.flags.Oculto = 0
				.Counters.TiempoOculto = 0

				If .flags.Navegando = 1 Then
					If .clase = Declaraciones.eClass.Pirat Then
						' Pierde la apariencia de fragata fantasmal
						Call ToogleBoatBody(UserIndex)
						Call WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!", FontTypeNames.FONTTYPE_INFO)
						Call ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading, NingunArma, NingunEscudo, NingunCasco)
					End If
				Else
					If .flags.invisible = 0 Then
						Call UsUaRiOs.SetInvisible(UserIndex, .Char_Renamed.CharIndex, False)
						Call WriteConsoleMsg(UserIndex, "¡Has vuelto a ser visible!", FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			End If
		End With
	End Sub

	''
	' Handles the "PickUp" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandlePickUp(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 07/25/09
		'02/26/2006: Marco - Agregué un checkeo por si el usuario trata de agarrar un item mientras comercia.
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'If dead, it can't pick up objects
			If .flags.Muerto = 1 Then Exit Sub

			'If user is trading items and attempts to pickup an item, he's cheating, so we kick him.
			If .flags.Comerciando Then Exit Sub

			'Lower rank administrators can't pick up items
			If .flags.Privilegios And Declaraciones.PlayerType.Consejero Then
				If Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster Then
					Call WriteConsoleMsg(UserIndex, "No puedes tomar ningún objeto.", FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If
			End If

			Call GetObj(UserIndex)
		End With
	End Sub

	''
	' Handles the "SafeToggle" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleSafeToggle(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			If .flags.Seguro Then
				Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.SafeModeOff) 'Call WriteSafeModeOff(UserIndex)
			Else
				Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.SafeModeOn) 'Call WriteSafeModeOn(UserIndex)
			End If

			.flags.Seguro = Not .flags.Seguro
		End With
	End Sub

	''
	' Handles the "ResuscitationSafeToggle" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleResuscitationToggle(ByVal UserIndex As Short)
		'***************************************************
		'Author: Rapsodius
		'Creation Date: 10/10/07
		'***************************************************
		With UserList(UserIndex)
			Call .incomingData.ReadByte()

			.flags.SeguroResu = Not .flags.SeguroResu

			If .flags.SeguroResu Then
				Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.ResuscitationSafeOn) 'Call WriteResuscitationSafeOn(UserIndex)
			Else
				Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.ResuscitationSafeOff) 'Call WriteResuscitationSafeOff(UserIndex)
			End If
		End With
	End Sub

	''
	' Handles the "RequestGuildLeaderInfo" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleRequestGuildLeaderInfo(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		UserList(UserIndex).incomingData.ReadByte()

		Call modGuilds.SendGuildLeaderInfo(UserIndex)
	End Sub

	''
	' Handles the "RequestAtributes" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleRequestAtributes(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()

		Call WriteAttributes(UserIndex)
	End Sub

	''
	' Handles the "RequestFame" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleRequestFame(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()

		Call EnviarFama(UserIndex)
	End Sub

	''
	' Handles the "RequestSkills" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleRequestSkills(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()

		Call WriteSendSkills(UserIndex)
	End Sub

	''
	' Handles the "RequestMiniStats" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleRequestMiniStats(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()

		Call WriteMiniStats(UserIndex)
	End Sub

	''
	' Handles the "CommerceEnd" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleCommerceEnd(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()

		'User quits commerce mode
		UserList(UserIndex).flags.Comerciando = False
		Call WriteCommerceEnd(UserIndex)
	End Sub

	''
	' Handles the "UserCommerceEnd" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleUserCommerceEnd(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 11/03/2010
		'11/03/2010: ZaMa - Le avisa por consola al que cencela que dejo de comerciar.
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'Quits commerce mode with user
			If .ComUsu.DestUsu > 0 Then
				If UserList(.ComUsu.DestUsu).ComUsu.DestUsu = UserIndex Then
					Call WriteConsoleMsg(.ComUsu.DestUsu, .name & " ha dejado de comerciar con vos.", FontTypeNames.FONTTYPE_TALK)
					Call FinComerciarUsu(.ComUsu.DestUsu)

					'Send data in the outgoing buffer of the other user
					Call FlushBuffer(.ComUsu.DestUsu)
				End If
			End If

			Call FinComerciarUsu(UserIndex)
			Call WriteConsoleMsg(UserIndex, "Has dejado de comerciar.", FontTypeNames.FONTTYPE_TALK)
		End With
	End Sub

	''
	' Handles the "UserCommerceConfirm" message.
	'
	' @param    userIndex The index of the user sending the message.
	Private Sub HandleUserCommerceConfirm(ByVal UserIndex As Short)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 14/12/2009
		'
		'***************************************************

		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()

		'Validate the commerce
		If PuedeSeguirComerciando(UserIndex) Then
			'Tell the other user the confirmation of the offer
			Call WriteUserOfferConfirm(UserList(UserIndex).ComUsu.DestUsu)
			UserList(UserIndex).ComUsu.Confirmo = True
		End If

	End Sub

	Private Sub HandleCommerceChat(ByVal UserIndex As Short)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 03/12/2009
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim Chat As String
		With UserList(UserIndex)

			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			Chat = buffer.ReadASCIIString()

			If migr_LenB(Chat) <> 0 Then
				If PuedeSeguirComerciando(UserIndex) Then
					'Analize chat...
					Call Statistics.ParseChat(Chat)

					Chat = UserList(UserIndex).name & "> " & Chat
					Call WriteCommerceChat(UserIndex, Chat, FontTypeNames.FONTTYPE_PARTY)
					Call WriteCommerceChat(UserList(UserIndex).ComUsu.DestUsu, Chat, FontTypeNames.FONTTYPE_PARTY)
				End If
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub


	''
	' Handles the "BankEnd" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleBankEnd(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'User exits banking mode
			.flags.Comerciando = False
			Call WriteBankEnd(UserIndex)
		End With
	End Sub

	''
	' Handles the "UserCommerceOk" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleUserCommerceOk(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()

		'Trade accepted
		Call AceptarComercioUsu(UserIndex)
	End Sub

	''
	' Handles the "UserCommerceReject" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleUserCommerceReject(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		Dim otherUser As Short

		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			otherUser = .ComUsu.DestUsu

			'Offer rejected
			If otherUser > 0 Then
				If UserList(otherUser).flags.UserLogged Then
					Call WriteConsoleMsg(otherUser, .name & " ha rechazado tu oferta.", FontTypeNames.FONTTYPE_TALK)
					Call FinComerciarUsu(otherUser)

					'Send data in the outgoing buffer of the other user
					Call FlushBuffer(otherUser)
				End If
			End If

			Call WriteConsoleMsg(UserIndex, "Has rechazado la oferta del otro usuario.", FontTypeNames.FONTTYPE_TALK)
			Call FinComerciarUsu(UserIndex)
		End With
	End Sub

	''
	' Handles the "Drop" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleDrop(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 07/25/09
		'07/25/09: Marco - Agregué un checkeo para patear a los usuarios que tiran items mientras comercian.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 4 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim Slot As Byte
		Dim Amount As Short

		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			Slot = .incomingData.ReadByte()
			Amount = .incomingData.ReadInteger()


			'low rank admins can't drop item. Neither can the dead nor those sailing.
			If .flags.Navegando = 1 Or .flags.Muerto = 1 Or ((.flags.Privilegios And Declaraciones.PlayerType.Consejero) <> 0 And (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0) Then Exit Sub

			'If the user is trading, he can't drop items => He's cheating, we kick him.
			If .flags.Comerciando Then Exit Sub

			'Are we dropping gold or other items??
			If Slot = FLAGORO Then
				If Amount > 10000 Then Exit Sub 'Don't drop too much gold

				Call TirarOro(Amount, UserIndex)

				Call WriteUpdateGold(UserIndex)
			Else
				'Only drop valid slots
				If Slot <= MAX_INVENTORY_SLOTS And Slot > 0 Then
					If .Invent.Object_Renamed(Slot).ObjIndex = 0 Then
						Exit Sub
					End If

					Call DropObj(UserIndex, Slot, Amount, .Pos.Map, .Pos.X, .Pos.Y)
				End If
			End If
		End With
	End Sub

	''
	' Handles the "CastSpell" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleCastSpell(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'13/11/2009: ZaMa - Ahora los npcs pueden atacar al usuario si quizo castear un hechizo
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim Spell As Byte
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()


			Spell = .incomingData.ReadByte()

			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Now you can be atacked
			.flags.NoPuedeSerAtacado = False

			If Spell < 1 Then
				.flags.Hechizo = 0
				Exit Sub
			ElseIf Spell > MAXUSERHECHIZOS Then
				.flags.Hechizo = 0
				Exit Sub
			End If

			.flags.Hechizo = .Stats.UserHechizos(Spell)
		End With
	End Sub

	''
	' Handles the "LeftClick" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleLeftClick(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim X As Byte
		Dim Y As Byte
		With UserList(UserIndex).incomingData
			'Remove packet ID
			Call .ReadByte()


			X = .ReadByte()
			Y = .ReadByte()

			Call LookatTile(UserIndex, UserList(UserIndex).Pos.Map, X, Y)
		End With
	End Sub

	''
	' Handles the "DoubleClick" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleDoubleClick(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim X As Byte
		Dim Y As Byte
		With UserList(UserIndex).incomingData
			'Remove packet ID
			Call .ReadByte()


			X = .ReadByte()
			Y = .ReadByte()

			Call Accion(UserIndex, UserList(UserIndex).Pos.Map, X, Y)
		End With
	End Sub

	''
	' Handles the "Work" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleWork(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 13/01/2010 (ZaMa)
		'13/01/2010: ZaMa - El pirata se puede ocultar en barca
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim Skill As Declaraciones.eSkill
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()


			Skill = .incomingData.ReadByte()

			If UserList(UserIndex).flags.Muerto = 1 Then Exit Sub

			'If exiting, cancel
			Call CancelExit(UserIndex)

			Select Case Skill
				Case Declaraciones.eSkill.Robar, Declaraciones.eSkill.Magia, Declaraciones.eSkill.Domar
					Call WriteMultiMessage(UserIndex, Declaraciones.eMessages.WorkRequestTarget, Skill) 'Call WriteWorkRequestTarget(UserIndex, Skill)
				Case Declaraciones.eSkill.Ocultarse

					If .flags.EnConsulta Then
						Call WriteConsoleMsg(UserIndex, "No puedes ocultarte si estás en consulta.", FontTypeNames.FONTTYPE_INFO)
						Exit Sub
					End If

					If .flags.Navegando = 1 Then
						If .clase <> Declaraciones.eClass.Pirat Then
							'[CDT 17-02-2004]
							If Not .flags.UltimoMensaje = 3 Then
								Call WriteConsoleMsg(UserIndex, "No puedes ocultarte si estás navegando.", FontTypeNames.FONTTYPE_INFO)
								.flags.UltimoMensaje = 3
							End If
							'[/CDT]
							Exit Sub
						End If
					End If

					If .flags.Oculto = 1 Then
						'[CDT 17-02-2004]
						If Not .flags.UltimoMensaje = 2 Then
							Call WriteConsoleMsg(UserIndex, "Ya estás oculto.", FontTypeNames.FONTTYPE_INFO)
							.flags.UltimoMensaje = 2
						End If
						'[/CDT]
						Exit Sub
					End If

					Call DoOcultarse(UserIndex)
			End Select
		End With
	End Sub

	''
	' Handles the "InitCrafting" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleInitCrafting(ByVal UserIndex As Short)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 29/01/2010
		'
		'***************************************************
		Dim TotalItems As Integer
		Dim ItemsPorCiclo As Short

		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			TotalItems = .incomingData.ReadLong
			ItemsPorCiclo = .incomingData.ReadInteger

			If TotalItems > 0 Then

				.Construir.Cantidad = TotalItems
				.Construir.PorCiclo = MinimoInt(MaxItemsConstruibles(UserIndex), ItemsPorCiclo)

			End If
		End With
	End Sub

	''
	' Handles the "UseSpellMacro" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleUseSpellMacro(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			Call SendData(modSendData.SendTarget.ToAdmins, UserIndex, PrepareMessageConsoleMsg(.name & " fue expulsado por Anti-macro de hechizos.", FontTypeNames.FONTTYPE_VENENO))
			Call WriteErrorMsg(UserIndex, "Has sido expulsado por usar macro de hechizos. Recomendamos leer el reglamento sobre el tema macros.")
			Call FlushBuffer(UserIndex)
			Call CloseSocket(UserIndex)
		End With
	End Sub

	''
	' Handles the "UseItem" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleUseItem(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim Slot As Byte
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()


			Slot = .incomingData.ReadByte()

			If Slot <= .CurrentInventorySlots And Slot > 0 Then
				If .Invent.Object_Renamed(Slot).ObjIndex = 0 Then Exit Sub
			End If

			If .flags.Meditando Then
				Exit Sub 'The error message should have been provided by the client.
			End If

			Call UseInvItem(UserIndex, Slot)
		End With
	End Sub

	''
	' Handles the "CraftBlacksmith" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleCraftBlacksmith(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim Item As Short
		With UserList(UserIndex).incomingData
			'Remove packet ID
			Call .ReadByte()


			Item = .ReadInteger()

			If Item < 1 Then Exit Sub

			If ObjData_Renamed(Item).SkHerreria = 0 Then Exit Sub

			Call HerreroConstruirItem(UserIndex, Item)
		End With
	End Sub

	''
	' Handles the "CraftCarpenter" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleCraftCarpenter(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim Item As Short
		With UserList(UserIndex).incomingData
			'Remove packet ID
			Call .ReadByte()


			Item = .ReadInteger()

			If Item < 1 Then Exit Sub

			If ObjData_Renamed(Item).SkCarpinteria = 0 Then Exit Sub

			Call CarpinteroConstruirItem(UserIndex, Item)
		End With
	End Sub

	''
	' Handles the "WorkLeftClick" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleWorkLeftClick(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 14/01/2010 (ZaMa)
		'16/11/2009: ZaMa - Agregada la posibilidad de extraer madera elfica.
		'12/01/2010: ZaMa - Ahora se admiten armas arrojadizas (proyectiles sin municiones).
		'14/01/2010: ZaMa - Ya no se pierden municiones al atacar npcs con dueño.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 4 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim X As Byte
		Dim Y As Byte
		Dim Skill As Declaraciones.eSkill
		Dim DummyInt As Short
		Dim tU As Short
		Dim tN As Short
		Dim Atacked As Boolean 'Target NPC 'Target user
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()


			X = .incomingData.ReadByte()
			Y = .incomingData.ReadByte()

			Skill = .incomingData.ReadByte()


			If .flags.Muerto = 1 Or .flags.Descansar Or .flags.Meditando Or Not InMapBounds(.Pos.Map, X, Y) Then Exit Sub

			If Not InRangoVision(UserIndex, X, Y) Then
				Call WritePosUpdate(UserIndex)
				Exit Sub
			End If

			'If exiting, cancel
			Call CancelExit(UserIndex)

			Select Case Skill
				Case Declaraciones.eSkill.Proyectiles

					'Check attack interval
					If Not IntervaloPermiteAtacar(UserIndex, False) Then Exit Sub
					'Check Magic interval
					If Not IntervaloPermiteLanzarSpell(UserIndex, False) Then Exit Sub
					'Check bow's interval
					If Not IntervaloPermiteUsarArcos(UserIndex) Then Exit Sub

					Atacked = True

					'Make sure the item is valid and there is ammo equipped.
					With .Invent
						' Tiene arma equipada?
						If .WeaponEqpObjIndex = 0 Then
							DummyInt = 1
							' En un slot válido?
						ElseIf .WeaponEqpSlot < 1 Or .WeaponEqpSlot > UserList(UserIndex).CurrentInventorySlots Then
							DummyInt = 1
							' Usa munición? (Si no la usa, puede ser un arma arrojadiza)
						ElseIf ObjData_Renamed(.WeaponEqpObjIndex).Municion = 1 Then
							' La municion esta equipada en un slot valido?
							If .MunicionEqpSlot < 1 Or .MunicionEqpSlot > UserList(UserIndex).CurrentInventorySlots Then
								DummyInt = 1
								' Tiene munición?
							ElseIf .MunicionEqpObjIndex = 0 Then
								DummyInt = 1
								' Son flechas?
							ElseIf ObjData_Renamed(.MunicionEqpObjIndex).OBJType <> Declaraciones.eOBJType.otFlechas Then
								DummyInt = 1
								' Tiene suficientes?
							ElseIf .Object_Renamed(.MunicionEqpSlot).Amount < 1 Then
								DummyInt = 1
							End If
							' Es un arma de proyectiles?
						ElseIf ObjData_Renamed(.WeaponEqpObjIndex).proyectil <> 1 Then
							DummyInt = 2
						End If

						If DummyInt <> 0 Then
							If DummyInt = 1 Then
								Call WriteConsoleMsg(UserIndex, "No tienes municiones.", FontTypeNames.FONTTYPE_INFO)

								Call Desequipar(UserIndex, .WeaponEqpSlot)
							End If

							Call Desequipar(UserIndex, .MunicionEqpSlot)
							Exit Sub
						End If
					End With

					'Quitamos stamina
					If .Stats.MinSta >= 10 Then
						Call QuitarSta(UserIndex, RandomNumber(1, 10))
					Else
						If .Genero = Declaraciones.eGenero.Hombre Then
							Call WriteConsoleMsg(UserIndex, "Estás muy cansado para luchar.", FontTypeNames.FONTTYPE_INFO)
						Else
							Call WriteConsoleMsg(UserIndex, "Estás muy cansada para luchar.", FontTypeNames.FONTTYPE_INFO)
						End If
						Exit Sub
					End If

					Call LookatTile(UserIndex, .Pos.Map, X, Y)

					tU = .flags.TargetUser
					tN = .flags.TargetNPC

					'Validate target
					If tU > 0 Then
						'Only allow to atack if the other one can retaliate (can see us)
						If System.Math.Abs(UserList(tU).Pos.Y - .Pos.Y) > RANGO_VISION_Y Then
							Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos para atacar.", FontTypeNames.FONTTYPE_WARNING)
							Exit Sub
						End If

						'Prevent from hitting self
						If tU = UserIndex Then
							Call WriteConsoleMsg(UserIndex, "¡No puedes atacarte a vos mismo!", FontTypeNames.FONTTYPE_INFO)
							Exit Sub
						End If

						'Attack!
						Atacked = UsuarioAtacaUsuario(UserIndex, tU)

					ElseIf tN > 0 Then
						'Only allow to atack if the other one can retaliate (can see us)
						If System.Math.Abs(Npclist(tN).Pos.Y - .Pos.Y) > RANGO_VISION_Y And System.Math.Abs(Npclist(tN).Pos.X - .Pos.X) > RANGO_VISION_X Then
							Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos para atacar.", FontTypeNames.FONTTYPE_WARNING)
							Exit Sub
						End If

						'Is it attackable???
						If Npclist(tN).Attackable <> 0 Then

							'Attack!
							Atacked = UsuarioAtacaNpc(UserIndex, tN)
						End If
					End If

					' Solo pierde la munición si pudo atacar al target, o tiro al aire
					If Atacked Then
						With .Invent
							' Tiene equipado arco y flecha?
							If ObjData_Renamed(.WeaponEqpObjIndex).Municion = 1 Then
								DummyInt = .MunicionEqpSlot


								'Take 1 arrow away - we do it AFTER hitting, since if Ammo Slot is 0 it gives a rt9 and kicks players
								Call QuitarUserInvItem(UserIndex, DummyInt, 1)

								If .Object_Renamed(DummyInt).Amount > 0 Then
									'QuitarUserInvItem unequips the ammo, so we equip it again
									.MunicionEqpSlot = DummyInt
									.MunicionEqpObjIndex = .Object_Renamed(DummyInt).ObjIndex
									.Object_Renamed(DummyInt).Equipped = 1
								Else
									.MunicionEqpSlot = 0
									.MunicionEqpObjIndex = 0
								End If
								' Tiene equipado un arma arrojadiza
							Else
								DummyInt = .WeaponEqpSlot

								'Take 1 knife away
								Call QuitarUserInvItem(UserIndex, DummyInt, 1)

								If .Object_Renamed(DummyInt).Amount > 0 Then
									'QuitarUserInvItem unequips the weapon, so we equip it again
									.WeaponEqpSlot = DummyInt
									.WeaponEqpObjIndex = .Object_Renamed(DummyInt).ObjIndex
									.Object_Renamed(DummyInt).Equipped = 1
								Else
									.WeaponEqpSlot = 0
									.WeaponEqpObjIndex = 0
								End If

							End If

							Call UpdateUserInv(False, UserIndex, DummyInt)
						End With
					End If

				Case Declaraciones.eSkill.Magia
					'Check the map allows spells to be casted.
					If MapInfo_Renamed(.Pos.Map).MagiaSinEfecto > 0 Then
						Call WriteConsoleMsg(UserIndex, "Una fuerza oscura te impide canalizar tu energía.", FontTypeNames.FONTTYPE_FIGHT)
						Exit Sub
					End If

					'Target whatever is in that tile
					Call LookatTile(UserIndex, .Pos.Map, X, Y)

					'If it's outside range log it and exit
					If System.Math.Abs(.Pos.X - X) > RANGO_VISION_X Or System.Math.Abs(.Pos.Y - Y) > RANGO_VISION_Y Then
						Call LogCheating("Ataque fuera de rango de " & .name & "(" & .Pos.Map & "/" & .Pos.X & "/" & .Pos.Y & ") ip: " & .ip & " a la posición (" & .Pos.Map & "/" & X & "/" & Y & ")")
						Exit Sub
					End If

					'Check bow's interval
					If Not IntervaloPermiteUsarArcos(UserIndex, False) Then Exit Sub


					'Check Spell-Hit interval
					If Not IntervaloPermiteGolpeMagia(UserIndex) Then
						'Check Magic interval
						If Not IntervaloPermiteLanzarSpell(UserIndex) Then
							Exit Sub
						End If
					End If


					'Check intervals and cast
					If .flags.Hechizo > 0 Then
						Call LanzarHechizo(.flags.Hechizo, UserIndex)
						.flags.Hechizo = 0
					Else
						Call WriteConsoleMsg(UserIndex, "¡Primero selecciona el hechizo que quieres lanzar!", FontTypeNames.FONTTYPE_INFO)
					End If

				Case Declaraciones.eSkill.Pesca
					DummyInt = .Invent.WeaponEqpObjIndex
					If DummyInt = 0 Then Exit Sub

					'Check interval
					If Not IntervaloPermiteTrabajar(UserIndex) Then Exit Sub

					'Basado en la idea de Barrin
					'Comentario por Barrin: jah, "basado", caradura ! ^^
					If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = 1 Then
						Call WriteConsoleMsg(UserIndex, "No puedes pescar desde donde te encuentras.", FontTypeNames.FONTTYPE_INFO)
						Exit Sub
					End If

					If HayAgua(.Pos.Map, X, Y) Then
						Select Case DummyInt
							Case CAÑA_PESCA
								Call DoPescar(UserIndex)

							Case RED_PESCA
								If System.Math.Abs(.Pos.X - X) + System.Math.Abs(.Pos.Y - Y) > 2 Then
									Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos para pescar.", FontTypeNames.FONTTYPE_INFO)
									Exit Sub
								End If

								Call DoPescarRed(UserIndex)

							Case Else
								Exit Sub 'Invalid item!
						End Select

						'Play sound!
						Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessagePlayWave(SND_PESCAR, .Pos.X, .Pos.Y))
					Else
						Call WriteConsoleMsg(UserIndex, "No hay agua donde pescar. Busca un lago, río o mar.", FontTypeNames.FONTTYPE_INFO)
					End If

				Case Declaraciones.eSkill.Robar
					'Does the map allow us to steal here?
					If MapInfo_Renamed(.Pos.Map).Pk Then

						'Check interval
						If Not IntervaloPermiteTrabajar(UserIndex) Then Exit Sub

						'Target whatever is in that tile
						Call LookatTile(UserIndex, UserList(UserIndex).Pos.Map, X, Y)

						tU = .flags.TargetUser

						If tU > 0 And tU <> UserIndex Then
							'Can't steal administrative players
							If UserList(tU).flags.Privilegios And Declaraciones.PlayerType.User Then
								If UserList(tU).flags.Muerto = 0 Then
									If System.Math.Abs(.Pos.X - X) + System.Math.Abs(.Pos.Y - Y) > 1 Then
										Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
										Exit Sub
									End If

									'17/09/02
									'Check the trigger
									If MapData(UserList(tU).Pos.Map, X, Y).trigger = Declaraciones.eTrigger.ZONASEGURA Then
										Call WriteConsoleMsg(UserIndex, "No puedes robar aquí.", FontTypeNames.FONTTYPE_WARNING)
										Exit Sub
									End If

									If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = Declaraciones.eTrigger.ZONASEGURA Then
										Call WriteConsoleMsg(UserIndex, "No puedes robar aquí.", FontTypeNames.FONTTYPE_WARNING)
										Exit Sub
									End If

									Call DoRobar(UserIndex, tU)
								End If
							End If
						Else
							Call WriteConsoleMsg(UserIndex, "¡No hay a quien robarle!", FontTypeNames.FONTTYPE_INFO)
						End If
					Else
						Call WriteConsoleMsg(UserIndex, "¡No puedes robar en zonas seguras!", FontTypeNames.FONTTYPE_INFO)
					End If

				Case Declaraciones.eSkill.Talar
					'Check interval
					If Not IntervaloPermiteTrabajar(UserIndex) Then Exit Sub

					If .Invent.WeaponEqpObjIndex = 0 Then
						Call WriteConsoleMsg(UserIndex, "Deberías equiparte el hacha.", FontTypeNames.FONTTYPE_INFO)
						Exit Sub
					End If

					If .Invent.WeaponEqpObjIndex <> HACHA_LEÑADOR And .Invent.WeaponEqpObjIndex <> HACHA_LEÑA_ELFICA Then
						' Podemos llegar acá si el user equipó el anillo dsp de la U y antes del click
						Exit Sub
					End If

					DummyInt = MapData(.Pos.Map, X, Y).ObjInfo.ObjIndex

					If DummyInt > 0 Then
						If System.Math.Abs(.Pos.X - X) + System.Math.Abs(.Pos.Y - Y) > 2 Then
							Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
							Exit Sub
						End If

						'Barrin 29/9/03
						If .Pos.X = X And .Pos.Y = Y Then
							Call WriteConsoleMsg(UserIndex, "No puedes talar desde allí.", FontTypeNames.FONTTYPE_INFO)
							Exit Sub
						End If

						'¿Hay un arbol donde clickeo?
						If ObjData_Renamed(DummyInt).OBJType = Declaraciones.eOBJType.otArboles And .Invent.WeaponEqpObjIndex = HACHA_LEÑADOR Then
							Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessagePlayWave(SND_TALAR, .Pos.X, .Pos.Y))
							Call DoTalar(UserIndex)
						ElseIf ObjData_Renamed(DummyInt).OBJType = Declaraciones.eOBJType.otArbolElfico And .Invent.WeaponEqpObjIndex = HACHA_LEÑA_ELFICA Then
							If .Invent.WeaponEqpObjIndex = HACHA_LEÑA_ELFICA Then
								Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessagePlayWave(SND_TALAR, .Pos.X, .Pos.Y))
								Call DoTalar(UserIndex, True)
							Else
								Call WriteConsoleMsg(UserIndex, "El hacha utilizado no es suficientemente poderosa.", FontTypeNames.FONTTYPE_INFO)
							End If
						End If
					Else
						Call WriteConsoleMsg(UserIndex, "No hay ningún árbol ahí.", FontTypeNames.FONTTYPE_INFO)
					End If

				Case Declaraciones.eSkill.Mineria
					If Not IntervaloPermiteTrabajar(UserIndex) Then Exit Sub

					If .Invent.WeaponEqpObjIndex = 0 Then Exit Sub

					If .Invent.WeaponEqpObjIndex <> PIQUETE_MINERO Then
						' Podemos llegar acá si el user equipó el anillo dsp de la U y antes del click
						Exit Sub
					End If

					'Target whatever is in the tile
					Call LookatTile(UserIndex, .Pos.Map, X, Y)

					DummyInt = MapData(.Pos.Map, X, Y).ObjInfo.ObjIndex

					If DummyInt > 0 Then
						'Check distance
						If System.Math.Abs(.Pos.X - X) + System.Math.Abs(.Pos.Y - Y) > 2 Then
							Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
							Exit Sub
						End If

						DummyInt = MapData(.Pos.Map, X, Y).ObjInfo.ObjIndex 'CHECK
						'¿Hay un yacimiento donde clickeo?
						If ObjData_Renamed(DummyInt).OBJType = Declaraciones.eOBJType.otYacimiento Then
							Call DoMineria(UserIndex)
						Else
							Call WriteConsoleMsg(UserIndex, "Ahí no hay ningún yacimiento.", FontTypeNames.FONTTYPE_INFO)
						End If
					Else
						Call WriteConsoleMsg(UserIndex, "Ahí no hay ningún yacimiento.", FontTypeNames.FONTTYPE_INFO)
					End If

				Case Declaraciones.eSkill.Domar
					'Modificado 25/11/02
					'Optimizado y solucionado el bug de la doma de
					'criaturas hostiles.

					'Target whatever is that tile
					Call LookatTile(UserIndex, .Pos.Map, X, Y)
					tN = .flags.TargetNPC

					If tN > 0 Then
						If Npclist(tN).flags.Domable > 0 Then
							If System.Math.Abs(.Pos.X - X) + System.Math.Abs(.Pos.Y - Y) > 2 Then
								Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
								Exit Sub
							End If

							If migr_LenB(Npclist(tN).flags.AttackedBy) <> 0 Then
								Call WriteConsoleMsg(UserIndex, "No puedes domar una criatura que está luchando con un jugador.", FontTypeNames.FONTTYPE_INFO)
								Exit Sub
							End If

							Call DoDomar(UserIndex, tN)
						Else
							Call WriteConsoleMsg(UserIndex, "No puedes domar a esa criatura.", FontTypeNames.FONTTYPE_INFO)
						End If
					Else
						Call WriteConsoleMsg(UserIndex, "¡No hay ninguna criatura allí!", FontTypeNames.FONTTYPE_INFO)
					End If

				Case FundirMetal 'UGLY!!! This is a constant, not a skill!!
					'Check interval
					If Not IntervaloPermiteTrabajar(UserIndex) Then Exit Sub

					'Check there is a proper item there
					If .flags.TargetObj > 0 Then
						If ObjData_Renamed(.flags.TargetObj).OBJType = Declaraciones.eOBJType.otFragua Then
							'Validate other items
							If .flags.TargetObjInvSlot < 1 Or .flags.TargetObjInvSlot > .CurrentInventorySlots Then
								Exit Sub
							End If

							''chequeamos que no se zarpe duplicando oro
							If .Invent.Object_Renamed(.flags.TargetObjInvSlot).ObjIndex <> .flags.TargetObjInvIndex Then
								If .Invent.Object_Renamed(.flags.TargetObjInvSlot).ObjIndex = 0 Or .Invent.Object_Renamed(.flags.TargetObjInvSlot).Amount = 0 Then
									Call WriteConsoleMsg(UserIndex, "No tienes más minerales.", FontTypeNames.FONTTYPE_INFO)
									Exit Sub
								End If

								''FUISTE
								Call WriteErrorMsg(UserIndex, "Has sido expulsado por el sistema anti cheats.")
								Call FlushBuffer(UserIndex)
								Call CloseSocket(UserIndex)
								Exit Sub
							End If
							If ObjData_Renamed(.flags.TargetObjInvIndex).OBJType = Declaraciones.eOBJType.otMinerales Then
								Call FundirMineral(UserIndex)
							ElseIf ObjData_Renamed(.flags.TargetObjInvIndex).OBJType = Declaraciones.eOBJType.otWeapon Then
								Call FundirArmas(UserIndex)
							End If
						Else
							Call WriteConsoleMsg(UserIndex, "Ahí no hay ninguna fragua.", FontTypeNames.FONTTYPE_INFO)
						End If
					Else
						Call WriteConsoleMsg(UserIndex, "Ahí no hay ninguna fragua.", FontTypeNames.FONTTYPE_INFO)
					End If

				Case Declaraciones.eSkill.Herreria
					'Target wehatever is in that tile
					Call LookatTile(UserIndex, .Pos.Map, X, Y)

					If .flags.TargetObj > 0 Then
						If ObjData_Renamed(.flags.TargetObj).OBJType = Declaraciones.eOBJType.otYunque Then
							Call EnivarArmasConstruibles(UserIndex)
							Call EnivarArmadurasConstruibles(UserIndex)
							Call WriteShowBlacksmithForm(UserIndex)
						Else
							Call WriteConsoleMsg(UserIndex, "Ahí no hay ningún yunque.", FontTypeNames.FONTTYPE_INFO)
						End If
					Else
						Call WriteConsoleMsg(UserIndex, "Ahí no hay ningún yunque.", FontTypeNames.FONTTYPE_INFO)
					End If
			End Select
		End With
	End Sub

	''
	' Handles the "CreateNewGuild" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleCreateNewGuild(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/11/09
		'05/11/09: Pato - Ahora se quitan los espacios del principio y del fin del nombre del clan
		'***************************************************
		If UserList(UserIndex).incomingData.length < 9 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim desc As String
		Dim GuildName As String
		Dim site As String
		Dim codex() As String
		Dim errorStr As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			desc = buffer.ReadASCIIString()
			GuildName = Trim(buffer.ReadASCIIString())
			site = buffer.ReadASCIIString()
			codex = Split(buffer.ReadASCIIString(), SEPARATOR)

			If modGuilds.CrearNuevoClan(UserIndex, desc, GuildName, site, codex, .FundandoGuildAlineacion, errorStr) Then
				Call SendData(modSendData.SendTarget.ToAll, UserIndex, PrepareMessageConsoleMsg(.name & " fundó el clan " & GuildName & " de alineación " & modGuilds.GuildAlignment(.GuildIndex) & ".", FontTypeNames.FONTTYPE_GUILD))
				Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessagePlayWave(44, NO_3D_SOUND, NO_3D_SOUND))


				'Update tag
				Call RefreshCharStatus(UserIndex)
			Else
				Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "SpellInfo" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleSpellInfo(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim spellSlot As Byte
		Dim Spell As Short
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()


			spellSlot = .incomingData.ReadByte()

			'Validate slot
			If spellSlot < 1 Or spellSlot > MAXUSERHECHIZOS Then
				Call WriteConsoleMsg(UserIndex, "¡Primero selecciona el hechizo!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Validate spell in the slot
			Spell = .Stats.UserHechizos(spellSlot)
			If Spell > 0 And Spell < NumeroHechizos + 1 Then
				With Hechizos(Spell)
					'Send information
					Call WriteConsoleMsg(UserIndex, "%%%%%%%%%%%% INFO DEL HECHIZO %%%%%%%%%%%%" & vbCrLf & "Nombre:" & .Nombre & vbCrLf & "Descripción:" & .desc & vbCrLf & "Skill requerido: " & .MinSkill & " de magia." & vbCrLf & "Maná necesario: " & .ManaRequerido & vbCrLf & "Energía necesaria: " & .StaRequerido & vbCrLf & "%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%", FontTypeNames.FONTTYPE_INFO)
				End With
			End If
		End With
	End Sub

	''
	' Handles the "EquipItem" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleEquipItem(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim itemSlot As Byte
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()


			itemSlot = .incomingData.ReadByte()

			'Dead users can't equip items
			If .flags.Muerto = 1 Then Exit Sub

			'Validate item slot
			If itemSlot > .CurrentInventorySlots Or itemSlot < 1 Then Exit Sub

			If .Invent.Object_Renamed(itemSlot).ObjIndex = 0 Then Exit Sub

			Call EquiparInvItem(UserIndex, itemSlot)
		End With
	End Sub

	''
	' Handles the "ChangeHeading" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleChangeHeading(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 06/28/2008
		'Last Modified By: NicoNZ
		' 10/01/2008: Tavo - Se cancela la salida del juego si el user esta saliendo
		' 06/28/2008: NicoNZ - Sólo se puede cambiar si está inmovilizado.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim heading As Declaraciones.eHeading
		Dim posX As Short
		Dim posY As Short
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()


			heading = .incomingData.ReadByte()

			If .flags.Paralizado = 1 And .flags.Inmovilizado = 0 Then
				Select Case heading
					Case Declaraciones.eHeading.NORTH
						posY = -1
					Case Declaraciones.eHeading.EAST
						posX = 1
					Case Declaraciones.eHeading.SOUTH
						posY = 1
					Case Declaraciones.eHeading.WEST
						posX = -1
				End Select

				If LegalPos(.Pos.Map, .Pos.X + posX, .Pos.Y + posY, CBool(.flags.Navegando), Not CBool(.flags.Navegando)) Then
					Exit Sub
				End If
			End If

			'Validate heading (VB won't say invalid cast if not a valid index like .Net languages would do... *sigh*)
			If heading > 0 And heading < 5 Then
				.Char_Renamed.heading = heading
				Call ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading, .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim, .Char_Renamed.CascoAnim)
			End If
		End With
	End Sub

	''
	' Handles the "ModifySkills" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleModifySkills(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 11/19/09
		'11/19/09: Pato - Adapting to new skills system.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 1 + NUMSKILLS Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim i As Integer
		Dim Count As Short
		'UPGRADE_WARNING: El límite inferior de la matriz points ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		Dim points(NUMSKILLS) As Byte
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()


			'Codigo para prevenir el hackeo de los skills
			'<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
			For i = 1 To NUMSKILLS
				points(i) = .incomingData.ReadByte()

				If points(i) < 0 Then
					Call LogHackAttemp(.name & " IP:" & .ip & " trató de hackear los skills.")
					.Stats.SkillPts = 0
					Call CloseSocket(UserIndex)
					Exit Sub
				End If

				Count = Count + points(i)
			Next i

			If Count > .Stats.SkillPts Then
				Call LogHackAttemp(.name & " IP:" & .ip & " trató de hackear los skills.")
				Call CloseSocket(UserIndex)
				Exit Sub
			End If
			'<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

			.Counters.AsignedSkills = MinimoInt(10, .Counters.AsignedSkills + Count)

			With .Stats
				For i = 1 To NUMSKILLS
					If points(i) > 0 Then
						.SkillPts = .SkillPts - points(i)
						.UserSkills(i) = .UserSkills(i) + points(i)

						'Client should prevent this, but just in case...
						If .UserSkills(i) > 100 Then
							.SkillPts = .SkillPts + .UserSkills(i) - 100
							.UserSkills(i) = 100
						End If

						Call CheckEluSkill(UserIndex, i, True)
					End If
				Next i
			End With
		End With
	End Sub

	''
	' Handles the "Train" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleTrain(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim SpawnedNpc As Short
		Dim PetIndex As Byte
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()


			PetIndex = .incomingData.ReadByte()

			If .flags.TargetNPC = 0 Then Exit Sub

			If Npclist(.flags.TargetNPC).NPCtype <> Declaraciones.eNPCType.Entrenador Then Exit Sub

			If Npclist(.flags.TargetNPC).Mascotas < MAXMASCOTASENTRENADOR Then
				If PetIndex > 0 And PetIndex < Npclist(.flags.TargetNPC).NroCriaturas + 1 Then
					'Create the creature
					SpawnedNpc = SpawnNpc(Npclist(.flags.TargetNPC).Criaturas(PetIndex).NpcIndex, Npclist(.flags.TargetNPC).Pos, True, False)

					If SpawnedNpc > 0 Then
						Npclist(SpawnedNpc).MaestroNpc = .flags.TargetNPC
						Npclist(.flags.TargetNPC).Mascotas = Npclist(.flags.TargetNPC).Mascotas + 1
					End If
				End If
			Else
				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageChatOverHead("No puedo traer más criaturas, mata las existentes.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)))
			End If
		End With
	End Sub

	''
	' Handles the "CommerceBuy" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleCommerceBuy(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 4 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim Slot As Byte
		Dim Amount As Short
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()


			Slot = .incomingData.ReadByte()
			Amount = .incomingData.ReadInteger()

			'Dead people can't commerce...
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'¿El target es un NPC valido?
			If .flags.TargetNPC < 1 Then Exit Sub

			'¿El NPC puede comerciar?
			If Npclist(.flags.TargetNPC).Comercia = 0 Then
				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageChatOverHead("No tengo ningún interés en comerciar.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)))
				Exit Sub
			End If

			'Only if in commerce mode....
			If Not .flags.Comerciando Then
				Call WriteConsoleMsg(UserIndex, "No estás comerciando.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'User compra el item
			Call Comercio(modSistemaComercio.eModoComercio.Compra, UserIndex, .flags.TargetNPC, Slot, Amount)
		End With
	End Sub

	''
	' Handles the "BankExtractItem" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleBankExtractItem(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 4 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim Slot As Byte
		Dim Amount As Short
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()


			Slot = .incomingData.ReadByte()
			Amount = .incomingData.ReadInteger()

			'Dead people can't commerce
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'¿El target es un NPC valido?
			If .flags.TargetNPC < 1 Then Exit Sub

			'¿Es el banquero?
			If Npclist(.flags.TargetNPC).NPCtype <> Declaraciones.eNPCType.Banquero Then
				Exit Sub
			End If

			'User retira el item del slot
			Call UserRetiraItem(UserIndex, Slot, Amount)
		End With
	End Sub

	''
	' Handles the "CommerceSell" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleCommerceSell(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 4 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim Slot As Byte
		Dim Amount As Short
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()


			Slot = .incomingData.ReadByte()
			Amount = .incomingData.ReadInteger()

			'Dead people can't commerce...
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'¿El target es un NPC valido?
			If .flags.TargetNPC < 1 Then Exit Sub

			'¿El NPC puede comerciar?
			If Npclist(.flags.TargetNPC).Comercia = 0 Then
				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageChatOverHead("No tengo ningún interés en comerciar.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)))
				Exit Sub
			End If

			'User compra el item del slot
			Call Comercio(modSistemaComercio.eModoComercio.Venta, UserIndex, .flags.TargetNPC, Slot, Amount)
		End With
	End Sub

	''
	' Handles the "BankDeposit" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleBankDeposit(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 4 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim Slot As Byte
		Dim Amount As Short
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()


			Slot = .incomingData.ReadByte()
			Amount = .incomingData.ReadInteger()

			'Dead people can't commerce...
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'¿El target es un NPC valido?
			If .flags.TargetNPC < 1 Then Exit Sub

			'¿El NPC puede comerciar?
			If Npclist(.flags.TargetNPC).NPCtype <> Declaraciones.eNPCType.Banquero Then
				Exit Sub
			End If

			'User deposita el item del slot rdata
			Call UserDepositaItem(UserIndex, Slot, Amount)
		End With
	End Sub

	''
	' Handles the "ForumPost" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleForumPost(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 02/01/2010
		'02/01/2010: ZaMa - Implemento nuevo sistema de foros
		'***************************************************
		If UserList(UserIndex).incomingData.length < 6 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim ForumMsgType As Declaraciones.eForumMsgType
		Dim file As String
		Dim Title As String
		Dim Post As String
		Dim ForumIndex As Short
		Dim postFile As String
		Dim ForumType As Byte
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()



			ForumMsgType = buffer.ReadByte()

			Title = buffer.ReadASCIIString()
			Post = buffer.ReadASCIIString()

			If .flags.TargetObj > 0 Then
				ForumType = ForumAlignment(ForumMsgType)

				Select Case ForumType

					Case Declaraciones.eForumType.ieGeneral
						ForumIndex = GetForumIndex(ObjData_Renamed(.flags.TargetObj).ForoID)

					Case Declaraciones.eForumType.ieREAL
						ForumIndex = GetForumIndex(FORO_REAL_ID)

					Case Declaraciones.eForumType.ieCAOS
						ForumIndex = GetForumIndex(FORO_CAOS_ID)

				End Select

				Call AddPost(ForumIndex, Post, .name, Title, EsAnuncio(ForumMsgType))
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "MoveSpell" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleMoveSpell(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		'UPGRADE_NOTE: dir se actualizó a dir_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim dir_Renamed As Short
		With UserList(UserIndex).incomingData
			'Remove packet ID
			Call .ReadByte()


			If .ReadBoolean() Then
				dir_Renamed = 1
			Else
				dir_Renamed = -1
			End If

			Call DesplazarHechizo(UserIndex, dir_Renamed, .ReadByte())
		End With
	End Sub

	''
	' Handles the "MoveBank" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleMoveBank(ByVal UserIndex As Short)
		'***************************************************
		'Author: Torres Patricio (Pato)
		'Last Modification: 06/14/09
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim Slot As Byte
		Dim TempItem As Obj
		'UPGRADE_NOTE: dir se actualizó a dir_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim dir_Renamed As Short
		With UserList(UserIndex).incomingData
			'Remove packet ID
			Call .ReadByte()


			If .ReadBoolean() Then
				dir_Renamed = 1
			Else
				dir_Renamed = -1
			End If

			Slot = .ReadByte()
		End With

		With UserList(UserIndex)
			TempItem.ObjIndex = .BancoInvent.Object_Renamed(Slot).ObjIndex
			TempItem.Amount = .BancoInvent.Object_Renamed(Slot).Amount

			If dir_Renamed = 1 Then 'Mover arriba
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().BancoInvent.Object(Slot). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				.BancoInvent.Object_Renamed(Slot) = .BancoInvent.Object_Renamed(Slot - 1)
				.BancoInvent.Object_Renamed(Slot - 1).ObjIndex = TempItem.ObjIndex
				.BancoInvent.Object_Renamed(Slot - 1).Amount = TempItem.Amount
			Else 'mover abajo
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().BancoInvent.Object(Slot). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				.BancoInvent.Object_Renamed(Slot) = .BancoInvent.Object_Renamed(Slot + 1)
				.BancoInvent.Object_Renamed(Slot + 1).ObjIndex = TempItem.ObjIndex
				.BancoInvent.Object_Renamed(Slot + 1).Amount = TempItem.Amount
			End If
		End With

		Call UpdateBanUserInv(True, UserIndex, 0)
		Call UpdateVentanaBanco(UserIndex)

	End Sub

	''
	' Handles the "ClanCodexUpdate" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleClanCodexUpdate(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim desc As String
		Dim codex() As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			desc = buffer.ReadASCIIString()
			codex = Split(buffer.ReadASCIIString(), SEPARATOR)

			Call modGuilds.ChangeCodexAndDesc(desc, codex, .GuildIndex)

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "UserCommerceOffer" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleUserCommerceOffer(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 24/11/2009
		'24/11/2009: ZaMa - Nuevo sistema de comercio
		'***************************************************
		If UserList(UserIndex).incomingData.length < 7 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		Dim Amount As Integer
		Dim Slot As Byte
		Dim tUser As Short
		Dim OfferSlot As Byte
		Dim ObjIndex As Short
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()


			Slot = .incomingData.ReadByte()
			Amount = .incomingData.ReadLong()
			OfferSlot = .incomingData.ReadByte()

			'Get the other player
			tUser = .ComUsu.DestUsu

			' If he's already confirmed his offer, but now tries to change it, then he's cheating
			If UserList(UserIndex).ComUsu.Confirmo = True Then

				' Finish the trade
				Call FinComerciarUsu(UserIndex)

				If tUser <= 0 Or tUser > MaxUsers Then
					Call FinComerciarUsu(tUser)
					Call Protocol.FlushBuffer(tUser)
				End If

				Exit Sub
			End If

			'If slot is invalid and it's not gold or it's not 0 (Substracting), then ignore it.
			If ((Slot < 0 Or Slot > UserList(UserIndex).CurrentInventorySlots) And Slot <> FLAGORO) Then Exit Sub

			'If OfferSlot is invalid, then ignore it.
			If OfferSlot < 1 Or OfferSlot > MAX_OFFER_SLOTS + 1 Then Exit Sub

			' Can be negative if substracted from the offer, but never 0.
			If Amount = 0 Then Exit Sub

			'Has he got enough??
			If Slot = FLAGORO Then
				' Can't offer more than he has
				If Amount > .Stats.GLD - .ComUsu.GoldAmount Then
					Call WriteCommerceChat(UserIndex, "No tienes esa cantidad de oro para agregar a la oferta.", FontTypeNames.FONTTYPE_TALK)
					Exit Sub
				End If
			Else
				'If modifing a filled offerSlot, we already got the objIndex, then we don't need to know it
				If Slot <> 0 Then ObjIndex = .Invent.Object_Renamed(Slot).ObjIndex
				' Can't offer more than he has
				If Not TieneObjetos(ObjIndex, TotalOfferItems(ObjIndex, UserIndex) + Amount, UserIndex) Then

					Call WriteCommerceChat(UserIndex, "No tienes esa cantidad.", FontTypeNames.FONTTYPE_TALK)
					Exit Sub
				End If

				If ItemNewbie(ObjIndex) Then
					Call WriteCancelOfferItem(UserIndex, OfferSlot)
					Exit Sub
				End If

				'Don't allow to sell boats if they are equipped (you can't take them off in the water and causes trouble)
				If .flags.Navegando = 1 Then
					If .Invent.BarcoSlot = Slot Then
						Call WriteCommerceChat(UserIndex, "No puedes vender tu barco mientras lo estés usando.", FontTypeNames.FONTTYPE_TALK)
						Exit Sub
					End If
				End If

				If .Invent.MochilaEqpSlot > 0 Then
					If .Invent.MochilaEqpSlot = Slot Then
						Call WriteCommerceChat(UserIndex, "No puedes vender tu mochila mientras la estés usando.", FontTypeNames.FONTTYPE_TALK)
						Exit Sub
					End If
				End If
			End If



			Call AgregarOferta(UserIndex, OfferSlot, ObjIndex, Amount, Slot = FLAGORO)

			Call EnviarOferta(tUser, OfferSlot)
		End With
	End Sub

	''
	' Handles the "GuildAcceptPeace" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildAcceptPeace(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim guild As String
		Dim errorStr As String
		Dim otherClanIndex As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			guild = buffer.ReadASCIIString()

			otherClanIndex = CStr(modGuilds.r_AceptarPropuestaDePaz(UserIndex, guild, errorStr))

			If CDbl(otherClanIndex) = 0 Then
				Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
			Else
				Call SendData(modSendData.SendTarget.ToGuildMembers, .GuildIndex, PrepareMessageConsoleMsg("Tu clan ha firmado la paz con " & guild & ".", FontTypeNames.FONTTYPE_GUILD))
				Call SendData(modSendData.SendTarget.ToGuildMembers, CShort(otherClanIndex), PrepareMessageConsoleMsg("Tu clan ha firmado la paz con " & modGuilds.GuildName(.GuildIndex) & ".", FontTypeNames.FONTTYPE_GUILD))
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildRejectAlliance" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildRejectAlliance(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim guild As String
		Dim errorStr As String
		Dim otherClanIndex As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			guild = buffer.ReadASCIIString()

			otherClanIndex = CStr(modGuilds.r_RechazarPropuestaDeAlianza(UserIndex, guild, errorStr))

			If CDbl(otherClanIndex) = 0 Then
				Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
			Else
				Call SendData(modSendData.SendTarget.ToGuildMembers, .GuildIndex, PrepareMessageConsoleMsg("Tu clan rechazado la propuesta de alianza de " & guild, FontTypeNames.FONTTYPE_GUILD))
				Call SendData(modSendData.SendTarget.ToGuildMembers, CShort(otherClanIndex), PrepareMessageConsoleMsg(modGuilds.GuildName(.GuildIndex) & " ha rechazado nuestra propuesta de alianza con su clan.", FontTypeNames.FONTTYPE_GUILD))
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildRejectPeace" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildRejectPeace(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim guild As String
		Dim errorStr As String
		Dim otherClanIndex As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			guild = buffer.ReadASCIIString()

			otherClanIndex = CStr(modGuilds.r_RechazarPropuestaDePaz(UserIndex, guild, errorStr))

			If CDbl(otherClanIndex) = 0 Then
				Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
			Else
				Call SendData(modSendData.SendTarget.ToGuildMembers, .GuildIndex, PrepareMessageConsoleMsg("Tu clan rechazado la propuesta de paz de " & guild & ".", FontTypeNames.FONTTYPE_GUILD))
				Call SendData(modSendData.SendTarget.ToGuildMembers, CShort(otherClanIndex), PrepareMessageConsoleMsg(modGuilds.GuildName(.GuildIndex) & " ha rechazado nuestra propuesta de paz con su clan.", FontTypeNames.FONTTYPE_GUILD))
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildAcceptAlliance" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildAcceptAlliance(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim guild As String
		Dim errorStr As String
		Dim otherClanIndex As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			guild = buffer.ReadASCIIString()

			otherClanIndex = CStr(modGuilds.r_AceptarPropuestaDeAlianza(UserIndex, guild, errorStr))

			If CDbl(otherClanIndex) = 0 Then
				Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
			Else
				Call SendData(modSendData.SendTarget.ToGuildMembers, .GuildIndex, PrepareMessageConsoleMsg("Tu clan ha firmado la alianza con " & guild & ".", FontTypeNames.FONTTYPE_GUILD))
				Call SendData(modSendData.SendTarget.ToGuildMembers, CShort(otherClanIndex), PrepareMessageConsoleMsg("Tu clan ha firmado la paz con " & modGuilds.GuildName(.GuildIndex) & ".", FontTypeNames.FONTTYPE_GUILD))
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildOfferPeace" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildOfferPeace(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim guild As String
		Dim proposal As String
		Dim errorStr As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			guild = buffer.ReadASCIIString()
			proposal = buffer.ReadASCIIString()

			If modGuilds.r_ClanGeneraPropuesta(UserIndex, guild, modGuilds.RELACIONES_GUILD.PAZ, proposal, errorStr) Then
				Call WriteConsoleMsg(UserIndex, "Propuesta de paz enviada.", FontTypeNames.FONTTYPE_GUILD)
			Else
				Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildOfferAlliance" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildOfferAlliance(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim guild As String
		Dim proposal As String
		Dim errorStr As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			guild = buffer.ReadASCIIString()
			proposal = buffer.ReadASCIIString()

			If modGuilds.r_ClanGeneraPropuesta(UserIndex, guild, modGuilds.RELACIONES_GUILD.ALIADOS, proposal, errorStr) Then
				Call WriteConsoleMsg(UserIndex, "Propuesta de alianza enviada.", FontTypeNames.FONTTYPE_GUILD)
			Else
				Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildAllianceDetails" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildAllianceDetails(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim guild As String
		Dim errorStr As String
		Dim details As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			guild = buffer.ReadASCIIString()

			details = modGuilds.r_VerPropuesta(UserIndex, guild, modGuilds.RELACIONES_GUILD.ALIADOS, errorStr)

			If migr_LenB(details) = 0 Then
				Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
			Else
				Call WriteOfferDetails(UserIndex, details)
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildPeaceDetails" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildPeaceDetails(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim guild As String
		Dim errorStr As String
		Dim details As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			guild = buffer.ReadASCIIString()

			details = modGuilds.r_VerPropuesta(UserIndex, guild, modGuilds.RELACIONES_GUILD.PAZ, errorStr)

			If migr_LenB(details) = 0 Then
				Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
			Else
				Call WriteOfferDetails(UserIndex, details)
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildRequestJoinerInfo" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildRequestJoinerInfo(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim details As String
		'UPGRADE_NOTE: User se actualizó a User_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim User_Renamed As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			User_Renamed = buffer.ReadASCIIString()

			details = modGuilds.a_DetallesAspirante(UserIndex, User_Renamed)

			If migr_LenB(details) = 0 Then
				Call WriteConsoleMsg(UserIndex, "El personaje no ha mandado solicitud, o no estás habilitado para verla.", FontTypeNames.FONTTYPE_GUILD)
			Else
				Call WriteShowUserRequest(UserIndex, details)
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildAlliancePropList" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildAlliancePropList(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()

		Call WriteAlianceProposalsList(UserIndex, r_ListaDePropuestas(UserIndex, modGuilds.RELACIONES_GUILD.ALIADOS))
	End Sub

	''
	' Handles the "GuildPeacePropList" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildPeacePropList(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()

		Call WritePeaceProposalsList(UserIndex, r_ListaDePropuestas(UserIndex, modGuilds.RELACIONES_GUILD.PAZ))
	End Sub

	''
	' Handles the "GuildDeclareWar" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildDeclareWar(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim guild As String
		Dim errorStr As String
		Dim otherGuildIndex As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			guild = buffer.ReadASCIIString()

			otherGuildIndex = modGuilds.r_DeclararGuerra(UserIndex, guild, errorStr)

			If otherGuildIndex = 0 Then
				Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
			Else
				'WAR shall be!
				Call SendData(modSendData.SendTarget.ToGuildMembers, .GuildIndex, PrepareMessageConsoleMsg("TU CLAN HA ENTRADO EN GUERRA CON " & guild & ".", FontTypeNames.FONTTYPE_GUILD))
				Call SendData(modSendData.SendTarget.ToGuildMembers, otherGuildIndex, PrepareMessageConsoleMsg(modGuilds.GuildName(.GuildIndex) & " LE DECLARA LA GUERRA A TU CLAN.", FontTypeNames.FONTTYPE_GUILD))
				Call SendData(modSendData.SendTarget.ToGuildMembers, .GuildIndex, PrepareMessagePlayWave(45, NO_3D_SOUND, NO_3D_SOUND))
				Call SendData(modSendData.SendTarget.ToGuildMembers, otherGuildIndex, PrepareMessagePlayWave(45, NO_3D_SOUND, NO_3D_SOUND))
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildNewWebsite" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildNewWebsite(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()

			Call modGuilds.ActualizarWebSite(UserIndex, buffer.ReadASCIIString())

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildAcceptNewMember" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildAcceptNewMember(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim errorStr As String
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			UserName = buffer.ReadASCIIString()

			If Not modGuilds.a_AceptarAspirante(UserIndex, UserName, errorStr) Then
				Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
			Else
				tUser = NameIndex(UserName)
				If tUser > 0 Then
					Call modGuilds.m_ConectarMiembroAClan(tUser, .GuildIndex)
					Call RefreshCharStatus(tUser)
				End If

				Call SendData(modSendData.SendTarget.ToGuildMembers, .GuildIndex, PrepareMessageConsoleMsg(UserName & " ha sido aceptado como miembro del clan.", FontTypeNames.FONTTYPE_GUILD))
				Call SendData(modSendData.SendTarget.ToGuildMembers, .GuildIndex, PrepareMessagePlayWave(43, NO_3D_SOUND, NO_3D_SOUND))
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildRejectNewMember" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildRejectNewMember(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 01/08/07
		'Last Modification by: (liquid)
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim errorStr As String
		Dim UserName As String
		Dim reason As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			UserName = buffer.ReadASCIIString()
			reason = buffer.ReadASCIIString()

			If Not modGuilds.a_RechazarAspirante(UserIndex, UserName, errorStr) Then
				Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
			Else
				tUser = NameIndex(UserName)

				If tUser > 0 Then
					Call WriteConsoleMsg(tUser, errorStr & " : " & reason, FontTypeNames.FONTTYPE_GUILD)
				Else
					'hay que grabar en el char su rechazo
					Call modGuilds.a_RechazarAspiranteChar(UserName, .GuildIndex, reason)
				End If
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildKickMember" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildKickMember(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim GuildIndex As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			UserName = buffer.ReadASCIIString()

			GuildIndex = modGuilds.m_EcharMiembroDeClan(UserIndex, UserName)

			If GuildIndex > 0 Then
				Call SendData(modSendData.SendTarget.ToGuildMembers, GuildIndex, PrepareMessageConsoleMsg(UserName & " fue expulsado del clan.", FontTypeNames.FONTTYPE_GUILD))
				Call SendData(modSendData.SendTarget.ToGuildMembers, GuildIndex, PrepareMessagePlayWave(45, NO_3D_SOUND, NO_3D_SOUND))
			Else
				Call WriteConsoleMsg(UserIndex, "No puedes expulsar ese personaje del clan.", FontTypeNames.FONTTYPE_GUILD)
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildUpdateNews" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildUpdateNews(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()

			Call modGuilds.ActualizarNoticias(UserIndex, buffer.ReadASCIIString())

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildMemberInfo" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildMemberInfo(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()

			Call modGuilds.SendDetallesPersonaje(UserIndex, buffer.ReadASCIIString())

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildOpenElections" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildOpenElections(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As String
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()


			If Not modGuilds.v_AbrirElecciones(UserIndex, error_Renamed) Then
				Call WriteConsoleMsg(UserIndex, error_Renamed, FontTypeNames.FONTTYPE_GUILD)
			Else
				Call SendData(modSendData.SendTarget.ToGuildMembers, .GuildIndex, PrepareMessageConsoleMsg("¡Han comenzado las elecciones del clan! Puedes votar escribiendo /VOTO seguido del nombre del personaje, por ejemplo: /VOTO " & .name, FontTypeNames.FONTTYPE_GUILD))
			End If
		End With
	End Sub

	''
	' Handles the "GuildRequestMembership" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildRequestMembership(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim guild As String
		Dim application As String
		Dim errorStr As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()


			guild = buffer.ReadASCIIString()
			application = buffer.ReadASCIIString()

			If Not modGuilds.a_NuevoAspirante(UserIndex, guild, application, errorStr) Then
				Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
			Else
				Call WriteConsoleMsg(UserIndex, "Tu solicitud ha sido enviada. Espera prontas noticias del líder de " & guild & ".", FontTypeNames.FONTTYPE_GUILD)
			End If

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "GuildRequestDetails" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildRequestDetails(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If

		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)

			'Remove packet ID
			Call buffer.ReadByte()

			Call modGuilds.SendGuildDetails(UserIndex, buffer.ReadASCIIString())

			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With

Errhandler:
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0

		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing

		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub

	''
	' Handles the "Online" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleOnline(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		Dim i As Integer
		Dim Count As Integer

		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			For i = 1 To LastUser
				If migr_LenB(UserList(i).name) <> 0 Then
					If UserList(i).flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero) Then Count = Count + 1
				End If
			Next i

			Call WriteConsoleMsg(UserIndex, "Número de usuarios: " & CStr(Count), FontTypeNames.FONTTYPE_INFO)
		End With
	End Sub

	''
	' Handles the "Quit" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleQuit(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 04/15/2008 (NicoNZ)
		'If user is invisible, it automatically becomes
		'visible before doing the countdown to exit
		'04/15/2008 - No se reseteaban lso contadores de invi ni de ocultar. (NicoNZ)
		'***************************************************
		Dim tUser As Short
		Dim isNotVisible As Boolean

		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			If .flags.Paralizado = 1 Then
				Call WriteConsoleMsg(UserIndex, "No puedes salir estando paralizado.", FontTypeNames.FONTTYPE_WARNING)
				Exit Sub
			End If

			'exit secure commerce
			If .ComUsu.DestUsu > 0 Then
				tUser = .ComUsu.DestUsu

				If UserList(tUser).flags.UserLogged Then
					If UserList(tUser).ComUsu.DestUsu = UserIndex Then
						Call WriteConsoleMsg(tUser, "Comercio cancelado por el otro usuario.", FontTypeNames.FONTTYPE_TALK)
						Call FinComerciarUsu(tUser)
					End If
				End If

				Call WriteConsoleMsg(UserIndex, "Comercio cancelado.", FontTypeNames.FONTTYPE_TALK)
				Call FinComerciarUsu(UserIndex)
			End If

			Call Cerrar_Usuario(UserIndex)
		End With
	End Sub

	''
	' Handles the "GuildLeave" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleGuildLeave(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		Dim GuildIndex As Short

		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'obtengo el guildindex
			GuildIndex = m_EcharMiembroDeClan(UserIndex, .name)

			If GuildIndex > 0 Then
				Call WriteConsoleMsg(UserIndex, "Dejas el clan.", FontTypeNames.FONTTYPE_GUILD)
				Call SendData(modSendData.SendTarget.ToGuildMembers, GuildIndex, PrepareMessageConsoleMsg(.name & " deja el clan.", FontTypeNames.FONTTYPE_GUILD))
			Else
				Call WriteConsoleMsg(UserIndex, "Tú no puedes salir de este clan.", FontTypeNames.FONTTYPE_GUILD)
			End If
		End With
	End Sub

	''
	' Handles the "RequestAccountState" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleRequestAccountState(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		Dim earnings As Short
		Dim Percentage As Short

		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'Dead people can't check their accounts
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Validate target NPC
			If .flags.TargetNPC = 0 Then
				Call WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			If Distancia(Npclist(.flags.TargetNPC).Pos, .Pos) > 3 Then
				Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos del vendedor.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			Select Case Npclist(.flags.TargetNPC).NPCtype
				Case Declaraciones.eNPCType.Banquero
					Call WriteChatOverHead(UserIndex, "Tienes " & .Stats.Banco & " monedas de oro en tu cuenta.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))

				Case Declaraciones.eNPCType.Timbero
					If Not .flags.Privilegios And Declaraciones.PlayerType.User Then
						earnings = Apuestas.Ganancias - Apuestas.Perdidas

						If earnings >= 0 And Apuestas.Ganancias <> 0 Then
							Percentage = Int(earnings * 100 / Apuestas.Ganancias)
						End If

						If earnings < 0 And Apuestas.Perdidas <> 0 Then
							Percentage = Int(earnings * 100 / Apuestas.Perdidas)
						End If

						Call WriteConsoleMsg(UserIndex, "Entradas: " & Apuestas.Ganancias & " Salida: " & Apuestas.Perdidas & " Ganancia Neta: " & earnings & " (" & Percentage & "%) Jugadas: " & Apuestas.Jugadas, FontTypeNames.FONTTYPE_INFO)
					End If
			End Select
		End With
	End Sub

	''
	' Handles the "PetStand" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandlePetStand(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'Dead people can't use pets
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Validate target NPC
			If .flags.TargetNPC = 0 Then
				Call WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Make sure it's close enough
			If Distancia(Npclist(.flags.TargetNPC).Pos, .Pos) > 10 Then
				Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Make sure it's his pet
			If Npclist(.flags.TargetNPC).MaestroUser <> UserIndex Then Exit Sub

			'Do it!
			Npclist(.flags.TargetNPC).Movement = AI.TipoAI.ESTATICO

			Call Expresar(.flags.TargetNPC, UserIndex)
		End With
	End Sub

	''
	' Handles the "PetFollow" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandlePetFollow(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'Dead users can't use pets
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Validate target NPC
			If .flags.TargetNPC = 0 Then
				Call WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Make sure it's close enough
			If Distancia(Npclist(.flags.TargetNPC).Pos, .Pos) > 10 Then
				Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Make usre it's the user's pet
			If Npclist(.flags.TargetNPC).MaestroUser <> UserIndex Then Exit Sub

			'Do it
			Call FollowAmo(.flags.TargetNPC)

			Call Expresar(.flags.TargetNPC, UserIndex)
		End With
	End Sub


	''
	' Handles the "ReleasePet" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleReleasePet(ByVal UserIndex As Short)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 18/11/2009
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'Dead users can't use pets
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Validate target NPC
			If .flags.TargetNPC = 0 Then
				Call WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Make sure it's close enough
			If Distancia(Npclist(.flags.TargetNPC).Pos, .Pos) > 10 Then
				Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Make usre it's the user's pet
			If Npclist(.flags.TargetNPC).MaestroUser <> UserIndex Then Exit Sub

			'Do it
			Call QuitarPet(UserIndex, .flags.TargetNPC)

		End With
	End Sub

	''
	' Handles the "TrainList" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleTrainList(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'Dead users can't use pets
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Validate target NPC
			If .flags.TargetNPC = 0 Then
				Call WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Make sure it's close enough
			If Distancia(Npclist(.flags.TargetNPC).Pos, .Pos) > 10 Then
				Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Make sure it's the trainer
			If Npclist(.flags.TargetNPC).NPCtype <> Declaraciones.eNPCType.Entrenador Then Exit Sub

			Call WriteTrainerCreatureList(UserIndex, .flags.TargetNPC)
		End With
	End Sub

	''
	' Handles the "Rest" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleRest(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'Dead users can't use pets
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Solo puedes usar ítems cuando estás vivo.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			If HayOBJarea(.Pos, FOGATA) Then
				Call WriteRestOK(UserIndex)

				If Not .flags.Descansar Then
					Call WriteConsoleMsg(UserIndex, "Te acomodás junto a la fogata y comienzas a descansar.", FontTypeNames.FONTTYPE_INFO)
				Else
					Call WriteConsoleMsg(UserIndex, "Te levantas.", FontTypeNames.FONTTYPE_INFO)
				End If

				.flags.Descansar = Not .flags.Descansar
			Else
				If .flags.Descansar Then
					Call WriteRestOK(UserIndex)
					Call WriteConsoleMsg(UserIndex, "Te levantas.", FontTypeNames.FONTTYPE_INFO)

					.flags.Descansar = False
					Exit Sub
				End If

				Call WriteConsoleMsg(UserIndex, "No hay ninguna fogata junto a la cual descansar.", FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub

	''
	' Handles the "Meditate" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleMeditate(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 04/15/08 (NicoNZ)
		'Arreglé un bug que mandaba un index de la meditacion diferente
		'al que decia el server.
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'Dead users can't use pets
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Sólo puedes meditar cuando estás vivo.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Can he meditate?
			If .Stats.MaxMAN = 0 Then
				Call WriteConsoleMsg(UserIndex, "Sólo las clases mágicas conocen el arte de la meditación.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Admins don't have to wait :D
			If Not .flags.Privilegios And Declaraciones.PlayerType.User Then
				.Stats.MinMAN = .Stats.MaxMAN
				Call WriteConsoleMsg(UserIndex, "Maná restaurado.", FontTypeNames.FONTTYPE_VENENO)
				Call WriteUpdateMana(UserIndex)
				Exit Sub
			End If

			Call WriteMeditateToggle(UserIndex)

			If .flags.Meditando Then Call WriteConsoleMsg(UserIndex, "Dejas de meditar.", FontTypeNames.FONTTYPE_INFO)

			.flags.Meditando = Not .flags.Meditando

			'Barrin 3/10/03 Tiempo de inicio al meditar
			If .flags.Meditando Then
				.Counters.tInicioMeditar = GetTickCount()

				Call WriteConsoleMsg(UserIndex, "Te estás concentrando. En " & Fix(TIEMPO_INICIOMEDITAR / 1000) & " segundos comenzarás a meditar.", FontTypeNames.FONTTYPE_INFO)

				.Char_Renamed.loops = INFINITE_LOOPS

				'Show proper FX according to level
				If .Stats.ELV < 13 Then
					.Char_Renamed.FX = Declaraciones.FXIDs.FXMEDITARCHICO

				ElseIf .Stats.ELV < 25 Then
					.Char_Renamed.FX = Declaraciones.FXIDs.FXMEDITARMEDIANO

				ElseIf .Stats.ELV < 35 Then
					.Char_Renamed.FX = Declaraciones.FXIDs.FXMEDITARGRANDE

				ElseIf .Stats.ELV < 42 Then
					.Char_Renamed.FX = Declaraciones.FXIDs.FXMEDITARXGRANDE

				Else
					.Char_Renamed.FX = Declaraciones.FXIDs.FXMEDITARXXGRANDE
				End If

				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageCreateFX(.Char_Renamed.CharIndex, .Char_Renamed.FX, INFINITE_LOOPS))
			Else
				.Counters.bPuedeMeditar = False

				.Char_Renamed.FX = 0
				.Char_Renamed.loops = 0
				Call SendData(modSendData.SendTarget.ToPCArea, UserIndex, PrepareMessageCreateFX(.Char_Renamed.CharIndex, 0, 0))
			End If
		End With
	End Sub

	''
	' Handles the "Resucitate" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleResucitate(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'Se asegura que el target es un npc
			If .flags.TargetNPC = 0 Then
				Call WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Validate NPC and make sure player is dead
			If (Npclist(.flags.TargetNPC).NPCtype <> Declaraciones.eNPCType.Revividor And (Npclist(.flags.TargetNPC).NPCtype <> Declaraciones.eNPCType.ResucitadorNewbie Or Not EsNewbie(UserIndex))) Or .flags.Muerto = 0 Then Exit Sub

			'Make sure it's close enough
			If Distancia(.Pos, Npclist(.flags.TargetNPC).Pos) > 10 Then
				Call WriteConsoleMsg(UserIndex, "El sacerdote no puede resucitarte debido a que estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			Call RevivirUsuario(UserIndex)
			Call WriteConsoleMsg(UserIndex, "¡¡Has sido resucitado!!", FontTypeNames.FONTTYPE_INFO)
		End With
	End Sub

	''
	' Handles the "Consultation" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleConsultation(ByVal UserIndex As String)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 01/05/2010
		'Habilita/Deshabilita el modo consulta.
		'01/05/2010: ZaMa - Agrego validaciones.
		'***************************************************

		Dim UserConsulta As Short

		Dim UserName As String
		With UserList(CInt(UserIndex))
			'Remove packet ID
			Call .incomingData.ReadByte()

			' Comando exclusivo para gms
			If Not EsGM(CShort(UserIndex)) Then Exit Sub

			UserConsulta = .flags.TargetUser

			'Se asegura que el target es un usuario
			If UserConsulta = 0 Then
				Call WriteConsoleMsg(CShort(UserIndex), "Primero tienes que seleccionar un usuario, haz click izquierdo sobre él.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			' No podes ponerte a vos mismo en modo consulta.
			If UserConsulta = CDbl(UserIndex) Then Exit Sub

			' No podes estra en consulta con otro gm
			If EsGM(UserConsulta) Then
				Call WriteConsoleMsg(CShort(UserIndex), "No puedes iniciar el modo consulta con otro administrador.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			UserName = UserList(UserConsulta).name

			' Si ya estaba en consulta, termina la consulta
			If UserList(UserConsulta).flags.EnConsulta Then
				Call WriteConsoleMsg(CShort(UserIndex), "Has terminado el modo consulta con " & UserName & ".", FontTypeNames.FONTTYPE_INFOBOLD)
				Call WriteConsoleMsg(UserConsulta, "Has terminado el modo consulta.", FontTypeNames.FONTTYPE_INFOBOLD)
				Call LogGM(.name, "Termino consulta con " & UserName)

				UserList(UserConsulta).flags.EnConsulta = False

				' Sino la inicia
			Else
				Call WriteConsoleMsg(CShort(UserIndex), "Has iniciado el modo consulta con " & UserName & ".", FontTypeNames.FONTTYPE_INFOBOLD)
				Call WriteConsoleMsg(UserConsulta, "Has iniciado el modo consulta.", FontTypeNames.FONTTYPE_INFOBOLD)
				Call LogGM(.name, "Inicio consulta con " & UserName)

				With UserList(UserConsulta)
					.flags.EnConsulta = True

					' Pierde invi u ocu
					If .flags.invisible = 1 Or .flags.Oculto = 1 Then
						.flags.Oculto = 0
						.flags.invisible = 0
						.Counters.TiempoOculto = 0
						.Counters.Invisibilidad = 0

						Call UsUaRiOs.SetInvisible(UserConsulta, UserList(UserConsulta).Char_Renamed.CharIndex, False)
					End If
				End With
			End If

			Call UsUaRiOs.SetConsulatMode(UserConsulta)
		End With

	End Sub

	''
	' Handles the "Heal" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleHeal(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'Se asegura que el target es un npc
			If .flags.TargetNPC = 0 Then
				Call WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			If (Npclist(.flags.TargetNPC).NPCtype <> Declaraciones.eNPCType.Revividor And Npclist(.flags.TargetNPC).NPCtype <> Declaraciones.eNPCType.ResucitadorNewbie) Or .flags.Muerto <> 0 Then Exit Sub

			If Distancia(.Pos, Npclist(.flags.TargetNPC).Pos) > 10 Then
				Call WriteConsoleMsg(UserIndex, "El sacerdote no puede curarte debido a que estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			.Stats.MinHp = .Stats.MaxHp

			Call WriteUpdateHP(UserIndex)

			Call WriteConsoleMsg(UserIndex, "¡¡Has sido curado!!", FontTypeNames.FONTTYPE_INFO)
		End With
	End Sub

	''
	' Handles the "RequestStats" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleRequestStats(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()

		Call SendUserStatsTxt(UserIndex, UserIndex)
	End Sub

	''
	' Handles the "Help" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleHelp(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()

		Call SendHelp(UserIndex)
	End Sub

	''
	' Handles the "CommerceStart" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleCommerceStart(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		Dim i As Short
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'Dead people can't commerce
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Is it already in commerce mode??
			If .flags.Comerciando Then
				Call WriteConsoleMsg(UserIndex, "Ya estás comerciando.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Validate target NPC
			If .flags.TargetNPC > 0 Then
				'Does the NPC want to trade??
				If Npclist(.flags.TargetNPC).Comercia = 0 Then
					If migr_LenB(Npclist(.flags.TargetNPC).desc) <> 0 Then
						Call WriteChatOverHead(UserIndex, "No tengo ningún interés en comerciar.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
					End If

					Exit Sub
				End If

				If Distancia(Npclist(.flags.TargetNPC).Pos, .Pos) > 3 Then
					Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos del vendedor.", FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If

				'Start commerce....
				Call IniciarComercioNPC(UserIndex)
				'[Alejo]
			ElseIf .flags.TargetUser > 0 Then
				'User commerce...
				'Can he commerce??
				If .flags.Privilegios And Declaraciones.PlayerType.Consejero Then
					Call WriteConsoleMsg(UserIndex, "No puedes vender ítems.", FontTypeNames.FONTTYPE_WARNING)
					Exit Sub
				End If

				'Is the other one dead??
				If UserList(.flags.TargetUser).flags.Muerto = 1 Then
					Call WriteConsoleMsg(UserIndex, "¡¡No puedes comerciar con los muertos!!", FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If

				'Is it me??
				If .flags.TargetUser = UserIndex Then
					Call WriteConsoleMsg(UserIndex, "¡¡No puedes comerciar con vos mismo!!", FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If

				'Check distance
				If Distancia(UserList(.flags.TargetUser).Pos, .Pos) > 3 Then
					Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos del usuario.", FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If

				'Is he already trading?? is it with me or someone else??
				If UserList(.flags.TargetUser).flags.Comerciando = True And UserList(.flags.TargetUser).ComUsu.DestUsu <> UserIndex Then
					Call WriteConsoleMsg(UserIndex, "No puedes comerciar con el usuario en este momento.", FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If

				'Initialize some variables...
				.ComUsu.DestUsu = .flags.TargetUser
				.ComUsu.DestNick = UserList(.flags.TargetUser).name
				For i = 1 To MAX_OFFER_SLOTS
					.ComUsu.cant(i) = 0
					.ComUsu.Objeto(i) = 0
				Next i
				.ComUsu.GoldAmount = 0

				.ComUsu.Acepto = False
				.ComUsu.Confirmo = False

				'Rutina para comerciar con otro usuario
				Call IniciarComercioConUsuario(UserIndex, .flags.TargetUser)
			Else
				Call WriteConsoleMsg(UserIndex, "Primero haz click izquierdo sobre el personaje.", FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub

	''
	' Handles the "BankStart" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleBankStart(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'Dead people can't commerce
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			If .flags.Comerciando Then
				Call WriteConsoleMsg(UserIndex, "Ya estás comerciando.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			'Validate target NPC
			If .flags.TargetNPC > 0 Then
				If Distancia(Npclist(.flags.TargetNPC).Pos, .Pos) > 3 Then
					Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos del vendedor.", FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If

				'If it's the banker....
				If Npclist(.flags.TargetNPC).NPCtype = Declaraciones.eNPCType.Banquero Then
					Call IniciarDeposito(UserIndex)
				End If
			Else
				Call WriteConsoleMsg(UserIndex, "Primero haz click izquierdo sobre el personaje.", FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub

	''
	' Handles the "Enlist" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleEnlist(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'Validate target NPC
			If .flags.TargetNPC = 0 Then
				Call WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			If Npclist(.flags.TargetNPC).NPCtype <> Declaraciones.eNPCType.Noble Or .flags.Muerto <> 0 Then Exit Sub

			If Distancia(.Pos, Npclist(.flags.TargetNPC).Pos) > 4 Then
				Call WriteConsoleMsg(UserIndex, "Debes acercarte más.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			If Npclist(.flags.TargetNPC).flags.Faccion = 0 Then
				Call EnlistarArmadaReal(UserIndex)
			Else
				Call EnlistarCaos(UserIndex)
			End If
		End With
	End Sub

	''
	' Handles the "Information" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleInformation(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		Dim Matados As Short
		Dim NextRecom As Short
		Dim Diferencia As Short

		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'Validate target NPC
			If .flags.TargetNPC = 0 Then
				Call WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			If Npclist(.flags.TargetNPC).NPCtype <> Declaraciones.eNPCType.Noble Or .flags.Muerto <> 0 Then Exit Sub

			If Distancia(.Pos, Npclist(.flags.TargetNPC).Pos) > 4 Then
				Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If


			NextRecom = .Faccion.NextRecompensa

			If Npclist(.flags.TargetNPC).flags.Faccion = 0 Then
				If .Faccion.ArmadaReal = 0 Then
					Call WriteChatOverHead(UserIndex, "¡¡No perteneces a las tropas reales!!", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
					Exit Sub
				End If

				Matados = .Faccion.CriminalesMatados
				Diferencia = NextRecom - Matados

				If Diferencia > 0 Then
					Call WriteChatOverHead(UserIndex, "Tu deber es combatir criminales, mata " & Diferencia & " criminales más y te daré una recompensa.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
				Else
					Call WriteChatOverHead(UserIndex, "Tu deber es combatir criminales, y ya has matado los suficientes como para merecerte una recompensa.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
				End If
			Else
				If .Faccion.FuerzasCaos = 0 Then
					Call WriteChatOverHead(UserIndex, "¡¡No perteneces a la legión oscura!!", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
					Exit Sub
				End If

				Matados = .Faccion.CiudadanosMatados
				Diferencia = NextRecom - Matados

				If Diferencia > 0 Then
					Call WriteChatOverHead(UserIndex, "Tu deber es sembrar el caos y la desesperanza, mata " & Diferencia & " ciudadanos más y te daré una recompensa.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
				Else
					Call WriteChatOverHead(UserIndex, "Tu deber es sembrar el caos y la desesperanza, y creo que estás en condiciones de merecer una recompensa.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
				End If
			End If
		End With
	End Sub

	''
	' Handles the "Reward" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleReward(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()

			'Validate target NPC
			If .flags.TargetNPC = 0 Then
				Call WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			If Npclist(.flags.TargetNPC).NPCtype <> Declaraciones.eNPCType.Noble Or .flags.Muerto <> 0 Then Exit Sub

			If Distancia(.Pos, Npclist(.flags.TargetNPC).Pos) > 4 Then
				Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If

			If Npclist(.flags.TargetNPC).flags.Faccion = 0 Then
				If .Faccion.ArmadaReal = 0 Then
					Call WriteChatOverHead(UserIndex, "¡¡No perteneces a las tropas reales!!", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
					Exit Sub
				End If
				Call RecompensaArmadaReal(UserIndex)
			Else
				If .Faccion.FuerzasCaos = 0 Then
					Call WriteChatOverHead(UserIndex, "¡¡No perteneces a la legión oscura!!", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
					Exit Sub
				End If
				Call RecompensaCaos(UserIndex)
			End If
		End With
	End Sub

	''
	' Handles the "RequestMOTD" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleRequestMOTD(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()

		Call SendMOTD(UserIndex)
	End Sub

	''
	' Handles the "UpTime" message.
	'
	' @param    userIndex The index of the user sending the message.

	Private Sub HandleUpTime(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 01/10/08
		'01/10/2008 - Marcos Martinez (ByVal) - Automatic restart removed from the server along with all their assignments and varibles
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()

		Dim time As Integer
		Dim UpTimeStr As String

		'Get total time in seconds
		time = (GetTickCount() - tInicioServer) \ 1000

		'Get times in dd:hh:mm:ss format
		UpTimeStr = (time Mod 60) & " segundos."
		time = time \ 60
		
		UpTimeStr = (time Mod 60) & " minutos, " & UpTimeStr
		time = time \ 60
		
		UpTimeStr = (time Mod 24) & " horas, " & UpTimeStr
		time = time \ 24
		
		If time = 1 Then
			UpTimeStr = time & " día, " & UpTimeStr
		Else
			UpTimeStr = time & " días, " & UpTimeStr
		End If
		
		Call WriteConsoleMsg(UserIndex, "Server Online: " & UpTimeStr, FontTypeNames.FONTTYPE_INFO)
	End Sub
	
	''
	' Handles the "PartyLeave" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandlePartyLeave(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()
		
		Call mdParty.SalirDeParty(UserIndex)
	End Sub
	
	''
	' Handles the "PartyCreate" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandlePartyCreate(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()
		
		If Not mdParty.PuedeCrearParty(UserIndex) Then Exit Sub
		
		Call mdParty.CrearParty(UserIndex)
	End Sub
	
	''
	' Handles the "PartyJoin" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandlePartyJoin(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()
		
		Call mdParty.SolicitarIngresoAParty(UserIndex)
	End Sub
	
	''
	' Handles the "ShareNpc" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleShareNpc(ByVal UserIndex As Short)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 15/04/2010
		'Shares owned npcs with other user
		'***************************************************
		
		Dim TargetUserIndex As Short
		Dim SharingUserIndex As Short
		
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			' Didn't target any user
			TargetUserIndex = .flags.TargetUser
			If TargetUserIndex = 0 Then Exit Sub
			
			' Can't share with admins
			If EsGM(TargetUserIndex) Then
				Call WriteConsoleMsg(UserIndex, "No puedes compartir npcs con administradores!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If
			
			' Pk or Caos?
			If criminal(UserIndex) Then
				' Caos can only share with other caos
				If esCaos(UserIndex) Then
					If Not esCaos(TargetUserIndex) Then
						Call WriteConsoleMsg(UserIndex, "Solo puedes compartir npcs con miembros de tu misma facción!!", FontTypeNames.FONTTYPE_INFO)
						Exit Sub
					End If
					
					' Pks don't need to share with anyone
				Else
					Exit Sub
				End If
				
				' Ciuda or Army?
			Else
				' Can't share
				If criminal(TargetUserIndex) Then
					Call WriteConsoleMsg(UserIndex, "No puedes compartir npcs con criminales!!", FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If
			End If
			
			' Already sharing with target
			SharingUserIndex = .flags.ShareNpcWith
			If SharingUserIndex = TargetUserIndex Then Exit Sub
			
			' Aviso al usuario anterior que dejo de compartir
			If SharingUserIndex <> 0 Then
				Call WriteConsoleMsg(SharingUserIndex, .name & " ha dejado de compartir sus npcs contigo.", FontTypeNames.FONTTYPE_INFO)
				Call WriteConsoleMsg(UserIndex, "Has dejado de compartir tus npcs con " & UserList(SharingUserIndex).name & ".", FontTypeNames.FONTTYPE_INFO)
			End If
			
			.flags.ShareNpcWith = TargetUserIndex
			
			Call WriteConsoleMsg(TargetUserIndex, .name & " ahora comparte sus npcs contigo.", FontTypeNames.FONTTYPE_INFO)
			Call WriteConsoleMsg(UserIndex, "Ahora compartes tus npcs con " & UserList(TargetUserIndex).name & ".", FontTypeNames.FONTTYPE_INFO)
			
		End With
		
	End Sub
	
	''
	' Handles the "StopSharingNpc" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleStopSharingNpc(ByVal UserIndex As Short)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 15/04/2010
		'Stop Sharing owned npcs with other user
		'***************************************************
		
		Dim SharingUserIndex As Short
		
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			SharingUserIndex = .flags.ShareNpcWith
			
			If SharingUserIndex <> 0 Then
				
				' Aviso al que compartia y al que le compartia.
				Call WriteConsoleMsg(SharingUserIndex, .name & " ha dejado de compartir sus npcs contigo.", FontTypeNames.FONTTYPE_INFO)
				Call WriteConsoleMsg(SharingUserIndex, "Has dejado de compartir tus npcs con " & UserList(SharingUserIndex).name & ".", FontTypeNames.FONTTYPE_INFO)
				
				.flags.ShareNpcWith = 0
			End If
			
		End With
		
	End Sub
	
	''
	' Handles the "Inquiry" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleInquiry(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()
		
		ConsultaPopular.SendInfoEncuesta(UserIndex)
	End Sub
	
	''
	' Handles the "GuildMessage" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleGuildMessage(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 15/07/2009
		'02/03/2009: ZaMa - Arreglado un indice mal pasado a la funcion de cartel de clanes overhead.
		'15/07/2009: ZaMa - Now invisible admins only speak by console
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim Chat As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			Chat = buffer.ReadASCIIString()
			
			If migr_LenB(Chat) <> 0 Then
				'Analize chat...
				Call Statistics.ParseChat(Chat)
				
				If .GuildIndex > 0 Then
					Call SendData(modSendData.SendTarget.ToDiosesYclan, .GuildIndex, PrepareMessageGuildChat(.name & "> " & Chat))
					
					If Not (.flags.AdminInvisible = 1) Then Call SendData(modSendData.SendTarget.ToClanArea, UserIndex, PrepareMessageChatOverHead("< " & Chat & " >", .Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Yellow)))
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "PartyMessage" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandlePartyMessage(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim Chat As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			Chat = buffer.ReadASCIIString()
			
			If migr_LenB(Chat) <> 0 Then
				'Analize chat...
				Call Statistics.ParseChat(Chat)
				
				Call mdParty.BroadCastParty(UserIndex, Chat)
				'TODO : Con la 0.12.1 se debe definir si esto vuelve o se borra (/CMSG overhead)
				'Call SendData(SendTarget.ToPartyArea, UserIndex, UserList(UserIndex).Pos.map, "||" & vbYellow & "°< " & mid$(rData, 7) & " >°" & CStr(UserList(UserIndex).Char.CharIndex))
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "CentinelReport" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleCentinelReport(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			Call CentinelaCheckClave(UserIndex, .incomingData.ReadInteger())
		End With
	End Sub
	
	''
	' Handles the "GuildOnline" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleGuildOnline(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		Dim onlineList As String
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			
			onlineList = modGuilds.m_ListaDeMiembrosOnline(UserIndex, .GuildIndex)
			
			If .GuildIndex <> 0 Then
				Call WriteConsoleMsg(UserIndex, "Compañeros de tu clan conectados: " & onlineList, FontTypeNames.FONTTYPE_GUILDMSG)
			Else
				Call WriteConsoleMsg(UserIndex, "No pertences a ningún clan.", FontTypeNames.FONTTYPE_GUILDMSG)
			End If
		End With
	End Sub
	
	''
	' Handles the "PartyOnline" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandlePartyOnline(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		'Remove packet ID
		Call UserList(UserIndex).incomingData.ReadByte()
		
		Call mdParty.OnlineParty(UserIndex)
	End Sub
	
	''
	' Handles the "CouncilMessage" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleCouncilMessage(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim Chat As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			Chat = buffer.ReadASCIIString()
			
			If migr_LenB(Chat) <> 0 Then
				'Analize chat...
				Call Statistics.ParseChat(Chat)
				
				If .flags.Privilegios And Declaraciones.PlayerType.RoyalCouncil Then
					Call SendData(modSendData.SendTarget.ToConsejo, UserIndex, PrepareMessageConsoleMsg("(Consejero) " & .name & "> " & Chat, FontTypeNames.FONTTYPE_CONSEJO))
				ElseIf .flags.Privilegios And Declaraciones.PlayerType.ChaosCouncil Then 
					Call SendData(modSendData.SendTarget.ToConsejoCaos, UserIndex, PrepareMessageConsoleMsg("(Consejero) " & .name & "> " & Chat, FontTypeNames.FONTTYPE_CONSEJOCAOS))
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "RoleMasterRequest" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleRoleMasterRequest(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim request As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			request = buffer.ReadASCIIString()
			
			If migr_LenB(request) <> 0 Then
				Call WriteConsoleMsg(UserIndex, "Su solicitud ha sido enviada.", FontTypeNames.FONTTYPE_INFO)
				Call SendData(modSendData.SendTarget.ToRolesMasters, 0, PrepareMessageConsoleMsg(.name & " PREGUNTA ROL: " & request, FontTypeNames.FONTTYPE_GUILDMSG))
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "GMRequest" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleGMRequest(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If Not Ayuda.Existe(.name) Then
				Call WriteConsoleMsg(UserIndex, "El mensaje ha sido entregado, ahora sólo debes esperar que se desocupe algún GM.", FontTypeNames.FONTTYPE_INFO)
				Call Ayuda.Push(.name)
			Else
				Call Ayuda.Quitar(.name)
				Call Ayuda.Push(.name)
				Call WriteConsoleMsg(UserIndex, "Ya habías mandado un mensaje, tu mensaje ha sido movido al final de la cola de mensajes.", FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub
	
	''
	' Handles the "BugReport" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleBugReport(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim N As Short
		Dim bugReport As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			bugReport = buffer.ReadASCIIString()
			
			N = FreeFile
			FileOpen(N, My.Application.Info.DirectoryPath & "\LOGS\BUGs.log", OpenMode.Append, , OpenShare.Shared)
			PrintLine(N, "Usuario:" & .name & "  Fecha:" & Today & "    Hora:" & TimeOfDay)
			PrintLine(N, "BUG:")
			PrintLine(N, bugReport)
			PrintLine(N, "########################################################################")
			FileClose(N)
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "ChangeDescription" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleChangeDescription(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim Description As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			Description = buffer.ReadASCIIString()
			
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "No puedes cambiar la descripción estando muerto.", FontTypeNames.FONTTYPE_INFO)
			Else
				If Not AsciiValidos(Description) Then
					Call WriteConsoleMsg(UserIndex, "La descripción tiene caracteres inválidos.", FontTypeNames.FONTTYPE_INFO)
				Else
					.desc = Trim(Description)
					Call WriteConsoleMsg(UserIndex, "La descripción ha cambiado.", FontTypeNames.FONTTYPE_INFO)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "GuildVote" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleGuildVote(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim vote As String
		Dim errorStr As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			vote = buffer.ReadASCIIString()
			
			If Not modGuilds.v_UsuarioVota(UserIndex, vote, errorStr) Then
				Call WriteConsoleMsg(UserIndex, "Voto NO contabilizado: " & errorStr, FontTypeNames.FONTTYPE_GUILD)
			Else
				Call WriteConsoleMsg(UserIndex, "Voto contabilizado.", FontTypeNames.FONTTYPE_GUILD)
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "ShowGuildNews" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleShowGuildNews(ByVal UserIndex As Short)
		'***************************************************
		'Author: ZaMA
		'Last Modification: 05/17/06
		'
		'***************************************************
		
		With UserList(UserIndex)
			
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			Call modGuilds.SendGuildNews(UserIndex)
		End With
	End Sub
	
	''
	' Handles the "Punishments" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandlePunishments(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 25/08/2009
		'25/08/2009: ZaMa - Now only admins can see other admins' punishment list
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim name As String
		Dim Count As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			name = buffer.ReadASCIIString()
			
			If migr_LenB(name) <> 0 Then
				If (migr_InStrB(name, "\") <> 0) Then
					name = Replace(name, "\", "")
				End If
				If (migr_InStrB(name, "/") <> 0) Then
					name = Replace(name, "/", "")
				End If
				If (migr_InStrB(name, ":") <> 0) Then
					name = Replace(name, ":", "")
				End If
				If (migr_InStrB(name, "|") <> 0) Then
					name = Replace(name, "|", "")
				End If
				
				If (EsAdmin(name) Or EsDios(name) Or EsSemiDios(name) Or EsConsejero(name) Or EsRolesMaster(name)) And (UserList(UserIndex).flags.Privilegios And Declaraciones.PlayerType.User) Then
					Call WriteConsoleMsg(UserIndex, "No puedes ver las penas de los administradores.", FontTypeNames.FONTTYPE_INFO)
				Else
					If FileExist(CharPath & name & ".chr") Then
						Count = Val(GetVar(CharPath & name & ".chr", "PENAS", "Cant"))
						If Count = 0 Then
							Call WriteConsoleMsg(UserIndex, "Sin prontuario..", FontTypeNames.FONTTYPE_INFO)
						Else
							While Count > 0
								Call WriteConsoleMsg(UserIndex, Count & " - " & GetVar(CharPath & name & ".chr", "PENAS", "P" & Count), FontTypeNames.FONTTYPE_INFO)
								Count = Count - 1
							End While
						End If
					Else
						Call WriteConsoleMsg(UserIndex, "Personaje """ & name & """ inexistente.", FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "ChangePassword" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleChangePassword(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Creation Date: 10/10/07
		'Last Modified By: Rapsodius
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim oldPass As String
		Dim newPass As String
		Dim oldPass2 As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			oldPass = UCase(buffer.ReadASCIIString())
			newPass = UCase(buffer.ReadASCIIString())
			
			If migr_LenB(newPass) = 0 Then
				Call WriteConsoleMsg(UserIndex, "Debes especificar una contraseña nueva, inténtalo de nuevo.", FontTypeNames.FONTTYPE_INFO)
			Else
				oldPass2 = UCase(GetVar(CharPath & UserList(UserIndex).name & ".chr", "INIT", "Password"))
				
				If oldPass2 <> oldPass Then
					Call WriteConsoleMsg(UserIndex, "La contraseña actual proporcionada no es correcta. La contraseña no ha sido cambiada, inténtalo de nuevo.", FontTypeNames.FONTTYPE_INFO)
				Else
					Call WriteVar(CharPath & UserList(UserIndex).name & ".chr", "INIT", "Password", newPass)
					Call WriteConsoleMsg(UserIndex, "La contraseña fue cambiada con éxito.", FontTypeNames.FONTTYPE_INFO)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	
	''
	' Handles the "Gamble" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleGamble(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim Amount As Short
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			
			Amount = .incomingData.ReadInteger()
			
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
			ElseIf .flags.TargetNPC = 0 Then 
				'Validate target NPC
				Call WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.", FontTypeNames.FONTTYPE_INFO)
			ElseIf Distancia(Npclist(.flags.TargetNPC).Pos, .Pos) > 10 Then 
				Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
			ElseIf Npclist(.flags.TargetNPC).NPCtype <> Declaraciones.eNPCType.Timbero Then 
				Call WriteChatOverHead(UserIndex, "No tengo ningún interés en apostar.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
			ElseIf Amount < 1 Then 
				Call WriteChatOverHead(UserIndex, "El mínimo de apuesta es 1 moneda.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
			ElseIf Amount > 5000 Then 
				Call WriteChatOverHead(UserIndex, "El máximo de apuesta es 5000 monedas.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
			ElseIf .Stats.GLD < Amount Then 
				Call WriteChatOverHead(UserIndex, "No tienes esa cantidad.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
			Else
				If RandomNumber(1, 100) <= 47 Then
					.Stats.GLD = .Stats.GLD + Amount
					Call WriteChatOverHead(UserIndex, "¡Felicidades! Has ganado " & CStr(Amount) & " monedas de oro.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
					
					Apuestas.Perdidas = Apuestas.Perdidas + Amount
					Call WriteVar(DatPath & "apuestas.dat", "Main", "Perdidas", CStr(Apuestas.Perdidas))
				Else
					.Stats.GLD = .Stats.GLD - Amount
					Call WriteChatOverHead(UserIndex, "Lo siento, has perdido " & CStr(Amount) & " monedas de oro.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
					
					Apuestas.Ganancias = Apuestas.Ganancias + Amount
					Call WriteVar(DatPath & "apuestas.dat", "Main", "Ganancias", CStr(Apuestas.Ganancias))
				End If
				
				Apuestas.Jugadas = Apuestas.Jugadas + 1
				
				Call WriteVar(DatPath & "apuestas.dat", "Main", "Jugadas", CStr(Apuestas.Jugadas))
				
				Call WriteUpdateGold(UserIndex)
			End If
		End With
	End Sub
	
	''
	' Handles the "InquiryVote" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleInquiryVote(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim opt As Byte
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			
			opt = .incomingData.ReadByte()
			
			Call WriteConsoleMsg(UserIndex, ConsultaPopular.doVotar(UserIndex, opt), FontTypeNames.FONTTYPE_GUILD)
		End With
	End Sub
	
	''
	' Handles the "BankExtractGold" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleBankExtractGold(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim Amount As Integer
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			
			Amount = .incomingData.ReadLong()
			
			'Dead people can't leave a faction.. they can't talk...
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If
			
			'Validate target NPC
			If .flags.TargetNPC = 0 Then
				Call WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If
			
			If Npclist(.flags.TargetNPC).NPCtype <> Declaraciones.eNPCType.Banquero Then Exit Sub
			
			If Distancia(.Pos, Npclist(.flags.TargetNPC).Pos) > 10 Then
				Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If
			
			If Amount > 0 And Amount <= .Stats.Banco Then
				.Stats.Banco = .Stats.Banco - Amount
				.Stats.GLD = .Stats.GLD + Amount
				Call WriteChatOverHead(UserIndex, "Tenés " & .Stats.Banco & " monedas de oro en tu cuenta.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
			Else
				Call WriteChatOverHead(UserIndex, "No tienes esa cantidad.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
			End If
			
			Call WriteUpdateGold(UserIndex)
			Call WriteUpdateBankGold(UserIndex)
		End With
	End Sub
	
	''
	' Handles the "LeaveFaction" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleLeaveFaction(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		
		Dim TalkToKing As Boolean
		Dim TalkToDemon As Boolean
		Dim NpcIndex As Short
		
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			'Dead people can't leave a faction.. they can't talk...
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If
			
			' Chequea si habla con el rey o el demonio. Puede salir sin hacerlo, pero si lo hace le reponden los npcs
			NpcIndex = .flags.TargetNPC
			If NpcIndex <> 0 Then
				' Es rey o domonio?
				If Npclist(NpcIndex).NPCtype = Declaraciones.eNPCType.Noble Then
					'Rey?
					If Npclist(NpcIndex).flags.Faccion = 0 Then
						TalkToKing = True
						' Demonio
					Else
						TalkToDemon = True
					End If
				End If
			End If
			
			'Quit the Royal Army?
			If .Faccion.ArmadaReal = 1 Then
				' Si le pidio al demonio salir de la armada, este le responde.
				If TalkToDemon Then
					Call WriteChatOverHead(UserIndex, "¡¡¡Sal de aquí bufón!!!", Npclist(NpcIndex).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
					
				Else
					' Si le pidio al rey salir de la armada, le responde.
					If TalkToKing Then
						Call WriteChatOverHead(UserIndex, "Serás bienvenido a las fuerzas imperiales si deseas regresar.", Npclist(NpcIndex).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
					End If
					
					Call ExpulsarFaccionReal(UserIndex, False)
					
				End If
				
				'Quit the Chaos Legion?
			ElseIf .Faccion.FuerzasCaos = 1 Then 
				' Si le pidio al rey salir del caos, le responde.
				If TalkToKing Then
					Call WriteChatOverHead(UserIndex, "¡¡¡Sal de aquí maldito criminal!!!", Npclist(NpcIndex).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
				Else
					' Si le pidio al demonio salir del caos, este le responde.
					If TalkToDemon Then
						Call WriteChatOverHead(UserIndex, "Ya volverás arrastrandote.", Npclist(NpcIndex).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
					End If
					
					Call ExpulsarFaccionCaos(UserIndex, False)
				End If
				' No es faccionario
			Else
				
				' Si le hablaba al rey o demonio, le repsonden ellos
				If NpcIndex > 0 Then
					Call WriteChatOverHead(UserIndex, "¡No perteneces a ninguna facción!", Npclist(NpcIndex).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
				Else
					Call WriteConsoleMsg(UserIndex, "¡No perteneces a ninguna facción!", FontTypeNames.FONTTYPE_FIGHT)
				End If
				
			End If
			
		End With
		
	End Sub
	
	''
	' Handles the "BankDepositGold" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleBankDepositGold(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim Amount As Integer
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			
			Amount = .incomingData.ReadLong()
			
			'Dead people can't leave a faction.. they can't talk...
			If .flags.Muerto = 1 Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If
			
			'Validate target NPC
			If .flags.TargetNPC = 0 Then
				Call WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If
			
			If Distancia(Npclist(.flags.TargetNPC).Pos, .Pos) > 10 Then
				Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If
			
			If Npclist(.flags.TargetNPC).NPCtype <> Declaraciones.eNPCType.Banquero Then Exit Sub
			
			If Amount > 0 And Amount <= .Stats.GLD Then
				.Stats.Banco = .Stats.Banco + Amount
				.Stats.GLD = .Stats.GLD - Amount
				Call WriteChatOverHead(UserIndex, "Tenés " & .Stats.Banco & " monedas de oro en tu cuenta.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
				
				Call WriteUpdateGold(UserIndex)
				Call WriteUpdateBankGold(UserIndex)
			Else
				Call WriteChatOverHead(UserIndex, "No tenés esa cantidad.", Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
			End If
		End With
	End Sub
	
	''
	' Handles the "Denounce" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleDenounce(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim Text As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			Text = buffer.ReadASCIIString()
			
			If .flags.Silenciado = 0 Then
				'Analize chat...
				Call Statistics.ParseChat(Text)
				
				Call SendData(modSendData.SendTarget.ToAdmins, 0, PrepareMessageConsoleMsg(LCase(.name) & " DENUNCIA: " & Text, FontTypeNames.FONTTYPE_GUILDMSG))
				Call WriteConsoleMsg(UserIndex, "Denuncia enviada, espere..", FontTypeNames.FONTTYPE_INFO)
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "GuildFundate" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleGuildFundate(ByVal UserIndex As Short)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 14/12/2009
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 1 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		With UserList(UserIndex)
			Call .incomingData.ReadByte()
			
			If HasFound(.name) Then
				Call WriteConsoleMsg(UserIndex, "¡Ya has fundado un clan, no puedes fundar otro!", FontTypeNames.FONTTYPE_INFOBOLD)
				Exit Sub
			End If
			
			Call WriteShowGuildAlign(UserIndex)
		End With
	End Sub
	
	''
	' Handles the "GuildFundation" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleGuildFundation(ByVal UserIndex As Short)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 14/12/2009
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim clanType As Declaraciones.eClanType
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As String
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			
			clanType = .incomingData.ReadByte()
			
			If HasFound(.name) Then
				Call WriteConsoleMsg(UserIndex, "¡Ya has fundado un clan, no puedes fundar otro!", FontTypeNames.FONTTYPE_INFOBOLD)
				Call LogCheating("El usuario " & .name & " ha intentado fundar un clan ya habiendo fundado otro desde la IP " & .ip)
				Exit Sub
			End If
			
			Select Case UCase(Trim(CStr(clanType)))
				Case CStr(Declaraciones.eClanType.ct_RoyalArmy)
					.FundandoGuildAlineacion = modGuilds.ALINEACION_GUILD.ALINEACION_ARMADA
				Case CStr(Declaraciones.eClanType.ct_Evil)
					.FundandoGuildAlineacion = modGuilds.ALINEACION_GUILD.ALINEACION_LEGION
				Case CStr(Declaraciones.eClanType.ct_Neutral)
					.FundandoGuildAlineacion = modGuilds.ALINEACION_GUILD.ALINEACION_NEUTRO
				Case CStr(Declaraciones.eClanType.ct_GM)
					.FundandoGuildAlineacion = modGuilds.ALINEACION_GUILD.ALINEACION_MASTER
				Case CStr(Declaraciones.eClanType.ct_Legal)
					.FundandoGuildAlineacion = modGuilds.ALINEACION_GUILD.ALINEACION_CIUDA
				Case CStr(Declaraciones.eClanType.ct_Criminal)
					.FundandoGuildAlineacion = modGuilds.ALINEACION_GUILD.ALINEACION_CRIMINAL
				Case Else
					Call WriteConsoleMsg(UserIndex, "Alineación inválida.", FontTypeNames.FONTTYPE_GUILD)
					Exit Sub
			End Select
			
			If modGuilds.PuedeFundarUnClan(UserIndex, .FundandoGuildAlineacion, error_Renamed) Then
				Call WriteShowGuildFundationForm(UserIndex)
			Else
				.FundandoGuildAlineacion = 0
				Call WriteConsoleMsg(UserIndex, error_Renamed, FontTypeNames.FONTTYPE_GUILD)
			End If
		End With
	End Sub
	
	''
	' Handles the "PartyKick" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandlePartyKick(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/05/09
		'Last Modification by: Marco Vanotti (Marco)
		'- 05/05/09: Now it uses "UserPuedeEjecutarComandos" to check if the user can use party commands
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If UserPuedeEjecutarComandos(UserIndex) Then
				tUser = NameIndex(UserName)
				
				If tUser > 0 Then
					Call mdParty.ExpulsarDeParty(UserIndex, tUser)
				Else
					If InStr(UserName, "+") Then
						UserName = Replace(UserName, "+", " ")
					End If
					
					Call WriteConsoleMsg(UserIndex, LCase(UserName) & " no pertenece a tu party.", FontTypeNames.FONTTYPE_INFO)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "PartySetLeader" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandlePartySetLeader(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/05/09
		'Last Modification by: Marco Vanotti (MarKoxX)
		'- 05/05/09: Now it uses "UserPuedeEjecutarComandos" to check if the user can use party commands
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		'On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		Dim rank As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			rank = Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.Consejero
			
			UserName = buffer.ReadASCIIString()
			If UserPuedeEjecutarComandos(UserIndex) Then
				tUser = NameIndex(UserName)
				If tUser > 0 Then
					'Don't allow users to spoof online GMs
					If (UserDarPrivilegioLevel(UserName) And rank) <= (.flags.Privilegios And rank) Then
						Call mdParty.TransformarEnLider(UserIndex, tUser)
					Else
						Call WriteConsoleMsg(UserIndex, LCase(UserList(tUser).name) & " no pertenece a tu party.", FontTypeNames.FONTTYPE_INFO)
					End If
					
				Else
					If InStr(UserName, "+") Then
						UserName = Replace(UserName, "+", " ")
					End If
					Call WriteConsoleMsg(UserIndex, LCase(UserName) & " no pertenece a tu party.", FontTypeNames.FONTTYPE_INFO)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "PartyAcceptMember" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandlePartyAcceptMember(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/05/09
		'Last Modification by: Marco Vanotti (Marco)
		'- 05/05/09: Now it uses "UserPuedeEjecutarComandos" to check if the user can use party commands
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		Dim rank As Short
		Dim bUserVivo As Boolean
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			rank = Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.Consejero
			
			UserName = buffer.ReadASCIIString()
			If UserList(UserIndex).flags.Muerto Then
				Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_PARTY)
			Else
				bUserVivo = True
			End If
			
			If mdParty.UserPuedeEjecutarComandos(UserIndex) And bUserVivo Then
				tUser = NameIndex(UserName)
				If tUser > 0 Then
					'Validate administrative ranks - don't allow users to spoof online GMs
					If (UserList(tUser).flags.Privilegios And rank) <= (.flags.Privilegios And rank) Then
						Call mdParty.AprobarIngresoAParty(UserIndex, tUser)
					Else
						Call WriteConsoleMsg(UserIndex, "No puedes incorporar a tu party a personajes de mayor jerarquía.", FontTypeNames.FONTTYPE_INFO)
					End If
				Else
					If InStr(UserName, "+") Then
						UserName = Replace(UserName, "+", " ")
					End If
					
					'Don't allow users to spoof online GMs
					If (UserDarPrivilegioLevel(UserName) And rank) <= (.flags.Privilegios And rank) Then
						Call WriteConsoleMsg(UserIndex, LCase(UserName) & " no ha solicitado ingresar a tu party.", FontTypeNames.FONTTYPE_PARTY)
					Else
						Call WriteConsoleMsg(UserIndex, "No puedes incorporar a tu party a personajes de mayor jerarquía.", FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "GuildMemberList" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleGuildMemberList(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim guild As String
		Dim memberCount As Short
		Dim i As Integer
		Dim UserName As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			guild = buffer.ReadASCIIString()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios) Then
				If (migr_InStrB(guild, "\") <> 0) Then
					guild = Replace(guild, "\", "")
				End If
				If (migr_InStrB(guild, "/") <> 0) Then
					guild = Replace(guild, "/", "")
				End If
				
				If Not FileExist(My.Application.Info.DirectoryPath & "\guilds\" & guild & "-members.mem") Then
					Call WriteConsoleMsg(UserIndex, "No existe el clan: " & guild, FontTypeNames.FONTTYPE_INFO)
				Else
					memberCount = Val(GetVar(My.Application.Info.DirectoryPath & "\Guilds\" & guild & "-Members" & ".mem", "INIT", "NroMembers"))
					
					For i = 1 To memberCount
						UserName = GetVar(My.Application.Info.DirectoryPath & "\Guilds\" & guild & "-Members" & ".mem", "Members", "Member" & i)
						
						Call WriteConsoleMsg(UserIndex, UserName & "<" & guild & ">", FontTypeNames.FONTTYPE_INFO)
					Next i
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "GMMessage" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleGMMessage(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 01/08/07
		'Last Modification by: (liquid)
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim message As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			message = buffer.ReadASCIIString()
			
			If Not .flags.Privilegios And Declaraciones.PlayerType.User Then
				Call LogGM(.name, "Mensaje a Gms:" & message)
				
				If migr_LenB(message) <> 0 Then
					'Analize chat...
					Call Statistics.ParseChat(message)
					
					Call SendData(modSendData.SendTarget.ToAdmins, 0, PrepareMessageConsoleMsg(.name & "> " & message, FontTypeNames.FONTTYPE_GMMSG))
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "ShowName" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleShowName(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.RoleMaster) Then
				.showName = Not .showName 'Show / Hide the name
				
				Call RefreshCharStatus(UserIndex)
			End If
		End With
	End Sub
	
	''
	' Handles the "OnlineRoyalArmy" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleOnlineRoyalArmy(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		Dim i As Integer
		Dim list As String
		With UserList(UserIndex)
			'Remove packet ID
			.incomingData.ReadByte()
			
			If .flags.Privilegios And Declaraciones.PlayerType.User Then Exit Sub
			
			
			For i = 1 To LastUser
				If UserList(i).ConnID <> -1 Then
					If UserList(i).Faccion.ArmadaReal = 1 Then
						If UserList(i).flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Or .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin) Then
							list = list & UserList(i).name & ", "
						End If
					End If
				End If
			Next i
		End With
		
		If Len(list) > 0 Then
			Call WriteConsoleMsg(UserIndex, "Reales conectados: " & Left(list, Len(list) - 2), FontTypeNames.FONTTYPE_INFO)
		Else
			Call WriteConsoleMsg(UserIndex, "No hay reales conectados.", FontTypeNames.FONTTYPE_INFO)
		End If
	End Sub
	
	''
	' Handles the "OnlineChaosLegion" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleOnlineChaosLegion(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		Dim i As Integer
		Dim list As String
		With UserList(UserIndex)
			'Remove packet ID
			.incomingData.ReadByte()
			
			If .flags.Privilegios And Declaraciones.PlayerType.User Then Exit Sub
			
			
			For i = 1 To LastUser
				If UserList(i).ConnID <> -1 Then
					If UserList(i).Faccion.FuerzasCaos = 1 Then
						If UserList(i).flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Or .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin) Then
							list = list & UserList(i).name & ", "
						End If
					End If
				End If
			Next i
		End With
		
		If Len(list) > 0 Then
			Call WriteConsoleMsg(UserIndex, "Caos conectados: " & Left(list, Len(list) - 2), FontTypeNames.FONTTYPE_INFO)
		Else
			Call WriteConsoleMsg(UserIndex, "No hay Caos conectados.", FontTypeNames.FONTTYPE_INFO)
		End If
	End Sub
	
	''
	' Handles the "GoNearby" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleGoNearby(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 01/10/07
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tIndex As Short
		Dim X As Integer
		Dim Y As Integer
		Dim i As Integer
		Dim found As Boolean
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			
			tIndex = NameIndex(UserName)
			
			'Check the user has enough powers
			If .flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.Consejero) Then
				'Si es dios o Admins no podemos salvo que nosotros también lo seamos
				If Not (EsDios(UserName) Or EsAdmin(UserName)) Or (.flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin)) Then
					If tIndex <= 0 Then 'existe el usuario destino?
						Call WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO)
					Else
						For i = 2 To 5 'esto for sirve ir cambiando la distancia destino
							For X = UserList(tIndex).Pos.X - i To UserList(tIndex).Pos.X + i
								For Y = UserList(tIndex).Pos.Y - i To UserList(tIndex).Pos.Y + i
									If MapData(UserList(tIndex).Pos.Map, X, Y).UserIndex = 0 Then
										If LegalPos(UserList(tIndex).Pos.Map, X, Y, True, True) Then
											Call WarpUserChar(UserIndex, UserList(tIndex).Pos.Map, X, Y, True)
											Call LogGM(.name, "/IRCERCA " & UserName & " Mapa:" & UserList(tIndex).Pos.Map & " X:" & UserList(tIndex).Pos.X & " Y:" & UserList(tIndex).Pos.Y)
											found = True
											Exit For
										End If
									End If
								Next Y
								
								If found Then Exit For ' Feo, pero hay que abortar 3 fors sin usar GoTo
							Next X
							
							If found Then Exit For ' Feo, pero hay que abortar 3 fors sin usar GoTo
						Next i
						
						'No space found??
						If Not found Then
							Call WriteConsoleMsg(UserIndex, "Todos los lugares están ocupados.", FontTypeNames.FONTTYPE_INFO)
						End If
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "Comment" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleComment(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim comment As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			comment = buffer.ReadASCIIString()
			
			If Not .flags.Privilegios And Declaraciones.PlayerType.User Then
				Call LogGM(.name, "Comentario: " & comment)
				Call WriteConsoleMsg(UserIndex, "Comentario salvado...", FontTypeNames.FONTTYPE_INFO)
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "ServerTime" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleServerTime(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 01/08/07
		'Last Modification by: (liquid)
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And Declaraciones.PlayerType.User Then Exit Sub
			
			Call LogGM(.name, "Hora.")
		End With
		
		Call modSendData.SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg("Hora: " & TimeOfDay & " " & Today, FontTypeNames.FONTTYPE_INFO))
	End Sub
	
	''
	' Handles the "Where" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleWhere(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If Not .flags.Privilegios And Declaraciones.PlayerType.User Then
				tUser = NameIndex(UserName)
				If tUser <= 0 Then
					Call WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO)
				Else
					If (UserList(tUser).flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios)) <> 0 Or ((UserList(tUser).flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin) <> 0) And (.flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin)) <> 0) Then
						Call WriteConsoleMsg(UserIndex, "Ubicación  " & UserName & ": " & UserList(tUser).Pos.Map & ", " & UserList(tUser).Pos.X & ", " & UserList(tUser).Pos.Y & ".", FontTypeNames.FONTTYPE_INFO)
						Call LogGM(.name, "/Donde " & UserName)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "CreaturesInMap" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleCreaturesInMap(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 30/07/06
		'Pablo (ToxicWaste): modificaciones generales para simplificar la visualización.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim Map As Short
		Dim NPCcant1() As Short
		Dim NPCcant2() As Short
		Dim List1() As String
		Dim List2() As String
		Dim NPCcount1 As Object
		Dim NPCcount2 As Short
		Dim i As Object
		Dim j As Integer
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			
			Map = .incomingData.ReadInteger()
			
			If .flags.Privilegios And Declaraciones.PlayerType.User Then Exit Sub
			
			If MapaValido(Map) Then
				For i = 1 To LastNPC
					'VB isn't lazzy, so we put more restrictive condition first to speed up the process
					'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					If Npclist(i).Pos.Map = Map Then
						'¿esta vivo?
						'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
						If Npclist(i).flags.NPCActive And Npclist(i).Hostile = 1 And Npclist(i).Stats.Alineacion = 2 Then
							'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
							If NPCcount1 = 0 Then
								ReDim List1(0)
								ReDim NPCcant1(0)
								'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
								NPCcount1 = 1
								'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
								List1(0) = Npclist(i).name & ": (" & Npclist(i).Pos.X & "," & Npclist(i).Pos.Y & ")"
								NPCcant1(0) = 1
							Else
								'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
								For j = 0 To NPCcount1 - 1
									'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
									If Left(List1(j), Len(Npclist(i).name)) = Npclist(i).name Then
										'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
										List1(j) = List1(j) & ", (" & Npclist(i).Pos.X & "," & Npclist(i).Pos.Y & ")"
										NPCcant1(j) = NPCcant1(j) + 1
										Exit For
									End If
								Next j
								'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
								If j = NPCcount1 Then
									'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
									ReDim Preserve List1(NPCcount1)
									'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
									ReDim Preserve NPCcant1(NPCcount1)
									'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
									NPCcount1 = NPCcount1 + 1
									'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
									List1(j) = Npclist(i).name & ": (" & Npclist(i).Pos.X & "," & Npclist(i).Pos.Y & ")"
									NPCcant1(j) = 1
								End If
							End If
						Else
							If NPCcount2 = 0 Then
								ReDim List2(0)
								ReDim NPCcant2(0)
								NPCcount2 = 1
								'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
								List2(0) = Npclist(i).name & ": (" & Npclist(i).Pos.X & "," & Npclist(i).Pos.Y & ")"
								NPCcant2(0) = 1
							Else
								For j = 0 To NPCcount2 - 1
									'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
									If Left(List2(j), Len(Npclist(i).name)) = Npclist(i).name Then
										'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
										List2(j) = List2(j) & ", (" & Npclist(i).Pos.X & "," & Npclist(i).Pos.Y & ")"
										NPCcant2(j) = NPCcant2(j) + 1
										Exit For
									End If
								Next j
								If j = NPCcount2 Then
									ReDim Preserve List2(NPCcount2)
									ReDim Preserve NPCcant2(NPCcount2)
									NPCcount2 = NPCcount2 + 1
									'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
									List2(j) = Npclist(i).name & ": (" & Npclist(i).Pos.X & "," & Npclist(i).Pos.Y & ")"
									NPCcant2(j) = 1
								End If
							End If
						End If
					End If
				Next i
				
				Call WriteConsoleMsg(UserIndex, "Npcs Hostiles en mapa: ", FontTypeNames.FONTTYPE_WARNING)
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				If NPCcount1 = 0 Then
					Call WriteConsoleMsg(UserIndex, "No hay NPCS Hostiles.", FontTypeNames.FONTTYPE_INFO)
				Else
					'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
					For j = 0 To NPCcount1 - 1
						Call WriteConsoleMsg(UserIndex, NPCcant1(j) & " " & List1(j), FontTypeNames.FONTTYPE_INFO)
					Next j
				End If
				Call WriteConsoleMsg(UserIndex, "Otros Npcs en mapa: ", FontTypeNames.FONTTYPE_WARNING)
				If NPCcount2 = 0 Then
					Call WriteConsoleMsg(UserIndex, "No hay más NPCS.", FontTypeNames.FONTTYPE_INFO)
				Else
					For j = 0 To NPCcount2 - 1
						Call WriteConsoleMsg(UserIndex, NPCcant2(j) & " " & List2(j), FontTypeNames.FONTTYPE_INFO)
					Next j
				End If
				Call LogGM(.name, "Numero enemigos en mapa " & Map)
			End If
		End With
	End Sub
	
	''
	' Handles the "WarpMeToTarget" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleWarpMeToTarget(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 26/03/09
		'26/03/06: ZaMa - Chequeo que no se teletransporte donde haya un char o npc
		'***************************************************
		Dim X As Short
		Dim Y As Short
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			
			If .flags.Privilegios And Declaraciones.PlayerType.User Then Exit Sub
			
			X = .flags.TargetX
			Y = .flags.TargetY
			
			Call FindLegalPos(UserIndex, .flags.TargetMap, X, Y)
			Call WarpUserChar(UserIndex, .flags.TargetMap, X, Y, True)
			Call LogGM(.name, "/TELEPLOC a x:" & .flags.TargetX & " Y:" & .flags.TargetY & " Map:" & .Pos.Map)
		End With
	End Sub
	
	''
	' Handles the "WarpChar" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleWarpChar(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 26/03/2009
		'26/03/2009: ZaMa -  Chequeo que no se teletransporte a un tile donde haya un char o npc.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 7 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim Map As Short
		Dim X As Short
		Dim Y As Short
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			Map = buffer.ReadInteger()
			X = buffer.ReadByte()
			Y = buffer.ReadByte()
			
			If Not .flags.Privilegios And Declaraciones.PlayerType.User Then
				If MapaValido(Map) And migr_LenB(UserName) <> 0 Then
					If UCase(UserName) <> "YO" Then
						If Not .flags.Privilegios And Declaraciones.PlayerType.Consejero Then
							tUser = NameIndex(UserName)
						End If
					Else
						tUser = UserIndex
					End If
					
					If tUser <= 0 Then
						Call WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO)
					ElseIf InMapBounds(Map, X, Y) Then 
						Call FindLegalPos(tUser, Map, X, Y)
						Call WarpUserChar(tUser, Map, X, Y, True, True)
						Call WriteConsoleMsg(UserIndex, UserList(tUser).name & " transportado.", FontTypeNames.FONTTYPE_INFO)
						Call LogGM(.name, "Transportó a " & UserList(tUser).name & " hacia " & "Mapa" & Map & " X:" & X & " Y:" & Y)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "Silence" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleSilence(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If Not .flags.Privilegios And Declaraciones.PlayerType.User Then
				tUser = NameIndex(UserName)
				
				If tUser <= 0 Then
					Call WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO)
				Else
					If UserList(tUser).flags.Silenciado = 0 Then
						UserList(tUser).flags.Silenciado = 1
						Call WriteConsoleMsg(UserIndex, "Usuario silenciado.", FontTypeNames.FONTTYPE_INFO)
						Call WriteShowMessageBox(tUser, "Estimado usuario, ud. ha sido silenciado por los administradores. Sus denuncias serán ignoradas por el servidor de aquí en más. Utilice /GM para contactar un administrador.")
						Call LogGM(.name, "/silenciar " & UserList(tUser).name)
						
						'Flush the other user's buffer
						Call FlushBuffer(tUser)
					Else
						UserList(tUser).flags.Silenciado = 0
						Call WriteConsoleMsg(UserIndex, "Usuario des silenciado.", FontTypeNames.FONTTYPE_INFO)
						Call LogGM(.name, "/DESsilenciar " & UserList(tUser).name)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "SOSShowList" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleSOSShowList(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And Declaraciones.PlayerType.User Then Exit Sub
			Call WriteShowSOSForm(UserIndex)
		End With
	End Sub
	
	''
	' Handles the "RequestPartyForm" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandlePartyForm(ByVal UserIndex As Short)
		'***************************************************
		'Author: Budi
		'Last Modification: 11/26/09
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			If .PartyIndex > 0 Then
				Call WriteShowPartyForm(UserIndex)
				
			Else
				Call WriteConsoleMsg(UserIndex, "No perteneces a ningún grupo!", FontTypeNames.FONTTYPE_INFOBOLD)
			End If
		End With
	End Sub
	
	''
	' Handles the "ItemUpgrade" message.
	'
	' @param    UserIndex The index of the user sending the message.
	
	Private Sub HandleItemUpgrade(ByVal UserIndex As Short)
		'***************************************************
		'Author: Torres Patricio
		'Last Modification: 12/09/09
		'
		'***************************************************
		Dim ItemIndex As Short
		With UserList(UserIndex)
			
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			ItemIndex = .incomingData.ReadInteger()
			
			If ItemIndex <= 0 Then Exit Sub
			If Not TieneObjetos(ItemIndex, 1, UserIndex) Then Exit Sub
			
			Call DoUpgrade(UserIndex, ItemIndex)
		End With
	End Sub
	
	''
	' Handles the "SOSRemove" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleSOSRemove(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			UserName = buffer.ReadASCIIString()
			
			If Not .flags.Privilegios And Declaraciones.PlayerType.User Then Call Ayuda.Quitar(UserName)
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "GoToChar" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleGoToChar(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 26/03/2009
		'26/03/2009: ZaMa -  Chequeo que no se teletransporte a un tile donde haya un char o npc.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		Dim X As Short
		Dim Y As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			tUser = NameIndex(UserName)
			
			If .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.Consejero) Then
				'Si es dios o Admins no podemos salvo que nosotros también lo seamos
				If Not (EsDios(UserName) Or EsAdmin(UserName)) Or (.flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin)) <> 0 Then
					If tUser <= 0 Then
						Call WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO)
					Else
						X = UserList(tUser).Pos.X
						Y = UserList(tUser).Pos.Y + 1
						Call FindLegalPos(UserIndex, UserList(tUser).Pos.Map, X, Y)
						
						Call WarpUserChar(UserIndex, UserList(tUser).Pos.Map, X, Y, True)
						
						If .flags.AdminInvisible = 0 Then
							Call WriteConsoleMsg(tUser, .name & " se ha trasportado hacia donde te encuentras.", FontTypeNames.FONTTYPE_INFO)
							Call FlushBuffer(tUser)
						End If
						
						Call LogGM(.name, "/IRA " & UserName & " Mapa:" & UserList(tUser).Pos.Map & " X:" & UserList(tUser).Pos.X & " Y:" & UserList(tUser).Pos.Y)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "Invisible" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleInvisible(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And Declaraciones.PlayerType.User Then Exit Sub
			
			Call DoAdminInvisible(UserIndex)
			Call LogGM(.name, "/INVISIBLE")
		End With
	End Sub
	
	''
	' Handles the "GMPanel" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleGMPanel(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And Declaraciones.PlayerType.User Then Exit Sub
			
			Call WriteShowGMPanelForm(UserIndex)
		End With
	End Sub
	
	''
	' Handles the "GMPanel" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleRequestUserList(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 01/09/07
		'Last modified by: Lucas Tavolaro Ortiz (Tavo)
		'I haven`t found a solution to split, so i make an array of names
		'***************************************************
		Dim i As Integer
		Dim names() As String
		Dim Count As Integer
		
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			'UPGRADE_WARNING: El límite inferior de la matriz names ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
			ReDim names(LastUser)
			Count = 1
			
			For i = 1 To LastUser
				If (migr_LenB(UserList(i).name) <> 0) Then
					If UserList(i).flags.Privilegios And Declaraciones.PlayerType.User Then
						names(Count) = UserList(i).name
						Count = Count + 1
					End If
				End If
			Next i
			
			If Count > 1 Then Call WriteUserNameList(UserIndex, names, Count - 1)
		End With
	End Sub
	
	''
	' Handles the "Working" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleWorking(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		Dim i As Integer
		Dim users As String
		
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			For i = 1 To LastUser
				If UserList(i).flags.UserLogged And UserList(i).Counters.Trabajando > 0 Then
					users = users & ", " & UserList(i).name
					
					' Display the user being checked by the centinel
					If modCentinela.Centinela.RevisandoUserIndex = i Then users = users & " (*)"
				End If
			Next i
			
			If migr_LenB(users) <> 0 Then
				users = Right(users, Len(users) - 2)
				Call WriteConsoleMsg(UserIndex, "Usuarios trabajando: " & users, FontTypeNames.FONTTYPE_INFO)
			Else
				Call WriteConsoleMsg(UserIndex, "No hay usuarios trabajando.", FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub
	
	''
	' Handles the "Hiding" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleHiding(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		Dim i As Integer
		Dim users As String
		
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			For i = 1 To LastUser
				If (migr_LenB(UserList(i).name) <> 0) And UserList(i).Counters.Ocultando > 0 Then
					users = users & UserList(i).name & ", "
				End If
			Next i
			
			If migr_LenB(users) <> 0 Then
				users = Left(users, Len(users) - 2)
				Call WriteConsoleMsg(UserIndex, "Usuarios ocultandose: " & users, FontTypeNames.FONTTYPE_INFO)
			Else
				Call WriteConsoleMsg(UserIndex, "No hay usuarios ocultandose.", FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub
	
	''
	' Handles the "Jail" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleJail(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 6 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim reason As String
		Dim jailTime As Byte
		Dim Count As Byte
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			reason = buffer.ReadASCIIString()
			jailTime = buffer.ReadByte()
			
			If InStr(1, UserName, "+") Then
				UserName = Replace(UserName, "+", " ")
			End If
			
			'/carcel nick@motivo@<tiempo>
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (Not .flags.Privilegios And Declaraciones.PlayerType.User) <> 0 Then
				If migr_LenB(UserName) = 0 Or migr_LenB(reason) = 0 Then
					Call WriteConsoleMsg(UserIndex, "Utilice /carcel nick@motivo@tiempo", FontTypeNames.FONTTYPE_INFO)
				Else
					tUser = NameIndex(UserName)
					
					If tUser <= 0 Then
						Call WriteConsoleMsg(UserIndex, "El usuario no está online.", FontTypeNames.FONTTYPE_INFO)
					Else
						If Not UserList(tUser).flags.Privilegios And Declaraciones.PlayerType.User Then
							Call WriteConsoleMsg(UserIndex, "No puedes encarcelar a administradores.", FontTypeNames.FONTTYPE_INFO)
						ElseIf jailTime > 60 Then 
							Call WriteConsoleMsg(UserIndex, "No puedés encarcelar por más de 60 minutos.", FontTypeNames.FONTTYPE_INFO)
						Else
							If (migr_InStrB(UserName, "\") <> 0) Then
								UserName = Replace(UserName, "\", "")
							End If
							If (migr_InStrB(UserName, "/") <> 0) Then
								UserName = Replace(UserName, "/", "")
							End If
							
							If FileExist(CharPath & UserName & ".chr") Then
								Count = Val(GetVar(CharPath & UserName & ".chr", "PENAS", "Cant"))
								Call WriteVar(CharPath & UserName & ".chr", "PENAS", "Cant", CStr(Count + 1))
								Call WriteVar(CharPath & UserName & ".chr", "PENAS", "P" & Count + 1, LCase(.name) & ": CARCEL " & jailTime & "m, MOTIVO: " & LCase(reason) & " " & Today & " " & TimeOfDay)
							End If
							
							Call Encarcelar(tUser, jailTime, .name)
							Call LogGM(.name, " encarceló a " & UserName)
						End If
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "KillNPC" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleKillNPC(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 04/22/08 (NicoNZ)
		'
		'***************************************************
		Dim tNPC As Short
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura auxNPC, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim auxNPC As npc
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And Declaraciones.PlayerType.User Then Exit Sub
			
			
			'Los consejeros no pueden RMATAr a nada en el mapa pretoriano
			If .flags.Privilegios And Declaraciones.PlayerType.Consejero Then
				If .Pos.Map = MAPA_PRETORIANO Then
					Call WriteConsoleMsg(UserIndex, "Los consejeros no pueden usar este comando en el mapa pretoriano.", FontTypeNames.FONTTYPE_INFO)
					Exit Sub
				End If
			End If
			
			tNPC = .flags.TargetNPC
			
			If tNPC > 0 Then
				Call WriteConsoleMsg(UserIndex, "RMatas (con posible respawn) a: " & Npclist(tNPC).name, FontTypeNames.FONTTYPE_INFO)
				
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto auxNPC. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				auxNPC = Npclist(tNPC)
				Call QuitarNPC(tNPC)
				Call ReSpawnNpc(auxNPC)
				
				.flags.TargetNPC = 0
			Else
				Call WriteConsoleMsg(UserIndex, "Antes debes hacer click sobre el NPC.", FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub
	
	''
	' Handles the "WarnUser" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleWarnUser(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/26/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim reason As String
		Dim privs As Declaraciones.PlayerType
		Dim Count As Byte
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			reason = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (Not .flags.Privilegios And Declaraciones.PlayerType.User) <> 0 Then
				If migr_LenB(UserName) = 0 Or migr_LenB(reason) = 0 Then
					Call WriteConsoleMsg(UserIndex, "Utilice /advertencia nick@motivo", FontTypeNames.FONTTYPE_INFO)
				Else
					privs = UserDarPrivilegioLevel(UserName)
					
					If Not privs And Declaraciones.PlayerType.User Then
						Call WriteConsoleMsg(UserIndex, "No puedes advertir a administradores.", FontTypeNames.FONTTYPE_INFO)
					Else
						If (migr_InStrB(UserName, "\") <> 0) Then
							UserName = Replace(UserName, "\", "")
						End If
						If (migr_InStrB(UserName, "/") <> 0) Then
							UserName = Replace(UserName, "/", "")
						End If
						
						If FileExist(CharPath & UserName & ".chr") Then
							Count = Val(GetVar(CharPath & UserName & ".chr", "PENAS", "Cant"))
							Call WriteVar(CharPath & UserName & ".chr", "PENAS", "Cant", CStr(Count + 1))
							Call WriteVar(CharPath & UserName & ".chr", "PENAS", "P" & Count + 1, LCase(.name) & ": ADVERTENCIA por: " & LCase(reason) & " " & Today & " " & TimeOfDay)
							
							Call WriteConsoleMsg(UserIndex, "Has advertido a " & UCase(UserName) & ".", FontTypeNames.FONTTYPE_INFO)
							Call LogGM(.name, " advirtio a " & UserName)
						End If
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "EditChar" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleEditChar(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 11/06/2009
		'02/03/2009: ZaMa - Cuando editas nivel, chequea si el pj puede permanecer en clan faccionario
		'11/06/2009: ZaMa - Todos los comandos se pueden usar aunque el pj este offline
		'***************************************************
		If UserList(UserIndex).incomingData.length < 8 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		Dim opcion As Byte
		Dim Arg1 As String
		Dim Arg2 As String
		Dim valido As Boolean
		Dim LoopC As Byte
		Dim CommandString As String
		Dim N As Byte
		Dim UserCharPath As String
		Dim Var As Integer
		Dim GI As Short
		Dim Sex As Byte
		Dim raza As Byte
		Dim bankGold As Integer
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			
			UserName = Replace(buffer.ReadASCIIString(), "+", " ")
			
			If UCase(UserName) = "YO" Then
				tUser = UserIndex
			Else
				tUser = NameIndex(UserName)
			End If
			
			opcion = buffer.ReadByte()
			Arg1 = buffer.ReadASCIIString()
			Arg2 = buffer.ReadASCIIString()
			
			If .flags.Privilegios And Declaraciones.PlayerType.RoleMaster Then
				Select Case .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.Consejero)
					Case Declaraciones.PlayerType.Consejero
						' Los RMs consejeros sólo se pueden editar su head, body y level
						valido = tUser = UserIndex And (opcion = eEditOptions.eo_Body Or opcion = eEditOptions.eo_Head Or opcion = eEditOptions.eo_Level)
						
					Case Declaraciones.PlayerType.SemiDios
						' Los RMs sólo se pueden editar su level y el head y body de cualquiera
						valido = (opcion = eEditOptions.eo_Level And tUser = UserIndex) Or opcion = eEditOptions.eo_Body Or opcion = eEditOptions.eo_Head
						
					Case Declaraciones.PlayerType.Dios
						' Los DRMs pueden aplicar los siguientes comandos sobre cualquiera
						' pero si quiere modificar el level sólo lo puede hacer sobre sí mismo
						valido = (opcion = eEditOptions.eo_Level And tUser = UserIndex) Or opcion = eEditOptions.eo_Body Or opcion = eEditOptions.eo_Head Or opcion = eEditOptions.eo_CiticensKilled Or opcion = eEditOptions.eo_CriminalsKilled Or opcion = eEditOptions.eo_Class Or opcion = eEditOptions.eo_Skills Or opcion = eEditOptions.eo_addGold
				End Select
				
			ElseIf .flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios) Then  'Si no es RM debe ser dios para poder usar este comando
				valido = True
			End If
			
			If valido Then
				UserCharPath = CharPath & UserName & ".chr"
				If tUser <= 0 And Not FileExist(UserCharPath) Then
					Call WriteConsoleMsg(UserIndex, "Estás intentando editar un usuario inexistente.", FontTypeNames.FONTTYPE_INFO)
					Call LogGM(.name, "Intentó editar un usuario inexistente.")
				Else
					'For making the Log
					CommandString = "/MOD "
					
					Select Case opcion
						Case eEditOptions.eo_Gold
							If Val(Arg1) <= MAX_ORO_EDIT Then
								If tUser <= 0 Then ' Esta offline?
									Call WriteVar(UserCharPath, "STATS", "GLD", CStr(Val(Arg1)))
									Call WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName, FontTypeNames.FONTTYPE_INFO)
								Else ' Online
									UserList(tUser).Stats.GLD = Val(Arg1)
									Call WriteUpdateGold(tUser)
								End If
							Else
								Call WriteConsoleMsg(UserIndex, "No está permitido utilizar valores mayores a " & MAX_ORO_EDIT & ". Su comando ha quedado en los logs del juego.", FontTypeNames.FONTTYPE_INFO)
							End If
							
							' Log it
							CommandString = CommandString & "ORO "
							
						Case eEditOptions.eo_Experience
							If Val(Arg1) > 20000000 Then
								Arg1 = CStr(20000000)
							End If
							
							If tUser <= 0 Then ' Offline
								Var = CInt(GetVar(UserCharPath, "STATS", "EXP"))
								Call WriteVar(UserCharPath, "STATS", "EXP", CStr(Var + Val(Arg1)))
								Call WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName, FontTypeNames.FONTTYPE_INFO)
							Else ' Online
								UserList(tUser).Stats.Exp = UserList(tUser).Stats.Exp + Val(Arg1)
								Call CheckUserLevel(tUser)
								Call WriteUpdateExp(tUser)
							End If
							
							' Log it
							CommandString = CommandString & "EXP "
							
						Case eEditOptions.eo_Body
							If tUser <= 0 Then
								Call WriteVar(UserCharPath, "INIT", "Body", Arg1)
								Call WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName, FontTypeNames.FONTTYPE_INFO)
							Else
								Call ChangeUserChar(tUser, Val(Arg1), UserList(tUser).Char_Renamed.Head, UserList(tUser).Char_Renamed.heading, UserList(tUser).Char_Renamed.WeaponAnim, UserList(tUser).Char_Renamed.ShieldAnim, UserList(tUser).Char_Renamed.CascoAnim)
							End If
							
							' Log it
							CommandString = CommandString & "BODY "
							
						Case eEditOptions.eo_Head
							If tUser <= 0 Then
								Call WriteVar(UserCharPath, "INIT", "Head", Arg1)
								Call WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName, FontTypeNames.FONTTYPE_INFO)
							Else
								Call ChangeUserChar(tUser, UserList(tUser).Char_Renamed.body, Val(Arg1), UserList(tUser).Char_Renamed.heading, UserList(tUser).Char_Renamed.WeaponAnim, UserList(tUser).Char_Renamed.ShieldAnim, UserList(tUser).Char_Renamed.CascoAnim)
							End If
							
							' Log it
							CommandString = CommandString & "HEAD "
							
						Case eEditOptions.eo_CriminalsKilled
							Var = IIf(Val(Arg1) > MAXUSERMATADOS, MAXUSERMATADOS, Val(Arg1))
							
							If tUser <= 0 Then ' Offline
								Call WriteVar(UserCharPath, "FACCIONES", "CrimMatados", CStr(Var))
								Call WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName, FontTypeNames.FONTTYPE_INFO)
							Else ' Online
								UserList(tUser).Faccion.CriminalesMatados = Var
							End If
							
							' Log it
							CommandString = CommandString & "CRI "
							
						Case eEditOptions.eo_CiticensKilled
							Var = IIf(Val(Arg1) > MAXUSERMATADOS, MAXUSERMATADOS, Val(Arg1))
							
							If tUser <= 0 Then ' Offline
								Call WriteVar(UserCharPath, "FACCIONES", "CiudMatados", CStr(Var))
								Call WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName, FontTypeNames.FONTTYPE_INFO)
							Else ' Online
								UserList(tUser).Faccion.CiudadanosMatados = Var
							End If
							
							' Log it
							CommandString = CommandString & "CIU "
							
						Case eEditOptions.eo_Level
							If Val(Arg1) > STAT_MAXELV Then
								Arg1 = CStr(STAT_MAXELV)
								Call WriteConsoleMsg(UserIndex, "No puedes tener un nivel superior a " & STAT_MAXELV & ".", FontTypeNames.FONTTYPE_INFO)
							End If
							
							' Chequeamos si puede permanecer en el clan
							If Val(Arg1) >= 25 Then
								
								If tUser <= 0 Then
									GI = CShort(GetVar(UserCharPath, "GUILD", "GUILDINDEX"))
								Else
									GI = UserList(tUser).GuildIndex
								End If
								
								If GI > 0 Then
									If modGuilds.GuildAlignment(GI) = "Del Mal" Or modGuilds.GuildAlignment(GI) = "Real" Then
										'We get here, so guild has factionary alignment, we have to expulse the user
										Call modGuilds.m_EcharMiembroDeClan(-1, UserName)
										
										Call SendData(modSendData.SendTarget.ToGuildMembers, GI, PrepareMessageConsoleMsg(UserName & " deja el clan.", FontTypeNames.FONTTYPE_GUILD))
										' Si esta online le avisamos
										If tUser > 0 Then Call WriteConsoleMsg(tUser, "¡Ya tienes la madurez suficiente como para decidir bajo que estandarte pelearás! Por esta razón, hasta tanto no te enlistes en la facción bajo la cual tu clan está alineado, estarás excluído del mismo.", FontTypeNames.FONTTYPE_GUILD)
									End If
								End If
							End If
							
							If tUser <= 0 Then ' Offline
								Call WriteVar(UserCharPath, "STATS", "ELV", CStr(Val(Arg1)))
								Call WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName, FontTypeNames.FONTTYPE_INFO)
							Else ' Online
								UserList(tUser).Stats.ELV = Val(Arg1)
								Call WriteUpdateUserStats(tUser)
							End If
							
							' Log it
							CommandString = CommandString & "LEVEL "
							
						Case eEditOptions.eo_Class
							For LoopC = 1 To NUMCLASES
								If UCase(ListaClases(LoopC)) = UCase(Arg1) Then Exit For
							Next LoopC
							
							If LoopC > NUMCLASES Then
								Call WriteConsoleMsg(UserIndex, "Clase desconocida. Intente nuevamente.", FontTypeNames.FONTTYPE_INFO)
							Else
								If tUser <= 0 Then ' Offline
									Call WriteVar(UserCharPath, "INIT", "Clase", CStr(LoopC))
									Call WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName, FontTypeNames.FONTTYPE_INFO)
								Else ' Online
									UserList(tUser).clase = LoopC
								End If
							End If
							
							' Log it
							CommandString = CommandString & "CLASE "
							
						Case eEditOptions.eo_Skills
							For LoopC = 1 To NUMSKILLS
								If UCase(Replace(SkillsNames(LoopC), " ", "+")) = UCase(Arg1) Then Exit For
							Next LoopC
							
							If LoopC > NUMSKILLS Then
								Call WriteConsoleMsg(UserIndex, "Skill Inexistente!", FontTypeNames.FONTTYPE_INFO)
							Else
								If tUser <= 0 Then ' Offline
									Call WriteVar(UserCharPath, "Skills", "SK" & LoopC, Arg2)
									Call WriteVar(UserCharPath, "Skills", "EXPSK" & LoopC, CStr(0))
									
									If CDbl(Arg2) < MAXSKILLPOINTS Then
										Call WriteVar(UserCharPath, "Skills", "ELUSK" & LoopC, CStr(ELU_SKILL_INICIAL * 1.05 ^ CDbl(Arg2)))
									Else
										Call WriteVar(UserCharPath, "Skills", "ELUSK" & LoopC, CStr(0))
									End If
									
									Call WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName, FontTypeNames.FONTTYPE_INFO)
								Else ' Online
									UserList(tUser).Stats.UserSkills(LoopC) = Val(Arg2)
									Call CheckEluSkill(tUser, LoopC, True)
								End If
							End If
							
							' Log it
							CommandString = CommandString & "SKILLS "
							
						Case eEditOptions.eo_SkillPointsLeft
							If tUser <= 0 Then ' Offline
								Call WriteVar(UserCharPath, "STATS", "SkillPtsLibres", Arg1)
								Call WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName, FontTypeNames.FONTTYPE_INFO)
							Else ' Online
								UserList(tUser).Stats.SkillPts = Val(Arg1)
							End If
							
							' Log it
							CommandString = CommandString & "SKILLSLIBRES "
							
						Case eEditOptions.eo_Nobleza
							Var = IIf(Val(Arg1) > MAXREP, MAXREP, Val(Arg1))
							
							If tUser <= 0 Then ' Offline
								Call WriteVar(UserCharPath, "REP", "Nobles", CStr(Var))
								Call WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName, FontTypeNames.FONTTYPE_INFO)
							Else ' Online
								UserList(tUser).Reputacion.NobleRep = Var
							End If
							
							' Log it
							CommandString = CommandString & "NOB "
							
						Case eEditOptions.eo_Asesino
							Var = IIf(Val(Arg1) > MAXREP, MAXREP, Val(Arg1))
							
							If tUser <= 0 Then ' Offline
								Call WriteVar(UserCharPath, "REP", "Asesino", CStr(Var))
								Call WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName, FontTypeNames.FONTTYPE_INFO)
							Else ' Online
								UserList(tUser).Reputacion.AsesinoRep = Var
							End If
							
							' Log it
							CommandString = CommandString & "ASE "
							
						Case eEditOptions.eo_Sex
							Sex = IIf(UCase(Arg1) = "MUJER", Declaraciones.eGenero.Mujer, 0) ' Mujer?
							Sex = IIf(UCase(Arg1) = "HOMBRE", Declaraciones.eGenero.Hombre, Sex) ' Hombre?
							
							If Sex <> 0 Then ' Es Hombre o mujer?
								If tUser <= 0 Then ' OffLine
									Call WriteVar(UserCharPath, "INIT", "Genero", CStr(Sex))
									Call WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName, FontTypeNames.FONTTYPE_INFO)
								Else ' Online
									UserList(tUser).Genero = Sex
								End If
							Else
								Call WriteConsoleMsg(UserIndex, "Genero desconocido. Intente nuevamente.", FontTypeNames.FONTTYPE_INFO)
							End If
							
							' Log it
							CommandString = CommandString & "SEX "
							
						Case eEditOptions.eo_Raza
							
							Arg1 = UCase(Arg1)
							Select Case Arg1
								Case "HUMANO"
									raza = Declaraciones.eRaza.Humano
								Case "ELFO"
									raza = Declaraciones.eRaza.Elfo
								Case "DROW"
									raza = Declaraciones.eRaza.Drow
								Case "ENANO"
									raza = Declaraciones.eRaza.Enano
								Case "GNOMO"
									raza = Declaraciones.eRaza.Gnomo
								Case Else
									raza = 0
							End Select
							
							
							If raza = 0 Then
								Call WriteConsoleMsg(UserIndex, "Raza desconocida. Intente nuevamente.", FontTypeNames.FONTTYPE_INFO)
							Else
								If tUser <= 0 Then
									Call WriteVar(UserCharPath, "INIT", "Raza", CStr(raza))
									Call WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName, FontTypeNames.FONTTYPE_INFO)
								Else
									UserList(tUser).raza = raza
								End If
							End If
							
							' Log it
							CommandString = CommandString & "RAZA "
							
						Case eEditOptions.eo_addGold
							
							
							If System.Math.Abs(CDbl(Arg1)) > MAX_ORO_EDIT Then
								Call WriteConsoleMsg(UserIndex, "No está permitido utilizar valores mayores a " & MAX_ORO_EDIT & ".", FontTypeNames.FONTTYPE_INFO)
							Else
								If tUser <= 0 Then
									bankGold = CInt(GetVar(CharPath & UserName & ".chr", "STATS", "BANCO"))
									Call WriteVar(UserCharPath, "STATS", "BANCO", IIf(bankGold + Val(Arg1) <= 0, 0, bankGold + Val(Arg1)))
									Call WriteConsoleMsg(UserIndex, "Se le ha agregado " & Arg1 & " monedas de oro a " & UserName & ".", FontTypeNames.FONTTYPE_TALK)
								Else
									UserList(tUser).Stats.Banco = IIf(UserList(tUser).Stats.Banco + Val(Arg1) <= 0, 0, UserList(tUser).Stats.Banco + Val(Arg1))
									Call WriteConsoleMsg(tUser, STANDARD_BOUNTY_HUNTER_MESSAGE, FontTypeNames.FONTTYPE_TALK)
								End If
							End If
							
							' Log it
							CommandString = CommandString & "AGREGAR "
							
						Case Else
							Call WriteConsoleMsg(UserIndex, "Comando no permitido.", FontTypeNames.FONTTYPE_INFO)
							CommandString = CommandString & "UNKOWN "
							
					End Select
					
					CommandString = CommandString & Arg1 & " " & Arg2
					Call LogGM(.name, CommandString & " " & UserName)
					
				End If
			End If
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	
	''
	' Handles the "RequestCharInfo" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleRequestCharInfo(ByVal UserIndex As Short)
		'***************************************************
		'Author: Fredy Horacio Treboux (liquid)
		'Last Modification: 01/08/07
		'Last Modification by: (liquid).. alto bug zapallo..
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim targetName As String
		Dim TargetIndex As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			targetName = Replace(buffer.ReadASCIIString(), "+", " ")
			TargetIndex = NameIndex(targetName)
			
			
			If .flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios) Then
				'is the player offline?
				If TargetIndex <= 0 Then
					'don't allow to retrieve administrator's info
					If Not (EsDios(targetName) Or EsAdmin(targetName)) Then
						Call WriteConsoleMsg(UserIndex, "Usuario offline, buscando en charfile.", FontTypeNames.FONTTYPE_INFO)
						Call SendUserStatsTxtOFF(UserIndex, targetName)
					End If
				Else
					'don't allow to retrieve administrator's info
					If UserList(TargetIndex).flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then
						Call SendUserStatsTxt(UserIndex, TargetIndex)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "RequestCharStats" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleRequestCharStats(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			UserName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) <> 0 Then
				Call LogGM(.name, "/STAT " & UserName)
				
				tUser = NameIndex(UserName)
				
				If tUser <= 0 Then
					Call WriteConsoleMsg(UserIndex, "Usuario offline. Leyendo charfile... ", FontTypeNames.FONTTYPE_INFO)
					
					Call SendUserMiniStatsTxtFromChar(UserIndex, UserName)
				Else
					Call SendUserMiniStatsTxt(UserIndex, tUser)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "RequestCharGold" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleRequestCharGold(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			tUser = NameIndex(UserName)
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) Then
				Call LogGM(.name, "/BAL " & UserName)
				
				If tUser <= 0 Then
					Call WriteConsoleMsg(UserIndex, "Usuario offline. Leyendo charfile... ", FontTypeNames.FONTTYPE_TALK)
					
					Call SendUserOROTxtFromChar(UserIndex, UserName)
				Else
					Call WriteConsoleMsg(UserIndex, "El usuario " & UserName & " tiene " & UserList(tUser).Stats.Banco & " en el banco.", FontTypeNames.FONTTYPE_TALK)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "RequestCharInventory" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleRequestCharInventory(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			tUser = NameIndex(UserName)
			
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) Then
				Call LogGM(.name, "/INV " & UserName)
				
				If tUser <= 0 Then
					Call WriteConsoleMsg(UserIndex, "Usuario offline. Leyendo del charfile...", FontTypeNames.FONTTYPE_TALK)
					
					Call SendUserInvTxtFromChar(UserIndex, UserName)
				Else
					Call SendUserInvTxt(UserIndex, tUser)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "RequestCharBank" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleRequestCharBank(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			tUser = NameIndex(UserName)
			
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) Then
				Call LogGM(.name, "/BOV " & UserName)
				
				If tUser <= 0 Then
					Call WriteConsoleMsg(UserIndex, "Usuario offline. Leyendo charfile... ", FontTypeNames.FONTTYPE_TALK)
					
					Call SendUserBovedaTxtFromChar(UserIndex, UserName)
				Else
					Call SendUserBovedaTxt(UserIndex, tUser)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "RequestCharSkills" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleRequestCharSkills(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		Dim LoopC As Integer
		Dim message As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			tUser = NameIndex(UserName)
			
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) Then
				Call LogGM(.name, "/STATS " & UserName)
				
				If tUser <= 0 Then
					If (migr_InStrB(UserName, "\") <> 0) Then
						UserName = Replace(UserName, "\", "")
					End If
					If (migr_InStrB(UserName, "/") <> 0) Then
						UserName = Replace(UserName, "/", "")
					End If
					
					For LoopC = 1 To NUMSKILLS
						message = message & "CHAR>" & SkillsNames(LoopC) & " = " & GetVar(CharPath & UserName & ".chr", "SKILLS", "SK" & LoopC) & vbCrLf
					Next LoopC
					
					Call WriteConsoleMsg(UserIndex, message & "CHAR> Libres:" & GetVar(CharPath & UserName & ".chr", "STATS", "SKILLPTSLIBRES"), FontTypeNames.FONTTYPE_INFO)
				Else
					Call SendUserSkillsTxt(UserIndex, tUser)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "ReviveChar" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleReviveChar(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 11/03/2010
		'11/03/2010: ZaMa - Al revivir con el comando, si esta navegando le da cuerpo e barca.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		Dim LoopC As Byte
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) Then
				If UCase(UserName) <> "YO" Then
					tUser = NameIndex(UserName)
				Else
					tUser = UserIndex
				End If
				
				If tUser <= 0 Then
					Call WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO)
				Else
					With UserList(tUser)
						'If dead, show him alive (naked).
						If .flags.Muerto = 1 Then
							.flags.Muerto = 0
							
							If .flags.Navegando = 1 Then
								Call ToogleBoatBody(UserIndex)
							Else
								Call DarCuerpoDesnudo(tUser)
							End If
							
							If .flags.Traveling = 1 Then
								.flags.Traveling = 0
								.Counters.goHome = 0
								Call WriteMultiMessage(tUser, Declaraciones.eMessages.CancelHome)
							End If
							
							Call ChangeUserChar(tUser, .Char_Renamed.body, .OrigChar.Head, .Char_Renamed.heading, .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim, .Char_Renamed.CascoAnim)
							
							Call WriteConsoleMsg(tUser, UserList(UserIndex).name & " te ha resucitado.", FontTypeNames.FONTTYPE_INFO)
						Else
							Call WriteConsoleMsg(tUser, UserList(UserIndex).name & " te ha curado.", FontTypeNames.FONTTYPE_INFO)
						End If
						
						.Stats.MinHp = .Stats.MaxHp
						
						If .flags.Traveling = 1 Then
							.Counters.goHome = 0
							.flags.Traveling = 0
							Call WriteMultiMessage(tUser, Declaraciones.eMessages.CancelHome)
						End If
						
					End With
					
					Call WriteUpdateHP(tUser)
					
					Call FlushBuffer(tUser)
					
					Call LogGM(.name, "Resucito a " & UserName)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "OnlineGM" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleOnlineGM(ByVal UserIndex As Short)
		'***************************************************
		'Author: Fredy Horacio Treboux (liquid)
		'Last Modification: 12/28/06
		'
		'***************************************************
		Dim i As Integer
		Dim list As String
		Dim priv As Declaraciones.PlayerType
		
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero) Then Exit Sub
			
			priv = Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios
			If .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin) Then priv = priv Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin
			
			For i = 1 To LastUser
				If UserList(i).flags.UserLogged Then
					If UserList(i).flags.Privilegios And priv Then list = list & UserList(i).name & ", "
				End If
			Next i
			
			If migr_LenB(list) <> 0 Then
				list = Left(list, Len(list) - 2)
				Call WriteConsoleMsg(UserIndex, list & ".", FontTypeNames.FONTTYPE_INFO)
			Else
				Call WriteConsoleMsg(UserIndex, "No hay GMs Online.", FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub
	
	''
	' Handles the "OnlineMap" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleOnlineMap(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 23/03/2009
		'23/03/2009: ZaMa - Ahora no requiere estar en el mapa, sino que por defecto se toma en el que esta, pero se puede especificar otro
		'***************************************************
		Dim Map As Short
		Dim LoopC As Integer
		Dim list As String
		Dim priv As Declaraciones.PlayerType
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			Map = .incomingData.ReadInteger
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero) Then Exit Sub
			
			
			priv = Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios
			If .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin) Then priv = priv + CShort(Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin)
			
			For LoopC = 1 To LastUser
				If migr_LenB(UserList(LoopC).name) <> 0 And UserList(LoopC).Pos.Map = Map Then
					If UserList(LoopC).flags.Privilegios And priv Then list = list & UserList(LoopC).name & ", "
				End If
			Next LoopC
			
			If Len(list) > 2 Then list = Left(list, Len(list) - 2)
			
			Call WriteConsoleMsg(UserIndex, "Usuarios en el mapa: " & list, FontTypeNames.FONTTYPE_INFO)
		End With
	End Sub
	
	''
	' Handles the "Forgive" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleForgive(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) <> 0 Then
				tUser = NameIndex(UserName)
				
				If tUser > 0 Then
					If EsNewbie(tUser) Then
						Call VolverCiudadano(tUser)
					Else
						Call LogGM(.name, "Intento perdonar un personaje de nivel avanzado.")
						Call WriteConsoleMsg(UserIndex, "Sólo se permite perdonar newbies.", FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "Kick" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleKick(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		Dim rank As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			rank = Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.Consejero
			
			UserName = buffer.ReadASCIIString()
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) Then
				tUser = NameIndex(UserName)
				
				If tUser <= 0 Then
					Call WriteConsoleMsg(UserIndex, "El usuario no está online.", FontTypeNames.FONTTYPE_INFO)
				Else
					If (UserList(tUser).flags.Privilegios And rank) > (.flags.Privilegios And rank) Then
						Call WriteConsoleMsg(UserIndex, "No puedes echar a alguien con jerarquía mayor a la tuya.", FontTypeNames.FONTTYPE_INFO)
					Else
						Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg(.name & " echó a " & UserName & ".", FontTypeNames.FONTTYPE_INFO))
						Call CloseSocket(tUser)
						Call LogGM(.name, "Echó a " & UserName)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "Execute" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleExecute(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) <> 0 Then
				tUser = NameIndex(UserName)
				
				If tUser > 0 Then
					If Not UserList(tUser).flags.Privilegios And Declaraciones.PlayerType.User Then
						Call WriteConsoleMsg(UserIndex, "¿¿Estás loco?? ¿¿Cómo vas a piñatear un gm?? :@", FontTypeNames.FONTTYPE_INFO)
					Else
						Call UserDie(tUser)
						Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg(.name & " ha ejecutado a " & UserName & ".", FontTypeNames.FONTTYPE_EJECUCION))
						Call LogGM(.name, " ejecuto a " & UserName)
					End If
				Else
					Call WriteConsoleMsg(UserIndex, "No está online.", FontTypeNames.FONTTYPE_INFO)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "BanChar" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleBanChar(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim reason As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			reason = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) <> 0 Then
				Call BanCharacter(UserIndex, UserName, reason)
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "UnbanChar" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleUnbanChar(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim cantPenas As Byte
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) <> 0 Then
				If (migr_InStrB(UserName, "\") <> 0) Then
					UserName = Replace(UserName, "\", "")
				End If
				If (migr_InStrB(UserName, "/") <> 0) Then
					UserName = Replace(UserName, "/", "")
				End If
				
				If Not FileExist(CharPath & UserName & ".chr") Then
					Call WriteConsoleMsg(UserIndex, "Charfile inexistente (no use +).", FontTypeNames.FONTTYPE_INFO)
				Else
					If (Val(GetVar(CharPath & UserName & ".chr", "FLAGS", "Ban")) = 1) Then
						Call UnBan(UserName)
						
						'penas
						cantPenas = Val(GetVar(CharPath & UserName & ".chr", "PENAS", "Cant"))
						Call WriteVar(CharPath & UserName & ".chr", "PENAS", "Cant", CStr(cantPenas + 1))
						Call WriteVar(CharPath & UserName & ".chr", "PENAS", "P" & cantPenas + 1, LCase(.name) & ": UNBAN. " & Today & " " & TimeOfDay)
						
						Call LogGM(.name, "/UNBAN a " & UserName)
						Call WriteConsoleMsg(UserIndex, UserName & " unbanned.", FontTypeNames.FONTTYPE_INFO)
					Else
						Call WriteConsoleMsg(UserIndex, UserName & " no está baneado. Imposible unbanear.", FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "NPCFollow" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleNPCFollow(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero) Then Exit Sub
			
			If .flags.TargetNPC > 0 Then
				Call DoFollow(.flags.TargetNPC, .name)
				Npclist(.flags.TargetNPC).flags.Inmovilizado = 0
				Npclist(.flags.TargetNPC).flags.Paralizado = 0
				Npclist(.flags.TargetNPC).Contadores.Paralisis = 0
			End If
		End With
	End Sub
	
	''
	' Handles the "SummonChar" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleSummonChar(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 26/03/2009
		'26/03/2009: ZaMa - Chequeo que no se teletransporte donde haya un char o npc
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		Dim X As Short
		Dim Y As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) Then
				tUser = NameIndex(UserName)
				
				If tUser <= 0 Then
					Call WriteConsoleMsg(UserIndex, "El jugador no está online.", FontTypeNames.FONTTYPE_INFO)
				Else
					If (.flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin)) <> 0 Or (UserList(tUser).flags.Privilegios And (Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.User)) <> 0 Then
						Call WriteConsoleMsg(tUser, .name & " te ha trasportado.", FontTypeNames.FONTTYPE_INFO)
						X = .Pos.X
						Y = .Pos.Y + 1
						Call FindLegalPos(tUser, .Pos.Map, X, Y)
						Call WarpUserChar(tUser, .Pos.Map, X, Y, True, True)
						Call LogGM(.name, "/SUM " & UserName & " Map:" & .Pos.Map & " X:" & .Pos.X & " Y:" & .Pos.Y)
					Else
						Call WriteConsoleMsg(UserIndex, "No puedes invocar a dioses y admins.", FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "SpawnListRequest" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleSpawnListRequest(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero) Then Exit Sub
			
			Call EnviarSpawnList(UserIndex)
		End With
	End Sub
	
	''
	' Handles the "SpawnCreature" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleSpawnCreature(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		'UPGRADE_NOTE: npc se actualizó a npc_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim npc_Renamed As Short
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			npc_Renamed = .incomingData.ReadInteger()
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) Then
				If npc_Renamed > 0 And npc_Renamed <= UBound(Declaraciones.SpawnList) Then Call SpawnNpc(Declaraciones.SpawnList(npc_Renamed).NpcIndex, .Pos, True, False)
				
				Call LogGM(.name, "Sumoneo " & Declaraciones.SpawnList(npc_Renamed).NpcName)
			End If
		End With
	End Sub
	
	''
	' Handles the "ResetNPCInventory" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleResetNPCInventory(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			If .flags.TargetNPC = 0 Then Exit Sub
			
			Call ResetNpcInv(.flags.TargetNPC)
			Call LogGM(.name, "/RESETINV " & Npclist(.flags.TargetNPC).name)
		End With
	End Sub
	
	''
	' Handles the "CleanWorld" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleCleanWorld(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Call LimpiarMundo()
		End With
	End Sub
	
	''
	' Handles the "ServerMessage" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleServerMessage(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim message As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			message = buffer.ReadASCIIString()
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) Then
				If migr_LenB(message) <> 0 Then
					Call LogGM(.name, "Mensaje Broadcast:" & message)
					Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg(UserList(UserIndex).name & "> " & message, FontTypeNames.FONTTYPE_TALK))
					''''''''''''''''SOLO PARA EL TESTEO'''''''
					''''''''''SE USA PARA COMUNICARSE CON EL SERVER'''''''''''
					frmMain.txtChat.Text = frmMain.txtChat.Text & vbNewLine & UserList(UserIndex).name & " > " & message
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "NickToIP" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleNickToIP(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 24/07/07
		'Pablo (ToxicWaste): Agrego para uqe el /nick2ip tambien diga los nicks en esa ip por pedido de la DGM.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		Dim priv As Declaraciones.PlayerType
		Dim ip As String
		Dim lista As String
		Dim LoopC As Integer
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) <> 0 Then
				tUser = NameIndex(UserName)
				Call LogGM(.name, "NICK2IP Solicito la IP de " & UserName)
				
				If .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin) Then
					priv = Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin
				Else
					priv = Declaraciones.PlayerType.User
				End If
				
				If tUser > 0 Then
					If UserList(tUser).flags.Privilegios And priv Then
						Call WriteConsoleMsg(UserIndex, "El ip de " & UserName & " es " & UserList(tUser).ip, FontTypeNames.FONTTYPE_INFO)
						ip = UserList(tUser).ip
						For LoopC = 1 To LastUser
							If UserList(LoopC).ip = ip Then
								If migr_LenB(UserList(LoopC).name) <> 0 And UserList(LoopC).flags.UserLogged Then
									If UserList(LoopC).flags.Privilegios And priv Then
										lista = lista & UserList(LoopC).name & ", "
									End If
								End If
							End If
						Next LoopC
						If migr_LenB(lista) <> 0 Then lista = Left(lista, Len(lista) - 2)
						Call WriteConsoleMsg(UserIndex, "Los personajes con ip " & ip & " son: " & lista, FontTypeNames.FONTTYPE_INFO)
					End If
				Else
					Call WriteConsoleMsg(UserIndex, "No hay ningún personaje con ese nick.", FontTypeNames.FONTTYPE_INFO)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "IPToNick" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleIPToNick(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim ip As String
		Dim LoopC As Integer
		Dim lista As String
		Dim priv As Declaraciones.PlayerType
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			
			ip = .incomingData.ReadByte() & "."
			ip = ip & .incomingData.ReadByte() & "."
			ip = ip & .incomingData.ReadByte() & "."
			ip = ip & .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Call LogGM(.name, "IP2NICK Solicito los Nicks de IP " & ip)
			
			If .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin) Then
				priv = Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin
			Else
				priv = Declaraciones.PlayerType.User
			End If
			
			For LoopC = 1 To LastUser
				If UserList(LoopC).ip = ip Then
					If migr_LenB(UserList(LoopC).name) <> 0 And UserList(LoopC).flags.UserLogged Then
						If UserList(LoopC).flags.Privilegios And priv Then
							lista = lista & UserList(LoopC).name & ", "
						End If
					End If
				End If
			Next LoopC
			
			If migr_LenB(lista) <> 0 Then lista = Left(lista, Len(lista) - 2)
			Call WriteConsoleMsg(UserIndex, "Los personajes con ip " & ip & " son: " & lista, FontTypeNames.FONTTYPE_INFO)
		End With
	End Sub
	
	''
	' Handles the "GuildOnlineMembers" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleGuildOnlineMembers(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim GuildName As String
		Dim tGuild As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			GuildName = buffer.ReadASCIIString()
			
			If (migr_InStrB(GuildName, "+") <> 0) Then
				GuildName = Replace(GuildName, "+", " ")
			End If
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) <> 0 Then
				tGuild = GuildIndex(GuildName)
				
				If tGuild > 0 Then
					Call WriteConsoleMsg(UserIndex, "Clan " & UCase(GuildName) & ": " & modGuilds.m_ListaDeMiembrosOnline(UserIndex, tGuild), FontTypeNames.FONTTYPE_GUILDMSG)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "TeleportCreate" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleTeleportCreate(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 22/03/2010
		'15/11/2009: ZaMa - Ahora se crea un teleport con un radio especificado.
		'22/03/2010: ZaMa - Harcodeo los teleps y radios en el dat, para evitar mapas bugueados.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 6 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim mapa As Short
		Dim X As Byte
		Dim Y As Byte
		Dim Radio As Byte
		Dim ET As Obj
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			
			mapa = .incomingData.ReadInteger()
			X = .incomingData.ReadByte()
			Y = .incomingData.ReadByte()
			Radio = .incomingData.ReadByte()
			
			Radio = MinimoInt(Radio, 6)
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			
			Call LogGM(.name, "/CT " & mapa & "," & X & "," & Y & "," & Radio)
			
			If Not MapaValido(mapa) Or Not InMapBounds(mapa, X, Y) Then Exit Sub
			
			If MapData(.Pos.Map, .Pos.X, .Pos.Y - 1).ObjInfo.ObjIndex > 0 Then Exit Sub
			
			If MapData(.Pos.Map, .Pos.X, .Pos.Y - 1).TileExit.Map > 0 Then Exit Sub
			
			If MapData(mapa, X, Y).ObjInfo.ObjIndex > 0 Then
				Call WriteConsoleMsg(UserIndex, "Hay un objeto en el piso en ese lugar.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If
			
			If MapData(mapa, X, Y).TileExit.Map > 0 Then
				Call WriteConsoleMsg(UserIndex, "No puedes crear un teleport que apunte a la entrada de otro.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If
			
			ET.Amount = 1
			' Es el numero en el dat. El indice es el comienzo + el radio, todo harcodeado :(.
			ET.ObjIndex = TELEP_OBJ_INDEX + Radio
			
			With MapData(.Pos.Map, .Pos.X, .Pos.Y - 1)
				.TileExit.Map = mapa
				.TileExit.X = X
				.TileExit.Y = Y
			End With
			
			Call MakeObj(ET, .Pos.Map, .Pos.X, .Pos.Y - 1)
		End With
	End Sub
	
	''
	' Handles the "TeleportDestroy" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleTeleportDestroy(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		Dim mapa As Short
		Dim X As Byte
		Dim Y As Byte
		With UserList(UserIndex)
			
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			'/dt
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			
			mapa = .flags.TargetMap
			X = .flags.TargetX
			Y = .flags.TargetY
			
			If Not InMapBounds(mapa, X, Y) Then Exit Sub
			
			With MapData(mapa, X, Y)
				If .ObjInfo.ObjIndex = 0 Then Exit Sub
				
				If ObjData_Renamed(.ObjInfo.ObjIndex).OBJType = Declaraciones.eOBJType.otTeleport And .TileExit.Map > 0 Then
					Call LogGM(UserList(UserIndex).name, "/DT: " & mapa & "," & X & "," & Y)
					
					Call EraseObj(.ObjInfo.Amount, mapa, X, Y)
					
					If MapData(.TileExit.Map, .TileExit.X, .TileExit.Y).ObjInfo.ObjIndex = 651 Then
						Call EraseObj(1, .TileExit.Map, .TileExit.X, .TileExit.Y)
					End If
					
					.TileExit.Map = 0
					.TileExit.X = 0
					.TileExit.Y = 0
				End If
			End With
		End With
	End Sub
	
	''
	' Handles the "RainToggle" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleRainToggle(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero) Then Exit Sub
			
			Call LogGM(.name, "/LLUVIA")
			Lloviendo = Not Lloviendo
			
			Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageRainToggle())
		End With
	End Sub
	
	''
	' Handles the "SetCharDescription" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleSetCharDescription(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim tUser As Short
		Dim desc As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			desc = buffer.ReadASCIIString()
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin)) <> 0 Or (.flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 Then
				tUser = .flags.TargetUser
				If tUser > 0 Then
					UserList(tUser).DescRM = desc
				Else
					Call WriteConsoleMsg(UserIndex, "Haz click sobre un personaje antes.", FontTypeNames.FONTTYPE_INFO)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "ForceMIDIToMap" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HanldeForceMIDIToMap(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 4 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim midiID As Byte
		Dim mapa As Short
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			
			midiID = .incomingData.ReadByte
			mapa = .incomingData.ReadInteger
			
			'Solo dioses, admins y RMS
			If .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.RoleMaster) Then
				'Si el mapa no fue enviado tomo el actual
				If Not InMapBounds(mapa, 50, 50) Then
					mapa = .Pos.Map
				End If
				
				If midiID = 0 Then
					'Ponemos el default del mapa
					Call SendData(modSendData.SendTarget.toMap, mapa, PrepareMessagePlayMidi(CByte(MapInfo_Renamed(.Pos.Map).Music)))
				Else
					'Ponemos el pedido por el GM
					Call SendData(modSendData.SendTarget.toMap, mapa, PrepareMessagePlayMidi(midiID))
				End If
			End If
		End With
	End Sub
	
	''
	' Handles the "ForceWAVEToMap" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleForceWAVEToMap(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 6 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim waveID As Byte
		Dim mapa As Short
		Dim X As Byte
		Dim Y As Byte
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			
			waveID = .incomingData.ReadByte()
			mapa = .incomingData.ReadInteger()
			X = .incomingData.ReadByte()
			Y = .incomingData.ReadByte()
			
			'Solo dioses, admins y RMS
			If .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.RoleMaster) Then
				'Si el mapa no fue enviado tomo el actual
				If Not InMapBounds(mapa, X, Y) Then
					mapa = .Pos.Map
					X = .Pos.X
					Y = .Pos.Y
				End If
				
				'Ponemos el pedido por el GM
				Call SendData(modSendData.SendTarget.toMap, mapa, PrepareMessagePlayWave(waveID, X, Y))
			End If
		End With
	End Sub
	
	''
	' Handles the "RoyalArmyMessage" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleRoyalArmyMessage(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim message As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			message = buffer.ReadASCIIString()
			
			'Solo dioses, admins y RMS
			If .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.RoleMaster) Then
				Call SendData(modSendData.SendTarget.ToRealYRMs, 0, PrepareMessageConsoleMsg("EJÉRCITO REAL> " & message, FontTypeNames.FONTTYPE_TALK))
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "ChaosLegionMessage" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleChaosLegionMessage(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim message As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			message = buffer.ReadASCIIString()
			
			'Solo dioses, admins y RMS
			If .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.RoleMaster) Then
				Call SendData(modSendData.SendTarget.ToCaosYRMs, 0, PrepareMessageConsoleMsg("FUERZAS DEL CAOS> " & message, FontTypeNames.FONTTYPE_TALK))
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "CitizenMessage" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleCitizenMessage(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim message As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			message = buffer.ReadASCIIString()
			
			'Solo dioses, admins y RMS
			If .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.RoleMaster) Then
				Call SendData(modSendData.SendTarget.ToCiudadanosYRMs, 0, PrepareMessageConsoleMsg("CIUDADANOS> " & message, FontTypeNames.FONTTYPE_TALK))
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "CriminalMessage" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleCriminalMessage(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim message As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			message = buffer.ReadASCIIString()
			
			'Solo dioses, admins y RMS
			If .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.RoleMaster) Then
				Call SendData(modSendData.SendTarget.ToCriminalesYRMs, 0, PrepareMessageConsoleMsg("CRIMINALES> " & message, FontTypeNames.FONTTYPE_TALK))
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "TalkAsNPC" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleTalkAsNPC(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/29/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim message As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			message = buffer.ReadASCIIString()
			
			'Solo dioses, admins y RMS
			If .flags.Privilegios And (Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.RoleMaster) Then
				'Asegurarse haya un NPC seleccionado
				If .flags.TargetNPC > 0 Then
					Call SendData(modSendData.SendTarget.ToNPCArea, .flags.TargetNPC, PrepareMessageChatOverHead(message, Npclist(.flags.TargetNPC).Char_Renamed.CharIndex, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)))
				Else
					Call WriteConsoleMsg(UserIndex, "Debes seleccionar el NPC por el que quieres hablar antes de usar este comando.", FontTypeNames.FONTTYPE_INFO)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "DestroyAllItemsInArea" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleDestroyAllItemsInArea(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		Dim X As Integer
		Dim Y As Integer
		Dim bIsExit As Boolean
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			
			
			For Y = .Pos.Y - MinYBorder + 1 To .Pos.Y + MinYBorder - 1
				For X = .Pos.X - MinXBorder + 1 To .Pos.X + MinXBorder - 1
					If X > 0 And Y > 0 And X < 101 And Y < 101 Then
						If MapData(.Pos.Map, X, Y).ObjInfo.ObjIndex > 0 Then
							bIsExit = MapData(.Pos.Map, X, Y).TileExit.Map > 0
							If ItemNoEsDeMapa(MapData(.Pos.Map, X, Y).ObjInfo.ObjIndex, bIsExit) Then
								Call EraseObj(MAX_INVENTORY_OBJS, .Pos.Map, X, Y)
							End If
						End If
					End If
				Next X
			Next Y
			
			Call LogGM(UserList(UserIndex).name, "/MASSDEST")
		End With
	End Sub
	
	''
	' Handles the "AcceptRoyalCouncilMember" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleAcceptRoyalCouncilMember(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		Dim LoopC As Byte
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) Then
				tUser = NameIndex(UserName)
				If tUser <= 0 Then
					Call WriteConsoleMsg(UserIndex, "Usuario offline", FontTypeNames.FONTTYPE_INFO)
				Else
					Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg(UserName & " fue aceptado en el honorable Consejo Real de Banderbill.", FontTypeNames.FONTTYPE_CONSEJO))
					With UserList(tUser)
						If .flags.Privilegios And Declaraciones.PlayerType.ChaosCouncil Then .flags.Privilegios = .flags.Privilegios - Declaraciones.PlayerType.ChaosCouncil
						If Not .flags.Privilegios And Declaraciones.PlayerType.RoyalCouncil Then .flags.Privilegios = .flags.Privilegios + Declaraciones.PlayerType.RoyalCouncil
						
						Call WarpUserChar(tUser, .Pos.Map, .Pos.X, .Pos.Y, False)
					End With
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "ChaosCouncilMember" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleAcceptChaosCouncilMember(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		Dim LoopC As Byte
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) Then
				tUser = NameIndex(UserName)
				If tUser <= 0 Then
					Call WriteConsoleMsg(UserIndex, "Usuario offline", FontTypeNames.FONTTYPE_INFO)
				Else
					Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg(UserName & " fue aceptado en el Concilio de las Sombras.", FontTypeNames.FONTTYPE_CONSEJO))
					
					With UserList(tUser)
						If .flags.Privilegios And Declaraciones.PlayerType.RoyalCouncil Then .flags.Privilegios = .flags.Privilegios - Declaraciones.PlayerType.RoyalCouncil
						If Not .flags.Privilegios And Declaraciones.PlayerType.ChaosCouncil Then .flags.Privilegios = .flags.Privilegios + Declaraciones.PlayerType.ChaosCouncil
						
						Call WarpUserChar(tUser, .Pos.Map, .Pos.X, .Pos.Y, False)
					End With
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "ItemsInTheFloor" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleItemsInTheFloor(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		Dim tObj As Short
		Dim lista As String
		Dim X As Integer
		Dim Y As Integer
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			
			
			For X = 5 To 95
				For Y = 5 To 95
					tObj = MapData(.Pos.Map, X, Y).ObjInfo.ObjIndex
					If tObj > 0 Then
						If ObjData_Renamed(tObj).OBJType <> Declaraciones.eOBJType.otArboles Then
							Call WriteConsoleMsg(UserIndex, "(" & X & "," & Y & ") " & ObjData_Renamed(tObj).name, FontTypeNames.FONTTYPE_INFO)
						End If
					End If
				Next Y
			Next X
		End With
	End Sub
	
	''
	' Handles the "MakeDumb" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleMakeDumb(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If ((.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) <> 0 Or ((.flags.Privilegios And (Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster)) = (Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster))) Then
				tUser = NameIndex(UserName)
				'para deteccion de aoice
				If tUser <= 0 Then
					Call WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO)
				Else
					Call WriteDumb(tUser)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "MakeDumbNoMore" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleMakeDumbNoMore(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If ((.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) <> 0 Or ((.flags.Privilegios And (Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster)) = (Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster))) Then
				tUser = NameIndex(UserName)
				'para deteccion de aoice
				If tUser <= 0 Then
					Call WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO)
				Else
					Call WriteDumbNoMore(tUser)
					Call FlushBuffer(tUser)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "DumpIPTables" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleDumpIPTables(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			
			Call SecurityIp.DumpTables()
		End With
	End Sub
	
	''
	' Handles the "CouncilKick" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleCouncilKick(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) <> 0 Then
				tUser = NameIndex(UserName)
				If tUser <= 0 Then
					If FileExist(CharPath & UserName & ".chr") Then
						Call WriteConsoleMsg(UserIndex, "Usuario offline, echando de los consejos.", FontTypeNames.FONTTYPE_INFO)
						Call WriteVar(CharPath & UserName & ".chr", "CONSEJO", "PERTENECE", CStr(0))
						Call WriteVar(CharPath & UserName & ".chr", "CONSEJO", "PERTENECECAOS", CStr(0))
					Else
						Call WriteConsoleMsg(UserIndex, "No se encuentra el charfile " & CharPath & UserName & ".chr", FontTypeNames.FONTTYPE_INFO)
					End If
				Else
					With UserList(tUser)
						If .flags.Privilegios And Declaraciones.PlayerType.RoyalCouncil Then
							Call WriteConsoleMsg(tUser, "Has sido echado del consejo de Banderbill.", FontTypeNames.FONTTYPE_TALK)
							.flags.Privilegios = .flags.Privilegios - Declaraciones.PlayerType.RoyalCouncil
							
							Call WarpUserChar(tUser, .Pos.Map, .Pos.X, .Pos.Y, False)
							Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg(UserName & " fue expulsado del consejo de Banderbill.", FontTypeNames.FONTTYPE_CONSEJO))
						End If
						
						If .flags.Privilegios And Declaraciones.PlayerType.ChaosCouncil Then
							Call WriteConsoleMsg(tUser, "Has sido echado del Concilio de las Sombras.", FontTypeNames.FONTTYPE_TALK)
							.flags.Privilegios = .flags.Privilegios - Declaraciones.PlayerType.ChaosCouncil
							
							Call WarpUserChar(tUser, .Pos.Map, .Pos.X, .Pos.Y, False)
							Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg(UserName & " fue expulsado del Concilio de las Sombras.", FontTypeNames.FONTTYPE_CONSEJO))
						End If
					End With
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "SetTrigger" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleSetTrigger(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim tTrigger As Byte
		Dim tLog As String
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			
			tTrigger = .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			If tTrigger >= 0 Then
				MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = tTrigger
				tLog = "Trigger " & tTrigger & " en mapa " & .Pos.Map & " " & .Pos.X & "," & .Pos.Y
				
				Call LogGM(.name, tLog)
				Call WriteConsoleMsg(UserIndex, tLog, FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub
	
	''
	' Handles the "AskTrigger" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleAskTrigger(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 04/13/07
		'
		'***************************************************
		Dim tTrigger As Byte
		
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			tTrigger = MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger
			
			Call LogGM(.name, "Miro el trigger en " & .Pos.Map & "," & .Pos.X & "," & .Pos.Y & ". Era " & tTrigger)
			
			Call WriteConsoleMsg(UserIndex, "Trigger " & tTrigger & " en mapa " & .Pos.Map & " " & .Pos.X & ", " & .Pos.Y, FontTypeNames.FONTTYPE_INFO)
		End With
	End Sub
	
	''
	' Handles the "BannedIPList" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleBannedIPList(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		Dim lista As String
		Dim LoopC As Integer
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			
			Call LogGM(.name, "/BANIPLIST")
			
			For LoopC = 1 To BanIps.Count()
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto BanIps.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				lista = lista & BanIps.Item(LoopC) & ", "
			Next LoopC
			
			If migr_LenB(lista) <> 0 Then lista = Left(lista, Len(lista) - 2)
			
			Call WriteConsoleMsg(UserIndex, lista, FontTypeNames.FONTTYPE_INFO)
		End With
	End Sub
	
	''
	' Handles the "BannedIPReload" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleBannedIPReload(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Call BanIpGuardar()
			Call BanIpCargar()
		End With
	End Sub
	
	''
	' Handles the "GuildBan" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleGuildBan(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim GuildName As String
		Dim cantMembers As Short
		Dim LoopC As Integer
		Dim member As String
		Dim Count As Byte
		Dim tIndex As Short
		Dim tFile As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			GuildName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) Then
				tFile = My.Application.Info.DirectoryPath & "\guilds\" & GuildName & "-members.mem"
				
				If Not FileExist(tFile) Then
					Call WriteConsoleMsg(UserIndex, "No existe el clan: " & GuildName, FontTypeNames.FONTTYPE_INFO)
				Else
					Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg(.name & " baneó al clan " & UCase(GuildName), FontTypeNames.FONTTYPE_FIGHT))
					
					'baneamos a los miembros
					Call LogGM(.name, "BANCLAN a " & UCase(GuildName))
					
					cantMembers = Val(GetVar(tFile, "INIT", "NroMembers"))
					
					For LoopC = 1 To cantMembers
						member = GetVar(tFile, "Members", "Member" & LoopC)
						'member es la victima
						Call Ban(member, "Administracion del servidor", "Clan Banned")
						
						Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg("   " & member & "<" & GuildName & "> ha sido expulsado del servidor.", FontTypeNames.FONTTYPE_FIGHT))
						
						tIndex = NameIndex(member)
						If tIndex > 0 Then
							'esta online
							UserList(tIndex).flags.Ban = 1
							Call CloseSocket(tIndex)
						End If
						
						'ponemos el flag de ban a 1
						Call WriteVar(CharPath & member & ".chr", "FLAGS", "Ban", "1")
						'ponemos la pena
						Count = Val(GetVar(CharPath & member & ".chr", "PENAS", "Cant"))
						Call WriteVar(CharPath & member & ".chr", "PENAS", "Cant", CStr(Count + 1))
						Call WriteVar(CharPath & member & ".chr", "PENAS", "P" & Count + 1, LCase(.name) & ": BAN AL CLAN: " & GuildName & " " & Today & " " & TimeOfDay)
					Next LoopC
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "BanIP" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleBanIP(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 07/02/09
		'Agregado un CopyBuffer porque se producia un bucle
		'inifito al intentar banear una ip ya baneada. (NicoNZ)
		'07/02/09 Pato - Ahora no es posible saber si un gm está o no online.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 6 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim bannedIP As String
		Dim tUser As Short
		Dim reason As String
		Dim i As Integer
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			' Is it by ip??
			If buffer.ReadBoolean() Then
				bannedIP = buffer.ReadByte() & "."
				bannedIP = bannedIP & buffer.ReadByte() & "."
				bannedIP = bannedIP & buffer.ReadByte() & "."
				bannedIP = bannedIP & buffer.ReadByte()
			Else
				tUser = NameIndex(buffer.ReadASCIIString())
				
				If tUser > 0 Then bannedIP = UserList(tUser).ip
			End If
			
			reason = buffer.ReadASCIIString()
			
			
			If .flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios) Then
				If migr_LenB(bannedIP) > 0 Then
					Call LogGM(.name, "/BanIP " & bannedIP & " por " & reason)
					
					If BanIpBuscar(bannedIP) > 0 Then
						Call WriteConsoleMsg(UserIndex, "La IP " & bannedIP & " ya se encuentra en la lista de bans.", FontTypeNames.FONTTYPE_INFO)
					Else
						Call BanIpAgrega(bannedIP)
						Call SendData(modSendData.SendTarget.ToAdmins, 0, PrepareMessageConsoleMsg(.name & " baneó la IP " & bannedIP & " por " & reason, FontTypeNames.FONTTYPE_FIGHT))
						
						'Find every player with that ip and ban him!
						For i = 1 To LastUser
							If UserList(i).ConnIDValida Then
								If UserList(i).ip = bannedIP Then
									Call BanCharacter(UserIndex, UserList(i).name, "IP POR " & reason)
								End If
							End If
						Next i
					End If
				ElseIf tUser <= 0 Then 
					Call WriteConsoleMsg(UserIndex, "El personaje no está online.", FontTypeNames.FONTTYPE_INFO)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "UnbanIP" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleUnbanIP(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/30/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim bannedIP As String
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			
			bannedIP = .incomingData.ReadByte() & "."
			bannedIP = bannedIP & .incomingData.ReadByte() & "."
			bannedIP = bannedIP & .incomingData.ReadByte() & "."
			bannedIP = bannedIP & .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			If BanIpQuita(bannedIP) Then
				Call WriteConsoleMsg(UserIndex, "La IP """ & bannedIP & """ se ha quitado de la lista de bans.", FontTypeNames.FONTTYPE_INFO)
			Else
				Call WriteConsoleMsg(UserIndex, "La IP """ & bannedIP & """ NO se encuentra en la lista de bans.", FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub
	
	''
	' Handles the "CreateItem" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleCreateItem(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim tObj As Short
		Dim tStr As String
		Dim Objeto As Obj
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			tObj = .incomingData.ReadInteger()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			
			Call LogGM(.name, "/CI: " & tObj)
			
			If MapData(.Pos.Map, .Pos.X, .Pos.Y - 1).ObjInfo.ObjIndex > 0 Then Exit Sub
			
			If MapData(.Pos.Map, .Pos.X, .Pos.Y - 1).TileExit.Map > 0 Then Exit Sub
			
			If tObj < 1 Or tObj > NumObjDatas Then Exit Sub
			
			'Is the object not null?
			If migr_LenB(ObjData_Renamed(tObj).name) = 0 Then Exit Sub
			
			Call WriteConsoleMsg(UserIndex, "¡¡ATENCIÓN: FUERON CREADOS ***100*** ÍTEMS, TIRE Y /DEST LOS QUE NO NECESITE!!", FontTypeNames.FONTTYPE_GUILD)
			
			Objeto.Amount = 100
			Objeto.ObjIndex = tObj
			Call MakeObj(Objeto, .Pos.Map, .Pos.X, .Pos.Y - 1)
		End With
	End Sub
	
	''
	' Handles the "DestroyItems" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleDestroyItems(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			
			If MapData(.Pos.Map, .Pos.X, .Pos.Y).ObjInfo.ObjIndex = 0 Then Exit Sub
			
			Call LogGM(.name, "/DEST")
			
			If ObjData_Renamed(MapData(.Pos.Map, .Pos.X, .Pos.Y).ObjInfo.ObjIndex).OBJType = Declaraciones.eOBJType.otTeleport And MapData(.Pos.Map, .Pos.X, .Pos.Y).TileExit.Map > 0 Then
				
				Call WriteConsoleMsg(UserIndex, "No puede destruir teleports así. Utilice /DT.", FontTypeNames.FONTTYPE_INFO)
				Exit Sub
			End If
			
			Call EraseObj(10000, .Pos.Map, .Pos.X, .Pos.Y)
		End With
	End Sub
	
	''
	' Handles the "ChaosLegionKick" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleChaosLegionKick(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) <> 0 Then
				If (migr_InStrB(UserName, "\") <> 0) Then
					UserName = Replace(UserName, "\", "")
				End If
				If (migr_InStrB(UserName, "/") <> 0) Then
					UserName = Replace(UserName, "/", "")
				End If
				tUser = NameIndex(UserName)
				
				Call LogGM(.name, "ECHO DEL CAOS A: " & UserName)
				
				If tUser > 0 Then
					Call ExpulsarFaccionCaos(tUser, True)
					UserList(tUser).Faccion.Reenlistadas = 200
					Call WriteConsoleMsg(UserIndex, UserName & " expulsado de las fuerzas del caos y prohibida la reenlistada.", FontTypeNames.FONTTYPE_INFO)
					Call WriteConsoleMsg(tUser, .name & " te ha expulsado en forma definitiva de las fuerzas del caos.", FontTypeNames.FONTTYPE_FIGHT)
					Call FlushBuffer(tUser)
				Else
					If FileExist(CharPath & UserName & ".chr") Then
						Call WriteVar(CharPath & UserName & ".chr", "FACCIONES", "EjercitoCaos", CStr(0))
						Call WriteVar(CharPath & UserName & ".chr", "FACCIONES", "Reenlistadas", CStr(200))
						Call WriteVar(CharPath & UserName & ".chr", "FACCIONES", "Extra", "Expulsado por " & .name)
						Call WriteConsoleMsg(UserIndex, UserName & " expulsado de las fuerzas del caos y prohibida la reenlistada.", FontTypeNames.FONTTYPE_INFO)
					Else
						Call WriteConsoleMsg(UserIndex, UserName & ".chr inexistente.", FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "RoyalArmyKick" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleRoyalArmyKick(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) <> 0 Then
				If (migr_InStrB(UserName, "\") <> 0) Then
					UserName = Replace(UserName, "\", "")
				End If
				If (migr_InStrB(UserName, "/") <> 0) Then
					UserName = Replace(UserName, "/", "")
				End If
				tUser = NameIndex(UserName)
				
				Call LogGM(.name, "ECHÓ DE LA REAL A: " & UserName)
				
				If tUser > 0 Then
					Call ExpulsarFaccionReal(tUser, True)
					UserList(tUser).Faccion.Reenlistadas = 200
					Call WriteConsoleMsg(UserIndex, UserName & " expulsado de las fuerzas reales y prohibida la reenlistada.", FontTypeNames.FONTTYPE_INFO)
					Call WriteConsoleMsg(tUser, .name & " te ha expulsado en forma definitiva de las fuerzas reales.", FontTypeNames.FONTTYPE_FIGHT)
					Call FlushBuffer(tUser)
				Else
					If FileExist(CharPath & UserName & ".chr") Then
						Call WriteVar(CharPath & UserName & ".chr", "FACCIONES", "EjercitoReal", CStr(0))
						Call WriteVar(CharPath & UserName & ".chr", "FACCIONES", "Reenlistadas", CStr(200))
						Call WriteVar(CharPath & UserName & ".chr", "FACCIONES", "Extra", "Expulsado por " & .name)
						Call WriteConsoleMsg(UserIndex, UserName & " expulsado de las fuerzas reales y prohibida la reenlistada.", FontTypeNames.FONTTYPE_INFO)
					Else
						Call WriteConsoleMsg(UserIndex, UserName & ".chr inexistente.", FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "ForceMIDIAll" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleForceMIDIAll(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim midiID As Byte
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			midiID = .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			
			Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg(.name & " broadcast música: " & midiID, FontTypeNames.FONTTYPE_SERVER))
			
			Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessagePlayMidi(midiID))
		End With
	End Sub
	
	''
	' Handles the "ForceWAVEAll" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleForceWAVEAll(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim waveID As Byte
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			waveID = .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			
			Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessagePlayWave(waveID, NO_3D_SOUND, NO_3D_SOUND))
		End With
	End Sub
	
	''
	' Handles the "RemovePunishment" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleRemovePunishment(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 1/05/07
		'Pablo (ToxicWaste): 1/05/07, You can now edit the punishment.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 6 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim punishment As Byte
		Dim NewText As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			punishment = buffer.ReadByte
			NewText = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) Then
				If migr_LenB(UserName) = 0 Then
					Call WriteConsoleMsg(UserIndex, "Utilice /borrarpena Nick@NumeroDePena@NuevaPena", FontTypeNames.FONTTYPE_INFO)
				Else
					If (migr_InStrB(UserName, "\") <> 0) Then
						UserName = Replace(UserName, "\", "")
					End If
					If (migr_InStrB(UserName, "/") <> 0) Then
						UserName = Replace(UserName, "/", "")
					End If
					
					If FileExist(CharPath & UserName & ".chr") Then
						Call LogGM(.name, " borro la pena: " & punishment & "-" & GetVar(CharPath & UserName & ".chr", "PENAS", "P" & punishment) & " de " & UserName & " y la cambió por: " & NewText)
						
						Call WriteVar(CharPath & UserName & ".chr", "PENAS", "P" & punishment, LCase(.name) & ": <" & NewText & "> " & Today & " " & TimeOfDay)
						
						Call WriteConsoleMsg(UserIndex, "Pena modificada.", FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "TileBlockedToggle" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleTileBlockedToggle(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			
			Call LogGM(.name, "/BLOQ")
			
			If MapData(.Pos.Map, .Pos.X, .Pos.Y).Blocked = 0 Then
				MapData(.Pos.Map, .Pos.X, .Pos.Y).Blocked = 1
			Else
				MapData(.Pos.Map, .Pos.X, .Pos.Y).Blocked = 0
			End If
			
			Call Bloquear(True, .Pos.Map, .Pos.X, .Pos.Y, MapData(.Pos.Map, .Pos.X, .Pos.Y).Blocked)
		End With
	End Sub
	
	''
	' Handles the "KillNPCNoRespawn" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleKillNPCNoRespawn(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			
			If .flags.TargetNPC = 0 Then Exit Sub
			
			Call QuitarNPC(.flags.TargetNPC)
			Call LogGM(.name, "/MATA " & Npclist(.flags.TargetNPC).name)
		End With
	End Sub
	
	''
	' Handles the "KillAllNearbyNPCs" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleKillAllNearbyNPCs(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		Dim X As Integer
		Dim Y As Integer
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			
			
			For Y = .Pos.Y - MinYBorder + 1 To .Pos.Y + MinYBorder - 1
				For X = .Pos.X - MinXBorder + 1 To .Pos.X + MinXBorder - 1
					If X > 0 And Y > 0 And X < 101 And Y < 101 Then
						If MapData(.Pos.Map, X, Y).NpcIndex > 0 Then Call QuitarNPC(MapData(.Pos.Map, X, Y).NpcIndex)
					End If
				Next X
			Next Y
			Call LogGM(.name, "/MASSKILL")
		End With
	End Sub
	
	''
	' Handles the "LastIP" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Private Sub HandleLastIP(ByVal UserIndex As Short)
		'***************************************************
		'Author: Nicolas Matias Gonzalez (NIGO)
		'Last Modification: 12/30/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim lista As String
		Dim LoopC As Byte
		Dim priv As Short
		Dim validCheck As Boolean
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			priv = Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.Consejero
			UserName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios)) <> 0 Then
				'Handle special chars
				If (migr_InStrB(UserName, "\") <> 0) Then
					UserName = Replace(UserName, "\", "")
				End If
				If (migr_InStrB(UserName, "\") <> 0) Then
					UserName = Replace(UserName, "/", "")
				End If
				If (migr_InStrB(UserName, "+") <> 0) Then
					UserName = Replace(UserName, "+", " ")
				End If
				
				'Only Gods and Admins can see the ips of adminsitrative characters. All others can be seen by every adminsitrative char.
				If NameIndex(UserName) > 0 Then
					validCheck = (UserList(NameIndex(UserName)).flags.Privilegios And priv) = 0 Or (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) <> 0
				Else
					validCheck = (UserDarPrivilegioLevel(UserName) And priv) = 0 Or (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) <> 0
				End If
				
				If validCheck Then
					Call LogGM(.name, "/LASTIP " & UserName)
					
					If FileExist(CharPath & UserName & ".chr") Then
						lista = "Las ultimas IPs con las que " & UserName & " se conectó son:"
						For LoopC = 1 To 5
							lista = lista & vbCrLf & LoopC & " - " & GetVar(CharPath & UserName & ".chr", "INIT", "LastIP" & LoopC)
						Next LoopC
						Call WriteConsoleMsg(UserIndex, lista, FontTypeNames.FONTTYPE_INFO)
					Else
						Call WriteConsoleMsg(UserIndex, "Charfile """ & UserName & """ inexistente.", FontTypeNames.FONTTYPE_INFO)
					End If
				Else
					Call WriteConsoleMsg(UserIndex, UserName & " es de mayor jerarquía que vos.", FontTypeNames.FONTTYPE_INFO)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "ChatColor" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Public Sub HandleChatColor(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/23/06
		'Last modified by: Juan Martín Sotuyo Dodero (Maraxus)
		'Change the user`s chat color
		'***************************************************
		If UserList(UserIndex).incomingData.length < 4 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim color As Integer
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			
			color = RGB(.incomingData.ReadByte(), .incomingData.ReadByte(), .incomingData.ReadByte())
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.RoleMaster)) Then
				.flags.ChatColor = color
			End If
		End With
	End Sub
	
	''
	' Handles the "Ignored" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Public Sub HandleIgnored(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/23/06
		'Ignore the user
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.Consejero) Then
				.flags.AdminPerseguible = Not .flags.AdminPerseguible
			End If
		End With
	End Sub
	
	''
	' Handles the "CheckSlot" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Public Sub HandleCheckSlot(ByVal UserIndex As Short)
		'***************************************************
		'Author: Pablo (ToxicWaste)
		'Last Modification: 09/09/2008 (NicoNZ)
		'Check one Users Slot in Particular from Inventory
		'***************************************************
		If UserList(UserIndex).incomingData.length < 4 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim Slot As Byte
		Dim tIndex As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			'Reads the UserName and Slot Packets
			
			UserName = buffer.ReadASCIIString() 'Que UserName?
			Slot = buffer.ReadByte() 'Que Slot?
			
			If .flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.Dios) Then
				tIndex = NameIndex(UserName) 'Que user index?
				
				Call LogGM(.name, .name & " Checkeó el slot " & Slot & " de " & UserName)
				
				If tIndex > 0 Then
					If Slot > 0 And Slot <= UserList(tIndex).CurrentInventorySlots Then
						If UserList(tIndex).Invent.Object_Renamed(Slot).ObjIndex > 0 Then
							Call WriteConsoleMsg(UserIndex, " Objeto " & Slot & ") " & ObjData_Renamed(UserList(tIndex).Invent.Object_Renamed(Slot).ObjIndex).name & " Cantidad:" & UserList(tIndex).Invent.Object_Renamed(Slot).Amount, FontTypeNames.FONTTYPE_INFO)
						Else
							Call WriteConsoleMsg(UserIndex, "No hay ningún objeto en slot seleccionado.", FontTypeNames.FONTTYPE_INFO)
						End If
					Else
						Call WriteConsoleMsg(UserIndex, "Slot Inválido.", FontTypeNames.FONTTYPE_TALK)
					End If
				Else
					Call WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_TALK)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handles the "ResetAutoUpdate" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Public Sub HandleResetAutoUpdate(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/23/06
		'Reset the AutoUpdate
		'***************************************************
		With UserList(UserIndex)
			'Remove packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			If UCase(.name) <> "MARAXUS" Then Exit Sub
			
			Call WriteConsoleMsg(UserIndex, "TID: " & CStr(ReiniciarAutoUpdate()), FontTypeNames.FONTTYPE_INFO)
		End With
	End Sub
	
	''
	' Handles the "Restart" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Public Sub HandleRestart(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/23/06
		'Restart the game
		'***************************************************
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			If UCase(.name) <> "MARAXUS" Then Exit Sub
			
			'time and Time BUG!
			Call LogGM(.name, .name & " reinició el mundo.")
			
			Call ReiniciarServidor(True)
		End With
	End Sub
	
	''
	' Handles the "ReloadObjects" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Public Sub HandleReloadObjects(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/23/06
		'Reload the objects
		'***************************************************
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Call LogGM(.name, .name & " ha recargado los objetos.")
			
			Call LoadOBJData()
		End With
	End Sub
	
	''
	' Handles the "ReloadSpells" message.
	'
	' @param    userIndex The index of the user sending the message.
	
	Public Sub HandleReloadSpells(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/23/06
		'Reload the spells
		'***************************************************
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Call LogGM(.name, .name & " ha recargado los hechizos.")
			
			Call CargarHechizos()
		End With
	End Sub
	
	''
	' Handle the "ReloadServerIni" message.
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleReloadServerIni(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/23/06
		'Reload the Server`s INI
		'***************************************************
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Call LogGM(.name, .name & " ha recargado los INITs.")
			
			Call LoadSini()
		End With
	End Sub
	
	''
	' Handle the "ReloadNPCs" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleReloadNPCs(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/23/06
		'Reload the Server`s NPC
		'***************************************************
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Call LogGM(.name, .name & " ha recargado los NPCs.")
			
			Call CargaNpcsDat()
			
			Call WriteConsoleMsg(UserIndex, "Npcs.dat recargado.", FontTypeNames.FONTTYPE_INFO)
		End With
	End Sub
	
	''
	' Handle the "KickAllChars" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleKickAllChars(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/23/06
		'Kick all the chars that are online
		'***************************************************
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Call LogGM(.name, .name & " ha echado a todos los personajes.")
			
			Call EcharPjsNoPrivilegiados()
		End With
	End Sub
	
	''
	' Handle the "Night" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleNight(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/23/06
		'Last modified by: Juan Martín Sotuyo Dodero (Maraxus)
		'
		'***************************************************
		Dim i As Integer
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			If UCase(.name) <> "MARAXUS" Then Exit Sub
			
			DeNoche = Not DeNoche
			
			
			For i = 1 To NumUsers
				If UserList(i).flags.UserLogged And UserList(i).ConnID > -1 Then
					Call EnviarNoche(i)
				End If
			Next i
		End With
	End Sub
	
	''
	' Handle the "ShowServerForm" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleShowServerForm(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/23/06
		'Show the server form
		'***************************************************
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Call LogGM(.name, .name & " ha solicitado mostrar el formulario del servidor.")
			' TODO FIX: no funciona como se espera, de todas formas no es algo funcional
			Call frmMain.Show()
		End With
	End Sub
	
	''
	' Handle the "CleanSOS" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleCleanSOS(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/23/06
		'Clean the SOS
		'***************************************************
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Call LogGM(.name, .name & " ha borrado los SOS.")
			
			Call Ayuda.Reset_Renamed()
		End With
	End Sub
	
	''
	' Handle the "SaveChars" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleSaveChars(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/23/06
		'Save the characters
		'***************************************************
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Call LogGM(.name, .name & " ha guardado todos los chars.")
			
			Call mdParty.ActualizaExperiencias()
			Call GuardarUsuarios()
		End With
	End Sub
	
	''
	' Handle the "ChangeMapInfoBackup" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleChangeMapInfoBackup(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/24/06
		'Last modified by: Juan Martín Sotuyo Dodero (Maraxus)
		'Change the backup`s info of the map
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim doTheBackUp As Boolean
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			
			doTheBackUp = .incomingData.ReadBoolean()
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) = 0 Then Exit Sub
			
			Call LogGM(.name, .name & " ha cambiado la información sobre el BackUp.")
			
			'Change the boolean to byte in a fast way
			If doTheBackUp Then
				MapInfo_Renamed(.Pos.Map).BackUp = 1
			Else
				MapInfo_Renamed(.Pos.Map).BackUp = 0
			End If
			
			'Change the boolean to string in a fast way
			Call WriteVar(My.Application.Info.DirectoryPath & MapPath & "mapa" & .Pos.Map & ".dat", "Mapa" & .Pos.Map, "backup", CStr(MapInfo_Renamed(.Pos.Map).BackUp))
			
			Call WriteConsoleMsg(UserIndex, "Mapa " & .Pos.Map & " Backup: " & MapInfo_Renamed(.Pos.Map).BackUp, FontTypeNames.FONTTYPE_INFO)
		End With
	End Sub
	
	''
	' Handle the "ChangeMapInfoPK" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleChangeMapInfoPK(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/24/06
		'Last modified by: Juan Martín Sotuyo Dodero (Maraxus)
		'Change the pk`s info of the  map
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim isMapPk As Boolean
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			
			isMapPk = .incomingData.ReadBoolean()
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) = 0 Then Exit Sub
			
			Call LogGM(.name, .name & " ha cambiado la información sobre si es PK el mapa.")
			
			MapInfo_Renamed(.Pos.Map).Pk = isMapPk
			
			'Change the boolean to string in a fast way
			Call WriteVar(My.Application.Info.DirectoryPath & MapPath & "mapa" & .Pos.Map & ".dat", "Mapa" & .Pos.Map, "Pk", IIf(isMapPk, "1", "0"))
			
			Call WriteConsoleMsg(UserIndex, "Mapa " & .Pos.Map & " PK: " & MapInfo_Renamed(.Pos.Map).Pk, FontTypeNames.FONTTYPE_INFO)
		End With
	End Sub
	
	''
	' Handle the "ChangeMapInfoRestricted" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleChangeMapInfoRestricted(ByVal UserIndex As Short)
		'***************************************************
		'Author: Pablo (ToxicWaste)
		'Last Modification: 26/01/2007
		'Restringido -> Options: "NEWBIE", "NO", "ARMADA", "CAOS", "FACCION".
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim tStr As String
		
		Dim buffer As New clsByteQueue
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove Packet ID
			Call buffer.ReadByte()
			
			tStr = buffer.ReadASCIIString()
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) <> 0 Then
				If tStr = "NEWBIE" Or tStr = "NO" Or tStr = "ARMADA" Or tStr = "CAOS" Or tStr = "FACCION" Then
					Call LogGM(.name, .name & " ha cambiado la información sobre si es restringido el mapa.")
					MapInfo_Renamed(UserList(UserIndex).Pos.Map).Restringir = tStr
					Call WriteVar(My.Application.Info.DirectoryPath & MapPath & "mapa" & UserList(UserIndex).Pos.Map & ".dat", "Mapa" & UserList(UserIndex).Pos.Map, "Restringir", tStr)
					Call WriteConsoleMsg(UserIndex, "Mapa " & .Pos.Map & " Restringido: " & MapInfo_Renamed(.Pos.Map).Restringir, FontTypeNames.FONTTYPE_INFO)
				Else
					Call WriteConsoleMsg(UserIndex, "Opciones para restringir: 'NEWBIE', 'NO', 'ARMADA', 'CAOS', 'FACCION'", FontTypeNames.FONTTYPE_INFO)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handle the "ChangeMapInfoNoMagic" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleChangeMapInfoNoMagic(ByVal UserIndex As Short)
		'***************************************************
		'Author: Pablo (ToxicWaste)
		'Last Modification: 26/01/2007
		'MagiaSinEfecto -> Options: "1" , "0".
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim nomagic As Boolean
		
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			nomagic = .incomingData.ReadBoolean
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) <> 0 Then
				Call LogGM(.name, .name & " ha cambiado la información sobre si está permitido usar la magia el mapa.")
				MapInfo_Renamed(UserList(UserIndex).Pos.Map).MagiaSinEfecto = nomagic
				Call WriteVar(My.Application.Info.DirectoryPath & MapPath & "mapa" & UserList(UserIndex).Pos.Map & ".dat", "Mapa" & UserList(UserIndex).Pos.Map, "MagiaSinEfecto", CStr(nomagic))
				Call WriteConsoleMsg(UserIndex, "Mapa " & .Pos.Map & " MagiaSinEfecto: " & MapInfo_Renamed(.Pos.Map).MagiaSinEfecto, FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub
	
	''
	' Handle the "ChangeMapInfoNoInvi" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleChangeMapInfoNoInvi(ByVal UserIndex As Short)
		'***************************************************
		'Author: Pablo (ToxicWaste)
		'Last Modification: 26/01/2007
		'InviSinEfecto -> Options: "1", "0"
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim noinvi As Boolean
		
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			noinvi = .incomingData.ReadBoolean()
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) <> 0 Then
				Call LogGM(.name, .name & " ha cambiado la información sobre si está permitido usar la invisibilidad en el mapa.")
				MapInfo_Renamed(UserList(UserIndex).Pos.Map).InviSinEfecto = noinvi
				Call WriteVar(My.Application.Info.DirectoryPath & MapPath & "mapa" & UserList(UserIndex).Pos.Map & ".dat", "Mapa" & UserList(UserIndex).Pos.Map, "InviSinEfecto", CStr(noinvi))
				Call WriteConsoleMsg(UserIndex, "Mapa " & .Pos.Map & " InviSinEfecto: " & MapInfo_Renamed(.Pos.Map).InviSinEfecto, FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub
	
	''
	' Handle the "ChangeMapInfoNoResu" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleChangeMapInfoNoResu(ByVal UserIndex As Short)
		'***************************************************
		'Author: Pablo (ToxicWaste)
		'Last Modification: 26/01/2007
		'ResuSinEfecto -> Options: "1", "0"
		'***************************************************
		If UserList(UserIndex).incomingData.length < 2 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim noresu As Boolean
		
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			noresu = .incomingData.ReadBoolean()
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) <> 0 Then
				Call LogGM(.name, .name & " ha cambiado la información sobre si está permitido usar el resucitar en el mapa.")
				MapInfo_Renamed(UserList(UserIndex).Pos.Map).ResuSinEfecto = noresu
				Call WriteVar(My.Application.Info.DirectoryPath & MapPath & "mapa" & UserList(UserIndex).Pos.Map & ".dat", "Mapa" & UserList(UserIndex).Pos.Map, "ResuSinEfecto", CStr(noresu))
				Call WriteConsoleMsg(UserIndex, "Mapa " & .Pos.Map & " ResuSinEfecto: " & MapInfo_Renamed(.Pos.Map).ResuSinEfecto, FontTypeNames.FONTTYPE_INFO)
			End If
		End With
	End Sub
	
	''
	' Handle the "ChangeMapInfoLand" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleChangeMapInfoLand(ByVal UserIndex As Short)
		'***************************************************
		'Author: Pablo (ToxicWaste)
		'Last Modification: 26/01/2007
		'Terreno -> Opciones: "BOSQUE", "NIEVE", "DESIERTO", "CIUDAD", "CAMPO", "DUNGEON".
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim tStr As String
		
		Dim buffer As New clsByteQueue
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove Packet ID
			Call buffer.ReadByte()
			
			tStr = buffer.ReadASCIIString()
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) <> 0 Then
				If tStr = "BOSQUE" Or tStr = "NIEVE" Or tStr = "DESIERTO" Or tStr = "CIUDAD" Or tStr = "CAMPO" Or tStr = "DUNGEON" Then
					Call LogGM(.name, .name & " ha cambiado la información del terreno del mapa.")
					MapInfo_Renamed(UserList(UserIndex).Pos.Map).Terreno = tStr
					Call WriteVar(My.Application.Info.DirectoryPath & MapPath & "mapa" & UserList(UserIndex).Pos.Map & ".dat", "Mapa" & UserList(UserIndex).Pos.Map, "Terreno", tStr)
					Call WriteConsoleMsg(UserIndex, "Mapa " & .Pos.Map & " Terreno: " & MapInfo_Renamed(.Pos.Map).Terreno, FontTypeNames.FONTTYPE_INFO)
				Else
					Call WriteConsoleMsg(UserIndex, "Opciones para terreno: 'BOSQUE', 'NIEVE', 'DESIERTO', 'CIUDAD', 'CAMPO', 'DUNGEON'", FontTypeNames.FONTTYPE_INFO)
					Call WriteConsoleMsg(UserIndex, "Igualmente, el único útil es 'NIEVE' ya que al ingresarlo, la gente muere de frío en el mapa.", FontTypeNames.FONTTYPE_INFO)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handle the "ChangeMapInfoZone" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleChangeMapInfoZone(ByVal UserIndex As Short)
		'***************************************************
		'Author: Pablo (ToxicWaste)
		'Last Modification: 26/01/2007
		'Zona -> Opciones: "BOSQUE", "NIEVE", "DESIERTO", "CIUDAD", "CAMPO", "DUNGEON".
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim tStr As String
		
		Dim buffer As New clsByteQueue
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove Packet ID
			Call buffer.ReadByte()
			
			tStr = buffer.ReadASCIIString()
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) <> 0 Then
				If tStr = "BOSQUE" Or tStr = "NIEVE" Or tStr = "DESIERTO" Or tStr = "CIUDAD" Or tStr = "CAMPO" Or tStr = "DUNGEON" Then
					Call LogGM(.name, .name & " ha cambiado la información de la zona del mapa.")
					MapInfo_Renamed(UserList(UserIndex).Pos.Map).Zona = tStr
					Call WriteVar(My.Application.Info.DirectoryPath & MapPath & "mapa" & UserList(UserIndex).Pos.Map & ".dat", "Mapa" & UserList(UserIndex).Pos.Map, "Zona", tStr)
					Call WriteConsoleMsg(UserIndex, "Mapa " & .Pos.Map & " Zona: " & MapInfo_Renamed(.Pos.Map).Zona, FontTypeNames.FONTTYPE_INFO)
				Else
					Call WriteConsoleMsg(UserIndex, "Opciones para terreno: 'BOSQUE', 'NIEVE', 'DESIERTO', 'CIUDAD', 'CAMPO', 'DUNGEON'", FontTypeNames.FONTTYPE_INFO)
					Call WriteConsoleMsg(UserIndex, "Igualmente, el único útil es 'DUNGEON' ya que al ingresarlo, NO se sentirá el efecto de la lluvia en este mapa.", FontTypeNames.FONTTYPE_INFO)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handle the "SaveMap" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleSaveMap(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/24/06
		'Saves the map
		'***************************************************
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Call LogGM(.name, .name & " ha guardado el mapa " & CStr(.Pos.Map))
			
			Call GrabarMapa(.Pos.Map, My.Application.Info.DirectoryPath & "\WorldBackUp\Mapa" & .Pos.Map)
			
			Call WriteConsoleMsg(UserIndex, "Mapa Guardado.", FontTypeNames.FONTTYPE_INFO)
		End With
	End Sub
	
	''
	' Handle the "ShowGuildMessages" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleShowGuildMessages(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/24/06
		'Last modified by: Juan Martín Sotuyo Dodero (Maraxus)
		'Allows admins to read guild messages
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim guild As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			guild = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) Then
				Call modGuilds.GMEscuchaClan(UserIndex, guild)
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handle the "DoBackUp" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleDoBackUp(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/24/06
		'Show guilds messages
		'***************************************************
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Call LogGM(.name, .name & " ha hecho un backup.")
			
			Call ES.DoBackUp() 'Sino lo confunde con la id del paquete
		End With
	End Sub
	
	''
	' Handle the "ToggleCentinelActivated" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleToggleCentinelActivated(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/26/06
		'Last modified by: Juan Martín Sotuyo Dodero (Maraxus)
		'Activate or desactivate the Centinel
		'***************************************************
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			
			centinelaActivado = Not centinelaActivado
			
			With Centinela
				.RevisandoUserIndex = 0
				.clave = 0
				.TiempoRestante = 0
			End With
			
			If CentinelaNPCIndex Then
				Call QuitarNPC(CentinelaNPCIndex)
				CentinelaNPCIndex = 0
			End If
			
			If centinelaActivado Then
				Call SendData(modSendData.SendTarget.ToAdmins, 0, PrepareMessageConsoleMsg("El centinela ha sido activado.", FontTypeNames.FONTTYPE_SERVER))
			Else
				Call SendData(modSendData.SendTarget.ToAdmins, 0, PrepareMessageConsoleMsg("El centinela ha sido desactivado.", FontTypeNames.FONTTYPE_SERVER))
			End If
		End With
	End Sub
	
	''
	' Handle the "AlterName" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleAlterName(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/26/06
		'Change user name
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim newName As String
		Dim changeNameUI As Short
		Dim GuildIndex As Short
		Dim cantPenas As Byte
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			'Reads the userName and newUser Packets
			
			UserName = buffer.ReadASCIIString()
			newName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) Then
				If migr_LenB(UserName) = 0 Or migr_LenB(newName) = 0 Then
					Call WriteConsoleMsg(UserIndex, "Usar: /ANAME origen@destino", FontTypeNames.FONTTYPE_INFO)
				Else
					changeNameUI = NameIndex(UserName)
					
					If changeNameUI > 0 Then
						Call WriteConsoleMsg(UserIndex, "El Pj está online, debe salir para hacer el cambio.", FontTypeNames.FONTTYPE_WARNING)
					Else
						If Not FileExist(CharPath & UserName & ".chr") Then
							Call WriteConsoleMsg(UserIndex, "El pj " & UserName & " es inexistente.", FontTypeNames.FONTTYPE_INFO)
						Else
							GuildIndex = Val(GetVar(CharPath & UserName & ".chr", "GUILD", "GUILDINDEX"))
							
							If GuildIndex > 0 Then
								Call WriteConsoleMsg(UserIndex, "El pj " & UserName & " pertenece a un clan, debe salir del mismo con /salirclan para ser transferido.", FontTypeNames.FONTTYPE_INFO)
							Else
								If Not FileExist(CharPath & newName & ".chr") Then
									Call FileCopy(CharPath & UserName & ".chr", CharPath & UCase(newName) & ".chr")
									
									Call WriteConsoleMsg(UserIndex, "Transferencia exitosa.", FontTypeNames.FONTTYPE_INFO)
									
									Call WriteVar(CharPath & UserName & ".chr", "FLAGS", "Ban", "1")
									
									
									cantPenas = Val(GetVar(CharPath & UserName & ".chr", "PENAS", "Cant"))
									
									Call WriteVar(CharPath & UserName & ".chr", "PENAS", "Cant", CStr(cantPenas + 1))
									
									Call WriteVar(CharPath & UserName & ".chr", "PENAS", "P" & CStr(cantPenas + 1), LCase(.name) & ": BAN POR Cambio de nick a " & UCase(newName) & " " & Today & " " & TimeOfDay)
									
									Call LogGM(.name, "Ha cambiado de nombre al usuario " & UserName & ". Ahora se llama " & newName)
								Else
									Call WriteConsoleMsg(UserIndex, "El nick solicitado ya existe.", FontTypeNames.FONTTYPE_INFO)
								End If
							End If
						End If
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handle the "AlterName" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleAlterMail(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/26/06
		'Change user password
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim newMail As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			newMail = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) Then
				If migr_LenB(UserName) = 0 Or migr_LenB(newMail) = 0 Then
					Call WriteConsoleMsg(UserIndex, "usar /AEMAIL <pj>-<nuevomail>", FontTypeNames.FONTTYPE_INFO)
				Else
					If Not FileExist(CharPath & UserName & ".chr") Then
						Call WriteConsoleMsg(UserIndex, "No existe el charfile " & UserName & ".chr", FontTypeNames.FONTTYPE_INFO)
					Else
						Call WriteVar(CharPath & UserName & ".chr", "CONTACTO", "Email", newMail)
						Call WriteConsoleMsg(UserIndex, "Email de " & UserName & " cambiado a: " & newMail, FontTypeNames.FONTTYPE_INFO)
					End If
					
					Call LogGM(.name, "Le ha cambiado el mail a " & UserName)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handle the "AlterPassword" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleAlterPassword(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/26/06
		'Change user password
		'***************************************************
		If UserList(UserIndex).incomingData.length < 5 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim copyFrom As String
		Dim Password As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = Replace(buffer.ReadASCIIString(), "+", " ")
			copyFrom = Replace(buffer.ReadASCIIString(), "+", " ")
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) Then
				Call LogGM(.name, "Ha alterado la contraseña de " & UserName)
				
				If migr_LenB(UserName) = 0 Or migr_LenB(copyFrom) = 0 Then
					Call WriteConsoleMsg(UserIndex, "usar /APASS <pjsinpass>@<pjconpass>", FontTypeNames.FONTTYPE_INFO)
				Else
					If Not FileExist(CharPath & UserName & ".chr") Or Not FileExist(CharPath & copyFrom & ".chr") Then
						Call WriteConsoleMsg(UserIndex, "Alguno de los PJs no existe " & UserName & "@" & copyFrom, FontTypeNames.FONTTYPE_INFO)
					Else
						Password = GetVar(CharPath & copyFrom & ".chr", "INIT", "Password")
						Call WriteVar(CharPath & UserName & ".chr", "INIT", "Password", Password)
						
						Call WriteConsoleMsg(UserIndex, "Password de " & UserName & " ha cambiado por la de " & copyFrom, FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handle the "HandleCreateNPC" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleCreateNPC(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/24/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim NpcIndex As Short
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			
			NpcIndex = .incomingData.ReadInteger()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			
			NpcIndex = SpawnNpc(NpcIndex, .Pos, True, False)
			
			If NpcIndex <> 0 Then
				Call LogGM(.name, "Sumoneó a " & Npclist(NpcIndex).name & " en mapa " & .Pos.Map)
			End If
		End With
	End Sub
	
	
	''
	' Handle the "CreateNPCWithRespawn" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleCreateNPCWithRespawn(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/24/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim NpcIndex As Short
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			
			NpcIndex = .incomingData.ReadInteger()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios) Then Exit Sub
			
			NpcIndex = SpawnNpc(NpcIndex, .Pos, True, True)
			
			If NpcIndex <> 0 Then
				Call LogGM(.name, "Sumoneó con respawn " & Npclist(NpcIndex).name & " en mapa " & .Pos.Map)
			End If
		End With
	End Sub
	
	''
	' Handle the "ImperialArmour" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleImperialArmour(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/24/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 4 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim Index As Byte
		Dim ObjIndex As Short
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			
			Index = .incomingData.ReadByte()
			ObjIndex = .incomingData.ReadInteger()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Select Case Index
				Case 1
					ArmaduraImperial1 = ObjIndex
					
				Case 2
					ArmaduraImperial2 = ObjIndex
					
				Case 3
					ArmaduraImperial3 = ObjIndex
					
				Case 4
					TunicaMagoImperial = ObjIndex
			End Select
		End With
	End Sub
	
	''
	' Handle the "ChaosArmour" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleChaosArmour(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/24/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 4 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		Dim Index As Byte
		Dim ObjIndex As Short
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			
			Index = .incomingData.ReadByte()
			ObjIndex = .incomingData.ReadInteger()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Select Case Index
				Case 1
					ArmaduraCaos1 = ObjIndex
					
				Case 2
					ArmaduraCaos2 = ObjIndex
					
				Case 3
					ArmaduraCaos3 = ObjIndex
					
				Case 4
					TunicaMagoCaos = ObjIndex
			End Select
		End With
	End Sub
	
	''
	' Handle the "NavigateToggle" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleNavigateToggle(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 01/12/07
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero) Then Exit Sub
			
			If .flags.Navegando = 1 Then
				.flags.Navegando = 0
			Else
				.flags.Navegando = 1
			End If
			
			'Tell the client that we are navigating.
			Call WriteNavigateToggle(UserIndex)
		End With
	End Sub
	
	''
	' Handle the "ServerOpenToUsersToggle" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleServerOpenToUsersToggle(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/24/06
		'
		'***************************************************
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			If ServerSoloGMs > 0 Then
				Call WriteConsoleMsg(UserIndex, "Servidor habilitado para todos.", FontTypeNames.FONTTYPE_INFO)
				ServerSoloGMs = 0
			Else
				Call WriteConsoleMsg(UserIndex, "Servidor restringido a administradores.", FontTypeNames.FONTTYPE_INFO)
				ServerSoloGMs = 1
			End If
		End With
	End Sub
	
	''
	' Handle the "TurnOffServer" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleTurnOffServer(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/24/06
		'Turns off the server
		'***************************************************
		Dim handle As Short
		
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios Or Declaraciones.PlayerType.RoleMaster) Then Exit Sub
			
			Call LogGM(.name, "/APAGAR")
			Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg("¡¡¡" & .name & " VA A APAGAR EL SERVIDOR!!!", FontTypeNames.FONTTYPE_FIGHT))
			
			'Log
			handle = FreeFile
			FileOpen(handle, My.Application.Info.DirectoryPath & "\logs\Main.log", OpenMode.Append, , OpenShare.Shared)
			
			PrintLine(handle, Today & " " & TimeOfDay & " server apagado por " & .name & ". ")
			
			FileClose(handle)
			
			frmMain.Close()
		End With
	End Sub
	
	''
	' Handle the "TurnCriminal" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleTurnCriminal(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/26/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) Then
				Call LogGM(.name, "/CONDEN " & UserName)
				
				tUser = NameIndex(UserName)
				If tUser > 0 Then Call VolverCriminal(tUser)
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handle the "ResetFactions" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleResetFactions(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 06/09/09
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim tUser As Short
		'UPGRADE_NOTE: Char se actualizó a Char_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim Char_Renamed As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) Then
				Call LogGM(.name, "/RAJAR " & UserName)
				
				tUser = NameIndex(UserName)
				
				If tUser > 0 Then
					Call ResetFacciones(tUser)
				Else
					Char_Renamed = CharPath & UserName & ".chr"
					
					If FileExist(Char_Renamed) Then
						Call WriteVar(Char_Renamed, "FACCIONES", "EjercitoReal", CStr(0))
						Call WriteVar(Char_Renamed, "FACCIONES", "CiudMatados", CStr(0))
						Call WriteVar(Char_Renamed, "FACCIONES", "CrimMatados", CStr(0))
						Call WriteVar(Char_Renamed, "FACCIONES", "EjercitoCaos", CStr(0))
						Call WriteVar(Char_Renamed, "FACCIONES", "FechaIngreso", "No ingresó a ninguna Facción")
						Call WriteVar(Char_Renamed, "FACCIONES", "rArCaos", CStr(0))
						Call WriteVar(Char_Renamed, "FACCIONES", "rArReal", CStr(0))
						Call WriteVar(Char_Renamed, "FACCIONES", "rExCaos", CStr(0))
						Call WriteVar(Char_Renamed, "FACCIONES", "rExReal", CStr(0))
						Call WriteVar(Char_Renamed, "FACCIONES", "recCaos", CStr(0))
						Call WriteVar(Char_Renamed, "FACCIONES", "recReal", CStr(0))
						Call WriteVar(Char_Renamed, "FACCIONES", "Reenlistadas", CStr(0))
						Call WriteVar(Char_Renamed, "FACCIONES", "NivelIngreso", CStr(0))
						Call WriteVar(Char_Renamed, "FACCIONES", "MatadosIngreso", CStr(0))
						Call WriteVar(Char_Renamed, "FACCIONES", "NextRecompensa", CStr(0))
					Else
						Call WriteConsoleMsg(UserIndex, "El personaje " & UserName & " no existe.", FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handle the "RemoveCharFromGuild" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleRemoveCharFromGuild(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/26/06
		'
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim GuildIndex As Short
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) Then
				Call LogGM(.name, "/RAJARCLAN " & UserName)
				
				GuildIndex = modGuilds.m_EcharMiembroDeClan(UserIndex, UserName)
				
				If GuildIndex = 0 Then
					Call WriteConsoleMsg(UserIndex, "No pertenece a ningún clan o es fundador.", FontTypeNames.FONTTYPE_INFO)
				Else
					Call WriteConsoleMsg(UserIndex, "Expulsado.", FontTypeNames.FONTTYPE_INFO)
					Call SendData(modSendData.SendTarget.ToGuildMembers, GuildIndex, PrepareMessageConsoleMsg(UserName & " ha sido expulsado del clan por los administradores del servidor.", FontTypeNames.FONTTYPE_GUILD))
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handle the "RequestCharMail" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleRequestCharMail(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/26/06
		'Request user mail
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim UserName As String
		Dim mail As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			UserName = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) Then
				If FileExist(CharPath & UserName & ".chr") Then
					mail = GetVar(CharPath & UserName & ".chr", "CONTACTO", "email")
					
					Call WriteConsoleMsg(UserIndex, "Last email de " & UserName & ":" & mail, FontTypeNames.FONTTYPE_INFO)
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handle the "SystemMessage" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleSystemMessage(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/29/06
		'Send a message to all the users
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim message As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			message = buffer.ReadASCIIString()
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) Then
				Call LogGM(.name, "Mensaje de sistema:" & message)
				
				Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageShowMessageBox(message))
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handle the "SetMOTD" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleSetMOTD(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 03/31/07
		'Set the MOTD
		'Modified by: Juan Martín Sotuyo Dodero (Maraxus)
		'   - Fixed a bug that prevented from properly setting the new number of lines.
		'   - Fixed a bug that caused the player to be kicked.
		'***************************************************
		If UserList(UserIndex).incomingData.length < 3 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		Dim buffer As New clsByteQueue
		Dim newMOTD As String
		Dim auxiliaryString() As String
		Dim LoopC As Integer
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			newMOTD = buffer.ReadASCIIString()
			auxiliaryString = Split(newMOTD, vbCrLf)
			
			If (Not .flags.Privilegios And Declaraciones.PlayerType.RoleMaster) <> 0 And (.flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios)) Then
				Call LogGM(.name, "Ha fijado un nuevo MOTD")
				
				MaxLines = UBound(auxiliaryString) + 1
				
				'UPGRADE_WARNING: El límite inferior de la matriz MOTD ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
				ReDim MOTD(MaxLines)
				
				Call WriteVar(My.Application.Info.DirectoryPath & "\Dat\Motd.ini", "INIT", "NumLines", CStr(MaxLines))
				
				For LoopC = 1 To MaxLines
					Call WriteVar(My.Application.Info.DirectoryPath & "\Dat\Motd.ini", "Motd", "Line" & CStr(LoopC), auxiliaryString(LoopC - 1))
					
					MOTD(LoopC).texto = auxiliaryString(LoopC - 1)
				Next LoopC
				
				Call WriteConsoleMsg(UserIndex, "Se ha cambiado el MOTD con éxito.", FontTypeNames.FONTTYPE_INFO)
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		error_Renamed = Err.Number
		On Error GoTo 0
		
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Handle the "ChangeMOTD" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleChangeMOTD(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín sotuyo Dodero (Maraxus)
		'Last Modification: 12/29/06
		'Change the MOTD
		'***************************************************
		Dim auxiliaryString As String
		Dim LoopC As Integer
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			If (.flags.Privilegios And (Declaraciones.PlayerType.RoleMaster Or Declaraciones.PlayerType.User Or Declaraciones.PlayerType.Consejero Or Declaraciones.PlayerType.SemiDios)) Then
				Exit Sub
			End If
			
			
			For LoopC = LBound(MOTD) To UBound(MOTD)
				auxiliaryString = auxiliaryString & MOTD(LoopC).texto & vbCrLf
			Next LoopC
			
			If Len(auxiliaryString) >= 2 Then
				If Right(auxiliaryString, 2) = vbCrLf Then
					auxiliaryString = Left(auxiliaryString, Len(auxiliaryString) - 2)
				End If
			End If
			
			Call WriteShowMOTDEditionForm(UserIndex, auxiliaryString)
		End With
	End Sub
	
	''
	' Handle the "Ping" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandlePing(ByVal UserIndex As Short)
		'***************************************************
		'Author: Lucas Tavolaro Ortiz (Tavo)
		'Last Modification: 12/24/06
		'Show guilds messages
		'***************************************************
		With UserList(UserIndex)
			'Remove Packet ID
			Call .incomingData.ReadByte()
			
			Call WritePong(UserIndex)
		End With
	End Sub
	
	''
	' Handle the "SetIniVar" message
	'
	' @param userIndex The index of the user sending the message
	
	Public Sub HandleSetIniVar(ByVal UserIndex As Short)
		'***************************************************
		'Author: Brian Chaia (BrianPr)
		'Last Modification: 01/23/10 (Marco)
		'Modify server.ini
		'***************************************************
		If UserList(UserIndex).incomingData.length < 6 Then
			Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
			Exit Sub
		End If
		
		On Error GoTo Errhandler
		
		Dim buffer As New clsByteQueue
		Dim sLlave As String
		Dim sClave As String
		Dim sValor As String
		Dim sTmp As String
		With UserList(UserIndex)
			'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
			
			Call buffer.CopyBuffer(.incomingData)
			
			'Remove packet ID
			Call buffer.ReadByte()
			
			
			'Obtengo los parámetros
			sLlave = buffer.ReadASCIIString()
			sClave = buffer.ReadASCIIString()
			sValor = buffer.ReadASCIIString()
			
			If .flags.Privilegios And (Declaraciones.PlayerType.Admin Or Declaraciones.PlayerType.Dios) Then
				
				'No podemos modificar [INIT]Dioses ni [Dioses]*
				If (UCase(sLlave) = "INIT" And UCase(sClave) = "DIOSES") Or UCase(sLlave) = "DIOSES" Then
					Call WriteConsoleMsg(UserIndex, "¡No puedes modificar esa información desde aquí!", FontTypeNames.FONTTYPE_INFO)
				Else
					'Obtengo el valor según llave y clave
					sTmp = GetVar(IniPath & "Server.ini", sLlave, sClave)
					
					'Si obtengo un valor escribo en el server.ini
					If migr_LenB(sTmp) Then
						Call WriteVar(IniPath & "Server.ini", sLlave, sClave, sValor)
						Call LogGM(.name, "Modificó en server.ini (" & sLlave & " " & sClave & ") el valor " & sTmp & " por " & sValor)
						Call WriteConsoleMsg(UserIndex, "Modificó " & sLlave & " " & sClave & " a " & sValor & ". Valor anterior " & sTmp, FontTypeNames.FONTTYPE_INFO)
					Else
						Call WriteConsoleMsg(UserIndex, "No existe la llave y/o clave", FontTypeNames.FONTTYPE_INFO)
					End If
				End If
			End If
			
			'If we got here then packet is complete, copy data back to original queue
			Call .incomingData.CopyBuffer(buffer)
		End With
		
Errhandler: 
		'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim error_Renamed As Integer
		
		error_Renamed = Err.Number
		
		On Error GoTo 0
		'Destroy auxiliar buffer
		'UPGRADE_NOTE: El objeto buffer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
		buffer = Nothing
		
		If error_Renamed <> 0 Then Err.Raise(error_Renamed)
	End Sub
	
	''
	' Writes the "Logged" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteLoggedMessage(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "Logged" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.Logged)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "RemoveDialogs" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteRemoveAllDialogs(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "RemoveDialogs" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.RemoveDialogs)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "RemoveCharDialog" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    CharIndex Character whose dialog will be removed.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteRemoveCharDialog(ByVal UserIndex As Short, ByVal CharIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "RemoveCharDialog" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageRemoveCharDialog(CharIndex))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "NavigateToggle" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteNavigateToggle(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "NavigateToggle" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.NavigateToggle)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "Disconnect" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteDisconnect(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "Disconnect" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.Disconnect)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "UserOfferConfirm" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUserOfferConfirm(ByVal UserIndex As Short)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 14/12/2009
		'Writes the "UserOfferConfirm" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.UserOfferConfirm)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	
	''
	' Writes the "CommerceEnd" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteCommerceEnd(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "CommerceEnd" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.CommerceEnd)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "BankEnd" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteBankEnd(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "BankEnd" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.BankEnd)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "CommerceInit" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteCommerceInit(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "CommerceInit" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.CommerceInit)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "BankInit" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteBankInit(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "BankInit" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.BankInit)
		Call UserList(UserIndex).outgoingData.WriteLong(UserList(UserIndex).Stats.Banco)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "UserCommerceInit" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUserCommerceInit(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "UserCommerceInit" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.UserCommerceInit)
		Call UserList(UserIndex).outgoingData.WriteASCIIString(UserList(UserIndex).ComUsu.DestNick)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "UserCommerceEnd" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUserCommerceEnd(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "UserCommerceEnd" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.UserCommerceEnd)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ShowBlacksmithForm" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteShowBlacksmithForm(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ShowBlacksmithForm" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.ShowBlacksmithForm)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ShowCarpenterForm" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteShowCarpenterForm(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ShowCarpenterForm" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.ShowCarpenterForm)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "UpdateSta" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUpdateSta(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "UpdateMana" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.UpdateSta)
			Call .WriteInteger(UserList(UserIndex).Stats.MinSta)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "UpdateMana" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUpdateMana(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "UpdateMana" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.UpdateMana)
			Call .WriteInteger(UserList(UserIndex).Stats.MinMAN)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "UpdateHP" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUpdateHP(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "UpdateMana" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.UpdateHP)
			Call .WriteInteger(UserList(UserIndex).Stats.MinHp)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "UpdateGold" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUpdateGold(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "UpdateGold" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.UpdateGold)
			Call .WriteLong(UserList(UserIndex).Stats.GLD)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "UpdateBankGold" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUpdateBankGold(ByVal UserIndex As Short)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 14/12/2009
		'Writes the "UpdateBankGold" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.UpdateBankGold)
			Call .WriteLong(UserList(UserIndex).Stats.Banco)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	
	''
	' Writes the "UpdateExp" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUpdateExp(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "UpdateExp" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.UpdateExp)
			Call .WriteLong(UserList(UserIndex).Stats.Exp)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "UpdateStrenghtAndDexterity" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUpdateStrenghtAndDexterity(ByVal UserIndex As Short)
		'***************************************************
		'Author: Budi
		'Last Modification: 11/26/09
		'Writes the "UpdateStrenghtAndDexterity" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.UpdateStrenghtAndDexterity)
			Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(Declaraciones.eAtributos.Fuerza))
			Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(Declaraciones.eAtributos.Agilidad))
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	' Writes the "UpdateStrenghtAndDexterity" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUpdateDexterity(ByVal UserIndex As Short)
		'***************************************************
		'Author: Budi
		'Last Modification: 11/26/09
		'Writes the "UpdateStrenghtAndDexterity" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.UpdateDexterity)
			Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(Declaraciones.eAtributos.Agilidad))
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	' Writes the "UpdateStrenghtAndDexterity" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUpdateStrenght(ByVal UserIndex As Short)
		'***************************************************
		'Author: Budi
		'Last Modification: 11/26/09
		'Writes the "UpdateStrenghtAndDexterity" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.UpdateStrenght)
			Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(Declaraciones.eAtributos.Fuerza))
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ChangeMap" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    map The new map to load.
	' @param    version The version of the map in the server to check if client is properly updated.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteChangeMap(ByVal UserIndex As Short, ByVal Map As Short, ByVal version As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ChangeMap" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.ChangeMap)
			Call .WriteInteger(Map)
			Call .WriteInteger(version)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "PosUpdate" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WritePosUpdate(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "PosUpdate" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.PosUpdate)
			Call .WriteByte(UserList(UserIndex).Pos.X)
			Call .WriteByte(UserList(UserIndex).Pos.Y)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ChatOverHead" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    Chat Text to be displayed over the char's head.
	' @param    CharIndex The character uppon which the chat will be displayed.
	' @param    Color The color to be used when displaying the chat.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteChatOverHead(ByVal UserIndex As Short, ByVal Chat As String, ByVal CharIndex As Short, ByVal color As Integer)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ChatOverHead" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageChatOverHead(Chat, CharIndex, color))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ConsoleMsg" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    Chat Text to be displayed over the char's head.
	' @param    FontIndex Index of the FONTTYPE structure to use.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteConsoleMsg(ByVal UserIndex As Short, ByVal Chat As String, ByVal FontIndex As FontTypeNames)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ConsoleMsg" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageConsoleMsg(Chat, FontIndex))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	Public Sub WriteCommerceChat(ByVal UserIndex As Short, ByVal Chat As String, ByVal FontIndex As FontTypeNames)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 05/17/06
		'Writes the "ConsoleMsg" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareCommerceConsoleMsg(Chat, FontIndex))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "GuildChat" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    Chat Text to be displayed over the char's head.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteGuildChat(ByVal UserIndex As Short, ByVal Chat As String)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "GuildChat" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageGuildChat(Chat))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ShowMessageBox" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    Message Text to be displayed in the message box.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteShowMessageBox(ByVal UserIndex As Short, ByVal message As String)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ShowMessageBox" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.ShowMessageBox)
			Call .WriteASCIIString(message)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "UserIndexInServer" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUserIndexInServer(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "UserIndexInServer" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.UserIndexInServer)
			Call .WriteInteger(UserIndex)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "UserCharIndexInServer" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUserCharIndexInServer(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "UserIndexInServer" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.UserCharIndexInServer)
			Call .WriteInteger(UserList(UserIndex).Char_Renamed.CharIndex)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "CharacterCreate" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    body Body index of the new character.
	' @param    head Head index of the new character.
	' @param    heading Heading in which the new character is looking.
	' @param    CharIndex The index of the new character.
	' @param    X X coord of the new character's position.
	' @param    Y Y coord of the new character's position.
	' @param    weapon Weapon index of the new character.
	' @param    shield Shield index of the new character.
	' @param    FX FX index to be displayed over the new character.
	' @param    FXLoops Number of times the FX should be rendered.
	' @param    helmet Helmet index of the new character.
	' @param    name Name of the new character.
	' @param    criminal Determines if the character is a criminal or not.
	' @param    privileges Sets if the character is a normal one or any kind of administrative character.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteCharacterCreate(ByVal UserIndex As Short, ByVal body As Short, ByVal Head As Short, ByVal heading As Declaraciones.eHeading, ByVal CharIndex As Short, ByVal X As Byte, ByVal Y As Byte, ByVal weapon As Short, ByVal shield As Short, ByVal FX As Short, ByVal FXLoops As Short, ByVal helmet As Short, ByVal name As String, ByVal NickColor As Byte, ByVal Privileges As Byte)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "CharacterCreate" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageCharacterCreate(body, Head, heading, CharIndex, X, Y, weapon, shield, FX, FXLoops, helmet, name, NickColor, Privileges))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "CharacterRemove" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    CharIndex Character to be removed.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteCharacterRemove(ByVal UserIndex As Short, ByVal CharIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "CharacterRemove" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageCharacterRemove(CharIndex))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "CharacterMove" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    CharIndex Character which is moving.
	' @param    X X coord of the character's new position.
	' @param    Y Y coord of the character's new position.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteCharacterMove(ByVal UserIndex As Short, ByVal CharIndex As Short, ByVal X As Byte, ByVal Y As Byte)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "CharacterMove" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageCharacterMove(CharIndex, X, Y))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	Public Sub WriteForceCharMove(ByVal UserIndex As Object, ByVal Direccion As Declaraciones.eHeading)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 26/03/2009
		'Writes the "ForceCharMove" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserIndex. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageForceCharMove(Direccion))
		Exit Sub
		
Errhandler: 
		'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserIndex. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserIndex. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "CharacterChange" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    body Body index of the new character.
	' @param    head Head index of the new character.
	' @param    heading Heading in which the new character is looking.
	' @param    CharIndex The index of the new character.
	' @param    weapon Weapon index of the new character.
	' @param    shield Shield index of the new character.
	' @param    FX FX index to be displayed over the new character.
	' @param    FXLoops Number of times the FX should be rendered.
	' @param    helmet Helmet index of the new character.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteCharacterChange(ByVal UserIndex As Short, ByVal body As Short, ByVal Head As Short, ByVal heading As Declaraciones.eHeading, ByVal CharIndex As Short, ByVal weapon As Short, ByVal shield As Short, ByVal FX As Short, ByVal FXLoops As Short, ByVal helmet As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "CharacterChange" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageCharacterChange(body, Head, heading, CharIndex, weapon, shield, FX, FXLoops, helmet))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ObjectCreate" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    GrhIndex Grh of the object.
	' @param    X X coord of the character's new position.
	' @param    Y Y coord of the character's new position.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteObjectCreate(ByVal UserIndex As Short, ByVal GrhIndex As Short, ByVal X As Byte, ByVal Y As Byte)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ObjectCreate" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageObjectCreate(GrhIndex, X, Y))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ObjectDelete" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    X X coord of the character's new position.
	' @param    Y Y coord of the character's new position.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteObjectDelete(ByVal UserIndex As Short, ByVal X As Byte, ByVal Y As Byte)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ObjectDelete" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageObjectDelete(X, Y))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "BlockPosition" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    X X coord of the character's new position.
	' @param    Y Y coord of the character's new position.
	' @param    Blocked True if the position is blocked.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteBlockPosition(ByVal UserIndex As Short, ByVal X As Byte, ByVal Y As Byte, ByVal Blocked As Boolean)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "BlockPosition" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.BlockPosition)
			Call .WriteByte(X)
			Call .WriteByte(Y)
			Call .WriteBoolean(Blocked)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "PlayMidi" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    midi The midi to be played.
	' @param    loops Number of repets for the midi.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WritePlayMidi(ByVal UserIndex As Short, ByVal midi As Byte, Optional ByVal loops As Short = -1)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "PlayMidi" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessagePlayMidi(midi, loops))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "PlayWave" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    wave The wave to be played.
	' @param    X The X position in map coordinates from where the sound comes.
	' @param    Y The Y position in map coordinates from where the sound comes.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WritePlayWave(ByVal UserIndex As Short, ByVal wave As Byte, ByVal X As Byte, ByVal Y As Byte)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 08/08/07
		'Last Modified by: Rapsodius
		'Added X and Y positions for 3D Sounds
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessagePlayWave(wave, X, Y))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "GuildList" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    GuildList List of guilds to be sent.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteGuildList(ByVal UserIndex As Short, ByRef guildList() As String)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "GuildList" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim Tmp As String
		Dim i As Integer
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.guildList)
			
			' Prepare guild name's list
			For i = LBound(guildList) To UBound(guildList)
				Tmp = Tmp & guildList(i) & SEPARATOR
			Next i
			
			If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
			
			Call .WriteASCIIString(Tmp)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "AreaChanged" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteAreaChanged(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "AreaChanged" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.AreaChanged)
			Call .WriteByte(UserList(UserIndex).Pos.X)
			Call .WriteByte(UserList(UserIndex).Pos.Y)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "PauseToggle" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WritePauseToggle(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "PauseToggle" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessagePauseToggle())
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "RainToggle" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteRainToggle(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "RainToggle" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageRainToggle())
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "CreateFX" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    CharIndex Character upon which the FX will be created.
	' @param    FX FX index to be displayed over the new character.
	' @param    FXLoops Number of times the FX should be rendered.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteCreateFX(ByVal UserIndex As Short, ByVal CharIndex As Short, ByVal FX As Short, ByVal FXLoops As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "CreateFX" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageCreateFX(CharIndex, FX, FXLoops))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "UpdateUserStats" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUpdateUserStats(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "UpdateUserStats" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.UpdateUserStats)
			Call .WriteInteger(UserList(UserIndex).Stats.MaxHp)
			Call .WriteInteger(UserList(UserIndex).Stats.MinHp)
			Call .WriteInteger(UserList(UserIndex).Stats.MaxMAN)
			Call .WriteInteger(UserList(UserIndex).Stats.MinMAN)
			Call .WriteInteger(UserList(UserIndex).Stats.MaxSta)
			Call .WriteInteger(UserList(UserIndex).Stats.MinSta)
			Call .WriteLong(UserList(UserIndex).Stats.GLD)
			Call .WriteByte(UserList(UserIndex).Stats.ELV)
			Call .WriteLong(UserList(UserIndex).Stats.ELU)
			Call .WriteLong(UserList(UserIndex).Stats.Exp)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "WorkRequestTarget" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    Skill The skill for which we request a target.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteWorkRequestTarget(ByVal UserIndex As Short, ByVal Skill As Declaraciones.eSkill)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "WorkRequestTarget" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.WorkRequestTarget)
			Call .WriteByte(Skill)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ChangeInventorySlot" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    slot Inventory slot which needs to be updated.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteChangeInventorySlot(ByVal UserIndex As Short, ByVal Slot As Byte)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 3/12/09
		'Writes the "ChangeInventorySlot" message to the given user's outgoing data buffer
		'3/12/09: Budi - Ahora se envia MaxDef y MinDef en lugar de Def
		'***************************************************
		On Error GoTo Errhandler
		Dim ObjIndex As Short
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura obData, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim obData As ObjData
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.ChangeInventorySlot)
			Call .WriteByte(Slot)
			
			
			ObjIndex = UserList(UserIndex).Invent.Object_Renamed(Slot).ObjIndex
			
			If ObjIndex > 0 Then
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto obData. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				obData = ObjData_Renamed(ObjIndex)
			End If
			
			Call .WriteInteger(ObjIndex)
			Call .WriteASCIIString(obData.name)
			Call .WriteInteger(UserList(UserIndex).Invent.Object_Renamed(Slot).Amount)
			Call .WriteBoolean(UserList(UserIndex).Invent.Object_Renamed(Slot).Equipped)
			Call .WriteInteger(obData.GrhIndex)
			Call .WriteByte(obData.OBJType)
			Call .WriteInteger(obData.MaxHIT)
			Call .WriteInteger(obData.MinHIT)
			Call .WriteInteger(obData.MaxDef)
			Call .WriteInteger(obData.MinDef)
			Call .WriteSingle(SalePrice(ObjIndex))
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	Public Sub WriteAddSlots(ByVal UserIndex As Short, ByVal Mochila As Declaraciones.eMochilas)
		'***************************************************
		'Author: Budi
		'Last Modification: 01/12/09
		'Writes the "AddSlots" message to the given user's outgoing data buffer
		'***************************************************
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.AddSlots)
			Call .WriteByte(Mochila)
		End With
	End Sub
	
	
	''
	' Writes the "ChangeBankSlot" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    slot Inventory slot which needs to be updated.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteChangeBankSlot(ByVal UserIndex As Short, ByVal Slot As Byte)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/03/09
		'Writes the "ChangeBankSlot" message to the given user's outgoing data buffer
		'12/03/09: Budi - Ahora se envia MaxDef y MinDef en lugar de sólo Def
		'***************************************************
		On Error GoTo Errhandler
		Dim ObjIndex As Short
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura obData, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim obData As ObjData
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.ChangeBankSlot)
			Call .WriteByte(Slot)
			
			
			ObjIndex = UserList(UserIndex).BancoInvent.Object_Renamed(Slot).ObjIndex
			
			Call .WriteInteger(ObjIndex)
			
			If ObjIndex > 0 Then
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto obData. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				obData = ObjData_Renamed(ObjIndex)
			End If
			
			Call .WriteASCIIString(obData.name)
			Call .WriteInteger(UserList(UserIndex).BancoInvent.Object_Renamed(Slot).Amount)
			Call .WriteInteger(obData.GrhIndex)
			Call .WriteByte(obData.OBJType)
			Call .WriteInteger(obData.MaxHIT)
			Call .WriteInteger(obData.MinHIT)
			Call .WriteInteger(obData.MaxDef)
			Call .WriteInteger(obData.MinDef)
			Call .WriteLong(obData.Valor)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ChangeSpellSlot" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    slot Spell slot to update.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteChangeSpellSlot(ByVal UserIndex As Short, ByVal Slot As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ChangeSpellSlot" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.ChangeSpellSlot)
			Call .WriteByte(Slot)
			Call .WriteInteger(UserList(UserIndex).Stats.UserHechizos(Slot))
			
			If UserList(UserIndex).Stats.UserHechizos(Slot) > 0 Then
				Call .WriteASCIIString(Hechizos(UserList(UserIndex).Stats.UserHechizos(Slot)).Nombre)
			Else
				Call .WriteASCIIString("(None)")
			End If
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "Atributes" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteAttributes(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "Atributes" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.Atributes)
			Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(Declaraciones.eAtributos.Fuerza))
			Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(Declaraciones.eAtributos.Agilidad))
			Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(Declaraciones.eAtributos.Inteligencia))
			Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(Declaraciones.eAtributos.Carisma))
			Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(Declaraciones.eAtributos.Constitucion))
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "BlacksmithWeapons" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteBlacksmithWeapons(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 04/15/2008 (NicoNZ) Habia un error al fijarse los skills del personaje
		'Writes the "BlacksmithWeapons" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		'UPGRADE_NOTE: Obj se actualizó a Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim Obj_Renamed As ObjData
		Dim validIndexes() As Short
		Dim Count As Short
		
		'UPGRADE_WARNING: El límite inferior de la matriz validIndexes ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		ReDim validIndexes(UBound(ArmasHerrero))
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.BlacksmithWeapons)
			
			For i = 1 To UBound(ArmasHerrero)
				' Can the user create this object? If so add it to the list....
				If ObjData_Renamed(ArmasHerrero(i)).SkHerreria <= System.Math.Round(UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Herreria) / ModHerreriA(UserList(UserIndex).clase), 0) Then
					Count = Count + 1
					validIndexes(Count) = i
				End If
			Next i
			
			' Write the number of objects in the list
			Call .WriteInteger(Count)
			
			' Write the needed data of each object
			For i = 1 To Count
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				Obj_Renamed = ObjData_Renamed(ArmasHerrero(validIndexes(i)))
				Call .WriteASCIIString(Obj_Renamed.name)
				Call .WriteInteger(Obj_Renamed.GrhIndex)
				Call .WriteInteger(Obj_Renamed.LingH)
				Call .WriteInteger(Obj_Renamed.LingP)
				Call .WriteInteger(Obj_Renamed.LingO)
				Call .WriteInteger(ArmasHerrero(validIndexes(i)))
				Call .WriteInteger(Obj_Renamed.Upgrade)
			Next i
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "BlacksmithArmors" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteBlacksmithArmors(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 04/15/2008 (NicoNZ) Habia un error al fijarse los skills del personaje
		'Writes the "BlacksmithArmors" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		'UPGRADE_NOTE: Obj se actualizó a Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim Obj_Renamed As ObjData
		Dim validIndexes() As Short
		Dim Count As Short
		
		'UPGRADE_WARNING: El límite inferior de la matriz validIndexes ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		ReDim validIndexes(UBound(ArmadurasHerrero))
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.BlacksmithArmors)
			
			For i = 1 To UBound(ArmadurasHerrero)
				' Can the user create this object? If so add it to the list....
				If ObjData_Renamed(ArmadurasHerrero(i)).SkHerreria <= System.Math.Round(UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Herreria) / ModHerreriA(UserList(UserIndex).clase), 0) Then
					Count = Count + 1
					validIndexes(Count) = i
				End If
			Next i
			
			' Write the number of objects in the list
			Call .WriteInteger(Count)
			
			' Write the needed data of each object
			For i = 1 To Count
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				Obj_Renamed = ObjData_Renamed(ArmadurasHerrero(validIndexes(i)))
				Call .WriteASCIIString(Obj_Renamed.name)
				Call .WriteInteger(Obj_Renamed.GrhIndex)
				Call .WriteInteger(Obj_Renamed.LingH)
				Call .WriteInteger(Obj_Renamed.LingP)
				Call .WriteInteger(Obj_Renamed.LingO)
				Call .WriteInteger(ArmadurasHerrero(validIndexes(i)))
				Call .WriteInteger(Obj_Renamed.Upgrade)
			Next i
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "CarpenterObjects" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteCarpenterObjects(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "CarpenterObjects" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		'UPGRADE_NOTE: Obj se actualizó a Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim Obj_Renamed As ObjData
		Dim validIndexes() As Short
		Dim Count As Short
		
		'UPGRADE_WARNING: El límite inferior de la matriz validIndexes ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		ReDim validIndexes(UBound(ObjCarpintero))
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.CarpenterObjects)
			
			For i = 1 To UBound(ObjCarpintero)
				' Can the user create this object? If so add it to the list....
				If ObjData_Renamed(ObjCarpintero(i)).SkCarpinteria <= UserList(UserIndex).Stats.UserSkills(Declaraciones.eSkill.Carpinteria) \ ModCarpinteria(UserList(UserIndex).clase) Then
					Count = Count + 1
					validIndexes(Count) = i
				End If
			Next i
			
			' Write the number of objects in the list
			Call .WriteInteger(Count)
			
			' Write the needed data of each object
			For i = 1 To Count
				'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Obj_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
				Obj_Renamed = ObjData_Renamed(ObjCarpintero(validIndexes(i)))
				Call .WriteASCIIString(Obj_Renamed.name)
				Call .WriteInteger(Obj_Renamed.GrhIndex)
				Call .WriteInteger(Obj_Renamed.Madera)
				Call .WriteInteger(Obj_Renamed.MaderaElfica)
				Call .WriteInteger(ObjCarpintero(validIndexes(i)))
				Call .WriteInteger(Obj_Renamed.Upgrade)
			Next i
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "RestOK" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteRestOK(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "RestOK" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.RestOK)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ErrorMsg" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    message The error message to be displayed.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteErrorMsg(ByVal UserIndex As Short, ByVal message As String)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ErrorMsg" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageErrorMsg(message))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "Blind" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteBlind(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "Blind" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.Blind)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "Dumb" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteDumb(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "Dumb" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.Dumb)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ShowSignal" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    objIndex Index of the signal to be displayed.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteShowSignal(ByVal UserIndex As Short, ByVal ObjIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ShowSignal" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.ShowSignal)
			Call .WriteASCIIString(ObjData_Renamed(ObjIndex).texto)
			Call .WriteInteger(ObjData_Renamed(ObjIndex).GrhSecundario)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ChangeNPCInventorySlot" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex   User to which the message is intended.
	' @param    slot        The inventory slot in which this item is to be placed.
	' @param    obj         The object to be set in the NPC's inventory window.
	' @param    price       The value the NPC asks for the object.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteChangeNPCInventorySlot(ByVal UserIndex As Short, ByVal Slot As Byte, ByRef Obj As Obj, ByVal price As Single)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/03/09
		'Last Modified by: Budi
		'Writes the "ChangeNPCInventorySlot" message to the given user's outgoing data buffer
		'12/03/09: Budi - Ahora se envia MaxDef y MinDef en lugar de sólo Def
		'***************************************************
		On Error GoTo Errhandler
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura ObjInfo, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim ObjInfo As ObjData
		
		If Obj.ObjIndex >= LBound(ObjData_Renamed) And Obj.ObjIndex <= UBound(ObjData_Renamed) Then
			'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto ObjInfo. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
			ObjInfo = ObjData_Renamed(Obj.ObjIndex)
		End If
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.ChangeNPCInventorySlot)
			Call .WriteByte(Slot)
			Call .WriteASCIIString(ObjInfo.name)
			Call .WriteInteger(Obj.Amount)
			Call .WriteSingle(price)
			Call .WriteInteger(ObjInfo.GrhIndex)
			Call .WriteInteger(Obj.ObjIndex)
			Call .WriteByte(ObjInfo.OBJType)
			Call .WriteInteger(ObjInfo.MaxHIT)
			Call .WriteInteger(ObjInfo.MinHIT)
			Call .WriteInteger(ObjInfo.MaxDef)
			Call .WriteInteger(ObjInfo.MinDef)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "UpdateHungerAndThirst" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUpdateHungerAndThirst(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "UpdateHungerAndThirst" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.UpdateHungerAndThirst)
			Call .WriteByte(UserList(UserIndex).Stats.MaxAGU)
			Call .WriteByte(UserList(UserIndex).Stats.MinAGU)
			Call .WriteByte(UserList(UserIndex).Stats.MaxHam)
			Call .WriteByte(UserList(UserIndex).Stats.MinHam)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "Fame" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteFame(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "Fame" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.Fame)
			
			Call .WriteLong(UserList(UserIndex).Reputacion.AsesinoRep)
			Call .WriteLong(UserList(UserIndex).Reputacion.BandidoRep)
			Call .WriteLong(UserList(UserIndex).Reputacion.BurguesRep)
			Call .WriteLong(UserList(UserIndex).Reputacion.LadronesRep)
			Call .WriteLong(UserList(UserIndex).Reputacion.NobleRep)
			Call .WriteLong(UserList(UserIndex).Reputacion.PlebeRep)
			Call .WriteLong(UserList(UserIndex).Reputacion.Promedio)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "MiniStats" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteMiniStats(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "MiniStats" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.MiniStats)
			
			Call .WriteLong(UserList(UserIndex).Faccion.CiudadanosMatados)
			Call .WriteLong(UserList(UserIndex).Faccion.CriminalesMatados)
			
			'TODO : Este valor es calculable, no debería NI EXISTIR, ya sea en el servidor ni en el cliente!!!
			Call .WriteLong(UserList(UserIndex).Stats.UsuariosMatados)
			
			Call .WriteInteger(UserList(UserIndex).Stats.NPCsMuertos)
			
			Call .WriteByte(UserList(UserIndex).clase)
			Call .WriteLong(UserList(UserIndex).Counters.Pena)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "LevelUp" message to the given user's outgoing data buffer.
	'
	' @param    skillPoints The number of free skill points the player has.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteLevelUp(ByVal UserIndex As Short, ByVal skillPoints As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "LevelUp" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.LevelUp)
			Call .WriteInteger(skillPoints)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "AddForumMsg" message to the given user's outgoing data buffer.
	'
	' @param    title The title of the message to display.
	' @param    message The message to be displayed.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteAddForumMsg(ByVal UserIndex As Short, ByVal ForumType As Declaraciones.eForumType, ByRef Title As String, ByRef Author As String, ByRef message As String)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 02/01/2010
		'Writes the "AddForumMsg" message to the given user's outgoing data buffer
		'02/01/2010: ZaMa - Now sends Author and forum type
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.AddForumMsg)
			Call .WriteByte(ForumType)
			Call .WriteASCIIString(Title)
			Call .WriteASCIIString(Author)
			Call .WriteASCIIString(message)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ShowForumForm" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteShowForumForm(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ShowForumForm" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		
		Dim Visibilidad As Byte
		Dim CanMakeSticky As Byte
		
		With UserList(UserIndex)
			Call .outgoingData.WriteByte(ServerPacketID.ShowForumForm)
			
			Visibilidad = Declaraciones.eForumVisibility.ieGENERAL_MEMBER
			
			If esCaos(UserIndex) Or EsGM(UserIndex) Then
				Visibilidad = Visibilidad Or Declaraciones.eForumVisibility.ieCAOS_MEMBER
			End If
			
			If esArmada(UserIndex) Or EsGM(UserIndex) Then
				Visibilidad = Visibilidad Or Declaraciones.eForumVisibility.ieREAL_MEMBER
			End If
			
			Call .outgoingData.WriteByte(Visibilidad)
			
			' Pueden mandar sticky los gms o los del consejo de armada/caos
			If EsGM(UserIndex) Then
				CanMakeSticky = 2
			ElseIf (.flags.Privilegios And Declaraciones.PlayerType.ChaosCouncil) <> 0 Then 
				CanMakeSticky = 1
			ElseIf (.flags.Privilegios And Declaraciones.PlayerType.RoyalCouncil) <> 0 Then 
				CanMakeSticky = 1
			End If
			
			Call .outgoingData.WriteByte(CanMakeSticky)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "SetInvisible" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    CharIndex The char turning visible / invisible.
	' @param    invisible True if the char is no longer visible, False otherwise.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteSetInvisible(ByVal UserIndex As Short, ByVal CharIndex As Short, ByVal invisible As Boolean)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "SetInvisible" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageSetInvisible(CharIndex, invisible))
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "DiceRoll" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteDiceRoll(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "DiceRoll" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.DiceRoll)
			
			Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(Declaraciones.eAtributos.Fuerza))
			Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(Declaraciones.eAtributos.Agilidad))
			Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(Declaraciones.eAtributos.Inteligencia))
			Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(Declaraciones.eAtributos.Carisma))
			Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(Declaraciones.eAtributos.Constitucion))
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "MeditateToggle" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteMeditateToggle(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "MeditateToggle" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.MeditateToggle)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "BlindNoMore" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteBlindNoMore(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "BlindNoMore" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.BlindNoMore)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "DumbNoMore" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteDumbNoMore(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "DumbNoMore" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.DumbNoMore)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "SendSkills" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteSendSkills(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 11/19/09
		'Writes the "SendSkills" message to the given user's outgoing data buffer
		'11/19/09: Pato - Now send the percentage of progress of the skills.
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		
		With UserList(UserIndex)
			Call .outgoingData.WriteByte(ServerPacketID.SendSkills)
			Call .outgoingData.WriteByte(.clase)
			
			For i = 1 To NUMSKILLS
				Call .outgoingData.WriteByte(UserList(UserIndex).Stats.UserSkills(i))
				If .Stats.UserSkills(i) < MAXSKILLPOINTS Then
					Call .outgoingData.WriteByte(Int(.Stats.ExpSkills(i) * 100 / .Stats.EluSkills(i)))
				Else
					Call .outgoingData.WriteByte(0)
				End If
			Next i
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "TrainerCreatureList" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    npcIndex The index of the requested trainer.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteTrainerCreatureList(ByVal UserIndex As Short, ByVal NpcIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "TrainerCreatureList" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		'UPGRADE_NOTE: str se actualizó a str_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
		Dim str_Renamed As String
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.TrainerCreatureList)
			
			For i = 1 To Npclist(NpcIndex).NroCriaturas
				str_Renamed = str_Renamed & Npclist(NpcIndex).Criaturas(i).NpcName & SEPARATOR
			Next i
			
			If migr_LenB(str_Renamed) > 0 Then str_Renamed = Left(str_Renamed, Len(str_Renamed) - 1)
			
			Call .WriteASCIIString(str_Renamed)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "GuildNews" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    guildNews The guild's news.
	' @param    enemies The list of the guild's enemies.
	' @param    allies The list of the guild's allies.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteGuildNews(ByVal UserIndex As Short, ByVal guildNews As String, ByRef enemies() As String, ByRef allies() As String)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "GuildNews" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		Dim Tmp As String
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.guildNews)
			
			Call .WriteASCIIString(guildNews)
			
			'Prepare enemies' list
			For i = LBound(enemies) To UBound(enemies)
				Tmp = Tmp & enemies(i) & SEPARATOR
			Next i
			
			If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
			
			Call .WriteASCIIString(Tmp)
			
			Tmp = vbNullString
			'Prepare allies' list
			For i = LBound(allies) To UBound(allies)
				Tmp = Tmp & allies(i) & SEPARATOR
			Next i
			
			If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
			
			Call .WriteASCIIString(Tmp)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "OfferDetails" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    details Th details of the Peace proposition.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteOfferDetails(ByVal UserIndex As Short, ByVal details As String)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "OfferDetails" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.OfferDetails)
			
			Call .WriteASCIIString(details)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "AlianceProposalsList" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    guilds The list of guilds which propossed an alliance.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteAlianceProposalsList(ByVal UserIndex As Short, ByRef guilds() As String)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "AlianceProposalsList" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		Dim Tmp As String
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.AlianceProposalsList)
			
			' Prepare guild's list
			For i = LBound(guilds) To UBound(guilds)
				Tmp = Tmp & guilds(i) & SEPARATOR
			Next i
			
			If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
			
			Call .WriteASCIIString(Tmp)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "PeaceProposalsList" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    guilds The list of guilds which propossed peace.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WritePeaceProposalsList(ByVal UserIndex As Short, ByRef guilds() As String)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "PeaceProposalsList" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		Dim Tmp As String
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.PeaceProposalsList)
			
			' Prepare guilds' list
			For i = LBound(guilds) To UBound(guilds)
				Tmp = Tmp & guilds(i) & SEPARATOR
			Next i
			
			If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
			
			Call .WriteASCIIString(Tmp)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "CharacterInfo" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    charName The requested char's name.
	' @param    race The requested char's race.
	' @param    class The requested char's class.
	' @param    gender The requested char's gender.
	' @param    level The requested char's level.
	' @param    gold The requested char's gold.
	' @param    reputation The requested char's reputation.
	' @param    previousPetitions The requested char's previous petitions to enter guilds.
	' @param    currentGuild The requested char's current guild.
	' @param    previousGuilds The requested char's previous guilds.
	' @param    RoyalArmy True if tha char belongs to the Royal Army.
	' @param    CaosLegion True if tha char belongs to the Caos Legion.
	' @param    citicensKilled The number of citicens killed by the requested char.
	' @param    criminalsKilled The number of criminals killed by the requested char.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	'UPGRADE_NOTE: Class se actualizó a Class_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public Sub WriteCharacterInfo(ByVal UserIndex As Short, ByVal charName As String, ByVal race As Declaraciones.eRaza, ByVal Class_Renamed As Declaraciones.eClass, ByVal gender As Declaraciones.eGenero, ByVal level As Byte, ByVal gold As Integer, ByVal bank As Integer, ByVal reputation As Integer, ByVal previousPetitions As String, ByVal currentGuild As String, ByVal previousGuilds As String, ByVal RoyalArmy As Boolean, ByVal CaosLegion As Boolean, ByVal citicensKilled As Integer, ByVal criminalsKilled As Integer)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "CharacterInfo" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.CharacterInfo)
			
			Call .WriteASCIIString(charName)
			Call .WriteByte(race)
			Call .WriteByte(Class_Renamed)
			Call .WriteByte(gender)
			
			Call .WriteByte(level)
			Call .WriteLong(gold)
			Call .WriteLong(bank)
			Call .WriteLong(reputation)
			
			Call .WriteASCIIString(previousPetitions)
			Call .WriteASCIIString(currentGuild)
			Call .WriteASCIIString(previousGuilds)
			
			Call .WriteBoolean(RoyalArmy)
			Call .WriteBoolean(CaosLegion)
			
			Call .WriteLong(citicensKilled)
			Call .WriteLong(criminalsKilled)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "GuildLeaderInfo" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    guildList The list of guild names.
	' @param    memberList The list of the guild's members.
	' @param    guildNews The guild's news.
	' @param    joinRequests The list of chars which requested to join the clan.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteGuildLeaderInfo(ByVal UserIndex As Short, ByRef guildList() As String, ByRef MemberList() As String, ByVal guildNews As String, ByRef joinRequests() As String)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "GuildLeaderInfo" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		Dim Tmp As String
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.GuildLeaderInfo)
			
			' Prepare guild name's list
			For i = LBound(guildList) To UBound(guildList)
				Tmp = Tmp & guildList(i) & SEPARATOR
			Next i
			
			If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
			
			Call .WriteASCIIString(Tmp)
			
			' Prepare guild member's list
			Tmp = vbNullString
			For i = LBound(MemberList) To UBound(MemberList)
				Tmp = Tmp & MemberList(i) & SEPARATOR
			Next i
			
			If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
			
			Call .WriteASCIIString(Tmp)
			
			' Store guild news
			Call .WriteASCIIString(guildNews)
			
			' Prepare the join request's list
			Tmp = vbNullString
			For i = LBound(joinRequests) To UBound(joinRequests)
				Tmp = Tmp & joinRequests(i) & SEPARATOR
			Next i
			
			If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
			
			Call .WriteASCIIString(Tmp)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "GuildLeaderInfo" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    guildList The list of guild names.
	' @param    memberList The list of the guild's members.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteGuildMemberInfo(ByVal UserIndex As Short, ByRef guildList() As String, ByRef MemberList() As String)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 21/02/2010
		'Writes the "GuildMemberInfo" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		Dim Tmp As String
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.GuildMemberInfo)
			
			' Prepare guild name's list
			For i = LBound(guildList) To UBound(guildList)
				Tmp = Tmp & guildList(i) & SEPARATOR
			Next i
			
			If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
			
			Call .WriteASCIIString(Tmp)
			
			' Prepare guild member's list
			Tmp = vbNullString
			For i = LBound(MemberList) To UBound(MemberList)
				Tmp = Tmp & MemberList(i) & SEPARATOR
			Next i
			
			If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
			
			Call .WriteASCIIString(Tmp)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "GuildDetails" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    guildName The requested guild's name.
	' @param    founder The requested guild's founder.
	' @param    foundationDate The requested guild's foundation date.
	' @param    leader The requested guild's current leader.
	' @param    URL The requested guild's website.
	' @param    memberCount The requested guild's member count.
	' @param    electionsOpen True if the clan is electing it's new leader.
	' @param    alignment The requested guild's alignment.
	' @param    enemiesCount The requested guild's enemy count.
	' @param    alliesCount The requested guild's ally count.
	' @param    antifactionPoints The requested guild's number of antifaction acts commited.
	' @param    codex The requested guild's codex.
	' @param    guildDesc The requested guild's description.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteGuildDetails(ByVal UserIndex As Short, ByVal GuildName As String, ByVal founder As String, ByVal foundationDate As String, ByVal leader As String, ByVal URL As String, ByVal memberCount As Short, ByVal electionsOpen As Boolean, ByVal alignment As String, ByVal enemiesCount As Short, ByVal AlliesCount As Short, ByVal antifactionPoints As String, ByRef codex() As String, ByVal guildDesc As String)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "GuildDetails" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		Dim temp As String
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.GuildDetails)
			
			Call .WriteASCIIString(GuildName)
			Call .WriteASCIIString(founder)
			Call .WriteASCIIString(foundationDate)
			Call .WriteASCIIString(leader)
			Call .WriteASCIIString(URL)
			
			Call .WriteInteger(memberCount)
			Call .WriteBoolean(electionsOpen)
			
			Call .WriteASCIIString(alignment)
			
			Call .WriteInteger(enemiesCount)
			Call .WriteInteger(AlliesCount)
			
			Call .WriteASCIIString(antifactionPoints)
			
			For i = LBound(codex) To UBound(codex)
				temp = temp & codex(i) & SEPARATOR
			Next i
			
			If Len(temp) > 1 Then temp = Left(temp, Len(temp) - 1)
			
			Call .WriteASCIIString(temp)
			
			Call .WriteASCIIString(guildDesc)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	
	''
	' Writes the "ShowGuildAlign" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteShowGuildAlign(ByVal UserIndex As Short)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 14/12/2009
		'Writes the "ShowGuildAlign" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.ShowGuildAlign)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ShowGuildFundationForm" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteShowGuildFundationForm(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ShowGuildFundationForm" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.ShowGuildFundationForm)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ParalizeOK" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteParalizeOK(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 08/12/07
		'Last Modified By: Lucas Tavolaro Ortiz (Tavo)
		'Writes the "ParalizeOK" message to the given user's outgoing data buffer
		'And updates user position
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.ParalizeOK)
		Call WritePosUpdate(UserIndex)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ShowUserRequest" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    details DEtails of the char's request.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteShowUserRequest(ByVal UserIndex As Short, ByVal details As String)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ShowUserRequest" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.ShowUserRequest)
			
			Call .WriteASCIIString(details)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "TradeOK" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteTradeOK(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "TradeOK" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.TradeOK)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "BankOK" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteBankOK(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "BankOK" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.BankOK)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ChangeUserTradeSlot" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    ObjIndex The object's index.
	' @param    amount The number of objects offered.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteChangeUserTradeSlot(ByVal UserIndex As Short, ByVal OfferSlot As Byte, ByVal ObjIndex As Short, ByVal Amount As Integer)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 12/03/09
		'Writes the "ChangeUserTradeSlot" message to the given user's outgoing data buffer
		'25/11/2009: ZaMa - Now sends the specific offer slot to be modified.
		'12/03/09: Budi - Ahora se envia MaxDef y MinDef en lugar de sólo Def
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.ChangeUserTradeSlot)
			
			Call .WriteByte(OfferSlot)
			Call .WriteInteger(ObjIndex)
			Call .WriteLong(Amount)
			
			If ObjIndex > 0 Then
				Call .WriteInteger(ObjData_Renamed(ObjIndex).GrhIndex)
				Call .WriteByte(ObjData_Renamed(ObjIndex).OBJType)
				Call .WriteInteger(ObjData_Renamed(ObjIndex).MaxHIT)
				Call .WriteInteger(ObjData_Renamed(ObjIndex).MinHIT)
				Call .WriteInteger(ObjData_Renamed(ObjIndex).MaxDef)
				Call .WriteInteger(ObjData_Renamed(ObjIndex).MinDef)
				Call .WriteLong(SalePrice(ObjIndex))
				Call .WriteASCIIString(ObjData_Renamed(ObjIndex).name)
			Else ' Borra el item
				Call .WriteInteger(0)
				Call .WriteByte(0)
				Call .WriteInteger(0)
				Call .WriteInteger(0)
				Call .WriteInteger(0)
				Call .WriteInteger(0)
				Call .WriteLong(0)
				Call .WriteASCIIString("")
			End If
		End With
		Exit Sub
		
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "SendNight" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteSendNight(ByVal UserIndex As Short, ByVal night As Boolean)
		'***************************************************
		'Author: Fredy Horacio Treboux (liquid)
		'Last Modification: 01/08/07
		'Writes the "SendNight" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.SendNight)
			Call .WriteBoolean(night)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "SpawnList" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    npcNames The names of the creatures that can be spawned.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteSpawnList(ByVal UserIndex As Short, ByRef npcNames() As String)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "SpawnList" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		Dim Tmp As String
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.SpawnList)
			
			For i = LBound(npcNames) To UBound(npcNames)
				Tmp = Tmp & npcNames(i) & SEPARATOR
			Next i
			
			If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
			
			Call .WriteASCIIString(Tmp)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ShowSOSForm" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteShowSOSForm(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ShowSOSForm" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		Dim Tmp As String
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.ShowSOSForm)
			
			For i = 1 To Ayuda.Longitud
				Tmp = Tmp & Ayuda.VerElemento(i) & SEPARATOR
			Next i
			
			If migr_LenB(Tmp) <> 0 Then Tmp = Left(Tmp, Len(Tmp) - 1)
			
			Call .WriteASCIIString(Tmp)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	
	''
	' Writes the "ShowSOSForm" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteShowPartyForm(ByVal UserIndex As Short)
		'***************************************************
		'Author: Budi
		'Last Modification: 11/26/09
		'Writes the "ShowPartyForm" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		Dim Tmp As String
		Dim PI As Short
		Dim members(PARTY_MAXMEMBERS) As Short
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.ShowPartyForm)
			
			PI = UserList(UserIndex).PartyIndex
			Call .WriteByte(CByte(Parties(PI).EsPartyLeader(UserIndex)))
			
			If PI > 0 Then
				Call Parties(PI).ObtenerMiembrosOnline(members)
				For i = 1 To PARTY_MAXMEMBERS
					If members(i) > 0 Then
						Tmp = Tmp & UserList(members(i)).name & " (" & Fix(Parties(PI).MiExperiencia(members(i))) & ")" & SEPARATOR
					End If
				Next i
			End If
			
			If migr_LenB(Tmp) <> 0 Then Tmp = Left(Tmp, Len(Tmp) - 1)
			
			Call .WriteASCIIString(Tmp)
			Call .WriteLong(Parties(PI).ObtenerExperienciaTotal)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ShowMOTDEditionForm" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    currentMOTD The current Message Of The Day.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteShowMOTDEditionForm(ByVal UserIndex As Short, ByVal currentMOTD As String)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ShowMOTDEditionForm" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.ShowMOTDEditionForm)
			
			Call .WriteASCIIString(currentMOTD)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "ShowGMPanelForm" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteShowGMPanelForm(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "ShowGMPanelForm" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.ShowGMPanelForm)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "UserNameList" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    userNameList List of user names.
	' @param    Cant Number of names to send.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WriteUserNameList(ByVal UserIndex As Short, ByRef userNamesList() As String, ByVal cant As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06 NIGO:
		'Writes the "UserNameList" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Dim i As Integer
		Dim Tmp As String
		
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.UserNameList)
			
			' Prepare user's names list
			For i = 1 To cant
				Tmp = Tmp & userNamesList(i) & SEPARATOR
			Next i
			
			If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
			
			Call .WriteASCIIString(Tmp)
		End With
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "Pong" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Sub WritePong(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "Pong" message to the given user's outgoing data buffer
		'***************************************************
		On Error GoTo Errhandler
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.Pong)
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Flushes the outgoing data buffer of the user.
	'
	' @param    UserIndex User whose outgoing data buffer will be flushed.
	
	Public Sub FlushBuffer(ByVal UserIndex As Short)
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Sends all data existing in the buffer
		'***************************************************
		Dim sndData As String
		
		With UserList(UserIndex).outgoingData
			If .length = 0 Then Exit Sub
			
			sndData = .ReadASCIIStringFixed(.length)
			
			Call EnviarDatosASlot(UserIndex, sndData)
		End With
	End Sub
	
	''
	' Prepares the "SetInvisible" message and returns it.
	'
	' @param    CharIndex The char turning visible / invisible.
	' @param    invisible True if the char is no longer visible, False otherwise.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The message is written to no outgoing buffer, but only prepared in a single string to be easily sent to several clients.
	
	Public Function PrepareMessageSetInvisible(ByVal CharIndex As Short, ByVal invisible As Boolean) As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Prepares the "SetInvisible" message and returns it.
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.SetInvisible)
			
			Call .WriteInteger(CharIndex)
			Call .WriteBoolean(invisible)
			
			PrepareMessageSetInvisible = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	Public Function PrepareMessageCharacterChangeNick(ByVal CharIndex As Short, ByVal newNick As String) As String
		'***************************************************
		'Author: Budi
		'Last Modification: 07/23/09
		'Prepares the "Change Nick" message and returns it.
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.CharacterChangeNick)
			
			Call .WriteInteger(CharIndex)
			Call .WriteASCIIString(newNick)
			
			PrepareMessageCharacterChangeNick = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "ChatOverHead" message and returns it.
	'
	' @param    Chat Text to be displayed over the char's head.
	' @param    CharIndex The character uppon which the chat will be displayed.
	' @param    Color The color to be used when displaying the chat.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The message is written to no outgoing buffer, but only prepared in a single string to be easily sent to several clients.
	
	Public Function PrepareMessageChatOverHead(ByVal Chat As String, ByVal CharIndex As Short, ByVal color As Integer) As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Prepares the "ChatOverHead" message and returns it.
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.ChatOverHead)
			Call .WriteASCIIString(Chat)
			Call .WriteInteger(CharIndex)
			
			' Write rgb channels and save one byte from long :D
			Call .WriteByte(color And &HFF)
			Call .WriteByte((color And &HFF00) \ &H100)
			Call .WriteByte((color And &HFF0000) \ &H10000)
			
			PrepareMessageChatOverHead = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "ConsoleMsg" message and returns it.
	'
	' @param    Chat Text to be displayed over the char's head.
	' @param    FontIndex Index of the FONTTYPE structure to use.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessageConsoleMsg(ByVal Chat As String, ByVal FontIndex As FontTypeNames) As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Prepares the "ConsoleMsg" message and returns it.
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.ConsoleMsg)
			Call .WriteASCIIString(Chat)
			Call .WriteByte(FontIndex)
			
			PrepareMessageConsoleMsg = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	Public Function PrepareCommerceConsoleMsg(ByRef Chat As String, ByVal FontIndex As FontTypeNames) As String
		'***************************************************
		'Author: ZaMa
		'Last Modification: 03/12/2009
		'Prepares the "CommerceConsoleMsg" message and returns it.
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.CommerceChat)
			Call .WriteASCIIString(Chat)
			Call .WriteByte(FontIndex)
			
			PrepareCommerceConsoleMsg = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "CreateFX" message and returns it.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    CharIndex Character upon which the FX will be created.
	' @param    FX FX index to be displayed over the new character.
	' @param    FXLoops Number of times the FX should be rendered.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessageCreateFX(ByVal CharIndex As Short, ByVal FX As Short, ByVal FXLoops As Short) As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Prepares the "CreateFX" message and returns it
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.CreateFX)
			Call .WriteInteger(CharIndex)
			Call .WriteInteger(FX)
			Call .WriteInteger(FXLoops)
			
			PrepareMessageCreateFX = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "PlayWave" message and returns it.
	'
	' @param    wave The wave to be played.
	' @param    X The X position in map coordinates from where the sound comes.
	' @param    Y The Y position in map coordinates from where the sound comes.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessagePlayWave(ByVal wave As Byte, ByVal X As Byte, ByVal Y As Byte) As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 08/08/07
		'Last Modified by: Rapsodius
		'Added X and Y positions for 3D Sounds
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.PlayWave)
			Call .WriteByte(wave)
			Call .WriteByte(X)
			Call .WriteByte(Y)
			
			PrepareMessagePlayWave = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "GuildChat" message and returns it.
	'
	' @param    Chat Text to be displayed over the char's head.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessageGuildChat(ByVal Chat As String) As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Prepares the "GuildChat" message and returns it
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.GuildChat)
			Call .WriteASCIIString(Chat)
			
			PrepareMessageGuildChat = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "ShowMessageBox" message and returns it.
	'
	' @param    Message Text to be displayed in the message box.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessageShowMessageBox(ByVal Chat As String) As String
		'***************************************************
		'Author: Fredy Horacio Treboux (liquid)
		'Last Modification: 01/08/07
		'Prepares the "ShowMessageBox" message and returns it
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.ShowMessageBox)
			Call .WriteASCIIString(Chat)
			
			PrepareMessageShowMessageBox = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	
	''
	' Prepares the "PlayMidi" message and returns it.
	'
	' @param    midi The midi to be played.
	' @param    loops Number of repets for the midi.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessagePlayMidi(ByVal midi As Byte, Optional ByVal loops As Short = -1) As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Prepares the "GuildChat" message and returns it
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.PlayMidi)
			Call .WriteByte(midi)
			Call .WriteInteger(loops)
			
			PrepareMessagePlayMidi = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "PauseToggle" message and returns it.
	'
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessagePauseToggle() As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Prepares the "PauseToggle" message and returns it
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.PauseToggle)
			PrepareMessagePauseToggle = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "RainToggle" message and returns it.
	'
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessageRainToggle() As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Prepares the "RainToggle" message and returns it
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.RainToggle)
			
			PrepareMessageRainToggle = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "ObjectDelete" message and returns it.
	'
	' @param    X X coord of the character's new position.
	' @param    Y Y coord of the character's new position.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessageObjectDelete(ByVal X As Byte, ByVal Y As Byte) As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Prepares the "ObjectDelete" message and returns it
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.ObjectDelete)
			Call .WriteByte(X)
			Call .WriteByte(Y)
			
			PrepareMessageObjectDelete = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "BlockPosition" message and returns it.
	'
	' @param    X X coord of the tile to block/unblock.
	' @param    Y Y coord of the tile to block/unblock.
	' @param    Blocked Blocked status of the tile
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessageBlockPosition(ByVal X As Byte, ByVal Y As Byte, ByVal Blocked As Boolean) As String
		'***************************************************
		'Author: Fredy Horacio Treboux (liquid)
		'Last Modification: 01/08/07
		'Prepares the "BlockPosition" message and returns it
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.BlockPosition)
			Call .WriteByte(X)
			Call .WriteByte(Y)
			Call .WriteBoolean(Blocked)
			
			PrepareMessageBlockPosition = .ReadASCIIStringFixed(.length)
		End With
		
	End Function
	
	''
	' Prepares the "ObjectCreate" message and returns it.
	'
	' @param    GrhIndex Grh of the object.
	' @param    X X coord of the character's new position.
	' @param    Y Y coord of the character's new position.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessageObjectCreate(ByVal GrhIndex As Short, ByVal X As Byte, ByVal Y As Byte) As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'prepares the "ObjectCreate" message and returns it
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.ObjectCreate)
			Call .WriteByte(X)
			Call .WriteByte(Y)
			Call .WriteInteger(GrhIndex)
			
			PrepareMessageObjectCreate = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "CharacterRemove" message and returns it.
	'
	' @param    CharIndex Character to be removed.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessageCharacterRemove(ByVal CharIndex As Short) As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Prepares the "CharacterRemove" message and returns it
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.CharacterRemove)
			Call .WriteInteger(CharIndex)
			
			PrepareMessageCharacterRemove = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "RemoveCharDialog" message and returns it.
	'
	' @param    CharIndex Character whose dialog will be removed.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessageRemoveCharDialog(ByVal CharIndex As Short) As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Writes the "RemoveCharDialog" message to the given user's outgoing data buffer
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.RemoveCharDialog)
			Call .WriteInteger(CharIndex)
			
			PrepareMessageRemoveCharDialog = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Writes the "CharacterCreate" message to the given user's outgoing data buffer.
	'
	' @param    body Body index of the new character.
	' @param    head Head index of the new character.
	' @param    heading Heading in which the new character is looking.
	' @param    CharIndex The index of the new character.
	' @param    X X coord of the new character's position.
	' @param    Y Y coord of the new character's position.
	' @param    weapon Weapon index of the new character.
	' @param    shield Shield index of the new character.
	' @param    FX FX index to be displayed over the new character.
	' @param    FXLoops Number of times the FX should be rendered.
	' @param    helmet Helmet index of the new character.
	' @param    name Name of the new character.
	' @param    NickColor Determines if the character is a criminal or not, and if can be atacked by someone
	' @param    privileges Sets if the character is a normal one or any kind of administrative character.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessageCharacterCreate(ByVal body As Short, ByVal Head As Short, ByVal heading As Declaraciones.eHeading, ByVal CharIndex As Short, ByVal X As Byte, ByVal Y As Byte, ByVal weapon As Short, ByVal shield As Short, ByVal FX As Short, ByVal FXLoops As Short, ByVal helmet As Short, ByVal name As String, ByVal NickColor As Byte, ByVal Privileges As Byte) As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Prepares the "CharacterCreate" message and returns it
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.CharacterCreate)
			
			Call .WriteInteger(CharIndex)
			Call .WriteInteger(body)
			Call .WriteInteger(Head)
			Call .WriteByte(heading)
			Call .WriteByte(X)
			Call .WriteByte(Y)
			Call .WriteInteger(weapon)
			Call .WriteInteger(shield)
			Call .WriteInteger(helmet)
			Call .WriteInteger(FX)
			Call .WriteInteger(FXLoops)
			Call .WriteASCIIString(name)
			Call .WriteByte(NickColor)
			Call .WriteByte(Privileges)
			
			PrepareMessageCharacterCreate = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "CharacterChange" message and returns it.
	'
	' @param    body Body index of the new character.
	' @param    head Head index of the new character.
	' @param    heading Heading in which the new character is looking.
	' @param    CharIndex The index of the new character.
	' @param    weapon Weapon index of the new character.
	' @param    shield Shield index of the new character.
	' @param    FX FX index to be displayed over the new character.
	' @param    FXLoops Number of times the FX should be rendered.
	' @param    helmet Helmet index of the new character.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessageCharacterChange(ByVal body As Short, ByVal Head As Short, ByVal heading As Declaraciones.eHeading, ByVal CharIndex As Short, ByVal weapon As Short, ByVal shield As Short, ByVal FX As Short, ByVal FXLoops As Short, ByVal helmet As Short) As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Prepares the "CharacterChange" message and returns it
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.CharacterChange)
			
			Call .WriteInteger(CharIndex)
			Call .WriteInteger(body)
			Call .WriteInteger(Head)
			Call .WriteByte(heading)
			Call .WriteInteger(weapon)
			Call .WriteInteger(shield)
			Call .WriteInteger(helmet)
			Call .WriteInteger(FX)
			Call .WriteInteger(FXLoops)
			
			PrepareMessageCharacterChange = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "CharacterMove" message and returns it.
	'
	' @param    CharIndex Character which is moving.
	' @param    X X coord of the character's new position.
	' @param    Y Y coord of the character's new position.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessageCharacterMove(ByVal CharIndex As Short, ByVal X As Byte, ByVal Y As Byte) As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Prepares the "CharacterMove" message and returns it
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.CharacterMove)
			Call .WriteInteger(CharIndex)
			Call .WriteByte(X)
			Call .WriteByte(Y)
			
			PrepareMessageCharacterMove = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	Public Function PrepareMessageForceCharMove(ByVal Direccion As Declaraciones.eHeading) As String
		'***************************************************
		'Author: ZaMa
		'Last Modification: 26/03/2009
		'Prepares the "ForceCharMove" message and returns it
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.ForceCharMove)
			Call .WriteByte(Direccion)
			
			PrepareMessageForceCharMove = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "UpdateTagAndStatus" message and returns it.
	'
	' @param    CharIndex Character which is moving.
	' @param    X X coord of the character's new position.
	' @param    Y Y coord of the character's new position.
	' @return   The formated message ready to be writen as is on outgoing buffers.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessageUpdateTagAndStatus(ByVal UserIndex As Short, ByVal NickColor As Byte, ByRef Tag As String) As String
		'***************************************************
		'Author: Alejandro Salvo (Salvito)
		'Last Modification: 04/07/07
		'Last Modified By: Juan Martín Sotuyo Dodero (Maraxus)
		'Prepares the "UpdateTagAndStatus" message and returns it
		'15/01/2010: ZaMa - Now sends the nick color instead of the status.
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.UpdateTagAndStatus)
			
			Call .WriteInteger(UserList(UserIndex).Char_Renamed.CharIndex)
			Call .WriteByte(NickColor)
			Call .WriteASCIIString(Tag)
			
			PrepareMessageUpdateTagAndStatus = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Prepares the "ErrorMsg" message and returns it.
	'
	' @param    message The error message to be displayed.
	' @remarks  The data is not actually sent until the buffer is properly flushed.
	
	Public Function PrepareMessageErrorMsg(ByVal message As String) As String
		'***************************************************
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Prepares the "ErrorMsg" message and returns it
		'***************************************************
		With auxiliarBuffer
			Call .WriteByte(ServerPacketID.ErrorMsg)
			Call .WriteASCIIString(message)
			
			PrepareMessageErrorMsg = .ReadASCIIStringFixed(.length)
		End With
	End Function
	
	''
	' Writes the "StopWorking" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	
	Public Sub WriteStopWorking(ByVal UserIndex As Short)
		'***************************************************
		'Author: ZaMa
		'Last Modification: 21/02/2010
		'
		'***************************************************
		On Error GoTo Errhandler
		
		Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.StopWorking)
		
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
	
	''
	' Writes the "CancelOfferItem" message to the given user's outgoing data buffer.
	'
	' @param    UserIndex User to which the message is intended.
	' @param    Slot      The slot to cancel.
	
	Public Sub WriteCancelOfferItem(ByVal UserIndex As Short, ByVal Slot As Byte)
		'***************************************************
		'Author: Torres Patricio (Pato)
		'Last Modification: 05/03/2010
		'
		'***************************************************
		On Error GoTo Errhandler
		With UserList(UserIndex).outgoingData
			Call .WriteByte(ServerPacketID.CancelOfferItem)
			Call .WriteByte(Slot)
		End With
		
		Exit Sub
		
Errhandler: 
		If Err.Number = UserList(UserIndex).outgoingData.NotEnoughSpaceErrCode Then
			Call FlushBuffer(UserIndex)
			Resume 
		End If
	End Sub
End Module
