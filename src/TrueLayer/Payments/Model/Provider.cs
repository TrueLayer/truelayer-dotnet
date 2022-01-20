using TrueLayer.Serialization;

namespace TrueLayer.Payments.Model
{
    /// <summary>
    /// Provider types
    /// </summary>
    public static class Provider
    {
        /// <summary>
        /// Represents provider options for 'submit provider selection' action
        /// </summary>
        [JsonDiscriminator("user_selection")]
        public record UserSelection : IDiscriminated
        {
            /// <summary>
            /// Gets the provider type
            /// </summary>
            public string Type => "user_selection";

            /// <summary>
            /// Gets or sets the filter used to determine the banks that should be displayed on the bank selection screen
            /// </summary>
            public ProviderFilter? Filter { get; init; }
        }
    }
}
