using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("marcacion", Schema = "public")]
    public class Marcacion
    {
        [Key]
        public long id { get; set; }

        public long id_empleado { get; set; }
        public long codigo_turno { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime entrada_programada { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime salida_programada { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime? entrada_real { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime? salida_real { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime? entrada_movil { get; set; }

        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime? salida_movil { get; set; }

        public double latitud_entrada { get; set; }
        public double longitud_entrada { get; set; }
        public double latitud_salida { get; set; }
        public double longitud_salida { get; set; }
        public long? id_empresa_entrada { get; set; }
        public long? id_empresa_salida { get; set; }
        public string tipo_marcacion_entrada { get; set; }
        public string tipo_marcacion_salida { get; set; }

        [NotMapped]
        public Empleado empleado { get; set; }

        [NotMapped]
        public DateTime fecha_marcacion { get; set; }

        [NotMapped]
        public long id_sucursal { get; set; }

        [NotMapped]
        public string nombre_sucursal { get; set; }
    }
}