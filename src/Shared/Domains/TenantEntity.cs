using Shared.Domains.Abstractions;

namespace Shared.Domains;

public abstract record TenantEntity<TId> : Entity<TId>, ITenantEntity<TId>
{
    public int TenantId { get; set; }
}

public abstract record TenantEntity : TenantEntity<int>, ITenantEntity
{
}