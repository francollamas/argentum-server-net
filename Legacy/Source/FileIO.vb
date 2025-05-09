Option Strict Off
Option Explicit On

Imports System.IO

Module ES
    Public Sub CargarSpawnList()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim N, LoopC As Short
        N = Val(GetVar(AppDomain.CurrentDomain.BaseDirectory & "Dat/Invokar.dat", "INIT", "NumNPCs"))
        ReDim SpawnList(N)
        For LoopC = 1 To N
            SpawnList(LoopC).NpcIndex = Val(GetVar(AppDomain.CurrentDomain.BaseDirectory & "Dat/Invokar.dat", "LIST",
                                                   "NI" & LoopC))
            SpawnList(LoopC).NpcName = GetVar(AppDomain.CurrentDomain.BaseDirectory & "Dat/Invokar.dat", "LIST",
                                              "NN" & LoopC)
        Next LoopC
    End Sub

    Function EsAdmin(name As String) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim NumWizs As Short
        Dim WizNum As Short
        Dim NomB As String

        NumWizs = Val(GetVar(IniPath & "Server.ini", "INIT", "Admines"))

        For WizNum = 1 To NumWizs
            NomB = UCase(GetVar(IniPath & "Server.ini", "Admines", "Admin" & WizNum))

            If Left(NomB, 1) = "*" Or Left(NomB, 1) = "+" Then NomB = Right(NomB, Len(NomB) - 1)
            If UCase(name) = NomB Then
                EsAdmin = True
                Exit Function
            End If
        Next WizNum
        EsAdmin = False
    End Function

    Function EsDios(name As String) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim NumWizs As Short
        Dim WizNum As Short
        Dim NomB As String

        NumWizs = Val(GetVar(IniPath & "Server.ini", "INIT", "Dioses"))
        For WizNum = 1 To NumWizs
            NomB = UCase(GetVar(IniPath & "Server.ini", "Dioses", "Dios" & WizNum))

            If Left(NomB, 1) = "*" Or Left(NomB, 1) = "+" Then NomB = Right(NomB, Len(NomB) - 1)
            If UCase(name) = NomB Then
                EsDios = True
                Exit Function
            End If
        Next WizNum
        EsDios = False
    End Function

    Function EsSemiDios(name As String) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim NumWizs As Short
        Dim WizNum As Short
        Dim NomB As String

        NumWizs = Val(GetVar(IniPath & "Server.ini", "INIT", "SemiDioses"))
        For WizNum = 1 To NumWizs
            NomB = UCase(GetVar(IniPath & "Server.ini", "SemiDioses", "SemiDios" & WizNum))

            If Left(NomB, 1) = "*" Or Left(NomB, 1) = "+" Then NomB = Right(NomB, Len(NomB) - 1)
            If UCase(name) = NomB Then
                EsSemiDios = True
                Exit Function
            End If
        Next WizNum
        EsSemiDios = False
    End Function

    Function EsConsejero(name As String) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim NumWizs As Short
        Dim WizNum As Short
        Dim NomB As String

        NumWizs = Val(GetVar(IniPath & "Server.ini", "INIT", "Consejeros"))
        For WizNum = 1 To NumWizs
            NomB = UCase(GetVar(IniPath & "Server.ini", "Consejeros", "Consejero" & WizNum))

            If Left(NomB, 1) = "*" Or Left(NomB, 1) = "+" Then NomB = Right(NomB, Len(NomB) - 1)
            If UCase(name) = NomB Then
                EsConsejero = True
                Exit Function
            End If
        Next WizNum
        EsConsejero = False
    End Function

    Function EsRolesMaster(name As String) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim NumWizs As Short
        Dim WizNum As Short
        Dim NomB As String

        NumWizs = Val(GetVar(IniPath & "Server.ini", "INIT", "RolesMasters"))
        For WizNum = 1 To NumWizs
            NomB = UCase(GetVar(IniPath & "Server.ini", "RolesMasters", "RM" & WizNum))

            If Left(NomB, 1) = "*" Or Left(NomB, 1) = "+" Then NomB = Right(NomB, Len(NomB) - 1)
            If UCase(name) = NomB Then
                EsRolesMaster = True
                Exit Function
            End If
        Next WizNum
        EsRolesMaster = False
    End Function


    Public Function TxtDimension(name As String) As Integer
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim N As Short
        Dim cad As String
        Dim Tam As Integer
        N = FreeFile()
        FileOpen(N, name, OpenMode.Input)
        Tam = 0
        Do While Not EOF(N)
            Tam = Tam + 1
            cad = LineInput(N)
        Loop
        FileClose(N)
        TxtDimension = Tam
    End Function

    Public Sub CargarForbidenWords()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'UPGRADE_WARNING: El límite inferior de la matriz ForbidenNames ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim ForbidenNames(TxtDimension(DatPath & "NombresInvalidos.txt"))
        Dim N, i As Short
        N = FreeFile()
        FileOpen(N, DatPath & "NombresInvalidos.txt", OpenMode.Input)

        For i = 1 To UBound(ForbidenNames)
            ForbidenNames(i) = LineInput(N)
        Next i

        FileClose(N)
    End Sub

    Public Sub CargarHechizos()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        '###################################################
        '#               ATENCION PELIGRO                  #
        '###################################################
        '
        '  ¡¡¡¡ NO USAR GetVar PARA LEER Hechizos.dat !!!!
        '
        'El que ose desafiar esta LEY, se las tendrá que ver
        'con migo. Para leer Hechizos.dat se deberá usar
        'la nueva clase clsLeerInis.
        '
        'Alejo
        '
        '###################################################

        Try

            Dim Hechizo As Short
            Dim Leer As New clsIniReader

            Call Leer.Initialize(DatPath & "Hechizos.dat")

            'obtiene el numero de hechizos
            NumeroHechizos = Val(Leer.GetValue("INIT", "NumeroHechizos"))

            'UPGRADE_WARNING: El límite inferior de la matriz Hechizos ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim Hechizos(NumeroHechizos)

            'Llena la lista
            For Hechizo = 1 To NumeroHechizos
                With Hechizos(Hechizo)
                    .Nombre = Leer.GetValue("Hechizo" & Hechizo, "Nombre")
                    .desc = Leer.GetValue("Hechizo" & Hechizo, "Desc")
                    .PalabrasMagicas = Leer.GetValue("Hechizo" & Hechizo, "PalabrasMagicas")

                    .HechizeroMsg = Leer.GetValue("Hechizo" & Hechizo, "HechizeroMsg")
                    .TargetMsg = Leer.GetValue("Hechizo" & Hechizo, "TargetMsg")
                    .PropioMsg = Leer.GetValue("Hechizo" & Hechizo, "PropioMsg")

                    .Tipo = Val(Leer.GetValue("Hechizo" & Hechizo, "Tipo"))
                    .WAV = Val(Leer.GetValue("Hechizo" & Hechizo, "WAV"))
                    .FXgrh = Val(Leer.GetValue("Hechizo" & Hechizo, "Fxgrh"))

                    .loops = Val(Leer.GetValue("Hechizo" & Hechizo, "Loops"))

                    '    .Resis = val(Leer.GetValue("Hechizo" & Hechizo, "Resis"))

                    .SubeHP = Val(Leer.GetValue("Hechizo" & Hechizo, "SubeHP"))
                    .MinHp = Val(Leer.GetValue("Hechizo" & Hechizo, "MinHP"))
                    .MaxHp = Val(Leer.GetValue("Hechizo" & Hechizo, "MaxHP"))

                    .SubeMana = Val(Leer.GetValue("Hechizo" & Hechizo, "SubeMana"))
                    .MiMana = Val(Leer.GetValue("Hechizo" & Hechizo, "MinMana"))
                    .MaMana = Val(Leer.GetValue("Hechizo" & Hechizo, "MaxMana"))

                    .SubeSta = Val(Leer.GetValue("Hechizo" & Hechizo, "SubeSta"))
                    .MinSta = Val(Leer.GetValue("Hechizo" & Hechizo, "MinSta"))
                    .MaxSta = Val(Leer.GetValue("Hechizo" & Hechizo, "MaxSta"))

                    .SubeHam = Val(Leer.GetValue("Hechizo" & Hechizo, "SubeHam"))
                    .MinHam = Val(Leer.GetValue("Hechizo" & Hechizo, "MinHam"))
                    .MaxHam = Val(Leer.GetValue("Hechizo" & Hechizo, "MaxHam"))

                    .SubeSed = Val(Leer.GetValue("Hechizo" & Hechizo, "SubeSed"))
                    .MinSed = Val(Leer.GetValue("Hechizo" & Hechizo, "MinSed"))
                    .MaxSed = Val(Leer.GetValue("Hechizo" & Hechizo, "MaxSed"))

                    .SubeAgilidad = Val(Leer.GetValue("Hechizo" & Hechizo, "SubeAG"))
                    .MinAgilidad = Val(Leer.GetValue("Hechizo" & Hechizo, "MinAG"))
                    .MaxAgilidad = Val(Leer.GetValue("Hechizo" & Hechizo, "MaxAG"))

                    .SubeFuerza = Val(Leer.GetValue("Hechizo" & Hechizo, "SubeFU"))
                    .MinFuerza = Val(Leer.GetValue("Hechizo" & Hechizo, "MinFU"))
                    .MaxFuerza = Val(Leer.GetValue("Hechizo" & Hechizo, "MaxFU"))

                    .SubeCarisma = Val(Leer.GetValue("Hechizo" & Hechizo, "SubeCA"))
                    .MinCarisma = Val(Leer.GetValue("Hechizo" & Hechizo, "MinCA"))
                    .MaxCarisma = Val(Leer.GetValue("Hechizo" & Hechizo, "MaxCA"))


                    .Invisibilidad = Val(Leer.GetValue("Hechizo" & Hechizo, "Invisibilidad"))
                    .Paraliza = Val(Leer.GetValue("Hechizo" & Hechizo, "Paraliza"))
                    .Inmoviliza = Val(Leer.GetValue("Hechizo" & Hechizo, "Inmoviliza"))
                    .RemoverParalisis = Val(Leer.GetValue("Hechizo" & Hechizo, "RemoverParalisis"))
                    .RemoverEstupidez = Val(Leer.GetValue("Hechizo" & Hechizo, "RemoverEstupidez"))
                    .RemueveInvisibilidadParcial = Val(Leer.GetValue("Hechizo" & Hechizo, "RemueveInvisibilidadParcial"))


                    .CuraVeneno = Val(Leer.GetValue("Hechizo" & Hechizo, "CuraVeneno"))
                    .Envenena = Val(Leer.GetValue("Hechizo" & Hechizo, "Envenena"))
                    .Maldicion = Val(Leer.GetValue("Hechizo" & Hechizo, "Maldicion"))
                    .RemoverMaldicion = Val(Leer.GetValue("Hechizo" & Hechizo, "RemoverMaldicion"))
                    .Bendicion = Val(Leer.GetValue("Hechizo" & Hechizo, "Bendicion"))
                    .Revivir = Val(Leer.GetValue("Hechizo" & Hechizo, "Revivir"))

                    .Ceguera = Val(Leer.GetValue("Hechizo" & Hechizo, "Ceguera"))
                    .Estupidez = Val(Leer.GetValue("Hechizo" & Hechizo, "Estupidez"))

                    .Warp = Val(Leer.GetValue("Hechizo" & Hechizo, "Warp"))

                    .Invoca = Val(Leer.GetValue("Hechizo" & Hechizo, "Invoca"))
                    .NumNpc = Val(Leer.GetValue("Hechizo" & Hechizo, "NumNpc"))
                    .cant = Val(Leer.GetValue("Hechizo" & Hechizo, "Cant"))
                    .Mimetiza = Val(Leer.GetValue("hechizo" & Hechizo, "Mimetiza"))

                    '    .Materializa = val(Leer.GetValue("Hechizo" & Hechizo, "Materializa"))
                    '    .ItemIndex = val(Leer.GetValue("Hechizo" & Hechizo, "ItemIndex"))

                    .MinSkill = Val(Leer.GetValue("Hechizo" & Hechizo, "MinSkill"))
                    .ManaRequerido = Val(Leer.GetValue("Hechizo" & Hechizo, "ManaRequerido"))

                    'Barrin 30/9/03
                    .StaRequerido = Val(Leer.GetValue("Hechizo" & Hechizo, "StaRequerido"))

                    .Target = Val(Leer.GetValue("Hechizo" & Hechizo, "Target"))

                    .NeedStaff = Val(Leer.GetValue("Hechizo" & Hechizo, "NeedStaff"))
                    .StaffAffected = CBool(Val(Leer.GetValue("Hechizo" & Hechizo, "StaffAffected")))
                End With
            Next Hechizo

            'UPGRADE_NOTE: El objeto Leer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            Leer = Nothing


        Catch ex As Exception
            Console.WriteLine("Error in CargarSpawnList: " & ex.Message)
            MsgBox("Error cargando hechizos.dat " & Err.Number & ": " & Err.Description)
        End Try
    End Sub

    Sub LoadMotd()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim i As Short

        MaxLines = Val(GetVar(AppDomain.CurrentDomain.BaseDirectory & "Dat/Motd.ini", "INIT", "NumLines"))

        'UPGRADE_WARNING: El límite inferior de la matriz MOTD ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim MOTD(MaxLines)
        For i = 1 To MaxLines
            MOTD(i).texto = GetVar(AppDomain.CurrentDomain.BaseDirectory & "Dat/Motd.ini", "Motd", "Line" & i)
            MOTD(i).Formato = vbNullString
        Next i
    End Sub

    Public Sub DoBackUp()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        haciendoBK = True
        Dim i As Short


        ' Lo saco porque elimina elementales y mascotas - Maraxus
        ''''''''''''''lo pongo aca x sugernecia del yind
        'For i = 1 To LastNPC
        '    If Npclist(i).flags.NPCActive Then
        '        If Npclist(i).Contadores.TiempoExistencia > 0 Then
        '            Call MuereNpc(i, 0)
        '        End If
        '    End If
        'Next i
        '''''''''''/'lo pongo aca x sugernecia del yind


        Call SendData(SendTarget.ToAll, 0, PrepareMessagePauseToggle())


        Call LimpiarMundo()
        Call WorldSave()
        Call v_RutinaElecciones()
        Call ResetCentinelaInfo() 'Reseteamos al centinela


        Call SendData(SendTarget.ToAll, 0, PrepareMessagePauseToggle())

        'Call EstadisticasWeb.Informar(EVENTO_NUEVO_CLAN, 0)

        haciendoBK = False

        'Log
        Try
            Dim nfile As Short
            nfile = FreeFile ' obtenemos un canal
            FileOpen(nfile, AppDomain.CurrentDomain.BaseDirectory & "logs/BackUps.log", OpenMode.Append, ,
                     OpenShare.Shared)
            PrintLine(nfile, Today & " " & TimeOfDay)
            FileClose(nfile)

        Catch ex As Exception
            Console.WriteLine("Error in CargarSpawnList: " & ex.Message)
        End Try
    End Sub

    Public Sub GrabarMapa(Map As Integer, MAPFILE As String)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Try
            Dim FreeFileMap As Integer
            Dim FreeFileInf As Integer
            Dim Y As Integer
            Dim X As Integer
            Dim ByFlags As Byte
            Dim TempInt As Short
            Dim LoopC As Integer

            If FileExist(MAPFILE & ".map") Then
                Kill(MAPFILE & ".map")
            End If

            If FileExist(MAPFILE & ".inf") Then
                Kill(MAPFILE & ".inf")
            End If

            'Open .map file
            FreeFileMap = FreeFile
            FileOpen(FreeFileMap, MAPFILE & ".Map", OpenMode.Binary)
            Seek(FreeFileMap, 1)

            'Open .inf file
            FreeFileInf = FreeFile
            FileOpen(FreeFileInf, MAPFILE & ".Inf", OpenMode.Binary)
            Seek(FreeFileInf, 1)
            'map Header

            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            FilePut(FreeFileMap, MapInfo_Renamed(Map).MapVersion)
            Seek(FreeFileMap, Seek(FreeFileMap) + 263)
            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            FilePut(FreeFileMap, TempInt)
            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            FilePut(FreeFileMap, TempInt)
            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            FilePut(FreeFileMap, TempInt)
            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            FilePut(FreeFileMap, TempInt)

            'inf Header
            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            FilePut(FreeFileInf, TempInt)
            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            FilePut(FreeFileInf, TempInt)
            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            FilePut(FreeFileInf, TempInt)
            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            FilePut(FreeFileInf, TempInt)
            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            FilePut(FreeFileInf, TempInt)

            'Write .map file
            For Y = YMinMapSize To YMaxMapSize
                For X = XMinMapSize To XMaxMapSize
                    With MapData(Map, X, Y)
                        ByFlags = 0

                        If .Blocked Then ByFlags = ByFlags Or 1
                        If .Graphic(2) Then ByFlags = ByFlags Or 2
                        If .Graphic(3) Then ByFlags = ByFlags Or 4
                        If .Graphic(4) Then ByFlags = ByFlags Or 8
                        If .trigger Then ByFlags = ByFlags Or 16

                        'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
                        FilePut(FreeFileMap, ByFlags)

                        'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
                        FilePut(FreeFileMap, .Graphic(1))

                        For LoopC = 2 To 4
                            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
                            If .Graphic(LoopC) Then FilePut(FreeFileMap, .Graphic(LoopC))
                        Next LoopC

                        'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
                        If .trigger Then FilePut(FreeFileMap, CShort(.trigger))

                        '.inf file

                        ByFlags = 0

                        If .ObjInfo.ObjIndex > 0 Then
                            If ObjData_Renamed(.ObjInfo.ObjIndex).OBJType = eOBJType.otFogata Then
                                .ObjInfo.ObjIndex = 0
                                .ObjInfo.Amount = 0
                            End If
                        End If

                        If .TileExit.Map Then ByFlags = ByFlags Or 1
                        If .NpcIndex Then ByFlags = ByFlags Or 2
                        If .ObjInfo.ObjIndex Then ByFlags = ByFlags Or 4

                        'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
                        FilePut(FreeFileInf, ByFlags)

                        If .TileExit.Map Then
                            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
                            FilePut(FreeFileInf, .TileExit.Map)
                            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
                            FilePut(FreeFileInf, .TileExit.X)
                            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
                            FilePut(FreeFileInf, .TileExit.Y)
                        End If

                        'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
                        If .NpcIndex Then FilePut(FreeFileInf, Npclist(.NpcIndex).Numero)

                        If .ObjInfo.ObjIndex Then
                            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
                            FilePut(FreeFileInf, .ObjInfo.ObjIndex)
                            'UPGRADE_WARNING: Put se actualizó a FilePut y tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
                            FilePut(FreeFileInf, .ObjInfo.Amount)
                        End If
                    End With
                Next X
            Next Y

            'Close .map file
            FileClose(FreeFileMap)

            'Close .inf file
            FileClose(FreeFileInf)

            With MapInfo_Renamed(Map)

                'write .dat file
                Call WriteVar(MAPFILE & ".dat", "Mapa" & Map, "Name", .name)
                Call WriteVar(MAPFILE & ".dat", "Mapa" & Map, "MusicNum", .Music)
                Call WriteVar(MAPFILE & ".dat", "mapa" & Map, "MagiaSinefecto", CStr(.MagiaSinEfecto))
                Call WriteVar(MAPFILE & ".dat", "mapa" & Map, "InviSinEfecto", CStr(.InviSinEfecto))
                Call WriteVar(MAPFILE & ".dat", "mapa" & Map, "ResuSinEfecto", CStr(.ResuSinEfecto))

                Call WriteVar(MAPFILE & ".dat", "Mapa" & Map, "Terreno", .Terreno)
                Call WriteVar(MAPFILE & ".dat", "Mapa" & Map, "Zona", .Zona)
                Call WriteVar(MAPFILE & ".dat", "Mapa" & Map, "Restringir", .Restringir)
                Call WriteVar(MAPFILE & ".dat", "Mapa" & Map, "BackUp", Str(.BackUp))

                If .Pk Then
                    Call WriteVar(MAPFILE & ".dat", "Mapa" & Map, "Pk", "0")
                Else
                    Call WriteVar(MAPFILE & ".dat", "Mapa" & Map, "Pk", "1")
                End If
            End With

        Catch ex As Exception
            Console.WriteLine("Error in GrabarMapa: " & ex.Message)
        End Try
    End Sub

    Sub LoadArmasHerreria()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim N, lc As Short

        N = Val(GetVar(DatPath & "ArmasHerrero.dat", "INIT", "NumArmas"))

        'UPGRADE_WARNING: El límite inferior de la matriz ArmasHerrero ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim Preserve ArmasHerrero(N)

        For lc = 1 To N
            ArmasHerrero(lc) = Val(GetVar(DatPath & "ArmasHerrero.dat", "Arma" & lc, "Index"))
        Next lc
    End Sub

    Sub LoadArmadurasHerreria()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim N, lc As Short

        N = Val(GetVar(DatPath & "ArmadurasHerrero.dat", "INIT", "NumArmaduras"))

        'UPGRADE_WARNING: El límite inferior de la matriz ArmadurasHerrero ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim Preserve ArmadurasHerrero(N)

        For lc = 1 To N
            ArmadurasHerrero(lc) = Val(GetVar(DatPath & "ArmadurasHerrero.dat", "Armadura" & lc, "Index"))
        Next lc
    End Sub

    Sub LoadBalance()
        '***************************************************
        'Author: Unknown
        'Last Modification: 15/04/2010
        '15/04/2010: ZaMa - Agrego recompensas faccionarias.
        '***************************************************

        Dim i As Integer

        'Modificadores de Clase
        For i = 1 To NUMCLASES
            With ModClase_Renamed(i)
                .Evasion = Val(GetVar(DatPath & "Balance.dat", "MODEVASION", ListaClases(i)))
                .AtaqueArmas = Val(GetVar(DatPath & "Balance.dat", "MODATAQUEARMAS", ListaClases(i)))
                .AtaqueProyectiles = Val(GetVar(DatPath & "Balance.dat", "MODATAQUEPROYECTILES", ListaClases(i)))
                .AtaqueWrestling = Val(GetVar(DatPath & "Balance.dat", "MODATAQUEWRESTLING", ListaClases(i)))
                .DañoArmas = Val(GetVar(DatPath & "Balance.dat", "MODDAÑOARMAS", ListaClases(i)))
                .DañoProyectiles = Val(GetVar(DatPath & "Balance.dat", "MODDAÑOPROYECTILES", ListaClases(i)))
                .DañoWrestling = Val(GetVar(DatPath & "Balance.dat", "MODDAÑOWRESTLING", ListaClases(i)))
                .Escudo = Val(GetVar(DatPath & "Balance.dat", "MODESCUDO", ListaClases(i)))
            End With
        Next i

        'Modificadores de Raza
        For i = 1 To NUMRAZAS
            With ModRaza_Renamed(i)
                .Fuerza = Val(GetVar(DatPath & "Balance.dat", "MODRAZA", ListaRazas(i) & "Fuerza"))
                .Agilidad = Val(GetVar(DatPath & "Balance.dat", "MODRAZA", ListaRazas(i) & "Agilidad"))
                .Inteligencia = Val(GetVar(DatPath & "Balance.dat", "MODRAZA", ListaRazas(i) & "Inteligencia"))
                .Carisma = Val(GetVar(DatPath & "Balance.dat", "MODRAZA", ListaRazas(i) & "Carisma"))
                .Constitucion = Val(GetVar(DatPath & "Balance.dat", "MODRAZA", ListaRazas(i) & "Constitucion"))
            End With
        Next i

        'Modificadores de Vida
        For i = 1 To NUMCLASES
            ModVida(i) = Val(GetVar(DatPath & "Balance.dat", "MODVIDA", ListaClases(i)))
        Next i

        'Distribución de Vida
        For i = 1 To 5
            DistribucionEnteraVida(i) = Val(GetVar(DatPath & "Balance.dat", "DISTRIBUCION", "E" & CStr(i)))
        Next i
        For i = 1 To 4
            DistribucionSemienteraVida(i) = Val(GetVar(DatPath & "Balance.dat", "DISTRIBUCION", "S" & CStr(i)))
        Next i

        'Extra
        PorcentajeRecuperoMana = Val(GetVar(DatPath & "Balance.dat", "EXTRA", "PorcentajeRecuperoMana"))

        'Party
        ExponenteNivelParty = Val(GetVar(DatPath & "Balance.dat", "PARTY", "ExponenteNivelParty"))

        ' Recompensas faccionarias
        For i = 1 To NUM_RANGOS_FACCION
            RecompensaFacciones(i - 1) = Val(GetVar(DatPath & "Balance.dat", "RECOMPENSAFACCION", "Rango" & i))
        Next i
    End Sub

    Sub LoadObjCarpintero()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim N, lc As Short

        N = Val(GetVar(DatPath & "ObjCarpintero.dat", "INIT", "NumObjs"))

        'UPGRADE_WARNING: El límite inferior de la matriz ObjCarpintero ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim Preserve ObjCarpintero(N)

        For lc = 1 To N
            ObjCarpintero(lc) = Val(GetVar(DatPath & "ObjCarpintero.dat", "Obj" & lc, "Index"))
        Next lc
    End Sub


    Sub LoadOBJData()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        '###################################################
        '#               ATENCION PELIGRO                  #
        '###################################################
        '
        '¡¡¡¡ NO USAR GetVar PARA LEER DESDE EL OBJ.DAT !!!!
        '
        'El que ose desafiar esta LEY, se las tendrá que ver
        'con migo. Para leer desde el OBJ.DAT se deberá usar
        'la nueva clase clsLeerInis.
        '
        'Alejo
        '
        '###################################################

        'Call LogTarea("Sub LoadOBJData")

        Try

            '*****************************************************************
            'Carga la lista de objetos
            '*****************************************************************
            'UPGRADE_NOTE: Object se actualizó a Object_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            Dim Object_Renamed As Short
            Dim Leer As New clsIniReader

            Call Leer.Initialize(DatPath & "Obj.dat")

            'obtiene el numero de obj
            NumObjDatas = Val(Leer.GetValue("INIT", "NumObjs"))

            'UPGRADE_WARNING: Es posible que la matriz ObjData_Renamed necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
            'UPGRADE_WARNING: El límite inferior de la matriz ObjData_Renamed ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            'UPGRADE_NOTE: ObjData se actualizó a ObjData_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            ReDim Preserve ObjData_Renamed(NumObjDatas)
            InitializeStruct(ObjData_Renamed)


            'Llena la lista
            Dim i As Short
            Dim N As Short
            Dim S As String
            For Object_Renamed = 1 To NumObjDatas
                With ObjData_Renamed(Object_Renamed)
                    .name = Leer.GetValue("OBJ" & Object_Renamed, "Name")

                    'Pablo (ToxicWaste) Log de Objetos.
                    .Log = Val(Leer.GetValue("OBJ" & Object_Renamed, "Log"))
                    .NoLog = Val(Leer.GetValue("OBJ" & Object_Renamed, "NoLog"))
                    '07/09/07

                    .GrhIndex = Val(Leer.GetValue("OBJ" & Object_Renamed, "GrhIndex"))
                    If .GrhIndex = 0 Then
                        .GrhIndex = .GrhIndex
                    End If

                    .OBJType = Val(Leer.GetValue("OBJ" & Object_Renamed, "ObjType"))

                    .Newbie = Val(Leer.GetValue("OBJ" & Object_Renamed, "Newbie"))

                    Select Case .OBJType
                        Case eOBJType.otArmadura
                            .Real = Val(Leer.GetValue("OBJ" & Object_Renamed, "Real"))
                            .Caos = Val(Leer.GetValue("OBJ" & Object_Renamed, "Caos"))
                            .LingH = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingH"))
                            .LingP = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingP"))
                            .LingO = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingO"))
                            .SkHerreria = Val(Leer.GetValue("OBJ" & Object_Renamed, "SkHerreria"))

                        Case eOBJType.otESCUDO
                            .ShieldAnim = Val(Leer.GetValue("OBJ" & Object_Renamed, "Anim"))
                            .LingH = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingH"))
                            .LingP = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingP"))
                            .LingO = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingO"))
                            .SkHerreria = Val(Leer.GetValue("OBJ" & Object_Renamed, "SkHerreria"))
                            .Real = Val(Leer.GetValue("OBJ" & Object_Renamed, "Real"))
                            .Caos = Val(Leer.GetValue("OBJ" & Object_Renamed, "Caos"))

                        Case eOBJType.otCASCO
                            .CascoAnim = Val(Leer.GetValue("OBJ" & Object_Renamed, "Anim"))
                            .LingH = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingH"))
                            .LingP = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingP"))
                            .LingO = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingO"))
                            .SkHerreria = Val(Leer.GetValue("OBJ" & Object_Renamed, "SkHerreria"))
                            .Real = Val(Leer.GetValue("OBJ" & Object_Renamed, "Real"))
                            .Caos = Val(Leer.GetValue("OBJ" & Object_Renamed, "Caos"))

                        Case eOBJType.otWeapon
                            .WeaponAnim = Val(Leer.GetValue("OBJ" & Object_Renamed, "Anim"))
                            .Apuñala = Val(Leer.GetValue("OBJ" & Object_Renamed, "Apuñala"))
                            .Envenena = Val(Leer.GetValue("OBJ" & Object_Renamed, "Envenena"))
                            .MaxHIT = Val(Leer.GetValue("OBJ" & Object_Renamed, "MaxHIT"))
                            .MinHIT = Val(Leer.GetValue("OBJ" & Object_Renamed, "MinHIT"))
                            .proyectil = Val(Leer.GetValue("OBJ" & Object_Renamed, "Proyectil"))
                            .Municion = Val(Leer.GetValue("OBJ" & Object_Renamed, "Municiones"))
                            .StaffPower = Val(Leer.GetValue("OBJ" & Object_Renamed, "StaffPower"))
                            .StaffDamageBonus = Val(Leer.GetValue("OBJ" & Object_Renamed, "StaffDamageBonus"))
                            .Refuerzo = Val(Leer.GetValue("OBJ" & Object_Renamed, "Refuerzo"))

                            .LingH = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingH"))
                            .LingP = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingP"))
                            .LingO = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingO"))
                            .SkHerreria = Val(Leer.GetValue("OBJ" & Object_Renamed, "SkHerreria"))
                            .Real = Val(Leer.GetValue("OBJ" & Object_Renamed, "Real"))
                            .Caos = Val(Leer.GetValue("OBJ" & Object_Renamed, "Caos"))

                            .WeaponRazaEnanaAnim = Val(Leer.GetValue("OBJ" & Object_Renamed, "RazaEnanaAnim"))

                        Case eOBJType.otInstrumentos
                            .Snd1 = Val(Leer.GetValue("OBJ" & Object_Renamed, "SND1"))
                            .Snd2 = Val(Leer.GetValue("OBJ" & Object_Renamed, "SND2"))
                            .Snd3 = Val(Leer.GetValue("OBJ" & Object_Renamed, "SND3"))
                            'Pablo (ToxicWaste)
                            .Real = Val(Leer.GetValue("OBJ" & Object_Renamed, "Real"))
                            .Caos = Val(Leer.GetValue("OBJ" & Object_Renamed, "Caos"))

                        Case eOBJType.otMinerales
                            .MinSkill = Val(Leer.GetValue("OBJ" & Object_Renamed, "MinSkill"))

                        Case eOBJType.otPuertas, eOBJType.otBotellaVacia,
                            eOBJType.otBotellaLlena
                            .IndexAbierta = Val(Leer.GetValue("OBJ" & Object_Renamed, "IndexAbierta"))
                            .IndexCerrada = Val(Leer.GetValue("OBJ" & Object_Renamed, "IndexCerrada"))
                            .IndexCerradaLlave = Val(Leer.GetValue("OBJ" & Object_Renamed, "IndexCerradaLlave"))

                        Case eOBJType.otPociones
                            .TipoPocion = Val(Leer.GetValue("OBJ" & Object_Renamed, "TipoPocion"))
                            .MaxModificador = Val(Leer.GetValue("OBJ" & Object_Renamed, "MaxModificador"))
                            .MinModificador = Val(Leer.GetValue("OBJ" & Object_Renamed, "MinModificador"))
                            .DuracionEfecto = Val(Leer.GetValue("OBJ" & Object_Renamed, "DuracionEfecto"))

                        Case eOBJType.otBarcos
                            .MinSkill = Val(Leer.GetValue("OBJ" & Object_Renamed, "MinSkill"))
                            .MaxHIT = Val(Leer.GetValue("OBJ" & Object_Renamed, "MaxHIT"))
                            .MinHIT = Val(Leer.GetValue("OBJ" & Object_Renamed, "MinHIT"))

                        Case eOBJType.otFlechas
                            .MaxHIT = Val(Leer.GetValue("OBJ" & Object_Renamed, "MaxHIT"))
                            .MinHIT = Val(Leer.GetValue("OBJ" & Object_Renamed, "MinHIT"))
                            .Envenena = Val(Leer.GetValue("OBJ" & Object_Renamed, "Envenena"))
                            .Paraliza = Val(Leer.GetValue("OBJ" & Object_Renamed, "Paraliza"))

                        Case eOBJType.otAnillo 'Pablo (ToxicWaste)
                            .LingH = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingH"))
                            .LingP = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingP"))
                            .LingO = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingO"))
                            .SkHerreria = Val(Leer.GetValue("OBJ" & Object_Renamed, "SkHerreria"))
                            .MaxHIT = Val(Leer.GetValue("OBJ" & Object_Renamed, "MaxHIT"))
                            .MinHIT = Val(Leer.GetValue("OBJ" & Object_Renamed, "MinHIT"))

                        Case eOBJType.otTeleport
                            .Radio = Val(Leer.GetValue("OBJ" & Object_Renamed, "Radio"))

                        Case eOBJType.otMochilas
                            .MochilaType = Val(Leer.GetValue("OBJ" & Object_Renamed, "MochilaType"))

                        Case eOBJType.otForos
                            Call AddForum(Leer.GetValue("OBJ" & Object_Renamed, "ID"))

                    End Select

                    .Ropaje = Val(Leer.GetValue("OBJ" & Object_Renamed, "NumRopaje"))
                    .HechizoIndex = Val(Leer.GetValue("OBJ" & Object_Renamed, "HechizoIndex"))

                    .LingoteIndex = Val(Leer.GetValue("OBJ" & Object_Renamed, "LingoteIndex"))

                    .MineralIndex = Val(Leer.GetValue("OBJ" & Object_Renamed, "MineralIndex"))

                    .MaxHp = Val(Leer.GetValue("OBJ" & Object_Renamed, "MaxHP"))
                    .MinHp = Val(Leer.GetValue("OBJ" & Object_Renamed, "MinHP"))

                    .Mujer = Val(Leer.GetValue("OBJ" & Object_Renamed, "Mujer"))
                    .Hombre = Val(Leer.GetValue("OBJ" & Object_Renamed, "Hombre"))

                    .MinHam = Val(Leer.GetValue("OBJ" & Object_Renamed, "MinHam"))
                    .MinSed = Val(Leer.GetValue("OBJ" & Object_Renamed, "MinAgu"))

                    .MinDef = Val(Leer.GetValue("OBJ" & Object_Renamed, "MINDEF"))
                    .MaxDef = Val(Leer.GetValue("OBJ" & Object_Renamed, "MAXDEF"))
                    .def = (.MinDef + .MaxDef)/2

                    .RazaEnana = Val(Leer.GetValue("OBJ" & Object_Renamed, "RazaEnana"))
                    .RazaDrow = Val(Leer.GetValue("OBJ" & Object_Renamed, "RazaDrow"))
                    .RazaElfa = Val(Leer.GetValue("OBJ" & Object_Renamed, "RazaElfa"))
                    .RazaGnoma = Val(Leer.GetValue("OBJ" & Object_Renamed, "RazaGnoma"))
                    .RazaHumana = Val(Leer.GetValue("OBJ" & Object_Renamed, "RazaHumana"))

                    .Valor = Val(Leer.GetValue("OBJ" & Object_Renamed, "Valor"))

                    .Crucial = Val(Leer.GetValue("OBJ" & Object_Renamed, "Crucial"))

                    .Cerrada = Val(Leer.GetValue("OBJ" & Object_Renamed, "abierta"))
                    If .Cerrada = 1 Then
                        .Llave = Val(Leer.GetValue("OBJ" & Object_Renamed, "Llave"))
                        .clave = Val(Leer.GetValue("OBJ" & Object_Renamed, "Clave"))
                    End If

                    'Puertas y llaves
                    .clave = Val(Leer.GetValue("OBJ" & Object_Renamed, "Clave"))

                    .texto = Leer.GetValue("OBJ" & Object_Renamed, "Texto")
                    .GrhSecundario = Val(Leer.GetValue("OBJ" & Object_Renamed, "VGrande"))

                    .Agarrable = Val(Leer.GetValue("OBJ" & Object_Renamed, "Agarrable"))
                    .ForoID = Leer.GetValue("OBJ" & Object_Renamed, "ID")

                    .Acuchilla = Val(Leer.GetValue("OBJ" & Object_Renamed, "Acuchilla"))

                    .Guante = Val(Leer.GetValue("OBJ" & Object_Renamed, "Guante"))

                    'CHECK: !!! Esto es provisorio hasta que los de Dateo cambien los valores de string a numerico
                    For i = 1 To NUMCLASES
                        S = UCase(Leer.GetValue("OBJ" & Object_Renamed, "CP" & i))
                        N = 1
                        Do While migr_LenB(S) > 0 And UCase(ListaClases(N)) <> S
                            N = N + 1
                        Loop
                        .ClaseProhibida(i) = IIf(migr_LenB(S) > 0, N, 0)
                    Next i

                    .DefensaMagicaMax = Val(Leer.GetValue("OBJ" & Object_Renamed, "DefensaMagicaMax"))
                    .DefensaMagicaMin = Val(Leer.GetValue("OBJ" & Object_Renamed, "DefensaMagicaMin"))

                    .SkCarpinteria = Val(Leer.GetValue("OBJ" & Object_Renamed, "SkCarpinteria"))

                    If .SkCarpinteria > 0 Then .Madera = Val(Leer.GetValue("OBJ" & Object_Renamed, "Madera"))
                    .MaderaElfica = Val(Leer.GetValue("OBJ" & Object_Renamed, "MaderaElfica"))

                    'Bebidas
                    .MinSta = Val(Leer.GetValue("OBJ" & Object_Renamed, "MinST"))

                    .NoSeCae = Val(Leer.GetValue("OBJ" & Object_Renamed, "NoSeCae"))

                    .Upgrade = Val(Leer.GetValue("OBJ" & Object_Renamed, "Upgrade"))

                End With
            Next Object_Renamed


            'UPGRADE_NOTE: El objeto Leer no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            Leer = Nothing

            ' Inicializo los foros faccionarios
            Call AddForum(FORO_CAOS_ID)
            Call AddForum(FORO_REAL_ID)


        Catch ex As Exception
            Console.WriteLine("Error in LoadMotd: " & ex.Message)
            MsgBox("error cargando objetos " & Err.Number & ": " & Err.Description)
        End Try
    End Sub

    Sub LoadUserStats(UserIndex As Short, ByRef UserFile As clsIniReader)
        '*************************************************
        'Author: Unknown
        'Last modified: 11/19/2009
        '11/19/2009: Pato - Load the EluSkills and ExpSkills
        '*************************************************
        Dim LoopC As Integer

        With UserList(UserIndex)
            With .Stats
                For LoopC = 1 To NUMATRIBUTOS
                    .UserAtributos(LoopC) = CShort(UserFile.GetValue("ATRIBUTOS", "AT" & LoopC))
                    .UserAtributosBackUP(LoopC) = .UserAtributos(LoopC)
                Next LoopC

                For LoopC = 1 To NUMSKILLS
                    .UserSkills(LoopC) = CShort(UserFile.GetValue("SKILLS", "SK" & LoopC))
                    .EluSkills(LoopC) = CShort(UserFile.GetValue("SKILLS", "ELUSK" & LoopC))
                    .ExpSkills(LoopC) = CShort(UserFile.GetValue("SKILLS", "EXPSK" & LoopC))
                Next LoopC

                For LoopC = 1 To MAXUSERHECHIZOS
                    .UserHechizos(LoopC) = CShort(UserFile.GetValue("Hechizos", "H" & LoopC))
                Next LoopC

                .GLD = CInt(UserFile.GetValue("STATS", "GLD"))
                .Banco = CInt(UserFile.GetValue("STATS", "BANCO"))

                .MaxHp = CShort(UserFile.GetValue("STATS", "MaxHP"))
                .MinHp = CShort(UserFile.GetValue("STATS", "MinHP"))

                .MinSta = CShort(UserFile.GetValue("STATS", "MinSTA"))
                .MaxSta = CShort(UserFile.GetValue("STATS", "MaxSTA"))

                .MaxMAN = CShort(UserFile.GetValue("STATS", "MaxMAN"))
                .MinMAN = CShort(UserFile.GetValue("STATS", "MinMAN"))

                .MaxHIT = CShort(UserFile.GetValue("STATS", "MaxHIT"))
                .MinHIT = CShort(UserFile.GetValue("STATS", "MinHIT"))

                .MaxAGU = CByte(UserFile.GetValue("STATS", "MaxAGU"))
                .MinAGU = CByte(UserFile.GetValue("STATS", "MinAGU"))

                .MaxHam = CByte(UserFile.GetValue("STATS", "MaxHAM"))
                .MinHam = CByte(UserFile.GetValue("STATS", "MinHAM"))

                .SkillPts = CShort(UserFile.GetValue("STATS", "SkillPtsLibres"))

                .Exp = CDbl(UserFile.GetValue("STATS", "EXP"))
                .ELU = CInt(UserFile.GetValue("STATS", "ELU"))
                .ELV = CByte(UserFile.GetValue("STATS", "ELV"))


                .UsuariosMatados = CInt(UserFile.GetValue("MUERTES", "UserMuertes"))
                .NPCsMuertos = CShort(UserFile.GetValue("MUERTES", "NpcsMuertes"))
            End With

            With .flags
                If CByte(UserFile.GetValue("CONSEJO", "PERTENECE")) Then _
                    .Privilegios = .Privilegios Or PlayerType.RoyalCouncil

                If CByte(UserFile.GetValue("CONSEJO", "PERTENECECAOS")) Then _
                    .Privilegios = .Privilegios Or PlayerType.ChaosCouncil
            End With
        End With
    End Sub

    Sub LoadUserReputacion(UserIndex As Short, ByRef UserFile As clsIniReader)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        With UserList(UserIndex).Reputacion
            .AsesinoRep = Val(UserFile.GetValue("REP", "Asesino"))
            .BandidoRep = Val(UserFile.GetValue("REP", "Bandido"))
            .BurguesRep = Val(UserFile.GetValue("REP", "Burguesia"))
            .LadronesRep = Val(UserFile.GetValue("REP", "Ladrones"))
            .NobleRep = Val(UserFile.GetValue("REP", "Nobles"))
            .PlebeRep = Val(UserFile.GetValue("REP", "Plebe"))
            .Promedio = Val(UserFile.GetValue("REP", "Promedio"))
        End With
    End Sub

    Sub LoadUserInit(UserIndex As Short, ByRef UserFile As clsIniReader)
        '*************************************************
        'Author: Unknown
        'Last modified: 19/11/2006
        'Loads the Users records
        '23/01/2007 Pablo (ToxicWaste) - Agrego NivelIngreso, FechaIngreso, MatadosIngreso y NextRecompensa.
        '23/01/2007 Pablo (ToxicWaste) - Quito CriminalesMatados de Stats porque era redundante.
        '*************************************************
        Dim LoopC As Integer
        Dim ln As String

        Dim NpcIndex As Short
        With UserList(UserIndex)
            With .Faccion
                .ArmadaReal = CByte(UserFile.GetValue("FACCIONES", "EjercitoReal"))
                .FuerzasCaos = CByte(UserFile.GetValue("FACCIONES", "EjercitoCaos"))
                .CiudadanosMatados = CInt(UserFile.GetValue("FACCIONES", "CiudMatados"))
                .CriminalesMatados = CInt(UserFile.GetValue("FACCIONES", "CrimMatados"))
                .RecibioArmaduraCaos = CByte(UserFile.GetValue("FACCIONES", "rArCaos"))
                .RecibioArmaduraReal = CByte(UserFile.GetValue("FACCIONES", "rArReal"))
                .RecibioExpInicialCaos = CByte(UserFile.GetValue("FACCIONES", "rExCaos"))
                .RecibioExpInicialReal = CByte(UserFile.GetValue("FACCIONES", "rExReal"))
                .RecompensasCaos = CInt(UserFile.GetValue("FACCIONES", "recCaos"))
                .RecompensasReal = CInt(UserFile.GetValue("FACCIONES", "recReal"))
                .Reenlistadas = CByte(UserFile.GetValue("FACCIONES", "Reenlistadas"))
                .NivelIngreso = CShort(UserFile.GetValue("FACCIONES", "NivelIngreso"))
                .FechaIngreso = UserFile.GetValue("FACCIONES", "FechaIngreso")
                .MatadosIngreso = CShort(UserFile.GetValue("FACCIONES", "MatadosIngreso"))
                .NextRecompensa = CShort(UserFile.GetValue("FACCIONES", "NextRecompensa"))
            End With

            With .flags
                .Muerto = CByte(UserFile.GetValue("FLAGS", "Muerto"))
                .Escondido = CByte(UserFile.GetValue("FLAGS", "Escondido"))

                .Hambre = CByte(UserFile.GetValue("FLAGS", "Hambre"))
                .Sed = CByte(UserFile.GetValue("FLAGS", "Sed"))
                .Desnudo = CByte(UserFile.GetValue("FLAGS", "Desnudo"))
                .Navegando = CByte(UserFile.GetValue("FLAGS", "Navegando"))
                .Envenenado = CByte(UserFile.GetValue("FLAGS", "Envenenado"))
                .Paralizado = CByte(UserFile.GetValue("FLAGS", "Paralizado"))

                'Matrix
                .lastMap = CShort(UserFile.GetValue("FLAGS", "LastMap"))
            End With

            If .flags.Paralizado = 1 Then
                .Counters.Paralisis = IntervaloParalizado
            End If


            .Counters.Pena = CInt(UserFile.GetValue("COUNTERS", "Pena"))
            .Counters.AsignedSkills = CByte(Val(UserFile.GetValue("COUNTERS", "SkillsAsignados")))

            .email = UserFile.GetValue("CONTACTO", "Email")

            .Genero = CShort(UserFile.GetValue("INIT", "Genero"))
            .clase = CShort(UserFile.GetValue("INIT", "Clase"))
            .raza = CShort(UserFile.GetValue("INIT", "Raza"))
            .Hogar = CShort(UserFile.GetValue("INIT", "Hogar"))
            .Char_Renamed.heading = CShort(UserFile.GetValue("INIT", "Heading"))


            With .OrigChar
                .Head = CShort(UserFile.GetValue("INIT", "Head"))
                .body = CShort(UserFile.GetValue("INIT", "Body"))
                .WeaponAnim = CShort(UserFile.GetValue("INIT", "Arma"))
                .ShieldAnim = CShort(UserFile.GetValue("INIT", "Escudo"))
                .CascoAnim = CShort(UserFile.GetValue("INIT", "Casco"))

                .heading = eHeading.SOUTH
            End With

            .UpTime = CInt(UserFile.GetValue("INIT", "UpTime"))

            If .flags.Muerto = 0 Then
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto UserList().Char. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                .Char_Renamed = .OrigChar
            Else
                .Char_Renamed.body = iCuerpoMuerto
                .Char_Renamed.Head = iCabezaMuerto
                .Char_Renamed.WeaponAnim = NingunArma
                .Char_Renamed.ShieldAnim = NingunEscudo
                .Char_Renamed.CascoAnim = NingunCasco
            End If


            .desc = UserFile.GetValue("INIT", "Desc")

            .Pos.Map = CShort(ReadField(1, UserFile.GetValue("INIT", "Position"), 45))
            .Pos.X = CShort(ReadField(2, UserFile.GetValue("INIT", "Position"), 45))
            .Pos.Y = CShort(ReadField(3, UserFile.GetValue("INIT", "Position"), 45))

            .Invent.NroItems = CShort(UserFile.GetValue("Inventory", "CantidadItems"))

            '[KEVIN]--------------------------------------------------------------------
            '***********************************************************************************
            .BancoInvent.NroItems = CShort(UserFile.GetValue("BancoInventory", "CantidadItems"))
            'Lista de objetos del banco
            For LoopC = 1 To MAX_BANCOINVENTORY_SLOTS
                ln = UserFile.GetValue("BancoInventory", "Obj" & LoopC)
                .BancoInvent.Object_Renamed(LoopC).ObjIndex = CShort(ReadField(1, ln, 45))
                .BancoInvent.Object_Renamed(LoopC).Amount = CShort(ReadField(2, ln, 45))
            Next LoopC
            '------------------------------------------------------------------------------------
            '[/KEVIN]*****************************************************************************


            'Lista de objetos
            For LoopC = 1 To MAX_INVENTORY_SLOTS
                ln = UserFile.GetValue("Inventory", "Obj" & LoopC)
                .Invent.Object_Renamed(LoopC).ObjIndex = CShort(ReadField(1, ln, 45))
                .Invent.Object_Renamed(LoopC).Amount = CShort(ReadField(2, ln, 45))
                .Invent.Object_Renamed(LoopC).Equipped = CByte(ReadField(3, ln, 45))
            Next LoopC

            'Obtiene el indice-objeto del arma
            .Invent.WeaponEqpSlot = CByte(UserFile.GetValue("Inventory", "WeaponEqpSlot"))
            If .Invent.WeaponEqpSlot > 0 Then
                .Invent.WeaponEqpObjIndex = .Invent.Object_Renamed(.Invent.WeaponEqpSlot).ObjIndex
            End If

            'Obtiene el indice-objeto del armadura
            .Invent.ArmourEqpSlot = CByte(UserFile.GetValue("Inventory", "ArmourEqpSlot"))
            If .Invent.ArmourEqpSlot > 0 Then
                .Invent.ArmourEqpObjIndex = .Invent.Object_Renamed(.Invent.ArmourEqpSlot).ObjIndex
                .flags.Desnudo = 0
            Else
                .flags.Desnudo = 1
            End If

            'Obtiene el indice-objeto del escudo
            .Invent.EscudoEqpSlot = CByte(UserFile.GetValue("Inventory", "EscudoEqpSlot"))
            If .Invent.EscudoEqpSlot > 0 Then
                .Invent.EscudoEqpObjIndex = .Invent.Object_Renamed(.Invent.EscudoEqpSlot).ObjIndex
            End If

            'Obtiene el indice-objeto del casco
            .Invent.CascoEqpSlot = CByte(UserFile.GetValue("Inventory", "CascoEqpSlot"))
            If .Invent.CascoEqpSlot > 0 Then
                .Invent.CascoEqpObjIndex = .Invent.Object_Renamed(.Invent.CascoEqpSlot).ObjIndex
            End If

            'Obtiene el indice-objeto barco
            .Invent.BarcoSlot = CByte(UserFile.GetValue("Inventory", "BarcoSlot"))
            If .Invent.BarcoSlot > 0 Then
                .Invent.BarcoObjIndex = .Invent.Object_Renamed(.Invent.BarcoSlot).ObjIndex
            End If

            'Obtiene el indice-objeto municion
            .Invent.MunicionEqpSlot = CByte(UserFile.GetValue("Inventory", "MunicionSlot"))
            If .Invent.MunicionEqpSlot > 0 Then
                .Invent.MunicionEqpObjIndex = .Invent.Object_Renamed(.Invent.MunicionEqpSlot).ObjIndex
            End If

            '[Alejo]
            'Obtiene el indice-objeto anilo
            .Invent.AnilloEqpSlot = CByte(UserFile.GetValue("Inventory", "AnilloSlot"))
            If .Invent.AnilloEqpSlot > 0 Then
                .Invent.AnilloEqpObjIndex = .Invent.Object_Renamed(.Invent.AnilloEqpSlot).ObjIndex
            End If

            .Invent.MochilaEqpSlot = CByte(UserFile.GetValue("Inventory", "MochilaSlot"))
            If .Invent.MochilaEqpSlot > 0 Then
                .Invent.MochilaEqpObjIndex = .Invent.Object_Renamed(.Invent.MochilaEqpSlot).ObjIndex
            End If

            .NroMascotas = CShort(UserFile.GetValue("MASCOTAS", "NroMascotas"))
            For LoopC = 1 To MAXMASCOTAS
                .MascotasType(LoopC) = Val(UserFile.GetValue("MASCOTAS", "MAS" & LoopC))
            Next LoopC

            ln = UserFile.GetValue("Guild", "GUILDINDEX")
            If IsNumeric(ln) Then
                .GuildIndex = CShort(ln)
            Else
                .GuildIndex = 0
            End If
        End With
    End Sub

    ' TODO MIGRA: funciona pero es lento e ineficiente
    Public Function GetVar(filePath As String, sectionName As String, keyName As String,
                           Optional ByRef EmptySpaces As Integer = 1024) As String
        Dim fileNumber As Short
        Dim currentLine As String
        Dim currentSection As String
        Dim equalPos As Integer

        GetVar = "" ' Default return if not found

        ' Check if file exists
        If Not File.Exists(filePath) Then
            Return ""
        End If

        Try
            fileNumber = FreeFile()
            FileOpen(fileNumber, filePath, OpenMode.Input)

            currentSection = ""
            While Not EOF(fileNumber)
                currentLine = LineInput(fileNumber)
                currentLine = Trim(currentLine)

                ' Check if it's a section line, e.g. [SECTION]
                If Left(currentLine, 1) = "[" And Right(currentLine, 1) = "]" Then
                    currentSection = Mid(currentLine, 2, Len(currentLine) - 2)
                ElseIf StrComp(currentSection, sectionName, CompareMethod.Text) = 0 Then
                    ' We are in the correct section, check if line contains the key
                    equalPos = InStr(1, currentLine, "=", CompareMethod.Text)
                    If equalPos > 1 Then
                        ' Extract the key (left side of '=') and compare
                        If StrComp(Trim(Left(currentLine, equalPos - 1)), keyName, CompareMethod.Text) = 0 Then
                            ' Return the value (right side of '='), trimmed
                            Return Trim(Mid(currentLine, equalPos + 1))
                        End If
                    End If
                End If
            End While
        Catch ex As Exception
            ' Podés loguear el error si querés
        Finally
            If fileNumber <> 0 Then
                FileClose(fileNumber)
            End If
        End Try

        Return GetVar
    End Function


    Sub CargarBackUp()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim Map As Short
        Dim TempInt As Short
        Dim tFileName As String
        Dim npcfile As String

        Try

            NumMaps = Val(GetVar(DatPath & "Map.dat", "INIT", "NumMaps"))
            Call InitAreas()

            MapPath = GetVar(DatPath & "Map.dat", "INIT", "MapPath")

            'UPGRADE_WARNING: Es posible que la matriz MapData necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
            'UPGRADE_WARNING: El límite inferior de la matriz MapData ha cambiado de 1,XMinMapSize,YMinMapSize a 0,0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim MapData(NumMaps, XMaxMapSize, YMaxMapSize)
            InitializeStruct(MapData)
            'UPGRADE_WARNING: El límite inferior de la matriz MapInfo_Renamed ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            'UPGRADE_NOTE: MapInfo se actualizó a MapInfo_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            ReDim MapInfo_Renamed(NumMaps)

            For Map = 1 To NumMaps
                If _
                    Val(GetVar(AppDomain.CurrentDomain.BaseDirectory & MapPath & "Mapa" & Map & ".Dat", "Mapa" & Map,
                               "BackUp")) <> 0 Then
                    tFileName = AppDomain.CurrentDomain.BaseDirectory & "WorldBackUp/Mapa" & Map

                    If Not FileExist(tFileName & ".*") Then _
'Miramos que exista al menos uno de los 3 archivos, sino lo cargamos de la carpeta de los mapas
                        tFileName = AppDomain.CurrentDomain.BaseDirectory & MapPath & "Mapa" & Map
                    End If
                Else
                    tFileName = AppDomain.CurrentDomain.BaseDirectory & MapPath & "Mapa" & Map
                End If

                Call CargarMapa(Map, tFileName)
            Next Map


        Catch ex As Exception
            Console.WriteLine("Error in CargarBackUp: " & ex.Message)
            MsgBox("Error durante la carga de mapas, el mapa " & Map & " contiene errores")
            Call LogError(Today & " " & Err.Description & " " & Err.HelpContext & " " & Err.HelpFile & " " & Err.Source)
        End Try
    End Sub

    Sub LoadMapData()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim Map As Short
        Dim tFileName As String

        Try

            NumMaps = Val(GetVar(DatPath & "Map.dat", "INIT", "NumMaps"))
            Call InitAreas()

            MapPath = GetVar(DatPath & "Map.dat", "INIT", "MapPath")

            'UPGRADE_WARNING: Es posible que la matriz MapData necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
            'UPGRADE_WARNING: El límite inferior de la matriz MapData ha cambiado de 1,XMinMapSize,YMinMapSize a 0,0,0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim MapData(NumMaps, XMaxMapSize, YMaxMapSize)
            InitializeStruct(MapData)
            'UPGRADE_WARNING: El límite inferior de la matriz MapInfo_Renamed ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            'UPGRADE_NOTE: MapInfo se actualizó a MapInfo_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            ReDim MapInfo_Renamed(NumMaps)

            For Map = 1 To NumMaps
                Console.WriteLine("Cargando mapa:" & Map)
                tFileName = AppDomain.CurrentDomain.BaseDirectory & MapPath & "Mapa" & Map
                Call CargarMapa(Map, tFileName)
            Next Map


        Catch ex As Exception
            Console.WriteLine("Error in LoadMapData: " & ex.Message)
            MsgBox("Error durante la carga de mapas, el mapa " & Map & " contiene errores")
            Call LogError(Today & " " & Err.Description & " " & Err.HelpContext & " " & Err.HelpFile & " " & Err.Source)
        End Try
    End Sub

    Sub LoadSini()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim Temporal As Integer

        BootDelBackUp = Val(GetVar(IniPath & "Server.ini", "INIT", "IniciarDesdeBackUp"))

        Puerto = Val(GetVar(IniPath & "Server.ini", "INIT", "StartPort"))
        HideMe = Val(GetVar(IniPath & "Server.ini", "INIT", "Hide"))
        AllowMultiLogins = Val(GetVar(IniPath & "Server.ini", "INIT", "AllowMultiLogins"))
        IdleLimit = Val(GetVar(IniPath & "Server.ini", "INIT", "IdleLimit"))
        'Lee la version correcta del cliente
        ULTIMAVERSION = GetVar(IniPath & "Server.ini", "INIT", "Version")

        PuedeCrearPersonajes = Val(GetVar(IniPath & "Server.ini", "INIT", "PuedeCrearPersonajes"))
        ServerSoloGMs = Val(GetVar(IniPath & "Server.ini", "init", "ServerSoloGMs"))

        ArmaduraImperial1 = Val(GetVar(IniPath & "Server.ini", "INIT", "ArmaduraImperial1"))
        ArmaduraImperial2 = Val(GetVar(IniPath & "Server.ini", "INIT", "ArmaduraImperial2"))
        ArmaduraImperial3 = Val(GetVar(IniPath & "Server.ini", "INIT", "ArmaduraImperial3"))
        TunicaMagoImperial = Val(GetVar(IniPath & "Server.ini", "INIT", "TunicaMagoImperial"))
        TunicaMagoImperialEnanos = Val(GetVar(IniPath & "Server.ini", "INIT", "TunicaMagoImperialEnanos"))
        ArmaduraCaos1 = Val(GetVar(IniPath & "Server.ini", "INIT", "ArmaduraCaos1"))
        ArmaduraCaos2 = Val(GetVar(IniPath & "Server.ini", "INIT", "ArmaduraCaos2"))
        ArmaduraCaos3 = Val(GetVar(IniPath & "Server.ini", "INIT", "ArmaduraCaos3"))
        TunicaMagoCaos = Val(GetVar(IniPath & "Server.ini", "INIT", "TunicaMagoCaos"))
        TunicaMagoCaosEnanos = Val(GetVar(IniPath & "Server.ini", "INIT", "TunicaMagoCaosEnanos"))

        VestimentaImperialHumano = Val(GetVar(IniPath & "Server.ini", "INIT", "VestimentaImperialHumano"))
        VestimentaImperialEnano = Val(GetVar(IniPath & "Server.ini", "INIT", "VestimentaImperialEnano"))
        TunicaConspicuaHumano = Val(GetVar(IniPath & "Server.ini", "INIT", "TunicaConspicuaHumano"))
        TunicaConspicuaEnano = Val(GetVar(IniPath & "Server.ini", "INIT", "TunicaConspicuaEnano"))
        ArmaduraNobilisimaHumano = Val(GetVar(IniPath & "Server.ini", "INIT", "ArmaduraNobilisimaHumano"))
        ArmaduraNobilisimaEnano = Val(GetVar(IniPath & "Server.ini", "INIT", "ArmaduraNobilisimaEnano"))
        ArmaduraGranSacerdote = Val(GetVar(IniPath & "Server.ini", "INIT", "ArmaduraGranSacerdote"))

        VestimentaLegionHumano = Val(GetVar(IniPath & "Server.ini", "INIT", "VestimentaLegionHumano"))
        VestimentaLegionEnano = Val(GetVar(IniPath & "Server.ini", "INIT", "VestimentaLegionEnano"))
        TunicaLobregaHumano = Val(GetVar(IniPath & "Server.ini", "INIT", "TunicaLobregaHumano"))
        TunicaLobregaEnano = Val(GetVar(IniPath & "Server.ini", "INIT", "TunicaLobregaEnano"))
        TunicaEgregiaHumano = Val(GetVar(IniPath & "Server.ini", "INIT", "TunicaEgregiaHumano"))
        TunicaEgregiaEnano = Val(GetVar(IniPath & "Server.ini", "INIT", "TunicaEgregiaEnano"))
        SacerdoteDemoniaco = Val(GetVar(IniPath & "Server.ini", "INIT", "SacerdoteDemoniaco"))

        MAPA_PRETORIANO = Val(GetVar(IniPath & "Server.ini", "INIT", "MapaPretoriano"))

        EnTesting = Val(GetVar(IniPath & "Server.ini", "INIT", "Testing"))

        'Intervalos
        SanaIntervaloSinDescansar = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "SanaIntervaloSinDescansar"))
        StaminaIntervaloSinDescansar = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "StaminaIntervaloSinDescansar"))
        SanaIntervaloDescansar = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "SanaIntervaloDescansar"))
        StaminaIntervaloDescansar = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "StaminaIntervaloDescansar"))

        IntervaloSed = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloSed"))
        IntervaloHambre = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloHambre"))
        IntervaloVeneno = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloVeneno"))
        IntervaloParalizado = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloParalizado"))
        IntervaloInvisible = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloInvisible"))
        IntervaloFrio = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloFrio"))
        IntervaloWavFx = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloWAVFX"))
        IntervaloInvocacion = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloInvocacion"))
        IntervaloParaConexion = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloParaConexion"))

        '&&&&&&&&&&&&&&&&&&&&& TIMERS &&&&&&&&&&&&&&&&&&&&&&&

        IntervaloPuedeSerAtacado = 5000 ' Cargar desde balance.dat
        IntervaloAtacable = 60000 ' Cargar desde balance.dat
        IntervaloOwnedNpc = 18000 ' Cargar desde balance.dat

        IntervaloUserPuedeCastear = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloLanzaHechizo"))

        'UPGRADE_WARNING: La propiedad Timer TIMER_AI.Interval no puede tener un valor de 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="169ECF4A-1968-402D-B243-16603CC08604"'
        timerAIInterval = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloNpcAI"))

        'UPGRADE_WARNING: La propiedad Timer npcataca.Interval no puede tener un valor de 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="169ECF4A-1968-402D-B243-16603CC08604"'
        npcAtacaInterval = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloNpcPuedeAtacar"))

        IntervaloUserPuedeTrabajar = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloTrabajo"))

        IntervaloUserPuedeAtacar = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloUserPuedeAtacar"))

        'TODO : Agregar estos intervalos al form!!!
        IntervaloMagiaGolpe = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloMagiaGolpe"))
        IntervaloGolpeMagia = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloGolpeMagia"))
        IntervaloGolpeUsar = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloGolpeUsar"))

        'UPGRADE_WARNING: La propiedad Timer tLluvia.Interval no puede tener un valor de 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="169ECF4A-1968-402D-B243-16603CC08604"'
        lluviaInterval = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloPerdidaStaminaLluvia"))

        MinutosWs = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloWS"))
        If MinutosWs < 60 Then MinutosWs = 180

        IntervaloCerrarConexion = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloCerrarConexion"))
        IntervaloUserPuedeUsar = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloUserPuedeUsar"))
        IntervaloFlechasCazadores = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloFlechasCazadores"))

        IntervaloOculto = Val(GetVar(IniPath & "Server.ini", "INTERVALOS", "IntervaloOculto"))

        '&&&&&&&&&&&&&&&&&&&&& FIN TIMERS &&&&&&&&&&&&&&&&&&&&&&&

        recordusuarios = Val(GetVar(IniPath & "Server.ini", "INIT", "Record"))

        'Max users
        Temporal = Val(GetVar(IniPath & "Server.ini", "INIT", "MaxUsers"))
        If MaxUsers = 0 Then
            MaxUsers = Temporal
            'UPGRADE_WARNING: Es posible que la matriz UserList necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
            'UPGRADE_WARNING: El límite inferior de la matriz UserList ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
            ReDim UserList(MaxUsers)
            InitializeStruct(UserList)
        End If

        '&&&&&&&&&&&&&&&&&&&&& BALANCE &&&&&&&&&&&&&&&&&&&&&&&
        'Se agregó en LoadBalance y en el Balance.dat
        'PorcentajeRecuperoMana = val(GetVar(IniPath & "Server.ini", "BALANCE", "PorcentajeRecuperoMana"))

        ''&&&&&&&&&&&&&&&&&&&&& FIN BALANCE &&&&&&&&&&&&&&&&&&&&&&&
        Call Initialize()

        Ullathorpe.Map = CShort(GetVar(DatPath & "Ciudades.dat", "Ullathorpe", "Mapa"))
        Ullathorpe.X = CShort(GetVar(DatPath & "Ciudades.dat", "Ullathorpe", "X"))
        Ullathorpe.Y = CShort(GetVar(DatPath & "Ciudades.dat", "Ullathorpe", "Y"))

        Nix.Map = CShort(GetVar(DatPath & "Ciudades.dat", "Nix", "Mapa"))
        Nix.X = CShort(GetVar(DatPath & "Ciudades.dat", "Nix", "X"))
        Nix.Y = CShort(GetVar(DatPath & "Ciudades.dat", "Nix", "Y"))

        Banderbill.Map = CShort(GetVar(DatPath & "Ciudades.dat", "Banderbill", "Mapa"))
        Banderbill.X = CShort(GetVar(DatPath & "Ciudades.dat", "Banderbill", "X"))
        Banderbill.Y = CShort(GetVar(DatPath & "Ciudades.dat", "Banderbill", "Y"))

        Lindos.Map = CShort(GetVar(DatPath & "Ciudades.dat", "Lindos", "Mapa"))
        Lindos.X = CShort(GetVar(DatPath & "Ciudades.dat", "Lindos", "X"))
        Lindos.Y = CShort(GetVar(DatPath & "Ciudades.dat", "Lindos", "Y"))

        Arghal.Map = CShort(GetVar(DatPath & "Ciudades.dat", "Arghal", "Mapa"))
        Arghal.X = CShort(GetVar(DatPath & "Ciudades.dat", "Arghal", "X"))
        Arghal.Y = CShort(GetVar(DatPath & "Ciudades.dat", "Arghal", "Y"))

        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Ciudades(eCiudad.cUllathorpe). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        Ciudades(eCiudad.cUllathorpe) = Ullathorpe
        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Ciudades(eCiudad.cNix). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        Ciudades(eCiudad.cNix) = Nix
        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Ciudades(eCiudad.cBanderbill). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        Ciudades(eCiudad.cBanderbill) = Banderbill
        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Ciudades(eCiudad.cLindos). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        Ciudades(eCiudad.cLindos) = Lindos
        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Ciudades(eCiudad.cArghal). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        Ciudades(eCiudad.cArghal) = Arghal

        Call MD5sCarga()

        Call ConsultaPopular.LoadData()
    End Sub

    ' TODO MIGRA: funciona pero es lento e ineficiente
    Public Sub WriteVar(fileName As String, Main As String, Var As String, Value As String)
        ' Reemplazamos Collection con estructuras modernas
        Dim sectionNames As New List(Of String)() ' Para guardar el orden de aparición de secciones
        Dim sectionData As New Dictionary(Of String, Dictionary(Of String, String))(StringComparer.OrdinalIgnoreCase) _
        ' Secciones y sus claves/valores

        Dim currentSection As String
        Dim line As String
        Dim fileContent As String
        Dim rawLines() As String

        ' --- Leer archivo si existe ---
        If File.Exists(fileName) Then
            fileContent = File.ReadAllText(fileName)
        Else
            fileContent = "" ' Archivo no existe, se creará
        End If

        ' --- Separar en líneas ---
        If Not String.IsNullOrEmpty(fileContent) Then
            rawLines = fileContent.Split(New String() {vbCrLf}, StringSplitOptions.None)
        Else
            ReDim rawLines(0)
            rawLines(0) = ""
        End If

        ' --- Inicializar sección actual como vacío ---
        currentSection = ""

        ' -----------------------------------------------------------------------------
        ' 1) Parsear todo el archivo en memoria: secciones y claves
        '    - Al encontrar una nueva sección, si ya existe en 'sectionNames', seguimos usando la misma
        '      para no duplicarla, de lo contrario se crea.
        '    - Al encontrar "Clave=Valor", se guarda/actualiza en el diccionario de la sección actual.
        '    - Se ignoran líneas vacías.
        ' -----------------------------------------------------------------------------
        Dim posEqual As Integer
        Dim keyName As String
        Dim keyValue As String

        For i = 0 To rawLines.Length - 1
            line = rawLines(i).Trim()
            If Not String.IsNullOrEmpty(line) Then
                If line.StartsWith("[") AndAlso line.EndsWith("]") Then
                    ' Sección
                    currentSection = line.Substring(1, line.Length - 2)

                    ' Verificar si la sección ya existe
                    If Not sectionNames.Contains(currentSection, StringComparer.OrdinalIgnoreCase) Then
                        ' Crear la sección y su diccionario
                        sectionNames.Add(currentSection) ' se guarda en el orden en que aparece
                        sectionData.Add(currentSection,
                                        New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase))
                    End If

                ElseIf Not String.IsNullOrEmpty(currentSection) Then
                    ' Estamos dentro de una sección, parsear "clave=valor"
                    posEqual = line.IndexOf("=")
                    If posEqual > 0 Then
                        keyName = line.Substring(0, posEqual).Trim()
                        keyValue = line.Substring(posEqual + 1)

                        ' Actualizar o agregar la clave
                        If sectionData(currentSection).ContainsKey(keyName) Then
                            sectionData(currentSection)(keyName) = keyValue
                        Else
                            sectionData(currentSection).Add(keyName, keyValue)
                        End If
                    End If
                End If
            End If
        Next

        ' -----------------------------------------------------------------------------
        ' 2) Agregar/actualizar la clave solicitada en la sección "Main"
        '    - Si la sección no existe, se crea.
        '    - Se actualiza (o se crea) la Key=Value especificada.
        ' -----------------------------------------------------------------------------
        ' Verificar si la sección principal existe
        If Not sectionData.ContainsKey(Main) Then
            sectionNames.Add(Main)
            sectionData.Add(Main, New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase))
        End If

        ' Actualizar o crear la clave Var=Value dentro de la sección Main
        If sectionData(Main).ContainsKey(Var) Then
            sectionData(Main)(Var) = Value
        Else
            sectionData(Main).Add(Var, Value)
        End If

        ' -----------------------------------------------------------------------------
        ' 3) Reconstruir el archivo .ini: no debe haber secciones duplicadas, sin líneas en blanco
        ' -----------------------------------------------------------------------------
        Dim outputLines As New List(Of String)()

        ' Procesar las secciones en el orden en que se encontraron
        For Each secName As String In sectionNames
            ' Escribir la sección
            outputLines.Add("[" & secName & "]")

            ' Escribir las claves/valores
            For Each kvp As KeyValuePair(Of String, String) In sectionData(secName)
                outputLines.Add(kvp.Key & "=" & kvp.Value)
            Next
        Next

        ' Si no hay contenido, crear la sección y clave por defecto
        If outputLines.Count = 0 Then
            outputLines.Add("[" & Main & "]")
            outputLines.Add(Var & "=" & Value)
        End If

        ' -----------------------------------------------------------------------------
        ' 4) Escribir el archivo resultante
        ' -----------------------------------------------------------------------------
        fileContent = String.Join(vbCrLf, outputLines.ToArray())
        File.WriteAllText(fileName, fileContent)
    End Sub

    Sub SaveUser(UserIndex As Short, UserFile As String)
        '*************************************************
        'Author: Unknown
        'Last modified: 12/01/2010 (ZaMa)
        'Saves the Users records
        '23/01/2007 Pablo (ToxicWaste) - Agrego NivelIngreso, FechaIngreso, MatadosIngreso y NextRecompensa.
        '11/19/2009: Pato - Save the EluSkills and ExpSkills
        '12/01/2010: ZaMa - Los druidas pierden la inmunidad de ser atacados cuando pierden el efecto del mimetismo.
        '*************************************************

        Try

            Dim OldUserHead As Integer

            Dim LoopC As Short
            Dim TempDate As Date
            Dim i As Short
            Dim loopd As Short
            Dim L As Integer
            Dim cad As String
            Dim NroMascotas As Integer
            With UserList(UserIndex)

                'ESTO TIENE QUE EVITAR ESE BUGAZO QUE NO SE POR QUE GRABA USUARIOS NULOS
                'clase=0 es el error, porq el enum empieza de 1!!
                If .clase = 0 Or .Stats.ELV = 0 Then
                    Call LogCriticEvent("Estoy intentantdo guardar un usuario nulo de nombre: " & .name)
                    Exit Sub
                End If


                If .flags.Mimetizado = 1 Then
                    .Char_Renamed.body = .CharMimetizado.body
                    .Char_Renamed.Head = .CharMimetizado.Head
                    .Char_Renamed.CascoAnim = .CharMimetizado.CascoAnim
                    .Char_Renamed.ShieldAnim = .CharMimetizado.ShieldAnim
                    .Char_Renamed.WeaponAnim = .CharMimetizado.WeaponAnim
                    .Counters.Mimetismo = 0
                    .flags.Mimetizado = 0
                    ' Se fue el efecto del mimetismo, puede ser atacado por npcs
                    .flags.Ignorado = False
                End If

                If FileExist(UserFile) Then
                    If .flags.Muerto = 1 Then
                        OldUserHead = .Char_Renamed.Head
                        .Char_Renamed.Head = CShort(GetVar(UserFile, "INIT", "Head"))
                    End If
                    '       Kill UserFile
                End If


                Call WriteVar(UserFile, "FLAGS", "Muerto", CStr(.flags.Muerto))
                Call WriteVar(UserFile, "FLAGS", "Escondido", CStr(.flags.Escondido))
                Call WriteVar(UserFile, "FLAGS", "Hambre", CStr(.flags.Hambre))
                Call WriteVar(UserFile, "FLAGS", "Sed", CStr(.flags.Sed))
                Call WriteVar(UserFile, "FLAGS", "Desnudo", CStr(.flags.Desnudo))
                Call WriteVar(UserFile, "FLAGS", "Ban", CStr(.flags.Ban))
                Call WriteVar(UserFile, "FLAGS", "Navegando", CStr(.flags.Navegando))
                Call WriteVar(UserFile, "FLAGS", "Envenenado", CStr(.flags.Envenenado))
                Call WriteVar(UserFile, "FLAGS", "Paralizado", CStr(.flags.Paralizado))
                'Matrix
                Call WriteVar(UserFile, "FLAGS", "LastMap", CStr(.flags.lastMap))

                Call _
                    WriteVar(UserFile, "CONSEJO", "PERTENECE",
                             IIf(.flags.Privilegios And PlayerType.RoyalCouncil, "1", "0"))
                Call _
                    WriteVar(UserFile, "CONSEJO", "PERTENECECAOS",
                             IIf(.flags.Privilegios And PlayerType.ChaosCouncil, "1", "0"))


                Call WriteVar(UserFile, "COUNTERS", "Pena", CStr(.Counters.Pena))
                Call WriteVar(UserFile, "COUNTERS", "SkillsAsignados", CStr(.Counters.AsignedSkills))

                Call WriteVar(UserFile, "FACCIONES", "EjercitoReal", CStr(.Faccion.ArmadaReal))
                Call WriteVar(UserFile, "FACCIONES", "EjercitoCaos", CStr(.Faccion.FuerzasCaos))
                Call WriteVar(UserFile, "FACCIONES", "CiudMatados", CStr(.Faccion.CiudadanosMatados))
                Call WriteVar(UserFile, "FACCIONES", "CrimMatados", CStr(.Faccion.CriminalesMatados))
                Call WriteVar(UserFile, "FACCIONES", "rArCaos", CStr(.Faccion.RecibioArmaduraCaos))
                Call WriteVar(UserFile, "FACCIONES", "rArReal", CStr(.Faccion.RecibioArmaduraReal))
                Call WriteVar(UserFile, "FACCIONES", "rExCaos", CStr(.Faccion.RecibioExpInicialCaos))
                Call WriteVar(UserFile, "FACCIONES", "rExReal", CStr(.Faccion.RecibioExpInicialReal))
                Call WriteVar(UserFile, "FACCIONES", "recCaos", CStr(.Faccion.RecompensasCaos))
                Call WriteVar(UserFile, "FACCIONES", "recReal", CStr(.Faccion.RecompensasReal))
                Call WriteVar(UserFile, "FACCIONES", "Reenlistadas", CStr(.Faccion.Reenlistadas))
                Call WriteVar(UserFile, "FACCIONES", "NivelIngreso", CStr(.Faccion.NivelIngreso))
                Call WriteVar(UserFile, "FACCIONES", "FechaIngreso", .Faccion.FechaIngreso)
                Call WriteVar(UserFile, "FACCIONES", "MatadosIngreso", CStr(.Faccion.MatadosIngreso))
                Call WriteVar(UserFile, "FACCIONES", "NextRecompensa", CStr(.Faccion.NextRecompensa))


                '¿Fueron modificados los atributos del usuario?
                If Not .flags.TomoPocion Then
                    For LoopC = 1 To UBound(.Stats.UserAtributos)
                        Call WriteVar(UserFile, "ATRIBUTOS", "AT" & LoopC, CStr(.Stats.UserAtributos(LoopC)))
                    Next LoopC
                Else
                    For LoopC = 1 To UBound(.Stats.UserAtributos)
                        '.Stats.UserAtributos(LoopC) = .Stats.UserAtributosBackUP(LoopC)
                        Call WriteVar(UserFile, "ATRIBUTOS", "AT" & LoopC, CStr(.Stats.UserAtributosBackUP(LoopC)))
                    Next LoopC
                End If

                For LoopC = 1 To UBound(.Stats.UserSkills)
                    Call WriteVar(UserFile, "SKILLS", "SK" & LoopC, CStr(.Stats.UserSkills(LoopC)))
                    Call WriteVar(UserFile, "SKILLS", "ELUSK" & LoopC, CStr(.Stats.EluSkills(LoopC)))
                    Call WriteVar(UserFile, "SKILLS", "EXPSK" & LoopC, CStr(.Stats.ExpSkills(LoopC)))
                Next LoopC


                Call WriteVar(UserFile, "CONTACTO", "Email", .email)

                Call WriteVar(UserFile, "INIT", "Genero", CStr(.Genero))
                Call WriteVar(UserFile, "INIT", "Raza", CStr(.raza))
                Call WriteVar(UserFile, "INIT", "Hogar", CStr(.Hogar))
                Call WriteVar(UserFile, "INIT", "Clase", CStr(.clase))
                Call WriteVar(UserFile, "INIT", "Desc", .desc)

                Call WriteVar(UserFile, "INIT", "Heading", CStr(.Char_Renamed.heading))

                Call WriteVar(UserFile, "INIT", "Head", CStr(.OrigChar.Head))

                If .flags.Muerto = 0 Then
                    Call WriteVar(UserFile, "INIT", "Body", CStr(.Char_Renamed.body))
                End If

                Call WriteVar(UserFile, "INIT", "Arma", CStr(.Char_Renamed.WeaponAnim))
                Call WriteVar(UserFile, "INIT", "Escudo", CStr(.Char_Renamed.ShieldAnim))
                Call WriteVar(UserFile, "INIT", "Casco", CStr(.Char_Renamed.CascoAnim))

                TempDate = DateTime.FromOADate(Now.ToOADate - .LogOnTime.ToOADate)
                .LogOnTime = Now
                .UpTime = .UpTime + (Math.Abs(TempDate.Day - 30)*24*3600) + Hour(TempDate)*3600 + Minute(TempDate)*60 +
                          Second(TempDate)
                .UpTime = .UpTime
                Call WriteVar(UserFile, "INIT", "UpTime", CStr(.UpTime))

                'First time around?
                If GetVar(UserFile, "INIT", "LastIP1") = vbNullString Then
                    Call WriteVar(UserFile, "INIT", "LastIP1", .ip & " - " & Today & ":" & TimeOfDay)
                    'Is it a different ip from last time?
                ElseIf _
                    .ip <>
                    Left(GetVar(UserFile, "INIT", "LastIP1"), InStr(1, GetVar(UserFile, "INIT", "LastIP1"), " ") - 1) _
                    Then
                    For i = 5 To 2 Step - 1
                        Call WriteVar(UserFile, "INIT", "LastIP" & i, GetVar(UserFile, "INIT", "LastIP" & CStr(i - 1)))
                    Next i
                    Call WriteVar(UserFile, "INIT", "LastIP1", .ip & " - " & Today & ":" & TimeOfDay)
                    'Same ip, just update the date
                Else
                    Call WriteVar(UserFile, "INIT", "LastIP1", .ip & " - " & Today & ":" & TimeOfDay)
                End If

                Call WriteVar(UserFile, "INIT", "Position", .Pos.Map & "-" & .Pos.X & "-" & .Pos.Y)


                Call WriteVar(UserFile, "STATS", "GLD", CStr(.Stats.GLD))
                Call WriteVar(UserFile, "STATS", "BANCO", CStr(.Stats.Banco))

                Call WriteVar(UserFile, "STATS", "MaxHP", CStr(.Stats.MaxHp))
                Call WriteVar(UserFile, "STATS", "MinHP", CStr(.Stats.MinHp))

                Call WriteVar(UserFile, "STATS", "MaxSTA", CStr(.Stats.MaxSta))
                Call WriteVar(UserFile, "STATS", "MinSTA", CStr(.Stats.MinSta))

                Call WriteVar(UserFile, "STATS", "MaxMAN", CStr(.Stats.MaxMAN))
                Call WriteVar(UserFile, "STATS", "MinMAN", CStr(.Stats.MinMAN))

                Call WriteVar(UserFile, "STATS", "MaxHIT", CStr(.Stats.MaxHIT))
                Call WriteVar(UserFile, "STATS", "MinHIT", CStr(.Stats.MinHIT))

                Call WriteVar(UserFile, "STATS", "MaxAGU", CStr(.Stats.MaxAGU))
                Call WriteVar(UserFile, "STATS", "MinAGU", CStr(.Stats.MinAGU))

                Call WriteVar(UserFile, "STATS", "MaxHAM", CStr(.Stats.MaxHam))
                Call WriteVar(UserFile, "STATS", "MinHAM", CStr(.Stats.MinHam))

                Call WriteVar(UserFile, "STATS", "SkillPtsLibres", CStr(.Stats.SkillPts))

                Call WriteVar(UserFile, "STATS", "EXP", CStr(.Stats.Exp))
                Call WriteVar(UserFile, "STATS", "ELV", CStr(.Stats.ELV))


                Call WriteVar(UserFile, "STATS", "ELU", CStr(.Stats.ELU))
                Call WriteVar(UserFile, "MUERTES", "UserMuertes", CStr(.Stats.UsuariosMatados))
                'Call WriteVar(UserFile, "MUERTES", "CrimMuertes", CStr(.Stats.CriminalesMatados))
                Call WriteVar(UserFile, "MUERTES", "NpcsMuertes", CStr(.Stats.NPCsMuertos))

                '[KEVIN]----------------------------------------------------------------------------
                '*******************************************************************************************
                Call WriteVar(UserFile, "BancoInventory", "CantidadItems", CStr(Val(CStr(.BancoInvent.NroItems))))
                For loopd = 1 To MAX_BANCOINVENTORY_SLOTS
                    Call _
                        WriteVar(UserFile, "BancoInventory", "Obj" & loopd,
                                 .BancoInvent.Object_Renamed(loopd).ObjIndex & "-" &
                                 .BancoInvent.Object_Renamed(loopd).Amount)
                Next loopd
                '*******************************************************************************************
                '[/KEVIN]-----------

                'Save Inv
                Call WriteVar(UserFile, "Inventory", "CantidadItems", CStr(Val(CStr(.Invent.NroItems))))

                For LoopC = 1 To MAX_INVENTORY_SLOTS
                    Call _
                        WriteVar(UserFile, "Inventory", "Obj" & LoopC,
                                 .Invent.Object_Renamed(LoopC).ObjIndex & "-" & .Invent.Object_Renamed(LoopC).Amount &
                                 "-" &
                                 .Invent.Object_Renamed(LoopC).Equipped)
                Next LoopC

                Call WriteVar(UserFile, "Inventory", "WeaponEqpSlot", CStr(.Invent.WeaponEqpSlot))
                Call WriteVar(UserFile, "Inventory", "ArmourEqpSlot", CStr(.Invent.ArmourEqpSlot))
                Call WriteVar(UserFile, "Inventory", "CascoEqpSlot", CStr(.Invent.CascoEqpSlot))
                Call WriteVar(UserFile, "Inventory", "EscudoEqpSlot", CStr(.Invent.EscudoEqpSlot))
                Call WriteVar(UserFile, "Inventory", "BarcoSlot", CStr(.Invent.BarcoSlot))
                Call WriteVar(UserFile, "Inventory", "MunicionSlot", CStr(.Invent.MunicionEqpSlot))
                Call WriteVar(UserFile, "Inventory", "MochilaSlot", CStr(.Invent.MochilaEqpSlot))
                '/Nacho

                Call WriteVar(UserFile, "Inventory", "AnilloSlot", CStr(.Invent.AnilloEqpSlot))


                'Reputacion
                Call WriteVar(UserFile, "REP", "Asesino", CStr(.Reputacion.AsesinoRep))
                Call WriteVar(UserFile, "REP", "Bandido", CStr(.Reputacion.BandidoRep))
                Call WriteVar(UserFile, "REP", "Burguesia", CStr(.Reputacion.BurguesRep))
                Call WriteVar(UserFile, "REP", "Ladrones", CStr(.Reputacion.LadronesRep))
                Call WriteVar(UserFile, "REP", "Nobles", CStr(.Reputacion.NobleRep))
                Call WriteVar(UserFile, "REP", "Plebe", CStr(.Reputacion.PlebeRep))

                L = (- .Reputacion.AsesinoRep) + (- .Reputacion.BandidoRep) + .Reputacion.BurguesRep +
                    (- .Reputacion.LadronesRep) + .Reputacion.NobleRep + .Reputacion.PlebeRep
                L = L/6
                Call WriteVar(UserFile, "REP", "Promedio", CStr(L))


                For LoopC = 1 To MAXUSERHECHIZOS
                    cad = CStr(.Stats.UserHechizos(LoopC))
                    Call WriteVar(UserFile, "HECHIZOS", "H" & LoopC, cad)
                Next

                NroMascotas = .NroMascotas

                For LoopC = 1 To MAXMASCOTAS
                    ' Mascota valida?
                    If .MascotasIndex(LoopC) > 0 Then
                        ' Nos aseguramos que la criatura no fue invocada
                        If Npclist(.MascotasIndex(LoopC)).Contadores.TiempoExistencia = 0 Then
                            cad = CStr(.MascotasType(LoopC))
                        Else 'Si fue invocada no la guardamos
                            cad = "0"
                            NroMascotas = NroMascotas - 1
                        End If
                        Call WriteVar(UserFile, "MASCOTAS", "MAS" & LoopC, cad)
                    Else
                        cad = CStr(.MascotasType(LoopC))
                        Call WriteVar(UserFile, "MASCOTAS", "MAS" & LoopC, cad)
                    End If

                Next

                Call WriteVar(UserFile, "MASCOTAS", "NroMascotas", CStr(NroMascotas))

                'Devuelve el head de muerto
                If .flags.Muerto = 1 Then
                    .Char_Renamed.Head = iCabezaMuerto
                End If
            End With


        Catch ex As Exception
            Console.WriteLine("Error in LoadSini: " & ex.Message)
            Call LogError("Error en SaveUser")
        End Try
    End Sub

    Function criminal(UserIndex As Short) As Boolean
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim L As Integer

        With UserList(UserIndex).Reputacion
            L = (- .AsesinoRep) + (- .BandidoRep) + .BurguesRep + (- .LadronesRep) + .NobleRep + .PlebeRep
            L = L/6
            criminal = (L < 0)
        End With
    End Function

    Sub BackUPnPc(ByRef NpcIndex As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim NpcNumero As Short
        Dim npcfile As String
        Dim LoopC As Short


        NpcNumero = Npclist(NpcIndex).Numero

        'If NpcNumero > 499 Then
        '    npcfile = DatPath & "bkNPCs-HOSTILES.dat"
        'Else
        npcfile = DatPath & "bkNPCs.dat"
        'End If

        With Npclist(NpcIndex)
            'General
            Call WriteVar(npcfile, "NPC" & NpcNumero, "Name", .name)
            Call WriteVar(npcfile, "NPC" & NpcNumero, "Desc", .desc)
            Call WriteVar(npcfile, "NPC" & NpcNumero, "Head", CStr(Val(CStr(.Char_Renamed.Head))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "Body", CStr(Val(CStr(.Char_Renamed.body))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "Heading", CStr(Val(CStr(.Char_Renamed.heading))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "Movement", CStr(Val(CStr(.Movement))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "Attackable", CStr(Val(CStr(.Attackable))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "Comercia", CStr(Val(CStr(.Comercia))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "TipoItems", CStr(Val(CStr(.TipoItems))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "Hostil", CStr(Val(CStr(.Hostile))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "GiveEXP", CStr(Val(CStr(.GiveEXP))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "GiveGLD", CStr(Val(CStr(.GiveGLD))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "Hostil", CStr(Val(CStr(.Hostile))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "InvReSpawn", CStr(Val(CStr(.InvReSpawn))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "NpcType", CStr(Val(CStr(.NPCtype))))


            'Stats
            Call WriteVar(npcfile, "NPC" & NpcNumero, "Alineacion", CStr(Val(CStr(.Stats.Alineacion))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "DEF", CStr(Val(CStr(.Stats.def))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "MaxHit", CStr(Val(CStr(.Stats.MaxHIT))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "MaxHp", CStr(Val(CStr(.Stats.MaxHp))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "MinHit", CStr(Val(CStr(.Stats.MinHIT))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "MinHp", CStr(Val(CStr(.Stats.MinHp))))


            'Flags
            Call WriteVar(npcfile, "NPC" & NpcNumero, "ReSpawn", CStr(Val(CStr(.flags.Respawn))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "BackUp", CStr(Val(CStr(.flags.BackUp))))
            Call WriteVar(npcfile, "NPC" & NpcNumero, "Domable", CStr(Val(CStr(.flags.Domable))))

            'Inventario
            Call WriteVar(npcfile, "NPC" & NpcNumero, "NroItems", CStr(Val(CStr(.Invent.NroItems))))
            If .Invent.NroItems > 0 Then
                For LoopC = 1 To MAX_INVENTORY_SLOTS
                    Call _
                        WriteVar(npcfile, "NPC" & NpcNumero, "Obj" & LoopC,
                                 .Invent.Object_Renamed(LoopC).ObjIndex & "-" & .Invent.Object_Renamed(LoopC).Amount)
                Next LoopC
            End If
        End With
    End Sub

    Sub CargarNpcBackUp(ByRef NpcIndex As Short, NpcNumber As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim npcfile As String

        'If NpcNumber > 499 Then
        '    npcfile = DatPath & "bkNPCs-HOSTILES.dat"
        'Else
        npcfile = DatPath & "bkNPCs.dat"
        'End If

        Dim LoopC As Short
        Dim ln As String
        With Npclist(NpcIndex)

            .Numero = NpcNumber
            .name = GetVar(npcfile, "NPC" & NpcNumber, "Name")
            .desc = GetVar(npcfile, "NPC" & NpcNumber, "Desc")
            .Movement = Val(GetVar(npcfile, "NPC" & NpcNumber, "Movement"))
            .NPCtype = Val(GetVar(npcfile, "NPC" & NpcNumber, "NpcType"))

            .Char_Renamed.body = Val(GetVar(npcfile, "NPC" & NpcNumber, "Body"))
            .Char_Renamed.Head = Val(GetVar(npcfile, "NPC" & NpcNumber, "Head"))
            .Char_Renamed.heading = Val(GetVar(npcfile, "NPC" & NpcNumber, "Heading"))

            .Attackable = Val(GetVar(npcfile, "NPC" & NpcNumber, "Attackable"))
            .Comercia = Val(GetVar(npcfile, "NPC" & NpcNumber, "Comercia"))
            .Hostile = Val(GetVar(npcfile, "NPC" & NpcNumber, "Hostile"))
            .GiveEXP = Val(GetVar(npcfile, "NPC" & NpcNumber, "GiveEXP"))


            .GiveGLD = Val(GetVar(npcfile, "NPC" & NpcNumber, "GiveGLD"))

            .InvReSpawn = Val(GetVar(npcfile, "NPC" & NpcNumber, "InvReSpawn"))

            .Stats.MaxHp = Val(GetVar(npcfile, "NPC" & NpcNumber, "MaxHP"))
            .Stats.MinHp = Val(GetVar(npcfile, "NPC" & NpcNumber, "MinHP"))
            .Stats.MaxHIT = Val(GetVar(npcfile, "NPC" & NpcNumber, "MaxHIT"))
            .Stats.MinHIT = Val(GetVar(npcfile, "NPC" & NpcNumber, "MinHIT"))
            .Stats.def = Val(GetVar(npcfile, "NPC" & NpcNumber, "DEF"))
            .Stats.Alineacion = Val(GetVar(npcfile, "NPC" & NpcNumber, "Alineacion"))


            .Invent.NroItems = Val(GetVar(npcfile, "NPC" & NpcNumber, "NROITEMS"))
            If .Invent.NroItems > 0 Then
                For LoopC = 1 To MAX_INVENTORY_SLOTS
                    ln = GetVar(npcfile, "NPC" & NpcNumber, "Obj" & LoopC)
                    .Invent.Object_Renamed(LoopC).ObjIndex = Val(ReadField(1, ln, 45))
                    .Invent.Object_Renamed(LoopC).Amount = Val(ReadField(2, ln, 45))

                Next LoopC
            Else
                For LoopC = 1 To MAX_INVENTORY_SLOTS
                    .Invent.Object_Renamed(LoopC).ObjIndex = 0
                    .Invent.Object_Renamed(LoopC).Amount = 0
                Next LoopC
            End If

            For LoopC = 1 To MAX_NPC_DROPS
                ln = GetVar(npcfile, "NPC" & NpcNumber, "Drop" & LoopC)
                .Drop(LoopC).ObjIndex = Val(ReadField(1, ln, 45))
                .Drop(LoopC).Amount = Val(ReadField(2, ln, 45))
            Next LoopC

            .flags.NPCActive = True
            .flags.Respawn = Val(GetVar(npcfile, "NPC" & NpcNumber, "ReSpawn"))
            .flags.BackUp = Val(GetVar(npcfile, "NPC" & NpcNumber, "BackUp"))
            .flags.Domable = Val(GetVar(npcfile, "NPC" & NpcNumber, "Domable"))
            .flags.RespawnOrigPos = Val(GetVar(npcfile, "NPC" & NpcNumber, "OrigPos"))

            'Tipo de items con los que comercia
            .TipoItems = Val(GetVar(npcfile, "NPC" & NpcNumber, "TipoItems"))
        End With
    End Sub


    Sub LogBan(BannedIndex As Short, UserIndex As Short, motivo As String)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Call _
            WriteVar(AppDomain.CurrentDomain.BaseDirectory & "logs/" & "BanDetail.log", UserList(BannedIndex).name,
                     "BannedBy", UserList(UserIndex).name)
        Call _
            WriteVar(AppDomain.CurrentDomain.BaseDirectory & "logs/" & "BanDetail.log", UserList(BannedIndex).name,
                     "Reason", motivo)

        'Log interno del servidor, lo usa para hacer un UNBAN general de toda la gente banned
        Dim mifile As Short
        mifile = FreeFile
        FileOpen(mifile, AppDomain.CurrentDomain.BaseDirectory & "logs/GenteBanned.log", OpenMode.Append, ,
                 OpenShare.Shared)
        PrintLine(mifile, UserList(BannedIndex).name)
        FileClose(mifile)
    End Sub


    Sub LogBanFromName(BannedName As String, UserIndex As Short, motivo As String)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Call _
            WriteVar(AppDomain.CurrentDomain.BaseDirectory & "logs/" & "BanDetail.dat", BannedName, "BannedBy",
                     UserList(UserIndex).name)
        Call WriteVar(AppDomain.CurrentDomain.BaseDirectory & "logs/" & "BanDetail.dat", BannedName, "Reason", motivo)

        'Log interno del servidor, lo usa para hacer un UNBAN general de toda la gente banned
        Dim mifile As Short
        mifile = FreeFile
        FileOpen(mifile, AppDomain.CurrentDomain.BaseDirectory & "logs/GenteBanned.log", OpenMode.Append, ,
                 OpenShare.Shared)
        PrintLine(mifile, BannedName)
        FileClose(mifile)
    End Sub


    Sub Ban(BannedName As String, Baneador As String, motivo As String)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Call _
            WriteVar(AppDomain.CurrentDomain.BaseDirectory & "logs/" & "BanDetail.dat", BannedName, "BannedBy", Baneador)
        Call WriteVar(AppDomain.CurrentDomain.BaseDirectory & "logs/" & "BanDetail.dat", BannedName, "Reason", motivo)


        'Log interno del servidor, lo usa para hacer un UNBAN general de toda la gente banned
        Dim mifile As Short
        mifile = FreeFile
        FileOpen(mifile, AppDomain.CurrentDomain.BaseDirectory & "logs/GenteBanned.log", OpenMode.Append, ,
                 OpenShare.Shared)
        PrintLine(mifile, BannedName)
        FileClose(mifile)
    End Sub

    Public Sub CargaApuestas()
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Apuestas.Ganancias = Val(GetVar(DatPath & "apuestas.dat", "Main", "Ganancias"))
        Apuestas.Perdidas = Val(GetVar(DatPath & "apuestas.dat", "Main", "Perdidas"))
        Apuestas.Jugadas = Val(GetVar(DatPath & "apuestas.dat", "Main", "Jugadas"))
    End Sub

    Public Sub generateMatrix(mapa As Short)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim i As Short
        Dim j As Short
        Dim X As Short
        Dim Y As Short

        'UPGRADE_WARNING: Es posible que la matriz distanceToCities necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
        'UPGRADE_WARNING: El límite inferior de la matriz distanceToCities ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
        ReDim distanceToCities(NumMaps)
        InitializeStruct(distanceToCities)

        For j = 1 To NUMCIUDADES
            For i = 1 To NumMaps
                distanceToCities(i).distanceToCity(j) = - 1
            Next i
        Next j

        For j = 1 To NUMCIUDADES
            For i = 1 To 4
                Select Case i
                    Case eHeading.NORTH
                        Call setDistance(getLimit(Ciudades(j).Map, eHeading.NORTH), j, i, 0, 1)
                    Case eHeading.EAST
                        Call setDistance(getLimit(Ciudades(j).Map, eHeading.EAST), j, i, 1, 0)
                    Case eHeading.SOUTH
                        Call setDistance(getLimit(Ciudades(j).Map, eHeading.SOUTH), j, i, 0, 1)
                    Case eHeading.WEST
                        Call setDistance(getLimit(Ciudades(j).Map, eHeading.WEST), j, i, - 1, 0)
                End Select
            Next i
        Next j
    End Sub

    Public Sub setDistance(mapa As Short, city As Byte, side As Short, Optional ByVal X As Short = 0,
                           Optional ByVal Y As Short = 0)
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Dim i As Short
        Dim lim As Short

        If mapa <= 0 Or mapa > NumMaps Then Exit Sub

        If distanceToCities(mapa).distanceToCity(city) >= 0 Then Exit Sub

        If mapa = Ciudades(city).Map Then
            distanceToCities(mapa).distanceToCity(city) = 0
        Else
            distanceToCities(mapa).distanceToCity(city) = Math.Abs(X) + Math.Abs(Y)
        End If

        For i = 1 To 4
            lim = getLimit(mapa, i)
            If lim > 0 Then
                Select Case i
                    Case eHeading.NORTH
                        Call setDistance(lim, city, i, X, Y + 1)
                    Case eHeading.EAST
                        Call setDistance(lim, city, i, X + 1, Y)
                    Case eHeading.SOUTH
                        Call setDistance(lim, city, i, X, Y - 1)
                    Case eHeading.WEST
                        Call setDistance(lim, city, i, X - 1, Y)
                End Select
            End If
        Next i
    End Sub

    Public Function getLimit(mapa As Short, side As Byte) As Short
        '***************************************************
        'Author: Budi
        'Last Modification: 31/01/2010
        'Retrieves the limit in the given side in the given map.
        'TODO: This should be set in the .inf map file.
        '***************************************************
        Dim i, X As Object
        Dim Y As Short

        If mapa <= 0 Then Exit Function

        For X = 15 To 87
            For Y = 0 To 3
                Select Case side
                    Case eHeading.NORTH
                        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto X. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        getLimit = MapData(mapa, X, 7 + Y).TileExit.Map
                    Case eHeading.EAST
                        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto X. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        getLimit = MapData(mapa, 92 - Y, X).TileExit.Map
                    Case eHeading.SOUTH
                        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto X. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        getLimit = MapData(mapa, X, 94 - Y).TileExit.Map
                    Case eHeading.WEST
                        'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto X. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                        getLimit = MapData(mapa, 9 + Y, X).TileExit.Map
                End Select
                If getLimit > 0 Then Exit Function
            Next Y
        Next X
    End Function


    Public Sub LoadArmadurasFaccion()
        '***************************************************
        'Author: ZaMa
        'Last Modification: 15/04/2010
        '
        '***************************************************
        Dim ClassIndex As Integer
        Dim RaceIndex As Integer

        Dim ArmaduraIndex As Short


        For ClassIndex = 1 To NUMCLASES

            ' Defensa minima para armadas altos
            ArmaduraIndex = Val(GetVar(DatPath & "ArmadurasFaccionarias.dat", "CLASE" & ClassIndex, "DefMinArmyAlto"))

            ArmadurasFaccion(ClassIndex, eRaza.Drow).Armada(eTipoDefArmors.ieBaja) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Elfo).Armada(eTipoDefArmors.ieBaja) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Humano).Armada(eTipoDefArmors.ieBaja) =
                ArmaduraIndex

            ' Defensa minima para armadas bajos
            ArmaduraIndex = Val(GetVar(DatPath & "ArmadurasFaccionarias.dat", "CLASE" & ClassIndex, "DefMinArmyBajo"))

            ArmadurasFaccion(ClassIndex, eRaza.Enano).Armada(eTipoDefArmors.ieBaja) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Gnomo).Armada(eTipoDefArmors.ieBaja) =
                ArmaduraIndex

            ' Defensa minima para caos altos
            ArmaduraIndex = Val(GetVar(DatPath & "ArmadurasFaccionarias.dat", "CLASE" & ClassIndex, "DefMinCaosAlto"))

            ArmadurasFaccion(ClassIndex, eRaza.Drow).Caos(eTipoDefArmors.ieBaja) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Elfo).Caos(eTipoDefArmors.ieBaja) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Humano).Caos(eTipoDefArmors.ieBaja) =
                ArmaduraIndex

            ' Defensa minima para caos bajos
            ArmaduraIndex = Val(GetVar(DatPath & "ArmadurasFaccionarias.dat", "CLASE" & ClassIndex, "DefMinCaosBajo"))

            ArmadurasFaccion(ClassIndex, eRaza.Enano).Caos(eTipoDefArmors.ieBaja) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Gnomo).Caos(eTipoDefArmors.ieBaja) =
                ArmaduraIndex


            ' Defensa media para armadas altos
            ArmaduraIndex = Val(GetVar(DatPath & "ArmadurasFaccionarias.dat", "CLASE" & ClassIndex, "DefMedArmyAlto"))

            ArmadurasFaccion(ClassIndex, eRaza.Drow).Armada(eTipoDefArmors.ieMedia) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Elfo).Armada(eTipoDefArmors.ieMedia) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Humano).Armada(eTipoDefArmors.ieMedia) =
                ArmaduraIndex

            ' Defensa media para armadas bajos
            ArmaduraIndex = Val(GetVar(DatPath & "ArmadurasFaccionarias.dat", "CLASE" & ClassIndex, "DefMedArmyBajo"))

            ArmadurasFaccion(ClassIndex, eRaza.Enano).Armada(eTipoDefArmors.ieMedia) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Gnomo).Armada(eTipoDefArmors.ieMedia) =
                ArmaduraIndex

            ' Defensa media para caos altos
            ArmaduraIndex = Val(GetVar(DatPath & "ArmadurasFaccionarias.dat", "CLASE" & ClassIndex, "DefMedCaosAlto"))

            ArmadurasFaccion(ClassIndex, eRaza.Drow).Caos(eTipoDefArmors.ieMedia) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Elfo).Caos(eTipoDefArmors.ieMedia) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Humano).Caos(eTipoDefArmors.ieMedia) =
                ArmaduraIndex

            ' Defensa media para caos bajos
            ArmaduraIndex = Val(GetVar(DatPath & "ArmadurasFaccionarias.dat", "CLASE" & ClassIndex, "DefMedCaosBajo"))

            ArmadurasFaccion(ClassIndex, eRaza.Enano).Caos(eTipoDefArmors.ieMedia) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Gnomo).Caos(eTipoDefArmors.ieMedia) =
                ArmaduraIndex


            ' Defensa alta para armadas altos
            ArmaduraIndex = Val(GetVar(DatPath & "ArmadurasFaccionarias.dat", "CLASE" & ClassIndex, "DefAltaArmyAlto"))

            ArmadurasFaccion(ClassIndex, eRaza.Drow).Armada(eTipoDefArmors.ieAlta) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Elfo).Armada(eTipoDefArmors.ieAlta) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Humano).Armada(eTipoDefArmors.ieAlta) =
                ArmaduraIndex

            ' Defensa alta para armadas bajos
            ArmaduraIndex = Val(GetVar(DatPath & "ArmadurasFaccionarias.dat", "CLASE" & ClassIndex, "DefAltaArmyBajo"))

            ArmadurasFaccion(ClassIndex, eRaza.Enano).Armada(eTipoDefArmors.ieAlta) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Gnomo).Armada(eTipoDefArmors.ieAlta) =
                ArmaduraIndex

            ' Defensa alta para caos altos
            ArmaduraIndex = Val(GetVar(DatPath & "ArmadurasFaccionarias.dat", "CLASE" & ClassIndex, "DefAltaCaosAlto"))

            ArmadurasFaccion(ClassIndex, eRaza.Drow).Caos(eTipoDefArmors.ieAlta) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Elfo).Caos(eTipoDefArmors.ieAlta) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Humano).Caos(eTipoDefArmors.ieAlta) =
                ArmaduraIndex

            ' Defensa alta para caos bajos
            ArmaduraIndex = Val(GetVar(DatPath & "ArmadurasFaccionarias.dat", "CLASE" & ClassIndex, "DefAltaCaosBajo"))

            ArmadurasFaccion(ClassIndex, eRaza.Enano).Caos(eTipoDefArmors.ieAlta) =
                ArmaduraIndex
            ArmadurasFaccion(ClassIndex, eRaza.Gnomo).Caos(eTipoDefArmors.ieAlta) =
                ArmaduraIndex

        Next ClassIndex
    End Sub
End Module
