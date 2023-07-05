namespace Dotnet.Auth.API.Interfaces
{
    public interface IResponse<T> where T : class
    {
        bool success { get; set; }
        int statusCode { get; set; }
        string? message { get; set; }
        T? data { get; set; }
    }
}
