using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EPU = Entities.Public;
using BPU = Business.Public;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Converters;
using wDualAssistence.Helpers;
using wDualAssistence.Utility.Notificador;
using System.Threading.Tasks;
using wDualAssistence.Utility;
using wDualAssistence.Configs;
using Entities.Constants;

namespace wDualAssistence.Controllers.Api;

[Route("api/[controller]/[action]")]
[ApiController]
public class MarcacionController : ApiMainController
{
    [HttpGet()]
    [BasicAuth]
    public string obtenerHora()
    {
        try
        {
            string resultado = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            return resultado;
        }
        catch (Exception ex)
        {
            this._log.Error(ex);
            throw ex;
        }
    }

    [HttpPost()]
    [BasicAuth]
    public string enviarMarcacion([FromBody] string json)
    {
        BPU.Marcacion bMarcacion = new BPU.Marcacion(this._appCnx);
        BPU.ConfigParametro bConfigParametro = new BPU.ConfigParametro(this._appCnx);
        EPU.RespuestaWS eRespuesta = new EPU.RespuestaWS();
        EPU.Marcacion eMarcacion = null;
        try
        {
            eMarcacion = JsonConvert.DeserializeObject<EPU.Marcacion>(json);
            bool no_validar_marcacion_entrada = bConfigParametro.ValidarParametroByNombre(Config.Name.NO_VALIDAR_MARCACION_DE_ENTRADA, "SI", false, eMarcacion.id_empleado);
            eMarcacion.fecha_marcacion = DateTime.Now;
            EPU.Marcacion eMarcacionProgramada = new BPU.Marcacion(this._appCnx).BuscarMarcacion(eMarcacion.id_empleado, eMarcacion.codigo_turno, DateTime.Now/*eMarcacion.fecha_marcacion*/);
            if (eMarcacionProgramada != null)
            {
                bool isPrimerEntrada = false;
                bool isSalidaFinal = false;
                DateTime entrada_real = DateTime.Now;
                if (eMarcacionProgramada.entrada_real == null)
                {
                    if (eMarcacion.entrada_real == null)
                    {
                        if (!no_validar_marcacion_entrada)
                        {
                            throw new Exception("No se tiene registrada la entrada !");
                        }
                    }
                    else
                    {
                        entrada_real = eMarcacion.entrada_real.Value; // entrada desde el movil
                    }
                }
                else
                {
                    entrada_real = eMarcacionProgramada.entrada_real.Value; // entrada manual desde la web
                }

                List<EPU.Marcacion> lstMarcacionesProgramadas = new BPU.Marcacion(this._appCnx).listarMarcacionesEmpleado(eMarcacion.id_empleado, entrada_real);
                int marcacionesSalidaEsperadas = lstMarcacionesProgramadas.Count();
                int cantidadEntradas = 0;
                int cantidadSalidas = 0;
                //throw new Exception("TEST");
                foreach (EPU.Marcacion iter in lstMarcacionesProgramadas)
                {
                    if (iter.entrada_real != null)
                        cantidadEntradas++;

                    if (iter.salida_real != null)
                        cantidadSalidas++;
                }

                if ((cantidadEntradas == 0))
                    isPrimerEntrada = true;

                if (((eMarcacion.salida_movil != null)
                            && ((marcacionesSalidaEsperadas - cantidadSalidas)
                            == 1)))
                {
                    isSalidaFinal = true;
                }

                double HORAS_MINIMA_TRABAJO_DIARIO = 0;
                if (Startup.validar_tareas) HORAS_MINIMA_TRABAJO_DIARIO = (new BPU.Turno(this._appCnx)).GetHorasTrabajo(eMarcacion.id_empleado, eMarcacion.fecha_marcacion.DayOfWeek);

                if (isPrimerEntrada)
                {
                    /// -------------------------- VERIFICAR Sabado y domingo
                    DateTime fecha_anterior = DateTime.Now.AddDays(-1);

                    /// -------------------------- Validando dias feriados y excepciones
                    EPU.Empleado eEmpleado = (new BPU.Empleado(this._appCnx)).GetEntity(eMarcacion.id_empleado);
                    EPU.Dia_Feriado eDia_Feriado = new BPU.Dia_Feriado(this._appCnx).GetEntityByFecha(fecha_anterior);
                    EPU.Excepcion eExcepcion = new BPU.Excepcion(this._appCnx).BuscarByFechaEmpleado(eEmpleado.id, fecha_anterior);

                    eEmpleado = new BPU.Empleado(this._appCnx).GetEntity(eMarcacion.id_empleado);
                    eDia_Feriado = new BPU.Dia_Feriado(this._appCnx).GetEntityByFecha(fecha_anterior);

                    if (eEmpleado == null) throw new Exception("No existe el empleado con id: " + eMarcacion.id_empleado);

                    eExcepcion = new BPU.Excepcion(this._appCnx).BuscarByFechaEmpleado(eEmpleado.id, fecha_anterior);

                    while (eDia_Feriado != null ||
                            fecha_anterior.DayOfWeek == DayOfWeek.Sunday ||
                            fecha_anterior.DayOfWeek == DayOfWeek.Saturday ||
                            eExcepcion != null)
                    {
                        fecha_anterior = fecha_anterior.AddDays(-1);
                        eDia_Feriado = new BPU.Dia_Feriado(this._appCnx).GetEntityByFecha(fecha_anterior);
                        eExcepcion = new BPU.Excepcion(this._appCnx).BuscarByFechaEmpleado(eEmpleado.id, fecha_anterior);
                    }

                    //  verificando las horas de trabajo del dia anterior
                    List<EPU.Empleado_Hoja_Trabajo> lstHojasTrabajo = new BPU.Marcacion(this._appCnx).listaHojaTrabajo(eMarcacion.id_empleado, fecha_anterior);
                    if (Startup.validar_tareas) HORAS_MINIMA_TRABAJO_DIARIO = (new BPU.Turno(this._appCnx)).GetHorasTrabajo(eMarcacion.id_empleado, fecha_anterior.DayOfWeek);

                    Double horasTrabajadasDiaAnterior = 0;
                    if (lstHojasTrabajo != null)
                    {
                        horasTrabajadasDiaAnterior = lstHojasTrabajo.Sum(e => e.hora);
                    }

                    if ((horasTrabajadasDiaAnterior < HORAS_MINIMA_TRABAJO_DIARIO))
                    {
                        throw new Exception($"No es posible marcar la entrada. Las horas trabajadas en fecha {fecha_anterior.ToString("yyyy-MM-dd")} es menor a {HORAS_MINIMA_TRABAJO_DIARIO}. Favor de registrar sus horas de trabajo en la fecha especificada.");
                    }
                }

                if (isSalidaFinal)
                {
                    if (eMarcacion.salida_movil == null) throw new Exception("El campo Salida movil es nulo !");
                    DateTime salida_movil = eMarcacion.salida_movil.Value;
                    //  verificando si subio sus datos de marcacion
                    List<EPU.Empleado_Hoja_Trabajo> lstHojasTrabajo = new BPU.Marcacion(this._appCnx).listaHojaTrabajo(eMarcacion.id_empleado, salida_movil);
                    Double horasTrabajadas = 0;
                    if (lstHojasTrabajo != null)
                    {
                        horasTrabajadas = lstHojasTrabajo.Sum(e => e.hora);
                    }

                    if (horasTrabajadas != HORAS_MINIMA_TRABAJO_DIARIO)
                        throw new Exception($"No es posible marcar la salida. Las horas de trabajo registradas en fecha {salida_movil.ToString("yyyy-MM-dd")} deben ser igual a {HORAS_MINIMA_TRABAJO_DIARIO}");
                }
                eMarcacion.id = eMarcacionProgramada.id;
                eMarcacion.entrada_programada = eMarcacionProgramada.entrada_programada;
                eMarcacion.salida_programada = eMarcacionProgramada.salida_programada;
                //eMarcacion.entrada_real = eMarcacionProgramada.entrada_real;
                //eMarcacion.salida_real = eMarcacionProgramada.salida_real;

                if (eMarcacion.entrada_real != null)
                    eMarcacion.tipo_marcacion_entrada = "MOVIL";

                if (eMarcacion.salida_real != null)
                {
                    eMarcacion.tipo_marcacion_salida = "MOVIL";
                    eMarcacion.entrada_real = (eMarcacionProgramada.entrada_real == null) ? eMarcacionProgramada.entrada_movil : eMarcacionProgramada.entrada_real;
                    eMarcacion.tipo_marcacion_entrada = eMarcacionProgramada.tipo_marcacion_entrada;
                    eMarcacion.id_empresa_entrada = eMarcacionProgramada.id_empresa_entrada;
                }

                new BPU.Marcacion(this._appCnx).Modificar(eMarcacion);
            }

            if (eMarcacion.id != 0)
            {
                eRespuesta.id = eMarcacion.id;
                eRespuesta.codigo_erp = eMarcacion.id + "";
                eRespuesta.descripcion = "OK";
                eRespuesta.jsonCompleto = JsonConvert.SerializeObject(eMarcacion.id);
            }
            else
            {
                eRespuesta.id = 0L;
                eRespuesta.codigo_erp = "0";
                eRespuesta.descripcion = "No se encontro marcación programada";
            }
        }
        catch (Exception ex)
        {
            eRespuesta.id = 0L;
            eRespuesta.descripcion = ex.Message;
            this._log.Error("Error al enviarMarcacion con el JSON " + json, ex);
        }
        return JsonConvert.SerializeObject(eRespuesta);
    }

    [HttpPost()]
    [BasicAuth]
    public string verificarEnviarMarcacion([FromBody] string json)
    {
        BPU.Marcacion bMarcacion = new BPU.Marcacion(this._appCnx);
        BPU.ConfigParametro bConfigParametro = new BPU.ConfigParametro(this._appCnx);
        EPU.RespuestaWS eRespuesta = new EPU.RespuestaWS();
        EPU.Marcacion eMarcacion = null;
        try
        {
            var serializeSettings = new JsonSerializerSettings();
            serializeSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff" });

            eMarcacion = JsonConvert.DeserializeObject<EPU.Marcacion>(json);
            eMarcacion.id = 0;
            //bMarcacion.VerificarMarcacion(eMarcacion);
            bool no_validar_marcacion_entrada = bConfigParametro.ValidarParametroByNombre(Config.Name.NO_VALIDAR_MARCACION_DE_ENTRADA, "SI", false, eMarcacion.id_empleado);
            double HORAS_MINIMA_TRABAJO_DIARIO = (Startup.validar_tareas) ? (new BPU.Turno(this._appCnx)).GetHorasTrabajo(eMarcacion.id_empleado, eMarcacion.fecha_marcacion.DayOfWeek) : 0; //Poner como configuracion global
            eMarcacion.fecha_marcacion = DateTime.Now;
            EPU.Marcacion eMarcacionProgramada = new BPU.Marcacion(this._appCnx).BuscarMarcacion(eMarcacion.id_empleado, eMarcacion.codigo_turno, DateTime.Now/*eMarcacion.fecha_marcacion*/);
            if (eMarcacionProgramada != null)
            {
                // si ya existe una entrada y el campo salida es nulo se asume que quiere volver a marcar entrada
                if (eMarcacionProgramada.entrada_real != null && eMarcacion.salida_real == null)
                    throw new Exception("Ya tiene registrada una marcación de entrada en fecha: " + eMarcacionProgramada.entrada_real.Value.ToString("dd/MM/yyyy HH:mm")); // verificar con multiples turnos

                // Si ya existe entrada y salida  entonces no debe marcar ni salida ni entrada
                if (eMarcacionProgramada.entrada_real != null && eMarcacionProgramada.salida_real != null)
                    throw new Exception("Ya tiene registrada una marcación de entrada en fecha: " + eMarcacionProgramada.entrada_real.Value.ToString("dd/MM/yyyy HH:mm") + ". Y salida en: " + eMarcacionProgramada.salida_real.Value.ToString("dd/MM/yyyy HH:mm")); // verificar con multiples turnos

                bool isPrimerEntrada = false;
                bool isSalidaFinal = false;
                DateTime entrada_real = DateTime.Now;
                if (eMarcacionProgramada.entrada_real == null)
                {
                    if (eMarcacion.entrada_real == null)
                    {
                        if (!no_validar_marcacion_entrada)
                        {
                            throw new Exception("No se tiene registrada la entrada !");
                        }
                    }
                    else
                    {
                        entrada_real = eMarcacion.entrada_real.Value; // entrada desde el movil
                    }
                }
                else
                {
                    entrada_real = eMarcacionProgramada.entrada_real.Value; // entrada manual desde la web
                }

                List<EPU.Marcacion> lstMarcacionesProgramadas = new BPU.Marcacion(this._appCnx).listarMarcacionesEmpleado(eMarcacion.id_empleado, entrada_real);
                int marcacionesSalidaEsperadas = lstMarcacionesProgramadas.Count();
                int cantidadEntradas = 0;
                int cantidadSalidas = 0;
                foreach (EPU.Marcacion iter in lstMarcacionesProgramadas)
                {
                    if (iter.entrada_real != null)
                    {
                        cantidadEntradas++;
                    }

                    if (iter.salida_real != null)
                        cantidadSalidas++;
                }

                if ((cantidadEntradas == 0))
                    isPrimerEntrada = true;

                if (((eMarcacion.salida_movil != null)
                            && ((marcacionesSalidaEsperadas - cantidadSalidas)
                            == 1)))
                {
                    isSalidaFinal = true;
                }

                if (isPrimerEntrada)
                {
                    /// -------------------------- VERIFICAR Sabado y domingo
                    DateTime fecha_anterior = DateTime.Now.AddDays(-1);

                    /// -------------------------- Validando dias feriados y excepciones
                    EPU.Empleado eEmpleado = new BPU.Empleado(this._appCnx).GetEntity(eMarcacion.id_empleado);
                    if (eEmpleado == null) throw new Exception("No existe el empleado con id: " + eMarcacion.id_empleado);

                    EPU.Dia_Feriado eDia_Feriado = new BPU.Dia_Feriado(this._appCnx).GetEntityByFecha(fecha_anterior);
                    EPU.Excepcion eExcepcion = new BPU.Excepcion(this._appCnx).BuscarByFechaEmpleado(eEmpleado.id, fecha_anterior);
                    while (eDia_Feriado != null ||
                            fecha_anterior.DayOfWeek == DayOfWeek.Sunday ||
                            fecha_anterior.DayOfWeek == DayOfWeek.Saturday ||
                            eExcepcion != null)
                    {
                        fecha_anterior = fecha_anterior.AddDays(-1);
                        eDia_Feriado = new BPU.Dia_Feriado(this._appCnx).GetEntityByFecha(fecha_anterior);
                        eExcepcion = new BPU.Excepcion(this._appCnx).BuscarByFechaEmpleado(eEmpleado.id, fecha_anterior);
                    }

                    //  verificando las horas de trabajo del dia anterior
                    List<EPU.Empleado_Hoja_Trabajo> lstHojasTrabajo = new BPU.Marcacion(this._appCnx).listaHojaTrabajo(eMarcacion.id_empleado, fecha_anterior);
                    if (Startup.validar_tareas) HORAS_MINIMA_TRABAJO_DIARIO = (new BPU.Turno(this._appCnx)).GetHorasTrabajo(eMarcacion.id_empleado, fecha_anterior.DayOfWeek);
                    Double horasTrabajadasDiaAnterior = 0;
                    if (lstHojasTrabajo != null)
                    {
                        horasTrabajadasDiaAnterior = lstHojasTrabajo.Sum(e => e.hora);
                    }

                    if ((horasTrabajadasDiaAnterior < HORAS_MINIMA_TRABAJO_DIARIO))
                    {
                        throw new Exception($"No es posible marcar la entrada. Las horas trabajadas en fecha {fecha_anterior.ToString("yyyy-MM-dd")} es menor a {HORAS_MINIMA_TRABAJO_DIARIO}. Favor de registrar sus horas de trabajo en la fecha especificada.");
                    }
                }
                if (isSalidaFinal)
                {
                    if (eMarcacionProgramada.salida_real != null) throw new Exception("Ya tiene registrada una marcación de salida en fecha: " + eMarcacionProgramada.salida_real.Value.ToString("dd/MM/yyyy"));

                    if (eMarcacion.salida_movil == null) throw new Exception("El campo Salida movil es nulo !");
                    DateTime salida_movil = eMarcacion.salida_movil.Value;
                    //  verificando si subio sus datos de marcacion
                    List<EPU.Empleado_Hoja_Trabajo> lstHojasTrabajo = new BPU.Marcacion(this._appCnx).listaHojaTrabajo(eMarcacion.id_empleado, salida_movil);
                    Double horasTrabajadas = 0;
                    if (lstHojasTrabajo != null)
                    {
                        horasTrabajadas = lstHojasTrabajo.Sum(e => e.hora);
                    }

                    if (horasTrabajadas != HORAS_MINIMA_TRABAJO_DIARIO)
                        throw new Exception($"No es posible marcar la salida. Las horas de trabajo registradas en fecha {salida_movil.ToString("yyyy-MM-dd")} deben ser igual a {HORAS_MINIMA_TRABAJO_DIARIO}");
                }

                eMarcacion.id = eMarcacionProgramada.id;
                eMarcacion.entrada_programada = eMarcacionProgramada.entrada_programada;
                eMarcacion.salida_programada = eMarcacionProgramada.salida_programada;

                if (eMarcacion.entrada_real != null)
                    eMarcacion.tipo_marcacion_entrada = "MOVIL";

                if (eMarcacion.salida_real != null)
                    eMarcacion.tipo_marcacion_salida = "MOVIL";

                //if (eMarcacion.entrada_real != null)
                //    eMarcacion.tipo_marcacion_entrada = "MOVIL";

                //if (eMarcacion.salida_real != null)
                //{
                //    eMarcacion.tipo_marcacion_salida = "MOVIL";
                //    eMarcacion.entrada_real = (eMarcacionProgramada.entrada_real == null) ? eMarcacionProgramada.entrada_movil : eMarcacionProgramada.entrada_real;
                //    eMarcacion.tipo_marcacion_entrada = eMarcacionProgramada.tipo_marcacion_entrada;
                //    eMarcacion.id_empresa_entrada = eMarcacionProgramada.id_empresa_entrada;
                //}

                //new BPU.Marcacion().Modificar(eMarcacion);
            }

            if (eMarcacion.id != 0)
            {
                eRespuesta.id = eMarcacion.id;
                eRespuesta.codigo_erp = eMarcacion.id + "";
                eRespuesta.descripcion = "OK";
                eRespuesta.jsonCompleto = JsonConvert.SerializeObject(eMarcacion.id);
            }
            else
            {
                eRespuesta.id = 0L;
                eRespuesta.codigo_erp = "0";
                eRespuesta.descripcion = "No se encontro marcación programada";
            }
        }
        catch (Exception ex)
        {
            eRespuesta.id = 0L;
            eRespuesta.descripcion = ex.Message;
            this._log.Error("Error al enviarMarcacion con el JSON " + json, ex);
        }
        return JsonConvert.SerializeObject(eRespuesta);
    }

    [HttpPost()]
    [BasicAuth]
    public string enviarMarcacionErronea([FromBody] string pJson)
    {
        EPU.RespuestaWS eRespuesta = new EPU.RespuestaWS();
        try
        {
            BPU.MarcacionErronea bMarcacion = new BPU.MarcacionErronea(this._appCnx);

            EPU.MarcacionErronea eMarcacionErronea = null;
            EPU.MarcacionErronea eMarcacionErroneaAux = null;

            eMarcacionErronea = JsonConvert.DeserializeObject<EPU.MarcacionErronea>(pJson);

            eMarcacionErroneaAux = bMarcacion.Buscar(eMarcacionErronea);
            if (eMarcacionErroneaAux == null)
            {
                eMarcacionErronea.id = 0;
                new BPU.MarcacionErronea(this._appCnx).Guardar(eMarcacionErronea);
            }
            else
            {
                eMarcacionErronea.id = eMarcacionErroneaAux.id;
                new BPU.MarcacionErronea(this._appCnx).Modificar(eMarcacionErronea);
            }

            //Envio de notificacion de la marcacion erronea
            mandarAlertaDeMarcacionErronea();

            eRespuesta.id = eMarcacionErronea.id;
            eRespuesta.codigo_erp = eMarcacionErronea.id + "";
            eRespuesta.descripcion = "OK";
            eRespuesta.jsonCompleto = JsonConvert.SerializeObject(eMarcacionErronea.id);
        }
        catch (Exception ex)
        {
            this._log.Error(ex);
            eRespuesta.id = 0;
            eRespuesta.codigo_erp = "0";
            eRespuesta.descripcion = ex.Message;
        }
        return JsonConvert.SerializeObject(eRespuesta);
    }

    private async void mandarAlertaDeMarcacionErronea()
    {
        try
        {
            Business.Public.AlertaMarcacionErronea bConfigAlertaMarcacionErronea = new Business.Public.AlertaMarcacionErronea(this._appCnx);
            Entities.Public.AlertaMarcacionErronea configAlertaMarcacionErronea = bConfigAlertaMarcacionErronea.Get();
            if (configAlertaMarcacionErronea != null && configAlertaMarcacionErronea.activo)
            {
                // Validar id de campana

                if (configAlertaMarcacionErronea.id_campana <= 0)
                {
                    throw new Exception("No se configuro el id campaña para alertas de marcacion erronea.");
                }

                // Validar url del notificador
                if (configAlertaMarcacionErronea.url_notificador != null && configAlertaMarcacionErronea.url_notificador.Count() <= 0)
                {
                    throw new Exception("No se configuro la url del notificador para alertas de marcacion erronea.");
                }

                // Instanciar httpclient
                NotificadorClient notificadorClient = new NotificadorClient();
                notificadorClient.configurar(configAlertaMarcacionErronea.url_notificador);
                await notificadorClient.ejecutarCampana(configAlertaMarcacionErronea.id_campana);
            }
        }
        catch (Exception ex)
        {
            this._log.Error(ex);
        }
    }

    private int ObtenerTipoMultimediaPorExtension(string extension)
    {
        switch (extension.ToLower())
        {
            case "png":
            case "jpg":
            case "jpeg":
                return 1;

            case "mp4":
                return 2;

            case "mp3":
                return 3;

            case "pdf":
                return 4;

            case "txt":
                return 5;

            case "xls":
            case "xlsx":
                return 6;

            case "doc":
            case "docx":
                return 7;

            case "xml":
                return 8;

            case "webp":
                return 100;
        }
        return 0;
    }

    [HttpPost()]
    [BasicAuth]
    public string enviarPermanencia([FromBody] string pJson)
    {
        EPU.RespuestaWS eRespuesta = new EPU.RespuestaWS();
        try
        {
            BPU.Permanencia bPermanencia = new BPU.Permanencia(this._appCnx);
            EPU.Permanencia ePermanencia = null;

            ePermanencia = JsonConvert.DeserializeObject<EPU.Permanencia>(pJson);
            ePermanencia.id = 0;
            bPermanencia.Guardar(ePermanencia);

            //Envio de correo ****pendiente TO DO RODRIGO

            eRespuesta.id = ePermanencia.id;
            eRespuesta.codigo_erp = ePermanencia.id + "";
            eRespuesta.descripcion = "OK";
            eRespuesta.jsonCompleto = JsonConvert.SerializeObject(ePermanencia.id);
        }
        catch (Exception ex)
        {
            this._log.Error(ex);
            eRespuesta.id = 0;
            eRespuesta.codigo_erp = "o";
            eRespuesta.descripcion = ex.Message;
        }
        return JsonConvert.SerializeObject(eRespuesta);
    }

    [HttpPost()]
    [BasicAuth]
    public string enviarFotoMarcacion(/*IFormFile pJson*/)
    {
        long resultado = 0L;
        try
        {
            EPU.Foto_Marcacion eFoto_MarcacionAux = Newtonsoft.Json.JsonConvert.DeserializeObject<EPU.Foto_Marcacion>(HttpContext.Request.Form["fotoMarcacion"]);
            IFormFile pFileImagen = HttpContext.Request.Form.Files["imagen"];// .Form["imagen"];

            EPU.Foto_Marcacion eFoto_Marcacion = new EPU.Foto_Marcacion();
            string sNameFile = DateTime.Now.Ticks + ".jpg";
            eFoto_Marcacion.url_foto = "../../marcacion/" + sNameFile;
            eFoto_Marcacion.fecha_hora = DateTime.Now;
            eFoto_Marcacion.id_marcacion = eFoto_MarcacionAux.id_marcacion;
            eFoto_Marcacion.posicion = eFoto_MarcacionAux.posicion;
            new BPU.Foto_Marcacion(this._appCnx).Guardar(eFoto_Marcacion);

            if (pFileImagen != null)
            {
                var filename = sNameFile;
                filename = $@"{Startup.appPath}\wwwroot\assets\marcacion\{filename}";
                if (!System.IO.File.Exists(filename))
                {
                    string directorio = Path.GetDirectoryName(filename);
                    if (!Directory.Exists(directorio))
                    {
                        Directory.CreateDirectory(directorio);
                    }
                    using (FileStream fs = System.IO.File.Create(filename))
                    {
                        pFileImagen.CopyTo(fs);
                        fs.Flush();
                    }
                }
            }

            resultado = eFoto_Marcacion.id;
        }
        catch (Exception ex)
        {
            this._log.Error(ex);
            resultado = 0L;
        }
        return JsonConvert.SerializeObject(resultado);
    }

    [HttpGet("{id_empleado}/{fecha}")]
    [BasicAuth]
    public string listarMarcacionesEmpleado(long id_empleado, string fecha)
    {
        try
        {
            BPU.ViewMarcacion bMarcacion = new BPU.ViewMarcacion(this._appCnx);
            List<Entities.Helpers.MarcacionHelpers> lista = bMarcacion.listarMarcaciones(id_empleado, Convert.ToDateTime(fecha), Convert.ToDateTime(fecha));

            return JsonConvert.SerializeObject(lista);
        }
        catch (Exception ex)
        {
            this._log.Error(ex);
            throw ex;
        }
    }

    [HttpGet("{fecha_inicio}/{fecha_fin}")]
    [BasicAuth]
    public List<EPU.Marcacion> listarMarcacionPorFecha(string fecha_inicio, string fecha_fin)
    {
        List<EPU.Marcacion> lista = new List<EPU.Marcacion>();
        try
        {
            BPU.Marcacion bMarcacion = new BPU.Marcacion(this._appCnx);
            lista = bMarcacion.listarMarcacionPorFecha(0, Convert.ToDateTime(fecha_inicio), Convert.ToDateTime(fecha_fin));
        }
        catch (Exception ex)
        {
            this._log.Error(ex);
        }

        return lista;
    }

    [HttpGet("{id_sucursal}/{fecha_inicio}/{fecha_fin}")]
    [BasicAuth]
    public List<EPU.Marcacion> listarMarcacionRealizadaPorFecha(long id_sucursal, string fecha_inicio, string fecha_fin)
    {
        List<EPU.Marcacion> lista = new List<EPU.Marcacion>();
        try
        {
            BPU.Marcacion bMarcacion = new BPU.Marcacion(this._appCnx);
            lista = bMarcacion.listarMarcacionPorFecha(id_sucursal, Convert.ToDateTime(fecha_inicio), Convert.ToDateTime(fecha_fin), swSoloConRegistro: true);
        }
        catch (Exception ex)
        {
            this._log.Error(ex);
        }

        return lista;
    }

    [HttpPost()]
    [BasicAuth]
    public async Task<string> ValidarFotoMarcacion()
    {
        ReconocimientoFacialConfig recoFaceConfig = Startup.reconocimientoFacialConfig;
        EPU.RespuestaWS eRespuesta = new EPU.RespuestaWS();

        try
        {
            // Recibir el empleadoId desde el formulario
            string empleadoId = HttpContext.Request.Form["empleadoId"];

            // Validar si se recibió el empleadoId
            if (string.IsNullOrEmpty(empleadoId))
            {
                throw new Exception("No se proporcionó el empleadoId.");
            }

            // Deserializar los datos de la marcación recibidos en la solicitud
            IFormFile pFileImagen = HttpContext.Request.Form.Files["imagen"];

            if (pFileImagen == null)
            {
                throw new Exception("No se proporcionó ninguna imagen.");
            }

            // Convertir la imagen a formato Base64
            string imagenBase64;
            using (var ms = new MemoryStream())
            {
                await pFileImagen.CopyToAsync(ms);
                byte[] imageBytes = ms.ToArray();
                imagenBase64 = Convert.ToBase64String(imageBytes);
            }

            // Llamar a la API de reconocimiento facial
            FACE_RECOGNITION_API faceApi = new FACE_RECOGNITION_API(recoFaceConfig.ApiEndPoint);
            ResultadoVerificacionRostro resultadoApi = await faceApi.VerificarRostro(empleadoId, imagenBase64);

            // Verificar si existe un mensaje de Error
            if (!string.IsNullOrEmpty(resultadoApi.Error))
            {
                // TO-DO guardar foto como mala
                throw new Exception(resultadoApi.Error);
            }

            eRespuesta.id = long.Parse(empleadoId);
            eRespuesta.codigo_erp = empleadoId;
            eRespuesta.descripcion = "OK";

            // Verificar si la "Certeza" cumple con el mínimo
            if (resultadoApi.EmpleadoId == "Desconocido" || resultadoApi.Certeza < recoFaceConfig.PorcentajeCertezaMin)
            {
                // TO-DO guardar foto como mala
                // loguear?
                eRespuesta.jsonCompleto = "false";
            }
            else
            {
                // TO-DO guardar foto como buena
                // loguear ?
                eRespuesta.jsonCompleto = "true";
            }
        }
        catch (Exception ex)
        {
            this._log.Error(ex);
            eRespuesta.id = 0;
            eRespuesta.codigo_erp = "0";
            eRespuesta.descripcion = ex.Message;
        }

        return JsonConvert.SerializeObject(eRespuesta);
    }
}