using System;
using Xunit;

namespace TrueLayer.Tests
{
    public class GuardTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Not_empty_or_whitespace_throws(string? value)
        {
            Assert.Throws<ArgumentException>(() => value.NotEmptyOrWhiteSpace(nameof(value)));
        }

        [Fact]
        public void Not_empty_or_whitespace_allows_null()
        {
            _ = default(string?).NotEmptyOrWhiteSpace("value");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(default(string?))]
        public void Not_null_or_whitespace_throws(string? value)
            => Assert.Throws<ArgumentException>(() => value.NotNullOrWhiteSpace(nameof(value)));

        [Fact]
        public void Not_null_throws_if_null()
            => Assert.Throws<ArgumentNullException>(() => default(string?).NotNull("value"));

        [Theory]
        [InlineData(10)]
        [InlineData(5)]
        public void Greater_than_throws_if_less_or_equal_to_value(int value)
            => Assert.Throws<ArgumentOutOfRangeException>(() => value.GreaterThan(10, nameof(value)));

        [Fact]
        public void Greater_than_does_not_throw_if_greater_than_value()
            => _ = 10.GreaterThan(5, "value");

        [Theory]
        [InlineData(null)]
        [InlineData("not_a_url")]
        [InlineData("anotherNonUrl")]
        [InlineData("7effef4a-17f2-4139-aee2-fae13544530a")]
        [InlineData("85BF9448-A93F-4F5F-A325-8B5BA7845F83")]
        [InlineData("{C5A41B28-109A-41C5-8CFD-695CC52A7539}")]
        [InlineData("12345")]
        public void NotAUrl_WithNullOrNonUrlValue_ReturnsSameValue(string? value)
        {
            Assert.Equal(value, value.NotAUrl("value"));
        }

        [Theory]
        [InlineData("http://example.com")]
        [InlineData("https://example.com")]
        [InlineData("/relative/url")]
        [InlineData("http://example.com?query=string")]
        [InlineData("http://example.com?query=string&otherquery=foo")]
        [InlineData("http://example.com/path%20with%20spaces")]
        [InlineData("string with spaces")]
        [InlineData("A7+uG3zwvUiKtrwb/ZtQow==")]
        [InlineData("\\/g8ph66mx5ltptbsdfwmr6kut2k8bw8kx.oastify.com")]
        [InlineData("fake.test.com")]
        [InlineData("fake.com")]
        [InlineData("fake.com/")]
        public void NotAUrl_WithUrlValue_ThrowsArgumentException(string value)
            => Assert.Throws<ArgumentException>(() => value.NotAUrl("value"));

        [Theory]
        // null URI
        [InlineData(null, null, null, null, true, typeof(ArgumentNullException))]
        [InlineData(null, null, null, null, false, typeof(ArgumentNullException))]
        [InlineData(null, "http://foo.com", "http://foo.com", "http://foo.com", true, typeof(ArgumentNullException))]
        [InlineData(null, "http://foo.com", "http://foo.com", "http://foo.com", false, typeof(ArgumentNullException))]
        // URI not based on TL URIs
        [InlineData("http://www.foo.com", null, null, null, true, typeof(ArgumentException))]
        [InlineData("http://www.foo.com", null, null, null, false, typeof(ArgumentException))]
        [InlineData("https://www.foo.com/path/", null, null, null, true, typeof(ArgumentException))]
        [InlineData("https://www.foo.com/path", null, null, null, false, typeof(ArgumentException))]
        // URI not based on custom configured Payments URIs
        [InlineData("http://www.foo.com", "http://payments.sandbox-foo.com", null, null, false, typeof(ArgumentException))]
        [InlineData("https://payments.foo.com", "http://payments.sandbox-foo.com", null, null, false, typeof(ArgumentException))]
        [InlineData("http://payments.foo.com", "https://payments.sandbox-foo.com", null, null, false, typeof(ArgumentException))]
        [InlineData("http://payments.foo.com/path", "https://payments.sandbox-foo.com", null, null, false, typeof(ArgumentException))]
        [InlineData("http://www.foo.com", "http://payments.foo.com", null, null, true, typeof(ArgumentException))]
        // URI not based on custom configured Hpp URIs
        [InlineData("http://www.foo.com", null, "http://hpp.sandbox-foo.com", null, false, typeof(ArgumentException))]
        [InlineData("http://www.foo.com", null, "http://hpp.foo.com", null, true, typeof(ArgumentException))]
        // URI not based on custom configured Auth URIs
        [InlineData("http://www.foo.com", null, null, "http://auth.sandbox-foo.com", false, typeof(ArgumentException))]
        [InlineData("http://www.foo.com", null, null, "http://auth.foo.com", true, typeof(ArgumentException))]
        public void HasValidBaseUri_WithNullOrNotValidValue_ThrowsExpectedException(
            string? url,
            string? configuredPaymentApiUrl,
            string? configuredPaymentHppUrl,
            string? configuredAuthApiUrl,
            bool useSandbox,
            Type exceptionType)
        {
            var value = url != null ?  new Uri(url) : null;
            var options = new TrueLayerOptions()
            {
                UseSandbox = useSandbox,
                Payments = new()
                {
                    Uri = configuredPaymentApiUrl is not null ? new Uri(configuredPaymentApiUrl) : null,
                    HppUri =
                        configuredPaymentHppUrl is not null ? new Uri(configuredPaymentHppUrl) : null,
                },
                Auth = new()
                {
                    Uri = configuredAuthApiUrl is not null ? new Uri(configuredAuthApiUrl) : null,
                }
            };

            Assert.Throws(exceptionType, () => value.HasValidBaseUri(nameof(value), options));
        }

        [Theory]
        // URI based on custom configured Payments URIs
        [InlineData("http://payments.foo.com", "http://payments.foo.com", null, null, false)]
        [InlineData("http://payments.foo.com/path/", "http://payments.foo.com", null, null, false)]
        [InlineData("http://payments.foo.com/path", "http://payments.foo.com", null, null, false)]
        [InlineData("https://payments.foo.com/path", "https://payments.foo.com", null, null, false)]
        [InlineData("http://payments.sandbox-foo.com", "http://payments.sandbox-foo.com", null, null, true)]
        // URI based on custom configured Hpp URIs
        [InlineData("http://hpp.sandbox-foo.com", null, "http://hpp.sandbox-foo.com", null, true)]
        [InlineData("http://hpp.foo.com", null, "http://hpp.foo.com", null, false)]
        // URI based on custom configured Auth URIs
        [InlineData("http://auth.sandbox-foo.com", null, null, "http://auth.sandbox-foo.com", true)]
        [InlineData("http://auth.foo.com", null, null, "http://auth.foo.com", false)]
        // URI based on Tl URIs
        [InlineData("https://auth.truelayer-sandbox.com/v3/foo", null, null, null, true)]
        [InlineData("https://auth.truelayer.com/v3/foo/", null, null, null, false)]
        [InlineData("https://api.truelayer-sandbox.com/v3/foo", null, null, null, true)]
        [InlineData("https://api.truelayer.com/v3/foo/", null, null, null, false)]
        [InlineData("https://payment.truelayer-sandbox.com/v3/foo", null, null, null, true)]
        [InlineData("https://payment.truelayer.com/v3/foo/", null, null, null, false)]
        // URI is localhost
        [InlineData("https://localhost/v3/foo/", null, null, null, false)]
        [InlineData("http://localhost/v3/foo/", null, null, null, false)]
        [InlineData("http://localhost/v3/foo/", null, null, null, true)]
        public void HasValidBaseUri_WithValidInput_ReturnsSameValue(
            string url,
            string? configuredPaymentApiUrl,
            string? configuredPaymentHppUrl,
            string? configuredAuthApiUrl,
            bool useSandbox)
        {
            var value = new Uri(url);
            var options = new TrueLayerOptions()
            {
                UseSandbox = useSandbox,
                Payments = new()
                {
                    Uri = configuredPaymentApiUrl is not null ? new Uri(configuredPaymentApiUrl) : null,
                    HppUri =
                        configuredPaymentHppUrl is not null ? new Uri(configuredPaymentHppUrl) : null,
                },
                Auth = new()
                {
                    Uri = configuredAuthApiUrl is not null ? new Uri(configuredAuthApiUrl) : null,
                }
            };

            var actual = value.HasValidBaseUri(nameof(value), options);
            Assert.Equal(value, actual);

        }
    }
}
