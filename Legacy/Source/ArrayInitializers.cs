namespace Legacy;

internal static class ArrayInitializers
{
    public static void InitializeStruct(Declaraciones.ObjData[] @struct)
    {
        int i;
        var loopTo = @struct.Length - 1;
        for (i = 0; i <= loopTo; i++)
            @struct[i].Initialize();
    }

    public static void InitializeStruct(Declaraciones.User[] @struct)
    {
        int i;
        var loopTo = @struct.Length - 1;
        for (i = 0; i <= loopTo; i++)
            @struct[i].Initialize();
    }

    public static void InitializeStruct(Declaraciones.tForo[] @struct)
    {
        int i;
        var loopTo = @struct.Length - 1;
        for (i = 0; i <= loopTo; i++)
            @struct[i].Initialize();
    }

    public static void InitializeStruct(modForum.tForo[] @struct)
    {
        int i;
        var loopTo = @struct.Length - 1;
        for (i = 0; i <= loopTo; i++)
            @struct[i].Initialize();
    }

    public static void InitializeStruct(Declaraciones.npc[] @struct)
    {
        int i;
        var loopTo = @struct.Length - 1;
        for (i = 0; i <= loopTo; i++)
            @struct[i].Initialize();
    }

    public static void InitializeStruct(ModFacciones.tFaccionArmaduras[,] @struct)
    {
        int i, j;
        var loopTo = @struct.GetUpperBound(0);
        for (i = 0; i <= loopTo; i++)
        {
            var loopTo1 = @struct.GetUpperBound(1);
            for (j = 0; j <= loopTo1; j++)
                @struct[i, j].Initialize();
        }
    }

    public static void InitializeStruct(Statistics.fragLvlRace[] @struct)
    {
        int i;
        var loopTo = @struct.Length - 1;
        for (i = 0; i <= loopTo; i++)
            @struct[i].Initialize();
    }

    public static void InitializeStruct(Statistics.fragLvlLvl[] @struct)
    {
        int i;
        var loopTo = @struct.Length - 1;
        for (i = 0; i <= loopTo; i++)
            @struct[i].Initialize();
    }

    public static void InitializeStruct(Declaraciones.MapBlock[,,] @struct)
    {
        int i, j, k;
        var loopTo = @struct.GetUpperBound(0);
        for (i = 0; i <= loopTo; i++)
        {
            var loopTo1 = @struct.GetUpperBound(1);
            for (j = 0; j <= loopTo1; j++)
            {
                var loopTo2 = @struct.GetUpperBound(2);
                for (k = 0; k <= loopTo2; k++)
                    @struct[i, j, k].Initialize();
            }
        }
    }

    public static void InitializeStruct(Declaraciones.HomeDistance[] @struct)
    {
        int i;
        var loopTo = @struct.Length - 1;
        for (i = 0; i <= loopTo; i++)
            @struct[i].Initialize();
    }
}