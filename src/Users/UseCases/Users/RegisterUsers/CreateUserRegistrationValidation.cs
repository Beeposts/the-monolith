using FluentValidation;
using Users.Domain;

namespace Users.UseCases.Users.RegisterUsers;

public class CreateUserRegistrationValidation : AbstractValidator<CreateUserRegistrationRequest>
{
    public CreateUserRegistrationValidation()
    {

        RuleFor(x => x.Email)
            .NotEmpty()
            .MaximumLength(UserSpecification.EmailMaxLength)
            .EmailAddress();

        RuleFor(x => x.Name)
            .NotEmpty();
    }
}