using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("excepcion", Schema = "public")]
    public class Excepcion
    {
        [Key]
        public long id { get; set; }
        public string ci { get; set; }
        public long cod_origen { get; set; }
        public double horas { get; set; }
        public string tipo { get; set; }
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime fecha_reg { get; set; }
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime fecha_inicio { get; set; }
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime fecha_fin { get; set; }
        public long id_empleado { get; set; }
        public int estado { get; set; }
    }
}
