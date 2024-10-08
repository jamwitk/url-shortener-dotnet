using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using ShortUrl.Data;
using ShortUrl.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<UrlShortenerContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("UrlShortenerContext")));


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        await context.Response.WriteAsync("Status: " + report.Status + "\n");
    }
});

app.UseMiddleware<RateLimitingMiddleware>();

app.MapRazorPages();
app.MapControllers();

app.Run();