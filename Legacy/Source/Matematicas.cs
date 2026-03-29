using System;
using Microsoft.VisualBasic;

namespace Legacy
{
    static class Matematicas
    {
        public static int Porcentaje(int Total, int Porc)
        {
            int PorcentajeRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            PorcentajeRet = Convert.ToInt32(Total * Porc / 100d);
            return PorcentajeRet;
        }

        public static int Distancia(ref Declaraciones.WorldPos wp1, ref Declaraciones.WorldPos wp2)
        {
            int DistanciaRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            // Encuentra la distancia entre dos WorldPos
            DistanciaRet = Math.Abs((short)(wp1.X - wp2.X)) + Math.Abs((short)(wp1.Y - wp2.Y)) + Math.Abs((short)(wp1.Map - wp2.Map)) * 100;
            return DistanciaRet;
        }

        public static double Distance(short X1, short Y1, short X2, short Y2)
        {
            double DistanceRet = default;
            // ***************************************************
            // Author: Unknown
            // Last Modification: -
            // 
            // ***************************************************

            // Encuentra la distancia entre dos puntos

            DistanceRet = Math.Sqrt(Math.Pow(Y1 - Y2, 2d) + Math.Pow(X1 - X2, 2d));
            return DistanceRet;
        }

        public static int RandomNumber(int LowerBound, int UpperBound)
        {
            int RandomNumberRet = default;
            // **************************************************************
            // Author: Juan Martín Sotuyo Dodero
            // Last Modify Date: 3/06/2006
            // Generates a random number in the range given - recoded to use longs and work properly with ranges
            // **************************************************************
            RandomNumberRet = Convert.ToInt32(Conversion.Fix(VBMath.Rnd() * (UpperBound - LowerBound + 1))) + LowerBound;
            return RandomNumberRet;
        }
    }
}