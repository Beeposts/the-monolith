using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using Shared.Commands;
using Users.Database;
using Users.Domain;
using Users.StateMachines;

namespace Users.UseCases.Users.RegisterUsers;

public record RegisterUserRequest : IRequest<Result<Unit>>
{
    [FromBody]
    public RegisterUserRequestData? Data { get; set; }
    
    public record RegisterUserRequestData(string FirstName, string LastName, string Email, string Password, string ConfirmPassword);
}

public class RegisterUserHandler : IRequestHandler<RegisterUserRequest, Result<Unit>>
{
    readonly UserDbContext _userDbContext;

    public RegisterUserHandler(UserDbContext userDbContext)
    {
        _userDbContext = userDbContext;
    }
    public async ValueTask<Result<Unit>> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var entity = new UserRegistration
        {
            Email = request.Data!.Email,
            FirstName = request.Data!.FirstName,
            LastName = request.Data!.LastName,
            InviteCode = Guid.NewGuid().ToString(),
            Status = UserRegistrationStatus.New,
            StatusReason = UserRegistrationStatusReason.Created
        };
        
        await _userDbContext.UserRegistration.AddAsync(entity, cancellationToken);
        await _userDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}