namespace Shared.Abstractions;

public interface IUserSession
{
    int? UserId { get; }
    int? ClientId { get; }
    string? IdentityUserId { get; }
    string? IdentityClientId { get; }
    string? Email { get;  }
    
    public void SetUserId(int userId);
    public void SetClientId(int clientId);
}