using Microsoft.AspNetCore.Mvc;

namespace ShortUrl.Controllers;

public class HealthController : ControllerBase
{
    [HttpGet("/ping")]
    public IActionResult Get()
    {
        return Ok("pong");
    }
}