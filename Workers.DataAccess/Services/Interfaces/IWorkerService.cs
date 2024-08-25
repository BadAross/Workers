using Workers.DataAccess.Dto.Requests;

namespace Workers.DataAccess.Services.Interfaces;

public interface IWorkerService
{ 
    Task<int> CreateWorkerAsync(CreateWorkerRequest request);
}