using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Security
{
    [Table("clasificador", Schema = "security")]
    public class Clasificador
    {
        [Key]
        public long id { get; set; }
        public long id_tipo_clasificador { get; set; }
        public string descripcion { get; set; }
        public string valor { get; set; }
        public bool @base { get; set; }
        public bool estado { get; set; }
        public string valor2 { get; set; }
    }
}
