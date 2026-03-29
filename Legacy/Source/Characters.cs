namespace Legacy;

internal static class Characters
{
    // '
    // Value representing invalid indexes.
    public const short INVALID_INDEX = 0;

    // '
    // Retrieves the UserList index of the user with the give char index.
    // 
    // @param    CharIndex   The char index being used by the user to be retrieved.
    // @return   The index of the user with the char placed in CharIndex or INVALID_INDEX if it's not a user or valid char index.
    // @see      INVALID_INDEX

    public static short CharIndexToUserIndex(short CharIndex)
    {
        short CharIndexToUserIndexRet = default;
        // ***************************************************
        // Autor: Juan Martín Sotuyo Dodero (Maraxus)
        // Last Modification: 05/17/06
        // Takes a CharIndex and transforms it into a UserIndex. Returns INVALID_INDEX in case of error.
        // ***************************************************
        CharIndexToUserIndexRet = Declaraciones.CharList[CharIndex];

        if ((CharIndexToUserIndexRet < 1) | (CharIndexToUserIndexRet > Declaraciones.MaxUsers))
        {
            CharIndexToUserIndexRet = INVALID_INDEX;
            return CharIndexToUserIndexRet;
        }

        if (Declaraciones.UserList[CharIndexToUserIndexRet].Char_Renamed.CharIndex != CharIndex)
        {
            CharIndexToUserIndexRet = INVALID_INDEX;
            return CharIndexToUserIndexRet;
        }

        return CharIndexToUserIndexRet;
    }
}