using System.ComponentModel.DataAnnotations;

namespace Entities.Helpers
{
    public class ViewDashboardReport
    {
        [Key]
        public long id { get; set; }
        public long id_empresa { get; set; }
        public string nombre_empresa { get; set; }
        public long id_engagement { get; set; }
        public string nombre_engagement { get; set; }
        public string facturable { get; set; }
        public double total_horas_estimadas { get; set; }
        public long id_empleado { get; set; }
        public string nombre_empleado { get; set; }
        public double horas_asignadas_empleado { get; set; }
        public double horas_ejecutadas_empleado { get; set; }
    }
}
