using Shared.Domains.Abstractions;

namespace Shared.Domains;

public abstract record AuditableEntity<TId> : TenantEntity<TId>, IAuditableEntity<TId>
{
    public DateTime? CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
}

public abstract record AuditableEntity : AuditableEntity<int>, IAuditableEntity
{
}