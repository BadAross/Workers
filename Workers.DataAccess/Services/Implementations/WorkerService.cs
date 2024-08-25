using Workers.DataAccess.Dto.Requests;
using Workers.DataAccess.Services.Interfaces;

namespace Workers.DataAccess.Services.Implementations;

public sealed class WorkerService : IWorkerService
{
    public async Task<int> CreateWorkerAsync(CreateWorkerRequest request)
    {
        return 0;
    }
}