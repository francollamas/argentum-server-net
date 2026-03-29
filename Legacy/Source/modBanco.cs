using System;
using System.IO;
using Microsoft.VisualBasic;

namespace Legacy;

internal static class modBanco
{
    public static void IniciarDeposito(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            // Hacemos un Update del inventario del usuario
            UpdateBanUserInv(true, UserIndex, 0);
            // Actualizamos el dinero
            Protocol.WriteUpdateUserStats(UserIndex);
            // Mostramos la ventana pa' comerciar y ver ladear la osamenta. jajaja
            Protocol.WriteBankInit(UserIndex);
            Declaraciones.UserList[UserIndex].flags.Comerciando = true;
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in IniciarDeposito: " + ex.Message);
        }
    }

    // UPGRADE_NOTE: Object se actualizó a Object_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    public static void SendBanObj(ref short UserIndex, ref byte Slot, ref Declaraciones.UserOBJ Object_Renamed)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().BancoInvent.Object(Slot). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        Declaraciones.UserList[UserIndex].BancoInvent.Object_Renamed[Slot] = Object_Renamed;

        Protocol.WriteChangeBankSlot(UserIndex, Slot);
    }

    public static void UpdateBanUserInv(bool UpdateAll, short UserIndex, byte Slot)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        var NullObj = default(Declaraciones.UserOBJ);
        byte LoopC;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Actualiza un solo slot
            if (!UpdateAll)
            {
                // Actualiza el inventario
                if (withBlock.BancoInvent.Object_Renamed[Slot].ObjIndex > 0)
                    SendBanObj(ref UserIndex, ref Slot, ref withBlock.BancoInvent.Object_Renamed[Slot]);
                else
                    SendBanObj(ref UserIndex, ref Slot, ref NullObj);
            }
            else
            {
                // Actualiza todos los slots
                for (LoopC = 1; LoopC <= Declaraciones.MAX_BANCOINVENTORY_SLOTS; LoopC++)
                    // Actualiza el inventario
                    if (withBlock.BancoInvent.Object_Renamed[LoopC].ObjIndex > 0)
                        SendBanObj(ref UserIndex, ref LoopC, ref withBlock.BancoInvent.Object_Renamed[LoopC]);
                    else
                        SendBanObj(ref UserIndex, ref LoopC, ref NullObj);
            }
        }
    }

    public static void UserRetiraItem(short UserIndex, short i, short Cantidad)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            if (Cantidad < 1)
                return;

            Protocol.WriteUpdateUserStats(UserIndex);

            if (Declaraciones.UserList[UserIndex].BancoInvent.Object_Renamed[i].Amount > 0)
            {
                if (Cantidad > Declaraciones.UserList[UserIndex].BancoInvent.Object_Renamed[i].Amount)
                    Cantidad = Declaraciones.UserList[UserIndex].BancoInvent.Object_Renamed[i].Amount;
                // Agregamos el obj que compro al inventario
                UserReciveObj(UserIndex, Convert.ToInt16(i), Cantidad);
                // Actualizamos el inventario del usuario
                InvUsuario.UpdateUserInv(true, UserIndex, 0);
                // Actualizamos el banco
                UpdateBanUserInv(true, UserIndex, 0);
            }

            // Actualizamos la ventana de comercio
            UpdateVentanaBanco(UserIndex);
        }


        catch (Exception ex)
        {
            Console.WriteLine("Error in SendBanObj: " + ex.Message);
        }
    }

    public static void UserReciveObj(short UserIndex, short ObjIndex, short Cantidad)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short Slot;
        short obji;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.BancoInvent.Object_Renamed[ObjIndex].Amount <= 0)
                return;

            obji = withBlock.BancoInvent.Object_Renamed[ObjIndex].ObjIndex;


            // ¿Ya tiene un objeto de este tipo?
            Slot = 1;
            while (!((withBlock.Invent.Object_Renamed[Slot].ObjIndex == obji) &
                     ((short)(withBlock.Invent.Object_Renamed[Slot].Amount + Cantidad) <=
                      Declaraciones.MAX_INVENTORY_OBJS)))

            {
                Slot = Convert.ToInt16(Slot + 1);
                if (Slot > withBlock.CurrentInventorySlots) break;
            }

            // Sino se fija por un slot vacio
            if (Slot > withBlock.CurrentInventorySlots)
            {
                Slot = 1;
                while (withBlock.Invent.Object_Renamed[Slot].ObjIndex != 0)
                {
                    Slot = Convert.ToInt16(Slot + 1);

                    if (Slot > withBlock.CurrentInventorySlots)
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "No podés tener mas objetos.",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }
                }

                withBlock.Invent.NroItems = Convert.ToInt16(withBlock.Invent.NroItems + 1);
            }


            // Mete el obj en el slot
            if ((short)(withBlock.Invent.Object_Renamed[Slot].Amount + Cantidad) <= Declaraciones.MAX_INVENTORY_OBJS)
            {
                // Menor que MAX_INV_OBJS
                withBlock.Invent.Object_Renamed[Slot].ObjIndex = obji;
                withBlock.Invent.Object_Renamed[Slot].Amount =
                    (short)(withBlock.Invent.Object_Renamed[Slot].Amount + Cantidad);

                QuitarBancoInvItem(UserIndex, Convert.ToByte(ObjIndex), Cantidad);
            }
            else
            {
                Protocol.WriteConsoleMsg(UserIndex, "No podés tener mas objetos.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }
        }
    }

    public static void QuitarBancoInvItem(short UserIndex, byte Slot, short Cantidad)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short ObjIndex;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            ObjIndex = withBlock.BancoInvent.Object_Renamed[Slot].ObjIndex;

            // Quita un Obj

            withBlock.BancoInvent.Object_Renamed[Slot].Amount =
                (short)(withBlock.BancoInvent.Object_Renamed[Slot].Amount - Cantidad);

            if (withBlock.BancoInvent.Object_Renamed[Slot].Amount <= 0)
            {
                withBlock.BancoInvent.NroItems = Convert.ToInt16(withBlock.BancoInvent.NroItems - 1);
                withBlock.BancoInvent.Object_Renamed[Slot].ObjIndex = 0;
                withBlock.BancoInvent.Object_Renamed[Slot].Amount = 0;
            }
        }
    }

    public static void UpdateVentanaBanco(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        Protocol.WriteBankOK(UserIndex);
    }

    public static void UserDepositaItem(short UserIndex, short Item, short Cantidad)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            if ((Declaraciones.UserList[UserIndex].Invent.Object_Renamed[Item].Amount > 0) & (Cantidad > 0))
            {
                if (Cantidad > Declaraciones.UserList[UserIndex].Invent.Object_Renamed[Item].Amount)
                    Cantidad = Declaraciones.UserList[UserIndex].Invent.Object_Renamed[Item].Amount;

                // Agregamos el obj que deposita al banco
                UserDejaObj(UserIndex, Convert.ToInt16(Item), Cantidad);

                // Actualizamos el inventario del usuario
                InvUsuario.UpdateUserInv(true, UserIndex, 0);

                // Actualizamos el inventario del banco
                UpdateBanUserInv(true, UserIndex, 0);
            }

            // Actualizamos la ventana del banco
            UpdateVentanaBanco(UserIndex);
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in UserReciveObj: " + ex.Message);
        }
    }

    public static void UserDejaObj(short UserIndex, short ObjIndex, short Cantidad)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short Slot;
        short obji;

        if (Cantidad < 1)
            return;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            obji = withBlock.Invent.Object_Renamed[ObjIndex].ObjIndex;

            // ¿Ya tiene un objeto de este tipo?
            Slot = 1;
            while (!((withBlock.BancoInvent.Object_Renamed[Slot].ObjIndex == obji) &
                     ((short)(withBlock.BancoInvent.Object_Renamed[Slot].Amount + Cantidad) <=
                      Declaraciones.MAX_INVENTORY_OBJS)))

            {
                Slot = Convert.ToInt16(Slot + 1);

                if (Slot > Declaraciones.MAX_BANCOINVENTORY_SLOTS) break;
            }

            // Sino se fija por un slot vacio antes del slot devuelto
            if (Slot > Declaraciones.MAX_BANCOINVENTORY_SLOTS)
            {
                Slot = 1;
                while (withBlock.BancoInvent.Object_Renamed[Slot].ObjIndex != 0)
                {
                    Slot = Convert.ToInt16(Slot + 1);

                    if (Slot > Declaraciones.MAX_BANCOINVENTORY_SLOTS)
                    {
                        Protocol.WriteConsoleMsg(UserIndex, "No tienes mas espacio en el banco!!",
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                        return;
                    }
                }

                withBlock.BancoInvent.NroItems = Convert.ToInt16(withBlock.BancoInvent.NroItems + 1);
            }

            if (Slot <= Declaraciones.MAX_BANCOINVENTORY_SLOTS) // Slot valido
            {
                // Mete el obj en el slot
                if ((short)(withBlock.BancoInvent.Object_Renamed[Slot].Amount + Cantidad) <=
                    Declaraciones.MAX_INVENTORY_OBJS)
                {
                    // Menor que MAX_INV_OBJS
                    withBlock.BancoInvent.Object_Renamed[Slot].ObjIndex = obji;
                    withBlock.BancoInvent.Object_Renamed[Slot].Amount =
                        (short)(withBlock.BancoInvent.Object_Renamed[Slot].Amount + Cantidad);

                    InvUsuario.QuitarUserInvItem(UserIndex, Convert.ToByte(ObjIndex), Cantidad);
                }
                else
                {
                    Protocol.WriteConsoleMsg(UserIndex, "El banco no puede cargar tantos objetos.",
                        Protocol.FontTypeNames.FONTTYPE_INFO);
                }
            }
        }
    }

    public static void SendUserBovedaTxt(short sendIndex, short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            short j;

            Protocol.WriteConsoleMsg(sendIndex, Declaraciones.UserList[UserIndex].name,
                Protocol.FontTypeNames.FONTTYPE_INFO);
            Protocol.WriteConsoleMsg(sendIndex,
                "Tiene " + Declaraciones.UserList[UserIndex].BancoInvent.NroItems + " objetos.",
                Protocol.FontTypeNames.FONTTYPE_INFO);

            for (j = 1; j <= Declaraciones.MAX_BANCOINVENTORY_SLOTS; j++)
                if (Declaraciones.UserList[UserIndex].BancoInvent.Object_Renamed[j].ObjIndex > 0)
                    Protocol.WriteConsoleMsg(sendIndex,
                        "Objeto " + j + " " +
                        Declaraciones
                            .ObjData_Renamed[Declaraciones.UserList[UserIndex].BancoInvent.Object_Renamed[j].ObjIndex]
                            .name + " Cantidad:" +
                        Declaraciones.UserList[UserIndex].BancoInvent.Object_Renamed[j].Amount,
                        Protocol.FontTypeNames.FONTTYPE_INFO);
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in IniciarDeposito: " + ex.Message);
        }
    }

    public static void SendUserBovedaTxtFromChar(short sendIndex, string charName)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        try
        {
            short j;
            string CharFile, Tmp;
            int ObjInd, ObjCant;

            CharFile = Declaraciones.CharPath + charName + ".chr";

            if (File.Exists(CharFile))
            {
                Protocol.WriteConsoleMsg(sendIndex, charName, Protocol.FontTypeNames.FONTTYPE_INFO);
                var argEmptySpaces = 1024;
                Protocol.WriteConsoleMsg(sendIndex,
                    "Tiene " + ES.GetVar(CharFile, "BancoInventory", "CantidadItems", ref argEmptySpaces) + " objetos.",
                    Protocol.FontTypeNames.FONTTYPE_INFO);
                for (j = 1; j <= Declaraciones.MAX_BANCOINVENTORY_SLOTS; j++)
                {
                    var argEmptySpaces1 = 1024;
                    Tmp = ES.GetVar(CharFile, "BancoInventory", "Obj" + j, ref argEmptySpaces1);
                    ObjInd = Convert.ToInt32(General.ReadField(1, ref Tmp, Convert.ToByte(Strings.Asc("-"))));
                    ObjCant = Convert.ToInt32(General.ReadField(2, ref Tmp, Convert.ToByte(Strings.Asc("-"))));
                    if (ObjInd > 0)
                        Protocol.WriteConsoleMsg(sendIndex,
                            "Objeto " + j + " " + Declaraciones.ObjData_Renamed[ObjInd].name + " Cantidad:" + ObjCant,
                            Protocol.FontTypeNames.FONTTYPE_INFO);
                }
            }
            else
            {
                Protocol.WriteConsoleMsg(sendIndex, "Usuario inexistente: " + charName,
                    Protocol.FontTypeNames.FONTTYPE_INFO);
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine("Error in SendUserBovedaTxtFromChar: " + ex.Message);
        }
    }
}