using SubFlow.Domain.Entities;

namespace SubFlow.Application.Abstractions;

public interface ISubscriptionRepository
{
    Task<IReadOnlyList<Subscription>> GetByOwnerAsync(Guid ownerUserId, CancellationToken ct);
    Task AddAsync(Subscription subscription, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
