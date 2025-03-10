using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace IntuneMobilityViolation.Job.Tests
{
    public class ServiceExtensionsTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly IServiceCollection _services;

        public ServiceExtensionsTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _services = new ServiceCollection();
        }

        [Fact]
        public void AddCustomHttpClient_ShouldRegisterHttpClients()
        {
            // Arrange
            _mockConfig.Setup(c => c["ProxySettings:ProxyAddress"]).Returns("http://valid-proxy.com");

            // Act
            _services.AddCustomHttpClient(_mockConfig.Object);
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            var httpClient1 = serviceProvider.GetService<IHttpClientFactory>();
            Assert.NotNull(httpClient1); // Ensure HttpClientFactory is registered

            var httpClient2 = serviceProvider.GetService<IHttpService>();
            Assert.NotNull(httpClient2); // Ensure IHttpService is registered

            var httpClient3 = serviceProvider.GetService<IGraphPagingService>();
            Assert.NotNull(httpClient3); // Ensure IGraphPagingService is registered
        }

        [Fact]
        public void CreateHttpClientHandler_ShouldReturnHandlerWithProxy()
        {
            // Arrange
            _mockConfig.Setup(c => c["ProxySettings:ProxyAddress"]).Returns("http://valid-proxy.com");

            // Act
            var handler = typeof(ServiceExtensions)
                .GetMethod("CreateHttpClientHandler", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .Invoke(null, new object[] { _mockConfig.Object }) as HttpClientHandler;

            // Assert
            Assert.NotNull(handler);
            Assert.NotNull(handler.Proxy);
            Assert.IsType<WebProxy>(handler.Proxy);

            var webProxy = (WebProxy)handler.Proxy;
            Assert.Equal("http://valid-proxy.com", webProxy.Address.ToString());
            Assert.Contains("wellsfargo.net", webProxy.BypassList);
            Assert.Contains("ent.wfb.bank.qa", webProxy.BypassList);
        }

        [Fact]
        public void CreateHttpClientHandler_ShouldThrowExceptionForInvalidProxy()
        {
            // Arrange
            _mockConfig.Setup(c => c["ProxySettings:ProxyAddress"]).Returns("invalid-url");

            // Act & Assert
            var exception = Assert.Throws<TargetInvocationException>(() =>
            {
                typeof(ServiceExtensions)
                    .GetMethod("CreateHttpClientHandler", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                    .Invoke(null, new object[] { _mockConfig.Object });
            });

            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Contains("Invalid proxy address", exception.InnerException.Message);
        }
    }
}