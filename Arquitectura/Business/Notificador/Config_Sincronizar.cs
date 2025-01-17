using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Business.Notificador
{
    public class Config_Sincronizar : Base
    {
        public Config_Sincronizar(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public long CrearConfig(Entities.Notificador.Config_Sincronizar eConfig)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Config_Sincronizar.Add(eConfig);
                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return eConfig.id;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }

        public bool ModificarConfig(Entities.Notificador.Config_Sincronizar eConfig)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Config_Sincronizar.Update(eConfig);
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

        public Entities.Notificador.Config_Sincronizar GetConfig(int id_empresa = 0)
        {
            try
            {
                return this._DBcontext.Config_Sincronizar.FirstOrDefault();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}