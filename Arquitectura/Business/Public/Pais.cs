using Data;
using System.Collections.Generic;
using System.Linq;
using System;
using EPU = Entities.Public;

namespace Business.Public;

public class Pais : Base, IBusiness<EPU.Pais>
{
    public Pais(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
    {

    }

    public EPU.Pais GetEntity(long id)
    {
        try
        {
            EPU.Pais ePais = _DBcontext.Pais.FirstOrDefault(p => p.marca_eliminado == 0 && p.id == id);
            if (ePais == null) return null;
            return ePais;
        }
        catch (Exception)
        {
            //this._log.Error(ex);
            throw;
        }
    }

    public List<EPU.Pais> GetLista()
    {
        try
        {
            return _DBcontext.Pais.Where(e => e.marca_eliminado == 0).ToList();
        }
        catch (Exception)
        {

            throw;
        }
    }

    public bool Guardar(EPU.Pais eEntidad)
    {
        using (var oTrans = this._DBcontext.Database.BeginTransaction())
        {
            try
            {
                this._DBcontext.Pais.Add(eEntidad);
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

    public bool Modificar(EPU.Pais eEntidad)
    {
        using (var oTrans = this._DBcontext.Database.BeginTransaction())
        {
            try
            {
                this._DBcontext.Pais.Update(eEntidad);
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
                EPU.Pais eEntidad = this.GetEntity(id);
                if (eEntidad == null) throw new Exception("País Inexistente!!...");

                eEntidad.marca_eliminado = 9;
                this._DBcontext.Pais.Update(eEntidad);
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
