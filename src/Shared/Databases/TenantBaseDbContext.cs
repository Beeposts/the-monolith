using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shared.Abstractions;
using Shared.Domains;

namespace Shared.Databases;

public abstract class TenantBaseDbContext<TDbContext> : DbContext
    where TDbContext : DbContext
{
    private readonly IUserSession _userSession;
    private readonly ITenantSession _tenantSession;
    protected TenantBaseDbContext(DbContextOptions<TDbContext> options, 
        IUserSession userSession,
            ITenantSession tenantSession) : base(options)
    {
        _userSession = userSession;
        _tenantSession = tenantSession;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ApplyTenantFilters(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var tenantId = _tenantSession.TenantId;
        ApplyDomainRules(ChangeTracker, tenantId);
        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    protected void ApplyTenantFilters(ModelBuilder modelBuilder)
    {
        ApplyTenantFilter<int>(modelBuilder);
        ApplyTenantFilter<Guid>(modelBuilder);
        ApplyTenantFilter<long>(modelBuilder);
    }
    private void ApplyTenantFilter<TPrimaryKey>(ModelBuilder modelBuilder)
    {

        var method = GetType()
            .GetMethod(nameof(GenerateTenantFilter), BindingFlags.Instance | BindingFlags.NonPublic);
        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(x => typeof(TenantEntity<TPrimaryKey>).IsAssignableFrom(x.ClrType)))
        {
            LambdaExpression lambda = (method?
                .MakeGenericMethod(entityType.ClrType, typeof(TPrimaryKey))
                .Invoke(this, null) as LambdaExpression)!;

            entityType.SetQueryFilter(lambda);
        }
    }

    private void ApplyDomainRules(ChangeTracker changeTracker, int? tenantId)
    {
        ApplyDomainRule<int>(changeTracker, tenantId);
        ApplyDomainRule<long>(changeTracker, tenantId);
        ApplyDomainRule<Guid>(changeTracker, tenantId); 
    }

    private void ApplyDomainRule<TPrimaryKey>(ChangeTracker changeTracker, int? tenantId)
    {
        if(tenantId is null)
            return;
        
        var entities = changeTracker.Entries<TenantEntity<TPrimaryKey>>().Where(x => x.State == EntityState.Added).Select(x => x.Entity);
        foreach (var entity in entities)
        {
            entity.TenantId = tenantId.Value;
        }
    }
    
    protected Expression<Func<T, bool>> GenerateTenantFilter<T, TPrimaryKey>() where T : TenantEntity<TPrimaryKey>
    {
        return x => _tenantSession.TenantId == 0 || x.TenantId == _tenantSession.TenantId;
    }
}