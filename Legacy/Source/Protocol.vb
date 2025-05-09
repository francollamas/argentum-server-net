Option Strict Off
Option Explicit On

Imports System.Drawing

Module Protocol
    '**************************************************************
    ' Protocol.bas - Handles all incoming / outgoing messages for client-server communications.
    ' Uses a binary protocol designed by myself.
    '
    ' Designed and implemented by Juan Martín Sotuyo Dodero (Maraxus)
    ' (juansotuyo@gmail.com)
    '**************************************************************

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
    Private ReadOnly SEPARATOR As Char = Chr(0)

    ''
    'Auxiliar ByteQueue used as buffer to generate messages not intended to be sent right away.
    'Specially usefull to create a message once and send it over to several clients.
    Private ReadOnly auxiliarBuffer As New clsByteQueue


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

    Public Sub HandleIncomingData(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 01/09/07
        '
        '***************************************************
        Try
            Dim packetID As Byte

            packetID = UserList(UserIndex).incomingData.PeekByte()

            'Does the packet requires a logged user??
            If _
                Not _
                (packetID = ClientPacketID.ThrowDices Or packetID = ClientPacketID.LoginExistingChar Or
                 packetID = ClientPacketID.LoginNewChar) Then

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
                Call _
                    LogError(
                        "Error: " & Err.Number & " [" & Err.Description & "] " & " Source: " & Err.Source & vbTab &
                        " HelpFile: " & Err.HelpFile & vbTab & " HelpContext: " & Err.HelpContext & vbTab &
                        " LastDllError: " & Err.LastDllError & vbTab & " - UserIndex: " & UserIndex &
                        " - producido al manejar el paquete: " & CStr(packetID))
                Call CloseSocket(UserIndex)

            Else
                'Flush buffer - send everything that has been written
                Call FlushBuffer(UserIndex)
            End If

        Catch ex As Exception
            Console.WriteLine("Error in HandleIncomingData: " & ex.Message)
        End Try
    End Sub


    Public Sub WriteMultiMessage(UserIndex As Short, MessageIndex As Short,
                                 Optional ByVal Arg1 As Integer = 0, Optional ByVal Arg2 As Integer = 0,
                                 Optional ByVal Arg3 As Integer = 0, Optional ByVal StringArg1 As String = "")
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.MultiMessage)
                    Call .WriteByte(MessageIndex)

                    Select Case MessageIndex
                        Case eMessages.DontSeeAnything, eMessages.NPCSwing,
                                     eMessages.NPCKillUser, eMessages.BlockedWithShieldUser,
                                     eMessages.BlockedWithShieldother, eMessages.UserSwing,
                                     eMessages.SafeModeOn, eMessages.SafeModeOff,
                                     eMessages.ResuscitationSafeOff, eMessages.ResuscitationSafeOn,
                                     eMessages.NobilityLost, eMessages.CantUseWhileMeditating,
                                     eMessages.CancelHome, eMessages.FinishHome

                        Case eMessages.NPCHitUser
                            Call .WriteByte(Arg1) 'Target
                            Call .WriteInteger(Arg2) 'damage

                        Case eMessages.UserHitNPC
                            Call .WriteLong(Arg1) 'damage

                        Case eMessages.UserAttackedSwing
                            Call .WriteInteger(UserList(Arg1).Char_Renamed.CharIndex)

                        Case eMessages.UserHittedByUser
                            Call .WriteInteger(Arg1) 'AttackerIndex
                            Call .WriteByte(Arg2) 'Target
                            Call .WriteInteger(Arg3) 'damage

                        Case eMessages.UserHittedUser
                            Call .WriteInteger(Arg1) 'AttackerIndex
                            Call .WriteByte(Arg2) 'Target
                            Call .WriteInteger(Arg3) 'damage

                        Case eMessages.WorkRequestTarget
                            Call .WriteByte(Arg1) 'skill

                        Case eMessages.HaveKilledUser _
'"Has matado a " & UserList(VictimIndex).name & "!" "Has ganado " & DaExp & " puntos de experiencia."
                            Call .WriteInteger(UserList(Arg1).Char_Renamed.CharIndex) 'VictimIndex
                            Call .WriteLong(Arg2) 'Expe

                        Case eMessages.UserKill '"¡" & .name & " te ha matado!"
                            Call .WriteInteger(UserList(Arg1).Char_Renamed.CharIndex) 'AttackerIndex

                        Case eMessages.EarnExp

                        Case eMessages.Home
                            Call .WriteByte(CByte(Arg1))
                            Call .WriteInteger(CShort(Arg2))
                            'El cliente no conoce nada sobre nombre de mapas y hogares, por lo tanto _
                            'hasta que no se pasen los dats e .INFs al cliente, esto queda así.
                            Call .WriteASCIIString(StringArg1) 'Call .WriteByte(CByte(Arg2))

                    End Select
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub


    Private Sub HandleGMCommands(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'UPGRADE_NOTE: Command se actualizó a Command_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim Command_Renamed As Byte

        Try

            With UserList(UserIndex)
                Call .incomingData.ReadByte()

                Command_Renamed = .incomingData.PeekByte

                Select Case Command_Renamed
                    Case eGMCommands.GMMessage '/GMSG
                        Call HandleGMMessage(UserIndex)

                    Case eGMCommands.showName '/SHOWNAME
                        Call HandleShowName(UserIndex)

                    Case eGMCommands.OnlineRoyalArmy
                        Call HandleOnlineRoyalArmy(UserIndex)

                    Case eGMCommands.OnlineChaosLegion '/ONLINECAOS
                        Call HandleOnlineChaosLegion(UserIndex)

                    Case eGMCommands.GoNearby '/IRCERCA
                        Call HandleGoNearby(UserIndex)

                    Case eGMCommands.comment '/REM
                        Call HandleComment(UserIndex)

                    Case eGMCommands.serverTime '/HORA
                        Call HandleServerTime(UserIndex)

                    Case eGMCommands.Where '/DONDE
                        Call HandleWhere(UserIndex)

                    Case eGMCommands.CreaturesInMap '/NENE
                        Call HandleCreaturesInMap(UserIndex)

                    Case eGMCommands.WarpMeToTarget '/TELEPLOC
                        Call HandleWarpMeToTarget(UserIndex)

                    Case eGMCommands.WarpChar '/TELEP
                        Call HandleWarpChar(UserIndex)

                    Case eGMCommands.Silence '/SILENCIAR
                        Call HandleSilence(UserIndex)

                    Case eGMCommands.SOSShowList '/SHOW SOS
                        Call HandleSOSShowList(UserIndex)

                    Case eGMCommands.SOSRemove 'SOSDONE
                        Call HandleSOSRemove(UserIndex)

                    Case eGMCommands.GoToChar '/IRA
                        Call HandleGoToChar(UserIndex)

                    Case eGMCommands.invisible '/INVISIBLE
                        Call HandleInvisible(UserIndex)

                    Case eGMCommands.GMPanel '/PANELGM
                        Call HandleGMPanel(UserIndex)

                    Case eGMCommands.RequestUserList 'LISTUSU
                        Call HandleRequestUserList(UserIndex)

                    Case eGMCommands.Working '/TRABAJANDO
                        Call HandleWorking(UserIndex)

                    Case eGMCommands.Hiding '/OCULTANDO
                        Call HandleHiding(UserIndex)

                    Case eGMCommands.Jail '/CARCEL
                        Call HandleJail(UserIndex)

                    Case eGMCommands.KillNPC '/RMATA
                        Call HandleKillNPC(UserIndex)

                    Case eGMCommands.WarnUser '/ADVERTENCIA
                        Call HandleWarnUser(UserIndex)

                    Case eGMCommands.EditChar '/MOD
                        Call HandleEditChar(UserIndex)

                    Case eGMCommands.RequestCharInfo '/INFO
                        Call HandleRequestCharInfo(UserIndex)

                    Case eGMCommands.RequestCharStats '/STAT
                        Call HandleRequestCharStats(UserIndex)

                    Case eGMCommands.RequestCharGold '/BAL
                        Call HandleRequestCharGold(UserIndex)

                    Case eGMCommands.RequestCharInventory '/INV
                        Call HandleRequestCharInventory(UserIndex)

                    Case eGMCommands.RequestCharBank '/BOV
                        Call HandleRequestCharBank(UserIndex)

                    Case eGMCommands.RequestCharSkills '/SKILLS
                        Call HandleRequestCharSkills(UserIndex)

                    Case eGMCommands.ReviveChar '/REVIVIR
                        Call HandleReviveChar(UserIndex)

                    Case eGMCommands.OnlineGM '/ONLINEGM
                        Call HandleOnlineGM(UserIndex)

                    Case eGMCommands.OnlineMap '/ONLINEMAP
                        Call HandleOnlineMap(UserIndex)

                    Case eGMCommands.Forgive '/PERDON
                        Call HandleForgive(UserIndex)

                    Case eGMCommands.Kick '/ECHAR
                        Call HandleKick(UserIndex)

                    Case eGMCommands.Execute '/EJECUTAR
                        Call HandleExecute(UserIndex)

                    Case eGMCommands.BanChar '/BAN
                        Call HandleBanChar(UserIndex)

                    Case eGMCommands.UnbanChar '/UNBAN
                        Call HandleUnbanChar(UserIndex)

                    Case eGMCommands.NPCFollow '/SEGUIR
                        Call HandleNPCFollow(UserIndex)

                    Case eGMCommands.SummonChar '/SUM
                        Call HandleSummonChar(UserIndex)

                    Case eGMCommands.SpawnListRequest '/CC
                        Call HandleSpawnListRequest(UserIndex)

                    Case eGMCommands.SpawnCreature 'SPA
                        Call HandleSpawnCreature(UserIndex)

                    Case eGMCommands.ResetNPCInventory '/RESETINV
                        Call HandleResetNPCInventory(UserIndex)

                    Case eGMCommands.CleanWorld '/LIMPIAR
                        Call HandleCleanWorld(UserIndex)

                    Case eGMCommands.ServerMessage '/RMSG
                        Call HandleServerMessage(UserIndex)

                    Case eGMCommands.NickToIP '/NICK2IP
                        Call HandleNickToIP(UserIndex)

                    Case eGMCommands.IPToNick '/IP2NICK
                        Call HandleIPToNick(UserIndex)

                    Case eGMCommands.GuildOnlineMembers '/ONCLAN
                        Call HandleGuildOnlineMembers(UserIndex)

                    Case eGMCommands.TeleportCreate '/CT
                        Call HandleTeleportCreate(UserIndex)

                    Case eGMCommands.TeleportDestroy '/DT
                        Call HandleTeleportDestroy(UserIndex)

                    Case eGMCommands.RainToggle '/LLUVIA
                        Call HandleRainToggle(UserIndex)

                    Case eGMCommands.SetCharDescription '/SETDESC
                        Call HandleSetCharDescription(UserIndex)

                    Case eGMCommands.ForceMIDIToMap '/FORCEMIDIMAP
                        Call HanldeForceMIDIToMap(UserIndex)

                    Case eGMCommands.ForceWAVEToMap '/FORCEWAVMAP
                        Call HandleForceWAVEToMap(UserIndex)

                    Case eGMCommands.RoyalArmyMessage '/REALMSG
                        Call HandleRoyalArmyMessage(UserIndex)

                    Case eGMCommands.ChaosLegionMessage '/CAOSMSG
                        Call HandleChaosLegionMessage(UserIndex)

                    Case eGMCommands.CitizenMessage '/CIUMSG
                        Call HandleCitizenMessage(UserIndex)

                    Case eGMCommands.CriminalMessage '/CRIMSG
                        Call HandleCriminalMessage(UserIndex)

                    Case eGMCommands.TalkAsNPC '/TALKAS
                        Call HandleTalkAsNPC(UserIndex)

                    Case eGMCommands.DestroyAllItemsInArea '/MASSDEST
                        Call HandleDestroyAllItemsInArea(UserIndex)

                    Case eGMCommands.AcceptRoyalCouncilMember '/ACEPTCONSE
                        Call HandleAcceptRoyalCouncilMember(UserIndex)

                    Case eGMCommands.AcceptChaosCouncilMember '/ACEPTCONSECAOS
                        Call HandleAcceptChaosCouncilMember(UserIndex)

                    Case eGMCommands.ItemsInTheFloor '/PISO
                        Call HandleItemsInTheFloor(UserIndex)

                    Case eGMCommands.MakeDumb '/ESTUPIDO
                        Call HandleMakeDumb(UserIndex)

                    Case eGMCommands.MakeDumbNoMore '/NOESTUPIDO
                        Call HandleMakeDumbNoMore(UserIndex)

                    Case eGMCommands.DumpIPTables '/DUMPSECURITY
                        Call HandleDumpIPTables(UserIndex)

                    Case eGMCommands.CouncilKick '/KICKCONSE
                        Call HandleCouncilKick(UserIndex)

                    Case eGMCommands.SetTrigger '/TRIGGER
                        Call HandleSetTrigger(UserIndex)

                    Case eGMCommands.AskTrigger '/TRIGGER with no args
                        Call HandleAskTrigger(UserIndex)

                    Case eGMCommands.BannedIPList '/BANIPLIST
                        Call HandleBannedIPList(UserIndex)

                    Case eGMCommands.BannedIPReload '/BANIPRELOAD
                        Call HandleBannedIPReload(UserIndex)

                    Case eGMCommands.GuildMemberList '/MIEMBROSCLAN
                        Call HandleGuildMemberList(UserIndex)

                    Case eGMCommands.GuildBan '/BANCLAN
                        Call HandleGuildBan(UserIndex)

                    Case eGMCommands.BanIP '/BANIP
                        Call HandleBanIP(UserIndex)

                    Case eGMCommands.UnbanIP '/UNBANIP
                        Call HandleUnbanIP(UserIndex)

                    Case eGMCommands.CreateItem '/CI
                        Call HandleCreateItem(UserIndex)

                    Case eGMCommands.DestroyItems '/DEST
                        Call HandleDestroyItems(UserIndex)

                    Case eGMCommands.ChaosLegionKick '/NOCAOS
                        Call HandleChaosLegionKick(UserIndex)

                    Case eGMCommands.RoyalArmyKick '/NOREAL
                        Call HandleRoyalArmyKick(UserIndex)

                    Case eGMCommands.ForceMIDIAll '/FORCEMIDI
                        Call HandleForceMIDIAll(UserIndex)

                    Case eGMCommands.ForceWAVEAll '/FORCEWAV
                        Call HandleForceWAVEAll(UserIndex)

                    Case eGMCommands.RemovePunishment '/BORRARPENA
                        Call HandleRemovePunishment(UserIndex)

                    Case eGMCommands.TileBlockedToggle '/BLOQ
                        Call HandleTileBlockedToggle(UserIndex)

                    Case eGMCommands.KillNPCNoRespawn '/MATA
                        Call HandleKillNPCNoRespawn(UserIndex)

                    Case eGMCommands.KillAllNearbyNPCs '/MASSKILL
                        Call HandleKillAllNearbyNPCs(UserIndex)

                    Case eGMCommands.LastIP '/LASTIP
                        Call HandleLastIP(UserIndex)

                    Case eGMCommands.ChangeMOTD '/MOTDCAMBIA
                        Call HandleChangeMOTD(UserIndex)

                    Case eGMCommands.SetMOTD 'ZMOTD
                        Call HandleSetMOTD(UserIndex)

                    Case eGMCommands.SystemMessage '/SMSG
                        Call HandleSystemMessage(UserIndex)

                    Case eGMCommands.CreateNPC '/ACC
                        Call HandleCreateNPC(UserIndex)

                    Case eGMCommands.CreateNPCWithRespawn '/RACC
                        Call HandleCreateNPCWithRespawn(UserIndex)

                    Case eGMCommands.ImperialArmour '/AI1 - 4
                        Call HandleImperialArmour(UserIndex)

                    Case eGMCommands.ChaosArmour '/AC1 - 4
                        Call HandleChaosArmour(UserIndex)

                    Case eGMCommands.NavigateToggle '/NAVE
                        Call HandleNavigateToggle(UserIndex)

                    Case eGMCommands.ServerOpenToUsersToggle '/HABILITAR
                        Call HandleServerOpenToUsersToggle(UserIndex)

                    Case eGMCommands.TurnOffServer '/APAGAR
                        Call HandleTurnOffServer(UserIndex)

                    Case eGMCommands.TurnCriminal '/CONDEN
                        Call HandleTurnCriminal(UserIndex)

                    Case eGMCommands.ResetFactions '/RAJAR
                        Call HandleResetFactions(UserIndex)

                    Case eGMCommands.RemoveCharFromGuild '/RAJARCLAN
                        Call HandleRemoveCharFromGuild(UserIndex)

                    Case eGMCommands.RequestCharMail '/LASTEMAIL
                        Call HandleRequestCharMail(UserIndex)

                    Case eGMCommands.AlterPassword '/APASS
                        Call HandleAlterPassword(UserIndex)

                    Case eGMCommands.AlterMail '/AEMAIL
                        Call HandleAlterMail(UserIndex)

                    Case eGMCommands.AlterName '/ANAME
                        Call HandleAlterName(UserIndex)

                    Case eGMCommands.ToggleCentinelActivated '/CENTINELAACTIVADO
                        Call HandleToggleCentinelActivated(UserIndex)

                    Case eGMCommands.DoBackUp '/DOBACKUP
                        Call HandleDoBackUp(UserIndex)

                    Case eGMCommands.ShowGuildMessages '/SHOWCMSG
                        Call HandleShowGuildMessages(UserIndex)

                    Case eGMCommands.SaveMap '/GUARDAMAPA
                        Call HandleSaveMap(UserIndex)

                    Case eGMCommands.ChangeMapInfoPK '/MODMAPINFO PK
                        Call HandleChangeMapInfoPK(UserIndex)

                    Case eGMCommands.ChangeMapInfoBackup '/MODMAPINFO BACKUP
                        Call HandleChangeMapInfoBackup(UserIndex)

                    Case eGMCommands.ChangeMapInfoRestricted '/MODMAPINFO RESTRINGIR
                        Call HandleChangeMapInfoRestricted(UserIndex)

                    Case eGMCommands.ChangeMapInfoNoMagic '/MODMAPINFO MAGIASINEFECTO
                        Call HandleChangeMapInfoNoMagic(UserIndex)

                    Case eGMCommands.ChangeMapInfoNoInvi '/MODMAPINFO INVISINEFECTO
                        Call HandleChangeMapInfoNoInvi(UserIndex)

                    Case eGMCommands.ChangeMapInfoNoResu '/MODMAPINFO RESUSINEFECTO
                        Call HandleChangeMapInfoNoResu(UserIndex)

                    Case eGMCommands.ChangeMapInfoLand '/MODMAPINFO TERRENO
                        Call HandleChangeMapInfoLand(UserIndex)

                    Case eGMCommands.ChangeMapInfoZone '/MODMAPINFO ZONA
                        Call HandleChangeMapInfoZone(UserIndex)

                    Case eGMCommands.SaveChars '/GRABAR
                        Call HandleSaveChars(UserIndex)

                    Case eGMCommands.CleanSOS '/BORRAR SOS
                        Call HandleCleanSOS(UserIndex)

                    Case eGMCommands.ShowServerForm '/SHOW INT
                        Call HandleShowServerForm(UserIndex)

                    Case eGMCommands.night '/NOCHE
                        Call HandleNight(UserIndex)

                    Case eGMCommands.KickAllChars '/ECHARTODOSPJS
                        Call HandleKickAllChars(UserIndex)

                    Case eGMCommands.ReloadNPCs '/RELOADNPCS
                        Call HandleReloadNPCs(UserIndex)

                    Case eGMCommands.ReloadServerIni '/RELOADSINI
                        Call HandleReloadServerIni(UserIndex)

                    Case eGMCommands.ReloadSpells '/RELOADHECHIZOS
                        Call HandleReloadSpells(UserIndex)

                    Case eGMCommands.ReloadObjects '/RELOADOBJ
                        Call HandleReloadObjects(UserIndex)

                    Case eGMCommands.Restart '/REINICIAR
                        Call HandleRestart(UserIndex)

                    Case eGMCommands.ResetAutoUpdate '/AUTOUPDATE
                        Call HandleResetAutoUpdate(UserIndex)

                    Case eGMCommands.ChatColor '/CHATCOLOR
                        Call HandleChatColor(UserIndex)

                    Case eGMCommands.Ignored '/IGNORADO
                        Call HandleIgnored(UserIndex)

                    Case eGMCommands.CheckSlot '/SLOT
                        Call HandleCheckSlot(UserIndex)

                    Case eGMCommands.SetIniVar '/SETINIVAR LLAVE CLAVE VALOR
                        Call HandleSetIniVar(UserIndex)
                End Select
            End With

        Catch ex As Exception
            Console.WriteLine("Error in HandleGMCommands: " & ex.Message)
            Call _
                LogError(
                    "Error en GmCommands. Error: " & Err.Number & " - " & Err.Description & ". Paquete: " &
                    Command_Renamed)
        End Try
    End Sub

    ''
    ' Handles the "Home" message.
    '
    ' @param    userIndex The index of the user sending the message.
    Private Sub HandleHome(UserIndex As Short)
        '***************************************************
        'Author: Budi
        'Creation Date: 06/01/2010
        'Last Modification: 05/06/10
        'Pato - 05/06/10: Add the Ucase$ to prevent problems.
        '***************************************************
        With UserList(UserIndex)
            Call .incomingData.ReadByte()
            If .flags.TargetNpcTipo = eNPCType.Gobernador Then
                Call setHome(UserIndex, Npclist(.flags.TargetNPC).Ciudad, .flags.TargetNPC)
            Else
                If .flags.Muerto = 1 Then
                    'Si es un mapa común y no está en cana
                    If UCase(MapInfo_Renamed(.Pos.Map).Restringir) = "NO" And .Counters.Pena = 0 Then
                        If .flags.Traveling = 0 Then
                            If Ciudades(.Hogar).Map <> .Pos.Map Then
                                Call goHome(UserIndex)
                            Else
                                Call _
                                    WriteConsoleMsg(UserIndex, "Ya te encuentras en tu hogar.",
                                                    FontTypeNames.FONTTYPE_INFO)
                            End If
                        Else
                            Call WriteMultiMessage(UserIndex, eMessages.CancelHome)
                            .flags.Traveling = 0
                            .Counters.goHome = 0
                        End If
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, "No puedes usar este comando aquí.", FontTypeNames.FONTTYPE_FIGHT)
                    End If
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "Debes estar muerto para utilizar este comando.",
                                        FontTypeNames.FONTTYPE_INFO)
                End If
            End If
        End With
    End Sub

    ''
    ' Handles the "LoginExistingChar" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleLoginExistingChar(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 6 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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
                Call _
                    WriteErrorMsg(UserIndex,
                                  "Se te ha prohibido la entrada a Argentum Online debido a tu mal comportamiento. Puedes consultar el reglamento y el sistema de soporte desde www.argentumonline.com.ar")
            ElseIf Not VersionOK(version) Then
                Call _
                    WriteErrorMsg(UserIndex,
                                  "Esta versión del juego es obsoleta, la versión correcta es la " & ULTIMAVERSION &
                                  ". La misma se encuentra disponible en www.argentumonline.com.ar")
            Else
                Call ConnectUser(UserIndex, UserName, Password)
            End If

            'If we got here then packet is complete, copy data back to original queue
            Call UserList(UserIndex).incomingData.CopyBuffer(buffer)


        Catch ex As Exception
            Console.WriteLine("Error in HandleLoginExistingChar: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "ThrowDices" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleThrowDices(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        'Remove packet ID
        Call UserList(UserIndex).incomingData.ReadByte()

        With UserList(UserIndex).Stats
            .UserAtributos(eAtributos.Fuerza) = MaximoInt(15, 13 + RandomNumber(0, 3) + RandomNumber(0, 2))
            .UserAtributos(eAtributos.Agilidad) = MaximoInt(15,
                                                            12 + RandomNumber(0, 3) + RandomNumber(0, 3))
            .UserAtributos(eAtributos.Inteligencia) = MaximoInt(16,
                                                                13 + RandomNumber(0, 3) +
                                                                RandomNumber(0, 2))
            .UserAtributos(eAtributos.Carisma) = MaximoInt(15,
                                                           12 + RandomNumber(0, 3) + RandomNumber(0, 3))
            .UserAtributos(eAtributos.Constitucion) = 16 + RandomNumber(0, 1) + RandomNumber(0, 1)
        End With

        Call WriteDiceRoll(UserIndex)
    End Sub

    ''
    ' Handles the "LoginNewChar" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleLoginNewChar(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 15 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
            Dim buffer As New clsByteQueue
            Call buffer.CopyBuffer(UserList(UserIndex).incomingData)

            'Remove packet ID
            Call buffer.ReadByte()

            Dim UserName As String
            Dim Password As String
            Dim version As String
            Dim race As eRaza
            Dim gender As eGenero
            Dim homeland As eCiudad
            'UPGRADE_NOTE: Class se actualizó a Class_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            Dim Class_Renamed As eClass
            Dim Head As Short
            Dim mail As String

            If PuedeCrearPersonajes = 0 Then
                Call WriteErrorMsg(UserIndex, "La creación de personajes en este servidor se ha deshabilitado.")
                Call FlushBuffer(UserIndex)
                Call CloseSocket(UserIndex)

                Exit Sub
            End If

            If ServerSoloGMs <> 0 Then
                Call _
                    WriteErrorMsg(UserIndex,
                                  "Servidor restringido a administradores. Consulte la página oficial o el foro oficial para más información.")
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
                Call _
                    WriteErrorMsg(UserIndex,
                                  "Esta versión del juego es obsoleta, la versión correcta es la " & ULTIMAVERSION &
                                  ". La misma se encuentra disponible en www.argentumonline.com.ar")
            Else
                Call ConnectNewUser(UserIndex, UserName, Password, race, gender, Class_Renamed, mail, homeland, Head)
            End If

            'If we got here then packet is complete, copy data back to original queue
            Call UserList(UserIndex).incomingData.CopyBuffer(buffer)


        Catch ex As Exception
            Console.WriteLine("Error in HandleLoginNewChar: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "Talk" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleTalk(UserIndex As Short)
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

        Try
            Dim buffer As New clsByteQueue
            Dim Chat As String
            With UserList(UserIndex)

                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                Chat = buffer.ReadASCIIString()

                '[Consejeros & GMs]
                If .flags.Privilegios And (PlayerType.Consejero Or PlayerType.SemiDios) Then
                    Call LogGM(.name, "Dijo: " & Chat)
                End If

                'I see you....
                If .flags.Oculto > 0 Then
                    .flags.Oculto = 0
                    .Counters.TiempoOculto = 0

                    If .flags.Navegando = 1 Then
                        If .clase = eClass.Pirat Then
                            ' Pierde la apariencia de fragata fantasmal
                            Call ToogleBoatBody(UserIndex)
                            Call _
                                WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!",
                                                FontTypeNames.FONTTYPE_INFO)
                            Call _
                                ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading,
                                               NingunArma, NingunEscudo, NingunCasco)
                        End If
                    Else
                        If .flags.invisible = 0 Then
                            Call SetInvisible(UserIndex, UserList(UserIndex).Char_Renamed.CharIndex, False)
                            Call WriteConsoleMsg(UserIndex, "¡Has vuelto a ser visible!", FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If
                End If

                If migr_LenB(Chat) <> 0 Then
                    'Analize chat...
                    Call ParseChat(Chat)

                    If Not (.flags.AdminInvisible = 1) Then
                        If .flags.Muerto = 1 Then
                            Call _
                                SendData(SendTarget.ToDeadArea, UserIndex,
                                         PrepareMessageChatOverHead(Chat, .Char_Renamed.CharIndex, CHAT_COLOR_DEAD_CHAR))
                        Else
                            Call _
                                SendData(SendTarget.ToPCArea, UserIndex,
                                         PrepareMessageChatOverHead(Chat, .Char_Renamed.CharIndex, .flags.ChatColor))
                        End If
                    Else
                        If RTrim(Chat) <> "" Then
                            Call _
                                SendData(SendTarget.ToPCArea, UserIndex,
                                         PrepareMessageConsoleMsg("Gm> " & Chat, FontTypeNames.FONTTYPE_GM))
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleTalk: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "Yell" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleYell(UserIndex As Short)
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

        Try
            Dim buffer As New clsByteQueue
            Dim Chat As String
            With UserList(UserIndex)

                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                Chat = buffer.ReadASCIIString()


                '[Consejeros & GMs]
                If .flags.Privilegios And (PlayerType.Consejero Or PlayerType.SemiDios) Then
                    Call LogGM(.name, "Grito: " & Chat)
                End If

                'I see you....
                If .flags.Oculto > 0 Then
                    .flags.Oculto = 0
                    .Counters.TiempoOculto = 0

                    If .flags.Navegando = 1 Then
                        If .clase = eClass.Pirat Then
                            ' Pierde la apariencia de fragata fantasmal
                            Call ToogleBoatBody(UserIndex)
                            Call _
                                WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!",
                                                FontTypeNames.FONTTYPE_INFO)
                            Call _
                                ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading,
                                               NingunArma, NingunEscudo, NingunCasco)
                        End If
                    Else
                        If .flags.invisible = 0 Then
                            Call SetInvisible(UserIndex, .Char_Renamed.CharIndex, False)
                            Call WriteConsoleMsg(UserIndex, "¡Has vuelto a ser visible!", FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If
                End If

                If migr_LenB(Chat) <> 0 Then
                    'Analize chat...
                    Call ParseChat(Chat)

                    If .flags.Privilegios And PlayerType.User Then
                        If UserList(UserIndex).flags.Muerto = 1 Then
                            Call _
                                SendData(SendTarget.ToDeadArea, UserIndex,
                                         PrepareMessageChatOverHead(Chat, .Char_Renamed.CharIndex, CHAT_COLOR_DEAD_CHAR))
                        Else
                            Call _
                                SendData(SendTarget.ToPCArea, UserIndex,
                                         PrepareMessageChatOverHead(Chat, .Char_Renamed.CharIndex,
                                                                    ColorTranslator.ToOle(
                                                                        Color.Red)))
                        End If
                    Else
                        If Not (.flags.AdminInvisible = 1) Then
                            Call _
                                SendData(SendTarget.ToPCArea, UserIndex,
                                         PrepareMessageChatOverHead(Chat, .Char_Renamed.CharIndex, CHAT_COLOR_GM_YELL))
                        Else
                            Call _
                                SendData(SendTarget.ToPCArea, UserIndex,
                                         PrepareMessageConsoleMsg("Gm> " & Chat, FontTypeNames.FONTTYPE_GM))
                        End If
                    End If
                End If


                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleYell: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "Whisper" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleWhisper(UserIndex As Short)
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

        Try
            Dim buffer As New clsByteQueue
            Dim Chat As String
            Dim targetCharIndex As Short
            Dim TargetUserIndex As Short
            Dim targetPriv As PlayerType
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                targetCharIndex = buffer.ReadInteger()
                Chat = buffer.ReadASCIIString()

                TargetUserIndex = CharIndexToUserIndex(targetCharIndex)

                If .flags.Muerto Then
                    Call _
                        WriteConsoleMsg(UserIndex,
                                        "¡¡Estás muerto!! Los muertos no pueden comunicarse con el mundo de los vivos. ",
                                        FontTypeNames.FONTTYPE_INFO)
                Else
                    If TargetUserIndex = INVALID_INDEX Then
                        Call WriteConsoleMsg(UserIndex, "Usuario inexistente.", FontTypeNames.FONTTYPE_INFO)
                    Else
                        targetPriv = UserList(TargetUserIndex).flags.Privilegios
                        'A los dioses y admins no vale susurrarles si no sos uno vos mismo (así no pueden ver si están conectados o no)
                        If _
                            (targetPriv And (PlayerType.Dios Or PlayerType.Admin)) <> 0 And
                            (.flags.Privilegios And
                             (PlayerType.User Or PlayerType.Consejero Or
                              PlayerType.SemiDios)) <> 0 Then
                            ' Controlamos que no este invisible
                            If UserList(TargetUserIndex).flags.AdminInvisible <> 1 Then
                                Call _
                                    WriteConsoleMsg(UserIndex, "No puedes susurrarle a los Dioses y Admins.",
                                                    FontTypeNames.FONTTYPE_INFO)
                            End If
                            'A los Consejeros y SemiDioses no vale susurrarles si sos un PJ común.
                        ElseIf _
                            (.flags.Privilegios And PlayerType.User) <> 0 And
                            (Not targetPriv And PlayerType.User) <> 0 Then
                            ' Controlamos que no este invisible
                            If UserList(TargetUserIndex).flags.AdminInvisible <> 1 Then
                                Call _
                                    WriteConsoleMsg(UserIndex, "No puedes susurrarle a los GMs.",
                                                    FontTypeNames.FONTTYPE_INFO)
                            End If
                        ElseIf Not EstaPCarea(UserIndex, TargetUserIndex) Then
                            Call WriteConsoleMsg(UserIndex, "Estás muy lejos del usuario.", FontTypeNames.FONTTYPE_INFO)

                        Else
                            '[Consejeros & GMs]
                            If _
                                .flags.Privilegios And
                                (PlayerType.Consejero Or PlayerType.SemiDios) Then
                                Call LogGM(.name, "Le dijo a '" & UserList(TargetUserIndex).name & "' " & Chat)
                            End If

                            If migr_LenB(Chat) <> 0 Then
                                'Analize chat...
                                Call ParseChat(Chat)

                                If Not (.flags.AdminInvisible = 1) Then
                                    Call _
                                        WriteChatOverHead(UserIndex, Chat, .Char_Renamed.CharIndex,
                                                          ColorTranslator.ToOle(Color.Blue))
                                    Call _
                                        WriteChatOverHead(TargetUserIndex, Chat, .Char_Renamed.CharIndex,
                                                          ColorTranslator.ToOle(Color.Blue))
                                    Call FlushBuffer(TargetUserIndex)

                                    '[CDT 17-02-2004]
                                    If _
                                        .flags.Privilegios And
                                        (PlayerType.User Or PlayerType.Consejero) Then
                                        Call _
                                            SendData(SendTarget.ToAdminsAreaButConsejeros, UserIndex,
                                                     PrepareMessageChatOverHead(
                                                         "A " & UserList(TargetUserIndex).name & "> " & Chat,
                                                         .Char_Renamed.CharIndex,
                                                         ColorTranslator.ToOle(Color.Yellow)))
                                    End If
                                Else
                                    Call WriteConsoleMsg(UserIndex, "Susurraste> " & Chat, FontTypeNames.FONTTYPE_GM)
                                    If UserIndex <> TargetUserIndex Then _
                                        Call _
                                            WriteConsoleMsg(TargetUserIndex, "Gm susurra> " & Chat,
                                                            FontTypeNames.FONTTYPE_GM)

                                    If _
                                        .flags.Privilegios And
                                        (PlayerType.User Or PlayerType.Consejero) Then
                                        Call _
                                            SendData(SendTarget.ToAdminsAreaButConsejeros, UserIndex,
                                                     PrepareMessageConsoleMsg(
                                                         "Gm dijo a " & UserList(TargetUserIndex).name & "> " & Chat,
                                                         FontTypeNames.FONTTYPE_GM))
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleWhisper: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "Walk" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleWalk(UserIndex As Short)
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
        Dim heading As eHeading

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
                        If dummy <> 0 Then dummy = 126000\dummy

                        Call LogHackAttemp("Tramposo SH: " & .name & " , " & dummy)
                        Call _
                            SendData(SendTarget.ToAdmins, 0,
                                     PrepareMessageConsoleMsg(
                                         "Servidor> " & .name & " ha sido echado por el servidor por posible uso de SH.",
                                         FontTypeNames.FONTTYPE_SERVER))
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

                    Call _
                        SendData(SendTarget.ToPCArea, UserIndex,
                                 PrepareMessageCreateFX(.Char_Renamed.CharIndex, 0, 0))
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

                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes moverte porque estás paralizado.",
                                        FontTypeNames.FONTTYPE_INFO)
                End If

                .flags.CountSH = 0
            End If

            'Can't move while hidden except he is a thief
            If .flags.Oculto = 1 And .flags.AdminInvisible = 0 Then
                If .clase <> eClass.Thief And .clase <> eClass.Bandit Then
                    .flags.Oculto = 0
                    .Counters.TiempoOculto = 0

                    If .flags.Navegando = 1 Then
                        If .clase = eClass.Pirat Then
                            ' Pierde la apariencia de fragata fantasmal
                            Call ToogleBoatBody(UserIndex)
                            Call _
                                WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!",
                                                FontTypeNames.FONTTYPE_INFO)
                            Call _
                                ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading,
                                               NingunArma, NingunEscudo, NingunCasco)
                        End If
                    Else
                        'If not under a spell effect, show char
                        If .flags.invisible = 0 Then
                            Call WriteConsoleMsg(UserIndex, "Has vuelto a ser visible.", FontTypeNames.FONTTYPE_INFO)
                            Call SetInvisible(UserIndex, .Char_Renamed.CharIndex, False)
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

    Private Sub HandleRequestPositionUpdate(UserIndex As Short)
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

    Private Sub HandleAttack(UserIndex As Short)
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
                    If .clase = eClass.Pirat Then
                        ' Pierde la apariencia de fragata fantasmal
                        Call ToogleBoatBody(UserIndex)
                        Call _
                            WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!",
                                            FontTypeNames.FONTTYPE_INFO)
                        Call _
                            ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading,
                                           NingunArma, NingunEscudo, NingunCasco)
                    End If
                Else
                    If .flags.invisible = 0 Then
                        Call SetInvisible(UserIndex, .Char_Renamed.CharIndex, False)
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

    Private Sub HandlePickUp(UserIndex As Short)
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
            If .flags.Privilegios And PlayerType.Consejero Then
                If Not .flags.Privilegios And PlayerType.RoleMaster Then
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

    Private Sub HandleSafeToggle(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If .flags.Seguro Then
                Call WriteMultiMessage(UserIndex, eMessages.SafeModeOff) 'Call WriteSafeModeOff(UserIndex)
            Else
                Call WriteMultiMessage(UserIndex, eMessages.SafeModeOn) 'Call WriteSafeModeOn(UserIndex)
            End If

            .flags.Seguro = Not .flags.Seguro
        End With
    End Sub

    ''
    ' Handles the "ResuscitationSafeToggle" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleResuscitationToggle(UserIndex As Short)
        '***************************************************
        'Author: Rapsodius
        'Creation Date: 10/10/07
        '***************************************************
        With UserList(UserIndex)
            Call .incomingData.ReadByte()

            .flags.SeguroResu = Not .flags.SeguroResu

            If .flags.SeguroResu Then
                Call WriteMultiMessage(UserIndex, eMessages.ResuscitationSafeOn) _
                'Call WriteResuscitationSafeOn(UserIndex)
            Else
                Call WriteMultiMessage(UserIndex, eMessages.ResuscitationSafeOff) _
                'Call WriteResuscitationSafeOff(UserIndex)
            End If
        End With
    End Sub

    ''
    ' Handles the "RequestGuildLeaderInfo" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleRequestGuildLeaderInfo(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        'Remove packet ID
        UserList(UserIndex).incomingData.ReadByte()

        Call SendGuildLeaderInfo(UserIndex)
    End Sub

    ''
    ' Handles the "RequestAtributes" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleRequestAtributes(UserIndex As Short)
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

    Private Sub HandleRequestFame(UserIndex As Short)
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

    Private Sub HandleRequestSkills(UserIndex As Short)
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

    Private Sub HandleRequestMiniStats(UserIndex As Short)
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

    Private Sub HandleCommerceEnd(UserIndex As Short)
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

    Private Sub HandleUserCommerceEnd(UserIndex As Short)
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
                    Call _
                        WriteConsoleMsg(.ComUsu.DestUsu, .name & " ha dejado de comerciar con vos.",
                                        FontTypeNames.FONTTYPE_TALK)
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
    Private Sub HandleUserCommerceConfirm(UserIndex As Short)
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

    Private Sub HandleCommerceChat(UserIndex As Short)
        '***************************************************
        'Author: ZaMa
        'Last Modification: 03/12/2009
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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
                        Call ParseChat(Chat)

                        Chat = UserList(UserIndex).name & "> " & Chat
                        Call WriteCommerceChat(UserIndex, Chat, FontTypeNames.FONTTYPE_PARTY)
                        Call WriteCommerceChat(UserList(UserIndex).ComUsu.DestUsu, Chat, FontTypeNames.FONTTYPE_PARTY)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleCommerceChat: " & ex.Message)
        End Try
    End Sub


    ''
    ' Handles the "BankEnd" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleBankEnd(UserIndex As Short)
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

    Private Sub HandleUserCommerceOk(UserIndex As Short)
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

    Private Sub HandleUserCommerceReject(UserIndex As Short)
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

    Private Sub HandleDrop(UserIndex As Short)
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
            If _
                .flags.Navegando = 1 Or .flags.Muerto = 1 Or
                ((.flags.Privilegios And PlayerType.Consejero) <> 0 And
                 (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0) Then Exit Sub

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

    Private Sub HandleCastSpell(UserIndex As Short)
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

    Private Sub HandleLeftClick(UserIndex As Short)
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

    Private Sub HandleDoubleClick(UserIndex As Short)
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

    Private Sub HandleWork(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 13/01/2010 (ZaMa)
        '13/01/2010: ZaMa - El pirata se puede ocultar en barca
        '***************************************************
        If UserList(UserIndex).incomingData.length < 2 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Dim Skill As eSkill
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()


            Skill = .incomingData.ReadByte()

            If UserList(UserIndex).flags.Muerto = 1 Then Exit Sub

            'If exiting, cancel
            Call CancelExit(UserIndex)

            Select Case Skill
                Case eSkill.Robar, eSkill.Magia, eSkill.Domar
                    Call WriteMultiMessage(UserIndex, eMessages.WorkRequestTarget, Skill) _
                    'Call WriteWorkRequestTarget(UserIndex, Skill)
                Case eSkill.Ocultarse

                    If .flags.EnConsulta Then
                        Call _
                            WriteConsoleMsg(UserIndex, "No puedes ocultarte si estás en consulta.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If

                    If .flags.Navegando = 1 Then
                        If .clase <> eClass.Pirat Then
                            '[CDT 17-02-2004]
                            If Not .flags.UltimoMensaje = 3 Then
                                Call _
                                    WriteConsoleMsg(UserIndex, "No puedes ocultarte si estás navegando.",
                                                    FontTypeNames.FONTTYPE_INFO)
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

    Private Sub HandleInitCrafting(UserIndex As Short)
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

    Private Sub HandleUseSpellMacro(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            Call _
                SendData(SendTarget.ToAdmins, UserIndex,
                         PrepareMessageConsoleMsg(.name & " fue expulsado por Anti-macro de hechizos.",
                                                  FontTypeNames.FONTTYPE_VENENO))
            Call _
                WriteErrorMsg(UserIndex,
                              "Has sido expulsado por usar macro de hechizos. Recomendamos leer el reglamento sobre el tema macros.")
            Call FlushBuffer(UserIndex)
            Call CloseSocket(UserIndex)
        End With
    End Sub

    ''
    ' Handles the "UseItem" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleUseItem(UserIndex As Short)
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

    Private Sub HandleCraftBlacksmith(UserIndex As Short)
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

    Private Sub HandleCraftCarpenter(UserIndex As Short)
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

    Private Sub HandleWorkLeftClick(UserIndex As Short)
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
        Dim Skill As eSkill
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


            If .flags.Muerto = 1 Or .flags.Descansar Or .flags.Meditando Or Not InMapBounds(.Pos.Map, X, Y) Then _
                Exit Sub

            If Not InRangoVision(UserIndex, X, Y) Then
                Call WritePosUpdate(UserIndex)
                Exit Sub
            End If

            'If exiting, cancel
            Call CancelExit(UserIndex)

            Select Case Skill
                Case eSkill.Proyectiles

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
                            ElseIf ObjData_Renamed(.MunicionEqpObjIndex).OBJType <> eOBJType.otFlechas _
                                Then
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
                        If .Genero = eGenero.Hombre Then
                            Call _
                                WriteConsoleMsg(UserIndex, "Estás muy cansado para luchar.", FontTypeNames.FONTTYPE_INFO)
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex, "Estás muy cansada para luchar.", FontTypeNames.FONTTYPE_INFO)
                        End If
                        Exit Sub
                    End If

                    Call LookatTile(UserIndex, .Pos.Map, X, Y)

                    tU = .flags.TargetUser
                    tN = .flags.TargetNPC

                    'Validate target
                    If tU > 0 Then
                        'Only allow to atack if the other one can retaliate (can see us)
                        If Math.Abs(UserList(tU).Pos.Y - .Pos.Y) > RANGO_VISION_Y Then
                            Call _
                                WriteConsoleMsg(UserIndex, "Estás demasiado lejos para atacar.",
                                                FontTypeNames.FONTTYPE_WARNING)
                            Exit Sub
                        End If

                        'Prevent from hitting self
                        If tU = UserIndex Then
                            Call _
                                WriteConsoleMsg(UserIndex, "¡No puedes atacarte a vos mismo!",
                                                FontTypeNames.FONTTYPE_INFO)
                            Exit Sub
                        End If

                        'Attack!
                        Atacked = UsuarioAtacaUsuario(UserIndex, tU)

                    ElseIf tN > 0 Then
                        'Only allow to atack if the other one can retaliate (can see us)
                        If _
                            Math.Abs(Npclist(tN).Pos.Y - .Pos.Y) > RANGO_VISION_Y And
                            Math.Abs(Npclist(tN).Pos.X - .Pos.X) > RANGO_VISION_X Then
                            Call _
                                WriteConsoleMsg(UserIndex, "Estás demasiado lejos para atacar.",
                                                FontTypeNames.FONTTYPE_WARNING)
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

                Case eSkill.Magia
                    'Check the map allows spells to be casted.
                    If MapInfo_Renamed(.Pos.Map).MagiaSinEfecto > 0 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "Una fuerza oscura te impide canalizar tu energía.",
                                            FontTypeNames.FONTTYPE_FIGHT)
                        Exit Sub
                    End If

                    'Target whatever is in that tile
                    Call LookatTile(UserIndex, .Pos.Map, X, Y)

                    'If it's outside range log it and exit
                    If Math.Abs(.Pos.X - X) > RANGO_VISION_X Or Math.Abs(.Pos.Y - Y) > RANGO_VISION_Y Then
                        Call _
                            LogCheating(
                                "Ataque fuera de rango de " & .name & "(" & .Pos.Map & "/" & .Pos.X & "/" & .Pos.Y &
                                ") ip: " & .ip & " a la posición (" & .Pos.Map & "/" & X & "/" & Y & ")")
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
                        Call _
                            WriteConsoleMsg(UserIndex, "¡Primero selecciona el hechizo que quieres lanzar!",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If

                Case eSkill.Pesca
                    DummyInt = .Invent.WeaponEqpObjIndex
                    If DummyInt = 0 Then Exit Sub

                    'Check interval
                    If Not IntervaloPermiteTrabajar(UserIndex) Then Exit Sub

                    'Basado en la idea de Barrin
                    'Comentario por Barrin: jah, "basado", caradura ! ^^
                    If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = 1 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "No puedes pescar desde donde te encuentras.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If

                    If HayAgua(.Pos.Map, X, Y) Then
                        Select Case DummyInt
                            Case CAÑA_PESCA
                                Call DoPescar(UserIndex)

                            Case RED_PESCA
                                If Math.Abs(.Pos.X - X) + Math.Abs(.Pos.Y - Y) > 2 Then
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Estás demasiado lejos para pescar.",
                                                        FontTypeNames.FONTTYPE_INFO)
                                    Exit Sub
                                End If

                                Call DoPescarRed(UserIndex)

                            Case Else
                                Exit Sub 'Invalid item!
                        End Select

                        'Play sound!
                        Call _
                            SendData(SendTarget.ToPCArea, UserIndex,
                                     PrepareMessagePlayWave(SND_PESCAR, .Pos.X, .Pos.Y))
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, "No hay agua donde pescar. Busca un lago, río o mar.",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If

                Case eSkill.Robar
                    'Does the map allow us to steal here?
                    If MapInfo_Renamed(.Pos.Map).Pk Then

                        'Check interval
                        If Not IntervaloPermiteTrabajar(UserIndex) Then Exit Sub

                        'Target whatever is in that tile
                        Call LookatTile(UserIndex, UserList(UserIndex).Pos.Map, X, Y)

                        tU = .flags.TargetUser

                        If tU > 0 And tU <> UserIndex Then
                            'Can't steal administrative players
                            If UserList(tU).flags.Privilegios And PlayerType.User Then
                                If UserList(tU).flags.Muerto = 0 Then
                                    If Math.Abs(.Pos.X - X) + Math.Abs(.Pos.Y - Y) > 1 Then
                                        Call _
                                            WriteConsoleMsg(UserIndex, "Estás demasiado lejos.",
                                                            FontTypeNames.FONTTYPE_INFO)
                                        Exit Sub
                                    End If

                                    '17/09/02
                                    'Check the trigger
                                    If MapData(UserList(tU).Pos.Map, X, Y).trigger = eTrigger.ZONASEGURA _
                                        Then
                                        Call _
                                            WriteConsoleMsg(UserIndex, "No puedes robar aquí.",
                                                            FontTypeNames.FONTTYPE_WARNING)
                                        Exit Sub
                                    End If

                                    If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = eTrigger.ZONASEGURA _
                                        Then
                                        Call _
                                            WriteConsoleMsg(UserIndex, "No puedes robar aquí.",
                                                            FontTypeNames.FONTTYPE_WARNING)
                                        Exit Sub
                                    End If

                                    Call DoRobar(UserIndex, tU)
                                End If
                            End If
                        Else
                            Call WriteConsoleMsg(UserIndex, "¡No hay a quien robarle!", FontTypeNames.FONTTYPE_INFO)
                        End If
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, "¡No puedes robar en zonas seguras!", FontTypeNames.FONTTYPE_INFO)
                    End If

                Case eSkill.Talar
                    'Check interval
                    If Not IntervaloPermiteTrabajar(UserIndex) Then Exit Sub

                    If .Invent.WeaponEqpObjIndex = 0 Then
                        Call WriteConsoleMsg(UserIndex, "Deberías equiparte el hacha.", FontTypeNames.FONTTYPE_INFO)
                        Exit Sub
                    End If

                    If .Invent.WeaponEqpObjIndex <> HACHA_LEÑADOR And .Invent.WeaponEqpObjIndex <> HACHA_LEÑA_ELFICA _
                        Then
                        ' Podemos llegar acá si el user equipó el anillo dsp de la U y antes del click
                        Exit Sub
                    End If

                    DummyInt = MapData(.Pos.Map, X, Y).ObjInfo.ObjIndex

                    If DummyInt > 0 Then
                        If Math.Abs(.Pos.X - X) + Math.Abs(.Pos.Y - Y) > 2 Then
                            Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
                            Exit Sub
                        End If

                        'Barrin 29/9/03
                        If .Pos.X = X And .Pos.Y = Y Then
                            Call WriteConsoleMsg(UserIndex, "No puedes talar desde allí.", FontTypeNames.FONTTYPE_INFO)
                            Exit Sub
                        End If

                        '¿Hay un arbol donde clickeo?
                        If _
                            ObjData_Renamed(DummyInt).OBJType = eOBJType.otArboles And
                            .Invent.WeaponEqpObjIndex = HACHA_LEÑADOR Then
                            Call _
                                SendData(SendTarget.ToPCArea, UserIndex,
                                         PrepareMessagePlayWave(SND_TALAR, .Pos.X, .Pos.Y))
                            Call DoTalar(UserIndex)
                        ElseIf _
                            ObjData_Renamed(DummyInt).OBJType = eOBJType.otArbolElfico And
                            .Invent.WeaponEqpObjIndex = HACHA_LEÑA_ELFICA Then
                            If .Invent.WeaponEqpObjIndex = HACHA_LEÑA_ELFICA Then
                                Call _
                                    SendData(SendTarget.ToPCArea, UserIndex,
                                             PrepareMessagePlayWave(SND_TALAR, .Pos.X, .Pos.Y))
                                Call DoTalar(UserIndex, True)
                            Else
                                Call _
                                    WriteConsoleMsg(UserIndex, "El hacha utilizado no es suficientemente poderosa.",
                                                    FontTypeNames.FONTTYPE_INFO)
                            End If
                        End If
                    Else
                        Call WriteConsoleMsg(UserIndex, "No hay ningún árbol ahí.", FontTypeNames.FONTTYPE_INFO)
                    End If

                Case eSkill.Mineria
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
                        If Math.Abs(.Pos.X - X) + Math.Abs(.Pos.Y - Y) > 2 Then
                            Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
                            Exit Sub
                        End If

                        DummyInt = MapData(.Pos.Map, X, Y).ObjInfo.ObjIndex 'CHECK
                        '¿Hay un yacimiento donde clickeo?
                        If ObjData_Renamed(DummyInt).OBJType = eOBJType.otYacimiento Then
                            Call DoMineria(UserIndex)
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex, "Ahí no hay ningún yacimiento.", FontTypeNames.FONTTYPE_INFO)
                        End If
                    Else
                        Call WriteConsoleMsg(UserIndex, "Ahí no hay ningún yacimiento.", FontTypeNames.FONTTYPE_INFO)
                    End If

                Case eSkill.Domar
                    'Modificado 25/11/02
                    'Optimizado y solucionado el bug de la doma de
                    'criaturas hostiles.

                    'Target whatever is that tile
                    Call LookatTile(UserIndex, .Pos.Map, X, Y)
                    tN = .flags.TargetNPC

                    If tN > 0 Then
                        If Npclist(tN).flags.Domable > 0 Then
                            If Math.Abs(.Pos.X - X) + Math.Abs(.Pos.Y - Y) > 2 Then
                                Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
                                Exit Sub
                            End If

                            If migr_LenB(Npclist(tN).flags.AttackedBy) <> 0 Then
                                Call _
                                    WriteConsoleMsg(UserIndex,
                                                    "No puedes domar una criatura que está luchando con un jugador.",
                                                    FontTypeNames.FONTTYPE_INFO)
                                Exit Sub
                            End If

                            Call DoDomar(UserIndex, tN)
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex, "No puedes domar a esa criatura.",
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    Else
                        Call WriteConsoleMsg(UserIndex, "¡No hay ninguna criatura allí!", FontTypeNames.FONTTYPE_INFO)
                    End If

                Case FundirMetal 'UGLY!!! This is a constant, not a skill!!
                    'Check interval
                    If Not IntervaloPermiteTrabajar(UserIndex) Then Exit Sub

                    'Check there is a proper item there
                    If .flags.TargetObj > 0 Then
                        If ObjData_Renamed(.flags.TargetObj).OBJType = eOBJType.otFragua Then
                            'Validate other items
                            If .flags.TargetObjInvSlot < 1 Or .flags.TargetObjInvSlot > .CurrentInventorySlots Then
                                Exit Sub
                            End If

                            ''chequeamos que no se zarpe duplicando oro
                            If .Invent.Object_Renamed(.flags.TargetObjInvSlot).ObjIndex <> .flags.TargetObjInvIndex Then
                                If _
                                    .Invent.Object_Renamed(.flags.TargetObjInvSlot).ObjIndex = 0 Or
                                    .Invent.Object_Renamed(.flags.TargetObjInvSlot).Amount = 0 Then
                                    Call _
                                        WriteConsoleMsg(UserIndex, "No tienes más minerales.",
                                                        FontTypeNames.FONTTYPE_INFO)
                                    Exit Sub
                                End If

                                ''FUISTE
                                Call WriteErrorMsg(UserIndex, "Has sido expulsado por el sistema anti cheats.")
                                Call FlushBuffer(UserIndex)
                                Call CloseSocket(UserIndex)
                                Exit Sub
                            End If
                            If ObjData_Renamed(.flags.TargetObjInvIndex).OBJType = eOBJType.otMinerales _
                                Then
                                Call FundirMineral(UserIndex)
                            ElseIf ObjData_Renamed(.flags.TargetObjInvIndex).OBJType = eOBJType.otWeapon _
                                Then
                                Call FundirArmas(UserIndex)
                            End If
                        Else
                            Call WriteConsoleMsg(UserIndex, "Ahí no hay ninguna fragua.", FontTypeNames.FONTTYPE_INFO)
                        End If
                    Else
                        Call WriteConsoleMsg(UserIndex, "Ahí no hay ninguna fragua.", FontTypeNames.FONTTYPE_INFO)
                    End If

                Case eSkill.Herreria
                    'Target wehatever is in that tile
                    Call LookatTile(UserIndex, .Pos.Map, X, Y)

                    If .flags.TargetObj > 0 Then
                        If ObjData_Renamed(.flags.TargetObj).OBJType = eOBJType.otYunque Then
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

    Private Sub HandleCreateNewGuild(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/11/09
        '05/11/09: Pato - Ahora se quitan los espacios del principio y del fin del nombre del clan
        '***************************************************
        If UserList(UserIndex).incomingData.length < 9 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If CrearNuevoClan(UserIndex, desc, GuildName, site, codex, .FundandoGuildAlineacion, errorStr) _
                    Then
                    Call _
                        SendData(SendTarget.ToAll, UserIndex,
                                 PrepareMessageConsoleMsg(
                                     .name & " fundó el clan " & GuildName & " de alineación " &
                                     GuildAlignment(.GuildIndex) & ".", FontTypeNames.FONTTYPE_GUILD))
                    Call SendData(SendTarget.ToAll, 0, PrepareMessagePlayWave(44, NO_3D_SOUND, NO_3D_SOUND))


                    'Update tag
                    Call RefreshCharStatus(UserIndex)
                Else
                    Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleCreateNewGuild: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "SpellInfo" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleSpellInfo(UserIndex As Short)
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
                    Call _
                        WriteConsoleMsg(UserIndex,
                                        "%%%%%%%%%%%% INFO DEL HECHIZO %%%%%%%%%%%%" & vbCrLf & "Nombre:" & .Nombre &
                                        vbCrLf & "Descripción:" & .desc & vbCrLf & "Skill requerido: " & .MinSkill &
                                        " de magia." & vbCrLf & "Maná necesario: " & .ManaRequerido & vbCrLf &
                                        "Energía necesaria: " & .StaRequerido & vbCrLf &
                                        "%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%", FontTypeNames.FONTTYPE_INFO)
                End With
            End If
        End With
    End Sub

    ''
    ' Handles the "EquipItem" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleEquipItem(UserIndex As Short)
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

    Private Sub HandleChangeHeading(UserIndex As Short)
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

        Dim heading As eHeading
        Dim posX As Short
        Dim posY As Short
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()


            heading = .incomingData.ReadByte()

            If .flags.Paralizado = 1 And .flags.Inmovilizado = 0 Then
                Select Case heading
                    Case eHeading.NORTH
                        posY = - 1
                    Case eHeading.EAST
                        posX = 1
                    Case eHeading.SOUTH
                        posY = 1
                    Case eHeading.WEST
                        posX = - 1
                End Select

                If _
                    LegalPos(.Pos.Map, .Pos.X + posX, .Pos.Y + posY, CBool(.flags.Navegando),
                             Not CBool(.flags.Navegando)) Then
                    Exit Sub
                End If
            End If

            'Validate heading (VB won't say invalid cast if not a valid index like .Net languages would do... *sigh*)
            If heading > 0 And heading < 5 Then
                .Char_Renamed.heading = heading
                Call _
                    ChangeUserChar(UserIndex, .Char_Renamed.body, .Char_Renamed.Head, .Char_Renamed.heading,
                                   .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim, .Char_Renamed.CascoAnim)
            End If
        End With
    End Sub

    ''
    ' Handles the "ModifySkills" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleModifySkills(UserIndex As Short)
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

    Private Sub HandleTrain(UserIndex As Short)
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

            If Npclist(.flags.TargetNPC).NPCtype <> eNPCType.Entrenador Then Exit Sub

            If Npclist(.flags.TargetNPC).Mascotas < MAXMASCOTASENTRENADOR Then
                If PetIndex > 0 And PetIndex < Npclist(.flags.TargetNPC).NroCriaturas + 1 Then
                    'Create the creature
                    SpawnedNpc = SpawnNpc(Npclist(.flags.TargetNPC).Criaturas(PetIndex).NpcIndex,
                                          Npclist(.flags.TargetNPC).Pos, True, False)

                    If SpawnedNpc > 0 Then
                        Npclist(SpawnedNpc).MaestroNpc = .flags.TargetNPC
                        Npclist(.flags.TargetNPC).Mascotas = Npclist(.flags.TargetNPC).Mascotas + 1
                    End If
                End If
            Else
                Call _
                    SendData(SendTarget.ToPCArea, UserIndex,
                             PrepareMessageChatOverHead("No puedo traer más criaturas, mata las existentes.",
                                                        Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                                        ColorTranslator.ToOle(Color.White)))
            End If
        End With
    End Sub

    ''
    ' Handles the "CommerceBuy" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleCommerceBuy(UserIndex As Short)
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
                Call _
                    SendData(SendTarget.ToPCArea, UserIndex,
                             PrepareMessageChatOverHead("No tengo ningún interés en comerciar.",
                                                        Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                                        ColorTranslator.ToOle(Color.White)))
                Exit Sub
            End If

            'Only if in commerce mode....
            If Not .flags.Comerciando Then
                Call WriteConsoleMsg(UserIndex, "No estás comerciando.", FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            'User compra el item
            Call Comercio(eModoComercio.Compra, UserIndex, .flags.TargetNPC, Slot, Amount)
        End With
    End Sub

    ''
    ' Handles the "BankExtractItem" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleBankExtractItem(UserIndex As Short)
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
            If Npclist(.flags.TargetNPC).NPCtype <> eNPCType.Banquero Then
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

    Private Sub HandleCommerceSell(UserIndex As Short)
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
                Call _
                    SendData(SendTarget.ToPCArea, UserIndex,
                             PrepareMessageChatOverHead("No tengo ningún interés en comerciar.",
                                                        Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                                        ColorTranslator.ToOle(Color.White)))
                Exit Sub
            End If

            'User compra el item del slot
            Call Comercio(eModoComercio.Venta, UserIndex, .flags.TargetNPC, Slot, Amount)
        End With
    End Sub

    ''
    ' Handles the "BankDeposit" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleBankDeposit(UserIndex As Short)
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
            If Npclist(.flags.TargetNPC).NPCtype <> eNPCType.Banquero Then
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

    Private Sub HandleForumPost(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 02/01/2010
        '02/01/2010: ZaMa - Implemento nuevo sistema de foros
        '***************************************************
        If UserList(UserIndex).incomingData.length < 6 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim ForumMsgType As eForumMsgType
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

                        Case eForumType.ieGeneral
                            ForumIndex = GetForumIndex(ObjData_Renamed(.flags.TargetObj).ForoID)

                        Case eForumType.ieREAL
                            ForumIndex = GetForumIndex(FORO_REAL_ID)

                        Case eForumType.ieCAOS
                            ForumIndex = GetForumIndex(FORO_CAOS_ID)

                    End Select

                    Call AddPost(ForumIndex, Post, .name, Title, EsAnuncio(ForumMsgType))
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleForumPost: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "MoveSpell" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleMoveSpell(UserIndex As Short)
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
                dir_Renamed = - 1
            End If

            Call DesplazarHechizo(UserIndex, dir_Renamed, .ReadByte())
        End With
    End Sub

    ''
    ' Handles the "MoveBank" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleMoveBank(UserIndex As Short)
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
                dir_Renamed = - 1
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

    Private Sub HandleClanCodexUpdate(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 5 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                Call ChangeCodexAndDesc(desc, codex, .GuildIndex)

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleClanCodexUpdate: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "UserCommerceOffer" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleUserCommerceOffer(UserIndex As Short)
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
                    Call FlushBuffer(tUser)
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
                    Call _
                        WriteCommerceChat(UserIndex, "No tienes esa cantidad de oro para agregar a la oferta.",
                                          FontTypeNames.FONTTYPE_TALK)
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
                        Call _
                            WriteCommerceChat(UserIndex, "No puedes vender tu barco mientras lo estés usando.",
                                              FontTypeNames.FONTTYPE_TALK)
                        Exit Sub
                    End If
                End If

                If .Invent.MochilaEqpSlot > 0 Then
                    If .Invent.MochilaEqpSlot = Slot Then
                        Call _
                            WriteCommerceChat(UserIndex, "No puedes vender tu mochila mientras la estés usando.",
                                              FontTypeNames.FONTTYPE_TALK)
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

    Private Sub HandleGuildAcceptPeace(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                otherClanIndex = CStr(r_AceptarPropuestaDePaz(UserIndex, guild, errorStr))

                If CDbl(otherClanIndex) = 0 Then
                    Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
                Else
                    Call _
                        SendData(SendTarget.ToGuildMembers, .GuildIndex,
                                 PrepareMessageConsoleMsg("Tu clan ha firmado la paz con " & guild & ".",
                                                          FontTypeNames.FONTTYPE_GUILD))
                    Call _
                        SendData(SendTarget.ToGuildMembers, CShort(otherClanIndex),
                                 PrepareMessageConsoleMsg(
                                     "Tu clan ha firmado la paz con " & GuildName(.GuildIndex) & ".",
                                     FontTypeNames.FONTTYPE_GUILD))
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildAcceptPeace: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildRejectAlliance" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildRejectAlliance(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                otherClanIndex = CStr(r_RechazarPropuestaDeAlianza(UserIndex, guild, errorStr))

                If CDbl(otherClanIndex) = 0 Then
                    Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
                Else
                    Call _
                        SendData(SendTarget.ToGuildMembers, .GuildIndex,
                                 PrepareMessageConsoleMsg("Tu clan rechazado la propuesta de alianza de " & guild,
                                                          FontTypeNames.FONTTYPE_GUILD))
                    Call _
                        SendData(SendTarget.ToGuildMembers, CShort(otherClanIndex),
                                 PrepareMessageConsoleMsg(
                                     GuildName(.GuildIndex) &
                                     " ha rechazado nuestra propuesta de alianza con su clan.",
                                     FontTypeNames.FONTTYPE_GUILD))
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildRejectAlliance: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildRejectPeace" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildRejectPeace(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                otherClanIndex = CStr(r_RechazarPropuestaDePaz(UserIndex, guild, errorStr))

                If CDbl(otherClanIndex) = 0 Then
                    Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
                Else
                    Call _
                        SendData(SendTarget.ToGuildMembers, .GuildIndex,
                                 PrepareMessageConsoleMsg("Tu clan rechazado la propuesta de paz de " & guild & ".",
                                                          FontTypeNames.FONTTYPE_GUILD))
                    Call _
                        SendData(SendTarget.ToGuildMembers, CShort(otherClanIndex),
                                 PrepareMessageConsoleMsg(
                                     GuildName(.GuildIndex) &
                                     " ha rechazado nuestra propuesta de paz con su clan.", FontTypeNames.FONTTYPE_GUILD))
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildRejectPeace: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildAcceptAlliance" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildAcceptAlliance(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                otherClanIndex = CStr(r_AceptarPropuestaDeAlianza(UserIndex, guild, errorStr))

                If CDbl(otherClanIndex) = 0 Then
                    Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
                Else
                    Call _
                        SendData(SendTarget.ToGuildMembers, .GuildIndex,
                                 PrepareMessageConsoleMsg("Tu clan ha firmado la alianza con " & guild & ".",
                                                          FontTypeNames.FONTTYPE_GUILD))
                    Call _
                        SendData(SendTarget.ToGuildMembers, CShort(otherClanIndex),
                                 PrepareMessageConsoleMsg(
                                     "Tu clan ha firmado la paz con " & GuildName(.GuildIndex) & ".",
                                     FontTypeNames.FONTTYPE_GUILD))
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildAcceptAlliance: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildOfferPeace" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildOfferPeace(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 5 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If r_ClanGeneraPropuesta(UserIndex, guild, RELACIONES_GUILD.PAZ, proposal, errorStr) _
                    Then
                    Call WriteConsoleMsg(UserIndex, "Propuesta de paz enviada.", FontTypeNames.FONTTYPE_GUILD)
                Else
                    Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildOfferPeace: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildOfferAlliance" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildOfferAlliance(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 5 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If r_ClanGeneraPropuesta(UserIndex, guild, RELACIONES_GUILD.ALIADOS, proposal, errorStr) _
                    Then
                    Call WriteConsoleMsg(UserIndex, "Propuesta de alianza enviada.", FontTypeNames.FONTTYPE_GUILD)
                Else
                    Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildOfferAlliance: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildAllianceDetails" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildAllianceDetails(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                details = r_VerPropuesta(UserIndex, guild, RELACIONES_GUILD.ALIADOS, errorStr)

                If migr_LenB(details) = 0 Then
                    Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
                Else
                    Call WriteOfferDetails(UserIndex, details)
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildAllianceDetails: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildPeaceDetails" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildPeaceDetails(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                details = r_VerPropuesta(UserIndex, guild, RELACIONES_GUILD.PAZ, errorStr)

                If migr_LenB(details) = 0 Then
                    Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
                Else
                    Call WriteOfferDetails(UserIndex, details)
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildPeaceDetails: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildRequestJoinerInfo" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildRequestJoinerInfo(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                details = a_DetallesAspirante(UserIndex, User_Renamed)

                If migr_LenB(details) = 0 Then
                    Call _
                        WriteConsoleMsg(UserIndex,
                                        "El personaje no ha mandado solicitud, o no estás habilitado para verla.",
                                        FontTypeNames.FONTTYPE_GUILD)
                Else
                    Call WriteShowUserRequest(UserIndex, details)
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildRequestJoinerInfo: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildAlliancePropList" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildAlliancePropList(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        'Remove packet ID
        Call UserList(UserIndex).incomingData.ReadByte()

        Call WriteAlianceProposalsList(UserIndex, r_ListaDePropuestas(UserIndex, RELACIONES_GUILD.ALIADOS))
    End Sub

    ''
    ' Handles the "GuildPeacePropList" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildPeacePropList(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        'Remove packet ID
        Call UserList(UserIndex).incomingData.ReadByte()

        Call WritePeaceProposalsList(UserIndex, r_ListaDePropuestas(UserIndex, RELACIONES_GUILD.PAZ))
    End Sub

    ''
    ' Handles the "GuildDeclareWar" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildDeclareWar(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                otherGuildIndex = r_DeclararGuerra(UserIndex, guild, errorStr)

                If otherGuildIndex = 0 Then
                    Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
                Else
                    'WAR shall be!
                    Call _
                        SendData(SendTarget.ToGuildMembers, .GuildIndex,
                                 PrepareMessageConsoleMsg("TU CLAN HA ENTRADO EN GUERRA CON " & guild & ".",
                                                          FontTypeNames.FONTTYPE_GUILD))
                    Call _
                        SendData(SendTarget.ToGuildMembers, otherGuildIndex,
                                 PrepareMessageConsoleMsg(
                                     GuildName(.GuildIndex) & " LE DECLARA LA GUERRA A TU CLAN.",
                                     FontTypeNames.FONTTYPE_GUILD))
                    Call _
                        SendData(SendTarget.ToGuildMembers, .GuildIndex,
                                 PrepareMessagePlayWave(45, NO_3D_SOUND, NO_3D_SOUND))
                    Call _
                        SendData(SendTarget.ToGuildMembers, otherGuildIndex,
                                 PrepareMessagePlayWave(45, NO_3D_SOUND, NO_3D_SOUND))
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildDeclareWar: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildNewWebsite" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildNewWebsite(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()

                Call ActualizarWebSite(UserIndex, buffer.ReadASCIIString())

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildNewWebsite: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildAcceptNewMember" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildAcceptNewMember(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If Not a_AceptarAspirante(UserIndex, UserName, errorStr) Then
                    Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
                Else
                    tUser = NameIndex(UserName)
                    If tUser > 0 Then
                        Call m_ConectarMiembroAClan(tUser, .GuildIndex)
                        Call RefreshCharStatus(tUser)
                    End If

                    Call _
                        SendData(SendTarget.ToGuildMembers, .GuildIndex,
                                 PrepareMessageConsoleMsg(UserName & " ha sido aceptado como miembro del clan.",
                                                          FontTypeNames.FONTTYPE_GUILD))
                    Call _
                        SendData(SendTarget.ToGuildMembers, .GuildIndex,
                                 PrepareMessagePlayWave(43, NO_3D_SOUND, NO_3D_SOUND))
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildAcceptNewMember: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildRejectNewMember" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildRejectNewMember(UserIndex As Short)
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

        Try
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

                If Not a_RechazarAspirante(UserIndex, UserName, errorStr) Then
                    Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
                Else
                    tUser = NameIndex(UserName)

                    If tUser > 0 Then
                        Call WriteConsoleMsg(tUser, errorStr & " : " & reason, FontTypeNames.FONTTYPE_GUILD)
                    Else
                        'hay que grabar en el char su rechazo
                        Call a_RechazarAspiranteChar(UserName, .GuildIndex, reason)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildRejectNewMember: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildKickMember" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildKickMember(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim GuildIndex As Short
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()

                GuildIndex = m_EcharMiembroDeClan(UserIndex, UserName)

                If GuildIndex > 0 Then
                    Call _
                        SendData(SendTarget.ToGuildMembers, GuildIndex,
                                 PrepareMessageConsoleMsg(UserName & " fue expulsado del clan.",
                                                          FontTypeNames.FONTTYPE_GUILD))
                    Call _
                        SendData(SendTarget.ToGuildMembers, GuildIndex,
                                 PrepareMessagePlayWave(45, NO_3D_SOUND, NO_3D_SOUND))
                Else
                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes expulsar ese personaje del clan.",
                                        FontTypeNames.FONTTYPE_GUILD)
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildKickMember: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildUpdateNews" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildUpdateNews(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()

                Call ActualizarNoticias(UserIndex, buffer.ReadASCIIString())

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildUpdateNews: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildMemberInfo" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildMemberInfo(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()

                Call SendDetallesPersonaje(UserIndex, buffer.ReadASCIIString())

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildMemberInfo: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildOpenElections" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildOpenElections(UserIndex As Short)
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


            If Not v_AbrirElecciones(UserIndex, error_Renamed) Then
                Call WriteConsoleMsg(UserIndex, error_Renamed, FontTypeNames.FONTTYPE_GUILD)
            Else
                Call _
                    SendData(SendTarget.ToGuildMembers, .GuildIndex,
                             PrepareMessageConsoleMsg(
                                 "¡Han comenzado las elecciones del clan! Puedes votar escribiendo /VOTO seguido del nombre del personaje, por ejemplo: /VOTO " &
                                 .name, FontTypeNames.FONTTYPE_GUILD))
            End If
        End With
    End Sub

    ''
    ' Handles the "GuildRequestMembership" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildRequestMembership(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 5 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If Not a_NuevoAspirante(UserIndex, guild, application, errorStr) Then
                    Call WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD)
                Else
                    Call _
                        WriteConsoleMsg(UserIndex,
                                        "Tu solicitud ha sido enviada. Espera prontas noticias del líder de " & guild &
                                        ".",
                                        FontTypeNames.FONTTYPE_GUILD)
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildRequestMembership: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildRequestDetails" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildRequestDetails(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()

                Call SendGuildDetails(UserIndex, buffer.ReadASCIIString())

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildRequestDetails: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "Online" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleOnline(UserIndex As Short)
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
                    If _
                        UserList(i).flags.Privilegios And
                        (PlayerType.User Or PlayerType.Consejero) Then Count = Count + 1
                End If
            Next i

            Call WriteConsoleMsg(UserIndex, "Número de usuarios: " & CStr(Count), FontTypeNames.FONTTYPE_INFO)
        End With
    End Sub

    ''
    ' Handles the "Quit" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleQuit(UserIndex As Short)
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
                        Call _
                            WriteConsoleMsg(tUser, "Comercio cancelado por el otro usuario.",
                                            FontTypeNames.FONTTYPE_TALK)
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

    Private Sub HandleGuildLeave(UserIndex As Short)
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
                Call _
                    SendData(SendTarget.ToGuildMembers, GuildIndex,
                             PrepareMessageConsoleMsg(.name & " deja el clan.", FontTypeNames.FONTTYPE_GUILD))
            Else
                Call WriteConsoleMsg(UserIndex, "Tú no puedes salir de este clan.", FontTypeNames.FONTTYPE_GUILD)
            End If
        End With
    End Sub

    ''
    ' Handles the "RequestAccountState" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleRequestAccountState(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If Distancia(Npclist(.flags.TargetNPC).Pos, .Pos) > 3 Then
                Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos del vendedor.", FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            Select Case Npclist(.flags.TargetNPC).NPCtype
                Case eNPCType.Banquero
                    Call _
                        WriteChatOverHead(UserIndex, "Tienes " & .Stats.Banco & " monedas de oro en tu cuenta.",
                                          Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                          ColorTranslator.ToOle(Color.White))

                Case eNPCType.Timbero
                    If Not .flags.Privilegios And PlayerType.User Then
                        earnings = Apuestas.Ganancias - Apuestas.Perdidas

                        If earnings >= 0 And Apuestas.Ganancias <> 0 Then
                            Percentage = Int(earnings*100/Apuestas.Ganancias)
                        End If

                        If earnings < 0 And Apuestas.Perdidas <> 0 Then
                            Percentage = Int(earnings*100/Apuestas.Perdidas)
                        End If

                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "Entradas: " & Apuestas.Ganancias & " Salida: " & Apuestas.Perdidas &
                                            " Ganancia Neta: " & earnings & " (" & Percentage & "%) Jugadas: " &
                                            Apuestas.Jugadas, FontTypeNames.FONTTYPE_INFO)
                    End If
            End Select
        End With
    End Sub

    ''
    ' Handles the "PetStand" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandlePetStand(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                                    FontTypeNames.FONTTYPE_INFO)
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
            Npclist(.flags.TargetNPC).Movement = TipoAI.ESTATICO

            Call Expresar(.flags.TargetNPC, UserIndex)
        End With
    End Sub

    ''
    ' Handles the "PetFollow" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandlePetFollow(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                                    FontTypeNames.FONTTYPE_INFO)
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

    Private Sub HandleReleasePet(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                                    FontTypeNames.FONTTYPE_INFO)
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

    Private Sub HandleTrainList(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            'Make sure it's close enough
            If Distancia(Npclist(.flags.TargetNPC).Pos, .Pos) > 10 Then
                Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            'Make sure it's the trainer
            If Npclist(.flags.TargetNPC).NPCtype <> eNPCType.Entrenador Then Exit Sub

            Call WriteTrainerCreatureList(UserIndex, .flags.TargetNPC)
        End With
    End Sub

    ''
    ' Handles the "Rest" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleRest(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Solo puedes usar ítems cuando estás vivo.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If HayOBJarea(.Pos, FOGATA) Then
                Call WriteRestOK(UserIndex)

                If Not .flags.Descansar Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Te acomodás junto a la fogata y comienzas a descansar.",
                                        FontTypeNames.FONTTYPE_INFO)
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

                Call _
                    WriteConsoleMsg(UserIndex, "No hay ninguna fogata junto a la cual descansar.",
                                    FontTypeNames.FONTTYPE_INFO)
            End If
        End With
    End Sub

    ''
    ' Handles the "Meditate" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleMeditate(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Sólo puedes meditar cuando estás vivo.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            'Can he meditate?
            If .Stats.MaxMAN = 0 Then
                Call _
                    WriteConsoleMsg(UserIndex, "Sólo las clases mágicas conocen el arte de la meditación.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            'Admins don't have to wait :D
            If Not .flags.Privilegios And PlayerType.User Then
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

                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Te estás concentrando. En " & Fix(TIEMPO_INICIOMEDITAR/1000) &
                                    " segundos comenzarás a meditar.", FontTypeNames.FONTTYPE_INFO)

                .Char_Renamed.loops = INFINITE_LOOPS

                'Show proper FX according to level
                If .Stats.ELV < 13 Then
                    .Char_Renamed.FX = FXIDs.FXMEDITARCHICO

                ElseIf .Stats.ELV < 25 Then
                    .Char_Renamed.FX = FXIDs.FXMEDITARMEDIANO

                ElseIf .Stats.ELV < 35 Then
                    .Char_Renamed.FX = FXIDs.FXMEDITARGRANDE

                ElseIf .Stats.ELV < 42 Then
                    .Char_Renamed.FX = FXIDs.FXMEDITARXGRANDE

                Else
                    .Char_Renamed.FX = FXIDs.FXMEDITARXXGRANDE
                End If

                Call _
                    SendData(SendTarget.ToPCArea, UserIndex,
                             PrepareMessageCreateFX(.Char_Renamed.CharIndex, .Char_Renamed.FX, INFINITE_LOOPS))
            Else
                .Counters.bPuedeMeditar = False

                .Char_Renamed.FX = 0
                .Char_Renamed.loops = 0
                Call _
                    SendData(SendTarget.ToPCArea, UserIndex,
                             PrepareMessageCreateFX(.Char_Renamed.CharIndex, 0, 0))
            End If
        End With
    End Sub

    ''
    ' Handles the "Resucitate" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleResucitate(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            'Validate NPC and make sure player is dead
            If _
                (Npclist(.flags.TargetNPC).NPCtype <> eNPCType.Revividor And
                 (Npclist(.flags.TargetNPC).NPCtype <> eNPCType.ResucitadorNewbie Or
                  Not EsNewbie(UserIndex))) Or .flags.Muerto = 0 Then Exit Sub

            'Make sure it's close enough
            If Distancia(.Pos, Npclist(.flags.TargetNPC).Pos) > 10 Then
                Call _
                    WriteConsoleMsg(UserIndex, "El sacerdote no puede resucitarte debido a que estás demasiado lejos.",
                                    FontTypeNames.FONTTYPE_INFO)
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

    Private Sub HandleConsultation(UserIndex As String)
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
                Call _
                    WriteConsoleMsg(CShort(UserIndex),
                                    "Primero tienes que seleccionar un usuario, haz click izquierdo sobre él.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            ' No podes ponerte a vos mismo en modo consulta.
            If UserConsulta = CDbl(UserIndex) Then Exit Sub

            ' No podes estra en consulta con otro gm
            If EsGM(UserConsulta) Then
                Call _
                    WriteConsoleMsg(CShort(UserIndex), "No puedes iniciar el modo consulta con otro administrador.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            UserName = UserList(UserConsulta).name

            ' Si ya estaba en consulta, termina la consulta
            If UserList(UserConsulta).flags.EnConsulta Then
                Call _
                    WriteConsoleMsg(CShort(UserIndex), "Has terminado el modo consulta con " & UserName & ".",
                                    FontTypeNames.FONTTYPE_INFOBOLD)
                Call WriteConsoleMsg(UserConsulta, "Has terminado el modo consulta.", FontTypeNames.FONTTYPE_INFOBOLD)
                Call LogGM(.name, "Termino consulta con " & UserName)

                UserList(UserConsulta).flags.EnConsulta = False

                ' Sino la inicia
            Else
                Call _
                    WriteConsoleMsg(CShort(UserIndex), "Has iniciado el modo consulta con " & UserName & ".",
                                    FontTypeNames.FONTTYPE_INFOBOLD)
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

                        Call SetInvisible(UserConsulta, UserList(UserConsulta).Char_Renamed.CharIndex, False)
                    End If
                End With
            End If

            Call SetConsulatMode(UserConsulta)
        End With
    End Sub

    ''
    ' Handles the "Heal" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleHeal(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If _
                (Npclist(.flags.TargetNPC).NPCtype <> eNPCType.Revividor And
                 Npclist(.flags.TargetNPC).NPCtype <> eNPCType.ResucitadorNewbie) Or .flags.Muerto <> 0 _
                Then Exit Sub

            If Distancia(.Pos, Npclist(.flags.TargetNPC).Pos) > 10 Then
                Call _
                    WriteConsoleMsg(UserIndex, "El sacerdote no puede curarte debido a que estás demasiado lejos.",
                                    FontTypeNames.FONTTYPE_INFO)
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

    Private Sub HandleRequestStats(UserIndex As Short)
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

    Private Sub HandleHelp(UserIndex As Short)
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

    Private Sub HandleCommerceStart(UserIndex As Short)
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
                        Call _
                            WriteChatOverHead(UserIndex, "No tengo ningún interés en comerciar.",
                                              Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                              ColorTranslator.ToOle(Color.White))
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
                If .flags.Privilegios And PlayerType.Consejero Then
                    Call WriteConsoleMsg(UserIndex, "No puedes vender ítems.", FontTypeNames.FONTTYPE_WARNING)
                    Exit Sub
                End If

                'Is the other one dead??
                If UserList(.flags.TargetUser).flags.Muerto = 1 Then
                    Call _
                        WriteConsoleMsg(UserIndex, "¡¡No puedes comerciar con los muertos!!",
                                        FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If

                'Is it me??
                If .flags.TargetUser = UserIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "¡¡No puedes comerciar con vos mismo!!", FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If

                'Check distance
                If Distancia(UserList(.flags.TargetUser).Pos, .Pos) > 3 Then
                    Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos del usuario.", FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If

                'Is he already trading?? is it with me or someone else??
                If _
                    UserList(.flags.TargetUser).flags.Comerciando = True And
                    UserList(.flags.TargetUser).ComUsu.DestUsu <> UserIndex Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes comerciar con el usuario en este momento.",
                                        FontTypeNames.FONTTYPE_INFO)
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
                Call _
                    WriteConsoleMsg(UserIndex, "Primero haz click izquierdo sobre el personaje.",
                                    FontTypeNames.FONTTYPE_INFO)
            End If
        End With
    End Sub

    ''
    ' Handles the "BankStart" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleBankStart(UserIndex As Short)
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
                If Npclist(.flags.TargetNPC).NPCtype = eNPCType.Banquero Then
                    Call IniciarDeposito(UserIndex)
                End If
            Else
                Call _
                    WriteConsoleMsg(UserIndex, "Primero haz click izquierdo sobre el personaje.",
                                    FontTypeNames.FONTTYPE_INFO)
            End If
        End With
    End Sub

    ''
    ' Handles the "Enlist" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleEnlist(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If Npclist(.flags.TargetNPC).NPCtype <> eNPCType.Noble Or .flags.Muerto <> 0 Then Exit Sub

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

    Private Sub HandleInformation(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If Npclist(.flags.TargetNPC).NPCtype <> eNPCType.Noble Or .flags.Muerto <> 0 Then Exit Sub

            If Distancia(.Pos, Npclist(.flags.TargetNPC).Pos) > 4 Then
                Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If


            NextRecom = .Faccion.NextRecompensa

            If Npclist(.flags.TargetNPC).flags.Faccion = 0 Then
                If .Faccion.ArmadaReal = 0 Then
                    Call _
                        WriteChatOverHead(UserIndex, "¡¡No perteneces a las tropas reales!!",
                                          Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                          ColorTranslator.ToOle(Color.White))
                    Exit Sub
                End If

                Matados = .Faccion.CriminalesMatados
                Diferencia = NextRecom - Matados

                If Diferencia > 0 Then
                    Call _
                        WriteChatOverHead(UserIndex,
                                          "Tu deber es combatir criminales, mata " & Diferencia &
                                          " criminales más y te daré una recompensa.",
                                          Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                          ColorTranslator.ToOle(Color.White))
                Else
                    Call _
                        WriteChatOverHead(UserIndex,
                                          "Tu deber es combatir criminales, y ya has matado los suficientes como para merecerte una recompensa.",
                                          Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                          ColorTranslator.ToOle(Color.White))
                End If
            Else
                If .Faccion.FuerzasCaos = 0 Then
                    Call _
                        WriteChatOverHead(UserIndex, "¡¡No perteneces a la legión oscura!!",
                                          Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                          ColorTranslator.ToOle(Color.White))
                    Exit Sub
                End If

                Matados = .Faccion.CiudadanosMatados
                Diferencia = NextRecom - Matados

                If Diferencia > 0 Then
                    Call _
                        WriteChatOverHead(UserIndex,
                                          "Tu deber es sembrar el caos y la desesperanza, mata " & Diferencia &
                                          " ciudadanos más y te daré una recompensa.",
                                          Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                          ColorTranslator.ToOle(Color.White))
                Else
                    Call _
                        WriteChatOverHead(UserIndex,
                                          "Tu deber es sembrar el caos y la desesperanza, y creo que estás en condiciones de merecer una recompensa.",
                                          Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                          ColorTranslator.ToOle(Color.White))
                End If
            End If
        End With
    End Sub

    ''
    ' Handles the "Reward" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleReward(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If Npclist(.flags.TargetNPC).NPCtype <> eNPCType.Noble Or .flags.Muerto <> 0 Then Exit Sub

            If Distancia(.Pos, Npclist(.flags.TargetNPC).Pos) > 4 Then
                Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If Npclist(.flags.TargetNPC).flags.Faccion = 0 Then
                If .Faccion.ArmadaReal = 0 Then
                    Call _
                        WriteChatOverHead(UserIndex, "¡¡No perteneces a las tropas reales!!",
                                          Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                          ColorTranslator.ToOle(Color.White))
                    Exit Sub
                End If
                Call RecompensaArmadaReal(UserIndex)
            Else
                If .Faccion.FuerzasCaos = 0 Then
                    Call _
                        WriteChatOverHead(UserIndex, "¡¡No perteneces a la legión oscura!!",
                                          Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                          ColorTranslator.ToOle(Color.White))
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

    Private Sub HandleRequestMOTD(UserIndex As Short)
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

    Private Sub HandleUpTime(UserIndex As Short)
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
        time = (GetTickCount() - tInicioServer)\1000

        'Get times in dd:hh:mm:ss format
        UpTimeStr = (time Mod 60) & " segundos."
        time = time\60

        UpTimeStr = (time Mod 60) & " minutos, " & UpTimeStr
        time = time\60

        UpTimeStr = (time Mod 24) & " horas, " & UpTimeStr
        time = time\24

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

    Private Sub HandlePartyLeave(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        'Remove packet ID
        Call UserList(UserIndex).incomingData.ReadByte()

        Call SalirDeParty(UserIndex)
    End Sub

    ''
    ' Handles the "PartyCreate" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandlePartyCreate(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        'Remove packet ID
        Call UserList(UserIndex).incomingData.ReadByte()

        If Not PuedeCrearParty(UserIndex) Then Exit Sub

        Call CrearParty(UserIndex)
    End Sub

    ''
    ' Handles the "PartyJoin" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandlePartyJoin(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        'Remove packet ID
        Call UserList(UserIndex).incomingData.ReadByte()

        Call SolicitarIngresoAParty(UserIndex)
    End Sub

    ''
    ' Handles the "ShareNpc" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleShareNpc(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex, "No puedes compartir npcs con administradores!!",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            ' Pk or Caos?
            If criminal(UserIndex) Then
                ' Caos can only share with other caos
                If esCaos(UserIndex) Then
                    If Not esCaos(TargetUserIndex) Then
                        Call _
                            WriteConsoleMsg(UserIndex, "Solo puedes compartir npcs con miembros de tu misma facción!!",
                                            FontTypeNames.FONTTYPE_INFO)
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
                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes compartir npcs con criminales!!",
                                        FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If
            End If

            ' Already sharing with target
            SharingUserIndex = .flags.ShareNpcWith
            If SharingUserIndex = TargetUserIndex Then Exit Sub

            ' Aviso al usuario anterior que dejo de compartir
            If SharingUserIndex <> 0 Then
                Call _
                    WriteConsoleMsg(SharingUserIndex, .name & " ha dejado de compartir sus npcs contigo.",
                                    FontTypeNames.FONTTYPE_INFO)
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Has dejado de compartir tus npcs con " & UserList(SharingUserIndex).name & ".",
                                    FontTypeNames.FONTTYPE_INFO)
            End If

            .flags.ShareNpcWith = TargetUserIndex

            Call _
                WriteConsoleMsg(TargetUserIndex, .name & " ahora comparte sus npcs contigo.",
                                FontTypeNames.FONTTYPE_INFO)
            Call _
                WriteConsoleMsg(UserIndex, "Ahora compartes tus npcs con " & UserList(TargetUserIndex).name & ".",
                                FontTypeNames.FONTTYPE_INFO)

        End With
    End Sub

    ''
    ' Handles the "StopSharingNpc" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleStopSharingNpc(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(SharingUserIndex, .name & " ha dejado de compartir sus npcs contigo.",
                                    FontTypeNames.FONTTYPE_INFO)
                Call _
                    WriteConsoleMsg(SharingUserIndex,
                                    "Has dejado de compartir tus npcs con " & UserList(SharingUserIndex).name & ".",
                                    FontTypeNames.FONTTYPE_INFO)

                .flags.ShareNpcWith = 0
            End If

        End With
    End Sub

    ''
    ' Handles the "Inquiry" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleInquiry(UserIndex As Short)
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

    Private Sub HandleGuildMessage(UserIndex As Short)
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

        Try
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
                    Call ParseChat(Chat)

                    If .GuildIndex > 0 Then
                        Call _
                            SendData(SendTarget.ToDiosesYclan, .GuildIndex,
                                     PrepareMessageGuildChat(.name & "> " & Chat))

                        If Not (.flags.AdminInvisible = 1) Then _
                            Call _
                                SendData(SendTarget.ToClanArea, UserIndex,
                                         PrepareMessageChatOverHead("< " & Chat & " >", .Char_Renamed.CharIndex,
                                                                    ColorTranslator.ToOle(
                                                                        Color.Yellow)))
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildMessage: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "PartyMessage" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandlePartyMessage(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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
                    Call ParseChat(Chat)

                    Call BroadCastParty(UserIndex, Chat)
                    'TODO : Con la 0.12.1 se debe definir si esto vuelve o se borra (/CMSG overhead)
                    'Call SendData(SendTarget.ToPartyArea, UserIndex, UserList(UserIndex).Pos.map, "||" & vbYellow & "°< " & mid$(rData, 7) & " >°" & CStr(UserList(UserIndex).Char.CharIndex))
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandlePartyMessage: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "CentinelReport" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleCentinelReport(UserIndex As Short)
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

    Private Sub HandleGuildOnline(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        Dim onlineList As String
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()


            onlineList = m_ListaDeMiembrosOnline(UserIndex, .GuildIndex)

            If .GuildIndex <> 0 Then
                Call _
                    WriteConsoleMsg(UserIndex, "Compañeros de tu clan conectados: " & onlineList,
                                    FontTypeNames.FONTTYPE_GUILDMSG)
            Else
                Call WriteConsoleMsg(UserIndex, "No pertences a ningún clan.", FontTypeNames.FONTTYPE_GUILDMSG)
            End If
        End With
    End Sub

    ''
    ' Handles the "PartyOnline" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandlePartyOnline(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        'Remove packet ID
        Call UserList(UserIndex).incomingData.ReadByte()

        Call OnlineParty(UserIndex)
    End Sub

    ''
    ' Handles the "CouncilMessage" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleCouncilMessage(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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
                    Call ParseChat(Chat)

                    If .flags.Privilegios And PlayerType.RoyalCouncil Then
                        Call _
                            SendData(SendTarget.ToConsejo, UserIndex,
                                     PrepareMessageConsoleMsg("(Consejero) " & .name & "> " & Chat,
                                                              FontTypeNames.FONTTYPE_CONSEJO))
                    ElseIf .flags.Privilegios And PlayerType.ChaosCouncil Then
                        Call _
                            SendData(SendTarget.ToConsejoCaos, UserIndex,
                                     PrepareMessageConsoleMsg("(Consejero) " & .name & "> " & Chat,
                                                              FontTypeNames.FONTTYPE_CONSEJOCAOS))
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleCouncilMessage: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "RoleMasterRequest" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleRoleMasterRequest(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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
                    Call _
                        SendData(SendTarget.ToRolesMasters, 0,
                                 PrepareMessageConsoleMsg(.name & " PREGUNTA ROL: " & request,
                                                          FontTypeNames.FONTTYPE_GUILDMSG))
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleRoleMasterRequest: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GMRequest" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGMRequest(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If Not Ayuda.Existe(.name) Then
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "El mensaje ha sido entregado, ahora sólo debes esperar que se desocupe algún GM.",
                                    FontTypeNames.FONTTYPE_INFO)
                Call Ayuda.Push(.name)
            Else
                Call Ayuda.Quitar(.name)
                Call Ayuda.Push(.name)
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Ya habías mandado un mensaje, tu mensaje ha sido movido al final de la cola de mensajes.",
                                    FontTypeNames.FONTTYPE_INFO)
            End If
        End With
    End Sub

    ''
    ' Handles the "BugReport" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleBugReport(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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
                FileOpen(N, AppDomain.CurrentDomain.BaseDirectory & "LOGS/BUGs.log", OpenMode.Append, , OpenShare.Shared)
                PrintLine(N, "Usuario:" & .name & "  Fecha:" & Today & "    Hora:" & TimeOfDay)
                PrintLine(N, "BUG:")
                PrintLine(N, bugReport)
                PrintLine(N, "########################################################################")
                FileClose(N)

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleBugReport: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "ChangeDescription" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleChangeDescription(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim Description As String
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                Description = buffer.ReadASCIIString()

                If .flags.Muerto = 1 Then
                    Call _
                        WriteConsoleMsg(UserIndex, "No puedes cambiar la descripción estando muerto.",
                                        FontTypeNames.FONTTYPE_INFO)
                Else
                    If Not AsciiValidos(Description) Then
                        Call _
                            WriteConsoleMsg(UserIndex, "La descripción tiene caracteres inválidos.",
                                            FontTypeNames.FONTTYPE_INFO)
                    Else
                        .desc = Trim(Description)
                        Call WriteConsoleMsg(UserIndex, "La descripción ha cambiado.", FontTypeNames.FONTTYPE_INFO)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleChangeDescription: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildVote" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildVote(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim vote As String
            Dim errorStr As String
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                vote = buffer.ReadASCIIString()

                If Not v_UsuarioVota(UserIndex, vote, errorStr) Then
                    Call WriteConsoleMsg(UserIndex, "Voto NO contabilizado: " & errorStr, FontTypeNames.FONTTYPE_GUILD)
                Else
                    Call WriteConsoleMsg(UserIndex, "Voto contabilizado.", FontTypeNames.FONTTYPE_GUILD)
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildVote: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "ShowGuildNews" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleShowGuildNews(UserIndex As Short)
        '***************************************************
        'Author: ZaMA
        'Last Modification: 05/17/06
        '
        '***************************************************

        With UserList(UserIndex)

            'Remove packet ID
            Call .incomingData.ReadByte()

            Call SendGuildNews(UserIndex)
        End With
    End Sub

    ''
    ' Handles the "Punishments" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandlePunishments(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 25/08/2009
        '25/08/2009: ZaMa - Now only admins can see other admins' punishment list
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                    If _
                        (EsAdmin(name) Or EsDios(name) Or EsSemiDios(name) Or EsConsejero(name) Or EsRolesMaster(name)) And
                        (UserList(UserIndex).flags.Privilegios And PlayerType.User) Then
                        Call _
                            WriteConsoleMsg(UserIndex, "No puedes ver las penas de los administradores.",
                                            FontTypeNames.FONTTYPE_INFO)
                    Else
                        If FileExist(CharPath & name & ".chr") Then
                            Count = Val(GetVar(CharPath & name & ".chr", "PENAS", "Cant"))
                            If Count = 0 Then
                                Call WriteConsoleMsg(UserIndex, "Sin prontuario..", FontTypeNames.FONTTYPE_INFO)
                            Else
                                While Count > 0
                                    Call _
                                        WriteConsoleMsg(UserIndex,
                                                        Count & " - " &
                                                        GetVar(CharPath & name & ".chr", "PENAS", "P" & Count),
                                                        FontTypeNames.FONTTYPE_INFO)
                                    Count = Count - 1
                                End While
                            End If
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex, "Personaje """ & name & """ inexistente.",
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandlePunishments: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "ChangePassword" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleChangePassword(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Creation Date: 10/10/07
        'Last Modified By: Rapsodius
        '***************************************************
        If UserList(UserIndex).incomingData.length < 5 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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
                    Call _
                        WriteConsoleMsg(UserIndex, "Debes especificar una contraseña nueva, inténtalo de nuevo.",
                                        FontTypeNames.FONTTYPE_INFO)
                Else
                    oldPass2 = UCase(GetVar(CharPath & UserList(UserIndex).name & ".chr", "INIT", "Password"))

                    If oldPass2 <> oldPass Then
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "La contraseña actual proporcionada no es correcta. La contraseña no ha sido cambiada, inténtalo de nuevo.",
                                            FontTypeNames.FONTTYPE_INFO)
                    Else
                        Call WriteVar(CharPath & UserList(UserIndex).name & ".chr", "INIT", "Password", newPass)
                        Call _
                            WriteConsoleMsg(UserIndex, "La contraseña fue cambiada con éxito.",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleChangePassword: " & ex.Message)
        End Try
    End Sub


    ''
    ' Handles the "Gamble" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGamble(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                                    FontTypeNames.FONTTYPE_INFO)
            ElseIf Distancia(Npclist(.flags.TargetNPC).Pos, .Pos) > 10 Then
                Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
            ElseIf Npclist(.flags.TargetNPC).NPCtype <> eNPCType.Timbero Then
                Call _
                    WriteChatOverHead(UserIndex, "No tengo ningún interés en apostar.",
                                      Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                      ColorTranslator.ToOle(Color.White))
            ElseIf Amount < 1 Then
                Call _
                    WriteChatOverHead(UserIndex, "El mínimo de apuesta es 1 moneda.",
                                      Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                      ColorTranslator.ToOle(Color.White))
            ElseIf Amount > 5000 Then
                Call _
                    WriteChatOverHead(UserIndex, "El máximo de apuesta es 5000 monedas.",
                                      Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                      ColorTranslator.ToOle(Color.White))
            ElseIf .Stats.GLD < Amount Then
                Call _
                    WriteChatOverHead(UserIndex, "No tienes esa cantidad.",
                                      Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                      ColorTranslator.ToOle(Color.White))
            Else
                If RandomNumber(1, 100) <= 47 Then
                    .Stats.GLD = .Stats.GLD + Amount
                    Call _
                        WriteChatOverHead(UserIndex, "¡Felicidades! Has ganado " & CStr(Amount) & " monedas de oro.",
                                          Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                          ColorTranslator.ToOle(Color.White))

                    Apuestas.Perdidas = Apuestas.Perdidas + Amount
                    Call WriteVar(DatPath & "apuestas.dat", "Main", "Perdidas", CStr(Apuestas.Perdidas))
                Else
                    .Stats.GLD = .Stats.GLD - Amount
                    Call _
                        WriteChatOverHead(UserIndex, "Lo siento, has perdido " & CStr(Amount) & " monedas de oro.",
                                          Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                          ColorTranslator.ToOle(Color.White))

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

    Private Sub HandleInquiryVote(UserIndex As Short)
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

    Private Sub HandleBankExtractGold(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If Npclist(.flags.TargetNPC).NPCtype <> eNPCType.Banquero Then Exit Sub

            If Distancia(.Pos, Npclist(.flags.TargetNPC).Pos) > 10 Then
                Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If Amount > 0 And Amount <= .Stats.Banco Then
                .Stats.Banco = .Stats.Banco - Amount
                .Stats.GLD = .Stats.GLD + Amount
                Call _
                    WriteChatOverHead(UserIndex, "Tenés " & .Stats.Banco & " monedas de oro en tu cuenta.",
                                      Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                      ColorTranslator.ToOle(Color.White))
            Else
                Call _
                    WriteChatOverHead(UserIndex, "No tienes esa cantidad.",
                                      Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                      ColorTranslator.ToOle(Color.White))
            End If

            Call WriteUpdateGold(UserIndex)
            Call WriteUpdateBankGold(UserIndex)
        End With
    End Sub

    ''
    ' Handles the "LeaveFaction" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleLeaveFaction(UserIndex As Short)
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
                If Npclist(NpcIndex).NPCtype = eNPCType.Noble Then
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
                    Call _
                        WriteChatOverHead(UserIndex, "¡¡¡Sal de aquí bufón!!!", Npclist(NpcIndex).Char_Renamed.CharIndex,
                                          ColorTranslator.ToOle(Color.White))

                Else
                    ' Si le pidio al rey salir de la armada, le responde.
                    If TalkToKing Then
                        Call _
                            WriteChatOverHead(UserIndex, "Serás bienvenido a las fuerzas imperiales si deseas regresar.",
                                              Npclist(NpcIndex).Char_Renamed.CharIndex,
                                              ColorTranslator.ToOle(Color.White))
                    End If

                    Call ExpulsarFaccionReal(UserIndex, False)

                End If

                'Quit the Chaos Legion?
            ElseIf .Faccion.FuerzasCaos = 1 Then
                ' Si le pidio al rey salir del caos, le responde.
                If TalkToKing Then
                    Call _
                        WriteChatOverHead(UserIndex, "¡¡¡Sal de aquí maldito criminal!!!",
                                          Npclist(NpcIndex).Char_Renamed.CharIndex,
                                          ColorTranslator.ToOle(Color.White))
                Else
                    ' Si le pidio al demonio salir del caos, este le responde.
                    If TalkToDemon Then
                        Call _
                            WriteChatOverHead(UserIndex, "Ya volverás arrastrandote.",
                                              Npclist(NpcIndex).Char_Renamed.CharIndex,
                                              ColorTranslator.ToOle(Color.White))
                    End If

                    Call ExpulsarFaccionCaos(UserIndex, False)
                End If
                ' No es faccionario
            Else

                ' Si le hablaba al rey o demonio, le repsonden ellos
                If NpcIndex > 0 Then
                    Call _
                        WriteChatOverHead(UserIndex, "¡No perteneces a ninguna facción!",
                                          Npclist(NpcIndex).Char_Renamed.CharIndex,
                                          ColorTranslator.ToOle(Color.White))
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

    Private Sub HandleBankDepositGold(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If Distancia(Npclist(.flags.TargetNPC).Pos, .Pos) > 10 Then
                Call WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If Npclist(.flags.TargetNPC).NPCtype <> eNPCType.Banquero Then Exit Sub

            If Amount > 0 And Amount <= .Stats.GLD Then
                .Stats.Banco = .Stats.Banco + Amount
                .Stats.GLD = .Stats.GLD - Amount
                Call _
                    WriteChatOverHead(UserIndex, "Tenés " & .Stats.Banco & " monedas de oro en tu cuenta.",
                                      Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                      ColorTranslator.ToOle(Color.White))

                Call WriteUpdateGold(UserIndex)
                Call WriteUpdateBankGold(UserIndex)
            Else
                Call _
                    WriteChatOverHead(UserIndex, "No tenés esa cantidad.",
                                      Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                      ColorTranslator.ToOle(Color.White))
            End If
        End With
    End Sub

    ''
    ' Handles the "Denounce" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleDenounce(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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
                    Call ParseChat(Text)

                    Call _
                        SendData(SendTarget.ToAdmins, 0,
                                 PrepareMessageConsoleMsg(LCase(.name) & " DENUNCIA: " & Text,
                                                          FontTypeNames.FONTTYPE_GUILDMSG))
                    Call WriteConsoleMsg(UserIndex, "Denuncia enviada, espere..", FontTypeNames.FONTTYPE_INFO)
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleDenounce: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildFundate" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildFundate(UserIndex As Short)
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
                Call _
                    WriteConsoleMsg(UserIndex, "¡Ya has fundado un clan, no puedes fundar otro!",
                                    FontTypeNames.FONTTYPE_INFOBOLD)
                Exit Sub
            End If

            Call WriteShowGuildAlign(UserIndex)
        End With
    End Sub

    ''
    ' Handles the "GuildFundation" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildFundation(UserIndex As Short)
        '***************************************************
        'Author: ZaMa
        'Last Modification: 14/12/2009
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 2 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Dim clanType As eClanType
        'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim error_Renamed As String
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()


            clanType = .incomingData.ReadByte()

            If HasFound(.name) Then
                Call _
                    WriteConsoleMsg(UserIndex, "¡Ya has fundado un clan, no puedes fundar otro!",
                                    FontTypeNames.FONTTYPE_INFOBOLD)
                Call _
                    LogCheating(
                        "El usuario " & .name & " ha intentado fundar un clan ya habiendo fundado otro desde la IP " &
                        .ip)
                Exit Sub
            End If

            Select Case UCase(Trim(CStr(clanType)))
                Case CStr(eClanType.ct_RoyalArmy)
                    .FundandoGuildAlineacion = ALINEACION_GUILD.ALINEACION_ARMADA
                Case CStr(eClanType.ct_Evil)
                    .FundandoGuildAlineacion = ALINEACION_GUILD.ALINEACION_LEGION
                Case CStr(eClanType.ct_Neutral)
                    .FundandoGuildAlineacion = ALINEACION_GUILD.ALINEACION_NEUTRO
                Case CStr(eClanType.ct_GM)
                    .FundandoGuildAlineacion = ALINEACION_GUILD.ALINEACION_MASTER
                Case CStr(eClanType.ct_Legal)
                    .FundandoGuildAlineacion = ALINEACION_GUILD.ALINEACION_CIUDA
                Case CStr(eClanType.ct_Criminal)
                    .FundandoGuildAlineacion = ALINEACION_GUILD.ALINEACION_CRIMINAL
                Case Else
                    Call WriteConsoleMsg(UserIndex, "Alineación inválida.", FontTypeNames.FONTTYPE_GUILD)
                    Exit Sub
            End Select

            If PuedeFundarUnClan(UserIndex, .FundandoGuildAlineacion, error_Renamed) Then
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

    Private Sub HandlePartyKick(UserIndex As Short)
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

        Try
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
                        Call ExpulsarDeParty(UserIndex, tUser)
                    Else
                        If InStr(UserName, "+") Then
                            UserName = Replace(UserName, "+", " ")
                        End If

                        Call _
                            WriteConsoleMsg(UserIndex, LCase(UserName) & " no pertenece a tu party.",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandlePartyKick: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "PartySetLeader" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandlePartySetLeader(UserIndex As Short)
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

        Dim buffer As New clsByteQueue
        Dim UserName As String
        Dim tUser As Short
        Dim rank As Short
        With UserList(UserIndex)
            'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
            Call buffer.CopyBuffer(.incomingData)

            'Remove packet ID
            Call buffer.ReadByte()

            rank = PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios Or
                   PlayerType.Consejero

            UserName = buffer.ReadASCIIString()
            If UserPuedeEjecutarComandos(UserIndex) Then
                tUser = NameIndex(UserName)
                If tUser > 0 Then
                    'Don't allow users to spoof online GMs
                    If (UserDarPrivilegioLevel(UserName) And rank) <= (.flags.Privilegios And rank) Then
                        Call TransformarEnLider(UserIndex, tUser)
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, LCase(UserList(tUser).name) & " no pertenece a tu party.",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If

                Else
                    If InStr(UserName, "+") Then
                        UserName = Replace(UserName, "+", " ")
                    End If
                    Call _
                        WriteConsoleMsg(UserIndex, LCase(UserName) & " no pertenece a tu party.",
                                        FontTypeNames.FONTTYPE_INFO)
                End If
            End If

            'If we got here then packet is complete, copy data back to original queue
            Call .incomingData.CopyBuffer(buffer)
        End With
    End Sub

    ''
    ' Handles the "PartyAcceptMember" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandlePartyAcceptMember(UserIndex As Short)
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

        Try
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


                rank = PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios Or
                       PlayerType.Consejero

                UserName = buffer.ReadASCIIString()
                If UserList(UserIndex).flags.Muerto Then
                    Call WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_PARTY)
                Else
                    bUserVivo = True
                End If

                If UserPuedeEjecutarComandos(UserIndex) And bUserVivo Then
                    tUser = NameIndex(UserName)
                    If tUser > 0 Then
                        'Validate administrative ranks - don't allow users to spoof online GMs
                        If (UserList(tUser).flags.Privilegios And rank) <= (.flags.Privilegios And rank) Then
                            Call AprobarIngresoAParty(UserIndex, tUser)
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex,
                                                "No puedes incorporar a tu party a personajes de mayor jerarquía.",
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    Else
                        If InStr(UserName, "+") Then
                            UserName = Replace(UserName, "+", " ")
                        End If

                        'Don't allow users to spoof online GMs
                        If (UserDarPrivilegioLevel(UserName) And rank) <= (.flags.Privilegios And rank) Then
                            Call _
                                WriteConsoleMsg(UserIndex, LCase(UserName) & " no ha solicitado ingresar a tu party.",
                                                FontTypeNames.FONTTYPE_PARTY)
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex,
                                                "No puedes incorporar a tu party a personajes de mayor jerarquía.",
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandlePartyAcceptMember: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GuildMemberList" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildMemberList(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If .flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios) Then
                    If (migr_InStrB(guild, "\") <> 0) Then
                        guild = Replace(guild, "\", "")
                    End If
                    If (migr_InStrB(guild, "/") <> 0) Then
                        guild = Replace(guild, "/", "")
                    End If

                    If Not FileExist(AppDomain.CurrentDomain.BaseDirectory & "guilds/" & guild & "-members.mem") Then
                        Call WriteConsoleMsg(UserIndex, "No existe el clan: " & guild, FontTypeNames.FONTTYPE_INFO)
                    Else
                        memberCount =
                            Val(GetVar(AppDomain.CurrentDomain.BaseDirectory & "Guilds/" & guild & "-Members" & ".mem",
                                       "INIT", "NroMembers"))

                        For i = 1 To memberCount
                            UserName =
                                GetVar(AppDomain.CurrentDomain.BaseDirectory & "Guilds/" & guild & "-Members" & ".mem",
                                       "Members", "Member" & i)

                            Call WriteConsoleMsg(UserIndex, UserName & "<" & guild & ">", FontTypeNames.FONTTYPE_INFO)
                        Next i
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildMemberList: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GMMessage" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGMMessage(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 01/08/07
        'Last Modification by: (liquid)
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim message As String
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                message = buffer.ReadASCIIString()

                If Not .flags.Privilegios And PlayerType.User Then
                    Call LogGM(.name, "Mensaje a Gms:" & message)

                    If migr_LenB(message) <> 0 Then
                        'Analize chat...
                        Call ParseChat(message)

                        Call _
                            SendData(SendTarget.ToAdmins, 0,
                                     PrepareMessageConsoleMsg(.name & "> " & message, FontTypeNames.FONTTYPE_GMMSG))
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGMMessage: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "ShowName" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleShowName(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.Dios Or PlayerType.Admin Or PlayerType.RoleMaster) _
                Then
                .showName = Not .showName 'Show / Hide the name

                Call RefreshCharStatus(UserIndex)
            End If
        End With
    End Sub

    ''
    ' Handles the "OnlineRoyalArmy" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleOnlineRoyalArmy(UserIndex As Short)
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

            If .flags.Privilegios And PlayerType.User Then Exit Sub


            For i = 1 To LastUser
                If UserList(i).ConnID <> - 1 Then
                    If UserList(i).Faccion.ArmadaReal = 1 Then
                        If _
                            UserList(i).flags.Privilegios And
                            (PlayerType.User Or PlayerType.Consejero Or
                             PlayerType.SemiDios) Or
                            .flags.Privilegios And (PlayerType.Dios Or PlayerType.Admin) _
                            Then
                            list = list & UserList(i).name & ", "
                        End If
                    End If
                End If
            Next i
        End With

        If Len(list) > 0 Then
            Call _
                WriteConsoleMsg(UserIndex, "Reales conectados: " & Left(list, Len(list) - 2),
                                FontTypeNames.FONTTYPE_INFO)
        Else
            Call WriteConsoleMsg(UserIndex, "No hay reales conectados.", FontTypeNames.FONTTYPE_INFO)
        End If
    End Sub

    ''
    ' Handles the "OnlineChaosLegion" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleOnlineChaosLegion(UserIndex As Short)
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

            If .flags.Privilegios And PlayerType.User Then Exit Sub


            For i = 1 To LastUser
                If UserList(i).ConnID <> - 1 Then
                    If UserList(i).Faccion.FuerzasCaos = 1 Then
                        If _
                            UserList(i).flags.Privilegios And
                            (PlayerType.User Or PlayerType.Consejero Or
                             PlayerType.SemiDios) Or
                            .flags.Privilegios And (PlayerType.Dios Or PlayerType.Admin) _
                            Then
                            list = list & UserList(i).name & ", "
                        End If
                    End If
                End If
            Next i
        End With

        If Len(list) > 0 Then
            Call _
                WriteConsoleMsg(UserIndex, "Caos conectados: " & Left(list, Len(list) - 2), FontTypeNames.FONTTYPE_INFO)
        Else
            Call WriteConsoleMsg(UserIndex, "No hay Caos conectados.", FontTypeNames.FONTTYPE_INFO)
        End If
    End Sub

    ''
    ' Handles the "GoNearby" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGoNearby(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 01/10/07
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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
                If _
                    .flags.Privilegios And
                    (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios Or
                     PlayerType.Consejero) Then
                    'Si es dios o Admins no podemos salvo que nosotros también lo seamos
                    If _
                        Not (EsDios(UserName) Or EsAdmin(UserName)) Or
                        (.flags.Privilegios And (PlayerType.Dios Or PlayerType.Admin)) Then
                        If tIndex <= 0 Then 'existe el usuario destino?
                            Call WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO)
                        Else
                            For i = 2 To 5 'esto for sirve ir cambiando la distancia destino
                                For X = UserList(tIndex).Pos.X - i To UserList(tIndex).Pos.X + i
                                    For Y = UserList(tIndex).Pos.Y - i To UserList(tIndex).Pos.Y + i
                                        If MapData(UserList(tIndex).Pos.Map, X, Y).UserIndex = 0 Then
                                            If LegalPos(UserList(tIndex).Pos.Map, X, Y, True, True) Then
                                                Call WarpUserChar(UserIndex, UserList(tIndex).Pos.Map, X, Y, True)
                                                Call _
                                                    LogGM(.name,
                                                          "/IRCERCA " & UserName & " Mapa:" & UserList(tIndex).Pos.Map &
                                                          " X:" & UserList(tIndex).Pos.X & " Y:" &
                                                          UserList(tIndex).Pos.Y)
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
                                Call _
                                    WriteConsoleMsg(UserIndex, "Todos los lugares están ocupados.",
                                                    FontTypeNames.FONTTYPE_INFO)
                            End If
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGoNearby: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "Comment" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleComment(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim comment As String
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()

                comment = buffer.ReadASCIIString()

                If Not .flags.Privilegios And PlayerType.User Then
                    Call LogGM(.name, "Comentario: " & comment)
                    Call WriteConsoleMsg(UserIndex, "Comentario salvado...", FontTypeNames.FONTTYPE_INFO)
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleComment: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "ServerTime" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleServerTime(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 01/08/07
        'Last Modification by: (liquid)
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If .flags.Privilegios And PlayerType.User Then Exit Sub

            Call LogGM(.name, "Hora.")
        End With

        Call _
            SendData(SendTarget.ToAll, 0,
                     PrepareMessageConsoleMsg("Hora: " & TimeOfDay & " " & Today,
                                              FontTypeNames.FONTTYPE_INFO))
    End Sub

    ''
    ' Handles the "Where" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleWhere(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim tUser As Short
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()

                If Not .flags.Privilegios And PlayerType.User Then
                    tUser = NameIndex(UserName)
                    If tUser <= 0 Then
                        Call WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO)
                    Else
                        If _
                            (UserList(tUser).flags.Privilegios And
                             (PlayerType.User Or PlayerType.Consejero Or
                              PlayerType.SemiDios)) <> 0 Or
                            ((UserList(tUser).flags.Privilegios And
                              (PlayerType.Dios Or PlayerType.Admin) <> 0) And
                             (.flags.Privilegios And (PlayerType.Dios Or PlayerType.Admin)) <> 0) _
                            Then
                            Call _
                                WriteConsoleMsg(UserIndex,
                                                "Ubicación  " & UserName & ": " & UserList(tUser).Pos.Map & ", " &
                                                UserList(tUser).Pos.X & ", " & UserList(tUser).Pos.Y & ".",
                                                FontTypeNames.FONTTYPE_INFO)
                            Call LogGM(.name, "/Donde " & UserName)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleWhere: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "CreaturesInMap" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleCreaturesInMap(UserIndex As Short)
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

            If .flags.Privilegios And PlayerType.User Then Exit Sub

            If MapaValido(Map) Then
                For i = 1 To LastNPC
                    'VB isn't lazzy, so we put more restrictive condition first to speed up the process
                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    If Npclist(i).Pos.Map = Map Then
                        '¿esta vivo?
                        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        If Npclist(i).flags.NPCActive And Npclist(i).Hostile = 1 And Npclist(i).Stats.Alineacion = 2 _
                            Then
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

    Private Sub HandleWarpMeToTarget(UserIndex As Short)
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


            If .flags.Privilegios And PlayerType.User Then Exit Sub

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

    Private Sub HandleWarpChar(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 26/03/2009
        '26/03/2009: ZaMa -  Chequeo que no se teletransporte a un tile donde haya un char o npc.
        '***************************************************
        If UserList(UserIndex).incomingData.length < 7 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If Not .flags.Privilegios And PlayerType.User Then
                    If MapaValido(Map) And migr_LenB(UserName) <> 0 Then
                        If UCase(UserName) <> "YO" Then
                            If Not .flags.Privilegios And PlayerType.Consejero Then
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
                            Call _
                                WriteConsoleMsg(UserIndex, UserList(tUser).name & " transportado.",
                                                FontTypeNames.FONTTYPE_INFO)
                            Call _
                                LogGM(.name,
                                      "Transportó a " & UserList(tUser).name & " hacia " & "Mapa" & Map & " X:" & X &
                                      " Y:" &
                                      Y)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleWarpChar: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "Silence" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleSilence(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim tUser As Short
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()

                If Not .flags.Privilegios And PlayerType.User Then
                    tUser = NameIndex(UserName)

                    If tUser <= 0 Then
                        Call WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO)
                    Else
                        If UserList(tUser).flags.Silenciado = 0 Then
                            UserList(tUser).flags.Silenciado = 1
                            Call WriteConsoleMsg(UserIndex, "Usuario silenciado.", FontTypeNames.FONTTYPE_INFO)
                            Call _
                                WriteShowMessageBox(tUser,
                                                    "Estimado usuario, ud. ha sido silenciado por los administradores. Sus denuncias serán ignoradas por el servidor de aquí en más. Utilice /GM para contactar un administrador.")
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


        Catch ex As Exception
            Console.WriteLine("Error in HandleSilence: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "SOSShowList" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleSOSShowList(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If .flags.Privilegios And PlayerType.User Then Exit Sub
            Call WriteShowSOSForm(UserIndex)
        End With
    End Sub

    ''
    ' Handles the "RequestPartyForm" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandlePartyForm(UserIndex As Short)
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

    Private Sub HandleItemUpgrade(UserIndex As Short)
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

    Private Sub HandleSOSRemove(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()

                UserName = buffer.ReadASCIIString()

                If Not .flags.Privilegios And PlayerType.User Then Call Ayuda.Quitar(UserName)

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleSOSRemove: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "GoToChar" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGoToChar(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 26/03/2009
        '26/03/2009: ZaMa -  Chequeo que no se teletransporte a un tile donde haya un char o npc.
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If _
                    .flags.Privilegios And
                    (PlayerType.Dios Or PlayerType.Admin Or PlayerType.SemiDios Or
                     PlayerType.Consejero) Then
                    'Si es dios o Admins no podemos salvo que nosotros también lo seamos
                    If _
                        Not (EsDios(UserName) Or EsAdmin(UserName)) Or
                        (.flags.Privilegios And (PlayerType.Dios Or PlayerType.Admin)) <> 0 Then
                        If tUser <= 0 Then
                            Call WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO)
                        Else
                            X = UserList(tUser).Pos.X
                            Y = UserList(tUser).Pos.Y + 1
                            Call FindLegalPos(UserIndex, UserList(tUser).Pos.Map, X, Y)

                            Call WarpUserChar(UserIndex, UserList(tUser).Pos.Map, X, Y, True)

                            If .flags.AdminInvisible = 0 Then
                                Call _
                                    WriteConsoleMsg(tUser, .name & " se ha trasportado hacia donde te encuentras.",
                                                    FontTypeNames.FONTTYPE_INFO)
                                Call FlushBuffer(tUser)
                            End If

                            Call _
                                LogGM(.name,
                                      "/IRA " & UserName & " Mapa:" & UserList(tUser).Pos.Map & " X:" &
                                      UserList(tUser).Pos.X & " Y:" & UserList(tUser).Pos.Y)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGoToChar: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "Invisible" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleInvisible(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If .flags.Privilegios And PlayerType.User Then Exit Sub

            Call DoAdminInvisible(UserIndex)
            Call LogGM(.name, "/INVISIBLE")
        End With
    End Sub

    ''
    ' Handles the "GMPanel" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGMPanel(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If .flags.Privilegios And PlayerType.User Then Exit Sub

            Call WriteShowGMPanelForm(UserIndex)
        End With
    End Sub

    ''
    ' Handles the "GMPanel" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleRequestUserList(UserIndex As Short)
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

            If .flags.Privilegios And (PlayerType.User Or PlayerType.RoleMaster) Then _
                Exit Sub

            'UPGRADE_WARNING: El límite inferior de la matriz names ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim names(LastUser)
            Count = 1

            For i = 1 To LastUser
                If (migr_LenB(UserList(i).name) <> 0) Then
                    If UserList(i).flags.Privilegios And PlayerType.User Then
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

    Private Sub HandleWorking(UserIndex As Short)
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

            If .flags.Privilegios And (PlayerType.User Or PlayerType.RoleMaster) Then _
                Exit Sub

            For i = 1 To LastUser
                If UserList(i).flags.UserLogged And UserList(i).Counters.Trabajando > 0 Then
                    users = users & ", " & UserList(i).name

                    ' Display the user being checked by the centinel
                    If Centinela.RevisandoUserIndex = i Then users = users & " (*)"
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

    Private Sub HandleHiding(UserIndex As Short)
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

            If .flags.Privilegios And (PlayerType.User Or PlayerType.RoleMaster) Then _
                Exit Sub

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

    Private Sub HandleJail(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 6 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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
                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (Not .flags.Privilegios And PlayerType.User) <> 0 Then
                    If migr_LenB(UserName) = 0 Or migr_LenB(reason) = 0 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "Utilice /carcel nick@motivo@tiempo", FontTypeNames.FONTTYPE_INFO)
                    Else
                        tUser = NameIndex(UserName)

                        If tUser <= 0 Then
                            Call WriteConsoleMsg(UserIndex, "El usuario no está online.", FontTypeNames.FONTTYPE_INFO)
                        Else
                            If Not UserList(tUser).flags.Privilegios And PlayerType.User Then
                                Call _
                                    WriteConsoleMsg(UserIndex, "No puedes encarcelar a administradores.",
                                                    FontTypeNames.FONTTYPE_INFO)
                            ElseIf jailTime > 60 Then
                                Call _
                                    WriteConsoleMsg(UserIndex, "No puedés encarcelar por más de 60 minutos.",
                                                    FontTypeNames.FONTTYPE_INFO)
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
                                    Call _
                                        WriteVar(CharPath & UserName & ".chr", "PENAS", "P" & Count + 1,
                                                 LCase(.name) & ": CARCEL " & jailTime & "m, MOTIVO: " & LCase(reason) &
                                                 " " &
                                                 Today & " " & TimeOfDay)
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


        Catch ex As Exception
            Console.WriteLine("Error in HandleJail: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "KillNPC" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleKillNPC(UserIndex As Short)
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

            If .flags.Privilegios And PlayerType.User Then Exit Sub


            'Los consejeros no pueden RMATAr a nada en el mapa pretoriano
            If .flags.Privilegios And PlayerType.Consejero Then
                If .Pos.Map = MAPA_PRETORIANO Then
                    Call _
                        WriteConsoleMsg(UserIndex, "Los consejeros no pueden usar este comando en el mapa pretoriano.",
                                        FontTypeNames.FONTTYPE_INFO)
                    Exit Sub
                End If
            End If

            tNPC = .flags.TargetNPC

            If tNPC > 0 Then
                Call _
                    WriteConsoleMsg(UserIndex, "RMatas (con posible respawn) a: " & Npclist(tNPC).name,
                                    FontTypeNames.FONTTYPE_INFO)

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

    Private Sub HandleWarnUser(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/26/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 5 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim reason As String
            Dim privs As PlayerType
            Dim Count As Byte
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()
                reason = buffer.ReadASCIIString()

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (Not .flags.Privilegios And PlayerType.User) <> 0 Then
                    If migr_LenB(UserName) = 0 Or migr_LenB(reason) = 0 Then
                        Call WriteConsoleMsg(UserIndex, "Utilice /advertencia nick@motivo", FontTypeNames.FONTTYPE_INFO)
                    Else
                        privs = UserDarPrivilegioLevel(UserName)

                        If Not privs And PlayerType.User Then
                            Call _
                                WriteConsoleMsg(UserIndex, "No puedes advertir a administradores.",
                                                FontTypeNames.FONTTYPE_INFO)
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
                                Call _
                                    WriteVar(CharPath & UserName & ".chr", "PENAS", "P" & Count + 1,
                                             LCase(.name) & ": ADVERTENCIA por: " & LCase(reason) & " " & Today & " " &
                                             TimeOfDay)

                                Call _
                                    WriteConsoleMsg(UserIndex, "Has advertido a " & UCase(UserName) & ".",
                                                    FontTypeNames.FONTTYPE_INFO)
                                Call LogGM(.name, " advirtio a " & UserName)
                            End If
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleWarnUser: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "EditChar" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleEditChar(UserIndex As Short)
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

        Try
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

                If .flags.Privilegios And PlayerType.RoleMaster Then
                    Select Case _
                        .flags.Privilegios And
                        (PlayerType.Dios Or PlayerType.SemiDios Or
                         PlayerType.Consejero)
                        Case PlayerType.Consejero
                            ' Los RMs consejeros sólo se pueden editar su head, body y level
                            valido = tUser = UserIndex And
                                     (opcion = eEditOptions.eo_Body Or opcion = eEditOptions.eo_Head Or
                                      opcion = eEditOptions.eo_Level)

                        Case PlayerType.SemiDios
                            ' Los RMs sólo se pueden editar su level y el head y body de cualquiera
                            valido = (opcion = eEditOptions.eo_Level And tUser = UserIndex) Or
                                     opcion = eEditOptions.eo_Body Or
                                     opcion = eEditOptions.eo_Head

                        Case PlayerType.Dios
                            ' Los DRMs pueden aplicar los siguientes comandos sobre cualquiera
                            ' pero si quiere modificar el level sólo lo puede hacer sobre sí mismo
                            valido = (opcion = eEditOptions.eo_Level And tUser = UserIndex) Or
                                     opcion = eEditOptions.eo_Body Or
                                     opcion = eEditOptions.eo_Head Or opcion = eEditOptions.eo_CiticensKilled Or
                                     opcion = eEditOptions.eo_CriminalsKilled Or opcion = eEditOptions.eo_Class Or
                                     opcion = eEditOptions.eo_Skills Or opcion = eEditOptions.eo_addGold
                    End Select

                ElseIf .flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios) Then _
'Si no es RM debe ser dios para poder usar este comando
                    valido = True
                End If

                If valido Then
                    UserCharPath = CharPath & UserName & ".chr"
                    If tUser <= 0 And Not FileExist(UserCharPath) Then
                        Call _
                            WriteConsoleMsg(UserIndex, "Estás intentando editar un usuario inexistente.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Call LogGM(.name, "Intentó editar un usuario inexistente.")
                    Else
                        'For making the Log
                        CommandString = "/MOD "

                        Select Case opcion
                            Case eEditOptions.eo_Gold
                                If Val(Arg1) <= MAX_ORO_EDIT Then
                                    If tUser <= 0 Then ' Esta offline?
                                        Call WriteVar(UserCharPath, "STATS", "GLD", CStr(Val(Arg1)))
                                        Call _
                                            WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName,
                                                            FontTypeNames.FONTTYPE_INFO)
                                    Else ' Online
                                        UserList(tUser).Stats.GLD = Val(Arg1)
                                        Call WriteUpdateGold(tUser)
                                    End If
                                Else
                                    Call _
                                        WriteConsoleMsg(UserIndex,
                                                        "No está permitido utilizar valores mayores a " & MAX_ORO_EDIT &
                                                        ". Su comando ha quedado en los logs del juego.",
                                                        FontTypeNames.FONTTYPE_INFO)
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
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName,
                                                        FontTypeNames.FONTTYPE_INFO)
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
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName,
                                                        FontTypeNames.FONTTYPE_INFO)
                                Else
                                    Call _
                                        ChangeUserChar(tUser, Val(Arg1), UserList(tUser).Char_Renamed.Head,
                                                       UserList(tUser).Char_Renamed.heading,
                                                       UserList(tUser).Char_Renamed.WeaponAnim,
                                                       UserList(tUser).Char_Renamed.ShieldAnim,
                                                       UserList(tUser).Char_Renamed.CascoAnim)
                                End If

                                ' Log it
                                CommandString = CommandString & "BODY "

                            Case eEditOptions.eo_Head
                                If tUser <= 0 Then
                                    Call WriteVar(UserCharPath, "INIT", "Head", Arg1)
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName,
                                                        FontTypeNames.FONTTYPE_INFO)
                                Else
                                    Call _
                                        ChangeUserChar(tUser, UserList(tUser).Char_Renamed.body, Val(Arg1),
                                                       UserList(tUser).Char_Renamed.heading,
                                                       UserList(tUser).Char_Renamed.WeaponAnim,
                                                       UserList(tUser).Char_Renamed.ShieldAnim,
                                                       UserList(tUser).Char_Renamed.CascoAnim)
                                End If

                                ' Log it
                                CommandString = CommandString & "HEAD "

                            Case eEditOptions.eo_CriminalsKilled
                                Var = IIf(Val(Arg1) > MAXUSERMATADOS, MAXUSERMATADOS, Val(Arg1))

                                If tUser <= 0 Then ' Offline
                                    Call WriteVar(UserCharPath, "FACCIONES", "CrimMatados", CStr(Var))
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName,
                                                        FontTypeNames.FONTTYPE_INFO)
                                Else ' Online
                                    UserList(tUser).Faccion.CriminalesMatados = Var
                                End If

                                ' Log it
                                CommandString = CommandString & "CRI "

                            Case eEditOptions.eo_CiticensKilled
                                Var = IIf(Val(Arg1) > MAXUSERMATADOS, MAXUSERMATADOS, Val(Arg1))

                                If tUser <= 0 Then ' Offline
                                    Call WriteVar(UserCharPath, "FACCIONES", "CiudMatados", CStr(Var))
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName,
                                                        FontTypeNames.FONTTYPE_INFO)
                                Else ' Online
                                    UserList(tUser).Faccion.CiudadanosMatados = Var
                                End If

                                ' Log it
                                CommandString = CommandString & "CIU "

                            Case eEditOptions.eo_Level
                                If Val(Arg1) > STAT_MAXELV Then
                                    Arg1 = CStr(STAT_MAXELV)
                                    Call _
                                        WriteConsoleMsg(UserIndex,
                                                        "No puedes tener un nivel superior a " & STAT_MAXELV & ".",
                                                        FontTypeNames.FONTTYPE_INFO)
                                End If

                                ' Chequeamos si puede permanecer en el clan
                                If Val(Arg1) >= 25 Then

                                    If tUser <= 0 Then
                                        GI = CShort(GetVar(UserCharPath, "GUILD", "GUILDINDEX"))
                                    Else
                                        GI = UserList(tUser).GuildIndex
                                    End If

                                    If GI > 0 Then
                                        If GuildAlignment(GI) = "Del Mal" Or GuildAlignment(GI) = "Real" _
                                            Then
                                            'We get here, so guild has factionary alignment, we have to expulse the user
                                            Call m_EcharMiembroDeClan(- 1, UserName)

                                            Call _
                                                SendData(SendTarget.ToGuildMembers, GI,
                                                         PrepareMessageConsoleMsg(UserName & " deja el clan.",
                                                                                  FontTypeNames.FONTTYPE_GUILD))
                                            ' Si esta online le avisamos
                                            If tUser > 0 Then _
                                                Call _
                                                    WriteConsoleMsg(tUser,
                                                                    "¡Ya tienes la madurez suficiente como para decidir bajo que estandarte pelearás! Por esta razón, hasta tanto no te enlistes en la facción bajo la cual tu clan está alineado, estarás excluído del mismo.",
                                                                    FontTypeNames.FONTTYPE_GUILD)
                                        End If
                                    End If
                                End If

                                If tUser <= 0 Then ' Offline
                                    Call WriteVar(UserCharPath, "STATS", "ELV", CStr(Val(Arg1)))
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName,
                                                        FontTypeNames.FONTTYPE_INFO)
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
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Clase desconocida. Intente nuevamente.",
                                                        FontTypeNames.FONTTYPE_INFO)
                                Else
                                    If tUser <= 0 Then ' Offline
                                        Call WriteVar(UserCharPath, "INIT", "Clase", CStr(LoopC))
                                        Call _
                                            WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName,
                                                            FontTypeNames.FONTTYPE_INFO)
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
                                            Call _
                                                WriteVar(UserCharPath, "Skills", "ELUSK" & LoopC,
                                                         CStr(ELU_SKILL_INICIAL*1.05^CDbl(Arg2)))
                                        Else
                                            Call WriteVar(UserCharPath, "Skills", "ELUSK" & LoopC, CStr(0))
                                        End If

                                        Call _
                                            WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName,
                                                            FontTypeNames.FONTTYPE_INFO)
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
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName,
                                                        FontTypeNames.FONTTYPE_INFO)
                                Else ' Online
                                    UserList(tUser).Stats.SkillPts = Val(Arg1)
                                End If

                                ' Log it
                                CommandString = CommandString & "SKILLSLIBRES "

                            Case eEditOptions.eo_Nobleza
                                Var = IIf(Val(Arg1) > MAXREP, MAXREP, Val(Arg1))

                                If tUser <= 0 Then ' Offline
                                    Call WriteVar(UserCharPath, "REP", "Nobles", CStr(Var))
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName,
                                                        FontTypeNames.FONTTYPE_INFO)
                                Else ' Online
                                    UserList(tUser).Reputacion.NobleRep = Var
                                End If

                                ' Log it
                                CommandString = CommandString & "NOB "

                            Case eEditOptions.eo_Asesino
                                Var = IIf(Val(Arg1) > MAXREP, MAXREP, Val(Arg1))

                                If tUser <= 0 Then ' Offline
                                    Call WriteVar(UserCharPath, "REP", "Asesino", CStr(Var))
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName,
                                                        FontTypeNames.FONTTYPE_INFO)
                                Else ' Online
                                    UserList(tUser).Reputacion.AsesinoRep = Var
                                End If

                                ' Log it
                                CommandString = CommandString & "ASE "

                            Case eEditOptions.eo_Sex
                                Sex = IIf(UCase(Arg1) = "MUJER", eGenero.Mujer, 0) ' Mujer?
                                Sex = IIf(UCase(Arg1) = "HOMBRE", eGenero.Hombre, Sex) ' Hombre?

                                If Sex <> 0 Then ' Es Hombre o mujer?
                                    If tUser <= 0 Then ' OffLine
                                        Call WriteVar(UserCharPath, "INIT", "Genero", CStr(Sex))
                                        Call _
                                            WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName,
                                                            FontTypeNames.FONTTYPE_INFO)
                                    Else ' Online
                                        UserList(tUser).Genero = Sex
                                    End If
                                Else
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Genero desconocido. Intente nuevamente.",
                                                        FontTypeNames.FONTTYPE_INFO)
                                End If

                                ' Log it
                                CommandString = CommandString & "SEX "

                            Case eEditOptions.eo_Raza

                                Arg1 = UCase(Arg1)
                                Select Case Arg1
                                    Case "HUMANO"
                                        raza = eRaza.Humano
                                    Case "ELFO"
                                        raza = eRaza.Elfo
                                    Case "DROW"
                                        raza = eRaza.Drow
                                    Case "ENANO"
                                        raza = eRaza.Enano
                                    Case "GNOMO"
                                        raza = eRaza.Gnomo
                                    Case Else
                                        raza = 0
                                End Select


                                If raza = 0 Then
                                    Call _
                                        WriteConsoleMsg(UserIndex, "Raza desconocida. Intente nuevamente.",
                                                        FontTypeNames.FONTTYPE_INFO)
                                Else
                                    If tUser <= 0 Then
                                        Call WriteVar(UserCharPath, "INIT", "Raza", CStr(raza))
                                        Call _
                                            WriteConsoleMsg(UserIndex, "Charfile Alterado: " & UserName,
                                                            FontTypeNames.FONTTYPE_INFO)
                                    Else
                                        UserList(tUser).raza = raza
                                    End If
                                End If

                                ' Log it
                                CommandString = CommandString & "RAZA "

                            Case eEditOptions.eo_addGold


                                If Math.Abs(CDbl(Arg1)) > MAX_ORO_EDIT Then
                                    Call _
                                        WriteConsoleMsg(UserIndex,
                                                        "No está permitido utilizar valores mayores a " & MAX_ORO_EDIT &
                                                        ".",
                                                        FontTypeNames.FONTTYPE_INFO)
                                Else
                                    If tUser <= 0 Then
                                        bankGold = CInt(GetVar(CharPath & UserName & ".chr", "STATS", "BANCO"))
                                        Call _
                                            WriteVar(UserCharPath, "STATS", "BANCO",
                                                     IIf(bankGold + Val(Arg1) <= 0, 0, bankGold + Val(Arg1)))
                                        Call _
                                            WriteConsoleMsg(UserIndex,
                                                            "Se le ha agregado " & Arg1 & " monedas de oro a " &
                                                            UserName &
                                                            ".", FontTypeNames.FONTTYPE_TALK)
                                    Else
                                        UserList(tUser).Stats.Banco = IIf(UserList(tUser).Stats.Banco + Val(Arg1) <= 0,
                                                                          0,
                                                                          UserList(tUser).Stats.Banco + Val(Arg1))
                                        Call _
                                            WriteConsoleMsg(tUser, STANDARD_BOUNTY_HUNTER_MESSAGE,
                                                            FontTypeNames.FONTTYPE_TALK)
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


        Catch ex As Exception
            Console.WriteLine("Error in HandleEditChar: " & ex.Message)
        End Try
    End Sub


    ''
    ' Handles the "RequestCharInfo" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleRequestCharInfo(UserIndex As Short)
        '***************************************************
        'Author: Fredy Horacio Treboux (liquid)
        'Last Modification: 01/08/07
        'Last Modification by: (liquid).. alto bug zapallo..
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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


                If _
                    .flags.Privilegios And
                    (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios) _
                    Then
                    'is the player offline?
                    If TargetIndex <= 0 Then
                        'don't allow to retrieve administrator's info
                        If Not (EsDios(targetName) Or EsAdmin(targetName)) Then
                            Call _
                                WriteConsoleMsg(UserIndex, "Usuario offline, buscando en charfile.",
                                                FontTypeNames.FONTTYPE_INFO)
                            Call SendUserStatsTxtOFF(UserIndex, targetName)
                        End If
                    Else
                        'don't allow to retrieve administrator's info
                        If _
                            UserList(TargetIndex).flags.Privilegios And
                            (PlayerType.User Or PlayerType.Consejero Or
                             PlayerType.SemiDios) Then
                            Call SendUserStatsTxt(UserIndex, TargetIndex)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleRequestCharInfo: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "RequestCharStats" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleRequestCharStats(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim tUser As Short
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()

                UserName = buffer.ReadASCIIString()

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) <>
                    0 Then
                    Call LogGM(.name, "/STAT " & UserName)

                    tUser = NameIndex(UserName)

                    If tUser <= 0 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "Usuario offline. Leyendo charfile... ",
                                            FontTypeNames.FONTTYPE_INFO)

                        Call SendUserMiniStatsTxtFromChar(UserIndex, UserName)
                    Else
                        Call SendUserMiniStatsTxt(UserIndex, tUser)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleRequestCharStats: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "RequestCharGold" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleRequestCharGold(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If _
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) _
                    Then
                    Call LogGM(.name, "/BAL " & UserName)

                    If tUser <= 0 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "Usuario offline. Leyendo charfile... ",
                                            FontTypeNames.FONTTYPE_TALK)

                        Call SendUserOROTxtFromChar(UserIndex, UserName)
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "El usuario " & UserName & " tiene " & UserList(tUser).Stats.Banco &
                                            " en el banco.", FontTypeNames.FONTTYPE_TALK)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleRequestCharGold: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "RequestCharInventory" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleRequestCharInventory(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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


                If _
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) _
                    Then
                    Call LogGM(.name, "/INV " & UserName)

                    If tUser <= 0 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "Usuario offline. Leyendo del charfile...",
                                            FontTypeNames.FONTTYPE_TALK)

                        Call SendUserInvTxtFromChar(UserIndex, UserName)
                    Else
                        Call SendUserInvTxt(UserIndex, tUser)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleRequestCharInventory: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "RequestCharBank" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleRequestCharBank(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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


                If _
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) _
                    Then
                    Call LogGM(.name, "/BOV " & UserName)

                    If tUser <= 0 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "Usuario offline. Leyendo charfile... ",
                                            FontTypeNames.FONTTYPE_TALK)

                        Call SendUserBovedaTxtFromChar(UserIndex, UserName)
                    Else
                        Call SendUserBovedaTxt(UserIndex, tUser)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleRequestCharBank: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "RequestCharSkills" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleRequestCharSkills(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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


                If _
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) _
                    Then
                    Call LogGM(.name, "/STATS " & UserName)

                    If tUser <= 0 Then
                        If (migr_InStrB(UserName, "\") <> 0) Then
                            UserName = Replace(UserName, "\", "")
                        End If
                        If (migr_InStrB(UserName, "/") <> 0) Then
                            UserName = Replace(UserName, "/", "")
                        End If

                        For LoopC = 1 To NUMSKILLS
                            message = message & "CHAR>" & SkillsNames(LoopC) & " = " &
                                      GetVar(CharPath & UserName & ".chr", "SKILLS", "SK" & LoopC) & vbCrLf
                        Next LoopC

                        Call _
                            WriteConsoleMsg(UserIndex,
                                            message & "CHAR> Libres:" &
                                            GetVar(CharPath & UserName & ".chr", "STATS", "SKILLPTSLIBRES"),
                                            FontTypeNames.FONTTYPE_INFO)
                    Else
                        Call SendUserSkillsTxt(UserIndex, tUser)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleRequestCharSkills: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "ReviveChar" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleReviveChar(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 11/03/2010
        '11/03/2010: ZaMa - Al revivir con el comando, si esta navegando le da cuerpo e barca.
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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


                If _
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) _
                    Then
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
                                    Call WriteMultiMessage(tUser, eMessages.CancelHome)
                                End If

                                Call _
                                    ChangeUserChar(tUser, .Char_Renamed.body, .OrigChar.Head, .Char_Renamed.heading,
                                                   .Char_Renamed.WeaponAnim, .Char_Renamed.ShieldAnim,
                                                   .Char_Renamed.CascoAnim)

                                Call _
                                    WriteConsoleMsg(tUser, UserList(UserIndex).name & " te ha resucitado.",
                                                    FontTypeNames.FONTTYPE_INFO)
                            Else
                                Call _
                                    WriteConsoleMsg(tUser, UserList(UserIndex).name & " te ha curado.",
                                                    FontTypeNames.FONTTYPE_INFO)
                            End If

                            .Stats.MinHp = .Stats.MaxHp

                            If .flags.Traveling = 1 Then
                                .Counters.goHome = 0
                                .flags.Traveling = 0
                                Call WriteMultiMessage(tUser, eMessages.CancelHome)
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


        Catch ex As Exception
            Console.WriteLine("Error in HandleReviveChar: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "OnlineGM" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleOnlineGM(UserIndex As Short)
        '***************************************************
        'Author: Fredy Horacio Treboux (liquid)
        'Last Modification: 12/28/06
        '
        '***************************************************
        Dim i As Integer
        Dim list As String
        Dim priv As PlayerType

        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If .flags.Privilegios And (PlayerType.User Or PlayerType.Consejero) Then _
                Exit Sub

            priv = PlayerType.Consejero Or PlayerType.SemiDios
            If .flags.Privilegios And (PlayerType.Dios Or PlayerType.Admin) Then _
                priv = priv Or PlayerType.Dios Or PlayerType.Admin

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

    Private Sub HandleOnlineMap(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 23/03/2009
        '23/03/2009: ZaMa - Ahora no requiere estar en el mapa, sino que por defecto se toma en el que esta, pero se puede especificar otro
        '***************************************************
        Dim Map As Short
        Dim LoopC As Integer
        Dim list As String
        Dim priv As PlayerType
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            Map = .incomingData.ReadInteger

            If .flags.Privilegios And (PlayerType.User Or PlayerType.Consejero) Then _
                Exit Sub


            priv = PlayerType.User Or PlayerType.Consejero Or
                   PlayerType.SemiDios
            If .flags.Privilegios And (PlayerType.Dios Or PlayerType.Admin) Then _
                priv = priv + CShort(PlayerType.Dios Or PlayerType.Admin)

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

    Private Sub HandleForgive(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim tUser As Short
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) <>
                    0 Then
                    tUser = NameIndex(UserName)

                    If tUser > 0 Then
                        If EsNewbie(tUser) Then
                            Call VolverCiudadano(tUser)
                        Else
                            Call LogGM(.name, "Intento perdonar un personaje de nivel avanzado.")
                            Call _
                                WriteConsoleMsg(UserIndex, "Sólo se permite perdonar newbies.",
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleForgive: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "Kick" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleKick(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim tUser As Short
            Dim rank As Short
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                rank = PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios Or
                       PlayerType.Consejero

                UserName = buffer.ReadASCIIString()

                If _
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) _
                    Then
                    tUser = NameIndex(UserName)

                    If tUser <= 0 Then
                        Call WriteConsoleMsg(UserIndex, "El usuario no está online.", FontTypeNames.FONTTYPE_INFO)
                    Else
                        If (UserList(tUser).flags.Privilegios And rank) > (.flags.Privilegios And rank) Then
                            Call _
                                WriteConsoleMsg(UserIndex, "No puedes echar a alguien con jerarquía mayor a la tuya.",
                                                FontTypeNames.FONTTYPE_INFO)
                        Else
                            Call _
                                SendData(SendTarget.ToAll, 0,
                                         PrepareMessageConsoleMsg(.name & " echó a " & UserName & ".",
                                                                  FontTypeNames.FONTTYPE_INFO))
                            Call CloseSocket(tUser)
                            Call LogGM(.name, "Echó a " & UserName)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleKick: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "Execute" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleExecute(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim tUser As Short
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) <>
                    0 Then
                    tUser = NameIndex(UserName)

                    If tUser > 0 Then
                        If Not UserList(tUser).flags.Privilegios And PlayerType.User Then
                            Call _
                                WriteConsoleMsg(UserIndex, "¿¿Estás loco?? ¿¿Cómo vas a piñatear un gm?? :@",
                                                FontTypeNames.FONTTYPE_INFO)
                        Else
                            Call UserDie(tUser)
                            Call _
                                SendData(SendTarget.ToAll, 0,
                                         PrepareMessageConsoleMsg(.name & " ha ejecutado a " & UserName & ".",
                                                                  FontTypeNames.FONTTYPE_EJECUCION))
                            Call LogGM(.name, " ejecuto a " & UserName)
                        End If
                    Else
                        Call WriteConsoleMsg(UserIndex, "No está online.", FontTypeNames.FONTTYPE_INFO)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleExecute: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "BanChar" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleBanChar(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 5 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) <>
                    0 Then
                    Call BanCharacter(UserIndex, UserName, reason)
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleBanChar: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "UnbanChar" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleUnbanChar(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim cantPenas As Byte
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) <>
                    0 Then
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
                            Call _
                                WriteVar(CharPath & UserName & ".chr", "PENAS", "P" & cantPenas + 1,
                                         LCase(.name) & ": UNBAN. " & Today & " " & TimeOfDay)

                            Call LogGM(.name, "/UNBAN a " & UserName)
                            Call WriteConsoleMsg(UserIndex, UserName & " unbanned.", FontTypeNames.FONTTYPE_INFO)
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex, UserName & " no está baneado. Imposible unbanear.",
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleUnbanChar: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "NPCFollow" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleNPCFollow(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If .flags.Privilegios And (PlayerType.User Or PlayerType.Consejero) Then _
                Exit Sub

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

    Private Sub HandleSummonChar(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 26/03/2009
        '26/03/2009: ZaMa - Chequeo que no se teletransporte donde haya un char o npc
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If _
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) _
                    Then
                    tUser = NameIndex(UserName)

                    If tUser <= 0 Then
                        Call WriteConsoleMsg(UserIndex, "El jugador no está online.", FontTypeNames.FONTTYPE_INFO)
                    Else
                        If _
                            (.flags.Privilegios And (PlayerType.Dios Or PlayerType.Admin)) <> 0 Or
                            (UserList(tUser).flags.Privilegios And
                             (PlayerType.Consejero Or PlayerType.User)) <> 0 Then
                            Call WriteConsoleMsg(tUser, .name & " te ha trasportado.", FontTypeNames.FONTTYPE_INFO)
                            X = .Pos.X
                            Y = .Pos.Y + 1
                            Call FindLegalPos(tUser, .Pos.Map, X, Y)
                            Call WarpUserChar(tUser, .Pos.Map, X, Y, True, True)
                            Call LogGM(.name, "/SUM " & UserName & " Map:" & .Pos.Map & " X:" & .Pos.X & " Y:" & .Pos.Y)
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex, "No puedes invocar a dioses y admins.",
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleSummonChar: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "SpawnListRequest" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleSpawnListRequest(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If .flags.Privilegios And (PlayerType.User Or PlayerType.Consejero) Then _
                Exit Sub

            Call EnviarSpawnList(UserIndex)
        End With
    End Sub

    ''
    ' Handles the "SpawnCreature" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleSpawnCreature(UserIndex As Short)
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

            If _
                (.flags.Privilegios And
                 (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) _
                Then
                If npc_Renamed > 0 And npc_Renamed <= UBound(SpawnList) Then _
                    Call SpawnNpc(SpawnList(npc_Renamed).NpcIndex, .Pos, True, False)

                Call LogGM(.name, "Sumoneo " & SpawnList(npc_Renamed).NpcName)
            End If
        End With
    End Sub

    ''
    ' Handles the "ResetNPCInventory" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleResetNPCInventory(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.RoleMaster) Then Exit Sub
            If .flags.TargetNPC = 0 Then Exit Sub

            Call ResetNpcInv(.flags.TargetNPC)
            Call LogGM(.name, "/RESETINV " & Npclist(.flags.TargetNPC).name)
        End With
    End Sub

    ''
    ' Handles the "CleanWorld" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleCleanWorld(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.RoleMaster) Then Exit Sub

            Call LimpiarMundo()
        End With
    End Sub

    ''
    ' Handles the "ServerMessage" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleServerMessage(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim message As String
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()

                message = buffer.ReadASCIIString()

                If _
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) _
                    Then
                    If migr_LenB(message) <> 0 Then
                        Call LogGM(.name, "Mensaje Broadcast:" & message)
                        Call _
                            SendData(SendTarget.ToAll, 0,
                                     PrepareMessageConsoleMsg(UserList(UserIndex).name & "> " & message,
                                                              FontTypeNames.FONTTYPE_TALK))
                        ''''''''''''''''SOLO PARA EL TESTEO'''''''
                        ''''''''''SE USA PARA COMUNICARSE CON EL SERVER'''''''''''
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleServerMessage: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "NickToIP" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleNickToIP(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 24/07/07
        'Pablo (ToxicWaste): Agrego para uqe el /nick2ip tambien diga los nicks en esa ip por pedido de la DGM.
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim tUser As Short
            Dim priv As PlayerType
            Dim ip As String
            Dim lista As String
            Dim LoopC As Integer
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) <>
                    0 Then
                    tUser = NameIndex(UserName)
                    Call LogGM(.name, "NICK2IP Solicito la IP de " & UserName)

                    If .flags.Privilegios And (PlayerType.Dios Or PlayerType.Admin) Then
                        priv = PlayerType.User Or PlayerType.Consejero Or
                               PlayerType.SemiDios Or PlayerType.Dios Or
                               PlayerType.Admin
                    Else
                        priv = PlayerType.User
                    End If

                    If tUser > 0 Then
                        If UserList(tUser).flags.Privilegios And priv Then
                            Call _
                                WriteConsoleMsg(UserIndex, "El ip de " & UserName & " es " & UserList(tUser).ip,
                                                FontTypeNames.FONTTYPE_INFO)
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
                            Call _
                                WriteConsoleMsg(UserIndex, "Los personajes con ip " & ip & " son: " & lista,
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, "No hay ningún personaje con ese nick.",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleNickToIP: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "IPToNick" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleIPToNick(UserIndex As Short)
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
        Dim priv As PlayerType
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()


            ip = .incomingData.ReadByte() & "."
            ip = ip & .incomingData.ReadByte() & "."
            ip = ip & .incomingData.ReadByte() & "."
            ip = ip & .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.RoleMaster) Then Exit Sub

            Call LogGM(.name, "IP2NICK Solicito los Nicks de IP " & ip)

            If .flags.Privilegios And (PlayerType.Dios Or PlayerType.Admin) Then
                priv = PlayerType.User Or PlayerType.Consejero Or
                       PlayerType.SemiDios Or PlayerType.Dios Or
                       PlayerType.Admin
            Else
                priv = PlayerType.User
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
            Call _
                WriteConsoleMsg(UserIndex, "Los personajes con ip " & ip & " son: " & lista, FontTypeNames.FONTTYPE_INFO)
        End With
    End Sub

    ''
    ' Handles the "GuildOnlineMembers" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildOnlineMembers(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) <>
                    0 Then
                    tGuild = GuildIndex(GuildName)

                    If tGuild > 0 Then
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "Clan " & UCase(GuildName) & ": " &
                                            m_ListaDeMiembrosOnline(UserIndex, tGuild),
                                            FontTypeNames.FONTTYPE_GUILDMSG)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildOnlineMembers: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "TeleportCreate" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleTeleportCreate(UserIndex As Short)
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

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub

            Call LogGM(.name, "/CT " & mapa & "," & X & "," & Y & "," & Radio)

            If Not MapaValido(mapa) Or Not InMapBounds(mapa, X, Y) Then Exit Sub

            If MapData(.Pos.Map, .Pos.X, .Pos.Y - 1).ObjInfo.ObjIndex > 0 Then Exit Sub

            If MapData(.Pos.Map, .Pos.X, .Pos.Y - 1).TileExit.Map > 0 Then Exit Sub

            If MapData(mapa, X, Y).ObjInfo.ObjIndex > 0 Then
                Call WriteConsoleMsg(UserIndex, "Hay un objeto en el piso en ese lugar.", FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            If MapData(mapa, X, Y).TileExit.Map > 0 Then
                Call _
                    WriteConsoleMsg(UserIndex, "No puedes crear un teleport que apunte a la entrada de otro.",
                                    FontTypeNames.FONTTYPE_INFO)
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

    Private Sub HandleTeleportDestroy(UserIndex As Short)
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
            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub

            mapa = .flags.TargetMap
            X = .flags.TargetX
            Y = .flags.TargetY

            If Not InMapBounds(mapa, X, Y) Then Exit Sub

            With MapData(mapa, X, Y)
                If .ObjInfo.ObjIndex = 0 Then Exit Sub

                If ObjData_Renamed(.ObjInfo.ObjIndex).OBJType = eOBJType.otTeleport And .TileExit.Map > 0 _
                    Then
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

    Private Sub HandleRainToggle(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If .flags.Privilegios And (PlayerType.User Or PlayerType.Consejero) Then _
                Exit Sub

            Call LogGM(.name, "/LLUVIA")
            Lloviendo = Not Lloviendo

            Call SendData(SendTarget.ToAll, 0, PrepareMessageRainToggle())
        End With
    End Sub

    ''
    ' Handles the "SetCharDescription" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleSetCharDescription(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim tUser As Short
            Dim desc As String
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                desc = buffer.ReadASCIIString()

                If _
                    (.flags.Privilegios And (PlayerType.Dios Or PlayerType.Admin)) <> 0 Or
                    (.flags.Privilegios And PlayerType.RoleMaster) <> 0 Then
                    tUser = .flags.TargetUser
                    If tUser > 0 Then
                        UserList(tUser).DescRM = desc
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, "Haz click sobre un personaje antes.",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleSetCharDescription: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "ForceMIDIToMap" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HanldeForceMIDIToMap(UserIndex As Short)
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
            If _
                .flags.Privilegios And
                (PlayerType.Dios Or PlayerType.Admin Or PlayerType.RoleMaster) _
                Then
                'Si el mapa no fue enviado tomo el actual
                If Not InMapBounds(mapa, 50, 50) Then
                    mapa = .Pos.Map
                End If

                If midiID = 0 Then
                    'Ponemos el default del mapa
                    Call _
                        SendData(SendTarget.toMap, mapa,
                                 PrepareMessagePlayMidi(CByte(MapInfo_Renamed(.Pos.Map).Music)))
                Else
                    'Ponemos el pedido por el GM
                    Call SendData(SendTarget.toMap, mapa, PrepareMessagePlayMidi(midiID))
                End If
            End If
        End With
    End Sub

    ''
    ' Handles the "ForceWAVEToMap" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleForceWAVEToMap(UserIndex As Short)
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
            If _
                .flags.Privilegios And
                (PlayerType.Dios Or PlayerType.Admin Or PlayerType.RoleMaster) _
                Then
                'Si el mapa no fue enviado tomo el actual
                If Not InMapBounds(mapa, X, Y) Then
                    mapa = .Pos.Map
                    X = .Pos.X
                    Y = .Pos.Y
                End If

                'Ponemos el pedido por el GM
                Call SendData(SendTarget.toMap, mapa, PrepareMessagePlayWave(waveID, X, Y))
            End If
        End With
    End Sub

    ''
    ' Handles the "RoyalArmyMessage" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleRoyalArmyMessage(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim message As String
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()

                message = buffer.ReadASCIIString()

                'Solo dioses, admins y RMS
                If _
                    .flags.Privilegios And
                    (PlayerType.Dios Or PlayerType.Admin Or PlayerType.RoleMaster) _
                    Then
                    Call _
                        SendData(SendTarget.ToRealYRMs, 0,
                                 PrepareMessageConsoleMsg("EJÉRCITO REAL> " & message, FontTypeNames.FONTTYPE_TALK))
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleRoyalArmyMessage: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "ChaosLegionMessage" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleChaosLegionMessage(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim message As String
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()

                message = buffer.ReadASCIIString()

                'Solo dioses, admins y RMS
                If _
                    .flags.Privilegios And
                    (PlayerType.Dios Or PlayerType.Admin Or PlayerType.RoleMaster) _
                    Then
                    Call _
                        SendData(SendTarget.ToCaosYRMs, 0,
                                 PrepareMessageConsoleMsg("FUERZAS DEL CAOS> " & message, FontTypeNames.FONTTYPE_TALK))
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleChaosLegionMessage: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "CitizenMessage" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleCitizenMessage(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim message As String
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()

                message = buffer.ReadASCIIString()

                'Solo dioses, admins y RMS
                If _
                    .flags.Privilegios And
                    (PlayerType.Dios Or PlayerType.Admin Or PlayerType.RoleMaster) _
                    Then
                    Call _
                        SendData(SendTarget.ToCiudadanosYRMs, 0,
                                 PrepareMessageConsoleMsg("CIUDADANOS> " & message, FontTypeNames.FONTTYPE_TALK))
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleCitizenMessage: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "CriminalMessage" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleCriminalMessage(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim message As String
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()

                message = buffer.ReadASCIIString()

                'Solo dioses, admins y RMS
                If _
                    .flags.Privilegios And
                    (PlayerType.Dios Or PlayerType.Admin Or PlayerType.RoleMaster) _
                    Then
                    Call _
                        SendData(SendTarget.ToCriminalesYRMs, 0,
                                 PrepareMessageConsoleMsg("CRIMINALES> " & message, FontTypeNames.FONTTYPE_TALK))
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleCriminalMessage: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "TalkAsNPC" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleTalkAsNPC(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/29/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim message As String
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()

                message = buffer.ReadASCIIString()

                'Solo dioses, admins y RMS
                If _
                    .flags.Privilegios And
                    (PlayerType.Dios Or PlayerType.Admin Or PlayerType.RoleMaster) _
                    Then
                    'Asegurarse haya un NPC seleccionado
                    If .flags.TargetNPC > 0 Then
                        Call _
                            SendData(SendTarget.ToNPCArea, .flags.TargetNPC,
                                     PrepareMessageChatOverHead(message,
                                                                Npclist(.flags.TargetNPC).Char_Renamed.CharIndex,
                                                                ColorTranslator.ToOle(
                                                                    Color.White)))
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "Debes seleccionar el NPC por el que quieres hablar antes de usar este comando.",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleTalkAsNPC: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "DestroyAllItemsInArea" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleDestroyAllItemsInArea(UserIndex As Short)
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

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub


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

    Private Sub HandleAcceptRoyalCouncilMember(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/30/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) Then
                    tUser = NameIndex(UserName)
                    If tUser <= 0 Then
                        Call WriteConsoleMsg(UserIndex, "Usuario offline", FontTypeNames.FONTTYPE_INFO)
                    Else
                        Call _
                            SendData(SendTarget.ToAll, 0,
                                     PrepareMessageConsoleMsg(
                                         UserName & " fue aceptado en el honorable Consejo Real de Banderbill.",
                                         FontTypeNames.FONTTYPE_CONSEJO))
                        With UserList(tUser)
                            If .flags.Privilegios And PlayerType.ChaosCouncil Then _
                                .flags.Privilegios = .flags.Privilegios - PlayerType.ChaosCouncil
                            If Not .flags.Privilegios And PlayerType.RoyalCouncil Then _
                                .flags.Privilegios = .flags.Privilegios + PlayerType.RoyalCouncil

                            Call WarpUserChar(tUser, .Pos.Map, .Pos.X, .Pos.Y, False)
                        End With
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleAcceptRoyalCouncilMember: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "ChaosCouncilMember" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleAcceptChaosCouncilMember(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/30/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) Then
                    tUser = NameIndex(UserName)
                    If tUser <= 0 Then
                        Call WriteConsoleMsg(UserIndex, "Usuario offline", FontTypeNames.FONTTYPE_INFO)
                    Else
                        Call _
                            SendData(SendTarget.ToAll, 0,
                                     PrepareMessageConsoleMsg(UserName & " fue aceptado en el Concilio de las Sombras.",
                                                              FontTypeNames.FONTTYPE_CONSEJO))

                        With UserList(tUser)
                            If .flags.Privilegios And PlayerType.RoyalCouncil Then _
                                .flags.Privilegios = .flags.Privilegios - PlayerType.RoyalCouncil
                            If Not .flags.Privilegios And PlayerType.ChaosCouncil Then _
                                .flags.Privilegios = .flags.Privilegios + PlayerType.ChaosCouncil

                            Call WarpUserChar(tUser, .Pos.Map, .Pos.X, .Pos.Y, False)
                        End With
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleAcceptChaosCouncilMember: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "ItemsInTheFloor" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleItemsInTheFloor(UserIndex As Short)
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

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub


            For X = 5 To 95
                For Y = 5 To 95
                    tObj = MapData(.Pos.Map, X, Y).ObjInfo.ObjIndex
                    If tObj > 0 Then
                        If ObjData_Renamed(tObj).OBJType <> eOBJType.otArboles Then
                            Call _
                                WriteConsoleMsg(UserIndex, "(" & X & "," & Y & ") " & ObjData_Renamed(tObj).name,
                                                FontTypeNames.FONTTYPE_INFO)
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

    Private Sub HandleMakeDumb(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/30/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim tUser As Short
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()

                If _
                    ((.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) <> 0 Or
                     ((.flags.Privilegios And (PlayerType.SemiDios Or PlayerType.RoleMaster)) =
                      (PlayerType.SemiDios Or PlayerType.RoleMaster))) Then
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


        Catch ex As Exception
            Console.WriteLine("Error in HandleMakeDumb: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "MakeDumbNoMore" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleMakeDumbNoMore(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/30/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim tUser As Short
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()

                If _
                    ((.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) <> 0 Or
                     ((.flags.Privilegios And (PlayerType.SemiDios Or PlayerType.RoleMaster)) =
                      (PlayerType.SemiDios Or PlayerType.RoleMaster))) Then
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


        Catch ex As Exception
            Console.WriteLine("Error in HandleMakeDumbNoMore: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "DumpIPTables" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleDumpIPTables(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/30/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub

            Call DumpTables()
        End With
    End Sub

    ''
    ' Handles the "CouncilKick" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleCouncilKick(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/30/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim tUser As Short
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) <> 0 Then
                    tUser = NameIndex(UserName)
                    If tUser <= 0 Then
                        If FileExist(CharPath & UserName & ".chr") Then
                            Call _
                                WriteConsoleMsg(UserIndex, "Usuario offline, echando de los consejos.",
                                                FontTypeNames.FONTTYPE_INFO)
                            Call WriteVar(CharPath & UserName & ".chr", "CONSEJO", "PERTENECE", CStr(0))
                            Call WriteVar(CharPath & UserName & ".chr", "CONSEJO", "PERTENECECAOS", CStr(0))
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex, "No se encuentra el charfile " & CharPath & UserName & ".chr",
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    Else
                        With UserList(tUser)
                            If .flags.Privilegios And PlayerType.RoyalCouncil Then
                                Call _
                                    WriteConsoleMsg(tUser, "Has sido echado del consejo de Banderbill.",
                                                    FontTypeNames.FONTTYPE_TALK)
                                .flags.Privilegios = .flags.Privilegios - PlayerType.RoyalCouncil

                                Call WarpUserChar(tUser, .Pos.Map, .Pos.X, .Pos.Y, False)
                                Call _
                                    SendData(SendTarget.ToAll, 0,
                                             PrepareMessageConsoleMsg(
                                                 UserName & " fue expulsado del consejo de Banderbill.",
                                                 FontTypeNames.FONTTYPE_CONSEJO))
                            End If

                            If .flags.Privilegios And PlayerType.ChaosCouncil Then
                                Call _
                                    WriteConsoleMsg(tUser, "Has sido echado del Concilio de las Sombras.",
                                                    FontTypeNames.FONTTYPE_TALK)
                                .flags.Privilegios = .flags.Privilegios - PlayerType.ChaosCouncil

                                Call WarpUserChar(tUser, .Pos.Map, .Pos.X, .Pos.Y, False)
                                Call _
                                    SendData(SendTarget.ToAll, 0,
                                             PrepareMessageConsoleMsg(
                                                 UserName & " fue expulsado del Concilio de las Sombras.",
                                                 FontTypeNames.FONTTYPE_CONSEJO))
                            End If
                        End With
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleCouncilKick: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "SetTrigger" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleSetTrigger(UserIndex As Short)
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

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

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

    Private Sub HandleAskTrigger(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 04/13/07
        '
        '***************************************************
        Dim tTrigger As Byte

        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

            tTrigger = MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger

            Call LogGM(.name, "Miro el trigger en " & .Pos.Map & "," & .Pos.X & "," & .Pos.Y & ". Era " & tTrigger)

            Call _
                WriteConsoleMsg(UserIndex, "Trigger " & tTrigger & " en mapa " & .Pos.Map & " " & .Pos.X & ", " & .Pos.Y,
                                FontTypeNames.FONTTYPE_INFO)
        End With
    End Sub

    ''
    ' Handles the "BannedIPList" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleBannedIPList(UserIndex As Short)
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

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub


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

    Private Sub HandleBannedIPReload(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/30/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

            Call BanIpGuardar()
            Call BanIpCargar()
        End With
    End Sub

    ''
    ' Handles the "GuildBan" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleGuildBan(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/30/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) Then
                    tFile = AppDomain.CurrentDomain.BaseDirectory & "guilds/" & GuildName & "-members.mem"

                    If Not FileExist(tFile) Then
                        Call WriteConsoleMsg(UserIndex, "No existe el clan: " & GuildName, FontTypeNames.FONTTYPE_INFO)
                    Else
                        Call _
                            SendData(SendTarget.ToAll, 0,
                                     PrepareMessageConsoleMsg(.name & " baneó al clan " & UCase(GuildName),
                                                              FontTypeNames.FONTTYPE_FIGHT))

                        'baneamos a los miembros
                        Call LogGM(.name, "BANCLAN a " & UCase(GuildName))

                        cantMembers = Val(GetVar(tFile, "INIT", "NroMembers"))

                        For LoopC = 1 To cantMembers
                            member = GetVar(tFile, "Members", "Member" & LoopC)
                            'member es la victima
                            Call Ban(member, "Administracion del servidor", "Clan Banned")

                            Call _
                                SendData(SendTarget.ToAll, 0,
                                         PrepareMessageConsoleMsg(
                                             "   " & member & "<" & GuildName & "> ha sido expulsado del servidor.",
                                             FontTypeNames.FONTTYPE_FIGHT))

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
                            Call _
                                WriteVar(CharPath & member & ".chr", "PENAS", "P" & Count + 1,
                                         LCase(.name) & ": BAN AL CLAN: " & GuildName & " " & Today & " " & TimeOfDay)
                        Next LoopC
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleGuildBan: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "BanIP" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleBanIP(UserIndex As Short)
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

        Try
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


                If .flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios) Then
                    If migr_LenB(bannedIP) > 0 Then
                        Call LogGM(.name, "/BanIP " & bannedIP & " por " & reason)

                        If BanIpBuscar(bannedIP) > 0 Then
                            Call _
                                WriteConsoleMsg(UserIndex, "La IP " & bannedIP & " ya se encuentra en la lista de bans.",
                                                FontTypeNames.FONTTYPE_INFO)
                        Else
                            Call BanIpAgrega(bannedIP)
                            Call _
                                SendData(SendTarget.ToAdmins, 0,
                                         PrepareMessageConsoleMsg(.name & " baneó la IP " & bannedIP & " por " & reason,
                                                                  FontTypeNames.FONTTYPE_FIGHT))

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


        Catch ex As Exception
            Console.WriteLine("Error in HandleBanIP: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "UnbanIP" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleUnbanIP(UserIndex As Short)
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

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

            If BanIpQuita(bannedIP) Then
                Call _
                    WriteConsoleMsg(UserIndex, "La IP """ & bannedIP & """ se ha quitado de la lista de bans.",
                                    FontTypeNames.FONTTYPE_INFO)
            Else
                Call _
                    WriteConsoleMsg(UserIndex, "La IP """ & bannedIP & """ NO se encuentra en la lista de bans.",
                                    FontTypeNames.FONTTYPE_INFO)
            End If
        End With
    End Sub

    ''
    ' Handles the "CreateItem" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleCreateItem(UserIndex As Short)
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

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub

            Call LogGM(.name, "/CI: " & tObj)

            If MapData(.Pos.Map, .Pos.X, .Pos.Y - 1).ObjInfo.ObjIndex > 0 Then Exit Sub

            If MapData(.Pos.Map, .Pos.X, .Pos.Y - 1).TileExit.Map > 0 Then Exit Sub

            If tObj < 1 Or tObj > NumObjDatas Then Exit Sub

            'Is the object not null?
            If migr_LenB(ObjData_Renamed(tObj).name) = 0 Then Exit Sub

            Call _
                WriteConsoleMsg(UserIndex,
                                "¡¡ATENCIÓN: FUERON CREADOS ***100*** ÍTEMS, TIRE Y /DEST LOS QUE NO NECESITE!!",
                                FontTypeNames.FONTTYPE_GUILD)

            Objeto.Amount = 100
            Objeto.ObjIndex = tObj
            Call MakeObj(Objeto, .Pos.Map, .Pos.X, .Pos.Y - 1)
        End With
    End Sub

    ''
    ' Handles the "DestroyItems" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleDestroyItems(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/30/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub

            If MapData(.Pos.Map, .Pos.X, .Pos.Y).ObjInfo.ObjIndex = 0 Then Exit Sub

            Call LogGM(.name, "/DEST")

            If _
                ObjData_Renamed(MapData(.Pos.Map, .Pos.X, .Pos.Y).ObjInfo.ObjIndex).OBJType =
                eOBJType.otTeleport And MapData(.Pos.Map, .Pos.X, .Pos.Y).TileExit.Map > 0 Then

                Call _
                    WriteConsoleMsg(UserIndex, "No puede destruir teleports así. Utilice /DT.",
                                    FontTypeNames.FONTTYPE_INFO)
                Exit Sub
            End If

            Call EraseObj(10000, .Pos.Map, .Pos.X, .Pos.Y)
        End With
    End Sub

    ''
    ' Handles the "ChaosLegionKick" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleChaosLegionKick(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/30/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim tUser As Short
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) <> 0 Then
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
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            UserName & " expulsado de las fuerzas del caos y prohibida la reenlistada.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Call _
                            WriteConsoleMsg(tUser,
                                            .name & " te ha expulsado en forma definitiva de las fuerzas del caos.",
                                            FontTypeNames.FONTTYPE_FIGHT)
                        Call FlushBuffer(tUser)
                    Else
                        If FileExist(CharPath & UserName & ".chr") Then
                            Call WriteVar(CharPath & UserName & ".chr", "FACCIONES", "EjercitoCaos", CStr(0))
                            Call WriteVar(CharPath & UserName & ".chr", "FACCIONES", "Reenlistadas", CStr(200))
                            Call WriteVar(CharPath & UserName & ".chr", "FACCIONES", "Extra", "Expulsado por " & .name)
                            Call _
                                WriteConsoleMsg(UserIndex,
                                                UserName &
                                                " expulsado de las fuerzas del caos y prohibida la reenlistada.",
                                                FontTypeNames.FONTTYPE_INFO)
                        Else
                            Call WriteConsoleMsg(UserIndex, UserName & ".chr inexistente.", FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleChaosLegionKick: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "RoyalArmyKick" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleRoyalArmyKick(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/30/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim tUser As Short
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) <> 0 Then
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
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            UserName & " expulsado de las fuerzas reales y prohibida la reenlistada.",
                                            FontTypeNames.FONTTYPE_INFO)
                        Call _
                            WriteConsoleMsg(tUser, .name & " te ha expulsado en forma definitiva de las fuerzas reales.",
                                            FontTypeNames.FONTTYPE_FIGHT)
                        Call FlushBuffer(tUser)
                    Else
                        If FileExist(CharPath & UserName & ".chr") Then
                            Call WriteVar(CharPath & UserName & ".chr", "FACCIONES", "EjercitoReal", CStr(0))
                            Call WriteVar(CharPath & UserName & ".chr", "FACCIONES", "Reenlistadas", CStr(200))
                            Call WriteVar(CharPath & UserName & ".chr", "FACCIONES", "Extra", "Expulsado por " & .name)
                            Call _
                                WriteConsoleMsg(UserIndex,
                                                UserName &
                                                " expulsado de las fuerzas reales y prohibida la reenlistada.",
                                                FontTypeNames.FONTTYPE_INFO)
                        Else
                            Call WriteConsoleMsg(UserIndex, UserName & ".chr inexistente.", FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleRoyalArmyKick: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "ForceMIDIAll" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleForceMIDIAll(UserIndex As Short)
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

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub

            Call _
                SendData(SendTarget.ToAll, 0,
                         PrepareMessageConsoleMsg(.name & " broadcast música: " & midiID, FontTypeNames.FONTTYPE_SERVER))

            Call SendData(SendTarget.ToAll, 0, PrepareMessagePlayMidi(midiID))
        End With
    End Sub

    ''
    ' Handles the "ForceWAVEAll" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleForceWAVEAll(UserIndex As Short)
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

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub

            Call SendData(SendTarget.ToAll, 0, PrepareMessagePlayWave(waveID, NO_3D_SOUND, NO_3D_SOUND))
        End With
    End Sub

    ''
    ' Handles the "RemovePunishment" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleRemovePunishment(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 1/05/07
        'Pablo (ToxicWaste): 1/05/07, You can now edit the punishment.
        '***************************************************
        If UserList(UserIndex).incomingData.length < 6 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) Then
                    If migr_LenB(UserName) = 0 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "Utilice /borrarpena Nick@NumeroDePena@NuevaPena",
                                            FontTypeNames.FONTTYPE_INFO)
                    Else
                        If (migr_InStrB(UserName, "\") <> 0) Then
                            UserName = Replace(UserName, "\", "")
                        End If
                        If (migr_InStrB(UserName, "/") <> 0) Then
                            UserName = Replace(UserName, "/", "")
                        End If

                        If FileExist(CharPath & UserName & ".chr") Then
                            Call _
                                LogGM(.name,
                                      " borro la pena: " & punishment & "-" &
                                      GetVar(CharPath & UserName & ".chr", "PENAS", "P" & punishment) & " de " &
                                      UserName &
                                      " y la cambió por: " & NewText)

                            Call _
                                WriteVar(CharPath & UserName & ".chr", "PENAS", "P" & punishment,
                                         LCase(.name) & ": <" & NewText & "> " & Today & " " & TimeOfDay)

                            Call WriteConsoleMsg(UserIndex, "Pena modificada.", FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleRemovePunishment: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "TileBlockedToggle" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleTileBlockedToggle(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/30/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub

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

    Private Sub HandleKillNPCNoRespawn(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/30/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub

            If .flags.TargetNPC = 0 Then Exit Sub

            Call QuitarNPC(.flags.TargetNPC)
            Call LogGM(.name, "/MATA " & Npclist(.flags.TargetNPC).name)
        End With
    End Sub

    ''
    ' Handles the "KillAllNearbyNPCs" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Private Sub HandleKillAllNearbyNPCs(UserIndex As Short)
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

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub


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

    Private Sub HandleLastIP(UserIndex As Short)
        '***************************************************
        'Author: Nicolas Matias Gonzalez (NIGO)
        'Last Modification: 12/30/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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


                priv = PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios Or
                       PlayerType.Consejero
                UserName = buffer.ReadASCIIString()

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And
                     (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios)) <>
                    0 Then
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
                        validCheck = (UserList(NameIndex(UserName)).flags.Privilegios And priv) = 0 Or
                                     (.flags.Privilegios And
                                      (PlayerType.Admin Or PlayerType.Dios)) <> 0
                    Else
                        validCheck = (UserDarPrivilegioLevel(UserName) And priv) = 0 Or
                                     (.flags.Privilegios And
                                      (PlayerType.Admin Or PlayerType.Dios)) <> 0
                    End If

                    If validCheck Then
                        Call LogGM(.name, "/LASTIP " & UserName)

                        If FileExist(CharPath & UserName & ".chr") Then
                            lista = "Las ultimas IPs con las que " & UserName & " se conectó son:"
                            For LoopC = 1 To 5
                                lista = lista & vbCrLf & LoopC & " - " &
                                        GetVar(CharPath & UserName & ".chr", "INIT", "LastIP" & LoopC)
                            Next LoopC
                            Call WriteConsoleMsg(UserIndex, lista, FontTypeNames.FONTTYPE_INFO)
                        Else
                            Call _
                                WriteConsoleMsg(UserIndex, "Charfile """ & UserName & """ inexistente.",
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex, UserName & " es de mayor jerarquía que vos.",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleLastIP: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "ChatColor" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Public Sub HandleChatColor(UserIndex As Short)
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

            If _
                (.flags.Privilegios And
                 (PlayerType.Admin Or PlayerType.Dios Or PlayerType.RoleMaster)) _
                Then
                .flags.ChatColor = color
            End If
        End With
    End Sub

    ''
    ' Handles the "Ignored" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Public Sub HandleIgnored(UserIndex As Short)
        '***************************************************
        'Author: Lucas Tavolaro Ortiz (Tavo)
        'Last Modification: 12/23/06
        'Ignore the user
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.Admin Or PlayerType.Dios Or PlayerType.SemiDios Or
                 PlayerType.Consejero) Then
                .flags.AdminPerseguible = Not .flags.AdminPerseguible
            End If
        End With
    End Sub

    ''
    ' Handles the "CheckSlot" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Public Sub HandleCheckSlot(UserIndex As Short)
        '***************************************************
        'Author: Pablo (ToxicWaste)
        'Last Modification: 09/09/2008 (NicoNZ)
        'Check one Users Slot in Particular from Inventory
        '***************************************************
        If UserList(UserIndex).incomingData.length < 4 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If _
                    .flags.Privilegios And
                    (PlayerType.Admin Or PlayerType.SemiDios Or PlayerType.Dios) _
                    Then
                    tIndex = NameIndex(UserName) 'Que user index?

                    Call LogGM(.name, .name & " Checkeó el slot " & Slot & " de " & UserName)

                    If tIndex > 0 Then
                        If Slot > 0 And Slot <= UserList(tIndex).CurrentInventorySlots Then
                            If UserList(tIndex).Invent.Object_Renamed(Slot).ObjIndex > 0 Then
                                Call _
                                    WriteConsoleMsg(UserIndex,
                                                    " Objeto " & Slot & ") " &
                                                    ObjData_Renamed(
                                                        UserList(tIndex).Invent.Object_Renamed(Slot).ObjIndex).
                                                        name & " Cantidad:" &
                                                    UserList(tIndex).Invent.Object_Renamed(Slot).Amount,
                                                    FontTypeNames.FONTTYPE_INFO)
                            Else
                                Call _
                                    WriteConsoleMsg(UserIndex, "No hay ningún objeto en slot seleccionado.",
                                                    FontTypeNames.FONTTYPE_INFO)
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


        Catch ex As Exception
            Console.WriteLine("Error in HandleCheckSlot: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handles the "ResetAutoUpdate" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Public Sub HandleResetAutoUpdate(UserIndex As Short)
        '***************************************************
        'Author: Lucas Tavolaro Ortiz (Tavo)
        'Last Modification: 12/23/06
        'Reset the AutoUpdate
        '***************************************************
        With UserList(UserIndex)
            'Remove packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub
            If UCase(.name) <> "MARAXUS" Then Exit Sub

            Call WriteConsoleMsg(UserIndex, "TID: " & CStr(ReiniciarAutoUpdate()), FontTypeNames.FONTTYPE_INFO)
        End With
    End Sub

    ''
    ' Handles the "Restart" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Public Sub HandleRestart(UserIndex As Short)
        '***************************************************
        'Author: Lucas Tavolaro Ortiz (Tavo)
        'Last Modification: 12/23/06
        'Restart the game
        '***************************************************
        With UserList(UserIndex)
            'Remove Packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub
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

    Public Sub HandleReloadObjects(UserIndex As Short)
        '***************************************************
        'Author: Lucas Tavolaro Ortiz (Tavo)
        'Last Modification: 12/23/06
        'Reload the objects
        '***************************************************
        With UserList(UserIndex)
            'Remove Packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

            Call LogGM(.name, .name & " ha recargado los objetos.")

            Call LoadOBJData()
        End With
    End Sub

    ''
    ' Handles the "ReloadSpells" message.
    '
    ' @param    userIndex The index of the user sending the message.

    Public Sub HandleReloadSpells(UserIndex As Short)
        '***************************************************
        'Author: Lucas Tavolaro Ortiz (Tavo)
        'Last Modification: 12/23/06
        'Reload the spells
        '***************************************************
        With UserList(UserIndex)
            'Remove Packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

            Call LogGM(.name, .name & " ha recargado los hechizos.")

            Call CargarHechizos()
        End With
    End Sub

    ''
    ' Handle the "ReloadServerIni" message.
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleReloadServerIni(UserIndex As Short)
        '***************************************************
        'Author: Lucas Tavolaro Ortiz (Tavo)
        'Last Modification: 12/23/06
        'Reload the Server`s INI
        '***************************************************
        With UserList(UserIndex)
            'Remove Packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

            Call LogGM(.name, .name & " ha recargado los INITs.")

            Call LoadSini()
        End With
    End Sub

    ''
    ' Handle the "ReloadNPCs" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleReloadNPCs(UserIndex As Short)
        '***************************************************
        'Author: Lucas Tavolaro Ortiz (Tavo)
        'Last Modification: 12/23/06
        'Reload the Server`s NPC
        '***************************************************
        With UserList(UserIndex)
            'Remove Packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

            Call LogGM(.name, .name & " ha recargado los NPCs.")

            Call CargaNpcsDat()

            Call WriteConsoleMsg(UserIndex, "Npcs.dat recargado.", FontTypeNames.FONTTYPE_INFO)
        End With
    End Sub

    ''
    ' Handle the "KickAllChars" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleKickAllChars(UserIndex As Short)
        '***************************************************
        'Author: Lucas Tavolaro Ortiz (Tavo)
        'Last Modification: 12/23/06
        'Kick all the chars that are online
        '***************************************************
        With UserList(UserIndex)
            'Remove Packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

            Call LogGM(.name, .name & " ha echado a todos los personajes.")

            Call EcharPjsNoPrivilegiados()
        End With
    End Sub

    ''
    ' Handle the "Night" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleNight(UserIndex As Short)
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

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub
            If UCase(.name) <> "MARAXUS" Then Exit Sub

            DeNoche = Not DeNoche


            For i = 1 To NumUsers
                If UserList(i).flags.UserLogged And UserList(i).ConnID > - 1 Then
                    Call EnviarNoche(i)
                End If
            Next i
        End With
    End Sub

    ''
    ' Handle the "ShowServerForm" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleShowServerForm(UserIndex As Short)
        '***************************************************
        'Author: Lucas Tavolaro Ortiz (Tavo)
        'Last Modification: 12/23/06
        'Show the server form
        '***************************************************
        With UserList(UserIndex)
            'Remove Packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

            Call LogGM(.name, .name & " ha solicitado mostrar el formulario del servidor.")
            ' TODO FIX: no funciona como se espera, de todas formas no es algo funcional
        End With
    End Sub

    ''
    ' Handle the "CleanSOS" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleCleanSOS(UserIndex As Short)
        '***************************************************
        'Author: Lucas Tavolaro Ortiz (Tavo)
        'Last Modification: 12/23/06
        'Clean the SOS
        '***************************************************
        With UserList(UserIndex)
            'Remove Packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

            Call LogGM(.name, .name & " ha borrado los SOS.")

            Call Ayuda.Reset_Renamed()
        End With
    End Sub

    ''
    ' Handle the "SaveChars" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleSaveChars(UserIndex As Short)
        '***************************************************
        'Author: Lucas Tavolaro Ortiz (Tavo)
        'Last Modification: 12/23/06
        'Save the characters
        '***************************************************
        With UserList(UserIndex)
            'Remove Packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

            Call LogGM(.name, .name & " ha guardado todos los chars.")

            Call ActualizaExperiencias()
            Call GuardarUsuarios()
        End With
    End Sub

    ''
    ' Handle the "ChangeMapInfoBackup" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleChangeMapInfoBackup(UserIndex As Short)
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

            If (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) = 0 Then _
                Exit Sub

            Call LogGM(.name, .name & " ha cambiado la información sobre el BackUp.")

            'Change the boolean to byte in a fast way
            If doTheBackUp Then
                MapInfo_Renamed(.Pos.Map).BackUp = 1
            Else
                MapInfo_Renamed(.Pos.Map).BackUp = 0
            End If

            'Change the boolean to string in a fast way
            Call _
                WriteVar(AppDomain.CurrentDomain.BaseDirectory & MapPath & "mapa" & .Pos.Map & ".dat", "Mapa" & .Pos.Map,
                         "backup", CStr(MapInfo_Renamed(.Pos.Map).BackUp))

            Call _
                WriteConsoleMsg(UserIndex, "Mapa " & .Pos.Map & " Backup: " & MapInfo_Renamed(.Pos.Map).BackUp,
                                FontTypeNames.FONTTYPE_INFO)
        End With
    End Sub

    ''
    ' Handle the "ChangeMapInfoPK" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleChangeMapInfoPK(UserIndex As Short)
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

            If (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) = 0 Then _
                Exit Sub

            Call LogGM(.name, .name & " ha cambiado la información sobre si es PK el mapa.")

            MapInfo_Renamed(.Pos.Map).Pk = isMapPk

            'Change the boolean to string in a fast way
            Call _
                WriteVar(AppDomain.CurrentDomain.BaseDirectory & MapPath & "mapa" & .Pos.Map & ".dat", "Mapa" & .Pos.Map,
                         "Pk", IIf(isMapPk, "1", "0"))

            Call _
                WriteConsoleMsg(UserIndex, "Mapa " & .Pos.Map & " PK: " & MapInfo_Renamed(.Pos.Map).Pk,
                                FontTypeNames.FONTTYPE_INFO)
        End With
    End Sub

    ''
    ' Handle the "ChangeMapInfoRestricted" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleChangeMapInfoRestricted(UserIndex As Short)
        '***************************************************
        'Author: Pablo (ToxicWaste)
        'Last Modification: 26/01/2007
        'Restringido -> Options: "NEWBIE", "NO", "ARMADA", "CAOS", "FACCION".
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim tStr As String

            Dim buffer As New clsByteQueue
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove Packet ID
                Call buffer.ReadByte()

                tStr = buffer.ReadASCIIString()

                If (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) <> 0 Then
                    If tStr = "NEWBIE" Or tStr = "NO" Or tStr = "ARMADA" Or tStr = "CAOS" Or tStr = "FACCION" Then
                        Call LogGM(.name, .name & " ha cambiado la información sobre si es restringido el mapa.")
                        MapInfo_Renamed(UserList(UserIndex).Pos.Map).Restringir = tStr
                        Call _
                            WriteVar(
                                AppDomain.CurrentDomain.BaseDirectory & MapPath & "mapa" & UserList(UserIndex).Pos.Map &
                                ".dat", "Mapa" & UserList(UserIndex).Pos.Map, "Restringir", tStr)
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "Mapa " & .Pos.Map & " Restringido: " & MapInfo_Renamed(.Pos.Map).Restringir,
                                            FontTypeNames.FONTTYPE_INFO)
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "Opciones para restringir: 'NEWBIE', 'NO', 'ARMADA', 'CAOS', 'FACCION'",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleChangeMapInfoRestricted: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handle the "ChangeMapInfoNoMagic" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleChangeMapInfoNoMagic(UserIndex As Short)
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

            If (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) <> 0 Then
                Call LogGM(.name, .name & " ha cambiado la información sobre si está permitido usar la magia el mapa.")
                MapInfo_Renamed(UserList(UserIndex).Pos.Map).MagiaSinEfecto = nomagic
                Call _
                    WriteVar(
                        AppDomain.CurrentDomain.BaseDirectory & MapPath & "mapa" & UserList(UserIndex).Pos.Map & ".dat",
                        "Mapa" & UserList(UserIndex).Pos.Map, "MagiaSinEfecto", CStr(nomagic))
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Mapa " & .Pos.Map & " MagiaSinEfecto: " & MapInfo_Renamed(.Pos.Map).MagiaSinEfecto,
                                    FontTypeNames.FONTTYPE_INFO)
            End If
        End With
    End Sub

    ''
    ' Handle the "ChangeMapInfoNoInvi" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleChangeMapInfoNoInvi(UserIndex As Short)
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

            If (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) <> 0 Then
                Call _
                    LogGM(.name,
                          .name &
                          " ha cambiado la información sobre si está permitido usar la invisibilidad en el mapa.")
                MapInfo_Renamed(UserList(UserIndex).Pos.Map).InviSinEfecto = noinvi
                Call _
                    WriteVar(
                        AppDomain.CurrentDomain.BaseDirectory & MapPath & "mapa" & UserList(UserIndex).Pos.Map & ".dat",
                        "Mapa" & UserList(UserIndex).Pos.Map, "InviSinEfecto", CStr(noinvi))
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Mapa " & .Pos.Map & " InviSinEfecto: " & MapInfo_Renamed(.Pos.Map).InviSinEfecto,
                                    FontTypeNames.FONTTYPE_INFO)
            End If
        End With
    End Sub

    ''
    ' Handle the "ChangeMapInfoNoResu" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleChangeMapInfoNoResu(UserIndex As Short)
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

            If (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) <> 0 Then
                Call _
                    LogGM(.name,
                          .name & " ha cambiado la información sobre si está permitido usar el resucitar en el mapa.")
                MapInfo_Renamed(UserList(UserIndex).Pos.Map).ResuSinEfecto = noresu
                Call _
                    WriteVar(
                        AppDomain.CurrentDomain.BaseDirectory & MapPath & "mapa" & UserList(UserIndex).Pos.Map & ".dat",
                        "Mapa" & UserList(UserIndex).Pos.Map, "ResuSinEfecto", CStr(noresu))
                Call _
                    WriteConsoleMsg(UserIndex,
                                    "Mapa " & .Pos.Map & " ResuSinEfecto: " & MapInfo_Renamed(.Pos.Map).ResuSinEfecto,
                                    FontTypeNames.FONTTYPE_INFO)
            End If
        End With
    End Sub

    ''
    ' Handle the "ChangeMapInfoLand" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleChangeMapInfoLand(UserIndex As Short)
        '***************************************************
        'Author: Pablo (ToxicWaste)
        'Last Modification: 26/01/2007
        'Terreno -> Opciones: "BOSQUE", "NIEVE", "DESIERTO", "CIUDAD", "CAMPO", "DUNGEON".
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim tStr As String

            Dim buffer As New clsByteQueue
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove Packet ID
                Call buffer.ReadByte()

                tStr = buffer.ReadASCIIString()

                If (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) <> 0 Then
                    If _
                        tStr = "BOSQUE" Or tStr = "NIEVE" Or tStr = "DESIERTO" Or tStr = "CIUDAD" Or tStr = "CAMPO" Or
                        tStr = "DUNGEON" Then
                        Call LogGM(.name, .name & " ha cambiado la información del terreno del mapa.")
                        MapInfo_Renamed(UserList(UserIndex).Pos.Map).Terreno = tStr
                        Call _
                            WriteVar(
                                AppDomain.CurrentDomain.BaseDirectory & MapPath & "mapa" & UserList(UserIndex).Pos.Map &
                                ".dat", "Mapa" & UserList(UserIndex).Pos.Map, "Terreno", tStr)
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "Mapa " & .Pos.Map & " Terreno: " & MapInfo_Renamed(.Pos.Map).Terreno,
                                            FontTypeNames.FONTTYPE_INFO)
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "Opciones para terreno: 'BOSQUE', 'NIEVE', 'DESIERTO', 'CIUDAD', 'CAMPO', 'DUNGEON'",
                                            FontTypeNames.FONTTYPE_INFO)
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "Igualmente, el único útil es 'NIEVE' ya que al ingresarlo, la gente muere de frío en el mapa.",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleChangeMapInfoLand: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handle the "ChangeMapInfoZone" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleChangeMapInfoZone(UserIndex As Short)
        '***************************************************
        'Author: Pablo (ToxicWaste)
        'Last Modification: 26/01/2007
        'Zona -> Opciones: "BOSQUE", "NIEVE", "DESIERTO", "CIUDAD", "CAMPO", "DUNGEON".
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim tStr As String

            Dim buffer As New clsByteQueue
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove Packet ID
                Call buffer.ReadByte()

                tStr = buffer.ReadASCIIString()

                If (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) <> 0 Then
                    If _
                        tStr = "BOSQUE" Or tStr = "NIEVE" Or tStr = "DESIERTO" Or tStr = "CIUDAD" Or tStr = "CAMPO" Or
                        tStr = "DUNGEON" Then
                        Call LogGM(.name, .name & " ha cambiado la información de la zona del mapa.")
                        MapInfo_Renamed(UserList(UserIndex).Pos.Map).Zona = tStr
                        Call _
                            WriteVar(
                                AppDomain.CurrentDomain.BaseDirectory & MapPath & "mapa" & UserList(UserIndex).Pos.Map &
                                ".dat", "Mapa" & UserList(UserIndex).Pos.Map, "Zona", tStr)
                        Call _
                            WriteConsoleMsg(UserIndex, "Mapa " & .Pos.Map & " Zona: " & MapInfo_Renamed(.Pos.Map).Zona,
                                            FontTypeNames.FONTTYPE_INFO)
                    Else
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "Opciones para terreno: 'BOSQUE', 'NIEVE', 'DESIERTO', 'CIUDAD', 'CAMPO', 'DUNGEON'",
                                            FontTypeNames.FONTTYPE_INFO)
                        Call _
                            WriteConsoleMsg(UserIndex,
                                            "Igualmente, el único útil es 'DUNGEON' ya que al ingresarlo, NO se sentirá el efecto de la lluvia en este mapa.",
                                            FontTypeNames.FONTTYPE_INFO)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleChangeMapInfoZone: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handle the "SaveMap" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleSaveMap(UserIndex As Short)
        '***************************************************
        'Author: Lucas Tavolaro Ortiz (Tavo)
        'Last Modification: 12/24/06
        'Saves the map
        '***************************************************
        With UserList(UserIndex)
            'Remove Packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

            Call LogGM(.name, .name & " ha guardado el mapa " & CStr(.Pos.Map))

            Call GrabarMapa(.Pos.Map, AppDomain.CurrentDomain.BaseDirectory & "WorldBackUp/Mapa" & .Pos.Map)

            Call WriteConsoleMsg(UserIndex, "Mapa Guardado.", FontTypeNames.FONTTYPE_INFO)
        End With
    End Sub

    ''
    ' Handle the "ShowGuildMessages" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleShowGuildMessages(UserIndex As Short)
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

        Try
            Dim buffer As New clsByteQueue
            Dim guild As String
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                guild = buffer.ReadASCIIString()

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) Then
                    Call GMEscuchaClan(UserIndex, guild)
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleShowGuildMessages: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handle the "DoBackUp" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleDoBackUp(UserIndex As Short)
        '***************************************************
        'Author: Lucas Tavolaro Ortiz (Tavo)
        'Last Modification: 12/24/06
        'Show guilds messages
        '***************************************************
        With UserList(UserIndex)
            'Remove Packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

            Call LogGM(.name, .name & " ha hecho un backup.")

            Call DoBackUp() 'Sino lo confunde con la id del paquete
        End With
    End Sub

    ''
    ' Handle the "ToggleCentinelActivated" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleToggleCentinelActivated(UserIndex As Short)
        '***************************************************
        'Author: Lucas Tavolaro Ortiz (Tavo)
        'Last Modification: 12/26/06
        'Last modified by: Juan Martín Sotuyo Dodero (Maraxus)
        'Activate or desactivate the Centinel
        '***************************************************
        With UserList(UserIndex)
            'Remove Packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub

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
                Call _
                    SendData(SendTarget.ToAdmins, 0,
                             PrepareMessageConsoleMsg("El centinela ha sido activado.", FontTypeNames.FONTTYPE_SERVER))
            Else
                Call _
                    SendData(SendTarget.ToAdmins, 0,
                             PrepareMessageConsoleMsg("El centinela ha sido desactivado.", FontTypeNames.FONTTYPE_SERVER))
            End If
        End With
    End Sub

    ''
    ' Handle the "AlterName" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleAlterName(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 12/26/06
        'Change user name
        '***************************************************
        If UserList(UserIndex).incomingData.length < 5 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) Then
                    If migr_LenB(UserName) = 0 Or migr_LenB(newName) = 0 Then
                        Call WriteConsoleMsg(UserIndex, "Usar: /ANAME origen@destino", FontTypeNames.FONTTYPE_INFO)
                    Else
                        changeNameUI = NameIndex(UserName)

                        If changeNameUI > 0 Then
                            Call _
                                WriteConsoleMsg(UserIndex, "El Pj está online, debe salir para hacer el cambio.",
                                                FontTypeNames.FONTTYPE_WARNING)
                        Else
                            If Not FileExist(CharPath & UserName & ".chr") Then
                                Call _
                                    WriteConsoleMsg(UserIndex, "El pj " & UserName & " es inexistente.",
                                                    FontTypeNames.FONTTYPE_INFO)
                            Else
                                GuildIndex = Val(GetVar(CharPath & UserName & ".chr", "GUILD", "GUILDINDEX"))

                                If GuildIndex > 0 Then
                                    Call _
                                        WriteConsoleMsg(UserIndex,
                                                        "El pj " & UserName &
                                                        " pertenece a un clan, debe salir del mismo con /salirclan para ser transferido.",
                                                        FontTypeNames.FONTTYPE_INFO)
                                Else
                                    If Not FileExist(CharPath & newName & ".chr") Then
                                        Call FileCopy(CharPath & UserName & ".chr", CharPath & UCase(newName) & ".chr")

                                        Call _
                                            WriteConsoleMsg(UserIndex, "Transferencia exitosa.",
                                                            FontTypeNames.FONTTYPE_INFO)

                                        Call WriteVar(CharPath & UserName & ".chr", "FLAGS", "Ban", "1")


                                        cantPenas = Val(GetVar(CharPath & UserName & ".chr", "PENAS", "Cant"))

                                        Call _
                                            WriteVar(CharPath & UserName & ".chr", "PENAS", "Cant", CStr(cantPenas + 1))

                                        Call _
                                            WriteVar(CharPath & UserName & ".chr", "PENAS", "P" & CStr(cantPenas + 1),
                                                     LCase(.name) & ": BAN POR Cambio de nick a " & UCase(newName) & " " &
                                                     Today & " " & TimeOfDay)

                                        Call _
                                            LogGM(.name,
                                                  "Ha cambiado de nombre al usuario " & UserName & ". Ahora se llama " &
                                                  newName)
                                    Else
                                        Call _
                                            WriteConsoleMsg(UserIndex, "El nick solicitado ya existe.",
                                                            FontTypeNames.FONTTYPE_INFO)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleAlterName: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handle the "AlterName" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleAlterMail(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 12/26/06
        'Change user password
        '***************************************************
        If UserList(UserIndex).incomingData.length < 5 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) Then
                    If migr_LenB(UserName) = 0 Or migr_LenB(newMail) = 0 Then
                        Call WriteConsoleMsg(UserIndex, "usar /AEMAIL <pj>-<nuevomail>", FontTypeNames.FONTTYPE_INFO)
                    Else
                        If Not FileExist(CharPath & UserName & ".chr") Then
                            Call _
                                WriteConsoleMsg(UserIndex, "No existe el charfile " & UserName & ".chr",
                                                FontTypeNames.FONTTYPE_INFO)
                        Else
                            Call WriteVar(CharPath & UserName & ".chr", "CONTACTO", "Email", newMail)
                            Call _
                                WriteConsoleMsg(UserIndex, "Email de " & UserName & " cambiado a: " & newMail,
                                                FontTypeNames.FONTTYPE_INFO)
                        End If

                        Call LogGM(.name, "Le ha cambiado el mail a " & UserName)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleAlterMail: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handle the "AlterPassword" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleAlterPassword(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 12/26/06
        'Change user password
        '***************************************************
        If UserList(UserIndex).incomingData.length < 5 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) Then
                    Call LogGM(.name, "Ha alterado la contraseña de " & UserName)

                    If migr_LenB(UserName) = 0 Or migr_LenB(copyFrom) = 0 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "usar /APASS <pjsinpass>@<pjconpass>",
                                            FontTypeNames.FONTTYPE_INFO)
                    Else
                        If Not FileExist(CharPath & UserName & ".chr") Or Not FileExist(CharPath & copyFrom & ".chr") _
                            Then
                            Call _
                                WriteConsoleMsg(UserIndex, "Alguno de los PJs no existe " & UserName & "@" & copyFrom,
                                                FontTypeNames.FONTTYPE_INFO)
                        Else
                            Password = GetVar(CharPath & copyFrom & ".chr", "INIT", "Password")
                            Call WriteVar(CharPath & UserName & ".chr", "INIT", "Password", Password)

                            Call _
                                WriteConsoleMsg(UserIndex,
                                                "Password de " & UserName & " ha cambiado por la de " & copyFrom,
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleAlterPassword: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handle the "HandleCreateNPC" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleCreateNPC(UserIndex As Short)
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

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub

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

    Public Sub HandleCreateNPCWithRespawn(UserIndex As Short)
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

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios) Then Exit Sub

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

    Public Sub HandleImperialArmour(UserIndex As Short)
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

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

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

    Public Sub HandleChaosArmour(UserIndex As Short)
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

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

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

    Public Sub HandleNavigateToggle(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 01/12/07
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove Packet ID
            Call .incomingData.ReadByte()

            If .flags.Privilegios And (PlayerType.User Or PlayerType.Consejero) Then _
                Exit Sub

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

    Public Sub HandleServerOpenToUsersToggle(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 12/24/06
        '
        '***************************************************
        With UserList(UserIndex)
            'Remove Packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

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

    Public Sub HandleTurnOffServer(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 12/24/06
        'Turns off the server
        '***************************************************
        Dim handle As Short

        With UserList(UserIndex)
            'Remove Packet ID
            Call .incomingData.ReadByte()

            If _
                .flags.Privilegios And
                (PlayerType.User Or PlayerType.Consejero Or
                 PlayerType.SemiDios Or PlayerType.RoleMaster) Then Exit Sub

            Call LogGM(.name, "/APAGAR")
            Call _
                SendData(SendTarget.ToAll, 0,
                         PrepareMessageConsoleMsg("¡¡¡" & .name & " VA A APAGAR EL SERVIDOR!!!",
                                                  FontTypeNames.FONTTYPE_FIGHT))

            'Log
            handle = FreeFile
            FileOpen(handle, AppDomain.CurrentDomain.BaseDirectory & "logs/Main.log", OpenMode.Append, ,
                     OpenShare.Shared)

            PrintLine(handle, Today & " " & TimeOfDay & " server apagado por " & .name & ". ")

            FileClose(handle)
        End With
    End Sub

    ''
    ' Handle the "TurnCriminal" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleTurnCriminal(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 12/26/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim tUser As Short
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) Then
                    Call LogGM(.name, "/CONDEN " & UserName)

                    tUser = NameIndex(UserName)
                    If tUser > 0 Then Call VolverCriminal(tUser)
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleTurnCriminal: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handle the "ResetFactions" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleResetFactions(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 06/09/09
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
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

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) Then
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
                            Call _
                                WriteConsoleMsg(UserIndex, "El personaje " & UserName & " no existe.",
                                                FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleResetFactions: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handle the "RemoveCharFromGuild" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleRemoveCharFromGuild(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 12/26/06
        '
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim GuildIndex As Short
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) Then
                    Call LogGM(.name, "/RAJARCLAN " & UserName)

                    GuildIndex = m_EcharMiembroDeClan(UserIndex, UserName)

                    If GuildIndex = 0 Then
                        Call _
                            WriteConsoleMsg(UserIndex, "No pertenece a ningún clan o es fundador.",
                                            FontTypeNames.FONTTYPE_INFO)
                    Else
                        Call WriteConsoleMsg(UserIndex, "Expulsado.", FontTypeNames.FONTTYPE_INFO)
                        Call _
                            SendData(SendTarget.ToGuildMembers, GuildIndex,
                                     PrepareMessageConsoleMsg(
                                         UserName & " ha sido expulsado del clan por los administradores del servidor.",
                                         FontTypeNames.FONTTYPE_GUILD))
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleRemoveCharFromGuild: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handle the "RequestCharMail" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleRequestCharMail(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 12/26/06
        'Request user mail
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim UserName As String
            Dim mail As String
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()


                UserName = buffer.ReadASCIIString()

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) Then
                    If FileExist(CharPath & UserName & ".chr") Then
                        mail = GetVar(CharPath & UserName & ".chr", "CONTACTO", "email")

                        Call _
                            WriteConsoleMsg(UserIndex, "Last email de " & UserName & ":" & mail,
                                            FontTypeNames.FONTTYPE_INFO)
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleRequestCharMail: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handle the "SystemMessage" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleSystemMessage(UserIndex As Short)
        '***************************************************
        'Author: Lucas Tavolaro Ortiz (Tavo)
        'Last Modification: 12/29/06
        'Send a message to all the users
        '***************************************************
        If UserList(UserIndex).incomingData.length < 3 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try
            Dim buffer As New clsByteQueue
            Dim message As String
            With UserList(UserIndex)
                'This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                Call buffer.CopyBuffer(.incomingData)

                'Remove packet ID
                Call buffer.ReadByte()

                message = buffer.ReadASCIIString()

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) Then
                    Call LogGM(.name, "Mensaje de sistema:" & message)

                    Call SendData(SendTarget.ToAll, 0, PrepareMessageShowMessageBox(message))
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleSystemMessage: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handle the "SetMOTD" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleSetMOTD(UserIndex As Short)
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

        Try
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

                If _
                    (Not .flags.Privilegios And PlayerType.RoleMaster) <> 0 And
                    (.flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios)) Then
                    Call LogGM(.name, "Ha fijado un nuevo MOTD")

                    MaxLines = UBound(auxiliaryString) + 1

                    'UPGRADE_WARNING: El límite inferior de la matriz MOTD ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                    ReDim MOTD(MaxLines)

                    Call _
                        WriteVar(AppDomain.CurrentDomain.BaseDirectory & "Dat/Motd.ini", "INIT", "NumLines",
                                 CStr(MaxLines))

                    For LoopC = 1 To MaxLines
                        Call _
                            WriteVar(AppDomain.CurrentDomain.BaseDirectory & "Dat/Motd.ini", "Motd",
                                     "Line" & CStr(LoopC),
                                     auxiliaryString(LoopC - 1))

                        MOTD(LoopC).texto = auxiliaryString(LoopC - 1)
                    Next LoopC

                    Call WriteConsoleMsg(UserIndex, "Se ha cambiado el MOTD con éxito.", FontTypeNames.FONTTYPE_INFO)
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleSetMOTD: " & ex.Message)
        End Try
    End Sub

    ''
    ' Handle the "ChangeMOTD" message
    '
    ' @param userIndex The index of the user sending the message

    Public Sub HandleChangeMOTD(UserIndex As Short)
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

            If _
                (.flags.Privilegios And
                 (PlayerType.RoleMaster Or PlayerType.User Or
                  PlayerType.Consejero Or PlayerType.SemiDios)) Then
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

    Public Sub HandlePing(UserIndex As Short)
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

    Public Sub HandleSetIniVar(UserIndex As Short)
        '***************************************************
        'Author: Brian Chaia (BrianPr)
        'Last Modification: 01/23/10 (Marco)
        'Modify server.ini
        '***************************************************
        If UserList(UserIndex).incomingData.length < 6 Then
            Err.Raise(UserList(UserIndex).incomingData.NotEnoughDataErrCode)
            Exit Sub
        End If

        Try

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

                If .flags.Privilegios And (PlayerType.Admin Or PlayerType.Dios) Then

                    'No podemos modificar [INIT]Dioses ni [Dioses]*
                    If (UCase(sLlave) = "INIT" And UCase(sClave) = "DIOSES") Or UCase(sLlave) = "DIOSES" Then
                        Call _
                            WriteConsoleMsg(UserIndex, "¡No puedes modificar esa información desde aquí!",
                                            FontTypeNames.FONTTYPE_INFO)
                    Else
                        'Obtengo el valor según llave y clave
                        sTmp = GetVar(IniPath & "Server.ini", sLlave, sClave)

                        'Si obtengo un valor escribo en el server.ini
                        If migr_LenB(sTmp) Then
                            Call WriteVar(IniPath & "Server.ini", sLlave, sClave, sValor)
                            Call _
                                LogGM(.name,
                                      "Modificó en server.ini (" & sLlave & " " & sClave & ") el valor " & sTmp &
                                      " por " &
                                      sValor)
                            Call _
                                WriteConsoleMsg(UserIndex,
                                                "Modificó " & sLlave & " " & sClave & " a " & sValor &
                                                ". Valor anterior " &
                                                sTmp, FontTypeNames.FONTTYPE_INFO)
                        Else
                            Call WriteConsoleMsg(UserIndex, "No existe la llave y/o clave", FontTypeNames.FONTTYPE_INFO)
                        End If
                    End If
                End If

                'If we got here then packet is complete, copy data back to original queue
                Call .incomingData.CopyBuffer(buffer)
            End With


        Catch ex As Exception
            Console.WriteLine("Error in HandleSetIniVar: " & ex.Message)
            'UPGRADE_NOTE: error se actualizó a error_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            Dim error_Renamed As Integer
        End Try
    End Sub

    ''
    ' Writes the "Logged" message to the given user's outgoing data buffer.
    '
    ' @param    UserIndex User to which the message is intended.
    ' @remarks  The data is not actually sent until the buffer is properly flushed.

    Public Sub WriteLoggedMessage(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "Logged" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.Logged)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteRemoveAllDialogs(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "RemoveDialogs" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.RemoveDialogs)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteRemoveCharDialog(UserIndex As Short, CharIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "RemoveCharDialog" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageRemoveCharDialog(CharIndex))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteNavigateToggle(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "NavigateToggle" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.NavigateToggle)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteDisconnect(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "Disconnect" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.Disconnect)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUserOfferConfirm(UserIndex As Short)
        '***************************************************
        'Author: ZaMa
        'Last Modification: 14/12/2009
        'Writes the "UserOfferConfirm" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.UserOfferConfirm)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteCommerceEnd(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "CommerceEnd" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.CommerceEnd)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteBankEnd(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "BankEnd" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.BankEnd)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteCommerceInit(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "CommerceInit" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.CommerceInit)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteBankInit(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "BankInit" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.BankInit)
                Call UserList(UserIndex).outgoingData.WriteLong(UserList(UserIndex).Stats.Banco)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUserCommerceInit(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "UserCommerceInit" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.UserCommerceInit)
                Call UserList(UserIndex).outgoingData.WriteASCIIString(UserList(UserIndex).ComUsu.DestNick)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUserCommerceEnd(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "UserCommerceEnd" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.UserCommerceEnd)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteShowBlacksmithForm(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ShowBlacksmithForm" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.ShowBlacksmithForm)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteShowCarpenterForm(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ShowCarpenterForm" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.ShowCarpenterForm)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUpdateSta(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "UpdateMana" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.UpdateSta)
                    Call .WriteInteger(UserList(UserIndex).Stats.MinSta)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUpdateMana(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "UpdateMana" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.UpdateMana)
                    Call .WriteInteger(UserList(UserIndex).Stats.MinMAN)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUpdateHP(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "UpdateMana" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.UpdateHP)
                    Call .WriteInteger(UserList(UserIndex).Stats.MinHp)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUpdateGold(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "UpdateGold" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.UpdateGold)
                    Call .WriteLong(UserList(UserIndex).Stats.GLD)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUpdateBankGold(UserIndex As Short)
        '***************************************************
        'Author: ZaMa
        'Last Modification: 14/12/2009
        'Writes the "UpdateBankGold" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.UpdateBankGold)
                    Call .WriteLong(UserList(UserIndex).Stats.Banco)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUpdateExp(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "UpdateExp" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.UpdateExp)
                    Call .WriteLong(UserList(UserIndex).Stats.Exp)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUpdateStrenghtAndDexterity(UserIndex As Short)
        '***************************************************
        'Author: Budi
        'Last Modification: 11/26/09
        'Writes the "UpdateStrenghtAndDexterity" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.UpdateStrenghtAndDexterity)
                    Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(eAtributos.Fuerza))
                    Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(eAtributos.Agilidad))
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUpdateDexterity(UserIndex As Short)
        '***************************************************
        'Author: Budi
        'Last Modification: 11/26/09
        'Writes the "UpdateStrenghtAndDexterity" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.UpdateDexterity)
                    Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(eAtributos.Agilidad))
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUpdateStrenght(UserIndex As Short)
        '***************************************************
        'Author: Budi
        'Last Modification: 11/26/09
        'Writes the "UpdateStrenghtAndDexterity" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.UpdateStrenght)
                    Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(eAtributos.Fuerza))
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteChangeMap(UserIndex As Short, Map As Short, version As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ChangeMap" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.ChangeMap)
                    Call .WriteInteger(Map)
                    Call .WriteInteger(version)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WritePosUpdate(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "PosUpdate" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.PosUpdate)
                    Call .WriteByte(UserList(UserIndex).Pos.X)
                    Call .WriteByte(UserList(UserIndex).Pos.Y)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteChatOverHead(UserIndex As Short, Chat As String, CharIndex As Short, color As Integer)
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call _
                                     UserList(UserIndex).outgoingData.WriteASCIIStringFixed(
                                         PrepareMessageChatOverHead(Chat, CharIndex, color))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteConsoleMsg(UserIndex As Short, Chat As String, FontIndex As FontTypeNames)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ConsoleMsg" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageConsoleMsg(Chat, FontIndex))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteCommerceChat(UserIndex As Short, Chat As String, FontIndex As FontTypeNames)
        '***************************************************
        'Author: ZaMa
        'Last Modification: 05/17/06
        'Writes the "ConsoleMsg" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareCommerceConsoleMsg(Chat, FontIndex))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteGuildChat(UserIndex As Short, Chat As String)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "GuildChat" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageGuildChat(Chat))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteShowMessageBox(UserIndex As Short, message As String)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ShowMessageBox" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.ShowMessageBox)
                    Call .WriteASCIIString(message)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUserIndexInServer(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "UserIndexInServer" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.UserIndexInServer)
                    Call .WriteInteger(UserIndex)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUserCharIndexInServer(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "UserIndexInServer" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.UserCharIndexInServer)
                    Call .WriteInteger(UserList(UserIndex).Char_Renamed.CharIndex)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteCharacterCreate(UserIndex As Short, body As Short, Head As Short,
                                    heading As eHeading, CharIndex As Short, X As Byte,
                                    Y As Byte, weapon As Short, shield As Short, FX As Short,
                                    FXLoops As Short, helmet As Short, name As String,
                                    NickColor As Byte, Privileges As Byte)
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call _
                                     UserList(UserIndex).outgoingData.WriteASCIIStringFixed(
                                         PrepareMessageCharacterCreate(body, Head, heading,
                                                                       CharIndex, X, Y, weapon,
                                                                       shield, FX, FXLoops,
                                                                       helmet, name, NickColor,
                                                                       Privileges))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteCharacterRemove(UserIndex As Short, CharIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "CharacterRemove" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageCharacterRemove(CharIndex))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteCharacterMove(UserIndex As Short, CharIndex As Short, X As Byte, Y As Byte)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "CharacterMove" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call _
                                     UserList(UserIndex).outgoingData.WriteASCIIStringFixed(
                                         PrepareMessageCharacterMove(CharIndex, X, Y))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteForceCharMove(UserIndex As Object, Direccion As eHeading)
        '***************************************************
        'Author: ZaMa
        'Last Modification: 26/03/2009
        'Writes the "ForceCharMove" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageForceCharMove(Direccion))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteCharacterChange(UserIndex As Short, body As Short, Head As Short,
                                    heading As eHeading, CharIndex As Short,
                                    weapon As Short, shield As Short, FX As Short,
                                    FXLoops As Short, helmet As Short)
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call _
                                     UserList(UserIndex).outgoingData.WriteASCIIStringFixed(
                                         PrepareMessageCharacterChange(body, Head, heading,
                                                                       CharIndex, weapon,
                                                                       shield, FX, FXLoops,
                                                                       helmet))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteObjectCreate(UserIndex As Short, GrhIndex As Short, X As Byte, Y As Byte)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ObjectCreate" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageObjectCreate(GrhIndex, X, Y))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteObjectDelete(UserIndex As Short, X As Byte, Y As Byte)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ObjectDelete" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageObjectDelete(X, Y))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteBlockPosition(UserIndex As Short, X As Byte, Y As Byte, Blocked As Boolean)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "BlockPosition" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.BlockPosition)
                    Call .WriteByte(X)
                    Call .WriteByte(Y)
                    Call .WriteBoolean(Blocked)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WritePlayMidi(UserIndex As Short, midi As Byte, Optional ByVal loops As Short = - 1)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "PlayMidi" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessagePlayMidi(midi, loops))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WritePlayWave(UserIndex As Short, wave As Byte, X As Byte, Y As Byte)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 08/08/07
        'Last Modified by: Rapsodius
        'Added X and Y positions for 3D Sounds
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessagePlayWave(wave, X, Y))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteGuildList(UserIndex As Short, guildList() As String)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "GuildList" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim Tmp As String
                Dim i As Integer
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.guildList)
                    For i = LBound(guildList) To UBound(guildList)
                        Tmp = Tmp & guildList(i) & SEPARATOR
                    Next i
                    If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
                    Call .WriteASCIIString(Tmp)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteAreaChanged(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "AreaChanged" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.AreaChanged)
                    Call .WriteByte(UserList(UserIndex).Pos.X)
                    Call .WriteByte(UserList(UserIndex).Pos.Y)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WritePauseToggle(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "PauseToggle" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessagePauseToggle())
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteRainToggle(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "RainToggle" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageRainToggle())
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteCreateFX(UserIndex As Short, CharIndex As Short, FX As Short,
                             FXLoops As Short)
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call _
                                     UserList(UserIndex).outgoingData.WriteASCIIStringFixed(
                                         PrepareMessageCreateFX(CharIndex, FX, FXLoops))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUpdateUserStats(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "UpdateUserStats" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
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
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteWorkRequestTarget(UserIndex As Short, Skill As eSkill)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "WorkRequestTarget" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.WorkRequestTarget)
                    Call .WriteByte(Skill)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteChangeInventorySlot(UserIndex As Short, Slot As Byte)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 3/12/09
        'Writes the "ChangeInventorySlot" message to the given user's outgoing data buffer
        '3/12/09: Budi - Ahora se envia MaxDef y MinDef en lugar de Def
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim ObjIndex As Short
                Dim obData As ObjData
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.ChangeInventorySlot)
                    Call .WriteByte(Slot)
                    ObjIndex = UserList(UserIndex).Invent.Object_Renamed(Slot).ObjIndex
                    If ObjIndex > 0 Then
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
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteAddSlots(UserIndex As Short, Mochila As eMochilas)
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

    Public Sub WriteChangeBankSlot(UserIndex As Short, Slot As Byte)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 12/03/09
        'Writes the "ChangeBankSlot" message to the given user's outgoing data buffer
        '12/03/09: Budi - Ahora se envia MaxDef y MinDef en lugar de sólo Def
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim ObjIndex As Short
                Dim obData As ObjData
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.ChangeBankSlot)
                    Call .WriteByte(Slot)
                    ObjIndex = UserList(UserIndex).BancoInvent.Object_Renamed(Slot).ObjIndex
                    Call .WriteInteger(ObjIndex)
                    If ObjIndex > 0 Then
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
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteChangeSpellSlot(UserIndex As Short, Slot As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ChangeSpellSlot" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
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
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteAttributes(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "Atributes" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.Atributes)
                    Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(eAtributos.Fuerza))
                    Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(eAtributos.Agilidad))
                    Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(eAtributos.Inteligencia))
                    Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(eAtributos.Carisma))
                    Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(eAtributos.Constitucion))
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteBlacksmithWeapons(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 04/15/2008 (NicoNZ) Habia un error al fijarse los skills del personaje
        'Writes the "BlacksmithWeapons" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim i As Integer
                Dim Obj_Renamed As ObjData
                Dim validIndexes() As Short
                Dim Count As Short
                ReDim validIndexes(UBound(ArmasHerrero))
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.BlacksmithWeapons)
                    For i = 1 To UBound(ArmasHerrero)
                        If _
                                     ObjData_Renamed(ArmasHerrero(i)).SkHerreria <=
                                     Math.Round(
                                         UserList(UserIndex).Stats.UserSkills(eSkill.Herreria)/
                                         ModHerreriA(UserList(UserIndex).clase), 0) Then
                            Count = Count + 1
                            validIndexes(Count) = i
                        End If
                    Next i
                    Call .WriteInteger(Count)
                    For i = 1 To Count
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
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteBlacksmithArmors(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 04/15/2008 (NicoNZ) Habia un error al fijarse los skills del personaje
        'Writes the "BlacksmithArmors" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim i As Integer
                Dim Obj_Renamed As ObjData
                Dim validIndexes() As Short
                Dim Count As Short
                ReDim validIndexes(UBound(ArmadurasHerrero))
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.BlacksmithArmors)
                    For i = 1 To UBound(ArmadurasHerrero)
                        If _
                                     ObjData_Renamed(ArmadurasHerrero(i)).SkHerreria <=
                                     Math.Round(
                                         UserList(UserIndex).Stats.UserSkills(eSkill.Herreria)/
                                         ModHerreriA(UserList(UserIndex).clase), 0) Then
                            Count = Count + 1
                            validIndexes(Count) = i
                        End If
                    Next i
                    Call .WriteInteger(Count)
                    For i = 1 To Count
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
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteCarpenterObjects(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "CarpenterObjects" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim i As Integer
                Dim Obj_Renamed As ObjData
                Dim validIndexes() As Short
                Dim Count As Short
                ReDim validIndexes(UBound(ObjCarpintero))
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.CarpenterObjects)
                    For i = 1 To UBound(ObjCarpintero)
                        If _
                                     ObjData_Renamed(ObjCarpintero(i)).SkCarpinteria <=
                                     UserList(UserIndex).Stats.UserSkills(eSkill.Carpinteria)\
                                     ModCarpinteria(UserList(UserIndex).clase) Then
                            Count = Count + 1
                            validIndexes(Count) = i
                        End If
                    Next i
                    Call .WriteInteger(Count)
                    For i = 1 To Count
                        Obj_Renamed = ObjData_Renamed(ObjCarpintero(validIndexes(i)))
                        Call .WriteASCIIString(Obj_Renamed.name)
                        Call .WriteInteger(Obj_Renamed.GrhIndex)
                        Call .WriteInteger(Obj_Renamed.Madera)
                        Call .WriteInteger(Obj_Renamed.MaderaElfica)
                        Call .WriteInteger(ObjCarpintero(validIndexes(i)))
                        Call .WriteInteger(Obj_Renamed.Upgrade)
                    Next i
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteRestOK(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "RestOK" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.RestOK)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteErrorMsg(UserIndex As Short, message As String)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ErrorMsg" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteASCIIStringFixed(PrepareMessageErrorMsg(message))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteBlind(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "Blind" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.Blind)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteDumb(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "Dumb" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.Dumb)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteShowSignal(UserIndex As Short, ObjIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ShowSignal" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.ShowSignal)
                    Call .WriteASCIIString(ObjData_Renamed(ObjIndex).texto)
                    Call .WriteInteger(ObjData_Renamed(ObjIndex).GrhSecundario)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteChangeNPCInventorySlot(UserIndex As Short, Slot As Byte, Obj As Obj, price As Single)
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim ObjInfo As ObjData
                If Obj.ObjIndex >= LBound(ObjData_Renamed) And Obj.ObjIndex <= UBound(ObjData_Renamed) Then
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
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUpdateHungerAndThirst(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "UpdateHungerAndThirst" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.UpdateHungerAndThirst)
                    Call .WriteByte(UserList(UserIndex).Stats.MaxAGU)
                    Call .WriteByte(UserList(UserIndex).Stats.MinAGU)
                    Call .WriteByte(UserList(UserIndex).Stats.MaxHam)
                    Call .WriteByte(UserList(UserIndex).Stats.MinHam)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteFame(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "Fame" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
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
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteMiniStats(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "MiniStats" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.MiniStats)
                    Call .WriteLong(UserList(UserIndex).Faccion.CiudadanosMatados)
                    Call .WriteLong(UserList(UserIndex).Faccion.CriminalesMatados)
                    Call .WriteLong(UserList(UserIndex).Stats.UsuariosMatados)
                    Call .WriteInteger(UserList(UserIndex).Stats.NPCsMuertos)
                    Call .WriteByte(UserList(UserIndex).clase)
                    Call .WriteLong(UserList(UserIndex).Counters.Pena)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteLevelUp(UserIndex As Short, skillPoints As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "LevelUp" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.LevelUp)
                    Call .WriteInteger(skillPoints)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteAddForumMsg(UserIndex As Short, ForumType As eForumType, Title As String, Author As String,
                                message As String)
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.AddForumMsg)
                    Call .WriteByte(ForumType)
                    Call .WriteASCIIString(Title)
                    Call .WriteASCIIString(Author)
                    Call .WriteASCIIString(message)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteShowForumForm(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ShowForumForm" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim Visibilidad As Byte
                Dim CanMakeSticky As Byte
                With UserList(UserIndex)
                    Call .outgoingData.WriteByte(ServerPacketID.ShowForumForm)
                    Visibilidad = eForumVisibility.ieGENERAL_MEMBER
                    If esCaos(UserIndex) Or EsGM(UserIndex) Then
                        Visibilidad = Visibilidad Or eForumVisibility.ieCAOS_MEMBER
                    End If
                    If esArmada(UserIndex) Or EsGM(UserIndex) Then
                        Visibilidad = Visibilidad Or eForumVisibility.ieREAL_MEMBER
                    End If
                    Call .outgoingData.WriteByte(Visibilidad)
                    If EsGM(UserIndex) Then
                        CanMakeSticky = 2
                    ElseIf (.flags.Privilegios And PlayerType.ChaosCouncil) <> 0 Then
                        CanMakeSticky = 1
                    ElseIf (.flags.Privilegios And PlayerType.RoyalCouncil) <> 0 Then
                        CanMakeSticky = 1
                    End If
                    Call .outgoingData.WriteByte(CanMakeSticky)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteSetInvisible(UserIndex As Short, CharIndex As Short, invisible As Boolean)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "SetInvisible" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call _
                                     UserList(UserIndex).outgoingData.WriteASCIIStringFixed(
                                         PrepareMessageSetInvisible(CharIndex, invisible))
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteDiceRoll(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "DiceRoll" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.DiceRoll)
                    Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(eAtributos.Fuerza))
                    Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(eAtributos.Agilidad))
                    Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(eAtributos.Inteligencia))
                    Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(eAtributos.Carisma))
                    Call .WriteByte(UserList(UserIndex).Stats.UserAtributos(eAtributos.Constitucion))
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteMeditateToggle(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "MeditateToggle" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.MeditateToggle)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteBlindNoMore(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "BlindNoMore" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.BlindNoMore)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteDumbNoMore(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "DumbNoMore" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.DumbNoMore)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteSendSkills(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 11/19/09
        'Writes the "SendSkills" message to the given user's outgoing data buffer
        '11/19/09: Pato - Now send the percentage of progress of the skills.
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim i As Integer
                With UserList(UserIndex)
                    Call .outgoingData.WriteByte(ServerPacketID.SendSkills)
                    Call .outgoingData.WriteByte(.clase)
                    For i = 1 To NUMSKILLS
                        Call .outgoingData.WriteByte(UserList(UserIndex).Stats.UserSkills(i))
                        If .Stats.UserSkills(i) < MAXSKILLPOINTS Then
                            Call .outgoingData.WriteByte(Int(.Stats.ExpSkills(i)*100/.Stats.EluSkills(i)))
                        Else
                            Call .outgoingData.WriteByte(0)
                        End If
                    Next i
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteTrainerCreatureList(UserIndex As Short, NpcIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "TrainerCreatureList" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim i As Integer
                Dim str_Renamed As String
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.TrainerCreatureList)
                    For i = 1 To Npclist(NpcIndex).NroCriaturas
                        str_Renamed = str_Renamed & Npclist(NpcIndex).Criaturas(i).NpcName & SEPARATOR
                    Next i
                    If migr_LenB(str_Renamed) > 0 Then str_Renamed = Left(str_Renamed, Len(str_Renamed) - 1)
                    Call .WriteASCIIString(str_Renamed)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteGuildNews(UserIndex As Short, guildNews As String, enemies() As String, allies() As String)
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim i As Integer
                Dim Tmp As String
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.guildNews)
                    Call .WriteASCIIString(guildNews)
                    For i = LBound(enemies) To UBound(enemies)
                        Tmp = Tmp & enemies(i) & SEPARATOR
                    Next i
                    If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
                    Call .WriteASCIIString(Tmp)
                    Tmp = vbNullString
                    For i = LBound(allies) To UBound(allies)
                        Tmp = Tmp & allies(i) & SEPARATOR
                    Next i
                    If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
                    Call .WriteASCIIString(Tmp)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteOfferDetails(UserIndex As Short, details As String)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "OfferDetails" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim i As Integer
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.OfferDetails)
                    Call .WriteASCIIString(details)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteAlianceProposalsList(UserIndex As Short, guilds() As String)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "AlianceProposalsList" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim i As Integer
                Dim Tmp As String
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.AlianceProposalsList)
                    For i = LBound(guilds) To UBound(guilds)
                        Tmp = Tmp & guilds(i) & SEPARATOR
                    Next i
                    If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
                    Call .WriteASCIIString(Tmp)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WritePeaceProposalsList(UserIndex As Short, guilds() As String)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "PeaceProposalsList" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim i As Integer
                Dim Tmp As String
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.PeaceProposalsList)
                    For i = LBound(guilds) To UBound(guilds)
                        Tmp = Tmp & guilds(i) & SEPARATOR
                    Next i
                    If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
                    Call .WriteASCIIString(Tmp)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteCharacterInfo(UserIndex As Short, charName As String, race As eRaza, Class_Renamed As eClass,
                                  gender As eGenero,
                                  level As Byte, gold As Integer, bank As Integer,
                                  reputation As Integer, previousPetitions As String,
                                  currentGuild As String, previousGuilds As String,
                                  RoyalArmy As Boolean, CaosLegion As Boolean,
                                  citicensKilled As Integer, criminalsKilled As Integer)
        RetryOnceIfNotEnoughSpace(
            Sub()
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
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteGuildLeaderInfo(UserIndex As Short, guildList() As String, MemberList() As String,
                                    guildNews As String, joinRequests() As String)
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim i As Integer
                Dim Tmp As String
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.GuildLeaderInfo)
                    For i = LBound(guildList) To UBound(guildList)
                        Tmp = Tmp & guildList(i) & SEPARATOR
                    Next i
                    If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
                    Call .WriteASCIIString(Tmp)
                    Tmp = vbNullString
                    For i = LBound(MemberList) To UBound(MemberList)
                        Tmp = Tmp & MemberList(i) & SEPARATOR
                    Next i
                    If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
                    Call .WriteASCIIString(Tmp)
                    Call .WriteASCIIString(guildNews)
                    Tmp = vbNullString
                    For i = LBound(joinRequests) To UBound(joinRequests)
                        Tmp = Tmp & joinRequests(i) & SEPARATOR
                    Next i
                    If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
                    Call .WriteASCIIString(Tmp)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteGuildMemberInfo(UserIndex As Short, guildList() As String, MemberList() As String)
        '***************************************************
        'Author: ZaMa
        'Last Modification: 21/02/2010
        'Writes the "GuildMemberInfo" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim i As Integer
                Dim Tmp As String
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.GuildMemberInfo)
                    For i = LBound(guildList) To UBound(guildList)
                        Tmp = Tmp & guildList(i) & SEPARATOR
                    Next i
                    If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
                    Call .WriteASCIIString(Tmp)
                    Tmp = vbNullString
                    For i = LBound(MemberList) To UBound(MemberList)
                        Tmp = Tmp & MemberList(i) & SEPARATOR
                    Next i
                    If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
                    Call .WriteASCIIString(Tmp)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteGuildDetails(UserIndex As Short, GuildName As String, founder As String, foundationDate As String,
                                 leader As String, URL As String,
                                 memberCount As Short, electionsOpen As Boolean, alignment As String,
                                 enemiesCount As Short, AlliesCount As Short,
                                 antifactionPoints As String, codex() As String, guildDesc As String)
        RetryOnceIfNotEnoughSpace(
            Sub()
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
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteShowGuildAlign(UserIndex As Short)
        '***************************************************
        'Author: ZaMa
        'Last Modification: 14/12/2009
        'Writes the "ShowGuildAlign" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.ShowGuildAlign)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteShowGuildFundationForm(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ShowGuildFundationForm" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.ShowGuildFundationForm)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteParalizeOK(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 08/12/07
        'Last Modified By: Lucas Tavolaro Ortiz (Tavo)
        'Writes the "ParalizeOK" message to the given user's outgoing data buffer
        'And updates user position
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.ParalizeOK)
                Call WritePosUpdate(UserIndex)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteShowUserRequest(UserIndex As Short, details As String)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ShowUserRequest" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.ShowUserRequest)
                    Call .WriteASCIIString(details)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteTradeOK(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "TradeOK" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.TradeOK)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteBankOK(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "BankOK" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.BankOK)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteChangeUserTradeSlot(UserIndex As Short, OfferSlot As Byte, ObjIndex As Short, Amount As Integer)
        RetryOnceIfNotEnoughSpace(
            Sub()
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
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteSendNight(UserIndex As Short, night As Boolean)
        '***************************************************
        'Author: Fredy Horacio Treboux (liquid)
        'Last Modification: 01/08/07
        'Writes the "SendNight" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.SendNight)
                    Call .WriteBoolean(night)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteSpawnList(UserIndex As Short, npcNames() As String)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "SpawnList" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
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
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteShowSOSForm(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ShowSOSForm" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim i As Integer
                Dim Tmp As String
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.ShowSOSForm)
                    For i = 0 To Ayuda.Longitud - 1
                        Tmp = Tmp & Ayuda.VerElemento(i) & SEPARATOR
                    Next i
                    If migr_LenB(Tmp) <> 0 Then Tmp = Left(Tmp, Len(Tmp) - 1)
                    Call .WriteASCIIString(Tmp)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteShowPartyForm(UserIndex As Short)
        '***************************************************
        'Author: Budi
        'Last Modification: 11/26/09
        'Writes the "ShowPartyForm" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
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
                                Tmp = Tmp & UserList(members(i)).name & " (" &
                                      Fix(Parties(PI).MiExperiencia(members(i))) & ")" &
                                      SEPARATOR
                            End If
                        Next i
                    End If
                    If migr_LenB(Tmp) <> 0 Then Tmp = Left(Tmp, Len(Tmp) - 1)
                    Call .WriteASCIIString(Tmp)
                    Call .WriteLong(Parties(PI).ObtenerExperienciaTotal)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteShowMOTDEditionForm(UserIndex As Short, currentMOTD As String)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ShowMOTDEditionForm" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.ShowMOTDEditionForm)
                    Call .WriteASCIIString(currentMOTD)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteShowGMPanelForm(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "ShowGMPanelForm" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.ShowGMPanelForm)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteUserNameList(UserIndex As Short, userNamesList() As String, cant As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06 NIGO:
        'Writes the "UserNameList" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Dim i As Integer
                Dim Tmp As String
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.UserNameList)
                    For i = 1 To cant
                        Tmp = Tmp & userNamesList(i) & SEPARATOR
                    Next i
                    If Len(Tmp) Then Tmp = Left(Tmp, Len(Tmp) - 1)
                    Call .WriteASCIIString(Tmp)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub


    ''
    ' Writes the "Pong" message to the given user's outgoing data buffer.
    '
    ' @param    UserIndex User to which the message is intended.
    ' @remarks  The data is not actually sent until the buffer is properly flushed.

    Public Sub WritePong(UserIndex As Short)
        '***************************************************
        'Author: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Writes the "Pong" message to the given user's outgoing data buffer
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.Pong)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    ''
    ' Flushes the outgoing data buffer of the user.
    '
    ' @param    UserIndex User whose outgoing data buffer will be flushed.

    Public Sub FlushBuffer(UserIndex As Short)
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

    Public Function PrepareMessageSetInvisible(CharIndex As Short, invisible As Boolean) As String
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

    Public Function PrepareMessageCharacterChangeNick(CharIndex As Short, newNick As String) As String
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

    Public Function PrepareMessageChatOverHead(Chat As String, CharIndex As Short, color As Integer) _
        As String
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
            Call .WriteByte((color And &HFF00)\&H100)
            Call .WriteByte((color And &HFF0000)\&H10000)

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

    Public Function PrepareMessageConsoleMsg(Chat As String, FontIndex As FontTypeNames) As String
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

    Public Function PrepareCommerceConsoleMsg(ByRef Chat As String, FontIndex As FontTypeNames) As String
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

    Public Function PrepareMessageCreateFX(CharIndex As Short, FX As Short, FXLoops As Short) _
        As String
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

    Public Function PrepareMessagePlayWave(wave As Byte, X As Byte, Y As Byte) As String
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

    Public Function PrepareMessageGuildChat(Chat As String) As String
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

    Public Function PrepareMessageShowMessageBox(Chat As String) As String
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

    Public Function PrepareMessagePlayMidi(midi As Byte, Optional ByVal loops As Short = - 1) As String
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

    Public Function PrepareMessageObjectDelete(X As Byte, Y As Byte) As String
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

    Public Function PrepareMessageBlockPosition(X As Byte, Y As Byte, Blocked As Boolean) As String
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

    Public Function PrepareMessageObjectCreate(GrhIndex As Short, X As Byte, Y As Byte) As String
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

    Public Function PrepareMessageCharacterRemove(CharIndex As Short) As String
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

    Public Function PrepareMessageRemoveCharDialog(CharIndex As Short) As String
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

    Public Function PrepareMessageCharacterCreate(body As Short, Head As Short,
                                                  heading As eHeading, CharIndex As Short,
                                                  X As Byte, Y As Byte, weapon As Short,
                                                  shield As Short, FX As Short, FXLoops As Short,
                                                  helmet As Short, name As String, NickColor As Byte,
                                                  Privileges As Byte) As String
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

    Public Function PrepareMessageCharacterChange(body As Short, Head As Short,
                                                  heading As eHeading, CharIndex As Short,
                                                  weapon As Short, shield As Short, FX As Short,
                                                  FXLoops As Short, helmet As Short) As String
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

    Public Function PrepareMessageCharacterMove(CharIndex As Short, X As Byte, Y As Byte) As String
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

    Public Function PrepareMessageForceCharMove(Direccion As eHeading) As String
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

    Public Function PrepareMessageUpdateTagAndStatus(UserIndex As Short, NickColor As Byte,
                                                     ByRef Tag As String) As String
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

    Public Function PrepareMessageErrorMsg(message As String) As String
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
    Public Sub WriteStopWorking(UserIndex As Short)
        '***************************************************
        'Author: ZaMa
        'Last Modification: 21/02/2010
        '
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                Call UserList(UserIndex).outgoingData.WriteByte(ServerPacketID.StopWorking)
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Public Sub WriteCancelOfferItem(UserIndex As Short, Slot As Byte)
        '***************************************************
        'Author: Torres Patricio (Pato)
        'Last Modification: 05/03/2010
        '
        '***************************************************
        RetryOnceIfNotEnoughSpace(
            Sub()
                With UserList(UserIndex).outgoingData
                    Call .WriteByte(ServerPacketID.CancelOfferItem)
                    Call .WriteByte(Slot)
                End With
            End Sub,
            Sub()
                FlushBuffer(UserIndex)
            End Sub
            )
    End Sub

    Private Sub RetryOnceIfNotEnoughSpace(tryAction As Action, onRetry As Action)
        Try
            tryAction()
        Catch ex As Exception
            onRetry()
            tryAction() ' Reintenta una sola vez
        End Try
    End Sub
End Module
