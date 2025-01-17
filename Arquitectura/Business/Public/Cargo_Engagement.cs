using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EPU = Entities.Public;

namespace Business.Public
{
    public class Cargo_Engagement : Base, IBusiness<EPU.Cargo_Engagement>
    {
        public Cargo_Engagement(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.Cargo_Engagement eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Cargo_Engagement.Add(eEntidad);
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

        public bool Modificar(EPU.Cargo_Engagement eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Cargo_Engagement.Update(eEntidad);
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
                    EPU.Cargo_Engagement eEntidad = this.GetEntity(id);
                    if (eEntidad == null) throw new Exception("Cargo Inexistente!!...");

                    eEntidad.estado = 9;
                    this._DBcontext.Cargo_Engagement.Update(eEntidad);
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

        public EPU.Cargo_Engagement GetEntity(long id)
        {
            try
            {
                return _DBcontext.Cargo_Engagement.FirstOrDefault(p => p.estado == 0 && p.id == id);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Cargo_Engagement GetEntityByNombre(string nombre)
        {
            try
            {
                return _DBcontext.Cargo_Engagement.FirstOrDefault(p => p.estado == 0 && p.nombre.ToUpper() == nombre.ToUpper());
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Cargo_Engagement> GetLista()
        {
            try
            {
                return _DBcontext.Cargo_Engagement.Where(u => u.estado == 0).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}