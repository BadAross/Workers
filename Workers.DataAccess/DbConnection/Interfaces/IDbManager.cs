using System.Data;

namespace Workers.DataAccess.DbConnection.Interfaces;

/// <summary>
/// Интерфейс подключения к БД
/// </summary>
public interface IDbManager
{
    /// <summary>
    /// Получение подключчения
    /// </summary>
    /// <returns>Открытое подключение к БД</returns>
    public IDbConnection GetConnection();
}