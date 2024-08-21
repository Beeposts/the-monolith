using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Modules.Abstractions;

namespace Users;

public class UserModule : IModuleRegister
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration, ILogger logger,
        List<Assembly> mediatorAssemblies)
    {
        logger.LogInformation("Registering User module services");
    }

    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("users", () => "Hello from User endpoint");
    }

    public void UseModule(IApplicationBuilder app)
    {
        
    }
}