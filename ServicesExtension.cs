using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddIntuneHttpService(this IServiceCollection services)
    {
        var bypassList = new List<string>
        {
            "localhost",
            "127.0.0.1",
            // Add other addresses to bypass here
        };

        var proxy = new WebProxy("http://your-proxy-address:your-proxy-port")
        {
            BypassProxyOnLocal = true,
            BypassList = bypassList.ToArray()
        };

        var handler = new HttpClientHandler
        {
            Proxy = proxy
        };

        services.AddSingleton(handler);
        services.AddHttpClient<IntuneHttpService>()
            .ConfigurePrimaryHttpMessageHandler(() => handler);
    }
}