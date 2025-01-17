using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EPU = Entities.Public;

namespace Business.Public
{
    public class Area_Engagement : Base, IBusiness<EPU.Area_Engagement>
    {
        public Area_Engagement(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.Area_Engagement eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Area_Engagement.Add(eEntidad);
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

        public bool Modificar(EPU.Area_Engagement eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Area_Engagement.Update(eEntidad);
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
                    EPU.Area_Engagement eEntidad = this.GetEntity(id);

                    bool inUse = this._DBcontext.Engagement.Where(x => x.id_area == id).ToList().Count > 0;

                    if (eEntidad == null) throw new Exception("Area Inexistente!!...");
                    if (inUse) throw new Exception("El area esta asociado a uno o más engagements!!...");

                    eEntidad.estado = 9;
                    this._DBcontext.Area_Engagement.Update(eEntidad);
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

        public EPU.Area_Engagement GetEntity(long id)
        {
            try
            {
                return _DBcontext.Area_Engagement.FirstOrDefault(p => p.estado == 0 && p.id == id);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Area_Engagement> GetLista()
        {
            try
            {
                return _DBcontext.Area_Engagement.Where(u => u.estado == 0).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}