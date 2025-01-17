using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Models
{
    public class EngagementSolicitudViewModel : ViewModelBase
    {

        public List<Entities.Public.Engagement_Modificacion_Solicitud> listaSolicitud { get; set; }

        public EngagementSolicitudViewModel() : base()
        {
        }

        public EngagementSolicitudViewModel(ClaimsPrincipal user) : base(user)
        {
        }

    }
}
