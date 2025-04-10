Option Strict Off
Option Explicit On
Friend Class clsIniReader
    '**************************************************************
    ' clsIniReader.cls - Loads INI files into memory and applies Binary Search to get values at high speed.
    ' Use it instead of GetVar when reading several values form the same file at once, otherwise it's not usefull.
    ' Based in the idea of AlejoLP and his clsLeerInis.
    '
    ' Designed and implemented by Juan Martín Sotuyo Dodero (Maraxus)
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

    ''
    'Loads a complete INI file into memory and sorts it's data and keys for faster searches.
    'It is MUCH faster than GetPrivateProfileStringA if you search for several values within a file, otherwise stick to the API.
    'It's particularly usefull when you can keep a whole file in memory such as NPCs.dat'
    ' Based in the idea of AlejoLP and his clsLeerInis.
    '
    ' @author Juan Martín Sotuyo Dodero (Maraxus) juansotuyo@gmail.com
    ' @version 1.1.0
    ' @date 20060501

    '01/05/2006 - Juan Martín Sotuyo Dodero (Maraxus) - (juansotuyo@gmail.com)
    '   - First Release
    '
    '01/04/2008 - Juan Martín Sotuyo Dodero (Maraxus) - (juansotuyo@gmail.com)
    '   - Add: KeyExists method allows to check for valid section keys.

    ''
    'Structure that contains a value and it's key in a INI file
    '
    ' @param    key String containing the key associated to the value.
    ' @param    value String containing the value of the INI entry.
    ' @see      MainNode
    '

    Private Structure ChildNode
        Dim Key As String
        Dim Value As String
    End Structure

    ''
    'Structure that contains all info under a tag in a INI file.
    'Such tags are indicated with the "[" and "]" characters.
    '
    ' @param    name String containing the text within the "[" and "]" characters.
    'It's the key used when searching for a main section of the INI data.
    ' @param    values Array of ChildNodes, each containing a value entry along with it's key.
    ' @param    numValues Number of entrys in the main node.

    Private Structure MainNode
        Dim name As String
        Dim values() As ChildNode
        Dim numValues As Short
    End Structure

    ''
    'Containts all Main sections of the loaded INI file
    Private fileData() As MainNode

    ''
    'Stores the total number of main sections in the loaded INI file
    Private MainNodes As Integer

    ''
    'Default constructor. Does nothing.

    'UPGRADE_NOTE: Class_Initialize se actualizó a Class_Initialize_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Private Sub Class_Initialize_Renamed()
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero
        'Last Modify Date: 5/01/2006
        '
        '**************************************************************
    End Sub

    Public Sub New()
        MyBase.New()
        Class_Initialize_Renamed()
    End Sub

    ''
    'Destroy every array and deallocates al memory.
    '

    'UPGRADE_NOTE: Class_Terminate se actualizó a Class_Terminate_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Private Sub Class_Terminate_Renamed()
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero
        'Last Modify Date: 5/01/2006
        '
        '**************************************************************
        Dim i As Integer

        'Clean up
        If MainNodes Then
            For i = 1 To MainNodes - 1
                Erase fileData(i).values
            Next i

            Erase fileData
        End If

        MainNodes = 0
    End Sub

    Protected Overrides Sub Finalize()
        Class_Terminate_Renamed()
        MyBase.Finalize()
    End Sub

    ''
    'Loads a INI file so it's values can be read. Must be called before being able to use GetValue.
    '
    ' @param    file Complete path of the INI file to be loaded.
    ' @see      GetValue

    Public Sub Initialize(ByVal file As String)
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero
        'Last Modify Date: 27/07/2006
        'Opens the requested file and loads it's data into memory
        '**************************************************************
        Dim handle As Short
        Dim Text As String
        Dim Pos As Integer

        'Prevent memory losses if we are attempting to reload a file....
        Call Class_Terminate_Renamed()

        'Get a free handle and start reading line by line until the end
        handle = FreeFile

        FileOpen(handle, file, OpenMode.Input)

        Do Until EOF(handle)
            Text = LineInput(handle)

            'Is it null??
            If Len(Text) Then
                'If it starts with '[' it is a main node or nothing (GetPrivateProfileStringA works this way), otherwise it's a value
                If Left(Text, 1) = "[" Then
                    'If it has an ending ']' it's a main node, otherwise it's nothing
                    Pos = InStr(2, Text, "]")
                    If Pos Then
                        'Add a main node
                        ReDim Preserve fileData(MainNodes)

                        fileData(MainNodes).name = UCase(Trim(Mid(Text, 2, Pos - 2)))

                        MainNodes = MainNodes + 1
                    End If
                Else
                    'So it's a value. Check if it has a '=', otherwise it's nothing
                    Pos = InStr(2, Text, "=")
                    If Pos Then
                        'Is it under any main node??
                        If MainNodes Then
                            With fileData(MainNodes - 1)
                                'Add it to the main node's value
                                ReDim Preserve .values(.numValues)

                                .values(.numValues).Value = Right(Text, Len(Text) - Pos)
                                .values(.numValues).Key = UCase(Left(Text, Pos - 1))

                                .numValues = .numValues + 1
                            End With
                        End If
                    End If
                End If
            End If
        Loop

        FileClose(handle)

        Dim i As Integer

        If MainNodes Then
            'Sort main nodes to allow binary search
            Call SortMainNodes(0, MainNodes - 1)

            'Sort values of each node to allow binary search
            For i = 0 To MainNodes - 1
                If fileData(i).numValues Then Call SortChildNodes(fileData(i), 0, fileData(i).numValues - 1)
            Next i
        End If
    End Sub

    ''
    'Sorts all child nodes within the given MainNode alphabetically by their keys. Uses quicksort.
    '
    ' @param    Node The MainNode whose values are to be sorted.
    ' @param    first The first index to consider when sorting.
    ' @param    last The last index to be considered when sorting.

    Private Sub SortChildNodes(ByRef Node As MainNode, ByVal First As Short, ByVal Last As Short)
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero
        'Last Modify Date: 5/01/2006
        'Sorts the list of values in a given MainNode using quicksort,
        'this allows the use of Binary Search for faster searches
        '**************************************************************
        Dim min As Short 'First item in the list
        Dim max As Short 'Last item in the list
        Dim comp As String 'Item used to compare
        Dim temp As ChildNode

        min = First
        max = Last

        With Node
            comp = .values((min + max)\2).Key

            Do While min <= max
                Do While .values(min).Key < comp And min < Last
                    min = min + 1
                Loop
                Do While .values(max).Key > comp And max > First
                    max = max - 1
                Loop
                If min <= max Then
                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto temp. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    temp = .values(min)
                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Node.values(min). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    .values(min) = .values(max)
                    'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Node.values(max). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    .values(max) = temp
                    min = min + 1
                    max = max - 1
                End If
            Loop
        End With

        If First < max Then SortChildNodes(Node, First, max)
        If min < Last Then SortChildNodes(Node, min, Last)
    End Sub

    ''
    'Sorts all main nodes in the loaded INI file alphabetically by their names. Uses quicksort.
    '
    ' @param    first The first index to consider when sorting.
    ' @param    last The last index to be considered when sorting.

    Private Sub SortMainNodes(ByVal First As Short, ByVal Last As Short)
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero
        'Last Modify Date: 5/01/2006
        'Sorts the MainNodes list using quicksort,
        'this allows the use of Binary Search for faster searches
        '**************************************************************
        Dim min As Short 'First item in the list
        Dim max As Short 'Last item in the list
        Dim comp As String 'Item used to compare
        'UPGRADE_WARNING: Puede que necesite inicializar las matrices de la estructura temp, antes de poder utilizarlas. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="814DF224-76BD-4BB4-BFFB-EA359CB9FC48"'
        Dim temp As MainNode

        min = First
        max = Last

        comp = fileData((min + max)\2).name

        Do While min <= max
            Do While fileData(min).name < comp And min < Last
                min = min + 1
            Loop
            Do While fileData(max).name > comp And max > First
                max = max - 1
            Loop
            If min <= max Then
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto temp. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                temp = fileData(min)
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto fileData(min). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                fileData(min) = fileData(max)
                'UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto fileData(max). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                fileData(max) = temp
                min = min + 1
                max = max - 1
            End If
        Loop

        If First < max Then SortMainNodes(First, max)
        If min < Last Then SortMainNodes(min, Last)
    End Sub

    ''
    'Searches for a given key within a given main section and if it exists retrieves it's value, otherwise a null string
    '
    ' @param    Main The name of the main section in which we will be searching.
    ' @param    key The key of the value we are looking for.
    ' @returns  The value asociated with the given key under the requeted main section of the INI file or a null string if it's not found.

    Public Function GetValue(ByVal Main As String, ByVal Key As String) As String
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero
        'Last Modify Date: 5/01/2006
        'Returns a value if the key and main node exist, or a nullstring otherwise
        '**************************************************************
        Dim i As Integer
        Dim j As Integer

        'Search for the main node
        i = FindMain(UCase(Main))

        If i >= 0 Then
            'If valid, binary search among keys
            j = FindKey(fileData(i), UCase(Key))

            'If we found it we return it
            If j >= 0 Then GetValue = fileData(i).values(j).Value
        End If
    End Function

    ''
    'Searches for a given key within a given main section and if it exists retrieves it's value, otherwise a null string
    '
    ' @param    Main The name of the main section in which we will be searching.
    ' @param    key The key of the value we are looking for.
    ' @returns  The value asociated with the given key under the requeted main section of the INI file or a null string if it's not found.

    Public Sub ChangeValue(ByVal Main As String, ByVal Key As String, ByVal Value As Integer)
        '**************************************************************
        'Author: ZaMa
        'Last Modify Date: 27/05/2009
        'If the key and main node exist, changes the value
        '**************************************************************
        Dim i As Integer
        Dim j As Integer

        'Search for the main node
        i = FindMain(UCase(Main))

        If i >= 0 Then
            'If valid, binary search among keys
            j = FindKey(fileData(i), UCase(Key))

            'If we found it we change it
            If j >= 0 Then fileData(i).values(j).Value = CStr(Value)
        End If
    End Sub

    ''
    'Searches for a given key within a given main node and returns the index in which it's stored or the negation of the index in which it should be if not found.
    '
    ' @param    Node The MainNode among whose value entries we will be searching.
    ' @param    key The key of the value we are looking for.
    ' @returns  The index in which the value with the key we are looking for is stored or the negation of the index in which it should be if not found.

    Private Function FindKey(ByRef Node As MainNode, ByVal Key As String) As Integer
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero
        'Last Modify Date: 5/01/2006
        'Returns the index of the value which key matches the requested one,
        'or the negation of the position were it should be if not found
        '**************************************************************
        Dim min As Integer
        Dim max As Integer
        'UPGRADE_NOTE: mid se actualizó a mid_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim mid_Renamed As Integer

        min = 0
        max = Node.numValues - 1

        Do While min <= max
            mid_Renamed = (min + max)\2

            If Node.values(mid_Renamed).Key < Key Then
                min = mid_Renamed + 1
            ElseIf Node.values(mid_Renamed).Key > Key Then
                max = mid_Renamed - 1
            Else
                'We found it
                FindKey = mid_Renamed
                Exit Function
            End If
        Loop

        'Not found, return the negation of the position where it should be
        '(all higher values are to the right of the list and lower values are to the left)
        FindKey = Not mid_Renamed
    End Function

    ''
    'Searches for a main section with the given name within the loaded INI file and returns the index in which it's stored or the negation of the index in which it should be if not found.
    '
    ' @param    name The name of the MainNode we are looking for.
    ' @returns  The index in which the main section we are looking for is stored or the negation of the index in which it should be if not found.

    Private Function FindMain(ByVal name As String) As Integer
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero
        'Last Modify Date: 5/01/2006
        'Returns the index of the MainNode which name matches the requested one,
        'or the negation of the position were it should be if not found
        '**************************************************************
        Dim min As Integer
        Dim max As Integer
        'UPGRADE_NOTE: mid se actualizó a mid_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim mid_Renamed As Integer

        min = 0
        max = MainNodes - 1

        Do While min <= max
            mid_Renamed = (min + max)\2

            If fileData(mid_Renamed).name < name Then
                min = mid_Renamed + 1
            ElseIf fileData(mid_Renamed).name > name Then
                max = mid_Renamed - 1
            Else
                'We found it
                FindMain = mid_Renamed
                Exit Function
            End If
        Loop

        'Not found, return the negation of the position where it should be
        '(all higher values are to the right of the list and lower values are to the left)
        FindMain = Not mid_Renamed
    End Function

    ''
    'Checks wether a given key exists or not.
    '
    ' @param    name    The name of the element whose existance is being checked.
    ' @returns  True if the key exists, false otherwise.

    Public Function KeyExists(ByVal name As String) As Boolean
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero
        'Last Modify Date: 04/01/2008
        'Returns true of the key exists, false otherwise.
        '**************************************************************
        KeyExists = FindMain(UCase(name)) >= 0
    End Function
End Class
