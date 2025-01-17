using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using ESE = Entities.Security;
using BSE = Business.Security;
using Microsoft.AspNetCore.Authorization;
using wDualAssistence.Helpers;

namespace wDualAssistence.Controllers
{
    [Authorize]
    public class ModuloController : MainController
    {
        [HttpPost]
        public ActionResult<List<ESE.Modulo>> GetModulosByUsuario(long idUsuario)
        {
            try
            {
                long tipo = Convert.ToInt64(this.User.GetClaimValue("tipo"));
                return new BSE.Acceso(this._appCnx).GetModulosxUsuario(idUsuario, tipo);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult<List<ESE.Acceso>> GetAccesoByUsuario(long idUsuario)
        {
            try
            {
                long tipo = Convert.ToInt64(this.User.GetClaimValue("tipo"));
                return new BSE.Acceso(this._appCnx).GetAccesoxUsuario(idUsuario, tipo);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }
    }
}