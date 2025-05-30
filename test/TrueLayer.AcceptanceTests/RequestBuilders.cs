using System.Collections.Generic;
using OneOf;
using TrueLayer.Mandates.Model;
using TrueLayer.Payments.Model;

namespace TrueLayer.AcceptanceTests;

using MandateUnion = OneOf<Mandate.VRPCommercialMandate, Mandate.VRPSweepingMandate>;

public static class RequestBuilders
{
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

    public static CreatePaymentRequest CreateTestPaymentRequestWithSubMerchants(
        string merchantAccountId,
        long amountInMinor = 30000,
        string currency = Currencies.GBP)
        => new(
            amountInMinor,
            currency,
            new PaymentMethod.BankTransfer(
                new TrueLayer.Payments.Model.Provider.Preselected("mock-payments-gb-redirect"),
                new TrueLayer.Payments.Model.Beneficiary.MerchantAccount(merchantAccountId)),
            new PaymentUserRequest(
                id: "f9b48c9d-176b-46dd-b2da-fe1a2b77350c",
                name: "John Test",
                email: "john.test@example.com",
                phone: "+44123456789"),
            metadata: new Dictionary<string, string> { { "test-key", "test-value" } },
            subMerchants: new SubMerchants(new UltimateCounterparty.BusinessDivision("div-test-123", "Test Division")));

}
