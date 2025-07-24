using System.Collections.Generic;
using OneOf;
using TrueLayer.Common;
using TrueLayer.Mandates.Model;
using TrueLayer.Payments.Model;
using TrueLayer.Payouts.Model;

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

    public static PaymentSubMerchants CreateTestPaymentSubMerchantsBusinessClient()
        => new(new UltimateCounterpartyBusinessClient(
            commercialName: "Test Business Client Ltd",
            mcc: "1234",
            address: new Address("London", "England", "EC1R 4RB", "GB", "123 Business Street"),
            registrationNumber: "12345678"));

    public static PaymentSubMerchants CreateTestPaymentSubMerchantsBusinessDivision()
        => new(new UltimateCounterpartyBusinessDivision(
            id: "test-division-123",
            name: "Test Business Division"));

    public static PayoutSubMerchants CreateTestPayoutSubMerchants()
        => new(new UltimateCounterpartyBusinessClient(
            commercialName: "Test Payout Business Ltd",
            mcc: "5678",
            address: new Address("Manchester", "England", "M1 1AA", "GB", "456 Payout Street"),
            registrationNumber: "87654321"));

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

}
