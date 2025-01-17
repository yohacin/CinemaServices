using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Syncfusion.Drawing;
using Syncfusion.XlsIO;
using wDualAssistence.Models;
using wDualAssistence.Utility;

namespace wDualAssistence.Controllers
{
    public class ReporteController : MainController
    {
        //------------------------------------------------------------------------------------------Reporte MArcacion
        public IActionResult RptMarcaciones()
        {
            ViewModelBase vViewModelBase = new ViewModelBase(this.User);
            if (!this._RevisarAcceso(vViewModelBase.idUsuario, vViewModelBase.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }

            Business.Public.Marcacion oMarcacion = new Business.Public.Marcacion(this._appCnx);
            List<Entities.Helpers.MarcacionHelpers> listaMarcacionHelpers = oMarcacion.GetListaDetalle(DateTime.Now, DateTime.Now);
            if (listaMarcacionHelpers != null)
            {
                foreach (var itemMarcacion in listaMarcacionHelpers)
                {
                    var Foto_Marcacion_Ent = oMarcacion.GetEntityFotMarcacion(itemMarcacion.id, 1);
                    string urlFotoEnt = Startup.appUrl + "assets/recursos/question-icon.png";
                    if (Foto_Marcacion_Ent != null)
                    {
                        string[] arrayPath = Foto_Marcacion_Ent.url_foto.Split("/");
                        urlFotoEnt = Startup.url_java + arrayPath[arrayPath.Length - 1];
                    }
                    itemMarcacion.imagen_ent = urlFotoEnt;

                    string urlFotoSal = Startup.appUrl + "assets/recursos/question-icon.png";
                    var Foto_Marcacion_Sal = oMarcacion.GetEntityFotMarcacion(itemMarcacion.id, 2);
                    if (Foto_Marcacion_Sal != null)
                    {
                        string[] arrayPath = Foto_Marcacion_Sal.url_foto.Split("/");
                        urlFotoSal = Startup.url_java + arrayPath[arrayPath.Length - 1];
                    }
                    itemMarcacion.imagen_sal = urlFotoSal;
                }
            }

            ViewBag.lista = listaMarcacionHelpers;
            
            string vista = "RptMarcaciones";
            if (Startup.modo_independiente)
            {
                vista = "ModoIndependiente/RptMarcaciones";
            }

            return View(vista, vViewModelBase);
        }

        [HttpGet("Reporte/RptMarcacionesEXCEL/{desde}/{hasta}")]
        public ActionResult RptMarcacionesEXCEL(DateTime desde, DateTime hasta)
        {
            //Configuracion Hoja Excel
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2016;

            IWorkbook workbook = application.Workbooks.Create(1);
            IWorksheet sheetProspecto = workbook.Worksheets[0];
            sheetProspecto.Name = "marcaciones";

            #region Table data

            List<Entities.Helpers.MarcacionHelpers> listaMarcacionHelpers = new Business.Public.Marcacion(this._appCnx).GetListaDetalle(desde, hasta);

            DataTable table = new DataTable();

            table.Columns.Add("Nombre", typeof(string));
            table.Columns.Add("Empresa Entrada", typeof(string));
            table.Columns.Add("Empresa Salida", typeof(string));
            table.Columns.Add("Turno", typeof(string));
            table.Columns.Add("Entrada Programada", typeof(string));
            table.Columns.Add("Salida Programada", typeof(string));
            table.Columns.Add("Entrada Real", typeof(string));
            table.Columns.Add("Salida Real", typeof(string));
            table.Columns.Add("Tipo Marcacion Entrada", typeof(string));
            table.Columns.Add("Tipo Marcacion Salida", typeof(string));

            foreach (var itemMarcacion in listaMarcacionHelpers)
            {
                table.Rows.Add(new object[] {
                    itemMarcacion.nombre_empleado,
                    itemMarcacion.empresa_entrada,
                    itemMarcacion.empresa_salida,
                    itemMarcacion.descripcion_turno,
                    itemMarcacion.entrada_programada,
                    itemMarcacion.salida_programada,
                    itemMarcacion.entrada_real,
                    itemMarcacion.salida_real,
                    itemMarcacion.tipo_marcacion_entrada,
                    itemMarcacion.tipo_marcacion_salida
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

                IListObject tableProspectos = sheetProspecto.ListObjects.Create("table_marcaciones", sheetProspecto.UsedRange);

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
            sheetProspecto.Range["B3"].Text = desde.ToString("dd/MM/yyy");
            sheetProspecto.Range["B4"].Text = hasta.ToString("dd/MM/yyy");

            #endregion Table data

            try
            {
                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                string nombreReporte = "rptMarcaciones.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                throw new Exception(e.Message);
            }
        }

        [HttpGet("Reporte/RptMarcacionesGraficoEXCEL/{desde}/{hasta}")]
        public ActionResult RptMarcacionesGraficoEXCEL(DateTime desde, DateTime hasta)
        {
            //Configuracion Hoja Excel
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2016;

            IWorkbook workbook = application.Workbooks.Create(1);
            IWorksheet sheetProspecto = workbook.Worksheets[0];
            sheetProspecto.Name = "marcaciones";

            #region Table data

            List<Entities.Helpers.MarcacionHelpers> listaMarcacionHelpers = new Business.Public.Marcacion(this._appCnx).GetListaDetalle(desde, hasta);
            if (listaMarcacionHelpers == null) listaMarcacionHelpers = new List<Entities.Helpers.MarcacionHelpers>();

            var groupedEmpleado = listaMarcacionHelpers
              .GroupBy(u => u.codigo_empleado)
              .Select(grp => grp.ToList())
              .ToList();

            if (listaMarcacionHelpers.Count < 1)
            {
                sheetProspecto.Range["D5:I5"].Merge();
                sheetProspecto.Range["D5:I5"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                sheetProspecto.Range["D5:I5"].CellStyle.Font.Bold = true;
                sheetProspecto.Range["D5:I5"].Text = "NO EXISTEN DATOS";
            }
            else
            {
                int y = 6;
                int x = 0;
                sheetProspecto[y, 1].Text = "EMPLEADO";
                sheetProspecto[y, 1].VerticalAlignment = ExcelVAlign.VAlignCenter;
                sheetProspecto[y, 1].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                sheetProspecto[y, 1].CellStyle.Font.Bold = true;

                var itemDate = groupedEmpleado[0];
                string nombreColumnaLimite = "ZZZ";

                x = 1;
                foreach (var item in itemDate)
                {
                    x++;
                    sheetProspecto[6, x].Text = item.entrada_programada;
                    sheetProspecto[y, x].CellStyle.Rotation = 90;
                    sheetProspecto[y, x].VerticalAlignment = ExcelVAlign.VAlignCenter;
                    sheetProspecto[y, x].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    sheetProspecto[y, x].CellStyle.Font.Bold = true;
                    x++;
                    sheetProspecto[6, x].Text = item.salida_programada;
                    sheetProspecto[y, x].CellStyle.Rotation = 90;
                    sheetProspecto[y, x].VerticalAlignment = ExcelVAlign.VAlignCenter;
                    sheetProspecto[y, x].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    sheetProspecto[y, x].CellStyle.Font.Bold = true;
                }

                // Obtener nombre de la columna limite
                nombreColumnaLimite = new EXCELUtils().obtenerNombreColumna(x);

                foreach (var itemMarcacion in groupedEmpleado)
                {
                    y++;
                    sheetProspecto[y, 1].Value = itemMarcacion[0].nombre_empleado;
                    x = 1;
                    foreach (var marcacionDate in itemMarcacion)
                    {
                        x++;
                        sheetProspecto[y, x].Number = (marcacionDate.entrada_real == "SIN DATOS") ? -2 : 1;
                        x++;
                        sheetProspecto[y, x].Number = (marcacionDate.salida_real == "SIN DATOS") ? -2 : 1;
                    }
                }

                //Applying conditional formatting to worksheet
                sheetProspecto.Range["B7:ZZ" + y].CellStyle.Font.Color = ExcelKnownColors.White;
                IConditionalFormats conditionalFormats = sheetProspecto.Range["B7:" + nombreColumnaLimite + y].ConditionalFormats;

                IConditionalFormat conditionalFormat = conditionalFormats.AddCondition();

                //'Appyling icon set
                conditionalFormat.FormatType = ExcelCFType.IconSet;
                IIconSet iconSet = conditionalFormat.IconSet;

                iconSet.IconSet = ExcelIconSetType.ThreeSymbols;
                //'Icon set has 3 criteria of which second and third criteria can be changed.
                //'The first criterion type should not be changed as it is percent by default.
                //'The value in its condition changes with respect to the value assigned to the second criteria.
                iconSet.IconCriteria[1].Value = "-1";
                iconSet.IconCriteria[1].Type = ConditionValueType.Number;
                iconSet.IconCriteria[2].Value = "1";
                iconSet.IconCriteria[2].Type = ConditionValueType.Number;
                conditionalFormat.IconSet.ShowIconOnly = true;
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
            sheetProspecto.Range["B3"].Text = desde.ToString("dd/MM/yyy");
            sheetProspecto.Range["B4"].Text = hasta.ToString("dd/MM/yyy");

            #endregion Table data

            try
            {
                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                string nombreReporte = "rptMarcacionesGrafico.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                throw new Exception(e.Message);
            }
        }

        //-----------------------------------------------------------------------------------------Reporte PErmanencias
        public IActionResult RptPermanencias()
        {
            ViewModelBase vViewModelBase = new ViewModelBase(this.User);
            if (!this._RevisarAcceso(vViewModelBase.idUsuario, vViewModelBase.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }

            ViewBag.listaEmpresa = new Business.Public.Empresa(this._appCnx).GetLista();

            string vista = "RptPermanencias";
            if (Startup.modo_independiente)
            {
                vista = "ModoIndependiente/RptPermanencias";
            }


            return View(vista, vViewModelBase);
        }

        public IActionResult RptMarcacionErronea()
        {
            ViewModelBase vViewModelBase = new ViewModelBase(this.User);
            if (!this._RevisarAcceso(vViewModelBase.idUsuario, vViewModelBase.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }

            return View(vViewModelBase);
        }


        [HttpGet("Reporte/RptPermanenciasEXCEL/{desde}/{hasta}")]
        public ActionResult RptPermanenciasEXCEL(DateTime desde, DateTime hasta)
        {
            //Configuracion Hoja Excel
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2016;

            IWorkbook workbook = application.Workbooks.Create(1);
            IWorksheet sheetProspecto = workbook.Worksheets[0];
            sheetProspecto.Name = "permanencias";

            #region Table data

            List<Entities.Helpers.PermanenciaHelper> listaPermanencia = new Business.Public.Permanencia(this._appCnx).GetListaByDate(desde, hasta, 0);

            DataTable table = new DataTable();

            table.Columns.Add("Nombre Empleado", typeof(string));
            table.Columns.Add("Empresa", typeof(string));
            table.Columns.Add("Fecha / Hora Marcacion", typeof(DateTime));

            foreach (var itemPermanencia in listaPermanencia)
            {
                table.Rows.Add(new object[] {
                    itemPermanencia.nombre_empleado,
                    itemPermanencia.empresa,
                    itemPermanencia.hora_marcacion
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

                IListObject tableProspectos = sheetProspecto.ListObjects.Create("table_marcaciones", sheetProspecto.UsedRange);

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
            sheetProspecto.Range["B2:H2"].Text = "PERMANENCIAS";

            sheetProspecto.Range["A3"].Text = "Fecha Inicio";
            sheetProspecto.Range["A4"].Text = "Fecha Fin";
            sheetProspecto.Range["A3:A4"].CellStyle.Font.Bold = true;
            sheetProspecto.Range["B3"].Text = desde.ToString("dd/MM/yyy");
            sheetProspecto.Range["B4"].Text = hasta.ToString("dd/MM/yyy");

            #endregion Table data

            try
            {
                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                string nombreReporte = "rptMarcaciones.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                throw new Exception(e.Message);
            }
        }

        //-----------------------------------------------------------------------------------------Reporte seguimiento por tarea
        public IActionResult RptSeguimientoTarea()
        {
            ViewModelBase vViewModelBase = new ViewModelBase(this.User);
            if (!this._RevisarAcceso(vViewModelBase.idUsuario, vViewModelBase.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }
            return View(vViewModelBase);
        }

        [HttpGet("Reporte/RptSeguimientoTareaEXCEL/{desde}/{hasta}")]
        public ActionResult RptSeguimientoTareaEXCEL(DateTime desde, DateTime hasta)
        {
            string nombre_hoja = "seguimiento";
            ExcelEngine excelEngine = new ExcelEngine();

            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;

            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Excel2013;

            //A existing workbook is opened.
            string basePath = Startup.appPath + @"\wwwroot\assets\template\rptSeguimiento.xlsx";
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);
            try
            {
                ViewModelBase vViewModelBase = new ViewModelBase(this.User);

                //Access first worksheet from the workbook.
                IWorksheet worksheet = workbook.Worksheets[nombre_hoja];
                workbook.Worksheets.Remove(worksheet);

                worksheet = workbook.Worksheets.Create(nombre_hoja);

                #region Table data

                List<Entities.Helpers.SeguimientoEngagementHelper> listaSeguimiento = new Business.Public.Empleado_Hoja_Trabajo(this._appCnx).GetListaSeguimientoEngagement(desde, hasta);
                DataTable table = new DataTable();

                table.Columns.Add("EMPRESA", typeof(string));
                table.Columns.Add("NOMBRE ENGAGEMENT", typeof(string));
                table.Columns.Add("FECHA INICIO", typeof(DateTime));
                table.Columns.Add("FECHA FIN", typeof(DateTime));
                table.Columns.Add("ESTIMACION HORAS TAREA", typeof(double));
                table.Columns.Add("HORAS OVERRUN", typeof(double));
                table.Columns.Add("FACTURABLE", typeof(string));
                table.Columns.Add("LINEA DE SERVICIO", typeof(string));
                table.Columns.Add("TAREA REALIZADA", typeof(string));
                table.Columns.Add("EMPLEADO DE LA TAREA", typeof(string));
                table.Columns.Add("FECHA TAREA", typeof(DateTime));
                table.Columns.Add("TIEMPO EJECUTADO [HRS]", typeof(double));

                foreach (var itemDetalle in listaSeguimiento)
                {
                    table.Rows.Add(new object[] {
                        itemDetalle.nombre_empresa,
                        itemDetalle.nombre_engagement,
                        itemDetalle.fecha_inicio,
                        itemDetalle.fecha_fin,
                        itemDetalle.estimacion_horas_tarea,
                        itemDetalle.horas_overrun,
                        itemDetalle.facturable,
                        itemDetalle.categoria,
                        itemDetalle.tarea,
                        itemDetalle.empleado,
                        itemDetalle.fecha_tarea,
                        itemDetalle.tiempo_ejecutado
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

                    worksheet.ListObjects.Create("table_seguimiento", worksheet.UsedRange);

                    worksheet.UsedRange.AutofitColumns();
                }

                worksheet.Range["B2:H2"].Merge();
                worksheet.Range["B2:H2"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range["B2:H2"].CellStyle.Font.Bold = true;
                worksheet.Range["B2:H2"].CellStyle.Font.Size = 20;
                worksheet.Range["B2:H2"].Text = "SEGUIMIENTO DE TAREAS";

                worksheet.Range["A3"].Text = "GENERADO POR:";
                worksheet.Range["A4"].Text = "FECHA REPORTE:";
                worksheet.Range["A3:A4"].CellStyle.Font.Bold = true;
                worksheet.Range["B3"].Text = vViewModelBase.NombreUsuario;
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

                string nombreReporte = "rptSeguimiento.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                throw new Exception(e.Message);
            }
        }

        //-----------------------------------------------------------------------------------------Reporte Avance de engagement
        public IActionResult RptAvanceEngagement()
        {
            ViewModelBase vViewModelBase = new ViewModelBase(this.User);
            if (!this._RevisarAcceso(vViewModelBase.idUsuario, vViewModelBase.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }
            List<Entities.Helpers.AvanceEngagementHelper> listaAvance = new Business.Public.Engagement(this._appCnx).AvanceEngagement(DateTime.Now, DateTime.Now);
            ViewBag.lista = listaAvance;

            return View(vViewModelBase);
        }

        [HttpGet("Reporte/RptAvanceEngagementEXCEL/{desde}/{hasta}")]
        public ActionResult RptAvanceEngagementEXCEL(DateTime desde, DateTime hasta)
        {
            string nombre_hoja = "avance";
            ExcelEngine excelEngine = new ExcelEngine();

            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;
            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Excel2013;
            //A existing workbook is opened.
            string basePath = Startup.appPath + @"\wwwroot\assets\template\RptAvanceEngagement.xlsx";
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);
            try
            {
                //Access first worksheet from the workbook.
                IWorksheet worksheet = workbook.Worksheets[nombre_hoja];
                workbook.Worksheets.Remove(worksheet);
                worksheet = workbook.Worksheets.Create(nombre_hoja);

                #region Table data

                List<Entities.Helpers.AvanceEngagementHelper> listaAvance = new Business.Public.Engagement(this._appCnx).AvanceEngagement(desde, hasta);

                DataTable table = new DataTable();

                table.Columns.Add("EMPRESA", typeof(string));
                table.Columns.Add("ENGAGEMENT", typeof(string));
                table.Columns.Add("FECHA INICIO", typeof(DateTime));
                table.Columns.Add("FECHA FIN", typeof(DateTime));
                table.Columns.Add("HORAS TRABAJO", typeof(double));
                table.Columns.Add("HORAS EJECUTADAS", typeof(double));
                table.Columns.Add("AVANCE PORCENTAJE", typeof(double));

                foreach (var itemDetalle in listaAvance)
                {
                    table.Rows.Add(new object[] {
                        itemDetalle.nombre_empresa,
                        itemDetalle.nombre_engagement,
                        itemDetalle.desde,
                        itemDetalle.hasta,
                        itemDetalle.horas_trabajo,
                        itemDetalle.horas_ejecutadas,
                        itemDetalle.porcentaje_avance,
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

                    worksheet.ListObjects.Create("table_avance", worksheet.UsedRange);

                    worksheet.UsedRange.AutofitColumns();
                }

                worksheet.Range["B2:H2"].Merge();
                worksheet.Range["B2:H2"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range["B2:H2"].CellStyle.Font.Bold = true;
                worksheet.Range["B2:H2"].CellStyle.Font.Size = 20;
                worksheet.Range["B2:H2"].Text = "AVANCE DE ENGAGEMENT";

                worksheet.Range["A3"].Text = "DESDE:";
                worksheet.Range["A4"].Text = "HASTA:";
                worksheet.Range["A3:A4"].CellStyle.Font.Bold = true;
                worksheet.Range["B3"].Text = desde.ToString("dd/MM/yyyy");
                worksheet.Range["B4"].Text = hasta.ToString("dd/MM/yyy");

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

                string nombreReporte = "RptAvanceEngagement.xlsx";
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

        //---------------------------------------------------------------------------------------------Reporte Planeacion
        public IActionResult RptPlaneacion()
        {
            ViewModelBase vViewModelBase = new ViewModelBase(this.User);
            if (!this._RevisarAcceso(vViewModelBase.idUsuario, vViewModelBase.tipo, Url.ActionContext.HttpContext.Request.Path))
            {
                ErrorViewModel vErrorViewModel = new ErrorViewModel();
                vErrorViewModel.ErrorMessage = this.__MENSAJE_ACCESO_NEGADO;
                return View("Error", vErrorViewModel);
            }

            ViewBag.listaArea = new Business.Public.Area_Engagement(this._appCnx).GetLista();
            ViewBag.listaEmpresa = new Business.Public.Empresa(this._appCnx).GetLista();
            ViewBag.listaEngagement = new Business.Public.Engagement(this._appCnx).GetLista();

            return View(vViewModelBase);
        }

        private string[] sLetras = new string[] {
                        "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z",
                        "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV", "AW", "AX", "AY", "AZ",
                        "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN", "BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV", "BW", "BX", "BY", "BZ",
                        "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN", "CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV", "CW", "CX", "CY", "CZ",
                        "DA", "DB", "DC", "DD", "DE", "DF", "DG", "DH", "DI", "DJ", "DK", "DL", "DM", "DN", "DO", "DP", "DQ", "DR", "DS", "DT", "DU", "DV", "DW", "DX", "DY", "DZ",
                        "EA", "EB", "EC", "ED", "EE", "EF", "EG", "EH", "EI", "EJ", "EK", "EL", "EM", "EN", "EO", "EP", "EQ", "ER", "ES", "ET", "EU", "EV", "EW", "EX", "EY", "EZ",
                        "FA", "FB", "FC", "FD", "FE", "FF", "FG", "FH", "FI", "FJ", "FK", "FL", "FM", "FN", "FO", "FP", "FQ", "FR", "FS", "FT", "FU", "FV", "FW", "FX", "FY", "FZ",
                        "GA", "GB", "GC", "GD", "GE", "GF", "GG", "GH", "GI", "GJ", "GK", "GL", "GM", "GN", "GO", "GP", "GQ", "GR", "GS", "GT", "GU", "GV", "GW", "GX", "GY", "GZ",
                        "HA", "HB", "HC", "HD", "HE", "HF", "HG", "HH", "HI", "HJ", "HK", "HL", "HM", "HN", "HO", "HP", "HQ", "HR", "HS", "HT", "HU", "HV", "HW", "HX", "HY", "HZ",
                        "IA", "IB", "IC", "ID", "IE", "IF", "IG", "IH", "II", "IJ", "IK", "IL", "IM", "IN", "IO", "IP", "IQ", "IR", "IS", "IT", "IU", "IV", "IW", "IX", "IY", "IZ",
                        "JA", "JB", "JC", "JD", "JE", "JF", "JG", "JH", "JI", "JJ", "JK", "JL", "JM", "JN", "JO", "JP", "JQ", "JR", "JS", "JT", "JU", "JV", "JW", "JX", "JY", "JZ",
                        "KA", "KB", "KC", "KD", "KE", "KF", "KG", "KH", "KI", "KJ", "KK", "KL", "KM", "KN", "KO", "KP", "KQ", "KR", "KS", "KT", "KU", "KV", "KW", "KX", "KY", "KZ",
                    };

        [HttpGet("Reporte/RptPlaneacionEXCEL/{desde}/{hasta}/{id_empresa}/{id_area}/{id_engagement}")]
        public ActionResult RptPlaneacionEXCEL(DateTime desde, DateTime hasta, long id_empresa, long id_area, long id_engagement)
        {
            string nombre_hoja = "datos";
            ExcelEngine excelEngine = new ExcelEngine();

            //Instantiate the Excel application object
            IApplication application = excelEngine.Excel;
            //Assigns default application version
            application.DefaultVersion = ExcelVersion.Excel2013;
            //A existing workbook is opened.
            string basePath = Startup.appPath + @"\wwwroot\assets\template\RptPlaneacion.xlsx";
            FileStream sampleFile = new FileStream(basePath, FileMode.Open);

            IWorkbook workbook = application.Workbooks.Open(sampleFile);
            try
            {
                //Access first worksheet from the workbook.
                IWorksheet worksheet = workbook.Worksheets[nombre_hoja];
                workbook.Worksheets.Remove(worksheet);
                worksheet = workbook.Worksheets.Create(nombre_hoja);

                #region Table data

                List<Entities.Helpers.PlaneacionEngagementHelper> listaPlaneacion = new Business.Public.Engagement(this._appCnx).PlaneacionEngagement(desde, hasta, id_empresa, id_area, id_engagement);

                DataTable table = new DataTable();

                table.Columns.Add("EMPRESA", typeof(string));
                table.Columns.Add("ENGAGEMENT", typeof(string));
                table.Columns.Add("EMPLEADO", typeof(string));
                table.Columns.Add("CARGO", typeof(string));
                //------- columnas de fecha dinamica
                DateTime fechaInicio = desde;
                int iCant = 3;
                if (hasta < desde) throw new Exception("La fecha hasta debe ser mayor a la fecha desde");
                while (fechaInicio <= hasta)
                {
                    // sabados y domingos no cuentas
                    if (fechaInicio.DayOfWeek == DayOfWeek.Saturday || fechaInicio.DayOfWeek == DayOfWeek.Sunday)
                    {
                        fechaInicio = fechaInicio.AddDays(1);
                        continue;
                    }
                    iCant++;
                    table.Columns.Add(fechaInicio.Day.ToString() + "/" + fechaInicio.Month.ToString(), typeof(double));

                    fechaInicio = fechaInicio.AddDays(1);
                }
                table.Columns.Add("TOTAL HRS", typeof(double));

                foreach (var itemDetalle in listaPlaneacion)
                {
                    var objItem = new object[iCant + 2];

                    objItem[0] = itemDetalle.nombre_empresa;
                    objItem[1] = itemDetalle.nombre_engagement;
                    objItem[2] = itemDetalle.nombre_empleado;
                    objItem[3] = itemDetalle.cargo;

                    fechaInicio = desde;
                    int iCont = 3;
                    double total_fila = 0;
                    while (fechaInicio <= hasta)
                    {
                        // sabados y domingos no cuentas
                        if (fechaInicio.DayOfWeek == DayOfWeek.Saturday || fechaInicio.DayOfWeek == DayOfWeek.Sunday)
                        {
                            fechaInicio = fechaInicio.AddDays(1);
                            continue;
                        }
                        iCont++;
                        //table.Columns.Add(fechaInicio.Day.ToString(), typeof(string));

                        Entities.Public.Excepcion eExcepcion = new Business.Public.Excepcion(this._appCnx).BuscarByFechaEmpleado(itemDetalle.id_empleado, fechaInicio);
                        if (eExcepcion != null)
                        {
                            objItem[iCont] = "0";
                        }
                        else
                        {
                            List<Entities.Public.Detalle_Engagement_Empleado> listaDetalle = new Business.Public.Engagement(this._appCnx).DetallesEngagementEmpleadoByFecha(fechaInicio, itemDetalle.id_engagement, itemDetalle.id_empleado);

                            if (listaDetalle != null)
                            {
                                double sumHoras = listaDetalle.Sum(dtl => dtl.cantidad_horas);
                                if (sumHoras > 0)
                                {
                                    objItem[iCont] = sumHoras;
                                    total_fila += sumHoras;
                                }
                                //else {
                                //    objItem[iCont] = 1;
                                //}
                            }
                        }

                        //objItem[iCont] = (listaDetalle == null) ? "v" : .ToString();
                        fechaInicio = fechaInicio.AddDays(1);
                    }
                    iCont++;
                    objItem[iCont] = total_fila;

                    table.Rows.Add(objItem);
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
                    worksheet.ImportDataTable(table, true, 5, 1, true);

                    IListObject tableDynamic = worksheet.ListObjects.Create("table_planeacion", worksheet.UsedRange);

                    worksheet.UsedRange.AutofitColumns();

                    //-------------Estilos de la Hoja
                    ITableStyles tableStyles = workbook.TableStyles;
                    ITableStyle tableStyle = tableStyles.Add("Table Style Planeacion");
                    ITableStyleElements tableStyleElements = tableStyle.TableStyleElements;
                    ITableStyleElement tableStyleElement = tableStyleElements.Add(ExcelTableStyleElementType.SecondColumnStripe);

                    ITableStyleElement tableStyleElement1 = tableStyleElements.Add(ExcelTableStyleElementType.FirstColumn);
                    tableStyleElement1.FontColorRGB = Color.FromArgb(128, 128, 128);

                    ITableStyleElement tableStyleElement2 = tableStyleElements.Add(ExcelTableStyleElementType.HeaderRow);
                    tableStyleElement2.FontColor = ExcelKnownColors.White;
                    tableStyleElement2.BackColorRGB = Color.FromArgb(0, 112, 192);

                    ITableStyleElement tableStyleElement3 = tableStyleElements.Add(ExcelTableStyleElementType.TotalRow);
                    tableStyleElement3.BackColorRGB = Color.FromArgb(0, 112, 192);
                    tableStyleElement3.FontColor = ExcelKnownColors.White;

                    tableDynamic.TableStyleName = tableStyle.Name;

                    //-------------------------------------------------Formato Condicional
                    IConditionalFormats condition = worksheet.Range["E6:" + sLetras[iCant] + (listaPlaneacion.Count + 5).ToString()].ConditionalFormats;
                    IConditionalFormat condition1 = condition.AddCondition();
                    //conditional format to set duplicate format type
                    condition1.FormatType = ExcelCFType.SpecificText;
                    //condition1.Operator = ExcelComparisonOperator.Equal;
                    condition1.Text = "0";
                    condition1.BackColorRGB = Color.FromArgb(255, 255, 199, 206);

                    // -------------------------------------Totales
                    tableDynamic.ShowTotals = true;
                    tableDynamic.ShowFirstColumn = true;
                    tableDynamic.ShowTableStyleColumnStripes = true;
                    tableDynamic.Columns[0].TotalsRowLabel = "Total";

                    fechaInicio = desde;
                    int iCont = 3;
                    if (hasta < desde) throw new Exception("La fecha hasta debe ser mayor a la fecha desde");
                    while (fechaInicio <= hasta)
                    {
                        // sabados y domingos no cuentas
                        if (fechaInicio.DayOfWeek == DayOfWeek.Saturday || fechaInicio.DayOfWeek == DayOfWeek.Sunday)
                        {
                            fechaInicio = fechaInicio.AddDays(1);
                            continue;
                        }
                        iCont++;

                        tableDynamic.Columns[iCont].TotalsCalculation = ExcelTotalsCalculation.Sum;

                        fechaInicio = fechaInicio.AddDays(1);
                    }
                    iCont++;
                    tableDynamic.Columns[iCont].TotalsCalculation = ExcelTotalsCalculation.Sum;

                    //To freeze a row or column, the selected range should be next to the row or column
                    IRange range = worksheet[1, 5];
                    range.FreezePanes();

                    //To freeze a row or column, the selected range should be next to the row or column
                    IRange range2 = worksheet[6, 1];
                    range.FreezePanes();

                    worksheet.Range["D1"].Text = "VACACION";
                    worksheet.Range["D1"].CellStyle.FillBackgroundRGB = Color.FromArgb(255, 255, 199, 206);
                }

                worksheet.Range["A1:B1"].Merge();
                worksheet.Range["A1:B1"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                worksheet.Range["A1:B1"].CellStyle.Font.Bold = true;
                worksheet.Range["A1:B1"].CellStyle.FillBackground = ExcelKnownColors.Sky_blue;
                worksheet.Range["A1:B1"].Text = "PLANEACIÓN DE ENGAGEMENT";

                worksheet.Range["A2"].Text = "DESDE:";
                worksheet.Range["B2"].Text = desde.ToString("dd/MM/yyyy");
                worksheet.Range["B2"].HorizontalAlignment = ExcelHAlign.HAlignLeft;

                worksheet.Range["A3"].Text = "HASTA:";
                worksheet.Range["B3"].Text = hasta.ToString("dd/MM/yyyy");
                worksheet.Range["B3"].HorizontalAlignment = ExcelHAlign.HAlignLeft;

                worksheet.Range["C2"].Text = "FECHA REPORTE:";
                worksheet.Range["C2"].HorizontalAlignment = ExcelHAlign.HAlignRight;
                worksheet.Range["D2"].Text = DateTime.Now.ToString("dd/MM/yyyy");
                worksheet.Range["D2"].HorizontalAlignment = ExcelHAlign.HAlignLeft;

                worksheet.Range["A2:A4"].CellStyle.Font.Bold = true;

                IWorksheet worksheetDel = workbook.Worksheets["inicio"];
                workbook.Worksheets.Remove(worksheetDel);

                //worksheet.Range["A3"].Text = "DESDE:";
                //worksheet.Range["A4"].Text = "HASTA:";
                //worksheet.Range["A3:A4"].CellStyle.Font.Bold = true;
                //worksheet.Range["B3"].Text = desde.ToString("dd/MM/yyyy");
                //worksheet.Range["B4"].Text = hasta.ToString("dd/MM/yyy");

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

                string nombreReporte = "RptPlaneacionEngagement.xlsx";
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



        // -------------------------------------------------------------------------------------
        // -------------------------------------------------------------------------------------
        // -------------------------------------------------------------------------------------
        // -------------------------------------------------------------------------------------


        [HttpGet("Reporte/RptMarcacionesEXCELModoIndependiente/{desde}/{hasta}")]
        public ActionResult RptMarcacionesEXCELModoIndependiente(DateTime desde, DateTime hasta)
        {
            //Configuracion Hoja Excel
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2016;

            IWorkbook workbook = application.Workbooks.Create(1);
            IWorksheet sheetProspecto = workbook.Worksheets[0];
            sheetProspecto.Name = "marcaciones";

            #region Table data

            List<Entities.Helpers.MarcacionHelpers> listaMarcacionHelpers = new Business.Public.Marcacion(this._appCnx).GetListaDetalle(desde, hasta);

            DataTable table = new DataTable();

            table.Columns.Add("Nombre", typeof(string));
            table.Columns.Add("Ubicación Entrada", typeof(string));
            table.Columns.Add("Ubicación Salida", typeof(string));
            table.Columns.Add("Turno", typeof(string));
            table.Columns.Add("Entrada Programada", typeof(string));
            table.Columns.Add("Salida Programada", typeof(string));
            table.Columns.Add("Entrada Real", typeof(string));
            table.Columns.Add("Salida Real", typeof(string));
            table.Columns.Add("Tipo Marcacion Entrada", typeof(string));
            table.Columns.Add("Tipo Marcacion Salida", typeof(string));

            foreach (var itemMarcacion in listaMarcacionHelpers)
            {
                table.Rows.Add(new object[] {
                    itemMarcacion.nombre_empleado,
                    itemMarcacion.empresa_entrada,
                    itemMarcacion.empresa_salida,
                    itemMarcacion.descripcion_turno,
                    itemMarcacion.entrada_programada,
                    itemMarcacion.salida_programada,
                    itemMarcacion.entrada_real,
                    itemMarcacion.salida_real,
                    itemMarcacion.tipo_marcacion_entrada,
                    itemMarcacion.tipo_marcacion_salida
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

                IListObject tableProspectos = sheetProspecto.ListObjects.Create("table_marcaciones", sheetProspecto.UsedRange);

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
            sheetProspecto.Range["B3"].Text = desde.ToString("dd/MM/yyy");
            sheetProspecto.Range["B4"].Text = hasta.ToString("dd/MM/yyy");

            #endregion Table data

            try
            {
                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                string nombreReporte = "rptMarcaciones.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                throw new Exception(e.Message);
            }
        }

        [HttpGet("Reporte/RptMarcacionesGraficoEXCELModoIndependiente/{desde}/{hasta}")]
        public ActionResult RptMarcacionesGraficoEXCELModoIndependiente(DateTime desde, DateTime hasta)
        {
            //Configuracion Hoja Excel
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2016;

            IWorkbook workbook = application.Workbooks.Create(1);
            IWorksheet sheetProspecto = workbook.Worksheets[0];
            sheetProspecto.Name = "marcaciones";

            #region Table data

            List<Entities.Helpers.MarcacionHelpers> listaMarcacionHelpers = new Business.Public.Marcacion(this._appCnx).GetListaDetalle(desde, hasta);
            if (listaMarcacionHelpers == null) listaMarcacionHelpers = new List<Entities.Helpers.MarcacionHelpers>();

            var groupedEmpleado = listaMarcacionHelpers
              .GroupBy(u => u.codigo_empleado)
              .Select(grp => grp.ToList())
              .ToList();

            if (listaMarcacionHelpers.Count < 1)
            {
                sheetProspecto.Range["D5:I5"].Merge();
                sheetProspecto.Range["D5:I5"].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                sheetProspecto.Range["D5:I5"].CellStyle.Font.Bold = true;
                sheetProspecto.Range["D5:I5"].Text = "NO EXISTEN DATOS";
            }
            else
            {
                int y = 6;
                int x = 0;
                sheetProspecto[y, 1].Text = "FUNCIONARIO";
                sheetProspecto[y, 1].VerticalAlignment = ExcelVAlign.VAlignCenter;
                sheetProspecto[y, 1].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                sheetProspecto[y, 1].CellStyle.Font.Bold = true;

                var itemDate = groupedEmpleado[0];
                string nombreColumnaLimite = "ZZZ";

                x = 1;
                foreach (var item in itemDate)
                {
                    x++;
                    sheetProspecto[6, x].Text = item.entrada_programada;
                    sheetProspecto[y, x].CellStyle.Rotation = 90;
                    sheetProspecto[y, x].VerticalAlignment = ExcelVAlign.VAlignCenter;
                    sheetProspecto[y, x].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    sheetProspecto[y, x].CellStyle.Font.Bold = true;
                    x++;
                    sheetProspecto[6, x].Text = item.salida_programada;
                    sheetProspecto[y, x].CellStyle.Rotation = 90;
                    sheetProspecto[y, x].VerticalAlignment = ExcelVAlign.VAlignCenter;
                    sheetProspecto[y, x].HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    sheetProspecto[y, x].CellStyle.Font.Bold = true;
                }

                // Obtener nombre de la columna limite
                nombreColumnaLimite = new EXCELUtils().obtenerNombreColumna(x);

                foreach (var itemMarcacion in groupedEmpleado)
                {
                    y++;
                    sheetProspecto[y, 1].Value = itemMarcacion[0].nombre_empleado;
                    x = 1;
                    foreach (var marcacionDate in itemMarcacion)
                    {
                        x++;
                        sheetProspecto[y, x].Number = (marcacionDate.entrada_real == "SIN DATOS") ? -2 : 1;
                        x++;
                        sheetProspecto[y, x].Number = (marcacionDate.salida_real == "SIN DATOS") ? -2 : 1;
                    }
                }

                //Applying conditional formatting to worksheet
                sheetProspecto.Range["B7:ZZ" + y].CellStyle.Font.Color = ExcelKnownColors.White;
                IConditionalFormats conditionalFormats = sheetProspecto.Range["B7:" + nombreColumnaLimite + y].ConditionalFormats;

                IConditionalFormat conditionalFormat = conditionalFormats.AddCondition();

                //'Appyling icon set
                conditionalFormat.FormatType = ExcelCFType.IconSet;
                IIconSet iconSet = conditionalFormat.IconSet;

                iconSet.IconSet = ExcelIconSetType.ThreeSymbols;
                //'Icon set has 3 criteria of which second and third criteria can be changed.
                //'The first criterion type should not be changed as it is percent by default.
                //'The value in its condition changes with respect to the value assigned to the second criteria.
                iconSet.IconCriteria[1].Value = "-1";
                iconSet.IconCriteria[1].Type = ConditionValueType.Number;
                iconSet.IconCriteria[2].Value = "1";
                iconSet.IconCriteria[2].Type = ConditionValueType.Number;
                conditionalFormat.IconSet.ShowIconOnly = true;
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
            sheetProspecto.Range["B3"].Text = desde.ToString("dd/MM/yyy");
            sheetProspecto.Range["B4"].Text = hasta.ToString("dd/MM/yyy");

            #endregion Table data

            try
            {
                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                string nombreReporte = "rptMarcacionesGrafico.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                throw new Exception(e.Message);
            }
        }




        [HttpGet("Reporte/RptPermanenciasEXCELModoIndependiente/{desde}/{hasta}")]
        public ActionResult RptPermanenciasEXCELModoIndependiente(DateTime desde, DateTime hasta)
        {
            //Configuracion Hoja Excel
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2016;

            IWorkbook workbook = application.Workbooks.Create(1);
            IWorksheet sheetProspecto = workbook.Worksheets[0];
            sheetProspecto.Name = "permanencias";

            #region Table data

            List<Entities.Helpers.PermanenciaHelper> listaPermanencia = new Business.Public.Permanencia(this._appCnx).GetListaByDate(desde, hasta, 0);

            DataTable table = new DataTable();

            table.Columns.Add("Nombre Funcionario", typeof(string));
            table.Columns.Add("Ubicacion", typeof(string));
            table.Columns.Add("Fecha / Hora Marcacion", typeof(DateTime));

            foreach (var itemPermanencia in listaPermanencia)
            {
                table.Rows.Add(new object[] {
                    itemPermanencia.nombre_empleado,
                    itemPermanencia.empresa,
                    itemPermanencia.hora_marcacion
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

                IListObject tableProspectos = sheetProspecto.ListObjects.Create("table_marcaciones", sheetProspecto.UsedRange);

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
            sheetProspecto.Range["B2:H2"].Text = "PERMANENCIAS";

            sheetProspecto.Range["A3"].Text = "Fecha Inicio";
            sheetProspecto.Range["A4"].Text = "Fecha Fin";
            sheetProspecto.Range["A3:A4"].CellStyle.Font.Bold = true;
            sheetProspecto.Range["B3"].Text = desde.ToString("dd/MM/yyy");
            sheetProspecto.Range["B4"].Text = hasta.ToString("dd/MM/yyy");

            #endregion Table data

            try
            {
                workbook.Version = ExcelVersion.Excel2013;
                MemoryStream ms = new MemoryStream();
                workbook.SaveAs(ms);
                ms.Position = 0;

                string nombreReporte = "rptPermanencias.xlsx";
                return File(ms, "Application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nombreReporte);
            }
            catch (Exception e)
            {
                workbook.Close();
                excelEngine.Dispose();
                throw new Exception(e.Message);
            }
        }
    }
}