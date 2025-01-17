using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Security
{
    [Table("perfil", Schema = "security")]
    public class Perfil
    {
        [Key]
        public long id { get; set; }
        public string nombre { get; set; }
        public bool baja { get; set; }
        public bool estado { get; set; }
        public bool root { get; set; }
        public string descripcion { get; set; }

        [NotMapped]
        public List<Entities.Security.Perfil_Acceso> listaPerfil_Acceso { get; set; }

    }
}
