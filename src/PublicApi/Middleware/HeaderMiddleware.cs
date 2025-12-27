using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopWeb.PublicApi.Middleware;

public class HeaderMiddleware
{
    private readonly RequestDelegate _next;
    public HeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context, IAppLogger<HeaderMiddleware> logger, IConfiguration config)
    {
        // Add custom headers to the response
        context.Response.Headers.Append("X-Custom-Header", "MyHeaderValue");
        context.Response.Headers.Append("X-Powered-By", "eShopWeb");
        string value = config["brain:front"] ?? string.Empty;
        context.Response.Headers.Append("config-validator", value);
        logger.LogInformation("Logging from HeaderMiddleware: {value}", value);

        // Call the next middleware in the pipeline
        await _next(context);
    }
}
