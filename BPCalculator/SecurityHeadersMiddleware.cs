using System.Threading.Tasks;
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
                // The headers are applied from an OnStarting callback rather than
                // inline. The callback runs immediately before Kestrel flushes the
                // response, which is the only point at which the collection is still
                // writable *and* every header the server itself adds is already
                // present. Mutating the collection after await next() throws
                // "Headers are read-only, response has already started" and truncates
                // the response body.
                context.Response.OnStarting(() =>
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

                    // ZAP: Server Leaks Information. Kestrel is already configured with
                    // AddServerHeader = false and IIS strips X-Powered-By via web.config;
                    // this is defence in depth for any host that adds them anyway.
                    headers.Remove("Server");
                    headers.Remove("X-Powered-By");

                    return Task.CompletedTask;
                });

                await next();
            });
    }
}
