using FastEndpoints;
using Workers.DataAccess.Dto.Requests;
using Workers.DataAccess.Dto.Responses;
using Workers.DataAccess.Services.Interfaces;

namespace Workers.Api.Endpoints.Worker.GetManyWorker;

/// <summary>
/// Метод получения списка сотрудников по фильтрам
/// </summary>
/// <param name="workerService"></param>
public sealed class GetManyWorkerHandle(IWorkerService workerService)
    : Endpoint<GetManyWorkerRequest, GetManyWorkerResponse>
{
    private readonly IWorkerService _workerService = workerService;

    /// <inheritdoc />
    public override void Configure()
    {
        Put("");
        Group<WorkerGroup>();
        Summary(sum =>
        {
            sum.Summary = "Получения списка сотрудников по фильтрам";
        });
        AllowAnonymous();
    }
    
    /// <summary>
    /// Получения списка сотрудников по фильтрам
    /// </summary>
    /// <param name="req">запрос</param>
    /// <param name="ct">токен отмены</param>
    public override async Task HandleAsync(
        GetManyWorkerRequest req, CancellationToken ct)
    {
        var result = await _workerService.GetManyWorkerAsync(req);
        await SendAsync(result, cancellation: ct);
    }
}