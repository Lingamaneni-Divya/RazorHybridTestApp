using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

public class ServiceExtensionsTests
{
    [Fact]
    public void AddCustomHttpClient_RegistersHttpClientsCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configValues = new Dictionary<string, string>
        {
            { "SomeConfigKey", "SomeValue" } // Just a placeholder for configuration
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        // Act
        services.AddCustomHttpClient(configuration);
        var provider = services.BuildServiceProvider();

        // Assert
        var httpService = provider.GetRequiredService<IHttpService>();
        var graphPagingService = provider.GetRequiredService<IGraphPagingService>();

        Assert.NotNull(httpService);
        Assert.NotNull(graphPagingService);
    }

    [Fact]
    public async Task GetRetryPolicy_ShouldRetry_OnTransientErrors()
    {
        // Arrange
        var serviceProviderMock = new Mock<IServiceProvider>();
        var loggerMock = new Mock<ILogger<IHttpService>>();
        serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<IHttpService>)))
                           .Returns(loggerMock.Object);

        var retryPolicy = ServiceExtensions.GetRetryPolicy<IHttpService>(serviceProviderMock.Object, 3);

        var transientErrorResponse = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        var nonTransientResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

        // Act & Assert
        var transientResult = await retryPolicy.ExecuteAsync(() => Task.FromResult(transientErrorResponse));
        Assert.Equal(HttpStatusCode.TooManyRequests, transientResult.StatusCode);

        var nonTransientResult = await retryPolicy.ExecuteAsync(() => Task.FromResult(nonTransientResponse));
        Assert.Equal(HttpStatusCode.BadRequest, nonTransientResult.StatusCode);
    }
}