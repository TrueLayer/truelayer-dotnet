using System;
using AwesomeAssertions;
using TrueLayer.Mandates;
using Xunit;

namespace TrueLayer.Tests.Mandates;

public class HppLinkBuilderTests
{
    [Fact]
    public void Build_Mandate_Sandbox_ReturnsCorrectUri()
    {
        var mandateId = new Guid().ToString();
        const string token = "eyJhbGciOiJSUzUxMiIsImtpZCI6IlBDNHJFVHZycGZTV1dqYW91R2dIbmJHNTBBR184SFBHXzBuU0";
        var sut = new HppLinkBuilder(null, true);

        var result = sut.Build(mandateId, token, new Uri("https://return.client.com/"));

        result.Should().Be($"https://payment.truelayer-sandbox.com/mandates#mandate_id={mandateId}&resource_token={token}&return_uri=https://return.client.com/");
    }

    [Fact]
    public void Build_Mandate_Production_ReturnsCorrectUri()
    {
        var mandateId = new Guid().ToString();
        const string token = "eyJhbGciOiJSUzUxMiIsImtpZCI6IlBDNHJFVHZycGZTV1dqYW91R2dIbmJHNTBBR184SFBHXzBuU0";
        var sut = new HppLinkBuilder(null, false);

        var result = sut.Build(mandateId, token, new Uri("https://return.client.com/"));

        result.Should().Be($"https://payment.truelayer.com/mandates#mandate_id={mandateId}&resource_token={token}&return_uri=https://return.client.com/");
    }

    [Fact]
    public void Build_CustomUri_ReturnsCorrectUri()
    {
        var uri = new Uri("https://api.custom.com/");
        var mandateId = new Guid().ToString();
        const string token = "eyJhbGciOiJSUzUxMiIsImtpZCI6IlBDNHJFVHZycGZTV1dqYW91R2dIbmJHNTBBR184SFBHXzBuU0";
        var sut = new HppLinkBuilder(uri, false);

        var result = sut.Build(mandateId, token, new Uri("https://return.client.com/"));

        result.Should().Be($"https://api.custom.com/mandates#mandate_id={mandateId}&resource_token={token}&return_uri=https://return.client.com/");
    }
}
