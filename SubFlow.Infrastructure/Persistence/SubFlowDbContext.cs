using Microsoft.EntityFrameworkCore;
using SubFlow.Domain.Entities;

namespace SubFlow.Infrastructure.Persistence;

public sealed class SubFlowDbContext : DbContext
{
    public SubFlowDbContext(DbContextOptions<SubFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Service> Services => Set<Service>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SubFlowDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
