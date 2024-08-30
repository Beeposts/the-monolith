using Ardalis.Result;
using Mediator;
using Users.Database;
using Users.Domain;

namespace Users.UseCases.Users.RegisterUsers;

public record CreateUserInviteRequest(string Email) : IRequest<Result>;

public class CreateUserInviteHandler : IRequestHandler<CreateUserInviteRequest, Result>
{
    readonly UserDbContext _userDbContext;

    public CreateUserInviteHandler(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }
    public async ValueTask<Result> Handle(CreateUserInviteRequest request, CancellationToken cancellationToken)
    {
        var entity = new UserInvite
        {
            Email = request.Email,
            InviteCode = Guid.NewGuid().ToString(),
            Status = UserInviteStatus.Pending,
            StatusReason = UserInviteStatusReason.Created,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };
        
        await _userDbContext.UserInvite.AddAsync(entity, cancellationToken);
        await _userDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}