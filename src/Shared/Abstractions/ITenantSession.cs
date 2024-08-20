namespace Shared.Abstractions;

public interface ITenantSession
{
    int? TenantId { get; }
    string? Slug { get;  }
    void SetTenantId(int tenantId);
    void SetSlug(string slug);
}