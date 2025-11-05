using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model;

/// <summary>
/// Contains verification types for payout beneficiaries.
/// </summary>
public static class Verification
{
    /// <summary>
    /// Represents automated verification of beneficiary details.
    /// </summary>
    [JsonDiscriminator("automated")]
    public sealed record Automated: IDiscriminated
    {
        /// <summary>
        /// Gets the type discriminator for automated verification.
        /// </summary>
        public string Type => "automated";

        /// <summary>
        /// Enable or disable the verification of the remitter name.
        /// </summary>
        public bool RemitterName { get; init; }

        /// <summary>
        /// Enable or disable the verification of the remitter date of birth.
        /// </summary>
        public bool RemitterDateOfBirth { get; init; }
    }
}
