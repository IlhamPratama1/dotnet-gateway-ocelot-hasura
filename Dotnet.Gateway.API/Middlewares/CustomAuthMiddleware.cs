using Dotnet.Gateway.API.Services;
using Ocelot.Middleware;

namespace Dotnet.Gateway.API.Middlewares
{
    public class CustomAuthMiddleware
    {
        string[] protectedRoutes = new string[] { "/graphql", "/api/" };

        HttpRequestService requestService;
        CacheService cacheService;

        public CustomAuthMiddleware(ConfigurationManager _configuration) {
            requestService = new HttpRequestService();
            cacheService = new CacheService(_configuration);
        }
        public OcelotPipelineConfiguration ocelotMiddlewareConfiguration()
        {
            return new OcelotPipelineConfiguration
            {
                PreErrorResponderMiddleware = async (_ctx, _next) =>
                {
                    bool _blockRequest = false;
                    string _path = _ctx.Request.Path.Value != null ? _ctx.Request.Path.Value : "";

                    if (protectedRoutes.Any(route => _path.Contains(route)))
                    {
                        _blockRequest = true;
                        string customHeaderValue = _ctx.Request.Headers["Authorization"].ToString();

                        if (cacheService.CheckKeyExist(customHeaderValue))
                        {
                            _blockRequest = false;
                        }
                        else if (await requestService.checkTokenValidation(customHeaderValue))
                        {
                            cacheService.SetString(customHeaderValue, customHeaderValue);
                            _blockRequest = false;
                        }
                    }

                    if (_blockRequest)
                    {
                        _ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return;
                    }

                    await _next.Invoke();
                }
            };
        }        
    }
}
