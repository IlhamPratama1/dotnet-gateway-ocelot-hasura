using Dotnet.Gateway.API.Middlewares;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services
    .AddOcelot(builder.Configuration);

var middleware = new CustomAuthMiddleware(builder.Configuration);
var middlewareConfiguration = middleware.ocelotMiddlewareConfiguration();

var app = builder.Build();
await app.UseOcelot(middlewareConfiguration);

app.Run();
