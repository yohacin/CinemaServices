using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("engagement_modificacion_solicitud", Schema = "public")]
    public class Engagement_Modificacion_Solicitud
    {
        [Key]
        public long id { get; set; }
        public long id_engagement { get; set; }
        public long id_usuario_solicita { get; set; }
        public DateTime fecha_solicita { get; set; }
        public string json_modificado { get; set; }
        public long id_usuario_autoriza { get; set; }
        public DateTime? fecha_autoriza { get; set; }
        public string descripcion { get; set; }
        public string json_anterior { get; set; }
        public int estado_solicitud { get; set; }
        public int estado { get; set; }

        [NotMapped]
        public string nombre_solicitante { get; set; }
        [NotMapped]
        public string nombre_autorizador { get; set; }
    }
}
