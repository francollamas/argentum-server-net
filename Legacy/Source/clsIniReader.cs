using System;
using System.IO;
using Microsoft.VisualBasic.CompilerServices;

namespace Legacy;

internal class clsIniReader
{
    // '
    // Containts all Main sections of the loaded INI file
    private MainNode[] fileData;

    // '
    // Stores the total number of main sections in the loaded INI file
    private int MainNodes;

    public clsIniReader()
    {
        Class_Initialize();
    }

    // '
    // Default constructor. Does nothing.

    private void Class_Initialize()
    {
        // **************************************************************
        // Author: Juan Martín Sotuyo Dodero
        // Last Modify Date: 5/01/2006
        // 
        // **************************************************************
    }

    // '
    // Destroy every array and deallocates al memory.
    // 

    private void Class_Terminate()
    {
        // **************************************************************
        // Author: Juan Martín Sotuyo Dodero
        // Last Modify Date: 5/01/2006
        // 
        // **************************************************************
        int i;

        // Clean up
        if (MainNodes != 0)
        {
            var loopTo = MainNodes - 1;
            for (i = 1; i <= loopTo; i++)
                fileData[i].values = null;

            fileData = null;
        }

        MainNodes = 0;
    }

    ~clsIniReader()
    {
        Class_Terminate();
    }

    // '
    // Loads a INI file so it's values can be read. Must be called before being able to use GetValue.
    // 
    // @param    file Complete path of the INI file to be loaded.
    // @see      GetValue

    public void Initialize(string file)
    {
        // **************************************************************
        // Author: Juan Martín Sotuyo Dodero
        // Last Modify Date: 27/07/2006
        // Opens the requested file and loads it's data into memory
        // **************************************************************
        string Text;
        int Pos;

        // Prevent memory losses if we are attempting to reload a file....
        Class_Terminate();

        // Read line by line until the end
        using (var reader = new StreamReader(file))
        {
            while (!reader.EndOfStream)
            {
                Text = reader.ReadLine();

                // Is it null??
                if (Text.Length != 0)
                {
                    // If it starts with '[' it is a main node or nothing (GetPrivateProfileStringA works this way), otherwise it's a value
                    if (Text.Substring(0, 1) == "[")
                    {
                        // If it has an ending ']' it's a main node, otherwise it's nothing
                        Pos = Text.IndexOf("]", 1) + 1;
                        if (Pos != 0)
                        {
                            // Add a main node
                            Array.Resize(ref fileData, MainNodes + 1);

                            fileData[MainNodes].name = Text.Substring(1, Pos - 2).Trim().ToUpper();

                            MainNodes = MainNodes + 1;
                        }
                    }
                    else
                    {
                        // So it's a value. Check if it has a '=', otherwise it's nothing
                        Pos = Text.IndexOf("=", 1) + 1;
                        if (Pos != 0)
                            // Is it under any main node??
                            if (MainNodes != 0)
                            {
                                ref var withBlock = ref fileData[MainNodes - 1];
                                // Add it to the main node's value
                                Array.Resize(ref withBlock.values, withBlock.numValues + 1);

                                withBlock.values[withBlock.numValues].Value = Text.Substring(Pos);
                                withBlock.values[withBlock.numValues].Key = Text.Substring(0, Pos - 1).ToUpper();

                                withBlock.numValues = Convert.ToInt16(withBlock.numValues + 1);
                            }
                    }
                }
            }
        }

        int i;

        if (MainNodes != 0)
        {
            // Sort main nodes to allow binary search
            SortMainNodes(0, Convert.ToInt16(MainNodes - 1));

            // Sort values of each node to allow binary search
            var loopTo = MainNodes - 1;
            for (i = 0; i <= loopTo; i++)
                if (fileData[i].numValues != 0)
                    SortChildNodes(ref fileData[i], 0, Convert.ToInt16(fileData[i].numValues - 1));
        }
    }

    // '
    // Sorts all child nodes within the given MainNode alphabetically by their keys. Uses quicksort.
    // 
    // @param    Node The MainNode whose values are to be sorted.
    // @param    first The first index to consider when sorting.
    // @param    last The last index to be considered when sorting.

    private void SortChildNodes(ref MainNode Node, short First, short Last)
    {
        // **************************************************************
        // Author: Juan Martín Sotuyo Dodero
        // Last Modify Date: 5/01/2006
        // Sorts the list of values in a given MainNode using quicksort,
        // this allows the use of Binary Search for faster searches
        // **************************************************************
        short min; // First item in the list
        short max; // Last item in the list
        string comp; // Item used to compare
        ChildNode temp;

        min = First;
        max = Last;

        {
            ref var withBlock = ref Node;
            comp = withBlock.values[(min + max) / 2].Key;

            while (min <= max)
            {
                while ((Operators.CompareString(withBlock.values[min].Key, comp, false) < 0) & (min < Last))
                    min = Convert.ToInt16(min + 1);
                while ((Operators.CompareString(withBlock.values[max].Key, comp, false) > 0) & (max > First))
                    max = Convert.ToInt16(max - 1);
                if (min <= max)
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto temp. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    temp = withBlock.values[min];
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Node.values(min). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    withBlock.values[min] = withBlock.values[max];
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Node.values(max). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    withBlock.values[max] = temp;
                    min = Convert.ToInt16(min + 1);
                    max = Convert.ToInt16(max - 1);
                }
            }
        }

        if (First < max)
            SortChildNodes(ref Node, First, max);
        if (min < Last)
            SortChildNodes(ref Node, min, Last);
    }

    // '
    // Sorts all main nodes in the loaded INI file alphabetically by their names. Uses quicksort.
    // 
    // @param    first The first index to consider when sorting.
    // @param    last The last index to be considered when sorting.

    private void SortMainNodes(short First, short Last)
    {
        // **************************************************************
        // Author: Juan Martín Sotuyo Dodero
        // Last Modify Date: 5/01/2006
        // Sorts the MainNodes list using quicksort,
        // this allows the use of Binary Search for faster searches
        // **************************************************************
        short min; // First item in the list
        short max; // Last item in the list
        string comp; // Item used to compare
        // UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura temp, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        MainNode temp;

        min = First;
        max = Last;

        comp = fileData[(min + max) / 2].name;

        while (min <= max)
        {
            while ((Operators.CompareString(fileData[min].name, comp, false) < 0) & (min < Last))
                min = Convert.ToInt16(min + 1);
            while ((Operators.CompareString(fileData[max].name, comp, false) > 0) & (max > First))
                max = Convert.ToInt16(max - 1);
            if (min <= max)
            {
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto temp. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                temp = fileData[min];
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto fileData(min). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                fileData[min] = fileData[max];
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto fileData(max). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                fileData[max] = temp;
                min = Convert.ToInt16(min + 1);
                max = Convert.ToInt16(max - 1);
            }
        }

        if (First < max)
            SortMainNodes(First, max);
        if (min < Last)
            SortMainNodes(min, Last);
    }

    // '
    // Searches for a given key within a given main section and if it exists retrieves it's value, otherwise a null string
    // 
    // @param    Main The name of the main section in which we will be searching.
    // @param    key The key of the value we are looking for.
    // @returns  The value asociated with the given key under the requeted main section of the INI file or a null string if it's not found.

    public string GetValue(string Main, string Key)
    {
        string GetValueRet = default;
        // **************************************************************
        // Author: Juan Martín Sotuyo Dodero
        // Last Modify Date: 5/01/2006
        // Returns a value if the key and main node exist, or a nullstring otherwise
        // **************************************************************
        int i;
        int j;

        // Search for the main node
        i = FindMain(Main.ToUpper());

        if (i >= 0)
        {
            // If valid, binary search among keys
            j = FindKey(ref fileData[i], Key.ToUpper());

            // If we found it we return it
            if (j >= 0)
                GetValueRet = fileData[i].values[j].Value;
        }

        return GetValueRet;
    }

    // '
    // Searches for a given key within a given main section and if it exists retrieves it's value, otherwise a null string
    // 
    // @param    Main The name of the main section in which we will be searching.
    // @param    key The key of the value we are looking for.
    // @returns  The value asociated with the given key under the requeted main section of the INI file or a null string if it's not found.

    public void ChangeValue(string Main, string Key, int Value)
    {
        // **************************************************************
        // Author: ZaMa
        // Last Modify Date: 27/05/2009
        // If the key and main node exist, changes the value
        // **************************************************************
        int i;
        int j;

        // Search for the main node
        i = FindMain(Main.ToUpper());

        if (i >= 0)
        {
            // If valid, binary search among keys
            j = FindKey(ref fileData[i], Key.ToUpper());

            // If we found it we change it
            if (j >= 0)
                fileData[i].values[j].Value = Value.ToString();
        }
    }

    // '
    // Searches for a given key within a given main node and returns the index in which it's stored or the negation of the index in which it should be if not found.
    // 
    // @param    Node The MainNode among whose value entries we will be searching.
    // @param    key The key of the value we are looking for.
    // @returns  The index in which the value with the key we are looking for is stored or the negation of the index in which it should be if not found.

    private int FindKey(ref MainNode Node, string Key)
    {
        int FindKeyRet = default;
        // **************************************************************
        // Author: Juan Martín Sotuyo Dodero
        // Last Modify Date: 5/01/2006
        // Returns the index of the value which key matches the requested one,
        // or the negation of the position were it should be if not found
        // **************************************************************
        int min;
        int max;
        var mid = default(int);

        min = 0;
        max = Node.numValues - 1;

        while (min <= max)
        {
            mid = (min + max) / 2;

            if (Operators.CompareString(Node.values[mid].Key, Key, false) < 0)
            {
                min = mid + 1;
            }
            else if (Operators.CompareString(Node.values[mid].Key, Key, false) > 0)
            {
                max = mid - 1;
            }
            else
            {
                // We found it
                FindKeyRet = mid;
                return FindKeyRet;
            }
        }

        // Not found, return the negation of the position where it should be
        // (all higher values are to the right of the list and lower values are to the left)
        FindKeyRet = ~mid;
        return FindKeyRet;
    }

    // '
    // Searches for a main section with the given name within the loaded INI file and returns the index in which it's stored or the negation of the index in which it should be if not found.
    // 
    // @param    name The name of the MainNode we are looking for.
    // @returns  The index in which the main section we are looking for is stored or the negation of the index in which it should be if not found.

    private int FindMain(string name)
    {
        int FindMainRet = default;
        // **************************************************************
        // Author: Juan Martín Sotuyo Dodero
        // Last Modify Date: 5/01/2006
        // Returns the index of the MainNode which name matches the requested one,
        // or the negation of the position were it should be if not found
        // **************************************************************
        int min;
        int max;
        var mid = default(int);

        min = 0;
        max = MainNodes - 1;

        while (min <= max)
        {
            mid = (min + max) / 2;

            if (Operators.CompareString(fileData[mid].name, name, false) < 0)
            {
                min = mid + 1;
            }
            else if (Operators.CompareString(fileData[mid].name, name, false) > 0)
            {
                max = mid - 1;
            }
            else
            {
                // We found it
                FindMainRet = mid;
                return FindMainRet;
            }
        }

        // Not found, return the negation of the position where it should be
        // (all higher values are to the right of the list and lower values are to the left)
        FindMainRet = ~mid;
        return FindMainRet;
    }

    // '
    // Checks wether a given key exists or not.
    // 
    // @param    name    The name of the element whose existance is being checked.
    // @returns  True if the key exists, false otherwise.

    public bool KeyExists(string name)
    {
        bool KeyExistsRet = default;
        // **************************************************************
        // Author: Juan Martín Sotuyo Dodero
        // Last Modify Date: 04/01/2008
        // Returns true of the key exists, false otherwise.
        // **************************************************************
        KeyExistsRet = FindMain(name.ToUpper()) >= 0;
        return KeyExistsRet;
    }
    // **************************************************************
    // clsIniReader.cls - Loads INI files into memory and applies Binary Search to get values at high speed.
    // Use it instead of GetVar when reading several values form the same file at once, otherwise it's not usefull.
    // Based in the idea of AlejoLP and his clsLeerInis.
    // 
    // Designed and implemented by Juan Martín Sotuyo Dodero (Maraxus)
    // (juansotuyo@gmail.com)
    // **************************************************************

    // **************************************************************************
    // This program is free software; you can redistribute it and/or modify
    // it under the terms of the Affero General Public License;
    // either version 1 of the License, or any later version.
    // 
    // This program is distributed in the hope that it will be useful,
    // but WITHOUT ANY WARRANTY; without even the implied warranty of
    // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    // Affero General Public License for more details.
    // 
    // You should have received a copy of the Affero General Public License
    // along with this program; if not, you can find it at http://www.affero.org/oagpl.html
    // **************************************************************************

    // '
    // Loads a complete INI file into memory and sorts it's data and keys for faster searches.
    // It is MUCH faster than GetPrivateProfileStringA if you search for several values within a file, otherwise stick to the API.
    // It's particularly usefull when you can keep a whole file in memory such as NPCs.dat'
    // Based in the idea of AlejoLP and his clsLeerInis.
    // 
    // @author Juan Martín Sotuyo Dodero (Maraxus) juansotuyo@gmail.com
    // @version 1.1.0
    // @date 20060501

    // 01/05/2006 - Juan Martín Sotuyo Dodero (Maraxus) - (juansotuyo@gmail.com)
    // - First Release
    // 
    // 01/04/2008 - Juan Martín Sotuyo Dodero (Maraxus) - (juansotuyo@gmail.com)
    // - Add: KeyExists method allows to check for valid section keys.

    // '
    // Structure that contains a value and it's key in a INI file
    // 
    // @param    key String containing the key associated to the value.
    // @param    value String containing the value of the INI entry.
    // @see      MainNode
    // 

    private struct ChildNode
    {
        public string Key;
        public string Value;
    }

    // '
    // Structure that contains all info under a tag in a INI file.
    // Such tags are indicated with the "[" and "]" characters.
    // 
    // @param    name String containing the text within the "[" and "]" characters.
    // It's the key used when searching for a main section of the INI data.
    // @param    values Array of ChildNodes, each containing a value entry along with it's key.
    // @param    numValues Number of entrys in the main node.

    private struct MainNode
    {
        public string name;
        public ChildNode[] values;
        public short numValues;
    }
}