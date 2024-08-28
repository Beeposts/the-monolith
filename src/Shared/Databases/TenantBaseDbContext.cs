using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shared.Abstractions;
using Shared.Domains;
using Shared.Domains.Abstractions;

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
        var userId = _userSession.UserId;
        var clientId = _userSession.ClientId;
        ApplyDomainRules(ChangeTracker, tenantId);
        ApplyAuditRule(ChangeTracker, userId, clientId);
        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    void ApplyTenantFilters(ModelBuilder modelBuilder)
    {
        ApplyTenantFilter<int>(modelBuilder);
        ApplyTenantFilter<Guid>(modelBuilder);
        ApplyTenantFilter<long>(modelBuilder);
    }
    void ApplyTenantFilter<TPrimaryKey>(ModelBuilder modelBuilder)
    {

        var method = GetType()
            .GetMethod(nameof(GenerateTenantFilter), BindingFlags.Instance | BindingFlags.NonPublic);
        
        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(x => typeof(ITenantEntity<TPrimaryKey>).IsAssignableFrom(x.ClrType)))
        {
            var lambda = (method?
                .MakeGenericMethod(entityType.ClrType, typeof(TPrimaryKey))
                .Invoke(this, null) as LambdaExpression)!;

            entityType.SetQueryFilter(lambda);
        }
    }

    static void ApplyDomainRules(ChangeTracker changeTracker, int? tenantId)
    {
        ApplyDomainRule<int>(changeTracker, tenantId);
        ApplyDomainRule<long>(changeTracker, tenantId);
        ApplyDomainRule<Guid>(changeTracker, tenantId); 
    }

    static void ApplyDomainRule<TPrimaryKey>(ChangeTracker changeTracker, int? tenantId)
    {
        if(tenantId is null)
            return;
        
        var entities = changeTracker.Entries<ITenantEntity<TPrimaryKey>>().Where(x => x.State == EntityState.Added).Select(x => x.Entity);
        foreach (var entity in entities)
        {
            entity.TenantId = tenantId.Value;
        }
    }

    static void ApplyAuditRule(ChangeTracker changeTracker, int? userId, int? clientId)
    {
        ApplyAuditRule<int>(changeTracker, userId);
        ApplyAuditRule<long>(changeTracker, userId);
        ApplyAuditRule<Guid>(changeTracker, userId);
        
        ApplyApiClientAuditRule<int>(changeTracker, clientId);
        ApplyApiClientAuditRule<long>(changeTracker, clientId);
        ApplyApiClientAuditRule<Guid>(changeTracker, clientId);
    }

    static void ApplyAuditRule<TPrimaryKey>(ChangeTracker changeTracker, int? userId)
    {
        if(userId is null)
            return;
        
        var entities = changeTracker.Entries<IAuditableEntity<TPrimaryKey>>().Where(x => x.State == EntityState.Added).Select(x => x.Entity);
        foreach (var entity in entities)
        {
            entity.CreatedBy = userId.Value;
            entity.CreatedAt = DateTime.UtcNow;
        }
        
        entities = changeTracker.Entries<IAuditableEntity<TPrimaryKey>>().Where(x => x.State == EntityState.Modified).Select(x => x.Entity);
        foreach (var entity in entities)
        {
            entity.UpdatedBy = userId.Value;
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
    
    static void ApplyApiClientAuditRule<TPrimaryKey>(ChangeTracker changeTracker, int? clientId)
    {
        if(clientId is null)
            return;
        
        var entities = changeTracker.Entries<IAuditableEntity<TPrimaryKey>>().Where(x => x.State == EntityState.Added).Select(x => x.Entity);
        foreach (var entity in entities)
        {
            entity.CreatedByClientId = clientId.Value;
            entity.CreatedAt = DateTime.UtcNow;
        }
        
        entities = changeTracker.Entries<IAuditableEntity<TPrimaryKey>>().Where(x => x.State == EntityState.Modified).Select(x => x.Entity);
        foreach (var entity in entities)
        {
            entity.UpdatedByClientId = clientId.Value;
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
    
    protected Expression<Func<T, bool>> GenerateTenantFilter<T, TPrimaryKey>() where T : TenantEntity<TPrimaryKey>
    {
        return x => _tenantSession.TenantId == 0 || x.TenantId == _tenantSession.TenantId;
    }
}