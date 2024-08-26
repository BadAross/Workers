namespace Workers.DataAccess.Dto.Bases;

/// <summary>
/// Dto модели отдела
/// </summary>
public sealed class Department
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int Id { set; get; }
    
    /// <summary>
    /// Название отдела
    /// </summary>
    public string Name { set; get; } = null!;

    /// <summary>
    /// Номер телефона
    /// </summary>
    public string? Phone { set; get; }

    /// <summary>
    /// Компания
    /// </summary>
    public Company Company { set; get; } = null!;
}