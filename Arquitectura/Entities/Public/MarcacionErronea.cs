using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("marcacion_erronea", Schema = "public")]
    public class MarcacionErronea
    {
        [Key]
        public long id { get; set; }
        public long id_empleado { get; set; }
        public long codigo_turno { get; set; }
        public DateTime hora_marcacion { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public string tipo { get; set; }
    }
}
