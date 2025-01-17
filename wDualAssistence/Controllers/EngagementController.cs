using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using EPU = Entities.Public;
using ESE = Entities.Security;
using BPU = Business.Public;
using BSE = Business.Security;
using BNO = Business.Notificador;
using wDualAssistence.Models;
using Syncfusion.EJ2.Navigations;
using Newtonsoft.Json;
using wDualAssistence.Helpers;
using wDualAssistence.Utility;
using System.Text.RegularExpressions;
using Syncfusion.XlsIO;
using System.Data;
using Syncfusion.Drawing;
using System.IO;
using wDualAssistence.Utility.Controllers;

namespace wDualAssistence.Controllers
{
    [Authorize]
    public class EngagementController : MainController
    {
        public IActionResult Listado()
        {
            EngagementViewModel vEngagementViewModel = new EngagementViewModel(this.User);
            if (!this._RevisarAcceso(vEngagementViewModel.idUsuario, vEngagementViewModel.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }

            new BPU.Engagement(this._appCnx).CambiarEstadoEjecucion(); // cambiar estado de engagement que su fecha de inicio es menor a la fecha actual

            if (vEngagementViewModel.tipo == 1)
                vEngagementViewModel.listaEngagement = new BPU.Engagement(this._appCnx).GetListaByFiltro(1, 0);
            else
                vEngagementViewModel.listaEngagement = new BPU.Engagement(this._appCnx).GetListaByFiltro(1, vEngagementViewModel.idUsuario);

            vEngagementViewModel.listaEmpresa = new BPU.Empresa(this._appCnx).GetLista();

            vEngagementViewModel.idsEmpresas = vEngagementViewModel.listaEmpresa.Select(e => e.id).ToList();

            //Cargo Items para el Menu
            vEngagementViewModel.menuItems.Add(new
            {
                id = "1",
                text = "Asociacion Empleado",
                iconCss = "fa fa-cogs"
            });
            vEngagementViewModel.menuItems.Add(new
            {
                id = "2",
                text = "Editar",
                iconCss = "fa fa-pencil"
            });
            vEngagementViewModel.menuItems.Add(new
            {
                id = "3",
                text = "LOG",
                iconCss = "fa fa-pencil"
            });
            vEngagementViewModel.menuItems.Add(new
            {
                id = "4",
                text = "Eliminar",
                iconCss = "fa fa-remove"
            });

            return View(vEngagementViewModel);
        }

        [HttpPost]
        public IActionResult Log([FromForm] long id_engagement)
        {
            try
            {
                EngagementViewModel vEngagementViewModel = new EngagementViewModel(this.User);
                vEngagementViewModel.listaLog = new BPU.Engagement_Modificacion_Solicitud(this._appCnx).GetListaByEngagement(id_engagement);

                if (id_engagement != 0)
                {
                    vEngagementViewModel.eEngagement = new BPU.Engagement(this._appCnx).GetEntity(id_engagement);
                }
                else
                {
                    vEngagementViewModel.eEngagement = new EPU.Engagement();
                }

                return View(vEngagementViewModel);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult Crear([FromForm] long id_engagement)
        {
            try
            {
                EngagementViewModel vEngagementViewModel = new EngagementViewModel(this.User);
                vEngagementViewModel.listaEmpleado = new Business.Public.Empleado(this._appCnx).GetListaSinTurno();
                vEngagementViewModel.listaCategoriaTarea = new BPU.Tarea(this._appCnx).GetListaCategoriaTarea();
                vEngagementViewModel.listaCargo = new BPU.Cargo_Engagement(this._appCnx).GetLista();
                vEngagementViewModel.listaArea = new BPU.Area_Engagement(this._appCnx).GetLista();
                vEngagementViewModel.listaEmpresa = new BPU.Empresa(this._appCnx).GetLista();
                vEngagementViewModel.listaCategoria_Tarea = new BPU.Categoria_Tarea(this._appCnx).GetLista();
                vEngagementViewModel.eTarea = new EPU.Tarea();
                vEngagementViewModel.eSolicitudPendiente = new EPU.Engagement_Modificacion_Solicitud();
                //EPU.Empresa eEmpresa = new BPU.Empresa().GetEntity(id_empresa);
                //vEngagementViewModel.nombre_empresa = (eEmpresa == null) ? "NO EXISTE LA EMPRESA": eEmpresa.nombre;

                if (id_engagement != 0)
                {
                    vEngagementViewModel.eEngagement = new BPU.Engagement(this._appCnx).GetEntity(id_engagement);
                    if (vEngagementViewModel.eEngagement.estado_ejecucion == 1)
                    {
                        if (DateTime.Now > vEngagementViewModel.eEngagement.desde)
                        {
                            vEngagementViewModel.eEngagement.estado_ejecucion = 2;
                            new BPU.Engagement(this._appCnx).Modificar(vEngagementViewModel.eEngagement);
                        }
                    }
                    vEngagementViewModel.eSolicitudPendiente = new BPU.Engagement_Modificacion_Solicitud(this._appCnx).GetEntityPendienteByEngagement(id_engagement);
                    if (vEngagementViewModel.eSolicitudPendiente == null) vEngagementViewModel.eSolicitudPendiente = new EPU.Engagement_Modificacion_Solicitud();
                }
                else
                {
                    vEngagementViewModel.eEngagement = new EPU.Engagement();
                    vEngagementViewModel.eEngagement.estado_ejecucion = 1;
                    vEngagementViewModel.eEngagement.desde = DateTime.Now;
                    vEngagementViewModel.eEngagement.hasta = DateTime.Now.AddDays(1);
                    vEngagementViewModel.eEngagement.facturable = true;
                    vEngagementViewModel.eEngagement.listaDetalle_Hora_Engagement = new List<EPU.Detalle_Hora_Engagement>();
                    vEngagementViewModel.eEngagement.id_usuario = vEngagementViewModel.idUsuario;
                }

                ViewBag.positionData = new string[] { "Scrollable", "Popup" };
                ViewBag.orientationData = new string[] { "Top", "Bottom", "Left", "Right" };
                ViewBag.headerText1 = new TabHeader { Text = "Datos Generales", IconCss = "fa fa-file" };
                ViewBag.headerText2 = new TabHeader { Text = "Detalles de Horas", IconCss = "fa fa-clock-o" };
                ViewBag.headerText3 = new TabHeader { Text = "Tareas", IconCss = "fa fa-tasks" };
                ViewBag.headerText4 = new TabHeader { Text = "Alertas", IconCss = "fa fa-bell" };
                ViewBag.data = new string[] { "Bolivianos", "Dolares" };

                ViewBag.listaTipo = new string[] { "Horas Trabajo", "Horas Adicionales (Overrun)" };

                List<Valor> listaEstado = new List<Valor>();
                listaEstado.Add(new Valor { id = 1, nombre = "PLANIFICADO" });
                listaEstado.Add(new Valor { id = 2, nombre = "EN PROCESO" });
                listaEstado.Add(new Valor { id = 3, nombre = "FINALIZADO" });
                ViewBag.listaEstado = listaEstado;

                vEngagementViewModel.menuItems.Add(new
                {
                    id = "1",
                    text = "Expandir Todo",
                    iconCss = "fa fa-expand"
                });
                vEngagementViewModel.menuItems.Add(new
                {
                    id = "2",
                    text = "Colapsar Todo",
                    iconCss = "fa fa-compress"
                });

                return View(vEngagementViewModel);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpPost]
        public IActionResult AsociacionCalendar([FromForm] long id_engagement)
        {
            try
            {
                EngagementViewModel vEngagementViewModel = new EngagementViewModel(this.User);
                vEngagementViewModel.eEngagement = new BPU.Engagement(this._appCnx).GetEntityByHorasDisponibles(id_engagement);
                if (vEngagementViewModel.eEngagement == null) throw new Exception("No existe un engagement con codigo:" + id_engagement);

                vEngagementViewModel.listaEngagement_Empleado = new BPU.EngagementEmpleado(this._appCnx).GetListaEngagementEmpleado(id_engagement);
                vEngagementViewModel.nombre_empresa = new BPU.Empresa(this._appCnx).GetEntityNombre(vEngagementViewModel.eEngagement.id_empresa);
                vEngagementViewModel.listaEmpleado = new BPU.Empleado(this._appCnx).GetListaSinTurno();
                vEngagementViewModel.listaEngagement = new BPU.Engagement(this._appCnx).GetListaByEmpleadoTipo(vEngagementViewModel.idUsuario);

                List<string> groupedArea = vEngagementViewModel.listaEmpleado
                  .GroupBy(u => u.area)
                  .Select(grp => grp.Key)
                  .ToList();

                ViewBag.listaAreas = groupedArea;

                ViewBag.appointments = GetScheduleData();

                return View(vEngagementViewModel);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Helpers.AppointmentData> GetScheduleData()
        {
            List<Entities.Helpers.AppointmentData> appData = new List<Entities.Helpers.AppointmentData>();
            //appData.Add(new AppointmentData
            //{
            //    Subject = "-  4 Hrs disponibles",
            //    StartTime = new DateTime(2020, 9, 5, 0, 0, 0),
            //    EndTime = new DateTime(2020, 9, 5, 0, 0, 0),
            //    IsReadonly = true,
            //    FullDay = true,
            //});
            return appData;
        }

        public class Valor
        {
            public int id { get; set; }
            public string nombre { get; set; }
        }

        [HttpPost]
        public IActionResult AsociacionEmpleado([FromForm] long id_engagement, long id_empresa)
        {
            try
            {
                EngagementViewModel vEngagementViewModel = new EngagementViewModel(this.User);
                if (id_engagement == 0) throw new Exception("No existe un engagement con codigo:" + id_engagement);

                vEngagementViewModel.listaEngagement_Empleado = new BPU.EngagementEmpleado(this._appCnx).GetListaEngagementEmpleado(id_engagement);
                vEngagementViewModel.listaEmpleado = new BPU.Empleado(this._appCnx).GetListaAsignacion(id_empresa);
                vEngagementViewModel.eEngagement = new BPU.Engagement(this._appCnx).GetEntityByHorasDisponibles(id_engagement);
                vEngagementViewModel.nombre_empresa = new BPU.Empresa(this._appCnx).GetEntityNombre(id_empresa);

                List<string> groupedArea = vEngagementViewModel.listaEmpleado
              .GroupBy(u => u.area)
              .Select(grp => grp.Key)
              .ToList();

                ViewBag.listaAreas = groupedArea;

                //Cargo Items para el Menu
                vEngagementViewModel.menuItems.Add(new
                {
                    id = "1",
                    text = "Asignar Horas",
                    iconCss = "fa fa-list"
                });
                vEngagementViewModel.menuItems.Add(new
                {
                    id = "2",
                    text = "Detalle Horas",
                    iconCss = "fa fa-list"
                });
                vEngagementViewModel.menuItems.Add(new
                {
                    id = "3",
                    text = "Eliminar",
                    iconCss = "fa fa-remove"
                });

                return View(vEngagementViewModel);
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
        public IActionResult Post(EngagementViewModel vEngagementViewModel)
        {
            try
            {
                vEngagementViewModel.eEngagement.listaAlerta_Engagement = JsonConvert.DeserializeObject<List<EPU.Alerta_Engagement>>(vEngagementViewModel.stringListaAlertas);
                vEngagementViewModel.eEngagement.listaDetalle_Hora_Engagement = JsonConvert.DeserializeObject<List<EPU.Detalle_Hora_Engagement>>(vEngagementViewModel.stringListaDetalleHoras);
                vEngagementViewModel.eEngagement.listaResponsable_Engagement = JsonConvert.DeserializeObject<List<EPU.Responsable_Engagement>>(vEngagementViewModel.stringListaResponsables);
                vEngagementViewModel.eEngagement.listaTarea_Engagement = JsonConvert.DeserializeObject<List<EPU.Tarea_Engagement>>(vEngagementViewModel.stringListaTareas);

                if (vEngagementViewModel.eEngagement.id_empresa <= 0) return Ok(new { transaccion = false, mensaje = "Debe seleccionar la empresa !", tab = 0 });
                if (vEngagementViewModel.eEngagement.titulo == null) return Ok(new { transaccion = false, mensaje = "El campo titulo es requerido !", tab = 0 });
                if (vEngagementViewModel.eEngagement.id_area <= 0) return Ok(new { transaccion = false, mensaje = "El campo area es requerido !", tab = 0 });
                if (vEngagementViewModel.eEngagement.listaResponsable_Engagement == null) return Ok(new { transaccion = false, mensaje = "Debe asignar la lista de responsables en la pestaña 'DATOS GENERALES' !", tab = 0 });
                if (vEngagementViewModel.eEngagement.listaResponsable_Engagement.Count() == 0) return Ok(new { transaccion = false, mensaje = "Debe asignar la lista de responsables en la pestaña 'DATOS GENERALES' !", tab = 0 });
                if (vEngagementViewModel.eEngagement.listaDetalle_Hora_Engagement == null) return Ok(new { transaccion = false, mensaje = "Debe asignar la lista detalles de hora en la pestaña 'DETALLES DE HORAS'!", tab = 1 });
                if (vEngagementViewModel.eEngagement.listaTarea_Engagement == null) return Ok(new { transaccion = false, mensaje = "Debe asignar la lista de tareas en la pestaña 'TAREAS' !", tab = 2 });

                double iTotalHorasDetalle = vEngagementViewModel.eEngagement.listaDetalle_Hora_Engagement.Where(d => d.tipo == 1).Sum(d => d.cantidad);
                double iTotalHorasTareas = vEngagementViewModel.eEngagement.listaTarea_Engagement.Sum(d => d.maximo_horas);
                if (iTotalHorasTareas > iTotalHorasDetalle)
                    return Ok(new { transaccion = false, mensaje = "La sumatoria de HORAS POR TAREA (" + iTotalHorasTareas + ") no debe ser mayor al DETALLE DE HORAS del engagement( " + iTotalHorasDetalle + ") !", tab = 2 });

                if (vEngagementViewModel.eEngagement.id == 0)
                {
                    new BPU.Engagement(this._appCnx).Guardar(vEngagementViewModel.eEngagement);
                }
                else
                {
                    int tipo = Convert.ToInt32(this.User.GetClaimValue("tipo"));
                    if (tipo == 2)
                    { //validar si es el usuario que lo creo
                        new BPU.Engagement(this._appCnx).Modificar(vEngagementViewModel.eEngagement);

                        EPU.Engagement_Modificacion_Solicitud eSolicitud = new EPU.Engagement_Modificacion_Solicitud();
                        eSolicitud.fecha_solicita = DateTime.Now;
                        eSolicitud.id_engagement = vEngagementViewModel.eEngagement.id;
                        eSolicitud.id_usuario_solicita = Convert.ToInt64(this.User.GetClaimValue("idUsuario"));
                        eSolicitud.json_modificado = JsonConvert.SerializeObject(vEngagementViewModel.eEngagement);
                        eSolicitud.estado_solicitud = 1;
                        eSolicitud.fecha_autoriza = eSolicitud.fecha_solicita;
                        eSolicitud.id_usuario_autoriza = eSolicitud.id_usuario_solicita;
                        eSolicitud.json_anterior = eSolicitud.json_modificado;
                        new BPU.Engagement_Modificacion_Solicitud(this._appCnx).Guardar(eSolicitud);

                        List<EPU.Empleado> responsablesEngagement = new BPU.Engagement(this._appCnx).GetResponsablesEngagement(eSolicitud.id_engagement);
                        EPU.Empleado solicitante = new BPU.Empleado(this._appCnx).GetEntity(eSolicitud.id_usuario_solicita);
                        String template = new BNO.Plantilla(this._appCnx).Buscar(1).contenido;
                        String contenido = $"El usuario {solicitante.nombre} realizó la solicitud de cambios para el engagement {vEngagementViewModel.eEngagement.titulo}. Y Fue aprobado por el usuario: {solicitante.nombre}";

                        Regex regex = new Regex(@"{contenido}");
                        contenido = regex.Replace(template, contenido);
                        regex = new Regex(@"{titulo}");
                        contenido = regex.Replace(contenido, "Solicitud modificación de engagement");

                        Entities.Notificador.Credencial_Correo eCredencial_Correo = new Business.Notificador.Credencial_Correo(this._appCnx).GetConfig(1);

                        if (eCredencial_Correo == null)
                        {
                            return Ok(new { transaccion = false, mensaje = "¡ No se encuentra configurado las credenciales del servidor de correo !" });
                        }

                        CORREO eCORREO = new CORREO();
                        eCORREO._HOST = eCredencial_Correo.host;
                        eCORREO._PORT = eCredencial_Correo.puerto;
                        eCORREO._USER = eCredencial_Correo.usuario;
                        eCORREO._PASS = eCredencial_Correo.contrasena;

                        foreach (EPU.Empleado responsable in responsablesEngagement)
                        {
                            eCORREO.EnviarCorreoContenido(responsable.correo, contenido, "Solicitud modificación de engagement");
                        }
                    }
                    else
                    {
                        EPU.Engagement_Modificacion_Solicitud eSolicitud = new EPU.Engagement_Modificacion_Solicitud();
                        eSolicitud.fecha_solicita = DateTime.Now;
                        eSolicitud.id_engagement = vEngagementViewModel.eEngagement.id;
                        eSolicitud.id_usuario_solicita = Convert.ToInt64(this.User.GetClaimValue("idUsuario"));
                        eSolicitud.json_modificado = JsonConvert.SerializeObject(vEngagementViewModel.eEngagement);

                        new BPU.Engagement_Modificacion_Solicitud(this._appCnx).Guardar(eSolicitud);

                        List<EPU.Empleado> responsablesEngagement = new BPU.Engagement(this._appCnx).GetResponsablesEngagement(eSolicitud.id_engagement);
                        ESE.Usuario solicitante = new BSE.Usuario(this._appCnx).GetEntity(eSolicitud.id_usuario_solicita);
                        String template = new BNO.Plantilla(this._appCnx).Buscar(1).contenido;
                        String contenido = $"El usuario {solicitante.nombre} realizó la solicitud de cambios para el engagement {vEngagementViewModel.eEngagement.titulo}.";

                        Regex regex = new Regex(@"{contenido}");
                        contenido = regex.Replace(template, contenido);
                        regex = new Regex(@"{titulo}");
                        contenido = regex.Replace(contenido, "Solicitud modificación de engagement");

                        Entities.Notificador.Credencial_Correo eCredencial_Correo = new Business.Notificador.Credencial_Correo(this._appCnx).GetConfig(1);

                        if (eCredencial_Correo == null)
                        {
                            return Ok(new { transaccion = false, mensaje = "¡ No se encuentra configurado las credenciales del servidor de correo !" });
                        }

                        CORREO eCORREO = new CORREO();
                        eCORREO._HOST = eCredencial_Correo.host;
                        eCORREO._PORT = eCredencial_Correo.puerto;
                        eCORREO._USER = eCredencial_Correo.usuario;
                        eCORREO._PASS = eCredencial_Correo.contrasena;

                        foreach (EPU.Empleado responsable in responsablesEngagement)
                        {
                            eCORREO.EnviarCorreoContenido(responsable.correo, contenido, "Solicitud modificación de engagement");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message, tab = -1 });
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
                var listaDetalle = new BPU.Empleado_Hoja_Trabajo(this._appCnx).GetListaByEngagement(id);
                if (listaDetalle.Count() <= 0)
                {
                    new BPU.Engagement(this._appCnx).Eliminar(id);
                    return Ok(new { transaccion = true });
                }
                else
                {
                    return Ok(new { transaccion = false, mensaje = "El engagement ya tiene registrado horas de trabajo por el empleado !" });
                }
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
        }

        [HttpGet("Engagement/GetListaEmpleadoEngagement/{id_engagement}")]
        public ActionResult GetListaEmpleadoEngagement(long id_engagement)
        {
            try
            {
                var eEngagement = new BPU.Engagement(this._appCnx).GetEntityByHorasDisponibles(id_engagement);
                if (eEngagement == null) throw new Exception("No existe un engagement con codigo:" + id_engagement);

                var listaEngagement_Empleado = new BPU.EngagementEmpleado(this._appCnx).GetListaEngagementEmpleado(id_engagement);
                var nombre_empresa = new BPU.Empresa(this._appCnx).GetEntityNombre(eEngagement.id_empresa);

                return Ok(new { transaccion = true, contenido = listaEngagement_Empleado });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
        }

        [HttpGet("Engagement/GetListaByFiltro/{id_tipo}")]
        public ActionResult GetListaByFiltro(int id_tipo)
        {
            try
            {
                ViewModelBase vViewModelBase = new ViewModelBase(this.User);
                List<EPU.Engagement> listaEngagement = new List<EPU.Engagement>();
                if (vViewModelBase.tipo == 1)
                    listaEngagement = new BPU.Engagement(this._appCnx).GetListaByFiltro(id_tipo, 0);
                else
                    listaEngagement = new BPU.Engagement(this._appCnx).GetListaByFiltro(id_tipo, vViewModelBase.idUsuario);

                return Ok(new { transaccion = true, contenido = listaEngagement });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
        }

        [HttpGet("Engagement/GetListaByEmpresa/{id_empresa}/{fecha}")]
        public List<EPU.Engagement> GetListaByEmpresa(long id_empresa, DateTime fecha)
        {
            try
            {
                long id_empleado = Convert.ToInt64(this.User.GetClaimValue("idUsuario"));
                return new BPU.Engagement(this._appCnx).GetListaByEmpresaSemana(id_empresa, id_empleado, fecha);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpGet("Engagement/GetListaAvanceEngagement/{desde}/{hasta}")]
        public ActionResult GetListaAvanceEngagement(DateTime desde, DateTime hasta)
        {
            try
            {
                List<Entities.Helpers.AvanceEngagementHelper> listaAvance = new Business.Public.Engagement(this._appCnx).AvanceEngagement(desde, hasta);

                return Ok(new { transaccion = true, contenido = listaAvance });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
        }

        [HttpGet("Engagement/GetCalendarioByEmpresa/{id_engagement}/{id_empleado}")]
        public ActionResult GetCalendarioByEmpresa(long id_engagement, long id_empleado)
        {
            try
            {
                List<Entities.Helpers.AppointmentData> appData = new BPU.EngagementEmpleado(this._appCnx).GetListaAsignadasByEmpleado(id_empleado, id_engagement);

                return Ok(new { transaccion = true, contenido = appData });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
        }

        [HttpGet("Engagement/RptEngagementLogEXCEL/{id_engagement}")]
        public ActionResult RptEngagementLogEXCEL(long id_engagement)
        {
            //Configuracion Hoja Excel
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2016;

            IWorkbook workbook = application.Workbooks.Create(1);
            IWorksheet sheetProspecto = workbook.Worksheets[0];
            sheetProspecto.Name = "permanencias";

            #region Table data

            List<Entities.Public.Engagement_Modificacion_Solicitud> listaLog = new BPU.Engagement_Modificacion_Solicitud(this._appCnx).GetListaByEngagement(id_engagement);

            DataTable table = new DataTable();

            table.Columns.Add("Fecha de solicitud", typeof(DateTime));
            table.Columns.Add("Empleado solicitante", typeof(string));
            table.Columns.Add("Fecha autorización", typeof(DateTime));
            table.Columns.Add("Empleado autorizador", typeof(string));

            foreach (var item in listaLog)
            {
                table.Rows.Add(new object[] {
                    item.fecha_solicita,
                    item.nombre_solicitante,
                    item.fecha_autoriza,
                    item.nombre_autorizador
                });
            }

            if (table.Rows.Count < 1)
            {
                sheetProspecto.Range["D5:I5"].Merge();
                sheetProspecto.Range["D5:I5"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                sheetProspecto.Range["D5:I5"].CellStyle.Font.Bold = true;
                sheetProspecto.Range["D5:I5"].Text = "NO EXISTEN DATOS";
            }
            else
            {
                sheetProspecto.ImportDataTable(table, true, 6, 1, true);

                IListObject tableProspectos = sheetProspecto.ListObjects.Create("table_solicitudes", sheetProspecto.UsedRange);

                //Formato para numeros
                IStyle style = workbook.Styles.Add("CurrencyFormat");
                style.NumberFormat = "$* #,##0";

                string formatoColumna = "L2:L";

                sheetProspecto[formatoColumna + table.Rows.Count + 2].CellStyleName = "CurrencyFormat";

                //Estilos de la Hoja
                ITableStyles tableStyles = workbook.TableStyles;
                ITableStyle tableStyle = tableStyles.Add("Table Style 1");
                ITableStyleElements tableStyleElements = tableStyle.TableStyleElements;
                ITableStyleElement tableStyleElement = tableStyleElements.Add(ExcelTableStyleElementType.SecondColumnStripe);
                tableStyleElement.BackColorRGB = Color.FromArgb(217, 225, 242);

                ITableStyleElement tableStyleElement1 = tableStyleElements.Add(ExcelTableStyleElementType.FirstColumn);
                tableStyleElement1.FontColorRGB = Color.FromArgb(128, 128, 128);

                ITableStyleElement tableStyleElement2 = tableStyleElements.Add(ExcelTableStyleElementType.HeaderRow);
                tableStyleElement2.FontColor = ExcelKnownColors.White;
                tableStyleElement2.BackColorRGB = Color.FromArgb(0, 112, 192);

                ITableStyleElement tableStyleElement3 = tableStyleElements.Add(ExcelTableStyleElementType.TotalRow);
                tableStyleElement3.BackColorRGB = Color.FromArgb(0, 112, 192);
                tableStyleElement3.FontColor = ExcelKnownColors.White;

                tableProspectos.TableStyleName = tableStyle.Name;

                sheetProspecto.UsedRange.AutofitColumns();
            }

            sheetProspecto.Range["B2:H2"].Merge();
            sheetProspecto.Range["B2:H2"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
            sheetProspecto.Range["B2:H2"].CellStyle.Font.Bold = true;
            sheetProspecto.Range["B2:H2"].CellStyle.Font.Size = 20;
            sheetProspecto.Range["B2:H2"].Text = "LOG de Modificaciones";

            sheetProspecto.Range["A3"].Text = "Fecha Consulta";
            sheetProspecto.Range["A3:A4"].CellStyle.Font.Bold = true;
            sheetProspecto.Range["B3"].Text = DateTime.Now.ToString("dd/MM/yyy");

            #endregion Table data

            try
            {
                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                string nombreReporte = "rptEngagementLog.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                throw new Exception(e.Message);
            }
        }

        [HttpGet("Engagement/ValidarUsoTarea/{id_engagement}/{id_tarea}")]
        public ActionResult ValidarUsoTarea(long id_engagement, long id_tarea)
        {
            try
            {
                if (id_engagement == 0) return Ok(new { transaccion = false });

                bool transaccion = new BPU.Engagement(this._appCnx).TareaEnUso(id_engagement, id_tarea);
                return Ok(new { transaccion = transaccion });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpGet("Engagement/GetListaByEmpresa/{id_empresa}")]
        public ActionResult GetListaByEmpresa(long id_empresa)
        {
            try
            {
                if (id_empresa == 0) throw new Exception("El campo id_empresa debe ser mayor a 0");

                var lista = new BPU.Engagement(this._appCnx).GetListaByEmpresa(id_empresa);
                return Ok(new { transaccion = true, dataLista = lista });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
            }
        }

        //------------------------------------Notificar avance del engagement
        [HttpGet("NotificarAvance")]
        public bool NotificarAvance()
        {
            return new EngagementManager(this._appCnx, _log).NotificarAvance();
            //try
            //{
            //    //SincronizarByServicio();
            //    _log.Info("VERIFICANDO Si existena notificacion de avance de engagement");
            //    List<Entities.Helpers.NotificarAlertaHelper> listaNotificar = new BPU.Engagement(this._appCnx).GetNotificarAlertaHelper();

            //    if (listaNotificar == null) return false;
            //    foreach (var itemNotificar in listaNotificar)
            //    {
            //        Entities.Notificador.Plantilla ePlantilla = new Business.Notificador.Plantilla(this._appCnx).Buscar(Startup.id_plantilla_alerta);
            //        if (itemNotificar == null) continue;
            //        if (ePlantilla == null) throw new Exception("La plantilla para notificar alerta de avance de engagement no existe!");
            //        string template = ePlantilla.contenido;
            //        template = template.Replace("{nombre_empresa}", itemNotificar.nombre_empresa);
            //        template = template.Replace("{nombre_engagement}", itemNotificar.nombre_engagement);
            //        template = template.Replace("{notificar}", itemNotificar.notificar + "");
            //        template = template.Replace("{porcentaje_avance}", itemNotificar.porcentaje_avance + "");
            //        foreach (var eContacto in itemNotificar.listaContacto)
            //        {
            //            enviarAlerta(eContacto.correo, template);
            //        }
            //        new BPU.Engagement(this._appCnx).CambiarEstadoEjecutado(itemNotificar.id_alerta);
            //    }
            //}
            //catch (Exception e)
            //{
            //    enviarAlerta("billy.crespo@dualbiz.net", "<h1>ERROR NOTIFICAR ALERTAS DE AVANCE EN ENGAGEMENT</h1>" + e.Message + "*****************" + e.ToString());
            //}
            //return false;
        }

        //private void enviarAlerta(string correo, string contenido)
        //{
        //    try
        //    {
        //        Entities.Notificador.Credencial_Correo eCredencial_Correo = new Business.Notificador.Credencial_Correo(this._appCnx).GetConfig(1);
        //        CORREO eCORREO = new CORREO();
        //        eCORREO._HOST = eCredencial_Correo.host;
        //        eCORREO._PORT = eCredencial_Correo.puerto;
        //        eCORREO._USER = eCredencial_Correo.usuario;
        //        eCORREO._PASS = eCredencial_Correo.contrasena;

        //        eCORREO.EnviarCorreoContenido(correo, contenido, "NOTIFICADOR");
        //    }
        //    catch (Exception e)
        //    {
        //        return;
        //    }
        //}
    }
}