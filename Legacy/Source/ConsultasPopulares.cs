using System;
using System.Linq;
using Microsoft.VisualBasic;

namespace Legacy
{
    internal class ConsultasPopulares
    {
        private const string ARCHIVOMAILS = "logs/votaron.dat";
        private const string ARCHIVOCONFIG = "dat/consultas.dat";

        private short pEncuestaActualNum;
        private string pEncuestaActualTex;
        private short pNivelRequerido;
        private short[] pOpciones;


        public short Numero
        {
            get
            {
                short NumeroRet = default;
                NumeroRet = pEncuestaActualNum;
                return NumeroRet;
            }
            set
            {
                pEncuestaActualNum = value;
            }
        }


        public string texto
        {
            get
            {
                string textoRet = default;
                textoRet = pEncuestaActualTex;
                return textoRet;
            }
            set
            {
                pEncuestaActualTex = value;
            }
        }


        public void LoadData()
        {
            short CantOpciones;
            short i;

            int argEmptySpaces = 1024;
            pEncuestaActualNum = Convert.ToInt16(Migration.ParseVal(ES.GetVar(AppDomain.CurrentDomain.BaseDirectory + ARCHIVOCONFIG, "INIT", "ConsultaActual", EmptySpaces: ref argEmptySpaces)));
            int argEmptySpaces1 = 1024;
            pEncuestaActualTex = ES.GetVar(AppDomain.CurrentDomain.BaseDirectory + ARCHIVOCONFIG, "INIT", "ConsultaActualTexto", EmptySpaces: ref argEmptySpaces1);
            int argEmptySpaces2 = 1024;
            pNivelRequerido = Convert.ToInt16(ES.GetVar(AppDomain.CurrentDomain.BaseDirectory + ARCHIVOCONFIG, "INIT", "NivelRequerido", EmptySpaces: ref argEmptySpaces2));

            if (pEncuestaActualNum > 0)
            {
                // cargo todas las opciones
                int argEmptySpaces3 = 1024;
                CantOpciones = Convert.ToInt16(Migration.ParseVal(ES.GetVar(AppDomain.CurrentDomain.BaseDirectory + ARCHIVOCONFIG, "ENCUESTA" + pEncuestaActualNum, "CANTOPCIONES", EmptySpaces: ref argEmptySpaces3)));
                // UPGRADE_WARNING: El límite inferior de la matriz pOpciones ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                pOpciones = new short[(CantOpciones + 1)];
                var loopTo = CantOpciones;
                for (i = 1; i <= loopTo; i++)
                {
                    int argEmptySpaces4 = 1024;
                    pOpciones[i] = Convert.ToInt16(Migration.ParseVal(ES.GetVar(AppDomain.CurrentDomain.BaseDirectory + ARCHIVOCONFIG, "ENCUESTA" + pEncuestaActualNum, "OPCION" + i, EmptySpaces: ref argEmptySpaces4)));
                }
            }
        }

        public string doVotar(short UserIndex, short opcion)
        {
            string doVotarRet = default;
            try
            {
                bool YaVoto;
                string CharFile;
                short sufragio;

                // revisar q no haya votado
                // grabar en el charfile el numero de encuesta
                // actualizar resultados encuesta
                if (pEncuestaActualNum == 0)
                {
                    doVotarRet = "No hay consultas populares abiertas";
                    return doVotarRet;
                }

                CharFile = Declaraciones.CharPath + Declaraciones.UserList[UserIndex].name + ".chr";


                if (Declaraciones.UserList[UserIndex].Stats.ELV >= pNivelRequerido)
                {
                    if (OpcionValida(opcion))
                    {
                        int argEmptySpaces = 1024;
                        YaVoto = Migration.ParseVal(ES.GetVar(CharFile, "CONSULTAS", "Voto", EmptySpaces: ref argEmptySpaces)) >= pEncuestaActualNum;
                        if (!YaVoto)
                        {
                            if (!MailYaVoto(Declaraciones.UserList[UserIndex].email))
                            {
                                // pj apto para votar
                                int argEmptySpaces1 = 1024;
                                sufragio = Convert.ToInt16(Migration.ParseVal(ES.GetVar(AppDomain.CurrentDomain.BaseDirectory + ARCHIVOCONFIG, "RESULTADOS" + pEncuestaActualNum, "V" + opcion, EmptySpaces: ref argEmptySpaces1)));
                                sufragio = Convert.ToInt16(sufragio + 1);
                                ES.WriteVar(AppDomain.CurrentDomain.BaseDirectory + ARCHIVOCONFIG, "RESULTADOS" + pEncuestaActualNum, "V" + opcion, Conversion.Str(sufragio));
                                doVotarRet = "Tu voto ha sido computado. Opcion: " + opcion;
                                MarcarPjComoQueYaVoto(UserIndex);
                                MarcarMailComoQueYaVoto(Declaraciones.UserList[UserIndex].email);
                            }
                            else
                            {
                                MarcarPjComoQueYaVoto(UserIndex);
                                doVotarRet = "Este email ya voto en la consulta: " + pEncuestaActualTex;
                            }
                        }
                        else
                        {
                            doVotarRet = "Este personaje ya voto en la consulta: " + pEncuestaActualTex;
                        }
                    }
                    else
                    {
                        doVotarRet = "Esa no es una opcion para votar";
                    }
                }
                else
                {
                    doVotarRet = "Para votar en esta consulta debes ser nivel " + pNivelRequerido + " o superior";
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in LoadData: " + ex.Message);
                string argdesc = "Error en ConsultasPopularse.doVotar: " + ex.Message;
                General.LogError(ref argdesc);
            }

            return doVotarRet;
        }


        public string SendInfoEncuesta(short UserIndex)
        {
            short i;
            Protocol.WriteConsoleMsg(UserIndex, "CONSULTA POPULAR NUMERO " + pEncuestaActualNum, Protocol.FontTypeNames.FONTTYPE_GUILD);
            Protocol.WriteConsoleMsg(UserIndex, pEncuestaActualTex, Protocol.FontTypeNames.FONTTYPE_GUILD);
            Protocol.WriteConsoleMsg(UserIndex, " Opciones de voto: ", Protocol.FontTypeNames.FONTTYPE_GUILDMSG);
            var loopTo = Convert.ToInt16(pOpciones.Length - 1);
            for (i = 1; i <= loopTo; i++)
            {
                int argEmptySpaces = 1024;
                Protocol.WriteConsoleMsg(UserIndex, "(Opcion " + i + "): " + ES.GetVar(AppDomain.CurrentDomain.BaseDirectory + ARCHIVOCONFIG, "ENCUESTA" + pEncuestaActualNum, "OPCION" + i, EmptySpaces: ref argEmptySpaces), Protocol.FontTypeNames.FONTTYPE_GUILDMSG);
            }
            Protocol.WriteConsoleMsg(UserIndex, " Para votar una opcion, escribe /encuesta NUMERODEOPCION, por ejemplo para votar la opcion 1, escribe /encuesta 1. Tu voto no podra ser cambiado.", Protocol.FontTypeNames.FONTTYPE_VENENO);
            return default;
        }


        private void MarcarPjComoQueYaVoto(short UserIndex)
        {
            ES.WriteVar(Declaraciones.CharPath + Declaraciones.UserList[UserIndex].name + ".chr", "CONSULTAS", "Voto", Conversion.Str(pEncuestaActualNum));
        }


        private bool MailYaVoto(string email)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + ARCHIVOMAILS;
            return System.IO.File.ReadAllLines(filePath).Any(line => (line ?? "") == (email ?? ""));
        }


        private void MarcarMailComoQueYaVoto(string email)
        {
            General.AppendLog(ARCHIVOMAILS, email);
        }


        private bool OpcionValida(short opcion)
        {
            bool OpcionValidaRet = default;
            OpcionValidaRet = opcion > 0 & opcion <= pOpciones.Length - 1;
            return OpcionValidaRet;
        }
    }
}