using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPU = Entities.Public;

namespace Business.Public
{
    public class Excepcion : Base, IBusiness<EPU.Excepcion>
    {
        public Excepcion(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Eliminar(long id)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    EPU.Excepcion eEntidad = this.GetEntity(id);
                    if (eEntidad == null) throw new Exception("No existe el registro...");

                    eEntidad.estado = 9;
                    this._DBcontext.Excepcion.Update(eEntidad);
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

        public bool EliminarExcepcion(EPU.Excepcion eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    if (eEntidad == null) throw new Exception("No existe el registro...");
                    eEntidad.estado = 9;
                    this._DBcontext.Excepcion.Update(eEntidad);
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

        public EPU.Excepcion GetEntity(long id)
        {
            try
            {
                return _DBcontext.Excepcion.FirstOrDefault(p => p.estado == 0 && p.id == id);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Excepcion> GetLista()
        {
            try
            {
                return _DBcontext.Excepcion.Where(e => e.estado == 0).OrderByDescending(e => e.id).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public bool Guardar(EPU.Excepcion eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Excepcion.Add(eEntidad);
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

        public bool Modificar(EPU.Excepcion eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Excepcion.Update(eEntidad);
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

        public EPU.Excepcion buscarExcepcion(EPU.Excepcion eEntidad)
        {
            try
            {
                return _DBcontext.Excepcion.FirstOrDefault(e => e.tipo == eEntidad.tipo &&
                                                                   e.fecha_inicio.Date == eEntidad.fecha_inicio.Date &&
                                                                   e.fecha_fin.Date == eEntidad.fecha_fin.Date &&
                                                                   e.ci == eEntidad.ci);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Excepcion buscarExcepcionXTransIni(long pNtri, string pTipo)
        {
            try
            {
                return _DBcontext.Excepcion.FirstOrDefault(e => e.cod_origen == pNtri && e.tipo == pTipo);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Excepcion BuscarByFechaEmpleado(long id_empleado, DateTime fecha)
        {
            try
            {
                return _DBcontext.Excepcion.FirstOrDefault(e => e.id_empleado == id_empleado &&
                                                                fecha.Date >= e.fecha_inicio.Date &&
                                                                fecha.Date <= e.fecha_fin && e.estado == 0);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}