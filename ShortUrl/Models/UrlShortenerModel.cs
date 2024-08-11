using System.ComponentModel.DataAnnotations;

namespace ShortUrl.Models;

public class UrlShortenerModel
{
    public int Id { get; set; }

    [Required]
    public string OriginalUrl { get; set; }

    [Required]
    [MaxLength(10)] 
    public string ShortenedUrl { get; set; }
    
    }
    