using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("encuesta_config", Schema = "public")]
    public class Encuesta_Config
    {
        [Key]
        public long id { get; set; }
        public string pregunta { get; set; }
        public bool habilitado { get; set; }
        public int estado { get; set; }
        public long id_grupo { get; set; }


    }
}
