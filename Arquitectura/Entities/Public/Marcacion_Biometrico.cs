using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.OData.ModelBuilder;
using Entities.Security;
using Newtonsoft.Json;

namespace Entities.Public;

[Table("marcacion_biometrico", Schema = "public")]
public class Marcacion_Biometrico
{
    [Key]
    public long id { get; set; }

    public long id_formato_marcacion_biometrico { get; set; }

    public long id_empresa { get; set; }

    public long id_usuario { get; set; }

    public string nombre_archivo { get; set; }

    public string ruta_archivo { get; set; }

    [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]

    public DateTime fecha_registro { get; set; }

    [NotMapped]
    public List<Detalle_Marcacion_Biometrico> listaDetalleMarcacion {  get; set; }


    [AutoExpand]
    public Usuario usuario { get; set; }


    [AutoExpand]
    public Empresa empresa { get; set; }

    [AutoExpand]
    public Formato_Marcacion_Biometrico formato { get; set; }
}