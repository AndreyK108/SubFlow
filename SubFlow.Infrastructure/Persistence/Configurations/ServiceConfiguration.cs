using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubFlow.Domain.Entities;

namespace SubFlow.Infrastructure.Persistence.Configurations;

public sealed class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.WebsiteUrl)
            .HasMaxLength(500);

        builder.Property(x => x.LogoUrl)
            .HasMaxLength(500);
    }
}
