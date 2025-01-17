using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using EPU = Entities.Public;
using BPU = Business.Public;
using wDualAssistence.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace wDualAssistence.Controllers
{
    [Authorize]
    public class EngagementEmpleadoController : MainController
    {
        [HttpPost]
        public IActionResult Crear([FromForm] long id_engagement_empleado)
        {
            try
            {
                EngagementEmpleadoViewModel vEngagementEmpleadoViewModel = new EngagementEmpleadoViewModel(this.User);
                var eEmpleado = new Business.Public.Empleado(this._appCnx).GetEntity(1);
                vEngagementEmpleadoViewModel.nombre_empleado = "NOMBRE EMPLEADO";
                if (id_engagement_empleado != 0)
                {
                    vEngagementEmpleadoViewModel.eEngagement_Empleado = new BPU.EngagementEmpleado(this._appCnx).GetEntity(id_engagement_empleado);
                    vEngagementEmpleadoViewModel.listaDetalle_Engagement_Empleado = new BPU.EngagementEmpleado(this._appCnx).GetListaDetalle_Engagement_Empleado(id_engagement_empleado);
                    vEngagementEmpleadoViewModel.listaEmpleado = new BPU.EngagementEmpleado(this._appCnx).GetListaEmpleadoByEngagement(vEngagementEmpleadoViewModel.eEngagement_Empleado.id_engagement);

                    vEngagementEmpleadoViewModel.eEngagement = new BPU.Engagement(this._appCnx).GetEntityByEngagementEmpleado(vEngagementEmpleadoViewModel.eEngagement_Empleado.id_engagement);
                    vEngagementEmpleadoViewModel.nombre_empresa = new BPU.Empresa(this._appCnx).GetEntityNombre(vEngagementEmpleadoViewModel.eEngagement.id_empresa);
                    vEngagementEmpleadoViewModel.horas_disponibles = new BPU.Engagement(this._appCnx).HorasDisponiblesByEngagement(vEngagementEmpleadoViewModel.eEngagement_Empleado.id_engagement, vEngagementEmpleadoViewModel.eEngagement_Empleado.id_empleado);
                }
                else
                {
                    vEngagementEmpleadoViewModel.listaDetalle_Engagement_Empleado = new List<EPU.Detalle_Engagement_Empleado>();
                }

                return View(vEngagementEmpleadoViewModel);
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
        public IActionResult Post(long id_engagement, long id_empleado, string stringListaDetalle)
        {
            try
            {
                var format = "dd/MM/yyyy"; // your datetime format
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

                List<EPU.Detalle_Engagement_Empleado> listaDetalle = JsonConvert.DeserializeObject<List<EPU.Detalle_Engagement_Empleado>>(stringListaDetalle, dateTimeConverter);
                if (listaDetalle == null) throw new Exception("Lista del detalle de horas vacia!");

                EPU.Engagement_Empleado eEngagement_Empleado = new BPU.EngagementEmpleado(this._appCnx).GetEntity(id_engagement, id_empleado);

                if (eEngagement_Empleado != null)
                {
                    double horas_disponibles = new BPU.Engagement(this._appCnx).HorasDisponiblesByEngagement(eEngagement_Empleado.id_engagement, eEngagement_Empleado.id_empleado);
                    double horas_aumentadas = listaDetalle.Sum(dtl => dtl.cantidad_horas);
                    if (horas_disponibles - horas_aumentadas < 0) throw new Exception("El numero de horas asignadas (" + horas_aumentadas + ") no debe ser mayor al numero de horas disponibles(" + horas_disponibles + ")");

                    new BPU.EngagementEmpleado(this._appCnx).GuardarLista(listaDetalle, eEngagement_Empleado.id);

                    var listaEmpleado = new BPU.EngagementEmpleado(this._appCnx).GetListaEmpleadoByEngagement(id_engagement);
                    horas_disponibles = new BPU.Engagement(this._appCnx).HorasDisponiblesByEngagement(id_engagement, id_empleado);

                    return Ok(new { transaccion = true, listaEmpleado = listaEmpleado, horas_disponibles = horas_disponibles });
                }

                return Ok(new { transaccion = false, mensaje = "El parametro id_empleado es incorrecto:" + id_empleado });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult PostEmpleado(long id_engagement, string stringListaEmpleado)
        {
            try
            {
                List<EPU.Empleado> listaDetalle = JsonConvert.DeserializeObject<List<EPU.Empleado>>(stringListaEmpleado);

                if (listaDetalle == null) throw new Exception("Lista de empleados seleccionados vacia!");

                if (id_engagement != 0)
                {
                    List<EPU.Engagement_Empleado> listaEngagement_Empleado = new List<EPU.Engagement_Empleado>();
                    foreach (var eEmpleado in listaDetalle)
                    {
                        EPU.Engagement_Empleado eEngagement_Empleado = new EPU.Engagement_Empleado();
                        eEngagement_Empleado.fecha_asignacion = DateTime.Now;
                        eEngagement_Empleado.id_empleado = eEmpleado.id;
                        eEngagement_Empleado.id_engagement = id_engagement;
                        listaEngagement_Empleado.Add(eEngagement_Empleado);
                    }

                    new BPU.EngagementEmpleado(this._appCnx).GuardarLista(listaEngagement_Empleado);
                    return Ok(new { transaccion = true });
                }

                return Ok(new { transaccion = false, mensaje = "El parametro id_engagement_empleado es incorrecto:" + id_engagement });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
        }

        //-------------------------------------------Para la pantalla Propuesta del calendario

        [HttpGet("Engagement/AsociarEmpleado/{id_engagement}/{id_empleado}")]
        public ActionResult AsociarEmpleado(long id_engagement, long id_empleado)
        {
            try
            {
                if (id_engagement != 0)
                {
                    EPU.Engagement_Empleado eEngagement_Empleado = new EPU.Engagement_Empleado();
                    eEngagement_Empleado.fecha_asignacion = DateTime.Now;
                    eEngagement_Empleado.id_empleado = id_empleado;
                    eEngagement_Empleado.id_engagement = id_engagement;

                    new BPU.EngagementEmpleado(this._appCnx).Guardar(eEngagement_Empleado);
                    return Ok(new { transaccion = true });
                }

                return Ok(new { transaccion = false, mensaje = "El parametro id_engagement es incorrecto:" + id_engagement });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
        }

        [HttpGet("Engagement/AsignarHoras/{id_detalle}/{id_engagement}/{id_empleado}/{fecha}/{horas}")]
        public ActionResult AsignarHoras(long id_detalle, long id_engagement, long id_empleado, DateTime fecha, double horas)
        {
            try
            {
                if (id_engagement != 0)
                {
                    BPU.Engagement bEngagement = new BPU.Engagement(this._appCnx);
                    EPU.Engagement eEngagement = bEngagement.GetEntity(id_engagement);

                    double horas_disponibles = bEngagement.HorasDisponiblesByFecha(id_empleado, fecha);
                    double horas_aumentadas = horas;
                    double horas_ejecutadas = bEngagement.HorasEjecutadasByFecha(id_empleado, fecha);
                    bool validDateEngagement = eEngagement.desde <= fecha && fecha <= eEngagement.hasta;

                    double totalHoras = eEngagement.listaDetalle_Hora_Engagement.Sum(e => e.cantidad);
                    double totalHorasAsignadas = bEngagement.HorasAsignadas(id_engagement);

                    // total horas eg asignasa + por asignar <= totalhoras

                    if ((totalHorasAsignadas + horas) > totalHoras)
                    {
                        throw new Exception("El numero total de horas por asignar (" + (totalHorasAsignadas + horas_aumentadas) + ") no debe ser mayor al total de horas asignadas para el engagement (" + totalHoras + ")");
                    }

                    if (id_detalle == 0 && horas_disponibles - horas_aumentadas < 0)
                    {
                        throw new Exception("El numero de horas asignadas (" + horas_aumentadas + ") no debe ser mayor al numero de horas disponibles(" + horas_disponibles + ")");
                    }

                    if (horas_aumentadas < horas_ejecutadas)
                    {
                        throw new Exception("El numero de horas asignadas (" + horas_aumentadas + ") no debe ser menor al numero de horas ejecutadas(" + horas_ejecutadas + ")");
                    }

                    if (!validDateEngagement)
                    {
                        throw new Exception("La asignación de horas esta fuera de las fechas establecidas para el engagement: desde " + eEngagement.desde.ToString("dd/MM/yyyy") + ", hasta: " + eEngagement.hasta.ToString("dd/MM/yyyy"));
                    }

                    if (id_detalle == 0)
                    {
                        EPU.Detalle_Engagement_Empleado eDetalleAux = new BPU.EngagementEmpleado(this._appCnx).GetEntityDetalle(id_engagement, id_empleado, fecha);
                        if (eDetalleAux != null) throw new Exception("Ya existe una asignación de horas para este empleado en este engagement");
                    }

                    EPU.Detalle_Engagement_Empleado eDetalle_Engagement_Empleado = new EPU.Detalle_Engagement_Empleado();
                    eDetalle_Engagement_Empleado.id = id_detalle;
                    eDetalle_Engagement_Empleado.fecha_inicio = fecha;
                    eDetalle_Engagement_Empleado.fecha_fin = fecha;
                    eDetalle_Engagement_Empleado.cantidad_horas = horas;

                    new BPU.EngagementEmpleado(this._appCnx).GuardarAsignacion(eDetalle_Engagement_Empleado, id_engagement, id_empleado);
                    return Ok(new { transaccion = true });
                }

                return Ok(new { transaccion = false, mensaje = "El parametro id_engagement es incorrecto:" + id_engagement });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
        }

        [HttpGet("Engagement/EliminarAsignacionHoras/{id_detalle}/{id_empleado}/{fecha}")]
        public ActionResult EliminarAsignacionHoras(long id_detalle, long id_empleado, DateTime fecha)
        {
            try
            {
                if (id_detalle != 0)
                {
                    BPU.Engagement bEngagement = new BPU.Engagement(this._appCnx);
                    double horas_ejecutadas = bEngagement.HorasEjecutadasByFecha(id_empleado, fecha);
                    if (horas_ejecutadas > 0)
                    {
                        throw new Exception("No se puede quitar la asignación porque el empleado esta ejecutando sus tareas");
                    }

                    new BPU.EngagementEmpleado(this._appCnx).EliminarAsignacionHoras(id_detalle);
                    return Ok(new { transaccion = true });
                }

                return Ok(new { transaccion = false, mensaje = "El parametro id_detalle es incorrecto:" + id_detalle });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
        }

        [HttpGet("Engagement/EliminarAsociacionEmpleado/{id_engagement}/{id_empleado}")]
        public ActionResult EliminarAsociacionEmpleado(long id_engagement, long id_empleado)
        {
            try
            {
                if (id_engagement != 0 || id_empleado != 0)
                {
                    BPU.Engagement bEngagement = new BPU.Engagement(this._appCnx);
                    List<EPU.Detalle_Engagement_Empleado> detallesEngagement = bEngagement.DetallesEngagementEmpleadoBy(id_engagement, id_empleado);

                    if (detallesEngagement.Count > 0)
                    {
                        throw new Exception("No se puede quitar la asignación porque el empleado tiene horas asignadas");
                    }

                    new BPU.EngagementEmpleado(this._appCnx).EliminarAsignacionEmpleado(id_engagement, id_empleado);
                    return Ok(new { transaccion = true });
                }

                return Ok(new { transaccion = false, mensaje = "Parametros incorrectos engagement:" + id_engagement + ", empleado: " + id_empleado });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
        }

        [HttpGet("Engagement/GetHorasDisponibles/{id_empleado}/{fecha}")]
        public ActionResult GetHorasDisponibles(long id_empleado, DateTime fecha)
        {
            try
            {
                double horas_disponibles = new BPU.Engagement(this._appCnx).HorasDisponiblesByFecha(id_empleado, fecha);
                return Ok(new { transaccion = true, horas_disponibles = horas_disponibles });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
        }

        [HttpGet("Engagement/GetHorasTurno/{id_empleado}/{fecha}")]
        public ActionResult GetHorasTurno(long id_empleado, DateTime fecha)
        {
            try
            {
                double horas_turno = new BPU.Engagement(this._appCnx).HorasTurnoByFecha(id_empleado, fecha);
                return Ok(new { transaccion = true, horas_turno = horas_turno });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
        }

        //-------------------------------------------FIN Para la pantalla propuesta del calendario
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
                EPU.Engagement_Empleado eEngagement_Empleado = new BPU.EngagementEmpleado(this._appCnx).GetEntity(id);
                bool bTieneTareasCargadas = new BPU.EngagementEmpleado(this._appCnx).TieneTareasCargadas(eEngagement_Empleado.id_empleado, eEngagement_Empleado.id_engagement);
                if (bTieneTareasCargadas)
                    return Ok(new { transaccion = false, mensaje = "No se puede eliminar asociación, debido a que el empleado ya tiene tareas cargadas para este engagement !" });

                new BPU.EngagementEmpleado(this._appCnx).Eliminar(id);
                return Ok(new { transaccion = true });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
        }

        [HttpGet("EngagementEmpleado/GetDetalleByEmpleado/{id_empleado}/{id_engagement}")]
        public IActionResult GetDetalleByEmpleado(long id_empleado, long id_engagement)
        {
            try
            {
                var listaDetalle_Engagement_Empleado = new BPU.EngagementEmpleado(this._appCnx).GetEntityDetalle(id_engagement, id_empleado);

                double horas_disponibles = new BPU.Engagement(this._appCnx).HorasDisponiblesByEngagement(id_engagement, id_empleado);

                return Ok(new { transaccion = true, detalle = listaDetalle_Engagement_Empleado, horas_disponibles = horas_disponibles });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
        }
    }
}