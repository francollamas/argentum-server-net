using System;
using System.Drawing;
using System.IO;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Legacy;

internal static class TCP
{
    public static void DarCuerpo(short UserIndex)
    {
        // *************************************************
        // Author: Nacho (Integer)
        // Last modified: 14/03/2007
        // Elije una cabeza para el usuario y le da un body
        // *************************************************
        var NewBody = default(short);
        byte UserRaza;
        byte UserGenero;

        UserGenero = (byte)Declaraciones.UserList[UserIndex].Genero;
        UserRaza = (byte)Declaraciones.UserList[UserIndex].raza;

        switch (UserGenero)
        {
            case (byte)Declaraciones.eGenero.Hombre:
            {
                switch (UserRaza)
                {
                    case (byte)Declaraciones.eRaza.Humano:
                    {
                        NewBody = 1;
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Elfo:
                    {
                        NewBody = 2;
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Drow:
                    {
                        NewBody = 3;
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Enano:
                    {
                        NewBody = 300;
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Gnomo:
                    {
                        NewBody = 300;
                        break;
                    }
                }

                break;
            }
            case (byte)Declaraciones.eGenero.Mujer:
            {
                switch (UserRaza)
                {
                    case (byte)Declaraciones.eRaza.Humano:
                    {
                        NewBody = 1;
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Elfo:
                    {
                        NewBody = 2;
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Drow:
                    {
                        NewBody = 3;
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Gnomo:
                    {
                        NewBody = 300;
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Enano:
                    {
                        NewBody = 300;
                        break;
                    }
                }

                break;
            }
        }

        Declaraciones.UserList[UserIndex].character.body = NewBody;
    }

    private static bool ValidarCabeza(byte UserRaza, byte UserGenero, short Head)
    {
        bool ValidarCabezaRet = default;

        switch (UserGenero)
        {
            case (byte)Declaraciones.eGenero.Hombre:
            {
                switch (UserRaza)
                {
                    case (byte)Declaraciones.eRaza.Humano:
                    {
                        ValidarCabezaRet = (Head >= Declaraciones.HUMANO_H_PRIMER_CABEZA) &
                                           (Head <= Declaraciones.HUMANO_H_ULTIMA_CABEZA);
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Elfo:
                    {
                        ValidarCabezaRet = (Head >= Declaraciones.ELFO_H_PRIMER_CABEZA) &
                                           (Head <= Declaraciones.ELFO_H_ULTIMA_CABEZA);
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Drow:
                    {
                        ValidarCabezaRet = (Head >= Declaraciones.DROW_H_PRIMER_CABEZA) &
                                           (Head <= Declaraciones.DROW_H_ULTIMA_CABEZA);
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Enano:
                    {
                        ValidarCabezaRet = (Head >= Declaraciones.ENANO_H_PRIMER_CABEZA) &
                                           (Head <= Declaraciones.ENANO_H_ULTIMA_CABEZA);
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Gnomo:
                    {
                        ValidarCabezaRet = (Head >= Declaraciones.GNOMO_H_PRIMER_CABEZA) &
                                           (Head <= Declaraciones.GNOMO_H_ULTIMA_CABEZA);
                        break;
                    }
                }

                break;
            }

            case (byte)Declaraciones.eGenero.Mujer:
            {
                switch (UserRaza)
                {
                    case (byte)Declaraciones.eRaza.Humano:
                    {
                        ValidarCabezaRet = (Head >= Declaraciones.HUMANO_M_PRIMER_CABEZA) &
                                           (Head <= Declaraciones.HUMANO_M_ULTIMA_CABEZA);
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Elfo:
                    {
                        ValidarCabezaRet = (Head >= Declaraciones.ELFO_M_PRIMER_CABEZA) &
                                           (Head <= Declaraciones.ELFO_M_ULTIMA_CABEZA);
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Drow:
                    {
                        ValidarCabezaRet = (Head >= Declaraciones.DROW_M_PRIMER_CABEZA) &
                                           (Head <= Declaraciones.DROW_M_ULTIMA_CABEZA);
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Enano:
                    {
                        ValidarCabezaRet = (Head >= Declaraciones.ENANO_M_PRIMER_CABEZA) &
                                           (Head <= Declaraciones.ENANO_M_ULTIMA_CABEZA);
                        break;
                    }
                    case (byte)Declaraciones.eRaza.Gnomo:
                    {
                        ValidarCabezaRet = (Head >= Declaraciones.GNOMO_M_PRIMER_CABEZA) &
                                           (Head <= Declaraciones.GNOMO_M_ULTIMA_CABEZA);
                        break;
                    }
                }

                break;
            }
        }

        return ValidarCabezaRet;
    }

    public static bool AsciiValidos(string cad)
    {
        bool AsciiValidosRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        byte car;
        short i;

        cad = cad.ToLower();

        var loopTo = Convert.ToInt16(cad.Length);
        for (i = 1; i <= loopTo; i++)
        {
            car = Convert.ToByte(Strings.Asc(cad.Substring(i - 1, 1)));

            if (((car < 97) | (car > 122)) & (car != 255) & (car != 32))
            {
                AsciiValidosRet = false;
                return AsciiValidosRet;
            }
        }

        AsciiValidosRet = true;
        return AsciiValidosRet;
    }

    public static bool Numeric(string cad)
    {
        bool NumericRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        byte car;
        short i;

        cad = cad.ToLower();

        var loopTo = Convert.ToInt16(cad.Length);
        for (i = 1; i <= loopTo; i++)
        {
            car = Convert.ToByte(Strings.Asc(cad.Substring(i - 1, 1)));

            if ((car < 48) | (car > 57))
            {
                NumericRet = false;
                return NumericRet;
            }
        }

        NumericRet = true;
        return NumericRet;
    }


    public static bool NombrePermitido(string Nombre)
    {
        bool NombrePermitidoRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short i;

        var loopTo = Convert.ToInt16(Declaraciones.ForbidenNames.GetUpperBound(0));
        for (i = 0; i <= loopTo; i++)
            if (Nombre.IndexOf(Declaraciones.ForbidenNames[i]) >= 0)
            {
                NombrePermitidoRet = false;
                return NombrePermitidoRet;
            }

        NombrePermitidoRet = true;
        return NombrePermitidoRet;
    }

    public static bool ValidateSkills(short UserIndex)
    {
        bool ValidateSkillsRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short LoopC;

        for (LoopC = 1; LoopC <= Declaraciones.NUMSKILLS; LoopC++)
            if (Declaraciones.UserList[UserIndex].Stats.UserSkills[LoopC] < 0)
            {
                return ValidateSkillsRet;
                if (Declaraciones.UserList[UserIndex].Stats.UserSkills[LoopC] > 100)
                    Declaraciones.UserList[UserIndex].Stats.UserSkills[LoopC] = 100;
            }

        ValidateSkillsRet = true;
        return ValidateSkillsRet;
    }

    public static void ConnectNewUser(short UserIndex, ref string name, ref string Password,
        Declaraciones.eRaza UserRaza, Declaraciones.eGenero UserSexo, Declaraciones.eClass UserClase,
        ref string UserEmail, Declaraciones.eCiudad Hogar, short Head)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 3/12/2009
        // Conecta un nuevo Usuario
        // 23/01/2007 Pablo (ToxicWaste) - Agregué ResetFaccion al crear usuario
        // 24/01/2007 Pablo (ToxicWaste) - Agregué el nuevo mana inicial de los magos.
        // 12/02/2007 Pablo (ToxicWaste) - Puse + 1 de const al Elfo normal.
        // 20/04/2007 Pablo (ToxicWaste) - Puse -1 de fuerza al Elfo.
        // 09/01/2008 Pablo (ToxicWaste) - Ahora los modificadores de Raza se controlan desde Balance.dat
        // 11/19/2009: Pato - Modifico la maná inicial del bandido.
        // 11/19/2009: Pato - Asigno los valores iniciales de ExpSkills y EluSkills.
        // 03/12/2009: Budi - Optimización del código.
        // *************************************************
        int i;

        int MiInt;
        byte Slot;
        bool IsPaladin;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];

            if (!AsciiValidos(name) | (Migration.migr_LenB(name) == 0))
            {
                Protocol.WriteErrorMsg(UserIndex, "Nombre inválido.");
                return;
            }

            if (Declaraciones.UserList[UserIndex].flags.UserLogged)
            {
                var argtexto = "El usuario " + Declaraciones.UserList[UserIndex].name + " ha intentado crear a " +
                               name + " desde la IP " + Declaraciones.UserList[UserIndex].ip;
                General.LogCheating(ref argtexto);

                // Kick player ( and leave character inside :D )!
                CloseSocketSL(UserIndex);
                UsUaRiOs.Cerrar_Usuario(UserIndex);

                return;
            }

            // ¿Existe el personaje?
            if (File.Exists(Declaraciones.CharPath + name.ToUpper() + ".chr"))
            {
                Protocol.WriteErrorMsg(UserIndex, "Ya existe el personaje.");
                return;
            }

            // Tiró los dados antes de llegar acá??
            if (withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] == 0)
            {
                Protocol.WriteErrorMsg(UserIndex, "Debe tirar los dados antes de poder crear un personaje.");
                return;
            }

            if (!ValidarCabeza((byte)UserRaza, (byte)UserSexo, Head))
            {
                var argtexto1 = "El usuario " + name + " ha seleccionado la cabeza " + Head + " desde la IP " +
                                withBlock.ip;
                General.LogCheating(ref argtexto1);

                Protocol.WriteErrorMsg(UserIndex, "Cabeza inválida, elija una cabeza seleccionable.");
                return;
            }

            withBlock.flags.Muerto = 0;
            withBlock.flags.Escondido = 0;

            withBlock.Reputacion.AsesinoRep = 0;
            withBlock.Reputacion.BandidoRep = 0;
            withBlock.Reputacion.BurguesRep = 0;
            withBlock.Reputacion.LadronesRep = 0;
            withBlock.Reputacion.NobleRep = 1000;
            withBlock.Reputacion.PlebeRep = 30;

            withBlock.Reputacion.Promedio = 5;


            withBlock.name = name;
            withBlock.clase = UserClase;
            withBlock.raza = UserRaza;
            withBlock.Genero = UserSexo;
            withBlock.email = UserEmail;
            withBlock.Hogar = Hogar;

            // [Pablo (Toxic Waste) 9/01/08]
            withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] = Convert.ToByte(
                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] +
                Declaraciones.modRaza[(int)UserRaza].Fuerza);
            withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] = Convert.ToByte(
                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] +
                Declaraciones.modRaza[(int)UserRaza].Agilidad);
            withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Inteligencia] = Convert.ToByte(
                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Inteligencia] +
                Declaraciones.modRaza[(int)UserRaza].Inteligencia);
            withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Carisma] = Convert.ToByte(
                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Carisma] +
                Declaraciones.modRaza[(int)UserRaza].Carisma);
            withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Constitucion] = Convert.ToByte(
                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Constitucion] +
                Declaraciones.modRaza[(int)UserRaza].Constitucion);
            // [/Pablo (Toxic Waste)]

            for (i = 1; i <= Declaraciones.NUMSKILLS; i++)
            {
                withBlock.Stats.UserSkills[i] = 0;
                UsUaRiOs.CheckEluSkill(UserIndex, Convert.ToByte(i), true);
            }

            withBlock.Stats.SkillPts = 10;

            withBlock.character.heading = Declaraciones.eHeading.SOUTH;

            DarCuerpo(UserIndex);
            withBlock.character.Head = Head;

            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().OrigChar. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            withBlock.OrigChar = withBlock.character;

            MiInt = Matematicas.RandomNumber(1,
                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Constitucion] / 3);

            withBlock.Stats.MaxHp = Convert.ToInt16(15 + MiInt);
            withBlock.Stats.MinHp = Convert.ToInt16(15 + MiInt);

            MiInt = Matematicas.RandomNumber(1,
                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] / 6);
            if (MiInt == 1)
                MiInt = 2;

            withBlock.Stats.MaxSta = Convert.ToInt16(20 * MiInt);
            withBlock.Stats.MinSta = Convert.ToInt16(20 * MiInt);


            withBlock.Stats.MaxAGU = 100;
            withBlock.Stats.MinAGU = 100;

            withBlock.Stats.MaxHam = 100;
            withBlock.Stats.MinHam = 100;


            // <-----------------MANA----------------------->
            if (UserClase == Declaraciones.eClass.Mage) // Cambio en mana inicial (ToxicWaste)
            {
                MiInt = withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Inteligencia] * 3;
                withBlock.Stats.MaxMAN = Convert.ToInt16(MiInt);
                withBlock.Stats.MinMAN = Convert.ToInt16(MiInt);
            }
            else if ((UserClase == Declaraciones.eClass.Cleric) | (UserClase == Declaraciones.eClass.Druid) |
                     (UserClase == Declaraciones.eClass.Bard) | (UserClase == Declaraciones.eClass.Assasin))
            {
                withBlock.Stats.MaxMAN = 50;
                withBlock.Stats.MinMAN = 50;
            }
            else if (UserClase == Declaraciones.eClass.Bandit) // Mana Inicial del Bandido (ToxicWaste)
            {
                withBlock.Stats.MaxMAN = 50;
                withBlock.Stats.MinMAN = 50;
            }
            else
            {
                withBlock.Stats.MaxMAN = 0;
                withBlock.Stats.MinMAN = 0;
            }

            if ((UserClase == Declaraciones.eClass.Mage) | (UserClase == Declaraciones.eClass.Cleric) |
                (UserClase == Declaraciones.eClass.Druid) | (UserClase == Declaraciones.eClass.Bard) |
                (UserClase == Declaraciones.eClass.Assasin))
            {
                withBlock.Stats.UserHechizos[1] = 2;

                if (UserClase == Declaraciones.eClass.Druid)
                    withBlock.Stats.UserHechizos[2] = 46;
            }

            withBlock.Stats.MaxHIT = 2;
            withBlock.Stats.MinHIT = 1;

            withBlock.Stats.GLD = 0;

            withBlock.Stats.Exp = 0d;
            withBlock.Stats.ELU = 300;
            withBlock.Stats.ELV = 1;

            // ???????????????? INVENTARIO ¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿¿

            IsPaladin = UserClase == Declaraciones.eClass.Paladin;

            // Pociones Rojas (Newbie)
            Slot = 1;
            withBlock.Invent.userObj[Slot].ObjIndex = 857;
            withBlock.Invent.userObj[Slot].Amount = 200;

            // Pociones azules (Newbie)
            if ((withBlock.Stats.MaxMAN > 0) | IsPaladin)
            {
                Slot = Convert.ToByte(Slot + 1);
                withBlock.Invent.userObj[Slot].ObjIndex = 856;
                withBlock.Invent.userObj[Slot].Amount = 200;
            }

            else
            {
                // Pociones amarillas (Newbie)
                Slot = Convert.ToByte(Slot + 1);
                withBlock.Invent.userObj[Slot].ObjIndex = 855;
                withBlock.Invent.userObj[Slot].Amount = 100;

                // Pociones verdes (Newbie)
                Slot = Convert.ToByte(Slot + 1);
                withBlock.Invent.userObj[Slot].ObjIndex = 858;
                withBlock.Invent.userObj[Slot].Amount = 50;
            }

            // Ropa (Newbie)
            Slot = Convert.ToByte(Slot + 1);
            switch (UserRaza)
            {
                case Declaraciones.eRaza.Humano:
                {
                    withBlock.Invent.userObj[Slot].ObjIndex = 463;
                    break;
                }
                case Declaraciones.eRaza.Elfo:
                {
                    withBlock.Invent.userObj[Slot].ObjIndex = 464;
                    break;
                }
                case Declaraciones.eRaza.Drow:
                {
                    withBlock.Invent.userObj[Slot].ObjIndex = 465;
                    break;
                }
                case Declaraciones.eRaza.Enano:
                {
                    withBlock.Invent.userObj[Slot].ObjIndex = 466;
                    break;
                }
                case Declaraciones.eRaza.Gnomo:
                {
                    withBlock.Invent.userObj[Slot].ObjIndex = 466;
                    break;
                }
            }

            // Equipo ropa
            withBlock.Invent.userObj[Slot].Amount = 1;
            withBlock.Invent.userObj[Slot].Equipped = 1;

            withBlock.Invent.ArmourEqpSlot = Slot;
            withBlock.Invent.ArmourEqpObjIndex = withBlock.Invent.userObj[Slot].ObjIndex;

            // Arma (Newbie)
            Slot = Convert.ToByte(Slot + 1);
            switch (UserClase)
            {
                case Declaraciones.eClass.Hunter:
                {
                    // Arco (Newbie)
                    withBlock.Invent.userObj[Slot].ObjIndex = 859;
                    break;
                }
                case Declaraciones.eClass.Worker:
                {
                    // Herramienta (Newbie)
                    withBlock.Invent.userObj[Slot].ObjIndex =
                        Convert.ToInt16(Matematicas.RandomNumber(561, 565));
                    break;
                }

                default:
                {
                    // Daga (Newbie)
                    withBlock.Invent.userObj[Slot].ObjIndex = 460;
                    break;
                }
            }

            // Equipo arma
            withBlock.Invent.userObj[Slot].Amount = 1;
            withBlock.Invent.userObj[Slot].Equipped = 1;

            withBlock.Invent.WeaponEqpObjIndex = withBlock.Invent.userObj[Slot].ObjIndex;
            withBlock.Invent.WeaponEqpSlot = Slot;

            withBlock.character.WeaponAnim = UsUaRiOs.GetWeaponAnim(UserIndex, withBlock.Invent.WeaponEqpObjIndex);

            // Municiones (Newbie)
            if (UserClase == Declaraciones.eClass.Hunter)
            {
                Slot = Convert.ToByte(Slot + 1);
                withBlock.Invent.userObj[Slot].ObjIndex = 860;
                withBlock.Invent.userObj[Slot].Amount = 150;

                // Equipo flechas
                withBlock.Invent.userObj[Slot].Equipped = 1;
                withBlock.Invent.MunicionEqpSlot = Slot;
                withBlock.Invent.MunicionEqpObjIndex = 860;
            }

            // Manzanas (Newbie)
            Slot = Convert.ToByte(Slot + 1);
            withBlock.Invent.userObj[Slot].ObjIndex = 467;
            withBlock.Invent.userObj[Slot].Amount = 100;

            // Jugos (Nwbie)
            Slot = Convert.ToByte(Slot + 1);
            withBlock.Invent.userObj[Slot].ObjIndex = 468;
            withBlock.Invent.userObj[Slot].Amount = 100;

            // Sin casco y escudo
            withBlock.character.ShieldAnim = Declaraciones.NingunEscudo;
            withBlock.character.CascoAnim = Declaraciones.NingunCasco;

            // Total Items
            withBlock.Invent.NroItems = Slot;

            withBlock.LogOnTime = DateTime.Now;
            withBlock.UpTime = 0;
        }

        // Valores Default de facciones al Activar nuevo usuario
        ResetFacciones(UserIndex);

        ES.WriteVar(Declaraciones.CharPath + name.ToUpper() + ".chr", "INIT", "Password", Password);
        // grabamos el password aqui afuera, para no mantenerlo cargado en memoria

        ES.SaveUser(UserIndex, Declaraciones.CharPath + name.ToUpper() + ".chr");

        // Open User
        ConnectUser(UserIndex, ref name, ref Password);
    }

    public static void CloseSocket(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            if (UserIndex == Declaraciones.LastUser)
                while (!Declaraciones.UserList[Declaraciones.LastUser].flags.UserLogged)
                {
                    Declaraciones.LastUser = Convert.ToInt16(Declaraciones.LastUser - 1);
                    if (Declaraciones.LastUser < 1)
                        break;
                }

            // Call SecurityIp.IpRestarConexion(GetLongIp(UserList(UserIndex).ip))

            if (Declaraciones.UserList[UserIndex].ConnID != -1) CloseSocketSL(UserIndex);

            // Es el mismo user al que está revisando el centinela??
            // IMPORTANTE!!! hacerlo antes de resetear así todavía sabemos el nombre del user
            // y lo podemos loguear
            if (modCentinela.Centinela.RevisandoUserIndex == UserIndex)
                modCentinela.CentinelaUserLogout();

            // mato los comercios seguros
            if (Declaraciones.UserList[UserIndex].ComUsu.DestUsu > 0)
                if (Declaraciones.UserList[Declaraciones.UserList[UserIndex].ComUsu.DestUsu].flags.UserLogged)
                    if (Declaraciones.UserList[Declaraciones.UserList[UserIndex].ComUsu.DestUsu].ComUsu.DestUsu ==
                        UserIndex)
                    {
                        Protocol.WriteConsoleMsg(Declaraciones.UserList[UserIndex].ComUsu.DestUsu,
                            "Comercio cancelado por el otro usuario", Protocol.FontTypeNames.FONTTYPE_TALK);
                        mdlCOmercioConUsuario.FinComerciarUsu(Declaraciones.UserList[UserIndex].ComUsu.DestUsu);
                        Protocol.FlushBuffer(Declaraciones.UserList[UserIndex].ComUsu.DestUsu);
                    }

            // Empty buffer for reuse
            Declaraciones.UserList[UserIndex].incomingData
                .ReadASCIIStringFixed(Declaraciones.UserList[UserIndex].incomingData.length);

            if (Declaraciones.UserList[UserIndex].flags.UserLogged)
            {
                if (Declaraciones.NumUsers > 0)
                    Declaraciones.NumUsers = Convert.ToInt16(Declaraciones.NumUsers - 1);
                CloseUser(UserIndex);

                Admin.EstadisticasWeb.Informar(clsEstadisticasIPC.EstaNotificaciones.CANTIDAD_ONLINE,
                    Declaraciones.NumUsers);
            }
            else
            {
                ResetUserSlot(UserIndex);
            }

            Declaraciones.UserList[UserIndex].ConnID = -1;
            Declaraciones.UserList[UserIndex].ConnIDValida = false;
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in DarCuerpo: " + ex.Message);
            Declaraciones.UserList[UserIndex].ConnID = -1;
            Declaraciones.UserList[UserIndex].ConnIDValida = false;
            ResetUserSlot(UserIndex);

            var argdesc = "CloseSocket - Error = " + ex.GetType().Name + " - Descripción = " + ex.Message +
                          " - UserIndex = " + UserIndex;
            General.LogError(ref argdesc);
        }
    }

    // [Alejo-21-5]: Cierra un socket sin limpiar el slot
    public static void CloseSocketSL(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if ((Declaraciones.UserList[UserIndex].ConnID != -1) & Declaraciones.UserList[UserIndex].ConnIDValida)
        {
            wskapiAO.Winsock_Close(Declaraciones.UserList[UserIndex].ConnID);
            Declaraciones.UserList[UserIndex].ConnIDValida = false;
        }
    }

    // '
    // Send an string to a Slot
    // 
    // @param userIndex The index of the User
    // @param Datos The string that will be send

    public static int EnviarDatosASlot(short UserIndex, ref string datos)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 01/10/07
        // Last Modified By: Lucas Tavolaro Ortiz (Tavo)
        // Now it uses the clsByteQueue class and don`t make a FIFO Queue of String
        // ***************************************************
        try
        {
            int Ret;

            Ret = wskapiAO.WsApiEnviar(UserIndex, ref datos);

            if (Ret != 0)
            {
                // Close the socket avoiding any critical error
                CloseSocketSL(UserIndex);
                UsUaRiOs.Cerrar_Usuario(UserIndex);
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in CloseSocketSL: " + ex.Message);
        }

        return default;
    }

    public static bool EstaPCarea(ref short Index, ref short Index2)
    {
        bool EstaPCareaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short X, Y;
        var loopTo = Convert.ToInt16(Declaraciones.UserList[Index].Pos.Y + Declaraciones.MinYBorder - 1);
        for (Y = Convert.ToInt16(Declaraciones.UserList[Index].Pos.Y - Declaraciones.MinYBorder + 1); Y <= loopTo; Y++)
        {
            var loopTo1 = Convert.ToInt16(Declaraciones.UserList[Index].Pos.X + Declaraciones.MinXBorder - 1);
            for (X = Convert.ToInt16(Declaraciones.UserList[Index].Pos.X - Declaraciones.MinXBorder + 1);
                 X <= loopTo1;
                 X++)
                if (Declaraciones.MapData[Declaraciones.UserList[Index].Pos.Map, X, Y].UserIndex == Index2)
                {
                    EstaPCareaRet = true;
                    return EstaPCareaRet;
                }
        }

        EstaPCareaRet = false;
        return EstaPCareaRet;
    }

    public static bool HayPCarea(ref Declaraciones.WorldPos Pos)
    {
        bool HayPCareaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short X, Y;
        var loopTo = Convert.ToInt16(Pos.Y + Declaraciones.MinYBorder - 1);
        for (Y = Convert.ToInt16(Pos.Y - Declaraciones.MinYBorder + 1); Y <= loopTo; Y++)
        {
            var loopTo1 = Convert.ToInt16(Pos.X + Declaraciones.MinXBorder - 1);
            for (X = Convert.ToInt16(Pos.X - Declaraciones.MinXBorder + 1); X <= loopTo1; X++)
                if ((X > 0) & (Y > 0) & (X < 101) & (Y < 101))
                    if (Declaraciones.MapData[Pos.Map, X, Y].UserIndex > 0)
                    {
                        HayPCareaRet = true;
                        return HayPCareaRet;
                    }
        }

        HayPCareaRet = false;
        return HayPCareaRet;
    }

    public static bool HayOBJarea(ref Declaraciones.WorldPos Pos, ref short ObjIndex)
    {
        bool HayOBJareaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short X, Y;
        var loopTo = Convert.ToInt16(Pos.Y + Declaraciones.MinYBorder - 1);
        for (Y = Convert.ToInt16(Pos.Y - Declaraciones.MinYBorder + 1); Y <= loopTo; Y++)
        {
            var loopTo1 = Convert.ToInt16(Pos.X + Declaraciones.MinXBorder - 1);
            for (X = Convert.ToInt16(Pos.X - Declaraciones.MinXBorder + 1); X <= loopTo1; X++)
                if (Declaraciones.MapData[Pos.Map, X, Y].ObjInfo.ObjIndex == ObjIndex)
                {
                    HayOBJareaRet = true;
                    return HayOBJareaRet;
                }
        }

        HayOBJareaRet = false;
        return HayOBJareaRet;
    }

    public static bool ValidateChr(short UserIndex)
    {
        bool ValidateChrRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        ValidateChrRet = (Declaraciones.UserList[UserIndex].character.Head != 0) &
                         (Declaraciones.UserList[UserIndex].character.body != 0) & ValidateSkills(UserIndex);
        return ValidateChrRet;
    }

    public static void ConnectUser(short UserIndex, ref string name, ref string Password)
    {
        string tStr;

        var Leer = new clsIniReader();
        bool FoundPlace;
        bool esAgua;
        var tX = default(int);
        int tY;
        short i;
        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Barco, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Declaraciones.ObjData Barco;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];

            if (withBlock.flags.UserLogged)
            {
                var argtexto = "El usuario " + withBlock.name + " ha intentado loguear a " + name + " desde la IP " +
                               withBlock.ip;
                General.LogCheating(ref argtexto);
                // Kick player ( and leave character inside :D )!
                CloseSocketSL(UserIndex);
                UsUaRiOs.Cerrar_Usuario(UserIndex);
                return;
            }

            // Reseteamos los FLAGS
            withBlock.flags.Escondido = 0;
            withBlock.flags.TargetNPC = 0;
            withBlock.flags.TargetNpcTipo = Declaraciones.eNPCType.Comun;
            withBlock.flags.TargetObj = 0;
            withBlock.flags.TargetUser = 0;
            withBlock.character.FX = 0;

            // Controlamos no pasar el maximo de usuarios
            if (Declaraciones.NumUsers >= Declaraciones.MaxUsers)
            {
                Protocol.WriteErrorMsg(UserIndex,
                    "El servidor ha alcanzado el máximo de usuarios soportado, por favor vuelva a intertarlo más tarde.");
                Protocol.FlushBuffer(UserIndex);
                CloseSocket(UserIndex);
                return;
            }

            // ¿Este IP ya esta conectado?
            if (Declaraciones.AllowMultiLogins == 0)
                if (Extra.CheckForSameIP(UserIndex, withBlock.ip))
                {
                    Protocol.WriteErrorMsg(UserIndex, "No es posible usar más de un personaje al mismo tiempo.");
                    Protocol.FlushBuffer(UserIndex);
                    CloseSocket(UserIndex);
                    return;
                }

            // ¿Existe el personaje?
            if (!File.Exists(Declaraciones.CharPath + name.ToUpper() + ".chr"))
            {
                Protocol.WriteErrorMsg(UserIndex, "El personaje no existe.");
                Protocol.FlushBuffer(UserIndex);
                CloseSocket(UserIndex);
                return;
            }

            // ¿Es el passwd valido?
            var argEmptySpaces = 1024;
            if ((Password.ToUpper() ?? "") != (ES.GetVar(Declaraciones.CharPath + name.ToUpper() + ".chr", "INIT",
                    "Password", ref argEmptySpaces).ToUpper() ?? ""))
            {
                Protocol.WriteErrorMsg(UserIndex, "Password incorrecto.");
                Protocol.FlushBuffer(UserIndex);
                CloseSocket(UserIndex);
                return;
            }

            // ¿Ya esta conectado el personaje?
            if (Extra.CheckForSameName(name))
            {
                if (Declaraciones.UserList[Extra.NameIndex(name)].Counters.Saliendo)
                    Protocol.WriteErrorMsg(UserIndex, "El usuario está saliendo.");
                else
                    Protocol.WriteErrorMsg(UserIndex, "Perdón, un usuario con el mismo nombre se ha logueado.");
                Protocol.FlushBuffer(UserIndex);
                CloseSocket(UserIndex);
                return;
            }

            // Reseteamos los privilegios
            withBlock.flags.Privilegios = 0;

            // Vemos que clase de user es (se lo usa para setear los privilegios al loguear el PJ)
            if (ES.EsAdmin(name))
            {
                withBlock.flags.Privilegios = withBlock.flags.Privilegios | Declaraciones.PlayerType.Admin;
                var argtexto1 = "Se conecto con ip:" + withBlock.ip;
                General.LogGM(ref name, ref argtexto1);
            }
            else if (ES.EsDios(name))
            {
                withBlock.flags.Privilegios = withBlock.flags.Privilegios | Declaraciones.PlayerType.Dios;
                var argtexto2 = "Se conecto con ip:" + withBlock.ip;
                General.LogGM(ref name, ref argtexto2);
            }
            else if (ES.EsSemiDios(name))
            {
                withBlock.flags.Privilegios = withBlock.flags.Privilegios | Declaraciones.PlayerType.SemiDios;
                var argtexto3 = "Se conecto con ip:" + withBlock.ip;
                General.LogGM(ref name, ref argtexto3);
            }
            else if (ES.EsConsejero(name))
            {
                withBlock.flags.Privilegios = withBlock.flags.Privilegios | Declaraciones.PlayerType.Consejero;
                var argtexto4 = "Se conecto con ip:" + withBlock.ip;
                General.LogGM(ref name, ref argtexto4);
            }
            else
            {
                withBlock.flags.Privilegios = withBlock.flags.Privilegios | Declaraciones.PlayerType.User;
                withBlock.flags.AdminPerseguible = true;
            }

            // Add RM flag if needed
            if (ES.EsRolesMaster(name))
                withBlock.flags.Privilegios = withBlock.flags.Privilegios | Declaraciones.PlayerType.RoleMaster;

            if (Declaraciones.ServerSoloGMs > 0)
                if ((withBlock.flags.Privilegios & (Declaraciones.PlayerType.Admin | Declaraciones.PlayerType.Dios |
                                                    Declaraciones.PlayerType.SemiDios |
                                                    Declaraciones.PlayerType.Consejero)) == 0)
                {
                    Protocol.WriteErrorMsg(UserIndex,
                        "Servidor restringido a administradores. Por favor reintente en unos momentos.");
                    Protocol.FlushBuffer(UserIndex);
                    CloseSocket(UserIndex);
                    return;
                }

            // Cargamos el personaje

            Leer.Initialize(Declaraciones.CharPath + name.ToUpper() + ".chr");

            // Cargamos los datos del personaje
            ES.LoadUserInit(UserIndex, ref Leer);

            ES.LoadUserStats(UserIndex, ref Leer);

            if (!ValidateChr(UserIndex))
            {
                Protocol.WriteErrorMsg(UserIndex, "Error en el personaje.");
                CloseSocket(UserIndex);
                return;
            }

            ES.LoadUserReputacion(UserIndex, ref Leer);

            // UPGRADE_NOTE: El objeto Leer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            Leer = null;

            if (withBlock.Invent.EscudoEqpSlot == 0)
                withBlock.character.ShieldAnim = Declaraciones.NingunEscudo;
            if (withBlock.Invent.CascoEqpSlot == 0)
                withBlock.character.CascoAnim = Declaraciones.NingunCasco;
            if (withBlock.Invent.WeaponEqpSlot == 0)
                withBlock.character.WeaponAnim = Declaraciones.NingunArma;

            if (withBlock.Invent.MochilaEqpSlot > 0)
                withBlock.CurrentInventorySlots = Convert.ToByte(Declaraciones.MAX_NORMAL_INVENTORY_SLOTS +
                                                                 Declaraciones
                                                                     .objData[
                                                                         withBlock.Invent
                                                                             .userObj[
                                                                                 withBlock.Invent.MochilaEqpSlot]
                                                                             .ObjIndex].MochilaType * 5);
            else
                withBlock.CurrentInventorySlots = Declaraciones.MAX_NORMAL_INVENTORY_SLOTS;
            if (withBlock.flags.Muerto == 0)
            {
                withBlock.flags.SeguroResu = false;
                Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.ResuscitationSafeOff);
            }
            // Call WriteResuscitationSafeOff(UserIndex)
            else
            {
                withBlock.flags.SeguroResu = true;
                Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.ResuscitationSafeOn);
                // Call WriteResuscitationSafeOn(UserIndex)
            }

            InvUsuario.UpdateUserInv(true, UserIndex, 0);
            modHechizos.UpdateUserHechizos(true, UserIndex, 0);

            if (withBlock.flags.Paralizado != 0) Protocol.WriteParalizeOK(UserIndex);

            // '
            // TODO : Feo, esto tiene que ser parche cliente
            if (withBlock.flags.Estupidez == 0) Protocol.WriteDumbNoMore(UserIndex);

            // Posicion de comienzo
            if (withBlock.Pos.Map == 0)
            {
                switch (withBlock.Hogar)
                {
                    case Declaraciones.eCiudad.cNix:
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        withBlock.Pos = Declaraciones.Nix;
                        break;
                    }
                    case Declaraciones.eCiudad.cUllathorpe:
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        withBlock.Pos = Declaraciones.Ullathorpe;
                        break;
                    }
                    case Declaraciones.eCiudad.cBanderbill:
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        withBlock.Pos = Declaraciones.Banderbill;
                        break;
                    }
                    case Declaraciones.eCiudad.cLindos:
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        withBlock.Pos = Declaraciones.Lindos;
                        break;
                    }
                    case Declaraciones.eCiudad.cArghal:
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        withBlock.Pos = Declaraciones.Arghal;
                        break;
                    }

                    default:
                    {
                        withBlock.Hogar = Declaraciones.eCiudad.cUllathorpe;
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        withBlock.Pos = Declaraciones.Ullathorpe;
                        break;
                    }
                }
            }
            else if (!General.MapaValido(withBlock.Pos.Map))
            {
                Protocol.WriteErrorMsg(UserIndex, "El PJ se encuenta en un mapa inválido.");
                Protocol.FlushBuffer(UserIndex);
                CloseSocket(UserIndex);
                return;
            }

            // Tratamos de evitar en lo posible el "Telefrag". Solo 1 intento de loguear en pos adjacentes.
            // Codigo por Pablo (ToxicWaste) y revisado por Nacho (Integer), corregido para que realmetne ande y no tire el server por Juan Martín Sotuyo Dodero (Maraxus)
            if ((Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].UserIndex != 0) |
                (Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].NpcIndex != 0))
            {
                FoundPlace = false;
                esAgua = General.HayAgua(withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y);

                var loopTo = withBlock.Pos.Y + 1;
                for (tY = withBlock.Pos.Y - 1; tY <= loopTo; tY++)
                {
                    var loopTo1 = withBlock.Pos.X + 1;
                    for (tX = withBlock.Pos.X - 1; tX <= loopTo1; tX++)
                        if (esAgua)
                        {
                            // reviso que sea pos legal en agua, que no haya User ni NPC para poder loguear.
                            if (Extra.LegalPos(withBlock.Pos.Map, Convert.ToInt16(tX), Convert.ToInt16(tY), true,
                                    false))
                            {
                                FoundPlace = true;
                                break;
                            }
                        }
                        // reviso que sea pos legal en tierra, que no haya User ni NPC para poder loguear.
                        else if (Extra.LegalPos(withBlock.Pos.Map, Convert.ToInt16(tX), Convert.ToInt16(tY)))
                        {
                            FoundPlace = true;
                            break;
                        }

                    if (FoundPlace)
                        break;
                }

                if (FoundPlace) // Si encontramos un lugar, listo, nos quedamos ahi
                {
                    withBlock.Pos.X = Convert.ToInt16(tX);
                    withBlock.Pos.Y = Convert.ToInt16(tY);
                }
                // Si no encontramos un lugar, sacamos al usuario que tenemos abajo, y si es un NPC, lo pisamos.
                else if (Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].UserIndex != 0)
                {
                    // Si no encontramos lugar, y abajo teniamos a un usuario, lo pisamos y cerramos su comercio seguro
                    if (Declaraciones
                            .UserList[
                                Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].UserIndex]
                            .ComUsu.DestUsu > 0)
                    {
                        // Le avisamos al que estaba comerciando que se tuvo que ir.
                        if (Declaraciones
                            .UserList[
                                Declaraciones
                                    .UserList[
                                        Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y]
                                            .UserIndex].ComUsu.DestUsu].flags.UserLogged)
                        {
                            mdlCOmercioConUsuario.FinComerciarUsu(Declaraciones
                                .UserList[
                                    Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y]
                                        .UserIndex].ComUsu.DestUsu);
                            Protocol.WriteConsoleMsg(
                                Declaraciones
                                    .UserList[
                                        Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y]
                                            .UserIndex].ComUsu.DestUsu,
                                "Comercio cancelado. El otro usuario se ha desconectado.",
                                Protocol.FontTypeNames.FONTTYPE_TALK);
                            Protocol.FlushBuffer(Declaraciones
                                .UserList[
                                    Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y]
                                        .UserIndex].ComUsu.DestUsu);
                        }

                        // Lo sacamos.
                        if (Declaraciones
                            .UserList[
                                Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].UserIndex]
                            .flags.UserLogged)
                        {
                            mdlCOmercioConUsuario.FinComerciarUsu(Declaraciones
                                .MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].UserIndex);
                            Protocol.WriteErrorMsg(
                                Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].UserIndex,
                                "Alguien se ha conectado donde te encontrabas, por favor reconéctate...");
                            Protocol.FlushBuffer(Declaraciones
                                .MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].UserIndex);
                        }
                    }

                    CloseSocket(Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].UserIndex);
                }
            }

            // Nombre de sistema
            withBlock.name = name;

            withBlock.showName = true; // Por default los nombres son visibles

            // If in the water, and has a boat, equip it!
            if ((withBlock.Invent.BarcoObjIndex > 0) &
                (General.HayAgua(withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y) |
                 UsUaRiOs.BodyIsBoat(withBlock.character.body)))
            {
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Barco. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                Barco = Declaraciones.objData[withBlock.Invent.BarcoObjIndex];
                withBlock.character.Head = 0;
                if (withBlock.flags.Muerto == 0)
                {
                    UsUaRiOs.ToogleBoatBody(UserIndex);
                }
                else
                {
                    withBlock.character.body = Declaraciones.iFragataFantasmal;
                    withBlock.character.ShieldAnim = Declaraciones.NingunEscudo;
                    withBlock.character.WeaponAnim = Declaraciones.NingunArma;
                    withBlock.character.CascoAnim = Declaraciones.NingunCasco;
                }

                withBlock.flags.Navegando = 1;
            }


            // Info
            Protocol.WriteUserIndexInServer(UserIndex); // Enviamos el User index
            Protocol.WriteChangeMap(UserIndex, withBlock.Pos.Map,
                Declaraciones.mapInfo[withBlock.Pos.Map].MapVersion); // Carga el mapa
            Protocol.WritePlayMidi(UserIndex,
                Convert.ToByte(Migration.ParseVal(General.ReadField(1,
                    ref Declaraciones.mapInfo[withBlock.Pos.Map].Music, 45))));

            if (withBlock.flags.Privilegios == Declaraciones.PlayerType.Dios)
                withBlock.flags.ChatColor = Information.RGB(250, 250, 150);
            else if ((withBlock.flags.Privilegios != Declaraciones.PlayerType.User) &
                     (withBlock.flags.Privilegios !=
                      (Declaraciones.PlayerType.User | Declaraciones.PlayerType.ChaosCouncil)) &
                     (withBlock.flags.Privilegios !=
                      (Declaraciones.PlayerType.User | Declaraciones.PlayerType.RoyalCouncil)))
                withBlock.flags.ChatColor = Information.RGB(0, 255, 0);
            else if (withBlock.flags.Privilegios ==
                     (Declaraciones.PlayerType.User | Declaraciones.PlayerType.RoyalCouncil))
                withBlock.flags.ChatColor = Information.RGB(0, 255, 255);
            else if (withBlock.flags.Privilegios ==
                     (Declaraciones.PlayerType.User | Declaraciones.PlayerType.ChaosCouncil))
                withBlock.flags.ChatColor = Information.RGB(255, 128, 64);
            else
                withBlock.flags.ChatColor = ColorTranslator.ToOle(Color.White);

            withBlock.LogOnTime = DateTime.Now;

            // Crea  el personaje del usuario
            var argButIndex = false;
            UsUaRiOs.MakeUserChar(true, withBlock.Pos.Map, UserIndex, withBlock.Pos.Map, withBlock.Pos.X,
                withBlock.Pos.Y, ref argButIndex);

            Protocol.WriteUserCharIndexInServer(UserIndex);
            // '[/el oso]

            UsUaRiOs.CheckUserLevel(UserIndex);
            Protocol.WriteUpdateUserStats(UserIndex);

            Protocol.WriteUpdateHungerAndThirst(UserIndex);
            Protocol.WriteUpdateStrenghtAndDexterity(UserIndex);

            SendMOTD(UserIndex);

            if (Declaraciones.haciendoBK)
            {
                Protocol.WritePauseToggle(UserIndex);
                Protocol.WriteConsoleMsg(UserIndex,
                    "Servidor> Por favor espera algunos segundos, el WorldSave está ejecutándose.",
                    Protocol.FontTypeNames.FONTTYPE_SERVER);
            }

            if (Declaraciones.EnPausa)
            {
                Protocol.WritePauseToggle(UserIndex);
                Protocol.WriteConsoleMsg(UserIndex,
                    "Servidor> Lo sentimos mucho pero el servidor se encuentra actualmente detenido. Intenta ingresar más tarde.",
                    Protocol.FontTypeNames.FONTTYPE_SERVER);
            }

            if (Declaraciones.EnTesting & (withBlock.Stats.ELV >= 18))
            {
                Protocol.WriteErrorMsg(UserIndex,
                    "Servidor en Testing por unos minutos, conectese con PJs de nivel menor a 18. No se conecte con Pjs que puedan resultar importantes por ahora pues pueden arruinarse.");
                Protocol.FlushBuffer(UserIndex);
                CloseSocket(UserIndex);
                return;
            }

            // Actualiza el Num de usuarios
            // DE ACA EN ADELANTE GRABA EL CHARFILE, OJO!
            Declaraciones.NumUsers = Convert.ToInt16(Declaraciones.NumUsers + 1);
            withBlock.flags.UserLogged = true;

            // usado para borrar Pjs
            ES.WriteVar(Declaraciones.CharPath + withBlock.name + ".chr", "INIT", "Logged", "1");

            Admin.EstadisticasWeb.Informar(clsEstadisticasIPC.EstaNotificaciones.CANTIDAD_ONLINE,
                Declaraciones.NumUsers);

            Declaraciones.mapInfo[withBlock.Pos.Map].NumUsers =
                Convert.ToInt16(Declaraciones.mapInfo[withBlock.Pos.Map].NumUsers + 1);

            if (withBlock.Stats.SkillPts > 0)
            {
                Protocol.WriteSendSkills(UserIndex);
                Protocol.WriteLevelUp(UserIndex, withBlock.Stats.SkillPts);
            }

            if (Declaraciones.NumUsers > Declaraciones.recordusuarios)
            {
                modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                    Protocol.PrepareMessageConsoleMsg(
                        "Record de usuarios conectados simultaneamente." + "Hay " + Declaraciones.NumUsers +
                        " usuarios.", Protocol.FontTypeNames.FONTTYPE_INFO));
                Declaraciones.recordusuarios = Declaraciones.NumUsers;
                ES.WriteVar(Declaraciones.IniPath + "Server.ini", "INIT", "Record",
                    Conversion.Str(Declaraciones.recordusuarios));

                Admin.EstadisticasWeb.Informar(clsEstadisticasIPC.EstaNotificaciones.RECORD_USUARIOS,
                    Declaraciones.recordusuarios);
            }

            if ((withBlock.NroMascotas > 0) & Declaraciones.mapInfo[withBlock.Pos.Map].Pk)
                for (i = 1; i <= Declaraciones.MAXMASCOTAS; i++)
                    if (withBlock.MascotasType[i] > 0)
                    {
                        withBlock.MascotasIndex[i] =
                            NPCs.SpawnNpc(withBlock.MascotasType[i], ref withBlock.Pos, true, true);

                        if (withBlock.MascotasIndex[i] > 0)
                        {
                            Declaraciones.Npclist[withBlock.MascotasIndex[i]].MaestroUser = UserIndex;
                            NPCs.FollowAmo(withBlock.MascotasIndex[i]);
                        }
                        else
                        {
                            withBlock.MascotasIndex[i] = 0;
                        }
                    }

            if (withBlock.flags.Navegando == 1) Protocol.WriteNavigateToggle(UserIndex);

            if (ES.criminal(UserIndex))
            {
                Protocol.WriteMultiMessage(UserIndex,
                    (short)Declaraciones.eMessages.SafeModeOff); // Call WriteSafeModeOff(UserIndex)
                withBlock.flags.Seguro = false;
            }
            else
            {
                withBlock.flags.Seguro = true;
                Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.SafeModeOn);
            } // Call WriteSafeModeOn(UserIndex)

            if (withBlock.GuildIndex > 0)
                // welcome to the show baby...
                if (!modGuilds.m_ConectarMiembroAClan(UserIndex, withBlock.GuildIndex))
                    Protocol.WriteConsoleMsg(UserIndex, "Tu estado no te permite entrar al clan.",
                        Protocol.FontTypeNames.FONTTYPE_GUILD);

            modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                Protocol.PrepareMessageCreateFX(withBlock.character.CharIndex, (short)Declaraciones.FXIDs.FXWARP,
                    0));

            Protocol.WriteLoggedMessage(UserIndex);

            modGuilds.SendGuildNews(UserIndex);

            // Esta protegido del ataque de npcs por 5 segundos, si no realiza ninguna accion
            modNuevoTimer.IntervaloPermiteSerAtacado(UserIndex, true);

            if (Admin.Lloviendo) Protocol.WriteRainToggle(UserIndex);

            tStr = modGuilds.a_ObtenerRechazoDeChar(ref withBlock.name);

            if (Migration.migr_LenB(tStr) != 0)
                Protocol.WriteShowMessageBox(UserIndex,
                    "Tu solicitud de ingreso al clan ha sido rechazada. El clan te explica que: " + tStr);

            // Load the user statistics
            Statistics.UserConnected(UserIndex);

            General.MostrarNumUsers();

            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "logs/numusers.log",
                Declaraciones.NumUsers + Environment.NewLine);

            // Log
            General.AppendLog("logs/Connect.log",
                withBlock.name + " ha entrado al juego. UserIndex:" + UserIndex + " " +
                Conversions.ToString(DateAndTime.TimeOfDay) + " " + Conversions.ToString(DateTime.Today));
        }
    }

    public static void SendMOTD(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int j;

        Protocol.WriteGuildChat(UserIndex, "Mensajes de entrada:");
        var loopTo = (int)Admin.MaxLines;
        for (j = 1; j <= loopTo; j++)
            Protocol.WriteGuildChat(UserIndex, Admin.MOTD[j].texto);
    }

    public static void ResetFacciones(short UserIndex)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 23/01/2007
        // Resetea todos los valores generales y las stats
        // 03/15/2006 Maraxus - Uso de With para mayor performance y claridad.
        // 23/01/2007 Pablo (ToxicWaste) - Agrego NivelIngreso, FechaIngreso, MatadosIngreso y NextRecompensa.
        // *************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].Faccion;
            withBlock.ArmadaReal = 0;
            withBlock.CiudadanosMatados = 0;
            withBlock.CriminalesMatados = 0;
            withBlock.FuerzasCaos = 0;
            withBlock.FechaIngreso = "No ingresó a ninguna Facción";
            withBlock.RecibioArmaduraCaos = 0;
            withBlock.RecibioArmaduraReal = 0;
            withBlock.RecibioExpInicialCaos = 0;
            withBlock.RecibioExpInicialReal = 0;
            withBlock.RecompensasCaos = 0;
            withBlock.RecompensasReal = 0;
            withBlock.Reenlistadas = 0;
            withBlock.NivelIngreso = 0;
            withBlock.MatadosIngreso = 0;
            withBlock.NextRecompensa = 0;
        }
    }

    public static void ResetContadores(short UserIndex)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 03/15/2006
        // Resetea todos los valores generales y las stats
        // 03/15/2006 Maraxus - Uso de With para mayor performance y claridad.
        // 05/20/2007 Integer - Agregue todas las variables que faltaban.
        // *************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].Counters;
            withBlock.AGUACounter = 0;
            withBlock.AttackCounter = 0;
            withBlock.Ceguera = 0;
            withBlock.COMCounter = 0;
            withBlock.Estupidez = 0;
            withBlock.Frio = 0;
            withBlock.HPCounter = 0;
            withBlock.IdleCount = 0;
            withBlock.Invisibilidad = 0;
            withBlock.Paralisis = 0;
            withBlock.Pena = 0;
            withBlock.PiqueteC = 0;
            withBlock.STACounter = 0;
            withBlock.Veneno = 0;
            withBlock.Trabajando = 0;
            withBlock.Ocultando = 0;
            withBlock.bPuedeMeditar = false;
            withBlock.Lava = 0;
            withBlock.Mimetismo = 0;
            withBlock.Saliendo = false;
            withBlock.salir = 0;
            withBlock.TiempoOculto = 0;
            withBlock.TimerMagiaGolpe = 0;
            withBlock.TimerGolpeMagia = 0;
            withBlock.TimerLanzarSpell = 0;
            withBlock.TimerPuedeAtacar = 0;
            withBlock.TimerPuedeUsarArco = 0;
            withBlock.TimerPuedeTrabajar = 0;
            withBlock.TimerUsar = 0;
            withBlock.goHome = 0;
            withBlock.AsignedSkills = 0;
        }
    }

    public static void ResetCharInfo(short UserIndex)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 03/15/2006
        // Resetea todos los valores generales y las stats
        // 03/15/2006 Maraxus - Uso de With para mayor performance y claridad.
        // *************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].character;
            withBlock.body = 0;
            withBlock.CascoAnim = 0;
            withBlock.CharIndex = 0;
            withBlock.FX = 0;
            withBlock.Head = 0;
            withBlock.loops = 0;
            withBlock.heading = 0;
            withBlock.loops = 0;
            withBlock.ShieldAnim = 0;
            withBlock.WeaponAnim = 0;
        }
    }

    public static void ResetBasicUserInfo(short UserIndex)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 03/15/2006
        // Resetea todos los valores generales y las stats
        // 03/15/2006 Maraxus - Uso de With para mayor performance y claridad.
        // *************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            withBlock.name = Constants.vbNullString;
            withBlock.desc = Constants.vbNullString;
            withBlock.DescRM = Constants.vbNullString;
            withBlock.Pos.Map = 0;
            withBlock.Pos.X = 0;
            withBlock.Pos.Y = 0;
            withBlock.ip = Constants.vbNullString;
            withBlock.clase = 0;
            withBlock.email = Constants.vbNullString;
            withBlock.Genero = 0;
            withBlock.Hogar = 0;
            withBlock.raza = 0;

            withBlock.PartyIndex = 0;
            withBlock.PartySolicitud = 0;

            {
                ref var withBlock1 = ref withBlock.Stats;
                withBlock1.Banco = 0;
                withBlock1.ELV = 0;
                withBlock1.ELU = 0;
                withBlock1.Exp = 0d;
                withBlock1.def = 0;
                // .CriminalesMatados = 0
                withBlock1.NPCsMuertos = 0;
                withBlock1.UsuariosMatados = 0;
                withBlock1.SkillPts = 0;
                withBlock1.GLD = 0;
                withBlock1.UserAtributos[1] = 0;
                withBlock1.UserAtributos[2] = 0;
                withBlock1.UserAtributos[3] = 0;
                withBlock1.UserAtributos[4] = 0;
                withBlock1.UserAtributos[5] = 0;
                withBlock1.UserAtributosBackUP[1] = 0;
                withBlock1.UserAtributosBackUP[2] = 0;
                withBlock1.UserAtributosBackUP[3] = 0;
                withBlock1.UserAtributosBackUP[4] = 0;
                withBlock1.UserAtributosBackUP[5] = 0;
            }
        }
    }

    public static void ResetReputacion(short UserIndex)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 03/15/2006
        // Resetea todos los valores generales y las stats
        // 03/15/2006 Maraxus - Uso de With para mayor performance y claridad.
        // *************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].Reputacion;
            withBlock.AsesinoRep = 0;
            withBlock.BandidoRep = 0;
            withBlock.BurguesRep = 0;
            withBlock.LadronesRep = 0;
            withBlock.NobleRep = 0;
            withBlock.PlebeRep = 0;
            withBlock.NobleRep = 0;
            withBlock.Promedio = 0;
        }
    }

    public static void ResetGuildInfo(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if (Declaraciones.UserList[UserIndex].EscucheClan > 0)
        {
            modGuilds.GMDejaDeEscucharClan(UserIndex, Declaraciones.UserList[UserIndex].EscucheClan);
            Declaraciones.UserList[UserIndex].EscucheClan = 0;
        }

        if (Declaraciones.UserList[UserIndex].GuildIndex > 0)
            modGuilds.m_DesconectarMiembroDelClan(UserIndex, Declaraciones.UserList[UserIndex].GuildIndex);
        Declaraciones.UserList[UserIndex].GuildIndex = 0;
    }

    public static void ResetUserFlags(short UserIndex)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 06/28/2008
        // Resetea todos los valores generales y las stats
        // 03/15/2006 Maraxus - Uso de With para mayor performance y claridad.
        // 03/29/2006 Maraxus - Reseteo el CentinelaOK también.
        // 06/28/2008 NicoNZ - Agrego el flag Inmovilizado
        // *************************************************
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].flags;
            withBlock.Comerciando = false;
            withBlock.Ban = 0;
            withBlock.Escondido = 0;
            withBlock.DuracionEfecto = 0;
            withBlock.NpcInv = 0;
            withBlock.StatsChanged = 0;
            withBlock.TargetNPC = 0;
            withBlock.TargetNpcTipo = Declaraciones.eNPCType.Comun;
            withBlock.TargetObj = 0;
            withBlock.TargetObjMap = 0;
            withBlock.TargetObjX = 0;
            withBlock.TargetObjY = 0;
            withBlock.TargetUser = 0;
            withBlock.TipoPocion = 0;
            withBlock.TomoPocion = false;
            withBlock.Descuento = Constants.vbNullString;
            withBlock.Hambre = 0;
            withBlock.Sed = 0;
            withBlock.Descansar = false;
            withBlock.Vuela = 0;
            withBlock.Navegando = 0;
            withBlock.Oculto = 0;
            withBlock.Envenenado = 0;
            withBlock.invisible = 0;
            withBlock.Paralizado = 0;
            withBlock.Inmovilizado = 0;
            withBlock.Maldicion = 0;
            withBlock.Bendicion = 0;
            withBlock.Meditando = false;
            withBlock.Privilegios = 0;
            withBlock.PuedeMoverse = 0;
            withBlock.OldBody = 0;
            withBlock.OldHead = 0;
            withBlock.AdminInvisible = 0;
            withBlock.ValCoDe = 0;
            withBlock.Hechizo = 0;
            withBlock.TimesWalk = 0;
            withBlock.StartWalk = 0;
            withBlock.CountSH = 0;
            withBlock.Silenciado = 0;
            withBlock.CentinelaOK = false;
            withBlock.AdminPerseguible = false;
            withBlock.lastMap = 0;
            withBlock.Traveling = 0;
            withBlock.AtacablePor = 0;
            withBlock.AtacadoPorNpc = 0;
            withBlock.AtacadoPorUser = 0;
            withBlock.NoPuedeSerAtacado = false;
            withBlock.OwnedNpc = 0;
            withBlock.ShareNpcWith = 0;
            withBlock.EnConsulta = false;
            withBlock.Ignorado = false;
        }
    }

    public static void ResetUserSpells(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int LoopC;
        for (LoopC = 1; LoopC <= Declaraciones.MAXUSERHECHIZOS; LoopC++)
            Declaraciones.UserList[UserIndex].Stats.UserHechizos[LoopC] = 0;
    }

    public static void ResetUserPets(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int LoopC;

        Declaraciones.UserList[UserIndex].NroMascotas = 0;

        for (LoopC = 1; LoopC <= Declaraciones.MAXMASCOTAS; LoopC++)
        {
            Declaraciones.UserList[UserIndex].MascotasIndex[LoopC] = 0;
            Declaraciones.UserList[UserIndex].MascotasType[LoopC] = 0;
        }
    }

    public static void ResetUserBanco(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int LoopC;

        for (LoopC = 1; LoopC <= Declaraciones.MAX_BANCOINVENTORY_SLOTS; LoopC++)
        {
            Declaraciones.UserList[UserIndex].BancoInvent.userObj[LoopC].Amount = 0;
            Declaraciones.UserList[UserIndex].BancoInvent.userObj[LoopC].Equipped = 0;
            Declaraciones.UserList[UserIndex].BancoInvent.userObj[LoopC].ObjIndex = 0;
        }

        Declaraciones.UserList[UserIndex].BancoInvent.NroItems = 0;
    }

    public static void LimpiarComercioSeguro(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].ComUsu;
            if (withBlock.DestUsu > 0)
            {
                mdlCOmercioConUsuario.FinComerciarUsu(withBlock.DestUsu);
                mdlCOmercioConUsuario.FinComerciarUsu(UserIndex);
            }
        }
    }

    public static void ResetUserSlot(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int i;

        Declaraciones.UserList[UserIndex].ConnIDValida = false;
        Declaraciones.UserList[UserIndex].ConnID = -1;

        LimpiarComercioSeguro(UserIndex);
        ResetFacciones(UserIndex);
        ResetContadores(UserIndex);
        ResetGuildInfo(UserIndex);
        ResetCharInfo(UserIndex);
        ResetBasicUserInfo(UserIndex);
        ResetReputacion(UserIndex);
        ResetUserFlags(UserIndex);
        InvUsuario.LimpiarInventario(UserIndex);
        ResetUserSpells(UserIndex);
        ResetUserPets(UserIndex);
        ResetUserBanco(UserIndex);
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].ComUsu;
            withBlock.Acepto = false;

            for (i = 1; i <= mdlCOmercioConUsuario.MAX_OFFER_SLOTS; i++)
            {
                withBlock.cant[i] = 0;
                withBlock.Objeto[i] = 0;
            }

            withBlock.GoldAmount = 0;
            withBlock.DestNick = Constants.vbNullString;
            withBlock.DestUsu = 0;
        }
    }

    public static void CloseUser(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            short Map;
            string name;
            short i;

            short aN;

            aN = Declaraciones.UserList[UserIndex].flags.AtacadoPorNpc;
            if (aN > 0)
            {
                Declaraciones.Npclist[aN].Movement = Declaraciones.Npclist[aN].flags.OldMovement;
                Declaraciones.Npclist[aN].Hostile = Declaraciones.Npclist[aN].flags.OldHostil;
                Declaraciones.Npclist[aN].flags.AttackedBy = Constants.vbNullString;
            }

            aN = Declaraciones.UserList[UserIndex].flags.NPCAtacado;
            if (aN > 0)
                if ((Declaraciones.Npclist[aN].flags.AttackedFirstBy ?? "") ==
                    (Declaraciones.UserList[UserIndex].name ?? ""))
                    Declaraciones.Npclist[aN].flags.AttackedFirstBy = Constants.vbNullString;

            Declaraciones.UserList[UserIndex].flags.AtacadoPorNpc = 0;
            Declaraciones.UserList[UserIndex].flags.NPCAtacado = 0;

            Map = Declaraciones.UserList[UserIndex].Pos.Map;
            name = Declaraciones.UserList[UserIndex].name.ToUpper();

            Declaraciones.UserList[UserIndex].character.FX = 0;
            Declaraciones.UserList[UserIndex].character.loops = 0;
            modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                Protocol.PrepareMessageCreateFX(Declaraciones.UserList[UserIndex].character.CharIndex, 0, 0));


            Declaraciones.UserList[UserIndex].flags.UserLogged = false;
            Declaraciones.UserList[UserIndex].Counters.Saliendo = false;

            // Le devolvemos el body y head originales
            if (Declaraciones.UserList[UserIndex].flags.AdminInvisible == 1)
                Trabajo.DoAdminInvisible(UserIndex);

            // si esta en party le devolvemos la experiencia
            if (Declaraciones.UserList[UserIndex].PartyIndex > 0)
                mdParty.SalirDeParty(UserIndex);

            // Save statistics
            Statistics.UserDisconnected(UserIndex);

            // Grabamos el personaje del usuario
            ES.SaveUser(UserIndex, Declaraciones.CharPath + name + ".chr");

            // usado para borrar Pjs
            ES.WriteVar(Declaraciones.CharPath + Declaraciones.UserList[UserIndex].name + ".chr", "INIT", "Logged",
                "0");


            // Quitar el dialogo
            // If MapInfo(Map).NumUsers > 0 Then
            // Call SendToUserArea(UserIndex, "QDL" & UserList(UserIndex).Char.charindex)
            // End If

            if (Declaraciones.mapInfo[Map].NumUsers > 0)
                modSendData.SendData(modSendData.SendTarget.ToPCAreaButIndex, UserIndex,
                    Protocol.PrepareMessageRemoveCharDialog(Declaraciones.UserList[UserIndex].character.CharIndex));


            // Borrar el personaje
            if (Declaraciones.UserList[UserIndex].character.CharIndex > 0)
                UsUaRiOs.EraseUserChar(UserIndex, Declaraciones.UserList[UserIndex].flags.AdminInvisible == 1);

            // Borrar mascotas
            for (i = 1; i <= Declaraciones.MAXMASCOTAS; i++)
                if (Declaraciones.UserList[UserIndex].MascotasIndex[i] > 0)
                    if (Declaraciones.Npclist[Declaraciones.UserList[UserIndex].MascotasIndex[i]].flags.NPCActive)
                        NPCs.QuitarNPC(Declaraciones.UserList[UserIndex].MascotasIndex[i]);

            // Update Map Users
            Declaraciones.mapInfo[Map].NumUsers =
                Convert.ToInt16(Declaraciones.mapInfo[Map].NumUsers - 1);

            if (Declaraciones.mapInfo[Map].NumUsers < 0) Declaraciones.mapInfo[Map].NumUsers = 0;

            // Si el usuario habia dejado un msg en la gm's queue lo borramos
            if (Declaraciones.Ayuda.Existe(Declaraciones.UserList[UserIndex].name))
                Declaraciones.Ayuda.Quitar(Declaraciones.UserList[UserIndex].name);

            ResetUserSlot(UserIndex);

            General.MostrarNumUsers();

            General.AppendLog("logs/Connect.log",
                name + " ha dejado el juego. " + "User Index:" + UserIndex + " " +
                Conversions.ToString(DateAndTime.TimeOfDay) + " " + Conversions.ToString(DateTime.Today));
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in EstaPCarea: " + ex.Message);
            var argdesc = "Error en CloseUser. Número " + ex.GetType().Name + " Descripción: " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    public static void ReloadSokcet()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            if (Declaraciones.NumUsers <= 0) wskapiAO.WSApiReiniciarSockets();
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in ReloadSokcet: " + ex.Message);
            var argdesc = "Error en CheckSocketState " + ex.GetType().Name + ": " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    public static void EnviarNoche(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        Protocol.WriteSendNight(UserIndex,
            Admin.DeNoche &
            (((Declaraciones.mapInfo[Declaraciones.UserList[UserIndex].Pos.Map].Zona ?? "") ==
              Declaraciones.Campo) |
             ((Declaraciones.mapInfo[Declaraciones.UserList[UserIndex].Pos.Map].Zona ?? "") ==
              Declaraciones.Ciudad))
                ? true
                : false);
        Protocol.WriteSendNight(UserIndex, Admin.DeNoche ? true : false);
    }

    public static void EcharPjsNoPrivilegiados()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int LoopC;

        var loopTo = (int)Declaraciones.LastUser;
        for (LoopC = 1; LoopC <= loopTo; LoopC++)
            if (Declaraciones.UserList[LoopC].flags.UserLogged & (Declaraciones.UserList[LoopC].ConnID >= 0) &
                Declaraciones.UserList[LoopC].ConnIDValida)
                if ((Declaraciones.UserList[LoopC].flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                    CloseSocket(Convert.ToInt16(LoopC));
    }
}