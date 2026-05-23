using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using Xunit;

namespace CashAppBalanceChecker.Tests
{
    /// <summary>
    /// Unit tests for the CashAppBalanceService class.
    /// </summary>
    public class CashAppBalanceServiceTests
    {
        private readonly ILogger _logger;

        public CashAppBalanceServiceTests()
        {
            _logger = NullLogger.Instance;
        }

        [Fact]
        public async Task GetBalanceAsync_ValidResponse_ReturnsBalance()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"balance\": 150.75, \"currency\": \"USD\", \"updated_at\": \"2023-10-01T12:00:00Z\"}")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var service = new CashAppBalanceService("test_token", "test_client", _logger);

            // Use reflection to inject mock HttpClient (for testing purposes only)
            typeof(CashAppBalanceService)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(service, httpClient);

            // Act
            var balance = await service.GetBalanceAsync();

            // Assert
            Assert.Equal(150.75m, balance);
        }

        [Fact]
        public async Task GetBalanceAsync_ApiError_ThrowsHttpRequestException()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Content = new StringContent("{\"error\": \"Invalid token\"}")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var service = new CashAppBalanceService("bad_token", "test_client", _logger);

            typeof(CashAppBalanceService)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(service, httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetBalanceAsync());
        }

        [Fact]
        public async Task GetBalanceAsync_InvalidJson_ThrowsInvalidOperationException()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("not valid json")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var service = new CashAppBalanceService("test_token", "test_client", _logger);

            typeof(CashAppBalanceService)
                .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(service, httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetBalanceAsync());
        }

        [Fact]
        public void Constructor_NullApiToken_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CashAppBalanceService(null!, "client", _logger));
        }

        [Fact]
        public void Constructor_NullClientId_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CashAppBalanceService("token", null!, _logger));
        }

        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CashAppBalanceService("token", "client", null!));
        }
    }
}
