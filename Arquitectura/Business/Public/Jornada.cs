using System;
using System.Collections.Generic;

//Librerias Externas
using System.Linq;
using Data;
using EPU = Entities.Public;
using BSE = Business.Security;
using ESE = Entities.Security;

namespace Business.Public;

public class Jornada : Base, IBusiness<EPU.Jornada>
{
    public Jornada(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
    {
    }


    public EPU.Jornada GetEntity(long id)
    {
        try
        {
            EPU.Jornada eJornada = _DBcontext.Jornada.FirstOrDefault(p => p.marca_eliminado == 0 && p.id == id);
            if (eJornada == null) return null;
            eJornada.listaDetalleJornada = _DBcontext.Detalle_Jornada.Where(x => x.id_jornada == eJornada.id).ToList();
            eJornada.listaTurnoPlantilla = _DBcontext.Turno_Plantilla.Where(x => x.id_jornada == eJornada.id).ToList();
            return eJornada;
        }
        catch (Exception ex)
        {
            //this._log.Error(ex);
            throw ex;
        }
    }

    public List<EPU.Jornada> GetLista()
    {
        try
        {
            var listaJornadas = _DBcontext.Jornada.Where(e => e.marca_eliminado == 0).ToList();

            if (listaJornadas != null && listaJornadas.Any())
            {
                listaJornadas = listaJornadas.Select(j =>
                {
                    j.listaDetalleJornada = this._DBcontext.Detalle_Jornada.Where(t => t.id_jornada == j.id).ToList();
                    return j;
                }).ToList();
            }

            return listaJornadas;
        }
        catch (Exception ex)
        {
            //this._log.Error(ex);
            throw ex;
        }
    }

    public bool Guardar(EPU.Jornada eEntidad)
    {
        using (var oTrans = this._DBcontext.Database.BeginTransaction())
        {
            try
            {
                this._DBcontext.Jornada.Add(eEntidad);
                this._DBcontext.SaveChanges();


                // lista detalle jornada
                eEntidad.listaDetalleJornada = eEntidad.listaDetalleJornada.Select(e =>
                {
                    e.id = 0;
                    e.id_jornada = eEntidad.id;
                    e.dia = e.hora_entrada.Day;
                    return e;
                }).OrderBy(e => e.hora_entrada).ToList();

                this._DBcontext.Detalle_Jornada.AddRange(eEntidad.listaDetalleJornada);
                this._DBcontext.SaveChanges();


                // lista turno plantilla
                eEntidad.listaTurnoPlantilla = eEntidad.listaTurnoPlantilla.Select(e =>
                {
                    e.id = 0;
                    e.id_jornada = eEntidad.id;
                    return e;
                }).OrderBy(e => e.hora_entrada).ToList();

                this._DBcontext.Turno_Plantilla.AddRange(eEntidad.listaTurnoPlantilla);
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

    public bool Modificar(EPU.Jornada eEntidad)
    {
        using (var oTrans = this._DBcontext.Database.BeginTransaction())
        {
            try
            {
                this._DBcontext.Jornada.Update(eEntidad);
                this._DBcontext.SaveChanges();

                // Removiendo la lista detalle Jornada actual
                List<EPU.Detalle_Jornada> listaDetalleJornada = _DBcontext.Detalle_Jornada.Where(t => t.id_jornada == eEntidad.id).ToList();
                _DBcontext.Detalle_Jornada.RemoveRange(listaDetalleJornada);
                _DBcontext.SaveChanges();


                // guardando nuevos detalles jornada
                eEntidad.listaDetalleJornada = eEntidad.listaDetalleJornada.Select(e =>
                {
                    e.id = 0;
                    e.id_jornada = eEntidad.id;
                    e.dia = e.hora_entrada.Day;
                    return e;
                }).OrderBy(e => e.hora_entrada).ToList();
                this._DBcontext.Detalle_Jornada.AddRange(eEntidad.listaDetalleJornada);
                this._DBcontext.SaveChanges();


                // Removiendo la lista detalle Jornada actual
                List<EPU.Turno_Plantilla> listaTurnoPlantilla = _DBcontext.Turno_Plantilla.Where(t => t.id_jornada == eEntidad.id).ToList();
                _DBcontext.Turno_Plantilla.RemoveRange(listaTurnoPlantilla);
                _DBcontext.SaveChanges();

                // guardando nuevos turnos plantilla
                eEntidad.listaTurnoPlantilla = eEntidad.listaTurnoPlantilla.Select(e =>
                {
                    e.id = 0;
                    e.id_jornada = eEntidad.id;
                    return e;
                }).OrderBy(e => e.hora_entrada).ToList();

                this._DBcontext.Turno_Plantilla.AddRange(eEntidad.listaTurnoPlantilla);
                this._DBcontext.SaveChanges();

                // Actualizar todos los turnos de los empleados que tengan asociado este id_jornada
                // Obtener empleados
                var empleados_jornada = this._DBcontext.Empleado.Where(e => e.estado == 0 && e.id_jornada == eEntidad.id).ToList();

                BSE.Clasificador bClasificador = new BSE.Clasificador(this._DBcontext);
                const long tipoDiaSemana = 4;
                List<ESE.Clasificador> dias = bClasificador.ListarPorTipo(tipoDiaSemana);

                // Eliminar sus turnos actuales
                foreach (var empleado in empleados_jornada)
                {
                    //eliminar turnos anteriores
                    this._DBcontext.Turno.RemoveRange(this._DBcontext.Turno.Where(e => e.id_empleado == empleado.id));
                    this._DBcontext.SaveChanges();

                    // Nueva lista turnos, para los nuevos turnos a crearse
                    var listaTurno = new List<EPU.Turno>();

                    foreach (EPU.Detalle_Jornada detalle in eEntidad.listaDetalleJornada)
                    {
                        EPU.Turno turno = new EPU.Turno()
                        {
                            id_empleado = empleado.id,
                            codigo_turno = (int)detalle.id,
                            entrada = detalle.hora_entrada,
                            salida = detalle.hora_salida,
                            descripcion_turno = $"Turno {detalle.id} - {dias.Find(d => Convert.ToInt32(d.valor) == detalle.dia).descripcion} ({detalle.hora_entrada.ToString("HH:mm")} - {detalle.hora_salida.ToString("HH:mm")})",
                            anticipo_entrada = (int)((detalle.hora_entrada - detalle.inicio_marcacion_entrada).TotalHours * 60),
                            tolerancia_entrada = (int)((detalle.fin_marcacion_entrada - detalle.hora_entrada).TotalHours * 60),
                            anticipo_salida = (int)((detalle.hora_salida - detalle.inicio_marcacion_salida).TotalHours * 60),
                            tolerancia_salida = (int)((detalle.fin_marcacion_salida - detalle.hora_salida).TotalHours * 60),
                            idc_dia = detalle.dia
                        };

                        this._DBcontext.Turno.Add(turno);
                        this._DBcontext.SaveChanges();
                    }
                }

                // Crear nuevos turnos por cada empleado

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
                EPU.Jornada eEntidad = this.GetEntity(id);
                if (eEntidad == null) throw new Exception("Jornada Inexistente!!...");

                var empleados_jornada = this._DBcontext.Empleado.Where(e => e.estado == 0 && e.id_jornada == eEntidad.id).ToList();
                if (empleados_jornada.Any()) throw new Exception("No es posible eliminar la jornada porque está asociada a funcionarios.");

                eEntidad.marca_eliminado = 9;
                this._DBcontext.Jornada.Update(eEntidad);
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

