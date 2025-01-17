using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Notificador
{
    [Table("query_", Schema = "notificador")]
    public class Query_Campana
    {
        [Key]
        public Int64 id { get; set; }
        public String nombre { get; set; }
        public String sql_query { get; set; }
        public Int32 marca_eliminado { get; set; }
        public long id_empresa { get; set; }
    }
}
