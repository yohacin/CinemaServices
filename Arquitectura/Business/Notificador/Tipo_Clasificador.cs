using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#region Abreviaturas

using ENO = Entities.Notificador;

#endregion Abreviaturas

namespace Business.Notificador
{
    public class Tipo_Clasificador : Base
    {
        public Tipo_Clasificador(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(ENO.Tipo_Clasificador eTipoClasificador)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.TipoClasificador.Add(eTipoClasificador);
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

        public bool Modificar(ENO.Tipo_Clasificador eTipoClasificador)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Update(eTipoClasificador);
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

        public ENO.Tipo_Clasificador Buscar(long id)
        {
            try
            {
                return this._DBcontext.TipoClasificador.FirstOrDefault(tc => tc.id == id);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ENO.Tipo_Clasificador> Listar()
        {
            try
            {
                return this._DBcontext.TipoClasificador.ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}