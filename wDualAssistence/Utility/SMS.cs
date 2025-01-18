using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;

using System.Text;



using Newtonsoft.Json.Linq;

using ENO = Entities.Notificador;
using BNO = Business.Notificador;
using System.Net.Http.Json;


namespace wDualAssistence.Utility
{
    public class SMS{

        private string destino;
        private string origen;
        private string texto;

        private string username = "";
        private string password = "";
        public static HttpClient client;

        public string Destino
        { get { return destino; } set { destino = value; } }
        public string Origen
        { get { return origen; } set { origen = value; } }
        public string Texto
        { get { return texto; } set { texto = value; } }

        public SMS()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://yz4y1.api.infobip.com/");    // URL base del servicio
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public async Task<string> enviarSMSGet()
        {
            try
            {
              
                return "";
            }
            catch (Exception ex)
            {
              
                return "Error: no se ha podido enviar el mensaje de texto al numero " + destino.ToString();
            }
        }

        public async Task<string> enviarSMSPost()
        {
            try
            {
              
                    return "";
              
              
            }
            catch (Exception ex)
            {
              
                return "Error: no se ha podido enviar el mensaje de texto al numero " + destino.ToString();
            }
        }

        public async Task<bool> enviarSMSPost(string sms_prueba, string content)
        {
            try
            {
              
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: no se ha podido enviar el mensaje de texto al numero " + ex);
            }
        }

        public async Task<string> enviarSMSPostPrueba(string sms_prueba, string content)
        {
            try
            {
                
                    return "";
                
            }
            catch (Exception ex)
            {
                //Log.writeErrorLog("Error: " + ex.ToString());
                //return "Error: " + ex.Message;
                throw new Exception("Error: no se ha podido enviar el mensaje de texto al numero " + ex);
            }
        }


    }

}
    