using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Security
{
    [Table("perfil_acceso", Schema = "security")]
    public class Perfil_Acceso
    {
        [Key]
        public long id { get; set; }
        public long id_acceso { get; set; }
        public long id_perfil { get; set; }
    }
}
