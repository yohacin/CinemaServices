using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Notificador
{
    [Table("query", Schema = "notificador")]
    public class Query
    {
        [Key]
        public long id { get; set; }
        public long id_plantilla { get; set; }
        public string nombre { get; set; }
        public string contenido { get; set; }
        public long marca_eliminado { get; set; }
        public long id_query { get; set; }

        [NotMapped]
        public List<Object> queryResults { get; set; }
    }
}
