using Mediator;
using Microsoft.AspNetCore.Http;
using Shared;
using Shared.Abstractions;
using Users.UseCases.ApiClients;
using UsersContracts;

namespace Users.Middlewares;

public class UserSessionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext httpContext, IUserSession userSession, IMediator mediator)
    {
        if ((httpContext.User.Identity?.IsAuthenticated ?? false) && userSession.UserId is null)
        {
            var user = await mediator.Send(GetCurrentUserRequest.Instance);
            if(user.IsSuccess)
                userSession.SetUserId(user.Value.Id);
            
            var apiClient = await mediator.Send(GetCurrentApiClientRequest.Instance);
            if(apiClient.IsSuccess)
                userSession.SetClientId(apiClient.Value.Id);
        }
        await next(httpContext);    
    }

}