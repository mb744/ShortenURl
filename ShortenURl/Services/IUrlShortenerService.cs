using ShortenURl.Models;

namespace ShortenURl.Services
{
    public interface IUrlShortenerService
    {
        ShortenUrlResponse ShortenUrl(string originalUrl, string hostDomain);
        string ResolveUrl(string shortId);
    }

}
