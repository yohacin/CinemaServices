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
    public class Contacto : Base
    {
        public Contacto(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        /// <summary>
        /// Metodo que Guarda un ENO.Contacto
        /// </summary>
        /// <param name="eGrupo"></param>
        /// <returns>Retorna un bool</returns>
        public bool Guardar(ENO.Contacto eContacto)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Contacto.Add(eContacto);
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

        /// <summary>
        /// Metodo que Elimina una List<ENO.Contacto>
        /// </summary>
        /// <param name="eGrupo"></param>
        /// <returns>Retorna un bool</returns>
        public bool EliminarByCodGrupo(long codigoGrupo)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    string sDeleteString = @"DELETE FROM notificador.contacto WHERE codigo_grupo = @p_codGrupo";
                    this._DBcontext.Database.ExecuteSqlRaw(sDeleteString, new NpgsqlParameter("@p_codGrupo", codigoGrupo));

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

        public List<ENO.Contacto> ListAll()
        {
            try
            {
                return this._DBcontext.Contacto.Where(c => c.marca_eliminado == 0).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ENO.Contacto> ListContactoByCodGrupo(long codGrupo)
        {
            try
            {
                return this._DBcontext.Contacto.Where(c => c.codigo_grupo == codGrupo && c.marca_eliminado == 0).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ENO.Contacto> ListByGrupos(List<long> idsGrupo)
        {
            try
            {
                return this._DBcontext.Contacto.Where(c => idsGrupo.Contains(c.codigo_grupo)).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}