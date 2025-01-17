using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

using ESE = Entities.Security;

namespace Entities.Public
{
    [Table("turno", Schema = "public")]
    public class Turno
    {
        [Key]
        public long id { get; set; }
        public long id_empleado { get; set; }
        public int codigo_turno { get; set; }
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime entrada { get; set; }
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime salida { get; set; }
        public string descripcion_turno { get; set; }
        public int anticipo_entrada { get; set; }
        public int tolerancia_entrada { get; set; }
        public int anticipo_salida { get; set; }
        public int tolerancia_salida { get; set; }
        public int? idc_dia { get; set; }

        [NotMapped]
        public ESE.Clasificador dia { get; set; }
    }
}
