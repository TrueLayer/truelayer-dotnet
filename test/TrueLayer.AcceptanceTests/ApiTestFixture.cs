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

            ServiceProvider = new ServiceCollection()
                .AddTrueLayer(configuration, options =>
                {   
                    if (options.Payments?.SigningKey != null)
                    {
                        options.Payments.SigningKey.PrivateKey = File.ReadAllText("ec512-private-key.pem");
                    }
                })
                .BuildServiceProvider();

            Client = ServiceProvider.GetRequiredService<ITrueLayerClient>();
        }

        public IServiceProvider ServiceProvider { get; }
        public ITrueLayerClient Client { get; }

        private static IConfiguration LoadConfiguration()
            => new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", true)
                .AddEnvironmentVariables()
                .Build();
    }
}
