using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model;

public static class RemitterVerification
{
    [JsonDiscriminator("automated")]
    public sealed record Automated: IDiscriminated
    {
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
