using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("responsable_engagement", Schema = "public")]
    public class Responsable_Engagement
    {
        [Key]
        public long id { get; set; }
        public long id_usuario { get; set; }
        public string cargo { get; set; }
        public int estado { get; set; }
        public long id_engagement { get; set; }

        [NotMapped]
        public string nombre_usuario { get; set; }
    }
}
