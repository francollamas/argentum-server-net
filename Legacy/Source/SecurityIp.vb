Option Strict Off
Option Explicit On
Module SecurityIp
    ' TODO MIGRA: se modifico para NO usar CopyMemory. Quizas tenga algun problema de performance. Revisar!

    '**************************************************************
    ' General_IpSecurity.Bas - Maneja la seguridad de las IPs
    '
    ' Escrito y diseñado por DuNga (ltourrilhes@gmail.com)
    '**************************************************************

    '*************************************************  *************
    ' General_IpSecurity.Bas - Maneja la seguridad de las IPs
    '
    ' Escrito y diseñado por DuNga (ltourrilhes@gmail.com)
    '*************************************************  *************

    Private IpTables() As Integer 'USAMOS 2 LONGS: UNO DE LA IP, SEGUIDO DE UNO DE LA INFO
    Private EntrysCounter As Integer
    Private MaxValue As Integer
    Private Multiplicado As Integer 'Cuantas veces multiplike el EntrysCounter para que me entren?
    Private Const IntervaloEntreConexiones As Integer = 1000

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Declaraciones para maximas conexiones por usuario
    'Agregado por EL OSO
        Private MaxConTables() As Integer

    Private MaxConTablesEntry As Integer 'puntero a la ultima insertada

    Private Const LIMITECONEXIONESxIP As Integer = 10

    Private Enum e_SecurityIpTabla
        IP_INTERVALOS = 1
        IP_LIMITECONEXIONES = 2
    End Enum

    Public Sub InitIpTables(OptCountersValue As Integer)
        '*************************************************  *************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: EL OSO 21/01/06. Soporte para MaxConTables
        '
        '*************************************************  *************
        EntrysCounter = OptCountersValue
        Multiplicado = 1

        ReDim IpTables(EntrysCounter*2)
        MaxValue = 0

        ReDim MaxConTables(MaxUsers*2 - 1)
        MaxConTablesEntry = 0
    End Sub

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''FUNCIONES PARA INTERVALOS'''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Public Sub IpSecurityMantenimientoLista()
        '*************************************************  *************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: Unknow
        '
        '*************************************************  *************
        'Las borro todas cada 1 hora, asi se "renuevan"
        EntrysCounter = EntrysCounter\Multiplicado
        Multiplicado = 1
        ReDim IpTables(EntrysCounter*2)
        MaxValue = 0
    End Sub

    Public Function IpSecurityAceptarNuevaConexion(ip As Integer) As Boolean
        '*************************************************  *************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: Unknow
        '
        '*************************************************  *************
        Dim IpTableIndex As Integer


        IpTableIndex = FindTableIp(ip, e_SecurityIpTabla.IP_INTERVALOS)

        If IpTableIndex >= 0 Then
            If IpTables(IpTableIndex + 1) + IntervaloEntreConexiones <= GetTickCount Then _
'No está saturando de connects?
                IpTables(IpTableIndex + 1) = GetTickCount
                IpSecurityAceptarNuevaConexion = True
                Console.WriteLine("CONEXION ACEPTADA")
                Exit Function
            Else
                IpSecurityAceptarNuevaConexion = False

                Console.WriteLine("CONEXION NO ACEPTADA")
                Exit Function
            End If
        Else
            IpTableIndex = Not IpTableIndex
            AddNewIpIntervalo(ip, IpTableIndex)
            IpTables(IpTableIndex + 1) = GetTickCount
            IpSecurityAceptarNuevaConexion = True
            Exit Function
        End If
    End Function


    Private Sub AddNewIpIntervalo(ip As Integer, index As Integer)
        '*************************************************  *************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: Unknow
        'Modified to avoid using CopyMemory
        '*************************************************  *************
        Dim i As Integer

        '2) Pruebo si hay espacio, sino agrando la lista
        If MaxValue + 1 > EntrysCounter Then
            EntrysCounter = EntrysCounter\Multiplicado
            Multiplicado = Multiplicado + 1
            EntrysCounter = EntrysCounter*Multiplicado

            ReDim Preserve IpTables(EntrysCounter*2)
        End If

        '4) Corro todo el array para arriba usando bucle en lugar de CopyMemory
        For i = (MaxValue*2) + 1 To index + 1 Step - 1
            IpTables(i + 1) = IpTables(i)
        Next i

        For i = (MaxValue*2) To index Step - 1
            IpTables(i + 1) = IpTables(i)
        Next i

        IpTables(index) = ip

        '3) Subo el indicador de el maximo valor almacenado y listo :)
        MaxValue = MaxValue + 1
    End Sub

    ' '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' ''''''''''''''''''''FUNCIONES PARA LIMITES X IP''''''''''''''''''''''''''''''''
    ' '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    Public Function IPSecuritySuperaLimiteConexiones(ip As Integer) As Boolean
        Dim IpTableIndex As Integer

        IpTableIndex = FindTableIp(ip, e_SecurityIpTabla.IP_LIMITECONEXIONES)

        If IpTableIndex >= 0 Then

            If MaxConTables(IpTableIndex + 1) < LIMITECONEXIONESxIP Then
                LogIP(
                    ("Agregamos conexion a " & ip & " iptableindex=" & IpTableIndex & ". Conexiones: " &
                     MaxConTables(IpTableIndex + 1)))
                Console.WriteLine("suma conexion a " & ip & " total " & MaxConTables(IpTableIndex + 1) + 1)
                MaxConTables(IpTableIndex + 1) = MaxConTables(IpTableIndex + 1) + 1
                IPSecuritySuperaLimiteConexiones = False
            Else
                LogIP(
                    ("rechazamos conexion de " & ip & " iptableindex=" & IpTableIndex & ". Conexiones: " &
                     MaxConTables(IpTableIndex + 1)))
                Console.WriteLine("rechaza conexion a " & ip)
                IPSecuritySuperaLimiteConexiones = True
            End If
        Else
            IPSecuritySuperaLimiteConexiones = False
            If MaxConTablesEntry < MaxUsers Then 'si hay espacio..
                IpTableIndex = Not IpTableIndex
                AddNewIpLimiteConexiones(ip, IpTableIndex) 'iptableindex es donde lo agrego
                MaxConTables(IpTableIndex + 1) = 1
            Else
                Call _
                    LogCriticEvent("SecurityIP.IPSecuritySuperaLimiteConexiones: Se supero la disponibilidad de slots.")
            End If
        End If
    End Function

    Private Sub AddNewIpLimiteConexiones(ip As Integer, index As Integer)
        '*************************************************    *************
        'Author: (EL OSO)
        'Last Modify Date: 16/2/2006
        'Modified by Juan Martín Sotuyo Dodero (Maraxus)
        'Modified to avoid using CopyMemory
        '*************************************************    *************
        Console.WriteLine("agrega conexion a " & ip)
        Console.WriteLine("(Declaraciones.MaxUsers - index) = " & (MaxUsers - index))
        Console.WriteLine("Agrega conexion a nueva IP " & ip)

        Dim i As Integer
        Dim elementCount As Integer

        elementCount = MaxConTablesEntry - index\2

        ' Desplazar todos los elementos para hacer espacio para la nueva entrada
        For i = MaxConTablesEntry*2 - 1 To index + 1 Step - 1
            MaxConTables(i + 1) = MaxConTables(i)
        Next i

        For i = MaxConTablesEntry*2 - 2 To index Step - 1
            MaxConTables(i + 1) = MaxConTables(i)
        Next i

        MaxConTables(index) = ip

        '3) Subo el indicador de el maximo valor almacenado y listo :)
        MaxConTablesEntry = MaxConTablesEntry + 1
    End Sub

    Public Sub IpRestarConexion(ip As Integer)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        'Modified to avoid using CopyMemory
        '***************************************************

        Dim key As Integer
        Dim i As Integer
        Console.WriteLine("resta conexion a " & ip)

        key = FindTableIp(ip, e_SecurityIpTabla.IP_LIMITECONEXIONES)

        If key >= 0 Then
            If MaxConTables(key + 1) > 0 Then
                MaxConTables(key + 1) = MaxConTables(key + 1) - 1
            End If
            Call LogIP("restamos conexion a " & ip & " key=" & key & ". Conexiones: " & MaxConTables(key + 1))
            If MaxConTables(key + 1) <= 0 Then
                'la limpiamos - desplazar todos los elementos a la izquierda
                For i = key To (MaxConTablesEntry*2) - 3 Step 2
                    MaxConTables(i) = MaxConTables(i + 2)
                    MaxConTables(i + 1) = MaxConTables(i + 3)
                Next i
                MaxConTablesEntry = MaxConTablesEntry - 1
            End If
        Else 'Key <= 0
            Call LogIP("restamos conexion a " & ip & " key=" & key & ". NEGATIVO!!")
            'LogCriticEvent "SecurityIp.IpRestarconexion obtuvo un valor negativo en key"
        End If
    End Sub


    ' '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' ''''''''''''''''''''''''FUNCIONES GENERALES''''''''''''''''''''''''''''''''''''
    ' '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


    Private Function FindTableIp(ip As Integer, Tabla As e_SecurityIpTabla) As Integer
        '*************************************************  *************
        'Author: Lucio N. Tourrilhes (DuNga)
        'Last Modify Date: Unknow
        'Modified by Juan Martín Sotuyo Dodero (Maraxus) to use Binary Insertion
        '*************************************************  *************
        Dim First As Integer
        Dim Last As Integer
        Dim Middle As Integer

        Select Case Tabla
            Case e_SecurityIpTabla.IP_INTERVALOS
                First = 0
                Last = MaxValue
                Do While First <= Last
                    Middle = (First + Last)\2

                    If (IpTables(Middle*2) < ip) Then
                        First = Middle + 1
                    ElseIf (IpTables(Middle*2) > ip) Then
                        Last = Middle - 1
                    Else
                        FindTableIp = Middle*2
                        Exit Function
                    End If
                Loop
                FindTableIp = Not (Middle*2)

            Case e_SecurityIpTabla.IP_LIMITECONEXIONES

                First = 0
                Last = MaxConTablesEntry

                Do While First <= Last
                    Middle = (First + Last)\2

                    If MaxConTables(Middle*2) < ip Then
                        First = Middle + 1
                    ElseIf MaxConTables(Middle*2) > ip Then
                        Last = Middle - 1
                    Else
                        FindTableIp = Middle*2
                        Exit Function
                    End If
                Loop
                FindTableIp = Not (Middle*2)
        End Select
    End Function

    Public Function DumpTables() As Object
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim i As Short

        For i = 0 To MaxConTablesEntry*2 - 1 Step 2
            Call LogCriticEvent(GetAscIP(MaxConTables(i)) & " > " & MaxConTables(i + 1))
        Next i
    End Function
End Module
