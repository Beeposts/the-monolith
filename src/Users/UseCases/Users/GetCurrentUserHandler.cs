using Ardalis.Result;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Shared.Abstractions;
using Users.Database;
using Users.Domain;
using UsersContracts;

namespace Users.UseCases.Users;

public class GetCurrentUserHandler : IRequestHandler<GetCurrentUserRequest, Result<UserModel>>
{
    private readonly IUserSession _userSession;
    private readonly UserDbContext _dbContext;

    public GetCurrentUserHandler(IUserSession userSession, UserDbContext dbContext)
    {
        _userSession = userSession;
        _dbContext = dbContext;
    }
    public async ValueTask<Result<UserModel>> Handle(GetCurrentUserRequest request, CancellationToken cancellationToken)
    {
        if(_userSession.IdentityUserId is null)
            return Result<UserModel>.Invalid();
        
        var user = await _dbContext.User.FirstOrDefaultAsync(x => x.IdentityId == _userSession.IdentityUserId, cancellationToken);
        if (user is not null) return new UserModel(user.Id, user.IdentityId);
        
        user = new User
        {
            IdentityId = _userSession.IdentityUserId!,
            Email = _userSession.Email
        };
            
        _dbContext.User.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UserModel(user.Id, user.IdentityId);
    }
}