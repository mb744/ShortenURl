
using ShortenURl.Data;
using ShortenURl.Services;
using Xunit;

namespace ShortenURl.Tests
{
    public class UrlMappingRepositoryTests
    {
        private readonly IUrlMappingRepository _repository;

        public UrlMappingRepositoryTests()
        {
            _repository = new UrlMappingRepository();
        }

        [Fact]
        public void AddMapping_StoresUrlMappingSuccessfully()
        {
            // Arrange
            var shortId = "short123";
            var originalUrl = "http://example.com";

            // Act
            _repository.AddMapping(shortId, originalUrl);

            // Assert
            var result = _repository.GetOriginalUrl(shortId);
            Assert.Equal(originalUrl, result);
        }

        [Fact]
        public void GetOriginalUrl_NonExistentId_ThrowsKeyNotFoundException()
        {
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _repository.GetOriginalUrl("nonexistent"));
        }

        [Fact]
        public void Exists_ReturnsTrueForExistingId()
        {
            // Arrange
            var shortId = "short456";
            var originalUrl = "http://example.com";
            _repository.AddMapping(shortId, originalUrl);

            // Act
            var exists = _repository.Exists(shortId);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public void Exists_ReturnsFalseForNonexistentId()
        {
            // Act
            var exists = _repository.Exists("nonexistent");

            // Assert
            Assert.False(exists);
        }
    }
}
