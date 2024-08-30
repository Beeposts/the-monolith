using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;
using Shared.Databases;
using Users.Domain;

namespace Users.Database;

public class UserDbContext(DbContextOptions<UserDbContext> options, IUserSession userSession, ITenantSession tenantSession) : TenantBaseDbContext<UserDbContext>(options, userSession, tenantSession)
{
    public DbSet<User> User { get; init; } = default!;
    public DbSet<Role> Role { get; init; } = default!;
    public DbSet<Tenant> Tenant { get; init; } = default!;
    public DbSet<Permission> Permissions { get; init; } = default!;
    public DbSet<ApiClient> ApiClient { get; init; } = default!;
    public DbSet<UserInvite> UserInvite { get; init; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(UserConsts.SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}