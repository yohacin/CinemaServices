using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Notificador
{
    [Table("mensaje", Schema = "notificador")]
    public class Mensaje
    {
        [Key]
        public long codigo_mensaje { get; set; }
        public String descripcion { get; set; }
        public DateTime fecha { get; set; }
        public String tipo { get; set; }
        public String dato { get; set; }
        public long marca_eliminado { get; set; }
        public long codigo_campana { get; set; }
        public long codigo_grupo { get; set; }
        public long codigo_contacto { get; set; }

    }
}
