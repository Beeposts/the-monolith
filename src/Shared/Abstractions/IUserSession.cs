namespace Shared.Abstractions;

public interface IUserSession
{
    int? UserId { get; }
    string? IdentityUserId { get; }
    string? Email { get;  }
}