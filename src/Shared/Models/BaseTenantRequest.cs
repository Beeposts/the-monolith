using Microsoft.AspNetCore.Mvc;

namespace Shared.Models;

public abstract record BaseTenantRequest
{
    [FromHeader(Name = AppConsts.TenantHeader)]
    public string TenantSlug { get; init; } = string.Empty;
}