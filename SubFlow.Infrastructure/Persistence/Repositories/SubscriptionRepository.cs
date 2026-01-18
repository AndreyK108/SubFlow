using Microsoft.EntityFrameworkCore;
using SubFlow.Application.Abstractions;
using SubFlow.Domain.Entities;
using SubFlow.Infrastructure.Persistence;

namespace SubFlow.Infrastructure.Persistence.Repositories;

public sealed class SubscriptionRepository : ISubscriptionRepository
{
    private readonly SubFlowDbContext _db;

    public SubscriptionRepository(SubFlowDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Subscription>> GetByOwnerAsync(Guid ownerUserId, CancellationToken ct)
    {
        return await _db.Subscriptions
            .AsNoTracking()
            .Where(x => x.OwnerUserId == ownerUserId)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Subscription subscription, CancellationToken ct)
    {
        await _db.Subscriptions.AddAsync(subscription, ct);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _db.SaveChangesAsync(ct);
    }
}
