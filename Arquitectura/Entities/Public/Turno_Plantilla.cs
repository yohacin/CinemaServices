using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Public;

[Table("turno_plantilla", Schema = "public")]
public class Turno_Plantilla
{
    [Key]
    public long id {  get; set; }
    public long id_jornada { get; set; }
    public string descripcion_turno { get; set; }
    public string inicio_marcacion_entrada { get; set; }
    public string hora_entrada { get; set; }
    public string fin_marcacion_entrada { get; set; }
    public string inicio_marcacion_salida { get; set; }
    public string hora_salida { get; set; }
    public string fin_marcacion_salida { get; set; }
}