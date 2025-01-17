using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Notificador
{
    [Table("campana_detalle", Schema = "notificador")]
    public class Campana_Detalle
    {
        [Key]
        public long codigo_campana { get; set; }
        public long marca_eliminado { get; set; }
        [Key]
        public long codigo_grupo { get; set; }
        [Key]
        public long codigo_asignado { get; set; }
    }
}
