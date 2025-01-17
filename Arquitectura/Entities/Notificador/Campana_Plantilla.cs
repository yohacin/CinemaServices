using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Notificador
{
    [Table("campana_plantilla", Schema = "notificador")]
    public class Campana_Plantilla
    {
        [Key]
        public long id { get; set; }
        public long codigo_campana { get; set; }
        public long id_plantilla { get; set; }
    }
}
