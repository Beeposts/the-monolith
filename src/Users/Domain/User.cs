using Shared.Domains;

namespace Users.Domain;
public record User : Entity
{
    public required string IdentityId { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public List<Permission> Permissions { get; set; } = [];
    public List<Role> Roles { get; set; } = [];
    public List<Tenant> Tenants { get; set; } = [];
}

internal static class UserSpecification
{
    public const int IdentityIdMaxLength = 60;
}