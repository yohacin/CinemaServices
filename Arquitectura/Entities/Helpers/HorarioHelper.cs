using Entities.Public;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.Helpers
{
    public class HorarioHelper
    {
        [Key]
        public int codigo_horario { get; set; } // Codigo unico
        public int codigo_jornada { get; set; }
        public int codigo_tipo { get; set; }
        public int codigo_dia { get; set; } // Clasificador Día
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime hora_entrada { get; set; } // Entrada
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime hora_inicio_entrada { get; set; } // Anticipo de entrada
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime hora_fin_entrada { get; set; } // Tolerancia de entrada
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime hora_salida { get; set; } // Salida
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime hora_inicio_salida { get; set; } // Anticipo de salida
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime hora_fin_salida { get; set; } // Tolerancia de salida
        public int tolerancia { get; set; }
    }
}
