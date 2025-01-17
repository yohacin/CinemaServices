using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace wDualAssistence.Controllers
{
    public class PermanenciaController : MainController
    {
        [HttpGet("Permanencia/GetListaPermanencias/{desde}/{hasta}/{id_empresa}")]
        public ActionResult GetListaPermanencias(DateTime desde, DateTime hasta, long id_empresa)
        {
            try
            {
                List<Entities.Helpers.PermanenciaHelper> listaPermanencia = new Business.Public.Permanencia(this._appCnx).GetListaByDate(desde, hasta, id_empresa);

                return Ok(new { transaccion = true, contenido = listaPermanencia });
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