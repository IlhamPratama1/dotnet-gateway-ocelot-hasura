using Dotnet.Auth.API.Interfaces;

namespace Dotnet.Auth.API.Entities
{
    public class Response<T> : IResponse<T> where T : class
    {
        public bool success { get; set; }
        public int statusCode { get; set; }
        public string? message { get; set; }
        public T? data { get; set; }
    }
}
