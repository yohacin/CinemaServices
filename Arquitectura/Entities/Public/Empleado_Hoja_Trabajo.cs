using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("empleado_hoja_trabajo", Schema = "public")]
    public class Empleado_Hoja_Trabajo
    {
        [Key]
        public long id { get; set; }
        public DateTime fecha { get; set; }
        public long id_engagement { get; set; }
        public long id_tarea { get; set; }
        public long id_empleado { get; set; }
        public double hora { get; set; }
        public int estado { get; set; }

    }
}
