namespace Dotnet.Gateway.API.Services
{
    public class HttpRequestService
    {
        string baseUrl = "http://192.168.18.153:8055";

        public async Task<Boolean> checkTokenValidation(string bearerToken)
        {
            var _client = new HttpClient();
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/auth/current")
            {
                Headers = { { "Authorization", bearerToken } }
            };

            var response = await _client.SendAsync(httpRequestMessage);
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
