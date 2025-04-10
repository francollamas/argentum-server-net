Module ArrayInitializers
    Public Sub InitializeStruct(struct As ObjData())
        Dim i As Integer
        For i = 0 To struct.Length - 1
            struct(i).Initialize()
        Next
    End Sub

    Public Sub InitializeStruct(struct As User())
        Dim i As Integer
        For i = 0 To struct.Length - 1
            struct(i).Initialize()
        Next
    End Sub

    Public Sub InitializeStruct(struct As Declaraciones.tForo())
        Dim i As Integer
        For i = 0 To struct.Length - 1
            struct(i).Initialize()
        Next
    End Sub

    Public Sub InitializeStruct(struct As modForum.tForo())
        Dim i As Integer
        For i = 0 To struct.Length - 1
            struct(i).Initialize()
        Next
    End Sub

    Public Sub InitializeStruct(struct As npc())
        Dim i As Integer
        For i = 0 To struct.Length - 1
            struct(i).Initialize()
        Next
    End Sub

    Public Sub InitializeStruct(struct As tFaccionArmaduras(,))
        Dim i, j As Integer
        For i = 0 To struct.GetUpperBound(0)
            For j = 0 To struct.GetUpperBound(1)
                struct(i, j).Initialize()
            Next
        Next
    End Sub

    Public Sub InitializeStruct(struct As fragLvlRace())
        Dim i As Integer
        For i = 0 To struct.Length - 1
            struct(i).Initialize()
        Next
    End Sub

    Public Sub InitializeStruct(struct As fragLvlLvl())
        Dim i As Integer
        For i = 0 To struct.Length - 1
            struct(i).Initialize()
        Next
    End Sub

    Public Sub InitializeStruct(struct As MapBlock(,,))
        Dim i, j, k As Integer
        For i = 0 To struct.GetUpperBound(0)
            For j = 0 To struct.GetUpperBound(1)
                For k = 0 To struct.GetUpperBound(2)
                    struct(i, j, k).Initialize()
                Next
            Next
        Next
    End Sub

    Public Sub InitializeStruct(struct As HomeDistance())
        Dim i As Integer
        For i = 0 To struct.Length - 1
            struct(i).Initialize()
        Next
    End Sub
End Module
