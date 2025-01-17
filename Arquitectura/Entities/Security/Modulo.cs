using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Security
{
    [Table("modulo", Schema = "security")]
    public class Modulo
    {
        [Key]
        public long id { get; set; }
        public string nombre { get; set; }
        public long idc_app_type { get; set; }
        public int secuencia { get; set; }
        public string icon { get; set; }
    }
}
