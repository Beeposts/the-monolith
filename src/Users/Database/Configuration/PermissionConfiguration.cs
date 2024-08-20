using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain;

namespace Users.Database.Configuration;

internal class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(PermissionSpecification.NameMaxLength)
            .IsRequired();

        builder
            .HasIndex(p => new { p.Name, p.UserId, p.RoleId })
            .IsUnique();
    }
}