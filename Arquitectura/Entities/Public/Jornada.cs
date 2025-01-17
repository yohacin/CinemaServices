using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Public;

[Table("jornada", Schema = "public")]
public class Jornada
{
    [Key]
    public long id { get; set; }

    [Required(ErrorMessage = "Campo requerido!")]
    [DisplayName("Nombre")]
    [MaxLength(255, ErrorMessage = "El nombre no puede exceder los 255 caracteres.")]
    public string nombre { get; set; }

    [DisplayName("Descripción")]
    [MaxLength(500, ErrorMessage = "El nombre no puede exceder los 500 caracteres.")]
    public string descripcion { get; set; }

    public int marca_eliminado { get; set; }

    [NotMapped]
    public string detalleJSON { get; set; }

    [NotMapped]
    public string turnoPlantillaJSON { get; set; }

    [NotMapped]
    public List<Entities.Public.Detalle_Jornada> listaDetalleJornada { get; set; } = new List<Detalle_Jornada>();
    
    [NotMapped]
    public List<Entities.Public.Turno_Plantilla> listaTurnoPlantilla { get; set; } = new List<Turno_Plantilla>();
}
