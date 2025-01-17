using Data;
using System.Collections.Generic;
using System.Linq;
using System;
using EPU = Entities.Public;

namespace Business.Public;


public class Ciudad : Base, IBusiness<EPU.Ciudad>
{
    public Ciudad(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
    {

    }

    public EPU.Ciudad GetEntity(long id)
    {
        try
        {
            EPU.Ciudad eCiudad = _DBcontext.Ciudad.FirstOrDefault(p => p.marca_eliminado == 0 && p.id == id);
            if (eCiudad == null) return null;
            return eCiudad;
        }
        catch (Exception)
        {
            //this._log.Error(ex);
            throw;
        }
    }

    public List<EPU.Ciudad> GetLista()
    {
        try
        {
            return _DBcontext.Ciudad.Where(e => e.marca_eliminado == 0).ToList();
        }
        catch (Exception)
        {

            throw;
        }
    }

    public bool Guardar(EPU.Ciudad eEntidad)
    {
        using (var oTrans = this._DBcontext.Database.BeginTransaction())
        {
            try
            {
                this._DBcontext.Ciudad.Add(eEntidad);
                this._DBcontext.SaveChanges();

                oTrans.Commit();
                return true;
            }
            catch (Exception)
            {
                oTrans.Rollback();
                //this._log.Error(ex);
                throw;
            }
        }
    }

    public bool Modificar(EPU.Ciudad eEntidad)
    {
        using (var oTrans = this._DBcontext.Database.BeginTransaction())
        {
            try
            {
                this._DBcontext.Ciudad.Update(eEntidad);
                this._DBcontext.SaveChanges();

                oTrans.Commit();
                return true;
            }
            catch (Exception)
            {
                oTrans.Rollback();
                //this._log.Error(ex);
                throw;
            }
        }
    }

    public bool Eliminar(long id)
    {
        using (var oTrans = this._DBcontext.Database.BeginTransaction())
        {
            try
            {
                EPU.Ciudad eEntidad = this.GetEntity(id);
                if (eEntidad == null) throw new Exception("Ciudad Inexistente!!...");

                eEntidad.marca_eliminado = 9;
                this._DBcontext.Ciudad.Update(eEntidad);
                this._DBcontext.SaveChanges();

                oTrans.Commit();
                return true;
            }
            catch (Exception)
            {
                oTrans.Rollback();
                //this._log.Error(ex);
                throw;
            }
        }
    }

}