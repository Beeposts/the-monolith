using Ardalis.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared;
using Shared.Abstractions;
using Users.Database;

namespace Users.Services;

internal class TenantResolver : ITenantResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserDbContext _dbContext;
    private readonly IUserSession _userSession;
    private readonly ITenantSession _tenantSession;
    private const string TenantHeader = AppConsts.TenantHeader;
    private const string TenantIdSessionKey = "TenantId";

    public TenantResolver(
        IHttpContextAccessor httpContextAccessor,
        UserDbContext dbContext,
        IUserSession userSession,
        ITenantSession tenantSession)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
        _userSession = userSession;
        _tenantSession = tenantSession;
    }

    public async Task<Result<int>> Resolve(int tenantId)
    {
        var tenant = await _dbContext.Tenant.FirstOrDefaultAsync(x => x.Id == tenantId);
        
        if(tenant is null)
            return Result.NotFound("Tenant not found.");
        
        var userAllowed = await UserAllowedInTenant(tenant.Id);
        
        if(!userAllowed)
            return Result.Unauthorized();
        
        _tenantSession.SetTenantId(tenantId);
        return tenantId;
    }

    public async Task<Result<int>> Resolve()
    {
        var headerSlug = GetTenantSlugInHttpHeader();
        
        if (_tenantSession.TenantId is null || 
            (headerSlug is not null &&
             _tenantSession.Slug != headerSlug)) return await GetTenantInfo();
        
        return Result.Success(_tenantSession.TenantId.Value);
    }
    
    private async Task<Result<int>> GetTenantInfo()
    {
        var slug = GetTenantSlugInHttpHeader();
        if (slug is not null)
        {
            var tenant = await _dbContext.Tenant.FirstOrDefaultAsync(x => x.Slug == slug);
            if (tenant is null)
                return Result.CriticalError($"Tenant with slug '{slug}' not found.");
            
            var userAllowed = await UserAllowedInTenant(tenant.Id);
            
            if (!userAllowed)
                return Result.Unauthorized();
            
            _tenantSession.SetTenantId(tenant.Id);
            _tenantSession.SetSlug(slug);
            return tenant.Id;
        }
        
        var defaultTenantId = await GetUserDefaultTenantId();
        
        if(defaultTenantId is null)
            return Result.CriticalError("Default tenant not found.");
        
        _tenantSession.SetTenantId(defaultTenantId.Value);
        return defaultTenantId.Value;
    }
    
    private async Task<bool> UserAllowedInTenant(int tenantId)
    {
        if(_userSession.UserId is null)
            return true;
        
        var userBelongs = await _dbContext
            .User
            .AsNoTracking()
            .AnyAsync(u => u.Id == _userSession.UserId &&
                      u.Tenants.Any(t => t.Id == tenantId));
        return userBelongs;
    }
    
    private async Task<int?> GetUserDefaultTenantId()
    {
        if(_userSession.UserId is null)
            return 0;

        var tenant = await _dbContext
            .Tenant
            .OrderBy(t => t.Id)
            .Where(t => t.Users!.Any(u => u.Id == _userSession.UserId))
            .Select(t => new { t.Id })
            .FirstOrDefaultAsync();
        
        return tenant?.Id;
    }

    private string? GetTenantSlugInHttpHeader()
    {
        return _httpContextAccessor?.HttpContext?.Request?.Headers[TenantHeader];
    }
}