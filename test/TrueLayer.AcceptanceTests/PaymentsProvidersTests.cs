using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OneOf;
using Shouldly;
using TrueLayer.Models;
using TrueLayer.Payments.Model;
using TrueLayer.PaymentsProviders.Model;
using Xunit;
using AuthorizationFlow = TrueLayer.PaymentsProviders.Model.AuthorizationFlow;
using Provider = TrueLayer.Payments.Model.Provider;

namespace TrueLayer.AcceptanceTests
{
    using AccountIdentifierUnion = OneOf<AccountIdentifier.SortCodeAccountNumber, AccountIdentifier.Iban>;
    using ProviderUnion = OneOf<Provider.UserSelected, Provider.Preselected>;

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

            var response = await _fixture.Client.PaymentsProviders.GetPaymentsProvider(providerId);

            response.IsSuccessful.ShouldBeTrue();
            response.Data.ShouldNotBeNull();
            response.Data.Id.ShouldBe(providerId);
            response.Data.DisplayName.ShouldNotBeNullOrWhiteSpace();
            response.Data.IconUri.ShouldNotBeNullOrWhiteSpace();
            response.Data.LogoUri.ShouldNotBeNullOrWhiteSpace();
            response.Data.BgColor.ShouldNotBeNullOrWhiteSpace();
            response.Data.CountryCode.ShouldNotBeNullOrWhiteSpace();
            response.Data.Capabilities.Payments?.BankTransfer.ShouldNotBeNull();
            response.Data.Capabilities.Payments?.BankTransfer?.ReleaseChannel.ShouldNotBeNullOrWhiteSpace();
            response.Data.Capabilities.Payments?.BankTransfer?.Schemes.Count().ShouldBeGreaterThanOrEqualTo(1);
        }

        [Fact]
        public async Task Can_get_payments_provider_with_mandates_capabilities()
        {
            const string providerId = "ob-uki-mock-bank-sbox";

            var response = await _fixture.Client.PaymentsProviders.GetPaymentsProvider(providerId);

            response.IsSuccessful.ShouldBeTrue();
            response.Data.ShouldNotBeNull();
            response.Data.Id.ShouldBe(providerId);
            response.Data.DisplayName.ShouldNotBeNullOrWhiteSpace();
            response.Data.CountryCode.ShouldNotBeNullOrWhiteSpace();
            response.Data.Capabilities.Mandates?.VrpSweeping.ShouldNotBeNull();
            response.Data.Capabilities.Mandates?.VrpSweeping?.ReleaseChannel.ShouldNotBeNullOrWhiteSpace();
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

            var response = await _fixture.Client.PaymentsProviders.SearchPaymentsProviders(searchRequest);

            response.IsSuccessful.ShouldBeTrue();
            response.Data.ShouldNotBeNull();
            response.Data.Items.ShouldNotBeNull().ShouldNotBeEmpty();
            response.Data.Items.ForEach(pp =>
            {
                pp.Id.ShouldNotBeEmpty();
                pp.DisplayName.ShouldNotBeNullOrWhiteSpace();
                pp.CountryCode.ShouldNotBeNullOrWhiteSpace();
                pp.CountryCode.ShouldNotBeNullOrWhiteSpace();
                if (countries != null && countries.Any())
                {
                    countries.ShouldContain(pp.CountryCode);
                }
                pp.Capabilities.Mandates?.VrpSweeping.ShouldNotBeNull();
                pp.Capabilities.Mandates?.VrpSweeping?.ReleaseChannel.ShouldNotBeNullOrWhiteSpace();
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
