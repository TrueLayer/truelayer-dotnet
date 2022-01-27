using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TrueLayer.MerchantAccounts.Model;
using Shouldly;
using Xunit;
using System.Threading;
using System.Linq;
using TrueLayer.Payments.Model;

namespace TrueLayer.AcceptanceTests
{
    public class MerchantTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;

        public MerchantTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Can_get_merchant_accounts()
        {
            // Arrange
            var canceller = new CancellationTokenSource(5000).Token;

            // Act
            var response = await _fixture.Client.MerchantAccounts.ListMerchantAccounts(canceller);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK, $"TraceId: {response.TraceId}");
            response.Data.ShouldNotBeNull();
            response.Data.Items.ShouldBeOfType<List<MerchantAccount>>();
        }

        [Fact]
        public async Task Can_get_specific_merchant_account()
        {
            // Arrange
            var canceller = new CancellationTokenSource(5000).Token;

            var listMerchants = await _fixture.Client.MerchantAccounts.ListMerchantAccounts(canceller);
            listMerchants.StatusCode.ShouldBe(HttpStatusCode.OK, $"TraceId: {listMerchants.TraceId}");
            listMerchants.Data.ShouldNotBeNull();
            listMerchants.Data.Items.ShouldNotBeEmpty();
            var merchantId = listMerchants.Data.Items.First().Id;

            // Act
            var merchantResponse = await _fixture.Client.MerchantAccounts.GetMerchantAccount(merchantId, canceller);

            // Assert
            merchantResponse.StatusCode.ShouldBe(HttpStatusCode.OK, $"TraceId: {listMerchants.TraceId}");
            merchantResponse.Data.ShouldNotBeNull();
            merchantResponse.Data.Id.ShouldBe(merchantId);
            merchantResponse.Data.AccountHolderName.ShouldNotBeNullOrWhiteSpace();
            merchantResponse.Data.Currency.ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Can_get_user_payment_sources()
        {
            var listMerchants = await _fixture.Client.MerchantAccounts.ListMerchantAccounts();
            listMerchants.Data.ShouldNotBeNull();
            listMerchants.Data.Items.ShouldNotBeEmpty();
            var merchantId = listMerchants.Data!.Items.First(x => x.Currency == "GBP").Id;

            CreatePaymentRequest paymentRequest = CreatePaymentRequest(merchantId);

            var createPaymentResponse = await _fixture.Client.Payments.CreatePayment(
                paymentRequest, idempotencyKey: Guid.NewGuid().ToString());

            createPaymentResponse.IsSuccessful.ShouldBeTrue();
            var createPaymentUser = createPaymentResponse.Data!.User;


            var getUserExternalAccountsResponse
                = await _fixture.Client.MerchantAccounts.GetUserPaymentSources(merchantId, createPaymentUser.Id);

            getUserExternalAccountsResponse.IsSuccessful.ShouldBeTrue();
            getUserExternalAccountsResponse.Data.ShouldNotBeNull();
            var externalAccounts = getUserExternalAccountsResponse.Data!;
            externalAccounts.Items.ShouldBeOfType<UserPaymentSource[]>();
            externalAccounts.Items.ShouldBeEmpty();
        }

        private static CreatePaymentRequest CreatePaymentRequest(string merchantId)
            => new CreatePaymentRequest(
                100,
                Currencies.GBP,
                new PaymentMethod.BankTransfer
                {
                    Provider = new Provider.UserSelection
                    {
                        Filter = new ProviderFilter { ProviderIds = new[] { "mock-payments-gb-redirect" } }
                    }
                },
                new Beneficiary.MerchantAccount(merchantId),
                new PaymentUserRequest("Jane Doe", email: "jane.doe@example.com", phone: "+442079460087")
            );
    }
}
