using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Middleware
{
    public class GatewayRestrictionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string[] _allowedGateways;

        public GatewayRestrictionMiddleware(RequestDelegate next,  IOptions<GatewaySettings> options)
        {
            _next = next;
            _allowedGateways = options.Value.AllowedGateways ?? Array.Empty<string>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var remoteIpAddress = context.Connection.RemoteIpAddress?.ToString();

            if (!_allowedGateways.Contains(remoteIpAddress))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Access denied. Requests must come through API Gateway.");
                return;
            }

            await _next(context);
        }
    }

    public class GatewaySettings
    {
        public string[] AllowedGateways { get; set; } = Array.Empty<string>();
    }
}