Option Strict Off
Option Explicit On
Module modInvisibles
	
	Public Sub PonerInvisible(ByVal UserIndex As Short, ByVal estado As Boolean)
		UserList(UserIndex).flags.invisible = IIf(estado, 1, 0)
		UserList(UserIndex).flags.Oculto = IIf(estado, 1, 0)
		UserList(UserIndex).Counters.Invisibilidad = 0
		
		Call SetInvisible(UserIndex, UserList(UserIndex).Char_Renamed.CharIndex, Not estado)
		
	End Sub
End Module