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
    public async Task<int> CreateWorkerAsync(
        CreateWorkerRequest request, CancellationToken cancellationToken)
    {
        using var transaction = _dbConnection.BeginTransaction();
        
        try
        {
            var createdPassportId = await CreatePassport(
                request.Passport, transaction, cancellationToken);
            
            if (createdPassportId <= 0)
            {
                throw new CreatePassportException("Не удалось создать паспорт.");
            }
            
            var sql = GetWorkerQuery();    
            
            var commandDefinition = new CommandDefinition(sql,  
                parameters: new 
                {
                    name = request.Name, 
                    surname = request.Surname, 
                    phone = request.Phone,
                    passportId = createdPassportId,
                    departmentId = request.DepartmentId
                },
                transaction,
                cancellationToken: cancellationToken);
        
            var workerId = await _dbConnection
                .ExecuteScalarAsync<int>(commandDefinition);
            
            transaction.Commit();

            return workerId;
        }
        catch (CreatePassportException ex)
        {
            transaction.Rollback();
            throw;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            throw new InvalidOperationException("Не удалось создать сотрудника.");
        }
    }
    
    /// <inheritdoc/> 
    public async Task DeleteWorkerAsync(
        int workerId, CancellationToken cancellationToken)
    {
        var passportId = 
            await GetWorkerPassportId(workerId, cancellationToken);
     
         if (passportId == 0)
         {
             throw new InvalidOperationException("Сотрудник не найден.");
         }
         
        using var transaction = _dbConnection.BeginTransaction();

        try
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
            
            await DeletePassport(passportId, transaction, cancellationToken);
            
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw new InvalidOperationException("Не удалось удалить сотрудника.");
        }
    }

    /// <inheritdoc/> 
    public async Task<GetManyWorkerResponse> GetManyWorkerAsync(
        GetManyWorkerRequest filter, CancellationToken cancellationToken)
    {
        var sql = GetManyWorkerQuery();

        var parameters = new DynamicParameters();
    
        var whereAdded = false;

        if (filter.CompanyIds != null && filter.CompanyIds.Any())
        {
            sql += $" WHERE d.company_id = ANY(@CompanyIds)";
            parameters.Add("CompanyIds", filter.CompanyIds);
            whereAdded = true;
        }

        if (filter.DepartmentIds != null && filter.DepartmentIds.Any())
        {
            sql += whereAdded ? " AND " : " WHERE ";
            sql += $" w.department_id = ANY(@DepartmentIds)"; // Исправлено
            parameters.Add("DepartmentIds", filter.DepartmentIds);
        }

        try
        {
            var workers = await 
                _dbConnection.QueryAsync<Worker, ReadPassport, Department, Company, Worker>(
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

            if (workers is null || !workers.Any())
            {
                throw new WorkerNotFoundException("Пользователи не найдены. Попробуйте изменить фильтр.");
            }

            var result = new GetManyWorkerResponse()
            {
                Workers = workers.ToList()
            };

            return result;
        }
        catch (WorkerNotFoundException ex)
        {
            throw ;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Произошла ошибка при получении пользователей.", ex);
        }
    }
    
    /// <inheritdoc/> 
    public async Task UpdateWorkerAsync(
        UpdateWorkerRequest request, CancellationToken cancellationToken)
    {
        using var transaction = _dbConnection.BeginTransaction();

        try
        {
            await UpdatePassport(request.Id, request.Passport, 
                transaction, cancellationToken);
            
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
            
            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw new InvalidOperationException("Не удалось изменить сотрудника.");
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
    private async Task<int> CreatePassport(
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
    private async Task UpdatePassport(int workerId,
        WritePassport passport, IDbTransaction transaction,
        CancellationToken cancellationToken)
    {
        var passportId = 
            await GetWorkerPassportId(workerId, cancellationToken);
        
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
    private async Task DeletePassport(
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
    private async Task<int> GetWorkerPassportId(
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
}