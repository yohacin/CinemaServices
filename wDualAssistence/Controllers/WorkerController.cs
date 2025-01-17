using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EPU = Entities.Public;
using ESE = Entities.Security;
using BPU = Business.Public;
using BSE = Business.Security;
using wDualAssistence.Utility.Controllers;

namespace wDualAssistence.Controllers
{
    public class WorkerController : MainController
    {
        public void VerificarSincronizacion()
        {
            new WorkerManager(this._appCnx, _log).VerificarSincronizacion();
            //bool result = false;
            //string textoError = "";
            //int maxIntentos = 3; //constante para cantidad maximas de intentos para sincronizar
            //int intento = 1;

            ////Cambiar el estado de los empleado que la fecha actual sea <= a la fecha de salida
            //new BPU.Empleado(this._appCnx).VerificarEstadoSalida();

            //while ((intento <= maxIntentos) && (!result))
            //{
            //    try
            //    {
            //        DateTime fechaHOY = DateTime.Now;

            //        if (fechaHOY.DayOfWeek != DayOfWeek.Sunday)  //no hay marcacion para dia domingo
            //        {
            //            BPU.Empleado bEmpleado = new BPU.Empleado(this._appCnx);
            //            BPU.Marcacion bMarcacion = new BPU.Marcacion(this._appCnx);
            //            List<EPU.Empleado> lstEmpleados = new List<EPU.Empleado>();

            //            lstEmpleados = bEmpleado.listarEmpleadosSinMarcacionRegistrada(fechaHOY.Date);

            //            foreach (EPU.Empleado eEmpleado in lstEmpleados)
            //            {
            //                if (eEmpleado.fecha_ingreso != null)
            //                {
            //                    //fecha ingreso mayor a la fecha actual, TODAVIA NO MARCA
            //                    if (eEmpleado.fecha_ingreso > fechaHOY) continue;
            //                }

            //                if (eEmpleado.fecha_salida != null)
            //                {
            //                    //fecha del servidor es mayor a la fecha de retiro del empleado, YA NO DEBE MARCA
            //                    if (fechaHOY > eEmpleado.fecha_salida.Value.AddDays(1)) continue;
            //                }

            //                if (eEmpleado.listaTurno.Count > 0)
            //                {
            //                    foreach (EPU.Turno eTurno in eEmpleado.listaTurno)
            //                    {
            //                        if (eTurno.idc_dia % 7 != (int)fechaHOY.DayOfWeek) continue;

            //                        EPU.Marcacion eMarcacionXDefecto = new EPU.Marcacion();

            //                        DateTime fechaEntradaProgramada = new DateTime(fechaHOY.Year, fechaHOY.Month, fechaHOY.Day,
            //                                                                        eTurno.entrada.Hour, eTurno.entrada.Minute, eTurno.entrada.Second, fechaHOY.Kind);

            //                        DateTime fechaSalidaProgramada = new DateTime(fechaHOY.Year, fechaHOY.Month, fechaHOY.Day,
            //                                                                       eTurno.salida.Hour, eTurno.salida.Minute, eTurno.salida.Second, fechaHOY.Kind);

            //                        eMarcacionXDefecto.id_empleado = eEmpleado.id;
            //                        eMarcacionXDefecto.codigo_turno = eTurno.codigo_turno;
            //                        eMarcacionXDefecto.entrada_programada = fechaEntradaProgramada; //entrada programada
            //                        eMarcacionXDefecto.salida_programada = fechaSalidaProgramada; //salida programada
            //                        eMarcacionXDefecto.latitud_entrada = 0D;
            //                        eMarcacionXDefecto.longitud_entrada = 0D;
            //                        eMarcacionXDefecto.latitud_salida = 0D;
            //                        eMarcacionXDefecto.longitud_salida = 0D;

            //                        //persistir lista de marcaciones
            //                        EPU.Marcacion eMarcacionAux = new BPU.Marcacion(this._appCnx).BuscarMarcacion(eMarcacionXDefecto);
            //                        if (eMarcacionAux == null)
            //                            bMarcacion.Guardar(eMarcacionXDefecto);
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            result = true;
            //        }

            //        this._log.Info("Termino la actualizacion de TURNOS EMPLEADOS");

            //        result = true;
            //    }
            //    catch (Exception ex)
            //    {
            //        this._log.Error("Error al ejecutar los TURNOS EMPLEADOS.");
            //        intento++;
            //        result = false;
            //        textoError = ex.Message;
            //    }
            //}

            //ESE.LogSync eLogsync = new ESE.LogSync();
            //eLogsync.fecha = DateTime.Now;
            //eLogsync.metodo = "TURNOS EMPLEADOS";
            //eLogsync.descripcion = "Sincronizacion de TURNOS EMPLEADOS " + ((result == true) ? "CORRECTA " : "FALLIDA ") + textoError;

            //this.registerLog(eLogsync);
            //if (!result)
            //{
            //    //PENDIENTE

            //    //NotificationHelper.sendNotification(
            //    //        "Sincronizacion de TURNOS EMPLEADOS",
            //    //        (result == true ? "CORRECTA " : "FALLIDA "), result,
            //    //        textoError);
            //}
        }

        //private void registerLog(ESE.LogSync eLog)
        //{
        //    try
        //    {
        //        BSE.LogSync bLogAsync = new BSE.LogSync(this._appCnx);
        //        bLogAsync.Guardar(eLog);
        //    }
        //    catch (Exception ex)
        //    {
        //        this._log.Error("Error al guardar el Log de Sincronizacion", ex);
        //    }
        //}
    }
}