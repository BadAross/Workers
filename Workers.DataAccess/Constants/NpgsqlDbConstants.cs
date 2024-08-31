namespace Workers.DataAccess.Constants;

/// <summary>
/// Константы базы postgres
/// </summary>
internal static class NpgsqlDbConstants
{
    /// <summary>
    /// Название схемы
    /// </summary>
    public const string SchemeName = "base";
    
    /// <summary>
    /// Название таблицы сотрудников
    /// </summary>
    public const string WorkerTableName = "worker";
    
    /// <summary>
    /// Название таблицы отделов
    /// </summary>
    public const string DepartmentTableName = "department";
    
    /// <summary>
    /// Название таблицы компаний
    /// </summary>
    public const string CompanyTableName = "company";
    
    /// <summary>
    /// Название таблицы паспортных данных
    /// </summary>
    public const string PassportTableName = "passport";
    
    /// <summary>
    /// Название таблицы типов паспортов
    /// </summary>
    public const string PassportTypeTableName = "passport_type";
}