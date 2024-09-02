using Ardalis.Result.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Users.UseCases.Tenants;

namespace Users.Endpoints;

public static class TenantEndpointsRegister
{
    const string Base = "tenants";

    public static void MapTenantsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup(Base)
            .WithTags("Tenants");
        
        group.MapGet("", GetCurrentUserTenants);
    }
    
    static async Task<IResult> GetCurrentUserTenants(
        [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new GetCurrentUserTenantsRequest());
        return result.ToMinimalApiResult();
    }
}