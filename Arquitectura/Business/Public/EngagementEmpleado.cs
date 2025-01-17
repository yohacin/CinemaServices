using Data;
using Entities.Public;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using EPU = Entities.Public;

namespace Business.Public
{
    public class EngagementEmpleado : Base, IBusiness<EPU.Engagement_Empleado>
    {
        public EngagementEmpleado(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(Engagement_Empleado eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    var eEngagementRepetido = this._DBcontext.Engagement_Empleado.FirstOrDefault(e => e.id_engagement == eEntidad.id_engagement && e.id_empleado == eEntidad.id_empleado && e.estado == 0);
                    if (eEngagementRepetido == null)
                    {
                        this._DBcontext.Engagement_Empleado.Add(eEntidad);
                        this._DBcontext.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("El empleado ya está agregado en el engagement !");
                    }

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

        public bool GuardarLista(List<Engagement_Empleado> listaEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var eEngagement_Empleado in listaEntidad)
                    {
                        var eEngagementRepetido = this._DBcontext.Engagement_Empleado.FirstOrDefault(e => e.id_engagement == eEngagement_Empleado.id_engagement && e.id_empleado == eEngagement_Empleado.id_empleado);
                        if (eEngagementRepetido == null)
                        {
                            this._DBcontext.Engagement_Empleado.Add(eEngagement_Empleado);
                        }
                    }
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

        public bool GuardarListaDetalle(List<Detalle_Engagement_Empleado> listaEntidad, long id_engagement_empleado)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Database.ExecuteSqlRaw("UPDATE detalle_engagement_empleado set estado = 9 where id_engagement_empleado = " + id_engagement_empleado);

                    foreach (var eDetalle_Engagement_Empleado in listaEntidad)
                    {
                        eDetalle_Engagement_Empleado.id_engagement_empleado = id_engagement_empleado;
                        if (eDetalle_Engagement_Empleado.id >= 10000)
                        {
                            eDetalle_Engagement_Empleado.id = 0;
                            this._DBcontext.Detalle_Engagement_Empleado.Add(eDetalle_Engagement_Empleado);
                        }
                        else
                        {
                            this._DBcontext.Detalle_Engagement_Empleado.Update(eDetalle_Engagement_Empleado);
                        }
                    }
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

        public bool GuardarLista(List<Detalle_Engagement_Empleado> listaEntidad, long id_engagement_empleado)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Database.ExecuteSqlRaw("UPDATE detalle_engagement_empleado set estado = 9 where id_engagement_empleado = " + id_engagement_empleado);

                    foreach (var eDetalle_Engagement_Empleado in listaEntidad)
                    {
                        eDetalle_Engagement_Empleado.id_engagement_empleado = id_engagement_empleado;
                        eDetalle_Engagement_Empleado.estado = 0;
                        if (eDetalle_Engagement_Empleado.id == 0)
                        {
                            this._DBcontext.Detalle_Engagement_Empleado.Add(eDetalle_Engagement_Empleado);
                        }
                        else
                        {
                            this._DBcontext.Detalle_Engagement_Empleado.Update(eDetalle_Engagement_Empleado);
                        }
                    }
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

        public bool GuardarAsignacion(Detalle_Engagement_Empleado eEntidad, long id_engagement, long id_empleado)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    var eEngagementEmpleado = this._DBcontext.Engagement_Empleado.FirstOrDefault(e => e.id_engagement == id_engagement && e.id_empleado == id_empleado && e.estado == 0);
                    if (eEngagementEmpleado == null)
                    {
                        throw new Exception("No existe asociacion del Empleado en el Engagement !");
                    }
                    else
                    {
                        eEntidad.id_engagement_empleado = eEngagementEmpleado.id;

                        if (eEntidad.id > 0)
                        {
                            Detalle_Engagement_Empleado currentItem = this._DBcontext.Detalle_Engagement_Empleado.FirstOrDefault(x => x.id == eEntidad.id);

                            if (currentItem == null)
                            {
                                throw new Exception("No existe el detalle engagement empleado a editar !");
                            }

                            Engagement bEngagement = new Engagement(this._DBcontext);
                            double horas_disponibles = bEngagement.HorasDisponiblesByFecha(id_empleado, currentItem.fecha_inicio);
                            double horas_asignadas = eEntidad.cantidad_horas;
                            double horas_asignadas_actual = currentItem.cantidad_horas;
                            if (horas_asignadas > (horas_disponibles + horas_asignadas_actual))
                            {
                                throw new Exception("El numero de horas asignadas (" + horas_asignadas + ") no debe ser mayor al numero de horas disponibles(" + horas_disponibles + ")");
                            }

                            this._DBcontext.Entry(currentItem).CurrentValues.SetValues(eEntidad);
                        }
                        else
                        {
                            this._DBcontext.Detalle_Engagement_Empleado.Add(eEntidad);
                        }

                        this._DBcontext.SaveChanges();
                    }

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

        public bool EliminarAsignacionHoras(long id_detalle)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    Detalle_Engagement_Empleado currentItem = this._DBcontext.Detalle_Engagement_Empleado.FirstOrDefault(x => x.id == id_detalle);

                    if (currentItem == null)
                    {
                        throw new Exception("No existe el detalle engagement empleado a eliminar !");
                    }

                    currentItem.estado = 1;
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

        public bool EliminarAsignacionEmpleado(long id_engagement, long id_empleado)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    Engagement_Empleado currentItem = this._DBcontext.Engagement_Empleado.FirstOrDefault(x => x.id_engagement == id_engagement && x.id_empleado == id_empleado && x.estado == 0);

                    if (currentItem == null)
                    {
                        throw new Exception("No existe la asignación engagement empleado a eliminar !");
                    }

                    currentItem.estado = 1;
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

        public bool Modificar(Engagement_Empleado eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Engagement_Empleado.Update(eEntidad);
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
                    EPU.Engagement_Empleado eEntidad = this.GetEntity(id);
                    if (eEntidad == null) throw new Exception("Asociacion de empleado al engagement Inexistente!!...");

                    eEntidad.estado = 9;
                    this._DBcontext.Engagement_Empleado.Update(eEntidad);
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

        public bool TieneTareasCargadas(long id_empleado, long id_engagement)
        {
            try
            {
                var listaTareas = _DBcontext.Empleado_Hoja_Trabajo.Where(p => p.estado == 0 && p.id_empleado == id_empleado && p.id_engagement == id_engagement);
                if (listaTareas == null) return false;

                return (listaTareas.Count() > 0);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public Engagement_Empleado GetEntity(long id)
        {
            try
            {
                var eEntidad = _DBcontext.Engagement_Empleado.FirstOrDefault(p => p.estado == 0 && p.id == id);
                if (eEntidad != null)
                {
                    var eEmpleado = this._DBcontext.Empleado.FirstOrDefault(emp => emp.id == eEntidad.id_empleado);
                    eEntidad.nombre_empleado = (eEmpleado == null) ? "EMPLEADO NO ASIGNADO" : eEmpleado.nombre;

                    var listaDetalle = this._DBcontext.Detalle_Engagement_Empleado.Where(dtll => dtll.id_engagement_empleado == eEntidad.id && dtll.estado == 0);
                    eEntidad.cantidad_horas_habilitadas = (listaDetalle == null) ? 0 : listaDetalle.Sum(dtll => dtll.cantidad_horas);
                }

                return eEntidad;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public Engagement_Empleado GetEntity(long id_engagement, long id_empleado)
        {
            try
            {
                var eEntidad = _DBcontext.Engagement_Empleado.FirstOrDefault(p => p.estado == 0 && p.id_engagement == id_engagement && p.id_empleado == id_empleado);
                if (eEntidad != null)
                {
                    var eEmpleado = this._DBcontext.Empleado.FirstOrDefault(emp => emp.id == eEntidad.id_empleado);
                    eEntidad.nombre_empleado = (eEmpleado == null) ? "EMPLEADO NO ASIGNADO" : eEmpleado.nombre;

                    var listaDetalle = this._DBcontext.Detalle_Engagement_Empleado.Where(dtll => dtll.id_engagement_empleado == eEntidad.id && dtll.estado == 0);
                    eEntidad.cantidad_horas_habilitadas = (listaDetalle == null) ? 0 : listaDetalle.Sum(dtll => dtll.cantidad_horas);
                }

                return eEntidad;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Detalle_Engagement_Empleado> GetEntityDetalle(long id_engagement, long id_empleado)
        {
            try
            {
                string sQuery = $@"
                    SELECT dtl.*
                    FROM PUBLIC.detalle_engagement_empleado dtl
                    JOIN PUBLIC.engagement_empleado ee on ee.id = dtl.id_engagement_empleado
                    WHERE ee.id_engagement = {id_engagement} and ee.id_empleado = {id_empleado} and dtl.estado = 0 and ee.estado = 0

                ";

                return this._DBcontext.Detalle_Engagement_Empleado.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public Detalle_Engagement_Empleado GetEntityDetalle(long id_engagement, long id_empleado, DateTime fecha)
        {
            try
            {
                string sQuery = $@"
                    SELECT dtl.*
                    FROM PUBLIC.detalle_engagement_empleado dtl
                    JOIN PUBLIC.engagement_empleado ee on ee.id = dtl.id_engagement_empleado
                    WHERE ee.id_engagement = {id_engagement} and ee.id_empleado = {id_empleado} and dtl.estado = 0 and ee.estado = 0
                    and '{fecha.ToString("yyyy-MM-dd")}' >= dtl.fecha_inicio and '{fecha.ToString("yyyy-MM-dd")}' <= dtl.fecha_fin
                ";
                Detalle_Engagement_Empleado eDetalle = this._DBcontext.Detalle_Engagement_Empleado.FromSqlRaw(sQuery).FirstOrDefault();
                return eDetalle;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Engagement_Empleado> GetLista()
        {
            throw new NotImplementedException();
        }

        public List<Detalle_Engagement_Empleado> GetListaEngagementEmpleadoDetalle(long id_engagement)
        {
            try
            {
                string sQuery = $@"
                    SELECT *
                    FROM PUBLIC.detalle_engagement_empleado
                    WHERE estado = 0 and id_engagement_empleado in (
		                SELECT id FROM PUBLIC.engagement_empleado where id_engagement = {id_engagement} and estado = 0
                    )
                ";

                return this._DBcontext.Detalle_Engagement_Empleado.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Public.Empleado> GetListaEmpleadoByEngagement(long id_engagement)
        {
            try
            {
                string sQuery = $@"
                    SELECT *
                    FROM PUBLIC.empleado
                    WHERE estado = 0 and id in (
                        SELECT id_empleado FROM PUBLIC.engagement_empleado WHERE id_engagement = {id_engagement} and estado = 0
                    )

                ";

                var listaEmpleado = this._DBcontext.Empleado.FromSqlRaw(sQuery).ToList();

                if (listaEmpleado != null)
                {
                    listaEmpleado = listaEmpleado.Select(e =>
                    {
                        var eEngagementEmpleado = this._DBcontext.Engagement_Empleado.FirstOrDefault(d => d.id_engagement == id_engagement && d.id_empleado == e.id && d.estado == 0);
                        if (eEngagementEmpleado == null)
                            e.horas_asignadas = 0;
                        else
                            e.horas_asignadas = this._DBcontext.Detalle_Engagement_Empleado.Where(dtl => dtl.id_engagement_empleado == eEngagementEmpleado.id && dtl.estado == 0).Sum(dtl => dtl.cantidad_horas);

                        e.nombre = "(" + e.horas_asignadas + ") " + e.nombre;
                        return e;
                    }).ToList();
                }

                return listaEmpleado;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Engagement_Empleado> GetListaEngagementEmpleado(long id_engagement)
        {
            try
            {
                var listaEngagement_Empleado = _DBcontext.Engagement_Empleado.Where(u => u.id_engagement == id_engagement && u.estado == 0).ToList();
                if (listaEngagement_Empleado != null)
                {
                    listaEngagement_Empleado = listaEngagement_Empleado.Select(e =>
                    {
                        var eEmpleado = this._DBcontext.Empleado.FirstOrDefault(emp => emp.id == e.id_empleado);
                        e.nombre_empleado = (eEmpleado == null) ? "EMPLEADO NO ASIGNADO" : eEmpleado.nombre + " " + eEmpleado.apellido_paterno + " " + eEmpleado.apellido_materno;

                        var listaDetalle = this._DBcontext.Detalle_Engagement_Empleado.Where(dtll => dtll.id_engagement_empleado == e.id && dtll.estado == 0);
                        e.cantidad_horas_habilitadas = (listaDetalle == null) ? 0 : listaDetalle.Sum(dtll => dtll.cantidad_horas);
                        return e;
                    }).ToList();
                }

                return listaEngagement_Empleado;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Detalle_Engagement_Empleado> GetListaDetalle_Engagement_Empleado(long id_engagement_empleado)
        {
            try
            {
                return _DBcontext.Detalle_Engagement_Empleado.Where(u => u.estado == 0 && u.id_engagement_empleado == id_engagement_empleado).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Helpers.AppointmentData> GetListaAsignadasByEmpleado(long id_empleado, long id_engagement_seleccionado)
        {
            try
            {
                var listaAppointmentData = (from dtl in _DBcontext.Detalle_Engagement_Empleado
                                            join ee in _DBcontext.Engagement_Empleado on dtl.id_engagement_empleado equals ee.id
                                            join eng in _DBcontext.Engagement on ee.id_engagement equals eng.id
                                            where ee.id_empleado == id_empleado && dtl.estado == 0 && ee.estado == 0
                                                 && eng.estado == 0
                                            select new Entities.Helpers.AppointmentData()
                                            {
                                                Subject = $"{dtl.cantidad_horas} Hrs. asignadas",
                                                IdEngagement = eng.id,
                                                IdDetalleEngagementEmpleado = dtl.id,
                                                Engagement = eng.titulo,
                                                StartTime = dtl.fecha_inicio,
                                                EndTime = dtl.fecha_fin,
                                                HorasAsignadas = dtl.cantidad_horas,
                                                IsReadonly = true,
                                                FullDay = true,
                                                PrimaryColor = (eng.id == id_engagement_seleccionado ? "#317ab9" : "#ff6700"),
                                                CurrentEngagement = eng.id == id_engagement_seleccionado
                                            }).ToList();

                return listaAppointmentData;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}