using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wDualAssistence.Models;

using EPU = Entities.Public;
using BPU = Business.Public;
using System.IO;
using wDualAssistence.Helpers;

namespace wDualAssistence.Controllers
{
    [Authorize]
    public class MarcacionController : MainController
    {
        public IActionResult RegistroManual()
        {
            string vista = (Startup.modo_independiente) ? "ModoIndependiente/RegistroManual" : "RegistroManual";



            MarcacionViewModel vMarcacionViewModel = new MarcacionViewModel(this.User);
            if (!this._RevisarAcceso(vMarcacionViewModel.idUsuario, vMarcacionViewModel.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }

            vMarcacionViewModel.eMarcacion = new EPU.Marcacion();
            vMarcacionViewModel.eMarcacion.fecha_marcacion = DateTime.Now;
            vMarcacionViewModel.eMarcacion.tipo_marcacion_entrada = "1";
            vMarcacionViewModel.listaEmpleado = new BPU.Empleado(this._appCnx).GetLista();

            return View(vista, vMarcacionViewModel);
        }

        /// <summary>
        ///  Para la parte de carga de datos en base
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        public IActionResult Post(MarcacionViewModel vMarcacionViewModel)
        {
            try
            {
                EPU.Turno eTurno = new BPU.Turno(this._appCnx).GetEntityByEmpleado((int)vMarcacionViewModel.eMarcacion.codigo_turno, vMarcacionViewModel.eMarcacion.id_empleado);
                if (eTurno == null) throw new Exception("Debe seleccionar el turno!");
                if (vMarcacionViewModel.eMarcacion.id_empleado <= 0) throw new Exception("El campo empleado es requerido");
                if (vMarcacionViewModel.eMarcacion.id_empresa_entrada == null || vMarcacionViewModel.eMarcacion.id_empresa_entrada <= 0) throw new Exception("El campo empresa es requerido");

                long id_empleado = vMarcacionViewModel.eMarcacion.id_empleado;
                long codigo_turno = vMarcacionViewModel.eMarcacion.codigo_turno;
                EPU.Marcacion eMarcacion = new BPU.Marcacion(this._appCnx).GetEntityByTurno(id_empleado, codigo_turno, vMarcacionViewModel.eMarcacion.fecha_marcacion);

                if (eMarcacion == null) throw new Exception("No existe marcacion precargada. Empleado: " + id_empleado + "- turno: " + codigo_turno + "- Fecha:" + vMarcacionViewModel.eMarcacion.fecha_marcacion.ToString("dd/MM/yyyy"));

                string nombre_usuario = this.User.GetClaimValue("nombre_usuario");
                if (vMarcacionViewModel.eMarcacion.tipo_marcacion_entrada == "1")
                {
                    eMarcacion.tipo_marcacion_entrada = "MANUAL POR " + nombre_usuario;
                    eMarcacion.entrada_real = vMarcacionViewModel.eMarcacion.fecha_marcacion;
                    eMarcacion.id_empresa_entrada = vMarcacionViewModel.eMarcacion.id_empresa_entrada;
                }
                else
                {
                    eMarcacion.tipo_marcacion_salida = "MANUAL POR " + nombre_usuario;
                    eMarcacion.salida_real = vMarcacionViewModel.eMarcacion.fecha_marcacion;
                    eMarcacion.id_empresa_salida = vMarcacionViewModel.eMarcacion.id_empresa_entrada;
                }

                new BPU.Marcacion(this._appCnx).Modificar(eMarcacion);
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
        public IActionResult PostInserte(MarcacionViewModel vMarcacionViewModel)
        {
            try
            {
                BPU.Marcacion oMarcacion = new BPU.Marcacion(this._appCnx);

                if (vMarcacionViewModel.eMarcacion.id != 0)
                {
                    return Ok(new { transaccion = false, mensaje = "No se puede editar una marcación !" });
                }
                if (vMarcacionViewModel.eMarcacion.id_empresa_entrada <= 0) throw new Exception("Debe seleccionar la empresa !");

                EPU.Turno eTurno = new BPU.Turno(this._appCnx).GetEntityByEmpleado((int)vMarcacionViewModel.eMarcacion.codigo_turno, vMarcacionViewModel.eMarcacion.id_empleado);
                if (eTurno == null) throw new Exception("Debe seleccionar el turno!");

                DateTime dEntrada = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, eTurno.entrada.Hour, eTurno.entrada.Minute, eTurno.entrada.Second, DateTime.Now.Kind);
                DateTime dSalida = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, eTurno.salida.Hour, eTurno.salida.Minute, eTurno.salida.Second, DateTime.Now.Kind);

                vMarcacionViewModel.eMarcacion.entrada_programada = dEntrada;
                vMarcacionViewModel.eMarcacion.salida_programada = dSalida;
                vMarcacionViewModel.eMarcacion.salida_real = vMarcacionViewModel.eMarcacion.entrada_real;
                vMarcacionViewModel.eMarcacion.id_empresa_salida = vMarcacionViewModel.eMarcacion.id_empresa_entrada;

                if (vMarcacionViewModel.eMarcacion.tipo_marcacion_entrada == "1") // entrada
                {
                    vMarcacionViewModel.eMarcacion.tipo_marcacion_entrada = "MANUAL";
                    vMarcacionViewModel.eMarcacion.tipo_marcacion_salida = "";
                }
                else
                {// salida
                    vMarcacionViewModel.eMarcacion.tipo_marcacion_entrada = "";
                    vMarcacionViewModel.eMarcacion.tipo_marcacion_salida = "MANUAL";
                }

                oMarcacion.Guardar(vMarcacionViewModel.eMarcacion);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
            return Ok(new { transaccion = true });
        }

        [HttpGet("Marcacion/GetListaMarcaciones/{desde}/{hasta}")]
        public ActionResult GetListaMarcaciones(DateTime desde, DateTime hasta)
        {
            try
            {
                BPU.Marcacion oMarcacion = new BPU.Marcacion(this._appCnx);
                List<Entities.Helpers.MarcacionHelpers> listaMarcacionHelpers = oMarcacion.GetListaDetalle(desde, hasta);
                if (listaMarcacionHelpers != null)
                {
                    var Ids = listaMarcacionHelpers.Select(e => e.id).ToList();
                    var listaFotos = oMarcacion.GetListaXMarcaciones(Ids);
                    foreach (var itemMarcacion in listaMarcacionHelpers)
                    {
                        var Foto_Marcacion_Ent = listaFotos.FirstOrDefault(e => e.id_marcacion == itemMarcacion.id && e.posicion == 1);
                        string urlFotoEnt = Startup.appUrl + "assets/recursos/question-icon.png";
                        if (Foto_Marcacion_Ent != null)
                        {
                            string[] arrayPath = Foto_Marcacion_Ent.url_foto.Split("/");
                            urlFotoEnt = Startup.url_java + arrayPath[arrayPath.Length - 1];
                        }
                        itemMarcacion.imagen_ent = urlFotoEnt;

                        string urlFotoSal = Startup.appUrl + "/assets/recursos/question-icon.png";
                        var Foto_Marcacion_Sal = listaFotos.FirstOrDefault(e => e.id_marcacion == itemMarcacion.id && e.posicion == 2);
                        if (Foto_Marcacion_Sal != null)
                        {
                            string[] arrayPath = Foto_Marcacion_Sal.url_foto.Split("/");
                            urlFotoSal = Startup.url_java + arrayPath[arrayPath.Length - 1];
                        }
                        itemMarcacion.imagen_sal = urlFotoSal;
                    }
                }

                return Ok(new { transaccion = true, contenido = listaMarcacionHelpers });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
        }




        /// <summary>
        ///  Para la parte de carga de datos en base
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        public IActionResult GuardarModoIndependiente(MarcacionViewModel vMarcacionViewModel)
        {
            try
            {
                EPU.Turno eTurno = new BPU.Turno(this._appCnx).GetEntityByEmpleado((int)vMarcacionViewModel.eMarcacion.codigo_turno, vMarcacionViewModel.eMarcacion.id_empleado);
                if (eTurno == null) throw new Exception("Debe seleccionar el turno!");
                if (vMarcacionViewModel.eMarcacion.id_empleado <= 0) throw new Exception("El campo funcionario es requerido");
                if (vMarcacionViewModel.eMarcacion.id_empresa_entrada == null || vMarcacionViewModel.eMarcacion.id_empresa_entrada <= 0) throw new Exception("El campo ubicación es requerido");

                long id_empleado = vMarcacionViewModel.eMarcacion.id_empleado;
                long codigo_turno = vMarcacionViewModel.eMarcacion.codigo_turno;
                EPU.Marcacion eMarcacion = new BPU.Marcacion(this._appCnx).GetEntityByTurno(id_empleado, codigo_turno, vMarcacionViewModel.eMarcacion.fecha_marcacion);

                if (eMarcacion == null) throw new Exception("No existe marcacion precargada. Funcionario: " + id_empleado + "- Turno: " + codigo_turno + "- Fecha:" + vMarcacionViewModel.eMarcacion.fecha_marcacion.ToString("dd/MM/yyyy"));

                string nombre_usuario = this.User.GetClaimValue("nombre_usuario");
                if (vMarcacionViewModel.eMarcacion.tipo_marcacion_entrada == "1")
                {
                    eMarcacion.tipo_marcacion_entrada = "MANUAL POR " + nombre_usuario;
                    eMarcacion.entrada_real = vMarcacionViewModel.eMarcacion.fecha_marcacion;
                    eMarcacion.id_empresa_entrada = vMarcacionViewModel.eMarcacion.id_empresa_entrada;
                }
                else
                {
                    eMarcacion.tipo_marcacion_salida = "MANUAL POR " + nombre_usuario;
                    eMarcacion.salida_real = vMarcacionViewModel.eMarcacion.fecha_marcacion;
                    eMarcacion.id_empresa_salida = vMarcacionViewModel.eMarcacion.id_empresa_entrada;
                }

                new BPU.Marcacion(this._appCnx).Modificar(eMarcacion);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
            return Ok(new { transaccion = true });
        }
    }
}