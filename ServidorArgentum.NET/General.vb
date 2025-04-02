Option Strict Off
Option Explicit On
Module General
	'Argentum Online 0.12.2
	'Copyright (C) 2002 Márquez Pablo Ignacio
	'
	'This program is free software; you can redistribute it and/or modify
	'it under the terms of the Affero General Public License;
	'either version 1 of the License, or any later version.
	'
	'This program is distributed in the hope that it will be useful,
	'but WITHOUT ANY WARRANTY; without even the implied warranty of
	'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	'Affero General Public License for more details.
	'
	'You should have received a copy of the Affero General Public License
	'along with this program; if not, you can find it at http://www.affero.org/oagpl.html
	'
	'Argentum Online is based on Baronsoft's VB6 Online RPG
	'You can contact the original creator of ORE at aaron@baronsoft.com
	'for more information about ORE please visit http://www.baronsoft.com/
	'
	'
	'You can contact me at:
	'morgolock@speedy.com.ar
	'www.geocities.com/gmorgolock
	'Calle 3 número 983 piso 7 dto A
	'La Plata - Pcia, Buenos Aires - Republica Argentina
	'Código Postal 1900
	'Pablo Ignacio Márquez
	
	
	Public LeerNPCs As New clsIniReader
	
	Sub DarCuerpoDesnudo(ByVal UserIndex As Short, Optional ByVal Mimetizado As Boolean = False)
		'***************************************************
		'Autor: Nacho (Integer)
		'Last Modification: 03/14/07
		'Da cuerpo desnudo a un usuario
		'23/11/2009: ZaMa - Optimizacion de codigo.
		'***************************************************
		
		Dim CuerpoDesnudo As Short
		
		With UserList(UserIndex)
			Select Case .Genero
				Case Declaraciones.eGenero.Hombre
					Select Case .raza
						Case Declaraciones.eRaza.Humano
							CuerpoDesnudo = 21
						Case Declaraciones.eRaza.Drow
							CuerpoDesnudo = 32
						Case Declaraciones.eRaza.Elfo
							CuerpoDesnudo = 210
						Case Declaraciones.eRaza.Gnomo
							CuerpoDesnudo = 222
						Case Declaraciones.eRaza.Enano
							CuerpoDesnudo = 53
					End Select
				Case Declaraciones.eGenero.Mujer
					Select Case .raza
						Case Declaraciones.eRaza.Humano
							CuerpoDesnudo = 39
						Case Declaraciones.eRaza.Drow
							CuerpoDesnudo = 40
						Case Declaraciones.eRaza.Elfo
							CuerpoDesnudo = 259
						Case Declaraciones.eRaza.Gnomo
							CuerpoDesnudo = 260
						Case Declaraciones.eRaza.Enano
							CuerpoDesnudo = 60
					End Select
			End Select
			
			If Mimetizado Then
				.CharMimetizado.body = CuerpoDesnudo
			Else
				.Char_Renamed.body = CuerpoDesnudo
			End If
			
			.flags.Desnudo = 1
		End With
		
	End Sub
	
	
	Sub Bloquear(ByVal toMap As Boolean, ByVal sndIndex As Short, ByVal X As Short, ByVal Y As Short, ByVal b As Boolean)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'b ahora es boolean,
		'b=true bloquea el tile en (x,y)
		'b=false desbloquea el tile en (x,y)
		'toMap = true -> Envia los datos a todo el mapa
		'toMap = false -> Envia los datos al user
		'Unifique los tres parametros (sndIndex,sndMap y map) en sndIndex... pero de todas formas, el mapa jamas se indica.. eso esta bien asi?
		'Puede llegar a ser, que se quiera mandar el mapa, habria que agregar un nuevo parametro y modificar.. lo quite porque no se usaba ni aca ni en el cliente :s
		'***************************************************
		
		If toMap Then
			Call SendData(modSendData.SendTarget.toMap, sndIndex, PrepareMessageBlockPosition(X, Y, b))
		Else
			Call WriteBlockPosition(sndIndex, X, Y, b)
		End If
		
	End Sub
	
	
	Function HayAgua(ByVal Map As Short, ByVal X As Short, ByVal Y As Short) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		If Map > 0 And Map < NumMaps + 1 And X > 0 And X < 101 And Y > 0 And Y < 101 Then
			With MapData(Map, X, Y)
				If ((.Graphic(1) >= 1505 And .Graphic(1) <= 1520) Or (.Graphic(1) >= 5665 And .Graphic(1) <= 5680) Or (.Graphic(1) >= 13547 And .Graphic(1) <= 13562)) And .Graphic(2) = 0 Then
					HayAgua = True
				Else
					HayAgua = False
				End If
			End With
		Else
			HayAgua = False
		End If
		
	End Function
	
	Private Function HayLava(ByVal Map As Short, ByVal X As Short, ByVal Y As Short) As Boolean
		'***************************************************
		'Autor: Nacho (Integer)
		'Last Modification: 03/12/07
		'***************************************************
		If Map > 0 And Map < NumMaps + 1 And X > 0 And X < 101 And Y > 0 And Y < 101 Then
			If MapData(Map, X, Y).Graphic(1) >= 5837 And MapData(Map, X, Y).Graphic(1) <= 5852 Then
				HayLava = True
			Else
				HayLava = False
			End If
		Else
			HayLava = False
		End If
		
	End Function
	
	
	Sub LimpiarMundo()
		'***************************************************
		'Author: Unknow
		'Last Modification: 04/15/2008
		'01/14/2008: Marcos Martinez (ByVal) - La funcion FOR estaba mal. En ves de i habia un 1.
		'04/15/2008: (NicoNZ) - La funcion FOR estaba mal, de la forma que se hacia tiraba error.
		'***************************************************
		On Error GoTo Errhandler
		
		Dim i As Short
		Dim d As New cGarbage
		
		For i = TrashCollector.Count() To 1 Step -1
			d = TrashCollector.Item(i)
			Call EraseObj(1, d.Map, d.X, d.Y)
			Call TrashCollector.Remove(i)
			'UPGRADE_NOTE: El objeto d no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
			d = Nothing
		Next i
		
		Call SecurityIp.IpSecurityMantenimientoLista()
		
		Exit Sub
		
Errhandler: 
		Call LogError("Error producido en el sub LimpiarMundo: " & Err.Description)
	End Sub
	
	Sub EnviarSpawnList(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim k As Integer
		Dim npcNames() As String
		
		'UPGRADE_WARNING: El límite inferior de la matriz npcNames ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		ReDim npcNames(UBound(SpawnList))
		
		For k = 1 To UBound(SpawnList)
			npcNames(k) = SpawnList(k).NpcName
		Next k
		
		Call WriteSpawnList(UserIndex, npcNames)
		
	End Sub
	
	'UPGRADE_WARNING: La aplicación terminará cuando Sub Main() finalice. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="E08DDC71-66BA-424F-A612-80AF11498FF8"'
	Public Sub Main()
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error Resume Next
		Dim f As Date
		
		ChDir(My.Application.Info.DirectoryPath)
		ChDrive(My.Application.Info.DirectoryPath)
		
		Call LoadMotd()
		Call BanIpCargar()

		' TODO: reubicar estas inicializaciones!
		ArrayInitializers.InitializeStruct(Npclist)
		ArrayInitializers.InitializeStruct(ArmadurasFaccion)
		ArrayInitializers.InitializeStruct(fragLvlRaceData)
		ArrayInitializers.InitializeStruct(fragLvlLvlData)



		Prision.Map = 66
		Libertad.Map = 66
		
		Prision.X = 75
		Prision.Y = 47
		Libertad.X = 75
		Libertad.Y = 65
		
		
		LastBackup = VB6.Format(Now, "Short Time")
		Minutos = VB6.Format(Now, "Short Time")
		
		IniPath = My.Application.Info.DirectoryPath & "\"
		DatPath = My.Application.Info.DirectoryPath & "\Dat\"
		
		
		
		LevelSkill_Renamed(1).LevelValue = 3
		LevelSkill_Renamed(2).LevelValue = 5
		LevelSkill_Renamed(3).LevelValue = 7
		LevelSkill_Renamed(4).LevelValue = 10
		LevelSkill_Renamed(5).LevelValue = 13
		LevelSkill_Renamed(6).LevelValue = 15
		LevelSkill_Renamed(7).LevelValue = 17
		LevelSkill_Renamed(8).LevelValue = 20
		LevelSkill_Renamed(9).LevelValue = 23
		LevelSkill_Renamed(10).LevelValue = 25
		LevelSkill_Renamed(11).LevelValue = 27
		LevelSkill_Renamed(12).LevelValue = 30
		LevelSkill_Renamed(13).LevelValue = 33
		LevelSkill_Renamed(14).LevelValue = 35
		LevelSkill_Renamed(15).LevelValue = 37
		LevelSkill_Renamed(16).LevelValue = 40
		LevelSkill_Renamed(17).LevelValue = 43
		LevelSkill_Renamed(18).LevelValue = 45
		LevelSkill_Renamed(19).LevelValue = 47
		LevelSkill_Renamed(20).LevelValue = 50
		LevelSkill_Renamed(21).LevelValue = 53
		LevelSkill_Renamed(22).LevelValue = 55
		LevelSkill_Renamed(23).LevelValue = 57
		LevelSkill_Renamed(24).LevelValue = 60
		LevelSkill_Renamed(25).LevelValue = 63
		LevelSkill_Renamed(26).LevelValue = 65
		LevelSkill_Renamed(27).LevelValue = 67
		LevelSkill_Renamed(28).LevelValue = 70
		LevelSkill_Renamed(29).LevelValue = 73
		LevelSkill_Renamed(30).LevelValue = 75
		LevelSkill_Renamed(31).LevelValue = 77
		LevelSkill_Renamed(32).LevelValue = 80
		LevelSkill_Renamed(33).LevelValue = 83
		LevelSkill_Renamed(34).LevelValue = 85
		LevelSkill_Renamed(35).LevelValue = 87
		LevelSkill_Renamed(36).LevelValue = 90
		LevelSkill_Renamed(37).LevelValue = 93
		LevelSkill_Renamed(38).LevelValue = 95
		LevelSkill_Renamed(39).LevelValue = 97
		LevelSkill_Renamed(40).LevelValue = 100
		LevelSkill_Renamed(41).LevelValue = 100
		LevelSkill_Renamed(42).LevelValue = 100
		LevelSkill_Renamed(43).LevelValue = 100
		LevelSkill_Renamed(44).LevelValue = 100
		LevelSkill_Renamed(45).LevelValue = 100
		LevelSkill_Renamed(46).LevelValue = 100
		LevelSkill_Renamed(47).LevelValue = 100
		LevelSkill_Renamed(48).LevelValue = 100
		LevelSkill_Renamed(49).LevelValue = 100
		LevelSkill_Renamed(50).LevelValue = 100
		
		
		ListaRazas(Declaraciones.eRaza.Humano) = "Humano"
		ListaRazas(Declaraciones.eRaza.Elfo) = "Elfo"
		ListaRazas(Declaraciones.eRaza.Drow) = "Drow"
		ListaRazas(Declaraciones.eRaza.Gnomo) = "Gnomo"
		ListaRazas(Declaraciones.eRaza.Enano) = "Enano"
		
		ListaClases(Declaraciones.eClass.Mage) = "Mago"
		ListaClases(Declaraciones.eClass.Cleric) = "Clerigo"
		ListaClases(Declaraciones.eClass.Warrior) = "Guerrero"
		ListaClases(Declaraciones.eClass.Assasin) = "Asesino"
		ListaClases(Declaraciones.eClass.Thief) = "Ladron"
		ListaClases(Declaraciones.eClass.Bard) = "Bardo"
		ListaClases(Declaraciones.eClass.Druid) = "Druida"
		ListaClases(Declaraciones.eClass.Bandit) = "Bandido"
		ListaClases(Declaraciones.eClass.Paladin) = "Paladin"
		ListaClases(Declaraciones.eClass.Hunter) = "Cazador"
		ListaClases(Declaraciones.eClass.Worker) = "Trabajador"
		ListaClases(Declaraciones.eClass.Pirat) = "Pirata"
		
		SkillsNames(Declaraciones.eSkill.Magia) = "Magia"
		SkillsNames(Declaraciones.eSkill.Robar) = "Robar"
		SkillsNames(Declaraciones.eSkill.Tacticas) = "Evasión en combate"
		SkillsNames(Declaraciones.eSkill.Armas) = "Combate con armas"
		SkillsNames(Declaraciones.eSkill.Meditar) = "Meditar"
		SkillsNames(Declaraciones.eSkill.Apuñalar) = "Apuñalar"
		SkillsNames(Declaraciones.eSkill.Ocultarse) = "Ocultarse"
		SkillsNames(Declaraciones.eSkill.Supervivencia) = "Supervivencia"
		SkillsNames(Declaraciones.eSkill.Talar) = "Talar"
		SkillsNames(Declaraciones.eSkill.Comerciar) = "Comercio"
		SkillsNames(Declaraciones.eSkill.Defensa) = "Defensa con escudos"
		SkillsNames(Declaraciones.eSkill.Pesca) = "Pesca"
		SkillsNames(Declaraciones.eSkill.Mineria) = "Mineria"
		SkillsNames(Declaraciones.eSkill.Carpinteria) = "Carpinteria"
		SkillsNames(Declaraciones.eSkill.Herreria) = "Herreria"
		SkillsNames(Declaraciones.eSkill.Liderazgo) = "Liderazgo"
		SkillsNames(Declaraciones.eSkill.Domar) = "Domar animales"
		SkillsNames(Declaraciones.eSkill.Proyectiles) = "Combate a distancia"
		SkillsNames(Declaraciones.eSkill.Wrestling) = "Combate sin armas"
		SkillsNames(Declaraciones.eSkill.Navegacion) = "Navegacion"
		
		ListaAtributos(Declaraciones.eAtributos.Fuerza) = "Fuerza"
		ListaAtributos(Declaraciones.eAtributos.Agilidad) = "Agilidad"
		ListaAtributos(Declaraciones.eAtributos.Inteligencia) = "Inteligencia"
		ListaAtributos(Declaraciones.eAtributos.Carisma) = "Carisma"
		ListaAtributos(Declaraciones.eAtributos.Constitucion) = "Constitucion"
		
		frmCargando.Show()
		
		'Call PlayWaveAPI(App.Path & "\wav\harp3.wav")
		
		frmMain.Text = frmMain.Text & " V." & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Revision
		IniPath = My.Application.Info.DirectoryPath & "\"
		CharPath = My.Application.Info.DirectoryPath & "\Charfile\"
		
		'Bordes del mapa
		MinXBorder = XMinMapSize + (XWindow \ 2)
		MaxXBorder = XMaxMapSize - (XWindow \ 2)
		MinYBorder = YMinMapSize + (YWindow \ 2)
		MaxYBorder = YMaxMapSize - (YWindow \ 2)
		System.Windows.Forms.Application.DoEvents()
		
		frmCargando.Label1(2).Text = "Iniciando Arrays..."
		
		Call LoadGuildsDB()
		
		
		Call CargarSpawnList()
		Call CargarForbidenWords()
		'¿?¿?¿?¿?¿?¿?¿?¿ CARGAMOS DATOS DESDE ARCHIVOS ¿??¿?¿?¿?¿?¿?¿?¿
		frmCargando.Label1(2).Text = "Cargando Server.ini"
		
		MaxUsers = 0
		Call LoadSini()
		Call CargaApuestas()
		
		'*************************************************
		frmCargando.Label1(2).Text = "Cargando NPCs.Dat"
		Call CargaNpcsDat()
		'*************************************************
		
		frmCargando.Label1(2).Text = "Cargando Obj.Dat"
		'Call LoadOBJData
		Call LoadOBJData()
		
		frmCargando.Label1(2).Text = "Cargando Hechizos.Dat"
		Call CargarHechizos()
		
		
		frmCargando.Label1(2).Text = "Cargando Objetos de Herrería"
		Call LoadArmasHerreria()
		Call LoadArmadurasHerreria()
		
		frmCargando.Label1(2).Text = "Cargando Objetos de Carpintería"
		Call LoadObjCarpintero()
		
		frmCargando.Label1(2).Text = "Cargando Balance.Dat"
		Call LoadBalance() '4/01/08 Pablo ToxicWaste
		
		frmCargando.Label1(2).Text = "Cargando ArmadurasFaccionarias.dat"
		Call LoadArmadurasFaccion()
		
		If BootDelBackUp Then
			
			frmCargando.Label1(2).Text = "Cargando BackUp"
			Call CargarBackUp()
		Else
			frmCargando.Label1(2).Text = "Cargando Mapas"
			Call LoadMapData()
		End If
		
		
		Call SonidosMapas.LoadSoundMapInfo()
		
		Call generateMatrix(MATRIX_INITIAL_MAP)
		
		'Comentado porque hay worldsave en ese mapa!
		'Call CrearClanPretoriano(MAPA_PRETORIANO, ALCOBA2_X, ALCOBA2_Y)
		'¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
		
		Dim LoopC As Short
		
		'Resetea las conexiones de los usuarios
		For LoopC = 1 To MaxUsers
			UserList(LoopC).ConnID = -1
			UserList(LoopC).ConnIDValida = False
			UserList(LoopC).incomingData = New clsByteQueue
			UserList(LoopC).outgoingData = New clsByteQueue
		Next LoopC
		
		'¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
		
		With frmMain
			.AutoSave.Enabled = True
			.tLluvia.Enabled = True
			.tPiqueteC.Enabled = True
			.GameTimer.Enabled = True
			.tLluviaEvent.Enabled = True
			.FX.Enabled = True
			.Auditoria.Enabled = True
			.KillLog.Enabled = True
			.TIMER_AI.Enabled = True
			.npcataca.Enabled = True
		End With
		
		'¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
		'Configuracion de los sockets
		
		Call SecurityIp.InitIpTables(1000)
		
		Call IniciaWsApi(Puerto)
		
		If frmMain.Visible Then frmMain.txStatus.Text = "Escuchando conexiones entrantes ..."
		'¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿?¿
		
		frmCargando.Close()
		
		'Log
		Dim N As Short
		N = FreeFile
		FileOpen(N, My.Application.Info.DirectoryPath & "\logs\Main.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(N, Today & " " & TimeOfDay & " server iniciado " & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Revision)
		FileClose(N)

		tInicioServer = GetTickCount() And &H7FFFFFFF
		Call InicializaEstadisticas()

		Application.EnableVisualStyles()
		Application.SetCompatibleTextRenderingDefault(False)

		Dim mainForm As New frmMain()
		Application.Run(mainForm) ' Mantiene la aplicación viva hasta que se cierre frmMain

	End Sub
	
	Function FileExist(ByVal file As String) As Boolean
		'*****************************************************************
		'Se fija si existe el archivo
		'*****************************************************************
		
		On Error Resume Next
		'UPGRADE_WARNING: Dir tiene un nuevo comportamiento. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
		FileExist = (Dir(file) <> "")
		On Error GoTo 0
	End Function
	
	Function ReadField(ByVal Pos As Short, ByRef Text As String, ByVal SepASCII As Byte) As String
		'*****************************************************************
		'Gets a field from a string
		'Author: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modify Date: 11/15/2004
		'Gets a field from a delimited string
		'*****************************************************************
		
		Dim i As Integer
		Dim LastPos As Integer
		Dim CurrentPos As Integer
		Dim delimiter As String
		
		delimiter = Chr(SepASCII)
		
		For i = 1 To Pos
			LastPos = CurrentPos
			CurrentPos = InStr(LastPos + 1, Text, delimiter, CompareMethod.Binary)
		Next i
		
		If CurrentPos = 0 Then
			ReadField = Mid(Text, LastPos + 1, Len(Text) - LastPos)
		Else
			ReadField = Mid(Text, LastPos + 1, CurrentPos - LastPos - 1)
		End If
	End Function
	
	Function MapaValido(ByVal Map As Short) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		MapaValido = Map >= 1 And Map <= NumMaps
	End Function
	
	Sub MostrarNumUsers()
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		frmMain.CantUsuarios.Text = "Número de usuarios jugando: " & NumUsers
		
	End Sub
	
	
	Public Sub LogCriticEvent(ByRef desc As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\Eventos.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, Today & " " & TimeOfDay & " " & desc)
		FileClose(nfile)
		
		Exit Sub
		
Errhandler: 
		
	End Sub
	
	Public Sub LogEjercitoReal(ByRef desc As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\EjercitoReal.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, desc)
		FileClose(nfile)
		
		Exit Sub
		
Errhandler: 
		
	End Sub
	
	Public Sub LogEjercitoCaos(ByRef desc As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\EjercitoCaos.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, desc)
		FileClose(nfile)
		
		Exit Sub
		
Errhandler: 
		
	End Sub
	
	
	Public Sub LogIndex(ByVal Index As Short, ByVal desc As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\" & Index & ".log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, Today & " " & TimeOfDay & " " & desc)
		FileClose(nfile)
		
		Exit Sub
		
Errhandler: 
		
	End Sub
	
	
	Public Sub LogError(ByRef desc As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\errores.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, Today & " " & TimeOfDay & " " & desc)
		FileClose(nfile)
		
		Exit Sub
		
Errhandler: 
		
	End Sub
	
	Public Sub LogStatic(ByRef desc As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\Stats.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, Today & " " & TimeOfDay & " " & desc)
		FileClose(nfile)
		
		Exit Sub
		
Errhandler: 
		
	End Sub
	
	Public Sub LogTarea(ByRef desc As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim nfile As Short
		nfile = FreeFile() ' obtenemos un canal
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\haciendo.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, Today & " " & TimeOfDay & " " & desc)
		FileClose(nfile)
		
		Exit Sub
		
Errhandler: 
		
		
	End Sub
	
	
	'UPGRADE_NOTE: str se actualizó a str_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public Sub LogClanes(ByVal str_Renamed As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\clanes.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, Today & " " & TimeOfDay & " " & str_Renamed)
		FileClose(nfile)
		
	End Sub
	
	'UPGRADE_NOTE: str se actualizó a str_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public Sub LogIP(ByVal str_Renamed As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\IP.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, Today & " " & TimeOfDay & " " & str_Renamed)
		FileClose(nfile)
		
	End Sub
	
	
	'UPGRADE_NOTE: str se actualizó a str_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Public Sub LogDesarrollo(ByVal str_Renamed As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\desarrollo" & Month(Today) & Year(Today) & ".log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, Today & " " & TimeOfDay & " " & str_Renamed)
		FileClose(nfile)
		
	End Sub
	
	Public Sub LogGM(ByRef Nombre As String, ByRef texto As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************ç
		
		On Error GoTo Errhandler
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		'Guardamos todo en el mismo lugar. Pablo (ToxicWaste) 18/05/07
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\" & Nombre & ".log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, Today & " " & TimeOfDay & " " & texto)
		FileClose(nfile)
		
		Exit Sub
		
Errhandler: 
		
	End Sub
	
	Public Sub LogAsesinato(ByRef texto As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		Dim nfile As Short
		
		nfile = FreeFile ' obtenemos un canal
		
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\asesinatos.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, Today & " " & TimeOfDay & " " & texto)
		FileClose(nfile)
		
		Exit Sub
		
Errhandler: 
		
	End Sub
	Public Sub logVentaCasa(ByVal texto As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\propiedades.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, "----------------------------------------------------------")
		PrintLine(nfile, Today & " " & TimeOfDay & " " & texto)
		PrintLine(nfile, "----------------------------------------------------------")
		FileClose(nfile)
		
		Exit Sub
		
Errhandler: 
		
	End Sub
	Public Sub LogHackAttemp(ByRef texto As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\HackAttemps.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, "----------------------------------------------------------")
		PrintLine(nfile, Today & " " & TimeOfDay & " " & texto)
		PrintLine(nfile, "----------------------------------------------------------")
		FileClose(nfile)
		
		Exit Sub
		
Errhandler: 
		
	End Sub
	
	Public Sub LogCheating(ByRef texto As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\CH.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, Today & " " & TimeOfDay & " " & texto)
		FileClose(nfile)
		
		Exit Sub
		
Errhandler: 
		
	End Sub
	
	
	Public Sub LogCriticalHackAttemp(ByRef texto As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\CriticalHackAttemps.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, "----------------------------------------------------------")
		PrintLine(nfile, Today & " " & TimeOfDay & " " & texto)
		PrintLine(nfile, "----------------------------------------------------------")
		FileClose(nfile)
		
		Exit Sub
		
Errhandler: 
		
	End Sub
	
	Public Sub LogAntiCheat(ByRef texto As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim nfile As Short
		nfile = FreeFile ' obtenemos un canal
		FileOpen(nfile, My.Application.Info.DirectoryPath & "\logs\AntiCheat.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(nfile, Today & " " & TimeOfDay & " " & texto)
		PrintLine(nfile, "")
		FileClose(nfile)
		
		Exit Sub
		
Errhandler: 
		
	End Sub
	
	Function ValidInputNP(ByVal cad As String) As Boolean
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim Arg As String
		Dim i As Short
		
		
		For i = 1 To 33
			
			Arg = ReadField(i, cad, 44)
			
			If migr_LenB(Arg) = 0 Then Exit Function
			
		Next i
		
		ValidInputNP = True
		
	End Function
	
	
	Sub Restart()
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		'Se asegura de que los sockets estan cerrados e ignora cualquier err
		On Error Resume Next
		
		If frmMain.Visible Then frmMain.txStatus.Text = "Reiniciando."
		
		Dim LoopC As Integer
		
		'Inicia el socket de escucha
		Call IniciaWsApi(Puerto)
		
		'Initialize statistics!!
		Call Statistics.Initialize()
		
		For LoopC = 1 To UBound(UserList)
			'UPGRADE_NOTE: El objeto UserList().incomingData no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
			UserList(LoopC).incomingData = Nothing
			'UPGRADE_NOTE: El objeto UserList().outgoingData no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
			UserList(LoopC).outgoingData = Nothing
		Next LoopC

		'UPGRADE_WARNING: Es posible que la matriz UserList necesite tener elementos individuales inicializados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B97B714D-9338-48AC-B03F-345B617E2B02"'
		'UPGRADE_WARNING: El límite inferior de la matriz UserList ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		ReDim UserList(MaxUsers)
		ArrayInitializers.InitializeStruct(UserList)

		For LoopC = 1 To MaxUsers
			UserList(LoopC).ConnID = -1
			UserList(LoopC).ConnIDValida = False
			UserList(LoopC).incomingData = New clsByteQueue
			UserList(LoopC).outgoingData = New clsByteQueue
		Next LoopC
		
		LastUser = 0
		NumUsers = 0
		
		Call FreeNPCs()
		Call FreeCharIndexes()
		
		Call LoadSini()
		
		Call ResetForums()
		Call LoadOBJData()
		
		Call LoadMapData()
		
		Call CargarHechizos()
		
		If frmMain.Visible Then frmMain.txStatus.Text = "Escuchando conexiones entrantes ..."
		
		'Log it
		Dim N As Short
		N = FreeFile
		FileOpen(N, My.Application.Info.DirectoryPath & "\logs\Main.log", OpenMode.Append, , OpenShare.Shared)
		PrintLine(N, Today & " " & TimeOfDay & " servidor reiniciado.")
		FileClose(N)
		
		frmMain.Show()
		
	End Sub
	
	
	Public Function Intemperie(ByVal UserIndex As Short) As Boolean
		'**************************************************************
		'Author: Unknown
		'Last Modify Date: 15/11/2009
		'15/11/2009: ZaMa - La lluvia no quita stamina en las arenas.
		'23/11/2009: ZaMa - Optimizacion de codigo.
		'**************************************************************
		
		With UserList(UserIndex)
			If MapInfo_Renamed(.Pos.Map).Zona <> "DUNGEON" Then
				If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger <> 1 And MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger <> 2 And MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger <> 4 Then Intemperie = True
			Else
				Intemperie = False
			End If
		End With
		
		'En las arenas no te afecta la lluvia
		If IsArena(UserIndex) Then Intemperie = False
	End Function
	
	Public Sub EfectoLluvia(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		
		Dim modifi As Integer
		If UserList(UserIndex).flags.UserLogged Then
			If Intemperie(UserIndex) Then
				modifi = Porcentaje(UserList(UserIndex).Stats.MaxSta, 3)
				Call QuitarSta(UserIndex, modifi)
				Call FlushBuffer(UserIndex)
			End If
		End If
		
		Exit Sub
Errhandler: 
		LogError(("Error en EfectoLluvia"))
	End Sub
	
	Public Sub TiempoInvocacion(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim i As Short
		For i = 1 To MAXMASCOTAS
			With UserList(UserIndex)
				If .MascotasIndex(i) > 0 Then
					If Npclist(.MascotasIndex(i)).Contadores.TiempoExistencia > 0 Then
						Npclist(.MascotasIndex(i)).Contadores.TiempoExistencia = Npclist(.MascotasIndex(i)).Contadores.TiempoExistencia - 1
						If Npclist(.MascotasIndex(i)).Contadores.TiempoExistencia = 0 Then Call MuereNpc(.MascotasIndex(i), 0)
					End If
				End If
			End With
		Next i
	End Sub
	
	Public Sub EfectoFrio(ByVal UserIndex As Short)
		'***************************************************
		'Autor: Unkonwn
		'Last Modification: 23/11/2009
		'If user is naked and it's in a cold map, take health points from him
		'23/11/2009: ZaMa - Optimizacion de codigo.
		'***************************************************
		Dim modifi As Short
		
		With UserList(UserIndex)
			If .Counters.Frio < IntervaloFrio Then
				.Counters.Frio = .Counters.Frio + 1
			Else
				If MapInfo_Renamed(.Pos.Map).Terreno = Nieve Then
					Call WriteConsoleMsg(UserIndex, "¡¡Estás muriendo de frío, abrigate o morirás!!", Protocol.FontTypeNames.FONTTYPE_INFO)
					modifi = Porcentaje(.Stats.MaxHp, 5)
					.Stats.MinHp = .Stats.MinHp - modifi
					
					If .Stats.MinHp < 1 Then
						Call WriteConsoleMsg(UserIndex, "¡¡Has muerto de frío!!", Protocol.FontTypeNames.FONTTYPE_INFO)
						.Stats.MinHp = 0
						Call UserDie(UserIndex)
					End If
					
					Call WriteUpdateHP(UserIndex)
				Else
					modifi = Porcentaje(.Stats.MaxSta, 5)
					Call QuitarSta(UserIndex, modifi)
					Call WriteUpdateSta(UserIndex)
				End If
				
				.Counters.Frio = 0
			End If
		End With
	End Sub
	
	Public Sub EfectoLava(ByVal UserIndex As Short)
		'***************************************************
		'Autor: Nacho (Integer)
		'Last Modification: 23/11/2009
		'If user is standing on lava, take health points from him
		'23/11/2009: ZaMa - Optimizacion de codigo.
		'***************************************************
		With UserList(UserIndex)
			If .Counters.Lava < IntervaloFrio Then 'Usamos el mismo intervalo que el del frio
				.Counters.Lava = .Counters.Lava + 1
			Else
				If HayLava(.Pos.Map, .Pos.X, .Pos.Y) Then
					Call WriteConsoleMsg(UserIndex, "¡¡Quitate de la lava, te estás quemando!!", Protocol.FontTypeNames.FONTTYPE_INFO)
					.Stats.MinHp = .Stats.MinHp - Porcentaje(.Stats.MaxHp, 5)
					
					If .Stats.MinHp < 1 Then
						Call WriteConsoleMsg(UserIndex, "¡¡Has muerto quemado!!", Protocol.FontTypeNames.FONTTYPE_INFO)
						.Stats.MinHp = 0
						Call UserDie(UserIndex)
					End If
					
					Call WriteUpdateHP(UserIndex)
					
				End If
				
				.Counters.Lava = 0
			End If
		End With
	End Sub
	
	''
	' Maneja  el efecto del estado atacable
	'
	' @param UserIndex  El index del usuario a ser afectado por el estado atacable
	'
	
	Public Sub EfectoEstadoAtacable(ByVal UserIndex As Short)
		'******************************************************
		'Author: ZaMa
		'Last Update: 13/01/2010 (ZaMa)
		'******************************************************
		
		' Si ya paso el tiempo de penalizacion
		If Not IntervaloEstadoAtacable(UserIndex) Then
			' Deja de poder ser atacado
			UserList(UserIndex).flags.AtacablePor = 0
			' Send nick normal
			Call RefreshCharStatus(UserIndex)
		End If
	End Sub
	
	''
	' Maneja el tiempo y el efecto del mimetismo
	'
	' @param UserIndex  El index del usuario a ser afectado por el mimetismo
	'
	
	Public Sub EfectoMimetismo(ByVal UserIndex As Short)
		'******************************************************
		'Author: Unknown
		'Last Update: 12/01/2010 (ZaMa)
		'12/01/2010: ZaMa - Los druidas pierden la inmunidad de ser atacados cuando pierden el efecto del mimetismo.
		'******************************************************
		'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura Barco, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
		Dim Barco As ObjData
		
		With UserList(UserIndex)
			If .Counters.Mimetismo < IntervaloInvisible Then
				.Counters.Mimetismo = .Counters.Mimetismo + 1
			Else
				'restore old char
				Call WriteConsoleMsg(UserIndex, "Recuperas tu apariencia normal.", Protocol.FontTypeNames.FONTTYPE_INFO)
				
				If .flags.Navegando Then
					If .flags.Muerto = 0 Then
						If .Faccion.ArmadaReal = 1 Then
							.Char_Renamed.body = iFragataReal
						ElseIf .Faccion.FuerzasCaos = 1 Then 
							.Char_Renamed.body = iFragataCaos
						Else
							'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Barco. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
							Barco = ObjData_Renamed(UserList(UserIndex).Invent.BarcoObjIndex)
							If criminal(UserIndex) Then
								If Barco.Ropaje = iBarca Then .Char_Renamed.body = iBarcaPk
								If Barco.Ropaje = iGalera Then .Char_Renamed.body = iGaleraPk
								If Barco.Ropaje = iGaleon Then .Char_Renamed.body = iGaleonPk
							Else
								If Barco.Ropaje = iBarca Then .Char_Renamed.body = iBarcaCiuda
								If Barco.Ropaje = iGalera Then .Char_Renamed.body = iGaleraCiuda
								If Barco.Ropaje = iGaleon Then .Char_Renamed.body = iGaleonCiuda
							End If
						End If
					Else
						.Char_Renamed.body = iFragataFantasmal
					End If
					
					.Char_Renamed.ShieldAnim = NingunEscudo
					.Char_Renamed.WeaponAnim = NingunArma
					.Char_Renamed.CascoAnim = NingunCasco
				Else
					.Char_Renamed.body = .CharMimetizado.body
					.Char_Renamed.Head = .CharMimetizado.Head
					.Char_Renamed.CascoAnim = .CharMimetizado.CascoAnim
					.Char_Renamed.ShieldAnim = .CharMimetizado.ShieldAnim
					.Char_Renamed.WeaponAnim = .CharMimetizado.WeaponAnim
				End If
				
				With .Char_Renamed
					Call ChangeUserChar(UserIndex, .body, .Head, .heading, .WeaponAnim, .ShieldAnim, .CascoAnim)
				End With
				
				.Counters.Mimetismo = 0
				.flags.Mimetizado = 0
				' Se fue el efecto del mimetismo, puede ser atacado por npcs
				.flags.Ignorado = False
			End If
		End With
	End Sub
	
	Public Sub EfectoInvisibilidad(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		With UserList(UserIndex)
			If .Counters.Invisibilidad < IntervaloInvisible Then
				.Counters.Invisibilidad = .Counters.Invisibilidad + 1
			Else
				.Counters.Invisibilidad = RandomNumber(-100, 100) ' Invi variable :D
				.flags.invisible = 0
				If .flags.Oculto = 0 Then
					Call WriteConsoleMsg(UserIndex, "Has vuelto a ser visible.", Protocol.FontTypeNames.FONTTYPE_INFO)
					Call SetInvisible(UserIndex, .Char_Renamed.CharIndex, False)
					'Call SendData(SendTarget.ToPCArea, UserIndex, PrepareMessageSetInvisible(.Char.CharIndex, False))
				End If
			End If
		End With
		
	End Sub
	
	
	Public Sub EfectoParalisisNpc(ByVal NpcIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		With Npclist(NpcIndex)
			If .Contadores.Paralisis > 0 Then
				.Contadores.Paralisis = .Contadores.Paralisis - 1
			Else
				.flags.Paralizado = 0
				.flags.Inmovilizado = 0
			End If
		End With
		
	End Sub
	
	Public Sub EfectoCegueEstu(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		With UserList(UserIndex)
			If .Counters.Ceguera > 0 Then
				.Counters.Ceguera = .Counters.Ceguera - 1
			Else
				If .flags.Ceguera = 1 Then
					.flags.Ceguera = 0
					Call WriteBlindNoMore(UserIndex)
				End If
				If .flags.Estupidez = 1 Then
					.flags.Estupidez = 0
					Call WriteDumbNoMore(UserIndex)
				End If
				
			End If
		End With
		
	End Sub
	
	
	Public Sub EfectoParalisisUser(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		With UserList(UserIndex)
			If .Counters.Paralisis > 0 Then
				.Counters.Paralisis = .Counters.Paralisis - 1
			Else
				.flags.Paralizado = 0
				.flags.Inmovilizado = 0
				'.Flags.AdministrativeParalisis = 0
				Call WriteParalizeOK(UserIndex)
			End If
		End With
		
	End Sub
	
	Public Sub RecStamina(ByVal UserIndex As Short, ByRef EnviarStats As Boolean, ByVal Intervalo As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim massta As Short
		With UserList(UserIndex)
			If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = 1 And MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = 2 And MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = 4 Then Exit Sub
			
			
			If .Stats.MinSta < .Stats.MaxSta Then
				If .Counters.STACounter < Intervalo Then
					.Counters.STACounter = .Counters.STACounter + 1
				Else
					EnviarStats = True
					.Counters.STACounter = 0
					If .flags.Desnudo Then Exit Sub 'Desnudo no sube energía. (ToxicWaste)
					
					massta = RandomNumber(1, Porcentaje(.Stats.MaxSta, 5))
					.Stats.MinSta = .Stats.MinSta + massta
					If .Stats.MinSta > .Stats.MaxSta Then
						.Stats.MinSta = .Stats.MaxSta
					End If
				End If
			End If
		End With
		
	End Sub
	
	Public Sub EfectoVeneno(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim N As Short
		
		With UserList(UserIndex)
			If .Counters.Veneno < IntervaloVeneno Then
				.Counters.Veneno = .Counters.Veneno + 1
			Else
				Call WriteConsoleMsg(UserIndex, "Estás envenenado, si no te curas morirás.", Protocol.FontTypeNames.FONTTYPE_VENENO)
				.Counters.Veneno = 0
				N = RandomNumber(1, 5)
				.Stats.MinHp = .Stats.MinHp - N
				If .Stats.MinHp < 1 Then Call UserDie(UserIndex)
				Call WriteUpdateHP(UserIndex)
			End If
		End With
		
	End Sub
	
	Public Sub DuracionPociones(ByVal UserIndex As Short)
		'***************************************************
		'Author: ??????
		'Last Modification: 11/27/09 (Budi)
		'Cuando se pierde el efecto de la poción updatea fz y agi (No me gusta que ambos atributos aunque se haya modificado solo uno, pero bueno :p)
		'***************************************************
		Dim loopX As Short
		With UserList(UserIndex)
			'Controla la duracion de las pociones
			If .flags.DuracionEfecto > 0 Then
				.flags.DuracionEfecto = .flags.DuracionEfecto - 1
				If .flags.DuracionEfecto = 0 Then
					.flags.TomoPocion = False
					.flags.TipoPocion = 0
					'volvemos los atributos al estado normal
					
					For loopX = 1 To NUMATRIBUTOS
						.Stats.UserAtributos(loopX) = .Stats.UserAtributosBackUP(loopX)
					Next loopX
					
					Call WriteUpdateStrenghtAndDexterity(UserIndex)
				End If
			End If
		End With
		
	End Sub
	
	Public Sub HambreYSed(ByVal UserIndex As Short, ByRef fenviarAyS As Boolean)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		With UserList(UserIndex)
			If Not .flags.Privilegios And Declaraciones.PlayerType.User Then Exit Sub
			
			'Sed
			If .Stats.MinAGU > 0 Then
				If .Counters.AGUACounter < IntervaloSed Then
					.Counters.AGUACounter = .Counters.AGUACounter + 1
				Else
					.Counters.AGUACounter = 0
					.Stats.MinAGU = .Stats.MinAGU - 10
					
					If .Stats.MinAGU <= 0 Then
						.Stats.MinAGU = 0
						.flags.Sed = 1
					End If
					
					fenviarAyS = True
				End If
			End If
			
			'hambre
			If .Stats.MinHam > 0 Then
				If .Counters.COMCounter < IntervaloHambre Then
					.Counters.COMCounter = .Counters.COMCounter + 1
				Else
					.Counters.COMCounter = 0
					.Stats.MinHam = .Stats.MinHam - 10
					If .Stats.MinHam <= 0 Then
						.Stats.MinHam = 0
						.flags.Hambre = 1
					End If
					fenviarAyS = True
				End If
			End If
		End With
		
	End Sub
	
	Public Sub Sanar(ByVal UserIndex As Short, ByRef EnviarStats As Boolean, ByVal Intervalo As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim mashit As Short
		With UserList(UserIndex)
			If MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = 1 And MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = 2 And MapData(.Pos.Map, .Pos.X, .Pos.Y).trigger = 4 Then Exit Sub
			
			'con el paso del tiempo va sanando....pero muy lentamente ;-)
			If .Stats.MinHp < .Stats.MaxHp Then
				If .Counters.HPCounter < Intervalo Then
					.Counters.HPCounter = .Counters.HPCounter + 1
				Else
					mashit = RandomNumber(2, Porcentaje(.Stats.MaxSta, 5))
					
					.Counters.HPCounter = 0
					.Stats.MinHp = .Stats.MinHp + mashit
					If .Stats.MinHp > .Stats.MaxHp Then .Stats.MinHp = .Stats.MaxHp
					Call WriteConsoleMsg(UserIndex, "Has sanado.", Protocol.FontTypeNames.FONTTYPE_INFO)
					EnviarStats = True
				End If
			End If
		End With
		
	End Sub
	
	Public Sub CargaNpcsDat()
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim npcfile As String
		
		npcfile = DatPath & "NPCs.dat"
		Call LeerNPCs.Initialize(npcfile)
	End Sub
	
	Sub PasarSegundo()
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		On Error GoTo Errhandler
		Dim i As Integer
		
		For i = 1 To LastUser
			If UserList(i).flags.UserLogged Then
				'Cerrar usuario
				If UserList(i).Counters.Saliendo Then
					UserList(i).Counters.salir = UserList(i).Counters.salir - 1
					If UserList(i).Counters.salir <= 0 Then
						Call WriteConsoleMsg(i, "Gracias por jugar Argentum Online", Protocol.FontTypeNames.FONTTYPE_INFO)
						Call WriteDisconnect(i)
						Call FlushBuffer(i)
						
						Call CloseSocket(i)
					End If
				End If
			End If
		Next i
		Exit Sub
		
Errhandler: 
		Call LogError("Error en PasarSegundo. Err: " & Err.Description & " - " & Err.Number & " - UserIndex: " & i)
		Resume Next
	End Sub
	
	Public Function ReiniciarAutoUpdate() As Double
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		ReiniciarAutoUpdate = Shell(My.Application.Info.DirectoryPath & "\autoupdater\aoau.exe", AppWinStyle.MinimizedNoFocus)
		
	End Function
	
	Public Sub ReiniciarServidor(Optional ByVal EjecutarLauncher As Boolean = True)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		'WorldSave
		Call ES.DoBackUp()
		
		'commit experiencias
		Call mdParty.ActualizaExperiencias()
		
		'Guardar Pjs
		Call GuardarUsuarios()
		
		If EjecutarLauncher Then Shell(My.Application.Info.DirectoryPath & "\launcher.exe")
		
		'Chauuu
		frmMain.Close()
		
	End Sub
	
	
	Sub GuardarUsuarios()
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		haciendoBK = True
		
		Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessagePauseToggle())
		Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg("Servidor> Grabando Personajes", Protocol.FontTypeNames.FONTTYPE_SERVER))
		
		Dim i As Short
		For i = 1 To LastUser
			If UserList(i).flags.UserLogged Then
				Call SaveUser(i, CharPath & UCase(UserList(i).name) & ".chr")
			End If
		Next i
		
		Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessageConsoleMsg("Servidor> Personajes Grabados", Protocol.FontTypeNames.FONTTYPE_SERVER))
		Call SendData(modSendData.SendTarget.ToAll, 0, PrepareMessagePauseToggle())
		
		haciendoBK = False
	End Sub
	
	
	Sub InicializaEstadisticas()
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim Ta As Integer
		Ta = GetTickCount() And &H7FFFFFFF
		
		Call EstadisticasWeb.Inicializa(frmMain.Handle.ToInt32)
		Call EstadisticasWeb.Informar(clsEstadisticasIPC.EstaNotificaciones.CANTIDAD_MAPAS, NumMaps)
		Call EstadisticasWeb.Informar(clsEstadisticasIPC.EstaNotificaciones.CANTIDAD_ONLINE, NumUsers)
		Call EstadisticasWeb.Informar(clsEstadisticasIPC.EstaNotificaciones.UPTIME_SERVER, (Ta - tInicioServer) / 1000)
		Call EstadisticasWeb.Informar(clsEstadisticasIPC.EstaNotificaciones.RECORD_USUARIOS, recordusuarios)
		
	End Sub
	
	Public Sub FreeNPCs()
		'***************************************************
		'Autor: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Releases all NPC Indexes
		'***************************************************
		Dim LoopC As Integer
		
		' Free all NPC indexes
		For LoopC = 1 To MAXNPCS
			Npclist(LoopC).flags.NPCActive = False
		Next LoopC
	End Sub
	
	Public Sub FreeCharIndexes()
		'***************************************************
		'Autor: Juan Martín Sotuyo Dodero (Maraxus)
		'Last Modification: 05/17/06
		'Releases all char indexes
		'***************************************************
		' Free all char indexes (set them all to 0)
		' TODO MIGRA: antes se usaba ZeroMemory. Quizas como esta ahora no sea del todo performante, pero no estoy seguro
		Dim i As Short
		For i = 1 To MAXCHARS
			CharList(i) = 0
		Next i
	End Sub
End Module
