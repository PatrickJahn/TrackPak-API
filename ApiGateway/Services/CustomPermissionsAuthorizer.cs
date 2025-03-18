using System.Linq;
using Microsoft.AspNetCore.Http;
using Ocelot.Authorization;
using Ocelot.Middleware;
using System.Collections.Generic;
using System.Security.Claims;
using Ocelot.DownstreamRouteFinder.UrlMatcher;
using Ocelot.Responses;

namespace ApiGateway.Services;


public class CustomPermissionsAuthorizer(ILogger<CustomPermissionsAuthorizer> logger) : DelegatingHandler, IClaimsAuthorizer
{
  
    
    public Response<bool> Authorize(ClaimsPrincipal user, Dictionary<string, string> routeClaimsRequirement,
        List<PlaceholderNameAndValue> urlPathPlaceholderNameAndValues)
    {


        // If no claims are required, allow access
        if (routeClaimsRequirement == null || routeClaimsRequirement.Count == 0)
        {
            return new OkResponse<bool>(true);
        }

        foreach (var requiredClaim in routeClaimsRequirement)
        {
            logger.LogWarning(requiredClaim.Key);
            logger.LogWarning(requiredClaim.Value);

            var claimType = requiredClaim.Key; // Should be "permissions"
            var requiredValues = requiredClaim.Value.Split(',').Select(x => x.Trim());

            // Get all claims of the required type (permissions)
            var userClaims = user.Claims
                .Where(c => c.Type == claimType)
                .Select(c => c.Value)
                .ToList();

            logger.LogWarning($"Found {userClaims.Count} claims for claim type {claimType}");
            // Check if the user has at least one matching permission
            if (userClaims.Any(userPermission => requiredValues.Contains(userPermission)))
            {
                return new OkResponse<bool>(true);
            }
        }

        return new OkResponse<bool>(false); // No matching permissions
    }
}