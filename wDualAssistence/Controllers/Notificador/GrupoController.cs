using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using wDualAssistence.Models.Notificador;
using ENO = Entities.Notificador;
using BNO = Business.Notificador;
using Microsoft.AspNetCore.OData.Query;
using Newtonsoft.Json;
using wDualAssistence.Models;

namespace wDualAssistence.Controllers.Notificador
{
    [Authorize]
    public class GrupoController : MainController
    {
        public IActionResult Listado()
        {
            GrupoViewModel vGrupoViewModel = new GrupoViewModel(this.User);
            if (!this._RevisarAcceso(vGrupoViewModel.idUsuario, vGrupoViewModel.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }
            //Cargo Items para el Menu
            vGrupoViewModel.menuItems.Add(new
            {
                id = "1",
                text = "Editar",
                iconCss = "fa fa-pencil"
            });
            vGrupoViewModel.menuItems.Add(new
            {
                id = "2",
                text = "Eliminar",
                iconCss = "fa fa-remove"
            });

            return View(vGrupoViewModel);
        }

        /// <summary>
        /// Odata para listado
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public ActionResult<IEnumerable<ENO.Grupo>> Get(ODataQueryOptions<ENO.Grupo> queryOptions)
        {
            try
            {
                var lista = new BNO.Grupo(this._appCnx).ListGrupos();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult Crear([FromForm] long id)
        {
            try
            {
                GrupoViewModel vGrupoViewModel = new GrupoViewModel(this.User);

                vGrupoViewModel.listaUsuario = new Business.Security.Usuario(this._appCnx).GetLista();

                if (id != 0)
                {
                    vGrupoViewModel.eGrupo = new BNO.Grupo(this._appCnx).Buscar(id);
                }
                else
                {
                    vGrupoViewModel.eGrupo = new ENO.Grupo();
                }

                return View(vGrupoViewModel);
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
        public IActionResult Post(GrupoViewModel vGrupoViewModel)
        {
            try
            {
                if (vGrupoViewModel.eGrupo.tipo == 1)
                    vGrupoViewModel.eGrupo.oLstContactos = JsonConvert.DeserializeObject<List<ENO.Contacto>>(vGrupoViewModel.stringListaContacto);

                if (vGrupoViewModel.eGrupo == null) throw new Exception("Los Datos no fueron recibidos");
                if (vGrupoViewModel.eGrupo.nombre == null) throw new Exception("El campo nombre es requerido");

                if (vGrupoViewModel.eGrupo.tipo == 2 && vGrupoViewModel.eGrupo.query == null) throw new Exception("El campo query es requerido");
                vGrupoViewModel.eGrupo.marca_eliminado = 0;
                vGrupoViewModel.eGrupo.administrable = true;
                if (vGrupoViewModel.eGrupo.codigo_grupo != 0)
                {
                    new BNO.Grupo(this._appCnx).Modificar(vGrupoViewModel.eGrupo);
                }
                else
                {
                    new BNO.Grupo(this._appCnx).Guardar(vGrupoViewModel.eGrupo);
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
                new BNO.Grupo(this._appCnx).Eliminar(id);
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