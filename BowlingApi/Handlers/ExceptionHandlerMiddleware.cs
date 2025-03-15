using System.Net;
using System.Text.Json;

namespace BowlingApi.Handlers;

public class ExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlerMiddleware> logger,
    IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var statusCode = HttpStatusCode.InternalServerError; // 500 by default
        var errorMessage = "An unexpected error occurred";
        
        // Customize status code and message based on exception type
        if (exception is ArgumentException)
        {
            statusCode = HttpStatusCode.BadRequest; // 400
            errorMessage = exception.Message;
        }
        else if (exception is InvalidOperationException)
        {
            statusCode = HttpStatusCode.BadRequest; // 400
            errorMessage = exception.Message;
        }
        else if (exception is KeyNotFoundException)
        {
            statusCode = HttpStatusCode.NotFound; // 404
            errorMessage = exception.Message;
        }
        
        context.Response.StatusCode = (int)statusCode;
        
        var response = new
        {
            status = (int)statusCode,
            message = errorMessage,
            // Only include detailed error info in development
            detail = environment.IsDevelopment() ? exception.StackTrace : null
        };
        
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        var json = JsonSerializer.Serialize(response, options);
        
        return context.Response.WriteAsync(json);
    }
}

// Extension method for cleaner startup configuration
public static class ExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}