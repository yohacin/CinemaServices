using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using EPU = Entities.Public;
using BPU = Business.Public;
using Newtonsoft.Json;
using wDualAssistence.Utility;
using System.Text.RegularExpressions;
using wDualAssistence.Helpers;

namespace wDualAssistence.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EncuestaController : ApiMainController
    {
        [HttpGet("{id_empleado}")]
        [BasicAuth]
        public string GetEncuesta(long id_empleado)
        {
            try
            {
                EPU.Encuesta_Config eEncuesta_Config = new BPU.Encuesta_Config(this._appCnx).GetEntity(1);
                if (eEncuesta_Config == null) eEncuesta_Config = new EPU.Encuesta_Config();

                EPU.Encuesta eEncuesta = new BPU.Encuesta(this._appCnx).GetEntityByEmpleado(id_empleado, DateTime.Now.Date);
                if (eEncuesta != null) eEncuesta_Config = new EPU.Encuesta_Config();

                return JsonConvert.SerializeObject(eEncuesta_Config);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpPost()]
        [BasicAuth]
        public string GuardarRespuesta([FromBody] string sjson)
        {
            EPU.RespuestaWS eRespuesta = new EPU.RespuestaWS();
            try
            {
                BPU.Encuesta bEncuesta = new BPU.Encuesta(this._appCnx);
                EPU.Encuesta eEncuesta = JsonConvert.DeserializeObject<EPU.Encuesta>(sjson);

                if (eEncuesta != null)
                {
                    eRespuesta.id = eEncuesta.id;
                    eRespuesta.codigo_erp = "0";
                    eRespuesta.descripcion = "OK";

                    eEncuesta.notificar = (eEncuesta.respuesta == "SI");
                    eEncuesta.fecha = DateTime.Now;
                    bEncuesta.Guardar(eEncuesta);
                    eRespuesta.jsonCompleto = "";

                    EPU.Encuesta_Config eEncuesta_Config = new BPU.Encuesta_Config(this._appCnx).GetEntity(1);

                    if (eEncuesta_Config.habilitado && eEncuesta.notificar)
                    {
                        Entities.Notificador.Grupo eGrupo = new Business.Notificador.Grupo(this._appCnx).Buscar(eEncuesta_Config.id_grupo);
                        if (eGrupo.oLstContactos != null)
                        {
                            Entities.Notificador.Credencial_Correo eCredencial_Correo = new Business.Notificador.Credencial_Correo(this._appCnx).GetConfig(1);

                            if (eCredencial_Correo == null)
                            {
                                throw new Exception("No se encuentra configurado las credenciales del servidor de correo !");
                            }
                            EPU.Empleado eEmpleado = new BPU.Empleado(this._appCnx).GetEntity(eEncuesta.id_empleado);
                            String template = new Business.Notificador.Plantilla(this._appCnx).Buscar(1).contenido;
                            String contenido = $"El empleado {eEmpleado.nombre + " " + eEmpleado.apellido_paterno + " " + eEmpleado.apellido_materno} respondio 'SI' a la pregunta: {eEncuesta.pregunta}.";

                            Regex regex = new Regex(@"{contenido}");
                            contenido = regex.Replace(template, contenido);
                            regex = new Regex(@"{titulo}");
                            contenido = regex.Replace(contenido, "Respuesta de Encuesta");

                            CORREO eCORREO = new CORREO();
                            eCORREO._HOST = eCredencial_Correo.host;
                            eCORREO._PORT = eCredencial_Correo.puerto;
                            eCORREO._USER = eCredencial_Correo.usuario;
                            eCORREO._PASS = eCredencial_Correo.contrasena;

                            foreach (var eContacto in eGrupo.oLstContactos)
                            {
                                eCORREO.EnviarCorreoContenido(eContacto.correo, contenido, "Notificacion de encuesta");
                            }
                        }
                    }
                }
                else
                {
                    eRespuesta.id = 0L;
                    eRespuesta.codigo_erp = "0";
                    eRespuesta.descripcion = "Error al guardar la encuesta";
                }
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                eRespuesta.id = 0L;
                eRespuesta.codigo_erp = "0";
                eRespuesta.descripcion = ex.Message;
            }

            return JsonConvert.SerializeObject(eRespuesta);
        }
    }
}