using Ardalis.Result;
using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Entities;
using Duende.IdentityServer.Models;
using IdentityModel;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Commands;
using Shared.Utils;
using Users.Database;
using Users.Domain;

namespace Users.UseCases.ApiClients.CreateApiClient;

public record CreateApiClientRequest : TenantContextResolverCommand<CreateApiClientResponse>
{ 
    [FromBody]
    public ApiClientData? Data { get; set; }
    public record ApiClientData(string ClientName);
}

public record CreateApiClientResponse(string ClientName, string ClientId, string ClientSecret);

public class CreateApiClientHandler : IRequestHandler<CreateApiClientRequest, Result<CreateApiClientResponse>>
{
    private readonly UserDbContext _userDbContext;
    private readonly ConfigurationDbContext _identityDbContext;

    public CreateApiClientHandler(UserDbContext userDbContext, ConfigurationDbContext identityDbContext)
    {
        _userDbContext = userDbContext;
        _identityDbContext = identityDbContext;
    }

    public async ValueTask<Result<CreateApiClientResponse>> Handle(CreateApiClientRequest request,
        CancellationToken cancellationToken)
    {
        var clientSecret = ClientUtils.GenerateToken(30);
        var clientId = Guid.NewGuid().ToString();

        var clientEntity = new Duende.IdentityServer.EntityFramework.Entities.Client
        {
            ClientId = clientId,
            ClientClaimsPrefix = "",
            ClientName = request.Data!.ClientName,
            ClientSecrets =
            [
                new ClientSecret
                {
                    Created = DateTime.UtcNow,
                    Type = IdentityServerConstants.SecretTypes.SharedSecret,
                    Value = clientSecret.ToSha256()
                }
            ],
            AllowedGrantTypes = [new ClientGrantType { GrantType = GrantType.ClientCredentials }],
            AllowedScopes =
            [
                new ClientScope
                {
                    Scope = IdentityConsts.ApiScope
                }
            ]
        };

        await _identityDbContext.Clients.AddAsync(clientEntity, cancellationToken);
        await _identityDbContext.SaveChangesAsync(cancellationToken);
        
        var apiClient = new ApiClient
        {
            ClientId = clientId,
            ClientName = request.Data.ClientName
        };

        await _userDbContext.ApiClient.AddAsync(apiClient, cancellationToken);
        await _userDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new CreateApiClientResponse(clientEntity.ClientName, clientEntity.ClientId, clientSecret));
    }

}