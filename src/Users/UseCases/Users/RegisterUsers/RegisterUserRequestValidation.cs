using FluentValidation;
using Users.Domain;

namespace Users.UseCases.Users.RegisterUsers;

public class RegisterUserRequestValidation : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidation()
    {
        RuleFor(x => x.Data)
            .NotNull();
        
        RuleFor(x => x.Data!.FirstName)
            .NotEmpty()
            .MaximumLength(UserSpecification.FirstNameMaxLength)
            .When(x => x.Data is not null);
        
        RuleFor(x => x.Data!.LastName)
            .NotEmpty()
            .MaximumLength(UserSpecification.LastNameMaxLength)
            .When(x => x.Data is not null);
        
        RuleFor(x => x.Data!.Email)
            .NotEmpty()
            .MaximumLength(UserSpecification.EmailMaxLength)
            .EmailAddress()
            .When(x => x.Data is not null);
        
        RuleFor(x => x.Data!.Password)
            .NotEmpty()
            .MaximumLength(UserSpecification.PasswordMaxLength)
            .When(x => x.Data is not null);

        RuleFor(x => x.Data!.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Data!.Password, StringComparer.Ordinal)
            .When(x => x.Data is not null);
    }
}