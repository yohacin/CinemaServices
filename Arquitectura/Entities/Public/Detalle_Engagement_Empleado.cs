using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("detalle_engagement_empleado", Schema = "public")]
    public class Detalle_Engagement_Empleado
    {
        [Key]
        public long id { get; set; }
        public long id_engagement_empleado { get; set; }
        public DateTime fecha_inicio { get; set; }
        public DateTime fecha_fin { get; set; }
        public double cantidad_horas { get; set; }
        public int estado { get; set; }

        [NotMapped]
        public long id_ref { get; set; }


    }
}
