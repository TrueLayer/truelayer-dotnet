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
    using ProviderUnion = OneOf<Payments.Model.Provider.UserSelected, Mandates.Model.Provider.Preselected>;
    using MandateUnion = OneOf<Mandate.VRPCommercialMandate, Mandate.VRPSweepingMandate>;
    using AccountIdentifierUnion = OneOf<
        AccountIdentifier.SortCodeAccountNumber,
        AccountIdentifier.Iban,
        AccountIdentifier.Bban,
        AccountIdentifier.Nrb>;

    public class MandatesTests : IClassFixture<ApiTestFixture>
    {
        private readonly ApiTestFixture _fixture;

        public MandatesTests(ApiTestFixture fixture)
        {
            _fixture = fixture;
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
            yield return new object[]
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
            };
            yield return new object[]
            {
                CreateTestMandateRequest(MandateUnion.FromT1(new Mandate.VRPSweepingMandate(
                    "sweeping",
                    ProviderUnion.FromT1(new Mandates.Model.Provider.Preselected("preselected", "ob-natwest-vrp-sandbox")),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "My Bank Account",
                        AccountIdentifierUnion.FromT0(accountIdentifier))))),
            };
            yield return new object[]
            {
                CreateTestMandateRequest(MandateUnion.FromT0(new Mandate.VRPCommercialMandate(
                    "commercial",
                    ProviderUnion.FromT1(new Mandates.Model.Provider.Preselected("preselected", "ob-natwest-vrp-sandbox")),
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
    }
}