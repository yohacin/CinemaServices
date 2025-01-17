using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using EPU = Entities.Public;

namespace Business.Public;

public class Formato_Marcacion_Biometrico : Base, IBusiness<EPU.Formato_Marcacion_Biometrico>
{
    public Formato_Marcacion_Biometrico(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
    {

    }

    public EPU.Formato_Marcacion_Biometrico GetEntity(long id)
    {
        try
        {
            EPU.Formato_Marcacion_Biometrico eFormatoMarcacionExcel = _DBcontext.Formato_Marcacion_Biometrico.FirstOrDefault(p => p.marca_eliminado == 0 && p.id == id);
            if (eFormatoMarcacionExcel == null) return null;
            return eFormatoMarcacionExcel;
        }
        catch (Exception ex)
        {
            //this._log.Error(ex);
            throw ex;
        }
    }

    public List<EPU.Formato_Marcacion_Biometrico> GetLista()
    {
        try
        {
            return _DBcontext.Formato_Marcacion_Biometrico.Where(e => e.marca_eliminado == 0).ToList();
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public bool Guardar(EPU.Formato_Marcacion_Biometrico eEntidad)
    {
        using (var oTrans = this._DBcontext.Database.BeginTransaction())
        {
            try
            {
                this._DBcontext.Formato_Marcacion_Biometrico.Add(eEntidad);
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

    public bool Modificar(EPU.Formato_Marcacion_Biometrico eEntidad)
    {
        using (var oTrans = this._DBcontext.Database.BeginTransaction())
        {
            try
            {
                this._DBcontext.Formato_Marcacion_Biometrico.Update(eEntidad);
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
                EPU.Formato_Marcacion_Biometrico eEntidad = this.GetEntity(id);
                if (eEntidad == null) throw new Exception("Formato Marcacion Biométrico Inexistente!!...");

                eEntidad.marca_eliminado = 9;
                this._DBcontext.Formato_Marcacion_Biometrico.Update(eEntidad);
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

}
