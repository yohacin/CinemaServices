using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using ENO = Entities.Notificador;
using BNO = Business.Notificador;
using System.Net.Http.Json;

namespace wDualAssistence.Utility
{
    public class SMS
    {
        private string destino;
        private string origen;
        private string texto;

        private string username = "DUALBIZ";
        private string password = "dualbiz123.";
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
                string url = "sms/1/text/query?" +
                    "username=" + username +
                    "&macpnPassw=" + password +
                    "&to=" + Destino +
                    "&from=" + Origen +
                    "&text=" + Texto;
                HttpResponseMessage response = await client.GetAsync(url);
                //Log.writeErrorLog("Se ha enviado el mensaje de texto con exito!");
                return "";
            }
            catch (Exception ex)
            {
                //Log.writeErrorLog("Error: " + ex.ToString());
                //return ("Error:" + ex.Message);
                return "Error: no se ha podido enviar el mensaje de texto al numero " + destino.ToString();
            }
        }

        public async Task<string> enviarSMSPost()
        {
            try
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);
                string encode = System.Convert.ToBase64String(bytes);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encode);

                var cuerpo = new
                {
                    to = Destino,
                    from = Origen,
                    text = Texto
                };

                HttpResponseMessage response = await client.PostAsJsonAsync("sms/2/text/single", cuerpo);
                //HttpResponseMessage response = client.PostAsJsonAsync("sms/2/text/single", cuerpo).Result;
                JObject jObject = JObject.Parse(await response.Content.ReadAsStringAsync());
                //JObject jObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                JToken messages = jObject["messages"];
                JArray mensajes = (JArray)messages;
                JToken mensaje = mensajes[0];
                JToken status = mensaje["status"];
                string resultado = status.Value<string>("name") + ", " + status.Value<string>("description");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    //Log.writeErrorLog("Se ha enviado el mensaje de texto con exito al numero " + destino);
                    //Log.writeErrorLog("Datos extras: " + resultado);
                    return "";
                }
                else
                {
                    throw new Exception("Fallo al enviar el SMS. Motivo: " + resultado);
                }
            }
            catch (Exception ex)
            {
                //Log.writeErrorLog("Error: " + ex.ToString());
                //return "Error: " + ex.Message;
                return "Error: no se ha podido enviar el mensaje de texto al numero " + destino.ToString();
            }
        }

        public async Task<bool> enviarSMSPost(string sms_prueba, string content)
        {
            try
            {
                bool result = false;
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);
                string encode = System.Convert.ToBase64String(bytes);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encode);

                var cuerpo = new
                {
                    to = "591" + sms_prueba,
                    from = "DualAssistance",
                    text = content
                };

                HttpResponseMessage response = await client.PostAsJsonAsync("sms/2/text/single", cuerpo);
                //HttpResponseMessage response = client.PostAsJsonAsync("sms/2/text/single", cuerpo).Result;
                JObject jObject = JObject.Parse(await response.Content.ReadAsStringAsync());
                //JObject jObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                JToken messages = jObject["messages"];
                JArray mensajes = (JArray)messages;
                JToken mensaje = mensajes[0];
                JToken status = mensaje["status"];
                string resultado = status.Value<string>("name") + ", " + status.Value<string>("description");
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
                return result;
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
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);
                string encode = System.Convert.ToBase64String(bytes);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encode);

                var cuerpo = new
                {
                    to = "591" + sms_prueba,
                    from = "Factura",
                    text = content
                };

                HttpResponseMessage response = await client.PostAsJsonAsync("sms/2/text/single", cuerpo);
                //HttpResponseMessage response = client.PostAsJsonAsync("sms/2/text/single", cuerpo).Result;
                JObject jObject = JObject.Parse(await response.Content.ReadAsStringAsync());
                //JObject jObject = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                JToken messages = jObject["messages"];
                JArray mensajes = (JArray)messages;
                JToken mensaje = mensajes[0];
                JToken status = mensaje["status"];
                string resultado = status.Value<string>("name") + ", " + status.Value<string>("description");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    return "";
                }
                else
                {
                    throw new Exception("Fallo al enviar el SMS. Motivo: " + resultado);
                }
            }
            catch (Exception ex)
            {
                //Log.writeErrorLog("Error: " + ex.ToString());
                //return "Error: " + ex.Message;
                throw new Exception("Error: no se ha podido enviar el mensaje de texto al numero " + ex);
            }
        }

        //public async Task<string> enviarSMSAmazonAsync()
        //    {
        //        try
        //        {
        //            AmazonSimpleNotificationServiceClient sns = new AmazonSimpleNotificationServiceClient("AKIAI64EBMAGZ5BNIBXA", "U88Oe8EAjjWLKAGew4Kct+AbBTe3iaIu4Rso58he", Amazon.RegionEndpoint.USEast1);
        //            Dictionary<String, MessageAttributeValue> dicAttr = new Dictionary<String, MessageAttributeValue>();
        //            MessageAttributeValue value = new MessageAttributeValue();
        //            value.StringValue = Origen;
        //            value.DataType = "String";
        //            dicAttr.Add("AWS.SNS.SMS.SenderID", value);

        //            PublishResponse response = await sns.PublishAsync(new PublishRequest() { PhoneNumber = Destino, Message = Texto, MessageAttributes = dicAttr });
        //            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        //            {
        //                //Log.writeErrorLog("Se ha enviado el mensaje de texto con exito al numero " + destino);
        //                return "";
        //            }
        //            else
        //            {
        //                throw new Exception("Fallo al enviar el SMS. Motivo: " + response.MessageId);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //Log.writeErrorLog("Error: " + ex.ToString());
        //            return "Error: no se ha podido enviar el mensaje de texto al numero " + destino.ToString();
        //        }

        //    }
    }
}