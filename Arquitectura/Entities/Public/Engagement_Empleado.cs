using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("engagement_empleado", Schema = "public")]
    public class Engagement_Empleado
    {
        [Key]
        public long id { get; set; }
        public long id_empleado { get; set; }
        public long id_engagement { get; set; }
        public DateTime fecha_asignacion { get; set; }
        public int estado { get; set; }

        [NotMapped]
        public string nombre_empleado { get; set; }
        [NotMapped]
        public double cantidad_horas_habilitadas { get; set; }

    }
}
