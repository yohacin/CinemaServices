using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace wDualAssistence.Helpers
{
    public static class ControllerHelpers
    {
        public static void AddUpdateClaim(this Controller controlador, string key, string value)
        {
            var identity = controlador.User.Identity as ClaimsIdentity;
            if (identity == null)
                return;

            // check for existing claim and remove it
            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);

            // add new claim
            identity.AddClaim(new Claim(key, value));

            controlador.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, controlador.User);

            //var authenticationManager = controlador.HttpContext.Authentication;
            //authenticationManager.AuthenticateAsync = new AuthenticationResponseGrant(new ClaimsPrincipal(identity), new AuthenticationProperties() { IsPersistent = true });
        }

        public static byte[] GetBytes(this IFormFile formFile)
        {
            using(MemoryStream memoryStream = new MemoryStream())
            {
                formFile.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static MemoryStream GetStream(this IFormFile formFile)
        {
            using(MemoryStream memoryStream = new MemoryStream())
            {
                formFile.CopyTo(memoryStream);
                return memoryStream;
            }
        }
    }


    public enum Res
    {
        Ok,
        Error
    }
    public static class ResponseExtension
    {
        public static object GetResponse(this Res EnumResponse, Msg Mensaje)
        {           
            return GetResponse(EnumResponse, Mensaje.GetMsg()); 
        }

        public static object GetResponse(this Res EnumResponse, string Mensaje)
        {
            object response = new { };
            switch (EnumResponse)
            {
                case Res.Ok:
                    response = new { Result = true, Msg = Mensaje };
                    break;
                case Res.Error:
                    response = new { Result = false, Msg = Mensaje };
                    break;
                default:
                    break;
            }
            return response;
        }
    }

    public enum Msg
    {
        Correcto,
        Incorrecto,
        Custom
    }
    public static class MsgExtension
    {
        public static string GetMsg(this Msg EnumMsg, string MsgAdicional = "")
        {
            string Mensaje = "";
            switch (EnumMsg)
            {
                case Msg.Correcto:
                    Mensaje = "! Acción realizada correctamente !";
                    break;
                case Msg.Incorrecto:
                    Mensaje = "! Hubo un problema al realizar la acción !";
                    break;
                case Msg.Custom:
                    Mensaje = MsgAdicional;
                    break;
                default:
                    break;
            }
            return Mensaje;
        }
    }
}
