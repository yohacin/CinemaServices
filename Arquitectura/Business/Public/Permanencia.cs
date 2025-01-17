using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using EPU = Entities.Public;

namespace Business.Public
{
    public class Permanencia : Base, IBusiness<EPU.Permanencia>
    {
        public Permanencia(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.Permanencia eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Permanencia.Add(eEntidad);
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

        public bool Modificar(EPU.Permanencia eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Permanencia.Update(eEntidad);
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
            throw new NotImplementedException();
        }

        public EPU.Permanencia GetEntity(long id)
        {
            try
            {
                return _DBcontext.Permanencia.FirstOrDefault(p => p.id == id);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Permanencia> GetLista()
        {
            try
            {
                return _DBcontext.Permanencia.ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Helpers.PermanenciaHelper> GetListaByDate(DateTime desde, DateTime hasta, long id_empresa)
        {
            try
            {
                string sQuey = $@"
                    select
                    pe. id,
                    (
                    trim(both ' ' from emp.nombre) || ' ' || trim(
                    both ' '
                    from
                    emp.apellido_paterno
                    ) || ' ' || trim(
                    both ' '
                    from
                    emp.apellido_materno
                    )
                    ) as nombre_empleado,
                    COALESCE(em.nombre, 'SIN DATOS') as empresa,
                    pe.hora_marcacion,
                    pe.tipo
                    from
                    public .permanencia pe
                    left join public .empresa em on em. id = pe.id_empresa
                    inner join public .empleado emp on emp. id = pe.id_empleado
                    where
                    pe.hora_marcacion :: DATE BETWEEN DATE('{desde.ToString("yyyy-MM-dd")}')
                    AND DATE('{hasta.ToString("yyyy-MM-dd")}')
                    {(id_empresa == 0? "" : $" AND pe.id_empresa = {id_empresa} ")}
                    order by
                    emp.nombre,
                    pe.hora_marcacion

                ";
                var lista = _DBcontext.PermanenciaHelper.FromSqlRaw(sQuey).ToList();
                if (lista != null)
                {
                    lista = lista.Select(e =>
                    {
                        e._hora_string = e.hora_marcacion.ToString("dd/MM/yyyy HH:mm");
                        return e;
                    }).ToList();
                }

                return lista;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Permanencia> ListarPermanenciaPorFecha(DateTime pFechaInicio, DateTime pFechaFin)
        {
            try
            {
                string query = $@"SELECT *
                                FROM public.permanencia
                                WHERE hora_marcacion BETWEEN '{pFechaInicio.ToString("yyyy-MM-dd HH:mm:ss.fff")}' AND
                                                             '{pFechaFin.ToString("yyyy-MM-dd HH:mm:ss.fff")}'
                                ORDER BY hora_marcacion DESC";

                List<EPU.Permanencia> lstPermanencia = this._DBcontext.Permanencia.FromSqlRaw(query).ToList();

                var IDs = lstPermanencia.Select(e => e.id_empleado).ToList();
                var listaEmpleado = this._DBcontext.Empleado.Where(e => IDs.Contains(e.id) && e.estado == 0).ToList();

                foreach (var item in lstPermanencia)
                {
                    item.empleado = listaEmpleado.FirstOrDefault(e => e.id == item.id_empleado);
                }

                return lstPermanencia.Where(e => e.empleado != null).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}