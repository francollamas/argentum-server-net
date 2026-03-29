using System;
using Microsoft.VisualBasic;

namespace Legacy;

internal static class modHexaStrings
{
    public static string hexMd52Asc(string MD5)
    {
        string hexMd52AscRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int i;
        string L;

        if ((MD5.Length & 0x1) != 0)
            MD5 = "0" + MD5;

        var loopTo = MD5.Length / 2;
        for (i = 1; i <= loopTo; i++)
        {
            L = MD5.Substring(2 * i - 2, 2);
            hexMd52AscRet = hexMd52AscRet + Strings.Chr(hexHex2Dec(L));
        }

        return hexMd52AscRet;
    }

    // UPGRADE_NOTE: hex se actualizó a hex_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    public static int hexHex2Dec(string hex_Renamed)
    {
        int hexHex2DecRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        hexHex2DecRet = Convert.ToInt32(Convert.ToInt64(hex_Renamed, 16));
        return hexHex2DecRet;
    }

    public static string txtOffset(string Text, short off)
    {
        string txtOffsetRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int i;
        string L;

        var loopTo = Text.Length;
        for (i = 1; i <= loopTo; i++)
        {
            L = Text.Substring(i - 1, 1);
            txtOffsetRet = txtOffsetRet + Strings.Chr((Strings.Asc(L) + off) & 0xFF);
        }

        return txtOffsetRet;
    }
}