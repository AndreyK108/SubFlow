using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubFlow.Domain.Entities;
using SubFlow.Domain.Enums;

namespace SubFlow.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OwnerUserId).IsRequired();
        builder.Property(x => x.ServiceId).IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Currency)
            .HasMaxLength(8)
            .IsRequired();

        builder.Property(x => x.BillingPeriod)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.BillingPeriodDays);

        // DateOnly -> DateTime converter (EF Core needs it explicitly for SQL Server)
        builder.Property(x => x.NextChargeDate)
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v))
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.StartDate)
            .HasConversion(
                v => v.ToDateTime(TimeOnly.MinValue),
                v => DateOnly.FromDateTime(v))
            .HasColumnType("date")
            .IsRequired();

        builder.Property(x => x.EndDate)
            .HasConversion(
                v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null)
            .HasColumnType("date");

        builder.Property(x => x.TrialEndDate)
            .HasConversion(
                v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null)
            .HasColumnType("date");

        builder.Property(x => x.AutoRenew).IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => new { x.OwnerUserId, x.OrganizationId });
        builder.HasIndex(x => x.ServiceId);

        // Пока без FK-навигаций (мы не заводили navigation properties в Domain)
        // Позже можно добавить: builder.HasOne<Service>().WithMany()...
    }
}
