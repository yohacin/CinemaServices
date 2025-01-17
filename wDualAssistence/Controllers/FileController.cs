using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace wDualAssistence.Controllers
{
    public class FileController : MainController
    {
        private IHostingEnvironment hostingEnv;
        public FileController(IHostingEnvironment env)
        {
            this.hostingEnv = env;
        }
        [HttpPost]
        public IActionResult GuardarFotoEmpresa(IList<IFormFile> UploadFiles)
        {
            try
            {
                foreach (var file in UploadFiles)
                {
                    if (UploadFiles != null)
                    {
                        var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        filename = $@"{hostingEnv.WebRootPath}\assets\imagen\{filename}";
                        if (!System.IO.File.Exists(filename))
                        {
                            string directorio = Path.GetDirectoryName(filename);
                            if (!Directory.Exists(directorio))
                            {
                                Directory.CreateDirectory(directorio);
                            }
                            using (FileStream fs = System.IO.File.Create(filename))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                        }
                        else
                        {
                            Response.Clear();
                            Response.StatusCode = 204;
                            Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File already exists.";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "No Content";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
            return Content("");
        }

        [HttpPost]
        public IActionResult GuardarFotoUsuario(IList<IFormFile> UploadFiles)
        {
            try
            {
                foreach (var file in UploadFiles)
                {
                    if (UploadFiles != null)
                    {
                        var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        filename = $@"{hostingEnv.WebRootPath}\assets\usuarios\{filename}";
                        if (!System.IO.File.Exists(filename))
                        {
                            string directorio = Path.GetDirectoryName(filename);
                            if (!Directory.Exists(directorio))
                            {
                                Directory.CreateDirectory(directorio);
                            }
                            using (FileStream fs = System.IO.File.Create(filename))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                        }
                        else
                        {
                            Response.Clear();
                            Response.StatusCode = 204;
                            Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File already exists.";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "No Content";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
            return Content("");
        }

        [HttpPost]
        public IActionResult AdjuntoEngagement(IList<IFormFile> UploadFiles)
        {
            try
            {
                foreach (var file in UploadFiles)
                {
                    if (UploadFiles != null)
                    {
                        var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        filename = $@"{hostingEnv.WebRootPath}\assets\adjuntos\{filename}";
                        if (!System.IO.File.Exists(filename))
                        {
                            string directorio = Path.GetDirectoryName(filename);
                            if (!Directory.Exists(directorio))
                            {
                                Directory.CreateDirectory(directorio);
                            }
                            using (FileStream fs = System.IO.File.Create(filename))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                        }
                        else
                        {
                            Response.Clear();
                            Response.StatusCode = 204;
                            Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File already exists.";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "No Content";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
            return Content("");
        }

        [HttpPost]
        [RequestSizeLimit(100000000)]
        public IActionResult GuardarAdjuntosPlantilla(IList<IFormFile> uploadFiles)
        {
            try
            {
                IFormFile file = uploadFiles.First();

                string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Replace(" ", "_").Trim('"');
                string fileFullName = $@"{hostingEnv.WebRootPath}\adjuntosPlantilla\{fileName}";

                if (System.IO.File.Exists(fileFullName))
                {
                    //return Conflict(new { Mensaje = "El archivo ya existe!" });
                    return Ok(new { Mensaje = "Adjunto Subido Correctamente!", Data = new { nombre = fileName, path = fileFullName } });
                }

                string directorio = Path.GetDirectoryName(fileFullName);

                if (!Directory.Exists(directorio))
                {
                    Directory.CreateDirectory(directorio);
                }

                using (FileStream fs = System.IO.File.Create(fileFullName))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }

                return Ok(new { Mensaje = "Adjunto Subido Correctamente!", Data = new { nombre = fileName, path = fileFullName } });
            }
            catch (Exception ex)
            {
                string mensaje = "Error al guardar adjunto!";
                this._log.Error(mensaje, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Mensaje = mensaje, Exception = new { Message = ex.Message, StackTrace = ex.StackTrace } });
            }
        }

        [AcceptVerbs("Post")]
        public IActionResult EliminarAdjuntoPlantilla(IList<IFormFile> uploadFiles)
        {
            try
            {
                IFormFile file = uploadFiles.First();

                string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Replace(" ", "_").Trim('"');
                string fileFullName = $@"{hostingEnv.WebRootPath}\adjuntosPlantilla\{fileName}";

                if (!System.IO.File.Exists(fileFullName))
                {
                    return NotFound(new { Mensaje = "El adjunto ya no existe!" });
                }

                System.IO.File.Delete(fileFullName);

                return Ok(new { Mensaje = "Adjunto Eliminado Correctamente!" });
            }
            catch (Exception ex)
            {
                string mensaje = "Error al elimnar adjunto!";
                this._log.Error(mensaje, ex);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Mensaje = mensaje, Exception = new { Message = ex.Message, StackTrace = ex.StackTrace } });
            }
        }
    }
}
