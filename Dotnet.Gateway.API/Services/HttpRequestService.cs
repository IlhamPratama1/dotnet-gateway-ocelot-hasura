namespace Dotnet.Gateway.API.Services
{
    public class HttpRequestService
    {
        private string _url;
        private HttpClient _httpClient;

        public HttpRequestService() { 
            _url = $"{Environment.GetEnvironmentVariable("ASPNETCORE_AUTH_URL")}:{Environment.GetEnvironmentVariable("ASPNETCORE_AUTH_PORT")}";
            _httpClient = new HttpClient();
        }

        public async Task<Boolean> checkTokenValidation(string bearerToken)
        {
            if (string.IsNullOrEmpty(bearerToken))
            {
                return false;
            }

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_url}/auth/current")
            {
                Headers = { { "Authorization", bearerToken } }
            };

            var response = await _httpClient.SendAsync(httpRequestMessage);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return false;
            }
        }
    }
}
