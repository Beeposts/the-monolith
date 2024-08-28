using FluentValidation;

namespace Users.UseCases.Users.RegisterUsers;

public class ValidateUserInvitationCodeValidation : AbstractValidator<ValidateUserInvitationCodeRequest>
{
    public ValidateUserInvitationCodeValidation()
    {
        RuleFor(x => x.InvitationCode)
            .NotEmpty();
    }
}