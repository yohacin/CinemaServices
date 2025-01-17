using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.Helpers
{
    public class AsignacionEmpleadoHelper
    {
        [Key]
        public long id { get; set; }
        public long id_empresa { get; set; }
        public string nombre_empresa { get; set; }
        public long id_engagement { get; set; }
        public string nombre_engagement { get; set; }
        public DateTime fecha_inicio { get; set; }
        public DateTime fecha_fin { get; set; }
        public string facturable { get; set; }
        public double horas_asignadas { get; set; }
        public double estimacion_total_horas { get; set; }
        public double estimacion_overrun { get; set; }
        public long id_empleado { get; set; }
        public string nombre_empelado { get; set; }
        public double horas_ejecutadas { get; set; }
    }
}
