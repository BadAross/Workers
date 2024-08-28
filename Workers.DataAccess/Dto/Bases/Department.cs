using Workers.DataAccess.Extensions.DapperAttributeMapper;

namespace Workers.DataAccess.Dto.Bases;

/// <summary>
/// Dto модели отдела
/// </summary>
public sealed class Department
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Column("department_id")] 
    public int Id { init; get; }
    
    /// <summary>
    /// Название отдела
    /// </summary>
    public string Name { init; get; } = null!;

    /// <summary>
    /// Номер телефона
    /// </summary>
    public string? Phone { init; get; }

    /// <summary>
    /// Компания
    /// </summary>
    public Company Company { set; get; } = null!;
}