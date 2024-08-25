using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Swagger;
using Workers.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddFastEndpoints()
    .AddSwaggerGenWithSettings();
services.AddScope();

var app = builder.Build();
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