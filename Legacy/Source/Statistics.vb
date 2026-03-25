Option Strict On
Option Explicit On
Module Statistics
    '**************************************************************
    ' modStatistics.bas - Takes statistics on the game for later study.
    '
    ' Implemented by Juan Martín Sotuyo Dodero (Maraxus)
    ' (juansotuyo@gmail.com)
    '**************************************************************

    Private Structure trainningData
        Dim startTick As Integer
        Dim trainningTime As Integer
    End Structure

    Public Structure fragLvlRace
        <VBFixedArray(50, 5)> Dim matrix(,) As Integer

        'UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        Public Sub Initialize()
            'UPGRADE_WARNING: El límite inferior de la matriz matrix ha cambiado de 1,1 a 0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim matrix(50, 5)
        End Sub
    End Structure

    Public Structure fragLvlLvl
        <VBFixedArray(50, 50)> Dim matrix(,) As Integer

        'UPGRADE_TODO: Se debe llamar a "Initialize" para inicializar instancias de esta estructura. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B4BFF9E0-8631-45CF-910E-62AB3970F27B"'
        Public Sub Initialize()
            'UPGRADE_WARNING: El límite inferior de la matriz matrix ha cambiado de 1,1 a 0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim matrix(50, 50)
        End Sub
    End Structure

    Private trainningInfo() As trainningData

    'UPGRADE_WARNING: El límite inferior de la matriz fragLvlRaceData ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    'UPGRADE_WARNING: Es posible que la matriz fragLvlRaceData necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
    Public fragLvlRaceData(7) As fragLvlRace
    'UPGRADE_WARNING: El límite inferior de la matriz fragLvlLvlData ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    'UPGRADE_WARNING: Es posible que la matriz fragLvlLvlData necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
    Public fragLvlLvlData(7) As fragLvlLvl
    'UPGRADE_WARNING: El límite inferior de la matriz fragAlignmentLvlData ha cambiado de 1,1 a 0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
    Private ReadOnly fragAlignmentLvlData(50, 4) As Integer

    'Currency just in case.... chats are way TOO often...
    Private ReadOnly keyOcurrencies(255) As Decimal

    Public Sub Initialize()
        'UPGRADE_WARNING: El límite inferior de la matriz trainningInfo ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim trainningInfo(MaxUsers)
    End Sub

    Public Sub UserConnected(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'A new user connected, load it's trainning time count
        trainningInfo(UserIndex).trainningTime = Convert.ToInt32(ParseVal(GetVar(CharPath & UserList(UserIndex).name.ToUpper() & ".chr",
                                                            "RESEARCH", "TrainningTime", 30)))

        trainningInfo(UserIndex).startTick = Convert.ToInt32(GetTickCount())
    End Sub

    Public Sub UserDisconnected(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        With trainningInfo(UserIndex)
            'Update trainning time
            .trainningTime = Convert.ToInt32(.trainningTime + (GetTickCount() - .startTick)/1000)

            .startTick = Convert.ToInt32(GetTickCount())

            'Store info in char file
            Call _
                WriteVar(CharPath & UserList(UserIndex).name.ToUpper() & ".chr", "RESEARCH", "TrainningTime",
                         .trainningTime.ToString())
        End With
    End Sub

    Public Sub UserLevelUp(UserIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        With trainningInfo(UserIndex)
            'Log the data
            AppendLog("logs/statistics.log",
                      UserList(UserIndex).name.ToUpper() & " completó el nivel " & UserList(UserIndex).Stats.ELV.ToString() &
                      " en " & (.trainningTime + (GetTickCount() - .startTick)/1000).ToString() & " segundos.")

            'Reset data
            .trainningTime = 0
            .startTick = Convert.ToInt32(GetTickCount())
        End With
    End Sub

    Public Sub StoreFrag(killer As Short, victim As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim clase As Short
        Dim raza As Short
        Dim alignment As Short

        If UserList(victim).Stats.ELV > 50 Or UserList(killer).Stats.ELV > 50 Then Exit Sub

        Select Case UserList(killer).clase
            Case eClass.Assasin
                clase = 1

            Case eClass.Bard
                clase = 2

            Case eClass.Mage
                clase = 3

            Case eClass.Paladin
                clase = 4

            Case eClass.Warrior
                clase = 5

            Case eClass.Cleric
                clase = 6

            Case eClass.Hunter
                clase = 7

            Case Else
                Exit Sub
        End Select

        Select Case UserList(killer).raza
            Case eRaza.Elfo
                raza = 1

            Case eRaza.Drow
                raza = 2

            Case eRaza.Enano
                raza = 3

            Case eRaza.Gnomo
                raza = 4

            Case eRaza.Humano
                raza = 5

            Case Else
                Exit Sub
        End Select

        If UserList(killer).Faccion.ArmadaReal <> 0 Then
            alignment = 1
        ElseIf UserList(killer).Faccion.FuerzasCaos <> 0 Then
            alignment = 2
        ElseIf criminal(killer) Then
            alignment = 3
        Else
            alignment = 4
        End If

        fragLvlRaceData(clase).matrix(UserList(killer).Stats.ELV, raza) =
            fragLvlRaceData(clase).matrix(UserList(killer).Stats.ELV, raza) + 1

        fragLvlLvlData(clase).matrix(UserList(killer).Stats.ELV, UserList(victim).Stats.ELV) =
            fragLvlLvlData(clase).matrix(UserList(killer).Stats.ELV, UserList(victim).Stats.ELV) + 1

        fragAlignmentLvlData(UserList(killer).Stats.ELV, alignment) =
            fragAlignmentLvlData(UserList(killer).Stats.ELV, alignment) + 1
    End Sub

    Public Sub DumpStatistics()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim line As String
        Dim i As Integer
        Dim j As Integer

        Using writer As New IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory & "logs/frags.txt")
            'Save lvl vs lvl frag matrix for each class - we use GNU Octave's ASCII file format

            writer.WriteLine("# name: fragLvlLvl_Ase")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 50")
            writer.WriteLine("# columns: 50")

            For j = 1 To 50
                For i = 1 To 50
                    line = line & " " & fragLvlLvlData(1).matrix(i, j).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlLvl_Bar")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 50")
            writer.WriteLine("# columns: 50")

            For j = 1 To 50
                For i = 1 To 50
                    line = line & " " & fragLvlLvlData(2).matrix(i, j).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlLvl_Mag")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 50")
            writer.WriteLine("# columns: 50")

            For j = 1 To 50
                For i = 1 To 50
                    line = line & " " & fragLvlLvlData(3).matrix(i, j).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlLvl_Pal")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 50")
            writer.WriteLine("# columns: 50")

            For j = 1 To 50
                For i = 1 To 50
                    line = line & " " & fragLvlLvlData(4).matrix(i, j).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlLvl_Gue")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 50")
            writer.WriteLine("# columns: 50")

            For j = 1 To 50
                For i = 1 To 50
                    line = line & " " & fragLvlLvlData(5).matrix(i, j).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlLvl_Cle")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 50")
            writer.WriteLine("# columns: 50")

            For j = 1 To 50
                For i = 1 To 50
                    line = line & " " & fragLvlLvlData(6).matrix(i, j).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlLvl_Caz")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 50")
            writer.WriteLine("# columns: 50")

            For j = 1 To 50
                For i = 1 To 50
                    line = line & " " & fragLvlLvlData(7).matrix(i, j).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j


            'Save lvl vs race frag matrix for each class - we use GNU Octave's ASCII file format

            writer.WriteLine("# name: fragLvlRace_Ase")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 5")
            writer.WriteLine("# columns: 50")

            For j = 1 To 5
                For i = 1 To 50
                    line = line & " " & fragLvlRaceData(1).matrix(i, j).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlRace_Bar")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 5")
            writer.WriteLine("# columns: 50")

            For j = 1 To 5
                For i = 1 To 50
                    line = line & " " & fragLvlRaceData(2).matrix(i, j).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlRace_Mag")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 5")
            writer.WriteLine("# columns: 50")

            For j = 1 To 5
                For i = 1 To 50
                    line = line & " " & fragLvlRaceData(3).matrix(i, j).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlRace_Pal")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 5")
            writer.WriteLine("# columns: 50")

            For j = 1 To 5
                For i = 1 To 50
                    line = line & " " & fragLvlRaceData(4).matrix(i, j).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlRace_Gue")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 5")
            writer.WriteLine("# columns: 50")

            For j = 1 To 5
                For i = 1 To 50
                    line = line & " " & fragLvlRaceData(5).matrix(i, j).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlRace_Cle")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 5")
            writer.WriteLine("# columns: 50")

            For j = 1 To 5
                For i = 1 To 50
                    line = line & " " & fragLvlRaceData(6).matrix(i, j).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlRace_Caz")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 5")
            writer.WriteLine("# columns: 50")

            For j = 1 To 5
                For i = 1 To 50
                    line = line & " " & fragLvlRaceData(7).matrix(i, j).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j


            'Save lvl vs class frag matrix for each race - we use GNU Octave's ASCII file format

            writer.WriteLine("# name: fragLvlClass_Elf")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 7")
            writer.WriteLine("# columns: 50")

            For j = 1 To 7
                For i = 1 To 50
                    line = line & " " & fragLvlRaceData(j).matrix(i, 1).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlClass_Dar")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 7")
            writer.WriteLine("# columns: 50")

            For j = 1 To 7
                For i = 1 To 50
                    line = line & " " & fragLvlRaceData(j).matrix(i, 2).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlClass_Dwa")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 7")
            writer.WriteLine("# columns: 50")

            For j = 1 To 7
                For i = 1 To 50
                    line = line & " " & fragLvlRaceData(j).matrix(i, 3).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlClass_Gno")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 7")
            writer.WriteLine("# columns: 50")

            For j = 1 To 7
                For i = 1 To 50
                    line = line & " " & fragLvlRaceData(j).matrix(i, 4).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j

            writer.WriteLine("# name: fragLvlClass_Hum")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 7")
            writer.WriteLine("# columns: 50")

            For j = 1 To 7
                For i = 1 To 50
                    line = line & " " & fragLvlRaceData(j).matrix(i, 5).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j


            'Save lvl vs alignment frag matrix for each race - we use GNU Octave's ASCII file format

            writer.WriteLine("# name: fragAlignmentLvl")
            writer.WriteLine("# type: matrix")
            writer.WriteLine("# rows: 4")
            writer.WriteLine("# columns: 50")

            For j = 1 To 4
                For i = 1 To 50
                    line = line & " " & fragAlignmentLvlData(i, j).ToString()
                Next i

                writer.WriteLine(line)
                line = vbNullString
            Next j
        End Using


        'Dump Chat statistics
        Using writer As New IO.StreamWriter(AppDomain.CurrentDomain.BaseDirectory & "logs/huffman.log")
            Dim Total As Decimal

            'Compute total characters
            For i = 0 To 255
                Total = Total + keyOcurrencies(i)
            Next i

            'Show each character's ocurrencies
            If Total <> 0 Then
                For i = 0 To 255
                    writer.WriteLine(i.ToString() & "    " & Math.Round(keyOcurrencies(i)/Total, 8).ToString())
                Next i
            End If

            writer.WriteLine("TOTAL =    " & Total.ToString())
        End Using
    End Sub

    Public Sub ParseChat(ByRef S As String)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim i As Integer
        Dim key As Short

        For i = 1 To S.Length
            key = Convert.ToInt16(Asc(S.Substring(i - 1, 1)))

            keyOcurrencies(key) = keyOcurrencies(key) + 1
        Next i

        'Add a NULL-terminated to consider that possibility too....
        keyOcurrencies(0) = keyOcurrencies(0) + 1
    End Sub
End Module
