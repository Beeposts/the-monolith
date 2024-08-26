using Api;
using Api.Extensions;
using Community.Microsoft.Extensions.Caching.PostgreSql;
using Mediator;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Shared.Abstractions;
using Shared.Behaviors;
using Shared.Modules;
using Users;

var logger = LoggerFactory.Create(options => { options.AddConsole(); }).CreateLogger("Startup");
var builder = WebApplication.CreateBuilder(args);

//the package Open Api will change how to configure in dotnet 9
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "The Monolith Api", Version = "v1" });
});

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TenantBackgroundContextBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TenantContextResolverBehavior<,>));

builder.Services.ConfigureIdentityServer(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

builder.Services.AddModule<UserModule>()
    .RegisterModules(builder.Configuration, logger);

//builder.Services.AddDistributedMemoryCache();
builder.Services.AddDistributedPostgreSqlCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("CacheConnection");
    options.SchemaName = "cache";
    options.TableName = "cache";
    options.CreateInfrastructure = true;
});
builder.Services.AddSession(options => {
    options.Cookie.Name = ".TheMonolith.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(5);
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!")
    .RequireAuthorization()
    .WithOpenApi();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.UseModules(logger);

app.UseSwagger(options =>
{
    options.RouteTemplate = "/openapi/{documentName}.json";
});

app.MapScalarApiReference();

app.RegisterEndpoints(logger);
app.CleanModuleStartup();

app.Run();
