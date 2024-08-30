using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Interfaces;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Databases;
using Shared.Domains;
using Shared.Models;
using Users.Database;

namespace Users.Extensions;

public static class IdentityStartup
{
    public static void AddIdentityConfigurationDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new ConfigurationStoreOptions());
        services.AddSingleton(new OperationalStoreOptions());
        services.AddDbContext<ConfigurationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("IdentityConnection")));

        services.AddIdentityCore<ApplicationUser>()
            .AddEntityFrameworkStores<IdentityAppDbContext>();
        
        services.AddScoped<IConfigurationDbContext, ConfigurationDbContext>();

    }

    
    public static void AddIdentityDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityAppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("IdentityConnection")));
    }
    
    public static void ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentityDatabase(configuration);
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityAppDbContext>()
            .AddDefaultTokenProviders();
    } 

    public static void ConfigureIdentityService(this IServiceCollection services, IConfiguration configuration)
    {
        var migrationsAssembly = typeof(IdentityStartup).Assembly.GetName().Name;
        var connectionString = configuration.GetConnectionString("IdentityConnection");
        services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                
                options.EmitStaticAudienceClaim = true;
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseNpgsql(connectionString,
                    sql => sql.MigrationsAssembly(migrationsAssembly));
                options.EnableTokenCleanup = true;
            })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseNpgsql(connectionString,
                    sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddAspNetIdentity<ApplicationUser>();
    }
}