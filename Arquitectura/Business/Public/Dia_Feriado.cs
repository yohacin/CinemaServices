using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EPU = Entities.Public;

namespace Business.Public
{
    public class Dia_Feriado : Base, IBusiness<EPU.Dia_Feriado>
    {
        public Dia_Feriado(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.Dia_Feriado eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Dia_Feriado.Add(eEntidad);
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

        public bool Modificar(EPU.Dia_Feriado eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Dia_Feriado.Update(eEntidad);
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
                    EPU.Dia_Feriado eEntidad = this.GetEntity(id);
                    if (eEntidad == null) throw new Exception("Dia Feriado Inexistente!!...");

                    eEntidad.estado = 9;
                    this._DBcontext.Dia_Feriado.Update(eEntidad);
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

        public EPU.Dia_Feriado GetEntity(long id)
        {
            try
            {
                return _DBcontext.Dia_Feriado.FirstOrDefault(p => p.estado == 0 && p.id == id);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Dia_Feriado GetEntityByFecha(DateTime fecha)
        {
            try
            {
                return _DBcontext.Dia_Feriado.FirstOrDefault(p => p.estado == 0 && p.fecha.Date == fecha.Date);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Dia_Feriado> GetLista()
        {
            try
            {
                return _DBcontext.Dia_Feriado.Where(u => u.estado == 0).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }



        public List<EPU.Dia_Feriado> GetListaModoIndependiente()
        {
            try
            {
                var listaFeriado = _DBcontext.Dia_Feriado.Where(u => u.estado == 0).ToList();
                if (listaFeriado != null && listaFeriado.Any())
                {
                    listaFeriado = listaFeriado.Select(f => {
                        f.ciudad = this._DBcontext.Ciudad.FirstOrDefault(c => c.id == f.id_ciudad);
                        return f;
                    }).ToList();
                }
                return listaFeriado;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}