using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Entities.Public;

[Table("detalle_jornada", Schema = "public")]
public class Detalle_Jornada
{
    [Key]
    public long id { get; set; }
    public long id_jornada { get; set; }
    public int dia { get; set; }
    //public TipoHorario tipo_horario { get; set; }

    [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
    public DateTime inicio_marcacion_entrada { get; set; }

    [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
    public DateTime hora_entrada { get; set; }

    [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
    public DateTime fin_marcacion_entrada { get; set; }


    [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
    public DateTime inicio_marcacion_salida { get; set; }

    [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
    public DateTime hora_salida { get; set; }

    [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
    public DateTime fin_marcacion_salida { get; set; }


    [NotMapped]
    public string descripcion_turno
    {
        get
        {
            //string horario = tipo_horario.GetNombre();
            string horaEntrada = hora_entrada.ToString("HH:mm");
            string horaSalida = hora_salida.ToString("HH:mm");

            return $"{horaEntrada} - {horaSalida}";
        }
    }
}

public enum TipoHorario
{
    Continuo = 1,
    Manana = 2,
    Tarde = 3,
}

public static class TipoHorarioExtension
{
    public static string GetNombre(this TipoHorario value)
    {
        switch (value)
        {
            case TipoHorario.Continuo:
                return "HORARIO CONTINUO";
            case TipoHorario.Manana:
                return "HORARIO MAÑANA";
            case TipoHorario.Tarde:
                return "HORARIO TARDE";
            default:
                return null;
        }
    }
}