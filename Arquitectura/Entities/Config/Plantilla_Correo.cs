using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Config
{
    [Table("plantilla_correo", Schema = "config")]
    public class Plantilla_Correo
    {
        [Key]
        public long id { get; set; }
        public String cabecera_template { get; set; }
        public String cuerpo_template { get; set; }
        public String pie_template { get; set; }
        public String tipo { get; set; }
        public long id_empresa { get; set; }
        public int marca_eliminado { get; set; }
    }
}
