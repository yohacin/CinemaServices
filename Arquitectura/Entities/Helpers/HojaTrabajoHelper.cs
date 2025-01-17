using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities.Helpers
{
    public class HojaTrabajoHelper
    {
        [Key]
        public long id { get; set; }
        public DateTime fecha { get; set; }
        public long id_empresa { get; set; }
        public string nombre_empresa { get; set; }
        public long id_engagement { get; set; }
        public string titulo_engagement { get; set; }
        public long id_tarea { get; set; }
        public string nombre_tarea { get; set; }
        public double total { get; set; }
        public double lunes { get; set; }
        public double martes { get; set; }
        public double miercoles { get; set; }
        public double jueves { get; set; }
        public double viernes { get; set; }
        public double sabado { get; set; }
        public double domingo { get; set; }

        [NotMapped]
        public long id_ref { get; set; }
    }
}
