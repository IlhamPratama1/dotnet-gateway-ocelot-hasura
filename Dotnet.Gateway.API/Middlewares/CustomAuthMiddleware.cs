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
                PreErrorResponderMiddleware = async (ctx, next) =>
                {
                    bool _blockRequest = false;
                    string path = ctx.Request.Path.Value != null ? ctx.Request.Path.Value : "";

                    if (protectedRoutes.Any(route => path.Contains(route)))
                    {
                        _blockRequest = true;
                        string customHeaderValue = ctx.Request.Headers["Authorization"].ToString();

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
                        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return;
                    }

                    await next.Invoke();
                }
            };
        }        
    }
}
