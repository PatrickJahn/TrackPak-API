using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Monitoring;

namespace Shared.Middelware;

public class TracingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var parentContext = ActivityHelper.ExtractPropagationContextFromHttpRequest(context.Request);

        using var activity = LoggingService.activitySource.StartActivity(
            $"{context.Request.Method} {context.Request.Path}",
            ActivityKind.Server,
            parentContext.ActivityContext
        );

        LoggingService.Log
            .AddContext()
            .Information("Handling request {Method} {Path}", context.Request.Method, context.Request.Path);

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            LoggingService.Log
                .AddContext()
                .Error(ex, "Exception thrown while handling {Method} {Path}", context.Request.Method, context.Request.Path);
            throw;
        }
      
    }
}