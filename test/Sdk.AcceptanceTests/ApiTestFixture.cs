using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TrueLayer.Sdk.Acceptance.Tests
{
    public class ApiTestFixture
    {
        public ApiTestFixture()
        {
            IConfiguration configuration = LoadConfiguration();

            ServiceProvider = new ServiceCollection()
                .AddTrueLayerSdk(configuration, options => {
                    options.Payouts.SigningKey.Certificate = File.ReadAllText("ec512-private-key.pem");
                    options.PayDirect.SigningKey.Certificate = File.ReadAllText("ec512-private-key.pem");
                })
                .BuildServiceProvider();
            
            Api = ServiceProvider.GetRequiredService<ITrueLayerApi>();
        }

        public IServiceProvider ServiceProvider { get; }
        public ITrueLayerApi Api { get; }

        private static IConfiguration LoadConfiguration()
            => new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", true)
                .AddEnvironmentVariables(prefix: "SDK_")
                .Build();
    }
}
