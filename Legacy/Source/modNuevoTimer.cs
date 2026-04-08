using System;

namespace Legacy;

internal static class modNuevoTimer
{
    // 
    // Las siguientes funciones devuelven TRUE o FALSE si el intervalo
    // permite hacerlo. Si devuelve TRUE, setean automaticamente el
    // timer para que no se pueda hacer la accion hasta el nuevo ciclo.
    // 

    // CASTING DE HECHIZOS
    public static bool IntervaloPermiteLanzarSpell(short UserIndex, bool Actualizar = true)
    {
        bool IntervaloPermiteLanzarSpellRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int TActual;

        TActual = Convert.ToInt32(Migration.GetTickCount());

        if (TActual - Declaraciones.UserList[UserIndex].Counters.TimerLanzarSpell >= Admin.IntervaloUserPuedeCastear)
        {
            if (Actualizar) Declaraciones.UserList[UserIndex].Counters.TimerLanzarSpell = TActual;
            IntervaloPermiteLanzarSpellRet = true;
        }
        else
        {
            IntervaloPermiteLanzarSpellRet = false;
        }

        return IntervaloPermiteLanzarSpellRet;
    }

    public static bool IntervaloPermiteAtacar(short UserIndex, bool Actualizar = true)
    {
        bool IntervaloPermiteAtacarRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int TActual;

        TActual = Convert.ToInt32(Migration.GetTickCount());

        if (TActual - Declaraciones.UserList[UserIndex].Counters.TimerPuedeAtacar >= Admin.IntervaloUserPuedeAtacar)
        {
            if (Actualizar)
            {
                Declaraciones.UserList[UserIndex].Counters.TimerPuedeAtacar = TActual;
                Declaraciones.UserList[UserIndex].Counters.TimerGolpeUsar = TActual;
            }

            IntervaloPermiteAtacarRet = true;
        }
        else
        {
            IntervaloPermiteAtacarRet = false;
        }

        return IntervaloPermiteAtacarRet;
    }

    public static bool IntervaloPermiteGolpeUsar(short UserIndex, bool Actualizar = true)
    {
        bool IntervaloPermiteGolpeUsarRet = default;
        // ***************************************************
        // Author: ZaMa
        // Checks if the time that passed from the last hit is enough for the user to use a potion.
        // Last Modification: 06/04/2009
        // ***************************************************

        int TActual;

        TActual = Convert.ToInt32(Migration.GetTickCount());

        if (TActual - Declaraciones.UserList[UserIndex].Counters.TimerGolpeUsar >= Admin.IntervaloGolpeUsar)
        {
            if (Actualizar) Declaraciones.UserList[UserIndex].Counters.TimerGolpeUsar = TActual;
            IntervaloPermiteGolpeUsarRet = true;
        }
        else
        {
            IntervaloPermiteGolpeUsarRet = false;
        }

        return IntervaloPermiteGolpeUsarRet;
    }

    public static bool IntervaloPermiteMagiaGolpe(short UserIndex, bool Actualizar = true)
    {
        bool IntervaloPermiteMagiaGolpeRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************
        int TActual;

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            if (withBlock.Counters.TimerMagiaGolpe > withBlock.Counters.TimerLanzarSpell)
                return IntervaloPermiteMagiaGolpeRet;

            TActual = Convert.ToInt32(Migration.GetTickCount());

            if (TActual - withBlock.Counters.TimerLanzarSpell >= Admin.IntervaloMagiaGolpe)
            {
                if (Actualizar)
                {
                    withBlock.Counters.TimerMagiaGolpe = TActual;
                    withBlock.Counters.TimerPuedeAtacar = TActual;
                    withBlock.Counters.TimerGolpeUsar = TActual;
                }

                IntervaloPermiteMagiaGolpeRet = true;
            }
            else
            {
                IntervaloPermiteMagiaGolpeRet = false;
            }
        }

        return IntervaloPermiteMagiaGolpeRet;
    }

    public static bool IntervaloPermiteGolpeMagia(short UserIndex, bool Actualizar = true)
    {
        bool IntervaloPermiteGolpeMagiaRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int TActual;

        if (Declaraciones.UserList[UserIndex].Counters.TimerGolpeMagia >
            Declaraciones.UserList[UserIndex].Counters.TimerPuedeAtacar) return IntervaloPermiteGolpeMagiaRet;

        TActual = Convert.ToInt32(Migration.GetTickCount());

        if (TActual - Declaraciones.UserList[UserIndex].Counters.TimerPuedeAtacar >= Admin.IntervaloGolpeMagia)
        {
            if (Actualizar)
            {
                Declaraciones.UserList[UserIndex].Counters.TimerGolpeMagia = TActual;
                Declaraciones.UserList[UserIndex].Counters.TimerLanzarSpell = TActual;
            }

            IntervaloPermiteGolpeMagiaRet = true;
        }
        else
        {
            IntervaloPermiteGolpeMagiaRet = false;
        }

        return IntervaloPermiteGolpeMagiaRet;
    }

    // ATAQUE CUERPO A CUERPO
    // Public Function IntervaloPermiteAtacar(ByVal UserIndex As Integer, Optional ByVal Actualizar As Boolean = True) As Boolean
    // Dim TActual As Integer
    // 
    // TActual = Convert.ToInt32(GetTickCount())''
    // 
    // If TActual - UserList(UserIndex).Counters.TimerPuedeAtacar >= IntervaloUserPuedeAtacar Then
    // If Actualizar Then UserList(UserIndex).Counters.TimerPuedeAtacar = TActual
    // IntervaloPermiteAtacar = True
    // Else
    // IntervaloPermiteAtacar = False
    // End If
    // End Function

    // TRABAJO
    public static bool IntervaloPermiteTrabajar(short UserIndex, bool Actualizar = true)
    {
        bool IntervaloPermiteTrabajarRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int TActual;

        TActual = Convert.ToInt32(Migration.GetTickCount());

        if (TActual - Declaraciones.UserList[UserIndex].Counters.TimerPuedeTrabajar >= Admin.IntervaloUserPuedeTrabajar)
        {
            if (Actualizar)
                Declaraciones.UserList[UserIndex].Counters.TimerPuedeTrabajar = TActual;
            IntervaloPermiteTrabajarRet = true;
        }
        else
        {
            IntervaloPermiteTrabajarRet = false;
        }

        return IntervaloPermiteTrabajarRet;
    }

    // USAR OBJETOS
    public static bool IntervaloPermiteUsar(short UserIndex, bool Actualizar = true)
    {
        bool IntervaloPermiteUsarRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: 25/01/2010 (ZaMa)
        // 25/01/2010: ZaMa - General adjustments.
        // ***************************************************

        int TActual;

        TActual = Convert.ToInt32(Migration.GetTickCount());

        if (TActual - Declaraciones.UserList[UserIndex].Counters.TimerUsar >= Admin.IntervaloUserPuedeUsar)
        {
            if (Actualizar) Declaraciones.UserList[UserIndex].Counters.TimerUsar = TActual;
            // UserList(UserIndex).Counters.failedUsageAttempts = 0
            IntervaloPermiteUsarRet = true;
        }
        else
        {
            IntervaloPermiteUsarRet = false;

            // UserList(UserIndex).Counters.failedUsageAttempts = UserList(UserIndex).Counters.failedUsageAttempts + 1

            // Tolerancia arbitraria - 20 es MUY alta, la está chiteando zarpado
            // If UserList(UserIndex).Counters.failedUsageAttempts = 20 Then
            // Call SendData(SendTarget.ToAdmins, 0, PrepareMessageConsoleMsg(UserList(UserIndex).name & " kicked by the server por posible modificación de intervalos.", FontTypeNames.FONTTYPE_FIGHT))
            // Call CloseSocket(UserIndex)
            // End If
        }

        return IntervaloPermiteUsarRet;
    }

    public static bool IntervaloPermiteUsarArcos(short UserIndex, bool Actualizar = true)
    {
        bool IntervaloPermiteUsarArcosRet = default;
        // ***************************************************
        // Author: Unknown
        // Last Modification: -
        // 
        // ***************************************************

        int TActual;

        TActual = Convert.ToInt32(Migration.GetTickCount());

        if (TActual - Declaraciones.UserList[UserIndex].Counters.TimerPuedeUsarArco >= Admin.IntervaloFlechasCazadores)
        {
            if (Actualizar)
                Declaraciones.UserList[UserIndex].Counters.TimerPuedeUsarArco = TActual;
            IntervaloPermiteUsarArcosRet = true;
        }
        else
        {
            IntervaloPermiteUsarArcosRet = false;
        }

        return IntervaloPermiteUsarArcosRet;
    }

    public static bool IntervaloPermiteSerAtacado(short UserIndex, bool Actualizar = false)
    {
        bool IntervaloPermiteSerAtacadoRet = default;
        // **************************************************************
        // Author: ZaMa
        // Last Modify by: ZaMa
        // Last Modify Date: 13/11/2009
        // 13/11/2009: ZaMa - Add the Timer which determines wether the user can be atacked by a NPc or not
        // **************************************************************
        int TActual;

        TActual = Convert.ToInt32(Migration.GetTickCount());

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Inicializa el timer
            if (Actualizar)
            {
                withBlock.Counters.TimerPuedeSerAtacado = TActual;
                withBlock.flags.NoPuedeSerAtacado = true;
                IntervaloPermiteSerAtacadoRet = false;
            }
            else if (TActual - withBlock.Counters.TimerPuedeSerAtacado >= Admin.IntervaloPuedeSerAtacado)
            {
                withBlock.flags.NoPuedeSerAtacado = false;
                IntervaloPermiteSerAtacadoRet = true;
            }
            else
            {
                IntervaloPermiteSerAtacadoRet = false;
            }
        }

        return IntervaloPermiteSerAtacadoRet;
    }

    public static bool IntervaloPerdioNpc(short UserIndex, bool Actualizar = false)
    {
        bool IntervaloPerdioNpcRet = default;
        // **************************************************************
        // Author: ZaMa
        // Last Modify by: ZaMa
        // Last Modify Date: 13/11/2009
        // 13/11/2009: ZaMa - Add the Timer which determines wether the user still owns a Npc or not
        // **************************************************************
        int TActual;

        TActual = Convert.ToInt32(Migration.GetTickCount());

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Inicializa el timer
            if (Actualizar)
            {
                withBlock.Counters.TimerPerteneceNpc = TActual;
                IntervaloPerdioNpcRet = false;
            }
            else if (TActual - withBlock.Counters.TimerPerteneceNpc >= Admin.IntervaloOwnedNpc)
            {
                IntervaloPerdioNpcRet = true;
            }
            else
            {
                IntervaloPerdioNpcRet = false;
            }
        }

        return IntervaloPerdioNpcRet;
    }

    public static bool IntervaloEstadoAtacable(short UserIndex, bool Actualizar = false)
    {
        bool IntervaloEstadoAtacableRet = default;
        // **************************************************************
        // Author: ZaMa
        // Last Modify by: ZaMa
        // Last Modify Date: 13/01/2010
        // 13/01/2010: ZaMa - Add the Timer which determines wether the user can be atacked by an user or not
        // **************************************************************
        int TActual;

        TActual = Convert.ToInt32(Migration.GetTickCount());

        {
            ref var withBlock = ref Declaraciones.UserList[UserIndex];
            // Inicializa el timer
            if (Actualizar)
            {
                withBlock.Counters.TimerEstadoAtacable = TActual;
                IntervaloEstadoAtacableRet = true;
            }
            else if (TActual - withBlock.Counters.TimerEstadoAtacable >= Admin.IntervaloAtacable)
            {
                IntervaloEstadoAtacableRet = false;
            }
            else
            {
                IntervaloEstadoAtacableRet = true;
            }
        }

        return IntervaloEstadoAtacableRet;
    }
}