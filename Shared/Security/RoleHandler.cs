using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Shared.Security;

public class RoleHandler(IConfiguration configuration) : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement roleRequirement)
    {
        var roleClaim = context.User.Claims.FirstOrDefault(x => x.Type == $"{configuration["Auth0:Namespace"]}/role");
        
        if (roleClaim != null && roleRequirement.Roles.Contains(roleClaim.Value))
        {
            context.Succeed(roleRequirement);
            return Task.CompletedTask;
        }
        
        context.Fail(new AuthorizationFailureReason(this, "User is not Authorized for this action"));
        return Task.CompletedTask;
    }
}