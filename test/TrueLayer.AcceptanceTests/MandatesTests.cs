using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using OneOf;
using Shouldly;
using TrueLayer.Mandates.Model;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.AcceptanceTests
{
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using TrueLayer.Models;
    using ProviderUnion = OneOf<Payments.Model.Provider.UserSelected, Mandates.Model.Provider.Preselected>;
    using MandateUnion = OneOf<Mandate.VRPCommercialMandate, Mandate.VRPSweepingMandate>;
    using AccountIdentifierUnion = OneOf<
        AccountIdentifier.SortCodeAccountNumber,
        AccountIdentifier.Iban,
        AccountIdentifier.Bban,
        AccountIdentifier.Nrb>;
    using MandateDetailUnion = OneOf<MandateDetail.AuthorizationRequiredMandateDetail, MandateDetail.AuthorizingMandateDetail, MandateDetail.AuthorizedMandateDetail, MandateDetail.FailedMandateDetail, MandateDetail.RevokedMandateDetail>;
    using AuthorizationResponseUnion = OneOf<Models.AuthorisationFlowResponse.AuthorizationFlowAuthorizing, Models.AuthorisationFlowResponse.AuthorizationFlowAuthorizationFailed>;

    public class MandatesTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;
        private TrueLayerOptions configuration;
        public string RETURN_URI = "http://localhost:3000/callback";
        public static string PROVIDER_ID = "ob-natwest-vrp-sandbox";

        public MandatesTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
            configuration = fixture.ServiceProvider.GetRequiredService<IOptions<TrueLayerOptions>>().Value;
        }

        private static CreateMandateRequest CreateTestMandateRequest(
            MandateUnion mandate,
            string currency = Currencies.GBP)
            => new(
                mandate,
                currency,
                new Constraints(
                    MaximumIndividualAmount: 100,
                    new PeriodicLimits(Week: new Limit(1000, PeriodAlignment.Calendar)),
                    ValidFrom: DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ValidTo: DateTime.UtcNow.AddMonths(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")),
                new PaymentUserRequest(
                    id: "f9b48c9d-176b-46dd-b2da-fe1a2b77350c",
                    name: "Remi Terr",
                    email: "remi.terr@example.com",
                    phone: "+44777777777"));

        private static IEnumerable<object[]> CreateTestMandateRequests()
        {
            var accountIdentifier = new AccountIdentifier.SortCodeAccountNumber("111111", "10001000");
            /*yield return new object[]
            {
                CreateTestMandateRequest(MandateUnion.FromT1(new Mandate.VRPSweepingMandate(
                    "sweeping",
                    ProviderUnion.FromT0(new Payments.Model.Provider.UserSelected
                    {
                        Filter = new ProviderFilter {Countries = new[] {"GB"}, ReleaseChannel = "private_beta"},
                    }),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "My Bank Account",
                        AccountIdentifierUnion.FromT0(accountIdentifier))))),
            };
            yield return new object[]
            {
                CreateTestMandateRequest(MandateUnion.FromT0(new Mandate.VRPCommercialMandate(
                    "commercial",
                    ProviderUnion.FromT0(new Payments.Model.Provider.UserSelected
                    {
                        Filter = new ProviderFilter {Countries = new[] {"GB"}, ReleaseChannel = "private_beta"},
                    }),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "My Bank Account",
                        AccountIdentifierUnion.FromT0(accountIdentifier))))),
            };*/
            yield return new object[]
            {
                CreateTestMandateRequest(MandateUnion.FromT1(new Mandate.VRPSweepingMandate(
                    "sweeping",
                    ProviderUnion.FromT1(new Mandates.Model.Provider.Preselected("preselected", PROVIDER_ID)),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "My Bank Account",
                        AccountIdentifierUnion.FromT0(accountIdentifier))))),
            };
            yield return new object[]
            {
                CreateTestMandateRequest(MandateUnion.FromT0(new Mandate.VRPCommercialMandate(
                    "commercial",
                    ProviderUnion.FromT1(new Mandates.Model.Provider.Preselected("preselected", PROVIDER_ID)),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "My Bank Account",
                        AccountIdentifierUnion.FromT0(accountIdentifier))))),
            };
        }

        [Theory]
        [MemberData(nameof(CreateTestMandateRequests))]
        public async Task Can_create_mandate(CreateMandateRequest mandateRequest)
        {
            // Act
            var response = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            response.Data!.User.Id.ShouldBe(mandateRequest.User!.Id);
        }


        [Theory]
        [MemberData(nameof(CreateTestMandateRequests))]
        public async Task Can_get_mandate(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            var mandateId = createResponse.Data!.Id;
            // Act
            var response = await _fixture.Client.Mandates.GetMandate(mandateId, MandateType.sweeping);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Data.AsT0.User!.Id.ShouldBe(createResponse.Data.User!.Id);
            createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Theory]
        [MemberData(nameof(CreateTestMandateRequests))]
        public async Task Can_list_mandate(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            // Act
            var response = await _fixture.Client.Mandates.ListMandates(new ListMandatesQuery(createResponse.Data!.User.Id, null, 10), MandateType.sweeping);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Data!.Items.Count().ShouldBeLessThanOrEqualTo(10);
            createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Theory]
        [MemberData(nameof(CreateTestMandateRequests))]
        public async Task Can_start_authorization(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            var mandateId = createResponse.Data!.Id;
            StartAuthorizationFlowRequest authorizationRequest = new(
                new ProviderSelection(ConfigurationStatus.Supported),
                new Redirect(new Uri(RETURN_URI)));
            // Act
            var response = await _fixture.Client.Mandates.StartAuthorizationFlow(
                mandateId, authorizationRequest, idempotencyKey: Guid.NewGuid().ToString());
            await AuthorizeMandate(response);
            var mandate = await WaitForMandateToBeAuthorized(mandateId);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            mandate.AsT2.Status.ShouldBe("authorized");
            createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        private async Task AuthorizeMandate(AuthorizationResponseUnion authorizationFlowResponse)
        {
            var uri = authorizationFlowResponse.AsT0.AuthorizationFlow.Actions.Next.AsT4.Uri;
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Referrer = uri;
            var response = await client.GetAsync(uri);
            var paymentsSpaRedirectUrl = response.Headers.Location;
            bool isQuery = paymentsSpaRedirectUrl?.Query is not null;
            var rawParameters = isQuery ? paymentsSpaRedirectUrl?.Query : paymentsSpaRedirectUrl?.Fragment;
            var sanitizedParameters = rawParameters?.Replace("state=mandate-", "state=");
            string jsonPayload = isQuery
                ? "{\"query\": \"" + sanitizedParameters + "\"}"
                : "{\"fragment\": \"#" + sanitizedParameters + "\"}";
            var baseUri = configuration.Payments?.Uri;
            client.DefaultRequestHeaders.Clear();
            var submitProviderParamsResponse = await client.PostAsync(
                $"{baseUri}spa/payments-provider-return",
                new StringContent(jsonPayload, Encoding.UTF8, "application/json"));
            submitProviderParamsResponse.IsSuccessStatusCode.ShouldBeTrue();
        }

        private async Task<MandateDetailUnion> WaitForMandateToBeAuthorized(string mandateId)
        {
            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(1000);
                var mandate = await _fixture.Client.Mandates.GetMandate(mandateId, MandateType.sweeping);
                if (mandate.Data.AsT2.Status == "authorized")
                {
                    return mandate;
                }
            }
            return await _fixture.Client.Mandates.GetMandate(mandateId, MandateType.sweeping);
        }

        [Theory]
        [MemberData(nameof(CreateTestMandateRequests))]
        public async Task Can_submit_provider_selection(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            var mandateId = createResponse.Data!.Id;
            SubmitProviderSelectionRequest request = new(PROVIDER_ID);
            StartAuthorizationFlowRequest authorizationRequest = new(
                new ProviderSelection(ConfigurationStatus.Supported),
                new Redirect(new Uri(RETURN_URI)));
            await _fixture.Client.Mandates.StartAuthorizationFlow(
                mandateId, authorizationRequest, idempotencyKey: Guid.NewGuid().ToString());
            // Act
            var response = await _fixture.Client.Mandates.SubmitProviderSelection(
                mandateId, request, idempotencyKey: Guid.NewGuid().ToString());

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        }
    }
}
