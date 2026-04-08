using System;
using System.Runtime.InteropServices;

namespace Legacy;

internal static class InvUsuario
{
    public static bool TieneObjetosRobables(short UserIndex)
    {
        bool TieneObjetosRobablesRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // 17/09/02
        // Agregue que la función se asegure que el objeto no es un barco

        try
        {
            short i;
            short ObjIndex;

            var loopTo = (short)Declaraciones.UserList[UserIndex].CurrentInventorySlots;
            for (i = 1; i <= loopTo; i++)
            {
                ObjIndex = Declaraciones.UserList[UserIndex].Invent.userObj[i].ObjIndex;
                if (ObjIndex > 0)
                    if ((Declaraciones.objData[ObjIndex].OBJType != Declaraciones.eOBJType.otLlaves) &
                        (Declaraciones.objData[ObjIndex].OBJType != Declaraciones.eOBJType.otBarcos))
                    {
                        TieneObjetosRobablesRet = true;
                        return TieneObjetosRobablesRet;
                    }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in TieneObjetosRobables: " + ex.Message);
        }

        return TieneObjetosRobablesRet;
    }

    public static bool ClasePuedeUsarItem(short UserIndex, short ObjIndex,
        [Optional] [DefaultParameterValue("")] ref string sMotivo)
    {
        bool ClasePuedeUsarItemRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: 14/01/2010 (ZaMa)
        // 14/01/2010: ZaMa - Agrego el motivo por el que no puede equipar/usar el item.
        // ***************************************************

        try
        {
            // Admins can use ANYTHING!
            short i;
            if ((Declaraciones.UserList[UserIndex].flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                if (Declaraciones.objData[ObjIndex].ClaseProhibida[1] != 0)
                    for (i = 1; i <= Declaraciones.NUMCLASES; i++)
                        if (Declaraciones.objData[ObjIndex].ClaseProhibida[i] ==
                            Declaraciones.UserList[UserIndex].clase)
                        {
                            ClasePuedeUsarItemRet = false;
                            sMotivo = "Tu clase no puede usar este objeto.";
                            return ClasePuedeUsarItemRet;
                        }

            ClasePuedeUsarItemRet = true;
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in TieneObjetosRobables: " + ex.Message);
            var argdesc = "Error en ClasePuedeUsarItem";
            General.LogError(ref argdesc);
        }

        return ClasePuedeUsarItemRet;
    }

    public static void QuitarNewbieObj(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short j;

        Declaraciones.WorldPos DeDonde;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            var loopTo = (short)Declaraciones.UserList[UserIndex].CurrentInventorySlots;
            for (j = 1; j <= loopTo; j++)
                if (withBlock.Invent.userObj[j].ObjIndex > 0)
                {
                    if (Declaraciones.objData[withBlock.Invent.userObj[j].ObjIndex].Newbie == 1)
                        QuitarUserInvItem(UserIndex, Convert.ToByte(j), Declaraciones.MAX_INVENTORY_OBJS);
                    UpdateUserInv(false, UserIndex, Convert.ToByte(j));
                }

            // [Barrin 17-12-03] Si el usuario dejó de ser Newbie, y estaba en el Newbie Dungeon
            // es transportado a su hogar de origen ;)
            if (Declaraciones.mapInfo[withBlock.Pos.Map].Restringir.ToUpper() == "NEWBIE")
            {
                switch (withBlock.Hogar)
                {
                    case Declaraciones.eCiudad.cLindos: // Vamos a tener que ir por todo el desierto... uff!
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto DeDonde. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        DeDonde = Declaraciones.Lindos;
                        break;
                    }
                    case Declaraciones.eCiudad.cUllathorpe:
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto DeDonde. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        DeDonde = Declaraciones.Ullathorpe;
                        break;
                    }
                    case Declaraciones.eCiudad.cBanderbill:
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto DeDonde. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        DeDonde = Declaraciones.Banderbill;
                        break;
                    }

                    default:
                    {
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto DeDonde. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        DeDonde = Declaraciones.Nix;
                        break;
                    }
                }

                UsUaRiOs.WarpUserChar(UserIndex, DeDonde.Map, DeDonde.X, DeDonde.Y, true);
            }
            // [/Barrin]
        }
    }

    public static void LimpiarInventario(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short j;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            var loopTo = (short)withBlock.CurrentInventorySlots;
            for (j = 1; j <= loopTo; j++)
            {
                withBlock.Invent.userObj[j].ObjIndex = 0;
                withBlock.Invent.userObj[j].Amount = 0;
                withBlock.Invent.userObj[j].Equipped = 0;
            }

            withBlock.Invent.NroItems = 0;

            withBlock.Invent.ArmourEqpObjIndex = 0;
            withBlock.Invent.ArmourEqpSlot = 0;

            withBlock.Invent.WeaponEqpObjIndex = 0;
            withBlock.Invent.WeaponEqpSlot = 0;

            withBlock.Invent.CascoEqpObjIndex = 0;
            withBlock.Invent.CascoEqpSlot = 0;

            withBlock.Invent.EscudoEqpObjIndex = 0;
            withBlock.Invent.EscudoEqpSlot = 0;

            withBlock.Invent.AnilloEqpObjIndex = 0;
            withBlock.Invent.AnilloEqpSlot = 0;

            withBlock.Invent.MunicionEqpObjIndex = 0;
            withBlock.Invent.MunicionEqpSlot = 0;

            withBlock.Invent.BarcoObjIndex = 0;
            withBlock.Invent.BarcoSlot = 0;

            withBlock.Invent.MochilaEqpObjIndex = 0;
            withBlock.Invent.MochilaEqpSlot = 0;
        }
    }

    public static void TirarOro(int Cantidad, short UserIndex)
    {
        // ***************************************************
        // Autor: Unknown (orginal version)
        // Last Modification: 23/01/2007
        // 23/01/2007 -> Pablo (ToxicWaste): Billetera invertida y explotar oro en el agua.
        // ***************************************************
        try
        {
            // If Cantidad > 100000 Then Exit Sub

            Declaraciones.Obj MiObj;
            var loops = default(short);
            short j;
            short k;
            short M;
            var Cercanos = default(string);
            int TeniaOro;
            Declaraciones.WorldPos AuxPos;
            var extra = default(int);
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // SI EL Pjta TIENE ORO LO TIRAMOS
                if ((Cantidad > 0) & (Cantidad <= withBlock.Stats.GLD))
                {
                    // info debug

                    // Seguridad Alkon (guardo el oro tirado si supera los 50k)
                    if (Cantidad > 50000)
                    {
                        M = withBlock.Pos.Map;
                        var loopTo = Convert.ToInt16(withBlock.Pos.X + 10);
                        for (j = Convert.ToInt16(withBlock.Pos.X - 10); j <= loopTo; j++)
                        {
                            var loopTo1 = Convert.ToInt16(withBlock.Pos.Y + 10);
                            for (k = Convert.ToInt16(withBlock.Pos.Y - 10); k <= loopTo1; k++)
                                if (Extra.InMapBounds(M, j, k))
                                    if (Declaraciones.MapData[M, j, k].UserIndex > 0)
                                        Cercanos = Cercanos + Declaraciones
                                            .UserList[Declaraciones.MapData[M, j, k].UserIndex].name + ",";
                        }

                        General.LogDesarrollo(withBlock.name + " tira oro. Cercanos: " + Cercanos);
                    }

                    // /Seguridad
                    TeniaOro = withBlock.Stats.GLD;
                    if (Cantidad > 500000) // Para evitar explotar demasiado
                    {
                        extra = Cantidad - 500000;
                        Cantidad = 500000;
                    }

                    while (Cantidad > 0)
                    {
                        if ((Cantidad > Declaraciones.MAX_INVENTORY_OBJS) &
                            (withBlock.Stats.GLD > Declaraciones.MAX_INVENTORY_OBJS))
                        {
                            MiObj.Amount = Declaraciones.MAX_INVENTORY_OBJS;
                            Cantidad = Cantidad - MiObj.Amount;
                        }
                        else
                        {
                            MiObj.Amount = Convert.ToInt16(Cantidad);
                            Cantidad = Cantidad - MiObj.Amount;
                        }

                        MiObj.ObjIndex = Declaraciones.iORO;

                        if (Extra.EsGM(UserIndex))
                        {
                            var argtexto = "Tiró cantidad:" + MiObj.Amount + " Objeto:" +
                                           Declaraciones.objData[MiObj.ObjIndex].name;
                            General.LogGM(ref withBlock.name, ref argtexto);
                        }

                        if ((withBlock.clase == Declaraciones.eClass.Pirat) & (withBlock.Invent.BarcoObjIndex == 476))
                        {
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto AuxPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            var argNotPirata = false;
                            AuxPos = InvNpc.TirarItemAlPiso(ref withBlock.Pos, ref MiObj, ref argNotPirata);
                            if ((AuxPos.X != 0) & (AuxPos.Y != 0))
                                withBlock.Stats.GLD = withBlock.Stats.GLD - MiObj.Amount;
                        }
                        else
                        {
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto AuxPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            var argNotPirata1 = true;
                            AuxPos = InvNpc.TirarItemAlPiso(ref withBlock.Pos, ref MiObj, ref argNotPirata1);
                            if ((AuxPos.X != 0) & (AuxPos.Y != 0))
                                withBlock.Stats.GLD = withBlock.Stats.GLD - MiObj.Amount;
                        }

                        // info debug
                        loops = Convert.ToInt16(loops + 1);
                        if (loops > 100)
                        {
                            var argdesc = "Error en tiraroro";
                            General.LogError(ref argdesc);
                            return;
                        }
                    }

                    if (TeniaOro == withBlock.Stats.GLD)
                        extra = 0;
                    if (extra > 0) withBlock.Stats.GLD = withBlock.Stats.GLD - extra;
                }
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in QuitarNewbieObj: " + ex.Message);
            var argdesc1 = "Error en TirarOro. Error " + ex.GetType().Name + " : " + ex.Message;
            General.LogError(ref argdesc1);
        }
    }

    public static void QuitarUserInvItem(short UserIndex, byte Slot, short Cantidad)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            if ((Slot < 1) | (Slot > Declaraciones.UserList[UserIndex].CurrentInventorySlots))
                return;

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].Invent.userObj[Slot];
                if ((withBlock.Amount <= Cantidad) & (withBlock.Equipped == 1)) Desequipar(UserIndex, Slot);

                // Quita un objeto
                withBlock.Amount = (short)(withBlock.Amount - Cantidad);
                // ¿Quedan mas?
                if (withBlock.Amount <= 0)
                {
                    Declaraciones.UserList[UserIndex].Invent.NroItems =
                        Convert.ToInt16(Declaraciones.UserList[UserIndex].Invent.NroItems - 1);
                    withBlock.ObjIndex = 0;
                    withBlock.Amount = 0;
                }
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in QuitarUserInvItem: " + ex.Message);
            var argdesc = "Error en QuitarUserInvItem. Error " + ex.GetType().Name + " : " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    public static void UpdateUserInv(bool UpdateAll, short UserIndex, byte Slot)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            var NullObj = default(Declaraciones.UserOBJ);
            int LoopC;

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // Actualiza un solo slot
                if (!UpdateAll)
                {
                    // Actualiza el inventario
                    if (withBlock.Invent.userObj[Slot].ObjIndex > 0)
                        UsUaRiOs.ChangeUserInv(UserIndex, Slot, ref withBlock.Invent.userObj[Slot]);
                    else
                        UsUaRiOs.ChangeUserInv(UserIndex, Slot, ref NullObj);
                }

                else
                {
                    // Actualiza todos los slots
                    var loopTo = (int)withBlock.CurrentInventorySlots;
                    for (LoopC = 1; LoopC <= loopTo; LoopC++)
                        // Actualiza el inventario
                        if (withBlock.Invent.userObj[LoopC].ObjIndex > 0)
                            UsUaRiOs.ChangeUserInv(UserIndex, Convert.ToByte(LoopC),
                                ref withBlock.Invent.userObj[LoopC]);
                        else
                            UsUaRiOs.ChangeUserInv(UserIndex, Convert.ToByte(LoopC), ref NullObj);
                }
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in UpdateUserInv: " + ex.Message);
            var argdesc = "Error en UpdateUserInv. Error " + ex.GetType().Name + " : " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    public static void DropObj(short UserIndex, byte Slot, short num, short Map, short X, short Y)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        Declaraciones.Obj obj;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (num > 0)
            {
                if (num > withBlock.Invent.userObj[Slot].Amount)
                    num = withBlock.Invent.userObj[Slot].Amount;

                obj.ObjIndex = withBlock.Invent.userObj[Slot].ObjIndex;
                obj.Amount = num;

                if (ItemNewbie(obj.ObjIndex) &&
                    (withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No puedes tirar objetos newbie.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                // Check objeto en el suelo
                if ((Declaraciones.MapData[withBlock.Pos.Map, X, Y].ObjInfo.ObjIndex == 0) |
                    (Declaraciones.MapData[withBlock.Pos.Map, X, Y].ObjInfo.ObjIndex == obj.ObjIndex))
                {
                    if ((short)(num + Declaraciones.MapData[withBlock.Pos.Map, X, Y].ObjInfo.Amount) >
                        Declaraciones.MAX_INVENTORY_OBJS)
                        num = (short)(Declaraciones.MAX_INVENTORY_OBJS -
                                      Declaraciones.MapData[withBlock.Pos.Map, X, Y].ObjInfo.Amount);

                    MakeObj(ref obj, Map, X, Y);
                    QuitarUserInvItem(UserIndex, Slot, num);
                    UpdateUserInv(false, UserIndex, Slot);

                    if (Declaraciones.objData[obj.ObjIndex].OBJType == Declaraciones.eOBJType.otBarcos)
                        Protocol.WriteConsoleMsg(UserIndex, "¡¡ATENCIÓN!! ¡ACABAS DE TIRAR TU BARCA!",
                            Protocol.FontTypeNames.FONTTYPE_TALK);

                    if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) == 0)
                    {
                        var argtexto = "Tiró cantidad:" + num + " Objeto:" +
                                       Declaraciones.objData[obj.ObjIndex].name;
                        General.LogGM(ref withBlock.name, ref argtexto);
                    }

                    // Log de Objetos que se tiran al piso. Pablo (ToxicWaste) 07/09/07
                    // Es un Objeto que tenemos que loguear?
                    if (Declaraciones.objData[obj.ObjIndex].Log == 1)
                        General.LogDesarrollo(withBlock.name + " tiró al piso " + obj.Amount + " " +
                                              Declaraciones.objData[obj.ObjIndex].name + " Mapa: " +
                                              Map + " X: " + X + " Y: " + Y);
                    else if (obj.Amount > 5000)
                        // Es mucha cantidad? > Subí a 5000 el minimo porque si no se llenaba el log de cosas al pedo. (NicoNZ)
                        // Si no es de los prohibidos de loguear, lo logueamos.
                        if (Declaraciones.objData[obj.ObjIndex].NoLog != 1)
                            General.LogDesarrollo(withBlock.name + " tiró al piso " + obj.Amount + " " +
                                                  Declaraciones.objData[obj.ObjIndex].name + " Mapa: " +
                                                  Map + " X: " + X + " Y: " + Y);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "No hay espacio en el piso.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                }
            }
        }
    }

    public static void EraseObj(short num, short Map, short X, short Y)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.MapData[Map, X, Y];
            withBlock.ObjInfo.Amount = (short)(withBlock.ObjInfo.Amount - num);

            if (withBlock.ObjInfo.Amount <= 0)
            {
                withBlock.ObjInfo.ObjIndex = 0;
                withBlock.ObjInfo.Amount = 0;

                modSendData.SendToAreaByPos(Map, X, Y,
                    Protocol.PrepareMessageObjectDelete(Convert.ToByte(X), Convert.ToByte(Y)));
            }
        }
    }

    public static void MakeObj(ref Declaraciones.Obj Obj, short Map, short X, short Y)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if ((Obj.ObjIndex > 0) & (Obj.ObjIndex <= Declaraciones.objData.Length - 1))
        {
            ref var withBlock = ref Declaraciones.MapData[Map, X, Y];
            if (withBlock.ObjInfo.ObjIndex == Obj.ObjIndex)
            {
                withBlock.ObjInfo.Amount = (short)(withBlock.ObjInfo.Amount + Obj.Amount);
            }
            else
            {
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto MapData().ObjInfo. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                withBlock.ObjInfo = Obj;

                modSendData.SendToAreaByPos(Map, X, Y,
                    Protocol.PrepareMessageObjectCreate(Declaraciones.objData[Obj.ObjIndex].GrhIndex,
                        Convert.ToByte(X), Convert.ToByte(Y)));
            }
        }
    }

    public static bool MeterItemEnInventario(short UserIndex, ref Declaraciones.Obj MiObj)
    {
        bool MeterItemEnInventarioRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            byte Slot;

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // ¿el user ya tiene un objeto del mismo tipo?
                Slot = 1;
                while (!((withBlock.Invent.userObj[Slot].ObjIndex == MiObj.ObjIndex) &
                         ((short)(withBlock.Invent.userObj[Slot].Amount + MiObj.Amount) <=
                          Declaraciones.MAX_INVENTORY_OBJS)))

                {
                    Slot = Convert.ToByte(Slot + 1);
                    if (Slot > withBlock.CurrentInventorySlots) break;
                }

                // Sino busca un slot vacio
                if (Slot > withBlock.CurrentInventorySlots)
                {
                    Slot = 1;
                    while (withBlock.Invent.userObj[Slot].ObjIndex != 0)
                    {
                        Slot = Convert.ToByte(Slot + 1);
                        if (Slot > withBlock.CurrentInventorySlots)
                        {
                            Protocol.WriteConsoleMsg(UserIndex, "No puedes cargar más objetos.",
                                Protocol.FontTypeNames.FONTTYPE_FIGHT);
                            MeterItemEnInventarioRet = false;
                            return MeterItemEnInventarioRet;
                        }
                    }

                    withBlock.Invent.NroItems = Convert.ToInt16(withBlock.Invent.NroItems + 1);
                }

                if ((Slot > Declaraciones.MAX_NORMAL_INVENTORY_SLOTS) & (Slot < Declaraciones.MAX_INVENTORY_SLOTS))
                    if (!ItemSeCae(MiObj.ObjIndex))
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "No puedes contener objetos especiales en tu " +
                            Declaraciones.objData[withBlock.Invent.MochilaEqpObjIndex].name + ".",
                            Protocol.FontTypeNames.FONTTYPE_FIGHT);
                        MeterItemEnInventarioRet = false;
                        return MeterItemEnInventarioRet;
                    }

                // Mete el objeto
                if ((short)(withBlock.Invent.userObj[Slot].Amount + MiObj.Amount) <=
                    Declaraciones.MAX_INVENTORY_OBJS)
                {
                    // Menor que MAX_INV_OBJS
                    withBlock.Invent.userObj[Slot].ObjIndex = MiObj.ObjIndex;
                    withBlock.Invent.userObj[Slot].Amount =
                        (short)(withBlock.Invent.userObj[Slot].Amount + MiObj.Amount);
                }
                else
                {
                    withBlock.Invent.userObj[Slot].Amount = Declaraciones.MAX_INVENTORY_OBJS;
                }
            }

            MeterItemEnInventarioRet = true;

            UpdateUserInv(false, UserIndex, Slot);
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in DropObj: " + ex.Message);
            var argdesc = "Error en MeterItemEnInventario. Error " + ex.GetType().Name + " : " + ex.Message;
            General.LogError(ref argdesc);
        }

        return MeterItemEnInventarioRet;
    }

    public static void GetObj(short UserIndex)
    {
        // ***************************************************
        // Autor: Unknown (orginal version)
        // Last Modification: 18/12/2009
        // 18/12/2009: ZaMa - Oro directo a la billetera.
        // ***************************************************

        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Declaraciones.ObjData obj;
        Declaraciones.Obj MiObj;
        string ObjPos;

        short X;
        short Y;
        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // ¿Hay algun obj?
            if (Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].ObjInfo.ObjIndex > 0)
            {
                // ¿Esta permitido agarrar este obj?
                if (Declaraciones
                        .objData[
                            Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].ObjInfo.ObjIndex]
                        .Agarrable != 1)
                {
                    X = withBlock.Pos.X;
                    Y = withBlock.Pos.Y;

                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto obj. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    obj = Declaraciones.objData[
                        Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].ObjInfo.ObjIndex];
                    MiObj.Amount = Declaraciones.MapData[withBlock.Pos.Map, X, Y].ObjInfo.Amount;
                    MiObj.ObjIndex = Declaraciones.MapData[withBlock.Pos.Map, X, Y].ObjInfo.ObjIndex;

                    // Oro directo a la billetera!
                    if (obj.OBJType == Declaraciones.eOBJType.otGuita)
                    {
                        withBlock.Stats.GLD = withBlock.Stats.GLD + MiObj.Amount;
                        // Quitamos el objeto
                        EraseObj(Declaraciones.MapData[withBlock.Pos.Map, X, Y].ObjInfo.Amount, withBlock.Pos.Map,
                            withBlock.Pos.X, withBlock.Pos.Y);

                        Protocol.WriteUpdateGold(UserIndex);
                    }
                    else if (MeterItemEnInventario(UserIndex, ref MiObj))
                    {
                        // Quitamos el objeto
                        EraseObj(Declaraciones.MapData[withBlock.Pos.Map, X, Y].ObjInfo.Amount, withBlock.Pos.Map,
                            withBlock.Pos.X, withBlock.Pos.Y);
                        if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) == 0)
                        {
                            var argtexto = "Agarro:" + MiObj.Amount + " Objeto:" +
                                           Declaraciones.objData[MiObj.ObjIndex].name;
                            General.LogGM(ref withBlock.name, ref argtexto);
                        }

                        // Log de Objetos que se agarran del piso. Pablo (ToxicWaste) 07/09/07
                        // Es un Objeto que tenemos que loguear?
                        if (Declaraciones.objData[MiObj.ObjIndex].Log == 1)
                        {
                            ObjPos = " Mapa: " + withBlock.Pos.Map + " X: " + withBlock.Pos.X + " Y: " +
                                     withBlock.Pos.Y;
                            General.LogDesarrollo(withBlock.name + " juntó del piso " + MiObj.Amount + " " +
                                                  Declaraciones.objData[MiObj.ObjIndex].name + ObjPos);
                        }
                        else if (MiObj.Amount > Declaraciones.MAX_INVENTORY_OBJS - 1000) // Es mucha cantidad?
                        {
                            // Si no es de los prohibidos de loguear, lo logueamos.
                            if (Declaraciones.objData[MiObj.ObjIndex].NoLog != 1)
                            {
                                ObjPos = " Mapa: " + withBlock.Pos.Map + " X: " + withBlock.Pos.X + " Y: " +
                                         withBlock.Pos.Y;
                                General.LogDesarrollo(withBlock.name + " juntó del piso " + MiObj.Amount + " " +
                                                      Declaraciones.objData[MiObj.ObjIndex].name + ObjPos);
                            }
                        }
                    }
                }
            }
            else
            {
                Protocol.WriteConsoleMsg(UserIndex, "No hay nada aquí.", Protocol.FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    public static void Desequipar(short UserIndex, byte Slot)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            // Desequipa el item slot del inventario
            // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
            Declaraciones.ObjData obj;

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                {
                    ref var withBlock1 = ref withBlock.Invent;
                    if ((Slot < 0) | (Slot > withBlock1.userObj.Length - 1)) return;

                    if (withBlock1.userObj[Slot].ObjIndex == 0) return;

                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto obj. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    obj = Declaraciones.objData[withBlock1.userObj[Slot].ObjIndex];
                }

                switch (obj.OBJType)
                {
                    case Declaraciones.eOBJType.otWeapon:
                    {
                        {
                            ref var withBlock2 = ref withBlock.Invent;
                            withBlock2.userObj[Slot].Equipped = 0;
                            withBlock2.WeaponEqpObjIndex = 0;
                            withBlock2.WeaponEqpSlot = 0;
                        }

                        if (!(withBlock.flags.Mimetizado == 1))
                        {
                            ref var withBlock3 = ref withBlock.character;
                            withBlock3.WeaponAnim = Declaraciones.NingunArma;
                            UsUaRiOs.ChangeUserChar(UserIndex, withBlock3.body, withBlock3.Head,
                                (byte)withBlock3.heading, withBlock3.WeaponAnim, withBlock3.ShieldAnim,
                                withBlock3.CascoAnim);
                        }

                        break;
                    }

                    case Declaraciones.eOBJType.otFlechas:
                    {
                        {
                            ref var withBlock4 = ref withBlock.Invent;
                            withBlock4.userObj[Slot].Equipped = 0;
                            withBlock4.MunicionEqpObjIndex = 0;
                            withBlock4.MunicionEqpSlot = 0;
                        }

                        break;
                    }

                    case Declaraciones.eOBJType.otAnillo:
                    {
                        {
                            ref var withBlock5 = ref withBlock.Invent;
                            withBlock5.userObj[Slot].Equipped = 0;
                            withBlock5.AnilloEqpObjIndex = 0;
                            withBlock5.AnilloEqpSlot = 0;
                        }

                        break;
                    }

                    case Declaraciones.eOBJType.otArmadura:
                    {
                        {
                            ref var withBlock6 = ref withBlock.Invent;
                            withBlock6.userObj[Slot].Equipped = 0;
                            withBlock6.ArmourEqpObjIndex = 0;
                            withBlock6.ArmourEqpSlot = 0;
                        }

                        General.DarCuerpoDesnudo(UserIndex, withBlock.flags.Mimetizado == 1);
                        {
                            ref var withBlock7 = ref withBlock.character;
                            UsUaRiOs.ChangeUserChar(UserIndex, withBlock7.body, withBlock7.Head,
                                (byte)withBlock7.heading, withBlock7.WeaponAnim, withBlock7.ShieldAnim,
                                withBlock7.CascoAnim);
                        }

                        break;
                    }

                    case Declaraciones.eOBJType.otCASCO:
                    {
                        {
                            ref var withBlock8 = ref withBlock.Invent;
                            withBlock8.userObj[Slot].Equipped = 0;
                            withBlock8.CascoEqpObjIndex = 0;
                            withBlock8.CascoEqpSlot = 0;
                        }

                        if (!(withBlock.flags.Mimetizado == 1))
                        {
                            ref var withBlock9 = ref withBlock.character;
                            withBlock9.CascoAnim = Declaraciones.NingunCasco;
                            UsUaRiOs.ChangeUserChar(UserIndex, withBlock9.body, withBlock9.Head,
                                (byte)withBlock9.heading, withBlock9.WeaponAnim, withBlock9.ShieldAnim,
                                withBlock9.CascoAnim);
                        }

                        break;
                    }

                    case Declaraciones.eOBJType.otESCUDO:
                    {
                        {
                            ref var withBlock10 = ref withBlock.Invent;
                            withBlock10.userObj[Slot].Equipped = 0;
                            withBlock10.EscudoEqpObjIndex = 0;
                            withBlock10.EscudoEqpSlot = 0;
                        }

                        if (!(withBlock.flags.Mimetizado == 1))
                        {
                            ref var withBlock11 = ref withBlock.character;
                            withBlock11.ShieldAnim = Declaraciones.NingunEscudo;
                            UsUaRiOs.ChangeUserChar(UserIndex, withBlock11.body, withBlock11.Head,
                                (byte)withBlock11.heading, withBlock11.WeaponAnim, withBlock11.ShieldAnim,
                                withBlock11.CascoAnim);
                        }

                        break;
                    }

                    case Declaraciones.eOBJType.otMochilas:
                    {
                        {
                            ref var withBlock12 = ref withBlock.Invent;
                            withBlock12.userObj[Slot].Equipped = 0;
                            withBlock12.MochilaEqpObjIndex = 0;
                            withBlock12.MochilaEqpSlot = 0;
                        }

                        TirarTodosLosItemsEnMochila(UserIndex);
                        withBlock.CurrentInventorySlots = Declaraciones.MAX_NORMAL_INVENTORY_SLOTS;
                        break;
                    }
                }
            }

            Protocol.WriteUpdateUserStats(UserIndex);
            UpdateUserInv(false, UserIndex, Slot);
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in GetObj: " + ex.Message);
            var argdesc = "Error en Desquipar. Error " + ex.GetType().Name + " : " + ex.Message;
            General.LogError(ref argdesc);
        }
    }

    public static bool SexoPuedeUsarItem(short UserIndex, short ObjIndex,
        [Optional] [DefaultParameterValue("")] ref string sMotivo)
    {
        bool SexoPuedeUsarItemRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: 14/01/2010 (ZaMa)
        // 14/01/2010: ZaMa - Agrego el motivo por el que no puede equipar/usar el item.
        // ***************************************************

        try
        {
            if (Declaraciones.objData[ObjIndex].Mujer == 1)
                SexoPuedeUsarItemRet = Declaraciones.UserList[UserIndex].Genero != Declaraciones.eGenero.Hombre;
            else if (Declaraciones.objData[ObjIndex].Hombre == 1)
                SexoPuedeUsarItemRet = Declaraciones.UserList[UserIndex].Genero != Declaraciones.eGenero.Mujer;
            else
                SexoPuedeUsarItemRet = true;

            if (!SexoPuedeUsarItemRet)
                sMotivo = "Tu género no puede usar este objeto.";
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in SexoPuedeUsarItem: " + ex.Message);
            var argdesc = "SexoPuedeUsarItem";
            General.LogError(ref argdesc);
        }

        return SexoPuedeUsarItemRet;
    }


    public static bool FaccionPuedeUsarItem(short UserIndex, short ObjIndex,
        [Optional] [DefaultParameterValue("")] ref string sMotivo)
    {
        bool FaccionPuedeUsarItemRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: 14/01/2010 (ZaMa)
        // 14/01/2010: ZaMa - Agrego el motivo por el que no puede equipar/usar el item.
        // ***************************************************

        if (Declaraciones.objData[ObjIndex].Real == 1)
        {
            if (!ES.criminal(UserIndex))
                FaccionPuedeUsarItemRet = Extra.esArmada(UserIndex);
            else
                FaccionPuedeUsarItemRet = false;
        }
        else if (Declaraciones.objData[ObjIndex].Caos == 1)
        {
            if (ES.criminal(UserIndex))
                FaccionPuedeUsarItemRet = Extra.esCaos(UserIndex);
            else
                FaccionPuedeUsarItemRet = false;
        }
        else
        {
            FaccionPuedeUsarItemRet = true;
        }

        if (!FaccionPuedeUsarItemRet)
            sMotivo = "Tu alineación no puede usar este objeto.";
        return FaccionPuedeUsarItemRet;
    }

    public static void EquiparInvItem(short UserIndex, byte Slot)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 14/01/2010 (ZaMa)
        // 01/08/2009: ZaMa - Now it's not sent any sound made by an invisible admin
        // 14/01/2010: ZaMa - Agrego el motivo especifico por el que no puede equipar/usar el item.
        // *************************************************

        try
        {
            // Equipa un item del inventario
            // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
            // UPGRADE_NOTE: Obj se actualizó a obj. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            Declaraciones.ObjData obj;
            short ObjIndex;
            var sMotivo = default(string);

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                ObjIndex = withBlock.Invent.userObj[Slot].ObjIndex;
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto obj. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                obj = Declaraciones.objData[ObjIndex];

                if ((obj.Newbie == 1) & !Extra.EsNewbie(UserIndex))
                {
                    Protocol.WriteConsoleMsg(UserIndex, "Sólo los newbies pueden usar este objeto.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    return;
                }

                switch (obj.OBJType)
                {
                    case Declaraciones.eOBJType.otWeapon:
                    {
                        if (ClasePuedeUsarItem(UserIndex, ObjIndex, ref sMotivo) &
                            FaccionPuedeUsarItem(UserIndex, ObjIndex, ref sMotivo))
                        {
                            // Si esta equipado lo quita
                            if (withBlock.Invent.userObj[Slot].Equipped != 0)
                            {
                                // Quitamos del inv el item
                                Desequipar(UserIndex, Slot);
                                // Animacion por defecto
                                if (withBlock.flags.Mimetizado == 1)
                                {
                                    withBlock.CharMimetizado.WeaponAnim = Declaraciones.NingunArma;
                                }
                                else
                                {
                                    withBlock.character.WeaponAnim = Declaraciones.NingunArma;
                                    UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body,
                                        withBlock.character.Head, (byte)withBlock.character.heading,
                                        withBlock.character.WeaponAnim, withBlock.character.ShieldAnim,
                                        withBlock.character.CascoAnim);
                                }

                                return;
                            }

                            // Quitamos el elemento anterior
                            if (withBlock.Invent.WeaponEqpObjIndex > 0)
                                Desequipar(UserIndex, withBlock.Invent.WeaponEqpSlot);

                            withBlock.Invent.userObj[Slot].Equipped = 1;
                            withBlock.Invent.WeaponEqpObjIndex = ObjIndex;
                            withBlock.Invent.WeaponEqpSlot = Slot;

                            // El sonido solo se envia si no lo produce un admin invisible
                            if (!(withBlock.flags.AdminInvisible == 1))
                                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                    Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.SND_SACARARMA),
                                        Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));

                            if (withBlock.flags.Mimetizado == 1)
                            {
                                withBlock.CharMimetizado.WeaponAnim = UsUaRiOs.GetWeaponAnim(UserIndex, ObjIndex);
                            }
                            else
                            {
                                withBlock.character.WeaponAnim = UsUaRiOs.GetWeaponAnim(UserIndex, ObjIndex);
                                UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body,
                                    withBlock.character.Head, (byte)withBlock.character.heading,
                                    withBlock.character.WeaponAnim, withBlock.character.ShieldAnim,
                                    withBlock.character.CascoAnim);
                            }
                        }
                        else
                        {
                            Protocol.WriteConsoleMsg(UserIndex, sMotivo, Protocol.FontTypeNames.FONTTYPE_INFO);
                        }

                        break;
                    }

                    case Declaraciones.eOBJType.otAnillo:
                    {
                        if (ClasePuedeUsarItem(UserIndex, ObjIndex, ref sMotivo) &
                            FaccionPuedeUsarItem(UserIndex, ObjIndex, ref sMotivo))
                        {
                            // Si esta equipado lo quita
                            if (withBlock.Invent.userObj[Slot].Equipped != 0)
                            {
                                // Quitamos del inv el item
                                Desequipar(UserIndex, Slot);
                                return;
                            }

                            // Quitamos el elemento anterior
                            if (withBlock.Invent.AnilloEqpObjIndex > 0)
                                Desequipar(UserIndex, withBlock.Invent.AnilloEqpSlot);

                            withBlock.Invent.userObj[Slot].Equipped = 1;
                            withBlock.Invent.AnilloEqpObjIndex = ObjIndex;
                            withBlock.Invent.AnilloEqpSlot = Slot;
                        }

                        else
                        {
                            Protocol.WriteConsoleMsg(UserIndex, sMotivo, Protocol.FontTypeNames.FONTTYPE_INFO);
                        }

                        break;
                    }

                    case Declaraciones.eOBJType.otFlechas:
                    {
                        if (ClasePuedeUsarItem(UserIndex, ObjIndex, ref sMotivo) &
                            FaccionPuedeUsarItem(UserIndex, ObjIndex, ref sMotivo))
                        {
                            // Si esta equipado lo quita
                            if (withBlock.Invent.userObj[Slot].Equipped != 0)
                            {
                                // Quitamos del inv el item
                                Desequipar(UserIndex, Slot);
                                return;
                            }

                            // Quitamos el elemento anterior
                            if (withBlock.Invent.MunicionEqpObjIndex > 0)
                                Desequipar(UserIndex, withBlock.Invent.MunicionEqpSlot);

                            withBlock.Invent.userObj[Slot].Equipped = 1;
                            withBlock.Invent.MunicionEqpObjIndex = ObjIndex;
                            withBlock.Invent.MunicionEqpSlot = Slot;
                        }

                        else
                        {
                            Protocol.WriteConsoleMsg(UserIndex, sMotivo, Protocol.FontTypeNames.FONTTYPE_INFO);
                        }

                        break;
                    }

                    case Declaraciones.eOBJType.otArmadura:
                    {
                        if (withBlock.flags.Navegando == 1)
                            return;

                        // Nos aseguramos que puede usarla
                        if (ClasePuedeUsarItem(UserIndex, ObjIndex, ref sMotivo) &
                            SexoPuedeUsarItem(UserIndex, ObjIndex, ref sMotivo) &
                            CheckRazaUsaRopa(UserIndex, ref ObjIndex, ref sMotivo) &
                            FaccionPuedeUsarItem(UserIndex, ObjIndex, ref sMotivo))
                        {
                            // Si esta equipado lo quita
                            if (withBlock.Invent.userObj[Slot].Equipped != 0)
                            {
                                Desequipar(UserIndex, Slot);
                                General.DarCuerpoDesnudo(UserIndex, withBlock.flags.Mimetizado == 1);
                                if (!(withBlock.flags.Mimetizado == 1))
                                    UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body,
                                        withBlock.character.Head, (byte)withBlock.character.heading,
                                        withBlock.character.WeaponAnim, withBlock.character.ShieldAnim,
                                        withBlock.character.CascoAnim);
                                return;
                            }

                            // Quita el anterior
                            if (withBlock.Invent.ArmourEqpObjIndex > 0)
                                Desequipar(UserIndex, withBlock.Invent.ArmourEqpSlot);

                            // Lo equipa
                            withBlock.Invent.userObj[Slot].Equipped = 1;
                            withBlock.Invent.ArmourEqpObjIndex = ObjIndex;
                            withBlock.Invent.ArmourEqpSlot = Slot;

                            if (withBlock.flags.Mimetizado == 1)
                            {
                                withBlock.CharMimetizado.body = obj.Ropaje;
                            }
                            else
                            {
                                withBlock.character.body = obj.Ropaje;
                                UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body,
                                    withBlock.character.Head, (byte)withBlock.character.heading,
                                    withBlock.character.WeaponAnim, withBlock.character.ShieldAnim,
                                    withBlock.character.CascoAnim);
                            }

                            withBlock.flags.Desnudo = 0;
                        }
                        else
                        {
                            Protocol.WriteConsoleMsg(UserIndex, sMotivo, Protocol.FontTypeNames.FONTTYPE_INFO);
                        }

                        break;
                    }

                    case Declaraciones.eOBJType.otCASCO:
                    {
                        if (withBlock.flags.Navegando == 1)
                            return;
                        if (ClasePuedeUsarItem(UserIndex, ObjIndex, ref sMotivo))
                        {
                            // Si esta equipado lo quita
                            if (withBlock.Invent.userObj[Slot].Equipped != 0)
                            {
                                Desequipar(UserIndex, Slot);
                                if (withBlock.flags.Mimetizado == 1)
                                {
                                    withBlock.CharMimetizado.CascoAnim = Declaraciones.NingunCasco;
                                }
                                else
                                {
                                    withBlock.character.CascoAnim = Declaraciones.NingunCasco;
                                    UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body,
                                        withBlock.character.Head, (byte)withBlock.character.heading,
                                        withBlock.character.WeaponAnim, withBlock.character.ShieldAnim,
                                        withBlock.character.CascoAnim);
                                }

                                return;
                            }

                            // Quita el anterior
                            if (withBlock.Invent.CascoEqpObjIndex > 0)
                                Desequipar(UserIndex, withBlock.Invent.CascoEqpSlot);

                            // Lo equipa

                            withBlock.Invent.userObj[Slot].Equipped = 1;
                            withBlock.Invent.CascoEqpObjIndex = ObjIndex;
                            withBlock.Invent.CascoEqpSlot = Slot;
                            if (withBlock.flags.Mimetizado == 1)
                            {
                                withBlock.CharMimetizado.CascoAnim = obj.CascoAnim;
                            }
                            else
                            {
                                withBlock.character.CascoAnim = obj.CascoAnim;
                                UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body,
                                    withBlock.character.Head, (byte)withBlock.character.heading,
                                    withBlock.character.WeaponAnim, withBlock.character.ShieldAnim,
                                    withBlock.character.CascoAnim);
                            }
                        }
                        else
                        {
                            Protocol.WriteConsoleMsg(UserIndex, sMotivo, Protocol.FontTypeNames.FONTTYPE_INFO);
                        }

                        break;
                    }

                    case Declaraciones.eOBJType.otESCUDO:
                    {
                        if (withBlock.flags.Navegando == 1)
                            return;

                        if (ClasePuedeUsarItem(UserIndex, ObjIndex, ref sMotivo) &
                            FaccionPuedeUsarItem(UserIndex, ObjIndex, ref sMotivo))
                        {
                            // Si esta equipado lo quita
                            if (withBlock.Invent.userObj[Slot].Equipped != 0)
                            {
                                Desequipar(UserIndex, Slot);
                                if (withBlock.flags.Mimetizado == 1)
                                {
                                    withBlock.CharMimetizado.ShieldAnim = Declaraciones.NingunEscudo;
                                }
                                else
                                {
                                    withBlock.character.ShieldAnim = Declaraciones.NingunEscudo;
                                    UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body,
                                        withBlock.character.Head, (byte)withBlock.character.heading,
                                        withBlock.character.WeaponAnim, withBlock.character.ShieldAnim,
                                        withBlock.character.CascoAnim);
                                }

                                return;
                            }

                            // Quita el anterior
                            if (withBlock.Invent.EscudoEqpObjIndex > 0)
                                Desequipar(UserIndex, withBlock.Invent.EscudoEqpSlot);

                            // Lo equipa

                            withBlock.Invent.userObj[Slot].Equipped = 1;
                            withBlock.Invent.EscudoEqpObjIndex = ObjIndex;
                            withBlock.Invent.EscudoEqpSlot = Slot;

                            if (withBlock.flags.Mimetizado == 1)
                            {
                                withBlock.CharMimetizado.ShieldAnim = obj.ShieldAnim;
                            }
                            else
                            {
                                withBlock.character.ShieldAnim = obj.ShieldAnim;

                                UsUaRiOs.ChangeUserChar(UserIndex, withBlock.character.body,
                                    withBlock.character.Head, (byte)withBlock.character.heading,
                                    withBlock.character.WeaponAnim, withBlock.character.ShieldAnim,
                                    withBlock.character.CascoAnim);
                            }
                        }
                        else
                        {
                            Protocol.WriteConsoleMsg(UserIndex, sMotivo, Protocol.FontTypeNames.FONTTYPE_INFO);
                        }

                        break;
                    }

                    case Declaraciones.eOBJType.otMochilas:
                    {
                        if (withBlock.flags.Muerto == 1)
                        {
                            Protocol.WriteConsoleMsg(UserIndex,
                                "¡¡Estas muerto!! Solo podes usar items cuando estas vivo. ",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            return;
                        }

                        if (withBlock.Invent.userObj[Slot].Equipped != 0)
                        {
                            Desequipar(UserIndex, Slot);
                            return;
                        }

                        if (withBlock.Invent.MochilaEqpObjIndex > 0)
                            Desequipar(UserIndex, withBlock.Invent.MochilaEqpSlot);
                        withBlock.Invent.userObj[Slot].Equipped = 1;
                        withBlock.Invent.MochilaEqpObjIndex = ObjIndex;
                        withBlock.Invent.MochilaEqpSlot = Slot;
                        withBlock.CurrentInventorySlots =
                            Convert.ToByte(Declaraciones.MAX_NORMAL_INVENTORY_SLOTS + obj.MochilaType * 5);
                        Protocol.WriteAddSlots(UserIndex,
                            (Declaraciones.eMochilas)Convert.ToInt32(obj.MochilaType));
                        break;
                    }
                }
            }

            // Actualiza
            UpdateUserInv(false, UserIndex, Slot);
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in FaccionPuedeUsarItem: " + ex.Message);
            var argdesc = "EquiparInvItem Slot:" + Slot + " - Error: " + ex.GetType().Name + " - Error Description : " +
                          ex.Message;
            General.LogError(ref argdesc);
        }
    }

    private static bool CheckRazaUsaRopa(short UserIndex, ref short ItemIndex,
        [Optional] [DefaultParameterValue("")] ref string sMotivo)
    {
        bool CheckRazaUsaRopaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: 14/01/2010 (ZaMa)
        // 14/01/2010: ZaMa - Agrego el motivo por el que no puede equipar/usar el item.
        // ***************************************************

        try
        {
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // Verifica si la raza puede usar la ropa
                if ((withBlock.raza == Declaraciones.eRaza.Humano) | (withBlock.raza == Declaraciones.eRaza.Elfo) |
                    (withBlock.raza == Declaraciones.eRaza.Drow))
                    CheckRazaUsaRopaRet = Declaraciones.objData[ItemIndex].RazaEnana == 0;
                else
                    CheckRazaUsaRopaRet = Declaraciones.objData[ItemIndex].RazaEnana == 1;

                // Solo se habilita la ropa exclusiva para Drows por ahora. Pablo (ToxicWaste)
                if ((withBlock.raza != Declaraciones.eRaza.Drow) &
                    (Declaraciones.objData[ItemIndex].RazaDrow != 0)) CheckRazaUsaRopaRet = false;
            }

            if (!CheckRazaUsaRopaRet)
                sMotivo = "Tu raza no puede usar este objeto.";
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in CheckRazaUsaRopa: " + ex.Message);
            var argdesc = "Error CheckRazaUsaRopa ItemIndex:" + ItemIndex;
            General.LogError(ref argdesc);
        }

        return CheckRazaUsaRopaRet;
    }

    public static void UseInvItem(short UserIndex, byte Slot)
    {
        // *************************************************
        // Author: Unknown
        // Last modified: 10/12/2009
        // Handels the usage of items from inventory box.
        // 24/01/2007 Pablo (ToxicWaste) - Agrego el Cuerno de la Armada y la Legión.
        // 24/01/2007 Pablo (ToxicWaste) - Utilización nueva de Barco en lvl 20 por clase Pirata y Pescador.
        // 01/08/2009: ZaMa - Now it's not sent any sound made by an invisible admin, except to its own client
        // 17/11/2009: ZaMa - Ahora se envia una orientacion de la posicion hacia donde esta el que uso el cuerno.
        // 27/11/2009: Budi - Se envia indivualmente cuando se modifica a la Agilidad o la Fuerza del personaje.
        // 08/12/2009: ZaMa - Agrego el uso de hacha de madera elfica.
        // 10/12/2009: ZaMa - Arreglos y validaciones en todos las herramientas de trabajo.
        // *************************************************

        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Obj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Declaraciones.ObjData obj;
        short ObjIndex;
        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura TargObj, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Declaraciones.ObjData TargObj;
        Declaraciones.Obj MiObj;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];

            if (withBlock.Invent.userObj[Slot].Amount == 0)
                return;

            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto obj. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            obj = Declaraciones.objData[withBlock.Invent.userObj[Slot].ObjIndex];

            if ((obj.Newbie == 1) & !Extra.EsNewbie(UserIndex))
            {
                Protocol.WriteConsoleMsg(UserIndex, "Sólo los newbies pueden usar estos objetos.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                return;
            }

            if (obj.OBJType == Declaraciones.eOBJType.otWeapon)
            {
                if (obj.proyectil == 1)
                {
                    // valido para evitar el flood pero no bloqueo. El bloqueo se hace en WLC con proyectiles.
                    if (!modNuevoTimer.IntervaloPermiteUsar(UserIndex, false))
                        return;
                }
                // dagas
                else if (!modNuevoTimer.IntervaloPermiteUsar(UserIndex))
                {
                    return;
                }
            }
            else if (!modNuevoTimer.IntervaloPermiteUsar(UserIndex))
            {
                return;
            }

            ObjIndex = withBlock.Invent.userObj[Slot].ObjIndex;
            withBlock.flags.TargetObjInvIndex = ObjIndex;
            withBlock.flags.TargetObjInvSlot = Slot;

            switch (obj.OBJType)
            {
                case Declaraciones.eOBJType.otUseOnce:
                {
                    if (withBlock.flags.Muerto == 1)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    // Usa el item
                    withBlock.Stats.MinHam = (short)(withBlock.Stats.MinHam + obj.MinHam);
                    if (withBlock.Stats.MinHam > withBlock.Stats.MaxHam)
                        withBlock.Stats.MinHam = withBlock.Stats.MaxHam;
                    withBlock.flags.Hambre = 0;
                    Protocol.WriteUpdateHungerAndThirst(UserIndex);
                    // Sonido

                    if ((ObjIndex == (int)Declaraciones.e_ObjetosCriticos.Manzana) |
                        (ObjIndex == (int)Declaraciones.e_ObjetosCriticos.Manzana2) |
                        (ObjIndex == (int)Declaraciones.e_ObjetosCriticos.ManzanaNewbie))
                        Declaraciones.SonidosMapas.ReproducirSonido(modSendData.SendTarget.ToPCArea, UserIndex,
                            Convert.ToInt16((int)SoundMapInfo.e_SoundIndex.MORFAR_MANZANA));
                    else
                        Declaraciones.SonidosMapas.ReproducirSonido(modSendData.SendTarget.ToPCArea, UserIndex,
                            Convert.ToInt16((int)SoundMapInfo.e_SoundIndex.SOUND_COMIDA));

                    // Quitamos del inv el item
                    QuitarUserInvItem(UserIndex, Slot, 1);

                    UpdateUserInv(false, UserIndex, Slot);
                    break;
                }

                case Declaraciones.eOBJType.otGuita:
                {
                    if (withBlock.flags.Muerto == 1)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    withBlock.Stats.GLD = withBlock.Stats.GLD + withBlock.Invent.userObj[Slot].Amount;
                    withBlock.Invent.userObj[Slot].Amount = 0;
                    withBlock.Invent.userObj[Slot].ObjIndex = 0;
                    withBlock.Invent.NroItems = Convert.ToInt16(withBlock.Invent.NroItems - 1);

                    UpdateUserInv(false, UserIndex, Slot);
                    Protocol.WriteUpdateGold(UserIndex);
                    break;
                }

                case Declaraciones.eOBJType.otWeapon:
                {
                    if (withBlock.flags.Muerto == 1)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    if (!(withBlock.Stats.MinSta > 0))
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "Estás muy cansad" + (withBlock.Genero == Declaraciones.eGenero.Hombre ? "o" : "a") + ".",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    if (Declaraciones.objData[ObjIndex].proyectil == 1)
                    {
                        if (withBlock.Invent.userObj[Slot].Equipped == 0)
                        {
                            Protocol.WriteConsoleMsg(UserIndex, "Antes de usar la herramienta deberías equipartela.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            return;
                        }

                        Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.WorkRequestTarget,
                            (int)Declaraciones.eSkill.Proyectiles);
                    }
                    // Call WriteWorkRequestTarget(UserIndex, Proyectiles)
                    else if (withBlock.flags.TargetObj == Declaraciones.Leña)
                    {
                        if (withBlock.Invent.userObj[Slot].ObjIndex == Declaraciones.DAGA)
                        {
                            if (withBlock.Invent.userObj[Slot].Equipped == 0)
                            {
                                Protocol.WriteConsoleMsg(UserIndex,
                                    "Antes de usar la herramienta deberías equipartela.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                                return;
                            }

                            Trabajo.TratarDeHacerFogata(withBlock.flags.TargetObjMap, withBlock.flags.TargetObjX,
                                withBlock.flags.TargetObjY, UserIndex);
                        }
                    }
                    else
                    {
                        switch (ObjIndex)
                        {
                            case Declaraciones.CAÑA_PESCA:
                            case Declaraciones.RED_PESCA:
                            {
                                if ((withBlock.Invent.WeaponEqpObjIndex == Declaraciones.CAÑA_PESCA) |
                                    (withBlock.Invent.WeaponEqpObjIndex == Declaraciones.RED_PESCA))
                                    Protocol.WriteMultiMessage(UserIndex,
                                        (short)Declaraciones.eMessages.WorkRequestTarget,
                                        (int)Declaraciones.eSkill.Pesca);
                                // Call WriteWorkRequestTarget(UserIndex, eSkill.Pesca)
                                else
                                    Protocol.WriteConsoleMsg(UserIndex,
                                        "Debes tener equipada la herramienta para trabajar.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO);

                                break;
                            }

                            case Declaraciones.HACHA_LEÑADOR:
                            case Declaraciones.HACHA_LEÑA_ELFICA:
                            {
                                if ((withBlock.Invent.WeaponEqpObjIndex == Declaraciones.HACHA_LEÑADOR) |
                                    (withBlock.Invent.WeaponEqpObjIndex == Declaraciones.HACHA_LEÑA_ELFICA))
                                    Protocol.WriteMultiMessage(UserIndex,
                                        (short)Declaraciones.eMessages.WorkRequestTarget,
                                        (int)Declaraciones.eSkill.Talar);
                                else
                                    Protocol.WriteConsoleMsg(UserIndex,
                                        "Debes tener equipada la herramienta para trabajar.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO);

                                break;
                            }

                            case Declaraciones.PIQUETE_MINERO:
                            {
                                if (withBlock.Invent.WeaponEqpObjIndex == Declaraciones.PIQUETE_MINERO)
                                    Protocol.WriteMultiMessage(UserIndex,
                                        (short)Declaraciones.eMessages.WorkRequestTarget,
                                        (int)Declaraciones.eSkill.Mineria);
                                else
                                    Protocol.WriteConsoleMsg(UserIndex,
                                        "Debes tener equipada la herramienta para trabajar.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO);

                                break;
                            }

                            case Declaraciones.MARTILLO_HERRERO:
                            {
                                if (withBlock.Invent.WeaponEqpObjIndex == Declaraciones.MARTILLO_HERRERO)
                                    Protocol.WriteMultiMessage(UserIndex,
                                        (short)Declaraciones.eMessages.WorkRequestTarget,
                                        (int)Declaraciones.eSkill.Herreria);
                                else
                                    Protocol.WriteConsoleMsg(UserIndex,
                                        "Debes tener equipada la herramienta para trabajar.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO);

                                break;
                            }

                            case Declaraciones.SERRUCHO_CARPINTERO:
                            {
                                if (withBlock.Invent.WeaponEqpObjIndex == Declaraciones.SERRUCHO_CARPINTERO)
                                {
                                    EnivarObjConstruibles(UserIndex);
                                    Protocol.WriteShowCarpenterForm(UserIndex);
                                }
                                else
                                {
                                    Protocol.WriteConsoleMsg(UserIndex,
                                        "Debes tener equipada la herramienta para trabajar.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO);
                                } // Las herramientas no se pueden fundir

                                break;
                            }

                            default:
                            {
                                if (Declaraciones.objData[ObjIndex].SkHerreria > 0)
                                    // Solo objetos que pueda hacer el herrero
                                    Protocol.WriteMultiMessage(UserIndex,
                                        (short)Declaraciones.eMessages.WorkRequestTarget, Declaraciones.FundirMetal);
                                // Call WriteWorkRequestTarget(UserIndex, FundirMetal)
                                break;
                            }
                        }
                    }

                    break;
                }

                case Declaraciones.eOBJType.otPociones:
                {
                    if (withBlock.flags.Muerto == 1)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo. ",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    if (!modNuevoTimer.IntervaloPermiteGolpeUsar(UserIndex, false))
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "¡¡Debes esperar unos momentos para tomar otra poción!!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    withBlock.flags.TomoPocion = true;
                    withBlock.flags.TipoPocion = obj.TipoPocion;

                    switch (withBlock.flags.TipoPocion)
                    {
                        case 1: // Modif la agilidad
                        {
                            withBlock.flags.DuracionEfecto = obj.DuracionEfecto;

                            // Usa el item
                            withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] = Convert.ToByte(
                                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] +
                                Matematicas.RandomNumber(obj.MinModificador, obj.MaxModificador));
                            if (withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] >
                                Declaraciones.MAXATRIBUTOS)
                                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] =
                                    Declaraciones.MAXATRIBUTOS;
                            if (withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] >
                                2 * withBlock.Stats.UserAtributosBackUP[(int)Declaraciones.eAtributos.Agilidad])

                                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Agilidad] =
                                    Convert.ToByte(2 *
                                                   withBlock.Stats.UserAtributosBackUP[
                                                       (int)Declaraciones.eAtributos.Agilidad]);

                            // Quitamos del inv el item
                            QuitarUserInvItem(UserIndex, Slot, 1);

                            // Los admin invisibles solo producen sonidos a si mismos
                            if (withBlock.flags.AdminInvisible == 1)
                            {
                                var argdatos = Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.SND_BEBER),
                                    Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y));
                                TCP.EnviarDatosASlot(UserIndex, ref argdatos);
                            }
                            else
                            {
                                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                    Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.SND_BEBER),
                                        Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));
                            }

                            Protocol.WriteUpdateDexterity(UserIndex);
                            break;
                        }

                        case 2: // Modif la fuerza
                        {
                            withBlock.flags.DuracionEfecto = obj.DuracionEfecto;

                            // Usa el item
                            withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] = Convert.ToByte(
                                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] +
                                Matematicas.RandomNumber(obj.MinModificador, obj.MaxModificador));
                            if (withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] >
                                Declaraciones.MAXATRIBUTOS)
                                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] =
                                    Declaraciones.MAXATRIBUTOS;
                            if (withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] >
                                2 * withBlock.Stats.UserAtributosBackUP[(int)Declaraciones.eAtributos.Fuerza])

                                withBlock.Stats.UserAtributos[(int)Declaraciones.eAtributos.Fuerza] =
                                    Convert.ToByte(2 *
                                                   withBlock.Stats.UserAtributosBackUP[
                                                       (int)Declaraciones.eAtributos.Fuerza]);


                            // Quitamos del inv el item
                            QuitarUserInvItem(UserIndex, Slot, 1);

                            // Los admin invisibles solo producen sonidos a si mismos
                            if (withBlock.flags.AdminInvisible == 1)
                            {
                                var argdatos1 = Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.SND_BEBER),
                                    Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y));
                                TCP.EnviarDatosASlot(UserIndex, ref argdatos1);
                            }
                            else
                            {
                                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                    Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.SND_BEBER),
                                        Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));
                            }

                            Protocol.WriteUpdateStrenght(UserIndex);
                            break;
                        }

                        case 3: // Pocion roja, restaura HP
                        {
                            // Usa el item
                            withBlock.Stats.MinHp = (short)(withBlock.Stats.MinHp +
                                                            Convert.ToInt16(Matematicas.RandomNumber(
                                                                obj.MinModificador,
                                                                obj.MaxModificador)));
                            if (withBlock.Stats.MinHp > withBlock.Stats.MaxHp)
                                withBlock.Stats.MinHp = withBlock.Stats.MaxHp;

                            // Quitamos del inv el item
                            QuitarUserInvItem(UserIndex, Slot, 1);

                            // Los admin invisibles solo producen sonidos a si mismos
                            if (withBlock.flags.AdminInvisible == 1)
                            {
                                var argdatos2 = Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.SND_BEBER),
                                    Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y));
                                TCP.EnviarDatosASlot(UserIndex, ref argdatos2);
                            }
                            else
                            {
                                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                    Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.SND_BEBER),
                                        Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));
                            }

                            break;
                        }

                        case 4: // Pocion azul, restaura MANA
                        {
                            // Usa el item
                            // nuevo calculo para recargar mana
                            withBlock.Stats.MinMAN = Convert.ToInt16(withBlock.Stats.MinMAN +
                                                                     Matematicas.Porcentaje(withBlock.Stats.MaxMAN, 4) +
                                                                     withBlock.Stats.ELV / 2 +
                                                                     40d / withBlock.Stats.ELV);
                            if (withBlock.Stats.MinMAN > withBlock.Stats.MaxMAN)
                                withBlock.Stats.MinMAN = withBlock.Stats.MaxMAN;

                            // Quitamos del inv el item
                            QuitarUserInvItem(UserIndex, Slot, 1);

                            // Los admin invisibles solo producen sonidos a si mismos
                            if (withBlock.flags.AdminInvisible == 1)
                            {
                                var argdatos3 = Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.SND_BEBER),
                                    Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y));
                                TCP.EnviarDatosASlot(UserIndex, ref argdatos3);
                            }
                            else
                            {
                                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                    Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.SND_BEBER),
                                        Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));
                            }

                            break;
                        }

                        case 5: // Pocion violeta
                        {
                            if (withBlock.flags.Envenenado == 1)
                            {
                                withBlock.flags.Envenenado = 0;
                                Protocol.WriteConsoleMsg(UserIndex, "Te has curado del envenenamiento.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                            }

                            // Quitamos del inv el item
                            QuitarUserInvItem(UserIndex, Slot, 1);

                            // Los admin invisibles solo producen sonidos a si mismos
                            if (withBlock.flags.AdminInvisible == 1)
                            {
                                var argdatos4 = Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.SND_BEBER),
                                    Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y));
                                TCP.EnviarDatosASlot(UserIndex, ref argdatos4);
                            }
                            else
                            {
                                modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                                    Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.SND_BEBER),
                                        Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));
                            }

                            break;
                        }

                        case 6: // Pocion Negra
                        {
                            if ((withBlock.flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                            {
                                QuitarUserInvItem(UserIndex, Slot, 1);
                                UsUaRiOs.UserDie(UserIndex);
                                Protocol.WriteConsoleMsg(UserIndex, "Sientes un gran mareo y pierdes el conocimiento.",
                                    Protocol.FontTypeNames.FONTTYPE_FIGHT);
                            }

                            break;
                        }
                    }

                    Protocol.WriteUpdateUserStats(UserIndex);
                    UpdateUserInv(false, UserIndex, Slot);
                    break;
                }

                case Declaraciones.eOBJType.otBebidas:
                {
                    if (withBlock.flags.Muerto == 1)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    withBlock.Stats.MinAGU = (short)(withBlock.Stats.MinAGU + obj.MinSed);
                    if (withBlock.Stats.MinAGU > withBlock.Stats.MaxAGU)
                        withBlock.Stats.MinAGU = withBlock.Stats.MaxAGU;
                    withBlock.flags.Sed = 0;
                    Protocol.WriteUpdateHungerAndThirst(UserIndex);

                    // Quitamos del inv el item
                    QuitarUserInvItem(UserIndex, Slot, 1);

                    // Los admin invisibles solo producen sonidos a si mismos
                    if (withBlock.flags.AdminInvisible == 1)
                    {
                        var argdatos5 = Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.SND_BEBER),
                            Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y));
                        TCP.EnviarDatosASlot(UserIndex, ref argdatos5);
                    }
                    else
                    {
                        modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                            Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.SND_BEBER),
                                Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));
                    }

                    UpdateUserInv(false, UserIndex, Slot);
                    break;
                }

                case Declaraciones.eOBJType.otLlaves:
                {
                    if (withBlock.flags.Muerto == 1)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    if (withBlock.flags.TargetObj == 0)
                        return;
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto TargObj. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    TargObj = Declaraciones.objData[withBlock.flags.TargetObj];
                    // ¿El objeto clickeado es una puerta?
                    if (TargObj.OBJType == Declaraciones.eOBJType.otPuertas)
                    {
                        // ¿Esta cerrada?
                        if (TargObj.Cerrada == 1)
                        {
                            // ¿Cerrada con llave?
                            if (TargObj.Llave > 0)
                            {
                                if (TargObj.clave == obj.clave)
                                {
                                    Declaraciones.MapData[withBlock.flags.TargetObjMap, withBlock.flags.TargetObjX,
                                        withBlock.flags.TargetObjY].ObjInfo.ObjIndex = Declaraciones
                                        .objData[
                                            Declaraciones.MapData[withBlock.flags.TargetObjMap,
                                                withBlock.flags.TargetObjX,
                                                withBlock.flags.TargetObjY].ObjInfo.ObjIndex].IndexCerrada;
                                    withBlock.flags.TargetObj = Declaraciones.MapData[withBlock.flags.TargetObjMap,
                                        withBlock.flags.TargetObjX, withBlock.flags.TargetObjY].ObjInfo.ObjIndex;
                                    Protocol.WriteConsoleMsg(UserIndex, "Has abierto la puerta.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO);
                                }
                                else
                                {
                                    Protocol.WriteConsoleMsg(UserIndex, "La llave no sirve.",
                                        Protocol.FontTypeNames.FONTTYPE_INFO);
                                }
                            }
                            else if (TargObj.clave == obj.clave)
                            {
                                Declaraciones.MapData[withBlock.flags.TargetObjMap, withBlock.flags.TargetObjX,
                                    withBlock.flags.TargetObjY].ObjInfo.ObjIndex = Declaraciones
                                    .objData[
                                        Declaraciones.MapData[withBlock.flags.TargetObjMap, withBlock.flags.TargetObjX,
                                            withBlock.flags.TargetObjY].ObjInfo.ObjIndex].IndexCerradaLlave;
                                Protocol.WriteConsoleMsg(UserIndex, "Has cerrado con llave la puerta.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                                withBlock.flags.TargetObj = Declaraciones.MapData[withBlock.flags.TargetObjMap,
                                    withBlock.flags.TargetObjX, withBlock.flags.TargetObjY].ObjInfo.ObjIndex;
                            }
                            else
                            {
                                Protocol.WriteConsoleMsg(UserIndex, "La llave no sirve.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                            }
                        }
                        else
                        {
                            Protocol.WriteConsoleMsg(UserIndex, "No está cerrada.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                        }
                    }

                    break;
                }

                case Declaraciones.eOBJType.otBotellaVacia:
                {
                    if (withBlock.flags.Muerto == 1)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    if (!General.HayAgua(withBlock.Pos.Map, withBlock.flags.TargetX, withBlock.flags.TargetY))
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "No hay agua allí.", Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    MiObj.Amount = 1;
                    MiObj.ObjIndex = Declaraciones.objData[withBlock.Invent.userObj[Slot].ObjIndex]
                        .IndexAbierta;
                    QuitarUserInvItem(UserIndex, Slot, 1);
                    if (!MeterItemEnInventario(UserIndex, ref MiObj))
                    {
                        var argNotPirata = true;
                        InvNpc.TirarItemAlPiso(ref withBlock.Pos, ref MiObj, ref argNotPirata);
                    }

                    UpdateUserInv(false, UserIndex, Slot);
                    break;
                }

                case Declaraciones.eOBJType.otBotellaLlena:
                {
                    if (withBlock.flags.Muerto == 1)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    withBlock.Stats.MinAGU = (short)(withBlock.Stats.MinAGU + obj.MinSed);
                    if (withBlock.Stats.MinAGU > withBlock.Stats.MaxAGU)
                        withBlock.Stats.MinAGU = withBlock.Stats.MaxAGU;
                    withBlock.flags.Sed = 0;
                    Protocol.WriteUpdateHungerAndThirst(UserIndex);
                    MiObj.Amount = 1;
                    MiObj.ObjIndex = Declaraciones.objData[withBlock.Invent.userObj[Slot].ObjIndex]
                        .IndexCerrada;
                    QuitarUserInvItem(UserIndex, Slot, 1);
                    if (!MeterItemEnInventario(UserIndex, ref MiObj))
                    {
                        var argNotPirata1 = true;
                        InvNpc.TirarItemAlPiso(ref withBlock.Pos, ref MiObj, ref argNotPirata1);
                    }

                    UpdateUserInv(false, UserIndex, Slot);
                    break;
                }

                case Declaraciones.eOBJType.otPergaminos:
                {
                    if (withBlock.flags.Muerto == 1)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    if (withBlock.Stats.MaxMAN > 0)
                    {
                        if ((withBlock.flags.Hambre == 0) & (withBlock.flags.Sed == 0))
                        {
                            modHechizos.AgregarHechizo(UserIndex, Slot);
                            UpdateUserInv(false, UserIndex, Slot);
                        }
                        else
                        {
                            Protocol.WriteConsoleMsg(UserIndex, "Estás demasiado hambriento y sediento.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                        }
                    }
                    else
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "No tienes conocimientos de las Artes Arcanas.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                    }

                    break;
                }
                case Declaraciones.eOBJType.otMinerales:
                {
                    if (withBlock.flags.Muerto == 1)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    Protocol.WriteMultiMessage(UserIndex, (short)Declaraciones.eMessages.WorkRequestTarget,
                        Declaraciones.FundirMetal);
                    break;
                }
                // Call WriteWorkRequestTarget(UserIndex, FundirMetal)

                case Declaraciones.eOBJType.otInstrumentos:
                {
                    if (withBlock.flags.Muerto == 1)
                    {
                        Protocol.WriteConsoleMsg(UserIndex,
                            "¡¡Estás muerto!! Sólo puedes usar ítems cuando estás vivo.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    if (obj.Real != 0) // ¿Es el Cuerno Real?
                    {
                        var argsMotivo = "";
                        if (FaccionPuedeUsarItem(UserIndex, ObjIndex, ref argsMotivo))
                        {
                            if (!Declaraciones.mapInfo[withBlock.Pos.Map].Pk)
                            {
                                Protocol.WriteConsoleMsg(UserIndex, "No hay peligro aquí. Es zona segura.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                                return;
                            }

                            // Los admin invisibles solo producen sonidos a si mismos
                            if (withBlock.flags.AdminInvisible == 1)
                            {
                                var argdatos6 = Protocol.PrepareMessagePlayWave(Convert.ToByte(obj.Snd1),
                                    Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y));
                                TCP.EnviarDatosASlot(UserIndex, ref argdatos6);
                            }
                            else
                            {
                                modSendData.AlertarFaccionarios(UserIndex);
                                modSendData.SendData(modSendData.SendTarget.toMap, withBlock.Pos.Map,
                                    Protocol.PrepareMessagePlayWave(Convert.ToByte(obj.Snd1),
                                        Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));
                            }

                            return;
                        }

                        Protocol.WriteConsoleMsg(UserIndex, "Sólo miembros del ejército real pueden usar este cuerno.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    if (obj.Caos != 0) // ¿Es el Cuerno Legión?
                    {
                        var argsMotivo1 = "";
                        if (FaccionPuedeUsarItem(UserIndex, ObjIndex, ref argsMotivo1))
                        {
                            if (!Declaraciones.mapInfo[withBlock.Pos.Map].Pk)
                            {
                                Protocol.WriteConsoleMsg(UserIndex, "No hay peligro aquí. Es zona segura.",
                                    Protocol.FontTypeNames.FONTTYPE_INFO);
                                return;
                            }

                            // Los admin invisibles solo producen sonidos a si mismos
                            if (withBlock.flags.AdminInvisible == 1)
                            {
                                var argdatos7 = Protocol.PrepareMessagePlayWave(Convert.ToByte(obj.Snd1),
                                    Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y));
                                TCP.EnviarDatosASlot(UserIndex, ref argdatos7);
                            }
                            else
                            {
                                modSendData.AlertarFaccionarios(UserIndex);
                                modSendData.SendData(modSendData.SendTarget.toMap, withBlock.Pos.Map,
                                    Protocol.PrepareMessagePlayWave(Convert.ToByte(obj.Snd1),
                                        Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));
                            }

                            return;
                        }

                        Protocol.WriteConsoleMsg(UserIndex,
                            "Sólo miembros de la legión oscura pueden usar este cuerno.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }

                    // Si llega aca es porque es o Laud o Tambor o Flauta
                    // Los admin invisibles solo producen sonidos a si mismos
                    if (withBlock.flags.AdminInvisible == 1)
                    {
                        var argdatos8 = Protocol.PrepareMessagePlayWave(Convert.ToByte(obj.Snd1),
                            Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y));
                        TCP.EnviarDatosASlot(UserIndex, ref argdatos8);
                    }
                    else
                    {
                        modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex,
                            Protocol.PrepareMessagePlayWave(Convert.ToByte(obj.Snd1),
                                Convert.ToByte(withBlock.Pos.X), Convert.ToByte(withBlock.Pos.Y)));
                    }

                    break;
                }

                case Declaraciones.eOBJType.otBarcos:
                {
                    // Verifica si esta aproximado al agua antes de permitirle navegar
                    if (withBlock.Stats.ELV < 25)
                    {
                        if ((withBlock.clase != Declaraciones.eClass.Worker) &
                            (withBlock.clase != Declaraciones.eClass.Pirat))
                        {
                            Protocol.WriteConsoleMsg(UserIndex,
                                "Para recorrer los mares debes ser nivel 25 o superior.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            return;
                        }

                        if (withBlock.Stats.ELV < 20)
                        {
                            Protocol.WriteConsoleMsg(UserIndex,
                                "Para recorrer los mares debes ser nivel 20 o superior.",
                                Protocol.FontTypeNames.FONTTYPE_INFO);
                            return;
                        }
                    }

                    if (((Extra.LegalPos(withBlock.Pos.Map, Convert.ToInt16(withBlock.Pos.X - 1), withBlock.Pos.Y, true,
                              false) |
                          Extra.LegalPos(withBlock.Pos.Map, withBlock.Pos.X, Convert.ToInt16(withBlock.Pos.Y - 1), true,
                              false) |
                          Extra.LegalPos(withBlock.Pos.Map, Convert.ToInt16(withBlock.Pos.X + 1), withBlock.Pos.Y, true,
                              false) | Extra.LegalPos(withBlock.Pos.Map, withBlock.Pos.X,
                              Convert.ToInt16(withBlock.Pos.Y + 1), true, false)) & (withBlock.flags.Navegando == 0)) |
                        (withBlock.flags.Navegando == 1))
                        Trabajo.DoNavega(UserIndex, ref obj, Slot);
                    else
                        Protocol.WriteConsoleMsg(UserIndex, "¡Debes aproximarte al agua para usar el barco!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);

                    break;
                }
            }
        }
    }

    public static void EnivarArmasConstruibles(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        Protocol.WriteBlacksmithWeapons(UserIndex);
    }

    public static void EnivarObjConstruibles(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        Protocol.WriteCarpenterObjects(UserIndex);
    }

    public static void EnivarArmadurasConstruibles(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        Protocol.WriteBlacksmithArmors(UserIndex);
    }

    public static void TirarTodo(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            int Cantidad;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                if ((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger == 6)
                    return;

                TirarTodosLosItems(UserIndex);

                Cantidad = withBlock.Stats.GLD - Convert.ToInt32(withBlock.Stats.ELV) * 10000;

                if (Cantidad > 0)
                    TirarOro(Cantidad, UserIndex);
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in ClasePuedeUsarItem: " + ex.Message);
        }
    }

    public static bool ItemSeCae(short index)
    {
        bool ItemSeCaeRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref Declaraciones.objData[index];
            ItemSeCaeRet = ((withBlock.Real != 1) | (withBlock.NoSeCae == 0)) &
                           ((withBlock.Caos != 1) | (withBlock.NoSeCae == 0)) &
                           (withBlock.OBJType != Declaraciones.eOBJType.otLlaves) &
                           (withBlock.OBJType != Declaraciones.eOBJType.otBarcos) & (withBlock.NoSeCae == 0);
        }

        return ItemSeCaeRet;
    }

    public static void TirarTodosLosItems(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 12/01/2010 (ZaMa)
        // 12/01/2010: ZaMa - Ahora los piratas no explotan items solo si estan entre 20 y 25
        // ***************************************************

        byte i;
        var NuevaPos = default(Declaraciones.WorldPos);
        Declaraciones.Obj MiObj;
        short ItemIndex;
        bool DropAgua;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            var loopTo = withBlock.CurrentInventorySlots;
            for (i = 1; i <= loopTo; i++)
            {
                ItemIndex = withBlock.Invent.userObj[i].ObjIndex;
                if (ItemIndex > 0)
                    if (ItemSeCae(ItemIndex))
                    {
                        NuevaPos.X = 0;
                        NuevaPos.Y = 0;

                        // Creo el Obj
                        MiObj.Amount = withBlock.Invent.userObj[i].Amount;
                        MiObj.ObjIndex = ItemIndex;

                        DropAgua = true;
                        // Es pirata?
                        if (withBlock.clase == Declaraciones.eClass.Pirat)
                            // Si tiene galeon equipado
                            if (withBlock.Invent.BarcoObjIndex == 476)
                                // Limitación por nivel, después dropea normalmente
                                if ((withBlock.Stats.ELV >= 20) & (withBlock.Stats.ELV <= 25))
                                    // No dropea en agua
                                    DropAgua = false;

                        var argTierra = true;
                        UsUaRiOs.Tilelibre(ref withBlock.Pos, ref NuevaPos, ref MiObj, ref DropAgua, ref argTierra);

                        if ((NuevaPos.X != 0) & (NuevaPos.Y != 0))
                            DropObj(UserIndex, i, Declaraciones.MAX_INVENTORY_OBJS, NuevaPos.Map, NuevaPos.X,
                                NuevaPos.Y);
                    }
            }
        }
    }

    public static bool ItemNewbie(short ItemIndex)
    {
        bool ItemNewbieRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if ((ItemIndex < 1) | (ItemIndex > Declaraciones.objData.Length - 1))
            return ItemNewbieRet;

        ItemNewbieRet = Declaraciones.objData[ItemIndex].Newbie == 1;
        return ItemNewbieRet;
    }

    public static void TirarTodosLosItemsNoNewbies(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 23/11/2009
        // 07/11/09: Pato - Fix bug #2819911
        // 23/11/2009: ZaMa - Optimizacion de codigo.
        // ***************************************************
        byte i;
        var NuevaPos = default(Declaraciones.WorldPos);
        Declaraciones.Obj MiObj;
        short ItemIndex;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if ((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger == 6)
                return;

            var loopTo = Declaraciones.UserList[UserIndex].CurrentInventorySlots;
            for (i = 1; i <= loopTo; i++)
            {
                ItemIndex = withBlock.Invent.userObj[i].ObjIndex;
                if (ItemIndex > 0)
                    if (ItemSeCae(ItemIndex) & !ItemNewbie(ItemIndex))
                    {
                        NuevaPos.X = 0;
                        NuevaPos.Y = 0;

                        // Creo MiObj
                        MiObj.Amount = withBlock.Invent.userObj[i].Amount;
                        MiObj.ObjIndex = ItemIndex;
                        // Pablo (ToxicWaste) 24/01/2007
                        // Tira los Items no newbies en todos lados.
                        var argAgua = true;
                        var argTierra = true;
                        UsUaRiOs.Tilelibre(ref withBlock.Pos, ref NuevaPos, ref MiObj, ref argAgua, ref argTierra);
                        if ((NuevaPos.X != 0) & (NuevaPos.Y != 0))
                            DropObj(UserIndex, i, Declaraciones.MAX_INVENTORY_OBJS, NuevaPos.Map, NuevaPos.X,
                                NuevaPos.Y);
                    }
            }
        }
    }

    public static void TirarTodosLosItemsEnMochila(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: 12/01/09 (Budi)
        // ***************************************************
        byte i;
        var NuevaPos = default(Declaraciones.WorldPos);
        Declaraciones.Obj MiObj;
        short ItemIndex;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if ((int)Declaraciones.MapData[withBlock.Pos.Map, withBlock.Pos.X, withBlock.Pos.Y].trigger == 6)
                return;

            var loopTo = withBlock.CurrentInventorySlots;
            for (i = Declaraciones.MAX_NORMAL_INVENTORY_SLOTS + 1; i <= loopTo; i++)
            {
                ItemIndex = withBlock.Invent.userObj[i].ObjIndex;
                if (ItemIndex > 0)
                    if (ItemSeCae(ItemIndex))
                    {
                        NuevaPos.X = 0;
                        NuevaPos.Y = 0;

                        // Creo MiObj
                        MiObj.Amount = withBlock.Invent.userObj[i].Amount;
                        MiObj.ObjIndex = ItemIndex;
                        var argAgua = true;
                        var argTierra = true;
                        UsUaRiOs.Tilelibre(ref withBlock.Pos, ref NuevaPos, ref MiObj, ref argAgua, ref argTierra);
                        if ((NuevaPos.X != 0) & (NuevaPos.Y != 0))
                            DropObj(UserIndex, i, Declaraciones.MAX_INVENTORY_OBJS, NuevaPos.Map, NuevaPos.X,
                                NuevaPos.Y);
                    }
            }
        }
    }

    public static Declaraciones.eOBJType getObjType(short ObjIndex)
    {
        Declaraciones.eOBJType getObjTypeRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        if (ObjIndex > 0) getObjTypeRet = Declaraciones.objData[ObjIndex].OBJType;

        return getObjTypeRet;
    }
}