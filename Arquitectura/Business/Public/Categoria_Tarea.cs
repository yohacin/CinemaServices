using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EPU = Entities.Public;

namespace Business.Public
{
    public class Categoria_Tarea : Base, IBusiness<EPU.Categoria_Tarea>
    {
        public Categoria_Tarea(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.Categoria_Tarea eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Categoria_Tarea.Add(eEntidad);
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

        public bool Modificar(EPU.Categoria_Tarea eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Categoria_Tarea.Update(eEntidad);
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
                    EPU.Categoria_Tarea eEntidad = this.GetEntity(id);
                    if (eEntidad == null) throw new Exception("Categoria Inexistente!!...");

                    eEntidad.estado = 9;
                    this._DBcontext.Categoria_Tarea.Update(eEntidad);
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

        public EPU.Categoria_Tarea GetEntity(long id)
        {
            try
            {
                return _DBcontext.Categoria_Tarea.FirstOrDefault(p => p.estado == 0 && p.id == id);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public bool EstaUsado(long id)
        {
            try
            {
                string sQuery = $@"
	                SELECT tar.*
	                FROM PUBLIC.tarea tar
	                WHERE tar.id_categoria = {id} and tar.id in (SELECT id_tarea FROM PUBLIC.tarea_engagement WHERE estado = 0)
                ";
                var listaTarea = _DBcontext.Tarea.FromSqlRaw(sQuery).ToList();
                if (listaTarea == null) return false;

                return (listaTarea.Count > 0);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Categoria_Tarea GetEntityByNombre(string nombre)
        {
            try
            {
                return _DBcontext.Categoria_Tarea.FirstOrDefault(p => p.estado == 0 && p.nombre.ToUpper() == nombre.ToUpper());
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Categoria_Tarea> GetLista()
        {
            try
            {
                return _DBcontext.Categoria_Tarea.Where(u => u.estado == 0).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}