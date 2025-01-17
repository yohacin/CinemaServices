using Entities.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Syncfusion.Drawing;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using wDualAssistence.Helpers;
using wDualAssistence.Models;
using BPU = Business.Public;

using EPU = Entities.Public;

using EHE = Entities.Helpers;

namespace wDualAssistence.Controllers
{
    public class HojaTrabajoController : MainController
    {
        [Authorize]
        public IActionResult Listado()
        {
            HojaTrabajoViewModel vHojaTrabajoViewModel = new HojaTrabajoViewModel(this.User);
            if (!this._RevisarAcceso(vHojaTrabajoViewModel.idUsuario, vHojaTrabajoViewModel.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }

            long id_empleado = vHojaTrabajoViewModel.idUsuario;

            vHojaTrabajoViewModel.listaEmpresa = new Business.Public.Empresa(this._appCnx).GetListaByEngagementEmpleado(id_empleado);
            //vHojaTrabajoViewModel.listaEngagement = new Business.Public.Engagement().GetListaByEmpleado(id_empleado);

            //Cargo Items para el Menu
            vHojaTrabajoViewModel.menuItems.Add(new
            {
                id = "1",
                text = "Eliminar",
                iconCss = "fa fa-remove"
            });

            return View(vHojaTrabajoViewModel);
        }

        public IActionResult Listado2()
        {
            HojaTrabajoViewModel vHojaTrabajoViewModel = new HojaTrabajoViewModel(this.User);

            long id_empleado = vHojaTrabajoViewModel.idUsuario;

            vHojaTrabajoViewModel.listaEmpresa = new Business.Public.Empresa(this._appCnx).GetListaByEmpleado(id_empleado);
            //vHojaTrabajoViewModel.listaEngagement = new Business.Public.Engagement().GetListaByEmpleado(id_empleado);

            //Cargo Items para el Menu
            vHojaTrabajoViewModel.menuItems.Add(new
            {
                id = "1",
                text = "Eliminar",
                iconCss = "fa fa-remove"
            });

            return View(vHojaTrabajoViewModel);
        }

        [HttpPost]
        public IActionResult Post(string stringHojaTrabajo)
        {
            try
            {
                var format = "dd/MM/yyyy"; // your datetime format
                var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

                List<Entities.Helpers.HojaTrabajoHelper> listaHojaTrabajo = JsonConvert.DeserializeObject<List<Entities.Helpers.HojaTrabajoHelper>>(stringHojaTrabajo, dateTimeConverter);

                string sMensaje = "";
                if (listaHojaTrabajo != null)
                {
                    long id_empleado = Convert.ToInt64(this.User.GetClaimValue("idUsuario"));
                    BPU.Empleado_Hoja_Trabajo bEmpleadoHojaTrabajo = new BPU.Empleado_Hoja_Trabajo(this._appCnx);
                    string validacionHojaTrabajo = bEmpleadoHojaTrabajo.ValidarLista(listaHojaTrabajo, id_empleado);

                    if (validacionHojaTrabajo.Trim().Length > 0)
                    {
                        return Ok(new { transaccion = false, mensaje = validacionHojaTrabajo });
                    }

                    foreach (var eHojaTrabajoHelper in listaHojaTrabajo)
                    {
                        sMensaje += new BPU.Empleado_Hoja_Trabajo(this._appCnx).ModificarLista(eHojaTrabajoHelper, id_empleado);
                        eHojaTrabajoHelper.total = eHojaTrabajoHelper.lunes + eHojaTrabajoHelper.martes + eHojaTrabajoHelper.miercoles + eHojaTrabajoHelper.jueves + eHojaTrabajoHelper.viernes + eHojaTrabajoHelper.sabado + eHojaTrabajoHelper.domingo;
                    }
                }

                if (sMensaje == "")
                {
                    return Ok(new { transaccion = true });
                }
                else
                {
                    return Ok(new { transaccion = false, mensaje = sMensaje });
                }
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, mensaje = ex.Message });
                throw ex;
            }
        }

        [HttpGet]
        public IActionResult Save(Entities.Helpers.HojaTrabajoHelper eHojaTrabajoHelper)
        {
            try
            {
                if (eHojaTrabajoHelper != null)
                {
                    long id_empleado = Convert.ToInt64(this.User.GetClaimValue("idUsuario"));
                    eHojaTrabajoHelper.total = eHojaTrabajoHelper.lunes + eHojaTrabajoHelper.martes + eHojaTrabajoHelper.miercoles + eHojaTrabajoHelper.jueves + eHojaTrabajoHelper.viernes + eHojaTrabajoHelper.sabado + eHojaTrabajoHelper.domingo;

                    string sMensaje = new BPU.Empleado_Hoja_Trabajo(this._appCnx).ModificarLista(eHojaTrabajoHelper, id_empleado);

                    //return Ok(new { transaccion = true });
                    if (sMensaje == "")
                    {
                        return Ok(new { transaccion = true, obj = eHojaTrabajoHelper });
                    }
                    else
                    {
                        return Ok(new { transaccion = false, obj = eHojaTrabajoHelper, mensaje = sMensaje });
                    }
                }

                return Ok(new { transaccion = true, obj = eHojaTrabajoHelper });

                //return eHojaTrabajoHelper;
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                return Ok(new { transaccion = false, data = eHojaTrabajoHelper, mensaje = ex.Message });
                throw ex;
            }
        }

        [HttpGet("HojaTrabajo/GetListaBySemana/{fecha}")]
        public List<Entities.Helpers.HojaTrabajoHelper> GetListaBySemana(DateTime fecha)
        {
            try
            {
                long id_empleado = Convert.ToInt64(this.User.GetClaimValue("idUsuario"));
                return new BPU.Empleado_Hoja_Trabajo(this._appCnx).GetListaBySemana(fecha, id_empleado);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpGet("HojaTrabajo/ExisteDetalle/{fecha}/{id_engagement}/{id_tarea}")]
        public bool ExisteDetalle(DateTime fecha, long id_engagement, long id_tarea)
        {
            try
            {
                long id_empleado = Convert.ToInt64(this.User.GetClaimValue("idUsuario"));
                EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_TrabajoRepetido = new BPU.Empleado_Hoja_Trabajo(this._appCnx).GetEntityByFilter(id_engagement, id_tarea, id_empleado, fecha);

                return (eEmpleado_Hoja_TrabajoRepetido != null);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("HojaTrabajo/Delete/{id_engagement}/{id_tarea}/{fecha}")]
        public IActionResult Delete(long id_engagement, long id_tarea, DateTime fecha)
        {
            try
            {
                long id_empleado = Convert.ToInt64(this.User.GetClaimValue("idUsuario"));
                new BPU.Empleado_Hoja_Trabajo(this._appCnx).EliminarBySemana(id_engagement, id_tarea, id_empleado, fecha);
                return Ok(new { transaccion = true });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpGet("HojaTrabajo/RptListaEXCEL")]
        public ActionResult RptMarcacionesEXCEL()
        {
            ExcelEngine excelEngine = new ExcelEngine();

            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Excel2013;

            //A existing workbook is opened.
            string basePath = Startup.appPath + @"\wwwroot\assets\template\rptHojaTrabajo.xlsx";
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);
            //Access first worksheet from the workbook.
            IWorksheet worksheet = workbook.Worksheets["marcaciones"];
            workbook.Worksheets.Remove(worksheet);

            worksheet = workbook.Worksheets.Create("marcaciones");

            #region Table data

            List<Entities.Public.Empleado_Hoja_Trabajo> listaHojaTrabajo = new Business.Public.Empleado_Hoja_Trabajo(this._appCnx).GetLista();
            listaHojaTrabajo.Add(new EPU.Empleado_Hoja_Trabajo() { fecha = DateTime.Now, id_empleado = 10, id_engagement = 10, hora = 10 });
            DataTable table = new DataTable();

            table.Columns.Add("Fecha", typeof(DateTime));
            table.Columns.Add("id Empleado", typeof(long));
            table.Columns.Add("id Engagement", typeof(long));
            table.Columns.Add("Hora", typeof(Double));

            foreach (var itemDetalle in listaHojaTrabajo)
            {
                table.Rows.Add(new object[] {
                    itemDetalle.fecha,
                    itemDetalle.id_empleado,
                    itemDetalle.id_engagement,
                    itemDetalle.hora
                    });
            }

            if (table.Rows.Count < 1)
            {
                worksheet.Range["D5:I5"].Merge();
                worksheet.Range["D5:I5"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range["D5:I5"].CellStyle.Font.Bold = true;
                worksheet.Range["D5:I5"].Text = "NO EXISTEN DATOS";
            }
            else
            {
                worksheet.ImportDataTable(table, true, 6, 1, true);

                IListObject tableProspectos = worksheet.ListObjects.Create("table_hoja_trabajo", worksheet.UsedRange);

                worksheet.UsedRange.AutofitColumns();
            }

            worksheet.Range["B2:H2"].Merge();
            worksheet.Range["B2:H2"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
            worksheet.Range["B2:H2"].CellStyle.Font.Bold = true;
            worksheet.Range["B2:H2"].CellStyle.Font.Size = 20;
            worksheet.Range["B2:H2"].Text = "MARCACIONES";

            worksheet.Range["A3"].Text = "Fecha Inicio";
            worksheet.Range["A4"].Text = "Fecha Fin";
            worksheet.Range["A3:A4"].CellStyle.Font.Bold = true;
            worksheet.Range["B3"].Text = "dd/MM/yyy";
            worksheet.Range["B4"].Text = "dd/MM/yyy";

            #endregion Table data

            workbook.Version = ExcelVersion.Excel2013;
            MemoryStream ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;

            //Closing and Disponse los recursos ocupados.
            workbook.Close();
            excelEngine.Dispose();
            sampleFile.Close();
            sampleFile.Dispose();
            try
            {
                string nombreReporte = "rptHojaTrabajo.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                throw new Exception(e.Message);
            }
        }

        [HttpGet("HojaTrabajo/RptListaEXCEL2")]
        public ActionResult RptMarcacionesEXCEL2()
        {
            //Configuracion Hoja Excel
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2016;

            IWorkbook workbook = application.Workbooks.Create(1);
            IWorksheet sheetProspecto = workbook.Worksheets[0];
            sheetProspecto.Name = "marcaciones";

            #region Table data

            List<Entities.Public.Empleado_Hoja_Trabajo> listaHojaTrabajo = new Business.Public.Empleado_Hoja_Trabajo(this._appCnx).GetLista();

            DataTable table = new DataTable();

            table.Columns.Add("Fecha", typeof(DateTime));
            table.Columns.Add("id Empleado", typeof(long));
            table.Columns.Add("id Engagement", typeof(long));
            table.Columns.Add("Hora", typeof(Double));

            foreach (var itemDetalle in listaHojaTrabajo)
            {
                table.Rows.Add(new object[] {
                    itemDetalle.fecha,
                    itemDetalle.id_empleado,
                    itemDetalle.id_engagement,
                    itemDetalle.hora
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

                IListObject tableProspectos = sheetProspecto.ListObjects.Create("table_hoja_trabajo", sheetProspecto.UsedRange);

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
            sheetProspecto.Range["B2:H2"].Text = "MARCACIONES";

            sheetProspecto.Range["A3"].Text = "Fecha Inicio";
            sheetProspecto.Range["A4"].Text = "Fecha Fin";
            sheetProspecto.Range["A3:A4"].CellStyle.Font.Bold = true;
            sheetProspecto.Range["B3"].Text = "dd/MM/yyy";
            sheetProspecto.Range["B4"].Text = "dd/MM/yyy";

            #endregion Table data

            try
            {
                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                string nombreReporte = "rptHojaTrabajo.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                throw new Exception(e.Message);
            }
        }

        [HttpGet("Reporte/RptAsignacion")]
        public ActionResult RptAsignacion()
        {
            string nombre_hoja = "asignacion";
            ExcelEngine excelEngine = new ExcelEngine();

            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Excel2013;

            //A existing workbook is opened.
            string basePath = Startup.appPath + @"\wwwroot\assets\template\RptAsignacion.xlsx";
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);
            try
            {
                //Access first worksheet from the workbook.
                IWorksheet worksheet = workbook.Worksheets[nombre_hoja];
                workbook.Worksheets.Remove(worksheet);
                worksheet = workbook.Worksheets.Create(nombre_hoja);

                #region Table data

                List<Entities.Helpers.AsignacionEmpleadoHelper> listaAsignacion = new Business.Public.Empleado_Hoja_Trabajo(this._appCnx).GetListaAsignacionEmpleado();
                DataTable table = new DataTable();

                table.Columns.Add("EMPRESA", typeof(string));
                table.Columns.Add("ENGAGEMENT", typeof(string));
                table.Columns.Add("FECHA INICIO", typeof(DateTime));
                table.Columns.Add("FECHA FIN", typeof(DateTime));
                table.Columns.Add("FACTURABLE", typeof(string));
                table.Columns.Add("ESTIMACION TOTAL HORAS", typeof(double));
                table.Columns.Add("HORAS OVERRUN", typeof(double));
                table.Columns.Add("HORAS ASIGNADAS", typeof(double));
                table.Columns.Add("EMPLEADO", typeof(string));
                table.Columns.Add("HORAS EJECUTADAS", typeof(double));

                foreach (var itemDetalle in listaAsignacion)
                {
                    table.Rows.Add(new object[] {
                        itemDetalle.nombre_empresa,
                        itemDetalle.nombre_engagement,
                        itemDetalle.fecha_inicio,
                        itemDetalle.fecha_fin,
                        itemDetalle.facturable,
                        itemDetalle.estimacion_total_horas,
                        itemDetalle.estimacion_overrun,
                        itemDetalle.horas_asignadas,
                        itemDetalle.nombre_empelado,
                        itemDetalle.horas_ejecutadas,
                        });
                }

                if (table.Rows.Count < 1)
                {
                    worksheet.Range["D5:I5"].Merge();
                    worksheet.Range["D5:I5"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range["D5:I5"].CellStyle.Font.Bold = true;
                    worksheet.Range["D5:I5"].Text = "NO EXISTEN DATOS";
                }
                else
                {
                    worksheet.ImportDataTable(table, true, 6, 1, true);

                    worksheet.ListObjects.Create("table_asignacion", worksheet.UsedRange);

                    worksheet.UsedRange.AutofitColumns();
                }

                worksheet.Range["B2:H2"].Merge();
                worksheet.Range["B2:H2"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range["B2:H2"].CellStyle.Font.Bold = true;
                worksheet.Range["B2:H2"].CellStyle.Font.Size = 20;
                worksheet.Range["B2:H2"].Text = "SEGUIMIENTO DE ENGAGEMENTS";

                worksheet.Range["A3"].Text = "GENERADO POR:";
                worksheet.Range["A4"].Text = "FECHA REPORTE:";
                worksheet.Range["A3:A4"].CellStyle.Font.Bold = true;
                worksheet.Range["B3"].Text = "BILLY CRESPO";
                worksheet.Range["B4"].Text = DateTime.Now.ToString("dd/MM/yyy");

                #endregion Table data

                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                //Closing and Disponse los recursos ocupados.
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();

                string nombreReporte = "RptAsignacion.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();
                throw new Exception(e.Message);
            }
        }

        [HttpGet("Reporte/RptSeguimientoXEmpresa")]
        public ActionResult RptSeguimientoXEmpresa()
        {
            string nombre_hoja = "asignacion";
            ExcelEngine excelEngine = new ExcelEngine();

            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Excel2013;

            //A existing workbook is opened.
            string basePath = Startup.appPath + @"\wwwroot\assets\template\RptSeguimientoXEmpresa.xlsx";
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);
            try
            {
                //Access first worksheet from the workbook.
                IWorksheet worksheet = workbook.Worksheets[nombre_hoja];
                workbook.Worksheets.Remove(worksheet);
                worksheet = workbook.Worksheets.Create(nombre_hoja);

                #region Table data

                List<Entities.Helpers.AsignacionEmpleadoHelper> listaAsignacion = new Business.Public.Empleado_Hoja_Trabajo(this._appCnx).GetListaAsignacionEmpleado();
                DataTable table = new DataTable();

                table.Columns.Add("EMPRESA", typeof(string));
                table.Columns.Add("ENGAGEMENT", typeof(string));
                table.Columns.Add("FECHA INICIO", typeof(DateTime));
                table.Columns.Add("FECHA FIN", typeof(DateTime));
                table.Columns.Add("FACTURABLE", typeof(string));
                table.Columns.Add("ESTIMACION TOTAL HORAS", typeof(double));
                table.Columns.Add("HORAS OVERRUN", typeof(double));
                table.Columns.Add("HORAS ASIGNADAS", typeof(double));
                table.Columns.Add("EMPLEADO", typeof(string));
                table.Columns.Add("HORAS EJECUTADAS", typeof(double));

                foreach (var itemDetalle in listaAsignacion)
                {
                    table.Rows.Add(new object[] {
                        itemDetalle.nombre_empresa,
                        itemDetalle.nombre_engagement,
                        itemDetalle.fecha_inicio,
                        itemDetalle.fecha_fin,
                        itemDetalle.facturable,
                        itemDetalle.estimacion_total_horas,
                        itemDetalle.estimacion_overrun,
                        itemDetalle.horas_asignadas,
                        itemDetalle.nombre_empelado,
                        itemDetalle.horas_ejecutadas,
                        });
                }

                if (table.Rows.Count < 1)
                {
                    worksheet.Range["D5:I5"].Merge();
                    worksheet.Range["D5:I5"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range["D5:I5"].CellStyle.Font.Bold = true;
                    worksheet.Range["D5:I5"].Text = "NO EXISTEN DATOS";
                }
                else
                {
                    worksheet.ImportDataTable(table, true, 6, 1, true);

                    worksheet.ListObjects.Create("table_asignacion", worksheet.UsedRange);

                    worksheet.UsedRange.AutofitColumns();
                }

                worksheet.Range["B2:H2"].Merge();
                worksheet.Range["B2:H2"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range["B2:H2"].CellStyle.Font.Bold = true;
                worksheet.Range["B2:H2"].CellStyle.Font.Size = 20;
                worksheet.Range["B2:H2"].Text = "SEGUIMIENTO POR EMPRESA";

                worksheet.Range["A3"].Text = "GENERADO POR:";
                worksheet.Range["A4"].Text = "FECHA REPORTE:";
                worksheet.Range["A3:A4"].CellStyle.Font.Bold = true;
                worksheet.Range["B3"].Text = "BILLY CRESPO";
                worksheet.Range["B4"].Text = DateTime.Now.ToString("dd/MM/yyy");

                #endregion Table data

                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                //Closing and Disponse los recursos ocupados.
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();

                string nombreReporte = "RptSeguimientoXEmpresa.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();
                throw new Exception(e.Message);
            }
        }

        [HttpGet("Reporte/RptSeguimientoXEngagement/{id_empresa}")]
        public ActionResult RptSeguimientoXEngagement(long id_empresa)
        {
            string nombre_hoja = "asignacion";
            ExcelEngine excelEngine = new ExcelEngine();

            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Excel2013;

            //A existing workbook is opened.
            string basePath = Startup.appPath + @"\wwwroot\assets\template\RptSeguimientoXEngagement.xlsx";
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);
            try
            {
                //Access first worksheet from the workbook.
                IWorksheet worksheet = workbook.Worksheets[nombre_hoja];
                workbook.Worksheets.Remove(worksheet);
                worksheet = workbook.Worksheets.Create(nombre_hoja);

                #region Table data

                List<Entities.Helpers.AsignacionEmpleadoHelper> listaAsignacion = new Business.Public.Empleado_Hoja_Trabajo(this._appCnx).GetListaAsignacionEmpleado().Where(dtl => dtl.id_empresa == id_empresa).ToList();
                DataTable table = new DataTable();

                table.Columns.Add("ENGAGEMENT", typeof(string));
                table.Columns.Add("FECHA INICIO", typeof(DateTime));
                table.Columns.Add("FECHA FIN", typeof(DateTime));
                table.Columns.Add("FACTURABLE", typeof(string));
                table.Columns.Add("ESTIMACION TOTAL HORAS", typeof(double));
                table.Columns.Add("HORAS OVERRUN", typeof(double));
                table.Columns.Add("HORAS ASIGNADAS", typeof(double));
                table.Columns.Add("EMPLEADO", typeof(string));
                table.Columns.Add("HORAS EJECUTADAS", typeof(double));

                foreach (var itemDetalle in listaAsignacion)
                {
                    table.Rows.Add(new object[] {
                        itemDetalle.nombre_engagement,
                        itemDetalle.fecha_inicio,
                        itemDetalle.fecha_fin,
                        itemDetalle.facturable,
                        itemDetalle.estimacion_total_horas,
                        itemDetalle.estimacion_overrun,
                        itemDetalle.horas_asignadas,
                        itemDetalle.nombre_empelado,
                        itemDetalle.horas_ejecutadas,
                        });
                }

                if (table.Rows.Count < 1)
                {
                    worksheet.Range["D5:I5"].Merge();
                    worksheet.Range["D5:I5"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range["D5:I5"].CellStyle.Font.Bold = true;
                    worksheet.Range["D5:I5"].Text = "NO EXISTEN DATOS";
                }
                else
                {
                    worksheet.ImportDataTable(table, true, 6, 1, true);

                    worksheet.ListObjects.Create("table_asignacion", worksheet.UsedRange);

                    worksheet.UsedRange.AutofitColumns();
                }

                worksheet.Range["B2:H2"].Merge();
                worksheet.Range["B2:H2"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range["B2:H2"].CellStyle.Font.Bold = true;
                worksheet.Range["B2:H2"].CellStyle.Font.Size = 20;
                worksheet.Range["B2:H2"].Text = "SEGUIMIENTO POR ENGAGEMENT - EMPRESA:" + listaAsignacion[0].nombre_empresa;

                worksheet.Range["A3"].Text = "GENERADO POR:";
                worksheet.Range["A4"].Text = "FECHA REPORTE:";
                worksheet.Range["A3:A4"].CellStyle.Font.Bold = true;
                worksheet.Range["B3"].Text = "BILLY CRESPO";
                worksheet.Range["B4"].Text = DateTime.Now.ToString("dd/MM/yyy");

                #endregion Table data

                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                //Closing and Disponse los recursos ocupados.
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();

                string nombreReporte = "RptSeguimientoXEngagement.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();
                throw new Exception(e.Message);
            }
        }

        [HttpGet("Reporte/RptSeguimientoXEmpleado/{id_empresa}/{id_engagement}")]
        public ActionResult RptSeguimientoXEmpleado(long id_empresa, long id_engagement)
        {
            string nombre_hoja = "asignacion";
            ExcelEngine excelEngine = new ExcelEngine();

            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Excel2013;

            //A existing workbook is opened.
            string basePath = Startup.appPath + @"\wwwroot\assets\template\RptSeguimientoXEmpleado.xlsx";
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);
            try
            {
                //Access first worksheet from the workbook.
                IWorksheet worksheet = workbook.Worksheets[nombre_hoja];
                workbook.Worksheets.Remove(worksheet);
                worksheet = workbook.Worksheets.Create(nombre_hoja);

                #region Table data

                List<Entities.Helpers.AsignacionEmpleadoHelper> listaAsignacion = new Business.Public.Empleado_Hoja_Trabajo(this._appCnx).GetListaAsignacionEmpleado().Where(dtl => dtl.id_engagement == id_engagement).ToList();
                DataTable table = new DataTable();

                table.Columns.Add("ENGAGEMENT", typeof(string));
                table.Columns.Add("FECHA INICIO", typeof(DateTime));
                table.Columns.Add("FECHA FIN", typeof(DateTime));
                table.Columns.Add("FACTURABLE", typeof(string));
                table.Columns.Add("ESTIMACION TOTAL HORAS", typeof(double));
                table.Columns.Add("HORAS OVERRUN", typeof(double));
                table.Columns.Add("HORAS ASIGNADAS", typeof(double));
                table.Columns.Add("EMPLEADO", typeof(string));
                table.Columns.Add("HORAS EJECUTADAS", typeof(double));

                foreach (var itemDetalle in listaAsignacion)
                {
                    table.Rows.Add(new object[] {
                        itemDetalle.nombre_engagement,
                        itemDetalle.fecha_inicio,
                        itemDetalle.fecha_fin,
                        itemDetalle.facturable,
                        itemDetalle.estimacion_total_horas,
                        itemDetalle.estimacion_overrun,
                        itemDetalle.horas_asignadas,
                        itemDetalle.nombre_empelado,
                        itemDetalle.horas_ejecutadas,
                        });
                }

                if (table.Rows.Count < 1)
                {
                    worksheet.Range["D5:I5"].Merge();
                    worksheet.Range["D5:I5"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range["D5:I5"].CellStyle.Font.Bold = true;
                    worksheet.Range["D5:I5"].Text = "NO EXISTEN DATOS";
                }
                else
                {
                    worksheet.ImportDataTable(table, true, 6, 1, true);

                    worksheet.ListObjects.Create("table_asignacion", worksheet.UsedRange);

                    worksheet.UsedRange.AutofitColumns();
                }

                worksheet.Range["B2:H2"].Merge();
                worksheet.Range["B2:H2"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range["B2:H2"].CellStyle.Font.Bold = true;
                worksheet.Range["B2:H2"].CellStyle.Font.Size = 20;
                worksheet.Range["B2:H2"].Text = "SEGUIMIENTO POR EMPLEADO - ENGAGEMENT:" + listaAsignacion[0].nombre_engagement;

                worksheet.Range["A3"].Text = "GENERADO POR:";
                worksheet.Range["A4"].Text = "FECHA REPORTE:";
                worksheet.Range["A3:A4"].CellStyle.Font.Bold = true;
                worksheet.Range["B3"].Text = "BILLY CRESPO";
                worksheet.Range["B4"].Text = DateTime.Now.ToString("dd/MM/yyy");

                #endregion Table data

                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                //Closing and Disponse los recursos ocupados.
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();

                string nombreReporte = "RptSeguimientoXEmpleado.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();
                throw new Exception(e.Message);
            }
        }

        [HttpGet("Reporte/RptSeguimientoXEmpleado2")]
        public ActionResult RptSeguimientoXEmpleado2()
        {
            string nombre_hoja = "asignacion";
            ExcelEngine excelEngine = new ExcelEngine();

            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Excel2013;

            //A existing workbook is opened.
            string basePath = Startup.appPath + @"\wwwroot\assets\template\RptSeguimientoXEmpleado2.xlsx";
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);
            try
            {
                //Access first worksheet from the workbook.
                IWorksheet worksheet = workbook.Worksheets[nombre_hoja];
                workbook.Worksheets.Remove(worksheet);
                worksheet = workbook.Worksheets.Create(nombre_hoja);

                #region Table data

                List<Entities.Helpers.AsignacionEmpleadoHelper> listaAsignacion = new Business.Public.Empleado_Hoja_Trabajo(this._appCnx).GetListaAsignacionEmpleado().ToList();
                DataTable table = new DataTable();

                table.Columns.Add("ENGAGEMENT", typeof(string));
                table.Columns.Add("FECHA INICIO", typeof(DateTime));
                table.Columns.Add("FECHA FIN", typeof(DateTime));
                table.Columns.Add("FACTURABLE", typeof(string));
                table.Columns.Add("ESTIMACION TOTAL HORAS", typeof(double));
                table.Columns.Add("HORAS OVERRUN", typeof(double));
                table.Columns.Add("HORAS ASIGNADAS", typeof(double));
                table.Columns.Add("EMPLEADO", typeof(string));
                table.Columns.Add("HORAS EJECUTADAS", typeof(double));

                foreach (var itemDetalle in listaAsignacion)
                {
                    table.Rows.Add(new object[] {
                        itemDetalle.nombre_engagement,
                        itemDetalle.fecha_inicio,
                        itemDetalle.fecha_fin,
                        itemDetalle.facturable,
                        itemDetalle.estimacion_total_horas,
                        itemDetalle.estimacion_overrun,
                        itemDetalle.horas_asignadas,
                        itemDetalle.nombre_empelado,
                        itemDetalle.horas_ejecutadas,
                        });
                }

                if (table.Rows.Count < 1)
                {
                    worksheet.Range["D5:I5"].Merge();
                    worksheet.Range["D5:I5"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range["D5:I5"].CellStyle.Font.Bold = true;
                    worksheet.Range["D5:I5"].Text = "NO EXISTEN DATOS";
                }
                else
                {
                    worksheet.ImportDataTable(table, true, 6, 1, true);

                    worksheet.ListObjects.Create("table_asignacion", worksheet.UsedRange);

                    worksheet.UsedRange.AutofitColumns();
                }

                worksheet.Range["B2:H2"].Merge();
                worksheet.Range["B2:H2"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range["B2:H2"].CellStyle.Font.Bold = true;
                worksheet.Range["B2:H2"].CellStyle.Font.Size = 20;
                worksheet.Range["B2:H2"].Text = "SEGUIMIENTO POR EMPLEADO - ENGAGEMENT:" + listaAsignacion[0].nombre_engagement;

                worksheet.Range["A3"].Text = "GENERADO POR:";
                worksheet.Range["A4"].Text = "FECHA REPORTE:";
                worksheet.Range["A3:A4"].CellStyle.Font.Bold = true;
                worksheet.Range["B3"].Text = "DUAL ASSISTANCE";
                worksheet.Range["B4"].Text = DateTime.Now.ToString("dd/MM/yyy");

                #endregion Table data

                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                //Closing and Disponse los recursos ocupados.
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();

                string nombreReporte = "RptSeguimientoXEmpleado.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();
                throw new Exception(e.Message);
            }
        }

        [HttpGet("Reporte/RptSeguimientoXEngagement2/{id_empleado}")]
        public ActionResult RptSeguimientoXEngagement2(long id_empleado)
        {
            string nombre_hoja = "asignacion";
            ExcelEngine excelEngine = new ExcelEngine();

            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Excel2013;

            //A existing workbook is opened.
            string basePath = Startup.appPath + @"\wwwroot\assets\template\RptSeguimientoXEngagement2.xlsx";
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);
            try
            {
                //Access first worksheet from the workbook.
                IWorksheet worksheet = workbook.Worksheets[nombre_hoja];
                workbook.Worksheets.Remove(worksheet);
                worksheet = workbook.Worksheets.Create(nombre_hoja);

                #region Table data

                List<Entities.Helpers.AsignacionEmpleadoHelper> listaAsignacion = new Business.Public.Empleado_Hoja_Trabajo(this._appCnx).GetListaAsignacionEmpleado().Where(dtl => dtl.id_empleado == id_empleado).ToList();
                DataTable table = new DataTable();

                table.Columns.Add("ENGAGEMENT", typeof(string));
                table.Columns.Add("FECHA INICIO", typeof(DateTime));
                table.Columns.Add("FECHA FIN", typeof(DateTime));
                table.Columns.Add("FACTURABLE", typeof(string));
                table.Columns.Add("ESTIMACION TOTAL HORAS", typeof(double));
                table.Columns.Add("HORAS OVERRUN", typeof(double));
                table.Columns.Add("HORAS ASIGNADAS", typeof(double));
                table.Columns.Add("EMPLEADO", typeof(string));
                table.Columns.Add("HORAS EJECUTADAS", typeof(double));

                foreach (var itemDetalle in listaAsignacion)
                {
                    table.Rows.Add(new object[] {
                        itemDetalle.nombre_engagement,
                        itemDetalle.fecha_inicio,
                        itemDetalle.fecha_fin,
                        itemDetalle.facturable,
                        itemDetalle.estimacion_total_horas,
                        itemDetalle.estimacion_overrun,
                        itemDetalle.horas_asignadas,
                        itemDetalle.nombre_empelado,
                        itemDetalle.horas_ejecutadas,
                        });
                }

                if (table.Rows.Count < 1)
                {
                    worksheet.Range["D5:I5"].Merge();
                    worksheet.Range["D5:I5"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range["D5:I5"].CellStyle.Font.Bold = true;
                    worksheet.Range["D5:I5"].Text = "NO EXISTEN DATOS";
                }
                else
                {
                    worksheet.ImportDataTable(table, true, 6, 1, true);

                    worksheet.ListObjects.Create("table_asignacion", worksheet.UsedRange);

                    worksheet.UsedRange.AutofitColumns();
                }

                worksheet.Range["B2:H2"].Merge();
                worksheet.Range["B2:H2"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range["B2:H2"].CellStyle.Font.Bold = true;
                worksheet.Range["B2:H2"].CellStyle.Font.Size = 20;
                worksheet.Range["B2:H2"].Text = "SEGUIMIENTO POR ENGAGEMENT - EMPLEADO:" + listaAsignacion[0].nombre_empelado;

                worksheet.Range["A3"].Text = "GENERADO POR:";
                worksheet.Range["A4"].Text = "FECHA REPORTE:";
                worksheet.Range["A3:A4"].CellStyle.Font.Bold = true;
                worksheet.Range["B3"].Text = "DUAL ASSISTANCE";
                worksheet.Range["B4"].Text = DateTime.Now.ToString("dd/MM/yyy");

                #endregion Table data

                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                //Closing and Disponse los recursos ocupados.
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();

                string nombreReporte = "RptSeguimientoXEngagement.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();
                throw new Exception(e.Message);
            }
        }

        [HttpGet("Reporte/RptSeguimientoXEmpresa2/{id_empleado}/{id_engagement}")]
        public ActionResult RptSeguimientoXEmpresa2(long id_empleado, long id_engagement)
        {
            string nombre_hoja = "asignacion";
            ExcelEngine excelEngine = new ExcelEngine();

            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Excel2013;

            //A existing workbook is opened.
            string basePath = Startup.appPath + @"\wwwroot\assets\template\RptSeguimientoXEmpresa2.xlsx";
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);
            try
            {
                //Access first worksheet from the workbook.
                IWorksheet worksheet = workbook.Worksheets[nombre_hoja];
                workbook.Worksheets.Remove(worksheet);
                worksheet = workbook.Worksheets.Create(nombre_hoja);

                #region Table data

                List<Entities.Helpers.AsignacionEmpleadoHelper> listaAsignacion = new Business.Public.Empleado_Hoja_Trabajo(this._appCnx).GetListaAsignacionEmpleado().Where(dtl => dtl.id_empleado == id_empleado && dtl.id_engagement == id_engagement).ToList();
                DataTable table = new DataTable();

                table.Columns.Add("ENGAGEMENT", typeof(string));
                table.Columns.Add("FECHA INICIO", typeof(DateTime));
                table.Columns.Add("FECHA FIN", typeof(DateTime));
                table.Columns.Add("FACTURABLE", typeof(string));
                table.Columns.Add("ESTIMACION TOTAL HORAS", typeof(double));
                table.Columns.Add("HORAS OVERRUN", typeof(double));
                table.Columns.Add("HORAS ASIGNADAS", typeof(double));
                table.Columns.Add("EMPLEADO", typeof(string));
                table.Columns.Add("HORAS EJECUTADAS", typeof(double));

                foreach (var itemDetalle in listaAsignacion)
                {
                    table.Rows.Add(new object[] {
                        itemDetalle.nombre_engagement,
                        itemDetalle.fecha_inicio,
                        itemDetalle.fecha_fin,
                        itemDetalle.facturable,
                        itemDetalle.estimacion_total_horas,
                        itemDetalle.estimacion_overrun,
                        itemDetalle.horas_asignadas,
                        itemDetalle.nombre_empelado,
                        itemDetalle.horas_ejecutadas,
                        });
                }

                if (table.Rows.Count < 1)
                {
                    worksheet.Range["D5:I5"].Merge();
                    worksheet.Range["D5:I5"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range["D5:I5"].CellStyle.Font.Bold = true;
                    worksheet.Range["D5:I5"].Text = "NO EXISTEN DATOS";
                }
                else
                {
                    worksheet.ImportDataTable(table, true, 6, 1, true);

                    worksheet.ListObjects.Create("table_asignacion", worksheet.UsedRange);

                    worksheet.UsedRange.AutofitColumns();
                }

                worksheet.Range["B2:H2"].Merge();
                worksheet.Range["B2:H2"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range["B2:H2"].CellStyle.Font.Bold = true;
                worksheet.Range["B2:H2"].CellStyle.Font.Size = 20;
                worksheet.Range["B2:H2"].Text = "SEGUIMIENTO POR EMPLEADO - ENGAGEMENT:" + listaAsignacion[0].nombre_engagement;

                worksheet.Range["A3"].Text = "GENERADO POR:";
                worksheet.Range["A4"].Text = "FECHA REPORTE:";
                worksheet.Range["A3:A4"].CellStyle.Font.Bold = true;
                worksheet.Range["B3"].Text = "DUAL ASSISTANCE";
                worksheet.Range["B4"].Text = DateTime.Now.ToString("dd/MM/yyy");

                #endregion Table data

                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                //Closing and Disponse los recursos ocupados.
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();

                string nombreReporte = "RptSeguimientoXEmpresa.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();
                throw new Exception(e.Message);
            }
        }

        [HttpGet("Reporte/RptSeguimientoMarcacionXEmpleado")]
        public ActionResult RptSeguimientoMarcacionXEmpleado()
        {
            string nombre_hoja = "marcacion";
            ExcelEngine excelEngine = new ExcelEngine();

            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Excel2013;

            //A existing workbook is opened.
            string basePath = Startup.appPath + @"\wwwroot\assets\template\RptSeguimientoMarcacionXEmpleado.xlsx";
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);
            try
            {
                //Access first worksheet from the workbook.
                IWorksheet worksheet = workbook.Worksheets[nombre_hoja];
                workbook.Worksheets.Remove(worksheet);
                worksheet = workbook.Worksheets.Create(nombre_hoja);

                #region Table data

                List<Empresa> listaEmpresas = new BPU.Empresa(this._appCnx).GetLista();
                List<long> idsEmpresas = listaEmpresas.Select(e => e.id).ToList();

                List<EHE.ViewDashboardMarcacionEmpleadoReport> listaMarcacion = new BPU.DashboardPrincipal(this._appCnx).GetListaMarcacionEmpleados(idsEmpresas, DateTime.MinValue.Date, DateTime.Now.Date);

                DataTable table = new DataTable();

                table.Columns.Add("ID MARCACION", typeof(long));
                table.Columns.Add("CODIGO EMPLEADO", typeof(string));
                table.Columns.Add("NOMBRE EMPLEADO", typeof(string));
                table.Columns.Add("EMPRESA ENTRADA", typeof(string));
                table.Columns.Add("EMPRESA SALIDA", typeof(string));
                table.Columns.Add("DESCRIPCION TURNO", typeof(string));
                table.Columns.Add("ENTRADA PROGRAMADA", typeof(string));
                table.Columns.Add("SALIDA PROGRAMADA", typeof(string));
                table.Columns.Add("ENTRADA REAL", typeof(string));
                table.Columns.Add("SALIDA REAL", typeof(string));
                table.Columns.Add("ASISTENCIA", typeof(double));
                table.Columns.Add("RETRASO", typeof(double));
                table.Columns.Add("OBSERVACION ENTRADA", typeof(string));
                table.Columns.Add("OBSERVACION SALIDA", typeof(string));
                table.Columns.Add("TIPO MARCACION ENTRADA", typeof(string));
                table.Columns.Add("TIPO MARCACION SALIDA", typeof(string));

                foreach (var itemDetalle in listaMarcacion)
                {
                    table.Rows.Add(new object[] {
                        itemDetalle.id,
                        itemDetalle.codigo_empleado,
                        itemDetalle.nombre_empleado,
                        itemDetalle.empresa_entrada,
                        itemDetalle.empresa_salida,
                        itemDetalle.descripcion_turno,
                        itemDetalle.entrada_programada,
                        itemDetalle.salida_programada,
                        itemDetalle.entrada_real,
                        itemDetalle.salida_real,
                        Math.Round(itemDetalle.tiempo_asistido / 60, 2),
                        Math.Round(itemDetalle.tiempo_retraso / 60, 2),
                        itemDetalle.observacion_punto_entrada,
                        itemDetalle.observacion_punto_salida,
                        itemDetalle.tipo_marcacion_entrada,
                        itemDetalle.tipo_marcacion_salida,
                        });
                }

                if (table.Rows.Count < 1)
                {
                    worksheet.Range["D5:I5"].Merge();
                    worksheet.Range["D5:I5"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    worksheet.Range["D5:I5"].CellStyle.Font.Bold = true;
                    worksheet.Range["D5:I5"].Text = "NO EXISTEN DATOS";
                }
                else
                {
                    worksheet.ImportDataTable(table, true, 6, 1, true);

                    worksheet.ListObjects.Create("table_marcacion", worksheet.UsedRange);

                    worksheet.UsedRange.AutofitColumns();
                }

                worksheet.Range["B2:H2"].Merge();
                worksheet.Range["B2:H2"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range["B2:H2"].CellStyle.Font.Bold = true;
                worksheet.Range["B2:H2"].CellStyle.Font.Size = 20;
                worksheet.Range["B2:H2"].Text = "SEGUIMIENTO DE MARCACIONES";

                worksheet.Range["A3"].Text = "GENERADO POR:";
                worksheet.Range["A4"].Text = "FECHA REPORTE:";
                worksheet.Range["A3:A4"].CellStyle.Font.Bold = true;
                worksheet.Range["B3"].Text = "DUAL ASSISTANCE";
                worksheet.Range["B4"].Text = DateTime.Now.ToString("dd/MM/yyy");

                #endregion Table data

                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                //Closing and Disponse los recursos ocupados.
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();

                string nombreReporte = "RptSeguimientoMarcacionXEmpleado.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                sampleFile.Close();
                sampleFile.Dispose();
                throw new Exception(e.Message);
            }
        }
    }
}