using Api;
using Api.Extensions;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Shared.Abstractions;
using Shared.Modules;
using Users;

var logger = LoggerFactory.Create(options => { options.AddConsole(); }).CreateLogger("Startup");
var builder = WebApplication.CreateBuilder(args);
var mediatorAssemblies = new List<System.Reflection.Assembly>();

//the package Open Api will change how to configure in dotnet 9
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "The Monolith Api", Version = "v1" });
});

builder.Services.AddIdentityServer(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserSession, UserSession>();
builder.Services.AddModule<UserModule>()
    .RegisterModules(builder.Configuration, logger, mediatorAssemblies);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.Cookie.Name = ".TheMonolith.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(5);
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!")
    .WithOpenApi();


app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.UseModules(logger);

app.UseSwagger(options =>
{
    options.RouteTemplate = "/openapi/{documentName}.json";
});

app.MapScalarApiReference();

app.RegisterEndpoints(logger);
app.CleanModuleStartup();

app.Run();
