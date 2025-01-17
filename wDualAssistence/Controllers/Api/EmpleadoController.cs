using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EPU = Entities.Public;
using BPU = Business.Public;
using wDualAssistence.Helpers;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using wDualAssistence.Utility;
using wDualAssistence.Configs;

namespace wDualAssistence.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmpleadoController : ApiMainController
    {
        [HttpGet()]
        [BasicAuth]
        public string ListarEmpleados()
        {
            try
            {
                List<EPU.Empleado> listaEmpleados = new BPU.Empleado(this._appCnx).GetLista();

                return JsonConvert.SerializeObject(listaEmpleados);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw;
            }
        }

        [HttpPost()]
        public async Task<string> obtenerEmpleado([FromBody] string sjson)
        {
            EPU.RespuestaWS eRespuesta = new EPU.RespuestaWS();
            try
            {
                BPU.Empleado bEmpleado = new BPU.Empleado(this._appCnx);
                EPU.ViewLogin eLoginEmpleado = null;
                EPU.Empleado eEmpleado = null;

                //deserializar el empleado logueado
                eLoginEmpleado = JsonConvert.DeserializeObject<EPU.ViewLogin>(sjson);

                //encriptar la contraseña
                string password = Cryptography.EncriptarMD5(eLoginEmpleado.clave);

                //Empelado creado pero sin contraro en RRHH
                if (eLoginEmpleado.usuario == "*" && eLoginEmpleado.clave == "*") throw new Exception("Su usuario aún no tiene configurado las credenciales");

                //retornar empleado con esas credenciales
                eEmpleado = bEmpleado.LoginV2(eLoginEmpleado.usuario, password);

                if (eEmpleado != null)
                {
                    eRespuesta.id = eEmpleado.id;
                    eRespuesta.codigo_erp = eEmpleado.codigo;
                    eRespuesta.descripcion = "OK";
                    var settings = new JsonSerializerSettings
                    {
                        ContractResolver = ShouldSerializeContractResolver.Instance
                    };

                    eRespuesta.jsonCompleto = JsonConvert.SerializeObject(eEmpleado, settings);
                }
                else
                {
                    eRespuesta.id = 0L;
                    eRespuesta.codigo_erp = "0";
                    eRespuesta.descripcion = "Usuario / Contraseña no coinciden";
                }
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                eRespuesta.id = 0L;
                eRespuesta.codigo_erp = "0";
                eRespuesta.descripcion = ex.Message;
            }

            return JsonConvert.SerializeObject(eRespuesta);
        }

        //---------------------------Si se va volver a ocupar en otro lado mejor generalizar en Helpers
        public class ShouldSerializeContractResolver : DefaultContractResolver
        {
            public new static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);

                //------------------------------------------------------ocultar campos para empleado
                if (property.DeclaringType == typeof(EPU.Empleado) && property.PropertyName == "listaTurno")
                {
                    property.ShouldSerialize = instance => { return false; };
                }
                if (property.DeclaringType == typeof(EPU.Empleado) && property.PropertyName == "area")
                {
                    property.ShouldSerialize = instance => { return false; };
                }
                if (property.DeclaringType == typeof(EPU.Empleado) && property.PropertyName == "listaAsignacionHelper")
                {
                    property.ShouldSerialize = instance => { return false; };
                }
                if (property.DeclaringType == typeof(EPU.Empleado) && property.PropertyName == "horas_asignadas")
                {
                    property.ShouldSerialize = instance => { return false; };
                }
                //-----------------------------------------------------Fin Empleado
                return property;
            }
        }

        //--------------------------------------------------------------------------------
        [HttpPost()]
        [BasicAuth]
        public string guardarEmpleado([FromBody] string pJson)
        {
            EPU.RespuestaWS eRespuesta = new EPU.RespuestaWS();
            try
            {
                BPU.Empleado bEmpleado = new BPU.Empleado(this._appCnx);
                EPU.Empleado eEmpleado = null;

                //deserializar el empleado logueado
                eEmpleado = JsonConvert.DeserializeObject<EPU.Empleado>(pJson);

                EPU.Empleado eEmpleadoAux = new BPU.Empleado(this._appCnx).BuscarXCI(eEmpleado.ci);

                if (eEmpleadoAux == null) eEmpleado.id_perfil = Startup.id_perfil_defecto; // Se asigna el perfil por defecto

                bEmpleado.Guardar(eEmpleado);

                if (eEmpleado.id != 0)
                {
                    eRespuesta.id = eEmpleado.id;
                    eRespuesta.codigo_erp = eEmpleado.codigo;
                    eRespuesta.descripcion = "OK";
                    eRespuesta.jsonCompleto = JsonConvert.SerializeObject(eEmpleado);
                }
                else
                {
                    eRespuesta.id = 0L;
                    eRespuesta.codigo_erp = "0";
                    eRespuesta.descripcion = "ERROR";
                    eRespuesta.descripcion = "DualAssistance: No se logro guardar datos del empleado.";
                }
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                eRespuesta.id = 0L;
                eRespuesta.codigo_erp = "0";
                eRespuesta.descripcion = "ERROR: " + ex.Message;
            }

            return JsonConvert.SerializeObject(eRespuesta);
        }

        [HttpGet("{usuario}")]
        [BasicAuth]
        public string recuperarPass(string usuario)
        {
            try
            {
                BPU.Empleado bEmpleado = new BPU.Empleado(this._appCnx);
                EPU.RespuestaWS eRespuesta = null;
                EPU.Empleado eEmpleado = null;

                eEmpleado = bEmpleado.BuscarXUsuario(usuario);

                //envio de correo, queda pendiente
                //***********TO DO Rodrigo

                if (eEmpleado != null)
                {
                    eRespuesta.id = eEmpleado.id;
                    eRespuesta.codigo_erp = eEmpleado.id + "";
                    eRespuesta.descripcion = "SOLICITUD PROCESADA";
                    eRespuesta.jsonCompleto = JsonConvert.SerializeObject(eEmpleado);
                }
                else
                {
                    eRespuesta.id = 0L;
                    eRespuesta.codigo_erp = "0";
                    eRespuesta.descripcion = "NO SE ENCONTRO EMPLEADO";
                }

                return JsonConvert.SerializeObject(eRespuesta);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpPost()]
        [BasicAuth]
        public string login([FromBody] string pJson)
        {
            try
            {
                BPU.Empleado bEmpleado = new BPU.Empleado(this._appCnx);
                EPU.ViewLogin eUserLogueado = null;
                EPU.Empleado eEmpleado = null;
                bool result = false;

                //deserializar el empleado logueado
                eUserLogueado = JsonConvert.DeserializeObject<EPU.ViewLogin>(pJson);

                //encriptar la contraseña
                string password = Cryptography.EncriptarMD5(eUserLogueado.clave);

                eEmpleado = bEmpleado.Login(eUserLogueado.usuario, password);

                if (eEmpleado != null)
                    result = true;

                return JsonConvert.SerializeObject(result);
                //return Ok(new { transaccion = true, mensaje = "prueba" });
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                //return Ok(new { transaccion = false, mensaje = "pruebaError" });
                throw ex;
            }
        }

        [HttpPost()]
        [BasicAuth]
        public string actualizarFCM([FromBody] string pJson)
        {
            EPU.RespuestaWS respuesta = new EPU.RespuestaWS();
            try
            {
                BPU.Empleado bEmpleado = new BPU.Empleado(this._appCnx);
                EPU.Empleado eEmpleadoAux = null;
                EPU.Empleado eEmpleado = null;

                //deserializar el empleado
                eEmpleadoAux = (EPU.Empleado)JsonConvert.DeserializeObject(pJson);

                eEmpleado = bEmpleado.BuscarXCodigoEmpleado(eEmpleadoAux.codigo);

                if (eEmpleado != null)
                {
                    eEmpleado.fcm = eEmpleadoAux.fcm;
                    bEmpleado.Guardar(eEmpleado);
                }

                respuesta.id = eEmpleadoAux.id;
                respuesta.codigo_erp = eEmpleadoAux.id + "";
                respuesta.descripcion = "OK";
                respuesta.jsonCompleto = JsonConvert.SerializeObject(eEmpleadoAux.id);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                respuesta.id = 0L;
                respuesta.codigo_erp = "";
                respuesta.descripcion = ex.Message;
            }
            return JsonConvert.SerializeObject(respuesta);
        }

        [HttpPost()]
        [BasicAuth]
        public string guardarExcepcion([FromBody] string pJson)
        {
            try
            {
                EPU.RespuestaWS respuesta = new EPU.RespuestaWS();
                EPU.Excepcion eExcepcion = null;
                EPU.Excepcion eExcepcionAux = null;

                eExcepcion = JsonConvert.DeserializeObject<EPU.Excepcion>(pJson);

                EPU.Empleado eEmpleadoAux = new BPU.Empleado(this._appCnx).BuscarXCI(eExcepcion.ci);

                if (eEmpleadoAux == null)
                {
                    respuesta.id = 0L;
                    respuesta.codigo_erp = "0";
                    respuesta.descripcion = "No existe empleado con CI: " + eExcepcion.ci ?? "";

                    return JsonConvert.SerializeObject(respuesta);
                }
                eExcepcion.id_empleado = eEmpleadoAux.id;

                if (eExcepcion.estado == 9) //envia marca de eliminacion del dualrrhh en 9
                {
                    eExcepcionAux = new BPU.Excepcion(this._appCnx).buscarExcepcionXTransIni(eExcepcion.cod_origen, eExcepcion.tipo);
                    if (eExcepcionAux == null)
                    {
                        respuesta.id = 0L;
                        respuesta.codigo_erp = "0";
                        respuesta.descripcion = "No se logro guardar la excepcion";

                        return JsonConvert.SerializeObject(respuesta);
                    }
                    eExcepcion.id = eExcepcionAux.id;
                    new BPU.Excepcion(this._appCnx).EliminarExcepcion(eExcepcionAux);
                }
                else
                {
                    eExcepcionAux = new BPU.Excepcion(this._appCnx).buscarExcepcionXTransIni(eExcepcion.cod_origen, eExcepcion.tipo);
                    if (eExcepcionAux == null)
                    {
                        new BPU.Excepcion(this._appCnx).Guardar(eExcepcion);
                    }
                    else
                    {
                        eExcepcion.id = eExcepcionAux.id;
                        new BPU.Excepcion(this._appCnx).Modificar(eExcepcion);
                    }
                }

                if (eExcepcion.id != 0)
                {
                    respuesta.id = eExcepcion.id;
                    respuesta.codigo_erp = eExcepcion.ci ?? "";
                    respuesta.descripcion = "OK";
                    respuesta.jsonCompleto = JsonConvert.SerializeObject(eExcepcion);
                }
                else
                {
                    respuesta.id = 0L;
                    respuesta.codigo_erp = "0";
                    respuesta.descripcion = "No se logro guardar la excepcion";
                }

                return JsonConvert.SerializeObject(respuesta);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpGet("{ci}")]
        [BasicAuth]
        public string BuscarEmpleadoPorCI(string ci)
        {
            EPU.RespuestaWS eRespuesta = new EPU.RespuestaWS();
            try
            {
                var Empleado = new BPU.Empleado(this._appCnx).BuscarXCI(ci, swCargarFoto: true);

                eRespuesta.id = Empleado.id;
                eRespuesta.codigo_erp = Empleado.id.ToString();
                eRespuesta.descripcion = "OK";
                eRespuesta.jsonCompleto = JsonConvert.SerializeObject(Empleado);
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

        [HttpPost()]
        [BasicAuth]
        public async Task<string> GuardarFotosEmpleado()
        {
            ReconocimientoFacialConfig recoFaceConfig = Startup.reconocimientoFacialConfig;
            EPU.RespuestaWS eRespuesta = new EPU.RespuestaWS();
            try
            {
                long id_empleado = Convert.ToInt64(HttpContext.Request.Form["empleadoId"]);

                //limpiar directorio
                var path = $@"{Startup.appPath}\wwwroot\DirectorioFotos\fotos\{id_empleado}";
                if (System.IO.File.Exists(path))
                {
                    Directory.Delete(path, true);
                    _log.Info($"Eliminando directorio: {path}");
                }

                // Deserializar los datos de la marcación recibidos en la solicitud
                var listaFoto = new List<EPU.Foto_Empleado>();
                for (int item = 1; item <= 5; item++)
                {
                    IFormFile pFileImagen = HttpContext.Request.Form.Files[$"imagen{item}"];
                    if (pFileImagen == null) continue;

                    var Foto_Empleado = new EPU.Foto_Empleado();
                    Foto_Empleado.id_empleado = id_empleado;
                    Foto_Empleado.url_foto = $"{id_empleado}/{DateTime.Now.Ticks}.jpg";
                    Foto_Empleado.posicion = item;

                    var filename = $@"{Startup.appPath}/wwwroot/DirectorioFotos/fotos/{Foto_Empleado.url_foto}";
                    if (!System.IO.File.Exists(filename))
                    {
                        string directorio = Path.GetDirectoryName(filename);
                        if (!Directory.Exists(directorio))
                        {
                            _log.Info($"Creando directorio: {directorio}");
                            Directory.CreateDirectory(directorio);
                        }
                        using (FileStream fs = System.IO.File.Create(filename))
                        {
                            pFileImagen.CopyTo(fs);
                            fs.Flush();
                        }
                        _log.Info($"Imagen creada: {filename}");

                    }

                    listaFoto.Add(Foto_Empleado);
                }

                if (listaFoto.Count() == 0) throw new Exception("No se detectó ninguna foto.");

                new BPU.Empleado(this._appCnx).GuardarFotos(id_empleado, listaFoto);

                // Llamar a la API de reconocimiento facial
                if (!string.IsNullOrEmpty(recoFaceConfig.ApiEndPoint))
                {
                    FACE_RECOGNITION_API faceApi = new FACE_RECOGNITION_API(recoFaceConfig.ApiEndPoint);
                    await faceApi.ActualizarEntrenamiento();
                }

                eRespuesta.id = id_empleado;
                eRespuesta.codigo_erp = id_empleado.ToString();
                eRespuesta.descripcion = "OK";
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







        #region Metodos Modo Independiente

        [HttpPost()]
        [BasicAuth]
        public async Task<string> GuardarFotosEmpleadoV2()
        {
            ReconocimientoFacialConfig recoFaceConfig = Startup.reconocimientoFacialConfig;
            EPU.RespuestaWS eRespuesta = new EPU.RespuestaWS();
            try
            {
                BPU.Empleado bEmpleado = new BPU.Empleado(this._appCnx);
                int tipo_foto = Convert.ToInt32(HttpContext.Request.Form["tipo"]);
                long id_empleado = Convert.ToInt64(HttpContext.Request.Form["empleadoId"]);

                List<EPU.Foto_Empleado> listaFotosGuardadas = bEmpleado.ObtenerFotos(id_empleado, tipo_foto);

                // Eliminar fotos del empleado del mismo tipo
                var path = $@"{Startup.appPath}\wwwroot\DirectorioFotos\fotos\{id_empleado}";
                if (System.IO.File.Exists(path))
                {
                    foreach (var foto in listaFotosGuardadas)
                    {
                        string rutaFoto = Path.Combine(path, Path.GetFileName(foto.url_foto));
                        System.IO.File.Delete(rutaFoto);
                        _log.Info($"Foto eliminada: {rutaFoto}, Tipo: {(EPU.TipoFotoEmpleado)tipo_foto}");
                    }
                }

                // Deserializar los datos de la marcación recibidos en la solicitud
                var listaNuevasFoto = new List<EPU.Foto_Empleado>();
                for (int item = 1; item <= 5; item++)
                {
                    IFormFile pFileImagen = HttpContext.Request.Form.Files[$"imagen{item}"];
                    if (pFileImagen == null) continue;

                    var eFoto_Empleado = new EPU.Foto_Empleado();
                    eFoto_Empleado.id_empleado = id_empleado;
                    eFoto_Empleado.url_foto = $"{id_empleado}/{DateTime.Now.Ticks}.jpg";
                    eFoto_Empleado.posicion = item;
                    eFoto_Empleado.tipo_foto_empleado = (EPU.TipoFotoEmpleado)tipo_foto;

                    var filename = $@"{Startup.appPath}/wwwroot/DirectorioFotos/fotos/{eFoto_Empleado.url_foto}";
                    if (!System.IO.File.Exists(filename))
                    {
                        string directorio = Path.GetDirectoryName(filename);
                        if (!Directory.Exists(directorio))
                        {
                            _log.Info($"Creando directorio: {directorio}");
                            Directory.CreateDirectory(directorio);
                        }
                        using (FileStream fs = System.IO.File.Create(filename))
                        {
                            pFileImagen.CopyTo(fs);
                            fs.Flush();
                        }
                        _log.Info($"Imagen creada: {filename}");

                    }

                    listaNuevasFoto.Add(eFoto_Empleado);
                }

                if (listaNuevasFoto.Count() == 0) throw new Exception("No se detectó ninguna foto.");

                new BPU.Empleado(this._appCnx).GuardarFotosV2(id_empleado, listaNuevasFoto, tipo_foto);

                // Llamar a la API de reconocimiento facial
                if (!string.IsNullOrEmpty(recoFaceConfig.ApiEndPoint) && tipo_foto == (int)EPU.TipoFotoEmpleado.EntrenamientoMarcacion)
                {
                    FACE_RECOGNITION_API faceApi = new FACE_RECOGNITION_API(recoFaceConfig.ApiEndPoint);
                    await faceApi.ActualizarEntrenamiento();
                }

                eRespuesta.id = id_empleado;
                eRespuesta.codigo_erp = id_empleado.ToString();
                eRespuesta.descripcion = "OK";
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


        [HttpPost]
        [BasicAuth]
        public async Task<string> ActualizarEmpleado([FromForm] EPU.Empleado empleado, [FromForm] IFormFile foto)
        {
            EPU.RespuestaWS eRespuesta = new EPU.RespuestaWS();
            try
            {
                BPU.Empleado empleadoBussiness = new BPU.Empleado(this._appCnx);

                if (empleado == null || empleado.id <= 0)
                {
                    throw new Exception($"Datos no válidos");
                }

                EPU.Empleado eEmpleadoRepetido = empleadoBussiness.GetEntity(empleado.id);

                if (eEmpleadoRepetido == null)
                {
                    throw new Exception($"No se encontró al funcionario con CI: {empleado.ci}");
                }

                eEmpleadoRepetido.nombre = empleado.nombre;
                eEmpleadoRepetido.apellido_paterno = empleado.apellido_paterno;
                eEmpleadoRepetido.apellido_materno = empleado.apellido_materno;
                eEmpleadoRepetido.fecha_nacimiento = empleado.fecha_nacimiento;
                eEmpleadoRepetido.telefono = empleado.telefono;
                eEmpleadoRepetido.correo = empleado.correo;


                eRespuesta.id = empleado.id;
                eRespuesta.codigo_erp = empleado.id.ToString();
                eRespuesta.descripcion = "OK";

                if (foto != null)
                {

                    string[] extensionesPermitidas = { ".jpg", ".jpeg", ".png" };
                    string extension = Path.GetExtension(foto.FileName).ToLower();

                    if (!extensionesPermitidas.Contains(extension))
                    {
                        throw new Exception("El formato de archivo no es válido.");
                    }

                    if (foto.Length > 5 * 1024 * 1024) // 5 MB
                    {
                        throw new Exception("El archivo supera el tamaño permitido (5 MB).");
                    }

                    string rutaAbsolutaDirectorio = $@"{Startup.appPath}/wwwroot/DirectorioFotos/fotos/{eEmpleadoRepetido.id}";
                    if (!Directory.Exists(rutaAbsolutaDirectorio))
                    {
                        Directory.CreateDirectory(rutaAbsolutaDirectorio);
                    }
                    else
                    {
                        // Eliminando la foto existente
                        List<EPU.Foto_Empleado> listaFotosGuardadas = empleadoBussiness.ObtenerFotos(empleado.id, (int)EPU.TipoFotoEmpleado.Perfil);
                        if (System.IO.File.Exists(rutaAbsolutaDirectorio))
                        {
                            foreach (var fotoGuardada in listaFotosGuardadas)
                            {
                                string rutaFoto = Path.Combine(rutaAbsolutaDirectorio, Path.GetFileName(fotoGuardada.url_foto));
                                System.IO.File.Delete(rutaFoto);
                                _log.Info($"Foto eliminada: {rutaFoto}, Tipo: {(EPU.TipoFotoEmpleado.Perfil)}");
                            }
                        }

                    }
                    string nombreArchivo = $"{DateTime.Now.Ticks}.{extension}";
                    string rutaAbsolutaArchivo = Path.Combine(rutaAbsolutaDirectorio, nombreArchivo);
                    using (var stream = new FileStream(rutaAbsolutaArchivo, FileMode.Create))
                    {
                        await foto.CopyToAsync(stream);
                        stream.Flush();
                    }

                    EPU.Foto_Empleado fotoEmpleado = new EPU.Foto_Empleado()
                    {
                        id_empleado = empleado.id,
                        posicion = 0,
                        tipo_foto_empleado = EPU.TipoFotoEmpleado.Perfil,
                        url_foto = $"{eEmpleadoRepetido.id}/{nombreArchivo}"
                    };

                    empleadoBussiness.GuardarFotosV2(empleado.id, new List<EPU.Foto_Empleado>() { fotoEmpleado }, (int)EPU.TipoFotoEmpleado.Perfil);
                }
                empleadoBussiness.ValidarDatosEmpleado(eEmpleadoRepetido, false);
                empleadoBussiness.Modificar(eEmpleadoRepetido);

                eRespuesta.jsonCompleto = "true";
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

        #endregion
    }
}