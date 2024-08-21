using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Shared.Modules.Abstractions;

public interface IModuleRegister
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration, ILogger logger, List<System.Reflection.Assembly> mediatorAssemblies);
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints);
    public void UseModule(IApplicationBuilder app);
    
}