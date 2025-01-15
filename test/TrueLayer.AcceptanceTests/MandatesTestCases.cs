using OneOf;
using TrueLayer.Mandates.Model;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.AcceptanceTests
{
    using AccountIdentifierUnion = OneOf<
        AccountIdentifier.SortCodeAccountNumber,
        AccountIdentifier.Iban,
        AccountIdentifier.Bban,
        AccountIdentifier.Nrb>;
    using MandateUnion = OneOf<Mandate.VRPCommercialMandate, Mandate.VRPSweepingMandate>;
    using ProviderUnion = OneOf<Payments.Model.Provider.UserSelected, Mandates.Model.Provider.Preselected>;

    public static class MandatesTestCases
    {
        private static readonly AccountIdentifier.SortCodeAccountNumber AccountIdentifier = new("140662", "10003957");
        private const string ProviderId = "mock-payments-gb-redirect";

        public static TheoryData<CreateMandateRequest> CreateTestSweepingPreselectedMandateRequests() =>
            new(
                RequestBuilders.CreateTestMandateRequest(MandateUnion.FromT1(new Mandate.VRPSweepingMandate(
                    "sweeping",
                    ProviderUnion.FromT1(new Mandates.Model.Provider.Preselected("preselected", ProviderId)),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "Bob NET SDK",
                        AccountIdentifierUnion.FromT0(AccountIdentifier))))));

        public static TheoryData<CreateMandateRequest> CreateTestCommercialPreselectedMandateRequests() =>
            new(
                RequestBuilders.CreateTestMandateRequest(MandateUnion.FromT0(new Mandate.VRPCommercialMandate(
                    "commercial",
                    ProviderUnion.FromT1(new Mandates.Model.Provider.Preselected("preselected", ProviderId)),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "My Bank Account",
                        AccountIdentifierUnion.FromT0(AccountIdentifier))))));

        public static TheoryData<CreateMandateRequest> CreateTestSweepingUserSelectedMandateRequests() =>
            new(
                RequestBuilders.CreateTestMandateRequest(MandateUnion.FromT1(new Mandate.VRPSweepingMandate(
                    "sweeping",
                    ProviderUnion.FromT0(new Payments.Model.Provider.UserSelected
                    {
                        Filter = new ProviderFilter { Countries = ["GB"], ReleaseChannel = ReleaseChannels.PrivateBeta },
                    }),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "My Bank Account",
                        AccountIdentifierUnion.FromT0(AccountIdentifier))))));

        public static TheoryData<CreateMandateRequest> CreateTestCommercialUserSelectedMandateRequests() =>
            new(
                RequestBuilders.CreateTestMandateRequest(MandateUnion.FromT0(new Mandate.VRPCommercialMandate(
                    "commercial",
                    ProviderUnion.FromT0(new Payments.Model.Provider.UserSelected
                    {
                        Filter = new ProviderFilter { Countries = ["GB"], ReleaseChannel = ReleaseChannels.PrivateBeta },
                    }),
                    new Mandates.Model.Beneficiary.ExternalAccount(
                        "external_account",
                        "My Bank Account",
                        AccountIdentifierUnion.FromT0(AccountIdentifier))))));
    }
}
