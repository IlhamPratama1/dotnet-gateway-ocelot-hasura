namespace Dotnet.Auth.API.Entities
{
    public class HasuraRes
    {
        public string? access_token { get; set; }
        public long expires { get; set; }
        public string? refresh_token { get; set; }
    }
}
