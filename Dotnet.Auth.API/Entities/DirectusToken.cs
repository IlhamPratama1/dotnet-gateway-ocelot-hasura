namespace Dotnet.Auth.API.Entities
{
    public class DirectusToken
    {
        public string? id { get; set; }
        public string? role { get; set; }
        public bool app_access { get; set; }
        public bool admin_access { get; set; }
        public long iat { get; set; }
        public long exp { get; set; }
        public string? iss { get; set; }
    }
}
