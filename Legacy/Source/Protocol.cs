using System;
using System.Drawing;
using System.IO;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Legacy;

internal static class Protocol
{
    public enum eEditOptions : byte
    {
        eo_Gold = 1,
        eo_Experience,
        eo_Body,
        eo_Head,
        eo_CiticensKilled,
        eo_CriminalsKilled,
        eo_Level,
        eo_Class,
        eo_Skills,
        eo_SkillPointsLeft,
        eo_Nobleza,
        eo_Asesino,
        eo_Sex,
        eo_Raza,
        eo_addGold
    }

    public enum FontTypeNames
    {
        FONTTYPE_TALK,
        FONTTYPE_FIGHT,
        FONTTYPE_WARNING,
        FONTTYPE_INFO,
        FONTTYPE_INFOBOLD,
        FONTTYPE_EJECUCION,
        FONTTYPE_PARTY,
        FONTTYPE_VENENO,
        FONTTYPE_GUILD,
        FONTTYPE_SERVER,
        FONTTYPE_GUILDMSG,
        FONTTYPE_CONSEJO,
        FONTTYPE_CONSEJOCAOS,
        FONTTYPE_CONSEJOVesA,
        FONTTYPE_CONSEJOCAOSVesA,
        FONTTYPE_CENTINELA,
        FONTTYPE_GMMSG,
        FONTTYPE_GM,
        FONTTYPE_CITIZEN,
        FONTTYPE_CONSE,
        FONTTYPE_DIOS
    }

    // '
    // The last existing client packet id.
    private const byte LAST_CLIENT_PACKET_ID = 128;
    // **************************************************************
    // Protocol.bas - Handles all incoming / outgoing messages for client-server communications.
    // Uses a binary protocol designed by myself.
    // 
    // Designed and implemented by Juan Martín Sotuyo Dodero (Maraxus)
    // (juansotuyo@gmail.com)
    // **************************************************************

    // '
    // Handles all incoming / outgoing packets for client - server communications
    // The binary prtocol here used was designed by Juan Martín Sotuyo Dodero.
    // This is the first time it's used in Alkon, though the second time it's coded.
    // This implementation has several enhacements from the first design.
    // 
    // @author Juan Martín Sotuyo Dodero (Maraxus) juansotuyo@gmail.com
    // @version 1.0.0
    // @date 20060517

    // '
    // When we have a list of strings, we use this to separate them and prevent
    // having too many string lengths in the queue. Yes, each string is NULL-terminated :P
    // UPGRADE_NOTE: SEPARATOR ha cambiado de Constant a Variable. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C54B49D7-5804-4D48-834B-B3D81E4C2F13"'
    private static readonly char SEPARATOR = '\0';

    // '
    // Auxiliar ByteQueue used as buffer to generate messages not intended to be sent right away.
    // Specially usefull to create a message once and send it over to several clients.
    private static clsByteQueue auxiliarBuffer = new();


    // '
    // Handles incoming data.
    // 
    // @param    userIndex The index of the user sending the message.

    public static void HandleIncomingData(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 01/09/07
        // 
        // ***************************************************
        try
        {
            byte packetID;

            packetID = Declaraciones.UserList[UserIndex].incomingData.PeekByte();

            // Does the packet requires a logged user??
            if (!((packetID == (byte)ClientPacketID.ThrowDices) | (packetID == (byte)ClientPacketID.LoginExistingChar) |
                  (packetID == (byte)ClientPacketID.LoginNewChar)))

            {
                // Is the user actually logged?
                if (!Declaraciones.UserList[UserIndex].flags.UserLogged)
                {
                    TCP.CloseSocket(UserIndex);
                    return;
                }

                // He is logged. Reset idle counter if id is valid.

                if (packetID <= LAST_CLIENT_PACKET_ID) Declaraciones.UserList[UserIndex].Counters.IdleCount = 0;
            }
            else if (packetID <= LAST_CLIENT_PACKET_ID)
            {
                Declaraciones.UserList[UserIndex].Counters.IdleCount = 0;

                // Is the user logged?
                if (Declaraciones.UserList[UserIndex].flags.UserLogged)
                {
                    TCP.CloseSocket(UserIndex);
                    return;
                }
            }

            // Ante cualquier paquete, pierde la proteccion de ser atacado.
            Declaraciones.UserList[UserIndex].flags.NoPuedeSerAtacado = false;

            switch (packetID)
            {
                case (byte)ClientPacketID.LoginExistingChar: // OLOGIN
                {
                    HandleLoginExistingChar(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.ThrowDices: // TIRDAD
                {
                    HandleThrowDices(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.LoginNewChar: // NLOGIN
                {
                    HandleLoginNewChar(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Talk: // ;
                {
                    HandleTalk(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Yell: // -
                {
                    HandleYell(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Whisper: // \
                {
                    HandleWhisper(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Walk: // M
                {
                    HandleWalk(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.RequestPositionUpdate: // RPU
                {
                    HandleRequestPositionUpdate(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Attack: // AT
                {
                    HandleAttack(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.PickUp: // AG
                {
                    HandlePickUp(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.SafeToggle: // /SEG & SEG  (SEG's behaviour has to be coded in the client)
                {
                    HandleSafeToggle(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.ResuscitationSafeToggle:
                {
                    HandleResuscitationToggle(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.RequestGuildLeaderInfo: // GLINFO
                {
                    HandleRequestGuildLeaderInfo(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.RequestAtributes: // ATR
                {
                    HandleRequestAtributes(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.RequestFame: // FAMA
                {
                    HandleRequestFame(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.RequestSkills: // ESKI
                {
                    HandleRequestSkills(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.RequestMiniStats: // FEST
                {
                    HandleRequestMiniStats(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.CommerceEnd: // FINCOM
                {
                    HandleCommerceEnd(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.CommerceChat:
                {
                    HandleCommerceChat(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.UserCommerceEnd: // FINCOMUSU
                {
                    HandleUserCommerceEnd(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.UserCommerceConfirm:
                {
                    HandleUserCommerceConfirm(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.BankEnd: // FINBAN
                {
                    HandleBankEnd(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.UserCommerceOk: // COMUSUOK
                {
                    HandleUserCommerceOk(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.UserCommerceReject: // COMUSUNO
                {
                    HandleUserCommerceReject(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Drop: // TI
                {
                    HandleDrop(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.CastSpell: // LH
                {
                    HandleCastSpell(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.LeftClick: // LC
                {
                    HandleLeftClick(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.DoubleClick: // RC
                {
                    HandleDoubleClick(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Work: // UK
                {
                    HandleWork(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.UseSpellMacro: // UMH
                {
                    HandleUseSpellMacro(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.UseItem: // USA
                {
                    HandleUseItem(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.CraftBlacksmith: // CNS
                {
                    HandleCraftBlacksmith(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.CraftCarpenter: // CNC
                {
                    HandleCraftCarpenter(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.WorkLeftClick: // WLC
                {
                    HandleWorkLeftClick(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.CreateNewGuild: // CIG
                {
                    HandleCreateNewGuild(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.SpellInfo: // INFS
                {
                    HandleSpellInfo(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.EquipItem: // EQUI
                {
                    HandleEquipItem(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.ChangeHeading: // CHEA
                {
                    HandleChangeHeading(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.ModifySkills: // SKSE
                {
                    HandleModifySkills(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Train: // ENTR
                {
                    HandleTrain(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.CommerceBuy: // COMP
                {
                    HandleCommerceBuy(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.BankExtractItem: // RETI
                {
                    HandleBankExtractItem(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.CommerceSell: // VEND
                {
                    HandleCommerceSell(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.BankDeposit: // DEPO
                {
                    HandleBankDeposit(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.ForumPost: // DEMSG
                {
                    HandleForumPost(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.MoveSpell: // DESPHE
                {
                    HandleMoveSpell(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.MoveBank:
                {
                    HandleMoveBank(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.ClanCodexUpdate: // DESCOD
                {
                    HandleClanCodexUpdate(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.UserCommerceOffer: // OFRECER
                {
                    HandleUserCommerceOffer(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildAcceptPeace: // ACEPPEAT
                {
                    HandleGuildAcceptPeace(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildRejectAlliance: // RECPALIA
                {
                    HandleGuildRejectAlliance(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildRejectPeace: // RECPPEAT
                {
                    HandleGuildRejectPeace(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildAcceptAlliance: // ACEPALIA
                {
                    HandleGuildAcceptAlliance(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildOfferPeace: // PEACEOFF
                {
                    HandleGuildOfferPeace(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildOfferAlliance: // ALLIEOFF
                {
                    HandleGuildOfferAlliance(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildAllianceDetails: // ALLIEDET
                {
                    HandleGuildAllianceDetails(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildPeaceDetails: // PEACEDET
                {
                    HandleGuildPeaceDetails(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildRequestJoinerInfo: // ENVCOMEN
                {
                    HandleGuildRequestJoinerInfo(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildAlliancePropList: // ENVALPRO
                {
                    HandleGuildAlliancePropList(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildPeacePropList: // ENVPROPP
                {
                    HandleGuildPeacePropList(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildDeclareWar: // DECGUERR
                {
                    HandleGuildDeclareWar(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildNewWebsite: // NEWWEBSI
                {
                    HandleGuildNewWebsite(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildAcceptNewMember: // ACEPTARI
                {
                    HandleGuildAcceptNewMember(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildRejectNewMember: // RECHAZAR
                {
                    HandleGuildRejectNewMember(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildKickMember: // ECHARCLA
                {
                    HandleGuildKickMember(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildUpdateNews: // ACTGNEWS
                {
                    HandleGuildUpdateNews(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildMemberInfo: // 1HRINFO<
                {
                    HandleGuildMemberInfo(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildOpenElections: // ABREELEC
                {
                    HandleGuildOpenElections(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildRequestMembership: // SOLICITUD
                {
                    HandleGuildRequestMembership(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildRequestDetails: // CLANDETAILS
                {
                    HandleGuildRequestDetails(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Online: // /ONLINE
                {
                    HandleOnline(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Quit: // /SALIR
                {
                    HandleQuit(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildLeave: // /SALIRCLAN
                {
                    HandleGuildLeave(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.RequestAccountState: // /BALANCE
                {
                    HandleRequestAccountState(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.PetStand: // /QUIETO
                {
                    HandlePetStand(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.PetFollow: // /ACOMPAÑAR
                {
                    HandlePetFollow(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.ReleasePet: // /LIBERAR
                {
                    HandleReleasePet(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.TrainList: // /ENTRENAR
                {
                    HandleTrainList(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Rest: // /DESCANSAR
                {
                    HandleRest(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Meditate: // /MEDITAR
                {
                    HandleMeditate(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Resucitate: // /RESUCITAR
                {
                    HandleResucitate(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Heal: // /CURAR
                {
                    HandleHeal(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Help: // /AYUDA
                {
                    HandleHelp(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.RequestStats: // /EST
                {
                    HandleRequestStats(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.CommerceStart: // /COMERCIAR
                {
                    HandleCommerceStart(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.BankStart: // /BOVEDA
                {
                    HandleBankStart(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Enlist: // /ENLISTAR
                {
                    HandleEnlist(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Information: // /INFORMACION
                {
                    HandleInformation(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Reward: // /RECOMPENSA
                {
                    HandleReward(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.RequestMOTD: // /MOTD
                {
                    HandleRequestMOTD(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.UpTime: // /UPTIME
                {
                    HandleUpTime(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.PartyLeave: // /SALIRPARTY
                {
                    HandlePartyLeave(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.PartyCreate: // /CREARPARTY
                {
                    HandlePartyCreate(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.PartyJoin: // /PARTY
                {
                    HandlePartyJoin(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Inquiry: // /ENCUESTA ( with no params )
                {
                    HandleInquiry(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildMessage: // /CMSG
                {
                    HandleGuildMessage(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.PartyMessage: // /PMSG
                {
                    HandlePartyMessage(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.CentinelReport: // /CENTINELA
                {
                    HandleCentinelReport(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildOnline: // /ONLINECLAN
                {
                    HandleGuildOnline(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.PartyOnline: // /ONLINEPARTY
                {
                    HandlePartyOnline(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.CouncilMessage: // /BMSG
                {
                    HandleCouncilMessage(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.RoleMasterRequest: // /ROL
                {
                    HandleRoleMasterRequest(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GMRequest: // /GM
                {
                    HandleGMRequest(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.bugReport: // /_BUG
                {
                    HandleBugReport(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.ChangeDescription: // /DESC
                {
                    HandleChangeDescription(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildVote: // /VOTO
                {
                    HandleGuildVote(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Punishments: // /PENAS
                {
                    HandlePunishments(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.ChangePassword: // /CONTRASEÑA
                {
                    HandleChangePassword(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Gamble: // /APOSTAR
                {
                    HandleGamble(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.InquiryVote: // /ENCUESTA ( with parameters )
                {
                    HandleInquiryVote(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.LeaveFaction: // /RETIRAR ( with no arguments )
                {
                    HandleLeaveFaction(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.BankExtractGold: // /RETIRAR ( with arguments )
                {
                    HandleBankExtractGold(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.BankDepositGold: // /DEPOSITAR
                {
                    HandleBankDepositGold(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Denounce: // /DENUNCIAR
                {
                    HandleDenounce(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildFundate: // /FUNDARCLAN
                {
                    HandleGuildFundate(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GuildFundation:
                {
                    HandleGuildFundation(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.PartyKick: // /ECHARPARTY
                {
                    HandlePartyKick(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.PartySetLeader: // /PARTYLIDER
                {
                    HandlePartySetLeader(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.PartyAcceptMember: // /ACCEPTPARTY
                {
                    HandlePartyAcceptMember(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Ping: // /PING
                {
                    HandlePing(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.RequestPartyForm:
                {
                    HandlePartyForm(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.ItemUpgrade:
                {
                    HandleItemUpgrade(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.GMCommands: // GM Messages
                {
                    HandleGMCommands(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.InitCrafting:
                {
                    HandleInitCrafting(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Home:
                {
                    HandleHome(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.ShowGuildNews:
                {
                    HandleShowGuildNews(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.ShareNpc:
                {
                    HandleShareNpc(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.StopSharingNpc:
                {
                    HandleStopSharingNpc(UserIndex);
                    break;
                }

                case (byte)ClientPacketID.Consultation:
                {
                    HandleConsultation(UserIndex.ToString());
                    break;
                }

                default:
                {
                    // ERROR : Abort!
                    TCP.CloseSocket(UserIndex);
                    break;
                }
            }

            // Done with this packet, move on to next one or send everything if no more packets found
            if ((Declaraciones.UserList[UserIndex].incomingData.length > 0) & (Information.Err().Number == 0))
            {
                Information.Err().Clear();
                HandleIncomingData(UserIndex);
            }

            else if ((Information.Err().Number != 0) & !(Information.Err().Number ==
                                                         Declaraciones.UserList[UserIndex].incomingData
                                                             .NotEnoughDataErrCode))
            {
                // An error ocurred, log it and kick player.
                var argdesc = "Error: " + Information.Err().Number + " [" + Information.Err().Description + "] " +
                              " Source: " + Information.Err().Source + Constants.vbTab + " HelpFile: " +
                              Information.Err().HelpFile + Constants.vbTab + " HelpContext: " +
                              Information.Err().HelpContext + Constants.vbTab + " LastDllError: " +
                              Information.Err().LastDllError + Constants.vbTab + " - UserIndex: " + UserIndex +
                              " - producido al manejar el paquete: " + packetID;
                General.LogError(ref argdesc);
                TCP.CloseSocket(UserIndex);
            }

            else
            {
                // Flush buffer - send everything that has been written
                FlushBuffer(UserIndex);
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleIncomingData: " + ex.Message);
        }
    }


    public static void WriteMultiMessage(short UserIndex, short MessageIndex, int Arg1 = 0, int Arg2 = 0, int Arg3 = 0,
        string StringArg1 = "")
    {
        // "Has matado a " & UserList(VictimIndex).name & "!" "Has ganado " & DaExp & " puntos de experiencia."


        // El cliente no conoce nada sobre nombre de mapas y hogares, por lo tanto _
        // hasta que no se pasen los dats e .INFs al cliente, esto queda así.

        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.MultiMessage);
                withBlock.WriteByte(Convert.ToByte(MessageIndex));
                switch (MessageIndex)
                {
                    case (short)Declaraciones.eMessages.DontSeeAnything:
                    case (short)Declaraciones.eMessages.NPCSwing:
                    case (short)Declaraciones.eMessages.NPCKillUser:
                    case (short)Declaraciones.eMessages.BlockedWithShieldUser:
                    case (short)Declaraciones.eMessages.BlockedWithShieldother:
                    case (short)Declaraciones.eMessages.UserSwing:
                    case (short)Declaraciones.eMessages.SafeModeOn:
                    case (short)Declaraciones.eMessages.SafeModeOff:
                    case (short)Declaraciones.eMessages.ResuscitationSafeOff:
                    case (short)Declaraciones.eMessages.ResuscitationSafeOn:
                    case (short)Declaraciones.eMessages.NobilityLost:
                    case (short)Declaraciones.eMessages.CantUseWhileMeditating:
                    case (short)Declaraciones.eMessages.CancelHome:
                    case (short)Declaraciones.eMessages.FinishHome:
                    {
                        break;
                    }
                    case (short)Declaraciones.eMessages.NPCHitUser:
                    {
                        withBlock.WriteByte(Convert.ToByte(Arg1));
                        withBlock.WriteInteger(Convert.ToInt16(Arg2));
                        break;
                    }
                    case (short)Declaraciones.eMessages.UserHitNPC:
                    {
                        withBlock.WriteLong(Arg1);
                        break;
                    }
                    case (short)Declaraciones.eMessages.UserAttackedSwing:
                    {
                        withBlock.WriteInteger(Declaraciones.UserList[Arg1].character.CharIndex);
                        break;
                    }
                    case (short)Declaraciones.eMessages.UserHittedByUser:
                    {
                        withBlock.WriteInteger(Convert.ToInt16(Arg1));
                        withBlock.WriteByte(Convert.ToByte(Arg2));
                        withBlock.WriteInteger(Convert.ToInt16(Arg3));
                        break;
                    }
                    case (short)Declaraciones.eMessages.UserHittedUser:
                    {
                        withBlock.WriteInteger(Convert.ToInt16(Arg1));
                        withBlock.WriteByte(Convert.ToByte(Arg2));
                        withBlock.WriteInteger(Convert.ToInt16(Arg3));
                        break;
                    }
                    case (short)Declaraciones.eMessages.WorkRequestTarget:
                    {
                        withBlock.WriteByte(Convert.ToByte(Arg1));
                        break;
                    }
                    case (short)Declaraciones.eMessages.HaveKilledUser:
                    {
                        withBlock.WriteInteger(Declaraciones.UserList[Arg1].character.CharIndex);
                        withBlock.WriteLong(Arg2);
                        break;
                    }
                    case (short)Declaraciones.eMessages.UserKill:
                    {
                        withBlock.WriteInteger(Declaraciones.UserList[Arg1].character.CharIndex);
                        break;
                    }
                    case (short)Declaraciones.eMessages.EarnExp:
                    {
                        break;
                    }
                    case (short)Declaraciones.eMessages.Home:
                    {
                        withBlock.WriteByte(Convert.ToByte(Arg1));
                        withBlock.WriteInteger(Convert.ToInt16(Arg2));
                        withBlock.WriteASCIIString(StringArg1);
                        break;
                    }
                }
            }
        }, () => FlushBuffer(UserIndex)); // Target
        // damage
        // damage
        // AttackerIndex
        // Target
        // damage
        // AttackerIndex
        // Target
        // damage
        // skill

        // VictimIndex
        // Expe
        // "¡" & .name & " te ha matado!"
        // AttackerIndex
        // Call .WriteByte(CByte(Arg2))
    }


    private static void HandleGMCommands(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************
 
        var command = default(byte);

        try
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                withBlock.incomingData.ReadByte();

                command = withBlock.incomingData.PeekByte();

                switch (command)
                {
                    case (byte)Declaraciones.eGMCommands.GMMessage: // /GMSG
                    {
                        HandleGMMessage(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.showName: // /SHOWNAME
                    {
                        HandleShowName(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.OnlineRoyalArmy:
                    {
                        HandleOnlineRoyalArmy(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.OnlineChaosLegion: // /ONLINECAOS
                    {
                        HandleOnlineChaosLegion(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.GoNearby: // /IRCERCA
                    {
                        HandleGoNearby(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.comment: // /REM
                    {
                        HandleComment(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.serverTime: // /HORA
                    {
                        HandleServerTime(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.Where: // /DONDE
                    {
                        HandleWhere(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.CreaturesInMap: // /NENE
                    {
                        HandleCreaturesInMap(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.WarpMeToTarget: // /TELEPLOC
                    {
                        HandleWarpMeToTarget(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.WarpChar: // /TELEP
                    {
                        HandleWarpChar(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.Silence: // /SILENCIAR
                    {
                        HandleSilence(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.SOSShowList: // /SHOW SOS
                    {
                        HandleSOSShowList(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.SOSRemove: // SOSDONE
                    {
                        HandleSOSRemove(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.GoToChar: // /IRA
                    {
                        HandleGoToChar(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.invisible: // /INVISIBLE
                    {
                        HandleInvisible(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.GMPanel: // /PANELGM
                    {
                        HandleGMPanel(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.RequestUserList: // LISTUSU
                    {
                        HandleRequestUserList(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.Working: // /TRABAJANDO
                    {
                        HandleWorking(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.Hiding: // /OCULTANDO
                    {
                        HandleHiding(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.Jail: // /CARCEL
                    {
                        HandleJail(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.KillNPC: // /RMATA
                    {
                        HandleKillNPC(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.WarnUser: // /ADVERTENCIA
                    {
                        HandleWarnUser(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.EditChar: // /MOD
                    {
                        HandleEditChar(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.RequestCharInfo: // /INFO
                    {
                        HandleRequestCharInfo(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.RequestCharStats: // /STAT
                    {
                        HandleRequestCharStats(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.RequestCharGold: // /BAL
                    {
                        HandleRequestCharGold(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.RequestCharInventory: // /INV
                    {
                        HandleRequestCharInventory(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.RequestCharBank: // /BOV
                    {
                        HandleRequestCharBank(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.RequestCharSkills: // /SKILLS
                    {
                        HandleRequestCharSkills(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ReviveChar: // /REVIVIR
                    {
                        HandleReviveChar(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.OnlineGM: // /ONLINEGM
                    {
                        HandleOnlineGM(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.OnlineMap: // /ONLINEMAP
                    {
                        HandleOnlineMap(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.Forgive: // /PERDON
                    {
                        HandleForgive(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.Kick: // /ECHAR
                    {
                        HandleKick(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.Execute: // /EJECUTAR
                    {
                        HandleExecute(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.BanChar: // /BAN
                    {
                        HandleBanChar(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.UnbanChar: // /UNBAN
                    {
                        HandleUnbanChar(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.NPCFollow: // /SEGUIR
                    {
                        HandleNPCFollow(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.SummonChar: // /SUM
                    {
                        HandleSummonChar(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.SpawnListRequest: // /CC
                    {
                        HandleSpawnListRequest(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.SpawnCreature: // SPA
                    {
                        HandleSpawnCreature(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ResetNPCInventory: // /RESETINV
                    {
                        HandleResetNPCInventory(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.CleanWorld: // /LIMPIAR
                    {
                        HandleCleanWorld(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ServerMessage: // /RMSG
                    {
                        HandleServerMessage(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.NickToIP: // /NICK2IP
                    {
                        HandleNickToIP(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.IPToNick: // /IP2NICK
                    {
                        HandleIPToNick(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.GuildOnlineMembers: // /ONCLAN
                    {
                        HandleGuildOnlineMembers(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.TeleportCreate: // /CT
                    {
                        HandleTeleportCreate(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.TeleportDestroy: // /DT
                    {
                        HandleTeleportDestroy(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.RainToggle: // /LLUVIA
                    {
                        HandleRainToggle(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.SetCharDescription: // /SETDESC
                    {
                        HandleSetCharDescription(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ForceMIDIToMap: // /FORCEMIDIMAP
                    {
                        HanldeForceMIDIToMap(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ForceWAVEToMap: // /FORCEWAVMAP
                    {
                        HandleForceWAVEToMap(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.RoyalArmyMessage: // /REALMSG
                    {
                        HandleRoyalArmyMessage(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ChaosLegionMessage: // /CAOSMSG
                    {
                        HandleChaosLegionMessage(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.CitizenMessage: // /CIUMSG
                    {
                        HandleCitizenMessage(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.CriminalMessage: // /CRIMSG
                    {
                        HandleCriminalMessage(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.TalkAsNPC: // /TALKAS
                    {
                        HandleTalkAsNPC(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.DestroyAllItemsInArea: // /MASSDEST
                    {
                        HandleDestroyAllItemsInArea(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.AcceptRoyalCouncilMember: // /ACEPTCONSE
                    {
                        HandleAcceptRoyalCouncilMember(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.AcceptChaosCouncilMember: // /ACEPTCONSECAOS
                    {
                        HandleAcceptChaosCouncilMember(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ItemsInTheFloor: // /PISO
                    {
                        HandleItemsInTheFloor(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.MakeDumb: // /ESTUPIDO
                    {
                        HandleMakeDumb(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.MakeDumbNoMore: // /NOESTUPIDO
                    {
                        HandleMakeDumbNoMore(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.DumpIPTables: // /DUMPSECURITY
                    {
                        HandleDumpIPTables(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.CouncilKick: // /KICKCONSE
                    {
                        HandleCouncilKick(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.SetTrigger: // /TRIGGER
                    {
                        HandleSetTrigger(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.AskTrigger: // /TRIGGER with no args
                    {
                        HandleAskTrigger(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.BannedIPList: // /BANIPLIST
                    {
                        HandleBannedIPList(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.BannedIPReload: // /BANIPRELOAD
                    {
                        HandleBannedIPReload(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.GuildMemberList: // /MIEMBROSCLAN
                    {
                        HandleGuildMemberList(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.GuildBan: // /BANCLAN
                    {
                        HandleGuildBan(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.BanIP: // /BANIP
                    {
                        HandleBanIP(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.UnbanIP: // /UNBANIP
                    {
                        HandleUnbanIP(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.CreateItem: // /CI
                    {
                        HandleCreateItem(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.DestroyItems: // /DEST
                    {
                        HandleDestroyItems(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ChaosLegionKick: // /NOCAOS
                    {
                        HandleChaosLegionKick(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.RoyalArmyKick: // /NOREAL
                    {
                        HandleRoyalArmyKick(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ForceMIDIAll: // /FORCEMIDI
                    {
                        HandleForceMIDIAll(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ForceWAVEAll: // /FORCEWAV
                    {
                        HandleForceWAVEAll(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.RemovePunishment: // /BORRARPENA
                    {
                        HandleRemovePunishment(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.TileBlockedToggle: // /BLOQ
                    {
                        HandleTileBlockedToggle(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.KillNPCNoRespawn: // /MATA
                    {
                        HandleKillNPCNoRespawn(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.KillAllNearbyNPCs: // /MASSKILL
                    {
                        HandleKillAllNearbyNPCs(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.LastIP: // /LASTIP
                    {
                        HandleLastIP(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ChangeMOTD: // /MOTDCAMBIA
                    {
                        HandleChangeMOTD(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.SetMOTD: // ZMOTD
                    {
                        HandleSetMOTD(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.SystemMessage: // /SMSG
                    {
                        HandleSystemMessage(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.CreateNPC: // /ACC
                    {
                        HandleCreateNPC(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.CreateNPCWithRespawn: // /RACC
                    {
                        HandleCreateNPCWithRespawn(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ImperialArmour: // /AI1 - 4
                    {
                        HandleImperialArmour(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ChaosArmour: // /AC1 - 4
                    {
                        HandleChaosArmour(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.NavigateToggle: // /NAVE
                    {
                        HandleNavigateToggle(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ServerOpenToUsersToggle: // /HABILITAR
                    {
                        HandleServerOpenToUsersToggle(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.TurnOffServer: // /APAGAR
                    {
                        HandleTurnOffServer(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.TurnCriminal: // /CONDEN
                    {
                        HandleTurnCriminal(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ResetFactions: // /RAJAR
                    {
                        HandleResetFactions(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.RemoveCharFromGuild: // /RAJARCLAN
                    {
                        HandleRemoveCharFromGuild(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.RequestCharMail: // /LASTEMAIL
                    {
                        HandleRequestCharMail(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.AlterPassword: // /APASS
                    {
                        HandleAlterPassword(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.AlterMail: // /AEMAIL
                    {
                        HandleAlterMail(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.AlterName: // /ANAME
                    {
                        HandleAlterName(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ToggleCentinelActivated: // /CENTINELAACTIVADO
                    {
                        HandleToggleCentinelActivated(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.DoBackUp: // /DOBACKUP
                    {
                        HandleDoBackUp(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ShowGuildMessages: // /SHOWCMSG
                    {
                        HandleShowGuildMessages(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.SaveMap: // /GUARDAMAPA
                    {
                        HandleSaveMap(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ChangeMapInfoPK: // /MODMAPINFO PK
                    {
                        HandleChangeMapInfoPK(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ChangeMapInfoBackup: // /MODMAPINFO BACKUP
                    {
                        HandleChangeMapInfoBackup(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ChangeMapInfoRestricted: // /MODMAPINFO RESTRINGIR
                    {
                        HandleChangeMapInfoRestricted(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ChangeMapInfoNoMagic: // /MODMAPINFO MAGIASINEFECTO
                    {
                        HandleChangeMapInfoNoMagic(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ChangeMapInfoNoInvi: // /MODMAPINFO INVISINEFECTO
                    {
                        HandleChangeMapInfoNoInvi(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ChangeMapInfoNoResu: // /MODMAPINFO RESUSINEFECTO
                    {
                        HandleChangeMapInfoNoResu(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ChangeMapInfoLand: // /MODMAPINFO TERRENO
                    {
                        HandleChangeMapInfoLand(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ChangeMapInfoZone: // /MODMAPINFO ZONA
                    {
                        HandleChangeMapInfoZone(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.SaveChars: // /GRABAR
                    {
                        HandleSaveChars(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.CleanSOS: // /BORRAR SOS
                    {
                        HandleCleanSOS(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ShowServerForm: // /SHOW INT
                    {
                        HandleShowServerForm(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.night: // /NOCHE
                    {
                        HandleNight(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.KickAllChars: // /ECHARTODOSPJS
                    {
                        HandleKickAllChars(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ReloadNPCs: // /RELOADNPCS
                    {
                        HandleReloadNPCs(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ReloadServerIni: // /RELOADSINI
                    {
                        HandleReloadServerIni(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ReloadSpells: // /RELOADHECHIZOS
                    {
                        HandleReloadSpells(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ReloadObjects: // /RELOADOBJ
                    {
                        HandleReloadObjects(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.Restart: // /REINICIAR
                    {
                        HandleRestart(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ResetAutoUpdate: // /AUTOUPDATE
                    {
                        HandleResetAutoUpdate(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.ChatColor: // /CHATCOLOR
                    {
                        HandleChatColor(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.Ignored: // /IGNORADO
                    {
                        HandleIgnored(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.CheckSlot: // /SLOT
                    {
                        HandleCheckSlot(UserIndex);
                        break;
                    }

                    case (byte)Declaraciones.eGMCommands.SetIniVar: // /SETINIVAR LLAVE CLAVE VALOR
                    {
                        HandleSetIniVar(UserIndex);
                        break;
                    }
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGMCommands: " + ex.Message);
            var argdesc = "Error en GmCommands. Error: " + ex.GetType().Name + " - " + ex.Message + ". Paquete: " +
                          command;
            General.LogError(ref argdesc);
        }
    }

    // '
    // Handles the "Home" message.
    // 
    // @param    userIndex The index of the user sending the message.
    private static void HandleHome(short UserIndex)
    {
        // ***************************************************
        // Author: Budi
        // Creation Date: 06/01/2010
        // Last Modification: 05/06/10
        // Pato - 05/06/10: Add the Ucase$ to prevent problems.
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            withBlock.incomingData.ReadByte();
            if (withBlock.flags.TargetNpcTipo == Declaraciones.eNPCType.Gobernador)
            {
                UsUaRiOs.setHome(UserIndex,
                    (Declaraciones.eCiudad)Declaraciones.Npclist[withBlock.flags.TargetNPC].Ciudad,
                    withBlock.flags.TargetNPC);
            }
            else if (withBlock.flags.Muerto == 1)
            {
                // Si es un mapa común y no está en cana
                if ((Declaraciones.mapInfo[withBlock.Pos.Map].Restringir.ToUpper() == "NO") &
                    (withBlock.Counters.Pena == 0))
                {
                    if (withBlock.flags.Traveling == 0)
                    {
                        if (Declaraciones.Ciudades[(int)withBlock.Hogar].Map != withBlock.Pos.Map)
                            UsUaRiOs.goHome(UserIndex);
                        else
                            WriteConsoleMsg(UserIndex, "Ya te encuentras en tu hogar.", FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.CancelHome);
                        withBlock.flags.Traveling = 0;
                        withBlock.Counters.goHome = 0;
                    }
                }
                else
                {
                    WriteConsoleMsg(UserIndex, "No puedes usar este comando aquí.", FontTypeNames.FONTTYPE_FIGHT);
                }
            }
            else
            {
                WriteConsoleMsg(UserIndex, "Debes estar muerto para utilizar este comando.",
                    FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Handles the "LoginExistingChar" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleLoginExistingChar(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 6)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
            var buffer = new clsByteQueue();
            buffer.CopyBuffer(ref Declaraciones.UserList[UserIndex].incomingData);

            // Remove packet ID
            buffer.ReadByte();

            string UserName;
            string Password;
            string version;

            UserName = buffer.ReadASCIIString();
            Password = buffer.ReadASCIIString();

            // Convert version number to string
            version = buffer.ReadByte() + "." + buffer.ReadByte() + "." + buffer.ReadByte();

            if (!TCP.AsciiValidos(UserName))
            {
                WriteErrorMsg(UserIndex, "Nombre inválido.");
                FlushBuffer(UserIndex);
                TCP.CloseSocket(UserIndex);

                return;
            }

            if (!Admin.PersonajeExiste(UserName))
            {
                WriteErrorMsg(UserIndex, "El personaje no existe.");
                FlushBuffer(UserIndex);
                TCP.CloseSocket(UserIndex);

                return;
            }

            if (Admin.BANCheck(UserName))
                WriteErrorMsg(UserIndex,
                    "Se te ha prohibido la entrada a Argentum Online debido a tu mal comportamiento. Puedes consultar el reglamento y el sistema de soporte desde www.argentumonline.com.ar");
            else if (!Admin.VersionOK(version))
                WriteErrorMsg(UserIndex,
                    "Esta versión del juego es obsoleta, la versión correcta es la " + Declaraciones.ULTIMAVERSION +
                    ". La misma se encuentra disponible en www.argentumonline.com.ar");
            else
                TCP.ConnectUser(UserIndex, ref UserName, ref Password);

            // If we got here then packet is complete, copy data back to original queue
            Declaraciones.UserList[UserIndex].incomingData.CopyBuffer(ref buffer);
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleLoginExistingChar: " + ex.Message);
        }
    }

    // '
    // Handles the "ThrowDices" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleThrowDices(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].Stats;
            withBlock.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] = Convert.ToByte(
                SistemaCombate.MaximoInt(15, 13 + Matematicas.RandomNumber(0, 3) + Matematicas.RandomNumber(0, 2)));
            withBlock.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] = Convert.ToByte(
                SistemaCombate.MaximoInt(15, 12 + Matematicas.RandomNumber(0, 3) + Matematicas.RandomNumber(0, 3)));
            withBlock.UserAtributos[(int)Declaraciones.eAtributos.Inteligencia] = Convert.ToByte(
                SistemaCombate.MaximoInt(16, 13 + Matematicas.RandomNumber(0, 3) + Matematicas.RandomNumber(0, 2)));
            withBlock.UserAtributos[(int)Declaraciones.eAtributos.Carisma] = Convert.ToByte(
                SistemaCombate.MaximoInt(15, 12 + Matematicas.RandomNumber(0, 3) + Matematicas.RandomNumber(0, 3)));
            withBlock.UserAtributos[(int)Declaraciones.eAtributos.Constitucion] =
                Convert.ToByte(16 + Matematicas.RandomNumber(0, 1) + Matematicas.RandomNumber(0, 1));
        }

        WriteDiceRoll(UserIndex);
    }

    // '
    // Handles the "LoginNewChar" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleLoginNewChar(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 15)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
            var buffer = new clsByteQueue();
            buffer.CopyBuffer(ref Declaraciones.UserList[UserIndex].incomingData);

            // Remove packet ID
            buffer.ReadByte();

            string UserName;
            string Password;
            string version;
            Declaraciones.eRaza race;
            Declaraciones.eGenero gender;
            Declaraciones.eCiudad homeland;
            Declaraciones.eClass characterClass;
            short Head;
            string mail;

            if (Declaraciones.PuedeCrearPersonajes == 0)
            {
                WriteErrorMsg(UserIndex, "La creación de personajes en este servidor se ha deshabilitado.");
                FlushBuffer(UserIndex);
                TCP.CloseSocket(UserIndex);

                return;
            }

            if (Declaraciones.ServerSoloGMs != 0)
            {
                WriteErrorMsg(UserIndex,
                    "Servidor restringido a administradores. Consulte la página oficial o el foro oficial para más información.");
                FlushBuffer(UserIndex);
                TCP.CloseSocket(UserIndex);

                return;
            }

            UserName = buffer.ReadASCIIString();
            Password = buffer.ReadASCIIString();

            // Convert version number to string
            version = buffer.ReadByte() + "." + buffer.ReadByte() + "." + buffer.ReadByte();
            race = (Declaraciones.eRaza)buffer.ReadByte();
            gender = (Declaraciones.eGenero)buffer.ReadByte();
            characterClass = (Declaraciones.eClass)buffer.ReadByte();
            Head = buffer.ReadInteger();
            mail = buffer.ReadASCIIString();
            homeland = (Declaraciones.eCiudad)buffer.ReadByte();

            if (!Admin.VersionOK(version))
                WriteErrorMsg(UserIndex,
                    "Esta versión del juego es obsoleta, la versión correcta es la " + Declaraciones.ULTIMAVERSION +
                    ". La misma se encuentra disponible en www.argentumonline.com.ar");
            else
                TCP.ConnectNewUser(UserIndex, ref UserName, ref Password, race, gender, characterClass, ref mail,
                    homeland, Head);

            // If we got here then packet is complete, copy data back to original queue
            Declaraciones.UserList[UserIndex].incomingData.CopyBuffer(ref buffer);
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleLoginNewChar: " + ex.Message);
        }
    }

    // '
    // Handles the "Talk" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleTalk(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 13/01/2010
        // 15/07/2009: ZaMa - Now invisible admins talk by console.
        // 23/09/2009: ZaMa - Now invisible admins can't send empty chat.
        // 13/01/2010: ZaMa - Now hidden on boat pirats recover the proper boat body.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string Chat;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];

                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                Chat = buffer.ReadASCIIString();

                // [Consejeros & GMs]
                if ((withBlock.flags.Privilegios &
                     (Declaraciones.PlayerType.Consejero | Declaraciones.PlayerType.SemiDios)) != 0)
                {
                    var argtexto = "Dijo: " + Chat;
                    General.LogGM(ref withBlock.name, ref argtexto);
                }

                // I see you....
                if (withBlock.flags.Oculto > 0)
                {
                    withBlock.flags.Oculto = 0;
                    withBlock.Counters.TiempoOculto = 0;

                    if (withBlock.flags.Navegando == 1)
                    {
                        if (withBlock.clase == Declaraciones.eClass.Pirat)
                        {
                            // Pierde la apariencia de fragata fantasmal
                            UsUaRiOs.ToogleBoatBody(UserIndex);
                            WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!",
                                FontTypeNames.FONTTYPE_INFO);
                            UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body, withBlock.character.Head,
                                (byte)withBlock.character.heading, Declaraciones.NingunArma,
                                Declaraciones.NingunEscudo, Declaraciones.NingunCasco);
                        }
                    }
                    else if (withBlock.flags.invisible == 0)
                    {
                        UsUaRiOs.SetInvisible(UserIndex, Declaraciones.UserList[UserIndex].character.CharIndex,
                            false);
                        WriteConsoleMsg(UserIndex, "¡Has vuelto a ser visible!", FontTypeNames.FONTTYPE_INFO);
                    }
                }

                if (Migration.migr_LenB(Chat) != 0)
                {
                    // Analize chat...
                    Statistics.ParseChat(ref Chat);

                    if (!(withBlock.flags.AdminInvisible == 1))
                    {
                        if (withBlock.flags.Muerto == 1)
                            modSendData.SendData(modSendData.SendTarget.ToDeadArea, UserIndex,
                                PrepareMessageChatOverHead(Chat, withBlock.character.CharIndex,
                                    Declaraciones.CHAT_COLOR_DEAD_CHAR));
                        else
                            modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                PrepareMessageChatOverHead(Chat, withBlock.character.CharIndex,
                                    withBlock.flags.ChatColor));
                    }
                    else if (!string.IsNullOrEmpty(Chat.TrimEnd()))
                    {
                        modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                            PrepareMessageConsoleMsg("Gm> " + Chat, FontTypeNames.FONTTYPE_GM));
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleTalk: " + ex.Message);
        }
    }

    // '
    // Handles the "Yell" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleYell(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 13/01/2010 (ZaMa)
        // 15/07/2009: ZaMa - Now invisible admins yell by console.
        // 13/01/2010: ZaMa - Now hidden on boat pirats recover the proper boat body.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string Chat;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];

                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                Chat = buffer.ReadASCIIString();


                // [Consejeros & GMs]
                if ((withBlock.flags.Privilegios &
                     (Declaraciones.PlayerType.Consejero | Declaraciones.PlayerType.SemiDios)) != 0)
                {
                    var argtexto = "Grito: " + Chat;
                    General.LogGM(ref withBlock.name, ref argtexto);
                }

                // I see you....
                if (withBlock.flags.Oculto > 0)
                {
                    withBlock.flags.Oculto = 0;
                    withBlock.Counters.TiempoOculto = 0;

                    if (withBlock.flags.Navegando == 1)
                    {
                        if (withBlock.clase == Declaraciones.eClass.Pirat)
                        {
                            // Pierde la apariencia de fragata fantasmal
                            UsUaRiOs.ToogleBoatBody(UserIndex);
                            WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!",
                                FontTypeNames.FONTTYPE_INFO);
                            UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body, withBlock.character.Head,
                                (byte)withBlock.character.heading, Declaraciones.NingunArma,
                                Declaraciones.NingunEscudo, Declaraciones.NingunCasco);
                        }
                    }
                    else if (withBlock.flags.invisible == 0)
                    {
                        UsUaRiOs.SetInvisible(UserIndex, withBlock.character.CharIndex, false);
                        WriteConsoleMsg(UserIndex, "¡Has vuelto a ser visible!", FontTypeNames.FONTTYPE_INFO);
                    }
                }

                if (Migration.migr_LenB(Chat) != 0)
                {
                    // Analize chat...
                    Statistics.ParseChat(ref Chat);

                    if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                    {
                        if (Declaraciones.UserList[UserIndex].flags.Muerto == 1)
                            modSendData.SendData(modSendData.SendTarget.ToDeadArea, UserIndex,
                                PrepareMessageChatOverHead(Chat, withBlock.character.CharIndex,
                                    Declaraciones.CHAT_COLOR_DEAD_CHAR));
                        else
                            modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                PrepareMessageChatOverHead(Chat, withBlock.character.CharIndex,
                                    ColorTranslator.ToOle(Color.Red)));
                    }
                    else if (!(withBlock.flags.AdminInvisible == 1))
                    {
                        modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                            PrepareMessageChatOverHead(Chat, withBlock.character.CharIndex,
                                Declaraciones.CHAT_COLOR_GM_YELL));
                    }
                    else
                    {
                        modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                            PrepareMessageConsoleMsg("Gm> " + Chat, FontTypeNames.FONTTYPE_GM));
                    }
                }


                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleYell: " + ex.Message);
        }
    }

    // '
    // Handles the "Whisper" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleWhisper(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 15/07/2009
        // 28/05/2009: ZaMa - Now it doesn't appear any message when private talking to an invisible admin
        // 15/07/2009: ZaMa - Now invisible admins wisper by console.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string Chat;
            short targetCharIndex;
            short TargetUserIndex;
            Declaraciones.PlayerType targetPriv;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                targetCharIndex = buffer.ReadInteger();
                Chat = buffer.ReadASCIIString();

                TargetUserIndex = Characters.CharIndexToUserIndex(targetCharIndex);

                if (withBlock.flags.Muerto != 0)
                {
                    WriteConsoleMsg(UserIndex,
                        "¡¡Estás muerto!! Los muertos no pueden comunicarse con el mundo de los vivos. ",
                        FontTypeNames.FONTTYPE_INFO);
                }
                else if (TargetUserIndex == Characters.INVALID_INDEX)
                {
                    WriteConsoleMsg(UserIndex, "Usuario inexistente.", FontTypeNames.FONTTYPE_INFO);
                }
                else
                {
                    targetPriv = Declaraciones.UserList[TargetUserIndex].flags.Privilegios;
                    // A los dioses y admins no vale susurrarles si no sos uno vos mismo (así no pueden ver si están conectados o no)
                    if (((targetPriv & (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin)) != 0) &
                        ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User |
                                                         Declaraciones.PlayerType.Consejero |
                                                         Declaraciones.PlayerType.SemiDios)) != 0))
                    {
                        // Controlamos que no este invisible
                        if (Declaraciones.UserList[TargetUserIndex].flags.AdminInvisible != 1)
                            WriteConsoleMsg(UserIndex, "No puedes susurrarle a los Dioses y Admins.",
                                FontTypeNames.FONTTYPE_INFO);
                        // A los Consejeros y SemiDioses no vale susurrarles si sos un PJ común.
                    }
                    else if (((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0) &
                             ((int)(~targetPriv & Declaraciones.PlayerType.User) != 0))
                    {
                        // Controlamos que no este invisible
                        if (Declaraciones.UserList[TargetUserIndex].flags.AdminInvisible != 1)
                            WriteConsoleMsg(UserIndex, "No puedes susurrarle a los GMs.", FontTypeNames.FONTTYPE_INFO);
                    }
                    else if (!TCP.EstaPCarea(ref UserIndex, ref TargetUserIndex))
                    {
                        WriteConsoleMsg(UserIndex, "Estás muy lejos del usuario.", FontTypeNames.FONTTYPE_INFO);
                    }

                    else
                    {
                        // [Consejeros & GMs]
                        if ((withBlock.flags.Privilegios &
                             (Declaraciones.PlayerType.Consejero | Declaraciones.PlayerType.SemiDios)) != 0)
                        {
                            var argtexto = "Le dijo a '" + Declaraciones.UserList[TargetUserIndex].name + "' " + Chat;
                            General.LogGM(ref withBlock.name, ref argtexto);
                        }

                        if (Migration.migr_LenB(Chat) != 0)
                        {
                            // Analize chat...
                            Statistics.ParseChat(ref Chat);

                            if (!(withBlock.flags.AdminInvisible == 1))
                            {
                                WriteChatOverHead(UserIndex, Chat, withBlock.character.CharIndex,
                                    ColorTranslator.ToOle(Color.Blue));
                                WriteChatOverHead(TargetUserIndex, Chat, withBlock.character.CharIndex,
                                    ColorTranslator.ToOle(Color.Blue));
                                FlushBuffer(TargetUserIndex);

                                // [CDT 17-02-2004]
                                if ((withBlock.flags.Privilegios &
                                     (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero)) != 0)
                                    modSendData.SendData(modSendData.SendTarget.ToAdminsAreaButConsejeros, UserIndex,
                                        PrepareMessageChatOverHead(
                                            "A " + Declaraciones.UserList[TargetUserIndex].name + "> " + Chat,
                                            withBlock.character.CharIndex, ColorTranslator.ToOle(Color.Yellow)));
                            }
                            else
                            {
                                WriteConsoleMsg(UserIndex, "Susurraste> " + Chat, FontTypeNames.FONTTYPE_GM);
                                if (UserIndex != TargetUserIndex)
                                    WriteConsoleMsg(TargetUserIndex, "Gm susurra> " + Chat, FontTypeNames.FONTTYPE_GM);

                                if ((withBlock.flags.Privilegios &
                                     (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero)) != 0)
                                    modSendData.SendData(modSendData.SendTarget.ToAdminsAreaButConsejeros, UserIndex,
                                        PrepareMessageConsoleMsg(
                                            "Gm dijo a " + Declaraciones.UserList[TargetUserIndex].name + "> " + Chat,
                                            FontTypeNames.FONTTYPE_GM));
                            }
                        }
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleWhisper: " + ex.Message);
        }
    }

    // '
    // Handles the "Walk" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleWalk(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 13/01/2010 (ZaMa)
        // 11/19/09 Pato - Now the class bandit can walk hidden.
        // 13/01/2010: ZaMa - Now hidden on boat pirats recover the proper boat body.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        int dummy;
        int TempTick;
        Declaraciones.eHeading heading;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            heading = (Declaraciones.eHeading)withBlock.incomingData.ReadByte();

            // Prevent SpeedHack
            if (withBlock.flags.TimesWalk >= 30)
            {
                TempTick = Convert.ToInt32(Migration.GetTickCount());
                dummy = TempTick - withBlock.flags.StartWalk;

                // 5800 is actually less than what would be needed in perfect conditions to take 30 steps
                // (it's about 193 ms per step against the over 200 needed in perfect conditions)
                if (dummy < 5800)
                {
                    if (TempTick - withBlock.flags.CountSH > 30000) withBlock.flags.CountSH = 0;

                    if (!(withBlock.flags.CountSH == 0))
                    {
                        if (dummy != 0)
                            dummy = 126000 / dummy;

                        var argtexto = "Tramposo SH: " + withBlock.name + " , " + dummy;
                        General.LogHackAttemp(ref argtexto);
                        modSendData.SendData(modSendData.SendTarget.ToAdmins, 0,
                            PrepareMessageConsoleMsg(
                                "Servidor> " + withBlock.name +
                                " ha sido echado por el servidor por posible uso de SH.",
                                FontTypeNames.FONTTYPE_SERVER));
                        TCP.CloseSocket(UserIndex);

                        return;
                    }

                    withBlock.flags.CountSH = TempTick;
                }

                withBlock.flags.StartWalk = TempTick;
                withBlock.flags.TimesWalk = 0;
            }

            withBlock.flags.TimesWalk = withBlock.flags.TimesWalk + 1;

            // If exiting, cancel
            UsUaRiOs.CancelExit(UserIndex);

            // TODO: Debería decirle por consola que no puede?
            // Esta usando el /HOGAR, no se puede mover
            if (withBlock.flags.Traveling == 1)
                return;

            if (withBlock.flags.Paralizado == 0)
            {
                if (withBlock.flags.Meditando)
                {
                    // Stop meditating, next action will start movement.
                    withBlock.flags.Meditando = false;
                    withBlock.character.FX = 0;
                    withBlock.character.loops = 0;

                    WriteMeditateToggle(UserIndex);
                    WriteConsoleMsg(UserIndex, "Dejas de meditar.", FontTypeNames.FONTTYPE_INFO);

                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                        PrepareMessageCreateFX(withBlock.character.CharIndex, 0, 0));
                }
                else
                {
                    // Move user
                    UsUaRiOs.MoveUserChar(UserIndex, heading);

                    // Stop resting if needed
                    if (withBlock.flags.Descansar)
                    {
                        withBlock.flags.Descansar = false;

                        WriteRestOK(UserIndex);
                        WriteConsoleMsg(UserIndex, "Has dejado de descansar.", FontTypeNames.FONTTYPE_INFO);
                    }
                }
            }
            else // paralized
            {
                if (!(withBlock.flags.UltimoMensaje == 1))
                {
                    withBlock.flags.UltimoMensaje = 1;

                    WriteConsoleMsg(UserIndex, "No puedes moverte porque estás paralizado.",
                        FontTypeNames.FONTTYPE_INFO);
                }

                withBlock.flags.CountSH = 0;
            }

            // Can't move while hidden except he is a thief
            if ((withBlock.flags.Oculto == 1) & (withBlock.flags.AdminInvisible == 0))
                if ((withBlock.clase != Declaraciones.eClass.Thief) & (withBlock.clase != Declaraciones.eClass.Bandit))
                {
                    withBlock.flags.Oculto = 0;
                    withBlock.Counters.TiempoOculto = 0;

                    if (withBlock.flags.Navegando == 1)
                    {
                        if (withBlock.clase == Declaraciones.eClass.Pirat)
                        {
                            // Pierde la apariencia de fragata fantasmal
                            UsUaRiOs.ToogleBoatBody(UserIndex);
                            WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!",
                                FontTypeNames.FONTTYPE_INFO);
                            UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body, withBlock.character.Head,
                                (byte)withBlock.character.heading, Declaraciones.NingunArma,
                                Declaraciones.NingunEscudo, Declaraciones.NingunCasco);
                        }
                    }
                    // If not under a spell effect, show char
                    else if (withBlock.flags.invisible == 0)
                    {
                        WriteConsoleMsg(UserIndex, "Has vuelto a ser visible.", FontTypeNames.FONTTYPE_INFO);
                        UsUaRiOs.SetInvisible(UserIndex, withBlock.character.CharIndex, false);
                    }
                }
        }
    }

    // '
    // Handles the "RequestPositionUpdate" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestPositionUpdate(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        WritePosUpdate(UserIndex);
    }

    // '
    // Handles the "Attack" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleAttack(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 13/01/2010
        // Last Modified By: ZaMa
        // 10/01/2008: Tavo - Se cancela la salida del juego si el user esta saliendo.
        // 13/11/2009: ZaMa - Se cancela el estado no atacable al atcar.
        // 13/01/2010: ZaMa - Now hidden on boat pirats recover the proper boat body.
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // If dead, can't attack
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // If user meditates, can't attack
            if (withBlock.flags.Meditando) return;

            // If equiped weapon is ranged, can't attack this way
            if (withBlock.Invent.WeaponEqpObjIndex > 0)
                if (Declaraciones.objData[withBlock.Invent.WeaponEqpObjIndex].proyectil == 1)
                {
                    WriteConsoleMsg(UserIndex, "No puedes usar así este arma.", FontTypeNames.FONTTYPE_INFO);
                    return;
                }

            // If exiting, cancel
            UsUaRiOs.CancelExit(UserIndex);

            // Attack!
            SistemaCombate.UsuarioAtaca(UserIndex);

            // Now you can be atacked
            withBlock.flags.NoPuedeSerAtacado = false;

            // I see you...
            if ((withBlock.flags.Oculto > 0) & (withBlock.flags.AdminInvisible == 0))
            {
                withBlock.flags.Oculto = 0;
                withBlock.Counters.TiempoOculto = 0;

                if (withBlock.flags.Navegando == 1)
                {
                    if (withBlock.clase == Declaraciones.eClass.Pirat)
                    {
                        // Pierde la apariencia de fragata fantasmal
                        UsUaRiOs.ToogleBoatBody(UserIndex);
                        WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!",
                            FontTypeNames.FONTTYPE_INFO);
                        UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body, withBlock.character.Head,
                            (byte)withBlock.character.heading, Declaraciones.NingunArma, Declaraciones.NingunEscudo,
                            Declaraciones.NingunCasco);
                    }
                }
                else if (withBlock.flags.invisible == 0)
                {
                    UsUaRiOs.SetInvisible(UserIndex, withBlock.character.CharIndex, false);
                    WriteConsoleMsg(UserIndex, "¡Has vuelto a ser visible!", FontTypeNames.FONTTYPE_INFO);
                }
            }
        }
    }

    // '
    // Handles the "PickUp" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandlePickUp(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 07/25/09
        // 02/26/2006: Marco - Agregué un checkeo por si el usuario trata de agarrar un item mientras comercia.
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // If dead, it can't pick up objects
            if (withBlock.flags.Muerto == 1)
                return;

            // If user is trading items and attempts to pickup an item, he's cheating, so we kick him.
            if (withBlock.flags.Comerciando)
                return;

            // Lower rank administrators can't pick up items
            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.Consejero) != 0)
                if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) == 0)
                {
                    WriteConsoleMsg(UserIndex, "No puedes tomar ningún objeto.", FontTypeNames.FONTTYPE_INFO);
                    return;
                }

            InvUsuario.GetObj(UserIndex);
        }
    }

    // '
    // Handles the "SafeToggle" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleSafeToggle(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if (withBlock.flags.Seguro)
                WriteMultiMessage(UserIndex,
                    (short)Declaraciones.eMessages.SafeModeOff); // Call WriteSafeModeOff(UserIndex)
            else
                WriteMultiMessage(UserIndex,
                    (short)Declaraciones.eMessages.SafeModeOn); // Call WriteSafeModeOn(UserIndex)

            withBlock.flags.Seguro = !withBlock.flags.Seguro;
        }
    }

    // '
    // Handles the "ResuscitationSafeToggle" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleResuscitationToggle(short UserIndex)
    {
        // ***************************************************
        // Author: Rapsodius
        // Creation Date: 10/10/07
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            withBlock.incomingData.ReadByte();

            withBlock.flags.SeguroResu = !withBlock.flags.SeguroResu;

            if (withBlock.flags.SeguroResu)
                WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.ResuscitationSafeOn);
            // Call WriteResuscitationSafeOn(UserIndex)
            else
                WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.ResuscitationSafeOff);
            // Call WriteResuscitationSafeOff(UserIndex)
        }
    }

    // '
    // Handles the "RequestGuildLeaderInfo" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestGuildLeaderInfo(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        modGuilds.SendGuildLeaderInfo(UserIndex);
    }

    // '
    // Handles the "RequestAtributes" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestAtributes(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        WriteAttributes(UserIndex);
    }

    // '
    // Handles the "RequestFame" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestFame(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        UsUaRiOs.EnviarFama(UserIndex);
    }

    // '
    // Handles the "RequestSkills" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestSkills(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        WriteSendSkills(UserIndex);
    }

    // '
    // Handles the "RequestMiniStats" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestMiniStats(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        WriteMiniStats(UserIndex);
    }

    // '
    // Handles the "CommerceEnd" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCommerceEnd(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        // User quits commerce mode
        Declaraciones.UserList[UserIndex].flags.Comerciando = false;
        WriteCommerceEnd(UserIndex);
    }

    // '
    // Handles the "UserCommerceEnd" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleUserCommerceEnd(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 11/03/2010
        // 11/03/2010: ZaMa - Le avisa por consola al que cencela que dejo de comerciar.
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Quits commerce mode with user
            if (withBlock.ComUsu.DestUsu > 0)
                if (Declaraciones.UserList[withBlock.ComUsu.DestUsu].ComUsu.DestUsu == UserIndex)
                {
                    WriteConsoleMsg(withBlock.ComUsu.DestUsu, withBlock.name + " ha dejado de comerciar con vos.",
                        FontTypeNames.FONTTYPE_TALK);
                    mdlCOmercioConUsuario.FinComerciarUsu(withBlock.ComUsu.DestUsu);

                    // Send data in the outgoing buffer of the other user
                    FlushBuffer(withBlock.ComUsu.DestUsu);
                }

            mdlCOmercioConUsuario.FinComerciarUsu(UserIndex);
            WriteConsoleMsg(UserIndex, "Has dejado de comerciar.", FontTypeNames.FONTTYPE_TALK);
        }
    }

    // '
    // Handles the "UserCommerceConfirm" message.
    // 
    // @param    userIndex The index of the user sending the message.
    private static void HandleUserCommerceConfirm(short UserIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 14/12/2009
        // 
        // ***************************************************

        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        // Validate the commerce
        if (mdlCOmercioConUsuario.PuedeSeguirComerciando(UserIndex))
        {
            // Tell the other user the confirmation of the offer
            WriteUserOfferConfirm(Declaraciones.UserList[UserIndex].ComUsu.DestUsu);
            Declaraciones.UserList[UserIndex].ComUsu.Confirmo = true;
        }
    }

    private static void HandleCommerceChat(short UserIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 03/12/2009
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string Chat;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];

                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                Chat = buffer.ReadASCIIString();

                if (Migration.migr_LenB(Chat) != 0)
                    if (mdlCOmercioConUsuario.PuedeSeguirComerciando(UserIndex))
                    {
                        // Analize chat...
                        Statistics.ParseChat(ref Chat);

                        Chat = Declaraciones.UserList[UserIndex].name + "> " + Chat;
                        WriteCommerceChat(UserIndex, Chat, FontTypeNames.FONTTYPE_PARTY);
                        WriteCommerceChat(Declaraciones.UserList[UserIndex].ComUsu.DestUsu, Chat,
                            FontTypeNames.FONTTYPE_PARTY);
                    }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleCommerceChat: " + ex.Message);
        }
    }


    // '
    // Handles the "BankEnd" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleBankEnd(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // User exits banking mode
            withBlock.flags.Comerciando = false;
            WriteBankEnd(UserIndex);
        }
    }

    // '
    // Handles the "UserCommerceOk" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleUserCommerceOk(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        // Trade accepted
        mdlCOmercioConUsuario.AceptarComercioUsu(UserIndex);
    }

    // '
    // Handles the "UserCommerceReject" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleUserCommerceReject(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        short otherUser;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            otherUser = withBlock.ComUsu.DestUsu;

            // Offer rejected
            if (otherUser > 0)
                if (Declaraciones.UserList[otherUser].flags.UserLogged)
                {
                    WriteConsoleMsg(otherUser, withBlock.name + " ha rechazado tu oferta.",
                        FontTypeNames.FONTTYPE_TALK);
                    mdlCOmercioConUsuario.FinComerciarUsu(otherUser);

                    // Send data in the outgoing buffer of the other user
                    FlushBuffer(otherUser);
                }

            WriteConsoleMsg(UserIndex, "Has rechazado la oferta del otro usuario.", FontTypeNames.FONTTYPE_TALK);
            mdlCOmercioConUsuario.FinComerciarUsu(UserIndex);
        }
    }

    // '
    // Handles the "Drop" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleDrop(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 07/25/09
        // 07/25/09: Marco - Agregué un checkeo para patear a los usuarios que tiran items mientras comercian.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 4)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte Slot;
        short Amount;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            Slot = withBlock.incomingData.ReadByte();
            Amount = withBlock.incomingData.ReadInteger();


            // low rank admins can't drop item. Neither can the dead nor those sailing.
            if ((withBlock.flags.Navegando == 1) | (withBlock.flags.Muerto == 1) |
                (((withBlock.flags.Privilegios & Declaraciones.PlayerType.Consejero) != 0) &
                 ((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0)))
                return;

            // If the user is trading, he can't drop items => He's cheating, we kick him.
            if (withBlock.flags.Comerciando)
                return;

            // Are we dropping gold or other items??
            if (Slot == Declaraciones.FLAGORO)
            {
                if (Amount > 10000)
                    return; // Don't drop too much gold

                InvUsuario.TirarOro(Amount, UserIndex);

                WriteUpdateGold(UserIndex);
            }
            // Only drop valid slots
            else if ((Slot <= Declaraciones.MAX_INVENTORY_SLOTS) & (Slot > 0))
            {
                if (withBlock.Invent.userObj[Slot].ObjIndex == 0) return;

                InvUsuario.DropObj(UserIndex, Slot, Amount, withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y);
            }
        }
    }

    // '
    // Handles the "CastSpell" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCastSpell(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 13/11/2009: ZaMa - Ahora los npcs pueden atacar al usuario si quizo castear un hechizo
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte Spell;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            Spell = withBlock.incomingData.ReadByte();

            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Now you can be atacked
            withBlock.flags.NoPuedeSerAtacado = false;

            if (Spell < 1)
            {
                withBlock.flags.Hechizo = 0;
                return;
            }

            if (Spell > Declaraciones.MAXUSERHECHIZOS)
            {
                withBlock.flags.Hechizo = 0;
                return;
            }

            withBlock.flags.Hechizo = withBlock.Stats.UserHechizos[Spell];
        }
    }

    // '
    // Handles the "LeftClick" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleLeftClick(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte X;
        byte Y;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].incomingData;
            // Remove packet ID
            withBlock.ReadByte();


            X = withBlock.ReadByte();
            Y = withBlock.ReadByte();

            Extra.LookatTile(UserIndex, Declaraciones.UserList[UserIndex].Pos.Map, X, Y);
        }
    }

    // '
    // Handles the "DoubleClick" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleDoubleClick(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte X;
        byte Y;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].incomingData;
            // Remove packet ID
            withBlock.ReadByte();


            X = withBlock.ReadByte();
            Y = withBlock.ReadByte();

            Acciones.Accion(UserIndex, Declaraciones.UserList[UserIndex].Pos.Map, X, Y);
        }
    }

    // '
    // Handles the "Work" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleWork(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 13/01/2010 (ZaMa)
        // 13/01/2010: ZaMa - El pirata se puede ocultar en barca
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        Declaraciones.eSkill Skill;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            Skill = (Declaraciones.eSkill)withBlock.incomingData.ReadByte();

            if (Declaraciones.UserList[UserIndex].flags.Muerto == 1)
                return;

            // If exiting, cancel
            UsUaRiOs.CancelExit(UserIndex);

            switch (Skill)
            {
                case Declaraciones.eSkill.Robar:
                case Declaraciones.eSkill.Magia:
                case Declaraciones.eSkill.Domar:
                {
                    WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.WorkRequestTarget, (int)Skill);
                    break;
                }
                // Call WriteWorkRequestTarget(UserIndex, Skill)
                case Declaraciones.eSkill.Ocultarse:
                {
                    if (withBlock.flags.EnConsulta)
                    {
                        WriteConsoleMsg(UserIndex, "No puedes ocultarte si estás en consulta.",
                            FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    if (withBlock.flags.Navegando == 1)
                        if (withBlock.clase != Declaraciones.eClass.Pirat)
                        {
                            // [CDT 17-02-2004]
                            if (!(withBlock.flags.UltimoMensaje == 3))
                            {
                                WriteConsoleMsg(UserIndex, "No puedes ocultarte si estás navegando.",
                                    FontTypeNames.FONTTYPE_INFO);
                                withBlock.flags.UltimoMensaje = 3;
                            }

                            // [/CDT]
                            return;
                        }

                    if (withBlock.flags.Oculto == 1)
                    {
                        // [CDT 17-02-2004]
                        if (!(withBlock.flags.UltimoMensaje == 2))
                        {
                            WriteConsoleMsg(UserIndex, "Ya estás oculto.", FontTypeNames.FONTTYPE_INFO);
                            withBlock.flags.UltimoMensaje = 2;
                        }

                        // [/CDT]
                        return;
                    }

                    Trabajo.DoOcultarse(UserIndex);
                    break;
                }
            }
        }
    }

    // '
    // Handles the "InitCrafting" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleInitCrafting(short UserIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 29/01/2010
        // 
        // ***************************************************
        int TotalItems;
        short ItemsPorCiclo;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            TotalItems = withBlock.incomingData.ReadLong();
            ItemsPorCiclo = withBlock.incomingData.ReadInteger();

            if (TotalItems > 0)
            {
                withBlock.Construir.Cantidad = TotalItems;
                withBlock.Construir.PorCiclo =
                    Convert.ToInt16(SistemaCombate.MinimoInt(Trabajo.MaxItemsConstruibles(UserIndex), ItemsPorCiclo));
            }
        }
    }

    // '
    // Handles the "UseSpellMacro" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleUseSpellMacro(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            modSendData.SendData(modSendData.SendTarget.ToAdmins, UserIndex,
                PrepareMessageConsoleMsg(withBlock.name + " fue expulsado por Anti-macro de hechizos.",
                    FontTypeNames.FONTTYPE_VENENO));
            WriteErrorMsg(UserIndex,
                "Has sido expulsado por usar macro de hechizos. Recomendamos leer el reglamento sobre el tema macros.");
            FlushBuffer(UserIndex);
            TCP.CloseSocket(UserIndex);
        }
    }

    // '
    // Handles the "UseItem" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleUseItem(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte Slot;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            Slot = withBlock.incomingData.ReadByte();

            if ((Slot <= withBlock.CurrentInventorySlots) & (Slot > 0))
                if (withBlock.Invent.userObj[Slot].ObjIndex == 0)
                    return;

            if (withBlock.flags.Meditando) return; // The error message should have been provided by the client.

            InvUsuario.UseInvItem(UserIndex, Slot);
        }
    }

    // '
    // Handles the "CraftBlacksmith" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCraftBlacksmith(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        short Item;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].incomingData;
            // Remove packet ID
            withBlock.ReadByte();


            Item = withBlock.ReadInteger();

            if (Item < 1)
                return;

            if (Declaraciones.objData[Item].SkHerreria == 0)
                return;

            Trabajo.HerreroConstruirItem(UserIndex, Item);
        }
    }

    // '
    // Handles the "CraftCarpenter" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCraftCarpenter(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        short Item;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].incomingData;
            // Remove packet ID
            withBlock.ReadByte();


            Item = withBlock.ReadInteger();

            if (Item < 1)
                return;

            if (Declaraciones.objData[Item].SkCarpinteria == 0)
                return;

            Trabajo.CarpinteroConstruirItem(UserIndex, Item);
        }
    }

    // '
    // Handles the "WorkLeftClick" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleWorkLeftClick(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 14/01/2010 (ZaMa)
        // 16/11/2009: ZaMa - Agregada la posibilidad de extraer madera elfica.
        // 12/01/2010: ZaMa - Ahora se admiten armas arrojadizas (proyectiles sin municiones).
        // 14/01/2010: ZaMa - Ya no se pierden municiones al atacar npcs con dueño.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 4)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte X;
        byte Y;
        Declaraciones.eSkill Skill;
        var DummyInt = default(short);
        short tU;
        short tN;
        bool Atacked; // Target NPC 'Target user
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            X = withBlock.incomingData.ReadByte();
            Y = withBlock.incomingData.ReadByte();

            Skill = (Declaraciones.eSkill)withBlock.incomingData.ReadByte();


            if ((withBlock.flags.Muerto == 1) | withBlock.flags.Descansar | withBlock.flags.Meditando |
                !Extra.InMapBounds(withBlock.Pos.Map, X, Y))
                return;

            if (!Extra.InRangoVision(UserIndex, X, Y))
            {
                WritePosUpdate(UserIndex);
                return;
            }

            // If exiting, cancel
            UsUaRiOs.CancelExit(UserIndex);

            switch (Skill)
            {
                case Declaraciones.eSkill.Proyectiles:
                {
                    // Check attack interval
                    if (!modNuevoTimer.IntervaloPermiteAtacar(UserIndex, false))
                        return;
                    // Check Magic interval
                    if (!modNuevoTimer.IntervaloPermiteLanzarSpell(UserIndex, false))
                        return;
                    // Check bow's interval
                    if (!modNuevoTimer.IntervaloPermiteUsarArcos(UserIndex))
                        return;

                    Atacked = true;

                    // Make sure the item is valid and there is ammo equipped.
                    {
                        ref var withBlock1 = ref withBlock.Invent;
                        // Tiene arma equipada?
                        if (withBlock1.WeaponEqpObjIndex == 0)
                        {
                            DummyInt = 1;
                        }
                        // En un slot válido?
                        else if ((withBlock1.WeaponEqpSlot < 1) | (withBlock1.WeaponEqpSlot >
                                                                   Declaraciones.UserList[UserIndex]
                                                                       .CurrentInventorySlots))
                        {
                            DummyInt = 1;
                        }
                        // Usa munición? (Si no la usa, puede ser un arma arrojadiza)
                        else if (Declaraciones.objData[withBlock1.WeaponEqpObjIndex].Municion == 1)
                        {
                            // La municion esta equipada en un slot valido?
                            if ((withBlock1.MunicionEqpSlot < 1) | (withBlock1.MunicionEqpSlot >
                                                                    Declaraciones.UserList[UserIndex]
                                                                        .CurrentInventorySlots))
                                DummyInt = 1;
                            // Tiene munición?
                            else if (withBlock1.MunicionEqpObjIndex == 0)
                                DummyInt = 1;
                            // Son flechas?
                            else if (Declaraciones.objData[withBlock1.MunicionEqpObjIndex].OBJType !=
                                     Declaraciones.eOBJType.otFlechas)
                                DummyInt = 1;
                            // Tiene suficientes?
                            else if (withBlock1.userObj[withBlock1.MunicionEqpSlot].Amount < 1) DummyInt = 1;
                        }
                        // Es un arma de proyectiles?
                        else if (Declaraciones.objData[withBlock1.WeaponEqpObjIndex].proyectil != 1)
                        {
                            DummyInt = 2;
                        }

                        if (DummyInt != 0)
                        {
                            if (DummyInt == 1)
                            {
                                WriteConsoleMsg(UserIndex, "No tienes municiones.", FontTypeNames.FONTTYPE_INFO);

                                InvUsuario.Desequipar(UserIndex, withBlock1.WeaponEqpSlot);
                            }

                            InvUsuario.Desequipar(UserIndex, withBlock1.MunicionEqpSlot);
                            return;
                        }
                    }

                    // Quitamos stamina
                    if (withBlock.Stats.MinSta >= 10)
                    {
                        Trabajo.QuitarSta(UserIndex, Convert.ToInt16(Matematicas.RandomNumber(1, 10)));
                    }
                    else
                    {
                        if (withBlock.Genero == Declaraciones.eGenero.Hombre)
                            WriteConsoleMsg(UserIndex, "Estás muy cansado para luchar.", FontTypeNames.FONTTYPE_INFO);
                        else
                            WriteConsoleMsg(UserIndex, "Estás muy cansada para luchar.", FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    Extra.LookatTile(UserIndex, withBlock.Pos.Map, X, Y);

                    tU = withBlock.flags.TargetUser;
                    tN = withBlock.flags.TargetNPC;

                    // Validate target
                    if (tU > 0)
                    {
                        // Only allow to atack if the other one can retaliate (can see us)
                        if (Math.Abs((short)(Declaraciones.UserList[tU].Pos.Y - withBlock.Pos.Y)) > AI.RANGO_VISION_Y)
                        {
                            WriteConsoleMsg(UserIndex, "Estás demasiado lejos para atacar.",
                                FontTypeNames.FONTTYPE_WARNING);
                            return;
                        }

                        // Prevent from hitting self
                        if (tU == UserIndex)
                        {
                            WriteConsoleMsg(UserIndex, "¡No puedes atacarte a vos mismo!", FontTypeNames.FONTTYPE_INFO);
                            return;
                        }

                        // Attack!
                        Atacked = SistemaCombate.UsuarioAtacaUsuario(UserIndex, tU);
                    }

                    else if (tN > 0)
                    {
                        // Only allow to atack if the other one can retaliate (can see us)
                        if ((Math.Abs((short)(Declaraciones.Npclist[tN].Pos.Y - withBlock.Pos.Y)) > AI.RANGO_VISION_Y) &
                            (Math.Abs((short)(Declaraciones.Npclist[tN].Pos.X - withBlock.Pos.X)) > AI.RANGO_VISION_X))
                        {
                            WriteConsoleMsg(UserIndex, "Estás demasiado lejos para atacar.",
                                FontTypeNames.FONTTYPE_WARNING);
                            return;
                        }

                        // Is it attackable???
                        if (Declaraciones.Npclist[tN].Attackable != 0)
                            // Attack!
                            Atacked = SistemaCombate.UsuarioAtacaNpc(UserIndex, tN);
                    }

                    // Solo pierde la munición si pudo atacar al target, o tiro al aire
                    if (Atacked)
                    {
                        ref var withBlock2 = ref withBlock.Invent;
                        // Tiene equipado arco y flecha?
                        if (Declaraciones.objData[withBlock2.WeaponEqpObjIndex].Municion == 1)
                        {
                            DummyInt = withBlock2.MunicionEqpSlot;


                            // Take 1 arrow away - we do it AFTER hitting, since if Ammo Slot is 0 it gives a rt9 and kicks players
                            InvUsuario.QuitarUserInvItem(UserIndex, Convert.ToByte(DummyInt), 1);

                            if (withBlock2.userObj[DummyInt].Amount > 0)
                            {
                                // QuitarUserInvItem unequips the ammo, so we equip it again
                                withBlock2.MunicionEqpSlot = Convert.ToByte(DummyInt);
                                withBlock2.MunicionEqpObjIndex = withBlock2.userObj[DummyInt].ObjIndex;
                                withBlock2.userObj[DummyInt].Equipped = 1;
                            }
                            else
                            {
                                withBlock2.MunicionEqpSlot = 0;
                                withBlock2.MunicionEqpObjIndex = 0;
                            }
                        }
                        // Tiene equipado un arma arrojadiza
                        else
                        {
                            DummyInt = withBlock2.WeaponEqpSlot;

                            // Take 1 knife away
                            InvUsuario.QuitarUserInvItem(UserIndex, Convert.ToByte(DummyInt), 1);

                            if (withBlock2.userObj[DummyInt].Amount > 0)
                            {
                                // QuitarUserInvItem unequips the weapon, so we equip it again
                                withBlock2.WeaponEqpSlot = Convert.ToByte(DummyInt);
                                withBlock2.WeaponEqpObjIndex = withBlock2.userObj[DummyInt].ObjIndex;
                                withBlock2.userObj[DummyInt].Equipped = 1;
                            }
                            else
                            {
                                withBlock2.WeaponEqpSlot = 0;
                                withBlock2.WeaponEqpObjIndex = 0;
                            }
                        }

                        InvUsuario.UpdateUserInv(false, UserIndex, Convert.ToByte(DummyInt));
                    }

                    break;
                }

                case Declaraciones.eSkill.Magia:
                {
                    // Check the map allows spells to be casted.
                    if (Declaraciones.mapInfo[withBlock.Pos.Map].MagiaSinEfecto > 0)
                    {
                        WriteConsoleMsg(UserIndex, "Una fuerza oscura te impide canalizar tu energía.",
                            FontTypeNames.FONTTYPE_FIGHT);
                        return;
                    }

                    // Target whatever is in that tile
                    Extra.LookatTile(UserIndex, withBlock.Pos.Map, X, Y);

                    // If it's outside range log it and exit
                    if ((Math.Abs((short)(withBlock.Pos.X - X)) > AI.RANGO_VISION_X) |
                        (Math.Abs((short)(withBlock.Pos.Y - Y)) > AI.RANGO_VISION_Y))
                    {
                        var argtexto = "Ataque fuera de rango de " + withBlock.name + "(" + withBlock.Pos.Map + "/" +
                                       withBlock.Pos.X + "/" + withBlock.Pos.Y + ") ip: " + withBlock.ip +
                                       " a la posición (" + withBlock.Pos.Map + "/" + X + "/" + Y + ")";
                        General.LogCheating(ref argtexto);
                        return;
                    }

                    // Check bow's interval
                    if (!modNuevoTimer.IntervaloPermiteUsarArcos(UserIndex, false))
                        return;


                    // Check Spell-Hit interval
                    if (!modNuevoTimer.IntervaloPermiteGolpeMagia(UserIndex))
                        // Check Magic interval
                        if (!modNuevoTimer.IntervaloPermiteLanzarSpell(UserIndex))
                            return;


                    // Check intervals and cast
                    if (withBlock.flags.Hechizo > 0)
                    {
                        modHechizos.LanzarHechizo(withBlock.flags.Hechizo, UserIndex);
                        withBlock.flags.Hechizo = 0;
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, "¡Primero selecciona el hechizo que quieres lanzar!",
                            FontTypeNames.FONTTYPE_INFO);
                    }

                    break;
                }

                case Declaraciones.eSkill.Pesca:
                {
                    DummyInt = withBlock.Invent.WeaponEqpObjIndex;
                    if (DummyInt == 0)
                        return;

                    // Check interval
                    if (!modNuevoTimer.IntervaloPermiteTrabajar(UserIndex))
                        return;

                    // Basado en la idea de Barrin
                    // Comentario por Barrin: jah, "basado", caradura ! ^^
                    if ((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger == 1)
                    {
                        WriteConsoleMsg(UserIndex, "No puedes pescar desde donde te encuentras.",
                            FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    if (General.HayAgua(withBlock.Pos.Map, X, Y))
                    {
                        switch (DummyInt)
                        {
                            case Declaraciones.CAÑA_PESCA:
                            {
                                Trabajo.DoPescar(UserIndex);
                                break;
                            }

                            case Declaraciones.RED_PESCA:
                            {
                                if (Math.Abs((short)(withBlock.Pos.X - X)) + Math.Abs((short)(withBlock.Pos.Y - Y)) > 2)
                                {
                                    WriteConsoleMsg(UserIndex, "Estás demasiado lejos para pescar.",
                                        FontTypeNames.FONTTYPE_INFO);
                                    return;
                                }

                                Trabajo.DoPescarRed(UserIndex);
                                break;
                            }

                            default:
                            {
                                return; // Invalid item!
                            }
                        }

                        // Play sound!
                        modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                            PrepareMessagePlayWave(Declaraciones.SND_PESCAR, Convert.ToByte(withBlock.Pos.X),
                                Convert.ToByte(withBlock.Pos.Y)));
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, "No hay agua donde pescar. Busca un lago, río o mar.",
                            FontTypeNames.FONTTYPE_INFO);
                    }

                    break;
                }

                case Declaraciones.eSkill.Robar:
                {
                    // Does the map allow us to steal here?
                    if (Declaraciones.mapInfo[withBlock.Pos.Map].Pk)
                    {
                        // Check interval
                        if (!modNuevoTimer.IntervaloPermiteTrabajar(UserIndex))
                            return;

                        // Target whatever is in that tile
                        Extra.LookatTile(UserIndex, Declaraciones.UserList[UserIndex].Pos.Map, X, Y);

                        tU = withBlock.flags.TargetUser;

                        if ((tU > 0) & (tU != UserIndex))
                        {
                            // Can't steal administrative players
                            if ((Declaraciones.UserList[tU].flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                                if (Declaraciones.UserList[tU].flags.Muerto == 0)
                                {
                                    if (Math.Abs((short)(withBlock.Pos.X - X)) +
                                        Math.Abs((short)(withBlock.Pos.Y - Y)) > 1)
                                    {
                                        WriteConsoleMsg(UserIndex, "Estás demasiado lejos.",
                                            FontTypeNames.FONTTYPE_INFO);
                                        return;
                                    }

                                    // 17/09/02
                                    // Check the trigger
                                    if (Declaraciones.MapData[Declaraciones.UserList[tU].Pos.Map, X, Y].trigger ==
                                        Declaraciones.eTrigger.ZONASEGURA)
                                    {
                                        WriteConsoleMsg(UserIndex, "No puedes robar aquí.",
                                            FontTypeNames.FONTTYPE_WARNING);
                                        return;
                                    }

                                    if (Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y]
                                            .trigger == Declaraciones.eTrigger.ZONASEGURA)
                                    {
                                        WriteConsoleMsg(UserIndex, "No puedes robar aquí.",
                                            FontTypeNames.FONTTYPE_WARNING);
                                        return;
                                    }

                                    Trabajo.DoRobar(UserIndex, tU);
                                }
                        }
                        else
                        {
                            WriteConsoleMsg(UserIndex, "¡No hay a quien robarle!", FontTypeNames.FONTTYPE_INFO);
                        }
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, "¡No puedes robar en zonas seguras!", FontTypeNames.FONTTYPE_INFO);
                    }

                    break;
                }

                case Declaraciones.eSkill.Talar:
                {
                    // Check interval
                    if (!modNuevoTimer.IntervaloPermiteTrabajar(UserIndex))
                        return;

                    if (withBlock.Invent.WeaponEqpObjIndex == 0)
                    {
                        WriteConsoleMsg(UserIndex, "Deberías equiparte el hacha.", FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    if ((withBlock.Invent.WeaponEqpObjIndex != Declaraciones.HACHA_LEÑADOR) &
                        (withBlock.Invent.WeaponEqpObjIndex != Declaraciones.HACHA_LEÑA_ELFICA))
                        // Podemos llegar acá si el user equipó el anillo dsp de la U y antes del click
                        return;

                    DummyInt = Declaraciones.MapData[withBlock.Pos.Map, X, Y].ObjInfo.ObjIndex;

                    if (DummyInt > 0)
                    {
                        if (Math.Abs((short)(withBlock.Pos.X - X)) + Math.Abs((short)(withBlock.Pos.Y - Y)) > 2)
                        {
                            WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO);
                            return;
                        }

                        // Barrin 29/9/03
                        if ((withBlock.Pos.X == X) & (withBlock.Pos.Y == Y))
                        {
                            WriteConsoleMsg(UserIndex, "No puedes talar desde allí.", FontTypeNames.FONTTYPE_INFO);
                            return;
                        }

                        // ¿Hay un arbol donde clickeo?
                        if ((Declaraciones.objData[DummyInt].OBJType == Declaraciones.eOBJType.otArboles) &
                            (withBlock.Invent.WeaponEqpObjIndex == Declaraciones.HACHA_LEÑADOR))
                        {
                            modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                PrepareMessagePlayWave(Declaraciones.SND_TALAR, Convert.ToByte(withBlock.Pos.X),
                                    Convert.ToByte(withBlock.Pos.Y)));
                            Trabajo.DoTalar(UserIndex);
                        }
                        else if ((Declaraciones.objData[DummyInt].OBJType ==
                                  Declaraciones.eOBJType.otArbolElfico) & (withBlock.Invent.WeaponEqpObjIndex ==
                                                                           Declaraciones.HACHA_LEÑA_ELFICA))
                        {
                            if (withBlock.Invent.WeaponEqpObjIndex == Declaraciones.HACHA_LEÑA_ELFICA)
                            {
                                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                    PrepareMessagePlayWave(Declaraciones.SND_TALAR, Convert.ToByte(withBlock.Pos.X),
                                        Convert.ToByte(withBlock.Pos.Y)));
                                Trabajo.DoTalar(UserIndex, true);
                            }
                            else
                            {
                                WriteConsoleMsg(UserIndex, "El hacha utilizado no es suficientemente poderosa.",
                                    FontTypeNames.FONTTYPE_INFO);
                            }
                        }
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, "No hay ningún árbol ahí.", FontTypeNames.FONTTYPE_INFO);
                    }

                    break;
                }

                case Declaraciones.eSkill.Mineria:
                {
                    if (!modNuevoTimer.IntervaloPermiteTrabajar(UserIndex))
                        return;

                    if (withBlock.Invent.WeaponEqpObjIndex == 0)
                        return;

                    if (withBlock.Invent.WeaponEqpObjIndex != Declaraciones.PIQUETE_MINERO)
                        // Podemos llegar acá si el user equipó el anillo dsp de la U y antes del click
                        return;

                    // Target whatever is in the tile
                    Extra.LookatTile(UserIndex, withBlock.Pos.Map, X, Y);

                    DummyInt = Declaraciones.MapData[withBlock.Pos.Map, X, Y].ObjInfo.ObjIndex;

                    if (DummyInt > 0)
                    {
                        // Check distance
                        if (Math.Abs((short)(withBlock.Pos.X - X)) + Math.Abs((short)(withBlock.Pos.Y - Y)) > 2)
                        {
                            WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO);
                            return;
                        }

                        DummyInt = Declaraciones.MapData[withBlock.Pos.Map, X, Y].ObjInfo.ObjIndex; // CHECK
                        // ¿Hay un yacimiento donde clickeo?
                        if (Declaraciones.objData[DummyInt].OBJType == Declaraciones.eOBJType.otYacimiento)
                            Trabajo.DoMineria(UserIndex);
                        else
                            WriteConsoleMsg(UserIndex, "Ahí no hay ningún yacimiento.", FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, "Ahí no hay ningún yacimiento.", FontTypeNames.FONTTYPE_INFO);
                    }

                    break;
                }

                case Declaraciones.eSkill.Domar:
                {
                    // Modificado 25/11/02
                    // Optimizado y solucionado el bug de la doma de
                    // criaturas hostiles.

                    // Target whatever is that tile
                    Extra.LookatTile(UserIndex, withBlock.Pos.Map, X, Y);
                    tN = withBlock.flags.TargetNPC;

                    if (tN > 0)
                    {
                        if (Declaraciones.Npclist[tN].flags.Domable > 0)
                        {
                            if (Math.Abs((short)(withBlock.Pos.X - X)) + Math.Abs((short)(withBlock.Pos.Y - Y)) > 2)
                            {
                                WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO);
                                return;
                            }

                            if (Migration.migr_LenB(Declaraciones.Npclist[tN].flags.AttackedBy) != 0)
                            {
                                WriteConsoleMsg(UserIndex,
                                    "No puedes domar una criatura que está luchando con un jugador.",
                                    FontTypeNames.FONTTYPE_INFO);
                                return;
                            }

                            Trabajo.DoDomar(UserIndex, tN);
                        }
                        else
                        {
                            WriteConsoleMsg(UserIndex, "No puedes domar a esa criatura.", FontTypeNames.FONTTYPE_INFO);
                        }
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, "¡No hay ninguna criatura allí!", FontTypeNames.FONTTYPE_INFO);
                    }

                    break;
                }

                case (Declaraciones.eSkill)Declaraciones.FundirMetal: // UGLY!!! This is a constant, not a skill!!
                {
                    // Check interval
                    if (!modNuevoTimer.IntervaloPermiteTrabajar(UserIndex))
                        return;

                    // Check there is a proper item there
                    if (withBlock.flags.TargetObj > 0)
                    {
                        if (Declaraciones.objData[withBlock.flags.TargetObj].OBJType ==
                            Declaraciones.eOBJType.otFragua)
                        {
                            // Validate other items
                            if ((withBlock.flags.TargetObjInvSlot < 1) |
                                (withBlock.flags.TargetObjInvSlot > withBlock.CurrentInventorySlots)) return;

                            // 'chequeamos que no se zarpe duplicando oro
                            if (withBlock.Invent.userObj[withBlock.flags.TargetObjInvSlot].ObjIndex !=
                                withBlock.flags.TargetObjInvIndex)
                            {
                                if ((withBlock.Invent.userObj[withBlock.flags.TargetObjInvSlot].ObjIndex == 0) |
                                    (withBlock.Invent.userObj[withBlock.flags.TargetObjInvSlot].Amount == 0))
                                {
                                    WriteConsoleMsg(UserIndex, "No tienes más minerales.", FontTypeNames.FONTTYPE_INFO);
                                    return;
                                }

                                // 'FUISTE
                                WriteErrorMsg(UserIndex, "Has sido expulsado por el sistema anti cheats.");
                                FlushBuffer(UserIndex);
                                TCP.CloseSocket(UserIndex);
                                return;
                            }

                            if (Declaraciones.objData[withBlock.flags.TargetObjInvIndex].OBJType ==
                                Declaraciones.eOBJType.otMinerales)
                                Trabajo.FundirMineral(UserIndex);
                            else if (Declaraciones.objData[withBlock.flags.TargetObjInvIndex].OBJType ==
                                     Declaraciones.eOBJType.otWeapon) Trabajo.FundirArmas(UserIndex);
                        }
                        else
                        {
                            WriteConsoleMsg(UserIndex, "Ahí no hay ninguna fragua.", FontTypeNames.FONTTYPE_INFO);
                        }
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, "Ahí no hay ninguna fragua.", FontTypeNames.FONTTYPE_INFO);
                    }

                    break;
                }

                case Declaraciones.eSkill.Herreria:
                {
                    // Target wehatever is in that tile
                    Extra.LookatTile(UserIndex, withBlock.Pos.Map, X, Y);

                    if (withBlock.flags.TargetObj > 0)
                    {
                        if (Declaraciones.objData[withBlock.flags.TargetObj].OBJType ==
                            Declaraciones.eOBJType.otYunque)
                        {
                            InvUsuario.EnivarArmasConstruibles(UserIndex);
                            InvUsuario.EnivarArmadurasConstruibles(UserIndex);
                            WriteShowBlacksmithForm(UserIndex);
                        }
                        else
                        {
                            WriteConsoleMsg(UserIndex, "Ahí no hay ningún yunque.", FontTypeNames.FONTTYPE_INFO);
                        }
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, "Ahí no hay ningún yunque.", FontTypeNames.FONTTYPE_INFO);
                    }

                    break;
                }
            }
        }
    }

    // '
    // Handles the "CreateNewGuild" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCreateNewGuild(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/11/09
        // 05/11/09: Pato - Ahora se quitan los espacios del principio y del fin del nombre del clan
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 9)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string desc;
            string GuildName;
            string site;
            string[] codex;
            var errorStr = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                desc = buffer.ReadASCIIString();
                GuildName = buffer.ReadASCIIString().Trim();
                site = buffer.ReadASCIIString();
                codex = Strings.Split(buffer.ReadASCIIString(), Conversions.ToString(SEPARATOR));

                if (modGuilds.CrearNuevoClan(UserIndex, ref desc, ref GuildName, ref site, ref codex,
                        withBlock.FundandoGuildAlineacion, ref errorStr))
                {
                    modSendData.SendData(modSendData.SendTarget.ToAll, UserIndex,
                        PrepareMessageConsoleMsg(
                            withBlock.name + " fundó el clan " + GuildName + " de alineación " +
                            modGuilds.GuildAlignment(withBlock.GuildIndex) + ".", FontTypeNames.FONTTYPE_GUILD));
                    modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                        PrepareMessagePlayWave(44, Declaraciones.NO_3D_SOUND, Declaraciones.NO_3D_SOUND));


                    // Update tag
                    UsUaRiOs.RefreshCharStatus(UserIndex);
                }
                else
                {
                    WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD);
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleCreateNewGuild: " + ex.Message);
        }
    }

    // '
    // Handles the "SpellInfo" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleSpellInfo(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte spellSlot;
        short Spell;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            spellSlot = withBlock.incomingData.ReadByte();

            // Validate slot
            if ((spellSlot < 1) | (spellSlot > Declaraciones.MAXUSERHECHIZOS))
            {
                WriteConsoleMsg(UserIndex, "¡Primero selecciona el hechizo!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Validate spell in the slot
            Spell = withBlock.Stats.UserHechizos[spellSlot];
            if ((Spell > 0) & (Spell < Declaraciones.NumeroHechizos + 1))
            {
                ref var withBlock1 = ref Declaraciones.Hechizos[Spell];
                // Send information
                WriteConsoleMsg(UserIndex,
                    "%%%%%%%%%%%% INFO DEL HECHIZO %%%%%%%%%%%%" + Constants.vbCrLf + "Nombre:" + withBlock1.Nombre +
                    Constants.vbCrLf + "Descripción:" + withBlock1.desc + Constants.vbCrLf + "Skill requerido: " +
                    withBlock1.MinSkill + " de magia." + Constants.vbCrLf + "Maná necesario: " +
                    withBlock1.ManaRequerido + Constants.vbCrLf + "Energía necesaria: " + withBlock1.StaRequerido +
                    Constants.vbCrLf + "%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%", FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Handles the "EquipItem" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleEquipItem(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte itemSlot;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            itemSlot = withBlock.incomingData.ReadByte();

            // Dead users can't equip items
            if (withBlock.flags.Muerto == 1)
                return;

            // Validate item slot
            if ((itemSlot > withBlock.CurrentInventorySlots) | (itemSlot < 1))
                return;

            if (withBlock.Invent.userObj[itemSlot].ObjIndex == 0)
                return;

            InvUsuario.EquiparInvItem(UserIndex, itemSlot);
        }
    }

    // '
    // Handles the "ChangeHeading" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleChangeHeading(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 06/28/2008
        // Last Modified By: NicoNZ
        // 10/01/2008: Tavo - Se cancela la salida del juego si el user esta saliendo
        // 06/28/2008: NicoNZ - Sólo se puede cambiar si está inmovilizado.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        Declaraciones.eHeading heading;
        var posX = default(short);
        var posY = default(short);
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            heading = (Declaraciones.eHeading)withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Paralizado == 1) & (withBlock.flags.Inmovilizado == 0))
            {
                switch (heading)
                {
                    case Declaraciones.eHeading.NORTH:
                    {
                        posY = -1;
                        break;
                    }
                    case Declaraciones.eHeading.EAST:
                    {
                        posX = 1;
                        break;
                    }
                    case Declaraciones.eHeading.SOUTH:
                    {
                        posY = 1;
                        break;
                    }
                    case Declaraciones.eHeading.WEST:
                    {
                        posX = -1;
                        break;
                    }
                }

                if (Extra.LegalPos(withBlock.Pos.Map, (short)(withBlock.Pos.X + posX), (short)(withBlock.Pos.Y + posY),
                        Convert.ToBoolean(withBlock.flags.Navegando),
                        !Convert.ToBoolean(withBlock.flags.Navegando))) return;
            }

            // Validate heading (VB won't say invalid cast if not a valid index like .Net languages would do... *sigh*)
            if (((int)heading > 0) & ((int)heading < 5))
            {
                withBlock.character.heading = heading;
                UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body, withBlock.character.Head,
                    (byte)withBlock.character.heading, withBlock.character.WeaponAnim,
                    withBlock.character.ShieldAnim, withBlock.character.CascoAnim);
            }
        }
    }

    // '
    // Handles the "ModifySkills" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleModifySkills(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 11/19/09
        // 11/19/09: Pato - Adapting to new skills system.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 1 + Declaraciones.NUMSKILLS)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        int i;
        var Count = default(short);
        // UPGRADE_WARNING: El límite inferior de la matriz points ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        var points = new byte[Declaraciones.NUMSKILLS + 1];
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            // Codigo para prevenir el hackeo de los skills
            // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            for (i = 1; i <= Declaraciones.NUMSKILLS; i++)
            {
                points[i] = withBlock.incomingData.ReadByte();

                if (points[i] < 0)
                {
                    var argtexto = withBlock.name + " IP:" + withBlock.ip + " trató de hackear los skills.";
                    General.LogHackAttemp(ref argtexto);
                    withBlock.Stats.SkillPts = 0;
                    TCP.CloseSocket(UserIndex);
                    return;
                }

                Count = (short)(Count + points[i]);
            }

            if (Count > withBlock.Stats.SkillPts)
            {
                var argtexto1 = withBlock.name + " IP:" + withBlock.ip + " trató de hackear los skills.";
                General.LogHackAttemp(ref argtexto1);
                TCP.CloseSocket(UserIndex);
                return;
            }
            // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

            withBlock.Counters.AsignedSkills =
                Convert.ToByte(SistemaCombate.MinimoInt(10, withBlock.Counters.AsignedSkills + Count));

            {
                ref var withBlock1 = ref withBlock.Stats;
                for (i = 1; i <= Declaraciones.NUMSKILLS; i++)
                    if (points[i] > 0)
                    {
                        withBlock1.SkillPts = (short)(withBlock1.SkillPts - points[i]);
                        withBlock1.UserSkills[i] = (byte)(withBlock1.UserSkills[i] + points[i]);

                        // Client should prevent this, but just in case...
                        if (withBlock1.UserSkills[i] > 100)
                        {
                            withBlock1.SkillPts = Convert.ToInt16(withBlock1.SkillPts + withBlock1.UserSkills[i] - 100);
                            withBlock1.UserSkills[i] = 100;
                        }

                        UsUaRiOs.CheckEluSkill(UserIndex, Convert.ToByte(i), true);
                    }
            }
        }
    }

    // '
    // Handles the "Train" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleTrain(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        short SpawnedNpc;
        byte PetIndex;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            PetIndex = withBlock.incomingData.ReadByte();

            if (withBlock.flags.TargetNPC == 0)
                return;

            if (Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype != Declaraciones.eNPCType.Entrenador)
                return;

            if (Declaraciones.Npclist[withBlock.flags.TargetNPC].Mascotas < Declaraciones.MAXMASCOTASENTRENADOR)
            {
                if ((PetIndex > 0) & (PetIndex < Declaraciones.Npclist[withBlock.flags.TargetNPC].NroCriaturas + 1))
                {
                    // Create the creature
                    SpawnedNpc =
                        NPCs.SpawnNpc(Declaraciones.Npclist[withBlock.flags.TargetNPC].Criaturas[PetIndex].NpcIndex,
                            ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos, true, false);

                    if (SpawnedNpc > 0)
                    {
                        Declaraciones.Npclist[SpawnedNpc].MaestroNpc = withBlock.flags.TargetNPC;
                        Declaraciones.Npclist[withBlock.flags.TargetNPC].Mascotas =
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].Mascotas + 1);
                    }
                }
            }
            else
            {
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    PrepareMessageChatOverHead("No puedo traer más criaturas, mata las existentes.",
                        Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                        ColorTranslator.ToOle(Color.White)));
            }
        }
    }

    // '
    // Handles the "CommerceBuy" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCommerceBuy(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 4)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte Slot;
        short Amount;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            Slot = withBlock.incomingData.ReadByte();
            Amount = withBlock.incomingData.ReadInteger();

            // Dead people can't commerce...
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // ¿El target es un NPC valido?
            if (withBlock.flags.TargetNPC < 1)
                return;

            // ¿El NPC puede comerciar?
            if (Declaraciones.Npclist[withBlock.flags.TargetNPC].Comercia == 0)
            {
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    PrepareMessageChatOverHead("No tengo ningún interés en comerciar.",
                        Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                        ColorTranslator.ToOle(Color.White)));
                return;
            }

            // Only if in commerce mode....
            if (!withBlock.flags.Comerciando)
            {
                WriteConsoleMsg(UserIndex, "No estás comerciando.", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // User compra el item
            modSistemaComercio.Comercio(modSistemaComercio.eModoComercio.Compra, UserIndex, withBlock.flags.TargetNPC,
                Slot, Amount);
        }
    }

    // '
    // Handles the "BankExtractItem" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleBankExtractItem(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 4)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte Slot;
        short Amount;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            Slot = withBlock.incomingData.ReadByte();
            Amount = withBlock.incomingData.ReadInteger();

            // Dead people can't commerce
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // ¿El target es un NPC valido?
            if (withBlock.flags.TargetNPC < 1)
                return;

            // ¿Es el banquero?
            if (Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype != Declaraciones.eNPCType.Banquero) return;

            // User retira el item del slot
            modBanco.UserRetiraItem(UserIndex, Slot, Amount);
        }
    }

    // '
    // Handles the "CommerceSell" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCommerceSell(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 4)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte Slot;
        short Amount;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            Slot = withBlock.incomingData.ReadByte();
            Amount = withBlock.incomingData.ReadInteger();

            // Dead people can't commerce...
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // ¿El target es un NPC valido?
            if (withBlock.flags.TargetNPC < 1)
                return;

            // ¿El NPC puede comerciar?
            if (Declaraciones.Npclist[withBlock.flags.TargetNPC].Comercia == 0)
            {
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    PrepareMessageChatOverHead("No tengo ningún interés en comerciar.",
                        Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                        ColorTranslator.ToOle(Color.White)));
                return;
            }

            // User compra el item del slot
            modSistemaComercio.Comercio(modSistemaComercio.eModoComercio.Venta, UserIndex, withBlock.flags.TargetNPC,
                Slot, Amount);
        }
    }

    // '
    // Handles the "BankDeposit" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleBankDeposit(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 4)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte Slot;
        short Amount;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            Slot = withBlock.incomingData.ReadByte();
            Amount = withBlock.incomingData.ReadInteger();

            // Dead people can't commerce...
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // ¿El target es un NPC valido?
            if (withBlock.flags.TargetNPC < 1)
                return;

            // ¿El NPC puede comerciar?
            if (Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype != Declaraciones.eNPCType.Banquero) return;

            // User deposita el item del slot rdata
            modBanco.UserDepositaItem(UserIndex, Slot, Amount);
        }
    }

    // '
    // Handles the "ForumPost" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleForumPost(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 02/01/2010
        // 02/01/2010: ZaMa - Implemento nuevo sistema de foros
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 6)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            Declaraciones.eForumMsgType ForumMsgType;
            string Title;
            string Post;
            var ForumIndex = default(short);
            byte ForumType;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                ForumMsgType = (Declaraciones.eForumMsgType)buffer.ReadByte();

                Title = buffer.ReadASCIIString();
                Post = buffer.ReadASCIIString();

                if (withBlock.flags.TargetObj > 0)
                {
                    ForumType = modForum.ForumAlignment((byte)ForumMsgType);

                    switch (ForumType)
                    {
                        case (byte)Declaraciones.eForumType.ieGeneral:
                        {
                            ForumIndex =
                                modForum.GetForumIndex(ref Declaraciones.objData[withBlock.flags.TargetObj]
                                    .ForoID);
                            break;
                        }

                        case (byte)Declaraciones.eForumType.ieREAL:
                        {
                            var argsForoID = modForum.FORO_REAL_ID;
                            ForumIndex = modForum.GetForumIndex(ref argsForoID);
                            break;
                        }

                        case (byte)Declaraciones.eForumType.ieCAOS:
                        {
                            var argsForoID1 = modForum.FORO_CAOS_ID;
                            ForumIndex = modForum.GetForumIndex(ref argsForoID1);
                            break;
                        }
                    }

                    modForum.AddPost(ForumIndex, ref Post, ref withBlock.name, ref Title,
                        modForum.EsAnuncio((byte)ForumMsgType));
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleForumPost: " + ex.Message);
        }
    }

    // '
    // Handles the "MoveSpell" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleMoveSpell(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        short dir;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].incomingData;
            // Remove packet ID
            withBlock.ReadByte();


            if (withBlock.ReadBoolean())
                dir = 1;
            else
                dir = -1;

            modHechizos.DesplazarHechizo(UserIndex, dir, withBlock.ReadByte());
        }
    }

    // '
    // Handles the "MoveBank" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleMoveBank(short UserIndex)
    {
        // ***************************************************
        // Author: Torres Patricio (Pato)
        // Last Modification: 06/14/09
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte Slot;
        Declaraciones.Obj TempItem;
        short dir;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].incomingData;
            // Remove packet ID
            withBlock.ReadByte();


            if (withBlock.ReadBoolean())
                dir = 1;
            else
                dir = -1;

            Slot = withBlock.ReadByte();
        }

        {
            ref var withBlock1 = ref Declaraciones.UserList[UserIndex];
            TempItem.ObjIndex = withBlock1.BancoInvent.userObj[Slot].ObjIndex;
            TempItem.Amount = withBlock1.BancoInvent.userObj[Slot].Amount;

            if (dir == 1) // Mover arriba
            {
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().BancoInvent.Object(Slot). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                withBlock1.BancoInvent.userObj[Slot] = withBlock1.BancoInvent.userObj[Slot - 1];
                withBlock1.BancoInvent.userObj[Slot - 1].ObjIndex = TempItem.ObjIndex;
                withBlock1.BancoInvent.userObj[Slot - 1].Amount = TempItem.Amount;
            }
            else // mover abajo
            {
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().BancoInvent.Object(Slot). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                withBlock1.BancoInvent.userObj[Slot] = withBlock1.BancoInvent.userObj[Slot + 1];
                withBlock1.BancoInvent.userObj[Slot + 1].ObjIndex = TempItem.ObjIndex;
                withBlock1.BancoInvent.userObj[Slot + 1].Amount = TempItem.Amount;
            }
        }

        modBanco.UpdateBanUserInv(true, UserIndex, 0);
        modBanco.UpdateVentanaBanco(UserIndex);
    }

    // '
    // Handles the "ClanCodexUpdate" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleClanCodexUpdate(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string desc;
            string[] codex;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                desc = buffer.ReadASCIIString();
                codex = Strings.Split(buffer.ReadASCIIString(), Conversions.ToString(SEPARATOR));

                modGuilds.ChangeCodexAndDesc(ref desc, ref codex, withBlock.GuildIndex);

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleClanCodexUpdate: " + ex.Message);
        }
    }

    // '
    // Handles the "UserCommerceOffer" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleUserCommerceOffer(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 24/11/2009
        // 24/11/2009: ZaMa - Nuevo sistema de comercio
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 7)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        int Amount;
        byte Slot;
        short tUser;
        byte OfferSlot;
        var ObjIndex = default(short);
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            Slot = withBlock.incomingData.ReadByte();
            Amount = withBlock.incomingData.ReadLong();
            OfferSlot = withBlock.incomingData.ReadByte();

            // Get the other player
            tUser = withBlock.ComUsu.DestUsu;

            // If he's already confirmed his offer, but now tries to change it, then he's cheating
            if (Declaraciones.UserList[UserIndex].ComUsu.Confirmo)
            {
                // Finish the trade
                mdlCOmercioConUsuario.FinComerciarUsu(UserIndex);

                if ((tUser <= 0) | (tUser > Declaraciones.MaxUsers))
                {
                    mdlCOmercioConUsuario.FinComerciarUsu(tUser);
                    FlushBuffer(tUser);
                }

                return;
            }

            // If slot is invalid and it's not gold or it's not 0 (Substracting), then ignore it.
            if (((Slot < 0) | (Slot > Declaraciones.UserList[UserIndex].CurrentInventorySlots)) &
                (Slot != Declaraciones.FLAGORO))
                return;

            // If OfferSlot is invalid, then ignore it.
            if ((OfferSlot < 1) | (OfferSlot > mdlCOmercioConUsuario.MAX_OFFER_SLOTS + 1))
                return;

            // Can be negative if substracted from the offer, but never 0.
            if (Amount == 0)
                return;

            // Has he got enough??
            if (Slot == Declaraciones.FLAGORO)
            {
                // Can't offer more than he has
                if (Amount > withBlock.Stats.GLD - withBlock.ComUsu.GoldAmount)
                {
                    WriteCommerceChat(UserIndex, "No tienes esa cantidad de oro para agregar a la oferta.",
                        FontTypeNames.FONTTYPE_TALK);
                    return;
                }
            }
            else
            {
                // If modifing a filled offerSlot, we already got the objIndex, then we don't need to know it
                if (Slot != 0)
                    ObjIndex = withBlock.Invent.userObj[Slot].ObjIndex;
                // Can't offer more than he has
                if (!Trabajo.TieneObjetos(ObjIndex,
                        Convert.ToInt16(UsUaRiOs.TotalOfferItems(ObjIndex, UserIndex) + Amount), UserIndex))
                {
                    WriteCommerceChat(UserIndex, "No tienes esa cantidad.", FontTypeNames.FONTTYPE_TALK);
                    return;
                }

                if (InvUsuario.ItemNewbie(ObjIndex))
                {
                    WriteCancelOfferItem(UserIndex, OfferSlot);
                    return;
                }

                // Don't allow to sell boats if they are equipped (you can't take them off in the water and causes trouble)
                if (withBlock.flags.Navegando == 1)
                    if (withBlock.Invent.BarcoSlot == Slot)
                    {
                        WriteCommerceChat(UserIndex, "No puedes vender tu barco mientras lo estés usando.",
                            FontTypeNames.FONTTYPE_TALK);
                        return;
                    }

                if (withBlock.Invent.MochilaEqpSlot > 0)
                    if (withBlock.Invent.MochilaEqpSlot == Slot)
                    {
                        WriteCommerceChat(UserIndex, "No puedes vender tu mochila mientras la estés usando.",
                            FontTypeNames.FONTTYPE_TALK);
                        return;
                    }
            }


            mdlCOmercioConUsuario.AgregarOferta(UserIndex, OfferSlot, ObjIndex, Amount, Slot == Declaraciones.FLAGORO);

            mdlCOmercioConUsuario.EnviarOferta(tUser, OfferSlot);
        }
    }

    // '
    // Handles the "GuildAcceptPeace" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildAcceptPeace(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string guild;
            var errorStr = default(string);
            string otherClanIndex;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                guild = buffer.ReadASCIIString();

                otherClanIndex = modGuilds.r_AceptarPropuestaDePaz(UserIndex, ref guild, ref errorStr).ToString();

                if (Convert.ToDouble(otherClanIndex) == 0d)
                {
                    WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD);
                }
                else
                {
                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, withBlock.GuildIndex,
                        PrepareMessageConsoleMsg("Tu clan ha firmado la paz con " + guild + ".",
                            FontTypeNames.FONTTYPE_GUILD));
                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, Convert.ToInt16(otherClanIndex),
                        PrepareMessageConsoleMsg(
                            "Tu clan ha firmado la paz con " + modGuilds.GuildName(withBlock.GuildIndex) + ".",
                            FontTypeNames.FONTTYPE_GUILD));
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildAcceptPeace: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildRejectAlliance" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildRejectAlliance(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string guild;
            var errorStr = default(string);
            string otherClanIndex;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                guild = buffer.ReadASCIIString();

                otherClanIndex = modGuilds.r_RechazarPropuestaDeAlianza(UserIndex, ref guild, ref errorStr).ToString();

                if (Convert.ToDouble(otherClanIndex) == 0d)
                {
                    WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD);
                }
                else
                {
                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, withBlock.GuildIndex,
                        PrepareMessageConsoleMsg("Tu clan rechazado la propuesta de alianza de " + guild,
                            FontTypeNames.FONTTYPE_GUILD));
                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, Convert.ToInt16(otherClanIndex),
                        PrepareMessageConsoleMsg(
                            modGuilds.GuildName(withBlock.GuildIndex) +
                            " ha rechazado nuestra propuesta de alianza con su clan.", FontTypeNames.FONTTYPE_GUILD));
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildRejectAlliance: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildRejectPeace" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildRejectPeace(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string guild;
            var errorStr = default(string);
            string otherClanIndex;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                guild = buffer.ReadASCIIString();

                otherClanIndex = modGuilds.r_RechazarPropuestaDePaz(UserIndex, ref guild, ref errorStr).ToString();

                if (Convert.ToDouble(otherClanIndex) == 0d)
                {
                    WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD);
                }
                else
                {
                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, withBlock.GuildIndex,
                        PrepareMessageConsoleMsg("Tu clan rechazado la propuesta de paz de " + guild + ".",
                            FontTypeNames.FONTTYPE_GUILD));
                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, Convert.ToInt16(otherClanIndex),
                        PrepareMessageConsoleMsg(
                            modGuilds.GuildName(withBlock.GuildIndex) +
                            " ha rechazado nuestra propuesta de paz con su clan.", FontTypeNames.FONTTYPE_GUILD));
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildRejectPeace: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildAcceptAlliance" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildAcceptAlliance(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string guild;
            var errorStr = default(string);
            string otherClanIndex;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                guild = buffer.ReadASCIIString();

                otherClanIndex = modGuilds.r_AceptarPropuestaDeAlianza(UserIndex, ref guild, ref errorStr).ToString();

                if (Convert.ToDouble(otherClanIndex) == 0d)
                {
                    WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD);
                }
                else
                {
                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, withBlock.GuildIndex,
                        PrepareMessageConsoleMsg("Tu clan ha firmado la alianza con " + guild + ".",
                            FontTypeNames.FONTTYPE_GUILD));
                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, Convert.ToInt16(otherClanIndex),
                        PrepareMessageConsoleMsg(
                            "Tu clan ha firmado la paz con " + modGuilds.GuildName(withBlock.GuildIndex) + ".",
                            FontTypeNames.FONTTYPE_GUILD));
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildAcceptAlliance: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildOfferPeace" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildOfferPeace(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string guild;
            string proposal;
            var errorStr = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                guild = buffer.ReadASCIIString();
                proposal = buffer.ReadASCIIString();

                if (modGuilds.r_ClanGeneraPropuesta(UserIndex, ref guild, modGuilds.RELACIONES_GUILD.PAZ, ref proposal,
                        ref errorStr))
                    WriteConsoleMsg(UserIndex, "Propuesta de paz enviada.", FontTypeNames.FONTTYPE_GUILD);
                else
                    WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD);

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildOfferPeace: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildOfferAlliance" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildOfferAlliance(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string guild;
            string proposal;
            var errorStr = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                guild = buffer.ReadASCIIString();
                proposal = buffer.ReadASCIIString();

                if (modGuilds.r_ClanGeneraPropuesta(UserIndex, ref guild, modGuilds.RELACIONES_GUILD.ALIADOS,
                        ref proposal, ref errorStr))
                    WriteConsoleMsg(UserIndex, "Propuesta de alianza enviada.", FontTypeNames.FONTTYPE_GUILD);
                else
                    WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD);

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildOfferAlliance: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildAllianceDetails" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildAllianceDetails(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string guild;
            var errorStr = default(string);
            string details;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                guild = buffer.ReadASCIIString();

                details = modGuilds.r_VerPropuesta(UserIndex, ref guild, modGuilds.RELACIONES_GUILD.ALIADOS,
                    ref errorStr);

                if (Migration.migr_LenB(details) == 0)
                    WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD);
                else
                    WriteOfferDetails(UserIndex, details);

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildAllianceDetails: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildPeaceDetails" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildPeaceDetails(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string guild;
            var errorStr = default(string);
            string details;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                guild = buffer.ReadASCIIString();

                details = modGuilds.r_VerPropuesta(UserIndex, ref guild, modGuilds.RELACIONES_GUILD.PAZ, ref errorStr);

                if (Migration.migr_LenB(details) == 0)
                    WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD);
                else
                    WriteOfferDetails(UserIndex, details);

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildPeaceDetails: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildRequestJoinerInfo" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildRequestJoinerInfo(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string details;
            string user;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                user = buffer.ReadASCIIString();

                details = modGuilds.a_DetallesAspirante(UserIndex, ref user);

                if (Migration.migr_LenB(details) == 0)
                    WriteConsoleMsg(UserIndex,
                        "El personaje no ha mandado solicitud, o no estás habilitado para verla.",
                        FontTypeNames.FONTTYPE_GUILD);
                else
                    WriteShowUserRequest(UserIndex, details);

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildRequestJoinerInfo: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildAlliancePropList" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildAlliancePropList(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        WriteAlianceProposalsList(UserIndex,
            modGuilds.r_ListaDePropuestas(UserIndex, modGuilds.RELACIONES_GUILD.ALIADOS));
    }

    // '
    // Handles the "GuildPeacePropList" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildPeacePropList(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        WritePeaceProposalsList(UserIndex, modGuilds.r_ListaDePropuestas(UserIndex, modGuilds.RELACIONES_GUILD.PAZ));
    }

    // '
    // Handles the "GuildDeclareWar" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildDeclareWar(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string guild;
            var errorStr = default(string);
            short otherGuildIndex;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                guild = buffer.ReadASCIIString();

                otherGuildIndex = modGuilds.r_DeclararGuerra(UserIndex, ref guild, ref errorStr);

                if (otherGuildIndex == 0)
                {
                    WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD);
                }
                else
                {
                    // WAR shall be!
                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, withBlock.GuildIndex,
                        PrepareMessageConsoleMsg("TU CLAN HA ENTRADO EN GUERRA CON " + guild + ".",
                            FontTypeNames.FONTTYPE_GUILD));
                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, otherGuildIndex,
                        PrepareMessageConsoleMsg(
                            modGuilds.GuildName(withBlock.GuildIndex) + " LE DECLARA LA GUERRA A TU CLAN.",
                            FontTypeNames.FONTTYPE_GUILD));
                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, withBlock.GuildIndex,
                        PrepareMessagePlayWave(45, Declaraciones.NO_3D_SOUND, Declaraciones.NO_3D_SOUND));
                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, otherGuildIndex,
                        PrepareMessagePlayWave(45, Declaraciones.NO_3D_SOUND, Declaraciones.NO_3D_SOUND));
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildDeclareWar: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildNewWebsite" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildNewWebsite(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                var argWeb = buffer.ReadASCIIString();
                modGuilds.ActualizarWebSite(UserIndex, ref argWeb);

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildNewWebsite: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildAcceptNewMember" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildAcceptNewMember(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            var errorStr = default(string);
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (!modGuilds.a_AceptarAspirante(UserIndex, ref UserName, ref errorStr))
                {
                    WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD);
                }
                else
                {
                    tUser = Extra.NameIndex(UserName);
                    if (tUser > 0)
                    {
                        modGuilds.m_ConectarMiembroAClan(tUser, withBlock.GuildIndex);
                        UsUaRiOs.RefreshCharStatus(tUser);
                    }

                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, withBlock.GuildIndex,
                        PrepareMessageConsoleMsg(UserName + " ha sido aceptado como miembro del clan.",
                            FontTypeNames.FONTTYPE_GUILD));
                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, withBlock.GuildIndex,
                        PrepareMessagePlayWave(43, Declaraciones.NO_3D_SOUND, Declaraciones.NO_3D_SOUND));
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildAcceptNewMember: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildRejectNewMember" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildRejectNewMember(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 01/08/07
        // Last Modification by: (liquid)
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            var errorStr = default(string);
            string UserName;
            string reason;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();
                reason = buffer.ReadASCIIString();

                if (!modGuilds.a_RechazarAspirante(UserIndex, ref UserName, ref errorStr))
                {
                    WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD);
                }
                else
                {
                    tUser = Extra.NameIndex(UserName);

                    if (tUser > 0)
                        WriteConsoleMsg(tUser, errorStr + " : " + reason, FontTypeNames.FONTTYPE_GUILD);
                    else
                        // hay que grabar en el char su rechazo
                        modGuilds.a_RechazarAspiranteChar(ref UserName, withBlock.GuildIndex, ref reason);
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildRejectNewMember: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildKickMember" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildKickMember(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short GuildIndex;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                GuildIndex = modGuilds.m_EcharMiembroDeClan(UserIndex, UserName);

                if (GuildIndex > 0)
                {
                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, GuildIndex,
                        PrepareMessageConsoleMsg(UserName + " fue expulsado del clan.", FontTypeNames.FONTTYPE_GUILD));
                    modSendData.SendData(modSendData.SendTarget.ToGuildMembers, GuildIndex,
                        PrepareMessagePlayWave(45, Declaraciones.NO_3D_SOUND, Declaraciones.NO_3D_SOUND));
                }
                else
                {
                    WriteConsoleMsg(UserIndex, "No puedes expulsar ese personaje del clan.",
                        FontTypeNames.FONTTYPE_GUILD);
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildKickMember: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildUpdateNews" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildUpdateNews(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                var argdatos = buffer.ReadASCIIString();
                modGuilds.ActualizarNoticias(UserIndex, ref argdatos);

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildUpdateNews: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildMemberInfo" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildMemberInfo(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                modGuilds.SendDetallesPersonaje(UserIndex, buffer.ReadASCIIString());

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildMemberInfo: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildOpenElections" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildOpenElections(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        var error = default(string);
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            if (!modGuilds.v_AbrirElecciones(UserIndex, ref error))
                WriteConsoleMsg(UserIndex, error, FontTypeNames.FONTTYPE_GUILD);
            else
                modSendData.SendData(modSendData.SendTarget.ToGuildMembers, withBlock.GuildIndex,
                    PrepareMessageConsoleMsg(
                        "¡Han comenzado las elecciones del clan! Puedes votar escribiendo /VOTO seguido del nombre del personaje, por ejemplo: /VOTO " +
                        withBlock.name, FontTypeNames.FONTTYPE_GUILD));
        }
    }

    // '
    // Handles the "GuildRequestMembership" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildRequestMembership(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string guild;
            string application;
            var errorStr = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                guild = buffer.ReadASCIIString();
                application = buffer.ReadASCIIString();

                if (!modGuilds.a_NuevoAspirante(UserIndex, ref guild, ref application, ref errorStr))
                    WriteConsoleMsg(UserIndex, errorStr, FontTypeNames.FONTTYPE_GUILD);
                else
                    WriteConsoleMsg(UserIndex,
                        "Tu solicitud ha sido enviada. Espera prontas noticias del líder de " + guild + ".",
                        FontTypeNames.FONTTYPE_GUILD);

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildRequestMembership: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildRequestDetails" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildRequestDetails(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                var argGuildName = buffer.ReadASCIIString();
                modGuilds.SendGuildDetails(UserIndex, ref argGuildName);

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildRequestDetails: " + ex.Message);
        }
    }

    // '
    // Handles the "Online" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleOnline(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        int i;
        var Count = default(int);

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            var loopTo = (int)Declaraciones.LastUser;
            for (i = 1; i <= loopTo; i++)
                if (Migration.migr_LenB(Declaraciones.UserList[i].name) != 0)
                    if ((Declaraciones.UserList[i].flags.Privilegios &
                         (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero)) != 0)
                        Count = Count + 1;

            WriteConsoleMsg(UserIndex, "Número de usuarios: " + Count, FontTypeNames.FONTTYPE_INFO);
        }
    }

    // '
    // Handles the "Quit" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleQuit(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 04/15/2008 (NicoNZ)
        // If user is invisible, it automatically becomes
        // visible before doing the countdown to exit
        // 04/15/2008 - No se reseteaban lso contadores de invi ni de ocultar. (NicoNZ)
        // ***************************************************
        short tUser;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if (withBlock.flags.Paralizado == 1)
            {
                WriteConsoleMsg(UserIndex, "No puedes salir estando paralizado.", FontTypeNames.FONTTYPE_WARNING);
                return;
            }

            // exit secure commerce
            if (withBlock.ComUsu.DestUsu > 0)
            {
                tUser = withBlock.ComUsu.DestUsu;

                if (Declaraciones.UserList[tUser].flags.UserLogged)
                    if (Declaraciones.UserList[tUser].ComUsu.DestUsu == UserIndex)
                    {
                        WriteConsoleMsg(tUser, "Comercio cancelado por el otro usuario.", FontTypeNames.FONTTYPE_TALK);
                        mdlCOmercioConUsuario.FinComerciarUsu(tUser);
                    }

                WriteConsoleMsg(UserIndex, "Comercio cancelado.", FontTypeNames.FONTTYPE_TALK);
                mdlCOmercioConUsuario.FinComerciarUsu(UserIndex);
            }

            UsUaRiOs.Cerrar_Usuario(UserIndex);
        }
    }

    // '
    // Handles the "GuildLeave" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildLeave(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        short GuildIndex;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // obtengo el guildindex
            GuildIndex = modGuilds.m_EcharMiembroDeClan(UserIndex, withBlock.name);

            if (GuildIndex > 0)
            {
                WriteConsoleMsg(UserIndex, "Dejas el clan.", FontTypeNames.FONTTYPE_GUILD);
                modSendData.SendData(modSendData.SendTarget.ToGuildMembers, GuildIndex,
                    PrepareMessageConsoleMsg(withBlock.name + " deja el clan.", FontTypeNames.FONTTYPE_GUILD));
            }
            else
            {
                WriteConsoleMsg(UserIndex, "Tú no puedes salir de este clan.", FontTypeNames.FONTTYPE_GUILD);
            }
        }
    }

    // '
    // Handles the "RequestAccountState" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestAccountState(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        short earnings;
        var Percentage = default(short);

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Dead people can't check their accounts
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Validate target NPC
            if (withBlock.flags.TargetNPC == 0)
            {
                WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if (Matematicas.Distancia(ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos, ref withBlock.Pos) > 3)
            {
                WriteConsoleMsg(UserIndex, "Estás demasiado lejos del vendedor.", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            switch (Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype)
            {
                case Declaraciones.eNPCType.Banquero:
                {
                    WriteChatOverHead(UserIndex, "Tienes " + withBlock.Stats.Banco + " monedas de oro en tu cuenta.",
                        Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                        ColorTranslator.ToOle(Color.White));
                    break;
                }

                case Declaraciones.eNPCType.Timbero:
                {
                    if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) == 0)
                    {
                        earnings = Convert.ToInt16(Admin.Apuestas.Ganancias - Admin.Apuestas.Perdidas);

                        if ((earnings >= 0) & (Admin.Apuestas.Ganancias != 0))
                            Percentage =
                                Convert.ToInt16(Conversion.Int(earnings * 100 / (double)Admin.Apuestas.Ganancias));

                        if ((earnings < 0) & (Admin.Apuestas.Perdidas != 0))
                            Percentage =
                                Convert.ToInt16(Conversion.Int(earnings * 100 / (double)Admin.Apuestas.Perdidas));

                        WriteConsoleMsg(UserIndex,
                            "Entradas: " + Admin.Apuestas.Ganancias + " Salida: " + Admin.Apuestas.Perdidas +
                            " Ganancia Neta: " + earnings + " (" + Percentage + "%) Jugadas: " + Admin.Apuestas.Jugadas,
                            FontTypeNames.FONTTYPE_INFO);
                    }

                    break;
                }
            }
        }
    }

    // '
    // Handles the "PetStand" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandlePetStand(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Dead people can't use pets
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Validate target NPC
            if (withBlock.flags.TargetNPC == 0)
            {
                WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Make sure it's close enough
            if (Matematicas.Distancia(ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos, ref withBlock.Pos) > 10)
            {
                WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Make sure it's his pet
            if (Declaraciones.Npclist[withBlock.flags.TargetNPC].MaestroUser != UserIndex)
                return;

            // Do it!
            Declaraciones.Npclist[withBlock.flags.TargetNPC].Movement = AI.TipoAI.ESTATICO;

            Extra.Expresar(withBlock.flags.TargetNPC, UserIndex);
        }
    }

    // '
    // Handles the "PetFollow" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandlePetFollow(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Dead users can't use pets
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Validate target NPC
            if (withBlock.flags.TargetNPC == 0)
            {
                WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Make sure it's close enough
            if (Matematicas.Distancia(ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos, ref withBlock.Pos) > 10)
            {
                WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Make usre it's the user's pet
            if (Declaraciones.Npclist[withBlock.flags.TargetNPC].MaestroUser != UserIndex)
                return;

            // Do it
            NPCs.FollowAmo(withBlock.flags.TargetNPC);

            Extra.Expresar(withBlock.flags.TargetNPC, UserIndex);
        }
    }


    // '
    // Handles the "ReleasePet" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleReleasePet(short UserIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 18/11/2009
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Dead users can't use pets
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Validate target NPC
            if (withBlock.flags.TargetNPC == 0)
            {
                WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Make sure it's close enough
            if (Matematicas.Distancia(ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos, ref withBlock.Pos) > 10)
            {
                WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Make usre it's the user's pet
            if (Declaraciones.Npclist[withBlock.flags.TargetNPC].MaestroUser != UserIndex)
                return;

            // Do it
            NPCs.QuitarPet(UserIndex, withBlock.flags.TargetNPC);
        }
    }

    // '
    // Handles the "TrainList" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleTrainList(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Dead users can't use pets
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Validate target NPC
            if (withBlock.flags.TargetNPC == 0)
            {
                WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Make sure it's close enough
            if (Matematicas.Distancia(ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos, ref withBlock.Pos) > 10)
            {
                WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Make sure it's the trainer
            if (Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype != Declaraciones.eNPCType.Entrenador)
                return;

            WriteTrainerCreatureList(UserIndex, withBlock.flags.TargetNPC);
        }
    }

    // '
    // Handles the "Rest" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRest(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Dead users can't use pets
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Solo puedes usar ítems cuando estás vivo.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            var argObjIndex = Declaraciones.FOGATA;
            if (TCP.HayOBJarea(ref withBlock.Pos, ref argObjIndex))
            {
                WriteRestOK(UserIndex);

                if (!withBlock.flags.Descansar)
                    WriteConsoleMsg(UserIndex, "Te acomodás junto a la fogata y comienzas a descansar.",
                        FontTypeNames.FONTTYPE_INFO);
                else
                    WriteConsoleMsg(UserIndex, "Te levantas.", FontTypeNames.FONTTYPE_INFO);

                withBlock.flags.Descansar = !withBlock.flags.Descansar;
            }
            else
            {
                if (withBlock.flags.Descansar)
                {
                    WriteRestOK(UserIndex);
                    WriteConsoleMsg(UserIndex, "Te levantas.", FontTypeNames.FONTTYPE_INFO);

                    withBlock.flags.Descansar = false;
                    return;
                }

                WriteConsoleMsg(UserIndex, "No hay ninguna fogata junto a la cual descansar.",
                    FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Handles the "Meditate" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleMeditate(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 04/15/08 (NicoNZ)
        // Arreglé un bug que mandaba un index de la meditacion diferente
        // al que decia el server.
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Dead users can't use pets
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!! Sólo puedes meditar cuando estás vivo.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Can he meditate?
            if (withBlock.Stats.MaxMAN == 0)
            {
                WriteConsoleMsg(UserIndex, "Sólo las clases mágicas conocen el arte de la meditación.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Admins don't have to wait :D
            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) == 0)
            {
                withBlock.Stats.MinMAN = withBlock.Stats.MaxMAN;
                WriteConsoleMsg(UserIndex, "Maná restaurado.", FontTypeNames.FONTTYPE_VENENO);
                WriteUpdateMana(UserIndex);
                return;
            }

            WriteMeditateToggle(UserIndex);

            if (withBlock.flags.Meditando)
                WriteConsoleMsg(UserIndex, "Dejas de meditar.", FontTypeNames.FONTTYPE_INFO);

            withBlock.flags.Meditando = !withBlock.flags.Meditando;

            // Barrin 3/10/03 Tiempo de inicio al meditar
            if (withBlock.flags.Meditando)
            {
                withBlock.Counters.tInicioMeditar = Convert.ToInt32(Migration.GetTickCount());

                WriteConsoleMsg(UserIndex,
                    "Te estás concentrando. En " + Conversion.Fix(Declaraciones.TIEMPO_INICIOMEDITAR / 1000d) +
                    " segundos comenzarás a meditar.", FontTypeNames.FONTTYPE_INFO);

                withBlock.character.loops = Declaraciones.INFINITE_LOOPS;

                // Show proper FX according to level
                if (withBlock.Stats.ELV < 13)
                    withBlock.character.FX = (short)Declaraciones.FXIDs.FXMEDITARCHICO;

                else if (withBlock.Stats.ELV < 25)
                    withBlock.character.FX = (short)Declaraciones.FXIDs.FXMEDITARMEDIANO;

                else if (withBlock.Stats.ELV < 35)
                    withBlock.character.FX = (short)Declaraciones.FXIDs.FXMEDITARGRANDE;

                else if (withBlock.Stats.ELV < 42)
                    withBlock.character.FX = (short)Declaraciones.FXIDs.FXMEDITARXGRANDE;

                else
                    withBlock.character.FX = (short)Declaraciones.FXIDs.FXMEDITARXXGRANDE;

                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    PrepareMessageCreateFX(withBlock.character.CharIndex, withBlock.character.FX,
                        Declaraciones.INFINITE_LOOPS));
            }
            else
            {
                withBlock.Counters.bPuedeMeditar = false;

                withBlock.character.FX = 0;
                withBlock.character.loops = 0;
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    PrepareMessageCreateFX(withBlock.character.CharIndex, 0, 0));
            }
        }
    }

    // '
    // Handles the "Resucitate" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleResucitate(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Se asegura que el target es un npc
            if (withBlock.flags.TargetNPC == 0)
            {
                WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Validate NPC and make sure player is dead
            if (((Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype != Declaraciones.eNPCType.Revividor) &
                 ((Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype !=
                   Declaraciones.eNPCType.ResucitadorNewbie) | !Extra.EsNewbie(UserIndex))) |
                (withBlock.flags.Muerto == 0))
                return;

            // Make sure it's close enough
            if (Matematicas.Distancia(ref withBlock.Pos, ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos) > 10)
            {
                WriteConsoleMsg(UserIndex, "El sacerdote no puede resucitarte debido a que estás demasiado lejos.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            UsUaRiOs.RevivirUsuario(UserIndex);
            WriteConsoleMsg(UserIndex, "¡¡Has sido resucitado!!", FontTypeNames.FONTTYPE_INFO);
        }
    }

    // '
    // Handles the "Consultation" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleConsultation(string UserIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 01/05/2010
        // Habilita/Deshabilita el modo consulta.
        // 01/05/2010: ZaMa - Agrego validaciones.
        // ***************************************************

        short UserConsulta;

        string UserName;
        {
            ref var withBlock = ref Declaraciones.UserList[Convert.ToInt32(UserIndex)];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Comando exclusivo para gms
            if (!Extra.EsGM(Convert.ToInt16(UserIndex)))
                return;

            UserConsulta = withBlock.flags.TargetUser;

            // Se asegura que el target es un usuario
            if (UserConsulta == 0)
            {
                WriteConsoleMsg(Convert.ToInt16(UserIndex),
                    "Primero tienes que seleccionar un usuario, haz click izquierdo sobre él.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // No podes ponerte a vos mismo en modo consulta.
            if (UserConsulta == Convert.ToDouble(UserIndex))
                return;

            // No podes estra en consulta con otro gm
            if (Extra.EsGM(UserConsulta))
            {
                WriteConsoleMsg(Convert.ToInt16(UserIndex),
                    "No puedes iniciar el modo consulta con otro administrador.", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            UserName = Declaraciones.UserList[UserConsulta].name;

            // Si ya estaba en consulta, termina la consulta
            if (Declaraciones.UserList[UserConsulta].flags.EnConsulta)
            {
                WriteConsoleMsg(Convert.ToInt16(UserIndex), "Has terminado el modo consulta con " + UserName + ".",
                    FontTypeNames.FONTTYPE_INFOBOLD);
                WriteConsoleMsg(UserConsulta, "Has terminado el modo consulta.", FontTypeNames.FONTTYPE_INFOBOLD);
                var argtexto = "Termino consulta con " + UserName;
                General.LogGM(ref withBlock.name, ref argtexto);

                Declaraciones.UserList[UserConsulta].flags.EnConsulta = false;
            }

            // Sino la inicia
            else
            {
                WriteConsoleMsg(Convert.ToInt16(UserIndex), "Has iniciado el modo consulta con " + UserName + ".",
                    FontTypeNames.FONTTYPE_INFOBOLD);
                WriteConsoleMsg(UserConsulta, "Has iniciado el modo consulta.", FontTypeNames.FONTTYPE_INFOBOLD);
                var argtexto1 = "Inicio consulta con " + UserName;
                General.LogGM(ref withBlock.name, ref argtexto1);

                {
                    ref var withBlock1 = ref Declaraciones.UserList[UserConsulta];
                    withBlock1.flags.EnConsulta = true;

                    // Pierde invi u ocu
                    if ((withBlock1.flags.invisible == 1) | (withBlock1.flags.Oculto == 1))
                    {
                        withBlock1.flags.Oculto = 0;
                        withBlock1.flags.invisible = 0;
                        withBlock1.Counters.TiempoOculto = 0;
                        withBlock1.Counters.Invisibilidad = 0;

                        UsUaRiOs.SetInvisible(UserConsulta, Declaraciones.UserList[UserConsulta].character.CharIndex,
                            false);
                    }
                }
            }

            UsUaRiOs.SetConsulatMode(UserConsulta);
        }
    }

    // '
    // Handles the "Heal" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleHeal(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Se asegura que el target es un npc
            if (withBlock.flags.TargetNPC == 0)
            {
                WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if (((Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype != Declaraciones.eNPCType.Revividor) &
                 (Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype !=
                  Declaraciones.eNPCType.ResucitadorNewbie)) | (withBlock.flags.Muerto != 0))

                return;

            if (Matematicas.Distancia(ref withBlock.Pos, ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos) > 10)
            {
                WriteConsoleMsg(UserIndex, "El sacerdote no puede curarte debido a que estás demasiado lejos.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            withBlock.Stats.MinHp = withBlock.Stats.MaxHp;

            WriteUpdateHP(UserIndex);

            WriteConsoleMsg(UserIndex, "¡¡Has sido curado!!", FontTypeNames.FONTTYPE_INFO);
        }
    }

    // '
    // Handles the "RequestStats" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestStats(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        UsUaRiOs.SendUserStatsTxt(UserIndex, UserIndex);
    }

    // '
    // Handles the "Help" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleHelp(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        Extra.SendHelp(UserIndex);
    }

    // '
    // Handles the "CommerceStart" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCommerceStart(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        short i;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Dead people can't commerce
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Is it already in commerce mode??
            if (withBlock.flags.Comerciando)
            {
                WriteConsoleMsg(UserIndex, "Ya estás comerciando.", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Validate target NPC
            if (withBlock.flags.TargetNPC > 0)
            {
                // Does the NPC want to trade??
                if (Declaraciones.Npclist[withBlock.flags.TargetNPC].Comercia == 0)
                {
                    if (Migration.migr_LenB(Declaraciones.Npclist[withBlock.flags.TargetNPC].desc) != 0)
                        WriteChatOverHead(UserIndex, "No tengo ningún interés en comerciar.",
                            Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                            ColorTranslator.ToOle(Color.White));

                    return;
                }

                if (Matematicas.Distancia(ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos, ref withBlock.Pos) >
                    3)
                {
                    WriteConsoleMsg(UserIndex, "Estás demasiado lejos del vendedor.", FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                // Start commerce....
                modSistemaComercio.IniciarComercioNPC(UserIndex);
            }
            // [Alejo]
            else if (withBlock.flags.TargetUser > 0)
            {
                // User commerce...
                // Can he commerce??
                if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.Consejero) != 0)
                {
                    WriteConsoleMsg(UserIndex, "No puedes vender ítems.", FontTypeNames.FONTTYPE_WARNING);
                    return;
                }

                // Is the other one dead??
                if (Declaraciones.UserList[withBlock.flags.TargetUser].flags.Muerto == 1)
                {
                    WriteConsoleMsg(UserIndex, "¡¡No puedes comerciar con los muertos!!", FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                // Is it me??
                if (withBlock.flags.TargetUser == UserIndex)
                {
                    WriteConsoleMsg(UserIndex, "¡¡No puedes comerciar con vos mismo!!", FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                // Check distance
                if (Matematicas.Distancia(ref Declaraciones.UserList[withBlock.flags.TargetUser].Pos,
                        ref withBlock.Pos) > 3)
                {
                    WriteConsoleMsg(UserIndex, "Estás demasiado lejos del usuario.", FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                // Is he already trading?? is it with me or someone else??
                if (Declaraciones.UserList[withBlock.flags.TargetUser].flags.Comerciando &
                    (Declaraciones.UserList[withBlock.flags.TargetUser].ComUsu.DestUsu != UserIndex))
                {
                    WriteConsoleMsg(UserIndex, "No puedes comerciar con el usuario en este momento.",
                        FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                // Initialize some variables...
                withBlock.ComUsu.DestUsu = withBlock.flags.TargetUser;
                withBlock.ComUsu.DestNick = Declaraciones.UserList[withBlock.flags.TargetUser].name;
                for (i = 1; i <= mdlCOmercioConUsuario.MAX_OFFER_SLOTS; i++)
                {
                    withBlock.ComUsu.cant[i] = 0;
                    withBlock.ComUsu.Objeto[i] = 0;
                }

                withBlock.ComUsu.GoldAmount = 0;

                withBlock.ComUsu.Acepto = false;
                withBlock.ComUsu.Confirmo = false;

                // Rutina para comerciar con otro usuario
                mdlCOmercioConUsuario.IniciarComercioConUsuario(UserIndex, withBlock.flags.TargetUser);
            }
            else
            {
                WriteConsoleMsg(UserIndex, "Primero haz click izquierdo sobre el personaje.",
                    FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Handles the "BankStart" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleBankStart(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Dead people can't commerce
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if (withBlock.flags.Comerciando)
            {
                WriteConsoleMsg(UserIndex, "Ya estás comerciando.", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Validate target NPC
            if (withBlock.flags.TargetNPC > 0)
            {
                if (Matematicas.Distancia(ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos, ref withBlock.Pos) >
                    3)
                {
                    WriteConsoleMsg(UserIndex, "Estás demasiado lejos del vendedor.", FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                // If it's the banker....
                if (Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype == Declaraciones.eNPCType.Banquero)
                    modBanco.IniciarDeposito(UserIndex);
            }
            else
            {
                WriteConsoleMsg(UserIndex, "Primero haz click izquierdo sobre el personaje.",
                    FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Handles the "Enlist" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleEnlist(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Validate target NPC
            if (withBlock.flags.TargetNPC == 0)
            {
                WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if ((Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype != Declaraciones.eNPCType.Noble) |
                (withBlock.flags.Muerto != 0))
                return;

            if (Matematicas.Distancia(ref withBlock.Pos, ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos) > 4)
            {
                WriteConsoleMsg(UserIndex, "Debes acercarte más.", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if (Declaraciones.Npclist[withBlock.flags.TargetNPC].flags.Faccion == 0)
                ModFacciones.EnlistarArmadaReal(UserIndex);
            else
                ModFacciones.EnlistarCaos(UserIndex);
        }
    }

    // '
    // Handles the "Information" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleInformation(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        short Matados;
        short NextRecom;
        short Diferencia;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Validate target NPC
            if (withBlock.flags.TargetNPC == 0)
            {
                WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if ((Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype != Declaraciones.eNPCType.Noble) |
                (withBlock.flags.Muerto != 0))
                return;

            if (Matematicas.Distancia(ref withBlock.Pos, ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos) > 4)
            {
                WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO);
                return;
            }


            NextRecom = withBlock.Faccion.NextRecompensa;

            if (Declaraciones.Npclist[withBlock.flags.TargetNPC].flags.Faccion == 0)
            {
                if (withBlock.Faccion.ArmadaReal == 0)
                {
                    WriteChatOverHead(UserIndex, "¡¡No perteneces a las tropas reales!!",
                        Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                        ColorTranslator.ToOle(Color.White));
                    return;
                }

                Matados = Convert.ToInt16(withBlock.Faccion.CriminalesMatados);
                Diferencia = (short)(NextRecom - Matados);

                if (Diferencia > 0)
                    WriteChatOverHead(UserIndex,
                        "Tu deber es combatir criminales, mata " + Diferencia +
                        " criminales más y te daré una recompensa.",
                        Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                        ColorTranslator.ToOle(Color.White));
                else
                    WriteChatOverHead(UserIndex,
                        "Tu deber es combatir criminales, y ya has matado los suficientes como para merecerte una recompensa.",
                        Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                        ColorTranslator.ToOle(Color.White));
            }
            else
            {
                if (withBlock.Faccion.FuerzasCaos == 0)
                {
                    WriteChatOverHead(UserIndex, "¡¡No perteneces a la legión oscura!!",
                        Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                        ColorTranslator.ToOle(Color.White));
                    return;
                }

                Matados = Convert.ToInt16(withBlock.Faccion.CiudadanosMatados);
                Diferencia = (short)(NextRecom - Matados);

                if (Diferencia > 0)
                    WriteChatOverHead(UserIndex,
                        "Tu deber es sembrar el caos y la desesperanza, mata " + Diferencia +
                        " ciudadanos más y te daré una recompensa.",
                        Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                        ColorTranslator.ToOle(Color.White));
                else
                    WriteChatOverHead(UserIndex,
                        "Tu deber es sembrar el caos y la desesperanza, y creo que estás en condiciones de merecer una recompensa.",
                        Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                        ColorTranslator.ToOle(Color.White));
            }
        }
    }

    // '
    // Handles the "Reward" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleReward(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Validate target NPC
            if (withBlock.flags.TargetNPC == 0)
            {
                WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if ((Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype != Declaraciones.eNPCType.Noble) |
                (withBlock.flags.Muerto != 0))
                return;

            if (Matematicas.Distancia(ref withBlock.Pos, ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos) > 4)
            {
                WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if (Declaraciones.Npclist[withBlock.flags.TargetNPC].flags.Faccion == 0)
            {
                if (withBlock.Faccion.ArmadaReal == 0)
                {
                    WriteChatOverHead(UserIndex, "¡¡No perteneces a las tropas reales!!",
                        Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                        ColorTranslator.ToOle(Color.White));
                    return;
                }

                ModFacciones.RecompensaArmadaReal(UserIndex);
            }
            else
            {
                if (withBlock.Faccion.FuerzasCaos == 0)
                {
                    WriteChatOverHead(UserIndex, "¡¡No perteneces a la legión oscura!!",
                        Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                        ColorTranslator.ToOle(Color.White));
                    return;
                }

                ModFacciones.RecompensaCaos(UserIndex);
            }
        }
    }

    // '
    // Handles the "RequestMOTD" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestMOTD(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        TCP.SendMOTD(UserIndex);
    }

    // '
    // Handles the "UpTime" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleUpTime(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 01/10/08
        // 01/10/2008 - Marcos Martinez (ByVal) - Automatic restart removed from the server along with all their assignments and varibles
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        int time;
        string UpTimeStr;

        // Get total time in seconds
        time = Convert.ToInt32((Migration.GetTickCount() - Admin.tInicioServer) / 1000L);

        // Get times in dd:hh:mm:ss format
        UpTimeStr = time % 60 + " segundos.";
        time = time / 60;

        UpTimeStr = time % 60 + " minutos, " + UpTimeStr;
        time = time / 60;

        UpTimeStr = time % 24 + " horas, " + UpTimeStr;
        time = time / 24;

        if (time == 1)
            UpTimeStr = time + " día, " + UpTimeStr;
        else
            UpTimeStr = time + " días, " + UpTimeStr;

        WriteConsoleMsg(UserIndex, "Server Online: " + UpTimeStr, FontTypeNames.FONTTYPE_INFO);
    }

    // '
    // Handles the "PartyLeave" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandlePartyLeave(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        mdParty.SalirDeParty(UserIndex);
    }

    // '
    // Handles the "PartyCreate" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandlePartyCreate(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        if (!mdParty.PuedeCrearParty(UserIndex))
            return;

        mdParty.CrearParty(UserIndex);
    }

    // '
    // Handles the "PartyJoin" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandlePartyJoin(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        mdParty.SolicitarIngresoAParty(UserIndex);
    }

    // '
    // Handles the "ShareNpc" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleShareNpc(short UserIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 15/04/2010
        // Shares owned npcs with other user
        // ***************************************************

        short TargetUserIndex;
        short SharingUserIndex;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Didn't target any user
            TargetUserIndex = withBlock.flags.TargetUser;
            if (TargetUserIndex == 0)
                return;

            // Can't share with admins
            if (Extra.EsGM(TargetUserIndex))
            {
                WriteConsoleMsg(UserIndex, "No puedes compartir npcs con administradores!!",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Pk or Caos?
            if (ES.criminal(UserIndex))
            {
                // Caos can only share with other caos
                if (Extra.esCaos(UserIndex))
                {
                    if (!Extra.esCaos(TargetUserIndex))
                    {
                        WriteConsoleMsg(UserIndex, "Solo puedes compartir npcs con miembros de tu misma facción!!",
                            FontTypeNames.FONTTYPE_INFO);
                        return;
                    }
                }

                // Pks don't need to share with anyone
                else
                {
                    return;
                }
            }

            // Ciuda or Army?
            // Can't share
            else if (ES.criminal(TargetUserIndex))
            {
                WriteConsoleMsg(UserIndex, "No puedes compartir npcs con criminales!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Already sharing with target
            SharingUserIndex = withBlock.flags.ShareNpcWith;
            if (SharingUserIndex == TargetUserIndex)
                return;

            // Aviso al usuario anterior que dejo de compartir
            if (SharingUserIndex != 0)
            {
                WriteConsoleMsg(SharingUserIndex, withBlock.name + " ha dejado de compartir sus npcs contigo.",
                    FontTypeNames.FONTTYPE_INFO);
                WriteConsoleMsg(UserIndex,
                    "Has dejado de compartir tus npcs con " + Declaraciones.UserList[SharingUserIndex].name + ".",
                    FontTypeNames.FONTTYPE_INFO);
            }

            withBlock.flags.ShareNpcWith = TargetUserIndex;

            WriteConsoleMsg(TargetUserIndex, withBlock.name + " ahora comparte sus npcs contigo.",
                FontTypeNames.FONTTYPE_INFO);
            WriteConsoleMsg(UserIndex,
                "Ahora compartes tus npcs con " + Declaraciones.UserList[TargetUserIndex].name + ".",
                FontTypeNames.FONTTYPE_INFO);
        }
    }

    // '
    // Handles the "StopSharingNpc" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleStopSharingNpc(short UserIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 15/04/2010
        // Stop Sharing owned npcs with other user
        // ***************************************************

        short SharingUserIndex;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            SharingUserIndex = withBlock.flags.ShareNpcWith;

            if (SharingUserIndex != 0)
            {
                // Aviso al que compartia y al que le compartia.
                WriteConsoleMsg(SharingUserIndex, withBlock.name + " ha dejado de compartir sus npcs contigo.",
                    FontTypeNames.FONTTYPE_INFO);
                WriteConsoleMsg(SharingUserIndex,
                    "Has dejado de compartir tus npcs con " + Declaraciones.UserList[SharingUserIndex].name + ".",
                    FontTypeNames.FONTTYPE_INFO);

                withBlock.flags.ShareNpcWith = 0;
            }
        }
    }

    // '
    // Handles the "Inquiry" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleInquiry(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        Declaraciones.ConsultaPopular.SendInfoEncuesta(UserIndex);
    }

    // '
    // Handles the "GuildMessage" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildMessage(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 15/07/2009
        // 02/03/2009: ZaMa - Arreglado un indice mal pasado a la funcion de cartel de clanes overhead.
        // 15/07/2009: ZaMa - Now invisible admins only speak by console
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string Chat;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                Chat = buffer.ReadASCIIString();

                if (Migration.migr_LenB(Chat) != 0)
                {
                    // Analize chat...
                    Statistics.ParseChat(ref Chat);

                    if (withBlock.GuildIndex > 0)
                    {
                        modSendData.SendData(modSendData.SendTarget.ToDiosesYclan, withBlock.GuildIndex,
                            PrepareMessageGuildChat(withBlock.name + "> " + Chat));

                        if (!(withBlock.flags.AdminInvisible == 1))
                            modSendData.SendData(modSendData.SendTarget.ToClanArea, UserIndex,
                                PrepareMessageChatOverHead("< " + Chat + " >", withBlock.character.CharIndex,
                                    ColorTranslator.ToOle(Color.Yellow)));
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildMessage: " + ex.Message);
        }
    }

    // '
    // Handles the "PartyMessage" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandlePartyMessage(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string Chat;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                Chat = buffer.ReadASCIIString();

                if (Migration.migr_LenB(Chat) != 0)
                {
                    // Analize chat...
                    Statistics.ParseChat(ref Chat);

                    mdParty.BroadCastParty(UserIndex, ref Chat);
                    // TODO : Con la 0.12.1 se debe definir si esto vuelve o se borra (/CMSG overhead)
                    // Call SendData(SendTarget.ToPartyArea, UserIndex, UserList(UserIndex).Pos.map, "||" & vbYellow & "°< " & mid$(rData, 7) & " >°" & CStr(UserList(UserIndex).Char.CharIndex))
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandlePartyMessage: " + ex.Message);
        }
    }

    // '
    // Handles the "CentinelReport" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCentinelReport(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            modCentinela.CentinelaCheckClave(UserIndex, withBlock.incomingData.ReadInteger());
        }
    }

    // '
    // Handles the "GuildOnline" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildOnline(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        string onlineList;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            onlineList = modGuilds.m_ListaDeMiembrosOnline(UserIndex, withBlock.GuildIndex);

            if (withBlock.GuildIndex != 0)
                WriteConsoleMsg(UserIndex, "Compañeros de tu clan conectados: " + onlineList,
                    FontTypeNames.FONTTYPE_GUILDMSG);
            else
                WriteConsoleMsg(UserIndex, "No pertences a ningún clan.", FontTypeNames.FONTTYPE_GUILDMSG);
        }
    }

    // '
    // Handles the "PartyOnline" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandlePartyOnline(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        // Remove packet ID
        Declaraciones.UserList[UserIndex].incomingData.ReadByte();

        mdParty.OnlineParty(UserIndex);
    }

    // '
    // Handles the "CouncilMessage" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCouncilMessage(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string Chat;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                Chat = buffer.ReadASCIIString();

                if (Migration.migr_LenB(Chat) != 0)
                {
                    // Analize chat...
                    Statistics.ParseChat(ref Chat);

                    if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.RoyalCouncil) != 0)
                        modSendData.SendData(modSendData.SendTarget.ToConsejo, UserIndex,
                            PrepareMessageConsoleMsg("(Consejero) " + withBlock.name + "> " + Chat,
                                FontTypeNames.FONTTYPE_CONSEJO));
                    else if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.ChaosCouncil) != 0)
                        modSendData.SendData(modSendData.SendTarget.ToConsejoCaos, UserIndex,
                            PrepareMessageConsoleMsg("(Consejero) " + withBlock.name + "> " + Chat,
                                FontTypeNames.FONTTYPE_CONSEJOCAOS));
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleCouncilMessage: " + ex.Message);
        }
    }

    // '
    // Handles the "RoleMasterRequest" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRoleMasterRequest(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string request;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                request = buffer.ReadASCIIString();

                if (Migration.migr_LenB(request) != 0)
                {
                    WriteConsoleMsg(UserIndex, "Su solicitud ha sido enviada.", FontTypeNames.FONTTYPE_INFO);
                    modSendData.SendData(modSendData.SendTarget.ToRolesMasters, 0,
                        PrepareMessageConsoleMsg(withBlock.name + " PREGUNTA ROL: " + request,
                            FontTypeNames.FONTTYPE_GUILDMSG));
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleRoleMasterRequest: " + ex.Message);
        }
    }

    // '
    // Handles the "GMRequest" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGMRequest(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if (!Declaraciones.Ayuda.Existe(withBlock.name))
            {
                WriteConsoleMsg(UserIndex,
                    "El mensaje ha sido entregado, ahora sólo debes esperar que se desocupe algún GM.",
                    FontTypeNames.FONTTYPE_INFO);
                Declaraciones.Ayuda.Push(withBlock.name);
            }
            else
            {
                Declaraciones.Ayuda.Quitar(withBlock.name);
                Declaraciones.Ayuda.Push(withBlock.name);
                WriteConsoleMsg(UserIndex,
                    "Ya habías mandado un mensaje, tu mensaje ha sido movido al final de la cola de mensajes.",
                    FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Handles the "BugReport" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleBugReport(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string bugReport;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...

                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                bugReport = buffer.ReadASCIIString();

                General.AppendLog("LOGS/BUGs.log",
                    "Usuario:" + withBlock.name + "  Fecha:" + Conversions.ToString(DateTime.Today) + "    Hora:" +
                    Conversions.ToString(DateAndTime.TimeOfDay));
                General.AppendLog("LOGS/BUGs.log", "BUG:");
                General.AppendLog("LOGS/BUGs.log", bugReport);
                General.AppendLog("LOGS/BUGs.log",
                    "########################################################################");

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleBugReport: " + ex.Message);
        }
    }

    // '
    // Handles the "ChangeDescription" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleChangeDescription(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string Description;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                Description = buffer.ReadASCIIString();

                if (withBlock.flags.Muerto == 1)
                {
                    WriteConsoleMsg(UserIndex, "No puedes cambiar la descripción estando muerto.",
                        FontTypeNames.FONTTYPE_INFO);
                }
                else if (!TCP.AsciiValidos(Description))
                {
                    WriteConsoleMsg(UserIndex, "La descripción tiene caracteres inválidos.",
                        FontTypeNames.FONTTYPE_INFO);
                }
                else
                {
                    withBlock.desc = Description.Trim();
                    WriteConsoleMsg(UserIndex, "La descripción ha cambiado.", FontTypeNames.FONTTYPE_INFO);
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleChangeDescription: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildVote" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildVote(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string vote;
            var errorStr = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                vote = buffer.ReadASCIIString();

                if (!modGuilds.v_UsuarioVota(UserIndex, ref vote, ref errorStr))
                    WriteConsoleMsg(UserIndex, "Voto NO contabilizado: " + errorStr, FontTypeNames.FONTTYPE_GUILD);
                else
                    WriteConsoleMsg(UserIndex, "Voto contabilizado.", FontTypeNames.FONTTYPE_GUILD);

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildVote: " + ex.Message);
        }
    }

    // '
    // Handles the "ShowGuildNews" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleShowGuildNews(short UserIndex)
    {
        // ***************************************************
        // Author: ZaMA
        // Last Modification: 05/17/06
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];

            // Remove packet ID
            withBlock.incomingData.ReadByte();

            modGuilds.SendGuildNews(UserIndex);
        }
    }

    // '
    // Handles the "Punishments" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandlePunishments(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 25/08/2009
        // 25/08/2009: ZaMa - Now only admins can see other admins' punishment list
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string name;
            short Count;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                name = buffer.ReadASCIIString();

                if (Migration.migr_LenB(name) != 0)
                {
                    if (Migration.migr_InStrB(name, @"\") != 0) name = name.Replace(@"\", "");
                    if (Migration.migr_InStrB(name, "/") != 0) name = name.Replace("/", "");
                    if (Migration.migr_InStrB(name, ":") != 0) name = name.Replace(":", "");
                    if (Migration.migr_InStrB(name, "|") != 0) name = name.Replace("|", "");

                    if ((ES.EsAdmin(name) | ES.EsDios(name) | ES.EsSemiDios(name) | ES.EsConsejero(name) |
                         ES.EsRolesMaster(name)) & ((Declaraciones.UserList[UserIndex].flags.Privilegios &
                                                     Declaraciones.PlayerType.User) != 0))
                    {
                        WriteConsoleMsg(UserIndex, "No puedes ver las penas de los administradores.",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                    else if (File.Exists(Declaraciones.CharPath + name + ".chr"))
                    {
                        var argEmptySpaces = 1024;
                        Count = Convert.ToInt16(Migration.ParseVal(ES.GetVar(Declaraciones.CharPath + name + ".chr",
                            "PENAS", "Cant", ref argEmptySpaces)));
                        if (Count == 0)
                            WriteConsoleMsg(UserIndex, "Sin prontuario..", FontTypeNames.FONTTYPE_INFO);
                        else
                            while (Count > 0)
                            {
                                var argEmptySpaces1 = 1024;
                                WriteConsoleMsg(UserIndex,
                                    Count + " - " + ES.GetVar(Declaraciones.CharPath + name + ".chr", "PENAS",
                                        "P" + Count, ref argEmptySpaces1), FontTypeNames.FONTTYPE_INFO);
                                Count = Convert.ToInt16(Count - 1);
                            }
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, "Personaje \"" + name + "\" inexistente.",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandlePunishments: " + ex.Message);
        }
    }

    // '
    // Handles the "ChangePassword" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleChangePassword(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Creation Date: 10/10/07
        // Last Modified By: Rapsodius
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string oldPass;
            string newPass;
            string oldPass2;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);


                // Remove packet ID
                buffer.ReadByte();

                oldPass = buffer.ReadASCIIString().ToUpper();
                newPass = buffer.ReadASCIIString().ToUpper();

                if (Migration.migr_LenB(newPass) == 0)
                {
                    WriteConsoleMsg(UserIndex, "Debes especificar una contraseña nueva, inténtalo de nuevo.",
                        FontTypeNames.FONTTYPE_INFO);
                }
                else
                {
                    var argEmptySpaces = 1024;
                    oldPass2 = ES.GetVar(Declaraciones.CharPath + Declaraciones.UserList[UserIndex].name + ".chr",
                        "INIT", "Password", ref argEmptySpaces).ToUpper();

                    if ((oldPass2 ?? "") != (oldPass ?? ""))
                    {
                        WriteConsoleMsg(UserIndex,
                            "La contraseña actual proporcionada no es correcta. La contraseña no ha sido cambiada, inténtalo de nuevo.",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        ES.WriteVar(Declaraciones.CharPath + Declaraciones.UserList[UserIndex].name + ".chr", "INIT",
                            "Password", newPass);
                        WriteConsoleMsg(UserIndex, "La contraseña fue cambiada con éxito.",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleChangePassword: " + ex.Message);
        }
    }


    // '
    // Handles the "Gamble" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGamble(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        short Amount;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            Amount = withBlock.incomingData.ReadInteger();

            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
            }
            else if (withBlock.flags.TargetNPC == 0)
            {
                // Validate target NPC
                WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                    FontTypeNames.FONTTYPE_INFO);
            }
            else if (Matematicas.Distancia(ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos,
                         ref withBlock.Pos) > 10)
            {
                WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO);
            }
            else if (Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype != Declaraciones.eNPCType.Timbero)
            {
                WriteChatOverHead(UserIndex, "No tengo ningún interés en apostar.",
                    Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                    ColorTranslator.ToOle(Color.White));
            }
            else if (Amount < 1)
            {
                WriteChatOverHead(UserIndex, "El mínimo de apuesta es 1 moneda.",
                    Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                    ColorTranslator.ToOle(Color.White));
            }
            else if (Amount > 5000)
            {
                WriteChatOverHead(UserIndex, "El máximo de apuesta es 5000 monedas.",
                    Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                    ColorTranslator.ToOle(Color.White));
            }
            else if (withBlock.Stats.GLD < Amount)
            {
                WriteChatOverHead(UserIndex, "No tienes esa cantidad.",
                    Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                    ColorTranslator.ToOle(Color.White));
            }
            else
            {
                if (Matematicas.RandomNumber(1, 100) <= 47)
                {
                    withBlock.Stats.GLD = withBlock.Stats.GLD + Amount;
                    WriteChatOverHead(UserIndex, "¡Felicidades! Has ganado " + Amount + " monedas de oro.",
                        Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                        ColorTranslator.ToOle(Color.White));

                    Admin.Apuestas.Perdidas = Admin.Apuestas.Perdidas + Amount;
                    ES.WriteVar(Declaraciones.DatPath + "apuestas.dat", "Main", "Perdidas",
                        Admin.Apuestas.Perdidas.ToString());
                }
                else
                {
                    withBlock.Stats.GLD = withBlock.Stats.GLD - Amount;
                    WriteChatOverHead(UserIndex, "Lo siento, has perdido " + Amount + " monedas de oro.",
                        Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                        ColorTranslator.ToOle(Color.White));

                    Admin.Apuestas.Ganancias = Admin.Apuestas.Ganancias + Amount;
                    ES.WriteVar(Declaraciones.DatPath + "apuestas.dat", "Main", "Ganancias",
                        Admin.Apuestas.Ganancias.ToString());
                }

                Admin.Apuestas.Jugadas = Admin.Apuestas.Jugadas + 1;

                ES.WriteVar(Declaraciones.DatPath + "apuestas.dat", "Main", "Jugadas",
                    Admin.Apuestas.Jugadas.ToString());

                WriteUpdateGold(UserIndex);
            }
        }
    }

    // '
    // Handles the "InquiryVote" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleInquiryVote(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte opt;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            opt = withBlock.incomingData.ReadByte();

            WriteConsoleMsg(UserIndex, Declaraciones.ConsultaPopular.doVotar(UserIndex, opt),
                FontTypeNames.FONTTYPE_GUILD);
        }
    }

    // '
    // Handles the "BankExtractGold" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleBankExtractGold(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        int Amount;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            Amount = withBlock.incomingData.ReadLong();

            // Dead people can't leave a faction.. they can't talk...
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Validate target NPC
            if (withBlock.flags.TargetNPC == 0)
            {
                WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if (Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype != Declaraciones.eNPCType.Banquero)
                return;

            if (Matematicas.Distancia(ref withBlock.Pos, ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos) > 10)
            {
                WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if ((Amount > 0) & (Amount <= withBlock.Stats.Banco))
            {
                withBlock.Stats.Banco = withBlock.Stats.Banco - Amount;
                withBlock.Stats.GLD = withBlock.Stats.GLD + Amount;
                WriteChatOverHead(UserIndex, "Tenés " + withBlock.Stats.Banco + " monedas de oro en tu cuenta.",
                    Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                    ColorTranslator.ToOle(Color.White));
            }
            else
            {
                WriteChatOverHead(UserIndex, "No tienes esa cantidad.",
                    Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                    ColorTranslator.ToOle(Color.White));
            }

            WriteUpdateGold(UserIndex);
            WriteUpdateBankGold(UserIndex);
        }
    }

    // '
    // Handles the "LeaveFaction" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleLeaveFaction(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************

        var TalkToKing = default(bool);
        var TalkToDemon = default(bool);
        short NpcIndex;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // Dead people can't leave a faction.. they can't talk...
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Chequea si habla con el rey o el demonio. Puede salir sin hacerlo, pero si lo hace le reponden los npcs
            NpcIndex = withBlock.flags.TargetNPC;
            if (NpcIndex != 0)
                // Es rey o domonio?
                if (Declaraciones.Npclist[NpcIndex].NPCtype == Declaraciones.eNPCType.Noble)
                {
                    // Rey?
                    if (Declaraciones.Npclist[NpcIndex].flags.Faccion == 0)
                        TalkToKing = true;
                    // Demonio
                    else
                        TalkToDemon = true;
                }

            // Quit the Royal Army?
            if (withBlock.Faccion.ArmadaReal == 1)
            {
                // Si le pidio al demonio salir de la armada, este le responde.
                if (TalkToDemon)
                {
                    WriteChatOverHead(UserIndex, "¡¡¡Sal de aquí bufón!!!",
                        Declaraciones.Npclist[NpcIndex].character.CharIndex, ColorTranslator.ToOle(Color.White));
                }

                else
                {
                    // Si le pidio al rey salir de la armada, le responde.
                    if (TalkToKing)
                        WriteChatOverHead(UserIndex, "Serás bienvenido a las fuerzas imperiales si deseas regresar.",
                            Declaraciones.Npclist[NpcIndex].character.CharIndex, ColorTranslator.ToOle(Color.White));

                    var argExpulsado = false;
                    ModFacciones.ExpulsarFaccionReal(UserIndex, ref argExpulsado);
                }
            }

            // Quit the Chaos Legion?
            else if (withBlock.Faccion.FuerzasCaos == 1)
            {
                // Si le pidio al rey salir del caos, le responde.
                if (TalkToKing)
                {
                    WriteChatOverHead(UserIndex, "¡¡¡Sal de aquí maldito criminal!!!",
                        Declaraciones.Npclist[NpcIndex].character.CharIndex, ColorTranslator.ToOle(Color.White));
                }
                else
                {
                    // Si le pidio al demonio salir del caos, este le responde.
                    if (TalkToDemon)
                        WriteChatOverHead(UserIndex, "Ya volverás arrastrandote.",
                            Declaraciones.Npclist[NpcIndex].character.CharIndex, ColorTranslator.ToOle(Color.White));

                    var argExpulsado1 = false;
                    ModFacciones.ExpulsarFaccionCaos(UserIndex, ref argExpulsado1);
                }
            }
            // No es faccionario

            // Si le hablaba al rey o demonio, le repsonden ellos
            else if (NpcIndex > 0)
            {
                WriteChatOverHead(UserIndex, "¡No perteneces a ninguna facción!",
                    Declaraciones.Npclist[NpcIndex].character.CharIndex, ColorTranslator.ToOle(Color.White));
            }
            else
            {
                WriteConsoleMsg(UserIndex, "¡No perteneces a ninguna facción!", FontTypeNames.FONTTYPE_FIGHT);
            }
        }
    }

    // '
    // Handles the "BankDepositGold" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleBankDepositGold(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        int Amount;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            Amount = withBlock.incomingData.ReadLong();

            // Dead people can't leave a faction.. they can't talk...
            if (withBlock.flags.Muerto == 1)
            {
                WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            // Validate target NPC
            if (withBlock.flags.TargetNPC == 0)
            {
                WriteConsoleMsg(UserIndex, "Primero tienes que seleccionar un personaje, haz click izquierdo sobre él.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if (Matematicas.Distancia(ref Declaraciones.Npclist[withBlock.flags.TargetNPC].Pos, ref withBlock.Pos) > 10)
            {
                WriteConsoleMsg(UserIndex, "Estás demasiado lejos.", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if (Declaraciones.Npclist[withBlock.flags.TargetNPC].NPCtype != Declaraciones.eNPCType.Banquero)
                return;

            if ((Amount > 0) & (Amount <= withBlock.Stats.GLD))
            {
                withBlock.Stats.Banco = withBlock.Stats.Banco + Amount;
                withBlock.Stats.GLD = withBlock.Stats.GLD - Amount;
                WriteChatOverHead(UserIndex, "Tenés " + withBlock.Stats.Banco + " monedas de oro en tu cuenta.",
                    Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                    ColorTranslator.ToOle(Color.White));

                WriteUpdateGold(UserIndex);
                WriteUpdateBankGold(UserIndex);
            }
            else
            {
                WriteChatOverHead(UserIndex, "No tenés esa cantidad.",
                    Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                    ColorTranslator.ToOle(Color.White));
            }
        }
    }

    // '
    // Handles the "Denounce" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleDenounce(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string Text;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                Text = buffer.ReadASCIIString();

                if (withBlock.flags.Silenciado == 0)
                {
                    // Analize chat...
                    Statistics.ParseChat(ref Text);

                    modSendData.SendData(modSendData.SendTarget.ToAdmins, 0,
                        PrepareMessageConsoleMsg(withBlock.name.ToLower() + " DENUNCIA: " + Text,
                            FontTypeNames.FONTTYPE_GUILDMSG));
                    WriteConsoleMsg(UserIndex, "Denuncia enviada, espere..", FontTypeNames.FONTTYPE_INFO);
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleDenounce: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildFundate" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildFundate(short UserIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 14/12/2009
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 1)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            withBlock.incomingData.ReadByte();

            if (modGuilds.HasFound(ref withBlock.name))
            {
                WriteConsoleMsg(UserIndex, "¡Ya has fundado un clan, no puedes fundar otro!",
                    FontTypeNames.FONTTYPE_INFOBOLD);
                return;
            }

            WriteShowGuildAlign(UserIndex);
        }
    }

    // '
    // Handles the "GuildFundation" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildFundation(short UserIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 14/12/2009
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        Declaraciones.eClanType clanType;
        var error = default(string);
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            clanType = (Declaraciones.eClanType)withBlock.incomingData.ReadByte();

            if (modGuilds.HasFound(ref withBlock.name))
            {
                WriteConsoleMsg(UserIndex, "¡Ya has fundado un clan, no puedes fundar otro!",
                    FontTypeNames.FONTTYPE_INFOBOLD);
                var argtexto = "El usuario " + withBlock.name +
                               " ha intentado fundar un clan ya habiendo fundado otro desde la IP " + withBlock.ip;
                General.LogCheating(ref argtexto);
                return;
            }

            switch (clanType.ToString().Trim().ToUpper() ?? "")
            {
                case var @case when @case == (Declaraciones.eClanType.ct_RoyalArmy.ToString() ?? ""):
                {
                    withBlock.FundandoGuildAlineacion = modGuilds.ALINEACION_GUILD.ALINEACION_ARMADA;
                    break;
                }
                case var case1 when case1 == (Declaraciones.eClanType.ct_Evil.ToString() ?? ""):
                {
                    withBlock.FundandoGuildAlineacion = modGuilds.ALINEACION_GUILD.ALINEACION_LEGION;
                    break;
                }
                case var case2 when case2 == (Declaraciones.eClanType.ct_Neutral.ToString() ?? ""):
                {
                    withBlock.FundandoGuildAlineacion = modGuilds.ALINEACION_GUILD.ALINEACION_NEUTRO;
                    break;
                }
                case var case3 when case3 == (Declaraciones.eClanType.ct_GM.ToString() ?? ""):
                {
                    withBlock.FundandoGuildAlineacion = modGuilds.ALINEACION_GUILD.ALINEACION_MASTER;
                    break;
                }
                case var case4 when case4 == (Declaraciones.eClanType.ct_Legal.ToString() ?? ""):
                {
                    withBlock.FundandoGuildAlineacion = modGuilds.ALINEACION_GUILD.ALINEACION_CIUDA;
                    break;
                }
                case var case5 when case5 == (Declaraciones.eClanType.ct_Criminal.ToString() ?? ""):
                {
                    withBlock.FundandoGuildAlineacion = modGuilds.ALINEACION_GUILD.ALINEACION_CRIMINAL;
                    break;
                }

                default:
                {
                    WriteConsoleMsg(UserIndex, "Alineación inválida.", FontTypeNames.FONTTYPE_GUILD);
                    return;
                }
            }

            if (modGuilds.PuedeFundarUnClan(UserIndex, withBlock.FundandoGuildAlineacion, ref error))
            {
                WriteShowGuildFundationForm(UserIndex);
            }
            else
            {
                withBlock.FundandoGuildAlineacion = 0;
                WriteConsoleMsg(UserIndex, error, FontTypeNames.FONTTYPE_GUILD);
            }
        }
    }

    // '
    // Handles the "PartyKick" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandlePartyKick(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/05/09
        // Last Modification by: Marco Vanotti (Marco)
        // - 05/05/09: Now it uses "UserPuedeEjecutarComandos" to check if the user can use party commands
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (mdParty.UserPuedeEjecutarComandos(UserIndex))
                {
                    tUser = Extra.NameIndex(UserName);

                    if (tUser > 0)
                    {
                        mdParty.ExpulsarDeParty(UserIndex, tUser);
                    }
                    else
                    {
                        if (UserName.IndexOf("+") + 1 != 0) UserName = UserName.Replace("+", " ");

                        WriteConsoleMsg(UserIndex, UserName.ToLower() + " no pertenece a tu party.",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandlePartyKick: " + ex.Message);
        }
    }

    // '
    // Handles the "PartySetLeader" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandlePartySetLeader(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/05/09
        // Last Modification by: Marco Vanotti (MarKoxX)
        // - 05/05/09: Now it uses "UserPuedeEjecutarComandos" to check if the user can use party commands
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        var buffer = new clsByteQueue();
        string UserName;
        short tUser;
        short rank;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
            buffer.CopyBuffer(ref withBlock.incomingData);

            // Remove packet ID
            buffer.ReadByte();

            rank = Convert.ToInt16((int)(Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                         Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.Consejero));

            UserName = buffer.ReadASCIIString();
            if (mdParty.UserPuedeEjecutarComandos(UserIndex))
            {
                tUser = Extra.NameIndex(UserName);
                if (tUser > 0)
                {
                    // Don't allow users to spoof online GMs
                    if (((int)Admin.UserDarPrivilegioLevel(UserName) & rank) <=
                        ((int)withBlock.flags.Privilegios & rank))
                        mdParty.TransformarEnLider(UserIndex, tUser);
                    else
                        WriteConsoleMsg(UserIndex,
                            Declaraciones.UserList[tUser].name.ToLower() + " no pertenece a tu party.",
                            FontTypeNames.FONTTYPE_INFO);
                }

                else
                {
                    if (UserName.IndexOf("+") + 1 != 0) UserName = UserName.Replace("+", " ");
                    WriteConsoleMsg(UserIndex, UserName.ToLower() + " no pertenece a tu party.",
                        FontTypeNames.FONTTYPE_INFO);
                }
            }

            // If we got here then packet is complete, copy data back to original queue
            withBlock.incomingData.CopyBuffer(ref buffer);
        }
    }

    // '
    // Handles the "PartyAcceptMember" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandlePartyAcceptMember(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/05/09
        // Last Modification by: Marco Vanotti (Marco)
        // - 05/05/09: Now it uses "UserPuedeEjecutarComandos" to check if the user can use party commands
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            short rank;
            var bUserVivo = default(bool);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                rank = Convert.ToInt16((int)(Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                             Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.Consejero));

                UserName = buffer.ReadASCIIString();
                if (Declaraciones.UserList[UserIndex].flags.Muerto != 0)
                    WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", FontTypeNames.FONTTYPE_PARTY);
                else
                    bUserVivo = true;

                if (mdParty.UserPuedeEjecutarComandos(UserIndex) & bUserVivo)
                {
                    tUser = Extra.NameIndex(UserName);
                    if (tUser > 0)
                    {
                        // Validate administrative ranks - don't allow users to spoof online GMs
                        if (((int)Declaraciones.UserList[tUser].flags.Privilegios & rank) <=
                            ((int)withBlock.flags.Privilegios & rank))
                            mdParty.AprobarIngresoAParty(UserIndex, tUser);
                        else
                            WriteConsoleMsg(UserIndex,
                                "No puedes incorporar a tu party a personajes de mayor jerarquía.",
                                FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        if (UserName.IndexOf("+") + 1 != 0) UserName = UserName.Replace("+", " ");

                        // Don't allow users to spoof online GMs
                        if (((int)Admin.UserDarPrivilegioLevel(UserName) & rank) <=
                            ((int)withBlock.flags.Privilegios & rank))
                            WriteConsoleMsg(UserIndex, UserName.ToLower() + " no ha solicitado ingresar a tu party.",
                                FontTypeNames.FONTTYPE_PARTY);
                        else
                            WriteConsoleMsg(UserIndex,
                                "No puedes incorporar a tu party a personajes de mayor jerarquía.",
                                FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandlePartyAcceptMember: " + ex.Message);
        }
    }

    // '
    // Handles the "GuildMemberList" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildMemberList(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string guild;
            short memberCount;
            int i;
            string UserName;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                guild = buffer.ReadASCIIString();

                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                    0)
                {
                    if (Migration.migr_InStrB(guild, @"\") != 0) guild = guild.Replace(@"\", "");
                    if (Migration.migr_InStrB(guild, "/") != 0) guild = guild.Replace("/", "");

                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "guilds/" + guild + "-members.mem"))
                    {
                        WriteConsoleMsg(UserIndex, "No existe el clan: " + guild, FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        var argEmptySpaces = 1024;
                        memberCount = Convert.ToInt16(Migration.ParseVal(ES.GetVar(
                            AppDomain.CurrentDomain.BaseDirectory + "Guilds/" + guild + "-Members" + ".mem", "INIT",
                            "NroMembers", ref argEmptySpaces)));

                        var loopTo = (int)memberCount;
                        for (i = 1; i <= loopTo; i++)
                        {
                            var argEmptySpaces1 = 1024;
                            UserName = ES.GetVar(
                                AppDomain.CurrentDomain.BaseDirectory + "Guilds/" + guild + "-Members" + ".mem",
                                "Members", "Member" + i, ref argEmptySpaces1);

                            WriteConsoleMsg(UserIndex, UserName + "<" + guild + ">", FontTypeNames.FONTTYPE_INFO);
                        }
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildMemberList: " + ex.Message);
        }
    }

    // '
    // Handles the "GMMessage" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGMMessage(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 01/08/07
        // Last Modification by: (liquid)
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string message;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                message = buffer.ReadASCIIString();

                if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) == 0)
                {
                    var argtexto = "Mensaje a Gms:" + message;
                    General.LogGM(ref withBlock.name, ref argtexto);

                    if (Migration.migr_LenB(message) != 0)
                    {
                        // Analize chat...
                        Statistics.ParseChat(ref message);

                        modSendData.SendData(modSendData.SendTarget.ToAdmins, 0,
                            PrepareMessageConsoleMsg(withBlock.name + "> " + message, FontTypeNames.FONTTYPE_GMMSG));
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGMMessage: " + ex.Message);
        }
    }

    // '
    // Handles the "ShowName" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleShowName(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)

            {
                withBlock.showName = !withBlock.showName; // Show / Hide the name

                UsUaRiOs.RefreshCharStatus(UserIndex);
            }
        }
    }

    // '
    // Handles the "OnlineRoyalArmy" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleOnlineRoyalArmy(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        int i;
        var list = default(string);
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                return;


            var loopTo = (int)Declaraciones.LastUser;
            for (i = 1; i <= loopTo; i++)
                if (Declaraciones.UserList[i].ConnID != -1)
                    if (Declaraciones.UserList[i].Faccion.ArmadaReal == 1)
                        if (((Declaraciones.UserList[i].flags.Privilegios & (Declaraciones.PlayerType.User |
                                                                             Declaraciones.PlayerType.Consejero |
                                                                             Declaraciones.PlayerType.SemiDios)) != 0) |
                            ((withBlock.flags.Privilegios &
                              (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin)) != 0))

                            list = list + Declaraciones.UserList[i].name + ", ";
        }

        if (list.Length > 0)
            WriteConsoleMsg(UserIndex,
                "Reales conectados: " + list.Substring(0, Math.Min(list.Length - 2, list.Length)),
                FontTypeNames.FONTTYPE_INFO);
        else
            WriteConsoleMsg(UserIndex, "No hay reales conectados.", FontTypeNames.FONTTYPE_INFO);
    }

    // '
    // Handles the "OnlineChaosLegion" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleOnlineChaosLegion(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        int i;
        var list = default(string);
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                return;


            var loopTo = (int)Declaraciones.LastUser;
            for (i = 1; i <= loopTo; i++)
                if (Declaraciones.UserList[i].ConnID != -1)
                    if (Declaraciones.UserList[i].Faccion.FuerzasCaos == 1)
                        if (((Declaraciones.UserList[i].flags.Privilegios & (Declaraciones.PlayerType.User |
                                                                             Declaraciones.PlayerType.Consejero |
                                                                             Declaraciones.PlayerType.SemiDios)) != 0) |
                            ((withBlock.flags.Privilegios &
                              (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin)) != 0))

                            list = list + Declaraciones.UserList[i].name + ", ";
        }

        if (list.Length > 0)
            WriteConsoleMsg(UserIndex, "Caos conectados: " + list.Substring(0, Math.Min(list.Length - 2, list.Length)),
                FontTypeNames.FONTTYPE_INFO);
        else
            WriteConsoleMsg(UserIndex, "No hay Caos conectados.", FontTypeNames.FONTTYPE_INFO);
    }

    // '
    // Handles the "GoNearby" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGoNearby(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 01/10/07
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tIndex;
            int X;
            int Y;
            int i;
            var found = default(bool);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();


                tIndex = Extra.NameIndex(UserName);

                // Check the user has enough powers
                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                    Declaraciones.PlayerType.SemiDios |
                                                    Declaraciones.PlayerType.Consejero)) != 0)
                    // Si es dios o Admins no podemos salvo que nosotros también lo seamos
                    if (!(ES.EsDios(UserName) | ES.EsAdmin(UserName)) | ((withBlock.flags.Privilegios &
                                                                          (Declaraciones.PlayerType.Dios |
                                                                           Declaraciones.PlayerType.Admin)) != 0))
                    {
                        if (tIndex <= 0) // existe el usuario destino?
                        {
                            WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO);
                        }
                        else
                        {
                            for (i = 2; i <= 5; i++) // esto for sirve ir cambiando la distancia destino
                            {
                                var loopTo = Declaraciones.UserList[tIndex].Pos.X + i;
                                for (X = Declaraciones.UserList[tIndex].Pos.X - i; X <= loopTo; X++)
                                {
                                    var loopTo1 = Declaraciones.UserList[tIndex].Pos.Y + i;
                                    for (Y = Declaraciones.UserList[tIndex].Pos.Y - i; Y <= loopTo1; Y++)
                                        if (Declaraciones.MapData[Declaraciones.UserList[tIndex].Pos.Map, X, Y]
                                                .UserIndex == 0)
                                            if (Extra.LegalPos(Declaraciones.UserList[tIndex].Pos.Map,
                                                    Convert.ToInt16(X), Convert.ToInt16(Y), true))
                                            {
                                                UsUaRiOs.WarpUserChar(UserIndex, Declaraciones.UserList[tIndex].Pos.Map,
                                                    Convert.ToInt16(X), Convert.ToInt16(Y), true);
                                                var argtexto = "/IRCERCA " + UserName + " Mapa:" +
                                                               Declaraciones.UserList[tIndex].Pos.Map + " X:" +
                                                               Declaraciones.UserList[tIndex].Pos.X + " Y:" +
                                                               Declaraciones.UserList[tIndex].Pos.Y;
                                                General.LogGM(ref withBlock.name, ref argtexto);
                                                found = true;
                                                break;
                                            }

                                    if (found)
                                        break; // Feo, pero hay que abortar 3 fors sin usar GoTo
                                }

                                if (found)
                                    break; // Feo, pero hay que abortar 3 fors sin usar GoTo
                            }

                            // No space found??
                            if (!found)
                                WriteConsoleMsg(UserIndex, "Todos los lugares están ocupados.",
                                    FontTypeNames.FONTTYPE_INFO);
                        }
                    }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGoNearby: " + ex.Message);
        }
    }

    // '
    // Handles the "Comment" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleComment(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string comment;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                comment = buffer.ReadASCIIString();

                if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) == 0)
                {
                    var argtexto = "Comentario: " + comment;
                    General.LogGM(ref withBlock.name, ref argtexto);
                    WriteConsoleMsg(UserIndex, "Comentario salvado...", FontTypeNames.FONTTYPE_INFO);
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleComment: " + ex.Message);
        }
    }

    // '
    // Handles the "ServerTime" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleServerTime(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 01/08/07
        // Last Modification by: (liquid)
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                return;

            var argtexto = "Hora.";
            General.LogGM(ref withBlock.name, ref argtexto);
        }

        modSendData.SendData(modSendData.SendTarget.ToAll, 0,
            PrepareMessageConsoleMsg(
                "Hora: " + Conversions.ToString(DateAndTime.TimeOfDay) + " " + Conversions.ToString(DateTime.Today),
                FontTypeNames.FONTTYPE_INFO));
    }

    // '
    // Handles the "Where" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleWhere(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) == 0)
                {
                    tUser = Extra.NameIndex(UserName);
                    if (tUser <= 0)
                    {
                        WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO);
                    }
                    else if (((Declaraciones.UserList[tUser].flags.Privilegios & (Declaraciones.PlayerType.User |
                                 Declaraciones.PlayerType.Consejero | Declaraciones.PlayerType.SemiDios)) != 0) |
                             (((Declaraciones.UserList[tUser].flags.Privilegios &
                                (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin)) != 0) &
                              ((withBlock.flags.Privilegios &
                                (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin)) != 0)))

                    {
                        WriteConsoleMsg(UserIndex,
                            "Ubicación  " + UserName + ": " + Declaraciones.UserList[tUser].Pos.Map + ", " +
                            Declaraciones.UserList[tUser].Pos.X + ", " + Declaraciones.UserList[tUser].Pos.Y + ".",
                            FontTypeNames.FONTTYPE_INFO);
                        var argtexto = "/Donde " + UserName;
                        General.LogGM(ref withBlock.name, ref argtexto);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleWhere: " + ex.Message);
        }
    }

    // '
    // Handles the "CreaturesInMap" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCreaturesInMap(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 30/07/06
        // Pablo (ToxicWaste): modificaciones generales para simplificar la visualización.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        short Map;
        var NPCcant1 = default(short[]);
        var NPCcant2 = default(short[]);
        var List1 = default(string[]);
        var List2 = default(string[]);
        var NPCcount1 = default(int);
        var NPCcount2 = default(short);
        short i;
        int j;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            Map = withBlock.incomingData.ReadInteger();

            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                return;

            if (General.MapaValido(Map))
            {
                var loopTo = Declaraciones.LastNPC;
                for (i = 1; i <= loopTo; i++)
                    // VB isn't lazzy, so we put more restrictive condition first to speed up the process
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    if (Declaraciones.Npclist[i].Pos.Map == Map)
                    {
                        // ¿esta vivo?
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        if (Declaraciones.Npclist[i].flags.NPCActive & (Declaraciones.Npclist[i].Hostile == 1) &
                            (Declaraciones.Npclist[i].Stats.Alineacion == 2))
                        {
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            if (NPCcount1 == 0)
                            {
                                List1 = new string[1];
                                NPCcant1 = new short[1];
                                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                NPCcount1 = 1;
                                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                List1[0] = Declaraciones.Npclist[i].name + ": (" + Declaraciones.Npclist[i].Pos.X +
                                           "," + Declaraciones.Npclist[i].Pos.Y + ")";
                                NPCcant1[0] = 1;
                            }
                            else
                            {
                                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                var loopTo1 = NPCcount1 - 1;
                                for (j = 0; j <= loopTo1; j++)
                                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                    if ((List1[j].Substring(0,
                                            Math.Min(Declaraciones.Npclist[i].name.Length, List1[j].Length)) ?? "") ==
                                        (Declaraciones.Npclist[i].name ?? ""))
                                    {
                                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                        List1[j] = List1[j] + ", (" + Declaraciones.Npclist[i].Pos.X + "," +
                                                   Declaraciones.Npclist[i].Pos.Y + ")";
                                        NPCcant1[j] = Convert.ToInt16(NPCcant1[j] + 1);
                                        break;
                                    }

                                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                if (j == NPCcount1)
                                {
                                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                    Array.Resize(ref List1, NPCcount1 + 1);
                                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                    Array.Resize(ref NPCcant1, NPCcount1 + 1);
                                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                    NPCcount1 = NPCcount1 + 1;
                                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                    List1[j] = Declaraciones.Npclist[i].name + ": (" + Declaraciones.Npclist[i].Pos.X +
                                               "," + Declaraciones.Npclist[i].Pos.Y + ")";
                                    NPCcant1[j] = 1;
                                }
                            }
                        }
                        else if (NPCcount2 == 0)
                        {
                            List2 = new string[1];
                            NPCcant2 = new short[1];
                            NPCcount2 = 1;
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            List2[0] = Declaraciones.Npclist[i].name + ": (" + Declaraciones.Npclist[i].Pos.X + "," +
                                       Declaraciones.Npclist[i].Pos.Y + ")";
                            NPCcant2[0] = 1;
                        }
                        else
                        {
                            var loopTo2 = NPCcount2 - 1;
                            for (j = 0; j <= loopTo2; j++)
                                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                if ((List2[j].Substring(0,
                                        Math.Min(Declaraciones.Npclist[i].name.Length, List2[j].Length)) ?? "") ==
                                    (Declaraciones.Npclist[i].name ?? ""))
                                {
                                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                    List2[j] = List2[j] + ", (" + Declaraciones.Npclist[i].Pos.X + "," +
                                               Declaraciones.Npclist[i].Pos.Y + ")";
                                    NPCcant2[j] = Convert.ToInt16(NPCcant2[j] + 1);
                                    break;
                                }

                            if (j == NPCcount2)
                            {
                                Array.Resize(ref List2, NPCcount2 + 1);
                                Array.Resize(ref NPCcant2, NPCcount2 + 1);
                                NPCcount2 = Convert.ToInt16(NPCcount2 + 1);
                                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto i. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                List2[j] = Declaraciones.Npclist[i].name + ": (" + Declaraciones.Npclist[i].Pos.X +
                                           "," + Declaraciones.Npclist[i].Pos.Y + ")";
                                NPCcant2[j] = 1;
                            }
                        }
                    }

                WriteConsoleMsg(UserIndex, "Npcs Hostiles en mapa: ", FontTypeNames.FONTTYPE_WARNING);
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                if (NPCcount1 == 0)
                {
                    WriteConsoleMsg(UserIndex, "No hay NPCS Hostiles.", FontTypeNames.FONTTYPE_INFO);
                }
                else
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto NPCcount1. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    var loopTo3 = NPCcount1 - 1;
                    for (j = 0; j <= loopTo3; j++)
                        WriteConsoleMsg(UserIndex, NPCcant1[j] + " " + List1[j], FontTypeNames.FONTTYPE_INFO);
                }

                WriteConsoleMsg(UserIndex, "Otros Npcs en mapa: ", FontTypeNames.FONTTYPE_WARNING);
                if (NPCcount2 == 0)
                {
                    WriteConsoleMsg(UserIndex, "No hay más NPCS.", FontTypeNames.FONTTYPE_INFO);
                }
                else
                {
                    var loopTo4 = NPCcount2 - 1;
                    for (j = 0; j <= loopTo4; j++)
                        WriteConsoleMsg(UserIndex, NPCcant2[j] + " " + List2[j], FontTypeNames.FONTTYPE_INFO);
                }

                var argtexto = "Numero enemigos en mapa " + Map;
                General.LogGM(ref withBlock.name, ref argtexto);
            }
        }
    }

    // '
    // Handles the "WarpMeToTarget" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleWarpMeToTarget(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 26/03/09
        // 26/03/06: ZaMa - Chequeo que no se teletransporte donde haya un char o npc
        // ***************************************************
        short X;
        short Y;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                return;

            X = withBlock.flags.TargetX;
            Y = withBlock.flags.TargetY;

            Extra.FindLegalPos(UserIndex, withBlock.flags.TargetMap, ref X, ref Y);
            UsUaRiOs.WarpUserChar(UserIndex, withBlock.flags.TargetMap, X, Y, true);
            var argtexto = "/TELEPLOC a x:" + withBlock.flags.TargetX + " Y:" + withBlock.flags.TargetY + " Map:" +
                           withBlock.Pos.Map;
            General.LogGM(ref withBlock.name, ref argtexto);
        }
    }

    // '
    // Handles the "WarpChar" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleWarpChar(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 26/03/2009
        // 26/03/2009: ZaMa -  Chequeo que no se teletransporte a un tile donde haya un char o npc.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 7)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short Map;
            short X;
            short Y;
            var tUser = default(short);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();
                Map = buffer.ReadInteger();
                X = buffer.ReadByte();
                Y = buffer.ReadByte();

                if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) == 0)
                    if (General.MapaValido(Map) & (Migration.migr_LenB(UserName) != 0))
                    {
                        if (UserName.ToUpper() != "YO")
                        {
                            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.Consejero) == 0)
                                tUser = Extra.NameIndex(UserName);
                        }
                        else
                        {
                            tUser = UserIndex;
                        }

                        if (tUser <= 0)
                        {
                            WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO);
                        }
                        else if (Extra.InMapBounds(Map, X, Y))
                        {
                            Extra.FindLegalPos(tUser, Map, ref X, ref Y);
                            UsUaRiOs.WarpUserChar(tUser, Map, X, Y, true, true);
                            WriteConsoleMsg(UserIndex, Declaraciones.UserList[tUser].name + " transportado.",
                                FontTypeNames.FONTTYPE_INFO);
                            var argtexto = "Transportó a " + Declaraciones.UserList[tUser].name + " hacia " + "Mapa" +
                                           Map + " X:" + X + " Y:" + Y;
                            General.LogGM(ref withBlock.name, ref argtexto);
                        }
                    }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleWarpChar: " + ex.Message);
        }
    }

    // '
    // Handles the "Silence" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleSilence(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) == 0)
                {
                    tUser = Extra.NameIndex(UserName);

                    if (tUser <= 0)
                    {
                        WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO);
                    }
                    else if (Declaraciones.UserList[tUser].flags.Silenciado == 0)
                    {
                        Declaraciones.UserList[tUser].flags.Silenciado = 1;
                        WriteConsoleMsg(UserIndex, "Usuario silenciado.", FontTypeNames.FONTTYPE_INFO);
                        WriteShowMessageBox(tUser,
                            "Estimado usuario, ud. ha sido silenciado por los administradores. Sus denuncias serán ignoradas por el servidor de aquí en más. Utilice /GM para contactar un administrador.");
                        var argtexto = "/silenciar " + Declaraciones.UserList[tUser].name;
                        General.LogGM(ref withBlock.name, ref argtexto);

                        // Flush the other user's buffer
                        FlushBuffer(tUser);
                    }
                    else
                    {
                        Declaraciones.UserList[tUser].flags.Silenciado = 0;
                        WriteConsoleMsg(UserIndex, "Usuario des silenciado.", FontTypeNames.FONTTYPE_INFO);
                        var argtexto1 = "/DESsilenciar " + Declaraciones.UserList[tUser].name;
                        General.LogGM(ref withBlock.name, ref argtexto1);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleSilence: " + ex.Message);
        }
    }

    // '
    // Handles the "SOSShowList" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleSOSShowList(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                return;
            WriteShowSOSForm(UserIndex);
        }
    }

    // '
    // Handles the "RequestPartyForm" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandlePartyForm(short UserIndex)
    {
        // ***************************************************
        // Author: Budi
        // Last Modification: 11/26/09
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();
            if (withBlock.PartyIndex > 0)
                WriteShowPartyForm(UserIndex);

            else
                WriteConsoleMsg(UserIndex, "No perteneces a ningún grupo!", FontTypeNames.FONTTYPE_INFOBOLD);
        }
    }

    // '
    // Handles the "ItemUpgrade" message.
    // 
    // @param    UserIndex The index of the user sending the message.

    private static void HandleItemUpgrade(short UserIndex)
    {
        // ***************************************************
        // Author: Torres Patricio
        // Last Modification: 12/09/09
        // 
        // ***************************************************
        short ItemIndex;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];

            // Remove packet ID
            withBlock.incomingData.ReadByte();

            ItemIndex = withBlock.incomingData.ReadInteger();

            if (ItemIndex <= 0)
                return;
            if (!Trabajo.TieneObjetos(ItemIndex, 1, UserIndex))
                return;

            Trabajo.DoUpgrade(UserIndex, ItemIndex);
        }
    }

    // '
    // Handles the "SOSRemove" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleSOSRemove(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                UserName = buffer.ReadASCIIString();

                if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) == 0)
                    Declaraciones.Ayuda.Quitar(UserName);

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleSOSRemove: " + ex.Message);
        }
    }

    // '
    // Handles the "GoToChar" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGoToChar(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 26/03/2009
        // 26/03/2009: ZaMa -  Chequeo que no se teletransporte a un tile donde haya un char o npc.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            short X;
            short Y;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();
                tUser = Extra.NameIndex(UserName);

                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin |
                                                    Declaraciones.PlayerType.SemiDios |
                                                    Declaraciones.PlayerType.Consejero)) != 0)
                    // Si es dios o Admins no podemos salvo que nosotros también lo seamos
                    if (!(ES.EsDios(UserName) | ES.EsAdmin(UserName)) | ((withBlock.flags.Privilegios &
                                                                          (Declaraciones.PlayerType.Dios |
                                                                           Declaraciones.PlayerType.Admin)) != 0))
                    {
                        if (tUser <= 0)
                        {
                            WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO);
                        }
                        else
                        {
                            X = Declaraciones.UserList[tUser].Pos.X;
                            Y = Convert.ToInt16(Declaraciones.UserList[tUser].Pos.Y + 1);
                            Extra.FindLegalPos(UserIndex, Declaraciones.UserList[tUser].Pos.Map, ref X, ref Y);

                            UsUaRiOs.WarpUserChar(UserIndex, Declaraciones.UserList[tUser].Pos.Map, X, Y, true);

                            if (withBlock.flags.AdminInvisible == 0)
                            {
                                WriteConsoleMsg(tUser, withBlock.name + " se ha trasportado hacia donde te encuentras.",
                                    FontTypeNames.FONTTYPE_INFO);
                                FlushBuffer(tUser);
                            }

                            var argtexto = "/IRA " + UserName + " Mapa:" + Declaraciones.UserList[tUser].Pos.Map +
                                           " X:" + Declaraciones.UserList[tUser].Pos.X + " Y:" +
                                           Declaraciones.UserList[tUser].Pos.Y;
                            General.LogGM(ref withBlock.name, ref argtexto);
                        }
                    }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGoToChar: " + ex.Message);
        }
    }

    // '
    // Handles the "Invisible" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleInvisible(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                return;

            Trabajo.DoAdminInvisible(UserIndex);
            var argtexto = "/INVISIBLE";
            General.LogGM(ref withBlock.name, ref argtexto);
        }
    }

    // '
    // Handles the "GMPanel" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGMPanel(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                return;

            WriteShowGMPanelForm(UserIndex);
        }
    }

    // '
    // Handles the "GMPanel" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestUserList(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 01/09/07
        // Last modified by: Lucas Tavolaro Ortiz (Tavo)
        // I haven`t found a solution to split, so i make an array of names
        // ***************************************************
        int i;
        string[] names;
        int Count;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.RoleMaster)) !=
                0)
                return;

            // UPGRADE_WARNING: El límite inferior de la matriz names ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            names = new string[Declaraciones.LastUser + 1];
            Count = 1;

            var loopTo = (int)Declaraciones.LastUser;
            for (i = 1; i <= loopTo; i++)
                if (Migration.migr_LenB(Declaraciones.UserList[i].name) != 0)
                    if ((Declaraciones.UserList[i].flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                    {
                        names[Count] = Declaraciones.UserList[i].name;
                        Count = Count + 1;
                    }

            if (Count > 1)
                WriteUserNameList(UserIndex, names, Convert.ToInt16(Count - 1));
        }
    }

    // '
    // Handles the "Working" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleWorking(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        int i;
        var users = default(string);

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.RoleMaster)) !=
                0)
                return;

            var loopTo = (int)Declaraciones.LastUser;
            for (i = 1; i <= loopTo; i++)
                if (Declaraciones.UserList[i].flags.UserLogged & (Declaraciones.UserList[i].Counters.Trabajando > 0))
                {
                    users = users + ", " + Declaraciones.UserList[i].name;

                    // Display the user being checked by the centinel
                    if (modCentinela.Centinela.RevisandoUserIndex == i)
                        users = users + " (*)";
                }

            if (Migration.migr_LenB(users) != 0)
            {
                users = users.Substring(Math.Max(0, users.Length - users.Length - 2));
                WriteConsoleMsg(UserIndex, "Usuarios trabajando: " + users, FontTypeNames.FONTTYPE_INFO);
            }
            else
            {
                WriteConsoleMsg(UserIndex, "No hay usuarios trabajando.", FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Handles the "Hiding" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleHiding(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        int i;
        var users = default(string);

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.RoleMaster)) !=
                0)
                return;

            var loopTo = (int)Declaraciones.LastUser;
            for (i = 1; i <= loopTo; i++)
                if ((Migration.migr_LenB(Declaraciones.UserList[i].name) != 0) &
                    (Declaraciones.UserList[i].Counters.Ocultando > 0))
                    users = users + Declaraciones.UserList[i].name + ", ";

            if (Migration.migr_LenB(users) != 0)
            {
                users = users.Substring(0, Math.Min(users.Length - 2, users.Length));
                WriteConsoleMsg(UserIndex, "Usuarios ocultandose: " + users, FontTypeNames.FONTTYPE_INFO);
            }
            else
            {
                WriteConsoleMsg(UserIndex, "No hay usuarios ocultandose.", FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Handles the "Jail" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleJail(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 6)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            string reason;
            byte jailTime;
            byte Count;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();
                reason = buffer.ReadASCIIString();
                jailTime = buffer.ReadByte();

                if (UserName.IndexOf("+", 1 - 1) + 1 != 0) UserName = UserName.Replace("+", " ");

                // /carcel nick@motivo@<tiempo>
                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0))
                {
                    if ((Migration.migr_LenB(UserName) == 0) | (Migration.migr_LenB(reason) == 0))
                    {
                        WriteConsoleMsg(UserIndex, "Utilice /carcel nick@motivo@tiempo", FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        tUser = Extra.NameIndex(UserName);

                        if (tUser <= 0)
                        {
                            WriteConsoleMsg(UserIndex, "El usuario no está online.", FontTypeNames.FONTTYPE_INFO);
                        }
                        else if ((Declaraciones.UserList[tUser].flags.Privilegios & Declaraciones.PlayerType.User) == 0)
                        {
                            WriteConsoleMsg(UserIndex, "No puedes encarcelar a administradores.",
                                FontTypeNames.FONTTYPE_INFO);
                        }
                        else if (jailTime > 60)
                        {
                            WriteConsoleMsg(UserIndex, "No puedés encarcelar por más de 60 minutos.",
                                FontTypeNames.FONTTYPE_INFO);
                        }
                        else
                        {
                            if (Migration.migr_InStrB(UserName, @"\") != 0) UserName = UserName.Replace(@"\", "");
                            if (Migration.migr_InStrB(UserName, "/") != 0) UserName = UserName.Replace("/", "");

                            if (File.Exists(Declaraciones.CharPath + UserName + ".chr"))
                            {
                                var argEmptySpaces = 1024;
                                Count = Convert.ToByte(Migration.ParseVal(ES.GetVar(
                                    Declaraciones.CharPath + UserName + ".chr", "PENAS", "Cant", ref argEmptySpaces)));
                                ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "PENAS", "Cant",
                                    (Count + 1).ToString());
                                ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "PENAS", "P" + (Count + 1),
                                    withBlock.name.ToLower() + ": CARCEL " + jailTime + "m, MOTIVO: " +
                                    reason.ToLower() + " " + Conversions.ToString(DateTime.Today) + " " +
                                    Conversions.ToString(DateAndTime.TimeOfDay));
                            }

                            Admin.Encarcelar(tUser, jailTime, withBlock.name);
                            var argtexto = " encarceló a " + UserName;
                            General.LogGM(ref withBlock.name, ref argtexto);
                        }
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleJail: " + ex.Message);
        }
    }

    // '
    // Handles the "KillNPC" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleKillNPC(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 04/22/08 (NicoNZ)
        // 
        // ***************************************************
        short tNPC;
        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura auxNPC, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Declaraciones.npc auxNPC;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                return;


            // Los consejeros no pueden RMATAr a nada en el mapa pretoriano
            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.Consejero) != 0)
                if (withBlock.Pos.Map == PraetoriansCoopNPC.MAPA_PRETORIANO)
                {
                    WriteConsoleMsg(UserIndex, "Los consejeros no pueden usar este comando en el mapa pretoriano.",
                        FontTypeNames.FONTTYPE_INFO);
                    return;
                }

            tNPC = withBlock.flags.TargetNPC;

            if (tNPC > 0)
            {
                WriteConsoleMsg(UserIndex, "RMatas (con posible respawn) a: " + Declaraciones.Npclist[tNPC].name,
                    FontTypeNames.FONTTYPE_INFO);

                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto auxNPC. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                auxNPC = Declaraciones.Npclist[tNPC];
                NPCs.QuitarNPC(tNPC);
                NPCs.ReSpawnNpc(ref auxNPC);

                withBlock.flags.TargetNPC = 0;
            }
            else
            {
                WriteConsoleMsg(UserIndex, "Antes debes hacer click sobre el NPC.", FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Handles the "WarnUser" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleWarnUser(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/26/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            string reason;
            Declaraciones.PlayerType privs;
            byte Count;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();
                reason = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0))
                {
                    if ((Migration.migr_LenB(UserName) == 0) | (Migration.migr_LenB(reason) == 0))
                    {
                        WriteConsoleMsg(UserIndex, "Utilice /advertencia nick@motivo", FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        privs = Admin.UserDarPrivilegioLevel(UserName);

                        if ((privs & Declaraciones.PlayerType.User) == 0)
                        {
                            WriteConsoleMsg(UserIndex, "No puedes advertir a administradores.",
                                FontTypeNames.FONTTYPE_INFO);
                        }
                        else
                        {
                            if (Migration.migr_InStrB(UserName, @"\") != 0) UserName = UserName.Replace(@"\", "");
                            if (Migration.migr_InStrB(UserName, "/") != 0) UserName = UserName.Replace("/", "");

                            if (File.Exists(Declaraciones.CharPath + UserName + ".chr"))
                            {
                                var argEmptySpaces = 1024;
                                Count = Convert.ToByte(Migration.ParseVal(ES.GetVar(
                                    Declaraciones.CharPath + UserName + ".chr", "PENAS", "Cant", ref argEmptySpaces)));
                                ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "PENAS", "Cant",
                                    (Count + 1).ToString());
                                ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "PENAS", "P" + (Count + 1),
                                    withBlock.name.ToLower() + ": ADVERTENCIA por: " + reason.ToLower() + " " +
                                    Conversions.ToString(DateTime.Today) + " " +
                                    Conversions.ToString(DateAndTime.TimeOfDay));

                                WriteConsoleMsg(UserIndex, "Has advertido a " + UserName.ToUpper() + ".",
                                    FontTypeNames.FONTTYPE_INFO);
                                var argtexto = " advirtio a " + UserName;
                                General.LogGM(ref withBlock.name, ref argtexto);
                            }
                        }
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleWarnUser: " + ex.Message);
        }
    }

    // '
    // Handles the "EditChar" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleEditChar(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 11/06/2009
        // 02/03/2009: ZaMa - Cuando editas nivel, chequea si el pj puede permanecer en clan faccionario
        // 11/06/2009: ZaMa - Todos los comandos se pueden usar aunque el pj este offline
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 8)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            byte opcion;
            string Arg1;
            string Arg2;
            var valido = default(bool);
            byte LoopC;
            string CommandString;
            string UserCharPath;
            int Var;
            short GI;
            byte Sex;
            byte raza;
            int bankGold;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString().Replace("+", " ");

                if (UserName.ToUpper() == "YO")
                    tUser = UserIndex;
                else
                    tUser = Extra.NameIndex(UserName);

                opcion = buffer.ReadByte();
                Arg1 = buffer.ReadASCIIString();
                Arg2 = buffer.ReadASCIIString();

                if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0)
                    switch (withBlock.flags.Privilegios & (Declaraciones.PlayerType.Dios |
                                                           Declaraciones.PlayerType.SemiDios |
                                                           Declaraciones.PlayerType.Consejero))
                    {
                        case Declaraciones.PlayerType.Consejero:
                        {
                            // Los RMs consejeros sólo se pueden editar su head, body y level
                            valido = (tUser == UserIndex) & ((opcion == (byte)eEditOptions.eo_Body) |
                                                             (opcion == (byte)eEditOptions.eo_Head) |
                                                             (opcion == (byte)eEditOptions.eo_Level));
                            break;
                        }

                        case Declaraciones.PlayerType.SemiDios:
                        {
                            // Los RMs sólo se pueden editar su level y el head y body de cualquiera
                            valido = ((opcion == (byte)eEditOptions.eo_Level) & (tUser == UserIndex)) |
                                     (opcion == (byte)eEditOptions.eo_Body) | (opcion == (byte)eEditOptions.eo_Head);
                            break;
                        }

                        case Declaraciones.PlayerType.Dios:
                        {
                            // Los DRMs pueden aplicar los siguientes comandos sobre cualquiera
                            // pero si quiere modificar el level sólo lo puede hacer sobre sí mismo
                            valido = ((opcion == (byte)eEditOptions.eo_Level) & (tUser == UserIndex)) |
                                     (opcion == (byte)eEditOptions.eo_Body) | (opcion == (byte)eEditOptions.eo_Head) |
                                     (opcion == (byte)eEditOptions.eo_CiticensKilled) |
                                     (opcion == (byte)eEditOptions.eo_CriminalsKilled) |
                                     (opcion == (byte)eEditOptions.eo_Class) |
                                     (opcion == (byte)eEditOptions.eo_Skills) |
                                     (opcion == (byte)eEditOptions.eo_addGold);
                            break;
                        }
                    }

                else if ((withBlock.flags.Privilegios &
                          (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) != 0)
                    // Si no es RM debe ser dios para poder usar este comando
                    valido = true;

                if (valido)
                {
                    UserCharPath = Declaraciones.CharPath + UserName + ".chr";
                    if ((tUser <= 0) & !File.Exists(UserCharPath))
                    {
                        WriteConsoleMsg(UserIndex, "Estás intentando editar un usuario inexistente.",
                            FontTypeNames.FONTTYPE_INFO);
                        var argtexto = "Intentó editar un usuario inexistente.";
                        General.LogGM(ref withBlock.name, ref argtexto);
                    }
                    else
                    {
                        // For making the Log
                        CommandString = "/MOD ";

                        switch (opcion)
                        {
                            case (byte)eEditOptions.eo_Gold:
                            {
                                if (Migration.ParseVal(Arg1) <= Declaraciones.MAX_ORO_EDIT)
                                {
                                    if (tUser <= 0) // Esta offline?
                                    {
                                        ES.WriteVar(UserCharPath, "STATS", "GLD", Migration.ParseVal(Arg1).ToString());
                                        WriteConsoleMsg(UserIndex, "Charfile Alterado: " + UserName,
                                            FontTypeNames.FONTTYPE_INFO);
                                    }
                                    else // Online
                                    {
                                        Declaraciones.UserList[tUser].Stats.GLD =
                                            Convert.ToInt32(Migration.ParseVal(Arg1));
                                        WriteUpdateGold(tUser);
                                    }
                                }
                                else
                                {
                                    WriteConsoleMsg(UserIndex,
                                        "No está permitido utilizar valores mayores a " + Declaraciones.MAX_ORO_EDIT +
                                        ". Su comando ha quedado en los logs del juego.", FontTypeNames.FONTTYPE_INFO);
                                }

                                // Log it
                                CommandString = CommandString + "ORO ";
                                break;
                            }

                            case (byte)eEditOptions.eo_Experience:
                            {
                                if (Migration.ParseVal(Arg1) > 20000000d) Arg1 = 20000000.ToString();

                                if (tUser <= 0) // Offline
                                {
                                    var argEmptySpaces = 1024;
                                    Var = Convert.ToInt32(ES.GetVar(UserCharPath, "STATS", "EXP", ref argEmptySpaces));
                                    ES.WriteVar(UserCharPath, "STATS", "EXP",
                                        (Var + Migration.ParseVal(Arg1)).ToString());
                                    WriteConsoleMsg(UserIndex, "Charfile Alterado: " + UserName,
                                        FontTypeNames.FONTTYPE_INFO);
                                }
                                else // Online
                                {
                                    Declaraciones.UserList[tUser].Stats.Exp = Declaraciones.UserList[tUser].Stats.Exp +
                                                                              Migration.ParseVal(Arg1);
                                    UsUaRiOs.CheckUserLevel(tUser);
                                    WriteUpdateExp(tUser);
                                }

                                // Log it
                                CommandString = CommandString + "EXP ";
                                break;
                            }

                            case (byte)eEditOptions.eo_Body:
                            {
                                if (tUser <= 0)
                                {
                                    ES.WriteVar(UserCharPath, "INIT", "Body", Arg1);
                                    WriteConsoleMsg(UserIndex, "Charfile Alterado: " + UserName,
                                        FontTypeNames.FONTTYPE_INFO);
                                }
                                else
                                {
                                    UsUaRiOs.ChangeUserChar(tUser, Convert.ToInt16(Migration.ParseVal(Arg1)),
                                        Declaraciones.UserList[tUser].character.Head,
                                        (byte)Declaraciones.UserList[tUser].character.heading,
                                        Declaraciones.UserList[tUser].character.WeaponAnim,
                                        Declaraciones.UserList[tUser].character.ShieldAnim,
                                        Declaraciones.UserList[tUser].character.CascoAnim);
                                }

                                // Log it
                                CommandString = CommandString + "BODY ";
                                break;
                            }

                            case (byte)eEditOptions.eo_Head:
                            {
                                if (tUser <= 0)
                                {
                                    ES.WriteVar(UserCharPath, "INIT", "Head", Arg1);
                                    WriteConsoleMsg(UserIndex, "Charfile Alterado: " + UserName,
                                        FontTypeNames.FONTTYPE_INFO);
                                }
                                else
                                {
                                    UsUaRiOs.ChangeUserChar(tUser, Declaraciones.UserList[tUser].character.body,
                                        Convert.ToInt16(Migration.ParseVal(Arg1)),
                                        (byte)Declaraciones.UserList[tUser].character.heading,
                                        Declaraciones.UserList[tUser].character.WeaponAnim,
                                        Declaraciones.UserList[tUser].character.ShieldAnim,
                                        Declaraciones.UserList[tUser].character.CascoAnim);
                                }

                                // Log it
                                CommandString = CommandString + "HEAD ";
                                break;
                            }

                            case (byte)eEditOptions.eo_CriminalsKilled:
                            {
                                Var = Migration.ParseVal(Arg1) > Declaraciones.MAXUSERMATADOS
                                    ? Declaraciones.MAXUSERMATADOS
                                    : Convert.ToInt32(Migration.ParseVal(Arg1));

                                if (tUser <= 0) // Offline
                                {
                                    ES.WriteVar(UserCharPath, "FACCIONES", "CrimMatados", Var.ToString());
                                    WriteConsoleMsg(UserIndex, "Charfile Alterado: " + UserName,
                                        FontTypeNames.FONTTYPE_INFO);
                                }
                                else // Online
                                {
                                    Declaraciones.UserList[tUser].Faccion.CriminalesMatados = Var;
                                }

                                // Log it
                                CommandString = CommandString + "CRI ";
                                break;
                            }

                            case (byte)eEditOptions.eo_CiticensKilled:
                            {
                                Var = Migration.ParseVal(Arg1) > Declaraciones.MAXUSERMATADOS
                                    ? Declaraciones.MAXUSERMATADOS
                                    : Convert.ToInt32(Migration.ParseVal(Arg1));

                                if (tUser <= 0) // Offline
                                {
                                    ES.WriteVar(UserCharPath, "FACCIONES", "CiudMatados", Var.ToString());
                                    WriteConsoleMsg(UserIndex, "Charfile Alterado: " + UserName,
                                        FontTypeNames.FONTTYPE_INFO);
                                }
                                else // Online
                                {
                                    Declaraciones.UserList[tUser].Faccion.CiudadanosMatados = Var;
                                }

                                // Log it
                                CommandString = CommandString + "CIU ";
                                break;
                            }

                            case (byte)eEditOptions.eo_Level:
                            {
                                if (Migration.ParseVal(Arg1) > Declaraciones.STAT_MAXELV)
                                {
                                    Arg1 = Declaraciones.STAT_MAXELV.ToString();
                                    WriteConsoleMsg(UserIndex,
                                        "No puedes tener un nivel superior a " + Declaraciones.STAT_MAXELV + ".",
                                        FontTypeNames.FONTTYPE_INFO);
                                }

                                // Chequeamos si puede permanecer en el clan
                                if (Migration.ParseVal(Arg1) >= 25d)
                                {
                                    if (tUser <= 0)
                                    {
                                        var argEmptySpaces1 = 1024;
                                        GI = Convert.ToInt16(ES.GetVar(UserCharPath, "GUILD", "GUILDINDEX",
                                            ref argEmptySpaces1));
                                    }
                                    else
                                    {
                                        GI = Declaraciones.UserList[tUser].GuildIndex;
                                    }

                                    if (GI > 0)
                                        if ((modGuilds.GuildAlignment(GI) == "Del Mal") |
                                            (modGuilds.GuildAlignment(GI) == "Real"))
                                        {
                                            // We get here, so guild has factionary alignment, we have to expulse the user
                                            modGuilds.m_EcharMiembroDeClan(-1, UserName);

                                            modSendData.SendData(modSendData.SendTarget.ToGuildMembers, GI,
                                                PrepareMessageConsoleMsg(UserName + " deja el clan.",
                                                    FontTypeNames.FONTTYPE_GUILD));
                                            // Si esta online le avisamos
                                            if (tUser > 0)
                                                WriteConsoleMsg(tUser,
                                                    "¡Ya tienes la madurez suficiente como para decidir bajo que estandarte pelearás! Por esta razón, hasta tanto no te enlistes en la facción bajo la cual tu clan está alineado, estarás excluído del mismo.",
                                                    FontTypeNames.FONTTYPE_GUILD);
                                        }
                                }

                                if (tUser <= 0) // Offline
                                {
                                    ES.WriteVar(UserCharPath, "STATS", "ELV", Migration.ParseVal(Arg1).ToString());
                                    WriteConsoleMsg(UserIndex, "Charfile Alterado: " + UserName,
                                        FontTypeNames.FONTTYPE_INFO);
                                }
                                else // Online
                                {
                                    Declaraciones.UserList[tUser].Stats.ELV = Convert.ToByte(Migration.ParseVal(Arg1));
                                    WriteUpdateUserStats(tUser);
                                }

                                // Log it
                                CommandString = CommandString + "LEVEL ";
                                break;
                            }

                            case (byte)eEditOptions.eo_Class:
                            {
                                for (LoopC = 1; LoopC <= Declaraciones.NUMCLASES; LoopC++)
                                    if ((Declaraciones.ListaClases[LoopC].ToUpper() ?? "") == (Arg1.ToUpper() ?? ""))
                                        break;

                                if (LoopC > Declaraciones.NUMCLASES)
                                {
                                    WriteConsoleMsg(UserIndex, "Clase desconocida. Intente nuevamente.",
                                        FontTypeNames.FONTTYPE_INFO);
                                }
                                else if (tUser <= 0) // Offline
                                {
                                    ES.WriteVar(UserCharPath, "INIT", "Clase", LoopC.ToString());
                                    WriteConsoleMsg(UserIndex, "Charfile Alterado: " + UserName,
                                        FontTypeNames.FONTTYPE_INFO);
                                }
                                else // Online
                                {
                                    Declaraciones.UserList[tUser].clase = (Declaraciones.eClass)LoopC;
                                }

                                // Log it
                                CommandString = CommandString + "CLASE ";
                                break;
                            }

                            case (byte)eEditOptions.eo_Skills:
                            {
                                for (LoopC = 1; LoopC <= Declaraciones.NUMSKILLS; LoopC++)
                                    if ((Declaraciones.SkillsNames[LoopC].Replace(" ", "+").ToUpper() ?? "") ==
                                        (Arg1.ToUpper() ?? ""))
                                        break;

                                if (LoopC > Declaraciones.NUMSKILLS)
                                {
                                    WriteConsoleMsg(UserIndex, "Skill Inexistente!", FontTypeNames.FONTTYPE_INFO);
                                }
                                else if (tUser <= 0) // Offline
                                {
                                    ES.WriteVar(UserCharPath, "Skills", "SK" + LoopC, Arg2);
                                    ES.WriteVar(UserCharPath, "Skills", "EXPSK" + LoopC, 0.ToString());

                                    if (Convert.ToDouble(Arg2) < Declaraciones.MAXSKILLPOINTS)
                                        ES.WriteVar(UserCharPath, "Skills", "ELUSK" + LoopC,
                                            (Declaraciones.ELU_SKILL_INICIAL * Math.Pow(1.05d, Convert.ToDouble(Arg2)))
                                            .ToString());
                                    else
                                        ES.WriteVar(UserCharPath, "Skills", "ELUSK" + LoopC, 0.ToString());

                                    WriteConsoleMsg(UserIndex, "Charfile Alterado: " + UserName,
                                        FontTypeNames.FONTTYPE_INFO);
                                }
                                else // Online
                                {
                                    Declaraciones.UserList[tUser].Stats.UserSkills[LoopC] =
                                        Convert.ToByte(Migration.ParseVal(Arg2));
                                    UsUaRiOs.CheckEluSkill(tUser, LoopC, true);
                                }

                                // Log it
                                CommandString = CommandString + "SKILLS ";
                                break;
                            }

                            case (byte)eEditOptions.eo_SkillPointsLeft:
                            {
                                if (tUser <= 0) // Offline
                                {
                                    ES.WriteVar(UserCharPath, "STATS", "SkillPtsLibres", Arg1);
                                    WriteConsoleMsg(UserIndex, "Charfile Alterado: " + UserName,
                                        FontTypeNames.FONTTYPE_INFO);
                                }
                                else // Online
                                {
                                    Declaraciones.UserList[tUser].Stats.SkillPts =
                                        Convert.ToInt16(Migration.ParseVal(Arg1));
                                }

                                // Log it
                                CommandString = CommandString + "SKILLSLIBRES ";
                                break;
                            }

                            case (byte)eEditOptions.eo_Nobleza:
                            {
                                Var = Migration.ParseVal(Arg1) > Declaraciones.MAXREP
                                    ? Declaraciones.MAXREP
                                    : Convert.ToInt32(Migration.ParseVal(Arg1));

                                if (tUser <= 0) // Offline
                                {
                                    ES.WriteVar(UserCharPath, "REP", "Nobles", Var.ToString());
                                    WriteConsoleMsg(UserIndex, "Charfile Alterado: " + UserName,
                                        FontTypeNames.FONTTYPE_INFO);
                                }
                                else // Online
                                {
                                    Declaraciones.UserList[tUser].Reputacion.NobleRep = Var;
                                }

                                // Log it
                                CommandString = CommandString + "NOB ";
                                break;
                            }

                            case (byte)eEditOptions.eo_Asesino:
                            {
                                Var = Migration.ParseVal(Arg1) > Declaraciones.MAXREP
                                    ? Declaraciones.MAXREP
                                    : Convert.ToInt32(Migration.ParseVal(Arg1));

                                if (tUser <= 0) // Offline
                                {
                                    ES.WriteVar(UserCharPath, "REP", "Asesino", Var.ToString());
                                    WriteConsoleMsg(UserIndex, "Charfile Alterado: " + UserName,
                                        FontTypeNames.FONTTYPE_INFO);
                                }
                                else // Online
                                {
                                    Declaraciones.UserList[tUser].Reputacion.AsesinoRep = Var;
                                }

                                // Log it
                                CommandString = CommandString + "ASE ";
                                break;
                            }

                            case (byte)eEditOptions.eo_Sex:
                            {
                                Sex = Arg1.ToUpper() == "MUJER" ? (byte)Declaraciones.eGenero.Mujer : (byte)0; // Mujer?
                                Sex = Arg1.ToUpper() == "HOMBRE" ? (byte)Declaraciones.eGenero.Hombre : Sex; // Hombre?

                                if (Sex != 0) // Es Hombre o mujer?
                                {
                                    if (tUser <= 0) // OffLine
                                    {
                                        ES.WriteVar(UserCharPath, "INIT", "Genero", Convert.ToInt32(Sex).ToString());
                                        WriteConsoleMsg(UserIndex, "Charfile Alterado: " + UserName,
                                            FontTypeNames.FONTTYPE_INFO);
                                    }
                                    else // Online
                                    {
                                        Declaraciones.UserList[tUser].Genero = (Declaraciones.eGenero)Sex;
                                    }
                                }
                                else
                                {
                                    WriteConsoleMsg(UserIndex, "Genero desconocido. Intente nuevamente.",
                                        FontTypeNames.FONTTYPE_INFO);
                                }

                                // Log it
                                CommandString = CommandString + "SEX ";
                                break;
                            }

                            case (byte)eEditOptions.eo_Raza:
                            {
                                Arg1 = Arg1.ToUpper();
                                switch (Arg1 ?? "")
                                {
                                    case "HUMANO":
                                    {
                                        raza = (byte)Declaraciones.eRaza.Humano;
                                        break;
                                    }
                                    case "ELFO":
                                    {
                                        raza = (byte)Declaraciones.eRaza.Elfo;
                                        break;
                                    }
                                    case "DROW":
                                    {
                                        raza = (byte)Declaraciones.eRaza.Drow;
                                        break;
                                    }
                                    case "ENANO":
                                    {
                                        raza = (byte)Declaraciones.eRaza.Enano;
                                        break;
                                    }
                                    case "GNOMO":
                                    {
                                        raza = (byte)Declaraciones.eRaza.Gnomo;
                                        break;
                                    }

                                    default:
                                    {
                                        raza = 0;
                                        break;
                                    }
                                }


                                if (raza == 0)
                                {
                                    WriteConsoleMsg(UserIndex, "Raza desconocida. Intente nuevamente.",
                                        FontTypeNames.FONTTYPE_INFO);
                                }
                                else if (tUser <= 0)
                                {
                                    ES.WriteVar(UserCharPath, "INIT", "Raza", Convert.ToInt32(raza).ToString());
                                    WriteConsoleMsg(UserIndex, "Charfile Alterado: " + UserName,
                                        FontTypeNames.FONTTYPE_INFO);
                                }
                                else
                                {
                                    Declaraciones.UserList[tUser].raza = (Declaraciones.eRaza)raza;
                                }

                                // Log it
                                CommandString = CommandString + "RAZA ";
                                break;
                            }

                            case (byte)eEditOptions.eo_addGold:
                            {
                                if (Math.Abs(Convert.ToDouble(Arg1)) > Declaraciones.MAX_ORO_EDIT)
                                {
                                    WriteConsoleMsg(UserIndex,
                                        "No está permitido utilizar valores mayores a " + Declaraciones.MAX_ORO_EDIT +
                                        ".", FontTypeNames.FONTTYPE_INFO);
                                }
                                else if (tUser <= 0)
                                {
                                    var argEmptySpaces2 = 1024;
                                    bankGold = Convert.ToInt32(ES.GetVar(Declaraciones.CharPath + UserName + ".chr",
                                        "STATS", "BANCO", ref argEmptySpaces2));
                                    ES.WriteVar(UserCharPath, "STATS", "BANCO",
                                        (bankGold + Migration.ParseVal(Arg1) <= 0d
                                            ? 0d
                                            : bankGold + Migration.ParseVal(Arg1)).ToString());
                                    WriteConsoleMsg(UserIndex,
                                        "Se le ha agregado " + Arg1 + " monedas de oro a " + UserName + ".",
                                        FontTypeNames.FONTTYPE_TALK);
                                }
                                else
                                {
                                    Declaraciones.UserList[tUser].Stats.Banco =
                                        Declaraciones.UserList[tUser].Stats.Banco +
                                        Convert.ToInt32(Migration.ParseVal(Arg1)) <= 0
                                            ? 0
                                            : Convert.ToInt32(Declaraciones.UserList[tUser].Stats.Banco +
                                                              Migration.ParseVal(Arg1));
                                    WriteConsoleMsg(tUser, Declaraciones.STANDARD_BOUNTY_HUNTER_MESSAGE,
                                        FontTypeNames.FONTTYPE_TALK);
                                }

                                // Log it
                                CommandString = CommandString + "AGREGAR ";
                                break;
                            }

                            default:
                            {
                                WriteConsoleMsg(UserIndex, "Comando no permitido.", FontTypeNames.FONTTYPE_INFO);
                                CommandString = CommandString + "UNKOWN ";
                                break;
                            }
                        }

                        CommandString = CommandString + Arg1 + " " + Arg2;
                        var argtexto1 = CommandString + " " + UserName;
                        General.LogGM(ref withBlock.name, ref argtexto1);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleEditChar: " + ex.Message);
        }
    }


    // '
    // Handles the "RequestCharInfo" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestCharInfo(short UserIndex)
    {
        // ***************************************************
        // Author: Fredy Horacio Treboux (liquid)
        // Last Modification: 01/08/07
        // Last Modification by: (liquid).. alto bug zapallo..
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string targetName;
            short TargetIndex;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                targetName = buffer.ReadASCIIString().Replace("+", " ");
                TargetIndex = Extra.NameIndex(targetName);


                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                    Declaraciones.PlayerType.SemiDios)) != 0)

                {
                    // is the player offline?
                    if (TargetIndex <= 0)
                    {
                        // don't allow to retrieve administrator's info
                        if (!(ES.EsDios(targetName) | ES.EsAdmin(targetName)))
                        {
                            WriteConsoleMsg(UserIndex, "Usuario offline, buscando en charfile.",
                                FontTypeNames.FONTTYPE_INFO);
                            UsUaRiOs.SendUserStatsTxtOFF(UserIndex, targetName);
                        }
                    }
                    // don't allow to retrieve administrator's info
                    else if ((Declaraciones.UserList[TargetIndex].flags.Privilegios & (Declaraciones.PlayerType.User |
                                 Declaraciones.PlayerType.Consejero | Declaraciones.PlayerType.SemiDios)) != 0)
                    {
                        UsUaRiOs.SendUserStatsTxt(UserIndex, TargetIndex);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleRequestCharInfo: " + ex.Message);
        }
    }

    // '
    // Handles the "RequestCharStats" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestCharStats(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                UserName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                     Declaraciones.PlayerType.SemiDios)) != 0))
                {
                    var argtexto = "/STAT " + UserName;
                    General.LogGM(ref withBlock.name, ref argtexto);

                    tUser = Extra.NameIndex(UserName);

                    if (tUser <= 0)
                    {
                        WriteConsoleMsg(UserIndex, "Usuario offline. Leyendo charfile... ",
                            FontTypeNames.FONTTYPE_INFO);

                        UsUaRiOs.SendUserMiniStatsTxtFromChar(UserIndex, UserName);
                    }
                    else
                    {
                        UsUaRiOs.SendUserMiniStatsTxt(UserIndex, tUser);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleRequestCharStats: " + ex.Message);
        }
    }

    // '
    // Handles the "RequestCharGold" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestCharGold(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();
                tUser = Extra.NameIndex(UserName);

                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                    Declaraciones.PlayerType.SemiDios)) != 0)

                {
                    var argtexto = "/BAL " + UserName;
                    General.LogGM(ref withBlock.name, ref argtexto);

                    if (tUser <= 0)
                    {
                        WriteConsoleMsg(UserIndex, "Usuario offline. Leyendo charfile... ",
                            FontTypeNames.FONTTYPE_TALK);

                        UsUaRiOs.SendUserOROTxtFromChar(UserIndex, UserName);
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex,
                            "El usuario " + UserName + " tiene " + Declaraciones.UserList[tUser].Stats.Banco +
                            " en el banco.", FontTypeNames.FONTTYPE_TALK);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleRequestCharGold: " + ex.Message);
        }
    }

    // '
    // Handles the "RequestCharInventory" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestCharInventory(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();
                tUser = Extra.NameIndex(UserName);


                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                    Declaraciones.PlayerType.SemiDios)) != 0)

                {
                    var argtexto = "/INV " + UserName;
                    General.LogGM(ref withBlock.name, ref argtexto);

                    if (tUser <= 0)
                    {
                        WriteConsoleMsg(UserIndex, "Usuario offline. Leyendo del charfile...",
                            FontTypeNames.FONTTYPE_TALK);

                        UsUaRiOs.SendUserInvTxtFromChar(UserIndex, UserName);
                    }
                    else
                    {
                        UsUaRiOs.SendUserInvTxt(UserIndex, tUser);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleRequestCharInventory: " + ex.Message);
        }
    }

    // '
    // Handles the "RequestCharBank" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestCharBank(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();
                tUser = Extra.NameIndex(UserName);


                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                    Declaraciones.PlayerType.SemiDios)) != 0)

                {
                    var argtexto = "/BOV " + UserName;
                    General.LogGM(ref withBlock.name, ref argtexto);

                    if (tUser <= 0)
                    {
                        WriteConsoleMsg(UserIndex, "Usuario offline. Leyendo charfile... ",
                            FontTypeNames.FONTTYPE_TALK);

                        modBanco.SendUserBovedaTxtFromChar(UserIndex, UserName);
                    }
                    else
                    {
                        modBanco.SendUserBovedaTxt(UserIndex, tUser);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleRequestCharBank: " + ex.Message);
        }
    }

    // '
    // Handles the "RequestCharSkills" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRequestCharSkills(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            int LoopC;
            var message = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();
                tUser = Extra.NameIndex(UserName);


                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                    Declaraciones.PlayerType.SemiDios)) != 0)

                {
                    var argtexto = "/STATS " + UserName;
                    General.LogGM(ref withBlock.name, ref argtexto);

                    if (tUser <= 0)
                    {
                        if (Migration.migr_InStrB(UserName, @"\") != 0) UserName = UserName.Replace(@"\", "");
                        if (Migration.migr_InStrB(UserName, "/") != 0) UserName = UserName.Replace("/", "");

                        for (LoopC = 1; LoopC <= Declaraciones.NUMSKILLS; LoopC++)
                        {
                            var argEmptySpaces = 1024;
                            message = message + "CHAR>" + Declaraciones.SkillsNames[LoopC] + " = " +
                                      ES.GetVar(Declaraciones.CharPath + UserName + ".chr", "SKILLS", "SK" + LoopC,
                                          ref argEmptySpaces) + Constants.vbCrLf;
                        }

                        var argEmptySpaces1 = 1024;
                        WriteConsoleMsg(UserIndex,
                            message + "CHAR> Libres:" + ES.GetVar(Declaraciones.CharPath + UserName + ".chr", "STATS",
                                "SKILLPTSLIBRES", ref argEmptySpaces1), FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        UsUaRiOs.SendUserSkillsTxt(UserIndex, tUser);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleRequestCharSkills: " + ex.Message);
        }
    }

    // '
    // Handles the "ReviveChar" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleReviveChar(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 11/03/2010
        // 11/03/2010: ZaMa - Al revivir con el comando, si esta navegando le da cuerpo e barca.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();


                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                    Declaraciones.PlayerType.SemiDios)) != 0)

                {
                    if (UserName.ToUpper() != "YO")
                        tUser = Extra.NameIndex(UserName);
                    else
                        tUser = UserIndex;

                    if (tUser <= 0)
                    {
                        WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        {
                            ref var withBlock1 = ref Declaraciones.UserList[tUser];
                            // If dead, show him alive (naked).
                            if (withBlock1.flags.Muerto == 1)
                            {
                                withBlock1.flags.Muerto = 0;

                                if (withBlock1.flags.Navegando == 1)
                                    UsUaRiOs.ToogleBoatBody(UserIndex);
                                else
                                    General.DarCuerpoDesnudo(tUser);

                                if (withBlock1.flags.Traveling == 1)
                                {
                                    withBlock1.flags.Traveling = 0;
                                    withBlock1.Counters.goHome = 0;
                                    WriteMultiMessage(tUser, (short)Declaraciones.eMessages.CancelHome);
                                }

                                UsUaRiOs.ChangeUserChar(tUser, withBlock1.character.body, withBlock1.OrigChar.Head,
                                    (byte)withBlock1.character.heading, withBlock1.character.WeaponAnim,
                                    withBlock1.character.ShieldAnim, withBlock1.character.CascoAnim);

                                WriteConsoleMsg(tUser, Declaraciones.UserList[UserIndex].name + " te ha resucitado.",
                                    FontTypeNames.FONTTYPE_INFO);
                            }
                            else
                            {
                                WriteConsoleMsg(tUser, Declaraciones.UserList[UserIndex].name + " te ha curado.",
                                    FontTypeNames.FONTTYPE_INFO);
                            }

                            withBlock1.Stats.MinHp = withBlock1.Stats.MaxHp;

                            if (withBlock1.flags.Traveling == 1)
                            {
                                withBlock1.Counters.goHome = 0;
                                withBlock1.flags.Traveling = 0;
                                WriteMultiMessage(tUser, (short)Declaraciones.eMessages.CancelHome);
                            }
                        }

                        WriteUpdateHP(tUser);

                        FlushBuffer(tUser);

                        var argtexto = "Resucito a " + UserName;
                        General.LogGM(ref withBlock.name, ref argtexto);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleReviveChar: " + ex.Message);
        }
    }

    // '
    // Handles the "OnlineGM" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleOnlineGM(short UserIndex)
    {
        // ***************************************************
        // Author: Fredy Horacio Treboux (liquid)
        // Last Modification: 12/28/06
        // 
        // ***************************************************
        int i;
        var list = default(string);
        Declaraciones.PlayerType priv;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero)) !=
                0)
                return;

            priv = Declaraciones.PlayerType.Consejero | Declaraciones.PlayerType.SemiDios;
            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin)) != 0)
                priv = priv | Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin;

            var loopTo = (int)Declaraciones.LastUser;
            for (i = 1; i <= loopTo; i++)
                if (Declaraciones.UserList[i].flags.UserLogged)
                    if ((Declaraciones.UserList[i].flags.Privilegios & priv) != 0)
                        list = list + Declaraciones.UserList[i].name + ", ";

            if (Migration.migr_LenB(list) != 0)
            {
                list = list.Substring(0, Math.Min(list.Length - 2, list.Length));
                WriteConsoleMsg(UserIndex, list + ".", FontTypeNames.FONTTYPE_INFO);
            }
            else
            {
                WriteConsoleMsg(UserIndex, "No hay GMs Online.", FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Handles the "OnlineMap" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleOnlineMap(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 23/03/2009
        // 23/03/2009: ZaMa - Ahora no requiere estar en el mapa, sino que por defecto se toma en el que esta, pero se puede especificar otro
        // ***************************************************
        short Map;
        int LoopC;
        var list = default(string);
        Declaraciones.PlayerType priv;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            Map = withBlock.incomingData.ReadInteger();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero)) !=
                0)
                return;


            priv = Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                   Declaraciones.PlayerType.SemiDios;
            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin)) != 0)
                priv = priv | Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin;

            var loopTo = (int)Declaraciones.LastUser;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
                if ((Migration.migr_LenB(Declaraciones.UserList[LoopC].name) != 0) &
                    (Declaraciones.UserList[LoopC].Pos.Map == Map))
                    if ((Declaraciones.UserList[LoopC].flags.Privilegios & priv) != 0)
                        list = list + Declaraciones.UserList[LoopC].name + ", ";

            if (list.Length > 2)
                list = list.Substring(0, Math.Min(list.Length - 2, list.Length));

            WriteConsoleMsg(UserIndex, "Usuarios en el mapa: " + list, FontTypeNames.FONTTYPE_INFO);
        }
    }

    // '
    // Handles the "Forgive" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleForgive(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                     Declaraciones.PlayerType.SemiDios)) != 0))
                {
                    tUser = Extra.NameIndex(UserName);

                    if (tUser > 0)
                    {
                        if (Extra.EsNewbie(tUser))
                        {
                            UsUaRiOs.VolverCiudadano(tUser);
                        }
                        else
                        {
                            var argtexto = "Intento perdonar un personaje de nivel avanzado.";
                            General.LogGM(ref withBlock.name, ref argtexto);
                            WriteConsoleMsg(UserIndex, "Sólo se permite perdonar newbies.",
                                FontTypeNames.FONTTYPE_INFO);
                        }
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleForgive: " + ex.Message);
        }
    }

    // '
    // Handles the "Kick" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleKick(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            short rank;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                rank = Convert.ToInt16((int)(Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                             Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.Consejero));

                UserName = buffer.ReadASCIIString();

                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                    Declaraciones.PlayerType.SemiDios)) != 0)

                {
                    tUser = Extra.NameIndex(UserName);

                    if (tUser <= 0)
                    {
                        WriteConsoleMsg(UserIndex, "El usuario no está online.", FontTypeNames.FONTTYPE_INFO);
                    }
                    else if (((int)Declaraciones.UserList[tUser].flags.Privilegios & rank) >
                             ((int)withBlock.flags.Privilegios & rank))
                    {
                        WriteConsoleMsg(UserIndex, "No puedes echar a alguien con jerarquía mayor a la tuya.",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                            PrepareMessageConsoleMsg(withBlock.name + " echó a " + UserName + ".",
                                FontTypeNames.FONTTYPE_INFO));
                        TCP.CloseSocket(tUser);
                        var argtexto = "Echó a " + UserName;
                        General.LogGM(ref withBlock.name, ref argtexto);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleKick: " + ex.Message);
        }
    }

    // '
    // Handles the "Execute" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleExecute(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                     Declaraciones.PlayerType.SemiDios)) != 0))
                {
                    tUser = Extra.NameIndex(UserName);

                    if (tUser > 0)
                    {
                        if ((Declaraciones.UserList[tUser].flags.Privilegios & Declaraciones.PlayerType.User) == 0)
                        {
                            WriteConsoleMsg(UserIndex, "¿¿Estás loco?? ¿¿Cómo vas a piñatear un gm?? :@",
                                FontTypeNames.FONTTYPE_INFO);
                        }
                        else
                        {
                            UsUaRiOs.UserDie(tUser);
                            modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                                PrepareMessageConsoleMsg(withBlock.name + " ha ejecutado a " + UserName + ".",
                                    FontTypeNames.FONTTYPE_EJECUCION));
                            var argtexto = " ejecuto a " + UserName;
                            General.LogGM(ref withBlock.name, ref argtexto);
                        }
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, "No está online.", FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleExecute: " + ex.Message);
        }
    }

    // '
    // Handles the "BanChar" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleBanChar(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            string reason;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();
                reason = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                     Declaraciones.PlayerType.SemiDios)) !=
                     0)) Admin.BanCharacter(UserIndex, UserName, reason);

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleBanChar: " + ex.Message);
        }
    }

    // '
    // Handles the "UnbanChar" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleUnbanChar(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            byte cantPenas;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                     Declaraciones.PlayerType.SemiDios)) != 0))
                {
                    if (Migration.migr_InStrB(UserName, @"\") != 0) UserName = UserName.Replace(@"\", "");
                    if (Migration.migr_InStrB(UserName, "/") != 0) UserName = UserName.Replace("/", "");

                    if (!File.Exists(Declaraciones.CharPath + UserName + ".chr"))
                    {
                        WriteConsoleMsg(UserIndex, "Charfile inexistente (no use +).", FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        var argEmptySpaces1 = 1024;
                        if (Migration.ParseVal(ES.GetVar(Declaraciones.CharPath + UserName + ".chr", "FLAGS", "Ban",
                                ref argEmptySpaces1)) == 1d)
                        {
                            Admin.UnBan(UserName);

                            // penas
                            var argEmptySpaces = 1024;
                            cantPenas = Convert.ToByte(Migration.ParseVal(ES.GetVar(
                                Declaraciones.CharPath + UserName + ".chr", "PENAS", "Cant", ref argEmptySpaces)));
                            ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "PENAS", "Cant",
                                (cantPenas + 1).ToString());
                            ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "PENAS", "P" + (cantPenas + 1),
                                withBlock.name.ToLower() + ": UNBAN. " + Conversions.ToString(DateTime.Today) + " " +
                                Conversions.ToString(DateAndTime.TimeOfDay));

                            var argtexto = "/UNBAN a " + UserName;
                            General.LogGM(ref withBlock.name, ref argtexto);
                            WriteConsoleMsg(UserIndex, UserName + " unbanned.", FontTypeNames.FONTTYPE_INFO);
                        }
                        else
                        {
                            WriteConsoleMsg(UserIndex, UserName + " no está baneado. Imposible unbanear.",
                                FontTypeNames.FONTTYPE_INFO);
                        }
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleUnbanChar: " + ex.Message);
        }
    }

    // '
    // Handles the "NPCFollow" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleNPCFollow(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero)) !=
                0)
                return;

            if (withBlock.flags.TargetNPC > 0)
            {
                NPCs.DoFollow(withBlock.flags.TargetNPC, withBlock.name);
                Declaraciones.Npclist[withBlock.flags.TargetNPC].flags.Inmovilizado = 0;
                Declaraciones.Npclist[withBlock.flags.TargetNPC].flags.Paralizado = 0;
                Declaraciones.Npclist[withBlock.flags.TargetNPC].Contadores.Paralisis = 0;
            }
        }
    }

    // '
    // Handles the "SummonChar" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleSummonChar(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 26/03/2009
        // 26/03/2009: ZaMa - Chequeo que no se teletransporte donde haya un char o npc
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            short X;
            short Y;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                    Declaraciones.PlayerType.SemiDios)) != 0)

                {
                    tUser = Extra.NameIndex(UserName);

                    if (tUser <= 0)
                    {
                        WriteConsoleMsg(UserIndex, "El jugador no está online.", FontTypeNames.FONTTYPE_INFO);
                    }
                    else if (((withBlock.flags.Privilegios &
                               (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin)) != 0) |
                             ((Declaraciones.UserList[tUser].flags.Privilegios &
                               (Declaraciones.PlayerType.Consejero | Declaraciones.PlayerType.User)) != 0))
                    {
                        WriteConsoleMsg(tUser, withBlock.name + " te ha trasportado.", FontTypeNames.FONTTYPE_INFO);
                        X = withBlock.Pos.X;
                        Y = Convert.ToInt16(withBlock.Pos.Y + 1);
                        Extra.FindLegalPos(tUser, withBlock.Pos.Map, ref X, ref Y);
                        UsUaRiOs.WarpUserChar(tUser, withBlock.Pos.Map, X, Y, true, true);
                        var argtexto = "/SUM " + UserName + " Map:" + withBlock.Pos.Map + " X:" + withBlock.Pos.X +
                                       " Y:" + withBlock.Pos.Y;
                        General.LogGM(ref withBlock.name, ref argtexto);
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, "No puedes invocar a dioses y admins.", FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleSummonChar: " + ex.Message);
        }
    }

    // '
    // Handles the "SpawnListRequest" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleSpawnListRequest(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero)) !=
                0)
                return;

            General.EnviarSpawnList(UserIndex);
        }
    }

    // '
    // Handles the "SpawnCreature" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleSpawnCreature(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        short npc;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            npc = withBlock.incomingData.ReadInteger();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                Declaraciones.PlayerType.SemiDios)) != 0)

            {
                if ((npc > 0) & (npc <= Declaraciones.SpawnList.Length - 1))
                    NPCs.SpawnNpc(Declaraciones.SpawnList[npc].NpcIndex, ref withBlock.Pos, true, false);

                var argtexto = "Sumoneo " + Declaraciones.SpawnList[npc].NpcName;
                General.LogGM(ref withBlock.name, ref argtexto);
            }
        }
    }

    // '
    // Handles the "ResetNPCInventory" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleResetNPCInventory(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;
            if (withBlock.flags.TargetNPC == 0)
                return;

            InvNpc.ResetNpcInv(withBlock.flags.TargetNPC);
            var argtexto = "/RESETINV " + Declaraciones.Npclist[withBlock.flags.TargetNPC].name;
            General.LogGM(ref withBlock.name, ref argtexto);
        }
    }

    // '
    // Handles the "CleanWorld" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCleanWorld(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            General.LimpiarMundo();
        }
    }

    // '
    // Handles the "ServerMessage" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleServerMessage(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string message;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                message = buffer.ReadASCIIString();

                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                    Declaraciones.PlayerType.SemiDios)) != 0)

                    if (Migration.migr_LenB(message) != 0)
                    {
                        var argtexto = "Mensaje Broadcast:" + message;
                        General.LogGM(ref withBlock.name, ref argtexto);
                        modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                            PrepareMessageConsoleMsg(Declaraciones.UserList[UserIndex].name + "> " + message,
                                FontTypeNames.FONTTYPE_TALK));
                        // '''''''''''''''SOLO PARA EL TESTEO'''''''
                        // '''''''''SE USA PARA COMUNICARSE CON EL SERVER'''''''''''
                    }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleServerMessage: " + ex.Message);
        }
    }

    // '
    // Handles the "NickToIP" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleNickToIP(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 24/07/07
        // Pablo (ToxicWaste): Agrego para uqe el /nick2ip tambien diga los nicks en esa ip por pedido de la DGM.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            Declaraciones.PlayerType priv;
            string ip;
            var lista = default(string);
            int LoopC;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                     Declaraciones.PlayerType.SemiDios)) != 0))
                {
                    tUser = Extra.NameIndex(UserName);
                    var argtexto = "NICK2IP Solicito la IP de " + UserName;
                    General.LogGM(ref withBlock.name, ref argtexto);

                    if ((withBlock.flags.Privilegios &
                         (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin)) != 0)
                        priv = Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                               Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.Dios |
                               Declaraciones.PlayerType.Admin;
                    else
                        priv = Declaraciones.PlayerType.User;

                    if (tUser > 0)
                    {
                        if ((Declaraciones.UserList[tUser].flags.Privilegios & priv) != 0)
                        {
                            WriteConsoleMsg(UserIndex,
                                "El ip de " + UserName + " es " + Declaraciones.UserList[tUser].ip,
                                FontTypeNames.FONTTYPE_INFO);
                            ip = Declaraciones.UserList[tUser].ip;
                            var loopTo = (int)Declaraciones.LastUser;
                            for (LoopC = 1; LoopC <= loopTo; LoopC++)
                                if ((Declaraciones.UserList[LoopC].ip ?? "") == (ip ?? ""))
                                    if ((Migration.migr_LenB(Declaraciones.UserList[LoopC].name) != 0) &
                                        Declaraciones.UserList[LoopC].flags.UserLogged)
                                        if ((Declaraciones.UserList[LoopC].flags.Privilegios & priv) != 0)
                                            lista = lista + Declaraciones.UserList[LoopC].name + ", ";

                            if (Migration.migr_LenB(lista) != 0)
                                lista = lista.Substring(0, Math.Min(lista.Length - 2, lista.Length));
                            WriteConsoleMsg(UserIndex, "Los personajes con ip " + ip + " son: " + lista,
                                FontTypeNames.FONTTYPE_INFO);
                        }
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, "No hay ningún personaje con ese nick.",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleNickToIP: " + ex.Message);
        }
    }

    // '
    // Handles the "IPToNick" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleIPToNick(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        string ip;
        int LoopC;
        var lista = default(string);
        Declaraciones.PlayerType priv;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            ip = withBlock.incomingData.ReadByte() + ".";
            ip = ip + withBlock.incomingData.ReadByte() + ".";
            ip = ip + withBlock.incomingData.ReadByte() + ".";
            ip = ip + withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            var argtexto = "IP2NICK Solicito los Nicks de IP " + ip;
            General.LogGM(ref withBlock.name, ref argtexto);

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin)) != 0)
                priv = Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                       Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.Dios |
                       Declaraciones.PlayerType.Admin;
            else
                priv = Declaraciones.PlayerType.User;

            var loopTo = (int)Declaraciones.LastUser;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
                if ((Declaraciones.UserList[LoopC].ip ?? "") == (ip ?? ""))
                    if ((Migration.migr_LenB(Declaraciones.UserList[LoopC].name) != 0) &
                        Declaraciones.UserList[LoopC].flags.UserLogged)
                        if ((Declaraciones.UserList[LoopC].flags.Privilegios & priv) != 0)
                            lista = lista + Declaraciones.UserList[LoopC].name + ", ";

            if (Migration.migr_LenB(lista) != 0)
                lista = lista.Substring(0, Math.Min(lista.Length - 2, lista.Length));
            WriteConsoleMsg(UserIndex, "Los personajes con ip " + ip + " son: " + lista, FontTypeNames.FONTTYPE_INFO);
        }
    }

    // '
    // Handles the "GuildOnlineMembers" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildOnlineMembers(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string GuildName;
            short tGuild;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                GuildName = buffer.ReadASCIIString();

                if (Migration.migr_InStrB(GuildName, "+") != 0) GuildName = GuildName.Replace("+", " ");

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                     Declaraciones.PlayerType.SemiDios)) != 0))
                {
                    tGuild = modGuilds.GuildIndex(ref GuildName);

                    if (tGuild > 0)
                        WriteConsoleMsg(UserIndex,
                            "Clan " + GuildName.ToUpper() + ": " + modGuilds.m_ListaDeMiembrosOnline(UserIndex, tGuild),
                            FontTypeNames.FONTTYPE_GUILDMSG);
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildOnlineMembers: " + ex.Message);
        }
    }

    // '
    // Handles the "TeleportCreate" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleTeleportCreate(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 22/03/2010
        // 15/11/2009: ZaMa - Ahora se crea un teleport con un radio especificado.
        // 22/03/2010: ZaMa - Harcodeo los teleps y radios en el dat, para evitar mapas bugueados.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 6)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        short mapa;
        byte X;
        byte Y;
        byte Radio;
        Declaraciones.Obj ET;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            mapa = withBlock.incomingData.ReadInteger();
            X = withBlock.incomingData.ReadByte();
            Y = withBlock.incomingData.ReadByte();
            Radio = withBlock.incomingData.ReadByte();

            Radio = Convert.ToByte(SistemaCombate.MinimoInt(Radio, 6));

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;

            var argtexto = "/CT " + mapa + "," + X + "," + Y + "," + Radio;
            General.LogGM(ref withBlock.name, ref argtexto);

            if (!General.MapaValido(mapa) | !Extra.InMapBounds(mapa, X, Y))
                return;

            if (Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y - 1].ObjInfo.ObjIndex > 0)
                return;

            if (Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y - 1].TileExit.Map > 0)
                return;

            if (Declaraciones.MapData[mapa, X, Y].ObjInfo.ObjIndex > 0)
            {
                WriteConsoleMsg(UserIndex, "Hay un objeto en el piso en ese lugar.", FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if (Declaraciones.MapData[mapa, X, Y].TileExit.Map > 0)
            {
                WriteConsoleMsg(UserIndex, "No puedes crear un teleport que apunte a la entrada de otro.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            ET.Amount = 1;
            // Es el numero en el dat. El indice es el comienzo + el radio, todo harcodeado :(.
            ET.ObjIndex = (short)(Declaraciones.TELEP_OBJ_INDEX + Radio);

            {
                ref var withBlock1 = ref Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y - 1];
                withBlock1.TileExit.Map = mapa;
                withBlock1.TileExit.X = X;
                withBlock1.TileExit.Y = Y;
            }

            InvUsuario.MakeObj(ref ET, withBlock.Pos.Map, withBlock.Pos.X, Convert.ToInt16(withBlock.Pos.Y - 1));
        }
    }

    // '
    // Handles the "TeleportDestroy" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleTeleportDestroy(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        short mapa;
        byte X;
        byte Y;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];

            // Remove packet ID
            withBlock.incomingData.ReadByte();

            // /dt
            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;

            mapa = withBlock.flags.TargetMap;
            X = Convert.ToByte(withBlock.flags.TargetX);
            Y = Convert.ToByte(withBlock.flags.TargetY);

            if (!Extra.InMapBounds(mapa, X, Y))
                return;

            {
                ref var withBlock1 = ref Declaraciones.MapData[mapa, X, Y];
                if (withBlock1.ObjInfo.ObjIndex == 0)
                    return;

                if ((Declaraciones.objData[withBlock1.ObjInfo.ObjIndex].OBJType ==
                     Declaraciones.eOBJType.otTeleport) & (withBlock1.TileExit.Map > 0))
                {
                    var argtexto = "/DT: " + mapa + "," + X + "," + Y;
                    General.LogGM(ref Declaraciones.UserList[UserIndex].name, ref argtexto);

                    InvUsuario.EraseObj(withBlock1.ObjInfo.Amount, mapa, X, Y);

                    if (Declaraciones.MapData[withBlock1.TileExit.Map, withBlock1.TileExit.X, withBlock1.TileExit.Y]
                            .ObjInfo.ObjIndex ==
                        651)
                        InvUsuario.EraseObj(1, withBlock1.TileExit.Map, withBlock1.TileExit.X, withBlock1.TileExit.Y);

                    withBlock1.TileExit.Map = 0;
                    withBlock1.TileExit.X = 0;
                    withBlock1.TileExit.Y = 0;
                }
            }
        }
    }

    // '
    // Handles the "RainToggle" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRainToggle(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero)) !=
                0)
                return;

            var argtexto = "/LLUVIA";
            General.LogGM(ref withBlock.name, ref argtexto);
            Admin.Lloviendo = !Admin.Lloviendo;

            modSendData.SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageRainToggle());
        }
    }

    // '
    // Handles the "SetCharDescription" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleSetCharDescription(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            short tUser;
            string desc;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                desc = buffer.ReadASCIIString();

                if (((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin)) !=
                     0) | ((withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0))
                {
                    tUser = withBlock.flags.TargetUser;
                    if (tUser > 0)
                        Declaraciones.UserList[tUser].DescRM = desc;
                    else
                        WriteConsoleMsg(UserIndex, "Haz click sobre un personaje antes.", FontTypeNames.FONTTYPE_INFO);
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleSetCharDescription: " + ex.Message);
        }
    }

    // '
    // Handles the "ForceMIDIToMap" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HanldeForceMIDIToMap(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 4)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte midiID;
        short mapa;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            midiID = withBlock.incomingData.ReadByte();
            mapa = withBlock.incomingData.ReadInteger();

            // Solo dioses, admins y RMS
            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)

            {
                // Si el mapa no fue enviado tomo el actual
                if (!Extra.InMapBounds(mapa, 50, 50)) mapa = withBlock.Pos.Map;

                if (midiID == 0)
                    // Ponemos el default del mapa
                    modSendData.SendData(modSendData.SendTarget.toMap, mapa,
                        PrepareMessagePlayMidi(Convert.ToByte(Declaraciones.mapInfo[withBlock.Pos.Map].Music)));
                else
                    // Ponemos el pedido por el GM
                    modSendData.SendData(modSendData.SendTarget.toMap, mapa, PrepareMessagePlayMidi(midiID));
            }
        }
    }

    // '
    // Handles the "ForceWAVEToMap" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleForceWAVEToMap(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 6)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte waveID;
        short mapa;
        byte X;
        byte Y;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            waveID = withBlock.incomingData.ReadByte();
            mapa = withBlock.incomingData.ReadInteger();
            X = withBlock.incomingData.ReadByte();
            Y = withBlock.incomingData.ReadByte();

            // Solo dioses, admins y RMS
            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)

            {
                // Si el mapa no fue enviado tomo el actual
                if (!Extra.InMapBounds(mapa, X, Y))
                {
                    mapa = withBlock.Pos.Map;
                    X = Convert.ToByte(withBlock.Pos.X);
                    Y = Convert.ToByte(withBlock.Pos.Y);
                }

                // Ponemos el pedido por el GM
                modSendData.SendData(modSendData.SendTarget.toMap, mapa, PrepareMessagePlayWave(waveID, X, Y));
            }
        }
    }

    // '
    // Handles the "RoyalArmyMessage" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRoyalArmyMessage(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string message;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                message = buffer.ReadASCIIString();

                // Solo dioses, admins y RMS
                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin |
                                                    Declaraciones.PlayerType.RoleMaster)) !=
                    0)
                    modSendData.SendData(modSendData.SendTarget.ToRealYRMs, 0,
                        PrepareMessageConsoleMsg("EJÉRCITO REAL> " + message, FontTypeNames.FONTTYPE_TALK));

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleRoyalArmyMessage: " + ex.Message);
        }
    }

    // '
    // Handles the "ChaosLegionMessage" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleChaosLegionMessage(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string message;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                message = buffer.ReadASCIIString();

                // Solo dioses, admins y RMS
                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin |
                                                    Declaraciones.PlayerType.RoleMaster)) !=
                    0)
                    modSendData.SendData(modSendData.SendTarget.ToCaosYRMs, 0,
                        PrepareMessageConsoleMsg("FUERZAS DEL CAOS> " + message, FontTypeNames.FONTTYPE_TALK));

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleChaosLegionMessage: " + ex.Message);
        }
    }

    // '
    // Handles the "CitizenMessage" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCitizenMessage(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string message;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                message = buffer.ReadASCIIString();

                // Solo dioses, admins y RMS
                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin |
                                                    Declaraciones.PlayerType.RoleMaster)) !=
                    0)
                    modSendData.SendData(modSendData.SendTarget.ToCiudadanosYRMs, 0,
                        PrepareMessageConsoleMsg("CIUDADANOS> " + message, FontTypeNames.FONTTYPE_TALK));

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleCitizenMessage: " + ex.Message);
        }
    }

    // '
    // Handles the "CriminalMessage" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCriminalMessage(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string message;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                message = buffer.ReadASCIIString();

                // Solo dioses, admins y RMS
                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin |
                                                    Declaraciones.PlayerType.RoleMaster)) !=
                    0)
                    modSendData.SendData(modSendData.SendTarget.ToCriminalesYRMs, 0,
                        PrepareMessageConsoleMsg("CRIMINALES> " + message, FontTypeNames.FONTTYPE_TALK));

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleCriminalMessage: " + ex.Message);
        }
    }

    // '
    // Handles the "TalkAsNPC" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleTalkAsNPC(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/29/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string message;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                message = buffer.ReadASCIIString();

                // Solo dioses, admins y RMS
                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Dios | Declaraciones.PlayerType.Admin |
                                                    Declaraciones.PlayerType.RoleMaster)) != 0)

                {
                    // Asegurarse haya un NPC seleccionado
                    if (withBlock.flags.TargetNPC > 0)
                        modSendData.SendData(modSendData.SendTarget.ToNPCArea, withBlock.flags.TargetNPC,
                            PrepareMessageChatOverHead(message,
                                Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex,
                                ColorTranslator.ToOle(Color.White)));
                    else
                        WriteConsoleMsg(UserIndex,
                            "Debes seleccionar el NPC por el que quieres hablar antes de usar este comando.",
                            FontTypeNames.FONTTYPE_INFO);
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleTalkAsNPC: " + ex.Message);
        }
    }

    // '
    // Handles the "DestroyAllItemsInArea" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleDestroyAllItemsInArea(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        int X;
        int Y;
        bool bIsExit;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;


            var loopTo = withBlock.Pos.Y + Declaraciones.MinYBorder - 1;
            for (Y = withBlock.Pos.Y - Declaraciones.MinYBorder + 1; Y <= loopTo; Y++)
            {
                var loopTo1 = withBlock.Pos.X + Declaraciones.MinXBorder - 1;
                for (X = withBlock.Pos.X - Declaraciones.MinXBorder + 1; X <= loopTo1; X++)
                    if ((X > 0) & (Y > 0) & (X < 101) & (Y < 101))
                        if (Declaraciones.MapData[withBlock.Pos.Map, X, Y].ObjInfo.ObjIndex > 0)
                        {
                            bIsExit = Declaraciones.MapData[withBlock.Pos.Map, X, Y].TileExit.Map > 0;
                            if (Extra.ItemNoEsDeMapa(Declaraciones.MapData[withBlock.Pos.Map, X, Y].ObjInfo.ObjIndex,
                                    bIsExit))
                                InvUsuario.EraseObj(Declaraciones.MAX_INVENTORY_OBJS, withBlock.Pos.Map,
                                    Convert.ToInt16(X), Convert.ToInt16(Y));
                        }
            }

            var argtexto = "/MASSDEST";
            General.LogGM(ref Declaraciones.UserList[UserIndex].name, ref argtexto);
        }
    }

    // '
    // Handles the "AcceptRoyalCouncilMember" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleAcceptRoyalCouncilMember(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                {
                    tUser = Extra.NameIndex(UserName);
                    if (tUser <= 0)
                    {
                        WriteConsoleMsg(UserIndex, "Usuario offline", FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                            PrepareMessageConsoleMsg(
                                UserName + " fue aceptado en el honorable Consejo Real de Banderbill.",
                                FontTypeNames.FONTTYPE_CONSEJO));
                        {
                            ref var withBlock1 = ref Declaraciones.UserList[tUser];
                            if ((withBlock1.flags.Privilegios & Declaraciones.PlayerType.ChaosCouncil) != 0)
                                withBlock1.flags.Privilegios =
                                    (Declaraciones.PlayerType)((int)withBlock1.flags.Privilegios -
                                                               (int)Declaraciones.PlayerType.ChaosCouncil);
                            if ((withBlock1.flags.Privilegios & Declaraciones.PlayerType.RoyalCouncil) == 0)
                                withBlock1.flags.Privilegios =
                                    (Declaraciones.PlayerType)((int)withBlock1.flags.Privilegios +
                                                               (int)Declaraciones.PlayerType.RoyalCouncil);

                            UsUaRiOs.WarpUserChar(tUser, withBlock1.Pos.Map, withBlock1.Pos.X, withBlock1.Pos.Y, false);
                        }
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleAcceptRoyalCouncilMember: " + ex.Message);
        }
    }

    // '
    // Handles the "ChaosCouncilMember" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleAcceptChaosCouncilMember(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                {
                    tUser = Extra.NameIndex(UserName);
                    if (tUser <= 0)
                    {
                        WriteConsoleMsg(UserIndex, "Usuario offline", FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                            PrepareMessageConsoleMsg(UserName + " fue aceptado en el Concilio de las Sombras.",
                                FontTypeNames.FONTTYPE_CONSEJO));

                        {
                            ref var withBlock1 = ref Declaraciones.UserList[tUser];
                            if ((withBlock1.flags.Privilegios & Declaraciones.PlayerType.RoyalCouncil) != 0)
                                withBlock1.flags.Privilegios =
                                    (Declaraciones.PlayerType)((int)withBlock1.flags.Privilegios -
                                                               (int)Declaraciones.PlayerType.RoyalCouncil);
                            if ((withBlock1.flags.Privilegios & Declaraciones.PlayerType.ChaosCouncil) == 0)
                                withBlock1.flags.Privilegios =
                                    (Declaraciones.PlayerType)((int)withBlock1.flags.Privilegios +
                                                               (int)Declaraciones.PlayerType.ChaosCouncil);

                            UsUaRiOs.WarpUserChar(tUser, withBlock1.Pos.Map, withBlock1.Pos.X, withBlock1.Pos.Y, false);
                        }
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleAcceptChaosCouncilMember: " + ex.Message);
        }
    }

    // '
    // Handles the "ItemsInTheFloor" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleItemsInTheFloor(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        short tObj;
        int X;
        int Y;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;


            for (X = 5; X <= 95; X++)
            for (Y = 5; Y <= 95; Y++)
            {
                tObj = Declaraciones.MapData[withBlock.Pos.Map, X, Y].ObjInfo.ObjIndex;
                if (tObj > 0)
                    if (Declaraciones.objData[tObj].OBJType != Declaraciones.eOBJType.otArboles)
                        WriteConsoleMsg(UserIndex, "(" + X + "," + Y + ") " + Declaraciones.objData[tObj].name,
                            FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Handles the "MakeDumb" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleMakeDumb(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0) | ((withBlock.flags.Privilegios &
                            (Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.RoleMaster)) ==
                           (Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.RoleMaster)))
                {
                    tUser = Extra.NameIndex(UserName);
                    // para deteccion de aoice
                    if (tUser <= 0)
                        WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO);
                    else
                        WriteDumb(tUser);
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleMakeDumb: " + ex.Message);
        }
    }

    // '
    // Handles the "MakeDumbNoMore" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleMakeDumbNoMore(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0) | ((withBlock.flags.Privilegios &
                            (Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.RoleMaster)) ==
                           (Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.RoleMaster)))
                {
                    tUser = Extra.NameIndex(UserName);
                    // para deteccion de aoice
                    if (tUser <= 0)
                    {
                        WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        WriteDumbNoMore(tUser);
                        FlushBuffer(tUser);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleMakeDumbNoMore: " + ex.Message);
        }
    }

    // '
    // Handles the "DumpIPTables" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleDumpIPTables(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;

            SecurityIp.DumpTables();
        }
    }

    // '
    // Handles the "CouncilKick" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCouncilKick(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                {
                    tUser = Extra.NameIndex(UserName);
                    if (tUser <= 0)
                    {
                        if (File.Exists(Declaraciones.CharPath + UserName + ".chr"))
                        {
                            WriteConsoleMsg(UserIndex, "Usuario offline, echando de los consejos.",
                                FontTypeNames.FONTTYPE_INFO);
                            ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "CONSEJO", "PERTENECE",
                                0.ToString());
                            ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "CONSEJO", "PERTENECECAOS",
                                0.ToString());
                        }
                        else
                        {
                            WriteConsoleMsg(UserIndex,
                                "No se encuentra el charfile " + Declaraciones.CharPath + UserName + ".chr",
                                FontTypeNames.FONTTYPE_INFO);
                        }
                    }
                    else
                    {
                        {
                            ref var withBlock1 = ref Declaraciones.UserList[tUser];
                            if ((withBlock1.flags.Privilegios & Declaraciones.PlayerType.RoyalCouncil) != 0)
                            {
                                WriteConsoleMsg(tUser, "Has sido echado del consejo de Banderbill.",
                                    FontTypeNames.FONTTYPE_TALK);
                                withBlock1.flags.Privilegios =
                                    (Declaraciones.PlayerType)((int)withBlock1.flags.Privilegios -
                                                               (int)Declaraciones.PlayerType.RoyalCouncil);

                                UsUaRiOs.WarpUserChar(tUser, withBlock1.Pos.Map, withBlock1.Pos.X, withBlock1.Pos.Y,
                                    false);
                                modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                                    PrepareMessageConsoleMsg(UserName + " fue expulsado del consejo de Banderbill.",
                                        FontTypeNames.FONTTYPE_CONSEJO));
                            }

                            if ((withBlock1.flags.Privilegios & Declaraciones.PlayerType.ChaosCouncil) != 0)
                            {
                                WriteConsoleMsg(tUser, "Has sido echado del Concilio de las Sombras.",
                                    FontTypeNames.FONTTYPE_TALK);
                                withBlock1.flags.Privilegios =
                                    (Declaraciones.PlayerType)((int)withBlock1.flags.Privilegios -
                                                               (int)Declaraciones.PlayerType.ChaosCouncil);

                                UsUaRiOs.WarpUserChar(tUser, withBlock1.Pos.Map, withBlock1.Pos.X, withBlock1.Pos.Y,
                                    false);
                                modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                                    PrepareMessageConsoleMsg(UserName + " fue expulsado del Concilio de las Sombras.",
                                        FontTypeNames.FONTTYPE_CONSEJO));
                            }
                        }
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleCouncilKick: " + ex.Message);
        }
    }

    // '
    // Handles the "SetTrigger" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleSetTrigger(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte tTrigger;
        string tLog;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            tTrigger = withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            if (tTrigger >= 0)
            {
                Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger =
                    (Declaraciones.eTrigger)tTrigger;
                tLog = "Trigger " + tTrigger + " en mapa " + withBlock.Pos.Map + " " + withBlock.Pos.X + "," +
                       withBlock.Pos.Y;

                General.LogGM(ref withBlock.name, ref tLog);
                WriteConsoleMsg(UserIndex, tLog, FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Handles the "AskTrigger" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleAskTrigger(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 04/13/07
        // 
        // ***************************************************
        byte tTrigger;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            tTrigger = Convert.ToByte((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y]
                .trigger);

            var argtexto = "Miro el trigger en " + withBlock.Pos.Map + "," + withBlock.Pos.X + "," + withBlock.Pos.Y +
                           ". Era " + tTrigger;
            General.LogGM(ref withBlock.name, ref argtexto);

            WriteConsoleMsg(UserIndex,
                "Trigger " + tTrigger + " en mapa " + withBlock.Pos.Map + " " + withBlock.Pos.X + ", " +
                withBlock.Pos.Y, FontTypeNames.FONTTYPE_INFO);
        }
    }

    // '
    // Handles the "BannedIPList" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleBannedIPList(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        var lista = default(string);
        int LoopC;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;


            var argtexto = "/BANIPLIST";
            General.LogGM(ref withBlock.name, ref argtexto);

            var loopTo = Declaraciones.BanIps.Count;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto BanIps.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                lista = lista + Declaraciones.BanIps[LoopC] + ", ";

            if (Migration.migr_LenB(lista) != 0)
                lista = lista.Substring(0, Math.Min(lista.Length - 2, lista.Length));

            WriteConsoleMsg(UserIndex, lista, FontTypeNames.FONTTYPE_INFO);
        }
    }

    // '
    // Handles the "BannedIPReload" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleBannedIPReload(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            Admin.BanIpGuardar();
            Admin.BanIpCargar();
        }
    }

    // '
    // Handles the "GuildBan" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleGuildBan(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string GuildName;
            short cantMembers;
            int LoopC;
            string member;
            byte Count;
            short tIndex;
            string tFile;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                GuildName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                {
                    tFile = AppDomain.CurrentDomain.BaseDirectory + "guilds/" + GuildName + "-members.mem";

                    if (!File.Exists(tFile))
                    {
                        WriteConsoleMsg(UserIndex, "No existe el clan: " + GuildName, FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                            PrepareMessageConsoleMsg(withBlock.name + " baneó al clan " + GuildName.ToUpper(),
                                FontTypeNames.FONTTYPE_FIGHT));

                        // baneamos a los miembros
                        var argtexto = "BANCLAN a " + GuildName.ToUpper();
                        General.LogGM(ref withBlock.name, ref argtexto);

                        var argEmptySpaces = 1024;
                        cantMembers =
                            Convert.ToInt16(Migration.ParseVal(ES.GetVar(tFile, "INIT", "NroMembers",
                                ref argEmptySpaces)));

                        var loopTo = (int)cantMembers;
                        for (LoopC = 1; LoopC <= loopTo; LoopC++)
                        {
                            var argEmptySpaces1 = 1024;
                            member = ES.GetVar(tFile, "Members", "Member" + LoopC, ref argEmptySpaces1);
                            // member es la victima
                            ES.Ban(member, "Administracion del servidor", "Clan Banned");

                            modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                                PrepareMessageConsoleMsg(
                                    "   " + member + "<" + GuildName + "> ha sido expulsado del servidor.",
                                    FontTypeNames.FONTTYPE_FIGHT));

                            tIndex = Extra.NameIndex(member);
                            if (tIndex > 0)
                            {
                                // esta online
                                Declaraciones.UserList[tIndex].flags.Ban = 1;
                                TCP.CloseSocket(tIndex);
                            }

                            // ponemos el flag de ban a 1
                            ES.WriteVar(Declaraciones.CharPath + member + ".chr", "FLAGS", "Ban", "1");
                            // ponemos la pena
                            var argEmptySpaces2 = 1024;
                            Count = Convert.ToByte(Migration.ParseVal(ES.GetVar(
                                Declaraciones.CharPath + member + ".chr", "PENAS", "Cant", ref argEmptySpaces2)));
                            ES.WriteVar(Declaraciones.CharPath + member + ".chr", "PENAS", "Cant",
                                (Count + 1).ToString());
                            ES.WriteVar(Declaraciones.CharPath + member + ".chr", "PENAS", "P" + (Count + 1),
                                withBlock.name.ToLower() + ": BAN AL CLAN: " + GuildName + " " +
                                Conversions.ToString(DateTime.Today) + " " +
                                Conversions.ToString(DateAndTime.TimeOfDay));
                        }
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleGuildBan: " + ex.Message);
        }
    }

    // '
    // Handles the "BanIP" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleBanIP(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 07/02/09
        // Agregado un CopyBuffer porque se producia un bucle
        // inifito al intentar banear una ip ya baneada. (NicoNZ)
        // 07/02/09 Pato - Ahora no es posible saber si un gm está o no online.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 6)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            var bannedIP = default(string);
            var tUser = default(short);
            string reason;
            int i;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                // Is it by ip??
                if (buffer.ReadBoolean())
                {
                    bannedIP = buffer.ReadByte() + ".";
                    bannedIP = bannedIP + buffer.ReadByte() + ".";
                    bannedIP = bannedIP + buffer.ReadByte() + ".";
                    bannedIP = bannedIP + buffer.ReadByte();
                }
                else
                {
                    tUser = Extra.NameIndex(buffer.ReadASCIIString());

                    if (tUser > 0)
                        bannedIP = Declaraciones.UserList[tUser].ip;
                }

                reason = buffer.ReadASCIIString();


                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                    0)
                {
                    if (Migration.migr_LenB(bannedIP) > 0)
                    {
                        var argtexto = "/BanIP " + bannedIP + " por " + reason;
                        General.LogGM(ref withBlock.name, ref argtexto);

                        if (Admin.BanIpBuscar(bannedIP) > 0)
                        {
                            WriteConsoleMsg(UserIndex, "La IP " + bannedIP + " ya se encuentra en la lista de bans.",
                                FontTypeNames.FONTTYPE_INFO);
                        }
                        else
                        {
                            Admin.BanIpAgrega(bannedIP);
                            modSendData.SendData(modSendData.SendTarget.ToAdmins, 0,
                                PrepareMessageConsoleMsg(withBlock.name + " baneó la IP " + bannedIP + " por " + reason,
                                    FontTypeNames.FONTTYPE_FIGHT));

                            // Find every player with that ip and ban him!
                            var loopTo = (int)Declaraciones.LastUser;
                            for (i = 1; i <= loopTo; i++)
                                if (Declaraciones.UserList[i].ConnIDValida)
                                    if ((Declaraciones.UserList[i].ip ?? "") == (bannedIP ?? ""))
                                        Admin.BanCharacter(UserIndex, Declaraciones.UserList[i].name,
                                            "IP POR " + reason);
                        }
                    }
                    else if (tUser <= 0)
                    {
                        WriteConsoleMsg(UserIndex, "El personaje no está online.", FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleBanIP: " + ex.Message);
        }
    }

    // '
    // Handles the "UnbanIP" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleUnbanIP(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        string bannedIP;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            bannedIP = withBlock.incomingData.ReadByte() + ".";
            bannedIP = bannedIP + withBlock.incomingData.ReadByte() + ".";
            bannedIP = bannedIP + withBlock.incomingData.ReadByte() + ".";
            bannedIP = bannedIP + withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            if (Admin.BanIpQuita(bannedIP))
                WriteConsoleMsg(UserIndex, "La IP \"" + bannedIP + "\" se ha quitado de la lista de bans.",
                    FontTypeNames.FONTTYPE_INFO);
            else
                WriteConsoleMsg(UserIndex, "La IP \"" + bannedIP + "\" NO se encuentra en la lista de bans.",
                    FontTypeNames.FONTTYPE_INFO);
        }
    }

    // '
    // Handles the "CreateItem" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleCreateItem(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        short tObj;
        Declaraciones.Obj Objeto;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            tObj = withBlock.incomingData.ReadInteger();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;

            var argtexto = "/CI: " + tObj;
            General.LogGM(ref withBlock.name, ref argtexto);

            if (Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y - 1].ObjInfo.ObjIndex > 0)
                return;

            if (Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y - 1].TileExit.Map > 0)
                return;

            if ((tObj < 1) | (tObj > Declaraciones.NumObjDatas))
                return;

            // Is the object not null?
            if (Migration.migr_LenB(Declaraciones.objData[tObj].name) == 0)
                return;

            WriteConsoleMsg(UserIndex, "¡¡ATENCIÓN: FUERON CREADOS ***100*** ÍTEMS, TIRE Y /DEST LOS QUE NO NECESITE!!",
                FontTypeNames.FONTTYPE_GUILD);

            Objeto.Amount = 100;
            Objeto.ObjIndex = tObj;
            InvUsuario.MakeObj(ref Objeto, withBlock.Pos.Map, withBlock.Pos.X, Convert.ToInt16(withBlock.Pos.Y - 1));
        }
    }

    // '
    // Handles the "DestroyItems" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleDestroyItems(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;

            if (Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].ObjInfo.ObjIndex == 0)
                return;

            var argtexto = "/DEST";
            General.LogGM(ref withBlock.name, ref argtexto);

            if ((Declaraciones
                    .objData
                        [Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].ObjInfo.ObjIndex]
                    .OBJType == Declaraciones.eOBJType.otTeleport) & (Declaraciones
                    .MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].TileExit.Map > 0))
            {
                WriteConsoleMsg(UserIndex, "No puede destruir teleports así. Utilice /DT.",
                    FontTypeNames.FONTTYPE_INFO);
                return;
            }

            InvUsuario.EraseObj(10000, withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y);
        }
    }

    // '
    // Handles the "ChaosLegionKick" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleChaosLegionKick(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                {
                    if (Migration.migr_InStrB(UserName, @"\") != 0) UserName = UserName.Replace(@"\", "");
                    if (Migration.migr_InStrB(UserName, "/") != 0) UserName = UserName.Replace("/", "");
                    tUser = Extra.NameIndex(UserName);

                    var argtexto = "ECHO DEL CAOS A: " + UserName;
                    General.LogGM(ref withBlock.name, ref argtexto);

                    if (tUser > 0)
                    {
                        var argExpulsado = true;
                        ModFacciones.ExpulsarFaccionCaos(tUser, ref argExpulsado);
                        Declaraciones.UserList[tUser].Faccion.Reenlistadas = 200;
                        WriteConsoleMsg(UserIndex,
                            UserName + " expulsado de las fuerzas del caos y prohibida la reenlistada.",
                            FontTypeNames.FONTTYPE_INFO);
                        WriteConsoleMsg(tUser,
                            withBlock.name + " te ha expulsado en forma definitiva de las fuerzas del caos.",
                            FontTypeNames.FONTTYPE_FIGHT);
                        FlushBuffer(tUser);
                    }
                    else if (File.Exists(Declaraciones.CharPath + UserName + ".chr"))
                    {
                        ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "FACCIONES", "EjercitoCaos",
                            0.ToString());
                        ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "FACCIONES", "Reenlistadas",
                            200.ToString());
                        ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "FACCIONES", "Extra",
                            "Expulsado por " + withBlock.name);
                        WriteConsoleMsg(UserIndex,
                            UserName + " expulsado de las fuerzas del caos y prohibida la reenlistada.",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, UserName + ".chr inexistente.", FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleChaosLegionKick: " + ex.Message);
        }
    }

    // '
    // Handles the "RoyalArmyKick" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRoyalArmyKick(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                {
                    if (Migration.migr_InStrB(UserName, @"\") != 0) UserName = UserName.Replace(@"\", "");
                    if (Migration.migr_InStrB(UserName, "/") != 0) UserName = UserName.Replace("/", "");
                    tUser = Extra.NameIndex(UserName);

                    var argtexto = "ECHÓ DE LA REAL A: " + UserName;
                    General.LogGM(ref withBlock.name, ref argtexto);

                    if (tUser > 0)
                    {
                        var argExpulsado = true;
                        ModFacciones.ExpulsarFaccionReal(tUser, ref argExpulsado);
                        Declaraciones.UserList[tUser].Faccion.Reenlistadas = 200;
                        WriteConsoleMsg(UserIndex,
                            UserName + " expulsado de las fuerzas reales y prohibida la reenlistada.",
                            FontTypeNames.FONTTYPE_INFO);
                        WriteConsoleMsg(tUser,
                            withBlock.name + " te ha expulsado en forma definitiva de las fuerzas reales.",
                            FontTypeNames.FONTTYPE_FIGHT);
                        FlushBuffer(tUser);
                    }
                    else if (File.Exists(Declaraciones.CharPath + UserName + ".chr"))
                    {
                        ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "FACCIONES", "EjercitoReal",
                            0.ToString());
                        ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "FACCIONES", "Reenlistadas",
                            200.ToString());
                        ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "FACCIONES", "Extra",
                            "Expulsado por " + withBlock.name);
                        WriteConsoleMsg(UserIndex,
                            UserName + " expulsado de las fuerzas reales y prohibida la reenlistada.",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, UserName + ".chr inexistente.", FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleRoyalArmyKick: " + ex.Message);
        }
    }

    // '
    // Handles the "ForceMIDIAll" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleForceMIDIAll(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte midiID;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            midiID = withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;

            modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                PrepareMessageConsoleMsg(withBlock.name + " broadcast música: " + midiID,
                    FontTypeNames.FONTTYPE_SERVER));

            modSendData.SendData(modSendData.SendTarget.ToAll, 0, PrepareMessagePlayMidi(midiID));
        }
    }

    // '
    // Handles the "ForceWAVEAll" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleForceWAVEAll(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte waveID;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            waveID = withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;

            modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                PrepareMessagePlayWave(waveID, Declaraciones.NO_3D_SOUND, Declaraciones.NO_3D_SOUND));
        }
    }

    // '
    // Handles the "RemovePunishment" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleRemovePunishment(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 1/05/07
        // Pablo (ToxicWaste): 1/05/07, You can now edit the punishment.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 6)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            byte punishment;
            string NewText;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();
                punishment = buffer.ReadByte();
                NewText = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                {
                    if (Migration.migr_LenB(UserName) == 0)
                    {
                        WriteConsoleMsg(UserIndex, "Utilice /borrarpena Nick@NumeroDePena@NuevaPena",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        if (Migration.migr_InStrB(UserName, @"\") != 0) UserName = UserName.Replace(@"\", "");
                        if (Migration.migr_InStrB(UserName, "/") != 0) UserName = UserName.Replace("/", "");

                        if (File.Exists(Declaraciones.CharPath + UserName + ".chr"))
                        {
                            var argEmptySpaces = 1024;
                            var argtexto = " borro la pena: " + punishment + "-" +
                                           ES.GetVar(Declaraciones.CharPath + UserName + ".chr", "PENAS",
                                               "P" + punishment, ref argEmptySpaces) + " de " + UserName +
                                           " y la cambió por: " + NewText;
                            General.LogGM(ref withBlock.name, ref argtexto);

                            ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "PENAS", "P" + punishment,
                                withBlock.name.ToLower() + ": <" + NewText + "> " +
                                Conversions.ToString(DateTime.Today) + " " +
                                Conversions.ToString(DateAndTime.TimeOfDay));

                            WriteConsoleMsg(UserIndex, "Pena modificada.", FontTypeNames.FONTTYPE_INFO);
                        }
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleRemovePunishment: " + ex.Message);
        }
    }

    // '
    // Handles the "TileBlockedToggle" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleTileBlockedToggle(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;

            var argtexto = "/BLOQ";
            General.LogGM(ref withBlock.name, ref argtexto);

            if (Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].Blocked == 0)
                Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].Blocked = 1;
            else
                Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].Blocked = 0;

            General.Bloquear(true, withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y,
                Convert.ToBoolean(Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].Blocked));
        }
    }

    // '
    // Handles the "KillNPCNoRespawn" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleKillNPCNoRespawn(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;

            if (withBlock.flags.TargetNPC == 0)
                return;

            NPCs.QuitarNPC(withBlock.flags.TargetNPC);
            var argtexto = "/MATA " + Declaraciones.Npclist[withBlock.flags.TargetNPC].name;
            General.LogGM(ref withBlock.name, ref argtexto);
        }
    }

    // '
    // Handles the "KillAllNearbyNPCs" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleKillAllNearbyNPCs(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        int X;
        int Y;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;


            var loopTo = withBlock.Pos.Y + Declaraciones.MinYBorder - 1;
            for (Y = withBlock.Pos.Y - Declaraciones.MinYBorder + 1; Y <= loopTo; Y++)
            {
                var loopTo1 = withBlock.Pos.X + Declaraciones.MinXBorder - 1;
                for (X = withBlock.Pos.X - Declaraciones.MinXBorder + 1; X <= loopTo1; X++)
                    if ((X > 0) & (Y > 0) & (X < 101) & (Y < 101))
                        if (Declaraciones.MapData[withBlock.Pos.Map, X, Y].NpcIndex > 0)
                            NPCs.QuitarNPC(Declaraciones.MapData[withBlock.Pos.Map, X, Y].NpcIndex);
            }

            var argtexto = "/MASSKILL";
            General.LogGM(ref withBlock.name, ref argtexto);
        }
    }

    // '
    // Handles the "LastIP" message.
    // 
    // @param    userIndex The index of the user sending the message.

    private static void HandleLastIP(short UserIndex)
    {
        // ***************************************************
        // Author: Nicolas Matias Gonzalez (NIGO)
        // Last Modification: 12/30/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            string lista;
            byte LoopC;
            short priv;
            bool validCheck;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                priv = Convert.ToInt16((int)(Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                             Declaraciones.PlayerType.SemiDios | Declaraciones.PlayerType.Consejero));
                UserName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                     Declaraciones.PlayerType.SemiDios)) != 0))
                {
                    // Handle special chars
                    if (Migration.migr_InStrB(UserName, @"\") != 0) UserName = UserName.Replace(@"\", "");
                    if (Migration.migr_InStrB(UserName, @"\") != 0) UserName = UserName.Replace("/", "");
                    if (Migration.migr_InStrB(UserName, "+") != 0) UserName = UserName.Replace("+", " ");

                    // Only Gods and Admins can see the ips of adminsitrative characters. All others can be seen by every adminsitrative char.
                    if (Extra.NameIndex(UserName) > 0)
                        validCheck =
                            (((int)Declaraciones.UserList[Extra.NameIndex(UserName)].flags.Privilegios & priv) == 0) |
                            ((withBlock.flags.Privilegios &
                              (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) != 0);
                    else
                        validCheck = (((int)Admin.UserDarPrivilegioLevel(UserName) & priv) == 0) |
                                     ((withBlock.flags.Privilegios &
                                       (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) != 0);

                    if (validCheck)
                    {
                        var argtexto = "/LASTIP " + UserName;
                        General.LogGM(ref withBlock.name, ref argtexto);

                        if (File.Exists(Declaraciones.CharPath + UserName + ".chr"))
                        {
                            lista = "Las ultimas IPs con las que " + UserName + " se conectó son:";
                            for (LoopC = 1; LoopC <= 5; LoopC++)
                            {
                                var argEmptySpaces = 1024;
                                lista = lista + Constants.vbCrLf + LoopC + " - " + ES.GetVar(
                                    Declaraciones.CharPath + UserName + ".chr", "INIT", "LastIP" + LoopC,
                                    ref argEmptySpaces);
                            }

                            WriteConsoleMsg(UserIndex, lista, FontTypeNames.FONTTYPE_INFO);
                        }
                        else
                        {
                            WriteConsoleMsg(UserIndex, "Charfile \"" + UserName + "\" inexistente.",
                                FontTypeNames.FONTTYPE_INFO);
                        }
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, UserName + " es de mayor jerarquía que vos.",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleLastIP: " + ex.Message);
        }
    }

    // '
    // Handles the "ChatColor" message.
    // 
    // @param    userIndex The index of the user sending the message.

    public static void HandleChatColor(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/23/06
        // Last modified by: Juan Martín Sotuyo Dodero (Maraxus)
        // Change the user`s chat color
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 4)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        int color;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();


            color = Information.RGB(withBlock.incomingData.ReadByte(), withBlock.incomingData.ReadByte(),
                withBlock.incomingData.ReadByte());

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                Declaraciones.PlayerType.RoleMaster)) !=
                0) withBlock.flags.ChatColor = color;
        }
    }

    // '
    // Handles the "Ignored" message.
    // 
    // @param    userIndex The index of the user sending the message.

    public static void HandleIgnored(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/23/06
        // Ignore the user
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.Consejero)) !=
                0) withBlock.flags.AdminPerseguible = !withBlock.flags.AdminPerseguible;
        }
    }

    // '
    // Handles the "CheckSlot" message.
    // 
    // @param    userIndex The index of the user sending the message.

    public static void HandleCheckSlot(short UserIndex)
    {
        // ***************************************************
        // Author: Pablo (ToxicWaste)
        // Last Modification: 09/09/2008 (NicoNZ)
        // Check one Users Slot in Particular from Inventory
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 4)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            byte Slot;
            short tIndex;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                // Reads the UserName and Slot Packets

                UserName = buffer.ReadASCIIString(); // Que UserName?
                Slot = buffer.ReadByte(); // Que Slot?

                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.SemiDios |
                                                    Declaraciones.PlayerType.Dios)) != 0)

                {
                    tIndex = Extra.NameIndex(UserName); // Que user index?

                    var argtexto = withBlock.name + " Checkeó el slot " + Slot + " de " + UserName;
                    General.LogGM(ref withBlock.name, ref argtexto);

                    if (tIndex > 0)
                    {
                        if ((Slot > 0) & (Slot <= Declaraciones.UserList[tIndex].CurrentInventorySlots))
                        {
                            if (Declaraciones.UserList[tIndex].Invent.userObj[Slot].ObjIndex > 0)
                                WriteConsoleMsg(UserIndex,
                                    " Objeto " + Slot + ") " +
                                    Declaraciones
                                        .objData[
                                            Declaraciones.UserList[tIndex].Invent.userObj[Slot].ObjIndex].name +
                                    " Cantidad:" + Declaraciones.UserList[tIndex].Invent.userObj[Slot].Amount,
                                    FontTypeNames.FONTTYPE_INFO);
                            else
                                WriteConsoleMsg(UserIndex, "No hay ningún objeto en slot seleccionado.",
                                    FontTypeNames.FONTTYPE_INFO);
                        }
                        else
                        {
                            WriteConsoleMsg(UserIndex, "Slot Inválido.", FontTypeNames.FONTTYPE_TALK);
                        }
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, "Usuario offline.", FontTypeNames.FONTTYPE_TALK);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleCheckSlot: " + ex.Message);
        }
    }

    // '
    // Handles the "ResetAutoUpdate" message.
    // 
    // @param    userIndex The index of the user sending the message.

    public static void HandleResetAutoUpdate(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/23/06
        // Reset the AutoUpdate
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;
            if (withBlock.name.ToUpper() != "MARAXUS")
                return;

            WriteConsoleMsg(UserIndex, "TID: " + General.ReiniciarAutoUpdate(), FontTypeNames.FONTTYPE_INFO);
        }
    }

    // '
    // Handles the "Restart" message.
    // 
    // @param    userIndex The index of the user sending the message.

    public static void HandleRestart(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/23/06
        // Restart the game
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;
            if (withBlock.name.ToUpper() != "MARAXUS")
                return;

            // time and Time BUG!
            var argtexto = withBlock.name + " reinició el mundo.";
            General.LogGM(ref withBlock.name, ref argtexto);

            General.ReiniciarServidor();
        }
    }

    // '
    // Handles the "ReloadObjects" message.
    // 
    // @param    userIndex The index of the user sending the message.

    public static void HandleReloadObjects(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/23/06
        // Reload the objects
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            var argtexto = withBlock.name + " ha recargado los objetos.";
            General.LogGM(ref withBlock.name, ref argtexto);

            ES.LoadOBJData();
        }
    }

    // '
    // Handles the "ReloadSpells" message.
    // 
    // @param    userIndex The index of the user sending the message.

    public static void HandleReloadSpells(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/23/06
        // Reload the spells
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            var argtexto = withBlock.name + " ha recargado los hechizos.";
            General.LogGM(ref withBlock.name, ref argtexto);

            ES.CargarHechizos();
        }
    }

    // '
    // Handle the "ReloadServerIni" message.
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleReloadServerIni(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/23/06
        // Reload the Server`s INI
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            var argtexto = withBlock.name + " ha recargado los INITs.";
            General.LogGM(ref withBlock.name, ref argtexto);

            ES.LoadSini();
        }
    }

    // '
    // Handle the "ReloadNPCs" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleReloadNPCs(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/23/06
        // Reload the Server`s NPC
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            var argtexto = withBlock.name + " ha recargado los NPCs.";
            General.LogGM(ref withBlock.name, ref argtexto);

            General.CargaNpcsDat();

            WriteConsoleMsg(UserIndex, "Npcs.dat recargado.", FontTypeNames.FONTTYPE_INFO);
        }
    }

    // '
    // Handle the "KickAllChars" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleKickAllChars(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/23/06
        // Kick all the chars that are online
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            var argtexto = withBlock.name + " ha echado a todos los personajes.";
            General.LogGM(ref withBlock.name, ref argtexto);

            TCP.EcharPjsNoPrivilegiados();
        }
    }

    // '
    // Handle the "Night" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleNight(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/23/06
        // Last modified by: Juan Martín Sotuyo Dodero (Maraxus)
        // 
        // ***************************************************
        int i;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;
            if (withBlock.name.ToUpper() != "MARAXUS")
                return;

            Admin.DeNoche = !Admin.DeNoche;


            var loopTo = (int)Declaraciones.NumUsers;
            for (i = 1; i <= loopTo; i++)
                if (Declaraciones.UserList[i].flags.UserLogged & (Declaraciones.UserList[i].ConnID > -1))
                    TCP.EnviarNoche(Convert.ToInt16(i));
        }
    }

    // '
    // Handle the "ShowServerForm" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleShowServerForm(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/23/06
        // Show the server form
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            var argtexto = withBlock.name + " ha solicitado mostrar el formulario del servidor.";
            General.LogGM(ref withBlock.name, ref argtexto);
            // TODO FIX: no funciona como se espera, de todas formas no es algo funcional
        }
    }

    // '
    // Handle the "CleanSOS" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleCleanSOS(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/23/06
        // Clean the SOS
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            var argtexto = withBlock.name + " ha borrado los SOS.";
            General.LogGM(ref withBlock.name, ref argtexto);

            Declaraciones.Ayuda.Reset();
        }
    }

    // '
    // Handle the "SaveChars" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleSaveChars(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/23/06
        // Save the characters
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            var argtexto = withBlock.name + " ha guardado todos los chars.";
            General.LogGM(ref withBlock.name, ref argtexto);

            mdParty.ActualizaExperiencias();
            General.GuardarUsuarios();
        }
    }

    // '
    // Handle the "ChangeMapInfoBackup" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleChangeMapInfoBackup(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/24/06
        // Last modified by: Juan Martín Sotuyo Dodero (Maraxus)
        // Change the backup`s info of the map
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        bool doTheBackUp;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();


            doTheBackUp = withBlock.incomingData.ReadBoolean();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) == 0)
                return;

            var argtexto = withBlock.name + " ha cambiado la información sobre el BackUp.";
            General.LogGM(ref withBlock.name, ref argtexto);

            // Change the boolean to byte in a fast way
            if (doTheBackUp)
                Declaraciones.mapInfo[withBlock.Pos.Map].BackUp = 1;
            else
                Declaraciones.mapInfo[withBlock.Pos.Map].BackUp = 0;

            // Change the boolean to string in a fast way
            ES.WriteVar(
                AppDomain.CurrentDomain.BaseDirectory + Declaraciones.MapPath + "mapa" + withBlock.Pos.Map + ".dat",
                "Mapa" + withBlock.Pos.Map, "backup",
                Declaraciones.mapInfo[withBlock.Pos.Map].BackUp.ToString());

            WriteConsoleMsg(UserIndex,
                "Mapa " + withBlock.Pos.Map + " Backup: " + Declaraciones.mapInfo[withBlock.Pos.Map].BackUp,
                FontTypeNames.FONTTYPE_INFO);
        }
    }

    // '
    // Handle the "ChangeMapInfoPK" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleChangeMapInfoPK(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/24/06
        // Last modified by: Juan Martín Sotuyo Dodero (Maraxus)
        // Change the pk`s info of the  map
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        bool isMapPk;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();


            isMapPk = withBlock.incomingData.ReadBoolean();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) == 0)
                return;

            var argtexto = withBlock.name + " ha cambiado la información sobre si es PK el mapa.";
            General.LogGM(ref withBlock.name, ref argtexto);

            Declaraciones.mapInfo[withBlock.Pos.Map].Pk = isMapPk;

            // Change the boolean to string in a fast way
            ES.WriteVar(
                AppDomain.CurrentDomain.BaseDirectory + Declaraciones.MapPath + "mapa" + withBlock.Pos.Map + ".dat",
                "Mapa" + withBlock.Pos.Map, "Pk", isMapPk ? "1" : "0");

            WriteConsoleMsg(UserIndex,
                "Mapa " + withBlock.Pos.Map + " PK: " + Declaraciones.mapInfo[withBlock.Pos.Map].Pk,
                FontTypeNames.FONTTYPE_INFO);
        }
    }

    // '
    // Handle the "ChangeMapInfoRestricted" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleChangeMapInfoRestricted(short UserIndex)
    {
        // ***************************************************
        // Author: Pablo (ToxicWaste)
        // Last Modification: 26/01/2007
        // Restringido -> Options: "NEWBIE", "NO", "ARMADA", "CAOS", "FACCION".
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            string tStr;

            var buffer = new clsByteQueue();
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove Packet ID
                buffer.ReadByte();

                tStr = buffer.ReadASCIIString();

                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                    0)
                {
                    if ((tStr == "NEWBIE") | (tStr == "NO") | (tStr == "ARMADA") | (tStr == "CAOS") |
                        (tStr == "FACCION"))
                    {
                        var argtexto = withBlock.name + " ha cambiado la información sobre si es restringido el mapa.";
                        General.LogGM(ref withBlock.name, ref argtexto);
                        Declaraciones.mapInfo[Declaraciones.UserList[UserIndex].Pos.Map].Restringir = tStr;
                        ES.WriteVar(
                            AppDomain.CurrentDomain.BaseDirectory + Declaraciones.MapPath + "mapa" +
                            Declaraciones.UserList[UserIndex].Pos.Map + ".dat",
                            "Mapa" + Declaraciones.UserList[UserIndex].Pos.Map, "Restringir", tStr);
                        WriteConsoleMsg(UserIndex,
                            "Mapa " + withBlock.Pos.Map + " Restringido: " +
                            Declaraciones.mapInfo[withBlock.Pos.Map].Restringir, FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex,
                            "Opciones para restringir: 'NEWBIE', 'NO', 'ARMADA', 'CAOS', 'FACCION'",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleChangeMapInfoRestricted: " + ex.Message);
        }
    }

    // '
    // Handle the "ChangeMapInfoNoMagic" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleChangeMapInfoNoMagic(short UserIndex)
    {
        // ***************************************************
        // Author: Pablo (ToxicWaste)
        // Last Modification: 26/01/2007
        // MagiaSinEfecto -> Options: "1" , "0".
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        bool nomagic;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            nomagic = withBlock.incomingData.ReadBoolean();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) != 0)
            {
                var argtexto = withBlock.name +
                               " ha cambiado la información sobre si está permitido usar la magia el mapa.";
                General.LogGM(ref withBlock.name, ref argtexto);
                Declaraciones.mapInfo[Declaraciones.UserList[UserIndex].Pos.Map].MagiaSinEfecto =
                    nomagic ? (byte)1 : (byte)0;
                ES.WriteVar(
                    AppDomain.CurrentDomain.BaseDirectory + Declaraciones.MapPath + "mapa" +
                    Declaraciones.UserList[UserIndex].Pos.Map + ".dat",
                    "Mapa" + Declaraciones.UserList[UserIndex].Pos.Map, "MagiaSinEfecto", nomagic.ToString());
                WriteConsoleMsg(UserIndex,
                    "Mapa " + withBlock.Pos.Map + " MagiaSinEfecto: " +
                    Declaraciones.mapInfo[withBlock.Pos.Map].MagiaSinEfecto, FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Handle the "ChangeMapInfoNoInvi" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleChangeMapInfoNoInvi(short UserIndex)
    {
        // ***************************************************
        // Author: Pablo (ToxicWaste)
        // Last Modification: 26/01/2007
        // InviSinEfecto -> Options: "1", "0"
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        bool noinvi;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            noinvi = withBlock.incomingData.ReadBoolean();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) != 0)
            {
                var argtexto = withBlock.name +
                               " ha cambiado la información sobre si está permitido usar la invisibilidad en el mapa.";
                General.LogGM(ref withBlock.name, ref argtexto);
                Declaraciones.mapInfo[Declaraciones.UserList[UserIndex].Pos.Map].InviSinEfecto =
                    noinvi ? (byte)1 : (byte)0;
                ES.WriteVar(
                    AppDomain.CurrentDomain.BaseDirectory + Declaraciones.MapPath + "mapa" +
                    Declaraciones.UserList[UserIndex].Pos.Map + ".dat",
                    "Mapa" + Declaraciones.UserList[UserIndex].Pos.Map, "InviSinEfecto", noinvi.ToString());
                WriteConsoleMsg(UserIndex,
                    "Mapa " + withBlock.Pos.Map + " InviSinEfecto: " +
                    Declaraciones.mapInfo[withBlock.Pos.Map].InviSinEfecto, FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Handle the "ChangeMapInfoNoResu" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleChangeMapInfoNoResu(short UserIndex)
    {
        // ***************************************************
        // Author: Pablo (ToxicWaste)
        // Last Modification: 26/01/2007
        // ResuSinEfecto -> Options: "1", "0"
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 2)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        bool noresu;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            noresu = withBlock.incomingData.ReadBoolean();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) != 0)
            {
                var argtexto = withBlock.name +
                               " ha cambiado la información sobre si está permitido usar el resucitar en el mapa.";
                General.LogGM(ref withBlock.name, ref argtexto);
                Declaraciones.mapInfo[Declaraciones.UserList[UserIndex].Pos.Map].ResuSinEfecto =
                    noresu ? (byte)1 : (byte)0;
                ES.WriteVar(
                    AppDomain.CurrentDomain.BaseDirectory + Declaraciones.MapPath + "mapa" +
                    Declaraciones.UserList[UserIndex].Pos.Map + ".dat",
                    "Mapa" + Declaraciones.UserList[UserIndex].Pos.Map, "ResuSinEfecto", noresu.ToString());
                WriteConsoleMsg(UserIndex,
                    "Mapa " + withBlock.Pos.Map + " ResuSinEfecto: " +
                    Declaraciones.mapInfo[withBlock.Pos.Map].ResuSinEfecto, FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    // '
    // Handle the "ChangeMapInfoLand" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleChangeMapInfoLand(short UserIndex)
    {
        // ***************************************************
        // Author: Pablo (ToxicWaste)
        // Last Modification: 26/01/2007
        // Terreno -> Opciones: "BOSQUE", "NIEVE", "DESIERTO", "CIUDAD", "CAMPO", "DUNGEON".
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            string tStr;

            var buffer = new clsByteQueue();
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove Packet ID
                buffer.ReadByte();

                tStr = buffer.ReadASCIIString();

                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                    0)
                {
                    if ((tStr == "BOSQUE") | (tStr == "NIEVE") | (tStr == "DESIERTO") | (tStr == "CIUDAD") |
                        (tStr == "CAMPO") | (tStr == "DUNGEON"))
                    {
                        var argtexto = withBlock.name + " ha cambiado la información del terreno del mapa.";
                        General.LogGM(ref withBlock.name, ref argtexto);
                        Declaraciones.mapInfo[Declaraciones.UserList[UserIndex].Pos.Map].Terreno = tStr;
                        ES.WriteVar(
                            AppDomain.CurrentDomain.BaseDirectory + Declaraciones.MapPath + "mapa" +
                            Declaraciones.UserList[UserIndex].Pos.Map + ".dat",
                            "Mapa" + Declaraciones.UserList[UserIndex].Pos.Map, "Terreno", tStr);
                        WriteConsoleMsg(UserIndex,
                            "Mapa " + withBlock.Pos.Map + " Terreno: " +
                            Declaraciones.mapInfo[withBlock.Pos.Map].Terreno, FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex,
                            "Opciones para terreno: 'BOSQUE', 'NIEVE', 'DESIERTO', 'CIUDAD', 'CAMPO', 'DUNGEON'",
                            FontTypeNames.FONTTYPE_INFO);
                        WriteConsoleMsg(UserIndex,
                            "Igualmente, el único útil es 'NIEVE' ya que al ingresarlo, la gente muere de frío en el mapa.",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleChangeMapInfoLand: " + ex.Message);
        }
    }

    // '
    // Handle the "ChangeMapInfoZone" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleChangeMapInfoZone(short UserIndex)
    {
        // ***************************************************
        // Author: Pablo (ToxicWaste)
        // Last Modification: 26/01/2007
        // Zona -> Opciones: "BOSQUE", "NIEVE", "DESIERTO", "CIUDAD", "CAMPO", "DUNGEON".
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            string tStr;

            var buffer = new clsByteQueue();
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove Packet ID
                buffer.ReadByte();

                tStr = buffer.ReadASCIIString();

                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                    0)
                {
                    if ((tStr == "BOSQUE") | (tStr == "NIEVE") | (tStr == "DESIERTO") | (tStr == "CIUDAD") |
                        (tStr == "CAMPO") | (tStr == "DUNGEON"))
                    {
                        var argtexto = withBlock.name + " ha cambiado la información de la zona del mapa.";
                        General.LogGM(ref withBlock.name, ref argtexto);
                        Declaraciones.mapInfo[Declaraciones.UserList[UserIndex].Pos.Map].Zona = tStr;
                        ES.WriteVar(
                            AppDomain.CurrentDomain.BaseDirectory + Declaraciones.MapPath + "mapa" +
                            Declaraciones.UserList[UserIndex].Pos.Map + ".dat",
                            "Mapa" + Declaraciones.UserList[UserIndex].Pos.Map, "Zona", tStr);
                        WriteConsoleMsg(UserIndex,
                            "Mapa " + withBlock.Pos.Map + " Zona: " +
                            Declaraciones.mapInfo[withBlock.Pos.Map].Zona, FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex,
                            "Opciones para terreno: 'BOSQUE', 'NIEVE', 'DESIERTO', 'CIUDAD', 'CAMPO', 'DUNGEON'",
                            FontTypeNames.FONTTYPE_INFO);
                        WriteConsoleMsg(UserIndex,
                            "Igualmente, el único útil es 'DUNGEON' ya que al ingresarlo, NO se sentirá el efecto de la lluvia en este mapa.",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleChangeMapInfoZone: " + ex.Message);
        }
    }

    // '
    // Handle the "SaveMap" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleSaveMap(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/24/06
        // Saves the map
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            var argtexto = withBlock.name + " ha guardado el mapa " + withBlock.Pos.Map;
            General.LogGM(ref withBlock.name, ref argtexto);

            ES.GrabarMapa(withBlock.Pos.Map,
                AppDomain.CurrentDomain.BaseDirectory + "WorldBackUp/Mapa" + withBlock.Pos.Map);

            WriteConsoleMsg(UserIndex, "Mapa Guardado.", FontTypeNames.FONTTYPE_INFO);
        }
    }

    // '
    // Handle the "ShowGuildMessages" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleShowGuildMessages(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/24/06
        // Last modified by: Juan Martín Sotuyo Dodero (Maraxus)
        // Allows admins to read guild messages
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string guild;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                guild = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0)) modGuilds.GMEscuchaClan(UserIndex, guild);

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleShowGuildMessages: " + ex.Message);
        }
    }

    // '
    // Handle the "DoBackUp" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleDoBackUp(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/24/06
        // Show guilds messages
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            var argtexto = withBlock.name + " ha hecho un backup.";
            General.LogGM(ref withBlock.name, ref argtexto);

            ES.DoBackUp(); // Sino lo confunde con la id del paquete
        }
    }

    // '
    // Handle the "ToggleCentinelActivated" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleToggleCentinelActivated(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/26/06
        // Last modified by: Juan Martín Sotuyo Dodero (Maraxus)
        // Activate or desactivate the Centinel
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;

            modCentinela.centinelaActivado = !modCentinela.centinelaActivado;

            {
                ref var withBlock1 = ref modCentinela.Centinela;
                withBlock1.RevisandoUserIndex = 0;
                withBlock1.clave = 0;
                withBlock1.TiempoRestante = 0;
            }

            if (modCentinela.CentinelaNPCIndex != 0)
            {
                NPCs.QuitarNPC(modCentinela.CentinelaNPCIndex);
                modCentinela.CentinelaNPCIndex = 0;
            }

            if (modCentinela.centinelaActivado)
                modSendData.SendData(modSendData.SendTarget.ToAdmins, 0,
                    PrepareMessageConsoleMsg("El centinela ha sido activado.", FontTypeNames.FONTTYPE_SERVER));
            else
                modSendData.SendData(modSendData.SendTarget.ToAdmins, 0,
                    PrepareMessageConsoleMsg("El centinela ha sido desactivado.", FontTypeNames.FONTTYPE_SERVER));
        }
    }

    // '
    // Handle the "AlterName" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleAlterName(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 12/26/06
        // Change user name
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            string newName;
            short changeNameUI;
            short GuildIndex;
            byte cantPenas;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                // Reads the userName and newUser Packets

                UserName = buffer.ReadASCIIString();
                newName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                {
                    if ((Migration.migr_LenB(UserName) == 0) | (Migration.migr_LenB(newName) == 0))
                    {
                        WriteConsoleMsg(UserIndex, "Usar: /ANAME origen@destino", FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        changeNameUI = Extra.NameIndex(UserName);

                        if (changeNameUI > 0)
                        {
                            WriteConsoleMsg(UserIndex, "El Pj está online, debe salir para hacer el cambio.",
                                FontTypeNames.FONTTYPE_WARNING);
                        }
                        else if (!File.Exists(Declaraciones.CharPath + UserName + ".chr"))
                        {
                            WriteConsoleMsg(UserIndex, "El pj " + UserName + " es inexistente.",
                                FontTypeNames.FONTTYPE_INFO);
                        }
                        else
                        {
                            var argEmptySpaces = 1024;
                            GuildIndex = Convert.ToInt16(Migration.ParseVal(ES.GetVar(
                                Declaraciones.CharPath + UserName + ".chr", "GUILD", "GUILDINDEX",
                                ref argEmptySpaces)));

                            if (GuildIndex > 0)
                            {
                                WriteConsoleMsg(UserIndex,
                                    "El pj " + UserName +
                                    " pertenece a un clan, debe salir del mismo con /salirclan para ser transferido.",
                                    FontTypeNames.FONTTYPE_INFO);
                            }
                            else if (!File.Exists(Declaraciones.CharPath + newName + ".chr"))
                            {
                                FileSystem.FileCopy(Declaraciones.CharPath + UserName + ".chr",
                                    Declaraciones.CharPath + newName.ToUpper() + ".chr");

                                WriteConsoleMsg(UserIndex, "Transferencia exitosa.", FontTypeNames.FONTTYPE_INFO);

                                ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "FLAGS", "Ban", "1");


                                var argEmptySpaces1 = 1024;
                                cantPenas = Convert.ToByte(Migration.ParseVal(ES.GetVar(
                                    Declaraciones.CharPath + UserName + ".chr", "PENAS", "Cant", ref argEmptySpaces1)));

                                ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "PENAS", "Cant",
                                    (cantPenas + 1).ToString());

                                ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "PENAS", "P" + (cantPenas + 1),
                                    withBlock.name.ToLower() + ": BAN POR Cambio de nick a " + newName.ToUpper() + " " +
                                    Conversions.ToString(DateTime.Today) + " " +
                                    Conversions.ToString(DateAndTime.TimeOfDay));

                                var argtexto = "Ha cambiado de nombre al usuario " + UserName + ". Ahora se llama " +
                                               newName;
                                General.LogGM(ref withBlock.name, ref argtexto);
                            }
                            else
                            {
                                WriteConsoleMsg(UserIndex, "El nick solicitado ya existe.",
                                    FontTypeNames.FONTTYPE_INFO);
                            }
                        }
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleAlterName: " + ex.Message);
        }
    }

    // '
    // Handle the "AlterName" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleAlterMail(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 12/26/06
        // Change user password
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            string newMail;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();
                newMail = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                {
                    if ((Migration.migr_LenB(UserName) == 0) | (Migration.migr_LenB(newMail) == 0))
                    {
                        WriteConsoleMsg(UserIndex, "usar /AEMAIL <pj>-<nuevomail>", FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        if (!File.Exists(Declaraciones.CharPath + UserName + ".chr"))
                        {
                            WriteConsoleMsg(UserIndex, "No existe el charfile " + UserName + ".chr",
                                FontTypeNames.FONTTYPE_INFO);
                        }
                        else
                        {
                            ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "CONTACTO", "Email", newMail);
                            WriteConsoleMsg(UserIndex, "Email de " + UserName + " cambiado a: " + newMail,
                                FontTypeNames.FONTTYPE_INFO);
                        }

                        var argtexto = "Le ha cambiado el mail a " + UserName;
                        General.LogGM(ref withBlock.name, ref argtexto);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleAlterMail: " + ex.Message);
        }
    }

    // '
    // Handle the "AlterPassword" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleAlterPassword(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 12/26/06
        // Change user password
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 5)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            string copyFrom;
            string Password;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString().Replace("+", " ");
                copyFrom = buffer.ReadASCIIString().Replace("+", " ");

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                {
                    var argtexto = "Ha alterado la contraseña de " + UserName;
                    General.LogGM(ref withBlock.name, ref argtexto);

                    if ((Migration.migr_LenB(UserName) == 0) | (Migration.migr_LenB(copyFrom) == 0))
                    {
                        WriteConsoleMsg(UserIndex, "usar /APASS <pjsinpass>@<pjconpass>", FontTypeNames.FONTTYPE_INFO);
                    }
                    else if (!File.Exists(Declaraciones.CharPath + UserName + ".chr") |
                             !File.Exists(Declaraciones.CharPath + copyFrom + ".chr"))
                    {
                        WriteConsoleMsg(UserIndex, "Alguno de los PJs no existe " + UserName + "@" + copyFrom,
                            FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        var argEmptySpaces = 1024;
                        Password = ES.GetVar(Declaraciones.CharPath + copyFrom + ".chr", "INIT", "Password",
                            ref argEmptySpaces);
                        ES.WriteVar(Declaraciones.CharPath + UserName + ".chr", "INIT", "Password", Password);

                        WriteConsoleMsg(UserIndex, "Password de " + UserName + " ha cambiado por la de " + copyFrom,
                            FontTypeNames.FONTTYPE_INFO);
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleAlterPassword: " + ex.Message);
        }
    }

    // '
    // Handle the "HandleCreateNPC" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleCreateNPC(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 12/24/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        short NpcIndex;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();


            NpcIndex = withBlock.incomingData.ReadInteger();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;

            NpcIndex = NPCs.SpawnNpc(NpcIndex, ref withBlock.Pos, true, false);

            if (NpcIndex != 0)
            {
                var argtexto = "Sumoneó a " + Declaraciones.Npclist[NpcIndex].name + " en mapa " + withBlock.Pos.Map;
                General.LogGM(ref withBlock.name, ref argtexto);
            }
        }
    }


    // '
    // Handle the "CreateNPCWithRespawn" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleCreateNPCWithRespawn(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 12/24/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        short NpcIndex;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();


            NpcIndex = withBlock.incomingData.ReadInteger();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0)
                return;

            NpcIndex = NPCs.SpawnNpc(NpcIndex, ref withBlock.Pos, true, true);

            if (NpcIndex != 0)
            {
                var argtexto = "Sumoneó con respawn " + Declaraciones.Npclist[NpcIndex].name + " en mapa " +
                               withBlock.Pos.Map;
                General.LogGM(ref withBlock.name, ref argtexto);
            }
        }
    }

    // '
    // Handle the "ImperialArmour" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleImperialArmour(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 12/24/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 4)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte Index;
        short ObjIndex;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();


            Index = withBlock.incomingData.ReadByte();
            ObjIndex = withBlock.incomingData.ReadInteger();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            switch (Index)
            {
                case 1:
                {
                    ModFacciones.ArmaduraImperial1 = ObjIndex;
                    break;
                }

                case 2:
                {
                    ModFacciones.ArmaduraImperial2 = ObjIndex;
                    break;
                }

                case 3:
                {
                    ModFacciones.ArmaduraImperial3 = ObjIndex;
                    break;
                }

                case 4:
                {
                    ModFacciones.TunicaMagoImperial = ObjIndex;
                    break;
                }
            }
        }
    }

    // '
    // Handle the "ChaosArmour" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleChaosArmour(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 12/24/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 4)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        byte Index;
        short ObjIndex;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();


            Index = withBlock.incomingData.ReadByte();
            ObjIndex = withBlock.incomingData.ReadInteger();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            switch (Index)
            {
                case 1:
                {
                    ModFacciones.ArmaduraCaos1 = ObjIndex;
                    break;
                }

                case 2:
                {
                    ModFacciones.ArmaduraCaos2 = ObjIndex;
                    break;
                }

                case 3:
                {
                    ModFacciones.ArmaduraCaos3 = ObjIndex;
                    break;
                }

                case 4:
                {
                    ModFacciones.TunicaMagoCaos = ObjIndex;
                    break;
                }
            }
        }
    }

    // '
    // Handle the "NavigateToggle" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleNavigateToggle(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 01/12/07
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero)) !=
                0)
                return;

            if (withBlock.flags.Navegando == 1)
                withBlock.flags.Navegando = 0;
            else
                withBlock.flags.Navegando = 1;

            // Tell the client that we are navigating.
            WriteNavigateToggle(UserIndex);
        }
    }

    // '
    // Handle the "ServerOpenToUsersToggle" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleServerOpenToUsersToggle(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 12/24/06
        // 
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            if (Declaraciones.ServerSoloGMs > 0)
            {
                WriteConsoleMsg(UserIndex, "Servidor habilitado para todos.", FontTypeNames.FONTTYPE_INFO);
                Declaraciones.ServerSoloGMs = 0;
            }
            else
            {
                WriteConsoleMsg(UserIndex, "Servidor restringido a administradores.", FontTypeNames.FONTTYPE_INFO);
                Declaraciones.ServerSoloGMs = 1;
            }
        }
    }

    // '
    // Handle the "TurnOffServer" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleTurnOffServer(short UserIndex)
    {

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.User | Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios |
                                                Declaraciones.PlayerType.RoleMaster)) != 0)
                return;

            var argtexto = "/APAGAR";
            General.LogGM(ref withBlock.name, ref argtexto);
            modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                PrepareMessageConsoleMsg("¡¡¡" + withBlock.name + " VA A APAGAR EL SERVIDOR!!!",
                    FontTypeNames.FONTTYPE_FIGHT));

            // Log
            General.AppendLog("logs/Main.log",
                Conversions.ToString(DateTime.Today) + " " + Conversions.ToString(DateAndTime.TimeOfDay) +
                " server apagado por " + withBlock.name + ". ");
        }
    }

    // '
    // Handle the "TurnCriminal" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleTurnCriminal(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 12/26/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                {
                    var argtexto = "/CONDEN " + UserName;
                    General.LogGM(ref withBlock.name, ref argtexto);

                    tUser = Extra.NameIndex(UserName);
                    if (tUser > 0)
                        UsUaRiOs.VolverCriminal(tUser);
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleTurnCriminal: " + ex.Message);
        }
    }

    // '
    // Handle the "ResetFactions" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleResetFactions(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 06/09/09
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short tUser;
            string character;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                {
                    var argtexto = "/RAJAR " + UserName;
                    General.LogGM(ref withBlock.name, ref argtexto);

                    tUser = Extra.NameIndex(UserName);

                    if (tUser > 0)
                    {
                        TCP.ResetFacciones(tUser);
                    }
                    else
                    {
                        character = Declaraciones.CharPath + UserName + ".chr";

                        if (File.Exists(character))
                        {
                            ES.WriteVar(character, "FACCIONES", "EjercitoReal", 0.ToString());
                            ES.WriteVar(character, "FACCIONES", "CiudMatados", 0.ToString());
                            ES.WriteVar(character, "FACCIONES", "CrimMatados", 0.ToString());
                            ES.WriteVar(character, "FACCIONES", "EjercitoCaos", 0.ToString());
                            ES.WriteVar(character, "FACCIONES", "FechaIngreso", "No ingresó a ninguna Facción");
                            ES.WriteVar(character, "FACCIONES", "rArCaos", 0.ToString());
                            ES.WriteVar(character, "FACCIONES", "rArReal", 0.ToString());
                            ES.WriteVar(character, "FACCIONES", "rExCaos", 0.ToString());
                            ES.WriteVar(character, "FACCIONES", "rExReal", 0.ToString());
                            ES.WriteVar(character, "FACCIONES", "recCaos", 0.ToString());
                            ES.WriteVar(character, "FACCIONES", "recReal", 0.ToString());
                            ES.WriteVar(character, "FACCIONES", "Reenlistadas", 0.ToString());
                            ES.WriteVar(character, "FACCIONES", "NivelIngreso", 0.ToString());
                            ES.WriteVar(character, "FACCIONES", "MatadosIngreso", 0.ToString());
                            ES.WriteVar(character, "FACCIONES", "NextRecompensa", 0.ToString());
                        }
                        else
                        {
                            WriteConsoleMsg(UserIndex, "El personaje " + UserName + " no existe.",
                                FontTypeNames.FONTTYPE_INFO);
                        }
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleResetFactions: " + ex.Message);
        }
    }

    // '
    // Handle the "RemoveCharFromGuild" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleRemoveCharFromGuild(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 12/26/06
        // 
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            short GuildIndex;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                {
                    var argtexto = "/RAJARCLAN " + UserName;
                    General.LogGM(ref withBlock.name, ref argtexto);

                    GuildIndex = modGuilds.m_EcharMiembroDeClan(UserIndex, UserName);

                    if (GuildIndex == 0)
                    {
                        WriteConsoleMsg(UserIndex, "No pertenece a ningún clan o es fundador.",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        WriteConsoleMsg(UserIndex, "Expulsado.", FontTypeNames.FONTTYPE_INFO);
                        modSendData.SendData(modSendData.SendTarget.ToGuildMembers, GuildIndex,
                            PrepareMessageConsoleMsg(
                                UserName + " ha sido expulsado del clan por los administradores del servidor.",
                                FontTypeNames.FONTTYPE_GUILD));
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleRemoveCharFromGuild: " + ex.Message);
        }
    }

    // '
    // Handle the "RequestCharMail" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleRequestCharMail(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 12/26/06
        // Request user mail
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string UserName;
            string mail;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                UserName = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                    if (File.Exists(Declaraciones.CharPath + UserName + ".chr"))
                    {
                        var argEmptySpaces = 1024;
                        mail = ES.GetVar(Declaraciones.CharPath + UserName + ".chr", "CONTACTO", "email",
                            ref argEmptySpaces);

                        WriteConsoleMsg(UserIndex, "Last email de " + UserName + ":" + mail,
                            FontTypeNames.FONTTYPE_INFO);
                    }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleRequestCharMail: " + ex.Message);
        }
    }

    // '
    // Handle the "SystemMessage" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleSystemMessage(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/29/06
        // Send a message to all the users
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string message;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();

                message = buffer.ReadASCIIString();

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                {
                    var argtexto = "Mensaje de sistema:" + message;
                    General.LogGM(ref withBlock.name, ref argtexto);

                    modSendData.SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageShowMessageBox(message));
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleSystemMessage: " + ex.Message);
        }
    }

    // '
    // Handle the "SetMOTD" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleSetMOTD(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 03/31/07
        // Set the MOTD
        // Modified by: Juan Martín Sotuyo Dodero (Maraxus)
        // - Fixed a bug that prevented from properly setting the new number of lines.
        // - Fixed a bug that caused the player to be kicked.
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 3)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string newMOTD;
            string[] auxiliaryString;
            int LoopC;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...
                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                newMOTD = buffer.ReadASCIIString();
                auxiliaryString = Strings.Split(newMOTD, Constants.vbCrLf);

                if (((int)(~withBlock.flags.Privilegios & Declaraciones.PlayerType.RoleMaster) != 0) &
                    ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                     0))
                {
                    var argtexto = "Ha fijado un nuevo MOTD";
                    General.LogGM(ref withBlock.name, ref argtexto);

                    Admin.MaxLines = Convert.ToInt16(auxiliaryString.Length);

                    // UPGRADE_WARNING: El límite inferior de la matriz MOTD ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                    Admin.MOTD = new Admin.tMotd[Admin.MaxLines + 1];

                    ES.WriteVar(AppDomain.CurrentDomain.BaseDirectory + "Dat/Motd.ini", "INIT", "NumLines",
                        Admin.MaxLines.ToString());

                    var loopTo = (int)Admin.MaxLines;
                    for (LoopC = 1; LoopC <= loopTo; LoopC++)
                    {
                        ES.WriteVar(AppDomain.CurrentDomain.BaseDirectory + "Dat/Motd.ini", "Motd", "Line" + LoopC,
                            auxiliaryString[LoopC - 1]);

                        Admin.MOTD[LoopC].texto = auxiliaryString[LoopC - 1];
                    }

                    WriteConsoleMsg(UserIndex, "Se ha cambiado el MOTD con éxito.", FontTypeNames.FONTTYPE_INFO);
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleSetMOTD: " + ex.Message);
        }
    }

    // '
    // Handle the "ChangeMOTD" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleChangeMOTD(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín sotuyo Dodero (Maraxus)
        // Last Modification: 12/29/06
        // Change the MOTD
        // ***************************************************
        var auxiliaryString = default(string);
        int LoopC;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.RoleMaster | Declaraciones.PlayerType.User |
                                                Declaraciones.PlayerType.Consejero |
                                                Declaraciones.PlayerType.SemiDios)) != 0) return;


            var loopTo = Admin.MOTD.Length - 1;
            for (LoopC = 0; LoopC <= loopTo; LoopC++)
                auxiliaryString = auxiliaryString + Admin.MOTD[LoopC].texto + Constants.vbCrLf;

            if (auxiliaryString.Length >= 2)
                if ((auxiliaryString.Substring(Math.Max(0, auxiliaryString.Length - 2)) ?? "") == Constants.vbCrLf)
                    auxiliaryString =
                        auxiliaryString.Substring(0, Math.Min(auxiliaryString.Length - 2, auxiliaryString.Length));

            WriteShowMOTDEditionForm(UserIndex, auxiliaryString);
        }
    }

    // '
    // Handle the "Ping" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandlePing(short UserIndex)
    {
        // ***************************************************
        // Author: Lucas Tavolaro Ortiz (Tavo)
        // Last Modification: 12/24/06
        // Show guilds messages
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Remove Packet ID
            withBlock.incomingData.ReadByte();

            WritePong(UserIndex);
        }
    }

    // '
    // Handle the "SetIniVar" message
    // 
    // @param userIndex The index of the user sending the message

    public static void HandleSetIniVar(short UserIndex)
    {
        // ***************************************************
        // Author: Brian Chaia (BrianPr)
        // Last Modification: 01/23/10 (Marco)
        // Modify server.ini
        // ***************************************************
        if (Declaraciones.UserList[UserIndex].incomingData.length < 6)
        {
            Information.Err().Raise(Declaraciones.UserList[UserIndex].incomingData.NotEnoughDataErrCode);
            return;
        }

        try
        {
            var buffer = new clsByteQueue();
            string sLlave;
            string sClave;
            string sValor;
            string sTmp;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // This packet contains strings, make a copy of the data to prevent losses if it's not complete yet...

                buffer.CopyBuffer(ref withBlock.incomingData);

                // Remove packet ID
                buffer.ReadByte();


                // Obtengo los parámetros
                sLlave = buffer.ReadASCIIString();
                sClave = buffer.ReadASCIIString();
                sValor = buffer.ReadASCIIString();

                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios)) !=
                    0)
                {
                    // No podemos modificar [INIT]Dioses ni [Dioses]*
                    if (((sLlave.ToUpper() == "INIT") & (sClave.ToUpper() == "DIOSES")) |
                        (sLlave.ToUpper() == "DIOSES"))
                    {
                        WriteConsoleMsg(UserIndex, "¡No puedes modificar esa información desde aquí!",
                            FontTypeNames.FONTTYPE_INFO);
                    }
                    else
                    {
                        // Obtengo el valor según llave y clave
                        var argEmptySpaces = 1024;
                        sTmp = ES.GetVar(Declaraciones.IniPath + "Server.ini", sLlave, sClave, ref argEmptySpaces);

                        // Si obtengo un valor escribo en el server.ini
                        if (Migration.migr_LenB(sTmp) != 0)
                        {
                            ES.WriteVar(Declaraciones.IniPath + "Server.ini", sLlave, sClave, sValor);
                            var argtexto = "Modificó en server.ini (" + sLlave + " " + sClave + ") el valor " + sTmp +
                                           " por " + sValor;
                            General.LogGM(ref withBlock.name, ref argtexto);
                            WriteConsoleMsg(UserIndex,
                                "Modificó " + sLlave + " " + sClave + " a " + sValor + ". Valor anterior " + sTmp,
                                FontTypeNames.FONTTYPE_INFO);
                        }
                        else
                        {
                            WriteConsoleMsg(UserIndex, "No existe la llave y/o clave", FontTypeNames.FONTTYPE_INFO);
                        }
                    }
                }

                // If we got here then packet is complete, copy data back to original queue
                withBlock.incomingData.CopyBuffer(ref buffer);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in HandleSetIniVar: " + ex.Message);
            int error;
        }
    }

    // '
    // Writes the "Logged" message to the given user's outgoing data buffer.
    // 
    // @param    UserIndex User to which the message is intended.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static void WriteLoggedMessage(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "Logged" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.Logged),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteRemoveAllDialogs(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "RemoveDialogs" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.RemoveDialogs),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteRemoveCharDialog(short UserIndex, short CharIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "RemoveCharDialog" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData
                .WriteASCIIStringFixed(PrepareMessageRemoveCharDialog(CharIndex)), () => FlushBuffer(UserIndex));
    }

    public static void WriteNavigateToggle(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "NavigateToggle" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.NavigateToggle),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteDisconnect(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "Disconnect" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.Disconnect),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteUserOfferConfirm(short UserIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 14/12/2009
        // Writes the "UserOfferConfirm" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.UserOfferConfirm),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteCommerceEnd(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "CommerceEnd" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.CommerceEnd),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteBankEnd(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "BankEnd" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.BankEnd),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteCommerceInit(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "CommerceInit" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.CommerceInit),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteBankInit(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "BankInit" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.BankInit);
            Declaraciones.UserList[UserIndex].outgoingData.WriteLong(Declaraciones.UserList[UserIndex].Stats.Banco);
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteUserCommerceInit(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "UserCommerceInit" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.UserCommerceInit);
            Declaraciones.UserList[UserIndex].outgoingData
                .WriteASCIIString(Declaraciones.UserList[UserIndex].ComUsu.DestNick);
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteUserCommerceEnd(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "UserCommerceEnd" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.UserCommerceEnd),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteShowBlacksmithForm(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ShowBlacksmithForm" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.ShowBlacksmithForm),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteShowCarpenterForm(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ShowCarpenterForm" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.ShowCarpenterForm),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteUpdateSta(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "UpdateMana" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.UpdateSta);
                withBlock.WriteInteger(Declaraciones.UserList[UserIndex].Stats.MinSta);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteUpdateMana(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "UpdateMana" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.UpdateMana);
                withBlock.WriteInteger(Declaraciones.UserList[UserIndex].Stats.MinMAN);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteUpdateHP(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "UpdateMana" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.UpdateHP);
                withBlock.WriteInteger(Declaraciones.UserList[UserIndex].Stats.MinHp);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteUpdateGold(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "UpdateGold" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.UpdateGold);
                withBlock.WriteLong(Declaraciones.UserList[UserIndex].Stats.GLD);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteUpdateBankGold(short UserIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 14/12/2009
        // Writes the "UpdateBankGold" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.UpdateBankGold);
                withBlock.WriteLong(Declaraciones.UserList[UserIndex].Stats.Banco);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteUpdateExp(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "UpdateExp" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.UpdateExp);
                withBlock.WriteLong(Convert.ToInt32(Declaraciones.UserList[UserIndex].Stats.Exp));
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteUpdateStrenghtAndDexterity(short UserIndex)
    {
        // ***************************************************
        // Author: Budi
        // Last Modification: 11/26/09
        // Writes the "UpdateStrenghtAndDexterity" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.UpdateStrenghtAndDexterity);
                withBlock.WriteByte(Declaraciones.UserList[UserIndex].Stats
                    .UserAtributos[(int)Declaraciones.eAtributos.Fuerza]);
                withBlock.WriteByte(Declaraciones.UserList[UserIndex].Stats
                    .UserAtributos[(int)Declaraciones.eAtributos.Agilidad]);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteUpdateDexterity(short UserIndex)
    {
        // ***************************************************
        // Author: Budi
        // Last Modification: 11/26/09
        // Writes the "UpdateStrenghtAndDexterity" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.UpdateDexterity);
                withBlock.WriteByte(Declaraciones.UserList[UserIndex].Stats
                    .UserAtributos[(int)Declaraciones.eAtributos.Agilidad]);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteUpdateStrenght(short UserIndex)
    {
        // ***************************************************
        // Author: Budi
        // Last Modification: 11/26/09
        // Writes the "UpdateStrenghtAndDexterity" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.UpdateStrenght);
                withBlock.WriteByte(Declaraciones.UserList[UserIndex].Stats
                    .UserAtributos[(int)Declaraciones.eAtributos.Fuerza]);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteChangeMap(short UserIndex, short Map, short version)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ChangeMap" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.ChangeMap);
                withBlock.WriteInteger(Map);
                withBlock.WriteInteger(version);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WritePosUpdate(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "PosUpdate" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.PosUpdate);
                withBlock.WriteByte(Convert.ToByte(Declaraciones.UserList[UserIndex].Pos.X));
                withBlock.WriteByte(Convert.ToByte(Declaraciones.UserList[UserIndex].Pos.Y));
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteChatOverHead(short UserIndex, string Chat, short CharIndex, int color)
    {
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData
                .WriteASCIIStringFixed(PrepareMessageChatOverHead(Chat, CharIndex, color)),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteConsoleMsg(short UserIndex, string Chat, FontTypeNames FontIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ConsoleMsg" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData
                .WriteASCIIStringFixed(PrepareMessageConsoleMsg(Chat, FontIndex)), () => FlushBuffer(UserIndex));
    }

    public static void WriteCommerceChat(short UserIndex, string Chat, FontTypeNames FontIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 05/17/06
        // Writes the "ConsoleMsg" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData
                .WriteASCIIStringFixed(PrepareCommerceConsoleMsg(ref Chat, FontIndex)), () => FlushBuffer(UserIndex));
    }

    public static void WriteGuildChat(short UserIndex, string Chat)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "GuildChat" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteASCIIStringFixed(PrepareMessageGuildChat(Chat)),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteShowMessageBox(short UserIndex, string message)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ShowMessageBox" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.ShowMessageBox);
                withBlock.WriteASCIIString(message);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteUserIndexInServer(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "UserIndexInServer" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.UserIndexInServer);
                withBlock.WriteInteger(UserIndex);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteUserCharIndexInServer(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "UserIndexInServer" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.UserCharIndexInServer);
                withBlock.WriteInteger(Declaraciones.UserList[UserIndex].character.CharIndex);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteCharacterCreate(short UserIndex, short body, short Head, Declaraciones.eHeading heading,
        short CharIndex, byte X, byte Y, short weapon, short shield, short FX, short FXLoops, short helmet, string name,
        byte NickColor, byte Privileges)
    {
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteASCIIStringFixed(
                PrepareMessageCharacterCreate(body, Head, heading, CharIndex, X, Y, weapon, shield, FX, FXLoops, helmet,
                    name, NickColor, Privileges)), () => FlushBuffer(UserIndex));
    }

    public static void WriteCharacterRemove(short UserIndex, short CharIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "CharacterRemove" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData
                .WriteASCIIStringFixed(PrepareMessageCharacterRemove(CharIndex)), () => FlushBuffer(UserIndex));
    }

    public static void WriteCharacterMove(short UserIndex, short CharIndex, byte X, byte Y)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "CharacterMove" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData
                .WriteASCIIStringFixed(PrepareMessageCharacterMove(CharIndex, X, Y)), () => FlushBuffer(UserIndex));
    }

    public static void WriteForceCharMove(short UserIndex, Declaraciones.eHeading Direccion)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 26/03/2009
        // Writes the "ForceCharMove" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData
                .WriteASCIIStringFixed(PrepareMessageForceCharMove(Direccion)), () => FlushBuffer(UserIndex));
    }

    public static void WriteCharacterChange(short UserIndex, short body, short Head, Declaraciones.eHeading heading,
        short CharIndex, short weapon, short shield, short FX, short FXLoops, short helmet)
    {
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteASCIIStringFixed(
                PrepareMessageCharacterChange(body, Head, heading, CharIndex, weapon, shield, FX, FXLoops, helmet)),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteObjectCreate(short UserIndex, short GrhIndex, byte X, byte Y)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ObjectCreate" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData
                .WriteASCIIStringFixed(PrepareMessageObjectCreate(GrhIndex, X, Y)), () => FlushBuffer(UserIndex));
    }

    public static void WriteObjectDelete(short UserIndex, byte X, byte Y)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ObjectDelete" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData
                .WriteASCIIStringFixed(PrepareMessageObjectDelete(X, Y)), () => FlushBuffer(UserIndex));
    }

    public static void WriteBlockPosition(short UserIndex, byte X, byte Y, bool Blocked)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "BlockPosition" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.BlockPosition);
                withBlock.WriteByte(X);
                withBlock.WriteByte(Y);
                withBlock.WriteBoolean(Blocked);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WritePlayMidi(short UserIndex, byte midi, short loops = -1)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "PlayMidi" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData
                .WriteASCIIStringFixed(PrepareMessagePlayMidi(midi, loops)), () => FlushBuffer(UserIndex));
    }

    public static void WritePlayWave(short UserIndex, byte wave, byte X, byte Y)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 08/08/07
        // Last Modified by: Rapsodius
        // Added X and Y positions for 3D Sounds
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData
                .WriteASCIIStringFixed(PrepareMessagePlayWave(wave, X, Y)), () => FlushBuffer(UserIndex));
    }

    public static void WriteGuildList(short UserIndex, string[] guildList)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "GuildList" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            var Tmp = default(string);
            int i;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.guildList);
                var loopTo = guildList.Length - 1;
                for (i = 0; i <= loopTo; i++)
                    Tmp = Tmp + guildList[i] + SEPARATOR;
                if (Tmp.Length > 0)
                    Tmp = Tmp.Substring(0, Math.Min(Tmp.Length - 1, Tmp.Length));
                withBlock.WriteASCIIString(Tmp);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteAreaChanged(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "AreaChanged" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.AreaChanged);
                withBlock.WriteByte(Convert.ToByte(Declaraciones.UserList[UserIndex].Pos.X));
                withBlock.WriteByte(Convert.ToByte(Declaraciones.UserList[UserIndex].Pos.Y));
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WritePauseToggle(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "PauseToggle" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteASCIIStringFixed(PrepareMessagePauseToggle()),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteRainToggle(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "RainToggle" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteASCIIStringFixed(PrepareMessageRainToggle()),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteCreateFX(short UserIndex, short CharIndex, short FX, short FXLoops)
    {
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData
                .WriteASCIIStringFixed(PrepareMessageCreateFX(CharIndex, FX, FXLoops)), () => FlushBuffer(UserIndex));
    }

    public static void WriteUpdateUserStats(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "UpdateUserStats" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.UpdateUserStats);
                withBlock.WriteInteger(Declaraciones.UserList[UserIndex].Stats.MaxHp);
                withBlock.WriteInteger(Declaraciones.UserList[UserIndex].Stats.MinHp);
                withBlock.WriteInteger(Declaraciones.UserList[UserIndex].Stats.MaxMAN);
                withBlock.WriteInteger(Declaraciones.UserList[UserIndex].Stats.MinMAN);
                withBlock.WriteInteger(Declaraciones.UserList[UserIndex].Stats.MaxSta);
                withBlock.WriteInteger(Declaraciones.UserList[UserIndex].Stats.MinSta);
                withBlock.WriteLong(Declaraciones.UserList[UserIndex].Stats.GLD);
                withBlock.WriteByte(Declaraciones.UserList[UserIndex].Stats.ELV);
                withBlock.WriteLong(Convert.ToInt32(Declaraciones.UserList[UserIndex].Stats.ELU));
                withBlock.WriteLong(Convert.ToInt32(Declaraciones.UserList[UserIndex].Stats.Exp));
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteWorkRequestTarget(short UserIndex, Declaraciones.eSkill Skill)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "WorkRequestTarget" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.WorkRequestTarget);
                withBlock.WriteByte(Convert.ToByte((short)Skill));
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteChangeInventorySlot(short UserIndex, byte Slot)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 3/12/09
        // Writes the "ChangeInventorySlot" message to the given user's outgoing data buffer
        // 3/12/09: Budi - Ahora se envia MaxDef y MinDef en lugar de Def
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            short ObjIndex;
            var obData = default(Declaraciones.ObjData);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.ChangeInventorySlot);
                withBlock.WriteByte(Slot);
                ObjIndex = Declaraciones.UserList[UserIndex].Invent.userObj[Slot].ObjIndex;
                if (ObjIndex > 0) obData = Declaraciones.objData[ObjIndex];
                withBlock.WriteInteger(ObjIndex);
                withBlock.WriteASCIIString(obData.name);
                withBlock.WriteInteger(Declaraciones.UserList[UserIndex].Invent.userObj[Slot].Amount);
                withBlock.WriteBoolean(
                    Convert.ToBoolean(Declaraciones.UserList[UserIndex].Invent.userObj[Slot].Equipped));
                withBlock.WriteInteger(obData.GrhIndex);
                withBlock.WriteByte(Convert.ToByte((int)obData.OBJType));
                withBlock.WriteInteger(obData.MaxHIT);
                withBlock.WriteInteger(obData.MinHIT);
                withBlock.WriteInteger(obData.MaxDef);
                withBlock.WriteInteger(obData.MinDef);
                withBlock.WriteSingle(modSistemaComercio.SalePrice(ObjIndex));
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteAddSlots(short UserIndex, Declaraciones.eMochilas Mochila)
    {
        // ***************************************************
        // Author: Budi
        // Last Modification: 01/12/09
        // Writes the "AddSlots" message to the given user's outgoing data buffer
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
            withBlock.WriteByte((byte)ServerPacketID.AddSlots);
            withBlock.WriteByte(Convert.ToByte((int)Mochila));
        }
    }


    // '
    // Writes the "ChangeBankSlot" message to the given user's outgoing data buffer.
    // 
    // @param    UserIndex User to which the message is intended.
    // @param    slot Inventory slot which needs to be updated.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static void WriteChangeBankSlot(short UserIndex, byte Slot)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 12/03/09
        // Writes the "ChangeBankSlot" message to the given user's outgoing data buffer
        // 12/03/09: Budi - Ahora se envia MaxDef y MinDef en lugar de sólo Def
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            short ObjIndex;
            var obData = default(Declaraciones.ObjData);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.ChangeBankSlot);
                withBlock.WriteByte(Slot);
                ObjIndex = Declaraciones.UserList[UserIndex].BancoInvent.userObj[Slot].ObjIndex;
                withBlock.WriteInteger(ObjIndex);
                if (ObjIndex > 0) obData = Declaraciones.objData[ObjIndex];
                withBlock.WriteASCIIString(obData.name);
                withBlock.WriteInteger(Declaraciones.UserList[UserIndex].BancoInvent.userObj[Slot].Amount);
                withBlock.WriteInteger(obData.GrhIndex);
                withBlock.WriteByte(Convert.ToByte((int)obData.OBJType));
                withBlock.WriteInteger(obData.MaxHIT);
                withBlock.WriteInteger(obData.MinHIT);
                withBlock.WriteInteger(obData.MaxDef);
                withBlock.WriteInteger(obData.MinDef);
                withBlock.WriteLong(obData.Valor);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteChangeSpellSlot(short UserIndex, short Slot)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ChangeSpellSlot" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.ChangeSpellSlot);
                withBlock.WriteByte(Convert.ToByte(Slot));
                withBlock.WriteInteger(Declaraciones.UserList[UserIndex].Stats.UserHechizos[Slot]);
                if (Declaraciones.UserList[UserIndex].Stats.UserHechizos[Slot] > 0)
                    withBlock.WriteASCIIString(Declaraciones
                        .Hechizos[Declaraciones.UserList[UserIndex].Stats.UserHechizos[Slot]].Nombre);
                else
                    withBlock.WriteASCIIString("(None)");
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteAttributes(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "Atributes" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.Atributes);
                withBlock.WriteByte(Declaraciones.UserList[UserIndex].Stats
                    .UserAtributos[(int)Declaraciones.eAtributos.Fuerza]);
                withBlock.WriteByte(Declaraciones.UserList[UserIndex].Stats
                    .UserAtributos[(int)Declaraciones.eAtributos.Agilidad]);
                withBlock.WriteByte(Declaraciones.UserList[UserIndex].Stats
                    .UserAtributos[(int)Declaraciones.eAtributos.Inteligencia]);
                withBlock.WriteByte(Declaraciones.UserList[UserIndex].Stats
                    .UserAtributos[(int)Declaraciones.eAtributos.Carisma]);
                withBlock.WriteByte(Declaraciones.UserList[UserIndex].Stats
                    .UserAtributos[(int)Declaraciones.eAtributos.Constitucion]);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteBlacksmithWeapons(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 04/15/2008 (NicoNZ) Habia un error al fijarse los skills del personaje
        // Writes the "BlacksmithWeapons" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            int i;
            Declaraciones.ObjData obj;
            short[] validIndexes;
            var Count = default(short);
            validIndexes = new short[Declaraciones.ArmasHerrero.Length];
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.BlacksmithWeapons);
                var loopTo = Declaraciones.ArmasHerrero.Length - 1;
                for (i = 1; i <= loopTo; i++)
                    if (Declaraciones.objData[Declaraciones.ArmasHerrero[i]].SkHerreria <= Math.Round(
                            Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Herreria] /
                            Trabajo.ModHerreriA(Declaraciones.UserList[UserIndex].clase), 0))
                    {
                        Count = Convert.ToInt16(Count + 1);
                        validIndexes[Count] = Convert.ToInt16(i);
                    }

                withBlock.WriteInteger(Count);
                var loopTo1 = (int)Count;
                for (i = 1; i <= loopTo1; i++)
                {
                    obj = Declaraciones.objData[Declaraciones.ArmasHerrero[validIndexes[i]]];
                    withBlock.WriteASCIIString(obj.name);
                    withBlock.WriteInteger(obj.GrhIndex);
                    withBlock.WriteInteger(obj.LingH);
                    withBlock.WriteInteger(obj.LingP);
                    withBlock.WriteInteger(obj.LingO);
                    withBlock.WriteInteger(Declaraciones.ArmasHerrero[validIndexes[i]]);
                    withBlock.WriteInteger(obj.Upgrade);
                }
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteBlacksmithArmors(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 04/15/2008 (NicoNZ) Habia un error al fijarse los skills del personaje
        // Writes the "BlacksmithArmors" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            int i;
            Declaraciones.ObjData obj;
            short[] validIndexes;
            var Count = default(short);
            validIndexes = new short[Declaraciones.ArmadurasHerrero.Length];
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.BlacksmithArmors);
                var loopTo = Declaraciones.ArmadurasHerrero.Length - 1;
                for (i = 1; i <= loopTo; i++)
                    if (Declaraciones.objData[Declaraciones.ArmadurasHerrero[i]].SkHerreria <= Math.Round(
                            Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Herreria] /
                            Trabajo.ModHerreriA(Declaraciones.UserList[UserIndex].clase), 0))
                    {
                        Count = Convert.ToInt16(Count + 1);
                        validIndexes[Count] = Convert.ToInt16(i);
                    }

                withBlock.WriteInteger(Count);
                var loopTo1 = (int)Count;
                for (i = 1; i <= loopTo1; i++)
                {
                    obj = Declaraciones.objData[Declaraciones.ArmadurasHerrero[validIndexes[i]]];
                    withBlock.WriteASCIIString(obj.name);
                    withBlock.WriteInteger(obj.GrhIndex);
                    withBlock.WriteInteger(obj.LingH);
                    withBlock.WriteInteger(obj.LingP);
                    withBlock.WriteInteger(obj.LingO);
                    withBlock.WriteInteger(Declaraciones.ArmadurasHerrero[validIndexes[i]]);
                    withBlock.WriteInteger(obj.Upgrade);
                }
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteCarpenterObjects(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "CarpenterObjects" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            int i;
            Declaraciones.ObjData obj;
            short[] validIndexes;
            var Count = default(short);
            validIndexes = new short[Declaraciones.ObjCarpintero.Length];
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.CarpenterObjects);
                var loopTo = Declaraciones.ObjCarpintero.Length - 1;
                for (i = 1; i <= loopTo; i++)
                    if (Declaraciones.objData[Declaraciones.ObjCarpintero[i]].SkCarpinteria <=
                        (short)(Declaraciones.UserList[UserIndex].Stats
                                    .UserSkills[(int)Declaraciones.eSkill.Carpinteria] /
                                Trabajo.ModCarpinteria(Declaraciones.UserList[UserIndex].clase)))
                    {
                        Count = Convert.ToInt16(Count + 1);
                        validIndexes[Count] = Convert.ToInt16(i);
                    }

                withBlock.WriteInteger(Count);
                var loopTo1 = (int)Count;
                for (i = 1; i <= loopTo1; i++)
                {
                    obj = Declaraciones.objData[Declaraciones.ObjCarpintero[validIndexes[i]]];
                    withBlock.WriteASCIIString(obj.name);
                    withBlock.WriteInteger(obj.GrhIndex);
                    withBlock.WriteInteger(obj.Madera);
                    withBlock.WriteInteger(obj.MaderaElfica);
                    withBlock.WriteInteger(Declaraciones.ObjCarpintero[validIndexes[i]]);
                    withBlock.WriteInteger(obj.Upgrade);
                }
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteRestOK(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "RestOK" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.RestOK),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteErrorMsg(short UserIndex, string message)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ErrorMsg" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteASCIIStringFixed(PrepareMessageErrorMsg(message)),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteBlind(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "Blind" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.Blind),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteDumb(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "Dumb" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.Dumb),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteShowSignal(short UserIndex, short ObjIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ShowSignal" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.ShowSignal);
                withBlock.WriteASCIIString(Declaraciones.objData[ObjIndex].texto);
                withBlock.WriteInteger(Declaraciones.objData[ObjIndex].GrhSecundario);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteChangeNPCInventorySlot(short UserIndex, byte Slot, Declaraciones.Obj Obj, float price)
    {
        RetryOnceIfNotEnoughSpace(() =>
        {
            var ObjInfo = default(Declaraciones.ObjData);
            if ((Obj.ObjIndex >= 0) & (Obj.ObjIndex <= Declaraciones.objData.Length - 1))
                ObjInfo = Declaraciones.objData[Obj.ObjIndex];
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.ChangeNPCInventorySlot);
                withBlock.WriteByte(Slot);
                withBlock.WriteASCIIString(ObjInfo.name);
                withBlock.WriteInteger(Obj.Amount);
                withBlock.WriteSingle(price);
                withBlock.WriteInteger(ObjInfo.GrhIndex);
                withBlock.WriteInteger(Obj.ObjIndex);
                withBlock.WriteByte(Convert.ToByte((int)ObjInfo.OBJType));
                withBlock.WriteInteger(ObjInfo.MaxHIT);
                withBlock.WriteInteger(ObjInfo.MinHIT);
                withBlock.WriteInteger(ObjInfo.MaxDef);
                withBlock.WriteInteger(ObjInfo.MinDef);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteUpdateHungerAndThirst(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "UpdateHungerAndThirst" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.UpdateHungerAndThirst);
                withBlock.WriteByte(Convert.ToByte(Declaraciones.UserList[UserIndex].Stats.MaxAGU));
                withBlock.WriteByte(Convert.ToByte(Declaraciones.UserList[UserIndex].Stats.MinAGU));
                withBlock.WriteByte(Convert.ToByte(Declaraciones.UserList[UserIndex].Stats.MaxHam));
                withBlock.WriteByte(Convert.ToByte(Declaraciones.UserList[UserIndex].Stats.MinHam));
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteFame(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "Fame" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.Fame);
                withBlock.WriteLong(Declaraciones.UserList[UserIndex].Reputacion.AsesinoRep);
                withBlock.WriteLong(Declaraciones.UserList[UserIndex].Reputacion.BandidoRep);
                withBlock.WriteLong(Declaraciones.UserList[UserIndex].Reputacion.BurguesRep);
                withBlock.WriteLong(Declaraciones.UserList[UserIndex].Reputacion.LadronesRep);
                withBlock.WriteLong(Declaraciones.UserList[UserIndex].Reputacion.NobleRep);
                withBlock.WriteLong(Declaraciones.UserList[UserIndex].Reputacion.PlebeRep);
                withBlock.WriteLong(Declaraciones.UserList[UserIndex].Reputacion.Promedio);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteMiniStats(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "MiniStats" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.MiniStats);
                withBlock.WriteLong(Declaraciones.UserList[UserIndex].Faccion.CiudadanosMatados);
                withBlock.WriteLong(Declaraciones.UserList[UserIndex].Faccion.CriminalesMatados);
                withBlock.WriteLong(Declaraciones.UserList[UserIndex].Stats.UsuariosMatados);
                withBlock.WriteInteger(Declaraciones.UserList[UserIndex].Stats.NPCsMuertos);
                withBlock.WriteByte(Convert.ToByte((int)Declaraciones.UserList[UserIndex].clase));
                withBlock.WriteLong(Declaraciones.UserList[UserIndex].Counters.Pena);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteLevelUp(short UserIndex, short skillPoints)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "LevelUp" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.LevelUp);
                withBlock.WriteInteger(skillPoints);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteAddForumMsg(short UserIndex, Declaraciones.eForumType ForumType, string Title,
        string Author, string message)
    {
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.AddForumMsg);
                withBlock.WriteByte((byte)ForumType);
                withBlock.WriteASCIIString(Title);
                withBlock.WriteASCIIString(Author);
                withBlock.WriteASCIIString(message);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteShowForumForm(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ShowForumForm" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            byte Visibilidad;
            var CanMakeSticky = default(byte);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                withBlock.outgoingData.WriteByte((byte)ServerPacketID.ShowForumForm);
                Visibilidad = Convert.ToByte((int)Declaraciones.eForumVisibility.ieGENERAL_MEMBER);
                if (Extra.esCaos(UserIndex) | Extra.EsGM(UserIndex))
                    Visibilidad = Convert.ToByte(Visibilidad | (int)Declaraciones.eForumVisibility.ieCAOS_MEMBER);
                if (Extra.esArmada(UserIndex) | Extra.EsGM(UserIndex))
                    Visibilidad = Convert.ToByte(Visibilidad | (int)Declaraciones.eForumVisibility.ieREAL_MEMBER);
                withBlock.outgoingData.WriteByte(Visibilidad);
                if (Extra.EsGM(UserIndex))
                    CanMakeSticky = 2;
                else if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.ChaosCouncil) != 0)
                    CanMakeSticky = 1;
                else if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.RoyalCouncil) != 0) CanMakeSticky = 1;
                withBlock.outgoingData.WriteByte(CanMakeSticky);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteSetInvisible(short UserIndex, short CharIndex, bool invisible)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "SetInvisible" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData
                .WriteASCIIStringFixed(PrepareMessageSetInvisible(CharIndex, invisible)), () => FlushBuffer(UserIndex));
    }

    public static void WriteDiceRoll(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "DiceRoll" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.DiceRoll);
                withBlock.WriteByte(Declaraciones.UserList[UserIndex].Stats
                    .UserAtributos[(int)Declaraciones.eAtributos.Fuerza]);
                withBlock.WriteByte(Declaraciones.UserList[UserIndex].Stats
                    .UserAtributos[(int)Declaraciones.eAtributos.Agilidad]);
                withBlock.WriteByte(Declaraciones.UserList[UserIndex].Stats
                    .UserAtributos[(int)Declaraciones.eAtributos.Inteligencia]);
                withBlock.WriteByte(Declaraciones.UserList[UserIndex].Stats
                    .UserAtributos[(int)Declaraciones.eAtributos.Carisma]);
                withBlock.WriteByte(Declaraciones.UserList[UserIndex].Stats
                    .UserAtributos[(int)Declaraciones.eAtributos.Constitucion]);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteMeditateToggle(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "MeditateToggle" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.MeditateToggle),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteBlindNoMore(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "BlindNoMore" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.BlindNoMore),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteDumbNoMore(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "DumbNoMore" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.DumbNoMore),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteSendSkills(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 11/19/09
        // Writes the "SendSkills" message to the given user's outgoing data buffer
        // 11/19/09: Pato - Now send the percentage of progress of the skills.
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            int i;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                withBlock.outgoingData.WriteByte((byte)ServerPacketID.SendSkills);
                withBlock.outgoingData.WriteByte(Convert.ToByte((int)withBlock.clase));
                for (i = 1; i <= Declaraciones.NUMSKILLS; i++)
                {
                    withBlock.outgoingData.WriteByte(Declaraciones.UserList[UserIndex].Stats.UserSkills[i]);
                    if (withBlock.Stats.UserSkills[i] < Declaraciones.MAXSKILLPOINTS)
                        withBlock.outgoingData.WriteByte(Convert.ToByte(Conversion.Int(withBlock.Stats.ExpSkills[i] *
                            100 / (double)withBlock.Stats.EluSkills[i])));
                    else
                        withBlock.outgoingData.WriteByte(0);
                }
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteTrainerCreatureList(short UserIndex, short NpcIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "TrainerCreatureList" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            int i;
            var str = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.TrainerCreatureList);
                var loopTo = (int)Declaraciones.Npclist[NpcIndex].NroCriaturas;
                for (i = 1; i <= loopTo; i++)
                    str = str + Declaraciones.Npclist[NpcIndex].Criaturas[i].NpcName + SEPARATOR;
                if (Migration.migr_LenB(str) > 0)
                    str = str.Substring(0, Math.Min(str.Length - 1, str.Length));
                withBlock.WriteASCIIString(str);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteGuildNews(short UserIndex, string guildNews, string[] enemies, string[] allies)
    {
        RetryOnceIfNotEnoughSpace(() =>
        {
            int i;
            var Tmp = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.guildNews);
                withBlock.WriteASCIIString(guildNews);
                var loopTo = enemies.Length - 1;
                for (i = 0; i <= loopTo; i++)
                    Tmp = Tmp + enemies[i] + SEPARATOR;
                if (Tmp.Length > 0)
                    Tmp = Tmp.Substring(0, Math.Min(Tmp.Length - 1, Tmp.Length));
                withBlock.WriteASCIIString(Tmp);
                Tmp = Constants.vbNullString;
                var loopTo1 = allies.Length - 1;
                for (i = 0; i <= loopTo1; i++)
                    Tmp = Tmp + allies[i] + SEPARATOR;
                if (Tmp.Length > 0)
                    Tmp = Tmp.Substring(0, Math.Min(Tmp.Length - 1, Tmp.Length));
                withBlock.WriteASCIIString(Tmp);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteOfferDetails(short UserIndex, string details)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "OfferDetails" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.OfferDetails);
                withBlock.WriteASCIIString(details);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteAlianceProposalsList(short UserIndex, string[] guilds)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "AlianceProposalsList" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            int i;
            var Tmp = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.AlianceProposalsList);
                var loopTo = guilds.Length - 1;
                for (i = 0; i <= loopTo; i++)
                    Tmp = Tmp + guilds[i] + SEPARATOR;
                if (Tmp.Length > 0)
                    Tmp = Tmp.Substring(0, Math.Min(Tmp.Length - 1, Tmp.Length));
                withBlock.WriteASCIIString(Tmp);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WritePeaceProposalsList(short UserIndex, string[] guilds)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "PeaceProposalsList" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            int i;
            var Tmp = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.PeaceProposalsList);
                var loopTo = guilds.Length - 1;
                for (i = 0; i <= loopTo; i++)
                    Tmp = Tmp + guilds[i] + SEPARATOR;
                if (Tmp.Length > 0)
                    Tmp = Tmp.Substring(0, Math.Min(Tmp.Length - 1, Tmp.Length));
                withBlock.WriteASCIIString(Tmp);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteCharacterInfo(short UserIndex, string charName, Declaraciones.eRaza race,
        Declaraciones.eClass characterClass, Declaraciones.eGenero gender, byte level, int gold, int bank,
        int reputation, string previousPetitions, string currentGuild, string previousGuilds, bool RoyalArmy,
        bool CaosLegion, int citicensKilled, int criminalsKilled)
    {
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.CharacterInfo);
                withBlock.WriteASCIIString(charName);
                withBlock.WriteByte((byte)race);
                withBlock.WriteByte(Convert.ToByte((int)characterClass));
                withBlock.WriteByte((byte)gender);
                withBlock.WriteByte(level);
                withBlock.WriteLong(gold);
                withBlock.WriteLong(bank);
                withBlock.WriteLong(reputation);
                withBlock.WriteASCIIString(previousPetitions);
                withBlock.WriteASCIIString(currentGuild);
                withBlock.WriteASCIIString(previousGuilds);
                withBlock.WriteBoolean(RoyalArmy);
                withBlock.WriteBoolean(CaosLegion);
                withBlock.WriteLong(citicensKilled);
                withBlock.WriteLong(criminalsKilled);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteGuildLeaderInfo(short UserIndex, string[] guildList, string[] MemberList, string guildNews,
        string[] joinRequests)
    {
        RetryOnceIfNotEnoughSpace(() =>
        {
            int i;
            var Tmp = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.GuildLeaderInfo);
                var loopTo = guildList.Length - 1;
                for (i = 0; i <= loopTo; i++)
                    Tmp = Tmp + guildList[i] + SEPARATOR;
                if (Tmp.Length > 0)
                    Tmp = Tmp.Substring(0, Math.Min(Tmp.Length - 1, Tmp.Length));
                withBlock.WriteASCIIString(Tmp);
                Tmp = Constants.vbNullString;
                var loopTo1 = MemberList.Length - 1;
                for (i = 0; i <= loopTo1; i++)
                    Tmp = Tmp + MemberList[i] + SEPARATOR;
                if (Tmp.Length > 0)
                    Tmp = Tmp.Substring(0, Math.Min(Tmp.Length - 1, Tmp.Length));
                withBlock.WriteASCIIString(Tmp);
                withBlock.WriteASCIIString(guildNews);
                Tmp = Constants.vbNullString;
                var loopTo2 = joinRequests.Length - 1;
                for (i = 0; i <= loopTo2; i++)
                    Tmp = Tmp + joinRequests[i] + SEPARATOR;
                if (Tmp.Length > 0)
                    Tmp = Tmp.Substring(0, Math.Min(Tmp.Length - 1, Tmp.Length));
                withBlock.WriteASCIIString(Tmp);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteGuildMemberInfo(short UserIndex, string[] guildList, string[] MemberList)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 21/02/2010
        // Writes the "GuildMemberInfo" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            int i;
            var Tmp = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.GuildMemberInfo);
                var loopTo = guildList.Length - 1;
                for (i = 0; i <= loopTo; i++)
                    Tmp = Tmp + guildList[i] + SEPARATOR;
                if (Tmp.Length > 0)
                    Tmp = Tmp.Substring(0, Math.Min(Tmp.Length - 1, Tmp.Length));
                withBlock.WriteASCIIString(Tmp);
                Tmp = Constants.vbNullString;
                var loopTo1 = MemberList.Length - 1;
                for (i = 0; i <= loopTo1; i++)
                    Tmp = Tmp + MemberList[i] + SEPARATOR;
                if (Tmp.Length > 0)
                    Tmp = Tmp.Substring(0, Math.Min(Tmp.Length - 1, Tmp.Length));
                withBlock.WriteASCIIString(Tmp);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteGuildDetails(short UserIndex, string GuildName, string founder, string foundationDate,
        string leader, string URL, short memberCount, bool electionsOpen, string alignment, short enemiesCount,
        short AlliesCount, string antifactionPoints, string[] codex, string guildDesc)
    {
        RetryOnceIfNotEnoughSpace(() =>
        {
            int i;
            var temp = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.GuildDetails);
                withBlock.WriteASCIIString(GuildName);
                withBlock.WriteASCIIString(founder);
                withBlock.WriteASCIIString(foundationDate);
                withBlock.WriteASCIIString(leader);
                withBlock.WriteASCIIString(URL);
                withBlock.WriteInteger(memberCount);
                withBlock.WriteBoolean(electionsOpen);
                withBlock.WriteASCIIString(alignment);
                withBlock.WriteInteger(enemiesCount);
                withBlock.WriteInteger(AlliesCount);
                withBlock.WriteASCIIString(antifactionPoints);
                var loopTo = codex.Length - 1;
                for (i = 0; i <= loopTo; i++)
                    temp = temp + codex[i] + SEPARATOR;
                if (temp.Length > 1)
                    temp = temp.Substring(0, Math.Min(temp.Length - 1, temp.Length));
                withBlock.WriteASCIIString(temp);
                withBlock.WriteASCIIString(guildDesc);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteShowGuildAlign(short UserIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 14/12/2009
        // Writes the "ShowGuildAlign" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.ShowGuildAlign),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteShowGuildFundationForm(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ShowGuildFundationForm" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.ShowGuildFundationForm),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteParalizeOK(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 08/12/07
        // Last Modified By: Lucas Tavolaro Ortiz (Tavo)
        // Writes the "ParalizeOK" message to the given user's outgoing data buffer
        // And updates user position
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.ParalizeOK);
            WritePosUpdate(UserIndex);
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteShowUserRequest(short UserIndex, string details)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ShowUserRequest" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.ShowUserRequest);
                withBlock.WriteASCIIString(details);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteTradeOK(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "TradeOK" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.TradeOK),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteBankOK(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "BankOK" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.BankOK),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteChangeUserTradeSlot(short UserIndex, byte OfferSlot, short ObjIndex, int Amount)
    {
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.ChangeUserTradeSlot);
                withBlock.WriteByte(OfferSlot);
                withBlock.WriteInteger(ObjIndex);
                withBlock.WriteLong(Amount);
                if (ObjIndex > 0)
                {
                    withBlock.WriteInteger(Declaraciones.objData[ObjIndex].GrhIndex);
                    withBlock.WriteByte(Convert.ToByte((int)Declaraciones.objData[ObjIndex].OBJType));
                    withBlock.WriteInteger(Declaraciones.objData[ObjIndex].MaxHIT);
                    withBlock.WriteInteger(Declaraciones.objData[ObjIndex].MinHIT);
                    withBlock.WriteInteger(Declaraciones.objData[ObjIndex].MaxDef);
                    withBlock.WriteInteger(Declaraciones.objData[ObjIndex].MinDef);
                    withBlock.WriteLong(Convert.ToInt32(modSistemaComercio.SalePrice(ObjIndex)));
                    withBlock.WriteASCIIString(Declaraciones.objData[ObjIndex].name);
                }
                else
                {
                    withBlock.WriteInteger(0);
                    withBlock.WriteByte(0);
                    withBlock.WriteInteger(0);
                    withBlock.WriteInteger(0);
                    withBlock.WriteInteger(0);
                    withBlock.WriteInteger(0);
                    withBlock.WriteLong(0);
                    withBlock.WriteASCIIString("");
                }
            }
        }, () => FlushBuffer(UserIndex)); // Borra el item
    }

    public static void WriteSendNight(short UserIndex, bool night)
    {
        // ***************************************************
        // Author: Fredy Horacio Treboux (liquid)
        // Last Modification: 01/08/07
        // Writes the "SendNight" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.SendNight);
                withBlock.WriteBoolean(night);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteSpawnList(short UserIndex, string[] npcNames)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "SpawnList" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            int i;
            var Tmp = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.SpawnList);
                var loopTo = npcNames.Length - 1;
                for (i = 0; i <= loopTo; i++)
                    Tmp = Tmp + npcNames[i] + SEPARATOR;
                if (Tmp.Length > 0)
                    Tmp = Tmp.Substring(0, Math.Min(Tmp.Length - 1, Tmp.Length));
                withBlock.WriteASCIIString(Tmp);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteShowSOSForm(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ShowSOSForm" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            int i;
            var Tmp = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.ShowSOSForm);
                var loopTo = Declaraciones.Ayuda.Longitud - 1;
                for (i = 0; i <= loopTo; i++)
                    Tmp = Tmp + Declaraciones.Ayuda.VerElemento(Convert.ToInt16(i)) + SEPARATOR;
                if (Migration.migr_LenB(Tmp) != 0)
                    Tmp = Tmp.Substring(0, Math.Min(Tmp.Length - 1, Tmp.Length));
                withBlock.WriteASCIIString(Tmp);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteShowPartyForm(short UserIndex)
    {
        // ***************************************************
        // Author: Budi
        // Last Modification: 11/26/09
        // Writes the "ShowPartyForm" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            int i;
            var Tmp = default(string);
            short PI;
            var members = new short[mdParty.PARTY_MAXMEMBERS + 1];
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.ShowPartyForm);
                PI = Declaraciones.UserList[UserIndex].PartyIndex;
                withBlock.WriteByte(Convert.ToByte(Declaraciones.Parties[PI].EsPartyLeader(UserIndex)));
                if (PI > 0)
                {
                    Declaraciones.Parties[PI].ObtenerMiembrosOnline(ref members);
                    for (i = 1; i <= mdParty.PARTY_MAXMEMBERS; i++)
                        if (members[i] > 0)
                            Tmp = Tmp + Declaraciones.UserList[members[i]].name + " (" +
                                  Conversion.Fix(Declaraciones.Parties[PI].MiExperiencia(members[i])) + ")" + SEPARATOR;
                }

                if (Migration.migr_LenB(Tmp) != 0)
                    Tmp = Tmp.Substring(0, Math.Min(Tmp.Length - 1, Tmp.Length));
                withBlock.WriteASCIIString(Tmp);
                withBlock.WriteLong(Declaraciones.Parties[PI].ObtenerExperienciaTotal());
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteShowMOTDEditionForm(short UserIndex, string currentMOTD)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ShowMOTDEditionForm" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.ShowMOTDEditionForm);
                withBlock.WriteASCIIString(currentMOTD);
            }
        }, () => FlushBuffer(UserIndex));
    }

    public static void WriteShowGMPanelForm(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "ShowGMPanelForm" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.ShowGMPanelForm),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteUserNameList(short UserIndex, string[] userNamesList, short cant)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06 NIGO:
        // Writes the "UserNameList" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            int i;
            var Tmp = default(string);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.UserNameList);
                var loopTo = (int)cant;
                for (i = 1; i <= loopTo; i++)
                    Tmp = Tmp + userNamesList[i] + SEPARATOR;
                if (Tmp.Length > 0)
                    Tmp = Tmp.Substring(0, Math.Min(Tmp.Length - 1, Tmp.Length));
                withBlock.WriteASCIIString(Tmp);
            }
        }, () => FlushBuffer(UserIndex));
    }


    // '
    // Writes the "Pong" message to the given user's outgoing data buffer.
    // 
    // @param    UserIndex User to which the message is intended.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static void WritePong(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "Pong" message to the given user's outgoing data buffer
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.Pong),
            () => FlushBuffer(UserIndex));
    }

    // '
    // Flushes the outgoing data buffer of the user.
    // 
    // @param    UserIndex User whose outgoing data buffer will be flushed.

    public static void FlushBuffer(short UserIndex)
    {
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Sends all data existing in the buffer
        // ***************************************************
        string sndData;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
            if (withBlock.length == 0)
                return;

            sndData = withBlock.ReadASCIIStringFixed(withBlock.length);

            TCP.EnviarDatosASlot(UserIndex, ref sndData);
        }
    }

    // '
    // Prepares the "SetInvisible" message and returns it.
    // 
    // @param    CharIndex The char turning visible / invisible.
    // @param    invisible True if the char is no longer visible, False otherwise.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The message is written to no outgoing buffer, but only prepared in a single string to be easily sent to several clients.

    public static string PrepareMessageSetInvisible(short CharIndex, bool invisible)
    {
        string PrepareMessageSetInvisibleRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Prepares the "SetInvisible" message and returns it.
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.SetInvisible);

            withBlock.WriteInteger(CharIndex);
            withBlock.WriteBoolean(invisible);

            PrepareMessageSetInvisibleRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageSetInvisibleRet;
    }

    public static string PrepareMessageCharacterChangeNick(short CharIndex, string newNick)
    {
        string PrepareMessageCharacterChangeNickRet = default;
        // ***************************************************
        // Author: Budi
        // Last Modification: 07/23/09
        // Prepares the "Change Nick" message and returns it.
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.CharacterChangeNick);

            withBlock.WriteInteger(CharIndex);
            withBlock.WriteASCIIString(newNick);

            PrepareMessageCharacterChangeNickRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageCharacterChangeNickRet;
    }

    // '
    // Prepares the "ChatOverHead" message and returns it.
    // 
    // @param    Chat Text to be displayed over the char's head.
    // @param    CharIndex The character uppon which the chat will be displayed.
    // @param    Color The color to be used when displaying the chat.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The message is written to no outgoing buffer, but only prepared in a single string to be easily sent to several clients.

    public static string PrepareMessageChatOverHead(string Chat, short CharIndex, int color)
    {
        string PrepareMessageChatOverHeadRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Prepares the "ChatOverHead" message and returns it.
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.ChatOverHead);
            withBlock.WriteASCIIString(Chat);
            withBlock.WriteInteger(CharIndex);

            // Write rgb channels and save one byte from long :D
            withBlock.WriteByte(Convert.ToByte(color & 0xFF));
            withBlock.WriteByte(Convert.ToByte((color & 0xFF00) / 0x100));
            withBlock.WriteByte(Convert.ToByte((color & 0xFF0000) / 0x10000));

            PrepareMessageChatOverHeadRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageChatOverHeadRet;
    }

    // '
    // Prepares the "ConsoleMsg" message and returns it.
    // 
    // @param    Chat Text to be displayed over the char's head.
    // @param    FontIndex Index of the FONTTYPE structure to use.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessageConsoleMsg(string Chat, FontTypeNames FontIndex)
    {
        string PrepareMessageConsoleMsgRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Prepares the "ConsoleMsg" message and returns it.
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.ConsoleMsg);
            withBlock.WriteASCIIString(Chat);
            withBlock.WriteByte(Convert.ToByte((int)FontIndex));

            PrepareMessageConsoleMsgRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageConsoleMsgRet;
    }

    public static string PrepareCommerceConsoleMsg(ref string Chat, FontTypeNames FontIndex)
    {
        string PrepareCommerceConsoleMsgRet = default;
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 03/12/2009
        // Prepares the "CommerceConsoleMsg" message and returns it.
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.CommerceChat);
            withBlock.WriteASCIIString(Chat);
            withBlock.WriteByte(Convert.ToByte((int)FontIndex));

            PrepareCommerceConsoleMsgRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareCommerceConsoleMsgRet;
    }

    // '
    // Prepares the "CreateFX" message and returns it.
    // 
    // @param    UserIndex User to which the message is intended.
    // @param    CharIndex Character upon which the FX will be created.
    // @param    FX FX index to be displayed over the new character.
    // @param    FXLoops Number of times the FX should be rendered.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessageCreateFX(short CharIndex, short FX, short FXLoops)
    {
        string PrepareMessageCreateFXRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Prepares the "CreateFX" message and returns it
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.CreateFX);
            withBlock.WriteInteger(CharIndex);
            withBlock.WriteInteger(FX);
            withBlock.WriteInteger(FXLoops);

            PrepareMessageCreateFXRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageCreateFXRet;
    }

    // '
    // Prepares the "PlayWave" message and returns it.
    // 
    // @param    wave The wave to be played.
    // @param    X The X position in map coordinates from where the sound comes.
    // @param    Y The Y position in map coordinates from where the sound comes.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessagePlayWave(byte wave, byte X, byte Y)
    {
        string PrepareMessagePlayWaveRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 08/08/07
        // Last Modified by: Rapsodius
        // Added X and Y positions for 3D Sounds
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.PlayWave);
            withBlock.WriteByte(wave);
            withBlock.WriteByte(X);
            withBlock.WriteByte(Y);

            PrepareMessagePlayWaveRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessagePlayWaveRet;
    }

    // '
    // Prepares the "GuildChat" message and returns it.
    // 
    // @param    Chat Text to be displayed over the char's head.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessageGuildChat(string Chat)
    {
        string PrepareMessageGuildChatRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Prepares the "GuildChat" message and returns it
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.GuildChat);
            withBlock.WriteASCIIString(Chat);

            PrepareMessageGuildChatRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageGuildChatRet;
    }

    // '
    // Prepares the "ShowMessageBox" message and returns it.
    // 
    // @param    Message Text to be displayed in the message box.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessageShowMessageBox(string Chat)
    {
        string PrepareMessageShowMessageBoxRet = default;
        // ***************************************************
        // Author: Fredy Horacio Treboux (liquid)
        // Last Modification: 01/08/07
        // Prepares the "ShowMessageBox" message and returns it
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.ShowMessageBox);
            withBlock.WriteASCIIString(Chat);

            PrepareMessageShowMessageBoxRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageShowMessageBoxRet;
    }


    // '
    // Prepares the "PlayMidi" message and returns it.
    // 
    // @param    midi The midi to be played.
    // @param    loops Number of repets for the midi.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessagePlayMidi(byte midi, short loops = -1)
    {
        string PrepareMessagePlayMidiRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Prepares the "GuildChat" message and returns it
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.PlayMidi);
            withBlock.WriteByte(midi);
            withBlock.WriteInteger(loops);

            PrepareMessagePlayMidiRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessagePlayMidiRet;
    }

    // '
    // Prepares the "PauseToggle" message and returns it.
    // 
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessagePauseToggle()
    {
        string PrepareMessagePauseToggleRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Prepares the "PauseToggle" message and returns it
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.PauseToggle);
            PrepareMessagePauseToggleRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessagePauseToggleRet;
    }

    // '
    // Prepares the "RainToggle" message and returns it.
    // 
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessageRainToggle()
    {
        string PrepareMessageRainToggleRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Prepares the "RainToggle" message and returns it
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.RainToggle);

            PrepareMessageRainToggleRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageRainToggleRet;
    }

    // '
    // Prepares the "ObjectDelete" message and returns it.
    // 
    // @param    X X coord of the character's new position.
    // @param    Y Y coord of the character's new position.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessageObjectDelete(byte X, byte Y)
    {
        string PrepareMessageObjectDeleteRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Prepares the "ObjectDelete" message and returns it
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.ObjectDelete);
            withBlock.WriteByte(X);
            withBlock.WriteByte(Y);

            PrepareMessageObjectDeleteRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageObjectDeleteRet;
    }

    // '
    // Prepares the "BlockPosition" message and returns it.
    // 
    // @param    X X coord of the tile to block/unblock.
    // @param    Y Y coord of the tile to block/unblock.
    // @param    Blocked Blocked status of the tile
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessageBlockPosition(byte X, byte Y, bool Blocked)
    {
        string PrepareMessageBlockPositionRet = default;
        // ***************************************************
        // Author: Fredy Horacio Treboux (liquid)
        // Last Modification: 01/08/07
        // Prepares the "BlockPosition" message and returns it
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.BlockPosition);
            withBlock.WriteByte(X);
            withBlock.WriteByte(Y);
            withBlock.WriteBoolean(Blocked);

            PrepareMessageBlockPositionRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageBlockPositionRet;
    }

    // '
    // Prepares the "ObjectCreate" message and returns it.
    // 
    // @param    GrhIndex Grh of the object.
    // @param    X X coord of the character's new position.
    // @param    Y Y coord of the character's new position.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessageObjectCreate(short GrhIndex, byte X, byte Y)
    {
        string PrepareMessageObjectCreateRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // prepares the "ObjectCreate" message and returns it
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.ObjectCreate);
            withBlock.WriteByte(X);
            withBlock.WriteByte(Y);
            withBlock.WriteInteger(GrhIndex);

            PrepareMessageObjectCreateRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageObjectCreateRet;
    }

    // '
    // Prepares the "CharacterRemove" message and returns it.
    // 
    // @param    CharIndex Character to be removed.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessageCharacterRemove(short CharIndex)
    {
        string PrepareMessageCharacterRemoveRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Prepares the "CharacterRemove" message and returns it
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.CharacterRemove);
            withBlock.WriteInteger(CharIndex);

            PrepareMessageCharacterRemoveRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageCharacterRemoveRet;
    }

    // '
    // Prepares the "RemoveCharDialog" message and returns it.
    // 
    // @param    CharIndex Character whose dialog will be removed.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessageRemoveCharDialog(short CharIndex)
    {
        string PrepareMessageRemoveCharDialogRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Writes the "RemoveCharDialog" message to the given user's outgoing data buffer
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.RemoveCharDialog);
            withBlock.WriteInteger(CharIndex);

            PrepareMessageRemoveCharDialogRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageRemoveCharDialogRet;
    }

    // '
    // Writes the "CharacterCreate" message to the given user's outgoing data buffer.
    // 
    // @param    body Body index of the new character.
    // @param    head Head index of the new character.
    // @param    heading Heading in which the new character is looking.
    // @param    CharIndex The index of the new character.
    // @param    X X coord of the new character's position.
    // @param    Y Y coord of the new character's position.
    // @param    weapon Weapon index of the new character.
    // @param    shield Shield index of the new character.
    // @param    FX FX index to be displayed over the new character.
    // @param    FXLoops Number of times the FX should be rendered.
    // @param    helmet Helmet index of the new character.
    // @param    name Name of the new character.
    // @param    NickColor Determines if the character is a criminal or not, and if can be atacked by someone
    // @param    privileges Sets if the character is a normal one or any kind of administrative character.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessageCharacterCreate(short body, short Head, Declaraciones.eHeading heading,
        short CharIndex, byte X, byte Y, short weapon, short shield, short FX, short FXLoops, short helmet, string name,
        byte NickColor, byte Privileges)
    {
        string PrepareMessageCharacterCreateRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Prepares the "CharacterCreate" message and returns it
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.CharacterCreate);

            withBlock.WriteInteger(CharIndex);
            withBlock.WriteInteger(body);
            withBlock.WriteInteger(Head);
            withBlock.WriteByte((byte)heading);
            withBlock.WriteByte(X);
            withBlock.WriteByte(Y);
            withBlock.WriteInteger(weapon);
            withBlock.WriteInteger(shield);
            withBlock.WriteInteger(helmet);
            withBlock.WriteInteger(FX);
            withBlock.WriteInteger(FXLoops);
            withBlock.WriteASCIIString(name);
            withBlock.WriteByte(NickColor);
            withBlock.WriteByte(Privileges);

            PrepareMessageCharacterCreateRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageCharacterCreateRet;
    }

    // '
    // Prepares the "CharacterChange" message and returns it.
    // 
    // @param    body Body index of the new character.
    // @param    head Head index of the new character.
    // @param    heading Heading in which the new character is looking.
    // @param    CharIndex The index of the new character.
    // @param    weapon Weapon index of the new character.
    // @param    shield Shield index of the new character.
    // @param    FX FX index to be displayed over the new character.
    // @param    FXLoops Number of times the FX should be rendered.
    // @param    helmet Helmet index of the new character.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessageCharacterChange(short body, short Head, Declaraciones.eHeading heading,
        short CharIndex, short weapon, short shield, short FX, short FXLoops, short helmet)
    {
        string PrepareMessageCharacterChangeRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Prepares the "CharacterChange" message and returns it
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.CharacterChange);

            withBlock.WriteInteger(CharIndex);
            withBlock.WriteInteger(body);
            withBlock.WriteInteger(Head);
            withBlock.WriteByte((byte)heading);
            withBlock.WriteInteger(weapon);
            withBlock.WriteInteger(shield);
            withBlock.WriteInteger(helmet);
            withBlock.WriteInteger(FX);
            withBlock.WriteInteger(FXLoops);

            PrepareMessageCharacterChangeRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageCharacterChangeRet;
    }

    // '
    // Prepares the "CharacterMove" message and returns it.
    // 
    // @param    CharIndex Character which is moving.
    // @param    X X coord of the character's new position.
    // @param    Y Y coord of the character's new position.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessageCharacterMove(short CharIndex, byte X, byte Y)
    {
        string PrepareMessageCharacterMoveRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Prepares the "CharacterMove" message and returns it
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.CharacterMove);
            withBlock.WriteInteger(CharIndex);
            withBlock.WriteByte(X);
            withBlock.WriteByte(Y);

            PrepareMessageCharacterMoveRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageCharacterMoveRet;
    }

    public static string PrepareMessageForceCharMove(Declaraciones.eHeading Direccion)
    {
        string PrepareMessageForceCharMoveRet = default;
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 26/03/2009
        // Prepares the "ForceCharMove" message and returns it
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.ForceCharMove);
            withBlock.WriteByte((byte)Direccion);

            PrepareMessageForceCharMoveRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageForceCharMoveRet;
    }

    // '
    // Prepares the "UpdateTagAndStatus" message and returns it.
    // 
    // @param    CharIndex Character which is moving.
    // @param    X X coord of the character's new position.
    // @param    Y Y coord of the character's new position.
    // @return   The formated message ready to be writen as is on outgoing buffers.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessageUpdateTagAndStatus(short UserIndex, byte NickColor, ref string Tag)
    {
        string PrepareMessageUpdateTagAndStatusRet = default;
        // ***************************************************
        // Author: Alejandro Salvo (Salvito)
        // Last Modification: 04/07/07
        // Last Modified By: Juan Martín Sotuyo Dodero (Maraxus)
        // Prepares the "UpdateTagAndStatus" message and returns it
        // 15/01/2010: ZaMa - Now sends the nick color instead of the status.
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.UpdateTagAndStatus);

            withBlock.WriteInteger(Declaraciones.UserList[UserIndex].character.CharIndex);
            withBlock.WriteByte(NickColor);
            withBlock.WriteASCIIString(Tag);

            PrepareMessageUpdateTagAndStatusRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageUpdateTagAndStatusRet;
    }

    // '
    // Prepares the "ErrorMsg" message and returns it.
    // 
    // @param    message The error message to be displayed.
    // @remarks  The data is not actually sent until the buffer is properly flushed.

    public static string PrepareMessageErrorMsg(string message)
    {
        string PrepareMessageErrorMsgRet = default;
        // ***************************************************
        // Author: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Prepares the "ErrorMsg" message and returns it
        // ***************************************************
        {
            ref var withBlock = ref auxiliarBuffer;
            withBlock.WriteByte((byte)ServerPacketID.ErrorMsg);
            withBlock.WriteASCIIString(message);

            PrepareMessageErrorMsgRet = withBlock.ReadASCIIStringFixed(withBlock.length);
        }

        return PrepareMessageErrorMsgRet;
    }

    // '
    // Writes the "StopWorking" message to the given user's outgoing data buffer.
    // 
    // @param    UserIndex User to which the message is intended.
    public static void WriteStopWorking(short UserIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 21/02/2010
        // 
        // ***************************************************
        RetryOnceIfNotEnoughSpace(
            () => Declaraciones.UserList[UserIndex].outgoingData.WriteByte((byte)ServerPacketID.StopWorking),
            () => FlushBuffer(UserIndex));
    }

    public static void WriteCancelOfferItem(short UserIndex, byte Slot)
    {
        // ***************************************************
        // Author: Torres Patricio (Pato)
        // Last Modification: 05/03/2010
        // 
        // ***************************************************
        RetryOnceIfNotEnoughSpace(() =>
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].outgoingData;
                withBlock.WriteByte((byte)ServerPacketID.CancelOfferItem);
                withBlock.WriteByte(Slot);
            }
        }, () => FlushBuffer(UserIndex));
    }

    private static void RetryOnceIfNotEnoughSpace(Action tryAction, Action onRetry)
    {
        try
        {
            tryAction();
        }
        catch (Exception ex)
        {
            onRetry();
            tryAction(); // Reintenta una sola vez
        }
    }


    private enum ServerPacketID : byte
    {
        Logged, // LOGGED
        RemoveDialogs, // QTDL
        RemoveCharDialog, // QDL
        NavigateToggle, // NAVEG
        Disconnect, // FINOK
        CommerceEnd, // FINCOMOK
        BankEnd, // FINBANOK
        CommerceInit, // INITCOM
        BankInit, // INITBANCO
        UserCommerceInit, // INITCOMUSU
        UserCommerceEnd, // FINCOMUSUOK
        UserOfferConfirm,
        CommerceChat,
        ShowBlacksmithForm, // SFH
        ShowCarpenterForm, // SFC
        UpdateSta, // ASS
        UpdateMana, // ASM
        UpdateHP, // ASH
        UpdateGold, // ASG
        UpdateBankGold,
        UpdateExp, // ASE
        ChangeMap, // CM
        PosUpdate, // PU
        ChatOverHead, // ||
        ConsoleMsg, // || - Beware!! its the same as above, but it was properly splitted
        GuildChat, // |+
        ShowMessageBox, // !!
        UserIndexInServer, // IU
        UserCharIndexInServer, // IP
        CharacterCreate, // CC
        CharacterRemove, // BP
        CharacterChangeNick,
        CharacterMove, // MP, +, * and _ '
        ForceCharMove,
        CharacterChange, // CP
        ObjectCreate, // HO
        ObjectDelete, // BO
        BlockPosition, // BQ
        PlayMidi, // TM
        PlayWave, // TW
        guildList, // GL
        AreaChanged, // CA
        PauseToggle, // BKW
        RainToggle, // LLU
        CreateFX, // CFX
        UpdateUserStats, // EST
        WorkRequestTarget, // T01
        ChangeInventorySlot, // CSI
        ChangeBankSlot, // SBO
        ChangeSpellSlot, // SHS
        Atributes, // ATR
        BlacksmithWeapons, // LAH
        BlacksmithArmors, // LAR
        CarpenterObjects, // OBR
        RestOK, // DOK
        ErrorMsg, // ERR
        Blind, // CEGU
        Dumb, // DUMB
        ShowSignal, // MCAR
        ChangeNPCInventorySlot, // NPCI
        UpdateHungerAndThirst, // EHYS
        Fame, // FAMA
        MiniStats, // MEST
        LevelUp, // SUNI
        AddForumMsg, // FMSG
        ShowForumForm, // MFOR
        SetInvisible, // NOVER
        DiceRoll, // DADOS
        MeditateToggle, // MEDOK
        BlindNoMore, // NSEGUE
        DumbNoMore, // NESTUP
        SendSkills, // SKILLS
        TrainerCreatureList, // LSTCRI
        guildNews, // GUILDNE
        OfferDetails, // PEACEDE & ALLIEDE
        AlianceProposalsList, // ALLIEPR
        PeaceProposalsList, // PEACEPR
        CharacterInfo, // CHRINFO
        GuildLeaderInfo, // LEADERI
        GuildMemberInfo,
        GuildDetails, // CLANDET
        ShowGuildFundationForm, // SHOWFUN
        ParalizeOK, // PARADOK
        ShowUserRequest, // PETICIO
        TradeOK, // TRANSOK
        BankOK, // BANCOOK
        ChangeUserTradeSlot, // COMUSUINV
        SendNight, // NOC
        Pong,
        UpdateTagAndStatus,

        // GM messages
        SpawnList, // SPL
        ShowSOSForm, // MSOS
        ShowMOTDEditionForm, // ZMOTD
        ShowGMPanelForm, // ABPANEL
        UserNameList, // LISTUSU
        ShowGuildAlign,
        ShowPartyForm,
        UpdateStrenghtAndDexterity,
        UpdateStrenght,
        UpdateDexterity,
        AddSlots,
        MultiMessage,
        StopWorking,
        CancelOfferItem
    }

    private enum ClientPacketID : byte
    {
        LoginExistingChar, // OLOGIN
        ThrowDices, // TIRDAD
        LoginNewChar, // NLOGIN
        Talk, // ;
        Yell, // -
        Whisper, // \
        Walk, // M
        RequestPositionUpdate, // RPU
        Attack, // AT
        PickUp, // AG
        SafeToggle, // /SEG & SEG  (SEG's behaviour has to be coded in the client)
        ResuscitationSafeToggle,
        RequestGuildLeaderInfo, // GLINFO
        RequestAtributes, // ATR
        RequestFame, // FAMA
        RequestSkills, // ESKI
        RequestMiniStats, // FEST
        CommerceEnd, // FINCOM
        UserCommerceEnd, // FINCOMUSU
        UserCommerceConfirm,
        CommerceChat,
        BankEnd, // FINBAN
        UserCommerceOk, // COMUSUOK
        UserCommerceReject, // COMUSUNO
        Drop, // TI
        CastSpell, // LH
        LeftClick, // LC
        DoubleClick, // RC
        Work, // UK
        UseSpellMacro, // UMH
        UseItem, // USA
        CraftBlacksmith, // CNS
        CraftCarpenter, // CNC
        WorkLeftClick, // WLC
        CreateNewGuild, // CIG
        SpellInfo, // INFS
        EquipItem, // EQUI
        ChangeHeading, // CHEA
        ModifySkills, // SKSE
        Train, // ENTR
        CommerceBuy, // COMP
        BankExtractItem, // RETI
        CommerceSell, // VEND
        BankDeposit, // DEPO
        ForumPost, // DEMSG
        MoveSpell, // DESPHE
        MoveBank,
        ClanCodexUpdate, // DESCOD
        UserCommerceOffer, // OFRECER
        GuildAcceptPeace, // ACEPPEAT
        GuildRejectAlliance, // RECPALIA
        GuildRejectPeace, // RECPPEAT
        GuildAcceptAlliance, // ACEPALIA
        GuildOfferPeace, // PEACEOFF
        GuildOfferAlliance, // ALLIEOFF
        GuildAllianceDetails, // ALLIEDET
        GuildPeaceDetails, // PEACEDET
        GuildRequestJoinerInfo, // ENVCOMEN
        GuildAlliancePropList, // ENVALPRO
        GuildPeacePropList, // ENVPROPP
        GuildDeclareWar, // DECGUERR
        GuildNewWebsite, // NEWWEBSI
        GuildAcceptNewMember, // ACEPTARI
        GuildRejectNewMember, // RECHAZAR
        GuildKickMember, // ECHARCLA
        GuildUpdateNews, // ACTGNEWS
        GuildMemberInfo, // 1HRINFO<
        GuildOpenElections, // ABREELEC
        GuildRequestMembership, // SOLICITUD
        GuildRequestDetails, // CLANDETAILS
        Online, // /ONLINE
        Quit, // /SALIR
        GuildLeave, // /SALIRCLAN
        RequestAccountState, // /BALANCE
        PetStand, // /QUIETO
        PetFollow, // /ACOMPAÑAR
        ReleasePet, // /LIBERAR
        TrainList, // /ENTRENAR
        Rest, // /DESCANSAR
        Meditate, // /MEDITAR
        Resucitate, // /RESUCITAR
        Heal, // /CURAR
        Help, // /AYUDA
        RequestStats, // /EST
        CommerceStart, // /COMERCIAR
        BankStart, // /BOVEDA
        Enlist, // /ENLISTAR
        Information, // /INFORMACION
        Reward, // /RECOMPENSA
        RequestMOTD, // /MOTD
        UpTime, // /UPTIME
        PartyLeave, // /SALIRPARTY
        PartyCreate, // /CREARPARTY
        PartyJoin, // /PARTY
        Inquiry, // /ENCUESTA ( with no params )
        GuildMessage, // /CMSG
        PartyMessage, // /PMSG
        CentinelReport, // /CENTINELA
        GuildOnline, // /ONLINECLAN
        PartyOnline, // /ONLINEPARTY
        CouncilMessage, // /BMSG
        RoleMasterRequest, // /ROL
        GMRequest, // /GM
        bugReport, // /_BUG
        ChangeDescription, // /DESC
        GuildVote, // /VOTO
        Punishments, // /PENAS
        ChangePassword, // /CONTRASEÑA
        Gamble, // /APOSTAR
        InquiryVote, // /ENCUESTA ( with parameters )
        LeaveFaction, // /RETIRAR ( with no arguments )
        BankExtractGold, // /RETIRAR ( with arguments )
        BankDepositGold, // /DEPOSITAR
        Denounce, // /DENUNCIAR
        GuildFundate, // /FUNDARCLAN
        GuildFundation,
        PartyKick, // /ECHARPARTY
        PartySetLeader, // /PARTYLIDER
        PartyAcceptMember, // /ACCEPTPARTY
        Ping, // /PING
        RequestPartyForm,
        ItemUpgrade,
        GMCommands,
        InitCrafting,
        Home,
        ShowGuildNews,
        ShareNpc, // /COMPARTIR
        StopSharingNpc,
        Consultation
    }
}