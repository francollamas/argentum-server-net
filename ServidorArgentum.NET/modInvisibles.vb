Option Strict Off
Option Explicit On
Module modInvisibles
	
	' 0 = viejo
	' 1 = nuevo
#Const MODO_INVISIBILIDAD = 0
	
	' cambia el estado de invisibilidad a 1 o 0 dependiendo del modo: true o false
	'
	Public Sub PonerInvisible(ByVal UserIndex As Short, ByVal estado As Boolean)
#If MODO_INVISIBILIDAD = 0 Then
		
		UserList(UserIndex).flags.invisible = IIf(estado, 1, 0)
		UserList(UserIndex).flags.Oculto = IIf(estado, 1, 0)
		UserList(UserIndex).Counters.Invisibilidad = 0
		
		Call SetInvisible(UserIndex, UserList(UserIndex).Char_Renamed.CharIndex, Not estado)
		'Call SendData(SendTarget.ToPCArea, UserIndex, PrepareMessageSetInvisible(UserList(UserIndex).Char.CharIndex, Not estado))
		
		
#Else
		'UPGRADE_NOTE: El bloque #If #EndIf no se actualizó porque la expresión Else no dio como resultado True o ni siquiera se evaluó. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="27EE2C3C-05AF-4C04-B2AF-657B4FB6B5FC"'
		
		Dim EstadoActual As Boolean
		
		' Está invisible ?
		EstadoActual = (UserList(UserIndex).flags.invisible = 1)
		
		'If EstadoActual <> Modo Then
		If Modo = True Then
		' Cuando se hace INVISIBLE se les envia a los
		' clientes un Borrar Char
		UserList(UserIndex).flags.invisible = 1
		'        'Call SendData(SendTarget.ToMap, 0, UserList(UserIndex).Pos.Map, "NOVER" & UserList(UserIndex).Char.CharIndex & ",1")
		Call SendData(SendTarget.toMap, UserList(UserIndex).Pos.Map, PrepareMessageCharacterRemove(UserList(UserIndex).Char.CharIndex))
		Else
		
		End If
		'End If
		
#End If
	End Sub
End Module