using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

public static async Task<string> GetAsync(string baseUrl, Dictionary<string, string> parameters)
{
    using (HttpClient client = new HttpClient())
    {
        var uriBuilder = new UriBuilder(baseUrl);
        var query = new FormUrlEncodedContent(parameters).ReadAsStringAsync().Result;
        uriBuilder.Query = query;

        HttpResponseMessage response = await client.GetAsync(uriBuilder.Uri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

public static async Task<string> PostAsync(string baseUrl, Dictionary<string, string> parameters, string jsonContent)
{
    using (HttpClient client = new HttpClient())
    {
        var uriBuilder = new UriBuilder(baseUrl);
        var query = new FormUrlEncodedContent(parameters).ReadAsStringAsync().Result;
        uriBuilder.Query = query;

        HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(uriBuilder.Uri, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

public static async Task<string> PutAsync(string baseUrl, Dictionary<string, string> parameters, string jsonContent)
{
    using (HttpClient client = new HttpClient())
    {
        var uriBuilder = new UriBuilder(baseUrl);
        var query = new FormUrlEncodedContent(parameters).ReadAsStringAsync().Result;
        uriBuilder.Query = query;

        HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PutAsync(uriBuilder.Uri, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

public static async Task<string> DeleteAsync(string baseUrl, Dictionary<string, string> parameters)
{
    using (HttpClient client = new HttpClient())
    {
        var uriBuilder = new UriBuilder(baseUrl);
        var query = new FormUrlEncodedContent(parameters).ReadAsStringAsync().Result;
        uriBuilder.Query = query;

        HttpResponseMessage response = await client.DeleteAsync(uriBuilder.Uri);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}