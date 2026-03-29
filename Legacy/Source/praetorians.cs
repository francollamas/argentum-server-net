using System;
using System.Drawing;
using Microsoft.VisualBasic;

namespace Legacy
{

    static class PraetoriansCoopNPC
    {
        // ''''''''''''''''''''''''''''''''''''''''
        // ' DECLARACIONES DEL MODULO PRETORIANO ''
        // ''''''''''''''''''''''''''''''''''''''''
        // ' Estas constantes definen que valores tienen
        // ' los NPCs pretorianos en el NPC-HOSTILES.DAT
        // ' Son FIJAS, pero se podria hacer una rutina que
        // ' las lea desde el npcshostiles.dat

        public const short PRCLER_NPC = 900; // '"Sacerdote Pretoriano"
        public const short PRGUER_NPC = 901; // '"Guerrero  Pretoriano"
        public const short PRMAGO_NPC = 902; // '"Mago Pretoriano"
        public const short PRCAZA_NPC = 903; // '"Cazador Pretoriano"
        public const short PRKING_NPC = 904; // '"Rey Pretoriano"
                                             // '''''''''''''''''''''''''''''''''''''''''''''
                                             // 'Esta constante identifica en que mapa esta
                                             // 'la fortaleza pretoriana (no es lo mismo de
                                             // 'donde estan los NPCs!).
                                             // 'Se extrae el dato del server.ini en sub LoadSIni
        public static short MAPA_PRETORIANO;

        // '''''''''''''''''''''''''''''''''''''''''''''
        // 'Estos numeros son necesarios por cuestiones de
        // 'sonido. Son los numeros de los wavs del cliente.
        private const short SONIDO_Dragon_VIVO = 30;
        // 'ALCOBAS REALES
        // 'OJO LOS BICHOS TAN HARDCODEADOS, NO CAMBIAR EL MAPA DONDE
        // 'ESTÁN UBICADOS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // 'MUCHO MENOS LA COORDENADA Y DE LAS ALCOBAS YA QUE DEBE SER LA MISMA!!!
        // '(HAY FUNCIONES Q CUENTAN CON QUE ES LA MISMA!)
        public const short ALCOBA1_X = 35;
        public const short ALCOBA1_Y = 25;
        public const short ALCOBA2_X = 67;
        public const short ALCOBA2_Y = 25;

        // Added by Nacho
        // Cuantos pretorianos vivos quedan. Uno por cada alcoba
        public static short pretorianosVivos;

        // /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\
        // /\/\/\/\/\/\/\/\ MODULO DE COMBATE PRETORIANO /\/\/\/\/\/\/\/\/\
        // /\/\/\/\/\/\/\/\ (NPCS COOPERATIVOS TIPO CLAN)/\/\/\/\/\/\/\/\/\
        // /\/\/\/\/\/\/\/\         por EL OSO           /\/\/\/\/\/\/\/\/\
        // /\/\/\/\/\/\/\/\       mbarrou@dc.uba.ar      /\/\/\/\/\/\/\/\/\
        // /\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\/\

        public static short esPretoriano(short NpcIndex)
        {
            short esPretorianoRet = default;
            try
            {

                short N;
                short i;
                N = Declaraciones.Npclist[NpcIndex].Numero;
                i = Declaraciones.Npclist[NpcIndex].Char_Renamed.CharIndex;
                // Call SendData(SendTarget.ToNPCArea, NpcIndex, Npclist(NpcIndex).Pos.Map, "||" & vbGreen & "° Soy Pretoriano °" & Str(ind))
                switch (Declaraciones.Npclist[NpcIndex].Numero)
                {
                    case PRCLER_NPC:
                        {
                            esPretorianoRet = 1;
                            break;
                        }
                    case PRMAGO_NPC:
                        {
                            esPretorianoRet = 2;
                            break;
                        }
                    case PRCAZA_NPC:
                        {
                            esPretorianoRet = 3;
                            break;
                        }
                    case PRKING_NPC:
                        {
                            esPretorianoRet = 4;
                            break;
                        }
                    case PRGUER_NPC:
                        {
                            esPretorianoRet = 5;
                            break;
                        }
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in LoadSIni: " + ex.Message);
                string argdesc = "Error en NPCAI.EsPretoriano? " + Declaraciones.Npclist[NpcIndex].name;
                General.LogError(ref argdesc);
                // do nothing
            }

            return esPretorianoRet;
        }


        public static void CrearClanPretoriano(short X)
        {
            // ********************************************************
            // Author: EL OSO
            // Inicializa el clan Pretoriano.
            // Last Modify Date: 22/6/06: (Nacho) Seteamos cuantos NPCs creamos
            // ********************************************************
            try
            {

                // '------------------------------------------------------
                // 'recibe el X,Y donde EL REY ANTERIOR ESTABA POSICIONADO.
                // '------------------------------------------------------
                // '35,25 y 67,25 son las posiciones del rey

                // 'Sub CrearNPC(NroNPC As Integer, mapa As Integer, OrigPos As WorldPos)
                // 'Public Const PRCLER_NPC = 900
                // 'Public Const PRGUER_NPC = 901
                // 'Public Const PRMAGO_NPC = 902
                // 'Public Const PRCAZA_NPC = 903
                // 'Public Const PRKING_NPC = 904
                Declaraciones.WorldPos wp;
                var wp2 = default(Declaraciones.WorldPos);
                short TeleFrag;

                wp.Map = MAPA_PRETORIANO;
                if (X < 50) // 'forma burda de ver que alcoba es
                {
                    wp.X = ALCOBA2_X;
                    wp.Y = ALCOBA2_Y;
                }
                else
                {
                    wp.X = ALCOBA1_X;
                    wp.Y = ALCOBA1_Y;
                }
                pretorianosVivos = 7; // Hay 7 + el Rey.
                TeleFrag = Declaraciones.MapData[wp.Map, wp.X, wp.Y].NpcIndex;

                if (TeleFrag > 0)
                {
                    // 'El rey va a pisar a un npc de antiguo rey
                    // 'Obtengo en WP2 la mejor posicion cercana
                    bool argPuedeAgua = false;
                    bool argPuedeTierra = true;
                    Extra.ClosestLegalPos(ref wp, ref wp2, PuedeAgua: ref argPuedeAgua, PuedeTierra: ref argPuedeTierra);
                    if (Extra.LegalPos(wp2.Map, wp2.X, wp2.Y))
                    {
                        // 'mover al actual

                        modSendData.SendData(modSendData.SendTarget.ToNPCArea, TeleFrag, Protocol.PrepareMessageCharacterMove(Declaraciones.Npclist[TeleFrag].Char_Renamed.CharIndex, Convert.ToByte(wp2.X), Convert.ToByte(wp2.Y)));
                        // Update map and user pos
                        Declaraciones.MapData[wp.Map, wp.X, wp.Y].NpcIndex = 0;
                        // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Npclist().Pos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        Declaraciones.Npclist[TeleFrag].Pos = wp2;
                        Declaraciones.MapData[wp2.Map, wp2.X, wp2.Y].NpcIndex = TeleFrag;
                    }
                    else
                    {
                        // 'TELEFRAG!!!
                        NPCs.QuitarNPC(TeleFrag);
                    }
                }
                // 'ya limpié el lugar para el rey (wp)
                // 'Los otros no necesitan este caso ya que respawnan lejos
                var nPos = default(Declaraciones.WorldPos);
                // Busco la posicion legal mas cercana aca, aun que creo que tendría que ir en el crearnpc. (NicoNZ)
                bool argPuedeAgua1 = false;
                bool argPuedeTierra1 = true;
                Extra.ClosestLegalPos(ref wp, ref nPos, ref argPuedeAgua1, ref argPuedeTierra1);
                short argNroNPC = PRKING_NPC;
                NPCs.CrearNPC(ref argNroNPC, ref MAPA_PRETORIANO, ref nPos);

                wp.X = Convert.ToInt16(wp.X + 3);
                bool argPuedeAgua2 = false;
                bool argPuedeTierra2 = true;
                Extra.ClosestLegalPos(ref wp, ref nPos, ref argPuedeAgua2, ref argPuedeTierra2);
                short argNroNPC1 = PRCLER_NPC;
                NPCs.CrearNPC(ref argNroNPC1, ref MAPA_PRETORIANO, ref nPos);

                wp.X = Convert.ToInt16(wp.X - 6);
                bool argPuedeAgua3 = false;
                bool argPuedeTierra3 = true;
                Extra.ClosestLegalPos(ref wp, ref nPos, ref argPuedeAgua3, ref argPuedeTierra3);
                short argNroNPC2 = PRCLER_NPC;
                NPCs.CrearNPC(ref argNroNPC2, ref MAPA_PRETORIANO, ref nPos);

                wp.Y = Convert.ToInt16(wp.Y + 3);
                bool argPuedeAgua4 = false;
                bool argPuedeTierra4 = true;
                Extra.ClosestLegalPos(ref wp, ref nPos, ref argPuedeAgua4, ref argPuedeTierra4);
                short argNroNPC3 = PRGUER_NPC;
                NPCs.CrearNPC(ref argNroNPC3, ref MAPA_PRETORIANO, ref nPos);

                wp.X = Convert.ToInt16(wp.X + 3);
                bool argPuedeAgua5 = false;
                bool argPuedeTierra5 = true;
                Extra.ClosestLegalPos(ref wp, ref nPos, ref argPuedeAgua5, ref argPuedeTierra5);
                short argNroNPC4 = PRGUER_NPC;
                NPCs.CrearNPC(ref argNroNPC4, ref MAPA_PRETORIANO, ref nPos);

                wp.X = Convert.ToInt16(wp.X + 3);
                bool argPuedeAgua6 = false;
                bool argPuedeTierra6 = true;
                Extra.ClosestLegalPos(ref wp, ref nPos, ref argPuedeAgua6, ref argPuedeTierra6);
                short argNroNPC5 = PRGUER_NPC;
                NPCs.CrearNPC(ref argNroNPC5, ref MAPA_PRETORIANO, ref nPos);

                wp.Y = Convert.ToInt16(wp.Y - 6);
                wp.X = Convert.ToInt16(wp.X - 1);
                bool argPuedeAgua7 = false;
                bool argPuedeTierra7 = true;
                Extra.ClosestLegalPos(ref wp, ref nPos, ref argPuedeAgua7, ref argPuedeTierra7);
                short argNroNPC6 = PRCAZA_NPC;
                NPCs.CrearNPC(ref argNroNPC6, ref MAPA_PRETORIANO, ref nPos);

                wp.X = Convert.ToInt16(wp.X - 4);
                bool argPuedeAgua8 = false;
                bool argPuedeTierra8 = true;
                Extra.ClosestLegalPos(ref wp, ref nPos, ref argPuedeAgua8, ref argPuedeTierra8);
                short argNroNPC7 = PRMAGO_NPC;
                NPCs.CrearNPC(ref argNroNPC7, ref MAPA_PRETORIANO, ref nPos);
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in CrearClanPretoriano: " + ex.Message);
                string argdesc = "Error en NPCAI.CrearClanPretoriano ";
                General.LogError(ref argdesc);
                // do nothing
            }
        }

        public static void PRCAZA_AI(short npcind)
        {
            try
            {
                // ' NO CAMBIAR:
                // ' HECHIZOS: 1- FLECHA


                short X;
                short Y;
                short NPCPosX;
                short NPCPosY;
                short NPCPosM;
                var BestTarget = default(short);
                short NPCAlInd;
                short PJEnInd;

                bool PJBestTarget;
                short BTx;
                short BTy;
                short Xc;
                short Yc;
                short azar;
                short azar2;

                byte quehacer;
                // '1- Ataca usuarios

                NPCPosX = Declaraciones.Npclist[npcind].Pos.X;
                NPCPosY = Declaraciones.Npclist[npcind].Pos.Y;
                NPCPosM = Declaraciones.Npclist[npcind].Pos.Map;

                PJBestTarget = false;
                X = 0;
                Y = 0;
                quehacer = 0;


                azar = Convert.ToInt16(Math.Sign(Matematicas.RandomNumber(-1, 1)));
                // azar = Sgn(azar)
                if (azar == 0)
                    azar = 1;
                azar2 = Convert.ToInt16(Math.Sign(Matematicas.RandomNumber(-1, 1)));
                // azar2 = Sgn(azar2)
                if (azar2 == 0)
                    azar2 = 1;


                // pick the best target according to the following criteria:
                // 1) magues ARE dangerous, but they are weak too, they're
                // our primary target
                // 2) in any other case, our nearest enemy will be attacked

                var loopTo = Convert.ToInt16(NPCPosX + azar * -8);
                for (X = Convert.ToInt16(NPCPosX + azar * 8); -azar >= 0 ? X <= loopTo : X >= loopTo; X += -azar)
                {
                    var loopTo1 = Convert.ToInt16(NPCPosY + azar2 * -7);
                    for (Y = Convert.ToInt16(NPCPosY + azar2 * 7); -azar2 >= 0 ? Y <= loopTo1 : Y >= loopTo1; Y += -azar2)
                    {
                        NPCAlInd = Declaraciones.MapData[NPCPosM, X, Y].NpcIndex; // 'por si implementamos algo contra NPCs
                        PJEnInd = Declaraciones.MapData[NPCPosM, X, Y].UserIndex;
                        if (PJEnInd > 0 & Declaraciones.Npclist[npcind].CanAttack == 1)
                        {
                            if ((Declaraciones.UserList[PJEnInd].flags.invisible == 0 | Declaraciones.UserList[PJEnInd].flags.Oculto == 0) & !(Declaraciones.UserList[PJEnInd].flags.Muerto == 1) & !(Declaraciones.UserList[PJEnInd].flags.AdminInvisible == 1) & Declaraciones.UserList[PJEnInd].flags.AdminPerseguible)
                            {
                                // ToDo: Borrar los GMs
                                if (EsMagoOClerigo(PJEnInd))
                                {
                                    // 'say no more, atacar a este
                                    PJBestTarget = true;
                                    BestTarget = PJEnInd;
                                    quehacer = 1;
                                    // Call NpcLanzaSpellSobreUser(npcind, PJEnInd, Npclist(npcind).Spells(1)) ''flecha pasa como spell
                                    X = Convert.ToInt16(NPCPosX + azar * -8);
                                    Y = Convert.ToInt16(NPCPosY + azar2 * -7);
                                }
                                // 'forma espantosa de zafar del for
                                else if (BestTarget > 0)
                                {
                                    // 'ver el mas cercano a mi
                                    if (Math.Sqrt(Math.Pow(X - NPCPosX, 2d) + Math.Pow(Y - NPCPosY, 2d)) < Math.Sqrt(Math.Pow(NPCPosX - Declaraciones.UserList[BestTarget].Pos.X, 2d) + Math.Pow(NPCPosY - Declaraciones.UserList[BestTarget].Pos.Y, 2d)))
                                    {
                                        // 'el nuevo esta mas cerca
                                        PJBestTarget = true;
                                        BestTarget = PJEnInd;
                                        quehacer = 1;
                                    }
                                }
                                else
                                {
                                    PJBestTarget = true;
                                    BestTarget = PJEnInd;
                                    quehacer = 1;
                                }
                            }
                        } // 'Fin analisis del tile
                    }
                }

                switch (quehacer)
                {
                    case 1: // 'nearest target
                        {
                            if (Declaraciones.Npclist[npcind].CanAttack == 1)
                            {
                                modHechizos.NpcLanzaSpellSobreUser(npcind, BestTarget, Declaraciones.Npclist[npcind].Spells[1]);
                            }

                            break;
                        }
                        // 'case 2: not yet implemented
                }

                // '  Vamos a setear el hold on del cazador en el medio entre el rey
                // '  y el atacante. De esta manera se lo podra atacar aun asi está lejos
                // '  pero sin alejarse del rango de los an hoax vorps de los
                // '  clerigos o rey. A menos q este paralizado, claro

                if (Declaraciones.Npclist[npcind].flags.Paralizado == 1)
                    return;

                if (!(NPCPosM == MAPA_PRETORIANO))
                    return;


                // MEJORA: Si quedan solos, se van con el resto del ejercito
                if (Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot != 0)
                {
                    // si me estoy yendo a alguna alcoba
                    CambiarAlcoba(npcind);
                    return;
                }


                if (EstoyMuyLejos(npcind))
                {
                    VolverAlCentro(npcind);
                    return;
                }

                if (BestTarget > 0)
                {

                    BTx = Declaraciones.UserList[BestTarget].Pos.X;
                    BTy = Declaraciones.UserList[BestTarget].Pos.Y;

                    if (NPCPosX < 50)
                    {

                        GreedyWalkTo(npcind, MAPA_PRETORIANO, Convert.ToInt16(ALCOBA1_X + (BTx - ALCOBA1_X) / 2), Convert.ToInt16(ALCOBA1_Y + (BTy - ALCOBA1_Y) / 2));
                    }
                    // GreedyWalkTo npcind, MAPA_PRETORIANO, ALCOBA1_X + ((BTx - ALCOBA1_X) \ 2), ALCOBA1_Y + ((BTy - ALCOBA1_Y) \ 2)
                    else
                    {
                        GreedyWalkTo(npcind, MAPA_PRETORIANO, Convert.ToInt16(ALCOBA2_X + (BTx - ALCOBA2_X) / 2), Convert.ToInt16(ALCOBA2_Y + (BTy - ALCOBA2_Y) / 2));
                        // GreedyWalkTo npcind, MAPA_PRETORIANO, ALCOBA2_X + ((BTx - ALCOBA2_X) \ 2), ALCOBA2_Y + ((BTy - ALCOBA2_Y) \ 2)
                    }
                }
                else
                {
                    // '2do Loop. Busca gente acercandose por otros frentes para frenarla
                    if (NPCPosX < 50)
                        Xc = ALCOBA1_X;
                    else
                        Xc = ALCOBA2_X;
                    Yc = ALCOBA1_Y;

                    var loopTo2 = Convert.ToInt16(Xc + 16);
                    for (X = Convert.ToInt16(Xc - 16); X <= loopTo2; X++)
                    {
                        var loopTo3 = Convert.ToInt16(Yc + 14);
                        for (Y = Convert.ToInt16(Yc - 14); Y <= loopTo3; Y++)
                        {
                            if (!(X <= NPCPosX + 8 & X >= NPCPosX - 8 & Y >= NPCPosY - 7 & Y <= NPCPosY + 7))
                            {
                                // 'si es un tile no analizado
                                PJEnInd = Declaraciones.MapData[NPCPosM, X, Y].UserIndex; // 'por si implementamos algo contra NPCs
                                if (PJEnInd > 0)
                                {
                                    if (!(Declaraciones.UserList[PJEnInd].flags.invisible == 1 | Declaraciones.UserList[PJEnInd].flags.Oculto == 1 | Declaraciones.UserList[PJEnInd].flags.Muerto == 1))

                                    {
                                        // 'si no esta muerto.., ya encontro algo para ir a buscar
                                        GreedyWalkTo(npcind, MAPA_PRETORIANO, Declaraciones.UserList[PJEnInd].Pos.X, Declaraciones.UserList[PJEnInd].Pos.Y);
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    // 'vuelve si no esta en proceso de ataque a usuarios
                    if (Declaraciones.Npclist[npcind].CanAttack == 1)
                        VolverAlCentro(npcind);

                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in PRCAZA_AI: " + ex.Message);
                string argdesc = "Error en NPCAI.PRCAZA_AI ";
                General.LogError(ref argdesc);
                // do nothing
            }
        }

        public static void PRMAGO_AI(short npcind)
        {
            try
            {

                // HECHIZOS: NO CAMBIAR ACA
                // REPRESENTAN LA UBICACION DE LOS SPELLS EN NPC_HOSTILES.DAT y si se los puede cambiar en ese archivo
                // 1- APOCALIPSIS 'modificable
                // 2- REMOVER INVISIBILIDAD 'NO MODIFICABLE
                short DAT_APOCALIPSIS;
                short DAT_REMUEVE_INVI;
                DAT_APOCALIPSIS = 1;
                DAT_REMUEVE_INVI = 2;

                // 'EL mago pretoriano guarda  el index al NPC Rey en el
                // 'inventario.barcoobjind parameter. Ese no es usado nunca.
                // 'EL objetivo es no modificar al TAD NPC utilizando una propiedad
                // 'que nunca va a ser utilizada por un NPC (espero)
                short X;
                short Y;
                short NPCPosX;
                short NPCPosY;
                short NPCPosM;
                short BestTarget;
                short NPCAlInd;
                short PJEnInd;
                bool PJBestTarget;
                byte bs;
                short azar;
                short azar2;

                byte quehacer;
                // '1- atacar a enemigos
                // '2- remover invisibilidades
                // '3- rotura de vara

                NPCPosX = Declaraciones.Npclist[npcind].Pos.X; // 'store current position
                NPCPosY = Declaraciones.Npclist[npcind].Pos.Y; // 'for direct access
                NPCPosM = Declaraciones.Npclist[npcind].Pos.Map;

                PJBestTarget = false;
                BestTarget = 0;
                quehacer = 0;
                X = 0;
                Y = 0;


                if (Declaraciones.Npclist[npcind].Stats.MinHp < 750) // 'Dying
                {
                    quehacer = 3; // 'va a romper su vara en 5 segundos
                }
                else
                {
                    if (!(Declaraciones.Npclist[npcind].Invent.BarcoSlot == 6))
                    {
                        Declaraciones.Npclist[npcind].Invent.BarcoSlot = 6; // 'restore wand break counter
                        modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageCreateFX(Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, 0, 0));
                    }

                    // pick the best target according to the following criteria:
                    // 1) invisible enemies can be detected sometimes
                    // 2) a wizard's mission is background spellcasting attack

                    azar = Convert.ToInt16(Math.Sign(Matematicas.RandomNumber(-1, 1)));
                    // azar = Sgn(azar)
                    if (azar == 0)
                        azar = 1;
                    azar2 = Convert.ToInt16(Math.Sign(Matematicas.RandomNumber(-1, 1)));
                    // azar2 = Sgn(azar2)
                    if (azar2 == 0)
                        azar2 = 1;

                    // 'esto fue para rastrear el combat field al azar
                    // 'Si no se hace asi, los NPCs Pretorianos "combinan" ataques, y cada
                    // 'ataque puede sumar hasta 700 Hit Points, lo cual los vuelve
                    // 'invulnerables

                    // azar = 1

                    var loopTo = Convert.ToInt16(NPCPosX + azar * -8);
                    for (X = Convert.ToInt16(NPCPosX + azar * 8); -azar >= 0 ? X <= loopTo : X >= loopTo; X += -azar)
                    {
                        var loopTo1 = Convert.ToInt16(NPCPosY + azar2 * -7);
                        for (Y = Convert.ToInt16(NPCPosY + azar2 * 7); -azar2 >= 0 ? Y <= loopTo1 : Y >= loopTo1; Y += -azar2)
                        {
                            NPCAlInd = Declaraciones.MapData[NPCPosM, X, Y].NpcIndex; // 'por si implementamos algo contra NPCs
                            PJEnInd = Declaraciones.MapData[NPCPosM, X, Y].UserIndex;

                            if (PJEnInd > 0 & Declaraciones.Npclist[npcind].CanAttack == 1)
                            {
                                if (!(Declaraciones.UserList[PJEnInd].flags.Muerto == 1) & !(Declaraciones.UserList[PJEnInd].flags.AdminInvisible == 1) & Declaraciones.UserList[PJEnInd].flags.AdminPerseguible)
                                {
                                    if (Declaraciones.UserList[PJEnInd].flags.invisible == 1 | Declaraciones.UserList[PJEnInd].flags.Oculto == 1)
                                    {
                                        // 'usuario invisible, vamos a ver si se la podemos sacar

                                        if (Matematicas.RandomNumber(1, 100) <= 35)
                                        {
                                            // 'mago detecta invisiblidad
                                            Declaraciones.Npclist[npcind].CanAttack = 0;
                                            NPCRemueveInvisibilidad(npcind, PJEnInd, DAT_REMUEVE_INVI);
                                            return; // 'basta, SUFICIENTE!, jeje
                                        }
                                        if (Declaraciones.UserList[PJEnInd].flags.Paralizado == 1)
                                        {
                                            // 'los usuarios invisibles y paralizados son un buen target!
                                            BestTarget = PJEnInd;
                                            PJBestTarget = true;
                                            quehacer = 2;
                                        }
                                    }
                                    else if (Declaraciones.UserList[PJEnInd].flags.Paralizado == 1)
                                    {
                                        if (BestTarget > 0)
                                        {
                                            if (!(Declaraciones.UserList[BestTarget].flags.invisible == 1 | Declaraciones.UserList[PJEnInd].flags.Oculto == 1))


                                            {
                                                // 'encontre un paralizado visible, y no hay un besttarget invisible (paralizado invisible)
                                                BestTarget = PJEnInd;
                                                PJBestTarget = true;
                                                quehacer = 2;
                                            }
                                        }
                                        else
                                        {
                                            BestTarget = PJEnInd;
                                            PJBestTarget = true;
                                            quehacer = 2;
                                        }
                                    }
                                    else if (BestTarget == 0)
                                    {
                                        // 'movil visible
                                        BestTarget = PJEnInd;
                                        PJBestTarget = true;
                                        quehacer = 2;
                                    } // '
                                } // 'endif:    not muerto
                            } // 'endif: es un tile con PJ y puede atacar
                        }
                    }
                } // 'endif esta muriendo


                switch (quehacer)
                {
                    // 'case 1 esta "harcodeado" en el doble for
                    // 'es remover invisibilidades
                    case 2: // 'apocalipsis Rahma Nañarak O'al
                        {
                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead(Declaraciones.Hechizos[Declaraciones.Npclist[npcind].Spells[DAT_APOCALIPSIS]].PalabrasMagicas, Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Cyan)));
                            NpcLanzaSpellSobreUser2(npcind, BestTarget, Declaraciones.Npclist[npcind].Spells[DAT_APOCALIPSIS]);
                            break;
                        }
                    // 'SPELL 1 de Mago: Apocalipsis
                    case 3:
                        {

                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageCreateFX(Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, (short)Declaraciones.FXIDs.FXMEDITARGRANDE, Declaraciones.INFINITE_LOOPS));
                            // 'UserList(UserIndex).Char.FX = FXIDs.FXMEDITARGRANDE

                            if (Declaraciones.Npclist[npcind].CanAttack == 1)
                            {
                                Declaraciones.Npclist[npcind].CanAttack = 0;
                                bs = Declaraciones.Npclist[npcind].Invent.BarcoSlot;
                                bs = Convert.ToByte(bs - 1);
                                MagoDestruyeWand(npcind, bs, DAT_APOCALIPSIS);
                                if (bs == 0)
                                {
                                    NPCs.MuereNpc(npcind, 0);
                                }
                                else
                                {
                                    Declaraciones.Npclist[npcind].Invent.BarcoSlot = bs;
                                }
                            }

                            break;
                        }
                }


                // 'movimiento (si puede)
                // 'El mago no se mueve a menos q tenga alguien al lado

                if (Declaraciones.Npclist[npcind].flags.Paralizado == 1)
                    return;

                if (!(quehacer == 3)) // 'si no ta matandose
                {
                    // 'alejarse si tiene un PJ cerca
                    // 'pero alejarse sin alejarse del rey
                    if (!(NPCPosM == MAPA_PRETORIANO))
                        return;

                    // 'Si no hay nadie cerca, o no tengo nada que hacer...
                    if (BestTarget == 0 & Declaraciones.Npclist[npcind].CanAttack == 1)
                        VolverAlCentro(npcind);

                    PJEnInd = Declaraciones.MapData[NPCPosM, NPCPosX - 1, NPCPosY].UserIndex;
                    if (PJEnInd > 0)
                    {
                        if (!(Declaraciones.UserList[PJEnInd].flags.Muerto == 1) & !(Declaraciones.UserList[PJEnInd].flags.invisible == 1 | Declaraciones.UserList[PJEnInd].flags.Oculto == 1))
                        {
                            // 'esta es una forma muy facil de matar 2 pajaros
                            // 'de un tiro. Se aleja del usuario pq el centro va a
                            // 'estar ocupado, y a la vez se aproxima al rey, manteniendo
                            // 'una linea de defensa compacta
                            VolverAlCentro(npcind);
                            return;
                        }
                    }

                    PJEnInd = Declaraciones.MapData[NPCPosM, NPCPosX + 1, NPCPosY].UserIndex;
                    if (PJEnInd > 0)
                    {
                        if (!(Declaraciones.UserList[PJEnInd].flags.Muerto == 1) & !(Declaraciones.UserList[PJEnInd].flags.invisible == 1 | Declaraciones.UserList[PJEnInd].flags.Oculto == 1))
                        {
                            VolverAlCentro(npcind);
                            return;
                        }
                    }

                    PJEnInd = Declaraciones.MapData[NPCPosM, NPCPosX, NPCPosY - 1].UserIndex;
                    if (PJEnInd > 0)
                    {
                        if (!(Declaraciones.UserList[PJEnInd].flags.Muerto == 1) & !(Declaraciones.UserList[PJEnInd].flags.invisible == 1 | Declaraciones.UserList[PJEnInd].flags.Oculto == 1))
                        {
                            VolverAlCentro(npcind);
                            return;
                        }
                    }

                    PJEnInd = Declaraciones.MapData[NPCPosM, NPCPosX, NPCPosY + 1].UserIndex;
                    if (PJEnInd > 0)
                    {
                        if (!(Declaraciones.UserList[PJEnInd].flags.Muerto == 1) & !(Declaraciones.UserList[PJEnInd].flags.invisible == 1 | Declaraciones.UserList[PJEnInd].flags.Oculto == 1))
                        {
                            VolverAlCentro(npcind);
                            return;
                        }
                    }


                } // 'end if not matandose
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in PRMAGO_AI: " + ex.Message);
                string argdesc = "Error en NPCAI.PRMAGO_AI? ";
                General.LogError(ref argdesc);
            }
        }

        public static void PRREY_AI(short npcind)
        {
            try
            {
                // HECHIZOS: NO CAMBIAR ACA
                // REPRESENTAN LA UBICACION DE LOS SPELLS EN NPC_HOSTILES.DAT y si se los puede cambiar en ese archivo
                // 1- CURAR_LEVES 'NO MODIFICABLE
                // 2- REMOVER PARALISIS 'NO MODIFICABLE
                // 3- CEUGERA - 'NO MODIFICABLE
                // 4- ESTUPIDEZ - 'NO MODIFICABLE
                // 5- CURARVENENO - 'NO MODIFICABLE
                short DAT_CURARLEVES;
                short DAT_REMUEVEPARALISIS;
                short DAT_CEGUERA;
                short DAT_ESTUPIDEZ;
                short DAT_CURARVENENO;
                DAT_CURARLEVES = 1;
                DAT_REMUEVEPARALISIS = 2;
                DAT_CEGUERA = 3;
                DAT_ESTUPIDEZ = 4;
                DAT_CURARVENENO = 5;


                short UI;
                short X;
                short Y;
                short NPCPosX;
                short NPCPosY;
                short NPCPosM;
                short NPCAlInd;
                short PJEnInd;
                short BestTarget;
                short distBestTarget;
                short dist;
                short e_p;
                bool hayPretorianos;
                Declaraciones.eHeading headingloop;
                Declaraciones.WorldPos nPos;
                // 'Dim quehacer As Integer
                // '1- remueve paralisis con un minimo % de efecto
                // '2- remueve veneno
                // '3- cura

                NPCPosM = Declaraciones.Npclist[npcind].Pos.Map;
                NPCPosX = Declaraciones.Npclist[npcind].Pos.X;
                NPCPosY = Declaraciones.Npclist[npcind].Pos.Y;
                BestTarget = 0;
                distBestTarget = 0;
                hayPretorianos = false;

                // pick the best target according to the following criteria:
                // King won't fight. Since praetorians' mission is to keep him alive
                // he will stay as far as possible from combat environment, but close enought
                // as to aid his loyal army.
                // If his army has been annihilated, the king will pick the
                // closest enemy an chase it using his special 'weapon speedhack' ability
                var loopTo = Convert.ToInt16(NPCPosX + 8);
                for (X = Convert.ToInt16(NPCPosX - 8); X <= loopTo; X++)
                {
                    var loopTo1 = Convert.ToInt16(NPCPosY + 7);
                    for (Y = Convert.ToInt16(NPCPosY - 7); Y <= loopTo1; Y++)
                    {
                        // scan combat field
                        NPCAlInd = Declaraciones.MapData[NPCPosM, X, Y].NpcIndex;
                        PJEnInd = Declaraciones.MapData[NPCPosM, X, Y].UserIndex;
                        if (Declaraciones.Npclist[npcind].CanAttack == 1) // 'saltea el analisis si no puede atacar para evitar cuentas
                        {
                            if (NPCAlInd > 0)
                            {
                                e_p = esPretoriano(NPCAlInd);
                                if (e_p > 0 & e_p < 6 & !(NPCAlInd == npcind))
                                {
                                    hayPretorianos = true;

                                    // Me curo mientras haya pretorianos (no es lo ideal, debería no dar experiencia tampoco, pero por ahora es lo que hay)
                                    Declaraciones.Npclist[npcind].Stats.MinHp = Declaraciones.Npclist[npcind].Stats.MaxHp;
                                }

                                if (Declaraciones.Npclist[NPCAlInd].flags.Paralizado == 1 & e_p > 0 & e_p < 6)
                                {
                                    // 'el rey puede desparalizar con una efectividad del 20%
                                    if (Matematicas.RandomNumber(1, 100) < 21)
                                    {
                                        NPCRemueveParalisisNPC(npcind, NPCAlInd, DAT_REMUEVEPARALISIS);
                                        Declaraciones.Npclist[npcind].CanAttack = 0;
                                        return;
                                    }
                                }

                                // 'failed to remove
                                else if (Declaraciones.Npclist[NPCAlInd].flags.Envenenado == 1) // 'un chiche :D
                                {
                                    if (esPretoriano(NPCAlInd) != 0)
                                    {
                                        NPCRemueveVenenoNPC(npcind, NPCAlInd, DAT_CURARVENENO);
                                        Declaraciones.Npclist[npcind].CanAttack = 0;
                                        return;
                                    }
                                }
                                else if (Declaraciones.Npclist[NPCAlInd].Stats.MaxHp > Declaraciones.Npclist[NPCAlInd].Stats.MinHp)
                                {
                                    if (esPretoriano(NPCAlInd) != 0 & !(NPCAlInd == npcind))
                                    {
                                        // 'cura, salvo q sea yo mismo. Eso lo hace 'despues'
                                        NPCCuraLevesNPC(npcind, NPCAlInd, DAT_CURARLEVES);
                                        Declaraciones.Npclist[npcind].CanAttack = 0;
                                        // 'Exit Sub
                                    }
                                }
                            }

                            if (PJEnInd > 0 & !hayPretorianos)
                            {
                                if (!(Declaraciones.UserList[PJEnInd].flags.Muerto == 1 | Declaraciones.UserList[PJEnInd].flags.invisible == 1 | Declaraciones.UserList[PJEnInd].flags.Oculto == 1 | Declaraciones.UserList[PJEnInd].flags.Ceguera == 1) & Declaraciones.UserList[PJEnInd].flags.AdminPerseguible)

                                {
                                    // 'si no esta muerto o invisible o ciego... o tiene el /ignorando
                                    dist = Convert.ToInt16(Math.Pow(Math.Sqrt(Declaraciones.UserList[PJEnInd].Pos.X - NPCPosX), 2d) + Math.Pow(Declaraciones.UserList[PJEnInd].Pos.Y - NPCPosY, 2d));
                                    if (dist < distBestTarget | BestTarget == 0)
                                    {
                                        BestTarget = PJEnInd;
                                        distBestTarget = dist;
                                    }
                                }
                            }
                        } // 'canattack = 1
                    }
                }

                if (!hayPretorianos)
                {
                    // 'si estoy aca es porque no hay pretorianos cerca!!!
                    // 'Todo mi ejercito fue asesinado
                    // 'Salgo a atacar a todos a lo loco a espadazos
                    if (BestTarget > 0)
                    {
                        if (EsAlcanzable(npcind, BestTarget))
                        {
                            GreedyWalkTo(npcind, Declaraciones.UserList[BestTarget].Pos.Map, Declaraciones.UserList[BestTarget].Pos.X, Declaraciones.UserList[BestTarget].Pos.Y);
                        }
                        // GreedyWalkTo npcind, UserList(BestTarget).Pos.Map, UserList(BestTarget).Pos.X, UserList(BestTarget).Pos.Y
                        else
                        {
                            // 'el chabon es piola y ataca desde lejos entonces lo castigamos!
                            NPCLanzaEstupidezPJ(npcind, BestTarget, DAT_ESTUPIDEZ);
                            NPCLanzaCegueraPJ(npcind, BestTarget, DAT_CEGUERA);
                        }

                        // 'heading loop de ataque
                        // 'teclavolaespada
                        for (headingloop = Declaraciones.eHeading.NORTH; headingloop <= Declaraciones.eHeading.WEST; headingloop++)
                        {
                            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                            nPos = Declaraciones.Npclist[npcind].Pos;
                            Extra.HeadtoPos(headingloop, ref nPos);
                            if (Extra.InMapBounds(nPos.Map, nPos.X, nPos.Y))
                            {
                                UI = Declaraciones.MapData[nPos.Map, nPos.X, nPos.Y].UserIndex;
                                if (UI > 0)
                                {
                                    if (SistemaCombate.NpcAtacaUser(npcind, UI))
                                    {
                                        NPCs.ChangeNPCChar(npcind, Declaraciones.Npclist[npcind].Char_Renamed.body, Declaraciones.Npclist[npcind].Char_Renamed.Head, headingloop);
                                    }

                                    // 'special speed ability for praetorian king ---------
                                    Declaraciones.Npclist[npcind].CanAttack = 1; // 'this is NOT a bug!!
                                                                                 // ----------------------------------------------------

                                }
                            }
                        }
                    }

                    else // 'no hay targets cerca
                    {
                        VolverAlCentro(npcind);
                        if (Declaraciones.Npclist[npcind].Stats.MinHp < Declaraciones.Npclist[npcind].Stats.MaxHp & Declaraciones.Npclist[npcind].CanAttack == 1)
                        {
                            // 'si no hay ndie y estoy daniado me curo
                            NPCCuraLevesNPC(npcind, npcind, DAT_CURARLEVES);
                            Declaraciones.Npclist[npcind].CanAttack = 0;
                        }

                    }
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in PRREY_AI: " + ex.Message);
                string argdesc = "Error en NPCAI.PRREY_AI? ";
                General.LogError(ref argdesc);
            }
        }

        public static void PRGUER_AI(short npcind)
        {
            try
            {

                Declaraciones.eHeading headingloop;
                Declaraciones.WorldPos nPos;
                short X;
                short Y;
                short dist;
                short distBestTarget;
                short NPCPosX;
                short NPCPosY;
                short NPCPosM;
                short NPCAlInd;
                short UI;
                short PJEnInd;
                short BestTarget;
                NPCPosM = Declaraciones.Npclist[npcind].Pos.Map;
                NPCPosX = Declaraciones.Npclist[npcind].Pos.X;
                NPCPosY = Declaraciones.Npclist[npcind].Pos.Y;
                BestTarget = 0;
                dist = 0;
                distBestTarget = 0;

                var loopTo = Convert.ToInt16(NPCPosX + 8);
                for (X = Convert.ToInt16(NPCPosX - 8); X <= loopTo; X++)
                {
                    var loopTo1 = Convert.ToInt16(NPCPosY + 7);
                    for (Y = Convert.ToInt16(NPCPosY - 7); Y <= loopTo1; Y++)
                    {
                        PJEnInd = Declaraciones.MapData[NPCPosM, X, Y].UserIndex;
                        if (PJEnInd > 0)
                        {
                            if (!(Declaraciones.UserList[PJEnInd].flags.invisible == 1 | Declaraciones.UserList[PJEnInd].flags.Oculto == 1 | Declaraciones.UserList[PJEnInd].flags.Muerto == 1) & EsAlcanzable(npcind, PJEnInd) & Declaraciones.UserList[PJEnInd].flags.AdminPerseguible)

                            {
                                // 'caluclo la distancia al PJ, si esta mas cerca q el actual
                                // 'mejor besttarget entonces ataco a ese.
                                if (BestTarget > 0)
                                {
                                    dist = Convert.ToInt16(Math.Pow(Math.Sqrt(Declaraciones.UserList[PJEnInd].Pos.X - NPCPosX), 2d) + Math.Pow(Declaraciones.UserList[PJEnInd].Pos.Y - NPCPosY, 2d));
                                    if (dist < distBestTarget)
                                    {
                                        BestTarget = PJEnInd;
                                        distBestTarget = dist;
                                    }
                                }
                                else
                                {
                                    distBestTarget = Convert.ToInt16(Math.Sqrt(Math.Pow(Declaraciones.UserList[PJEnInd].Pos.X - NPCPosX, 2d) + Math.Pow(Declaraciones.UserList[PJEnInd].Pos.Y - NPCPosY, 2d)));
                                    BestTarget = PJEnInd;
                                }
                            }
                        }
                    }
                }

                // 'LLamo a esta funcion si lo llevaron muy lejos.
                // 'La idea es que no lo "alejen" del rey y despues queden
                // 'lejos de la batalla cuando matan a un enemigo o este
                // 'sale del area de combate (tipica forma de separar un clan)
                if (Declaraciones.Npclist[npcind].flags.Paralizado == 0)
                {

                    // MEJORA: Si quedan solos, se van con el resto del ejercito
                    if (Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot != 0)
                    {
                        CambiarAlcoba(npcind);
                    }
                    // si me estoy yendo a alguna alcoba
                    else if (BestTarget == 0 | EstoyMuyLejos(npcind))
                    {
                        VolverAlCentro(npcind);
                    }
                    else if (BestTarget > 0)
                    {
                        GreedyWalkTo(npcind, Declaraciones.UserList[BestTarget].Pos.Map, Declaraciones.UserList[BestTarget].Pos.X, Declaraciones.UserList[BestTarget].Pos.Y);
                    }
                }

                // 'teclavolaespada
                for (headingloop = Declaraciones.eHeading.NORTH; headingloop <= Declaraciones.eHeading.WEST; headingloop++)
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto nPos. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    nPos = Declaraciones.Npclist[npcind].Pos;
                    Extra.HeadtoPos(headingloop, ref nPos);
                    if (Extra.InMapBounds(nPos.Map, nPos.X, nPos.Y))
                    {
                        UI = Declaraciones.MapData[nPos.Map, nPos.X, nPos.Y].UserIndex;
                        if (UI > 0)
                        {
                            if (!(Declaraciones.UserList[UI].flags.Muerto == 1))
                            {
                                if (SistemaCombate.NpcAtacaUser(npcind, UI))
                                {
                                    NPCs.ChangeNPCChar(npcind, Declaraciones.Npclist[npcind].Char_Renamed.body, Declaraciones.Npclist[npcind].Char_Renamed.Head, headingloop);
                                }
                                Declaraciones.Npclist[npcind].CanAttack = 0;
                            }
                        }
                    }
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in PRGUER_AI: " + ex.Message);
                string argdesc = "Error en NPCAI.PRGUER_AI? ";
                General.LogError(ref argdesc);
            }
        }

        public static void PRCLER_AI(short npcind)
        {
            try
            {

                // HECHIZOS: NO CAMBIAR ACA
                // REPRESENTAN LA UBICACION DE LOS SPELLS EN NPC_HOSTILES.DAT y si se los puede cambiar en ese archivo
                // 1- PARALIZAR PJS 'MODIFICABLE
                // 2- REMOVER PARALISIS 'NO MODIFICABLE
                // 3- CURARGRAVES - 'NO MODIFICABLE
                // 4- PARALIZAR MASCOTAS - 'NO MODIFICABLE
                // 5- CURARVENENO - 'NO MODIFICABLE
                short DAT_PARALIZARPJ;
                short DAT_REMUEVEPARALISIS;
                short DAT_CURARGRAVES;
                short DAT_PARALIZAR_NPC;
                short DAT_TORMENTAAVANZADA;
                DAT_PARALIZARPJ = 1;
                DAT_REMUEVEPARALISIS = 2;
                DAT_PARALIZAR_NPC = 3;
                DAT_CURARGRAVES = 4;
                DAT_TORMENTAAVANZADA = 5;

                short X;
                short Y;
                short NPCPosX;
                short NPCPosY;
                short NPCPosM;
                short NPCAlInd;
                short PJEnInd;
                short centroX;
                short centroY;
                short BestTarget;
                bool PJBestTarget;
                short azar;
                short azar2;
                byte quehacer;
                // '1- paralizar enemigo,
                // '2- bombardear enemigo
                // '3- ataque a mascotas
                // '4- curar aliado
                quehacer = 0;
                NPCPosM = Declaraciones.Npclist[npcind].Pos.Map;
                NPCPosX = Declaraciones.Npclist[npcind].Pos.X;
                NPCPosY = Declaraciones.Npclist[npcind].Pos.Y;
                PJBestTarget = false;
                BestTarget = 0;

                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto azar. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                azar = Convert.ToInt16(Math.Sign(Matematicas.RandomNumber(-1, 1)));
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto azar. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                if (azar == 0)
                    azar = 1;
                azar2 = Convert.ToInt16(Math.Sign(Matematicas.RandomNumber(-1, 1)));
                if (azar2 == 0)
                    azar2 = 1;

                // pick the best target according to the following criteria:
                // 1) "hoaxed" friends MUST be released
                // 2) enemy shall be annihilated no matter what
                // 3) party healing if no threats
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto azar. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                var loopTo = Convert.ToInt16(NPCPosX + azar * -8);
                for (X = Convert.ToInt16(NPCPosX + azar * 8); -azar >= 0 ? X <= loopTo : X >= loopTo; X += -azar)
                {
                    var loopTo1 = Convert.ToInt16(NPCPosY + azar2 * -7);
                    for (Y = Convert.ToInt16(NPCPosY + azar2 * 7); -azar2 >= 0 ? Y <= loopTo1 : Y >= loopTo1; Y += -azar2)
                    {
                        // scan combat field
                        NPCAlInd = Declaraciones.MapData[NPCPosM, X, Y].NpcIndex;
                        PJEnInd = Declaraciones.MapData[NPCPosM, X, Y].UserIndex;
                        if (Declaraciones.Npclist[npcind].CanAttack == 1) // 'saltea el analisis si no puede atacar para evitar cuentas
                        {
                            if (NPCAlInd > 0) // 'allie?
                            {
                                if (esPretoriano(NPCAlInd) == 0)
                                {
                                    if (Declaraciones.Npclist[NPCAlInd].MaestroUser > 0 & !(Declaraciones.Npclist[NPCAlInd].flags.Paralizado > 0))

                                    {
                                        NPCparalizaNPC(npcind, NPCAlInd, DAT_PARALIZAR_NPC);
                                        Declaraciones.Npclist[npcind].CanAttack = 0;
                                        return;
                                    }
                                }
                                else if (Declaraciones.Npclist[NPCAlInd].flags.Paralizado == 1) // es un PJ aliado en combate
                                {
                                    // amigo paralizado, an hoax vorp YA
                                    NPCRemueveParalisisNPC(npcind, NPCAlInd, DAT_REMUEVEPARALISIS);
                                    Declaraciones.Npclist[npcind].CanAttack = 0;
                                    return;
                                }
                                else if (BestTarget == 0) // 'si no tiene nada q hacer..
                                {
                                    if (Declaraciones.Npclist[NPCAlInd].Stats.MaxHp > Declaraciones.Npclist[NPCAlInd].Stats.MinHp)
                                    {
                                        BestTarget = NPCAlInd; // 'cura heridas
                                        PJBestTarget = false;
                                        quehacer = 4;
                                    }
                                }
                            }
                            else if (PJEnInd > 0) // 'aggressor
                            {
                                if (!(Declaraciones.UserList[PJEnInd].flags.Muerto == 1) & Declaraciones.UserList[PJEnInd].flags.AdminPerseguible)
                                {
                                    if (Declaraciones.UserList[PJEnInd].flags.Paralizado == 0)
                                    {
                                        if (!(Declaraciones.UserList[PJEnInd].flags.invisible == 1 | Declaraciones.UserList[PJEnInd].flags.Oculto == 1))


                                        {
                                            // 'PJ movil y visible, jeje, si o si es target
                                            BestTarget = PJEnInd;
                                            PJBestTarget = true;
                                            quehacer = 1;
                                        }
                                    }
                                    else if (!(BestTarget > 0) | !PJBestTarget) // 'PJ paralizado, ataca este invisible o no
                                                                                // 'a menos q tenga algo mejor
                                    {
                                        BestTarget = PJEnInd;
                                        PJBestTarget = true;
                                        quehacer = 2;
                                    } // 'endif paralizado
                                } // 'end if not muerto
                            } // 'listo el analisis del tile
                        }
                        // 'saltea el analisis si no puede atacar, en realidad no es lo "mejor" pero evita cuentas inútiles
                    }
                }

                // 'aqui (si llego) tiene el mejor target
                switch (quehacer)
                {
                    case 0:
                        {
                            // 'nada que hacer. Buscar mas alla del campo de visión algun aliado, a menos
                            // 'que este paralizado pq no puedo ir
                            if (Declaraciones.Npclist[npcind].flags.Paralizado == 1)
                                return;

                            if (!(NPCPosM == MAPA_PRETORIANO))
                                return;

                            if (NPCPosX < 50)
                                centroX = ALCOBA1_X;
                            else
                                centroX = ALCOBA2_X;
                            centroY = ALCOBA1_Y;
                            // 'aca establecí el lugar de las alcobas

                            // 'Este doble for busca amigos paralizados lejos para ir a rescatarlos
                            // 'Entra aca solo si en el area cercana al rey no hay algo mejor que
                            // 'hacer.
                            var loopTo2 = Convert.ToInt16(centroX + 16);
                            for (X = Convert.ToInt16(centroX - 16); X <= loopTo2; X++)
                            {
                                var loopTo3 = Convert.ToInt16(centroY + 15);
                                for (Y = Convert.ToInt16(centroY - 15); Y <= loopTo3; Y++)
                                {
                                    if (!(X < NPCPosX + 8 & X > NPCPosX + 8 & Y < NPCPosY + 7 & Y > NPCPosY - 7))
                                    {
                                        // 'si no es un tile ya analizado... (evito cuentas)
                                        NPCAlInd = Declaraciones.MapData[NPCPosM, X, Y].NpcIndex;
                                        if (NPCAlInd > 0)
                                        {
                                            if (esPretoriano(NPCAlInd) > 0 & Declaraciones.Npclist[NPCAlInd].flags.Paralizado == 1)
                                            {
                                                // 'si esta paralizado lo va a rescatar, sino
                                                // 'ya va a volver por su cuenta
                                                GreedyWalkTo(npcind, NPCPosM, Declaraciones.Npclist[NPCAlInd].Pos.X, Declaraciones.Npclist[NPCAlInd].Pos.Y);
                                                // GreedyWalkTo npcind, NPCPosM, Npclist(NPCAlInd).Pos.X, Npclist(NPCAlInd).Pos.Y
                                                return;
                                            }
                                        } // 'endif npc
                                    } // 'endif tile analizado
                                }
                            }

                            // 'si estoy aca esta totalmente al cuete el clerigo o mal posicionado por rescate anterior
                            if (Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot == 0)
                            {
                                VolverAlCentro(npcind);
                                return;
                            }

                            break;
                        }
                    // 'fin quehacer = 0 (npc al cuete)

                    case 1: // ' paralizar enemigo PJ
                        {
                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead(Declaraciones.Hechizos[Declaraciones.Npclist[npcind].Spells[DAT_PARALIZARPJ]].PalabrasMagicas, Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Cyan)));
                            modHechizos.NpcLanzaSpellSobreUser(npcind, BestTarget, Declaraciones.Npclist[npcind].Spells[DAT_PARALIZARPJ]);
                            // 'SPELL 1 de Clerigo es PARALIZAR
                            return;
                        }
                    case 2: // ' ataque a usuarios (invisibles tambien)
                        {
                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead(Declaraciones.Hechizos[Declaraciones.Npclist[npcind].Spells[DAT_TORMENTAAVANZADA]].PalabrasMagicas, Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Cyan)));
                            NpcLanzaSpellSobreUser2(npcind, BestTarget, Declaraciones.Npclist[npcind].Spells[DAT_TORMENTAAVANZADA]);
                            // 'SPELL 2 de Clerigo es Vax On Tar avanzado
                            return;
                        }
                    case 3: // ' ataque a mascotas
                        {
                            if (!(Declaraciones.Npclist[BestTarget].flags.Paralizado == 1))
                            {
                                NPCparalizaNPC(npcind, BestTarget, DAT_PARALIZAR_NPC);
                                Declaraciones.Npclist[npcind].CanAttack = 0;
                            } // 'TODO: vax on tar sobre mascotas

                            break;
                        }
                    case 4: // ' party healing
                        {
                            NPCcuraNPC(npcind, BestTarget, DAT_CURARGRAVES);
                            Declaraciones.Npclist[npcind].CanAttack = 0;
                            break;
                        }
                }


                // 'movimientos
                // 'EL clerigo no tiene un movimiento fijo, pero es esperable
                // 'que no se aleje mucho del rey... y si se aleje de espaderos

                if (Declaraciones.Npclist[npcind].flags.Paralizado == 1)
                    return;

                if (!(NPCPosM == MAPA_PRETORIANO))
                    return;

                // MEJORA: Si quedan solos, se van con el resto del ejercito
                if (Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot != 0)
                {
                    CambiarAlcoba(npcind);
                    return;
                }


                PJEnInd = Declaraciones.MapData[NPCPosM, NPCPosX - 1, NPCPosY].UserIndex;
                if (PJEnInd > 0)
                {
                    if (!(Declaraciones.UserList[PJEnInd].flags.Muerto == 1) & !(Declaraciones.UserList[PJEnInd].flags.invisible == 1 | Declaraciones.UserList[PJEnInd].flags.Oculto == 1))
                    {
                        // 'esta es una forma muy facil de matar 2 pajaros
                        // 'de un tiro. Se aleja del usuario pq el centro va a
                        // 'estar ocupado, y a la vez se aproxima al rey, manteniendo
                        // 'una linea de defensa compacta
                        VolverAlCentro(npcind);
                        return;
                    }
                }

                PJEnInd = Declaraciones.MapData[NPCPosM, NPCPosX + 1, NPCPosY].UserIndex;
                if (PJEnInd > 0)
                {
                    if (!(Declaraciones.UserList[PJEnInd].flags.Muerto == 1) & !(Declaraciones.UserList[PJEnInd].flags.invisible == 1 | Declaraciones.UserList[PJEnInd].flags.Oculto == 1))
                    {
                        VolverAlCentro(npcind);
                        return;
                    }
                }

                PJEnInd = Declaraciones.MapData[NPCPosM, NPCPosX, NPCPosY - 1].UserIndex;
                if (PJEnInd > 0)
                {
                    if (!(Declaraciones.UserList[PJEnInd].flags.Muerto == 1) & !(Declaraciones.UserList[PJEnInd].flags.invisible == 1 | Declaraciones.UserList[PJEnInd].flags.Oculto == 1))
                    {
                        VolverAlCentro(npcind);
                        return;
                    }
                }

                PJEnInd = Declaraciones.MapData[NPCPosM, NPCPosX, NPCPosY + 1].UserIndex;
                if (PJEnInd > 0)
                {
                    if (!(Declaraciones.UserList[PJEnInd].flags.Muerto == 1) & !(Declaraciones.UserList[PJEnInd].flags.invisible == 1 | Declaraciones.UserList[PJEnInd].flags.Oculto == 1))
                    {
                        VolverAlCentro(npcind);
                        return;
                    }
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in PRCLER_AI: " + ex.Message);
                string argdesc = "Error en NPCAI.PRCLER_AI? ";
                General.LogError(ref argdesc);
            }
        }

        public static bool EsMagoOClerigo(short PJEnInd)
        {
            bool EsMagoOClerigoRet = default;
            try
            {

                EsMagoOClerigoRet = Declaraciones.UserList[PJEnInd].clase == Declaraciones.eClass.Mage | Declaraciones.UserList[PJEnInd].clase == Declaraciones.eClass.Cleric | Declaraciones.UserList[PJEnInd].clase == Declaraciones.eClass.Druid | Declaraciones.UserList[PJEnInd].clase == Declaraciones.eClass.Bard;
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in EsMagoOClerigo: " + ex.Message);
                string argdesc = "Error en NPCAI.EsMagoOClerigo? ";
                General.LogError(ref argdesc);
            }

            return EsMagoOClerigoRet;
        }

        public static void NPCRemueveVenenoNPC(short npcind, short NPCAlInd, short indice)
        {
            try
            {
                short indireccion;

                indireccion = Declaraciones.Npclist[npcind].Spells[indice];
                // ' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead(Declaraciones.Hechizos[indireccion].PalabrasMagicas, Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Cyan)));
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, NPCAlInd, Protocol.PrepareMessageCreateFX(Declaraciones.Npclist[NPCAlInd].Char_Renamed.CharIndex, Declaraciones.Hechizos[indireccion].FXgrh, Declaraciones.Hechizos[indireccion].loops));
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, NPCAlInd, Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[indireccion].WAV), Convert.ToByte(Declaraciones.Npclist[NPCAlInd].Pos.X), Convert.ToByte(Declaraciones.Npclist[NPCAlInd].Pos.Y)));
                Declaraciones.Npclist[NPCAlInd].Veneno = 0;
                Declaraciones.Npclist[NPCAlInd].flags.Envenenado = 0;
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in NPCRemueveVenenoNPC: " + ex.Message);
                string argdesc = "Error en NPCAI.NPCRemueveVenenoNPC? ";
                General.LogError(ref argdesc);
            }
        }

        public static void NPCCuraLevesNPC(short npcind, short NPCAlInd, short indice)
        {
            try
            {
                short indireccion;

                indireccion = Declaraciones.Npclist[npcind].Spells[indice];
                // ' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead(Declaraciones.Hechizos[indireccion].PalabrasMagicas, Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Cyan)));
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, NPCAlInd, Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[indireccion].WAV), Convert.ToByte(Declaraciones.Npclist[NPCAlInd].Pos.X), Convert.ToByte(Declaraciones.Npclist[NPCAlInd].Pos.Y)));
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, NPCAlInd, Protocol.PrepareMessageCreateFX(Declaraciones.Npclist[NPCAlInd].Char_Renamed.CharIndex, Declaraciones.Hechizos[indireccion].FXgrh, Declaraciones.Hechizos[indireccion].loops));

                if (Declaraciones.Npclist[NPCAlInd].Stats.MinHp + 5 < Declaraciones.Npclist[NPCAlInd].Stats.MaxHp)
                {
                    Declaraciones.Npclist[NPCAlInd].Stats.MinHp = Declaraciones.Npclist[NPCAlInd].Stats.MinHp + 5;
                }
                else
                {
                    Declaraciones.Npclist[NPCAlInd].Stats.MinHp = Declaraciones.Npclist[NPCAlInd].Stats.MaxHp;
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in NPCCuraLevesNPC: " + ex.Message);
                string argdesc = "Error en NPCAI.NPCCuraLevesNPC? ";
                General.LogError(ref argdesc);
            }
        }


        public static void NPCRemueveParalisisNPC(short npcind, short NPCAlInd, short indice)
        {
            try
            {
                short indireccion;

                indireccion = Declaraciones.Npclist[npcind].Spells[indice];
                // ' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead(Declaraciones.Hechizos[indireccion].PalabrasMagicas, Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Cyan)));
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, NPCAlInd, Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[indireccion].WAV), Convert.ToByte(Declaraciones.Npclist[NPCAlInd].Pos.X), Convert.ToByte(Declaraciones.Npclist[NPCAlInd].Pos.Y)));
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, NPCAlInd, Protocol.PrepareMessageCreateFX(Declaraciones.Npclist[NPCAlInd].Char_Renamed.CharIndex, Declaraciones.Hechizos[indireccion].FXgrh, Declaraciones.Hechizos[indireccion].loops));
                Declaraciones.Npclist[NPCAlInd].Contadores.Paralisis = 0;
                Declaraciones.Npclist[NPCAlInd].flags.Paralizado = 0;
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in NPCRemueveParalisisNPC: " + ex.Message);
                string argdesc = "Error en NPCAI.NPCRemueveParalisisNPC? ";
                General.LogError(ref argdesc);
            }
        }

        public static void NPCparalizaNPC(short paralizador, short Paralizado, short indice)
        {
            try
            {
                short indireccion;

                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto indice. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                indireccion = Declaraciones.Npclist[paralizador].Spells[indice];
                // ' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, paralizador, Protocol.PrepareMessageChatOverHead(Declaraciones.Hechizos[indireccion].PalabrasMagicas, Declaraciones.Npclist[paralizador].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Cyan)));
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, Paralizado, Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[indireccion].WAV), Convert.ToByte(Declaraciones.Npclist[Paralizado].Pos.X), Convert.ToByte(Declaraciones.Npclist[Paralizado].Pos.Y)));
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, Paralizado, Protocol.PrepareMessageCreateFX(Declaraciones.Npclist[Paralizado].Char_Renamed.CharIndex, Declaraciones.Hechizos[indireccion].FXgrh, Declaraciones.Hechizos[indireccion].loops));

                Declaraciones.Npclist[Paralizado].flags.Paralizado = 1;
                Declaraciones.Npclist[Paralizado].Contadores.Paralisis = Convert.ToInt16(Admin.IntervaloParalizado * 2);
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in NPCparalizaNPC: " + ex.Message);
                string argdesc = "Error en NPCAI.NPCParalizaNPC? ";
                General.LogError(ref argdesc);
            }
        }

        public static void NPCcuraNPC(short curador, short curado, short indice)
        {
            try
            {
                short indireccion;


                indireccion = Declaraciones.Npclist[curador].Spells[indice];
                // ' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, curador, Protocol.PrepareMessageChatOverHead(Declaraciones.Hechizos[indireccion].PalabrasMagicas, Declaraciones.Npclist[curador].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Cyan)));
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, curado, Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[indireccion].WAV), Convert.ToByte(Declaraciones.Npclist[curado].Pos.X), Convert.ToByte(Declaraciones.Npclist[curado].Pos.Y)));
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, curado, Protocol.PrepareMessageCreateFX(Declaraciones.Npclist[curado].Char_Renamed.CharIndex, Declaraciones.Hechizos[indireccion].FXgrh, Declaraciones.Hechizos[indireccion].loops));
                if (Declaraciones.Npclist[curado].Stats.MinHp + 30 > Declaraciones.Npclist[curado].Stats.MaxHp)
                {
                    Declaraciones.Npclist[curado].Stats.MinHp = Declaraciones.Npclist[curado].Stats.MaxHp;
                }
                else
                {
                    Declaraciones.Npclist[curado].Stats.MinHp = Declaraciones.Npclist[curado].Stats.MinHp + 30;
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in NPCcuraNPC: " + ex.Message);
                string argdesc = "Error en NPCAI.NPCcuraNPC? ";
                General.LogError(ref argdesc);
            }
        }

        public static void NPCLanzaCegueraPJ(short npcind, short PJEnInd, short indice)
        {
            try
            {
                short indireccion;

                indireccion = Declaraciones.Npclist[npcind].Spells[indice];
                // ' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead(Declaraciones.Hechizos[indireccion].PalabrasMagicas, Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Cyan)));
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, PJEnInd, Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[indireccion].WAV), Convert.ToByte(Declaraciones.UserList[PJEnInd].Pos.X), Convert.ToByte(Declaraciones.UserList[PJEnInd].Pos.Y)));
                modSendData.SendData(modSendData.SendTarget.ToPCArea, PJEnInd, Protocol.PrepareMessageCreateFX(Declaraciones.UserList[PJEnInd].Char_Renamed.CharIndex, Declaraciones.Hechizos[indireccion].FXgrh, Declaraciones.Hechizos[indireccion].loops));

                Declaraciones.UserList[PJEnInd].flags.Ceguera = 1;
                Declaraciones.UserList[PJEnInd].Counters.Ceguera = Convert.ToInt16(Admin.IntervaloInvisible);
                // 'Envia ceguera
                Protocol.WriteBlind(PJEnInd);
                // 'bardea si es el rey
                if (Declaraciones.Npclist[npcind].name == "Rey Pretoriano")
                {
                    Protocol.WriteConsoleMsg(PJEnInd, "El rey pretoriano te ha vuelto ciego ", Protocol.FontTypeNames.FONTTYPE_FIGHT);
                    Protocol.WriteConsoleMsg(PJEnInd, "A la distancia escuchas las siguientes palabras: ¡Cobarde, no eres digno de luchar conmigo si escapas! ", Protocol.FontTypeNames.FONTTYPE_VENENO);
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in NPCLanzaCegueraPJ: " + ex.Message);
                string argdesc = "Error en NPCAI.NPCLanzaCegueraPJ? ";
                General.LogError(ref argdesc);
            }
        }

        public static void NPCLanzaEstupidezPJ(short npcind, short PJEnInd, short indice)
        {
            try
            {
                short indireccion;


                indireccion = Declaraciones.Npclist[npcind].Spells[indice];
                // ' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead(Declaraciones.Hechizos[indireccion].PalabrasMagicas, Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Cyan)));
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, PJEnInd, Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[indireccion].WAV), Convert.ToByte(Declaraciones.UserList[PJEnInd].Pos.X), Convert.ToByte(Declaraciones.UserList[PJEnInd].Pos.Y)));
                modSendData.SendData(modSendData.SendTarget.ToPCArea, PJEnInd, Protocol.PrepareMessageCreateFX(Declaraciones.UserList[PJEnInd].Char_Renamed.CharIndex, Declaraciones.Hechizos[indireccion].FXgrh, Declaraciones.Hechizos[indireccion].loops));
                Declaraciones.UserList[PJEnInd].flags.Estupidez = 1;
                Declaraciones.UserList[PJEnInd].Counters.Estupidez = Convert.ToInt16(Admin.IntervaloInvisible);
                // manda estupidez
                Protocol.WriteDumb(PJEnInd);

                // bardea si es el rey
                if (Declaraciones.Npclist[npcind].name == "Rey Pretoriano")
                {
                    Protocol.WriteConsoleMsg(PJEnInd, "El rey pretoriano te ha vuelto estúpido.", Protocol.FontTypeNames.FONTTYPE_FIGHT);
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in NPCLanzaEstupidezPJ: " + ex.Message);
                string argdesc = "Error en NPCAI.NPCLanzaEstupidezPJ? ";
                General.LogError(ref argdesc);
            }
        }

        public static void NPCRemueveInvisibilidad(short npcind, short PJEnInd, short indice)
        {
            try
            {
                short indireccion;

                indireccion = Declaraciones.Npclist[npcind].Spells[indice];
                // ' Envia las palabras magicas, fx y wav del indice-esimo hechizo del npc-hostiles.dat
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead(Declaraciones.Hechizos[indireccion].PalabrasMagicas, Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Cyan)));
                modSendData.SendData(modSendData.SendTarget.ToNPCArea, PJEnInd, Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[indireccion].WAV), Convert.ToByte(Declaraciones.UserList[PJEnInd].Pos.X), Convert.ToByte(Declaraciones.UserList[PJEnInd].Pos.Y)));
                modSendData.SendData(modSendData.SendTarget.ToPCArea, PJEnInd, Protocol.PrepareMessageCreateFX(Declaraciones.UserList[PJEnInd].Char_Renamed.CharIndex, Declaraciones.Hechizos[indireccion].FXgrh, Declaraciones.Hechizos[indireccion].loops));

                // Sacamos el efecto de ocultarse
                if (Declaraciones.UserList[PJEnInd].flags.Oculto == 1)
                {
                    Declaraciones.UserList[PJEnInd].Counters.TiempoOculto = 0;
                    Declaraciones.UserList[PJEnInd].flags.Oculto = 0;
                    UsUaRiOs.SetInvisible(PJEnInd, Declaraciones.UserList[PJEnInd].Char_Renamed.CharIndex, false);
                    // Call SendData(SendTarget.ToPCArea, PJEnInd, PrepareMessageSetInvisible(UserList(PJEnInd).Char.CharIndex, False))
                    Protocol.WriteConsoleMsg(PJEnInd, "¡Has sido detectado!", Protocol.FontTypeNames.FONTTYPE_VENENO);
                }
                else
                {
                    // sino, solo lo "iniciamos" en la sacada de invisibilidad.
                    Protocol.WriteConsoleMsg(PJEnInd, "Comienzas a hacerte visible.", Protocol.FontTypeNames.FONTTYPE_VENENO);
                    Declaraciones.UserList[PJEnInd].Counters.Invisibilidad = Convert.ToInt16(Admin.IntervaloInvisible - 1);
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in NPCRemueveInvisibilidad: " + ex.Message);
                string argdesc = "Error en NPCAI.NPCRemueveInvisibilidad ";
                General.LogError(ref argdesc);
            }
        }

        public static void NpcLanzaSpellSobreUser2(short NpcIndex, short UserIndex, short Spell)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: 05/09/09
            // 05/09/09: Pato - Ahora actualiza la vida del usuario atacado
            // ***************************************************
            try
            {
                // '  Igual a la otra pero ataca invisibles!!!
                // ' (malditos controles de casos imposibles...)

                if (Declaraciones.Npclist[NpcIndex].CanAttack == 0)
                    return;
                // If UserList(UserIndex).Flags.Invisible = 1 Then Exit Sub

                Declaraciones.Npclist[NpcIndex].CanAttack = 0;
                short daño;

                if (Declaraciones.Hechizos[Spell].SubeHP == 1)
                {

                    daño = Convert.ToInt16(Matematicas.RandomNumber(Declaraciones.Hechizos[Spell].MinHp, Declaraciones.Hechizos[Spell].MaxHp));
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex, Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[Spell].WAV), Convert.ToByte(Declaraciones.UserList[UserIndex].Pos.X), Convert.ToByte(Declaraciones.UserList[UserIndex].Pos.Y)));
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex, Protocol.PrepareMessageCreateFX(Declaraciones.UserList[UserIndex].Char_Renamed.CharIndex, Declaraciones.Hechizos[Spell].FXgrh, Declaraciones.Hechizos[Spell].loops));

                    Declaraciones.UserList[UserIndex].Stats.MinHp = (short)(Declaraciones.UserList[UserIndex].Stats.MinHp + daño);
                    if (Declaraciones.UserList[UserIndex].Stats.MinHp > Declaraciones.UserList[UserIndex].Stats.MaxHp)
                        Declaraciones.UserList[UserIndex].Stats.MinHp = Declaraciones.UserList[UserIndex].Stats.MaxHp;

                    Protocol.WriteConsoleMsg(UserIndex, Declaraciones.Npclist[NpcIndex].name + " te ha quitado " + daño + " puntos de vida.", Protocol.FontTypeNames.FONTTYPE_FIGHT);

                    Protocol.WriteUpdateHP(UserIndex);
                }
                else if (Declaraciones.Hechizos[Spell].SubeHP == 2)
                {

                    daño = Convert.ToInt16(Matematicas.RandomNumber(Declaraciones.Hechizos[Spell].MinHp, Declaraciones.Hechizos[Spell].MaxHp));
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex, Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[Spell].WAV), Convert.ToByte(Declaraciones.UserList[UserIndex].Pos.X), Convert.ToByte(Declaraciones.UserList[UserIndex].Pos.Y)));
                    modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex, Protocol.PrepareMessageCreateFX(Declaraciones.UserList[UserIndex].Char_Renamed.CharIndex, Declaraciones.Hechizos[Spell].FXgrh, Declaraciones.Hechizos[Spell].loops));

                    if ((Declaraciones.UserList[UserIndex].flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                        Declaraciones.UserList[UserIndex].Stats.MinHp = (short)(Declaraciones.UserList[UserIndex].Stats.MinHp - daño);

                    Protocol.WriteConsoleMsg(UserIndex, Declaraciones.Npclist[NpcIndex].name + " te ha quitado " + daño + " puntos de vida.", Protocol.FontTypeNames.FONTTYPE_FIGHT);

                    // Muere
                    if (Declaraciones.UserList[UserIndex].Stats.MinHp < 1)
                    {
                        Declaraciones.UserList[UserIndex].Stats.MinHp = 0;
                        UsUaRiOs.UserDie(UserIndex);
                    }

                    Protocol.WriteUpdateHP(UserIndex);
                }

                if (Declaraciones.Hechizos[Spell].Paraliza == 1)
                {
                    if (Declaraciones.UserList[UserIndex].flags.Paralizado == 0)
                    {
                        Declaraciones.UserList[UserIndex].flags.Paralizado = 1;
                        Declaraciones.UserList[UserIndex].Counters.Paralisis = Convert.ToInt16(Admin.IntervaloParalizado);
                        modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex, Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[Spell].WAV), Convert.ToByte(Declaraciones.UserList[UserIndex].Pos.X), Convert.ToByte(Declaraciones.UserList[UserIndex].Pos.Y)));
                        modSendData.SendData(modSendData.SendTarget.ToPCArea, UserIndex, Protocol.PrepareMessageCreateFX(Declaraciones.UserList[UserIndex].Char_Renamed.CharIndex, Declaraciones.Hechizos[Spell].FXgrh, Declaraciones.Hechizos[Spell].loops));

                        Protocol.WriteParalizeOK(UserIndex);

                    }
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in NpcLanzaSpellSobreUser2: " + ex.Message);
                string argdesc = "Error en NPCAI.NPCLanzaSpellSobreUser2 ";
                General.LogError(ref argdesc);
            }
        }


        public static void MagoDestruyeWand(short npcind, byte bs, short indice)
        {
            try
            {
                // 'sonidos: 30 y 32, y no los cambien sino termina siendo muy chistoso...
                // 'Para el FX utiliza el del hechizos(indice)
                short X;
                short Y;
                short PJInd;
                short NPCPosX;
                short NPCPosY;
                short NPCPosM;
                double danio;
                double dist;
                short danioI;
                short MascotaInd;
                short indireccion;

                switch (bs)
                {
                    case 5:
                        {
                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead("Rahma", Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Lime)));
                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessagePlayWave(Convert.ToByte(SONIDO_Dragon_VIVO), Convert.ToByte(Declaraciones.Npclist[npcind].Pos.X), Convert.ToByte(Declaraciones.Npclist[npcind].Pos.Y)));
                            break;
                        }
                    case 4:
                        {
                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead("vôrtax", Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Lime)));
                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessagePlayWave(Convert.ToByte(SONIDO_Dragon_VIVO), Convert.ToByte(Declaraciones.Npclist[npcind].Pos.X), Convert.ToByte(Declaraciones.Npclist[npcind].Pos.Y)));
                            break;
                        }
                    case 3:
                        {
                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead("Zill", Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Lime)));
                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessagePlayWave(Convert.ToByte(SONIDO_Dragon_VIVO), Convert.ToByte(Declaraciones.Npclist[npcind].Pos.X), Convert.ToByte(Declaraciones.Npclist[npcind].Pos.Y)));
                            break;
                        }
                    case 2:
                        {
                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead("yäkà E'nta", Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Lime)));
                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessagePlayWave(Convert.ToByte(SONIDO_Dragon_VIVO), Convert.ToByte(Declaraciones.Npclist[npcind].Pos.X), Convert.ToByte(Declaraciones.Npclist[npcind].Pos.Y)));
                            break;
                        }
                    case 1:
                        {
                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead("¡¡Koràtá!!", Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Lime)));
                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessagePlayWave(Convert.ToByte(SONIDO_Dragon_VIVO), Convert.ToByte(Declaraciones.Npclist[npcind].Pos.X), Convert.ToByte(Declaraciones.Npclist[npcind].Pos.Y)));
                            break;
                        }
                    case 0:
                        {
                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead(Constants.vbNullString, Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Lime)));
                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessagePlayWave(Convert.ToByte(SONIDO_Dragon_VIVO), Convert.ToByte(Declaraciones.Npclist[npcind].Pos.X), Convert.ToByte(Declaraciones.Npclist[npcind].Pos.Y)));
                            NPCPosX = Declaraciones.Npclist[npcind].Pos.X;
                            NPCPosY = Declaraciones.Npclist[npcind].Pos.Y;
                            NPCPosM = Declaraciones.Npclist[npcind].Pos.Map;
                            PJInd = 0;
                            indireccion = Declaraciones.Npclist[npcind].Spells[indice];
                            // 'Daño masivo por destruccion de wand
                            for (X = 8; X <= 95; X++)
                            {
                                for (Y = 8; Y <= 95; Y++)
                                {
                                    PJInd = Declaraciones.MapData[NPCPosM, X, Y].UserIndex;
                                    MascotaInd = Declaraciones.MapData[NPCPosM, X, Y].NpcIndex;
                                    if (PJInd > 0)
                                    {
                                        dist = Convert.ToInt16(Math.Pow(Math.Sqrt(Declaraciones.UserList[PJInd].Pos.X - NPCPosX), 2d) + Math.Pow(Declaraciones.UserList[PJInd].Pos.Y - NPCPosY, 2d));
                                        danio = 880d / Math.Pow(dist, 3d / 7d);
                                        danioI = Convert.ToInt16(Math.Abs(Conversion.Int(danio)));
                                        // 'efectiviza el danio
                                        if ((Declaraciones.UserList[PJInd].flags.Privilegios & Declaraciones.PlayerType.User) != 0)
                                            Declaraciones.UserList[PJInd].Stats.MinHp = (short)(Declaraciones.UserList[PJInd].Stats.MinHp - danioI);

                                        Protocol.WriteConsoleMsg(PJInd, Declaraciones.Npclist[npcind].name + " te ha quitado " + danioI + " puntos de vida al romper su vara.", Protocol.FontTypeNames.FONTTYPE_FIGHT);
                                        modSendData.SendData(modSendData.SendTarget.ToPCArea, PJInd, Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[indireccion].WAV), Convert.ToByte(Declaraciones.UserList[PJInd].Pos.X), Convert.ToByte(Declaraciones.UserList[PJInd].Pos.Y)));
                                        modSendData.SendData(modSendData.SendTarget.ToPCArea, PJInd, Protocol.PrepareMessageCreateFX(Declaraciones.UserList[PJInd].Char_Renamed.CharIndex, Declaraciones.Hechizos[indireccion].FXgrh, Declaraciones.Hechizos[indireccion].loops));

                                        if (Declaraciones.UserList[PJInd].Stats.MinHp < 1)
                                        {
                                            Declaraciones.UserList[PJInd].Stats.MinHp = 0;
                                            UsUaRiOs.UserDie(PJInd);
                                        }
                                    }

                                    else if (MascotaInd > 0)
                                    {
                                        if (Declaraciones.Npclist[MascotaInd].MaestroUser > 0)
                                        {

                                            dist = Convert.ToInt16(Math.Pow(Math.Sqrt(Declaraciones.Npclist[MascotaInd].Pos.X - NPCPosX), 2d) + Math.Pow(Declaraciones.Npclist[MascotaInd].Pos.Y - NPCPosY, 2d));
                                            danio = 880d / Math.Pow(dist, 3d / 7d);
                                            danioI = Convert.ToInt16(Math.Abs(Conversion.Int(danio)));
                                            // 'efectiviza el danio
                                            Declaraciones.Npclist[MascotaInd].Stats.MinHp = Declaraciones.Npclist[MascotaInd].Stats.MinHp - danioI;

                                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, MascotaInd, Protocol.PrepareMessagePlayWave(Convert.ToByte(Declaraciones.Hechizos[indireccion].WAV), Convert.ToByte(Declaraciones.Npclist[MascotaInd].Pos.X), Convert.ToByte(Declaraciones.Npclist[MascotaInd].Pos.Y)));
                                            modSendData.SendData(modSendData.SendTarget.ToNPCArea, MascotaInd, Protocol.PrepareMessageCreateFX(Declaraciones.Npclist[MascotaInd].Char_Renamed.CharIndex, Declaraciones.Hechizos[indireccion].FXgrh, Declaraciones.Hechizos[indireccion].loops));

                                            if (Declaraciones.Npclist[MascotaInd].Stats.MinHp < 1)
                                            {
                                                Declaraciones.Npclist[MascotaInd].Stats.MinHp = 0;
                                                NPCs.MuereNpc(MascotaInd, 0);
                                            }
                                        } // 'es mascota
                                    } // 'hay npc

                                }
                            }

                            break;
                        }
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in MagoDestruyeWand: " + ex.Message);
                string argdesc = "Error en NPCAI.MagoDestruyeWand ";
                General.LogError(ref argdesc);
            }
        }


        public static void GreedyWalkTo(short npcorig, short Map, short X, short Y)
        {
            try
            {
                // '  Este procedimiento es llamado cada vez que un NPC deba ir
                // '  a otro lugar en el mismo mapa. Utiliza una técnica
                // '  de programación greedy no determinística.
                // '  Cada paso azaroso que me acerque al destino, es un buen paso.
                // '  Si no hay mejor paso válido, entonces hay que volver atrás y reintentar.
                // '  Si no puedo moverme, me considero piketeado
                // '  La funcion es larga, pero es O(1) - orden algorítmico temporal constante

                // Rapsodius - Changed Mod by And for speed

                short NPCx;
                short NPCy;
                short USRx;
                short USRy;
                short dual;
                short mapa;

                if (!(Declaraciones.Npclist[npcorig].Pos.Map == Map))
                    return; // 'si son distintos mapas abort

                NPCx = Declaraciones.Npclist[npcorig].Pos.X;
                NPCy = Declaraciones.Npclist[npcorig].Pos.Y;

                if (NPCx == X & NPCy == Y)
                    return; // 'ya llegué!!


                // '  Levanto las coordenadas del destino
                USRx = X;
                USRy = Y;
                mapa = Map;

                // '  moverse
                if (NPCx > USRx)
                {
                    if (NPCy < USRy)
                    {
                        // 'NPC esta arriba a la derecha
                        dual = Convert.ToInt16(Matematicas.RandomNumber(0, 10));
                        if ((dual & 1) == 0) // 'move down
                        {
                            if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy + 1)))
                            {
                                MoverAba(npcorig);
                                return;
                            }
                            else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx - 1), NPCy))
                            {
                                MoverIzq(npcorig);
                                return;
                            }
                            else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx + 1), NPCy))
                            {
                                MoverDer(npcorig);
                                return;
                            }
                            else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy - 1)))
                            {
                                MoverArr(npcorig);
                                return;
                            }
                            // 'aqui no puedo ir a ningun lado. Hay q ver si me bloquean caspers
                            else if (CasperBlock(npcorig))
                                LiberarCasperBlock(npcorig);
                        }

                        else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx - 1), NPCy)) // 'random first move
                        {
                            MoverIzq(npcorig);
                            return;
                        }
                        else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy + 1)))
                        {
                            MoverAba(npcorig);
                            return;
                        }
                        else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx + 1), NPCy))
                        {
                            MoverDer(npcorig);
                            return;
                        }
                        else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy - 1)))
                        {
                            MoverArr(npcorig);
                            return;
                        }
                        else if (CasperBlock(npcorig))
                            LiberarCasperBlock(npcorig); // 'checked random first move
                    }
                    else if (NPCy > USRy) // 'NPC esta abajo a la derecha
                    {
                        dual = Convert.ToInt16(Matematicas.RandomNumber(0, 10));
                        if ((dual & 1) == 0) // 'move up
                        {
                            if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy - 1))) // 'U
                            {
                                MoverArr(npcorig);
                                return;
                            }
                            else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx - 1), NPCy)) // 'L
                            {
                                MoverIzq(npcorig);
                                return;
                            }
                            else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy + 1))) // 'D
                            {
                                MoverAba(npcorig);
                                return;
                            }
                            else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx + 1), NPCy)) // 'R
                            {
                                MoverDer(npcorig);
                                return;
                            }
                            else if (CasperBlock(npcorig))
                                LiberarCasperBlock(npcorig);
                        }
                        else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx - 1), NPCy)) // 'random first move
                                                                                        // 'L
                        {
                            MoverIzq(npcorig);
                            return;
                        }
                        else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy - 1))) // 'U
                        {
                            MoverArr(npcorig);
                            return;
                        }
                        else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy + 1))) // 'D
                        {
                            MoverAba(npcorig);
                            return;
                        }
                        else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx + 1), NPCy)) // 'R
                        {
                            MoverDer(npcorig);
                            return;
                        }
                        else if (CasperBlock(npcorig))
                            LiberarCasperBlock(npcorig); // 'endif random first move
                    }
                    else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx - 1), NPCy)) // 'x completitud, esta en la misma Y
                                                                                    // 'L
                    {
                        MoverIzq(npcorig);
                        return;
                    }
                    else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy + 1))) // 'D
                    {
                        MoverAba(npcorig);
                        return;
                    }
                    else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy - 1))) // 'U
                    {
                        MoverArr(npcorig);
                        return;
                    }
                    // 'si me muevo abajo entro en loop. Aca el algoritmo falla
                    else if (Declaraciones.Npclist[npcorig].CanAttack == 1 & Matematicas.RandomNumber(1, 100) > 95)
                    {
                        modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcorig, Protocol.PrepareMessageChatOverHead("Maldito bastardo, ¡Ven aquí!", Convert.ToInt16(Conversion.Str(Declaraciones.Npclist[npcorig].Char_Renamed.CharIndex)), ColorTranslator.ToOle(Color.Yellow)));
                        Declaraciones.Npclist[npcorig].CanAttack = 0;
                    }
                }

                else if (NPCx < USRx)
                {

                    if (NPCy < USRy)
                    {
                        // 'NPC esta arriba a la izquierda
                        dual = Convert.ToInt16(Matematicas.RandomNumber(0, 10));
                        if ((dual & 1) == 0) // 'move down
                        {
                            if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy + 1))) // 'ABA
                            {
                                MoverAba(npcorig);
                                return;
                            }
                            else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx + 1), NPCy)) // 'R
                            {
                                MoverDer(npcorig);
                                return;
                            }
                            else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx - 1), NPCy))
                            {
                                MoverIzq(npcorig);
                                return;
                            }
                            else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy - 1)))
                            {
                                MoverArr(npcorig);
                                return;
                            }
                            else if (CasperBlock(npcorig))
                                LiberarCasperBlock(npcorig);
                        }
                        else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx + 1), NPCy)) // 'random first move
                                                                                        // 'DER
                        {
                            MoverDer(npcorig);
                            return;
                        }
                        else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy + 1))) // 'ABA
                        {
                            MoverAba(npcorig);
                            return;
                        }
                        else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx - 1), NPCy))
                        {
                            MoverIzq(npcorig);
                            return;
                        }
                        else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy - 1)))
                        {
                            MoverArr(npcorig);
                            return;
                        }
                        else if (CasperBlock(npcorig))
                            LiberarCasperBlock(npcorig);
                    }

                    else if (NPCy > USRy) // 'NPC esta abajo a la izquierda
                    {
                        dual = Convert.ToInt16(Matematicas.RandomNumber(0, 10));
                        if ((dual & 1) == 0) // 'move up
                        {
                            if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy - 1))) // 'U
                            {
                                MoverArr(npcorig);
                                return;
                            }
                            else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx + 1), NPCy)) // 'R
                            {
                                MoverDer(npcorig);
                                return;
                            }
                            else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx - 1), NPCy)) // 'L
                            {
                                MoverIzq(npcorig);
                                return;
                            }
                            else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy + 1))) // 'D
                            {
                                MoverAba(npcorig);
                                return;
                            }
                            else if (CasperBlock(npcorig))
                                LiberarCasperBlock(npcorig);
                        }
                        else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx + 1), NPCy)) // 'R
                        {
                            MoverDer(npcorig);
                            return;
                        }
                        else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy - 1))) // 'U
                        {
                            MoverArr(npcorig);
                            return;
                        }
                        else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy + 1))) // 'D
                        {
                            MoverAba(npcorig);
                            return;
                        }
                        else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx - 1), NPCy)) // 'L
                        {
                            MoverIzq(npcorig);
                            return;
                        }
                        else if (CasperBlock(npcorig))
                            LiberarCasperBlock(npcorig);
                    }
                    else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx + 1), NPCy)) // 'x completitud, esta en la misma Y
                                                                                    // 'R
                    {
                        MoverDer(npcorig);
                        return;
                    }
                    else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy + 1))) // 'D
                    {
                        MoverAba(npcorig);
                        return;
                    }
                    else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy - 1))) // 'U
                    {
                        MoverArr(npcorig);
                        return;
                    }
                    // 'si me muevo loopeo. aca falla el algoritmo
                    else if (Declaraciones.Npclist[npcorig].CanAttack == 1 & Matematicas.RandomNumber(1, 100) > 95)
                    {
                        modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcorig, Protocol.PrepareMessageChatOverHead("Maldito bastardo, ¡Ven aquí!", Declaraciones.Npclist[npcorig].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Yellow)));
                        Declaraciones.Npclist[npcorig].CanAttack = 0;
                    }
                }


                else if (NPCy > USRy) // 'igual X
                                      // 'NPC ESTA ABAJO
                {
                    if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy - 1))) // 'U
                    {
                        MoverArr(npcorig);
                        return;
                    }
                    else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx + 1), NPCy)) // 'R
                    {
                        MoverDer(npcorig);
                        return;
                    }
                    else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx - 1), NPCy)) // 'L
                    {
                        MoverIzq(npcorig);
                        return;
                    }
                    // 'aca tambien falla el algoritmo
                    else if (Declaraciones.Npclist[npcorig].CanAttack == 1 & Matematicas.RandomNumber(1, 100) > 95)
                    {
                        modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcorig, Protocol.PrepareMessageChatOverHead("Maldito bastardo, ¡Ven aquí!", Declaraciones.Npclist[npcorig].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Yellow)));
                        Declaraciones.Npclist[npcorig].CanAttack = 0;
                    }
                }
                else if (Extra.LegalPos(mapa, NPCx, Convert.ToInt16(NPCy + 1))) // 'NPC ESTA ARRIBA
                                                                                // 'ABA
                {
                    MoverAba(npcorig);
                    return;
                }
                else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx + 1), NPCy)) // 'R
                {
                    MoverDer(npcorig);
                    return;
                }
                else if (Extra.LegalPos(mapa, Convert.ToInt16(NPCx - 1), NPCy)) // 'L
                {
                    MoverIzq(npcorig);
                    return;
                }
                // 'posible loop
                else if (Declaraciones.Npclist[npcorig].CanAttack == 1 & Matematicas.RandomNumber(1, 100) > 95)
                {
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcorig, Protocol.PrepareMessageChatOverHead("Maldito bastardo, ¡Ven aquí!", Declaraciones.Npclist[npcorig].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Yellow)));
                    Declaraciones.Npclist[npcorig].CanAttack = 0;
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in GreedyWalkTo: " + ex.Message);
                string argdesc = "Error en NPCAI.GreedyWalkTo";
                General.LogError(ref argdesc);
            }
        }

        public static void MoverAba(short npcorig)
        {
            try
            {

                short mapa;
                short NPCx;
                short NPCy;
                mapa = Declaraciones.Npclist[npcorig].Pos.Map;
                NPCx = Declaraciones.Npclist[npcorig].Pos.X;
                NPCy = Declaraciones.Npclist[npcorig].Pos.Y;

                modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcorig, Protocol.PrepareMessageCharacterMove(Declaraciones.Npclist[npcorig].Char_Renamed.CharIndex, Convert.ToByte(NPCx), Convert.ToByte(NPCy + 1)));
                // Update map and npc pos
                Declaraciones.MapData[mapa, NPCx, NPCy].NpcIndex = 0;
                Declaraciones.Npclist[npcorig].Pos.Y = Convert.ToInt16(NPCy + 1);
                Declaraciones.Npclist[npcorig].Char_Renamed.heading = Declaraciones.eHeading.SOUTH;
                Declaraciones.MapData[mapa, NPCx, NPCy + 1].NpcIndex = npcorig;

                // Revisamos sidebemos cambair el área
                ModAreas.CheckUpdateNeededNpc(npcorig, (byte)Declaraciones.eHeading.SOUTH);
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in MoverAba: " + ex.Message);
                string argdesc = "Error en NPCAI.MoverAba ";
                General.LogError(ref argdesc);
            }
        }

        public static void MoverArr(short npcorig)
        {
            try
            {

                short mapa;
                short NPCx;
                short NPCy;
                mapa = Declaraciones.Npclist[npcorig].Pos.Map;
                NPCx = Declaraciones.Npclist[npcorig].Pos.X;
                NPCy = Declaraciones.Npclist[npcorig].Pos.Y;

                modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcorig, Protocol.PrepareMessageCharacterMove(Declaraciones.Npclist[npcorig].Char_Renamed.CharIndex, Convert.ToByte(NPCx), Convert.ToByte(NPCy - 1)));
                // Update map and npc pos
                Declaraciones.MapData[mapa, NPCx, NPCy].NpcIndex = 0;
                Declaraciones.Npclist[npcorig].Pos.Y = Convert.ToInt16(NPCy - 1);
                Declaraciones.Npclist[npcorig].Char_Renamed.heading = Declaraciones.eHeading.NORTH;
                Declaraciones.MapData[mapa, NPCx, NPCy - 1].NpcIndex = npcorig;

                // Revisamos sidebemos cambair el área
                ModAreas.CheckUpdateNeededNpc(npcorig, (byte)Declaraciones.eHeading.NORTH);
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in MoverArr: " + ex.Message);
                string argdesc = "Error en NPCAI.MoverArr";
                General.LogError(ref argdesc);
            }
        }

        public static void MoverIzq(short npcorig)
        {
            try
            {

                short mapa;
                short NPCx;
                short NPCy;
                mapa = Declaraciones.Npclist[npcorig].Pos.Map;
                NPCx = Declaraciones.Npclist[npcorig].Pos.X;
                NPCy = Declaraciones.Npclist[npcorig].Pos.Y;

                modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcorig, Protocol.PrepareMessageCharacterMove(Declaraciones.Npclist[npcorig].Char_Renamed.CharIndex, Convert.ToByte(NPCx - 1), Convert.ToByte(NPCy)));
                // Update map and npc pos
                Declaraciones.MapData[mapa, NPCx, NPCy].NpcIndex = 0;
                Declaraciones.Npclist[npcorig].Pos.X = Convert.ToInt16(NPCx - 1);
                Declaraciones.Npclist[npcorig].Char_Renamed.heading = Declaraciones.eHeading.WEST;
                Declaraciones.MapData[mapa, NPCx - 1, NPCy].NpcIndex = npcorig;

                // Revisamos sidebemos cambair el área
                ModAreas.CheckUpdateNeededNpc(npcorig, (byte)Declaraciones.eHeading.WEST);
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in MoverIzq: " + ex.Message);
                string argdesc = "Error en NPCAI.MoverIzq";
                General.LogError(ref argdesc);
            }
        }

        public static void MoverDer(short npcorig)
        {
            try
            {

                short mapa;
                short NPCx;
                short NPCy;
                mapa = Declaraciones.Npclist[npcorig].Pos.Map;
                NPCx = Declaraciones.Npclist[npcorig].Pos.X;
                NPCy = Declaraciones.Npclist[npcorig].Pos.Y;

                modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcorig, Protocol.PrepareMessageCharacterMove(Declaraciones.Npclist[npcorig].Char_Renamed.CharIndex, Convert.ToByte(NPCx + 1), Convert.ToByte(NPCy)));
                // Update map and npc pos
                Declaraciones.MapData[mapa, NPCx, NPCy].NpcIndex = 0;
                Declaraciones.Npclist[npcorig].Pos.X = Convert.ToInt16(NPCx + 1);
                Declaraciones.Npclist[npcorig].Char_Renamed.heading = Declaraciones.eHeading.EAST;
                Declaraciones.MapData[mapa, NPCx + 1, NPCy].NpcIndex = npcorig;

                // Revisamos sidebemos cambair el área
                ModAreas.CheckUpdateNeededNpc(npcorig, (byte)Declaraciones.eHeading.EAST);
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in MoverDer: " + ex.Message);
                string argdesc = "Error en NPCAI.MoverDer";
                General.LogError(ref argdesc);
            }
        }


        public static void VolverAlCentro(short npcind)
        {
            try
            {

                short NPCPosX;
                short NPCPosY;
                short NpcMap;
                NPCPosX = Declaraciones.Npclist[npcind].Pos.X;
                NPCPosY = Declaraciones.Npclist[npcind].Pos.Y;
                NpcMap = Declaraciones.Npclist[npcind].Pos.Map;

                if (NpcMap == MAPA_PRETORIANO)
                {
                    // '35,25 y 67,25 son las posiciones del rey
                    if (NPCPosX < 50) // 'esta a la izquierda
                    {
                        GreedyWalkTo(npcind, NpcMap, ALCOBA1_X, ALCOBA1_Y);
                    }
                    // GreedyWalkTo npcind, NpcMap, 35, 25
                    else
                    {
                        GreedyWalkTo(npcind, NpcMap, ALCOBA2_X, ALCOBA2_Y);
                        // GreedyWalkTo npcind, NpcMap, 67, 25
                    }
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in VolverAlCentro: " + ex.Message);
                string argdesc = "Error en NPCAI.VolverAlCentro";
                General.LogError(ref argdesc);
            }
        }

        public static bool EstoyMuyLejos(short npcind)
        {
            bool EstoyMuyLejosRet = default;
            // 'me dice si estoy fuera del anillo exterior de proteccion
            // 'de los clerigos

            bool retvalue;

            // If Npclist(npcind).Pos.X < 50 Then
            // retvalue = Npclist(npcind).Pos.X < 43 And Npclist(npcind).Pos.X > 27
            // Else
            // retvalue = Npclist(npcind).Pos.X < 80 And Npclist(npcind).Pos.X > 59
            // End If

            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto npcind. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            retvalue = Declaraciones.Npclist[npcind].Pos.Y > 39;

            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto npcind. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            if (!(Declaraciones.Npclist[npcind].Pos.Map == MAPA_PRETORIANO))
            {
                EstoyMuyLejosRet = false;
            }
            else
            {
                EstoyMuyLejosRet = retvalue;
            }

            return EstoyMuyLejosRet;

        errorh:
            ;

            string argdesc = "Error en NPCAI.EstoymUYLejos";
            General.LogError(ref argdesc);
        }

        public static bool EstoyLejos(short npcind)
        {
            bool EstoyLejosRet = default;
            try
            {

                // '35,25 y 67,25 son las posiciones del rey
                // 'esta fction me indica si estoy lejos del rango de vision


                bool retvalue;

                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto npcind. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                if (Declaraciones.Npclist[npcind].Pos.X < 50)
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto npcind. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    retvalue = Declaraciones.Npclist[npcind].Pos.X < 43 & Declaraciones.Npclist[npcind].Pos.X > 27;
                }
                else
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto npcind. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    retvalue = Declaraciones.Npclist[npcind].Pos.X < 75 & Declaraciones.Npclist[npcind].Pos.X > 59;
                }

                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto npcind. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                retvalue = retvalue & Declaraciones.Npclist[npcind].Pos.Y > 19 & Declaraciones.Npclist[npcind].Pos.Y < 31;

                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto npcind. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                if (!(Declaraciones.Npclist[npcind].Pos.Map == MAPA_PRETORIANO))
                {
                    EstoyLejosRet = false;
                }
                else
                {
                    EstoyLejosRet = !retvalue;
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in EstoyMuyLejos: " + ex.Message);
                string argdesc = "Error en NPCAI.EstoyLejos";
                General.LogError(ref argdesc);
            }

            return EstoyLejosRet;
        }

        public static bool EsAlcanzable(short npcind, short PJEnInd)
        {
            bool EsAlcanzableRet = default;
            try
            {

                // 'esta funcion es especialmente hecha para el mapa pretoriano
                // 'Está diseñada para que se ignore a los PJs que estan demasiado lejos
                // 'evitando asi que los "lockeen" en la pelea sacandolos de combate
                // 'sin matarlos. La fcion es totalmente inutil si los NPCs estan en otro mapa.
                // 'Chequea la posibilidad que les hagan /racc desde otro mapa para evitar
                // 'malos comportamientos
                // '35,25 y 67,25 son las posiciones del rey
                // 'Try


                bool retvalue;
                bool retValue2;

                short PJPosX;
                short PJPosY;
                short NPCPosX;
                short NPCPosY;

                PJPosX = Declaraciones.UserList[PJEnInd].Pos.X;
                PJPosY = Declaraciones.UserList[PJEnInd].Pos.Y;
                NPCPosX = Declaraciones.Npclist[npcind].Pos.X;
                NPCPosY = Declaraciones.Npclist[npcind].Pos.Y;

                if (Declaraciones.Npclist[npcind].Pos.Map == MAPA_PRETORIANO & Declaraciones.UserList[PJEnInd].Pos.Map == MAPA_PRETORIANO)
                {
                    // 'los bounds del mapa pretoriano son fijos.
                    // 'Esta en una posicion alcanzable si esta dentro del
                    // 'espacio de las alcobas reales del mapa diseñado por mi.
                    // 'Y dentro de la alcoba en el rango del perimetro de defensa
                    // ' 8+8+8+8 x 7+7+7+7
                    retvalue = PJPosX > 18 & PJPosX < 49 & NPCPosX <= 51; // And NPCPosX < 49
                    retvalue = retvalue & PJPosY > 14 & PJPosY < 40; // And NPCPosY > 14 And NPCPosY < 50)
                    retValue2 = PJPosX > 52 & PJPosX < 81 & NPCPosX > 51; // And NPCPosX < 81
                    retValue2 = retValue2 & PJPosY > 14 & PJPosY < 40; // And NPCPosY > 14 And NPCPosY < 50)
                                                                       // 'rv dice si estan en la alcoba izquierda los 2 y en zona valida de combate
                                                                       // 'rv2 dice si estan en la derecha
                    retvalue = retvalue | retValue2;
                }
                // If retvalue = False Then
                // If Npclist(npcind).CanAttack = 1 Then
                // Call SendData(SendTarget.ToNPCArea, npcind, Npclist(npcind).Pos.Map, "||" & vbYellow & "°¡ Cobarde !°" & str(Npclist(npcind).Char.CharIndex))
                // Npclist(npcind).CanAttack = 0
                // End If
                // End If
                else
                {
                    retvalue = false;
                }

                EsAlcanzableRet = retvalue;
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in EsAlcanzable: " + ex.Message);
                string argdesc = "Error en NPCAI.EsAlcanzable";
                General.LogError(ref argdesc);
            }

            return EsAlcanzableRet;
        }


        public static bool CasperBlock(short npc)
        {
            bool CasperBlockRet = default;
            try
            {

                short NPCPosM;
                short NPCPosX;
                short NPCPosY;
                short PJ;

                bool retvalue;

                NPCPosX = Declaraciones.Npclist[npc].Pos.X;
                NPCPosY = Declaraciones.Npclist[npc].Pos.Y;
                NPCPosM = Declaraciones.Npclist[npc].Pos.Map;

                retvalue = !(Extra.LegalPos(NPCPosM, Convert.ToInt16(NPCPosX + 1), NPCPosY) | Extra.LegalPos(NPCPosM, Convert.ToInt16(NPCPosX - 1), NPCPosY) | Extra.LegalPos(NPCPosM, NPCPosX, Convert.ToInt16(NPCPosY + 1)) | Extra.LegalPos(NPCPosM, NPCPosX, Convert.ToInt16(NPCPosY - 1)));

                if (retvalue)
                {
                    // 'si son todas invalidas
                    // 'busco que algun casper sea causante de piketeo
                    retvalue = false;

                    PJ = Declaraciones.MapData[NPCPosM, NPCPosX + 1, NPCPosY].UserIndex;
                    if (PJ > 0)
                    {
                        retvalue = Declaraciones.UserList[PJ].flags.Muerto == 1;
                    }

                    PJ = Declaraciones.MapData[NPCPosM, NPCPosX - 1, NPCPosY].UserIndex;
                    if (PJ > 0)
                    {
                        retvalue = retvalue | Declaraciones.UserList[PJ].flags.Muerto == 1;
                    }

                    PJ = Declaraciones.MapData[NPCPosM, NPCPosX, NPCPosY + 1].UserIndex;
                    if (PJ > 0)
                    {
                        retvalue = retvalue | Declaraciones.UserList[PJ].flags.Muerto == 1;
                    }

                    PJ = Declaraciones.MapData[NPCPosM, NPCPosX, NPCPosY - 1].UserIndex;
                    if (PJ > 0)
                    {
                        retvalue = retvalue | Declaraciones.UserList[PJ].flags.Muerto == 1;
                    }
                }

                else
                {
                    retvalue = false;

                }

                CasperBlockRet = retvalue;
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in CasperBlock: " + ex.Message);
                // MsgBox ("ERROR!!")
                CasperBlockRet = false;
                string argdesc = "Error en NPCAI.CasperBlock";
                General.LogError(ref argdesc);
            }

            return CasperBlockRet;
        }


        public static void LiberarCasperBlock(short npcind)
        {
            try
            {

                short NPCPosX;
                short NPCPosY;
                short NPCPosM;

                NPCPosX = Declaraciones.Npclist[npcind].Pos.X;
                NPCPosY = Declaraciones.Npclist[npcind].Pos.Y;
                NPCPosM = Declaraciones.Npclist[npcind].Pos.Map;

                if (Extra.LegalPos(NPCPosM, Convert.ToInt16(NPCPosX + 1), Convert.ToInt16(NPCPosY + 1)))
                {
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageCharacterMove(Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, Convert.ToByte(NPCPosX + 1), Convert.ToByte(NPCPosY + 1)));
                    // Update map and npc pos
                    Declaraciones.MapData[NPCPosM, NPCPosX, NPCPosY].NpcIndex = 0;
                    Declaraciones.Npclist[npcind].Pos.Y = Convert.ToInt16(NPCPosY + 1);
                    Declaraciones.Npclist[npcind].Pos.X = Convert.ToInt16(NPCPosX + 1);
                    Declaraciones.Npclist[npcind].Char_Renamed.heading = Declaraciones.eHeading.SOUTH;
                    Declaraciones.MapData[NPCPosM, NPCPosX + 1, NPCPosY + 1].NpcIndex = npcind;
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead("¡¡JA JA JA JA!!", Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Yellow)));
                    return;
                }

                if (Extra.LegalPos(NPCPosM, Convert.ToInt16(NPCPosX - 1), Convert.ToInt16(NPCPosY - 1)))
                {
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageCharacterMove(Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, Convert.ToByte(NPCPosX - 1), Convert.ToByte(NPCPosY - 1)));
                    // Update map and npc pos
                    Declaraciones.MapData[NPCPosM, NPCPosX, NPCPosY].NpcIndex = 0;
                    Declaraciones.Npclist[npcind].Pos.Y = Convert.ToInt16(NPCPosY - 1);
                    Declaraciones.Npclist[npcind].Pos.X = Convert.ToInt16(NPCPosX - 1);
                    Declaraciones.Npclist[npcind].Char_Renamed.heading = Declaraciones.eHeading.NORTH;
                    Declaraciones.MapData[NPCPosM, NPCPosX - 1, NPCPosY - 1].NpcIndex = npcind;
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead("¡¡JA JA JA JA!!", Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Yellow)));
                    return;
                }

                if (Extra.LegalPos(NPCPosM, Convert.ToInt16(NPCPosX + 1), Convert.ToInt16(NPCPosY - 1)))
                {
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageCharacterMove(Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, Convert.ToByte(NPCPosX + 1), Convert.ToByte(NPCPosY - 1)));
                    // Update map and npc pos
                    Declaraciones.MapData[NPCPosM, NPCPosX, NPCPosY].NpcIndex = 0;
                    Declaraciones.Npclist[npcind].Pos.Y = Convert.ToInt16(NPCPosY - 1);
                    Declaraciones.Npclist[npcind].Pos.X = Convert.ToInt16(NPCPosX + 1);
                    Declaraciones.Npclist[npcind].Char_Renamed.heading = Declaraciones.eHeading.EAST;
                    Declaraciones.MapData[NPCPosM, NPCPosX + 1, NPCPosY - 1].NpcIndex = npcind;
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead("¡¡JA JA JA JA!!", Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Yellow)));
                    return;
                }

                if (Extra.LegalPos(NPCPosM, Convert.ToInt16(NPCPosX - 1), Convert.ToInt16(NPCPosY + 1)))
                {
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageCharacterMove(Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, Convert.ToByte(NPCPosX - 1), Convert.ToByte(NPCPosY + 1)));
                    // Update map and npc pos
                    Declaraciones.MapData[NPCPosM, NPCPosX, NPCPosY].NpcIndex = 0;
                    Declaraciones.Npclist[npcind].Pos.Y = Convert.ToInt16(NPCPosY + 1);
                    Declaraciones.Npclist[npcind].Pos.X = Convert.ToInt16(NPCPosX - 1);
                    Declaraciones.Npclist[npcind].Char_Renamed.heading = Declaraciones.eHeading.WEST;
                    Declaraciones.MapData[NPCPosM, NPCPosX - 1, NPCPosY + 1].NpcIndex = npcind;
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead("¡¡JA JA JA JA!!", Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Yellow)));
                    return;
                }

                // 'si esta aca, estamos fritos!
                if (Declaraciones.Npclist[npcind].CanAttack == 1)
                {
                    modSendData.SendData(modSendData.SendTarget.ToNPCArea, npcind, Protocol.PrepareMessageChatOverHead("¡Por las barbas de los antiguos reyes! ¡Alejáos endemoniados espectros o sufriréis la furia de los dioses!", Declaraciones.Npclist[npcind].Char_Renamed.CharIndex, ColorTranslator.ToOle(Color.Yellow)));
                    Declaraciones.Npclist[npcind].CanAttack = 0;
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in LiberarCasperBlock: " + ex.Message);
                string argdesc = "Error en NPCAI.LiberarCasperBlock";
                General.LogError(ref argdesc);
            }
        }

        public static void CambiarAlcoba(short npcind)
        {
            try
            {

                switch (Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot)
                {
                    case 2:
                        {
                            GreedyWalkTo(npcind, MAPA_PRETORIANO, 48, 70);
                            if (Declaraciones.Npclist[npcind].Pos.X == 48 & Declaraciones.Npclist[npcind].Pos.Y == 70)
                                Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot = Convert.ToByte(Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot + 1);
                            break;
                        }
                    case 6:
                        {
                            GreedyWalkTo(npcind, MAPA_PRETORIANO, 52, 71);
                            if (Declaraciones.Npclist[npcind].Pos.X == 52 & Declaraciones.Npclist[npcind].Pos.Y == 71)
                                Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot = Convert.ToByte(Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot + 1);
                            break;
                        }
                    case 1:
                        {
                            GreedyWalkTo(npcind, MAPA_PRETORIANO, 73, 56);
                            if (Declaraciones.Npclist[npcind].Pos.X == 73 & Declaraciones.Npclist[npcind].Pos.Y == 56)
                                Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot = Convert.ToByte(Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot + 1);
                            break;
                        }
                    case 7:
                        {
                            GreedyWalkTo(npcind, MAPA_PRETORIANO, 73, 48);
                            if (Declaraciones.Npclist[npcind].Pos.X == 73 & Declaraciones.Npclist[npcind].Pos.Y == 48)
                                Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot = Convert.ToByte(Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot + 1);
                            break;
                        }
                    case 5:
                        {
                            GreedyWalkTo(npcind, MAPA_PRETORIANO, 31, 56);
                            if (Declaraciones.Npclist[npcind].Pos.X == 31 & Declaraciones.Npclist[npcind].Pos.Y == 56)
                                Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot = Convert.ToByte(Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot + 1);
                            break;
                        }
                    case 3:
                        {
                            GreedyWalkTo(npcind, MAPA_PRETORIANO, 31, 48);
                            if (Declaraciones.Npclist[npcind].Pos.X == 31 & Declaraciones.Npclist[npcind].Pos.Y == 48)
                                Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot = Convert.ToByte(Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot + 1);
                            break;
                        }
                    case 4:
                    case 8:
                        {
                            Declaraciones.Npclist[npcind].Invent.ArmourEqpSlot = 0;
                            return;
                        }
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in CambiarAlcoba: " + ex.Message);
                string argdesc = "Error en CambiarAlcoba " + ex.Message;
                General.LogError(ref argdesc);
            }
        }
    }
}