using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace wDualAssistence.Utility;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class FACE_RECOGNITION_API
{
    public static HttpClient client;
    public string URL_API = "http://127.0.0.1:5000/";

    public FACE_RECOGNITION_API(string sURL_API)
    {
        URL_API = sURL_API;

        client = new HttpClient();
        client.BaseAddress = new Uri(URL_API);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }


    // Método para consumir el endpoint /verifyb64 con la imagen en formato Base64
    // Método para consumir el endpoint /verifyb64 con la imagen en formato Base64
    public async Task<ResultadoVerificacionRostro> VerificarRostro(string empleadoId, string imagenBase64)
    {
        // Preparación del contenido JSON para la solicitud
        var payload = new
        {
            empleadoId,
            imagenB64 = imagenBase64
        };

        var jsonContent = JsonConvert.SerializeObject(payload);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Envío de la solicitud POST
        HttpResponseMessage response = await client.PostAsync("verifyb64", content);

        // Manejo de respuesta exitosa
        if (response.IsSuccessStatusCode)
        {
            string resultContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResultadoVerificacionRostro>(resultContent);
        }

        // Manejo de respuesta con error 400 (petición inválida)
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ResultadoVerificacionRostro>(errorContent);
        }

        // Lanza excepción para otros códigos de error no manejados explícitamente
        throw new HttpRequestException("Error de comunicación con el servicio de reconocimiento facial.");
    }

    public async Task<bool> ActualizarEntrenamiento()
    {
        // Envío de la solicitud POST
        HttpResponseMessage response = await client.PostAsync("update_faces", null);

        // Manejo de respuesta exitosa
        if (response.IsSuccessStatusCode)
        {
            string resultContent = await response.Content.ReadAsStringAsync();
            return true;
        }

        // Manejo de respuesta con error 400 (petición inválida)
        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception(errorContent);
        }

        // Lanza excepción para otros códigos de error no manejados explícitamente
        throw new HttpRequestException("Error de comunicación con el servicio de reconocimiento facial.");
    }

}

public class ResultadoVerificacionRostro
{
    public string EmpleadoId { get; set; }
    public double Certeza { get; set; }
    public string Error { get; set; }
}
