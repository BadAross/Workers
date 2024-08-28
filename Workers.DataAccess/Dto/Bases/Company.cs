using Workers.DataAccess.Extensions.DapperAttributeMapper;

namespace Workers.DataAccess.Dto.Bases;

/// <summary>
/// Dto модели компании
/// </summary>
public sealed class Company
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    [Column("company_id")] 
    public int Id { init; get; }

    /// <summary>
    /// Название 
    /// </summary>
    public string Name { init; get; } = null!;
}