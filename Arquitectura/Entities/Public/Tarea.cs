using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Public
{
    [Table("tarea", Schema = "public")]
    public class Tarea
    {
        [Key]
        public long id { get; set; }

        public string nombre { get; set; }
        public long id_categoria { get; set; }
        public int estado { get; set; }

        [AutoExpand]
        public string categoria { get; set; }
    }
}