using Ardalis.Result;
using Mediator;

namespace Shared.Commands;

public abstract class TenantBackgroundContextCommand<TResponse> : IRequest<Result<TResponse>>
{
    public int? TenantId { get; init; }
}