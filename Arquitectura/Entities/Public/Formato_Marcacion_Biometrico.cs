using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Public;

[Table("formato_marcacion_biometrico", Schema = "public")]

public class Formato_Marcacion_Biometrico
{
    [Key]
    public long id { get; set; }

    [Required(ErrorMessage = "Campo requerido!")]
    [MaxLength(255, ErrorMessage = "El Nombre no puede exceder los 255 caracteres.")]
    public string nombre { get; set; }

    [Required(ErrorMessage = "Campo requerido!")]
    [MaxLength(255, ErrorMessage = "El campo no puede exceder los 100 caracteres.")]
    public string col_cod_biometrico { get; set; }

    [Required(ErrorMessage = "Campo requerido!")]
    [MaxLength(255, ErrorMessage = "El campo no puede exceder los 100 caracteres.")]
    public string col_fecha_hora_entrada { get; set; }

    [MaxLength(255, ErrorMessage = "El campo no puede exceder los 100 caracteres.")]
    public string col_fecha_hora_salida { get; set; }
    public bool estan_misma_fila { get; set; }
    public int marca_eliminado { get; set; }
}
