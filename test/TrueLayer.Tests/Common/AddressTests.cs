using System;
using TrueLayer.Common;
using Xunit;

namespace TrueLayer.Tests.Common;

public class AddressTests
{
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void New_address_throws_if_address_line_1_empty_or_whitespace(string addressLine1)
    {
        Assert.Throws<ArgumentException>("addressLine1",
            () => new Address("city", "state", "country Code", "zip", addressLine1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void New_address_throws_if_city_null_or_empty(string city)
    {
        Assert.Throws<ArgumentException>("city",
            () => new Address(city, "state", "zip", "country Code", "addressLine1"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void New_address_throws_if_state_null_or_empty(string state)
    {
        Assert.Throws<ArgumentException>("state",
            () => new Address("city", state, "zip", "country Code", "addressLine1"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void New_address_throws_if_zip_null_or_empty(string zip)
    {
        Assert.Throws<ArgumentException>("zip",
            () => new Address("city", "state", zip, "country Code", "addressLine1"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void New_address_throws_if_country_code_null_or_empty(string countryCode)
    {
        Assert.Throws<ArgumentException>("countryCode",
            () => new Address("city", "state", "zip", countryCode, "addressLine1"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void New_address_throws_if_address_line_2_null_or_empty(string addressLine2)
    {
        Assert.Throws<ArgumentException>("addressLine2",
            () => new Address("city", "state", "zip", "countryCode", "addressLine1", addressLine2));
    }
}
