using FluentValidation;
using Users.Domain;

namespace Users.UseCases.ApiClients.CreateApiClient;

public class CreateApiClientRequestValidator : AbstractValidator<CreateApiClientRequest>
{
    public CreateApiClientRequestValidator()
    {
        RuleFor(x => x.ApiClient)
            .NotNull();

        RuleFor(x => x.ApiClient!.ClientName)
            .NotEmpty()
            .MaximumLength(ApiClientSpecification.ClientNameMaxLength)
            .When(x => x.ApiClient is not null);

    }
}