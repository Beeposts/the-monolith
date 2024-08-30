using FluentValidation;

namespace Users.UseCases.Users.RegisterUsers;

public class RegisterUserValidation : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidation()
    {
        RuleFor(x => x.InvitationCode)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password);
    }
}