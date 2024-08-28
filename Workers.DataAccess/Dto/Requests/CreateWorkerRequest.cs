using Workers.DataAccess.Dto.Bases;

namespace Workers.DataAccess.Dto.Requests;

/// <summary>
/// Dto запроса на создание сотрудника
/// </summary>
public sealed class CreateWorkerRequest
{
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
    /// Идентификатор отдела
    /// </summary>
    public int DepartmentId { init; get; }
}