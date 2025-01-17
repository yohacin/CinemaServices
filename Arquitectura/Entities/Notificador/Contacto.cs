using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Entities.Notificador
{
    [Table("contacto", Schema = "notificador")]
    public class Contacto
    {
        [Key]
        public long codigo_grupo { get; set; }
        [Key]
        public long codigo_contacto { get; set; }
        public String nombre { get; set; }
        public String correo { get; set; }
        public String telefono { get; set; }
        public String tipo { get; set; }
        public long marca_eliminado { get; set; }
    }
}
