using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPU = Entities.Public;

namespace Business.Public
{
    public class ViewMarcacion : Base
    {
        public ViewMarcacion(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public List<Entities.Helpers.MarcacionHelpers> listarMarcaciones(long id_empleado, DateTime pFechaInicio, DateTime pFechaFin)
        {
            try
            {
                string sQuery = $@"SELECT mr. id, emp.codigo as codigo_empleado,(emp.nombre || ' ' || emp.apellido_paterno || ' ' || emp.apellido_materno) as nombre_empleado,
                                             COALESCE(em_entrada.nombre,'SIN DATOS') as empresa_entrada, COALESCE(em_salida.nombre,'SIN DATOS') as empresa_salida,
                                             tur.descripcion_turno, mr.entrada_programada :: TEXT,mr.salida_programada :: TEXT,
                                             COALESCE(mr.entrada_real :: TEXT,'SIN DATOS') as entrada_real,
                                             COALESCE(mr.salida_real :: TEXT,'SIN DATOS') as salida_real,
                                             mr.latitud_entrada, mr.longitud_entrada,
                                             (CASE
                                                WHEN(mr.latitud_entrada = 0.0) THEN 'SIN PUNTO'
                                                ELSE ( CASE
                                                            WHEN(geodistance(mr.latitud_entrada,mr.longitud_entrada,em_entrada.latitud,em_entrada.longitud) * 1609.34
                                                                ) <= em_entrada.perimetro THEN 'DENTRO DEL PERIMETRO'
                                                            ELSE 'FUERA DEL PERIMETRO'
                                                        END)
                                              END) as observacion_punto_entrada,
                                             mr.latitud_salida,mr.longitud_salida,
                                             (CASE
                                                    WHEN(mr.latitud_salida = 0.0) THEN 'SIN PUNTO'
                                                    ELSE ( CASE
                                                                WHEN(geodistance(mr.latitud_salida,mr.longitud_salida,em_salida.latitud,em_salida.longitud) * 1609.34
                                                                    ) <= em_salida.perimetro THEN 'DENTRO DEL PERIMETRO'
                                                                ELSE 'FUERA DEL PERIMETRO'
                                                            END)
                                              END) as observacion_punto_salida,
                                             COALESCE(em_entrada.perimetro,0) as perimetro_entrada,
                                             COALESCE(em_salida.perimetro,0) as perimetro_salida,
                                             COALESCE(mr.tipo_marcacion_entrada, ' ') as tipo_marcacion_entrada,
                                             COALESCE(mr.tipo_marcacion_salida, ' ') as tipo_marcacion_salida
                                      FROM PUBLIC.marcacion mr
                                                INNER JOIN PUBLIC .empleado emp ON emp. ID = mr.id_empleado and emp.id={id_empleado}
                                                INNER JOIN PUBLIC .turno tur ON tur.codigo_turno = mr.codigo_turno
                                                            AND tur.id_empleado = mr.id_empleado
                                                LEFT JOIN PUBLIC .empresa em_entrada on em_entrada. id = mr.id_empresa_entrada
                                                LEFT JOIN PUBLIC .empresa em_salida on em_salida. id = mr.id_empresa_salida
                                      WHERE
                                              mr.entrada_programada :: DATE BETWEEN DATE('{pFechaInicio.ToString("yyyy-MM-dd")}')
                                              AND DATE('{pFechaFin.ToString("yyyy-MM-dd")}')
                                      ORDER BY
                                               mr.entrada_programada, em_entrada.nombre, emp.nombre";

                var listaMarcacion = _DBcontext.MarcacionHelpers.FromSqlRaw(sQuery).ToList();

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