using FastEndpoints;
using Workers.DataAccess.Dto.Requests;
using Workers.DataAccess.Services.Interfaces;

namespace Workers.Api.Endpoints.Worker.CreateWorker;

public class CreateWorkerHandle
    : Endpoint<CreateWorkerRequest, int>
{
    private readonly IWorkerService _workerService;

    public CreateWorkerHandle(IWorkerService workerService)
    {
        _workerService = workerService;
    }
    
    /// <inheritdoc />
    public override void Configure()
    {
        Put("");
        Group<WorkerGroup>();
        Summary(sum =>
        {
            sum.Summary = "Создание сотрудника";
        });
    }
    
    /// <summary>
    /// Создание сотрудника
    /// </summary>
    /// <param name="req">запрос</param>
    /// <param name="ct">токен отмены</param>
    public override async Task HandleAsync(
        CreateWorkerRequest req, CancellationToken ct)
    {
        var result = await _workerService.CreateWorkerAsync(req);
        await SendAsync(result, cancellation: ct);
    }
}