using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class EngagementViewModel : ViewModelBase
    {
        public Entities.Public.Engagement eEngagement { get; set; }
        public string nombre_empresa { get; set; }
        public List<Entities.Public.Empleado> listaEmpleado { get; set; }
        public List<Entities.Helpers.TreeCollection> listaCategoriaTarea { get; set; }
        public List<Entities.Public.Cargo_Engagement> listaCargo { get; set; }
        public List<Entities.Public.Area_Engagement> listaArea { get; set; }
        public List<Entities.Public.Engagement> listaEngagement { get; set; }
        public List<Entities.Public.Empresa> listaEmpresa { get; set; }
        public List<Entities.Public.Engagement_Modificacion_Solicitud> listaLog{ get; set; }
        public Entities.Public.Engagement_Modificacion_Solicitud eSolicitudPendiente { get; set; }

        public string stringListaAlertas { get; set; }
        public string stringListaDetalleHoras { get; set; }
        public string stringListaResponsables { get; set; }
        public string stringListaTareas { get; set; }


        public Entities.Public.Tarea eTarea { get; set; }
        public List<Entities.Public.Categoria_Tarea> listaCategoria_Tarea { get; set; }

        public List<Entities.Public.Engagement_Empleado> listaEngagement_Empleado { get; set; }

        public List<long> idsEmpresas { get; set; }

        public EngagementViewModel() : base()
        {
        }

        public EngagementViewModel(ClaimsPrincipal user) : base(user)
        {
        }
    }
}
