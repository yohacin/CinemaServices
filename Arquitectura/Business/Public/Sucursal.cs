using Data;
using System.Collections.Generic;
using System.Linq;
using System;
using EPU = Entities.Public;

namespace Business.Public;

public class Sucursal : Base, IBusiness<EPU.Sucursal>
{
    public Sucursal(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
    {
    }

    public EPU.Sucursal GetEntity(long id)
    {
        try
        {
            EPU.Sucursal eSucursal = _DBcontext.Sucursal.FirstOrDefault(p => p.marca_eliminado == 0 && p.id == id);
            if (eSucursal == null) return null;
            return eSucursal;
        }
        catch (Exception)
        {
            //this._log.Error(ex);
            throw;
        }
    }

    public List<EPU.Sucursal> GetLista()
    {
        try
        {
            var listaSucursales = _DBcontext.Sucursal.Where(e => e.marca_eliminado == 0).ToList();

            if (listaSucursales != null && listaSucursales.Any())
            {
                listaSucursales = listaSucursales.Select(s => {
                    s.ciudad = this._DBcontext.Ciudad.FirstOrDefault(c => c.id == s.id_ciudad);
                    return s;
                }).ToList();
            }

            return listaSucursales;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public bool Guardar(EPU.Sucursal eEntidad)
    {
        using (var oTrans = this._DBcontext.Database.BeginTransaction())
        {
            try
            {
                this._DBcontext.Sucursal.Add(eEntidad);
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

    public bool Modificar(EPU.Sucursal eEntidad)
    {
        using (var oTrans = this._DBcontext.Database.BeginTransaction())
        {
            try
            {
                this._DBcontext.Sucursal.Update(eEntidad);
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
                EPU.Sucursal eEntidad = this.GetEntity(id);
                if (eEntidad == null) throw new Exception("Sucursal Inexistente!!...");

                var empleados = this._DBcontext.Empleado.Where(e => e.estado == 0 && e.id_sucursal == eEntidad.id).ToList();
                if (empleados.Any()) throw new Exception("No es posible eliminar la sucursal porque está asociada a empleados.");

                eEntidad.marca_eliminado = 9;
                this._DBcontext.Sucursal.Update(eEntidad);
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
