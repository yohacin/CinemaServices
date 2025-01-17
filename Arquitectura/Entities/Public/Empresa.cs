using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("empresa", Schema = "public")]
    public class Empresa
    {
        [Key]
        public long id { get; set; }
        public string nombre { get; set; }
        public string direccion { get; set; }
        public string nit { get; set; }
        public string razon_social { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public int perimetro { get; set; }
        public bool tiene_sucursales { get; set; }
        public long codigo_casa_matriz { get; set; }
        public long id_creador { get; set; }
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd HH:mm:ss.fff")]
        public DateTime fecha_registro { get; set; }
        public string persona_contacto { get; set; }
        public string logo { get; set; }
        public int marca_eliminado { get; set; }

        [NotMapped]
        public List<Entities.Public.Foto_Empresa> listFotos { get; set; }

        [NotMapped]
        public List<Entities.Public.Empleado> listaEmpleado { get; set; }
        [NotMapped]
        public List<Entities.Public.Empleado_Empresa> listaEmpleado_Empresa { get; set; }
        [NotMapped]
        public List<Entities.Public.Engagement> listaEngagement { get; set; }
    }
}
