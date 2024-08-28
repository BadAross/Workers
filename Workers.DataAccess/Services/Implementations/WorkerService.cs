using Workers.DataAccess.Dto.Requests;
using Workers.DataAccess.Dto.Responses;
using Workers.DataAccess.Repositories.Interface;
using Workers.DataAccess.Services.Interfaces;

namespace Workers.DataAccess.Services.Implementations;

/// <summary>
/// Реализация работы с сотрудниками
/// </summary>
public sealed class WorkerService(IWorkerRepository workerRepository) : IWorkerService
{
    private readonly IWorkerRepository _workerRepository = workerRepository;
    
    /// <inheritdoc/> 
    public async Task<int> CreateWorkerAsync(
        CreateWorkerRequest request, CancellationToken cancellationToken)
    {
        var isThereThisPassport = await _workerRepository
            .IsThereThisPassportAsync(request.Passport.Number, cancellationToken);
        if (isThereThisPassport)
        {
            throw new InvalidOperationException("Данный пользователь уже существует.");
        }

        var workerId = await _workerRepository
            .CreateWorkerAsync(request, cancellationToken);
        return workerId;
    }
    
    /// <inheritdoc/> 
    public async Task DeleteWorkerAsync(
        int workerId, CancellationToken cancellationToken)
    {
        await _workerRepository.DeleteWorkerAsync(workerId, cancellationToken);
    }
    
    /// <inheritdoc/> 
    public async Task<GetManyWorkerResponse> GetManyWorkerAsync(
        GetManyWorkerRequest filter, CancellationToken cancellationToken)
    {
        var result = 
           await _workerRepository.GetManyWorkerAsync(filter, cancellationToken);

        return result;
    }
    
    /// <inheritdoc/> 
    public async Task UpdateWorkerAsync(
        UpdateWorkerRequest request, CancellationToken cancellationToken)
    {
        await _workerRepository.UpdateWorkerAsync(request, cancellationToken);
    }
}