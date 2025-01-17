using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPU = Entities.Public;

namespace Business.Public
{
    public class AlertaMarcacionErronea : Base, IBusiness<EPU.AlertaMarcacionErronea>
    {
        public AlertaMarcacionErronea(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.AlertaMarcacionErronea eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.AlertaMarcacionErronea.Add(eEntidad);
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

        public bool Modificar(EPU.AlertaMarcacionErronea eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.AlertaMarcacionErronea.Update(eEntidad);
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

        public EPU.AlertaMarcacionErronea GetEntity(long id)
        {
            throw new NotImplementedException();
        }

        public List<EPU.AlertaMarcacionErronea> GetLista()
        {
            throw new NotImplementedException();
        }

        public EPU.AlertaMarcacionErronea Get()
        {
            try
            {
                return _DBcontext.AlertaMarcacionErronea.FirstOrDefault();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}