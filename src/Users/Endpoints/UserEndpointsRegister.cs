using Ardalis.Result.AspNetCore;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Users.UseCases.Users.RegisterUsers;

namespace Users.Endpoints;

public static class UserEndpointsRegister
{
    const string Base = "users";
    
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup(Base)
            .WithTags("Users");
        
        group.MapPost("", CreateUserRegistration);
        group.MapPost("/register", ConfirmUserRegistration);
        group.MapPost("/register/validate", ValidateUserInvitationCode);
    }
    
    static async Task<IResult> CreateUserRegistration(
        [FromBody] CreateUserRegistrationRequest request, 
        [FromServices] ISender mediator)
    {
        var result = await mediator.Send(request);
        return result.ToMinimalApiResult();
    }

    static async Task<IResult> ConfirmUserRegistration(
        [FromBody] ConfirmUserRegistrationRequest request,
        [FromServices] ISender mediator
        )
    {
        var result = await mediator.Send(request);
        return result.ToMinimalApiResult();
    }
    
    static async Task<IResult> ValidateUserInvitationCode(
        [AsParameters] ValidateUserInvitationCodeRequest request,
        [FromServices] ISender mediator
        )
    {
        var result = await mediator.Send(request);
        return result.ToMinimalApiResult();
    }
}