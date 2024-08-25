using FastEndpoints;

namespace Workers.Api.Endpoints.Worker;

internal sealed class WorkerGroup : Group
{
    public WorkerGroup()
    {
        Configure("worker", ep =>
        {
            ep.Description(x => x
                .WithTags("worker"));
        });
    }
}