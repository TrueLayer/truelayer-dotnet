namespace TrueLayer.Payments.Model;

/// <summary>
/// Represents risk assessment information for a payment.
/// </summary>
/// <param name="Segment">Optional risk segment classification.</param>
public record RiskAssessment(string? Segment = null);
