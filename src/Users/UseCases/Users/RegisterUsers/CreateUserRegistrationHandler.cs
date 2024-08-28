using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Shared.Commands;
using Users.Database;
using Users.Domain;
using Users.StateMachines;

namespace Users.UseCases.Users.RegisterUsers;

public record CreateUserRegistrationRequest(string Email, string Name) : IRequest<Result>;

public class CreateUserRegistrationHandler : IRequestHandler<CreateUserRegistrationRequest, Result>
{
    readonly UserDbContext _userDbContext;

    public CreateUserRegistrationHandler(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }
    public async ValueTask<Result> Handle(CreateUserRegistrationRequest request, CancellationToken cancellationToken)
    {
        var entity = new UserRegistration
        {
            Email = request.Email,
            InviteCode = Guid.NewGuid().ToString(),
            Status = UserRegistrationStatus.New,
            StatusReason = UserRegistrationStatusReason.Created
        };
        
        await _userDbContext.UserRegistration.AddAsync(entity, cancellationToken);
        await _userDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}