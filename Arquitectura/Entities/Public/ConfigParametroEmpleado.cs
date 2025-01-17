using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Public
{
    [Table("config_parametro_empleado", Schema = "public")]
    public class ConfigParametroEmpleado
    {
        [Key]
        public long id { get; set; }

        public long id_config_parametro { get; set; }
        public long id_empleado { get; set; }
        public string valor { get; set; }
    }
}