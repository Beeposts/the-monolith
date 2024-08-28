using FluentValidation;
using Users.Domain;

namespace Users.UseCases.ApiClients.CreateApiClient;

public class CreateApiClientRequestValidator : AbstractValidator<CreateApiClientRequest>
{
    public CreateApiClientRequestValidator()
    {
        RuleFor(x => x.Data)
            .NotNull();

        RuleFor(x => x.Data!.ClientName)
            .NotEmpty()
            .MaximumLength(ApiClientSpecification.ClientNameMaxLength)
            .When(x => x.Data is not null);

    }
}