using System;
using Microsoft.VisualBasic;

namespace Legacy
{
    static class mdlCOmercioConUsuario
    {
        private const int MAX_ORO_LOGUEABLE = 50000;
        private const int MAX_OBJ_LOGUEABLE = 1000;

        public const short MAX_OFFER_SLOTS = 30; // 20
        public const short GOLD_OFFER_SLOT = 31;

        public struct tCOmercioUsuario
        {
            public short DestUsu; // El otro Usuario
            public string DestNick;
            [VBFixedArray(MAX_OFFER_SLOTS)]
            public short[] Objeto; // Indice de los objetos que se desea dar
            public int GoldAmount;
            [VBFixedArray(MAX_OFFER_SLOTS)]
            public int[] cant; // Cuantos objetos desea dar
            public bool Acepto;
            public bool Confirmo;

            // UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
            public void Initialize()
            {
                // UPGRADE_WARNING: El límite inferior de la matriz Objeto ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                Objeto = new short[(MAX_OFFER_SLOTS + 1)];
                // UPGRADE_WARNING: El límite inferior de la matriz cant ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                cant = new int[(MAX_OFFER_SLOTS + 1)];
            }
        }

        // origen: origen de la transaccion, originador del comando
        // destino: receptor de la transaccion
        public static void IniciarComercioConUsuario(short Origen, short Destino)
        {
            // ***************************************************
            // Autor: Unkown
            // Last Modification: 25/11/2009
            // 
            // ***************************************************
            try
            {

                // Si ambos pusieron /comerciar entonces
                if (Declaraciones.UserList[Origen].ComUsu.DestUsu == Destino & Declaraciones.UserList[Destino].ComUsu.DestUsu == Origen)
                {
                    // Actualiza el inventario del usuario
                    InvUsuario.UpdateUserInv(true, Origen, 0);
                    // Decirle al origen que abra la ventanita.
                    Protocol.WriteUserCommerceInit(Origen);
                    Declaraciones.UserList[Origen].flags.Comerciando = true;

                    // Actualiza el inventario del usuario
                    InvUsuario.UpdateUserInv(true, Destino, 0);
                    // Decirle al origen que abra la ventanita.
                    Protocol.WriteUserCommerceInit(Destino);
                    Declaraciones.UserList[Destino].flags.Comerciando = true;
                }

                // Call EnviarObjetoTransaccion(Origen)
                else
                {
                    // Es el primero que comercia ?
                    Protocol.WriteConsoleMsg(Destino, Declaraciones.UserList[Origen].name + " desea comerciar. Si deseas aceptar, escribe /COMERCIAR.", Protocol.FontTypeNames.FONTTYPE_TALK);
                    Declaraciones.UserList[Destino].flags.TargetUser = Origen;

                }

                Protocol.FlushBuffer(Destino);
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in Initialize: " + ex.Message);
                string argdesc = "Error en IniciarComercioConUsuario: " + ex.Message;
                General.LogError(ref argdesc);
            }
        }

        public static void EnviarOferta(short UserIndex, byte OfferSlot)
        {
            // ***************************************************
            // Autor: Unkown
            // Last Modification: 25/11/2009
            // Sends the offer change to the other trading user
            // 25/11/2009: ZaMa - Implementado nuevo sistema de comercio con ofertas variables.
            // ***************************************************
            short ObjIndex;
            int ObjAmount;

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                if (OfferSlot == GOLD_OFFER_SLOT)
                {
                    ObjIndex = Declaraciones.iORO;
                    ObjAmount = Declaraciones.UserList[withBlock.ComUsu.DestUsu].ComUsu.GoldAmount;
                }
                else
                {
                    ObjIndex = Declaraciones.UserList[withBlock.ComUsu.DestUsu].ComUsu.Objeto[OfferSlot];
                    ObjAmount = Declaraciones.UserList[withBlock.ComUsu.DestUsu].ComUsu.cant[OfferSlot];
                }
            }

            Protocol.WriteChangeUserTradeSlot(UserIndex, OfferSlot, ObjIndex, ObjAmount);
            Protocol.FlushBuffer(UserIndex);
        }

        public static void FinComerciarUsu(short UserIndex)
        {
            // ***************************************************
            // Autor: Unkown
            // Last Modification: 25/11/2009
            // 25/11/2009: ZaMa - Limpio los arrays (por el nuevo sistema)
            // ***************************************************
            int i;

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                if (withBlock.ComUsu.DestUsu > 0)
                {
                    Protocol.WriteUserCommerceEnd(UserIndex);
                }

                withBlock.ComUsu.Acepto = false;
                withBlock.ComUsu.Confirmo = false;
                withBlock.ComUsu.DestUsu = 0;

                for (i = 1; i <= MAX_OFFER_SLOTS; i++)
                {
                    withBlock.ComUsu.cant[i] = 0;
                    withBlock.ComUsu.Objeto[i] = 0;
                }

                withBlock.ComUsu.GoldAmount = 0;
                withBlock.ComUsu.DestNick = Constants.vbNullString;
                withBlock.flags.Comerciando = false;
            }
        }

        public static void AceptarComercioUsu(short UserIndex)
        {
            // ***************************************************
            // Autor: Unkown
            // Last Modification: 25/11/2009
            // 25/11/2009: ZaMa - Ahora se traspasan hasta 5 items + oro al comerciar
            // ***************************************************
            Declaraciones.Obj TradingObj;
            short OtroUserIndex;
            bool TerminarAhora;
            short OfferSlot;

            Declaraciones.UserList[UserIndex].ComUsu.Acepto = true;

            OtroUserIndex = Declaraciones.UserList[UserIndex].ComUsu.DestUsu;

            if (Declaraciones.UserList[OtroUserIndex].ComUsu.Acepto == false)
            {
                return;
            }

            // Envio los items a quien corresponde
            for (OfferSlot = 1; OfferSlot <= MAX_OFFER_SLOTS + 1; OfferSlot++)
            {

                // Items del 1er usuario
                {
                    ref var withBlock = ref Declaraciones.UserList[UserIndex];
                    // Le pasa el oro
                    if (OfferSlot == GOLD_OFFER_SLOT)
                    {
                        // Quito la cantidad de oro ofrecida
                        withBlock.Stats.GLD = withBlock.Stats.GLD - withBlock.ComUsu.GoldAmount;
                        // Log
                        if (withBlock.ComUsu.GoldAmount > MAX_ORO_LOGUEABLE)
                            General.LogDesarrollo(withBlock.name + " soltó oro en comercio seguro con " + Declaraciones.UserList[OtroUserIndex].name + ". Cantidad: " + withBlock.ComUsu.GoldAmount);
                        // Update Usuario
                        Protocol.WriteUpdateUserStats(UserIndex);
                        // Se la doy al otro
                        Declaraciones.UserList[OtroUserIndex].Stats.GLD = Declaraciones.UserList[OtroUserIndex].Stats.GLD + withBlock.ComUsu.GoldAmount;
                        // Update Otro Usuario
                        Protocol.WriteUpdateUserStats(OtroUserIndex);
                    }

                    // Le pasa lo ofertado de los slots con items
                    else if (withBlock.ComUsu.Objeto[OfferSlot] > 0)
                    {
                        TradingObj.ObjIndex = Convert.ToInt16(withBlock.ComUsu.Objeto[OfferSlot]);
                        TradingObj.Amount = Convert.ToInt16(withBlock.ComUsu.cant[OfferSlot]);

                        // Quita el objeto y se lo da al otro
                        if (!InvUsuario.MeterItemEnInventario(OtroUserIndex, ref TradingObj))
                        {
                            bool argNotPirata = true;
                            InvNpc.TirarItemAlPiso(ref Declaraciones.UserList[OtroUserIndex].Pos, ref TradingObj, NotPirata: ref argNotPirata);
                        }

                        Trabajo.QuitarObjetos(TradingObj.ObjIndex, TradingObj.Amount, UserIndex);

                        // Es un Objeto que tenemos que loguear? Pablo (ToxicWaste) 07/09/07
                        if (Declaraciones.ObjData_Renamed[TradingObj.ObjIndex].Log == 1)
                        {
                            General.LogDesarrollo(withBlock.name + " le pasó en comercio seguro a " + Declaraciones.UserList[OtroUserIndex].name + " " + TradingObj.Amount + " " + Declaraciones.ObjData_Renamed[TradingObj.ObjIndex].name);
                        }

                        // Es mucha cantidad?
                        if (TradingObj.Amount > MAX_OBJ_LOGUEABLE)
                        {
                            // Si no es de los prohibidos de loguear, lo logueamos.
                            if (Declaraciones.ObjData_Renamed[TradingObj.ObjIndex].NoLog != 1)
                            {
                                General.LogDesarrollo(Declaraciones.UserList[OtroUserIndex].name + " le pasó en comercio seguro a " + withBlock.name + " " + TradingObj.Amount + " " + Declaraciones.ObjData_Renamed[TradingObj.ObjIndex].name);
                            }
                        }
                    }
                }

                // Items del 2do usuario
                {
                    ref var withBlock1 = ref Declaraciones.UserList[OtroUserIndex];
                    // Le pasa el oro
                    if (OfferSlot == GOLD_OFFER_SLOT)
                    {
                        // Quito la cantidad de oro ofrecida
                        withBlock1.Stats.GLD = withBlock1.Stats.GLD - withBlock1.ComUsu.GoldAmount;
                        // Log
                        if (withBlock1.ComUsu.GoldAmount > MAX_ORO_LOGUEABLE)
                            General.LogDesarrollo(withBlock1.name + " soltó oro en comercio seguro con " + Declaraciones.UserList[UserIndex].name + ". Cantidad: " + withBlock1.ComUsu.GoldAmount);
                        // Update Usuario
                        Protocol.WriteUpdateUserStats(OtroUserIndex);
                        // y se la doy al otro
                        Declaraciones.UserList[UserIndex].Stats.GLD = Declaraciones.UserList[UserIndex].Stats.GLD + withBlock1.ComUsu.GoldAmount;
                        if (withBlock1.ComUsu.GoldAmount > MAX_ORO_LOGUEABLE)
                            General.LogDesarrollo(Declaraciones.UserList[UserIndex].name + " recibió oro en comercio seguro con " + withBlock1.name + ". Cantidad: " + withBlock1.ComUsu.GoldAmount);
                        // Update Otro Usuario
                        Protocol.WriteUpdateUserStats(UserIndex);
                    }

                    // Le pasa la oferta de los slots con items
                    else if (withBlock1.ComUsu.Objeto[OfferSlot] > 0)
                    {
                        TradingObj.ObjIndex = Convert.ToInt16(withBlock1.ComUsu.Objeto[OfferSlot]);
                        TradingObj.Amount = Convert.ToInt16(withBlock1.ComUsu.cant[OfferSlot]);

                        // Quita el objeto y se lo da al otro
                        if (!InvUsuario.MeterItemEnInventario(UserIndex, ref TradingObj))
                        {
                            bool argNotPirata1 = true;
                            InvNpc.TirarItemAlPiso(ref Declaraciones.UserList[UserIndex].Pos, ref TradingObj, NotPirata: ref argNotPirata1);
                        }

                        Trabajo.QuitarObjetos(TradingObj.ObjIndex, TradingObj.Amount, OtroUserIndex);

                        // Es un Objeto que tenemos que loguear? Pablo (ToxicWaste) 07/09/07
                        if (Declaraciones.ObjData_Renamed[TradingObj.ObjIndex].Log == 1)
                        {
                            General.LogDesarrollo(withBlock1.name + " le pasó en comercio seguro a " + Declaraciones.UserList[UserIndex].name + " " + TradingObj.Amount + " " + Declaraciones.ObjData_Renamed[TradingObj.ObjIndex].name);
                        }

                        // Es mucha cantidad?
                        if (TradingObj.Amount > MAX_OBJ_LOGUEABLE)
                        {
                            // Si no es de los prohibidos de loguear, lo logueamos.
                            if (Declaraciones.ObjData_Renamed[TradingObj.ObjIndex].NoLog != 1)
                            {
                                General.LogDesarrollo(withBlock1.name + " le pasó en comercio seguro a " + Declaraciones.UserList[UserIndex].name + " " + TradingObj.Amount + " " + Declaraciones.ObjData_Renamed[TradingObj.ObjIndex].name);
                            }
                        }
                    }
                }

            }

            // End Trade
            FinComerciarUsu(UserIndex);
            FinComerciarUsu(OtroUserIndex);
        }

        public static void AgregarOferta(short UserIndex, byte OfferSlot, short ObjIndex, int Amount, bool IsGold)
        {
            // ***************************************************
            // Autor: ZaMa
            // Last Modification: 24/11/2009
            // Adds gold or items to the user's offer
            // ***************************************************

            if (PuedeSeguirComerciando(UserIndex))
            {
                {
                    ref var withBlock = ref Declaraciones.UserList[UserIndex].ComUsu;
                    // Si ya confirmo su oferta, no puede cambiarla!
                    if (!withBlock.Confirmo)
                    {
                        if (IsGold)
                        {
                            // Agregamos (o quitamos) mas oro a la oferta
                            withBlock.GoldAmount = withBlock.GoldAmount + Amount;

                            // Imposible que pase, pero por las dudas..
                            if (withBlock.GoldAmount < 0)
                                withBlock.GoldAmount = 0;
                        }
                        else
                        {
                            // Agreamos (o quitamos) el item y su cantidad en el slot correspondiente
                            // Si es 0 estoy modificando la cantidad, no agregando
                            if (ObjIndex > 0)
                                withBlock.Objeto[OfferSlot] = ObjIndex;
                            withBlock.cant[OfferSlot] = withBlock.cant[OfferSlot] + Amount;

                            // Quitó todos los items de ese tipo
                            if (withBlock.cant[OfferSlot] <= 0)
                            {
                                // Removemos el objeto para evitar conflictos
                                withBlock.Objeto[OfferSlot] = 0;
                                withBlock.cant[OfferSlot] = 0;
                            }
                        }
                    }
                }
            }
        }

        public static bool PuedeSeguirComerciando(short UserIndex)
        {
            bool PuedeSeguirComerciandoRet = default;
            // ***************************************************
            // Autor: ZaMa
            // Last Modification: 24/11/2009
            // Validates wether the conditions for the commerce to keep going are satisfied
            // ***************************************************
            short OtroUserIndex;
            var ComercioInvalido = default(bool);

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                // Usuario valido?
                if (withBlock.ComUsu.DestUsu <= 0 | withBlock.ComUsu.DestUsu > Declaraciones.MaxUsers)
                {
                    ComercioInvalido = true;
                }

                OtroUserIndex = withBlock.ComUsu.DestUsu;

                if (!ComercioInvalido)
                {
                    // Estan logueados?
                    if (Declaraciones.UserList[OtroUserIndex].flags.UserLogged == false | withBlock.flags.UserLogged == false)
                    {
                        ComercioInvalido = true;
                    }
                }

                if (!ComercioInvalido)
                {
                    // Se estan comerciando el uno al otro?
                    if (Declaraciones.UserList[OtroUserIndex].ComUsu.DestUsu != UserIndex)
                    {
                        ComercioInvalido = true;
                    }
                }

                if (!ComercioInvalido)
                {
                    // El nombre del otro es el mismo que al que le comercio?
                    if ((Declaraciones.UserList[OtroUserIndex].name ?? "") != (withBlock.ComUsu.DestNick ?? ""))
                    {
                        ComercioInvalido = true;
                    }
                }

                if (!ComercioInvalido)
                {
                    // Mi nombre  es el mismo que al que el le comercia?
                    if ((withBlock.name ?? "") != (Declaraciones.UserList[OtroUserIndex].ComUsu.DestNick ?? ""))
                    {
                        ComercioInvalido = true;
                    }
                }

                if (!ComercioInvalido)
                {
                    // Esta vivo?
                    if (Declaraciones.UserList[OtroUserIndex].flags.Muerto == 1)
                    {
                        ComercioInvalido = true;
                    }
                }

                // Fin del comercio
                if (ComercioInvalido == true)
                {
                    FinComerciarUsu(UserIndex);

                    if (OtroUserIndex <= 0 | OtroUserIndex > Declaraciones.MaxUsers)
                    {
                        FinComerciarUsu(OtroUserIndex);
                        Protocol.FlushBuffer(OtroUserIndex);
                    }

                    return PuedeSeguirComerciandoRet;
                }
            }

            PuedeSeguirComerciandoRet = true;
            return PuedeSeguirComerciandoRet;
        }
    }
}