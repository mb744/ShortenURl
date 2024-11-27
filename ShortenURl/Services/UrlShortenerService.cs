namespace ShortenURl.Services
{
    using ShortenURl.Data;
    using ShortenURl.Models;
    using System.Collections.Concurrent;
    using System.Text.RegularExpressions;

    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly IUrlMappingRepository _repository;

        public UrlShortenerService(IUrlMappingRepository repository)
        {
            _repository = repository;
        }

        public ShortenUrlResponse ShortenUrl(string originalUrl, string hostDomain)
        {
            if (!IsValidUrl(originalUrl))
                throw new ArgumentException("Invalid URL");

            // Check for existing URL
            var existingShortId = _repository.GetShortIdByOriginalUrl(originalUrl);
            if (existingShortId != null)
            {
                return new ShortenUrlResponse
                {
                    ShortId = existingShortId,
                    ShortUrl = $"{hostDomain}/{existingShortId}"
                };
            }

            var shortId = Guid.NewGuid().ToString("N").Substring(0, 8);

            if (_repository.Exists(shortId))
            {
                throw new InvalidOperationException("Short ID conflict. Try again.");
            }

            _repository.AddMapping(shortId, originalUrl);

            return new ShortenUrlResponse
            {
                ShortId = shortId,
                ShortUrl = $"{hostDomain}/{shortId}"
            };
        }

        public string ResolveUrl(string shortId)
        {
            return _repository.GetOriginalUrl(shortId);
        }

        private bool IsValidUrl(string url)
        {
            var regex = new Regex(@"^(https?://)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(:\d+)?(/.*)?$");
            return regex.IsMatch(url);
        }
    }

}
