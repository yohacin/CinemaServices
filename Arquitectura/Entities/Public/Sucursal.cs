using Microsoft.OData.ModelBuilder;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Public;

[Table("sucursal", Schema = "public")]
public class Sucursal
{
    [Key]
    public long id { get; set; }
    public long id_ciudad { get; set; }

    [Required(ErrorMessage = "Campo requerido!")]
    [MaxLength(255, ErrorMessage = "El Nombre no puede exceder los 255 caracteres.")]
    public string nombre { get; set; }

    [MaxLength(500, ErrorMessage = "La Dirección no puede exceder los 500 caracteres.")]
    public string direccion { get; set; }

    public int marca_eliminado { get; set; }


    [AutoExpand]
    public Entities.Public.Ciudad ciudad { get; set; }
}
