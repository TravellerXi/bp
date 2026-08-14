using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BPCalculator
{
    /// <summary>
    /// Response headers that close the alerts an OWASP ZAP baseline scan raises
    /// against a default ASP.NET Core Razor Pages site.
    /// </summary>
    public static class SecurityHeadersMiddleware
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app) =>
            app.Use(async (context, next) =>
            {
                var headers = context.Response.Headers;

                // ZAP: Content Security Policy (CSP) Header Not Set
                headers["Content-Security-Policy"] =
                    "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; " +
                    "object-src 'none'; frame-ancestors 'none'; base-uri 'self'; form-action 'self'";

                // ZAP: Missing Anti-clickjacking Header
                headers["X-Frame-Options"] = "DENY";

                // ZAP: X-Content-Type-Options Header Missing
                headers["X-Content-Type-Options"] = "nosniff";

                // ZAP: Permissions Policy Header Not Set
                headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";

                headers["Referrer-Policy"] = "no-referrer";

                // ZAP: Re-examine Cache-control Directives. The result page reflects
                // submitted health data, so it must not be cached by shared proxies.
                headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
                headers["Pragma"] = "no-cache";

                await next();

                // ZAP: Server Leaks Information. Kestrel and IIS append these while the
                // response is being written, so they can only be stripped on the way out.
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-Powered-By");
            });
    }
}
