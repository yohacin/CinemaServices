using Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using EHE = Entities.Helpers;

namespace Business.Public
{
    public class DashboardPrincipal : Base
    {
        public DashboardPrincipal(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public List<EHE.ViewDashboardReport> GetListaFiltrada(List<long> idsEmpresa, DateTime desde, DateTime hasta)
        {
            try
            {
                string condicionEmpresas = "false";
                if (idsEmpresa.Count > 0)
                {
                    string ids = String.Join(',', idsEmpresa);
                    condicionEmpresas = $@" epr.id IN ({ids}) ";
                }

                string condicionFecha = "true";
                if (desde != null && hasta != null)
                {
                    condicionFecha = $@" (eng.desde BETWEEN '{desde:s}' AND '{hasta:s}' OR eng.hasta BETWEEN '{desde:s}' AND '{hasta:s}') ";
                }

                string sQuery = $@"
                    SELECT
	                    ROW_NUMBER() OVER (ORDER BY r.id_empresa) id,
	                    r.id_empresa,
	                    r.nombre_empresa,
	                    r.id_engagement,
	                    r.nombre_engagement,
	                    r.facturable,
	                    r.total_horas_estimadas,
	                    r.id_empleado,
	                    r.nombre_empleado,
	                    r.horas_asignadas_empleado,
	                    r.horas_ejecutadas_empleado
                    FROM (
                        SELECT
                            epr.id id_empresa,
                            epr.nombre nombre_empresa,
                            eng.id id_engagement,
                            eng.titulo nombre_engagement,
                            CASE WHEN eng.facturable THEN 'SI' ELSE'NO' END facturable,
                            COALESCE((
                                SELECT SUM(cantidad)
                                FROM public.detalle_hora_engagement
                                WHERE id_engagement = eng.id AND estado = 0
                            ), 0) total_horas_estimadas,
                            epl.id id_empleado,
                            CONCAT(epl.nombre, ' ', epl.apellido_paterno, ' ', epl.apellido_materno) nombre_empleado,
                            COALESCE((
                                SELECT SUM(cantidad_horas)
                                FROM public.detalle_engagement_empleado
                                WHERE id_engagement_empleado = eng_epl.id AND estado = 0
                            ) ,0) horas_asignadas_empleado,
                            COALESCE((
                                SELECT SUM(hora)
                                FROM public.empleado_hoja_trabajo
                                WHERE id_empleado = epl.id AND id_engagement = eng.id AND estado = 0
                            ), 0) horas_ejecutadas_empleado
                        FROM public.empresa epr
                        JOIN public.engagement eng ON eng.id_empresa = epr.id
                        JOIN public.engagement_empleado eng_epl ON eng_epl.id_engagement = eng.id
                        JOIN public.empleado epl ON epl.id = eng_epl.id_empleado
                        WHERE eng.estado = 0 AND eng_epl.estado = 0 AND epl.estado = 0
                            AND eng.estado_ejecucion <> 3 AND eng.estado = 0 AND {condicionEmpresas}
                            AND {condicionFecha}
                        ORDER BY epr.id, eng.id, epl.id
                    ) r
                ";
                return _DBcontext.ViewDashboardReport.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EHE.ViewDashboardReport> GetLista()
        {
            try
            {
                string sQuery = $@"
                    SELECT
	                    ROW_NUMBER() OVER (ORDER BY r.id_empresa) id,
	                    r.id_empresa,
	                    r.nombre_empresa,
	                    r.id_engagement,
	                    r.nombre_engagement,
	                    r.facturable,
	                    r.total_horas_estimadas,
	                    r.id_empleado,
	                    r.nombre_empleado,
	                    r.horas_asignadas_empleado,
	                    r.horas_ejecutadas_empleado
                    FROM (
                        SELECT
                            epr.id id_empresa,
                            epr.nombre nombre_empresa,
                            eng.id id_engagement,
                            eng.titulo nombre_engagement,
                            CASE WHEN eng.facturable THEN 'SI' ELSE'NO' END facturable,
                            COALESCE((
                                SELECT SUM(cantidad)
                                FROM public.detalle_hora_engagement
                                WHERE id_engagement = eng.id AND estado = 0
                            ), 0) total_horas_estimadas,
                            epl.id id_empleado,
                            CONCAT(epl.nombre, ' ', epl.apellido_paterno, ' ', epl.apellido_materno) nombre_empleado,
                            COALESCE((
                                SELECT SUM(cantidad_horas)
                                FROM public.detalle_engagement_empleado
                                WHERE id_engagement_empleado = eng_epl.id AND estado = 0
                            ) ,0) horas_asignadas_empleado,
                            COALESCE((
                                SELECT SUM(hora)
                                FROM public.empleado_hoja_trabajo
                                WHERE id_empleado = epl.id AND id_engagement = eng.id AND estado = 0
                            ), 0) horas_ejecutadas_empleado
                        FROM public.empresa epr
                        JOIN public.engagement eng ON eng.id_empresa = epr.id
                        JOIN public.engagement_empleado eng_epl ON eng_epl.id_engagement = eng.id
                        JOIN public.empleado epl ON epl.id = eng_epl.id_empleado
                        WHERE eng.estado = 0 AND eng_epl.estado = 0 AND epl.estado = 0
                            AND eng.estado_ejecucion <> 3 AND eng.estado = 0 --AND epr.id IN (1)
                        ORDER BY epr.id, eng.id, epl.id
                    ) r
                ";
                return _DBcontext.ViewDashboardReport.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EHE.ViewDashboardMarcacionEmpleadoReport> GetListaMarcacionEmpleados(List<long> idsEmpresa, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                string condicionIdsEmpresas = "false";
                if (idsEmpresa.Count > 0)
                {
                    string ids = String.Join(',', idsEmpresa);
                    condicionIdsEmpresas = $@" (mr.id_empresa_entrada IN ({ids}) OR mr.id_empresa_salida IN ({ids})) ";
                }

                string sQuery = $@"
                    SELECT
                        mr.ID,
                        emp.codigo AS codigo_empleado,
                        (
	                        emp.id || '-' || TRIM ( BOTH ' ' FROM emp.nombre ) || ' ' || TRIM ( BOTH ' ' FROM emp.apellido_paterno ) || ' ' || TRIM ( BOTH ' ' FROM emp.apellido_materno )
                        ) AS nombre_empleado,
                        COALESCE ( em_entrada.nombre, 'SIN DATOS' ) AS empresa_entrada,
                        COALESCE ( em_salida.nombre, 'SIN DATOS' ) AS empresa_salida,
                        tur.descripcion_turno,
                        mr.entrada_programada :: TEXT,
                        mr.salida_programada :: TEXT,
                        SUBSTRING ( COALESCE ( mr.entrada_real :: TEXT, 'SIN DATOS' ) FROM 0 FOR 20 ) AS entrada_real,
                        SUBSTRING ( COALESCE ( mr.salida_real :: TEXT, 'SIN DATOS' ) FROM 0 FOR 20 ) AS salida_real,
                        --ROUND(EXTRACT(EPOCH FROM (mr.salida_real - mr.entrada_real)) / 3600.00, 2) AS tiempo_asistido,
                        ROUND( ( EXTRACT ( EPOCH FROM ( mr.salida_real - mr.entrada_real ) ) / 60.00 ) :: NUMERIC, 2 ) AS tiempo_asistido,
                        CASE WHEN (mr.entrada_real > (mr.entrada_programada + (tur.tolerancia_entrada || ' minutes')::INTERVAL)) THEN
				                ROUND( ( EXTRACT ( EPOCH FROM ( mr.entrada_real - mr.entrada_programada) ) / 60.00 ) :: NUMERIC, 2 )
		                ELSE
			                0
		                END
		                 AS tiempo_retraso,
                        mr.latitud_entrada,
                        mr.longitud_entrada,
                        (
	                        CASE

			                WHEN ( mr.latitud_entrada = 0.0 ) THEN
			                'SIN PUNTO' ELSE (
			                CASE

					                WHEN ( geodistance ( mr.latitud_entrada, mr.longitud_entrada, em_entrada.latitud, em_entrada.longitud ) * 1609.34 ) <= em_entrada.perimetro THEN
					                'DENTRO DEL PERIMETRO' ELSE'FUERA DEL PERIMETRO'
				                END
				                )
			                END
			                ) AS observacion_punto_entrada,
			                mr.latitud_salida,
			                mr.longitud_salida,
			                (
			                CASE

					        WHEN ( mr.latitud_salida = 0.0 ) THEN
					        'SIN PUNTO' ELSE (
					        CASE

							        WHEN ( geodistance ( mr.latitud_salida, mr.longitud_salida, em_salida.latitud, em_salida.longitud ) * 1609.34 ) <= em_salida.perimetro THEN
							        'DENTRO DEL PERIMETRO' ELSE'FUERA DEL PERIMETRO'
						        END
						        )
					        END
					    ) AS observacion_punto_salida,
					    COALESCE ( em_entrada.perimetro, 0 ) AS perimetro_entrada,
					    COALESCE ( em_salida.perimetro, 0 ) AS perimetro_salida,
					    COALESCE ( mr.tipo_marcacion_entrada, ' ' ) AS tipo_marcacion_entrada,
					    COALESCE ( mr.tipo_marcacion_salida, ' ' ) AS tipo_marcacion_salida
				    FROM PUBLIC.marcacion mr
					INNER JOIN PUBLIC.empleado emp ON emp.ID = mr.id_empleado and emp.estado = 0
					INNER JOIN PUBLIC.turno tur ON tur.codigo_turno = mr.codigo_turno
					AND tur.id_empleado = mr.id_empleado
					LEFT JOIN PUBLIC.empresa em_entrada ON em_entrada.ID = mr.id_empresa_entrada
					LEFT JOIN PUBLIC.empresa em_salida ON em_salida.ID = mr.id_empresa_salida
				    WHERE
					    mr.entrada_programada :: DATE BETWEEN @fechaInicio AND @fechaFin
                        AND {condicionIdsEmpresas}
					    AND mr.salida_real IS NOT NULL
					    AND mr.entrada_real IS NOT NULL
				    ORDER BY
				        nombre_empleado, mr.entrada_programada
                ";

                // Usa parámetros para evitar inyección SQL
                var paramters = new[] {
                    new NpgsqlParameter("@fechaInicio", NpgsqlTypes.NpgsqlDbType.Date) { Value = fechaInicio.Date },
                    new NpgsqlParameter("@fechaFin", NpgsqlTypes.NpgsqlDbType.Date) { Value = fechaFin.Date }
                };
                var listaMarcacion = _DBcontext.ViewDashboardMarcacionEmpleadoReport
                    .FromSqlRaw(sQuery, paramters)
                    .ToList();
                return listaMarcacion;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}