namespace Shared.Abstractions;

public interface ITenantSession
{
    int? TenantId { get; }
    Guid? Slug { get;  }
    void SetTenantId(int tenantId);
    void SetSlug(Guid slug);
}