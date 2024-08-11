using System.Collections.Concurrent;
using System.Net;

namespace ShortUrl.Middlewares;

public class RateLimitingMiddleware(RequestDelegate next)
{
    private static readonly ConcurrentDictionary<string, (int Count, DateTime ResetTime)> RequestCounts = new();
    private const int Limit = 100;
    private static readonly TimeSpan TimeWindow = TimeSpan.FromMinutes(1);
    
    public async Task InvokeAsync(HttpContext context)
    {
        // var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? "unknown";
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        
        if (!RequestCounts.ContainsKey(ipAddress))
        {
            RequestCounts[ipAddress] = (0, DateTime.UtcNow.Add(TimeWindow));
        }

        var (count, resetTime) = RequestCounts[ipAddress];
        if (DateTime.UtcNow > resetTime)
        {
            RequestCounts[ipAddress] = (0, DateTime.UtcNow.Add(TimeWindow));
            count = 0;
        }

        if (count >= Limit)
        {
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            await context.Response.WriteAsync("Too many requests."+ ipAddress+" !!");
            return;
        }

        RequestCounts[ipAddress] = (count + 1, resetTime);
        await next(context);
    }
}