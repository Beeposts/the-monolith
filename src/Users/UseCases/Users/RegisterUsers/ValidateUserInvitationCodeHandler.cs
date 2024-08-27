using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.Database;

namespace Users.UseCases.Users.RegisterUsers;

public record ValidateUserInvitationCodeRequest : IRequest<Result<Unit>>
{
    [FromBody]
    public ValidateUserInvitationCodeData? Data { get; set; }
    public record ValidateUserInvitationCodeData
    {
        public string? InvitationCode { get; set; }
    }
}

public class ValidateUserInvitationCodeHandler : IRequestHandler<ValidateUserInvitationCodeRequest, Result<Unit>>
{
    readonly UserDbContext _dbContext;

    public ValidateUserInvitationCodeHandler(UserDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async ValueTask<Result<Unit>> Handle(ValidateUserInvitationCodeRequest request, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.UserRegistration
            .FirstOrDefaultAsync(x => 
                    x.InviteCode == request.Data!.InvitationCode &&
                    x.ExpiresAt > DateTime.UtcNow
                ,cancellationToken);

        return entity is null ? Result.Invalid(new ValidationError("Invalid invitation code")) : Result.Success();
    }
}