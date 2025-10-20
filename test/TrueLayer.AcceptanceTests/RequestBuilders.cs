using System.Collections.Generic;
using OneOf;
using TrueLayer.Mandates.Model;
using TrueLayer.Payments.Model;
using static TrueLayer.Payments.Model.CreatePaymentMethod;
using PaymentMandate = TrueLayer.Payments.Model.CreatePaymentMethod.Mandate;

namespace TrueLayer.AcceptanceTests;

using MandateUnion = OneOf<TrueLayer.Mandates.Model.Mandate.VRPCommercialMandate, TrueLayer.Mandates.Model.Mandate.VRPSweepingMandate>;

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
            new PaymentMandate(mandateId, "reference", null),
            mandateRequest.User,
            setRelatedProducts ? new RelatedProducts(new SignupPlus()) : null);

}
