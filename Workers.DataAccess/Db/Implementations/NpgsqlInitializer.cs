using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using static Workers.DataAccess.Constants.NpgsqlDbConstants;

namespace Workers.DataAccess.Db.Implementations;

public static class NpgsqlInitializer
{
    #region Query

    
    private static string IsDbInitializedQuery()
        => $"""
            SELECT COUNT(*) 
            FROM pg_database 
            WHERE datname = @dbName;
            """;
    
    private static string CreateDbQuery(string dbName)
        => $"""
            CREATE DATABASE "{dbName}";
            """;
    
    private static string DeleteDbQuery(string dbName)
        => $"""
             DROP DATABASE "{dbName}";
             """;

    private static string CreateSchemeQuery(string schemeName)
        => $"""
            CREATE SCHEMA IF NOT EXISTS "{schemeName}";
            """;
    
    private static string CreateWorkerTableQuery()
        => $"""
            CREATE TABLE IF NOT EXISTS "{SchemeName}"."{WorkerTableName}" (
           	    id SERIAL PRIMARY KEY,
           	    "name" VARCHAR(200) NOT NULL,
           	    surname VARCHAR(200) NOT NULL,
           	    phone VARCHAR(16) NULL,
           	    passport_id INT NOT NULL,
           	    department_id INT NOT NULL,
           	    FOREIGN KEY (passport_id) REFERENCES "{SchemeName}"."{PassportTableName}"(id),
           	    FOREIGN KEY (department_id) REFERENCES "{SchemeName}"."{DepartmentTableName}"(id)
            );
            COMMENT ON TABLE "{SchemeName}"."{WorkerTableName}" IS 'Таблица сотрудников';
           
            COMMENT ON COLUMN "{SchemeName}"."{WorkerTableName}".id IS 'Идентификатор';
            COMMENT ON COLUMN "{SchemeName}"."{WorkerTableName}"."name" IS 'Имя';
            COMMENT ON COLUMN "{SchemeName}"."{WorkerTableName}".surname IS 'Фамилия';
            COMMENT ON COLUMN "{SchemeName}"."{WorkerTableName}".phone IS 'Номер телефона';
            COMMENT ON COLUMN "{SchemeName}"."{WorkerTableName}".passport_id IS 'Идентификатор паспортных данных';
            COMMENT ON COLUMN "{SchemeName}"."{WorkerTableName}".department_id IS 'Идентификатор отдела';
           """;

    private static string CreateDepartmentTableQuery()
        => $"""
            CREATE TABLE IF NOT EXISTS "{SchemeName}"."{DepartmentTableName}" (
                id SERIAL PRIMARY KEY,
                "name" VARCHAR(500) NOT NULL,
                phone VARCHAR(16) NULL,
                company_id INT NOT NULL,
                FOREIGN KEY (company_id) REFERENCES "{SchemeName}"."{CompanyTableName}"(id)
            );
            COMMENT ON TABLE "{SchemeName}"."{DepartmentTableName}" IS 'Таблица отделов';
            
            COMMENT ON COLUMN "{SchemeName}"."{DepartmentTableName}".id IS 'Идентификатор';
            COMMENT ON COLUMN "{SchemeName}"."{DepartmentTableName}"."name" IS 'Название';
            COMMENT ON COLUMN "{SchemeName}"."{DepartmentTableName}".phone IS 'Номер телефона';
            COMMENT ON COLUMN "{SchemeName}"."{DepartmentTableName}".company_id IS 'Идентификатор компании';
            """;
                
    private static string CreateCompanyTableQuery()
        => $"""
            CREATE TABLE IF NOT EXISTS "{SchemeName}"."{CompanyTableName}" (
            	id SERIAL PRIMARY KEY,
            	"name" VARCHAR(500) NOT null
            );
            COMMENT ON TABLE "{SchemeName}"."{CompanyTableName}" IS 'Таблица компаний';
            
            COMMENT ON COLUMN "{SchemeName}"."{CompanyTableName}".id IS 'Идентификатор';
            COMMENT ON COLUMN "{SchemeName}"."{CompanyTableName}"."name" IS 'Название';
            """;
    
    private static string CreatePassportTableQuery()
        => $"""
            CREATE TABLE IF NOT EXISTS "{SchemeName}"."{PassportTableName}" (
            	id SERIAL PRIMARY KEY,
            	passport_type_id INT NOT NULL,
            	passport_number VARCHAR(50) NOT NULL,
            	FOREIGN KEY (passport_type_id) REFERENCES "{SchemeName}"."{PassportTypeTableName}"(id)
            );
            COMMENT ON TABLE "{SchemeName}"."{PassportTableName}" IS 'Таблица данных паспортов';
            
            COMMENT ON COLUMN "{SchemeName}"."{PassportTableName}".id IS 'Идентификатор';
            COMMENT ON COLUMN "{SchemeName}"."{PassportTableName}".passport_type_id IS 'Идентификатор типа паспорта';
            COMMENT ON COLUMN "{SchemeName}"."{PassportTableName}".passport_number IS 'Номер паспорта';
            """;
    
    private static string CreatePassportTypeTableQuery()
        => $"""
            CREATE TABLE IF NOT EXISTS "{SchemeName}"."{PassportTypeTableName}" (
            	id SERIAL PRIMARY KEY,
            	"name" VARCHAR(500) NOT null
            );
            COMMENT ON TABLE "{SchemeName}"."{PassportTypeTableName}" IS 'Таблица общего словаря данных';
            
            COMMENT ON COLUMN "{SchemeName}"."{PassportTypeTableName}".id IS 'Идентификатор';
            COMMENT ON COLUMN "{SchemeName}"."{PassportTypeTableName}"."name" IS 'Название типа паспорта';
            """;

    #endregion

    /// <summary>
    /// Инициализация БД на postgres
    /// </summary>
    /// <param name="configuration">Конфиг</param>
    /// <exception cref="InvalidOperationException">Не удалось создать базу</exception>
    public static async Task InitializeDb(IConfiguration configuration)
    {
       var connectionString = configuration["ConnectionStrings:PostgresConnection"];
       if (connectionString is null)
       {
           throw new InvalidOperationException("Нет строки поключения к БД.");
       }
       
       await using (var initialConnection = GetNotDbNameConnection(connectionString))
       {
           await initialConnection.OpenAsync();

           var dbGetName = GetDatabaseName(connectionString);
           var isInitialized = await IsDbInitialized(initialConnection, dbGetName!);
           if (isInitialized)
           {
               return;
           }
           await CreateDb(initialConnection, dbGetName!);
       }
        
       await using var connection = new NpgsqlConnection(connectionString);
       await connection.OpenAsync();

       await using var transaction = await connection.BeginTransactionAsync();

       try
       {
           await InitializeSchema(connection, transaction);
           await transaction.CommitAsync();
       }
       catch (Exception)
       {
           await using var deleteConnection = GetNotDbNameConnection(connectionString);
           await DeleteDb(connection.Database, deleteConnection);
           await transaction.RollbackAsync();
           throw new InvalidOperationException("Не удалось создать БД.");
       }
    }
    
    /// <summary>
    /// Получение названия БД из appsettings
    /// </summary>
    /// <param name="connectionString">Строка подключения</param>
    /// <returns>Название БД</returns>
    private static string? GetDatabaseName(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        return builder.Database;
    }

    /// <summary>
    /// Проверка наличия БД
    /// </summary>
    /// <param name="connection">Подключение к БД</param>
    /// <param name="dbName">Название БД</param>
    /// <returns>true - БД существует, иначе - false</returns>
    private static async Task<bool> IsDbInitialized(NpgsqlConnection connection, string dbName)
    {
        var query = IsDbInitializedQuery();
        
        var dbExists = await connection
            .QuerySingleOrDefaultAsync<int>(query, new { dbName });

        return dbExists > 0;
    }

    /// <summary>
    /// Создание БД
    /// </summary>
    /// <param name="connection">Подключение к БД</param>
    /// <param name="dbName">Название БД</param>
    private static async Task CreateDb(NpgsqlConnection connection, string dbName)
    {
        var query = CreateDbQuery(dbName);
        await connection.ExecuteAsync(query);
    }

    /// <summary>
    /// Инициализация схемы
    /// </summary>
    /// <param name="connection">Подключение к БД</param>
    /// <param name="transaction">Транзакция</param>
    private static async Task InitializeSchema(NpgsqlConnection connection, IDbTransaction transaction)
    {
        await CreateScheme(SchemeName, connection, transaction);

        var passportTypeTable = CreatePassportTypeTableQuery();
        await CreateTable(passportTypeTable, connection, transaction);
        
        var passportTable = CreatePassportTableQuery();
        await CreateTable(passportTable, connection, transaction);
        
        var companyTable = CreateCompanyTableQuery();
        await CreateTable(companyTable, connection, transaction);
        
        var departmentTable = CreateDepartmentTableQuery();
        await CreateTable(departmentTable, connection, transaction);
        
        var workerTable = CreateWorkerTableQuery();
        await CreateTable(workerTable, connection, transaction);
    }
    
    /// <summary>
    /// Создание схемы
    /// </summary>
    /// <param name="schemeName">Название схемы</param>
    /// <param name="connection">Подключение к БД</param>
    /// <param name="transaction">Транзакция</param>
    private static async Task CreateScheme(string schemeName, 
        NpgsqlConnection connection, IDbTransaction transaction)
    {
        var query = CreateSchemeQuery(schemeName);
        await connection.ExecuteAsync(query, transaction);
    }

    /// <summary>
    /// Создание таблицы
    /// </summary>
    /// <param name="createTableQuery">Запрос на создание</param>
    /// <param name="connection">Подключение к БД</param>
    /// <param name="transaction">Транзакция</param>
    private static async Task CreateTable(string createTableQuery, 
        NpgsqlConnection connection, IDbTransaction transaction)
        => await connection.ExecuteAsync(createTableQuery, transaction);

    /// <summary>
    /// Удаление БД
    /// </summary>
    /// <param name="dbName">Название БД</param>
    /// <param name="deleteConnection">Подключение дял удаления</param>
    private static async Task DeleteDb(string dbName, NpgsqlConnection deleteConnection)
    {
        var query = DeleteDbQuery(dbName);
        await deleteConnection.ExecuteAsync(query);
    }
    
    /// <summary>
    /// Подучение подключения без названия БД
    /// </summary>
    /// <param name="connectionString">Строка подключения</param>
    /// <returns>Подключение к БД</returns>
    private static NpgsqlConnection GetNotDbNameConnection(string connectionString) 
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = null
        };

        return new NpgsqlConnection(builder.ToString());
    }
}