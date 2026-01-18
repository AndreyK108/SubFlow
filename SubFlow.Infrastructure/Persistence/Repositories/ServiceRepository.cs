using Microsoft.EntityFrameworkCore;
using SubFlow.Application.Abstractions;
using SubFlow.Domain.Entities;
using SubFlow.Infrastructure.Persistence;

namespace SubFlow.Infrastructure.Persistence.Repositories;

public sealed class ServiceRepository : IServiceRepository
{
    private readonly SubFlowDbContext _db;

    public ServiceRepository(SubFlowDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Service>> GetAllAsync(CancellationToken ct)
    {
        return await _db.Services
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
    }

    public Task AddAsync(Service service, CancellationToken ct)
    {
        _db.Services.Add(service);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return _db.SaveChangesAsync(ct);
    }
}
