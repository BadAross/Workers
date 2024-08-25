namespace Workers.DataAccess.Dto.Requests;

/// <summary>
/// Dto запроса на создание сотрудника
/// </summary>
public sealed class CreateWorkerRequest
{
    /// <summary>
    /// Имя сотрудника
    /// </summary>
    public string Name { set; get; } = null!;
    
    /// <summary>
    /// Фамилия сотрудника
    /// </summary>
    public string Surname { set; get; } = null!;
    
    /// <summary>
    /// Номер телефона
    /// </summary>
    public string? Phone { set; get; }
    
    /// <summary>
    /// Идентфикатор типа паспорта
    /// </summary>
    public int PassportTypeId { set; get; }
    
    /// <summary>
    /// Номер паспорта
    /// </summary>
    public string PassportNumber { set; get; } = null!;
    
    /// <summary>
    /// Идентификатор отдела
    /// </summary>
    public int DepartmentId { set; get; }
}