using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("detalle_hora_engagement", Schema = "public")]
    public class Detalle_Hora_Engagement
    {
        [Key]
        public long id { get; set; }
        public long id_engagement { get; set; }
        public int tipo { get; set; }
        public double cantidad { get; set; }
        public string descripcion { get; set; }
        public int estado { get; set; }

        [NotMapped]
        public string tipo_descripcion { get; set; }
    }
}
