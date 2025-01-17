using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EPU = Entities.Public;

namespace Business.Public
{
    public class Tarea : Base, IBusiness<EPU.Tarea>
    {
        public Tarea(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.Tarea eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Tarea.Add(eEntidad);
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

        public bool Modificar(EPU.Tarea eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Tarea.Update(eEntidad);
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
                    EPU.Tarea eEntidad = this.GetEntity(id);
                    if (eEntidad == null) throw new Exception("Tarea Inexistente!!...");

                    eEntidad.estado = 9;
                    this._DBcontext.Tarea.Update(eEntidad);
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

        public EPU.Tarea GetEntity(long id)
        {
            try
            {
                return _DBcontext.Tarea.FirstOrDefault(p => p.estado == 0 && p.id == id);
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
	                WHERE tar.id = {id} and tar.id in (SELECT id_tarea FROM PUBLIC.tarea_engagement WHERE estado = 0)
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

        public EPU.Tarea GetEntityByNombre(string nombre)
        {
            try
            {
                return _DBcontext.Tarea.FirstOrDefault(p => p.estado == 0 && p.nombre.ToUpper() == nombre.ToUpper());
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Tarea> GetLista()
        {
            try
            {
                var listaTarea = _DBcontext.Tarea.Where(u => u.estado == 0).ToList();
                if (listaTarea != null)
                {
                    listaTarea = listaTarea.Select(e =>
                    {
                        var eCategoria = this._DBcontext.Categoria_Tarea.FirstOrDefault(c => c.id == e.id_categoria);
                        e.categoria = (eCategoria == null) ? "SIN CATEGORIA" : eCategoria.nombre;
                        return e;
                    }).ToList();
                }
                return listaTarea;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Helpers.TreeCollection> GetListaCategoriaTarea()
        {
            try
            {
                string sQuery = @"
                 SELECT c.id as Id, 0 as parentId, false as isChecked, true as hasChild, c.nombre as sNombre
	                 FROM PUBLIC.categoria_tarea c
	                 WHERE c.id in (select id_categoria from PUBLIC.tarea where estado = 0) and c.estado = 0
                UNION
                SELECT a.id + (a.id_categoria*1000) Id, a.id_categoria parentId, false as isChecked, false as hasChild, a.nombre as sNombre
	                 FROM PUBLIC.tarea a
	                 WHERE a.estado = 0
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