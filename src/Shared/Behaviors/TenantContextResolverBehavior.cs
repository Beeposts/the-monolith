using Ardalis.Result;
using Mediator;
using Shared.Abstractions;
using Shared.Commands;

namespace Shared.Behaviors;

public class TenantContextResolverBehavior<TRequest, TResponse> : PipelineBehaviorErrorHandler, IPipelineBehavior<TRequest, TResponse> 
    where TRequest : ITenantContextResolverCommand
    where TResponse : IResult
{
    private readonly ITenantSession _tenantSession;
    private readonly ITenantResolver _tenantResolver;

    public TenantContextResolverBehavior(ITenantSession tenantSession, ITenantResolver tenantResolver)
    {
        _tenantSession = tenantSession;
        _tenantResolver = tenantResolver;
    }

    public async ValueTask<TResponse> Handle(TRequest message, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next)
    {
        var resolveResult = await _tenantResolver.Resolve();
        if (resolveResult.IsSuccess) return await next(message, cancellationToken);
        
        var errors = resolveResult.Errors.Select(x => new ValidationError(x)).ToList();
        errors.AddRange(resolveResult.ValidationErrors.ToList());
        return CreateFailureResult<TResponse>(errors.ToArray());
    }
}