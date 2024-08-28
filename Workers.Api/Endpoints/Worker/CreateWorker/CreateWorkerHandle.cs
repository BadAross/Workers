using FastEndpoints;
using Workers.DataAccess.Dto.Requests;
using Workers.DataAccess.Services.Interfaces;

namespace Workers.Api.Endpoints.Worker.CreateWorker;

/// <summary>
/// Метод создания сотрудника
/// </summary>
/// <param name="workerService"></param>
public sealed class CreateWorkerHandle(IWorkerService workerService) 
    : Endpoint<CreateWorkerRequest, int>
{
    private readonly IWorkerService _workerService = workerService;

    /// <inheritdoc />
    public override void Configure()
    {
        Put("");
        Group<WorkerGroup>();
        Summary(sum =>
        {
            sum.Summary = "Создание сотрудника";
        });
        AllowAnonymous();
    }
    
    /// <summary>
    /// Создание сотрудника
    /// </summary>
    /// <param name="req">запрос</param>
    /// <param name="ct">токен отмены</param>
    public override async Task HandleAsync(
        CreateWorkerRequest req, CancellationToken ct)
    {
        var result = await _workerService.CreateWorkerAsync(req, ct);
        await SendAsync(result, cancellation: ct);
    }
}