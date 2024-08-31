using FastEndpoints.Swagger;
using NSwag;

namespace Workers.Api.Extensions;

/// <summary>
/// Класс расширения program
/// </summary>
public static class ServiceProviderExtension
{
    /// <summary>
    /// Настройки свагера
    /// </summary>
    /// <param name="services">Сервис</param>
    public static void AddSwaggerGenWithSettings(this IServiceCollection services)
    {
        services.SwaggerDocument(settings =>
        {
            settings.AutoTagPathSegmentIndex = 0;
            settings.DocumentSettings = generatorSettings =>
            {
                generatorSettings.DocumentName = "Workers API";
                generatorSettings.PostProcess = document =>
                {
                    document.Info = new OpenApiInfo { Title = "Workers API", Version = "v1" };
                };
            };
            settings.FlattenSchema = true;
        });
    }
}