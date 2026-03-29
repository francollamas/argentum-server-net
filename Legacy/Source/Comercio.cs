using System;
using Microsoft.VisualBasic;

namespace Legacy;

internal static class modSistemaComercio
{
    public enum eModoComercio
    {
        Compra = 1,
        Venta = 2
    }

    public const byte REDUCTOR_PRECIOVENTA = 3;

    // '
    // Makes a trade. (Buy or Sell)
    // 
    // @param Modo The trade type (sell or buy)
    // @param UserIndex Specifies the index of the user
    // @param NpcIndex specifies the index of the Npc
    // @param Slot Specifies which slot are you trying to sell / buy
    // @param Cantidad Specifies how many items in that slot are you trying to sell / buy
    public static void Comercio(eModoComercio Modo, short UserIndex, short NpcIndex, short Slot, short Cantidad)
    {
        // *************************************************
        // Author: Nacho (Integer)
        // Last modified: 27/07/08 (MarKoxX) | New changes in the way of trading (now when you buy it rounds to ceil and when you sell it rounds to floor)
        // - 06/13/08 (NicoNZ)
        // *************************************************
        int Precio;
        Declaraciones.Obj Objeto;

        if ((Cantidad < 1) | (Slot < 1))
            return;

        short NpcSlot;
        if (Modo == eModoComercio.Compra)
        {
            if (Slot > Declaraciones.MAX_INVENTORY_SLOTS) return;

            if (Cantidad > Declaraciones.MAX_INVENTORY_OBJS)
            {
                modSendData.SendData(modSendData.SendTarget.ToAll, 0,
                    Protocol.PrepareMessageConsoleMsg(
                        Declaraciones.UserList[UserIndex].name + " ha sido baneado por el sistema anti-cheats.",
                        Protocol.FontTypeNames.FONTTYPE_FIGHT));
                ES.Ban(Declaraciones.UserList[UserIndex].name, "Sistema Anti Cheats",
                    "Intentar hackear el sistema de comercio. Quiso comprar demasiados ítems:" + Cantidad);
                Declaraciones.UserList[UserIndex].flags.Ban = 1;
                Protocol.WriteErrorMsg(UserIndex, "Has sido baneado por el Sistema AntiCheat.");
                Protocol.FlushBuffer(UserIndex);
                TCP.CloseSocket(UserIndex);
                return;
            }

            if (!(Declaraciones.Npclist[NpcIndex].Invent.userObj[Slot].Amount > 0)) return;

            if (Cantidad > Declaraciones.Npclist[NpcIndex].Invent.userObj[Slot].Amount)
                Cantidad = Declaraciones.Npclist[Declaraciones.UserList[UserIndex].flags.TargetNPC].Invent
                    .userObj[Slot].Amount;

            Objeto.Amount = Cantidad;
            Objeto.ObjIndex = Declaraciones.Npclist[NpcIndex].Invent.userObj[Slot].ObjIndex;

            // El precio, cuando nos venden algo, lo tenemos que redondear para arriba.
            // Es decir, 1.1 = 2, por lo cual se hace de la siguiente forma Precio = Clng(PrecioFinal + 0.5) Siempre va a darte el proximo numero. O el "Techo" (MarKoxX)

            Precio = Convert.ToInt32(
                Declaraciones.objData[Declaraciones.Npclist[NpcIndex].Invent.userObj[Slot].ObjIndex]
                    .Valor / Descuento(UserIndex) * Cantidad + 0.5d);

            if (Declaraciones.UserList[UserIndex].Stats.GLD < Precio)
            {
                Protocol.WriteConsoleMsg(UserIndex, "No tienes suficiente dinero.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                return;
            }


            if (!InvUsuario.MeterItemEnInventario(UserIndex, ref Objeto))
            {
                // Call WriteConsoleMsg(UserIndex, "No puedes cargar mas objetos.", FontTypeNames.FONTTYPE_INFO)
                EnviarNpcInv(UserIndex, Declaraciones.UserList[UserIndex].flags.TargetNPC);
                Protocol.WriteTradeOK(UserIndex);
                return;
            }

            Declaraciones.UserList[UserIndex].Stats.GLD = Declaraciones.UserList[UserIndex].Stats.GLD - Precio;

            InvNpc.QuitarNpcInvItem(Declaraciones.UserList[UserIndex].flags.TargetNPC, Convert.ToByte(Slot), Cantidad);

            // Bien, ahora logueo de ser necesario. Pablo (ToxicWaste) 07/09/07
            // Es un Objeto que tenemos que loguear?
            if (Declaraciones.objData[Objeto.ObjIndex].Log == 1)
                General.LogDesarrollo(Declaraciones.UserList[UserIndex].name + " compró del NPC " + Objeto.Amount +
                                      " " + Declaraciones.objData[Objeto.ObjIndex].name);
            else if (Objeto.Amount == 1000) // Es mucha cantidad?
                // Si no es de los prohibidos de loguear, lo logueamos.
                if (Declaraciones.objData[Objeto.ObjIndex].NoLog != 1)
                    General.LogDesarrollo(Declaraciones.UserList[UserIndex].name + " compró del NPC " + Objeto.Amount +
                                          " " + Declaraciones.objData[Objeto.ObjIndex].name);

            // Agregado para que no se vuelvan a vender las llaves si se recargan los .dat.
            if (Declaraciones.objData[Objeto.ObjIndex].OBJType == Declaraciones.eOBJType.otLlaves)
            {
                ES.WriteVar(Declaraciones.DatPath + "NPCs.dat", "NPC" + Declaraciones.Npclist[NpcIndex].Numero,
                    "obj" + Slot, Objeto.ObjIndex + "-0");
                General.logVentaCasa(Declaraciones.UserList[UserIndex].name + " compró " +
                                     Declaraciones.objData[Objeto.ObjIndex].name);
            }
        }

        else if (Modo == eModoComercio.Venta)
        {
            if (Cantidad > Declaraciones.UserList[UserIndex].Invent.userObj[Slot].Amount)
                Cantidad = Declaraciones.UserList[UserIndex].Invent.userObj[Slot].Amount;

            Objeto.Amount = Cantidad;
            Objeto.ObjIndex = Declaraciones.UserList[UserIndex].Invent.userObj[Slot].ObjIndex;

            if (Objeto.ObjIndex == 0) return;

            if (((Declaraciones.Npclist[NpcIndex].TipoItems !=
                  (int)Declaraciones.objData[Objeto.ObjIndex].OBJType) &
                 (Declaraciones.Npclist[NpcIndex].TipoItems != (int)Declaraciones.eOBJType.otCualquiera)) |
                (Objeto.ObjIndex == Declaraciones.iORO))
            {
                Protocol.WriteConsoleMsg(UserIndex, "Lo siento, no estoy interesado en este tipo de objetos.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                EnviarNpcInv(UserIndex, Declaraciones.UserList[UserIndex].flags.TargetNPC);
                Protocol.WriteTradeOK(UserIndex);
                return;
            }

            if (Declaraciones.objData[Objeto.ObjIndex].Real == 1)
            {
                if (Declaraciones.Npclist[NpcIndex].name != "SR")
                {
                    Protocol.WriteConsoleMsg(UserIndex,
                        "Las armaduras del ejército real sólo pueden ser vendidas a los sastres reales.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    EnviarNpcInv(UserIndex, Declaraciones.UserList[UserIndex].flags.TargetNPC);
                    Protocol.WriteTradeOK(UserIndex);
                    return;
                }
            }
            else if (Declaraciones.objData[Objeto.ObjIndex].Caos == 1)
            {
                if (Declaraciones.Npclist[NpcIndex].name != "SC")
                {
                    Protocol.WriteConsoleMsg(UserIndex,
                        "Las armaduras de la legión oscura sólo pueden ser vendidas a los sastres del demonio.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                    EnviarNpcInv(UserIndex, Declaraciones.UserList[UserIndex].flags.TargetNPC);
                    Protocol.WriteTradeOK(UserIndex);
                    return;
                }
            }
            else if ((Declaraciones.UserList[UserIndex].Invent.userObj[Slot].Amount < 0) | (Cantidad == 0))
            {
                return;
            }
            else if ((Slot < 0) | (Slot > Declaraciones.UserList[UserIndex].Invent.userObj.Length - 1))
            {
                EnviarNpcInv(UserIndex, Declaraciones.UserList[UserIndex].flags.TargetNPC);
                return;
            }
            else if ((Declaraciones.UserList[UserIndex].flags.Privilegios & Declaraciones.PlayerType.Consejero) != 0)
            {
                Protocol.WriteConsoleMsg(UserIndex, "No puedes vender ítems.", Protocol.FontTypeNames.FONTTYPE_WARNING);
                EnviarNpcInv(UserIndex, Declaraciones.UserList[UserIndex].flags.TargetNPC);
                Protocol.WriteTradeOK(UserIndex);
                return;
            }

            InvUsuario.QuitarUserInvItem(UserIndex, Convert.ToByte(Slot), Cantidad);

            // Precio = Round(ObjData(Objeto.ObjIndex).valor / REDUCTOR_PRECIOVENTA * Cantidad, 0)
            Precio = Convert.ToInt32(Conversion.Fix(SalePrice(Objeto.ObjIndex) * Cantidad));
            Declaraciones.UserList[UserIndex].Stats.GLD = Declaraciones.UserList[UserIndex].Stats.GLD + Precio;

            if (Declaraciones.UserList[UserIndex].Stats.GLD > Declaraciones.MAXORO)
                Declaraciones.UserList[UserIndex].Stats.GLD = Declaraciones.MAXORO;

            NpcSlot = SlotEnNPCInv(NpcIndex, Objeto.ObjIndex, Objeto.Amount);

            if (NpcSlot <= Declaraciones.MAX_INVENTORY_SLOTS) // Slot valido
            {
                // Mete el obj en el slot
                Declaraciones.Npclist[NpcIndex].Invent.userObj[NpcSlot].ObjIndex = Objeto.ObjIndex;
                Declaraciones.Npclist[NpcIndex].Invent.userObj[NpcSlot].Amount =
                    (short)(Declaraciones.Npclist[NpcIndex].Invent.userObj[NpcSlot].Amount + Objeto.Amount);
                if (Declaraciones.Npclist[NpcIndex].Invent.userObj[NpcSlot].Amount >
                    Declaraciones.MAX_INVENTORY_OBJS)
                    Declaraciones.Npclist[NpcIndex].Invent.userObj[NpcSlot].Amount =
                        Declaraciones.MAX_INVENTORY_OBJS;
            }

            // Bien, ahora logueo de ser necesario. Pablo (ToxicWaste) 07/09/07
            // Es un Objeto que tenemos que loguear?
            if (Declaraciones.objData[Objeto.ObjIndex].Log == 1)
                General.LogDesarrollo(Declaraciones.UserList[UserIndex].name + " vendió al NPC " + Objeto.Amount + " " +
                                      Declaraciones.objData[Objeto.ObjIndex].name);
            else if (Objeto.Amount == 1000) // Es mucha cantidad?
                // Si no es de los prohibidos de loguear, lo logueamos.
                if (Declaraciones.objData[Objeto.ObjIndex].NoLog != 1)
                    General.LogDesarrollo(Declaraciones.UserList[UserIndex].name + " vendió al NPC " + Objeto.Amount +
                                          " " + Declaraciones.objData[Objeto.ObjIndex].name);
        }

        InvUsuario.UpdateUserInv(true, UserIndex, 0);
        Protocol.WriteUpdateUserStats(UserIndex);
        EnviarNpcInv(UserIndex, Declaraciones.UserList[UserIndex].flags.TargetNPC);
        Protocol.WriteTradeOK(UserIndex);

        UsUaRiOs.SubirSkill(UserIndex, (short)Declaraciones.eSkill.Comerciar, true);
    }

    public static void IniciarComercioNPC(short UserIndex)
    {
        // *************************************************
        // Author: Nacho (Integer)
        // Last modified: 2/8/06
        // *************************************************
        EnviarNpcInv(UserIndex, Declaraciones.UserList[UserIndex].flags.TargetNPC);
        Declaraciones.UserList[UserIndex].flags.Comerciando = true;
        Protocol.WriteCommerceInit(UserIndex);
    }

    private static short SlotEnNPCInv(short NpcIndex, short Objeto, short Cantidad)
    {
        short SlotEnNPCInvRet = default;
        // *************************************************
        // Author: Nacho (Integer)
        // Last modified: 2/8/06
        // *************************************************
        SlotEnNPCInvRet = 1;
        while (!((Declaraciones.Npclist[NpcIndex].Invent.userObj[SlotEnNPCInvRet].ObjIndex == Objeto) &
                 ((short)(Declaraciones.Npclist[NpcIndex].Invent.userObj[SlotEnNPCInvRet].Amount + Cantidad) <=
                  Declaraciones.MAX_INVENTORY_OBJS)))

        {
            SlotEnNPCInvRet = Convert.ToInt16(SlotEnNPCInvRet + 1);
            if (SlotEnNPCInvRet > Declaraciones.MAX_INVENTORY_SLOTS)
                break;
        }

        if (SlotEnNPCInvRet > Declaraciones.MAX_INVENTORY_SLOTS)
        {
            SlotEnNPCInvRet = 1;

            while (Declaraciones.Npclist[NpcIndex].Invent.userObj[SlotEnNPCInvRet].ObjIndex != 0)
            {
                SlotEnNPCInvRet = Convert.ToInt16(SlotEnNPCInvRet + 1);
                if (SlotEnNPCInvRet > Declaraciones.MAX_INVENTORY_SLOTS)
                    break;
            }

            if (SlotEnNPCInvRet <= Declaraciones.MAX_INVENTORY_SLOTS)
                Declaraciones.Npclist[NpcIndex].Invent.NroItems =
                    Convert.ToInt16(Declaraciones.Npclist[NpcIndex].Invent.NroItems + 1);
        }

        return SlotEnNPCInvRet;
    }

    private static float Descuento(short UserIndex)
    {
        float DescuentoRet = default;
        // *************************************************
        // Author: Nacho (Integer)
        // Last modified: 2/8/06
        // *************************************************
        DescuentoRet = (float)(1d +
                               Declaraciones.UserList[UserIndex].Stats.UserSkills[(int)Declaraciones.eSkill.Comerciar] /
                               100d);
        return DescuentoRet;
    }

    // '
    // Send the inventory of the Npc to the user
    // 
    // @param userIndex The index of the User
    // @param npcIndex The index of the NPC

    private static void EnviarNpcInv(short UserIndex, short NpcIndex)
    {
        // *************************************************
        // Author: Nacho (Integer)
        // Last Modified: 06/14/08
        // Last Modified By: Nicolás Ezequiel Bouhid (NicoNZ)
        // *************************************************
        byte Slot;
        float val;

        Declaraciones.Obj thisObj;
        var DummyObj = default(Declaraciones.Obj);
        for (Slot = 1; Slot <= Declaraciones.MAX_NORMAL_INVENTORY_SLOTS; Slot++)
            if (Declaraciones.Npclist[NpcIndex].Invent.userObj[Slot].ObjIndex > 0)
            {
                thisObj.ObjIndex = Declaraciones.Npclist[NpcIndex].Invent.userObj[Slot].ObjIndex;
                thisObj.Amount = Declaraciones.Npclist[NpcIndex].Invent.userObj[Slot].Amount;

                val = Declaraciones.objData[thisObj.ObjIndex].Valor / Descuento(UserIndex);

                Protocol.WriteChangeNPCInventorySlot(UserIndex, Slot, thisObj, val);
            }
            else
            {
                Protocol.WriteChangeNPCInventorySlot(UserIndex, Slot, DummyObj, 0f);
            }
    }

    // '
    // Devuelve el valor de venta del objeto
    // 
    // @param ObjIndex  El número de objeto al cual le calculamos el precio de venta

    public static float SalePrice(short ObjIndex)
    {
        float SalePriceRet = default;
        // *************************************************
        // Author: Nicolás (NicoNZ)
        // 
        // *************************************************
        if ((ObjIndex < 1) | (ObjIndex > Declaraciones.objData.Length - 1))
            return SalePriceRet;
        if (InvUsuario.ItemNewbie(ObjIndex))
            return SalePriceRet;

        SalePriceRet = (float)(Declaraciones.objData[ObjIndex].Valor / (double)REDUCTOR_PRECIOVENTA);
        return SalePriceRet;
    }
}