using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("encuesta", Schema = "public")]
    public class Encuesta
    {
        [Key]
        public long id { get; set; }
        public DateTime fecha { get; set; }
        public string pregunta { get; set; }
        public string respuesta { get; set; }
        public bool notificar { get; set; }
        public bool notificacion_ejecutada { get; set; }
        public long id_empleado { get; set; }
        public int estado { get; set; }


    }
}
