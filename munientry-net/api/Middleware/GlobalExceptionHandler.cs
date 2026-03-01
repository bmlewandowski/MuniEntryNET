using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Munientry.Api.Middleware;

/// <summary>
/// Catches any unhandled exception that escapes an endpoint handler, logs the full details
/// server-side (including message and stack trace), and returns a sanitized RFC 7807
/// Problem Details response that contains only the trace ID.
///
/// This prevents internal error details — template paths, exception types, stack frames —
/// from surfacing to clients (security review item #5).
///
/// Usage pattern in endpoint files:
///   catch (Exception) { throw; }
///   — or — remove the try/catch entirely.
/// The exception propagates here; the client sees only the trace ID.
///
/// Registration: services.AddExceptionHandler&lt;GlobalExceptionHandler&gt;()
///               services.AddProblemDetails()
/// Pipeline:     app.UseExceptionHandler()  (before UseMiddleware&lt;AuditMiddleware&gt;)
/// </summary>
internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? httpContext.TraceIdentifier;

        // Log the full exception details so developers and ops can diagnose failures
        // from the server-side log / K8s log aggregator — never from the HTTP response.
        logger.LogError(
            exception,
            "Unhandled exception TraceId={TraceId} Method={Method} Path={Path} ExceptionType={ExceptionType}",
            traceId,
            httpContext.Request.Method,
            httpContext.Request.Path,
            exception.GetType().Name);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // Return only the trace ID — no message, no stack, no template paths.
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred processing your request.",
            Detail = $"Contact support with trace ID: {traceId}",
        };
        problem.Extensions["traceId"] = traceId;

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true; // exception handled — do not rethrow
    }
}
