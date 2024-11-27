using System.Collections.Concurrent;
namespace ShortenURl.Data
{
    public class UrlMappingRepository : IUrlMappingRepository
    {
        private readonly ConcurrentDictionary<string, string> _urlMappings = new();
        private readonly ConcurrentDictionary<string, string> _reverseMappings = new();

        public void AddMapping(string shortId, string originalUrl)
        {
            if (!_urlMappings.TryAdd(shortId, originalUrl))
                throw new InvalidOperationException("Short ID already exists.");

            _reverseMappings[originalUrl] = shortId; // Maintain reverse mapping
        }

        public string GetOriginalUrl(string shortId)
        {
            if (_urlMappings.TryGetValue(shortId, out var originalUrl))
            {
                return originalUrl;
            }

            throw new KeyNotFoundException("Shortened URL not found.");
        }

        public string? GetShortIdByOriginalUrl(string originalUrl)
        {
            _reverseMappings.TryGetValue(originalUrl, out var shortId);
            return shortId;
        }

        public bool Exists(string shortId)
        {
            return _urlMappings.ContainsKey(shortId);
        }
    }
}
