namespace Dotnet.Auth.API.Dtos
{
    public class RegisterDto
    {
        public string? email { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? password { get; set; }
        public string? role { get; set; }
    }
}
