using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain;

namespace Users.Database.Configuration;

public class ApiClientConfiguration : IEntityTypeConfiguration<ApiClient>
{
    public void Configure(EntityTypeBuilder<ApiClient> builder)
    {
        builder.Property(p => p.ClientId)
            .HasMaxLength(ApiClientSpecification.ClientIdMaxLength);

        builder.Property(p => p.ClientName)
            .HasMaxLength(ApiClientSpecification.ClientNameMaxLength);
    }
}