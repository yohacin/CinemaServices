using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entities.Helpers
{
    public class AsignacionHelper
    {
        [Key]
        public long id_engagement_empleado { get; set; }
        public long id_empresa { get; set; }
        public string nombre_empresa { get; set; }
        public long id_engagement { get; set; }
        public string nombre_engagement { get; set; }
        public double horas_asignadas { get; set; }
        public double horas_utilizadas { get; set; }
        public double horas_disponibles { get; set; }


    }
}
