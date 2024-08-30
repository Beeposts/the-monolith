using FluentValidation;
using Users.Domain;

namespace Users.UseCases.Users.RegisterUsers;

public class CreateUserInviteValidation : AbstractValidator<CreateUserInviteRequest>
{
    public CreateUserInviteValidation()
    {

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(UserSpecification.EmailMaxLength)
            .EmailAddress();
    }
}