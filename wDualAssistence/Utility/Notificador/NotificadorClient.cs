using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace wDualAssistence.Utility.Notificador
{
    public class NotificadorClient
    {
        public string urlApi { get; set; }

        private HttpClient httpClient;
        private string urlApiDefault = "http://demo.dualbiz.net/dms/";

        public NotificadorClient()
        {
            urlApi = urlApiDefault;
            inicializar();
        }

        private void inicializar()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(urlApi);    // URL base del servicio
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void configurar(string urlApi)
        {
            if (urlApi != null && urlApi == "default")
            {
                this.urlApi = urlApiDefault;
            }
            else
            {
                this.urlApi = urlApi;
            }

            inicializar();
        }

        public async Task<ResponseNotificador> ejecutarCampana(long idCampana)
        {
            string url = "Campana/EjecutarCampana";
            HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, new Campana(idCampana));
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"{((long)response.StatusCode)}-{response.ReasonPhrase}: {response.RequestMessage.RequestUri.AbsoluteUri}");
            }
            return JsonConvert.DeserializeObject<ResponseNotificador>(await response.Content.ReadAsStringAsync());
        }
    }
}