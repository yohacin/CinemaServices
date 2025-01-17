using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Public
{
    [Table("alerta_marcacion_erronea", Schema = "public")]
    public class AlertaMarcacionErronea
    {
        [Key]
        public long id { get; set; }

        public string url_notificador { get; set; }
        public long id_campana { get; set; }
        public bool activo { get; set; }
    }
}