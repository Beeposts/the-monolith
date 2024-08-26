namespace Shared.Domains.Abstractions;

public interface IAuditableEntity<TId> : ITenantEntity<TId>
{
    DateTime? CreatedAt { get; set; }
    int? CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    int? UpdatedBy { get; set; }
}

public interface IAuditableEntity : IAuditableEntity<int>, ITenantEntity
{
}