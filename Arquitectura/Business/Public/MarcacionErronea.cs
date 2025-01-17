using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using EPU = Entities.Public;

namespace Business.Public
{
    public class MarcacionErronea : Base, IBusiness<EPU.MarcacionErronea>
    {
        public MarcacionErronea(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.MarcacionErronea eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.MarcacionErronea.Add(eEntidad);
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

        public bool Modificar(EPU.MarcacionErronea eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.MarcacionErronea.Update(eEntidad);
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

        public EPU.MarcacionErronea GetEntity(long id)
        {
            throw new NotImplementedException();
        }

        public List<EPU.MarcacionErronea> GetLista()
        {
            throw new NotImplementedException();
        }

        public EPU.MarcacionErronea Buscar(EPU.MarcacionErronea pMarcacion)
        {
            try
            {
                string query = $@"SELECT * FROM public.marcacion_erronea
                                  WHERE id_empleado={pMarcacion.id_empleado} and codigo_turno={pMarcacion.codigo_turno}
                                    and hora_marcacion::DATE = '{pMarcacion.hora_marcacion.ToString("yyyy-MM-dd")}'";

                return _DBcontext.MarcacionErronea.FromSqlRaw(query).FirstOrDefault();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }


        public List<Entities.Helpers.MarcacionErroneaHelper> GetListaByDate(DateTime desde, DateTime hasta)
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
                    COALESCE(em.descripcion_turno, 'SIN DATOS') as turno,
                    pe.hora_marcacion,
                    pe.tipo
                    from
                    public .marcacion_erronea pe
                    left join public .turno em on em.codigo_turno = pe.codigo_turno
                    inner join public .empleado emp on emp. id = pe.id_empleado
                    where pe.hora_marcacion :: DATE BETWEEN DATE('{desde.ToString("yyyy-MM-dd")}') AND DATE('{hasta.ToString("yyyy-MM-dd")}')
                    order by
                    emp.nombre,
                    pe.hora_marcacion

                ";
                var lista = _DBcontext.MarcacionErroneaHelper.FromSqlRaw(sQuey).ToList();
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


    }
}