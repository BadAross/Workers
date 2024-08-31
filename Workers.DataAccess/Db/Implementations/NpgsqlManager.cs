using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Workers.DataAccess.Db.Interfaces;

namespace Workers.DataAccess.Db.Implementations;

/// <summary>
/// Подключение к базе postgres
/// </summary>
public sealed class NpgsqlManager : IDbManager
{ 
    private readonly IDbConnection _dbConnection;

    public NpgsqlManager(IConfiguration configuration)
    {
        var npgsqlConnection = configuration["ConnectionStrings:PostgresConnection"];
        _dbConnection = new NpgsqlConnection(npgsqlConnection);
    }

    /// <inheritdoc/> 
    public IDbConnection GetConnection() 
    {
        _dbConnection.Open();
        return _dbConnection;
    }
}