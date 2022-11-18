// See https://aka.ms/new-console-template for more information

using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrueLayer;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.local.json", true)
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection();
services.AddTrueLayer(configuration, options =>
{
    string privateKey = File.ReadAllText("ec512-private-key.pem");
    if (options.Payments?.SigningKey != null)
    {
        options.Payments.SigningKey.PrivateKey = privateKey;
    }
});
var provider = services.BuildServiceProvider();

var tlClient = provider.GetRequiredService<ITrueLayerClient>();

var paymentProvider = await tlClient.PaymentsProviders.GetPaymentsProvider("mock-payments-gb-redirect");

if (paymentProvider.IsSuccessful)
{
    Console.WriteLine(paymentProvider?.Data?.DisplayName ?? "null");
}
else
{
    Console.WriteLine(":(");
}
