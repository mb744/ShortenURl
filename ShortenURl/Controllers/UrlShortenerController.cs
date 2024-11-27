using Microsoft.AspNetCore.Mvc;
using ShortenURl.Models;
using ShortenURl.Services;

namespace ShortenURl.Controllers
{
    

    [ApiController]
    [Route("api")]
    public class UrlShortenerController : ControllerBase
    {
        private readonly IUrlShortenerService _urlShortenerService;

        public UrlShortenerController(IUrlShortenerService urlShortenerService)
        {
            _urlShortenerService = urlShortenerService;
        }

        [HttpPost("shorten")]
        public IActionResult ShortenUrl([FromBody] ShortenUrlRequest request)
        {
            try
            {
                // Dynamically retrieve the host from the request
                var hostDomain = $"{Request.Scheme}://{Request.Host}";
                var response = _urlShortenerService.ShortenUrl(request.OriginalUrl, hostDomain);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{shortId}")]
        public IActionResult ResolveUrl(string shortId)
        {
            try
            {
                var originalUrl = _urlShortenerService.ResolveUrl(shortId);
                return Redirect(originalUrl);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Shortened URL not found.");
            }
        }
    }

}
