using System;
using System.Runtime.InteropServices;

namespace Legacy
{
    static class InvNpc
    {
        // Modulo Inv & Obj
        // Modulo para controlar los objetos y los inventarios.
        public static Declaraciones.WorldPos TirarItemAlPiso(ref Declaraciones.WorldPos Pos, ref Declaraciones.Obj Obj, [Optional, DefaultParameterValue(true)] ref bool NotPirata)
        {
            Declaraciones.WorldPos TirarItemAlPisoRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            try
            {

                var NuevaPos = default(Declaraciones.WorldPos);
                NuevaPos.X = 0;
                NuevaPos.Y = 0;

                bool argTierra = true;
                UsUaRiOs.Tilelibre(ref Pos, ref NuevaPos, ref Obj, ref NotPirata, ref argTierra);
                if (NuevaPos.X != 0 & NuevaPos.Y != 0)
                {
                    InvUsuario.MakeObj(ref Obj, Pos.Map, NuevaPos.X, NuevaPos.Y);
                }
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto TirarItemAlPiso. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                TirarItemAlPisoRet = NuevaPos;
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in TirarItemAlPiso: " + ex.Message);

            }

            return TirarItemAlPisoRet;
        }

        public static void NPC_TIRAR_ITEMS(ref Declaraciones.npc npc, bool IsPretoriano)
        {
            // ***************************************************
            // Autor: Unknown (orginal version)
            // Last Modification: 28/11/2009
            // Give away npc's items.
            // 28/11/2009: ZaMa - Implementado drops complejos
            // 02/04/2010: ZaMa - Los pretos vuelven a tirar oro.
            // ***************************************************
            try
            {

                byte i;
                Declaraciones.Obj MiObj;
                short NroDrop;
                short Random;
                short ObjIndex;


                // Tira todo el inventario
                if (IsPretoriano)
                {
                    for (i = 1; i <= Declaraciones.MAX_INVENTORY_SLOTS; i++)
                    {
                        if (npc.Invent.Object_Renamed[i].ObjIndex > 0)
                        {
                            MiObj.Amount = npc.Invent.Object_Renamed[i].Amount;
                            MiObj.ObjIndex = npc.Invent.Object_Renamed[i].ObjIndex;
                            bool argNotPirata = true;
                            TirarItemAlPiso(ref npc.Pos, ref MiObj, NotPirata: ref argNotPirata);
                        }
                    }

                    // Dropea oro?
                    if (npc.GiveGLD > 0)
                        TirarOroNpc(npc.GiveGLD, ref npc.Pos);

                    return;
                }

                Random = Convert.ToInt16(Matematicas.RandomNumber(1, 100));

                // Tiene 10% de prob de no tirar nada
                if (Random <= 90)
                {
                    NroDrop = 1;

                    if (Random <= 10)
                    {
                        NroDrop = Convert.ToInt16(NroDrop + 1);

                        for (i = 1; i <= 3; i++)
                        {
                            // 10% de ir pasando de etapas
                            if (Matematicas.RandomNumber(1, 100) <= 10)
                            {
                                NroDrop = Convert.ToInt16(NroDrop + 1);
                            }
                            else
                            {
                                break;
                            }
                        }

                    }


                    ObjIndex = npc.Drop[NroDrop].ObjIndex;
                    if (ObjIndex > 0)
                    {

                        if (ObjIndex == Declaraciones.iORO)
                        {
                            TirarOroNpc(npc.Drop[NroDrop].Amount, ref npc.Pos);
                        }
                        else
                        {
                            MiObj.Amount = Convert.ToInt16(npc.Drop[NroDrop].Amount);
                            MiObj.ObjIndex = npc.Drop[NroDrop].ObjIndex;

                            bool argNotPirata1 = true;
                            TirarItemAlPiso(ref npc.Pos, ref MiObj, NotPirata: ref argNotPirata1);
                        }
                    }


                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in TirarItemAlPiso: " + ex.Message);
            }
        }

        public static bool QuedanItems(short NpcIndex, short ObjIndex)
        {
            bool QuedanItemsRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            try
            {

                short i;
                if (Declaraciones.Npclist[NpcIndex].Invent.NroItems > 0)
                {
                    for (i = 1; i <= Declaraciones.MAX_INVENTORY_SLOTS; i++)
                    {
                        if (Declaraciones.Npclist[NpcIndex].Invent.Object_Renamed[i].ObjIndex == ObjIndex)
                        {
                            QuedanItemsRet = true;
                            return QuedanItemsRet;
                        }
                    }
                }
                QuedanItemsRet = false;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in QuedanItems: " + ex.Message);
            }

            return QuedanItemsRet;
        }

        // '
        // Gets the amount of a certain item that an npc has.
        // 
        // @param npcIndex Specifies reference to npcmerchant
        // @param ObjIndex Specifies reference to object
        // @return   The amount of the item that the npc has
        // @remarks This function reads the Npc.dat file
        public static short EncontrarCant(short NpcIndex, short ObjIndex)
        {
            short EncontrarCantRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: 03/09/08
            // Last Modification By: Marco Vanotti (Marco)
            // - 03/09/08 EncontrarCant now returns 0 if the npc doesn't have it (Marco)
            // ***************************************************
            try
            {
                // Devuelve la cantidad original del obj de un npc

                string ln, npcfile;
                short i;

                npcfile = Declaraciones.DatPath + "NPCs.dat";

                for (i = 1; i <= Declaraciones.MAX_INVENTORY_SLOTS; i++)
                {
                    int argEmptySpaces = 1024;
                    ln = ES.GetVar(npcfile, "NPC" + Declaraciones.Npclist[NpcIndex].Numero, "Obj" + i, EmptySpaces: ref argEmptySpaces);
                    if (ObjIndex == Convert.ToInt16(Migration.ParseVal(General.ReadField(1, ref ln, 45))))
                    {
                        EncontrarCantRet = Convert.ToInt16(Migration.ParseVal(General.ReadField(2, ref ln, 45)));
                        return EncontrarCantRet;
                    }
                }

                EncontrarCantRet = 0;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in reads: " + ex.Message);
            }

            return EncontrarCantRet;
        }

        public static void ResetNpcInv(short NpcIndex)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            try
            {

                short i;

                {
                    ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                    withBlock.Invent.NroItems = 0;

                    for (i = 1; i <= Declaraciones.MAX_INVENTORY_SLOTS; i++)
                    {
                        withBlock.Invent.Object_Renamed[i].ObjIndex = 0;
                        withBlock.Invent.Object_Renamed[i].Amount = 0;
                    }

                    withBlock.InvReSpawn = 0;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in ResetNpcInv: " + ex.Message);
            }
        }

        // '
        // Removes a certain amount of items from a slot of an npc's inventory
        // 
        // @param npcIndex Specifies reference to npcmerchant
        // @param Slot Specifies reference to npc's inventory's slot
        // @param antidad Specifies amount of items that will be removed
        public static void QuitarNpcInvItem(short NpcIndex, byte Slot, short Cantidad)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: 23/11/2009
            // Last Modification By: Marco Vanotti (Marco)
            // - 03/09/08 Now this sub checks that te npc has an item before respawning it (Marco)
            // 23/11/2009: ZaMa - Optimizacion de codigo.
            // ***************************************************
            short ObjIndex;
            short iCant;

            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                ObjIndex = withBlock.Invent.Object_Renamed[Slot].ObjIndex;

                // Quita un Obj
                if (Declaraciones.ObjData_Renamed[withBlock.Invent.Object_Renamed[Slot].ObjIndex].Crucial == 0)
                {
                    withBlock.Invent.Object_Renamed[Slot].Amount = Convert.ToInt16((short)(withBlock.Invent.Object_Renamed[Slot].Amount - Cantidad));

                    if (withBlock.Invent.Object_Renamed[Slot].Amount <= 0)
                    {
                        withBlock.Invent.NroItems = Convert.ToInt16(withBlock.Invent.NroItems - 1);
                        withBlock.Invent.Object_Renamed[Slot].ObjIndex = 0;
                        withBlock.Invent.Object_Renamed[Slot].Amount = 0;
                        if (withBlock.Invent.NroItems == 0 & withBlock.InvReSpawn != 1)
                        {
                            CargarInvent(NpcIndex); // Reponemos el inventario
                        }
                    }
                }
                else
                {
                    withBlock.Invent.Object_Renamed[Slot].Amount = Convert.ToInt16((short)(withBlock.Invent.Object_Renamed[Slot].Amount - Cantidad));

                    if (withBlock.Invent.Object_Renamed[Slot].Amount <= 0)
                    {
                        withBlock.Invent.NroItems = Convert.ToInt16(withBlock.Invent.NroItems - 1);
                        withBlock.Invent.Object_Renamed[Slot].ObjIndex = 0;
                        withBlock.Invent.Object_Renamed[Slot].Amount = 0;

                        if (!QuedanItems(NpcIndex, ObjIndex))
                        {
                            // Check if the item is in the npc's dat.
                            iCant = Convert.ToInt16(EncontrarCant(NpcIndex, ObjIndex));
                            if (iCant != 0)
                            {
                                withBlock.Invent.Object_Renamed[Slot].ObjIndex = ObjIndex;
                                withBlock.Invent.Object_Renamed[Slot].Amount = iCant;
                                withBlock.Invent.NroItems = Convert.ToInt16(withBlock.Invent.NroItems + 1);
                            }
                        }

                        if (withBlock.Invent.NroItems == 0 & withBlock.InvReSpawn != 1)
                        {
                            CargarInvent(NpcIndex); // Reponemos el inventario
                        }
                    }
                }
            }
        }

        public static void CargarInvent(short NpcIndex)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            // Vuelve a cargar el inventario del npc NpcIndex
            short LoopC;
            string ln;
            string npcfile;

            npcfile = Declaraciones.DatPath + "NPCs.dat";

            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                int argEmptySpaces = 1024;
                withBlock.Invent.NroItems = Convert.ToInt16(Migration.ParseVal(ES.GetVar(npcfile, "NPC" + withBlock.Numero, "NROITEMS", EmptySpaces: ref argEmptySpaces)));

                var loopTo = withBlock.Invent.NroItems;
                for (LoopC = 1; LoopC <= loopTo; LoopC++)
                {
                    int argEmptySpaces1 = 1024;
                    ln = ES.GetVar(npcfile, "NPC" + withBlock.Numero, "Obj" + LoopC, EmptySpaces: ref argEmptySpaces1);
                    withBlock.Invent.Object_Renamed[LoopC].ObjIndex = Convert.ToInt16(Migration.ParseVal(General.ReadField(1, ref ln, 45)));
                    withBlock.Invent.Object_Renamed[LoopC].Amount = Convert.ToInt16(Migration.ParseVal(General.ReadField(2, ref ln, 45)));

                }
            }
        }


        public static void TirarOroNpc(int Cantidad, ref Declaraciones.WorldPos Pos)
        {
            // ***************************************************
            // Autor: ZaMa
            // Last Modification: 13/02/2010
            // ***************************************************
            try
            {

                byte i;
                Declaraciones.Obj MiObj;
                int RemainingGold;
                if (Cantidad > 0)
                {

                    RemainingGold = Cantidad;

                    while (RemainingGold > 0)
                    {

                        // Tira pilon de 10k
                        if (RemainingGold > Declaraciones.MAX_INVENTORY_OBJS)
                        {
                            MiObj.Amount = Declaraciones.MAX_INVENTORY_OBJS;
                            RemainingGold = RemainingGold - Declaraciones.MAX_INVENTORY_OBJS;
                        }

                        // Tira lo que quede
                        else
                        {
                            MiObj.Amount = Convert.ToInt16(RemainingGold);
                            RemainingGold = 0;
                        }

                        MiObj.ObjIndex = Declaraciones.iORO;

                        bool argNotPirata = true;
                        TirarItemAlPiso(ref Pos, ref MiObj, NotPirata: ref argNotPirata);
                    }
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in NPC_TIRAR_ITEMS: " + ex.Message);
                string argdesc = "Error en TirarOro. Error " + ex.GetType().Name + " : " + ex.Message;
                General.LogError(ref argdesc);
            }
        }
    }
}