using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using wDualAssistence.Models;
using ESE = Entities.Security;
using BSE = Business.Security;
using Newtonsoft.Json;
using wDualAssistence.Helpers;
using Syncfusion.EJ2.Navigations;
using Microsoft.AspNetCore.OData.Query;

namespace wDualAssistence.Controllers
{
    [Authorize]
    public class UsuarioController : MainController
    {
        public IActionResult Listado()
        {
            UsuarioViewModel vUsuarioViewModel = new UsuarioViewModel(this.User);
            if (!this._RevisarAcceso(vUsuarioViewModel.idUsuario, vUsuarioViewModel.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }

            //Cargo Items para el Menu
            vUsuarioViewModel.menuItems.Add(new
            {
                id = "1",
                text = "Editar",
                iconCss = "fa fa-pencil"
            });
            vUsuarioViewModel.menuItems.Add(new
            {
                id = "2",
                text = "Eliminar",
                iconCss = "fa fa-remove"
            });

            return View(vUsuarioViewModel);
        }

        /// <summary>
        /// Odata para listado de usuarios
        /// </summary>
        /// <param name="queryOptions"></param>
        /// <returns></returns>
        [EnableQuery]
        [HttpGet]
        public ActionResult<IEnumerable<ESE.Usuario>> Get(ODataQueryOptions<ESE.Usuario> queryOptions)
        {
            try
            {
                var lista = new BSE.Usuario(this._appCnx).GetLista();
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
                UsuarioViewModel vUsuarioViewModel = new UsuarioViewModel(this.User);
                vUsuarioViewModel.listaPerfil = new BSE.Perfil(this._appCnx).GetLista();

                if (id != 0)
                {
                    vUsuarioViewModel.eUsuario = new BSE.Usuario(this._appCnx).GetEntity(id);
                    vUsuarioViewModel.listaModuloAcceso = new BSE.Perfil(this._appCnx).GetListaModuloAccesoExcudePerfil(vUsuarioViewModel.eUsuario.id_perfil);

                    List<ESE.Usuario_Acceso> listaUsuario_Acceso = new BSE.Usuario(this._appCnx).GetUsuario_Acceso(id);
                    if (listaUsuario_Acceso != null)
                    {
                        //Seteo los check seleccionados
                        foreach (ESE.Usuario_Acceso itemAcceso in listaUsuario_Acceso)
                        {
                            ESE.Acceso eAcceso = new BSE.Acceso(this._appCnx).GetEntity(itemAcceso.id_acceso);
                            foreach (Entities.Helpers.TreeCollection itemCollection in vUsuarioViewModel.listaModuloAcceso
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
                    vUsuarioViewModel.eUsuario = new ESE.Usuario();
                    vUsuarioViewModel.eUsuario.estado = true;
                    vUsuarioViewModel.eUsuario.dashboard = true;
                    vUsuarioViewModel.eUsuario.configuracion = true;
                    vUsuarioViewModel.listaModuloAcceso = new BSE.Perfil(this._appCnx).GetListaModuloAcceso();
                }

                ViewBag.positionData = new string[] { "Scrollable", "Popup" };
                ViewBag.orientationData = new string[] { "Top", "Bottom", "Left", "Right" };
                ViewBag.headerTextTwo = new TabHeader { Text = "Perfiles Adicionales", IconCss = "fa fa-list" };
                ViewBag.headerTextOne = new TabHeader { Text = "Foto", IconCss = "fa e-photo" };
                ViewBag.data = new string[] { "Bolivianos", "Dolares" };

                vUsuarioViewModel.menuItems.Add(new
                {
                    id = "1",
                    text = "Expandir Todo",
                    iconCss = "fa fa-expand"
                });
                vUsuarioViewModel.menuItems.Add(new
                {
                    id = "2",
                    text = "Colapsar Todo",
                    iconCss = "fa fa-compress"
                });

                return View(vUsuarioViewModel);
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
        public IActionResult Post(UsuarioViewModel vUsuarioViewModel)
        {
            try
            {
                BSE.Usuario oUsuario = new BSE.Usuario(this._appCnx);
                if (vUsuarioViewModel.eUsuario.id_perfil <= 0)
                    throw new Exception("Debe seleccionar el perfil del usuario");
                vUsuarioViewModel.listaModuloAcceso = JsonConvert.DeserializeObject<List<Entities.Helpers.TreeCollection>>(vUsuarioViewModel.stringListaPerfiles);
                vUsuarioViewModel.eUsuario.listaUsuario_Acceso = new List<ESE.Usuario_Acceso>();
                foreach (Entities.Helpers.TreeCollection item in vUsuarioViewModel.listaModuloAcceso.Where(p => p.parentId > 0 && p.isChecked == true))
                {
                    ESE.Usuario_Acceso eUsuario_Acceso = new ESE.Usuario_Acceso();
                    eUsuario_Acceso.id_acceso = item.Id - (item.parentId * 1000);

                    vUsuarioViewModel.eUsuario.listaUsuario_Acceso.Add(eUsuario_Acceso);
                }
                vUsuarioViewModel.eUsuario.baja = false;

                if (vUsuarioViewModel.eUsuario.id != 0)
                {
                    ESE.Usuario eUsuarioRepetido = new BSE.Usuario(this._appCnx).GetEntity(vUsuarioViewModel.eUsuario.id);
                    vUsuarioViewModel.eUsuario.clave = eUsuarioRepetido.clave;
                    vUsuarioViewModel.eUsuario.updated_at = eUsuarioRepetido.updated_at;
                    oUsuario.Modificar(vUsuarioViewModel.eUsuario);
                }
                else
                {
                    if (vUsuarioViewModel.eUsuario.foto == null) vUsuarioViewModel.eUsuario.foto = "";
                    vUsuarioViewModel.eUsuario.updated_at = DateTime.Now;
                    vUsuarioViewModel.eUsuario.clave = Cryptography.Encrypt(vUsuarioViewModel.eUsuario.clave, Startup.llaveCryptography);
                    oUsuario.Guardar(vUsuarioViewModel.eUsuario);
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

        [HttpPost]
        public IActionResult setContrasena(long id_usuario, string nueva_contrasena)
        {
            try
            {
                UsuarioViewModel vUsuarioViewModel = new UsuarioViewModel(this.User);

                nueva_contrasena = Cryptography.Encrypt(nueva_contrasena, Startup.llaveCryptography);
                if (vUsuarioViewModel.idUsuario == id_usuario)
                {
                    this.AddUpdateClaim("contrasena", nueva_contrasena);
                }

                if (new BSE.Usuario(this._appCnx).setContrasena(id_usuario, nueva_contrasena))
                {
                    return Ok(new { transaccion = true, mensaje = "¡ Contraseña cambiada correctamente !" });
                }
                else
                {
                    return Ok(new { transaccion = false, mensaje = "¡ Error al cambiar la contraseña !" });
                }
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpGet("Usuario/SetContrasenaValidar/{id_usuario}/{actual_contrasena}/{nueva_contrasena}/{tipo}")]
        public IActionResult SetContrasenaValidar(long id_usuario, string actual_contrasena, string nueva_contrasena, long tipo)
        {
            try
            {
                if (tipo == 1) // tipo usuario
                {
                    UsuarioViewModel vUsuarioViewModel = new UsuarioViewModel(this.User);

                    actual_contrasena = Cryptography.Encrypt(actual_contrasena, Startup.llaveCryptography);
                    if (actual_contrasena != vUsuarioViewModel.Contrasena)
                        return Ok(new { transaccion = false, mensaje = "La contraseña actual no coincide !" });

                    nueva_contrasena = Cryptography.Encrypt(nueva_contrasena, Startup.llaveCryptography);
                    if (vUsuarioViewModel.idUsuario == id_usuario)
                    {
                        this.AddUpdateClaim("contrasena", nueva_contrasena);
                    }

                    if (new BSE.Usuario(this._appCnx).setContrasena(id_usuario, nueva_contrasena))
                    {
                        return Ok(new { transaccion = true, mensaje = "¡ Contraseña cambiada correctamente !" });
                    }
                    else
                    {
                        return Ok(new { transaccion = false, mensaje = "¡ Error al cambiar la contraseña !" });
                    }
                }
                else
                { // ---------------------tipo empleado
                    UsuarioViewModel vUsuarioViewModel = new UsuarioViewModel(this.User);

                    actual_contrasena = Cryptography.EncriptarMD5(actual_contrasena);
                    if (actual_contrasena != vUsuarioViewModel.Contrasena)
                        return Ok(new { transaccion = false, mensaje = "La contraseña actual no coincide !" });

                    nueva_contrasena = Cryptography.EncriptarMD5(nueva_contrasena);
                    if (vUsuarioViewModel.idUsuario == id_usuario)
                    {
                        this.AddUpdateClaim("contrasena", nueva_contrasena);
                    }

                    if (new Business.Public.Empleado(this._appCnx).setContrasena(id_usuario, nueva_contrasena))
                    {
                        return Ok(new { transaccion = true, mensaje = "¡ Contraseña cambiada correctamente !" });
                    }
                    else
                    {
                        return Ok(new { transaccion = false, mensaje = "¡ Error al cambiar la contraseña !" });
                    }
                }
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpGet("Usuario/Get/{id_usuario}")]
        public IActionResult GetById(long id_usuario)
        {
            try
            {
                var eUsuario = new BSE.Usuario(this._appCnx).GetEntity(id_usuario);
                if (eUsuario != null)
                {
                    return Ok(new { transaccion = true, usuario = eUsuario });
                }
                else
                {
                    return Ok(new { transaccion = false, mensaje = "No existe el usuario con id: " + id_usuario });
                }
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
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
                new BSE.Usuario(this._appCnx).Eliminar(id);
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