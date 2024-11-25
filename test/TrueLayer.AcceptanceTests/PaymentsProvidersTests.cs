using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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

            var response = await _fixture.Client.PaymentsProviders.GetPaymentsProvider(providerId);

            response.IsSuccessful.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data!.Id.Should().Be(providerId);
            response.Data.DisplayName.Should().NotBeNullOrWhiteSpace();
            response.Data.IconUri.Should().NotBeNullOrWhiteSpace();
            response.Data.LogoUri.Should().NotBeNullOrWhiteSpace();
            response.Data.BgColor.Should().NotBeNullOrWhiteSpace();
            response.Data.CountryCode.Should().NotBeNullOrWhiteSpace();
            response.Data.Capabilities.Payments?.BankTransfer.Should().NotBeNull();
            response.Data.Capabilities.Payments?.BankTransfer?.ReleaseChannel.Should().NotBeNullOrWhiteSpace();
            response.Data.Capabilities.Payments?.BankTransfer?.Schemes.Count().Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public async Task Can_get_payments_provider_with_mandates_capabilities()
        {
            const string providerId = "ob-uki-mock-bank-sbox";

            var response = await _fixture.Client.PaymentsProviders.GetPaymentsProvider(providerId);

            response.IsSuccessful.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data!.Id.Should().Be(providerId);
            response.Data.DisplayName.Should().NotBeNullOrWhiteSpace();
            response.Data.CountryCode.Should().NotBeNullOrWhiteSpace();
            response.Data.Capabilities.Mandates?.VrpSweeping.Should().NotBeNull();
            response.Data.Capabilities.Mandates?.VrpSweeping?.ReleaseChannel.Should().NotBeNullOrWhiteSpace();
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

            response.IsSuccessful.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data!.Items.Should().NotBeNull();
            response.Data.Items.Should().NotBeEmpty();
            response.Data.Items.ForEach(pp =>
            {
                pp.Id.Should().NotBeEmpty();
                pp.DisplayName.Should().NotBeNullOrWhiteSpace();
                pp.CountryCode.Should().NotBeNullOrWhiteSpace();
                pp.CountryCode.Should().NotBeNullOrWhiteSpace();
                if (countries != null && countries.Any())
                {
                    countries.Should().Contain(pp.CountryCode);
                }
                pp.Capabilities.Mandates?.VrpSweeping.Should().NotBeNull();
                pp.Capabilities.Mandates?.VrpSweeping?.ReleaseChannel.Should().NotBeNullOrWhiteSpace();
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
