using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

public class GraphAuthHelper
{
    private readonly IConfidentialClientApplication _confidentialClientApp;

    public GraphAuthHelper(string tenantId, string clientId, string clientSecret)
    {
        _confidentialClientApp = ConfidentialClientApplicationBuilder
            .Create(clientId)
            .WithClientSecret(clientSecret)
            .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
            .Build();
    }

    public async Task<string> GetAccessTokenAsync()
    {
        string[] scopes = new string[] { "https://graph.microsoft.com/.default" };

        try
        {
            var authResult = await _confidentialClientApp.AcquireTokenForClient(scopes).ExecuteAsync();
            return authResult.AccessToken;
        }
        catch (MsalServiceException ex)
        {
            // Handle exception
            throw new Exception($"Failed to acquire token. Error: {ex.Message}");
        }
    }
}