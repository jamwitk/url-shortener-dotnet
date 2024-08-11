using ShortUrl.Models;

namespace ShortUrl.Data;
using Microsoft.EntityFrameworkCore;

public class UrlShortenerContext(DbContextOptions<UrlShortenerContext> options) : DbContext(options)
{
    public DbSet<UrlShortenerModel> Urls { get; init; }
}