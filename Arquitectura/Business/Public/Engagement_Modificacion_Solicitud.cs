using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using EPU = Entities.Public;

namespace Business.Public
{
    public class Engagement_Modificacion_Solicitud : Base, IBusiness<EPU.Engagement_Modificacion_Solicitud>
    {
        public Engagement_Modificacion_Solicitud(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.Engagement_Modificacion_Solicitud eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Engagement_Modificacion_Solicitud.Add(eEntidad);
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

        public bool Modificar(EPU.Engagement_Modificacion_Solicitud eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Engagement_Modificacion_Solicitud.Update(eEntidad);
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
                    EPU.Engagement_Modificacion_Solicitud eEntidad = this.GetEntity(id);
                    if (eEntidad == null) throw new Exception("Entidad Inexistente!!...");

                    eEntidad.estado = 9;
                    this._DBcontext.Engagement_Modificacion_Solicitud.Update(eEntidad);
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

        public EPU.Engagement_Modificacion_Solicitud GetEntity(long id)
        {
            try
            {
                return _DBcontext.Engagement_Modificacion_Solicitud.FirstOrDefault(p => p.estado == 0 && p.id == id);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Engagement_Modificacion_Solicitud GetEntityPendienteByEngagement(long id_engagement)
        {
            try
            {
                return _DBcontext.Engagement_Modificacion_Solicitud.FirstOrDefault(p => p.estado == 0 && p.estado_solicitud == 0 && p.id_engagement == id_engagement);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Engagement_Modificacion_Solicitud> GetLista()
        {
            try
            {
                var listaTarea = _DBcontext.Engagement_Modificacion_Solicitud.Where(u => u.estado == 0).ToList();

                return listaTarea;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Engagement_Modificacion_Solicitud> GetListaByEmpleado(long id_empleado)
        {
            try
            {
                string sQuery = $@"
                    SELECT sol.*
                    FROM PUBLIC.engagement_modificacion_solicitud sol
                    JOIN PUBLIC.responsable_engagement res on res.id_engagement = sol.id_engagement
                    WHERE sol.estado = 0 and sol.estado_solicitud = 0 and res.id_usuario = {id_empleado}
                ";

                var listaSolicitud = _DBcontext.Engagement_Modificacion_Solicitud.FromSqlRaw(sQuery).ToList();
                if (listaSolicitud != null)
                {
                    listaSolicitud = listaSolicitud.Select(sol =>
                    {
                        sol.nombre_solicitante = this._DBcontext.Empleado.FirstOrDefault(e => e.id == sol.id_usuario_solicita).nombre;
                        return sol;
                    }).ToList();
                }

                return listaSolicitud;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Engagement_Modificacion_Solicitud> GetListaByEngagement(long id_engagement)
        {
            try
            {
                var listaSolicitud = _DBcontext.Engagement_Modificacion_Solicitud.Where(u => u.estado == 0 && u.id_engagement == id_engagement).ToList();

                if (listaSolicitud != null)
                {
                    listaSolicitud = listaSolicitud.Select(sol =>
                    {
                        var eSolicita = this._DBcontext.Empleado.FirstOrDefault(e => e.id == sol.id_usuario_solicita);
                        var eAutoriza = this._DBcontext.Empleado.FirstOrDefault(e => e.id == sol.id_usuario_autoriza);
                        sol.nombre_solicitante = (eSolicita == null) ? "SIN NOMBRE" : eSolicita.nombre;
                        sol.nombre_autorizador = (eAutoriza == null) ? "SIN NOMBRE" : eAutoriza.nombre;
                        return sol;
                    }).ToList();
                }

                return listaSolicitud;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}