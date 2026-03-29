using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;

namespace Legacy
{
    internal class cCola
    {
        // Metodos publicos
        // 
        // Public sub Push(byval i as variant) mete el elemento i
        // al final de la cola.
        // 
        // Public Function Pop As Variant: quita de la cola el primer elem
        // y lo devuelve
        // 
        // Public Function VerElemento(ByVal Index As Integer) As Variant
        // muestra el elemento numero Index de la cola sin quitarlo
        // 
        // Public Function PopByVal() As Variant: muestra el primer
        // elemento de la cola sin quitarlo
        // 
        // Public Property Get Longitud() As Integer: devuelve la
        // cantidad de elementos que tiene la cola.

        private const short FRENTE = 0;

        private List<string> Cola;

        // UPGRADE_NOTE: Reset se actualizó a Reset_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        public void Reset_Renamed()
        {
            try
            {

                short i;
                var loopTo = Convert.ToInt16(Longitud - 1);
                for (i = 0; i <= loopTo; i++)
                    Cola.RemoveAt(FRENTE);
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in Push: " + ex.Message);
            }
        }

        public short Longitud
        {
            get
            {
                short LongitudRet = default;
                LongitudRet = Convert.ToInt16(Cola.Count);
                return LongitudRet;
            }
        }

        private bool IndexValido(short i)
        {
            bool IndexValidoRet = default;
            IndexValidoRet = i >= 0 & i < Longitud;
            return IndexValidoRet;
        }

        // UPGRADE_NOTE: Class_Initialize se actualizó a Class_Initialize_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        private void Class_Initialize_Renamed()
        {
            Cola = new List<string>();
        }

        public cCola() : base()
        {
            Class_Initialize_Renamed();
        }

        public string VerElemento(short index)
        {
            string VerElementoRet = default;
            try
            {
                if (IndexValido(index))
                {
                    // Pablo
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Cola.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    VerElementoRet = Cola[index].ToString().ToUpper();
                }
                // /Pablo
                // VerElemento = Cola(Index)
                else
                {
                    VerElementoRet = "0";
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in IndexValido: " + ex.Message);
            }

            return VerElementoRet;
        }


        public void Push(string Nombre)
        {
            try
            {
                // Mete elemento en la cola
                // Pablo
                string aux;
                aux = DateAndTime.TimeString + " " + Nombre.ToUpper();
                Cola.Add(aux);
            }
            // /Pablo

            // Call Cola.Add(UCase$(Nombre))

            catch (Exception ex)
            {
                Console.WriteLine("Error in Push: " + ex.Message);
            }
        }

        public string Pop()
        {
            string PopRet = default;
            try
            {
                // Quita elemento de la cola
                if (Cola.Count > 0)
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Cola(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    PopRet = Cola[FRENTE];
                    Cola.RemoveAt(FRENTE);
                }
                else
                {
                    PopRet = "0";
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in Pop: " + ex.Message);
            }

            return PopRet;
        }

        public string PopByVal()
        {
            string PopByValRet = default;
            try
            {
                // Call LogTarea("PopByVal SOS")

                // Quita elemento de la cola
                if (Cola.Count > 0)
                {
                    // UPGRADE_WARNING: No se puede resolver la propiedad predeterminada del objeto Cola.Item(). Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    PopByValRet = Cola[1];
                }
                else
                {
                    PopByValRet = "0";
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in PopByVal: " + ex.Message);
            }

            return PopByValRet;
        }

        public bool Existe(string Nombre)
        {
            bool ExisteRet = default;
            try
            {

                string V;
                short i;
                string NombreEnMayusculas;
                NombreEnMayusculas = Nombre.ToUpper();

                var loopTo = Convert.ToInt16(Longitud - 1);
                for (i = 0; i <= loopTo; i++)
                {
                    // Pablo
                    V = VerElemento(i).Substring(9);
                    // /Pablo
                    // V = Me.VerElemento(i)
                    if ((V ?? "") == (NombreEnMayusculas ?? ""))
                    {
                        ExisteRet = true;
                        return ExisteRet;
                    }
                }
                ExisteRet = false;
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in Existe: " + ex.Message);
            }

            return ExisteRet;
        }

        public void Quitar(string Nombre)
        {
            try
            {
                string V;
                short i;
                string NombreEnMayusculas;

                NombreEnMayusculas = Nombre.ToUpper();

                var loopTo = Convert.ToInt16(Longitud - 1);
                for (i = 0; i <= loopTo; i++)
                {
                    // Pablo
                    V = VerElemento(i).Substring(9);
                    // /Pablo
                    // V = Me.VerElemento(i)
                    if ((V ?? "") == (NombreEnMayusculas ?? ""))
                    {
                        Cola.RemoveAt(i);
                        return;
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in Quitar: " + ex.Message);
            }
        }

        public void QuitarIndex(short index)
        {
            try
            {
                if (IndexValido(index))
                    Cola.RemoveAt(index);
            }

            catch (Exception ex)
            {
                Console.WriteLine("Error in QuitarIndex: " + ex.Message);
            }
        }


        // UPGRADE_NOTE: Class_Terminate se actualizó a Class_Terminate_Renamed. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        private void Class_Terminate_Renamed()
        {
            // Destruimos el objeto Cola
            // UPGRADE_NOTE: El objeto Cola no se puede destruir hasta que no se realice la recolección de los elementos no utilizados. Haga clic aquí para obtener más información: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"'
            Cola = null;
        }

        ~cCola()
        {
            Class_Terminate_Renamed();
        }
    }
}