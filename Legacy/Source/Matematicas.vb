Option Strict Off
Option Explicit On
Module Matematicas
    Public Function Porcentaje(Total As Integer, Porc As Integer) As Integer
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        Porcentaje = (Total*Porc)/100
    End Function

    Function Distancia(ByRef wp1 As WorldPos, ByRef wp2 As WorldPos) As Integer
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'Encuentra la distancia entre dos WorldPos
        Distancia = Math.Abs(wp1.X - wp2.X) + Math.Abs(wp1.Y - wp2.Y) +
                    (Math.Abs(wp1.map - wp2.map)*100)
    End Function

    Function Distance(X1 As Short, Y1 As Short, X2 As Short, Y2 As Short) As Double
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'Encuentra la distancia entre dos puntos

        Distance = Math.Sqrt((Y1 - Y2)^2 + (X1 - X2)^2)
    End Function

    Public Function RandomNumber(LowerBound As Integer, UpperBound As Integer) As Integer
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero
        'Last Modify Date: 3/06/2006
        'Generates a random number in the range given - recoded to use longs and work properly with ranges
        '**************************************************************
        RandomNumber = Fix(Rnd()*(UpperBound - LowerBound + 1)) + LowerBound
    End Function
End Module
