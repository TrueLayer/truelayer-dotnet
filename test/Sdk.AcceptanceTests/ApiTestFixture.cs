using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrueLayerSdk;

namespace TrueLayer.Acceptance.Tests
{
    public class ApiTestFixture
    {
        public ApiTestFixture()
        {
            IConfiguration configuration = LoadConfiguration();

            ServiceProvider = new ServiceCollection()
                .AddTruelayerSdk(configuration)
                .BuildServiceProvider();
            
            Api = ServiceProvider.GetRequiredService<ITruelayerApi>();
        }

        public IServiceProvider ServiceProvider { get; }
        public ITruelayerApi Api { get; }

        private static IConfiguration LoadConfiguration()
            => new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", true)
                .AddEnvironmentVariables(prefix: "SDK_")
                .Build();
    }
}
