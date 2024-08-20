namespace Shared.Abstractions;

public interface ISessionService
{
    string? IdentityUserId { get; }
    string? Email { get;  }
    int? TenantId { get; }
    int UserId { get; }
    void SetTenantId(int tenantId); 
}