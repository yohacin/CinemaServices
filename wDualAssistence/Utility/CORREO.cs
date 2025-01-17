using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

#region Abreviaturas
using ENO = Entities.Notificador;
#endregion

namespace wDualAssistence.Utility
{
    public class CORREO
    {
        public string _HOST = "mail.dualbiz.net";
        public int _PORT = 25;
        public string _USER = "billy.crespo@dualbiz.net";
        public string _PASS = "B1lly123..";

        public bool EnviarCorreoPrueba(string correo)
        {
            try
            {
                MailMessage Msg = new MailMessage();

                Msg.From = new MailAddress(_USER);
                Msg.To.Add(correo);
                Msg.CC.Add(correo);
                Msg.Subject = "Prueba";
                Msg.IsBodyHtml = true;
                Msg.Body = "<h1>Prueba envio de correo</h1>";

                NetworkCredential loginInfo = new NetworkCredential(_USER, _PASS);

                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Host = _HOST;
                smtp.Port = _PORT;
                smtp.Credentials = loginInfo;

                ServicePointManager.ServerCertificateValidationCallback =
                        delegate (
                            object s,
                            X509Certificate certificate,
                            X509Chain chain,
                            SslPolicyErrors sslPolicyErrors
                        ) {
                            return true;
                        };

                smtp.Send(Msg);

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("ERROR" + e);
            }
        }

        public bool EnviarCorreoContenido(string correo, string contenido, string asunto = "Prueba")
        {
            try
            {
                MailMessage Msg = new MailMessage();

                Msg.From = new MailAddress(_USER);
                Msg.To.Add(correo);
                Msg.Subject = asunto;
                Msg.IsBodyHtml = true;
                Msg.Body = contenido;

                NetworkCredential loginInfo = new NetworkCredential(_USER, _PASS);

                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Host = _HOST;
                smtp.Port = _PORT;
                smtp.Credentials = loginInfo;

                ServicePointManager.ServerCertificateValidationCallback =
                        delegate (
                            object s,
                            X509Certificate certificate,
                            X509Chain chain,
                            SslPolicyErrors sslPolicyErrors
                        ) {
                            return true;
                        };

                smtp.Send(Msg);

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("ERROR" + e);
            }
        }

        public bool EnviarCorreoContenidoLista(string correo, string contenido, List<string> listaCopia, string asunto = "Sin Asunto")
        {
            try
            {
                MailMessage Msg = new MailMessage();

                Msg.From = new MailAddress(_USER);
                Msg.To.Add(correo);
                foreach (string item in listaCopia)
                { /// ----------- Lista de correos en copia
                    Msg.CC.Add(item);
                }
                Msg.Subject = asunto;
                Msg.IsBodyHtml = true;
                Msg.Body = contenido;

                NetworkCredential loginInfo = new NetworkCredential(_USER, _PASS);

                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Host = _HOST;
                smtp.Port = _PORT;
                smtp.Credentials = loginInfo;

                ServicePointManager.ServerCertificateValidationCallback =
                        delegate (
                            object s,
                            X509Certificate certificate,
                            X509Chain chain,
                            SslPolicyErrors sslPolicyErrors
                        ) {
                            return true;
                        };

                smtp.Send(Msg);

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("ERROR" + e);
            }
        }

        public bool EnviarCorreoConAdjunto(string correoEnviar, string asuntoEnviar, string contenido, FileContentResult eFileContentResult)
        {
            try
            {
                MailMessage Msg = new MailMessage();

                Msg.From = new MailAddress(_USER);
                Msg.To.Add(correoEnviar);
                Msg.Subject = asuntoEnviar;
                Msg.IsBodyHtml = true;
                Msg.Body = contenido;

                var contentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);

                MemoryStream attachmentStream = new MemoryStream(eFileContentResult.FileContents);
                var attachmentTitle = "factura";

                Msg.Attachments.Add(new Attachment(attachmentStream, attachmentTitle, contentType.ToString()));

                NetworkCredential loginInfo = new NetworkCredential(_USER, _PASS);

                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Host = _HOST;
                smtp.Port = _PORT;
                smtp.Credentials = loginInfo;

                ServicePointManager.ServerCertificateValidationCallback =
                        delegate (
                            object s,
                            X509Certificate certificate,
                            X509Chain chain,
                            SslPolicyErrors sslPolicyErrors
                        ) {
                            return true;
                        };

                smtp.Send(Msg);

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("ERROR" + e);
            }
        }

        public bool EnviarConAdjuntos(string destinatario, string asunto, string contenido, List<ENO.Adjunto> adjuntos)
        {
            try
            {
                MailMessage mensaje = new MailMessage();
                mensaje.From = new MailAddress(this._USER);
                mensaje.To.Add(destinatario);
                mensaje.Subject = asunto;
                mensaje.IsBodyHtml = true;
                mensaje.Body = contenido;

                foreach (ENO.Adjunto adjunto in adjuntos)
                {
                    Attachment attachment = new Attachment(adjunto.path, adjunto.mime);
                    attachment.Name = $@"{adjunto.alias}.{adjunto.extension}";
                    mensaje.Attachments.Add(attachment);
                }

                NetworkCredential loginInfo = new NetworkCredential(this._USER, this._PASS);

                SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Host = this._HOST;
                smtp.Port = this._PORT;
                smtp.Credentials = loginInfo;

                ServicePointManager.ServerCertificateValidationCallback =
                        delegate (
                            object s,
                            X509Certificate certificate,
                            X509Chain chain,
                            SslPolicyErrors sslPolicyErrors
                        ) {
                            return true;
                        };

                smtp.Send(mensaje);

                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
