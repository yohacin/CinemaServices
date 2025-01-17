using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using EPU = Entities.Public;

namespace Business.Public
{
    public class Encuesta : Base, IBusiness<EPU.Encuesta>
    {
        public Encuesta(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.Encuesta eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Encuesta.Add(eEntidad);
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

        public bool Modificar(EPU.Encuesta eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Encuesta.Update(eEntidad);
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
            throw new NotImplementedException();
        }

        public EPU.Encuesta GetEntity(long id)
        {
            throw new NotImplementedException();
        }

        public List<EPU.Encuesta> GetLista()
        {
            try
            {
                return _DBcontext.Encuesta.Where(u => u.estado == 0).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Encuesta GetEntityByEmpleado(long id_empleado, DateTime fecha)
        {
            try
            {
                string sQuery = $@"
                SELECT *
                FROM PUBLIC.encuesta
                WHERE id_empleado = {id_empleado} and fecha::DATE = '{fecha.ToString("yyyy-MM-dd")}' and estado = 0
                ";
                return _DBcontext.Encuesta.FromSqlRaw(sQuery).FirstOrDefault();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}