Option Strict Off
Option Explicit On
Module Statistics
	'**************************************************************
	' modStatistics.bas - Takes statistics on the game for later study.
	'
	' Implemented by Juan Martín Sotuyo Dodero (Maraxus)
	' (juansotuyo@gmail.com)
	'**************************************************************
	
	'**************************************************************************
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
	'**************************************************************************
	
	
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
	Private fragAlignmentLvlData(50, 4) As Integer
	
	'Currency just in case.... chats are way TOO often...
	Private keyOcurrencies(255) As Decimal
	
	Public Sub Initialize()
		'UPGRADE_WARNING: El límite inferior de la matriz trainningInfo ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		ReDim trainningInfo(MaxUsers)
	End Sub
	
	Public Sub UserConnected(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		'A new user connected, load it's trainning time count
		trainningInfo(UserIndex).trainningTime = Val(GetVar(CharPath & UCase(UserList(UserIndex).name) & ".chr", "RESEARCH", "TrainningTime", 30))
		
		trainningInfo(UserIndex).startTick = (GetTickCount() And &H7FFFFFFF)
	End Sub
	
	Public Sub UserDisconnected(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		With trainningInfo(UserIndex)
			'Update trainning time
			.trainningTime = .trainningTime + CShort(CShort(GetTickCount() And &H7FFFFFFF) - .startTick) / 1000
			
			.startTick = (GetTickCount() And &H7FFFFFFF)
			
			'Store info in char file
			Call WriteVar(CharPath & UCase(UserList(UserIndex).name) & ".chr", "RESEARCH", "TrainningTime", CStr(.trainningTime))
		End With
	End Sub
	
	Public Sub UserLevelUp(ByVal UserIndex As Short)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim handle As Short
		handle = FreeFile
		
		With trainningInfo(UserIndex)
			'Log the data
			FileOpen(handle, My.Application.Info.DirectoryPath & "\logs\statistics.log", OpenMode.Append, , OpenShare.Shared)
			
			PrintLine(handle, UCase(UserList(UserIndex).name) & " completó el nivel " & CStr(UserList(UserIndex).Stats.ELV) & " en " & CStr(.trainningTime + CShort(CShort(GetTickCount() And &H7FFFFFFF) - .startTick) / 1000) & " segundos.")
			
			FileClose(handle)
			
			'Reset data
			.trainningTime = 0
			.startTick = (GetTickCount() And &H7FFFFFFF)
		End With
	End Sub
	
	Public Sub StoreFrag(ByVal killer As Short, ByVal victim As Short)
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
			Case Declaraciones.eClass.Assasin
				clase = 1
				
			Case Declaraciones.eClass.Bard
				clase = 2
				
			Case Declaraciones.eClass.Mage
				clase = 3
				
			Case Declaraciones.eClass.Paladin
				clase = 4
				
			Case Declaraciones.eClass.Warrior
				clase = 5
				
			Case Declaraciones.eClass.Cleric
				clase = 6
				
			Case Declaraciones.eClass.Hunter
				clase = 7
				
			Case Else
				Exit Sub
		End Select
		
		Select Case UserList(killer).raza
			Case Declaraciones.eRaza.Elfo
				raza = 1
				
			Case Declaraciones.eRaza.Drow
				raza = 2
				
			Case Declaraciones.eRaza.Enano
				raza = 3
				
			Case Declaraciones.eRaza.Gnomo
				raza = 4
				
			Case Declaraciones.eRaza.Humano
				raza = 5
				
			Case Else
				Exit Sub
		End Select
		
		If UserList(killer).Faccion.ArmadaReal Then
			alignment = 1
		ElseIf UserList(killer).Faccion.FuerzasCaos Then 
			alignment = 2
		ElseIf criminal(killer) Then 
			alignment = 3
		Else
			alignment = 4
		End If
		
		fragLvlRaceData(clase).matrix(UserList(killer).Stats.ELV, raza) = fragLvlRaceData(clase).matrix(UserList(killer).Stats.ELV, raza) + 1
		
		fragLvlLvlData(clase).matrix(UserList(killer).Stats.ELV, UserList(victim).Stats.ELV) = fragLvlLvlData(clase).matrix(UserList(killer).Stats.ELV, UserList(victim).Stats.ELV) + 1
		
		fragAlignmentLvlData(UserList(killer).Stats.ELV, alignment) = fragAlignmentLvlData(UserList(killer).Stats.ELV, alignment) + 1
	End Sub
	
	Public Sub DumpStatistics()
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim handle As Short
		handle = FreeFile
		
		Dim line As String
		Dim i As Integer
		Dim j As Integer
		
		FileOpen(handle, My.Application.Info.DirectoryPath & "\logs\frags.txt", OpenMode.Output)
		
		'Save lvl vs lvl frag matrix for each class - we use GNU Octave's ASCII file format
		
		PrintLine(handle, "# name: fragLvlLvl_Ase")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 50")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 50
			For i = 1 To 50
				line = line & " " & CStr(fragLvlLvlData(1).matrix(i, j))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlLvl_Bar")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 50")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 50
			For i = 1 To 50
				line = line & " " & CStr(fragLvlLvlData(2).matrix(i, j))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlLvl_Mag")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 50")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 50
			For i = 1 To 50
				line = line & " " & CStr(fragLvlLvlData(3).matrix(i, j))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlLvl_Pal")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 50")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 50
			For i = 1 To 50
				line = line & " " & CStr(fragLvlLvlData(4).matrix(i, j))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlLvl_Gue")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 50")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 50
			For i = 1 To 50
				line = line & " " & CStr(fragLvlLvlData(5).matrix(i, j))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlLvl_Cle")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 50")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 50
			For i = 1 To 50
				line = line & " " & CStr(fragLvlLvlData(6).matrix(i, j))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlLvl_Caz")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 50")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 50
			For i = 1 To 50
				line = line & " " & CStr(fragLvlLvlData(7).matrix(i, j))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		
		
		
		
		'Save lvl vs race frag matrix for each class - we use GNU Octave's ASCII file format
		
		PrintLine(handle, "# name: fragLvlRace_Ase")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 5")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 5
			For i = 1 To 50
				line = line & " " & CStr(fragLvlRaceData(1).matrix(i, j))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlRace_Bar")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 5")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 5
			For i = 1 To 50
				line = line & " " & CStr(fragLvlRaceData(2).matrix(i, j))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlRace_Mag")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 5")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 5
			For i = 1 To 50
				line = line & " " & CStr(fragLvlRaceData(3).matrix(i, j))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlRace_Pal")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 5")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 5
			For i = 1 To 50
				line = line & " " & CStr(fragLvlRaceData(4).matrix(i, j))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlRace_Gue")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 5")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 5
			For i = 1 To 50
				line = line & " " & CStr(fragLvlRaceData(5).matrix(i, j))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlRace_Cle")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 5")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 5
			For i = 1 To 50
				line = line & " " & CStr(fragLvlRaceData(6).matrix(i, j))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlRace_Caz")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 5")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 5
			For i = 1 To 50
				line = line & " " & CStr(fragLvlRaceData(7).matrix(i, j))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		
		
		
		
		
		'Save lvl vs class frag matrix for each race - we use GNU Octave's ASCII file format
		
		PrintLine(handle, "# name: fragLvlClass_Elf")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 7")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 7
			For i = 1 To 50
				line = line & " " & CStr(fragLvlRaceData(j).matrix(i, 1))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlClass_Dar")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 7")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 7
			For i = 1 To 50
				line = line & " " & CStr(fragLvlRaceData(j).matrix(i, 2))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlClass_Dwa")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 7")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 7
			For i = 1 To 50
				line = line & " " & CStr(fragLvlRaceData(j).matrix(i, 3))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlClass_Gno")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 7")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 7
			For i = 1 To 50
				line = line & " " & CStr(fragLvlRaceData(j).matrix(i, 4))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		PrintLine(handle, "# name: fragLvlClass_Hum")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 7")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 7
			For i = 1 To 50
				line = line & " " & CStr(fragLvlRaceData(j).matrix(i, 5))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		
		
		
		'Save lvl vs alignment frag matrix for each race - we use GNU Octave's ASCII file format
		
		PrintLine(handle, "# name: fragAlignmentLvl")
		PrintLine(handle, "# type: matrix")
		PrintLine(handle, "# rows: 4")
		PrintLine(handle, "# columns: 50")
		
		For j = 1 To 4
			For i = 1 To 50
				line = line & " " & CStr(fragAlignmentLvlData(i, j))
			Next i
			
			PrintLine(handle, line)
			line = vbNullString
		Next j
		
		FileClose(handle)
		
		
		
		'Dump Chat statistics
		handle = FreeFile
		
		FileOpen(handle, My.Application.Info.DirectoryPath & "\logs\huffman.log", OpenMode.Output)
		
		Dim Total As Decimal
		
		'Compute total characters
		For i = 0 To 255
			Total = Total + keyOcurrencies(i)
		Next i
		
		'Show each character's ocurrencies
		If Total <> 0 Then
			For i = 0 To 255
				PrintLine(handle, CStr(i) & "    " & CStr(System.Math.Round(keyOcurrencies(i) / Total, 8)))
			Next i
		End If
		
		PrintLine(handle, "TOTAL =    " & CStr(Total))
		
		FileClose(handle)
	End Sub
	
	Public Sub ParseChat(ByRef S As String)
		'***************************************************
		'Author: Unknown
		'Last Modification: -
		'
		'***************************************************
		
		Dim i As Integer
		Dim key As Short
		
		For i = 1 To Len(S)
			key = Asc(Mid(S, i, 1))
			
			keyOcurrencies(key) = keyOcurrencies(key) + 1
		Next i
		
		'Add a NULL-terminated to consider that possibility too....
		keyOcurrencies(0) = keyOcurrencies(0) + 1
	End Sub
End Module
