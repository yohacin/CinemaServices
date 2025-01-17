using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using EPU = Entities.Public;
using BPU = Business.Public;
using Microsoft.AspNetCore.OData.Query;

namespace wDualAssistence.Controllers
{
    public class CargoController : MainController
    {
        /// <summary>
        /// Odata para listado
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public ActionResult<IEnumerable<EPU.Cargo_Engagement>> Get(ODataQueryOptions<EPU.Cargo_Engagement> queryOptions)
        {
            try
            {
                var lista = new BPU.Cargo_Engagement(this._appCnx).GetLista();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        ///  Para la parte de carga de datos en base
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post(EPU.Cargo_Engagement eCargo_Engagement)
        {
            try
            {
                if (eCargo_Engagement == null) throw new Exception("Los Datos no fueron recibidos");
                if (eCargo_Engagement.nombre == null) throw new Exception("El campo nombre es requerido");

                eCargo_Engagement.estado = 0;
                if (eCargo_Engagement.id != 0)
                {
                    new BPU.Cargo_Engagement(this._appCnx).Modificar(eCargo_Engagement);
                }
                else
                {
                    new BPU.Cargo_Engagement(this._appCnx).Guardar(eCargo_Engagement);
                }
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
            return Ok(new { transaccion = true });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                new BPU.Cargo_Engagement(this._appCnx).Eliminar(id);
                return Ok(new { transaccion = true });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
        }
    }
}