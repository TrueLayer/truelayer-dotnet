using System;
using TrueLayer.Payments;
using Xunit;

namespace TrueLayer.Tests.Extensions;

public class TrueLayerOptionsExtensionsTests
{
    [Fact]
    public void GetApiBaseUri_UseCustomUri_ReturnsCustomUri()
    {
        var options = new TrueLayerOptions
        {
            Auth = new ApiOptions
            {
                Uri = new Uri("https://auth.custom.com")
            },
            Payments = new PaymentsOptions
            {
                Uri = new Uri("https://api.custom.com")
            },
            UseSandbox = true
        };

        var actual = options.GetApiBaseUri();

        // Assert
        Assert.Equal("https://api.custom.com/", actual.ToString());
    }

    [Fact]
    public void GetApiBaseUri_UseSandBox_ReturnsSbxUri()
    {
        var options = new TrueLayerOptions
        {
            UseSandbox = true,
            Auth = new ApiOptions
            {
            Uri = new Uri("https://auth.custom.com")
        },
        };

        var actual = options.GetApiBaseUri();

        // Assert
        Assert.Equal("https://api.truelayer-sandbox.com/", actual.ToString());
    }

    [Fact]
    public void GetApiBaseUri_ReturnsProdUri()
    {
        var options = new TrueLayerOptions
        {
            UseSandbox = false,
            Auth = new ApiOptions
            {
                Uri = new Uri("https://auth.custom.com")
            },
        };

        var actual = options.GetApiBaseUri();

        // Assert
        Assert.Equal("https://api.truelayer.com/", actual.ToString());
    }

    [Fact]
    public void GetAuthBaseUri_UseCustomUri_ReturnsCustomUri()
    {
        var options = new TrueLayerOptions
        {
            Auth = new ApiOptions
            {
                Uri = new Uri("https://auth.custom.com")
            },
            Payments = new PaymentsOptions
            {
                Uri = new Uri("https://api.custom.com")
            },
            UseSandbox = true
        };

        var actual = options.GetAuthBaseUri();

        // Assert
        Assert.Equal("https://auth.custom.com/", actual.ToString());
    }

    [Fact]
    public void GetAuthBaseUri_UseSandBox_ReturnsSbxUri()
    {
        var options = new TrueLayerOptions
        {
            UseSandbox = true,
            Payments = new PaymentsOptions
            {
                Uri = new Uri("https://api.custom.com")
            }
        };

        var actual = options.GetAuthBaseUri();

        // Assert
        Assert.Equal("https://auth.truelayer-sandbox.com/", actual.ToString());
    }

    [Fact]
    public void GetAuthBaseUri_ReturnsProdUri()
    {
        var options = new TrueLayerOptions
        {
            UseSandbox = false,
            Payments = new PaymentsOptions
            {
                Uri = new Uri("https://api.custom.com")
            },
        };

        var actual = options.GetAuthBaseUri();

        // Assert
        Assert.Equal("https://auth.truelayer.com/", actual.ToString());
    }
}
