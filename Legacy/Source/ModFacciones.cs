using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Legacy;

internal static class ModFacciones
{
    public enum eTipoDefArmors
    {
        ieBaja,
        ieMedia,
        ieAlta
    }


    public const short NUM_RANGOS_FACCION = 15;
    private const byte NUM_DEF_FACCION_ARMOURS = 3;
    public static short ArmaduraImperial1;
    public static short ArmaduraImperial2;
    public static short ArmaduraImperial3;
    public static short TunicaMagoImperial;
    public static short TunicaMagoImperialEnanos;
    public static short ArmaduraCaos1;
    public static short ArmaduraCaos2;
    public static short ArmaduraCaos3;
    public static short TunicaMagoCaos;
    public static short TunicaMagoCaosEnanos;

    public static short VestimentaImperialHumano;
    public static short VestimentaImperialEnano;
    public static short TunicaConspicuaHumano;
    public static short TunicaConspicuaEnano;
    public static short ArmaduraNobilisimaHumano;
    public static short ArmaduraNobilisimaEnano;
    public static short ArmaduraGranSacerdote;

    public static short VestimentaLegionHumano;
    public static short VestimentaLegionEnano;
    public static short TunicaLobregaHumano;
    public static short TunicaLobregaEnano;
    public static short TunicaEgregiaHumano;
    public static short TunicaEgregiaEnano;
    public static short SacerdoteDemoniaco;

    // Matriz que contiene las armaduras faccionarias segun raza, clase, faccion y defensa de armadura
    // UPGRADE_WARNING: El límite inferior de la matriz ArmadurasFaccion ha cambiado de 1,1 a 0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    // UPGRADE_WARNING: Es posible que la matriz ArmadurasFaccion necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
    public static tFaccionArmaduras[,] ArmadurasFaccion =
        new tFaccionArmaduras[Declaraciones.NUMCLASES + 1, Declaraciones.NUMRAZAS + 1];

    // Contiene la cantidad de exp otorgada cada vez que aumenta el rango
    public static int[] RecompensaFacciones = new int[NUM_RANGOS_FACCION + 1];

    private static short GetArmourAmount(short Rango, eTipoDefArmors TipoDef)
    {
        short GetArmourAmountRet = default;
        // ***************************************************
        // Autor: ZaMa
        // Last Modification: 15/04/2010
        // Returns the amount of armours to give, depending on the specified rank
        // ***************************************************

        switch (TipoDef)
        {
            case eTipoDefArmors.ieBaja:
            {
                GetArmourAmountRet = Convert.ToInt16(20d / (Rango + 1));
                break;
            }

            case eTipoDefArmors.ieMedia:
            {
                GetArmourAmountRet =
                    Convert.ToInt16(Rango * 2 / (double)SistemaCombate.MaximoInt(Convert.ToInt16(Rango - 4), 1));
                break;
            }

            case eTipoDefArmors.ieAlta:
            {
                GetArmourAmountRet = Convert.ToInt16(Rango * 1.35d);
                break;
            }
        }

        return GetArmourAmountRet;
    }

    private static void GiveFactionArmours(short UserIndex, bool IsCaos)
    {
        // ***************************************************
        // Autor: ZaMa
        // Last Modification: 15/04/2010
        // Gives faction armours to user
        // ***************************************************

        Declaraciones.Obj ObjArmour;
        short Rango;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];

            Rango = Convert.ToInt16(
                (IsCaos ? withBlock.Faccion.RecompensasCaos : withBlock.Faccion.RecompensasReal) + 1);


            // Entrego armaduras de defensa baja
            ObjArmour.Amount = GetArmourAmount(Rango, eTipoDefArmors.ieBaja);

            if (IsCaos)
                ObjArmour.ObjIndex = ArmadurasFaccion[(int)withBlock.clase, (int)withBlock.raza]
                    .Caos[(int)eTipoDefArmors.ieBaja];
            else
                ObjArmour.ObjIndex = ArmadurasFaccion[(int)withBlock.clase, (int)withBlock.raza]
                    .Armada[(int)eTipoDefArmors.ieBaja];

            if (!InvUsuario.MeterItemEnInventario(UserIndex, ref ObjArmour))
            {
                var argNotPirata = true;
                InvNpc.TirarItemAlPiso(ref withBlock.Pos, ref ObjArmour, ref argNotPirata);
            }


            // Entrego armaduras de defensa media
            ObjArmour.Amount = GetArmourAmount(Rango, eTipoDefArmors.ieMedia);

            if (IsCaos)
                ObjArmour.ObjIndex = ArmadurasFaccion[(int)withBlock.clase, (int)withBlock.raza]
                    .Caos[(int)eTipoDefArmors.ieMedia];
            else
                ObjArmour.ObjIndex = ArmadurasFaccion[(int)withBlock.clase, (int)withBlock.raza]
                    .Armada[(int)eTipoDefArmors.ieMedia];

            if (!InvUsuario.MeterItemEnInventario(UserIndex, ref ObjArmour))
            {
                var argNotPirata1 = true;
                InvNpc.TirarItemAlPiso(ref withBlock.Pos, ref ObjArmour, ref argNotPirata1);
            }


            // Entrego armaduras de defensa alta
            ObjArmour.Amount = GetArmourAmount(Rango, eTipoDefArmors.ieAlta);

            if (IsCaos)
                ObjArmour.ObjIndex = ArmadurasFaccion[(int)withBlock.clase, (int)withBlock.raza]
                    .Caos[(int)eTipoDefArmors.ieAlta];
            else
                ObjArmour.ObjIndex = ArmadurasFaccion[(int)withBlock.clase, (int)withBlock.raza]
                    .Armada[(int)eTipoDefArmors.ieAlta];

            if (!InvUsuario.MeterItemEnInventario(UserIndex, ref ObjArmour))
            {
                var argNotPirata2 = true;
                InvNpc.TirarItemAlPiso(ref withBlock.Pos, ref ObjArmour, ref argNotPirata2);
            }
        }
    }

    public static void GiveExpReward(short UserIndex, int Rango)
    {
        // ***************************************************
        // Autor: ZaMa
        // Last Modification: 15/04/2010
        // Gives reward exp to user
        // ***************************************************

        int GivenExp;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];

            GivenExp = RecompensaFacciones[Rango];

            withBlock.Stats.Exp = withBlock.Stats.Exp + GivenExp;

            if (withBlock.Stats.Exp > Declaraciones.MAXEXP)
                withBlock.Stats.Exp = Declaraciones.MAXEXP;

            Protocol.WriteConsoleMsg(UserIndex, "Has sido recompensado con " + GivenExp + " puntos de experiencia.",
                Protocol.FontTypeNames.FONTTYPE_FIGHT);

            UsUaRiOs.CheckUserLevel(UserIndex);
        }
    }

    public static void EnlistarArmadaReal(short UserIndex)
    {
        // ***************************************************
        // Autor: Pablo (ToxicWaste) & Unknown (orginal version)
        // Last Modification: 15/04/2010
        // Handles the entrance of users to the "Armada Real"
        // 15/03/2009: ZaMa - No se puede enlistar el fundador de un clan con alineación neutral.
        // 27/11/2009: ZaMa - Ahora no se puede enlistar un miembro de un clan neutro, por ende saque la antifaccion.
        // 15/04/2010: ZaMa - Cambio en recompensas iniciales.
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.Faccion.ArmadaReal == 1)
            {
                Protocol.WriteChatOverHead(UserIndex,
                    "¡¡¡Ya perteneces a las tropas reales!!! Ve a combatir criminales.",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            if (withBlock.Faccion.FuerzasCaos == 1)
            {
                Protocol.WriteChatOverHead(UserIndex, "¡¡¡Maldito insolente!!! Vete de aquí seguidor de las sombras.",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            if (ES.criminal(UserIndex))
            {
                Protocol.WriteChatOverHead(UserIndex, "¡¡¡No se permiten criminales en el ejército real!!!",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            if (withBlock.Faccion.CriminalesMatados < 30)
            {
                Protocol.WriteChatOverHead(UserIndex,
                    "Para unirte a nuestras fuerzas debes matar al menos 30 criminales, sólo has matado " +
                    withBlock.Faccion.CriminalesMatados + ".",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            if (withBlock.Stats.ELV < 25)
            {
                Protocol.WriteChatOverHead(UserIndex,
                    "¡¡¡Para unirte a nuestras fuerzas debes ser al menos de nivel 25!!!",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            if (withBlock.Faccion.CiudadanosMatados > 0)
            {
                Protocol.WriteChatOverHead(UserIndex,
                    "¡Has asesinado gente inocente, no aceptamos asesinos en las tropas reales!",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            if (withBlock.Faccion.Reenlistadas > 4)
            {
                Protocol.WriteChatOverHead(UserIndex, "¡Has sido expulsado de las fuerzas reales demasiadas veces!",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            if (withBlock.Reputacion.NobleRep < 1000000)
            {
                Protocol.WriteChatOverHead(UserIndex,
                    "Necesitas ser aún más noble para integrar el ejército real, sólo tienes " +
                    withBlock.Reputacion.NobleRep + "/1.000.000 puntos de nobleza",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            if (withBlock.GuildIndex > 0)
                if (modGuilds.GuildAlignment(withBlock.GuildIndex) == "Neutral")
                {
                    Protocol.WriteChatOverHead(UserIndex,
                        "¡¡¡Perteneces a un clan neutro, sal de él si quieres unirte a nuestras fuerzas!!!",
                        Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                        ColorTranslator.ToOle(Color.White));
                    return;
                }

            withBlock.Faccion.ArmadaReal = 1;
            withBlock.Faccion.Reenlistadas = Convert.ToByte(withBlock.Faccion.Reenlistadas + 1);

            Protocol.WriteChatOverHead(UserIndex,
                "¡¡¡Bienvenido al ejército real!!! Aquí tienes tus vestimentas. Cumple bien tu labor exterminando criminales y me encargaré de recompensarte.",
                Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                ColorTranslator.ToOle(Color.White));

            // TODO: Dejo esta variable por ahora, pero con chequear las reenlistadas deberia ser suficiente :S
            if (withBlock.Faccion.RecibioArmaduraReal == 0)
            {
                GiveFactionArmours(UserIndex, false);
                GiveExpReward(UserIndex, 0);

                withBlock.Faccion.RecibioArmaduraReal = 1;
                withBlock.Faccion.NivelIngreso = withBlock.Stats.ELV;
                withBlock.Faccion.FechaIngreso = DateTime.Today.ToString();
                // Esto por ahora es inútil, siempre va a ser cero, pero bueno, despues va a servir.
                withBlock.Faccion.MatadosIngreso = Convert.ToInt16(withBlock.Faccion.CiudadanosMatados);

                withBlock.Faccion.RecibioExpInicialReal = 1;
                withBlock.Faccion.RecompensasReal = 0;
                withBlock.Faccion.NextRecompensa = 70;
            }

            if (withBlock.flags.Navegando != 0)
                UsUaRiOs.RefreshCharStatus(UserIndex); // Actualizamos la barca si esta navegando (NicoNZ)

            var argdesc = withBlock.name + " ingresó el " + Conversions.ToString(DateTime.Today) +
                          " cuando era nivel " + withBlock.Stats.ELV;
            General.LogEjercitoReal(ref argdesc);
        }
    }

    public static void RecompensaArmadaReal(short UserIndex)
    {
        // ***************************************************
        // Autor: Pablo (ToxicWaste) & Unknown (orginal version)
        // Last Modification: 15/04/2010
        // Handles the way of gaining new ranks in the "Armada Real"
        // 15/04/2010: ZaMa - Agrego recompensas de oro y armaduras
        // ***************************************************
        int Crimis;
        byte Lvl;
        int NextRecom;
        int Nobleza;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            Lvl = withBlock.Stats.ELV;
            Crimis = withBlock.Faccion.CriminalesMatados;
            NextRecom = withBlock.Faccion.NextRecompensa;
            Nobleza = withBlock.Reputacion.NobleRep;

            if (Crimis < NextRecom)
            {
                Protocol.WriteChatOverHead(UserIndex,
                    "Mata " + (NextRecom - Crimis) + " criminales más para recibir la próxima recompensa.",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            switch (NextRecom)
            {
                case 70:
                {
                    withBlock.Faccion.RecompensasReal = 1;
                    withBlock.Faccion.NextRecompensa = 130;
                    break;
                }

                case 130:
                {
                    withBlock.Faccion.RecompensasReal = 2;
                    withBlock.Faccion.NextRecompensa = 210;
                    break;
                }

                case 210:
                {
                    withBlock.Faccion.RecompensasReal = 3;
                    withBlock.Faccion.NextRecompensa = 320;
                    break;
                }

                case 320:
                {
                    withBlock.Faccion.RecompensasReal = 4;
                    withBlock.Faccion.NextRecompensa = 460;
                    break;
                }

                case 460:
                {
                    withBlock.Faccion.RecompensasReal = 5;
                    withBlock.Faccion.NextRecompensa = 640;
                    break;
                }

                case 640:
                {
                    if (Lvl < 27)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes criminales, pero te faltan " + (27 - Lvl) +
                            " niveles para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    withBlock.Faccion.RecompensasReal = 6;
                    withBlock.Faccion.NextRecompensa = 870;
                    break;
                }

                case 870:
                {
                    withBlock.Faccion.RecompensasReal = 7;
                    withBlock.Faccion.NextRecompensa = 1160;
                    break;
                }

                case 1160:
                {
                    withBlock.Faccion.RecompensasReal = 8;
                    withBlock.Faccion.NextRecompensa = 2000;
                    break;
                }

                case 2000:
                {
                    if (Lvl < 30)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes criminales, pero te faltan " + (30 - Lvl) +
                            " niveles para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    withBlock.Faccion.RecompensasReal = 9;
                    withBlock.Faccion.NextRecompensa = 2500;
                    break;
                }

                case 2500:
                {
                    if (Nobleza < 2000000)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes criminales, pero te faltan " + (2000000 - Nobleza) +
                            " puntos de nobleza para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    withBlock.Faccion.RecompensasReal = 10;
                    withBlock.Faccion.NextRecompensa = 3000;
                    break;
                }

                case 3000:
                {
                    if (Nobleza < 3000000)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes criminales, pero te faltan " + (3000000 - Nobleza) +
                            " puntos de nobleza para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    withBlock.Faccion.RecompensasReal = 11;
                    withBlock.Faccion.NextRecompensa = 3500;
                    break;
                }

                case 3500:
                {
                    if (Lvl < 35)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes criminales, pero te faltan " + (35 - Lvl) +
                            " niveles para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    if (Nobleza < 4000000)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes criminales, pero te faltan " + (4000000 - Nobleza) +
                            " puntos de nobleza para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    withBlock.Faccion.RecompensasReal = 12;
                    withBlock.Faccion.NextRecompensa = 4000;
                    break;
                }

                case 4000:
                {
                    if (Lvl < 36)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes criminales, pero te faltan " + (36 - Lvl) +
                            " niveles para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    if (Nobleza < 5000000)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes criminales, pero te faltan " + (5000000 - Nobleza) +
                            " puntos de nobleza para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    withBlock.Faccion.RecompensasReal = 13;
                    withBlock.Faccion.NextRecompensa = 5000;
                    break;
                }

                case 5000:
                {
                    if (Lvl < 37)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes criminales, pero te faltan " + (37 - Lvl) +
                            " niveles para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    if (Nobleza < 6000000)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes criminales, pero te faltan " + (6000000 - Nobleza) +
                            " puntos de nobleza para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    withBlock.Faccion.RecompensasReal = 14;
                    withBlock.Faccion.NextRecompensa = 10000;
                    break;
                }

                case 10000:
                {
                    Protocol.WriteChatOverHead(UserIndex,
                        "Eres uno de mis mejores soldados. Mataste " + Crimis +
                        " criminales, sigue así. Ya no tengo más recompensa para darte que mi agradecimiento. ¡Felicidades!",
                        Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                        ColorTranslator.ToOle(Color.White));
                    return;
                }

                default:
                {
                    return;
                }
            }

            Protocol.WriteChatOverHead(UserIndex, "¡¡¡Aquí tienes tu recompensa " + TituloReal(UserIndex) + "!!!",
                Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                ColorTranslator.ToOle(Color.White));

            // Recompensas de armaduras y exp
            GiveFactionArmours(UserIndex, false);
            GiveExpReward(UserIndex, withBlock.Faccion.RecompensasReal);
        }
    }

    public static void ExpulsarFaccionReal(short UserIndex, [Optional] [DefaultParameterValue(true)] ref bool Expulsado)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            withBlock.Faccion.ArmadaReal = 0;
            // Call PerderItemsFaccionarios(UserIndex)
            if (Expulsado)
                Protocol.WriteConsoleMsg(UserIndex, "¡¡¡Has sido expulsado del ejército real!!!",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
            else
                Protocol.WriteConsoleMsg(UserIndex, "¡¡¡Te has retirado del ejército real!!!",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);

            if (withBlock.Invent.ArmourEqpObjIndex != 0)
                // Desequipamos la armadura real si está equipada
                if (Declaraciones.objData[withBlock.Invent.ArmourEqpObjIndex].Real == 1)
                    InvUsuario.Desequipar(UserIndex, withBlock.Invent.ArmourEqpSlot);

            if (withBlock.Invent.EscudoEqpObjIndex != 0)
                // Desequipamos el escudo de caos si está equipado
                if (Declaraciones.objData[withBlock.Invent.EscudoEqpObjIndex].Real == 1)
                    InvUsuario.Desequipar(UserIndex, Convert.ToByte(withBlock.Invent.EscudoEqpObjIndex));

            if (withBlock.flags.Navegando != 0)
                UsUaRiOs.RefreshCharStatus(UserIndex); // Actualizamos la barca si esta navegando (NicoNZ)
        }
    }

    public static void ExpulsarFaccionCaos(short UserIndex, [Optional] [DefaultParameterValue(true)] ref bool Expulsado)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            withBlock.Faccion.FuerzasCaos = 0;
            // Call PerderItemsFaccionarios(UserIndex)
            if (Expulsado)
                Protocol.WriteConsoleMsg(UserIndex, "¡¡¡Has sido expulsado de la Legión Oscura!!!",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
            else
                Protocol.WriteConsoleMsg(UserIndex, "¡¡¡Te has retirado de la Legión Oscura!!!",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);

            if (withBlock.Invent.ArmourEqpObjIndex != 0)
                // Desequipamos la armadura de caos si está equipada
                if (Declaraciones.objData[withBlock.Invent.ArmourEqpObjIndex].Caos == 1)
                    InvUsuario.Desequipar(UserIndex, withBlock.Invent.ArmourEqpSlot);

            if (withBlock.Invent.EscudoEqpObjIndex != 0)
                // Desequipamos el escudo de caos si está equipado
                if (Declaraciones.objData[withBlock.Invent.EscudoEqpObjIndex].Caos == 1)
                    InvUsuario.Desequipar(UserIndex, Convert.ToByte(withBlock.Invent.EscudoEqpObjIndex));

            if (withBlock.flags.Navegando != 0)
                UsUaRiOs.RefreshCharStatus(UserIndex); // Actualizamos la barca si esta navegando (NicoNZ)
        }
    }

    public static string TituloReal(short UserIndex)
    {
        string TituloRealRet = default;
        // ***************************************************
        // Autor: Unknown
        // Last Modification: 23/01/2007 Pablo (ToxicWaste)
        // Handles the titles of the members of the "Armada Real"
        // ***************************************************

        switch (Declaraciones.UserList[UserIndex].Faccion.RecompensasReal)
        {
            // Rango 1: Aprendiz (30 Criminales)
            // Rango 2: Escudero (70 Criminales)
            // Rango 3: Soldado (130 Criminales)
            // Rango 4: Sargento (210 Criminales)
            // Rango 5: Caballero (320 Criminales)
            // Rango 6: Comandante (460 Criminales)
            // Rango 7: Capitán (640 Criminales + > lvl 27)
            // Rango 8: Senescal (870 Criminales)
            // Rango 9: Mariscal (1160 Criminales)
            // Rango 10: Condestable (2000 Criminales + > lvl 30)
            // Rangos de Honor de la Armada Real: (Consejo de Bander)
            // Rango 11: Ejecutor Imperial (2500 Criminales + 2.000.000 Nobleza)
            // Rango 12: Protector del Reino (3000 Criminales + 3.000.000 Nobleza)
            // Rango 13: Avatar de la Justicia (3500 Criminales + 4.000.000 Nobleza + > lvl 35)
            // Rango 14: Guardián del Bien (4000 Criminales + 5.000.000 Nobleza + > lvl 36)
            // Rango 15: Campeón de la Luz (5000 Criminales + 6.000.000 Nobleza + > lvl 37)

            case 0:
            {
                TituloRealRet = "Aprendiz";
                break;
            }
            case 1:
            {
                TituloRealRet = "Escudero";
                break;
            }
            case 2:
            {
                TituloRealRet = "Soldado";
                break;
            }
            case 3:
            {
                TituloRealRet = "Sargento";
                break;
            }
            case 4:
            {
                TituloRealRet = "Teniente";
                break;
            }
            case 5:
            {
                TituloRealRet = "Comandante";
                break;
            }
            case 6:
            {
                TituloRealRet = "Capitán";
                break;
            }
            case 7:
            {
                TituloRealRet = "Senescal";
                break;
            }
            case 8:
            {
                TituloRealRet = "Mariscal";
                break;
            }
            case 9:
            {
                TituloRealRet = "Condestable";
                break;
            }
            case 10:
            {
                TituloRealRet = "Ejecutor Imperial";
                break;
            }
            case 11:
            {
                TituloRealRet = "Protector del Reino";
                break;
            }
            case 12:
            {
                TituloRealRet = "Avatar de la Justicia";
                break;
            }
            case 13:
            {
                TituloRealRet = "Guardián del Bien";
                break;
            }

            default:
            {
                TituloRealRet = "Campeón de la Luz";
                break;
            }
        }

        return TituloRealRet;
    }

    public static void EnlistarCaos(short UserIndex)
    {
        // ***************************************************
        // Autor: Pablo (ToxicWaste) & Unknown (orginal version)
        // Last Modification: 27/11/2009
        // 15/03/2009: ZaMa - No se puede enlistar el fundador de un clan con alineación neutral.
        // 27/11/2009: ZaMa - Ahora no se puede enlistar un miembro de un clan neutro, por ende saque la antifaccion.
        // Handles the entrance of users to the "Legión Oscura"
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (!ES.criminal(UserIndex))
            {
                Protocol.WriteChatOverHead(UserIndex, "¡¡¡Lárgate de aquí, bufón!!!",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            if (withBlock.Faccion.FuerzasCaos == 1)
            {
                Protocol.WriteChatOverHead(UserIndex, "¡¡¡Ya perteneces a la legión oscura!!!",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            if (withBlock.Faccion.ArmadaReal == 1)
            {
                Protocol.WriteChatOverHead(UserIndex,
                    "Las sombras reinarán en Argentum. ¡¡¡Fuera de aquí insecto real!!!",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            // [Barrin 17-12-03] Si era miembro de la Armada Real no se puede enlistar
            if (withBlock.Faccion.RecibioExpInicialReal ==
                1) // Tomamos el valor de ahí: ¿Recibio la experiencia para entrar?
            {
                Protocol.WriteChatOverHead(UserIndex, "No permitiré que ningún insecto real ingrese a mis tropas.",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }
            // [/Barrin]

            if (!ES.criminal(UserIndex))
            {
                Protocol.WriteChatOverHead(UserIndex, "¡¡Ja ja ja!! Tú no eres bienvenido aquí asqueroso ciudadano.",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            if (withBlock.Faccion.CiudadanosMatados < 70)
            {
                Protocol.WriteChatOverHead(UserIndex,
                    "Para unirte a nuestras fuerzas debes matar al menos 70 ciudadanos, sólo has matado " +
                    withBlock.Faccion.CiudadanosMatados + ".",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            if (withBlock.Stats.ELV < 25)
            {
                Protocol.WriteChatOverHead(UserIndex,
                    "¡¡¡Para unirte a nuestras fuerzas debes ser al menos nivel 25!!!",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            if (withBlock.GuildIndex > 0)
                if (modGuilds.GuildAlignment(withBlock.GuildIndex) == "Neutral")
                {
                    Protocol.WriteChatOverHead(UserIndex,
                        "¡¡¡Perteneces a un clan neutro, sal de él si quieres unirte a nuestras fuerzas!!!",
                        Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                        ColorTranslator.ToOle(Color.White));
                    return;
                }


            if (withBlock.Faccion.Reenlistadas > 4)
            {
                if (withBlock.Faccion.Reenlistadas == 200)
                    Protocol.WriteChatOverHead(UserIndex,
                        "Has sido expulsado de las fuerzas oscuras y durante tu rebeldía has atacado a mi ejército. ¡Vete de aquí!",
                        Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                        ColorTranslator.ToOle(Color.White));
                else
                    Protocol.WriteChatOverHead(UserIndex,
                        "¡Has sido expulsado de las fuerzas oscuras demasiadas veces!",
                        Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                        ColorTranslator.ToOle(Color.White));
                return;
            }

            withBlock.Faccion.Reenlistadas = Convert.ToByte(withBlock.Faccion.Reenlistadas + 1);
            withBlock.Faccion.FuerzasCaos = 1;

            Protocol.WriteChatOverHead(UserIndex,
                "¡¡¡Bienvenido al lado oscuro!!! Aquí tienes tus armaduras. Derrama sangre ciudadana y real, y serás recompensado, lo prometo.",
                Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                ColorTranslator.ToOle(Color.White));

            if (withBlock.Faccion.RecibioArmaduraCaos == 0)
            {
                GiveFactionArmours(UserIndex, true);
                GiveExpReward(UserIndex, 0);

                withBlock.Faccion.RecibioArmaduraCaos = 1;
                withBlock.Faccion.NivelIngreso = withBlock.Stats.ELV;
                withBlock.Faccion.FechaIngreso = DateTime.Today.ToString();

                withBlock.Faccion.RecibioExpInicialCaos = 1;
                withBlock.Faccion.RecompensasCaos = 0;
                withBlock.Faccion.NextRecompensa = 160;
            }

            if (withBlock.flags.Navegando != 0)
                UsUaRiOs.RefreshCharStatus(UserIndex); // Actualizamos la barca si esta navegando (NicoNZ)

            var argdesc = withBlock.name + " ingresó el " + Conversions.ToString(DateTime.Today) +
                          " cuando era nivel " + withBlock.Stats.ELV;
            General.LogEjercitoCaos(ref argdesc);
        }
    }

    public static void RecompensaCaos(short UserIndex)
    {
        // ***************************************************
        // Author: Pablo (ToxicWaste) & Unknown (orginal version)
        // Last Modification: 15/04/2010
        // Handles the way of gaining new ranks in the "Legión Oscura"
        // 15/04/2010: ZaMa - Agrego recompensas de oro y armaduras
        // ***************************************************
        int Ciudas;
        byte Lvl;
        int NextRecom;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            Lvl = withBlock.Stats.ELV;
            Ciudas = withBlock.Faccion.CiudadanosMatados;
            NextRecom = withBlock.Faccion.NextRecompensa;

            if (Ciudas < NextRecom)
            {
                Protocol.WriteChatOverHead(UserIndex,
                    "Mata " + (NextRecom - Ciudas) + " cuidadanos más para recibir la próxima recompensa.",
                    Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                    ColorTranslator.ToOle(Color.White));
                return;
            }

            switch (NextRecom)
            {
                case 160:
                {
                    withBlock.Faccion.RecompensasCaos = 1;
                    withBlock.Faccion.NextRecompensa = 300;
                    break;
                }

                case 300:
                {
                    withBlock.Faccion.RecompensasCaos = 2;
                    withBlock.Faccion.NextRecompensa = 490;
                    break;
                }

                case 490:
                {
                    withBlock.Faccion.RecompensasCaos = 3;
                    withBlock.Faccion.NextRecompensa = 740;
                    break;
                }

                case 740:
                {
                    withBlock.Faccion.RecompensasCaos = 4;
                    withBlock.Faccion.NextRecompensa = 1100;
                    break;
                }

                case 1100:
                {
                    withBlock.Faccion.RecompensasCaos = 5;
                    withBlock.Faccion.NextRecompensa = 1500;
                    break;
                }

                case 1500:
                {
                    if (Lvl < 27)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes ciudadanos, pero te faltan " + (27 - Lvl) +
                            " niveles para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    withBlock.Faccion.RecompensasCaos = 6;
                    withBlock.Faccion.NextRecompensa = 2010;
                    break;
                }

                case 2010:
                {
                    withBlock.Faccion.RecompensasCaos = 7;
                    withBlock.Faccion.NextRecompensa = 2700;
                    break;
                }

                case 2700:
                {
                    withBlock.Faccion.RecompensasCaos = 8;
                    withBlock.Faccion.NextRecompensa = 4600;
                    break;
                }

                case 4600:
                {
                    if (Lvl < 30)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes ciudadanos, pero te faltan " + (30 - Lvl) +
                            " niveles para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    withBlock.Faccion.RecompensasCaos = 9;
                    withBlock.Faccion.NextRecompensa = 5800;
                    break;
                }

                case 5800:
                {
                    if (Lvl < 31)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes ciudadanos, pero te faltan " + (31 - Lvl) +
                            " niveles para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    withBlock.Faccion.RecompensasCaos = 10;
                    withBlock.Faccion.NextRecompensa = 6990;
                    break;
                }

                case 6990:
                {
                    if (Lvl < 33)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes ciudadanos, pero te faltan " + (33 - Lvl) +
                            " niveles para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    withBlock.Faccion.RecompensasCaos = 11;
                    withBlock.Faccion.NextRecompensa = 8100;
                    break;
                }

                case 8100:
                {
                    if (Lvl < 35)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes ciudadanos, pero te faltan " + (35 - Lvl) +
                            " niveles para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    withBlock.Faccion.RecompensasCaos = 12;
                    withBlock.Faccion.NextRecompensa = 9300;
                    break;
                }

                case 9300:
                {
                    if (Lvl < 36)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes ciudadanos, pero te faltan " + (36 - Lvl) +
                            " niveles para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    withBlock.Faccion.RecompensasCaos = 13;
                    withBlock.Faccion.NextRecompensa = 11500;
                    break;
                }

                case 11500:
                {
                    if (Lvl < 37)
                    {
                        Protocol.WriteChatOverHead(UserIndex,
                            "Mataste suficientes ciudadanos, pero te faltan " + (37 - Lvl) +
                            " niveles para poder recibir la próxima recompensa.",
                            Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                            ColorTranslator.ToOle(Color.White));
                        return;
                    }

                    withBlock.Faccion.RecompensasCaos = 14;
                    withBlock.Faccion.NextRecompensa = 23000;
                    break;
                }

                case 23000:
                {
                    Protocol.WriteChatOverHead(UserIndex,
                        "Eres uno de mis mejores soldados. Mataste " + Ciudas +
                        " ciudadanos . Tu única recompensa será la sangre derramada. ¡¡Continúa así!!",
                        Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                        ColorTranslator.ToOle(Color.White));
                    return;
                }

                default:
                {
                    return;
                }
            }

            Protocol.WriteChatOverHead(UserIndex,
                "¡¡¡Bien hecho " + TituloCaos(UserIndex) + ", aquí tienes tu recompensa!!!",
                Convert.ToInt16(Declaraciones.Npclist[withBlock.flags.TargetNPC].character.CharIndex),
                ColorTranslator.ToOle(Color.White));

            // Recompensas de armaduras y exp
            GiveFactionArmours(UserIndex, true);
            GiveExpReward(UserIndex, withBlock.Faccion.RecompensasCaos);
        }
    }

    public static string TituloCaos(short UserIndex)
    {
        string TituloCaosRet = default;
        // ***************************************************
        // Autor: Unknown (orginal version)
        // Last Modification: 23/01/2007 Pablo (ToxicWaste)
        // Handles the titles of the members of the "Legión Oscura"
        // ***************************************************
        // Rango 1: Acólito (70)
        // Rango 2: Alma Corrupta (160)
        // Rango 3: Paria (300)
        // Rango 4: Condenado (490)
        // Rango 5: Esbirro (740)
        // Rango 6: Sanguinario (1100)
        // Rango 7: Corruptor (1500 + lvl 27)
        // Rango 8: Heraldo Impio (2010)
        // Rango 9: Caballero de la Oscuridad (2700)
        // Rango 10: Señor del Miedo (4600 + lvl 30)
        // Rango 11: Ejecutor Infernal (5800 + lvl 31)
        // Rango 12: Protector del Averno (6990 + lvl 33)
        // Rango 13: Avatar de la Destrucción (8100 + lvl 35)
        // Rango 14: Guardián del Mal (9300 + lvl 36)
        // Rango 15: Campeón de la Oscuridad (11500 + lvl 37)

        switch (Declaraciones.UserList[UserIndex].Faccion.RecompensasCaos)
        {
            case 0:
            {
                TituloCaosRet = "Acólito";
                break;
            }
            case 1:
            {
                TituloCaosRet = "Alma Corrupta";
                break;
            }
            case 2:
            {
                TituloCaosRet = "Paria";
                break;
            }
            case 3:
            {
                TituloCaosRet = "Condenado";
                break;
            }
            case 4:
            {
                TituloCaosRet = "Esbirro";
                break;
            }
            case 5:
            {
                TituloCaosRet = "Sanguinario";
                break;
            }
            case 6:
            {
                TituloCaosRet = "Corruptor";
                break;
            }
            case 7:
            {
                TituloCaosRet = "Heraldo Impío";
                break;
            }
            case 8:
            {
                TituloCaosRet = "Caballero de la Oscuridad";
                break;
            }
            case 9:
            {
                TituloCaosRet = "Señor del Miedo";
                break;
            }
            case 10:
            {
                TituloCaosRet = "Ejecutor Infernal";
                break;
            }
            case 11:
            {
                TituloCaosRet = "Protector del Averno";
                break;
            }
            case 12:
            {
                TituloCaosRet = "Avatar de la Destrucción";
                break;
            }
            case 13:
            {
                TituloCaosRet = "Guardián del Mal";
                break;
            }

            default:
            {
                TituloCaosRet = "Campeón de la Oscuridad";
                break;
            }
        }

        return TituloCaosRet;
    }

    public struct tFaccionArmaduras
    {
        [VBFixedArray(NUM_DEF_FACCION_ARMOURS - 1)]
        public short[] Armada;

        [VBFixedArray(NUM_DEF_FACCION_ARMOURS - 1)]
        public short[] Caos;

        // UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        public void Initialize()
        {
            Armada = new short[3];
            Caos = new short[3];
        }
    }
}