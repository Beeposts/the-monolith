using Shared.Domains;

namespace Users.Domain;

internal record Permission : TenantEntity<long>
{
    public required string Name { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }
    public int? RoleId { get; set; }
    public Role? Role { get; set; }
}

internal static class PermissionSpecification
{
    public const int NameMaxLength = 100;
}