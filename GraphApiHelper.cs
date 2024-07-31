public class GraphApiResponse<T>
{
    [JsonPropertyName("value")]
    public List<T> Value { get; set; }

    [JsonPropertyName("@odata.nextLink")]
    public string NextLink { get; set; }
}

public class GraphApiHelper
{
    private readonly HttpService _httpService;

    public GraphApiHelper(HttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<List<T>> GetAllDataAsync<T>(string baseUrl, Dictionary<string, string> parameters = null)
    {
        var allData = new List<T>();
        string url = baseUrl;

        while (!string.IsNullOrEmpty(url))
        {
            string responseContent = await _httpService.GetAsync(url, parameters);
            var apiResponse = JsonSerializer.Deserialize<GraphApiResponse<T>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            allData.AddRange(apiResponse.Value);
            url = apiResponse.NextLink;
            parameters = null; // Clear parameters after the first request to avoid appending them again
        }

        return allData;
    }
}