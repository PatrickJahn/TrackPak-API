namespace Shared.Extensions;

using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

public static class HttpContextUserClaimsExtensions
{
    /// <summary>
    /// Retrieves the User ID from the request headers.
    /// </summary>
    public static Guid? GetUserId(this HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue("X-User-Id", out var userIdString) &&
            Guid.TryParse(userIdString, out var userId))
        {
            return userId;
        }
        return null; // Return null if not found or invalid
    }

    /// <summary>
    /// Retrieves the Company ID from the request headers.
    /// </summary>
    public static Guid? GetCompanyId(this HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue("X-Company-Id", out var companyIdString) &&
            Guid.TryParse(companyIdString, out var companyId))
        {
            return companyId;
        }
        return null; // Return null if not found or invalid
    }
    /// <summary>
    /// Retrieves the Employee ID from the request headers.
    /// </summary>
    public static Guid? GetEmployeeId(this HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue("X-Employee-Id", out var employeeIdString) &&
            Guid.TryParse(employeeIdString, out var employeeId))
        {
            return employeeId;
        }
        return null; // Return null if not found or invalid
    }
    /// <summary>
    /// Retrieves the single role from the request headers.
    /// </summary>
    public static string? GetRole(this HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue("X-Role", out var role))
        {
            return role.ToString().Trim();
        }

        return null;
    }

    /// <summary>
    /// Checks if the user has the required role.
    /// </summary>
    public static bool HasRole(this HttpContext httpContext, string requiredRole)
    {
        var role = httpContext.GetRole();
        return string.Equals(role, requiredRole, StringComparison.OrdinalIgnoreCase);
    }
}