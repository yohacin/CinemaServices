using System;
using System.Collections.Generic;

//Librerias Externas
using System.Linq;
using Data;
using Microsoft.EntityFrameworkCore;
using ESE = Entities.Security;

namespace Business.Security
{
    public class Usuario : Base
    {
        public Usuario(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(ESE.Usuario eUsuario)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Usuario.Add(eUsuario);
                    this._DBcontext.SaveChanges();

                    //guardar los accesos
                    eUsuario.listaUsuario_Acceso = eUsuario.listaUsuario_Acceso.Select(e => { e.id_usuario = eUsuario.id; return e; }).ToList();
                    this._DBcontext.Usuario_Acceso.AddRange(eUsuario.listaUsuario_Acceso);
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

        public bool Modificar(ESE.Usuario eUsuario)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Update(eUsuario);
                    this._DBcontext.SaveChanges();

                    //eliminar accesos anteriores
                    this._DBcontext.Usuario_Acceso.RemoveRange(this._DBcontext.Usuario_Acceso.Where(e => e.id_usuario == eUsuario.id));
                    this._DBcontext.SaveChanges();

                    //guardar los accesos
                    eUsuario.listaUsuario_Acceso = eUsuario.listaUsuario_Acceso.Select(e => { e.id_usuario = eUsuario.id; return e; }).ToList();
                    this._DBcontext.Usuario_Acceso.AddRange(eUsuario.listaUsuario_Acceso);
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

        public bool setContrasena(long id, String nueva_contrasena)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    ESE.Usuario eUsuario = this.GetEntity(id);
                    eUsuario.clave = nueva_contrasena;
                    this._DBcontext.Entry(eUsuario).State = EntityState.Modified;
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
                    ESE.Usuario eUsuario = this.GetEntity(id);
                    if (eUsuario == null) throw new Exception("Usuario Inexistente!!...");

                    eUsuario.baja = true;
                    this._DBcontext.Entry(eUsuario).State = EntityState.Modified;
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

        public ESE.Usuario GetEntity(long id)
        {
            try
            {
                return _DBcontext.Usuario.FirstOrDefault(p => !p.baja && p.id == id);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public ESE.Usuario GetUsuarioByEmail(string email)
        {
            try
            {
                return _DBcontext.Usuario.FirstOrDefault(p => !p.baja && p.email == email);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public IQueryable<ESE.Usuario> GetListaUsuarios(long codigo_empresa = 0)
        {
            try
            {
                if (codigo_empresa == 0)
                {
                    var lstUsuarioAux = this._DBcontext.Usuario.Where(oUser => !oUser.baja).OrderByDescending(c => c.id);

                    return lstUsuarioAux;
                }
                else
                {
                    var lstUsuario = this._DBcontext.Usuario
                                     .FromSqlRaw($@"
                                            SELECT DISTINCT u.*
                                            FROM security.usuario u
                                            JOIN security.usuario_perfil_empresa em
                                            ON em.codigo_usuario = u.codigo_usuario and em.codigo_empresa = {codigo_empresa}
                                            WHERE u.marca_eliminado != 9
                                            ORDER BY u.codigo_usuario DESC");
                    return lstUsuario;
                }
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ESE.Usuario> GetLista()
        {
            try
            {
                var listaUsuario = _DBcontext.Usuario.Where(u => !u.baja && u.estado).ToList();
                if (listaUsuario != null)
                {
                    listaUsuario = listaUsuario.Select(e =>
                    {
                        var ePerfil = this._DBcontext.Perfil.FirstOrDefault(p => p.id == e.id_perfil);
                        e.perfil = (ePerfil == null) ? "SIN PERFIL" : ePerfil.nombre;
                        return e;
                    }).ToList();
                }

                return listaUsuario;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ESE.Usuario_Acceso> GetUsuario_Acceso(long id_usuario)
        {
            try
            {
                return _DBcontext.Usuario_Acceso.Where(u => u.id_usuario == id_usuario).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public ESE.Usuario Login(string usuario, string contrasena)
        {
            try
            {
                ESE.Usuario eUsuario = _DBcontext.Usuario.FirstOrDefault(u => u.usuario.ToUpper() == usuario.ToUpper() && u.clave == contrasena);
                return eUsuario;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ESE.Modulo> GetModulosxUsuario(long id_usuario)
        {
            try
            {
                string sQuery = $@"
                    select * from security.modulo m
                    where m.id in (
                        select a.id_modulo
                        from security.usuario u
                        join security.perfil_acceso pa on u.id_perfil = pa.id_perfil
                        join security.acceso a on pa.id_acceso = a.id
                        where u.id = {id_usuario} and a.estado = true
                    )
                    UNION
                    select* from security.modulo m
                    where m.id in (
                        select a.id_modulo
                        from security.usuario_acceso ua
                        join security.acceso a on ua.id_acceso = a.id
                        where ua.id_usuario = {id_usuario} and a.estado = true
                    )
                    ORDER BY id

                ";
                return this._DBcontext.Modulo.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<ESE.Acceso> GetAccesoxUsuario(long id_usuario)
        {
            try
            {
                string sQuery = $@"
                    select a.*
                    from security.usuario u
                    join security.perfil_acceso pa on u.id_perfil = pa.id_perfil
                    join security.acceso a on pa.id_acceso = a.id
                    where u.id = {id_usuario} and a.estado = true
                    UNION
                    select a.*
                    from security.usuario_acceso ua
                    join security.acceso a on ua.id_acceso = a.id
                    where ua.id_usuario = {id_usuario} and a.estado = true
                    ORDER BY id_modulo, secuencia

                ";
                return this._DBcontext.Acceso.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}