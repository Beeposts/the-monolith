using FluentValidation;
using Users.Domain;

namespace Users.UseCases.ApiClients.CreateApiClient;

public class CreateApiClientRequestValidator : AbstractValidator<CreateApiClientRequest>
{
    public CreateApiClientRequestValidator()
    {

        RuleFor(x => x.ClientName)
            .NotEmpty()
            .MaximumLength(ApiClientSpecification.ClientNameMaxLength);
    }
}