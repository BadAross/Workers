using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Workers.DataAccess.Db.Implementations;
using Workers.DataAccess.Db.Interfaces;
using Workers.DataAccess.Extensions.DapperAttributeMapper;
using Workers.DataAccess.Repositories.Implementations;
using Workers.DataAccess.Repositories.Interface;
using Workers.DataAccess.Services.Implementations;
using Workers.DataAccess.Services.Interfaces;

namespace Workers.DataAccess.Extensions;

/// <summary>
/// Класс расширения program
/// </summary>
public static class ServiceExtension
{
    public static async Task AddWorkers(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.DapperSettings();
        services.AddScope();
        
        await NpgsqlInitializer.InitializeDb(builder.Configuration);
    }

    /// <summary>
    /// Регистрация интерфесов-реализаций
    /// </summary>
    /// <param name="services">Сервис</param>
    private static void AddScope(this IServiceCollection services)
    {
        services.AddScoped<IWorkerService, WorkerService>();
        services.AddScoped<IWorkerRepository, WorkerRepository>();
        services.AddTransient<IDbManager, NpgsqlManager>();
    }
    
    /// <summary>
    /// Настройки дапера
    /// </summary>
    /// <param name="services">Сервис</param>
    private static void DapperSettings(this IServiceCollection services)
    {
        TypeMapper.Initialize("Workers.DataAccess.Dto.Bases");
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    public static async void InitializeDb(ConfigurationManager configuration)
    {
        await NpgsqlInitializer.InitializeDb(configuration);
    }
}