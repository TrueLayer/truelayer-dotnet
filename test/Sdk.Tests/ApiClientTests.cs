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

namespace TrueLayer.Sdk.Tests
{
    public class ApiClientTests : IDisposable
    {
        private static System.Text.Json.JsonSerializerOptions SerializerOptions = new()
        {
            IgnoreNullValues = true,
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
        };

        private readonly MockHttpMessageHandler _httpMessageHandler;
        private readonly ApiClient _apiClient;
        private readonly ISerializer _jsonSerializer;
        private readonly TestResponse _stub;

        public ApiClientTests()
        {
            _httpMessageHandler = new MockHttpMessageHandler();
            _jsonSerializer = new JsonSerializer();

            _apiClient = new ApiClient(
                _httpMessageHandler.ToHttpClient(),
                _jsonSerializer
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
                .Respond(HttpStatusCode.OK, MediaTypeNames.Application.Json, _jsonSerializer.Serialize(_stub));

            TestResponse response = await _apiClient.GetAsync<TestResponse>(
                new Uri("http://localhost/get-json"),
                "access-token"
            );

            AssertSame(response, _stub);
        }

        [Fact]
        public async Task Posts_http_content_and_returns_deserialized_json()
        {
            string requestJson = _jsonSerializer.Serialize(new
            {
                data = "http-content"
            });

            _httpMessageHandler
                .Expect(HttpMethod.Post, "http://localhost/post-http-content")
                .WithHeaders("Authorization", "Bearer access-token")
                .WithContent(requestJson)
                .Respond(HttpStatusCode.OK, MediaTypeNames.Application.Json, _jsonSerializer.Serialize(_stub));

            TestResponse response = await _apiClient.PostAsync<TestResponse>(
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

            var json = _jsonSerializer.Serialize(obj);

            _httpMessageHandler
                .Expect(HttpMethod.Post, "http://localhost/post-object")
                .WithHeaders("Authorization", "Bearer access-token")
                .WithContent(json)
                .Respond(HttpStatusCode.OK, MediaTypeNames.Application.Json, _jsonSerializer.Serialize(_stub));

            TestResponse response = await _apiClient.PostAsync<TestResponse>(
                new Uri("http://localhost/post-object"),
                obj,
                "access-token"
            );

            AssertSame(response, _stub);
        }

        [Fact]
        public async Task Given_resource_not_found_throws_resource_not_found_exception()
        {
            _httpMessageHandler
                .Expect(HttpMethod.Get, "http://localhost/not-found")
                .Respond(() =>
                {
                    var response = new HttpResponseMessage(HttpStatusCode.NotFound);
                    response.Headers.TryAddWithoutValidation(CustomHeaders.RequestId, "request-id");
                    return Task.FromResult(response);
                });

            var exception = await Assert.ThrowsAsync<TrueLayerResourceNotFoundException>(
                () => _apiClient.GetAsync<TestResponse>(new Uri("http://localhost/not-found"))
            );

            exception.RequestId.ShouldBe("request-id");
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.UnprocessableEntity)]
        public async Task Given_bad_request_throws_validation_exception(HttpStatusCode httpStatusCode)
        {
            _httpMessageHandler
                .Expect(HttpMethod.Get, "http://localhost/bad-request")
                .Respond(() =>
                {
                    var response = new HttpResponseMessage(httpStatusCode);
                    response.Headers.TryAddWithoutValidation(CustomHeaders.RequestId, "request-id");
                    return Task.FromResult(response);
                });

            var exception = await Assert.ThrowsAsync<TrueLayerValidationException>(
                () => _apiClient.GetAsync<TestResponse>(new Uri("http://localhost/bad-request"))
            );

            exception.RequestId.ShouldBe("request-id");
            exception.Error.ShouldBeNull();
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.UnprocessableEntity)]
        public async Task Given_bad_request_with_body_throws_validation_exception_with_error_details(HttpStatusCode httpStatusCode)
        {
            var error = new ErrorResponse
            {
                Error = "error",
                ErrorDescription = "An error occurred",
                ErrorDetails = new ErrorDetails
                {
                    Parameters = new()
                    {
                        { "param", new[] { "invalid_param" } }
                    }
                }
            };

            _httpMessageHandler
                .Expect(HttpMethod.Get, "http://localhost/bad-request-error-details")
                .Respond(() =>
                {
                    var response = new HttpResponseMessage(httpStatusCode);
                    response.Content = new StringContent(_jsonSerializer.Serialize(error));
                    response.Headers.TryAddWithoutValidation(CustomHeaders.RequestId, "request-id");
                    return Task.FromResult(response);
                });

            var exception = await Assert.ThrowsAsync<TrueLayerValidationException>(
                () => _apiClient.GetAsync<TestResponse>(new Uri("http://localhost/bad-request-error-details"))
            );

            exception.RequestId.ShouldBe("request-id");
            AssertSame(exception.Error, error);
        }

        [Theory]
        [InlineData(HttpStatusCode.PreconditionFailed)]
        [InlineData(HttpStatusCode.UnsupportedMediaType)]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.BadGateway)]
        public async Task Given_other_non_successful_status_throws_api_exception(HttpStatusCode httpStatusCode)
        {
            _httpMessageHandler
                .Expect(HttpMethod.Get, "http://localhost/non-successful")
                .Respond(() =>
                {
                    var response = new HttpResponseMessage(httpStatusCode);
                    response.Headers.TryAddWithoutValidation(CustomHeaders.RequestId, "request-id");
                    return Task.FromResult(response);
                });

            var exception = await Assert.ThrowsAsync<TrueLayerApiException>(
                () => _apiClient.GetAsync<TestResponse>(new Uri("http://localhost/non-successful"))
            );

            exception.RequestId.ShouldBe("request-id");

        }

        private static void AssertSame(TestResponse response, TestResponse expected)
        {
            response.FirstName.ShouldBe(expected.FirstName);
            response.LastName.ShouldBe(expected.LastName);
            response.Age.ShouldBe(expected.Age);
        }

        private static void AssertSame(ErrorResponse error, ErrorResponse expected)
        {
            error.Error.ShouldBe(expected.Error);
            error.ErrorDescription.ShouldBe(expected.ErrorDescription);

            if (expected.ErrorDetails is not null)
            {
                error.ErrorDetails.ShouldNotBeNull();

                if (expected.ErrorDetails.Parameters is not null)
                {
                    error.ErrorDetails.ShouldNotBeNull();

                    foreach (var param in expected.ErrorDetails.Parameters)
                    {
                        error.ErrorDetails.Parameters[param.Key].ShouldBe(param.Value);
                    }
                }
            }

        }

        public void Dispose()
        {
            _httpMessageHandler.VerifyNoOutstandingExpectation();
        }

        class TestResponse
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }

            public Dictionary<string, string> Headers { get; } = new();
        }
    }
}
