using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("alerta_engagement", Schema = "public")]
    public class Alerta_Engagement
    {
        [Key]
        public long id { get; set; }
        public int porcentaje_notificador { get; set; }
        public long id_engagement { get; set; }
        public int estado { get; set; }
        public int ejecutado { get; set; }

    }
}
