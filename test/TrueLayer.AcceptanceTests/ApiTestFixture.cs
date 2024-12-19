using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TrueLayer.AcceptanceTests
{
    public class ApiTestFixture
    {
        public ApiTestFixture()
        {
            IConfiguration configuration = LoadConfiguration();

            const string configName1 = "TrueLayer";
            const string configName2 = "TrueLayer2";
            const string serviceKey1 = "TrueLayerClient";
            const string serviceKey2 = "TrueLayerClient2";

            ServiceProvider = new ServiceCollection()
                .AddTrueLayer(configuration, options =>
                    {
                        var privateKey = File.ReadAllText("ec512-private-key.pem");
                        if (options.Payments?.SigningKey != null)
                        {
                            options.Payments.SigningKey.PrivateKey = privateKey;
                        }
                    },
                    configurationSectionName: configName1,
                    serviceKey: serviceKey1)
                .AddTrueLayer(configuration, options =>
                    {
                        var privateKey = File.ReadAllText("ec512-private-key.pem");
                        if (options.Payments?.SigningKey != null)
                        {
                            options.Payments.SigningKey.PrivateKey = privateKey;
                        }
                    },
                    configurationSectionName: configName2,
                    serviceKey: serviceKey2)
                .AddAuthTokenInMemoryCaching(configName2)
                .BuildServiceProvider();

            Client = ServiceProvider.GetRequiredKeyedService<ITrueLayerClient>(serviceKey1);
            Client2 = ServiceProvider.GetRequiredKeyedService<ITrueLayerClient>(serviceKey2);
        }

        public IServiceProvider ServiceProvider { get; }
        public ITrueLayerClient Client { get; }
        public ITrueLayerClient Client2 { get; }

        private static IConfiguration LoadConfiguration()
            => new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", true)
                .AddEnvironmentVariables()
                .Build();
    }
}
