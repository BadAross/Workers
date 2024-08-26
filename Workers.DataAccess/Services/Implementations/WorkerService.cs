using Workers.DataAccess.Dto.Requests;
using Workers.DataAccess.Dto.Responses;
using Workers.DataAccess.Services.Interfaces;

namespace Workers.DataAccess.Services.Implementations;

/// <summary>
/// Реализация работы с сотрудниками
/// </summary>
public sealed class WorkerService : IWorkerService
{
    /// <inheritdoc/> 
    public async Task<int> CreateWorkerAsync(CreateWorkerRequest request)
    {
        return 0;
    }
    
    /// <inheritdoc/> 
    public async Task DeleteWorkerAsync(int workerId)
    {
    }
    
    /// <inheritdoc/> 
    public async Task<GetManyWorkerResponse> GetManyWorkerAsync(GetManyWorkerRequest filter)
    {
        return new();
    }
}