using Moq;
using ShortenURl.Data;
using ShortenURl.Services;
using System;
using Xunit;

namespace ShortenURl.Tests
{
    public class UrlShortenerServiceTests
    {
        private readonly Mock<IUrlMappingRepository> _mockRepository;
        private readonly IUrlShortenerService _service;

        public UrlShortenerServiceTests()
        {
            _mockRepository = new Mock<IUrlMappingRepository>();
            _service = new UrlShortenerService(_mockRepository.Object);
        }

        [Fact]
        public void ShortenUrl_ValidUrl_ReturnsShortUrl()
        {
            // Arrange
            var originalUrl = "http://example.com";
            var hostDomain = "http://localhost";
            var shortId = "abc123";

            _mockRepository.Setup(repo => repo.Exists(It.IsAny<string>())).Returns(false);
            _mockRepository.Setup(repo => repo.AddMapping(shortId, originalUrl));

            // Act
            var result = _service.ShortenUrl(originalUrl, hostDomain);

            // Assert
            Assert.StartsWith(hostDomain, result.ShortUrl);
            Assert.NotNull(result.ShortId);

            _mockRepository.Verify(repo => repo.AddMapping(It.IsAny<string>(), originalUrl), Times.Once);
        }

        [Fact]
        public void ShortenUrl_InvalidUrl_ThrowsArgumentException()
        {
            // Arrange
            var invalidUrl = "invalid_url";
            var hostDomain = "http://localhost";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _service.ShortenUrl(invalidUrl, hostDomain));
        }

        [Fact]
        public void ResolveUrl_ExistingShortId_ReturnsOriginalUrl()
        {
            // Arrange
            var shortId = "abc123";
            var originalUrl = "http://example.com";

            _mockRepository.Setup(repo => repo.GetOriginalUrl(shortId)).Returns(originalUrl);

            // Act
            var result = _service.ResolveUrl(shortId);

            // Assert
            Assert.Equal(originalUrl, result);
            _mockRepository.Verify(repo => repo.GetOriginalUrl(shortId), Times.Once);
        }

        [Fact]
        public void ResolveUrl_NonExistentShortId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var shortId = "nonexistent";

            _mockRepository.Setup(repo => repo.GetOriginalUrl(shortId)).Throws<KeyNotFoundException>();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _service.ResolveUrl(shortId));
            _mockRepository.Verify(repo => repo.GetOriginalUrl(shortId), Times.Once);
        }
    }
}
