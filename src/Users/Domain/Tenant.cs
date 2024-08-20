using Shared.Domains;

namespace Users.Domain;
internal record Tenant : Entity
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public List<User>? Users { get; set; }
}

internal static class TenantSpecification
{
    public const int NameMaxLength = 100;
    public const int SlugMaxLength = 130;
}