using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TrueLayer.Merchants.Model;
using Shouldly;
using Xunit;
using System.Threading;
using System.Linq;

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
            var response = await _fixture.Client.Merchants.ListMerchants(canceller);

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
            
            var listMerchants = await _fixture.Client.Merchants.ListMerchants(canceller);
            listMerchants.StatusCode.ShouldBe(HttpStatusCode.OK, $"TraceId: {listMerchants.TraceId}");
            listMerchants.Data.ShouldNotBeNull();
            listMerchants.Data.Items.ShouldNotBeEmpty();
            var merchantId = listMerchants.Data.Items.First().Id;
            
            // Act
            var merchantResponse = await _fixture.Client.Merchants.GetMerchant(merchantId, canceller);

            // Assert
            merchantResponse.StatusCode.ShouldBe(HttpStatusCode.OK, $"TraceId: {listMerchants.TraceId}");
            merchantResponse.Data.ShouldNotBeNull();
            merchantResponse.Data.Id.ShouldBe(merchantId);
            merchantResponse.Data.AccountHolderName.ShouldNotBeNullOrWhiteSpace();
            merchantResponse.Data.Currency.ShouldNotBeNullOrWhiteSpace();
        }
    }
}
