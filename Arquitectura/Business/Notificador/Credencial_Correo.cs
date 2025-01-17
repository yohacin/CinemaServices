using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#region Abreviaturas

using ENO = Entities.Notificador;
using ESE = Entities.Security;
using BSE = Business.Security;
using Data;

#endregion Abreviaturas

namespace Business.Notificador
{
    public class Credencial_Correo : Base
    {
        public Credencial_Correo(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public long CrearConfig(ENO.Credencial_Correo eConfig)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Credencial_Correo.Add(eConfig);
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

        public bool ModificarConfig(ENO.Credencial_Correo eConfig)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Credencial_Correo.Update(eConfig);
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

        public ENO.Credencial_Correo Buscar(long id)
        {
            try
            {
                ENO.Credencial_Correo entity = this._DBcontext.Credencial_Correo.Where(cc => cc.id == id).FirstOrDefault();

                return entity;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public ENO.Credencial_Correo GetConfig(int id_empresa = 0)
        {
            try
            {
                ENO.Credencial_Correo entity = this._DBcontext.Credencial_Correo.Where(cc => cc.id_empresa == id_empresa).FirstOrDefault();

                return entity;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ENO.Credencial_Correo> Listar()
        {
            try
            {
                List<ENO.Credencial_Correo> entityList = this._DBcontext.Credencial_Correo.ToList();

                return entityList;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}