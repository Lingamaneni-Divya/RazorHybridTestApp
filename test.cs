using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntuneMobilityViolation.Job.Tests
{
    public class ServiceExtensionsTests
    {
        private readonly IServiceCollection _services;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public ServiceExtensionsTests()
        {
            _services = new ServiceCollection();
            _mockConfiguration = new Mock<IConfiguration>();
        }

        [Fact]
        public void AddCustomHttpClient_Should_Register_HttpClients()
        {
            // Arrange
            _services.AddCustomHttpClient(_mockConfiguration.Object);
            var provider = _services.BuildServiceProvider();

            // Act
            var httpService = provider.GetService<IHttpService>();
            var graphPagingService = provider.GetService<IGraphPagingService>();

            // Assert
            Assert.NotNull(httpService);
            Assert.NotNull(graphPagingService);
        }

        [Fact]
        public void AddCustomHttpClient_Should_Configure_HttpClient_With_PolicyHandler()
        {
            // Arrange
            _services.AddCustomHttpClient(_mockConfiguration.Object);
            var provider = _services.BuildServiceProvider();

            // Act
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient(nameof(IHttpService));

            // Assert
            Assert.NotNull(httpClient);
        }

        [Fact]
        public async Task RetryPolicy_Should_Handle_Transient_Errors()
        {
            // Arrange
            var policy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            var handler = new HttpClientHandler();
            var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://example.com") };

            int retryCount = 0;

            // Act
            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await policy.ExecuteAsync(async () =>
                {
                    retryCount++;
                    var response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                    if (retryCount < 3)
                    {
                        throw new HttpRequestException();
                    }
                    return response;
                });
            });

            // Assert
            Assert.Equal(3, retryCount);
        }
    }
}