using System.Diagnostics;
using System.Globalization;

namespace Legacy;

internal static class Migration
{
    private static readonly Stopwatch StopWatch = Stopwatch.StartNew();

    public static long GetTickCount()
    {
        long GetTickCountRet = default;
        GetTickCountRet = StopWatch.ElapsedMilliseconds;
        return GetTickCountRet;
    }

    // UPGRADE_NOTE: str se actualizó a str_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    public static int migr_LenB(string str_Renamed)
    {
        int migr_LenBRet = default;
        if (string.IsNullOrEmpty(str_Renamed))
            migr_LenBRet = 0;
        else
            migr_LenBRet = str_Renamed.Length * 2;

        return migr_LenBRet;
    }

    public static int migr_InStrB(string s1, string s2)
    {
        int migr_InStrBRet = default;
        int i;
        int maxPos;

        // Verificar que la subcadena no sea vacía
        if (s2.Length == 0)
        {
            migr_InStrBRet = 1; // Si la subcadena está vacía, consideramos que se encuentra al principio
            return migr_InStrBRet;
        }

        // La búsqueda solo puede ir hasta la longitud de la cadena principal menos la longitud de la subcadena
        maxPos = s1.Length - s2.Length + 1;

        // Buscar la subcadena dentro de la cadena principal
        var loopTo = maxPos;
        for (i = 1; i <= loopTo; i++)
            if ((s1.Substring(i - 1, s2.Length) ?? "") == (s2 ?? ""))
            {
                migr_InStrBRet = i;
                return migr_InStrBRet;
            }

        // Si no se encuentra la subcadena, devolver 0
        migr_InStrBRet = 0;
        return migr_InStrBRet;
    }

    /// <summary>
    ///     Reemplazo de Val() de VB6. Retorna 0 si el string no es numérico (mismo comportamiento que Val()).
    /// </summary>
    public static double ParseVal(string s)
    {
        if (s is null)
            return 0d;
        double result;
        if (double.TryParse(s.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out result)) return result;
        return 0d;
    }
}