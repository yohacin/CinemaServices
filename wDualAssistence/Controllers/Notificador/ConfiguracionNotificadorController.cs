using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using wDualAssistence.Models.Notificador;

using ENO = Entities.Notificador;
using BNO = Business.Notificador;

using Business.Notificador;

using wDualAssistence.Utility;
using wDualAssistence.Models;

namespace wDualAssistence.Controllers.Notificador
{
    public class ConfiguracionNotificadorController : MainController
    {
        private BNO.Credencial_Correo bCredencial_Correo;

        private ConfiguracionNotificadorViewModel vConfiguracion;

        public ConfiguracionNotificadorController()
        {
            this.bCredencial_Correo = new BNO.Credencial_Correo(this._appCnx);
        }

        public IActionResult Index()
        {
            this.vConfiguracion = new ConfiguracionNotificadorViewModel(this.User);
            if (!this._RevisarAcceso(vConfiguracion.idUsuario, vConfiguracion.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }
            this.vConfiguracion.eCredencial_Correo = this.bCredencial_Correo.GetConfig(1) ?? new ENO.Credencial_Correo();
            return View(vConfiguracion);
        }

        [HttpPost]
        public IActionResult Post(ConfiguracionNotificadorViewModel vConfigViewModel)
        {
            try
            {
                _log.Info("Guardando Cambios de Configuracion SINCRONZACION y CREDENCIALES CORREO");
                this.vConfiguracion = new ConfiguracionNotificadorViewModel(this.User);

                // credenciales correo
                //BNO.Credencial_Correo oCredencial_Correo = new BNO.Credencial_Correo();
                const int idMainter = 1;
                vConfigViewModel.eCredencial_Correo.id_empresa = idMainter;

                if (vConfigViewModel.eCredencial_Correo.id == 0)
                {
                    bCredencial_Correo.CrearConfig(vConfigViewModel.eCredencial_Correo);
                }
                else
                {
                    bCredencial_Correo.ModificarConfig(vConfigViewModel.eCredencial_Correo);
                }

                _log.Info("Cambios de Configuracion Guardado CORRECTAMENTE");

                return Ok(new { transaccion = true });
            }
            catch (Exception ex)
            {
                _log.Error("Cambios de Configuracion ERROR:" + ex.Message);
                return Ok(new { transaccion = false });
            }
        }

        [HttpGet]
        public IActionResult ProbarCorreo(string enviar_correo, string host, int puerto, string usuario, string contrasena)
        {
            try
            {
                CORREO eCORREO = new CORREO();
                eCORREO._HOST = host;
                eCORREO._PORT = puerto;
                eCORREO._USER = usuario;
                eCORREO._PASS = contrasena;

                return Ok(new { transaccion = eCORREO.EnviarCorreoPrueba(enviar_correo) });
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
        }
    }
}