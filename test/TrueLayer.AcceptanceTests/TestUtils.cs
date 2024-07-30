using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;

namespace TrueLayer.AcceptanceTests;

public class HeadlessResourceAuthorization
{
    public static HeadlessResourceAuthorization New(HeadlessResource resource, HeadlessResourceAction action) =>
        new()
        {
            Resource = resource,
            Action = action,
            Payload = $@"{{""action"":""{action}"", ""redirect"": false}}",
            Path = resource switch {
                HeadlessResource.Payments => "single-immediate-payments",
                HeadlessResource.Mandates => "vrp-consents",
                _ => throw new ArgumentOutOfRangeException(nameof(resource), resource, null)
            }
        };

    public HeadlessResource Resource { get; set; }
    public HeadlessResourceAction Action { get; set; }
    public string Path { get; set; } = null!;
    public string Payload { get; set; } = null!;
}

public enum HeadlessResourceAction
{
    Invalid,
    Execute,
    Authorize,
    RejectAuthorization
}

public enum HeadlessResource {
    Invalid,
    Payments,
    Mandates
}

public static class TestUtils
{
    public async static Task RunAndAssertHeadlessResourceAuthorisation(
        TrueLayerOptions configuration,
        Uri redirectUri,
        HeadlessResourceAuthorization authorization)
    {
        var resourceId = redirectUri.PathAndQuery.Split("/").Last();
        var token = redirectUri.Fragment.Split("=").Last();
        resourceId.ShouldNotBeEmpty();
        token.ShouldNotBeEmpty();

        var url = $"{redirectUri.Scheme}://{redirectUri.Host}/api/{authorization.Path}/{resourceId}/action";
        var testClient = new HttpClient();
        testClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var authResponse = await testClient.PostAsync(url,
            new StringContent(authorization.Payload, System.Text.Encoding.UTF8, "application/json"));
        authResponse.IsSuccessStatusCode.ShouldBeTrue();

        var authResponseString = await authResponse.Content.ReadAsStringAsync();
        var authResponseUri = new Uri(authResponseString);

        String query = authorization.Resource == HeadlessResource.Mandates
            ? authResponseUri.Query.Replace("mandate-", "")
            : authResponseUri.Query;

        var jsonPayload = $@"{{""query"":""{query.Split("?").Last()}"", ""fragment"": ""{authResponseUri.Fragment.Split("?").Last()}""}}";
        var submitParamsUri =  new Uri($"{configuration.Payments?.Uri}payments-provider-return");
        testClient.DefaultRequestHeaders.Clear();
        var submitProviderParamsResponse =
            await testClient.PostAsync(
                submitParamsUri,
                new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json"));

        submitProviderParamsResponse.IsSuccessStatusCode.ShouldBeTrue();
    }
}
