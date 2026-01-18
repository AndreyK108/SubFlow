using SubFlow.Application.Abstractions;
using SubFlow.Domain.Entities;

namespace SubFlow.Application.Services.Commands;

public sealed class CreateServiceCommand
{
    private readonly IServiceRepository _repo;

    public CreateServiceCommand(IServiceRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> ExecuteAsync(string name, CancellationToken ct)
    {
        var service = new Service(name);

        await _repo.AddAsync(service, ct);
        await _repo.SaveChangesAsync(ct);

        return service.Id;
    }
}
