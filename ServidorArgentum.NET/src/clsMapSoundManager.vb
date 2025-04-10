Option Strict Off
Option Explicit On
Friend Class SoundMapInfo
	'**************************************************************
	' SoundMapInfo.cls
	'
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
	
	
	Private Structure p_tSoundMapInfo
		Dim Cantidad As Short
		Dim SoundIndex() As Short
		Dim flags() As Integer
		Dim Probabilidad() As Single
	End Structure
	
	Private Enum p_eSoundFlags
		ninguna = 0
		Lluvia = 1
	End Enum
	
	Private p_Mapas() As p_tSoundMapInfo
	
	
	'sonidos conocidos, pasados a enum para intelisense
	Public Enum e_SoundIndex
		MUERTE_HOMBRE = 11
		MUERTE_MUJER = 74
		FLECHA_IMPACTO = 65
		CONVERSION_BARCO = 55
		MORFAR_MANZANA = 82
		SOUND_COMIDA = 7
	End Enum
	
	'UPGRADE_NOTE: Class_Initialize se actualizó a Class_Initialize_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
	Private Sub Class_Initialize_Renamed()
		'armar el array
		'UPGRADE_WARNING: El límite inferior de la matriz p_Mapas ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
		ReDim p_Mapas(NumMaps)
		Call LoadSoundMapInfo()
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	Public Sub LoadSoundMapInfo()
		Dim i As Short
		Dim j As Short
		Dim Temps As String
		Dim MAPFILE As String
		
		MAPFILE = AppDomain.CurrentDomain.BaseDirectory & MapPath & "MAPA"
		
		'Usage of Val() prevents errors when dats are corrputed or incomplete. All invalid values are assumed to be zero.
		
		'TODO : Log the error in the dat for correction.
		For i = 1 To UBound(p_Mapas)
			Temps = GetVar(MAPFILE & i & ".dat", "SONIDOS", "Cantidad")
			
			If IsNumeric(Temps) Then
				p_Mapas(i).Cantidad = Val(Temps)
				
				'UPGRADE_WARNING: El límite inferior de la matriz p_Mapas(i).flags ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
				ReDim p_Mapas(i).flags(p_Mapas(i).Cantidad)
				'UPGRADE_WARNING: El límite inferior de la matriz p_Mapas(i).Probabilidad ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
				ReDim p_Mapas(i).Probabilidad(p_Mapas(i).Cantidad)
				'UPGRADE_WARNING: El límite inferior de la matriz p_Mapas(i).SoundIndex ha cambiado de 1 a 0. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"'
				ReDim p_Mapas(i).SoundIndex(p_Mapas(i).Cantidad)
				
				For j = 1 To p_Mapas(i).Cantidad
					p_Mapas(i).flags(j) = Val(GetVar(MAPFILE & i & ".dat", "SONIDO" & j, "Flags"))
					p_Mapas(i).Probabilidad(j) = Val(GetVar(MAPFILE & i & ".dat", "SONIDO" & j, "Probabilidad"))
					p_Mapas(i).SoundIndex(j) = Val(GetVar(MAPFILE & i & ".dat", "SONIDO" & j, "Sonido"))
				Next j
			Else
				p_Mapas(i).Cantidad = 0
			End If
		Next i
	End Sub
	
	Public Sub ReproducirSonidosDeMapas()
		Dim i As Integer
		Dim SonidoMapa As Byte
		Dim posX As Byte
		Dim posY As Byte
		
		'Sounds are played at a random position
		posX = RandomNumber(XMinMapSize, XMaxMapSize)
		posY = RandomNumber(YMinMapSize, YMaxMapSize)
		
		For i = 1 To UBound(p_Mapas)
			If p_Mapas(i).Cantidad > 0 Then
				SonidoMapa = RandomNumber(1, p_Mapas(i).Cantidad)
				If RandomNumber(1, 100) <= p_Mapas(i).Probabilidad(SonidoMapa) Then
					'tocarlo
					If Lloviendo Then
						If p_Mapas(i).flags(SonidoMapa) Xor p_eSoundFlags.Lluvia Then
							Call SendData(modSendData.SendTarget.toMap, i, PrepareMessagePlayWave(p_Mapas(i).SoundIndex(SonidoMapa), posX, posY))
						End If
					Else
						If p_Mapas(i).flags(SonidoMapa) Xor p_eSoundFlags.ninguna Then
							Call SendData(modSendData.SendTarget.toMap, i, PrepareMessagePlayWave(p_Mapas(i).SoundIndex(SonidoMapa), posX, posY))
						End If
					End If
				End If
			End If
		Next i
	End Sub
	
	Public Sub ReproducirSonido(ByVal Destino As modSendData.SendTarget, ByVal index As Short, ByVal SoundIndex As Short)
		Call SendData(Destino, index, PrepareMessagePlayWave(SoundIndex, UserList(index).Pos.X, UserList(index).Pos.Y))
	End Sub
End Class
