using System;
using System.IO;
using Microsoft.VisualBasic;

namespace Legacy;

internal static class Statistics
{
    private static trainningData[] trainningInfo;

    // UPGRADE_WARNING: El límite inferior de la matriz fragLvlRaceData ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    // UPGRADE_WARNING: Es posible que la matriz fragLvlRaceData necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
    public static fragLvlRace[] fragLvlRaceData = new fragLvlRace[8];

    // UPGRADE_WARNING: El límite inferior de la matriz fragLvlLvlData ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    // UPGRADE_WARNING: Es posible que la matriz fragLvlLvlData necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
    public static fragLvlLvl[] fragLvlLvlData = new fragLvlLvl[8];

    // UPGRADE_WARNING: El límite inferior de la matriz fragAlignmentLvlData ha cambiado de 1,1 a 0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    private static readonly int[,] fragAlignmentLvlData = new int[51, 5];

    // Currency just in case.... chats are way TOO often...
    private static readonly decimal[] keyOcurrencies = new decimal[256];

    public static void Initialize()
    {
        // UPGRADE_WARNING: El límite inferior de la matriz trainningInfo ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        trainningInfo = new trainningData[Declaraciones.MaxUsers + 1];
    }

    public static void UserConnected(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        // A new user connected, load it's trainning time count
        var argEmptySpaces = 30;
        trainningInfo[UserIndex].trainningTime = Convert.ToInt32(Migration.ParseVal(
            ES.GetVar(Declaraciones.CharPath + Declaraciones.UserList[UserIndex].name.ToUpper() + ".chr", "RESEARCH",
                "TrainningTime", ref argEmptySpaces)));

        trainningInfo[UserIndex].startTick = Convert.ToInt32(Migration.GetTickCount());
    }

    public static void UserDisconnected(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref trainningInfo[UserIndex];
            // Update trainning time
            withBlock.trainningTime =
                Convert.ToInt32(withBlock.trainningTime + (Migration.GetTickCount() - withBlock.startTick) / 1000d);

            withBlock.startTick = Convert.ToInt32(Migration.GetTickCount());

            // Store info in char file
            ES.WriteVar(Declaraciones.CharPath + Declaraciones.UserList[UserIndex].name.ToUpper() + ".chr", "RESEARCH",
                "TrainningTime", withBlock.trainningTime.ToString());
        }
    }

    public static void UserLevelUp(short UserIndex)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        {
            ref var withBlock = ref trainningInfo[UserIndex];
            // Log the data
            General.AppendLog("logs/statistics.log",
                Declaraciones.UserList[UserIndex].name.ToUpper() + " completó el nivel " +
                Declaraciones.UserList[UserIndex].Stats.ELV + " en " +
                (withBlock.trainningTime + (Migration.GetTickCount() - withBlock.startTick) / 1000d) + " segundos.");

            // Reset data
            withBlock.trainningTime = 0;
            withBlock.startTick = Convert.ToInt32(Migration.GetTickCount());
        }
    }

    public static void StoreFrag(short killer, short victim)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        short clase;
        short raza;
        short alignment;

        if ((Declaraciones.UserList[victim].Stats.ELV > 50) | (Declaraciones.UserList[killer].Stats.ELV > 50))
            return;

        switch (Declaraciones.UserList[killer].clase)
        {
            case Declaraciones.eClass.Assasin:
            {
                clase = 1;
                break;
            }

            case Declaraciones.eClass.Bard:
            {
                clase = 2;
                break;
            }

            case Declaraciones.eClass.Mage:
            {
                clase = 3;
                break;
            }

            case Declaraciones.eClass.Paladin:
            {
                clase = 4;
                break;
            }

            case Declaraciones.eClass.Warrior:
            {
                clase = 5;
                break;
            }

            case Declaraciones.eClass.Cleric:
            {
                clase = 6;
                break;
            }

            case Declaraciones.eClass.Hunter:
            {
                clase = 7;
                break;
            }

            default:
            {
                return;
            }
        }

        switch (Declaraciones.UserList[killer].raza)
        {
            case Declaraciones.eRaza.Elfo:
            {
                raza = 1;
                break;
            }

            case Declaraciones.eRaza.Drow:
            {
                raza = 2;
                break;
            }

            case Declaraciones.eRaza.Enano:
            {
                raza = 3;
                break;
            }

            case Declaraciones.eRaza.Gnomo:
            {
                raza = 4;
                break;
            }

            case Declaraciones.eRaza.Humano:
            {
                raza = 5;
                break;
            }

            default:
            {
                return;
            }
        }

        if (Declaraciones.UserList[killer].Faccion.ArmadaReal != 0)
            alignment = 1;
        else if (Declaraciones.UserList[killer].Faccion.FuerzasCaos != 0)
            alignment = 2;
        else if (ES.criminal(killer))
            alignment = 3;
        else
            alignment = 4;

        fragLvlRaceData[clase].matrix[Declaraciones.UserList[killer].Stats.ELV, raza] =
            fragLvlRaceData[clase].matrix[Declaraciones.UserList[killer].Stats.ELV, raza] + 1;

        fragLvlLvlData[clase]
                .matrix[Declaraciones.UserList[killer].Stats.ELV, Declaraciones.UserList[victim].Stats.ELV] =
            fragLvlLvlData[clase]
                .matrix[Declaraciones.UserList[killer].Stats.ELV, Declaraciones.UserList[victim].Stats.ELV] + 1;

        fragAlignmentLvlData[Declaraciones.UserList[killer].Stats.ELV, alignment] =
            fragAlignmentLvlData[Declaraciones.UserList[killer].Stats.ELV, alignment] + 1;
    }

    public static void DumpStatistics()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        var line = default(string);
        int i;
        int j;

        using (var writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "logs/frags.txt"))
        {
            // Save lvl vs lvl frag matrix for each class - we use GNU Octave's ASCII file format

            writer.WriteLine("# name: fragLvlLvl_Ase");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 50");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 50; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlLvlData[1].matrix[i, j];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlLvl_Bar");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 50");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 50; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlLvlData[2].matrix[i, j];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlLvl_Mag");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 50");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 50; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlLvlData[3].matrix[i, j];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlLvl_Pal");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 50");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 50; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlLvlData[4].matrix[i, j];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlLvl_Gue");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 50");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 50; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlLvlData[5].matrix[i, j];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlLvl_Cle");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 50");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 50; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlLvlData[6].matrix[i, j];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlLvl_Caz");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 50");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 50; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlLvlData[7].matrix[i, j];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }


            // Save lvl vs race frag matrix for each class - we use GNU Octave's ASCII file format

            writer.WriteLine("# name: fragLvlRace_Ase");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 5");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 5; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlRaceData[1].matrix[i, j];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlRace_Bar");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 5");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 5; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlRaceData[2].matrix[i, j];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlRace_Mag");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 5");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 5; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlRaceData[3].matrix[i, j];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlRace_Pal");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 5");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 5; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlRaceData[4].matrix[i, j];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlRace_Gue");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 5");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 5; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlRaceData[5].matrix[i, j];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlRace_Cle");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 5");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 5; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlRaceData[6].matrix[i, j];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlRace_Caz");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 5");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 5; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlRaceData[7].matrix[i, j];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }


            // Save lvl vs class frag matrix for each race - we use GNU Octave's ASCII file format

            writer.WriteLine("# name: fragLvlClass_Elf");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 7");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 7; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlRaceData[j].matrix[i, 1];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlClass_Dar");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 7");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 7; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlRaceData[j].matrix[i, 2];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlClass_Dwa");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 7");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 7; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlRaceData[j].matrix[i, 3];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlClass_Gno");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 7");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 7; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlRaceData[j].matrix[i, 4];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }

            writer.WriteLine("# name: fragLvlClass_Hum");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 7");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 7; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragLvlRaceData[j].matrix[i, 5];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }


            // Save lvl vs alignment frag matrix for each race - we use GNU Octave's ASCII file format

            writer.WriteLine("# name: fragAlignmentLvl");
            writer.WriteLine("# type: matrix");
            writer.WriteLine("# rows: 4");
            writer.WriteLine("# columns: 50");

            for (j = 1; j <= 4; j++)
            {
                for (i = 1; i <= 50; i++)
                    line = line + " " + fragAlignmentLvlData[i, j];

                writer.WriteLine(line);
                line = Constants.vbNullString;
            }
        }


        // Dump Chat statistics
        using (var writer = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "logs/huffman.log"))
        {
            var Total = default(decimal);

            // Compute total characters
            for (i = 0; i <= 255; i++)
                Total = Total + keyOcurrencies[i];

            // Show each character's ocurrencies
            if (Total != 0m)
                for (i = 0; i <= 255; i++)
                    writer.WriteLine(i + "    " + Math.Round(keyOcurrencies[i] / Total, 8));

            writer.WriteLine("TOTAL =    " + Total);
        }
    }

    public static void ParseChat(ref string S)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int i;
        short key;

        var loopTo = S.Length;
        for (i = 1; i <= loopTo; i++)
        {
            key = Convert.ToInt16(Strings.Asc(S.Substring(i - 1, 1)));

            keyOcurrencies[key] = keyOcurrencies[key] + 1m;
        }

        // Add a NULL-terminated to consider that possibility too....
        keyOcurrencies[0] = keyOcurrencies[0] + 1m;
    }
    // **************************************************************
    // modStatistics.bas - Takes statistics on the game for later study.
    // 
    // Implemented by Juan Martín Sotuyo Dodero (Maraxus)
    // (juansotuyo@gmail.com)
    // **************************************************************

    private struct trainningData
    {
        public int startTick;
        public int trainningTime;
    }

    public struct fragLvlRace
    {
        [VBFixedArray(50, 5)] public int[,] matrix;

        // UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        public void Initialize()
        {
            // UPGRADE_WARNING: El límite inferior de la matriz matrix ha cambiado de 1,1 a 0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            matrix = new int[51, 6];
        }
    }

    public struct fragLvlLvl
    {
        [VBFixedArray(50, 50)] public int[,] matrix;

        // UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        public void Initialize()
        {
            // UPGRADE_WARNING: El límite inferior de la matriz matrix ha cambiado de 1,1 a 0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            matrix = new int[51, 51];
        }
    }
}