namespace TrueLayer.Payments.Model
{
    /// <summary>
    /// Represents an end user
    /// </summary>
    public record PaymentUser(string Id, string? Name = null, string? Email = null, string? Phone = null);
}
