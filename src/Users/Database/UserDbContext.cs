using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;
using Shared.Databases;
using Users.Domain;

namespace Users.Database;

internal class UserDbContext(DbContextOptions<UserDbContext> options, IUserSession userSession, ITenantSession tenantSession) : TenantBaseDbContext<UserDbContext>(options, userSession, tenantSession)
{
    public DbSet<User> User { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<Tenant> Tenant { get; set; }
    public DbSet<Permission> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(UserConsts.SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}