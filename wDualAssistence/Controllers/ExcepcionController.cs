using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using wDualAssistence.Models;

namespace wDualAssistence.Controllers
{
    [Authorize]
    public class ExcepcionController : MainController
    {
        public IActionResult Listado()
        {
            ExcepcionViewModel vExcepcionViewModel = new ExcepcionViewModel(this.User);
            if (!this._RevisarAcceso(vExcepcionViewModel.idUsuario, vExcepcionViewModel.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }

            vExcepcionViewModel.listaExcepcion = new Business.Public.Excepcion(this._appCnx).GetLista();

            return View(vExcepcionViewModel);
        }
    }
}