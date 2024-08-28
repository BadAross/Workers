using Workers.DataAccess.Dto.Requests;
using Workers.DataAccess.Dto.Responses;

namespace Workers.DataAccess.Services.Interfaces;

/// <summary>
/// Интерфейс работы с сотрудниками
/// </summary>
public interface IWorkerService
{
    /// <summary>
    /// Метод создания сотрудника
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Идентификатор созданного сотрудника</returns>
    Task<int> CreateWorkerAsync(
        CreateWorkerRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Метод удаления сотрудника
    /// </summary>
    /// <param name="workerId">Идентификатор сотрудника</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task DeleteWorkerAsync(
        int workerId, CancellationToken cancellationToken);

    /// <summary>
    /// Метод получения сотрудника по фильтрам
    /// </summary>
    /// <param name="filter">Фильтр</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список сотрудников</returns>
    Task<GetManyWorkerResponse> GetManyWorkerAsync(
        GetManyWorkerRequest filter, CancellationToken cancellationToken);

    /// <summary>
    /// Метод получения сотрудника по фильтрам
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken">Токен отмены</param>
    Task UpdateWorkerAsync(
        UpdateWorkerRequest request, CancellationToken cancellationToken);
}