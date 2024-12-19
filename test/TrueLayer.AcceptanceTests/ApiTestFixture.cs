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

            var configName1 = "TrueLayer";
            var configName2 = "TrueLayerClient2";
            ServiceProvider = new ServiceCollection()
                .AddTrueLayer(configuration, options =>
                {
                    string privateKey = File.ReadAllText("ec512-private-key.pem");
                    if (options.Payments?.SigningKey != null)
                    {
                        options.Payments.SigningKey.PrivateKey = privateKey;
                    }
                })
                .AddTrueLayer(configuration, options =>
                {
                    string privateKey = File.ReadAllText("ec512-private-key-sbx.pem");
                    if (options.Payments?.SigningKey != null)
                    {
                        options.Payments.SigningKey.PrivateKey = privateKey;
                    }
                }, configurationSectionName: configName2)
                .AddAuthTokenInMemoryCaching(configName2)
                .BuildServiceProvider();

            Client = ServiceProvider.GetRequiredKeyedService<ITrueLayerClient>(configName1);
            Client2 = ServiceProvider.GetRequiredKeyedService<ITrueLayerClient>(configName2);
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
