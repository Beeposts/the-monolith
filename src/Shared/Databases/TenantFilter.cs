using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Shared.Abstractions;
using Shared.Domains;

namespace Shared.Databases;

public static class TenantFilter
{
    public static void ApplyTenantFilters(ModelBuilder modelBuilder, ISessionService sessionService)
    {
        ApplyTenantFilter<int>(modelBuilder, sessionService);
        ApplyTenantFilter<Guid>(modelBuilder, sessionService);
        ApplyTenantFilter<long>(modelBuilder, sessionService);
    }
    private static void ApplyTenantFilter<TPrimaryKey>(ModelBuilder modelBuilder, ISessionService sessionService)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes().Where(x => typeof(TenantEntity<TPrimaryKey>).IsAssignableFrom(x.ClrType)))
        {
            LambdaExpression lambda = (typeof(TenantFilter)
                .GetMethod(nameof(GenerateTenantFilter), BindingFlags.Static | BindingFlags.NonPublic)!
                .MakeGenericMethod(entityType.ClrType, typeof(TPrimaryKey))
                .Invoke(null, [sessionService]) as LambdaExpression)!;

            entityType.SetQueryFilter(lambda);
        }
    }

    public static void ApplyDomainRules(ChangeTracker changeTracker, int tenantId)
    {
        ApplyDomainRule<int>(changeTracker, tenantId);
        ApplyDomainRule<long>(changeTracker, tenantId);
        ApplyDomainRule<Guid>(changeTracker, tenantId); 
    }

    private static void ApplyDomainRule<TPrimaryKey>(ChangeTracker changeTracker, int tenantId)
    {
        var entities = changeTracker.Entries<TenantEntity<TPrimaryKey>>().Where(x => x.State == EntityState.Added).Select(x => x.Entity);
        foreach (var entity in entities)
        {
            entity.TenantId = tenantId;
        }
    }
    
    private static Expression<Func<T, bool>> GenerateTenantFilter<T, TPrimaryKey>(ISessionService sessionService) where T : TenantEntity<TPrimaryKey>
    {
        return x => x.TenantId == sessionService.TenantId;
    }
}