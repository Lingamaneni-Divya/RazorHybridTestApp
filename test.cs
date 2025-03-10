public static void AddCustomHttpClient(this IServiceCollection services, IConfiguration configuration)
{
    services.AddHttpClient<IHttpService, HttpService>()
        .AddPolicyHandler((services, _) => GetRetryPolicy<IHttpService>(services, 13))
        .ConfigurePrimaryHttpMessageHandler(() => CreateHttpClientHandler(configuration));

    services.AddHttpClient<IGraphPagingService, GraphPagingService>()
        .AddPolicyHandler((services, _) => GetRetryPolicy<IGraphPagingService>(services, 13))
        .ConfigurePrimaryHttpMessageHandler(() => CreateHttpClientHandler(configuration));
}

private static HttpClientHandler CreateHttpClientHandler(IConfiguration configuration)
{
    var proxyAddress = configuration["ProxySettings:ProxyAddress"];

    // Validate Proxy Address
    if (!Uri.TryCreate(proxyAddress, UriKind.Absolute, out _))
    {
        throw new ArgumentException($"Invalid proxy address: {proxyAddress}");
    }

    return new HttpClientHandler
    {
        Proxy = new WebProxy(proxyAddress, true, new string[]
        {
            "wellsfargo.net",
            "wellsfargo.com",
            "ent.wfb.bank.corp",
            "ent.wfb.bank.qa"
        })
    };
}