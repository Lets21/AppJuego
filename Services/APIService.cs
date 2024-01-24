using AppJuego.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppJuego.Services
{
    public class APIService
    {

        public static string _baseUrl;
        public HttpClient _httpClient;

        // Constructor: inicializa el URL base y el cliente HTTP.
        public APIService()
        {
            _baseUrl = "http://10.0.2.2:5082";
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
        }

        // Método para registrar un ganador
        public async Task<bool> RegistrarGanador(Juego juego)
        {
            var json = JsonConvert.SerializeObject(juego);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/Juego", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Juego>> ObtenerGanadores()
        {
            var response = await _httpClient.GetAsync("/api/Juego");
            if (response.IsSuccessStatusCode)
            {
                var json_response = await response.Content.ReadAsStringAsync();
                List<Juego> juegos = JsonConvert.DeserializeObject<List<Juego>>(json_response);
                return juegos;
            }
            return new List<Juego>();
        }

    }
}
