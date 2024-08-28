using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;
using Users.Database;
using Users.Domain;

namespace Users.UseCases.ApiClients;

public record GetCurrentApiClientRequest : IRequest<Result<GetCurrentApiClientResponse>>
{
    public static GetCurrentApiClientRequest Instance => new();
}

public record GetCurrentApiClientResponse(int Id, string IdentityClientId, string Name, int TenantId);

public class GetCurrentApiClientHandler : IRequestHandler<GetCurrentApiClientRequest, Result<GetCurrentApiClientResponse>>
{
    readonly UserDbContext _dbContext;
    readonly IUserSession _userSession;

    public GetCurrentApiClientHandler(UserDbContext dbContext, IUserSession userSession)
    {
        _dbContext = dbContext;
        _userSession = userSession;
    }

    public async ValueTask<Result<GetCurrentApiClientResponse>> Handle(GetCurrentApiClientRequest request, CancellationToken cancellationToken)
    {
        var apiClient = await _dbContext.ApiClient
            .Where(x => x.ClientId == _userSession.IdentityClientId)
            .Select(x => new GetCurrentApiClientResponse(x.Id, x.ClientId, x.ClientName, x.TenantId))
            .FirstOrDefaultAsync(cancellationToken);

        if (apiClient is null)
            return Result.NotFound();

        return apiClient;
    }
}