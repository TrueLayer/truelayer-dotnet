using System.Text.Json;
using AwesomeAssertions;
using TrueLayer.Payments.Model;
using TrueLayer.Serialization;
using Xunit;

namespace TrueLayer.Tests.Payments;

public class MandatePaymentMethodSerializationTests
{
    [Theory]
    [InlineData(UserInteraction.Present, "present")]
    [InlineData(UserInteraction.NotPresent, "not_present")]
    public void Mandate_payment_method_serializes_user_interaction(UserInteraction userInteraction, string expected)
    {
        var paymentMethod = new CreatePaymentMethod.Mandate("mandate-id", null, null, userInteraction);

        string json = JsonSerializer.Serialize(paymentMethod, SerializerOptions.Default);

        using var document = JsonDocument.Parse(json);
        document.RootElement.GetProperty("user_interaction").GetString().Should().Be(expected);
    }

    [Fact]
    public void Mandate_payment_method_omits_user_interaction_when_not_set()
    {
        var paymentMethod = new CreatePaymentMethod.Mandate("mandate-id", null, null);

        string json = JsonSerializer.Serialize(paymentMethod, SerializerOptions.Default);

        using var document = JsonDocument.Parse(json);
        document.RootElement.TryGetProperty("user_interaction", out _).Should().BeFalse();
    }
}
