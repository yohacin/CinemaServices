using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using EPU = Entities.Public;

namespace Business.Public
{
    public class Marcacion : Base, IBusiness<EPU.Marcacion>
    {
        public Marcacion(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.Marcacion eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Marcacion.Add(eEntidad);
                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }

        public bool GuardarMarcacion(EPU.Marcacion eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    EPU.Marcacion eMarcacionRepetida = this._DBcontext.Marcacion.FirstOrDefault(e => e.id_empleado == eEntidad.id_empleado && e.codigo_turno == eEntidad.codigo_turno && e.entrada_programada.Date == DateTime.Now.Date);
                    if (eMarcacionRepetida == null) return false;

                    bool isPrimerEntrada = false;
                    bool isSalidaFinal = false;

                    //-------Logica de Arnol

                    //List<Marcacion> lstMarcacionesProgramadas = listarMarcacionesEmpleado(marcacion.getId_empleado(), marcacion.getEntrada_real());
                    //int marcacionesSalidaEsperadas = lstMarcacionesProgramadas.size();
                    //int cantidadEntradas = 0;
                    //int cantidadSalidas = 0;
                    //foreach (Marcacion iter in lstMarcacionesProgramadas)
                    //{
                    //    if ((iter.getEntrada_movil() != null))
                    //    {
                    //        cantidadEntradas++;
                    //    }

                    //    if ((iter.getSalida_movil() != null))
                    //    {
                    //        cantidadSalidas++;
                    //    }

                    //}

                    //if ((cantidadEntradas == 0))
                    //{
                    //    isPrimerEntrada = true;
                    //}

                    //if (((marcacion.getSalida_movil() != null)
                    //            && ((marcacionesSalidaEsperadas - cantidadSalidas)
                    //            == 1)))
                    //{
                    //    isSalidaFinal = true;
                    //}

                    //if (isPrimerEntrada)
                    //{
                    //    Calendar calendarFechaVerificacion = Calendar.getInstance(TimeZone.getTimeZone(Constantes.TIME_ZONE));
                    //    calendarFechaVerificacion.add(Calendar.DATE, -1);
                    //    DiaFeriado diaFeriado = getDiaFeriado(calendarFechaVerificacion.getTime());
                    //    while ((diaFeriado != null))
                    //    {
                    //        calendarFechaVerificacion.add(Calendar.DATE, -1);
                    //        diaFeriado = getDiaFeriado(calendarFechaVerificacion.getTime());
                    //    }

                    //    if ((calendarFechaVerificacion.get(Calendar.DAY_OF_WEEK) == Calendar.SUNDAY))
                    //    {
                    //        calendarFechaVerificacion.add(Calendar.DATE, -2);
                    //    }

                    //    if ((calendarFechaVerificacion.get(Calendar.DAY_OF_WEEK) == Calendar.SATURDAY))
                    //    {
                    //        calendarFechaVerificacion.add(Calendar.DATE, -1);
                    //    }

                    //    //  verificando las horas de trabajo del dia anterior
                    //    List<EmpleadoHojaTrabajo> lstHojasTrabajo = getHojasTrabajo(marcacion.getId_empleado(), calendarFechaVerificacion.getTime());
                    //    Double horasTrabajadasDiaAnterior = 0;
                    //    if (!lstHojasTrabajo.isEmpty())
                    //    {
                    //        horasTrabajadasDiaAnterior = sumFrom(lstHojasTrabajo).getHora();
                    //    }

                    //    SimpleDateFormat df = new SimpleDateFormat(Constantes.FORMATO_FECHA_BD);
                    //    if ((horasTrabajadasDiaAnterior < Constantes.HORAS_MINIMA_TRABAJO_DIARIO))
                    //    {
                    //        throw new Exception(String.format("No es posible marcar la entrada. Las horas trabajadas en fecha %s es menor a %s. Favor de registrar s" +
                    //                "us horas de trabajo en la fecha especificada.", df.format(calendarFechaVerificacion.getTime()), Constantes.HORAS_MINIMA_TRABAJO_DIARIO));
                    //    }

                    //}

                    //if (isSalidaFinal)
                    //{
                    //    //  verificando si subio sus datos de marcacion
                    //    List<EmpleadoHojaTrabajo> lstHojasTrabajo = getHojasTrabajo(marcacion.getId_empleado(), marcacion.getSalida_movil());
                    //    Double horasTrabajadas = 0;
                    //    if (!lstHojasTrabajo.isEmpty())
                    //    {
                    //        horasTrabajadas = sumFrom(lstHojasTrabajo).getHora();
                    //    }

                    //    SimpleDateFormat df = new SimpleDateFormat(Constantes.FORMATO_FECHA_BD);
                    //    if ((horasTrabajadas < Constantes.HORAS_MINIMA_TRABAJO_DIARIO))
                    //    {
                    //        throw new Exception(String.format("No es posible marcar la salida. Las horas de trabajo registradas en fecha %s son menor a %s", df.format(marcacion.getSalida_movil()), Constantes.HORAS_MINIMA_TRABAJO_DIARIO));
                    //    }

                    //}

                    //marcacion.setId(programada.getId());
                    //marcacion.setEntrada_programada(programada.getEntrada_programada());
                    //marcacion.setSalida_programada(programada.getSalida_programada());
                    //marcacion.setAction(Action.Update);
                    //if ((marcacion.getEntrada_real() != null))
                    //{
                    //    marcacion.setTipo_marcacion_entrada("MOVIL");
                    //}

                    //if ((marcacion.getSalida_real() != null))
                    //{
                    //    marcacion.setTipo_marcacion_salida("MOVIL");
                    //}

                    //this.save(marcacion);
                    //  /** NO HA MARCADO NADA, y SOLO MARCO ENTRADA **/
                    //  if (programada.getEntrada_movil() == null
                    //  && programada.getSalida_movil() == null) {
                    //  marcacion.setId(programada.getId());
                    //  marcacion.setEntrada_movil(marcacion.getFecha_marcacion());
                    //  marcacion.setEntrada_real(marcacion.getFecha_marcacion_real());
                    //  marcacion.setLatitud_entrada(marcacion.getLatitud_movil());
                    //  marcacion.setLongitud_entrada(marcacion.getLongitud_movil());
                    //  marcacion.setAction(Action.Update);
                    //  this.save(marcacion);
                    //
                    //  }
                    //
                    //  /** YA SE MARCO LA ENTRADA MOVIL, SOLO SE MARCA SALIDA **/
                    //  if (programada.getEntrada_movil() != null
                    //  && programada.getSalida_movil() == null) {
                    //  marcacion.setId(programada.getId());
                    //  marcacion.setSalida_movil(marcacion.getFecha_marcacion());
                    //  marcacion.setSalida_real(marcacion.getFecha_marcacion_real());
                    //  marcacion.setLatitud_salida(marcacion.getLatitud_movil());
                    //  marcacion.setLongitud_salida(marcacion.getLongitud_movil());
                    //  marcacion.setAction(Action.Update);
                    //  this.save(marcacion);
                    //  }

                    this._DBcontext.Marcacion.Add(eEntidad);
                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }

        public bool Modificar(EPU.Marcacion eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Marcacion.Update(eEntidad);
                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }

        public bool Eliminar(long id)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    EPU.Marcacion eEntidad = this.GetEntity(id);
                    if (eEntidad == null) throw new Exception("Marcación Inexistente!!...");

                    this._DBcontext.Marcacion.Remove(eEntidad);
                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }

        public EPU.Marcacion GetEntity(long id)
        {
            try
            {
                return _DBcontext.Marcacion.FirstOrDefault(p => p.id == id);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Marcacion> listarMarcacionesEmpleado(long id_empleado, DateTime fecha)
        {
            try
            {
                string sQuery = $@"
                    SELECT *
                    FROM public.marcacion
                    WHERE id_empleado={id_empleado} and entrada_programada::DATE = '{fecha.ToString("yyyy-MM-dd")}'
                    ORDER by entrada_programada;
                ";
                return _DBcontext.Marcacion.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Marcacion GetEntityByTurno(long id_empleado, long codigo_turno, DateTime fecha)
        {
            try
            {
                return _DBcontext.Marcacion.FirstOrDefault(p => p.id_empleado == id_empleado && p.codigo_turno == codigo_turno && p.entrada_programada.Date == fecha.Date);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Foto_Marcacion GetEntityFotMarcacion(long id_marcacion, int posicion)
        {
            try
            {
                return _DBcontext.Foto_Marcacion.FirstOrDefault(p => p.id_marcacion == id_marcacion && p.posicion == posicion);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
        public List<EPU.Foto_Marcacion> GetListaXMarcaciones(List<long> IdsMarcacion)
        {
            try
            {
                return _DBcontext.Foto_Marcacion.Where(p => IdsMarcacion.Contains(p.id_marcacion)).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<EPU.Empleado_Hoja_Trabajo> listaHojaTrabajo(long id_empleado, DateTime fecha)
        {
            try
            {
                string sQuery = $@"
                    SELECT *
                    FROM public.empleado_hoja_trabajo
                    WHERE estado = 0 and id_empleado = {id_empleado} and fecha :: DATE = '{fecha.ToString("yyyy-MM-dd")}'
                ";

                return _DBcontext.Empleado_Hoja_Trabajo.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Marcacion BuscarMarcacion(EPU.Marcacion eEntidad)
        {
            try
            {
                string sQuery = $@"
                                SELECT *
                                FROM PUBLIC.marcacion
                                WHERE id_empleado = {eEntidad.id_empleado} AND codigo_turno = {eEntidad.codigo_turno}
                                    AND entrada_programada::DATE=DATE('{DateTime.Now.Date.ToString("yyyy-MM-dd")}')";

                return this._DBcontext.Marcacion.FromSqlRaw(sQuery).FirstOrDefault();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Marcacion BuscarMarcacion(long id_empleado, long codigo_turno, DateTime fecha)
        {
            try
            {
                string sQuery = $@"
                    SELECT *
                    FROM PUBLIC.marcacion
                    WHERE id_empleado = {id_empleado} AND codigo_turno = {codigo_turno}
                        AND entrada_programada::DATE = '{fecha.ToString("yyyy-MM-dd")}'
                ";

                return this._DBcontext.Marcacion.FromSqlRaw(sQuery).FirstOrDefault();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public void VerificarMarcacion(EPU.Marcacion eMarcacion)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    int HORAS_MINIMA_TRABAJO_DIARIO = 8;//Poner como configuracion global
                    eMarcacion.fecha_marcacion = DateTime.Now;
                    EPU.Marcacion eMarcacionProgramada = BuscarMarcacion(eMarcacion.id_empleado, eMarcacion.codigo_turno, eMarcacion.fecha_marcacion);
                    if (eMarcacionProgramada != null)
                    {
                        bool isPrimerEntrada = false;
                        bool isSalidaFinal = false;
                        if (eMarcacion.entrada_real == null) return;
                        DateTime entrada_real = eMarcacion.entrada_real.Value;

                        List<EPU.Marcacion> lstMarcacionesProgramadas = listarMarcacionesEmpleado(eMarcacion.id_empleado, entrada_real);
                        int marcacionesSalidaEsperadas = lstMarcacionesProgramadas.Count();
                        int cantidadEntradas = 0;
                        int cantidadSalidas = 0;
                        foreach (EPU.Marcacion iter in lstMarcacionesProgramadas)
                        {
                            if ((iter.entrada_movil != null))
                            {
                                cantidadEntradas++;
                            }

                            if ((iter.salida_movil != null))
                            {
                                cantidadSalidas++;
                            }
                        }

                        if ((cantidadEntradas == 0))
                        {
                            isPrimerEntrada = true;
                        }

                        if (((eMarcacion.salida_movil != null)
                                    && ((marcacionesSalidaEsperadas - cantidadSalidas)
                                    == 1)))
                        {
                            isSalidaFinal = true;
                        }

                        if (isPrimerEntrada)
                        {
                            //VERIFICAR FERIADO
                            /*
                            Calendar calendarFechaVerificacion = Calendar.getInstance(TimeZone.getTimeZone(Constantes.TIME_ZONE));
                            calendarFechaVerificacion.add(Calendar.DATE, -1);
                            DiaFeriado diaFeriado = getDiaFeriado(calendarFechaVerificacion.getTime());
                            while ((diaFeriado != null))
                            {
                                calendarFechaVerificacion.add(Calendar.DATE, -1);
                                diaFeriado = getDiaFeriado(calendarFechaVerificacion.getTime());
                            }

                            if ((calendarFechaVerificacion.get(Calendar.DAY_OF_WEEK) == Calendar.SUNDAY))
                            {
                                calendarFechaVerificacion.add(Calendar.DATE, -2);
                            }

                            if ((calendarFechaVerificacion.get(Calendar.DAY_OF_WEEK) == Calendar.SATURDAY))
                            {
                                calendarFechaVerificacion.add(Calendar.DATE, -1);
                            }
                            */
                            DateTime fecha_anterior = DateTime.Now.AddDays(-1);

                            //  verificando las horas de trabajo del dia anterior
                            List<EPU.Empleado_Hoja_Trabajo> lstHojasTrabajo = listaHojaTrabajo(eMarcacion.id_empleado, fecha_anterior);
                            Double horasTrabajadasDiaAnterior = 0;
                            if (lstHojasTrabajo != null)
                            {
                                horasTrabajadasDiaAnterior = lstHojasTrabajo.Sum(e => e.hora);
                            }

                            if ((horasTrabajadasDiaAnterior < HORAS_MINIMA_TRABAJO_DIARIO))
                            {
                                throw new Exception($"No es posible marcar la entrada. Las horas trabajadas en fecha {fecha_anterior.ToString("yyyy-MM-dd")} es menor a {HORAS_MINIMA_TRABAJO_DIARIO}. Favor de registrar sus horas de trabajo en la fecha especificada.");
                            }
                        }

                        if (isSalidaFinal)
                        {
                            if (eMarcacion.salida_movil == null) return;
                            DateTime salida_movil = eMarcacion.salida_movil.Value;
                            //  verificando si subio sus datos de marcacion
                            List<EPU.Empleado_Hoja_Trabajo> lstHojasTrabajo = listaHojaTrabajo(eMarcacion.id_empleado, salida_movil);
                            Double horasTrabajadas = 0;
                            if (lstHojasTrabajo != null)
                            {
                                horasTrabajadas = lstHojasTrabajo.Sum(e => e.hora);
                            }

                            if (horasTrabajadas != HORAS_MINIMA_TRABAJO_DIARIO)
                            {
                                throw new Exception($"No es posible marcar la salida. Las horas de trabajo registradas en fecha {salida_movil.ToString("yyyy-MM-dd")} deben ser igual a {HORAS_MINIMA_TRABAJO_DIARIO}");
                            }
                        }
                        eMarcacion.id = eMarcacionProgramada.id;
                        eMarcacion.entrada_programada = eMarcacionProgramada.entrada_programada;
                        eMarcacion.salida_programada = eMarcacionProgramada.salida_programada;

                        if (eMarcacion.entrada_real != null)
                        {
                            eMarcacion.tipo_marcacion_entrada = "MOVIL";
                        }

                        if (eMarcacion.salida_real != null)
                        {
                            eMarcacion.tipo_marcacion_salida = "MOVIL";
                        }

                        this._DBcontext.Marcacion.Update(eMarcacion);
                        this._DBcontext.SaveChanges();
                        oTrans.Commit();
                    }
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }

        public List<EPU.Marcacion> GetLista()
        {
            try
            {
                return _DBcontext.Marcacion.ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Helpers.MarcacionHelpers> GetListaDetalle(DateTime desde, DateTime hasta)
        {
            try
            {
                string sQuery = $@"
                    SELECT
                        mr. id,
                        emp.codigo as codigo_empleado,
                        (
                        trim(both ' ' from emp.nombre) || ' ' || trim(
                        both ' '
                        from
                        emp.apellido_paterno
                        ) || ' ' || trim(
                        both ' '
                        from
                        emp.apellido_materno
                        )
                        ) as nombre_empleado,
                        COALESCE(em_entrada.nombre, 'SIN DATOS') as empresa_entrada,
                        COALESCE(em_salida.nombre, 'SIN DATOS') as empresa_salida,
                        tur.descripcion_turno,
                        mr.entrada_programada :: TEXT,
                        mr.salida_programada :: TEXT,
                        SUBSTRING(
                        COALESCE(
                        mr.entrada_real :: TEXT,
                        'SIN DATOS'
                        )
                        from
                        0 for 20
                        ) as entrada_real,
                        SUBSTRING(
                        COALESCE(
                        mr.salida_real :: TEXT,
                        'SIN DATOS'
                        )
                        from
                        0 for 20
                        ) as salida_real,
                        mr.latitud_entrada,
                        mr.longitud_entrada,
                        (
                        CASE
                        WHEN(mr.latitud_entrada = 0.0) THEN
                        'SIN PUNTO'
                        ELSE
                        (
                        CASE
                        WHEN(
                        geodistance(
                        mr.latitud_entrada,
                        mr.longitud_entrada,
                        em_entrada.latitud,
                        em_entrada.longitud
                        ) * 1609.34
                        ) <= em_entrada.perimetro THEN
                        'DENTRO DEL PERIMETRO'
                        ELSE
                        'FUERA DEL PERIMETRO'
                        END
                        )
                        END
                        ) as observacion_punto_entrada,
                        mr.latitud_salida,
                        mr.longitud_salida,
                        (
                        CASE
                        WHEN(mr.latitud_salida = 0.0) THEN
                        'SIN PUNTO'
                        ELSE
                        (
                        CASE
                        WHEN(
                        geodistance(
                        mr.latitud_salida,
                        mr.longitud_salida,
                        em_salida.latitud,
                        em_salida.longitud
                        ) * 1609.34
                        ) <= em_salida.perimetro THEN
                        'DENTRO DEL PERIMETRO'
                        ELSE
                        'FUERA DEL PERIMETRO'
                        END
                        )
                        END
                        ) as observacion_punto_salida,
                        COALESCE(em_entrada.perimetro, 0) as perimetro_entrada,
                        COALESCE(em_salida.perimetro, 0) as perimetro_salida,
                        COALESCE(mr.tipo_marcacion_entrada, ' ') as tipo_marcacion_entrada,
                        COALESCE(mr.tipo_marcacion_salida, ' ') as tipo_marcacion_salida
                        FROM
                        PUBLIC .marcacion mr
                        INNER JOIN PUBLIC .empleado emp ON emp. ID = mr.id_empleado and emp.estado = 0
                        INNER JOIN PUBLIC .turno tur ON tur.codigo_turno = mr.codigo_turno
                        AND tur.id_empleado = mr.id_empleado
                        LEFT JOIN PUBLIC .empresa em_entrada on em_entrada. id = mr.id_empresa_entrada
                        LEFT JOIN PUBLIC .empresa em_salida on em_salida. id = mr.id_empresa_salida
                        WHERE
                        mr.entrada_programada :: DATE BETWEEN DATE('{desde.ToString("yyyy-MM-dd")}')
                        AND DATE('{hasta.ToString("yyyy-MM-dd")}')
                        ORDER BY
                        nombre_empleado,
                        mr.entrada_programada
                ";
                var listaMarcacion = _DBcontext.MarcacionHelpers.FromSqlRaw(sQuery).ToList();
                //if (listaMarcacion != null) {
                //    foreach(var item in listaMarcacion) {
                //        var eMarcacion = this._DBcontext.Marcacion.FirstOrDefault(o => o.id == item.id);
                //        if (eMarcacion != null) {
                //            var eEmpresa = this._DBcontext.Empleado.FirstOrDefault(p => p.id == eMarcacion.id_empresa_entrada);
                //            item.empresa_entrada = (eEmpresa == null) ? "SIN DATOS" : eEmpresa.nombre;
                //            item.empresa_salida = (eEmpresa == null) ? "SIN DATOS" : eEmpresa.nombre;
                //        }
                //    }
                //}
                return listaMarcacion;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Marcacion> listarMarcacionPorFecha(long id_sucursal, DateTime pFechaInicio, DateTime pFechaFin, bool swSoloConRegistro = false)
        {
            try
            {
                string query = $@" 
                    SELECT m.* 
                    FROM public.marcacion m
                    JOIN public.empleado e on m.id_empleado = e.id
                    WHERE m.entrada_programada::DATE >= DATE('{pFechaInicio.ToString("yyyy-MM-dd")}')
                        AND m.entrada_programada::DATE <= DATE('{pFechaFin.ToString("yyyy-MM-dd")}')
                        {(!swSoloConRegistro ? "" : " AND (m.entrada_real is not null or m.salida_real is not NULL) ")}
                        {(id_sucursal == 0 ? "" : $" AND  e.id_sucursal = {id_sucursal} ")}
                    order by e.id_sucursal	
                ";

                List<EPU.Marcacion> lstMar = _DBcontext.Marcacion.FromSqlRaw(query).ToList();
                List<EPU.Marcacion> lstMarcaciones = new List<EPU.Marcacion>();
                //cargar entidad empleado relacionado a cada marcacion
                var IDs = lstMar.Select(e => e.id_empleado).ToList();
                var listaEmpleado = this._DBcontext.Empleado.Where(e => IDs.Contains(e.id) && e.estado == 0).ToList();
                foreach (EPU.Marcacion oMarcacion in lstMar)
                {
                    var Empleado = listaEmpleado.FirstOrDefault(e => e.id == oMarcacion.id_empleado);
                    if (Empleado != null)
                    {
                        oMarcacion.empleado = Empleado;
                        oMarcacion.id_sucursal = Empleado.id_sucursal;
                        oMarcacion.nombre_sucursal = Empleado.nombre_sucursal;
                        lstMarcaciones.Add(oMarcacion);
                    }
                }

                return lstMarcaciones;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}