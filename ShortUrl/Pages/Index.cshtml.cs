using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShortUrl.Data;
using ShortUrl.Models;

namespace ShortUrl.Pages
{
    public class IndexModel : PageModel
    {
        private readonly UrlShortenerContext _dbContext;
        private readonly IConfiguration _configuration;

        public IndexModel(UrlShortenerContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [BindProperty] public string OriginalUrl { get; set; }
         public string ShortenedUrl { get; set; }

        public void OnGet(string shortenedUrl = "")
        {
            if(TempData.TryGetValue("ShortenedUrl", out var value))
            {
                if (value is not null)
                {
                    ShortenedUrl = value.ToString() ?? string.Empty;
                }
            }
            else
            {
                ShortenedUrl = shortenedUrl;
            }

            OriginalUrl = "";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var longUrl = Request.Form["OriginalUrl"];
            
            if (ModelState.IsValid && !string.IsNullOrEmpty(longUrl))
            {
                var baseUrl = Request.Scheme + "://" + Request.Host;
                var shortenedPath = Guid.NewGuid().ToString().Substring(0, 6);
                ShortenedUrl = $"{baseUrl}/{shortenedPath}";

                var urlEntry = new UrlShortenerModel
                {
                    OriginalUrl = longUrl.ToString(),
                    ShortenedUrl = shortenedPath
                };
                
                TempData["ShortenedUrl"] = ShortenedUrl;

                _dbContext.Urls.Add(urlEntry);
                await _dbContext.SaveChangesAsync();
                
                
                return RedirectToPage("/Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid URL submission.");
            }

            return Page();
        }
    }
}