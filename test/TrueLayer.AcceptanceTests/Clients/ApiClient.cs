using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace TrueLayer.AcceptanceTests.Clients;

public class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> SubmitPaymentsProviderReturnAsync(string query, string fragment)
    {
        var requestBody = new SubmitProviderReturnParametersRequest { Query = query, Fragment = fragment };

        var request = new HttpRequestMessage(HttpMethod.Post, "/spa/payments-provider-return")
        {
            Content = JsonContent.Create(requestBody)
        };
        var response = await _httpClient.SendAsync(request);
        return response;
    }
}

