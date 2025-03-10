[Fact]
public void AddCustomHttpClient_Should_Register_HttpClientServices()
{
    // Arrange
    var services = new ServiceCollection();
    var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

    // Act
    services.AddCustomHttpClient(configuration);
    var provider = services.BuildServiceProvider();

    // Assert
    Assert.NotNull(provider.GetService<IHttpService>());
    Assert.NotNull(provider.GetService<IGraphPagingService>());
}

[Fact]
public void AddCustomHttpClient_Should_Apply_RetryPolicy()
{
    // Arrange
    var services = new ServiceCollection();
    var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

    // Act
    services.AddCustomHttpClient(configuration);
    var provider = services.BuildServiceProvider();
    var factory = provider.GetRequiredService<IHttpClientFactory>();

    var client = factory.CreateClient(nameof(IHttpService));

    // Assert
    Assert.NotNull(client);
}

[Fact]
public void CreateHttpClientHandler_Should_ThrowException_When_InvalidProxy()
{
    // Arrange
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ProxySettings:ProxyAddress", "invalid_url" }
        })
        .Build();

    // Act & Assert
    var exception = Assert.Throws<ArgumentException>(() => CreateHttpClientHandler(configuration));
    Assert.Contains("Invalid proxy address", exception.Message);
}

[Fact]
public void CreateHttpClientHandler_Should_Return_Handler_With_Proxy()
{
    // Arrange
    var proxyAddress = "http://validproxy.com";
    var configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ProxySettings:ProxyAddress", proxyAddress }
        })
        .Build();

    // Act
    var handler = CreateHttpClientHandler(configuration);

    // Assert
    Assert.NotNull(handler);
    Assert.NotNull(handler.Proxy);
    Assert.IsType<WebProxy>(handler.Proxy);
    Assert.Equal(proxyAddress, ((WebProxy)handler.Proxy!).Address.ToString());
}