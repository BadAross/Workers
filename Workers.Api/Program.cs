using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using Workers.Api.Extensions;
using Workers.Api.Middlewares;
using Workers.DataAccess.Extensions;

var builder = WebApplication.CreateBuilder(args);
await builder.AddWorkers();

var services = builder.Services;
services.AddFastEndpoints()
    .AddSwaggerGenWithSettings();

var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseFastEndpoints(conf =>
    {
        conf.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
        conf.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        conf.Endpoints.RoutePrefix = "api";
        conf.Endpoints.ShortNames = true;
    })
    .UseOpenApi()
    .UseSwaggerUi(x => x.ConfigureDefaults());

app.Run();