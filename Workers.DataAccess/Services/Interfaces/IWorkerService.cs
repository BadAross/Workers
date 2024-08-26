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
    /// <param name="request">запрос</param>
    /// <returns>Идентификатор созданного сотрудника</returns>
    Task<int> CreateWorkerAsync(CreateWorkerRequest request);
    
    /// <summary>
    /// Метод удаления сотрудника
    /// </summary>
    /// <param name="workerId">Идентификатор сотрудника</param>
    Task DeleteWorkerAsync(int workerId);
    
    /// <summary>
    /// Метод получения сотрудника по фильтрам
    /// </summary>
    /// <param name="filter">Фильтр</param>
    /// <returns>Список сотрудников</returns>
    Task<GetManyWorkerResponse> GetManyWorkerAsync(GetManyWorkerRequest filter);
}