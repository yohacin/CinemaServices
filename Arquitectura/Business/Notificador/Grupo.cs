using Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using ENO = Entities.Notificador;

namespace Business.Notificador
{
    public class Grupo : Base
    {
        public Grupo(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        /// <summary>
        /// Metodo que Guarda un ENO.Grupo
        /// </summary>
        /// <param name="eGrupo"></param>
        /// <returns>Retorna un bool</returns>
        public bool Guardar(ENO.Grupo eGrupo)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Grupo.Add(eGrupo);
                    this._DBcontext.SaveChanges();

                    if (eGrupo.tipo == 1)
                    {
                        eGrupo.oLstContactos = eGrupo.oLstContactos.Select(c =>
                        {
                            c.codigo_grupo = eGrupo.codigo_grupo;
                            return c;
                        }).ToList();
                        this._DBcontext.Contacto.AddRange(eGrupo.oLstContactos);
                        this._DBcontext.SaveChanges();
                    }

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

        public bool Modificar(ENO.Grupo eGrupo)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Grupo.Update(eGrupo);
                    this._DBcontext.SaveChanges();

                    this._DBcontext.Contacto.RemoveRange(this._DBcontext.Contacto.Where(c => c.codigo_grupo == eGrupo.codigo_grupo));
                    this._DBcontext.SaveChanges();

                    if (eGrupo.tipo == 1)
                    {
                        eGrupo.oLstContactos = eGrupo.oLstContactos.Select(c =>
                        {
                            c.codigo_grupo = eGrupo.codigo_grupo;
                            return c;
                        }).ToList();
                        this._DBcontext.Contacto.AddRange(eGrupo.oLstContactos);
                        this._DBcontext.SaveChanges();
                    }

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

        public List<ENO.Grupo> ListGrupos()
        {
            try
            {
                return _DBcontext.Grupo.Where(g => g.marca_eliminado == 0).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                return null;
            }
        }

        public ENO.Grupo Buscar(long id)
        {
            try
            {
                ENO.Grupo entity = this._DBcontext.Grupo.FirstOrDefault(g => g.codigo_grupo == id);
                if (entity != null)
                {
                    entity.oLstContactos = this._DBcontext.Contacto.Where(c => c.codigo_grupo == entity.codigo_grupo && c.marca_eliminado == 0).ToList();
                }
                return entity;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ENO.Grupo> ListGruposAdministrables()
        {
            try
            {
                List<ENO.Grupo> grupos = this._DBcontext.Grupo.Where(g => g.administrable == true && g.marca_eliminado == 0).ToList();

                if (grupos.Count > 0)
                {
                    List<long> idsGrupos = grupos.Select(g => g.codigo_grupo).ToList();

                    Contacto bContacto = new Contacto(this._DBcontext);

                    List<ENO.Contacto> contactos = bContacto.ListByGrupos(idsGrupos);

                    foreach (ENO.Grupo grupo in grupos)
                    {
                        grupo.oLstContactos = contactos.Where(c => c.codigo_grupo == grupo.codigo_grupo && c.marca_eliminado == 0).ToList();
                    }
                }

                return grupos;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                return null;
            }
        }

        public List<ENO.Grupo> ListarByCampana(long codigoCampana)
        {
            try
            {
                List<ENO.Grupo> grupos =
                    (from g in _DBcontext.Grupo
                     join cd in _DBcontext.Campana_Detalle on g.codigo_grupo equals cd.codigo_grupo
                     where cd.codigo_campana == codigoCampana
                     select g).ToList();

                if (grupos.Count > 0)
                {
                    List<long> idsGrupos = grupos.Select(g => g.codigo_grupo).ToList();

                    Contacto bContacto = new Contacto(this._DBcontext);

                    List<ENO.Contacto> contactos = bContacto.ListByGrupos(idsGrupos);

                    foreach (ENO.Grupo grupo in grupos)
                    {
                        grupo.oLstContactos = contactos.Where(c => c.codigo_grupo == grupo.codigo_grupo && c.marca_eliminado == 0).ToList();
                    }
                }

                return grupos;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ENO.Grupo> ListGrupoByCodCampana(long codCampana)
        {
            try
            {
                IEnumerable<ENO.Grupo> oListaGrupo =
                 from g in _DBcontext.Grupo
                 join cd in _DBcontext.Campana_Detalle on g.codigo_grupo equals cd.codigo_grupo
                 where cd.codigo_campana == codCampana

                 select new ENO.Grupo
                 {
                     codigo_grupo = g.codigo_grupo,
                     nombre = g.nombre,
                     descripcion = g.descripcion,
                     tipo = g.tipo,
                     query = g.query,
                     activo = g.activo,
                     marca_eliminado = g.marca_eliminado
                 };

                return new List<ENO.Grupo>(oListaGrupo);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool Eliminar(long id)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    ENO.Grupo eGrupo = this.Buscar(id);
                    if (eGrupo == null)
                    {
                        throw new Exception("Grupo Inexistente!!...");
                    }
                    eGrupo.marca_eliminado = 9;
                    this._DBcontext.Entry(eGrupo).State = EntityState.Modified;
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

        public List<Object> ObtenerPreviewQuery(String query)
        {
            try
            {
                //List<NpgsqlParameter> oParamList = new List<NpgsqlParameter>();
                List<Object> queryResult = _DBcontext.CollectionFromSql(query).ToList<Object>();

                return queryResult;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}