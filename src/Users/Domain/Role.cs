using Shared.Domains;

namespace Users.Domain;

public record Role : TenantEntity
{
    public required string Name { get; set; }
    public List<Permission> Permissions { get; set; } = [];
    public List<User> Users { get; set; } = [];
}

internal class RoleSpecification
{
    public const int NameMaxLength = 100;
}