using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Notificador
{
    [Table("tipo_clasificador", Schema = "notificador")]
    public class Tipo_Clasificador
    {
        [Key]
        public long id { get; set; }
        public String descripcion { get; set; }
        public bool editable { get; set; }
    }
}
