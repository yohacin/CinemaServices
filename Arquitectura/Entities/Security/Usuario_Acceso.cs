using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Security
{
    [Table("usuario_acceso", Schema = "security")]
    public class Usuario_Acceso
    {
        [Key]
        public long id { get; set; }
        public long id_usuario { get; set; }
        public long id_acceso { get; set; }
    }
}
