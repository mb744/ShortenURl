using System.Collections.Concurrent;
namespace ShortenURl.Data
{
    public class UrlMappingRepository : IUrlMappingRepository
    {
        private readonly ConcurrentDictionary<string, string> _urlMappings = new();

        public void AddMapping(string shortId, string originalUrl)
        {
            if (!_urlMappings.TryAdd(shortId, originalUrl))
            {
                throw new InvalidOperationException("Short ID already exists.");
            }
        }

        public string GetOriginalUrl(string shortId)
        {
            if (_urlMappings.TryGetValue(shortId, out var originalUrl))
            {
                return originalUrl;
            }

            throw new KeyNotFoundException("Shortened URL not found.");
        }

        public bool Exists(string shortId)
        {
            return _urlMappings.ContainsKey(shortId);
        }
    }
}
