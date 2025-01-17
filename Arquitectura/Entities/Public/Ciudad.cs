using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Public;

[Table("ciudad", Schema = "public")]
public class Ciudad
{
    [Key]
    public long id { get; set; } 
    public long id_pais { get; set; }
    public string nombre { get; set; }
    public double? latitud { get; set; }
    public double? longitud { get; set; }
    public int marca_eliminado { get; set; }
}
