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
    /// Идентификатор типа паспорта
    /// </summary>
    public int PassportTypeId { set; get; }
    
    /// <summary>
    /// Номер паспорта
    /// </summary>
    public string PassportNumber { set; get; } = null!;
    
    /// <summary>
    /// Идентификатор отдлеа
    /// </summary>
    public int DepartmentId { set; get; }

    /// <summary>
    /// Метод установки идентификатор сотрудника
    /// </summary>
    /// <param name="workerId">Идентификатор сотрудника</param>
    public void SetId(int workerId) => Id = workerId;
}