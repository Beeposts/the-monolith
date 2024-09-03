using Ardalis.Result;
using MassTransit;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Users.Database;
using Users.Domain;
using UsersContracts;

namespace Users.UseCases.Tenants;

public record GenerateDefaultTenant(int UserId) : IRequest<Result<TenantModel>>;

public class GenerateDefaultTenantHandler : IRequestHandler<GenerateDefaultTenant,Result<TenantModel>>
{
    readonly UserDbContext _dbContext;

    public GenerateDefaultTenantHandler(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask<Result<TenantModel>> Handle(GenerateDefaultTenant request, CancellationToken cancellationToken)
    {
        var tenant = new Tenant
        {
            Name = "Default",
            Slug = NewId.NextSequentialGuid()
        };

        var user = await _dbContext.User.FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        
        if(user is null)
        {
            return Result.NotFound("User not found.");
        }
        
        user.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return new TenantModel(tenant.Id, tenant.Name, tenant.Slug);
    }
}