using Microsoft.AspNetCore.Mvc;
using Shared.Commands;

namespace Shared.Models;

public record TenantContextRequest<TRequest>
    where TRequest : class
{
    [FromHeader(Name = AppConsts.TenantHeader)]
    public string TenantSlug { get; init; } = string.Empty;
    [FromBody] public required TRequest Data { get; set; }
}