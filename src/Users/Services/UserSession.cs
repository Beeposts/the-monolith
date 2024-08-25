using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Shared;
using Shared.Abstractions;

namespace Users.Services;

public class UserSession : IUserSession
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserSession(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId => _httpContextAccessor.HttpContext?.Session.GetInt32(AppConsts.UserSessionKey);
    public string? IdentityUserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtClaimTypes.Email);
    
    public void SetUserId(int userId)
    {
        _httpContextAccessor.HttpContext?.Session?.SetInt32(AppConsts.UserSessionKey, userId);
    }
}