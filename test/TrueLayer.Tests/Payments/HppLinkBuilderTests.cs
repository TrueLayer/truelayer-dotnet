using System;
using TrueLayer.Payments;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.Tests.Payments;

public class HppLinkBuilderTests
{
    [Fact]
    public void Build_Payment_Sandbox_ReturnsCorrectUri()
    {
        var paymentId = new Guid().ToString();
        const string token = "eyJhbGciOiJSUzUxMiIsImtpZCI6IlBDNHJFVHZycGZTV1dqYW91R2dIbmJHNTBBR184SFBHXzBuU0";
        var sut = new HppLinkBuilder(null, true);

        var result = sut.Build(paymentId, token, new Uri("https://return.client.com/"), ResourceType.Payment);

        Assert.Equal($"https://payment.truelayer-sandbox.com/payments#payment_id={paymentId}&resource_token={token}&return_uri=https://return.client.com/", result);
    }

    [Fact]
    public void Build_Payment_Production_ReturnsCorrectUri()
    {
        var paymentId = new Guid().ToString();
        const string token = "eyJhbGciOiJSUzUxMiIsImtpZCI6IlBDNHJFVHZycGZTV1dqYW91R2dIbmJHNTBBR184SFBHXzBuU0";
        var sut = new HppLinkBuilder(null, false);

        var result = sut.Build(paymentId, token, new Uri("https://return.client.com/"), ResourceType.Payment);

        Assert.Equal($"https://payment.truelayer.com/payments#payment_id={paymentId}&resource_token={token}&return_uri=https://return.client.com/", result);
    }

    [Fact]
    public void Build_CustomUri_ReturnsCorrectUri()
    {
        var uri = new Uri("https://api.custom.com/");
        var paymentId = new Guid().ToString();
        const string token = "eyJhbGciOiJSUzUxMiIsImtpZCI6IlBDNHJFVHZycGZTV1dqYW91R2dIbmJHNTBBR184SFBHXzBuU0";
        var sut = new HppLinkBuilder(uri, false);

        var result = sut.Build(paymentId, token, new Uri("https://return.client.com/"), ResourceType.Payment);

        Assert.Equal($"https://api.custom.com/payments#payment_id={paymentId}&resource_token={token}&return_uri=https://return.client.com/", result);
    }

    [Fact]
    public void Build_Payment_Sandbox_MaxWait_Signup_ReturnsCorrectUri()
    {
        var paymentId = new Guid().ToString();
        const string token = "eyJhbGciOiJSUzUxMiIsImtpZCI6IlBDNHJFVHZycGZTV1dqYW91R2dIbmJHNTBBR184SFBHXzBuU0";
        var sut = new HppLinkBuilder(null, true);

        var result = sut.Build(paymentId, token, new Uri("https://return.client.com/"), ResourceType.Payment, 60, true);

        Assert.Equal($"https://payment.truelayer-sandbox.com/payments#payment_id={paymentId}&resource_token={token}&return_uri=https://return.client.com/&max_wait_seconds=60&signup=true", result);
    }

    [Fact]
    public void Build_Mandate_Sandbox_ReturnsCorrectUri()
    {
        var mandateId = new Guid().ToString();
        const string token = "eyJhbGciOiJSUzUxMiIsImtpZCI6IlBDNHJFVHZycGZTV1dqYW91R2dIbmJHNTBBR184SFBHXzBuU0";
        var sut = new HppLinkBuilder(null, true);

        var result = sut.Build(mandateId, token, new Uri("https://return.client.com/"), ResourceType.Mandate);

        Assert.Equal($"https://payment.truelayer-sandbox.com/mandates#mandate_id={mandateId}&resource_token={token}&return_uri=https://return.client.com/", result);
    }

    [Fact]
    public void Build_Mandate_Production_ReturnsCorrectUri()
    {
        var mandateId = new Guid().ToString();
        const string token = "eyJhbGciOiJSUzUxMiIsImtpZCI6IlBDNHJFVHZycGZTV1dqYW91R2dIbmJHNTBBR184SFBHXzBuU0";
        var sut = new HppLinkBuilder(null, false);

        var result = sut.Build(mandateId, token, new Uri("https://return.client.com/"), ResourceType.Mandate);

        Assert.Equal($"https://payment.truelayer.com/mandates#mandate_id={mandateId}&resource_token={token}&return_uri=https://return.client.com/", result);
    }
}
