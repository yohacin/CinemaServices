using Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using EPU = Entities.Public;

namespace Business.Public
{
    public class Empleado_Hoja_Trabajo : Base, IBusiness<EPU.Empleado_Hoja_Trabajo>
    {
        public Empleado_Hoja_Trabajo(IApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }

        public bool Guardar(EPU.Empleado_Hoja_Trabajo eEntidad)
        {
            throw new NotImplementedException();
        }

        public bool GuardarLista(List<EPU.Empleado_Hoja_Trabajo> listaEmpleado_Hoja_Trabajo)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    this._DBcontext.Empleado_Hoja_Trabajo.AddRange(listaEmpleado_Hoja_Trabajo);
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

        public string ValidarLista(List<Entities.Helpers.HojaTrabajoHelper> eHojaTrabajoHelperList, long id_empleado)
        {
            try
            {
                Dictionary<string, double> totalHorasTrabajoPorDia = new Dictionary<string, double> {
                    {"lunes", eHojaTrabajoHelperList.Sum(hth => hth.lunes)},
                    {"martes", eHojaTrabajoHelperList.Sum(hth => hth.martes)},
                    {"miercoles", eHojaTrabajoHelperList.Sum(hth => hth.miercoles)},
                    {"jueves", eHojaTrabajoHelperList.Sum(hth => hth.jueves)},
                    {"viernes", eHojaTrabajoHelperList.Sum(hth => hth.viernes)},
                    {"sabado", eHojaTrabajoHelperList.Sum(hth => hth.sabado)},
                    {"domingo", eHojaTrabajoHelperList.Sum(hth => hth.domingo)}
                };

                DateTime fecha = eHojaTrabajoHelperList[0].fecha;
                string mensajeValidacion = "";
                foreach (var dia in totalHorasTrabajoPorDia)
                {
                    string diaTrabajo = dia.Key;
                    double totalHorasTrabajo = dia.Value;
                    if (totalHorasTrabajo > 0)
                    {
                        foreach (var hojaTrabajo in eHojaTrabajoHelperList)
                        {
                            double horasAsignadas = (double)hojaTrabajo.GetType().GetProperty(diaTrabajo).GetValue(hojaTrabajo);
                            double horasDisponibles = this.GetCantidadHoras(hojaTrabajo.id_engagement, id_empleado, fecha);
                            if (horasDisponibles - horasAsignadas < 0)
                            {
                                mensajeValidacion += $"\n Usted no tiene horas asignadas suficientes para el engagement: '{hojaTrabajo.titulo_engagement}' en la fecha: {fecha.ToString("dd/MM/yyyy")}, las horas disponibles son: " + horasDisponibles;
                            }
                        }
                    }

                    fecha = fecha.AddDays(1);
                }

                return mensajeValidacion;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw;
            }
        }

        public string ModificarLista(Entities.Helpers.HojaTrabajoHelper eHojaTrabajoHelper, long id_empleado)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    string sMensaje = "";
                    DateTime fecha = eHojaTrabajoHelper.fecha;
                    DateTime fechaFin = eHojaTrabajoHelper.fecha.AddDays(6);
                    var listaEmpleadoHojaTrabajo = this._DBcontext.Empleado_Hoja_Trabajo.Where(e => e.fecha.Date >= fecha.Date && e.fecha.Date <= fechaFin.Date && e.id_empleado == id_empleado && e.id_engagement == eHojaTrabajoHelper.id_engagement && e.id_tarea == eHojaTrabajoHelper.id_tarea);
                    //-------Lunes
                    var itemLunes = listaEmpleadoHojaTrabajo.FirstOrDefault(e => e.fecha.Date == fecha.Date);
                    if (itemLunes == null)
                    {
                        if (eHojaTrabajoHelper.lunes > 0)
                        {
                            //Para las fechas anterior validar que solo se pueda guardar si es igual a 0
                            if (fecha.Date < DateTime.Now.Date)
                            {
                                double horasDisponibles = this.GetCantidadDisponible(eHojaTrabajoHelper.id_engagement, id_empleado, fecha);
                                if (horasDisponibles > 0)
                                {
                                    EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_Trabajo = new EPU.Empleado_Hoja_Trabajo();
                                    eEmpleado_Hoja_Trabajo.fecha = fecha;
                                    eEmpleado_Hoja_Trabajo.hora = eHojaTrabajoHelper.lunes;
                                    eEmpleado_Hoja_Trabajo.id_empleado = id_empleado;
                                    eEmpleado_Hoja_Trabajo.id_engagement = eHojaTrabajoHelper.id_engagement;
                                    eEmpleado_Hoja_Trabajo.id_tarea = eHojaTrabajoHelper.id_tarea;
                                    this._DBcontext.Empleado_Hoja_Trabajo.Add(eEmpleado_Hoja_Trabajo);
                                }
                                else
                                {
                                    var eEngagement = this._DBcontext.Engagement.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_engagement);
                                    if (eEngagement != null)
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")} para el engagement: '{eEngagement.titulo}', las horas disponibles son: " + horasDisponibles;
                                    else
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")}, las horas disponibles son: " + horasDisponibles;
                                }
                            }
                            if (DateTime.Now.Date == fecha.Date)
                            {
                                double horasDisponibles = this.GetCantidadDisponible(eHojaTrabajoHelper.id_engagement, id_empleado, fecha);
                                if (horasDisponibles > 0)
                                {
                                    EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_Trabajo = new EPU.Empleado_Hoja_Trabajo();
                                    eEmpleado_Hoja_Trabajo.fecha = fecha;
                                    eEmpleado_Hoja_Trabajo.hora = eHojaTrabajoHelper.lunes;
                                    eEmpleado_Hoja_Trabajo.id_empleado = id_empleado;
                                    eEmpleado_Hoja_Trabajo.id_engagement = eHojaTrabajoHelper.id_engagement;
                                    eEmpleado_Hoja_Trabajo.id_tarea = eHojaTrabajoHelper.id_tarea;
                                    this._DBcontext.Empleado_Hoja_Trabajo.Add(eEmpleado_Hoja_Trabajo);
                                }
                                else
                                {
                                    var eEngagement = this._DBcontext.Engagement.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_engagement);
                                    if (eEngagement != null)
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")} para el engagement: '{eEngagement.titulo}', las horas disponibles son: " + horasDisponibles;
                                    else
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")}, las horas disponibles son: " + horasDisponibles;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (DateTime.Now.Date == fecha.Date) // Solo puede modificar en la fecha del día
                        {
                            itemLunes.hora = eHojaTrabajoHelper.lunes;
                            this._DBcontext.Empleado_Hoja_Trabajo.Update(itemLunes);
                        }
                    }

                    //-----------Martes
                    fecha = fecha.AddDays(1);
                    var itemMartes = listaEmpleadoHojaTrabajo.FirstOrDefault(e => e.fecha.Date == fecha.Date);
                    if (itemMartes == null)
                    {
                        if (eHojaTrabajoHelper.martes > 0)
                        {
                            //Para las fechas anterior validar que solo se pueda guardar si es igual a 0
                            if (fecha.Date < DateTime.Now.Date)
                            {
                                double horasDisponibles = this.GetCantidadDisponible(eHojaTrabajoHelper.id_engagement, id_empleado, fecha);
                                if (horasDisponibles > 0)
                                {
                                    EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_Trabajo = new EPU.Empleado_Hoja_Trabajo();
                                    eEmpleado_Hoja_Trabajo.fecha = fecha;
                                    eEmpleado_Hoja_Trabajo.hora = eHojaTrabajoHelper.martes;
                                    eEmpleado_Hoja_Trabajo.id_empleado = id_empleado;
                                    eEmpleado_Hoja_Trabajo.id_engagement = eHojaTrabajoHelper.id_engagement;
                                    eEmpleado_Hoja_Trabajo.id_tarea = eHojaTrabajoHelper.id_tarea;
                                    this._DBcontext.Empleado_Hoja_Trabajo.Add(eEmpleado_Hoja_Trabajo);
                                }
                                else
                                {
                                    var eEngagement = this._DBcontext.Engagement.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_engagement);
                                    if (eEngagement != null)
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")} para el engagement: '{eEngagement.titulo}', las horas disponibles son: " + horasDisponibles;
                                    else
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")}, las horas disponibles son: " + horasDisponibles;
                                }
                            }
                            if (DateTime.Now.Date == fecha.Date)
                            {
                                double horasDisponibles = this.GetCantidadDisponible(eHojaTrabajoHelper.id_engagement, id_empleado, fecha);
                                if (horasDisponibles > 0)
                                {
                                    EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_Trabajo = new EPU.Empleado_Hoja_Trabajo();
                                    eEmpleado_Hoja_Trabajo.fecha = fecha;
                                    eEmpleado_Hoja_Trabajo.hora = eHojaTrabajoHelper.martes;
                                    eEmpleado_Hoja_Trabajo.id_empleado = id_empleado;
                                    eEmpleado_Hoja_Trabajo.id_engagement = eHojaTrabajoHelper.id_engagement;
                                    eEmpleado_Hoja_Trabajo.id_tarea = eHojaTrabajoHelper.id_tarea;
                                    this._DBcontext.Empleado_Hoja_Trabajo.Add(eEmpleado_Hoja_Trabajo);
                                }
                                else
                                {
                                    var eEngagement = this._DBcontext.Engagement.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_engagement);
                                    if (eEngagement != null)
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")} para el engagement: '{eEngagement.titulo}', las horas disponibles son: " + horasDisponibles;
                                    else
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")}, las horas disponibles son: " + horasDisponibles;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (DateTime.Now.Date == fecha.Date) // Solo puede modificar en la fecha del día
                        {
                            itemMartes.hora = eHojaTrabajoHelper.martes;
                            this._DBcontext.Empleado_Hoja_Trabajo.Update(itemMartes);
                        }
                    }

                    //----------Miercoles
                    fecha = fecha.AddDays(1);
                    var itemMiercoles = listaEmpleadoHojaTrabajo.FirstOrDefault(e => e.fecha.Date == fecha.Date);
                    if (itemMiercoles == null)
                    {
                        if (eHojaTrabajoHelper.miercoles > 0)
                        {
                            //Para las fechas anterior validar que solo se pueda guardar si es igual a 0
                            if (fecha.Date < DateTime.Now.Date)
                            {
                                double horasDisponibles = this.GetCantidadDisponible(eHojaTrabajoHelper.id_engagement, id_empleado, fecha);
                                if (horasDisponibles > 0)
                                {
                                    EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_Trabajo = new EPU.Empleado_Hoja_Trabajo();
                                    eEmpleado_Hoja_Trabajo.fecha = fecha;
                                    eEmpleado_Hoja_Trabajo.hora = eHojaTrabajoHelper.miercoles;
                                    eEmpleado_Hoja_Trabajo.id_empleado = id_empleado;
                                    eEmpleado_Hoja_Trabajo.id_engagement = eHojaTrabajoHelper.id_engagement;
                                    eEmpleado_Hoja_Trabajo.id_tarea = eHojaTrabajoHelper.id_tarea;
                                    this._DBcontext.Empleado_Hoja_Trabajo.Add(eEmpleado_Hoja_Trabajo);
                                }
                                else
                                {
                                    var eEngagement = this._DBcontext.Engagement.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_engagement);
                                    if (eEngagement != null)
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")} para el engagement: '{eEngagement.titulo}', las horas disponibles son: " + horasDisponibles;
                                    else
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")}, las horas disponibles son: " + horasDisponibles;
                                }
                            }
                            if (DateTime.Now.Date == fecha.Date)
                            {
                                double horasDisponibles = this.GetCantidadDisponible(eHojaTrabajoHelper.id_engagement, id_empleado, fecha);
                                if (horasDisponibles > 0)
                                {
                                    EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_Trabajo = new EPU.Empleado_Hoja_Trabajo();
                                    eEmpleado_Hoja_Trabajo.fecha = fecha;
                                    eEmpleado_Hoja_Trabajo.hora = eHojaTrabajoHelper.miercoles;
                                    eEmpleado_Hoja_Trabajo.id_empleado = id_empleado;
                                    eEmpleado_Hoja_Trabajo.id_engagement = eHojaTrabajoHelper.id_engagement;
                                    eEmpleado_Hoja_Trabajo.id_tarea = eHojaTrabajoHelper.id_tarea;
                                    this._DBcontext.Empleado_Hoja_Trabajo.Add(eEmpleado_Hoja_Trabajo);
                                }
                                else
                                {
                                    var eEngagement = this._DBcontext.Engagement.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_engagement);
                                    if (eEngagement != null)
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")} para el engagement: '{eEngagement.titulo}', las horas disponibles son: " + horasDisponibles;
                                    else
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")}, las horas disponibles son: " + horasDisponibles;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (DateTime.Now.Date == fecha.Date) // Solo puede modificar en la fecha del día
                        {
                            itemMiercoles.hora = eHojaTrabajoHelper.miercoles;
                            this._DBcontext.Empleado_Hoja_Trabajo.Update(itemMiercoles);
                        }
                    }

                    //------------------Jueves
                    fecha = fecha.AddDays(1);
                    var itemJueves = listaEmpleadoHojaTrabajo.FirstOrDefault(e => e.fecha.Date == fecha.Date);
                    if (itemJueves == null)
                    {
                        if (eHojaTrabajoHelper.jueves > 0)
                        {
                            //Para las fechas anterior validar que solo se pueda guardar si es igual a 0
                            if (fecha.Date < DateTime.Now.Date)
                            {
                                double horasDisponibles = this.GetCantidadDisponible(eHojaTrabajoHelper.id_engagement, id_empleado, fecha);
                                if (horasDisponibles > 0)
                                {
                                    EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_Trabajo = new EPU.Empleado_Hoja_Trabajo();
                                    eEmpleado_Hoja_Trabajo.fecha = fecha;
                                    eEmpleado_Hoja_Trabajo.hora = eHojaTrabajoHelper.jueves;
                                    eEmpleado_Hoja_Trabajo.id_empleado = id_empleado;
                                    eEmpleado_Hoja_Trabajo.id_engagement = eHojaTrabajoHelper.id_engagement;
                                    eEmpleado_Hoja_Trabajo.id_tarea = eHojaTrabajoHelper.id_tarea;
                                    this._DBcontext.Empleado_Hoja_Trabajo.Add(eEmpleado_Hoja_Trabajo);
                                }
                                else
                                {
                                    var eEngagement = this._DBcontext.Engagement.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_engagement);
                                    if (eEngagement != null)
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")} para el engagement: '{eEngagement.titulo}', las horas disponibles son: " + horasDisponibles;
                                    else
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")}, las horas disponibles son: " + horasDisponibles;
                                }
                            }
                            if (DateTime.Now.Date == fecha.Date)
                            {
                                double horasDisponibles = this.GetCantidadDisponible(eHojaTrabajoHelper.id_engagement, id_empleado, fecha);
                                if (horasDisponibles > 0)
                                {
                                    EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_Trabajo = new EPU.Empleado_Hoja_Trabajo();
                                    eEmpleado_Hoja_Trabajo.fecha = fecha;
                                    eEmpleado_Hoja_Trabajo.hora = eHojaTrabajoHelper.jueves;
                                    eEmpleado_Hoja_Trabajo.id_empleado = id_empleado;
                                    eEmpleado_Hoja_Trabajo.id_engagement = eHojaTrabajoHelper.id_engagement;
                                    eEmpleado_Hoja_Trabajo.id_tarea = eHojaTrabajoHelper.id_tarea;
                                    this._DBcontext.Empleado_Hoja_Trabajo.Add(eEmpleado_Hoja_Trabajo);
                                }
                                else
                                {
                                    var eEngagement = this._DBcontext.Engagement.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_engagement);
                                    if (eEngagement != null)
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")} para el engagement: '{eEngagement.titulo}', las horas disponibles son: " + horasDisponibles;
                                    else
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")}, las horas disponibles son: " + horasDisponibles;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (DateTime.Now.Date == fecha.Date) // Solo puede modificar en la fecha del día
                        {
                            itemJueves.hora = eHojaTrabajoHelper.jueves;
                            this._DBcontext.Empleado_Hoja_Trabajo.Update(itemJueves);
                        }
                    }

                    //-----------Viernes
                    fecha = fecha.AddDays(1);
                    var itemViernes = listaEmpleadoHojaTrabajo.FirstOrDefault(e => e.fecha.Date == fecha.Date);
                    if (itemViernes == null)
                    {
                        if (eHojaTrabajoHelper.viernes > 0)
                        {
                            //Para las fechas anterior validar que solo se pueda guardar si es igual a 0
                            if (fecha.Date < DateTime.Now.Date)
                            {
                                double horasDisponibles = this.GetCantidadDisponible(eHojaTrabajoHelper.id_engagement, id_empleado, fecha);
                                if (horasDisponibles > 0)
                                {
                                    EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_Trabajo = new EPU.Empleado_Hoja_Trabajo();
                                    eEmpleado_Hoja_Trabajo.fecha = fecha;
                                    eEmpleado_Hoja_Trabajo.hora = eHojaTrabajoHelper.viernes;
                                    eEmpleado_Hoja_Trabajo.id_empleado = id_empleado;
                                    eEmpleado_Hoja_Trabajo.id_engagement = eHojaTrabajoHelper.id_engagement;
                                    eEmpleado_Hoja_Trabajo.id_tarea = eHojaTrabajoHelper.id_tarea;
                                    this._DBcontext.Empleado_Hoja_Trabajo.Add(eEmpleado_Hoja_Trabajo);
                                }
                                else
                                {
                                    var eEngagement = this._DBcontext.Engagement.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_engagement);
                                    if (eEngagement != null)
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")} para el engagement: '{eEngagement.titulo}', las horas disponibles son: " + horasDisponibles;
                                    else
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")}, las horas disponibles son: " + horasDisponibles;
                                }
                            }
                            if (DateTime.Now.Date == fecha.Date)
                            {
                                double horasDisponibles = this.GetCantidadDisponible(eHojaTrabajoHelper.id_engagement, id_empleado, fecha);
                                if (horasDisponibles > 0)
                                {
                                    EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_Trabajo = new EPU.Empleado_Hoja_Trabajo();
                                    eEmpleado_Hoja_Trabajo.fecha = fecha;
                                    eEmpleado_Hoja_Trabajo.hora = eHojaTrabajoHelper.viernes;
                                    eEmpleado_Hoja_Trabajo.id_empleado = id_empleado;
                                    eEmpleado_Hoja_Trabajo.id_engagement = eHojaTrabajoHelper.id_engagement;
                                    eEmpleado_Hoja_Trabajo.id_tarea = eHojaTrabajoHelper.id_tarea;
                                    this._DBcontext.Empleado_Hoja_Trabajo.Add(eEmpleado_Hoja_Trabajo);
                                }
                                else
                                {
                                    var eEngagement = this._DBcontext.Engagement.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_engagement);
                                    if (eEngagement != null)
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")} para el engagement: '{eEngagement.titulo}', las horas disponibles son: " + horasDisponibles;
                                    else
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")}, las horas disponibles son: " + horasDisponibles;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (DateTime.Now.Date == fecha.Date) // Solo puede modificar en la fecha del día
                        {
                            itemViernes.hora = eHojaTrabajoHelper.viernes;
                            this._DBcontext.Empleado_Hoja_Trabajo.Update(itemViernes);
                        }
                    }

                    //----------------------------------Sabado
                    fecha = fecha.AddDays(1);
                    var itemSabado = listaEmpleadoHojaTrabajo.FirstOrDefault(e => e.fecha.Date == fecha.Date);
                    if (itemSabado == null)
                    {
                        if (eHojaTrabajoHelper.sabado > 0)
                        {
                            //Para las fechas anterior validar que solo se pueda guardar si es igual a 0
                            if (fecha.Date < DateTime.Now.Date)
                            {
                                double horasDisponibles = this.GetCantidadDisponible(eHojaTrabajoHelper.id_engagement, id_empleado, fecha);
                                if (horasDisponibles > 0)
                                {
                                    EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_Trabajo = new EPU.Empleado_Hoja_Trabajo();
                                    eEmpleado_Hoja_Trabajo.fecha = fecha;
                                    eEmpleado_Hoja_Trabajo.hora = eHojaTrabajoHelper.sabado;
                                    eEmpleado_Hoja_Trabajo.id_empleado = id_empleado;
                                    eEmpleado_Hoja_Trabajo.id_engagement = eHojaTrabajoHelper.id_engagement;
                                    eEmpleado_Hoja_Trabajo.id_tarea = eHojaTrabajoHelper.id_tarea;
                                    this._DBcontext.Empleado_Hoja_Trabajo.Add(eEmpleado_Hoja_Trabajo);
                                }
                                else
                                {
                                    var eEngagement = this._DBcontext.Engagement.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_engagement);
                                    if (eEngagement != null)
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")} para el engagement: '{eEngagement.titulo}', las horas disponibles son: " + horasDisponibles;
                                    else
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")}, las horas disponibles son: " + horasDisponibles;
                                }
                            }
                            if (DateTime.Now.Date == fecha.Date)
                            {
                                double horasDisponibles = this.GetCantidadDisponible(eHojaTrabajoHelper.id_engagement, id_empleado, fecha);
                                if (horasDisponibles > 0)
                                {
                                    EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_Trabajo = new EPU.Empleado_Hoja_Trabajo();
                                    eEmpleado_Hoja_Trabajo.fecha = fecha;
                                    eEmpleado_Hoja_Trabajo.hora = eHojaTrabajoHelper.sabado;
                                    eEmpleado_Hoja_Trabajo.id_empleado = id_empleado;
                                    eEmpleado_Hoja_Trabajo.id_engagement = eHojaTrabajoHelper.id_engagement;
                                    eEmpleado_Hoja_Trabajo.id_tarea = eHojaTrabajoHelper.id_tarea;
                                    this._DBcontext.Empleado_Hoja_Trabajo.Add(eEmpleado_Hoja_Trabajo);
                                }
                                else
                                {
                                    var eEngagement = this._DBcontext.Engagement.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_engagement);
                                    if (eEngagement != null)
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")} para el engagement: '{eEngagement.titulo}', las horas disponibles son: " + horasDisponibles;
                                    else
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")}, las horas disponibles son: " + horasDisponibles;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (DateTime.Now.Date == fecha.Date) // Solo puede modificar en la fecha del día
                        {
                            itemSabado.hora = eHojaTrabajoHelper.sabado;
                            this._DBcontext.Empleado_Hoja_Trabajo.Update(itemSabado);
                        }
                    }

                    //----------------------------------Domingo
                    fecha = fecha.AddDays(1);
                    var itemDomingo = listaEmpleadoHojaTrabajo.FirstOrDefault(e => e.fecha.Date == fecha.Date);
                    if (itemDomingo == null)
                    {
                        if (eHojaTrabajoHelper.sabado > 0)
                        {
                            //Para las fechas anterior validar que solo se pueda guardar si es igual a 0
                            if (fecha.Date < DateTime.Now.Date)
                            {
                                double horasDisponibles = this.GetCantidadDisponible(eHojaTrabajoHelper.id_engagement, id_empleado, fecha);
                                if (horasDisponibles > 0)
                                {
                                    EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_Trabajo = new EPU.Empleado_Hoja_Trabajo();
                                    eEmpleado_Hoja_Trabajo.fecha = fecha;
                                    eEmpleado_Hoja_Trabajo.hora = eHojaTrabajoHelper.domingo;
                                    eEmpleado_Hoja_Trabajo.id_empleado = id_empleado;
                                    eEmpleado_Hoja_Trabajo.id_engagement = eHojaTrabajoHelper.id_engagement;
                                    eEmpleado_Hoja_Trabajo.id_tarea = eHojaTrabajoHelper.id_tarea;
                                    this._DBcontext.Empleado_Hoja_Trabajo.Add(eEmpleado_Hoja_Trabajo);
                                }
                                else
                                {
                                    var eEngagement = this._DBcontext.Engagement.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_engagement);
                                    if (eEngagement != null)
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")} para el engagement: '{eEngagement.titulo}', las horas disponibles son: " + horasDisponibles;
                                    else
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")}, las horas disponibles son: " + horasDisponibles;
                                }
                            }
                            if (DateTime.Now.Date == fecha.Date)
                            {
                                double horasDisponibles = this.GetCantidadDisponible(eHojaTrabajoHelper.id_engagement, id_empleado, fecha);
                                if (horasDisponibles > 0)
                                {
                                    EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_Trabajo = new EPU.Empleado_Hoja_Trabajo();
                                    eEmpleado_Hoja_Trabajo.fecha = fecha;
                                    eEmpleado_Hoja_Trabajo.hora = eHojaTrabajoHelper.domingo;
                                    eEmpleado_Hoja_Trabajo.id_empleado = id_empleado;
                                    eEmpleado_Hoja_Trabajo.id_engagement = eHojaTrabajoHelper.id_engagement;
                                    eEmpleado_Hoja_Trabajo.id_tarea = eHojaTrabajoHelper.id_tarea;
                                    this._DBcontext.Empleado_Hoja_Trabajo.Add(eEmpleado_Hoja_Trabajo);
                                }
                                else
                                {
                                    var eEngagement = this._DBcontext.Engagement.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_engagement);
                                    if (eEngagement != null)
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")} para el engagement: '{eEngagement.titulo}', las horas disponibles son: " + horasDisponibles;
                                    else
                                        sMensaje += $"\nUsted no tiene horas asignadas en la fecha:{fecha.ToString("dd/MM/yyyy")}, las horas disponibles son: " + horasDisponibles;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (DateTime.Now.Date == fecha.Date) // Solo puede modificar en la fecha del día
                        {
                            itemDomingo.hora = eHojaTrabajoHelper.domingo;
                            this._DBcontext.Empleado_Hoja_Trabajo.Update(itemDomingo);
                        }
                    }

                    this._DBcontext.SaveChanges();

                    oTrans.Commit();
                    return sMensaje;
                }
                catch (Exception ex)
                {
                    oTrans.Rollback();
                    //this._log.Error(ex);
                    throw ex;
                }
            }
        }

        public double GetCantidadDisponible(long id_engagement, long id_empleado, DateTime fecha)
        {
            try
            {
                string sQuery = $@"
                    SELECT dtl.*
                    FROM PUBLIC.detalle_engagement_empleado dtl
                    JOIN PUBLIC.engagement_empleado ee on ee.id = dtl.id_engagement_empleado
                    WHERE ee.id_engagement = {id_engagement} and ee.id_empleado = {id_empleado} and dtl.estado = 0 and ee.estado = 0
                    and '{fecha.ToString("yyyy-MM-dd")}' >= (dtl.fecha_inicio::DATE) and '{fecha.ToString("yyyy-MM-dd")}' <= (dtl.fecha_fin::DATE)
                ";
                Entities.Public.Detalle_Engagement_Empleado eDetalle = this._DBcontext.Detalle_Engagement_Empleado.FromSqlRaw(sQuery).FirstOrDefault();

                if (eDetalle == null) return 0;
                //19

                string sQueryDisponibles = $@"
                    SELECT trab.*
                    FROM PUBLIC.empleado_hoja_trabajo trab
                    WHERE trab.id_engagement = {id_engagement} and trab.id_empleado = {id_empleado} and trab.estado = 0
                    and (trab.fecha::DATE)  >= '{eDetalle.fecha_inicio.ToString("yyyy-MM-dd")}' and (trab.fecha::DATE) <= '{eDetalle.fecha_fin.ToString("yyyy-MM-dd")}'
                ";

                List<Entities.Public.Empleado_Hoja_Trabajo> listaUsado = this._DBcontext.Empleado_Hoja_Trabajo.FromSqlRaw(sQueryDisponibles).ToList();
                double horasUsadas = listaUsado.Sum(dtl => dtl.hora);

                return eDetalle.cantidad_horas - horasUsadas;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public double GetCantidadHoras(long id_engagement, long id_empleado, DateTime fecha)
        {
            try
            {
                string sQuery = $@"
                    SELECT dtl.*
                    FROM PUBLIC.detalle_engagement_empleado dtl
                    JOIN PUBLIC.engagement_empleado ee on ee.id = dtl.id_engagement_empleado
                    WHERE ee.id_engagement = {id_engagement} and ee.id_empleado = {id_empleado} and dtl.estado = 0 and ee.estado = 0
                    and '{fecha.ToString("yyyy-MM-dd")}' >= (dtl.fecha_inicio::DATE) and '{fecha.ToString("yyyy-MM-dd")}' <= (dtl.fecha_fin::DATE)
                ";
                Entities.Public.Detalle_Engagement_Empleado eDetalle = this._DBcontext.Detalle_Engagement_Empleado.FromSqlRaw(sQuery).FirstOrDefault();

                if (eDetalle == null) return 0;
                //19

                return eDetalle.cantidad_horas;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public bool Modificar(EPU.Empleado_Hoja_Trabajo eEntidad)
        {
            throw new NotImplementedException();
        }

        public bool Eliminar(long id)
        {
            throw new NotImplementedException();
        }

        public bool EliminarBySemana(long id_engagement, long id_tarea, long id_empleado, DateTime fechaInicio)
        {
            using (var oTrans = this._DBcontext.Database.BeginTransaction())
            {
                try
                {
                    DateTime fechaFin = fechaInicio.AddDays(6);

                    string sQuery = $@"
                        UPDATE PUBLIC.empleado_hoja_trabajo set estado = 9
                        WHERE id_engagement = {id_engagement} and id_tarea = {id_tarea} and id_empleado = {id_empleado}
                        and fecha >= '{fechaInicio.ToString("yyyy-MM-dd")}' and fecha <= '{fechaFin.ToString("yyyy-MM-dd")}'
                    ";

                    this._DBcontext.Database.ExecuteSqlRaw(sQuery);
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

        public EPU.Empleado_Hoja_Trabajo GetEntity(long id)
        {
            try
            {
                return this._DBcontext.Empleado_Hoja_Trabajo.FirstOrDefault(e => e.id == id && e.estado == 0);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public EPU.Empleado_Hoja_Trabajo GetEntityByFilter(long id_engagement, long id_tarea, long id_empleado, DateTime fechaInicio)
        {
            try
            {
                DateTime fechaFin = fechaInicio.AddDays(6);
                return this._DBcontext.Empleado_Hoja_Trabajo.FirstOrDefault(e => e.fecha.Date >= fechaInicio.Date && e.fecha.Date <= fechaFin.Date && e.id_empleado == id_empleado && e.id_engagement == id_engagement && e.id_tarea == id_tarea && e.estado == 0);
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Empleado_Hoja_Trabajo> GetLista()
        {
            try
            {
                List<EPU.Empleado_Hoja_Trabajo> listaHojaTrabajo = this._DBcontext.Empleado_Hoja_Trabajo.Where(e => e.estado == 0).ToList();

                return listaHojaTrabajo;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<EPU.Empleado_Hoja_Trabajo> GetListaByEngagement(long id_engagement)
        {
            try
            {
                List<EPU.Empleado_Hoja_Trabajo> listaHojaTrabajo = this._DBcontext.Empleado_Hoja_Trabajo.Where(e => e.id_engagement == id_engagement && e.estado == 0).ToList();
                if (listaHojaTrabajo == null)
                {
                    listaHojaTrabajo = new List<EPU.Empleado_Hoja_Trabajo>();
                }
                return listaHojaTrabajo;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Helpers.HojaTrabajoHelper> GetListaBySemana(DateTime fechaInicio, long id_empleado)
        {
            try
            {
                List<Entities.Helpers.HojaTrabajoHelper> listaHojaTrabajoHelper = new List<Entities.Helpers.HojaTrabajoHelper>();

                DateTime fechaFin = fechaInicio.AddDays(6);
                List<EPU.Empleado_Hoja_Trabajo> listaHojasTrabajo = this._DBcontext.Empleado_Hoja_Trabajo.Where(e => e.fecha.Date >= fechaInicio.Date && e.fecha.Date <= fechaFin.Date && e.id_empleado == id_empleado && e.estado == 0).OrderBy(e => e.id).ToList();

                var groupedEmpleado = listaHojasTrabajo
                  .GroupBy(u => new { u.id_engagement, u.id_tarea })
                  .Select(grp => grp.ToList())
                  .ToList();

                int contador = 0;
                foreach (var item in groupedEmpleado)
                {
                    contador++;
                    var fecha = fechaInicio;
                    Entities.Helpers.HojaTrabajoHelper eHojaTrabajoHelper = new Entities.Helpers.HojaTrabajoHelper();
                    eHojaTrabajoHelper.id = contador;
                    eHojaTrabajoHelper.id_engagement = item[0].id_engagement;
                    eHojaTrabajoHelper.id_tarea = item[0].id_tarea;
                    eHojaTrabajoHelper.fecha = fecha;

                    EPU.Empleado_Hoja_Trabajo eEmpleado_Hoja_Trabajo = this._DBcontext.Empleado_Hoja_Trabajo.FirstOrDefault(e => e.fecha.Date == fecha.Date && e.id_engagement == item[0].id_engagement && e.id_tarea == item[0].id_tarea && e.id_empleado == id_empleado && e.estado == 0);
                    if (eEmpleado_Hoja_Trabajo != null) eHojaTrabajoHelper.lunes = eEmpleado_Hoja_Trabajo.hora;

                    fecha = fecha.AddDays(1);
                    eEmpleado_Hoja_Trabajo = item.FirstOrDefault(e => e.fecha.Date == fecha.Date && e.id_engagement == item[0].id_engagement && e.id_tarea == item[0].id_tarea && e.id_empleado == id_empleado && e.estado == 0);
                    if (eEmpleado_Hoja_Trabajo != null) eHojaTrabajoHelper.martes = eEmpleado_Hoja_Trabajo.hora;

                    fecha = fecha.AddDays(1);
                    eEmpleado_Hoja_Trabajo = item.FirstOrDefault(e => e.fecha.Date == fecha.Date && e.id_engagement == item[0].id_engagement && e.id_tarea == item[0].id_tarea && e.id_empleado == id_empleado && e.estado == 0);
                    if (eEmpleado_Hoja_Trabajo != null) eHojaTrabajoHelper.miercoles = eEmpleado_Hoja_Trabajo.hora;

                    fecha = fecha.AddDays(1);
                    eEmpleado_Hoja_Trabajo = item.FirstOrDefault(e => e.fecha.Date == fecha.Date && e.id_engagement == item[0].id_engagement && e.id_tarea == item[0].id_tarea && e.id_empleado == id_empleado && e.estado == 0);
                    if (eEmpleado_Hoja_Trabajo != null) eHojaTrabajoHelper.jueves = eEmpleado_Hoja_Trabajo.hora;

                    fecha = fecha.AddDays(1);
                    eEmpleado_Hoja_Trabajo = item.FirstOrDefault(e => e.fecha.Date == fecha.Date && e.id_engagement == item[0].id_engagement && e.id_tarea == item[0].id_tarea && e.id_empleado == id_empleado && e.estado == 0);
                    if (eEmpleado_Hoja_Trabajo != null) eHojaTrabajoHelper.viernes = eEmpleado_Hoja_Trabajo.hora;

                    var eTarea = this._DBcontext.Tarea.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_tarea && e.estado == 0);
                    eHojaTrabajoHelper.nombre_tarea = (eTarea == null) ? "NINGUNO" : eTarea.nombre;

                    var eEngagement = this._DBcontext.Engagement.FirstOrDefault(e => e.id == eHojaTrabajoHelper.id_engagement && e.estado == 0);
                    if (eEngagement != null)
                    {
                        eHojaTrabajoHelper.titulo_engagement = eEngagement.titulo;
                        var eEmpresa = this._DBcontext.Empresa.FirstOrDefault(e => e.id == eEngagement.id_empresa);
                        eHojaTrabajoHelper.id_empresa = eEmpresa.id;
                        eHojaTrabajoHelper.nombre_empresa = eEmpresa.nombre;
                    }
                    eHojaTrabajoHelper.total = eHojaTrabajoHelper.lunes + eHojaTrabajoHelper.martes + eHojaTrabajoHelper.miercoles + eHojaTrabajoHelper.jueves + eHojaTrabajoHelper.viernes;
                    listaHojaTrabajoHelper.Add(eHojaTrabajoHelper);
                }

                return listaHojaTrabajoHelper;
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Helpers.SeguimientoEngagementHelper> GetListaSeguimientoEngagement(DateTime desde, DateTime hasta)
        {
            try
            {
                string sQuery = $@"
                    SELECT
	                    htr.id id_empleado_hoja_trabajo,
	                    empr.nombre nombre_empresa,
	                    eng.titulo nombre_engagement,
	                    eng.desde fecha_inicio,
	                    eng.hasta fecha_fin,
	                    COALESCE((
		                    SELECT SUM(maximo_horas)
                            FROM PUBLIC.tarea_engagement
                            WHERE id_engagement = eng.id and id_tarea = tar.id and estado = 0
	                    ), 0) estimacion_horas_tarea,
	                    COALESCE((
		                    SELECT SUM(cantidad)
                            FROM PUBLIC.detalle_hora_engagement
                            WHERE id_engagement = eng.id and estado = 0 and tipo = 2
	                    ), 0) horas_overrun,
                    CASE
                          WHEN eng.facturable  THEN 'SI'
                          ELSE 'NO'
                    END facturable,
	                    cat.nombre categoria,
	                    tar.nombre tarea,
	                    CONCAT(emp.nombre, ' ', emp.apellido_paterno, ' ', emp.apellido_materno) empleado,
	                    htr.fecha fecha_tarea,
	                    htr.hora tiempo_ejecutado
                    FROM PUBLIC.empleado_hoja_trabajo htr
                    JOIN PUBLIC.empleado emp on emp.id = htr.id_empleado
                    JOIN PUBLIC.tarea tar on tar.id = htr.id_tarea
                    JOIN PUBLIC.categoria_tarea cat on cat.id = tar.id_categoria
                    JOIN PUBLIC.engagement eng on eng.id = htr.id_engagement
                    JOIN PUBLIC.empresa empr on empr.id = eng.id_empresa
                    WHERE htr.estado = 0 and tar.estado = 0 and cat.estado = 0 and eng.estado = 0
                        AND htr.fecha::DATE >= '{desde.ToString("yyyy-MM-dd")}' AND htr.fecha::DATE <= '{hasta.ToString("yyyy-MM-dd")}'
                ";
                return this._DBcontext.SeguimientoEngagementHelper.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }

        public List<Entities.Helpers.AsignacionEmpleadoHelper> GetListaAsignacionEmpleado()
        {
            try
            {
                string sQuery = $@"
                    SELECT
	                    ROW_NUMBER() OVER (ORDER BY empr.id) id,
	                    empr.id id_empresa,
	                    empr.nombre nombre_empresa,
	                    eng.id id_engagement,
	                    eng.titulo nombre_engagement,
	                    eng.desde fecha_inicio,
	                    eng.hasta fecha_fin,
                    CASE
                          WHEN eng.facturable  THEN 'SI'
                          ELSE 'NO'
                    END facturable,
	                    COALESCE((
		                    SELECT SUM(cantidad_horas) FROM PUBLIC.detalle_engagement_empleado WHERE id_engagement_empleado = eng_emp.id and estado = 0
	                    ), 0) horas_asignadas,
	                    COALESCE((
		                    SELECT SUM(cantidad) FROM PUBLIC.detalle_hora_engagement WHERE id_engagement = eng.id and estado = 0 and tipo = 1
	                    ), 0) estimacion_total_horas,
	                    COALESCE((
		                    SELECT SUM(cantidad) FROM PUBLIC.detalle_hora_engagement WHERE id_engagement = eng.id and estado = 0 and tipo = 2
	                    ), 0) estimacion_overrun,
	                    emp.id id_empleado,
	                    emp.nombre nombre_empelado,
                    COALESCE((
		                    SELECT SUM(hora) FROM PUBLIC.empleado_hoja_trabajo WHERE id_empleado = emp.id and id_engagement = eng.id and estado = 0
	                    ), 0) horas_ejecutadas
                    FROM PUBLIC.empresa empr
                    JOIN PUBLIC.engagement eng on eng.id_empresa = empr.id
                    JOIN PUBLIC.engagement_empleado eng_emp on eng_emp.id_engagement = eng.id
                    JOIN PUBLIC.empleado emp on emp.id = eng_emp.id_empleado
                    WHERE eng.estado = 0 and eng_emp.estado = 0 and emp.estado = 0
                ";
                return this._DBcontext.AsignacionEmpleadoHelper.FromSqlRaw(sQuery).ToList();
            }
            catch (Exception ex)
            {
                //this._log.Error(ex);
                throw ex;
            }
        }
    }
}