var myChart; // Variable global del grafico creado
var objetoGrafico; // Variable global del objeto utilizado para dibujar un grafico
var anioReporte; // Año que se usara para generar el reporñte
var codigoContacto;  // Codigo del contacto que se usara para generar el reporte
var tipoGrafico;    // Id del grafico actual que se usara para generar los reportes
var redrawEnabled = true; // variable usada para comprobar si se repinta de nuevo

/*-------------------------------------------------------------------------------------------*/
/* Eventos utilizado para realizar la exportacion del grafico en formato PDF, PNG y JPEG */
/*-------------------------------------------------------------------------------------------*/

$("#download-chart-png").click(function () {
    myChart.exportChart();
});

$("#download-chart-jpeg").click(function () {
    myChart.exportChart({
        type: 'image/jpeg',
        filename: 'chart'
    });
});

$("#download-chart-pdf").click(function () {
    myChart.exportChart({
        type: 'application/pdf',
        filename: 'chart-pdf'
    });
});

const TYPE_EMPRESA = 1;
const TYPE_EMPLEADO = 2;

/*-------------------------------------------------------------------------------------------*/

function actualizarGrafico(graphicObject, container = 'container', type = TYPE_EMPRESA) {
    var g = graphicObject;
    var options = {
        title: {
            text: g.nombreGrafico
        },
        subtitle: {
            text: g.subtitutloGrafico
        },
        chart: {
            height: 460,
            renderTo: container, //'contenedor'+nro,
            type: g.tipoGrafico,
        },
        credits: {
            enabled: false
        },
        navigation: {
            buttonOptions: {
                enabled: false
            }
        },
        exporting: {
            chartOptions: {
                chart: {
                    width: 1600,
                    height: 900
                }
            }
        },
        xAxis: {
            min: (g.ejeX.min != null) ? g.ejeX.min : undefined,
            max: (g.ejeX.max != null) ? g.ejeX.max : undefined,
            align: 'center',
            categories: g.ejeX.categorias,    // En caso de que sea un grafico Pie este valor esta vacio []
            title: {
                text: (g.ejeX.titulo != null) ? g.ejeX.titulo : "",
                margin: 15
            },
            style: {
                fontSize: '15px',
                fontFamily: 'Verdana, sans-serif'
            },
            scrollbar: {
                enabled: g.ejeX.scrollbar
            }
        },
        yAxis: {
            min: (g.ejeY.min != null) ? g.ejeY.min : undefined,
            max: (g.ejeY.max != null) ? g.ejeY.max : undefined,
            title: {
                text: g.ejeY.titulo
            },
            labels: {
                overflow: 'justify',
                format: (g.ejeY.formatLabels != null) ? g.ejeY.formatLabels : "{value}",
                style: {
                    fontSize: '10px',
                    fontFamily: 'Verdana, sans-serif'
                }
            },
            stackLabels: {
                enabled: false,
                style: {
                    fontSize: '10px',
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        legend: {
            layout: (g.leyenda.sentido != null) ? g.leyenda.sentido : "horizontal",
            align: (g.leyenda.alineacion != null) ? g.leyenda.alineacion : "center",
            verticalAlign: (g.leyenda.alineacionVertical != null) ? g.leyenda.alineacionVertical : "bottom",
            x: (g.leyenda.x != null) ? g.leyenda.x : 0,
            y: (g.leyenda.y != null) ? g.leyenda.y : 0,
            floating: (g.leyenda.flotante != null) ? g.leyenda.flotante : false,
            backgroundColor: (g.leyenda.colorFondo != null) ? g.leyenda.colorFondo : undefined, //(Highcharts.theme && Highcharts.theme.legendBackgroundColor || "#f6ffff"),
            borderColor: (g.leyenda.colorBorde != null) ? g.leyenda.colorBorde : "#999",

            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            },
            borderWidth: 1,
            shadow: false,
        },
        series: [],
        drilldown: {
            allowPointDrilldown: false,
            drillUpButton: {
                position: {
                    x: -50,
                    y: -45
                }
            },
            series: []
        }
    };

    Highcharts.setOptions({
        lang: {
            drillUpText: "Volver atras",
            thousandsSep: ','
        }
    });

    g.series.forEach(s => {
        var serie = {
            id: s.id,
            type: s.type,
            name: 'Horas Estimadas',
            //color: s.color,
            color: '#006BCC',
            zIndex: 1,
            center: s.center,
            size: s.size,
            showInLegend: s.showInLegend,
            tooltip: {
                pointFormat: s.tooltipFormat != null ? s.tooltipFormat : '{series.name}: <b>{point.y}</b><br/>.'
            },
            data: s.data,
            dataLabels: {
                format: s.dataLabelsFormat != null ? s.dataLabelsFormat : undefined
            }
        }

        if (s.type == "column") {
            const data = s.data.map(data => {
                let dataY2 = Object.assign({}, data);
                dataY2.y = data.y2;
                dataY2.drilldown = `${data.drilldown}-ejecutados`;
                return dataY2;
            });
            const serieY2 = {
                id: `${s.id}-ejecutados`,
                type: s.type,
                name: 'Horas Ejecutadas',
                color: '#CCAA0A',
                zIndex: 2,
                center: s.center,
                size: s.size,
                pointPadding: 0.3,
                showInLegend: s.showInLegend,
                tooltip: {
                    pointFormat: s.tooltipFormat != null ? s.tooltipFormat : '{series.name}: <b>{point.y}</b><br/>.'
                },
                data: data,
                dataLabels: {
                    format: s.dataLabelsFormat != null ? s.dataLabelsFormat : undefined
                }
            }

            options.series.push(serieY2);
        } else {
            serie.color = '#CCAA0A';
            serie.zIndex = 3;
        }

        options.series.push(serie);
    })
    if (g.drilldown.series != null) {
        g.drilldown.series.forEach(s => {
            var serie = {
                id: s.id,
                type: s.type,
                name: 'Horas Establecidas',
                //color: s.color,
                color: '#006BCC',
                zIndex: 1,
                center: s.center,
                size: s.size,
                showInLegend: s.showInLegend,
                tooltip: {
                    pointFormat: s.tooltipFormat != null ? s.tooltipFormat : '{series.name}: <b>{point.y}</b><br/>.'
                },
                data: s.data,
                dataLabels: {
                    format: s.dataLabelsFormat != null ? s.dataLabelsFormat : undefined
                }
            }

            if (s.type == "column") {
                const data = s.data.map(data => {
                    let dataY2 = Object.assign({}, data);
                    dataY2.y = data.y2;
                    dataY2.drilldown = `${data.drilldown}-ejecutados`;
                    return dataY2;
                });

                const serieY2 = {
                    id: `${s.id}-ejecutados`,
                    type: s.type,
                    name: 'Horas Ejecutadas',
                    color: '#CCAA0A',
                    zIndex: 2,
                    center: s.center,
                    pointPadding: 0.3,
                    size: s.size,
                    showInLegend: s.showInLegend,
                    tooltip: {
                        pointFormat: s.tooltipFormat != null ? s.tooltipFormat : '{series.name}: <b>{point.y}</b><br/>.'
                    },
                    data: data,
                    dataLabels: {
                        format: s.dataLabelsFormat != null ? s.dataLabelsFormat : undefined
                    }
                }

                options.drilldown.series.push(serieY2);
            }

            options.drilldown.series.push(serie);
        });
    }

    graficoVentas(options);

    myChart = Highcharts.chart(options);

    if (type === TYPE_EMPRESA) {
        myChart.renderer.button('Generar Reporte', null, null)
            .attr({
                zIndex: 3,
                align: 'right',
                title: 'Click para generar reporte'
            }).on('click', function () {
                //console.debug('generar reporte click: ', myChart);
                console.debug('levels: ', myChart.drilldownLevels);
                const levelsInfo = myChart.drilldownLevels;
                let level = myChart.series[0].options._levelNumber ? myChart.series[0].options._levelNumber : 0;
                let levelInfo;
                if (levelsInfo) {
                    levelInfo = levelsInfo.find(item => item.levelNumber == level - 1);
                }

                console.debug('level: ', level);
                switch (level) {
                    case 0:
                        generarReporteGeneral();
                        break;
                    case 1:
                        generarReportePorEmpresa(parseInt(levelInfo.pointOptions.id));
                        break;
                    case 2:
                        const ids = levelInfo.pointOptions.id.split('/');
                        generaReportePorEngagement(parseInt(ids[0]), parseInt(ids[1]));
                        break;
                }
            }).add(myChart.zoomGroupButton).align({
                align: 'right',
                x: -160,
                y: 8
            }, false, null);
    }

    if (type === TYPE_EMPLEADO) {
        myChart.renderer.button('Generar Reporte', null, null)
            .attr({
                zIndex: 3,
                align: 'right',
                title: 'Click para generar reporte'
            }).on('click', function () {
                //console.debug('generar reporte click: ', myChart);
                console.debug('levels: ', myChart.drilldownLevels);
                const levelsInfo = myChart.drilldownLevels;
                let level = myChart.series[0].options._levelNumber ? myChart.series[0].options._levelNumber : 0;
                let levelInfo;
                if (levelsInfo) {
                    levelInfo = levelsInfo.find(item => item.levelNumber == level - 1);
                }

                console.debug('level: ', level);
                switch (level) {
                    case 0:
                        generarReporteGeneral();
                        break;
                    case 1:
                        generarReportePorEmpleado(parseInt(levelInfo.pointOptions.id));
                        break;
                    case 2:
                        const ids = levelInfo.pointOptions.id.split('/');
                        generaReportePorEngagement(parseInt(ids[0]), parseInt(ids[1]));
                        break;
                }
            }).add(myChart.zoomGroupButton).align({
                align: 'right',
                x: -160,
                y: 8
            }, false, null);
    }
}

function actualizarGraficoDashboardMarcacionEmpleados(graphicObject, container = 'container', type = TYPE_EMPRESA) {
    var g = graphicObject;
    var options = {
        title: {
            text: g.nombreGrafico
        },
        subtitle: {
            text: g.subtitutloGrafico
        },
        chart: {
            height: 460,
            renderTo: container, //'contenedor'+nro,
            type: g.tipoGrafico,
        },
        credits: {
            enabled: false
        },
        navigation: {
            buttonOptions: {
                enabled: false
            }
        },
        exporting: {
            chartOptions: {
                chart: {
                    width: 1600,
                    height: 900
                }
            }
        },
        xAxis: {
            min: (g.ejeX.min != null) ? g.ejeX.min : undefined,
            max: (g.ejeX.max != null) ? g.ejeX.max : undefined,
            align: 'center',
            categories: g.ejeX.categorias,    // En caso de que sea un grafico Pie este valor esta vacio []
            title: {
                text: (g.ejeX.titulo != null) ? g.ejeX.titulo : "",
                margin: 15
            },
            style: {
                fontSize: '15px',
                fontFamily: 'Verdana, sans-serif'
            },
            scrollbar: {
                enabled: g.ejeX.scrollbar
            }
        },
        yAxis: {
            min: (g.ejeY.min != null) ? g.ejeY.min : undefined,
            max: (g.ejeY.max != null) ? g.ejeY.max : undefined,
            title: {
                text: g.ejeY.titulo
            },
            labels: {
                overflow: 'justify',
                format: (g.ejeY.formatLabels != null) ? g.ejeY.formatLabels : "{value}",
                style: {
                    fontSize: '10px',
                    fontFamily: 'Verdana, sans-serif'
                }
            },
            stackLabels: {
                enabled: true,
                style: {
                    fontSize: '10px',
                    fontWeight: 'bold',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'gray'
                }
            }
        },
        legend: {
            layout: (g.leyenda.sentido != null) ? g.leyenda.sentido : "horizontal",
            align: (g.leyenda.alineacion != null) ? g.leyenda.alineacion : "center",
            verticalAlign: (g.leyenda.alineacionVertical != null) ? g.leyenda.alineacionVertical : "bottom",
            x: (g.leyenda.x != null) ? g.leyenda.x : 0,
            y: (g.leyenda.y != null) ? g.leyenda.y : 0,
            floating: (g.leyenda.flotante != null) ? g.leyenda.flotante : false,
            backgroundColor: (g.leyenda.colorFondo != null) ? g.leyenda.colorFondo : undefined, //(Highcharts.theme && Highcharts.theme.legendBackgroundColor || "#f6ffff"),
            borderColor: (g.leyenda.colorBorde != null) ? g.leyenda.colorBorde : "#999",

            itemMarginTop: 5,
            itemMarginBottom: 5,
            itemStyle: {
                lineHeight: '14px'
            },
            borderWidth: 1,
            shadow: false,
        },
        plotOptions: {
            column: {
                stacking: 'normal'
            }
        },
        series: [],
        drilldown: {
            allowPointDrilldown: false,
            drillUpButton: {
                position: {
                    x: -50,
                    y: -45
                }
            },
            series: []
        }
    };

    Highcharts.setOptions({
        lang: {
            drillUpText: "Volver atras",
            thousandsSep: ','
        }
    });

    g.series.forEach(s => {
        var serie = {
            name: s.name,
            color: s.color,
            //color: '#006BCC',
            stack: s.stack,
            center: s.center,
            zIndex: 1,
            showInLegend: s.showInLegend,
            tooltip: {
                pointFormat: s.tooltipFormat != null ? s.tooltipFormat : '{series.name}: <b>{point.y}</b><br/>.'
            },
            data: s.data,
            dataLabels: {
                format: s.dataLabelsFormat != null ? s.dataLabelsFormat : undefined
            }
        }

        if (s.type == "column") {
            const data = s.data.map(data => {
                let dataY2 = Object.assign({}, data);
                dataY2.y = data.y2;
                dataY2.drilldown = `${data.drilldown}-retrasados`;
                return dataY2;
            });
            const serieY2 = {
                name: 'Horas Retraso',
                color: '#FF0000',
                zIndex: 2,
                stack: s.stack,
                pointPadding: 0.3,
                showInLegend: s.showInLegend,
                tooltip: {
                    pointFormat: s.tooltipFormat != null ? s.tooltipFormat : '{series.name}: <b>{point.y}</b><br/>.'
                },
                data: data,
                dataLabels: {
                    format: s.dataLabelsFormat != null ? s.dataLabelsFormat : undefined
                }
            }

            options.series.push(serieY2);
        } else {
            serie.color = '#CCAA0A';
            serie.zIndex = 3;
        }

        options.series.push(serie);
    })
    if (g.drilldown.series != null) {
        g.drilldown.series.forEach(s => {
            var serie = {
                id: s.id,
                type: s.type,
                name: 'Horas Establecidas',
                //color: s.color,
                color: '#006BCC',
                zIndex: 1,
                center: s.center,
                size: s.size,
                showInLegend: s.showInLegend,
                tooltip: {
                    pointFormat: s.tooltipFormat != null ? s.tooltipFormat : '{series.name}: <b>{point.y}</b><br/>.'
                },
                data: s.data,
                dataLabels: {
                    format: s.dataLabelsFormat != null ? s.dataLabelsFormat : undefined
                }
            }

            if (s.type == "column") {
                const data = s.data.map(data => {
                    let dataY2 = Object.assign({}, data);
                    dataY2.y = data.y2;
                    dataY2.drilldown = `${data.drilldown}-ejecutados`;
                    return dataY2;
                });

                const serieY2 = {
                    id: `${s.id}-ejecutados`,
                    type: s.type,
                    name: 'Horas Ejecutadas',
                    color: '#CCAA0A',
                    zIndex: 2,
                    center: s.center,
                    pointPadding: 0.3,
                    size: s.size,
                    showInLegend: s.showInLegend,
                    tooltip: {
                        pointFormat: s.tooltipFormat != null ? s.tooltipFormat : '{series.name}: <b>{point.y}</b><br/>.'
                    },
                    data: data,
                    dataLabels: {
                        format: s.dataLabelsFormat != null ? s.dataLabelsFormat : undefined
                    }
                }

                options.drilldown.series.push(serieY2);
            }

            options.drilldown.series.push(serie);
        });
    }

    graficoVentas(options);

    myChart = Highcharts.chart(options);

    if (type === TYPE_EMPRESA) {
        myChart.renderer.button('Generar Reporte', null, null)
            .attr({
                zIndex: 3,
                align: 'right',
                title: 'Click para generar reporte'
            }).on('click', function () {
                //console.debug('generar reporte click: ', myChart);
                console.debug('levels: ', myChart.drilldownLevels);
                const levelsInfo = myChart.drilldownLevels;
                let level = myChart.series[0].options._levelNumber ? myChart.series[0].options._levelNumber : 0;
                let levelInfo;
                if (levelsInfo) {
                    levelInfo = levelsInfo.find(item => item.levelNumber == level - 1);
                }

                console.debug('level: ', level);
                switch (level) {
                    case 0:
                        generarReporteGeneral();
                        break;
                    case 1:
                        generarReportePorEmpresa(parseInt(levelInfo.pointOptions.id));
                        break;
                    case 2:
                        const ids = levelInfo.pointOptions.id.split('/');
                        generaReportePorEngagement(parseInt(ids[0]), parseInt(ids[1]));
                        break;
                }
            }).add(myChart.zoomGroupButton).align({
                align: 'right',
                x: -160,
                y: 8
            }, false, null);
    }

    if (type === TYPE_EMPLEADO) {
        myChart.renderer.button('Generar reporte', null, null)
            .attr({
                zIndex: 3,
                align: 'right',
                title: 'Click para generar reporte'
            }).on('click', function () {
                //console.debug('generar reporte click: ', myChart);
                console.debug('levels: ', myChart.drilldownLevels);
                const levelsInfo = myChart.drilldownLevels;
                let level = myChart.series[0].options._levelNumber ? myChart.series[0].options._levelNumber : 0;
                let levelInfo;
                if (levelsInfo) {
                    levelInfo = levelsInfo.find(item => item.levelNumber == level - 1);
                }

                console.debug('level: ', level);
                switch (level) {
                    case 0:
                        generarReporteGeneral();
                        break;
                    case 1:
                        generarReportePorEmpleado(parseInt(levelInfo.pointOptions.id));
                        break;
                    case 2:
                        const ids = levelInfo.pointOptions.id.split('/');
                        generaReportePorEngagement(parseInt(ids[0]), parseInt(ids[1]));
                        break;
                }
            }).add(myChart.zoomGroupButton).align({
                align: 'right',
                x: -160,
                y: 8
            }, false, null);
    }
}

function graficoVentas(options) {
    console.debug('changedOptions: ', options);
    options.chart.events = {
        drilldown: function (e) {
            setTimeout(function () {
                console.debug('eventDrilldown: ', e);
                if (e.category != undefined) {
                    var cantidadMaxima = 5;
                    var cantidad = 0;
                    let values = [];
                    e.target.xAxis[0].series.forEach(s => {
                        if (s.yData.length > cantidad) {
                            cantidad = s.yData.length
                        }
                        s.yData.forEach(value => {
                            values.push(value);
                        })
                    });

                    if (cantidad <= cantidadMaxima) {
                        cantidadMaxima = cantidad - 1
                    }
                    let max = values.reduce((acc, val) => (acc > val) ? acc : val, 0);
                    myChart.yAxis[0].update({ min: 0, max: max });
                    myChart.xAxis[0].update({ min: 0, max: cantidadMaxima });
                } else {
                    var cantidadMaxima = 5; //this.xAxis[0].max; Este valor por ahora se esta colocando en duro, lo ideal es que se lo tome desde el grafico
                    let values = [];

                    e.seriesOptions.data.forEach(d => {
                        values.push(d.y);
                    });

                    if (e.seriesOptions.data.length <= cantidadMaxima) {
                        cantidadMaxima = e.seriesOptions.data.length - 1
                    }

                    let max = values.reduce((acc, val) => (acc > val) ? acc : val, 0);
                    myChart.yAxis[0].update({ min: 0, max: max });
                    myChart.xAxis[0].update({ min: 0, max: cantidadMaxima });
                }
            }, 20);
        },
        drillup: function (e) {
            setTimeout(function () {
                console.debug('eventDrillup: ', e);
                var cantidadMaxima = 5; //this.xAxis[0].max; Este valor por ahora se esta colocando en duro, lo ideal es que se lo tome desde el grafico
                var cantidad = 0;
                let values = [];

                myChart.series.forEach(s => {
                    if (s.visible && s.type == "column") {
                        if (s.yData.length > cantidad) {
                            cantidad = s.yData.length;
                        }
                        s.yData.forEach(value => {
                            values.push(value);
                        })
                    }
                })
                if (cantidad < cantidadMaxima) {
                    cantidadMaxima = cantidad - 1
                }
                let max = values.reduce((acc, val) => (acc > val) ? acc : val, 0);
                myChart.yAxis[0].update({ min: 0, max: max });
                myChart.xAxis[0].update({ min: 0, max: cantidadMaxima });
            }, 20);
        }
    };
    options.plotOptions = {
        series: {
            events: {
                legendItemClick: function (e) {
                    setTimeout(function () {
                        let valores = [];
                        myChart.series.forEach(function (serie) {
                            if (serie.visible && serie.type == "column") {
                                serie.yData.forEach(function (value) {
                                    valores.push(value);
                                })
                            }
                        })
                        let max = valores.reduce((acc, val) => (acc > val) ? acc : val, 0);
                        myChart.yAxis[0].update({ min: 0, max: max });
                    }, 20)
                },
                click: function (e) {
                    if (e.point.drilldown == undefined) {
                        if (!e.point.datoExtra) {
                            return;
                        }
                        var datos = e.point.datoExtra.split("/");
                        var mes = parseInt(datos[0]) - 1;
                        var local = parseInt(datos[1]);
                        var vendedor = parseInt(datos[2]);
                        var moneda = parseInt(datos[3]);
                        var year = new Date().getFullYear();
                        var fechaIni = new Date(year, mes, 1);
                        var fechaFin = new Date(year, mes + 1, 0);
                        generarReporteCrecimientoVentas(fechaIni.toISOString(), fechaFin.toISOString(), local, vendedor)
                    }
                }
            }
        },
        column: {
            cursor: 'pointer',
            cropThreshold: 1000,    // Hay que asegurarse de que este valor sea mayor a la cantidad de puntos que se deben graficar en un eje
            dataLabels: {
                enabled: true,
                inside: false,
                format: '{point.y:,.2f}',   // Definido en el objeto Chart
                style: {
                    color: "black"
                }
            },
            grouping: false, // para solapar columnas series
        },
        spline: {
            tooltip: {
                pointFormatter: function () {
                    var text = false;
                    var prevPoint = this.x == 0 ? null : this.series.points[this.x - 1];
                    text = 'Inicial';
                    if (prevPoint) {
                        text = 'Crecimiento: ' + Highcharts.numberFormat((((this.y / prevPoint.y) - 1) * 100), 2) + '%';
                    }
                    return text;
                }
            }
        }
    };

    options.xAxis.type = 'category';
}