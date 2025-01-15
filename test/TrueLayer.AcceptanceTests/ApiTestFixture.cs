using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrueLayer.AcceptanceTests.Clients;
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

            var serviceProvider = new ServiceCollection()
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
                        var privateKey = File.ReadAllText("ec512-private-key.pem");
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
                serviceProvider.GetRequiredKeyedService<ITrueLayerClient>(serviceKey1),
                serviceProvider.GetRequiredKeyedService<ITrueLayerClient>(serviceKey2)
            ];

            ClientMerchantAccounts =
            [
                GetMerchantBeneficiaryAccountsAsync(TlClients[0]).Result,
                GetMerchantBeneficiaryAccountsAsync(TlClients[1]).Result,
            ];

            MockBankClient = new MockBankClient(new HttpClient
            {
                BaseAddress = new Uri("https://pay-mock-connect.truelayer-sandbox.com/")
            });
            PayApiClient = new PayApiClient(new HttpClient
            {
                BaseAddress = new Uri("https://pay-api.truelayer-sandbox.com")
            });
            ApiClient = new ApiClient(new HttpClient
            {
                BaseAddress = new Uri("https://api.truelayer-sandbox.com")
            });
        }

        public readonly ITrueLayerClient[] TlClients;
        public readonly (string GbpMerchantAccountId, string EurMerchantAccountId)[] ClientMerchantAccounts;
        public readonly MockBankClient MockBankClient;
        public readonly PayApiClient PayApiClient;
        public readonly ApiClient ApiClient;

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
