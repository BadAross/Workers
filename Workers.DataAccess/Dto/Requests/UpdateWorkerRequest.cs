using Workers.DataAccess.Dto.Bases;

namespace Workers.DataAccess.Dto.Requests;

/// <summary>
/// Модель запроса на обновление данных сотрудника
/// </summary>
public sealed class UpdateWorkerRequest
{
    /// <summary>
    /// Идентификатор сотрудника
    /// </summary>
    public int Id { private set; get; }
    
    /// <summary>
    /// Имя сотрудника
    /// </summary>
    public string Name { init; get; } = null!;
    
    /// <summary>
    /// Фамилия сотрудника
    /// </summary>
    public string Surname { init; get; } = null!;
    
    /// <summary>
    /// Номер телефона
    /// </summary>
    public string? Phone { init; get; }
    
    /// <summary>
    /// Данные паспорта
    /// </summary>
    public WritePassport Passport { init; get; } = null!;
    
    /// <summary>
    /// Идентификатор отдлеа
    /// </summary>
    public int DepartmentId { init; get; }

    /// <summary>
    /// Метод установки идентификатор сотрудника
    /// </summary>
    /// <param name="workerId">Идентификатор сотрудника</param>
    public void SetId(int workerId) => Id = workerId;
}