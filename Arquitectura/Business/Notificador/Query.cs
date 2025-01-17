using Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#region Abreviaturas

using ENO = Entities.Notificador;

#endregion Abreviaturas

namespace Business.Notificador
{
    internal class Query : Base
    {
        public Query(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(ENO.Query entity)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Query.Add(entity);
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

        public bool EliminarByPlantilla(long idPlantilla)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    string sDeleteString = @"DELETE FROM notificador.query WHERE id_plantilla = @p_idPlantilla";
                    this._DBcontext.Database.ExecuteSqlRaw(sDeleteString, new NpgsqlParameter("@p_idPlantilla", idPlantilla));

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

        public List<ENO.Query> ListarByPlantilla(long idPlantilla)
        {
            try
            {
                return this._DBcontext.Query.Where(q => q.id_plantilla == idPlantilla).OrderBy(q => q.id_query).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}