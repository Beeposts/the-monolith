using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain;

namespace Users.Database.Configuration;

internal class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(TenantSpecification.NameMaxLength)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasMaxLength(TenantSpecification.SlugMaxLength)
            .IsRequired();

        builder.HasIndex(x => new { x.Slug })
            .IncludeProperties(x => new { x.Name, x.Id })
            .IsUnique();
    }
}