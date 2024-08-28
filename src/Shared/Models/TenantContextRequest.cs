using Microsoft.AspNetCore.Mvc;
using Shared.Commands;

namespace Shared.Models;

public record TenantContextRequest<TRequest, TResponse> : TenantContextResolverCommand<TResponse>
    where TRequest : class
{
    [FromBody] public TRequest? Data { get; set; }
}

public record TenantContextRequest<TRequest> : TenantContextResolverCommand
{
    [FromBody] public TRequest? Data { get; set; }
}