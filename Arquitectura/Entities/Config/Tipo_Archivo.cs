using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Config
{
    [Table("tipo_archivo", Schema = "config")]
    public class Tipo_Archivo
    {
        [Key]
        public Int64 id { get; set; }
        public String extension { get; set; }
        public String tipo { get; set; }
        public Int16 ancho { get; set; }
        public Int16 alto { get; set; }
        public Int32 tamano_max { get; set; }        
    }
}
