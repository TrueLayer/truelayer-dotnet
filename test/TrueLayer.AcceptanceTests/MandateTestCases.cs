using System.Collections.Generic;
using OneOf;
using TrueLayer.Mandates.Model;
using TrueLayer.Payments.Model;

namespace TrueLayer.AcceptanceTests
{
    using AccountIdentifierUnion = OneOf<
        AccountIdentifier.SortCodeAccountNumber,
        AccountIdentifier.Iban,
        AccountIdentifier.Bban,
        AccountIdentifier.Nrb>;
    using MandateUnion = OneOf<Mandate.VRPCommercialMandate, Mandate.VRPSweepingMandate>;
    using ProviderUnion = OneOf<Payments.Model.Provider.UserSelected, Mandates.Model.Provider.Preselected>;

    public static class MandateTestCases
    {
        private static AccountIdentifier.SortCodeAccountNumber accountIdentifier = new("140662", "10003957");
        private const string CommercialProviderId = "mock-payments-gb-redirect";
        private const string ProviderId = "mock-payments-gb-redirect";
        public static CreateMandateRequest CreateTestMandateRequest(
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

        public static CreatePaymentRequest CreateTestMandatePaymentRequest(
            CreateMandateRequest mandateRequest,
            string mandateId,
            bool setRelatedProducts = true)
            => new(
                mandateRequest.Constraints.MaximumIndividualAmount,
                mandateRequest.Currency,
                new PaymentMethod.Mandate(mandateId, "reference", null),
                mandateRequest.User,
                setRelatedProducts ? new RelatedProducts(new SignupPlus()) : null);

        public static IEnumerable<object[]> CreateTestSweepingPreselectedMandateRequests()
        {
            yield return
            [
                CreateTestMandateRequest(MandateUnion.FromT1(new Mandate.VRPSweepingMandate(
                    "sweeping",
                    ProviderUnion.FromT1(new Mandates.Model.Provider.Preselected("preselected", ProviderId)),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "Bob NET SDK",
                        AccountIdentifierUnion.FromT0(accountIdentifier)))))
            ];
        }

        public static IEnumerable<object[]> CreateTestCommercialPreselectedMandateRequests()
        {
            yield return
            [
                CreateTestMandateRequest(MandateUnion.FromT0(new Mandate.VRPCommercialMandate(
                    "commercial",
                    ProviderUnion.FromT1(new Mandates.Model.Provider.Preselected("preselected", CommercialProviderId)),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "My Bank Account",
                        AccountIdentifierUnion.FromT0(accountIdentifier)))))
            ];
        }

        public static IEnumerable<object[]> CreateTestSweepingUserSelectedMandateRequests()
        {
            yield return
            [
                CreateTestMandateRequest(MandateUnion.FromT1(new Mandate.VRPSweepingMandate(
                    "sweeping",
                    ProviderUnion.FromT0(new Payments.Model.Provider.UserSelected
                    {
                        Filter = new ProviderFilter { Countries = ["GB"], ReleaseChannel = "alpha" },
                    }),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "My Bank Account",
                        AccountIdentifierUnion.FromT0(accountIdentifier)))))
            ];
        }

        public static IEnumerable<object[]> CreateTestCommercialUserSelectedMandateRequests()
        {
            yield return
            [
                CreateTestMandateRequest(MandateUnion.FromT0(new Mandate.VRPCommercialMandate(
                    "commercial",
                    ProviderUnion.FromT0(new Payments.Model.Provider.UserSelected
                    {
                        Filter = new ProviderFilter { Countries = ["GB"], ReleaseChannel = "alpha" },
                    }),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "My Bank Account",
                        AccountIdentifierUnion.FromT0(accountIdentifier)))))
            ];
        }
    }
}
