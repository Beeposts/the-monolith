using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Modules.Abstractions;

namespace Shared.Modules;

public static class ModuleLoader
{
    private static List<Type> ModulesTypes { get; set; } = [];
    private static Dictionary<string, IModuleRegister> Modules { get; } = new();
    
    public static void RegisterModules(this IServiceCollection services, IConfiguration configuration, ILogger logger, List<System.Reflection.Assembly> mediatorAssemblies)
    {
        logger.LogInformation("Loading modules");
        
        logger.LogInformation("Found {ModuleCount} modules", ModulesTypes.Count);
        foreach (var moduleType in ModulesTypes)
        {
            var module = GetModuleInstance(moduleType);
            logger.LogInformation("Registering {Module}", moduleType.Name);
            module.RegisterServices(services, configuration, logger, mediatorAssemblies);
            logger.LogInformation("{Module} registered", moduleType.Name);
        }
    }

    public static IServiceCollection AddModule<TModule>(this IServiceCollection services) where TModule : IModuleRegister
    {
        var moduleType = typeof(TModule);
        ModulesTypes.Add(moduleType);
        return services;
    }
    
    public static void RegisterEndpoints(this IEndpointRouteBuilder endpoints, ILogger logger)
    {
        
        logger.LogInformation("Registering modules endpoints");
        foreach (var moduleType in ModulesTypes)
        {
            logger.LogInformation("Registering {Module} endpoints", moduleType.Name);
            var module = GetModuleInstance(moduleType);
            module.RegisterEndpoints(endpoints);
            logger.LogInformation("{Module} endpoints registered", moduleType.Name);
        }
    }
    
    public static void UseModules(this IApplicationBuilder app, ILogger logger)
    {
        logger.LogInformation("Registering modules middlewares");
        foreach (var moduleType in ModulesTypes)
        {
            logger.LogInformation("Registering {Module} middlewares", moduleType.Name);
            var module = GetModuleInstance(moduleType);
            module.UseModule(app);
            logger.LogInformation("{Module} middlewares registered", moduleType.Name);
        }
    }
    
    public static void CleanModuleStartup(this IApplicationBuilder app)
    {
        ModulesTypes.Clear();
        Modules.Clear();
    }
    
    private static IModuleRegister GetModuleInstance(Type moduleType)
    {
        if(Modules.TryGetValue(moduleType.Name, out var instance))
            return instance;
        
        instance = (IModuleRegister)Activator.CreateInstance(moduleType)!;
        Modules.Add(moduleType.Name, instance);
        return instance;
    }
}