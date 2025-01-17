using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.Helpers
{
    public class SeguimientoEngagementHelper
    {
        [Key]
        public long id_empleado_hoja_trabajo { get; set; }
        public string nombre_empresa { get; set; }
        public string nombre_engagement { get; set; }
        public DateTime fecha_inicio { get; set; }
        public DateTime fecha_fin { get; set; }
        public double estimacion_horas_tarea { get; set; }
        public double horas_overrun { get; set; }
        public string facturable { get; set; }
        public string categoria { get; set; }
        public string tarea { get; set; }
        public string empleado { get; set; }
        public DateTime fecha_tarea { get; set; }
        public double tiempo_ejecutado { get; set; }
    }
}
