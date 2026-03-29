using System;
using Microsoft.VisualBasic;

namespace Legacy
{
    internal class SoundMapInfo
    {
        private struct p_tSoundMapInfo
        {
            public short Cantidad;
            public short[] SoundIndex;
            public int[] flags;
            public float[] Probabilidad;
        }

        private enum p_eSoundFlags
        {
            ninguna = 0,
            Lluvia = 1
        }

        private p_tSoundMapInfo[] p_Mapas;


        // sonidos conocidos, pasados a enum para intelisense
        public enum e_SoundIndex
        {
            MUERTE_HOMBRE = 11,
            MUERTE_MUJER = 74,
            FLECHA_IMPACTO = 65,
            CONVERSION_BARCO = 55,
            MORFAR_MANZANA = 82,
            SOUND_COMIDA = 7
        }

        // UPGRADE_NOTE: Class_Initialize se actualizó a Class_Initialize_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        private void Class_Initialize_Renamed()
        {
            // armar el array
            // UPGRADE_WARNING: El límite inferior de la matriz p_Mapas ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            p_Mapas = new p_tSoundMapInfo[(Declaraciones.NumMaps + 1)];
            LoadSoundMapInfo();
        }

        public SoundMapInfo() : base()
        {
            Class_Initialize_Renamed();
        }

        public void LoadSoundMapInfo()
        {
            int i;
            short j;
            string Temps;
            string MAPFILE;

            MAPFILE = AppDomain.CurrentDomain.BaseDirectory + Declaraciones.MapPath + "MAPA";

            var loopTo = p_Mapas.Length - 1;
            for (i = 1; i <= loopTo; i++)
            {
                int argEmptySpaces = 1024;
                Temps = ES.GetVar(MAPFILE + i + ".dat", "SONIDOS", "Cantidad", EmptySpaces: ref argEmptySpaces);

                if (Information.IsNumeric(Temps))
                {
                    p_Mapas[i].Cantidad = Convert.ToInt16(Migration.ParseVal(Temps));

                    p_Mapas[i].flags = new int[(p_Mapas[i].Cantidad + 1)];
                    p_Mapas[i].Probabilidad = new float[(p_Mapas[i].Cantidad + 1)];
                    p_Mapas[i].SoundIndex = new short[(p_Mapas[i].Cantidad + 1)];

                    var loopTo1 = p_Mapas[i].Cantidad;
                    for (j = 1; j <= loopTo1; j++)
                    {
                        int argEmptySpaces1 = 1024;
                        p_Mapas[i].flags[j] = Convert.ToInt32(Migration.ParseVal(ES.GetVar(MAPFILE + i + ".dat", "SONIDO" + j, "Flags", EmptySpaces: ref argEmptySpaces1)));
                        int argEmptySpaces2 = 1024;
                        p_Mapas[i].Probabilidad[j] = Convert.ToSingle(Migration.ParseVal(ES.GetVar(MAPFILE + i + ".dat", "SONIDO" + j, "Probabilidad", EmptySpaces: ref argEmptySpaces2)));
                        int argEmptySpaces3 = 1024;
                        p_Mapas[i].SoundIndex[j] = Convert.ToInt16(Migration.ParseVal(ES.GetVar(MAPFILE + i + ".dat", "SONIDO" + j, "Sonido", EmptySpaces: ref argEmptySpaces3)));
                    }
                }
                else
                {
                    p_Mapas[i].Cantidad = 0;
                }
            }
        }

        public void ReproducirSonidosDeMapas()
        {
            int i;
            byte SonidoMapa;
            byte posX;
            byte posY;

            posX = Convert.ToByte(Matematicas.RandomNumber(Declaraciones.XMinMapSize, Declaraciones.XMaxMapSize));
            posY = Convert.ToByte(Matematicas.RandomNumber(Declaraciones.YMinMapSize, Declaraciones.YMaxMapSize));

            var loopTo = p_Mapas.Length - 1;
            for (i = 1; i <= loopTo; i++)
            {
                if (p_Mapas[i].Cantidad > 0)
                {
                    SonidoMapa = Convert.ToByte(Matematicas.RandomNumber(1, p_Mapas[i].Cantidad));
                    if (Matematicas.RandomNumber(1, 100) <= p_Mapas[i].Probabilidad[SonidoMapa])
                    {
                        if (Admin.Lloviendo)
                        {
                            if ((p_Mapas[i].flags[SonidoMapa] ^ (int)p_eSoundFlags.Lluvia) != 0)
                            {
                                modSendData.SendData(modSendData.SendTarget.toMap, Convert.ToInt16(i), Protocol.PrepareMessagePlayWave(Convert.ToByte(p_Mapas[i].SoundIndex[SonidoMapa]), Convert.ToByte(posX), Convert.ToByte(posY)));
                            }
                        }
                        else if ((p_Mapas[i].flags[SonidoMapa] ^ (int)p_eSoundFlags.ninguna) != 0)
                        {
                            modSendData.SendData(modSendData.SendTarget.toMap, Convert.ToInt16(i), Protocol.PrepareMessagePlayWave(Convert.ToByte(p_Mapas[i].SoundIndex[SonidoMapa]), Convert.ToByte(posX), Convert.ToByte(posY)));
                        }
                    }
                }
            }
        }

        public void ReproducirSonido(modSendData.SendTarget Destino, short index, short SoundIndex)
        {
            modSendData.SendData(Destino, index, Protocol.PrepareMessagePlayWave(Convert.ToByte(SoundIndex), Convert.ToByte(Declaraciones.UserList[index].Pos.X), Convert.ToByte(Declaraciones.UserList[index].Pos.Y)));
        }
    }
}