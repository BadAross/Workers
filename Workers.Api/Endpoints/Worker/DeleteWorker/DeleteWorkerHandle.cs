using FastEndpoints;
using Workers.DataAccess.Services.Interfaces;

namespace Workers.Api.Endpoints.Worker.DeleteWorker;

/// <summary>
/// Метод удаления сотрудника
/// </summary>
public sealed class DeleteWorkerHandle(IWorkerService workerService) 
    : EndpointWithoutRequest 
{
    private readonly IWorkerService _workerService = workerService;

    /// <inheritdoc />
    public override void Configure()
    {
        Delete("{workerId}");
        Group<WorkerGroup>();
        Summary(sum =>
        {
            sum.Summary = "Удаление сотрудника";
        });
        AllowAnonymous();
    }
    
    /// <summary>
    /// Удаление сотрудника
    /// </summary>
    /// <param name="ct">токен отмены</param>
    public override async Task HandleAsync(
         CancellationToken ct)
    {
        var workerId = Route<int>("workerId");
        await _workerService.DeleteWorkerAsync(workerId, ct);
        await SendOkAsync(ct);
    }
}