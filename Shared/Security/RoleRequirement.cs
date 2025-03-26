using Microsoft.AspNetCore.Authorization;

namespace Shared.Security;

public class RoleRequirement(IEnumerable<string> roles) : IAuthorizationRequirement
{
    public IEnumerable<string> Roles { get; } = roles;
}