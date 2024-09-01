using System.Data;
using Dapper;
using Workers.DataAccess.Db.Interfaces;
using Workers.DataAccess.Dto.Bases;
using Workers.DataAccess.Dto.Requests;
using Workers.DataAccess.Dto.Responses;
using Workers.DataAccess.Exceptions;
using Workers.DataAccess.Repositories.Interface;
using static Workers.DataAccess.Constants.NpgsqlDbConstants;

namespace Workers.DataAccess.Repositories.Implementations;

/// <summary>
/// Реальзация репозитория сотрудника
/// </summary>
/// <param name="dbManager">Подключение к БД</param>
public sealed class WorkerRepository(IDbManager dbManager)
    : IWorkerRepository
{
    private readonly IDbConnection _dbConnection = dbManager.GetConnection();

    #region Queries

    private static string GetWorkerQuery()
        => $"""
            INSERT INTO "{SchemeName}"."{WorkerTableName}"
            (name, surname, phone, passport_id, department_id) 
            VALUES (@name, @surname, @phone, @passportId, @departmentId)
            RETURNING id
            """;
    
    private static string DeleteWorkerQuery()
        => $"""
            DELETE FROM "{SchemeName}"."{WorkerTableName}"
            WHERE id = @id
            """;
    
    private static string GetManyWorkerQuery() 
        => $"""
            SELECT 
                w.id AS worker_id, 
                w.name, 
                w.surname, 
                w.phone, 
                p.id AS passport_id, 
                pt.name AS type,
                p.passport_number AS number, 
                d.id AS department_id, 
                d.name,
                d.phone,
                c.id AS company_id, 
                c.name
            FROM 
                "{SchemeName}"."{WorkerTableName}" w
            LEFT JOIN 
                "{SchemeName}"."{PassportTableName}" p ON w.passport_id = p.Id
            LEFT JOIN 
                "{SchemeName}"."{PassportTypeTableName}" pt ON p.passport_type_id = pt.Id
            LEFT JOIN 
                "{SchemeName}"."{DepartmentTableName}" d ON w.department_id = d.Id
            LEFT JOIN 
                "{SchemeName}"."{CompanyTableName}" c ON d.company_id = c.Id 
            """;

    private static string UpdateWorkerQuery()
        => $"""
            UPDATE "{SchemeName}"."{WorkerTableName}"
            SET name = @name,
                surname = @surname,
                phone = @phone,
                department_id = @departmentId
            WHERE id = @id
            """;

    private static string IsThereThisPassportQuery()
        => $"""
            SELECT 1 
                FROM "{SchemeName}"."{PassportTableName}" 
            WHERE passport_number = @Number
            """;

    private static string CreatePassportQuery()
        => $"""
            INSERT INTO "{SchemeName}"."{PassportTableName}" 
            (passport_type_id, passport_number) 
            VALUES (@passportTypeId, @passportNumber)
            RETURNING id
            """;

    private static string UpdatePassportQuery()
        => $"""
            UPDATE "{SchemeName}"."{PassportTableName}"
            SET passport_type_id = @typeId,
                passport_number = @number
            WHERE id = @id
            """;

    private static string DeletePassportQuery()
        => $"""
            DELETE FROM "{SchemeName}"."{PassportTableName}"
            WHERE id = @id
            """;

    private static string GetWorkerPassportIdQuery()
        => $"""
            SELECT passport_id
            FROM "{SchemeName}"."{WorkerTableName}" 
            WHERE id = @Id
            """;

    #endregion

    /// <inheritdoc/> 
    public async Task<int> CreateWorkerAsync(CreateWorkerRequest request, CancellationToken cancellationToken)
    {
        using var transaction = _dbConnection.BeginTransaction();
        try
        {
            var createdPassportId = await CreatePassportAsync(request.Passport, transaction, cancellationToken);
            ValidatePassportId(createdPassportId);
        
            var workerId = await CreateWorkerInDatabaseAsync(request, createdPassportId, transaction, cancellationToken);
        
            transaction.Commit();
            return workerId;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw HandleException(ex, "Не удалось создать сотрудника.");
        }
    }
    
    /// <inheritdoc/> 
    public async Task DeleteWorkerAsync(int workerId, CancellationToken cancellationToken)
    {
        var passportId = await GetWorkerPassportIdAsync(workerId, cancellationToken);
        ValidatePassportExists(passportId);
    
        using var transaction = _dbConnection.BeginTransaction();
    
        try
        {
            await DeleteWorkerFromDatabaseAsync(workerId, transaction, cancellationToken);
            await DeletePassportAsync(passportId, transaction, cancellationToken);
        
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw HandleException(ex, "Не удалось удалить сотрудника.");
        }
    }
    
    /// <inheritdoc/> 
    public async Task<GetManyWorkerResponse> GetManyWorkerAsync(
        GetManyWorkerRequest filter, CancellationToken cancellationToken)
    {
        var sql = GetManyWorkerQuery();
        var parameters = new DynamicParameters();

        BuildWhereClause(ref sql, parameters, filter);

        try
        {
            var workers = await GetWorkerFromDatabaseAsync(sql, parameters);

            if (workers == null || !workers.Any())
            {
                throw new WorkerNotFoundException("Сотрудники не найдены. Попробуйте изменить фильтр.");
            }

            return new GetManyWorkerResponse
            {
                Workers = workers.ToList()
            };
        }
        catch (Exception ex)
        {
            throw HandleException(ex, "Произошла ошибка при получении сотрудников.");
        }
    }
    
    /// <inheritdoc/> 
    public async Task UpdateWorkerAsync(
        UpdateWorkerRequest request, CancellationToken cancellationToken)
    {
        using var transaction = _dbConnection.BeginTransaction();

        try
        {
            await UpdatePassportAsync(request.Id, request.Passport, 
                transaction, cancellationToken);

            await UpdateWorkerInDatabaseAsync(
                request, transaction, cancellationToken);
            
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw HandleException(ex, "Не удалось изменить сотрудника.");
        }
    }
    
    /// <inheritdoc/> 
    public async Task<bool> IsThereThisPassportAsync(
        string passportNumber, CancellationToken cancellationToken)
    {
        var sql = IsThereThisPassportQuery();  
        
        var commandDefinition = new CommandDefinition(sql,  
            parameters: new
            {
                Number = passportNumber
            },
            cancellationToken:  cancellationToken);
        
        var passportId = await _dbConnection
            .QueryFirstOrDefaultAsync<int?>(commandDefinition);

        return passportId.HasValue;
    }

    /// <summary>
    /// Метод создания поспорта
    /// </summary>
    /// <param name="passport">Данные паспорта</param>
    /// <param name="transaction">Транзакция</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Идентификатор паспорта</returns>
    private async Task<int> CreatePassportAsync(
        WritePassport passport, IDbTransaction transaction,
        CancellationToken cancellationToken)
    {
        var sql = CreatePassportQuery();    
        
        var commandDefinition = new CommandDefinition(sql,  
            parameters: new
            {
                passportTypeId = passport.TypeId, 
                passportNumber = passport.Number
            },
            transaction,
            cancellationToken:  cancellationToken);
        
        var passportId = await _dbConnection
            .ExecuteScalarAsync<int>(commandDefinition);

        return passportId;
    }
    
    /// <summary>
    /// Метод обновления данных паспорта
    /// </summary>
    /// <param name="workerId">Идентификатор сотрудника</param>
    /// <param name="passport">Данные паспорта</param>
    /// <param name="transaction">Транзация</param>
    /// <param name="cancellationToken">Токен отмены</param>
    private async Task UpdatePassportAsync(int workerId,
        WritePassport passport, IDbTransaction transaction,
        CancellationToken cancellationToken)
    {
        var passportId = 
            await GetWorkerPassportIdAsync(workerId, cancellationToken);
        
        var sql = UpdatePassportQuery();
        
        var commandDefinition = new CommandDefinition(sql,
            parameters: new
            {
                typeId = passport.TypeId,
                number = passport.Number,
                id = passportId
            }, 
            transaction,
            cancellationToken:  cancellationToken);

        await _dbConnection.ExecuteAsync(commandDefinition);
    }
    
    /// <summary>
    /// Метод Удаления пасспортных данных
    /// </summary>
    /// <param name="passportId">Идентификатор пасспорта</param>
    /// <param name="transaction">Транзакция</param>
    /// <param name="cancellationToken">Токен отмены</param>
    private async Task DeletePassportAsync(
        int passportId, IDbTransaction transaction,
        CancellationToken cancellationToken)
    {
        var sql = DeletePassportQuery();
        
        var commandDefinition = new CommandDefinition(sql,
            parameters: new
            {
                id = passportId
            }, 
            transaction,
            cancellationToken:  cancellationToken);

        await _dbConnection.ExecuteAsync(commandDefinition);
    }

    /// <summary>
    /// Метод получения паспорта сотрудника 
    /// </summary>
    /// <param name="workerId">Идентификатор сотрудника</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Идентификатор паспорта</returns>
    private async Task<int> GetWorkerPassportIdAsync(
        int workerId, CancellationToken cancellationToken)
    {
        var sql = GetWorkerPassportIdQuery();   
        
        var commandDefinition = new CommandDefinition(sql,  
            parameters: new
            {
                Id = workerId
            }, 
            cancellationToken: cancellationToken);
        
        var passportId = await _dbConnection
            .QueryFirstOrDefaultAsync<int>(commandDefinition);

        return passportId;
    }  
    
    /// <summary>
    /// Создание сотрудника
    /// </summary>
    /// <param name="request">Данные сотрудника</param>
    /// <param name="passportId">Идентификатор созданного пасспорта</param>
    /// <param name="transaction">Транзакция</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Идентификатор созданного сотрудника</returns>
    private async Task<int> CreateWorkerInDatabaseAsync(
        CreateWorkerRequest request, int passportId, 
        IDbTransaction transaction, CancellationToken cancellationToken)
    {
        var sql = GetWorkerQuery();
        var commandDefinition = new CommandDefinition(sql,  
            parameters: new 
            {
                name = request.Name, 
                surname = request.Surname, 
                phone = request.Phone,
                passportId,
                departmentId = request.DepartmentId
            }, 
            transaction,
            cancellationToken: cancellationToken);

        return await _dbConnection.ExecuteScalarAsync<int>(commandDefinition);
    }

    /// <summary>
    /// Удаление сотрудника
    /// </summary>
    /// <param name="workerId">Идентификатор сотрудника</param>
    /// <param name="transaction">Транзакция</param>
    /// <param name="cancellationToken">Токен отмены</param>
    private async Task DeleteWorkerFromDatabaseAsync(int workerId, IDbTransaction transaction, CancellationToken cancellationToken)
    {
        var sql = DeleteWorkerQuery();
        var commandDefinition = new CommandDefinition(sql,  
            parameters: new 
            {
                id = workerId
            },
            transaction,
            cancellationToken: cancellationToken);

        await _dbConnection.ExecuteAsync(commandDefinition);
    }
    
    /// <summary>
    /// Получение списка сотрудников
    /// </summary>
    /// <param name="sql">Запрос</param>
    /// <param name="parameters">Параметры</param>
    /// <returns>Список сотрудников</returns>
    private async Task<IEnumerable<Worker>> GetWorkerFromDatabaseAsync(string sql, DynamicParameters parameters)
    {
        return await _dbConnection.QueryAsync<Worker, ReadPassport, Department, Company, Worker>(
            sql,
            (worker, passport, department, company) =>
            {
                worker.Passport = passport;
                worker.Department = department;
                worker.Department.Company = company;
                return worker;
            },
            parameters,
            splitOn: "passport_id, department_id, company_id");
    }

    /// <summary>
    /// Изменение данных сотрудника
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="transaction">Транзакция</param>
    /// <param name="cancellationToken">Токен отмены</param>
    private async Task UpdateWorkerInDatabaseAsync(
        UpdateWorkerRequest request, IDbTransaction transaction, 
        CancellationToken cancellationToken)
    {
        var sql = UpdateWorkerQuery();
            
        var commandDefinition = new CommandDefinition(sql, 
            parameters: new
            {
                name = request.Name,
                surname = request.Surname,
                phone = request.Phone,
                departmentId = request.DepartmentId,
                id = request.Id
            }, 
            transaction,
            cancellationToken: cancellationToken);

        await _dbConnection.ExecuteAsync(commandDefinition);
    }  

    /// <summary>
    /// Проверка создания паспорта
    /// </summary>
    /// <param name="passportId">Идентификатор созданного паспорта</param>
    /// <exception cref="CreatePassportException">Если идентификатор меньше или равен 0
    /// - пасспорт не создан</exception>
    private static void ValidatePassportId(int passportId)
    {
        if (passportId <= 0)
        {
            throw new CreatePassportException("Не удалось создать паспорт.");
        }
    }

    /// <summary>
    /// Проверка наличия паспорта сотрудника
    /// </summary>
    /// <param name="passportId">Идентификатор паспорта</param>
    /// <exception cref="PassportNotFoundException">Если идентификатор равен 0, паспорт отсутвует</exception>
    private static void ValidatePassportExists(int passportId)
    {
        if (passportId == 0)
        {
            throw new PassportNotFoundException("Паспорт сотрудника не найден.");
        }
    }
    
    /// <summary>
    /// Добавление фильтров к запросу
    /// </summary>
    /// <param name="sql">Запрос</param>
    /// <param name="parameters">Праметры</param>
    /// <param name="filter">Фильтры</param>
    private static void BuildWhereClause(ref string sql, DynamicParameters parameters, GetManyWorkerRequest filter)
    {
        var whereAdded = false;

        if (filter.CompanyIds != null && filter.CompanyIds.Count != 0)
        {
            sql += $" WHERE d.company_id = ANY(@CompanyIds)";
            parameters.Add("CompanyIds", filter.CompanyIds);
            whereAdded = true;
        }

        if (filter.DepartmentIds != null && filter.DepartmentIds.Count != 0)
        {
            sql += whereAdded ? " AND " : " WHERE ";
            sql += $" w.department_id = ANY(@DepartmentIds)";
            parameters.Add("DepartmentIds", filter.DepartmentIds);
        }
    }
    
    /// <summary>
    /// Обработчик исключений при создании сотрудника
    /// </summary>
    /// <param name="ex">Исключение</param>
    /// <param name="defaultMessage"></param>
    /// <returns>Ошибку в зависимости от типа исключения</returns>
    private static Exception HandleException(Exception ex, string defaultMessage)
    {
        switch (ex)
        {
            case CreatePassportException:
                return ex;

            case PassportNotFoundException:
                return ex;

            case WorkerNotFoundException:
                return ex;

            default:
                return new InvalidOperationException(defaultMessage, ex);
        }
    }
}