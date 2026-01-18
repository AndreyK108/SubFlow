using SubFlow.Domain.Entities;

namespace SubFlow.Application.Abstractions;

public interface IServiceRepository
{
    Task<IReadOnlyList<Service>> GetAllAsync(CancellationToken ct);
    Task AddAsync(Service service, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
