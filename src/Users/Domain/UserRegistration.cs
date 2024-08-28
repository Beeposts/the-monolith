using Shared.Domains;

namespace Users.Domain;

public record UserRegistration : Entity
{
    public required string InviteCode { get; init; }
    public required string Email { get; init; }
    public bool IsInvitedByUser => InvitedByUserId.HasValue;
    public bool IsInvitedByTenant => InvitedByTenantId.HasValue;
    public int? InvitedByUserId { get; set; }
    public int? InvitedByTenantId { get; set; }
    public UserRegistrationStatus Status { get; set; } = UserRegistrationStatus.New;
    public UserRegistrationStatusReason StatusReason { get; set; } = UserRegistrationStatusReason.Created;
    public DateTime ExpiresAt { get; set; }
}