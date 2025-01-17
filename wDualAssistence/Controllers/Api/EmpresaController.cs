using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EPU = Entities.Public;
using BPU = Business.Public;
using Newtonsoft.Json;
using System.IO;
using wDualAssistence.Helpers;

namespace wDualAssistence.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmpresaController : ApiMainController
    {
        [HttpGet("{id_empleado}")]
        [BasicAuth]
        public string listarEmpresasUsuario(long id_empleado)
        {
            try
            {
                BPU.Empresa bEmpresa = new BPU.Empresa(this._appCnx);
                List<EPU.Empresa> listaEmpresas = bEmpresa.ListarEmpresasUsuario(id_empleado);

                return JsonConvert.SerializeObject(listaEmpresas);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpGet()]
        [BasicAuth]
        public string listarCasasMatrices()
        {
            try
            {
                BPU.Empresa bEmpresa = new BPU.Empresa(this._appCnx);
                List<EPU.Empresa> lista = bEmpresa.ListarCasaMatrices();

                return JsonConvert.SerializeObject(lista);
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                throw ex;
            }
        }

        [HttpPost()]
        [BasicAuth]
        public string enviarEmpresa([FromBody] string pJson)
        {
            EPU.RespuestaWS eRespuesta = new EPU.RespuestaWS();
            try
            {
                BPU.Empresa bEmpresa = new BPU.Empresa(this._appCnx);
                EPU.Empresa eEmpresa = null;

                eEmpresa = JsonConvert.DeserializeObject<EPU.Empresa>(pJson);
                eEmpresa.marca_eliminado = 0;

                if (eEmpresa.id == 0)
                    bEmpresa.GuardarSinEmpleado(eEmpresa);
                else
                    bEmpresa.ModificarSinEmpleado(eEmpresa);

                if (eEmpresa.id != 0)
                {
                    eRespuesta.id = eEmpresa.id;
                    eRespuesta.codigo_erp = eEmpresa.id + "";
                    eRespuesta.descripcion = "OK";
                    eRespuesta.jsonCompleto = JsonConvert.SerializeObject(eEmpresa.id);
                }
                else
                {
                    eRespuesta.id = 0L;
                    eRespuesta.codigo_erp = "0";
                    eRespuesta.descripcion = "No se logro registrar la Empresa";
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

        [HttpPost()]
        [BasicAuth]
        public string enviarFotoEmpresa(/*IFormFile pJson*/)
        {
            long resultado = 0L;
            try
            {
                EPU.Foto_Empresa eFoto_EmpresaAux = Newtonsoft.Json.JsonConvert.DeserializeObject<EPU.Foto_Empresa>(HttpContext.Request.Form["fotoEmpresa"]);
                IFormFile pFileImagen = HttpContext.Request.Form.Files["imagen"];// .Form["imagen"];

                EPU.Foto_Empresa eFoto_Empresa = new EPU.Foto_Empresa();
                string sNameFile = DateTime.Now.Ticks + ".jpg";
                eFoto_Empresa.url_foto = "imagen/" + sNameFile;
                eFoto_Empresa.id_empresa = eFoto_EmpresaAux.id_empresa;
                eFoto_Empresa.posicion = eFoto_EmpresaAux.posicion;
                new BPU.Empresa(this._appCnx).GuardarFotoEmpresa(eFoto_Empresa);

                if (pFileImagen != null)
                {
                    var filename = sNameFile;
                    filename = $@"{Startup.appPath}\wwwroot\assets\imagen\{filename}";
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

                resultado = eFoto_Empresa.id;
            }
            catch (Exception ex)
            {
                this._log.Error(ex);
                resultado = 0L;
            }
            return JsonConvert.SerializeObject(resultado);
        }
    }
}