using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrueLayer.PaymentsProviders.Model;
using Xunit;
using AuthorizationFlow = TrueLayer.PaymentsProviders.Model.AuthorizationFlow;

namespace TrueLayer.AcceptanceTests
{
    public class PaymentProvidersTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;

        public PaymentProvidersTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Can_get_payments_provider_with_payments_capabilities()
        {
            const string providerId = "mock-payments-gb-redirect";

            var response = await _fixture.TlClients[0].PaymentsProviders.GetPaymentsProvider(providerId);

            Assert.True(response.IsSuccessful);
            Assert.NotNull(response.Data);
            Assert.Equal(providerId, response.Data!.Id);
            Assert.False(string.IsNullOrWhiteSpace(response.Data.DisplayName));
            Assert.False(string.IsNullOrWhiteSpace(response.Data.IconUri));
            Assert.False(string.IsNullOrWhiteSpace(response.Data.LogoUri));
            Assert.False(string.IsNullOrWhiteSpace(response.Data.BgColor));
            Assert.False(string.IsNullOrWhiteSpace(response.Data.CountryCode));
            Assert.NotNull(response.Data.Capabilities.Payments?.BankTransfer);
            Assert.False(string.IsNullOrWhiteSpace(response.Data.Capabilities.Payments?.BankTransfer?.ReleaseChannel));
            Assert.True(response.Data.Capabilities.Payments?.BankTransfer?.Schemes.Count() >= 1);
        }

        [Fact]
        public async Task Can_get_payments_provider_with_mandates_capabilities()
        {
            const string providerId = "ob-uki-mock-bank-sbox";

            var response = await _fixture.TlClients[0].PaymentsProviders.GetPaymentsProvider(providerId);

            Assert.True(response.IsSuccessful);
            Assert.NotNull(response.Data);
            Assert.Equal(providerId, response.Data!.Id);
            Assert.False(string.IsNullOrWhiteSpace(response.Data.DisplayName));
            Assert.False(string.IsNullOrWhiteSpace(response.Data.CountryCode));
            Assert.NotNull(response.Data.Capabilities.Mandates?.VrpSweeping);
            Assert.False(string.IsNullOrWhiteSpace(response.Data.Capabilities.Mandates?.VrpSweeping?.ReleaseChannel));
        }

        [Theory]
        [MemberData(nameof(SearchPaymentProvidersData))]
        public async Task Can_search_payments_providers(
            List<string>? countries = null,
            List<string>? currencies = null,
            string? releaseChannel = null,
            List<string>? customerSegments = null)
        {
            var searchRequest = new SearchPaymentsProvidersRequest(
                new AuthorizationFlow(new AuthorizationFlowConfiguration()),
                countries,
                currencies,
                releaseChannel,
                customerSegments
            );

            var response = await _fixture.TlClients[0].PaymentsProviders.SearchPaymentsProviders(searchRequest);

            Assert.True(response.IsSuccessful);
            Assert.NotNull(response.Data);
            Assert.NotNull(response.Data!.Items);
            Assert.NotEmpty(response.Data.Items);
            response.Data.Items.ForEach(pp =>
            {
                Assert.NotEmpty(pp.Id);
                Assert.False(string.IsNullOrWhiteSpace(pp.DisplayName));
                Assert.False(string.IsNullOrWhiteSpace(pp.CountryCode));
                Assert.False(string.IsNullOrWhiteSpace(pp.CountryCode));
                if (countries != null && countries.Any())
                {
                    Assert.Contains(pp.CountryCode, countries);
                }
                Assert.NotNull(pp.Capabilities.Mandates?.VrpSweeping);
                Assert.False(string.IsNullOrWhiteSpace(pp.Capabilities.Mandates?.VrpSweeping?.ReleaseChannel));
            });
        }

        public static IEnumerable<object?[]> SearchPaymentProvidersData()
        {
            yield return new object?[] { null, null, null, null };
            yield return new object?[] { new List<string> { "GB" }, null, null, null };
            yield return new object?[] { new List<string> { "GB" }, new List<string> { "GBP" }, null, null };
            yield return new object?[] { new List<string> { "GB" }, new List<string> { "GBP" }, "general_availability", null };
            yield return new object?[] { new List<string> { "GB" }, new List<string> { "GBP" }, "private_beta", null };
            yield return new object?[] { new List<string> { "GB" }, new List<string> { "GBP" }, "public_beta", null };
            yield return new object?[] { new List<string> { "GB" }, new List<string> { "GBP" }, "general_availability", new List<string> { "retail" } };
            yield return new object?[] { new List<string> { "GB" }, new List<string> { "GBP" }, "general_availability", new List<string> { "retail", "corporate" } };
        }
    }
}
