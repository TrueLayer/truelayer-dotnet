using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using System;
using System.Net;
using Shouldly;
using RichardSzalay.MockHttp;
using System.Net.Mime;
using TrueLayer.Serialization;
using System.Text;
using System.Text.Json;

namespace TrueLayer.Sdk.Tests
{
    public class ApiClientTests : IDisposable
    {
        private readonly MockHttpMessageHandler _httpMessageHandler;
        private readonly ApiClient _apiClient;
        private readonly TestResponse _stub;

        public ApiClientTests()
        {
            _httpMessageHandler = new MockHttpMessageHandler();

            _apiClient = new ApiClient(
                _httpMessageHandler.ToHttpClient()
            );

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
            response.IsSuccessful.ShouldBeFalse();
            response.StatusCode.ShouldBe(statusCode);
            response.TraceId.ShouldBe("trace-id");
            response.Data.ShouldBeNull();
        }


        [Fact]
        public async Task Given_request_fails_returns_problem_details()
        {
            string json = @"
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
            response.IsSuccessful.ShouldBeFalse();
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
            response.TraceId.ShouldBe("trace-id");
            response.Data.ShouldBeNull();
            response.Problem.ShouldNotBeNull();
            response.Problem.Type.ShouldBe("https://docs.truelayer.com/errors#invalid_parameters");
            response.Problem.Title.ShouldBe("Validation Error");
            response.Problem.Detail.ShouldBe("Invalid Parameters");
            response.Problem.Errors.ShouldNotBeNull();
            response.Problem.Errors["summary"].ShouldContain("summary_required");
        }

        private static void AssertSame(TestResponse? response, TestResponse expected)
        {
            response.ShouldNotBeNull();
            response.FirstName.ShouldBe(expected.FirstName);
            response.LastName.ShouldBe(expected.LastName);
            response.Age.ShouldBe(expected.Age);
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
