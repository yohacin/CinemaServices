using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("tarea_engagement", Schema = "public")]
    public class Tarea_Engagement
    {
        [Key]
        public long id { get; set; }
        public long id_engagement { get; set; }
        public long id_tarea { get; set; }
        public string descripcion { get; set; }
        public double maximo_horas { get; set; }
        public int estado { get; set; }

        [NotMapped]
        public string categoria { get; set; }
    }
}
