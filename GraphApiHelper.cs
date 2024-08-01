using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

public class HttpService
{
    private readonly HttpClient _httpClient;

    public HttpService(HttpService httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetAsync(string baseUrl, Dictionary<string, string> parameters = null)
    {
        var finalUrl = BuildUrlWithParameters(baseUrl, parameters);

        var response = await _httpClient.GetAsync(finalUrl);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PostAsync(string baseUrl, HttpContent content, Dictionary<string, string> parameters = null)
    {
        var finalUrl = BuildUrlWithParameters(baseUrl, parameters);

        var response = await _httpClient.PostAsync(finalUrl, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> PutAsync(string baseUrl, HttpContent content, Dictionary<string, string> parameters = null)
    {
        var finalUrl = BuildUrlWithParameters(baseUrl, parameters);

        var response = await _httpClient.PutAsync(finalUrl, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> DeleteAsync(string baseUrl, Dictionary<string, string> parameters = null)
    {
        var finalUrl = BuildUrlWithParameters(baseUrl, parameters);

        var response = await _httpClient.DeleteAsync(finalUrl);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private string BuildUrlWithParameters(string baseUrl, Dictionary<string, string> parameters)
    {
        var uriBuilder = new UriBuilder(baseUrl);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                query[param.Key] = param.Value;
            }
        }

        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
}