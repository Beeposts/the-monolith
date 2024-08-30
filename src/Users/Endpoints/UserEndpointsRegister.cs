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
        
        group.MapPost("/invite", CreateUserRegistration);
        group.MapPost("/invite/validate", ValidateUserInvitationCode);
        group.MapPost("/register", ConfirmUserRegistration);
    }
    
    static async Task<IResult> CreateUserRegistration(
        [FromBody] CreateUserInviteRequest request, 
        [FromServices] ISender mediator)
    {
        var result = await mediator.Send(request);
        return result.ToMinimalApiResult();
    }

    static async Task<IResult> ConfirmUserRegistration(
        [FromBody] RegisterUserRequest request,
        [FromServices] ISender mediator
        )
    {
        var result = await mediator.Send(request);
        return result.ToMinimalApiResult();
    }
    
    static async Task<IResult> ValidateUserInvitationCode(
        [FromBody] ValidateUserInvitationCodeRequest request,
        [FromServices] ISender mediator
        )
    {
        var result = await mediator.Send(request);
        return result.ToMinimalApiResult();
    }
}