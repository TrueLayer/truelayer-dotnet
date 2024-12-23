using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
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
        [MemberData(nameof(MandateTestCases.CreateTestSweepingUserSelectedMandateRequests), MemberType = typeof(MandateTestCases))]
        [MemberData(nameof(MandateTestCases.CreateTestCommercialUserSelectedMandateRequests), MemberType = typeof(MandateTestCases),
            Skip = "It returns forbidden. Need to investigate.")]
        [MemberData(nameof(MandateTestCases.CreateTestSweepingPreselectedMandateRequests), MemberType = typeof(MandateTestCases))]
        [MemberData(nameof(MandateTestCases.CreateTestCommercialPreselectedMandateRequests), MemberType = typeof(MandateTestCases),
            Skip = "It returns forbidden. Need to investigate.")]
        public async Task Can_Get_Mandate(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var client = _fixture.TlClients[0];
            var createResponse = await client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var mandateId = createResponse.Data!.Id;

            // Act
            var response = await client.Mandates.GetMandate(mandateId, MandateType.Sweeping);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Data.AsT0.User!.Id.Should().Be(createResponse.Data.User.Id);
        }

        [Theory]
        [MemberData(nameof(MandateTestCases.CreateTestSweepingUserSelectedMandateRequests), MemberType = typeof(MandateTestCases))]
        [MemberData(nameof(MandateTestCases.CreateTestCommercialUserSelectedMandateRequests), MemberType = typeof(MandateTestCases))]
        [MemberData(nameof(MandateTestCases.CreateTestSweepingPreselectedMandateRequests), MemberType = typeof(MandateTestCases))]
        [MemberData(nameof(MandateTestCases.CreateTestCommercialPreselectedMandateRequests), MemberType = typeof(MandateTestCases))]
        public async Task Can_List_Mandate(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var client = _fixture.TlClients[0];
            var createResponse = await client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Act
            var response = await client.Mandates
                .ListMandates(new ListMandatesQuery(createResponse.Data!.User.Id, null, 10), MandateType.Sweeping);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Data!.Items.Count().Should().BeLessThanOrEqualTo(10);
        }

        [Theory]
        [MemberData(nameof(MandateTestCases.CreateTestSweepingPreselectedMandateRequests), MemberType = typeof(MandateTestCases))]
        public async Task Can_Start_Preselected_Authorization(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var client = _fixture.TlClients[0];
            var createResponse = await client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
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
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            mandate.AsT2.Status.Should().Be("authorized");
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Theory]
        [MemberData(nameof(MandateTestCases.CreateTestSweepingUserSelectedMandateRequests), MemberType = typeof(MandateTestCases))]
        [MemberData(nameof(MandateTestCases.CreateTestCommercialUserSelectedMandateRequests), MemberType = typeof(MandateTestCases))]
        public async Task Can_Complete_UserSelected_Full_Auth_Flow(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var client = _fixture.TlClients[0];
            const string providerId = "mock-payments-gb-redirect";
            var createResponse = await client.Mandates
                .CreateMandate(mandateRequest, Guid.NewGuid().ToString());

            var mandateId = createResponse.Data!.Id;
            var mandateType = mandateRequest.Mandate.IsT0 ? MandateType.Commercial : MandateType.Sweeping;

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
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [MemberData(nameof(MandateTestCases.CreateTestSweepingPreselectedMandateRequests), MemberType = typeof(MandateTestCases))]
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
            fundsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            fundsResponse.Data!.ConfirmedAt.Date.Should().Be(DateTime.UtcNow.Date);
            fundsResponse.Data!.Confirmed.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(MandateTestCases.CreateTestSweepingPreselectedMandateRequests), MemberType = typeof(MandateTestCases))]
        public async Task Can_Get_Mandate_Constraints(CreateMandateRequest createMandateRequest)
        {
            //Arrange
            var mandateId = await CreateAuthorizedSweepingMandate(createMandateRequest);

            // Act
            var response = await _fixture.TlClients[0].Mandates.GetMandateConstraints(
                mandateId,
                MandateType.Sweeping);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [MemberData(nameof(MandateTestCases.CreateTestSweepingPreselectedMandateRequests), MemberType = typeof(MandateTestCases))]
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
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Theory]
        [MemberData(nameof(MandateTestCases.CreateTestSweepingPreselectedMandateRequests), MemberType = typeof(MandateTestCases))]
        public async Task Can_Create_Mandate_Payment(CreateMandateRequest mandateRequest)
        {
            // Arrange
            string mandateId = await CreateAuthorizedSweepingMandate(mandateRequest);
            var paymentRequest = MandateTestCases.CreateTestMandatePaymentRequest(mandateRequest, mandateId, false);

            // Act
            var response = await _fixture.TlClients[0].Payments.CreatePayment(
                paymentRequest,
                idempotencyKey: Guid.NewGuid().ToString());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Data.IsT1.Should().BeTrue();
            response.Data.AsT1.Id.Should().NotBeNullOrWhiteSpace();
            response.Data.AsT1.ResourceToken.Should().NotBeNullOrWhiteSpace();
            response.Data.AsT1.User.Should().NotBeNull();
            response.Data.AsT1.User.Id.Should().NotBeNullOrWhiteSpace();
            response.Data.AsT1.Status.Should().Be("authorized");
        }

        private async Task<string> CreateAuthorizedSweepingMandate(CreateMandateRequest mandateRequest)
        {
            var client = _fixture.TlClients[0];
            var createResponse = await client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            var mandateId = createResponse.Data!.Id;

            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

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

            getMandateResponse.IsSuccessful.Should().BeTrue();
            return getMandateResponse.Data;
        }
    }
}
