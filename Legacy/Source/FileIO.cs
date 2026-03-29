using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Legacy
{

    static class ES
    {
        public static void CargarSpawnList()
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short N, LoopC;
            int argEmptySpaces = 1024;
            N = Convert.ToInt16(Migration.ParseVal(GetVar(AppDomain.CurrentDomain.BaseDirectory + "Dat/Invokar.dat", "INIT", "NumNPCs", EmptySpaces: ref argEmptySpaces)));
            Declaraciones.SpawnList = new Declaraciones.tCriaturasEntrenador[(N + 1)];
            var loopTo = N;
            for (LoopC = 1; LoopC <= loopTo; LoopC++)
            {
                int argEmptySpaces1 = 1024;
                Declaraciones.SpawnList[LoopC].NpcIndex = Convert.ToInt16(Migration.ParseVal(GetVar(AppDomain.CurrentDomain.BaseDirectory + "Dat/Invokar.dat", "LIST", "NI" + LoopC, EmptySpaces: ref argEmptySpaces1)));
                int argEmptySpaces2 = 1024;
                Declaraciones.SpawnList[LoopC].NpcName = GetVar(AppDomain.CurrentDomain.BaseDirectory + "Dat/Invokar.dat", "LIST", "NN" + LoopC, EmptySpaces: ref argEmptySpaces2);
            }
        }

        public static bool EsAdmin(string name)
        {
            bool EsAdminRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short NumWizs;
            short WizNum;
            string NomB;

            int argEmptySpaces = 1024;
            NumWizs = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "Admines", EmptySpaces: ref argEmptySpaces)));

            var loopTo = NumWizs;
            for (WizNum = 1; WizNum <= loopTo; WizNum++)
            {
                int argEmptySpaces1 = 1024;
                string tmpVal = GetVar(Declaraciones.IniPath + "Server.ini", "Admines", "Admin" + WizNum, EmptySpaces: ref argEmptySpaces1);
                NomB = tmpVal is not null ? tmpVal.ToUpper() : "";

                if (NomB.Length > 0 && NomB.Substring(0, 1) == "*" | NomB.Substring(0, 1) == "+")
                    NomB = NomB.Length > 1 ? NomB.Substring(1) : "";
                if ((name.ToUpper() ?? "") == (NomB ?? ""))
                {
                    EsAdminRet = true;
                    return EsAdminRet;
                }
            }
            EsAdminRet = false;
            return EsAdminRet;
        }

        public static bool EsDios(string name)
        {
            bool EsDiosRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short NumWizs;
            short WizNum;
            string NomB;

            int argEmptySpaces = 1024;
            NumWizs = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "Dioses", EmptySpaces: ref argEmptySpaces)));
            var loopTo = NumWizs;
            for (WizNum = 1; WizNum <= loopTo; WizNum++)
            {
                int argEmptySpaces1 = 1024;
                string tmpVal = GetVar(Declaraciones.IniPath + "Server.ini", "Dioses", "Dios" + WizNum, EmptySpaces: ref argEmptySpaces1);
                NomB = tmpVal is not null ? tmpVal.ToUpper() : "";

                if (NomB.Length > 0 && NomB.Substring(0, 1) == "*" | NomB.Substring(0, 1) == "+")
                    NomB = NomB.Length > 1 ? NomB.Substring(1) : "";
                if ((name.ToUpper() ?? "") == (NomB ?? ""))
                {
                    EsDiosRet = true;
                    return EsDiosRet;
                }
            }
            EsDiosRet = false;
            return EsDiosRet;
        }

        public static bool EsSemiDios(string name)
        {
            bool EsSemiDiosRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short NumWizs;
            short WizNum;
            string NomB;

            int argEmptySpaces = 1024;
            NumWizs = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "SemiDioses", EmptySpaces: ref argEmptySpaces)));
            var loopTo = NumWizs;
            for (WizNum = 1; WizNum <= loopTo; WizNum++)
            {
                int argEmptySpaces1 = 1024;
                string tmpVal = GetVar(Declaraciones.IniPath + "Server.ini", "SemiDioses", "SemiDios" + WizNum, EmptySpaces: ref argEmptySpaces1);
                NomB = tmpVal is not null ? tmpVal.ToUpper() : "";

                if (NomB.Length > 0 && NomB.Substring(0, 1) == "*" | NomB.Substring(0, 1) == "+")
                    NomB = NomB.Length > 1 ? NomB.Substring(1) : "";
                if ((name.ToUpper() ?? "") == (NomB ?? ""))
                {
                    EsSemiDiosRet = true;
                    return EsSemiDiosRet;
                }
            }
            EsSemiDiosRet = false;
            return EsSemiDiosRet;
        }

        public static bool EsConsejero(string name)
        {
            bool EsConsejeroRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short NumWizs;
            short WizNum;
            string NomB;

            int argEmptySpaces = 1024;
            NumWizs = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "Consejeros", EmptySpaces: ref argEmptySpaces)));
            var loopTo = NumWizs;
            for (WizNum = 1; WizNum <= loopTo; WizNum++)
            {
                int argEmptySpaces1 = 1024;
                string tmpVal = GetVar(Declaraciones.IniPath + "Server.ini", "Consejeros", "Consejero" + WizNum, EmptySpaces: ref argEmptySpaces1);
                NomB = tmpVal is not null ? tmpVal.ToUpper() : "";

                if (NomB.Length > 0 && NomB.Substring(0, 1) == "*" | NomB.Substring(0, 1) == "+")
                    NomB = NomB.Length > 1 ? NomB.Substring(1) : "";
                if ((name.ToUpper() ?? "") == (NomB ?? ""))
                {
                    EsConsejeroRet = true;
                    return EsConsejeroRet;
                }
            }
            EsConsejeroRet = false;
            return EsConsejeroRet;
        }

        public static bool EsRolesMaster(string name)
        {
            bool EsRolesMasterRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short NumWizs;
            short WizNum;
            string NomB;

            int argEmptySpaces = 1024;
            NumWizs = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "RolesMasters", EmptySpaces: ref argEmptySpaces)));
            var loopTo = NumWizs;
            for (WizNum = 1; WizNum <= loopTo; WizNum++)
            {
                int argEmptySpaces1 = 1024;
                string tmpVal = GetVar(Declaraciones.IniPath + "Server.ini", "RolesMasters", "RM" + WizNum, EmptySpaces: ref argEmptySpaces1);
                NomB = tmpVal is not null ? tmpVal.ToUpper() : "";

                if (NomB.Length > 0 && NomB.Substring(0, 1) == "*" | NomB.Substring(0, 1) == "+")
                    NomB = NomB.Length > 1 ? NomB.Substring(1) : "";
                if ((name.ToUpper() ?? "") == (NomB ?? ""))
                {
                    EsRolesMasterRet = true;
                    return EsRolesMasterRet;
                }
            }
            EsRolesMasterRet = false;
            return EsRolesMasterRet;
        }


        public static int TxtDimension(string name)
        {
            return File.ReadAllLines(name).Length;
        }

        public static void CargarForbidenWords()
        {
            string[] lines = File.ReadAllLines(Declaraciones.DatPath + "NombresInvalidos.txt");
            Declaraciones.ForbidenNames = new string[lines.Length + 1];
            for (int i = 0, loopTo = lines.Length - 1; i <= loopTo; i++)
                Declaraciones.ForbidenNames[i + 1] = lines[i];
        }

        public static void CargarHechizos()
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            // ###################################################
            // #               ATENCION PELIGRO                  #
            // ###################################################
            // 
            // ¡¡¡¡ NO USAR GetVar PARA LEER Hechizos.dat !!!!
            // 
            // El que ose desafiar esta LEY, se las tendrá que ver
            // con migo. Para leer Hechizos.dat se deberá usar
            // la nueva clase clsLeerInis.
            // 
            // Alejo
            // 
            // ###################################################

            try
            {

                short Hechizo;
                var Leer = new clsIniReader();

                Leer.Initialize(Declaraciones.DatPath + "Hechizos.dat");

                // obtiene el numero de hechizos
                Declaraciones.NumeroHechizos = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("INIT", "NumeroHechizos")));

                // UPGRADE_WARNING: El límite inferior de la matriz Hechizos ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                Declaraciones.Hechizos = new Declaraciones.tHechizo[(Declaraciones.NumeroHechizos + 1)];

                // Llena la lista
                var loopTo = Declaraciones.NumeroHechizos;
                for (Hechizo = 1; Hechizo <= loopTo; Hechizo++)
                {
                    {
                        ref var withBlock = ref Declaraciones.Hechizos[Hechizo];
                        withBlock.Nombre = Leer.GetValue("Hechizo" + Hechizo, "Nombre");
                        withBlock.desc = Leer.GetValue("Hechizo" + Hechizo, "Desc");
                        withBlock.PalabrasMagicas = Leer.GetValue("Hechizo" + Hechizo, "PalabrasMagicas");

                        withBlock.HechizeroMsg = Leer.GetValue("Hechizo" + Hechizo, "HechizeroMsg");
                        withBlock.TargetMsg = Leer.GetValue("Hechizo" + Hechizo, "TargetMsg");
                        withBlock.PropioMsg = Leer.GetValue("Hechizo" + Hechizo, "PropioMsg");

                        withBlock.Tipo = (Declaraciones.TipoHechizo)Convert.ToInt32(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Tipo")));
                        withBlock.WAV = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "WAV")));
                        withBlock.FXgrh = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Fxgrh")));

                        withBlock.loops = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Loops")));

                        // .Resis = val(Leer.GetValue("Hechizo" & Hechizo, "Resis"))

                        withBlock.SubeHP = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "SubeHP")));
                        withBlock.MinHp = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MinHP")));
                        withBlock.MaxHp = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MaxHP")));

                        withBlock.SubeMana = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "SubeMana")));
                        withBlock.MiMana = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MinMana")));
                        withBlock.MaMana = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MaxMana")));

                        withBlock.SubeSta = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "SubeSta")));
                        withBlock.MinSta = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MinSta")));
                        withBlock.MaxSta = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MaxSta")));

                        withBlock.SubeHam = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "SubeHam")));
                        withBlock.MinHam = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MinHam")));
                        withBlock.MaxHam = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MaxHam")));

                        withBlock.SubeSed = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "SubeSed")));
                        withBlock.MinSed = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MinSed")));
                        withBlock.MaxSed = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MaxSed")));

                        withBlock.SubeAgilidad = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "SubeAG")));
                        withBlock.MinAgilidad = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MinAG")));
                        withBlock.MaxAgilidad = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MaxAG")));

                        withBlock.SubeFuerza = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "SubeFU")));
                        withBlock.MinFuerza = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MinFU")));
                        withBlock.MaxFuerza = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MaxFU")));

                        withBlock.SubeCarisma = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "SubeCA")));
                        withBlock.MinCarisma = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MinCA")));
                        withBlock.MaxCarisma = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MaxCA")));


                        withBlock.Invisibilidad = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Invisibilidad")));
                        withBlock.Paraliza = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Paraliza")));
                        withBlock.Inmoviliza = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Inmoviliza")));
                        withBlock.RemoverParalisis = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "RemoverParalisis")));
                        withBlock.RemoverEstupidez = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "RemoverEstupidez")));
                        withBlock.RemueveInvisibilidadParcial = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "RemueveInvisibilidadParcial")));


                        withBlock.CuraVeneno = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "CuraVeneno")));
                        withBlock.Envenena = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Envenena")));
                        withBlock.Maldicion = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Maldicion")));
                        withBlock.RemoverMaldicion = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "RemoverMaldicion")));
                        withBlock.Bendicion = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Bendicion")));
                        withBlock.Revivir = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Revivir")));

                        withBlock.Ceguera = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Ceguera")));
                        withBlock.Estupidez = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Estupidez")));

                        withBlock.Warp = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Warp")));

                        withBlock.Invoca = Convert.ToByte(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Invoca")));
                        withBlock.NumNpc = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "NumNpc")));
                        withBlock.cant = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Cant")));
                        withBlock.Mimetiza = Convert.ToByte(Migration.ParseVal(Leer.GetValue("hechizo" + Hechizo, "Mimetiza")));

                        // .Materializa = val(Leer.GetValue("Hechizo" & Hechizo, "Materializa"))
                        // .ItemIndex = val(Leer.GetValue("Hechizo" & Hechizo, "ItemIndex"))

                        withBlock.MinSkill = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "MinSkill")));
                        withBlock.ManaRequerido = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "ManaRequerido")));

                        // Barrin 30/9/03
                        withBlock.StaRequerido = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "StaRequerido")));

                        withBlock.Target = (Declaraciones.TargetType)Convert.ToInt32(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "Target")));

                        withBlock.NeedStaff = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "NeedStaff")));
                        withBlock.StaffAffected = Convert.ToBoolean(Migration.ParseVal(Leer.GetValue("Hechizo" + Hechizo, "StaffAffected")));
                    }
                }

                // UPGRADE_NOTE: El objeto Leer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
                Leer = null;
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in CargarSpawnList: " + ex.Message);
                string argdesc = "[" + ex.GetType().Name + "] Error cargando hechizos.dat: " + ex.Message;
                General.LogError(ref argdesc);
            }
        }

        public static void LoadMotd()
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short i;

            int argEmptySpaces = 1024;
            Admin.MaxLines = Convert.ToInt16(Migration.ParseVal(GetVar(AppDomain.CurrentDomain.BaseDirectory + "Dat/Motd.ini", "INIT", "NumLines", EmptySpaces: ref argEmptySpaces)));

            // UPGRADE_WARNING: El límite inferior de la matriz MOTD ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            Admin.MOTD = new Admin.tMotd[(Admin.MaxLines + 1)];
            var loopTo = Admin.MaxLines;
            for (i = 1; i <= loopTo; i++)
            {
                int argEmptySpaces1 = 1024;
                Admin.MOTD[i].texto = GetVar(AppDomain.CurrentDomain.BaseDirectory + "Dat/Motd.ini", "Motd", "Line" + i, EmptySpaces: ref argEmptySpaces1);
                Admin.MOTD[i].Formato = Constants.vbNullString;
            }
        }

        public static void DoBackUp()
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            Declaraciones.haciendoBK = true;
            short i;


            // Lo saco porque elimina elementales y mascotas - Maraxus
            // '''''''''''''lo pongo aca x sugernecia del yind
            // For i = 1 To LastNPC
            // If Npclist(i).flags.NPCActive Then
            // If Npclist(i).Contadores.TiempoExistencia > 0 Then
            // Call MuereNpc(i, 0)
            // End If
            // End If
            // Next i
            // ''''''''''/'lo pongo aca x sugernecia del yind


            modSendData.SendData(modSendData.SendTarget.ToAll, 0, Protocol.PrepareMessagePauseToggle());


            General.LimpiarMundo();
            Admin.WorldSave();
            modGuilds.v_RutinaElecciones();
            modCentinela.ResetCentinelaInfo(); // Reseteamos al centinela


            modSendData.SendData(modSendData.SendTarget.ToAll, 0, Protocol.PrepareMessagePauseToggle());

            // Call EstadisticasWeb.Informar(EVENTO_NUEVO_CLAN, 0)

            Declaraciones.haciendoBK = false;

            // Log
            General.AppendLog("logs/BackUps.log", Conversions.ToString(DateTime.Today) + " " + Conversions.ToString(DateAndTime.TimeOfDay));
        }

        public static void GrabarMapa(int Map, string MAPFILE)
        {
            try
            {
                int Y;
                int X;
                byte ByFlags;
                var TempInt = default(short);
                int LoopC;

                // Write .map and .inf files using BinaryWriter
                using (var fsMap = new FileStream(MAPFILE + ".Map", FileMode.Create))
                using (var writerMap = new BinaryWriter(fsMap))
                using (var fsInf = new FileStream(MAPFILE + ".Inf", FileMode.Create))
                using (var writerInf = new BinaryWriter(fsInf))
                {

                    // map Header
                    writerMap.Write(Declaraciones.MapInfo_Renamed[Map].MapVersion); // Int16
                                                                                    // Skip 263 bytes from current position (padding)
                    fsMap.Position = fsMap.Position + 263L;
                    writerMap.Write(TempInt);
                    writerMap.Write(TempInt);
                    writerMap.Write(TempInt);
                    writerMap.Write(TempInt);

                    // inf Header
                    writerInf.Write(TempInt);
                    writerInf.Write(TempInt);
                    writerInf.Write(TempInt);
                    writerInf.Write(TempInt);
                    writerInf.Write(TempInt);

                    // Write tile data
                    for (Y = Declaraciones.YMinMapSize; Y <= Declaraciones.YMaxMapSize; Y++)
                    {
                        for (X = Declaraciones.XMinMapSize; X <= Declaraciones.XMaxMapSize; X++)
                        {
                            {
                                ref var withBlock = ref Declaraciones.MapData[Map, X, Y];
                                ByFlags = 0;

                                if (withBlock.Blocked != 0)
                                    ByFlags = Convert.ToByte(ByFlags | 1);
                                if (withBlock.Graphic[2] != 0)
                                    ByFlags = Convert.ToByte(ByFlags | 2);
                                if (withBlock.Graphic[3] != 0)
                                    ByFlags = Convert.ToByte(ByFlags | 4);
                                if (withBlock.Graphic[4] != 0)
                                    ByFlags = Convert.ToByte(ByFlags | 8);
                                if (withBlock.trigger != 0)
                                    ByFlags = Convert.ToByte(ByFlags | 16);

                                writerMap.Write(ByFlags);
                                writerMap.Write(withBlock.Graphic[1]);

                                for (LoopC = 2; LoopC <= 4; LoopC++)
                                {
                                    if (withBlock.Graphic[LoopC] != 0)
                                        writerMap.Write(withBlock.Graphic[LoopC]);
                                }

                                if (withBlock.trigger != 0)
                                    writerMap.Write(Convert.ToInt16((int)withBlock.trigger));

                                // .inf file
                                ByFlags = 0;

                                if (withBlock.ObjInfo.ObjIndex > 0)
                                {
                                    if (Declaraciones.ObjData_Renamed[withBlock.ObjInfo.ObjIndex].OBJType == Declaraciones.eOBJType.otFogata)
                                    {
                                        withBlock.ObjInfo.ObjIndex = 0;
                                        withBlock.ObjInfo.Amount = 0;
                                    }
                                }

                                if (withBlock.TileExit.Map != 0)
                                    ByFlags = Convert.ToByte(ByFlags | 1);
                                if (withBlock.NpcIndex != 0)
                                    ByFlags = Convert.ToByte(ByFlags | 2);
                                if (withBlock.ObjInfo.ObjIndex != 0)
                                    ByFlags = Convert.ToByte(ByFlags | 4);

                                writerInf.Write(ByFlags);

                                if (withBlock.TileExit.Map != 0)
                                {
                                    writerInf.Write(withBlock.TileExit.Map);
                                    writerInf.Write(withBlock.TileExit.X);
                                    writerInf.Write(withBlock.TileExit.Y);
                                }

                                if (withBlock.NpcIndex != 0)
                                    writerInf.Write(Declaraciones.Npclist[withBlock.NpcIndex].Numero);

                                if (withBlock.ObjInfo.ObjIndex != 0)
                                {
                                    writerInf.Write(withBlock.ObjInfo.ObjIndex);
                                    writerInf.Write(withBlock.ObjInfo.Amount);
                                }
                            }
                        }
                    }
                }

                {
                    ref var withBlock1 = ref Declaraciones.MapInfo_Renamed[Map];

                    // write .dat file
                    WriteVar(MAPFILE + ".dat", "Mapa" + Map, "Name", withBlock1.name);
                    WriteVar(MAPFILE + ".dat", "Mapa" + Map, "MusicNum", withBlock1.Music);
                    WriteVar(MAPFILE + ".dat", "mapa" + Map, "MagiaSinefecto", withBlock1.MagiaSinEfecto.ToString());
                    WriteVar(MAPFILE + ".dat", "mapa" + Map, "InviSinEfecto", withBlock1.InviSinEfecto.ToString());
                    WriteVar(MAPFILE + ".dat", "mapa" + Map, "ResuSinEfecto", withBlock1.ResuSinEfecto.ToString());

                    WriteVar(MAPFILE + ".dat", "Mapa" + Map, "Terreno", withBlock1.Terreno);
                    WriteVar(MAPFILE + ".dat", "Mapa" + Map, "Zona", withBlock1.Zona);
                    WriteVar(MAPFILE + ".dat", "Mapa" + Map, "Restringir", withBlock1.Restringir);
                    WriteVar(MAPFILE + ".dat", "Mapa" + Map, "BackUp", Conversion.Str(withBlock1.BackUp));

                    if (withBlock1.Pk)
                    {
                        WriteVar(MAPFILE + ".dat", "Mapa" + Map, "Pk", "0");
                    }
                    else
                    {
                        WriteVar(MAPFILE + ".dat", "Mapa" + Map, "Pk", "1");
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in GrabarMapa: " + ex.Message);
            }
        }

        public static void LoadArmasHerreria()
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short N, lc;

            int argEmptySpaces = 1024;
            N = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmasHerrero.dat", "INIT", "NumArmas", EmptySpaces: ref argEmptySpaces)));

            // UPGRADE_WARNING: El límite inferior de la matriz ArmasHerrero ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            Array.Resize(ref Declaraciones.ArmasHerrero, N + 1);

            var loopTo = N;
            for (lc = 1; lc <= loopTo; lc++)
            {
                int argEmptySpaces1 = 1024;
                Declaraciones.ArmasHerrero[lc] = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmasHerrero.dat", "Arma" + lc, "Index", EmptySpaces: ref argEmptySpaces1)));
            }
        }

        public static void LoadArmadurasHerreria()
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short N, lc;

            int argEmptySpaces = 1024;
            N = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmadurasHerrero.dat", "INIT", "NumArmaduras", EmptySpaces: ref argEmptySpaces)));

            // UPGRADE_WARNING: El límite inferior de la matriz ArmadurasHerrero ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            Array.Resize(ref Declaraciones.ArmadurasHerrero, N + 1);

            var loopTo = N;
            for (lc = 1; lc <= loopTo; lc++)
            {
                int argEmptySpaces1 = 1024;
                Declaraciones.ArmadurasHerrero[lc] = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmadurasHerrero.dat", "Armadura" + lc, "Index", EmptySpaces: ref argEmptySpaces1)));
            }
        }

        public static void LoadBalance()
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: 15/04/2010
            // 15/04/2010: ZaMa - Agrego recompensas faccionarias.
            // ***************************************************

            int i;

            // Modificadores de Clase
            for (i = 1; i <= Declaraciones.NUMCLASES; i++)
            {
                {
                    ref var withBlock = ref Declaraciones.ModClase_Renamed[i];
                    int argEmptySpaces = 1024;
                    withBlock.Evasion = Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "MODEVASION", Declaraciones.ListaClases[i], EmptySpaces: ref argEmptySpaces));
                    int argEmptySpaces1 = 1024;
                    withBlock.AtaqueArmas = Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "MODATAQUEARMAS", Declaraciones.ListaClases[i], EmptySpaces: ref argEmptySpaces1));
                    int argEmptySpaces2 = 1024;
                    withBlock.AtaqueProyectiles = Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "MODATAQUEPROYECTILES", Declaraciones.ListaClases[i], EmptySpaces: ref argEmptySpaces2));
                    int argEmptySpaces3 = 1024;
                    withBlock.AtaqueWrestling = Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "MODATAQUEWRESTLING", Declaraciones.ListaClases[i], EmptySpaces: ref argEmptySpaces3));
                    int argEmptySpaces4 = 1024;
                    withBlock.DañoArmas = Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "MODDAÑOARMAS", Declaraciones.ListaClases[i], EmptySpaces: ref argEmptySpaces4));
                    int argEmptySpaces5 = 1024;
                    withBlock.DañoProyectiles = Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "MODDAÑOPROYECTILES", Declaraciones.ListaClases[i], EmptySpaces: ref argEmptySpaces5));
                    int argEmptySpaces6 = 1024;
                    withBlock.DañoWrestling = Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "MODDAÑOWRESTLING", Declaraciones.ListaClases[i], EmptySpaces: ref argEmptySpaces6));
                    int argEmptySpaces7 = 1024;
                    withBlock.Escudo = Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "MODESCUDO", Declaraciones.ListaClases[i], EmptySpaces: ref argEmptySpaces7));
                }
            }

            // Modificadores de Raza
            for (i = 1; i <= Declaraciones.NUMRAZAS; i++)
            {
                {
                    ref var withBlock1 = ref Declaraciones.ModRaza_Renamed[i];
                    int argEmptySpaces8 = 1024;
                    withBlock1.Fuerza = Convert.ToSingle(Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "MODRAZA", Declaraciones.ListaRazas[i] + "Fuerza", EmptySpaces: ref argEmptySpaces8)));
                    int argEmptySpaces9 = 1024;
                    withBlock1.Agilidad = Convert.ToSingle(Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "MODRAZA", Declaraciones.ListaRazas[i] + "Agilidad", EmptySpaces: ref argEmptySpaces9)));
                    int argEmptySpaces10 = 1024;
                    withBlock1.Inteligencia = Convert.ToSingle(Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "MODRAZA", Declaraciones.ListaRazas[i] + "Inteligencia", EmptySpaces: ref argEmptySpaces10)));
                    int argEmptySpaces11 = 1024;
                    withBlock1.Carisma = Convert.ToSingle(Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "MODRAZA", Declaraciones.ListaRazas[i] + "Carisma", EmptySpaces: ref argEmptySpaces11)));
                    int argEmptySpaces12 = 1024;
                    withBlock1.Constitucion = Convert.ToSingle(Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "MODRAZA", Declaraciones.ListaRazas[i] + "Constitucion", EmptySpaces: ref argEmptySpaces12)));
                }
            }

            // Modificadores de Vida
            for (i = 1; i <= Declaraciones.NUMCLASES; i++)
            {
                int argEmptySpaces13 = 1024;
                Declaraciones.ModVida[i] = Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "MODVIDA", Declaraciones.ListaClases[i], EmptySpaces: ref argEmptySpaces13));
            }

            // Distribución de Vida
            for (i = 1; i <= 5; i++)
            {
                int argEmptySpaces14 = 1024;
                Declaraciones.DistribucionEnteraVida[i] = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "DISTRIBUCION", "E" + i.ToString(), EmptySpaces: ref argEmptySpaces14)));
            }
            for (i = 1; i <= 4; i++)
            {
                int argEmptySpaces15 = 1024;
                Declaraciones.DistribucionSemienteraVida[i] = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "DISTRIBUCION", "S" + i.ToString(), EmptySpaces: ref argEmptySpaces15)));
            }

            // Extra
            int argEmptySpaces16 = 1024;
            Admin.PorcentajeRecuperoMana = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "EXTRA", "PorcentajeRecuperoMana", EmptySpaces: ref argEmptySpaces16)));

            // Party
            int argEmptySpaces17 = 1024;
            mdParty.ExponenteNivelParty = Convert.ToSingle(Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "PARTY", "ExponenteNivelParty", EmptySpaces: ref argEmptySpaces17)));

            // Recompensas faccionarias
            for (i = 1; i <= ModFacciones.NUM_RANGOS_FACCION; i++)
            {
                int argEmptySpaces18 = 1024;
                ModFacciones.RecompensaFacciones[i - 1] = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.DatPath + "Balance.dat", "RECOMPENSAFACCION", "Rango" + i, EmptySpaces: ref argEmptySpaces18)));
            }
        }

        public static void LoadObjCarpintero()
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short N, lc;

            int argEmptySpaces = 1024;
            N = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ObjCarpintero.dat", "INIT", "NumObjs", EmptySpaces: ref argEmptySpaces)));

            // UPGRADE_WARNING: El límite inferior de la matriz ObjCarpintero ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            Array.Resize(ref Declaraciones.ObjCarpintero, N + 1);

            var loopTo = N;
            for (lc = 1; lc <= loopTo; lc++)
            {
                int argEmptySpaces1 = 1024;
                Declaraciones.ObjCarpintero[lc] = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ObjCarpintero.dat", "Obj" + lc, "Index", EmptySpaces: ref argEmptySpaces1)));
            }
        }


        public static void LoadOBJData()
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            // ###################################################
            // #               ATENCION PELIGRO                  #
            // ###################################################
            // 
            // ¡¡¡¡ NO USAR GetVar PARA LEER DESDE EL OBJ.DAT !!!!
            // 
            // El que ose desafiar esta LEY, se las tendrá que ver
            // con migo. Para leer desde el OBJ.DAT se deberá usar
            // la nueva clase clsLeerInis.
            // 
            // Alejo
            // 
            // ###################################################

            // Call LogTarea("Sub LoadOBJData")

            try
            {

                // *****************************************************************
                // Carga la lista de objetos
                // *****************************************************************
                // UPGRADE_NOTE: Object se actualizó a Object_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
                short Object_Renamed;
                var Leer = new clsIniReader();

                Leer.Initialize(Declaraciones.DatPath + "Obj.dat");

                // obtiene el numero de obj
                Declaraciones.NumObjDatas = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("INIT", "NumObjs")));

                // UPGRADE_WARNING: Es posible que la matriz ObjData_Renamed necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
                // UPGRADE_WARNING: El límite inferior de la matriz ObjData_Renamed ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                // UPGRADE_NOTE: ObjData se actualizó a ObjData_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
                Array.Resize(ref Declaraciones.ObjData_Renamed, Declaraciones.NumObjDatas + 1);
                ArrayInitializers.InitializeStruct(Declaraciones.ObjData_Renamed);


                // Llena la lista
                short i;
                short N;
                string S;
                var loopTo = Declaraciones.NumObjDatas;
                for (Object_Renamed = 1; Object_Renamed <= loopTo; Object_Renamed++)
                {
                    {
                        ref var withBlock = ref Declaraciones.ObjData_Renamed[Object_Renamed];
                        withBlock.name = Leer.GetValue("OBJ" + Object_Renamed, "Name");

                        // Pablo (ToxicWaste) Log de Objetos.
                        withBlock.Log = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Log")));
                        withBlock.NoLog = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "NoLog")));
                        // 07/09/07

                        withBlock.GrhIndex = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "GrhIndex")));
                        if (withBlock.GrhIndex == 0)
                        {
                            withBlock.GrhIndex = withBlock.GrhIndex;
                        }

                        withBlock.OBJType = (Declaraciones.eOBJType)Convert.ToInt32(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "ObjType")));

                        withBlock.Newbie = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Newbie")));

                        switch (withBlock.OBJType)
                        {
                            case Declaraciones.eOBJType.otArmadura:
                                {
                                    withBlock.Real = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Real")));
                                    withBlock.Caos = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Caos")));
                                    withBlock.LingH = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingH")));
                                    withBlock.LingP = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingP")));
                                    withBlock.LingO = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingO")));
                                    withBlock.SkHerreria = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "SkHerreria")));
                                    break;
                                }

                            case Declaraciones.eOBJType.otESCUDO:
                                {
                                    withBlock.ShieldAnim = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Anim")));
                                    withBlock.LingH = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingH")));
                                    withBlock.LingP = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingP")));
                                    withBlock.LingO = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingO")));
                                    withBlock.SkHerreria = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "SkHerreria")));
                                    withBlock.Real = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Real")));
                                    withBlock.Caos = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Caos")));
                                    break;
                                }

                            case Declaraciones.eOBJType.otCASCO:
                                {
                                    withBlock.CascoAnim = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Anim")));
                                    withBlock.LingH = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingH")));
                                    withBlock.LingP = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingP")));
                                    withBlock.LingO = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingO")));
                                    withBlock.SkHerreria = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "SkHerreria")));
                                    withBlock.Real = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Real")));
                                    withBlock.Caos = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Caos")));
                                    break;
                                }

                            case Declaraciones.eOBJType.otWeapon:
                                {
                                    withBlock.WeaponAnim = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Anim")));
                                    withBlock.Apuñala = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Apuñala")));
                                    withBlock.Envenena = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Envenena")));
                                    withBlock.MaxHIT = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MaxHIT")));
                                    withBlock.MinHIT = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MinHIT")));
                                    withBlock.proyectil = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Proyectil")));
                                    withBlock.Municion = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Municiones")));
                                    withBlock.StaffPower = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "StaffPower")));
                                    withBlock.StaffDamageBonus = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "StaffDamageBonus")));
                                    withBlock.Refuerzo = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Refuerzo")));

                                    withBlock.LingH = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingH")));
                                    withBlock.LingP = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingP")));
                                    withBlock.LingO = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingO")));
                                    withBlock.SkHerreria = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "SkHerreria")));
                                    withBlock.Real = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Real")));
                                    withBlock.Caos = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Caos")));

                                    withBlock.WeaponRazaEnanaAnim = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "RazaEnanaAnim")));
                                    break;
                                }

                            case Declaraciones.eOBJType.otInstrumentos:
                                {
                                    withBlock.Snd1 = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "SND1")));
                                    withBlock.Snd2 = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "SND2")));
                                    withBlock.Snd3 = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "SND3")));
                                    // Pablo (ToxicWaste)
                                    withBlock.Real = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Real")));
                                    withBlock.Caos = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Caos")));
                                    break;
                                }

                            case Declaraciones.eOBJType.otMinerales:
                                {
                                    withBlock.MinSkill = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MinSkill")));
                                    break;
                                }

                            case Declaraciones.eOBJType.otPuertas:
                            case Declaraciones.eOBJType.otBotellaVacia:
                            case Declaraciones.eOBJType.otBotellaLlena:
                                {
                                    withBlock.IndexAbierta = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "IndexAbierta")));
                                    withBlock.IndexCerrada = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "IndexCerrada")));
                                    withBlock.IndexCerradaLlave = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "IndexCerradaLlave")));
                                    break;
                                }

                            case Declaraciones.eOBJType.otPociones:
                                {
                                    withBlock.TipoPocion = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "TipoPocion")));
                                    withBlock.MaxModificador = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MaxModificador")));
                                    withBlock.MinModificador = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MinModificador")));
                                    withBlock.DuracionEfecto = Convert.ToInt32(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "DuracionEfecto")));
                                    break;
                                }

                            case Declaraciones.eOBJType.otBarcos:
                                {
                                    withBlock.MinSkill = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MinSkill")));
                                    withBlock.MaxHIT = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MaxHIT")));
                                    withBlock.MinHIT = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MinHIT")));
                                    break;
                                }

                            case Declaraciones.eOBJType.otFlechas:
                                {
                                    withBlock.MaxHIT = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MaxHIT")));
                                    withBlock.MinHIT = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MinHIT")));
                                    withBlock.Envenena = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Envenena")));
                                    withBlock.Paraliza = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Paraliza")));
                                    break;
                                }

                            case Declaraciones.eOBJType.otAnillo: // Pablo (ToxicWaste)
                                {
                                    withBlock.LingH = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingH")));
                                    withBlock.LingP = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingP")));
                                    withBlock.LingO = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingO")));
                                    withBlock.SkHerreria = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "SkHerreria")));
                                    withBlock.MaxHIT = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MaxHIT")));
                                    withBlock.MinHIT = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MinHIT")));
                                    break;
                                }

                            case Declaraciones.eOBJType.otTeleport:
                                {
                                    withBlock.Radio = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Radio")));
                                    break;
                                }

                            case Declaraciones.eOBJType.otMochilas:
                                {
                                    withBlock.MochilaType = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MochilaType")));
                                    break;
                                }

                            case Declaraciones.eOBJType.otForos:
                                {
                                    modForum.AddForum(Leer.GetValue("OBJ" + Object_Renamed, "ID"));
                                    break;
                                }

                        }

                        withBlock.Ropaje = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "NumRopaje")));
                        withBlock.HechizoIndex = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "HechizoIndex")));

                        withBlock.LingoteIndex = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "LingoteIndex")));

                        withBlock.MineralIndex = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MineralIndex")));

                        withBlock.MaxHp = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MaxHP")));
                        withBlock.MinHp = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MinHP")));

                        withBlock.Mujer = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Mujer")));
                        withBlock.Hombre = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Hombre")));

                        withBlock.MinHam = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MinHam")));
                        withBlock.MinSed = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MinAgu")));

                        withBlock.MinDef = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MINDEF")));
                        withBlock.MaxDef = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MAXDEF")));
                        withBlock.def = Convert.ToInt16((withBlock.MinDef + withBlock.MaxDef) / 2);

                        withBlock.RazaEnana = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "RazaEnana")));
                        withBlock.RazaDrow = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "RazaDrow")));
                        withBlock.RazaElfa = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "RazaElfa")));
                        withBlock.RazaGnoma = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "RazaGnoma")));
                        withBlock.RazaHumana = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "RazaHumana")));

                        withBlock.Valor = Convert.ToInt32(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Valor")));

                        withBlock.Crucial = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Crucial")));

                        withBlock.Cerrada = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "abierta")));
                        if (withBlock.Cerrada == 1)
                        {
                            withBlock.Llave = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Llave")));
                            withBlock.clave = Convert.ToInt32(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Clave")));
                        }

                        // Puertas y llaves
                        withBlock.clave = Convert.ToInt32(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Clave")));

                        withBlock.texto = Leer.GetValue("OBJ" + Object_Renamed, "Texto");
                        withBlock.GrhSecundario = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "VGrande")));

                        withBlock.Agarrable = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Agarrable")));
                        withBlock.ForoID = Leer.GetValue("OBJ" + Object_Renamed, "ID");

                        withBlock.Acuchilla = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Acuchilla")));

                        withBlock.Guante = Convert.ToByte(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Guante")));

                        // CHECK: !!! Esto es provisorio hasta que los de Dateo cambien los valores de string a numerico
                        for (i = 1; i <= Declaraciones.NUMCLASES; i++)
                        {
                            string tmpVal = Leer.GetValue("OBJ" + Object_Renamed, "CP" + i);
                            S = tmpVal is not null ? tmpVal.ToUpper() : "";
                            N = 1;
                            while (S.Length > 0 & ((Declaraciones.ListaClases[N] is not null ? Declaraciones.ListaClases[N].ToUpper() : "") ?? "") != (S ?? ""))
                                N = Convert.ToInt16(N + 1);
                            withBlock.ClaseProhibida[i] = (Declaraciones.eClass)Convert.ToInt32(Migration.migr_LenB(S) > 0 ? N : 0);
                        }

                        withBlock.DefensaMagicaMax = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "DefensaMagicaMax")));
                        withBlock.DefensaMagicaMin = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "DefensaMagicaMin")));

                        withBlock.SkCarpinteria = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "SkCarpinteria")));

                        if (withBlock.SkCarpinteria > 0)
                            withBlock.Madera = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Madera")));
                        withBlock.MaderaElfica = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MaderaElfica")));

                        // Bebidas
                        withBlock.MinSta = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "MinST")));

                        withBlock.NoSeCae = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "NoSeCae")));

                        withBlock.Upgrade = Convert.ToInt16(Migration.ParseVal(Leer.GetValue("OBJ" + Object_Renamed, "Upgrade")));

                    }
                }


                // UPGRADE_NOTE: El objeto Leer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
                Leer = null;

                // Inicializo los foros faccionarios
                modForum.AddForum(modForum.FORO_CAOS_ID);
                modForum.AddForum(modForum.FORO_REAL_ID);
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in LoadMotd: " + ex.Message);
                string argdesc = "[" + ex.GetType().Name + "] Error cargando objetos: " + ex.Message;
                General.LogError(ref argdesc);
            }
        }

        public static void LoadUserStats(short UserIndex, ref clsIniReader UserFile)
        {
            // *************************************************
            // Author: Unknown
            // Last modified: 11/19/2009
            // 11/19/2009: Pato - Load the EluSkills and ExpSkills
            // *************************************************
            int LoopC;

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                {
                    ref var withBlock1 = ref withBlock.Stats;
                    for (LoopC = 1; LoopC <= Declaraciones.NUMATRIBUTOS; LoopC++)
                    {
                        withBlock1.UserAtributos[LoopC] = Convert.ToByte(UserFile.GetValue("ATRIBUTOS", "AT" + LoopC));
                        withBlock1.UserAtributosBackUP[LoopC] = withBlock1.UserAtributos[LoopC];
                    }

                    for (LoopC = 1; LoopC <= Declaraciones.NUMSKILLS; LoopC++)
                    {
                        withBlock1.UserSkills[LoopC] = Convert.ToByte(UserFile.GetValue("SKILLS", "SK" + LoopC));
                        withBlock1.EluSkills[LoopC] = Convert.ToInt16(UserFile.GetValue("SKILLS", "ELUSK" + LoopC));
                        withBlock1.ExpSkills[LoopC] = Convert.ToInt16(UserFile.GetValue("SKILLS", "EXPSK" + LoopC));
                    }

                    for (LoopC = 1; LoopC <= Declaraciones.MAXUSERHECHIZOS; LoopC++)
                        withBlock1.UserHechizos[LoopC] = Convert.ToInt16(UserFile.GetValue("Hechizos", "H" + LoopC));

                    withBlock1.GLD = Convert.ToInt32(UserFile.GetValue("STATS", "GLD"));
                    withBlock1.Banco = Convert.ToInt32(UserFile.GetValue("STATS", "BANCO"));

                    withBlock1.MaxHp = Convert.ToInt16(UserFile.GetValue("STATS", "MaxHP"));
                    withBlock1.MinHp = Convert.ToInt16(UserFile.GetValue("STATS", "MinHP"));

                    withBlock1.MinSta = Convert.ToInt16(UserFile.GetValue("STATS", "MinSTA"));
                    withBlock1.MaxSta = Convert.ToInt16(UserFile.GetValue("STATS", "MaxSTA"));

                    withBlock1.MaxMAN = Convert.ToInt16(UserFile.GetValue("STATS", "MaxMAN"));
                    withBlock1.MinMAN = Convert.ToInt16(UserFile.GetValue("STATS", "MinMAN"));

                    withBlock1.MaxHIT = Convert.ToInt16(UserFile.GetValue("STATS", "MaxHIT"));
                    withBlock1.MinHIT = Convert.ToInt16(UserFile.GetValue("STATS", "MinHIT"));

                    withBlock1.MaxAGU = Convert.ToByte(UserFile.GetValue("STATS", "MaxAGU"));
                    withBlock1.MinAGU = Convert.ToByte(UserFile.GetValue("STATS", "MinAGU"));

                    withBlock1.MaxHam = Convert.ToByte(UserFile.GetValue("STATS", "MaxHAM"));
                    withBlock1.MinHam = Convert.ToByte(UserFile.GetValue("STATS", "MinHAM"));

                    withBlock1.SkillPts = Convert.ToInt16(UserFile.GetValue("STATS", "SkillPtsLibres"));

                    withBlock1.Exp = Convert.ToDouble(UserFile.GetValue("STATS", "EXP"));
                    withBlock1.ELU = Convert.ToInt32(UserFile.GetValue("STATS", "ELU"));
                    withBlock1.ELV = Convert.ToByte(UserFile.GetValue("STATS", "ELV"));


                    withBlock1.UsuariosMatados = Convert.ToInt32(UserFile.GetValue("MUERTES", "UserMuertes"));
                    withBlock1.NPCsMuertos = Convert.ToInt16(UserFile.GetValue("MUERTES", "NpcsMuertes"));
                }

                {
                    ref var withBlock2 = ref withBlock.flags;
                    if (Convert.ToByte(UserFile.GetValue("CONSEJO", "PERTENECE")) != 0)
                        withBlock2.Privilegios = withBlock2.Privilegios | Declaraciones.PlayerType.RoyalCouncil;

                    if (Convert.ToByte(UserFile.GetValue("CONSEJO", "PERTENECECAOS")) != 0)
                        withBlock2.Privilegios = withBlock2.Privilegios | Declaraciones.PlayerType.ChaosCouncil;
                }
            }
        }

        public static void LoadUserReputacion(short UserIndex, ref clsIniReader UserFile)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].Reputacion;
                withBlock.AsesinoRep = Convert.ToInt32(Migration.ParseVal(UserFile.GetValue("REP", "Asesino")));
                withBlock.BandidoRep = Convert.ToInt32(Migration.ParseVal(UserFile.GetValue("REP", "Bandido")));
                withBlock.BurguesRep = Convert.ToInt32(Migration.ParseVal(UserFile.GetValue("REP", "Burguesia")));
                withBlock.LadronesRep = Convert.ToInt32(Migration.ParseVal(UserFile.GetValue("REP", "Ladrones")));
                withBlock.NobleRep = Convert.ToInt32(Migration.ParseVal(UserFile.GetValue("REP", "Nobles")));
                withBlock.PlebeRep = Convert.ToInt32(Migration.ParseVal(UserFile.GetValue("REP", "Plebe")));
                withBlock.Promedio = Convert.ToInt32(Migration.ParseVal(UserFile.GetValue("REP", "Promedio")));
            }
        }

        public static void LoadUserInit(short UserIndex, ref clsIniReader UserFile)
        {
            // *************************************************
            // Author: Unknown
            // Last modified: 19/11/2006
            // Loads the Users records
            // 23/01/2007 Pablo (ToxicWaste) - Agrego NivelIngreso, FechaIngreso, MatadosIngreso y NextRecompensa.
            // 23/01/2007 Pablo (ToxicWaste) - Quito CriminalesMatados de Stats porque era redundante.
            // *************************************************
            int LoopC;
            string ln;

            short NpcIndex;
            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex];
                {
                    ref var withBlock1 = ref withBlock.Faccion;
                    withBlock1.ArmadaReal = Convert.ToByte(UserFile.GetValue("FACCIONES", "EjercitoReal"));
                    withBlock1.FuerzasCaos = Convert.ToByte(UserFile.GetValue("FACCIONES", "EjercitoCaos"));
                    withBlock1.CiudadanosMatados = Convert.ToInt32(UserFile.GetValue("FACCIONES", "CiudMatados"));
                    withBlock1.CriminalesMatados = Convert.ToInt32(UserFile.GetValue("FACCIONES", "CrimMatados"));
                    withBlock1.RecibioArmaduraCaos = Convert.ToByte(UserFile.GetValue("FACCIONES", "rArCaos"));
                    withBlock1.RecibioArmaduraReal = Convert.ToByte(UserFile.GetValue("FACCIONES", "rArReal"));
                    withBlock1.RecibioExpInicialCaos = Convert.ToByte(UserFile.GetValue("FACCIONES", "rExCaos"));
                    withBlock1.RecibioExpInicialReal = Convert.ToByte(UserFile.GetValue("FACCIONES", "rExReal"));
                    withBlock1.RecompensasCaos = Convert.ToInt32(UserFile.GetValue("FACCIONES", "recCaos"));
                    withBlock1.RecompensasReal = Convert.ToInt32(UserFile.GetValue("FACCIONES", "recReal"));
                    withBlock1.Reenlistadas = Convert.ToByte(UserFile.GetValue("FACCIONES", "Reenlistadas"));
                    withBlock1.NivelIngreso = Convert.ToInt16(UserFile.GetValue("FACCIONES", "NivelIngreso"));
                    withBlock1.FechaIngreso = UserFile.GetValue("FACCIONES", "FechaIngreso");
                    withBlock1.MatadosIngreso = Convert.ToInt16(UserFile.GetValue("FACCIONES", "MatadosIngreso"));
                    withBlock1.NextRecompensa = Convert.ToInt16(UserFile.GetValue("FACCIONES", "NextRecompensa"));
                }

                {
                    ref var withBlock2 = ref withBlock.flags;
                    withBlock2.Muerto = Convert.ToByte(UserFile.GetValue("FLAGS", "Muerto"));
                    withBlock2.Escondido = Convert.ToByte(UserFile.GetValue("FLAGS", "Escondido"));

                    withBlock2.Hambre = Convert.ToByte(UserFile.GetValue("FLAGS", "Hambre"));
                    withBlock2.Sed = Convert.ToByte(UserFile.GetValue("FLAGS", "Sed"));
                    withBlock2.Desnudo = Convert.ToByte(UserFile.GetValue("FLAGS", "Desnudo"));
                    withBlock2.Navegando = Convert.ToByte(UserFile.GetValue("FLAGS", "Navegando"));
                    withBlock2.Envenenado = Convert.ToByte(UserFile.GetValue("FLAGS", "Envenenado"));
                    withBlock2.Paralizado = Convert.ToByte(UserFile.GetValue("FLAGS", "Paralizado"));

                    // Matrix
                    withBlock2.lastMap = Convert.ToInt16(UserFile.GetValue("FLAGS", "LastMap"));
                }

                if (withBlock.flags.Paralizado == 1)
                {
                    withBlock.Counters.Paralisis = Admin.IntervaloParalizado;
                }


                withBlock.Counters.Pena = Convert.ToInt32(UserFile.GetValue("COUNTERS", "Pena"));
                withBlock.Counters.AsignedSkills = Convert.ToByte(Migration.ParseVal(UserFile.GetValue("COUNTERS", "SkillsAsignados")));

                withBlock.email = UserFile.GetValue("CONTACTO", "Email");

                withBlock.Genero = (Declaraciones.eGenero)Convert.ToByte(UserFile.GetValue("INIT", "Genero"));
                withBlock.clase = (Declaraciones.eClass)Convert.ToInt32(UserFile.GetValue("INIT", "Clase"));
                withBlock.raza = (Declaraciones.eRaza)Convert.ToByte(UserFile.GetValue("INIT", "Raza"));
                withBlock.Hogar = (Declaraciones.eCiudad)Convert.ToInt32(UserFile.GetValue("INIT", "Hogar"));
                withBlock.Char_Renamed.heading = (Declaraciones.eHeading)Convert.ToByte(UserFile.GetValue("INIT", "Heading"));


                {
                    ref var withBlock3 = ref withBlock.OrigChar;
                    withBlock3.Head = Convert.ToInt16(UserFile.GetValue("INIT", "Head"));
                    withBlock3.body = Convert.ToInt16(UserFile.GetValue("INIT", "Body"));
                    withBlock3.WeaponAnim = Convert.ToInt16(UserFile.GetValue("INIT", "Arma"));
                    withBlock3.ShieldAnim = Convert.ToInt16(UserFile.GetValue("INIT", "Escudo"));
                    withBlock3.CascoAnim = Convert.ToInt16(UserFile.GetValue("INIT", "Casco"));

                    withBlock3.heading = Declaraciones.eHeading.SOUTH;
                }

                withBlock.UpTime = Convert.ToInt32(UserFile.GetValue("INIT", "UpTime"));

                if (withBlock.flags.Muerto == 0)
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Char. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    withBlock.Char_Renamed = withBlock.OrigChar;
                }
                else
                {
                    withBlock.Char_Renamed.body = Declaraciones.iCuerpoMuerto;
                    withBlock.Char_Renamed.Head = Declaraciones.iCabezaMuerto;
                    withBlock.Char_Renamed.WeaponAnim = Declaraciones.NingunArma;
                    withBlock.Char_Renamed.ShieldAnim = Declaraciones.NingunEscudo;
                    withBlock.Char_Renamed.CascoAnim = Declaraciones.NingunCasco;
                }


                withBlock.desc = UserFile.GetValue("INIT", "Desc");

                string positionStr = UserFile.GetValue("INIT", "Position");
                string localReadField() { string argText = positionStr; var ret = General.ReadField(1, ref argText, 45); return ret; }

                withBlock.Pos.Map = Convert.ToInt16(localReadField());
                string localReadField1() { string argText1 = positionStr; var ret = General.ReadField(2, ref argText1, 45); return ret; }

                withBlock.Pos.X = Convert.ToInt16(localReadField1());
                string localReadField2() { string argText2 = positionStr; var ret = General.ReadField(3, ref argText2, 45); return ret; }

                withBlock.Pos.Y = Convert.ToInt16(localReadField2());

                withBlock.Invent.NroItems = Convert.ToInt16(UserFile.GetValue("Inventory", "CantidadItems"));

                // [KEVIN]--------------------------------------------------------------------
                // ***********************************************************************************
                withBlock.BancoInvent.NroItems = Convert.ToInt16(UserFile.GetValue("BancoInventory", "CantidadItems"));
                // Lista de objetos del banco
                for (LoopC = 1; LoopC <= Declaraciones.MAX_BANCOINVENTORY_SLOTS; LoopC++)
                {
                    ln = UserFile.GetValue("BancoInventory", "Obj" + LoopC);
                    withBlock.BancoInvent.Object_Renamed[LoopC].ObjIndex = Convert.ToInt16(General.ReadField(1, ref ln, 45));
                    withBlock.BancoInvent.Object_Renamed[LoopC].Amount = Convert.ToInt16(General.ReadField(2, ref ln, 45));
                }
                // ------------------------------------------------------------------------------------
                // [/KEVIN]*****************************************************************************


                // Lista de objetos
                for (LoopC = 1; LoopC <= Declaraciones.MAX_INVENTORY_SLOTS; LoopC++)
                {
                    ln = UserFile.GetValue("Inventory", "Obj" + LoopC);
                    withBlock.Invent.Object_Renamed[LoopC].ObjIndex = Convert.ToInt16(General.ReadField(1, ref ln, 45));
                    withBlock.Invent.Object_Renamed[LoopC].Amount = Convert.ToInt16(General.ReadField(2, ref ln, 45));
                    withBlock.Invent.Object_Renamed[LoopC].Equipped = Convert.ToByte(General.ReadField(3, ref ln, 45));
                }

                // Obtiene el indice-objeto del arma
                withBlock.Invent.WeaponEqpSlot = Convert.ToByte(UserFile.GetValue("Inventory", "WeaponEqpSlot"));
                if (withBlock.Invent.WeaponEqpSlot > 0)
                {
                    withBlock.Invent.WeaponEqpObjIndex = withBlock.Invent.Object_Renamed[withBlock.Invent.WeaponEqpSlot].ObjIndex;
                }

                // Obtiene el indice-objeto del armadura
                withBlock.Invent.ArmourEqpSlot = Convert.ToByte(UserFile.GetValue("Inventory", "ArmourEqpSlot"));
                if (withBlock.Invent.ArmourEqpSlot > 0)
                {
                    withBlock.Invent.ArmourEqpObjIndex = withBlock.Invent.Object_Renamed[withBlock.Invent.ArmourEqpSlot].ObjIndex;
                    withBlock.flags.Desnudo = 0;
                }
                else
                {
                    withBlock.flags.Desnudo = 1;
                }

                // Obtiene el indice-objeto del escudo
                withBlock.Invent.EscudoEqpSlot = Convert.ToByte(UserFile.GetValue("Inventory", "EscudoEqpSlot"));
                if (withBlock.Invent.EscudoEqpSlot > 0)
                {
                    withBlock.Invent.EscudoEqpObjIndex = withBlock.Invent.Object_Renamed[withBlock.Invent.EscudoEqpSlot].ObjIndex;
                }

                // Obtiene el indice-objeto del casco
                withBlock.Invent.CascoEqpSlot = Convert.ToByte(UserFile.GetValue("Inventory", "CascoEqpSlot"));
                if (withBlock.Invent.CascoEqpSlot > 0)
                {
                    withBlock.Invent.CascoEqpObjIndex = withBlock.Invent.Object_Renamed[withBlock.Invent.CascoEqpSlot].ObjIndex;
                }

                // Obtiene el indice-objeto barco
                withBlock.Invent.BarcoSlot = Convert.ToByte(UserFile.GetValue("Inventory", "BarcoSlot"));
                if (withBlock.Invent.BarcoSlot > 0)
                {
                    withBlock.Invent.BarcoObjIndex = withBlock.Invent.Object_Renamed[withBlock.Invent.BarcoSlot].ObjIndex;
                }

                // Obtiene el indice-objeto municion
                withBlock.Invent.MunicionEqpSlot = Convert.ToByte(UserFile.GetValue("Inventory", "MunicionSlot"));
                if (withBlock.Invent.MunicionEqpSlot > 0)
                {
                    withBlock.Invent.MunicionEqpObjIndex = withBlock.Invent.Object_Renamed[withBlock.Invent.MunicionEqpSlot].ObjIndex;
                }

                // [Alejo]
                // Obtiene el indice-objeto anilo
                withBlock.Invent.AnilloEqpSlot = Convert.ToByte(UserFile.GetValue("Inventory", "AnilloSlot"));
                if (withBlock.Invent.AnilloEqpSlot > 0)
                {
                    withBlock.Invent.AnilloEqpObjIndex = withBlock.Invent.Object_Renamed[withBlock.Invent.AnilloEqpSlot].ObjIndex;
                }

                withBlock.Invent.MochilaEqpSlot = Convert.ToByte(UserFile.GetValue("Inventory", "MochilaSlot"));
                if (withBlock.Invent.MochilaEqpSlot > 0)
                {
                    withBlock.Invent.MochilaEqpObjIndex = withBlock.Invent.Object_Renamed[withBlock.Invent.MochilaEqpSlot].ObjIndex;
                }

                withBlock.NroMascotas = Convert.ToInt16(UserFile.GetValue("MASCOTAS", "NroMascotas"));
                for (LoopC = 1; LoopC <= Declaraciones.MAXMASCOTAS; LoopC++)
                    withBlock.MascotasType[LoopC] = Convert.ToInt16(Migration.ParseVal(UserFile.GetValue("MASCOTAS", "MAS" + LoopC)));

                ln = UserFile.GetValue("Guild", "GUILDINDEX");
                if (Information.IsNumeric(ln))
                {
                    withBlock.GuildIndex = Convert.ToInt16(ln);
                }
                else
                {
                    withBlock.GuildIndex = 0;
                }
            }
        }

        // TODO MIGRA: funciona pero es lento e ineficiente
        public static string GetVar(string filePath, string sectionName, string keyName, [Optional, DefaultParameterValue(1024)] ref int EmptySpaces)
        {
            string GetVarRet = default;
            string currentLine;
            string currentSection;
            int equalPos;

            GetVarRet = ""; // Default return if not found

            // Check if file exists
            if (!File.Exists(filePath))
            {
                return "";
            }

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    currentSection = "";
                    while (!reader.EndOfStream)
                    {
                        currentLine = reader.ReadLine();
                        currentLine = currentLine is not null ? currentLine.Trim() : "";

                        // Check if it's a section line, e.g. [SECTION]
                        if ((currentLine.Length > 0 && currentLine.Substring(0, 1) == "[") & currentLine.Length > 0 && currentLine.Substring(currentLine.Length - 1, 1) == "]")
                        {
                            currentSection = currentLine.Length > 2 ? currentLine.Substring(1, currentLine.Length - 2) : "";
                        }
                        else if (Strings.StrComp(currentSection, sectionName, CompareMethod.Text) == 0)
                        {
                            // We are in the correct section, check if line contains the key
                            equalPos = currentLine is not null ? currentLine.IndexOf("=") + 1 : 0;
                            if (equalPos > 1)
                            {
                                // Extract the key (left side of '=') and compare
                                if (Strings.StrComp(currentLine is not null ? currentLine.Substring(0, equalPos - 1).Trim() : "", keyName, CompareMethod.Text) == 0)
                                {
                                    // Return the value (right side of '='), trimmed
                                    return currentLine is not null ? currentLine.Substring(equalPos).Trim() : "";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Podés loguear el error si querés
            }

            return GetVarRet;
        }


        public static void CargarBackUp()
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            var Map = default(short);
            short TempInt;
            string tFileName;
            string npcfile;

            try
            {

                int argEmptySpaces = 1024;
                Declaraciones.NumMaps = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "Map.dat", "INIT", "NumMaps", EmptySpaces: ref argEmptySpaces)));
                ModAreas.InitAreas();

                int argEmptySpaces1 = 1024;
                Declaraciones.MapPath = GetVar(Declaraciones.DatPath + "Map.dat", "INIT", "MapPath", EmptySpaces: ref argEmptySpaces1);

                // UPGRADE_WARNING: Es posible que la matriz MapData necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
                // UPGRADE_WARNING: El límite inferior de la matriz MapData ha cambiado de 1,XMinMapSize,YMinMapSize a 0,0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                Declaraciones.MapData = new Declaraciones.MapBlock[(Declaraciones.NumMaps + 1), (Declaraciones.XMaxMapSize + 1), (Declaraciones.YMaxMapSize + 1)];
                ArrayInitializers.InitializeStruct(Declaraciones.MapData);
                // UPGRADE_WARNING: El límite inferior de la matriz MapInfo_Renamed ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                // UPGRADE_NOTE: MapInfo se actualizó a MapInfo_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
                Declaraciones.MapInfo_Renamed = new Declaraciones.MapInfo[(Declaraciones.NumMaps + 1)];

                var loopTo = Declaraciones.NumMaps;
                for (Map = 1; Map <= loopTo; Map++)
                {
                    int argEmptySpaces2 = 1024;
                    if (Migration.ParseVal(GetVar(AppDomain.CurrentDomain.BaseDirectory + Declaraciones.MapPath + "Mapa" + Map + ".Dat", "Mapa" + Map, "BackUp", EmptySpaces: ref argEmptySpaces2)) != 0d)
                    {
                        tFileName = AppDomain.CurrentDomain.BaseDirectory + "WorldBackUp/Mapa" + Map;

                        if (!File.Exists(tFileName + ".*"))
                        {
                            // Miramos que exista al menos uno de los 3 archivos, sino lo cargamos de la carpeta de los mapas
                            tFileName = AppDomain.CurrentDomain.BaseDirectory + Declaraciones.MapPath + "Mapa" + Map;
                        }
                    }
                    else
                    {
                        tFileName = AppDomain.CurrentDomain.BaseDirectory + Declaraciones.MapPath + "Mapa" + Map;
                    }

                    CargaMapa.CargarMapa(Map, tFileName);
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in CargarBackUp: " + ex.Message);
                throw new Exception("Error durante la carga de mapas, el mapa " + Map + " contiene errores: " + ex.Message, ex);
            }
        }

        public static void LoadMapData()
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            var Map = default(short);
            string tFileName;

            try
            {

                int argEmptySpaces = 1024;
                Declaraciones.NumMaps = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "Map.dat", "INIT", "NumMaps", EmptySpaces: ref argEmptySpaces)));
                ModAreas.InitAreas();

                int argEmptySpaces1 = 1024;
                Declaraciones.MapPath = GetVar(Declaraciones.DatPath + "Map.dat", "INIT", "MapPath", EmptySpaces: ref argEmptySpaces1);

                // UPGRADE_WARNING: Es posible que la matriz MapData necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
                // UPGRADE_WARNING: El límite inferior de la matriz MapData ha cambiado de 1,XMinMapSize,YMinMapSize a 0,0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                Declaraciones.MapData = new Declaraciones.MapBlock[(Declaraciones.NumMaps + 1), (Declaraciones.XMaxMapSize + 1), (Declaraciones.YMaxMapSize + 1)];
                ArrayInitializers.InitializeStruct(Declaraciones.MapData);
                // UPGRADE_WARNING: El límite inferior de la matriz MapInfo_Renamed ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                // UPGRADE_NOTE: MapInfo se actualizó a MapInfo_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
                Declaraciones.MapInfo_Renamed = new Declaraciones.MapInfo[(Declaraciones.NumMaps + 1)];

                var loopTo = Declaraciones.NumMaps;
                for (Map = 1; Map <= loopTo; Map++)
                {
                    tFileName = AppDomain.CurrentDomain.BaseDirectory + Declaraciones.MapPath + "Mapa" + Map;
                    CargaMapa.CargarMapa(Map, tFileName);
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in LoadMapData: " + ex.Message);
                throw new Exception("Error durante la carga de mapas, el mapa " + Map + " contiene errores: " + ex.Message, ex);
            }
        }

        public static void LoadSini()
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            int Temporal;

            int argEmptySpaces = 1024;
            Admin.BootDelBackUp = Convert.ToByte(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "IniciarDesdeBackUp", EmptySpaces: ref argEmptySpaces)));

            int argEmptySpaces1 = 1024;
            Admin.Puerto = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "StartPort", EmptySpaces: ref argEmptySpaces1)));
            int argEmptySpaces2 = 1024;
            Declaraciones.HideMe = Convert.ToByte(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "Hide", EmptySpaces: ref argEmptySpaces2)));
            int argEmptySpaces3 = 1024;
            Declaraciones.AllowMultiLogins = Convert.ToByte(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "AllowMultiLogins", EmptySpaces: ref argEmptySpaces3)));
            int argEmptySpaces4 = 1024;
            Declaraciones.IdleLimit = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "IdleLimit", EmptySpaces: ref argEmptySpaces4)));
            // Lee la version correcta del cliente
            int argEmptySpaces5 = 1024;
            Declaraciones.ULTIMAVERSION = GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "Version", EmptySpaces: ref argEmptySpaces5);

            int argEmptySpaces6 = 1024;
            Declaraciones.PuedeCrearPersonajes = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "PuedeCrearPersonajes", EmptySpaces: ref argEmptySpaces6)));
            int argEmptySpaces7 = 1024;
            Declaraciones.ServerSoloGMs = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "init", "ServerSoloGMs", EmptySpaces: ref argEmptySpaces7)));

            int argEmptySpaces8 = 1024;
            ModFacciones.ArmaduraImperial1 = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "ArmaduraImperial1", EmptySpaces: ref argEmptySpaces8)));
            int argEmptySpaces9 = 1024;
            ModFacciones.ArmaduraImperial2 = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "ArmaduraImperial2", EmptySpaces: ref argEmptySpaces9)));
            int argEmptySpaces10 = 1024;
            ModFacciones.ArmaduraImperial3 = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "ArmaduraImperial3", EmptySpaces: ref argEmptySpaces10)));
            int argEmptySpaces11 = 1024;
            ModFacciones.TunicaMagoImperial = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "TunicaMagoImperial", EmptySpaces: ref argEmptySpaces11)));
            int argEmptySpaces12 = 1024;
            ModFacciones.TunicaMagoImperialEnanos = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "TunicaMagoImperialEnanos", EmptySpaces: ref argEmptySpaces12)));
            int argEmptySpaces13 = 1024;
            ModFacciones.ArmaduraCaos1 = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "ArmaduraCaos1", EmptySpaces: ref argEmptySpaces13)));
            int argEmptySpaces14 = 1024;
            ModFacciones.ArmaduraCaos2 = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "ArmaduraCaos2", EmptySpaces: ref argEmptySpaces14)));
            int argEmptySpaces15 = 1024;
            ModFacciones.ArmaduraCaos3 = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "ArmaduraCaos3", EmptySpaces: ref argEmptySpaces15)));
            int argEmptySpaces16 = 1024;
            ModFacciones.TunicaMagoCaos = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "TunicaMagoCaos", EmptySpaces: ref argEmptySpaces16)));
            int argEmptySpaces17 = 1024;
            ModFacciones.TunicaMagoCaosEnanos = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "TunicaMagoCaosEnanos", EmptySpaces: ref argEmptySpaces17)));

            int argEmptySpaces18 = 1024;
            ModFacciones.VestimentaImperialHumano = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "VestimentaImperialHumano", EmptySpaces: ref argEmptySpaces18)));
            int argEmptySpaces19 = 1024;
            ModFacciones.VestimentaImperialEnano = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "VestimentaImperialEnano", EmptySpaces: ref argEmptySpaces19)));
            int argEmptySpaces20 = 1024;
            ModFacciones.TunicaConspicuaHumano = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "TunicaConspicuaHumano", EmptySpaces: ref argEmptySpaces20)));
            int argEmptySpaces21 = 1024;
            ModFacciones.TunicaConspicuaEnano = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "TunicaConspicuaEnano", EmptySpaces: ref argEmptySpaces21)));
            int argEmptySpaces22 = 1024;
            ModFacciones.ArmaduraNobilisimaHumano = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "ArmaduraNobilisimaHumano", EmptySpaces: ref argEmptySpaces22)));
            int argEmptySpaces23 = 1024;
            ModFacciones.ArmaduraNobilisimaEnano = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "ArmaduraNobilisimaEnano", EmptySpaces: ref argEmptySpaces23)));
            int argEmptySpaces24 = 1024;
            ModFacciones.ArmaduraGranSacerdote = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "ArmaduraGranSacerdote", EmptySpaces: ref argEmptySpaces24)));

            int argEmptySpaces25 = 1024;
            ModFacciones.VestimentaLegionHumano = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "VestimentaLegionHumano", EmptySpaces: ref argEmptySpaces25)));
            int argEmptySpaces26 = 1024;
            ModFacciones.VestimentaLegionEnano = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "VestimentaLegionEnano", EmptySpaces: ref argEmptySpaces26)));
            int argEmptySpaces27 = 1024;
            ModFacciones.TunicaLobregaHumano = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "TunicaLobregaHumano", EmptySpaces: ref argEmptySpaces27)));
            int argEmptySpaces28 = 1024;
            ModFacciones.TunicaLobregaEnano = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "TunicaLobregaEnano", EmptySpaces: ref argEmptySpaces28)));
            int argEmptySpaces29 = 1024;
            ModFacciones.TunicaEgregiaHumano = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "TunicaEgregiaHumano", EmptySpaces: ref argEmptySpaces29)));
            int argEmptySpaces30 = 1024;
            ModFacciones.TunicaEgregiaEnano = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "TunicaEgregiaEnano", EmptySpaces: ref argEmptySpaces30)));
            int argEmptySpaces31 = 1024;
            ModFacciones.SacerdoteDemoniaco = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "SacerdoteDemoniaco", EmptySpaces: ref argEmptySpaces31)));

            int argEmptySpaces32 = 1024;
            PraetoriansCoopNPC.MAPA_PRETORIANO = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "MapaPretoriano", EmptySpaces: ref argEmptySpaces32)));

            int argEmptySpaces33 = 1024;
            Declaraciones.EnTesting = Convert.ToBoolean(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "Testing", EmptySpaces: ref argEmptySpaces33)));

            // Intervalos
            int argEmptySpaces34 = 1024;
            Admin.SanaIntervaloSinDescansar = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "SanaIntervaloSinDescansar", EmptySpaces: ref argEmptySpaces34)));
            int argEmptySpaces35 = 1024;
            Admin.StaminaIntervaloSinDescansar = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "StaminaIntervaloSinDescansar", EmptySpaces: ref argEmptySpaces35)));
            int argEmptySpaces36 = 1024;
            Admin.SanaIntervaloDescansar = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "SanaIntervaloDescansar", EmptySpaces: ref argEmptySpaces36)));
            int argEmptySpaces37 = 1024;
            Admin.StaminaIntervaloDescansar = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "StaminaIntervaloDescansar", EmptySpaces: ref argEmptySpaces37)));

            int argEmptySpaces38 = 1024;
            Admin.IntervaloSed = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloSed", EmptySpaces: ref argEmptySpaces38)));
            int argEmptySpaces39 = 1024;
            Admin.IntervaloHambre = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloHambre", EmptySpaces: ref argEmptySpaces39)));
            int argEmptySpaces40 = 1024;
            Admin.IntervaloVeneno = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloVeneno", EmptySpaces: ref argEmptySpaces40)));
            int argEmptySpaces41 = 1024;
            Admin.IntervaloParalizado = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloParalizado", EmptySpaces: ref argEmptySpaces41)));
            int argEmptySpaces42 = 1024;
            Admin.IntervaloInvisible = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloInvisible", EmptySpaces: ref argEmptySpaces42)));
            int argEmptySpaces43 = 1024;
            Admin.IntervaloFrio = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloFrio", EmptySpaces: ref argEmptySpaces43)));
            int argEmptySpaces44 = 1024;
            Admin.IntervaloWavFx = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloWAVFX", EmptySpaces: ref argEmptySpaces44)));
            int argEmptySpaces45 = 1024;
            Admin.IntervaloInvocacion = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloInvocacion", EmptySpaces: ref argEmptySpaces45)));
            int argEmptySpaces46 = 1024;
            Admin.IntervaloParaConexion = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloParaConexion", EmptySpaces: ref argEmptySpaces46)));

            // &&&&&&&&&&&&&&&&&&&&& TIMERS &&&&&&&&&&&&&&&&&&&&&&&

            Admin.IntervaloPuedeSerAtacado = 5000; // Cargar desde balance.dat
            Admin.IntervaloAtacable = 60000; // Cargar desde balance.dat
            Admin.IntervaloOwnedNpc = 18000; // Cargar desde balance.dat

            int argEmptySpaces47 = 1024;
            Admin.IntervaloUserPuedeCastear = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloLanzaHechizo", EmptySpaces: ref argEmptySpaces47)));

            // UPGRADE_WARNING: La propiedad Timer TIMER_AI.Interval no puede tener un valor de 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="169ECF4A-1968-402D-B243-16603CC08604"'
            int argEmptySpaces48 = 1024;
            GameLoop.timerAIInterval = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloNpcAI", EmptySpaces: ref argEmptySpaces48)));

            // UPGRADE_WARNING: La propiedad Timer npcataca.Interval no puede tener un valor de 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="169ECF4A-1968-402D-B243-16603CC08604"'
            int argEmptySpaces49 = 1024;
            GameLoop.npcAtacaInterval = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloNpcPuedeAtacar", EmptySpaces: ref argEmptySpaces49)));

            int argEmptySpaces50 = 1024;
            Admin.IntervaloUserPuedeTrabajar = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloTrabajo", EmptySpaces: ref argEmptySpaces50)));

            int argEmptySpaces51 = 1024;
            Admin.IntervaloUserPuedeAtacar = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloUserPuedeAtacar", EmptySpaces: ref argEmptySpaces51)));

            // TODO : Agregar estos intervalos al form!!!
            int argEmptySpaces52 = 1024;
            Admin.IntervaloMagiaGolpe = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloMagiaGolpe", EmptySpaces: ref argEmptySpaces52)));
            int argEmptySpaces53 = 1024;
            Admin.IntervaloGolpeMagia = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloGolpeMagia", EmptySpaces: ref argEmptySpaces53)));
            int argEmptySpaces54 = 1024;
            Admin.IntervaloGolpeUsar = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloGolpeUsar", EmptySpaces: ref argEmptySpaces54)));

            // UPGRADE_WARNING: La propiedad Timer tLluvia.Interval no puede tener un valor de 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="169ECF4A-1968-402D-B243-16603CC08604"'
            int argEmptySpaces55 = 1024;
            GameLoop.lluviaInterval = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloPerdidaStaminaLluvia", EmptySpaces: ref argEmptySpaces55)));

            int argEmptySpaces56 = 1024;
            Admin.MinutosWs = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloWS", EmptySpaces: ref argEmptySpaces56)));
            if (Admin.MinutosWs < 60)
                Admin.MinutosWs = 180;

            int argEmptySpaces57 = 1024;
            Admin.IntervaloCerrarConexion = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloCerrarConexion", EmptySpaces: ref argEmptySpaces57)));
            int argEmptySpaces58 = 1024;
            Admin.IntervaloUserPuedeUsar = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloUserPuedeUsar", EmptySpaces: ref argEmptySpaces58)));
            int argEmptySpaces59 = 1024;
            Admin.IntervaloFlechasCazadores = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloFlechasCazadores", EmptySpaces: ref argEmptySpaces59)));

            int argEmptySpaces60 = 1024;
            Admin.IntervaloOculto = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INTERVALOS", "IntervaloOculto", EmptySpaces: ref argEmptySpaces60)));

            // &&&&&&&&&&&&&&&&&&&&& FIN TIMERS &&&&&&&&&&&&&&&&&&&&&&&

            int argEmptySpaces61 = 1024;
            Declaraciones.recordusuarios = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "Record", EmptySpaces: ref argEmptySpaces61)));

            // Max users
            int argEmptySpaces62 = 1024;
            Temporal = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.IniPath + "Server.ini", "INIT", "MaxUsers", EmptySpaces: ref argEmptySpaces62)));
            if (Declaraciones.MaxUsers == 0)
            {
                Declaraciones.MaxUsers = Convert.ToInt16(Temporal);
                // UPGRADE_WARNING: Es posible que la matriz UserList necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
                // UPGRADE_WARNING: El límite inferior de la matriz UserList ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
                Declaraciones.UserList = new Declaraciones.User[(Declaraciones.MaxUsers + 1)];
                ArrayInitializers.InitializeStruct(Declaraciones.UserList);
            }

            // &&&&&&&&&&&&&&&&&&&&& BALANCE &&&&&&&&&&&&&&&&&&&&&&&
            // Se agregó en LoadBalance y en el Balance.dat
            // PorcentajeRecuperoMana = val(GetVar(IniPath & "Server.ini", "BALANCE", "PorcentajeRecuperoMana"))

            // '&&&&&&&&&&&&&&&&&&&&& FIN BALANCE &&&&&&&&&&&&&&&&&&&&&&&
            Statistics.Initialize();

            int argEmptySpaces63 = 1024;
            Declaraciones.Ullathorpe.Map = Convert.ToInt16(GetVar(Declaraciones.DatPath + "Ciudades.dat", "Ullathorpe", "Mapa", EmptySpaces: ref argEmptySpaces63));
            int argEmptySpaces64 = 1024;
            Declaraciones.Ullathorpe.X = Convert.ToInt16(GetVar(Declaraciones.DatPath + "Ciudades.dat", "Ullathorpe", "X", EmptySpaces: ref argEmptySpaces64));
            int argEmptySpaces65 = 1024;
            Declaraciones.Ullathorpe.Y = Convert.ToInt16(GetVar(Declaraciones.DatPath + "Ciudades.dat", "Ullathorpe", "Y", EmptySpaces: ref argEmptySpaces65));

            int argEmptySpaces66 = 1024;
            Declaraciones.Nix.Map = Convert.ToInt16(GetVar(Declaraciones.DatPath + "Ciudades.dat", "Nix", "Mapa", EmptySpaces: ref argEmptySpaces66));
            int argEmptySpaces67 = 1024;
            Declaraciones.Nix.X = Convert.ToInt16(GetVar(Declaraciones.DatPath + "Ciudades.dat", "Nix", "X", EmptySpaces: ref argEmptySpaces67));
            int argEmptySpaces68 = 1024;
            Declaraciones.Nix.Y = Convert.ToInt16(GetVar(Declaraciones.DatPath + "Ciudades.dat", "Nix", "Y", EmptySpaces: ref argEmptySpaces68));

            int argEmptySpaces69 = 1024;
            Declaraciones.Banderbill.Map = Convert.ToInt16(GetVar(Declaraciones.DatPath + "Ciudades.dat", "Banderbill", "Mapa", EmptySpaces: ref argEmptySpaces69));
            int argEmptySpaces70 = 1024;
            Declaraciones.Banderbill.X = Convert.ToInt16(GetVar(Declaraciones.DatPath + "Ciudades.dat", "Banderbill", "X", EmptySpaces: ref argEmptySpaces70));
            int argEmptySpaces71 = 1024;
            Declaraciones.Banderbill.Y = Convert.ToInt16(GetVar(Declaraciones.DatPath + "Ciudades.dat", "Banderbill", "Y", EmptySpaces: ref argEmptySpaces71));

            int argEmptySpaces72 = 1024;
            Declaraciones.Lindos.Map = Convert.ToInt16(GetVar(Declaraciones.DatPath + "Ciudades.dat", "Lindos", "Mapa", EmptySpaces: ref argEmptySpaces72));
            int argEmptySpaces73 = 1024;
            Declaraciones.Lindos.X = Convert.ToInt16(GetVar(Declaraciones.DatPath + "Ciudades.dat", "Lindos", "X", EmptySpaces: ref argEmptySpaces73));
            int argEmptySpaces74 = 1024;
            Declaraciones.Lindos.Y = Convert.ToInt16(GetVar(Declaraciones.DatPath + "Ciudades.dat", "Lindos", "Y", EmptySpaces: ref argEmptySpaces74));

            int argEmptySpaces75 = 1024;
            Declaraciones.Arghal.Map = Convert.ToInt16(GetVar(Declaraciones.DatPath + "Ciudades.dat", "Arghal", "Mapa", EmptySpaces: ref argEmptySpaces75));
            int argEmptySpaces76 = 1024;
            Declaraciones.Arghal.X = Convert.ToInt16(GetVar(Declaraciones.DatPath + "Ciudades.dat", "Arghal", "X", EmptySpaces: ref argEmptySpaces76));
            int argEmptySpaces77 = 1024;
            Declaraciones.Arghal.Y = Convert.ToInt16(GetVar(Declaraciones.DatPath + "Ciudades.dat", "Arghal", "Y", EmptySpaces: ref argEmptySpaces77));

            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Ciudades(eCiudad.cUllathorpe). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Declaraciones.Ciudades[(int)Declaraciones.eCiudad.cUllathorpe] = Declaraciones.Ullathorpe;
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Ciudades(eCiudad.cNix). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Declaraciones.Ciudades[(int)Declaraciones.eCiudad.cNix] = Declaraciones.Nix;
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Ciudades(eCiudad.cBanderbill). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Declaraciones.Ciudades[(int)Declaraciones.eCiudad.cBanderbill] = Declaraciones.Banderbill;
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Ciudades(eCiudad.cLindos). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Declaraciones.Ciudades[(int)Declaraciones.eCiudad.cLindos] = Declaraciones.Lindos;
            // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Ciudades(eCiudad.cArghal). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            Declaraciones.Ciudades[(int)Declaraciones.eCiudad.cArghal] = Declaraciones.Arghal;

            Admin.MD5sCarga();

            Declaraciones.ConsultaPopular.LoadData();
        }

        // TODO MIGRA: funciona pero es lento e ineficiente
        public static void WriteVar(string fileName, string Main, string Var, string Value)
        {
            // Reemplazamos Collection con estructuras modernas
            var sectionNames = new List<string>(); // Para guardar el orden de aparición de secciones
            var sectionData = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            // Secciones y sus claves/valores

            string currentSection;
            string line;
            string fileContent;
            string[] rawLines;

            // --- Leer archivo si existe ---
            if (File.Exists(fileName))
            {
                fileContent = File.ReadAllText(fileName);
            }
            else
            {
                fileContent = "";
            } // Archivo no existe, se creará

            // --- Separar en líneas ---
            if (!string.IsNullOrEmpty(fileContent))
            {
                rawLines = fileContent.Split(new string[] { Constants.vbCrLf }, StringSplitOptions.None);
            }
            else
            {
                rawLines = new string[1];
                rawLines[0] = "";
            }

            // --- Inicializar sección actual como vacío ---
            currentSection = "";

            // -----------------------------------------------------------------------------
            // 1) Parsear todo el archivo en memoria: secciones y claves
            // - Al encontrar una nueva sección, si ya existe en 'sectionNames', seguimos usando la misma
            // para no duplicarla, de lo contrario se crea.
            // - Al encontrar "Clave=Valor", se guarda/actualiza en el diccionario de la sección actual.
            // - Se ignoran líneas vacías.
            // -----------------------------------------------------------------------------
            int posEqual;
            string keyName;
            string keyValue;

            for (int i = 0, loopTo = rawLines.Length - 1; i <= loopTo; i++)
            {
                line = rawLines[i].Trim();
                if (!string.IsNullOrEmpty(line))
                {
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        // Sección
                        currentSection = line.Substring(1, line.Length - 2);

                        // Verificar si la sección ya existe
                        if (!sectionNames.Contains(currentSection, StringComparer.OrdinalIgnoreCase))
                        {
                            // Crear la sección y su diccionario
                            sectionNames.Add(currentSection); // se guarda en el orden en que aparece
                            sectionData.Add(currentSection, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                        }
                    }

                    else if (!string.IsNullOrEmpty(currentSection))
                    {
                        // Estamos dentro de una sección, parsear "clave=valor"
                        posEqual = line.IndexOf("=");
                        if (posEqual > 0)
                        {
                            keyName = line.Substring(0, posEqual).Trim();
                            keyValue = line.Substring(posEqual + 1);

                            // Actualizar o agregar la clave
                            if (sectionData[currentSection].ContainsKey(keyName))
                            {
                                sectionData[currentSection][keyName] = keyValue;
                            }
                            else
                            {
                                sectionData[currentSection].Add(keyName, keyValue);
                            }
                        }
                    }
                }
            }

            // -----------------------------------------------------------------------------
            // 2) Agregar/actualizar la clave solicitada en la sección "Main"
            // - Si la sección no existe, se crea.
            // - Se actualiza (o se crea) la Key=Value especificada.
            // -----------------------------------------------------------------------------
            // Verificar si la sección principal existe
            if (!sectionData.ContainsKey(Main))
            {
                sectionNames.Add(Main);
                sectionData.Add(Main, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
            }

            // Actualizar o crear la clave Var=Value dentro de la sección Main
            if (sectionData[Main].ContainsKey(Var))
            {
                sectionData[Main][Var] = Value;
            }
            else
            {
                sectionData[Main].Add(Var, Value);
            }

            // -----------------------------------------------------------------------------
            // 3) Reconstruir el archivo .ini: no debe haber secciones duplicadas, sin líneas en blanco
            // -----------------------------------------------------------------------------
            var outputLines = new List<string>();

            // Procesar las secciones en el orden en que se encontraron
            foreach (string secName in sectionNames)
            {
                // Escribir la sección
                outputLines.Add("[" + secName + "]");

                // Escribir las claves/valores
                foreach (KeyValuePair<string, string> kvp in sectionData[secName])
                    outputLines.Add(kvp.Key + "=" + kvp.Value);
            }

            // Si no hay contenido, crear la sección y clave por defecto
            if (outputLines.Count == 0)
            {
                outputLines.Add("[" + Main + "]");
                outputLines.Add(Var + "=" + Value);
            }

            // -----------------------------------------------------------------------------
            // 4) Escribir el archivo resultante
            // -----------------------------------------------------------------------------
            fileContent = string.Join(Constants.vbCrLf, outputLines.ToArray());
            File.WriteAllText(fileName, fileContent);
        }

        public static void SaveUser(short UserIndex, string UserFile)
        {
            // *************************************************
            // Author: Unknown
            // Last modified: 12/01/2010 (ZaMa)
            // Saves the Users records
            // 23/01/2007 Pablo (ToxicWaste) - Agrego NivelIngreso, FechaIngreso, MatadosIngreso y NextRecompensa.
            // 11/19/2009: Pato - Save the EluSkills and ExpSkills
            // 12/01/2010: ZaMa - Los druidas pierden la inmunidad de ser atacados cuando pierden el efecto del mimetismo.
            // *************************************************

            try
            {

                int OldUserHead;

                int LoopC;
                DateTime TempDate;
                int i;
                int loopd;
                int L;
                string cad;
                int NroMascotas;
                {
                    ref var withBlock = ref Declaraciones.UserList[UserIndex];

                    // ESTO TIENE QUE EVITAR ESE BUGAZO QUE NO SE POR QUE GRABA USUARIOS NULOS
                    // clase=0 es el error, porq el enum empieza de 1!!
                    if (withBlock.clase == 0 | withBlock.Stats.ELV == 0)
                    {
                        string argdesc = "Estoy intentantdo guardar un usuario nulo de nombre: " + withBlock.name;
                        General.LogCriticEvent(ref argdesc);
                        return;
                    }


                    if (withBlock.flags.Mimetizado == 1)
                    {
                        withBlock.Char_Renamed.body = withBlock.CharMimetizado.body;
                        withBlock.Char_Renamed.Head = withBlock.CharMimetizado.Head;
                        withBlock.Char_Renamed.CascoAnim = withBlock.CharMimetizado.CascoAnim;
                        withBlock.Char_Renamed.ShieldAnim = withBlock.CharMimetizado.ShieldAnim;
                        withBlock.Char_Renamed.WeaponAnim = withBlock.CharMimetizado.WeaponAnim;
                        withBlock.Counters.Mimetismo = 0;
                        withBlock.flags.Mimetizado = 0;
                        // Se fue el efecto del mimetismo, puede ser atacado por npcs
                        withBlock.flags.Ignorado = false;
                    }

                    if (File.Exists(UserFile))
                    {
                        if (withBlock.flags.Muerto == 1)
                        {
                            OldUserHead = withBlock.Char_Renamed.Head;
                            int argEmptySpaces = 1024;
                            withBlock.Char_Renamed.Head = Convert.ToInt16(GetVar(UserFile, "INIT", "Head", EmptySpaces: ref argEmptySpaces));
                        }
                        // Kill UserFile
                    }


                    WriteVar(UserFile, "FLAGS", "Muerto", withBlock.flags.Muerto.ToString());
                    WriteVar(UserFile, "FLAGS", "Escondido", withBlock.flags.Escondido.ToString());
                    WriteVar(UserFile, "FLAGS", "Hambre", withBlock.flags.Hambre.ToString());
                    WriteVar(UserFile, "FLAGS", "Sed", withBlock.flags.Sed.ToString());
                    WriteVar(UserFile, "FLAGS", "Desnudo", withBlock.flags.Desnudo.ToString());
                    WriteVar(UserFile, "FLAGS", "Ban", withBlock.flags.Ban.ToString());
                    WriteVar(UserFile, "FLAGS", "Navegando", withBlock.flags.Navegando.ToString());
                    WriteVar(UserFile, "FLAGS", "Envenenado", withBlock.flags.Envenenado.ToString());
                    WriteVar(UserFile, "FLAGS", "Paralizado", withBlock.flags.Paralizado.ToString());
                    // Matrix
                    WriteVar(UserFile, "FLAGS", "LastMap", withBlock.flags.lastMap.ToString());

                    WriteVar(UserFile, "CONSEJO", "PERTENECE", (withBlock.flags.Privilegios & Declaraciones.PlayerType.RoyalCouncil) != 0 ? "1" : "0");
                    WriteVar(UserFile, "CONSEJO", "PERTENECECAOS", (withBlock.flags.Privilegios & Declaraciones.PlayerType.ChaosCouncil) != 0 ? "1" : "0");


                    WriteVar(UserFile, "COUNTERS", "Pena", withBlock.Counters.Pena.ToString());
                    WriteVar(UserFile, "COUNTERS", "SkillsAsignados", withBlock.Counters.AsignedSkills.ToString());

                    WriteVar(UserFile, "FACCIONES", "EjercitoReal", withBlock.Faccion.ArmadaReal.ToString());
                    WriteVar(UserFile, "FACCIONES", "EjercitoCaos", withBlock.Faccion.FuerzasCaos.ToString());
                    WriteVar(UserFile, "FACCIONES", "CiudMatados", withBlock.Faccion.CiudadanosMatados.ToString());
                    WriteVar(UserFile, "FACCIONES", "CrimMatados", withBlock.Faccion.CriminalesMatados.ToString());
                    WriteVar(UserFile, "FACCIONES", "rArCaos", withBlock.Faccion.RecibioArmaduraCaos.ToString());
                    WriteVar(UserFile, "FACCIONES", "rArReal", withBlock.Faccion.RecibioArmaduraReal.ToString());
                    WriteVar(UserFile, "FACCIONES", "rExCaos", withBlock.Faccion.RecibioExpInicialCaos.ToString());
                    WriteVar(UserFile, "FACCIONES", "rExReal", withBlock.Faccion.RecibioExpInicialReal.ToString());
                    WriteVar(UserFile, "FACCIONES", "recCaos", withBlock.Faccion.RecompensasCaos.ToString());
                    WriteVar(UserFile, "FACCIONES", "recReal", withBlock.Faccion.RecompensasReal.ToString());
                    WriteVar(UserFile, "FACCIONES", "Reenlistadas", withBlock.Faccion.Reenlistadas.ToString());
                    WriteVar(UserFile, "FACCIONES", "NivelIngreso", withBlock.Faccion.NivelIngreso.ToString());
                    WriteVar(UserFile, "FACCIONES", "FechaIngreso", withBlock.Faccion.FechaIngreso);
                    WriteVar(UserFile, "FACCIONES", "MatadosIngreso", withBlock.Faccion.MatadosIngreso.ToString());
                    WriteVar(UserFile, "FACCIONES", "NextRecompensa", withBlock.Faccion.NextRecompensa.ToString());


                    // ¿Fueron modificados los atributos del usuario?
                    if (!withBlock.flags.TomoPocion)
                    {
                        var loopTo = withBlock.Stats.UserAtributos.Length - 1;
                        for (LoopC = 1; LoopC <= loopTo; LoopC++)
                            WriteVar(UserFile, "ATRIBUTOS", "AT" + LoopC, withBlock.Stats.UserAtributos[LoopC].ToString());
                    }
                    else
                    {
                        var loopTo1 = withBlock.Stats.UserAtributos.Length - 1;
                        for (LoopC = 1; LoopC <= loopTo1; LoopC++)
                            // .Stats.UserAtributos(LoopC) = .Stats.UserAtributosBackUP(LoopC)
                            WriteVar(UserFile, "ATRIBUTOS", "AT" + LoopC, withBlock.Stats.UserAtributosBackUP[LoopC].ToString());
                    }

                    var loopTo2 = withBlock.Stats.UserSkills.Length - 1;
                    for (LoopC = 1; LoopC <= loopTo2; LoopC++)
                    {
                        WriteVar(UserFile, "SKILLS", "SK" + LoopC, withBlock.Stats.UserSkills[LoopC].ToString());
                        WriteVar(UserFile, "SKILLS", "ELUSK" + LoopC, withBlock.Stats.EluSkills[LoopC].ToString());
                        WriteVar(UserFile, "SKILLS", "EXPSK" + LoopC, withBlock.Stats.ExpSkills[LoopC].ToString());
                    }


                    WriteVar(UserFile, "CONTACTO", "Email", withBlock.email);

                    WriteVar(UserFile, "INIT", "Genero", Convert.ToInt32((byte)withBlock.Genero).ToString());
                    WriteVar(UserFile, "INIT", "Raza", Convert.ToInt32((byte)withBlock.raza).ToString());
                    WriteVar(UserFile, "INIT", "Hogar", Convert.ToInt32((int)withBlock.Hogar).ToString());
                    WriteVar(UserFile, "INIT", "Clase", Convert.ToInt32((int)withBlock.clase).ToString());
                    WriteVar(UserFile, "INIT", "Desc", withBlock.desc);

                    WriteVar(UserFile, "INIT", "Heading", Convert.ToInt32((byte)withBlock.Char_Renamed.heading).ToString());

                    WriteVar(UserFile, "INIT", "Head", withBlock.OrigChar.Head.ToString());

                    if (withBlock.flags.Muerto == 0)
                    {
                        WriteVar(UserFile, "INIT", "Body", withBlock.Char_Renamed.body.ToString());
                    }

                    WriteVar(UserFile, "INIT", "Arma", withBlock.Char_Renamed.WeaponAnim.ToString());
                    WriteVar(UserFile, "INIT", "Escudo", withBlock.Char_Renamed.ShieldAnim.ToString());
                    WriteVar(UserFile, "INIT", "Casco", withBlock.Char_Renamed.CascoAnim.ToString());

                    TempDate = DateTime.FromOADate(DateTime.Now.ToOADate() - withBlock.LogOnTime.ToOADate());
                    withBlock.LogOnTime = DateTime.Now;
                    withBlock.UpTime = withBlock.UpTime + Math.Abs(TempDate.Day - 30) * 24 * 3600 + System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetHour(TempDate) * 3600 + System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetMinute(TempDate) * 60 + System.Threading.Thread.CurrentThread.CurrentCulture.Calendar.GetSecond(TempDate);
                    withBlock.UpTime = withBlock.UpTime;
                    WriteVar(UserFile, "INIT", "UpTime", withBlock.UpTime.ToString());

                    // First time around?
                    int argEmptySpaces2 = 1024;
                    if (string.IsNullOrEmpty(GetVar(UserFile, "INIT", "LastIP1", EmptySpaces: ref argEmptySpaces2)))
                    {
                        // Is it a different ip from last time?
                        WriteVar(UserFile, "INIT", "LastIP1", withBlock.ip + " - " + Conversions.ToString(DateTime.Today) + ":" + Conversions.ToString(DateAndTime.TimeOfDay));
                    }
                    else
                    {
                        string getLastIPFromVar()
                        {
                            int argEmptySpaces = 1024;
                            string lastIP = GetVar(UserFile, "INIT", "LastIP1", EmptySpaces: ref argEmptySpaces);
                            if (lastIP is not null)
                            {
                                int spacePos = lastIP.IndexOf(" ");
                                return spacePos > 0 ? lastIP.Substring(0, spacePos) : "";
                            }
                            return "";
                        }

                        if ((withBlock.ip ?? "") != (getLastIPFromVar() ?? ""))
                        {
                            for (i = 5; i >= 2; i -= 1)
                            {
                                int argEmptySpaces1 = 1024;
                                WriteVar(UserFile, "INIT", "LastIP" + i, GetVar(UserFile, "INIT", "LastIP" + (i - 1).ToString(), EmptySpaces: ref argEmptySpaces1));
                            }
                            WriteVar(UserFile, "INIT", "LastIP1", withBlock.ip + " - " + Conversions.ToString(DateTime.Today) + ":" + Conversions.ToString(DateAndTime.TimeOfDay));
                        }
                        // Same ip, just update the date
                        else
                        {
                            WriteVar(UserFile, "INIT", "LastIP1", withBlock.ip + " - " + Conversions.ToString(DateTime.Today) + ":" + Conversions.ToString(DateAndTime.TimeOfDay));
                        }
                    }

                    WriteVar(UserFile, "INIT", "Position", withBlock.Pos.Map + "-" + withBlock.Pos.X + "-" + withBlock.Pos.Y);


                    WriteVar(UserFile, "STATS", "GLD", withBlock.Stats.GLD.ToString());
                    WriteVar(UserFile, "STATS", "BANCO", withBlock.Stats.Banco.ToString());

                    WriteVar(UserFile, "STATS", "MaxHP", withBlock.Stats.MaxHp.ToString());
                    WriteVar(UserFile, "STATS", "MinHP", withBlock.Stats.MinHp.ToString());

                    WriteVar(UserFile, "STATS", "MaxSTA", withBlock.Stats.MaxSta.ToString());
                    WriteVar(UserFile, "STATS", "MinSTA", withBlock.Stats.MinSta.ToString());

                    WriteVar(UserFile, "STATS", "MaxMAN", withBlock.Stats.MaxMAN.ToString());
                    WriteVar(UserFile, "STATS", "MinMAN", withBlock.Stats.MinMAN.ToString());

                    WriteVar(UserFile, "STATS", "MaxHIT", withBlock.Stats.MaxHIT.ToString());
                    WriteVar(UserFile, "STATS", "MinHIT", withBlock.Stats.MinHIT.ToString());

                    WriteVar(UserFile, "STATS", "MaxAGU", withBlock.Stats.MaxAGU.ToString());
                    WriteVar(UserFile, "STATS", "MinAGU", withBlock.Stats.MinAGU.ToString());

                    WriteVar(UserFile, "STATS", "MaxHAM", withBlock.Stats.MaxHam.ToString());
                    WriteVar(UserFile, "STATS", "MinHAM", withBlock.Stats.MinHam.ToString());

                    WriteVar(UserFile, "STATS", "SkillPtsLibres", withBlock.Stats.SkillPts.ToString());

                    WriteVar(UserFile, "STATS", "EXP", withBlock.Stats.Exp.ToString());
                    WriteVar(UserFile, "STATS", "ELV", withBlock.Stats.ELV.ToString());


                    WriteVar(UserFile, "STATS", "ELU", withBlock.Stats.ELU.ToString());
                    WriteVar(UserFile, "MUERTES", "UserMuertes", withBlock.Stats.UsuariosMatados.ToString());
                    // Call WriteVar(UserFile, "MUERTES", "CrimMuertes", .Stats.CriminalesMatados.ToString())
                    WriteVar(UserFile, "MUERTES", "NpcsMuertes", withBlock.Stats.NPCsMuertos.ToString());

                    // [KEVIN]----------------------------------------------------------------------------
                    // *******************************************************************************************
                    WriteVar(UserFile, "BancoInventory", "CantidadItems", Migration.ParseVal(withBlock.BancoInvent.NroItems.ToString()).ToString());
                    for (loopd = 1; loopd <= Declaraciones.MAX_BANCOINVENTORY_SLOTS; loopd++)
                        WriteVar(UserFile, "BancoInventory", "Obj" + loopd, withBlock.BancoInvent.Object_Renamed[loopd].ObjIndex + "-" + withBlock.BancoInvent.Object_Renamed[loopd].Amount);
                    // *******************************************************************************************
                    // [/KEVIN]-----------

                    // Save Inv
                    WriteVar(UserFile, "Inventory", "CantidadItems", Migration.ParseVal(withBlock.Invent.NroItems.ToString()).ToString());

                    for (LoopC = 1; LoopC <= Declaraciones.MAX_INVENTORY_SLOTS; LoopC++)
                        WriteVar(UserFile, "Inventory", "Obj" + LoopC, withBlock.Invent.Object_Renamed[LoopC].ObjIndex + "-" + withBlock.Invent.Object_Renamed[LoopC].Amount + "-" + withBlock.Invent.Object_Renamed[LoopC].Equipped);

                    WriteVar(UserFile, "Inventory", "WeaponEqpSlot", withBlock.Invent.WeaponEqpSlot.ToString());
                    WriteVar(UserFile, "Inventory", "ArmourEqpSlot", withBlock.Invent.ArmourEqpSlot.ToString());
                    WriteVar(UserFile, "Inventory", "CascoEqpSlot", withBlock.Invent.CascoEqpSlot.ToString());
                    WriteVar(UserFile, "Inventory", "EscudoEqpSlot", withBlock.Invent.EscudoEqpSlot.ToString());
                    WriteVar(UserFile, "Inventory", "BarcoSlot", withBlock.Invent.BarcoSlot.ToString());
                    WriteVar(UserFile, "Inventory", "MunicionSlot", withBlock.Invent.MunicionEqpSlot.ToString());
                    WriteVar(UserFile, "Inventory", "MochilaSlot", withBlock.Invent.MochilaEqpSlot.ToString());
                    // /Nacho

                    WriteVar(UserFile, "Inventory", "AnilloSlot", withBlock.Invent.AnilloEqpSlot.ToString());


                    // Reputacion
                    WriteVar(UserFile, "REP", "Asesino", withBlock.Reputacion.AsesinoRep.ToString());
                    WriteVar(UserFile, "REP", "Bandido", withBlock.Reputacion.BandidoRep.ToString());
                    WriteVar(UserFile, "REP", "Burguesia", withBlock.Reputacion.BurguesRep.ToString());
                    WriteVar(UserFile, "REP", "Ladrones", withBlock.Reputacion.LadronesRep.ToString());
                    WriteVar(UserFile, "REP", "Nobles", withBlock.Reputacion.NobleRep.ToString());
                    WriteVar(UserFile, "REP", "Plebe", withBlock.Reputacion.PlebeRep.ToString());

                    L = -withBlock.Reputacion.AsesinoRep + -withBlock.Reputacion.BandidoRep + withBlock.Reputacion.BurguesRep + -withBlock.Reputacion.LadronesRep + withBlock.Reputacion.NobleRep + withBlock.Reputacion.PlebeRep;
                    L = L / 6;
                    WriteVar(UserFile, "REP", "Promedio", L.ToString());


                    for (LoopC = 1; LoopC <= Declaraciones.MAXUSERHECHIZOS; LoopC++)
                    {
                        cad = withBlock.Stats.UserHechizos[LoopC].ToString();
                        WriteVar(UserFile, "HECHIZOS", "H" + LoopC, cad);
                    }

                    NroMascotas = withBlock.NroMascotas;

                    for (LoopC = 1; LoopC <= Declaraciones.MAXMASCOTAS; LoopC++)
                    {
                        // Mascota valida?
                        if (withBlock.MascotasIndex[LoopC] > 0)
                        {
                            // Nos aseguramos que la criatura no fue invocada
                            if (Declaraciones.Npclist[withBlock.MascotasIndex[LoopC]].Contadores.TiempoExistencia == 0)
                            {
                                cad = withBlock.MascotasType[LoopC].ToString();
                            }
                            else // Si fue invocada no la guardamos
                            {
                                cad = "0";
                                NroMascotas = NroMascotas - 1;
                            }
                            WriteVar(UserFile, "MASCOTAS", "MAS" + LoopC, cad);
                        }
                        else
                        {
                            cad = withBlock.MascotasType[LoopC].ToString();
                            WriteVar(UserFile, "MASCOTAS", "MAS" + LoopC, cad);
                        }

                    }

                    WriteVar(UserFile, "MASCOTAS", "NroMascotas", NroMascotas.ToString());

                    // Devuelve el head de muerto
                    if (withBlock.flags.Muerto == 1)
                    {
                        withBlock.Char_Renamed.Head = Declaraciones.iCabezaMuerto;
                    }
                }
            }


            catch (Exception ex)
            {
                Console.WriteLine("Error in LoadSini: " + ex.Message);
                string argdesc1 = "Error en SaveUser";
                General.LogError(ref argdesc1);
            }
        }

        public static bool criminal(short UserIndex)
        {
            bool criminalRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            int L;

            {
                ref var withBlock = ref Declaraciones.UserList[UserIndex].Reputacion;
                L = -withBlock.AsesinoRep + -withBlock.BandidoRep + withBlock.BurguesRep + -withBlock.LadronesRep + withBlock.NobleRep + withBlock.PlebeRep;
                L = L / 6;
                criminalRet = L < 0;
            }

            return criminalRet;
        }

        public static void BackUPnPc(ref short NpcIndex)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short NpcNumero;
            string npcfile;
            short LoopC;


            NpcNumero = Declaraciones.Npclist[NpcIndex].Numero;

            // If NpcNumero > 499 Then
            // npcfile = DatPath & "bkNPCs-HOSTILES.dat"
            // Else
            npcfile = Declaraciones.DatPath + "bkNPCs.dat";
            // End If

            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];
                // General
                WriteVar(npcfile, "NPC" + NpcNumero, "Name", withBlock.name);
                WriteVar(npcfile, "NPC" + NpcNumero, "Desc", withBlock.desc);
                WriteVar(npcfile, "NPC" + NpcNumero, "Head", Migration.ParseVal(withBlock.Char_Renamed.Head.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "Body", Migration.ParseVal(withBlock.Char_Renamed.body.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "Heading", Convert.ToInt32((byte)withBlock.Char_Renamed.heading).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "Movement", Convert.ToInt32((int)withBlock.Movement).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "Attackable", Migration.ParseVal(withBlock.Attackable.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "Comercia", Migration.ParseVal(withBlock.Comercia.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "TipoItems", Migration.ParseVal(withBlock.TipoItems.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "Hostil", Migration.ParseVal(withBlock.Hostile.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "GiveEXP", Migration.ParseVal(withBlock.GiveEXP.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "GiveGLD", Migration.ParseVal(withBlock.GiveGLD.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "Hostil", Migration.ParseVal(withBlock.Hostile.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "InvReSpawn", Migration.ParseVal(withBlock.InvReSpawn.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "NpcType", Convert.ToInt32((int)withBlock.NPCtype).ToString());


                // Stats
                WriteVar(npcfile, "NPC" + NpcNumero, "Alineacion", Migration.ParseVal(withBlock.Stats.Alineacion.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "DEF", Migration.ParseVal(withBlock.Stats.def.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "MaxHit", Migration.ParseVal(withBlock.Stats.MaxHIT.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "MaxHp", Migration.ParseVal(withBlock.Stats.MaxHp.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "MinHit", Migration.ParseVal(withBlock.Stats.MinHIT.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "MinHp", Migration.ParseVal(withBlock.Stats.MinHp.ToString()).ToString());


                // Flags
                WriteVar(npcfile, "NPC" + NpcNumero, "ReSpawn", Migration.ParseVal(withBlock.flags.Respawn.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "BackUp", Migration.ParseVal(withBlock.flags.BackUp.ToString()).ToString());
                WriteVar(npcfile, "NPC" + NpcNumero, "Domable", Migration.ParseVal(withBlock.flags.Domable.ToString()).ToString());

                // Inventario
                WriteVar(npcfile, "NPC" + NpcNumero, "NroItems", Migration.ParseVal(withBlock.Invent.NroItems.ToString()).ToString());
                if (withBlock.Invent.NroItems > 0)
                {
                    for (LoopC = 1; LoopC <= Declaraciones.MAX_INVENTORY_SLOTS; LoopC++)
                        WriteVar(npcfile, "NPC" + NpcNumero, "Obj" + LoopC, withBlock.Invent.Object_Renamed[LoopC].ObjIndex + "-" + withBlock.Invent.Object_Renamed[LoopC].Amount);
                }
            }
        }

        public static void CargarNpcBackUp(ref short NpcIndex, short NpcNumber)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            string npcfile;

            // If NpcNumber > 499 Then
            // npcfile = DatPath & "bkNPCs-HOSTILES.dat"
            // Else
            npcfile = Declaraciones.DatPath + "bkNPCs.dat";
            // End If

            short LoopC;
            string ln;
            {
                ref var withBlock = ref Declaraciones.Npclist[NpcIndex];

                withBlock.Numero = NpcNumber;
                int argEmptySpaces = 1024;
                withBlock.name = GetVar(npcfile, "NPC" + NpcNumber, "Name", EmptySpaces: ref argEmptySpaces);
                int argEmptySpaces1 = 1024;
                withBlock.desc = GetVar(npcfile, "NPC" + NpcNumber, "Desc", EmptySpaces: ref argEmptySpaces1);
                int argEmptySpaces2 = 1024;
                int argEmptySpaces3 = 1024;
                withBlock.Movement = (AI.TipoAI)Convert.ToInt32(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "Movement", EmptySpaces: ref argEmptySpaces3)));
                int argEmptySpaces4 = 1024;
                int argEmptySpaces5 = 1024;
                withBlock.NPCtype = (Declaraciones.eNPCType)Convert.ToInt32(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "NpcType", EmptySpaces: ref argEmptySpaces5)));

                int argEmptySpaces6 = 1024;
                withBlock.Char_Renamed.body = Convert.ToInt16(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "Body", EmptySpaces: ref argEmptySpaces6)));
                int argEmptySpaces7 = 1024;
                withBlock.Char_Renamed.Head = Convert.ToInt16(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "Head", EmptySpaces: ref argEmptySpaces7)));
                int argEmptySpaces8 = 1024;
                int argEmptySpaces9 = 1024;
                withBlock.Char_Renamed.heading = (Declaraciones.eHeading)Convert.ToByte(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "Heading", EmptySpaces: ref argEmptySpaces9)));

                int argEmptySpaces10 = 1024;
                withBlock.Attackable = Convert.ToByte(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "Attackable", EmptySpaces: ref argEmptySpaces10)));
                int argEmptySpaces11 = 1024;
                withBlock.Comercia = Convert.ToInt16(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "Comercia", EmptySpaces: ref argEmptySpaces11)));
                int argEmptySpaces12 = 1024;
                withBlock.Hostile = Convert.ToByte(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "Hostile", EmptySpaces: ref argEmptySpaces12)));
                int argEmptySpaces13 = 1024;
                withBlock.GiveEXP = Convert.ToInt32(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "GiveEXP", EmptySpaces: ref argEmptySpaces13)));


                int argEmptySpaces14 = 1024;
                withBlock.GiveGLD = Convert.ToInt32(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "GiveGLD", EmptySpaces: ref argEmptySpaces14)));

                int argEmptySpaces15 = 1024;
                withBlock.InvReSpawn = Convert.ToByte(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "InvReSpawn", EmptySpaces: ref argEmptySpaces15)));

                int argEmptySpaces16 = 1024;
                withBlock.Stats.MaxHp = Convert.ToInt32(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "MaxHP", EmptySpaces: ref argEmptySpaces16)));
                int argEmptySpaces17 = 1024;
                withBlock.Stats.MinHp = Convert.ToInt32(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "MinHP", EmptySpaces: ref argEmptySpaces17)));
                int argEmptySpaces18 = 1024;
                withBlock.Stats.MaxHIT = Convert.ToInt16(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "MaxHIT", EmptySpaces: ref argEmptySpaces18)));
                int argEmptySpaces19 = 1024;
                withBlock.Stats.MinHIT = Convert.ToInt16(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "MinHIT", EmptySpaces: ref argEmptySpaces19)));
                int argEmptySpaces20 = 1024;
                withBlock.Stats.def = Convert.ToInt16(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "DEF", EmptySpaces: ref argEmptySpaces20)));
                int argEmptySpaces21 = 1024;
                withBlock.Stats.Alineacion = Convert.ToInt16(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "Alineacion", EmptySpaces: ref argEmptySpaces21)));


                int argEmptySpaces22 = 1024;
                withBlock.Invent.NroItems = Convert.ToInt16(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "NROITEMS", EmptySpaces: ref argEmptySpaces22)));
                if (withBlock.Invent.NroItems > 0)
                {
                    for (LoopC = 1; LoopC <= Declaraciones.MAX_INVENTORY_SLOTS; LoopC++)
                    {
                        int argEmptySpaces23 = 1024;
                        ln = GetVar(npcfile, "NPC" + NpcNumber, "Obj" + LoopC, EmptySpaces: ref argEmptySpaces23);
                        withBlock.Invent.Object_Renamed[LoopC].ObjIndex = Convert.ToInt16(Migration.ParseVal(General.ReadField(1, ref ln, 45)));
                        withBlock.Invent.Object_Renamed[LoopC].Amount = Convert.ToInt16(Migration.ParseVal(General.ReadField(2, ref ln, 45)));

                    }
                }
                else
                {
                    for (LoopC = 1; LoopC <= Declaraciones.MAX_INVENTORY_SLOTS; LoopC++)
                    {
                        withBlock.Invent.Object_Renamed[LoopC].ObjIndex = 0;
                        withBlock.Invent.Object_Renamed[LoopC].Amount = 0;
                    }
                }

                for (LoopC = 1; LoopC <= Declaraciones.MAX_NPC_DROPS; LoopC++)
                {
                    int argEmptySpaces24 = 1024;
                    ln = GetVar(npcfile, "NPC" + NpcNumber, "Drop" + LoopC, EmptySpaces: ref argEmptySpaces24);
                    withBlock.Drop[LoopC].ObjIndex = Convert.ToInt16(Migration.ParseVal(General.ReadField(1, ref ln, 45)));
                    withBlock.Drop[LoopC].Amount = Convert.ToInt32(Migration.ParseVal(General.ReadField(2, ref ln, 45)));
                }

                withBlock.flags.NPCActive = true;
                int argEmptySpaces25 = 1024;
                withBlock.flags.Respawn = Convert.ToByte(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "ReSpawn", EmptySpaces: ref argEmptySpaces25)));
                int argEmptySpaces26 = 1024;
                withBlock.flags.BackUp = Convert.ToByte(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "BackUp", EmptySpaces: ref argEmptySpaces26)));
                int argEmptySpaces27 = 1024;
                withBlock.flags.Domable = Convert.ToInt16(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "Domable", EmptySpaces: ref argEmptySpaces27)));
                int argEmptySpaces28 = 1024;
                withBlock.flags.RespawnOrigPos = Convert.ToByte(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "OrigPos", EmptySpaces: ref argEmptySpaces28)));

                // Tipo de items con los que comercia
                int argEmptySpaces29 = 1024;
                withBlock.TipoItems = Convert.ToInt16(Migration.ParseVal(GetVar(npcfile, "NPC" + NpcNumber, "TipoItems", EmptySpaces: ref argEmptySpaces29)));
            }
        }


        public static void LogBan(short BannedIndex, short UserIndex, string motivo)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            WriteVar(AppDomain.CurrentDomain.BaseDirectory + "logs/" + "BanDetail.log", Declaraciones.UserList[BannedIndex].name, "BannedBy", Declaraciones.UserList[UserIndex].name);
            WriteVar(AppDomain.CurrentDomain.BaseDirectory + "logs/" + "BanDetail.log", Declaraciones.UserList[BannedIndex].name, "Reason", motivo);

            // Log interno del servidor, lo usa para hacer un UNBAN general de toda la gente banned
            General.AppendLog("logs/GenteBanned.log", Declaraciones.UserList[BannedIndex].name);
        }


        public static void LogBanFromName(string BannedName, short UserIndex, string motivo)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            WriteVar(AppDomain.CurrentDomain.BaseDirectory + "logs/" + "BanDetail.dat", BannedName, "BannedBy", Declaraciones.UserList[UserIndex].name);
            WriteVar(AppDomain.CurrentDomain.BaseDirectory + "logs/" + "BanDetail.dat", BannedName, "Reason", motivo);

            // Log interno del servidor, lo usa para hacer un UNBAN general de toda la gente banned
            General.AppendLog("logs/GenteBanned.log", BannedName);
        }


        public static void Ban(string BannedName, string Baneador, string motivo)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            WriteVar(AppDomain.CurrentDomain.BaseDirectory + "logs/" + "BanDetail.dat", BannedName, "BannedBy", Baneador);
            WriteVar(AppDomain.CurrentDomain.BaseDirectory + "logs/" + "BanDetail.dat", BannedName, "Reason", motivo);


            // Log interno del servidor, lo usa para hacer un UNBAN general de toda la gente banned
            General.AppendLog("logs/GenteBanned.log", BannedName);
        }

        public static void CargaApuestas()
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            int argEmptySpaces = 1024;
            Admin.Apuestas.Ganancias = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.DatPath + "apuestas.dat", "Main", "Ganancias", EmptySpaces: ref argEmptySpaces)));
            int argEmptySpaces1 = 1024;
            Admin.Apuestas.Perdidas = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.DatPath + "apuestas.dat", "Main", "Perdidas", EmptySpaces: ref argEmptySpaces1)));
            int argEmptySpaces2 = 1024;
            Admin.Apuestas.Jugadas = Convert.ToInt32(Migration.ParseVal(GetVar(Declaraciones.DatPath + "apuestas.dat", "Main", "Jugadas", EmptySpaces: ref argEmptySpaces2)));
        }

        public static void generateMatrix(short mapa)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short i;
            short j;
            short X;
            short Y;

            // UPGRADE_WARNING: Es posible que la matriz distanceToCities necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
            // UPGRADE_WARNING: El límite inferior de la matriz distanceToCities ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            Declaraciones.distanceToCities = new Declaraciones.HomeDistance[(Declaraciones.NumMaps + 1)];
            ArrayInitializers.InitializeStruct(Declaraciones.distanceToCities);

            for (j = 1; j <= Declaraciones.NUMCIUDADES; j++)
            {
                var loopTo = Declaraciones.NumMaps;
                for (i = 1; i <= loopTo; i++)
                    Declaraciones.distanceToCities[i].distanceToCity[j] = -1;
            }

            for (j = 1; j <= Declaraciones.NUMCIUDADES; j++)
            {
                for (i = 1; i <= 4; i++)
                {
                    switch (i)
                    {
                        case (short)Declaraciones.eHeading.NORTH:
                            {
                                setDistance(getLimit(Declaraciones.Ciudades[j].Map, (byte)Declaraciones.eHeading.NORTH), Convert.ToByte(j), i, 0, 1);
                                break;
                            }
                        case (short)Declaraciones.eHeading.EAST:
                            {
                                setDistance(getLimit(Declaraciones.Ciudades[j].Map, (byte)Declaraciones.eHeading.EAST), Convert.ToByte(j), i, 1, 0);
                                break;
                            }
                        case (short)Declaraciones.eHeading.SOUTH:
                            {
                                setDistance(getLimit(Declaraciones.Ciudades[j].Map, (byte)Declaraciones.eHeading.SOUTH), Convert.ToByte(j), i, 0, 1);
                                break;
                            }
                        case (short)Declaraciones.eHeading.WEST:
                            {
                                setDistance(getLimit(Declaraciones.Ciudades[j].Map, (byte)Declaraciones.eHeading.WEST), Convert.ToByte(j), i, -1, 0);
                                break;
                            }
                    }
                }
            }
        }

        public static void setDistance(short mapa, byte city, short side, short X = 0, short Y = 0)
        {
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            short i;
            short lim;

            if (mapa <= 0 | mapa > Declaraciones.NumMaps)
                return;

            if (Declaraciones.distanceToCities[mapa].distanceToCity[city] >= 0)
                return;

            if (mapa == Declaraciones.Ciudades[city].Map)
            {
                Declaraciones.distanceToCities[mapa].distanceToCity[city] = 0;
            }
            else
            {
                Declaraciones.distanceToCities[mapa].distanceToCity[city] = (short)(Math.Abs(X) + Math.Abs(Y));
            }

            for (i = 1; i <= 4; i++)
            {
                lim = getLimit(mapa, Convert.ToByte(i));
                if (lim > 0)
                {
                    switch (i)
                    {
                        case (short)Declaraciones.eHeading.NORTH:
                            {
                                setDistance(lim, city, i, X, Convert.ToInt16(Y + 1));
                                break;
                            }
                        case (short)Declaraciones.eHeading.EAST:
                            {
                                setDistance(lim, city, i, Convert.ToInt16(X + 1), Y);
                                break;
                            }
                        case (short)Declaraciones.eHeading.SOUTH:
                            {
                                setDistance(lim, city, i, X, Convert.ToInt16(Y - 1));
                                break;
                            }
                        case (short)Declaraciones.eHeading.WEST:
                            {
                                setDistance(lim, city, i, Convert.ToInt16(X - 1), Y);
                                break;
                            }
                    }
                }
            }
        }

        public static short getLimit(short mapa, byte side)
        {
            short getLimitRet = default;
            // ***************************************************
            // Author: Budi
            // Last Modification: 31/01/2010
            // Retrieves the limit in the given side in the given map.
            // TODO: This should be set in the .inf map file.
            // ***************************************************
            short i, X;
            short Y;

            if (mapa <= 0)
                return getLimitRet;

            for (X = 15; X <= 87; X++)
            {
                for (Y = 0; Y <= 3; Y++)
                {
                    switch (side)
                    {
                        case (byte)Declaraciones.eHeading.NORTH:
                            {
                                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto X. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                getLimitRet = Declaraciones.MapData[mapa, X, 7 + Y].TileExit.Map;
                                break;
                            }
                        case (byte)Declaraciones.eHeading.EAST:
                            {
                                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto X. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                getLimitRet = Declaraciones.MapData[mapa, 92 - Y, X].TileExit.Map;
                                break;
                            }
                        case (byte)Declaraciones.eHeading.SOUTH:
                            {
                                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto X. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                getLimitRet = Declaraciones.MapData[mapa, X, 94 - Y].TileExit.Map;
                                break;
                            }
                        case (byte)Declaraciones.eHeading.WEST:
                            {
                                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto X. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                                getLimitRet = Declaraciones.MapData[mapa, 9 + Y, X].TileExit.Map;
                                break;
                            }
                    }
                    if (getLimitRet > 0)
                        return getLimitRet;
                }
            }

            return getLimitRet;
        }


        public static void LoadArmadurasFaccion()
        {
            // ***************************************************
            // Author: ZaMa
            // Last Modification: 15/04/2010
            // 
            // ***************************************************
            int ClassIndex;
            int RaceIndex;

            short ArmaduraIndex;


            for (ClassIndex = 1; ClassIndex <= Declaraciones.NUMCLASES; ClassIndex++)
            {

                // Defensa minima para armadas altos
                int argEmptySpaces = 1024;
                ArmaduraIndex = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmadurasFaccionarias.dat", "CLASE" + ClassIndex, "DefMinArmyAlto", EmptySpaces: ref argEmptySpaces)));

                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Drow].Armada[(int)ModFacciones.eTipoDefArmors.ieBaja] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Elfo].Armada[(int)ModFacciones.eTipoDefArmors.ieBaja] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Humano].Armada[(int)ModFacciones.eTipoDefArmors.ieBaja] = ArmaduraIndex;

                // Defensa minima para armadas bajos
                int argEmptySpaces1 = 1024;
                ArmaduraIndex = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmadurasFaccionarias.dat", "CLASE" + ClassIndex, "DefMinArmyBajo", EmptySpaces: ref argEmptySpaces1)));

                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Enano].Armada[(int)ModFacciones.eTipoDefArmors.ieBaja] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Gnomo].Armada[(int)ModFacciones.eTipoDefArmors.ieBaja] = ArmaduraIndex;

                // Defensa minima para caos altos
                int argEmptySpaces2 = 1024;
                ArmaduraIndex = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmadurasFaccionarias.dat", "CLASE" + ClassIndex, "DefMinCaosAlto", EmptySpaces: ref argEmptySpaces2)));

                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Drow].Caos[(int)ModFacciones.eTipoDefArmors.ieBaja] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Elfo].Caos[(int)ModFacciones.eTipoDefArmors.ieBaja] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Humano].Caos[(int)ModFacciones.eTipoDefArmors.ieBaja] = ArmaduraIndex;

                // Defensa minima para caos bajos
                int argEmptySpaces3 = 1024;
                ArmaduraIndex = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmadurasFaccionarias.dat", "CLASE" + ClassIndex, "DefMinCaosBajo", EmptySpaces: ref argEmptySpaces3)));

                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Enano].Caos[(int)ModFacciones.eTipoDefArmors.ieBaja] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Gnomo].Caos[(int)ModFacciones.eTipoDefArmors.ieBaja] = ArmaduraIndex;


                // Defensa media para armadas altos
                int argEmptySpaces4 = 1024;
                ArmaduraIndex = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmadurasFaccionarias.dat", "CLASE" + ClassIndex, "DefMedArmyAlto", EmptySpaces: ref argEmptySpaces4)));

                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Drow].Armada[(int)ModFacciones.eTipoDefArmors.ieMedia] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Elfo].Armada[(int)ModFacciones.eTipoDefArmors.ieMedia] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Humano].Armada[(int)ModFacciones.eTipoDefArmors.ieMedia] = ArmaduraIndex;

                // Defensa media para armadas bajos
                int argEmptySpaces5 = 1024;
                ArmaduraIndex = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmadurasFaccionarias.dat", "CLASE" + ClassIndex, "DefMedArmyBajo", EmptySpaces: ref argEmptySpaces5)));

                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Enano].Armada[(int)ModFacciones.eTipoDefArmors.ieMedia] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Gnomo].Armada[(int)ModFacciones.eTipoDefArmors.ieMedia] = ArmaduraIndex;

                // Defensa media para caos altos
                int argEmptySpaces6 = 1024;
                ArmaduraIndex = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmadurasFaccionarias.dat", "CLASE" + ClassIndex, "DefMedCaosAlto", EmptySpaces: ref argEmptySpaces6)));

                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Drow].Caos[(int)ModFacciones.eTipoDefArmors.ieMedia] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Elfo].Caos[(int)ModFacciones.eTipoDefArmors.ieMedia] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Humano].Caos[(int)ModFacciones.eTipoDefArmors.ieMedia] = ArmaduraIndex;

                // Defensa media para caos bajos
                int argEmptySpaces7 = 1024;
                ArmaduraIndex = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmadurasFaccionarias.dat", "CLASE" + ClassIndex, "DefMedCaosBajo", EmptySpaces: ref argEmptySpaces7)));

                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Enano].Caos[(int)ModFacciones.eTipoDefArmors.ieMedia] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Gnomo].Caos[(int)ModFacciones.eTipoDefArmors.ieMedia] = ArmaduraIndex;


                // Defensa alta para armadas altos
                int argEmptySpaces8 = 1024;
                ArmaduraIndex = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmadurasFaccionarias.dat", "CLASE" + ClassIndex, "DefAltaArmyAlto", EmptySpaces: ref argEmptySpaces8)));

                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Drow].Armada[(int)ModFacciones.eTipoDefArmors.ieAlta] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Elfo].Armada[(int)ModFacciones.eTipoDefArmors.ieAlta] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Humano].Armada[(int)ModFacciones.eTipoDefArmors.ieAlta] = ArmaduraIndex;

                // Defensa alta para armadas bajos
                int argEmptySpaces9 = 1024;
                ArmaduraIndex = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmadurasFaccionarias.dat", "CLASE" + ClassIndex, "DefAltaArmyBajo", EmptySpaces: ref argEmptySpaces9)));

                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Enano].Armada[(int)ModFacciones.eTipoDefArmors.ieAlta] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Gnomo].Armada[(int)ModFacciones.eTipoDefArmors.ieAlta] = ArmaduraIndex;

                // Defensa alta para caos altos
                int argEmptySpaces10 = 1024;
                ArmaduraIndex = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmadurasFaccionarias.dat", "CLASE" + ClassIndex, "DefAltaCaosAlto", EmptySpaces: ref argEmptySpaces10)));

                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Drow].Caos[(int)ModFacciones.eTipoDefArmors.ieAlta] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Elfo].Caos[(int)ModFacciones.eTipoDefArmors.ieAlta] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Humano].Caos[(int)ModFacciones.eTipoDefArmors.ieAlta] = ArmaduraIndex;

                // Defensa alta para caos bajos
                int argEmptySpaces11 = 1024;
                ArmaduraIndex = Convert.ToInt16(Migration.ParseVal(GetVar(Declaraciones.DatPath + "ArmadurasFaccionarias.dat", "CLASE" + ClassIndex, "DefAltaCaosBajo", EmptySpaces: ref argEmptySpaces11)));

                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Enano].Caos[(int)ModFacciones.eTipoDefArmors.ieAlta] = ArmaduraIndex;
                ModFacciones.ArmadurasFaccion[ClassIndex, (int)Declaraciones.eRaza.Gnomo].Caos[(int)ModFacciones.eTipoDefArmors.ieAlta] = ArmaduraIndex;

            }
        }
    }
}