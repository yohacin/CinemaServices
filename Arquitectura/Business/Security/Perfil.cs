using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using ESE = Entities.Security;

namespace Business.Security
{
    public class Perfil : Base, IBusiness<ESE.Perfil>
    {
        public Perfil(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(ESE.Perfil eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Perfil.Add(eEntidad);
                    this._DBcontext.SaveChanges();

                    //guardar los accesos
                    eEntidad.listaPerfil_Acceso = eEntidad.listaPerfil_Acceso.Select(e => { e.id_perfil = eEntidad.id; return e; }).ToList();
                    this._DBcontext.Perfil_Acceso.AddRange(eEntidad.listaPerfil_Acceso);
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

        public bool Modificar(ESE.Perfil eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Perfil.Update(eEntidad);
                    this._DBcontext.SaveChanges();

                    //eliminar accesos anteriores
                    this._DBcontext.Perfil_Acceso.RemoveRange(this._DBcontext.Perfil_Acceso.Where(e => e.id_perfil == eEntidad.id));
                    this._DBcontext.SaveChanges();

                    //guardar los accesos
                    eEntidad.listaPerfil_Acceso = eEntidad.listaPerfil_Acceso.Select(e => { e.id_perfil = eEntidad.id; return e; }).ToList();
                    this._DBcontext.Perfil_Acceso.AddRange(eEntidad.listaPerfil_Acceso);
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
                    ESE.Perfil eEntidad = this.GetEntity(id);
                    if (eEntidad == null) throw new Exception("Perfil Inexistente!!...");

                    eEntidad.baja = true;
                    this._DBcontext.Perfil.Update(eEntidad);
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

        public ESE.Perfil GetEntity(long id)
        {
            try
            {
                return _DBcontext.Perfil.FirstOrDefault(p => !p.baja && p.id == id);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ESE.Perfil> GetLista()
        {
            try
            {
                return _DBcontext.Perfil.Where(u => !u.baja && u.estado).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ESE.Perfil_Acceso> GetPerfil_Acceso(long id_perfil)
        {
            try
            {
                return _DBcontext.Perfil_Acceso.Where(u => u.id_perfil == id_perfil).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public IQueryable<ESE.Perfil> GetListaQueryable()
        {
            try
            {
                return _DBcontext.Perfil.Where(u => !u.baja && u.estado);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Helpers.TreeCollection> GetListaModuloAcceso()
        {
            try
            {
                string sQuery = @"
                 SELECT m.id as Id, 0 as parentId, false as isChecked, true as hasChild, m.nombre as sNombre
	                 FROM security.modulo m
	                 WHERE m.id in (select id_modulo from security.acceso where estado)
                 UNION
                 SELECT a.id + (a.id_modulo*1000) Id, a.id_modulo parentId, false as isChecked, false as hasChild, a.nombre as sNombre
	                 FROM security.acceso a
	                 WHERE a.estado
                 ORDER BY 2, 1";

                return _DBcontext.TreeCollection.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Helpers.TreeCollection> GetListaModuloAccesoExcudePerfil(long id_perfil)
        {
            try
            {
                string sQuery = $@"
                SELECT m.id as Id, 0 as parentId, false as isChecked, true as hasChild, m.nombre as sNombre
                     FROM security.modulo m
                     WHERE m.id in (select id_modulo from security.acceso where estado and id not in (select id_acceso from security.perfil_acceso where id_perfil = {id_perfil}))
                UNION
                SELECT a.id + (a.id_modulo * 1000) Id, a.id_modulo parentId, false as isChecked, false as hasChild, a.nombre as sNombre
                     FROM security.acceso a
                     WHERE a.estado and id not in (select id_acceso from security.perfil_acceso where id_perfil = {id_perfil})
                ORDER BY 2, 1";

                return _DBcontext.TreeCollection.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}