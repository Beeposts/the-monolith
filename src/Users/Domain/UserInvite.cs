using Shared.Domains;

namespace Users.Domain;

public record UserInvite : Entity
{
    public required string InviteCode { get; init; }
    public required string Email { get; init; }
    public bool IsInvitedByUser => InvitedByUserId.HasValue;
    public bool IsInvitedByTenant => InvitedByTenantId.HasValue;
    public int? InvitedByUserId { get; set; }
    public int? InvitedByTenantId { get; set; }
    public UserInviteStatus Status { get; set; } = UserInviteStatus.Pending;
    public UserInviteStatusReason StatusReason { get; set; } = UserInviteStatusReason.Created;
    public DateTime ExpiresAt { get; set; }
}