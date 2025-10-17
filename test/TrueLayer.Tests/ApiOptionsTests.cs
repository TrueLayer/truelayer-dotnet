using System;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace TrueLayer.Tests;

public class ApiOptionsTests
{
    [Theory]
    [InlineData("")]
    [InlineData("payments")]
    [InlineData("/payments")]
    public void Invalid_if_provided_url_is_not_absolute(string value)
    {
        Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri);
        Assert.Throws<ValidationException>(() => new ApiOptions { Uri = uri }.Validate());
    }

    [Fact]
    public void Valid_if_absolute_uri()
    {
        new ApiOptions { Uri = new Uri("http://api.truelayer.com/") }.Validate();
    }
}