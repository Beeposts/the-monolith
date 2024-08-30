using Shared.StateMachines;
using Users.Domain;

namespace Users.StateMachines;

public class UserRegistrationStateMachine : BaseStateMachine<UserInviteStatus, UserInviteStatusReason>
{
    public UserRegistrationStateMachine(Func<UserInviteStatus> stateAccessor, Action<UserInviteStatus> stateMutator) : base(stateAccessor, stateMutator)
    {
    }

    protected override void Configure()
    {

        StateMachine.Configure(UserInviteStatus.Pending)
            .PermitReentry(UserInviteStatusReason.Invited)
            .Permit(UserInviteStatusReason.Registered, UserInviteStatus.Finished)
            .Permit(UserInviteStatusReason.Rejected, UserInviteStatus.Finished)
            .Permit(UserInviteStatusReason.Expired, UserInviteStatus.Finished);
    }
}