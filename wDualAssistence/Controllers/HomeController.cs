using Data;
using Entities.Security;
using Firebase.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using wDualAssistence.Helpers;
using wDualAssistence.Models;
using BSE = Business.Security;

using ESE = Entities.Security;

namespace wDualAssistence.Controllers
{
    [EnableCors("AllowAll")]
    public class HomeController : Controller
    {
        protected IApplicationDbContext _appCnx => (IApplicationDbContext)HttpContext.RequestServices.GetService(typeof(IApplicationDbContext));
        protected readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(HomeController));

        #region Metodos para Inicio de Sesion

        private class mensaje
        {
            public string usuario { get; set; }
            public string contenido { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            ViewModelBase vViewModelBase = new ViewModelBase();

            try
            {
                var auth = "RiZjCAbfCuqJyYCxBVQIapEk6WJQHjZ0o9MlEO7B"; // your app secret
                var firebaseClient = new FirebaseClient(
                  "https://chat-ejemplo-e365d.firebaseio.com",
                  new FirebaseOptions
                  {
                      AuthTokenAsyncFactory = () => Task.FromResult(auth)
                  });

                var dinos = await firebaseClient
                  .Child("mensaje")
                  .OnceAsync<mensaje>();

                var mensajes = firebaseClient
                  .Child("mensaje")
                  .AsObservable<mensaje>()
                   .Subscribe(d =>
                   {
                       _log.Info("KEY:" + d.Key);
                       _log.Info("USUARIO:" + d.Object.usuario);
                       _log.Info("MENSAJE:" + d.Object.contenido);
                   });
            }
            catch (Exception ex)
            {
            }

            return View(vViewModelBase);
        }

        [HttpGet]
        public IActionResult Index()
        {
            LoginViewModel oModelLogin = new LoginViewModel();
            return View(oModelLogin);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Validar(LoginViewModel uLogin)
        {
            if (!ModelState.IsValid)
            {
                //return Json(new { Result = false });
                ModelState.AddModelError(string.Empty, "Usuario o clave invalida");
                return PartialView(uLogin);
            }

            try
            {
                if (uLogin.login == "*" && uLogin.password == "*") throw new Exception("Su usuario aún no tiene configurado las credenciales");

                string sPassOriginal = uLogin.password;
                uLogin.password = Cryptography.Encrypt(sPassOriginal, Startup.llaveCryptography);
                ESE.Usuario eUsuarioValido = new BSE.Usuario(this._appCnx).Login(uLogin.login, uLogin.password);

                if (eUsuarioValido != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Sid, eUsuarioValido.id.ToString()),
                        new Claim(ClaimTypes.Name, eUsuarioValido.nombre),
                        new Claim(ClaimTypes.Role, "Administrador"),
                        new Claim(ClaimTypes.GroupSid, "666"),
                        new Claim("idUsuario", eUsuarioValido.id.ToString()),
                        new Claim("nombre_usuario", eUsuarioValido.nombre),
                        new Claim("correo", eUsuarioValido.email),
                        new Claim("contrasena", eUsuarioValido.clave),
                        new Claim("imagen_usuario", (eUsuarioValido.foto == null)? "":eUsuarioValido.foto),
                        new Claim("tipo", "1"), // 1= Usuario, 2= Empleado
                        new Claim("dashboard", (eUsuarioValido.dashboard)? "1":"0"), // 0= NO, 1= SI
                    };

                    ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "Basic");
                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    // Obtener accesos del usuario
                    List<Acceso> listaAcceso = new BSE.Usuario(this._appCnx).GetAccesoxUsuario(eUsuarioValido.id);

                    string url_dashboard_redirect = "";
                    foreach (var acceso in listaAcceso)
                    {
                        if (acceso.nombre.Equals("Dashboard V2"))
                        {
                            string urlBase = Url.Action("", "");
                            if (urlBase.Count() == 1)
                            {
                                url_dashboard_redirect = acceso.url_pagina;
                            }
                            else
                            {
                                url_dashboard_redirect = urlBase + acceso.url_pagina;
                            }
                        }
                    }

                    return Json(new { Result = true, Contenido = eUsuarioValido, tipo = 1, urldashboard = url_dashboard_redirect });
                }
                else
                {
                    uLogin.password = Cryptography.EncriptarMD5(sPassOriginal);
                    Entities.Public.Empleado eEmpleadoValido = new Business.Public.Empleado(this._appCnx).Login(uLogin.login, uLogin.password);

                    if (eEmpleadoValido == null) throw new Exception("No se encontro el Usuario!");

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Sid, eEmpleadoValido.id.ToString()),
                        new Claim(ClaimTypes.Name, eEmpleadoValido.nombre),
                        new Claim(ClaimTypes.Role, "Administrador"),
                        new Claim(ClaimTypes.GroupSid, "666"),
                        new Claim("idUsuario", eEmpleadoValido.id.ToString()),
                        new Claim("nombre_usuario", eEmpleadoValido.nombre + " " + eEmpleadoValido.apellido_paterno + " " + eEmpleadoValido.apellido_materno),
                        new Claim("correo", eEmpleadoValido.correo),
                        new Claim("contrasena", eEmpleadoValido.clave),
                        new Claim("imagen_usuario", ""),
                        new Claim("tipo", "2"), // 1= Usuario, 2= Empleado
                        new Claim("dashboard", "0"), // 0= NO, 1= SI
                    };

                    ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "Basic");
                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return Json(new { Result = true, Contenido = eEmpleadoValido, tipo = 2, urldashboard = "" });
                }
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError(string.Empty, ex.Message);
                //return View("Index" ,uLogin );
                return Json(new { Result = false, contenido = ex.Message });
            }
        }

        #endregion Metodos para Inicio de Sesion

        public IActionResult Error(int? statusCode = null)
        {
            ErrorViewModel oErrorViewModel = new ErrorViewModel();
            if (statusCode != null)
            {
                oErrorViewModel.RequestId = Convert.ToString(statusCode);
                oErrorViewModel.ErrorMessage = "Se produjo un error al procesar su solicitud";
            }

            return View(oErrorViewModel);
        }
    }
}