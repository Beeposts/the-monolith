using Api.Extensions;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//the package Open Api will change how to configure in dotnet 9
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "The Monolith Api", Version = "v1" });
});

builder.Services.AddIdentityServer(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

app.MapGet("/", () => "Hello World!")
    .WithOpenApi();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger(options =>
{
    options.RouteTemplate = "/openapi/{documentName}.json";
});

app.MapScalarApiReference();

app.Run();
