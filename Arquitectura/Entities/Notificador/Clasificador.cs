using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Notificador
{
    [Table("clasificador", Schema = "notificador")]
    public class Clasificador
    {
        [Key]
        public long id { get; set; }
        public long id_tipo_clasificador { get; set; }
        public String descripcion { get; set; }
        public long valor_referencial { get; set; }
    }
}
