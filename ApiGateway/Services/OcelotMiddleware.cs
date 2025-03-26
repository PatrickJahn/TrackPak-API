using Newtonsoft.Json;

namespace ApiGateway.Services
{
    public class OcelotHeaderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<OcelotHeaderMiddleware> _logger;

        public OcelotHeaderMiddleware(RequestDelegate next,  ILogger<OcelotHeaderMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
      
        
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.Claims
                    .FirstOrDefault(c => c.Type == "https://trackpak.dk/user_id")?.Value;

                var companyId = context.User.Claims
                    .FirstOrDefault(c => c.Type == "https://trackpak.dk/company_id")?.Value;

                var role = context.User.Claims
                    .FirstOrDefault(c => c.Type == "https://trackpak.dk/role")?.Value;

                _logger.LogInformation($"UserId: {userId}, CompanyId: {companyId}, Role: {role}");
                
                if (!string.IsNullOrEmpty(userId))
                {
                    context.Request.Headers["X-User-Id"] = userId;
                }
                if (!string.IsNullOrEmpty(companyId))
                {
                    context.Request.Headers["X-Company-Id"] = companyId;
                }
                if (!string.IsNullOrEmpty(role))
                {
                    context.Request.Headers["X-Role"] = role;
                }
            }

            await _next(context);
        }
    }
}