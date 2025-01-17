using System;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace wDualAssistence.Controllers
{
    [Authorize]
    public class MainController : Controller
    {
        protected IApplicationDbContext _appCnx => (IApplicationDbContext)HttpContext.RequestServices.GetService(typeof(IApplicationDbContext));
        protected readonly log4net.ILog _log;
        public string __MENSAJE_ACCESO_NEGADO = "Su usuario no tiene acceso a este programa";

        public MainController()
        {
            _log = log4net.LogManager.GetLogger(this.GetType());
        }

        public bool _RevisarAcceso(long id_usuario, long tipo, string pageRequest)
        {
            bool respuesta = false;
            try
            {
                respuesta = new Business.Security.Acceso(this._appCnx).RevisarAcceso(id_usuario, tipo, pageRequest);
            }
            catch (Exception ex)
            {
                _log.Error("ERROR VALIDAR ACCESO: " + ex);
            }
            return respuesta;
        }
    }
}