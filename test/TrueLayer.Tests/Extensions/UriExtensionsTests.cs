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
            string[] segments,
            Uri expectedUri)
        {
            // Act
            var actual = baseUri.Append(segments);

            // Assert
            actual.ShouldBe(expectedUri);
        }

        public static IEnumerable<object[]> UriTestData()
        {
            string baseUrl = "http://test.foo.com/";
            Uri baseUri = new(baseUrl);

            yield return new object[] { baseUri, new[] {"test"}, new Uri($"{baseUrl}test") };
            yield return new object[] { baseUri, new[] {"test", "/test2/"}, new Uri($"{baseUrl}test/test2") };
            yield return new object[]
            {
                new Uri("http://test.foo.test/extra-path"),
                new[] { "test/" },
                new Uri("http://test.foo.test/extra-path/test"),
            };
        }
    }
}
