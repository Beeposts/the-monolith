using Microsoft.AspNetCore.Http;
using Shared;
using Shared.Abstractions;

namespace Users.Middlewares;

internal class TenantResolverMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, ITenantResolver tenantResolver)
    {
        if (httpContext.Request.Headers.Keys.Contains(AppConsts.TenantHeader, StringComparer.InvariantCultureIgnoreCase))
        {
            var result = await tenantResolver.Resolve();
            if (!result.IsSuccess)
            {
                httpContext.Response.StatusCode = 400;
                await httpContext.Response.WriteAsync("Invalid tenant");
                return;
            }
        }

        await next(httpContext);
    }

}