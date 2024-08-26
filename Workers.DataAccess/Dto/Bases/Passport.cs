namespace Workers.DataAccess.Dto.Bases;

/// <summary>
/// Dto модели паспорта
/// </summary>
public sealed class Passport
{
    /// <summary>
    /// Тип паспорта
    /// </summary>
    public string Type { set; get; } = null!;
    
    /// <summary>
    /// Номер пасспорта
    /// </summary>
    public string Number { set; get; } = null!;
}