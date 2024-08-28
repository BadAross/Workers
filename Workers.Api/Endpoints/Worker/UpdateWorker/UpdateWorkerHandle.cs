using FastEndpoints;
using Workers.DataAccess.Dto.Requests;
using Workers.DataAccess.Services.Interfaces;

namespace Workers.Api.Endpoints.Worker.UpdateWorker;

public sealed class UpdateWorkerHandle(IWorkerService workerService)
    : Endpoint<UpdateWorkerRequest>
{
    private readonly IWorkerService _workerService = workerService;

    /// <inheritdoc />
    public override void Configure()
    {
        Put("{workerId}");
        Group<WorkerGroup>();
        Summary(sum =>
        {
            sum.Summary = "Изменить данные сотрудника";
        });
        AllowAnonymous();
    }
    
    /// <summary>
    /// Создание сотрудника
    /// </summary>
    /// <param name="req">запрос</param>
    /// <param name="ct">токен отмены</param>
    public override async Task HandleAsync(
        UpdateWorkerRequest req, CancellationToken ct)
    {
        var workerId = Route<int>("workerId");
        req.SetId(workerId);
        await _workerService.UpdateWorkerAsync(req, ct);
        await SendOkAsync(ct);
    }
}