Option Strict Off
Option Explicit On
Module Characters
    ''
    ' Value representing invalid indexes.
    Public Const INVALID_INDEX As Short = 0

    ''
    ' Retrieves the UserList index of the user with the give char index.
    '
    ' @param    CharIndex   The char index being used by the user to be retrieved.
    ' @return   The index of the user with the char placed in CharIndex or INVALID_INDEX if it's not a user or valid char index.
    ' @see      INVALID_INDEX

    Public Function CharIndexToUserIndex(ByVal CharIndex As Short) As Short
        '***************************************************
        'Autor: Juan Martín Sotuyo Dodero (Maraxus)
        'Last Modification: 05/17/06
        'Takes a CharIndex and transforms it into a UserIndex. Returns INVALID_INDEX in case of error.
        '***************************************************
        CharIndexToUserIndex = CharList(CharIndex)

        If CharIndexToUserIndex < 1 Or CharIndexToUserIndex > MaxUsers Then
            CharIndexToUserIndex = INVALID_INDEX
            Exit Function
        End If

        If UserList(CharIndexToUserIndex).Char_Renamed.CharIndex <> CharIndex Then
            CharIndexToUserIndex = INVALID_INDEX
            Exit Function
        End If
    End Function
End Module
