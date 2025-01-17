using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Public;

[Table("pais", Schema = "public")]
public class Pais
{
    [Key]
    public long id { get; set; }
    public string nombre { get; set; }
    public int marca_eliminado { get; set; }
}
