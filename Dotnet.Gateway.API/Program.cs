using Dotnet.Gateway.API.Services;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services
    .AddOcelot(builder.Configuration);

var app = builder.Build();

var configuration = new OcelotPipelineConfiguration
{
    PreErrorResponderMiddleware = async (ctx, next) =>
    {
        bool shouldBlockRequest = false;

        if (ctx.Request.Path.Value == "/graphql")
        {
            shouldBlockRequest = true;
            string customHeaderValue = ctx.Request.Headers["Authorization"].ToString();
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

await app.UseOcelot(configuration);

app.Run();
