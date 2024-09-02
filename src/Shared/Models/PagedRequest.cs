using Ardalis.Result;
using Mediator;

namespace Shared.Models;

public record PagedRequest<TResult> : IRequest<PagedResult<TResult>>
{
    public int Page { get; init; }
    public int PageSize { get; init; }   
}