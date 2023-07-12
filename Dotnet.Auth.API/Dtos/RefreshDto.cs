namespace Dotnet.Auth.API.Dtos
{
    public class RefreshDto
    {
        public string? refresh_token { get; set; }
        public string? mode { get; set; }
    }
}
