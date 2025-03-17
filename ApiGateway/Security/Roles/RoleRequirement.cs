using Microsoft.AspNetCore.Authorization;

namespace ApiGateway.Security.Roles;

public class RoleRequirement(IEnumerable<string> roles) : IAuthorizationRequirement
{
    public IEnumerable<string> Roles { get; } = roles;
}