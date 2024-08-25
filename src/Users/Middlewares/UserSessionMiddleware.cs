using Mediator;
using Microsoft.AspNetCore.Http;
using Shared;
using Shared.Abstractions;
using UsersContracts;

namespace Users.Middlewares;

public class UserSessionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, IUserSession userSession, IMediator mediator)
    {
        if ((httpContext.User.Identity?.IsAuthenticated ?? false) && userSession.UserId is null)
        {
            var user = await mediator.Send(GetCurrentUserRequest.Instance);
            userSession.SetUserId(user.Value.Id);
        }
        await next(httpContext);
    }

}