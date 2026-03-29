using System;
using Microsoft.VisualBasic;

namespace Legacy;

internal static class mdParty
{
    // '
    // SOPORTES PARA LAS PARTIES
    // (Ver este modulo como una clase abstracta "PartyManager")
    // 

    // '
    // cantidad maxima de parties en el servidor
    public const short MAX_PARTIES = 300;

    // '
    // nivel minimo para crear party
    public const byte MINPARTYLEVEL = 15;

    // '
    // Cantidad maxima de gente en la party
    public const byte PARTY_MAXMEMBERS = 5;

    // '
    // Si esto esta en True, la exp sale por cada golpe que le da
    // Si no, la exp la recibe al salirse de la party (pq las partys, floodean)
    public const bool PARTY_EXPERIENCIAPORGOLPE = false;

    // '
    // maxima diferencia de niveles permitida en una party
    public const byte MAXPARTYDELTALEVEL = 7;

    // '
    // distancia al leader para que este acepte el ingreso
    public const byte MAXDISTANCIAINGRESOPARTY = 2;

    // '
    // maxima distancia a un exito para obtener su experiencia
    public const byte PARTY_MAXDISTANCIA = 18;

    // '
    // restan las muertes de los miembros?
    public const bool CASTIGOS = false;

    // '
    // Numero al que elevamos el nivel de cada miembro de la party
    // Esto es usado para calcular la distribución de la experiencia entre los miembros
    // Se lee del archivo de balance
    public static float ExponenteNivelParty;


    public static short NextParty()
    {
        short NextPartyRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short i;
        NextPartyRet = -1;
        for (i = 1; i <= MAX_PARTIES; i++)
            if (Declaraciones.Parties[i] is null)
            {
                NextPartyRet = i;
                return NextPartyRet;
            }

        return NextPartyRet;
    }

    public static bool PuedeCrearParty(short UserIndex)
    {
        bool PuedeCrearPartyRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        PuedeCrearPartyRet = true;
        // If UserList(UserIndex).Stats.ELV < MINPARTYLEVEL Then

        if (Convert.ToInt16(
                Declaraciones.UserList[UserIndex].Stats.UserAtributos[(int)Declaraciones.eAtributos.Carisma]) *
            Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Liderazgo] < 100)
        {
            Protocol.WriteConsoleMsg(UserIndex, "Tu carisma y liderazgo no son suficientes para liderar una party.",
                Protocol.FontTypeNames.FONTTYPE_PARTY);
            PuedeCrearPartyRet = false;
        }
        else if (Declaraciones.UserList[UserIndex].flags.Muerto == 1)
        {
            Protocol.WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", Protocol.FontTypeNames.FONTTYPE_PARTY);
            PuedeCrearPartyRet = false;
        }

        return PuedeCrearPartyRet;
    }

    public static void CrearParty(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short tInt;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.PartyIndex == 0)
            {
                if (withBlock.flags.Muerto == 0)
                {
                    if (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Liderazgo] >= 5)
                    {
                        tInt = NextParty();
                        if (tInt == -1)
                        {
                            Protocol.WriteConsoleMsg(UserIndex, "Por el momento no se pueden crear más parties.",
                                Protocol.FontTypeNames.FONTTYPE_PARTY);
                        }
                        else
                        {
                            Declaraciones.Parties[tInt] = new clsParty();
                            if (!Declaraciones.Parties[tInt].NuevoMiembro(UserIndex))
                            {
                                Protocol.WriteConsoleMsg(UserIndex, "La party está llena, no puedes entrar.",
                                    Protocol.FontTypeNames.FONTTYPE_PARTY);
                                // UPGRADE_NOTE: El objeto Parties() no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
                                Declaraciones.Parties[tInt] = null;
                            }
                            else
                            {
                                Protocol.WriteConsoleMsg(UserIndex, "¡Has formado una party!",
                                    Protocol.FontTypeNames.FONTTYPE_PARTY);
                                withBlock.PartyIndex = tInt;
                                withBlock.PartySolicitud = 0;
                                if (!Declaraciones.Parties[tInt].HacerLeader(UserIndex))
                                    Protocol.WriteConsoleMsg(UserIndex, "No puedes hacerte líder.",
                                        Protocol.FontTypeNames.FONTTYPE_PARTY);
                                else
                                    Protocol.WriteConsoleMsg(UserIndex, "¡Te has convertido en líder de la party!",
                                        Protocol.FontTypeNames.FONTTYPE_PARTY);
                            }
                        }
                    }
                    else
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "No tienes suficientes puntos de liderazgo para liderar una party.",
                            Protocol.FontTypeNames.FONTTYPE_PARTY);
                    }
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", Protocol.FontTypeNames.FONTTYPE_PARTY);
                }
            }
            else
            {
                Protocol.WriteConsoleMsg(UserIndex, "Ya perteneces a una party.",
                    Protocol.FontTypeNames.FONTTYPE_PARTY);
            }
        }
    }

    public static void SolicitarIngresoAParty(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // ESTO ES enviado por el PJ para solicitar el ingreso a la party
        short tInt;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.PartyIndex > 0)
            {
                // si ya esta en una party
                Protocol.WriteConsoleMsg(UserIndex, "Ya perteneces a una party, escribe /SALIRPARTY para abandonarla",
                    Protocol.FontTypeNames.FONTTYPE_PARTY);
                withBlock.PartySolicitud = 0;
                return;
            }

            if (withBlock.flags.Muerto == 1)
            {
                Protocol.WriteConsoleMsg(UserIndex, "¡¡Estás muerto!!", Protocol.FontTypeNames.FONTTYPE_INFO);
                withBlock.PartySolicitud = 0;
                return;
            }

            tInt = withBlock.flags.TargetUser;
            if (tInt > 0)
            {
                if (Declaraciones.UserList[tInt].PartyIndex > 0)
                {
                    withBlock.PartySolicitud = Declaraciones.UserList[tInt].PartyIndex;
                    Protocol.WriteConsoleMsg(UserIndex, "El fundador decidirá si te acepta en la party.",
                        Protocol.FontTypeNames.FONTTYPE_PARTY);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex,
                        Declaraciones.UserList[tInt].name + " no es fundador de ninguna party.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    withBlock.PartySolicitud = 0;
                }
            }
            else
            {
                Protocol.WriteConsoleMsg(UserIndex,
                    "Para ingresar a una party debes hacer click sobre el fundador y luego escribir /PARTY",
                    Protocol.FontTypeNames.FONTTYPE_PARTY);
                withBlock.PartySolicitud = 0;
            }
        }
    }

    public static void SalirDeParty(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short PI;
        PI = Declaraciones.UserList[UserIndex].PartyIndex;
        if (PI > 0)
        {
            if (Declaraciones.Parties[PI].SaleMiembro(UserIndex))
                // sale el leader
                // UPGRADE_NOTE: El objeto Parties() no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
                Declaraciones.Parties[PI] = null;
            else
                Declaraciones.UserList[UserIndex].PartyIndex = 0;
        }
        else
        {
            Protocol.WriteConsoleMsg(UserIndex, "No eres miembro de ninguna party.",
                Protocol.FontTypeNames.FONTTYPE_INFO);
        }
    }

    public static void ExpulsarDeParty(short leader, short OldMember)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short PI;
        PI = Declaraciones.UserList[leader].PartyIndex;

        if (PI == Declaraciones.UserList[OldMember].PartyIndex)
        {
            if (Declaraciones.Parties[PI].SaleMiembro(OldMember))
                // si la funcion me da true, entonces la party se disolvio
                // y los partyindex fueron reseteados a 0
                // UPGRADE_NOTE: El objeto Parties() no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
                Declaraciones.Parties[PI] = null;
            else
                Declaraciones.UserList[OldMember].PartyIndex = 0;
        }
        else
        {
            Protocol.WriteConsoleMsg(leader,
                Declaraciones.UserList[OldMember].name.ToLower() + " no pertenece a tu party.",
                Protocol.FontTypeNames.FONTTYPE_INFO);
        }
    }

    // '
    // Determines if a user can use party commands like /acceptparty or not.
    // 
    // @param User Specifies reference to user
    // @return  True if the user can use party commands, false if not.
    public static bool UserPuedeEjecutarComandos(short User)
    {
        bool UserPuedeEjecutarComandosRet = default;
        // *************************************************
        // Author: Marco Vanotti(Marco)
        // Last modified: 05/05/09
        // 
        // *************************************************
        short PI;

        PI = Declaraciones.UserList[User].PartyIndex;

        if (PI > 0)
        {
            if (Declaraciones.Parties[PI].EsPartyLeader(User))
            {
                UserPuedeEjecutarComandosRet = true;
            }
            else
            {
                Protocol.WriteConsoleMsg(User, "¡No eres el líder de tu party!", Protocol.FontTypeNames.FONTTYPE_PARTY);
                return UserPuedeEjecutarComandosRet;
            }
        }
        else
        {
            Protocol.WriteConsoleMsg(User, "No eres miembro de ninguna party.", Protocol.FontTypeNames.FONTTYPE_INFO);
            return UserPuedeEjecutarComandosRet;
        }

        return UserPuedeEjecutarComandosRet;
    }

    public static void AprobarIngresoAParty(short leader, short NewMember)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 11/03/2010
        // 11/03/2010: ZaMa - Le avisa al lider si intenta aceptar a alguien que sea mimebro de su propia party.
        // ***************************************************

        // el UI es el leader
        short PI;
        var razon = default(string);

        PI = Declaraciones.UserList[leader].PartyIndex;

        {
            ref var withBlock = ref Declaraciones.UserList[NewMember];
            if (withBlock.PartySolicitud == PI)
            {
                if (!(withBlock.flags.Muerto == 1))
                {
                    if (withBlock.PartyIndex == 0)
                    {
                        if (Declaraciones.Parties[PI].PuedeEntrar(NewMember, ref razon))
                        {
                            if (Declaraciones.Parties[PI].NuevoMiembro(NewMember))
                            {
                                Declaraciones.Parties[PI].MandarMensajeAConsola(
                                    Declaraciones.UserList[leader].name + " ha aceptado a " + withBlock.name +
                                    " en la party.", "Servidor");
                                withBlock.PartyIndex = PI;
                                withBlock.PartySolicitud = 0;
                            }
                            else
                            {
                                // no pudo entrar
                                // ACA UNO PUEDE CODIFICAR OTRO TIPO DE ERRORES...
                                modSendData.SendData(modSendData.SendTarget.ToAdmins, leader,
                                    Protocol.PrepareMessageConsoleMsg(
                                        " Servidor> CATÁSTROFE EN PARTIES, NUEVOMIEMBRO DIO FALSE! :S ",
                                        Protocol.FontTypeNames.FONTTYPE_PARTY));
                            }
                        }
                        else
                        {
                            // no debe entrar
                            Protocol.WriteConsoleMsg(leader, razon, Protocol.FontTypeNames.FONTTYPE_PARTY);
                        }
                    }
                    else
                    {
                        if (withBlock.PartyIndex == PI)
                            Protocol.WriteConsoleMsg(leader, withBlock.name.ToLower() + " ya es miembro de la party.",
                                Protocol.FontTypeNames.FONTTYPE_PARTY);
                        else
                            Protocol.WriteConsoleMsg(leader, withBlock.name + " ya es miembro de otra party.",
                                Protocol.FontTypeNames.FONTTYPE_PARTY);
                    }
                }
                else
                {
                    Protocol.WriteConsoleMsg(leader, "¡Está muerto, no puedes aceptar miembros en ese estado!",
                        Protocol.FontTypeNames.FONTTYPE_PARTY);
                }
            }
            else
            {
                if (withBlock.PartyIndex == PI)
                    Protocol.WriteConsoleMsg(leader, withBlock.name.ToLower() + " ya es miembro de la party.",
                        Protocol.FontTypeNames.FONTTYPE_PARTY);
                else
                    Protocol.WriteConsoleMsg(leader,
                        withBlock.name.ToLower() + " no ha solicitado ingresar a tu party.",
                        Protocol.FontTypeNames.FONTTYPE_PARTY);
            }
        }
    }

    private static bool IsPartyMember(short UserIndex, short PartyIndex)
    {
        short MemberIndex;

        for (MemberIndex = 1; MemberIndex <= PARTY_MAXMEMBERS; MemberIndex++)
        {
        }

        return default;
    }

    public static void BroadCastParty(short UserIndex, ref string texto)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short PI;

        PI = Declaraciones.UserList[UserIndex].PartyIndex;

        if (PI > 0) Declaraciones.Parties[PI].MandarMensajeAConsola(texto, Declaraciones.UserList[UserIndex].name);
    }

    public static void OnlineParty(short UserIndex)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 11/27/09 (Budi)
        // Adapte la función a los nuevos métodos de clsParty
        // *************************************************
        short i;
        short PI;
        string Text;
        // UPGRADE_WARNING: El límite inferior de la matriz MembersOnline ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        var MembersOnline = new short[PARTY_MAXMEMBERS + 1];

        PI = Declaraciones.UserList[UserIndex].PartyIndex;

        if (PI > 0)
        {
            Declaraciones.Parties[PI].ObtenerMiembrosOnline(ref MembersOnline);
            Text = "Nombre(Exp): ";
            for (i = 1; i <= PARTY_MAXMEMBERS; i++)
                if (MembersOnline[i] > 0)
                    Text = Text + " - " + Declaraciones.UserList[MembersOnline[i]].name + " (" +
                           Conversion.Fix(Declaraciones.Parties[PI].MiExperiencia(MembersOnline[i])) + ")";

            Text = Text + ". Experiencia total: " + Declaraciones.Parties[PI].ObtenerExperienciaTotal();
            Protocol.WriteConsoleMsg(UserIndex, Text, Protocol.FontTypeNames.FONTTYPE_PARTY);
        }
    }


    public static void TransformarEnLider(short OldLeader, short NewLeader)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short PI;

        if (OldLeader == NewLeader)
            return;

        PI = Declaraciones.UserList[OldLeader].PartyIndex;

        if (PI == Declaraciones.UserList[NewLeader].PartyIndex)
        {
            if (Declaraciones.UserList[NewLeader].flags.Muerto == 0)
            {
                if (Declaraciones.Parties[PI].HacerLeader(NewLeader))
                    Declaraciones.Parties[PI].MandarMensajeAConsola(
                        "El nuevo líder de la party es " + Declaraciones.UserList[NewLeader].name,
                        Declaraciones.UserList[OldLeader].name);
                else
                    Protocol.WriteConsoleMsg(OldLeader, "¡No se ha hecho el cambio de mando!",
                        Protocol.FontTypeNames.FONTTYPE_PARTY);
            }
            else
            {
                Protocol.WriteConsoleMsg(OldLeader, "¡Está muerto!", Protocol.FontTypeNames.FONTTYPE_INFO);
            }
        }
        else
        {
            Protocol.WriteConsoleMsg(OldLeader,
                Declaraciones.UserList[NewLeader].name.ToLower() + " no pertenece a tu party.",
                Protocol.FontTypeNames.FONTTYPE_INFO);
        }
    }


    public static void ActualizaExperiencias()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // esta funcion se invoca antes de worlsaves, y apagar servidores
        // en caso que la experiencia sea acumulada y no por golpe
        // para que grabe los datos en los charfiles
        short i;

        if (!PARTY_EXPERIENCIAPORGOLPE)
        {
            Declaraciones.haciendoBK = true;
            modSendData.SendData(modSendData.SendTarget.ToAll, 0, Protocol.PrepareMessagePauseToggle());

            modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                Protocol.PrepareMessageConsoleMsg("Servidor> Distribuyendo experiencia en parties.",
                    Protocol.FontTypeNames.FONTTYPE_SERVER));
            for (i = 1; i <= MAX_PARTIES; i++)
                if (Declaraciones.Parties[i] is not null)
                    Declaraciones.Parties[i].FlushExperiencia();

            modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                Protocol.PrepareMessageConsoleMsg("Servidor> Experiencia distribuida.",
                    Protocol.FontTypeNames.FONTTYPE_SERVER));
            modSendData.SendData(modSendData.SendTarget.ToAll, 0, Protocol.PrepareMessagePauseToggle());
            Declaraciones.haciendoBK = false;
        }
    }

    public static void ObtenerExito(short UserIndex, int Exp, ref short mapa, ref short X, ref short Y)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if (Exp <= 0)
            if (!CASTIGOS)
                return;

        Declaraciones.Parties[Declaraciones.UserList[UserIndex].PartyIndex].ObtenerExito(Exp, mapa, ref X, ref Y);
    }

    public static short CantMiembros(short UserIndex)
    {
        short CantMiembrosRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        CantMiembrosRet = 0;
        if (Declaraciones.UserList[UserIndex].PartyIndex > 0)
            CantMiembrosRet = Declaraciones.Parties[Declaraciones.UserList[UserIndex].PartyIndex].CantMiembros();

        return CantMiembrosRet;
    }

    // '
    // Sets the new p_sumaniveleselevados to the party.
    // 
    // @param UserInidex Specifies reference to user
    // @remarks When a user level up and he is in a party, we call this sub to don't desestabilice the party exp formula
    public static void ActualizarSumaNivelesElevados(short UserIndex)
    {
        // *************************************************
        // Author: Marco Vanotti (MarKoxX)
        // Last modified: 28/10/08
        // 
        // *************************************************
        if (Declaraciones.UserList[UserIndex].PartyIndex > 0)
            Declaraciones.Parties[Declaraciones.UserList[UserIndex].PartyIndex]
                .UpdateSumaNivelesElevados(Declaraciones.UserList[UserIndex].Stats.ELV);
    }

    // '
    // tPartyMember
    // 
    // @param UserIndex UserIndex
    // @param Experiencia Experiencia
    // 
    public struct tPartyMember
    {
        public short UserIndex;
        public double Experiencia;
    }
}