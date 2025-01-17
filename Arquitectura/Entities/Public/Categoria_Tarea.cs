using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("categoria_tarea", Schema = "public")]
    public class Categoria_Tarea
    {
        [Key]
        public long id { get; set; }
        public string nombre { get; set; }
        public int estado { get; set; }
    }
}
