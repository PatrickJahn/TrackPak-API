using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Shared.Middelware
{
    public class GatewayRestrictionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _allowedGateways;
        private readonly ILogger<GatewayRestrictionMiddleware> _logger;

        public GatewayRestrictionMiddleware(RequestDelegate next, IOptions<GatewaySettings> options, ILogger<GatewayRestrictionMiddleware> logger)
        {
            _next = next;
            _allowedGateways = options.Value.AllowedGateways ?? Array.Empty<string>();
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestOrigin = context.Request.Headers["X-Forwarded-Host"].FirstOrDefault() ??
                                context.Request.Headers["Host"].ToString(); // Fallback

            _logger.LogInformation("Incoming request from: {RequestOrigin}", requestOrigin);

            // Validate the request comes from an allowed gateway
            if (string.IsNullOrEmpty(requestOrigin) || !_allowedGateways.Any(gateway => requestOrigin.Contains(gateway)))
            {
                _logger.LogWarning("Access denied. Origin: {RequestOrigin}", requestOrigin);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync($"Access denied. Requests must come through API Gateway. Allowed: {string.Join(", ", _allowedGateways)}");
                return;
            }

            _logger.LogInformation("Request from allowed gateway: {RequestOrigin}", requestOrigin);
            await _next(context);
        }
    }

    public class GatewaySettings
    {
        public string[] AllowedGateways { get; set; } = Array.Empty<string>();
    }
}
