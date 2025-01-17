using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("foto_marcacion", Schema = "public")]
    public class Foto_Marcacion
    {
        [Key]
        public long id { get; set; }
        public long id_marcacion { get; set; }
        public string url_foto { get; set; }
        public int posicion { get; set; }
        public DateTime fecha_hora { get; set; }

    }
}
