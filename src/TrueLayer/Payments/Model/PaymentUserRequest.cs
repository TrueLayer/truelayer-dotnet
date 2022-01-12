namespace TrueLayer.Payments.Model
{
    /// <summary>
    /// Payment user
    /// </summary>
    /// <param name="Id">The user identifier</param>
    /// <param name="Name">The user's name</param>
    /// <param name="Email">The user's email address</param>
    /// <param name="Phone">The user's phone number</param>
    public record PaymentUserRequest(string? Id, string? Name, string? Email, string? Phone);
}
