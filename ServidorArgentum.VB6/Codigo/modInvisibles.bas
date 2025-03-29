Attribute VB_Name = "modInvisibles"
Option Explicit

Public Sub PonerInvisible(ByVal UserIndex As Integer, ByVal estado As Boolean)
    UserList(UserIndex).flags.invisible = IIf(estado, 1, 0)
    UserList(UserIndex).flags.Oculto = IIf(estado, 1, 0)
    UserList(UserIndex).Counters.Invisibilidad = 0
    
    Call SetInvisible(UserIndex, UserList(UserIndex).Char.CharIndex, Not estado)

End Sub

