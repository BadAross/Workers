using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Workers.DataAccess.DbConnection.Interfaces;

namespace Workers.DataAccess.DbConnection.Implementations;

/// <summary>
/// Подключение к базе postgres
/// </summary>
/// <param name="configuration">Сонфиг</param>
public sealed class NpgsqlManager(IConfiguration configuration) : IDbManager
{
    private readonly string? _npgsqlConnection 
        = configuration["DbConnectionStrings:PostgresConnection"];

    public IDbConnection GetConnection() 
    {
        var connection = new NpgsqlConnection(_npgsqlConnection);
        connection.Open();
        return connection;
    }
}