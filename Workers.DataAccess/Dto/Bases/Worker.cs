namespace Workers.DataAccess.Dto.Bases;

/// <summary>
/// Dto модели сотрудника
/// </summary>
public sealed class Worker
{
    /// <summary>
    /// Идетнификатор сотрудника
    /// </summary>
    public int Id { set; get; }

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
    /// Паспортные данные
    /// </summary>
    public Passport Passport { set; get; } = null!;
    
    /// <summary>
    /// Отдел
    /// </summary>
    public Department Department { set; get; } = null!;
}