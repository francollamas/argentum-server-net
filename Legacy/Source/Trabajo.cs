using System;
using Microsoft.VisualBasic;

namespace Legacy;

internal static class Trabajo
{
    private const byte GASTO_ENERGIA_TRABAJADOR = 2;
    private const byte GASTO_ENERGIA_NO_TRABAJADOR = 6;


    public static void DoPermanecerOculto(short UserIndex)
    {
        // ********************************************************
        // Autor: Nacho (Integer)
        // Last Modif: 11/19/2009
        // Chequea si ya debe mostrarse
        // Pablo (ToxicWaste): Cambie los ordenes de prioridades porque sino no andaba.
        // 11/19/2009: Pato - Ahora el bandido se oculta la mitad del tiempo de las demás clases.
        // 13/01/2010: ZaMa - Now hidden on boat pirats recover the proper boat body.
        // 13/01/2010: ZaMa - Arreglo condicional para que el bandido camine oculto.
        // ********************************************************
        try
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                withBlock.Counters.TiempoOculto = Convert.ToInt16(withBlock.Counters.TiempoOculto - 1);
                if (withBlock.Counters.TiempoOculto <= 0)
                {
                    if (withBlock.clase == Declaraciones.eClass.Bandit)
                        withBlock.Counters.TiempoOculto = Convert.ToInt16(Conversion.Int(Admin.IntervaloOculto / 2d));
                    else
                        withBlock.Counters.TiempoOculto = Admin.IntervaloOculto;

                    if ((withBlock.clase == Declaraciones.eClass.Hunter) &
                        (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Ocultarse] > 90))
                        if ((withBlock.Invent.ArmourEqpObjIndex == 648) | (withBlock.Invent.ArmourEqpObjIndex == 360))
                            return;

                    withBlock.Counters.TiempoOculto = 0;
                    withBlock.flags.Oculto = 0;

                    if (withBlock.flags.Navegando == 1)
                    {
                        if (withBlock.clase == Declaraciones.eClass.Pirat)
                        {
                            // Pierde la apariencia de fragata fantasmal
                            UsUaRiOs.ToogleBoatBody(UserIndex);
                            Protocol.WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body, withBlock.character.Head,
                                (byte)withBlock.character.heading, Declaraciones.NingunArma,
                                Declaraciones.NingunEscudo, Declaraciones.NingunCasco);
                        }
                    }
                    else if (withBlock.flags.invisible == 0)
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "Has vuelto a ser visible.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        UsUaRiOs.SetInvisible(UserIndex, withBlock.character.CharIndex, false);
                    }
                }
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in DoPermanecerOculto: " + ex.Message);
            var argdesc = "Error en Sub DoPermanecerOculto";
            General.LogError(ref argdesc);
        }
    }

    public static void DoOcultarse(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 13/01/2010 (ZaMa)
        // Pablo (ToxicWaste): No olvidar agregar IntervaloOculto=500 al Server.ini.
        // Modifique la fórmula y ahora anda bien.
        // 13/01/2010: ZaMa - El pirata se transforma en galeon fantasmal cuando se oculta en agua.
        // ***************************************************

        try
        {
            double Suerte;
            short res;
            short Skill;

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                Skill = withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Ocultarse];

                Suerte = (((0.000002d * Skill - 0.0002d) * Skill + 0.0064d) * Skill + 0.1124d) * 100d;

                res = Convert.ToInt16(Matematicas.RandomNumber(1, 100));

                if (res <= Suerte)
                {
                    withBlock.flags.Oculto = 1;
                    Suerte = -0.000001d * Math.Pow(100 - Skill, 3d);
                    Suerte = Suerte + 0.00009229d * Math.Pow(100 - Skill, 2d);
                    Suerte = Suerte + -0.0088d * (100 - Skill);
                    Suerte = Suerte + 0.9571d;
                    Suerte = Suerte * Admin.IntervaloOculto;
                    withBlock.Counters.TiempoOculto = Convert.ToInt16(Suerte);

                    // No es pirata o es uno sin barca
                    if (withBlock.flags.Navegando == 0)
                    {
                        UsUaRiOs.SetInvisible(UserIndex, withBlock.character.CharIndex, true);

                        Protocol.WriteConsoleMsg(UserIndex, "¡Te has escondido entre las sombras!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                    }
                    // Es un pirata navegando
                    else
                    {
                        // Le cambiamos el body a galeon fantasmal
                        withBlock.character.body = Declaraciones.iFragataFantasmal;
                        // Actualizamos clientes
                        UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body, withBlock.character.Head,
                            (byte)withBlock.character.heading, Declaraciones.NingunArma, Declaraciones.NingunEscudo,
                            Declaraciones.NingunCasco);
                    }

                    UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Ocultarse, true);
                }
                else
                {
                    // [CDT 17-02-2004]
                    if (!(withBlock.flags.UltimoMensaje == 4))
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "¡No has logrado esconderte!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        withBlock.flags.UltimoMensaje = 4;
                    }
                    // [/CDT]

                    UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Ocultarse, false);
                }

                withBlock.Counters.Ocultando = withBlock.Counters.Ocultando + 1;
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in DoOcultarse: " + ex.Message);
            var argdesc = "Error en Sub DoOcultarse";
            General.LogError(ref argdesc);
        }
    }

    public static void DoNavega(short UserIndex, ref Declaraciones.ObjData Barco, short Slot)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 13/01/2010 (ZaMa)
        // 13/01/2010: ZaMa - El pirata pierde el ocultar si desequipa barca.
        // ***************************************************

        float ModNave;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            ModNave = ModNavegacion(withBlock.clase, UserIndex);

            if (withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Navegacion] / ModNave < Barco.MinSkill)
            {
                Protocol.WriteConsoleMsg(UserIndex, "No tienes suficientes conocimientos para usar este barco.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                Protocol.WriteConsoleMsg(UserIndex,
                    "Para usar este barco necesitas " + Barco.MinSkill * ModNave + " puntos en navegacion.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                return;
            }

            withBlock.Invent.BarcoObjIndex = withBlock.Invent.userObj[Slot].ObjIndex;
            withBlock.Invent.BarcoSlot = Convert.ToByte(Slot);

            // No estaba navegando
            if (withBlock.flags.Navegando == 0)
            {
                withBlock.character.Head = 0;

                // No esta muerto
                if (withBlock.flags.Muerto == 0)
                {
                    UsUaRiOs.ToogleBoatBody(UserIndex);

                    if (withBlock.clase == Declaraciones.eClass.Pirat)
                        if (withBlock.flags.Oculto == 1)
                        {
                            withBlock.flags.Oculto = 0;
                            UsUaRiOs.SetInvisible(UserIndex, withBlock.character.CharIndex, false);
                            Protocol.WriteConsoleMsg(UserIndex, "¡Has vuelto a ser visible!",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                        }
                }

                // Esta muerto
                else
                {
                    withBlock.character.body = Declaraciones.iFragataFantasmal;
                    withBlock.character.ShieldAnim = Declaraciones.NingunEscudo;
                    withBlock.character.WeaponAnim = Declaraciones.NingunArma;
                    withBlock.character.CascoAnim = Declaraciones.NingunCasco;
                }

                // Comienza a navegar
                withBlock.flags.Navegando = 1;
            }

            // Estaba navegando
            else
            {
                // No esta muerto
                if (withBlock.flags.Muerto == 0)
                {
                    withBlock.character.Head = withBlock.OrigChar.Head;

                    if (withBlock.clase == Declaraciones.eClass.Pirat)
                        if (withBlock.flags.Oculto == 1)
                        {
                            // Al desequipar barca, perdió el ocultar
                            withBlock.flags.Oculto = 0;
                            withBlock.Counters.Ocultando = 0;
                            Protocol.WriteConsoleMsg(UserIndex, "¡Has recuperado tu apariencia normal!",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                        }

                    if (withBlock.Invent.ArmourEqpObjIndex > 0)
                        withBlock.character.body =
                            Declaraciones.objData[withBlock.Invent.ArmourEqpObjIndex].Ropaje;
                    else
                        General.DarCuerpoDesnudo(UserIndex);

                    if (withBlock.Invent.EscudoEqpObjIndex > 0)
                        withBlock.character.ShieldAnim =
                            Declaraciones.objData[withBlock.Invent.EscudoEqpObjIndex].ShieldAnim;
                    if (withBlock.Invent.WeaponEqpObjIndex > 0)
                        withBlock.character.WeaponAnim =
                            UsUaRiOs.GetWeaponAnim(UserIndex, withBlock.Invent.WeaponEqpObjIndex);
                    if (withBlock.Invent.CascoEqpObjIndex > 0)
                        withBlock.character.CascoAnim =
                            Declaraciones.objData[withBlock.Invent.CascoEqpObjIndex].CascoAnim;
                }

                // Esta muerto
                else
                {
                    withBlock.character.body = Declaraciones.iCuerpoMuerto;
                    withBlock.character.Head = Declaraciones.iCabezaMuerto;
                    withBlock.character.ShieldAnim = Declaraciones.NingunEscudo;
                    withBlock.character.WeaponAnim = Declaraciones.NingunArma;
                    withBlock.character.CascoAnim = Declaraciones.NingunCasco;
                }

                // Termina de navegar
                withBlock.flags.Navegando = 0;
            }

            // Actualizo clientes
            UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body, withBlock.character.Head,
                (byte)withBlock.character.heading, withBlock.character.WeaponAnim,
                withBlock.character.ShieldAnim, withBlock.character.CascoAnim);
        }

        Protocol.WriteNavigateToggle(UserIndex);
    }

    public static void FundirMineral(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                if (withBlock.flags.TargetObjInvIndex > 0)
                {
                    if ((Declaraciones.objData[withBlock.flags.TargetObjInvIndex].OBJType ==
                         Declaraciones.eOBJType.otMinerales) &
                        (Declaraciones.objData[withBlock.flags.TargetObjInvIndex].MinSkill <=
                         withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Mineria] / ModFundicion(withBlock.clase)))
                        DoLingotes(UserIndex);
                    else
                        Protocol.WriteConsoleMsg(UserIndex,
                            "No tienes conocimientos de minería suficientes para trabajar este mineral.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                }
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in DoNavega: " + ex.Message);
            var argdesc = "Error en FundirMineral. Error " + ex.GetType().Name + " : " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    public static void FundirArmas(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                if (withBlock.flags.TargetObjInvIndex > 0)
                    if (Declaraciones.objData[withBlock.flags.TargetObjInvIndex].OBJType ==
                        Declaraciones.eOBJType.otWeapon)
                    {
                        if (Declaraciones.objData[withBlock.flags.TargetObjInvIndex].SkHerreria <=
                            withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Herreria] /
                            ModHerreriA(withBlock.clase))
                            DoFundir(UserIndex);
                        else
                            Protocol.WriteConsoleMsg(UserIndex,
                                "No tienes los conocimientos suficientes en herrería para fundir este objeto.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                    }
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in FundirArmas: " + ex.Message);
            var argdesc = "Error en FundirArmas. Error " + ex.GetType().Name + " : " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    public static bool TieneObjetos(short ItemIndex, short cant, short UserIndex)
    {
        bool TieneObjetosRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short i;
        var Total = default(int);
        var loopTo = (short)Declaraciones.UserList[UserIndex].CurrentInventorySlots;
        for (i = 1; i <= loopTo; i++)
            if (Declaraciones.UserList[UserIndex].Invent.userObj[i].ObjIndex == ItemIndex)
                Total = Total + Declaraciones.UserList[UserIndex].Invent.userObj[i].Amount;

        if (cant <= Total)
        {
            TieneObjetosRet = true;
            return TieneObjetosRet;
        }

        return TieneObjetosRet;
    }

    public static void QuitarObjetos(short ItemIndex, short cant, short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 05/08/09
        // 05/08/09: Pato - Cambie la funcion a procedimiento ya que se usa como procedimiento siempre, y fixie el bug 2788199
        // ***************************************************

        short i;
        var loopTo = (short)Declaraciones.UserList[UserIndex].CurrentInventorySlots;
        for (i = 1; i <= loopTo; i++)
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex].Invent.userObj[i];
            if (withBlock.ObjIndex == ItemIndex)
            {
                if ((withBlock.Amount <= cant) & (withBlock.Equipped == 1))
                    InvUsuario.Desequipar(UserIndex, Convert.ToByte(i));

                withBlock.Amount = (short)(withBlock.Amount - cant);
                if (withBlock.Amount <= 0)
                {
                    cant = Math.Abs(withBlock.Amount);
                    Declaraciones.UserList[UserIndex].Invent.NroItems =
                        Convert.ToInt16(Declaraciones.UserList[UserIndex].Invent.NroItems - 1);
                    withBlock.Amount = 0;
                    withBlock.ObjIndex = 0;
                }
                else
                {
                    cant = 0;
                }

                InvUsuario.UpdateUserInv(false, UserIndex, Convert.ToByte(i));

                if (cant == 0)
                    return;
            }
        }
    }

    public static void HerreroQuitarMateriales(short UserIndex, short ItemIndex, short CantidadItems)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 16/11/2009
        // 16/11/2009: ZaMa - Ahora considera la cantidad de items a construir
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.objData[ItemIndex];
            if (withBlock.LingH > 0)
                QuitarObjetos(Declaraciones.LingoteHierro, (short)(withBlock.LingH * CantidadItems), UserIndex);
            if (withBlock.LingP > 0)
                QuitarObjetos(Declaraciones.LingotePlata, (short)(withBlock.LingP * CantidadItems), UserIndex);
            if (withBlock.LingO > 0)
                QuitarObjetos(Declaraciones.LingoteOro, (short)(withBlock.LingO * CantidadItems), UserIndex);
        }
    }

    public static void CarpinteroQuitarMateriales(short UserIndex, short ItemIndex, short CantidadItems)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 16/11/2009
        // 16/11/2009: ZaMa - Ahora quita tambien madera elfica
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.objData[ItemIndex];
            if (withBlock.Madera > 0)
                QuitarObjetos(Declaraciones.Leña, (short)(withBlock.Madera * CantidadItems), UserIndex);
            if (withBlock.MaderaElfica > 0)
                QuitarObjetos(Declaraciones.LeñaElfica, (short)(withBlock.MaderaElfica * CantidadItems), UserIndex);
        }
    }

    public static bool CarpinteroTieneMateriales(short UserIndex, short ItemIndex, short Cantidad, bool ShowMsg = false)
    {
        bool CarpinteroTieneMaterialesRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: 16/11/2009
        // 16/11/2009: ZaMa - Agregada validacion a madera elfica.
        // 16/11/2009: ZaMa - Ahora considera la cantidad de items a construir
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.objData[ItemIndex];
            if (withBlock.Madera > 0)
                if (!TieneObjetos(Declaraciones.Leña, (short)(withBlock.Madera * Cantidad), UserIndex))
                {
                    if (ShowMsg)
                        Protocol.WriteConsoleMsg(UserIndex, "No tienes suficiente madera.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                    CarpinteroTieneMaterialesRet = false;
                    return CarpinteroTieneMaterialesRet;
                }

            if (withBlock.MaderaElfica > 0)
                if (!TieneObjetos(Declaraciones.LeñaElfica, (short)(withBlock.MaderaElfica * Cantidad), UserIndex))
                {
                    if (ShowMsg)
                        Protocol.WriteConsoleMsg(UserIndex, "No tienes suficiente madera élfica.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                    CarpinteroTieneMaterialesRet = false;
                    return CarpinteroTieneMaterialesRet;
                }
        }
        CarpinteroTieneMaterialesRet = true;
        return CarpinteroTieneMaterialesRet;
    }

    public static bool HerreroTieneMateriales(short UserIndex, short ItemIndex, short CantidadItems)
    {
        bool HerreroTieneMaterialesRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: 16/11/2009
        // 16/11/2009: ZaMa - Agregada validacion a madera elfica.
        // ***************************************************
        {
            ref var withBlock = ref Declaraciones.objData[ItemIndex];
            if (withBlock.LingH > 0)
                if (!TieneObjetos(Declaraciones.LingoteHierro, (short)(withBlock.LingH * CantidadItems), UserIndex))
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de hierro.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    HerreroTieneMaterialesRet = false;
                    return HerreroTieneMaterialesRet;
                }

            if (withBlock.LingP > 0)
                if (!TieneObjetos(Declaraciones.LingotePlata, (short)(withBlock.LingP * CantidadItems), UserIndex))
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de plata.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    HerreroTieneMaterialesRet = false;
                    return HerreroTieneMaterialesRet;
                }

            if (withBlock.LingO > 0)
                if (!TieneObjetos(Declaraciones.LingoteOro, (short)(withBlock.LingO * CantidadItems), UserIndex))
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de oro.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    HerreroTieneMaterialesRet = false;
                    return HerreroTieneMaterialesRet;
                }
        }
        HerreroTieneMaterialesRet = true;
        return HerreroTieneMaterialesRet;
    }

    public static bool TieneMaterialesUpgrade(short UserIndex, short ItemIndex)
    {
        bool TieneMaterialesUpgradeRet = default;
        // ***************************************************
        // Author: Torres Patricio (Pato)
        // Last Modification: 12/08/2009
        // 
        // ***************************************************
        short ItemUpgrade;

        ItemUpgrade = Declaraciones.objData[ItemIndex].Upgrade;

        {
            ref var withBlock = ref Declaraciones.objData[ItemUpgrade];
            if (withBlock.LingH > 0)
                if (!TieneObjetos(Declaraciones.LingoteHierro,
                        Convert.ToInt16(withBlock.LingH - Declaraciones.objData[ItemIndex].LingH *
                            Declaraciones.PORCENTAJE_MATERIALES_UPGRADE), UserIndex))

                {
                    Protocol.WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de hierro.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    TieneMaterialesUpgradeRet = false;
                    return TieneMaterialesUpgradeRet;
                }

            if (withBlock.LingP > 0)
                if (!TieneObjetos(Declaraciones.LingotePlata,
                        Convert.ToInt16(withBlock.LingP - Declaraciones.objData[ItemIndex].LingP *
                            Declaraciones.PORCENTAJE_MATERIALES_UPGRADE), UserIndex))

                {
                    Protocol.WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de plata.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    TieneMaterialesUpgradeRet = false;
                    return TieneMaterialesUpgradeRet;
                }

            if (withBlock.LingO > 0)
                if (!TieneObjetos(Declaraciones.LingoteOro,
                        Convert.ToInt16(withBlock.LingO - Declaraciones.objData[ItemIndex].LingO *
                            Declaraciones.PORCENTAJE_MATERIALES_UPGRADE), UserIndex))

                {
                    Protocol.WriteConsoleMsg(UserIndex, "No tienes suficientes lingotes de oro.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    TieneMaterialesUpgradeRet = false;
                    return TieneMaterialesUpgradeRet;
                }

            if (withBlock.Madera > 0)
                if (!TieneObjetos(Declaraciones.Leña,
                        Convert.ToInt16(withBlock.Madera - Declaraciones.objData[ItemIndex].Madera *
                            Declaraciones.PORCENTAJE_MATERIALES_UPGRADE), UserIndex))

                {
                    Protocol.WriteConsoleMsg(UserIndex, "No tienes suficiente madera.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    TieneMaterialesUpgradeRet = false;
                    return TieneMaterialesUpgradeRet;
                }

            if (withBlock.MaderaElfica > 0)
                if (!TieneObjetos(Declaraciones.LeñaElfica,
                        Convert.ToInt16(withBlock.MaderaElfica - Declaraciones.objData[ItemIndex].MaderaElfica *
                            Declaraciones.PORCENTAJE_MATERIALES_UPGRADE), UserIndex))


                {
                    Protocol.WriteConsoleMsg(UserIndex, "No tienes suficiente madera élfica.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    TieneMaterialesUpgradeRet = false;
                    return TieneMaterialesUpgradeRet;
                }
        }

        TieneMaterialesUpgradeRet = true;
        return TieneMaterialesUpgradeRet;
    }

    public static void QuitarMaterialesUpgrade(short UserIndex, short ItemIndex)
    {
        // ***************************************************
        // Author: Torres Patricio (Pato)
        // Last Modification: 12/08/2009
        // 
        // ***************************************************
        short ItemUpgrade;

        ItemUpgrade = Declaraciones.objData[ItemIndex].Upgrade;

        {
            ref var withBlock = ref Declaraciones.objData[ItemUpgrade];
            if (withBlock.LingH > 0)
                QuitarObjetos(Declaraciones.LingoteHierro,
                    Convert.ToInt16(withBlock.LingH - Declaraciones.objData[ItemIndex].LingH *
                        Declaraciones.PORCENTAJE_MATERIALES_UPGRADE), UserIndex);
            if (withBlock.LingP > 0)
                QuitarObjetos(Declaraciones.LingotePlata,
                    Convert.ToInt16(withBlock.LingP - Declaraciones.objData[ItemIndex].LingP *
                        Declaraciones.PORCENTAJE_MATERIALES_UPGRADE), UserIndex);
            if (withBlock.LingO > 0)
                QuitarObjetos(Declaraciones.LingoteOro,
                    Convert.ToInt16(withBlock.LingO - Declaraciones.objData[ItemIndex].LingO *
                        Declaraciones.PORCENTAJE_MATERIALES_UPGRADE), UserIndex);
            if (withBlock.Madera > 0)
                QuitarObjetos(Declaraciones.Leña,
                    Convert.ToInt16(withBlock.Madera - Declaraciones.objData[ItemIndex].Madera *
                        Declaraciones.PORCENTAJE_MATERIALES_UPGRADE), UserIndex);
            if (withBlock.MaderaElfica > 0)
                QuitarObjetos(Declaraciones.LeñaElfica,
                    Convert.ToInt16(withBlock.MaderaElfica - Declaraciones.objData[ItemIndex].MaderaElfica *
                        Declaraciones.PORCENTAJE_MATERIALES_UPGRADE), UserIndex);
        }

        QuitarObjetos(ItemIndex, 1, UserIndex);
    }

    public static bool PuedeConstruir(short UserIndex, short ItemIndex, short CantidadItems)
    {
        bool PuedeConstruirRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: 24/08/2009
        // 24/08/2008: ZaMa - Validates if the player has the required skill
        // 16/11/2009: ZaMa - Validates if the player has the required amount of materials, depending on the number of items to make
        // ***************************************************
        PuedeConstruirRet = HerreroTieneMateriales(UserIndex, ItemIndex, CantidadItems) &
                            (Math.Round(
                                 Declaraciones.UserList[UserIndex].Stats
                                     .UserSkills[(int)Declaraciones.eSkill.Herreria] /
                                 ModHerreriA(Declaraciones.UserList[UserIndex].clase), 0) >=
                             Declaraciones.objData[ItemIndex].SkHerreria);
        return PuedeConstruirRet;
    }

    public static bool PuedeConstruirHerreria(short ItemIndex)
    {
        bool PuedeConstruirHerreriaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************
        int i;

        var loopTo = Declaraciones.ArmasHerrero.Length - 1;
        for (i = 1; i <= loopTo; i++)
            if (Declaraciones.ArmasHerrero[i] == ItemIndex)
            {
                PuedeConstruirHerreriaRet = true;
                return PuedeConstruirHerreriaRet;
            }

        var loopTo1 = Declaraciones.ArmadurasHerrero.Length - 1;
        for (i = 1; i <= loopTo1; i++)
            if (Declaraciones.ArmadurasHerrero[i] == ItemIndex)
            {
                PuedeConstruirHerreriaRet = true;
                return PuedeConstruirHerreriaRet;
            }

        PuedeConstruirHerreriaRet = false;
        return PuedeConstruirHerreriaRet;
    }

    public static void HerreroConstruirItem(short UserIndex, short ItemIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 16/11/2009
        // 16/11/2009: ZaMa - Implementado nuevo sistema de construccion de items
        // ***************************************************
        short CantidadItems;
        var TieneMateriales = default(bool);

        Declaraciones.Obj MiObj;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            CantidadItems = withBlock.Construir.PorCiclo;

            if (withBlock.Construir.Cantidad < CantidadItems)
                CantidadItems = Convert.ToInt16(withBlock.Construir.Cantidad);

            if (withBlock.Construir.Cantidad > 0)
                withBlock.Construir.Cantidad = withBlock.Construir.Cantidad - CantidadItems;

            if (CantidadItems == 0)
            {
                Protocol.WriteStopWorking(UserIndex);
                return;
            }

            if (PuedeConstruirHerreria(ItemIndex))
            {
                while ((CantidadItems > 0) & !TieneMateriales)
                    if (PuedeConstruir(UserIndex, ItemIndex, CantidadItems))
                        TieneMateriales = true;
                    else
                        CantidadItems = Convert.ToInt16(CantidadItems - 1);

                // Chequeo si puede hacer al menos 1 item
                if (!TieneMateriales)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No tienes suficientes materiales.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    Protocol.WriteStopWorking(UserIndex);
                    return;
                }

                // Sacamos energía
                if (withBlock.clase == Declaraciones.eClass.Worker)
                {
                    // Chequeamos que tenga los puntos antes de sacarselos
                    if (withBlock.Stats.MinSta >= GASTO_ENERGIA_TRABAJADOR)
                    {
                        withBlock.Stats.MinSta = (short)(withBlock.Stats.MinSta - GASTO_ENERGIA_TRABAJADOR);
                        Protocol.WriteUpdateSta(UserIndex);
                    }
                    else
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "No tienes suficiente energía.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }
                }
                // Chequeamos que tenga los puntos antes de sacarselos
                else if (withBlock.Stats.MinSta >= GASTO_ENERGIA_NO_TRABAJADOR)
                {
                    withBlock.Stats.MinSta = (short)(withBlock.Stats.MinSta - GASTO_ENERGIA_NO_TRABAJADOR);
                    Protocol.WriteUpdateSta(UserIndex);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No tienes suficiente energía.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                HerreroQuitarMateriales(UserIndex, ItemIndex, CantidadItems);
                // AGREGAR FX
                if (Declaraciones.objData[ItemIndex].OBJType == Declaraciones.eOBJType.otWeapon)
                    Protocol.WriteConsoleMsg(UserIndex,
                        "Has construido " + (CantidadItems > 1 ? CantidadItems + " armas!" : "el arma!"),
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                else if (Declaraciones.objData[ItemIndex].OBJType == Declaraciones.eOBJType.otESCUDO)
                    Protocol.WriteConsoleMsg(UserIndex,
                        "Has construido " + (CantidadItems > 1 ? CantidadItems + " escudos!" : "el escudo!"),
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                else if (Declaraciones.objData[ItemIndex].OBJType == Declaraciones.eOBJType.otCASCO)
                    Protocol.WriteConsoleMsg(UserIndex,
                        "Has construido " + (CantidadItems > 1 ? CantidadItems + " cascos!" : "el casco!"),
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                else if (Declaraciones.objData[ItemIndex].OBJType == Declaraciones.eOBJType.otArmadura)
                    Protocol.WriteConsoleMsg(UserIndex,
                        "Has construido " + (CantidadItems > 1 ? CantidadItems + " armaduras" : "la armadura!"),
                        Protocol.FontTypeNames.FONTTYPE_INFO);


                MiObj.Amount = CantidadItems;
                MiObj.ObjIndex = ItemIndex;
                if (!InvUsuario.MeterItemEnInventario(UserIndex, ref MiObj))
                {
                    var argNotPirata = true;
                    InvNpc.TirarItemAlPiso(ref withBlock.Pos, ref MiObj, ref argNotPirata);
                }

                // Log de construcción de Items. Pablo (ToxicWaste) 10/09/07
                if (Declaraciones.objData[MiObj.ObjIndex].Log == 1)
                    General.LogDesarrollo(withBlock.name + " ha construído " + MiObj.Amount + " " +
                                          Declaraciones.objData[MiObj.ObjIndex].name);

                UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Herreria, true);
                InvUsuario.UpdateUserInv(true, UserIndex, 0);
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    Protocol.PrepareMessagePlayWave(Declaraciones.MARTILLOHERRERO, Convert.ToByte(withBlock.Pos.X),
                        Convert.ToByte(withBlock.Pos.Y)));

                withBlock.Reputacion.PlebeRep = withBlock.Reputacion.PlebeRep + Declaraciones.vlProleta;
                if (withBlock.Reputacion.PlebeRep > Declaraciones.MAXREP)
                    withBlock.Reputacion.PlebeRep = Declaraciones.MAXREP;

                withBlock.Counters.Trabajando = withBlock.Counters.Trabajando + 1;
            }
        }
    }

    public static bool PuedeConstruirCarpintero(short ItemIndex)
    {
        bool PuedeConstruirCarpinteroRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************
        int i;

        var loopTo = Declaraciones.ObjCarpintero.Length - 1;
        for (i = 1; i <= loopTo; i++)
            if (Declaraciones.ObjCarpintero[i] == ItemIndex)
            {
                PuedeConstruirCarpinteroRet = true;
                return PuedeConstruirCarpinteroRet;
            }

        PuedeConstruirCarpinteroRet = false;
        return PuedeConstruirCarpinteroRet;
    }

    public static void CarpinteroConstruirItem(short UserIndex, short ItemIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 16/11/2009
        // 24/08/2008: ZaMa - Validates if the player has the required skill
        // 16/11/2009: ZaMa - Implementado nuevo sistema de construccion de items
        // ***************************************************
        short CantidadItems;
        var TieneMateriales = default(bool);

        Declaraciones.Obj MiObj;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            CantidadItems = withBlock.Construir.PorCiclo;

            if (withBlock.Construir.Cantidad < CantidadItems)
                CantidadItems = Convert.ToInt16(withBlock.Construir.Cantidad);

            if (withBlock.Construir.Cantidad > 0)
                withBlock.Construir.Cantidad = withBlock.Construir.Cantidad - CantidadItems;

            if (CantidadItems == 0)
            {
                Protocol.WriteStopWorking(UserIndex);
                return;
            }

            if ((Math.Round(
                     (decimal)(withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Carpinteria] /
                               ModCarpinteria(withBlock.clase)), 0) >=
                 Declaraciones.objData[ItemIndex].SkCarpinteria) & PuedeConstruirCarpintero(ItemIndex) &
                (withBlock.Invent.WeaponEqpObjIndex == Declaraciones.SERRUCHO_CARPINTERO))
            {
                // Calculo cuantos item puede construir
                while ((CantidadItems > 0) & !TieneMateriales)
                    if (CarpinteroTieneMateriales(UserIndex, ItemIndex, CantidadItems))
                        TieneMateriales = true;
                    else
                        CantidadItems = Convert.ToInt16(CantidadItems - 1);

                // No tiene los materiales ni para construir 1 item?
                if (!TieneMateriales)
                {
                    // Para que muestre el mensaje
                    CarpinteroTieneMateriales(UserIndex, ItemIndex, 1, true);
                    Protocol.WriteStopWorking(UserIndex);
                    return;
                }

                // Sacamos energía
                if (withBlock.clase == Declaraciones.eClass.Worker)
                {
                    // Chequeamos que tenga los puntos antes de sacarselos
                    if (withBlock.Stats.MinSta >= GASTO_ENERGIA_TRABAJADOR)
                    {
                        withBlock.Stats.MinSta = (short)(withBlock.Stats.MinSta - GASTO_ENERGIA_TRABAJADOR);
                        Protocol.WriteUpdateSta(UserIndex);
                    }
                    else
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "No tienes suficiente energía.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }
                }
                // Chequeamos que tenga los puntos antes de sacarselos
                else if (withBlock.Stats.MinSta >= GASTO_ENERGIA_NO_TRABAJADOR)
                {
                    withBlock.Stats.MinSta = (short)(withBlock.Stats.MinSta - GASTO_ENERGIA_NO_TRABAJADOR);
                    Protocol.WriteUpdateSta(UserIndex);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No tienes suficiente energía.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                CarpinteroQuitarMateriales(UserIndex, ItemIndex, CantidadItems);
                Protocol.WriteConsoleMsg(UserIndex,
                    "Has construido " + CantidadItems + (CantidadItems == 1 ? " objeto!" : " objetos!"),
                    Protocol.FontTypeNames.FONTTYPE_INFO);

                MiObj.Amount = CantidadItems;
                MiObj.ObjIndex = ItemIndex;
                if (!InvUsuario.MeterItemEnInventario(UserIndex, ref MiObj))
                {
                    var argNotPirata = true;
                    InvNpc.TirarItemAlPiso(ref withBlock.Pos, ref MiObj, ref argNotPirata);
                }

                // Log de construcción de Items. Pablo (ToxicWaste) 10/09/07
                if (Declaraciones.objData[MiObj.ObjIndex].Log == 1)
                    General.LogDesarrollo(withBlock.name + " ha construído " + MiObj.Amount + " " +
                                          Declaraciones.objData[MiObj.ObjIndex].name);

                UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Carpinteria, true);
                InvUsuario.UpdateUserInv(true, UserIndex, 0);
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    Protocol.PrepareMessagePlayWave(Declaraciones.LABUROCARPINTERO, Convert.ToByte(withBlock.Pos.X),
                        Convert.ToByte(withBlock.Pos.Y)));


                withBlock.Reputacion.PlebeRep = withBlock.Reputacion.PlebeRep + Declaraciones.vlProleta;
                if (withBlock.Reputacion.PlebeRep > Declaraciones.MAXREP)
                    withBlock.Reputacion.PlebeRep = Declaraciones.MAXREP;

                withBlock.Counters.Trabajando = withBlock.Counters.Trabajando + 1;
            }

            else if (withBlock.Invent.WeaponEqpObjIndex != Declaraciones.SERRUCHO_CARPINTERO)
            {
                Protocol.WriteConsoleMsg(UserIndex, "Debes tener equipado el serrucho para trabajar.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    private static short MineralesParaLingote(Declaraciones.iMinerales Lingote)
    {
        short MineralesParaLingoteRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************
        switch (Lingote)
        {
            case Declaraciones.iMinerales.HierroCrudo:
            {
                MineralesParaLingoteRet = 14;
                break;
            }
            case Declaraciones.iMinerales.PlataCruda:
            {
                MineralesParaLingoteRet = 20;
                break;
            }
            case Declaraciones.iMinerales.OroCrudo:
            {
                MineralesParaLingoteRet = 35;
                break;
            }

            default:
            {
                MineralesParaLingoteRet = 10000;
                break;
            }
        }

        return MineralesParaLingoteRet;
    }


    public static void DoLingotes(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 16/11/2009
        // 16/11/2009: ZaMa - Implementado nuevo sistema de construccion de items
        // ***************************************************
        // Call LogTarea("Sub DoLingotes")
        short Slot;
        short obji;
        short CantidadItems;
        var TieneMinerales = default(bool);

        Declaraciones.Obj MiObj;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            CantidadItems =
                Convert.ToInt16(SistemaCombate.MaximoInt(1, Convert.ToInt32((withBlock.Stats.ELV - 4) / 5d)));

            Slot = withBlock.flags.TargetObjInvSlot;
            obji = withBlock.Invent.userObj[Slot].ObjIndex;

            while ((CantidadItems > 0) & !TieneMinerales)
                if (withBlock.Invent.userObj[Slot].Amount >=
                    (short)(MineralesParaLingote((Declaraciones.iMinerales)obji) * CantidadItems))
                    TieneMinerales = true;
                else
                    CantidadItems = Convert.ToInt16(CantidadItems - 1);

            if (!TieneMinerales | (Declaraciones.objData[obji].OBJType != Declaraciones.eOBJType.otMinerales))
            {
                Protocol.WriteConsoleMsg(UserIndex, "No tienes suficientes minerales para hacer un lingote.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                return;
            }

            withBlock.Invent.userObj[Slot].Amount = (short)(withBlock.Invent.userObj[Slot].Amount -
                                                                   (short)(MineralesParaLingote(
                                                                               (Declaraciones.iMinerales)obji) *
                                                                           CantidadItems));
            if (withBlock.Invent.userObj[Slot].Amount < 1)
            {
                withBlock.Invent.userObj[Slot].Amount = 0;
                withBlock.Invent.userObj[Slot].ObjIndex = 0;
            }

            MiObj.Amount = CantidadItems;
            MiObj.ObjIndex = Declaraciones.objData[withBlock.flags.TargetObjInvIndex].LingoteIndex;
            if (!InvUsuario.MeterItemEnInventario(UserIndex, ref MiObj))
            {
                var argNotPirata = true;
                InvNpc.TirarItemAlPiso(ref withBlock.Pos, ref MiObj, ref argNotPirata);
            }

            InvUsuario.UpdateUserInv(false, UserIndex, Convert.ToByte(Slot));
            Protocol.WriteConsoleMsg(UserIndex,
                "¡Has obtenido " + CantidadItems + " lingote" + (CantidadItems == 1 ? "" : "s") + "!",
                Protocol.FontTypeNames.FONTTYPE_INFO);

            withBlock.Counters.Trabajando = withBlock.Counters.Trabajando + 1;
        }
    }

    public static void DoFundir(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 03/06/2010
        // 03/06/2010 - Pato: Si es el último ítem a fundir y está equipado lo desequipamos.
        // 11/03/2010 - ZaMa: Reemplazo división por producto para uan mejor performanse.
        // ***************************************************
        short i;
        short num;
        byte Slot;
        var Lingotes = new short[3];

        var MiObj = new Declaraciones.Obj[3];
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            Slot = Convert.ToByte(withBlock.flags.TargetObjInvSlot);

            {
                ref var withBlock1 = ref withBlock.Invent.userObj[Slot];
                withBlock1.Amount = Convert.ToInt16(withBlock1.Amount - 1);

                if (withBlock1.Amount < 1)
                {
                    if (withBlock1.Equipped == 1)
                        InvUsuario.Desequipar(UserIndex, Slot);

                    withBlock1.Amount = 0;
                    withBlock1.ObjIndex = 0;
                }
            }

            num = Convert.ToInt16(Matematicas.RandomNumber(10, 25));

            Lingotes[0] = Convert.ToInt16(Declaraciones.objData[withBlock.flags.TargetObjInvIndex].LingH * num *
                                          0.01d);
            Lingotes[1] = Convert.ToInt16(Declaraciones.objData[withBlock.flags.TargetObjInvIndex].LingP * num *
                                          0.01d);
            Lingotes[2] = Convert.ToInt16(Declaraciones.objData[withBlock.flags.TargetObjInvIndex].LingO * num *
                                          0.01d);


            for (i = 0; i <= 2; i++)
            {
                MiObj[i].Amount = Lingotes[i];
                MiObj[i].ObjIndex = (short)(Declaraciones.LingoteHierro + i); // Una gran negrada pero práctica
                if (MiObj[i].Amount > 0)
                {
                    if (!InvUsuario.MeterItemEnInventario(UserIndex, ref MiObj[i]))
                    {
                        var argNotPirata = true;
                        InvNpc.TirarItemAlPiso(ref withBlock.Pos, ref MiObj[i], ref argNotPirata);
                    }

                    InvUsuario.UpdateUserInv(true, UserIndex, Slot);
                }
            }

            Protocol.WriteConsoleMsg(UserIndex,
                "¡Has obtenido el " + num + "% de los lingotes utilizados para la construcción del objeto!",
                Protocol.FontTypeNames.FONTTYPE_INFO);

            withBlock.Counters.Trabajando = withBlock.Counters.Trabajando + 1;
        }
    }

    public static void DoUpgrade(short UserIndex, short ItemIndex)
    {
        // ***************************************************
        // Author: Torres Patricio (Pato)
        // Last Modification: 12/08/2009
        // 12/08/2009: Pato - Implementado nuevo sistema de mejora de items
        // ***************************************************
        short ItemUpgrade;

        ItemUpgrade = Declaraciones.objData[ItemIndex].Upgrade;

        Declaraciones.Obj MiObj;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Sacamos energía
            if (withBlock.clase == Declaraciones.eClass.Worker)
            {
                // Chequeamos que tenga los puntos antes de sacarselos
                if (withBlock.Stats.MinSta >= GASTO_ENERGIA_TRABAJADOR)
                {
                    withBlock.Stats.MinSta = (short)(withBlock.Stats.MinSta - GASTO_ENERGIA_TRABAJADOR);
                    Protocol.WriteUpdateSta(UserIndex);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No tienes suficiente energía.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return;
                }
            }
            // Chequeamos que tenga los puntos antes de sacarselos
            else if (withBlock.Stats.MinSta >= GASTO_ENERGIA_NO_TRABAJADOR)
            {
                withBlock.Stats.MinSta = (short)(withBlock.Stats.MinSta - GASTO_ENERGIA_NO_TRABAJADOR);
                Protocol.WriteUpdateSta(UserIndex);
            }
            else
            {
                Protocol.WriteConsoleMsg(UserIndex, "No tienes suficiente energía.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if (ItemUpgrade <= 0)
                return;
            if (!TieneMaterialesUpgrade(UserIndex, ItemIndex))
                return;

            if (PuedeConstruirHerreria(ItemUpgrade))
            {
                if (withBlock.Invent.WeaponEqpObjIndex != Declaraciones.MARTILLO_HERRERO)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Debes equiparte el martillo de herrero.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                if (Math.Round(
                        withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Herreria] / ModHerreriA(withBlock.clase),
                        0) < Declaraciones.objData[ItemUpgrade].SkHerreria)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No tienes suficientes skills.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                switch (Declaraciones.objData[ItemIndex].OBJType)
                {
                    case Declaraciones.eOBJType.otWeapon:
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "Has mejorado el arma!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        break;
                    }

                    case Declaraciones.eOBJType.otESCUDO: // Todavía no hay, pero just in case
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "Has mejorado el escudo!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        break;
                    }

                    case Declaraciones.eOBJType.otCASCO:
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "Has mejorado el casco!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        break;
                    }

                    case Declaraciones.eOBJType.otArmadura:
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "Has mejorado la armadura!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        break;
                    }
                }

                UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Herreria, true);
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    Protocol.PrepareMessagePlayWave(Declaraciones.MARTILLOHERRERO, Convert.ToByte(withBlock.Pos.X),
                        Convert.ToByte(withBlock.Pos.Y)));
            }

            else if (PuedeConstruirCarpintero(ItemUpgrade))
            {
                if (withBlock.Invent.WeaponEqpObjIndex != Declaraciones.SERRUCHO_CARPINTERO)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Debes equiparte el serrucho.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                if (Math.Round(
                        (decimal)(withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Carpinteria] /
                                  ModCarpinteria(withBlock.clase)), 0) <
                    Declaraciones.objData[ItemUpgrade].SkCarpinteria)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No tienes suficientes skills.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                switch (Declaraciones.objData[ItemIndex].OBJType)
                {
                    case Declaraciones.eOBJType.otFlechas:
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "Has mejorado la flecha!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        break;
                    }

                    case Declaraciones.eOBJType.otWeapon:
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "Has mejorado el arma!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        break;
                    }

                    case Declaraciones.eOBJType.otBarcos:
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "Has mejorado el barco!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        break;
                    }
                }

                UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Carpinteria, true);
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    Protocol.PrepareMessagePlayWave(Declaraciones.LABUROCARPINTERO, Convert.ToByte(withBlock.Pos.X),
                        Convert.ToByte(withBlock.Pos.Y)));
            }
            else
            {
                return;
            }

            QuitarMaterialesUpgrade(UserIndex, ItemIndex);

            MiObj.Amount = 1;
            MiObj.ObjIndex = ItemUpgrade;

            if (!InvUsuario.MeterItemEnInventario(UserIndex, ref MiObj))
            {
                var argNotPirata = true;
                InvNpc.TirarItemAlPiso(ref withBlock.Pos, ref MiObj, ref argNotPirata);
            }

            if (Declaraciones.objData[ItemIndex].Log == 1)
                General.LogDesarrollo(withBlock.name + " ha mejorado el ítem " +
                                      Declaraciones.objData[ItemIndex].name + " a " +
                                      Declaraciones.objData[ItemUpgrade].name);

            InvUsuario.UpdateUserInv(true, UserIndex, 0);

            withBlock.Reputacion.PlebeRep = withBlock.Reputacion.PlebeRep + Declaraciones.vlProleta;
            if (withBlock.Reputacion.PlebeRep > Declaraciones.MAXREP)
                withBlock.Reputacion.PlebeRep = Declaraciones.MAXREP;

            withBlock.Counters.Trabajando = withBlock.Counters.Trabajando + 1;
        }
    }

    public static float ModNavegacion(Declaraciones.eClass clase, short UserIndex)
    {
        float ModNavegacionRet = default;
        // ***************************************************
        // Autor: Unknown (orginal version)
        // Last Modification: 27/11/2009
        // 27/11/2009: ZaMa - A worker can navigate before only if it's an expert fisher
        // 12/04/2010: ZaMa - Arreglo modificador de pescador, para que navegue con 60 skills.
        // ***************************************************
        switch (clase)
        {
            case Declaraciones.eClass.Pirat:
            {
                ModNavegacionRet = 1f;
                break;
            }
            case Declaraciones.eClass.Worker:
            {
                if (Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Pesca] == 100)
                    ModNavegacionRet = 1.71f;
                else
                    ModNavegacionRet = 2f;

                break;
            }

            default:
            {
                ModNavegacionRet = 2f;
                break;
            }
        }

        return ModNavegacionRet;
    }


    public static float ModFundicion(Declaraciones.eClass clase)
    {
        float ModFundicionRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        switch (clase)
        {
            case Declaraciones.eClass.Worker:
            {
                ModFundicionRet = 1f;
                break;
            }

            default:
            {
                ModFundicionRet = 3f;
                break;
            }
        }

        return ModFundicionRet;
    }

    public static short ModCarpinteria(Declaraciones.eClass clase)
    {
        short ModCarpinteriaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        switch (clase)
        {
            case Declaraciones.eClass.Worker:
            {
                ModCarpinteriaRet = 1;
                break;
            }

            default:
            {
                ModCarpinteriaRet = 3;
                break;
            }
        }

        return ModCarpinteriaRet;
    }

    public static float ModHerreriA(Declaraciones.eClass clase)
    {
        float ModHerreriARet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************
        switch (clase)
        {
            case Declaraciones.eClass.Worker:
            {
                ModHerreriARet = 1f;
                break;
            }

            default:
            {
                ModHerreriARet = 4f;
                break;
            }
        }

        return ModHerreriARet;
    }

    public static short ModDomar(Declaraciones.eClass clase)
    {
        short ModDomarRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************
        switch (clase)
        {
            case Declaraciones.eClass.Druid:
            {
                ModDomarRet = 6;
                break;
            }
            case Declaraciones.eClass.Hunter:
            {
                ModDomarRet = 6;
                break;
            }
            case Declaraciones.eClass.Cleric:
            {
                ModDomarRet = 7;
                break;
            }

            default:
            {
                ModDomarRet = 10;
                break;
            }
        }

        return ModDomarRet;
    }

    public static short FreeMascotaIndex(short UserIndex)
    {
        short FreeMascotaIndexRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: 02/03/09
        // 02/03/09: ZaMa - Busca un indice libre de mascotas, revisando los types y no los indices de los npcs
        // ***************************************************
        short j;
        for (j = 1; j <= Declaraciones.MAXMASCOTAS; j++)
            if (Declaraciones.UserList[UserIndex].MascotasType[j] == 0)
            {
                FreeMascotaIndexRet = j;
                return FreeMascotaIndexRet;
            }

        return FreeMascotaIndexRet;
    }

    public static void DoDomar(short UserIndex, short NpcIndex)
    {
        // ***************************************************
        // Author: Nacho (Integer)
        // Last Modification: 01/05/2010
        // 12/15/2008: ZaMa - Limits the number of the same type of pet to 2.
        // 02/03/2009: ZaMa - Las criaturas domadas en zona segura, esperan afuera (desaparecen).
        // 01/05/2010: ZaMa - Agrego bonificacion 11% para domar con flauta magica.
        // ***************************************************

        try
        {
            short puntosDomar;
            short puntosRequeridos;
            bool CanStay;
            short petType;
            short NroPets;


            if (Declaraciones.Npclist[NpcIndex].MaestroUser == UserIndex)
            {
                Protocol.WriteConsoleMsg(UserIndex, "Ya domaste a esa criatura.", Protocol.FontTypeNames.FONTTYPE_INFO);
                return;
            }

            short index;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                if (withBlock.NroMascotas < Declaraciones.MAXMASCOTAS)
                {
                    if ((Declaraciones.Npclist[NpcIndex].MaestroNpc > 0) |
                        (Declaraciones.Npclist[NpcIndex].MaestroUser > 0))
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "La criatura ya tiene amo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    if (!PuedeDomarMascota(UserIndex, NpcIndex))
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "No puedes domar más de dos criaturas del mismo tipo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    puntosDomar =
                        (short)(Convert.ToInt16(withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Carisma]) *
                                Convert.ToInt16(withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Domar]));

                    // 20% de bonificacion
                    if (withBlock.Invent.AnilloEqpObjIndex == Declaraciones.FLAUTAELFICA)
                        puntosRequeridos = Convert.ToInt16(Declaraciones.Npclist[NpcIndex].flags.Domable * 0.8d);

                    // 11% de bonificacion
                    else if (withBlock.Invent.AnilloEqpObjIndex == Declaraciones.FLAUTAMAGICA)
                        puntosRequeridos = Convert.ToInt16(Declaraciones.Npclist[NpcIndex].flags.Domable * 0.89d);

                    else
                        puntosRequeridos = Declaraciones.Npclist[NpcIndex].flags.Domable;

                    if ((puntosRequeridos <= puntosDomar) & (Matematicas.RandomNumber(1, 5) == 1))
                    {
                        withBlock.NroMascotas = Convert.ToInt16(withBlock.NroMascotas + 1);
                        index = FreeMascotaIndex(UserIndex);
                        withBlock.MascotasIndex[index] = NpcIndex;
                        withBlock.MascotasType[index] = Declaraciones.Npclist[NpcIndex].Numero;

                        Declaraciones.Npclist[NpcIndex].MaestroUser = UserIndex;

                        NPCs.FollowAmo(NpcIndex);
                        NPCs.ReSpawnNpc(ref Declaraciones.Npclist[NpcIndex]);

                        Protocol.WriteConsoleMsg(UserIndex, "La criatura te ha aceptado como su amo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);

                        // Es zona segura?
                        CanStay = Declaraciones.mapInfo[withBlock.Pos.Map].Pk;

                        if (!CanStay)
                        {
                            petType = Declaraciones.Npclist[NpcIndex].Numero;
                            NroPets = withBlock.NroMascotas;

                            NPCs.QuitarNPC(NpcIndex);

                            withBlock.MascotasType[index] = petType;
                            withBlock.NroMascotas = NroPets;

                            Protocol.WriteConsoleMsg(UserIndex,
                                "No se permiten mascotas en zona segura. Éstas te esperarán afuera.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                        }

                        UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Domar, true);
                    }

                    else
                    {
                        if (!(withBlock.flags.UltimoMensaje == 5))
                        {
                            Protocol.WriteConsoleMsg(UserIndex, "No has logrado domar la criatura.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            withBlock.flags.UltimoMensaje = 5;
                        }

                        UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Domar, false);
                    }
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No puedes controlar más criaturas.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                }
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in TieneObjetos: " + ex.Message);
            var argdesc = "Error en DoDomar. Error " + ex.GetType().Name + " : " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    // '
    // Checks if the user can tames a pet.
    // 
    // @param integer userIndex The user id from who wants tame the pet.
    // @param integer NPCindex The index of the npc to tome.
    // @return boolean True if can, false if not.
    private static bool PuedeDomarMascota(short UserIndex, short NpcIndex)
    {
        bool PuedeDomarMascotaRet = default;
        // ***************************************************
        // Author: ZaMa
        // This function checks how many NPCs of the same type have
        // been tamed by the user.
        // Returns True if that amount is less than two.
        // ***************************************************
        int i;
        var numMascotas = default(int);

        for (i = 1; i <= Declaraciones.MAXMASCOTAS; i++)
            if (Declaraciones.UserList[UserIndex].MascotasType[i] == Declaraciones.Npclist[NpcIndex].Numero)
                numMascotas = numMascotas + 1;

        if (numMascotas <= 1)
            PuedeDomarMascotaRet = true;
        return PuedeDomarMascotaRet;
    }

    public static void DoAdminInvisible(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 12/01/2010 (ZaMa)
        // Makes an admin invisible o visible.
        // 13/07/2009: ZaMa - Now invisible admins' chars are erased from all clients, except from themselves.
        // 12/01/2010: ZaMa - Los druidas pierden la inmunidad de ser atacados cuando pierden el efecto del mimetismo.
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.flags.AdminInvisible == 0)
            {
                // Sacamos el mimetizmo
                if (withBlock.flags.Mimetizado == 1)
                {
                    withBlock.character.body = withBlock.CharMimetizado.body;
                    withBlock.character.Head = withBlock.CharMimetizado.Head;
                    withBlock.character.CascoAnim = withBlock.CharMimetizado.CascoAnim;
                    withBlock.character.ShieldAnim = withBlock.CharMimetizado.ShieldAnim;
                    withBlock.character.WeaponAnim = withBlock.CharMimetizado.WeaponAnim;
                    withBlock.Counters.Mimetismo = 0;
                    withBlock.flags.Mimetizado = 0;
                    // Se fue el efecto del mimetismo, puede ser atacado por npcs
                    withBlock.flags.Ignorado = false;
                }

                withBlock.flags.AdminInvisible = 1;
                withBlock.flags.invisible = 1;
                withBlock.flags.Oculto = 1;
                withBlock.flags.OldBody = withBlock.character.body;
                withBlock.flags.OldHead = withBlock.character.Head;
                withBlock.character.body = 0;
                withBlock.character.Head = 0;

                // Solo el admin sabe que se hace invi
                var argdatos = Protocol.PrepareMessageSetInvisible(withBlock.character.CharIndex, true);
                TCP.EnviarDatosASlot(UserIndex, ref argdatos);
                // Le mandamos el mensaje para que borre el personaje a los clientes que estén cerca
                modSendData.SendData(modSendData.SendTarget.ToPCAreaButIndex, UserIndex,
                    Protocol.PrepareMessageCharacterRemove(withBlock.character.CharIndex));
            }
            else
            {
                withBlock.flags.AdminInvisible = 0;
                withBlock.flags.invisible = 0;
                withBlock.flags.Oculto = 0;
                withBlock.Counters.TiempoOculto = 0;
                withBlock.character.body = withBlock.flags.OldBody;
                withBlock.character.Head = withBlock.flags.OldHead;

                // Solo el admin sabe que se hace visible
                var argdatos1 = Protocol.PrepareMessageCharacterChange(withBlock.character.body,
                    withBlock.character.Head, withBlock.character.heading, withBlock.character.CharIndex,
                    withBlock.character.WeaponAnim, withBlock.character.ShieldAnim, withBlock.character.FX,
                    withBlock.character.loops, withBlock.character.CascoAnim);
                TCP.EnviarDatosASlot(UserIndex, ref argdatos1);
                var argdatos2 = Protocol.PrepareMessageSetInvisible(withBlock.character.CharIndex, false);
                TCP.EnviarDatosASlot(UserIndex, ref argdatos2);

                // Le mandamos el mensaje para crear el personaje a los clientes que estén cerca
                var argButIndex = true;
                UsUaRiOs.MakeUserChar(true, withBlock.Pos.Map, UserIndex, withBlock.Pos.Map, withBlock.Pos.X,
                    withBlock.Pos.Y, ref argButIndex);
            }
        }
    }

    public static void TratarDeHacerFogata(short Map, short X, short Y, short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        var Suerte = default(byte);
        byte exito;
        Declaraciones.Obj obj;
        Declaraciones.WorldPos posMadera = default;

        if (!Extra.LegalPos(Map, X, Y))
            return;

        {
            ref var withBlock = ref posMadera;
            withBlock.Map = Map;
            withBlock.X = X;
            withBlock.Y = Y;
        }

        if (Declaraciones.MapData[Map, X, Y].ObjInfo.ObjIndex != 58)
        {
            Protocol.WriteConsoleMsg(UserIndex, "Necesitas clickear sobre leña para hacer ramitas.",
                Protocol.FontTypeNames.FONTTYPE_INFO);
            return;
        }

        if (Matematicas.Distancia(ref posMadera, ref Declaraciones.UserList[UserIndex].Pos) > 2)
        {
            Protocol.WriteConsoleMsg(UserIndex, "Estás demasiado lejos para prender la fogata.",
                Protocol.FontTypeNames.FONTTYPE_INFO);
            return;
        }

        if (Declaraciones.UserList[UserIndex].flags.Muerto == 1)
        {
            Protocol.WriteConsoleMsg(UserIndex, "No puedes hacer fogatas estando muerto.",
                Protocol.FontTypeNames.FONTTYPE_INFO);
            return;
        }

        if (Declaraciones.MapData[Map, X, Y].ObjInfo.Amount < 3)
        {
            Protocol.WriteConsoleMsg(UserIndex, "Necesitas por lo menos tres troncos para hacer una fogata.",
                Protocol.FontTypeNames.FONTTYPE_INFO);
            return;
        }

        byte SupervivenciaSkill;

        SupervivenciaSkill =
            Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Supervivencia];

        if ((SupervivenciaSkill >= 0) & (SupervivenciaSkill < 6))
            Suerte = 3;
        else if ((SupervivenciaSkill >= 6) & (SupervivenciaSkill <= 34))
            Suerte = 2;
        else if (SupervivenciaSkill >= 35) Suerte = 1;

        exito = Convert.ToByte(Matematicas.RandomNumber(1, Suerte));

        if (exito == 1)
        {
            obj.ObjIndex = Declaraciones.FOGATA_APAG;
            obj.Amount = Convert.ToInt16(Declaraciones.MapData[Map, X, Y].ObjInfo.Amount / 3);

            Protocol.WriteConsoleMsg(UserIndex, "Has hecho " + obj.Amount + " fogatas.",
                Protocol.FontTypeNames.FONTTYPE_INFO);

            InvUsuario.MakeObj(ref obj, Map, X, Y);

            // Seteamos la fogata como el nuevo TargetObj del user
            Declaraciones.UserList[UserIndex].flags.TargetObj = Declaraciones.FOGATA_APAG;

            UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Supervivencia, true);
        }
        else
        {
            // [CDT 17-02-2004]
            if (!(Declaraciones.UserList[UserIndex].flags.UltimoMensaje == 10))
            {
                Protocol.WriteConsoleMsg(UserIndex, "No has podido hacer la fogata.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                Declaraciones.UserList[UserIndex].flags.UltimoMensaje = 10;
            }
            // [/CDT]

            UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Supervivencia, false);
        }
    }

    public static void DoPescar(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 16/11/2009
        // 16/11/2009: ZaMa - Implementado nuevo sistema de extraccion.
        // ***************************************************
        try
        {
            short Suerte;
            short res;
            short CantidadItems;

            if (Declaraciones.UserList[UserIndex].clase == Declaraciones.eClass.Worker)
                QuitarSta(UserIndex, Declaraciones.EsfuerzoPescarPescador);
            else
                QuitarSta(UserIndex, Declaraciones.EsfuerzoPescarGeneral);

            short Skill;
            Skill = Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Pesca];
            Suerte = Convert.ToInt16(Conversion.Int(-0.00125d * Skill * Skill - 0.3d * Skill + 49d));

            res = Convert.ToInt16(Matematicas.RandomNumber(1, Suerte));

            Declaraciones.Obj MiObj;
            if (res <= 6)
            {
                if (Declaraciones.UserList[UserIndex].clase == Declaraciones.eClass.Worker)
                {
                    {
                        ref var withBlock = ref Declaraciones.UserList[UserIndex];
                        CantidadItems =
                            Convert.ToInt16(1 +
                                            SistemaCombate.MaximoInt(1,
                                                Convert.ToInt32((withBlock.Stats.ELV - 4) / 5d)));
                    }

                    MiObj.Amount = Convert.ToInt16(Matematicas.RandomNumber(1, CantidadItems));
                }
                else
                {
                    MiObj.Amount = 1;
                }

                MiObj.ObjIndex = Declaraciones.Pescado;

                if (!InvUsuario.MeterItemEnInventario(UserIndex, ref MiObj))
                {
                    var argNotPirata = true;
                    InvNpc.TirarItemAlPiso(ref Declaraciones.UserList[UserIndex].Pos, ref MiObj, ref argNotPirata);
                }

                Protocol.WriteConsoleMsg(UserIndex, "¡Has pescado un lindo pez!", Protocol.FontTypeNames.FONTTYPE_INFO);

                UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Pesca, true);
            }
            else
            {
                // [CDT 17-02-2004]
                if (!(Declaraciones.UserList[UserIndex].flags.UltimoMensaje == 6))
                {
                    Protocol.WriteConsoleMsg(UserIndex, "¡No has pescado nada!", Protocol.FontTypeNames.FONTTYPE_INFO);
                    Declaraciones.UserList[UserIndex].flags.UltimoMensaje = 6;
                }
                // [/CDT]

                UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Pesca, false);
            }

            Declaraciones.UserList[UserIndex].Reputacion.PlebeRep =
                Declaraciones.UserList[UserIndex].Reputacion.PlebeRep + Declaraciones.vlProleta;
            if (Declaraciones.UserList[UserIndex].Reputacion.PlebeRep > Declaraciones.MAXREP)
                Declaraciones.UserList[UserIndex].Reputacion.PlebeRep = Declaraciones.MAXREP;

            Declaraciones.UserList[UserIndex].Counters.Trabajando =
                Declaraciones.UserList[UserIndex].Counters.Trabajando + 1;
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in PuedeDomarMascota: " + ex.Message);
            var argdesc = "Error en DoPescar. Error " + ex.GetType().Name + " : " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    public static void DoPescarRed(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************
        try
        {
            short iSkill;
            short Suerte;
            short res;
            bool EsPescador;

            if (Declaraciones.UserList[UserIndex].clase == Declaraciones.eClass.Worker)
            {
                QuitarSta(UserIndex, Declaraciones.EsfuerzoPescarPescador);
                EsPescador = true;
            }
            else
            {
                QuitarSta(UserIndex, Declaraciones.EsfuerzoPescarGeneral);
                EsPescador = false;
            }

            iSkill = Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Pesca];

            // m = (60-11)/(1-10)
            // y = mx - m*10 + 11

            Suerte = Convert.ToInt16(Conversion.Int(-0.00125d * iSkill * iSkill - 0.3d * iSkill + 49d));

            Declaraciones.Obj MiObj;
            // UPGRADE_WARNING: El límite inferior de la matriz PecesPosibles ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            var PecesPosibles = new short[5];
            if (Suerte > 0)
            {
                res = Convert.ToInt16(Matematicas.RandomNumber(1, Suerte));

                if (res < 6)
                {
                    PecesPosibles[1] = Convert.ToInt16((int)Declaraciones.PECES_POSIBLES.PESCADO1);
                    PecesPosibles[2] = Convert.ToInt16((int)Declaraciones.PECES_POSIBLES.PESCADO2);
                    PecesPosibles[3] = Convert.ToInt16((int)Declaraciones.PECES_POSIBLES.PESCADO3);
                    PecesPosibles[4] = Convert.ToInt16((int)Declaraciones.PECES_POSIBLES.PESCADO4);

                    if (EsPescador)
                        MiObj.Amount = Convert.ToInt16(Matematicas.RandomNumber(1, 5));
                    else
                        MiObj.Amount = 1;
                    MiObj.ObjIndex = PecesPosibles[Matematicas.RandomNumber(0, PecesPosibles.Length - 1)];

                    if (!InvUsuario.MeterItemEnInventario(UserIndex, ref MiObj))
                    {
                        var argNotPirata = true;
                        InvNpc.TirarItemAlPiso(ref Declaraciones.UserList[UserIndex].Pos, ref MiObj, ref argNotPirata);
                    }

                    Protocol.WriteConsoleMsg(UserIndex, "¡Has pescado algunos peces!",
                        Protocol.FontTypeNames.FONTTYPE_INFO);

                    UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Pesca, true);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "¡No has pescado nada!", Protocol.FontTypeNames.FONTTYPE_INFO);
                    UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Pesca, false);
                }
            }

            Declaraciones.UserList[UserIndex].Reputacion.PlebeRep =
                Declaraciones.UserList[UserIndex].Reputacion.PlebeRep + Declaraciones.vlProleta;
            if (Declaraciones.UserList[UserIndex].Reputacion.PlebeRep > Declaraciones.MAXREP)
                Declaraciones.UserList[UserIndex].Reputacion.PlebeRep = Declaraciones.MAXREP;
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in DoPescarRed: " + ex.Message);
            var argdesc = "Error en DoPescarRed";
            General.LogError(ref argdesc);
        }
    }

    // '
    // Try to steal an item / gold to another character
    // 
    // @param LadrOnIndex Specifies reference to user that stoles
    // @param VictimaIndex Specifies reference to user that is being stolen

    public static void DoRobar(short LadrOnIndex, short VictimaIndex)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 05/04/2010
        // Last Modification By: ZaMa
        // 24/07/08: Marco - Now it calls to WriteUpdateGold(VictimaIndex and LadrOnIndex) when the thief stoles gold. (MarKoxX)
        // 27/11/2009: ZaMa - Optimizacion de codigo.
        // 18/12/2009: ZaMa - Los ladrones ciudas pueden robar a pks.
        // 01/04/2010: ZaMa - Los ladrones pasan a robar oro acorde a su nivel.
        // 05/04/2010: ZaMa - Los armadas no pueden robarle a ciudadanos jamas.
        // 23/04/2010: ZaMa - No se puede robar mas sin energia.
        // 23/04/2010: ZaMa - El alcance de robo pasa a ser de 1 tile.
        // *************************************************

        try
        {
            if (!Declaraciones.mapInfo[Declaraciones.UserList[VictimaIndex].Pos.Map].Pk)
                return;

            if (Declaraciones.UserList[LadrOnIndex].flags.Seguro)
            {
                if (!ES.criminal(VictimaIndex))
                {
                    Protocol.WriteConsoleMsg(LadrOnIndex, "Debes quitarte el seguro para robarle a un ciudadano.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    return;
                }
            }
            else if (Declaraciones.UserList[LadrOnIndex].Faccion.ArmadaReal == 1)
            {
                Protocol.WriteConsoleMsg(LadrOnIndex,
                    "Los miembros del ejército real no tienen permitido robarle a ciudadanos.",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
                return;
            }

            if (SistemaCombate.TriggerZonaPelea(LadrOnIndex, VictimaIndex) != Declaraciones.eTrigger6.TRIGGER6_AUSENTE)
                return;


            var GuantesHurto = default(bool);
            var Suerte = default(short);
            short res;
            byte RobarSkill;
            short N;
            {
                ref var withBlock = ref Declaraciones.UserList[LadrOnIndex];

                // Caos robando a caos?
                if ((Declaraciones.UserList[VictimaIndex].Faccion.FuerzasCaos == 1) &
                    (withBlock.Faccion.FuerzasCaos == 1))
                {
                    Protocol.WriteConsoleMsg(LadrOnIndex, "No puedes robar a otros miembros de la legión oscura.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    return;
                }

                // Tiene energia?
                if (withBlock.Stats.MinSta < 15)
                {
                    if (withBlock.Genero == Declaraciones.eGenero.Hombre)
                        Protocol.WriteConsoleMsg(LadrOnIndex, "Estás muy cansado para robar.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                    else
                        Protocol.WriteConsoleMsg(LadrOnIndex, "Estás muy cansada para robar.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);

                    return;
                }

                // Quito energia
                QuitarSta(LadrOnIndex, 15);


                if (withBlock.Invent.AnilloEqpObjIndex == Declaraciones.GUANTE_HURTO)
                    GuantesHurto = true;

                if ((Declaraciones.UserList[VictimaIndex].flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                {
                    RobarSkill = withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Robar];

                    if (RobarSkill <= 10)
                        Suerte = 35;
                    else if ((RobarSkill <= 20) & (RobarSkill >= 11))
                        Suerte = 30;
                    else if ((RobarSkill <= 30) & (RobarSkill >= 21))
                        Suerte = 28;
                    else if ((RobarSkill <= 40) & (RobarSkill >= 31))
                        Suerte = 24;
                    else if ((RobarSkill <= 50) & (RobarSkill >= 41))
                        Suerte = 22;
                    else if ((RobarSkill <= 60) & (RobarSkill >= 51))
                        Suerte = 20;
                    else if ((RobarSkill <= 70) & (RobarSkill >= 61))
                        Suerte = 18;
                    else if ((RobarSkill <= 80) & (RobarSkill >= 71))
                        Suerte = 15;
                    else if ((RobarSkill <= 90) & (RobarSkill >= 81))
                        Suerte = 10;
                    else if ((RobarSkill < 100) & (RobarSkill >= 91))
                        Suerte = 7;
                    else if (RobarSkill == 100) Suerte = 5;

                    res = Convert.ToInt16(Matematicas.RandomNumber(1, Suerte));

                    if (res < 3) // Exito robo
                    {
                        if ((Matematicas.RandomNumber(1, 50) < 25) & (withBlock.clase == Declaraciones.eClass.Thief))
                        {
                            if (InvUsuario.TieneObjetosRobables(VictimaIndex))
                                RobarObjeto(LadrOnIndex, VictimaIndex);
                            else
                                Protocol.WriteConsoleMsg(LadrOnIndex,
                                    Declaraciones.UserList[VictimaIndex].name + " no tiene objetos.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                        }
                        else if (Declaraciones.UserList[VictimaIndex].Stats.GLD > 0) // Roba oro
                        {
                            if (withBlock.clase == Declaraciones.eClass.Thief)
                            {
                                // Si no tine puestos los guantes de hurto roba un 50% menos. Pablo (ToxicWaste)
                                if (GuantesHurto)
                                    N = Convert.ToInt16(Matematicas.RandomNumber(withBlock.Stats.ELV * 50,
                                        withBlock.Stats.ELV * 100));
                                else
                                    N = Convert.ToInt16(Matematicas.RandomNumber(withBlock.Stats.ELV * 25,
                                        withBlock.Stats.ELV * 50));
                            }
                            else
                            {
                                N = Convert.ToInt16(Matematicas.RandomNumber(1, 100));
                            }

                            if (N > Declaraciones.UserList[VictimaIndex].Stats.GLD)
                                N = Convert.ToInt16(Declaraciones.UserList[VictimaIndex].Stats.GLD);
                            Declaraciones.UserList[VictimaIndex].Stats.GLD =
                                Declaraciones.UserList[VictimaIndex].Stats.GLD - N;

                            withBlock.Stats.GLD = withBlock.Stats.GLD + N;
                            if (withBlock.Stats.GLD > Declaraciones.MAXORO)
                                withBlock.Stats.GLD = Declaraciones.MAXORO;

                            Protocol.WriteConsoleMsg(LadrOnIndex,
                                "Le has robado " + N + " monedas de oro a " + Declaraciones.UserList[VictimaIndex].name,
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            Protocol.WriteUpdateGold(LadrOnIndex); // Le actualizamos la billetera al ladron

                            Protocol.WriteUpdateGold(VictimaIndex); // Le actualizamos la billetera a la victima
                            Protocol.FlushBuffer(VictimaIndex);
                        }
                        else
                        {
                            Protocol.WriteConsoleMsg(LadrOnIndex,
                                Declaraciones.UserList[VictimaIndex].name + " no tiene oro.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                        }

                        UsUaRiOs.SubirSkill(LadrOnIndex, (short)Declaraciones.eSkill.Robar, true);
                    }
                    else
                    {
                        Protocol.WriteConsoleMsg(LadrOnIndex, "¡No has logrado robar nada!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        Protocol.WriteConsoleMsg(VictimaIndex, "¡" + withBlock.name + " ha intentado robarte!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        Protocol.WriteConsoleMsg(VictimaIndex, "¡" + withBlock.name + " es un criminal!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        Protocol.FlushBuffer(VictimaIndex);

                        UsUaRiOs.SubirSkill(LadrOnIndex, (short)Declaraciones.eSkill.Robar, false);
                    }

                    if (!ES.criminal(LadrOnIndex))
                        if (!ES.criminal(VictimaIndex))
                            UsUaRiOs.VolverCriminal(LadrOnIndex);

                    // Se pudo haber convertido si robo a un ciuda
                    if (ES.criminal(LadrOnIndex))
                    {
                        withBlock.Reputacion.LadronesRep = withBlock.Reputacion.LadronesRep + Declaraciones.vlLadron;
                        if (withBlock.Reputacion.LadronesRep > Declaraciones.MAXREP)
                            withBlock.Reputacion.LadronesRep = Declaraciones.MAXREP;
                    }
                }
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in DoRobar: " + ex.Message);
            var argdesc = "Error en DoRobar. Error " + ex.GetType().Name + " : " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    // '
    // Check if one item is stealable
    // 
    // @param VictimaIndex Specifies reference to victim
    // @param Slot Specifies reference to victim's inventory slot
    // @return If the item is stealable
    public static bool ObjEsRobable(short VictimaIndex, short Slot)
    {
        bool ObjEsRobableRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // Agregué los barcos
        // Esta funcion determina qué objetos son robables.
        // ***************************************************

        short OI;

        OI = Declaraciones.UserList[VictimaIndex].Invent.userObj[Slot].ObjIndex;

        ObjEsRobableRet = (Declaraciones.objData[OI].OBJType != Declaraciones.eOBJType.otLlaves) &
                          (Declaraciones.UserList[VictimaIndex].Invent.userObj[Slot].Equipped == 0) &
                          (Declaraciones.objData[OI].Real == 0) &
                          (Declaraciones.objData[OI].Caos == 0) & (Declaraciones.objData[OI].OBJType !=
                                                                           Declaraciones.eOBJType.otBarcos);
        return ObjEsRobableRet;
    }

    // '
    // Try to steal an item to another character
    // 
    // @param LadrOnIndex Specifies reference to user that stoles
    // @param VictimaIndex Specifies reference to user that is being stolen
    public static void RobarObjeto(short LadrOnIndex, short VictimaIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 02/04/2010
        // 02/04/2010: ZaMa - Modifico la cantidad de items robables por el ladron.
        // ***************************************************

        bool flag;
        short i;
        flag = false;

        if (Matematicas.RandomNumber(1, 12) < 6) // Comenzamos por el principio o el final?
        {
            i = 1;
            while (!flag & (i <= Declaraciones.UserList[VictimaIndex].CurrentInventorySlots))
            {
                // Hay objeto en este slot?
                if (Declaraciones.UserList[VictimaIndex].Invent.userObj[i].ObjIndex > 0)
                    if (ObjEsRobable(VictimaIndex, i))
                        if (Matematicas.RandomNumber(1, 10) < 4)
                            flag = true;

                if (!flag)
                    i = Convert.ToInt16(i + 1);
            }
        }
        else
        {
            i = 20;
            while (!flag & (i > 0))
            {
                // Hay objeto en este slot?
                if (Declaraciones.UserList[VictimaIndex].Invent.userObj[i].ObjIndex > 0)
                    if (ObjEsRobable(VictimaIndex, i))
                        if (Matematicas.RandomNumber(1, 10) < 4)
                            flag = true;

                if (!flag)
                    i = Convert.ToInt16(i - 1);
            }
        }

        Declaraciones.Obj MiObj;
        byte num;
        short ObjAmount;
        if (flag)
        {
            ObjAmount = Declaraciones.UserList[VictimaIndex].Invent.userObj[i].Amount;

            // Cantidad al azar entre el 5% y el 10% del total, con minimo 1.
            num = Convert.ToByte(SistemaCombate.MaximoInt(1,
                Matematicas.RandomNumber(Convert.ToInt32(ObjAmount * 0.05d), Convert.ToInt32(ObjAmount * 0.1d))));

            MiObj.Amount = num;
            MiObj.ObjIndex = Declaraciones.UserList[VictimaIndex].Invent.userObj[i].ObjIndex;

            Declaraciones.UserList[VictimaIndex].Invent.userObj[i].Amount = (short)(ObjAmount - num);

            if (Declaraciones.UserList[VictimaIndex].Invent.userObj[i].Amount <= 0)
                InvUsuario.QuitarUserInvItem(VictimaIndex, Convert.ToByte(i), 1);

            InvUsuario.UpdateUserInv(false, VictimaIndex, Convert.ToByte(i));

            if (!InvUsuario.MeterItemEnInventario(LadrOnIndex, ref MiObj))
            {
                var argNotPirata = true;
                InvNpc.TirarItemAlPiso(ref Declaraciones.UserList[LadrOnIndex].Pos, ref MiObj, ref argNotPirata);
            }

            if (Declaraciones.UserList[LadrOnIndex].clase == Declaraciones.eClass.Thief)
                Protocol.WriteConsoleMsg(LadrOnIndex,
                    "Has robado " + MiObj.Amount + " " + Declaraciones.objData[MiObj.ObjIndex].name,
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            else
                Protocol.WriteConsoleMsg(LadrOnIndex,
                    "Has hurtado " + MiObj.Amount + " " + Declaraciones.objData[MiObj.ObjIndex].name,
                    Protocol.FontTypeNames.FONTTYPE_INFO);
        }
        else
        {
            Protocol.WriteConsoleMsg(LadrOnIndex, "No has logrado robar ningún objeto.",
                Protocol.FontTypeNames.FONTTYPE_INFO);
        }

        // If exiting, cancel de quien es robado
        UsUaRiOs.CancelExit(VictimaIndex);
    }

    public static void DoApuñalar(short UserIndex, short VictimNpcIndex, short VictimUserIndex, short daño)
    {
        // ***************************************************
        // Autor: Nacho (Integer) & Unknown (orginal version)
        // Last Modification: 04/17/08 - (NicoNZ)
        // Simplifique la cuenta que hacia para sacar la suerte
        // y arregle la cuenta que hacia para sacar el daño
        // ***************************************************
        short Suerte;
        short Skill;

        Skill = Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Apuñalar];

        switch (Declaraciones.UserList[UserIndex].clase)
        {
            case Declaraciones.eClass.Assasin:
            {
                Suerte = Convert.ToInt16(Conversion.Int(((0.00003d * Skill - 0.002d) * Skill + 0.098d) * Skill +
                                                        4.25d));
                break;
            }

            case Declaraciones.eClass.Cleric:
            case Declaraciones.eClass.Paladin:
            case Declaraciones.eClass.Pirat:
            {
                Suerte = Convert.ToInt16(Conversion.Int(((0.000003d * Skill + 0.0006d) * Skill + 0.0107d) * Skill +
                                                        4.93d));
                break;
            }

            case Declaraciones.eClass.Bard:
            {
                Suerte = Convert.ToInt16(
                    Conversion.Int(((0.000002d * Skill + 0.0002d) * Skill + 0.032d) * Skill + 4.81d));
                break;
            }

            default:
            {
                Suerte = Convert.ToInt16(Conversion.Int(0.0361d * Skill + 4.39d));
                break;
            }
        }


        if (Matematicas.RandomNumber(0, 100) < Suerte)
        {
            if (VictimUserIndex != 0)
            {
                if (Declaraciones.UserList[UserIndex].clase == Declaraciones.eClass.Assasin)
                    daño = Convert.ToInt16(Math.Round(daño * 1.4d, 0));
                else
                    daño = Convert.ToInt16(Math.Round(daño * 1.5d, 0));

                Declaraciones.UserList[VictimUserIndex].Stats.MinHp =
                    (short)(Declaraciones.UserList[VictimUserIndex].Stats.MinHp - daño);
                Protocol.WriteConsoleMsg(UserIndex,
                    "Has apuñalado a " + Declaraciones.UserList[VictimUserIndex].name + " por " + daño,
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
                Protocol.WriteConsoleMsg(VictimUserIndex,
                    "Te ha apuñalado " + Declaraciones.UserList[UserIndex].name + " por " + daño,
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);

                Protocol.FlushBuffer(VictimUserIndex);
            }
            else
            {
                Declaraciones.Npclist[VictimNpcIndex].Stats.MinHp =
                    Declaraciones.Npclist[VictimNpcIndex].Stats.MinHp - Conversion.Int(daño * 2);
                Protocol.WriteConsoleMsg(UserIndex, "Has apuñalado la criatura por " + Conversion.Int(daño * 2),
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
                // [Alejo]
                SistemaCombate.CalcularDarExp(UserIndex, VictimNpcIndex, daño * 2);
            }

            UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Apuñalar, true);
        }
        else
        {
            Protocol.WriteConsoleMsg(UserIndex, "¡No has logrado apuñalar a tu enemigo!",
                Protocol.FontTypeNames.FONTTYPE_FIGHT);
            UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Apuñalar, false);
        }
    }

    public static void DoAcuchillar(short UserIndex, short VictimNpcIndex, short VictimUserIndex, short daño)
    {
        // ***************************************************
        // Autor: ZaMa
        // Last Modification: 12/01/2010
        // ***************************************************

        if (Declaraciones.UserList[UserIndex].clase != Declaraciones.eClass.Pirat)
            return;
        if (Declaraciones.UserList[UserIndex].Invent.WeaponEqpSlot == 0)
            return;

        if (Matematicas.RandomNumber(0, 100) < Declaraciones.PROB_ACUCHILLAR)
        {
            daño = Convert.ToInt16(Conversion.Int(daño * Declaraciones.DAÑO_ACUCHILLAR));

            if (VictimUserIndex != 0)
            {
                Declaraciones.UserList[VictimUserIndex].Stats.MinHp =
                    (short)(Declaraciones.UserList[VictimUserIndex].Stats.MinHp - daño);
                Protocol.WriteConsoleMsg(UserIndex,
                    "Has acuchillado a " + Declaraciones.UserList[VictimUserIndex].name + " por " + daño,
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
                Protocol.WriteConsoleMsg(VictimUserIndex,
                    Declaraciones.UserList[UserIndex].name + " te ha acuchillado por " + daño,
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
            }
            else
            {
                Declaraciones.Npclist[VictimNpcIndex].Stats.MinHp =
                    Declaraciones.Npclist[VictimNpcIndex].Stats.MinHp - daño;
                Protocol.WriteConsoleMsg(UserIndex, "Has acuchillado a la criatura por " + daño,
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
                SistemaCombate.CalcularDarExp(UserIndex, VictimNpcIndex, daño);
            }
        }
    }

    public static void DoGolpeCritico(short UserIndex, short VictimNpcIndex, short VictimUserIndex, short daño)
    {
        // ***************************************************
        // Autor: Pablo (ToxicWaste)
        // Last Modification: 28/01/2007
        // ***************************************************
        short Suerte;
        short Skill;

        if (Declaraciones.UserList[UserIndex].clase != Declaraciones.eClass.Bandit)
            return;
        if (Declaraciones.UserList[UserIndex].Invent.WeaponEqpSlot == 0)
            return;
        if (Declaraciones.objData[Declaraciones.UserList[UserIndex].Invent.WeaponEqpObjIndex].name !=
            "Espada Vikinga")
            return;


        Skill = Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Wrestling];

        Suerte = Convert.ToInt16(
            Conversion.Int((((0.00000003d * Skill + 0.000006d) * Skill + 0.000107d) * Skill + 0.0893d) * 100d));

        if (Matematicas.RandomNumber(0, 100) < Suerte)
        {
            daño = Convert.ToInt16(Conversion.Int(daño * 0.75d));
            if (VictimUserIndex != 0)
            {
                Declaraciones.UserList[VictimUserIndex].Stats.MinHp =
                    (short)(Declaraciones.UserList[VictimUserIndex].Stats.MinHp - daño);
                Protocol.WriteConsoleMsg(UserIndex,
                    "Has golpeado críticamente a " + Declaraciones.UserList[VictimUserIndex].name + " por " + daño +
                    ".", Protocol.FontTypeNames.FONTTYPE_FIGHT);
                Protocol.WriteConsoleMsg(VictimUserIndex,
                    Declaraciones.UserList[UserIndex].name + " te ha golpeado críticamente por " + daño + ".",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
            }
            else
            {
                Declaraciones.Npclist[VictimNpcIndex].Stats.MinHp =
                    Declaraciones.Npclist[VictimNpcIndex].Stats.MinHp - daño;
                Protocol.WriteConsoleMsg(UserIndex, "Has golpeado críticamente a la criatura por " + daño + ".",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
                // [Alejo]
                SistemaCombate.CalcularDarExp(UserIndex, VictimNpcIndex, daño);
            }
        }
    }

    public static void QuitarSta(short UserIndex, short Cantidad)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            Declaraciones.UserList[UserIndex].Stats.MinSta =
                (short)(Declaraciones.UserList[UserIndex].Stats.MinSta - Cantidad);
            if (Declaraciones.UserList[UserIndex].Stats.MinSta < 0)
                Declaraciones.UserList[UserIndex].Stats.MinSta = 0;
            Protocol.WriteUpdateSta(UserIndex);
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in ObjEsRobable: " + ex.Message);
            var argdesc = "Error en QuitarSta. Error " + ex.GetType().Name + " : " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    public static void DoTalar(short UserIndex, bool DarMaderaElfica = false)
    {
        // ***************************************************
        // Autor: Unknown
        // Last Modification: 16/11/2009
        // 16/11/2009: ZaMa - Ahora Se puede dar madera elfica.
        // 16/11/2009: ZaMa - Implementado nuevo sistema de extraccion.
        // ***************************************************
        try
        {
            short Suerte;
            short res;
            short CantidadItems;

            if (Declaraciones.UserList[UserIndex].clase == Declaraciones.eClass.Worker)
                QuitarSta(UserIndex, Declaraciones.EsfuerzoTalarLeñador);
            else
                QuitarSta(UserIndex, Declaraciones.EsfuerzoTalarGeneral);

            short Skill;
            Skill = Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Talar];
            Suerte = Convert.ToInt16(Conversion.Int(-0.00125d * Skill * Skill - 0.3d * Skill + 49d));

            res = Convert.ToInt16(Matematicas.RandomNumber(1, Suerte));

            Declaraciones.Obj MiObj;
            if (res <= 6)
            {
                if (Declaraciones.UserList[UserIndex].clase == Declaraciones.eClass.Worker)
                {
                    {
                        ref var withBlock = ref Declaraciones.UserList[UserIndex];
                        CantidadItems =
                            Convert.ToInt16(1 +
                                            SistemaCombate.MaximoInt(1,
                                                Convert.ToInt32((withBlock.Stats.ELV - 4) / 5d)));
                    }

                    MiObj.Amount = Convert.ToInt16(Matematicas.RandomNumber(1, CantidadItems));
                }
                else
                {
                    MiObj.Amount = 1;
                }

                MiObj.ObjIndex = DarMaderaElfica ? Declaraciones.LeñaElfica : Declaraciones.Leña;


                if (!InvUsuario.MeterItemEnInventario(UserIndex, ref MiObj))
                {
                    var argNotPirata = true;
                    InvNpc.TirarItemAlPiso(ref Declaraciones.UserList[UserIndex].Pos, ref MiObj, ref argNotPirata);
                }

                Protocol.WriteConsoleMsg(UserIndex, "¡Has conseguido algo de leña!",
                    Protocol.FontTypeNames.FONTTYPE_INFO);

                UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Talar, true);
            }
            else
            {
                // [CDT 17-02-2004]
                if (!(Declaraciones.UserList[UserIndex].flags.UltimoMensaje == 8))
                {
                    Protocol.WriteConsoleMsg(UserIndex, "¡No has obtenido leña!", Protocol.FontTypeNames.FONTTYPE_INFO);
                    Declaraciones.UserList[UserIndex].flags.UltimoMensaje = 8;
                }

                // [/CDT]
                UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Talar, false);
            }

            Declaraciones.UserList[UserIndex].Reputacion.PlebeRep =
                Declaraciones.UserList[UserIndex].Reputacion.PlebeRep + Declaraciones.vlProleta;
            if (Declaraciones.UserList[UserIndex].Reputacion.PlebeRep > Declaraciones.MAXREP)
                Declaraciones.UserList[UserIndex].Reputacion.PlebeRep = Declaraciones.MAXREP;

            Declaraciones.UserList[UserIndex].Counters.Trabajando =
                Declaraciones.UserList[UserIndex].Counters.Trabajando + 1;
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in DoTalar: " + ex.Message);
            var argdesc = "Error en DoTalar";
            General.LogError(ref argdesc);
        }
    }

    public static void DoMineria(short UserIndex)
    {
        // ***************************************************
        // Autor: Unknown
        // Last Modification: 16/11/2009
        // 16/11/2009: ZaMa - Implementado nuevo sistema de extraccion.
        // ***************************************************
        try
        {
            short Suerte;
            short res;
            short CantidadItems;

            short Skill;
            Declaraciones.Obj MiObj;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                if (withBlock.clase == Declaraciones.eClass.Worker)
                    QuitarSta(UserIndex, Declaraciones.EsfuerzoExcavarMinero);
                else
                    QuitarSta(UserIndex, Declaraciones.EsfuerzoExcavarGeneral);

                Skill = withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Mineria];
                Suerte = Convert.ToInt16(Conversion.Int(-0.00125d * Skill * Skill - 0.3d * Skill + 49d));

                res = Convert.ToInt16(Matematicas.RandomNumber(1, Suerte));

                if (res <= 5)
                {
                    if (withBlock.flags.TargetObj == 0)
                        return;

                    MiObj.ObjIndex = Declaraciones.objData[withBlock.flags.TargetObj].MineralIndex;

                    if (Declaraciones.UserList[UserIndex].clase == Declaraciones.eClass.Worker)
                    {
                        CantidadItems =
                            Convert.ToInt16(1 +
                                            SistemaCombate.MaximoInt(1,
                                                Convert.ToInt32((withBlock.Stats.ELV - 4) / 5d)));

                        MiObj.Amount = Convert.ToInt16(Matematicas.RandomNumber(1, CantidadItems));
                    }
                    else
                    {
                        MiObj.Amount = 1;
                    }

                    if (!InvUsuario.MeterItemEnInventario(UserIndex, ref MiObj))
                    {
                        var argNotPirata = true;
                        InvNpc.TirarItemAlPiso(ref withBlock.Pos, ref MiObj, ref argNotPirata);
                    }

                    Protocol.WriteConsoleMsg(UserIndex, "¡Has extraido algunos minerales!",
                        Protocol.FontTypeNames.FONTTYPE_INFO);

                    UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Mineria, true);
                }
                else
                {
                    // [CDT 17-02-2004]
                    if (!(withBlock.flags.UltimoMensaje == 9))
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "¡No has conseguido nada!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        withBlock.flags.UltimoMensaje = 9;
                    }

                    // [/CDT]
                    UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Mineria, false);
                }

                withBlock.Reputacion.PlebeRep = withBlock.Reputacion.PlebeRep + Declaraciones.vlProleta;
                if (withBlock.Reputacion.PlebeRep > Declaraciones.MAXREP)
                    withBlock.Reputacion.PlebeRep = Declaraciones.MAXREP;

                withBlock.Counters.Trabajando = Declaraciones.UserList[UserIndex].Counters.Trabajando + 1;
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in DoMineria: " + ex.Message);
            var argdesc = "Error en Sub DoMineria";
            General.LogError(ref argdesc);
        }
    }

    public static void DoMeditar(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        var Suerte = default(short);
        short res;
        short cant;
        byte MeditarSkill;
        int TActual;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            withBlock.Counters.IdleCount = 0;


            // Barrin 3/10/03
            // Esperamos a que se termine de concentrar
            TActual = Convert.ToInt32(Migration.GetTickCount());
            if (TActual - withBlock.Counters.tInicioMeditar < Declaraciones.TIEMPO_INICIOMEDITAR) return;

            if (!withBlock.Counters.bPuedeMeditar) withBlock.Counters.bPuedeMeditar = true;

            if (withBlock.Stats.MinMAN >= withBlock.Stats.MaxMAN)
            {
                Protocol.WriteConsoleMsg(UserIndex, "Has terminado de meditar.", Protocol.FontTypeNames.FONTTYPE_INFO);
                Protocol.WriteMeditateToggle(UserIndex);
                withBlock.flags.Meditando = false;
                withBlock.character.FX = 0;
                withBlock.character.loops = 0;
                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                    Protocol.PrepareMessageCreateFX(withBlock.character.CharIndex, 0, 0));
                return;
            }

            MeditarSkill = withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Meditar];

            if (MeditarSkill <= 10)
                Suerte = 35;
            else if ((MeditarSkill <= 20) & (MeditarSkill >= 11))
                Suerte = 30;
            else if ((MeditarSkill <= 30) & (MeditarSkill >= 21))
                Suerte = 28;
            else if ((MeditarSkill <= 40) & (MeditarSkill >= 31))
                Suerte = 24;
            else if ((MeditarSkill <= 50) & (MeditarSkill >= 41))
                Suerte = 22;
            else if ((MeditarSkill <= 60) & (MeditarSkill >= 51))
                Suerte = 20;
            else if ((MeditarSkill <= 70) & (MeditarSkill >= 61))
                Suerte = 18;
            else if ((MeditarSkill <= 80) & (MeditarSkill >= 71))
                Suerte = 15;
            else if ((MeditarSkill <= 90) & (MeditarSkill >= 81))
                Suerte = 10;
            else if ((MeditarSkill < 100) & (MeditarSkill >= 91))
                Suerte = 7;
            else if (MeditarSkill == 100) Suerte = 5;
            res = Convert.ToInt16(Matematicas.RandomNumber(1, Suerte));

            if (res == 1)
            {
                cant = Convert.ToInt16(Matematicas.Porcentaje(withBlock.Stats.MaxMAN, Admin.PorcentajeRecuperoMana));
                if (cant <= 0)
                    cant = 1;
                withBlock.Stats.MinMAN = (short)(withBlock.Stats.MinMAN + cant);
                if (withBlock.Stats.MinMAN > withBlock.Stats.MaxMAN)
                    withBlock.Stats.MinMAN = withBlock.Stats.MaxMAN;

                if (!(withBlock.flags.UltimoMensaje == 22))
                {
                    Protocol.WriteConsoleMsg(UserIndex, "¡Has recuperado " + cant + " puntos de maná!",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    withBlock.flags.UltimoMensaje = 22;
                }

                Protocol.WriteUpdateMana(UserIndex);
                UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Meditar, true);
            }
            else
            {
                UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Meditar, false);
            }
        }
    }

    public static void DoDesequipar(short UserIndex, short VictimIndex)
    {
        // ***************************************************
        // Author: ZaMa
        // Last Modif: 15/04/2010
        // Unequips either shield, weapon or helmet from target user.
        // ***************************************************

        short Probabilidad;
        short Resultado;
        byte WrestlingSkill;
        var AlgoEquipado = default(bool);

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Si no tiene guantes de hurto no desequipa.
            if (withBlock.Invent.AnilloEqpObjIndex != Declaraciones.GUANTE_HURTO)
                return;

            // Si no esta solo con manos, no desequipa tampoco.
            if (withBlock.Invent.WeaponEqpObjIndex > 0)
                return;

            WrestlingSkill = withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Wrestling];

            Probabilidad = Convert.ToInt16(WrestlingSkill * 0.2d + withBlock.Stats.ELV * 0.66d);
        }

        {
            ref var withBlock1 = ref Declaraciones.UserList[VictimIndex];
            // Si tiene escudo, intenta desequiparlo
            if (withBlock1.Invent.EscudoEqpObjIndex > 0)
            {
                Resultado = Convert.ToInt16(Matematicas.RandomNumber(1, 100));

                if (Resultado <= Probabilidad)
                {
                    // Se lo desequipo
                    InvUsuario.Desequipar(VictimIndex, withBlock1.Invent.EscudoEqpSlot);

                    Protocol.WriteConsoleMsg(UserIndex, "Has logrado desequipar el escudo de tu oponente!",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);

                    if (withBlock1.Stats.ELV < 20)
                        Protocol.WriteConsoleMsg(VictimIndex, "¡Tu oponente te ha desequipado el escudo!",
                            Protocol.FontTypeNames.FONTTYPE_FIGHT);

                    Protocol.FlushBuffer(VictimIndex);

                    return;
                }

                AlgoEquipado = true;
            }

            // No tiene escudo, o fallo desequiparlo, entonces trata de desequipar arma
            if (withBlock1.Invent.WeaponEqpObjIndex > 0)
            {
                Resultado = Convert.ToInt16(Matematicas.RandomNumber(1, 100));

                if (Resultado <= Probabilidad)
                {
                    // Se lo desequipo
                    InvUsuario.Desequipar(VictimIndex, withBlock1.Invent.WeaponEqpSlot);

                    Protocol.WriteConsoleMsg(UserIndex, "Has logrado desarmar a tu oponente!",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);

                    if (withBlock1.Stats.ELV < 20)
                        Protocol.WriteConsoleMsg(VictimIndex, "¡Tu oponente te ha desarmado!",
                            Protocol.FontTypeNames.FONTTYPE_FIGHT);

                    Protocol.FlushBuffer(VictimIndex);

                    return;
                }

                AlgoEquipado = true;
            }

            // No tiene arma, o fallo desequiparla, entonces trata de desequipar casco
            if (withBlock1.Invent.CascoEqpObjIndex > 0)
            {
                Resultado = Convert.ToInt16(Matematicas.RandomNumber(1, 100));

                if (Resultado <= Probabilidad)
                {
                    // Se lo desequipo
                    InvUsuario.Desequipar(VictimIndex, withBlock1.Invent.CascoEqpSlot);

                    Protocol.WriteConsoleMsg(UserIndex, "Has logrado desequipar el casco de tu oponente!",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);

                    if (withBlock1.Stats.ELV < 20)
                        Protocol.WriteConsoleMsg(VictimIndex, "¡Tu oponente te ha desequipado el casco!",
                            Protocol.FontTypeNames.FONTTYPE_FIGHT);

                    Protocol.FlushBuffer(VictimIndex);

                    return;
                }

                AlgoEquipado = true;
            }

            if (AlgoEquipado)
                Protocol.WriteConsoleMsg(UserIndex, "Tu oponente no tiene equipado items!",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
            else
                Protocol.WriteConsoleMsg(UserIndex, "No has logrado desequipar ningún item a tu oponente!",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
        }
    }

    public static void DoHurtar(short UserIndex, short VictimaIndex)
    {
        // ***************************************************
        // Author: Pablo (ToxicWaste)
        // Last Modif: 03/03/2010
        // Implements the pick pocket skill of the Bandit :)
        // 03/03/2010 - Pato: Sólo se puede hurtar si no está en trigger 6 :)
        // ***************************************************
        if (SistemaCombate.TriggerZonaPelea(UserIndex, VictimaIndex) != Declaraciones.eTrigger6.TRIGGER6_AUSENTE)
            return;

        if (Declaraciones.UserList[UserIndex].clase != Declaraciones.eClass.Bandit)
            return;
        // Esto es precario y feo, pero por ahora no se me ocurrió nada mejor.
        // Uso el slot de los anillos para "equipar" los guantes.
        // Y los reconozco porque les puse DefensaMagicaMin y Max = 0
        if (Declaraciones.UserList[UserIndex].Invent.AnilloEqpObjIndex != Declaraciones.GUANTE_HURTO)
            return;

        short res;
        res = Convert.ToInt16(Matematicas.RandomNumber(1, 100));
        if (res < 20)
        {
            if (InvUsuario.TieneObjetosRobables(VictimaIndex))
            {
                RobarObjeto(UserIndex, VictimaIndex);
                Protocol.WriteConsoleMsg(VictimaIndex, "¡" + Declaraciones.UserList[UserIndex].name + " es un Bandido!",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }
            else
            {
                Protocol.WriteConsoleMsg(UserIndex, Declaraciones.UserList[VictimaIndex].name + " no tiene objetos.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    public static void DoHandInmo(short UserIndex, short VictimaIndex)
    {
        // ***************************************************
        // Author: Pablo (ToxicWaste)
        // Last Modif: 17/02/2007
        // Implements the special Skill of the Thief
        // ***************************************************
        if (Declaraciones.UserList[VictimaIndex].flags.Paralizado == 1)
            return;
        if (Declaraciones.UserList[UserIndex].clase != Declaraciones.eClass.Thief)
            return;


        if (Declaraciones.UserList[UserIndex].Invent.AnilloEqpObjIndex != Declaraciones.GUANTE_HURTO)
            return;

        short res;
        res = Convert.ToInt16(Matematicas.RandomNumber(0, 100));
        if (res < Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Wrestling] / 4d)
        {
            Declaraciones.UserList[VictimaIndex].flags.Paralizado = 1;
            Declaraciones.UserList[VictimaIndex].Counters.Paralisis = Convert.ToInt16(Admin.IntervaloParalizado / 2d);
            Protocol.WriteParalizeOK(VictimaIndex);
            Protocol.WriteConsoleMsg(UserIndex, "Tu golpe ha dejado inmóvil a tu oponente",
                Protocol.FontTypeNames.FONTTYPE_INFO);
            Protocol.WriteConsoleMsg(VictimaIndex, "¡El golpe te ha dejado inmóvil!",
                Protocol.FontTypeNames.FONTTYPE_INFO);
        }
    }

    public static void Desarmar(short UserIndex, short VictimIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 02/04/2010 (ZaMa)
        // 02/04/2010: ZaMa - Nueva formula para desarmar.
        // ***************************************************

        short Probabilidad;
        short Resultado;
        byte WrestlingSkill;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            WrestlingSkill = withBlock.Stats.UserSkills[(int)Declaraciones.eSkill.Wrestling];

            Probabilidad = Convert.ToInt16(WrestlingSkill * 0.2d + withBlock.Stats.ELV * 0.66d);

            Resultado = Convert.ToInt16(Matematicas.RandomNumber(1, 100));

            if (Resultado <= Probabilidad)
            {
                InvUsuario.Desequipar(VictimIndex, Declaraciones.UserList[VictimIndex].Invent.WeaponEqpSlot);
                Protocol.WriteConsoleMsg(UserIndex, "Has logrado desarmar a tu oponente!",
                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
                if (Declaraciones.UserList[VictimIndex].Stats.ELV < 20)
                    Protocol.WriteConsoleMsg(VictimIndex, "¡Tu oponente te ha desarmado!",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT);
                Protocol.FlushBuffer(VictimIndex);
            }
        }
    }


    public static short MaxItemsConstruibles(short UserIndex)
    {
        short MaxItemsConstruiblesRet = default;
        // ***************************************************
        // Author: ZaMa
        // Last Modification: 29/01/2010
        // 
        // ***************************************************
        MaxItemsConstruiblesRet = Convert.ToInt16(SistemaCombate.MaximoInt(1,
            Convert.ToInt32((Declaraciones.UserList[UserIndex].Stats.ELV - 4) / 5d)));
        return MaxItemsConstruiblesRet;
    }
}