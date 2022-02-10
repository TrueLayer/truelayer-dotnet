using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Xunit;
using Shouldly;
using System.Collections.Generic;

namespace TrueLayer.Tests
{
    public class TrueLayerServiceCollectionExtensionsTests
    {
        [Fact]
        public void Should_register_truelayer_client()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new KeyValuePair<string, string>[] {
                    new("TrueLayer:ClientId", "client_id"),
                    new("TrueLayer:ClientSecret", "secret")
                })
                .Build();
            var services = new ServiceCollection()
                .AddTrueLayer(configuration)
                .BuildServiceProvider();

            var client = services.GetRequiredService<ITrueLayerClient>();
            client.ShouldNotBeNull();
        }
    }
}
