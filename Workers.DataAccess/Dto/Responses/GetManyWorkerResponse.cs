using Workers.DataAccess.Dto.Bases;

namespace Workers.DataAccess.Dto.Responses;

/// <summary>
/// Модель ответа на получение списка сотрудников
/// </summary>
public class GetManyWorkerResponse
{
    /// <summary>
    /// Список сотрудников
    /// </summary>
    public List<Worker>? Workers { set; get; }
}