using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.Authorization;

using wDualAssistence.Models;
using ESE = Entities.Security;
using BSE = Business.Security;
using Newtonsoft.Json;

namespace wDualAssistence.Controllers
{
    [Authorize]
    public class PerfilController : MainController
    {
        public IActionResult Listado()
        {
            PerfilViewModel vPerfilViewModel = new PerfilViewModel(this.User);
            if (!this._RevisarAcceso(vPerfilViewModel.idUsuario, vPerfilViewModel.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }

            //Cargo Items para el Menu
            vPerfilViewModel.menuItems.Add(new
            {
                id = "1",
                text = "Editar",
                iconCss = "fa fa-pencil"
            });
            vPerfilViewModel.menuItems.Add(new
            {
                id = "2",
                text = "Eliminar",
                iconCss = "fa fa-remove"
            });

            return View(vPerfilViewModel);
        }

        /// <summary>
        /// Odata para listado de usuarios
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public ActionResult<IEnumerable<ESE.Perfil>> Get(ODataQueryOptions<ESE.Perfil> queryOptions)
        {
            try
            {
                var lista = new BSE.Perfil(this._appCnx).GetLista();
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
                PerfilViewModel vPerfilViewModel = new PerfilViewModel(this.User);

                if (id != 0)
                {
                    vPerfilViewModel.ePerfil = new BSE.Perfil(this._appCnx).GetEntity(id);
                    vPerfilViewModel.listaModuloAcceso = new BSE.Perfil(this._appCnx).GetListaModuloAcceso();

                    List<ESE.Perfil_Acceso> listaPerfil_Acceso = new BSE.Perfil(this._appCnx).GetPerfil_Acceso(id);
                    if (listaPerfil_Acceso != null)
                    {
                        //Seteo los check seleccionados
                        foreach (ESE.Perfil_Acceso itemAcceso in listaPerfil_Acceso)
                        {
                            ESE.Acceso eAcceso = new BSE.Acceso(this._appCnx).GetEntity(itemAcceso.id_acceso);
                            foreach (Entities.Helpers.TreeCollection itemCollection in vPerfilViewModel.listaModuloAcceso
                                                    .Where(p => p.parentId > 0 && p.Id == (eAcceso.id + (eAcceso.id_modulo * 1000)) &&
                                                       p.parentId == (eAcceso.id_modulo)
                                                    ))
                            {
                                itemCollection.isChecked = true;
                            }
                        }
                    }
                }
                else
                {
                    vPerfilViewModel.ePerfil = new ESE.Perfil();
                    vPerfilViewModel.ePerfil.estado = true;
                    vPerfilViewModel.listaModuloAcceso = new BSE.Perfil(this._appCnx).GetListaModuloAcceso();
                }

                vPerfilViewModel.menuItems.Add(new
                {
                    id = "1",
                    text = "Expandir Todo",
                    iconCss = "fa fa-expand"
                });
                vPerfilViewModel.menuItems.Add(new
                {
                    id = "2",
                    text = "Colapsar Todo",
                    iconCss = "fa fa-compress"
                });

                return View(vPerfilViewModel);
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
        /// <param name="usuarioViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post(PerfilViewModel vPerfilViewModel)
        {
            try
            {
                BSE.Perfil oPerfil = new BSE.Perfil(this._appCnx);

                vPerfilViewModel.listaModuloAcceso = JsonConvert.DeserializeObject<List<Entities.Helpers.TreeCollection>>(vPerfilViewModel.stringListaPerfiles);
                vPerfilViewModel.ePerfil.listaPerfil_Acceso = new List<ESE.Perfil_Acceso>();
                foreach (Entities.Helpers.TreeCollection item in vPerfilViewModel.listaModuloAcceso.Where(p => p.parentId > 0 && p.isChecked == true))
                {
                    ESE.Perfil_Acceso ePerfil_Acceso = new ESE.Perfil_Acceso();
                    ePerfil_Acceso.id_acceso = item.Id - (item.parentId * 1000);

                    vPerfilViewModel.ePerfil.listaPerfil_Acceso.Add(ePerfil_Acceso);
                }

                vPerfilViewModel.ePerfil.baja = false;

                if (vPerfilViewModel.ePerfil.id != 0)
                {
                    oPerfil.Modificar(vPerfilViewModel.ePerfil);
                }
                else
                {
                    oPerfil.Guardar(vPerfilViewModel.ePerfil);
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
                new BSE.Perfil(this._appCnx).Eliminar(id);
                return Ok(new { transaccion = true });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }
    }
}