using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Users.Database;

namespace Users.UseCases.Tenants;

public record AddUserToTenantRequest(int UserId, int TenantId) : IRequest<Result>;

public class AddUserToTenantHandler : IRequestHandler<AddUserToTenantRequest, Result>
{
    readonly UserDbContext _dbContext;

    public AddUserToTenantHandler(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async ValueTask<Result> Handle(AddUserToTenantRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.User.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        var tenant = await _dbContext.Tenant.FirstOrDefaultAsync(x => x.Id == request.TenantId, cancellationToken);
        
        if(user is null)
        {
            return Result.NotFound("User not found.");
        }
        
        if(tenant is null)
        {
            return Result.NotFound("Tenant not found.");
        }
        
        user.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}