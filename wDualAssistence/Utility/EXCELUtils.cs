using System;

namespace wDualAssistence.Utility
{
    public class EXCELUtils
    {
        public EXCELUtils()
        {
        }

        public string obtenerNombreColumna(int numeroDeColumna)
        {
            string nombreColumna = String.Empty;

            while (numeroDeColumna > 0)
            {
                int remainder = (numeroDeColumna - 1) % 26;
                nombreColumna = (char)(remainder + 'A') + nombreColumna;
                numeroDeColumna = (numeroDeColumna - 1) / 26;
            }

            return nombreColumna;
        }
    }
}