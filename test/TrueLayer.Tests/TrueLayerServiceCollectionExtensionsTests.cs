using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrueLayer.Auth;
using Xunit;

namespace TrueLayer.Tests
{
    public class TrueLayerServiceCollectionExtensionsTests
    {
        [Fact]
        public void Should_register_truelayer_client()
        {
            const string privateKey = @"-----BEGIN EC PRIVATE KEY-----
MIHcAgEBBEIALJ2sKM+8mVDfTIlk50rqB5lkxaLBt+OECvhXq3nEaB+V0nqljZ9c
5aHRN3qqxMzNLvxFQ+4twifa4ezkMK2/j5WgBwYFK4EEACOhgYkDgYYABADmhZbj
i8bgJRfMTdtzy+5VbS5ScMaKC1LQfhII+PTzGzOr+Ts7Qv8My5cmYU5qarGK3tWF
c3VMlcFZw7Y0iLjxAQFPvHqJ9vn3xWp+d3JREU1vQJ9daXswwbcoer88o1oVFmFf
WS1/11+TH1x/lgKckAws6sAzJLPtCUZLV4IZTb6ENg==
-----END EC PRIVATE KEY-----";
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("TrueLayer:ClientId", "client_id"),
                    new KeyValuePair<string, string?>("TrueLayer:ClientSecret", "secret"),
                    new KeyValuePair<string, string?>("TrueLayer:Payments:SigningKey:KeyId", Guid.NewGuid().ToString()),
                    new KeyValuePair<string, string?>("TrueLayer:Payments:SigningKey:PrivateKey", privateKey)
                })
                .Build();
            var services = new ServiceCollection()
                .AddTrueLayer(configuration)
                .BuildServiceProvider();

            var client = services.GetRequiredService<ITrueLayerClient>();
            client.Should().NotBeNull();
        }

        [Fact]
        public async Task Should_register_keyed_truelayer_clients()
        {
            const string gbpServiceKey = "GbpClient";
            const string eurServiceKey = "EurClient";
            const string privateKey = @"-----BEGIN EC PRIVATE KEY-----
MIHcAgEBBEIALJ2sKM+8mVDfTIlk50rqB5lkxaLBt+OECvhXq3nEaB+V0nqljZ9c
5aHRN3qqxMzNLvxFQ+4twifa4ezkMK2/j5WgBwYFK4EEACOhgYkDgYYABADmhZbj
i8bgJRfMTdtzy+5VbS5ScMaKC1LQfhII+PTzGzOr+Ts7Qv8My5cmYU5qarGK3tWF
c3VMlcFZw7Y0iLjxAQFPvHqJ9vn3xWp+d3JREU1vQJ9daXswwbcoer88o1oVFmFf
WS1/11+TH1x/lgKckAws6sAzJLPtCUZLV4IZTb6ENg==
-----END EC PRIVATE KEY-----";
            var gbpConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("TrueLayer:ClientId", "client_gbp"),
                    new KeyValuePair<string, string?>("TrueLayer:ClientSecret", "secret_gbp"),
                    new KeyValuePair<string, string?>("TrueLayer:Payments:SigningKey:KeyId", Guid.NewGuid().ToString()),
                    new KeyValuePair<string, string?>("TrueLayer:Payments:SigningKey:PrivateKey", privateKey)
                })
                .Build();
            var eurConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("TrueLayer:ClientId", "client_eur"),
                    new KeyValuePair<string, string?>("TrueLayer:ClientSecret", "secret_eur"),
                    new KeyValuePair<string, string?>("TrueLayer:Payments:SigningKey:KeyId", Guid.NewGuid().ToString()),
                    new KeyValuePair<string, string?>("TrueLayer:Payments:SigningKey:PrivateKey", privateKey)
                })
                .Build();
            var services = new ServiceCollection()
                .AddKeyedTrueLayer(gbpConfiguration, serviceKey: gbpServiceKey)
                .AddKeyedTrueLayer(eurConfiguration, serviceKey: eurServiceKey)
                .BuildServiceProvider();

            var gbpClient = services.GetRequiredKeyedService<ITrueLayerClient>(gbpServiceKey);
            var eurClient = services.GetRequiredKeyedService<ITrueLayerClient>(eurServiceKey);

            gbpClient.Should().NotBeNull();
            eurClient.Should().NotBeNull();
        }
    }
}
