using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using wDualAssistence.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

#region Abreviaturas

using ESE = Entities.Security;
using ENO = Entities.Notificador;
using BNO = Business.Notificador;
using BCO = Business.Config;

#endregion Abreviaturas

#region Librerias Externas

using Syncfusion.EJ2.Navigations;
using wDualAssistence.Helpers;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using wDualAssistence.Controllers;
using wDualAssistence.Utility;
using wDualAssistence;

#endregion Librerias Externas

namespace wNetCoreMainter.Controllers
{
    public class PlantillaController : MainController
    {
        private BNO.Plantilla bPlantilla;
        private BNO.Clasificador bClasificador;
        private BCO.TipoArchivo bTipoArchivo;
        private PlantillaViewModel vPlantilla;

        private IHostingEnvironment hostingEnv;

        public PlantillaController(IHostingEnvironment env)
        {
            this.bPlantilla = new BNO.Plantilla(this._appCnx);
            this.bClasificador = new BNO.Clasificador(this._appCnx);
            this.bTipoArchivo = new BCO.TipoArchivo(this._appCnx);
            this.hostingEnv = env;
        }

        public IActionResult Listado()
        {
            this.vPlantilla = new PlantillaViewModel(this.User);
            if (!this._RevisarAcceso(vPlantilla.idUsuario, vPlantilla.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }

            vPlantilla.eLstPlantilla = bPlantilla.ListarByEmpresa(vPlantilla.idEmpresa);

            vPlantilla.menuItems.Add(new
            {
                id = "1",
                text = "Editar",
                iconCss = "fa fa-pencil"
            });
            vPlantilla.menuItems.Add(new
            {
                id = "2",
                text = "Eliminar",
                iconCss = "fa fa-remove"
            });

            return View(vPlantilla);
        }

        [HttpPost]
        public IActionResult Crear([FromForm] long idPlantilla)
        {
            try
            {
                this.vPlantilla = new PlantillaViewModel(this.User);

                if (idPlantilla != 0) //Editar
                {
                    vPlantilla.ePlantilla = bPlantilla.Buscar(idPlantilla);
                }
                else
                {
                    vPlantilla.ePlantilla.id_empresa = vPlantilla.idEmpresa;
                }

                ViewBag.toolsHead = new[] {
                "Bold", "Italic", "Underline",
                "FontName", "FontSize", "FontColor", "BackgroundColor",
                "LowerCase", "UpperCase", "|", "Alignments",
                "|", "Undo", "Redo", "SourceCode"
                    };

                ViewBag.toolsBody = new[] {
                "Bold", "Italic", "Underline",
                "FontName", "FontSize", "FontColor", "BackgroundColor",
                "LowerCase", "UpperCase", "|",
                "Formats", "Alignments", "OrderedList", "UnorderedList", "|",
                "Image", "CreateTable", "|", "Undo", "Redo", "SourceCode"
                    };

                this.vPlantilla.oLstTipoPlantilla = bClasificador.ListarByTipo(3);

                string rutaRecurso = $@"{this.hostingEnv.WebRootPath}\adjuntosPlantilla\";
                this.vPlantilla.longitudRutaRecurso = rutaRecurso.Length;
                this.vPlantilla.adjuntosUrl = Startup.appUrl + "adjuntosPlantilla/";
                this.vPlantilla.listaTipoArchivosPermitidos = bTipoArchivo.GetListaTipoArchivo();

                return View(vPlantilla);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult Post(PlantillaViewModel viewModel)
        {
            TryValidateModel(viewModel.ePlantilla);

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        return Json(new { Result = false, Msg = error.ErrorMessage });
                    }
                }
            }

            try
            {
                ViewModelBase.Accion accion;

                if (viewModel.ePlantilla.id != 0)
                {
                    bPlantilla.Modificar(viewModel.ePlantilla);
                    accion = ViewModelBase.Accion.Modificar;
                }
                else
                {
                    bPlantilla.Guardar(viewModel.ePlantilla);
                    accion = ViewModelBase.Accion.Guardar;
                }

                return Ok(new { Result = true, Msg = "¡ Registro realizado correctamente !" });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                this.vPlantilla = new PlantillaViewModel(this.User);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                bPlantilla.Eliminar(id);

                return Ok(new { Result = true, Msg = "¡ Registro Eliminado Correctamente !" });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult ObtenerCamposQuery([FromForm] string serializedQueries)
        {
            try
            {
                List<ENO.Query> queries = JsonConvert.DeserializeObject<List<ENO.Query>>(serializedQueries);
                var result = bPlantilla.ObtenerCamposQuery(queries);
                var tiposParametro = bClasificador.ListarByTipo(4);
                return Ok(new { Result = true, Msg = "¡ Campos obtenidos correctamente !", Data = result, TiposParametro = tiposParametro });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                var exception = new { Message = ex.Message, StackTrace = ex.StackTrace };
                return BadRequest(new { ErrorMessage = "Consulta Inválida!", Exception = exception });
            }
        }

        [HttpPost]
        public IActionResult ObtenerPreviewQuery([FromForm] string query)
        {
            try
            {
                var result = bPlantilla.ObtenerPreviewQuery(query);
                return Ok(new { Result = true, Msg = "¡ Tabla Obtenida correctamente !", Data = result });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                var exception = new { Message = ex.Message, StackTrace = ex.StackTrace };
                return BadRequest(new { ErrorMessage = "Consulta Inválida!", Exception = exception });
            }
        }

        [HttpPost]
        public IActionResult ProbarTemplateCorreoOld(string correo, string plantilla, List<string> queries)
        {
            try
            {
                long id_empresa = Convert.ToInt64(this.User.GetClaimValue("id_empresa"));

                Entities.Notificador.Credencial_Correo eCredencial_Correo = new Business.Notificador.Credencial_Correo(this._appCnx).GetConfig((int)id_empresa);

                if (eCredencial_Correo == null)
                {
                    return Ok(new { transaccion = false, mensaje = "¡ No se encuentra configurado las credenciales del servidor de correo !" });
                }

                CORREO eCORREO = new CORREO();
                eCORREO._HOST = eCredencial_Correo.host;
                eCORREO._PORT = eCredencial_Correo.puerto;
                eCORREO._USER = eCredencial_Correo.usuario;
                eCORREO._PASS = eCredencial_Correo.contrasena;

                string contenido = bPlantilla.ReemplazarValoresTemplate(plantilla, queries);

                bool envioExitoso = eCORREO.EnviarCorreoContenido(correo, contenido, "Prueba Plantilla Correo");

                return Ok(new { enviado = envioExitoso });
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ProbarTemplateCorreo(string correo, string plantilla, List<string> queries, List<ENO.Adjunto> adjuntos)
        {
            try
            {
                long id_empresa = 1; // Convert.ToInt64(this.User.GetClaimValue("id_empresa")); //Para multiempresa

                Entities.Notificador.Credencial_Correo eCredencial_Correo = new Business.Notificador.Credencial_Correo(this._appCnx).GetConfig((int)id_empresa);

                if (eCredencial_Correo == null)
                {
                    return Ok(new { transaccion = false, mensaje = "¡ No se encuentra configurado las credenciales del servidor de correo !" });
                }

                CORREO eCORREO = new CORREO();
                eCORREO._HOST = eCredencial_Correo.host;
                eCORREO._PORT = eCredencial_Correo.puerto;
                eCORREO._USER = eCredencial_Correo.usuario;
                eCORREO._PASS = eCredencial_Correo.contrasena;

                string contenido = bPlantilla.ReemplazarValoresTemplate(plantilla, queries);

                bool envioExitoso = eCORREO.EnviarConAdjuntos(correo, "Prueba Plantilla Correo", contenido, adjuntos);

                return Ok(new { enviado = envioExitoso });
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ProbarTemplateSMS(string numero, string plantilla, List<string> queries)
        {
            try
            {
                long id_empresa = 1;//Convert.ToInt64(this.User.GetClaimValue("id_empresa")); // Para multiempresa

                SMS sms = new SMS();

                string contenido = bPlantilla.ReemplazarValoresTemplate(plantilla, queries);

                new Thread(async () =>
                {
                    var x = await sms.enviarSMSPost(numero, contenido);
                }).Start();

                return Ok(new { enviado = true });
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return BadRequest(new { enviado = false, mensaje = ex.Message });
            }
        }
    }
}