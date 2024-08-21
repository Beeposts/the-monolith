using Shared.Abstractions;

namespace Api;

public class UserSession : IUserSession
{
    public int? UserId => 1;
    public string? IdentityUserId => "t123213";
    public string? Email => "teste@teste.com";
}