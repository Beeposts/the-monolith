using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Domains;
using Users.Database;
using Users.Domain;
using Users.StateMachines;

namespace Users.UseCases.Users.RegisterUsers;


public record RegisterUserRequest(string InvitationCode, string Password, string ConfirmPassword) : IRequest<Result>;

public class RegisterUserHandler : IRequestHandler<RegisterUserRequest, Result>
{
    readonly UserDbContext _dbContext;
    readonly ISender _sender;
    readonly UserManager<ApplicationUser> _userManager;

    public RegisterUserHandler(UserDbContext dbContext, ISender sender, UserManager<ApplicationUser> userManager)
    {
        _dbContext = dbContext;
        _sender = sender;
        _userManager = userManager;
    }
    
    public async ValueTask<Result> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var validateInvitationCode = await ValidateInvitationCode(request.InvitationCode);
        
        if(!validateInvitationCode.IsSuccess)
        {
            return validateInvitationCode;
        }
        
        var invitationData = await _dbContext.UserInvite
            .FirstOrDefaultAsync(x => x.InviteCode == request.InvitationCode, cancellationToken);

        var stateMachine = new UserRegistrationStateMachine(() => invitationData!.Status, s => invitationData!.Status = s);
        var userExists = await _userManager.FindByEmailAsync(invitationData!.Email);
        
        if(userExists is not null)
        {
            return Result.Invalid([new ValidationError("Email", "Email already registered", "Email", ValidationSeverity.Error)]);
        }

        var userCreationResult = await _userManager.CreateAsync(new ApplicationUser
        {
            Email = invitationData.Email,
            UserName = invitationData.Email,
            EmailConfirmed = true,
            
        }, request.Password);
        
        if(userCreationResult.Errors.Any())
        {
            var errors = userCreationResult.Errors.Select(x =>
                    new ValidationError(x.Code, x.Description, x.Code, ValidationSeverity.Error))
                .ToArray();
            
            return Result.Invalid(errors);
        }
        
        const UserInviteStatusReason registeredStatus = UserInviteStatusReason.Registered;
        if (!stateMachine.CanFire(registeredStatus))
        {
            return Result.Invalid([new ValidationError("Status", "Invalid status transition", "Status", ValidationSeverity.Error)]);
        }
        
        var newState = stateMachine.Fire(registeredStatus);
        invitationData.Status = newState;
        invitationData.StatusReason = registeredStatus;
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
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