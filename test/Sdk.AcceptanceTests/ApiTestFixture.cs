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
                .AddTrueLayerSdk(configuration)
                .BuildServiceProvider();
            
            Api = ServiceProvider.GetRequiredService<ITrueLayerApi>();
            Options = ServiceProvider.GetRequiredService<TrueLayerOptions>();
        }

        public IServiceProvider ServiceProvider { get; }
        public ITrueLayerApi Api { get; }
        public TrueLayerOptions Options { get; }

        private static IConfiguration LoadConfiguration()
            => new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", true)
                .AddEnvironmentVariables(prefix: "SDK_")
                .Build();
    }
}
