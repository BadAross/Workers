using Workers.DataAccess.Extensions.DapperAttributeMapper;

namespace Workers.DataAccess.Dto.Bases;

/// <summary>
/// Dto модели сотрудника
/// </summary>
public sealed class Worker
{
    /// <summary>
    /// Идетнификатор сотрудника
    /// </summary>
    [Column("worker_id")]
    public int Id { init; get; }

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
    /// Паспортные данные
    /// </summary>
    public ReadPassport Passport { set; get; } = null!;
    
    /// <summary>
    /// Отдел
    /// </summary>
    public Department Department { set; get; } = null!;
}