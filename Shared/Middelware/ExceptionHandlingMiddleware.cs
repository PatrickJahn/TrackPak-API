using Shared.Exceptions;

namespace Shared.Middelware;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;


public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");

            await HandleExceptionAsync(context, ex, _env);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception, IHostEnvironment env)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = exception switch
        {
            
            NotFoundException => new ErrorDetails { StatusCode = (int)HttpStatusCode.NotFound, Message = exception.Message },
            ConflictException => new ErrorDetails { StatusCode = (int)HttpStatusCode.Conflict, Message = exception.Message },
            ArgumentException => new ErrorDetails { StatusCode = (int)HttpStatusCode.BadRequest, Message = exception.Message },
            KeyNotFoundException => new ErrorDetails { StatusCode = (int)HttpStatusCode.NotFound, Message = "Resource not found." },
            UnauthorizedAccessException => new ErrorDetails { StatusCode = (int)HttpStatusCode.Unauthorized, Message = "Unauthorized access." },
            _ => new ErrorDetails { StatusCode = (int)HttpStatusCode.InternalServerError, Message = "An unexpected error occurred." }
        };

        if (env.IsDevelopment())
        {
            errorResponse.StackTrace = exception.StackTrace;
        }

        response.StatusCode = errorResponse.StatusCode;
        return response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public string? StackTrace { get; set; } // Stack trace only when in development
}