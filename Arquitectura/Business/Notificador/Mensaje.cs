using Data;
using System;
using System.Collections.Generic;
using System.Text;

using ENO = Entities.Notificador;

namespace Business.Notificador
{
    public class Mensaje : Base
    {
        public Mensaje(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(ENO.Mensaje eMensaje)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Mensaje.Add(eMensaje);
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
    }
}