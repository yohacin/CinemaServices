using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;

namespace wDualAssistence.Controllers
{
    public class MarcacionErroneaController : MainController
    {

        [HttpGet("MarcacionErronea/GetListaMarcacionErronea/{desde}/{hasta}")]
        public ActionResult GetListaMarcacionErronea(DateTime desde, DateTime hasta)
        {
            try
            {
                List<Entities.Helpers.MarcacionErroneaHelper> listaMarcacion = new Business.Public.MarcacionErronea(this._appCnx).GetListaByDate(desde, hasta);

                return Ok(new { transaccion = true, contenido = listaMarcacion });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
        }


    }
}
