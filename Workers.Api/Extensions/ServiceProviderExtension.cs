using FastEndpoints.Swagger;
using NSwag;
using Workers.DataAccess.DbConnection.Implementations;
using Workers.DataAccess.DbConnection.Interfaces;
using Workers.DataAccess.Extensions.DapperAttributeMapper;
using Workers.DataAccess.Repositories.Implementations;
using Workers.DataAccess.Repositories.Interface;
using Workers.DataAccess.Services.Implementations;
using Workers.DataAccess.Services.Interfaces;

namespace Workers.Api.Extensions;

/// <summary>
/// Класс расширения programm
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

    /// <summary>
    /// Регистрация интерфесов-реализаций
    /// </summary>
    /// <param name="services">Сервис</param>
    public static void AddScope(this IServiceCollection services)
    {
        services.AddScoped<IWorkerService, WorkerService>();
        services.AddScoped<IWorkerRepository, WorkerRepository>();
        services.AddTransient<IDbManager, NpgsqlManager>();
    }
    
    /// <summary>
    /// Настройки дапера
    /// </summary>
    /// <param name="services">Сервис</param>
    public static void DapperSettings(this IServiceCollection services)
    {
        TypeMapper.Initialize("Workers.DataAccess.Dto.Bases");
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }
}