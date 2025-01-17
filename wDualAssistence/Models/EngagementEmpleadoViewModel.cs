using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class EngagementEmpleadoViewModel : ViewModelBase
    {

        public Entities.Public.Engagement_Empleado eEngagement_Empleado { get; set; }
        public Entities.Public.Detalle_Engagement_Empleado eDetalle_Engagement_Empleado { get; set; }
        public List<Entities.Public.Detalle_Engagement_Empleado> listaDetalle_Engagement_Empleado { get; set; }
        public List<Entities.Public.Empleado> listaEmpleado { get; set; }
        public string nombre_empleado { get; set; }
        public string nombre_empresa { get; set; }
        public double horas_disponibles { get; set; }
        public Entities.Public.Engagement eEngagement { get; set; }

        public EngagementEmpleadoViewModel() : base()
        {
        }

        public EngagementEmpleadoViewModel(ClaimsPrincipal user) : base(user)
        {
        }

    }
}
