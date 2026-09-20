using App.infra.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace App.APIs.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly ILogPublisher _logPublisher;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, ILogPublisher logPublisher)
    {
        _next = next;
        _logger = logger;
        _logPublisher = logPublisher;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.TraceIdentifier;
        var machineName = Environment.MachineName;
        var threadId = Thread.CurrentThread.ManagedThreadId.ToString();

        // Log request start
        var requestLog = new LogMessage
        {
            Level = "Information",
            Message = $"HTTP {context.Request.Method} {context.Request.Path} started",
            Source = "RequestLoggingMiddleware",
            CorrelationId = correlationId,
            UserId = context.User?.Identity?.Name,
            RequestPath = context.Request.Path,
            RequestMethod = context.Request.Method,
            Timestamp = DateTime.UtcNow,
            MachineName = machineName,
            ThreadId = threadId,
            Properties = new Dictionary<string, object>
            {
                ["QueryString"] = context.Request.QueryString.ToString(),
                ["ContentType"] = context.Request.ContentType ?? "none",
                ["ContentLength"] = context.Request.ContentLength ?? 0
            }
        };

        await _logPublisher.PublishLogAsync(requestLog);

        try
        {
            await _next(context);
            
            stopwatch.Stop();

            // Log request completion
            var responseLog = new LogMessage
            {
                Level = context.Response.StatusCode >= 400 ? "Warning" : "Information",
                Message = $"HTTP {context.Request.Method} {context.Request.Path} completed with {context.Response.StatusCode} in {stopwatch.ElapsedMilliseconds}ms",
                Source = "RequestLoggingMiddleware",
                CorrelationId = correlationId,
                UserId = context.User?.Identity?.Name,
                RequestPath = context.Request.Path,
                RequestMethod = context.Request.Method,
                Timestamp = DateTime.UtcNow,
                MachineName = machineName,
                ThreadId = threadId,
                Properties = new Dictionary<string, object>
                {
                    ["StatusCode"] = context.Response.StatusCode,
                    ["ElapsedMilliseconds"] = stopwatch.ElapsedMilliseconds
                }
            };

            await _logPublisher.PublishLogAsync(responseLog);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log exception
            var errorLog = new LogMessage
            {
                Level = "Error",
                Message = $"HTTP {context.Request.Method} {context.Request.Path} failed after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}",
                Exception = ex.ToString(),
                Source = "RequestLoggingMiddleware",
                CorrelationId = correlationId,
                UserId = context.User?.Identity?.Name,
                RequestPath = context.Request.Path,
                RequestMethod = context.Request.Method,
                Timestamp = DateTime.UtcNow,
                MachineName = machineName,
                ThreadId = threadId,
                Properties = new Dictionary<string, object>
                {
                    ["ElapsedMilliseconds"] = stopwatch.ElapsedMilliseconds,
                    ["ExceptionType"] = ex.GetType().Name
                }
            };

            await _logPublisher.PublishLogAsync(errorLog);
            throw;
        }
    }
}