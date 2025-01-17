using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#region Abreviaturas

using ENO = Entities.Notificador;

#endregion Abreviaturas

namespace Business.Notificador
{
    public class Query_Campana : Base
    {
        public Query_Campana(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public List<ENO.Query_Campana> List()
        {
            return this._DBcontext.Query_Campana.Where(e => e.marca_eliminado == 0).ToList();
        }

        public List<ENO.Query_Campana> ListByEmpresa(long id)
        {
            return this._DBcontext.Query_Campana.Where(q => q.id_empresa == id).ToList();
        }

        public ENO.Query_Campana Get(long id)
        {
            return this._DBcontext.Query_Campana.FirstOrDefault(e => e.id == id);
        }

        public long Guardar(ENO.Query_Campana eQuery)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Query_Campana.Add(eQuery);
                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return eQuery.id;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }

        public bool Modificar(ENO.Query_Campana eQuery)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Entry(eQuery).State = EntityState.Modified;
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
                    ENO.Query_Campana eQuery = this.Get(id);

                    if (eQuery == null) throw new Exception("¡ Query Inexistente !");

                    eQuery.marca_eliminado = 9;
                    this._DBcontext.Entry(eQuery).State = EntityState.Modified;
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