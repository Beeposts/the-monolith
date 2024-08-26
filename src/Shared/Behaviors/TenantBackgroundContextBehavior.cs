using Ardalis.Result;
using Mediator;
using Shared.Abstractions;
using Shared.Commands;

namespace Shared.Behaviors;

public class TenantBackgroundContextBehavior<TRequest, TResponse> : PipelineBehaviorErrorHandler, IPipelineBehavior<TRequest, TResponse> 
    where TRequest : TenantBackgroundContextCommand<TResponse>
    where TResponse : IResult
{
    private readonly ITenantSession _tenantSession;
    private readonly ITenantResolver _tenantResolver;

    public TenantBackgroundContextBehavior(ITenantSession tenantSession, ITenantResolver tenantResolver)
    {
        _tenantSession = tenantSession;
        _tenantResolver = tenantResolver;
    }

    public async ValueTask<TResponse> Handle(TRequest message, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next)
    {
        await _tenantResolver.Resolve();
        if(_tenantSession.TenantId is not null)
        {
            return await next(message, cancellationToken);
        }
        
        if (message.TenantId is null)
        {
            return CreateFailureResult<TResponse>(new[] { new ValidationError("TenantId is required") });
        }

        var resolveResult = await _tenantResolver.Resolve(message.TenantId.Value);
        if (resolveResult.IsSuccess) return await next(message, cancellationToken);
        
        var errors = resolveResult.Errors.Select(x => new ValidationError(x)).ToList();
        errors.AddRange(resolveResult.ValidationErrors.ToList());
        return CreateFailureResult<TResponse>(errors.ToArray());
    }
}