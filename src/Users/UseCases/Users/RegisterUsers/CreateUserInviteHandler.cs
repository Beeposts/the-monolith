using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Users.Database;
using Users.Domain;

namespace Users.UseCases.Users.RegisterUsers;

public record CreateUserInviteRequest(string Email, int? InvitedByUserId, int? InvitedByTenantId) : IRequest<Result>;

public class CreateUserInviteHandler : IRequestHandler<CreateUserInviteRequest, Result>
{
    readonly UserDbContext _userDbContext;

    public CreateUserInviteHandler(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }
    public async ValueTask<Result> Handle(CreateUserInviteRequest request, CancellationToken cancellationToken)
    {
        var existingInvite = await _userDbContext.UserInvite
            .FirstOrDefaultAsync(x => x.Email == request.Email && x.Status == UserInviteStatus.Pending, cancellationToken);

        if (existingInvite is not null)
        {
            return Result.Invalid(new ValidationError("Email", "Email already invited", "Email", ValidationSeverity.Error));
        }
        
        var entity = new UserInvite
        {
            Email = request.Email,
            InviteCode = Guid.NewGuid().ToString(),
            Status = UserInviteStatus.Pending,
            StatusReason = UserInviteStatusReason.Created,
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            InvitedByUserId = request.InvitedByUserId,
            InvitedByTenantId = request.InvitedByTenantId
        };
        
        await _userDbContext.UserInvite.AddAsync(entity, cancellationToken);
        await _userDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}