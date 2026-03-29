using System;

namespace Legacy
{
    static class Queue
    {
        public struct tVertice
        {
            public short X;
            public short Y;
        }

        private const short MAXELEM = 1000;

        private static tVertice[] m_array;
        private static short m_lastelem;
        private static short m_firstelem;
        private static short m_size;

        public static bool IsEmpty()
        {
            bool IsEmptyRet = default;
            IsEmptyRet = m_size == 0;
            return IsEmptyRet;
        }

        public static bool IsFull()
        {
            bool IsFullRet = default;
            IsFullRet = m_lastelem == MAXELEM;
            return IsFullRet;
        }

        public static bool Push(ref tVertice Vertice)
        {
            bool PushRet = default;

            if (!IsFull())
            {

                if (IsEmpty())
                    m_firstelem = 1;

                m_lastelem = Convert.ToInt16(m_lastelem + 1);
                m_size = Convert.ToInt16(m_size + 1);
                // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto m_array(m_lastelem). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                m_array[m_lastelem] = Vertice;

                PushRet = true;
            }
            else
            {
                PushRet = false;
            }

            return PushRet;
        }

        public static tVertice Pop()
        {
            tVertice PopRet = default;

            if (!IsEmpty())
            {

                PopRet = m_array[m_firstelem];
                m_firstelem = Convert.ToInt16(m_firstelem + 1);
                m_size = Convert.ToInt16(m_size - 1);

                if (m_firstelem > m_lastelem & m_size == 0)
                {
                    m_lastelem = 0;
                    m_firstelem = 0;
                    m_size = 0;
                }

            }

            return PopRet;
        }

        public static void InitQueue()
        {
            m_array = new tVertice[(MAXELEM + 1)];
            m_lastelem = 0;
            m_firstelem = 0;
            m_size = 0;
        }
    }
}