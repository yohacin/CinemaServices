using Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ENO = Entities.Notificador;

namespace Business.Notificador
{
    public class Adjunto : Base
    {
        public Adjunto(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(ENO.Adjunto eAdjunto)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Adjunto.Add(eAdjunto);
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

        public List<ENO.Adjunto> ListarByPlantilla(long idPlantilla)
        {
            try
            {
                return this._DBcontext.Adjunto.Where(a => a.id_plantilla == idPlantilla).OrderBy(a => a.id).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                return new List<ENO.Adjunto>();
            }
        }

        public bool EliminarByPlantilla(long idPlantilla)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    string sDeleteString = @"DELETE FROM notificador.adjunto WHERE id_plantilla = @p_idPlantilla";
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
    }
}