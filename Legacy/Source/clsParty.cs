using System;
using Microsoft.VisualBasic;

namespace Legacy
{
    internal class clsParty
    {
        // UPGRADE_WARNING: El límite inferior de la matriz p_members ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        private readonly mdParty.tPartyMember[] p_members = new mdParty.tPartyMember[(mdParty.PARTY_MAXMEMBERS + 1)];
        // miembros

        private int p_expTotal;
        // Estadistica :D

        private short p_Fundador;
        // el creador

        private short p_CantMiembros;
        // cantidad de miembros

        private float p_SumaNivelesElevados;
        // suma de todos los niveles elevados a la ExponenteNivelParty > Esta variable se usa para calcular la experiencia repartida en la Party.

        // datos en los pjs: | indexParty(indice en p_members), partyLeader(userindex del lider) |

        // Constructor de clase
        // UPGRADE_NOTE: Class_Initialize se actualizó a Class_Initialize_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        public void Class_Initialize_Renamed()
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: 07/04/08
            // Last Modification By: Marco Vanotti (MarKoxX)
            // - 09/29/07 p_SumaNiveles added (Tavo)
            // - 07/04/08 p_SumaNiveles changed to p_SumaNivelesElevados (MarKoxX)
            // ***************************************************
            p_expTotal = 0;
            p_CantMiembros = 0;
            p_SumaNivelesElevados = 0f;
        }

        public clsParty() : base()
        {
            Class_Initialize_Renamed();
        }

        // Destructor de clase
        // UPGRADE_NOTE: Class_Terminate se actualizó a Class_Terminate_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        public void Class_Terminate_Renamed()
        {
        }

        ~clsParty()
        {
            Class_Terminate_Renamed();
        }

        // '
        // Sets the new p_sumaniveleselevados to the party.
        // 
        // @param lvl Specifies reference to user level
        // @remarks When a user level up and he is in a party, we update p_sumaNivelesElavados so the formula still works.
        public void UpdateSumaNivelesElevados(short Lvl)
        {
            // *************************************************
            // Author: Marco Vanotti (MarKoxX)
            // Last modified: 11/24/09
            // 11/24/09: Pato - Change the exponent to a variable with the exponent
            // *************************************************
            p_SumaNivelesElevados = Convert.ToSingle(p_SumaNivelesElevados - Math.Pow(Lvl - 1, mdParty.ExponenteNivelParty) + Math.Pow(Lvl, mdParty.ExponenteNivelParty));
        }

        public int MiExperiencia(short UserIndex)
        {
            int MiExperienciaRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: 11/27/09
            // Last Modification By: Budi
            // - 09/29/07 Experience is round to the biggest number less than that number
            // - 09/29/07 Now experience is a real-number
            // - 11/27/09 Arreglé el Out of Range.
            // ***************************************************
            // Me dice cuanta experiencia tengo colectada ya en la party
            short i;
            i = 1;

            while (i <= mdParty.PARTY_MAXMEMBERS & p_members[i].UserIndex != UserIndex)
                i = Convert.ToInt16(i + 1);

            if (i <= mdParty.PARTY_MAXMEMBERS)
            {
                MiExperienciaRet = Convert.ToInt32(Conversion.Fix(p_members[i].Experiencia));
            }
            else // esto no deberia pasar :p
            {
                MiExperienciaRet = -1;
            }

            return MiExperienciaRet;
        }

        public void ObtenerExito(int ExpGanada, short mapa, ref short X, ref short Y)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: 07/04/08
            // Last Modification By: Marco Vanotti (MarKoxX)
            // - 09/29/07 New formula for calculating the experience point of each user
            // - 09/29/07 Experience is round to the biggest number less than that number
            // - 09/29/07 Now experience is a real-number
            // - 04/04/08 Ahora antes de calcular la experiencia a X usuario se fija si ese usuario existe (MarKoxX)
            // - 07/04/08 New formula to calculate Experience for each user. (MarKoxX)
            // ***************************************************
            // Se produjo un evento que da experiencia en la wp referenciada
            short i;
            short UI;
            double expThisUser;

            p_expTotal = p_expTotal + ExpGanada;

            for (i = 1; i <= mdParty.PARTY_MAXMEMBERS; i++)
            {
                UI = p_members[i].UserIndex;
                if (UI > 0)
                {
                    // Formula: Exp* (Nivel ^ ExponenteNivelParty) / sumadeNivelesElevados
                    expThisUser = Convert.ToDouble(ExpGanada * Math.Pow(Declaraciones.UserList[p_members[i].UserIndex].Stats.ELV, mdParty.ExponenteNivelParty) / p_SumaNivelesElevados);

                    if (mapa == Declaraciones.UserList[UI].Pos.Map & Declaraciones.UserList[UI].flags.Muerto == 0)
                    {
                        if (Matematicas.Distance(Declaraciones.UserList[UI].Pos.X, Declaraciones.UserList[UI].Pos.Y, X, Y) <= mdParty.PARTY_MAXDISTANCIA)
                        {
                            p_members[i].Experiencia = p_members[i].Experiencia + expThisUser;
                            if (p_members[i].Experiencia < 0d)
                            {
                                p_members[i].Experiencia = 0d;
                            }
                            if (mdParty.PARTY_EXPERIENCIAPORGOLPE)
                            {
                                Declaraciones.UserList[UI].Stats.Exp = Declaraciones.UserList[UI].Stats.Exp + Conversion.Fix(expThisUser);
                                if (Declaraciones.UserList[UI].Stats.Exp > Declaraciones.MAXEXP)
                                    Declaraciones.UserList[UI].Stats.Exp = Declaraciones.MAXEXP;
                                UsUaRiOs.CheckUserLevel(UI);
                                Protocol.WriteUpdateUserStats(UI);
                            }
                        }
                    }
                }
            }
        }

        public void MandarMensajeAConsola(string texto, string Sender)
        {
            // feo feo, muy feo acceder a senddata desde aca, pero BUEEEEEEEEEEE...
            short i;

            for (i = 1; i <= mdParty.PARTY_MAXMEMBERS; i++)
            {
                if (p_members[i].UserIndex > 0)
                {
                    Protocol.WriteConsoleMsg(p_members[i].UserIndex, " [" + Sender + "] " + texto, Protocol.FontTypeNames.FONTTYPE_PARTY);
                }
            }
        }

        public bool EsPartyLeader(short UserIndex)
        {
            bool EsPartyLeaderRet = default;
            EsPartyLeaderRet = UserIndex == p_Fundador;
            return EsPartyLeaderRet;
        }

        public bool NuevoMiembro(short UserIndex)
        {
            bool NuevoMiembroRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: 07/04/08
            // Last Modification By: Marco Vanotti (MarKoxX)
            // - 09/29/07 There is no level prohibition (Tavo)
            // - 07/04/08 Added const ExponenteNivelParty. (MarKoxX)
            // ***************************************************

            short i;
            i = 1;
            while (i <= mdParty.PARTY_MAXMEMBERS & p_members[i].UserIndex > 0)
                i = Convert.ToInt16(i + 1);

            if (i <= mdParty.PARTY_MAXMEMBERS)
            {
                p_members[i].Experiencia = 0d;
                p_members[i].UserIndex = UserIndex;
                NuevoMiembroRet = true;
                p_CantMiembros = Convert.ToInt16(p_CantMiembros + 1);
                p_SumaNivelesElevados = Convert.ToSingle(p_SumaNivelesElevados + Math.Pow(Declaraciones.UserList[UserIndex].Stats.ELV, mdParty.ExponenteNivelParty));
            }
            else
            {
                NuevoMiembroRet = false;
            }

            return NuevoMiembroRet;
        }

        public bool SaleMiembro(short UserIndex)
        {
            bool SaleMiembroRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: 07/04/08
            // Last Modification By: Marco Vanotti (MarKoxX)
            // - 09/29/07 Experience is round to the biggest number less than that number
            // - 09/29/07 Now experience is a real-number (Tavo)
            // - 07/04/08 Added const ExponenteNivelParty. (MarKoxX)
            // 11/03/2010: ZaMa - Ahora no le dice al lider que salio de su propia party, y optimice con with.
            // ***************************************************
            // el valor de retorno representa si se disuelve la party
            short i;
            short j;
            short MemberIndex;

            i = 1;
            SaleMiembroRet = false;
            while (i <= mdParty.PARTY_MAXMEMBERS & p_members[i].UserIndex != UserIndex)
                i = Convert.ToInt16(i + 1);

            if (i == 1)
            {
                // sale el founder, la party se disuelve
                SaleMiembroRet = true;
                MandarMensajeAConsola("El líder disuelve la party.", "Servidor");

                for (j = mdParty.PARTY_MAXMEMBERS; j >= 1; j += -1)
                {

                    {
                        ref var withBlock = ref p_members[j];

                        if (withBlock.UserIndex > 0)
                        {

                            // No envia el mensaje al lider
                            if (j != 1)
                            {
                                Protocol.WriteConsoleMsg(withBlock.UserIndex, "Abandonas la party liderada por " + Declaraciones.UserList[p_members[1].UserIndex].name + ".", Protocol.FontTypeNames.FONTTYPE_PARTY);
                            }

                            Protocol.WriteConsoleMsg(withBlock.UserIndex, "Durante la misma has conseguido " + Conversion.Fix(withBlock.Experiencia).ToString() + " puntos de experiencia.", Protocol.FontTypeNames.FONTTYPE_PARTY);

                            if (!mdParty.PARTY_EXPERIENCIAPORGOLPE)
                            {
                                Declaraciones.UserList[withBlock.UserIndex].Stats.Exp = Declaraciones.UserList[withBlock.UserIndex].Stats.Exp + Conversion.Fix(withBlock.Experiencia);
                                if (Declaraciones.UserList[withBlock.UserIndex].Stats.Exp > Declaraciones.MAXEXP)
                                    Declaraciones.UserList[withBlock.UserIndex].Stats.Exp = Declaraciones.MAXEXP;
                                UsUaRiOs.CheckUserLevel(withBlock.UserIndex);
                                Protocol.WriteUpdateUserStats(withBlock.UserIndex);
                            }

                            MandarMensajeAConsola(Declaraciones.UserList[withBlock.UserIndex].name + " abandona la party.", "Servidor");

                            Declaraciones.UserList[withBlock.UserIndex].PartyIndex = 0;
                            p_CantMiembros = Convert.ToInt16(p_CantMiembros - 1);
                            p_SumaNivelesElevados = Convert.ToSingle(p_SumaNivelesElevados - Math.Pow(Declaraciones.UserList[UserIndex].Stats.ELV, mdParty.ExponenteNivelParty));
                            withBlock.UserIndex = 0;
                            withBlock.Experiencia = 0d;

                        }

                    }

                }
            }
            else if (i <= mdParty.PARTY_MAXMEMBERS)
            {

                MemberIndex = p_members[i].UserIndex;

                {
                    ref var withBlock1 = ref Declaraciones.UserList[MemberIndex];
                    if (!mdParty.PARTY_EXPERIENCIAPORGOLPE)
                    {
                        withBlock1.Stats.Exp = withBlock1.Stats.Exp + Conversion.Fix(p_members[i].Experiencia);
                        if (withBlock1.Stats.Exp > Declaraciones.MAXEXP)
                            withBlock1.Stats.Exp = Declaraciones.MAXEXP;

                        UsUaRiOs.CheckUserLevel(MemberIndex);
                        Protocol.WriteUpdateUserStats(MemberIndex);
                    }

                    MandarMensajeAConsola(withBlock1.name + " abandona la party.", "Servidor");
                    // TODO: Revisar que esto este bien, y no este faltando/sobrando un mensaje, ahora solo los estoy corrigiendo
                    Protocol.WriteConsoleMsg(MemberIndex, "Durante la misma has conseguido " + Conversion.Fix(p_members[i].Experiencia).ToString() + " puntos de experiencia.", Protocol.FontTypeNames.FONTTYPE_PARTY);

                    p_CantMiembros = Convert.ToInt16(p_CantMiembros - 1);
                    p_SumaNivelesElevados = Convert.ToSingle(p_SumaNivelesElevados - Math.Pow(Declaraciones.UserList[UserIndex].Stats.ELV, mdParty.ExponenteNivelParty));
                    MemberIndex = 0;
                    p_members[i].Experiencia = 0d;
                    p_members[i].UserIndex = 0;
                    CompactMemberList();
                }
            }

            return SaleMiembroRet;
        }

        public bool HacerLeader(short UserIndex)
        {
            bool HacerLeaderRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: 09/29/07
            // Last Modification By: Lucas Tavolaro Ortiz (Tavo)
            // - 09/29/07 There is no level prohibition
            // ***************************************************
            short i;
            short OldLeader;
            double oldExp;
            short UserIndexIndex;

            UserIndexIndex = 0;
            HacerLeaderRet = true;

            for (i = 1; i <= mdParty.PARTY_MAXMEMBERS; i++)
            {
                if (p_members[i].UserIndex > 0)
                {
                    if (p_members[i].UserIndex == UserIndex)
                    {
                        UserIndexIndex = i;
                    }
                }
            }

            if (!HacerLeaderRet)
                return HacerLeaderRet;

            if (UserIndexIndex == 0)
            {
                // catastrofe! esto no deberia pasar nunca! pero como es AO.... :p
                string argdesc = "INCONSISTENCIA DE PARTIES";
                General.LogError(ref argdesc);
                modSendData.SendData(modSendData.SendTarget.ToAdmins, 0, Protocol.PrepareMessageConsoleMsg("¡¡¡Inconsistencia de parties en HACERLEADER (UII = 0), AVISE A UN PROGRAMADOR ESTO ES UNA CATASTROFE!!!!", Protocol.FontTypeNames.FONTTYPE_GUILD));
                HacerLeaderRet = false;
                return HacerLeaderRet;
            }


            // aca esta todo bien y doy vuelta las collections
            OldLeader = p_members[1].UserIndex;
            oldExp = p_members[1].Experiencia;

            p_members[1].UserIndex = p_members[UserIndexIndex].UserIndex;
            // que en realdiad es el userindex, pero no quiero inconsistencias moviendo experiencias
            p_members[1].Experiencia = p_members[UserIndexIndex].Experiencia;

            p_members[UserIndexIndex].UserIndex = OldLeader;
            p_members[UserIndexIndex].Experiencia = oldExp;

            p_Fundador = p_members[1].UserIndex;
            return HacerLeaderRet;

            // no need to compact
        }


        public void ObtenerMiembrosOnline(ref short[] MemberList)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: 09/29/07
            // Last Modification By: Marco Vanotti (MarKoxX)
            // - 09/29/07 Experience is round to the biggest number less than that number
            // - 09/29/07 Now experience is a real-number (Tavo)
            // - 08/18/08 Now TotalExperience is fixed (MarKoxX)
            // - 11/27/09 Rehice la función, ahora devuelve el array con los UI online (Budi)
            // ***************************************************

            short i;

            for (i = 1; i <= mdParty.PARTY_MAXMEMBERS; i++)
            {
                if (p_members[i].UserIndex > 0)
                {
                    MemberList[i] = p_members[i].UserIndex;
                }
            }
        }

        public int ObtenerExperienciaTotal()
        {
            int ObtenerExperienciaTotalRet = default;
            // ***************************************************
            // Author: Budi
            // Last Modification: 11/27/09
            // Retrieves the total experience acumulated in the party
            // ***************************************************
            ObtenerExperienciaTotalRet = p_expTotal;
            return ObtenerExperienciaTotalRet;
        }

        public bool PuedeEntrar(short UserIndex, ref string razon)
        {
            bool PuedeEntrarRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: 09/29/07
            // Last Modification By: Lucas Tavolaro Ortiz (Tavo)
            // - 09/29/07 There is no level prohibition
            // ***************************************************
            // DEFINE LAS REGLAS DEL JUEGO PARA DEJAR ENTRAR A MIEMBROS
            bool esArmada;
            bool esCaos;
            short MyLevel;
            short i;
            bool rv;
            short UI;

            rv = true;
            esArmada = Declaraciones.UserList[UserIndex].Faccion.ArmadaReal == 1;
            esCaos = Declaraciones.UserList[UserIndex].Faccion.FuerzasCaos == 1;
            MyLevel = Declaraciones.UserList[UserIndex].Stats.ELV;

            rv = Matematicas.Distancia(ref Declaraciones.UserList[p_members[1].UserIndex].Pos, ref Declaraciones.UserList[UserIndex].Pos) <= mdParty.MAXDISTANCIAINGRESOPARTY;
            if (rv)
            {
                rv = p_members[mdParty.PARTY_MAXMEMBERS].UserIndex == 0;
                if (rv)
                {
                    for (i = 1; i <= mdParty.PARTY_MAXMEMBERS; i++)
                    {
                        UI = p_members[i].UserIndex;
                        // pongo los casos que evitarian que pueda entrar
                        // aspirante armada en party crimi
                        if (UI > 0)
                        {
                            if (esArmada & ES.criminal(UI))
                            {
                                razon = "Los miembros del ejército real no entran a una party con criminales.";
                                rv = false;
                            }
                            // aspirante caos en party ciuda
                            if (esCaos & !ES.criminal(UI))
                            {
                                razon = "Los miembros de la legión oscura no entran a una party con ciudadanos.";
                                rv = false;
                            }
                            // aspirante crimi en party armada
                            if (Declaraciones.UserList[UI].Faccion.ArmadaReal == 1 & ES.criminal(UserIndex))
                            {
                                razon = "Los criminales no entran a parties con miembros del ejército real.";
                                rv = false;
                            }
                            // aspirante ciuda en party caos
                            if (Declaraciones.UserList[UI].Faccion.FuerzasCaos == 1 & !ES.criminal(UserIndex))
                            {
                                razon = "Los ciudadanos no entran a parties con miembros de la legión oscura.";
                                rv = false;
                            }

                            if (!rv)
                                break; // violate una programacion estructurada
                        }
                    }
                }
                else
                {
                    razon = "La mayor cantidad de miembros es " + mdParty.PARTY_MAXMEMBERS;
                }
            }
            else
            {
                // ¿Con o sin nombre?
                razon = "El usuario " + Declaraciones.UserList[UserIndex].name + " se encuentra muy lejos.";
            }

            PuedeEntrarRet = rv;
            return PuedeEntrarRet;
        }


        public void FlushExperiencia()
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: 09/29/07
            // Last Modification By: Lucas Tavolaro Ortiz (Tavo)
            // - 09/29/07 Experience is round to the biggest number less than that number
            // - 09/29/07 Now experience is a real-number
            // ***************************************************
            // esta funcion se invoca frente a cerradas del servidor. Flushea la experiencia
            // acumulada a los usuarios.

            short i;
            if (!mdParty.PARTY_EXPERIENCIAPORGOLPE) // esto sirve SOLO cuando acumulamos la experiencia!
            {
                for (i = 1; i <= mdParty.PARTY_MAXMEMBERS; i++)
                {
                    if (p_members[i].UserIndex > 0)
                    {
                        if (p_members[i].Experiencia > 0d)
                        {
                            Declaraciones.UserList[p_members[i].UserIndex].Stats.Exp = Declaraciones.UserList[p_members[i].UserIndex].Stats.Exp + Conversion.Fix(p_members[i].Experiencia);
                            if (Declaraciones.UserList[p_members[i].UserIndex].Stats.Exp > Declaraciones.MAXEXP)
                                Declaraciones.UserList[p_members[i].UserIndex].Stats.Exp = Declaraciones.MAXEXP;
                            UsUaRiOs.CheckUserLevel(p_members[i].UserIndex);
                        }
                        else if (Math.Abs(Declaraciones.UserList[p_members[i].UserIndex].Stats.Exp) > Math.Abs(Conversion.Fix(p_members[i].Experiencia)))
                        {
                            Declaraciones.UserList[p_members[i].UserIndex].Stats.Exp = Declaraciones.UserList[p_members[i].UserIndex].Stats.Exp + Conversion.Fix(p_members[i].Experiencia);
                        }
                        else
                        {
                            Declaraciones.UserList[p_members[i].UserIndex].Stats.Exp = 0d;
                        }
                        p_members[i].Experiencia = 0d;
                        Protocol.WriteUpdateUserStats(p_members[i].UserIndex);
                    }
                }
            }
        }

        private void CompactMemberList()
        {
            short i;
            var freeIndex = default(short);
            i = 1;
            while (i <= mdParty.PARTY_MAXMEMBERS)
            {
                if (p_members[i].UserIndex == 0 & freeIndex == 0)
                {
                    freeIndex = i;
                }
                else if (p_members[i].UserIndex > 0 & freeIndex > 0)
                {
                    p_members[freeIndex].Experiencia = p_members[i].Experiencia;
                    p_members[freeIndex].UserIndex = p_members[i].UserIndex;
                    p_members[i].UserIndex = 0;
                    p_members[i].Experiencia = 0d;
                    // muevo el de la pos i a freeindex
                    i = freeIndex;
                    freeIndex = 0;
                }
                i = Convert.ToInt16(i + 1);
            }
        }

        public short CantMiembros()
        {
            short CantMiembrosRet = default;
            CantMiembrosRet = p_CantMiembros;
            return CantMiembrosRet;
        }
    }
}