using Microsoft.AspNetCore.Mvc;
using ShortUrl.Data;

namespace ShortUrl.Controllers;

public class Redirector(UrlShortenerContext dbContext) : Controller
{
    [HttpGet("/{shortenedUrl}")]
    public IActionResult GoToSite(string shortenedUrl)
    {
        var urlItem = dbContext.Urls.FirstOrDefault(u => u.ShortenedUrl == shortenedUrl);
        if (urlItem != null)
        {
            return Redirect(urlItem.OriginalUrl);
        }

        return NotFound();
    }
}