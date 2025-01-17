using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using EPU = Entities.Public;
using BPU = Business.Public;
using Microsoft.AspNetCore.OData.Query;

namespace wDualAssistence.Controllers
{
    public class AreaController : MainController
    {
        /// <summary>
        /// Odata para listado
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public ActionResult<IEnumerable<EPU.Area_Engagement>> Get(ODataQueryOptions<EPU.Area_Engagement> queryOptions)
        {
            try
            {
                var lista = new BPU.Area_Engagement(this._appCnx).GetLista();
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
        public IActionResult Post(EPU.Area_Engagement eArea_Engagement)
        {
            try
            {
                if (eArea_Engagement == null) throw new Exception("Los Datos no fueron recibidos");
                if (eArea_Engagement.nombre == null) throw new Exception("El campo nombre es requerido");

                eArea_Engagement.estado = 0;
                if (eArea_Engagement.id != 0)
                {
                    new BPU.Area_Engagement(this._appCnx).Modificar(eArea_Engagement);
                }
                else
                {
                    new BPU.Area_Engagement(this._appCnx).Guardar(eArea_Engagement);
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
                new BPU.Area_Engagement(this._appCnx).Eliminar(id);
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