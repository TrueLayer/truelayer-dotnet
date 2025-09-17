using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using OneOf;
using TrueLayer.AcceptanceTests.Clients;
using TrueLayer.AcceptanceTests.Helpers;
using TrueLayer.Mandates.Model;
using Xunit;

namespace TrueLayer.AcceptanceTests
{
    using Models;
    using MandateDetailUnion = OneOf<
        MandateDetail.AuthorizationRequiredMandateDetail,
        MandateDetail.AuthorizingMandateDetail,
        MandateDetail.AuthorizedMandateDetail,
        MandateDetail.FailedMandateDetail,
        MandateDetail.RevokedMandateDetail>;

    public class MandatesTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;
        private const string ReturnUri = "http://localhost:3000/callback";

        public MandatesTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [MemberData(nameof(MandatesTestCases.CreateTestSweepingUserSelectedMandateRequests), MemberType = typeof(MandatesTestCases))]
        [MemberData(nameof(MandatesTestCases.CreateTestCommercialUserSelectedMandateRequests), MemberType = typeof(MandatesTestCases))]
        [MemberData(nameof(MandatesTestCases.CreateTestSweepingPreselectedMandateRequests), MemberType = typeof(MandatesTestCases))]
        [MemberData(nameof(MandatesTestCases.CreateTestCommercialPreselectedMandateRequests), MemberType = typeof(MandatesTestCases))]
        public async Task Can_Get_Mandate(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var client = _fixture.TlClients[0];
            var createResponse = await client.Mandates.CreateMandate(mandateRequest);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var mandateId = createResponse.Data!.Id;
            var mandateType = mandateRequest.Mandate.Match(
                commercial => MandateType.Commercial,
                sweeping => MandateType.Sweeping);

            // Act
            var response = await client.Mandates.GetMandate(mandateId, mandateType);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(createResponse.Data.User.Id, response.Data.AsT0.User!.Id);
        }

        [Theory]
        [MemberData(nameof(MandatesTestCases.CreateTestSweepingUserSelectedMandateRequests), MemberType = typeof(MandatesTestCases))]
        [MemberData(nameof(MandatesTestCases.CreateTestCommercialUserSelectedMandateRequests), MemberType = typeof(MandatesTestCases))]
        [MemberData(nameof(MandatesTestCases.CreateTestSweepingPreselectedMandateRequests), MemberType = typeof(MandatesTestCases))]
        [MemberData(nameof(MandatesTestCases.CreateTestCommercialPreselectedMandateRequests), MemberType = typeof(MandatesTestCases))]
        public async Task Can_List_Mandate(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var client = _fixture.TlClients[0];
            var createResponse = await client.Mandates.CreateMandate(mandateRequest);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            // Act
            var response = await client.Mandates
                .ListMandates(new ListMandatesQuery(createResponse.Data!.User.Id, null, 10), MandateType.Sweeping);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Data!.Items.Count() <= 10);
        }

        [Theory]
        [MemberData(nameof(MandatesTestCases.CreateTestSweepingPreselectedMandateRequests), MemberType = typeof(MandatesTestCases))]
        public async Task Can_Start_Preselected_Authorization(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var client = _fixture.TlClients[0];
            var createResponse = await client.Mandates.CreateMandate(mandateRequest);
            var mandateId = createResponse.Data!.Id;
            StartAuthorizationFlowRequest authorizationRequest = new(
                new ProviderSelectionRequest(),
                new Redirect(new Uri(ReturnUri)));

            // Act
            var response = await client.Mandates.StartAuthorizationFlow(
                mandateId, authorizationRequest, idempotencyKey: Guid.NewGuid().ToString(), MandateType.Sweeping);

            var providerReturnUri= await _fixture.MockBankClient.AuthoriseMandateAsync(
                response.Data.AsT0.AuthorizationFlow.Actions.Next.AsT4.Uri, MockBankMandateAction.Authorise);

            var query = providerReturnUri.Query.Replace("mandate-", string.Empty);
            await _fixture.ApiClient.SubmitPaymentsProviderReturnAsync(query, providerReturnUri.Fragment);

            var mandate = await PollMandateForTerminalStatusAsync(
                client,
                mandateId,
                MandateType.Sweeping,
                typeof(MandateDetail.AuthorizedMandateDetail));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("authorized", mandate.AsT2.Status);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        }

        [Theory]
        [MemberData(nameof(MandatesTestCases.CreateTestSweepingUserSelectedMandateRequests), MemberType = typeof(MandatesTestCases))]
        [MemberData(nameof(MandatesTestCases.CreateTestCommercialUserSelectedMandateRequests), MemberType = typeof(MandatesTestCases))]
        public async Task Can_Complete_UserSelected_Full_Auth_Flow(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var client = _fixture.TlClients[0];
            const string providerId = "mock-payments-gb-redirect";
            var createResponse = await client.Mandates.CreateMandate(mandateRequest);

            var mandateId = createResponse.Data!.Id;
            var mandateType = mandateRequest.Mandate.Match(
                commercial => MandateType.Commercial,
                sweeping => MandateType.Sweeping);

            var authorizationRequest = new StartAuthorizationFlowRequest(
                new ProviderSelectionRequest(),
                new Redirect(new Uri(ReturnUri)),
                new Consent());

            await client.Mandates.StartAuthorizationFlow(
                mandateId, authorizationRequest, idempotencyKey: Guid.NewGuid().ToString(), mandateType);

            var submitProviderRequest = new SubmitProviderSelectionRequest(providerId);

            await client.Mandates.SubmitProviderSelection(
                mandateId, submitProviderRequest, idempotencyKey: Guid.NewGuid().ToString(), mandateType);

            // Act
            var response = await client.Mandates.SubmitConsent(
                mandateId, idempotencyKey: Guid.NewGuid().ToString(), mandateType);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(MandatesTestCases.CreateTestSweepingPreselectedMandateRequests), MemberType = typeof(MandatesTestCases))]
        public async Task Can_Get_Funds(CreateMandateRequest createMandateRequest)
        {
            //Arrange
            var mandateId = await CreateAuthorizedSweepingMandate(createMandateRequest);

            // Act
            var fundsResponse = await _fixture.TlClients[0].Mandates.GetConfirmationOfFunds(
                mandateId,
                1,
                "GBP",
                MandateType.Sweeping);

            // Assert
            Assert.Equal(HttpStatusCode.OK, fundsResponse.StatusCode);
            Assert.Equal(DateTime.UtcNow.Date, fundsResponse.Data!.ConfirmedAt.Date);
            Assert.True(fundsResponse.Data!.Confirmed);
        }

        [Theory]
        [MemberData(nameof(MandatesTestCases.CreateTestSweepingPreselectedMandateRequests), MemberType = typeof(MandatesTestCases))]
        public async Task Can_Get_Mandate_Constraints(CreateMandateRequest createMandateRequest)
        {
            //Arrange
            var mandateId = await CreateAuthorizedSweepingMandate(createMandateRequest);

            // Act
            var response = await _fixture.TlClients[0].Mandates.GetMandateConstraints(
                mandateId,
                MandateType.Sweeping);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(MandatesTestCases.CreateTestSweepingPreselectedMandateRequests), MemberType = typeof(MandatesTestCases))]
        public async Task Can_Revoke_Mandate(CreateMandateRequest mandateRequest)
        {
            //Arrange
            string mandateId = await CreateAuthorizedSweepingMandate(mandateRequest);

            // Act
            var response = await _fixture.TlClients[0].Mandates.RevokeMandate(
                mandateId,
                idempotencyKey: Guid.NewGuid().ToString(),
                MandateType.Sweeping);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(MandatesTestCases.CreateTestSweepingPreselectedMandateRequests), MemberType = typeof(MandatesTestCases))]
        public async Task Can_Create_Mandate_Payment(CreateMandateRequest mandateRequest)
        {
            // Arrange
            string mandateId = await CreateAuthorizedSweepingMandate(mandateRequest);
            var paymentRequest = RequestBuilders.CreateTestMandatePaymentRequest(mandateRequest, mandateId, false);

            // Act
            var response = await _fixture.TlClients[0].Payments.CreatePayment(paymentRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.True(response.Data.IsT1);
            Assert.False(string.IsNullOrWhiteSpace(response.Data.AsT1.Id));
            Assert.False(string.IsNullOrWhiteSpace(response.Data.AsT1.ResourceToken));
            Assert.NotNull(response.Data.AsT1.User);
            Assert.False(string.IsNullOrWhiteSpace(response.Data.AsT1.User.Id));
            Assert.Equal("authorized", response.Data.AsT1.Status);
        }

        [Fact]
        public async Task GetMandate_Url_As_MandateId_Should_Throw_Exception()
        {
            var client = _fixture.TlClients[0];
            const string mandateId = "https://test.com";

            var result = await Assert.ThrowsAsync<ArgumentException>(() =>
                client.Mandates.GetMandate(mandateId, MandateType.Sweeping));
            Assert.Equal("Value is malformed (Parameter 'mandateId')", result.Message);
        }

        [Fact]
        public async Task StartAuthFlow_Url_As_MandateId_Should_Throw_Exception()
        {
            var client = _fixture.TlClients[0];
            const string mandateId = "https://test.com";

            var result = await Assert.ThrowsAsync<ArgumentException>(() =>
                client.Mandates.StartAuthorizationFlow(mandateId,
                    new StartAuthorizationFlowRequest(new ProviderSelectionRequest(), new Redirect(new Uri("https://api.com"))),
                    Guid.NewGuid().ToString(), MandateType.Sweeping));
            Assert.Equal("Value is malformed (Parameter 'mandateId')", result.Message);
        }

        [Fact]
        public async Task SubmitProviderSelection_Url_As_MandateId_Should_Throw_Exception()
        {
            var client = _fixture.TlClients[0];
            const string mandateId = "https://test.com";

            var result = await Assert.ThrowsAsync<ArgumentException>(() =>
                client.Mandates.SubmitProviderSelection(mandateId, new SubmitProviderSelectionRequest("provider"), Guid.NewGuid().ToString(), MandateType.Sweeping));
            Assert.Equal("Value is malformed (Parameter 'mandateId')", result.Message);
        }

        [Fact]
        public async Task SubmitConsent_Url_As_MandateId_Should_Throw_Exception()
        {
            var client = _fixture.TlClients[0];
            const string mandateId = "https://test.com";

            var result = await Assert.ThrowsAsync<ArgumentException>(() =>
                client.Mandates.SubmitConsent(mandateId, Guid.NewGuid().ToString(), MandateType.Sweeping));
            Assert.Equal("Value is malformed (Parameter 'mandateId')", result.Message);
        }

        [Fact]
        public async Task GetConfirmationOfFunds_Url_As_MandateId_Should_Throw_Exception()
        {
            var client = _fixture.TlClients[0];
            const string mandateId = "https://test.com";

            var result = await Assert.ThrowsAsync<ArgumentException>(() =>
                client.Mandates.GetConfirmationOfFunds(mandateId, 100, Currencies.GBP, MandateType.Sweeping));
            Assert.Equal("Value is malformed (Parameter 'mandateId')", result.Message);
        }

        [Fact]
        public async Task GetMandateConstraints_Url_As_MandateId_Should_Throw_Exception()
        {
            var client = _fixture.TlClients[0];
            const string mandateId = "https://test.com";

            var result = await Assert.ThrowsAsync<ArgumentException>(() =>
                client.Mandates.GetMandateConstraints(mandateId, MandateType.Sweeping));
            Assert.Equal("Value is malformed (Parameter 'mandateId')", result.Message);
        }

        [Fact]
        public async Task RevokeMandate_Url_As_MandateId_Should_Throw_Exception()
        {
            var client = _fixture.TlClients[0];
            const string mandateId = "https://test.com";

            var result = await Assert.ThrowsAsync<ArgumentException>(() =>
                client.Mandates.RevokeMandate(mandateId, Guid.NewGuid().ToString(), MandateType.Sweeping));
            Assert.Equal("Value is malformed (Parameter 'mandateId')", result.Message);
        }

        private async Task<string> CreateAuthorizedSweepingMandate(CreateMandateRequest mandateRequest)
        {
            var client = _fixture.TlClients[0];
            var createResponse = await client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            var mandateId = createResponse.Data!.Id;

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var authorizationRequest = new StartAuthorizationFlowRequest(
                new ProviderSelectionRequest(),
                new Redirect(new Uri(ReturnUri)));

            var startAuthResponse = await client.Mandates.StartAuthorizationFlow(
                mandateId, authorizationRequest, idempotencyKey: Guid.NewGuid().ToString(), MandateType.Sweeping);

            var providerReturnUri = await _fixture.MockBankClient.AuthoriseMandateAsync(
                startAuthResponse.Data.AsT0.AuthorizationFlow.Actions.Next.AsT4.Uri, MockBankMandateAction.Authorise);
            var query = providerReturnUri.Query.Replace("mandate-", string.Empty);
            await _fixture.ApiClient.SubmitPaymentsProviderReturnAsync(query, providerReturnUri.Fragment);

            await PollMandateForTerminalStatusAsync(
                client,
                mandateId,
                MandateType.Sweeping,
                typeof(MandateDetail.AuthorizedMandateDetail));

            return mandateId;
        }

        private static async Task<MandateDetailUnion> PollMandateForTerminalStatusAsync(
            ITrueLayerClient trueLayerClient,
            string mandateId,
            MandateType mandateType,
            Type expectedStatus)
        {
            var getMandateResponse = await Waiter.WaitAsync(
                () => trueLayerClient.Mandates.GetMandate(mandateId, mandateType),
                r => r.Data.GetType() == expectedStatus);

            Assert.True(getMandateResponse.IsSuccessful);
            return getMandateResponse.Data;
        }
    }
}
