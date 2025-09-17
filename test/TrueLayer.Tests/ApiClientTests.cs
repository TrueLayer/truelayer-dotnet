using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using TrueLayer.Auth;
using TrueLayer.Serialization;
using TrueLayer.Tests.Mocks;
using Xunit;

namespace TrueLayer.Tests
{
    public class ApiClientTests : IDisposable
    {
        private readonly MockHttpMessageHandler _httpMessageHandler;
        private readonly ApiClient _apiClient;
        private readonly TestResponse _stub;
        private readonly InMemoryAuthTokenCacheMock _authTokenCache;
        private const string PrivateKey = @"-----BEGIN EC PRIVATE KEY-----
MIHcAgEBBEIALJ2sKM+8mVDfTIlk50rqB5lkxaLBt+OECvhXq3nEaB+V0nqljZ9c
5aHRN3qqxMzNLvxFQ+4twifa4ezkMK2/j5WgBwYFK4EEACOhgYkDgYYABADmhZbj
i8bgJRfMTdtzy+5VbS5ScMaKC1LQfhII+PTzGzOr+Ts7Qv8My5cmYU5qarGK3tWF
c3VMlcFZw7Y0iLjxAQFPvHqJ9vn3xWp+d3JREU1vQJ9daXswwbcoer88o1oVFmFf
WS1/11+TH1x/lgKckAws6sAzJLPtCUZLV4IZTb6ENg==
-----END EC PRIVATE KEY-----";

        public ApiClientTests()
        {
            _httpMessageHandler = new MockHttpMessageHandler();
            _authTokenCache = new InMemoryAuthTokenCacheMock();
;            _apiClient = new ApiClient(
                _httpMessageHandler.ToHttpClient(),
                _authTokenCache);
            _stub = new TestResponse
            {
                FirstName = "Jane",
                LastName = "Doe",
                Age = 35
            };
        }

        [Fact]
        public async Task Get_deserialized_json()
        {
            _httpMessageHandler
                .Expect(HttpMethod.Get, "http://localhost/get-json")
                .WithHeaders("Authorization", "Bearer access-token")
                .Respond(HttpStatusCode.OK, MediaTypeNames.Application.Json, JsonSerializer.Serialize(_stub, SerializerOptions.Default));

            TestResponse? response = await _apiClient.GetAsync<TestResponse>(
                new Uri("http://localhost/get-json"),
                "access-token"
            );

            AssertSame(response, _stub);
        }

        [Fact]
        public async Task Posts_http_content_and_returns_deserialized_json()
        {
            string requestJson = JsonSerializer.Serialize(new
            {
                data = "http-content"
            }, SerializerOptions.Default);

            _httpMessageHandler
                .Expect(HttpMethod.Post, "http://localhost/post-http-content")
                .WithHeaders("Authorization", "Bearer access-token")
                .WithContent(requestJson)
                .Respond(HttpStatusCode.OK, MediaTypeNames.Application.Json, JsonSerializer.Serialize(_stub, SerializerOptions.Default));

            TestResponse? response = await _apiClient.PostAsync<TestResponse>(
                new Uri("http://localhost/post-http-content"),
                new StringContent(requestJson),
                "access-token"
            );

            AssertSame(response, _stub);
        }

        [Fact]
        public async Task Posts_http_content_and_expects_no_response_content()
        {
            string requestJson = JsonSerializer.Serialize(new
            {
                data = "http-content"
            }, SerializerOptions.Default);

            _httpMessageHandler
                .Expect(HttpMethod.Post, "http://localhost/post-http-content")
                .WithHeaders("Authorization", "Bearer access-token")
                .WithContent(requestJson)
                .Respond(HttpStatusCode.NoContent);

            ApiResponse response = await _apiClient.PostAsync(
                new Uri("http://localhost/post-http-content"),
                new StringContent(requestJson),
                "access-token"
            );

            Assert.NotNull(response);
            Assert.True(response.IsSuccessful);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Posts_serialized_object_and_returns_deserialized_json()
        {
            var obj = new
            {
                data = "object"
            };

            var json = JsonSerializer.Serialize(obj, SerializerOptions.Default);

            _httpMessageHandler
                .Expect(HttpMethod.Post, "http://localhost/post-object")
                .WithHeaders("Authorization", "Bearer access-token")
                .WithContent(json)
                .Respond(HttpStatusCode.OK, MediaTypeNames.Application.Json, JsonSerializer.Serialize(_stub, SerializerOptions.Default));

            TestResponse? response = await _apiClient.PostAsync<TestResponse>(
                new Uri("http://localhost/post-object"),
                obj,
                accessToken: "access-token"
            );

            AssertSame(response, _stub);
        }

        [Fact]
        public async Task Posts_serialized_object_and_expects_no_response_content()
        {
            var obj = new
            {
                data = "object"
            };

            var json = JsonSerializer.Serialize(obj, SerializerOptions.Default);

            _httpMessageHandler
                .Expect(HttpMethod.Post, "http://localhost/post-object")
                .WithHeaders("Authorization", "Bearer access-token")
                .WithContent(json)
                .Respond(HttpStatusCode.NoContent);

            ApiResponse response = await _apiClient.PostAsync(
                new Uri("http://localhost/post-object"),
                obj,
                accessToken: "access-token"
            );

            Assert.NotNull(response);
            Assert.True(response.IsSuccessful);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.BadGateway)]
        public async Task Given_request_fails_returns_unsuccessful_response(HttpStatusCode statusCode)
        {
            _httpMessageHandler
                .Expect(HttpMethod.Get, "http://localhost/error")
                .Respond(() =>
                {
                    var response = new HttpResponseMessage(statusCode);
                    response.Headers.TryAddWithoutValidation(CustomHeaders.TraceId, "trace-id");
                    return Task.FromResult(response);
                });

            ApiResponse<TestResponse> response = await _apiClient.GetAsync<TestResponse>(new Uri("http://localhost/error"));

            Assert.False(response.IsSuccessful);
            Assert.Equal(statusCode, response.StatusCode);
            Assert.Equal("trace-id", response.TraceId);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task Given_request_unauthorized_clear_auth_token_cache()
        {
            var obj = new
            {
                data = "object"
            };

            _httpMessageHandler
                .Expect(HttpMethod.Post, "http://localhost/post-object")
                .Respond(() =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    response.Headers.TryAddWithoutValidation(CustomHeaders.TraceId, "trace-id");
                    return Task.FromResult(response);
                });

            var authData = new GetAuthTokenResponse("access-token", 3600, "Bearer", "scope");
            _authTokenCache.Set("test", new ApiResponse<GetAuthTokenResponse>(authData, HttpStatusCode.OK, "trace-id"), TimeSpan.FromMinutes(5));

            var response = await _apiClient.PostAsync(
                new Uri("http://localhost/post-object"),
                obj,
                accessToken: authData.AccessToken
            );

            Assert.True(_authTokenCache.IsEmpty);
            Assert.False(response.IsSuccessful);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("trace-id", response.TraceId);
        }

        [Fact]
        public async Task Given_request_fails_returns_problem_details()
        {
            const string json = @"
                {
                    ""type"": ""https://docs.truelayer.com/errors#invalid_parameters"",
                    ""title"": ""Validation Error"",
                    ""detail"": ""Invalid Parameters"",
                    ""status"": 400,
                    ""trace_id"": ""trace-id"",
                    ""errors"": {
                        ""summary"": [
                            ""summary_required""
                        ]
                    }
                }
            ";

            _httpMessageHandler
                .Expect(HttpMethod.Get, "http://localhost/bad-request-error-details")
                .Respond(() =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                    response.Content = new StringContent(json, Encoding.UTF8, "application/problem+json");
                    response.Headers.TryAddWithoutValidation(CustomHeaders.TraceId, "trace-id");
                    return Task.FromResult(response);
                });

            ApiResponse<TestResponse> response = await _apiClient.GetAsync<TestResponse>(new Uri("http://localhost/bad-request-error-details"));

            Assert.False(response.IsSuccessful);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("trace-id", response.TraceId);
            Assert.Null(response.Data);
            Assert.NotNull(response.Problem);
            Assert.Equal("https://docs.truelayer.com/errors#invalid_parameters", response.Problem!.Type);
            Assert.Equal("Validation Error", response.Problem.Title);
            Assert.Equal("Invalid Parameters", response.Problem.Detail);
            Assert.NotNull(response.Problem.Errors);
            Assert.Contains("summary_required", response.Problem.Errors!["summary"]);
        }

        [Fact]
        public async Task Sets_TL_agent_header()
        {
            _httpMessageHandler
                .Expect(HttpMethod.Get, "http://localhost/user-agent")
                .WithHeaders(CustomHeaders.Agent, $"truelayer-dotnet/{ReflectionUtils.GetAssemblyVersion<ApiClient>()} ({System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription})")
                .Respond(HttpStatusCode.OK, MediaTypeNames.Application.Json, "{}");

            var response = await _apiClient.GetAsync<TestResponse>(new Uri("http://localhost/user-agent"));
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Generates_request_signature_when_signing_key_and_body_provided()
        {
            var obj = new
            {
                key = "value"
            };

            var signingKey = new SigningKey { KeyId = Guid.NewGuid().ToString(), PrivateKey = PrivateKey };

            var requestUri = new Uri("http://localhost/signing");
            var idempotencyKey = Guid.NewGuid().ToString();

            _httpMessageHandler
                .Expect(HttpMethod.Post, "http://localhost/signing")
                .With(r => r.Headers.Contains(CustomHeaders.Signature))
                .WithHeaders(CustomHeaders.IdempotencyKey, idempotencyKey)
                .Respond(HttpStatusCode.OK, MediaTypeNames.Application.Json, "{}");

            var response = await _apiClient.PostAsync<TestResponse>(
                requestUri,
                obj,
                idempotencyKey: idempotencyKey,
                signingKey: signingKey);
        }

        [Fact]
        public async Task Generates_request_signature_when_signing_key_and_no_content_provided()
        {
            var signingKey = new SigningKey { KeyId = Guid.NewGuid().ToString(), PrivateKey = PrivateKey };

            var requestUri = new Uri("http://localhost/signing");
            var idempotencyKey = Guid.NewGuid().ToString();

            _httpMessageHandler
                .Expect(HttpMethod.Post, "http://localhost/signing")
                .With(r => r.Headers.Contains(CustomHeaders.Signature))
                .WithHeaders(CustomHeaders.IdempotencyKey, idempotencyKey)
                .Respond(HttpStatusCode.OK, MediaTypeNames.Application.Json, "{}");

            await _apiClient.PostAsync<TestResponse>(
                requestUri,
                null,
                idempotencyKey: idempotencyKey,
                signingKey: signingKey);
        }

        [Fact]
        public async Task Omits_signature_when_no_signing_key_provided()
        {
            var obj = new
            {
                key = "value"
            };

            var requestUri = new Uri("http://localhost/no-signing");
            var idempotencyKey = Guid.NewGuid().ToString();

            _httpMessageHandler
                .Expect(HttpMethod.Post, "http://localhost/no-signing")
                .With(r => !r.Headers.Contains(CustomHeaders.Signature))
                .WithHeaders(CustomHeaders.IdempotencyKey, idempotencyKey)
                .Respond(HttpStatusCode.OK, MediaTypeNames.Application.Json, "{}");

            var response = await _apiClient.PostAsync<TestResponse>(
                requestUri,
                obj,
                idempotencyKey: idempotencyKey);
        }

        private static void AssertSame(TestResponse? response, TestResponse expected)
        {
            Assert.NotNull(response);
            Assert.Equal(expected.FirstName, response!.FirstName);
            Assert.Equal(expected.LastName, response.LastName);
            Assert.Equal(expected.Age, response.Age);
        }

        public void Dispose()
        {
            _httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        class TestResponse
        {
            public string FirstName { get; set; } = null!;
            public string LastName { get; set; } = null!;
            public int Age { get; set; }

            public Dictionary<string, string> Headers { get; } = new();
        }
    }
}
