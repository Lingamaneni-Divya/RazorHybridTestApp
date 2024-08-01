{
  "ProxySettings": {
    "ProxyAddress": "http://your-proxy-address:your-proxy-port",
    "ProxyBypassList": [ "localhost", "127.0.0.1" ]
  }
}
public class ProxySettings
{
    public string ProxyAddress { get; set; }
    public List<string> ProxyBypassList { get; set; }
}
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddIntuneHttpService(this IServiceCollection services, IConfiguration configuration)
    {
        var proxySettings = configuration.GetSection("ProxySettings").Get<ProxySettings>();

        var proxy = new WebProxy(proxySettings.ProxyAddress)
        {
            BypassProxyOnLocal = true,
            BypassList = proxySettings.ProxyBypassList.ToArray(),
            Credentials = CredentialCache.DefaultNetworkCredentials
        };

        var handler = new HttpClientHandler
        {
            Proxy = proxy,
            UseDefaultCredentials = true,
            Credentials = CredentialCache.DefaultNetworkCredentials
        };

        services.AddSingleton(handler);
        services.AddHttpClient<IntuneHttpService>()
            .ConfigurePrimaryHttpMessageHandler(() => handler);
    }
}