using Workers.DataAccess.Extensions.DapperAttributeMapper;
namespace Workers.DataAccess.Dto.Bases;

/// <summary>
/// Dto модели чтения паспорта
/// </summary>
public sealed class ReadPassport
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Column("passport_id")] 
    public int Id { init; get; }
    
    /// <summary>
    /// Тип паспорта
    /// </summary>
    [Column("type")] 
    public string Type { init; get; } = null!;
    
    /// <summary>
    /// Номер пасспорта
    /// </summary>
    [Column("number")] 
    public string Number { init; get; } = null!;
}