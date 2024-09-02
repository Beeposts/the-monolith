using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;
using Shared.Models;
using Users.Database;
using UsersContracts;

namespace Users.UseCases.Tenants;

public record GetCurrentUserTenantsRequest : IRequest<Result<TenantModel[]>>;

public class GetCurrentUserTenantsHandler : IRequestHandler<GetCurrentUserTenantsRequest, Result<TenantModel[]>>
{
    readonly UserDbContext _dbContext;
    readonly IUserSession _session;

    public GetCurrentUserTenantsHandler(UserDbContext dbContext, IUserSession session)
    {
        _dbContext = dbContext;
        _session = session;
    }
    public async ValueTask<Result<TenantModel[]>> Handle(GetCurrentUserTenantsRequest request, CancellationToken cancellationToken)
    {
        var tenants = await _dbContext
            .Tenant
            .Where(x => x.Users!.Any(u => u.Id == _session.UserId))
            .Select(x => new TenantModel(x.Id, x.Name, x.Slug))
            .ToListAsync(cancellationToken);

        return tenants.ToArray();
    }
}