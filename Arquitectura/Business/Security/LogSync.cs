using Data;
using System;
using System.Collections.Generic;
using System.Text;
using ESE = Entities.Security;

namespace Business.Security
{
    public class LogSync : Base, IBusiness<ESE.LogSync>
    {
        public LogSync(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Eliminar(long id)
        {
            throw new NotImplementedException();
        }

        public ESE.LogSync GetEntity(long id)
        {
            throw new NotImplementedException();
        }

        public List<ESE.LogSync> GetLista()
        {
            throw new NotImplementedException();
        }

        public bool Guardar(ESE.LogSync eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.LogSync.Add(eEntidad);
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

        public bool Modificar(ESE.LogSync eEntidad)
        {
            throw new NotImplementedException();
        }
    }
}