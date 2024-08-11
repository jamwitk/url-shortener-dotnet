using System.Net;
using Microsoft.AspNetCore.TestHost;
using Xunit;
using Xunit.Abstractions;

namespace ShortUrl.Middlewares;

public class RateLimitingMiddlewareTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public RateLimitingMiddlewareTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private TestServer CreateServer()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services => { })
            .Configure(app =>
            {
                app.UseMiddleware<RateLimitingMiddleware>();
                app.Run(async context => { await context.Response.WriteAsync("Hello World"); });
            });

        return new TestServer(builder);
    }

    [Fact]
    public async Task RateLimitingMiddleware_AllowsRequestsUnderLimit()
    {
        var server = CreateServer();
        var client = server.CreateClient();
        client.DefaultRequestHeaders.Add("X-Forwarded-For", "192.168.1.1");

        for (int i = 0; i < 100; i++)
        {
            var response = await client.PostAsync("/", new StringContent(""));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    public async Task RateLimitingMiddleware_BlocksRequestsOverLimit()
    {
        var server = CreateServer();
        var client = server.CreateClient();
        client.DefaultRequestHeaders.Add("X-Forwarded-For", "192.168.2.1");

        for (int i = 0; i < 101; i++)
        {
            var response = await client.PostAsync("/", new StringContent(""));
            if (i < 100)
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
            else
            {
                Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
            }
        }
    }

    [Fact]
    public async Task RateLimitingMiddleware_ResetsCountAfterTimeWindow()
    {
        var server = CreateServer();
        var client = server.CreateClient();
        client.DefaultRequestHeaders.Add("X-Forwarded-For", "192.168.3.1");

        for (int i = 0; i < 100; i++)
        {
            var response = await client.PostAsync("/", new StringContent(""));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        await Task.Delay(TimeSpan.FromMinutes(1));

        var responseAfterWindow = await client.PostAsync("/", new StringContent(""));
        Assert.Equal(HttpStatusCode.OK, responseAfterWindow.StatusCode);
    }

    [Fact]
    public async Task RateLimitingMiddleware_AllowsRequestsFromDifferentIP()
    {
        var server = CreateServer();
        var client = server.CreateClient();

        // Simulate 100 requests from the first IP
        for (int i = 0; i < 100; i++)
        {
            client.DefaultRequestHeaders.Remove("X-Forwarded-For");
            client.DefaultRequestHeaders.Add("X-Forwarded-For", "192.168.4.1");
            var response = await client.PostAsync("/", new StringContent(""));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // Simulate a request from a different IP
        client.DefaultRequestHeaders.Remove("X-Forwarded-For");
        client.DefaultRequestHeaders.Add("X-Forwarded-For", "192.168.5.1");
        var responseFromDifferentIP = await client.PostAsync("/", new StringContent(""));
        Assert.Equal(HttpStatusCode.OK, responseFromDifferentIP.StatusCode);
    }
    [Fact]
    public async Task RateLimitingMiddleware_BlocksRequestsWhenIPAddressIsUnknown()
    {
        var server = CreateServer();
        var client = server.CreateClient();
        client.DefaultRequestHeaders.Add("X-Forwarded-For", "unknown");

        for (int i = 0; i < 101; i++)
        {
            var response = await client.PostAsync("/", new StringContent(""));
            if (i < 100)
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
            else
            {
                Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
            }
        }
    }
}