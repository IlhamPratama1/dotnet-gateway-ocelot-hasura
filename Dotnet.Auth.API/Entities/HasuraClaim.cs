using Newtonsoft.Json;

namespace Dotnet.Auth.API.Entities
{
    public class HasuraClaim
    {
        [JsonProperty("x-hasura-allowed-roles")]
        public string[]? roles { get; set; }

        [JsonProperty("x-hasura-default-role")]
        public string? role { get; set; }

        [JsonProperty("x-hasura-user-id")]
        public string? id { get; set; }
    }
}
