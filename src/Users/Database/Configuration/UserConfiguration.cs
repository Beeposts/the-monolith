using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared;
using Users.Domain;

namespace Users.Database.Configuration;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .Property(x => x.IdentityId)
            .HasMaxLength(UserSpecification.IdentityIdMaxLength);

        builder.Property(x => x.Email)
            .HasMaxLength(AppConsts.EmailMaxLength);
        
        builder.HasIndex(x => x.IdentityId)
            .IsUnique();

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.HasMany(p => p.Permissions)
            .WithOne(p => p.User)
            .OnDelete(DeleteBehavior.Cascade);
    }
}