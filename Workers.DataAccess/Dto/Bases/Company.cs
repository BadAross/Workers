namespace Workers.DataAccess.Dto.Bases;

/// <summary>
/// Dto модели компании
/// </summary>
public sealed class Company
{
    /// <summary>
    /// идентификатор
    /// </summary>
    public int Id { set; get; }

    /// <summary>
    /// Название 
    /// </summary>
    public string Name { set; get; } = null!;
}