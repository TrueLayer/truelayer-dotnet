using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TrueLayer.AcceptanceTests;

public class PayApiClient
{
    private readonly HttpClient _httpClient;

    public PayApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetJwksAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/.well-known/jwks.json");
        var response = await _httpClient.SendAsync(request);
        return response;
    }

    public async Task<HttpResponseMessage> SubmitProviderReturnParametersAsync(string query, string fragment)
    {
        var requestBody = new SubmitProviderReturnParametersRequest { Query = query, Fragment = fragment };

        var request = new HttpRequestMessage(HttpMethod.Post, "/spa/submit-provider-return-parameters")
        {
            Content = JsonContent.Create(requestBody)
        };
        var response = await _httpClient.SendAsync(request);
        return response;
    }
}

public class SubmitProviderReturnParametersRequest
{
    [JsonPropertyName("query")] public string? Query { get; set; }
    [JsonPropertyName("fragment")] public string? Fragment { get; set; }
}

