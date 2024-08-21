using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Abstractions;
using Shared.Modules.Abstractions;
using Users.Database;
using Users.Services;

namespace Users;

public class UserModule : IModuleRegister
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration, ILogger logger,
        List<Assembly> mediatorAssemblies)
    {
        logger.LogInformation("Registering User module services");
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseNpgsql(connectionString,
                x => { x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, UserConsts.SchemaName); });
        });

        services.AddScoped<ITenantSession, TenantSession>();
        
        mediatorAssemblies.Add(typeof(UserModule).Assembly);
    }

    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("users", () => "Hello from User endpoint");
    }

    public void UseModule(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        dbContext.Database.Migrate();
    }
}