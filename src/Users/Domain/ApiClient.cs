using Shared.Domains;

namespace Users.Domain;

public record ApiClient : AuditableEntity
{
    public required string ClientId { get; init; }
    public required string ClientName { get; init; }
}

internal static class ApiClientSpecification
{
    public const int ClientIdMaxLength = 150;
    public const int ClientNameMaxLength = 150;
}