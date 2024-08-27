using Shared.StateMachines;
using Users.Domain;

namespace Users.StateMachines;

public class UserRegistrationStateMachine : BaseStateMachine<UserRegistrationStatus, UserRegistrationStatusReason>
{
    public UserRegistrationStateMachine(UserRegistrationStatus initialState) : base(initialState)
    {
    }

    protected override void Configure()
    {
        StateMachine.Configure(UserRegistrationStatus.New)
            .Permit(UserRegistrationStatusReason.Created, UserRegistrationStatus.Pending);

        StateMachine.Configure(UserRegistrationStatus.Pending)
            .PermitReentry(UserRegistrationStatusReason.Invited)
            .PermitReentry(UserRegistrationStatusReason.Confirmed)
            .Permit(UserRegistrationStatusReason.Registered, UserRegistrationStatus.Finished)
            .Permit(UserRegistrationStatusReason.Rejected, UserRegistrationStatus.Finished);
    }
}