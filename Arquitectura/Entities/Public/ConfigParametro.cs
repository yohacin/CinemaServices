using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Public
{
    [Table("config_parametro", Schema = "public")]
    public class ConfigParametro
    {
        [Key]
        public long id { get; set; }

        public string nombre { get; set; }
        public string valor { get; set; }
        public string descripcion { get; set; }
    }
}