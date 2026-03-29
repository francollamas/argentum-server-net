using System;

namespace Legacy;

internal static class SecurityIp
{
    private const int IntervaloEntreConexiones = 1000;

    private const int LIMITECONEXIONESxIP = 10;
    // TODO MIGRA: se modifico para NO usar CopyMemory. Quizas tenga algun problema de performance. Revisar!

    // **************************************************************
    // General_IpSecurity.Bas - Maneja la seguridad de las IPs
    // 
    // Escrito y diseñado por DuNga (ltourrilhes@gmail.com)
    // **************************************************************

    // *************************************************  *************
    // General_IpSecurity.Bas - Maneja la seguridad de las IPs
    // 
    // Escrito y diseñado por DuNga (ltourrilhes@gmail.com)
    // *************************************************  *************

    private static int[] IpTables; // USAMOS 2 LONGS: UNO DE LA IP, SEGUIDO DE UNO DE LA INFO
    private static int EntrysCounter;
    private static int MaxValue;
    private static int Multiplicado; // Cuantas veces multiplike el EntrysCounter para que me entren?

    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // Declaraciones para maximas conexiones por usuario
    // Agregado por EL OSO
    private static int[] MaxConTables;

    private static int MaxConTablesEntry; // puntero a la ultima insertada

    public static void InitIpTables(int OptCountersValue)
    {
        // *************************************************  *************
        // Author: Lucio N. Tourrilhes (DuNga)
        // Last Modify Date: EL OSO 21/01/06. Soporte para MaxConTables
        // 
        // *************************************************  *************
        EntrysCounter = OptCountersValue;
        Multiplicado = 1;

        IpTables = new int[EntrysCounter * 2 + 1];
        MaxValue = 0;

        MaxConTables = new int[Declaraciones.MaxUsers * 2];
        MaxConTablesEntry = 0;
    }

    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // '''''''''''''''''''''FUNCIONES PARA INTERVALOS'''''''''''''''''''''''''''''''''
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    public static void IpSecurityMantenimientoLista()
    {
        // *************************************************  *************
        // Author: Lucio N. Tourrilhes (DuNga)
        // Last Modify Date: Unknow
        // 
        // *************************************************  *************
        // Las borro todas cada 1 hora, asi se "renuevan"
        EntrysCounter = EntrysCounter / Multiplicado;
        Multiplicado = 1;
        IpTables = new int[EntrysCounter * 2 + 1];
        MaxValue = 0;
    }

    public static bool IpSecurityAceptarNuevaConexion(int ip)
    {
        bool IpSecurityAceptarNuevaConexionRet = default;
        // *************************************************  *************
        // Author: Lucio N. Tourrilhes (DuNga)
        // Last Modify Date: Unknow
        // 
        // *************************************************  *************
        int IpTableIndex;


        IpTableIndex = FindTableIp(ip, e_SecurityIpTabla.IP_INTERVALOS);

        if (IpTableIndex >= 0)
        {
            if (IpTables[IpTableIndex + 1] + IntervaloEntreConexiones <= Migration.GetTickCount())
            {
                // No está saturando de connects?
                IpTables[IpTableIndex + 1] = Convert.ToInt32(Migration.GetTickCount());
                IpSecurityAceptarNuevaConexionRet = true;
                Console.WriteLine("CONEXION ACEPTADA");
                return IpSecurityAceptarNuevaConexionRet;
            }

            IpSecurityAceptarNuevaConexionRet = false;

            Console.WriteLine("CONEXION NO ACEPTADA");
            return IpSecurityAceptarNuevaConexionRet;
        }

        IpTableIndex = ~IpTableIndex;
        AddNewIpIntervalo(ip, IpTableIndex);
        IpTables[IpTableIndex + 1] = Convert.ToInt32(Migration.GetTickCount());
        IpSecurityAceptarNuevaConexionRet = true;
        return IpSecurityAceptarNuevaConexionRet;
    }


    private static void AddNewIpIntervalo(int ip, int index)
    {
        // *************************************************  *************
        // Author: Lucio N. Tourrilhes (DuNga)
        // Last Modify Date: Unknow
        // Modified to avoid using CopyMemory
        // *************************************************  *************
        int i;

        // 2) Pruebo si hay espacio, sino agrando la lista
        if (MaxValue + 1 > EntrysCounter)
        {
            EntrysCounter = EntrysCounter / Multiplicado;
            Multiplicado = Multiplicado + 1;
            EntrysCounter = EntrysCounter * Multiplicado;

            Array.Resize(ref IpTables, EntrysCounter * 2 + 1);
        }

        // 4) Corro todo el array para arriba usando bucle en lugar de CopyMemory
        var loopTo = index + 1;
        for (i = MaxValue * 2 + 1; i >= loopTo; i -= 1)
            IpTables[i + 1] = IpTables[i];

        var loopTo1 = index;
        for (i = MaxValue * 2; i >= loopTo1; i -= 1)
            IpTables[i + 1] = IpTables[i];

        IpTables[index] = ip;

        // 3) Subo el indicador de el maximo valor almacenado y listo :)
        MaxValue = MaxValue + 1;
    }

    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // ''''''''''''''''''''FUNCIONES PARA LIMITES X IP''''''''''''''''''''''''''''''''
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    public static bool IPSecuritySuperaLimiteConexiones(int ip)
    {
        bool IPSecuritySuperaLimiteConexionesRet = default;
        int IpTableIndex;

        IpTableIndex = FindTableIp(ip, e_SecurityIpTabla.IP_LIMITECONEXIONES);

        if (IpTableIndex >= 0)
        {
            if (MaxConTables[IpTableIndex + 1] < LIMITECONEXIONESxIP)
            {
                General.LogIP("Agregamos conexion a " + ip + " iptableindex=" + IpTableIndex + ". Conexiones: " +
                              MaxConTables[IpTableIndex + 1]);
                Console.WriteLine("suma conexion a " + ip + " total " + (MaxConTables[IpTableIndex + 1] + 1));
                MaxConTables[IpTableIndex + 1] = MaxConTables[IpTableIndex + 1] + 1;
                IPSecuritySuperaLimiteConexionesRet = false;
            }
            else
            {
                General.LogIP("rechazamos conexion de " + ip + " iptableindex=" + IpTableIndex + ". Conexiones: " +
                              MaxConTables[IpTableIndex + 1]);
                Console.WriteLine("rechaza conexion a " + ip);
                IPSecuritySuperaLimiteConexionesRet = true;
            }
        }
        else
        {
            IPSecuritySuperaLimiteConexionesRet = false;
            if (MaxConTablesEntry < Declaraciones.MaxUsers) // si hay espacio..
            {
                IpTableIndex = ~IpTableIndex;
                AddNewIpLimiteConexiones(ip, IpTableIndex); // iptableindex es donde lo agrego
                MaxConTables[IpTableIndex + 1] = 1;
            }
            else
            {
                var argdesc = "SecurityIP.IPSecuritySuperaLimiteConexiones: Se supero la disponibilidad de slots.";
                General.LogCriticEvent(ref argdesc);
            }
        }

        return IPSecuritySuperaLimiteConexionesRet;
    }

    private static void AddNewIpLimiteConexiones(int ip, int index)
    {
        // *************************************************    *************
        // Author: (EL OSO)
        // Last Modify Date: 16/2/2006
        // Modified by Juan Martín Sotuyo Dodero (Maraxus)
        // Modified to avoid using CopyMemory
        // *************************************************    *************
        Console.WriteLine("agrega conexion a " + ip);
        Console.WriteLine("(Declaraciones.MaxUsers - index) = " + (Declaraciones.MaxUsers - index));
        Console.WriteLine("Agrega conexion a nueva IP " + ip);

        int i;
        int elementCount;

        elementCount = MaxConTablesEntry - index / 2;

        // Desplazar todos los elementos para hacer espacio para la nueva entrada
        var loopTo = index + 1;
        for (i = MaxConTablesEntry * 2 - 1; i >= loopTo; i -= 1)
            MaxConTables[i + 1] = MaxConTables[i];

        var loopTo1 = index;
        for (i = MaxConTablesEntry * 2 - 2; i >= loopTo1; i -= 1)
            MaxConTables[i + 1] = MaxConTables[i];

        MaxConTables[index] = ip;

        // 3) Subo el indicador de el maximo valor almacenado y listo :)
        MaxConTablesEntry = MaxConTablesEntry + 1;
    }

    public static void IpRestarConexion(int ip)
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // Modified to avoid using CopyMemory
        // ***************************************************

        int key;
        int i;
        Console.WriteLine("resta conexion a " + ip);

        key = FindTableIp(ip, e_SecurityIpTabla.IP_LIMITECONEXIONES);

        if (key >= 0)
        {
            if (MaxConTables[key + 1] > 0) MaxConTables[key + 1] = MaxConTables[key + 1] - 1;
            General.LogIP("restamos conexion a " + ip + " key=" + key + ". Conexiones: " + MaxConTables[key + 1]);
            if (MaxConTables[key + 1] <= 0)
            {
                // la limpiamos - desplazar todos los elementos a la izquierda
                var loopTo = MaxConTablesEntry * 2 - 3;
                for (i = key; i <= loopTo; i += 2)
                {
                    MaxConTables[i] = MaxConTables[i + 2];
                    MaxConTables[i + 1] = MaxConTables[i + 3];
                }

                MaxConTablesEntry = MaxConTablesEntry - 1;
            }
        }
        else // Key <= 0
        {
            General.LogIP("restamos conexion a " + ip + " key=" + key + ". NEGATIVO!!");
            // LogCriticEvent "SecurityIp.IpRestarconexion obtuvo un valor negativo en key"
        }
    }


    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // ''''''''''''''''''''''''FUNCIONES GENERALES''''''''''''''''''''''''''''''''''''
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    // '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


    private static int FindTableIp(int ip, e_SecurityIpTabla Tabla)
    {
        int FindTableIpRet = default;
        // *************************************************  *************
        // Author: Lucio N. Tourrilhes (DuNga)
        // Last Modify Date: Unknow
        // Modified by Juan Martín Sotuyo Dodero (Maraxus) to use Binary Insertion
        // *************************************************  *************
        int First;
        int Last;
        var Middle = default(int);

        switch (Tabla)
        {
            case e_SecurityIpTabla.IP_INTERVALOS:
            {
                First = 0;
                Last = MaxValue;
                while (First <= Last)
                {
                    Middle = (First + Last) / 2;

                    if (IpTables[Middle * 2] < ip)
                    {
                        First = Middle + 1;
                    }
                    else if (IpTables[Middle * 2] > ip)
                    {
                        Last = Middle - 1;
                    }
                    else
                    {
                        FindTableIpRet = Middle * 2;
                        return FindTableIpRet;
                    }
                }

                FindTableIpRet = ~(Middle * 2);
                break;
            }

            case e_SecurityIpTabla.IP_LIMITECONEXIONES:
            {
                First = 0;
                Last = MaxConTablesEntry;

                while (First <= Last)
                {
                    Middle = (First + Last) / 2;

                    if (MaxConTables[Middle * 2] < ip)
                    {
                        First = Middle + 1;
                    }
                    else if (MaxConTables[Middle * 2] > ip)
                    {
                        Last = Middle - 1;
                    }
                    else
                    {
                        FindTableIpRet = Middle * 2;
                        return FindTableIpRet;
                    }
                }

                FindTableIpRet = ~(Middle * 2);
                break;
            }
        }

        return FindTableIpRet;
    }

    public static void DumpTables()
    {
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int i;

        var loopTo = MaxConTablesEntry * 2 - 1;
        for (i = 0; i <= loopTo; i += 2)
        {
            var argdesc = wskapiAO.GetAscIP(MaxConTables[i]) + " > " + MaxConTables[i + 1];
            General.LogCriticEvent(ref argdesc);
        }
    }

    private enum e_SecurityIpTabla
    {
        IP_INTERVALOS = 1,
        IP_LIMITECONEXIONES = 2
    }
}