using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrueLayer.Caching;

namespace TrueLayer.AcceptanceTests
{
    public class ApiTestFixture
    {
        public ApiTestFixture()
        {
            IConfiguration configuration = LoadConfiguration();

            const string serviceKey1 = "TrueLayer";
            const string serviceKey2 = "TrueLayerClient2";
            const string configName2 = "TrueLayer2";

            ServiceProvider = new ServiceCollection()
                .AddKeyedTrueLayer(serviceKey1,
                    configuration,
                    options =>
                    {
                        var privateKey = File.ReadAllText("ec512-private-key.pem");
                        if (options.Payments?.SigningKey != null)
                        {
                            options.Payments.SigningKey.PrivateKey = privateKey;
                        }
                    },
                    authTokenCachingStrategy: AuthTokenCachingStrategies.InMemory)
                .AddKeyedTrueLayer(serviceKey2,
                    configuration,
                    options =>
                    {
                        var privateKey = File.ReadAllText("ec512-private-key-sbx.pem");
                        if (options.Payments?.SigningKey != null)
                        {
                            options.Payments.SigningKey.PrivateKey = privateKey;
                        }
                    },
                    configurationSectionName: configName2,
                    authTokenCachingStrategy: AuthTokenCachingStrategies.InMemory)
                .BuildServiceProvider();

            TlClients =
            [
                ServiceProvider.GetRequiredKeyedService<ITrueLayerClient>(serviceKey1),
                ServiceProvider.GetRequiredKeyedService<ITrueLayerClient>(serviceKey2)
            ];

            ClientMerchantAccounts =
            [
                GetMerchantBeneficiaryAccountsAsync(TlClients[0]).Result,
                GetMerchantBeneficiaryAccountsAsync(TlClients[1]).Result,
            ];
        }

        public IServiceProvider ServiceProvider { get; }
        public ITrueLayerClient[] TlClients { get; }
        public (string GbpMerchantAccountId, string EurMerchantAccountId)[] ClientMerchantAccounts { get; }

        private static IConfiguration LoadConfiguration()
            => new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", true)
                .AddEnvironmentVariables()
                .Build();

        private static async Task<(string gbpMerchantAccountId, string eurMerchantAccountId)> GetMerchantBeneficiaryAccountsAsync(ITrueLayerClient client)
        {
            var merchantAccounts = await client.MerchantAccounts.ListMerchantAccounts();
            var gbpMerchantAccount = merchantAccounts.Data!.Items.First(m => m.Currency == Currencies.GBP);
            var eurMerchantAccount = merchantAccounts.Data!.Items.First(m => m.Currency == Currencies.EUR);
            return (gbpMerchantAccount.Id, eurMerchantAccount.Id);
        }
    }
}
