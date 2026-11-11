namespace TrueLayer.Payments.Model;

/// <summary>
/// Indicates whether the user is present when a mandate payment is created.
/// </summary>
public enum UserInteraction
{
    /// <summary>
    /// The user is present when the payment is created
    /// </summary>
    Present,

    /// <summary>
    /// The user is not present when the payment is created
    /// </summary>
    NotPresent
}
