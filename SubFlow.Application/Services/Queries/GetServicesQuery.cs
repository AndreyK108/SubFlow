using SubFlow.Application.Abstractions;
using SubFlow.Application.Services.Models;

namespace SubFlow.Application.Services.Queries;

public sealed class GetServicesQuery
{
    private readonly IServiceRepository _repo;

    public GetServicesQuery(IServiceRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<ServiceListItem>> ExecuteAsync(CancellationToken ct)
    {
        var items = await _repo.GetAllAsync(ct);

        return items
            .OrderBy(x => x.Name)
            .Select(x => new ServiceListItem(x.Id, x.Name))
            .ToList();
    }
}
