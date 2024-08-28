namespace Shared.Domains.Abstractions;

public interface IAuditableEntity<TId> : ITenantEntity<TId>
{
    DateTime? CreatedAt { get; set; }
    int? CreatedBy { get; set; }
    int? CreatedByClientId { get; set; }
    DateTime? UpdatedAt { get; set; }
    int? UpdatedBy { get; set; }
    int? UpdatedByClientId { get; set; }
}

public interface IAuditableEntity : IAuditableEntity<int>, ITenantEntity
{
}