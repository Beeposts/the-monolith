using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.Database;
using Users.Domain;

namespace Users.UseCases.Users.RegisterUsers;

public record ValidateUserInvitationCodeRequest : IRequest<Result>
{
    public string? InvitationCode { get; set; }
}

public class ValidateUserInvitationCodeHandler : IRequestHandler<ValidateUserInvitationCodeRequest, Result>
{
    readonly UserDbContext _dbContext;

    public ValidateUserInvitationCodeHandler(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async ValueTask<Result> Handle(ValidateUserInvitationCodeRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.UserInvite
            .FirstOrDefaultAsync(x => 
                    x.InviteCode == request.InvitationCode &&
                    x.ExpiresAt > DateTime.UtcNow &&
                    x.Status != UserInviteStatus.Finished
                ,cancellationToken);

        return entity is null ? Result.Invalid(new ValidationError(nameof(request.InvitationCode),"Invalid invitation code", nameof(request.InvitationCode), ValidationSeverity.Error)) : Result.Success();
    }
}