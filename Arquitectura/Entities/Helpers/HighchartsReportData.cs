using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities.Helpers
{
    public class HighchartsReportData
    {
        public struct Detalle
        {
            public string name;
            public string value;
        }

        public struct Data
        {
            public string id;
            public string name;
            public double y;
            public double y2;
            public string color;
            public string drilldown;
            public List<Detalle> detalle;
            public string datoExtra;

            public Data(List<Detalle> detalle) : this()
            {
                this.detalle = detalle;
            }
        }

        public struct Serie
        {
            public string id;
            public string type;
            public string name;
            public string color;
            public string center;
            public string size;
            public bool showInLegend;
            public string tooltipFormat;
            public string dataLabelsFormat;
            public List<Data> data;

            public Serie(List<Data> data) : this()
            {
                this.data = data;
            }
        }

        public struct Eje
        {
            public int min;
            public int max;
            public string alineacion;
            public List<string> categorias;
            public string titulo;
            public string formatLabels;
            public bool scrollbar;

            public Eje(List<string> categorias) : this()
            {
                this.categorias = categorias;
            }
        }

        public struct Leyenda
        {
            public string sentido;
            public string alineacion;
            public string alineacionVertical;
            public int x;
            public int y;
            public string colorFondo;
            public string colorBorde;
            public string flotante;
            public string alto;
            public string ancho;
        }

        public struct Parametros
        {
            public string moneda;
            public string usuario;
        }

        public struct DrillDown
        {
            public List<Serie> series;

            public DrillDown(List<Serie> series)
            {
                this.series = series;
            }
        }

        public string nombreGrafico { get; set; }
        public string subtituloGrafico { get; set; }
        public string tipoGrafico { get; set; }
        public Eje ejeX { get; set; }
        public Eje ejeY { get; set; }
        public Leyenda leyenda { get; set; }
        public List<Serie> series { get; set; }
        public Parametros parametros { get; set; }
        public DrillDown drilldown { get; set; }

        public HighchartsReportData()
        {
            this.ejeX = new Eje(new List<string>());
            this.ejeY = new Eje(new List<string>());
            this.leyenda = new Leyenda();
            this.series = new List<Serie>();
            this.parametros = new Parametros();
            this.drilldown = new DrillDown(new List<Serie>());
        }

        private void cargarSerieColumn(string id, string nombre, bool leyenda, List<Data> data,
            bool drilldown, string tooltip, string dataLabel)
        {
            Serie serie = new Serie(new List<Data>());
            serie.id = id;
            serie.type = "column";
            serie.name = nombre;
            serie.showInLegend = leyenda;
            serie.data = data;
            serie.dataLabelsFormat = dataLabel;
            serie.tooltipFormat = tooltip;

            if (drilldown)
            {
                this.drilldown.series.Add(serie);
            }
            else
            {
                this.series.Add(serie);
            }
        }

        public void cargarGraficoDashboard(List<ViewDashboardReport> registros)
        {
            this.nombreGrafico = "Engagements Activos";
            this.subtituloGrafico = "Navegue dando click en un indicador";
            this.tipoGrafico = "column";

            string sufijo = "Hrs.";
            double cantidadMayor = 0.0;
            string dataLabelsFormat = "{point.y:,.2f} " + sufijo;
            string toolTipoFormat = "{series.name}: <b>{point.y:,.2f} " + sufijo + "</b><br> Presione para ver detalle";
            string dataLabelsFormatDrill = "{point.y:,.2f} {point.id}";
            string toolTipoFormatDrill = "{series.name}: <b>{point.y:,.2f} {point.id}</b> Presione para generar reporte";

            var groupByEmpresa = registros.GroupBy(r => r.id_empresa).ToList();
            List<Data> dataListEmpresas = new List<Data>();

            foreach (var groupEmpresa in groupByEmpresa)
            {
                string idEmpresa = groupEmpresa.Key.ToString();
                string nombreEmpresa = "nombre empresa";

                double totalHorasEstimadasEmpresa = 0.0;
                double totalHorasEjecutadasEmpresa = 0.0;

                List<ViewDashboardReport> registrosEmpresa = groupEmpresa.ToList();

                nombreEmpresa = registrosEmpresa.First().nombre_empresa;

                var groupByEngagement = registrosEmpresa.GroupBy(r => r.id_engagement).ToList();
                List<Data> dataListEngagements = new List<Data>();

                foreach (var groupEngagement in groupByEngagement)
                {
                    string idEngagement = groupEngagement.Key.ToString();
                    string nombreEngagement = "nombre engagement";

                    double totalHorasEstimadasEngagement = 0.0;
                    double totalHorasEjecutadasEngagement = 0.0;

                    List<ViewDashboardReport> registrosEngagement = groupEngagement.ToList();

                    nombreEngagement = registrosEngagement.First().nombre_engagement;

                    var groupByEmpleado = registrosEngagement.GroupBy(r => r.id_empleado).ToList();
                    List<Data> dataListEmpleados = new List<Data>();

                    foreach (var groupEmpleado in groupByEmpleado)
                    {
                        string idEmpleado = groupEmpleado.Key.ToString();
                        string nombreEmpleado = "nombre empleado";

                        double totalHorasAsignadasEmpleado = 0.0;
                        double totalHorasEjecutadasEmpleado = 0.0;

                        List<ViewDashboardReport> registrosEmpleado = groupEmpleado.ToList();

                        nombreEmpleado = registrosEmpleado.First().nombre_empleado;

                        totalHorasAsignadasEmpleado = registrosEmpleado.Sum(r => r.horas_asignadas_empleado);
                        totalHorasEjecutadasEmpleado = registrosEmpleado.Sum(r => r.horas_ejecutadas_empleado);

                        //totalHorasEstimadasEngagement += totalHorasAsignadasEmpleado;
                        totalHorasEjecutadasEngagement += totalHorasEjecutadasEmpleado;

                        //totalHorasEstimadasEmpresa += totalHorasAsignadasEmpleado;
                        totalHorasEjecutadasEmpresa += totalHorasEjecutadasEmpleado;

                        if (totalHorasAsignadasEmpleado > 0)
                        {
                            Data dataEmpleado = new Data(new List<Detalle>());
                            dataEmpleado.id = $@"{idEmpresa}/{idEngagement}/{idEmpleado}";
                            dataEmpleado.name = nombreEmpleado;
                            dataEmpleado.y = totalHorasAsignadasEmpleado;
                            dataEmpleado.y2 = totalHorasEjecutadasEmpleado;
                            dataListEmpleados.Add(dataEmpleado);
                        }
                    }

                    string idSerieEmpleado = $@"{nombreEmpresa}/{nombreEngagement}";
                    string nombreSerieEmpleado = $@"{nombreEmpresa}/{nombreEngagement}";

                    this.cargarSerieColumn(idSerieEmpleado, nombreSerieEmpleado, true, dataListEmpleados, true, toolTipoFormat, dataLabelsFormat);

                    totalHorasEstimadasEngagement += registrosEngagement.First().total_horas_estimadas;

                    totalHorasEstimadasEmpresa += registrosEngagement.First().total_horas_estimadas;

                    if (totalHorasEstimadasEngagement > 0)
                    {
                        Data dataEngagement = new Data(new List<Detalle>());
                        dataEngagement.id = $@"{idEmpresa}/{idEngagement}";
                        dataEngagement.name = nombreEngagement;
                        dataEngagement.drilldown = $@"{nombreEmpresa}/{nombreEngagement}";
                        dataEngagement.y = totalHorasEstimadasEngagement;
                        dataEngagement.y2 = totalHorasEjecutadasEngagement;
                        dataListEngagements.Add(dataEngagement);
                    }
                }

                string idSerieEngagement = nombreEmpresa;
                string nombreSerieEngagement = nombreEmpresa;

                this.cargarSerieColumn(idSerieEngagement, nombreSerieEngagement, true, dataListEngagements, true, toolTipoFormat, dataLabelsFormat);

                if (totalHorasEstimadasEmpresa > 0)
                {
                    if (totalHorasEstimadasEmpresa > cantidadMayor)
                    {
                        cantidadMayor = totalHorasEstimadasEmpresa;
                    }

                    Data dataEmpresa = new Data(new List<Detalle>());
                    dataEmpresa.id = idEmpresa;
                    dataEmpresa.name = nombreEmpresa;
                    dataEmpresa.drilldown = nombreEmpresa;
                    dataEmpresa.y = totalHorasEstimadasEmpresa;
                    dataEmpresa.y2 = totalHorasEjecutadasEmpresa;
                    dataListEmpresas.Add(dataEmpresa);
                }
            }

            string idSerieEmpresa = "Empresas";
            string nombreSerieEmpresa = "Empresas";

            this.cargarSerieColumn(idSerieEmpresa, nombreSerieEmpresa, true, dataListEmpresas, false, toolTipoFormat, dataLabelsFormat);

            Serie serie = new Serie(new List<Data>());
            serie.id = "Horas tarea";
            serie.name = "<div id='divHoras' style='display:none;'>Horas tarea</div>";
            serie.type = "spline";
            serie.showInLegend = false;
            serie.data = dataListEmpresas;
            this.series.Add(serie);

            Eje x = new Eje(null); // categorias null para que muestre las etiquetas
            x.min = 0;
            x.max = groupByEmpresa.Count - 1;
            x.scrollbar = true;

            this.ejeX = x;

            Eje y = new Eje(null); // categorias null para que muestre las etiquetas
            y.min = 0;
            y.max = (int)cantidadMayor;
            y.formatLabels = "{value:,f} " + sufijo;

            this.ejeY = y;
        }

        public void cargarGraficoDashboardEmpleado(List<ViewDashboardReport> registros)
        {
            this.nombreGrafico = "Engagements Activos";
            this.subtituloGrafico = "Navegue dando click en un indicador";
            this.tipoGrafico = "column";

            string sufijo = "Hrs.";
            double cantidadMayor = 0.0;
            string dataLabelsFormat = "{point.y:,.2f} " + sufijo;
            string toolTipoFormat = "{series.name}: <b>{point.y:,.2f} " + sufijo + "</b><br> Presione para ver detalle";
            string dataLabelsFormatDrill = "{point.y:,.2f} {point.id}";
            string toolTipoFormatDrill = "{series.name}: <b>{point.y:,.2f} {point.id}</b> Presione para generar reporte";

            var groupByEmpleado = registros.GroupBy(r => r.id_empleado).ToList();
            List<Data> dataListEmpleados = new List<Data>();

            foreach (var groupEmpleado in groupByEmpleado)
            {
                string idEmpleado = groupEmpleado.Key.ToString();
                string nombreEmpleado = "nombre empleado";

                double totalHorasAsignadasEmpleado = 0.0;
                double totalHorasEjecutadasEmpleado = 0.0;

                List<ViewDashboardReport> registrosEmpleado = groupEmpleado.ToList();

                nombreEmpleado = registrosEmpleado.First().nombre_empleado;

                var groupByEngagement = registrosEmpleado.GroupBy(r => r.id_engagement).ToList();
                List<Data> dataListEngagements = new List<Data>();

                foreach (var groupEngagement in groupByEngagement)
                {
                    string idEngagement = groupEngagement.Key.ToString();
                    string nombreEngagement = "nombre engagement";

                    double totalHorasEstimadasEngagement = 0.0;
                    double totalHorasEjecutadasEngagement = 0.0;

                    List<ViewDashboardReport> registrosEngagement = groupEngagement.ToList();

                    nombreEngagement = registrosEngagement.First().nombre_engagement;

                    var groupByEmpresa = registrosEngagement.GroupBy(r => r.id_empresa).ToList();
                    List<Data> dataListEmpresas = new List<Data>();

                    foreach (var groupEmpresa in groupByEmpresa)
                    {
                        string idEmpresa = groupEmpresa.Key.ToString();
                        string nombreEmpresa = "nombre empresa";

                        double totalHorasEstimadasEmpresa = 0.0;
                        double totalHorasEjecutadasEmpresa = 0.0;

                        List<ViewDashboardReport> registrosEmpresa = groupEmpresa.ToList();

                        nombreEmpresa = registrosEmpresa.First().nombre_empresa;

                        totalHorasEstimadasEmpresa = registrosEmpresa.Sum(r => r.total_horas_estimadas);
                        totalHorasEjecutadasEmpresa = registrosEmpresa.Sum(r => r.horas_ejecutadas_empleado);

                        totalHorasEjecutadasEngagement += totalHorasEjecutadasEmpresa;

                        totalHorasEjecutadasEmpleado += totalHorasEjecutadasEmpresa;

                        if (totalHorasEstimadasEmpresa > 0)
                        {
                            Data dataEmpresa = new Data(new List<Detalle>());
                            dataEmpresa.id = $@"{idEmpleado}/{idEngagement}/{idEmpresa}";
                            dataEmpresa.name = nombreEmpresa;
                            dataEmpresa.y = totalHorasEstimadasEmpresa;
                            dataEmpresa.y2 = totalHorasEjecutadasEmpresa;
                            dataListEmpresas.Add(dataEmpresa);
                        }
                    }

                    string idSerieEmpresa = $@"{nombreEmpleado}/{nombreEngagement}";
                    string nombreSerieEmpresa = $@"{nombreEmpleado}/{nombreEngagement}";

                    this.cargarSerieColumn(idSerieEmpresa, nombreSerieEmpresa, true, dataListEmpresas, true, toolTipoFormat, dataLabelsFormat);

                    totalHorasEstimadasEngagement += registrosEngagement.First().total_horas_estimadas;

                    if (totalHorasEstimadasEngagement > 0)
                    {
                        Data dataEngagement = new Data(new List<Detalle>());
                        dataEngagement.id = $@"{idEmpleado}/{idEngagement}";
                        dataEngagement.name = nombreEngagement;
                        dataEngagement.drilldown = $@"{nombreEmpleado}/{nombreEngagement}";
                        dataEngagement.y = totalHorasEstimadasEngagement;
                        dataEngagement.y2 = totalHorasEjecutadasEngagement;
                        dataListEngagements.Add(dataEngagement);
                    }
                }

                string idSerieEngagement = nombreEmpleado;
                string nombreSerieEngagement = nombreEmpleado;

                this.cargarSerieColumn(idSerieEngagement, nombreSerieEngagement, true, dataListEngagements, true, toolTipoFormat, dataLabelsFormat);

                totalHorasAsignadasEmpleado = registrosEmpleado.Sum(r => r.horas_asignadas_empleado);

                if (totalHorasAsignadasEmpleado > 0)
                {
                    if (totalHorasAsignadasEmpleado > cantidadMayor)
                    {
                        cantidadMayor = totalHorasAsignadasEmpleado;
                    }

                    Data dataEmpleado = new Data(new List<Detalle>());
                    dataEmpleado.id = idEmpleado;
                    dataEmpleado.name = nombreEmpleado;
                    dataEmpleado.drilldown = nombreEmpleado;
                    dataEmpleado.y = totalHorasAsignadasEmpleado;
                    dataEmpleado.y2 = totalHorasEjecutadasEmpleado;
                    dataListEmpleados.Add(dataEmpleado);
                }
            }

            string idSerieEmpleado = "Empleados";
            string nombreSerieEmpleado = "Empleados";

            this.cargarSerieColumn(idSerieEmpleado, nombreSerieEmpleado, true, dataListEmpleados, false, toolTipoFormat, dataLabelsFormat);

            /* Serie linea (spline) */
            /*Serie serie = new Serie(new List<Data>());
            serie.id = "Horas tarea";
            serie.name = "<div id='divHoras' style='display:none;'>Horas tarea</div>";
            serie.type = "spline";
            serie.showInLegend = false;
            serie.data = dataListEmpleados;
            this.series.Add(serie);*/

            Eje x = new Eje(null);
            x.min = 0;
            x.max = groupByEmpleado.Count - 1;
            x.scrollbar = true;

            this.ejeX = x;

            Eje y = new Eje(null);
            y.min = 0;
            y.max = (int)cantidadMayor;
            y.formatLabels = "{value:,f} " + sufijo;

            this.ejeY = y;
        }

        public void cargarGraficoDashboardMarcacionEmpleado(List<ViewDashboardMarcacionEmpleadoReport> registros)
        {
            this.nombreGrafico = "Marcacion de Empleados";
            this.subtituloGrafico = "Navegue dando click en un indicador";
            this.tipoGrafico = "column";

            string sufijo = "Hrs.";
            double cantidadMayor = 0.0;
            string dataLabelsFormat = "{point.y:,.2f} " + sufijo;
            string toolTipoFormat = "{series.name}: <b>{point.y:,.2f} " + sufijo + "</b><br> ";// Presione para ver detalle";
            string dataLabelsFormatDrill = "{point.y:,.2f} {point.id}";
            string toolTipoFormatDrill = "{series.name}: <b>{point.y:,.2f} {point.id}</b> ";// Presione para generar reporte";

            // Agrupar por empleado
            var groupByEmpleado = registros.GroupBy(r => r.codigo_empleado).ToList();
            List<Data> dataListEmpleados = new List<Data>();

            foreach (var groupEmpleado in groupByEmpleado)
            {
                string codEmpleado = groupEmpleado.Key.ToString();
                string nombreEmpleado = "nombre empleado";

                double totalHorasAsistidoEmpleado = 0.0;
                double totalHorasRetrasosEmpleado = 0.0;

                List<ViewDashboardMarcacionEmpleadoReport> registrosEmpleado = groupEmpleado.ToList();

                var empleado = registrosEmpleado.First();
                nombreEmpleado = empleado.nombre_empleado;

                // Obtener total en horas
                totalHorasAsistidoEmpleado = Math.Round(registrosEmpleado.Sum(r => r.tiempo_asistido) / 60, 2);
                totalHorasRetrasosEmpleado = Math.Round(registrosEmpleado.Sum(r => r.tiempo_retraso) / 60, 2);

                if (totalHorasAsistidoEmpleado > 0)
                {
                    if (totalHorasAsistidoEmpleado > cantidadMayor)
                    {
                        cantidadMayor = totalHorasAsistidoEmpleado;
                    }

                    Data dataEmpleado = new Data(new List<Detalle>());
                    dataEmpleado.id = codEmpleado;
                    dataEmpleado.name = nombreEmpleado;
                    dataEmpleado.drilldown = nombreEmpleado;
                    dataEmpleado.y = totalHorasAsistidoEmpleado;
                    dataEmpleado.y2 = totalHorasRetrasosEmpleado;
                    dataListEmpleados.Add(dataEmpleado);
                }
            }

            string idSerieEmpleado = "Empleados";
            string nombreSerieEmpleado = "Horas asistencia";

            this.cargarSerieColumn(idSerieEmpleado, nombreSerieEmpleado, true, dataListEmpleados, false, toolTipoFormat, dataLabelsFormat);

            Eje x = new Eje(null);
            x.min = 0;
            x.max = groupByEmpleado.Count - 1;
            x.scrollbar = true;

            this.ejeX = x;

            Eje y = new Eje(null);
            y.min = 0;
            y.max = (int)cantidadMayor + 1;
            y.formatLabels = "{value:,f} " + sufijo;

            this.ejeY = y;
        }
    }
}