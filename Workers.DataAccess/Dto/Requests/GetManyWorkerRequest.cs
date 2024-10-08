﻿namespace Workers.DataAccess.Dto.Requests;

/// <summary>
/// Модель запроса на получение списка сотрудников
/// </summary>
public sealed class GetManyWorkerRequest
{
    /// <summary>
    /// Фильтр по идентификаторам компаний
    /// </summary>
    public List<int>? CompanyIds { init; get; }
    
    /// <summary>
    /// Фильтр по идентификаторам отделов компании
    /// </summary>
    public List<int>? DepartmentIds { init; get; }
}