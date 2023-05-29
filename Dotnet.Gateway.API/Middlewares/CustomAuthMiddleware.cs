using Dotnet.Gateway.API.Services;
using Ocelot.Middleware;

namespace Dotnet.Gateway.API.Middlewares
{
    public class CustomAuthMiddleware
    {
        public OcelotPipelineConfiguration ocelotMiddlewareConfiguration()
        {
            return new OcelotPipelineConfiguration
            {
                PreErrorResponderMiddleware = async (ctx, next) =>
                {
                    bool shouldBlockRequest = false;

                    if (ctx.Request.Path.Value == "/graphql")
                    {
                        shouldBlockRequest = true;
                        string customHeaderValue = ctx.Request.Headers["Authorization"].ToString();

                        // Cache 
                        bool isValid = await HttpRequestService.checkTokenValidation(customHeaderValue);
                        if (isValid)
                        {
                            shouldBlockRequest = false;
                        }
                    }

                    if (shouldBlockRequest)
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
