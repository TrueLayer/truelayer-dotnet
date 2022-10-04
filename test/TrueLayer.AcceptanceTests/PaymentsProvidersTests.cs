using System.Threading.Tasks;
using OneOf;
using Shouldly;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.AcceptanceTests
{
    using ProviderUnion = OneOf<Provider.UserSelected, Provider.Preselected>;
    using AccountIdentifierUnion = OneOf<AccountIdentifier.SortCodeAccountNumber, AccountIdentifier.Iban>;

    public class PaymentProvidersTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;
        private const string ProviderId = "mock-payments-gb-redirect";

        public PaymentProvidersTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Can_get_payments_provider()
        {
            var response = await _fixture.Client.PaymentsProviders.GetPaymentsProvider(ProviderId);

            response.IsSuccessful.ShouldBeTrue();
            response.Data.ShouldNotBeNull();
            response.Data.Id.ShouldBe(ProviderId);
            response.Data.DisplayName.ShouldNotBeNull();
            response.Data.IconUri.ShouldNotBeNull();
            response.Data.LogoUri.ShouldNotBeNull();
            response.Data.BgColor.ShouldNotBeNull();
            response.Data.CountryCode.ShouldNotBeNull();
            response.Data.Capabilities.Payments.BankTransfer.ShouldNotBeNull();
            response.Data.Capabilities.Payments.BankTransfer.Schemes.ShouldHaveSingleItem();
        }
    }
}
