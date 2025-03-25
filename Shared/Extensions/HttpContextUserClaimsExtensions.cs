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
        if (httpContext.Request.Headers.TryGetValue("X-Employee_Id", out var employeeIdString) &&
            Guid.TryParse(employeeIdString, out var employeeId))
        {
            return employeeId;
        }
        return null; // Return null if not found or invalid
    }
    /// <summary>
    /// Retrieves all roles from the request headers (comma-separated).
    /// </summary>
    public static List<string> GetRoles(this HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue("X-Roles", out var rolesString))
        {
            return rolesString
                .ToString()
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }

        return new List<string>(); // Return empty list if not found
    }

    /// <summary>
    /// Helper to check if the user has a specific role.
    /// </summary>
    public static bool HasRole(this HttpContext httpContext, string role)
    {
        var roles = httpContext.GetRoles();
        return roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
    
    public static class RoleAsString
    {
        public const string Customer = "Customer";
        public const string CompanyAdmin = "CompanyAdmin"; 
        public const string Driver = "Driver"; 
        public const string SystemAdmin = "SystemAdmin";
    }
}