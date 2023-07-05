using System.Net.Http.Headers;
using System.Text;
using Dotnet.Auth.API.Dtos;
using Newtonsoft.Json;

namespace Dotnet.Auth.API.Services
{
    public static class HttpRequestService<T>
    {
        public static readonly HttpClient _client;

        static HttpRequestService()
        {
            string url = Environment.GetEnvironmentVariable("ASPNETCORE_DIRECTUS_URL") ?? "NOT_FOUND";
            _client = new HttpClient() { BaseAddress = new Uri(url) };
        }

        public static void SetBearerAuth(string bearerToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        public static async Task<T> PostAsync<DTO>(string relativeUrl, DTO data)
        {
            var jsonData = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync(relativeUrl, content);
            response.EnsureSuccessStatusCode();

            string jsonString = await response.Content.ReadAsStringAsync();
            T? result = JsonConvert.DeserializeObject<T>(jsonString);

            return result ?? throw new InvalidOperationException("Deserialized object is null.");
        }

        public static async Task<T> GetAsync(string relativeUrl)
        {
            HttpResponseMessage response = await _client.GetAsync(relativeUrl);
            response.EnsureSuccessStatusCode();

            string jsonString = await response.Content.ReadAsStringAsync();
            T? user = JsonConvert.DeserializeObject<T>(jsonString);

            return user ?? throw new InvalidOperationException("Deserialized object is null.");
        }
    }
}
