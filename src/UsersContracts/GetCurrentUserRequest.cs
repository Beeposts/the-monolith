using Ardalis.Result;
using Mediator;

namespace UsersContracts;

public record GetCurrentUserRequest : IRequest<Result<UserModel>>
{
    public static GetCurrentUserRequest Instance => new GetCurrentUserRequest();
}