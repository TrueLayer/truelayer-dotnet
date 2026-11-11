using System.Text.Json;
using AwesomeAssertions;
using OneOf;
using TrueLayer.Mandates.Model;
using TrueLayer.Serialization;
using Xunit;
using static TrueLayer.Mandates.Model.CreateProviderSelection;

namespace TrueLayer.Tests.Mandates;

using MandateUnion = OneOf<Mandate.VRPCommercialMandate, Mandate.VRPSweepingMandate>;
using ProviderUnion = OneOf<UserSelected, Preselected>;

public class CreateMandateRequestSerializationTests
{
    [Theory]
    [InlineData(CommercialMandateUseCase.Charity, "charity")]
    [InlineData(CommercialMandateUseCase.FinancialServices, "financial_services")]
    [InlineData(CommercialMandateUseCase.Government, "government")]
    [InlineData(CommercialMandateUseCase.Utilities, "utilities")]
    public void Commercial_mandate_serializes_use_case_and_statement_reference(CommercialMandateUseCase useCase, string expectedUseCase)
    {
        var request = new CreateMandateRequest(
            MandateUnion.FromT0(new Mandate.VRPCommercialMandate(
                "commercial",
                useCase,
                ProviderUnion.FromT1(new Preselected("mock-payments-gb-redirect")),
                new Beneficiary.MerchantAccount("merchant_account", "merchant-account-id"),
                StatementReference: "statement-ref")),
            "GBP",
            new Constraints(1000, new PeriodicLimits(Month: new Limit(1000, PeriodAlignment.Calendar))));

        string json = JsonSerializer.Serialize(request, SerializerOptions.Default);

        using var document = JsonDocument.Parse(json);
        var mandate = document.RootElement.GetProperty("mandate");
        mandate.GetProperty("type").GetString().Should().Be("commercial");
        mandate.GetProperty("use_case").GetString().Should().Be(expectedUseCase);
        mandate.GetProperty("statement_reference").GetString().Should().Be("statement-ref");
    }
}
