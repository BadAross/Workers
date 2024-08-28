namespace Workers.DataAccess.Dto.Bases;

/// <summary>
/// Dto модели записи паспорта 
/// </summary>
public sealed class WritePassport
{
    /// <summary>
    /// Идентификатор типа паспорта
    /// </summary>
    public int TypeId { init; get; }
    
    /// <summary>
    /// Номер пасспорта
    /// </summary>
    public string Number { init; get; } = null!;
}