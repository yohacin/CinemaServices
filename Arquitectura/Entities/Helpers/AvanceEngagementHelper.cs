using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.Helpers
{
    public class AvanceEngagementHelper
    {
        [Key]
        public long id_engagement { get; set; }
        public long id_empresa { get; set; }
        public string nombre_empresa { get; set; }
        public string nombre_engagement { get; set; }
        public DateTime desde { get; set; }
        public DateTime hasta { get; set; }
        public double horas_trabajo { get; set; }
        public double horas_ejecutadas { get; set; }
        public int porcentaje_avance { get; set; }
    }
}
