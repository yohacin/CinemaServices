using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Entities.Public;

[Table("detalle_marcacion_biometrico", Schema = "public")]
public class Detalle_Marcacion_Biometrico
{
    [Key]
    public long id { get; set; }

    public long id_marcacion_biometrico { get; set; }

    public string codigo_biometrico { get; set; }

    [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
    public DateTime fecha_marcacion { get; set; }

    [NotMapped]
    public long id_empresa { get; set; }
}