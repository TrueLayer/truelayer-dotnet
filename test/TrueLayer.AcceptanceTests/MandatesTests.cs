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
    using System.Net.Http.Headers;

    public class MandatesTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;
        private TrueLayerOptions configuration;
        public string RETURN_URI = "http://localhost:3000/callback";
        public static string PROVIDER_ID = "ob-uki-mock-bank-sbox"; // Beta provider in closed access, requires a whitelisted ClientId.

        public MandatesTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
            configuration = fixture.ServiceProvider.GetRequiredService<IOptions<TrueLayerOptions>>().Value;
        }

        private static IEnumerable<CreateMandateRequest[]> CreateTestPreselectedMandateRequests()
        {
            var accountIdentifier = new AccountIdentifier.SortCodeAccountNumber("140662", "10003957");
            yield return new CreateMandateRequest[]
            {
                CreateTestMandateRequest(MandateUnion.FromT1(new Mandate.VRPSweepingMandate(
                    "sweeping",
                    ProviderUnion.FromT1(new Mandates.Model.Provider.Preselected("preselected", PROVIDER_ID)),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "Bob NET SDK",
                        AccountIdentifierUnion.FromT0(accountIdentifier))))),
            };
            //yield return new CreateMandateRequest[]
            //{
            //    CreateTestMandateRequest(MandateUnion.FromT0(new Mandate.VRPCommercialMandate(
            //        "commercial",
            //        ProviderUnion.FromT1(new Mandates.Model.Provider.Preselected("preselected", PROVIDER_ID)),
            //        new Mandates.Model.Beneficiary.ExternalAccount(
            //            "external_account",
            //            "My Bank Account",
            //            AccountIdentifierUnion.FromT0(accountIdentifier))))),
            //};
        }

        [Theory]
        [MemberData(nameof(CreateTestUserSelectedMandateRequests))]
        [MemberData(nameof(CreateTestPreselectedMandateRequests))]
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
        [MemberData(nameof(CreateTestUserSelectedMandateRequests))]
        [MemberData(nameof(CreateTestPreselectedMandateRequests))]
        public async Task Can_get_mandate(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            var mandateId = createResponse.Data!.Id;

            // Act
            var response = await _fixture.Client.Mandates.GetMandate(mandateId, MandateType.Sweeping);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Data.AsT0.User!.Id.ShouldBe(createResponse.Data.User!.Id);
        }

        [Theory]
        [MemberData(nameof(CreateTestUserSelectedMandateRequests))]
        [MemberData(nameof(CreateTestPreselectedMandateRequests))]
        public async Task Can_list_mandate(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

            // Act
            var response = await _fixture.Client.Mandates.ListMandates(new ListMandatesQuery(createResponse.Data!.User.Id, null, 10), MandateType.Sweeping);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            response.Data!.Items.Count().ShouldBeLessThanOrEqualTo(10);
        }

        [Theory]
        [MemberData(nameof(CreateTestPreselectedMandateRequests))]
        public async Task Can_start_authorization(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            var mandateId = createResponse.Data!.Id;
            StartAuthorizationFlowRequest authorizationRequest = new(
                new ProviderSelectionRequest(),
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

        [Theory]
        [MemberData(nameof(CreateTestUserSelectedMandateRequests))]
        public async Task Can_submit_provider_selection(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            var mandateId = createResponse.Data!.Id;
            SubmitProviderSelectionRequest request = new(PROVIDER_ID);
            StartAuthorizationFlowRequest authorizationRequest = new(
                new ProviderSelectionRequest(),
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

        [Theory]
        [MemberData(nameof(CreateTestPreselectedMandateRequests))]
        public async Task Can_Get_Funds(CreateMandateRequest mandateRequest)
        {
            // Arrange
            var createResponse = await _fixture.Client.Mandates.CreateMandate(
                mandateRequest, idempotencyKey: Guid.NewGuid().ToString());
            var mandateId = createResponse.Data!.Id;
            StartAuthorizationFlowRequest authorizationRequest = new(
                new ProviderSelectionRequest(),
                new Redirect(new Uri(RETURN_URI)));

            // Act
            var response = await _fixture.Client.Mandates.StartAuthorizationFlow(
                mandateId, authorizationRequest, idempotencyKey: Guid.NewGuid().ToString());
            await AuthorizeMandate(response);
            await WaitForMandateToBeAuthorized(mandateId);
            var fundsResponse = await _fixture.Client.Mandates.GetConfirmationOfFunds(mandateId, 1, "GBP");

            // Assert
            fundsResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            fundsResponse.Data!.ConfirmedAt.Date.ShouldBe(DateTime.UtcNow.Date);
            fundsResponse.Data!.Confirmed.ShouldBeTrue();
        }

        private static CreateMandateRequest CreateTestMandateRequest(
            MandateUnion mandate,
            string currency = Currencies.GBP)
            => new(
                mandate,
                currency,
                new Constraints(
                    MaximumIndividualAmount: 1000,
                    new PeriodicLimits(Month: new Limit(2000, PeriodAlignment.Calendar))),
                new PaymentUserRequest(
                    id: "f9b48c9d-176b-46dd-b2da-fe1a2b77350c",
                    name: "Remi Terr",
                    email: "remi.terr@example.com",
                    phone: "+44777777777"),
                Metadata: new Dictionary<string, string> { { "a_custom_key", "a-custom-value" } });

        private async Task AuthorizeMandate(AuthorizationResponseUnion authorizationFlowResponse)
        {
            var handler = new HttpClientHandler { AllowAutoRedirect = false };
            var client = new HttpClient(handler);

            var redirectUri = authorizationFlowResponse.AsT0.AuthorizationFlow.Actions.Next.AsT4.Uri;
            var redirectResponse = await client.GetAsync(redirectUri);
            var paymentsSpaRedirectUrl = redirectResponse.Headers.Location;

            var isFragment = paymentsSpaRedirectUrl?.Fragment is not null;
            var rawParameters = isFragment ? paymentsSpaRedirectUrl?.Fragment : paymentsSpaRedirectUrl?.Query;
            var sanitizedParameters = rawParameters?.Replace("state=mandate-", "state=");

            var jsonPayload = isFragment
                ? "{\"fragment\":\"" + sanitizedParameters + "\"}"
                : "{\"query\":\"" + sanitizedParameters + "\"}";

            var authUri = new Uri($"{configuration.Payments?.Uri}spa/payments-provider-return");

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var submitProviderParamsResponse =
                await client.PostAsync(
                    authUri,
                    new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

            submitProviderParamsResponse.IsSuccessStatusCode.ShouldBeTrue();
        }

        private async Task<MandateDetailUnion> WaitForMandateToBeAuthorized(string mandateId)
        {
            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(1000);
                var mandate = await _fixture.Client.Mandates.GetMandate(mandateId, MandateType.Sweeping);
                if (mandate.Data.AsT2.Status == "authorized")
                {
                    return mandate;
                }
            }
            return await _fixture.Client.Mandates.GetMandate(mandateId, MandateType.Sweeping);
        }

        private static IEnumerable<CreateMandateRequest[]> CreateTestUserSelectedMandateRequests()
        {
            var accountIdentifier = new AccountIdentifier.SortCodeAccountNumber("140662", "10003957");
            yield return new CreateMandateRequest[]
            {
                CreateTestMandateRequest(MandateUnion.FromT1(new Mandate.VRPSweepingMandate(
                    "sweeping",
                    ProviderUnion.FromT0(new Payments.Model.Provider.UserSelected
                    {
                        Filter = new ProviderFilter {Countries = new[] {"GB"}, ReleaseChannel = "general_availability"},
                    }),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "My Bank Account",
                        AccountIdentifierUnion.FromT0(accountIdentifier))))),
            };
            //yield return new CreateMandateRequest[]
            //{
            //    CreateTestMandateRequest(MandateUnion.FromT0(new Mandate.VRPCommercialMandate(
            //        "commercial",
            //        ProviderUnion.FromT0(new Payments.Model.Provider.UserSelected
            //        {
            //            Filter = new ProviderFilter {Countries = new[] {"GB"}, ReleaseChannel = "private_beta"},
            //        }),
            //        new Mandates.Model.Beneficiary.ExternalAccount(
            //            "external_account",
            //            "My Bank Account",
            //            AccountIdentifierUnion.FromT0(accountIdentifier))))),
            //};
        }
    }
}
