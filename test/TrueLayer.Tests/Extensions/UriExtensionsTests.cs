using System;
using System.Collections.Generic;
using Shouldly;
using TrueLayer.Extensions;
using Xunit;

namespace TrueLayer.Tests.Extensions
{
    public class UriExtensionsTests
    {
        [Theory]
        [MemberData(nameof(UriTestData))]
        public void Append_Returns_Expected_Uri(
            Uri baseUri,
            string?[] segments,
            Uri expectedUri)
        {
            // Act
            var actual = baseUri.Append(segments);

            // Assert
            actual.ShouldBe(expectedUri);
        }

        [Theory]
        [MemberData(nameof(QueryParametersTestData))]
        public void AppendQueryParameters_Returns_Expected_Uri(
            Uri baseUri,
            IDictionary<string, string?> queryParams,
            Uri expectedUri)
        {
            // Act
            var actual = baseUri.AppendQueryParameters(queryParams);

            // Assert
            actual.ShouldBe(expectedUri);
        }

        public static IEnumerable<object?[]> QueryParametersTestData()
        {
            string baseUrl = "http://test.foo.com/";
            Uri baseUri = new(baseUrl);

            yield return new object?[]
            {
                baseUri,
                new Dictionary<string, string?> { ["param1"] = "value1" },
                new Uri($"{baseUrl}?param1=value1")
            };
            yield return new object?[]
            {
                baseUri,
                new Dictionary<string, string?> { ["param1"] = "value1", ["param2"] = "value2" },
                new Uri($"{baseUrl}?param1=value1&param2=value2")
            };
            yield return new object?[]
            {
                baseUri,
                new Dictionary<string, string?> { ["param1"] = null, ["param2"] = "value2" },
                new Uri($"{baseUrl}?param2=value2")
            };
            yield return new object?[]
            {
                baseUri,
                new Dictionary<string, string?> { ["param1"] = null, ["param2"] = "" },
                new Uri($"{baseUrl}?param2=")
            };
            yield return new object?[]
            {
                baseUri,
                new Dictionary<string, string?> { ["param1"] = null, ["param2"] = "  " },
                new Uri($"{baseUrl}?param2=")
            };
            yield return new object?[]
            {
                baseUri,
                new Dictionary<string, string?> { ["param1"] = "  ", ["param2"] = "value2" },
                new Uri($"{baseUrl}?param1=&param2=value2")
            };
            yield return new object?[]
            {
                baseUri,
                new Dictionary<string, string?>(),
                new Uri(baseUrl)
            };
            yield return new object?[]
            {
                baseUri,
                null,
                new Uri(baseUrl)
            };
            yield return new object[]
            {
                new Uri($"{baseUrl}?existing=param"),
                new Dictionary<string, string?> { ["param1"] = "value1"  },
                new Uri($"{baseUrl}?existing=param&param1=value1")
            };
        }

        public static IEnumerable<object[]> UriTestData()
        {
            string baseUrl = "http://test.foo.com/";
            Uri baseUri = new(baseUrl);

            yield return new object[] { baseUri, new[] { "test" }, new Uri($"{baseUrl}test") };
            yield return new object[] { baseUri, new[] { "/test" }, new Uri($"{baseUrl}test") };
            yield return new object[] { baseUri, new[] { "test", "/test2/" }, new Uri($"{baseUrl}test/test2") };
            yield return new object[]
            {
                new Uri("http://test.foo.test/extra-path"),
                new[] { "test/" },
                new Uri("http://test.foo.test/extra-path/test"),
            };
            yield return new object[]
            {
                new Uri("http://test.foo.test"),
                new[] { "/test" },
                new Uri("http://test.foo.test/test"),
            };
            yield return new object[]
            {
                new Uri("http://test.foo.test"),
                new string?[] { null },
                new Uri("http://test.foo.test"),
            };
            yield return new object[]
            {
                new Uri("http://test.foo.test"),
                new[] { "", "foo", "  ", null, "test" },
                new Uri("http://test.foo.test/foo/test"),
            };
            yield return new object[]
            {
                new Uri("http://test.foo.test"),
                new[] { "", " " },
                new Uri("http://test.foo.test"),
            };
        }
    }
}
