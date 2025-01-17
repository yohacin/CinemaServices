using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using wDualAssistence.Models;
using BPU = Business.Public;
using EHE = Entities.Helpers;

namespace wDualAssistence.Controllers
{
    public class DashboardController : MainController
    {
        public IActionResult DashboardPrincipal()
        {
            DashboardPrincipalViewModel vDashboardPrincipalViewModel = new DashboardPrincipalViewModel(this.User);
            if (vDashboardPrincipalViewModel.tipo == 2)
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }

            List<EHE.ViewDashboardReport> reportItems = new BPU.DashboardPrincipal(this._appCnx).GetLista();

            vDashboardPrincipalViewModel.dashboardReport = new EHE.HighchartsReportData();
            vDashboardPrincipalViewModel.dashboardReport.cargarGraficoDashboard(reportItems);

            vDashboardPrincipalViewModel.listaEmpresas = new BPU.Empresa(this._appCnx).GetLista();
            vDashboardPrincipalViewModel.idsEmpresas = vDashboardPrincipalViewModel.listaEmpresas.Select(e => e.id).ToList();

            return View(vDashboardPrincipalViewModel);
        }

        public IActionResult DashboardPrincipalEmpleado()
        {
            DashboardPrincipalViewModel vDashboardPrincipalViewModel = new DashboardPrincipalViewModel(this.User);
            if (vDashboardPrincipalViewModel.tipo == 2)
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }
            vDashboardPrincipalViewModel.listaEmpresas = new BPU.Empresa(this._appCnx).GetLista();
            vDashboardPrincipalViewModel.idsEmpresas = vDashboardPrincipalViewModel.listaEmpresas.Select(e => e.id).ToList();
            List<EHE.ViewDashboardReport> reportItems = new BPU.DashboardPrincipal(this._appCnx).GetListaFiltrada(vDashboardPrincipalViewModel.idsEmpresas,
                                                                                                desde: DateTime.Now.AddMonths(-1), hasta: DateTime.Now);

            vDashboardPrincipalViewModel.dashboardReport = new EHE.HighchartsReportData();
            vDashboardPrincipalViewModel.dashboardReport.cargarGraficoDashboardEmpleado(reportItems);

            return View(vDashboardPrincipalViewModel);
        }

        public IActionResult DashboardTareasEmpleado()
        {
            DashboardTareasEmpleadoViewModel vDashboardTareasEmpleado = new DashboardTareasEmpleadoViewModel(this.User);
            if (vDashboardTareasEmpleado.tipo == 1)
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }
            vDashboardTareasEmpleado.listaEngagement = new BPU.Engagement(this._appCnx).GetListaByEmpleado(vDashboardTareasEmpleado.idUsuario);
            ViewBag.appointments = GetScheduleData();
            return View(vDashboardTareasEmpleado);
        }

        public IActionResult DashboardPrincipalMarcacionEmpleado()
        {
            DashboardPrincipalViewModel vDashboardPrincipalViewModel = new DashboardPrincipalViewModel(this.User);
            if (vDashboardPrincipalViewModel.tipo == 2)
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }
            vDashboardPrincipalViewModel.listaEmpresas = new BPU.Empresa(this._appCnx).GetLista();
            vDashboardPrincipalViewModel.idsEmpresas = vDashboardPrincipalViewModel.listaEmpresas.Select(e => e.id).ToList();

            List<EHE.ViewDashboardMarcacionEmpleadoReport> reportItems = new BPU.DashboardPrincipal(this._appCnx).GetListaMarcacionEmpleados(vDashboardPrincipalViewModel.idsEmpresas, DateTime.Now.AddMonths(-1).Date, DateTime.Now.Date);

            vDashboardPrincipalViewModel.dashboardReport = new EHE.HighchartsReportData();
            vDashboardPrincipalViewModel.dashboardReport.cargarGraficoDashboardMarcacionEmpleado(reportItems);

            string vista = "DashboardPrincipalMarcacionEmpleado";

            if (Startup.modo_independiente)
            {
                vDashboardPrincipalViewModel.dashboardReport.nombreGrafico = "Marcación de funcionarios";
                vista = "ModoIndependiente/DashboardPrincipalMarcacionEmpleado";
            }

            return View(vista, vDashboardPrincipalViewModel);
        }

        [HttpPost("Dashboard/GetListaDashboardReportMarcacionEmpleado/{desde}/{hasta}")]
        public ActionResult GetListaDashboardReportMarcacionEmpleado([FromBody] List<long> idsEmpresas, DateTime desde, DateTime hasta)
        {
            try
            {
                if (idsEmpresas == null)
                {
                    idsEmpresas = new List<long>();
                }
                EHE.HighchartsReportData reportData = new EHE.HighchartsReportData();
                reportData.cargarGraficoDashboardMarcacionEmpleado(new BPU.DashboardPrincipal(this._appCnx).GetListaMarcacionEmpleados(idsEmpresas, desde, hasta));
                if (Startup.modo_independiente)
                {
                    reportData.nombreGrafico = "Marcación de funcionarios";
                }

                return Ok(new { transaccion = true, contenido = reportData });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
        }

        public List<AppointmentData> GetScheduleData()
        {
            List<AppointmentData> appData = new List<AppointmentData>();
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

        public class AppointmentData
        {
            public string Subject { get; set; }
            public long IdEngagement { get; set; }
            public long IdDetalleEngagementEmpleado { get; set; }
            public string Engagement { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public double HorasAsignadas { get; set; }
            public bool IsReadonly { get; set; }
            public bool FullDay { get; set; }
            public string PrimaryColor { get; set; }
        }

        [HttpGet("Dashboard/GetListaDashboardReport/{empresas}/{desde}/{hasta}")]
        public ActionResult GetListaDashboardReport(String empresas, DateTime desde, DateTime hasta)
        {
            try
            {
                List<long> ids = new List<long>();
                if (empresas != "0")
                {
                    ids = new List<long>(Array.ConvertAll(empresas.Split('-'), Convert.ToInt64));
                }
                EHE.HighchartsReportData reportData = new EHE.HighchartsReportData();
                reportData.cargarGraficoDashboard(new BPU.DashboardPrincipal(this._appCnx).GetListaFiltrada(ids, desde, hasta));

                return Ok(new { transaccion = true, contenido = reportData });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
        }

        [HttpGet("Dashboard/GetListaDashboardReportEmpleado/{empresas}/{desde}/{hasta}")]
        public ActionResult GetListaDashboardReportEmpleado(String empresas, DateTime desde, DateTime hasta)
        {
            try
            {
                List<long> ids = new List<long>();
                if (empresas != "0")
                {
                    ids = new List<long>(Array.ConvertAll(empresas.Split('-'), Convert.ToInt64));
                }
                EHE.HighchartsReportData reportData = new EHE.HighchartsReportData();
                reportData.cargarGraficoDashboardEmpleado(new BPU.DashboardPrincipal(this._appCnx).GetListaFiltrada(ids, desde, hasta));

                return Ok(new { transaccion = true, contenido = reportData });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
        }
    }
}