using System.Collections.Generic;
namespace ShortenURl.Data
{
    public interface IUrlMappingRepository
    {
        void AddMapping(string shortId, string originalUrl);
        string GetOriginalUrl(string shortId);
        bool Exists(string shortId);
    }
}
