using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class HojaTrabajoViewModel : ViewModelBase
    {

        public List<Entities.Public.Empresa> listaEmpresa { get; set; }
        public List<Entities.Public.Engagement> listaEngagement { get; set; }


        public HojaTrabajoViewModel() : base()
        {
        }

        public HojaTrabajoViewModel(ClaimsPrincipal user) : base(user)
        {
        }

    }
}
