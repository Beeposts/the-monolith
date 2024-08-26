using Ardalis.Result;
using FluentValidation;
using Mediator;

namespace Shared.Behaviors;

public class ValidationPipelineBehavior<TRequest, TResponse>
    : PipelineBehaviorErrorHandler, IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async ValueTask<TResponse> Handle(TRequest message, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next)
    {
        if (!_validators.Any())
            return await next(message,cancellationToken);

        var errors = _validators
            .Select(v => v.Validate(message))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .Select(x => new ValidationError(x.ErrorMessage))
            .Distinct()
            .ToArray();

        if (errors.Length != 0)
            return CreateFailureResult<TResponse>(errors);

        return await next(message,cancellationToken);
    }
}