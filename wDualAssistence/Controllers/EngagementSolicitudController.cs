using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using EPU = Entities.Public;
using BPU = Business.Public;
using wDualAssistence.Models;

namespace wDualAssistence.Controllers
{
    public class EngagementSolicitudController : MainController
    {
        public IActionResult Listado()
        {
            EngagementSolicitudViewModel vEngagementSolicitudViewModel = new EngagementSolicitudViewModel(this.User);

            vEngagementSolicitudViewModel.listaSolicitud = new BPU.Engagement_Modificacion_Solicitud(this._appCnx).GetListaByEmpleado(vEngagementSolicitudViewModel.idUsuario);

            return View(vEngagementSolicitudViewModel);
        }
    }
}