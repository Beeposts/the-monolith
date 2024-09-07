using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;
using Shared.Models;
using Users.Database;
using UsersContracts;

namespace Users.UseCases.Users;

public record GetUsersFromCurrentTenantRequest : PagedRequest<UserModel[]>;

public class GetUsersFromCurrentTenantHandler : IRequestHandler<GetUsersFromCurrentTenantRequest, PagedResult<UserModel[]>>
{
    readonly UserDbContext _dbContext;
    readonly ITenantSession _session;

    public GetUsersFromCurrentTenantHandler(UserDbContext dbContext,
        ITenantSession session)
    {
        _dbContext = dbContext;
        _session = session;
    }
    
    public async ValueTask<PagedResult<UserModel[]>> Handle(GetUsersFromCurrentTenantRequest request, CancellationToken cancellationToken)
    {
        if(_session.TenantId is null)
            return Result<UserModel[]>
                .Invalid([new ValidationError("Tenant required.")])
                .ToPagedResult(new PagedInfo(0, 0, 0, 0));

        var usersQuery = _dbContext.User
            .Where(x => x.Tenants.Any(t => t.Id == _session.TenantId));
        
        var total = await usersQuery.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling((double)total / request.PageSize);
        
        var users = usersQuery.Take(request.PageSize)
            .Skip(request.PageSize * (request.Page - 1))
            .Select(x => new UserModel(x.Id, x.IdentityId));
        
        return new PagedResult<UserModel[]>(new PagedInfo(request.Page, request.PageSize, totalPages, total), users.ToArray());
    }
}