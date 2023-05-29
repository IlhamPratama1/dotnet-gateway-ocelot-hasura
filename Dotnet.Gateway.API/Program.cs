using Dotnet.Gateway.API.Middlewares;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services
    .AddOcelot(builder.Configuration);

var middleware = new CustomAuthMiddleware(builder.Configuration);
var app = builder.Build();
var middlewareConfiguration = middleware.ocelotMiddlewareConfiguration();

await app.UseOcelot(middlewareConfiguration);

app.Run();
