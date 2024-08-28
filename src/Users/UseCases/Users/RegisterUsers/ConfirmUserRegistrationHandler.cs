using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Shared.Models;
using Users.Database;

namespace Users.UseCases.Users.RegisterUsers;


public record ConfirmUserRegistrationRequest(string InvitationCode, string Password, string ConfirmPassword) : IRequest<Result>;

public class ConfirmUserRegistrationHandler : IRequestHandler<ConfirmUserRegistrationRequest, Result>
{
    readonly UserDbContext _dbContext;
    readonly ISender _sender;
    readonly UserManager<ApplicationUser> _userManager;

    public ConfirmUserRegistrationHandler(UserDbContext dbContext, ISender sender, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _sender = sender;
        _userManager = userManager;
    }
    
    public async ValueTask<Result> Handle(ConfirmUserRegistrationRequest request, CancellationToken cancellationToken)
    {
        var validateInvitationCode = await ValidateInvitationCode(request.InvitationCode);
        
        if(!validateInvitationCode.IsSuccess)
        {
            return validateInvitationCode;
        }
        throw new NotImplementedException();
    }
    
    async ValueTask<Result> ValidateInvitationCode(string invitationCode)
    {
        var validateInvitationCode = await _sender.Send(
            new ValidateUserInvitationCodeRequest
            {
                InvitationCode = invitationCode
            });

        return validateInvitationCode;
    }
}