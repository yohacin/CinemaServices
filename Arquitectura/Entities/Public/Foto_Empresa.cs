using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("foto_empresa", Schema = "public")]
    public class Foto_Empresa
    {
        [Key]
        public long id { get; set; }
        public long id_empresa { get; set; }
        public string url_foto { get; set; }
        public int posicion { get; set; }

    }
}
