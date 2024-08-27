using FluentValidation;

namespace Users.UseCases.Users.RegisterUsers;

public class ValidateUserInvitationCodeValidation : AbstractValidator<ValidateUserInvitationCodeRequest>
{
    public ValidateUserInvitationCodeValidation()
    {
        RuleFor(x => x.Data)
            .NotNull();
        
        RuleFor(x => x.Data!.InvitationCode)
            .NotEmpty()
            .When(x => x.Data is not null);
    }
}