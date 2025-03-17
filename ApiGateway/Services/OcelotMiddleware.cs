namespace ApiGateway.Services
{
    public class OcelotHeaderMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Example: Add a custom header to forwarded requests
            if (context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
                var companyId = context.User.Claims.FirstOrDefault(c => c.Type == "company_id")?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    context.Request.Headers["X-User-Id"] = userId;
                }
                if (!string.IsNullOrEmpty(companyId))
                {
                    context.Request.Headers["X-Company-Id"] = companyId;
                }
            }

            await next(context); // Pass request to next middleware
        }
    }
}