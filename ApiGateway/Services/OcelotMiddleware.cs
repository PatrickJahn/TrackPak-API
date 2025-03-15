namespace ApiGateway.Services;

public class OcelotHeaderMiddleware(
    IUserContextService userContextService,
    ILogger<OcelotHeaderMiddleware> logger)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = userContextService.GetUserId();
            string companyId = string.Empty;

            try
            {
                companyId = userContextService.GetCompanyId(); 
            }
            catch (NullReferenceException ex)
            {
                logger.LogWarning("Company ID is missing: {Message}", ex.Message);
            }

            if (userId != Guid.Empty)
            {
                request.Headers.Add("X-User-Id", userId.ToString());
            }

            if (!string.IsNullOrEmpty(companyId))
            {
                request.Headers.Add("X-Company-Id", companyId);
            }

            // Log the request headers
            logger.LogInformation("Added headers: X-User-Id={UserId}, X-Company-Id={CompanyId}", userId, companyId);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to inject user context headers: {Message}", ex.Message);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}