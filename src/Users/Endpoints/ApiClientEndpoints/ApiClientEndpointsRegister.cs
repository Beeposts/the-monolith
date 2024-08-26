using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Users.UseCases.ApiClients;
using Users.UseCases.ApiClients.CreateApiClient;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Users.Endpoints.ApiClientEndpoints;

public static class ApiClientEndpointsRegister
{
    public const string Base = "api_clients";
    
    public static void MapApiClientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup(Base)
            .WithTags("Api Client")
            .RequireAuthorization();
        
        group.MapPost("", CreateAsync);
    }
    
    private static async Task<IResult> CreateAsync(
        [AsParameters] CreateApiClientRequest request, 
        [FromServices] IMediator mediator)
    {
        var result = await mediator.Send(request);
        return result.ToMinimalApiResult();
    }
}