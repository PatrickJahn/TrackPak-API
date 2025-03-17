using System.Security.Claims;
using ApiGateway.Security.Roles;

namespace ApiGateway.Services;

public class UserContextService(
    IConfiguration configuration,
    IHttpContextAccessor httpContextAccessor,
    ILogger<IUserContextService> logger) : IUserContextService
{
    public Guid GetUserId()
    {
        var userId = httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type.EndsWith("/user_id"))?.Value;

        if (userId is not null) return new Guid(userId);

        if (IsClientCredentials())
        {
            return default;
        }

        logger.LogWarning("Custom claim 'user_id' is null");
        return default;
    }

    public Role GetRole()
    {
        var role = httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type.Equals($"{configuration["Auth0:Namespace"]}/role"))?.Value;

        if (role is not null)
        {
            return role switch
            {
                RoleAsString.SystemAdmin => Role.SystemAdmin,
                RoleAsString.CompanyAdmin => Role.CompanyAdmin,
                RoleAsString.Driver => Role.Driver,
                RoleAsString.Customer => Role.Customer,
                _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
            };
        }

        logger.LogWarning("Custom claim 'role' is null");
        throw new NullReferenceException("Role is null");
    }

    public Guid GetTenantId()
    {
        var organizationId = httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type.EndsWith("/organization_id"))?.Value;

        if (organizationId is not null) return new Guid(organizationId);

        if (IsClientCredentials())
        {
            return default;
        }

        logger.LogWarning("Custom claim 'organization_id' is null");
        return default;
    }

    public bool IsSystemAdministrator()
    {
        return httpContextAccessor.HttpContext?.User.IsInRole(RoleAsString.SystemAdmin) ?? false;
    }

    public string GetAuthId()
    {
        var auth0Id = httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (auth0Id is not null) return auth0Id;

        throw new NullReferenceException("Auth0 id is null");
    }
    public string GetCompanyId()
    {
        var companyId = httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type.Equals($"{configuration["Auth0:Namespace"]}/company_id"))?.Value;

        if (companyId is not null) return companyId;

        throw new NullReferenceException("Company ID is null");
    }

    public string GetEmail()
    {
        var email = httpContextAccessor.HttpContext?.User.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (email is not null) return email;

        throw new NullReferenceException("Email is null");
    }

    public bool IsClientCredentials()
    {
        var isClientCredentials = httpContextAccessor.HttpContext?.User.Claims
            .Any(c => c is { Type: "gty", Value: "client-credentials" });

        return isClientCredentials ?? false;
    }
    
}