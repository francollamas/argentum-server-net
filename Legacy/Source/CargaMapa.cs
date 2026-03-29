// Codigo by Loopzer todos los derechos se le reservan a él!
// GSZone 2010

using System;
using System.IO;
using System.Text.Json;

namespace Legacy;

internal static class CargaMapa
{
    public static void CargarMapa(short ind, string MAPFl)
    {
        short CountObj;
        short CTriger;
        short CG1;
        short CG2;
        short CG3;
        short CG4;
        short CNpc;
        short CSalir;
        short CBlk;

        TXYObj[] Mocha_Obj;
        TXYTriger[] Mocha_Triger;
        TXYG1[] Mocha_CG1;
        TXYG2[] Mocha_CG2;
        TXYG3[] Mocha_CG3;
        TXYG4[] Mocha_CG4;
        TXYNpc[] Mocha_Npc;
        TXYSalir[] Mocha_Salir;
        TXY[] Mocha_BLK;

        using (var fileStream =
               new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps", ind + ".map"), FileMode.Open))

        {
            using (var reader = new BinaryReader(fileStream))
            {
                CountObj = reader.ReadInt16();
                CTriger = reader.ReadInt16();
                CG1 = reader.ReadInt16();
                CG2 = reader.ReadInt16();
                CG3 = reader.ReadInt16();
                CG4 = reader.ReadInt16();
                CNpc = reader.ReadInt16();
                CSalir = reader.ReadInt16();
                CBlk = reader.ReadInt16();

                Mocha_Obj = new TXYObj[CountObj];
                Mocha_Triger = new TXYTriger[CTriger];
                Mocha_CG1 = new TXYG1[CG1];
                Mocha_CG2 = new TXYG2[CG2];
                Mocha_CG3 = new TXYG3[CG3];
                Mocha_CG4 = new TXYG4[CG4];
                Mocha_Npc = new TXYNpc[CNpc];
                Mocha_Salir = new TXYSalir[CSalir];
                Mocha_BLK = new TXY[CBlk];

                for (int i = 0, loopTo = CountObj - 1; i <= loopTo; i++)
                {
                    Mocha_Obj[i].XY.x = reader.ReadInt16();
                    Mocha_Obj[i].XY.y = reader.ReadInt16();
                    Mocha_Obj[i].mobj.ObjIndex = reader.ReadInt16();
                    Mocha_Obj[i].mobj.Amount = reader.ReadInt16();
                }

                for (int i = 0, loopTo1 = CTriger - 1; i <= loopTo1; i++)
                {
                    Mocha_Triger[i].XY.x = reader.ReadInt16();
                    Mocha_Triger[i].XY.y = reader.ReadInt16();
                    Mocha_Triger[i].Numero = reader.ReadInt16();
                }

                for (int i = 0, loopTo2 = CG1 - 1; i <= loopTo2; i++)
                {
                    Mocha_CG1[i].XY.x = reader.ReadInt16();
                    Mocha_CG1[i].XY.y = reader.ReadInt16();
                    Mocha_CG1[i].Numero = reader.ReadInt16();
                }

                for (int i = 0, loopTo3 = CG2 - 1; i <= loopTo3; i++)
                {
                    Mocha_CG2[i].XY.x = reader.ReadInt16();
                    Mocha_CG2[i].XY.y = reader.ReadInt16();
                    Mocha_CG2[i].Numero = reader.ReadInt16();
                }

                for (int i = 0, loopTo4 = CG3 - 1; i <= loopTo4; i++)
                {
                    Mocha_CG3[i].XY.x = reader.ReadInt16();
                    Mocha_CG3[i].XY.y = reader.ReadInt16();
                    Mocha_CG3[i].Numero = reader.ReadInt16();
                }

                for (int i = 0, loopTo5 = CG4 - 1; i <= loopTo5; i++)
                {
                    Mocha_CG4[i].XY.x = reader.ReadInt16();
                    Mocha_CG4[i].XY.y = reader.ReadInt16();
                    Mocha_CG4[i].Numero = reader.ReadInt16();
                }

                for (int i = 0, loopTo6 = CNpc - 1; i <= loopTo6; i++)
                {
                    Mocha_Npc[i].XY.x = reader.ReadInt16();
                    Mocha_Npc[i].XY.y = reader.ReadInt16();
                    Mocha_Npc[i].Numero = reader.ReadInt16();
                }

                for (int i = 0, loopTo7 = CSalir - 1; i <= loopTo7; i++)
                {
                    Mocha_Salir[i].XY.x = reader.ReadInt16();
                    Mocha_Salir[i].XY.y = reader.ReadInt16();
                    Mocha_Salir[i].Salida.Map = reader.ReadInt16();
                    Mocha_Salir[i].Salida.X = reader.ReadInt16();
                    Mocha_Salir[i].Salida.Y = reader.ReadInt16();
                }

                for (int i = 0, loopTo8 = CBlk - 1; i <= loopTo8; i++)
                {
                    Mocha_BLK[i].x = reader.ReadInt16();
                    Mocha_BLK[i].y = reader.ReadInt16();
                }
            }
        }

        short t;
        TXYObj tm;
        TXYTriger tt;
        TXYG1 tg1;
        TXYG2 tg2;
        TXYG3 tg3;
        TXYG4 tg4;
        TXYNpc tnpc;
        TXYSalir tsalir;
        TXY tblk;

        var loopTo9 = Convert.ToInt16(CountObj - 1);
        for (t = 0; t <= loopTo9; t++)
        {
            tm = Mocha_Obj[t];
            Declaraciones.MapData[ind, tm.XY.x, tm.XY.y].ObjInfo = tm.mobj;
        }

        var loopTo10 = Convert.ToInt16(CTriger - 1);
        for (t = 0; t <= loopTo10; t++)
        {
            tt = Mocha_Triger[t];
            Declaraciones.MapData[ind, tt.XY.x, tt.XY.y].trigger = (Declaraciones.eTrigger)tt.Numero;
        }

        var loopTo11 = Convert.ToInt16(CG1 - 1);
        for (t = 0; t <= loopTo11; t++)
        {
            tg1 = Mocha_CG1[t];
            Declaraciones.MapData[ind, tg1.XY.x, tg1.XY.y].Graphic[1] = tg1.Numero;
        }

        var loopTo12 = Convert.ToInt16(CG2 - 1);
        for (t = 0; t <= loopTo12; t++)
        {
            tg2 = Mocha_CG2[t];
            Declaraciones.MapData[ind, tg2.XY.x, tg2.XY.y].Graphic[2] = tg2.Numero;
        }

        var loopTo13 = Convert.ToInt16(CG3 - 1);
        for (t = 0; t <= loopTo13; t++)
        {
            tg3 = Mocha_CG3[t];
            Declaraciones.MapData[ind, tg3.XY.x, tg3.XY.y].Graphic[3] = tg3.Numero;
        }

        var loopTo14 = Convert.ToInt16(CG4 - 1);
        for (t = 0; t <= loopTo14; t++)
        {
            tg4 = Mocha_CG4[t];
            Declaraciones.MapData[ind, tg4.XY.x, tg4.XY.y].Graphic[4] = tg4.Numero;
        }

        short x;
        short y;
        string npcfile;

        var loopTo15 = Convert.ToInt16(CNpc - 1);
        for (t = 0; t <= loopTo15; t++)
        {
            tnpc = Mocha_Npc[t];
            x = tnpc.XY.x;
            y = tnpc.XY.y;
            Declaraciones.MapData[ind, x, y].NpcIndex = tnpc.Numero;

            npcfile = Declaraciones.DatPath + "NPCs.dat";

            // Si el npc debe hacer respawn en la pos
            // original la guardamos

            if (Convert.ToInt32(General.LeerNPCs.GetValue("NPC" + Declaraciones.MapData[ind, x, y].NpcIndex,
                    "PosOrig")) == 1)
            {
                // If Val(GetVar(npcfile, "NPC" & MapData(ind, x, y).NpcIndex, "PosOrig")) = 1 Then
                Declaraciones.MapData[ind, x, y].NpcIndex = NPCs.OpenNPC(Declaraciones.MapData[ind, x, y].NpcIndex);
                Declaraciones.Npclist[Declaraciones.MapData[ind, x, y].NpcIndex].Orig.Map = ind;
                Declaraciones.Npclist[Declaraciones.MapData[ind, x, y].NpcIndex].Orig.X = x;
                Declaraciones.Npclist[Declaraciones.MapData[ind, x, y].NpcIndex].Orig.Y = y;
            }
            else
            {
                Declaraciones.MapData[ind, x, y].NpcIndex = NPCs.OpenNPC(Declaraciones.MapData[ind, x, y].NpcIndex);
            }

            Declaraciones.Npclist[Declaraciones.MapData[ind, x, y].NpcIndex].Pos.Map = ind;
            Declaraciones.Npclist[Declaraciones.MapData[ind, x, y].NpcIndex].Pos.X = x;
            Declaraciones.Npclist[Declaraciones.MapData[ind, x, y].NpcIndex].Pos.Y = y;

            short argsndIndex = 0;
            NPCs.MakeNPCChar(true, ref argsndIndex, ref Declaraciones.MapData[ind, x, y].NpcIndex, ind, x, y);
        }

        var loopTo16 = Convert.ToInt16(CSalir - 1);
        for (t = 0; t <= loopTo16; t++)
        {
            tsalir = Mocha_Salir[t];
            Declaraciones.MapData[ind, tsalir.XY.x, tsalir.XY.y].TileExit = tsalir.Salida;
        }

        var loopTo17 = Convert.ToInt16(CBlk - 1);
        for (t = 0; t <= loopTo17; t++)
        {
            tblk = Mocha_BLK[t];
            Declaraciones.MapData[ind, tblk.x, tblk.y].Blocked = 1;
        }

        var jsonString = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Maps", ind + ".json"));
        var jsonDoc = JsonDocument.Parse(jsonString);
        var root = jsonDoc.RootElement;

        {
            ref var withBlock = ref Declaraciones.MapInfo_Renamed[ind];
            JsonElement tempProperty;
            if (root.TryGetProperty("name", out tempProperty))
                withBlock.name = tempProperty.GetString();
            if (root.TryGetProperty("musicnum", out tempProperty))
                withBlock.Music = tempProperty.GetInt32().ToString();
            if (root.TryGetProperty("magiasinefecto", out tempProperty))
                withBlock.MagiaSinEfecto = tempProperty.GetByte();
            if (root.TryGetProperty("invisinefecto", out tempProperty))
                withBlock.InviSinEfecto = tempProperty.GetByte();
            if (root.TryGetProperty("resusinefecto", out tempProperty))
                withBlock.ResuSinEfecto = tempProperty.GetByte();
            if (root.TryGetProperty("noencriptarmp", out tempProperty))
                withBlock.NoEncriptarMP = tempProperty.GetByte();
            if (root.TryGetProperty("robonpc", out tempProperty))
                withBlock.RoboNpcsPermitido = tempProperty.GetByte();
            if (root.TryGetProperty("pk", out tempProperty))
                withBlock.Pk = tempProperty.GetByte() == 1;
            if (root.TryGetProperty("terreno", out tempProperty))
                withBlock.Terreno = tempProperty.GetString();
            if (root.TryGetProperty("zona", out tempProperty))
                withBlock.Zona = tempProperty.GetString();
            if (root.TryGetProperty("restringir", out tempProperty))
                withBlock.Restringir = tempProperty.GetString();
            if (root.TryGetProperty("backup", out tempProperty))
                withBlock.BackUp = tempProperty.GetByte();
        }
    }

    public struct TXY
    {
        public short x;
        public short y;
    }

    public struct TXYTriger
    {
        public TXY XY;
        public short Numero;
    }

    public struct TXYG1
    {
        public TXY XY;
        public short Numero;
    }

    public struct TXYG2
    {
        public TXY XY;
        public short Numero;
    }

    public struct TXYG3
    {
        public TXY XY;
        public short Numero;
    }

    public struct TXYG4
    {
        public TXY XY;
        public short Numero;
    }

    public struct TXYObj
    {
        public TXY XY;
        public Declaraciones.Obj mobj;
    }

    public struct TXYNpc
    {
        public TXY XY;
        public short Numero;
    }

    public struct TXYSalir
    {
        public TXY XY;
        public Declaraciones.WorldPos Salida;
    }
}