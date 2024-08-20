namespace Shared.Domains.Abstractions;

public interface ITenantEntity<TId> : IEntity<TId>
{
    int TenantId { get; set; }
}

public interface ITenantEntity : ITenantEntity<int>, IEntity
{
}