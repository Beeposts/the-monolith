using Ardalis.Result;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Domains;
using Users.Database;
using Users.Domain;
using Users.StateMachines;
using Users.UseCases.Tenants;

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

        var identityUser = await _userManager.FindByEmailAsync(invitationData.Email);
        var user = new User
        {
            Email = invitationData.Email,
            IdentityId = identityUser!.Id
        };
        
        await _dbContext.User.AddAsync(user, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        const UserInviteStatusReason registeredStatus = UserInviteStatusReason.Registered;
        if (!stateMachine.CanFire(registeredStatus))
        {
            return Result.Invalid([new ValidationError("Status", "Invalid status transition", "Status", ValidationSeverity.Error)]);
        }
        
        var newState = stateMachine.Fire(registeredStatus);
        invitationData.Status = newState;
        invitationData.StatusReason = registeredStatus;
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        if (invitationData.InvitedByUserId is not null && invitationData.InvitedByTenantId is not null)
        {
            await _sender.Send(new AddUserToTenantRequest(user.Id, invitationData.InvitedByTenantId.Value), cancellationToken);
            return Result.Success();
        }
        
        await _sender.Send(new GenerateDefaultTenant(user.Id), cancellationToken);
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