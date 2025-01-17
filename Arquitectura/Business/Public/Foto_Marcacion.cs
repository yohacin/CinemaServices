using Data;
using System;
using System.Collections.Generic;
using System.Text;

using EPU = Entities.Public;

namespace Business.Public
{
    public class Foto_Marcacion : Base, IBusiness<EPU.Foto_Marcacion>
    {
        public Foto_Marcacion(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.Foto_Marcacion eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Foto_Marcacion.Add(eEntidad);
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

        public bool Modificar(EPU.Foto_Marcacion eEntidad)
        {
            throw new NotImplementedException();
        }

        public bool Eliminar(long id)
        {
            throw new NotImplementedException();
        }

        public EPU.Foto_Marcacion GetEntity(long id)
        {
            throw new NotImplementedException();
        }

        public List<EPU.Foto_Marcacion> GetLista()
        {
            throw new NotImplementedException();
        }
    }
}