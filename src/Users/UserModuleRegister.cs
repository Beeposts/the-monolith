using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Abstractions;
using Shared.Models;
using Shared.Modules.Abstractions;
using Users.Database;
using Users.Middlewares;
using Users.Services;

namespace Users;

public class UserModule : IModuleRegister
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration, ILogger logger)
    {
        logger.LogInformation("Registering User module services");
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseNpgsql(connectionString,
                x => { x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, UserConsts.SchemaName); });
        });

        services.AddScoped<ITenantSession, TenantSession>();
        services.AddScoped<ITenantResolver, TenantResolver>();
        services.AddScoped<IUserSession, UserSession>();
    }

    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("users", ([AsParameters]Dummy request) => $"Hello from User endpoint {request.TenantSlug}")
            .RequireAuthorization();

    }

    public void UseModule(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        dbContext.Database.Migrate();


        app.UseMiddleware<UserSessionMiddleware>();
        app.UseMiddleware<TenantResolverMiddleware>();
    }
}

public record Dummy : BaseTenantRequest
{
    [FromBody]
    public People? Person { get; init; }
}

public record People(string Name, string Email);