using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESE = Entities.Security;

namespace Business.Security
{
    public class Acceso : Base, IBusiness<ESE.Acceso>
    {
        public Acceso(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(ESE.Acceso eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Acceso.Add(eEntidad);
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

        public bool Modificar(ESE.Acceso eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Acceso.Update(eEntidad);
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
                    ESE.Acceso eEntidad = this.GetEntity(id);
                    if (eEntidad == null) throw new Exception("Acceso Inexistente!!...");

                    this._DBcontext.Acceso.Remove(eEntidad);
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

        public ESE.Acceso GetEntity(long id)
        {
            try
            {
                return _DBcontext.Acceso.FirstOrDefault(p => p.id == id);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ESE.Acceso> GetLista()
        {
            try
            {
                return _DBcontext.Acceso.Where(u => u.estado).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ESE.Modulo> GetModulosxUsuario(long id_usuario, long tipo)
        {
            try
            {
                string sQuery = "";
                if (tipo == 1)
                {  // Tipo Usuario
                    sQuery = $@"
                        SELECT * FROM security.modulo m
                        WHERE m.id in (
                            select a.id_modulo
                            from security.usuario u
                            join security.perfil_acceso pa on u.id_perfil = pa.id_perfil
                            join security.acceso a on pa.id_acceso = a.id
                            where u.id = {id_usuario} and a.estado = true
                        )
                        UNION
                        SELECT * FROM security.modulo m
                        WHERE m.id in (
                            SELECT a.id_modulo
                            FROM security.usuario_acceso ua
                            JOIN security.acceso a on ua.id_acceso = a.id
                            WHERE ua.id_usuario = {id_usuario} and a.estado = true
                        )
                        ORDER BY id
                    ";
                }
                else
                {  // Tipo empleado
                    sQuery = $@"
                        SELECT * FROM security.modulo m
                        WHERE m.id in (
                            select a.id_modulo
                            from PUBLIC.empleado u
                            join security.perfil_acceso pa on u.id_perfil = pa.id_perfil
                            join security.acceso a on pa.id_acceso = a.id
                            where u.id = {id_usuario} and a.estado = true
                        )
                        ORDER BY id
                    ";
                }

                return this._DBcontext.Modulo.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ESE.Acceso> GetAccesoxUsuario(long id_usuario, long tipo)
        {
            try
            {
                string sQuery = "";
                if (tipo == 1)
                {// Tipo Usuario
                    sQuery = $@"
                        SELECT a.*
                        FROM security.usuario u
                        JOIN security.perfil_acceso pa on u.id_perfil = pa.id_perfil
                        JOIN security.acceso a on pa.id_acceso = a.id
                        WHERE u.id = {id_usuario} and a.estado = true
                    UNION
                        SELECT a.*
                        FROM security.usuario_acceso ua
                        JOIN security.acceso a on ua.id_acceso = a.id
                        WHERE ua.id_usuario = {id_usuario} and a.estado = true
                        ORDER BY id_modulo, secuencia

                    ";
                }
                else
                {  // Tipo empleado
                    sQuery = $@"
                        SELECT a.*
                        FROM PUBLIC.empleado u
                        JOIN security.perfil_acceso pa on u.id_perfil = pa.id_perfil
                        JOIN security.acceso a on pa.id_acceso = a.id
                        WHERE u.id = {id_usuario} and a.estado = true
                    ";
                }

                return this._DBcontext.Acceso.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public bool RevisarAcceso(long id_usuario, long tipo, string pageRequest)
        {
            try
            {
                var listaAcceso = this.GetAccesoxUsuario(id_usuario, tipo);
                if (listaAcceso == null) return false;
                if (listaAcceso.Count == 0) return false;
                return (listaAcceso.Where(e => e.url_pagina == pageRequest).ToList().Count > 0);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}