using Workers.DataAccess.Dto.Requests;
using Workers.DataAccess.Dto.Responses;

namespace Workers.DataAccess.Repositories.Interface;

/// <summary>
/// Интерфейс репозитория сотрудника
/// </summary>
public interface IWorkerRepository
{
    /// <summary>
    /// Метод создания сотрудника
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Идентификатор сотрудника</returns>
    /// <exception cref="InvalidOperationException">Не удалось создать паспорт или сотрудника</exception>
    Task<int> CreateWorkerAsync(
        CreateWorkerRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Метод удавления сотрудника
    /// </summary>
    /// <param name="workerId">Идентификатор сотрудника</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <exception cref="InvalidOperationException">Не удалось удалить паспорт или сотрудника</exception>
    Task DeleteWorkerAsync(
        int workerId, CancellationToken cancellationToken);

    /// <summary>
    /// Метод получения списка сотрудников по фильтрам
    /// </summary>
    /// <param name="filter">Фильтр</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Список сотрудников</returns>
    /// <exception cref="InvalidOperationException">Список сотрудников пуст</exception>
    Task<GetManyWorkerResponse> GetManyWorkerAsync(
        GetManyWorkerRequest filter, CancellationToken cancellationToken);

    /// <summary>
    /// Метод изменения данных сотрудника
    /// </summary>
    /// <param name="request">Запрос</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <exception cref="InvalidOperationException">Не удалось изменить данные сотрудника</exception>
    Task UpdateWorkerAsync(
        UpdateWorkerRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Метод проверки наличия паспорта
    /// </summary>
    /// <param name="passportNumber">Номер паспорта</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>true - если паспорт уже есть, иначе - false</returns>
    Task<bool> IsThereThisPassportAsync(
        string passportNumber, CancellationToken cancellationToken);
}