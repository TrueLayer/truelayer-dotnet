using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TrueLayer.Sdk.Acceptance.Tests
{
    public class MockBankClient : IDisposable
    {
        private readonly HttpClient _httpClient = new();

        public async Task Authorize(string redirectUrl)
        {
            var bankSession = ParseMockBankUrl(redirectUrl);

            string body = "{ \"action\": \"Execute\" }";

            var request = new HttpRequestMessage(HttpMethod.Post, bankSession.AuthUrl)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bankSession.AuthToken);

            using HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        private static (string AuthUrl, string AuthToken) ParseMockBankUrl(string redirectUrl)
        {
            string[] parts = redirectUrl.Split('/', '=', '#');
            string authToken = parts[^1];
            string authId = parts[^3];

            return ($"https://pay-mock-connect.truelayer-sandbox.com/api/single-immediate-payments/{authId}/action", authToken);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
