using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.Helpers
{
    public class PlaneacionEngagementHelper
    {
        [Key]
        public long id { get; set; }
        public long id_empresa { get; set; }
        public string nombre_empresa { get; set; }
        public long id_engagement { get; set; }
        public string nombre_engagement { get; set; }
        public long id_empleado { get; set; }
        public string codigo_empleado { get; set; }
        public string nombre_empleado { get; set; }
        public string cargo { get; set; }


    }
}
