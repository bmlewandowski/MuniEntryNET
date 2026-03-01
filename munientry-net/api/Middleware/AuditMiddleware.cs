using System.Diagnostics;

namespace Munientry.Api.Middleware;

/// <summary>
/// Writes one structured audit record per request to stdout (captured by the K8s container
/// runtime). Each record carries the fields needed for a legal audit trail: who (UserId from
/// the Entra OID claim), what (HTTP method + path), when (timestamp from the logger), outcome
/// (HTTP status code), and how long (elapsed milliseconds).
///
/// When Entra ID authentication is not yet enabled UserId is logged as "anonymous" — no code
/// change is needed here when auth ships; the claim will simply be present.
///
/// Log category: Munientry.Api.Middleware.AuditMiddleware
/// Controlled in appsettings.json under Logging:LogLevel:Munientry.Api.Middleware
/// </summary>
public sealed class AuditMiddleware(ILogger<AuditMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Resolve user identity — OID claim is the stable, non-reassignable Entra user ID.
        // Falls back to "anonymous" until authentication is enabled (item #1/#2 in review).
        var userId = context.User.FindFirst("oid")?.Value
            ?? context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
            ?? "anonymous";

        // W3C TraceParent is created automatically by ASP.NET Core per-request.
        var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

        var sw = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
        finally
        {
            sw.Stop();

            // Exclude health-check probes from the audit log — they are machine traffic,
            // not user actions, and would account for the large majority of log volume in K8s.
            if (!context.Request.Path.StartsWithSegments("/healthz"))
            {
                logger.LogInformation(
                    "Audit TraceId={TraceId} UserId={UserId} Method={Method} Path={Path} Status={Status} ElapsedMs={ElapsedMs}",
                    traceId,
                    userId,
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    sw.ElapsedMilliseconds);
            }
        }
    }
}
