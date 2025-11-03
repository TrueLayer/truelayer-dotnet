using System;
using Xunit;

namespace TrueLayer.Tests;

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
        string? result = default(string?).NotEmptyOrWhiteSpace("value");
        Assert.Null(result);
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
    {
        int result = 10.GreaterThan(5, "value");
        Assert.Equal(10, result);
    }

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
}