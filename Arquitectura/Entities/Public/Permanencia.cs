using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("permanencia", Schema = "public")]
    public class Permanencia
    {
        [Key]
        public long id { get; set; }
        public long id_empleado { get; set; }
        public long codigo_turno { get; set; }
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime hora_marcacion { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public string tipo { get; set; }
        public long id_empresa { get; set; }

        [NotMapped]
        public string nombre { get; set; }
        [NotMapped]
        public string empresa { get; set; }
        [NotMapped]
        public Empleado empleado { get; set; }

    }
}
