using Data;
using Entities.Public;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using EPU = Entities.Public;

namespace Business.Public
{
    public class Engagement : Base, IBusiness<EPU.Engagement>
    {
        public Engagement(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.Engagement eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Engagement.Add(eEntidad);
                    this._DBcontext.SaveChanges();

                    // lista detalle hora
                    eEntidad.listaDetalle_Hora_Engagement = eEntidad.listaDetalle_Hora_Engagement.Select(e =>
                    {
                        e.id = 0;
                        e.id_engagement = eEntidad.id;
                        return e;
                    }).ToList();
                    this._DBcontext.Detalle_Hora_Engagement.AddRange(eEntidad.listaDetalle_Hora_Engagement);
                    this._DBcontext.SaveChanges();

                    //lista alerta
                    eEntidad.listaAlerta_Engagement = eEntidad.listaAlerta_Engagement.Select(e =>
                    {
                        e.id = 0;
                        e.id_engagement = eEntidad.id;
                        return e;
                    }).ToList();
                    this._DBcontext.Alerta_Engagement.AddRange(eEntidad.listaAlerta_Engagement);
                    this._DBcontext.SaveChanges();

                    //lista responsables y precarga de engagement empleado
                    foreach (var eResponsable in eEntidad.listaResponsable_Engagement)
                    {
                        eResponsable.id = 0;
                        eResponsable.id_engagement = eEntidad.id;
                        this._DBcontext.Responsable_Engagement.Add(eResponsable);

                        Entities.Public.Engagement_Empleado eEngagement_Empleado = new EPU.Engagement_Empleado();
                        eEngagement_Empleado.id = 0;
                        eEngagement_Empleado.id_empleado = eResponsable.id_usuario;
                        eEngagement_Empleado.id_engagement = eEntidad.id;
                        eEngagement_Empleado.fecha_asignacion = DateTime.Now;
                        eEngagement_Empleado.cantidad_horas_habilitadas = 0;
                        this._DBcontext.Engagement_Empleado.Add(eEngagement_Empleado);
                    }
                    this._DBcontext.SaveChanges();

                    //lista tareas
                    eEntidad.listaTarea_Engagement = eEntidad.listaTarea_Engagement.Select(e =>
                    {
                        e.id = 0;
                        e.id_engagement = eEntidad.id;
                        return e;
                    }).ToList();
                    this._DBcontext.Tarea_Engagement.AddRange(eEntidad.listaTarea_Engagement);
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

        public bool Modificar(EPU.Engagement eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Engagement.Update(eEntidad);
                    this._DBcontext.SaveChanges();

                    // lista detalle hora
                    this._DBcontext.Detalle_Hora_Engagement.RemoveRange(this._DBcontext.Detalle_Hora_Engagement.Where(e => e.id_engagement == eEntidad.id));
                    this._DBcontext.SaveChanges();
                    eEntidad.listaDetalle_Hora_Engagement = eEntidad.listaDetalle_Hora_Engagement.Select(e =>
                    {
                        e.id = 0;
                        e.id_engagement = eEntidad.id;
                        return e;
                    }).ToList();
                    this._DBcontext.Detalle_Hora_Engagement.AddRange(eEntidad.listaDetalle_Hora_Engagement);
                    this._DBcontext.SaveChanges();

                    //lista alerta
                    this._DBcontext.Alerta_Engagement.RemoveRange(this._DBcontext.Alerta_Engagement.Where(e => e.id_engagement == eEntidad.id));
                    this._DBcontext.SaveChanges();
                    eEntidad.listaAlerta_Engagement = eEntidad.listaAlerta_Engagement.Select(e =>
                    {
                        e.id = 0;
                        e.id_engagement = eEntidad.id;
                        return e;
                    }).ToList();
                    this._DBcontext.Alerta_Engagement.AddRange(eEntidad.listaAlerta_Engagement);
                    this._DBcontext.SaveChanges();

                    //lista responsables
                    this._DBcontext.Responsable_Engagement.RemoveRange(this._DBcontext.Responsable_Engagement.Where(e => e.id_engagement == eEntidad.id));
                    this._DBcontext.SaveChanges();
                    eEntidad.listaResponsable_Engagement = eEntidad.listaResponsable_Engagement.Select(e =>
                    {
                        e.id = 0;
                        e.id_engagement = eEntidad.id;
                        return e;
                    }).ToList();
                    this._DBcontext.Responsable_Engagement.AddRange(eEntidad.listaResponsable_Engagement);
                    this._DBcontext.SaveChanges();

                    //lista tareas
                    this._DBcontext.Tarea_Engagement.RemoveRange(this._DBcontext.Tarea_Engagement.Where(e => e.id_engagement == eEntidad.id));
                    this._DBcontext.SaveChanges();
                    eEntidad.listaTarea_Engagement = eEntidad.listaTarea_Engagement.Select(e =>
                    {
                        e.id = 0;
                        e.id_engagement = eEntidad.id;
                        return e;
                    }).ToList();
                    this._DBcontext.Tarea_Engagement.AddRange(eEntidad.listaTarea_Engagement);
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
                    var eEntidad = this.GetEntity(id);
                    if (eEntidad == null) throw new Exception("Engagement Inexistente!!...");

                    eEntidad.estado = 9;
                    this._DBcontext.Engagement.Update(eEntidad);
                    this._DBcontext.SaveChanges();

                    this._DBcontext.Database.ExecuteSqlRaw("UPDATE PUBLIC.engagement_empleado set estado = 9 WHERE id_engagement = " + eEntidad.id);
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

        public EPU.Engagement GetEntity(long id)
        {
            try
            {
                var eEngagement = _DBcontext.Engagement.FirstOrDefault(p => p.estado == 0 && p.id == id);
                if (eEngagement != null)
                {
                    eEngagement.listaDetalle_Hora_Engagement = this._DBcontext.Detalle_Hora_Engagement.Where(e => e.id_engagement == eEngagement.id).ToList();
                    if (eEngagement.listaDetalle_Hora_Engagement != null)
                    {
                        eEngagement.listaDetalle_Hora_Engagement = eEngagement.listaDetalle_Hora_Engagement.Select(d =>
                        {
                            d.tipo_descripcion = (d.tipo == 1) ? "Horas Trabajo" : "Horas Adicionales (Overrun)";
                            return d;
                        }).ToList();
                    }
                    eEngagement.listaAlerta_Engagement = this._DBcontext.Alerta_Engagement.Where(e => e.id_engagement == eEngagement.id).ToList();
                    eEngagement.listaResponsable_Engagement = this._DBcontext.Responsable_Engagement.Where(e => e.id_engagement == eEngagement.id).ToList();
                    if (eEngagement.listaResponsable_Engagement != null)
                    {
                        eEngagement.listaResponsable_Engagement = eEngagement.listaResponsable_Engagement.Select(d =>
                        {
                            var eUsuario = this._DBcontext.Empleado.FirstOrDefault(u => u.id == d.id_usuario);
                            d.nombre_usuario = (eUsuario == null) ? "SIN RESPONSABLE ASIGNADO" : eUsuario.nombre;
                            return d;
                        }).ToList();
                    }

                    eEngagement.listaTarea_Engagement = this._DBcontext.Tarea_Engagement.Where(e => e.id_engagement == eEngagement.id).ToList();
                    if (eEngagement.listaTarea_Engagement != null)
                    {
                        eEngagement.listaTarea_Engagement = eEngagement.listaTarea_Engagement.Select(d =>
                        {
                            var eTarea = this._DBcontext.Tarea.FirstOrDefault(u => u.id == d.id_tarea);
                            var eCategoria = this._DBcontext.Categoria_Tarea.FirstOrDefault(u => u.id == eTarea.id_categoria);
                            d.categoria = (eCategoria == null) ? "SIN CATEGORIA ASIGNADA" : eCategoria.nombre;
                            return d;
                        }).ToList();
                    }
                }

                return eEngagement;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Engagement GetEntityByHorasDisponibles(long id)
        {
            try
            {
                var eEngagement = _DBcontext.Engagement.FirstOrDefault(u => u.estado == 0 && u.id == id);
                if (eEngagement != null)
                {
                    eEngagement.hora_servicio = this._DBcontext.Detalle_Hora_Engagement.Where(d => d.id_engagement == eEngagement.id && d.tipo == 1).Sum(d => d.cantidad);
                    eEngagement.hora_extra = this._DBcontext.Detalle_Hora_Engagement.Where(d => d.id_engagement == eEngagement.id && d.tipo == 2).Sum(d => d.cantidad);
                }

                return eEngagement;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Engagement GetEntityByEngagementEmpleado(long id_engagemen)
        {
            try
            {
                var eEngagement = _DBcontext.Engagement.FirstOrDefault(p => p.estado == 0 && p.id == id_engagemen);

                return eEngagement;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Engagement> GetLista()
        {
            try
            {
                return _DBcontext.Engagement.Where(u => u.estado == 0).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public void CambiarEstadoEjecucion()
        {
            try
            {
                string sQuery = $@"
                    SELECT *
                    FROM public.engagement
                    WHERE estado = 0 and estado_ejecucion = 1 and desde:: DATE < '{DateTime.Now.ToString("yyyy-MM-dd")}'
                ";
                var lista = this._DBcontext.Engagement.FromSqlRaw(sQuery).ToList();
                if (lista != null)
                {
                    foreach (var item in lista)
                    {
                        if (DateTime.Now > item.desde)
                        {
                            item.estado_ejecucion = 2;
                            this._DBcontext.Engagement.Update(item);
                        }
                    }
                    this._DBcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public bool TareaEnUso(long id_engagement, long id_tarea)
        {
            try
            {
                var lista = this._DBcontext.Empleado_Hoja_Trabajo.Where(e => e.id_engagement == id_engagement && e.id_tarea == id_tarea && e.estado == 0).ToList();
                if (lista != null)
                {
                    return lista.Count > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Public.Engagement> GetListaByFiltro(int tipo, long id_empleado)
        {
            try
            {
                List<EPU.Engagement> lista = new List<EPU.Engagement>();
                if (id_empleado > 0)
                {
                    string sQuery = $@"
                        SELECT *
                        FROM PUBLIC.engagement
                        WHERE estado = 0 and estado_ejecucion = {tipo} and id in
                        (
	                        SELECT id_engagement
	                        FROM PUBLIC.responsable_engagement
	                        WHERE id_usuario = {id_empleado} and estado = 0
                        )
                    ";
                    lista = this._DBcontext.Engagement.FromSqlRaw(sQuery).ToList();
                }
                else
                    lista = this._DBcontext.Engagement.Where(u => u.estado == 0 && u.estado_ejecucion == tipo).ToList();

                if (lista != null)
                {
                    lista = lista.Select(e =>
                    {
                        var eEmpresa = this._DBcontext.Empresa.FirstOrDefault(d => d.id == e.id_empresa);
                        e.empresa = (eEmpresa == null) ? "SIN EMPRESA" : eEmpresa.nombre;
                        e.hora_servicio = this._DBcontext.Detalle_Hora_Engagement.Where(d => d.id_engagement == e.id && d.tipo == 1).Sum(d => d.cantidad);
                        e.hora_extra = this._DBcontext.Detalle_Hora_Engagement.Where(d => d.id_engagement == e.id && d.tipo == 2).Sum(d => d.cantidad);
                        e.horas_ejecutadas = this._DBcontext.Empleado_Hoja_Trabajo.Where(d => d.id_engagement == e.id && d.estado == 0).Sum(d => d.hora);
                        if (e.estado_ejecucion == 1) e.text_estado = "PLANIFICADO";
                        if (e.estado_ejecucion == 2) e.text_estado = "EN PROCESO";
                        if (e.estado_ejecucion == 3) e.text_estado = "FINALIZADO";
                        var eArea = this._DBcontext.Area_Engagement.FirstOrDefault(a => a.id == e.id_area);
                        e.area = (eArea == null) ? "SIN AREA" : eArea.nombre;
                        return e;
                    }).ToList();
                }

                return lista.OrderByDescending(e => e.id).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Public.Engagement> GetListaByEmpleadoTipo(long id_usuario)
        {
            try
            {
                List<EPU.Engagement> lista = new List<EPU.Engagement>();

                string sQuery = $@"
                    SELECT *
                    FROM PUBLIC.engagement
                    WHERE estado = 0 and id in
                    (
	                    SELECT id_engagement
	                    FROM PUBLIC.responsable_engagement
	                    WHERE id_usuario = {id_usuario} and estado = 0
                    )
                ";
                lista = this._DBcontext.Engagement.FromSqlRaw(sQuery).ToList();

                return lista;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Engagement> GetListaByEmpleado(long id_empleado)
        {
            try
            {
                string sQuery = $@"
                    SELECT e.*
                    FROM PUBLIC.engagement e
                    JOIN PUBLIC.engagement_empleado eemp on eemp.id_engagement = e.id
                    WHERE e.estado = 0 and eemp.estado = 0 and eemp.id_empleado = {id_empleado}
                ";
                return _DBcontext.Engagement.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Tarea> GetListaTareaByEngagment(long id_engagement)
        {
            try
            {
                string sQuery = $@"
                    SELECT t.*
                    FROM PUBLIC.tarea t
                    JOIN PUBLIC.tarea_engagement te ON te.id_tarea = t.id
                    WHERE t.estado = 0 and te.estado = 0 and te.id_engagement = {id_engagement}
                ";
                return _DBcontext.Tarea.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Engagement> GetListaByEmpresa(long id_empresa)
        {
            try
            {
                var listaEngagement = _DBcontext.Engagement.Where(u => u.estado == 0 && u.id_empresa == id_empresa).OrderByDescending(e => e.id).ToList();
                if (listaEngagement != null)
                {
                    listaEngagement = listaEngagement.Select(e =>
                    {
                        e.hora_servicio = this._DBcontext.Detalle_Hora_Engagement.Where(d => d.id_engagement == e.id && d.tipo == 1).Sum(d => d.cantidad);
                        e.hora_extra = this._DBcontext.Detalle_Hora_Engagement.Where(d => d.id_engagement == e.id && d.tipo == 2).Sum(d => d.cantidad);
                        return e;
                    }).ToList();
                }

                return listaEngagement;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Engagement> GetListaByEmpresaSemana(long id_empresa, long id_empleado, DateTime fecha_inicio)
        {
            try
            {
                DateTime fecha_fin = fecha_inicio.AddDays(6);
                string sQuery = $@"
                    SELECT eng.*
	                From PUBLIC.engagement eng
	                JOIN PUBLIC.engagement_empleado eng_emp on eng_emp.id_engagement = eng.id
	                WHERE eng.estado = 0 and eng_emp.estado = 0 and eng.id_empresa = {id_empresa} and eng_emp.id_empleado = {id_empleado}

                ";
                var listaEngagement = _DBcontext.Engagement.FromSqlRaw(sQuery).OrderByDescending(e => e.id).ToList();
                if (listaEngagement != null)
                {
                    listaEngagement = listaEngagement.Select(e =>
                    {
                        e.hora_servicio = this._DBcontext.Detalle_Hora_Engagement.Where(d => d.id_engagement == e.id && d.tipo == 1).Sum(d => d.cantidad);
                        e.hora_extra = this._DBcontext.Detalle_Hora_Engagement.Where(d => d.id_engagement == e.id && d.tipo == 2).Sum(d => d.cantidad);
                        return e;
                    }).ToList();
                }

                return listaEngagement;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public double HorasDisponiblesByEngagement(long id_engagement, long id_empleado = 0)
        {
            try
            {
                var eEngagement = _DBcontext.Engagement.FirstOrDefault(u => u.estado == 0 && u.id == id_engagement);
                if (eEngagement != null)
                {
                    eEngagement.hora_servicio = this._DBcontext.Detalle_Hora_Engagement.Where(d => d.id_engagement == eEngagement.id && d.tipo == 1).Sum(d => d.cantidad);
                    //eEngagement.hora_extra = this._DBcontext.Detalle_Hora_Engagement.Where(d => d.id_engagement == eEngagement.id && d.tipo == 2).Sum(d => d.cantidad);
                    //double horas_total = eEngagement.hora_servicio + eEngagement.hora_extra;
                    double horas_total = eEngagement.hora_servicio;

                    var listaDetalle = (id_empleado == 0) ?
                        this._DBcontext.Engagement_Empleado.Where(dtl => dtl.id_engagement == id_engagement && dtl.estado == 0)
                        :
                        this._DBcontext.Engagement_Empleado.Where(dtl => dtl.id_engagement == id_engagement && dtl.estado == 0 && dtl.id_empleado != id_empleado);

                    double horas_asignadas = 0;
                    foreach (var eDetalle in listaDetalle)
                    {
                        var horas = this._DBcontext.Detalle_Engagement_Empleado.Where(d => d.id_engagement_empleado == eDetalle.id && d.estado == 0).Sum(d => d.cantidad_horas);
                        horas_asignadas += horas;
                    }
                    return horas_total - horas_asignadas;
                }

                return 0;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public double HorasEjecutadasByFecha(long id_empleado, DateTime fecha)
        {
            try
            {
                string sQuery = $@"
                    SELECT eht.*
                    FROM public.empleado_hoja_trabajo eht
                    JOIN PUBLIC.engagement eng on eng.id = eht.id_engagement
                    WHERE eht.estado = 0 and eng.estado = 0 AND
                        eht.id_empleado = {id_empleado} AND eht.fecha::DATE = '{fecha.ToString("yyyy-MM-dd")}'";
                double horas_ejecutadas = _DBcontext.Empleado_Hoja_Trabajo.FromSqlRaw(sQuery).ToList().Sum(eht => eht.hora);

                return horas_ejecutadas;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public double HorasAsignadas(long id_engagement)
        {
            try
            {
                string sQuery = $@"
                    SELECT dee.*
                    FROM public.detalle_engagement_empleado dee
                    INNER JOIN public.engagement_empleado ee ON dee.id_engagement_empleado = ee.id
                    JOIN PUBLIC.engagement eng on eng.id = ee.id_engagement
                    WHERE ee.id_engagement = {id_engagement} AND dee.estado = 0 and eng.estado = 0;";
                double horas_asignadas = _DBcontext.Detalle_Engagement_Empleado.FromSqlRaw(sQuery).ToList().Sum(dtl => dtl.cantidad_horas);

                return horas_asignadas;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public double HorasDisponiblesByFecha(long id_empleado, DateTime fecha)
        {
            try
            {
                string sQuery = $@"
                    SELECT dtl.*
                    FROM PUBLIC.detalle_engagement_empleado dtl
                    JOIN PUBLIC.engagement_empleado eng_emp on eng_emp.id = dtl.id_engagement_empleado
                    JOIN PUBLIC.engagement eng on eng.id = eng_emp.id_engagement
                    WHERE dtl.estado = 0 and eng_emp.estado = 0 and eng.estado = 0 and eng_emp.id_empleado = {id_empleado} and
			            dtl.fecha_inicio::DATE = '{fecha.ToString("yyyy-MM-dd")}' and dtl.fecha_fin::DATE = '{fecha.ToString("yyyy-MM-dd")}'
                ";
                double horas_asignadas = _DBcontext.Detalle_Engagement_Empleado.FromSqlRaw(sQuery).ToList().Sum(dtl => dtl.cantidad_horas);

                // Obtener Horas del turno en base al empleado y el dia para la fecha
                double horasHorario = _DBcontext.Turno
                    .Where(t => t.id_empleado == id_empleado && t.idc_dia % 7 == (int)fecha.DayOfWeek)
                    .Sum(t => (t.salida - t.entrada).TotalHours);

                //return 8 - horas_asignadas;
                return horasHorario - horas_asignadas;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public double HorasTurnoByFecha(long id_empleado, DateTime fecha)
        {
            try
            {
                // Obtener Horas del turno en base al empleado y el dia para la fecha
                double horasHorario = _DBcontext.Turno
                    .Where(t => t.id_empleado == id_empleado && t.idc_dia % 7 == (int)fecha.DayOfWeek)
                    .Sum(t => (t.salida - t.entrada).TotalHours);

                //return 8 - horas_asignadas;
                return horasHorario;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Detalle_Engagement_Empleado> DetallesEngagementEmpleadoBy(long id_engagement, long id_empleado)
        {
            try
            {
                string sQuery = $@"
                    SELECT dee.*
                    FROM public.detalle_engagement_empleado dee
                    INNER JOIN public.engagement_empleado ee ON dee.id_engagement_empleado = ee.id
                    WHERE dee.estado = 0 AND ee.estado = 0 AND
		                ee.id_empleado = {id_empleado} AND ee.id_engagement = {id_engagement}";
                return this._DBcontext.Detalle_Engagement_Empleado.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Detalle_Engagement_Empleado> DetallesEngagementEmpleadoByFecha(DateTime fecha, long id_engagement, long id_empleado)
        {
            try
            {
                string sQuery = $@"
                    SELECT dee.*
                    FROM public.detalle_engagement_empleado dee
                    INNER JOIN public.engagement_empleado ee ON dee.id_engagement_empleado = ee.id
                    WHERE dee.estado = 0 AND ee.estado = 0 AND dee.fecha_inicio::DATE = '{fecha.ToString("yyyy-MM-dd")}' AND
		                ee.id_empleado = {id_empleado} AND ee.id_engagement = {id_engagement}";
                return this._DBcontext.Detalle_Engagement_Empleado.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Helpers.AvanceEngagementHelper> AvanceEngagement(DateTime desde, DateTime hasta)
        {
            try
            {
                string sQuery = $@"
                    SELECT
	                    eng.id id_engagement,
	                    eng.id_empresa,
	                    emp.nombre nombre_empresa,
	                    eng.titulo nombre_engagement,
                        eng.desde,
                        eng.hasta,
                        COALESCE((
	                        SELECT SUM(cantidad)
	                        FROM PUBLIC.detalle_hora_engagement where estado = 0 and tipo = 1 and id_engagement = eng.id
                        ), 0) horas_trabajo,
                        COALESCE((
	                        SELECT SUM(hora)
	                        FROM PUBLIC.empleado_hoja_trabajo where estado = 0 and id_engagement = eng.id
                        ), 0) horas_ejecutadas,
                        (
                            (
                            COALESCE((
	                            SELECT SUM(hora)
	                            FROM PUBLIC.empleado_hoja_trabajo where estado = 0 and id_engagement = eng.id
                            ), 0)
                        /
                            COALESCE((
	                            SELECT SUM(cantidad)
	                            FROM PUBLIC.detalle_hora_engagement where estado = 0 and tipo = 1 and id_engagement = eng.id
                            ), 1)
                        ) * 100 )::INTEGER porcentaje_avance
                    FROM PUBLIC.engagement eng
                    JOIN PUBLIC.empresa emp on emp.id = eng.id_empresa
                    WHERE eng.estado = 0 and eng.desde::DATE >= '{desde.ToString("yyyy-MM-dd")}' and eng.desde::DATE <= '{hasta.ToString("yyyy-MM-dd")}'

                ";

                var listaAvance = this._DBcontext.AvanceEngagementHelper.FromSqlRaw(sQuery).ToList();

                return listaAvance;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Public.Empleado> GetResponsablesEngagement(long idEngagement)
        {
            try
            {
                string sQuery = $@"SELECT e.*
                                   FROM public.empleado e
                                   INNER JOIN public.responsable_engagement re ON re.id_usuario = e.id AND re.id_engagement = {idEngagement}
                                   WHERE e.estado = 0";

                var responsables = this._DBcontext.Empleado.FromSqlRaw(sQuery).ToList();

                return responsables;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Helpers.NotificarAlertaHelper> GetNotificarAlertaHelper()
        {
            try
            {
                string sQuery = $@"
                    SELECT
	                    tabla.id_alerta,
	                    emp.nombre nombre_empresa,
	                    tabla.id_engagement,
	                    tabla.nombre_engagement,
	                    tabla.notificar,
	                    tabla.porcentaje_avance
                    FROM
                    (
	                    SELECT
		                    alert.id id_alerta,
		                    eng.id id_engagement,
		                    eng.titulo nombre_engagement,
		                    eng.id_empresa,
			                    (
				                    (
				                    COALESCE((
					                    SELECT SUM(hora)
					                    FROM PUBLIC.empleado_hoja_trabajo where estado = 0 and id_engagement = eng.id
				                    ), 0)
			                    /
				                    COALESCE((
					                    SELECT SUM(cantidad)
					                    FROM PUBLIC.detalle_hora_engagement where estado = 0 and tipo = 1 and id_engagement = eng.id
				                    ), 1)
			                    ) * 100 )::INTEGER porcentaje_avance,
			                    (alert.porcentaje_notificador) notificar
	                    FROM PUBLIC.engagement eng
	                    JOIN PUBLIC.alerta_engagement alert on alert.id_engagement = eng.id
	                    WHERE eng.estado = 0 and eng.estado_ejecucion <> 3 and alert.ejecutado = 0 and alert.estado = 0
                    ) as tabla
                    JOIN PUBLIC.empresa emp on emp.id = tabla.id_empresa
                    WHERE tabla.porcentaje_avance >= tabla.notificar
                    ";

                var listaNotificar = this._DBcontext.NotificarAlertaHelper.FromSqlRaw(sQuery).ToList();
                if (listaNotificar != null)
                {
                    foreach (var itemNotificar in listaNotificar)
                    {
                        string sQueryContacto = $@"
                            SELECT
	                            0 codigo_grupo,
	                            emp.id codigo_contacto,
	                            emp.nombre nombre,
	                            emp.correo correo,
	                            emp.telefono telefono,
	                            'P' tipo,
	                            0 marca_eliminado
                            FROM PUBLIC.engagement_empleado resp
                            JOIN PUBLIC.empleado emp on emp.id = resp.id_empleado
                            WHERE resp.estado = 0 and emp.estado = 0 and resp.id_engagement = {itemNotificar.id_engagement}
                        ";
                        itemNotificar.listaContacto = this._DBcontext.Contacto.FromSqlRaw(sQueryContacto).ToList();
                    }
                }
                return listaNotificar;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public bool CambiarEstadoEjecutado(long id_alerta)
        {
            try
            {
                var eAlerta = _DBcontext.Alerta_Engagement.FirstOrDefault(p => p.estado == 0 && p.id == id_alerta);
                if (eAlerta != null)
                {
                    eAlerta.ejecutado = 1;
                    this._DBcontext.Alerta_Engagement.Update(eAlerta);
                    this._DBcontext.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Helpers.PlaneacionEngagementHelper> PlaneacionEngagement(DateTime desde, DateTime hasta, long id_empresa, long id_area, long id_engagement)
        {
            try
            {
                string sQuery = $@"
drop table if exists tmp_planificacion_inc;
CREATE TEMP TABLE tmp_planificacion_inc AS (
	SELECT
		emp.id id_empresa,
		emp.nombre nombre_empresa,
		eng.id id_engagement,
		eng.titulo nombre_engagement,
		empl.id id_empleado,
			empl.codigo codigo_empleado,
		concat(empl.nombre, ' ', empl.apellido_paterno, ' ', empl.apellido_materno) nombre_empleado,
		empl.cargo
	FROM engagement eng
	JOIN empresa emp on emp.id = eng.id_empresa
	JOIN engagement_empleado eng_emp on eng_emp.id_engagement = eng.id
	JOIN empleado empl on empl.id = eng_emp.id_empleado
	WHERE eng.estado = 0 AND emp.marca_eliminado = 0 AND eng_emp.estado = 0
	AND
		(
		('{desde.ToString("yyyy-MM-dd")}' >= eng.desde::DATE AND '{desde.ToString("yyyy-MM-dd")}' <= eng.hasta::DATE)
		OR
		('{hasta.ToString("yyyy-MM-dd")}' >= eng.desde::DATE AND '{hasta.ToString("yyyy-MM-dd")}' <= eng.hasta::DATE)
	)

";

                if (id_empresa > 0) sQuery += "AND eng.id_empresa = " + id_empresa;
                if (id_area > 0) sQuery += "AND eng.id_area = " + id_area;
                if (id_engagement > 0) sQuery += "AND eng.id = " + id_engagement;

                sQuery += "ORDER BY 1 DESC";

                sQuery += @"
);

drop table if exists tmp_planificacion;
CREATE TEMP TABLE tmp_planificacion AS (

SELECT *
FROM tmp_planificacion_inc

UNION

SELECt
	0 id_empresa,
	'---' nombre_empresa,
	0 id_engagement,
	'---' nombre_engagement,
	empl.id id_empleado,
	empl.codigo codigo_empleado,
	concat(empl.nombre, ' ', empl.apellido_paterno, ' ', empl.apellido_materno) nombre_empleado,
	empl.cargo
FROM empleado empl
WHERE empl.id not in (SELECT id_empleado FROM tmp_planificacion_inc)

order by 1
);";

                sQuery += @"
                    SELECT
	                    ROW_NUMBER() OVER (ORDER BY tmp.id_empresa DESC) id,
	                    tmp.*
                    FROM tmp_planificacion tmp
                ";
                var listaPlaneacion = this._DBcontext.PlaneacionEngagementHelper.FromSqlRaw(sQuery).ToList();

                return listaPlaneacion;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}