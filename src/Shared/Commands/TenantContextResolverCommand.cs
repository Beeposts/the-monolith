using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Commands;

public interface ITenantContextResolverCommand : IMessage
{
}

public abstract record TenantContextResolverCommand<TResponse> : IRequest<Result<TResponse>>, ITenantContextResolverCommand 
{
    [FromHeader(Name = AppConsts.TenantHeader)]
    public string TenantSlug { get; init; } = string.Empty;
}