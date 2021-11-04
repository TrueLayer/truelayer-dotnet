using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TrueLayer.Merchants.Model;
using Shouldly;
using Xunit;

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
            // Act
            var response = await _fixture.Client.Merchants.ListMerchants();

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Data.ShouldNotBeNull();
            response.Data.items.ShouldBeOfType<List<MerchantAccount>>();
        }
    }
}
