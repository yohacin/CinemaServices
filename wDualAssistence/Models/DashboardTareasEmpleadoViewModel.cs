using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class DashboardTareasEmpleadoViewModel : ViewModelBase
    {
        public DashboardTareasEmpleadoViewModel()
        {
        }

        public DashboardTareasEmpleadoViewModel(ClaimsPrincipal User) : base(User)
        {
        }

        public List<Entities.Public.Engagement> listaEngagement { get; set; }

    }
}
