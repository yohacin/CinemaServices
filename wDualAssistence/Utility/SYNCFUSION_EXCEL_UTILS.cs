using Entities.Helpers;
using Syncfusion.XlsIO;
using System.Collections.Generic;
using System;
using EPU = Entities.Public;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Entities.Public;

namespace wDualAssistence.Utility;

public static class SYNCFUSION_EXCEL_UTILS
{
    public static async Task<List<Detalle_Marcacion_Biometrico>> ObtenerMarcacionesAsync(IFormFile archivo, EPU.Formato_Marcacion_Biometrico formato, long idEmpresa)
    {
        using var stream = new MemoryStream();
        await archivo.CopyToAsync(stream);
        stream.Position = 0;

        using ExcelEngine excelEngine = new ExcelEngine();
        IApplication application = excelEngine.Excel;
        application.DefaultVersion = ExcelVersion.Excel2016;

        IWorkbook workbook = application.Workbooks.Open(stream);
        IWorksheet worksheet = workbook.Worksheets["ASISTENCIA"];

        if (worksheet == null) throw new Exception("No se encontró la hoja de nombre ASISTENCIA");

        var marcaciones = new List<Detalle_Marcacion_Biometrico>();

        // Obtener índices de las columnas
        int colCodigoBiometrico = ObtenerIndiceColumna(worksheet, formato.col_cod_biometrico);
        int[] colFechaHoraEntrada = ObtenerIndicesColumnas(worksheet, formato.col_fecha_hora_entrada);
        int[] colFechaHoraSalida = string.IsNullOrWhiteSpace(formato.col_fecha_hora_salida)
         ? new int[0]  // Si no hay columna para la salida, devolver un arreglo vacío
         : ObtenerIndicesColumnas(worksheet, formato.col_fecha_hora_salida);

        // Validar que todas las columnas necesarias existan
        if (colCodigoBiometrico == -1 || colFechaHoraEntrada.Length == 0)
        {
            throw new Exception("No se encontraron todas las columnas necesarias en el archivo Excel.");
        }

        // Recorrer filas de datos (asumiendo que los datos empiezan en la fila 2)
        for (int i = 2; i <= worksheet.UsedRange.LastRow; i++)
        {
            // Leer código biométrico
            string codigoBiometrico = worksheet[i, colCodigoBiometrico]?.DisplayText?.Trim();
            if (string.IsNullOrWhiteSpace(codigoBiometrico)) continue;

            // Leer marcación de entrada
            ProcesarColumnas(worksheet, i, codigoBiometrico, idEmpresa, colFechaHoraEntrada, marcaciones);

            // Leer marcación de salida (si están en la misma fila)
            if (formato.estan_misma_fila)
            {
                ProcesarColumnas(worksheet, i, codigoBiometrico, idEmpresa, colFechaHoraSalida, marcaciones);
            }
        }

        return marcaciones;
    }

    private static void ProcesarColumnas(IWorksheet worksheet, int fila, string codigoBiometrico, long idEmpresa, int[] columnas, List<Detalle_Marcacion_Biometrico> marcaciones)
    {
        // Solo procesamos cuando hay columnas con datos
        if (columnas.Length == 2)
        {
            // Si hay dos columnas (Fecha y Hora) para la entrada/salida
            string fecha = worksheet[fila, columnas[0]]?.DisplayText?.Trim();
            string hora = worksheet[fila, columnas[1]]?.DisplayText?.Trim();

            // Verificamos si ambos tienen datos
            if (!string.IsNullOrWhiteSpace(fecha) && !string.IsNullOrWhiteSpace(hora))
            {
                string fechaHoraCombinada = fecha + " " + hora;

                if (DateTime.TryParse(fechaHoraCombinada, out DateTime fechaMarcacion))
                {
                    marcaciones.Add(new Detalle_Marcacion_Biometrico
                    {
                        codigo_biometrico = codigoBiometrico,
                        fecha_marcacion = fechaMarcacion,
                        id_empresa = idEmpresa
                    });
                }
            }
        }
        else
        {
            // Si solo hay una columna (por ejemplo, si fecha y hora ya vienen en la misma columna)
            foreach (var col in columnas)
            {
                string valor = worksheet[fila, col]?.DisplayText?.Trim();
                if (!string.IsNullOrWhiteSpace(valor) && DateTime.TryParse(valor, out DateTime fechaMarcacion))
                {
                    marcaciones.Add(new Detalle_Marcacion_Biometrico
                    {
                        codigo_biometrico = codigoBiometrico,
                        fecha_marcacion = fechaMarcacion,
                        id_empresa = idEmpresa
                    });
                }
            }
        }
    }


    private static int ObtenerIndiceColumna(IWorksheet worksheet, string nombreColumna)
    {
        for (int i = 1; i <= worksheet.UsedRange.LastColumn; i++)
        {
            if (worksheet[1, i]?.DisplayText?.Equals(nombreColumna, StringComparison.OrdinalIgnoreCase) == true)
            {
                return i;
            }
        }
        return -1; // No encontrada
    }

    private static int[] ObtenerIndicesColumnas(IWorksheet worksheet, string nombresColumnas)
    {
        var nombres = nombresColumnas.Split('|');
        var indices = new List<int>();

        foreach (var nombre in nombres)
        {
            int indice = ObtenerIndiceColumna(worksheet, nombre.Trim());
            if (indice != -1)
            {
                indices.Add(indice);
            }
        }

        return indices.ToArray();
    }
}
