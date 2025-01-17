using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Public
{
    [Table("empleado_empresa", Schema = "public")]
    public class Empleado_Empresa
    {
        [Key]
        public long id { get; set; }
        public string codigo_empleado { get; set; }
        public long id_empresa { get; set; }
        public long id_empleado { get; set; }
        public DateTime fecha_inicio { get; set; }
        public DateTime? fecha_fin { get; set; }
    }
}
