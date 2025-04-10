Option Strict Off
Option Explicit On
Module Matematicas
    Public Function Porcentaje(ByVal Total As Integer, ByVal Porc As Integer) As Integer
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
        Distancia = System.Math.Abs(wp1.X - wp2.X) + System.Math.Abs(wp1.Y - wp2.Y) +
                    (System.Math.Abs(wp1.map - wp2.map)*100)
    End Function

    Function Distance(ByVal X1 As Short, ByVal Y1 As Short, ByVal X2 As Short, ByVal Y2 As Short) As Double
        '***************************************************
        'Author: Unknown
        'Last Modification: -
        '
        '***************************************************

        'Encuentra la distancia entre dos puntos

        Distance = System.Math.Sqrt((Y1 - Y2)^2 + (X1 - X2)^2)
    End Function

    Public Function RandomNumber(ByVal LowerBound As Integer, ByVal UpperBound As Integer) As Integer
        '**************************************************************
        'Author: Juan Martín Sotuyo Dodero
        'Last Modify Date: 3/06/2006
        'Generates a random number in the range given - recoded to use longs and work properly with ranges
        '**************************************************************
        RandomNumber = Fix(Rnd()*(UpperBound - LowerBound + 1)) + LowerBound
    End Function
End Module
