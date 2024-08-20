using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain;

namespace Users.Database.Configuration;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(RoleSpecification.NameMaxLength)
            .IsRequired();

        builder.HasMany(p => p.Permissions)
            .WithOne(p => p.Role)
            .OnDelete(DeleteBehavior.Cascade);
    }
}