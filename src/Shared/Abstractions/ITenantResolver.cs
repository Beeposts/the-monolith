using Ardalis.Result;

namespace Shared.Abstractions;

public interface ITenantResolver
{
    Task<Result<int>> Resolve(int tenantId);
    Task<Result<int>> Resolve();
}