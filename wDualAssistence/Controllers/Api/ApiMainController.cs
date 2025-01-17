using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Mvc;

namespace wDualAssistence.Controllers.Api
{
    public class ApiMainController : Controller
    {
        protected IApplicationDbContext _appCnx => (IApplicationDbContext)HttpContext.RequestServices.GetService(typeof(IApplicationDbContext));
        protected readonly log4net.ILog _log;
        public string __MENSAJE_ACCESO_NEGADO = "Su usuario no tiene acceso a este programa";

        public ApiMainController()
        {
            _log = log4net.LogManager.GetLogger(this.GetType());
        }

    }
}