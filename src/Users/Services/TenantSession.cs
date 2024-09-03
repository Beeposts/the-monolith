using Microsoft.AspNetCore.Http;
using Shared.Abstractions;

namespace Users.Services;

internal class TenantSession : ITenantSession
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string TenantIdSessionKey = "TenantId";
    private const string TenantSlugSessionKey = "TenantSlug";
    private int? OverrideTenantId { get; set; }

    public TenantSession(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? TenantId => OverrideTenantId ?? _httpContextAccessor.HttpContext?.Session.GetInt32(TenantIdSessionKey);
    public Guid? Slug
    {
        get
        {
            var slugSession= _httpContextAccessor.HttpContext?.Session.GetString(TenantSlugSessionKey);

            if (slugSession is null)
                return null;
            
            return Guid.TryParse(slugSession, out var result) ? result : null;
        }
    }

    public void SetTenantId(int tenantId)
    {
        if(_httpContextAccessor.HttpContext?.Session is not null)
            _httpContextAccessor.HttpContext?.Session.SetInt32(TenantIdSessionKey, tenantId);
        else
            OverrideTenantId = tenantId;
    }
    
    public void SetSlug(Guid slug)
    {
        _httpContextAccessor.HttpContext?.Session?.SetString(TenantSlugSessionKey, slug.ToString());
    }
}