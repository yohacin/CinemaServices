using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EPU = Entities.Public;

namespace Business.Public
{
    public class Turno : Base, IBusiness<EPU.Turno>
    {
        public Turno(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.Turno eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Turno.Add(eEntidad);
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

        public bool Modificar(EPU.Turno eEntidad)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Turno.Update(eEntidad);
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

        public EPU.Turno GetEntity(long id)
        {
            try
            {
                return _DBcontext.Turno.FirstOrDefault(p => p.id == id);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Turno> GetLista()
        {
            throw new NotImplementedException();
        }

        public EPU.Turno GetEntityByEmpleado(int codigo_turno, long id_empleado)
        {
            try
            {
                return _DBcontext.Turno.FirstOrDefault(p => p.id_empleado == id_empleado && p.codigo_turno == codigo_turno);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// id = codigo empleado dual Assistance
        /// codigo = codigo empleado RRHH
        /// </summary>
        /// <param name="pCodigoEmpleado"> codigo empleado RRHH</param>
        public List<EPU.Turno> listarTurnosEmpleado(long id_empleado)
        {
            try
            {
                string query = $@"SELECT tu.*
                                 FROM public.empleado emp
                                     JOIN public.turno tu on tu.id_empleado=emp.id
                                 WHERE emp.id={id_empleado} ORDER BY tu.codigo_turno";

                return _DBcontext.Turno.FromSqlRaw(query).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public void deleteTurnosEmpleado(long pIdEmpleado)
        {
            try
            {
                List<EPU.Turno> lstTurnos = _DBcontext.Turno.Where(t => t.id_empleado == pIdEmpleado).ToList();

                _DBcontext.Turno.RemoveRange(lstTurnos);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// Obtiene el Nro de Horas de un Empleado Dependiendo del Dia de Trabajo
        /// </summary>
        /// <param name="idEmpleado"></param>
        /// id del empleado a Buscar
        /// <param name="iDiaDeSemana"></param>
        /// Dia de la semana para determinar las Horas de Trabajo de ese dia Espoecífico
        /// <returns></returns>
        public double GetHorasTrabajo(long idEmpleado, DayOfWeek iDiaDeSemana)
        {
            try
            {
                // Obtener Horas del turno en base al empleado y el dia para la fecha
                double horasHorario = _DBcontext.Turno
                    .Where(t => t.id_empleado == idEmpleado && t.idc_dia % 7 == (int)iDiaDeSemana)
                    .Sum(t => (t.salida - t.entrada).TotalHours);

                //return 8 - horas_asignadas;
                return horasHorario;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}