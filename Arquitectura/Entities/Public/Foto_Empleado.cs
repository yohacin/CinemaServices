using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Public
{
    [Table("foto_empleado", Schema = "public")]
    public class Foto_Empleado
    {
        [Key]
        public long id { get; set; }
        public long id_empleado { get; set; }
        public string url_foto { get; set; }
        public int posicion { get; set; }
        public TipoFotoEmpleado tipo_foto_empleado { get; set; }
    }

    public enum TipoFotoEmpleado : int
    {
        EntrenamientoMarcacion = 1,
        Perfil = 2,
    }
}
