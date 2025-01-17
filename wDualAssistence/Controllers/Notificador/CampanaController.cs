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

#endregion Abreviaturas

namespace wDualAssistence.Controllers
{
    public class CampanaController : MainController
    {
        private BNO.Campana bCampana;
        private BNO.Grupo bGrupo;
        private BNO.Clasificador bClasificador;
        private BNO.Plantilla bPlantilla;
        private BNO.Contacto bContacto;
        private CampanaViewModel vCampana;

        public CampanaController()
        {
            this.bCampana = new BNO.Campana(this._appCnx);
            this.bGrupo = new BNO.Grupo(this._appCnx);
            this.bClasificador = new BNO.Clasificador(this._appCnx);
            this.bPlantilla = new BNO.Plantilla(this._appCnx);
            this.bContacto = new BNO.Contacto(this._appCnx);
        }

        public IActionResult Listado()
        {
            this.vCampana = new CampanaViewModel(this.User);
            if (!this._RevisarAcceso(vCampana.idUsuario, vCampana.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }

            vCampana.eLstCampana = bCampana.ListarByEmpresa(1);

            vCampana.menuItems.Add(new
            {
                id = "1",
                text = "Editar",
                iconCss = "fa fa-pencil"
            });
            vCampana.menuItems.Add(new
            {
                id = "2",
                text = "Eliminar",
                iconCss = "fa fa-remove"
            });
            return View(vCampana);
        }

        [HttpPost]
        public IActionResult Crear([FromForm] long idCampana)
        {
            try
            {
                this.vCampana = new CampanaViewModel(this.User);

                if (idCampana != 0) //Editar
                {
                    vCampana.eCampana = bCampana.Search(idCampana);
                }
                else
                {
                    vCampana.eCampana.id_empresa = 1;
                }

                vCampana.eLstGruposDisponibles = bGrupo.ListGruposAdministrables();

                vCampana.eLstTipoRepeticion = bClasificador.ListarByTipo(1);
                vCampana.eLstDiasRepeticion = bClasificador.ListarByTipo(2);

                vCampana.eLstPlantillasCorreo = bPlantilla.ListarByEmpresaTipo(vCampana.idEmpresa, ENO.TipoPlantilla.CORREO);
                vCampana.eLstPlantillasSMS = bPlantilla.ListarByEmpresaTipo(vCampana.idEmpresa, ENO.TipoPlantilla.SMS);

                return View(vCampana);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult Post(CampanaViewModel viewModel)
        {
            TryValidateModel(viewModel.eCampana);

            if (!ModelState.IsValid)
            {
                _log.Error(ModelState);
                foreach (var modelState in ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        _log.Error(error);
                        return BadRequest(new { Result = false, Msg = "¡ Ocurrió un error en la validación del Modelo!", ValidacionError = new { Error = error, ModelState = modelState } });
                        //return Json(new { Result = false, Msg = error.ErrorMessage, Error = error, ModelStateValues = ModelState });
                    }
                }
            }

            try
            {
                ViewModelBase.Accion accion;
                if (viewModel.eCampana.indefinido)
                {
                    viewModel.eCampana.fecha_final_ejecucion = viewModel.eCampana.fecha_inicial_ejecucion;
                }

                viewModel.eCampana.id_empresa = 1;
                if (viewModel.eCampana.codigo_campana != 0)
                {
                    bCampana.Modificar(viewModel.eCampana);
                    accion = ViewModelBase.Accion.Modificar;
                }
                else
                {
                    bCampana.Guardar(viewModel.eCampana);
                    accion = ViewModelBase.Accion.Guardar;
                }

                return Ok(new { Result = true, Msg = "¡ Registro realizado correctamente !" });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return BadRequest(new { Result = false, Msg = "¡ Ocurrió un error !", Exception = new { Message = ex.Message, StackTrace = ex.StackTrace } });
                //throw ex;
            }
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            try
            {
                this.vCampana = new CampanaViewModel(this.User);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                bCampana.Eliminar(id);

                return Ok(new { Result = true, Msg = "¡ Registro Eliminado Correctamente !" });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        public IActionResult GetFechaHoraServidor()
        {
            try
            {
                return Ok(new { Result = true, Msg = "¡ Fecha y Hora del Servidor obtenidos !", Data = DateTime.Now.ToString() });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                //throw ex;
                return BadRequest(new { Result = false, Msg = "¡ Error al obtener datos del servidor !", Excepcion = ex });
            }
        }
    }
}