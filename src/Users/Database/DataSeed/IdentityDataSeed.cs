using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared;
using Shared.Databases;
using Shared.Domains;
using Shared.Models;

namespace Users.Database.DataSeed;

public static class IdentityDataSeed
{
    public static async Task EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var persistedGrantDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
        await persistedGrantDbContext.Database.MigrateAsync();

        await using var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        await configurationDbContext.Database.MigrateAsync();

        await using var context = scope.ServiceProvider.GetRequiredService<IdentityAppDbContext>();
        await context.Database.MigrateAsync();

        using var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        using var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await CreateRole(roleMgr);

        await GenerateDefaultIdentityResources(configurationDbContext);
        await GenerateDefaultApiScopes(configurationDbContext);
        await GenerateDefaultApiResources(configurationDbContext);

        await CreateUser(userMgr);
        await CreateInteractive(configurationDbContext);
        await CreateM2MDefaultClients(configurationDbContext);
    }

    private static Task CreateRole(RoleManager<IdentityRole> roleMgr)
    {
        return Task.CompletedTask;
    }

    private static async Task CreateUser(UserManager<ApplicationUser> userMgr)
    {
        var admin = await userMgr.FindByNameAsync("admin");
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@email.com",
                EmailConfirmed = true,
            };
            var result = await userMgr.CreateAsync(admin, "Pass123@");
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(admin, new Claim[]{
                new Claim(JwtClaimTypes.Name, "Monolith Admin"),
                new Claim(JwtClaimTypes.GivenName, "Monolith"),
                new Claim(JwtClaimTypes.FamilyName, "Admin"),
            }).Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
        }
    }

    private static async Task GenerateDefaultIdentityResources(ConfigurationDbContext dbContext)
    {
        if (await dbContext.IdentityResources.AnyAsync())
            return;

        var profile = new IdentityResources.Profile();

        await dbContext.IdentityResources.AddAsync(new Duende.IdentityServer.EntityFramework.Entities.IdentityResource
        {
            Name = profile.Name,
            DisplayName = profile.DisplayName,
            Emphasize = profile.Emphasize,
            UserClaims = profile.UserClaims.Select(x => new IdentityResourceClaim
            {
                Type = x
            }).ToList()
        });

        var openId = new IdentityResources.OpenId();

        await dbContext.IdentityResources.AddAsync(new Duende.IdentityServer.EntityFramework.Entities.IdentityResource
        {
            Name = openId.Name,
            DisplayName = openId.DisplayName,
            Emphasize = openId.Emphasize,
            UserClaims = openId.UserClaims.Select(x => new IdentityResourceClaim
            {
                Type = x
            }).ToList()
        });

        var email = new IdentityResources.Email();

        await dbContext.IdentityResources.AddAsync(new Duende.IdentityServer.EntityFramework.Entities.IdentityResource
        {
            Name = email.Name,
            DisplayName = email.DisplayName,
            Emphasize = email.Emphasize,
            UserClaims = email.UserClaims.Select(x => new IdentityResourceClaim
            {
                Type = x
            }).ToList()
        });

        await dbContext.SaveChangesAsync();
    }

    private static async Task GenerateDefaultApiScopes(ConfigurationDbContext dbContext)
    {

        if (await dbContext.ApiScopes.AnyAsync())
            return;

        await dbContext.ApiScopes.AddAsync(new Duende.IdentityServer.EntityFramework.Entities.ApiScope
        {
            Name = IdentityConsts.ApiScope,
            DisplayName = "Api"
        });

        await dbContext.SaveChangesAsync();
    }

    private static async Task GenerateDefaultApiResources(ConfigurationDbContext dbContext)
    {

        if (await dbContext.ApiResources.AnyAsync())
            return;
        
        await dbContext.ApiResources.AddAsync(new Duende.IdentityServer.EntityFramework.Entities.ApiResource
        {
            Name = IdentityConsts.ApiScope,
            DisplayName = "Api",
            Scopes = [new ApiResourceScope { Scope = IdentityConsts.ApiScope }],
        });

        await dbContext.SaveChangesAsync();
    }

    private static async Task CreateInteractive(ConfigurationDbContext dbContext)
    {
        const string clientId = "interactive";

        if (await dbContext.Clients.AnyAsync(x => x.ClientId == clientId))
            return;

        await dbContext.Clients.AddAsync(new Duende.IdentityServer.EntityFramework.Entities.Client
        {
            ClientId = clientId,
            ClientSecrets =
            [
                new ClientSecret
                {
                    Created = DateTime.UtcNow,
                    Type = IdentityServerConstants.SecretTypes.SharedSecret,
                    Value = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()
                }
            ],
            AllowedGrantTypes = [
                new ClientGrantType { GrantType = GrantType.AuthorizationCode },
                new ClientGrantType { GrantType = GrantType.ClientCredentials }
            ],
            AlwaysSendClientClaims = true,
            AlwaysIncludeUserClaimsInIdToken = true,
            RedirectUris = [new ClientRedirectUri { RedirectUri = "https://localhost:3000/auth/oidc/callback" }],            
            PostLogoutRedirectUris =
                [new ClientPostLogoutRedirectUri { PostLogoutRedirectUri = "https://localhost:3000" }],
            AllowedScopes =
            [
                new ClientScope
                {
                    Scope = IdentityServerConstants.StandardScopes.OpenId
                },

                new ClientScope
                {
                    Scope = IdentityServerConstants.StandardScopes.Profile
                },

                new ClientScope
                {
                    Scope = IdentityServerConstants.StandardScopes.Email
                },

                new ClientScope
                {
                    Scope = IdentityConsts.ApiScope
                }

            ]
        });

        await dbContext.SaveChangesAsync();
    }


    private static async Task CreateM2MDefaultClients(ConfigurationDbContext dbContext)
    {
        string clientId = "m2m.client";

        if (await dbContext.Clients.AnyAsync(x => x.ClientId == clientId))
            return;


        await dbContext.Clients.AddAsync(new Duende.IdentityServer.EntityFramework.Entities.Client
        {
            ClientId = clientId,
            ClientSecrets = new List<ClientSecret>{
                new ClientSecret {
                    Created = DateTime.UtcNow,
                    Type = IdentityServerConstants.SecretTypes.SharedSecret,
                    Value = "511536EF-F270-4058-80CA-1C89C192F69A".ToSha256()
                }
            },
            AllowedGrantTypes = [new ClientGrantType { GrantType = GrantType.ClientCredentials }],
            AlwaysSendClientClaims = true,
            AlwaysIncludeUserClaimsInIdToken = true,
            AllowedScopes =
            [
                new ClientScope
                {
                    Scope = IdentityConsts.ApiScope
                }

            ]
        });

        await dbContext.SaveChangesAsync();
    }
}