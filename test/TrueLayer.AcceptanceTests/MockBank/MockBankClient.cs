using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace TrueLayer.AcceptanceTests.MockBank;

public class MockBankClient
{
    private readonly HttpClient _httpClient;

    public MockBankClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Uri> AuthorisePaymentAsync(
        Uri authUri,
        MockBankAction action,
        int settlementDelayInSeconds = 0)
    {
        var mockPaymentId = authUri.Segments.Last();
        var token = authUri.Fragment[7..];

        var requestBody = $@"{{ ""action"": ""{action}"", ""settlement_delay_in_seconds"": {settlementDelayInSeconds} }}";

        var request = new HttpRequestMessage(HttpMethod.Post, $"api/single-immediate-payments/{mockPaymentId}/action")
        {
            Headers = { { "Authorization", $"Bearer {token}" } },
            Content = new StringContent(requestBody, Encoding.UTF8, MediaTypeNames.Application.Json),
        };
        var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.Accepted, "submit mock payment response should be 202");
        return new Uri(responseBody);
    }
}
